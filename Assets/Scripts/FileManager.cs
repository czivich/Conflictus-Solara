using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.IO;
using UnityEngine.SceneManagement;

public class FileManager : MonoBehaviour  {

	//manager
	protected GameManager gameManager;
	protected UIManager uiManager;
	protected TileMap tileMap;

	//variables to hold the 4 player collectors
	private GameObject greenCollector;
	private GameObject purpleCollector;
	private GameObject redCollector;
	private GameObject blueCollector;

	private GameObject[] collectors;

	//variable to hold the colony collector
	private GameObject colonyCollector;

	//variable to hold the current save/load file
	public string currentFileName {

		get;
		private set;

	}

	//variable to hold the autosave file name
	private string autoSaveFileName = "LastTurn";

	//event for sharing the saveGameData
	public LoadGameEvent OnSendSaveGameDataFromSave = new LoadGameEvent();
	public LoadGameEvent OnSendSaveGameDataFromLoad = new LoadGameEvent();
	public class LoadGameEvent : UnityEvent<SaveGameData>{};

	//unityActions
	private UnityAction<string> stringSaveGameAction;
	private UnityAction<string> stringDeleteGameAction;
	private UnityAction<string> stringLoadGameAction;
	private UnityAction<Player> playerAutoSaveAction;
	private UnityAction<Player> saveDataSetCurrentFileAction;

	//event for loading game
	public UnityEvent OnLoadGame = new UnityEvent();
	public UnityEvent OnDeleteGame = new UnityEvent();
	public UnityEvent OnAutosaveGame = new UnityEvent();

	//this class will hold all of the save data
	public class SaveGameData : IXmlSerializable{

		//variable to hold the parent class
		FileManager fileManager;

		//variable to hold the current save or load file name
		public string currentFileName;

		//variables for the gameManager state
		public bool teamsEnabled { get; private set; }
		public Player.Color currentTurn { get; private set; }
		public Player.Color firstTurn { get; private set; }
		public GameManager.TurnPhase currentTurnPhase {get; private set; }
		public int gameYear {get; private set; }
		public int victoryPlanets {get; private set; }

		//array to keep track of colonies
		public string[] colonyPlanetName {get; private set; }
		public bool[] colonyIsColonized {get; private set; }
		public Player.Color[] colonyColonizingColor {get; private set; }
		public Hex[] colonyPlanetHexLocation {get; private set; }

		//array to hold the players
		public Player[] playerArray { get; private set; }

		//variables for player attributes
		public Player.Color[] playerColor { get; private set; }
		public string[] playerName { get; private set; }
		public int[] playerMoney { get; private set; }
		public int[] playerPlanets { get; private set; }
		public bool[] playerIsAlive { get; private set; }
		public int[] playerScoutPurchased { get; private set; }
		public string[,] playerScoutNamesPurchased { get; private set; }
		public bool[,] playerScoutPurchasedAlive { get; private set; }
		public int[] playerBirdOfPreyPurchased { get; private set; }
		public string[,] playerBirdOfPreyNamesPurchased { get; private set; }
		public bool[,] playerBirdOfPreyPurchasedAlive { get; private set; }
		public int[] playerDestroyerPurchased { get; private set; }
		public string[,] playerDestroyerNamesPurchased { get; private set; }
		public bool[,] playerDestroyerPurchasedAlive { get; private set; }
		public int[] playerStarshipPurchased { get; private set; }
		public string[,] playerStarshipNamesPurchased { get; private set; }
		public bool[,] playerStarshipPurchasedAlive { get; private set; }
		public bool[] playerStarbaseIsAlive { get; private set; }


		//variables for ship attributes
		public Hex[,] scoutHexLocation { get; private set; }
		public bool[,] scoutHasRemainingPhaserAttack { get; private set; }
		//the unit name and serial number variables in CombatUnit can be populated from playerScoutNamesPurchased and the array index
		public bool[,] scoutPhaserSectionIsDestroyed { get; private set; }
		public int[,] scoutPhaserSectionShieldsCurrent { get; private set; }
		public bool[,] scoutUsedPhasersThisTurn { get; private set; }
		public int[,] scoutPhaserRadarShot { get; private set; }
		public bool[,] scoutPhaserRadarArray { get; private set; }
		public bool[,] scoutXRayKernel { get; private set; }
		public bool[,] scoutTractorBeam { get; private set; }
		public bool[,] scoutStorageSectionIsDestroyed { get; private set; }
		public int[,] scoutStorageSectionShieldsCurrent { get; private set; }
		public int[,] scoutDilithiumCrystals { get; private set; }
		public int[,] scoutTriilithiumCrystals { get; private set; }
		public int[,] scoutFlares { get; private set; }
		public bool[,] scoutRadarJammingSystem { get; private set; }
		public bool[,] scoutLaserScatteringSystem { get; private set; }
		public StorageSection.FlareMode[,] scoutFlareMode{ get; private set; }
		public bool[,] scoutEngineSectionIsDestroyed { get; private set; }
		public int[,] scoutEngineSectionShieldsCurrent { get; private set; }
		public int[,] scoutWarpBooster { get; private set; }
		public int[,] scoutTranswarpBooster { get; private set; }
		public bool[,] scoutWarpDrive { get; private set; }
		public bool[,] scoutTranswarpDrive { get; private set; }
		public int[,] scoutDistanceMovedThisTurn { get; private set; }
		public bool[,] scoutUsedWarpBoosterThisTurn { get; private set; }
		public bool[,] scoutUsedTranswarpBoosterThisTurn { get; private set; }


		public Hex[,] birdOfPreyHexLocation { get; private set; }
		public bool[,] birdOfPreyHasRemainingPhaserAttack { get; private set; }
		public bool[,] birdOfPreyPhaserSectionIsDestroyed { get; private set; }
		public int[,] birdOfPreyPhaserSectionShieldsCurrent { get; private set; }
		public bool[,] birdOfPreyUsedPhasersThisTurn { get; private set; }
		public int[,] birdOfPreyPhaserRadarShot { get; private set; }
		public bool[,] birdOfPreyPhaserRadarArray { get; private set; }
		public bool[,] birdOfPreyXRayKernel { get; private set; }
		public bool[,] birdOfPreyTractorBeam { get; private set; }
		public bool[,] birdOfPreyHasRemainingTorpedoAttack { get; private set; }
		public bool[,] birdOfPreyIsCloaked { get; private set; }
		public bool[,] birdOfPreyUsedCloakingDeviceThisTurn { get; private set; }
		//the unit name and serial number variables in CombatUnit can be populated from playerBirdOfPreyNamesPurchased and the array index
		public bool[,] birdOfPreyTorpedoSectionIsDestroyed { get; private set; }
		public int[,] birdOfPreyTorpedoSectionShieldsCurrent { get; private set; }
		public int[,] birdOfPreyTorpedoLaserShot { get; private set; }
		public bool[,] birdOfPreyLaserGuidanceSystem { get; private set; }
		public bool[,] birdOfPreyHighPressureTubes { get; private set; }
		public int[,] birdOfPreyLightTorpedos { get; private set; }
		public int[,] birdOfPreyHeavyTorpedos { get; private set; }
		public bool[,] birdOfPreyUsedTorpedosThisTurn { get; private set; }
		public bool[,] birdOfPreyEngineSectionIsDestroyed { get; private set; }
		public int[,] birdOfPreyEngineSectionShieldsCurrent { get; private set; }
		public int[,] birdOfPreyWarpBooster { get; private set; }
		public int[,] birdOfPreyTranswarpBooster { get; private set; }
		public bool[,] birdOfPreyWarpDrive { get; private set; }
		public bool[,] birdOfPreyTranswarpDrive { get; private set; }
		public int[,] birdOfPreyDistanceMovedThisTurn { get; private set; }
		public bool[,] birdOfPreyUsedWarpBoosterThisTurn { get; private set; }
		public bool[,] birdOfPreyUsedTranswarpBoosterThisTurn { get; private set; }


		public Hex[,] destroyerHexLocation { get; private set; }
		public bool[,] destroyerHasRemainingPhaserAttack { get; private set; }
		public bool[,] destroyerPhaserSectionIsDestroyed { get; private set; }
		public int[,] destroyerPhaserSectionShieldsCurrent { get; private set; }
		public bool[,] destroyerUsedPhasersThisTurn { get; private set; }
		public int[,] destroyerPhaserRadarShot { get; private set; }
		public bool[,] destroyerPhaserRadarArray { get; private set; }
		public bool[,] destroyerXRayKernel { get; private set; }
		public bool[,] destroyerTractorBeam { get; private set; }
		public bool[,] destroyerHasRemainingTorpedoAttack { get; private set; }
		//the unit name and serial number variables in CombatUnit can be populated from playerDestroyerNamesPurchased and the array index
		public bool[,] destroyerTorpedoSectionIsDestroyed { get; private set; }
		public int[,] destroyerTorpedoSectionShieldsCurrent { get; private set; }
		public int[,] destroyerTorpedoLaserShot { get; private set; }
		public bool[,] destroyerLaserGuidanceSystem { get; private set; }
		public bool[,] destroyerHighPressureTubes { get; private set; }
		public int[,] destroyerLightTorpedos { get; private set; }
		public int[,] destroyerHeavyTorpedos { get; private set; }
		public bool[,] destroyerUsedTorpedosThisTurn { get; private set; }
		public bool[,] destroyerStorageSectionIsDestroyed { get; private set; }
		public int[,] destroyerStorageSectionShieldsCurrent { get; private set; }
		public int[,] destroyerDilithiumCrystals { get; private set; }
		public int[,] destroyerTriilithiumCrystals { get; private set; }
		public int[,] destroyerFlares { get; private set; }
		public bool[,] destroyerRadarJammingSystem { get; private set; }
		public bool[,] destroyerLaserScatteringSystem { get; private set; }
		public StorageSection.FlareMode[,] destroyerFlareMode{ get; private set; }
		public bool[,] destroyerEngineSectionIsDestroyed { get; private set; }
		public int[,] destroyerEngineSectionShieldsCurrent { get; private set; }
		public int[,] destroyerWarpBooster { get; private set; }
		public int[,] destroyerTranswarpBooster { get; private set; }
		public bool[,] destroyerWarpDrive { get; private set; }
		public bool[,] destroyerTranswarpDrive { get; private set; }
		public int[,] destroyerDistanceMovedThisTurn { get; private set; }
		public bool[,] destroyerUsedWarpBoosterThisTurn { get; private set; }
		public bool[,] destroyerUsedTranswarpBoosterThisTurn { get; private set; }

		public Hex[,] starshipHexLocation { get; private set; }
		public bool[,] starshipHasRemainingPhaserAttack { get; private set; }
		public bool[,] starshipPhaserSectionIsDestroyed { get; private set; }
		public int[,] starshipPhaserSectionShieldsCurrent { get; private set; }
		public bool[,] starshipUsedPhasersThisTurn { get; private set; }
		public int[,] starshipPhaserRadarShot { get; private set; }
		public bool[,] starshipPhaserRadarArray { get; private set; }
		public bool[,] starshipXRayKernel { get; private set; }
		public bool[,] starshipTractorBeam { get; private set; }
		public bool[,] starshipHasRemainingTorpedoAttack { get; private set; }
		//the unit name and serial number variables in CombatUnit can be populated from playerStarshipNamesPurchased and the array index
		public bool[,] starshipTorpedoSectionIsDestroyed { get; private set; }
		public int[,] starshipTorpedoSectionShieldsCurrent { get; private set; }
		public int[,] starshipTorpedoLaserShot { get; private set; }
		public bool[,] starshipLaserGuidanceSystem { get; private set; }
		public bool[,] starshipHighPressureTubes { get; private set; }
		public int[,] starshipLightTorpedos { get; private set; }
		public int[,] starshipHeavyTorpedos { get; private set; }
		public bool[,] starshipUsedTorpedosThisTurn { get; private set; }
		public bool[,] starshipStorageSectionIsDestroyed { get; private set; }
		public int[,] starshipStorageSectionShieldsCurrent { get; private set; }
		public int[,] starshipDilithiumCrystals { get; private set; }
		public int[,] starshipTriilithiumCrystals { get; private set; }
		public int[,] starshipFlares { get; private set; }
		public bool[,] starshipRadarJammingSystem { get; private set; }
		public bool[,] starshipLaserScatteringSystem { get; private set; }
		public StorageSection.FlareMode[,] starshipFlareMode{ get; private set; }
		public bool[,] starshipCrewSectionIsDestroyed { get; private set; }
		public int[,] starshipCrewSectionShieldsCurrent { get; private set; }
		public bool[,] starshipRepairCrew { get; private set; }
		public bool[,] starshipShieldEngineeringTeam { get; private set; }
		public bool[,] starshipBattleCrew { get; private set; }
		public bool[,] starshipUsedRepairCrewThisTurn { get; private set; }
		public bool[,] starshipEngineSectionIsDestroyed { get; private set; }
		public int[,] starshipEngineSectionShieldsCurrent { get; private set; }
		public int[,] starshipWarpBooster { get; private set; }
		public int[,] starshipTranswarpBooster { get; private set; }
		public bool[,] starshipWarpDrive { get; private set; }
		public bool[,] starshipTranswarpDrive { get; private set; }
		public int[,] starshipDistanceMovedThisTurn { get; private set; }
		public bool[,] starshipUsedWarpBoosterThisTurn { get; private set; }
		public bool[,] starshipUsedTranswarpBoosterThisTurn { get; private set; }

		public Hex[] starbaseHexLocation { get; private set; }
		public bool[] starbaseHasRemainingPhaserAttack { get; private set; }
		public bool[] starbaseHasRemainingTorpedoAttack { get; private set; }
		public string[] starbaseUnitName { get; private set; }
		public bool[] starbaseHasRemainingRepairAction { get; private set; }
		public bool[] starbasePhaserSection1IsDestroyed { get; private set; }
		public int[] starbasePhaserSection1ShieldsCurrent { get; private set; }
		public bool[] starbasePhaserSection1UsedPhasersThisTurn { get; private set; }
		public int[] starbasePhaserRadarShot { get; private set; }
		public bool[] starbasePhaserRadarArray { get; private set; }
		public bool[] starbasePhaserSection2IsDestroyed { get; private set; }
		public int[] starbasePhaserSection2ShieldsCurrent { get; private set; }
		public bool[] starbasePhaserSection2UsedPhasersThisTurn { get; private set; }
		public bool[] starbaseXRayKernel { get; private set; }
		public bool[] starbaseTorpedoSectionIsDestroyed { get; private set; }
		public int[] starbaseTorpedoSectionShieldsCurrent { get; private set; }
		public int[] starbaseTorpedoLaserShot { get; private set; }
		public bool[] starbaseLaserGuidanceSystem { get; private set; }
		public bool[] starbaseHighPressureTubes { get; private set; }
		public int[] starbaseLightTorpedos { get; private set; }
		public int[] starbaseHeavyTorpedos { get; private set; }
		public bool[] starbaseUsedTorpedosThisTurn { get; private set; }
		public bool[] starbaseCrewSectionIsDestroyed { get; private set; }
		public int[] starbaseCrewSectionShieldsCurrent { get; private set; }
		public bool[] starbaseRepairCrew { get; private set; }
		public bool[] starbaseShieldEngineeringTeam { get; private set; }
		public bool[] starbaseBattleCrew { get; private set; }
		public bool[] starbaseUsedRepairCrewThisTurn { get; private set; }
		public bool[] starbaseStorageSection1IsDestroyed { get; private set; }
		public int[] starbaseStorageSection1ShieldsCurrent { get; private set; }
		public int[] starbaseDilithiumCrystals { get; private set; }
		public bool[] starbaseRadarJammingSystem { get; private set; }
		public bool[] starbaseStorageSection2IsDestroyed { get; private set; }
		public int[] starbaseStorageSection2ShieldsCurrent { get; private set; }
		public int[] starbaseTriilithiumCrystals { get; private set; }
		public int[] starbaseFlares { get; private set; }
		public bool[] starbaseLaserScatteringSystem { get; private set; }
		public StarbaseStorageSection2.FlareMode[] starbaseFlareMode{ get; private set; }


