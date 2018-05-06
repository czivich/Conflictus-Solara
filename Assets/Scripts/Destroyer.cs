using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class Destroyer : Ship {

	//this child class is for Destroyer ships
	//starting values for items on Destroyer
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
	public static readonly bool startingTractorBeam = false;
	public static readonly int phaserSectionShieldsMax = 100;

	//torpedo section values
	public static readonly int startingTorpedoLaserShot = 0;
	public static readonly bool startingTorpedoLaserGuidanceSystem = false;
	public static readonly bool startingHighPressureTubes = true;
	public static readonly int startingLightTorpedos = 2;
	public static readonly int startingHeavyTorpedos = 2;
	public static readonly int torpedoSectionShieldsMax = 100;

	//storage section values
	public static readonly int startingDilithiumCrystals = 0;
	public static readonly int startingTrilithiumCrystals = 0;
	public static readonly bool startingRadarJammingSystem = false;
	public static readonly bool startingLaserScatteringSystem = false;
	public static readonly int startingFlares = 3;
	public static readonly int storageSectionShieldsMax = 100;


	//these textMeshPro elements are public so they can be hooked up in the inspector
	public TextMeshPro torpedoText;

	//unityActions
	private UnityAction<CombatUnit> combatUnitUpdateTorpedoAttackTextAction;
	private UnityAction<CombatUnit> combatUnitCombatUnitDestroyedAction;


	//Use this for initialization
	protected override void OnInitLevel2 () {

		//set the actions
		combatUnitUpdateTorpedoAttackTextAction = (combatUnit) => {UpdateTorpedoAttackText(combatUnit);};
		combatUnitCombatUnitDestroyedAction = (combatUnit) => {CombatUnitDestroyed (combatUnit);};

		//add listener for the update attack status event
		CombatUnit.OnUpdateAttackStatus.AddListener(combatUnitUpdateTorpedoAttackTextAction);

		//add listener for the combat unit being destroyed
		Ship.OnShipDestroyed.AddListener(combatUnitCombatUnitDestroyedAction);

		//intialize the sections
		this.GetComponent<PhaserSection>().Init();
		this.GetComponent<TorpedoSection>().Init();
		this.GetComponent<StorageSection>().Init();
		this.GetComponent<EngineSection>().Init();

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

	//function to handke on Destroy
	private void OnDestroy(){

		RemoveAllListeners ();

	}

	//function to remove listeners
	private void RemoveAllListeners(){

		//remove listener for the update attack status event
		CombatUnit.OnUpdateAttackStatus.RemoveListener(combatUnitUpdateTorpedoAttackTextAction);

		//remove listener for the combat unit being destroyed
		Ship.OnShipDestroyed.RemoveListener(combatUnitCombatUnitDestroyedAction);

	}

}
