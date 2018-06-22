using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.Events;
//using UnityEditor;

public class MainMenuManager : MonoBehaviour {

	//gameObject for UIManagerMainMenu
	public GameObject uiManagerMainMenu;

    //networkManager
    private GameObject networkManager;

	//game object to hold the event system
	public GameObject eventSystem;

	//game object to hold the main menu button panel
	public GameObject mainMenuButtonsPanel;

	//enum to track type of game
	public enum GameType{

		NewLocalGame,
		LoadLocalGame,
		NewLANGame,
        LoadLANGame,
        NewInternetGame,
        LoadInternetGame,

	}

	//variable to track the game type
	public GameType gameType {

		get;
		private set;

	}

	//variable to hold the main scene
	private int mainMenuSceneIndex = 0;
	private int mainSceneIndex = 1;

	//variable to hold the local game to load
	public string localGameToLoad {

		get;
		private set;

	}

	//bool to keep track of whether this is the initial startup
	bool isInitialStartup = false;

	//event for startup
	public SceneStartupEvent OnSceneStartup = new SceneStartupEvent();

	public class SceneStartupEvent : UnityEvent<bool>{};

	//event for scene exit
	public UnityEvent OnBeginSceneExit = new UnityEvent();
	public UnityEvent OnDeleteFile = new UnityEvent();


	//unityActions
	private UnityAction<string> stringDeleteFileAction;
	private UnityAction exitGameAction;
	private UnityAction<string> stringLoadFileAction;
    private UnityAction<LANConnectionInfo> connectionNewLANGameAction;


	// Use this for initialization
	void Start () {

		//define the scenes
		Scene mainScene = SceneManager.GetSceneByBuildIndex (mainSceneIndex);
		Scene mainMenuScene = SceneManager.GetSceneByBuildIndex (mainMenuSceneIndex);

		//set the main scene as the active scene
		SceneManager.SetActiveScene (mainMenuScene);

		//check if the main game scene is loaded
		//check if the main menu scene is loaded
		if (mainScene.isLoaded == true) {

			//get the main scene UIManager
			GameObject mainUIManager = GameObject.Find ("UIManager");

			//if it is, we need to destroy its UIManager so we don't think it's this scene's UI manager
			DestroyImmediate (mainUIManager);

			//destroying the canvas will make the transition between scenes look better
			DestroyImmediate (GameObject.Find ("Canvas"));

			//we also need to destroy it's main camera
			Destroy (Camera.main.gameObject);

			//now we can start unloading the main menu scene
			SceneManager.UnloadSceneAsync (mainScene);

			//set the initialStartupFlag
			isInitialStartup = false;

		} else {

			//the else condition is that this is the initial startup, since the main scene is not loaded
			isInitialStartup = true;

		}

		//get the manager
		uiManagerMainMenu = GameObject.FindGameObjectWithTag("UIManager");
        networkManager = GameObject.FindGameObjectWithTag("NetworkManager");


        //initialize scripts
        InitializeScripts();

		//set actions
		SetActions();

		//add listeners
		AddEventListeners();

		//invoke the scene startup event
		OnSceneStartup.Invoke (isInitialStartup);

	}

	//this function initializes mainMenu scripts
	private void InitializeScripts(){

		//make the main ui panels fit
		uiManagerMainMenu.GetComponent<UIMainPanelFitter>().Init();

		uiManagerMainMenu.GetComponent<FileLoadWindow> ().Init ();
		uiManagerMainMenu.GetComponent<FileDeletePrompt> ().Init ();
		uiManagerMainMenu.GetComponent<PauseFadePanel2> ().Init ();
		uiManagerMainMenu.GetComponent<ExitGamePrompt> ().Init ();
		uiManagerMainMenu.GetComponent<PauseFadePanelMainMenu> ().Init ();
		uiManagerMainMenu.GetComponent<ConfigureLocalGameWindow> ().Init ();
		uiManagerMainMenu.GetComponent<SceneTransitionFadePanel> ().Init ();

		//this needs to be before settings since it is listening to settings
		uiManagerMainMenu.GetComponent<SoundManagerMainMenu> ().Init ();


		uiManagerMainMenu.GetComponent<Settings> ().Init ();
		uiManagerMainMenu.GetComponent<UINavigationMainMenu> ().Init ();
		uiManagerMainMenu.GetComponent<About> ().Init ();
        uiManagerMainMenu.GetComponent<NewLANGameWindow>().Init();
        uiManagerMainMenu.GetComponent<LANGameList>().Init();
        uiManagerMainMenu.GetComponent<LobbyLANGamePanel>().Init();

        networkManager.GetComponent<CustomNetworkManager>().Init();
        networkManager.GetComponent<NetworkInterface>().Init();
        networkManager.GetComponent<LocalNetworkDiscovery>().Init();

    }

    //this function sets actions
    private void SetActions(){

		stringDeleteFileAction = (fileName) => {DeleteSaveFile(fileName);};
		exitGameAction = () => {ExitGame ();};
		stringLoadFileAction = (fileName) => {LoadLocalGame(fileName);};
        connectionNewLANGameAction = (connection) => { SetGameTypeToNewLAN(); };

    }