		public SaveGameData(){

			fileManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<FileManager>();

			//get the gameManager data
			teamsEnabled = fileManager.gameManager.teamsEnabled;
			currentTurn = fileManager.gameManager.currentTurn;
			firstTurn = fileManager.gameManager.firstTurn;
			currentTurnPhase = fileManager.gameManager.currentTurnPhase;
			gameYear = fileManager.gameManager.gameYear;
			victoryPlanets = fileManager.gameManager.victoryPlanets;

			//set the colony data size
			colonyPlanetName = new string[fileManager.colonyCollector.transform.childCount];
			colonyIsColonized = new bool[fileManager.colonyCollector.transform.childCount];
			colonyColonizingColor = new Player.Color[fileManager.colonyCollector.transform.childCount];
			colonyPlanetHexLocation = new Hex[fileManager.colonyCollector.transform.childCount];

			//set the playerarray size
			playerArray = new Player[GameManager.numberPlayers];


			//define sizes of all the member arrays
			playerColor = new Player.Color[playerArray.Length];
			playerName = new string[playerArray.Length];
			playerMoney = new int[playerArray.Length];
			playerPlanets = new int[playerArray.Length];
			playerIsAlive = new bool[playerArray.Length];
			playerScoutPurchased = new int[playerArray.Length];
			playerScoutNamesPurchased = new string[playerArray.Length,GameManager.maxShipsPerClass];
			playerScoutPurchasedAlive = new bool[playerArray.Length,GameManager.maxShipsPerClass];
			playerBirdOfPreyPurchased = new int[playerArray.Length];
			playerBirdOfPreyNamesPurchased = new string[playerArray.Length,GameManager.maxShipsPerClass];
			playerBirdOfPreyPurchasedAlive = new bool[playerArray.Length,GameManager.maxShipsPerClass];
			playerDestroyerPurchased = new int[playerArray.Length];
			playerDestroyerNamesPurchased = new string[playerArray.Length,GameManager.maxShipsPerClass];
			playerDestroyerPurchasedAlive = new bool[playerArray.Length,GameManager.maxShipsPerClass];
			playerStarshipPurchased = new int[playerArray.Length];
			playerStarshipNamesPurchased = new string[playerArray.Length,GameManager.maxShipsPerClass];
			playerStarshipPurchasedAlive = new bool[playerArray.Length,GameManager.maxShipsPerClass];
			playerStarbaseIsAlive = new bool[playerArray.Length];

			//scout stuff
			scoutHexLocation = new Hex[playerArray.Length,GameManager.maxShipsPerClass];
			scoutHasRemainingPhaserAttack = new bool[playerArray.Length,GameManager.maxShipsPerClass];

			scoutPhaserSectionIsDestroyed = new bool[playerArray.Length,GameManager.maxShipsPerClass];
			scoutPhaserSectionShieldsCurrent = new int[playerArray.Length,GameManager.maxShipsPerClass];
			scoutUsedPhasersThisTurn = new bool[playerArray.Length,GameManager.maxShipsPerClass];
			scoutPhaserRadarShot = new int[playerArray.Length,GameManager.maxShipsPerClass];
			scoutPhaserRadarArray= new bool[playerArray.Length,GameManager.maxShipsPerClass];
			scoutXRayKernel = new bool[playerArray.Length,GameManager.maxShipsPerClass];
			scoutTractorBeam = new bool[playerArray.Length,GameManager.maxShipsPerClass];

			scoutStorageSectionIsDestroyed = new bool[playerArray.Length,GameManager.maxShipsPerClass]; 
			scoutStorageSectionShieldsCurrent = new int[playerArray.Length,GameManager.maxShipsPerClass];
			scoutDilithiumCrystals = new int[playerArray.Length,GameManager.maxShipsPerClass];
			scoutTriilithiumCrystals = new int[playerArray.Length,GameManager.maxShipsPerClass];
			scoutFlares = new int[playerArray.Length,GameManager.maxShipsPerClass];
			scoutRadarJammingSystem = new bool[playerArray.Length,GameManager.maxShipsPerClass]; 
			scoutLaserScatteringSystem = new bool[playerArray.Length,GameManager.maxShipsPerClass]; 
			scoutFlareMode = new StorageSection.FlareMode[playerArray.Length,GameManager.maxShipsPerClass];

			scoutEngineSectionIsDestroyed = new bool[playerArray.Length,GameManager.maxShipsPerClass]; 
			scoutEngineSectionShieldsCurrent = new int[playerArray.Length,GameManager.maxShipsPerClass];
			scoutWarpBooster = new int[playerArray.Length,GameManager.maxShipsPerClass];
			scoutTranswarpBooster = new int[playerArray.Length,GameManager.maxShipsPerClass];
			scoutWarpDrive = new bool[playerArray.Length,GameManager.maxShipsPerClass]; 
			scoutTranswarpDrive = new bool[playerArray.Length,GameManager.maxShipsPerClass]; 
			scoutDistanceMovedThisTurn = new int[playerArray.Length,GameManager.maxShipsPerClass];
			scoutUsedWarpBoosterThisTurn = new bool[playerArray.Length,GameManager.maxShipsPerClass]; 
			scoutUsedTranswarpBoosterThisTurn = new bool[playerArray.Length,GameManager.maxShipsPerClass]; 


			//bird of prey stuff
			birdOfPreyHexLocation = new Hex[playerArray.Length,GameManager.maxShipsPerClass];
			birdOfPreyHasRemainingPhaserAttack = new bool[playerArray.Length,GameManager.maxShipsPerClass];
			birdOfPreyIsCloaked = new bool[playerArray.Length,GameManager.maxShipsPerClass];
			birdOfPreyUsedCloakingDeviceThisTurn = new bool[playerArray.Length,GameManager.maxShipsPerClass];

			birdOfPreyHasRemainingPhaserAttack = new bool[playerArray.Length,GameManager.maxShipsPerClass];
			birdOfPreyPhaserSectionIsDestroyed = new bool[playerArray.Length,GameManager.maxShipsPerClass];
			birdOfPreyPhaserSectionShieldsCurrent = new int[playerArray.Length,GameManager.maxShipsPerClass];
			birdOfPreyUsedPhasersThisTurn = new bool[playerArray.Length,GameManager.maxShipsPerClass];
			birdOfPreyPhaserRadarShot = new int[playerArray.Length,GameManager.maxShipsPerClass];
			birdOfPreyPhaserRadarArray= new bool[playerArray.Length,GameManager.maxShipsPerClass];
			birdOfPreyXRayKernel = new bool[playerArray.Length,GameManager.maxShipsPerClass];
			birdOfPreyTractorBeam = new bool[playerArray.Length,GameManager.maxShipsPerClass];

			birdOfPreyHasRemainingTorpedoAttack = new bool[playerArray.Length,GameManager.maxShipsPerClass];
			birdOfPreyTorpedoSectionIsDestroyed = new bool[playerArray.Length,GameManager.maxShipsPerClass];
			birdOfPreyTorpedoSectionShieldsCurrent = new int[playerArray.Length,GameManager.maxShipsPerClass];
			birdOfPreyTorpedoLaserShot = new int[playerArray.Length,GameManager.maxShipsPerClass];
			birdOfPreyLaserGuidanceSystem = new bool[playerArray.Length,GameManager.maxShipsPerClass];
			birdOfPreyHighPressureTubes = new bool[playerArray.Length,GameManager.maxShipsPerClass];
			birdOfPreyLightTorpedos = new int[playerArray.Length,GameManager.maxShipsPerClass];
			birdOfPreyHeavyTorpedos = new int[playerArray.Length,GameManager.maxShipsPerClass];
			birdOfPreyUsedTorpedosThisTurn = new bool[playerArray.Length,GameManager.maxShipsPerClass];

			birdOfPreyEngineSectionIsDestroyed = new bool[playerArray.Length,GameManager.maxShipsPerClass]; 
			birdOfPreyEngineSectionShieldsCurrent = new int[playerArray.Length,GameManager.maxShipsPerClass];
			birdOfPreyWarpBooster = new int[playerArray.Length,GameManager.maxShipsPerClass];
			birdOfPreyTranswarpBooster = new int[playerArray.Length,GameManager.maxShipsPerClass];
			birdOfPreyWarpDrive = new bool[playerArray.Length,GameManager.maxShipsPerClass]; 
			birdOfPreyTranswarpDrive = new bool[playerArray.Length,GameManager.maxShipsPerClass]; 
			birdOfPreyDistanceMovedThisTurn = new int[playerArray.Length,GameManager.maxShipsPerClass];
			birdOfPreyUsedWarpBoosterThisTurn = new bool[playerArray.Length,GameManager.maxShipsPerClass]; 
			birdOfPreyUsedTranswarpBoosterThisTurn = new bool[playerArray.Length,GameManager.maxShipsPerClass]; 

			//destroyer stuff
			destroyerHexLocation = new Hex[playerArray.Length,GameManager.maxShipsPerClass];
			destroyerHasRemainingPhaserAttack = new bool[playerArray.Length,GameManager.maxShipsPerClass];
			destroyerPhaserSectionIsDestroyed = new bool[playerArray.Length,GameManager.maxShipsPerClass];
			destroyerPhaserSectionShieldsCurrent = new int[playerArray.Length,GameManager.maxShipsPerClass];
			destroyerUsedPhasersThisTurn = new bool[playerArray.Length,GameManager.maxShipsPerClass];
			destroyerPhaserRadarShot = new int[playerArray.Length,GameManager.maxShipsPerClass];
			destroyerPhaserRadarArray= new bool[playerArray.Length,GameManager.maxShipsPerClass];
			destroyerXRayKernel = new bool[playerArray.Length,GameManager.maxShipsPerClass];
			destroyerTractorBeam = new bool[playerArray.Length,GameManager.maxShipsPerClass];

			destroyerHasRemainingTorpedoAttack = new bool[playerArray.Length,GameManager.maxShipsPerClass];
			destroyerTorpedoSectionIsDestroyed = new bool[playerArray.Length,GameManager.maxShipsPerClass];
			destroyerTorpedoSectionShieldsCurrent = new int[playerArray.Length,GameManager.maxShipsPerClass];
			destroyerTorpedoLaserShot = new int[playerArray.Length,GameManager.maxShipsPerClass];
			destroyerLaserGuidanceSystem = new bool[playerArray.Length,GameManager.maxShipsPerClass];
			destroyerHighPressureTubes = new bool[playerArray.Length,GameManager.maxShipsPerClass];
			destroyerLightTorpedos = new int[playerArray.Length,GameManager.maxShipsPerClass];
			destroyerHeavyTorpedos = new int[playerArray.Length,GameManager.maxShipsPerClass];
			destroyerUsedTorpedosThisTurn = new bool[playerArray.Length,GameManager.maxShipsPerClass];

			destroyerStorageSectionIsDestroyed = new bool[playerArray.Length,GameManager.maxShipsPerClass]; 
			destroyerStorageSectionShieldsCurrent = new int[playerArray.Length,GameManager.maxShipsPerClass];
			destroyerDilithiumCrystals = new int[playerArray.Length,GameManager.maxShipsPerClass];
			destroyerTriilithiumCrystals = new int[playerArray.Length,GameManager.maxShipsPerClass];
			destroyerFlares = new int[playerArray.Length,GameManager.maxShipsPerClass];
			destroyerRadarJammingSystem = new bool[playerArray.Length,GameManager.maxShipsPerClass]; 
			destroyerLaserScatteringSystem = new bool[playerArray.Length,GameManager.maxShipsPerClass]; 
			destroyerFlareMode = new StorageSection.FlareMode[playerArray.Length,GameManager.maxShipsPerClass];

			destroyerEngineSectionIsDestroyed = new bool[playerArray.Length,GameManager.maxShipsPerClass]; 
			destroyerEngineSectionShieldsCurrent = new int[playerArray.Length,GameManager.maxShipsPerClass];
			destroyerWarpBooster = new int[playerArray.Length,GameManager.maxShipsPerClass];
			destroyerTranswarpBooster = new int[playerArray.Length,GameManager.maxShipsPerClass];
			destroyerWarpDrive = new bool[playerArray.Length,GameManager.maxShipsPerClass]; 
			destroyerTranswarpDrive = new bool[playerArray.Length,GameManager.maxShipsPerClass]; 
			destroyerDistanceMovedThisTurn = new int[playerArray.Length,GameManager.maxShipsPerClass];
			destroyerUsedWarpBoosterThisTurn = new bool[playerArray.Length,GameManager.maxShipsPerClass]; 
			destroyerUsedTranswarpBoosterThisTurn = new bool[playerArray.Length,GameManager.maxShipsPerClass]; 


			//starship stuff
			starshipHexLocation = new Hex[playerArray.Length,GameManager.maxShipsPerClass];
			starshipHasRemainingPhaserAttack = new bool[playerArray.Length,GameManager.maxShipsPerClass];
			starshipPhaserSectionIsDestroyed = new bool[playerArray.Length,GameManager.maxShipsPerClass];
			starshipPhaserSectionShieldsCurrent = new int[playerArray.Length,GameManager.maxShipsPerClass];
			starshipUsedPhasersThisTurn = new bool[playerArray.Length,GameManager.maxShipsPerClass];
			starshipPhaserRadarShot = new int[playerArray.Length,GameManager.maxShipsPerClass];
			starshipPhaserRadarArray= new bool[playerArray.Length,GameManager.maxShipsPerClass];
			starshipXRayKernel = new bool[playerArray.Length,GameManager.maxShipsPerClass];
			starshipTractorBeam = new bool[playerArray.Length,GameManager.maxShipsPerClass];

			starshipHasRemainingTorpedoAttack = new bool[playerArray.Length,GameManager.maxShipsPerClass];
			starshipTorpedoSectionIsDestroyed = new bool[playerArray.Length,GameManager.maxShipsPerClass];
			starshipTorpedoSectionShieldsCurrent = new int[playerArray.Length,GameManager.maxShipsPerClass];
			starshipTorpedoLaserShot = new int[playerArray.Length,GameManager.maxShipsPerClass];
			starshipLaserGuidanceSystem = new bool[playerArray.Length,GameManager.maxShipsPerClass];
			starshipHighPressureTubes = new bool[playerArray.Length,GameManager.maxShipsPerClass];
			starshipLightTorpedos = new int[playerArray.Length,GameManager.maxShipsPerClass];
			starshipHeavyTorpedos = new int[playerArray.Length,GameManager.maxShipsPerClass];
			starshipUsedTorpedosThisTurn = new bool[playerArray.Length,GameManager.maxShipsPerClass];

			starshipStorageSectionIsDestroyed = new bool[playerArray.Length,GameManager.maxShipsPerClass]; 
			starshipStorageSectionShieldsCurrent = new int[playerArray.Length,GameManager.maxShipsPerClass];
			starshipDilithiumCrystals = new int[playerArray.Length,GameManager.maxShipsPerClass];
			starshipTriilithiumCrystals = new int[playerArray.Length,GameManager.maxShipsPerClass];
			starshipFlares = new int[playerArray.Length,GameManager.maxShipsPerClass];
			starshipRadarJammingSystem = new bool[playerArray.Length,GameManager.maxShipsPerClass]; 
			starshipLaserScatteringSystem = new bool[playerArray.Length,GameManager.maxShipsPerClass]; 
			starshipFlareMode = new StorageSection.FlareMode[playerArray.Length,GameManager.maxShipsPerClass];

			starshipCrewSectionIsDestroyed = new bool[playerArray.Length,GameManager.maxShipsPerClass];
			starshipCrewSectionShieldsCurrent = new int[playerArray.Length,GameManager.maxShipsPerClass];
			starshipRepairCrew = new bool[playerArray.Length,GameManager.maxShipsPerClass];
			starshipShieldEngineeringTeam = new bool[playerArray.Length,GameManager.maxShipsPerClass];
			starshipBattleCrew = new bool[playerArray.Length,GameManager.maxShipsPerClass];
			starshipUsedRepairCrewThisTurn = new bool[playerArray.Length,GameManager.maxShipsPerClass];

			starshipEngineSectionIsDestroyed = new bool[playerArray.Length,GameManager.maxShipsPerClass]; 
			starshipEngineSectionShieldsCurrent = new int[playerArray.Length,GameManager.maxShipsPerClass];
			starshipWarpBooster = new int[playerArray.Length,GameManager.maxShipsPerClass];
			starshipTranswarpBooster = new int[playerArray.Length,GameManager.maxShipsPerClass];
			starshipWarpDrive = new bool[playerArray.Length,GameManager.maxShipsPerClass]; 
			starshipTranswarpDrive = new bool[playerArray.Length,GameManager.maxShipsPerClass]; 
			starshipDistanceMovedThisTurn = new int[playerArray.Length,GameManager.maxShipsPerClass];
			starshipUsedWarpBoosterThisTurn = new bool[playerArray.Length,GameManager.maxShipsPerClass]; 
			starshipUsedTranswarpBoosterThisTurn = new bool[playerArray.Length,GameManager.maxShipsPerClass]; 


			//starbase stuff
			starbaseHexLocation = new Hex[playerArray.Length];
			starbaseHasRemainingPhaserAttack = new bool[playerArray.Length];
			starbaseHasRemainingTorpedoAttack = new bool[playerArray.Length];
			starbaseUnitName = new string[playerArray.Length];
			starbaseHasRemainingRepairAction = new bool[playerArray.Length];

			starbasePhaserSection1IsDestroyed = new bool[playerArray.Length];
			starbasePhaserSection1ShieldsCurrent = new int[playerArray.Length];
			starbasePhaserSection1UsedPhasersThisTurn = new bool[playerArray.Length];
			starbasePhaserRadarShot = new int[playerArray.Length];
			starbasePhaserRadarArray= new bool[playerArray.Length];

			starbasePhaserSection2IsDestroyed = new bool[playerArray.Length];
			starbasePhaserSection2ShieldsCurrent = new int[playerArray.Length];
			starbasePhaserSection2UsedPhasersThisTurn = new bool[playerArray.Length];
			starbaseXRayKernel = new bool[playerArray.Length];

			starbaseTorpedoSectionIsDestroyed = new bool[playerArray.Length];
			starbaseTorpedoSectionShieldsCurrent = new int[playerArray.Length];
			starbaseTorpedoLaserShot = new int[playerArray.Length];
			starbaseLaserGuidanceSystem = new bool[playerArray.Length];
			starbaseHighPressureTubes = new bool[playerArray.Length];
			starbaseLightTorpedos = new int[playerArray.Length];
			starbaseHeavyTorpedos = new int[playerArray.Length];
			starbaseUsedTorpedosThisTurn = new bool[playerArray.Length];

			starbaseCrewSectionIsDestroyed = new bool[playerArray.Length];
			starbaseCrewSectionShieldsCurrent = new int[playerArray.Length];
			starbaseRepairCrew = new bool[playerArray.Length];
			starbaseShieldEngineeringTeam = new bool[playerArray.Length];
			starbaseBattleCrew = new bool[playerArray.Length];
			starbaseUsedRepairCrewThisTurn = new bool[playerArray.Length];

			starbaseStorageSection1IsDestroyed = new bool[playerArray.Length];
			starbaseStorageSection1ShieldsCurrent = new int[playerArray.Length];
			starbaseDilithiumCrystals = new int[playerArray.Length];
			starbaseRadarJammingSystem = new bool[playerArray.Length];

			starbaseStorageSection2IsDestroyed = new bool[playerArray.Length];
			starbaseStorageSection2ShieldsCurrent = new int[playerArray.Length];
			starbaseTriilithiumCrystals = new int[playerArray.Length];
			starbaseFlares = new int[playerArray.Length];
			starbaseLaserScatteringSystem = new bool[playerArray.Length];
			starbaseFlareMode = new StarbaseStorageSection2.FlareMode[playerArray.Length];
							
		}

		//I don't know what this does, but it is required
		public XmlSchema GetSchema(){

			return null;

		}

