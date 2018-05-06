using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class Starbase : CombatUnit {

	//phaser section 1 values
	public static readonly int startingPhaserRadarShot = 3;
	public static readonly bool startingPhaserRadarArray = false;
	public static readonly int phaserSection1ShieldsMax = 100;

	//phaser section 2 values
	public static readonly bool startingXRayKernelUpgrade = true;
	public static readonly int phaserSection2ShieldsMax = 100;

	//torpedo section values
	public static readonly int startingTorpedoLaserShot = 3;
	public static readonly bool startingTorpedoLaserGuidanceSystem = false;
	public static readonly bool startingHighPressureTubes = true;
	public static readonly int startingLightTorpedos = 5;
	public static readonly int startingHeavyTorpedos = 3;
	public static readonly int torpedoSectionShieldsMax = 100;

	//storage section 1 values
	public static readonly int startingDilithiumCrystals = 5;
	public static readonly bool startingRadarJammingSystem = true;
	public static readonly int storageSection1ShieldsMax = 100;

	//storage section 2 values
	public static readonly int startingTrilithiumCrystals = 3;
	public static readonly bool startingLaserScatteringSystem = true;
	public static readonly int startingFlares = 10;
	public static readonly int storageSection2ShieldsMax = 100;

	//crew section values
	public static readonly bool startingRepairCrew = true;
	public static readonly bool startingShieldEngineeringTeam = true;
	public static readonly bool startingBattleCrew = true;
	public static readonly int crewSectionShieldsMax = 100;

	//variable to store the base name
	public string baseName{

		get;
		private set;

	}

	//variable to keep track of remaining repair action
	public bool hasRemainingRepairAction {

		get;
		private set;

	}

	//public TextMeshPros to allow the texts on the unit
	public TextMeshPro baseNameText;
	public TextMeshPro phasersText;
	public TextMeshPro torpedoText;
	public TextMeshPro repairText;

	//event to announce the base was destroyed
	public static BaseDestroyedEvent OnBaseDestroyed = new BaseDestroyedEvent();
	public static BaseDestroyedEvent OnBaseDestroyedLate = new BaseDestroyedEvent();

	//event to announce base was renamed
	public static BaseRenamedEvent OnBaseRenamed = new BaseRenamedEvent();

	//event to announce repair crew status was updated
	public static BaseDestroyedEvent OnUpdateRepairStatus = new BaseDestroyedEvent();

	//class for event to announce base was destroyed
	public class BaseDestroyedEvent : UnityEvent<CombatUnit>{};

	//class for event to announce base was renamed
	public class BaseRenamedEvent : UnityEvent<CombatUnit, string>{};

	//unityActions
	private UnityAction<CombatUnit> combatUnitCheckBaseDestroyedAction;
	private UnityAction<CombatUnit,string,GameManager.ActionMode> unitRenamedChangeBaseNameAction;
	private UnityAction<CombatUnit> combatUnitUpdatePhaserAttackTextAction;
	private UnityAction<CombatUnit> combatUnitUpdateTorpedoAttackTextAction;
	private UnityAction<CombatUnit,CombatUnit,string> unitRepairedUseRepairCrewAction;
	private UnityAction<CombatUnit> combatUnitUpdateRepairStatusAction;
	private UnityAction resolveAllUnitsLoadedAction;
	private UnityAction<Player> playerUpdateRepairStatusAction;

	//Use this for initialization
	protected override void OnInit () {

		//assign the baseNameText
		baseNameText.text = this.unitName;

		//set the baseName
		baseName = baseNameText.text;

		//set the actions
		combatUnitCheckBaseDestroyedAction = (combatUnit) => {CheckBaseDestroyed(combatUnit);};
		unitRenamedChangeBaseNameAction = (combatUnit, newName, previousActionMode) => {ChangeBaseName(combatUnit, newName);};
		combatUnitUpdatePhaserAttackTextAction = (combatUnit) => {UpdatePhaserAttackText(combatUnit);};
		combatUnitUpdateTorpedoAttackTextAction = (combatUnit) => {UpdateTorpedoAttackText(combatUnit);};
		unitRepairedUseRepairCrewAction = (selectedUnit,targetedUnit,sectionTargetedString) => {UpdateRepairStatus(selectedUnit);};
		combatUnitUpdateRepairStatusAction = (combatUnit) => {UpdateRepairStatus(combatUnit);};
		resolveAllUnitsLoadedAction = () => {ResolveAllUnitsLoaded();};
		playerUpdateRepairStatusAction = (player) => {

			if(player == this.owner){
				UpdateRepairStatus(this.GetComponent<CombatUnit>());
			}

		};

		//add listeners for various sections of the base getting destroyed
		StarbasePhaserSection1.OnPhaserSection1Destroyed.AddListener(combatUnitCheckBaseDestroyedAction);
		StarbasePhaserSection2.OnPhaserSection2Destroyed.AddListener(combatUnitCheckBaseDestroyedAction);
		StarbaseTorpedoSection.OnTorpedoSectionDestroyed.AddListener(combatUnitCheckBaseDestroyedAction);
		StarbaseStorageSection1.OnStorageSection1Destroyed.AddListener(combatUnitCheckBaseDestroyedAction);
		StarbaseStorageSection2.OnStorageSection2Destroyed.AddListener(combatUnitCheckBaseDestroyedAction);
		StarbaseCrewSection.OnCrewSectionDestroyed.AddListener(combatUnitCheckBaseDestroyedAction);

		//add listener for the rename event
		uiManager.GetComponent<RenameShip>().OnRenameUnit.AddListener(unitRenamedChangeBaseNameAction);

		//add listener for the update attack status event
		CombatUnit.OnUpdateAttackStatus.AddListener(combatUnitUpdatePhaserAttackTextAction);

		//add listener for the update attack status event
		CombatUnit.OnUpdateAttackStatus.AddListener(combatUnitUpdateTorpedoAttackTextAction);

		//add listener for using repair crew
		StarbaseCrewSection.OnUseRepairCrew.AddListener(unitRepairedUseRepairCrewAction);

		//add listeners for crew sections being repaired or destroyed
		StarbaseCrewSection.OnCrewSectionDestroyed.AddListener(combatUnitUpdateRepairStatusAction);
		StarbaseCrewSection.OnCrewSectionRepaired.AddListener(combatUnitUpdateRepairStatusAction);

		//add listener for starting new turn
		gameManager.OnNewTurn.AddListener(playerUpdateRepairStatusAction);

		//add listener for all units done being loaded
		gameManager.OnAllLoadedUnitsCreated.AddListener(resolveAllUnitsLoadedAction);


		//intialize the sections
		this.GetComponent<StarbasePhaserSection1>().Init();
		this.GetComponent<StarbasePhaserSection2>().Init();
		this.GetComponent<StarbaseTorpedoSection>().Init();
		this.GetComponent<StarbaseCrewSection>().Init();
		this.GetComponent<StarbaseStorageSection1>().Init();
		this.GetComponent<StarbaseStorageSection2>().Init();

		//run an initial repair status update
		UpdateRepairStatus(this.GetComponent<CombatUnit> ());

	}

	//this function checks if the base is destroyed when a section gets destroyed
	private void CheckBaseDestroyed(CombatUnit combatUnit){

		//first, check if the section destroyed was on this combat unit
		if (this.GetComponent<CombatUnit> () == combatUnit) {

			//if all sections are destroyed
			if (combatUnit.GetComponent<StarbasePhaserSection1> ().isDestroyed == true &&
				combatUnit.GetComponent<StarbasePhaserSection2> ().isDestroyed == true &&
			    combatUnit.GetComponent<StarbaseTorpedoSection> ().isDestroyed == true &&
			    combatUnit.GetComponent<StarbaseStorageSection1> ().isDestroyed == true &&
				combatUnit.GetComponent<StarbaseStorageSection2> ().isDestroyed == true && 
				combatUnit.GetComponent<StarbaseCrewSection> ().isDestroyed == true) {

				//remove all listeners
				//remove listeners for various sections of the base getting destroyed
				StarbasePhaserSection1.OnPhaserSection1Destroyed.RemoveListener((combatUnitDelegate) => {CheckBaseDestroyed(combatUnitDelegate);});
				StarbasePhaserSection2.OnPhaserSection2Destroyed.RemoveListener((combatUnitDelegate) => {CheckBaseDestroyed(combatUnitDelegate);});
				StarbaseTorpedoSection.OnTorpedoSectionDestroyed.RemoveListener((combatUnitDelegate) => {CheckBaseDestroyed(combatUnitDelegate);});
				StarbaseStorageSection1.OnStorageSection1Destroyed.RemoveListener((combatUnitDelegate) => {CheckBaseDestroyed(combatUnitDelegate);});
				StarbaseStorageSection2.OnStorageSection2Destroyed.RemoveListener((combatUnitDelegate) => {CheckBaseDestroyed(combatUnitDelegate);});
				StarbaseCrewSection.OnCrewSectionDestroyed.RemoveListener((combatUnitDelegate) => {CheckBaseDestroyed(combatUnitDelegate);});

				//remove listener for the rename event
				uiManager.GetComponent<RenameShip>().OnRenameUnit.RemoveListener((combatUnitDelegate, newName, previousActionMode) => {ChangeBaseName(combatUnitDelegate, newName);});

				//remove listener for the update attack status event
				CombatUnit.OnUpdateAttackStatus.RemoveListener((combatUnitDelegate) => {UpdatePhaserAttackText(combatUnitDelegate);});

				//remove listener for the update attack status event
				CombatUnit.OnUpdateAttackStatus.RemoveListener((combatUnitDelegate) => {UpdateTorpedoAttackText(combatUnitDelegate);});

				//remove listener for using repair crew
				StarbaseCrewSection.OnUseRepairCrew.RemoveListener((selectedUnit,targetedUnit,sectionTargetedString) => {UpdateRepairStatus(selectedUnit);});

				//remove listeners for crew sections being repaired or destroyed
				StarbaseCrewSection.OnCrewSectionDestroyed.RemoveListener((combatUnitDelegate) => {UpdateRepairStatus(combatUnitDelegate);});
				StarbaseCrewSection.OnCrewSectionRepaired.RemoveListener((combatUnitDelegate) => {UpdateRepairStatus(combatUnitDelegate);});

				//remove listener for all units done being loaded
				gameManager.OnAllLoadedUnitsCreated.RemoveListener(() => {ResolveAllUnitsLoaded();});

				//invoke the ShipDestroyed event
				OnBaseDestroyed.Invoke (combatUnit);

				//destroy this gameobject
				Destroy (this.gameObject);

				//invoke the ShipDestroyed event
				OnBaseDestroyedLate.Invoke (combatUnit);

			}

		}

	}

	//this function changes the base name
	private void ChangeBaseName(CombatUnit combatUnit, string newName){

		//check if this is the unit that is being changed
		if (this.GetComponent<CombatUnit> () == combatUnit) {

			//cache the old base name
			string oldBaseName = this.baseName;

			//update the base name
			this.baseName = newName;

			//update the shipName text
			this.baseNameText.text = newName;

			//invoke the onRenamed event
			OnBaseRenamed.Invoke(this.GetComponent<CombatUnit> (),oldBaseName);

		}

	}

	//this function updates the phaser attack text
	private void UpdatePhaserAttackText(CombatUnit combatUnit){

		//check if this is the combat unit that was passed
		if (this.GetComponent<CombatUnit> () == combatUnit) {

			//check if we have a valid phaser attack
			if (this.GetComponent<CombatUnit> ().hasRemainingPhaserAttack == true) {

				phasersText.text = "P";
			} else if (this.GetComponent<CombatUnit> ().hasRemainingPhaserAttack == false) {

				phasersText.text = "";

			}

		}

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

	//function to update repair status
	private void UpdateRepairStatus(CombatUnit combatUnit){

		//check if the passed combat unit is this combat unit
		if (combatUnit == this.GetComponent<CombatUnit> ()) {

			//check if the crew section is alive
			if (combatUnit.GetComponent<StarbaseCrewSection> ().isDestroyed == false) {

				//check if the ship has a repair crew
				if (combatUnit.GetComponent<StarbaseCrewSection> ().repairCrew == true) {

					//check if repair crew has been used already
					if (combatUnit.GetComponent<StarbaseCrewSection> ().usedRepairCrewThisTurn == false) {

						hasRemainingRepairAction = true;

					}
					//the else condition is that we have already used the repair crew
					else if (combatUnit.GetComponent<StarbaseCrewSection> ().usedRepairCrewThisTurn == true) {

						hasRemainingRepairAction = false;

					}

				}
				//the else condition is that the ship does not have a repair crew
				else if (combatUnit.GetComponent<StarbaseCrewSection> ().repairCrew == false) {

					hasRemainingRepairAction = false;

				}

			}
			//the else condition is that the crew section is destroyed
			else if (combatUnit.GetComponent<StarbaseCrewSection> ().isDestroyed == true) {

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

	//this function updates the repair attack text
	private void UpdateRepairAttackText(CombatUnit combatUnit){

		//check if this is the combat unit that was passed
		if (this.GetComponent<CombatUnit> () == combatUnit) {

			//check if we have a valid torpedo attack
			if (this.hasRemainingRepairAction == true) {

				repairText.text = "R";

			} else if (this.hasRemainingRepairAction == false) {

				repairText.text = "";

			}

		}

	}

	//this function resolves an all units loaded signal
	private void ResolveAllUnitsLoaded(){

		UpdateRepairStatus(this.GetComponent<CombatUnit> ());

		UpdatePhaserAttackText (this.GetComponent<CombatUnit> ());

		UpdateTorpedoAttackText (this.GetComponent<CombatUnit> ());

	}

	//this function handles OnDestroy
	private void OnDestroy(){

		RemoveAllListeners ();

	}

	//this function removes all listeners
	private void RemoveAllListeners(){

		//remove listeners for various sections of the base getting destroyed
		StarbasePhaserSection1.OnPhaserSection1Destroyed.RemoveListener (combatUnitCheckBaseDestroyedAction);
		StarbasePhaserSection2.OnPhaserSection2Destroyed.RemoveListener (combatUnitCheckBaseDestroyedAction);
		StarbaseTorpedoSection.OnTorpedoSectionDestroyed.RemoveListener (combatUnitCheckBaseDestroyedAction);
		StarbaseStorageSection1.OnStorageSection1Destroyed.RemoveListener (combatUnitCheckBaseDestroyedAction);
		StarbaseStorageSection2.OnStorageSection2Destroyed.RemoveListener (combatUnitCheckBaseDestroyedAction);
		StarbaseCrewSection.OnCrewSectionDestroyed.RemoveListener (combatUnitCheckBaseDestroyedAction);

		//remove listener for the rename event
		if (uiManager != null) {
			
			uiManager.GetComponent<RenameShip> ().OnRenameUnit.RemoveListener (unitRenamedChangeBaseNameAction);

		}

		//remove listener for the update attack status event
		CombatUnit.OnUpdateAttackStatus.RemoveListener (combatUnitUpdatePhaserAttackTextAction);

		//remove listener for the update attack status event
		CombatUnit.OnUpdateAttackStatus.RemoveListener (combatUnitUpdateTorpedoAttackTextAction);

		//remove listener for using repair crew
		StarbaseCrewSection.OnUseRepairCrew.RemoveListener (unitRepairedUseRepairCrewAction);

		//remove listeners for crew sections being repaired or destroyed
		StarbaseCrewSection.OnCrewSectionDestroyed.RemoveListener (combatUnitUpdateRepairStatusAction);
		StarbaseCrewSection.OnCrewSectionRepaired.RemoveListener (combatUnitUpdateRepairStatusAction);

		//remove listener for all units done being loaded
		if (gameManager != null) {
			
			gameManager.OnAllLoadedUnitsCreated.RemoveListener (resolveAllUnitsLoadedAction);

		}

	}

}