	//this function adds event listeners
	private void AddEventListeners(){

		//add listener for creating a new local game
		uiManagerMainMenu.GetComponent<ConfigureLocalGameWindow>().OnCreateNewGame.AddListener(CreateNewLocalGame);

		//add listener for deleting a save file
		uiManagerMainMenu.GetComponent<FileLoadWindow>().OnFileConfirmedDelete.AddListener(stringDeleteFileAction);

		//add listener for exiting the game
		uiManagerMainMenu.GetComponent<ExitGamePrompt>().OnExitGameYesClicked.AddListener(exitGameAction);

		//add listener for loading a local game
		uiManagerMainMenu.GetComponent<FileLoadWindow>().OnFileLoadYesClicked.AddListener(stringLoadFileAction);

		//add listener for the fade out completing
		uiManagerMainMenu.GetComponent<SceneTransitionFadePanel>().OnFadeOutComplete.AddListener(LoadMainScene);

		//add listener for the fade in completing
		uiManagerMainMenu.GetComponent<SceneTransitionFadePanel>().OnFadeInComplete.AddListener(EnableMainMenuButtons);

        //add listener for creating a new LAN game
        uiManagerMainMenu.GetComponent<NewLANGameWindow>().OnCreateNewLANGame.AddListener(connectionNewLANGameAction);

	}

	//this function loads my main scene
	private void LoadMainScene(){

		//we want to destroy the event system from this scene before the new scene is loaded
		Destroy (eventSystem);

		//disable the audio listener so when the new scene loads only 1 audio listener is loaded
		Camera.main.GetComponent<AudioListener> ().enabled = false;

		//load the main scene
		SceneManager.LoadScene (mainSceneIndex, LoadSceneMode.Additive);
		//SceneManager.LoadScene (1);

	}

	//this function turns on the main menu buttons
	private void EnableMainMenuButtons(){

		mainMenuButtonsPanel.SetActive (true);

	}

	//this function begins a scene fadeOut
	private void BeginSceneFadeOut(){

		//invoke the exit scene event
		OnBeginSceneExit.Invoke();

	}

	//this function resolves creating a new local game
	private void CreateNewLocalGame(){

		//set the gameType
		gameType = GameType.NewLocalGame;

		//start the fade out
		BeginSceneFadeOut();

	}

	//this function resolves loading a local game
	private void LoadLocalGame(string gameName){

		//set the gameType
		gameType = GameType.LoadLocalGame;

		//store the string
		localGameToLoad = gameName;

		//start the fade out
		BeginSceneFadeOut();

	}

    //this function sets the game type to a new LAN game
    private void SetGameTypeToNewLAN()
    {
        gameType = GameType.NewLANGame;
    }
    
    //this function sets the game type to Load LAN game
    private void SetGameTypeToLoadLAN()
    {
        gameType = GameType.LoadLANGame;
    }
	//this function deletes a file from the saves directory
	private void DeleteSaveFile(string fileName){

		string deleteFileName = fileName;

		//filepath is the full save file
		string filePath = System.IO.Path.Combine(FileManager.FileSaveBasePath(), deleteFileName + ".txt");

		//delete the file
		File.Delete (filePath);

		//invoke the delete file event
		OnDeleteFile.Invoke();

	}

	//this function exits the game application
	private void ExitGame(){

		//quit the application
		Application.Quit ();

		//FIXME - this can be removed from final build
		//EditorApplication.isPlaying = false;

	}

	//this function handles OnDestroy
	private void OnDestroy(){

		RemoveAllListeners ();

	}

	//this function removes all listeners
	private void RemoveAllListeners(){

		if (uiManagerMainMenu != null) {
			
			//remove listener for deleting a save file
			uiManagerMainMenu.GetComponent<FileLoadWindow> ().OnFileConfirmedDelete.RemoveListener (stringDeleteFileAction);

			//remove listener for exiting the game
			uiManagerMainMenu.GetComponent<ExitGamePrompt>().OnExitGameYesClicked.RemoveListener(exitGameAction);

			//remove listener for creating a new local game
			uiManagerMainMenu.GetComponent<ConfigureLocalGameWindow>().OnCreateNewGame.RemoveListener(CreateNewLocalGame);

			//remove listener for loading a local game
			uiManagerMainMenu.GetComponent<FileLoadWindow>().OnFileLoadYesClicked.RemoveListener(stringLoadFileAction);

			//remove listener for the fade out completing
			uiManagerMainMenu.GetComponent<SceneTransitionFadePanel>().OnFadeOutComplete.RemoveListener(LoadMainScene);

			//remove listener for the fade in completing
			uiManagerMainMenu.GetComponent<SceneTransitionFadePanel>().OnFadeInComplete.RemoveListener(EnableMainMenuButtons);

            //remove listener for creating a new LAN game
            uiManagerMainMenu.GetComponent<NewLANGameWindow>().OnCreateNewLANGame.RemoveListener(connectionNewLANGameAction);

        }

	}

}