		//this function is for save info
		public void WriteXml(XmlWriter writer){

			//write the gameManager data
			writer.WriteStartElement("Game");

			writer.WriteStartElement ("teamsEnabled");
			writer.WriteValue (teamsEnabled.ToString().ToLowerInvariant());
			writer.WriteEndElement();

			writer.WriteStartElement ("currentTurn");
			writer.WriteValue (currentTurn.ToString());
			writer.WriteEndElement();

			writer.WriteStartElement ("firstTurn");
			writer.WriteValue (firstTurn.ToString());
			writer.WriteEndElement();

			writer.WriteStartElement ("currentTurnPhase");
			writer.WriteValue (currentTurnPhase.ToString());
			writer.WriteEndElement();

			writer.WriteStartElement ("gameYear");
			writer.WriteValue (gameYear.ToString());
			writer.WriteEndElement();

			writer.WriteStartElement ("victoryPlanets");
			writer.WriteValue (victoryPlanets.ToString());
			writer.WriteEndElement();

			//close Game
			writer.WriteEndElement();

			//start by writing the player data
			writer.WriteStartElement("Player");

			//loop through the players
			for (int i = 0; i < playerArray.Length; i++) {

				//within player, write the player
				writer.WriteStartElement (playerArray[i].name);

				//player color
				writer.WriteStartElement ("playerColor");
				writer.WriteValue (playerArray[i].color.ToString());
				writer.WriteEndElement();

				//player name
				writer.WriteStartElement ("playerName");
				writer.WriteValue (playerArray[i].playerName);
				writer.WriteEndElement();

				//player money
				writer.WriteStartElement ("playerMoney");
				writer.WriteValue (playerArray[i].playerMoney);
				writer.WriteEndElement();

				//player planets
				writer.WriteStartElement ("playerPlanets");
				writer.WriteValue (playerArray[i].playerPlanets);
				writer.WriteEndElement();

				//player alive status
				writer.WriteStartElement ("isAlive");
				writer.WriteValue (playerArray[i].isAlive.ToString().ToLowerInvariant());
				writer.WriteEndElement();

				//player scouts purchased
				writer.WriteStartElement ("playerScoutPurchased");
				writer.WriteValue (playerArray[i].playerScoutPurchased);
				writer.WriteEndElement();

				//loop through the array of the names of scouts purchased
				writer.WriteStartElement ("playerScoutNamesPurchased");

				for (int j = 0; j < playerArray[i].playerScoutPurchased; j++) {

					writer.WriteStartElement("SerialNumber" + j.ToString());

					writer.WriteStartElement ("CombatUnit");

					writer.WriteStartElement ("unitName");
					writer.WriteValue (playerArray[i].playerScoutNamesPurchased[j]);
					writer.WriteEndElement ();

					//write an element for alive status
					writer.WriteStartElement ("playerScoutPurchasedAlive");
					writer.WriteValue (playerArray [i].playerScoutPurchasedAlive [j].ToString ().ToLowerInvariant());
					writer.WriteEndElement ();


					//check if the unit is still alive and if at least that many units have been purchased
					if (playerArray [i].playerScoutPurchasedAlive [j] == true && j < playerArray [i].playerScoutPurchased) {

						//write unit data if alive
						writer.WriteStartElement ("hexLocation");

						writer.WriteStartElement ("column");
						writer.WriteValue (OffsetCoord.RoffsetFromCube(OffsetCoord.ODD,scoutHexLocation [i, j]).col.ToString ());
						writer.WriteEndElement ();

						writer.WriteStartElement ("row");
						writer.WriteValue (OffsetCoord.RoffsetFromCube(OffsetCoord.ODD,scoutHexLocation [i, j]).row.ToString ());
						writer.WriteEndElement ();

						//end hex location
						writer.WriteEndElement ();

						writer.WriteStartElement ("hasRemainingPhaserAttack");
						writer.WriteValue (scoutHasRemainingPhaserAttack [i, j].ToString ().ToLowerInvariant());
						writer.WriteEndElement ();

						//close combat unit
						writer.WriteEndElement ();

						writer.WriteStartElement ("PhaserSection");

						writer.WriteStartElement ("phaserSectionIsDestroyed");
						writer.WriteValue (scoutPhaserSectionIsDestroyed [i, j].ToString ().ToLowerInvariant());
						writer.WriteEndElement ();

						writer.WriteStartElement ("phaserSectionShieldsCurrent");
						writer.WriteValue (scoutPhaserSectionShieldsCurrent [i, j].ToString ());
						writer.WriteEndElement ();

						writer.WriteStartElement ("usedPhasersThisTurn");
						writer.WriteValue (scoutUsedPhasersThisTurn [i, j].ToString ().ToLowerInvariant());
						writer.WriteEndElement ();

						writer.WriteStartElement ("phaserRadarShot");
						writer.WriteValue (scoutPhaserRadarShot [i, j].ToString ());
						writer.WriteEndElement ();

						writer.WriteStartElement ("phaserRadarArray");
						writer.WriteValue (scoutPhaserRadarArray [i, j].ToString ().ToLowerInvariant());
						writer.WriteEndElement ();

						writer.WriteStartElement ("xRayKernel");
						writer.WriteValue (scoutXRayKernel [i, j].ToString ().ToLowerInvariant());
						writer.WriteEndElement ();

						writer.WriteStartElement ("tractorBeam");
						writer.WriteValue (scoutTractorBeam [i, j].ToString ().ToLowerInvariant());
						writer.WriteEndElement ();

						//end phaser section
						writer.WriteEndElement ();

						writer.WriteStartElement ("StorageSection");

						writer.WriteStartElement ("storageSectionIsDestroyed");
						writer.WriteValue (scoutStorageSectionIsDestroyed [i, j].ToString ().ToLowerInvariant());
						writer.WriteEndElement ();

						writer.WriteStartElement ("storageSectionShieldsCurrent");
						writer.WriteValue (scoutStorageSectionShieldsCurrent [i, j].ToString ());
						writer.WriteEndElement ();

						writer.WriteStartElement ("dilithiumCrystals");
						writer.WriteValue (scoutDilithiumCrystals [i, j].ToString ());
						writer.WriteEndElement ();

						writer.WriteStartElement ("triilithiumCrystals");
						writer.WriteValue (scoutTriilithiumCrystals [i, j].ToString ());
						writer.WriteEndElement ();

						writer.WriteStartElement ("flares");
						writer.WriteValue (scoutFlares [i, j].ToString ());
						writer.WriteEndElement ();

						writer.WriteStartElement ("radarJammingSystem");
						writer.WriteValue (scoutRadarJammingSystem [i, j].ToString ().ToLowerInvariant());
						writer.WriteEndElement ();

						writer.WriteStartElement ("laserScatteringSystem");
						writer.WriteValue (scoutLaserScatteringSystem [i, j].ToString ().ToLowerInvariant());
						writer.WriteEndElement ();

						writer.WriteStartElement ("flareMode");
						writer.WriteValue (scoutFlareMode [i, j].ToString ());
						writer.WriteEndElement ();

						//end storage section
						writer.WriteEndElement ();

						writer.WriteStartElement ("EngineSection");

						writer.WriteStartElement ("engineSectionIsDestroyed");
						writer.WriteValue (scoutEngineSectionIsDestroyed [i, j].ToString ().ToLowerInvariant());
						writer.WriteEndElement ();

						writer.WriteStartElement ("engineSectionShieldsCurrent");
						writer.WriteValue (scoutEngineSectionShieldsCurrent [i, j].ToString ());
						writer.WriteEndElement ();

						writer.WriteStartElement ("warpBooster");
						writer.WriteValue (scoutWarpBooster [i, j].ToString ());
						writer.WriteEndElement ();

						writer.WriteStartElement ("transwarpBooster");
						writer.WriteValue (scoutTranswarpBooster [i, j].ToString ());
						writer.WriteEndElement ();

						writer.WriteStartElement ("warpDrive");
						writer.WriteValue (scoutWarpDrive [i, j].ToString ().ToLowerInvariant());
						writer.WriteEndElement ();

						writer.WriteStartElement ("transwarpDrive");
						writer.WriteValue (scoutTranswarpDrive [i, j].ToString ().ToLowerInvariant());
						writer.WriteEndElement ();

						writer.WriteStartElement ("distanceMovedThisTurn");
						writer.WriteValue (scoutDistanceMovedThisTurn [i, j].ToString ());
						writer.WriteEndElement ();

						writer.WriteStartElement ("usedWarpBoosterThisTurn");
						writer.WriteValue (scoutUsedWarpBoosterThisTurn [i, j].ToString ().ToLowerInvariant());
						writer.WriteEndElement ();

						writer.WriteStartElement ("usedTranswarpBoosterThisTurn");
						writer.WriteValue (scoutUsedTranswarpBoosterThisTurn [i, j].ToString ().ToLowerInvariant());
						writer.WriteEndElement ();

						//end engine section
						writer.WriteEndElement ();

					} else {
						
						//close combat unit
						writer.WriteEndElement ();

					}

					//close the j element
					writer.WriteEndElement();

				}

				//close playerScoutNamesPurchased
				writer.WriteEndElement();

				//player bird of prey purchased
				writer.WriteStartElement ("playerBirdOfPreyPurchased");
				writer.WriteValue (playerArray[i].playerBirdOfPreyPurchased);
				writer.WriteEndElement();

				//loop through the array of the names of birds of prey purchased
				writer.WriteStartElement ("playerBirdOfPreyNamesPurchased");

				for (int j = 0; j < playerArray[i].playerBirdOfPreyPurchased; j++) {

					writer.WriteStartElement("SerialNumber" + j.ToString());

					writer.WriteStartElement ("CombatUnit");

					writer.WriteStartElement ("unitName");
					writer.WriteValue (playerArray[i].playerBirdOfPreyNamesPurchased[j]);
					writer.WriteEndElement ();

					//write an element for alive status
					writer.WriteStartElement ("playerBirdOfPreyPurchasedAlive");
					writer.WriteValue (playerArray [i].playerBirdOfPreyPurchasedAlive [j].ToString ().ToLowerInvariant());
					writer.WriteEndElement ();

					//check if the unit is still alive and if at least that many units have been purchased
					if (playerArray [i].playerBirdOfPreyPurchasedAlive [j] == true && j < playerArray [i].playerBirdOfPreyPurchased) {

						//write unit data if alive
						writer.WriteStartElement ("hexLocation");

						writer.WriteStartElement ("column");
						writer.WriteValue (OffsetCoord.RoffsetFromCube(OffsetCoord.ODD,birdOfPreyHexLocation [i, j]).col.ToString ());
						writer.WriteEndElement ();

						writer.WriteStartElement ("row");
						writer.WriteValue (OffsetCoord.RoffsetFromCube(OffsetCoord.ODD,birdOfPreyHexLocation [i, j]).row.ToString ());
						writer.WriteEndElement ();

						//end hex location
						writer.WriteEndElement ();

						writer.WriteStartElement ("hasRemainingPhaserAttack");
						writer.WriteValue (birdOfPreyHasRemainingPhaserAttack [i, j].ToString ().ToLowerInvariant());
						writer.WriteEndElement ();

						writer.WriteStartElement ("hasRemainingTorpedoAttack");
						writer.WriteValue (birdOfPreyHasRemainingTorpedoAttack [i, j].ToString ().ToLowerInvariant());
						writer.WriteEndElement ();

						writer.WriteStartElement ("isCloaked");
						writer.WriteValue (birdOfPreyIsCloaked [i, j].ToString ().ToLowerInvariant());
						writer.WriteEndElement ();

						writer.WriteStartElement ("usedCloakingDeviceThisTurn");
						writer.WriteValue (birdOfPreyUsedCloakingDeviceThisTurn [i, j].ToString ().ToLowerInvariant());
						writer.WriteEndElement ();

						//close combat unit
						writer.WriteEndElement ();

						writer.WriteStartElement ("PhaserSection");

						writer.WriteStartElement ("phaserSectionIsDestroyed");
						writer.WriteValue (birdOfPreyPhaserSectionIsDestroyed [i, j].ToString ().ToLowerInvariant());
						writer.WriteEndElement ();

						writer.WriteStartElement ("phaserSectionShieldsCurrent");
						writer.WriteValue (birdOfPreyPhaserSectionShieldsCurrent [i, j].ToString ());
						writer.WriteEndElement ();

						writer.WriteStartElement ("usedPhasersThisTurn");
						writer.WriteValue (birdOfPreyUsedPhasersThisTurn [i, j].ToString ().ToLowerInvariant());
						writer.WriteEndElement ();

						writer.WriteStartElement ("phaserRadarShot");
						writer.WriteValue (birdOfPreyPhaserRadarShot [i, j].ToString ());
						writer.WriteEndElement ();

						writer.WriteStartElement ("phaserRadarArray");
						writer.WriteValue (birdOfPreyPhaserRadarArray [i, j].ToString ().ToLowerInvariant());
						writer.WriteEndElement ();

						writer.WriteStartElement ("xRayKernel");
						writer.WriteValue (birdOfPreyXRayKernel [i, j].ToString ().ToLowerInvariant());
						writer.WriteEndElement ();

						writer.WriteStartElement ("tractorBeam");
						writer.WriteValue (birdOfPreyTractorBeam [i, j].ToString ().ToLowerInvariant());
						writer.WriteEndElement ();

						//end phaser section
						writer.WriteEndElement ();

						writer.WriteStartElement ("TorpedoSection");

						writer.WriteStartElement ("torpedoSectionIsDestroyed");
						writer.WriteValue (birdOfPreyTorpedoSectionIsDestroyed [i, j].ToString ().ToLowerInvariant());
						writer.WriteEndElement ();

						writer.WriteStartElement ("torpedoSectionShieldsCurrent");
						writer.WriteValue (birdOfPreyTorpedoSectionShieldsCurrent [i, j].ToString ());
						writer.WriteEndElement ();

						writer.WriteStartElement ("torpedoLaserShot");
						writer.WriteValue (birdOfPreyTorpedoLaserShot [i, j].ToString ());
						writer.WriteEndElement ();

						writer.WriteStartElement ("laserGuidanceSystem");
						writer.WriteValue (birdOfPreyLaserGuidanceSystem [i, j].ToString ().ToLowerInvariant());
						writer.WriteEndElement ();

						writer.WriteStartElement ("highPressureTubes");
						writer.WriteValue (birdOfPreyHighPressureTubes [i, j].ToString ().ToLowerInvariant());
						writer.WriteEndElement ();

						writer.WriteStartElement ("lightTorpedos");
						writer.WriteValue (birdOfPreyLightTorpedos [i, j].ToString ());
						writer.WriteEndElement ();

						writer.WriteStartElement ("heavyTorpedos");
						writer.WriteValue (birdOfPreyHeavyTorpedos [i, j].ToString ());
						writer.WriteEndElement ();

						writer.WriteStartElement ("usedTorpedosThisTurn");
						writer.WriteValue (birdOfPreyUsedTorpedosThisTurn [i, j].ToString ().ToLowerInvariant());
						writer.WriteEndElement ();

						//end torpedo section
						writer.WriteEndElement ();

						writer.WriteStartElement ("EngineSection");

						writer.WriteStartElement ("engineSectionIsDestroyed");
						writer.WriteValue (birdOfPreyEngineSectionIsDestroyed [i, j].ToString ().ToLowerInvariant());
						writer.WriteEndElement ();

						writer.WriteStartElement ("engineSectionShieldsCurrent");
						writer.WriteValue (birdOfPreyEngineSectionShieldsCurrent [i, j].ToString ());
						writer.WriteEndElement ();

						writer.WriteStartElement ("warpBooster");
						writer.WriteValue (birdOfPreyWarpBooster [i, j].ToString ());
						writer.WriteEndElement ();

						writer.WriteStartElement ("transwarpBooster");
						writer.WriteValue (birdOfPreyTranswarpBooster [i, j].ToString ());
						writer.WriteEndElement ();

						writer.WriteStartElement ("warpDrive");
						writer.WriteValue (birdOfPreyWarpDrive [i, j].ToString ().ToLowerInvariant());
						writer.WriteEndElement ();

						writer.WriteStartElement ("transwarpDrive");
						writer.WriteValue (birdOfPreyTranswarpDrive [i, j].ToString ().ToLowerInvariant());
						writer.WriteEndElement ();

						writer.WriteStartElement ("distanceMovedThisTurn");
						writer.WriteValue (birdOfPreyDistanceMovedThisTurn [i, j].ToString ());
						writer.WriteEndElement ();

						writer.WriteStartElement ("usedWarpBoosterThisTurn");
						writer.WriteValue (birdOfPreyUsedWarpBoosterThisTurn [i, j].ToString ().ToLowerInvariant());
						writer.WriteEndElement ();

						writer.WriteStartElement ("usedTranswarpBoosterThisTurn");
						writer.WriteValue (birdOfPreyUsedTranswarpBoosterThisTurn [i, j].ToString ().ToLowerInvariant());
						writer.WriteEndElement ();

						//end engine section
						writer.WriteEndElement ();

					} else {

						//close combat unit
						writer.WriteEndElement ();

					}

					//close the j element
					writer.WriteEndElement();

				}

				//close playerBirdOfPreyNamesPurchased
				writer.WriteEndElement();


				//player destroyers purchased
				writer.WriteStartElement ("playerDestroyerPurchased");
				writer.WriteValue (playerArray[i].playerDestroyerPurchased);
				writer.WriteEndElement();

				//loop through the array of the names of destroyers purchased
				writer.WriteStartElement ("playerDestroyerNamesPurchased");

				for (int j = 0; j < playerArray[i].playerDestroyerPurchased; j++) {

					writer.WriteStartElement("SerialNumber" + j.ToString());

					writer.WriteStartElement ("CombatUnit");

					writer.WriteStartElement ("unitName");
					writer.WriteValue (playerArray[i].playerDestroyerNamesPurchased[j]);
					writer.WriteEndElement ();

					//write an element for alive status
					writer.WriteStartElement ("playerDestroyerPurchasedAlive");
					writer.WriteValue (playerArray [i].playerDestroyerPurchasedAlive [j].ToString ().ToLowerInvariant());
					writer.WriteEndElement ();

					//check if the unit is still alive and if at least that many units have been purchased
					if (playerArray [i].playerDestroyerPurchasedAlive [j] == true && j < playerArray [i].playerDestroyerPurchased) {

						//write unit data if alive
						writer.WriteStartElement ("hexLocation");

						writer.WriteStartElement ("column");
						writer.WriteValue (OffsetCoord.RoffsetFromCube(OffsetCoord.ODD,destroyerHexLocation [i, j]).col.ToString ());
						writer.WriteEndElement ();

						writer.WriteStartElement ("row");
						writer.WriteValue (OffsetCoord.RoffsetFromCube(OffsetCoord.ODD,destroyerHexLocation [i, j]).row.ToString ());
						writer.WriteEndElement ();

						//end hex location
						writer.WriteEndElement ();

						writer.WriteStartElement ("hasRemainingPhaserAttack");
						writer.WriteValue (destroyerHasRemainingPhaserAttack [i, j].ToString ().ToLowerInvariant());
						writer.WriteEndElement ();

						writer.WriteStartElement ("hasRemainingTorpedoAttack");
						writer.WriteValue (destroyerHasRemainingTorpedoAttack [i, j].ToString ().ToLowerInvariant());
						writer.WriteEndElement ();

						//close combat unit
						writer.WriteEndElement ();

						writer.WriteStartElement ("PhaserSection");

						writer.WriteStartElement ("phaserSectionIsDestroyed");
						writer.WriteValue (destroyerPhaserSectionIsDestroyed [i, j].ToString ().ToLowerInvariant());
						writer.WriteEndElement ();

						writer.WriteStartElement ("phaserSectionShieldsCurrent");
						writer.WriteValue (destroyerPhaserSectionShieldsCurrent [i, j].ToString ());
						writer.WriteEndElement ();

						writer.WriteStartElement ("usedPhasersThisTurn");
						writer.WriteValue (destroyerUsedPhasersThisTurn [i, j].ToString ().ToLowerInvariant());
						writer.WriteEndElement ();

						writer.WriteStartElement ("phaserRadarShot");
						writer.WriteValue (destroyerPhaserRadarShot [i, j].ToString ());
						writer.WriteEndElement ();

						writer.WriteStartElement ("phaserRadarArray");
						writer.WriteValue (destroyerPhaserRadarArray [i, j].ToString ().ToLowerInvariant());
						writer.WriteEndElement ();

						writer.WriteStartElement ("xRayKernel");
						writer.WriteValue (destroyerXRayKernel [i, j].ToString ().ToLowerInvariant());
						writer.WriteEndElement ();

						writer.WriteStartElement ("tractorBeam");
						writer.WriteValue (destroyerTractorBeam [i, j].ToString ().ToLowerInvariant());
						writer.WriteEndElement ();

						//end phaser section
						writer.WriteEndElement ();

						writer.WriteStartElement ("TorpedoSection");

						writer.WriteStartElement ("torpedoSectionIsDestroyed");
						writer.WriteValue (destroyerTorpedoSectionIsDestroyed [i, j].ToString ().ToLowerInvariant());
						writer.WriteEndElement ();

						writer.WriteStartElement ("torpedoSectionShieldsCurrent");
						writer.WriteValue (destroyerTorpedoSectionShieldsCurrent [i, j].ToString ());
						writer.WriteEndElement ();

						writer.WriteStartElement ("torpedoLaserShot");
						writer.WriteValue (destroyerTorpedoLaserShot [i, j].ToString ());
						writer.WriteEndElement ();

						writer.WriteStartElement ("laserGuidanceSystem");
						writer.WriteValue (destroyerLaserGuidanceSystem [i, j].ToString ().ToLowerInvariant());
						writer.WriteEndElement ();

						writer.WriteStartElement ("highPressureTubes");
						writer.WriteValue (destroyerHighPressureTubes [i, j].ToString ().ToLowerInvariant());
						writer.WriteEndElement ();

						writer.WriteStartElement ("lightTorpedos");
						writer.WriteValue (destroyerLightTorpedos [i, j].ToString ());
						writer.WriteEndElement ();

						writer.WriteStartElement ("heavyTorpedos");
						writer.WriteValue (destroyerHeavyTorpedos [i, j].ToString ());
						writer.WriteEndElement ();

						writer.WriteStartElement ("usedTorpedosThisTurn");
						writer.WriteValue (destroyerUsedTorpedosThisTurn [i, j].ToString ().ToLowerInvariant());
						writer.WriteEndElement ();

						//end torpedo section
						writer.WriteEndElement ();

						writer.WriteStartElement ("StorageSection");

						writer.WriteStartElement ("storageSectionIsDestroyed");
						writer.WriteValue (destroyerStorageSectionIsDestroyed [i, j].ToString ().ToLowerInvariant());
						writer.WriteEndElement ();

						writer.WriteStartElement ("storageSectionShieldsCurrent");
						writer.WriteValue (destroyerStorageSectionShieldsCurrent [i, j].ToString ());
						writer.WriteEndElement ();

						writer.WriteStartElement ("dilithiumCrystals");
						writer.WriteValue (destroyerDilithiumCrystals [i, j].ToString ());
						writer.WriteEndElement ();

						writer.WriteStartElement ("triilithiumCrystals");
						writer.WriteValue (destroyerTriilithiumCrystals [i, j].ToString ());
						writer.WriteEndElement ();

						writer.WriteStartElement ("flares");
						writer.WriteValue (destroyerFlares [i, j].ToString ());
						writer.WriteEndElement ();

						writer.WriteStartElement ("radarJammingSystem");
						writer.WriteValue (destroyerRadarJammingSystem [i, j].ToString ().ToLowerInvariant());
						writer.WriteEndElement ();

						writer.WriteStartElement ("laserScatteringSystem");
						writer.WriteValue (destroyerLaserScatteringSystem [i, j].ToString ().ToLowerInvariant());
						writer.WriteEndElement ();

						writer.WriteStartElement ("flareMode");
						writer.WriteValue (destroyerFlareMode [i, j].ToString ());
						writer.WriteEndElement ();

						//end storage section
						writer.WriteEndElement ();

						writer.WriteStartElement ("EngineSection");

						writer.WriteStartElement ("engineSectionIsDestroyed");
						writer.WriteValue (destroyerEngineSectionIsDestroyed [i, j].ToString ().ToLowerInvariant());
						writer.WriteEndElement ();

						writer.WriteStartElement ("engineSectionShieldsCurrent");
						writer.WriteValue (destroyerEngineSectionShieldsCurrent [i, j].ToString ());
						writer.WriteEndElement ();

						writer.WriteStartElement ("warpBooster");
						writer.WriteValue (destroyerWarpBooster [i, j].ToString ());
						writer.WriteEndElement ();

						writer.WriteStartElement ("transwarpBooster");
						writer.WriteValue (destroyerTranswarpBooster [i, j].ToString ());
						writer.WriteEndElement ();

						writer.WriteStartElement ("warpDrive");
						writer.WriteValue (destroyerWarpDrive [i, j].ToString ().ToLowerInvariant());
						writer.WriteEndElement ();

						writer.WriteStartElement ("transwarpDrive");
						writer.WriteValue (destroyerTranswarpDrive [i, j].ToString ().ToLowerInvariant());
						writer.WriteEndElement ();

						writer.WriteStartElement ("distanceMovedThisTurn");
						writer.WriteValue (destroyerDistanceMovedThisTurn [i, j].ToString ());
						writer.WriteEndElement ();

						writer.WriteStartElement ("usedWarpBoosterThisTurn");
						writer.WriteValue (destroyerUsedWarpBoosterThisTurn [i, j].ToString ().ToLowerInvariant());
						writer.WriteEndElement ();

						writer.WriteStartElement ("usedTranswarpBoosterThisTurn");
						writer.WriteValue (destroyerUsedTranswarpBoosterThisTurn [i, j].ToString ().ToLowerInvariant());
						writer.WriteEndElement ();

						//end engine section
						writer.WriteEndElement ();

					} else {

						//close combat unit
						writer.WriteEndElement ();

					}

					//close the j element
					writer.WriteEndElement();

				}

				//close playerDestroyerNamesPurchased
				writer.WriteEndElement();

				//player starship purchased
				writer.WriteStartElement ("playerStarshipPurchased");
				writer.WriteValue (playerArray[i].playerStarshipPurchased);
				writer.WriteEndElement();

				//loop through the array of the names of starship purchased
				writer.WriteStartElement ("playerStarshipNamesPurchased");

				for (int j = 0; j < playerArray[i].playerStarshipPurchased; j++) {

					writer.WriteStartElement("SerialNumber" + j.ToString());

					writer.WriteStartElement ("CombatUnit");

					writer.WriteStartElement ("unitName");
					writer.WriteValue (playerArray[i].playerStarshipNamesPurchased[j]);
					writer.WriteEndElement ();

					//write an element for alive status
					writer.WriteStartElement ("playerStarshipPurchasedAlive");
					writer.WriteValue (playerArray [i].playerStarshipPurchasedAlive [j].ToString ().ToLowerInvariant());
					writer.WriteEndElement ();

					//check if the unit is still alive and if at least that many units have been purchased
					if (playerArray [i].playerStarshipPurchasedAlive [j] == true && j < playerArray [i].playerStarshipPurchased) {

						//write unit data if alive
						writer.WriteStartElement ("hexLocation");

						writer.WriteStartElement ("column");
						writer.WriteValue (OffsetCoord.RoffsetFromCube(OffsetCoord.ODD,starshipHexLocation [i, j]).col.ToString ());
						writer.WriteEndElement ();

						writer.WriteStartElement ("row");
						writer.WriteValue (OffsetCoord.RoffsetFromCube(OffsetCoord.ODD,starshipHexLocation [i, j]).row.ToString ());
						writer.WriteEndElement ();

						//end hex location
						writer.WriteEndElement ();

						writer.WriteStartElement ("hasRemainingPhaserAttack");
						writer.WriteValue (starshipHasRemainingPhaserAttack [i, j].ToString ().ToLowerInvariant());
						writer.WriteEndElement ();

						writer.WriteStartElement ("hasRemainingTorpedoAttack");
						writer.WriteValue (starshipHasRemainingTorpedoAttack [i, j].ToString ().ToLowerInvariant());
						writer.WriteEndElement ();

						//close combat unit
						writer.WriteEndElement ();

						writer.WriteStartElement ("PhaserSection");

						writer.WriteStartElement ("phaserSectionIsDestroyed");
						writer.WriteValue (starshipPhaserSectionIsDestroyed [i, j].ToString ().ToLowerInvariant());
						writer.WriteEndElement ();

						writer.WriteStartElement ("phaserSectionShieldsCurrent");
						writer.WriteValue (starshipPhaserSectionShieldsCurrent [i, j].ToString ());
						writer.WriteEndElement ();

						writer.WriteStartElement ("usedPhasersThisTurn");
						writer.WriteValue (starshipUsedPhasersThisTurn [i, j].ToString ().ToLowerInvariant());
						writer.WriteEndElement ();

						writer.WriteStartElement ("phaserRadarShot");
						writer.WriteValue (starshipPhaserRadarShot [i, j].ToString ());
						writer.WriteEndElement ();

						writer.WriteStartElement ("phaserRadarArray");
						writer.WriteValue (starshipPhaserRadarArray [i, j].ToString ().ToLowerInvariant());
						writer.WriteEndElement ();

						writer.WriteStartElement ("xRayKernel");
						writer.WriteValue (starshipXRayKernel [i, j].ToString ().ToLowerInvariant());
						writer.WriteEndElement ();

						writer.WriteStartElement ("tractorBeam");
						writer.WriteValue (starshipTractorBeam [i, j].ToString ().ToLowerInvariant());
						writer.WriteEndElement ();

						//end phaser section
						writer.WriteEndElement ();

						writer.WriteStartElement ("TorpedoSection");

						writer.WriteStartElement ("torpedoSectionIsDestroyed");
						writer.WriteValue (starshipTorpedoSectionIsDestroyed [i, j].ToString ().ToLowerInvariant());
						writer.WriteEndElement ();

						writer.WriteStartElement ("torpedoSectionShieldsCurrent");
						writer.WriteValue (starshipTorpedoSectionShieldsCurrent [i, j].ToString ());
						writer.WriteEndElement ();

						writer.WriteStartElement ("torpedoLaserShot");
						writer.WriteValue (starshipTorpedoLaserShot [i, j].ToString ());
						writer.WriteEndElement ();

						writer.WriteStartElement ("laserGuidanceSystem");
						writer.WriteValue (starshipLaserGuidanceSystem [i, j].ToString ().ToLowerInvariant());
						writer.WriteEndElement ();

						writer.WriteStartElement ("highPressureTubes");
						writer.WriteValue (starshipHighPressureTubes [i, j].ToString ().ToLowerInvariant());
						writer.WriteEndElement ();

						writer.WriteStartElement ("lightTorpedos");
						writer.WriteValue (starshipLightTorpedos [i, j].ToString ());
						writer.WriteEndElement ();

						writer.WriteStartElement ("heavyTorpedos");
						writer.WriteValue (starshipHeavyTorpedos [i, j].ToString ());
						writer.WriteEndElement ();

						writer.WriteStartElement ("usedTorpedosThisTurn");
						writer.WriteValue (starshipUsedTorpedosThisTurn [i, j].ToString ().ToLowerInvariant());
						writer.WriteEndElement ();

						//end torpedo section
						writer.WriteEndElement ();

						writer.WriteStartElement ("StorageSection");

						writer.WriteStartElement ("storageSectionIsDestroyed");
						writer.WriteValue (starshipStorageSectionIsDestroyed [i, j].ToString ().ToLowerInvariant());
						writer.WriteEndElement ();

						writer.WriteStartElement ("storageSectionShieldsCurrent");
						writer.WriteValue (starshipStorageSectionShieldsCurrent [i, j].ToString ());
						writer.WriteEndElement ();

						writer.WriteStartElement ("dilithiumCrystals");
						writer.WriteValue (starshipDilithiumCrystals [i, j].ToString ());
						writer.WriteEndElement ();

						writer.WriteStartElement ("triilithiumCrystals");
						writer.WriteValue (starshipTriilithiumCrystals [i, j].ToString ());
						writer.WriteEndElement ();

						writer.WriteStartElement ("flares");
						writer.WriteValue (starshipFlares [i, j].ToString ());
						writer.WriteEndElement ();

						writer.WriteStartElement ("radarJammingSystem");
						writer.WriteValue (starshipRadarJammingSystem [i, j].ToString ().ToLowerInvariant());
						writer.WriteEndElement ();

						writer.WriteStartElement ("laserScatteringSystem");
						writer.WriteValue (starshipLaserScatteringSystem [i, j].ToString ().ToLowerInvariant());
						writer.WriteEndElement ();

						writer.WriteStartElement ("flareMode");
						writer.WriteValue (starshipFlareMode [i, j].ToString ());
						writer.WriteEndElement ();

						//end storage section
						writer.WriteEndElement ();

						writer.WriteStartElement ("CrewSection");

						writer.WriteStartElement ("crewSectionIsDestroyed");
						writer.WriteValue (starshipCrewSectionIsDestroyed [i, j].ToString ().ToLowerInvariant());
						writer.WriteEndElement ();

						writer.WriteStartElement ("crewSectionShieldsCurrent");
						writer.WriteValue (starshipCrewSectionShieldsCurrent [i, j].ToString ());
						writer.WriteEndElement ();

						writer.WriteStartElement ("repairCrew");
						writer.WriteValue (starshipRepairCrew [i, j].ToString ().ToLowerInvariant());
						writer.WriteEndElement ();

						writer.WriteStartElement ("shieldEngineeringTeam");
						writer.WriteValue (starshipShieldEngineeringTeam [i, j].ToString ().ToLowerInvariant());
						writer.WriteEndElement ();

						writer.WriteStartElement ("battleCrew");
						writer.WriteValue (starshipBattleCrew [i, j].ToString ().ToLowerInvariant());
						writer.WriteEndElement ();

						writer.WriteStartElement ("usedRepairCrewThisTurn");
						writer.WriteValue (starshipUsedRepairCrewThisTurn [i, j].ToString ().ToLowerInvariant());
						writer.WriteEndElement ();

						//end crew section
						writer.WriteEndElement ();

						writer.WriteStartElement ("EngineSection");

						writer.WriteStartElement ("engineSectionIsDestroyed");
						writer.WriteValue (starshipEngineSectionIsDestroyed [i, j].ToString ().ToLowerInvariant());
						writer.WriteEndElement ();

						writer.WriteStartElement ("engineSectionShieldsCurrent");
						writer.WriteValue (starshipEngineSectionShieldsCurrent [i, j].ToString ());
						writer.WriteEndElement ();

						writer.WriteStartElement ("warpBooster");
						writer.WriteValue (starshipWarpBooster [i, j].ToString ());
						writer.WriteEndElement ();

						writer.WriteStartElement ("transwarpBooster");
						writer.WriteValue (starshipTranswarpBooster [i, j].ToString ());
						writer.WriteEndElement ();

						writer.WriteStartElement ("warpDrive");
						writer.WriteValue (starshipWarpDrive [i, j].ToString ().ToLowerInvariant());
						writer.WriteEndElement ();

						writer.WriteStartElement ("transwarpDrive");
						writer.WriteValue (starshipTranswarpDrive [i, j].ToString ().ToLowerInvariant());
						writer.WriteEndElement ();

						writer.WriteStartElement ("distanceMovedThisTurn");
						writer.WriteValue (starshipDistanceMovedThisTurn [i, j].ToString ());
						writer.WriteEndElement ();

						writer.WriteStartElement ("usedWarpBoosterThisTurn");
						writer.WriteValue (starshipUsedWarpBoosterThisTurn [i, j].ToString ().ToLowerInvariant());
						writer.WriteEndElement ();

						writer.WriteStartElement ("usedTranswarpBoosterThisTurn");
						writer.WriteValue (starshipUsedTranswarpBoosterThisTurn [i, j].ToString ().ToLowerInvariant());
						writer.WriteEndElement ();

						//end engine section
						writer.WriteEndElement ();

					} else {

						//close combat unit
						writer.WriteEndElement ();

					}

					//close the j element
					writer.WriteEndElement();

				}

				//close playerStarshipNamesPurchased
				writer.WriteEndElement();

				//record starbase status
				writer.WriteStartElement ("playerStarbaseAlive");
				writer.WriteValue (playerArray[i].playerStarbaseAlive.ToString().ToLowerInvariant());
				writer.WriteEndElement();

				//check if the starbase is alive
				if (playerArray [i].playerStarbaseAlive == true) {

					//write the detail status
					writer.WriteStartElement ("starbaseDetails");

					//write the combatUnit level details
					writer.WriteStartElement ("CombatUnit");

					writer.WriteStartElement ("unitName");
					writer.WriteValue (starbaseUnitName[i]);
					writer.WriteEndElement ();

					writer.WriteStartElement ("hexLocation");

					writer.WriteStartElement ("column");
					writer.WriteValue (OffsetCoord.RoffsetFromCube(OffsetCoord.ODD,starbaseHexLocation [i]).col.ToString ());
					writer.WriteEndElement ();

					writer.WriteStartElement ("row");
					writer.WriteValue (OffsetCoord.RoffsetFromCube(OffsetCoord.ODD,starbaseHexLocation [i]).row.ToString ());
					writer.WriteEndElement ();

					//end hex location
					writer.WriteEndElement ();

					writer.WriteStartElement ("hasRemainingPhaserAttack");
					writer.WriteValue (starbaseHasRemainingPhaserAttack[i].ToString ().ToLowerInvariant());
					writer.WriteEndElement ();

					writer.WriteStartElement ("hasRemainingTorpedoAttack");
					writer.WriteValue (starbaseHasRemainingTorpedoAttack[i].ToString ().ToLowerInvariant());
					writer.WriteEndElement ();

					writer.WriteStartElement ("hasRemainingRepairAction");
					writer.WriteValue (starbaseHasRemainingRepairAction[i].ToString ().ToLowerInvariant());
					writer.WriteEndElement ();

					//end combatUnit
					writer.WriteEndElement();

					writer.WriteStartElement ("PhaserSection1");

					writer.WriteStartElement ("phaserSection1IsDestroyed");
					writer.WriteValue (starbasePhaserSection1IsDestroyed [i].ToString ().ToLowerInvariant());
					writer.WriteEndElement ();

					writer.WriteStartElement ("phaserSection1ShieldsCurrent");
					writer.WriteValue (starbasePhaserSection1ShieldsCurrent [i].ToString ());
					writer.WriteEndElement ();

					writer.WriteStartElement ("phaserSection1UsedPhasersThisTurn");
					writer.WriteValue (starbasePhaserSection1UsedPhasersThisTurn [i].ToString ().ToLowerInvariant());
					writer.WriteEndElement ();

					writer.WriteStartElement ("phaserRadarShot");
					writer.WriteValue (starbasePhaserRadarShot [i].ToString ());
					writer.WriteEndElement ();

					writer.WriteStartElement ("phaserRadarArray");
					writer.WriteValue (starbasePhaserRadarArray [i].ToString ().ToLowerInvariant());
					writer.WriteEndElement ();

					//end PhaserSection1
					writer.WriteEndElement();

					writer.WriteStartElement ("PhaserSection2");

					writer.WriteStartElement ("phaserSection2IsDestroyed");
					writer.WriteValue (starbasePhaserSection2IsDestroyed [i].ToString ().ToLowerInvariant());
					writer.WriteEndElement ();

					writer.WriteStartElement ("phaserSection2ShieldsCurrent");
					writer.WriteValue (starbasePhaserSection2ShieldsCurrent [i].ToString ());
					writer.WriteEndElement ();

					writer.WriteStartElement ("phaserSection2UsedPhasersThisTurn");
					writer.WriteValue (starbasePhaserSection2UsedPhasersThisTurn [i].ToString ().ToLowerInvariant());
					writer.WriteEndElement ();

					writer.WriteStartElement ("xRayKernel");
					writer.WriteValue (starbaseXRayKernel [i].ToString ().ToLowerInvariant());
					writer.WriteEndElement ();

					//end PhaserSection2
					writer.WriteEndElement();

					writer.WriteStartElement ("TorpedoSection");

					writer.WriteStartElement ("torpedoSectionIsDestroyed");
					writer.WriteValue (starbaseTorpedoSectionIsDestroyed [i].ToString ().ToLowerInvariant());
					writer.WriteEndElement ();

					writer.WriteStartElement ("torpedoSectionShieldsCurrent");
					writer.WriteValue (starbaseTorpedoSectionShieldsCurrent [i].ToString ()); 
					writer.WriteEndElement ();

					writer.WriteStartElement ("torpedoLaserShot");
					writer.WriteValue (starbaseTorpedoLaserShot [i].ToString ());
					writer.WriteEndElement ();

					writer.WriteStartElement ("laserGuidanceSystem");
					writer.WriteValue (starbaseLaserGuidanceSystem [i].ToString ().ToLowerInvariant());
					writer.WriteEndElement ();

					writer.WriteStartElement ("highPressureTubes");
					writer.WriteValue (starbaseHighPressureTubes [i].ToString ().ToLowerInvariant());
					writer.WriteEndElement ();

					writer.WriteStartElement ("lightTorpedos");
					writer.WriteValue (starbaseLightTorpedos [i].ToString ());
					writer.WriteEndElement ();

					writer.WriteStartElement ("heavyTorpedos");
					writer.WriteValue (starbaseHeavyTorpedos [i].ToString ());
					writer.WriteEndElement ();

					writer.WriteStartElement ("usedTorpedosThisTurn");
					writer.WriteValue (starbaseUsedTorpedosThisTurn [i].ToString ().ToLowerInvariant());
					writer.WriteEndElement ();

					//end torpedoSection
					writer.WriteEndElement();

					writer.WriteStartElement ("CrewSection");

					writer.WriteStartElement ("crewSectionIsDestroyed");
					writer.WriteValue (starbaseCrewSectionIsDestroyed [i].ToString ().ToLowerInvariant());
					writer.WriteEndElement ();

					writer.WriteStartElement ("crewSectionShieldsCurrent");
					writer.WriteValue (starbaseCrewSectionShieldsCurrent [i].ToString ()); 
					writer.WriteEndElement ();

					writer.WriteStartElement ("repairCrew");
					writer.WriteValue (starbaseRepairCrew [i].ToString ().ToLowerInvariant());
					writer.WriteEndElement ();

					writer.WriteStartElement ("shieldEngineeringTeam");
					writer.WriteValue (starbaseShieldEngineeringTeam [i].ToString ().ToLowerInvariant());
					writer.WriteEndElement ();

					writer.WriteStartElement ("battleCrew");
					writer.WriteValue (starbaseBattleCrew [i].ToString ().ToLowerInvariant());
					writer.WriteEndElement ();

					writer.WriteStartElement ("usedRepairCrewThisTurn");
					writer.WriteValue (starbaseUsedRepairCrewThisTurn [i].ToString ().ToLowerInvariant());
					writer.WriteEndElement ();

					//end crewSection
					writer.WriteEndElement();

					writer.WriteStartElement ("StorageSection1");

					writer.WriteStartElement ("storageSection1IsDestroyed");
					writer.WriteValue (starbaseStorageSection1IsDestroyed [i].ToString ().ToLowerInvariant());
					writer.WriteEndElement ();

					writer.WriteStartElement ("storageSection1ShieldsCurrent");
					writer.WriteValue (starbaseStorageSection1ShieldsCurrent [i].ToString ()); 
					writer.WriteEndElement ();

					writer.WriteStartElement ("dilithiumCrystals");
					writer.WriteValue (starbaseDilithiumCrystals [i].ToString ());
					writer.WriteEndElement ();

					writer.WriteStartElement ("radarJammingSystem");
					writer.WriteValue (starbaseRadarJammingSystem [i].ToString ().ToLowerInvariant());
					writer.WriteEndElement ();

					//end StorageSection1
					writer.WriteEndElement();

					writer.WriteStartElement ("StorageSection2");

					writer.WriteStartElement ("storageSection2IsDestroyed");
					writer.WriteValue (starbaseStorageSection2IsDestroyed [i].ToString ().ToLowerInvariant());
					writer.WriteEndElement ();

					writer.WriteStartElement ("storageSection2ShieldsCurrent");
					writer.WriteValue (starbaseStorageSection2ShieldsCurrent [i].ToString ()); 
					writer.WriteEndElement ();

					writer.WriteStartElement ("triilithiumCrystals");
					writer.WriteValue (starbaseTriilithiumCrystals [i].ToString ());
					writer.WriteEndElement ();

					writer.WriteStartElement ("flares");
					writer.WriteValue (starbaseFlares [i].ToString ());
					writer.WriteEndElement ();

					writer.WriteStartElement ("laserScatteringSystem");
					writer.WriteValue (starbaseLaserScatteringSystem [i].ToString ().ToLowerInvariant());
					writer.WriteEndElement ();

					writer.WriteStartElement ("flareMode");
					writer.WriteValue (starbaseFlareMode [i].ToString ());
					writer.WriteEndElement ();

					//end StorageSection2
					writer.WriteEndElement();


					//end starbaseDetails
					writer.WriteEndElement();

				}

				//close the player
				writer.WriteEndElement();

			}

			//close player
			writer.WriteEndElement();

			//write the planet colony data
			writer.WriteStartElement ("PlanetColonies");

			//loop through the colony data array
			for (int i = 0; i < fileManager.colonyCollector.transform.childCount; i++) {

				//write each planet
				writer.WriteStartElement (colonyPlanetName[i]);

				//write the color
				if (colonyIsColonized[i] == true) {

					writer.WriteStartElement ("planetName");
					writer.WriteValue (colonyPlanetName[i]);
					writer.WriteEndElement ();

					writer.WriteStartElement ("ownerColor");
					writer.WriteValue (colonyColonizingColor[i].ToString ());
					writer.WriteEndElement ();

					writer.WriteStartElement ("hexLocation");


					writer.WriteStartElement ("column");
					writer.WriteValue (OffsetCoord.RoffsetFromCube(OffsetCoord.ODD,colonyPlanetHexLocation[i]).col.ToString ());
					writer.WriteEndElement ();

					writer.WriteStartElement ("row");
					writer.WriteValue (OffsetCoord.RoffsetFromCube(OffsetCoord.ODD,colonyPlanetHexLocation[i]).row.ToString ());
					writer.WriteEndElement ();

					//end hex location
					writer.WriteEndElement ();

				} else {

					writer.WriteStartElement ("planetName");
					writer.WriteValue (colonyPlanetName[i]);
					writer.WriteEndElement ();

					writer.WriteStartElement ("ownerColor");
					writer.WriteValue ("Uncolonized");
					writer.WriteEndElement ();

				}

				//close planet
				writer.WriteEndElement();

			}

			//close planet colonies
			writer.WriteEndElement();

		}

