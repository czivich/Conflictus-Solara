using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class BirdOfPrey : Ship {

	//this child class is for Bird of Prey ships

	//starting values for items on Bird of Prey
	//engine section values
	public static readonly int startingWarpBooster = 2;
	public static readonly int startingTranswarpBooster = 0;
	public static readonly bool startingWarpDrive = false;
	public static readonly bool startingTranswarpDrive = false;
	public static readonly int engineSectionShieldsMax = 90;

	//phaser section values
	public static readonly int startingPhaserRadarShot = 0;
	public static readonly bool startingXRayKernelUpgrade = false;
	public static readonly bool startingPhaserRadarArray = false;
	public static readonly bool startingTractorBeam = false;
	public static readonly int phaserSectionShieldsMax = 90;

	//torpedo section values
	public static readonly int startingTorpedoLaserShot = 0;
	public static readonly bool startingTorpedoLaserGuidanceSystem = false;
	public static readonly bool startingHighPressureTubes = false;
	public static readonly int startingLightTorpedos = 3;
	public static readonly int startingHeavyTorpedos = 0;
	public static readonly int torpedoSectionShieldsMax = 90;

	//variable to keep track of remaining cloaking action
	public bool hasRemainingCloakAction {

		get;
		private set;

	}

	//these textMeshPro elements are public so they can be hooked up in the inspector
	public TextMeshPro torpedoText;
	public TextMeshPro cloakText;

	public static UpdateEvent OnUpdateCloakingStatus = new UpdateEvent();

	//simple class derived from unityEvent to pass CombatUnit Object
	public class UpdateEvent : UnityEvent<CombatUnit>{};

	//unityAction for event listeners
	private UnityAction<CombatUnit> combatUnitUpdateTorpedoAttackStatusAction;
	private UnityAction<CombatUnit> combatUnitUpdateCloakingStatusAction;
	private UnityAction<CombatUnit> combatUnitCombatUnitDestroyedAction;
	private UnityAction<Player> playerUpdateCloakingStatusAction;

	//Use this for initialization
	protected override void OnInitLevel2 () {

		//set the actions
		combatUnitUpdateTorpedoAttackStatusAction = (combatUnit) => {UpdateTorpedoAttackText(combatUnit);};
		combatUnitUpdateCloakingStatusAction = (combatUnit) => {UpdateCloakingStatus(combatUnit);};
		combatUnitCombatUnitDestroyedAction = (combatUnit) => {CombatUnitDestroyed (combatUnit);};
		playerUpdateCloakingStatusAction = (player) => {

			if(player == this.owner){
				UpdateCloakingStatus(this.GetComponent<CombatUnit>());
			}

		};


		//add listener for the update attack status event
		CombatUnit.OnUpdateAttackStatus.AddListener(combatUnitUpdateTorpedoAttackStatusAction);

		//add listeners for cloaking events
		CloakingDevice.OnEngageCloakingDevice.AddListener(combatUnitUpdateCloakingStatusAction);
		CloakingDevice.OnDisengageCloakingDevice.AddListener(combatUnitUpdateCloakingStatusAction);


		//add listener for the combat unit being destroyed
		Ship.OnShipDestroyed.AddListener(combatUnitCombatUnitDestroyedAction);

		//add listener for starting new turn
		gameManager.OnNewTurn.AddListener(playerUpdateCloakingStatusAction);

		//intialize the sections
		this.GetComponent<PhaserSection>().Init();
		this.GetComponent<TorpedoSection>().Init();
		this.GetComponent<EngineSection>().Init();

		//initialize the cloaking device
		this.GetComponent<CloakingDevice>().Init();

		//run an initial cloak status update
		UpdateCloakingStatus(this.GetComponent<CombatUnit> ());

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

	//function to update cloaking status
	private void UpdateCloakingStatus(CombatUnit combatUnit){

		//check if the passed combat unit is this combat unit
		if (combatUnit == this.GetComponent<CombatUnit> ()) {

			//check if cloaking has been used already
			if (combatUnit.GetComponent<CloakingDevice> ().usedCloakingDeviceThisTurn == false) {

				hasRemainingCloakAction = true;
			}
			//the else condition is that we have already used the cloaking device
			else if (combatUnit.GetComponent<CloakingDevice> ().usedCloakingDeviceThisTurn == true) {

			hasRemainingCloakAction = false;

			}

			//call the cloaking text function
			UpdateCloakingText(this.GetComponent<CombatUnit>());

			//invoke the event
			OnUpdateCloakingStatus.Invoke(this.GetComponent<CombatUnit>());

		}

	}

	//this function updates the cloaking text
	private void UpdateCloakingText(CombatUnit combatUnit){

		//check if this is the combat unit that was passed
		if (this.GetComponent<CombatUnit> () == combatUnit) {

			//check if we have a valid remaining cloaking action
			if (this.hasRemainingCloakAction == true) {

				cloakText.text = "C";

			} else if (this.hasRemainingCloakAction == false) {

				cloakText.text = "";

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

	//this function will remove listeners when the gameobject is destroyed
	private void OnDestroy(){

		RemoveAllListeners ();

	}

	//this function removes all listeners
	private void RemoveAllListeners(){

		//remove listener for the update attack status event
		CombatUnit.OnUpdateAttackStatus.RemoveListener(combatUnitUpdateTorpedoAttackStatusAction);

		//remove listener for the combat unit being destroyed
		Ship.OnShipDestroyed.RemoveListener(combatUnitCombatUnitDestroyedAction);

		//remove listeners for cloaking events
		CloakingDevice.OnEngageCloakingDevice.RemoveListener(combatUnitUpdateCloakingStatusAction);
		CloakingDevice.OnDisengageCloakingDevice.RemoveListener(combatUnitUpdateCloakingStatusAction);

	}

}
