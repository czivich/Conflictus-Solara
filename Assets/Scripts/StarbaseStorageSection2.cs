using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class StarbaseStorageSection2 : MonoBehaviour {
	
	//variables for the managers
	private UIManager uiManager;
	private MouseManager mouseManager;

	//variable to hold the reference to the tileMap
	private TileMap tileMap;

	//variables that the storage section must manage
	public int trilithiumCrystals {

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
	public static StorageSectionDestroyedEvent OnStorageSection2Destroyed = new StorageSectionDestroyedEvent();

	//event to announce the section was repaired
	public static StorageSectionDestroyedEvent OnStorageSection2Repaired = new StorageSectionDestroyedEvent();

	//event to announce trilithium crystal was used
	public static UseCrystalEvent OnUseTrilithiumCrystal = new UseCrystalEvent();


	//class for event to announce section was destroyed
	public class StorageSectionDestroyedEvent : UnityEvent<CombatUnit>{};

	//class for passing data to crystal events
	public class UseCrystalEvent : UnityEvent<CombatUnit,CombatUnit,string>{};

	//event to announce damage was taken
	public static UnityEvent OnStorageDamageTaken = new UnityEvent();

	//unityActions
	private UnityAction<CombatUnit,CombatUnit,int> attackHitTakeDamageAction;
	private UnityAction<FlareManager.FlareEventData> flareDataUseFlaresAction;
	private UnityAction<CombatUnit> combatUnitSetFlareModeToManualAction;
	private UnityAction<CombatUnit> combatUnitSetFlareModeToAutoAction;
	private UnityAction<CombatUnit,CombatUnit,string> useCrystalUseTrilithiumCrystalAction;
	private UnityAction<CombatUnit,CombatUnit,CombatManager.CrystalType,int> crystalUsedHealDamageAction;
	private UnityAction earlyCalculateItemRangeAction;
	private UnityAction<CombatUnit,CombatUnit> repairCrewUsedOnThisRepairSectionAction;
	private UnityAction<CombatUnit> combatUnitCombatUnitDestroyedAction;
	private UnityAction<CombatUnit> combatUnitCalculatePhasorangeAction;
	private UnityAction<CombatUnit,FileManager.SaveGameData> saveDataResolveLoadedUnitAction;

	// Use this for initialization
	public void Init () {

		//get the managers
		uiManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
		mouseManager = GameObject.FindGameObjectWithTag("MouseManager").GetComponent<MouseManager>();

		//find tileMap in the game
		tileMap = GameObject.FindGameObjectWithTag ("TileMap").GetComponent<TileMap> ();

		//set the default FlareMode
		this.flareMode = FlareMode.Manual;

		//set the default item range
		itemRange = 1;

		//set the starting inventory
		this.trilithiumCrystals = Starbase.startingTrilithiumCrystals;
		this.laserScatteringSystem = Starbase.startingLaserScatteringSystem;
		this.flares = Starbase.startingFlares;
		this.shieldsMax = Starbase.storageSection2ShieldsMax;
		this.shieldsCurrent = shieldsMax;

		//set the actions
		attackHitTakeDamageAction = (attackingUnit, targetedUnit, damage) => {TakeDamage(targetedUnit,damage);};
		flareDataUseFlaresAction = (flareEventData) => {UseFlares(flareEventData.targetedUnit,flareEventData.numberFlaresUsed);};
		combatUnitSetFlareModeToManualAction = (selectedUnit) => {SetFlareModeToManual(selectedUnit);};
		combatUnitSetFlareModeToAutoAction = (selectedUnit) => {SetFlareModeToAuto (selectedUnit);};
		useCrystalUseTrilithiumCrystalAction = (selectedUnit, targetedUnit, sectionTargetedString) => {UseTrilithiumCrystal (selectedUnit, targetedUnit, sectionTargetedString);};
		crystalUsedHealDamageAction = (selectedUnit,targetedUnit,crystalType,shieldsHealed) => {HealDamage(targetedUnit,shieldsHealed);};
		earlyCalculateItemRangeAction = () => {CalculateItemRange(mouseManager.selectedUnit);};
		repairCrewUsedOnThisRepairSectionAction = (selectedUnit,targetedUnit) => {RepairSection(targetedUnit);};
		combatUnitCombatUnitDestroyedAction = (combatUnit) => {CombatUnitDestroyed(combatUnit);};
		saveDataResolveLoadedUnitAction = (combatUnit,saveGameData) => {ResolveLoadedUnit(combatUnit,saveGameData);};

		//add listener for getting hit by phasor attack
		//CombatManager.OnPhasorAttackHitBaseStorageSection2.AddListener(attackHitTakeDamageAction);
		CutsceneManager.OnPhasorHitBaseStorageSection2.AddListener(attackHitTakeDamageAction);

		//add listener for getting hit by torpedo attack
		//CombatManager.OnLightTorpedoAttackHitBaseStorageSection2.AddListener(attackHitTakeDamageAction);
		//CombatManager.OnHeavyTorpedoAttackHitBaseStorageSection2.AddListener(attackHitTakeDamageAction);
		CutsceneManager.OnLightTorpedoHitBaseStorageSection2.AddListener(attackHitTakeDamageAction);
		CutsceneManager.OnHeavyTorpedoHitBaseStorageSection2.AddListener(attackHitTakeDamageAction);

		//add listener for using flares
		uiManager.GetComponent<FlareManager>().OnUseFlaresYes.AddListener(flareDataUseFlaresAction);

		//add listeners for setting flare mode
		uiManager.GetComponent<UseItemMenu>().OnSetFlareModeToManual.AddListener(combatUnitSetFlareModeToManualAction);
		uiManager.GetComponent<UseItemMenu>().OnSetFlareModeToAuto.AddListener(combatUnitSetFlareModeToAutoAction);

		//add listeners for using crystals
		uiManager.GetComponent<UseItemMenu> ().OnUseTrilithiumCrystal.AddListener (useCrystalUseTrilithiumCrystalAction);

		//add listener for getting healed by a crystal
		CombatManager.OnCrystalUsedOnBaseStorageSection2.AddListener(crystalUsedHealDamageAction);

		//add listener to the ealy setSelectedUnit so range calculations can occur before other listeners react
		mouseManager.OnSetSelectedUnitEarly.AddListener(earlyCalculateItemRangeAction);

		//add listener for getting repaired by a repair crew
		CombatManager.OnRepairCrewUsedOnBaseStorageSection2.AddListener(repairCrewUsedOnThisRepairSectionAction);

		//add listener for the combat unit being destroyed
		Starbase.OnBaseDestroyed.AddListener(combatUnitCombatUnitDestroyedAction);

		//add listener for creating unit from load
		CombatUnit.OnCreateLoadedUnit.AddListener(saveDataResolveLoadedUnitAction);

	}

	//this function is called when the starbase uses flares
	private void UseFlares(CombatUnit targetedUnit, int numberFlaresUsed){

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

	//this function will calculate all hexes a starbase can reach in range
	private void CalculateItemRange(GameObject gameObject){

		//check if the gameObject passed to the function is a starbase
		if (gameObject.GetComponent<Starbase> () == true) {

			//if it is a starbase, check if it is this starbase
			if (gameObject.GetComponent<Starbase> () == this.GetComponent<Starbase>()) {

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
			OnStorageSection2Repaired.Invoke(this.GetComponent<CombatUnit>());

		}

	}

	//this function handles the section taking lethal damage and being destroyed
	private void DestroySection (){

		//if the section is destroyed, all the inventory on this section is removed
		this.trilithiumCrystals = 0;
		this.laserScatteringSystem = false;
		this.flares = 0;

		//set the isDestroyed flag to true
		this.isDestroyed = true;

		//invoke the destroyed section event
		OnStorageSection2Destroyed.Invoke(this.GetComponent<CombatUnit>());

	}

	//this function will remove all listeners when the combat unit is destroyed
	private void CombatUnitDestroyed(CombatUnit combatUnitDestroyed){

		//check if the passed combat unit is this combat unit
		if (this.GetComponent<CombatUnit> () == combatUnitDestroyed) {

			RemoveAllListeners ();

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
					//now we can set the name
					this.isDestroyed = saveGameData.starbaseStorageSection2IsDestroyed [i];
					this.shieldsCurrent = saveGameData.starbaseStorageSection2ShieldsCurrent [i];
					this.trilithiumCrystals = saveGameData.starbaseTriilithiumCrystals [i];
					this.flares = saveGameData.starbaseFlares [i];
					this.laserScatteringSystem = saveGameData.starbaseLaserScatteringSystem [i];
					this.flareMode = saveGameData.starbaseFlareMode [i];

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

		//remove listener for getting hit by phasor attack
		//CombatManager.OnPhasorAttackHitBaseStorageSection2.RemoveListener(attackHitTakeDamageAction);
		CutsceneManager.OnPhasorHitBaseStorageSection2.RemoveListener(attackHitTakeDamageAction);

		//remove listener for getting hit by torpedo attack
		//CombatManager.OnLightTorpedoAttackHitBaseStorageSection2.RemoveListener(attackHitTakeDamageAction);
		//CombatManager.OnHeavyTorpedoAttackHitBaseStorageSection2.RemoveListener(attackHitTakeDamageAction);
		CutsceneManager.OnLightTorpedoHitBaseStorageSection2.RemoveListener(attackHitTakeDamageAction);
		CutsceneManager.OnHeavyTorpedoHitBaseStorageSection2.RemoveListener(attackHitTakeDamageAction);

		//remove listener for using flares
		if (uiManager != null) {
			
			uiManager.GetComponent<FlareManager> ().OnUseFlaresYes.RemoveListener (flareDataUseFlaresAction);

			//remove listeners for setting flare mode
			uiManager.GetComponent<UseItemMenu> ().OnSetFlareModeToManual.RemoveListener (combatUnitSetFlareModeToManualAction);
			uiManager.GetComponent<UseItemMenu> ().OnSetFlareModeToAuto.RemoveListener (combatUnitSetFlareModeToAutoAction);

			//remove listeners for using crystals
			uiManager.GetComponent<UseItemMenu> ().OnUseTrilithiumCrystal.RemoveListener (useCrystalUseTrilithiumCrystalAction);

		}

		//remove listener for getting healed by a crystal
		CombatManager.OnCrystalUsedOnBaseStorageSection2.RemoveListener(crystalUsedHealDamageAction);

		//remove listener to the ealy setSelectedUnit so range calculations can occur before other listeners react
		if (mouseManager != null) {
			
			mouseManager.OnSetSelectedUnitEarly.RemoveListener (earlyCalculateItemRangeAction);

		}

		//remove listener for getting repaired by a repair crew
		CombatManager.OnRepairCrewUsedOnBaseStorageSection2.RemoveListener(repairCrewUsedOnThisRepairSectionAction);

		//remove listener for the combat unit being destroyed
		Starbase.OnBaseDestroyed.RemoveListener(combatUnitCombatUnitDestroyedAction);

		//remove listener for creating unit from load
		CombatUnit.OnCreateLoadedUnit.RemoveListener(saveDataResolveLoadedUnitAction);

	}

}