		//this function is for load info
		public void ReadXml(XmlReader reader){

			//now move to the element - this is the SaveGameData element
			reader.ReadStartElement();

			//now move to the next element - this is the game element
			reader.ReadStartElement();

			//read the element and store value - teamsEnabled
			teamsEnabled = reader.ReadElementContentAsBoolean();

			//do a switch case on reading the next element content for currentTurn
			switch (reader.ReadElementContentAsString ()) {

			case "Green":

				currentTurn = Player.Color.Green;
				break;

			case "Purple":

				currentTurn = Player.Color.Purple;
				break;

			case "Red":

				currentTurn = Player.Color.Red;
				break;

			case "Blue":

				currentTurn = Player.Color.Blue;
				break;

			default:

				Debug.LogError ("Couldn't Read Color for current turn");
				break;

			}

			//do a switch case on reading the next element content for firstTurn
			switch (reader.ReadElementContentAsString ()) {

			case "Green":

				firstTurn = Player.Color.Green;
				break;

			case "Purple":

				firstTurn = Player.Color.Purple;
				break;

			case "Red":

				firstTurn = Player.Color.Red;
				break;

			case "Blue":

				firstTurn = Player.Color.Blue;
				break;

			default:

				Debug.LogError ("Couldn't Read Color for first turn");
				break;

			}

			//do a switch case on reading the next element content for currentTurnPhase
			switch (reader.ReadElementContentAsString ()) {

			case "PurchasePhase":

				currentTurnPhase = GameManager.TurnPhase.PurchasePhase;
				break;

			case "MainPhase":

				currentTurnPhase = GameManager.TurnPhase.MainPhase;
				break;

			default:

				Debug.LogError ("Couldn't Read currentTurnPhase");
				break;

			}

			//read the next element as the game year
			gameYear = reader.ReadElementContentAsInt ();

			//read the next element as the number of victory planets
			victoryPlanets = reader.ReadElementContentAsInt ();

			//read the close of the Game element
			reader.ReadEndElement();

			//now move to the next element - this is the Player element
			reader.ReadStartElement();

			//we can now set up a loop to run through the 4 player elements
			for (int i = 0; i < GameManager.numberPlayers; i++) {

				//read the player colorPlayer element
				reader.ReadStartElement ();

				//do a switch case on reading the next element content for player color
				switch (reader.ReadElementContentAsString ()) {

				case "Green":

					playerColor [i] = Player.Color.Green;
					break;

				case "Purple":

					playerColor [i] = Player.Color.Purple;
					break;

				case "Red":

					playerColor [i] = Player.Color.Red;
					break;

				case "Blue":

					playerColor [i] = Player.Color.Blue;
					break;

				default:

					Debug.LogError ("Couldn't Read Color");
					break;

				}

				//read element for playerName
				playerName [i] = reader.ReadElementContentAsString ();

				//read element for playerMoney
				playerMoney [i] = reader.ReadElementContentAsInt ();

				//read element for playerPlanets
				playerPlanets [i] = reader.ReadElementContentAsInt ();

				//read element for playerIsAlive
				playerIsAlive [i] = reader.ReadElementContentAsBoolean ();

				//read element for playerScoutPurchased
				playerScoutPurchased [i] = reader.ReadElementContentAsInt ();

				//check if the ships purchased is greater than 0
				if (playerScoutPurchased [i] > 0) {

					//now that we know how many scouts were purchased, we can loop through the ships
					//the start element is ScoutNamesPurchased
					reader.ReadStartElement ();

					//loop through the number of ships purchased
					for (int j = 0; j < playerScoutPurchased [i]; j++) {

						//the start element is the serial number
						reader.ReadStartElement ();

						//the start element is the combatUnit
						reader.ReadStartElement ();

						//read element for unit name
						playerScoutNamesPurchased [i, j] = reader.ReadElementContentAsString ();

						//read element for scout alive
						playerScoutPurchasedAlive [i, j] = reader.ReadElementContentAsBoolean ();

						//check if the scout is alive.  If it is, there will be additional data
						if (playerScoutPurchasedAlive [i, j] == true) {

							//the start element is the hexLocation
							reader.ReadStartElement ();

							//store the hex location
							scoutHexLocation [i, j] = OffsetCoord.RoffsetToCube(OffsetCoord.ODD, new OffsetCoord(reader.ReadElementContentAsInt (),reader.ReadElementContentAsInt ()));

							//the end element is the hexLocation
							reader.ReadEndElement ();

							//read element for scout remaining phaser attack
							scoutHasRemainingPhaserAttack [i, j] = reader.ReadElementContentAsBoolean ();

							//the end element is the CombatUnit
							reader.ReadEndElement ();

							//the start element is the PhaserSection
							reader.ReadStartElement ();

							//read element for phaserSectionIsDestroyed
							scoutPhaserSectionIsDestroyed [i, j] = reader.ReadElementContentAsBoolean ();

							//read element for phaserSectionShieldsCurrent
							scoutPhaserSectionShieldsCurrent [i, j] = reader.ReadElementContentAsInt ();

							//read element for usedPhasersThisTurn
							scoutUsedPhasersThisTurn [i, j] = reader.ReadElementContentAsBoolean ();

							//read element for phaserRadarShot
							scoutPhaserRadarShot [i, j] = reader.ReadElementContentAsInt ();

							//read element for phaserRadarArray
							scoutPhaserRadarArray [i, j] = reader.ReadElementContentAsBoolean ();

							//read element for xRayKernel
							scoutXRayKernel [i, j] = reader.ReadElementContentAsBoolean ();

							//read element for tractorBeam
							scoutTractorBeam [i, j] = reader.ReadElementContentAsBoolean ();

							//the end element is the PhaserSection
							reader.ReadEndElement ();

							//the start element is the StorageSection
							reader.ReadStartElement ();

							//read element for storageSectionIsDestroyed
							scoutStorageSectionIsDestroyed [i, j] = reader.ReadElementContentAsBoolean ();

							//read element for storageSectionShieldsCurrent
							scoutStorageSectionShieldsCurrent [i, j] = reader.ReadElementContentAsInt ();

							//read element for dilithium crystals
							scoutDilithiumCrystals [i, j] = reader.ReadElementContentAsInt ();

							//read element for trilithium crystals
							scoutTriilithiumCrystals [i, j] = reader.ReadElementContentAsInt ();

							//read element for flares
							scoutFlares [i, j] = reader.ReadElementContentAsInt ();

							//read element for radarJammingSystem
							scoutRadarJammingSystem [i, j] = reader.ReadElementContentAsBoolean ();

							//read element for laserScatteringSystem
							scoutLaserScatteringSystem [i, j] = reader.ReadElementContentAsBoolean ();

							//switch case to read flare mode
							switch (reader.ReadElementContentAsString ()) {

							case "Manual":

								scoutFlareMode [i, j] = StorageSection.FlareMode.Manual;
								break;

							case "Auto":

								scoutFlareMode [i, j] = StorageSection.FlareMode.Auto;
								break;

							default:

								Debug.LogError ("Could not set Flare Mode");
								break;

							}

							//the end element is the StorageSection
							reader.ReadEndElement ();

							//the start element is the EngineSection
							reader.ReadStartElement ();

							//read element for engineSectionIsDestroyed
							scoutEngineSectionIsDestroyed [i, j] = reader.ReadElementContentAsBoolean ();

							//read element for engineSectionShieldsCurrent
							scoutEngineSectionShieldsCurrent [i, j] = reader.ReadElementContentAsInt ();

							//read element for warp booster
							scoutWarpBooster [i, j] = reader.ReadElementContentAsInt ();

							//read element for transwarp booster
							scoutTranswarpBooster [i, j] = reader.ReadElementContentAsInt ();

							//read element for warpDrive
							scoutWarpDrive [i, j] = reader.ReadElementContentAsBoolean ();

							//read element for transwarpDrive
							scoutTranswarpDrive [i, j] = reader.ReadElementContentAsBoolean ();

							//read element for distanceMovedThisTurn
							scoutDistanceMovedThisTurn [i, j] = reader.ReadElementContentAsInt ();

							//read element for usedWarpBoosterThisTurn
							scoutUsedWarpBoosterThisTurn [i, j] = reader.ReadElementContentAsBoolean ();

							//read element for usedTranswarpBoosterThisTurn
							scoutUsedTranswarpBoosterThisTurn [i, j] = reader.ReadElementContentAsBoolean ();

							//the end element is the EngineSection
							reader.ReadEndElement ();

						} else {

							//the else condition is that the scout is not alive.  Extra data is then omitted
							//the end element is the CombatUnit
							reader.ReadEndElement ();

						}

						//the start element is the serial number
						reader.ReadEndElement ();

					}

					//the end element is the playerScoutNamesPurchased
					reader.ReadEndElement ();

				} else {

					//if no ships were purchased, just advance the reader
					reader.Read();

				}

				//read element for playerBirdOfPreyPurchased
				playerBirdOfPreyPurchased [i] = reader.ReadElementContentAsInt ();

				//check if the ships purchased is greater than 0
				if (playerBirdOfPreyPurchased [i] > 0) {

					//now that we know how many birds of prey were purchased, we can loop through the ships
					//the start element is BirdOfPreyNamesPurchased
					reader.ReadStartElement ();

					//loop through the number of ships purchased
					for (int j = 0; j < playerBirdOfPreyPurchased [i]; j++) {

						//the start element is the serial number
						reader.ReadStartElement ();

						//the start element is the combatUnit
						reader.ReadStartElement ();

						//read element for unit name
						playerBirdOfPreyNamesPurchased [i, j] = reader.ReadElementContentAsString ();

						//read element for bird of prey alive
						playerBirdOfPreyPurchasedAlive [i, j] = reader.ReadElementContentAsBoolean ();

						//check if the bird of prey is alive.  If it is, there will be additional data
						if (playerBirdOfPreyPurchasedAlive [i, j] == true) {

							//the start element is the hexLocation
							reader.ReadStartElement ();

							//store the hex location
							birdOfPreyHexLocation [i, j] = OffsetCoord.RoffsetToCube(OffsetCoord.ODD, new OffsetCoord(reader.ReadElementContentAsInt (),reader.ReadElementContentAsInt ()));

							//the end element is the hexLocation
							reader.ReadEndElement ();

							//read element for remaining phaser attack
							birdOfPreyHasRemainingPhaserAttack [i, j] = reader.ReadElementContentAsBoolean ();

							//read element for remaining torpedo attack
							birdOfPreyHasRemainingTorpedoAttack [i, j] = reader.ReadElementContentAsBoolean ();

							//read element for isCloaked
							birdOfPreyIsCloaked [i, j] = reader.ReadElementContentAsBoolean ();

							//read element for usedCloakingDeviceThisTurn
							birdOfPreyUsedCloakingDeviceThisTurn [i, j] = reader.ReadElementContentAsBoolean ();

							//the end element is the CombatUnit
							reader.ReadEndElement ();

							//the start element is the PhaserSection
							reader.ReadStartElement ();

							//read element for phaserSectionIsDestroyed
							birdOfPreyPhaserSectionIsDestroyed [i, j] = reader.ReadElementContentAsBoolean ();

							//read element for phaserSectionShieldsCurrent
							birdOfPreyPhaserSectionShieldsCurrent [i, j] = reader.ReadElementContentAsInt ();

							//read element for usedPhasersThisTurn
							birdOfPreyUsedPhasersThisTurn [i, j] = reader.ReadElementContentAsBoolean ();

							//read element for phaserRadarShot
							birdOfPreyPhaserRadarShot [i, j] = reader.ReadElementContentAsInt ();

							//read element for phaserRadarArray
							birdOfPreyPhaserRadarArray [i, j] = reader.ReadElementContentAsBoolean ();

							//read element for xRayKernel
							birdOfPreyXRayKernel [i, j] = reader.ReadElementContentAsBoolean ();

							//read element for tractorBeam
							birdOfPreyTractorBeam [i, j] = reader.ReadElementContentAsBoolean ();

							//the end element is the PhaserSection
							reader.ReadEndElement ();

							//the start element is the TorpedoSection
							reader.ReadStartElement ();

							//read element for torpedostorageSectionIsDestroyed
							birdOfPreyTorpedoSectionIsDestroyed [i, j] = reader.ReadElementContentAsBoolean ();

							//read element for torpedoSectionShieldsCurrent
							birdOfPreyTorpedoSectionShieldsCurrent [i, j] = reader.ReadElementContentAsInt ();

							//read element for laser shot
							birdOfPreyTorpedoLaserShot [i, j] = reader.ReadElementContentAsInt ();

							//read element for laser guidance system
							birdOfPreyLaserGuidanceSystem [i, j] = reader.ReadElementContentAsBoolean ();

							//read element for high pressure tubes
							birdOfPreyHighPressureTubes [i, j] = reader.ReadElementContentAsBoolean ();

							//read element for light torpedos
							birdOfPreyLightTorpedos [i, j] = reader.ReadElementContentAsInt ();

							//read element for heavy torpedos
							birdOfPreyHeavyTorpedos [i, j] = reader.ReadElementContentAsInt ();

							//read element for high usedTorpedoThisTurn
							birdOfPreyUsedTorpedosThisTurn [i, j] = reader.ReadElementContentAsBoolean ();

							//the end element is the TorpedoSection
							reader.ReadEndElement ();

							//the start element is the EngineSection
							reader.ReadStartElement ();

							//read element for engineSectionIsDestroyed
							birdOfPreyEngineSectionIsDestroyed [i, j] = reader.ReadElementContentAsBoolean ();

							//read element for engineSectionShieldsCurrent
							birdOfPreyEngineSectionShieldsCurrent [i, j] = reader.ReadElementContentAsInt ();

							//read element for warp booster
							birdOfPreyWarpBooster [i, j] = reader.ReadElementContentAsInt ();

							//read element for transwarp booster
							birdOfPreyTranswarpBooster [i, j] = reader.ReadElementContentAsInt ();

							//read element for warpDrive
							birdOfPreyWarpDrive [i, j] = reader.ReadElementContentAsBoolean ();

							//read element for transwarpDrive
							birdOfPreyTranswarpDrive [i, j] = reader.ReadElementContentAsBoolean ();

							//read element for distanceMovedThisTurn
							birdOfPreyDistanceMovedThisTurn [i, j] = reader.ReadElementContentAsInt ();

							//read element for usedWarpBoosterThisTurn
							birdOfPreyUsedWarpBoosterThisTurn [i, j] = reader.ReadElementContentAsBoolean ();

							//read element for usedTranswarpBoosterThisTurn
							birdOfPreyUsedTranswarpBoosterThisTurn [i, j] = reader.ReadElementContentAsBoolean ();

							//the end element is the EngineSection
							reader.ReadEndElement ();

						} else {

							//the else condition is that the bird of prey is not alive.  Extra data is then omitted
							//the end element is the CombatUnit
							reader.ReadEndElement ();

						}

						//the start element is the serial number
						reader.ReadEndElement ();

					}

					//the end element is the playerBirdOfPreyNamesPurchased
					reader.ReadEndElement ();

				} else {

					//if no ships were purchased, just advance the reader
					reader.Read();

				}

				//read element for playerDestroyerPurchased
				playerDestroyerPurchased [i] = reader.ReadElementContentAsInt ();

				//check if the ships purchased is greater than 0
				if (playerDestroyerPurchased [i] > 0) {

					//now that we know how many destroyers were purchased, we can loop through the ships
					//the start element is DestroyerNamesPurchased
					reader.ReadStartElement ();

					//loop through the number of ships purchased
					for (int j = 0; j < playerDestroyerPurchased [i]; j++) {

						//the start element is the serial number
						reader.ReadStartElement ();

						//the start element is the combatUnit
						reader.ReadStartElement ();

						//read element for unit name
						playerDestroyerNamesPurchased [i, j] = reader.ReadElementContentAsString ();

						//read element for destroyer alive
						playerDestroyerPurchasedAlive [i, j] = reader.ReadElementContentAsBoolean ();

						//check if the destroyer is alive.  If it is, there will be additional data
						if (playerDestroyerPurchasedAlive [i, j] == true) {

							//the start element is the hexLocation
							reader.ReadStartElement ();

							//store the hex location
							destroyerHexLocation [i, j] = OffsetCoord.RoffsetToCube(OffsetCoord.ODD, new OffsetCoord(reader.ReadElementContentAsInt (),reader.ReadElementContentAsInt ()));

							//the end element is the hexLocation
							reader.ReadEndElement ();

							//read element for remaining phaser attack
							destroyerHasRemainingPhaserAttack [i, j] = reader.ReadElementContentAsBoolean ();

							//read element for remaining torpedo attack
							destroyerHasRemainingTorpedoAttack [i, j] = reader.ReadElementContentAsBoolean ();

							//the end element is the CombatUnit
							reader.ReadEndElement ();

							//the start element is the PhaserSection
							reader.ReadStartElement ();

							//read element for phaserSectionIsDestroyed
							destroyerPhaserSectionIsDestroyed [i, j] = reader.ReadElementContentAsBoolean ();

							//read element for phaserSectionShieldsCurrent
							destroyerPhaserSectionShieldsCurrent [i, j] = reader.ReadElementContentAsInt ();

							//read element for usedPhasersThisTurn
							destroyerUsedPhasersThisTurn [i, j] = reader.ReadElementContentAsBoolean ();

							//read element for phaserRadarShot
							destroyerPhaserRadarShot [i, j] = reader.ReadElementContentAsInt ();

							//read element for phaserRadarArray
							destroyerPhaserRadarArray [i, j] = reader.ReadElementContentAsBoolean ();

							//read element for xRayKernel
							destroyerXRayKernel [i, j] = reader.ReadElementContentAsBoolean ();

							//read element for tractorBeam
							destroyerTractorBeam [i, j] = reader.ReadElementContentAsBoolean ();

							//the end element is the PhaserSection
							reader.ReadEndElement ();

							//the start element is the TorpedoSection
							reader.ReadStartElement ();

							//read element for torpedostorageSectionIsDestroyed
							destroyerTorpedoSectionIsDestroyed [i, j] = reader.ReadElementContentAsBoolean ();

							//read element for torpedoSectionShieldsCurrent
							destroyerTorpedoSectionShieldsCurrent [i, j] = reader.ReadElementContentAsInt ();

							//read element for laser shot
							destroyerTorpedoLaserShot [i, j] = reader.ReadElementContentAsInt ();

							//read element for laser guidance system
							destroyerLaserGuidanceSystem [i, j] = reader.ReadElementContentAsBoolean ();

							//read element for high pressure tubes
							destroyerHighPressureTubes [i, j] = reader.ReadElementContentAsBoolean ();

							//read element for light torpedos
							destroyerLightTorpedos [i, j] = reader.ReadElementContentAsInt ();

							//read element for heavy torpedos
							destroyerHeavyTorpedos [i, j] = reader.ReadElementContentAsInt ();

							//read element for high usedTorpedoThisTurn
							destroyerUsedTorpedosThisTurn [i, j] = reader.ReadElementContentAsBoolean ();

							//the end element is the TorpedoSection
							reader.ReadEndElement ();

							//the start element is the StorageSection
							reader.ReadStartElement ();

							//read element for storageSectionIsDestroyed
							destroyerStorageSectionIsDestroyed [i, j] = reader.ReadElementContentAsBoolean ();

							//read element for storageSectionShieldsCurrent
							destroyerStorageSectionShieldsCurrent [i, j] = reader.ReadElementContentAsInt ();

							//read element for dilithium crystals
							destroyerDilithiumCrystals [i, j] = reader.ReadElementContentAsInt ();

							//read element for trilithium crystals
							destroyerTriilithiumCrystals [i, j] = reader.ReadElementContentAsInt ();

							//read element for flares
							destroyerFlares [i, j] = reader.ReadElementContentAsInt ();

							//read element for radarJammingSystem
							destroyerRadarJammingSystem [i, j] = reader.ReadElementContentAsBoolean ();

							//read element for laserScatteringSystem
							destroyerLaserScatteringSystem [i, j] = reader.ReadElementContentAsBoolean ();

							//switch case to read flare mode
							switch (reader.ReadElementContentAsString ()) {

							case "Manual":

								destroyerFlareMode [i, j] = StorageSection.FlareMode.Manual;
								break;

							case "Auto":

								destroyerFlareMode [i, j] = StorageSection.FlareMode.Auto;
								break;

							default:

								Debug.LogError ("Could not set Flare Mode");
								break;

							}

							//the end element is the StorageSection
							reader.ReadEndElement ();

							//the start element is the EngineSection
							reader.ReadStartElement ();

							//read element for engineSectionIsDestroyed
							destroyerEngineSectionIsDestroyed [i, j] = reader.ReadElementContentAsBoolean ();

							//read element for engineSectionShieldsCurrent
							destroyerEngineSectionShieldsCurrent [i, j] = reader.ReadElementContentAsInt ();

							//read element for warp booster
							destroyerWarpBooster [i, j] = reader.ReadElementContentAsInt ();

							//read element for transwarp booster
							destroyerTranswarpBooster [i, j] = reader.ReadElementContentAsInt ();

							//read element for warpDrive
							destroyerWarpDrive [i, j] = reader.ReadElementContentAsBoolean ();

							//read element for transwarpDrive
							destroyerTranswarpDrive [i, j] = reader.ReadElementContentAsBoolean ();

							//read element for distanceMovedThisTurn
							destroyerDistanceMovedThisTurn [i, j] = reader.ReadElementContentAsInt ();

							//read element for usedWarpBoosterThisTurn
							destroyerUsedWarpBoosterThisTurn [i, j] = reader.ReadElementContentAsBoolean ();

							//read element for usedTranswarpBoosterThisTurn
							destroyerUsedTranswarpBoosterThisTurn [i, j] = reader.ReadElementContentAsBoolean ();

							//the end element is the EngineSection
							reader.ReadEndElement ();

						} else {

							//the else condition is that the destroyer is not alive.  Extra data is then omitted
							//the end element is the CombatUnit
							reader.ReadEndElement ();

						}

						//the start element is the serial number
						reader.ReadEndElement ();

					}

					//the end element is the playerDestroyerNamesPurchased
					reader.ReadEndElement ();

				} else {

					//if no ships were purchased, just advance the reader
					reader.Read();

				}

				//read element for playerStarshipPurchased
				playerStarshipPurchased [i] = reader.ReadElementContentAsInt ();

				//check if the ships purchased is greater than 0
				if (playerStarshipPurchased [i] > 0) {

					//now that we know how many destroyers were purchased, we can loop through the ships
					//the start element is StarshipNamesPurchased
					reader.ReadStartElement ();

					//loop through the number of ships purchased
					for (int j = 0; j < playerStarshipPurchased [i]; j++) {

						//the start element is the serial number
						reader.ReadStartElement ();

						//the start element is the combatUnit
						reader.ReadStartElement ();

						//read element for unit name
						playerStarshipNamesPurchased [i, j] = reader.ReadElementContentAsString ();

						//read element for starship alive
						playerStarshipPurchasedAlive [i, j] = reader.ReadElementContentAsBoolean ();

						//check if the starship is alive.  If it is, there will be additional data
						if (playerStarshipPurchasedAlive [i, j] == true) {

							//the start element is the hexLocation
							reader.ReadStartElement ();

							//store the hex location
							starshipHexLocation [i, j] = OffsetCoord.RoffsetToCube(OffsetCoord.ODD, new OffsetCoord(reader.ReadElementContentAsInt (),reader.ReadElementContentAsInt ()));

							//the end element is the hexLocation
							reader.ReadEndElement ();

							//read element for remaining phaser attack
							starshipHasRemainingPhaserAttack [i, j] = reader.ReadElementContentAsBoolean ();

							//read element for remaining torpedo attack
							starshipHasRemainingTorpedoAttack [i, j] = reader.ReadElementContentAsBoolean ();

							//the end element is the CombatUnit
							reader.ReadEndElement ();

							//the start element is the PhaserSection
							reader.ReadStartElement ();

							//read element for phaserSectionIsDestroyed
							starshipPhaserSectionIsDestroyed [i, j] = reader.ReadElementContentAsBoolean ();

							//read element for phaserSectionShieldsCurrent
							starshipPhaserSectionShieldsCurrent [i, j] = reader.ReadElementContentAsInt ();

							//read element for usedPhasersThisTurn
							starshipUsedPhasersThisTurn [i, j] = reader.ReadElementContentAsBoolean ();

							//read element for phaserRadarShot
							starshipPhaserRadarShot [i, j] = reader.ReadElementContentAsInt ();

							//read element for phaserRadarArray
							starshipPhaserRadarArray [i, j] = reader.ReadElementContentAsBoolean ();

							//read element for xRayKernel
							starshipXRayKernel [i, j] = reader.ReadElementContentAsBoolean ();

							//read element for tractorBeam
							starshipTractorBeam [i, j] = reader.ReadElementContentAsBoolean ();

							//the end element is the PhaserSection
							reader.ReadEndElement ();

							//the start element is the TorpedoSection
							reader.ReadStartElement ();

							//read element for torpedostorageSectionIsDestroyed
							starshipTorpedoSectionIsDestroyed [i, j] = reader.ReadElementContentAsBoolean ();

							//read element for torpedoSectionShieldsCurrent
							starshipTorpedoSectionShieldsCurrent [i, j] = reader.ReadElementContentAsInt ();

							//read element for laser shot
							starshipTorpedoLaserShot [i, j] = reader.ReadElementContentAsInt ();

							//read element for laser guidance system
							starshipLaserGuidanceSystem [i, j] = reader.ReadElementContentAsBoolean ();

							//read element for high pressure tubes
							starshipHighPressureTubes [i, j] = reader.ReadElementContentAsBoolean ();

							//read element for light torpedos
							starshipLightTorpedos [i, j] = reader.ReadElementContentAsInt ();

							//read element for heavy torpedos
							destroyerHeavyTorpedos [i, j] = reader.ReadElementContentAsInt ();

							//read element for high usedTorpedoThisTurn
							starshipUsedTorpedosThisTurn [i, j] = reader.ReadElementContentAsBoolean ();

							//the end element is the TorpedoSection
							reader.ReadEndElement ();

							//the start element is the StorageSection
							reader.ReadStartElement ();

							//read element for storageSectionIsDestroyed
							starshipStorageSectionIsDestroyed [i, j] = reader.ReadElementContentAsBoolean ();

							//read element for storageSectionShieldsCurrent
							starshipStorageSectionShieldsCurrent [i, j] = reader.ReadElementContentAsInt ();

							//read element for dilithium crystals
							starshipDilithiumCrystals [i, j] = reader.ReadElementContentAsInt ();

							//read element for trilithium crystals
							starshipTriilithiumCrystals [i, j] = reader.ReadElementContentAsInt ();

							//read element for flares
							starshipFlares [i, j] = reader.ReadElementContentAsInt ();

							//read element for radarJammingSystem
							starshipRadarJammingSystem [i, j] = reader.ReadElementContentAsBoolean ();

							//read element for laserScatteringSystem
							starshipLaserScatteringSystem [i, j] = reader.ReadElementContentAsBoolean ();

							//switch case to read flare mode
							switch (reader.ReadElementContentAsString ()) {

							case "Manual":

								starshipFlareMode [i, j] = StorageSection.FlareMode.Manual;
								break;

							case "Auto":

								starshipFlareMode [i, j] = StorageSection.FlareMode.Auto;
								break;

							default:

								Debug.LogError ("Could not set Flare Mode");
								break;

							}

							//the end element is the StorageSection
							reader.ReadEndElement ();

							//the start element is the CrewSection
							reader.ReadStartElement ();

							//read element for crewSectionIsDestroyed
							starshipCrewSectionIsDestroyed [i, j] = reader.ReadElementContentAsBoolean ();

							//read element for crewSectionShieldsCurrent
							starshipCrewSectionShieldsCurrent [i, j] = reader.ReadElementContentAsInt ();

							//read element for repair crew
							starshipRepairCrew [i, j] = reader.ReadElementContentAsBoolean ();

							//read element for shield engineering team
							starshipShieldEngineeringTeam [i, j] = reader.ReadElementContentAsBoolean ();

							//read element for battle crew
							starshipBattleCrew [i, j] = reader.ReadElementContentAsBoolean ();

							//read element for used repair crew
							starshipUsedRepairCrewThisTurn [i, j] = reader.ReadElementContentAsBoolean ();

							//the end element is the CrewSection
							reader.ReadEndElement ();

							//the start element is the EngineSection
							reader.ReadStartElement ();

							//read element for engineSectionIsDestroyed
							starshipEngineSectionIsDestroyed [i, j] = reader.ReadElementContentAsBoolean ();

							//read element for engineSectionShieldsCurrent
							starshipEngineSectionShieldsCurrent [i, j] = reader.ReadElementContentAsInt ();

							//read element for warp booster
							starshipWarpBooster [i, j] = reader.ReadElementContentAsInt ();

							//read element for transwarp booster
							starshipTranswarpBooster [i, j] = reader.ReadElementContentAsInt ();

							//read element for warpDrive
							starshipWarpDrive [i, j] = reader.ReadElementContentAsBoolean ();

							//read element for transwarpDrive
							starshipTranswarpDrive [i, j] = reader.ReadElementContentAsBoolean ();

							//read element for distanceMovedThisTurn
							starshipDistanceMovedThisTurn [i, j] = reader.ReadElementContentAsInt ();

							//read element for usedWarpBoosterThisTurn
							starshipUsedWarpBoosterThisTurn [i, j] = reader.ReadElementContentAsBoolean ();

							//read element for usedTranswarpBoosterThisTurn
							starshipUsedTranswarpBoosterThisTurn [i, j] = reader.ReadElementContentAsBoolean ();

							//the end element is the EngineSection
							reader.ReadEndElement ();

						} else {

							//the else condition is that the starship is not alive.  Extra data is then omitted
							//the end element is the CombatUnit
							reader.ReadEndElement ();

						}

						//the start element is the serial number
						reader.ReadEndElement ();

					}

					//the end element is the playerStarshipNamesPurchased
					reader.ReadEndElement ();

				}  else {

					//if no ships were purchased, just advance the reader
					reader.Read();

				}

				//read element for starbaseAlive
				playerStarbaseIsAlive[i] = reader.ReadElementContentAsBoolean();

				//if the starbase is alive, there will be a bunch of subsequent details
				if (playerStarbaseIsAlive [i] == true) {

					//the start element is starbaseDetails
					reader.ReadStartElement();

					//the start element is CombatUnit
					reader.ReadStartElement();

					//read element for starbaseUnitName
					starbaseUnitName [i] = reader.ReadElementContentAsString ();

					//the start element is hex location
					reader.ReadStartElement();

					//store the hex location
					starbaseHexLocation [i] = OffsetCoord.RoffsetToCube(OffsetCoord.ODD, new OffsetCoord(reader.ReadElementContentAsInt (),reader.ReadElementContentAsInt ()));

					//the end element is the hexLocation
					reader.ReadEndElement ();

					//read element for hasRemainingPhaserAttack
					starbaseHasRemainingPhaserAttack [i] = reader.ReadElementContentAsBoolean ();

					//read element for hasRemainingTorpedoAttack
					starbaseHasRemainingTorpedoAttack [i] = reader.ReadElementContentAsBoolean ();

					//read element for hasRemainingRepairAction
					starbaseHasRemainingRepairAction [i] = reader.ReadElementContentAsBoolean ();

					//the end element is the CombatUnit
					reader.ReadEndElement ();

					//the start element is PhaserSection1
					reader.ReadStartElement();

					//read element for phaserSection1IsDestroyed
					starbasePhaserSection1IsDestroyed[i] = reader.ReadElementContentAsBoolean ();

					//read element for phaserSection1CurrentShields
					starbasePhaserSection1ShieldsCurrent[i] = reader.ReadElementContentAsInt ();

					//read element for phaserSection1UsedPhasers
					starbasePhaserSection1UsedPhasersThisTurn[i] = reader.ReadElementContentAsBoolean ();

					//read element for phaserRadarShot
					starbasePhaserRadarShot[i] = reader.ReadElementContentAsInt ();

					//read element for phaserRadarArray
					starbasePhaserRadarArray[i] = reader.ReadElementContentAsBoolean ();

					//the end element is the PhaserSection1
					reader.ReadEndElement ();

					//the start element is PhaserSection2
					reader.ReadStartElement();

					//read element for phaserSection2IsDestroyed
					starbasePhaserSection2IsDestroyed[i] = reader.ReadElementContentAsBoolean ();

					//read element for phaserSection2CurrentShields
					starbasePhaserSection2ShieldsCurrent[i] = reader.ReadElementContentAsInt ();

					//read element for phaserSection2UsedPhasers
					starbasePhaserSection2UsedPhasersThisTurn[i] = reader.ReadElementContentAsBoolean ();

					//read element for xRayKernel
					starbaseXRayKernel[i] = reader.ReadElementContentAsBoolean ();

					//the end element is the PhaserSection2
					reader.ReadEndElement ();

					//the start element is TorpedoSection
					reader.ReadStartElement();

					//read element for torpedoSectionIsDestroyed
					starbaseTorpedoSectionIsDestroyed[i] = reader.ReadElementContentAsBoolean ();

					//read element for torpedoSectionCurrentShields
					starbaseTorpedoSectionShieldsCurrent[i] = reader.ReadElementContentAsInt ();

					//read element for torpedoLaserShot
					starbaseTorpedoLaserShot[i] = reader.ReadElementContentAsInt ();

					//read element for laserGuidanceSystem
					starbaseLaserGuidanceSystem[i] = reader.ReadElementContentAsBoolean ();

					//read element for highPressureTubes
					starbaseHighPressureTubes[i] = reader.ReadElementContentAsBoolean ();

					//read element for lightTorpedos
					starbaseLightTorpedos[i] = reader.ReadElementContentAsInt ();

					//read element for heavyTorpedos
					starbaseHeavyTorpedos[i] = reader.ReadElementContentAsInt ();

					//read element for usedTorpedosThisTurn
					starbaseUsedTorpedosThisTurn[i] = reader.ReadElementContentAsBoolean ();

					//the end element is the TorpedoSection
					reader.ReadEndElement ();

					//the start element is CrewSection
					reader.ReadStartElement();

					//read element for crewSectionIsDestroyed
					starbaseCrewSectionIsDestroyed[i] = reader.ReadElementContentAsBoolean ();

					//read element for crewSectionCurrentShields
					starbaseCrewSectionShieldsCurrent[i] = reader.ReadElementContentAsInt ();

					//read element for repair crew
					starbaseRepairCrew[i] = reader.ReadElementContentAsBoolean ();

					//read element for shieldEngineeringTeam
					starbaseShieldEngineeringTeam[i] = reader.ReadElementContentAsBoolean ();

					//read element for battleCrew
					starbaseBattleCrew[i] = reader.ReadElementContentAsBoolean ();

					//read element for usedRepairCrewThisTurn
					starbaseUsedRepairCrewThisTurn[i] = reader.ReadElementContentAsBoolean ();

					//the end element is the CrewSection
					reader.ReadEndElement ();

					//the start element is StorageSection1
					reader.ReadStartElement();

					//read element for StorageSection1IsDestroyed
					starbaseStorageSection1IsDestroyed[i] = reader.ReadElementContentAsBoolean ();

					//read element for StorageSection1CurrentShields
					starbaseStorageSection1ShieldsCurrent[i] = reader.ReadElementContentAsInt ();

					//read element for dilithium crystals
					starbaseDilithiumCrystals[i] = reader.ReadElementContentAsInt();

					//read element for radarJammingSystem
					starbaseRadarJammingSystem[i] = reader.ReadElementContentAsBoolean ();

					//the end element is the StorageSection1
					reader.ReadEndElement ();

					//the start element is StorageSection2
					reader.ReadStartElement();

					//read element for StorageSection2IsDestroyed
					starbaseStorageSection2IsDestroyed[i] = reader.ReadElementContentAsBoolean ();

					//read element for StorageSection2CurrentShields
					starbaseStorageSection2ShieldsCurrent[i] = reader.ReadElementContentAsInt ();

					//read element for trilithium crystals
					starbaseTriilithiumCrystals[i] = reader.ReadElementContentAsInt();

					//read element for flares
					starbaseFlares[i] = reader.ReadElementContentAsInt();

					//read element for laserScatteringSystem
					starbaseLaserScatteringSystem[i] = reader.ReadElementContentAsBoolean ();

					//switch case to read flare mode
					switch (reader.ReadElementContentAsString ()) {

					case "Manual":

						starbaseFlareMode [i] = StarbaseStorageSection2.FlareMode.Manual;
						break;

					case "Auto":

						starbaseFlareMode [i] = StarbaseStorageSection2.FlareMode.Auto;
						break;

					default:

						Debug.LogError ("Could not set Flare Mode");
						break;

					}

					//the end element is the StorageSection2
					reader.ReadEndElement ();

					//the end element is the starbaseDetails
					reader.ReadEndElement ();


				} 

				//read the player colorPlayer element
				reader.ReadEndElement ();

			}

			//close the player 
			reader.ReadEndElement ();


			//the start element is PlanetColonies
			reader.ReadStartElement();

			//loop through the number of colonies
			for (int i = 0; i < fileManager.colonyCollector.transform.childCount; i++) {

				//the start element is the planet name
				reader.ReadStartElement();

				//read element for planet name
				colonyPlanetName[i] = reader.ReadElementContentAsString();


				//read element for ownerColor using switch case
				switch (reader.ReadElementContentAsString ()) {

				case "Green":

					colonyIsColonized[i] = true;
					colonyColonizingColor[i] = Player.Color.Green;

					break;

				case "Purple":

					colonyIsColonized[i] = true;
					colonyColonizingColor[i] = Player.Color.Purple;
					break;

				case "Red":

					colonyIsColonized[i] = true;
					colonyColonizingColor[i] = Player.Color.Red;
					break;

				case "Blue":

					colonyIsColonized[i] = true;
					colonyColonizingColor[i] = Player.Color.Blue;
					break;

				default:

					//colonyData [i].isColonized = false;
					colonyIsColonized [i] = false;
					break;

				}

				//check if the planet is colonized.  If it is, there will be hex location data
				if (colonyIsColonized[i] == true) {
					
					//the start element is hex location
					reader.ReadStartElement();

					//store the hex location
					colonyPlanetHexLocation[i] = OffsetCoord.RoffsetToCube(OffsetCoord.ODD, new OffsetCoord(reader.ReadElementContentAsInt (),reader.ReadElementContentAsInt ()));


					//the end element is the hexLocation
					reader.ReadEndElement ();

				}

				//the end element is the planetName
				reader.ReadEndElement ();

			}

			//the end element is the PlanetColonies
			reader.ReadEndElement ();

			//the end element is the SaveGameData
			reader.ReadEndElement ();

		}

