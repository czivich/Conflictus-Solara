using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EngineSection : MonoBehaviour {

	//variables for the managers
	private UIManager uiManager;
	private GameManager gameManager;
	private MouseManager mouseManager;

	//get the tilemap that the ship exists on
	private TileMap tileMap;

	//define the variables that the engine section must manage
	public int warpBooster {

		get;
		private set;

	}

	public int transwarpBooster {

		get;
		private set;

	}

	public bool warpDrive {

		get;
		private set;

	}

	public bool transwarpDrive {

		get;
		private set;

	}

	//set defaults for movement range increases
	private int transwarpMovementRange = 7;
	private int warpMovementRange = 5;

	//variable bools to track whether boosters have been used this turn or not
	public bool usedWarpBoosterThisTurn {

		get;
		private set;

	}
	public bool usedTranswarpBoosterThisTurn {

		get;
		private set;

	}

	//bool to track whether the section is destroyed or not
	public bool isDestroyed {

		get;
		private set;

	}

	//variable to hold the shields value of the section
	public int shieldsMax {

		get;
		private set;

	}

	public int shieldsCurrent {

		get;
		private set;

	}

	//variable to hold the ship that the engine section is attached to
	private Ship thisShip;


	//create a default maxMovementRange for ships
	private static int defaultMaxMovementRange = 3;

	//define variable for max movement range;
	private int maxMovementRange = defaultMaxMovementRange;

	//define property for current movement range
	private int currentMovementRange;
	public int CurrentMovementRange {

		get{

			return currentMovementRange;

		}

		set{

			//check if the value is different than what it is already stored as
			if (currentMovementRange == value) {

				//if so, return without doing anything
				return;

			}

			//if it is different, set the value
			currentMovementRange = value;

			//if the value is changing, set the value, then invoke an event that the value changed
			OnMovementRangeChange.Invoke (currentMovementRange);

		}

	}
		
	//define a variable for movement made this turn
	public int distanceMovedThisTurn {

		get;
		private set;

	}

	//define a variable for movement made in the current movement
	private int distanceMovedThisMove;

	//this list is hexes that are within the ship's movement range
	public List<Hex> ReachableHexes {
		get;
		private set;
	}

	//create a bool to track whether the ship is currently moving
	public bool isMoving {
		get;
		private set;
	}

	//create a list of hexes that defines a ship's movement path
	public List<Hex> currentMovementPath {

		get;
		private set;

	}

	//private variables for renderer and materials
	private Renderer shipRenderer;
	private Material originalMaterial;
	private Material fadeMaterial;
	private Color fadeColor;

	//define a time for the fade to take
	private float timeToFade = 1.0f;
	private float fadeCounter = 0.0f;

	//boolean to keep track of whether ship is warping through a wormhole
	private bool isWarping = false;

	//bool to keep track of whether the ship has updated this frame already
	private bool hasUpdatedThisFrame = false;

	//this enum will keep track of where we are in the warp cycle
	private enum WarpStatus {
		fadingOut,
		fadingIn,
	}

	//these bools will keep track of whether we are towing a unit or are being towed by another unit
	//make them accessible properties so messageManager can tell if there is movement via tractor beam
	public bool isTowing {

		get;
		private set;

	}

	public bool isBeingTowed {

		get;
		private set;

	}

	//we will need the ship to be able to track the ship it is either towing or being towed by
	private Ship isTowingShip;
	private Ship isBeingTowedByShip;

	//this variable keeds track of if the ship is traveling through a wormhole
	private WarpStatus warpStatus;

	//need a pair of variables to store starting and ending hexes for sending a movement message to the log
	private Hex movementStartingHex;
	private Hex movementEndingHex;

	//declare public events for movement
	//make it static so we don't need to listen to every unit instance
	public static MoveEvent OnMoveStart = new MoveEvent();
	public static MoveEvent OnMoveFinish = new MoveEvent();
	public static MoveEvent OnMoveTargetedShip = new MoveEvent();
	public static MoveEvent OnMoveSelectedShip = new MoveEvent();
	public static MoveEvent OnRefreshRange = new MoveEvent ();
	public static MoveEvent OnMoveWhileTowing = new MoveEvent();

	//simple class derived from unityEvent to pass Ship Object
	public class MoveEvent : UnityEvent<Ship>{};


	//this event specifically includes the starting and ending hexes
	public static MoveFromToEvent OnMoveFromToFinish = new MoveFromToEvent ();

	//simple class derived from unityEvent to pass Ship Object
	public class MoveFromToEvent : UnityEvent<Ship,Hex,Hex>{};


	//this event is for when the CurrentMovementRange property changes
	public MovementRangeEvent OnMovementRangeChange = new MovementRangeEvent();

	//simple class derived from unityEvent to pass int variable
	public class MovementRangeEvent : UnityEvent<int>{};


	//events to handle booster usage
	//make it static so that subscribers don't have to subscribe to every ship
	public static BoosterEvent OnUseWarpBooster = new BoosterEvent();
	public static BoosterEvent OnUseTranswarpBooster = new BoosterEvent();

	//simple class so I can have my event pass the ship parameter in the event
	public class BoosterEvent : UnityEvent<Ship>{};


	//event to announce the section was destroyed
	public static EngineSectionDestroyedEvent OnEngineSectionDestroyed = new EngineSectionDestroyedEvent();

	//event to announce the section was repaired
	public static EngineSectionDestroyedEvent OnEngineSectionRepaired = new EngineSectionDestroyedEvent();

	//class for event to announce section was destroyed
	public class EngineSectionDestroyedEvent : UnityEvent<CombatUnit>{};

	//event to announce damage was taken
	public static UnityEvent OnEngineDamageTaken = new UnityEvent();

	//event to announce start of warp fade out
	public static MoveEvent OnStartWarpFadeOut = new MoveEvent();

	//event to announce restart of normal movement after fading in
	public static MoveEvent OnResumeMovementAfterFadeIn = new MoveEvent();

	//event to announce waiting after a warp for the unit being towed to come through
	public static MoveEvent OnWaitForTowedUnitAfterWarping = new MoveEvent();

	//event to announce inventory updated
	public static InventoryUpdatedEvent OnInventoryUpdated = new InventoryUpdatedEvent();
	public class InventoryUpdatedEvent : UnityEvent<CombatUnit>{};

	//UnityActions
	private UnityAction<Ship> shipShowTranswarpBoosterAction;
	private UnityAction<Ship> shipShowWarpBoosterAction;
	private UnityAction<Ship> shipRestoreMovementRangeAction;
	private UnityAction<Player.Color> playerColorEndTurnAction;
	private UnityAction setSelectedUnitEarlyCalculateMovementRangeAction;
	private UnityAction<Ship,Hex,Ship> shipMovementMoveShipAction;
	private UnityAction<CombatUnit,CombatUnit,int> attackHitTakeDamageAction;
	private UnityAction<CombatUnit,CombatUnit,CombatManager.CrystalType,int> crystalUsedHealDamageAction;
	private UnityAction<CombatUnit,CombatUnit> repairUsedRepairSectionAction;
	private UnityAction<CombatUnit> combatUnitCombatUnitDestroyedAction;
	private UnityAction<CombatUnit> combatUnitCalculateMovementRangeSelectedUnitAction;
	private UnityAction<Dictionary<string,int>,int,CombatUnit> purchaseAddPurchaseItemsAction;
	private UnityAction<CombatUnit,int> incidentalTakeDamageAction;
	private UnityAction<CombatUnit,FileManager.SaveGameData> saveDataResolveLoadedUnitAction;
	private UnityAction clearSelectedUnitCalculateMovementRangeAction;

	public void Init(){

		//set the actions
		shipShowTranswarpBoosterAction = (ship) => {ShowTranswarpBooster(ship);};
		shipShowWarpBoosterAction = (ship) => {ShowWarpBooster(ship);};
		shipRestoreMovementRangeAction = (ship) => {RestoreMovementRange(ship);};
		playerColorEndTurnAction = (color) => {EndTurn(color);};
		setSelectedUnitEarlyCalculateMovementRangeAction = () => {

			//restore the range - I can do this because an engine section component will only be on a ship
			RestoreMovementRange(mouseManager.selectedUnit.GetComponent<Ship>());

			CalculateMovementRange(mouseManager.selectedUnit);
		
		};
		shipMovementMoveShipAction = (selectedShip,destinationHex,targetedShip) => {MoveShip(selectedShip,destinationHex,targetedShip,null);};
		attackHitTakeDamageAction = (attackingUnit, targetedUnit, phasorDamage) => {TakeDamage(targetedUnit,phasorDamage);};
		crystalUsedHealDamageAction = (selectedUnit,targetedUnit,crystalType,shieldsHealed) => {HealDamage(targetedUnit,shieldsHealed);};
		repairUsedRepairSectionAction = (selectedUnit,targetedUnit) => {RepairSection(targetedUnit);};
		combatUnitCombatUnitDestroyedAction = (combatUnit) => {CombatUnitDestroyed(combatUnit);};
		combatUnitCalculateMovementRangeSelectedUnitAction = (combatUnit) => {

			//check to make sure the selected unit is not null
			if(mouseManager.selectedUnit != null){

				CalculateMovementRange(mouseManager.selectedUnit);

			}

		};

		purchaseAddPurchaseItemsAction = (purchasedItems,purchasedValue,combatUnit) => {AddPurchasedItems(purchasedItems,combatUnit);};
		incidentalTakeDamageAction = (combatUnit, damage) => {TakeDamage(combatUnit,damage);};
		saveDataResolveLoadedUnitAction = (combatUnit,saveGameData) => {ResolveLoadedUnit(combatUnit,saveGameData);};
		clearSelectedUnitCalculateMovementRangeAction = () => {RestoreMovementRange(this.GetComponent<Ship>());};


		//cache the ship that the engine section is attached to
		thisShip = this.GetComponentInParent<Ship>();

		//get the managers
		uiManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
		gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
		mouseManager = GameObject.FindGameObjectWithTag("MouseManager").GetComponent<MouseManager>();

		//find tileMap in the game
		tileMap = GameObject.FindGameObjectWithTag ("TileMap").GetComponent<TileMap> ();

		//set the current movement range at start
		CurrentMovementRange = defaultMaxMovementRange;

		//set the isDestroyed flag to false
		isDestroyed = false;

		//initialize the towing flags
		isTowing = false;
		isBeingTowed = false;

		//set the starting inventory based on the type of ship we have
		switch (this.GetComponent<CombatUnit> ().unitType) {

		case CombatUnit.UnitType.Starship:
			this.warpBooster = Starship.startingWarpBooster;
			this.transwarpBooster = Starship.startingTranswarpBooster;
			this.warpDrive = Starship.startingWarpDrive;
			this.transwarpDrive = Starship.startingTranswarpDrive;
			this.shieldsMax = Starship.engineSectionShieldsMax;
			this.shieldsCurrent = shieldsMax;
			break;
		case CombatUnit.UnitType.Destroyer:
			this.warpBooster = Destroyer.startingWarpBooster;
			this.transwarpBooster = Destroyer.startingTranswarpBooster;
			this.warpDrive = Destroyer.startingWarpDrive;
			this.transwarpDrive = Destroyer.startingTranswarpDrive;
			this.shieldsMax = Destroyer.engineSectionShieldsMax;
			this.shieldsCurrent = shieldsMax;
			break;
		case CombatUnit.UnitType.BirdOfPrey:
			this.warpBooster = BirdOfPrey.startingWarpBooster;
			this.transwarpBooster = BirdOfPrey.startingTranswarpBooster;
			this.warpDrive = BirdOfPrey.startingWarpDrive;
			this.transwarpDrive = BirdOfPrey.startingTranswarpDrive;
			this.shieldsMax = BirdOfPrey.engineSectionShieldsMax;
			this.shieldsCurrent = shieldsMax;
			break;
		case CombatUnit.UnitType.Scout:
			this.warpBooster = Scout.startingWarpBooster;
			this.transwarpBooster = Scout.startingTranswarpBooster;
			this.warpDrive = Scout.startingWarpDrive;
			this.transwarpDrive = Scout.startingTranswarpDrive;
			this.shieldsMax = Scout.engineSectionShieldsMax;
			this.shieldsCurrent = shieldsMax;
			break;
		default:
			this.warpBooster = 0;
			this.transwarpBooster = 0;
			this.warpDrive = false;
			this.transwarpDrive = false;
			this.shieldsMax = 40;
			this.shieldsCurrent = shieldsMax;
			break;

		}

		//add listeners for the MoveMenu booster toggle events
		uiManager.GetComponent<MoveMenu>().OnTurnOnTranswarpBoosterToggle.AddListener(shipShowTranswarpBoosterAction);
		uiManager.GetComponent<MoveMenu>().OnTurnOnWarpBoosterToggle.AddListener(shipShowWarpBoosterAction);
		uiManager.GetComponent<MoveMenu>().OnTurnOffTranswarpBoosterToggle.AddListener(shipRestoreMovementRangeAction);
		uiManager.GetComponent<MoveMenu>().OnTurnOffWarpBoosterToggle.AddListener(shipRestoreMovementRangeAction);

		//add listener for end turn
		gameManager.OnEndTurn.AddListener(playerColorEndTurnAction);

		//add listener to the early setSelectedUnit so range calculations can occur before other listeners react
		mouseManager.OnSetSelectedUnitEarly.AddListener(setSelectedUnitEarlyCalculateMovementRangeAction);

		//add listener to mouseManager OnSignalMove event
		mouseManager.OnSignalMovement.AddListener(shipMovementMoveShipAction);

		//add listener for getting hit by phasor attack
		//CombatManager.OnPhasorAttackHitShipEngineSection.AddListener(attackHitTakeDamageAction);
		CutsceneManager.OnPhasorHitShipEngineSection.AddListener(attackHitTakeDamageAction);

		//add listener for getting hit by torpedo attack
		//CombatManager.OnLightTorpedoAttackHitShipEngineSection.AddListener(attackHitTakeDamageAction);
		//CombatManager.OnHeavyTorpedoAttackHitShipEngineSection.AddListener(attackHitTakeDamageAction);
		CutsceneManager.OnLightTorpedoHitShipEngineSection.AddListener(attackHitTakeDamageAction);
		CutsceneManager.OnHeavyTorpedoHitShipEngineSection.AddListener(attackHitTakeDamageAction);

		//add listener for getting healed by a crystal
		CombatManager.OnCrystalUsedOnShipEngineSection.AddListener(crystalUsedHealDamageAction);

		//add listener for getting repaired by a repair crew
		CombatManager.OnRepairCrewUsedOnShipEngineSection.AddListener(repairUsedRepairSectionAction);

		//add listener for the combat unit being destroyed
		Ship.OnShipDestroyed.AddListener(combatUnitCombatUnitDestroyedAction);

		//add listener for base being destroyed to recalculate range
		Starbase.OnBaseDestroyed.AddListener(combatUnitCalculateMovementRangeSelectedUnitAction);

		//add listener for ship being destroyed to recalculate range
		Ship.OnShipDestroyed.AddListener(combatUnitCalculateMovementRangeSelectedUnitAction);

		//add listener for purchasing items
		uiManager.GetComponent<PurchaseManager>().OnPurchaseItems.AddListener(purchaseAddPurchaseItemsAction);

		//check if this unit came with outfitted items
		if (this.GetComponent<CombatUnit> ().outfittedItemsAtPurchase != null) {

			//add the items
			AddPurchasedItems(this.GetComponent<CombatUnit> ().outfittedItemsAtPurchase,this.GetComponent<CombatUnit> ());

		}

		//add listener for sunburst damage
		Sunburst.OnSunburstDamageDealt.AddListener(incidentalTakeDamageAction);

		//add listener for creating unit from load
		CombatUnit.OnCreateLoadedUnit.AddListener(saveDataResolveLoadedUnitAction);

		//add listener for clearing selected unit
		mouseManager.OnClearSelectedUnit.AddListener(clearSelectedUnitCalculateMovementRangeAction);

	}

	// Update is called once per frame
	protected void Update (){

		//check if ship is supposed to be moving
		if (this.isMoving == true) {

			//check if we are being towed
			if (this.isBeingTowed == true) {

				//check if the ship that is towing us has already updated this frame or not
				if (this.isBeingTowedByShip.GetComponent<EngineSection>().hasUpdatedThisFrame == true) {

					//if the ship towing us has already updated, it will be either translating between hexes, snapped exactly to a hex after advancing movement path,
					//on a worm hole fading out, on a worm hole fading in, or arrived at it's destination hex and no longer moving
					//for some of these conditions, we will want to wait and not move this frame

					//if the ship towing us is translating between hexes, we want to move
					//if the ship towing us is snapped directly to a hex, we want to wait if that hex is our next movement path hex CurrentMovementPath[1]
					//if the ship towing us is fading out, we want to wait if that hex is our next movement path hex CurrentMovementPath[1]
					//if the ship towing us is fading in,  we want to wait if that hex is our next movement path hex CurrentMovementPath[1]
					//if the ship towing us has arrived and is no longer moving, we want to move

					//check if the towing ship actually has a current movement path - if it's arrived it will be null and our check will throw an error
					if (this.isBeingTowedByShip.GetComponent<EngineSection>().currentMovementPath != null) {

						//check if the ship is currently snapped to it's stating hex
						//if it is snapped to it's starting hex, it either just got there via advance movement path, or it is fading out, or it's fading in, or it's arrived
						if (this.isBeingTowedByShip.transform.position == tileMap.HexToWorldCoordinates (this.isBeingTowedByShip.GetComponent<EngineSection>().currentMovementPath [0]) + CombatUnit.mapOffset) {

							//we need to check if the hex that the towing ship is snapped to is our next hex
							if (this.isBeingTowedByShip.GetComponent<EngineSection>().currentMovementPath [0] == this.currentMovementPath [1]) {

								//if it is snapped to our next hex, but it will translate this frame, we are okay to move
								//if it is snapped to our next hex, and it's fading out, we want to wait
								//if it is snapped to our next hex, and it's fading in, we are okay to move
								if (this.isBeingTowedByShip.GetComponent<EngineSection>().isWarping == true && this.isBeingTowedByShip.GetComponent<EngineSection>().warpStatus == WarpStatus.fadingOut) {

									return;

								}

							}

						}

					}

				}
				//the else condition is that the towing ship has not updated this frame yet
				else {

					//check if the towing ship actually has a current movement path - if it's arrived it will be null and our check will throw an error
					if (this.isBeingTowedByShip.GetComponent<EngineSection>().currentMovementPath != null) {

						//our first check is if we are snapped to our starting hex
						if (this.transform.position == tileMap.HexToWorldCoordinates (this.currentMovementPath [0]) + CombatUnit.mapOffset) {

							//if we are, we don't want to move if our towing ship is about going to start warping this frame
							//we can check this by seeing if the towing ship's current movement path [1] and [2] are both wormholes
							if (this.isBeingTowedByShip.GetComponent<EngineSection>().currentMovementPath.Count > 2) {

								if ((tileMap.HexMap [this.isBeingTowedByShip.GetComponent<EngineSection>().currentMovementPath [0]].tileType == HexMapTile.TileType.BlueWormhole && tileMap.HexMap [this.isBeingTowedByShip.GetComponent<EngineSection>().currentMovementPath [1]].tileType == HexMapTile.TileType.BlueWormhole)
									|| (tileMap.HexMap [this.isBeingTowedByShip.GetComponent<EngineSection>().currentMovementPath [0]].tileType == HexMapTile.TileType.RedWormhole && tileMap.HexMap [this.isBeingTowedByShip.GetComponent<EngineSection>().currentMovementPath [1]].tileType == HexMapTile.TileType.RedWormhole)) {

									return;

								}

							}

						}

					}

				}

			}

			//check if we are towing another unit
			if (this.isTowing == true) {

				//check if we are snapped to our starting hex.  We don't want to wait if we are between hexes
				if (this.transform.position == tileMap.HexToWorldCoordinates (this.currentMovementPath [0]) + CombatUnit.mapOffset) {

					//we don't want to wait if we're on a wormhole tile, because we would want to continue fading in or moving off that tile
					if (this.tileMap.HexMap [currentMovementPath [0]].tileType != HexMapTile.TileType.BlueWormhole &&
						this.tileMap.HexMap [currentMovementPath [0]].tileType != HexMapTile.TileType.RedWormhole) {

						//if we are towing a unit, in general we only want to wait if that ship is finishing a warp
						//we don't want to leave the adjacent hex before it is able to continue following us
						if (this.isTowingShip.GetComponent<EngineSection>().isWarping == true && this.isWarping == false) {

							//we want to wait here
							OnWaitForTowedUnitAfterWarping.Invoke(this.GetComponent<Ship>());

							return;

						}

					}

				}

			}

			//the first hex in the currentMovementPath list is our starting hex for a movement between 2 hexes
			Hex startingHex = currentMovementPath [0];

			//the second hex in the currentMovementPath list is our ending hex for a movement between 2 hexes
			Hex endingHex = currentMovementPath [1];

			//check to see if we have moved close enough to the next hex in the movement path to start moving to the next hex after it
			//make sure to include the combat unit map offset
			//I only need to do this if I'm not warping
			if (isWarping == false) {

				if (Vector3.Distance (transform.position, tileMap.HexToWorldCoordinates (endingHex) + CombatUnit.mapOffset) < 0.1f) {

					//call advance movement path
					AdvanceMovementPath (endingHex);

				}

				//check to see if we are trying to go from a wormhole to a wormhole, if we are, we want to start warping
				//we only need to check this if we're not already warping
				//only need to check this if our path is not null - it becomes null when we finish our move from advance movement path
				if (currentMovementPath != null) {

					if ((tileMap.HexMap [startingHex].tileType == HexMapTile.TileType.BlueWormhole && tileMap.HexMap [endingHex].tileType == HexMapTile.TileType.BlueWormhole)
						|| (tileMap.HexMap [startingHex].tileType == HexMapTile.TileType.RedWormhole && tileMap.HexMap [endingHex].tileType == HexMapTile.TileType.RedWormhole)) {

						//set isWarping to true
						isWarping = true;

						//set warpStatus to fadingOut
						warpStatus = WarpStatus.fadingOut;

						//invoke the event
						OnStartWarpFadeOut.Invoke(this.GetComponent<Ship>());

					} 
				}

			}
			//check to see if the ship is in the middle of warping
			//note that this is not an else-if...it's possible to have set isWarping to true in the last if check
			if (isWarping == true) {

				//we want to fade out, then teleport, then fade in, then continue pathing like normal
				if (warpStatus == WarpStatus.fadingOut) {

					FadeOut (endingHex);

				}

				//this if condition will be for the ship fading back in at the 2nd wormhole
				else if (warpStatus == WarpStatus.fadingIn) {

					FadeIn ();

				}

			}

			//this code will execute if we are not in the middle of a warp
			else if (isWarping == false) {

				//instead, we want to move the ship normally, using MoveTowards
				//movement from the current location to the current ending hex using moveTowards is constant velocity
				//the 5.0f float value will change how fast the ship will move toward the target
				transform.position = Vector3.MoveTowards (transform.position, tileMap.HexToWorldCoordinates (endingHex) + CombatUnit.mapOffset, 5.0f * Time.deltaTime);

				//check if the ship that's moving is the targeted ship.  If it is, we want to invoke an event so the targeting cursor can listen and follow
				if (mouseManager.targetedUnit == this.gameObject) {

					//Invoke the OnMoveTargetedShip event
					OnMoveTargetedShip.Invoke (this.GetComponent<Ship>());

				}

				//check if the ship that's moving is the selected ship.  If it is, we want to invoke an event so the selection cursor can listen and follow
				if (mouseManager.selectedUnit == this.gameObject) {

					//Invoke the OnMoveSelectedShip event
					OnMoveSelectedShip.Invoke (this.GetComponent<Ship>());

				}

			}

		}

		//this if condition will be for the ship fading back in at the 2nd wormhole
		else if (isWarping == true && warpStatus == WarpStatus.fadingIn) {

			//call FadeIn function
			FadeIn();

		}


		//set flag to note that the ship has updated this frame
		hasUpdatedThisFrame = true;

	}

	//use late update to set the hasUpdated flat back to false for timing issues
	//this will fire after all ships regular update has executed
	private void LateUpdate(){

		//set has updated this frame to false at the end of the update turn
		hasUpdatedThisFrame = false;

	}

	//this function handles the FadeOut part of the movement update
	private void FadeOut(Hex endingHex){

		//start off by fading the material of the ship
		if (shipRenderer == null) {

			//cache the ship's renderer
			shipRenderer = this.GetComponentInChildren<Renderer> ();
		}

		if (originalMaterial == null) {

			//cache the renderer's material
			originalMaterial = shipRenderer.sharedMaterial;
		}

		if (fadeMaterial == null) {

			//cache the faded material
			fadeMaterial = shipRenderer.material;

			//set the fade material alpha to 0
			fadeColor = fadeMaterial.color;
			fadeColor.a = 0;

		}

		//this check let's us know if we have fully faded the ship yet or not
		if (fadeCounter < timeToFade) {

			//it's possible that the fadeCounter can exceed the timeToFade, so this caps the value at 1
			float fadeFactor = fadeCounter / timeToFade;
			if (fadeFactor > 1.0f) {

				fadeFactor = 1.0f;

			}

			//this code actually changes the color to the faded color over time
			shipRenderer.sharedMaterial.color = Color.Lerp (originalMaterial.color, fadeColor, fadeFactor);

			//increment the fade counter by the time since last frame
			fadeCounter += Time.deltaTime;

		} 

		//in order to hit the else statement, we must have fully faded our ship
		//we can now teleport the ship to the other wormhole, and set the warpStatus to FadingIn
		else {

			//teleport the ship to the 2nd wormhole
			transform.position = tileMap.HexToWorldCoordinates (endingHex) + CombatUnit.mapOffset;

			//reset the fadeCounter (we will want to use it for fadingIn)
			fadeCounter = 0.0f;

			//set warpStatus to fadingIn
			warpStatus = WarpStatus.fadingIn;

			//call the AdvanceMovementPath function to increment the pathfinding
			AdvanceMovementPath (endingHex);

		}


	}

	//this function handles the FadeIn part of the movement update
	private void FadeIn(){

		//this check let's us know if we have fully faded the ship back in yet or not
		if (fadeCounter < timeToFade) {

			//it's possible that the fadeCounter can exceed the timeToFade, so this caps the value at 1
			float fadeFactor = fadeCounter / timeToFade;
			if (fadeFactor > 1.0f) {

				fadeFactor = 1.0f;

			}

			//this code actually changes the from the faded color to the original color over time
			shipRenderer.sharedMaterial.color = Color.Lerp (fadeColor, originalMaterial.color, fadeFactor);

			//increment the fade counter by the time since last frame
			fadeCounter += Time.deltaTime;

		} 

		//in order to hit the else statement, we must have fully faded our ship back in
		else {

			//reset the fadeCounter (in case the ship warps again later)
			fadeCounter = 0.0f;

			//set the isWarping to false
			isWarping = false;

			//set the ship material back to the shared material, and destroy the instance created for the warp fading
			//this will prevent memory leaks and reduce draw calls
			shipRenderer.sharedMaterial = originalMaterial;
			Destroy (fadeMaterial);

			//check if there is still movement left to go to another hex after we've faded in
			if (currentMovementPath != null && currentMovementPath.Count > 1) {

				//continue moving after fade in complete
				OnResumeMovementAfterFadeIn.Invoke(this.GetComponent<Ship>());

			}

		}

	}

	//this function moves the ship
	//towedUnit is a ship that this ship is towing with a tractor beam
	//towingUnit is a ship that is towing the current ship with a tractor beam
	private void MoveShip(Ship shipToMove, Hex targetHex, Ship towedUnit = null, Ship towingUnit = null){

		//check to make sure this ship is the one that's supposed to move
		if (shipToMove == this.GetComponent<Ship>()) {

			//set the towing bools based on inputs fed to function
			if (towedUnit != null) {

				//if the function is passed a towed unit, this unit is towing
				this.isTowing = true;
				this.isTowingShip = towedUnit;

			} else if (towingUnit != null) {

				//if the function is passed a towing unit, this unit is being towed
				this.isBeingTowed = true;
				this.isBeingTowedByShip = towingUnit;

			}

			//this checks if there is a towing unit - if there is not, wer're moving ourselves
			if (towingUnit == null) {

				//cache the movement starting and ending hexes for sending a message later
				movementStartingHex = this.GetComponent<Ship>().currentLocation;
				movementEndingHex = targetHex;

				//first, I need to generate the path and cache it
				currentMovementPath = tileMap.GeneratePath (movementStartingHex, movementEndingHex, this.ReachableHexes);

				//update the distance moved based on how long the path is
				distanceMovedThisMove = currentMovementPath.Count - 1;

			} 

			//the else condition is that there is a towing unit - we are being towed
			else {

				//our starting hex is still our current location
				movementStartingHex = this.GetComponent<Ship>().currentLocation;

				//since we're not calling GeneratePath(), we need to instantiate a new list for current movement path
				currentMovementPath = new List<Hex> ();

				//our path will start with our current location
				currentMovementPath.Add (movementStartingHex);

				//now, we need to add the towing unit's movement path, minus the final tile of movement
				for (int i = 0; i < towingUnit.GetComponent<EngineSection>().currentMovementPath.Count - 1; i++) {

					currentMovementPath.Add (towingUnit.GetComponent<EngineSection>().currentMovementPath [i]);

				}

				//the ending hex is now the last hex in the movement path
				movementEndingHex = currentMovementPath [currentMovementPath.Count - 1];

				//if there is a towing unit, our distance moved (for range purposes) is zero
				distanceMovedThisMove = 0;

			}


			//Debug.Log ("movementPath length = " + currentMovementPath.Count);

			//set the isMoving flag to true
			this.isMoving = true;

			//Debug.Log ("Ship is Moving Flag set");

			//if we are towing a unit, we need to call a movement function on that ship
			if (towedUnit != null) {

				//invoke the OnMoveWhileTowing event
				OnMoveWhileTowing.Invoke(this.GetComponent<Ship>());

				//target hex is the next-to-last hex in our movement path, which is 2 less than the count
				//we pass null for the ship being towed, and this for the ship doing the towing
				towedUnit.GetComponent<EngineSection>().MoveShip (towedUnit, this.currentMovementPath [this.currentMovementPath.Count - 2], null, this.GetComponent<Ship>());

			}

			//determine booster usage
			DetermineBoosterUsage();

			//invoke the OnMoveStart event
			OnMoveStart.Invoke (this.GetComponent<Ship>());

		}

	}





	//this function will take the current movement path and increment it one space closer to the destination
	private void AdvanceMovementPath(Hex endingHex){

		//if the current movement path list is empty, return
		if (currentMovementPath == null) {

			return;
		}

		//this function will be called when the ship is close to a hex on the path
		//since it might not be exactly there yet, teleport it to that spot
		//make sure to include the mapOffset for units
		transform.position = tileMap.HexToWorldCoordinates(endingHex) + CombatUnit.mapOffset;

		//set the ship's current location to the ending hex in the movement
		GetComponent<Ship>().currentLocation = endingHex;

		//remove the old first hex in the movement path
		currentMovementPath.RemoveAt(0);

		//if there is only 1 hex left in the path, we are at the ultimate destination, and we are
		//already there, so I can clear out the movement path
		if (currentMovementPath.Count == 1) {

			currentMovementPath = null;


			//Debug.Log ("Arrived At Destination");

			//update movement range
			distanceMovedThisTurn += distanceMovedThisMove;
			CurrentMovementRange -= distanceMovedThisMove;


			this.CalculateMovementRange (this.gameObject);


			//set the isMoving flag to false
			isMoving = false;

			//invoke the OnMoveFinish event
			OnMoveFinish.Invoke(this.GetComponent<Ship>());
			OnMoveFromToFinish.Invoke (this.GetComponent<Ship>(), movementStartingHex, movementEndingHex);

			//clear the towing flags now that we've arrived
			//do this after the events so the events capture the correct tow status
			this.isTowing = false;
			this.isBeingTowed = false;
			this.isTowingShip = null;
			this.isBeingTowedByShip = null;

		}

	}
		
	//this function will calculate all hexes a ship can reach in range
	private void CalculateMovementRange(GameObject gameObject){

		//check if the gameObject passed to the function is a ship
		if (gameObject.GetComponent<Ship> () == true) {

			//if it is a ship, check if it is this ship
			if (gameObject.GetComponent<Ship> () == this.GetComponent<Ship>()) {

				//get a list of all hex locations that are reachable and
				//copy list to public variable ReachableHexes so mouseManager can access the reachable areas
				ReachableHexes = tileMap.ReachableTiles (this.GetComponent<Ship>().currentLocation,CurrentMovementRange);

				//check if reachable hexes contains one of the other color's home tile
				if (this.GetComponent<CombatUnit> ().owner.color != Player.Color.Blue) {

					foreach (Hex hex in tileMap.BlueStartTiles) {

						if (ReachableHexes.Contains (hex)) {

							ReachableHexes.Remove (hex);

						}

					}

				}

				if (this.GetComponent<CombatUnit> ().owner.color != Player.Color.Green) {

					foreach (Hex hex in tileMap.GreenStartTiles) {

						if (ReachableHexes.Contains (hex)) {

							ReachableHexes.Remove (hex);

						}

					}

				}
				if (this.GetComponent<CombatUnit> ().owner.color != Player.Color.Purple) {

					foreach (Hex hex in tileMap.PurpleStartTiles) {

						if (ReachableHexes.Contains (hex)) {

							ReachableHexes.Remove (hex);

						}

					}

				}
				if (this.GetComponent<CombatUnit> ().owner.color != Player.Color.Red) {

					foreach (Hex hex in tileMap.RedStartTiles) {

						if (ReachableHexes.Contains (hex)) {

							ReachableHexes.Remove (hex);

						}

					}

				}

			}

		}

	}

	//this function refreshes the range when boosters are toggled
	private void RangeRefresh(){

		//we only want to do something when the ship is not moving
		if (this.isMoving == false) {

			CalculateMovementRange (this.gameObject);
			OnRefreshRange.Invoke (thisShip);

		}

	}

	//create a function that uses a warpBooster to increase the movement range for the current turn
	private void ShowWarpBooster(Ship ship){

		//check to make sure the ship passed matches this ship
		if (ship == thisShip) {

			//increase the max movement range
			this.maxMovementRange = warpMovementRange;

			//set the current movement range
			this.CurrentMovementRange = this.maxMovementRange - this.distanceMovedThisTurn;

			//refresh the ship's range
			this.RangeRefresh ();

		} 

	}

	//create a function that uses a transwarpBooster to increase the movement range for the current turn
	private void ShowTranswarpBooster(Ship ship){

		//check to make sure the ship passed matches this ship
		if (ship == thisShip) {

			//increase the max movement range
			this.maxMovementRange = transwarpMovementRange;

			//set the current movement range
			this.CurrentMovementRange = this.maxMovementRange - this.distanceMovedThisTurn;

			//refresh the ship's range
			this.RangeRefresh ();


		}

	}


	//function to restore max movement range at end of turn when boosters wear off
	private void RestoreMovementRange(Ship ship){

		//check to make sure the ship passed matches this ship
		if (ship == thisShip) {

			//if the ship has transwarp drive, the maxMovementRange should be 7
			if (transwarpDrive == true) {

				//set the max movement range
				this.maxMovementRange = transwarpMovementRange;

				//check if the section is destroyed - if so, the max is zero
				if (this.isDestroyed == true) {

					this.maxMovementRange = 0;

				}

				//set the current movement range
				this.CurrentMovementRange = this.maxMovementRange - this.distanceMovedThisTurn;

				//if it becomes less than zero due to destroying/repairing, set to zero
				if (this.CurrentMovementRange < 0) {

					this.CurrentMovementRange = 0;

				}

			}

			//else if the ship has warp drive, the maxMovementRange should be 5
			else if (warpDrive == true) {

				//set the max movement range
				this.maxMovementRange = warpMovementRange;

				//check if the section is destroyed - if so, the max is zero
				if (this.isDestroyed == true) {

					this.maxMovementRange = 0;

				}

				//set the current movement range
				this.CurrentMovementRange = this.maxMovementRange - this.distanceMovedThisTurn;

				//if it becomes less than zero due to destroying/repairing, set to zero
				if (this.CurrentMovementRange < 0) {

					this.CurrentMovementRange = 0;

				}

			}

			//else if we've already used transwarp booster, the max should be 7
			else if (usedTranswarpBoosterThisTurn == true) {

				//set the max movement range
				this.maxMovementRange = transwarpMovementRange;

				//check if the section is destroyed - if so, the max is zero
				if (this.isDestroyed == true) {

					this.maxMovementRange = 0;

				}

				//set the current movement range
				this.CurrentMovementRange = this.maxMovementRange - this.distanceMovedThisTurn;

				//if it becomes less than zero due to destroying/repairing, set to zero
				if (this.CurrentMovementRange < 0) {

					this.CurrentMovementRange = 0;

				}

			}

			//else if we've already used warp booster, the max should be 5
			else if (usedWarpBoosterThisTurn == true) {

				//set the max movement range
				this.maxMovementRange = warpMovementRange;

				//check if the section is destroyed - if so, the max is zero
				if (this.isDestroyed == true) {

					this.maxMovementRange = 0;

				}

				//set the current movement range
				this.CurrentMovementRange = this.maxMovementRange - this.distanceMovedThisTurn;

				//if it becomes less than zero due to destroying/repairing, set to zero
				if (this.CurrentMovementRange < 0) {

					this.CurrentMovementRange = 0;

				}

			}

			//the else condition is there are no permanent engine section upgrades
			else {

				//restore the default max movement range
				this.maxMovementRange = defaultMaxMovementRange;

				//check if the section is destroyed - if so, the max is zero
				if (this.isDestroyed == true) {

					this.maxMovementRange = 0;

				}

				//set the current movement range
				this.CurrentMovementRange = this.maxMovementRange - this.distanceMovedThisTurn;

				//if it becomes less than zero due to destroying/repairing, set to zero
				if (this.CurrentMovementRange < 0) {

					this.CurrentMovementRange = 0;

				}

			}

			//refresh the ship's range
			RangeRefresh ();

		}

	}

	//this function uses a warp booster - it decrements inventory and notes that a booster was used this turn
	private void UseWarpBooster(){

		//check to see if we've already used the booster this turn
		if (usedWarpBoosterThisTurn == false) {

			//set used flag to true
			usedWarpBoosterThisTurn = true;

			//decrement the booster inventory
			warpBooster -= 1;

			//invoke the OnUseWarpBooster event
			OnUseWarpBooster.Invoke(thisShip);

		}

		//increase the max movement range
		this.maxMovementRange = warpMovementRange;

		//set the current movement range
		this.CurrentMovementRange = this.maxMovementRange - this.distanceMovedThisTurn;


	}

	//this function uses a transwarp booster - it decrements inventory and notes that a booster was used this turn
	private void UseTranswarpBooster(){

		//check to see if we've already used the booster this turn
		if (usedTranswarpBoosterThisTurn == false) {

			//set used flag to true
			usedTranswarpBoosterThisTurn = true;

			//decrement the booster inventory
			transwarpBooster -= 1;

			//invoke the OnUseTranswarpBooster event
			OnUseTranswarpBooster.Invoke(thisShip);

		}

		//increase the max movement range
		this.maxMovementRange = transwarpMovementRange;

		//set the current movement range
		this.CurrentMovementRange = this.maxMovementRange - this.distanceMovedThisTurn;

	}

	//this function will determine whether a moving ship used a booster
	private void DetermineBoosterUsage(){

		//Debug.Log (this.GetComponent<Ship>().shipName + " determine booster usage");

		//check if the ship is being towed (another ship that is towing this ship)
		//if this ship is being towed, our movement is free and we don't use any boosters
		//otherwise, if this ship is not being towed, we need to check for booster usage
		if (this.isBeingTowed == false) {

			//check to see if booster toggles are active when MoveShip was called
			if (uiManager.GetComponent<MoveMenu> ().useTranswarpBoosterToggle.isOn == true) {

				//if so, call UseTranswarpBooster from engine section
				this.UseTranswarpBooster ();

			} else if (uiManager.GetComponent<MoveMenu> ().useWarpBoosterToggle.isOn == true) {

				//if so, call UseWwarpBooster from engine section
				this.UseWarpBooster ();

			}

		}




	}

	//this function will handle taking damage if the section is hit by an attack
	private void TakeDamage(CombatUnit targetedUnit, int damage){

		//first, check if the unit that the engine section is attached to is the targeted unit that got hit
		if (this.GetComponent<CombatUnit> () == targetedUnit) {

			//this is the targeted unit - we can reduce the shields by the damage
			this.shieldsCurrent -= damage;

			//check if this puts shields at or below zero - if so, set to zero and call DestroySection
			if (shieldsCurrent <= 0) {

				shieldsCurrent = 0;
				this.DestroySection ();

			}

			OnEngineDamageTaken.Invoke ();

		}

	}

	//this function will handle healing damage if the section has a crystal used on it
	private void HealDamage(CombatUnit targetedUnit, int shieldsHealed){

		//first, check if the unit that the engine section is attached to is the targeted unit that got hit
		if (this.GetComponent<CombatUnit> () == targetedUnit) {

			//this is the targeted unit - we can increase the shields by the amount of healing
			this.shieldsCurrent += shieldsHealed;

			//check if this puts shields at or above max - if so, set to max
			if (shieldsCurrent > shieldsMax) {

				shieldsCurrent = shieldsMax;
				Debug.LogError ("We somehow healed this section for more than max shields");
			
			}

			OnEngineDamageTaken.Invoke ();

		}

	}

	//this function will handle repairing the section from a repair crew
	private void RepairSection(CombatUnit targetedUnit){

		//first, check if the unit that the section is attached to is the targeted unit that got hit
		if (this.GetComponent<CombatUnit> () == targetedUnit) {

			//check if the section wasn't destroyed - that would be a logic error
			if (this.isDestroyed == false) {

				Debug.LogError ("We somehow tried to repair a section that wasn't destroyed");

			}

			//this is the targeted unit - we can repair the section and restore the shields to 1
			this.isDestroyed = false;
			this.shieldsCurrent = 1;

			//calculate movement range so our current movement range will be restored
			this.RestoreMovementRange (this.GetComponent<Ship>());

			//invoke the repaired section event
			OnEngineSectionRepaired.Invoke(this.GetComponent<CombatUnit>());

		}

	}

	//this function handles the section taking lethal damage and being destroyed
	private void DestroySection (){

		//if the section is destroyed, all the inventory on this section is removed
		this.warpBooster = 0;
		this.transwarpBooster = 0;
		this.warpDrive = false;
		this.transwarpDrive = false;
		this.maxMovementRange = 0;


		//set the isDestroyed flag to true
		this.isDestroyed = true;


		//calculate movement range so our current movement range will go to zero
		this.RestoreMovementRange (this.GetComponent<Ship>());

		//invoke the destroyed section event
		OnEngineSectionDestroyed.Invoke(this.GetComponent<CombatUnit>());

	}

	//this function will clean up variables at end of turn
	private void EndTurn(Player.Color currentTurn){

		//check if the color passed to the function matches the owner color
		//if so, our turn is ending, and we need to reset stuff
		if (currentTurn == this.GetComponent<Ship>().owner.color) {

			this.usedTranswarpBoosterThisTurn = false;
			this.usedWarpBoosterThisTurn = false;
			this.RestoreMovementRange(this.GetComponent<Ship>());
			this.distanceMovedThisTurn = 0;
			this.CurrentMovementRange = maxMovementRange;
				
		}

	}

	//this function will remove all listeners when the combat unit is destroyed
	private void CombatUnitDestroyed(CombatUnit combatUnitDestroyed){

		//check if the passed combat unit is this combat unit
		if (this.GetComponent<CombatUnit> () == combatUnitDestroyed) {
			
			RemoveAllListeners ();

		}

	}

	//this function handles adding items via purchase
	private void AddPurchasedItems(Dictionary<string,int> purchasedItems, CombatUnit combatUnit){

		//check if this is the unit passed
		if (combatUnit == this.GetComponent<CombatUnit> ()) {

			//check if the purchased items have engine items
			if (purchasedItems.ContainsKey ("Warp Booster")) {

				//increase the quantity by the value
				this.warpBooster += purchasedItems ["Warp Booster"];

			}

			if (purchasedItems.ContainsKey ("Transwarp Booster")) {

				//increase the quantity by the value
				this.transwarpBooster += purchasedItems ["Transwarp Booster"];

			}

			if (purchasedItems.ContainsKey ("Warp Drive")) {

				//increase the quantity by the value
				this.warpDrive = true;

			}

			if (purchasedItems.ContainsKey ("Transwarp Drive")) {

				//increase the quantity by the value
				this.transwarpDrive = true;

			}

			//restore the movement range
			RestoreMovementRange(this.GetComponent<Ship>());

			//invoke event
			OnInventoryUpdated.Invoke (this.GetComponent<CombatUnit>());

		}

	}

	//this function resolves a loaded unit
	private void ResolveLoadedUnit(CombatUnit combatUnit, FileManager.SaveGameData saveGameData){

		//first, check that the unit matches
		if (this.GetComponent<CombatUnit> () == combatUnit) {

			//if we have the right unit, use the saveGameData to set values
			//need to find the index of the player colors from the saveGameData file
			for (int i = 0; i < GameManager.numberPlayers; i++) {

				//check if the color matches
				if (saveGameData.playerColor [i] == combatUnit.owner.color) {

					//we have found the right index for this player
					//now we need to check what kind of ship we're attached to
					switch (combatUnit.unitType) {

					case CombatUnit.UnitType.Scout:

						this.isDestroyed = saveGameData.scoutEngineSectionIsDestroyed [i, combatUnit.serialNumber];
						this.shieldsCurrent = saveGameData.scoutEngineSectionShieldsCurrent [i, combatUnit.serialNumber];
						this.warpBooster = saveGameData.scoutWarpBooster [i, combatUnit.serialNumber];
						this.transwarpBooster = saveGameData.scoutTranswarpBooster [i, combatUnit.serialNumber];
						this.warpDrive = saveGameData.scoutWarpDrive [i, combatUnit.serialNumber];
						this.transwarpDrive = saveGameData.scoutTranswarpDrive [i, combatUnit.serialNumber];
						this.distanceMovedThisTurn = saveGameData.scoutDistanceMovedThisTurn [i, combatUnit.serialNumber];
						this.usedWarpBoosterThisTurn = saveGameData.scoutUsedWarpBoosterThisTurn [i, combatUnit.serialNumber];
						this.usedTranswarpBoosterThisTurn = saveGameData.scoutUsedTranswarpBoosterThisTurn [i, combatUnit.serialNumber];

						break;

					case CombatUnit.UnitType.BirdOfPrey:

						this.isDestroyed = saveGameData.birdOfPreyEngineSectionIsDestroyed [i, combatUnit.serialNumber];
						this.shieldsCurrent = saveGameData.birdOfPreyEngineSectionShieldsCurrent [i, combatUnit.serialNumber];
						this.warpBooster = saveGameData.birdOfPreyWarpBooster [i, combatUnit.serialNumber];
						this.transwarpBooster = saveGameData.birdOfPreyTranswarpBooster [i, combatUnit.serialNumber];
						this.warpDrive = saveGameData.birdOfPreyWarpDrive [i, combatUnit.serialNumber];
						this.transwarpDrive = saveGameData.birdOfPreyTranswarpDrive [i, combatUnit.serialNumber];
						this.distanceMovedThisTurn = saveGameData.birdOfPreyDistanceMovedThisTurn [i, combatUnit.serialNumber];
						this.usedWarpBoosterThisTurn = saveGameData.birdOfPreyUsedWarpBoosterThisTurn [i, combatUnit.serialNumber];
						this.usedTranswarpBoosterThisTurn = saveGameData.birdOfPreyUsedTranswarpBoosterThisTurn [i, combatUnit.serialNumber];

						break;

					case CombatUnit.UnitType.Destroyer:

						this.isDestroyed = saveGameData.destroyerEngineSectionIsDestroyed [i, combatUnit.serialNumber];
						this.shieldsCurrent = saveGameData.destroyerEngineSectionShieldsCurrent [i, combatUnit.serialNumber];
						this.warpBooster = saveGameData.destroyerWarpBooster [i, combatUnit.serialNumber];
						this.transwarpBooster = saveGameData.destroyerTranswarpBooster [i, combatUnit.serialNumber];
						this.warpDrive = saveGameData.destroyerWarpDrive [i, combatUnit.serialNumber];
						this.transwarpDrive = saveGameData.destroyerTranswarpDrive [i, combatUnit.serialNumber];
						this.distanceMovedThisTurn = saveGameData.destroyerDistanceMovedThisTurn [i, combatUnit.serialNumber];
						this.usedWarpBoosterThisTurn = saveGameData.destroyerUsedWarpBoosterThisTurn [i, combatUnit.serialNumber];
						this.usedTranswarpBoosterThisTurn = saveGameData.destroyerUsedTranswarpBoosterThisTurn [i, combatUnit.serialNumber];

						break;

					case CombatUnit.UnitType.Starship:

						this.isDestroyed = saveGameData.starshipEngineSectionIsDestroyed [i, combatUnit.serialNumber];
						this.shieldsCurrent = saveGameData.starshipEngineSectionShieldsCurrent [i, combatUnit.serialNumber];
						this.warpBooster = saveGameData.starshipWarpBooster [i, combatUnit.serialNumber];
						this.transwarpBooster = saveGameData.starshipTranswarpBooster [i, combatUnit.serialNumber];
						this.warpDrive = saveGameData.starshipWarpDrive [i, combatUnit.serialNumber];
						this.transwarpDrive = saveGameData.starshipTranswarpDrive [i, combatUnit.serialNumber];
						this.distanceMovedThisTurn = saveGameData.starshipDistanceMovedThisTurn [i, combatUnit.serialNumber];
						this.usedWarpBoosterThisTurn = saveGameData.starshipUsedWarpBoosterThisTurn [i, combatUnit.serialNumber];
						this.usedTranswarpBoosterThisTurn = saveGameData.starshipUsedTranswarpBoosterThisTurn [i, combatUnit.serialNumber];

						break;

					default:

						break;

					}

					//restore the movement range
					RestoreMovementRange(this.GetComponent<Ship>());

				}

			}

		}

	}

	//private function to handle onDestroy
	private void OnDestroy(){

		RemoveAllListeners ();

	}

	//private function to remove all event listeners
	private void RemoveAllListeners(){

		if (uiManager != null) {
			
			//remove listeners for the MoveMenu booster toggle events
			uiManager.GetComponent<MoveMenu> ().OnTurnOnTranswarpBoosterToggle.RemoveListener (shipShowTranswarpBoosterAction);
			uiManager.GetComponent<MoveMenu> ().OnTurnOnWarpBoosterToggle.RemoveListener (shipShowWarpBoosterAction);
			uiManager.GetComponent<MoveMenu> ().OnTurnOffTranswarpBoosterToggle.RemoveListener (shipRestoreMovementRangeAction);
			uiManager.GetComponent<MoveMenu> ().OnTurnOffWarpBoosterToggle.RemoveListener (shipRestoreMovementRangeAction);

			//remove listener for purchasing items
			uiManager.GetComponent<PurchaseManager>().OnPurchaseItems.RemoveListener(purchaseAddPurchaseItemsAction);

		}

		if (gameManager != null) {
			
			//remove listener for end turn
			gameManager.OnEndTurn.RemoveListener (playerColorEndTurnAction);

		}

		if (mouseManager != null) {

			//remove listener to the early setSelectedUnit so range calculations can occur before other listeners react
			mouseManager.OnSetSelectedUnitEarly.RemoveListener (setSelectedUnitEarlyCalculateMovementRangeAction);

			//remove listener to mouseManager OnSignalMove event
			mouseManager.OnSignalMovement.RemoveListener (shipMovementMoveShipAction);

			//remove listener for clearing selected unit
			mouseManager.OnClearSelectedUnit.RemoveListener(clearSelectedUnitCalculateMovementRangeAction);

		}

		//remove listener for getting hit by phasor attack
		//CombatManager.OnPhasorAttackHitShipEngineSection.RemoveListener(attackHitTakeDamageAction);
		CutsceneManager.OnPhasorHitShipEngineSection.RemoveListener(attackHitTakeDamageAction);

		//remove listener for getting hit by torpedo attack
		//CombatManager.OnLightTorpedoAttackHitShipEngineSection.RemoveListener(attackHitTakeDamageAction);
		//CombatManager.OnHeavyTorpedoAttackHitShipEngineSection.RemoveListener(attackHitTakeDamageAction);
		CutsceneManager.OnLightTorpedoHitShipEngineSection.RemoveListener(attackHitTakeDamageAction);
		CutsceneManager.OnHeavyTorpedoHitShipEngineSection.RemoveListener(attackHitTakeDamageAction);

		//remove listener for getting healed by a crystal
		CombatManager.OnCrystalUsedOnShipEngineSection.RemoveListener(crystalUsedHealDamageAction);

		//remove listener for getting repaired by a repair crew
		CombatManager.OnRepairCrewUsedOnShipEngineSection.RemoveListener(repairUsedRepairSectionAction);

		//remove listener for the combat unit being destroyed
		Ship.OnShipDestroyed.RemoveListener(combatUnitCombatUnitDestroyedAction);

		//remove listener for base being destroyed to recalculate range
		Starbase.OnBaseDestroyed.RemoveListener(combatUnitCalculateMovementRangeSelectedUnitAction);

		//remove listener for ship being destroyed to recalculate range
		Ship.OnShipDestroyed.RemoveListener(combatUnitCalculateMovementRangeSelectedUnitAction);

		//remove listener for sunburst damage
		Sunburst.OnSunburstDamageDealt.RemoveListener(incidentalTakeDamageAction);

		//remove listener for creating unit from load
		CombatUnit.OnCreateLoadedUnit.RemoveListener(saveDataResolveLoadedUnitAction);

	}

}
