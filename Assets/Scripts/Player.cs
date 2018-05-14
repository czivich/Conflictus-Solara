using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class Player : NetworkBehaviour{

	//the Player class will keep track of the money each player has, and whether the player is still alive in the game

	//managers
	protected GameManager gameManager;
	private ColonyManager colonyManager;
	private UIManager uiManager;

	//this variable keeps track of the player's name
	public string playerName {

		get;
		private set;

	}

	//this variable keeps track of the player's money
	public int playerMoney {

		get;
		private set;

	}

	//this variable keeps track of the player's planet count
	public int playerPlanets {

		get;
		private set;

	}

	//this variable keeps track of the player's scouts
	public int playerScoutPurchased {

		get;
		private set;

	}

	//this variable keeps track of the player's birds
	public int playerBirdOfPreyPurchased {

		get;
		private set;

	}

	//this variable keeps track of the player's destroyers
	public int playerDestroyerPurchased {

		get;
		private set;

	}

	//this variable keeps track of the player's starships
	public int playerStarshipPurchased {

		get;
		private set;

	}

	//this variable keeps track of the player's scouts
	public string[] playerScoutNamesPurchased {

		get;
		private set;

	}

	//this variable keeps track of the player's birds
	public string[] playerBirdOfPreyNamesPurchased {

		get;
		private set;

	}

	//this variable keeps track of the player's destroyers
	public string[] playerDestroyerNamesPurchased {

		get;
		private set;

	}

	//this variable keeps track of the player's starships
	public string[] playerStarshipNamesPurchased {

		get;
		private set;

	}

	//this variable keeps track of the player's scouts
	public bool[] playerScoutPurchasedAlive {

		get;
		private set;

	}

	//this variable keeps track of the player's birds
	public bool[] playerBirdOfPreyPurchasedAlive {

		get;
		private set;

	}

	//this variable keeps track of the player's destroyers
	public bool[] playerDestroyerPurchasedAlive {

		get;
		private set;

	}

	//this variable keeps track of the player's starships
	public bool[] playerStarshipPurchasedAlive {

		get;
		private set;

	}

	//this variable keeps track of the player's starbase
	public bool playerStarbaseAlive {

		get;
		private set;

	}

	//enum to define colors a player can be
	public enum Color{

		Green,
		Purple,
		Red,
		Blue,
	
	}

	//variable to hold what color a player is
	public Color color {

		get;
		private set;

	}

	//this variable keeps track of whether the player is still in the game
	public bool isAlive {

		get;
		private set;

	}


	//create a static variable to control the default starting money
	private static int startingMoney = 0;

	//create a static variable to control the default starting planets
	private static int startingPlanets = 0;

	//value of nondestroyed starbase section
	private static int starbaseSectionValue = 150;

	//value of occupied planet
	public static readonly int planetValue = 100;

	//variable to hold the home starbase
	private Starbase homeStarbase;

	//event for player income update
	public static IncomeEvent OnCollectIncome = new IncomeEvent();

	public static IncomeEvent OnPlayerMoneyChange = new IncomeEvent();

	//class derived from unityEvent to pass a player object
	public class IncomeEvent : UnityEvent<Player,int>{};

	//constructor for player class
	public static Player createPlayer(Player prefabPlayer, Color newPlayerColor, string newPlayerName){

		//create a new player instance
		Player newPlayer = Instantiate(prefabPlayer, Vector3.zero, Quaternion.identity);

        //assign default values
        newPlayer.color = newPlayerColor;
		newPlayer.isAlive = true;
		newPlayer.playerMoney = startingMoney;
		newPlayer.playerPlanets = startingPlanets;
		newPlayer.playerStarbaseAlive = true;

		//sort the new player in the hierarchy
		newPlayer.transform.parent =  GameObject.Find("Players").transform;

		//name the new player
		newPlayer.playerName = newPlayerName;

		//name the game object based on the color
		switch (newPlayerColor){

		case Color.Green:

			newPlayer.name = "greenPlayer";
			break;

		case Color.Purple:

			newPlayer.name = "purplePlayer";
			break;

		case Color.Red:

			newPlayer.name = "redPlayer";
			break;

		case Color.Blue:

			newPlayer.name = "bluePlayer";
			break;

		default:

			break;

		}

		//call the start function
		newPlayer.Init();


		return newPlayer;

	}

	//unityActions
	private UnityAction<CombatUnit> combatUnitFindHomeStarbaseAction;
	private UnityAction<CombatUnit,FileManager.SaveGameData> combatUnitLoadedFindHomeStarbaseAction;
	private UnityAction<Player> playerAddPlayerIncomeAction;
	private UnityAction<string,Player> playerUpdatePlanetCountAction;
	private UnityAction<Dictionary<string,int>,int,CombatUnit> purchaseResolvePurchaseItemsAction;
	private UnityAction<NameNewShip.NewUnitEventData> newUnitResolvePurchaseShip;
	private UnityAction<CombatUnit> combatUnitResolveShipDestroyedAction;
	private UnityAction<CombatUnit,string,GameManager.ActionMode> combatUnitResolveRenameUnitAction;
	private UnityAction<FileManager.SaveGameData> loadPlayerAttributesAction;
	private UnityAction<Player> killPlayerAction;



	private void Init(){

		//get the manager
		gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
		colonyManager = GameObject.FindGameObjectWithTag("ColonyManager").GetComponent<ColonyManager>();
		uiManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();


		//Debug.Log ("Current turn player is " + gameManager.currentTurnPlayer.ToString());

		//initialize string array sizes
		playerScoutNamesPurchased = new string[GameManager.maxShipsPerClass];
		playerBirdOfPreyNamesPurchased = new string[GameManager.maxShipsPerClass];
		playerDestroyerNamesPurchased = new string[GameManager.maxShipsPerClass];
		playerStarshipNamesPurchased = new string[GameManager.maxShipsPerClass];

		playerScoutPurchasedAlive = new bool[GameManager.maxShipsPerClass];
		playerBirdOfPreyPurchasedAlive = new bool[GameManager.maxShipsPerClass];
		playerDestroyerPurchasedAlive = new bool[GameManager.maxShipsPerClass];
		playerStarshipPurchasedAlive = new bool[GameManager.maxShipsPerClass];

		for (int i = 0; i < GameManager.maxShipsPerClass; i++) {

			playerScoutPurchasedAlive [i] = true;
			playerBirdOfPreyPurchasedAlive [i] = true;
			playerDestroyerPurchasedAlive [i] = true;
			playerStarshipPurchasedAlive [i] = true;

		}

		//set the actions
		combatUnitFindHomeStarbaseAction = (newUnit) => {FindHomeStarbase(newUnit);};
		combatUnitLoadedFindHomeStarbaseAction = (newUnit,saveGameData) => {FindHomeStarbase(newUnit);};
		playerAddPlayerIncomeAction = (currentTurnPlayer) => {

			if(currentTurnPlayer == this){
				AddPlayerIncome(currentTurnPlayer);
			}

		};

		playerUpdatePlanetCountAction = (planet,player) => {UpdatePlayerPlanetCount(player);};
		purchaseResolvePurchaseItemsAction = (purchasedItems,purchasedValue,combatUnit) => {ResolvePurchaseItems(purchasedValue,combatUnit.owner);};
		newUnitResolvePurchaseShip = (newUnitData) => {ResolvePurchaseShip(newUnitData);};
		combatUnitResolveShipDestroyedAction = (combatUnit) => {ResolveShipDestroyed(combatUnit);};
		combatUnitResolveRenameUnitAction = (combatUnit, newName, previousActionMode) => {ResolveRenamedUnit( combatUnit, newName);};
		loadPlayerAttributesAction = (saveGameData) => {LoadPlayerAttributes(saveGameData);};
		killPlayerAction = (player) => {killPlayerAction(player);};


		//add listener for creating a new unit
		CombatUnit.OnCreateUnit.AddListener(combatUnitFindHomeStarbaseAction);
		CombatUnit.OnCreateLoadedUnit.AddListener(combatUnitLoadedFindHomeStarbaseAction);

		//add listeners for starting a new turn
		//call UpdatePlayerMoney only if the new turn player is this player
		gameManager.OnNewTurn.AddListener(playerAddPlayerIncomeAction);

		//add listener for planet owner changing
		ColonyManager.OnPlanetOwnerChanged.AddListener(playerUpdatePlanetCountAction);

		//add listener for purchasing items
		uiManager.GetComponent<PurchaseManager>().OnPurchaseItems.AddListener(purchaseResolvePurchaseItemsAction);

		//add listener for purchasing a ship
		uiManager.GetComponent<NameNewShip>().OnPurchasedNewShip.AddListener(newUnitResolvePurchaseShip);

		//add listener for ship dying
		Ship.OnShipDestroyed.AddListener(combatUnitResolveShipDestroyedAction);

		//add listener for starbase dying
		Starbase.OnBaseDestroyed.AddListener(combatUnitResolveShipDestroyedAction);

		//add listener for ship renaming
		uiManager.GetComponent<RenameShip>().OnRenameUnit.AddListener(combatUnitResolveRenameUnitAction);

		//add listener for loading players
		gameManager.OnPlayersLoaded.AddListener(loadPlayerAttributesAction);

		//add listener for killing player
		gameManager.OnKillPlayer.AddListener(killPlayerAction);

	}

	//this function finds the player home starbase
	private void FindHomeStarbase(CombatUnit newUnit){

		//only do something if the new unit is a starbase and it's color matches this player
		if (newUnit.GetComponent<Starbase> () == true && newUnit.owner == this) {

			//get the home starbase - need to determine which color we are first
			switch (this.color) {

			case Player.Color.Green: 

			//check if the starbase object still exists
				if (GameObject.Find ("GreenUnits").gameObject.GetComponentInChildren<Starbase> () == true) {

					homeStarbase = GameObject.Find ("GreenUnits").gameObject.GetComponentInChildren<Starbase> ();

				} else {

					homeStarbase = null;

				}

				break;

			case Player.Color.Red: 

			//check if the starbase object still exists
				if (GameObject.Find ("RedUnits").gameObject.GetComponentInChildren<Starbase> () == true) {

					homeStarbase = GameObject.Find ("RedUnits").gameObject.GetComponentInChildren<Starbase> ();

				} else {

					homeStarbase = null;

				}

				break;

			case Player.Color.Blue: 

			//check if the starbase object still exists
				if (GameObject.Find ("BlueUnits").gameObject.GetComponentInChildren<Starbase> () == true) {

					homeStarbase = GameObject.Find ("BlueUnits").gameObject.GetComponentInChildren<Starbase> ();

				} else {

					homeStarbase = null;

				}

				break;

			case Player.Color.Purple: 

			//check if the starbase object still exists
				if (GameObject.Find ("PurpleUnits").gameObject.GetComponentInChildren<Starbase> () == true) {

					homeStarbase = GameObject.Find ("PurpleUnits").gameObject.GetComponentInChildren<Starbase> ();

				} else {

					homeStarbase = null;

				}

				break;

			default:

				homeStarbase = null;
				break;

			}

		}

	}

	//this function will add the player's start of turn income to his balance
	private void AddPlayerIncome(Player currentTurnPlayer){

		//Debug.Log ("Income");

		//temporary variable to hold the turn income
		int turnIncome = 0;

		//check to make sure the player passed to the function is the current player
		if (currentTurnPlayer == this) {

			//get starbase income
			turnIncome += GetPlayerStarbaseIncome();

			//add income for the number of planets
			turnIncome += playerPlanets * planetValue;

			//add the turn income to the player money
			playerMoney += turnIncome;

			//invoke the income event
			OnCollectIncome.Invoke(currentTurnPlayer,turnIncome);

		}

	}

	//this function will update the player planet count
	private void UpdatePlayerPlanetCount(Player player){

		//not requiring player passed to function to match this because I want to update all players when
		//a planet owner changes

		//update the planet count
		this.playerPlanets = colonyManager.PlanetsControlledByPlayer(this);


	}

	//this function gets the planet value
	public static int GetPlanetValue(){

		return planetValue;

	}

	//this function gets the starbase value
	public static int GetStarbaseSectionValue(){

		return starbaseSectionValue;

	}

	//this function gets the player starbase income
	public int GetPlayerStarbaseIncome(){

		int starbaseIncome = 0;

		//check if the home starbase is not null
		if (this.homeStarbase != null) {

			//check if the individual sections are alive
			if (this.homeStarbase.GetComponent<StarbasePhaserSection1> ().isDestroyed == false) {

				//increment the turn income
				starbaseIncome += starbaseSectionValue;

			}

			//check if the individual sections are alive
			if (this.homeStarbase.GetComponent<StarbasePhaserSection2> ().isDestroyed == false) {

				//increment the turn income
				starbaseIncome += starbaseSectionValue;

			}

			//check if the individual sections are alive
			if (this.homeStarbase.GetComponent<StarbaseTorpedoSection> ().isDestroyed == false) {

				//increment the turn income
				starbaseIncome += starbaseSectionValue;

			}

			//check if the individual sections are alive
			if (this.homeStarbase.GetComponent<StarbaseStorageSection1> ().isDestroyed == false) {

				//increment the turn income
				starbaseIncome += starbaseSectionValue;

			}

			//check if the individual sections are alive
			if (this.homeStarbase.GetComponent<StarbaseStorageSection2> ().isDestroyed == false) {

				//increment the turn income
				starbaseIncome += starbaseSectionValue;

			}

			//check if the individual sections are alive
			if (this.homeStarbase.GetComponent<StarbaseCrewSection> ().isDestroyed == false) {

				//increment the turn income
				starbaseIncome += starbaseSectionValue;

			}

		}

		return starbaseIncome;

	}

	//this function handles reducing money after items are purchased
	private void ResolvePurchaseItems(int purchasedValue, Player player){

		//check if the player matches
		if (this == player) {

			//decrement the money by the purchased value
			this.playerMoney -= purchasedValue;

			OnPlayerMoneyChange.Invoke (this, purchasedValue);

			//check if money has gone negative
			if (this.playerMoney < 0) {

				Debug.LogError ("Somehow money went negative!");

			}

		}

	}

	//this function handles purchasing a new ship
	private void ResolvePurchaseShip(NameNewShip.NewUnitEventData newUnitData){

		//check if the player matches
		if (this == gameManager.currentTurnPlayer) {

			//decrement the money by the purchased value
			this.playerMoney -= newUnitData.newUnitTotalPrice;

			OnPlayerMoneyChange.Invoke (this, newUnitData.newUnitTotalPrice);

			//check if money has gone negative
			if (this.playerMoney < 0) {

				Debug.LogError ("Somehow money went negative!");

			}

			//increment the ship purchase count
			//switch case based on ship type
			switch (newUnitData.newUnitType) {

			case CombatUnit.UnitType.Scout:

				//the string array index gets populated first, since the index value is the count prior to increment
				this.playerScoutNamesPurchased [playerScoutPurchased] = newUnitData.newUnitName;

				this.playerScoutPurchased += 1;


				break;

			case CombatUnit.UnitType.BirdOfPrey:

				//the string array index gets populated first, since the index value is the count prior to increment
				this.playerBirdOfPreyNamesPurchased [playerBirdOfPreyPurchased] = newUnitData.newUnitName;

				this.playerBirdOfPreyPurchased += 1;

				break;

			case CombatUnit.UnitType.Destroyer:
				
				//the string array index gets populated first, since the index value is the count prior to increment
				this.playerDestroyerNamesPurchased [playerDestroyerPurchased] = newUnitData.newUnitName;

				this.playerDestroyerPurchased += 1;

				break;

			case CombatUnit.UnitType.Starship:

				//the string array index gets populated first, since the index value is the count prior to increment
				this.playerStarshipNamesPurchased [playerStarshipPurchased] = newUnitData.newUnitName;

				this.playerStarshipPurchased += 1;

				break;

			default:

				break;


			}

		}

	}

	//this function resolves a ship getting destroyed
	private void ResolveShipDestroyed(CombatUnit combatUnitDestroyed){

		//check if this player is the owner of the destroyed ship
		if (this == combatUnitDestroyed.owner) {

			//if this is the owner, use the unit serial number to mark the unit as dead

			switch (combatUnitDestroyed.unitType) {

			case CombatUnit.UnitType.Scout:

				this.playerScoutPurchasedAlive [combatUnitDestroyed.serialNumber] = false;
				break;

			case CombatUnit.UnitType.BirdOfPrey:

				this.playerBirdOfPreyPurchasedAlive [combatUnitDestroyed.serialNumber] = false;
				break;

			case CombatUnit.UnitType.Destroyer:

				this.playerDestroyerPurchasedAlive [combatUnitDestroyed.serialNumber] = false;
				break;

			case CombatUnit.UnitType.Starship:

				this.playerStarshipPurchasedAlive [combatUnitDestroyed.serialNumber] = false;
				break;

			case CombatUnit.UnitType.Starbase:

				this.playerStarbaseAlive = false;
				break;

			default:

				break;

			}

		}

	}

	//this function will handle a ship being renamed for the name logs
	private void ResolveRenamedUnit(CombatUnit combatUnit, string newName){

		//check if the unit owner matches this player
		if (combatUnit.owner == this) {

			//switch case based on the unit type
			switch (combatUnit.unitType) {

			case CombatUnit.UnitType.Scout:

				//update the name based on the serial number
				this.playerScoutNamesPurchased [combatUnit.serialNumber] = newName;

				break;

			case CombatUnit.UnitType.BirdOfPrey:

				//update the name based on the serial number
				this.playerBirdOfPreyNamesPurchased [combatUnit.serialNumber] = newName;

				break;

			case CombatUnit.UnitType.Destroyer:

				//update the name based on the serial number
				this.playerDestroyerNamesPurchased [combatUnit.serialNumber] = newName;

				break;

			case CombatUnit.UnitType.Starship:

				//update the name based on the serial number
				this.playerStarshipNamesPurchased [combatUnit.serialNumber] = newName;

				break;

			default:

				break;

			}

		}

	}

	//this function populates player attributes from the load game file
	private void LoadPlayerAttributes(FileManager.SaveGameData saveGameData){

		//loop through the saveGameData players to find the matching color
		for (int i = 0; i < GameManager.numberPlayers; i++) {

			//check if the color matches this player color
			if (this.color == saveGameData.playerColor [i]) {

				//now that we have the right index for the player, set the variables from the save file
				this.playerName = saveGameData.playerName[i];
				this.playerMoney = saveGameData.playerMoney[i];
				this.playerPlanets = saveGameData.playerPlanets[i];
				this.isAlive = saveGameData.playerIsAlive [i];

				this.playerScoutPurchased = saveGameData.playerScoutPurchased [i];

				//loop through the number of units purchased
				for (int j = 0; j < this.playerScoutPurchased; j++) {

					this.playerScoutNamesPurchased [j] = saveGameData.playerScoutNamesPurchased [i, j];
					this.playerScoutPurchasedAlive [j] = saveGameData.playerScoutPurchasedAlive [i, j];

				}

				this.playerBirdOfPreyPurchased = saveGameData.playerBirdOfPreyPurchased [i];

				//loop through the number of units purchased
				for (int j = 0; j < this.playerBirdOfPreyPurchased; j++) {

					this.playerBirdOfPreyNamesPurchased [j] = saveGameData.playerBirdOfPreyNamesPurchased [i, j];
					this.playerBirdOfPreyPurchasedAlive [j] = saveGameData.playerBirdOfPreyPurchasedAlive [i, j];

				}

				this.playerDestroyerPurchased = saveGameData.playerDestroyerPurchased [i];

				//loop through the number of units purchased
				for (int j = 0; j < this.playerDestroyerPurchased; j++) {

					this.playerDestroyerNamesPurchased [j] = saveGameData.playerDestroyerNamesPurchased [i, j];
					this.playerDestroyerPurchasedAlive [j] = saveGameData.playerDestroyerPurchasedAlive [i, j];

				}

				this.playerStarshipPurchased = saveGameData.playerStarshipPurchased [i];

				//loop through the number of units purchased
				for (int j = 0; j < this.playerStarshipPurchased; j++) {

					this.playerStarshipNamesPurchased [j] = saveGameData.playerStarshipNamesPurchased [i, j];
					this.playerStarshipPurchasedAlive [j] = saveGameData.playerStarshipPurchasedAlive [i, j];

				}

				this.playerStarbaseAlive = saveGameData.playerStarbaseIsAlive [i];

			}

		}

	}

	//this function kills the player
	private void KillPlayer(Player player){

		//check if the player passed is this player
		if (player == this) {

			//set the isAlive flag to false
			this.isAlive = false;

		}

	}

	//function to handle OnDestroy
	private void OnDestroy(){

		RemoveAllListeners ();

	}

	//this function removes all listeners
	private void RemoveAllListeners(){

		//remove listener for creating a new unit
		CombatUnit.OnCreateUnit.RemoveListener(combatUnitFindHomeStarbaseAction);
		CombatUnit.OnCreateLoadedUnit.RemoveListener(combatUnitLoadedFindHomeStarbaseAction);

		if (gameManager != null) {
			
			//remove listeners for starting a new turn
			//call UpdatePlayerMoney only if the new turn player is this player
			gameManager.OnNewTurn.RemoveListener (playerAddPlayerIncomeAction);

			//remove listener for loading players
			gameManager.OnPlayersLoaded.RemoveListener(loadPlayerAttributesAction);

			//remove listener for killing player
			gameManager.OnKillPlayer.RemoveListener(killPlayerAction);

		}

		//remove listener for planet owner changing
		ColonyManager.OnPlanetOwnerChanged.RemoveListener(playerUpdatePlanetCountAction);

		if (uiManager != null) {
			
			//remove listener for purchasing items
			uiManager.GetComponent<PurchaseManager> ().OnPurchaseItems.RemoveListener (purchaseResolvePurchaseItemsAction);

			//remove listener for purchasing a ship
			uiManager.GetComponent<NameNewShip> ().OnPurchasedNewShip.RemoveListener (newUnitResolvePurchaseShip);

			//remove listener for ship renaming
			uiManager.GetComponent<RenameShip>().OnRenameUnit.RemoveListener(combatUnitResolveRenameUnitAction);

		}

		//remove listener for ship dying
		Ship.OnShipDestroyed.RemoveListener(combatUnitResolveShipDestroyedAction);

		//remove listener for starbase dying
		Starbase.OnBaseDestroyed.RemoveListener(combatUnitResolveShipDestroyedAction);

	}

}
