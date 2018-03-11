using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CombatManager : MonoBehaviour {

	//variables for the managers
	private UIManager uiManager;

	//variable for the tileMap
	private TileMap tileMap;

	//define enum for attack type
	public enum AttackType{

		Phasor,
		LightTorpedo,
		HeavyTorpedo,

	}

	public enum ShipSectionTargeted{

		PhasorSection,
		TorpedoSection,
		StorageSection,
		CrewSection,
		EngineSection,
		Miss,
		Untargeted,

	}

	public enum BaseSectionTargeted{

		PhasorSection1,
		PhasorSection2,
		TorpedoSection,
		CrewSection,
		StorageSection1,
		StorageSection2,
		Untargeted,

	}

	//define enum for crystal type
	public enum CrystalType{

		Dilithium,
		Trilithium,

	}

	//private variable to hold the number of sides on an attack die
	private const int numberSidedDice = 20;

	private const int numberSidedFlareDice = 6;

	//variables to hold number of 20-sided attack dice for various attacks
	private const int numberDicePhasorPrimaryCone = 3;
	private const int numberDicePhasorSecondaryCone = 2;

	private const int numberDiceXRayPrimaryCone = 4;
	private const int numberDiceXRaySecondaryCone = 3;


	private const int numberDiceLightTorpedoPrimaryCone = 4;
	private const int numberDiceLightTorpedoSecondaryCone = 3;

	private const int numberDiceHeavyTorpedoPrimaryCone = 5;
	private const int numberDiceHeavyTorpedoSecondaryCone = 4;

	private const int numberDiceLightTorpedoPrimaryConeWithTubes = 4;
	private const int numberDiceLightTorpedoSecondaryConeWithTubes = 4;
	private const int numberDiceLightTorpedoTertiaryConeWithTubes = 3;

	private const int numberDiceHeavyTorpedoPrimaryConeWithTubes = 5;
	private const int numberDiceHeavyTorpedoSecondaryConeWithTubes = 5;
	private const int numberDiceHeavyTorpedoTertiaryConeWithTubes = 4;

	//variables to hold the healing values of crystals
	private const int dilithiumHealing = 30;


	//event to temporarily allow the targeted unit to be found by the GeneratePath
	public static TargetPathEvent OnStartTargetPath = new TargetPathEvent();
	public static TargetPathEvent OnEndTargetPath = new TargetPathEvent();

	//simple class derived from unityEvent to pass Ship Object
	public class TargetPathEvent : UnityEvent<CombatUnit>{};

	//ship phasor hits
	public static AttackHitResolutionEvent OnPhasorAttackHitShipPhasorSection = new AttackHitResolutionEvent();
	public static AttackHitResolutionEvent OnPhasorAttackHitShipTorpedoSection = new AttackHitResolutionEvent();
	public static AttackHitResolutionEvent OnPhasorAttackHitShipStorageSection = new AttackHitResolutionEvent();
	public static AttackHitResolutionEvent OnPhasorAttackHitShipCrewSection = new AttackHitResolutionEvent();
	public static AttackHitResolutionEvent OnPhasorAttackHitShipEngineSection = new AttackHitResolutionEvent();
	public static AttackMissResolutionEvent OnPhasorAttackMissShip = new AttackMissResolutionEvent();

	//base phasor hits
	public static AttackHitResolutionEvent OnPhasorAttackHitBasePhasorSection1 = new AttackHitResolutionEvent();
	public static AttackHitResolutionEvent OnPhasorAttackHitBasePhasorSection2 = new AttackHitResolutionEvent();
	public static AttackHitResolutionEvent OnPhasorAttackHitBaseTorpedoSection = new AttackHitResolutionEvent();
	public static AttackHitResolutionEvent OnPhasorAttackHitBaseCrewSection = new AttackHitResolutionEvent();
	public static AttackHitResolutionEvent OnPhasorAttackHitBaseStorageSection1 = new AttackHitResolutionEvent();
	public static AttackHitResolutionEvent OnPhasorAttackHitBaseStorageSection2 = new AttackHitResolutionEvent();
	public static AttackMissResolutionEvent OnPhasorAttackMissBase = new AttackMissResolutionEvent();

	//ship light torpedo hits
	public static AttackHitResolutionEvent OnLightTorpedoAttackHitShipPhasorSection = new AttackHitResolutionEvent();
	public static AttackHitResolutionEvent OnLightTorpedoAttackHitShipTorpedoSection = new AttackHitResolutionEvent();
	public static AttackHitResolutionEvent OnLightTorpedoAttackHitShipStorageSection = new AttackHitResolutionEvent();
	public static AttackHitResolutionEvent OnLightTorpedoAttackHitShipCrewSection = new AttackHitResolutionEvent();
	public static AttackHitResolutionEvent OnLightTorpedoAttackHitShipEngineSection = new AttackHitResolutionEvent();
	public static AttackMissResolutionEvent OnLightTorpedoAttackMissShip = new AttackMissResolutionEvent();

	//base light torpedo hits
	public static AttackHitResolutionEvent OnLightTorpedoAttackHitBasePhasorSection1 = new AttackHitResolutionEvent();
	public static AttackHitResolutionEvent OnLightTorpedoAttackHitBasePhasorSection2 = new AttackHitResolutionEvent();
	public static AttackHitResolutionEvent OnLightTorpedoAttackHitBaseTorpedoSection = new AttackHitResolutionEvent();
	public static AttackHitResolutionEvent OnLightTorpedoAttackHitBaseCrewSection = new AttackHitResolutionEvent();
	public static AttackHitResolutionEvent OnLightTorpedoAttackHitBaseStorageSection1 = new AttackHitResolutionEvent();
	public static AttackHitResolutionEvent OnLightTorpedoAttackHitBaseStorageSection2 = new AttackHitResolutionEvent();
	public static AttackMissResolutionEvent OnLightTorpedoAttackMissBase = new AttackMissResolutionEvent();

	//flare result events
	public static AttackFlareResolutionEvent OnLightTorpedoAttackDefeatedByFlares = new AttackFlareResolutionEvent();
	public static AttackFlareResolutionEvent OnHeavyTorpedoAttackDefeatedByFlares = new AttackFlareResolutionEvent();
	public static AttackHitResolutionEvent OnLightTorpedoAttackFlaresFailed= new AttackHitResolutionEvent();
	public static AttackHitResolutionEvent OnHeavyTorpedoAttackFlaresFailed = new AttackHitResolutionEvent();

	//ship heavy torpedo hits
	public static AttackHitResolutionEvent OnHeavyTorpedoAttackHitShipPhasorSection = new AttackHitResolutionEvent();
	public static AttackHitResolutionEvent OnHeavyTorpedoAttackHitShipTorpedoSection = new AttackHitResolutionEvent();
	public static AttackHitResolutionEvent OnHeavyTorpedoAttackHitShipStorageSection = new AttackHitResolutionEvent();
	public static AttackHitResolutionEvent OnHeavyTorpedoAttackHitShipCrewSection = new AttackHitResolutionEvent();
	public static AttackHitResolutionEvent OnHeavyTorpedoAttackHitShipEngineSection = new AttackHitResolutionEvent();
	public static AttackMissResolutionEvent OnHeavyTorpedoAttackMissShip = new AttackMissResolutionEvent();

	//base heavy torpedo hits
	public static AttackHitResolutionEvent OnHeavyTorpedoAttackHitBasePhasorSection1 = new AttackHitResolutionEvent();
	public static AttackHitResolutionEvent OnHeavyTorpedoAttackHitBasePhasorSection2 = new AttackHitResolutionEvent();
	public static AttackHitResolutionEvent OnHeavyTorpedoAttackHitBaseTorpedoSection = new AttackHitResolutionEvent();
	public static AttackHitResolutionEvent OnHeavyTorpedoAttackHitBaseCrewSection = new AttackHitResolutionEvent();
	public static AttackHitResolutionEvent OnHeavyTorpedoAttackHitBaseStorageSection1 = new AttackHitResolutionEvent();
	public static AttackHitResolutionEvent OnHeavyTorpedoAttackHitBaseStorageSection2 = new AttackHitResolutionEvent();
	public static AttackMissResolutionEvent OnHeavyTorpedoAttackMissBase = new AttackMissResolutionEvent();

	//attack ship with flares events
	public static AttackFlareResolutionEvent OnLightTorpedoUntargetedAttackShipWithFlares = new AttackFlareResolutionEvent();
	public static AttackFlareResolutionEvent OnLightTorpedoTargetedAttackShipWithFlares = new AttackFlareResolutionEvent();
	public static AttackFlareResolutionEvent OnHeavyTorpedoUntargetedAttackShipWithFlares = new AttackFlareResolutionEvent();
	public static AttackFlareResolutionEvent OnHeavyTorpedoTargetedAttackShipWithFlares = new AttackFlareResolutionEvent();

	//attack base with flares events
	public static AttackBaseFlareResolutionEvent OnLightTorpedoUntargetedAttackBaseWithFlares = new AttackBaseFlareResolutionEvent();
	public static AttackBaseFlareResolutionEvent OnLightTorpedoTargetedAttackBaseWithFlares = new AttackBaseFlareResolutionEvent();
	public static AttackBaseFlareResolutionEvent OnHeavyTorpedoUntargetedAttackBaseWithFlares = new AttackBaseFlareResolutionEvent();
	public static AttackBaseFlareResolutionEvent OnHeavyTorpedoTargetedAttackBaseWithFlares = new AttackBaseFlareResolutionEvent();

	//crystal used on ship events
	public static CrystalResolutionEvent OnCrystalUsedOnShipPhasorSection = new CrystalResolutionEvent();
	public static CrystalResolutionEvent OnCrystalUsedOnShipTorpedoSection = new CrystalResolutionEvent();
	public static CrystalResolutionEvent OnCrystalUsedOnShipStorageSection = new CrystalResolutionEvent();
	public static CrystalResolutionEvent OnCrystalUsedOnShipCrewSection = new CrystalResolutionEvent();
	public static CrystalResolutionEvent OnCrystalUsedOnShipEngineSection = new CrystalResolutionEvent();

	//repair crew used on ship events
	public static RepairResolutionEvent OnRepairCrewUsedOnShipPhasorSection = new RepairResolutionEvent();
	public static RepairResolutionEvent OnRepairCrewUsedOnShipTorpedoSection = new RepairResolutionEvent();
	public static RepairResolutionEvent OnRepairCrewUsedOnShipStorageSection = new RepairResolutionEvent();
	public static RepairResolutionEvent OnRepairCrewUsedOnShipCrewSection = new RepairResolutionEvent();
	public static RepairResolutionEvent OnRepairCrewUsedOnShipEngineSection = new RepairResolutionEvent();

	//crystal used on base events
	public static CrystalResolutionEvent OnCrystalUsedOnBasePhasorSection1 = new CrystalResolutionEvent();
	public static CrystalResolutionEvent OnCrystalUsedOnBasePhasorSection2 = new CrystalResolutionEvent();
	public static CrystalResolutionEvent OnCrystalUsedOnBaseTorpedoSection = new CrystalResolutionEvent();
	public static CrystalResolutionEvent OnCrystalUsedOnBaseCrewSection = new CrystalResolutionEvent();
	public static CrystalResolutionEvent OnCrystalUsedOnBaseStorageSection1 = new CrystalResolutionEvent();
	public static CrystalResolutionEvent OnCrystalUsedOnBaseStorageSection2 = new CrystalResolutionEvent();

	//repair crew used on ship events
	public static RepairResolutionEvent OnRepairCrewUsedOnBasePhasorSection1 = new RepairResolutionEvent();
	public static RepairResolutionEvent OnRepairCrewUsedOnBasePhasorSection2 = new RepairResolutionEvent();
	public static RepairResolutionEvent OnRepairCrewUsedOnBaseTorpedoSection = new RepairResolutionEvent();
	public static RepairResolutionEvent OnRepairCrewUsedOnBaseCrewSection = new RepairResolutionEvent();
	public static RepairResolutionEvent OnRepairCrewUsedOnBaseStorageSection1 = new RepairResolutionEvent();
	public static RepairResolutionEvent OnRepairCrewUsedOnBaseStorageSection2 = new RepairResolutionEvent();

	//class derived from unity event to announce combat
	public class AttackHitResolutionEvent : UnityEvent<CombatUnit,CombatUnit,int>{};
	public class AttackMissResolutionEvent : UnityEvent<CombatUnit,CombatUnit>{};

	public class AttackFlareResolutionEvent : UnityEvent<CombatUnit,CombatUnit,ShipSectionTargeted,int>{};
	public class AttackBaseFlareResolutionEvent : UnityEvent<CombatUnit,CombatUnit,BaseSectionTargeted,int>{};

	public class CrystalResolutionEvent : UnityEvent<CombatUnit,CombatUnit,CrystalType, int>{};

	public class RepairResolutionEvent : UnityEvent<CombatUnit,CombatUnit>{};

	//unityActions
	private UnityAction<CombatUnit,CombatUnit,string> firePhasorsResolvePhasorAttackAction;
	private UnityAction<CombatUnit,CombatUnit,string> fireTorpedoResolveLightTorpedoAttackAction;
	private UnityAction<CombatUnit,CombatUnit,string> fireTorpedoResolveHeavyTorpedoAttackAction;
	private UnityAction<FlareManager.FlareEventData> flareMenuResolveTorpedoAttackTrueAction;
	private UnityAction<FlareManager.FlareEventData> flareMenuResolveTorpedoAttackFalseAction;
	private UnityAction<CombatUnit,CombatUnit,string> useCrystalResolveDilithiumCrystalAction;
	private UnityAction<CombatUnit,CombatUnit,string> useCrystalResolveTrilithiumCrystalAction;
	private UnityAction<CombatUnit,CombatUnit,string> useRepairResolveRepairAction;


	// Use this for initialization
	public void Init () {

		//set the actions
		firePhasorsResolvePhasorAttackAction = (attackingUnit,targetedUnit,sectionTargeted) => {ResolvePhasorAttack(attackingUnit,targetedUnit,sectionTargeted);};
		fireTorpedoResolveLightTorpedoAttackAction = (attackingUnit,targetedUnit,sectionTargeted) => {ResolveTorpedoAttack(attackingUnit,targetedUnit,sectionTargeted,AttackType.LightTorpedo);};
		fireTorpedoResolveHeavyTorpedoAttackAction = (attackingUnit,targetedUnit,sectionTargeted) => {ResolveTorpedoAttack(attackingUnit,targetedUnit,sectionTargeted,AttackType.HeavyTorpedo);};
		flareMenuResolveTorpedoAttackTrueAction = (flareMenuData) => {ResolveTorpedoAttackAfterFlares(flareMenuData, true);};
		flareMenuResolveTorpedoAttackFalseAction = (flareMenuData) => {ResolveTorpedoAttackAfterFlares(flareMenuData, false);};
		useCrystalResolveDilithiumCrystalAction = (selectedUnit,targetedUnit,sectionTargetedString) => {ResolveCrystal(selectedUnit,targetedUnit,sectionTargetedString,CrystalType.Dilithium);};
		useCrystalResolveTrilithiumCrystalAction = (selectedUnit,targetedUnit,sectionTargetedString) => {ResolveCrystal(selectedUnit,targetedUnit,sectionTargetedString,CrystalType.Trilithium);};
		useRepairResolveRepairAction = (selectedUnit,targetedUnit,sectionTargetedString) => {ResolveRepair(selectedUnit,targetedUnit,sectionTargetedString);};


		//get the managers
		uiManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();

		//find tileMap in the game
		tileMap = GameObject.FindGameObjectWithTag ("TileMap").GetComponent<TileMap> ();

		//add a listener to the Phasor Section static combat event
		PhasorSection.OnFirePhasors.AddListener(firePhasorsResolvePhasorAttackAction);

		//add a listener to the Torpedo Section static combat event
		TorpedoSection.OnFireLightTorpedo.AddListener(fireTorpedoResolveLightTorpedoAttackAction);
		TorpedoSection.OnFireHeavyTorpedo.AddListener(fireTorpedoResolveHeavyTorpedoAttackAction);

		//add listeners for the flareMenu button presses
		uiManager.GetComponent<FlareManager>().OnUseFlaresYes.AddListener(flareMenuResolveTorpedoAttackTrueAction);
		uiManager.GetComponent<FlareManager>().OnUseFlaresCancel.AddListener(flareMenuResolveTorpedoAttackFalseAction);

		//add listeners for the useCrystal events
		StorageSection.OnUseDilithiumCrystal.AddListener(useCrystalResolveDilithiumCrystalAction);
		StorageSection.OnUseTrilithiumCrystal.AddListener(useCrystalResolveTrilithiumCrystalAction);

		//add listeners for the repair crew event
		CrewSection.OnUseRepairCrew.AddListener(useRepairResolveRepairAction);

		//add listener to the starbase phasor section static combat event
		StarbasePhasorSection1.OnFirePhasors.AddListener(firePhasorsResolvePhasorAttackAction);
		StarbasePhasorSection2.OnFirePhasors.AddListener(firePhasorsResolvePhasorAttackAction);

		//add a listener to the starbase Torpedo Section static combat event
		StarbaseTorpedoSection.OnFireLightTorpedo.AddListener(fireTorpedoResolveLightTorpedoAttackAction);
		StarbaseTorpedoSection.OnFireHeavyTorpedo.AddListener(fireTorpedoResolveHeavyTorpedoAttackAction);

		//add listeners to the starbase useCrystal events
		StarbaseStorageSection1.OnUseDilithiumCrystal.AddListener(useCrystalResolveDilithiumCrystalAction);
		StarbaseStorageSection2.OnUseTrilithiumCrystal.AddListener(useCrystalResolveTrilithiumCrystalAction);

		//add listener for the starbase repair crew event
		StarbaseCrewSection.OnUseRepairCrew.AddListener(useRepairResolveRepairAction);

	}

	//this function will resolve a phasor attack when one unit attacks another
	private void ResolvePhasorAttack(CombatUnit attackingUnit, CombatUnit targetedUnit, string sectionTargeted){
			
		//next, check if the targeted unit is a ship or a starbase
		if (targetedUnit.GetComponent<Ship> () == true) {

			ShipSectionTargeted shipSectionTargeted;

			//this was a ship attacking a ship.  Now we need to determine if it was a targeted attack or not
			//the sectionTargeted string will be "Choose Section" if it was a non-targeted attack
			if (sectionTargeted == "Choose Section") {

				//this was an untargeted attack.  We need to roll a die to determine which section is being attacked
				//we subtract one because the Dice function returns a number 1-6, and the enum has values 0-5 for the sections we can hit
				shipSectionTargeted = (ShipSectionTargeted)(Dice.DiceRollSum (1, 6) - 1);

				//we need to check if the random section targeted is valid, or if it is a miss
				switch (shipSectionTargeted) {

				case ShipSectionTargeted.PhasorSection:

					//check if the targeted unit has a phasor section
					if (targetedUnit.GetComponent<PhasorSection> () == true) {

						//check if the targeted unit's phasor section is not destroyed
						if (targetedUnit.GetComponent<PhasorSection> ().isDestroyed == false) {

							//resolve a hit to the phasor section
							PhasorAttackHitOnShip(attackingUnit, targetedUnit,shipSectionTargeted);

						}
						//the else condition is that the phasor section is already destroyed
						else {

							//resolve a miss
							PhasorAttackMissOnShip(attackingUnit,targetedUnit);

						}

					}
					//the else condition is that the targeted unit does not have a phasor section
					else {

						//resolve a miss
						PhasorAttackMissOnShip(attackingUnit,targetedUnit);

					}

					break;

				case ShipSectionTargeted.TorpedoSection:

					//check if the targeted unit has a torpedo section
					if (targetedUnit.GetComponent<TorpedoSection> () == true) {

						//check if the targeted unit's torpedo section is not destroyed
						if (targetedUnit.GetComponent<TorpedoSection> ().isDestroyed == false) {

							//resolve a hit to the torpedo section
							PhasorAttackHitOnShip(attackingUnit, targetedUnit,shipSectionTargeted);

						}
						//the else condition is that the torpedo section is already destroyed
						else {

							//resolve a miss
							PhasorAttackMissOnShip(attackingUnit,targetedUnit);

						}

					}
					//the else condition is that the targeted unit does not have a torpedo section
					else {

						//resolve a miss
						PhasorAttackMissOnShip(attackingUnit,targetedUnit);

					}

					break;

				case ShipSectionTargeted.StorageSection:

					//check if the targeted unit has a storage section
					if (targetedUnit.GetComponent<StorageSection> () == true) {

						//check if the targeted unit's storage section is not destroyed
						if (targetedUnit.GetComponent<StorageSection> ().isDestroyed == false) {

							//resolve a hit to the storage section
							PhasorAttackHitOnShip(attackingUnit, targetedUnit,shipSectionTargeted);

						}
						//the else condition is that the storage section is already destroyed
						else {

							//resolve a miss
							PhasorAttackMissOnShip(attackingUnit,targetedUnit);

						}

					}
					//the else condition is that the targeted unit does not have a storage section
					else {

						//resolve a miss
						PhasorAttackMissOnShip(attackingUnit,targetedUnit);

					}

					break;

				case ShipSectionTargeted.CrewSection:

					//check if the targeted unit has a crew section
					if (targetedUnit.GetComponent<CrewSection> () == true) {

						//check if the targeted unit's crew section is not destroyed
						if (targetedUnit.GetComponent<CrewSection> ().isDestroyed == false) {

							//resolve a hit to the crew section
							PhasorAttackHitOnShip(attackingUnit, targetedUnit,shipSectionTargeted);

						}
						//the else condition is that the crew section is already destroyed
						else {

							//resolve a miss
							PhasorAttackMissOnShip(attackingUnit,targetedUnit);

						}

					}
					//the else condition is that the targeted unit does not have a crew section
					else {

						//resolve a miss
						PhasorAttackMissOnShip(attackingUnit,targetedUnit);

					}

					break;

				case ShipSectionTargeted.EngineSection:

					//check if the targeted unit has a engine section
					if (targetedUnit.GetComponent<EngineSection> () == true) {

						//check if the targeted unit's engine section is not destroyed
						if (targetedUnit.GetComponent<EngineSection> ().isDestroyed == false) {

							//resolve a hit to the engine section
							PhasorAttackHitOnShip(attackingUnit, targetedUnit,shipSectionTargeted);

						}
						//the else condition is that the engine section is already destroyed
						else {

							//resolve a miss
							PhasorAttackMissOnShip(attackingUnit,targetedUnit);

						}

					}
					//the else condition is that the targeted unit does not have a engine section
					else {

						//resolve a miss
						PhasorAttackMissOnShip(attackingUnit,targetedUnit);

					}

					break;

				case ShipSectionTargeted.Miss:

					//resolve a miss
					PhasorAttackMissOnShip(attackingUnit,targetedUnit);

					break;

				default:

					//resolve a miss
					PhasorAttackMissOnShip(attackingUnit,targetedUnit);

					break;

				}


			}
			//the else condition is that the attack was targeted with radar
			else {

				//we need to convert the section targeted string to our enum
				switch (sectionTargeted){

				case "Phasor Section":
					shipSectionTargeted = ShipSectionTargeted.PhasorSection;
					break;
				case "Torpedo Section":
					shipSectionTargeted = ShipSectionTargeted.TorpedoSection;
					break;
				case "Storage Section":
					shipSectionTargeted = ShipSectionTargeted.StorageSection;
					break;
				case "Crew Section":
					shipSectionTargeted = ShipSectionTargeted.CrewSection;
					break;
				case "Engine Section":
					shipSectionTargeted = ShipSectionTargeted.EngineSection;
					break;
				default:
					Debug.LogError ("We have no section targeted despite using phaser radar");
					shipSectionTargeted = ShipSectionTargeted.Miss;
					break;

				}

				//we can call the resolve function immediately, since we know the targeted section is valid
				PhasorAttackHitOnShip(attackingUnit, targetedUnit,shipSectionTargeted);

			}


		}
		//the else condition is that the targeted unit is a starbase
		else if (targetedUnit.GetComponent<Starbase> () == true) {

			BaseSectionTargeted baseSectionTargeted;

			//this was a ship attacking a starbase.  Now we need to determine if it was a targeted attack or not
			//the sectionTargeted string will be "Choose Section" if it was a non-targeted attack
			if (sectionTargeted == "Choose Section") {

				//this was an untargeted attack.  We need to roll a die to determine which section is being attacked
				//we subtract one because the Dice function returns a number 1-6, and the enum has values 0-5 for the sections we can hit
				baseSectionTargeted = (BaseSectionTargeted)(Dice.DiceRollSum (1, 6) - 1);

				//we need to check if the random section targeted is valid, or if it is a miss
				switch (baseSectionTargeted) {

				case BaseSectionTargeted.PhasorSection1:

					//check if the targeted unit's phasor section 1 is not destroyed
					if (targetedUnit.GetComponent<StarbasePhasorSection1> ().isDestroyed == false) {

						//resolve a hit to the phasor section
						PhasorAttackHitOnBase (attackingUnit, targetedUnit, baseSectionTargeted);

					}
					//the else condition is that the phasor section 1 is already destroyed
					else {

						//resolve a miss
						PhasorAttackMissOnBase (attackingUnit, targetedUnit);

					}
		
					break;

				case BaseSectionTargeted.PhasorSection2:

					//check if the targeted unit's phasor section 2 is not destroyed
					if (targetedUnit.GetComponent<StarbasePhasorSection2> ().isDestroyed == false) {

						//resolve a hit to the phasor section
						PhasorAttackHitOnBase (attackingUnit, targetedUnit, baseSectionTargeted);

					}
					//the else condition is that the phasor section 2 is already destroyed
					else {

						//resolve a miss
						PhasorAttackMissOnBase (attackingUnit, targetedUnit);

					}

					break;

				case BaseSectionTargeted.TorpedoSection:

					//check if the targeted unit's torpedo section is not destroyed
					if (targetedUnit.GetComponent<StarbaseTorpedoSection> ().isDestroyed == false) {

						//resolve a hit to the torpedo section
						PhasorAttackHitOnBase (attackingUnit, targetedUnit, baseSectionTargeted);

					}
					//the else condition is that the torpedo section is already destroyed
					else {

						//resolve a miss
						PhasorAttackMissOnBase (attackingUnit, targetedUnit);

					}

					break;

				case BaseSectionTargeted.CrewSection:

					//check if the targeted unit's crew section is not destroyed
					if (targetedUnit.GetComponent<StarbaseCrewSection> ().isDestroyed == false) {

						//resolve a hit to the crew section
						PhasorAttackHitOnBase (attackingUnit, targetedUnit, baseSectionTargeted);

					}
					//the else condition is that the crew section is already destroyed
					else {

						//resolve a miss
						PhasorAttackMissOnBase (attackingUnit, targetedUnit);

					}

					break;

				case BaseSectionTargeted.StorageSection1:

					//check if the targeted unit's storage section 1 is not destroyed
					if (targetedUnit.GetComponent<StarbaseStorageSection1> ().isDestroyed == false) {

						//resolve a hit to the storage section 1
						PhasorAttackHitOnBase (attackingUnit, targetedUnit, baseSectionTargeted);

					}
					//the else condition is that the storage section 1 is already destroyed
					else {

						//resolve a miss
						PhasorAttackMissOnBase (attackingUnit, targetedUnit);

					}

					break;

				case BaseSectionTargeted.StorageSection2:

					//check if the targeted unit's storage section 2 is not destroyed
					if (targetedUnit.GetComponent<StarbaseStorageSection2> ().isDestroyed == false) {

						//resolve a hit to the storage section 2
						PhasorAttackHitOnBase (attackingUnit, targetedUnit, baseSectionTargeted);

					}
					//the else condition is that the storage section 2 is already destroyed
					else {

						//resolve a miss
						PhasorAttackMissOnBase (attackingUnit, targetedUnit);

					}

					break;

				

				case BaseSectionTargeted.Untargeted:

					//resolve a miss
					PhasorAttackMissOnBase (attackingUnit, targetedUnit);

					break;

				default:

					//resolve a miss
					PhasorAttackMissOnBase (attackingUnit, targetedUnit);

					break;

				}

			}
			//the else condition is that the attack was targeted with radar
			else {

				//we need to convert the section targeted string to our enum
				switch (sectionTargeted){

				case "Phasor Section 1":
					baseSectionTargeted = BaseSectionTargeted.PhasorSection1;
					break;
				case "Phasor Section 2":
					baseSectionTargeted = BaseSectionTargeted.PhasorSection2;
					break;
				case "Torpedo Section":
					baseSectionTargeted = BaseSectionTargeted.TorpedoSection;
					break;
				case "Crew Section":
					baseSectionTargeted = BaseSectionTargeted.CrewSection;
					break;
				case "Storage Section 1":
					baseSectionTargeted = BaseSectionTargeted.StorageSection1;
					break;
				case "Storage Section 2":
					baseSectionTargeted = BaseSectionTargeted.StorageSection2;
					break;
				default:
					Debug.LogError ("We have no section targeted despite using phaser radar");
					baseSectionTargeted = BaseSectionTargeted.Untargeted;
					break;

				}

				//we can call the resolve function immediately, since we know the targeted section is valid
				PhasorAttackHitOnBase(attackingUnit, targetedUnit,baseSectionTargeted);

			}

		}

	}

	//this function will resolve a light torpedo attack when one unit attacks another
	private void ResolveTorpedoAttack(CombatUnit attackingUnit, CombatUnit targetedUnit, string sectionTargeted, AttackType attackType){
			
		//next, check if the targeted unit is a ship or a starbase
		if (targetedUnit.GetComponent<Ship>() == true) {

			ShipSectionTargeted shipSectionTargeted;

			//this was a ship attacking a ship.  Now we need to determine if it was a targeted attack or not
			//the sectionTargeted string will be "Choose Section" if it was a non-targeted attack
			if (sectionTargeted == "Choose Section") {

				//this was an untargeted attack.  
				shipSectionTargeted = ShipSectionTargeted.Untargeted;

				//we need to determine if flares will be used to thwart the torpedo attack
				//check if targeted ship has a storage section
				if (targetedUnit.GetComponent<StorageSection> () == true) {

					//check if targeted ship has flares
					if (targetedUnit.GetComponent<StorageSection> ().flares > 0) {

						//determine the expected damage of the attack
						int numberDice = 0;
						if (attackingUnit.GetComponent<Ship> () == true) {

							numberDice = DetermineDice (attackType, attackingUnit, targetedUnit, GetAttackRange (attackingUnit, targetedUnit, attackingUnit.GetComponent<TorpedoSection> ().TargetableTorpedoHexes));

						}
						else if(attackingUnit.GetComponent<Starbase> () == true){

							numberDice = DetermineDice (attackType, attackingUnit, targetedUnit, GetAttackRange (attackingUnit, targetedUnit, attackingUnit.GetComponent<StarbaseTorpedoSection> ().TargetableTorpedoHexes));

						}

						//invoke flare event passing the attacking unit, targeted unit, targeted section, and expected damage
						if (attackType == AttackType.LightTorpedo) {
							
							OnLightTorpedoUntargetedAttackShipWithFlares.Invoke (attackingUnit, targetedUnit, shipSectionTargeted, numberDice);

						} else if (attackType == AttackType.HeavyTorpedo) {

							OnHeavyTorpedoUntargetedAttackShipWithFlares.Invoke (attackingUnit, targetedUnit, shipSectionTargeted, numberDice);

						}

						//after invoking the event, we can exit from this function
						return;

					}

				}

				//if we get to here, we did not have the flare event exit out of the function
				//the determine section hit call will finish determining section hit and invoking hit/miss events
				DetermineSectionHit (attackingUnit, targetedUnit, attackType);

			}
			//the else condition is that the attack was targeted with laser guidance
			else {

				//we need to convert the section targeted string to our enum
				switch (sectionTargeted){

				case "Phasor Section":
					shipSectionTargeted = ShipSectionTargeted.PhasorSection;
					break;
				case "Torpedo Section":
					shipSectionTargeted = ShipSectionTargeted.TorpedoSection;
					break;
				case "Storage Section":
					shipSectionTargeted = ShipSectionTargeted.StorageSection;
					break;
				case "Crew Section":
					shipSectionTargeted = ShipSectionTargeted.CrewSection;
					break;
				case "Engine Section":
					shipSectionTargeted = ShipSectionTargeted.EngineSection;
					break;
				default:
					Debug.LogError ("We have no section targeted despite using laser guidance");
					shipSectionTargeted = ShipSectionTargeted.Miss;
					break;

				}

				//we still need to check if the targeted ship has flares, and go through the flare usage event if necessary
				//check if targeted ship has a storage section
				if (targetedUnit.GetComponent<StorageSection> () == true) {

					//check if targeted ship has flares
					if (targetedUnit.GetComponent<StorageSection> ().flares > 0) {

						//determine the expected damage of the attack
						int numberDice = 0;
						if (attackingUnit.GetComponent<Ship> () == true) {

							numberDice = DetermineDice (attackType, attackingUnit, targetedUnit, GetAttackRange (attackingUnit, targetedUnit, attackingUnit.GetComponent<TorpedoSection> ().TargetableTorpedoHexes));

						}
						else if(attackingUnit.GetComponent<Starbase> () == true){

							numberDice = DetermineDice (attackType, attackingUnit, targetedUnit, GetAttackRange (attackingUnit, targetedUnit, attackingUnit.GetComponent<StarbaseTorpedoSection> ().TargetableTorpedoHexes));

						}

						//invoke targeted flare event passing the attacking unit, targeted unit, targeted section, and expected damage
						if (attackType == AttackType.LightTorpedo) {

							OnLightTorpedoTargetedAttackShipWithFlares.Invoke (attackingUnit, targetedUnit,shipSectionTargeted, numberDice);

						} else if (attackType == AttackType.HeavyTorpedo) {

							OnHeavyTorpedoTargetedAttackShipWithFlares.Invoke (attackingUnit, targetedUnit,shipSectionTargeted, numberDice);

						}

						//after invoking the event, we can exit from this function
						return;

					}

				}

				//if we are here, we did not exit the function for the flare event detour
				//we can call the resolve function immediately, since we know the targeted section is valid
				if (attackType == AttackType.LightTorpedo) {

					LightTorpedoAttackHitOnShip (attackingUnit, targetedUnit, shipSectionTargeted);

				} else if (attackType == AttackType.HeavyTorpedo) {

					HeavyTorpedoAttackHitOnShip (attackingUnit, targetedUnit, shipSectionTargeted);

				}

			}

		}
		//the else condition is that the targeted unit is a starbase
		else if(targetedUnit.GetComponent<Starbase>() == true) {

			BaseSectionTargeted baseSectionTargeted;

			//this was a ship attacking a base.  Now we need to determine if it was a targeted attack or not
			//the sectionTargeted string will be "Choose Section" if it was a non-targeted attack
			if (sectionTargeted == "Choose Section") {

				//this was an untargeted attack.  
				baseSectionTargeted = BaseSectionTargeted.Untargeted;

				//we need to determine if flares will be used to thwart the torpedo attack
				//check if targeted base has flares
				if (targetedUnit.GetComponent<StarbaseStorageSection2> ().flares > 0) {

					//determine the expected damage of the attack
					int numberDice = 0;
					if (attackingUnit.GetComponent<Ship> () == true) {

						numberDice = DetermineDice (attackType, attackingUnit, targetedUnit, GetAttackRange (attackingUnit, targetedUnit, attackingUnit.GetComponent<TorpedoSection> ().TargetableTorpedoHexes));

					}
					else if(attackingUnit.GetComponent<Starbase> () == true){

						numberDice = DetermineDice (attackType, attackingUnit, targetedUnit, GetAttackRange (attackingUnit, targetedUnit, attackingUnit.GetComponent<StarbaseTorpedoSection> ().TargetableTorpedoHexes));

					}

					//invoke flare event passing the attacking unit, targeted unit, targeted section, and expected damage
					if (attackType == AttackType.LightTorpedo) {

						OnLightTorpedoUntargetedAttackBaseWithFlares.Invoke (attackingUnit, targetedUnit, baseSectionTargeted, numberDice);

					} else if (attackType == AttackType.HeavyTorpedo) {

						OnHeavyTorpedoUntargetedAttackBaseWithFlares.Invoke (attackingUnit, targetedUnit, baseSectionTargeted, numberDice);

					}

					//after invoking the event, we can exit from this function
					return;

				}


				//if we get to here, we did not have the flare event exit out of the function
				//the determine section hit call will finish determining section hit and invoking hit/miss events
				DetermineSectionHit (attackingUnit, targetedUnit, attackType);

			}
			//the else condition is that the attack was targeted with laser guidance
			else {

				//we need to convert the section targeted string to our enum
				switch (sectionTargeted){

				case "Phasor Section 1":
					baseSectionTargeted = BaseSectionTargeted.PhasorSection1;
					break;
				case "Phasor Section 2":
					baseSectionTargeted = BaseSectionTargeted.PhasorSection2;
					break;
				case "Torpedo Section":
					baseSectionTargeted = BaseSectionTargeted.TorpedoSection;
					break;
				case "Crew Section":
					baseSectionTargeted = BaseSectionTargeted.CrewSection;
					break;
				case "Storage Section 1":
					baseSectionTargeted = BaseSectionTargeted.StorageSection1;
					break;
				case "Storage Section 2":
					baseSectionTargeted = BaseSectionTargeted.StorageSection2;
					break;
				default:
					Debug.LogError ("We have no section targeted despite using laser guidance");
					baseSectionTargeted = BaseSectionTargeted.Untargeted;
					break;

				}

				//we still need to check if the targeted ship has flares, and go through the flare usage event if necessary

				//check if targeted ship has flares
				if (targetedUnit.GetComponent<StarbaseStorageSection2> ().flares > 0) {

					//determine the expected damage of the attack
					int numberDice = 0;
					if (attackingUnit.GetComponent<Ship> () == true) {
						
						numberDice = DetermineDice (attackType, attackingUnit, targetedUnit, GetAttackRange (attackingUnit, targetedUnit, attackingUnit.GetComponent<TorpedoSection> ().TargetableTorpedoHexes));

					}
					else if(attackingUnit.GetComponent<Starbase> () == true){
					
						numberDice = DetermineDice (attackType, attackingUnit, targetedUnit, GetAttackRange (attackingUnit, targetedUnit, attackingUnit.GetComponent<StarbaseTorpedoSection> ().TargetableTorpedoHexes));
					
					}

					//invoke targeted flare event passing the attacking unit, targeted unit, targeted section, and expected damage
					if (attackType == AttackType.LightTorpedo) {

						OnLightTorpedoTargetedAttackBaseWithFlares.Invoke (attackingUnit, targetedUnit,baseSectionTargeted, numberDice);

					} else if (attackType == AttackType.HeavyTorpedo) {

						OnHeavyTorpedoTargetedAttackBaseWithFlares.Invoke (attackingUnit, targetedUnit,baseSectionTargeted, numberDice);

					}

					//after invoking the event, we can exit from this function
					return;

				}


				//if we are here, we did not exit the function for the flare event detour
				//we can call the resolve function immediately, since we know the targeted section is valid
				if (attackType == AttackType.LightTorpedo) {

					LightTorpedoAttackHitOnBase (attackingUnit, targetedUnit, baseSectionTargeted);

				} else if (attackType == AttackType.HeavyTorpedo) {

					HeavyTorpedoAttackHitOnBase (attackingUnit, targetedUnit, baseSectionTargeted);

				}

			}

		}

	}

	//this function will determine which section an attack hits, and invokes the appropriate event
	private void DetermineSectionHit(CombatUnit attackingUnit, CombatUnit targetedUnit, AttackType attackType){

		if (targetedUnit.GetComponent<Ship> () == true) {
			
			ShipSectionTargeted shipSectionTargeted;

			//We need to roll a die to determine which section is being attacked
			//we subtract one because the Dice function returns a number 1-6, and the enum has values 0-5
			shipSectionTargeted = (ShipSectionTargeted)(Dice.DiceRollSum (1, 6) - 1);

			//we need to check if the random section targeted is valid, or if it is a miss
			switch (shipSectionTargeted) {

			case ShipSectionTargeted.PhasorSection:

			//check if the targeted unit has a phasor section
				if (targetedUnit.GetComponent<PhasorSection> () == true) {

					//check if the targeted unit's phasor section is not destroyed
					if (targetedUnit.GetComponent<PhasorSection> ().isDestroyed == false) {

						//resolve a hit to the phasor section
						if (attackType == AttackType.LightTorpedo) {
						
							LightTorpedoAttackHitOnShip (attackingUnit, targetedUnit, shipSectionTargeted);

						} else if (attackType == AttackType.HeavyTorpedo) {

							HeavyTorpedoAttackHitOnShip (attackingUnit, targetedUnit, shipSectionTargeted);

						}

					}
				//the else condition is that the phasor section is already destroyed
				else {

						//resolve a miss
						if (attackType == AttackType.LightTorpedo) {
						
							LightTorpedoAttackMissOnShip (attackingUnit, targetedUnit);

						} else if (attackType == AttackType.HeavyTorpedo) {

							HeavyTorpedoAttackMissOnShip (attackingUnit, targetedUnit);

						}

					}

				}
			//the else condition is that the targeted unit does not have a phasor section
			else {

					//resolve a miss
					if (attackType == AttackType.LightTorpedo) {

						LightTorpedoAttackMissOnShip (attackingUnit, targetedUnit);

					} else if (attackType == AttackType.HeavyTorpedo) {

						HeavyTorpedoAttackMissOnShip (attackingUnit, targetedUnit);

					}

				}

				break;

			case ShipSectionTargeted.TorpedoSection:

			//check if the targeted unit has a torpedo section
				if (targetedUnit.GetComponent<TorpedoSection> () == true) {

					//check if the targeted unit's torpedo section is not destroyed
					if (targetedUnit.GetComponent<TorpedoSection> ().isDestroyed == false) {

						//resolve a hit to the torpedo section
						if (attackType == AttackType.LightTorpedo) {

							LightTorpedoAttackHitOnShip (attackingUnit, targetedUnit, shipSectionTargeted);

						} else if (attackType == AttackType.HeavyTorpedo) {

							HeavyTorpedoAttackHitOnShip (attackingUnit, targetedUnit, shipSectionTargeted);

						}

					}
				//the else condition is that the torpedo section is already destroyed
				else {

						//resolve a miss
						if (attackType == AttackType.LightTorpedo) {

							LightTorpedoAttackMissOnShip (attackingUnit, targetedUnit);

						} else if (attackType == AttackType.HeavyTorpedo) {

							HeavyTorpedoAttackMissOnShip (attackingUnit, targetedUnit);

						}

					}

				}
			//the else condition is that the targeted unit does not have a torpedo section
			else {

					//resolve a miss
					if (attackType == AttackType.LightTorpedo) {

						LightTorpedoAttackMissOnShip (attackingUnit, targetedUnit);

					} else if (attackType == AttackType.HeavyTorpedo) {

						HeavyTorpedoAttackMissOnShip (attackingUnit, targetedUnit);

					}

				}

				break;

			case ShipSectionTargeted.StorageSection:

			//check if the targeted unit has a storage section
				if (targetedUnit.GetComponent<StorageSection> () == true) {

					//check if the targeted unit's storage section is not destroyed
					if (targetedUnit.GetComponent<StorageSection> ().isDestroyed == false) {

						//resolve a hit to the storage section
						if (attackType == AttackType.LightTorpedo) {

							LightTorpedoAttackHitOnShip (attackingUnit, targetedUnit, shipSectionTargeted);

						} else if (attackType == AttackType.HeavyTorpedo) {

							HeavyTorpedoAttackHitOnShip (attackingUnit, targetedUnit, shipSectionTargeted);

						}

					}
				//the else condition is that the storage section is already destroyed
				else {

						//resolve a miss
						if (attackType == AttackType.LightTorpedo) {

							LightTorpedoAttackMissOnShip (attackingUnit, targetedUnit);

						} else if (attackType == AttackType.HeavyTorpedo) {

							HeavyTorpedoAttackMissOnShip (attackingUnit, targetedUnit);

						}

					}

				}
			//the else condition is that the targeted unit does not have a storage section
			else {

					//resolve a miss
					if (attackType == AttackType.LightTorpedo) {

						LightTorpedoAttackMissOnShip (attackingUnit, targetedUnit);

					} else if (attackType == AttackType.HeavyTorpedo) {

						HeavyTorpedoAttackMissOnShip (attackingUnit, targetedUnit);

					}

				}

				break;

			case ShipSectionTargeted.CrewSection:

			//check if the targeted unit has a crew section
				if (targetedUnit.GetComponent<CrewSection> () == true) {

					//check if the targeted unit's crew section is not destroyed
					if (targetedUnit.GetComponent<CrewSection> ().isDestroyed == false) {

						//resolve a hit to the crew section
						if (attackType == AttackType.LightTorpedo) {

							LightTorpedoAttackHitOnShip (attackingUnit, targetedUnit, shipSectionTargeted);

						} else if (attackType == AttackType.HeavyTorpedo) {

							HeavyTorpedoAttackHitOnShip (attackingUnit, targetedUnit, shipSectionTargeted);

						}

					}
				//the else condition is that the crew section is already destroyed
				else {
					
						//resolve a miss
						if (attackType == AttackType.LightTorpedo) {

							LightTorpedoAttackMissOnShip (attackingUnit, targetedUnit);

						} else if (attackType == AttackType.HeavyTorpedo) {

							HeavyTorpedoAttackMissOnShip (attackingUnit, targetedUnit);

						}

					}

				}
			//the else condition is that the targeted unit does not have a crew section
			else {

					//resolve a miss
					if (attackType == AttackType.LightTorpedo) {

						LightTorpedoAttackMissOnShip (attackingUnit, targetedUnit);

					} else if (attackType == AttackType.HeavyTorpedo) {

						HeavyTorpedoAttackMissOnShip (attackingUnit, targetedUnit);

					}

				}

				break;

			case ShipSectionTargeted.EngineSection:

			//check if the targeted unit has a engine section
				if (targetedUnit.GetComponent<EngineSection> () == true) {

					//check if the targeted unit's engine section is not destroyed
					if (targetedUnit.GetComponent<EngineSection> ().isDestroyed == false) {

						//resolve a hit to the engine section
						if (attackType == AttackType.LightTorpedo) {

							LightTorpedoAttackHitOnShip (attackingUnit, targetedUnit, shipSectionTargeted);

						} else if (attackType == AttackType.HeavyTorpedo) {

							HeavyTorpedoAttackHitOnShip (attackingUnit, targetedUnit, shipSectionTargeted);

						}

					}
				//the else condition is that the engine section is already destroyed
				else {

						//resolve a miss
						if (attackType == AttackType.LightTorpedo) {

							LightTorpedoAttackMissOnShip (attackingUnit, targetedUnit);

						} else if (attackType == AttackType.HeavyTorpedo) {

							HeavyTorpedoAttackMissOnShip (attackingUnit, targetedUnit);

						}

					}

				}
			//the else condition is that the targeted unit does not have a engine section
			else {

					//resolve a miss
					if (attackType == AttackType.LightTorpedo) {

						LightTorpedoAttackMissOnShip (attackingUnit, targetedUnit);

					} else if (attackType == AttackType.HeavyTorpedo) {

						HeavyTorpedoAttackMissOnShip (attackingUnit, targetedUnit);

					}

				}

				break;

			case ShipSectionTargeted.Miss:

			//resolve a miss
				if (attackType == AttackType.LightTorpedo) {

					LightTorpedoAttackMissOnShip (attackingUnit, targetedUnit);

				} else if (attackType == AttackType.HeavyTorpedo) {

					HeavyTorpedoAttackMissOnShip (attackingUnit, targetedUnit);

				}

				break;

			default:

			//resolve a miss
				if (attackType == AttackType.LightTorpedo) {

					LightTorpedoAttackMissOnShip (attackingUnit, targetedUnit);

				} else if (attackType == AttackType.HeavyTorpedo) {

					HeavyTorpedoAttackMissOnShip (attackingUnit, targetedUnit);

				}

				break;

			}

		}
		//the else condition is that targeted unit is a starbase
		else if (targetedUnit.GetComponent<Starbase> () == true){

			BaseSectionTargeted baseSectionTargeted;

			//We need to roll a die to determine which section is being attacked
			//we subtract one because the Dice function returns a number 1-6, and the enum has values 0-5
			baseSectionTargeted = (BaseSectionTargeted)(Dice.DiceRollSum (1, 6) - 1);

			//we need to check if the random section targeted is valid, or if it is a miss
			switch (baseSectionTargeted) {

			case BaseSectionTargeted.PhasorSection1:

				//check if the targeted unit's phasor section is not destroyed
				if (targetedUnit.GetComponent<StarbasePhasorSection1> ().isDestroyed == false) {

					//resolve a hit to the phasor section
					if (attackType == AttackType.LightTorpedo) {

						LightTorpedoAttackHitOnBase (attackingUnit, targetedUnit, baseSectionTargeted);

					} else if (attackType == AttackType.HeavyTorpedo) {

						HeavyTorpedoAttackHitOnBase (attackingUnit, targetedUnit, baseSectionTargeted);

					}

				}
				//the else condition is that the phasor section is already destroyed
				else {

					//resolve a miss
					if (attackType == AttackType.LightTorpedo) {

						LightTorpedoAttackMissOnBase (attackingUnit, targetedUnit);

					} else if (attackType == AttackType.HeavyTorpedo) {

						HeavyTorpedoAttackMissOnBase (attackingUnit, targetedUnit);

					}

				}

				break;


			case BaseSectionTargeted.PhasorSection2:

				//check if the targeted unit's phasor section is not destroyed
				if (targetedUnit.GetComponent<StarbasePhasorSection2> ().isDestroyed == false) {

					//resolve a hit to the phasor section
					if (attackType == AttackType.LightTorpedo) {

						LightTorpedoAttackHitOnBase (attackingUnit, targetedUnit, baseSectionTargeted);

					} else if (attackType == AttackType.HeavyTorpedo) {

						HeavyTorpedoAttackHitOnBase (attackingUnit, targetedUnit, baseSectionTargeted);

					}

				}
				//the else condition is that the phasor section is already destroyed
				else {

					//resolve a miss
					if (attackType == AttackType.LightTorpedo) {

						LightTorpedoAttackMissOnBase (attackingUnit, targetedUnit);

					} else if (attackType == AttackType.HeavyTorpedo) {

						HeavyTorpedoAttackMissOnBase (attackingUnit, targetedUnit);

					}

				}

				break;

			case BaseSectionTargeted.TorpedoSection:

				//check if the targeted unit's torpedo section is not destroyed
				if (targetedUnit.GetComponent<StarbaseTorpedoSection> ().isDestroyed == false) {

					//resolve a hit to the torpedo section
					if (attackType == AttackType.LightTorpedo) {

						LightTorpedoAttackHitOnBase (attackingUnit, targetedUnit, baseSectionTargeted);

					} else if (attackType == AttackType.HeavyTorpedo) {

						HeavyTorpedoAttackHitOnBase (attackingUnit, targetedUnit, baseSectionTargeted);

					}

				}
				//the else condition is that the torpedo section is already destroyed
				else {

					//resolve a miss
					if (attackType == AttackType.LightTorpedo) {

						LightTorpedoAttackMissOnBase (attackingUnit, targetedUnit);

					} else if (attackType == AttackType.HeavyTorpedo) {

						HeavyTorpedoAttackMissOnBase (attackingUnit, targetedUnit);

					}

				}

				break;

			case BaseSectionTargeted.CrewSection:

				//check if the targeted unit's crew section is not destroyed
				if (targetedUnit.GetComponent<StarbaseCrewSection> ().isDestroyed == false) {

					//resolve a hit to the crew section
					if (attackType == AttackType.LightTorpedo) {

						LightTorpedoAttackHitOnBase (attackingUnit, targetedUnit, baseSectionTargeted);

					} else if (attackType == AttackType.HeavyTorpedo) {

						HeavyTorpedoAttackHitOnBase (attackingUnit, targetedUnit, baseSectionTargeted);

					}

				}
				//the else condition is that the crew section is already destroyed
				else {

					//resolve a miss
					if (attackType == AttackType.LightTorpedo) {

						LightTorpedoAttackMissOnBase (attackingUnit, targetedUnit);

					} else if (attackType == AttackType.HeavyTorpedo) {

						HeavyTorpedoAttackMissOnBase (attackingUnit, targetedUnit);

					}

				}

				break;

			case BaseSectionTargeted.StorageSection1:

				//check if the targeted unit's storage section is not destroyed
				if (targetedUnit.GetComponent<StarbaseStorageSection1> ().isDestroyed == false) {

					//resolve a hit to the storage section
					if (attackType == AttackType.LightTorpedo) {

						LightTorpedoAttackHitOnBase (attackingUnit, targetedUnit, baseSectionTargeted);

					} else if (attackType == AttackType.HeavyTorpedo) {

						HeavyTorpedoAttackHitOnBase (attackingUnit, targetedUnit, baseSectionTargeted);

					}

				}
				//the else condition is that the storage section is already destroyed
				else {

					//resolve a miss
					if (attackType == AttackType.LightTorpedo) {

						LightTorpedoAttackMissOnBase (attackingUnit, targetedUnit);

					} else if (attackType == AttackType.HeavyTorpedo) {

						HeavyTorpedoAttackMissOnBase (attackingUnit, targetedUnit);

					}

				}

				break;

			case BaseSectionTargeted.StorageSection2:

				//check if the targeted unit's storage section is not destroyed
				if (targetedUnit.GetComponent<StarbaseStorageSection2> ().isDestroyed == false) {

					//resolve a hit to the storage section
					if (attackType == AttackType.LightTorpedo) {

						LightTorpedoAttackHitOnBase (attackingUnit, targetedUnit, baseSectionTargeted);

					} else if (attackType == AttackType.HeavyTorpedo) {

						HeavyTorpedoAttackHitOnBase (attackingUnit, targetedUnit, baseSectionTargeted);

					}

				}
				//the else condition is that the storage section is already destroyed
				else {

					//resolve a miss
					if (attackType == AttackType.LightTorpedo) {

						LightTorpedoAttackMissOnBase (attackingUnit, targetedUnit);

					} else if (attackType == AttackType.HeavyTorpedo) {

						HeavyTorpedoAttackMissOnBase (attackingUnit, targetedUnit);

					}

				}

				break;
					
			default:

				//resolve a miss
				if (attackType == AttackType.LightTorpedo) {

					LightTorpedoAttackMissOnBase (attackingUnit, targetedUnit);

				} else if (attackType == AttackType.HeavyTorpedo) {

					HeavyTorpedoAttackMissOnBase (attackingUnit, targetedUnit);

				}

				break;

			}

		}

	}
		
	//this function resolves a phasor attack hit on a ship
	private void PhasorAttackHitOnShip(CombatUnit attackingUnit, CombatUnit targetedUnit, ShipSectionTargeted shipSectionTargeted){
		
		//get the range between the attacking and targeted units
		int attackRange = 0;
		if (attackingUnit.GetComponent<Ship> () == true) {
			
			attackRange = GetAttackRange (attackingUnit, targetedUnit, attackingUnit.GetComponent<PhasorSection> ().TargetablePhasorHexes);

		} else if (attackingUnit.GetComponent<Starbase> () == true) {

			attackRange = GetAttackRange (attackingUnit, targetedUnit, attackingUnit.GetComponent<StarbasePhasorSection1> ().TargetablePhasorHexes);

		}

		//determine how many dice should be rolled for attack
		int numberDice = DetermineDice (AttackType.Phasor, attackingUnit, targetedUnit, attackRange);

		//roll the dice and get a damage total
		int phasorDamage = Dice.DiceRollSum(numberDice,numberSidedDice);

		//invoke the appropriate event based on the section targeted
		switch (shipSectionTargeted) {

		case ShipSectionTargeted.PhasorSection:
			OnPhasorAttackHitShipPhasorSection.Invoke (attackingUnit, targetedUnit, phasorDamage);
			break;
		case ShipSectionTargeted.TorpedoSection:
			OnPhasorAttackHitShipTorpedoSection.Invoke (attackingUnit, targetedUnit, phasorDamage);
			break;
		case ShipSectionTargeted.StorageSection:
			OnPhasorAttackHitShipStorageSection.Invoke (attackingUnit, targetedUnit, phasorDamage);
			break;
		case ShipSectionTargeted.CrewSection:
			OnPhasorAttackHitShipCrewSection.Invoke (attackingUnit, targetedUnit, phasorDamage);
			break;
		case ShipSectionTargeted.EngineSection:
			OnPhasorAttackHitShipEngineSection.Invoke (attackingUnit, targetedUnit, phasorDamage);
			break;
		default:
			break;

		}

	}

	//this function resolves a phasor attack hit on a base
	private void PhasorAttackHitOnBase(CombatUnit attackingUnit, CombatUnit targetedUnit, BaseSectionTargeted baseSectionTargeted){

		//get the range between the attacking and targeted units
		int attackRange = 0;
		if (attackingUnit.GetComponent<Ship> () == true) {

			attackRange = GetAttackRange (attackingUnit, targetedUnit, attackingUnit.GetComponent<PhasorSection> ().TargetablePhasorHexes);

		} else if (attackingUnit.GetComponent<Starbase> () == true) {

			attackRange = GetAttackRange (attackingUnit, targetedUnit, attackingUnit.GetComponent<StarbasePhasorSection1> ().TargetablePhasorHexes);

		}

		//determine how many dice should be rolled for attack
		int numberDice = DetermineDice (AttackType.Phasor, attackingUnit, targetedUnit, attackRange);

		//roll the dice and get a damage total
		int phasorDamage = Dice.DiceRollSum(numberDice,numberSidedDice);

		//invoke the appropriate event based on the section targeted
		switch (baseSectionTargeted) {

		case BaseSectionTargeted.PhasorSection1:
			OnPhasorAttackHitBasePhasorSection1.Invoke (attackingUnit, targetedUnit, phasorDamage);
			break;
		case BaseSectionTargeted.PhasorSection2:
			OnPhasorAttackHitBasePhasorSection2.Invoke (attackingUnit, targetedUnit, phasorDamage);
			break;
		case BaseSectionTargeted.TorpedoSection:
			OnPhasorAttackHitBaseTorpedoSection.Invoke (attackingUnit, targetedUnit, phasorDamage);
			break;
		case BaseSectionTargeted.CrewSection:
			OnPhasorAttackHitBaseCrewSection.Invoke (attackingUnit, targetedUnit, phasorDamage);
			break;
		case BaseSectionTargeted.StorageSection1:
			OnPhasorAttackHitBaseStorageSection1.Invoke (attackingUnit, targetedUnit, phasorDamage);
			break;
		case BaseSectionTargeted.StorageSection2:
			OnPhasorAttackHitBaseStorageSection2.Invoke (attackingUnit, targetedUnit, phasorDamage);
			break;
		default:
			break;

		}

	}

	//this function resolves a phasor attack miss on a ship
	private void PhasorAttackMissOnShip(CombatUnit attackingUnit, CombatUnit targetedUnit){

		//on a miss, we just need to fire off a miss event
		OnPhasorAttackMissShip.Invoke(attackingUnit,targetedUnit);

	}

	//this function resolves a phasor attack miss on a base
	private void PhasorAttackMissOnBase(CombatUnit attackingUnit, CombatUnit targetedUnit){

		//on a miss, we just need to fire off a miss event
		OnPhasorAttackMissBase.Invoke(attackingUnit,targetedUnit);

	}

	//this function resolves a light torpedo attack hit on a ship
	private void LightTorpedoAttackHitOnShip(CombatUnit attackingUnit, CombatUnit targetedUnit, ShipSectionTargeted shipSectionTargeted){


		//get the range between the attacking and targeted units
		int attackRange = 0;

		if (attackingUnit.GetComponent<Ship> () == true) {
			
			attackRange = GetAttackRange (attackingUnit, targetedUnit, attackingUnit.GetComponent<TorpedoSection> ().TargetableTorpedoHexes);

		}
		else if(attackingUnit.GetComponent<Starbase> () == true) {

			attackRange = GetAttackRange (attackingUnit, targetedUnit, attackingUnit.GetComponent<StarbaseTorpedoSection> ().TargetableTorpedoHexes);

		}

		//determine how many dice should be rolled for attack
		int numberDice = DetermineDice (AttackType.LightTorpedo, attackingUnit, targetedUnit, attackRange);

		//roll the dice and get a damage total
		int torpedoDamage = Dice.DiceRollSum(numberDice,numberSidedDice);

		//invoke the appropriate event based on the section targeted
		switch (shipSectionTargeted) {

		case ShipSectionTargeted.PhasorSection:
			OnLightTorpedoAttackHitShipPhasorSection.Invoke (attackingUnit, targetedUnit, torpedoDamage);
			break;
		case ShipSectionTargeted.TorpedoSection:
			OnLightTorpedoAttackHitShipTorpedoSection.Invoke (attackingUnit, targetedUnit, torpedoDamage);
			break;
		case ShipSectionTargeted.StorageSection:
			OnLightTorpedoAttackHitShipStorageSection.Invoke (attackingUnit, targetedUnit, torpedoDamage);
			break;
		case ShipSectionTargeted.CrewSection:
			OnLightTorpedoAttackHitShipCrewSection.Invoke (attackingUnit, targetedUnit, torpedoDamage);
			break;
		case ShipSectionTargeted.EngineSection:
			OnLightTorpedoAttackHitShipEngineSection.Invoke (attackingUnit, targetedUnit, torpedoDamage);
			break;
		default:
			break;

		}

	}

	//this function resolves a light torpedo attack hit on a base
	private void LightTorpedoAttackHitOnBase(CombatUnit attackingUnit, CombatUnit targetedUnit, BaseSectionTargeted baseSectionTargeted){


		//get the range between the attacking and targeted units
		int attackRange = 0;

		if (attackingUnit.GetComponent<Ship> () == true) {

			attackRange = GetAttackRange (attackingUnit, targetedUnit, attackingUnit.GetComponent<TorpedoSection> ().TargetableTorpedoHexes);

		}
		else if(attackingUnit.GetComponent<Starbase> () == true) {

			attackRange = GetAttackRange (attackingUnit, targetedUnit, attackingUnit.GetComponent<StarbaseTorpedoSection> ().TargetableTorpedoHexes);

		}
		//determine how many dice should be rolled for attack
		int numberDice = DetermineDice (AttackType.LightTorpedo, attackingUnit, targetedUnit, attackRange);

		//roll the dice and get a damage total
		int torpedoDamage = Dice.DiceRollSum(numberDice,numberSidedDice);

		//invoke the appropriate event based on the section targeted
		switch (baseSectionTargeted) {

		case BaseSectionTargeted.PhasorSection1:
			OnLightTorpedoAttackHitBasePhasorSection1.Invoke (attackingUnit, targetedUnit, torpedoDamage);
			break;
		case BaseSectionTargeted.PhasorSection2:
			OnLightTorpedoAttackHitBasePhasorSection2.Invoke (attackingUnit, targetedUnit, torpedoDamage);
			break;
		case BaseSectionTargeted.TorpedoSection:
			OnLightTorpedoAttackHitBaseTorpedoSection.Invoke (attackingUnit, targetedUnit, torpedoDamage);
			break;
		case BaseSectionTargeted.CrewSection:
			OnLightTorpedoAttackHitBaseCrewSection.Invoke (attackingUnit, targetedUnit, torpedoDamage);
			break;
		case BaseSectionTargeted.StorageSection1:
			OnLightTorpedoAttackHitBaseStorageSection1.Invoke (attackingUnit, targetedUnit, torpedoDamage);
			break;
		case BaseSectionTargeted.StorageSection2:
			OnLightTorpedoAttackHitBaseStorageSection2.Invoke (attackingUnit, targetedUnit, torpedoDamage);
			break;
		default:
			break;

		}

	}

	//this function resolves a light torpedo attack miss
	private void LightTorpedoAttackMissOnShip(CombatUnit attackingUnit, CombatUnit targetedUnit){

		//on a miss, we just need to fire off a miss event
		OnLightTorpedoAttackMissShip.Invoke(attackingUnit,targetedUnit);

	}

	//this function resolves a light torpedo attack miss
	private void LightTorpedoAttackMissOnBase(CombatUnit attackingUnit, CombatUnit targetedUnit){

		//on a miss, we just need to fire off a miss event
		OnLightTorpedoAttackMissBase.Invoke(attackingUnit,targetedUnit);

	}

	//this function resolves a heavy torpedo attack hit on a ship
	private void HeavyTorpedoAttackHitOnShip(CombatUnit attackingUnit, CombatUnit targetedUnit, ShipSectionTargeted shipSectionTargeted){


		//get the range between the attacking and targeted units
		int attackRange = 0;

		if (attackingUnit.GetComponent<Ship> () == true) {

			attackRange = GetAttackRange (attackingUnit, targetedUnit, attackingUnit.GetComponent<TorpedoSection> ().TargetableTorpedoHexes);

		}
		else if(attackingUnit.GetComponent<Starbase> () == true) {

			attackRange = GetAttackRange (attackingUnit, targetedUnit, attackingUnit.GetComponent<StarbaseTorpedoSection> ().TargetableTorpedoHexes);

		}
		//determine how many dice should be rolled for attack
		int numberDice = DetermineDice (AttackType.HeavyTorpedo, attackingUnit, targetedUnit, attackRange);

		//roll the dice and get a damage total
		int torpedoDamage = Dice.DiceRollSum(numberDice,numberSidedDice);

		//invoke the appropriate event based on the section targeted
		switch (shipSectionTargeted) {

		case ShipSectionTargeted.PhasorSection:
			OnHeavyTorpedoAttackHitShipPhasorSection.Invoke (attackingUnit, targetedUnit, torpedoDamage);
			break;
		case ShipSectionTargeted.TorpedoSection:
			OnHeavyTorpedoAttackHitShipTorpedoSection.Invoke (attackingUnit, targetedUnit, torpedoDamage);
			break;
		case ShipSectionTargeted.StorageSection:
			OnHeavyTorpedoAttackHitShipStorageSection.Invoke (attackingUnit, targetedUnit, torpedoDamage);
			break;
		case ShipSectionTargeted.CrewSection:
			OnHeavyTorpedoAttackHitShipCrewSection.Invoke (attackingUnit, targetedUnit, torpedoDamage);
			break;
		case ShipSectionTargeted.EngineSection:
			OnHeavyTorpedoAttackHitShipEngineSection.Invoke (attackingUnit, targetedUnit, torpedoDamage);
			break;
		default:
			break;

		}

	}

	//this function resolves a heavy torpedo attack hit on a base
	private void HeavyTorpedoAttackHitOnBase(CombatUnit attackingUnit, CombatUnit targetedUnit, BaseSectionTargeted baseSectionTargeted){
		
		//get the range between the attacking and targeted units
		int attackRange = 0;

		if (attackingUnit.GetComponent<Ship> () == true) {

			attackRange = GetAttackRange (attackingUnit, targetedUnit, attackingUnit.GetComponent<TorpedoSection> ().TargetableTorpedoHexes);

		}
		else if(attackingUnit.GetComponent<Starbase> () == true) {

			attackRange = GetAttackRange (attackingUnit, targetedUnit, attackingUnit.GetComponent<StarbaseTorpedoSection> ().TargetableTorpedoHexes);

		}
		//determine how many dice should be rolled for attack
		int numberDice = DetermineDice (AttackType.HeavyTorpedo, attackingUnit, targetedUnit, attackRange);

		//roll the dice and get a damage total
		int torpedoDamage = Dice.DiceRollSum(numberDice,numberSidedDice);

		//invoke the appropriate event based on the section targeted
		switch (baseSectionTargeted) {

		case BaseSectionTargeted.PhasorSection1:
			OnHeavyTorpedoAttackHitBasePhasorSection1.Invoke (attackingUnit, targetedUnit, torpedoDamage);
			break;
		case BaseSectionTargeted.PhasorSection2:
			OnHeavyTorpedoAttackHitBasePhasorSection2.Invoke (attackingUnit, targetedUnit, torpedoDamage);
			break;
		case BaseSectionTargeted.TorpedoSection:
			OnHeavyTorpedoAttackHitBaseTorpedoSection.Invoke (attackingUnit, targetedUnit, torpedoDamage);
			break;
		case BaseSectionTargeted.CrewSection:
			OnHeavyTorpedoAttackHitBaseCrewSection.Invoke (attackingUnit, targetedUnit, torpedoDamage);
			break;
		case BaseSectionTargeted.StorageSection1:
			OnHeavyTorpedoAttackHitBaseStorageSection1.Invoke (attackingUnit, targetedUnit, torpedoDamage);
			break;
		case BaseSectionTargeted.StorageSection2:
			OnHeavyTorpedoAttackHitBaseStorageSection2.Invoke (attackingUnit, targetedUnit, torpedoDamage);
			break;
		default:
			break;

		}

	}

	//this function resolves a heavy torpedo attack miss
	private void HeavyTorpedoAttackMissOnShip(CombatUnit attackingUnit, CombatUnit targetedUnit){

		//on a miss, we just need to fire off a miss event
		OnHeavyTorpedoAttackMissShip.Invoke(attackingUnit,targetedUnit);

	}

	//this function resolves a heavy torpedo attack miss
	private void HeavyTorpedoAttackMissOnBase(CombatUnit attackingUnit, CombatUnit targetedUnit){

		//on a miss, we just need to fire off a miss event
		OnHeavyTorpedoAttackMissBase.Invoke(attackingUnit,targetedUnit);

	}

	//this function is called after the flare prompt is responded to by the player
	private void ResolveTorpedoAttackAfterFlares(FlareManager.FlareEventData flareEventData, bool flaresUsed){

		//check if flares were used or not
		if (flaresUsed == true) {

			//check if flares stop torpedo
			bool flareSuccess = DoFlaresStopTorpedo(flareEventData.numberFlaresUsed);

			//check if flare was successful
			if (flareSuccess == true) {

				//check if it was a light or heavy torpedo
				if (flareEventData.flareTorpedoType == FlareManager.FlareTorpedoType.Light) {

					OnLightTorpedoAttackDefeatedByFlares.Invoke (flareEventData.attackingUnit, flareEventData.targetedUnit,flareEventData.shipSectionTargeted,flareEventData.numberFlaresUsed);


				}
				else if (flareEventData.flareTorpedoType == FlareManager.FlareTorpedoType.Heavy) {

					OnHeavyTorpedoAttackDefeatedByFlares.Invoke (flareEventData.attackingUnit, flareEventData.targetedUnit,flareEventData.shipSectionTargeted, flareEventData.numberFlaresUsed);

				}

			} else if (flareSuccess == false) {

				//if flares were not successful, we need to fire off an event to announce the flare miss
				//check if it was a light or heavy torpedo
				if (flareEventData.flareTorpedoType == FlareManager.FlareTorpedoType.Light) {

					OnLightTorpedoAttackFlaresFailed.Invoke (flareEventData.attackingUnit, flareEventData.targetedUnit, flareEventData.numberFlaresUsed);

				}
				else if (flareEventData.flareTorpedoType == FlareManager.FlareTorpedoType.Heavy) {

					OnHeavyTorpedoAttackFlaresFailed.Invoke (flareEventData.attackingUnit, flareEventData.targetedUnit, flareEventData.numberFlaresUsed);

				}

				//now that we've announced the flare misses, we need to resolve the attack, depending on whether it was targeted or untargeted
				//check if the attack was targeted
				if (flareEventData.shipSectionTargeted == ShipSectionTargeted.Untargeted) {

					//convert flareEventData flareTorpedoType to AttackType
					AttackType flareAttackType;

					if (flareEventData.flareTorpedoType == FlareManager.FlareTorpedoType.Light) {

						flareAttackType = AttackType.LightTorpedo;

					}
					//the else condition is that it is a heavy torpedo
					else {

						flareAttackType = AttackType.HeavyTorpedo;

					}

					//call the DetermineSectionHit function to complete determining the section hit and calling hit events
					DetermineSectionHit (flareEventData.attackingUnit, flareEventData.targetedUnit, flareAttackType);
				
				//the else condition is that the shipSectionTargeted is anything else - this means it was a targeted attack
				} else {

					//check if we have a ship or a starbase as the targeted unit
					if (flareEventData.targetedUnit.GetComponent<Ship> () == true) {

						//check if we have a light torpedo or a heavy torpedo
						if (flareEventData.flareTorpedoType == FlareManager.FlareTorpedoType.Light) {

							LightTorpedoAttackHitOnShip (flareEventData.attackingUnit, flareEventData.targetedUnit, flareEventData.shipSectionTargeted);

						} else if (flareEventData.flareTorpedoType == FlareManager.FlareTorpedoType.Heavy) {

							HeavyTorpedoAttackHitOnShip (flareEventData.attackingUnit, flareEventData.targetedUnit, flareEventData.shipSectionTargeted);

						}

					}
					//the else condition is that we have a starbase targeted
					else if (flareEventData.targetedUnit.GetComponent<Starbase> () == true) {

						//check if we have a light torpedo or a heavy torpedo
						if (flareEventData.flareTorpedoType == FlareManager.FlareTorpedoType.Light) {

							//we can cast the flareEvent shipSectionTargeted as a baseSection targeted, because in the FlareManager we had cast
							//the baseSectionTargeted as a ShipSectionTargeted to begin with
							LightTorpedoAttackHitOnBase (flareEventData.attackingUnit, flareEventData.targetedUnit, (BaseSectionTargeted)flareEventData.shipSectionTargeted);

						} else if (flareEventData.flareTorpedoType == FlareManager.FlareTorpedoType.Heavy) {

							//we can cast the flareEvent shipSectionTargeted as a baseSection targeted, because in the FlareManager we had cast
							//the baseSectionTargeted as a ShipSectionTargeted to begin with
							HeavyTorpedoAttackHitOnBase (flareEventData.attackingUnit, flareEventData.targetedUnit, (BaseSectionTargeted)flareEventData.shipSectionTargeted);

						}

					} //close else if targeted unit was a starbase

				}  //close else that it was a targeted attack
											
			}  //close else the flare failed to stop the torpedo

		} //close the if flares were used
		//the else condition is that flares were not used
		else if (flaresUsed == false) {

			//if flares were not used, we can send this to the appropriate function depending on whether it was a targeted attack or not
			//check if the attack was targeted
			if (flareEventData.shipSectionTargeted == ShipSectionTargeted.Untargeted) {

				//convert flareEventData flareTorpedoType to AttackType
				AttackType flareAttackType;

				if (flareEventData.flareTorpedoType == FlareManager.FlareTorpedoType.Light) {

					flareAttackType = AttackType.LightTorpedo;

				}
				//the else condition is that it is a heavy torpedo
				else {
					
					flareAttackType = AttackType.HeavyTorpedo;

				}

				//call the DetermineSectionHit function to complete determining the section hit and calling hit events
				DetermineSectionHit (flareEventData.attackingUnit, flareEventData.targetedUnit, flareAttackType);

				//the else condition is that the shipSectionTargeted is anything else - this means it was a targeted attack
			} else {

				//check if we have a ship or a starbase as the targeted unit
				if (flareEventData.targetedUnit.GetComponent<Ship> () == true) {

					//check if we have a light torpedo or a heavy torpedo
					if (flareEventData.flareTorpedoType == FlareManager.FlareTorpedoType.Light) {

						LightTorpedoAttackHitOnShip (flareEventData.attackingUnit, flareEventData.targetedUnit, flareEventData.shipSectionTargeted);

					} else if (flareEventData.flareTorpedoType == FlareManager.FlareTorpedoType.Heavy) {

						HeavyTorpedoAttackHitOnShip (flareEventData.attackingUnit, flareEventData.targetedUnit, flareEventData.shipSectionTargeted);

					}

				}
				//the else condition is that we have a starbase targeted
				else if (flareEventData.targetedUnit.GetComponent<Starbase> () == true) {

					//check if we have a light torpedo or a heavy torpedo
					if (flareEventData.flareTorpedoType == FlareManager.FlareTorpedoType.Light) {

						//we can cast the flareEvent shipSectionTargeted as a baseSection targeted, because in the FlareManager we had cast
						//the baseSectionTargeted as a ShipSectionTargeted to begin with
						LightTorpedoAttackHitOnBase (flareEventData.attackingUnit, flareEventData.targetedUnit, (BaseSectionTargeted)flareEventData.shipSectionTargeted);

					} else if (flareEventData.flareTorpedoType == FlareManager.FlareTorpedoType.Heavy) {

						//we can cast the flareEvent shipSectionTargeted as a baseSection targeted, because in the FlareManager we had cast
						//the baseSectionTargeted as a ShipSectionTargeted to begin with
						HeavyTorpedoAttackHitOnBase (flareEventData.attackingUnit, flareEventData.targetedUnit, (BaseSectionTargeted)flareEventData.shipSectionTargeted);

					}

				} //close else if targeted unit was a starbase

			}  //close else that it was a targeted attack

		} //close the else flares were not used

	} //close the function

	//this function will determine if flares are successful in thwarting the torpedo or not
	private bool DoFlaresStopTorpedo(int numberFlares){

		//odds of an individual flare stopping a torpedo are 1/numberSidedFlareDice
		//need at least 1 stop to stop the torpedo
		bool flareSuccess = Dice.DiceRollContains(numberFlares,numberSidedFlareDice,1);

		return flareSuccess;
	
	}

	//this function will determine the range between 2 units
	private int GetAttackRange(CombatUnit attackingUnit, CombatUnit targetedUnit, List<Hex> attackRangeHexes){

		//the attack range is a movement path from the attacking unit to the targeting unit, but first I need to temporarily make the targeted unit passable
		OnStartTargetPath.Invoke(targetedUnit);

		//generate the path, now that the target hex is valid for pathfinding
		//the path length includes the starting and ending hexes
		int pathlength = tileMap.GeneratePath(attackingUnit.currentLocation, targetedUnit.currentLocation, attackRangeHexes).Count;

		//invoke the end target path event, so the targeted unit hex can be marked impassable again
		OnEndTargetPath.Invoke(targetedUnit);

		//we want to return a range that is distance from the starting hex, so it is one less than the path length
		return pathlength - 1;

	}

	//this function determines how many dice should be rolled for an attack
	private int DetermineDice(AttackType attackType, CombatUnit attackingUnit, CombatUnit targetedUnit, int attackRange){

		int numberDice = 0;

		//check if it is a torpedo attack or a phasor attack
		if (attackType == AttackType.Phasor) {

			//check if the attacking unit is a ship or a starbase
			if (attackingUnit.GetComponent<Ship> () == true) {

				//check if the range is in the primary cone
				if (attackRange == 1) {

					//check if the attacking unit has the X-ray upgrade
					if (attackingUnit.GetComponent<PhasorSection> ().xRayKernalUpgrade == true) {

						//this is an attack with x-Ray upgrade in the primary cone
						numberDice = numberDiceXRayPrimaryCone;

					}
					//the else condition is that it is regular phasors
					else {

						//this is an attack with standard phasors in the primary cone
						numberDice = numberDicePhasorPrimaryCone;

					}

				}
				//else check if the range is in the secondary cone
				else if (attackRange == 2) {

					//check if the attacking unit has the X-ray upgrade
					if (attackingUnit.GetComponent<PhasorSection> ().xRayKernalUpgrade == true) {

						//this is an attack with x-Ray upgrade in the secondary cone
						numberDice = numberDiceXRaySecondaryCone;

					}
					//the else condition is that it is regular phasors
					else {

						//this is an attack with standard phasors in the secondary cone
						numberDice = numberDicePhasorSecondaryCone;

					}

				}
				//it should not be possible to be here with a range besides 1 or 2
				else {

					Debug.LogError ("Phasor Attack has a range besides 1 or 2");
					numberDice = 0;

				}

			}

			//the else condition is a starbase
			else if (attackingUnit.GetComponent<Starbase> () == true){

				//check if the range is in the primary cone
				if (attackRange == 1) {

					//check if the attacking unit has the X-ray upgrade
					if (attackingUnit.GetComponent<StarbasePhasorSection2> ().xRayKernalUpgrade == true) {

						//this is an attack with x-Ray upgrade in the primary cone
						numberDice = numberDiceXRayPrimaryCone;

					}
					//the else condition is that it is regular phasors
					else {

						//this is an attack with standard phasors in the primary cone
						numberDice = numberDicePhasorPrimaryCone;

					}

				}
				//else check if the range is in the secondary cone
				else if (attackRange == 2) {

					//check if the attacking unit has the X-ray upgrade
					if (attackingUnit.GetComponent<StarbasePhasorSection2> ().xRayKernalUpgrade == true) {

						//this is an attack with x-Ray upgrade in the secondary cone
						numberDice = numberDiceXRaySecondaryCone;

					}
					//the else condition is that it is regular phasors
					else {

						//this is an attack with standard phasors in the secondary cone
						numberDice = numberDicePhasorSecondaryCone;

					}

				}
				//it should not be possible to be here with a range besides 1 or 2
				else {

					Debug.LogError ("Phasor Attack has a range besides 1 or 2");
					numberDice = 0;

				}
			}

		} else if (attackType == AttackType.LightTorpedo) {


			//check if the attacking unit is a ship or a starbase
			if (attackingUnit.GetComponent<Ship> () == true) {

				//check if the attacking ship has high pressure tubes
				if (attackingUnit.GetComponent<TorpedoSection> ().highPressureTubes == true) {

					//check if the range is in the primary cone
					if (attackRange == 1) {

						//this is an attack with light torpedo in the primary cone with high pressure tubes
						numberDice = numberDiceLightTorpedoPrimaryConeWithTubes;


					}
					//else check if the range is in the secondary cone
					else if (attackRange == 2) {

						//this is an attack with light torpedo in the secondary cone with high pressure tubes
						numberDice = numberDiceLightTorpedoSecondaryConeWithTubes;


					} else if (attackRange == 3) {

						//this is an attack with light torpedo in the tertiary cone with high pressure tubes
						numberDice = numberDiceLightTorpedoTertiaryConeWithTubes;

					}
					//it should not be possible to be here with a range besides 1,2, or 3
					else {

						Debug.LogError ("Light Torpedo Attack has a range besides 1, 2, or 3 with high pressure tubes");
						numberDice = 0;

					}

				}
				//the else condition is that the attacking ship does not have high-pressure tubes
				else {

					//check if the range is in the primary cone
					if (attackRange == 1) {

						//this is an attack with light torpedo in the primary cone without high pressure tubes
						numberDice = numberDiceLightTorpedoPrimaryCone;


					}
					//else check if the range is in the secondary cone
					else if (attackRange == 2) {

						//this is an attack with light torpedo in the secondary cone without high pressure tubes
						numberDice = numberDiceLightTorpedoSecondaryCone;


					} 
					//it should not be possible to be here with a range besides 1 or 2
					else {

						Debug.LogError ("Light Torpedo Attack has a range besides 1 or 2 without high pressure tubes");
						numberDice = 0;

					}

				}

			}

			//the else condition is a starbase
			else if (attackingUnit.GetComponent<Starbase> () == true) {

				//check if the attacking ship has high pressure tubes
				if (attackingUnit.GetComponent<StarbaseTorpedoSection> ().highPressureTubes == true) {

					//check if the range is in the primary cone
					if (attackRange == 1) {

						//this is an attack with light torpedo in the primary cone with high pressure tubes
						numberDice = numberDiceLightTorpedoPrimaryConeWithTubes;


					}
					//else check if the range is in the secondary cone
					else if (attackRange == 2) {

						//this is an attack with light torpedo in the secondary cone with high pressure tubes
						numberDice = numberDiceLightTorpedoSecondaryConeWithTubes;


					} else if (attackRange == 3) {

						//this is an attack with light torpedo in the tertiary cone with high pressure tubes
						numberDice = numberDiceLightTorpedoTertiaryConeWithTubes;

					}
					//it should not be possible to be here with a range besides 1,2, or 3
					else {

						Debug.LogError ("Light Torpedo Attack has a range besides 1, 2, or 3 with high pressure tubes");
						numberDice = 0;

					}

				}
				//the else condition is that the attacking ship does not have high-pressure tubes
				else {

					//check if the range is in the primary cone
					if (attackRange == 1) {

						//this is an attack with light torpedo in the primary cone without high pressure tubes
						numberDice = numberDiceLightTorpedoPrimaryCone;


					}
					//else check if the range is in the secondary cone
					else if (attackRange == 2) {

						//this is an attack with light torpedo in the secondary cone without high pressure tubes
						numberDice = numberDiceLightTorpedoSecondaryCone;


					} 
					//it should not be possible to be here with a range besides 1 or 2
					else {

						Debug.LogError ("Light Torpedo Attack has a range besides 1 or 2 without high pressure tubes");
						numberDice = 0;

					}

				}


			}


		} else if (attackType == AttackType.HeavyTorpedo) {


			//check if the attacking unit is a ship or a starbase
			if (attackingUnit.GetComponent<Ship> () == true) {

				//check if the attacking ship has high pressure tubes
				if (attackingUnit.GetComponent<TorpedoSection> ().highPressureTubes == true) {

					//check if the range is in the primary cone
					if (attackRange == 1) {

						//this is an attack with heavy torpedo in the primary cone with high pressure tubes
						numberDice = numberDiceHeavyTorpedoPrimaryConeWithTubes;


					}
					//else check if the range is in the secondary cone
					else if (attackRange == 2) {

						//this is an attack with heavy torpedo in the secondary cone with high pressure tubes
						numberDice = numberDiceHeavyTorpedoSecondaryConeWithTubes;


					} else if (attackRange == 3) {

						//this is an attack with heavy torpedo in the tertiary cone with high pressure tubes
						numberDice = numberDiceHeavyTorpedoTertiaryConeWithTubes;

					}
					//it should not be possible to be here with a range besides 1,2, or 3
					else {

						Debug.LogError ("Heavy Torpedo Attack has a range besides 1, 2, or 3 with high pressure tubes");
						numberDice = 0;

					}

				}
				//the else condition is that the attacking ship does not have high-pressure tubes
				else {

					//check if the range is in the primary cone
					if (attackRange == 1) {

						//this is an attack with heavy torpedo in the primary cone without high pressure tubes
						numberDice = numberDiceHeavyTorpedoPrimaryCone;


					}
					//else check if the range is in the secondary cone
					else if (attackRange == 2) {

						//this is an attack with heavy torpedo in the secondary cone without high pressure tubes
						numberDice = numberDiceHeavyTorpedoSecondaryCone;


					} 
					//it should not be possible to be here with a range besides 1 or 2
					else {

						Debug.LogError ("Heavy Torpedo Attack has a range besides 1 or 2 without high pressure tubes");
						numberDice = 0;

					}

				}

			}

			//the else condition is a starbase
			else if (attackingUnit.GetComponent<Starbase> () == true) {

				//check if the attacking ship has high pressure tubes
				if (attackingUnit.GetComponent<StarbaseTorpedoSection> ().highPressureTubes == true) {

					//check if the range is in the primary cone
					if (attackRange == 1) {

						//this is an attack with heavy torpedo in the primary cone with high pressure tubes
						numberDice = numberDiceHeavyTorpedoPrimaryConeWithTubes;


					}
					//else check if the range is in the secondary cone
					else if (attackRange == 2) {

						//this is an attack with heavy torpedo in the secondary cone with high pressure tubes
						numberDice = numberDiceHeavyTorpedoSecondaryConeWithTubes;


					} else if (attackRange == 3) {

						//this is an attack with heavy torpedo in the tertiary cone with high pressure tubes
						numberDice = numberDiceHeavyTorpedoTertiaryConeWithTubes;

					}
					//it should not be possible to be here with a range besides 1,2, or 3
					else {

						Debug.LogError ("Heavy Torpedo Attack has a range besides 1, 2, or 3 with high pressure tubes");
						numberDice = 0;

					}

				}
				//the else condition is that the attacking ship does not have high-pressure tubes
				else {

					//check if the range is in the primary cone
					if (attackRange == 1) {

						//this is an attack with heavy torpedo in the primary cone without high pressure tubes
						numberDice = numberDiceHeavyTorpedoPrimaryCone;


					}
					//else check if the range is in the secondary cone
					else if (attackRange == 2) {

						//this is an attack with heavy torpedo in the secondary cone without high pressure tubes
						numberDice = numberDiceHeavyTorpedoSecondaryCone;


					} 
					//it should not be possible to be here with a range besides 1 or 2
					else {

						Debug.LogError ("Heavy Torpedo Attack has a range besides 1 or 2 without high pressure tubes");
						numberDice = 0;

					}

				}

			}

		}
		//we need to check if the defending unit has a shield engineering team, which will reduce the attacking dice by 1
		if (targetedUnit.GetComponent<Ship> () == true) {

			//check if the ship has a crew section
			if (targetedUnit.GetComponent<CrewSection> () == true) {

				//check if the crew section has a shield engineering team
				if (targetedUnit.GetComponent<CrewSection> ().shieldEngineeringTeam == true) {

					//need to make sure attack dice doesn't become negative
					if (numberDice != 0) {

						//reduce the number of dice by 1
						numberDice -= 1;

					}

				}

			}
				
		}
		//the else condition is that the targeted unit is a base
		else if (targetedUnit.GetComponent<Starbase> () == true)  {

			//check if the crew section has a shield engineering team
			if (targetedUnit.GetComponent<StarbaseCrewSection> ().shieldEngineeringTeam == true) {

				//need to make sure attack dice doesn't become negative
				if (numberDice != 0) {

					//reduce the number of dice by 1
					numberDice -= 1;

				}

			}

		}

		return numberDice;

	}

	//this function will resolve the use of a crystal by one ship on another
	private void ResolveCrystal(CombatUnit selectedUnit, CombatUnit targetedUnit, string shipSectionTargetedString, CrystalType crystalType){

		//next, check if the targeted unit is a ship or a starbase
		if (targetedUnit.GetComponent<Ship> () == true) {

			ShipSectionTargeted shipSectionTargeted;

			//this was a ship using a crystal on a ship
			//we need to convert the section targeted string to our enum
			switch (shipSectionTargetedString){

			case "Phasor Section":
				shipSectionTargeted = ShipSectionTargeted.PhasorSection;
				break;
			case "Torpedo Section":
				shipSectionTargeted = ShipSectionTargeted.TorpedoSection;
				break;
			case "Storage Section":
				shipSectionTargeted = ShipSectionTargeted.StorageSection;
				break;
			case "Crew Section":
				shipSectionTargeted = ShipSectionTargeted.CrewSection;
				break;
			case "Engine Section":
				shipSectionTargeted = ShipSectionTargeted.EngineSection;
				break;
			default:
				Debug.LogError ("We have no section targeted despite picking from the crystal dropdown");
				shipSectionTargeted = ShipSectionTargeted.Miss;
				break;

			}

			//we can call the resolve function since we know the targeted section is valid
			CrystalUsedOnShip(selectedUnit, targetedUnit,shipSectionTargeted,crystalType);

		}
		//the else condition is that the targeted unit is a starbase
		else if (targetedUnit.GetComponent<Starbase> () == true){

			BaseSectionTargeted baseSectionTargeted;

			//this was a unit using a crystal on a starbase
			//we need to convert the section targeted string to our enum
			switch (shipSectionTargetedString){

			case "Phasor Section 1":
				baseSectionTargeted = BaseSectionTargeted.PhasorSection1;
				break;
			case "Phasor Section 2":
				baseSectionTargeted = BaseSectionTargeted.PhasorSection2;
				break;
			case "Torpedo Section":
				baseSectionTargeted = BaseSectionTargeted.TorpedoSection;
				break;
			case "Crew Section":
				baseSectionTargeted = BaseSectionTargeted.CrewSection;
				break;
			case "Storage Section 1":
				baseSectionTargeted = BaseSectionTargeted.StorageSection1;
				break;
			case "Storage Section 2":
				baseSectionTargeted = BaseSectionTargeted.StorageSection2;
				break;
			default:
				Debug.LogError ("We have no base section targeted despite picking from the crystal dropdown");
				baseSectionTargeted = BaseSectionTargeted.Untargeted;
				break;

			}

			//we can call the resolve function since we know the targeted section is valid
			CrystalUsedOnBase(selectedUnit, targetedUnit,baseSectionTargeted,crystalType);

		}

	}

	//this function resolves a crystal use on a ship
	private void CrystalUsedOnShip(CombatUnit selectedUnit, CombatUnit targetedUnit, ShipSectionTargeted shipSectionTargeted, CrystalType crystalType){

		//define integer to hold the shields healed
		int shieldsHealed = 0;

		//invoke the appropriate event based on the section targeted
		switch (shipSectionTargeted) {

		case ShipSectionTargeted.PhasorSection:

			//calculate the shields healed
			if (crystalType == CrystalType.Dilithium) {

				//check if the targeted section has taken more damage than the crystal can heal
				if (targetedUnit.GetComponent<PhasorSection> ().shieldsMax - targetedUnit.GetComponent<PhasorSection> ().shieldsCurrent > dilithiumHealing) {

					shieldsHealed = dilithiumHealing;

				}
				//the else condition is that the section has taken less damage than a full crystal's worth.  In this case, the crystal fills the section up
				else {

					shieldsHealed = targetedUnit.GetComponent<PhasorSection> ().shieldsMax - targetedUnit.GetComponent<PhasorSection> ().shieldsCurrent;

				}
			
			}
			//the else condition is that the crystal type is a trilithium crystal
			else {

				//for a trilithium crystal, it always heals to full
				shieldsHealed = targetedUnit.GetComponent<PhasorSection> ().shieldsMax - targetedUnit.GetComponent<PhasorSection> ().shieldsCurrent;

			}

			OnCrystalUsedOnShipPhasorSection.Invoke (selectedUnit, targetedUnit, crystalType, shieldsHealed);

			break;

		case ShipSectionTargeted.TorpedoSection:
			
			//calculate the shields healed
			if (crystalType == CrystalType.Dilithium) {

				//check if the targeted section has taken more damage than the crystal can heal
				if (targetedUnit.GetComponent<TorpedoSection> ().shieldsMax - targetedUnit.GetComponent<TorpedoSection> ().shieldsCurrent > dilithiumHealing) {

					shieldsHealed = dilithiumHealing;

				}
				//the else condition is that the section has taken less damage than a full crystal's worth.  In this case, the crystal fills the section up
				else {

					shieldsHealed = targetedUnit.GetComponent<TorpedoSection> ().shieldsMax - targetedUnit.GetComponent<TorpedoSection> ().shieldsCurrent;

				}

			}
			//the else condition is that the crystal type is a trilithium crystal
			else {

				//for a trilithium crystal, it always heals to full
				shieldsHealed = targetedUnit.GetComponent<TorpedoSection> ().shieldsMax - targetedUnit.GetComponent<TorpedoSection> ().shieldsCurrent;

			}

			OnCrystalUsedOnShipTorpedoSection.Invoke (selectedUnit, targetedUnit, crystalType, shieldsHealed);

			break;

		case ShipSectionTargeted.StorageSection:

			//calculate the shields healed
			if (crystalType == CrystalType.Dilithium) {

				//check if the targeted section has taken more damage than the crystal can heal
				if (targetedUnit.GetComponent<StorageSection> ().shieldsMax - targetedUnit.GetComponent<StorageSection> ().shieldsCurrent > dilithiumHealing) {

					shieldsHealed = dilithiumHealing;

				}
				//the else condition is that the section has taken less damage than a full crystal's worth.  In this case, the crystal fills the section up
				else {

					shieldsHealed = targetedUnit.GetComponent<StorageSection> ().shieldsMax - targetedUnit.GetComponent<StorageSection> ().shieldsCurrent;

				}

			}
			//the else condition is that the crystal type is a trilithium crystal
			else {

				//for a trilithium crystal, it always heals to full
				shieldsHealed = targetedUnit.GetComponent<StorageSection> ().shieldsMax - targetedUnit.GetComponent<StorageSection> ().shieldsCurrent;

			}

			OnCrystalUsedOnShipStorageSection.Invoke (selectedUnit, targetedUnit, crystalType, shieldsHealed);

			break;

		case ShipSectionTargeted.CrewSection:

			//calculate the shields healed
			if (crystalType == CrystalType.Dilithium) {

				//check if the targeted section has taken more damage than the crystal can heal
				if (targetedUnit.GetComponent<CrewSection> ().shieldsMax - targetedUnit.GetComponent<CrewSection> ().shieldsCurrent > dilithiumHealing) {

					shieldsHealed = dilithiumHealing;

				}
				//the else condition is that the section has taken less damage than a full crystal's worth.  In this case, the crystal fills the section up
				else {

					shieldsHealed = targetedUnit.GetComponent<CrewSection> ().shieldsMax - targetedUnit.GetComponent<CrewSection> ().shieldsCurrent;

				}

			}
			//the else condition is that the crystal type is a trilithium crystal
			else {

				//for a trilithium crystal, it always heals to full
				shieldsHealed = targetedUnit.GetComponent<CrewSection> ().shieldsMax - targetedUnit.GetComponent<CrewSection> ().shieldsCurrent;

			}

			OnCrystalUsedOnShipCrewSection.Invoke (selectedUnit, targetedUnit, crystalType, shieldsHealed);
		
			break;

		case ShipSectionTargeted.EngineSection:

			//calculate the shields healed
			if (crystalType == CrystalType.Dilithium) {

				//check if the targeted section has taken more damage than the crystal can heal
				if (targetedUnit.GetComponent<EngineSection> ().shieldsMax - targetedUnit.GetComponent<EngineSection> ().shieldsCurrent > dilithiumHealing) {

					shieldsHealed = dilithiumHealing;

				}
				//the else condition is that the section has taken less damage than a full crystal's worth.  In this case, the crystal fills the section up
				else {

					shieldsHealed = targetedUnit.GetComponent<EngineSection> ().shieldsMax - targetedUnit.GetComponent<EngineSection> ().shieldsCurrent;

				}

			}
			//the else condition is that the crystal type is a trilithium crystal
			else {

				//for a trilithium crystal, it always heals to full
				shieldsHealed = targetedUnit.GetComponent<EngineSection> ().shieldsMax - targetedUnit.GetComponent<EngineSection> ().shieldsCurrent;

			}

			OnCrystalUsedOnShipEngineSection.Invoke (selectedUnit, targetedUnit, crystalType, shieldsHealed);

			break;

		default:
			break;

		}

	}

	//this function resolves a crystal use on a base
	private void CrystalUsedOnBase(CombatUnit selectedUnit, CombatUnit targetedUnit, BaseSectionTargeted baseSectionTargeted, CrystalType crystalType){

		//define integer to hold the shields healed
		int shieldsHealed = 0;

		//invoke the appropriate event based on the section targeted
		switch (baseSectionTargeted) {

		case BaseSectionTargeted.PhasorSection1:

			//calculate the shields healed
			if (crystalType == CrystalType.Dilithium) {

				//check if the targeted section has taken more damage than the crystal can heal
				if (targetedUnit.GetComponent<StarbasePhasorSection1> ().shieldsMax - targetedUnit.GetComponent<StarbasePhasorSection1> ().shieldsCurrent > dilithiumHealing) {

					shieldsHealed = dilithiumHealing;

				}
				//the else condition is that the section has taken less damage than a full crystal's worth.  In this case, the crystal fills the section up
				else {

					shieldsHealed = targetedUnit.GetComponent<StarbasePhasorSection1> ().shieldsMax - targetedUnit.GetComponent<StarbasePhasorSection1> ().shieldsCurrent;

				}

			}
			//the else condition is that the crystal type is a trilithium crystal
			else {

				//for a trilithium crystal, it always heals to full
				shieldsHealed = targetedUnit.GetComponent<StarbasePhasorSection1> ().shieldsMax - targetedUnit.GetComponent<StarbasePhasorSection1> ().shieldsCurrent;

			}

			OnCrystalUsedOnBasePhasorSection1.Invoke (selectedUnit, targetedUnit, crystalType, shieldsHealed);

			break;

		case BaseSectionTargeted.PhasorSection2:

			//calculate the shields healed
			if (crystalType == CrystalType.Dilithium) {

				//check if the targeted section has taken more damage than the crystal can heal
				if (targetedUnit.GetComponent<StarbasePhasorSection2> ().shieldsMax - targetedUnit.GetComponent<StarbasePhasorSection2> ().shieldsCurrent > dilithiumHealing) {

					shieldsHealed = dilithiumHealing;

				}
				//the else condition is that the section has taken less damage than a full crystal's worth.  In this case, the crystal fills the section up
				else {

					shieldsHealed = targetedUnit.GetComponent<StarbasePhasorSection2> ().shieldsMax - targetedUnit.GetComponent<StarbasePhasorSection2> ().shieldsCurrent;

				}

			}
			//the else condition is that the crystal type is a trilithium crystal
			else {

				//for a trilithium crystal, it always heals to full
				shieldsHealed = targetedUnit.GetComponent<StarbasePhasorSection2> ().shieldsMax - targetedUnit.GetComponent<StarbasePhasorSection2> ().shieldsCurrent;

			}

			OnCrystalUsedOnBasePhasorSection2.Invoke (selectedUnit, targetedUnit, crystalType, shieldsHealed);

			break;

		case BaseSectionTargeted.TorpedoSection:

			//calculate the shields healed
			if (crystalType == CrystalType.Dilithium) {

				//check if the targeted section has taken more damage than the crystal can heal
				if (targetedUnit.GetComponent<StarbaseTorpedoSection> ().shieldsMax - targetedUnit.GetComponent<StarbaseTorpedoSection> ().shieldsCurrent > dilithiumHealing) {

					shieldsHealed = dilithiumHealing;

				}
				//the else condition is that the section has taken less damage than a full crystal's worth.  In this case, the crystal fills the section up
				else {

					shieldsHealed = targetedUnit.GetComponent<StarbaseTorpedoSection> ().shieldsMax - targetedUnit.GetComponent<StarbaseTorpedoSection> ().shieldsCurrent;

				}

			}
			//the else condition is that the crystal type is a trilithium crystal
			else {

				//for a trilithium crystal, it always heals to full
				shieldsHealed = targetedUnit.GetComponent<StarbaseTorpedoSection> ().shieldsMax - targetedUnit.GetComponent<StarbaseTorpedoSection> ().shieldsCurrent;

			}

			OnCrystalUsedOnBaseTorpedoSection.Invoke (selectedUnit, targetedUnit, crystalType, shieldsHealed);

			break;

		case BaseSectionTargeted.CrewSection:

			//calculate the shields healed
			if (crystalType == CrystalType.Dilithium) {

				//check if the targeted section has taken more damage than the crystal can heal
				if (targetedUnit.GetComponent<StarbaseCrewSection> ().shieldsMax - targetedUnit.GetComponent<StarbaseCrewSection> ().shieldsCurrent > dilithiumHealing) {

					shieldsHealed = dilithiumHealing;

				}
				//the else condition is that the section has taken less damage than a full crystal's worth.  In this case, the crystal fills the section up
				else {

					shieldsHealed = targetedUnit.GetComponent<StarbaseCrewSection> ().shieldsMax - targetedUnit.GetComponent<StarbaseCrewSection> ().shieldsCurrent;

				}

			}
			//the else condition is that the crystal type is a trilithium crystal
			else {

				//for a trilithium crystal, it always heals to full
				shieldsHealed = targetedUnit.GetComponent<StarbaseCrewSection> ().shieldsMax - targetedUnit.GetComponent<StarbaseCrewSection> ().shieldsCurrent;

			}

			OnCrystalUsedOnBaseCrewSection.Invoke (selectedUnit, targetedUnit, crystalType, shieldsHealed);

			break;

		case BaseSectionTargeted.StorageSection1:

			//calculate the shields healed
			if (crystalType == CrystalType.Dilithium) {

				//check if the targeted section has taken more damage than the crystal can heal
				if (targetedUnit.GetComponent<StarbaseStorageSection1> ().shieldsMax - targetedUnit.GetComponent<StarbaseStorageSection1> ().shieldsCurrent > dilithiumHealing) {

					shieldsHealed = dilithiumHealing;

				}
				//the else condition is that the section has taken less damage than a full crystal's worth.  In this case, the crystal fills the section up
				else {

					shieldsHealed = targetedUnit.GetComponent<StarbaseStorageSection1> ().shieldsMax - targetedUnit.GetComponent<StarbaseStorageSection1> ().shieldsCurrent;

				}

			}
			//the else condition is that the crystal type is a trilithium crystal
			else {

				//for a trilithium crystal, it always heals to full
				shieldsHealed = targetedUnit.GetComponent<StarbaseStorageSection1> ().shieldsMax - targetedUnit.GetComponent<StarbaseStorageSection1> ().shieldsCurrent;

			}

			OnCrystalUsedOnBaseStorageSection1.Invoke (selectedUnit, targetedUnit, crystalType, shieldsHealed);

			break;

		case BaseSectionTargeted.StorageSection2:

			//calculate the shields healed
			if (crystalType == CrystalType.Dilithium) {

				//check if the targeted section has taken more damage than the crystal can heal
				if (targetedUnit.GetComponent<StarbaseStorageSection2> ().shieldsMax - targetedUnit.GetComponent<StarbaseStorageSection2> ().shieldsCurrent > dilithiumHealing) {

					shieldsHealed = dilithiumHealing;

				}
				//the else condition is that the section has taken less damage than a full crystal's worth.  In this case, the crystal fills the section up
				else {

					shieldsHealed = targetedUnit.GetComponent<StarbaseStorageSection2> ().shieldsMax - targetedUnit.GetComponent<StarbaseStorageSection2> ().shieldsCurrent;

				}

			}
			//the else condition is that the crystal type is a trilithium crystal
			else {

				//for a trilithium crystal, it always heals to full
				shieldsHealed = targetedUnit.GetComponent<StarbaseStorageSection2> ().shieldsMax - targetedUnit.GetComponent<StarbaseStorageSection2> ().shieldsCurrent;

			}

			OnCrystalUsedOnBaseStorageSection2.Invoke (selectedUnit, targetedUnit, crystalType, shieldsHealed);

			break;

		default:
			break;

		}

	}

	//this function will resolve the use of a repair crew by one ship on another
	private void ResolveRepair(CombatUnit selectedUnit, CombatUnit targetedUnit, string shipSectionTargetedString){

		//next, check if the targeted unit is a ship or a starbase
		if (targetedUnit.GetComponent<Ship> () == true) {

			ShipSectionTargeted shipSectionTargeted;

			//this was a ship using a repair crew on a ship
			//we need to convert the section targeted string to our enum
			switch (shipSectionTargetedString){

			case "Phasor Section":
				shipSectionTargeted = ShipSectionTargeted.PhasorSection;
				break;
			case "Torpedo Section":
				shipSectionTargeted = ShipSectionTargeted.TorpedoSection;
				break;
			case "Storage Section":
				shipSectionTargeted = ShipSectionTargeted.StorageSection;
				break;
			case "Crew Section":
				shipSectionTargeted = ShipSectionTargeted.CrewSection;
				break;
			case "Engine Section":
				shipSectionTargeted = ShipSectionTargeted.EngineSection;
				break;
			default:
				Debug.LogError ("We have no section targeted despite picking from the repair dropdown");
				shipSectionTargeted = ShipSectionTargeted.Miss;
				break;

			}

			//we can call the resolve function since we know the targeted section is valid
			RepairCrewUsedOnShip(selectedUnit, targetedUnit,shipSectionTargeted);

		}
		//the else condition is that the targeted unit is a starbase
		else if (targetedUnit.GetComponent<Starbase> () == true){

			BaseSectionTargeted baseSectionTargeted;

			//this was a unit using a crystal on a starbase
			//we need to convert the section targeted string to our enum
			switch (shipSectionTargetedString) {

			case "Phasor Section 1":
				baseSectionTargeted = BaseSectionTargeted.PhasorSection1;
				break;
			case "Phasor Section 2":
				baseSectionTargeted = BaseSectionTargeted.PhasorSection2;
				break;
			case "Torpedo Section":
				baseSectionTargeted = BaseSectionTargeted.TorpedoSection;
				break;
			case "Crew Section":
				baseSectionTargeted = BaseSectionTargeted.CrewSection;
				break;
			case "Storage Section 1":
				baseSectionTargeted = BaseSectionTargeted.StorageSection1;
				break;
			case "Storage Section 2":
				baseSectionTargeted = BaseSectionTargeted.StorageSection2;
				break;

			default:
				Debug.LogError ("We have no base section targeted despite picking from the repair dropdown");
				baseSectionTargeted = BaseSectionTargeted.Untargeted;
				break;

			}

			//we can call the resolve function since we know the targeted section is valid
			RepairCrewUsedOnBase(selectedUnit, targetedUnit,baseSectionTargeted);

		}

	}

	//this function resolves a repair crew used on a ship
	private void RepairCrewUsedOnShip(CombatUnit selectedUnit, CombatUnit targetedUnit, ShipSectionTargeted shipSectionTargeted){

		//invoke the appropriate event based on the section targeted
		switch (shipSectionTargeted) {

		case ShipSectionTargeted.PhasorSection:

			OnRepairCrewUsedOnShipPhasorSection.Invoke (selectedUnit, targetedUnit);

			break;

		case ShipSectionTargeted.TorpedoSection:


			OnRepairCrewUsedOnShipTorpedoSection.Invoke (selectedUnit, targetedUnit);

			break;

		case ShipSectionTargeted.StorageSection:
			
			OnRepairCrewUsedOnShipStorageSection.Invoke (selectedUnit, targetedUnit);

			break;

		case ShipSectionTargeted.CrewSection:
			
			OnRepairCrewUsedOnShipCrewSection.Invoke (selectedUnit, targetedUnit);

			break;

		case ShipSectionTargeted.EngineSection:
			
			OnRepairCrewUsedOnShipEngineSection.Invoke (selectedUnit, targetedUnit);

			break;

		default:
			break;

		}

	}

	//this function resolves a repair crew used on a base
	private void RepairCrewUsedOnBase(CombatUnit selectedUnit, CombatUnit targetedUnit, BaseSectionTargeted baseSectionTargeted){

		//invoke the appropriate event based on the section targeted
		switch (baseSectionTargeted) {

		case BaseSectionTargeted.PhasorSection1:

			OnRepairCrewUsedOnBasePhasorSection1.Invoke (selectedUnit, targetedUnit);

			break;

		case BaseSectionTargeted.PhasorSection2:

			OnRepairCrewUsedOnBasePhasorSection2.Invoke (selectedUnit, targetedUnit);

			break;

		case BaseSectionTargeted.TorpedoSection:


			OnRepairCrewUsedOnBaseTorpedoSection.Invoke (selectedUnit, targetedUnit);

			break;

		case BaseSectionTargeted.CrewSection:

			OnRepairCrewUsedOnBaseCrewSection.Invoke (selectedUnit, targetedUnit);

			break;

		case BaseSectionTargeted.StorageSection1:

			OnRepairCrewUsedOnBaseStorageSection1.Invoke (selectedUnit, targetedUnit);

			break;

		case BaseSectionTargeted.StorageSection2:

			OnRepairCrewUsedOnBaseStorageSection2.Invoke (selectedUnit, targetedUnit);

			break;

		default:
			break;

		}

	}

	//this function converts a base section to a string
	public static string GetBaseSectionString(BaseSectionTargeted baseSectionTargeted){

		string sectionString;

		//switch case on baseSection
		switch (baseSectionTargeted) {

		case BaseSectionTargeted.PhasorSection1:

			sectionString = "Phasor Section 1";
			break;

		case BaseSectionTargeted.PhasorSection2:

			sectionString = "Phasor Section 2";
			break;

		case BaseSectionTargeted.TorpedoSection:

			sectionString = "Torpedo Section";
			break;

		case BaseSectionTargeted.CrewSection:

			sectionString = "Crew Section";
			break;

		case BaseSectionTargeted.StorageSection1:

			sectionString = "Storage Section 1";
			break;

		case BaseSectionTargeted.StorageSection2:

			sectionString = "Storage Section 2";
			break;

		default:

			sectionString = null;
			break;

		}

		return sectionString;

	}

	//this function converts a ship section to a string
	public static string GetShipSectionString(ShipSectionTargeted shipSectionTargeted){

		string sectionString;

		//switch case on baseSection
		switch (shipSectionTargeted) {

		case ShipSectionTargeted.PhasorSection:

			sectionString = "Phasor Section";
			break;

		case ShipSectionTargeted.TorpedoSection:

			sectionString = "Torpedo Section";
			break;

		case ShipSectionTargeted.StorageSection:

			sectionString = "Storage Section";
			break;

		case ShipSectionTargeted.CrewSection:

			sectionString = "Crew Section";
			break;

		case ShipSectionTargeted.EngineSection:

			sectionString = "Engine Section";
			break;

		default:

			sectionString = null;
			break;

		}

		return sectionString;

	}

	//this function handles onDestroy
	private void OnDestroy(){

		RemoveAllListeners ();

	}

	//this function removes all listeners
	private void RemoveAllListeners(){

		//remove a listener to the Phasor Section static combat event
		PhasorSection.OnFirePhasors.RemoveListener(firePhasorsResolvePhasorAttackAction);

		//remove a listener to the Torpedo Section static combat event
		TorpedoSection.OnFireLightTorpedo.RemoveListener(fireTorpedoResolveLightTorpedoAttackAction);
		TorpedoSection.OnFireHeavyTorpedo.RemoveListener(fireTorpedoResolveHeavyTorpedoAttackAction);

		if (uiManager != null) {

			//remove listeners for the flareMenu button presses
			uiManager.GetComponent<FlareManager> ().OnUseFlaresYes.RemoveListener (flareMenuResolveTorpedoAttackTrueAction);
			uiManager.GetComponent<FlareManager> ().OnUseFlaresCancel.RemoveListener (flareMenuResolveTorpedoAttackFalseAction);

		}

		//remove listeners for the useCrystal events
		StorageSection.OnUseDilithiumCrystal.RemoveListener(useCrystalResolveDilithiumCrystalAction);
		StorageSection.OnUseTrilithiumCrystal.RemoveListener(useCrystalResolveTrilithiumCrystalAction);

		//remove listeners for the repair crew event
		CrewSection.OnUseRepairCrew.RemoveListener(useRepairResolveRepairAction);

		//remove listener to the starbase phasor section static combat event
		StarbasePhasorSection1.OnFirePhasors.RemoveListener(firePhasorsResolvePhasorAttackAction);
		StarbasePhasorSection2.OnFirePhasors.RemoveListener(firePhasorsResolvePhasorAttackAction);

		//remove a listener to the starbase Torpedo Section static combat event
		StarbaseTorpedoSection.OnFireLightTorpedo.RemoveListener(fireTorpedoResolveLightTorpedoAttackAction);
		StarbaseTorpedoSection.OnFireHeavyTorpedo.RemoveListener(fireTorpedoResolveHeavyTorpedoAttackAction);

		//remove listeners to the starbase useCrystal events
		StarbaseStorageSection1.OnUseDilithiumCrystal.RemoveListener(useCrystalResolveDilithiumCrystalAction);
		StarbaseStorageSection2.OnUseTrilithiumCrystal.RemoveListener(useCrystalResolveTrilithiumCrystalAction);

		//remove listener for the starbase repair crew event
		StarbaseCrewSection.OnUseRepairCrew.RemoveListener(useRepairResolveRepairAction);

	}

}