		//function for populating saveGameData when saving
		public void PopulateSaveGameDataForSave(){

			//initialize the player array with the 4 players
			playerArray[0] = fileManager.gameManager.greenPlayer;
			playerArray[1] = fileManager.gameManager.purplePlayer;
			playerArray[2] = fileManager.gameManager.redPlayer;
			playerArray[3] = fileManager.gameManager.bluePlayer;


			//loop through each player in the player array
			for(int i = 0; i < playerArray.Length; i++){

				//set player variables
				playerColor[i] = playerArray[i].color;
				playerName[i] = playerArray[i].playerName;
				playerMoney[i] = playerArray[i].playerMoney;
				playerPlanets[i] = playerArray[i].playerPlanets;
				playerIsAlive[i] = playerArray[i].isAlive;
				playerScoutPurchased[i] = playerArray[i].playerScoutPurchased;
				playerBirdOfPreyPurchased[i] = playerArray[i].playerBirdOfPreyPurchased;
				playerDestroyerPurchased[i] = playerArray[i].playerDestroyerPurchased;
				playerStarshipPurchased[i] = playerArray[i].playerStarshipPurchased;
				playerStarbaseIsAlive[i] = playerArray[i].playerStarbaseAlive;

				//for the 2-D arrays, loop through the number of ships
				for (int j = 0; j < GameManager.maxShipsPerClass; j++) {

					playerScoutNamesPurchased[i,j] = playerArray[i].playerScoutNamesPurchased[j];
					playerScoutPurchasedAlive[i,j] = playerArray[i].playerScoutPurchasedAlive[j];

					playerBirdOfPreyNamesPurchased[i,j] = playerArray[i].playerBirdOfPreyNamesPurchased[j];
					playerBirdOfPreyPurchasedAlive[i,j] = playerArray[i].playerBirdOfPreyPurchasedAlive[j];

					playerDestroyerNamesPurchased[i,j] = playerArray[i].playerDestroyerNamesPurchased[j];
					playerDestroyerPurchasedAlive[i,j] = playerArray[i].playerDestroyerPurchasedAlive[j];

					playerStarshipNamesPurchased[i,j] = playerArray[i].playerStarshipNamesPurchased[j];
					playerStarshipPurchasedAlive[i,j] = playerArray[i].playerStarshipPurchasedAlive[j];

				}

			}

			//loop through the unit collectors
			for(int i = 0; i < fileManager.collectors.Length; i++){

				//get a list of all children gameObjects under the collector
				List<CombatUnit> childCombatUnits = new List<CombatUnit>();

				for(int j = 0; j < fileManager.collectors[i].transform.childCount; j++){

					childCombatUnits.Add(fileManager.collectors[i].transform.GetChild(j).GetComponent<CombatUnit>());

				}

				//loop through each combat unit 
				foreach(CombatUnit combatUnit in childCombatUnits){

					//do a switch case on the unit type
					switch (combatUnit.unitType){

					case CombatUnit.UnitType.Scout:

						//populate the unit attribute data
						scoutHexLocation[i,combatUnit.serialNumber] = combatUnit.currentLocation;
						scoutHasRemainingPhaserAttack[i,combatUnit.serialNumber] = combatUnit.hasRemainingPhaserAttack;
						scoutPhaserSectionIsDestroyed[i,combatUnit.serialNumber] = combatUnit.GetComponent<PhaserSection>().isDestroyed;
						scoutPhaserSectionShieldsCurrent[i,combatUnit.serialNumber] = combatUnit.GetComponent<PhaserSection>().shieldsCurrent;
						scoutUsedPhasersThisTurn[i,combatUnit.serialNumber] = combatUnit.GetComponent<PhaserSection>().usedPhasersThisTurn;
						scoutPhaserRadarShot[i,combatUnit.serialNumber] = combatUnit.GetComponent<PhaserSection>().phaserRadarShot;
						scoutPhaserRadarArray[i,combatUnit.serialNumber]= combatUnit.GetComponent<PhaserSection>().phaserRadarArray;
						scoutXRayKernel[i,combatUnit.serialNumber] = combatUnit.GetComponent<PhaserSection>().xRayKernalUpgrade;
						scoutTractorBeam[i,combatUnit.serialNumber] = combatUnit.GetComponent<PhaserSection>().tractorBeam;

						scoutStorageSectionIsDestroyed[i,combatUnit.serialNumber] = combatUnit.GetComponent<StorageSection>().isDestroyed;
						scoutStorageSectionShieldsCurrent[i,combatUnit.serialNumber] = combatUnit.GetComponent<StorageSection>().shieldsCurrent;
						scoutDilithiumCrystals[i,combatUnit.serialNumber] = combatUnit.GetComponent<StorageSection>().dilithiumCrystals;
						scoutTriilithiumCrystals[i,combatUnit.serialNumber] = combatUnit.GetComponent<StorageSection>().trilithiumCrystals;
						scoutFlares[i,combatUnit.serialNumber] = combatUnit.GetComponent<StorageSection>().flares;
						scoutRadarJammingSystem[i,combatUnit.serialNumber] = combatUnit.GetComponent<StorageSection>().radarJammingSystem;
						scoutLaserScatteringSystem[i,combatUnit.serialNumber] = combatUnit.GetComponent<StorageSection>().laserScatteringSystem; 
						scoutFlareMode[i,combatUnit.serialNumber] = combatUnit.GetComponent<StorageSection>().flareMode;

						scoutEngineSectionIsDestroyed[i,combatUnit.serialNumber] = combatUnit.GetComponent<EngineSection>().isDestroyed; 
						scoutEngineSectionShieldsCurrent[i,combatUnit.serialNumber] = combatUnit.GetComponent<EngineSection>().shieldsCurrent;
						scoutWarpBooster[i,combatUnit.serialNumber] = combatUnit.GetComponent<EngineSection>().warpBooster;
						scoutTranswarpBooster[i,combatUnit.serialNumber] = combatUnit.GetComponent<EngineSection>().transwarpBooster;
						scoutWarpDrive[i,combatUnit.serialNumber] = combatUnit.GetComponent<EngineSection>().warpDrive;
						scoutTranswarpDrive[i,combatUnit.serialNumber] = combatUnit.GetComponent<EngineSection>().transwarpDrive;
						scoutDistanceMovedThisTurn[i,combatUnit.serialNumber] = combatUnit.GetComponent<EngineSection>().distanceMovedThisTurn;
						scoutUsedWarpBoosterThisTurn[i,combatUnit.serialNumber] = combatUnit.GetComponent<EngineSection>().usedWarpBoosterThisTurn;
						scoutUsedTranswarpBoosterThisTurn[i,combatUnit.serialNumber] = combatUnit.GetComponent<EngineSection>().usedTranswarpBoosterThisTurn; 

						break;

					case CombatUnit.UnitType.BirdOfPrey:

						//populate the unit attribute data
						birdOfPreyHexLocation [i, combatUnit.serialNumber] = combatUnit.currentLocation;
						birdOfPreyHasRemainingPhaserAttack [i, combatUnit.serialNumber] = combatUnit.hasRemainingPhaserAttack;
						birdOfPreyIsCloaked [i, combatUnit.serialNumber] = combatUnit.GetComponent<CloakingDevice> ().isCloaked;
						birdOfPreyUsedCloakingDeviceThisTurn [i, combatUnit.serialNumber] = combatUnit.GetComponent<CloakingDevice> ().usedCloakingDeviceThisTurn;
						birdOfPreyPhaserSectionIsDestroyed[i,combatUnit.serialNumber] = combatUnit.GetComponent<PhaserSection>().isDestroyed;
						birdOfPreyPhaserSectionShieldsCurrent[i,combatUnit.serialNumber] = combatUnit.GetComponent<PhaserSection>().shieldsCurrent;
						birdOfPreyUsedPhasersThisTurn[i,combatUnit.serialNumber] = combatUnit.GetComponent<PhaserSection>().usedPhasersThisTurn;
						birdOfPreyPhaserRadarShot[i,combatUnit.serialNumber] = combatUnit.GetComponent<PhaserSection>().phaserRadarShot;
						birdOfPreyPhaserRadarArray[i,combatUnit.serialNumber]= combatUnit.GetComponent<PhaserSection>().phaserRadarArray;
						birdOfPreyXRayKernel[i,combatUnit.serialNumber] = combatUnit.GetComponent<PhaserSection>().xRayKernalUpgrade;
						birdOfPreyTractorBeam[i,combatUnit.serialNumber] = combatUnit.GetComponent<PhaserSection>().tractorBeam;

						birdOfPreyHasRemainingTorpedoAttack[i,combatUnit.serialNumber] = combatUnit.hasRemainingTorpedoAttack;
						birdOfPreyTorpedoSectionIsDestroyed[i,combatUnit.serialNumber] = combatUnit.GetComponent<TorpedoSection>().isDestroyed;
						birdOfPreyTorpedoSectionShieldsCurrent[i,combatUnit.serialNumber] = combatUnit.GetComponent<TorpedoSection>().shieldsCurrent;
						birdOfPreyTorpedoLaserShot[i,combatUnit.serialNumber] = combatUnit.GetComponent<TorpedoSection>().torpedoLaserShot;
						birdOfPreyLaserGuidanceSystem[i,combatUnit.serialNumber] = combatUnit.GetComponent<TorpedoSection>().torpedoLaserGuidanceSystem;
						birdOfPreyHighPressureTubes[i,combatUnit.serialNumber] = combatUnit.GetComponent<TorpedoSection>().highPressureTubes;
						birdOfPreyLightTorpedos[i,combatUnit.serialNumber] = combatUnit.GetComponent<TorpedoSection>().lightTorpedos;
						birdOfPreyHeavyTorpedos[i,combatUnit.serialNumber] = combatUnit.GetComponent<TorpedoSection>().heavyTorpedos;
						birdOfPreyUsedTorpedosThisTurn[i,combatUnit.serialNumber] = combatUnit.GetComponent<TorpedoSection>().usedTorpedosThisTurn;

						birdOfPreyEngineSectionIsDestroyed[i,combatUnit.serialNumber] = combatUnit.GetComponent<EngineSection>().isDestroyed; 
						birdOfPreyEngineSectionShieldsCurrent[i,combatUnit.serialNumber] = combatUnit.GetComponent<EngineSection>().shieldsCurrent;
						birdOfPreyWarpBooster[i,combatUnit.serialNumber] = combatUnit.GetComponent<EngineSection>().warpBooster;
						birdOfPreyTranswarpBooster[i,combatUnit.serialNumber] = combatUnit.GetComponent<EngineSection>().transwarpBooster;
						birdOfPreyWarpDrive[i,combatUnit.serialNumber] = combatUnit.GetComponent<EngineSection>().warpDrive;
						birdOfPreyTranswarpDrive[i,combatUnit.serialNumber] = combatUnit.GetComponent<EngineSection>().transwarpDrive;
						birdOfPreyDistanceMovedThisTurn[i,combatUnit.serialNumber] = combatUnit.GetComponent<EngineSection>().distanceMovedThisTurn;
						birdOfPreyUsedWarpBoosterThisTurn[i,combatUnit.serialNumber] = combatUnit.GetComponent<EngineSection>().usedWarpBoosterThisTurn;
						birdOfPreyUsedTranswarpBoosterThisTurn[i,combatUnit.serialNumber] = combatUnit.GetComponent<EngineSection>().usedTranswarpBoosterThisTurn; 

						break;

					case CombatUnit.UnitType.Destroyer:

						//populate the unit attribute data
						destroyerHexLocation[i,combatUnit.serialNumber] = combatUnit.currentLocation;
						destroyerHasRemainingPhaserAttack[i,combatUnit.serialNumber] = combatUnit.hasRemainingPhaserAttack;
						destroyerPhaserSectionIsDestroyed[i,combatUnit.serialNumber] = combatUnit.GetComponent<PhaserSection>().isDestroyed;
						destroyerPhaserSectionShieldsCurrent[i,combatUnit.serialNumber] = combatUnit.GetComponent<PhaserSection>().shieldsCurrent;
						destroyerUsedPhasersThisTurn[i,combatUnit.serialNumber] = combatUnit.GetComponent<PhaserSection>().usedPhasersThisTurn;
						destroyerPhaserRadarShot[i,combatUnit.serialNumber] = combatUnit.GetComponent<PhaserSection>().phaserRadarShot;
						destroyerPhaserRadarArray[i,combatUnit.serialNumber]= combatUnit.GetComponent<PhaserSection>().phaserRadarArray;
						destroyerXRayKernel[i,combatUnit.serialNumber] = combatUnit.GetComponent<PhaserSection>().xRayKernalUpgrade;
						destroyerTractorBeam[i,combatUnit.serialNumber] = combatUnit.GetComponent<PhaserSection>().tractorBeam;

						destroyerHasRemainingTorpedoAttack[i,combatUnit.serialNumber] = combatUnit.hasRemainingTorpedoAttack;
						destroyerTorpedoSectionIsDestroyed[i,combatUnit.serialNumber] = combatUnit.GetComponent<TorpedoSection>().isDestroyed;
						destroyerTorpedoSectionShieldsCurrent[i,combatUnit.serialNumber] = combatUnit.GetComponent<TorpedoSection>().shieldsCurrent;
						destroyerTorpedoLaserShot[i,combatUnit.serialNumber] = combatUnit.GetComponent<TorpedoSection>().torpedoLaserShot;
						destroyerLaserGuidanceSystem[i,combatUnit.serialNumber] = combatUnit.GetComponent<TorpedoSection>().torpedoLaserGuidanceSystem;
						destroyerHighPressureTubes[i,combatUnit.serialNumber] = combatUnit.GetComponent<TorpedoSection>().highPressureTubes;
						destroyerLightTorpedos[i,combatUnit.serialNumber] = combatUnit.GetComponent<TorpedoSection>().lightTorpedos;
						destroyerHeavyTorpedos[i,combatUnit.serialNumber] = combatUnit.GetComponent<TorpedoSection>().heavyTorpedos;
						destroyerUsedTorpedosThisTurn[i,combatUnit.serialNumber] = combatUnit.GetComponent<TorpedoSection>().usedTorpedosThisTurn;

						destroyerStorageSectionIsDestroyed[i,combatUnit.serialNumber] = combatUnit.GetComponent<StorageSection>().isDestroyed;
						destroyerStorageSectionShieldsCurrent[i,combatUnit.serialNumber] = combatUnit.GetComponent<StorageSection>().shieldsCurrent;
						destroyerDilithiumCrystals[i,combatUnit.serialNumber] = combatUnit.GetComponent<StorageSection>().dilithiumCrystals;
						destroyerTriilithiumCrystals[i,combatUnit.serialNumber] = combatUnit.GetComponent<StorageSection>().trilithiumCrystals;
						destroyerFlares[i,combatUnit.serialNumber] = combatUnit.GetComponent<StorageSection>().flares;
						destroyerRadarJammingSystem[i,combatUnit.serialNumber] = combatUnit.GetComponent<StorageSection>().radarJammingSystem;
						destroyerLaserScatteringSystem[i,combatUnit.serialNumber] = combatUnit.GetComponent<StorageSection>().laserScatteringSystem; 
						destroyerFlareMode[i,combatUnit.serialNumber] = combatUnit.GetComponent<StorageSection>().flareMode;

						destroyerEngineSectionIsDestroyed[i,combatUnit.serialNumber] = combatUnit.GetComponent<EngineSection>().isDestroyed; 
						destroyerEngineSectionShieldsCurrent[i,combatUnit.serialNumber] = combatUnit.GetComponent<EngineSection>().shieldsCurrent;
						destroyerWarpBooster[i,combatUnit.serialNumber] = combatUnit.GetComponent<EngineSection>().warpBooster;
						destroyerTranswarpBooster[i,combatUnit.serialNumber] = combatUnit.GetComponent<EngineSection>().transwarpBooster;
						destroyerWarpDrive[i,combatUnit.serialNumber] = combatUnit.GetComponent<EngineSection>().warpDrive;
						destroyerTranswarpDrive[i,combatUnit.serialNumber] = combatUnit.GetComponent<EngineSection>().transwarpDrive;
						destroyerDistanceMovedThisTurn[i,combatUnit.serialNumber] = combatUnit.GetComponent<EngineSection>().distanceMovedThisTurn;
						destroyerUsedWarpBoosterThisTurn[i,combatUnit.serialNumber] = combatUnit.GetComponent<EngineSection>().usedWarpBoosterThisTurn;
						destroyerUsedTranswarpBoosterThisTurn[i,combatUnit.serialNumber] = combatUnit.GetComponent<EngineSection>().usedTranswarpBoosterThisTurn; 

						break;

					case CombatUnit.UnitType.Starship:

						//populate the unit attribute data
						starshipHexLocation[i,combatUnit.serialNumber] = combatUnit.currentLocation;
						starshipHasRemainingPhaserAttack[i,combatUnit.serialNumber] = combatUnit.hasRemainingPhaserAttack;
						starshipPhaserSectionIsDestroyed[i,combatUnit.serialNumber] = combatUnit.GetComponent<PhaserSection>().isDestroyed;
						starshipPhaserSectionShieldsCurrent[i,combatUnit.serialNumber] = combatUnit.GetComponent<PhaserSection>().shieldsCurrent;
						starshipUsedPhasersThisTurn[i,combatUnit.serialNumber] = combatUnit.GetComponent<PhaserSection>().usedPhasersThisTurn;
						starshipPhaserRadarShot[i,combatUnit.serialNumber] = combatUnit.GetComponent<PhaserSection>().phaserRadarShot;
						starshipPhaserRadarArray[i,combatUnit.serialNumber]= combatUnit.GetComponent<PhaserSection>().phaserRadarArray;
						starshipXRayKernel[i,combatUnit.serialNumber] = combatUnit.GetComponent<PhaserSection>().xRayKernalUpgrade;
						starshipTractorBeam[i,combatUnit.serialNumber] = combatUnit.GetComponent<PhaserSection>().tractorBeam;

						starshipHasRemainingTorpedoAttack[i,combatUnit.serialNumber] = combatUnit.hasRemainingTorpedoAttack;
						starshipTorpedoSectionIsDestroyed[i,combatUnit.serialNumber] = combatUnit.GetComponent<TorpedoSection>().isDestroyed;
						starshipTorpedoSectionShieldsCurrent[i,combatUnit.serialNumber] = combatUnit.GetComponent<TorpedoSection>().shieldsCurrent;
						starshipTorpedoLaserShot[i,combatUnit.serialNumber] = combatUnit.GetComponent<TorpedoSection>().torpedoLaserShot;
						starshipLaserGuidanceSystem[i,combatUnit.serialNumber] = combatUnit.GetComponent<TorpedoSection>().torpedoLaserGuidanceSystem;
						starshipHighPressureTubes[i,combatUnit.serialNumber] = combatUnit.GetComponent<TorpedoSection>().highPressureTubes;
						starshipLightTorpedos[i,combatUnit.serialNumber] = combatUnit.GetComponent<TorpedoSection>().lightTorpedos;
						starshipHeavyTorpedos[i,combatUnit.serialNumber] = combatUnit.GetComponent<TorpedoSection>().heavyTorpedos;
						starshipUsedTorpedosThisTurn[i,combatUnit.serialNumber] = combatUnit.GetComponent<TorpedoSection>().usedTorpedosThisTurn;

						starshipStorageSectionIsDestroyed[i,combatUnit.serialNumber] = combatUnit.GetComponent<StorageSection>().isDestroyed;
						starshipStorageSectionShieldsCurrent[i,combatUnit.serialNumber] = combatUnit.GetComponent<StorageSection>().shieldsCurrent;
						starshipDilithiumCrystals[i,combatUnit.serialNumber] = combatUnit.GetComponent<StorageSection>().dilithiumCrystals;
						starshipTriilithiumCrystals[i,combatUnit.serialNumber] = combatUnit.GetComponent<StorageSection>().trilithiumCrystals;
						starshipFlares[i,combatUnit.serialNumber] = combatUnit.GetComponent<StorageSection>().flares;
						starshipRadarJammingSystem[i,combatUnit.serialNumber] = combatUnit.GetComponent<StorageSection>().radarJammingSystem;
						starshipLaserScatteringSystem[i,combatUnit.serialNumber] = combatUnit.GetComponent<StorageSection>().laserScatteringSystem; 
						starshipFlareMode[i,combatUnit.serialNumber] = combatUnit.GetComponent<StorageSection>().flareMode;

						starshipCrewSectionIsDestroyed[i,combatUnit.serialNumber] = combatUnit.GetComponent<CrewSection>().isDestroyed;
						starshipCrewSectionShieldsCurrent[i,combatUnit.serialNumber] = combatUnit.GetComponent<CrewSection>().shieldsCurrent;
						starshipRepairCrew[i,combatUnit.serialNumber] = combatUnit.GetComponent<CrewSection>().repairCrew;
						starshipShieldEngineeringTeam[i,combatUnit.serialNumber] = combatUnit.GetComponent<CrewSection>().shieldEngineeringTeam;
						starshipBattleCrew[i,combatUnit.serialNumber] = combatUnit.GetComponent<CrewSection>().battleCrew;
						starshipUsedRepairCrewThisTurn[i,combatUnit.serialNumber] = combatUnit.GetComponent<CrewSection>().usedRepairCrewThisTurn;

						starshipEngineSectionIsDestroyed[i,combatUnit.serialNumber] = combatUnit.GetComponent<EngineSection>().isDestroyed; 
						starshipEngineSectionShieldsCurrent[i,combatUnit.serialNumber] = combatUnit.GetComponent<EngineSection>().shieldsCurrent;
						starshipWarpBooster[i,combatUnit.serialNumber] = combatUnit.GetComponent<EngineSection>().warpBooster;
						starshipTranswarpBooster[i,combatUnit.serialNumber] = combatUnit.GetComponent<EngineSection>().transwarpBooster;
						starshipWarpDrive[i,combatUnit.serialNumber] = combatUnit.GetComponent<EngineSection>().warpDrive;
						starshipTranswarpDrive[i,combatUnit.serialNumber] = combatUnit.GetComponent<EngineSection>().transwarpDrive;
						starshipDistanceMovedThisTurn[i,combatUnit.serialNumber] = combatUnit.GetComponent<EngineSection>().distanceMovedThisTurn;
						starshipUsedWarpBoosterThisTurn[i,combatUnit.serialNumber] = combatUnit.GetComponent<EngineSection>().usedWarpBoosterThisTurn;
						starshipUsedTranswarpBoosterThisTurn[i,combatUnit.serialNumber] = combatUnit.GetComponent<EngineSection>().usedTranswarpBoosterThisTurn; 

						break;

					case CombatUnit.UnitType.Starbase:

						//populate the attribute data
						starbaseHexLocation[i] = combatUnit.currentLocation;
						starbaseHasRemainingPhaserAttack[i]  = combatUnit.hasRemainingPhaserAttack;
						starbaseHasRemainingTorpedoAttack[i]  = combatUnit.hasRemainingTorpedoAttack;
						starbaseUnitName[i]  = combatUnit.GetComponent<Starbase>().baseName;
						starbaseHasRemainingRepairAction[i]  = combatUnit.GetComponent<Starbase>().hasRemainingRepairAction;

						starbasePhaserSection1IsDestroyed[i]  = combatUnit.GetComponent<StarbasePhaserSection1>().isDestroyed;
						starbasePhaserSection1ShieldsCurrent[i]  = combatUnit.GetComponent<StarbasePhaserSection1>().shieldsCurrent;
						starbasePhaserSection1UsedPhasersThisTurn[i]  = combatUnit.GetComponent<StarbasePhaserSection1>().usedPhasersThisTurn;
						starbasePhaserRadarShot[i]  = combatUnit.GetComponent<StarbasePhaserSection1>().phaserRadarShot;
						starbasePhaserRadarArray[i] = combatUnit.GetComponent<StarbasePhaserSection1>().phaserRadarArray;

						starbasePhaserSection2IsDestroyed[i]  = combatUnit.GetComponent<StarbasePhaserSection2>().isDestroyed;
						starbasePhaserSection2ShieldsCurrent[i]  = combatUnit.GetComponent<StarbasePhaserSection2>().shieldsCurrent;
						starbasePhaserSection2UsedPhasersThisTurn[i]  = combatUnit.GetComponent<StarbasePhaserSection2>().usedPhasersThisTurn;
						starbaseXRayKernel[i]  = combatUnit.GetComponent<StarbasePhaserSection2>().xRayKernalUpgrade;;

						starbaseTorpedoSectionIsDestroyed[i]  = combatUnit.GetComponent<StarbaseTorpedoSection>().isDestroyed;
						starbaseTorpedoSectionShieldsCurrent[i]  = combatUnit.GetComponent<StarbaseTorpedoSection>().shieldsCurrent;
						starbaseTorpedoLaserShot[i]  = combatUnit.GetComponent<StarbaseTorpedoSection>().torpedoLaserShot;
						starbaseLaserGuidanceSystem[i]  = combatUnit.GetComponent<StarbaseTorpedoSection>().torpedoLaserGuidanceSystem;
						starbaseHighPressureTubes[i]  = combatUnit.GetComponent<StarbaseTorpedoSection>().highPressureTubes;
						starbaseLightTorpedos[i]  = combatUnit.GetComponent<StarbaseTorpedoSection>().lightTorpedos;
						starbaseHeavyTorpedos[i]  = combatUnit.GetComponent<StarbaseTorpedoSection>().heavyTorpedos;
						starbaseUsedTorpedosThisTurn[i]  = combatUnit.GetComponent<StarbaseTorpedoSection>().usedTorpedosThisTurn;

						starbaseCrewSectionIsDestroyed[i]  = combatUnit.GetComponent<StarbaseCrewSection>().isDestroyed;
						starbaseCrewSectionShieldsCurrent[i]  = combatUnit.GetComponent<StarbaseCrewSection>().shieldsCurrent;
						starbaseRepairCrew[i]  = combatUnit.GetComponent<StarbaseCrewSection>().repairCrew;
						starbaseShieldEngineeringTeam[i]  = combatUnit.GetComponent<StarbaseCrewSection>().shieldEngineeringTeam;
						starbaseBattleCrew[i]  = combatUnit.GetComponent<StarbaseCrewSection>().battleCrew;
						starbaseUsedRepairCrewThisTurn[i]  = combatUnit.GetComponent<StarbaseCrewSection>().usedRepairCrewThisTurn;

						starbaseStorageSection1IsDestroyed[i]  = combatUnit.GetComponent<StarbaseStorageSection1>().isDestroyed;
						starbaseStorageSection1ShieldsCurrent[i]  = combatUnit.GetComponent<StarbaseStorageSection1>().shieldsCurrent;
						starbaseDilithiumCrystals[i]  = combatUnit.GetComponent<StarbaseStorageSection1>().dilithiumCrystals;
						starbaseRadarJammingSystem[i]  = combatUnit.GetComponent<StarbaseStorageSection1>().radarJammingSystem;

						starbaseStorageSection2IsDestroyed[i]  = combatUnit.GetComponent<StarbaseStorageSection2>().isDestroyed;
						starbaseStorageSection2ShieldsCurrent[i]  = combatUnit.GetComponent<StarbaseStorageSection2>().shieldsCurrent;
						starbaseTriilithiumCrystals[i]  = combatUnit.GetComponent<StarbaseStorageSection2>().trilithiumCrystals;
						starbaseFlares[i]  = combatUnit.GetComponent<StarbaseStorageSection2>().flares;
						starbaseLaserScatteringSystem[i]  = combatUnit.GetComponent<StarbaseStorageSection2>().laserScatteringSystem;
						starbaseFlareMode[i]  = combatUnit.GetComponent<StarbaseStorageSection2>().flareMode;

						break;

					default:

						Debug.Log("Somehow we have a combat unit without finding the unit type in a collector");

						break;

					}

				}

			}


			//get the colony data
			for(int i = 0; i < fileManager.colonyCollector.transform.childCount; i++){
				
				colonyPlanetName[i] = fileManager.colonyCollector.transform.GetChild(i).GetComponent<Colony>().hexMapTile.GetPlanetString();
				colonyIsColonized[i] = fileManager.colonyCollector.transform.GetChild(i).GetComponent<Colony>().hexMapTile.isColonized;
				colonyColonizingColor[i] = fileManager.colonyCollector.transform.GetChild(i).GetComponent<Colony>().hexMapTile.colonizingColor;
				colonyPlanetHexLocation[i] = fileManager.colonyCollector.transform.GetChild(i).GetComponent<Colony>().hexMapTile.hexLocation;

			}

		}

	}

