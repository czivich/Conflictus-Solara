using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CombatUnit : MonoBehaviour {

	//this child class will cover all units which take and receive damage - so it is all ships plus home starbases

	//managers
	protected GameManager gameManager;
	protected UIManager uiManager;

	//the currentLocation Hex defines where in the map the object lives
	public Hex currentLocation;

	//declare public event for creating the unit
	//make it static so we don't need to listen to every unit instance
	public static CreateUnitEvent OnCreateUnit = new CreateUnitEvent();

	public static CreateUnitEvent OnUpdateAttackStatus = new CreateUnitEvent();

	//simple class derived from unityEvent to pass CombatUnit Object
	public class CreateUnitEvent : UnityEvent<CombatUnit>{};

	//event announcing a ship created was outfitted
	public static CreateOutfittedShipEvent OnCreatedOutfittedShip = new CreateOutfittedShipEvent();

	//simple class used for outfitted ships
	public class CreateOutfittedShipEvent : UnityEvent<Dictionary<string,int>,CombatUnit>{};

	//class and event for a unit loaded from saveData
	public static CreateLoadedUnitEvent OnCreateLoadedUnit = new CreateLoadedUnitEvent();

	public class CreateLoadedUnitEvent : UnityEvent<CombatUnit, FileManager.SaveGameData>{};


	//set up an enum to keep track of what kind of unit the combat unit is
	public enum UnitType {
		Starship,
		Destroyer,
		BirdOfPrey,
		Scout,
		Starbase,

	}

	//unitType will store what type of unit is being created
	public UnitType unitType {

		get;
		private set;

	}

	//owner is the player that created the unit
	public Player owner {

		get;
		private set;
	
	}

	//variables to keep track of valid attacks
	public bool hasRemainingPhaserAttack {

		get;
		private set;

	}

	public bool hasRemainingTorpedoAttack {

		get;
		private set;

	}

	public string unitName {

		get;
		private set;

	}

	public Dictionary<string,int> outfittedItemsAtPurchase {

		get;
		private set;

	}

	//the serial number is for keeping track of ships being killed
	public int serialNumber {

		get;
		private set;

	}


	//create a vector3 offset so that combat units can exist just above the tilemap
	public static Vector3 mapOffset = new Vector3(0.0f, 0.01f, 0.0f);

	public static void createUnit(CombatUnit prefabUnit, HexMapTile hexMapTile, Player creatingPlayer, string name = "UnitName", Dictionary<string,int> outfittedItems = null){

		//cache data about the prefab and tile passed to the function
		Hex hexLocation = hexMapTile.hexLocation;
		TileMap tileMap = hexMapTile.tileMap;
		Layout layout = tileMap.layout;

		//covert the hex we want to create the unit at to the local coordinate system
		Vector3 unitLocalCoordinatesV3 = Layout.HexToPixelV3 (layout, hexLocation);

		//convert the local coordinates to world coordinates to instantiate the unit at
		Vector3 unitWorldCoordinatesV3 = new Vector3 ((tileMap.transform.localToWorldMatrix * (unitLocalCoordinatesV3)).x + tileMap.transform.position.x,
			(tileMap.transform.localToWorldMatrix * (unitLocalCoordinatesV3)).y + tileMap.transform.position.y,
			(tileMap.transform.localToWorldMatrix * (unitLocalCoordinatesV3)).z + tileMap.transform.position.z);

		CombatUnit newUnit;
		newUnit = Instantiate (prefabUnit, unitWorldCoordinatesV3, Quaternion.identity);

		//position the new unit above the map
		newUnit.transform.position += CombatUnit.mapOffset;

		//set newUnit current location
		newUnit.currentLocation = hexLocation;

		//set the owner
		newUnit.owner = creatingPlayer;

		//set the outfitted equipment

		//check if the outfitted items is null
		if (outfittedItems != null) {

			//invoke the event
			newUnit.outfittedItemsAtPurchase = outfittedItems;

		}

		//set the parent transform based on the owner color (this will keep all of a player's units under a single parent in the hierarchy)
		if (newUnit.owner.color == Player.Color.Blue) {

			newUnit.transform.parent = GameObject.Find("BlueUnits").transform;

		}

		else if(newUnit.owner.color == Player.Color.Green) {

			newUnit.transform.parent = GameObject.Find("GreenUnits").transform;

		}

		else if(newUnit.owner.color == Player.Color.Purple) {

			newUnit.transform.parent = GameObject.Find("PurpleUnits").transform;

		}

		else if(newUnit.owner.color == Player.Color.Red) {

			newUnit.transform.parent = GameObject.Find("RedUnits").transform;

		}

		//test to find out what child class the subunit belongs to
		if(prefabUnit.GetComponent<Starship>()){

			//Debug.Log("It's a starship!");

			//set the unitType
			newUnit.unitType = UnitType.Starship;

			//set the serial number
			newUnit.serialNumber = newUnit.owner.playerStarshipPurchased - 1;


		}
		else if(prefabUnit.GetComponent<Destroyer>()){

			//Debug.Log("It's a destroyer!");

			//set the unitType
			newUnit.unitType = UnitType.Destroyer;

			//set the serial number
			newUnit.serialNumber = newUnit.owner.playerDestroyerPurchased - 1;


		}
		else if(prefabUnit.GetComponent<BirdOfPrey>()){

			//Debug.Log("It's a bird of prey!");

			//set the unitType
			newUnit.unitType = UnitType.BirdOfPrey;

			//set the serial number
			newUnit.serialNumber = newUnit.owner.playerBirdOfPreyPurchased - 1;


		}
		else if(prefabUnit.GetComponent<Scout>()){

			//Debug.Log("It's a scout!");

			//set the unitType
			newUnit.unitType = UnitType.Scout;

			//set the serial number
			newUnit.serialNumber = newUnit.owner.playerScoutPurchased - 1;
			//Debug.Log(newUnit.serialNumber);

		}
		else if(prefabUnit.GetComponent<Starbase>()){

			//Debug.Log("It's a Starbase!");

			//set the unitType
			newUnit.unitType = UnitType.Starbase;

			//set the serial number
			newUnit.serialNumber = 0;
		}
		else {
			Debug.LogError ("Couldn't find unit type!!");
			newUnit = null;
			return;
		}

		//assign the name
		newUnit.unitName = name;

		//scale the unit
		SetUnitScale(newUnit,layout);

		newUnit.Init ();

		//invoke the new unit created event
		OnCreateUnit.Invoke(newUnit);


	}

	public static void createUnitFromLoad(CombatUnit prefabUnit, HexMapTile hexMapTile, Player creatingPlayer, int loadedSerialNumber, FileManager.SaveGameData saveGameData){

		//cache data about the prefab and tile passed to the function
		Hex hexLocation = hexMapTile.hexLocation;
		TileMap tileMap = hexMapTile.tileMap;
		Layout layout = tileMap.layout;

		//covert the hex we want to create the unit at to the local coordinate system
		Vector3 unitLocalCoordinatesV3 = Layout.HexToPixelV3 (layout, hexLocation);

		//convert the local coordinates to world coordinates to instantiate the unit at
		Vector3 unitWorldCoordinatesV3 = new Vector3 ((tileMap.transform.localToWorldMatrix * (unitLocalCoordinatesV3)).x + tileMap.transform.position.x,
			(tileMap.transform.localToWorldMatrix * (unitLocalCoordinatesV3)).y + tileMap.transform.position.y,
			(tileMap.transform.localToWorldMatrix * (unitLocalCoordinatesV3)).z + tileMap.transform.position.z);

		CombatUnit newUnit;
		newUnit = Instantiate (prefabUnit, unitWorldCoordinatesV3, Quaternion.identity);

		//position the new unit above the map
		newUnit.transform.position += CombatUnit.mapOffset;

		//set newUnit current location
		newUnit.currentLocation = hexLocation;

		//set the owner
		newUnit.owner = creatingPlayer;

		//set the parent transform based on the owner color (this will keep all of a player's units under a single parent in the hierarchy)
		if (newUnit.owner.color == Player.Color.Blue) {

			newUnit.transform.parent = GameObject.Find("BlueUnits").transform;

		}

		else if(newUnit.owner.color == Player.Color.Green) {

			newUnit.transform.parent = GameObject.Find("GreenUnits").transform;

		}

		else if(newUnit.owner.color == Player.Color.Purple) {

			newUnit.transform.parent = GameObject.Find("PurpleUnits").transform;

		}

		else if(newUnit.owner.color == Player.Color.Red) {

			newUnit.transform.parent = GameObject.Find("RedUnits").transform;

		}

		//test to find out what child class the subunit belongs to
		if(prefabUnit.GetComponent<Starship>()){

			//Debug.Log("It's a starship!");

			//set the unitType
			newUnit.unitType = UnitType.Starship;

			//set the serial number
			newUnit.serialNumber = loadedSerialNumber;

			//set the name
			//need to find the index of the player colors from the saveGameData file
			for (int i = 0; i < GameManager.numberPlayers; i++) {

				//check if the color matches
				if (saveGameData.playerColor [i] == newUnit.owner.color) {

					//we have found the right index for this player
					//now we can set the name
					newUnit.unitName = saveGameData.playerStarshipNamesPurchased[i,newUnit.serialNumber];

					//set the hasRemaining flags
					newUnit.hasRemainingPhaserAttack = saveGameData.starshipHasRemainingPhaserAttack[i,newUnit.serialNumber];
					newUnit.hasRemainingTorpedoAttack = saveGameData.starshipHasRemainingTorpedoAttack[i,newUnit.serialNumber];

				}

			}

		}
		else if(prefabUnit.GetComponent<Destroyer>()){

			//Debug.Log("It's a destroyer!");

			//set the unitType
			newUnit.unitType = UnitType.Destroyer;

			//set the serial number
			newUnit.serialNumber = loadedSerialNumber;

			//set the name
			//need to find the index of the player colors from the saveGameData file
			for (int i = 0; i < GameManager.numberPlayers; i++) {

				//check if the color matches
				if (saveGameData.playerColor [i] == newUnit.owner.color) {

					//we have found the right index for this player
					//now we can set the name
					newUnit.unitName = saveGameData.playerDestroyerNamesPurchased [i, newUnit.serialNumber];

					//set the hasRemaining flags
					newUnit.hasRemainingPhaserAttack = saveGameData.destroyerHasRemainingPhaserAttack[i,newUnit.serialNumber];
					newUnit.hasRemainingTorpedoAttack = saveGameData.destroyerHasRemainingTorpedoAttack[i,newUnit.serialNumber];

				}

			}

		}
		else if(prefabUnit.GetComponent<BirdOfPrey>()){

			//Debug.Log("It's a bird of prey!");

			//set the unitType
			newUnit.unitType = UnitType.BirdOfPrey;

			//set the serial number
			newUnit.serialNumber = loadedSerialNumber;

			//set the name
			//need to find the index of the player colors from the saveGameData file
			for (int i = 0; i < GameManager.numberPlayers; i++) {

				//check if the color matches
				if (saveGameData.playerColor [i] == newUnit.owner.color) {

					//we have found the right index for this player
					//now we can set the name
					newUnit.unitName = saveGameData.playerBirdOfPreyNamesPurchased [i, newUnit.serialNumber];

					//set the hasRemaining flags
					newUnit.hasRemainingPhaserAttack = saveGameData.birdOfPreyHasRemainingPhaserAttack[i,newUnit.serialNumber];
					newUnit.hasRemainingTorpedoAttack = saveGameData.birdOfPreyHasRemainingTorpedoAttack[i,newUnit.serialNumber];

				}

			}


		}
		else if(prefabUnit.GetComponent<Scout>()){

			//Debug.Log("It's a scout!");

			//set the unitType
			newUnit.unitType = UnitType.Scout;

			//set the serial number
			newUnit.serialNumber = loadedSerialNumber;

			//set the name
			//need to find the index of the player colors from the saveGameData file
			for (int i = 0; i < GameManager.numberPlayers; i++) {

				//check if the color matches
				if (saveGameData.playerColor [i] == newUnit.owner.color) {

					//we have found the right index for this player
					//now we can set the name
					newUnit.unitName = saveGameData.playerScoutNamesPurchased [i, newUnit.serialNumber];

					//set the hasRemaining flags
					newUnit.hasRemainingPhaserAttack = saveGameData.scoutHasRemainingPhaserAttack[i,newUnit.serialNumber];

				}

			}


		}
		else if(prefabUnit.GetComponent<Starbase>()){

			//Debug.Log("It's a Starbase!");

			//set the unitType
			newUnit.unitType = UnitType.Starbase;

			//set the serial number
			newUnit.serialNumber = loadedSerialNumber;

			//set the name
			//need to find the index of the player colors from the saveGameData file
			for (int i = 0; i < GameManager.numberPlayers; i++) {

				//check if the color matches
				if (saveGameData.playerColor [i] == newUnit.owner.color) {

					//we have found the right index for this player
					//now we can set the name
					newUnit.unitName = saveGameData.starbaseUnitName [i];

					//set the hasRemaining flags
					newUnit.hasRemainingPhaserAttack = saveGameData.starbaseHasRemainingPhaserAttack[i];
					newUnit.hasRemainingTorpedoAttack = saveGameData.starbaseHasRemainingTorpedoAttack[i];

				}

			}

		}

		else {
			
			Debug.LogError ("Couldn't find unit type!!");
			newUnit = null;
			return;

		}

		//scale the unit
		SetUnitScale(newUnit,layout);

		newUnit.Init ();

		//invoke the loaded unit created event
		OnCreateLoadedUnit.Invoke(newUnit,saveGameData);

		//run an initial attack status update
		newUnit.UpdateAttackStatus(newUnit);

	}

	//unityActions
	private UnityAction<CombatUnit,CombatUnit,string> attackSectionUpdateAttackStatusAction;
	private UnityAction<Ship> shipUpdateAttackStatusAction;
	private UnityAction<CombatUnit> combatUnitUpdateAttackStatusAction;
	private UnityAction<Dictionary<string,int>,int, CombatUnit> purchaseUpdateAttackStatusAction;
	private UnityAction<CombatUnit> combatUnitCombatUnitDestroyedAction;
	private UnityAction<Player> playerUpdateAttackStatusAction;

	private void Init(){

		//set the actions
		attackSectionUpdateAttackStatusAction = (firingUnit,targetedUnit,sectionTargeted) => {UpdateAttackStatus(firingUnit);};
		shipUpdateAttackStatusAction = (ship) => {UpdateAttackStatus (ship.GetComponent<CombatUnit> ());};
		combatUnitUpdateAttackStatusAction = (combatUnit) => {UpdateAttackStatus (combatUnit);};
		purchaseUpdateAttackStatusAction = (purchasedItems, purchasedValue, combatUnit) => {UpdateAttackStatus (combatUnit);};
		combatUnitCombatUnitDestroyedAction = (combatUnit) => {CombatUnitDestroyed (combatUnit);};
		playerUpdateAttackStatusAction = (player) => {

			if(player == this.owner){
				UpdateAttackStatus(this);
			}

		};

		//get the manager
		gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
		uiManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();


		//add listeners for attack events
		PhaserSection.OnFirePhasers.AddListener(attackSectionUpdateAttackStatusAction);
		PhaserSection.OnEngageTractorBeam.AddListener(shipUpdateAttackStatusAction);
		TorpedoSection.OnFireLightTorpedo.AddListener(attackSectionUpdateAttackStatusAction);
		TorpedoSection.OnFireHeavyTorpedo.AddListener(attackSectionUpdateAttackStatusAction);
		StarbasePhaserSection1.OnFirePhasers.AddListener(attackSectionUpdateAttackStatusAction);
		StarbasePhaserSection2.OnFirePhasers.AddListener(attackSectionUpdateAttackStatusAction);
		StarbaseTorpedoSection.OnFireLightTorpedo.AddListener(attackSectionUpdateAttackStatusAction);
		StarbaseTorpedoSection.OnFireHeavyTorpedo.AddListener(attackSectionUpdateAttackStatusAction);

		//add listeners for sections destroyed/healed
		PhaserSection.OnPhaserSectionDestroyed.AddListener(combatUnitUpdateAttackStatusAction);
		TorpedoSection.OnTorpedoSectionDestroyed.AddListener(combatUnitUpdateAttackStatusAction);
		StarbasePhaserSection1.OnPhaserSection1Destroyed.AddListener(combatUnitUpdateAttackStatusAction);
		StarbasePhaserSection2.OnPhaserSection2Destroyed.AddListener(combatUnitUpdateAttackStatusAction);
		StarbaseTorpedoSection.OnTorpedoSectionDestroyed.AddListener(combatUnitUpdateAttackStatusAction);

		PhaserSection.OnPhaserSectionRepaired.AddListener(combatUnitUpdateAttackStatusAction);
		TorpedoSection.OnTorpedoSectionRepaired.AddListener(combatUnitUpdateAttackStatusAction);
		StarbasePhaserSection1.OnPhaserSection1Repaired.AddListener(combatUnitUpdateAttackStatusAction);
		StarbasePhaserSection2.OnPhaserSection2Repaired.AddListener(combatUnitUpdateAttackStatusAction);
		StarbaseTorpedoSection.OnTorpedoSectionRepaired.AddListener(combatUnitUpdateAttackStatusAction);

		//add listener for purchasing an additional battle crew
		uiManager.GetComponent<PurchaseManager>().OnPurchaseItems.AddListener(purchaseUpdateAttackStatusAction);

		//add listeners for inventory updates
		CrewSection.OnInventoryUpdated.AddListener(combatUnitUpdateAttackStatusAction);

		//add listener for combat unit being destroyed
		Ship.OnShipDestroyed.AddListener(combatUnitCombatUnitDestroyedAction);
		Starbase.OnBaseDestroyed.AddListener(combatUnitCombatUnitDestroyedAction);

		//add listener for starting new turn
		gameManager.OnNewTurn.AddListener(playerUpdateAttackStatusAction);

		//run an initial attack status update
		UpdateAttackStatus(this);

		//call the OnInit function so that subclasses inheriting from CombatUnit (starbase and ship) can run OnInit instead of start
		OnInit ();

	}

	//this is required so that subclasses inheriting from CombatUnit (starbase and ship) can run OnInit instead of start
	protected virtual void OnInit(){



	}

	//this method adjusts the hex cursor size to match the scale of the map tiles
	private static void SetUnitScale(CombatUnit newUnit, Layout layout){
		
		Vector3 unitScale = new Vector3 (layout.size.x * 2.0f, 1.0f, layout.size.y * 2.0f);
		newUnit.transform.localScale = unitScale;

	}

	//function to update attack status
	private void UpdateAttackStatus(CombatUnit combatUnit){
		
		//check if the passed combat unit is this combat unit
		if (combatUnit == this) {

			//check the type of combat unit
			switch (combatUnit.unitType) {

			case CombatUnit.UnitType.Starship:

				//check if the phaser section is alive
				if (combatUnit.GetComponent<PhaserSection> ().isDestroyed == false) {

					//check if phasers have been used already
					if (combatUnit.GetComponent<PhaserSection> ().usedPhasersThisTurn == false) {

						//check if torpedos have been used already
						if (combatUnit.GetComponent<TorpedoSection> ().usedTorpedosThisTurn == false) {

							hasRemainingPhaserAttack = true;

						}
						//the else condition is that we have used torpedos already
						else {

							//check if there is an additional battle crew
							if (combatUnit.GetComponent<CrewSection> ().battleCrew == true) {

								hasRemainingPhaserAttack = true;

							}
							//the else condition is that there is no additional battle crew
							else {

								hasRemainingPhaserAttack = false;

							}

						}

					}
					//the else condition is that we have used phasers already
					else {

						hasRemainingPhaserAttack = false;

					}

				}
				//the else condition is that the phaser section is destroyed
				else {

					hasRemainingPhaserAttack = false;

				}


				//check if the torpedo section is alive
				if (combatUnit.GetComponent<TorpedoSection> ().isDestroyed == false) {

					//check if torpedos have been used already
					if (combatUnit.GetComponent<TorpedoSection> ().usedTorpedosThisTurn == false) {

						//check if phasers have been used already
						if (combatUnit.GetComponent<PhaserSection> ().usedPhasersThisTurn == false) {

							hasRemainingTorpedoAttack = true;

						}
						//the else condition is that we have used phasers already
						else {

							//check if there is an additional battle crew
							if (combatUnit.GetComponent<CrewSection> ().battleCrew == true) {

								hasRemainingTorpedoAttack = true;

							}
							//the else condition is that there is no additional battle crew
							else {

								hasRemainingTorpedoAttack = false;

							}

						}

					}
					//the else condition is that we have used torpedos already
					else {

						hasRemainingTorpedoAttack = false;

					}

				}
				//the else condition is that the torpedo section is destroyed
				else {

					hasRemainingTorpedoAttack = false;

				}

				break;


			case CombatUnit.UnitType.Destroyer:

				//check if the phaser section is alive
				if (combatUnit.GetComponent<PhaserSection> ().isDestroyed == false) {

					//check if phasers have been used already
					if (combatUnit.GetComponent<PhaserSection> ().usedPhasersThisTurn == false) {

						//check if torpedos have been used already
						if (combatUnit.GetComponent<TorpedoSection> ().usedTorpedosThisTurn == false) {

							hasRemainingPhaserAttack = true;

						}
						//the else condition is that we have used torpedos already
						else {

							hasRemainingPhaserAttack = false;

						}

					}
					//the else condition is that we have used phasers already
					else {

						hasRemainingPhaserAttack = false;

					}

				}
				//the else condition is that the phaser section is destroyed
				else {

					hasRemainingPhaserAttack = false;

				}


				//check if the torpedo section is alive
				if (combatUnit.GetComponent<TorpedoSection> ().isDestroyed == false) {

					//check if torpedos have been used already
					if (combatUnit.GetComponent<TorpedoSection> ().usedTorpedosThisTurn == false) {

						//check if phasers have been used already
						if (combatUnit.GetComponent<PhaserSection> ().usedPhasersThisTurn == false) {

							hasRemainingTorpedoAttack = true;

						}
						//the else condition is that we have used phasers already
						else {

							hasRemainingTorpedoAttack = false;

						}

					}
					//the else condition is that we have used torpedos already
					else {

						hasRemainingTorpedoAttack = false;

					}

				}
				//the else condition is that the torpedo section is destroyed
				else {

					hasRemainingTorpedoAttack = false;

				}

				break;

			case CombatUnit.UnitType.BirdOfPrey:

				//check if the phaser section is alive
				if (combatUnit.GetComponent<PhaserSection> ().isDestroyed == false) {

					//check if phasers have been used already
					if (combatUnit.GetComponent<PhaserSection> ().usedPhasersThisTurn == false) {

						//check if torpedos have been used already
						if (combatUnit.GetComponent<TorpedoSection> ().usedTorpedosThisTurn == false) {

							hasRemainingPhaserAttack = true;

						}
						//the else condition is that we have used torpedos already
						else {

							hasRemainingPhaserAttack = false;

						}

					}
					//the else condition is that we have used phasers already
					else {

						hasRemainingPhaserAttack = false;

					}

				}
				//the else condition is that the phaser section is destroyed
				else {

					hasRemainingPhaserAttack = false;

				}


				//check if the torpedo section is alive
				if (combatUnit.GetComponent<TorpedoSection> ().isDestroyed == false) {

					//check if torpedos have been used already
					if (combatUnit.GetComponent<TorpedoSection> ().usedTorpedosThisTurn == false) {

						//check if phasers have been used already
						if (combatUnit.GetComponent<PhaserSection> ().usedPhasersThisTurn == false) {

							hasRemainingTorpedoAttack = true;

						}
						//the else condition is that we have used phasers already
						else {

							hasRemainingTorpedoAttack = false;

						}

					}
					//the else condition is that we have used torpedos already
					else {

						hasRemainingTorpedoAttack = false;

					}

				}
				//the else condition is that the torpedo section is destroyed
				else {

					hasRemainingTorpedoAttack = false;

				}

				break;

			case CombatUnit.UnitType.Scout:

				//check if the phaser section is alive
				if (combatUnit.GetComponent<PhaserSection> ().isDestroyed == false) {

					//check if phasers have been used already
					if (combatUnit.GetComponent<PhaserSection> ().usedPhasersThisTurn == false) {

						hasRemainingPhaserAttack = true;

					}
					//the else condition is that we have used phasers already
					else {

						hasRemainingPhaserAttack = false;

					}

				}
				//the else condition is that the phaser section is destroyed
				else {

					hasRemainingPhaserAttack = false;

				}

				//a scout has no torpedo section, so hasRemainingTorpedoAttack is always false
				hasRemainingTorpedoAttack = false;

				break;


			case CombatUnit.UnitType.Starbase:

				//check if the phaser section is alive
				if (combatUnit.GetComponent<StarbasePhaserSection1> ().isDestroyed == false ||
					combatUnit.GetComponent<StarbasePhaserSection2> ().isDestroyed == false) {

					//check if phasers have been used already
					if (combatUnit.GetComponent<StarbasePhaserSection1> ().usedPhasersThisTurn == false &&
						combatUnit.GetComponent<StarbasePhaserSection2> ().usedPhasersThisTurn == false) {

						//check if torpedos have been used already
						if (combatUnit.GetComponent<StarbaseTorpedoSection> ().usedTorpedosThisTurn == false) {

							hasRemainingPhaserAttack = true;

						}
						//the else condition is that we have used torpedos already
						else {

							//check if there is an additional battle crew
							if (combatUnit.GetComponent<StarbaseCrewSection> ().battleCrew == true) {

								hasRemainingPhaserAttack = true;

							}
							//the else condition is that there is no additional battle crew
							else {

								hasRemainingPhaserAttack = false;

							}

						}

					}
					//the else condition is that we have used phasers already
					else {

						hasRemainingPhaserAttack = false;

					}

				}
				//the else condition is that the phaser section is destroyed
				else {

					hasRemainingPhaserAttack = false;

				}


				//check if the torpedo section is alive
				if (combatUnit.GetComponent<StarbaseTorpedoSection> ().isDestroyed == false) {

					//check if torpedos have been used already
					if (combatUnit.GetComponent<StarbaseTorpedoSection> ().usedTorpedosThisTurn == false) {

						//check if phasers have been used already
						if (combatUnit.GetComponent<StarbasePhaserSection1> ().usedPhasersThisTurn == false &&
							combatUnit.GetComponent<StarbasePhaserSection2> ().usedPhasersThisTurn == false) {

							hasRemainingTorpedoAttack = true;

						}
						//the else condition is that we have used phasers already
						else {

							//check if there is an additional battle crew
							if (combatUnit.GetComponent<StarbaseCrewSection> ().battleCrew == true) {

								hasRemainingTorpedoAttack = true;

							}
							//the else condition is that there is no additional battle crew
							else {

								hasRemainingTorpedoAttack = false;

							}

						}

					}
					//the else condition is that we have used torpedos already
					else {

						hasRemainingTorpedoAttack = false;

					}

				}
				//the else condition is that the torpedo section is destroyed
				else {

					hasRemainingTorpedoAttack = false;

				}

				break;

			}  //close switch case

			//invoke the OnUpdate Event
			OnUpdateAttackStatus.Invoke(this);

		}  //close check to make sure unit is this unit

	}

	//this function will remove listeners when the combat unit is destroyed
	private void CombatUnitDestroyed(CombatUnit combatUnitDestroyed){
		
		//check if the passed combat unit is this combat unit
		if (combatUnitDestroyed == this) {

			RemoveAllListeners ();

		}

	}

	//this function handles onDestroy
	private void OnDestroy(){

		RemoveAllListeners ();

	}

	//this function removes listeners on destroy
	private void RemoveAllListeners(){

		//listeners for attack events
		PhaserSection.OnFirePhasers.RemoveListener(attackSectionUpdateAttackStatusAction);
		PhaserSection.OnEngageTractorBeam.RemoveListener(shipUpdateAttackStatusAction);
		TorpedoSection.OnFireLightTorpedo.RemoveListener(attackSectionUpdateAttackStatusAction);
		TorpedoSection.OnFireHeavyTorpedo.RemoveListener(attackSectionUpdateAttackStatusAction);
		StarbasePhaserSection1.OnFirePhasers.RemoveListener(attackSectionUpdateAttackStatusAction);
		StarbasePhaserSection2.OnFirePhasers.RemoveListener(attackSectionUpdateAttackStatusAction);
		StarbaseTorpedoSection.OnFireLightTorpedo.RemoveListener(attackSectionUpdateAttackStatusAction);
		StarbaseTorpedoSection.OnFireHeavyTorpedo.RemoveListener(attackSectionUpdateAttackStatusAction);

		//remove listeners for sections destroyed/healed
		PhaserSection.OnPhaserSectionDestroyed.RemoveListener(combatUnitUpdateAttackStatusAction);
		TorpedoSection.OnTorpedoSectionDestroyed.RemoveListener(combatUnitUpdateAttackStatusAction);
		StarbasePhaserSection1.OnPhaserSection1Destroyed.RemoveListener(combatUnitUpdateAttackStatusAction);
		StarbasePhaserSection2.OnPhaserSection2Destroyed.RemoveListener(combatUnitUpdateAttackStatusAction);
		StarbaseTorpedoSection.OnTorpedoSectionDestroyed.RemoveListener(combatUnitUpdateAttackStatusAction);

		PhaserSection.OnPhaserSectionRepaired.RemoveListener(combatUnitUpdateAttackStatusAction);
		TorpedoSection.OnTorpedoSectionRepaired.RemoveListener(combatUnitUpdateAttackStatusAction);
		StarbasePhaserSection1.OnPhaserSection1Repaired.RemoveListener(combatUnitUpdateAttackStatusAction);
		StarbasePhaserSection2.OnPhaserSection2Repaired.RemoveListener(combatUnitUpdateAttackStatusAction);
		StarbaseTorpedoSection.OnTorpedoSectionRepaired.RemoveListener(combatUnitUpdateAttackStatusAction);

		if (uiManager != null) {

			//remove listener for purchasing an additional battle crew
			uiManager.GetComponent<PurchaseManager> ().OnPurchaseItems.RemoveListener (purchaseUpdateAttackStatusAction);

		}

		//remove listeners for inventory updates
		CrewSection.OnInventoryUpdated.RemoveListener(combatUnitUpdateAttackStatusAction);

		//remove listener for combat unit being destroyed
		Ship.OnShipDestroyed.RemoveListener(combatUnitCombatUnitDestroyedAction);
		Starbase.OnBaseDestroyed.RemoveListener(combatUnitCombatUnitDestroyedAction);

		if (gameManager != null) {

			//remove listener for starting new turn
			gameManager.OnNewTurn.RemoveListener (playerUpdateAttackStatusAction);

		}

	}

}
