using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PhaserSection : MonoBehaviour {

	//variables for the managers
	private UIManager uiManager;
	private GameManager gameManager;
	private MouseManager mouseManager;

	//variable to hold the reference to the tileMap
	private TileMap tileMap;

	//define the variables that the phaser section must manage
	public int phaserRadarShot {

		get;
		private set;

	}

	public bool xRayKernalUpgrade {

		get;
		private set;

	}

	public bool phaserRadarArray {

		get;
		private set;

	}

	public bool tractorBeam {

		get;
		private set;

	}

	//variable bools to track whether phasers or tractor beam have been used this turn
	public bool usedPhasersThisTurn {

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

	//bool to track whether tractor beam is engaged
	public bool tractorBeamIsEngaged {

		get;
		private set;

	}

	//bool to track whether tractor beam is primed
	public bool tractorBeamIsPrimed {

		get;
		private set;

	}

	//define a variable to control tractor beam range
	public int tractorBeamRange {

		get;
		private set;

	}

	//define a variable to control phaser attack range
	public int phaserRange {

		get;
		private set;

	}

	//this is the list of hexes that the tractor beam targeting can reach
	public List<Hex> TargetableTractorBeamHexes {
		get;
		private set;
	}

	//this is the list of hexes that the phaser targeting can reach
	public List<Hex> TargetablePhaserHexes {
		get;
		private set;
	}

	//events to handle tractor beam
	//make it static so that subscribers don't have to subscribe to every ship
	public static TractorBeamEvent OnDisengageTractorBeam = new TractorBeamEvent();
	public static TractorBeamEvent OnEngageTractorBeam = new TractorBeamEvent();
	public static TractorBeamEvent OnPrimeTractorBeam = new TractorBeamEvent();

	//simple class so I can have my event pass the ship parameter in the event
	public class TractorBeamEvent : UnityEvent<Ship>{};


	//events to handle phaser attacks
	public static PhaserAttackEvent OnFirePhasers = new PhaserAttackEvent();

	//simple class so I can have my event pass the combat units to the event
	public class PhaserAttackEvent : UnityEvent<CombatUnit,CombatUnit,string>{};


	//event to announce the section was destroyed
	public static PhaserSectionDestroyedEvent OnPhaserSectionDestroyed = new PhaserSectionDestroyedEvent();

	//event to announce the section was repaired
	public static PhaserSectionDestroyedEvent OnPhaserSectionRepaired = new PhaserSectionDestroyedEvent();

	//class for event to announce section was destroyed
	public class PhaserSectionDestroyedEvent : UnityEvent<CombatUnit>{};

	//event to announce damage was taken
	public static UnityEvent OnPhaserDamageTaken = new UnityEvent();

	//event to announce inventory updated
	public static InventoryUpdateEvent OnInventoryUpdated = new InventoryUpdateEvent();
	public class InventoryUpdateEvent : UnityEvent<CombatUnit>{};

	//unityActions
	private UnityAction<Player.Color> colorEndTurnAction;
	private UnityAction<Ship> shipPrimeTractorBeamAction;
	private UnityAction<Ship> shipDisengageTractorBeamAction;
	private UnityAction calculatePhaserRangeSelectedUnitAction;
	private UnityAction<Ship> shipEngageTractorBeamAction;
	private UnityAction<Ship> shipCalculatePhaserRangeAction;
	private UnityAction<CombatUnit,CombatUnit,string> phasersFirePhasersAction;
	private UnityAction<CombatUnit,CombatUnit,int> attackHitTakeDamageAction;
	private UnityAction<CombatUnit,CombatUnit,CombatManager.CrystalType,int> crystalUsedHealDamageAction;
	private UnityAction<CombatUnit,CombatUnit> repairCrewRepairAction;
	private UnityAction<CombatUnit> combatUnitCombatUnitDestroyedAction;
	private UnityAction<CombatUnit> combatUnitCalculatePhaserRangeAction;
	private UnityAction<Dictionary<string,int>,int,CombatUnit> purchaseAddPurchaseItemsAction;
	private UnityAction<CombatUnit,int> incidentalTakeDamageAction;
	private UnityAction<CombatUnit,FileManager.SaveGameData> saveDataResolveLoadedUnitAction;

	// Use this for initialization
	public void Init () {

		//get the managers
		uiManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
		gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
		mouseManager = GameObject.FindGameObjectWithTag("MouseManager").GetComponent<MouseManager>();

		//find tileMap in the game
		tileMap = GameObject.FindGameObjectWithTag ("TileMap").GetComponent<TileMap> ();

		//initialize variables
		this.isDestroyed = false;
		this.tractorBeamIsEngaged = false;
		this.tractorBeamIsPrimed = false;

		//set the starting ranges
		tractorBeamRange = 1;
		phaserRange = 2;

		//set the starting inventory based on the type of ship we have
		switch (this.GetComponent<CombatUnit> ().unitType) {

		case CombatUnit.UnitType.Starship:
			this.phaserRadarShot = Starship.startingPhaserRadarShot;
			this.xRayKernalUpgrade = Starship.startingXRayKernelUpgrade;
			this.phaserRadarArray = Starship.startingPhaserRadarArray;
			this.tractorBeam = Starship.startingTractorBeam;
			this.shieldsMax = Starship.phaserSectionShieldsMax;
			this.shieldsCurrent = shieldsMax;
			break;
		case CombatUnit.UnitType.Destroyer:
			this.phaserRadarShot = Destroyer.startingPhaserRadarShot;
			this.xRayKernalUpgrade = Destroyer.startingXRayKernelUpgrade;
			this.phaserRadarArray = Destroyer.startingPhaserRadarArray;
			this.tractorBeam = Destroyer.startingTractorBeam;
			this.shieldsMax = Destroyer.phaserSectionShieldsMax;
			this.shieldsCurrent = shieldsMax;
			break;
		case CombatUnit.UnitType.BirdOfPrey:
			this.phaserRadarShot = BirdOfPrey.startingPhaserRadarShot;
			this.xRayKernalUpgrade = BirdOfPrey.startingXRayKernelUpgrade;
			this.phaserRadarArray = BirdOfPrey.startingPhaserRadarArray;
			this.tractorBeam = BirdOfPrey.startingTractorBeam;
			this.shieldsMax = BirdOfPrey.phaserSectionShieldsMax;
			this.shieldsCurrent = shieldsMax;
			break;
		case CombatUnit.UnitType.Scout:
			this.phaserRadarShot = Scout.startingPhaserRadarShot;
			this.xRayKernalUpgrade = Scout.startingXRayKernelUpgrade;
			this.phaserRadarArray = Scout.startingPhaserRadarArray;
			this.tractorBeam = Scout.startingTractorBeam;
			this.shieldsMax = Scout.phaserSectionShieldsMax;
			this.shieldsCurrent = shieldsMax;
			break;
		default:
			this.phaserRadarShot = 0;
			this.xRayKernalUpgrade = false;
			this.phaserRadarArray = false;
			this.tractorBeam = false;
			this.shieldsMax = 40;
			this.shieldsCurrent = shieldsMax;
			break;

		}

		//set the actions
		colorEndTurnAction = (color) => {EndTurn(color);};
		shipPrimeTractorBeamAction = (ship) => {PrimeTractorBeam(ship);};
		shipDisengageTractorBeamAction = (ship) => {DisengageTractorBeam(ship);};
		calculatePhaserRangeSelectedUnitAction = () => {CalculatePhaserRange(mouseManager.selectedUnit);};
		shipEngageTractorBeamAction = (ship) => {EngageTractorBeam(ship);};
		shipCalculatePhaserRangeAction = (ship) => {CalculatePhaserRange(ship.gameObject);};
		phasersFirePhasersAction = (attackingUnit,targetedUnit,sectionTargeted) => {FirePhasers(attackingUnit,targetedUnit,sectionTargeted);};
		attackHitTakeDamageAction = (attackingUnit, targetedUnit, damage) => {TakeDamage(targetedUnit,damage);};
		crystalUsedHealDamageAction = (selectedUnit,targetedUnit,crystalType,shieldsHealed) => {HealDamage(targetedUnit,shieldsHealed);};
		repairCrewRepairAction = (selectedUnit,targetedUnit) => {RepairSection(targetedUnit);};
		combatUnitCombatUnitDestroyedAction = (combatUnit) => {CombatUnitDestroyed(combatUnit);};
		combatUnitCalculatePhaserRangeAction = (combatUnit) => {

			//check to make sure the selected unit is not null
			if(mouseManager.selectedUnit != null){

				CalculatePhaserRange(mouseManager.selectedUnit);

			}

		};

		purchaseAddPurchaseItemsAction = (purchasedItems,purchasedValue,combatUnit) => {AddPurchasedItems(purchasedItems,combatUnit);};
		incidentalTakeDamageAction = (combatUnit, damage) => {TakeDamage(combatUnit,damage);};
		saveDataResolveLoadedUnitAction = (combatUnit,saveGameData) => {ResolveLoadedUnit(combatUnit,saveGameData);};



		//check if this unit came with outfitted items
		if (this.GetComponent<CombatUnit> ().outfittedItemsAtPurchase != null) {

			//add the items
			AddPurchasedItems(this.GetComponent<CombatUnit> ().outfittedItemsAtPurchase,this.GetComponent<CombatUnit> ());

		}


		//add listener for end turn
		gameManager.OnEndTurn.AddListener(colorEndTurnAction);

		//add listeners for tractor beam menu events
		uiManager.GetComponent<TractorBeamMenu>().OnTurnOnTractorBeamToggle.AddListener(shipPrimeTractorBeamAction);
		uiManager.GetComponent<TractorBeamMenu>().OnTurnOffTractorBeamToggle.AddListener(shipDisengageTractorBeamAction);

		//add listener to the ealy setSelectedUnit so range calculations can occur before other listeners react
		mouseManager.OnSetSelectedUnitEarly.AddListener(calculatePhaserRangeSelectedUnitAction);

		//add listener for movement while towing event
		EngineSection.OnMoveWhileTowing.AddListener(shipEngageTractorBeamAction);

		//add listener for finishing movement - we want to recalculate the ranges if we move
		EngineSection.OnMoveFinish.AddListener(shipCalculatePhaserRangeAction);

		//add listener for the onFirePhasers event
		uiManager.GetComponent<PhaserMenu>().OnFirePhasers.AddListener (phasersFirePhasersAction);

		//add listener for getting hit by phaser attack
		//CombatManager.OnPhaserAttackHitShipPhaserSection.AddListener(attackHitTakeDamageAction);
		CutsceneManager.OnPhaserHitShipPhaserSection.AddListener(attackHitTakeDamageAction);

		//add listener for getting hit by torpedo attack
		//CombatManager.OnLightTorpedoAttackHitShipPhaserSection.AddListener(attackHitTakeDamageAction);
		//CombatManager.OnHeavyTorpedoAttackHitShipPhaserSection.AddListener(attackHitTakeDamageAction);
		CutsceneManager.OnLightTorpedoHitShipPhaserSection.AddListener(attackHitTakeDamageAction);
		CutsceneManager.OnHeavyTorpedoHitShipPhaserSection.AddListener(attackHitTakeDamageAction);

		//add listener for getting healed by a crystal
		CombatManager.OnCrystalUsedOnShipPhaserSection.AddListener(crystalUsedHealDamageAction);

		//add listener for getting repaired by a repair crew
		CombatManager.OnRepairCrewUsedOnShipPhaserSection.AddListener(repairCrewRepairAction);

		//add listener for the combat unit being destroyed
		Ship.OnShipDestroyed.AddListener(combatUnitCombatUnitDestroyedAction);

		//add listener for base being destroyed to recalculate range
		Starbase.OnBaseDestroyed.AddListener(combatUnitCalculatePhaserRangeAction);

		//add listener for ship being destroyed to recalculate range
		Ship.OnShipDestroyed.AddListener(combatUnitCalculatePhaserRangeAction);

		//add listener for purchasing items
		uiManager.GetComponent<PurchaseManager>().OnPurchaseItems.AddListener(purchaseAddPurchaseItemsAction);

		//add listener for sunburst damage
		Sunburst.OnSunburstDamageDealt.AddListener(incidentalTakeDamageAction);

		//add listener for creating unit from load
		CombatUnit.OnCreateLoadedUnit.AddListener(saveDataResolveLoadedUnitAction);

	}

	//this function gets the tractor beam ready to use
	private void PrimeTractorBeam(Ship ship){

		//check if the ship passed is this ship
		if (ship == this.GetComponent<Ship> ()) {

			//check to see if we've already used phasers this turn
			if (usedPhasersThisTurn == false) {

				//Debug.Log ("PRIME");

				//set tractor beam bool
				tractorBeamIsPrimed = true;

				//invoke the OnPrimeTractorBeam event
				OnPrimeTractorBeam.Invoke (this.GetComponent<Ship> ());

			}

		}

	}

	//this function uses the tractor beam
	private void EngageTractorBeam(Ship ship){

		//check if the ship passed is this ship
		if (ship == this.GetComponent<Ship> ()) {

			//check to see if we've already used phasers this turn
			if (usedPhasersThisTurn == false) {

				//Debug.Log ("ENGAGE");

				//set used flag to true
				usedPhasersThisTurn = true;

				//set tractor beam bool
				tractorBeamIsEngaged = true;

				//invoke the OnDisengageTractorBeam event
				OnEngageTractorBeam.Invoke (this.GetComponent<Ship> ());

			}

		}

	}

	//function to turn off the tractor beam
	private void DisengageTractorBeam(Ship ship){

		//check if the ship passed is this ship
		if (ship == this.GetComponent<Ship> ()) {

			//set tractor beam bool
			tractorBeamIsPrimed = false;
			tractorBeamIsEngaged = false;

			//invoke the OnDisengageTractorBeam event
			OnDisengageTractorBeam.Invoke (this.GetComponent<Ship> ());

		}

	}

	//this function will calculate all hexes a ship can reach in range
	private void CalculatePhaserRange(GameObject gameObject){

		//check if the gameObject passed to the function is a ship
		if (gameObject.GetComponent<Ship> () == true) {

			//if it is a ship, check if it is this ship
			if (gameObject.GetComponent<Ship> () == this.GetComponent<Ship>()) {

				//get a list of all hex locations that are reachable
				TargetableTractorBeamHexes = tileMap.TargetableTiles (this.GetComponent<CombatUnit>().currentLocation,tractorBeamRange, false);
				TargetablePhaserHexes = tileMap.TargetableTiles (this.GetComponent<CombatUnit>().currentLocation,phaserRange, false);

				//create a list of hexes to exclude from targeting
				List<Hex> HexesToRemoveFromTractorBeamTargeting = new List<Hex>();

				//check the targetable tractor beam hexes and remove any that are occupied by cloaked combat units
				foreach (Hex hex in TargetableTractorBeamHexes) {

					//check if the tile has a cloaked unit
					if (tileMap.HexMap [hex].tileCombatUnit != null && tileMap.HexMap [hex].tileCombatUnit.GetComponent<CloakingDevice> () == true &&
					    tileMap.HexMap [hex].tileCombatUnit.GetComponent<CloakingDevice> ().isCloaked == true) {

						//if it does, remove it from the targetable hexes
						HexesToRemoveFromTractorBeamTargeting.Add (hex);

					}
					//we also want to check if the targetable hexes contain a starbase, which cannot be targeted with a tractor beam
					else if (tileMap.HexMap [hex].tileCombatUnit != null && tileMap.HexMap [hex].tileCombatUnit.GetComponent<Starbase> () == true) {

						//if it does, remove it from the targetable hexes
						HexesToRemoveFromTractorBeamTargeting.Add (hex);

					}
					//we also want to remove any hexes that contain non-teammate combat units.  We can only use the tractor beam on friendly units
					else if (tileMap.HexMap [hex].tileCombatUnit != null &&
					        gameManager.UnitsAreTeammates (this.GetComponent<CombatUnit> (), tileMap.HexMap [hex].tileCombatUnit) == false) {

						//if it does, remove it from the targetable hexes
						HexesToRemoveFromTractorBeamTargeting.Add (hex);

					}

				}

				//remove the hexes from the targetable hexes
				foreach (Hex hex in HexesToRemoveFromTractorBeamTargeting) {

					TargetableTractorBeamHexes.Remove (hex);

				}

			}

		}

	}

	//this function will be called when phasers are fired
	private void FirePhasers(CombatUnit firingUnit, CombatUnit targetedUnit, string sectionTargeted){

		//check if this ship is the firing ship
		if (this.GetComponent<CombatUnit> () == firingUnit) {

			//set usedPhasers flag to true
			this.usedPhasersThisTurn = true;

			//check if the phaser radar shot item was being used
			if (uiManager.GetComponent<PhaserMenu> ().usePhaserRadarShotToggle.isOn == true) {

				//decrement the phaser radar shot inventory
				phaserRadarShot -= 1;

			}

			//invoke the static attack event
			OnFirePhasers.Invoke(firingUnit,targetedUnit,sectionTargeted);

		}

	}

	//this function will handle taking damage if the section is hit by an attack
	private void TakeDamage(CombatUnit targetedUnit, int damage){

		//first, check if the unit that the phaser section is attached to is the targeted unit that got hit
		if (this.GetComponent<CombatUnit> () == targetedUnit) {

			//this is the targeted unit - we can reduce the shields by the damage
			this.shieldsCurrent -= damage;

			//check if this puts shields at or below zero - if so, set to zero and call DestroySection
			if (shieldsCurrent <= 0) {

				shieldsCurrent = 0;
				this.DestroySection ();

			}

			OnPhaserDamageTaken.Invoke ();
				
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

			OnPhaserDamageTaken.Invoke ();

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

			//invoke the repaired section event
			OnPhaserSectionRepaired.Invoke(this.GetComponent<CombatUnit>());

		}

	}

	//this function handles the section taking lethal damage and being destroyed
	private void DestroySection (){

		//if the section is destroyed, all the inventory on this section is removed
		this.phaserRadarShot = 0;
		this.xRayKernalUpgrade = false;
		this.phaserRadarArray = false;
		this.tractorBeam = false;

		//set the isDestroyed flag to true
		this.isDestroyed = true;

		//invoke the destroyed section event
		OnPhaserSectionDestroyed.Invoke(this.GetComponent<CombatUnit>());

	}

	//this function will clean up variables at end of turn
	private void EndTurn(Player.Color currentTurn){

		//check if the color passed to the function matches the owner color
		//if so, our turn is ending, and we need to reset stuff
		if (currentTurn == this.GetComponent<Ship>().owner.color) {

			this.usedPhasersThisTurn = false;

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

			//check if the purchased items have phaser items
			if (purchasedItems.ContainsKey ("Phaser Radar Shot")) {

				//increase the quantity by the value
				this.phaserRadarShot += purchasedItems ["Phaser Radar Shot"];

			}

			if (purchasedItems.ContainsKey ("Phaser Radar Array")) {

				//increase the quantity by the value
				this.phaserRadarArray = true;

			}

			if (purchasedItems.ContainsKey ("X-Ray Kernel Upgrade")) {

				//increase the quantity by the value
				this.xRayKernalUpgrade = true;

			}

			if (purchasedItems.ContainsKey ("Tractor Beam")) {

				//increase the quantity by the value
				this.tractorBeam = true;

			}

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

						this.isDestroyed = saveGameData.scoutPhaserSectionIsDestroyed [i, combatUnit.serialNumber];
						this.shieldsCurrent = saveGameData.scoutPhaserSectionShieldsCurrent [i, combatUnit.serialNumber];
						this.usedPhasersThisTurn = saveGameData.scoutUsedPhasersThisTurn [i, combatUnit.serialNumber];
						this.phaserRadarShot = saveGameData.scoutPhaserRadarShot [i, combatUnit.serialNumber];
						this.phaserRadarArray = saveGameData.scoutPhaserRadarArray [i, combatUnit.serialNumber];
						this.xRayKernalUpgrade = saveGameData.scoutXRayKernel [i, combatUnit.serialNumber];
						this.tractorBeam = saveGameData.scoutTractorBeam [i, combatUnit.serialNumber];

						break;

					case CombatUnit.UnitType.BirdOfPrey:

						this.isDestroyed = saveGameData.birdOfPreyPhaserSectionIsDestroyed [i, combatUnit.serialNumber];
						this.shieldsCurrent = saveGameData.birdOfPreyPhaserSectionShieldsCurrent [i, combatUnit.serialNumber];
						this.usedPhasersThisTurn = saveGameData.birdOfPreyUsedPhasersThisTurn [i, combatUnit.serialNumber];
						this.phaserRadarShot = saveGameData.birdOfPreyPhaserRadarShot [i, combatUnit.serialNumber];
						this.phaserRadarArray = saveGameData.birdOfPreyPhaserRadarArray [i, combatUnit.serialNumber];
						this.xRayKernalUpgrade = saveGameData.birdOfPreyXRayKernel [i, combatUnit.serialNumber];
						this.tractorBeam = saveGameData.birdOfPreyTractorBeam [i, combatUnit.serialNumber];

						break;

					case CombatUnit.UnitType.Destroyer:

						this.isDestroyed = saveGameData.destroyerPhaserSectionIsDestroyed [i, combatUnit.serialNumber];
						this.shieldsCurrent = saveGameData.destroyerPhaserSectionShieldsCurrent [i, combatUnit.serialNumber];
						this.usedPhasersThisTurn = saveGameData.destroyerUsedPhasersThisTurn [i, combatUnit.serialNumber];
						this.phaserRadarShot = saveGameData.destroyerPhaserRadarShot [i, combatUnit.serialNumber];
						this.phaserRadarArray = saveGameData.destroyerPhaserRadarArray [i, combatUnit.serialNumber];
						this.xRayKernalUpgrade = saveGameData.destroyerXRayKernel [i, combatUnit.serialNumber];
						this.tractorBeam = saveGameData.destroyerTractorBeam [i, combatUnit.serialNumber];

						break;

					case CombatUnit.UnitType.Starship:

						this.isDestroyed = saveGameData.starshipPhaserSectionIsDestroyed [i, combatUnit.serialNumber];
						this.shieldsCurrent = saveGameData.starshipPhaserSectionShieldsCurrent [i, combatUnit.serialNumber];
						this.usedPhasersThisTurn = saveGameData.starshipUsedPhasersThisTurn [i, combatUnit.serialNumber];
						this.phaserRadarShot = saveGameData.starshipPhaserRadarShot [i, combatUnit.serialNumber];
						this.phaserRadarArray = saveGameData.starshipPhaserRadarArray [i, combatUnit.serialNumber];
						this.xRayKernalUpgrade = saveGameData.starshipXRayKernel [i, combatUnit.serialNumber];
						this.tractorBeam = saveGameData.starshipTractorBeam [i, combatUnit.serialNumber];

						break;

					default:

						break;

					}

				}

			}

		}

	}

	//this function handles OnDestroy
	private void OnDestroy(){

		RemoveAllListeners ();

	}

	//this function removes all listeners
	private void RemoveAllListeners(){

		if (gameManager != null) {
			
			//remove listener for end turn
			gameManager.OnEndTurn.RemoveListener (colorEndTurnAction);

		}

		if (uiManager != null) {
			
			//remove listeners for tractor beam menu events
			uiManager.GetComponent<TractorBeamMenu> ().OnTurnOnTractorBeamToggle.RemoveListener (shipPrimeTractorBeamAction);
			uiManager.GetComponent<TractorBeamMenu> ().OnTurnOffTractorBeamToggle.RemoveListener (shipDisengageTractorBeamAction);

			//remove listener for the onFirePhasers event
			uiManager.GetComponent<PhaserMenu>().OnFirePhasers.RemoveListener (phasersFirePhasersAction);

			//remove listener for purchasing items
			uiManager.GetComponent<PurchaseManager>().OnPurchaseItems.RemoveListener(purchaseAddPurchaseItemsAction);

		}

		if (mouseManager != null) {

			//remove listener to the ealy setSelectedUnit so range calculations can occur before other listeners react
			mouseManager.OnSetSelectedUnitEarly.RemoveListener (calculatePhaserRangeSelectedUnitAction);

		}

		//remove listener for movement while towing event
		EngineSection.OnMoveWhileTowing.RemoveListener(shipEngageTractorBeamAction);

		//remove listener for finishing movement - we want to recalculate the ranges if we move
		EngineSection.OnMoveFinish.RemoveListener(shipCalculatePhaserRangeAction);

		//remove listener for getting hit by phaser attack
		//CombatManager.OnPhaserAttackHitShipPhaserSection.RemoveListener(attackHitTakeDamageAction);
		CutsceneManager.OnPhaserHitShipPhaserSection.RemoveListener(attackHitTakeDamageAction);

		//remove listener for getting hit by torpedo attack
		//CombatManager.OnLightTorpedoAttackHitShipPhaserSection.RemoveListener(attackHitTakeDamageAction);
		//CombatManager.OnHeavyTorpedoAttackHitShipPhaserSection.RemoveListener(attackHitTakeDamageAction);
		CutsceneManager.OnLightTorpedoHitShipPhaserSection.RemoveListener(attackHitTakeDamageAction);
		CutsceneManager.OnHeavyTorpedoHitShipPhaserSection.RemoveListener(attackHitTakeDamageAction);

		//remove listener for getting healed by a crystal
		CombatManager.OnCrystalUsedOnShipPhaserSection.RemoveListener(crystalUsedHealDamageAction);

		//remove listener for getting repaired by a repair crew
		CombatManager.OnRepairCrewUsedOnShipPhaserSection.RemoveListener(repairCrewRepairAction);

		//remove listener for the combat unit being destroyed
		Ship.OnShipDestroyed.RemoveListener(combatUnitCombatUnitDestroyedAction);

		//remove listener for base being destroyed to recalculate range
		Starbase.OnBaseDestroyed.RemoveListener(combatUnitCalculatePhaserRangeAction);

		//add listener for ship being destroyed to recalculate range
		Ship.OnShipDestroyed.RemoveListener(combatUnitCalculatePhaserRangeAction);

		//remove listener for sunburst damage
		Sunburst.OnSunburstDamageDealt.RemoveListener(incidentalTakeDamageAction);

		//remove listener for creating unit from load
		CombatUnit.OnCreateLoadedUnit.RemoveListener(saveDataResolveLoadedUnitAction);

	}

}