	SaveGameData saveGameData;

	// Use this for initialization
	public void Init () {

		//manager
		gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
		uiManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
		tileMap = GameObject.FindGameObjectWithTag("TileMap").GetComponent<TileMap>();

		//set actions
		stringSaveGameAction = (fileName) => {SaveGame(fileName);};
		stringDeleteGameAction = (fileName) => {DeleteSaveFile(fileName);};
		stringLoadGameAction = (fileName) => {LoadGame(fileName);};
		playerAutoSaveAction = (player) => {autoSaveGame();};
		saveDataSetCurrentFileAction = (player) => {SetCurrentFile(GameManager.loadedGameData);};

		//get the collectors
		greenCollector = GameObject.Find ("GreenUnits");
		purpleCollector = GameObject.Find ("PurpleUnits");
		redCollector = GameObject.Find ("RedUnits");
		blueCollector = GameObject.Find ("BlueUnits");

		collectors = new GameObject[GameManager.numberPlayers];

		collectors [0] = greenCollector;
		collectors [1] = purpleCollector;
		collectors [2] = redCollector;
		collectors [3] = blueCollector;

		//get the colony collector
		colonyCollector = GameObject.Find ("Colonies");

		//add listener to the new file saved event
		uiManager.GetComponent<FileSaveWindow>().OnFileSaveYesClickedNewFile.AddListener(stringSaveGameAction);

		//add listener to the same file saved event
		uiManager.GetComponent<FileSaveWindow>().OnFileSaveYesClickedSameFile.AddListener(stringSaveGameAction);

		//add listener for the file overwrite event
		uiManager.GetComponent<FileOverwritePrompt>().OnFileOverwriteYesClicked.AddListener(stringSaveGameAction);

		//add a listener to the delete file confirmed event
		uiManager.GetComponent<FileLoadWindow>().OnFileConfirmedDelete.AddListener(stringDeleteGameAction);

		//add listener to the file load event
		uiManager.GetComponent<FileLoadWindow>().OnFileLoadYesClicked.AddListener(stringLoadGameAction);

		//add a listener for the autosave event
		gameManager.OnNewTurn.AddListener(playerAutoSaveAction);

		//add a listener for a load
		gameManager.OnLoadedTurn.AddListener(saveDataSetCurrentFileAction);

		//add listener for loading a local game from the main menu
		gameManager.OnLoadLocalGame.AddListener(stringLoadGameAction);

	}

