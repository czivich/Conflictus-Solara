using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class StarbasePhasorSection2 : MonoBehaviour {
	
	//variables for the managers
	private UIManager uiManager;
	private GameManager gameManager;
	private MouseManager mouseManager;

	//variable to hold the reference to the tileMap
	private TileMap tileMap;

	//define the variables that the phasor section must manage

	public bool xRayKernalUpgrade {

		get;
		private set;

	}


	//variable bools to track whether phasors have been used this turn
	public bool usedPhasorsThisTurn {

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

	//define a variable to control phasor attack range
	public int phasorRange {

		get;
		private set;

	}

	//this is the list of hexes that the phasor targeting can reach
	public List<Hex> TargetablePhasorHexes {
		get;
		private set;
	}

	//events to handle phasor attacks
	public static PhasorAttackEvent OnFirePhasors = new PhasorAttackEvent();

	//simple class so I can have my event pass the combat units to the event
	public class PhasorAttackEvent : UnityEvent<CombatUnit,CombatUnit,string>{};


	//event to announce the section was destroyed
	public static PhasorSectionDestroyedEvent OnPhasorSection2Destroyed = new PhasorSectionDestroyedEvent();

	//event to announce the section was repaired
	public static PhasorSectionDestroyedEvent OnPhasorSection2Repaired = new PhasorSectionDestroyedEvent();

	//class for event to announce section was destroyed
	public class PhasorSectionDestroyedEvent : UnityEvent<CombatUnit>{};

	//event to announce damage was taken
	public static UnityEvent OnPhasorDamageTaken = new UnityEvent();

	//unityActions
	private UnityAction<Player.Color> colorEndTurnAction;
	private UnityAction earlyCalculatePhasorRangeAction;
	private UnityAction<CombatUnit,CombatUnit,string> phasorAttackFirePhasorsAction;
	private UnityAction<CombatUnit,CombatUnit,int> attackHitTakeDamageAction;
	private UnityAction<CombatUnit,CombatUnit,CombatManager.CrystalType,int> crystalUsedHealDamageAction;
	private UnityAction<CombatUnit,CombatUnit> repairCrewUsedOnThisRepairSectionAction;
	private UnityAction<CombatUnit> combatUnitCombatUnitDestroyedAction;
	private UnityAction<CombatUnit> combatUnitCalculatePhasorRangeAction;
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

		//set the starting ranges
		phasorRange = 2;

		//set the starting inventory
		this.xRayKernalUpgrade = Starbase.startingXRayKernelUpgrade;
		this.shieldsMax = Starbase.phasorSection2ShieldsMax;
		this.shieldsCurrent = shieldsMax;

		//set the actions
		colorEndTurnAction = (color) => {EndTurn(color);};
		earlyCalculatePhasorRangeAction = () => {CalculatePhasorRange(mouseManager.selectedUnit);};
		phasorAttackFirePhasorsAction = (attackingUnit,targetedUnit,sectionTargeted) => {FirePhasors(attackingUnit,targetedUnit,sectionTargeted);};
		attackHitTakeDamageAction = (attackingUnit, targetedUnit, damage) => {TakeDamage(targetedUnit,damage);};
		crystalUsedHealDamageAction = (selectedUnit,targetedUnit,crystalType,shieldsHealed) => {HealDamage(targetedUnit,shieldsHealed);};
		repairCrewUsedOnThisRepairSectionAction = (selectedUnit,targetedUnit) => {RepairSection(targetedUnit);};
		combatUnitCombatUnitDestroyedAction = (combatUnit) => {CombatUnitDestroyed(combatUnit);};
		combatUnitCalculatePhasorRangeAction = (combatUnit) => {

			//check to make sure the selected unit is not null
			if(mouseManager.selectedUnit != null){

				CalculatePhasorRange(mouseManager.selectedUnit);
			}

		};

		saveDataResolveLoadedUnitAction = (combatUnit,saveGameData) => {ResolveLoadedUnit(combatUnit,saveGameData);};

		//add listener for end turn
		gameManager.OnEndTurn.AddListener(colorEndTurnAction);

		//add listener to the ealy setSelectedUnit so range calculations can occur before other listeners react
		mouseManager.OnSetSelectedUnitEarly.AddListener(earlyCalculatePhasorRangeAction);

		//add listener for the onFirePhasors event
		uiManager.GetComponent<PhasorMenu>().OnFirePhasors.AddListener (phasorAttackFirePhasorsAction);

		//add listener for getting hit by phasor attack
		//CombatManager.OnPhasorAttackHitBasePhasorSection2.AddListener(attackHitTakeDamageAction);
		CutsceneManager.OnPhasorHitBasePhasorSection2.AddListener(attackHitTakeDamageAction);

		//add listener for getting hit by torpedo attack
		//CombatManager.OnLightTorpedoAttackHitBasePhasorSection2.AddListener(attackHitTakeDamageAction);
		//CombatManager.OnHeavyTorpedoAttackHitBasePhasorSection2.AddListener(attackHitTakeDamageAction);
		CutsceneManager.OnLightTorpedoHitBasePhasorSection2.AddListener(attackHitTakeDamageAction);
		CutsceneManager.OnHeavyTorpedoHitBasePhasorSection2.AddListener(attackHitTakeDamageAction);

		//add listener for getting healed by a crystal
		CombatManager.OnCrystalUsedOnBasePhasorSection2.AddListener(crystalUsedHealDamageAction);

		//add listener for getting repaired by a repair crew
		CombatManager.OnRepairCrewUsedOnBasePhasorSection2.AddListener(repairCrewUsedOnThisRepairSectionAction);

		//add listener for the combat unit being destroyed
		Starbase.OnBaseDestroyed.AddListener(combatUnitCombatUnitDestroyedAction);

		//add listener for base being destroyed to recalculate range
		Starbase.OnBaseDestroyed.AddListener(combatUnitCalculatePhasorRangeAction);

		//add listener for ship being destroyed to recalculate range
		Ship.OnShipDestroyed.AddListener(combatUnitCalculatePhasorRangeAction);

		//add listener for creating unit from load
		CombatUnit.OnCreateLoadedUnit.AddListener(saveDataResolveLoadedUnitAction);

	}

	//this function will calculate all hexes the starbase can reach in range
	private void CalculatePhasorRange(GameObject gameObject){

		//check if the gameObject passed to the function is a starbase
		if (gameObject.GetComponent<Starbase> () == true) {

			//if it is a starbase, check if it is this starbase
			if (gameObject.GetComponent<Starbase> () == this.GetComponent<Starbase>()) {

				//get a list of all hex locations that are reachable
				TargetablePhasorHexes = tileMap.TargetableTiles (this.GetComponent<CombatUnit>().currentLocation,phasorRange, false);

			}

		}

	}

	//this function will be called when phasors are fired
	private void FirePhasors(CombatUnit firingUnit, CombatUnit targetedUnit, string sectionTargeted){

		//check if this starbase is the firing starbase
		if (this.GetComponent<CombatUnit> () == firingUnit) {

			//set usedPhasors flag to true
			this.usedPhasorsThisTurn = true;

			//check if the PhasorSection1 is still alive.  If it is, we will have that section invoke the event - we don't want to pass two OnFire events
			if (gameObject.GetComponent<StarbasePhasorSection1> ().isDestroyed == true) {
			
				//invoke the static attack event
				OnFirePhasors.Invoke (firingUnit, targetedUnit, sectionTargeted);
			
			}
		}

	}

	//this function will handle taking damage if the section is hit by an attack
	private void TakeDamage(CombatUnit targetedUnit, int damage){

		//first, check if the unit that the phasor section is attached to is the targeted unit that got hit
		if (this.GetComponent<CombatUnit> () == targetedUnit) {

			//this is the targeted unit - we can reduce the shields by the damage
			this.shieldsCurrent -= damage;

			//check if this puts shields at or below zero - if so, set to zero and call DestroySection
			if (shieldsCurrent <= 0) {

				shieldsCurrent = 0;
				this.DestroySection ();

			}

			OnPhasorDamageTaken.Invoke ();

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

			OnPhasorDamageTaken.Invoke ();

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
			OnPhasorSection2Repaired.Invoke(this.GetComponent<CombatUnit>());

		}

	}

	//this function handles the section taking lethal damage and being destroyed
	private void DestroySection (){

		//if the section is destroyed, all the inventory on this section is removed
		this.xRayKernalUpgrade = false;


		//set the isDestroyed flag to true
		this.isDestroyed = true;

		//invoke the destroyed section event
		OnPhasorSection2Destroyed.Invoke(this.GetComponent<CombatUnit>());

	}

	//this function will clean up variables at end of turn
	private void EndTurn(Player.Color currentTurn){

		//check if the color passed to the function matches the owner color
		//if so, our turn is ending, and we need to reset stuff
		if (currentTurn == this.GetComponent<CombatUnit>().owner.color) {

			this.usedPhasorsThisTurn = false;

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
					this.isDestroyed = saveGameData.starbasePhasorSection2IsDestroyed [i];
					this.shieldsCurrent = saveGameData.starbasePhasorSection2ShieldsCurrent [i];
					this.usedPhasorsThisTurn = saveGameData.starbasePhasorSection2UsedPhasorsThisTurn [i];
					this.xRayKernalUpgrade = saveGameData.starbaseXRayKernel [i];

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

		//remove listener for end turn
		if (gameManager != null) {
			
			gameManager.OnEndTurn.RemoveListener (colorEndTurnAction);

		}

		//remove listener to the ealy setSelectedUnit so range calculations can occur before other listeners react
		if (mouseManager != null) {
			
			mouseManager.OnSetSelectedUnitEarly.RemoveListener (earlyCalculatePhasorRangeAction);

		}

		//remove listener for the onFirePhasors event
		if (uiManager != null) {
			
			uiManager.GetComponent<PhasorMenu> ().OnFirePhasors.RemoveListener (phasorAttackFirePhasorsAction);

		}

		//remove listener for getting hit by phasor attack
		//CombatManager.OnPhasorAttackHitBasePhasorSection2.RemoveListener(attackHitTakeDamageAction);
		CutsceneManager.OnPhasorHitBasePhasorSection2.RemoveListener(attackHitTakeDamageAction);

		//remove listener for getting hit by torpedo attack
		//CombatManager.OnLightTorpedoAttackHitBasePhasorSection2.RemoveListener(attackHitTakeDamageAction);
		//CombatManager.OnHeavyTorpedoAttackHitBasePhasorSection2.RemoveListener(attackHitTakeDamageAction);
		CutsceneManager.OnLightTorpedoHitBasePhasorSection2.RemoveListener(attackHitTakeDamageAction);
		CutsceneManager.OnHeavyTorpedoHitBasePhasorSection2.RemoveListener(attackHitTakeDamageAction);

		//remove listener for getting healed by a crystal
		CombatManager.OnCrystalUsedOnBasePhasorSection2.RemoveListener(crystalUsedHealDamageAction);

		//remove listener for getting repaired by a repair crew
		CombatManager.OnRepairCrewUsedOnBasePhasorSection2.RemoveListener(repairCrewUsedOnThisRepairSectionAction);

		//remove listener for the combat unit being destroyed
		Starbase.OnBaseDestroyed.RemoveListener(combatUnitCombatUnitDestroyedAction);

		//remove listener for base being destroyed to recalculate range
		Starbase.OnBaseDestroyed.RemoveListener(combatUnitCalculatePhasorRangeAction);

		//remove listener for ship being destroyed to recalculate range
		Ship.OnShipDestroyed.RemoveListener(combatUnitCalculatePhasorRangeAction);

		//remove listener for creating unit from load
		CombatUnit.OnCreateLoadedUnit.RemoveListener(saveDataResolveLoadedUnitAction);

	}

}
