using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class Starship : Ship {

	//this child class is specific to Starship ships

	//starting values for items on Starship
	//engine section values
	public static readonly int startingWarpBooster = 2;
	public static readonly int startingTranswarpBooster = 0;
	public static readonly bool startingWarpDrive = false;
	public static readonly bool startingTranswarpDrive = false;
	public static readonly int engineSectionShieldsMax = 100;

	//phaser section values
	public static readonly int startingPhaserRadarShot = 0;
	public static readonly bool startingXRayKernelUpgrade = false;
	public static readonly bool startingPhaserRadarArray = false;
	public static readonly bool startingTractorBeam = true;
	public static readonly int phaserSectionShieldsMax = 100;

	//torpedo section values
	public static readonly int startingTorpedoLaserShot = 0;
	public static readonly bool startingTorpedoLaserGuidanceSystem = false;
	public static readonly bool startingHighPressureTubes = false;
	public static readonly int startingLightTorpedos = 2;
	public static readonly int startingHeavyTorpedos = 0;
	public static readonly int torpedoSectionShieldsMax = 100;

	//storage section values
	public static readonly int startingDilithiumCrystals = 2;
	public static readonly int startingTrilithiumCrystals = 0;
	public static readonly bool startingRadarJammingSystem = false;
	public static readonly bool startingLaserScatteringSystem = false;
	public static readonly int startingFlares = 2;
	public static readonly int storageSectionShieldsMax = 100;

	//crew section values
	public static readonly bool startingRepairCrew = true;
	public static readonly bool startingShieldEngineeringTeam = false;
	public static readonly bool startingBattleCrew = false;
	public static readonly int crewSectionShieldsMax = 100;


	//these textMeshPro elements are public so they can be hooked up in the inspector
	public TextMeshPro torpedoText;
	public TextMeshPro repairText;

	//variable to keep track of remaining repair action
	public bool hasRemainingRepairAction {

		get;
		private set;

	}

	public static UpdateEvent OnUpdateRepairStatus = new UpdateEvent();

	//simple class derived from unityEvent to pass CombatUnit Object
	public class UpdateEvent : UnityEvent<CombatUnit>{};

	//unityActions
	private UnityAction<CombatUnit> combatUnitUpdateTorpedoAttackTextAction;
	private UnityAction<CombatUnit> combatUnitCombatUnitDestroyedAction;
	private UnityAction<CombatUnit,CombatUnit,string> repairCrewUsedUpdateRepairStatusAction;
	private UnityAction<CombatUnit> combatUnitUpdateRepairStatusAction;
	private UnityAction<Player> playerUpdateRepairStatusAction;


	//Use this for initialization
	protected override void OnInitLevel2 () {

		//set actions
		combatUnitUpdateTorpedoAttackTextAction = (combatUnit) => {UpdateTorpedoAttackText(combatUnit);};
		combatUnitCombatUnitDestroyedAction = (combatUnit) => {CombatUnitDestroyed(combatUnit);};
		repairCrewUsedUpdateRepairStatusAction = (selectedUnit,targetedUnit,sectionTargetedString) => {UpdateRepairStatus(selectedUnit);};
		combatUnitUpdateRepairStatusAction = (selectedUnit) => {UpdateRepairStatus(selectedUnit);};
		playerUpdateRepairStatusAction = (player) => {

			if(player == this.owner){
				UpdateRepairStatus(this.GetComponent<CombatUnit>());
			}

		};

		//add listener for the update attack status event
		CombatUnit.OnUpdateAttackStatus.AddListener(combatUnitUpdateTorpedoAttackTextAction);

		//add listener for the combat unit being destroyed
		Ship.OnShipDestroyed.AddListener(combatUnitCombatUnitDestroyedAction);

		//add listener for using repair crew
		CrewSection.OnUseRepairCrew.AddListener(repairCrewUsedUpdateRepairStatusAction);

		//add listeners for crew sections being repaired or destroyed
		CrewSection.OnCrewSectionDestroyed.AddListener(combatUnitUpdateRepairStatusAction);
		CrewSection.OnCrewSectionRepaired.AddListener(combatUnitUpdateRepairStatusAction);

		//add listener for starting new turn
		gameManager.OnNewTurn.AddListener(playerUpdateRepairStatusAction);

		//add listener for purchasing a repair crew upgrade
		CrewSection.OnInventoryUpdated.AddListener(combatUnitUpdateRepairStatusAction);


		//intialize the sections
		this.GetComponent<PhaserSection>().Init();
		this.GetComponent<TorpedoSection>().Init();
		this.GetComponent<StorageSection>().Init();
		this.GetComponent<CrewSection>().Init();
		this.GetComponent<EngineSection>().Init();


		//run an initial repair status update
		UpdateRepairStatus(this.GetComponent<CombatUnit> ());

		//initial update
		UpdateRepairAttackText(this.GetComponent<CombatUnit>());

	}

	//this function updates the torpedo attack text
	private void UpdateTorpedoAttackText(CombatUnit combatUnit){

		//check if this is the combat unit that was passed
		if (this.GetComponent<CombatUnit> () == combatUnit) {

			//check if we have a valid torpedo attack
			if (this.GetComponent<CombatUnit> ().hasRemainingTorpedoAttack == true) {

				torpedoText.text = "T";

			} else if (this.GetComponent<CombatUnit> ().hasRemainingTorpedoAttack == false) {

				torpedoText.text = "";

			}

		}

	}

	//this function will remove all listeners when the combat unit is destroyed
	private void CombatUnitDestroyed(CombatUnit combatUnitDestroyed){

		//check if the passed combat unit is this combat unit
		if (this.GetComponent<CombatUnit> () == combatUnitDestroyed) {

			RemoveAllListeners ();

		}

	}

	//function to update repair status
	private void UpdateRepairStatus(CombatUnit combatUnit){

		//check if the passed combat unit is this combat unit
		if (combatUnit == this.GetComponent<CombatUnit> ()) {

			//check if the crew section is alive
			if (combatUnit.GetComponent<CrewSection> ().isDestroyed == false) {

				//check if the ship has a repair crew
				if (combatUnit.GetComponent<CrewSection> ().repairCrew == true) {

					//check if repair crew has been used already
					if (combatUnit.GetComponent<CrewSection> ().usedRepairCrewThisTurn == false) {

						hasRemainingRepairAction = true;
					}
					//the else condition is that we have already used the repair crew
					else if (combatUnit.GetComponent<CrewSection> ().usedRepairCrewThisTurn == true) {

						hasRemainingRepairAction = false;

					}
				
				}
				//the else condition is that the ship does not have a repair crew
				else if (combatUnit.GetComponent<CrewSection> ().repairCrew == false) {

					hasRemainingRepairAction = false;

				}

			}
			//the else condition is that the crew section is destroyed
			else if (combatUnit.GetComponent<CrewSection> ().isDestroyed == true) {
				
				hasRemainingRepairAction = false;

			} else {

				hasRemainingRepairAction = false;

			}

			//call the repair text function
			UpdateRepairAttackText(this.GetComponent<CombatUnit>());

			//invoke the event
			OnUpdateRepairStatus.Invoke(this.GetComponent<CombatUnit>());

		}

	}

	//this function updates the repair text
	private void UpdateRepairAttackText(CombatUnit combatUnit){

		//check if this is the combat unit that was passed
		if (this.GetComponent<CombatUnit> () == combatUnit) {

			//check if we have a valid repair action
			if (this.hasRemainingRepairAction == true) {

				repairText.text = "R";

			} else if (this.hasRemainingRepairAction == false) {

				repairText.text = "";

			}

		}

	}

	//function to handle OnDestroy
	private void OnDestroy(){

		RemoveAllListeners ();

	}

	//function to remove all listeners
	private void RemoveAllListeners(){

		//remove listener for the update attack status event
		CombatUnit.OnUpdateAttackStatus.RemoveListener (combatUnitUpdateTorpedoAttackTextAction);

		//remove listener for the combat unit being destroyed
		Ship.OnShipDestroyed.RemoveListener (combatUnitCombatUnitDestroyedAction);

		//remove listener for using repair crew
		CrewSection.OnUseRepairCrew.RemoveListener (repairCrewUsedUpdateRepairStatusAction);

		//remove listeners for crew sections being repaired or destroyed
		CrewSection.OnCrewSectionDestroyed.RemoveListener (combatUnitUpdateRepairStatusAction);
		CrewSection.OnCrewSectionRepaired.RemoveListener (combatUnitUpdateRepairStatusAction);

		if (gameManager != null) {
			
			//remove listener for starting new turn
			gameManager.OnNewTurn.AddListener (playerUpdateRepairStatusAction);

		}

		//remove listener for purchasing a repair crew upgrade
		CrewSection.OnInventoryUpdated.RemoveListener (combatUnitUpdateRepairStatusAction);

	}
				
}