	//this function resolves saving the game
	private void SaveGame(string fileName){

		//strore the current file
		currentFileName = fileName;

		//create an instance of SaveGameData
		saveGameData = new SaveGameData ();

		//populate saveGameData from the current game state
		saveGameData.PopulateSaveGameDataForSave ();

		//write to the XML serializer using the populated SaveGameData file
		XmlSerializer serializer = new XmlSerializer (typeof(SaveGameData));
		TextWriter writer = new StringWriter ();
		serializer.Serialize (writer, saveGameData);
		writer.Close ();

		//the saveFileName is what was passed to the function
		string saveFileName = fileName;

		//store the filename
		saveGameData.currentFileName = saveFileName;

		//filepath is the full save file
		string filePath = System.IO.Path.Combine(FileSaveBasePath(), saveFileName + ".txt");

		//check if the save directory exists
		if (Directory.Exists (FileSaveBasePath ()) == false) {

			//if it doesn't exist, create it
			Directory.CreateDirectory (FileSaveBasePath ());

		}

		//write the save data to file
		File.WriteAllText(filePath, writer.ToString());

		//invoke the save event
		OnSendSaveGameDataFromSave.Invoke(saveGameData);

	}

	//this function resolves loading the game
	private void LoadGame(string fileName){

		//Debug.Log ("Loaded game flag before load = " + GameManager.loadedGame.ToString());

		//invoke the loadGame event
		OnLoadGame.Invoke();

		SceneManager.LoadScene (SceneManager.GetActiveScene().name);

		string loadFileName = fileName;

		//set the current file to the loaded game
		currentFileName = loadFileName;

		//filepath is the full save file
		string filePath = System.IO.Path.Combine(FileSaveBasePath(), loadFileName + ".txt");

		//create an instance of SaveGameData
		saveGameData = new SaveGameData ();

		//read to the XML serializer using the populated SaveGameData file
		XmlSerializer serializer = new XmlSerializer (typeof(SaveGameData));
		TextReader reader = new StringReader (File.ReadAllText(filePath));
		saveGameData = (SaveGameData)serializer.Deserialize (reader);
		reader.Close ();

		//populate the file name
		saveGameData.currentFileName = loadFileName;

		//invoke the event that sends the saveGameData out for use
		OnSendSaveGameDataFromLoad.Invoke(saveGameData);

		//Debug.Log ("Test after load");

		//Debug.Log ("Loaded game flag after load = " + GameManager.loadedGame.ToString());

	}

