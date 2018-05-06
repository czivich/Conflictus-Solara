using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StorageSection : MonoBehaviour {

	//variables for the managers
	private UIManager uiManager;
	private MouseManager mouseManager;

	//variable to hold the reference to the tileMap
	private TileMap tileMap;

	//variables that the storage section must manage
	public int dilithiumCrystals {

		get;
		private set;

	}

	public int trilithiumCrystals {

		get;
		private set;

	}

	public bool radarJammingSystem {

		get;
		private set;

	}

	public bool laserScatteringSystem {

		get;
		private set;

	}

	public int flares {

		get;
		private set;

	}

	//enum to handle flare handling state
	public enum FlareMode{

		Auto,
		Manual,

	}

	public FlareMode flareMode {

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

	//this is the list of hexes that the item targeting can reach
	public List<Hex> TargetableItemHexes {
		get;
		private set;
	}

	//define a variable to control item use range
	public int itemRange {

		get;
		private set;

	}


	//event to announce the section was destroyed
	public static StorageSectionDestroyedEvent OnStorageSectionDestroyed = new StorageSectionDestroyedEvent();

	//event to announce the section was repaired
	public static StorageSectionDestroyedEvent OnStorageSectionRepaired = new StorageSectionDestroyedEvent();

	//event to announce dilithium crystal was used
	public static UseCrystalEvent OnUseDilithiumCrystal = new UseCrystalEvent();

	//event to announce trilithium crystal was used
	public static UseCrystalEvent OnUseTrilithiumCrystal = new UseCrystalEvent();


	//class for event to announce section was destroyed
	public class StorageSectionDestroyedEvent : UnityEvent<CombatUnit>{};

	//class for passing data to crystal events
	public class UseCrystalEvent : UnityEvent<CombatUnit,CombatUnit,string>{};

	//event to announce damage was taken
	public static UnityEvent OnStorageDamageTaken = new UnityEvent();

	//event to announce inventory updated
	public static InventoryUpdateEvent OnInventoryUpdated = new InventoryUpdateEvent();
	public class InventoryUpdateEvent : UnityEvent<CombatUnit>{};

	//UnityActions
	private UnityAction<CombatUnit,CombatUnit,int> attackHitTakeDamageAction;
	private UnityAction<FlareManager.FlareEventData> flareDataUseFlaresAction;
	private UnityAction<CombatUnit> combatUnitSetFlareModeToManualAction;
	private UnityAction<CombatUnit> combatUnitSetFlareModeToAutoAction;
	private UnityAction<CombatUnit,CombatUnit,string> useCrystalUseDilithiumCrystalAction;
	private UnityAction<CombatUnit,CombatUnit,string> useCrystalUseTrilithiumCrystalAction;
	private UnityAction<CombatUnit,CombatUnit,CombatManager.CrystalType,int> crystalUsedHealDamageAction;
	private UnityAction earlyCalculateItemRangeAction;
	private UnityAction<Ship> shipCalculateItemRangeAction;
	private UnityAction<CombatUnit,CombatUnit> repairUsedRepairSectionAction;
	private UnityAction<CombatUnit> combatUnitCombatUnitDestroyedAction;
	private UnityAction<CombatUnit> combatUnitCalculateMovementRangeSelectedUnitAction;
	private UnityAction<Dictionary<string,int>,int,CombatUnit> purchaseAddPurchaseItemsAction;
	private UnityAction<CombatUnit,int> incidentalTakeDamageAction;
	private UnityAction<CombatUnit,FileManager.SaveGameData> saveDataResolveLoadedUnitAction;

	// Use this for initialization
	public void Init () {

		//set the actions
		attackHitTakeDamageAction = (attackingUnit, targetedUnit, phaserDamage) => {TakeDamage(targetedUnit,phaserDamage);};
		flareDataUseFlaresAction = (flareEventData) => {UseFlares(flareEventData.targetedUnit,flareEventData.numberFlaresUsed);};
		combatUnitSetFlareModeToManualAction = (selectedUnit) => {SetFlareModeToManual(selectedUnit);};
		combatUnitSetFlareModeToAutoAction = (selectedUnit) => {SetFlareModeToAuto (selectedUnit);};
		useCrystalUseTrilithiumCrystalAction = (selectedUnit, targetedUnit, sectionTargetedString) => {UseTrilithiumCrystal (selectedUnit, targetedUnit, sectionTargetedString);};
		useCrystalUseDilithiumCrystalAction = (selectedUnit, targetedUnit, sectionTargetedString) => {UseDilithiumCrystal (selectedUnit, targetedUnit, sectionTargetedString);};
		crystalUsedHealDamageAction = (selectedUnit,targetedUnit,crystalType,shieldsHealed) => {HealDamage(targetedUnit,shieldsHealed);};
		earlyCalculateItemRangeAction = () => {CalculateItemRange(mouseManager.selectedUnit);};
		shipCalculateItemRangeAction = (ship) => {CalculateItemRange(ship.gameObject);};
		repairUsedRepairSectionAction = (selectedUnit,targetedUnit) => {RepairSection(targetedUnit);};
		combatUnitCombatUnitDestroyedAction = (combatUnit) => {CombatUnitDestroyed(combatUnit);};
		purchaseAddPurchaseItemsAction = (purchasedItems,purchasedValue,combatUnit) => {AddPurchasedItems(purchasedItems,combatUnit);};
		incidentalTakeDamageAction = (combatUnit, damage) => {TakeDamage(combatUnit,damage);};
		saveDataResolveLoadedUnitAction = (combatUnit,saveGameData) => {ResolveLoadedUnit(combatUnit,saveGameData);};

		//get the managers
		uiManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
		mouseManager = GameObject.FindGameObjectWithTag("MouseManager").GetComponent<MouseManager>();

		//find tileMap in the game
		tileMap = GameObject.FindGameObjectWithTag ("TileMap").GetComponent<TileMap> ();

		//set the default FlareMode
		this.flareMode = FlareMode.Manual;

		//set the default item range
		itemRange = 1;

		//set the starting inventory based on the type of ship we have
		switch (this.GetComponent<CombatUnit> ().unitType) {

		case CombatUnit.UnitType.Starship:
			this.dilithiumCrystals = Starship.startingDilithiumCrystals;
			this.trilithiumCrystals = Starship.startingTrilithiumCrystals;
			this.radarJammingSystem = Starship.startingRadarJammingSystem;
			this.laserScatteringSystem = Starship.startingLaserScatteringSystem;
			this.flares = Starship.startingFlares;
			this.shieldsMax = Starship.storageSectionShieldsMax;
			this.shieldsCurrent = shieldsMax;
			break;
		case CombatUnit.UnitType.Destroyer:
			this.dilithiumCrystals = Destroyer.startingDilithiumCrystals;
			this.trilithiumCrystals = Destroyer.startingTrilithiumCrystals;
			this.radarJammingSystem = Destroyer.startingRadarJammingSystem;
			this.laserScatteringSystem = Destroyer.startingLaserScatteringSystem;
			this.flares = Destroyer.startingFlares;
			this.shieldsMax = Destroyer.storageSectionShieldsMax;
			this.shieldsCurrent = shieldsMax;
			break;
		case CombatUnit.UnitType.BirdOfPrey:
			Debug.LogError ("A Storage Section is Attached to a Bird of Prey");
			this.dilithiumCrystals = 0;
			this.trilithiumCrystals = 0;
			this.radarJammingSystem = false;
			this.laserScatteringSystem = false;
			this.flares = 0;
			this.shieldsMax = 40;
			this.shieldsCurrent = shieldsMax;
			break;
		case CombatUnit.UnitType.Scout:
			this.dilithiumCrystals = Scout.startingDilithiumCrystals;
			this.trilithiumCrystals = Scout.startingTrilithiumCrystals;
			this.radarJammingSystem = Scout.startingRadarJammingSystem;
			this.laserScatteringSystem = Scout.startingLaserScatteringSystem;
			this.flares = Scout.startingFlares;
			this.shieldsMax = Scout.storageSectionShieldsMax;
			this.shieldsCurrent = shieldsMax;
			break;
		default:
			this.dilithiumCrystals = 0;
			this.trilithiumCrystals = 0;
			this.radarJammingSystem = false;
			this.laserScatteringSystem = false;
			this.flares = 0;
			this.shieldsMax = 40;
			this.shieldsCurrent = shieldsMax;
			break;

		}

		//add listener for getting hit by phaser attack
		//CombatManager.OnPhaserAttackHitShipStorageSection.AddListener(attackHitTakeDamageAction);
		CutsceneManager.OnPhaserHitShipStorageSection.AddListener(attackHitTakeDamageAction);

		//add listener for getting hit by torpedo attack
		//CombatManager.OnLightTorpedoAttackHitShipStorageSection.AddListener(attackHitTakeDamageAction);
		//CombatManager.OnHeavyTorpedoAttackHitShipStorageSection.AddListener(attackHitTakeDamageAction);
		CutsceneManager.OnLightTorpedoHitShipStorageSection.AddListener(attackHitTakeDamageAction);
		CutsceneManager.OnHeavyTorpedoHitShipStorageSection.AddListener(attackHitTakeDamageAction);

		//add listener for using flares
		uiManager.GetComponent<FlareManager>().OnUseFlaresYes.AddListener(flareDataUseFlaresAction);

		//add listeners for setting flare mode
		uiManager.GetComponent<UseItemMenu>().OnSetFlareModeToManual.AddListener(combatUnitSetFlareModeToManualAction);
		uiManager.GetComponent<UseItemMenu>().OnSetFlareModeToAuto.AddListener(combatUnitSetFlareModeToAutoAction);

		//add listeners for using crystals
		uiManager.GetComponent<UseItemMenu> ().OnUseDilithiumCrystal.AddListener (useCrystalUseDilithiumCrystalAction);

		uiManager.GetComponent<UseItemMenu> ().OnUseTrilithiumCrystal.AddListener (useCrystalUseTrilithiumCrystalAction);

		//add listener for getting healed by a crystal
		CombatManager.OnCrystalUsedOnShipStorageSection.AddListener(crystalUsedHealDamageAction);

		//add listener to the ealy setSelectedUnit so range calculations can occur before other listeners react
		mouseManager.OnSetSelectedUnitEarly.AddListener(earlyCalculateItemRangeAction);

		//add listener for finishing movement - we want to recalculate the ranges if we move
		EngineSection.OnMoveFinish.AddListener(shipCalculateItemRangeAction);

		//add listener for getting repaired by a repair crew
		CombatManager.OnRepairCrewUsedOnShipStorageSection.AddListener(repairUsedRepairSectionAction);

		//add listener for the combat unit being destroyed
		Ship.OnShipDestroyed.AddListener(combatUnitCombatUnitDestroyedAction);

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

	}

	//this function is called when the ship uses flares
	private void UseFlares(CombatUnit targetedUnit, int numberFlaresUsed){

		//Debug.Log ("Use FLares called");

		//first, check if the unit that the storage section is attached to is the targeted unit that got hit
		if (this.GetComponent<CombatUnit> () == targetedUnit) {

			//check if the section is not already destroyed

			if (this.isDestroyed == false) {
				//decrement the flares on board
				this.flares -= numberFlaresUsed;

				//check if somehow we have negative flares, set to zero
				if (this.flares < 0) {

					this.flares = 0;

					Debug.LogError ("Somehow we called UseFlares to use more than we had in inventory");

				}

			}

		}

	}

	//this function is called to set the flareMode to manual
	private void SetFlareModeToManual(CombatUnit selectedUnit){

		//check if this is the selected unit
		if (this.GetComponent<CombatUnit> () == selectedUnit) {

			this.flareMode = FlareMode.Manual;

		}

	}

	//this function is called to set the flareMode to auto
	private void SetFlareModeToAuto(CombatUnit selectedUnit){

		//check if this is the selected unit
		if (this.GetComponent<CombatUnit> () == selectedUnit) {

			this.flareMode = FlareMode.Auto;

		}

	}

	//this function will calculate all hexes a ship can reach in range
	private void CalculateItemRange(GameObject gameObject){

		//check if the gameObject passed to the function is a ship
		if (gameObject.GetComponent<Ship> () == true) {

			//if it is a ship, check if it is this ship
			if (gameObject.GetComponent<Ship> () == this.GetComponent<Ship>()) {

				//get a list of all hex locations that are reachable
				//set last argument to true so it includes the hex that the ship is on
				TargetableItemHexes = tileMap.TargetableTiles (this.GetComponent<CombatUnit>().currentLocation,itemRange, true);

				//create a list of hexes to exclude from targeting
				List<Hex> HexesToRemoveFromItemTargeting = new List<Hex>();

				//check the targetable tractor beam hexes and remove any that are occupied by cloaked combat units
				foreach (Hex hex in TargetableItemHexes) {

					//check if the tile has a cloaked unit
					if (tileMap.HexMap [hex].tileCombatUnit != null && tileMap.HexMap [hex].tileCombatUnit.GetComponent<CloakingDevice>() == true &&
						tileMap.HexMap [hex].tileCombatUnit.GetComponent<CloakingDevice>().isCloaked == true) {

						//if it does, remove it from the targetable hexes
						HexesToRemoveFromItemTargeting.Add(hex);

					}

				}

				//remove the hexes from the targetable hexes
				foreach (Hex hex in HexesToRemoveFromItemTargeting) {

					TargetableItemHexes.Remove (hex);

				}

			}

		}

	}

	//this function is called to use a Dilithium crystal
	private void UseDilithiumCrystal(CombatUnit selectedUnit, CombatUnit targetedUnit, string sectionTargetedString){

		//first, check if this is the selected unit
		if (this.GetComponent<CombatUnit> () == selectedUnit) {

			//this is the selected unit - we can reduce the crystal inventory
			this.dilithiumCrystals -= 1;

			//invoke the useCrystal event
			OnUseDilithiumCrystal.Invoke(selectedUnit,targetedUnit,sectionTargetedString);

		}

	}

	//this function is called to use a Trilithium crystal
	private void UseTrilithiumCrystal(CombatUnit selectedUnit, CombatUnit targetedUnit, string sectionTargetedString){

		//first, check if this is the selected unit
		if (this.GetComponent<CombatUnit> () == selectedUnit) {

			//this is the selected unit - we can reduce the crystal inventory
			this.trilithiumCrystals -= 1;

			//invoke the useCrystal event
			OnUseTrilithiumCrystal.Invoke(selectedUnit,targetedUnit,sectionTargetedString);

		}

	}


	//this function will handle taking damage if the section is hit by an attack
	private void TakeDamage(CombatUnit targetedUnit, int damage){

		//Debug.Log ("Take damage called");

		//first, check if the unit that the storage section is attached to is the targeted unit that got hit
		if (this.GetComponent<CombatUnit> () == targetedUnit) {

			//this is the targeted unit - we can reduce the shields by the damage
			this.shieldsCurrent -= damage;

			//check if this puts shields at or below zero - if so, set to zero and call DestroySection
			if (shieldsCurrent <= 0) {

				shieldsCurrent = 0;
				this.DestroySection ();

			}

			OnStorageDamageTaken.Invoke ();


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

			OnStorageDamageTaken.Invoke ();

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
			OnStorageSectionRepaired.Invoke(this.GetComponent<CombatUnit>());

		}

	}

	//this function handles the section taking lethal damage and being destroyed
	private void DestroySection (){

		//if the section is destroyed, all the inventory on this section is removed
		this.dilithiumCrystals = 0;
		this.trilithiumCrystals = 0;
		this.radarJammingSystem = false;
		this.laserScatteringSystem = false;
		this.flares = 0;

		//set the isDestroyed flag to true
		this.isDestroyed = true;

		//invoke the destroyed section event
		OnStorageSectionDestroyed.Invoke(this.GetComponent<CombatUnit>());

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

			//check if the purchased items have storage items
			if (purchasedItems.ContainsKey ("Dilithium Crystal")) {

				//increase the quantity by the value
				this.dilithiumCrystals += purchasedItems ["Dilithium Crystal"];

			}

			if (purchasedItems.ContainsKey ("Trilithium Crystal")) {

				//increase the quantity by the value
				this.trilithiumCrystals += purchasedItems ["Trilithium Crystal"];

			}

			if (purchasedItems.ContainsKey ("Flare")) {

				//increase the quantity by the value
				this.flares += purchasedItems ["Flare"];

			}

			if (purchasedItems.ContainsKey ("Radar Jamming System")) {

				//increase the quantity by the value
				this.radarJammingSystem = true;

			}

			if (purchasedItems.ContainsKey ("Laser Scattering System")) {

				//increase the quantity by the value
				this.laserScatteringSystem = true;

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

						this.isDestroyed = saveGameData.scoutStorageSectionIsDestroyed [i, combatUnit.serialNumber];
						this.shieldsCurrent = saveGameData.scoutStorageSectionShieldsCurrent [i, combatUnit.serialNumber];
						this.dilithiumCrystals = saveGameData.scoutDilithiumCrystals [i, combatUnit.serialNumber];
						this.trilithiumCrystals = saveGameData.scoutTriilithiumCrystals [i, combatUnit.serialNumber];
						this.flares = saveGameData.scoutFlares [i, combatUnit.serialNumber];
						this.radarJammingSystem = saveGameData.scoutRadarJammingSystem [i, combatUnit.serialNumber];
						this.laserScatteringSystem = saveGameData.scoutLaserScatteringSystem [i, combatUnit.serialNumber];
						this.flareMode = saveGameData.scoutFlareMode [i, combatUnit.serialNumber];

						break;

					case CombatUnit.UnitType.Destroyer:

						this.isDestroyed = saveGameData.destroyerStorageSectionIsDestroyed [i, combatUnit.serialNumber];
						this.shieldsCurrent = saveGameData.destroyerStorageSectionShieldsCurrent [i, combatUnit.serialNumber];
						this.dilithiumCrystals = saveGameData.destroyerDilithiumCrystals [i, combatUnit.serialNumber];
						this.trilithiumCrystals = saveGameData.destroyerTriilithiumCrystals [i, combatUnit.serialNumber];
						this.flares = saveGameData.destroyerFlares [i, combatUnit.serialNumber];
						this.radarJammingSystem = saveGameData.destroyerRadarJammingSystem [i, combatUnit.serialNumber];
						this.laserScatteringSystem = saveGameData.destroyerLaserScatteringSystem [i, combatUnit.serialNumber];
						this.flareMode = saveGameData.destroyerFlareMode [i, combatUnit.serialNumber];

						break;

					case CombatUnit.UnitType.Starship:

						this.isDestroyed = saveGameData.starshipStorageSectionIsDestroyed [i, combatUnit.serialNumber];
						this.shieldsCurrent = saveGameData.starshipStorageSectionShieldsCurrent [i, combatUnit.serialNumber];
						this.dilithiumCrystals = saveGameData.starshipDilithiumCrystals [i, combatUnit.serialNumber];
						this.trilithiumCrystals = saveGameData.starshipTriilithiumCrystals [i, combatUnit.serialNumber];
						this.flares = saveGameData.starshipFlares [i, combatUnit.serialNumber];
						this.radarJammingSystem = saveGameData.starshipRadarJammingSystem [i, combatUnit.serialNumber];
						this.laserScatteringSystem = saveGameData.starshipLaserScatteringSystem [i, combatUnit.serialNumber];
						this.flareMode = saveGameData.starshipFlareMode [i, combatUnit.serialNumber];

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

		//remove listener for getting hit by phaser attack
		//CombatManager.OnPhaserAttackHitShipStorageSection.RemoveListener (attackHitTakeDamageAction);
		CutsceneManager.OnPhaserHitShipStorageSection.RemoveListener(attackHitTakeDamageAction);

		//remove listener for getting hit by torpedo attack
		//CombatManager.OnLightTorpedoAttackHitShipStorageSection.RemoveListener (attackHitTakeDamageAction);
		//CombatManager.OnHeavyTorpedoAttackHitShipStorageSection.RemoveListener (attackHitTakeDamageAction);
		CutsceneManager.OnLightTorpedoHitShipStorageSection.RemoveListener(attackHitTakeDamageAction);
		CutsceneManager.OnHeavyTorpedoHitShipStorageSection.RemoveListener(attackHitTakeDamageAction);

		if (uiManager != null) {
			
			//remove listener for using flares
			uiManager.GetComponent<FlareManager> ().OnUseFlaresYes.RemoveListener (flareDataUseFlaresAction);

			//remove listeners for setting flare mode
			uiManager.GetComponent<UseItemMenu> ().OnSetFlareModeToManual.RemoveListener (combatUnitSetFlareModeToManualAction);
			uiManager.GetComponent<UseItemMenu> ().OnSetFlareModeToAuto.RemoveListener (combatUnitSetFlareModeToAutoAction);

			//remove listeners for using crystals
			uiManager.GetComponent<UseItemMenu> ().OnUseDilithiumCrystal.RemoveListener (useCrystalUseDilithiumCrystalAction);

			uiManager.GetComponent<UseItemMenu> ().OnUseTrilithiumCrystal.RemoveListener (useCrystalUseTrilithiumCrystalAction);

			//remove listener for purchasing items
			uiManager.GetComponent<PurchaseManager> ().OnPurchaseItems.RemoveListener (purchaseAddPurchaseItemsAction);

		}

		//remove listener for getting healed by a crystal
		CombatManager.OnCrystalUsedOnShipStorageSection.RemoveListener (crystalUsedHealDamageAction);

		if (mouseManager != null) {
			
			//remove listener to the ealy setSelectedUnit so range calculations can occur before other listeners react
			mouseManager.OnSetSelectedUnitEarly.RemoveListener (earlyCalculateItemRangeAction);

		}

		//remove listener for finishing movement - we want to recalculate the ranges if we move
		EngineSection.OnMoveFinish.RemoveListener (shipCalculateItemRangeAction);

		//remove listener for getting repaired by a repair crew
		CombatManager.OnRepairCrewUsedOnShipStorageSection.RemoveListener (repairUsedRepairSectionAction);

		//remove listener for the combat unit being destroyed
		Ship.OnShipDestroyed.RemoveListener (combatUnitCombatUnitDestroyedAction);

		//remove listener for sunburst damage
		Sunburst.OnSunburstDamageDealt.RemoveListener (incidentalTakeDamageAction);

		//remove listener for creating unit from load
		CombatUnit.OnCreateLoadedUnit.RemoveListener (saveDataResolveLoadedUnitAction);

	}

}
