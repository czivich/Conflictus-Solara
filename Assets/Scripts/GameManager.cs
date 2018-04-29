using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	//variable to control whether game is being created from load
	public static bool loadedGame {

		get;
		private set;

	}

	//variable to hold saveGameData
	public static FileManager.SaveGameData loadedGameData {

		get;
		private set;

	}

	//variable flag to keep track of whether this is the first turn since new game or load
	public bool firstTurnHasHappened {

		get;
		private set;

	}

	//game object to hold the event system
	public GameObject eventSystem;

	//game object to hold the canvas
	public Canvas canvas;

	//variables to hold prefabs - must be public to hook up in inspector
	public HexCursor prefabHexCursor;
	public Starship prefabStarship;
	public Destroyer prefabDestroyer;
	public BirdOfPrey prefabBirdOfPrey;
	public Scout prefabScout;
	public Starbase prefabStarbase;
	public Player prefabPlayer;

	public SelectionCursor prefabSelectionCursor;
	public TargetingCursor prefabTargetingCursor;
	public TargetingEmblem prefabTargetingEmblem;
	public WormholeRotation prefabWormholeRotation;

	//variables to hold the managers
	private UIManager uiManager;
	private MessageManager messageManager;
	private GraphicsManager graphicsManager;
	private ColonyManager colonyManager;
	private MouseManager mouseManager;
	private SoundManager soundManager;

	//variable to hold the tileMap
	private TileMap tileMap;

	//variables to hold the starbase locations
	private Vector2 RedStarbaseLocation = new Vector2(4, 26);
	private Vector2 BlueStarbaseLocation = new Vector2(4, 4);
	private Vector2 GreenStarbaseLocation = new Vector2(27, 4);
	private Vector2 PurpleStarbaseLocation = new Vector2(27, 26);


	public static Color greenColor = new Color (
		130.0f / 255.0f,
		214.0f / 255.0f,
		130.0f / 255.0f,
		255.0f / 255.0f);

	public static Color purpleColor = new Color (
		180.0f / 255.0f, 
		147.0f / 255.0f, 
		214.0f / 255.0f, 
		255.0f / 255.0f);

	public static Color redColor = new Color (
		214.0f / 255.0f, 
		130.0f / 255.0f, 
		130.0f / 255.0f, 
		255.0f  / 255.0f);

	public static Color blueColor = new Color (
		130.0f / 255.0f, 
		172.0f / 255.0f, 
		214.0f / 255.0f, 
		255.0f / 255.0f);

	//variable to control team settings
	public bool teamsEnabled {

		get;
		private set;

	}

	public const int numberPlayers = 4;

	//variable to hold the maximum number of ships of each class allowed
	public const int maxShipsPerClass = 5;

	//variable to track the number of planets win condition
	public int victoryPlanets {

		get;
		private set;

	}

	//variable to hold the miniMapCamera
	private Camera miniMapCamera;

	//variable to hold the uiSidePanel Camera
	public Camera uiSidePanelCamera;

	public static float mainCameraDefaultSize {

		get;
		private set;

	}

	//need a variable to track if a ship is moving on the map
	public bool aShipIsMoving {

		get;
		private set;

	}

	//create an enum for the turn phases
	public enum TurnPhase {

		PurchasePhase,
		MainPhase,

	}

	//variables to track the turn and phase
	public Player.Color currentTurn {

		get;
		private set;

	}

	public TurnPhase currentTurnPhase {

		get;
		private set;

	}

	//variable to track the game year - the year will increment every cycle of turns
	private int startingGameYear = 2201;
	public int gameYear {

		get;
		private set;

	}

	//variable to track who had the the first turn - needed to increment year correctly
	public Player.Color firstTurn {

		get;
		private set;

	}


	//create variables for the players
	public Player greenPlayer {
		get;
		private set;
	}

	public Player purplePlayer{
		get;
		private set;
	}

	public Player redPlayer{
		get;
		private set;
	}

	public Player bluePlayer{
		get;
		private set;
	}

	//variable to track the current turn player
	public Player currentTurnPlayer {

		get;
		private set;

	}

	//define an enum to track the current game activity so the mouse manager can react accordingly
	public enum ActionMode{

		Selection,
		Movement,
		TractorBeam,
		PhasorAttack,
		TorpedoAttack,
		FlareMode,
		ItemUse,
		Crew,
		Rename,
		EndTurn,
		Cloaking,
		Animation,
		PlaceNewUnit,

	}

	//this variable holds the action mode we were in before going into animation mode
	private ActionMode preAnimationActionMode;

	//create a variable to hold the actionMode
	private ActionMode currentActionMode;
	public ActionMode CurrentActionMode {

		get {

			return currentActionMode;

		}

		private set {

			if (value == currentActionMode) {

				return;

			} else {

				currentActionMode = value;
				//Debug.Log ("Action Mode is " + currentActionMode);
				//invoke the OnChangeActionMode event
				OnActionModeChange.Invoke(currentActionMode);
				//Debug.Log ("Current action mode is " + CurrentActionMode);

			}

		}

	}

	//these variables are for use for the minimap camera size
	public GameObject miniMapBorder;
	private float mapRatio;
	private float aspectRatio;
	private float miniMapScreenPercentBorderOffset = .005f;
	private float miniMapCameraViewportX;
	private float miniMapCameraViewportY;
	private float miniMapCameraViewportWidth;
	private float miniMapCameraViewportHeight;
	private float initialMiniMapBorderYMax;

	//these gameObjects are the unit collectors
	public GameObject greenUnits {get; private set;}
	public GameObject purpleUnits { get; private set;}
	public GameObject redUnits { get; private set;}
	public GameObject blueUnits { get; private set;}

	//this variable is for tracking whether the game has already been won, so that if they keep playing
	//the game won't continuously re-check for victory each year
	//but this isn't saved, so if you load a game that has already been won, it will check the first end of year
	private bool gameHasAlreadyBeenWon;

	//declare public event for ending the turn
	public EndTurnEvent OnEndTurn = new EndTurnEvent();

	//simple class derived from unityEvent to pass Player.Color property
	public class EndTurnEvent : UnityEvent<Player.Color>{};



	//event to announce the start of a new turn
	public TurnEvent OnNewTurn = new TurnEvent();

	//event to announce the resumption of a loaded turn
	public TurnEvent OnLoadedTurn = new TurnEvent();

	//event to announce the start of the main phase
	public TurnEvent OnBeginMainPhase = new TurnEvent();

	//event to announce a new ship was placed
	public TurnEvent OnNewUnitCreated = new TurnEvent();

	//class derived from unityEvent to pass a player object
	public class TurnEvent : UnityEvent<Player>{};


	//event to announce the ActionMode has changed
	public ActionModeEvent OnActionModeChange = new ActionModeEvent();

	//classs derived from unityEvent to pass the action mode
	public class ActionModeEvent : UnityEvent<ActionMode>{};

	//event to fire when all units have been created from load
	public UnityEvent OnAllLoadedUnitsCreated = new UnityEvent();

	//event to fire when players have been created
	public SaveGameDataEvent OnPlayersLoaded = new SaveGameDataEvent ();
	public class SaveGameDataEvent : UnityEvent<FileManager.SaveGameData>{};

	//event to fire to create a colony
	public LoadColonyEvent OnLoadColony = new LoadColonyEvent ();
	public class LoadColonyEvent : UnityEvent<HexMapTile, Player.Color>{};

	//event to load a local game
	public LoadLocalGameEvent OnLoadLocalGame = new LoadLocalGameEvent ();
	public class LoadLocalGameEvent : UnityEvent<string>{};

	//event for scene startup
	public UnityEvent OnSceneStartup = new UnityEvent();

	//event for wanting to black out the screen while objects initialize and load
	public UnityEvent OnStartLoadingSceneStartup = new UnityEvent();

	//event for starting scene exit
	public UnityEvent OnBeginSceneExit = new UnityEvent();

	//event for changing main camera size
	public UnityEvent OnChangedMainCameraSetup = new UnityEvent();

	//event for winning the game as a team
	public TeamVictoryEvent OnTeamVictory = new TeamVictoryEvent ();
	public class TeamVictoryEvent : UnityEvent<Player, Player>{};

	//event for winning the game as an individual
	public SoloVictoryEvent OnSoloVictory = new SoloVictoryEvent ();
	public class SoloVictoryEvent : UnityEvent<Player>{};

	//this event is for recognizing that a player should be dead and removed from the game
	public SoloVictoryEvent OnKillPlayer = new SoloVictoryEvent();

	//unityActions
	private UnityAction<Toggle> toggleSetActionModeToTractorBeamAction;
	private UnityAction tractorBeamSetActionModeToSelectionAction;
	private UnityAction<Toggle> toggleSetActionModeToMovementAction;
	private UnityAction movementSetActionModeToSelectionAction;
	private UnityAction<Toggle> toggleSetActionModeToPhasorAttackAction;
	private UnityAction phasorSetActionModeToSelectionAction;
	private UnityAction<Toggle> toggleSetActionModeToTorpedoAttackAction;
	private UnityAction torpedoSetActionModeToSelectionAction;
	private UnityAction<Toggle> toggleSetActionModeToItemUseAction;
	private UnityAction itemSetActionModeToSelectionAction;
	private UnityAction<Toggle> toggleSetActionModeToCrewAction;
	private UnityAction crewSetActionModeToSelectionAction;
	private UnityAction<Ship> shipSetShipMovingAction;
	private UnityAction<CombatUnit,CombatUnit,CombatManager.ShipSectionTargeted,int> torpedoAttackShipWithFlaresActionModeToFlareAction;
	private UnityAction<CombatUnit,CombatUnit,CombatManager.BaseSectionTargeted,int> torpedoAttackBaseWithFlaresActionModeToFlareAction;
	private UnityAction<FlareManager.FlareEventData> flareDataSetActionModeToTorpedoAction;
	private UnityAction renameSetActionModeToRenameAction;
	private UnityAction<CombatUnit,string,ActionMode> renameUnitSetActionModeToPreviousAction;
	private UnityAction<ActionMode> actionModeToPreviousAction;
	private UnityAction<Toggle> toggleSetActionModeToEndTurnAction;
	private UnityAction endTurnSetActionModeToSelectionAction;
	private UnityAction setActionModeToCloakingAction;
	private UnityAction cloakingSetActionModeToSelectionAction;
	private UnityAction<Ship> shipSetActionModeToAnimationAction;
	private UnityAction<Ship> shipSetActionModeToPreviousAction;
	private UnityAction<Dictionary<string,int>,int,CombatUnit.UnitType> purchaseSetActionModeToPlaceUnitAction;
	private UnityAction setActionModeToSelectionAction;
	private UnityAction<NameNewShip.NewUnitEventData> purchaseSetActionModeToSelectionAction;
	private UnityAction<NameNewShip.NewUnitEventData> purchaseCreateNewUnitAction;
	private UnityAction resolveLoadedGameAction;
	private UnityAction<FileManager.SaveGameData> resolveLoadedGameFromSaveAction;

	//variables to hold the scene indexes
	private int mainMenuSceneIndex = 0;
	private int mainSceneIndex = 1;

	//variables to hold the player names
	private string greenPlayerName;
	private string redPlayerName;
	private string purplePlayerName;
	private string bluePlayerName;

	//string to hold the local game to load
	private string localGameToLoad;

	//flag for loading local game
	private bool loadLocalGame = false;

	//flag for this being a newly loaded scene
	private static bool newScene = false;

	//flag for changed resolution
	private bool changedResolution = false;
	private int changedResolutionFrameDelay = 2;
	private int changedResolutionFrameCounter = 0;

	//main camera
	Camera mainCamera;

	// Use this for initialization
	private void Start () {

		//Debug.Log ("GameManager Start() Called");

		//define the scenes
		Scene mainScene = SceneManager.GetSceneByBuildIndex (mainSceneIndex);
		Scene mainMenuScene = SceneManager.GetSceneByBuildIndex (mainMenuSceneIndex);

		//set the main scene as the active scene
		SceneManager.SetActiveScene (mainScene);

		//get the unit collectors
		//get the collectors
		greenUnits = GameObject.Find ("GreenUnits");
		purpleUnits = GameObject.Find ("PurpleUnits");
		redUnits = GameObject.Find ("RedUnits");
		blueUnits = GameObject.Find ("BlueUnits");

		//check if the main menu scene is loaded
		if (mainMenuScene.isLoaded == true) {

			//set the newScene flag to true
			newScene = true;

			//get the mainMenuUIManager
			GameObject mainMenuUIManager = GameObject.Find("UIManagerMainMenu");

			//do a switch case on the MainMenu gameType
			switch (mainMenuUIManager.GetComponent<MainMenuManager> ().gameType) {

			case MainMenuManager.GameType.NewLocalGame:

				//set the teams enabled flag from the configuration
				teamsEnabled = mainMenuUIManager.GetComponent<ConfigureLocalGameWindow> ().teamsEnabled;

				//set the player names
				greenPlayerName = mainMenuUIManager.GetComponent<ConfigureLocalGameWindow> ().greenPlayerName;
				redPlayerName = mainMenuUIManager.GetComponent<ConfigureLocalGameWindow> ().redPlayerName;
				purplePlayerName = mainMenuUIManager.GetComponent<ConfigureLocalGameWindow> ().purplePlayerName;
				bluePlayerName = mainMenuUIManager.GetComponent<ConfigureLocalGameWindow> ().bluePlayerName;

				//set the victory planets
				victoryPlanets = mainMenuUIManager.GetComponent<ConfigureLocalGameWindow>().planetCount;

				break;

			case MainMenuManager.GameType.LoadLocalGame:

				//set the load local game flag
				loadLocalGame = true;

				//set the local game string
				localGameToLoad = mainMenuUIManager.GetComponent<MainMenuManager> ().localGameToLoad;

				break;

				//TODO - add case for NewNetworkGame

			default:

				break;

			}
		
			//if it is, we need to destroy its UIManager so we don't think it's this scene's UI manager
			DestroyImmediate (mainMenuUIManager);

			//destroying the canvas will make the transition between scenes look better
			DestroyImmediate (GameObject.Find("Canvas"));

			//we also need to destroy it's main camera
			Destroy (Camera.main.gameObject);

			//now we can start unloading the main menu scene
			SceneManager.UnloadSceneAsync (mainMenuScene);

			//check if we have a local game to load
			//this if block runs essential startup functions, then calls the load game in filemanager
			if (loadLocalGame == true) {

				//initialize the graphics manager
				graphicsManager = GameObject.FindGameObjectWithTag ("GraphicsManager").GetComponent<GraphicsManager> ();
				graphicsManager.Init ();

				//get the tilemap
				tileMap = GameObject.FindGameObjectWithTag ("TileMap").GetComponent<TileMap> ();

				//initialize the tileMap - a lot of other things depend on the tileMap
				tileMap.Init ();

				//get the colony manager
				colonyManager = GameObject.FindGameObjectWithTag ("ColonyManager").GetComponent<ColonyManager> ();
				colonyManager.Init ();

				//get the managers
				uiManager = GameObject.FindGameObjectWithTag ("UIManager").GetComponent<UIManager> ();
				messageManager = GameObject.FindGameObjectWithTag ("MessageManager").GetComponent<MessageManager> ();
				mouseManager = GameObject.FindGameObjectWithTag ("MouseManager").GetComponent<MouseManager> ();
				soundManager = GameObject.FindGameObjectWithTag ("SoundManager").GetComponent<SoundManager> ();

				//get the main camera
				mainCamera = Camera.main;

				//call the setup cameras function
				SetupCameras ();

				//create the cursors used in the game
				InstantiateCursors ();

				//initialize other game objects
				InitializeGameObjects ();

				//set UnityActions
				SetActions();

				//add event listeners
				AddEventListeners ();

				return;

			}

		}

		//initialize the graphics manager
		graphicsManager = GameObject.FindGameObjectWithTag ("GraphicsManager").GetComponent<GraphicsManager> ();
		graphicsManager.Init ();

		//get the tilemap
		tileMap = GameObject.FindGameObjectWithTag ("TileMap").GetComponent<TileMap> ();

		//initialize the tileMap - a lot of other things depend on the tileMap
		tileMap.Init ();

		//get the colony manager
		colonyManager = GameObject.FindGameObjectWithTag ("ColonyManager").GetComponent<ColonyManager> ();
		colonyManager.Init ();

		//get the managers
		uiManager = GameObject.FindGameObjectWithTag ("UIManager").GetComponent<UIManager> ();
		messageManager = GameObject.FindGameObjectWithTag ("MessageManager").GetComponent<MessageManager> ();
		mouseManager = GameObject.FindGameObjectWithTag ("MouseManager").GetComponent<MouseManager> ();
		soundManager = GameObject.FindGameObjectWithTag ("SoundManager").GetComponent<SoundManager> ();



		//set the aShipisMoving flag to false
		aShipIsMoving = false;

		//set the actionMode
		CurrentActionMode = ActionMode.Selection;

		//get the main camera
		mainCamera = Camera.main;

		//call the setup cameras function
		SetupCameras ();

		//create the cursors used in the game
		InstantiateCursors ();

		//create rotating wormholes
		CreateRotatingWormholes();

		//only perform the following tasks if we are not loading a game
		if (loadedGame == false) {

			//initialize the players
			InitializePlayers ();

			//set the first turn of the game
			SetFirstTurn ();


		} else {

			//the else condition is that we are loading a game
			//set the teams mode from the save game file
			teamsEnabled = GameManager.loadedGameData.teamsEnabled;

			//initialize the players
			InitializePlayersFromLoad(GameManager.loadedGameData);

			//set the first turn after load
			SetFirstTurnAfterLoad(GameManager.loadedGameData);

		}



		//initialize other game objects
		InitializeGameObjects ();

		//set UnityActions
		SetActions();

		//add event listeners
		AddEventListeners ();


		//only perform the following tasks if we are not loading a game
		if (loadedGame == false) {
			
			//create starbases
			CreateStarbases ();

			//invoke the OnNewTurn event
			OnNewTurn.Invoke (currentTurnPlayer);

			//change the first turn flag to true
			firstTurnHasHappened = true;

		} else {
			
			//create starbases after load
			CreateStarbasesFromLoad(GameManager.loadedGameData);

			//create ships after load
			CreateShipsFromLoad(GameManager.loadedGameData);

			//create colonies after load
			CreateColoniesFromLoad(GameManager.loadedGameData);

			//invoke event to signal all units loaded
			OnAllLoadedUnitsCreated.Invoke();

			//set loaded flag back to false
			loadedGame = false;

			//invoke the OnLoadedTurn event
			OnLoadedTurn.Invoke (currentTurnPlayer);

			//change the first turn flag to true
			firstTurnHasHappened = true;

		}

		//invoke the scene startup if this was a new scene
		if (newScene == true) {

			//set the newScene to false
			newScene = false;

			OnSceneStartup.Invoke ();

		}

	}



	// Update is called once per frame
	private void Update () {


		//check if we have a local game to load
		if (loadLocalGame == true) {

			//clear the flag
			loadLocalGame = false;

			//call the blackout event
			OnStartLoadingSceneStartup.Invoke();

			//invoke an event to load the local game
			OnLoadLocalGame.Invoke(localGameToLoad);

		}

		//check if there was a resolution change
		if (changedResolutionFrameCounter > 0) {

			changedResolutionFrameCounter--;

		}
		else if (changedResolution == true) {

			//clear the flag
			changedResolution = false;

			//set up the cameras
			SetupCameras (mainCamera.transform.position);

		}

	}


	//function to initialize the turn order - will be called for a new game
	private void SetFirstTurn(){

		//set the starting game year
		gameYear = startingGameYear;

		//define an array of all the values of the Player color enum
		System.Array possibleColors = System.Enum.GetValues (typeof(Player.Color));

		//pick a random turn from the possible values
		Player.Color randomColor = (Player.Color)possibleColors.GetValue (Random.Range (0, possibleColors.Length));

		//set the turn to the random turn we selected
		currentTurn = randomColor;

		//at the start of the turn, the phase should be purchasing units
		currentTurnPhase = TurnPhase.PurchasePhase;

		//set the first turn variable
		firstTurn = currentTurn;

		//set a new active player
		currentTurnPlayer = GetPlayerFromColor(currentTurn);

	}

	//function to initialize the turn from a loaded game
	private void SetFirstTurnAfterLoad(FileManager.SaveGameData saveGameData){

		//set the starting game year
		gameYear = saveGameData.gameYear;

		//set the turn from the save file
		currentTurn = saveGameData.currentTurn;

		//set the phase from the save file
		currentTurnPhase = saveGameData.currentTurnPhase;

		//set the first turn variable
		firstTurn = saveGameData.firstTurn;

		//set a new active player
		currentTurnPlayer = GetPlayerFromColor(currentTurn);

		//set the victory planets
		victoryPlanets = saveGameData.victoryPlanets;

	}

	//this function will advance the phase of the turn or the turn itself
	private void AdvanceTurn(){
		
		//i need to account for the fact that as players get eliminated, we don't need to cycle through all the enums anymore

		//if we are in the purchase phase and end the turn, all we need to do is advance to the main phase
		if (currentTurnPhase == TurnPhase.PurchasePhase) {

			//change phase to main
			currentTurnPhase = TurnPhase.MainPhase;

			//invoke the OnBeginMainPhase event
			OnBeginMainPhase.Invoke(currentTurnPlayer);

		}
		//else if we are in the main phase, we need to go to the next player's turn
		else if (currentTurnPhase == TurnPhase.MainPhase) {

			//invoke the endTurn Event
			OnEndTurn.Invoke(currentTurn);

			//check if any players should be dead
			if (PlayerShouldBeDead (greenPlayer) == true) {

				OnKillPlayer.Invoke (greenPlayer);

			}
			if (PlayerShouldBeDead (purplePlayer) == true) {

				OnKillPlayer.Invoke (purplePlayer);

			}
			if (PlayerShouldBeDead (redPlayer) == true) {

				OnKillPlayer.Invoke (redPlayer);

			}
			if (PlayerShouldBeDead (bluePlayer) == true) {

				OnKillPlayer.Invoke (bluePlayer);

			}


			//define an array of all the values of the Player color enum
			Player.Color[] possibleColors = System.Enum.GetValues (typeof(Player.Color)).Cast<Player.Color>().ToArray();

			//define a color for the next turn after the current one
			Player.Color possibleNextTurn;

			//check to see if we're at the last color in the enum.  if we're not, increment by 1
			if ((int)currentTurn < possibleColors.Length - 1) {

				//increment by 1
				possibleNextTurn = currentTurn + 1;

			}

			//else, we are at the last value in the enum.  In this case, we have to go back to the first element
			else {

				//go back to the first element
				possibleNextTurn = possibleColors [0];

			}

			//check to see if the player at the next element is alive
			//if so, the while loop doesn't execute
			//if the player is not alive, we will increment the possible next turn again and re-check
			while (GetPlayerFromColor (possibleNextTurn).isAlive == false) {

				//check to see if we're at the last color in the enum.  if we're not, increment by 1
				if ((int)possibleNextTurn < possibleColors.Length - 1) {

					//increment by 1
					possibleNextTurn = possibleNextTurn + 1;

				}

				//else, we are at the last value in the enum.  In this case, we have to go back to the first element
				else {

					//go back to the first element
					possibleNextTurn = possibleColors [0];

				}
					
			}


			//set turn equal to the possible next turn
			currentTurn = possibleNextTurn;

			//check to see if the current turn is the same as the first turn - if so, we need to increment the year
			if (currentTurn == firstTurn) {

				gameYear++;

				//check if the game has already been won
				if (gameHasAlreadyBeenWon == false) {

					//check if we have a game win condition
					if (teamsEnabled == true) {

						//check if the green and red team has won
						if (GreenAndRedTeamHasWon () == true) {

							//invoke the victoryEvent
							OnTeamVictory.Invoke (greenPlayer, redPlayer);

							//set the alreadyWon flag
							gameHasAlreadyBeenWon = true;

						} else if (PurpleAndBlueTeamHasWon () == true) {

							//invoke the victoryEvent
							OnTeamVictory.Invoke (purplePlayer, bluePlayer);

							//set the alreadyWon flag
							gameHasAlreadyBeenWon = true;

						}

					} else {

						//check if the green player has won
						if (GreenPlayerHasWon () == true) {

							//invoke the victoryEvent
							OnSoloVictory.Invoke (greenPlayer);

							//set the alreadyWon flag
							gameHasAlreadyBeenWon = true;

						} else if (PurplePlayerHasWon () == true) {

							//invoke the victoryEvent
							OnSoloVictory.Invoke (purplePlayer);

							//set the alreadyWon flag
							gameHasAlreadyBeenWon = true;

						} else if (RedPlayerHasWon () == true) {

							//invoke the victoryEvent
							OnSoloVictory.Invoke (redPlayer);

							//set the alreadyWon flag
							gameHasAlreadyBeenWon = true;

						} else if (BluePlayerHasWon () == true) {

							//invoke the victoryEvent
							OnSoloVictory.Invoke (bluePlayer);

							//set the alreadyWon flag
							gameHasAlreadyBeenWon = true;

						}

					}

				}

			}

			//set the phase back to purchase phase since it's a new turn
			currentTurnPhase = TurnPhase.PurchasePhase;

			//set a new active player
			currentTurnPlayer = GetPlayerFromColor(currentTurn);

			//invoke the OnNewTurn event
			OnNewTurn.Invoke(currentTurnPlayer);

		}

	}

	//this function will initialize the players at the start of the game
	private void InitializePlayers(){

		//create players
		greenPlayer = Player.createPlayer (prefabPlayer, Player.Color.Green, greenPlayerName);
		purplePlayer = Player.createPlayer  (prefabPlayer, Player.Color.Purple, purplePlayerName);
		redPlayer = Player.createPlayer  (prefabPlayer, Player.Color.Red, redPlayerName);
		bluePlayer = Player.createPlayer  (prefabPlayer, Player.Color.Blue, bluePlayerName);


	}

	//this function will initialize the players from a loaded file
	private void InitializePlayersFromLoad(FileManager.SaveGameData saveGameData){
		
		//need to find the index of the player colors from the saveGameData file
		for (int i = 0; i < numberPlayers; i++) {

			//switch case on the player color
			switch (saveGameData.playerColor [i]) {

			case Player.Color.Green:

				greenPlayer = Player.createPlayer (prefabPlayer, saveGameData.playerColor [i], saveGameData.playerName [i]);
				break;

			case Player.Color.Purple:

				purplePlayer = Player.createPlayer (prefabPlayer, saveGameData.playerColor [i], saveGameData.playerName [i]);
				break;

			case Player.Color.Red:

				redPlayer = Player.createPlayer (prefabPlayer, saveGameData.playerColor [i], saveGameData.playerName [i]);
				break;

			case Player.Color.Blue:

				bluePlayer = Player.createPlayer (prefabPlayer, saveGameData.playerColor [i], saveGameData.playerName [i]);
				break;

			default:

				Debug.LogError ("Could not read player from SaveGameData");
				break;

			}

		}

		//invoke the load players event
		OnPlayersLoaded.Invoke (GameManager.loadedGameData);

	}

	//this function returns the player object based on the color
	public Player GetPlayerFromColor(Player.Color color){

		Player player;

		switch(color){
		case Player.Color.Green:
			player =  greenPlayer;
			break;
		case Player.Color.Purple:
			player =  purplePlayer;
			break;
		case Player.Color.Red:
			player = redPlayer;
			break;
		case Player.Color.Blue:
			player = bluePlayer;
			break;
		default:
			player = greenPlayer;
			break;

		}

		return player;

	}

	//this function returns the player object based on the color string
	public Player GetPlayerFromColor(string color){

		Player player;

		switch(color){
		case "Green":
			player =  greenPlayer;
			break;
		case "Purple":
			player =  purplePlayer;
			break;
		case "Red":
			player = redPlayer;
			break;
		case "Blue":
			player = bluePlayer;
			break;
		default:
			player = greenPlayer;
			break;

		}

		return player;

	}

	//this function will instantiate cursors and emblems used in the game
	private void InstantiateCursors(){

		//instantiate a HexCursor called hexCursor
		Hex CursorOriginHex = new Hex (0,0,0);

		HexCursor.createHexCursor (prefabHexCursor, tileMap.HexMap [CursorOriginHex]);

		//instantiate a selection cursor on the map at 0,0,0
		if (prefabSelectionCursor != null) {
			SelectionCursor.createSelectionCursor (prefabSelectionCursor, tileMap.HexMap [new Hex (0, 0, 0)]);
		} else {
			Debug.LogError ("You forgot to assign the prefab selection cursor");
		}

		//instantiate a targeting cursor on the map at 0,0,0
		if (prefabTargetingCursor != null) {
			TargetingCursor.createTargetingCursor (prefabTargetingCursor, tileMap.HexMap [new Hex (0, 0, 0)]);
		} else {
			Debug.LogError ("You forgot to assign the prefab targeting cursor");
		}

		//instantiate a targeting emblem on the map at 0,0,0
		if (prefabTargetingEmblem != null) {
			TargetingEmblem.CreateTargetingEmblem (prefabTargetingEmblem, tileMap.HexMap [new Hex (0, 0, 0)]);
		} else {
			Debug.LogError ("You forgot to assign the prefab targeting emblem");
		}

	}

	//this function will set up the cameras
	private void SetupCameras(){

		mainCameraDefaultSize = 4.0f * tileMap.Tile_size * tileMap.Stretch_z;

		mainCamera.orthographicSize = mainCameraDefaultSize;

		//set the camera viewports
		SetMainCameraViewport ();
		SetUISidePanelCameraViewport ();

		miniMapCamera = GameObject.FindGameObjectWithTag("MiniMapCamera").GetComponent<Camera>();
		miniMapCamera.transform.position = new Vector3 (tileMap.maxWidth / 2.0f, 40.0f, tileMap.maxHeight / 2.0f);
		miniMapCamera.orthographicSize = tileMap.maxHeight / 2.0f *1.05f;

		//set camera starting position
		mainCamera.transform.position = new Vector3 (tileMap.origin.x + (mainCamera.orthographicSize  * mainCamera.aspect)- (tileMap.Tile_size * tileMap.Stretch_x), 
			10.0f,
			tileMap.origin.y + mainCamera.orthographicSize - (tileMap.Tile_size * tileMap.Stretch_z));

		//set values for the minimap size
		aspectRatio = (float)Screen.width / (float)Screen.height;
		mapRatio = tileMap.maxWidth / tileMap.maxHeight;

		//cache the starting miniMap Border ymax
		initialMiniMapBorderYMax = miniMapBorder.GetComponent<RectTransform> ().anchorMax.y;

		//check if the aspect ratio is wider than or equal to the target resolution
		if (aspectRatio >= canvas.GetComponent<CanvasScaler> ().referenceResolution.x / canvas.GetComponent<CanvasScaler> ().referenceResolution.y) {

			//Debug.Log ("Wider or equal");

			//if we are at the reference resolution or wider, we can increase the minimap border width and hold the height
			//SetMiniMapBorderXMin();

			//set the miniMapCamera rectTransform;
			SetMiniMapCameraRectTransform ();


		} else {

			//the else condition is that we are narrower than the reference resolution.  In this case, I want to maintain the width, and shrink the height
			//Debug.Log ("narrow");

			//set the miniMapCamera rectTransform;
			SetMiniMapCameraRectTransform ();

			//SetMiniMapBorderHeightToMatchCamera ();

			//top justify the minimap
			//TopJustifyMiniMap();

		}

		//invoke the camera change event
		OnChangedMainCameraSetup.Invoke();

		//Debug.Log ("setup cameras");

	}

	//this overloaded function will set up the cameras with a passed camera location
	private void SetupCameras(Vector3 mainCameraPosition){

		mainCameraDefaultSize = 4.0f * tileMap.Tile_size * tileMap.Stretch_z;

		mainCamera.orthographicSize = mainCameraDefaultSize;

		//set the camera viewports
		SetMainCameraViewport ();
		SetUISidePanelCameraViewport ();

		miniMapCamera = GameObject.FindGameObjectWithTag("MiniMapCamera").GetComponent<Camera>();
		miniMapCamera.transform.position = new Vector3 (tileMap.maxWidth / 2.0f, 40.0f, tileMap.maxHeight / 2.0f);
		miniMapCamera.orthographicSize = tileMap.maxHeight / 2.0f *1.05f;

		//set camera starting position
		mainCamera.transform.position = mainCameraPosition;

		//set values for the minimap size
		aspectRatio = (float)Screen.width / (float)Screen.height;
		mapRatio = tileMap.maxWidth / tileMap.maxHeight;

		//cache the starting miniMap Border ymax
		initialMiniMapBorderYMax = miniMapBorder.GetComponent<RectTransform> ().anchorMax.y;

		//check if the aspect ratio is wider than or equal to the target resolution
		if (aspectRatio >= canvas.GetComponent<CanvasScaler> ().referenceResolution.x / canvas.GetComponent<CanvasScaler> ().referenceResolution.y) {

			//Debug.Log ("Wider or equal");

			//if we are at the reference resolution or wider, we can increase the minimap border width and hold the height
			//SetMiniMapBorderXMin();

			//set the miniMapCamera rectTransform;
			SetMiniMapCameraRectTransform ();


		} else {

			//the else condition is that we are narrower than the reference resolution.  In this case, I want to maintain the width, and shrink the height
			//Debug.Log ("narrow");

			//set the miniMapCamera rectTransform;
			SetMiniMapCameraRectTransform ();

			//SetMiniMapBorderHeightToMatchCamera ();

			//top justify the minimap
			//TopJustifyMiniMap();

		}

		//invoke the camera change event
		OnChangedMainCameraSetup.Invoke();

		//Debug.Log ("setup cameras");

	}

	//this function sets the main camera viewport size based on the aspect ratio
	private void SetMainCameraViewport(){

		//get the reference resolution for the canvas
		Vector2 canvasReferenceResolution = canvas.GetComponent<CanvasScaler> ().referenceResolution;

		//convert the reference resolution to an aspectRatio
		float canvasReferenceAspectRatio = canvasReferenceResolution.x / canvasReferenceResolution.y;

		//get the current resolution
		float canvasCurrentAspectRatio = canvas.GetComponent<RectTransform>().rect.width / canvas.GetComponent<RectTransform>().rect.height;

		//calculate the main camera width based on aspect ratio
		float mainCameraWidth = ((canvasCurrentAspectRatio / canvasReferenceAspectRatio) - (1- uiManager.defaultMainCameraWidth))/
		(canvasCurrentAspectRatio / canvasReferenceAspectRatio);
			

		//set the main camera rect
		Camera.main.rect = new Rect(Camera.main.rect.x, Camera.main.rect.y, mainCameraWidth, Camera.main.rect.height);

		//Debug.Log (Camera.main.rect.width);

	}

	//this function sets the UI side panel camera viewport
	private void SetUISidePanelCameraViewport(){

		//this rect should fill up the rest of the screen not used by the main camera
		uiSidePanelCamera.rect = new Rect (Camera.main.rect.width, 0.0f, 1 - Camera.main.rect.width, 1.0f);

	}

	//this subfunction sets the minimap xmin position, based on holding the xmax, ymin, and ymax values
	private void SetMiniMapBorderXMin(){
		
		//set the miniMap border xmin to ensure a perfect square based on aspect ratio
		miniMapBorder.GetComponent<RectTransform> ().anchorMin = new Vector2 (miniMapBorder.GetComponent<RectTransform> ().anchorMax.x -
			((miniMapBorder.GetComponent<RectTransform> ().anchorMax.y - miniMapBorder.GetComponent<RectTransform> ().anchorMin.y) * mapRatio / aspectRatio),
			miniMapBorder.GetComponent<RectTransform> ().anchorMin.y);

	}

	//this subfuction will shrink the miniMapBorder height to match the minimap camera
	private void SetMiniMapBorderHeightToMatchCamera(){
		
		//now that the camera rect has been set, I want to shrink the border rect transform in y to match it
		miniMapBorder.GetComponent<RectTransform> ().anchorMin = new Vector2 (miniMapBorder.GetComponent<RectTransform> ().anchorMin.x,
			miniMapCameraViewportY - miniMapScreenPercentBorderOffset);

		miniMapBorder.GetComponent<RectTransform> ().anchorMax = new Vector2 (miniMapBorder.GetComponent<RectTransform> ().anchorMax.x,
			miniMapCameraViewportY + miniMapCameraViewportHeight + miniMapScreenPercentBorderOffset);

	}

	//this subfunction justifies the minimap border and camera back to the top y value it started at
	private void TopJustifyMiniMap(){

		//calculate how much we must shift to get back to the starting top justification
		float yValueToShift = initialMiniMapBorderYMax - miniMapBorder.GetComponent<RectTransform> ().anchorMax.y;

		//adjust the border anchors up by the shift value
		miniMapBorder.GetComponent<RectTransform> ().anchorMin = new Vector2 (miniMapBorder.GetComponent<RectTransform> ().anchorMin.x,
			miniMapBorder.GetComponent<RectTransform> ().anchorMin.y + yValueToShift);

		miniMapBorder.GetComponent<RectTransform> ().anchorMax = new Vector2 (miniMapBorder.GetComponent<RectTransform> ().anchorMax.x,
			miniMapBorder.GetComponent<RectTransform> ().anchorMax.y + yValueToShift);

		//adjust the minimap camera rect up by the shift value
		miniMapCameraViewportY = miniMapCameraViewportY + yValueToShift;
		miniMapCamera.rect = new Rect (miniMapCameraViewportX, miniMapCameraViewportY, miniMapCameraViewportWidth, miniMapCameraViewportHeight);

	}

	//this subfunction sets the miniMapCameraRectTransform based on the miniMapBorder
	private void SetMiniMapCameraRectTransform(){

		//the viewport width will be the screen percentage in x of the miniMap border (this is the delta in the anchor min and max)
		//then reduced by the screen percentage offset X2 (for the left and right)

		//this commented out version was when the anchors were to the overall screen, not the new UISidePanel
		//miniMapCameraViewportWidth = (miniMapBorder.GetComponent<RectTransform> ().anchorMax.x - miniMapBorder.GetComponent<RectTransform> ().anchorMin.x)
		//	- (2 * miniMapScreenPercentBorderOffset / aspectRatio);

		//this version includes ((1 - Camera.main.rect.xMax) + Camera.main.rect.xMax), which takes the local anchor back to the screen anchor value
		miniMapCameraViewportWidth = ((miniMapBorder.GetComponent<RectTransform> ().anchorMax.x * (1 - Camera.main.rect.xMax) + Camera.main.rect.xMax)
			- (miniMapBorder.GetComponent<RectTransform> ().anchorMin.x * (1 - Camera.main.rect.xMax) + Camera.main.rect.xMax))
			- (2 * miniMapScreenPercentBorderOffset / aspectRatio);

		//the minimap viewport height will be the width / mapRatio * aspect ratio
		//the map ratio is the pixel width / pixel height of the extents of the hex map
		//the aspect ratio is the screen width / screen height
		//so the mapRatio / aspectRatio is a factor that will make the camera display the entire hexMap in a square window
		miniMapCameraViewportHeight = miniMapCameraViewportWidth / mapRatio * aspectRatio;

		//the minimap camera x value is the border location, plus 1 offset rightwards
		//this commented out version was when the anchors were to the overall screen, not the new UISidePanel
		miniMapCameraViewportX = miniMapBorder.GetComponent<RectTransform> ().anchorMin.x + miniMapScreenPercentBorderOffset / aspectRatio;

		//this version includes ((1 - Camera.main.rect.xMax) + Camera.main.rect.xMax), which takes the local anchor back to the screen anchor value
		miniMapCameraViewportX = (miniMapBorder.GetComponent<RectTransform> ().anchorMin.x * (1 - Camera.main.rect.xMax) + Camera.main.rect.xMax) 
			+ miniMapScreenPercentBorderOffset / aspectRatio;
		
		//the miniMapCameraY position is the middle of the border y position (found by taking the delta max-min anchor y)/2 + the min)
		//then subtracting out half of the minimap camera height
		miniMapCameraViewportY = (miniMapBorder.GetComponent<RectTransform> ().anchorMax.y - miniMapBorder.GetComponent<RectTransform> ().anchorMin.y) / 2.0f +
			miniMapBorder.GetComponent<RectTransform> ().anchorMin.y - miniMapCameraViewportHeight / 2.0f;

		//set the rectTransform of the minimap camera
		miniMapCamera.rect = new Rect (miniMapCameraViewportX, miniMapCameraViewportY, miniMapCameraViewportWidth, miniMapCameraViewportHeight);

	}

	//this function resoloves a change in resolution
	private void ResolveResolutionChange(){

		//set the resolution changed flag
		changedResolution = true;

		//set the delay counter
		changedResolutionFrameCounter = changedResolutionFrameDelay;

	}

	//this function will create the player starbases
	private void CreateStarbases(){

		//create starbases
		if (prefabStarbase != null) {
			CombatUnit.createUnit (prefabStarbase, tileMap.HexMap [OffsetCoord.RoffsetToCube
				(OffsetCoord.ODD, new OffsetCoord((int)RedStarbaseLocation.x, (int)RedStarbaseLocation.y))], redPlayer, uiManager.GetComponent<DefaultUnitNames>().redStarbaseName);

			CombatUnit.createUnit (prefabStarbase, tileMap.HexMap [OffsetCoord.RoffsetToCube
				(OffsetCoord.ODD, new OffsetCoord((int)BlueStarbaseLocation.x, (int)BlueStarbaseLocation.y))], bluePlayer,uiManager.GetComponent<DefaultUnitNames>().blueStarbaseName);
			
			CombatUnit.createUnit (prefabStarbase, tileMap.HexMap [OffsetCoord.RoffsetToCube
				(OffsetCoord.ODD, new OffsetCoord((int)GreenStarbaseLocation.x, (int)GreenStarbaseLocation.y))], greenPlayer,uiManager.GetComponent<DefaultUnitNames>().greenStarbaseName);
		
			CombatUnit.createUnit (prefabStarbase, tileMap.HexMap [OffsetCoord.RoffsetToCube
				(OffsetCoord.ODD, new OffsetCoord((int)PurpleStarbaseLocation.x, (int)PurpleStarbaseLocation.y))], purplePlayer,uiManager.GetComponent<DefaultUnitNames>().purpleStarbaseName);
			
		} else {
			Debug.LogError ("You forgot to assign the prefab starbase");
		}


	}

	//this function will create the player starbases from load
	private void CreateStarbasesFromLoad(FileManager.SaveGameData saveGameData){

		//create starbases
		if (prefabStarbase != null) {

			//loop through the players
			//need to find the index of the player colors from the saveGameData file
			for (int i = 0; i < numberPlayers; i++) {

				//switch case on the player color
				switch (saveGameData.playerColor [i]) {

				case Player.Color.Green:

					//for the green player, check if the green starbase is alive
					if (saveGameData.playerStarbaseIsAlive [i] == true) {

						//if the starbase was alive, call the create unit function
						CombatUnit.createUnitFromLoad (prefabStarbase, tileMap.HexMap [OffsetCoord.RoffsetToCube
							(OffsetCoord.ODD, new OffsetCoord ((int)GreenStarbaseLocation.x, (int)GreenStarbaseLocation.y))], greenPlayer, 0, saveGameData);
						
					}
					break;

				case Player.Color.Purple:

					//for the purple player, check if the purple starbase is alive
					if (saveGameData.playerStarbaseIsAlive [i] == true) {

						//if the starbase was alive, call the create unit function
						CombatUnit.createUnitFromLoad (prefabStarbase, tileMap.HexMap [OffsetCoord.RoffsetToCube
							(OffsetCoord.ODD, new OffsetCoord ((int)PurpleStarbaseLocation.x, (int)PurpleStarbaseLocation.y))], purplePlayer, 0, saveGameData);

					}					
					break;

				case Player.Color.Red:

					//for the red player, check if the red starbase is alive
					if (saveGameData.playerStarbaseIsAlive [i] == true) {

						//if the starbase was alive, call the create unit function
						CombatUnit.createUnitFromLoad (prefabStarbase, tileMap.HexMap [OffsetCoord.RoffsetToCube
							(OffsetCoord.ODD, new OffsetCoord ((int)RedStarbaseLocation.x, (int)RedStarbaseLocation.y))], redPlayer, 0, saveGameData);

					}					
					break;

				case Player.Color.Blue:

					//for the blue player, check if the blue starbase is alive
					if (saveGameData.playerStarbaseIsAlive [i] == true) {

						//if the starbase was alive, call the create unit function
						CombatUnit.createUnitFromLoad (prefabStarbase, tileMap.HexMap [OffsetCoord.RoffsetToCube
							(OffsetCoord.ODD, new OffsetCoord ((int)BlueStarbaseLocation.x, (int)BlueStarbaseLocation.y))], bluePlayer, 0, saveGameData);

					}					
					break;

				default:

					Debug.LogError ("Could not create starbase from SaveGameData");
					break;

				}

			}

		} else {

			Debug.LogError ("You forgot to assign the prefab starbase");

		}

	}

	//this function will restore all ships from a loaded game
	private void CreateShipsFromLoad(FileManager.SaveGameData saveGameData){

		//loop through the players
		//need to find the index of the player colors from the saveGameData file
		for (int i = 0; i < numberPlayers; i++) {

			//switch case on the player color
			switch (saveGameData.playerColor [i]) {

			case Player.Color.Green:

				//loop through the number of scouts purchased
				for (int j = 0; j < saveGameData.playerScoutPurchased[i]; j++) {

					//for each purchased scout, see if it is supposed to be alive
					if (saveGameData.playerScoutPurchasedAlive [i, j] == true) {

						//check if the prefab is assigned
						if (prefabScout != null) {

							//create the unit
							CombatUnit.createUnitFromLoad (prefabScout, tileMap.HexMap [new Hex(saveGameData.scoutHexLocation[i,j].q, 
								saveGameData.scoutHexLocation[i,j].r, saveGameData.scoutHexLocation[i,j].s)], greenPlayer, j, saveGameData);

						} else {

							Debug.LogError ("Prefab Scout not assigned");

						}

					}

				}

				//loop through the number of birds purchased
				for (int j = 0; j < saveGameData.playerBirdOfPreyPurchased[i]; j++) {

					//for each purchased bird, see if it is supposed to be alive
					if (saveGameData.playerBirdOfPreyPurchasedAlive [i, j] == true) {

						//check if the prefab is assigned
						if (prefabBirdOfPrey != null) {

							//create the unit
							CombatUnit.createUnitFromLoad (prefabBirdOfPrey, tileMap.HexMap [new Hex(saveGameData.birdOfPreyHexLocation[i,j].q, 
								saveGameData.birdOfPreyHexLocation[i,j].r, saveGameData.birdOfPreyHexLocation[i,j].s)], greenPlayer, j, saveGameData);

						} else {

							Debug.LogError ("Prefab BirdOfPrey not assigned");

						}

					}

				}

				//loop through the number of destroyers purchased
				for (int j = 0; j < saveGameData.playerDestroyerPurchased[i]; j++) {

					//for each purchased bird, see if it is supposed to be alive
					if (saveGameData.playerDestroyerPurchasedAlive [i, j] == true) {

						//check if the prefab is assigned
						if (prefabDestroyer != null) {

							//create the unit
							CombatUnit.createUnitFromLoad (prefabDestroyer, tileMap.HexMap [new Hex(saveGameData.destroyerHexLocation[i,j].q, 
								saveGameData.destroyerHexLocation[i,j].r, saveGameData.destroyerHexLocation[i,j].s)], greenPlayer, j, saveGameData);

						} else {

							Debug.LogError ("Prefab Destroyer not assigned");

						}

					}

				}

				//loop through the number of starships purchased
				for (int j = 0; j < saveGameData.playerStarshipPurchased[i]; j++) {

					//for each purchased bird, see if it is supposed to be alive
					if (saveGameData.playerStarshipPurchasedAlive [i, j] == true) {

						//check if the prefab is assigned
						if (prefabStarship != null) {

							//create the unit
							CombatUnit.createUnitFromLoad (prefabStarship, tileMap.HexMap [new Hex(saveGameData.starshipHexLocation[i,j].q, 
								saveGameData.starshipHexLocation[i,j].r, saveGameData.starshipHexLocation[i,j].s)], greenPlayer, j, saveGameData);

						} else {

							Debug.LogError ("Prefab Starship not assigned");

						}

					}

				}

				break;

			case Player.Color.Purple:

				//loop through the number of scouts purchased
				for (int j = 0; j < saveGameData.playerScoutPurchased[i]; j++) {

					//for each purchased scout, see if it is supposed to be alive
					if (saveGameData.playerScoutPurchasedAlive [i, j] == true) {

						//check if the prefab is assigned
						if (prefabScout != null) {

							//create the unit
							CombatUnit.createUnitFromLoad (prefabScout, tileMap.HexMap [new Hex(saveGameData.scoutHexLocation[i,j].q, 
								saveGameData.scoutHexLocation[i,j].r, saveGameData.scoutHexLocation[i,j].s)], purplePlayer, j, saveGameData);

						} else {

							Debug.LogError ("Prefab Scout not assigned");

						}

					}

				}

				//loop through the number of birds purchased
				for (int j = 0; j < saveGameData.playerBirdOfPreyPurchased[i]; j++) {

					//for each purchased bird, see if it is supposed to be alive
					if (saveGameData.playerBirdOfPreyPurchasedAlive [i, j] == true) {

						//check if the prefab is assigned
						if (prefabBirdOfPrey != null) {

							//create the unit
							CombatUnit.createUnitFromLoad (prefabBirdOfPrey, tileMap.HexMap [new Hex(saveGameData.birdOfPreyHexLocation[i,j].q, 
								saveGameData.birdOfPreyHexLocation[i,j].r, saveGameData.birdOfPreyHexLocation[i,j].s)], purplePlayer, j, saveGameData);

						} else {

							Debug.LogError ("Prefab BirdOfPrey not assigned");

						}

					}

				}

				//loop through the number of destroyers purchased
				for (int j = 0; j < saveGameData.playerDestroyerPurchased[i]; j++) {

					//for each purchased bird, see if it is supposed to be alive
					if (saveGameData.playerDestroyerPurchasedAlive [i, j] == true) {

						//check if the prefab is assigned
						if (prefabDestroyer != null) {

							//create the unit
							CombatUnit.createUnitFromLoad (prefabDestroyer, tileMap.HexMap [new Hex(saveGameData.destroyerHexLocation[i,j].q, 
								saveGameData.destroyerHexLocation[i,j].r, saveGameData.destroyerHexLocation[i,j].s)], purplePlayer, j, saveGameData);

						} else {

							Debug.LogError ("Prefab Destroyer not assigned");

						}

					}

				}

				//loop through the number of starships purchased
				for (int j = 0; j < saveGameData.playerStarshipPurchased[i]; j++) {

					//for each purchased bird, see if it is supposed to be alive
					if (saveGameData.playerStarshipPurchasedAlive [i, j] == true) {

						//check if the prefab is assigned
						if (prefabStarship != null) {

							//create the unit
							CombatUnit.createUnitFromLoad (prefabStarship, tileMap.HexMap [new Hex(saveGameData.starshipHexLocation[i,j].q, 
								saveGameData.starshipHexLocation[i,j].r, saveGameData.starshipHexLocation[i,j].s)], purplePlayer, j, saveGameData);

						} else {

							Debug.LogError ("Prefab Starship not assigned");

						}

					}

				}
			
				break;

			case Player.Color.Red:

				//loop through the number of scouts purchased
				for (int j = 0; j < saveGameData.playerScoutPurchased[i]; j++) {

					//for each purchased scout, see if it is supposed to be alive
					if (saveGameData.playerScoutPurchasedAlive [i, j] == true) {

						//check if the prefab is assigned
						if (prefabScout != null) {

							//create the unit
							CombatUnit.createUnitFromLoad (prefabScout, tileMap.HexMap [new Hex(saveGameData.scoutHexLocation[i,j].q, 
								saveGameData.scoutHexLocation[i,j].r, saveGameData.scoutHexLocation[i,j].s)], redPlayer, j, saveGameData);

						} else {

							Debug.LogError ("Prefab Scout not assigned");

						}

					}

				}

				//loop through the number of birds purchased
				for (int j = 0; j < saveGameData.playerBirdOfPreyPurchased[i]; j++) {

					//for each purchased bird, see if it is supposed to be alive
					if (saveGameData.playerBirdOfPreyPurchasedAlive [i, j] == true) {

						//check if the prefab is assigned
						if (prefabBirdOfPrey != null) {

							//create the unit
							CombatUnit.createUnitFromLoad (prefabBirdOfPrey, tileMap.HexMap [new Hex(saveGameData.birdOfPreyHexLocation[i,j].q, 
								saveGameData.birdOfPreyHexLocation[i,j].r, saveGameData.birdOfPreyHexLocation[i,j].s)], redPlayer, j, saveGameData);

						} else {

							Debug.LogError ("Prefab BirdOfPrey not assigned");

						}

					}

				}

				//loop through the number of destroyers purchased
				for (int j = 0; j < saveGameData.playerDestroyerPurchased[i]; j++) {

					//for each purchased bird, see if it is supposed to be alive
					if (saveGameData.playerDestroyerPurchasedAlive [i, j] == true) {

						//check if the prefab is assigned
						if (prefabDestroyer != null) {

							//create the unit
							CombatUnit.createUnitFromLoad (prefabDestroyer, tileMap.HexMap [new Hex(saveGameData.destroyerHexLocation[i,j].q, 
								saveGameData.destroyerHexLocation[i,j].r, saveGameData.destroyerHexLocation[i,j].s)], redPlayer, j, saveGameData);

						} else {

							Debug.LogError ("Prefab Destroyer not assigned");

						}

					}

				}

				//loop through the number of starships purchased
				for (int j = 0; j < saveGameData.playerStarshipPurchased[i]; j++) {

					//for each purchased bird, see if it is supposed to be alive
					if (saveGameData.playerStarshipPurchasedAlive [i, j] == true) {

						//check if the prefab is assigned
						if (prefabStarship != null) {

							//create the unit
							CombatUnit.createUnitFromLoad (prefabStarship, tileMap.HexMap [new Hex(saveGameData.starshipHexLocation[i,j].q, 
								saveGameData.starshipHexLocation[i,j].r, saveGameData.starshipHexLocation[i,j].s)], redPlayer, j, saveGameData);

						} else {

							Debug.LogError ("Prefab Starship not assigned");

						}

					}

				}

				break;

			case Player.Color.Blue:

				//loop through the number of scouts purchased
				for (int j = 0; j < saveGameData.playerScoutPurchased[i]; j++) {

					//for each purchased scout, see if it is supposed to be alive
					if (saveGameData.playerScoutPurchasedAlive [i, j] == true) {

						//check if the prefab is assigned
						if (prefabScout != null) {

							//create the unit
							CombatUnit.createUnitFromLoad (prefabScout, tileMap.HexMap [new Hex(saveGameData.scoutHexLocation[i,j].q, 
								saveGameData.scoutHexLocation[i,j].r, saveGameData.scoutHexLocation[i,j].s)], bluePlayer, j, saveGameData);

						} else {

							Debug.LogError ("Prefab Scout not assigned");

						}

					}

				}

				//loop through the number of birds purchased
				for (int j = 0; j < saveGameData.playerBirdOfPreyPurchased[i]; j++) {

					//for each purchased bird, see if it is supposed to be alive
					if (saveGameData.playerBirdOfPreyPurchasedAlive [i, j] == true) {

						//check if the prefab is assigned
						if (prefabBirdOfPrey != null) {

							//create the unit
							CombatUnit.createUnitFromLoad (prefabBirdOfPrey, tileMap.HexMap [new Hex(saveGameData.birdOfPreyHexLocation[i,j].q, 
								saveGameData.birdOfPreyHexLocation[i,j].r, saveGameData.birdOfPreyHexLocation[i,j].s)], bluePlayer, j, saveGameData);

						} else {

							Debug.LogError ("Prefab BirdOfPrey not assigned");

						}

					}

				}

				//loop through the number of destroyers purchased
				for (int j = 0; j < saveGameData.playerDestroyerPurchased[i]; j++) {

					//for each purchased bird, see if it is supposed to be alive
					if (saveGameData.playerDestroyerPurchasedAlive [i, j] == true) {

						//check if the prefab is assigned
						if (prefabDestroyer != null) {

							//create the unit
							CombatUnit.createUnitFromLoad (prefabDestroyer, tileMap.HexMap [new Hex(saveGameData.destroyerHexLocation[i,j].q, 
								saveGameData.destroyerHexLocation[i,j].r, saveGameData.destroyerHexLocation[i,j].s)], bluePlayer, j, saveGameData);

						} else {

							Debug.LogError ("Prefab Destroyer not assigned");

						}

					}

				}

				//loop through the number of starships purchased
				for (int j = 0; j < saveGameData.playerStarshipPurchased[i]; j++) {

					//for each purchased bird, see if it is supposed to be alive
					if (saveGameData.playerStarshipPurchasedAlive [i, j] == true) {

						//check if the prefab is assigned
						if (prefabStarship != null) {

							//create the unit
							CombatUnit.createUnitFromLoad (prefabStarship, tileMap.HexMap [new Hex(saveGameData.starshipHexLocation[i,j].q, 
								saveGameData.starshipHexLocation[i,j].r, saveGameData.starshipHexLocation[i,j].s)], bluePlayer, j, saveGameData);

						} else {

							Debug.LogError ("Prefab Starship not assigned");

						}

					}

				}

				break;

			default:

				Debug.LogError ("Could not create ships from SaveGameData");
				break;

			}

		}

	}

	//this function creates the colonies from a load file
	private void CreateColoniesFromLoad(FileManager.SaveGameData saveGameData){

		//loop through the colony data in the save file
		for (int i = 0; i < saveGameData.colonyPlanetName.Length; i++) {

			//check if the planet is colonized
			if (saveGameData.colonyIsColonized[i] == true) {

				//invoke the OnLoadColony event, passing the hexMapTile and the colonizing player
				OnLoadColony.Invoke(tileMap.HexMap[saveGameData.colonyPlanetHexLocation[i]],saveGameData.colonyColonizingColor[i]);

			}

		}

	}

	//this function will initialize all other objects in the game
	private void InitializeGameObjects(){

		//this section of code will initialize various scripts

		//I need to resize the UI side panel before any of the vertical dropdowns size themselves
		uiManager.GetComponent<UISidePanel>().Init();
		uiManager.GetComponent<UIMainPanel>().Init();

		//make the main ui panels fit
		uiManager.GetComponent<UIMainPanelFitter>().Init();

		//initialize the VerticalDropDown scripts on all of the actionMenu submenus
		Component[] verticalDropDowns = uiManager.GetComponents<VerticalDropDown>();
		foreach(VerticalDropDown verticalDropDown in verticalDropDowns){

			verticalDropDown.Init ();

		}

		uiManager.GetComponent<NextUnit>().Init();
		uiManager.GetComponent<EndTurnToggle>().Init();
		uiManager.GetComponent<EndTurnDropDown>().Init();
		uiManager.GetComponent<TextInput>().Init();
		uiManager.GetComponent<TractorBeamToggle>().Init();
		uiManager.GetComponent<TractorBeamMenu>().Init();
		uiManager.GetComponent<PhasorToggle>().Init();
		uiManager.GetComponent<PhasorMenu>().Init();
		uiManager.GetComponent<TorpedoToggle>().Init();
		uiManager.GetComponent<TorpedoMenu>().Init();
		uiManager.GetComponent<FlareManager>().Init();
		uiManager.GetComponent<UseItemToggle>().Init();
		uiManager.GetComponent<UseItemMenu> ().Init ();
		uiManager.GetComponent<CrewToggle> ().Init ();
		uiManager.GetComponent<CrewMenu> ().Init ();
		uiManager.GetComponent<UnitPanel> ().Init ();
		uiManager.GetComponent<RenameShip> ().Init ();
		uiManager.GetComponent<CloakingDeviceToggle> ().Init ();
		uiManager.GetComponent<CloakingDeviceMenu> ().Init ();
		uiManager.GetComponent<PlayerStatus> ().Init ();
		uiManager.GetComponent<StatusPanel> ().Init ();
		uiManager.GetComponent<PauseFadePanel> ().Init ();
		uiManager.GetComponent<PurchaseManager> ().Init ();
		uiManager.GetComponent<InstructionPanel> ().Init ();
		uiManager.GetComponent<NameNewShip> ().Init ();
		uiManager.GetComponent<FileManager> ().Init ();

		uiManager.GetComponent<MoveToggle>().Init();
		uiManager.GetComponent<MoveMenu>().Init();
		uiManager.GetComponent<TurnCounter>().Init();
		uiManager.GetComponent<DefaultUnitNames>().Init();
		uiManager.GetComponent<FileSaveWindow>().Init();
		uiManager.GetComponent<FileOverwritePrompt>().Init();
		uiManager.GetComponent<PauseFadePanel2> ().Init ();
		uiManager.GetComponent<FileLoadWindow>().Init();
		uiManager.GetComponent<FileDeletePrompt>().Init();
		uiManager.GetComponent<CutsceneManager> ().Init ();
		uiManager.GetComponent<TileMapAnimationManager> ().Init ();
		uiManager.GetComponent<ExitGamePrompt> ().Init ();
		uiManager.GetComponent<SceneTransitionFadePanel> ().Init ();
		uiManager.GetComponent<VictoryPanel> ().Init ();



		uiManager.GetComponent<UINavigationMain> ().Init ();


		//these Inits were moved to the Create constructors
		//GameObject.FindGameObjectWithTag ("TargetingEmblem").GetComponent<TargetingEmblem> ().Init ();
		//GameObject.FindGameObjectWithTag ("TargetingCursor").GetComponent<TargetingCursor> ().Init ();
		//GameObject.FindGameObjectWithTag ("SelectionCursor").GetComponent<SelectionCursor> ().Init ();

		//this init must run this way because I only want to initialize the 1 range tile on the parent game object
		GameObject.FindGameObjectWithTag ("RangeParent").GetComponent<RangeTile> ().Init ();


		GameObject.FindGameObjectWithTag ("Background").GetComponent<BackgroundImage> ().Init (1000f);
		GameObject.FindGameObjectWithTag ("Background2").GetComponent<BackgroundImage> ().Init (500f);
		GameObject.FindGameObjectWithTag ("Background3").GetComponent<BackgroundImage> ().Init (300f);
		GameObject.FindGameObjectWithTag ("MiniMapCursor").GetComponent<MiniMapCursor> ().Init ();


		messageManager.Init ();

		//we want the combat manager to be initialized after the message manager, so that the message subscribes to a 
		//combat event before the combat manager resolves the attack and triggers another message 
		GameObject.FindGameObjectWithTag ("CombatManager").GetComponent<CombatManager> ().Init ();

		mouseManager.Init ();

		soundManager.Init ();

		//this needs to be after mouse manager since mouse manager is listening to it
		//this needs to be after sound manager since sound manager is listening to it
		uiManager.GetComponent<Settings> ().Init ();

		uiManager.Init ();


	}

	//this function handles setting action mode generically
	private void SetActionMode(ActionMode newActionMode){

		CurrentActionMode = newActionMode;
		//Debug.Log ("Action Mode is" + CurrentActionMode.ToString());


	}

	//this function is used to set the ActionMode to Selection
	private void SetActionModeToSelection(){

		CurrentActionMode = ActionMode.Selection;
		//Debug.Log ("Action Mode is Selection");
	}

	//this function is used to set the ActionMode to Movement
	private void SetActionModeToMovement(){

		CurrentActionMode = ActionMode.Movement;
		//Debug.Log ("Action Mode is Movement");

	}

	//this function is used to set the ActionMode to Tractor Beam
	private void SetActionModeToTractorBeam(){
		
		CurrentActionMode = ActionMode.TractorBeam;
		//Debug.Log ("Action Mode is Tractor Beam");

	}

	//this function is used to set the ActionMode to PhasorAttack
	private void SetActionModeToPhasorAttack(){

		CurrentActionMode = ActionMode.PhasorAttack;
		//Debug.Log ("Action Mode is PhasorAttack");

	}

	//this function is used to set the ActionMode to TorpedoAttack
	private void SetActionModeToTorpedoAttack(){

		CurrentActionMode = ActionMode.TorpedoAttack;
		//Debug.Log ("Action Mode is TorpedoAttack");

	}

	//this function is used to set the ActionMode to FlareMode
	private void SetActionModeToFlareMode(){

		CurrentActionMode = ActionMode.FlareMode;
		//Debug.Log ("Action Mode is FlareMode");

	}

	//this function is used to set the ActionMode to ItemUse
	private void SetActionModeToItemUse(){

		CurrentActionMode = ActionMode.ItemUse;
		//Debug.Log ("Action Mode is ItemUse");

	}

	//this function is used to set the ActionMode to Crew
	private void SetActionModeToCrew(){

		CurrentActionMode = ActionMode.Crew;
		//Debug.Log ("Action Mode is Crew");

	}

	//this function is used to set the ActionMode to Rename
	private void SetActionModeToRename(){

		CurrentActionMode = ActionMode.Rename;
		//Debug.Log ("Action Mode is Rename");

	}

	//this function is used to set the ActionMode to EndTurn
	private void SetActionModeToEndTurn(){

		CurrentActionMode = ActionMode.EndTurn;
		//Debug.Log ("Action Mode is EndTurn");

	}

	//this function is used to set the ActionMode to Cloaking
	private void SetActionModeToCloaking(){

		CurrentActionMode = ActionMode.Cloaking;
		//Debug.Log ("Action Mode is Cloaking");

	}

	//this function is used to set the ActionMode to Animation
	private void SetActionModeToAnimation(){

		//cache the previous actionMode in memory
		//I only want to do this if we are not in animationMode - if we are already there (like if 2 triggers happened in a row),
		//we don't want to overwrite the previous mode
		if (CurrentActionMode != ActionMode.Animation) {
			
			preAnimationActionMode = CurrentActionMode;

		}

		CurrentActionMode = ActionMode.Animation;
		//Debug.Log ("Action Mode is Animation");


	}

	//this function is used to set the ActionMode to PlaceNewUnit
	private void SetActionModeToPlaceNewUnit(){

		CurrentActionMode = ActionMode.PlaceNewUnit;
		//Debug.Log ("Action Mode is Cloaking");

	}


	//this function sets the UnityActions
	private void SetActions(){

		toggleSetActionModeToTractorBeamAction = (toggle) => {SetActionModeToTractorBeam ();};
		tractorBeamSetActionModeToSelectionAction = () => {
			//we only want to set the current action mode to selection if we are still on tractor beam
			if(currentActionMode == ActionMode.TractorBeam){

				SetActionModeToSelection();

			}
		};

		toggleSetActionModeToMovementAction = (toggle) => {SetActionModeToMovement ();};
		movementSetActionModeToSelectionAction = () => {
			//we only want to set the current action mode to selection if we are still on movement
			if (currentActionMode == ActionMode.Movement) {

				SetActionModeToSelection ();

			}
		};

		toggleSetActionModeToPhasorAttackAction = (toggle) => {SetActionModeToPhasorAttack();};
		phasorSetActionModeToSelectionAction = () => {
			//we only want to set the current action mode to selection if we are still on phasor attack
			if(currentActionMode == ActionMode.PhasorAttack){

				SetActionModeToSelection();

			}
		};

		toggleSetActionModeToTorpedoAttackAction = (toggle) => {SetActionModeToTorpedoAttack();};
		torpedoSetActionModeToSelectionAction = () => {
			//we only want to set the current action mode to selection if we are still on torpedo attack
			if(currentActionMode == ActionMode.TorpedoAttack){

				SetActionModeToSelection();

			}
		};	

		toggleSetActionModeToItemUseAction = (toggle) => {SetActionModeToItemUse();};
		itemSetActionModeToSelectionAction = () => {
			//we only want to set the current action mode to selection if we are still on useItem mode
			if(currentActionMode == ActionMode.ItemUse){

				SetActionModeToSelection();

			}
		};

		toggleSetActionModeToCrewAction = (toggle) => {SetActionModeToCrew();};
		crewSetActionModeToSelectionAction = () => {
			//we only want to set the current action mode to selection if we are still on crew
			if(currentActionMode == ActionMode.Crew){

				SetActionModeToSelection();

			}
		};

		shipSetShipMovingAction = (ship) => {SetShipMovingFlag(ship);};

		torpedoAttackShipWithFlaresActionModeToFlareAction = (attackingUnit,targetedUnit,shipSectionTargeted,expectedDamage) => {

			if(targetedUnit.GetComponent<StorageSection>().flareMode == StorageSection.FlareMode.Manual){

				//this is commented out so that we never leave torpedo mode during flare resolution, so that the targeted unit never changes
				//I think eventually it would be better to just not add the listener
				SetActionModeToFlareMode();

			}

		};

		torpedoAttackBaseWithFlaresActionModeToFlareAction = (attackingUnit,targetedUnit,baseSectionTargeted,expectedDamage) => {

			if(targetedUnit.GetComponent<StarbaseStorageSection2>().flareMode == StarbaseStorageSection2.FlareMode.Manual){

				//this is commented out so that we never leave torpedo mode during flare resolution, so that the targeted unit never changes
				//I think eventually it would be better to just not add the listener
				SetActionModeToFlareMode();

			}

		};

		flareDataSetActionModeToTorpedoAction = (flareEventData) => {SetActionModeToTorpedoAttack();};

		renameSetActionModeToRenameAction = () => {SetActionModeToRename();};

		renameUnitSetActionModeToPreviousAction = (combatUnit, newName, previousActionMode) => {SetActionMode (previousActionMode);};

		actionModeToPreviousAction = (previousActionMode) => {SetActionMode (previousActionMode);};

		toggleSetActionModeToEndTurnAction = (toggle) => {SetActionModeToEndTurn();};

		endTurnSetActionModeToSelectionAction = () => {
			//we only want to set the current action mode to selection if we are still on end turn
			if(currentActionMode == ActionMode.EndTurn){

				SetActionModeToSelection();

			}
		};

		setActionModeToCloakingAction = () => {SetActionModeToCloaking ();};

		cloakingSetActionModeToSelectionAction = () => {
			//we only want to set the current action mode to selection if we are still on cloaking
			if(currentActionMode == ActionMode.Cloaking){

				SetActionModeToSelection();

			}
		};

		shipSetActionModeToAnimationAction = (ship) =>{

			//we only want to change to animation mode if the ship is not being towed.  If it is being towed, the towing unit
			//will set the mode to animation
			if(ship.GetComponent<EngineSection>().isBeingTowed == false){

				//check if we are already in animation mode
				if(CurrentActionMode != ActionMode.Animation){

					//capture the previous action Mode
					preAnimationActionMode = CurrentActionMode;

					SetActionModeToAnimation();

				}

			}

		};

		shipSetActionModeToPreviousAction = (ship) => {

			//check if we are in animation mode
			if(CurrentActionMode == ActionMode.Animation){

				//return to the previous action mode
				SetActionMode(preAnimationActionMode);

			}

		};

		purchaseSetActionModeToPlaceUnitAction = (purchasedItems,purchaseCost,unitType) => {SetActionModeToPlaceNewUnit ();};

		setActionModeToSelectionAction = () => {SetActionModeToSelection();};

		purchaseSetActionModeToSelectionAction = (purchaseEventData) => {SetActionModeToSelection();};

		purchaseCreateNewUnitAction = (purchaseEventData) => {CreateNewShip(purchaseEventData);};

		resolveLoadedGameAction = () => {ResolveLoadedGame();};

		resolveLoadedGameFromSaveAction = (saveGameData) => {ResolveLoadedGameFromSaveFile(saveGameData);};

	}


	//this function adds listeners to various events
	private void AddEventListeners(){

		//add listeners to TractorBeamToggle events
		uiManager.GetComponent<TractorBeamToggle> ().OnTurnedOnTractorBeamToggle.AddListener (toggleSetActionModeToTractorBeamAction);
		uiManager.GetComponent<TractorBeamToggle> ().OnTurnedOffTractorBeamToggleWhileEngaged.AddListener(tractorBeamSetActionModeToSelectionAction);
		uiManager.GetComponent<TractorBeamToggle> ().OnTurnedOffTractorBeamToggleWhileNotEngaged.AddListener(tractorBeamSetActionModeToSelectionAction);

		//add listeners to the MoveToggle events
		uiManager.GetComponent<MoveToggle> ().OnTurnedOnMoveToggle.AddListener (toggleSetActionModeToMovementAction);
		uiManager.GetComponent<MoveToggle> ().OnTurnedOffMoveToggle.AddListener(movementSetActionModeToSelectionAction);

		//add listeners to the PhasorAttack events
		uiManager.GetComponent<PhasorToggle>().OnTurnedOnPhasorToggle.AddListener(toggleSetActionModeToPhasorAttackAction);
		uiManager.GetComponent<PhasorToggle>().OnTurnedOffPhasorToggle.AddListener(phasorSetActionModeToSelectionAction);

		//add listeners to the TorpedoAttack events
		uiManager.GetComponent<TorpedoToggle>().OnTurnedOnTorpedoToggle.AddListener(toggleSetActionModeToTorpedoAttackAction);
		uiManager.GetComponent<TorpedoToggle>().OnTurnedOffTorpedoToggle.AddListener(torpedoSetActionModeToSelectionAction);

		//add listeners to the UseItem events
		uiManager.GetComponent<UseItemToggle>().OnTurnedOnUseItemToggle.AddListener(toggleSetActionModeToItemUseAction);
		uiManager.GetComponent<UseItemToggle>().OnTurnedOffUseItemToggle.AddListener(itemSetActionModeToSelectionAction);

		//add listeners to the Crew events
		uiManager.GetComponent<CrewToggle>().OnTurnedOnCrewToggle.AddListener(toggleSetActionModeToCrewAction);
		uiManager.GetComponent<CrewToggle>().OnTurnedOffCrewToggle.AddListener(crewSetActionModeToSelectionAction);

		//add listeners to Movement start and finish
		EngineSection.OnMoveStart.AddListener(shipSetShipMovingAction);
		EngineSection.OnMoveFinish.AddListener(shipSetShipMovingAction);

		//add a listener for the end turn button
		uiManager.GetComponent<EndTurnDropDown>().OnAcceptEndTurnPrompt.AddListener(AdvanceTurn);

		//add listeners to flare menu events
		CombatManager.OnHeavyTorpedoUntargetedAttackShipWithFlares.AddListener(torpedoAttackShipWithFlaresActionModeToFlareAction);
		CombatManager.OnLightTorpedoUntargetedAttackShipWithFlares.AddListener(torpedoAttackShipWithFlaresActionModeToFlareAction);
		CombatManager.OnHeavyTorpedoTargetedAttackShipWithFlares.AddListener(torpedoAttackShipWithFlaresActionModeToFlareAction);
		CombatManager.OnLightTorpedoTargetedAttackShipWithFlares.AddListener(torpedoAttackShipWithFlaresActionModeToFlareAction);

		CombatManager.OnHeavyTorpedoUntargetedAttackBaseWithFlares.AddListener(torpedoAttackBaseWithFlaresActionModeToFlareAction);
		CombatManager.OnLightTorpedoUntargetedAttackBaseWithFlares.AddListener(torpedoAttackBaseWithFlaresActionModeToFlareAction);
		CombatManager.OnHeavyTorpedoTargetedAttackBaseWithFlares.AddListener(torpedoAttackBaseWithFlaresActionModeToFlareAction);
		CombatManager.OnLightTorpedoTargetedAttackBaseWithFlares.AddListener(torpedoAttackBaseWithFlaresActionModeToFlareAction);

		uiManager.GetComponent<FlareManager> ().OnUseFlaresYes.AddListener (flareDataSetActionModeToTorpedoAction);
		uiManager.GetComponent<FlareManager> ().OnUseFlaresCancel.AddListener (flareDataSetActionModeToTorpedoAction);

		//add listeners for the rename mode events
		uiManager.GetComponent<RenameShip>().OnEnterRenameMode.AddListener(renameSetActionModeToRenameAction);
		uiManager.GetComponent<RenameShip> ().OnRenameUnit.AddListener (renameUnitSetActionModeToPreviousAction);
		uiManager.GetComponent<RenameShip> ().OnRenameCancel.AddListener (actionModeToPreviousAction);

		//add listeners to end turn events
		uiManager.GetComponent<EndTurnDropDown>().OnEnterEndTurnPrompt.AddListener(toggleSetActionModeToEndTurnAction);
		uiManager.GetComponent<EndTurnDropDown>().OnCancelEndTurnPrompt.AddListener(endTurnSetActionModeToSelectionAction);
		uiManager.GetComponent<EndTurnDropDown>().OnAcceptEndTurnPrompt.AddListener(endTurnSetActionModeToSelectionAction);

		//add listeners to the Cloaking events
		uiManager.GetComponent<CloakingDeviceToggle> ().OnTurnedOnCloakingDeviceToggle.AddListener (setActionModeToCloakingAction);
		uiManager.GetComponent<CloakingDeviceToggle> ().OnTurnedOffCloakingDeviceToggle.AddListener(cloakingSetActionModeToSelectionAction);

		//add listeners for move start, which trigget animation mode
		EngineSection.OnMoveStart.AddListener(shipSetActionModeToAnimationAction);

		//add listener for move finish, which should send us back to the previous action mode
		EngineSection.OnMoveFinish.AddListener (shipSetActionModeToPreviousAction);

		//add listener for placing a new unit
		uiManager.GetComponent<PurchaseManager> ().OnOutfittedShip.AddListener (purchaseSetActionModeToPlaceUnitAction);

		//add listener for cancelling unit outfit
		uiManager.GetComponent<PurchaseManager>().cancelPurchaseItemsButton.onClick.AddListener(setActionModeToSelectionAction);
		uiManager.GetComponent<PurchaseManager>().cancelPurchaseShipButton.onClick.AddListener(setActionModeToSelectionAction);

		//add listener for cancelling from unit placement
		uiManager.GetComponent<InstructionPanel>().OnCancelPlaceUnit.AddListener(setActionModeToSelectionAction);


		//add listener for choosing an option at the name new ship panel
		uiManager.GetComponent<NameNewShip>().OnCanceledPurchase.AddListener(setActionModeToSelectionAction);
		uiManager.GetComponent<NameNewShip>().OnPurchasedNewShip.AddListener(purchaseSetActionModeToSelectionAction);

		//this listener creates a new ship
		uiManager.GetComponent<NameNewShip>().OnPurchasedNewShip.AddListener(purchaseCreateNewUnitAction);

		//add listener for loaded game
		uiManager.GetComponent<FileManager>().OnLoadGame.AddListener(resolveLoadedGameAction);
		uiManager.GetComponent<FileManager>().OnSendSaveGameDataFromLoad.AddListener(resolveLoadedGameFromSaveAction);

		//add listener for exiting to main menu
		uiManager.GetComponent<ExitGamePrompt>().OnExitGameYesClicked.AddListener(BeginSceneFadeOut);

		//add listener for fade out complete
		uiManager.GetComponent<SceneTransitionFadePanel>().OnFadeOutComplete.AddListener(ExitToMainMenu);

		//add listener for resolution changes
		uiManager.GetComponent<Settings>().OnChangeResolution.AddListener(ResolveResolutionChange);

		//add listener for invalid selected unit
		mouseManager.OnInvalidActionModeForSelectedUnit.AddListener(setActionModeToSelectionAction);

	}



	//function to handle setting aShipIsMoving flag
	private void SetShipMovingFlag(Ship ship){

		if (ship.GetComponent<EngineSection>().isMoving == true) {

			aShipIsMoving = true;

		} else if (ship.GetComponent<EngineSection>().isMoving == false) {

			aShipIsMoving = false;

		}

	}

	//this helper function takes 2 combat units and returns whether they are on the same team
	public bool UnitsAreTeammates(CombatUnit selectedUnit, CombatUnit targetedUnit){

		bool returnValue;

		//first, we can check if the 2 units are the same color.  If they are, they are on the same team and we can return true
		if (selectedUnit.owner.color == targetedUnit.owner.color) {

			returnValue =  true;

		}
		//the else condition is that the 2 units are different colors.  They could still be teammates if teams are enabled
		else {

			//check if teams are enabled
			if (teamsEnabled == true) {

				//if teams are enabled, we need to check specific color pairs to determine teammates
				switch (selectedUnit.owner.color) {

				case Player.Color.Blue:

					//we now need to check the possible targeted unit colors
					switch (targetedUnit.owner.color) {

					case Player.Color.Blue:
						returnValue = true;
						break;
					case Player.Color.Green:
						returnValue = false;
						break;
					case Player.Color.Purple:
						returnValue = true;
						break;
					case Player.Color.Red:
						returnValue = false;
						break;
					default:
						returnValue = false;
						break;

					}
					break;
				case Player.Color.Green:

					//we now need to check the possible targeted unit colors
					switch (targetedUnit.owner.color) {

					case Player.Color.Blue:
						returnValue =  false;
						break;
					case Player.Color.Green:
						returnValue =  true;
						break;
					case Player.Color.Purple:
						returnValue =  false;
						break;
					case Player.Color.Red:
						returnValue =  true;
						break;
					default:
						returnValue =  false;
						break;

					}
					break;
				case Player.Color.Purple:

					//we now need to check the possible targeted unit colors
					switch (targetedUnit.owner.color) {

					case Player.Color.Blue:
						returnValue =  true;
						break;
					case Player.Color.Green:
						returnValue =  false;
						break;
					case Player.Color.Purple:
						returnValue =  true;
						break;
					case Player.Color.Red:
						returnValue =  false;
						break;
					default:
						returnValue =  false;
						break;

					}
					break;
				case Player.Color.Red:

					//we now need to check the possible targeted unit colors
					switch (targetedUnit.owner.color) {

					case Player.Color.Blue:
						returnValue =  false;
						break;
					case Player.Color.Green:
						returnValue =  true;
						break;
					case Player.Color.Purple:
						returnValue =  false;
						break;
					case Player.Color.Red:
						returnValue =  true;
						break;
					default:
						returnValue =  false;
						break;

					}
					break;
				default:
					returnValue =  false;
					break;
				}


			}
			//the else condition is that teams are not enabled
			else {

				//since the colors don't match and there are no teams, we can return false
				returnValue =  false;

			}

		}  //end else the colors don't match

		return returnValue;

	}

	//this function creates a new ship that has been purchased
	private void CreateNewShip(NameNewShip.NewUnitEventData newUnitData){

		//switch case
		switch (newUnitData.newUnitType) {

		case CombatUnit.UnitType.Scout:

			CombatUnit.createUnit (prefabScout, tileMap.HexMap [newUnitData.newUnitHexLocation], currentTurnPlayer, newUnitData.newUnitName, newUnitData.newUnitOutfittedItems);

			break;

		case CombatUnit.UnitType.BirdOfPrey:

			CombatUnit.createUnit (prefabBirdOfPrey, tileMap.HexMap [newUnitData.newUnitHexLocation], currentTurnPlayer, newUnitData.newUnitName, newUnitData.newUnitOutfittedItems);

			break;

		case CombatUnit.UnitType.Destroyer:

			CombatUnit.createUnit (prefabDestroyer, tileMap.HexMap [newUnitData.newUnitHexLocation], currentTurnPlayer, newUnitData.newUnitName, newUnitData.newUnitOutfittedItems);

			break;

		case CombatUnit.UnitType.Starship:

			CombatUnit.createUnit (prefabStarship, tileMap.HexMap [newUnitData.newUnitHexLocation], currentTurnPlayer, newUnitData.newUnitName, newUnitData.newUnitOutfittedItems);

			break;


		default:

			break;

		}

		//invoke the created new ship event
		OnNewUnitCreated.Invoke(currentTurnPlayer);

	}

	//this function responds to a loaded game
	private void ResolveLoadedGame(){

		//set the loaded game flag to true
		loadedGame = true;

	}
	//this function responds to a loaded game save file passed
	private void ResolveLoadedGameFromSaveFile(FileManager.SaveGameData saveGameData){

		//store the saveGameData
		GameManager.loadedGameData = saveGameData;

	}

	//this function creates the rotating wormholes
	private void CreateRotatingWormholes(){

		foreach (KeyValuePair<Hex,HexMapTile> entry in tileMap.HexMap) {

			if (entry.Value.tileType == HexMapTile.TileType.RedWormhole) {

				//create a red rotating wormhole
				WormholeRotation.createRotatingWormhole (prefabWormholeRotation, entry.Value, WormholeRotation.WormholeColor.Red);

			} else if (entry.Value.tileType == HexMapTile.TileType.BlueWormhole) {

				//create a red rotating wormhole
				WormholeRotation.createRotatingWormhole (prefabWormholeRotation, entry.Value, WormholeRotation.WormholeColor.Blue);

			}

		}

	}

	//this function handles exiting to the main menu
	private void ExitToMainMenu(){
		
		//we want to destroy the event system from this scene before the new scene is loaded
		Destroy (eventSystem);

		//disable the audio listener so when the new scene loads only 1 audio listener is loaded
		Camera.main.GetComponent<AudioListener> ().enabled = false;

		//load the main menu scene
		SceneManager.LoadScene (mainMenuSceneIndex, LoadSceneMode.Additive);

	}

	//this function begins a scene fadeOut
	private void BeginSceneFadeOut(){

		//invoke the exit scene event
		OnBeginSceneExit.Invoke();

	}

	//this function checks if we have a player that has won the game
	private bool GreenAndRedTeamHasWon(){

		//this is a team game
		//we need to check if the green and red team has won
		int greenAndRedPlanetTotal = colonyManager.PlanetsControlledByPlayer(greenPlayer) + colonyManager.PlanetsControlledByPlayer(redPlayer);

		//check if the total planets is at least the victory total
		if (greenAndRedPlanetTotal >= victoryPlanets) {

			return true;

		} else {

			return false;

		}

	}

	//this function checks if we have a player that has won the game
	private bool PurpleAndBlueTeamHasWon(){

		//this is a team game
		//we need to check if the green and red team has won
		int purpleAndBluePlanetTotal = colonyManager.PlanetsControlledByPlayer(purplePlayer) + colonyManager.PlanetsControlledByPlayer(bluePlayer);

		//check if the total planets is at least the victory total
		if (purpleAndBluePlanetTotal >= victoryPlanets) {

			return true;

		} else {

			return false;

		}

	}

	//this function checks if the green player has won the game
	private bool GreenPlayerHasWon(){

		//check if the total planets is at least the victory total
		if (colonyManager.PlanetsControlledByPlayer(greenPlayer) >= victoryPlanets) {

			return true;

		} else {

			return false;

		}

	}

	//this function checks if the purple has won the game
	private bool PurplePlayerHasWon(){

		//check if the total planets is at least the victory total
		if (colonyManager.PlanetsControlledByPlayer(purplePlayer) >= victoryPlanets) {

			return true;

		} else {

			return false;

		}

	}

	//this function checks if the red has won the game
	private bool RedPlayerHasWon(){

		//check if the total planets is at least the victory total
		if (colonyManager.PlanetsControlledByPlayer(redPlayer) >= victoryPlanets) {

			return true;

		} else {

			return false;

		}

	}

	//this function checks if the blue has won the game
	private bool BluePlayerHasWon(){

		//check if the total planets is at least the victory total
		if (colonyManager.PlanetsControlledByPlayer(bluePlayer) >= victoryPlanets) {

			return true;

		} else {

			return false;

		}

	}

	//this function checks if the player should be dead
	private bool PlayerShouldBeDead(Player player){

		//a player should be dead if all their units are destroyed and they have no ability to purchase new units
		//either because they have no income, or have no available ships that could be bought

		//start by checking if a player has units
		switch (player.color) {

		case Player.Color.Green:

			if (greenUnits.transform.childCount > 0) {

				//we can return false because the player has units
				return false;

			}

			break;

		case Player.Color.Purple:

			if (purpleUnits.transform.childCount > 0) {

				//we can return false because the player has units
				return false;

			}

			break;

		case Player.Color.Red:

			if (redUnits.transform.childCount > 0) {

				//we can return false because the player has units
				return false;

			}

			break;

		case Player.Color.Blue:

			if (blueUnits.transform.childCount > 0) {

				//we can return false because the player has units
				return false;

			}

			break;

		default:

			return false;

		}

		//if we haven't returned false yet at this point, that means that the player has no units
		//in this case, we need to check if they have the capability to produce units in the future

		//check if the player can buy a scout
		if(player.playerScoutPurchased < maxShipsPerClass && (player.playerMoney + player.playerPlanets * Player.planetValue >= PurchaseManager.costScout)){

			//the player has bought less than the max scouts and can afford one with money on hand plus income to be generated from planets
			//this means they should not be dead
			return false;

		}

		//check if the player can buy a bird of prey
		if(player.playerBirdOfPreyPurchased < maxShipsPerClass && (player.playerMoney + player.playerPlanets * Player.planetValue >= PurchaseManager.costBirdOfPrey)){

			//the player has bought less than the max birds of prey and can afford one with money on hand plus income to be generated from planets
			//this means they should not be dead
			return false;

		}

		//check if the player can buy a destroyer
		if(player.playerDestroyerPurchased < maxShipsPerClass && (player.playerMoney + player.playerPlanets * Player.planetValue >= PurchaseManager.costDestroyer)){

			//the player has bought less than the max destroyers and can afford one with money on hand plus income to be generated from planets
			//this means they should not be dead
			return false;

		}

		//check if the player can buy a starship
		if(player.playerStarshipPurchased < maxShipsPerClass && (player.playerMoney + player.playerPlanets * Player.planetValue >= PurchaseManager.costStarship)){

			//the player has bought less than the max starships and can afford one with money on hand plus income to be generated from planets
			//this means they should not be dead
			return false;

		}

		//if we haven't returned false at this point, this means that not only does the player have no units,
		//but the player cannot buy a scout, bird of prey, destroyer, or starship
		//in this case, they can't participate in the game at all, and should be dead

		return true;

	}



	//this function handles OnDestroy
	private void OnDestroy(){

		RemoveAllListeners ();

	}

	//this function removes listeners
	private void RemoveAllListeners(){

		if (uiManager != null) {

			//remove listeners to TractorBeamToggle events
			uiManager.GetComponent<TractorBeamToggle> ().OnTurnedOnTractorBeamToggle.RemoveListener (toggleSetActionModeToTractorBeamAction);
			uiManager.GetComponent<TractorBeamToggle> ().OnTurnedOffTractorBeamToggleWhileEngaged.RemoveListener (tractorBeamSetActionModeToSelectionAction);
			uiManager.GetComponent<TractorBeamToggle> ().OnTurnedOffTractorBeamToggleWhileNotEngaged.RemoveListener (tractorBeamSetActionModeToSelectionAction);

			//remove listeners to the MoveToggle events
			uiManager.GetComponent<MoveToggle> ().OnTurnedOnMoveToggle.RemoveListener (toggleSetActionModeToMovementAction);
			uiManager.GetComponent<MoveToggle> ().OnTurnedOffMoveToggle.RemoveListener (movementSetActionModeToSelectionAction);

			//remove listeners to the PhasorAttack events
			uiManager.GetComponent<PhasorToggle> ().OnTurnedOnPhasorToggle.RemoveListener (toggleSetActionModeToPhasorAttackAction);
			uiManager.GetComponent<PhasorToggle> ().OnTurnedOffPhasorToggle.RemoveListener (phasorSetActionModeToSelectionAction);

			//remove listeners to the TorpedoAttack events
			uiManager.GetComponent<TorpedoToggle> ().OnTurnedOnTorpedoToggle.RemoveListener (toggleSetActionModeToTorpedoAttackAction);
			uiManager.GetComponent<TorpedoToggle> ().OnTurnedOffTorpedoToggle.RemoveListener (torpedoSetActionModeToSelectionAction);

			//remove listeners to the UseItem events
			uiManager.GetComponent<UseItemToggle> ().OnTurnedOnUseItemToggle.RemoveListener (toggleSetActionModeToItemUseAction);
			uiManager.GetComponent<UseItemToggle> ().OnTurnedOffUseItemToggle.RemoveListener (itemSetActionModeToSelectionAction);

			//remove listeners to the Crew events
			uiManager.GetComponent<CrewToggle> ().OnTurnedOnCrewToggle.RemoveListener (toggleSetActionModeToCrewAction);
			uiManager.GetComponent<CrewToggle> ().OnTurnedOffCrewToggle.RemoveListener (crewSetActionModeToSelectionAction);

			//remove a listener for the end turn button
			uiManager.GetComponent<EndTurnDropDown>().OnAcceptEndTurnPrompt.RemoveListener(AdvanceTurn);

			uiManager.GetComponent<FlareManager> ().OnUseFlaresYes.RemoveListener (flareDataSetActionModeToTorpedoAction);
			uiManager.GetComponent<FlareManager> ().OnUseFlaresCancel.RemoveListener (flareDataSetActionModeToTorpedoAction);

			//remove listeners for the rename mode events
			uiManager.GetComponent<RenameShip>().OnEnterRenameMode.RemoveListener(renameSetActionModeToRenameAction);
			uiManager.GetComponent<RenameShip> ().OnRenameUnit.RemoveListener (renameUnitSetActionModeToPreviousAction);
			uiManager.GetComponent<RenameShip> ().OnRenameCancel.RemoveListener (actionModeToPreviousAction);

			//add listeners to end turn events
			uiManager.GetComponent<EndTurnDropDown>().OnEnterEndTurnPrompt.RemoveListener(toggleSetActionModeToEndTurnAction);
			uiManager.GetComponent<EndTurnDropDown>().OnCancelEndTurnPrompt.RemoveListener(endTurnSetActionModeToSelectionAction);
			uiManager.GetComponent<EndTurnDropDown>().OnAcceptEndTurnPrompt.RemoveListener(endTurnSetActionModeToSelectionAction);

			//remove listeners to the Cloaking events
			uiManager.GetComponent<CloakingDeviceToggle> ().OnTurnedOnCloakingDeviceToggle.RemoveListener (setActionModeToCloakingAction);
			uiManager.GetComponent<CloakingDeviceToggle> ().OnTurnedOffCloakingDeviceToggle.RemoveListener(cloakingSetActionModeToSelectionAction);

			//remove listener for placing a new unit
			uiManager.GetComponent<PurchaseManager> ().OnOutfittedShip.RemoveListener (purchaseSetActionModeToPlaceUnitAction);

			//remove listener for cancelling unit outfit
			uiManager.GetComponent<PurchaseManager>().cancelPurchaseItemsButton.onClick.RemoveListener(setActionModeToSelectionAction);
			uiManager.GetComponent<PurchaseManager>().cancelPurchaseShipButton.onClick.RemoveListener(setActionModeToSelectionAction);

			//remove listener for cancelling from unit placement
			uiManager.GetComponent<InstructionPanel>().OnCancelPlaceUnit.RemoveListener(setActionModeToSelectionAction);

			//remove listener for choosing an option at the name new ship panel
			uiManager.GetComponent<NameNewShip>().OnCanceledPurchase.RemoveListener(setActionModeToSelectionAction);
			uiManager.GetComponent<NameNewShip>().OnPurchasedNewShip.RemoveListener(purchaseSetActionModeToSelectionAction);

			//this listener creates a new ship
			uiManager.GetComponent<NameNewShip>().OnPurchasedNewShip.RemoveListener(purchaseCreateNewUnitAction);

			//remove listener for loaded game
			uiManager.GetComponent<FileManager>().OnLoadGame.RemoveListener(resolveLoadedGameAction);
			uiManager.GetComponent<FileManager>().OnSendSaveGameDataFromLoad.RemoveListener(resolveLoadedGameFromSaveAction);

			//remove listener for exiting to main menu
			uiManager.GetComponent<ExitGamePrompt>().OnExitGameYesClicked.RemoveListener(BeginSceneFadeOut);

			//remove listener for fade out complete
			uiManager.GetComponent<SceneTransitionFadePanel>().OnFadeOutComplete.RemoveListener(ExitToMainMenu);

			//remove listener for resolution changes
			uiManager.GetComponent<Settings>().OnChangeResolution.RemoveListener(ResolveResolutionChange);

		}

		if (mouseManager != null) {

			//remove listener for invalid selected unit
			mouseManager.OnInvalidActionModeForSelectedUnit.RemoveListener (setActionModeToSelectionAction);

		}

		//remove listeners to Movement start and finish
		EngineSection.OnMoveStart.RemoveListener(shipSetShipMovingAction);
		EngineSection.OnMoveFinish.RemoveListener(shipSetShipMovingAction);

		//remove listeners to flare menu events
		CombatManager.OnHeavyTorpedoUntargetedAttackShipWithFlares.RemoveListener(torpedoAttackShipWithFlaresActionModeToFlareAction);
		CombatManager.OnLightTorpedoUntargetedAttackShipWithFlares.RemoveListener(torpedoAttackShipWithFlaresActionModeToFlareAction);
		CombatManager.OnHeavyTorpedoTargetedAttackShipWithFlares.RemoveListener(torpedoAttackShipWithFlaresActionModeToFlareAction);
		CombatManager.OnLightTorpedoTargetedAttackShipWithFlares.RemoveListener(torpedoAttackShipWithFlaresActionModeToFlareAction);

		CombatManager.OnHeavyTorpedoUntargetedAttackBaseWithFlares.RemoveListener(torpedoAttackBaseWithFlaresActionModeToFlareAction);
		CombatManager.OnLightTorpedoUntargetedAttackBaseWithFlares.RemoveListener(torpedoAttackBaseWithFlaresActionModeToFlareAction);
		CombatManager.OnHeavyTorpedoTargetedAttackBaseWithFlares.RemoveListener(torpedoAttackBaseWithFlaresActionModeToFlareAction);
		CombatManager.OnLightTorpedoTargetedAttackBaseWithFlares.RemoveListener(torpedoAttackBaseWithFlaresActionModeToFlareAction);

		//remove listeners for move start, which trigget animation mode
		EngineSection.OnMoveStart.RemoveListener(shipSetActionModeToAnimationAction);

		//remove listener for move finish, which should send us back to the previous action mode
		EngineSection.OnMoveFinish.RemoveListener (shipSetActionModeToPreviousAction);

	}



}