	//this is a helper function to return the fileSave base path
	public static string FileSaveBasePath(){
		
		string filePath = System.IO.Path.Combine (Application.persistentDataPath, "Saves");

		return filePath;

	}

	//this function deletes a file from the saves directory
	private void DeleteSaveFile(string fileName){

		string deleteFileName = fileName;

		//filepath is the full save file
		string filePath = System.IO.Path.Combine(FileSaveBasePath(), deleteFileName + ".txt");

		//delete the file
		File.Delete (filePath);

		//invoke the onDelete event
		OnDeleteGame.Invoke();

	}

	//this function resolves autosaving the game
	private void autoSaveGame(){

		//we only want to autosave if the firstTurnHasHappened flag is true
		//this is so that creating a new game doesn't immediately overwrite the LastTurn file
		if (gameManager.firstTurnHasHappened == true) {

			//create an instance of SaveGameData
			saveGameData = new SaveGameData ();

			//populate saveGameData from the current game state
			saveGameData.PopulateSaveGameDataForSave ();

			//write to the XML serializer using the populated SaveGameData file
			XmlSerializer serializer = new XmlSerializer (typeof(SaveGameData));
			TextWriter writer = new StringWriter ();
			serializer.Serialize (writer, saveGameData);
			writer.Close ();

			//the saveFileName is what was passed to the function
			string saveFileName = autoSaveFileName;

			//store the filename
			saveGameData.currentFileName = saveFileName;

			//filepath is the full save file
			string filePath = System.IO.Path.Combine (FileSaveBasePath (), saveFileName + ".txt");

			//check if the save directory exists
			if (Directory.Exists (FileSaveBasePath ()) == false) {

				//if it doesn't exist, create it
				Directory.CreateDirectory (FileSaveBasePath ());

			}

			//write the save data to file
			File.WriteAllText (filePath, writer.ToString ());

			//invoke the onAutosave event
			OnAutosaveGame.Invoke();

		}

	}

	//this function sets the current file after a load
	private void SetCurrentFile(FileManager.SaveGameData saveGameData){

		//set the currentFile from the saveGameData
		currentFileName = saveGameData.currentFileName;

	}


	//function to handle on Destroy
	private void OnDestroy(){

		RemoveAllListeners ();

	}

	//function to remove listeners
	private void RemoveAllListeners(){

		if (uiManager != null) {

			//remove listener to the new file saved event
			uiManager.GetComponent<FileSaveWindow> ().OnFileSaveYesClickedNewFile.RemoveListener (stringSaveGameAction);

			//remove listener to the same file saved event
			uiManager.GetComponent<FileSaveWindow>().OnFileSaveYesClickedSameFile.RemoveListener(stringSaveGameAction);

			//remove listener for the file overwrite event
			uiManager.GetComponent<FileOverwritePrompt>().OnFileOverwriteYesClicked.RemoveListener(stringSaveGameAction);

			//remove a listener to the delete file confirmed event
			uiManager.GetComponent<FileLoadWindow>().OnFileConfirmedDelete.RemoveListener(stringDeleteGameAction);

			//remove listener to the file load event
			uiManager.GetComponent<FileLoadWindow>().OnFileLoadYesClicked.RemoveListener(stringLoadGameAction);

		}

		if (gameManager != null) {

			//remove a listener for the autosave event
			gameManager.OnNewTurn.RemoveListener (playerAutoSaveAction);

			//remove a listener for a load
			gameManager.OnLoadedTurn.RemoveListener(saveDataSetCurrentFileAction);

			//remove listener for loading a local game from the main menu
			gameManager.OnLoadLocalGame.RemoveListener(stringLoadGameAction);

		}

	}

}
