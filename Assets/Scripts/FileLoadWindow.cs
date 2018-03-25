using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using System.IO;
using UnityEngine.SceneManagement;

public class FileLoadWindow : MonoBehaviour {

	//this variable holds the fileLoadWindowPanel
	public GameObject fileLoadWindow;

	//need a variable for the button which will open the file load window
	public Button openFileLoadWindowButton;

	//button to close the fileLoadWindow
	public Button closeFileLoadWindowButton;

	//variable to hold the fileList
	public GameObject fileList;

	//variable to hold the Load button
	public Button fileLoadYesButton;

	//variable to hold the Delete button
	public Button fileDeleteYesButton;

	//variable to hold the load cancel button
	public Button fileLoadCancelButton;

	//variable to hold the fileListItemPrefab
	public GameObject fileLoadListItemPrefab;

	//variable to hold the file save names
	public string[] fileLoadNames {

		get;
		private set;

	}

	//managers
	private GameObject uiManager;
	private GameManager gameManager;

	//string to hold the currentFile
	private string currentLoadFile;

	//variables to hold the scene indexes
	private int mainMenuSceneIndex = 0;
	private int mainSceneIndex = 1;

	//scene to hold the main scene
	private Scene mainScene;
	private Scene mainMenuScene;

	//event to announce opening FileLoadWindow
	public UnityEvent OnOpenFileLoadWindow;

	//event to announce fileLoadCancel pressed
	public UnityEvent OnFileLoadCancelClicked = new UnityEvent();

	//event and class to announce fileLoadYes clicked
	public FileLoadEvent OnFileLoadYesClicked = new FileLoadEvent();
	public FileLoadEvent OnFileDeleteYesClicked = new FileLoadEvent();
	public FileLoadEvent OnFileConfirmedDelete = new FileLoadEvent();


	public class FileLoadEvent : UnityEvent<string>{};

	//unityActions
	private UnityAction<FileLoadListItem> loadListItemSetCurrentFileAction;
	private UnityAction<string> stringDeleteFileAction;
	private UnityAction<GameManager.ActionMode> actionModeSetButtonStatusAction;
	private UnityAction<FileManager.SaveGameData> saveGameDataSetButtonStatusAction;
	private UnityAction blankSetButtonStatusAction;
	private UnityAction blankSetButtonStatusDummyAction;
	private UnityAction<bool> boolSetButtonStatusDummyAction;


	// Use this for initialization
	public void Init () {

		//get the managers
		uiManager = GameObject.FindGameObjectWithTag("UIManager");

		//get the main scene
		mainScene = SceneManager.GetSceneByBuildIndex(mainSceneIndex);
		mainMenuScene = SceneManager.GetSceneByBuildIndex(mainMenuSceneIndex);

		//check if we are in the main scene
		if (SceneManager.GetActiveScene() == mainScene) {

			gameManager = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameManager> ();

		}

		//set the actions
		loadListItemSetCurrentFileAction = (fileName) => {SetCurrentLoadFile(fileName);};
		stringDeleteFileAction = (fileName) => {ResolveConfirmedDelete (fileName);};
		actionModeSetButtonStatusAction = (actionMode) => {SetLoadButtonStatus (actionMode);};

		//note that this assumes gameManager is not null!  This works because this action can only come from the main scene
		saveGameDataSetButtonStatusAction = (saveGameData) => {SetLoadButtonStatus (gameManager.CurrentActionMode);};

		//note that this assumes gameManager is not null!  This works because this action can only come from the main scene
		blankSetButtonStatusAction = () => {SetLoadButtonStatus (gameManager.CurrentActionMode);};
		blankSetButtonStatusDummyAction = () => {SetLoadButtonStatus (GameManager.ActionMode.Selection);};
		boolSetButtonStatusDummyAction = (passedBool) => {SetLoadButtonStatus (GameManager.ActionMode.Selection);};

		//add event listeners
		AddEventListeners();

		//close the window
		CloseFileLoadWindow ();

	}

