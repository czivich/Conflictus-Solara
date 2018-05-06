using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class StarbasePhaserSection1 : MonoBehaviour {
	
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

	public bool phaserRadarArray {

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


	//define a variable to control phaser attack range
	public int phaserRange {

		get;
		private set;

	}

	//this is the list of hexes that the phaser targeting can reach
	public List<Hex> TargetablePhaserHexes {
		get;
		private set;
	}


	//events to handle phaser attacks
	public static PhaserAttackEvent OnFirePhasers = new PhaserAttackEvent();

	//simple class so I can have my event pass the combat units to the event
	public class PhaserAttackEvent : UnityEvent<CombatUnit,CombatUnit,string>{};


	//event to announce the section was destroyed
	public static PhaserSectionDestroyedEvent OnPhaserSection1Destroyed = new PhaserSectionDestroyedEvent();

	//event to announce the section was repaired
	public static PhaserSectionDestroyedEvent OnPhaserSection1Repaired = new PhaserSectionDestroyedEvent();

	//class for event to announce section was destroyed
	public class PhaserSectionDestroyedEvent : UnityEvent<CombatUnit>{};

	//event to announce damage was taken
	public static UnityEvent OnPhaserDamageTaken = new UnityEvent();

	//unityActions
	private UnityAction<Player.Color> colorEndTurnAction;
	private UnityAction earlyCalculatePhaserRangeAction;
	private UnityAction<CombatUnit,CombatUnit,string> phaserAttackFirePhasersAction;
	private UnityAction<CombatUnit,CombatUnit,int> attackHitTakeDamageAction;
	private UnityAction<CombatUnit,CombatUnit,CombatManager.CrystalType,int> crystalUsedHealDamageAction;
	private UnityAction<CombatUnit,CombatUnit> repairCrewUsedOnThisRepairSectionAction;
	private UnityAction<CombatUnit> combatUnitCombatUnitDestroyedAction;
	private UnityAction<CombatUnit> combatUnitCalculatePhaserRangeAction;
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

		phaserRange = 2;

		//set the starting inventory
		this.phaserRadarShot = Starbase.startingPhaserRadarShot;
		this.phaserRadarArray = Starbase.startingPhaserRadarArray;
		this.shieldsMax = Starbase.phaserSection1ShieldsMax;
		this.shieldsCurrent = shieldsMax;

		//set the actions
		colorEndTurnAction = (color) => {EndTurn(color);};
		earlyCalculatePhaserRangeAction = () => {CalculatePhaserRange(mouseManager.selectedUnit);};
		phaserAttackFirePhasersAction = (attackingUnit,targetedUnit,sectionTargeted) => {FirePhasers(attackingUnit,targetedUnit,sectionTargeted);};
		attackHitTakeDamageAction = (attackingUnit, targetedUnit, damage) => {TakeDamage(targetedUnit,damage);};
		crystalUsedHealDamageAction = (selectedUnit,targetedUnit,crystalType,shieldsHealed) => {HealDamage(targetedUnit,shieldsHealed);};
		repairCrewUsedOnThisRepairSectionAction = (selectedUnit,targetedUnit) => {RepairSection(targetedUnit);};
		combatUnitCombatUnitDestroyedAction = (combatUnit) => {CombatUnitDestroyed(combatUnit);};
		combatUnitCalculatePhaserRangeAction = (combatUnit) => {

			//check to make sure the selected unit is not null
			if(mouseManager.selectedUnit != null){

				CalculatePhaserRange(mouseManager.selectedUnit);
			}

		};

		saveDataResolveLoadedUnitAction = (combatUnit,saveGameData) => {ResolveLoadedUnit(combatUnit,saveGameData);};

		//add listener for end turn
		gameManager.OnEndTurn.AddListener(colorEndTurnAction);

		//add listener to the ealy setSelectedUnit so range calculations can occur before other listeners react
		mouseManager.OnSetSelectedUnitEarly.AddListener(earlyCalculatePhaserRangeAction);

		//add listener for the onFirePhasers event
		uiManager.GetComponent<PhaserMenu>().OnFirePhasers.AddListener (phaserAttackFirePhasersAction);

		//add listener for getting hit by phaser attack
		//CombatManager.OnPhaserAttackHitBasePhaserSection1.AddListener(attackHitTakeDamageAction);
		CutsceneManager.OnPhaserHitBasePhaserSection1.AddListener(attackHitTakeDamageAction);

		//add listener for getting hit by torpedo attack
		//CombatManager.OnLightTorpedoAttackHitBasePhaserSection1.AddListener(attackHitTakeDamageAction);
		//CombatManager.OnHeavyTorpedoAttackHitBasePhaserSection1.AddListener(attackHitTakeDamageAction);
		CutsceneManager.OnLightTorpedoHitBasePhaserSection1.AddListener(attackHitTakeDamageAction);
		CutsceneManager.OnHeavyTorpedoHitBasePhaserSection1.AddListener(attackHitTakeDamageAction);

		//add listener for getting healed by a crystal
		CombatManager.OnCrystalUsedOnBasePhaserSection1.AddListener(crystalUsedHealDamageAction);

		//add listener for getting repaired by a repair crew
		CombatManager.OnRepairCrewUsedOnBasePhaserSection1.AddListener(repairCrewUsedOnThisRepairSectionAction);

		//add listener for the combat unit being destroyed
		Starbase.OnBaseDestroyed.AddListener(combatUnitCombatUnitDestroyedAction);

		//add listener for base being destroyed to recalculate range
		Starbase.OnBaseDestroyed.AddListener(combatUnitCalculatePhaserRangeAction);

		//add listener for ship being destroyed to recalculate range
		Ship.OnShipDestroyed.AddListener(combatUnitCalculatePhaserRangeAction);

		//add listener for creating unit from load
		CombatUnit.OnCreateLoadedUnit.AddListener(saveDataResolveLoadedUnitAction);

	}

	//this function will calculate all hexes the base can reach in range
	private void CalculatePhaserRange(GameObject gameObject){

		//check if the gameObject passed to the function is a starbase
		if (gameObject.GetComponent<Starbase> () == true) {

			//if it is a starbase, check if it is this starbase
			if (gameObject.GetComponent<Starbase> () == this.GetComponent<Starbase>()) {

				//get a list of all hex locations that are reachable
				TargetablePhaserHexes = tileMap.TargetableTiles (this.GetComponent<CombatUnit>().currentLocation,phaserRange, false);

			}

		}

	}

	//this function will be called when phasers are fired
	private void FirePhasers(CombatUnit firingUnit, CombatUnit targetedUnit, string sectionTargeted){

		//check if this combat unit is the firing combat unit
		if (this.GetComponent<CombatUnit> () == firingUnit) {

			//set usedPhasers flag to true
			this.usedPhasersThisTurn = true;

			//check if the phaser radar shot item was being used
			if (uiManager.GetComponent<PhaserMenu> ().usePhaserRadarShotToggle.isOn == true) {

				//decrement the phaser radar shot inventory
				phaserRadarShot -= 1;

			}

			//check if this section is destroyed.  If it is, don't invoke the OnFire Event
			if (this.isDestroyed == false) {

				//invoke the static attack event
				OnFirePhasers.Invoke (firingUnit, targetedUnit, sectionTargeted);

			}

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
			OnPhaserSection1Repaired.Invoke(this.GetComponent<CombatUnit>());

		}

	}

	//this function handles the section taking lethal damage and being destroyed
	private void DestroySection (){

		//if the section is destroyed, all the inventory on this section is removed
		this.phaserRadarShot = 0;
		this.phaserRadarArray = false;

		//set the isDestroyed flag to true
		this.isDestroyed = true;

		//invoke the destroyed section event
		OnPhaserSection1Destroyed.Invoke(this.GetComponent<CombatUnit>());

	}

	//this function will clean up variables at end of turn
	private void EndTurn(Player.Color currentTurn){

		//check if the color passed to the function matches the owner color
		//if so, our turn is ending, and we need to reset stuff
		if (currentTurn == this.GetComponent<CombatUnit>().owner.color) {

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
					this.isDestroyed = saveGameData.starbasePhaserSection1IsDestroyed [i];
					this.shieldsCurrent = saveGameData.starbasePhaserSection1ShieldsCurrent [i];
					this.usedPhasersThisTurn = saveGameData.starbasePhaserSection1UsedPhasersThisTurn [i];
					this.phaserRadarShot = saveGameData.starbasePhaserRadarShot [i];
					this.phaserRadarArray = saveGameData.starbasePhaserRadarArray [i];

				}

			}

		}

	}

	//this function handles OnDestroy()
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
			
			mouseManager.OnSetSelectedUnitEarly.RemoveListener (earlyCalculatePhaserRangeAction);

		}

		//remove listener for the onFirePhasers event
		if (uiManager != null) {
			
			uiManager.GetComponent<PhaserMenu> ().OnFirePhasers.RemoveListener (phaserAttackFirePhasersAction);

		}

		//remove listener for getting hit by phaser attack
		//CombatManager.OnPhaserAttackHitBasePhaserSection1.RemoveListener (attackHitTakeDamageAction);
		CutsceneManager.OnPhaserHitBasePhaserSection1.RemoveListener(attackHitTakeDamageAction);

		//remove listener for getting hit by torpedo attack
		//CombatManager.OnLightTorpedoAttackHitBasePhaserSection1.RemoveListener (attackHitTakeDamageAction);
		//CombatManager.OnHeavyTorpedoAttackHitBasePhaserSection1.RemoveListener (attackHitTakeDamageAction);
		CutsceneManager.OnLightTorpedoHitBasePhaserSection1.RemoveListener(attackHitTakeDamageAction);
		CutsceneManager.OnHeavyTorpedoHitBasePhaserSection1.RemoveListener(attackHitTakeDamageAction);

		//remove listener for getting healed by a crystal
		CombatManager.OnCrystalUsedOnBasePhaserSection1.RemoveListener (crystalUsedHealDamageAction);

		//remove listener for getting repaired by a repair crew
		CombatManager.OnRepairCrewUsedOnBasePhaserSection1.RemoveListener (repairCrewUsedOnThisRepairSectionAction);

		//remove listener for the combat unit being destroyed
		Starbase.OnBaseDestroyed.RemoveListener (combatUnitCombatUnitDestroyedAction);

		//remove listener for base being destroyed to recalculate range
		Starbase.OnBaseDestroyed.RemoveListener (combatUnitCalculatePhaserRangeAction);

		//remove listener for ship being destroyed to recalculate range
		Ship.OnShipDestroyed.RemoveListener (combatUnitCalculatePhaserRangeAction);

		//remove listener for creating unit from load
		CombatUnit.OnCreateLoadedUnit.RemoveListener (saveDataResolveLoadedUnitAction);

	}

}
