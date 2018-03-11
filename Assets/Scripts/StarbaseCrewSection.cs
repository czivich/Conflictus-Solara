using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class StarbaseCrewSection : MonoBehaviour {
	
	//variables for the managers
	private UIManager uiManager;
	private GameManager gameManager;
	private MouseManager mouseManager;

	//variable to hold the reference to the tileMap
	private TileMap tileMap;

	//variables that the crew section must manage
	public bool repairCrew {

		get;
		private set;

	}

	public bool shieldEngineeringTeam {

		get;
		private set;

	}

	public bool battleCrew {

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

	//variable to hold whether the repair crew has already been used this turn
	public bool usedRepairCrewThisTurn {

		get;
		private set;

	}

	//this is the list of hexes that the crew can reach
	public List<Hex> TargetableCrewHexes {
		get;
		private set;
	}

	//define a variable to control crew range
	public int crewRange {

		get;
		private set;

	}

	//event to announce the section was destroyed
	public static CrewSectionDestroyedEvent OnCrewSectionDestroyed = new CrewSectionDestroyedEvent();

	//event to announce the section was repaired
	public static CrewSectionDestroyedEvent OnCrewSectionRepaired = new CrewSectionDestroyedEvent();

	//event to announce repair crew was used
	public static UseRepairCrewEvent OnUseRepairCrew = new UseRepairCrewEvent();

	//class for event to announce section was destroyed
	public class CrewSectionDestroyedEvent : UnityEvent<CombatUnit>{};

	//class for passing data to crystal events
	public class UseRepairCrewEvent : UnityEvent<CombatUnit,CombatUnit,string>{};

	//event to announce damage was taken
	public static UnityEvent OnCrewDamageTaken = new UnityEvent();

	//unityActions
	private UnityAction<Player.Color> colorEndTurnAction;
	private UnityAction<CombatUnit,CombatUnit,int> attackHitTakeDamageAction;
	private UnityAction<CombatUnit,CombatUnit,CombatManager.CrystalType,int> crystalUsedHealDamageAction;
	private UnityAction<CombatUnit,CombatUnit,string> repairCrewUsedRepairSectionAction;
	private UnityAction earlyCalculateCrewRangeAction;
	private UnityAction<CombatUnit,CombatUnit> repairCrewUsedOnThisRepairSectionAction;
	private UnityAction<CombatUnit> combatUnitCombatUnitDestroyedAction;
	private UnityAction<CombatUnit,FileManager.SaveGameData> saveDataResolveLoadedUnitAction;


	// Use this for initialization
	public void Init () {

		//get the managers
		uiManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
		gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
		mouseManager = GameObject.FindGameObjectWithTag("MouseManager").GetComponent<MouseManager>();

		//find tileMap in the game
		tileMap = GameObject.FindGameObjectWithTag ("TileMap").GetComponent<TileMap> ();

		//set used repair crew to false
		this.usedRepairCrewThisTurn = false;

		//set the starting inventory
		this.repairCrew = Starbase.startingRepairCrew;
		this.shieldEngineeringTeam = Starbase.startingShieldEngineeringTeam;
		this.battleCrew = Starbase.startingBattleCrew;
		this.shieldsMax = Starbase.crewSectionShieldsMax;
		this.shieldsCurrent = shieldsMax;

		//set the actions
		colorEndTurnAction = (color) => {EndTurn(color);};
		attackHitTakeDamageAction = (attackingUnit, targetedUnit, phasorDamage) => {TakeDamage(targetedUnit,phasorDamage);};
		crystalUsedHealDamageAction = (selectedUnit,targetedUnit,crystalType,shieldsHealed) => {HealDamage(targetedUnit,shieldsHealed);};
		repairCrewUsedRepairSectionAction = (selectedUnit, targetedUnit, sectionTargetedString) => {UseRepairCrew (selectedUnit, targetedUnit, sectionTargetedString);};
		earlyCalculateCrewRangeAction = () => {CalculateCrewRange(mouseManager.selectedUnit);};
		repairCrewUsedOnThisRepairSectionAction = (selectedUnit,targetedUnit) => {RepairSection(targetedUnit);};
		combatUnitCombatUnitDestroyedAction = (combatUnit) => {CombatUnitDestroyed(combatUnit);};
		saveDataResolveLoadedUnitAction = (combatUnit,saveGameData) => {ResolveLoadedUnit(combatUnit,saveGameData);};
			
		//add listener for end turn
		gameManager.OnEndTurn.AddListener(colorEndTurnAction);

		//add listener for getting hit by phasor attack
		//CombatManager.OnPhasorAttackHitBaseCrewSection.AddListener(attackHitTakeDamageAction);
		CutsceneManager.OnPhasorHitBaseCrewSection.AddListener(attackHitTakeDamageAction);

		//add listener for getting hit by torpedo attack
		//CombatManager.OnLightTorpedoAttackHitBaseCrewSection.AddListener(attackHitTakeDamageAction);
		//CombatManager.OnHeavyTorpedoAttackHitBaseCrewSection.AddListener(attackHitTakeDamageAction);
		CutsceneManager.OnLightTorpedoHitBaseCrewSection.AddListener(attackHitTakeDamageAction);
		CutsceneManager.OnHeavyTorpedoHitBaseCrewSection.AddListener(attackHitTakeDamageAction);

		//add listener for getting healed by a crystal
		CombatManager.OnCrystalUsedOnBaseCrewSection.AddListener(crystalUsedHealDamageAction);

		//add listeners for using repair crew
		uiManager.GetComponent<CrewMenu> ().OnUseRepairCrew.AddListener (repairCrewUsedRepairSectionAction);

		//add listener to the ealy setSelectedUnit so range calculations can occur before other listeners react
		mouseManager.OnSetSelectedUnitEarly.AddListener(earlyCalculateCrewRangeAction);

		//add listener for getting repaired by a repair crew
		CombatManager.OnRepairCrewUsedOnBaseCrewSection.AddListener(repairCrewUsedOnThisRepairSectionAction);

		//add listener for the combat unit being destroyed
		Starbase.OnBaseDestroyed.AddListener(combatUnitCombatUnitDestroyedAction);

		//add listener for creating unit from load
		CombatUnit.OnCreateLoadedUnit.AddListener(saveDataResolveLoadedUnitAction);

	}

	//this function is called to use a repair crew
	private void UseRepairCrew(CombatUnit selectedUnit, CombatUnit targetedUnit, string sectionTargetedString){

		//first, check if this is the selected unit
		if (this.GetComponent<CombatUnit> () == selectedUnit) {

			//this is the selected unit - we can set the usedRepairCrew flag to true
			this.usedRepairCrewThisTurn = true;

			//invoke the useRepairCrew event
			OnUseRepairCrew.Invoke(selectedUnit,targetedUnit,sectionTargetedString);

		}

	}

	//this function will calculate all hexes a starbase can reach in range
	private void CalculateCrewRange(GameObject gameObject){

		//check if the gameObject passed to the function is a starbase
		if (gameObject.GetComponent<Starbase> () == true) {

			//if it is a Starbase, check if it is this Starbase
			if (gameObject.GetComponent<Starbase> () == this.GetComponent<Starbase>()) {

				//get a list of all hex locations that are reachable
				//set last argument to true so it includes the hex that the ship is on
				TargetableCrewHexes = tileMap.TargetableTiles (this.GetComponent<CombatUnit>().currentLocation,crewRange, true);


				//create a list of hexes to exclude from targeting
				List<Hex> HexesToRemoveFromCrewTargeting = new List<Hex>();

				//check the targetable hexes and remove any that are occupied by cloaked combat units
				foreach (Hex hex in TargetableCrewHexes) {

					//check if the tile has a cloaked unit
					if (tileMap.HexMap [hex].tileCombatUnit != null && tileMap.HexMap [hex].tileCombatUnit.GetComponent<CloakingDevice>() == true &&
						tileMap.HexMap [hex].tileCombatUnit.GetComponent<CloakingDevice>().isCloaked == true) {

						//if it does, remove it from the targetable hexes
						HexesToRemoveFromCrewTargeting.Add(hex);

					}

				}

				//remove the hexes from the targetable hexes
				foreach (Hex hex in HexesToRemoveFromCrewTargeting) {

					TargetableCrewHexes.Remove (hex);

				}

			}

		}

	}

	//this function will handle taking damage if the section is hit by an attack
	private void TakeDamage(CombatUnit targetedUnit, int damage){

		//first, check if the unit that the crew section is attached to is the targeted unit that got hit
		if (this.GetComponent<CombatUnit> () == targetedUnit) {

			//this is the targeted unit - we can reduce the shields by the damage
			this.shieldsCurrent -= damage;

			//check if this puts shields at or below zero - if so, set to zero and call DestroySection
			if (shieldsCurrent <= 0) {

				shieldsCurrent = 0;
				this.DestroySection ();

			}

			OnCrewDamageTaken.Invoke ();

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

			OnCrewDamageTaken.Invoke ();


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
			OnCrewSectionRepaired.Invoke(this.GetComponent<CombatUnit>());

		}

	}

	//this function handles the section taking lethal damage and being destroyed
	private void DestroySection (){

		//if the section is destroyed, all the inventory on this section is removed
		this.repairCrew = false;
		this.shieldEngineeringTeam = false;
		this.battleCrew = false;

		//set the isDestroyed flag to true
		this.isDestroyed = true;

		//invoke the destroyed section event
		OnCrewSectionDestroyed.Invoke(this.GetComponent<CombatUnit>());

	}

	//this function will clean up variables at end of turn
	private void EndTurn(Player.Color currentTurn){

		//check if the color passed to the function matches the owner color
		//if so, our turn is ending, and we need to reset stuff
		if (currentTurn == this.GetComponent<CombatUnit>().owner.color) {

			this.usedRepairCrewThisTurn = false;

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
					this.isDestroyed = saveGameData.starbaseCrewSectionIsDestroyed [i];
					this.shieldsCurrent = saveGameData.starbaseCrewSectionShieldsCurrent [i];
					this.repairCrew = saveGameData.starbaseRepairCrew [i];
					this.shieldEngineeringTeam = saveGameData.starbaseShieldEngineeringTeam [i];
					this.battleCrew = saveGameData.starbaseBattleCrew [i];
					this.usedRepairCrewThisTurn = saveGameData.starbaseUsedRepairCrewThisTurn [i];

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

		//remove listener for getting hit by phasor attack
		//CombatManager.OnPhasorAttackHitBaseCrewSection.RemoveListener (attackHitTakeDamageAction);
		CutsceneManager.OnPhasorHitBaseCrewSection.RemoveListener(attackHitTakeDamageAction);

		//remove listener for getting hit by torpedo attack
		//CombatManager.OnLightTorpedoAttackHitBaseCrewSection.RemoveListener (attackHitTakeDamageAction);
		//CombatManager.OnHeavyTorpedoAttackHitBaseCrewSection.RemoveListener (attackHitTakeDamageAction);
		CutsceneManager.OnLightTorpedoHitBaseCrewSection.RemoveListener(attackHitTakeDamageAction);
		CutsceneManager.OnHeavyTorpedoHitBaseCrewSection.RemoveListener(attackHitTakeDamageAction);

		//remove listener for getting healed by a crystal
		CombatManager.OnCrystalUsedOnBaseCrewSection.RemoveListener (crystalUsedHealDamageAction);

		//remove listeners for using repair crew
		if (uiManager != null) {
			
			uiManager.GetComponent<CrewMenu> ().OnUseRepairCrew.RemoveListener (repairCrewUsedRepairSectionAction);

		}

		//remove listener to the ealy setSelectedUnit so range calculations can occur before other listeners react
		if (mouseManager != null) {
			
			mouseManager.OnSetSelectedUnitEarly.RemoveListener (earlyCalculateCrewRangeAction);

		}

		//remove listener for getting repaired by a repair crew
		CombatManager.OnRepairCrewUsedOnBaseCrewSection.RemoveListener (repairCrewUsedOnThisRepairSectionAction);

		//remove listener for the combat unit being destroyed
		Starbase.OnBaseDestroyed.RemoveListener (combatUnitCombatUnitDestroyedAction);

		//remove listener for creating unit from load
		CombatUnit.OnCreateLoadedUnit.RemoveListener (saveDataResolveLoadedUnitAction);

	}

}