	//this function adds event listeners
	private void AddEventListeners(){

		//check if we are in the MainMenuScene
		if (SceneManager.GetActiveScene () == mainMenuScene) {

			//add listener for scene startup
			uiManager.GetComponent<MainMenuManager>().OnSceneStartup.AddListener(boolSetButtonStatusDummyAction);

			//add listener for delete file
			uiManager.GetComponent<MainMenuManager>().OnDeleteFile.AddListener(blankSetButtonStatusDummyAction);

		}

		//check if we are in the MainScene
		if (SceneManager.GetActiveScene () == mainScene) {

			//add listener for scene startup
			gameManager.OnSceneStartup.AddListener(blankSetButtonStatusAction);

			//add listener for saving a game
			uiManager.GetComponent<FileManager>().OnSendSaveGameDataFromSave.AddListener(saveGameDataSetButtonStatusAction);

			//add a listener for autosaving a game
			uiManager.GetComponent<FileManager>().OnAutosaveGame.AddListener(blankSetButtonStatusAction);

			//add a listener for deleting a game
			uiManager.GetComponent<FileManager>().OnDeleteGame.AddListener(blankSetButtonStatusAction);

		}
			
		//add a listener for clicking the load open window button
		openFileLoadWindowButton.onClick.AddListener(OpenFileLoadWindow);

		//add a listener for clicking the close window X button
		closeFileLoadWindowButton.onClick.AddListener(ClickedFileLoadCancel);

		//add a listener for clicking the cancel load button
		fileLoadCancelButton.onClick.AddListener(ClickedFileLoadCancel);

		//add a listener for clicking the yes load button
		fileLoadYesButton.onClick.AddListener(ClickedFileLoadYes);

		//add a listener for clicking the yes delete button
		fileDeleteYesButton.onClick.AddListener(ClickedFileDeleteYes);

		//add a listener for the delete prompt yes button
		uiManager.GetComponent<FileDeletePrompt>().OnFileDeleteYesClicked.AddListener(stringDeleteFileAction);

		//add a listener for a load file being selected
		FileLoadListItem.OnLoadFileSelected.AddListener(loadListItemSetCurrentFileAction);

		if (gameManager != null) {

			//add a listener for action mode changes
			gameManager.OnActionModeChange.AddListener (actionModeSetButtonStatusAction);

		}
			
	}


	//this function opens the fileLoadWindow
	private void OpenFileLoadWindow(){

		//enable the window object
		fileLoadWindow.SetActive(true);

		//when we open the window, we want to get the existing load files
		GetExistingLoadFiles();

		//start with the fileLoadYesButton not interactable
		fileLoadYesButton.interactable = false;

		//start with the fileDeleteYesButton not interactable
		fileDeleteYesButton.interactable = false;

		//invoke the open window event
		OnOpenFileLoadWindow.Invoke ();

	}

	//this function opens the fileLoadWindow
	private void CloseFileLoadWindow(){

		//disable the window object
		fileLoadWindow.SetActive(false);

		//clear the file list
		ClearFileNames();

	}

	//this function gets the existing load files
	private void GetExistingLoadFiles(){

		//get an array of strings for all the exising save files
		string[] existingLoads = Directory.GetFiles (FileManager.FileSaveBasePath ());

		//store the existing saves
		fileLoadNames = existingLoads;

		//trim the fileSaveNames
		for (int i = 0; i < fileLoadNames.Length; i++) {

			fileLoadNames [i] = Path.GetFileNameWithoutExtension (fileLoadNames [i]);

		}

		//now for each file name, we want to create a fileList object
		foreach (string loadFileName in existingLoads) {

			//instantiate an object from the prefab
			GameObject fileLoadListItem = (GameObject)Instantiate(fileLoadListItemPrefab);

			//run the fileListItem Init()
			fileLoadListItem.GetComponent<FileLoadListItem>().Init();

			//Set the parent of the new fileListItem
			fileLoadListItem.transform.SetParent(fileList.transform);

			//set the scale of the new item
			fileLoadListItem.transform.localScale = Vector3.one;

			//set the text in the fileListItem to match the fileName
			fileLoadListItem.GetComponentInChildren<TextMeshProUGUI>().text = (Path.GetFileNameWithoutExtension(loadFileName));

		}

	}

	//this function checks if there are any save files available to load
	private bool FilesAvailableToLoad(){

		//check if the saves directory exists
		if (Directory.Exists (FileManager.FileSaveBasePath ())) {

			//Debug.Log ("save directory exists");

			//get an array of strings for all the exising save files
			string[] existingLoads = Directory.GetFiles (FileManager.FileSaveBasePath ());

			//check if existingLoads is null
			if (existingLoads == null || existingLoads.Length == 0) {

				//Debug.Log ("no files");

				//there are no files
				return false;

			} else {

				//Debug.Log ("files");

				//there are files
				return true;

			}

		} else {

			//Debug.Log ("save directory doesn't exist");

			//the save directory doesn't even exist, there are no files
			return false;

		}

	}

	//this function clears out file names
	private void ClearFileNames(){

		//destroy all children of the filelist
		for(int i = 0; i < fileList.transform.childCount; i++){

			GameObject.Destroy(fileList.transform.GetChild (i).gameObject);

		}

	}

	//this function handles clicking the FileLoadYesButton
	private void ClickedFileLoadYes(){

		//close the window
		CloseFileLoadWindow ();

		//invoke the load button event
		OnFileLoadYesClicked.Invoke (currentLoadFile);

	}

