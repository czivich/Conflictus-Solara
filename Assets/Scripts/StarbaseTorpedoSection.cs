using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class StarbaseTorpedoSection : MonoBehaviour {

	//variables for the managers
	private UIManager uiManager;
	private GameManager gameManager;
	private MouseManager mouseManager;

	//variable to hold the reference to the tileMap
	private TileMap tileMap;

	//define the variables that the torpedo section must manage
	public int torpedoLaserShot {

		get;
		private set;

	}

	public bool torpedoLaserGuidanceSystem {

		get;
		private set;

	}

	public bool highPressureTubes {

		get;
		private set;

	}

	public int lightTorpedos {

		get;
		private set;

	}

	public int heavyTorpedos {

		get;
		private set;

	}

	//variable bools to track whether torpedos have been used this turn
	public bool usedTorpedosThisTurn {

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

	//define a variable to control torpedo attack range
	public int torpedoRange {

		get;
		private set;

	}

	//default values for torpedo ranges
	private const int defaultTorpedoRange = 2;
	private const int highPressureTorpedoRange = 3;

	//this is the list of hexes that the torpedo targeting can reach
	public List<Hex> TargetableTorpedoHexes {
		get;
		private set;
	}

	//events to handle torpedo attacks
	public static TorpedoAttackEvent OnFireLightTorpedo = new TorpedoAttackEvent();
	public static TorpedoAttackEvent OnFireHeavyTorpedo = new TorpedoAttackEvent();

	//simple class so I can have my event pass the combat units to the event
	public class TorpedoAttackEvent : UnityEvent<CombatUnit,CombatUnit,string>{};


	//event to announce the section was destroyed
	public static TorpedoSectionDestroyedEvent OnTorpedoSectionDestroyed = new TorpedoSectionDestroyedEvent();

	//event to announce the section was repaired
	public static TorpedoSectionDestroyedEvent OnTorpedoSectionRepaired = new TorpedoSectionDestroyedEvent();

	//class for event to announce section was destroyed
	public class TorpedoSectionDestroyedEvent : UnityEvent<CombatUnit>{};

	//event to announce damage was taken
	public static UnityEvent OnTorpedoDamageTaken = new UnityEvent();

	//unityActions
	private UnityAction<Player.Color> colorEndTurnAction;
	private UnityAction earlyCalculateTorpedoRangeAction;
	private UnityAction<CombatUnit,CombatUnit,string> torpedoAttackFireLightTorpedoAction;
	private UnityAction<CombatUnit,CombatUnit,string> torpedoAttackFireHeavyTorpedoAction;
	private UnityAction<CombatUnit,CombatUnit,int> attackHitTakeDamageAction;
	private UnityAction<CombatUnit,CombatUnit,CombatManager.CrystalType,int> crystalUsedHealDamageAction;
	private UnityAction<CombatUnit,CombatUnit> repairCrewUsedOnThisRepairSectionAction;
	private UnityAction<CombatUnit> combatUnitCombatUnitDestroyedAction;
	private UnityAction<CombatUnit> combatUnitCalculateTorpedoRangeAction;
	private UnityAction<CombatUnit,FileManager.SaveGameData> saveDataResolveLoadedUnitAction;

	// Use this for initialization
	public void Init () {

		//get the managers
		uiManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
		gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
		mouseManager = GameObject.FindGameObjectWithTag("MouseManager").GetComponent<MouseManager>();

		//find tileMap in the game
		tileMap = GameObject.FindGameObjectWithTag ("TileMap").GetComponent<TileMap> ();

		//set the starting inventory
		this.torpedoLaserShot = Starbase.startingTorpedoLaserShot;
		this.torpedoLaserGuidanceSystem = Starbase.startingTorpedoLaserGuidanceSystem;
		this.highPressureTubes = Starbase.startingHighPressureTubes;
		this.lightTorpedos = Starbase.startingLightTorpedos;
		this.heavyTorpedos = Starbase.startingHeavyTorpedos;
		this.shieldsMax = Starbase.torpedoSectionShieldsMax;
		this.shieldsCurrent = shieldsMax;

		//set the actions
		colorEndTurnAction = (color) => {EndTurn(color);};
		earlyCalculateTorpedoRangeAction = () => {CalculateTorpedoRange(mouseManager.selectedUnit);};
		torpedoAttackFireLightTorpedoAction = (attackingUnit,targetedUnit,sectionTargeted) => {FireLightTorpedo(attackingUnit,targetedUnit,sectionTargeted);};
		torpedoAttackFireHeavyTorpedoAction = (attackingUnit,targetedUnit,sectionTargeted) => {FireHeavyTorpedo(attackingUnit,targetedUnit,sectionTargeted);};
		attackHitTakeDamageAction = (attackingUnit, targetedUnit, damage) => {TakeDamage(targetedUnit,damage);};
		crystalUsedHealDamageAction = (selectedUnit,targetedUnit,crystalType,shieldsHealed) => {HealDamage(targetedUnit,shieldsHealed);};
		repairCrewUsedOnThisRepairSectionAction = (selectedUnit,targetedUnit) => {RepairSection(targetedUnit);};
		combatUnitCombatUnitDestroyedAction = (combatUnit) => {CombatUnitDestroyed(combatUnit);};
		combatUnitCalculateTorpedoRangeAction = (combatUnit) => {

			//check to make sure the selected unit is not null
			if(mouseManager.selectedUnit != null){

				CalculateTorpedoRange(mouseManager.selectedUnit);
			}

		};

		saveDataResolveLoadedUnitAction = (combatUnit,saveGameData) => {ResolveLoadedUnit(combatUnit,saveGameData);};

		//add listener for end turn
		gameManager.OnEndTurn.AddListener(colorEndTurnAction);

		//add listener to the ealy setSelectedUnit so range calculations can occur before other listeners react
		mouseManager.OnSetSelectedUnitEarly.AddListener(earlyCalculateTorpedoRangeAction);

		//add listener for the onFireTorpedo events
		uiManager.GetComponent<TorpedoMenu>().OnFireLightTorpedo.AddListener (torpedoAttackFireLightTorpedoAction);
		uiManager.GetComponent<TorpedoMenu>().OnFireHeavyTorpedo.AddListener (torpedoAttackFireHeavyTorpedoAction);

		//add listener for getting hit by phasor attack
		//CombatManager.OnPhasorAttackHitBaseTorpedoSection.AddListener(attackHitTakeDamageAction);
		CutsceneManager.OnPhasorHitBaseTorpedoSection.AddListener(attackHitTakeDamageAction);

		//add listener for getting hit by torpedo attack
		//CombatManager.OnLightTorpedoAttackHitBaseTorpedoSection.AddListener(attackHitTakeDamageAction);
		//CombatManager.OnHeavyTorpedoAttackHitBaseTorpedoSection.AddListener(attackHitTakeDamageAction);
		CutsceneManager.OnLightTorpedoHitBaseTorpedoSection.AddListener(attackHitTakeDamageAction);
		CutsceneManager.OnHeavyTorpedoHitBaseTorpedoSection.AddListener(attackHitTakeDamageAction);

		//add listener for getting healed by a crystal
		CombatManager.OnCrystalUsedOnBaseTorpedoSection.AddListener(crystalUsedHealDamageAction);

		//add listener for getting repaired by a repair crew
		CombatManager.OnRepairCrewUsedOnBaseTorpedoSection.AddListener(repairCrewUsedOnThisRepairSectionAction);

		//add listener for the combat unit being destroyed
		Starbase.OnBaseDestroyed.AddListener(combatUnitCombatUnitDestroyedAction);

		//add listener for base being destroyed to recalculate range
		Starbase.OnBaseDestroyed.AddListener(combatUnitCalculateTorpedoRangeAction);

		//add listener for ship being destroyed to recalculate range
		Ship.OnShipDestroyed.AddListener(combatUnitCalculateTorpedoRangeAction);

		//add listener for creating unit from load
		CombatUnit.OnCreateLoadedUnit.AddListener(saveDataResolveLoadedUnitAction);

	}

	//this function will calculate all hexes the starbase can reach in range
	private void CalculateTorpedoRange(GameObject gameObject){

		//check if the gameObject passed to the function is a starbase
		if (gameObject.GetComponent<Starbase> () == true) {

			//if it is a starbase, check if it is this starbase
			if (gameObject.GetComponent<Starbase> () == this.GetComponent<Starbase>()) {

				//determine torpedo range based on whether this base has high pressure tubes
				if (this.highPressureTubes == true) {

					this.torpedoRange = highPressureTorpedoRange;

				} else {

					this.torpedoRange = defaultTorpedoRange;

				}


				//get a list of all hex locations that are reachable
				TargetableTorpedoHexes = tileMap.TargetableTiles (this.GetComponent<CombatUnit>().currentLocation,torpedoRange, false);

			}

		}

	}

	//this function will be called when a light torpedo is fired
	private void FireLightTorpedo(CombatUnit firingUnit, CombatUnit targetedUnit, string sectionTargeted){

		//check if this ship is the firing unit
		if (this.GetComponent<CombatUnit> () == firingUnit) {

			//set usedTorpedo flag to true
			this.usedTorpedosThisTurn = true;

			//check if the torpedo laser shot item was being used
			if (uiManager.GetComponent<TorpedoMenu> ().useTorpedoLaserShotToggle.isOn == true) {

				//decrement the laser shot inventory by 1
				this.torpedoLaserShot -= 1;

			}

			//decrement the light torpedo inventory
			this.lightTorpedos -= 1;


			//invoke the static attack event
			OnFireLightTorpedo.Invoke(firingUnit,targetedUnit,sectionTargeted);

		}

	}

	//this function will be called when a heavy torpedo is fired
	private void FireHeavyTorpedo(CombatUnit firingUnit, CombatUnit targetedUnit, string sectionTargeted){

		//check if this ship is the firing unit
		if (this.GetComponent<CombatUnit> () == firingUnit) {

			//set usedTorpedo flag to true
			this.usedTorpedosThisTurn = true;

			//check if the torpedo laser shot item was being used
			if (uiManager.GetComponent<TorpedoMenu> ().useTorpedoLaserShotToggle.isOn == true) {

				//decrement the laser shot inventory by 1
				torpedoLaserShot -= 1;

			}

			//decrement the heavy torpedo inventory
			this.heavyTorpedos -= 1;

			//invoke the static attack event
			OnFireHeavyTorpedo.Invoke(firingUnit,targetedUnit,sectionTargeted);

		}

	}


	//this function will handle taking damage if the section is hit by an attack
	private void TakeDamage(CombatUnit targetedUnit, int damage){

		//first, check if the unit that the torpedo section is attached to is the targeted unit that got hit
		if (this.GetComponent<CombatUnit> () == targetedUnit) {

			//this is the targeted unit - we can reduce the shields by the damage
			this.shieldsCurrent -= damage;

			//check if this puts shields at or below zero - if so, set to zero and call DestroySection
			if (shieldsCurrent <= 0) {

				shieldsCurrent = 0;
				this.DestroySection ();

			}

			OnTorpedoDamageTaken.Invoke ();

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

			OnTorpedoDamageTaken.Invoke ();

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
			OnTorpedoSectionRepaired.Invoke(this.GetComponent<CombatUnit>());

		}

	}

	//this function handles the section taking lethal damage and being destroyed
	private void DestroySection (){

		//if the section is destroyed, all the inventory on this section is removed
		this.torpedoLaserShot = 0;
		this.torpedoLaserGuidanceSystem = false;
		this.highPressureTubes = false;
		this.lightTorpedos = 0;
		this.heavyTorpedos = 0;

		//set the isDestroyed flag to true
		this.isDestroyed = true;

		//invoke the destroyed section event
		OnTorpedoSectionDestroyed.Invoke(this.GetComponent<CombatUnit>());

	}

	//this function will clean up variables at end of turn
	private void EndTurn(Player.Color currentTurn){

		//check if the color passed to the function matches the owner color
		//if so, our turn is ending, and we need to reset stuff
		if (currentTurn == this.GetComponent<CombatUnit>().owner.color) {

			this.usedTorpedosThisTurn = false;

		}

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
					this.isDestroyed = saveGameData.starbaseTorpedoSectionIsDestroyed [i];
					this.shieldsCurrent = saveGameData.starbaseTorpedoSectionShieldsCurrent [i];
					this.torpedoLaserShot = saveGameData.starbaseTorpedoLaserShot [i];
					this.torpedoLaserGuidanceSystem = saveGameData.starbaseLaserGuidanceSystem [i];
					this.highPressureTubes = saveGameData.starbaseHighPressureTubes [i];
					this.lightTorpedos = saveGameData.starbaseLightTorpedos [i];
					this.heavyTorpedos = saveGameData.starbaseHeavyTorpedos [i];
					this.usedTorpedosThisTurn = saveGameData.starbaseUsedTorpedosThisTurn [i];

				}

			}

		}

	}

	//function to handle OnDestroy
	private void OnDestroy(){

		RemoveAllListeners ();

	}

	//this function removes all listeners
	private void RemoveAllListeners(){

		//remove listener for end turn
		if (gameManager != null) {
			
			gameManager.OnEndTurn.RemoveListener (colorEndTurnAction);

		}

		//remove listener to the ealy setSelectedUnit so range calculations can occur before other listeners react
		if (mouseManager != null) {
			
			mouseManager.OnSetSelectedUnitEarly.RemoveListener (earlyCalculateTorpedoRangeAction);

		}

		//remove listener for the onFireTorpedo events
		if (uiManager != null) {
			
			uiManager.GetComponent<TorpedoMenu> ().OnFireLightTorpedo.RemoveListener (torpedoAttackFireLightTorpedoAction);
			uiManager.GetComponent<TorpedoMenu> ().OnFireHeavyTorpedo.RemoveListener (torpedoAttackFireHeavyTorpedoAction);

		}

		//remove listener for getting hit by phasor attack
		//CombatManager.OnPhasorAttackHitBaseTorpedoSection.RemoveListener(attackHitTakeDamageAction);
		CutsceneManager.OnPhasorHitBaseTorpedoSection.RemoveListener(attackHitTakeDamageAction);

		//remove listener for getting hit by torpedo attack
		//CombatManager.OnLightTorpedoAttackHitBaseTorpedoSection.RemoveListener(attackHitTakeDamageAction);
		//CombatManager.OnHeavyTorpedoAttackHitBaseTorpedoSection.RemoveListener(attackHitTakeDamageAction);
		CutsceneManager.OnLightTorpedoHitBaseTorpedoSection.RemoveListener(attackHitTakeDamageAction);
		CutsceneManager.OnHeavyTorpedoHitBaseTorpedoSection.RemoveListener(attackHitTakeDamageAction);

		//remove listener for getting healed by a crystal
		CombatManager.OnCrystalUsedOnBaseTorpedoSection.RemoveListener(crystalUsedHealDamageAction);

		//remove listener for getting repaired by a repair crew
		CombatManager.OnRepairCrewUsedOnBaseTorpedoSection.RemoveListener(repairCrewUsedOnThisRepairSectionAction);

		//remove listener for the combat unit being destroyed
		Starbase.OnBaseDestroyed.RemoveListener(combatUnitCombatUnitDestroyedAction);

		//remove listener for base being destroyed to recalculate range
		Starbase.OnBaseDestroyed.RemoveListener(combatUnitCalculateTorpedoRangeAction);

		//remove listener for ship being destroyed to recalculate range
		Ship.OnShipDestroyed.RemoveListener(combatUnitCalculateTorpedoRangeAction);

		//remove listener for creating unit from load
		CombatUnit.OnCreateLoadedUnit.RemoveListener(saveDataResolveLoadedUnitAction);

	}

}