	//this function handles clicking the FileDeleteYesButton
	private void ClickedFileDeleteYes(){

		//invoke the delete button event
		OnFileDeleteYesClicked.Invoke (currentLoadFile);

	}

	//this function handles clicking the FileLoadCancelButton
	private void ClickedFileLoadCancel(){

		//close the window
		CloseFileLoadWindow ();

		//invoke the cancel button event
		OnFileLoadCancelClicked.Invoke ();

	}

	//this function handles a confirmed delete
	private void ResolveConfirmedDelete(string fileName){

		//invoke the delete item action
		OnFileConfirmedDelete.Invoke(fileName);

		//clear the active file
		currentLoadFile = null;

		//clear the current load files
		ClearFileNames();

		//refresh the current load files
		GetExistingLoadFiles();

		//start with the fileLoadYesButton not interactable
		fileLoadYesButton.interactable = false;

		//start with the fileDeleteYesButton not interactable
		fileDeleteYesButton.interactable = false;

	}

	//function to set the currentLoadFile
	private void SetCurrentLoadFile(FileLoadListItem fileLoadListItem){

		//set the fileName
		currentLoadFile = fileLoadListItem.transform.GetComponentInChildren<TextMeshProUGUI> ().text;

		//make the load button interactable
		fileLoadYesButton.interactable = true;

		//make the delete button interactable
		fileDeleteYesButton.interactable = true;

	}

	//this function sets the load button status
	private void SetLoadButtonStatus(GameManager.ActionMode actionMode){

		//check if we are in a locked action mode
		if (UIManager.lockMenuActionModes.Contains (actionMode)) {

			openFileLoadWindowButton.interactable = false;

		} else {

			//check if there are files available to load
			if (FilesAvailableToLoad () == false) {

				//if there are no files, disable the load button
				openFileLoadWindowButton.interactable = false;

			} else{
				
				//if the mode isn't locked, and there are files available, the button can be interactable
				openFileLoadWindowButton.interactable = true;

			}

		}

	}

	//this function handles OnDestroy
	private void OnDestroy(){

		RemoveAllListeners ();

	}

	//this function removes all listeners
	private void RemoveAllListeners(){

		if (uiManager.GetComponent<MainMenuManager> () != null) {
			
			//remove listener for scene startup
			uiManager.GetComponent<MainMenuManager> ().OnSceneStartup.RemoveListener (boolSetButtonStatusDummyAction);


			//remove listener for delete file
			uiManager.GetComponent<MainMenuManager>().OnDeleteFile.RemoveListener(blankSetButtonStatusDummyAction);

		}

		if (uiManager.GetComponent<FileManager> () != null) {
			
			//remove listener for saving a game
			uiManager.GetComponent<FileManager> ().OnSendSaveGameDataFromSave.RemoveListener (saveGameDataSetButtonStatusAction);

			//remove a listener for autosaving a game
			uiManager.GetComponent<FileManager> ().OnAutosaveGame.RemoveListener (blankSetButtonStatusAction);

			//remove a listener for deleting a game
			uiManager.GetComponent<FileManager> ().OnDeleteGame.RemoveListener (blankSetButtonStatusAction);

		}

		if (openFileLoadWindowButton != null) {
			
			//remove a listener for clicking the load open window button
			openFileLoadWindowButton.onClick.RemoveListener (OpenFileLoadWindow);

		}

		if (closeFileLoadWindowButton != null) {
			
			//remove a listener for clicking the close window X button
			closeFileLoadWindowButton.onClick.RemoveListener (ClickedFileLoadCancel);

		}

		if (fileLoadCancelButton != null) {
			
			//remove a listener for clicking the cancel load button
			fileLoadCancelButton.onClick.RemoveListener (ClickedFileLoadCancel);

		}

		if (fileLoadYesButton != null) {
			
			//remove a listener for clicking the yes load button
			fileLoadYesButton.onClick.RemoveListener (ClickedFileLoadYes);

		}

		if (fileDeleteYesButton != null) {
			
			//remove a listener for clicking the yes delete button
			fileDeleteYesButton.onClick.RemoveListener (ClickedFileDeleteYes);

		}

		if (uiManager.GetComponent<FileDeletePrompt> () != null) {
			
			//remove a listener for the delete prompt yes button
			uiManager.GetComponent<FileDeletePrompt> ().OnFileDeleteYesClicked.RemoveListener (stringDeleteFileAction);

		}

		//remove a listener for a load file being selected
		FileLoadListItem.OnLoadFileSelected.RemoveListener(loadListItemSetCurrentFileAction);


		if (gameManager != null) {

			//remove a listener for action mode changes
			gameManager.OnActionModeChange.RemoveListener(actionModeSetButtonStatusAction);

			//remove listener for scene startup
			gameManager.OnSceneStartup.RemoveListener(blankSetButtonStatusAction);

		}

	}

}