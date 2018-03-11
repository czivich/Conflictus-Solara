using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class FlareManager : MonoBehaviour {

	//variables for the UI elements
	public RectTransform flareMenuPanel;
	public TextMeshProUGUI flareText;
	public Toggle flareCount;
	public Button useFlaresButton;
	public Button cancelFlaresButton;

	public Button flareCountUpButton;
	public Button flareCountDownButton;
	public Button autoCalcFlaresButton;

	//variable to hold the targetedUnit
	public CombatUnit flareTargetedUnit {

		get;
		private set;

	}
	public CombatUnit flareAttackingUnit{

		get;
		private set;

	}

	//variable to hold the number of flares used
	private int flaresUsed;

	//enum to keep track of whether attack was with light or heavy torpedo
	public enum FlareTorpedoType{

		Light,
		Heavy,

	}

	//enum to keep track of whether the attack was targeted or untargeted
	public enum FlareTargetedType{

		Targeted,
		Untargeted,

	}

	private FlareTorpedoType flareTorpedoType;
	private FlareTargetedType flareTargetedType;
	private CombatManager.ShipSectionTargeted flareSectionTargeted;
	private int flareNumberDice;

	public useFlareEvent OnUseFlaresYes = new useFlareEvent();
	public useFlareEvent OnUseFlaresCancel = new useFlareEvent();

	public class useFlareEvent : UnityEvent<FlareEventData>{};

	public UnityEvent OnShowFlarePanel = new UnityEvent();

	//create a class for FlareEventData so I can pass more than 4 parameters via event
	public class FlareEventData{

		private FlareManager parent;

		public CombatUnit attackingUnit { get; private set; }
		public CombatUnit targetedUnit { get; private set; }
		public FlareTorpedoType flareTorpedoType { get; private set; }
		public FlareTargetedType flareTargetedType { get; private set; }
		public CombatManager.ShipSectionTargeted shipSectionTargeted { get; private set; }
		public int flareNumberDice { get; private set; }
		public int numberFlaresUsed { get; private set; }

		public FlareEventData(FlareManager parent){

			this.attackingUnit = parent.flareAttackingUnit;
			this.targetedUnit = parent.flareTargetedUnit;
			this.flareTorpedoType = parent.flareTorpedoType;
			this.flareTargetedType = parent.flareTargetedType;
			this.shipSectionTargeted = parent.flareSectionTargeted;
			this.flareNumberDice = parent.flareNumberDice;
			this.numberFlaresUsed = parent.flaresUsed;

		}


	}

	//unityActions
	private UnityAction<CombatUnit,CombatUnit,CombatManager.ShipSectionTargeted,int> lightTorpedoUntargetedAttackWithFlaresResolveFlareUseAction;
	private UnityAction<CombatUnit,CombatUnit,CombatManager.ShipSectionTargeted,int> heavyTorpedoUntargetedAttackWithFlaresResolveFlareUseAction;
	private UnityAction<CombatUnit,CombatUnit,CombatManager.ShipSectionTargeted,int> lightTorpedoTargetedAttackWithFlaresResolveFlareUseAction;
	private UnityAction<CombatUnit,CombatUnit,CombatManager.ShipSectionTargeted,int> heavyTorpedoTargetedAttackWithFlaresResolveFlareUseAction;

	private UnityAction<CombatUnit,CombatUnit,CombatManager.BaseSectionTargeted,int> lightTorpedoUntargetedAttackBaseWithFlaresResolveFlareUseAction;
	private UnityAction<CombatUnit,CombatUnit,CombatManager.BaseSectionTargeted,int> heavyTorpedoUntargetedAttacBasekWithFlaresResolveFlareUseAction;
	private UnityAction<CombatUnit,CombatUnit,CombatManager.BaseSectionTargeted,int> lightTorpedoTargetedAttackBaseWithFlaresResolveFlareUseAction;
	private UnityAction<CombatUnit,CombatUnit,CombatManager.BaseSectionTargeted,int> heavyTorpedoTargetedAttackBaseWithFlaresResolveFlareUseAction;

	// Use this for initialization
	public void Init () {

		//set the actions
		lightTorpedoUntargetedAttackWithFlaresResolveFlareUseAction = (attackingUnit,targetedUnit, shipSectionTargeted, numberDice) => 
		{ResolveFlareUse(attackingUnit, targetedUnit, FlareTorpedoType.Light, FlareTargetedType.Untargeted, shipSectionTargeted, numberDice);};

		heavyTorpedoUntargetedAttackWithFlaresResolveFlareUseAction = (attackingUnit,targetedUnit,shipSectionTargeted, numberDice) => 
		{ResolveFlareUse(attackingUnit, targetedUnit, FlareTorpedoType.Heavy, FlareTargetedType.Untargeted, shipSectionTargeted, numberDice);};

		lightTorpedoTargetedAttackWithFlaresResolveFlareUseAction = (attackingUnit, targetedUnit, shipSectionTargeted, numberDice) => {
			ResolveFlareUse(attackingUnit, targetedUnit, FlareTorpedoType.Light, FlareTargetedType.Targeted, shipSectionTargeted, numberDice);};

		heavyTorpedoTargetedAttackWithFlaresResolveFlareUseAction = (attackingUnit, targetedUnit, shipSectionTargeted, numberDice) => {
			ResolveFlareUse(attackingUnit, targetedUnit, FlareTorpedoType.Heavy, FlareTargetedType.Targeted, shipSectionTargeted, numberDice);};

		lightTorpedoUntargetedAttackBaseWithFlaresResolveFlareUseAction = (attackingUnit,targetedUnit, baseSectionTargeted, numberDice) => 
		{ResolveFlareUse(attackingUnit, targetedUnit, FlareTorpedoType.Light, FlareTargetedType.Untargeted, (CombatManager.ShipSectionTargeted)baseSectionTargeted, numberDice);};

		heavyTorpedoUntargetedAttacBasekWithFlaresResolveFlareUseAction = (attackingUnit,targetedUnit,baseSectionTargeted, numberDice) => 
		{ResolveFlareUse(attackingUnit, targetedUnit, FlareTorpedoType.Heavy, FlareTargetedType.Untargeted, (CombatManager.ShipSectionTargeted)baseSectionTargeted, numberDice);};

		lightTorpedoTargetedAttackBaseWithFlaresResolveFlareUseAction = (attackingUnit, targetedUnit, baseSectionTargeted, numberDice) => {
			ResolveFlareUse(attackingUnit, targetedUnit, FlareTorpedoType.Light, FlareTargetedType.Targeted, (CombatManager.ShipSectionTargeted)baseSectionTargeted, numberDice);};

		heavyTorpedoTargetedAttackBaseWithFlaresResolveFlareUseAction = (attackingUnit, targetedUnit, baseSectionTargeted, numberDice) => {
			ResolveFlareUse(attackingUnit, targetedUnit, FlareTorpedoType.Heavy, FlareTargetedType.Targeted, (CombatManager.ShipSectionTargeted)baseSectionTargeted, numberDice);};

		//add listeners for clicking the yes and cancel buttons
		useFlaresButton.onClick.AddListener(ClickedYes);
		cancelFlaresButton.onClick.AddListener(ClickedCancel);

		//add listener for torpedo attacks on ships
		CombatManager.OnLightTorpedoUntargetedAttackShipWithFlares.AddListener(lightTorpedoUntargetedAttackWithFlaresResolveFlareUseAction);
		
		CombatManager.OnHeavyTorpedoUntargetedAttackShipWithFlares.AddListener(heavyTorpedoUntargetedAttackWithFlaresResolveFlareUseAction);

		CombatManager.OnLightTorpedoTargetedAttackShipWithFlares.AddListener (lightTorpedoTargetedAttackWithFlaresResolveFlareUseAction);

		CombatManager.OnHeavyTorpedoTargetedAttackShipWithFlares.AddListener (heavyTorpedoTargetedAttackWithFlaresResolveFlareUseAction);

		//add listener for torpedo attacks on bases
		//I am casting the base section targeted as a shipSectionTargeted so I can pass it to the same functions
		CombatManager.OnLightTorpedoUntargetedAttackBaseWithFlares.AddListener(lightTorpedoUntargetedAttackBaseWithFlaresResolveFlareUseAction);

		CombatManager.OnHeavyTorpedoUntargetedAttackBaseWithFlares.AddListener(heavyTorpedoUntargetedAttacBasekWithFlaresResolveFlareUseAction);

		CombatManager.OnLightTorpedoTargetedAttackBaseWithFlares.AddListener (lightTorpedoTargetedAttackBaseWithFlaresResolveFlareUseAction);

		CombatManager.OnHeavyTorpedoTargetedAttackBaseWithFlares.AddListener (heavyTorpedoTargetedAttackBaseWithFlaresResolveFlareUseAction);

		//add listener for the up button
		flareCountUpButton.onClick.AddListener(ResolveFlareCountUpButtonPress);

		//add listener for the down button
		flareCountDownButton.onClick.AddListener(ResolveFlareCountDownButtonPress);

		//add listener for the autocalc button 
		autoCalcFlaresButton.onClick.AddListener(ResolveAutoCalcFlaresButtonPress);

		//set starting state to inactive
		flareMenuPanel.gameObject.SetActive(false);

	}
	
	//this function brings up the FlareMenu window
	private void ResolveFlareUse(CombatUnit attackingUnit, CombatUnit targetedUnit, FlareTorpedoType type, 
		FlareTargetedType targetType, CombatManager.ShipSectionTargeted shipSectionTargeted, int numberDice ){

		//cache the attack type state
		flareTargetedUnit = targetedUnit;
		flareAttackingUnit = attackingUnit;
		flareTorpedoType = type;
		flareTargetedType = targetType;
		flareSectionTargeted = shipSectionTargeted;
		flareNumberDice = numberDice;


		//check if the targeted unit is a ship
		if (targetedUnit.GetComponent<Ship> () == true) {

			//check if flare mode is manual or automatic
			if (targetedUnit.GetComponent<StorageSection> ().flareMode == StorageSection.FlareMode.Manual) {

				//show the panel
				flareMenuPanel.gameObject.SetActive(true);

				//set the border color
				SetBorderColor(targetedUnit.owner.color);

				//set the flare count toggle to zero to start
				flareCount.GetComponentInChildren<TextMeshProUGUI> ().text = "0";

				//set the useFlares button status
				SetUseFlaresButtonInteractabilityStatus();

				OnShowFlarePanel.Invoke ();

				//update the text to list the targeted unit
				if (flareSectionTargeted == CombatManager.ShipSectionTargeted.Untargeted) {

					flareText.text = targetedUnit.GetComponent<Ship> ().shipType + " " + targetedUnit.GetComponent<Ship> ().shipName +
						" is being attacked with untargeted " + flareTorpedoType.ToString () + " Torpedo. " + targetedUnit.GetComponent<Ship> ().shipType + " " + targetedUnit.GetComponent<Ship> ().shipName +
					" has " + targetedUnit.GetComponent<StorageSection> ().flares + " flares";

				} else if (flareSectionTargeted == CombatManager.ShipSectionTargeted.PhasorSection) {

					flareText.text = targetedUnit.GetComponent<Ship> ().shipType + " " + targetedUnit.GetComponent<Ship> ().shipName +
						" is being attacked with targeted " + flareTorpedoType.ToString () + " Torpedo, targeting Phasor Section.  Phasor Section has " +
					targetedUnit.GetComponent<PhasorSection> ().shieldsCurrent.ToString () + " shields.  " + targetedUnit.GetComponent<Ship> ().shipType +
					" " + targetedUnit.GetComponent<Ship> ().shipName +	" has " + targetedUnit.GetComponent<StorageSection> ().flares + " flares";
				
				} else if (flareSectionTargeted == CombatManager.ShipSectionTargeted.TorpedoSection) {

					flareText.text = targetedUnit.GetComponent<Ship> ().shipType + " " + targetedUnit.GetComponent<Ship> ().shipName +
						" is being attacked with targeted " + flareTorpedoType.ToString () + " Torpedo, targeting Torpedo Section.  Torpedo Section has " +
					targetedUnit.GetComponent<TorpedoSection> ().shieldsCurrent.ToString () + " shields.  " + targetedUnit.GetComponent<Ship> ().shipType +
					" " + targetedUnit.GetComponent<Ship> ().shipName +	" has " + targetedUnit.GetComponent<StorageSection> ().flares + " flares";

				} else if (flareSectionTargeted == CombatManager.ShipSectionTargeted.StorageSection) {

					flareText.text = targetedUnit.GetComponent<Ship> ().shipType + " " + targetedUnit.GetComponent<Ship> ().shipName +
						" is being attacked with targeted " + flareTorpedoType.ToString () + " Torpedo, targeting Storage Section.  Storage Section has " +
					targetedUnit.GetComponent<StorageSection> ().shieldsCurrent.ToString () + " shields.  " + targetedUnit.GetComponent<Ship> ().shipType +
					" " + targetedUnit.GetComponent<Ship> ().shipName +	" has " + targetedUnit.GetComponent<StorageSection> ().flares + " flares";

				} else if (flareSectionTargeted == CombatManager.ShipSectionTargeted.CrewSection) {

					flareText.text = targetedUnit.GetComponent<Ship> ().shipType + " " + targetedUnit.GetComponent<Ship> ().shipName +
						" is being attacked with targeted " + flareTorpedoType.ToString () + " Torpedo, targeting Crew Section.  Crew Section has " +
					targetedUnit.GetComponent<CrewSection> ().shieldsCurrent.ToString () + " shields.  " + targetedUnit.GetComponent<Ship> ().shipType +
					" " + targetedUnit.GetComponent<Ship> ().shipName +	" has " + targetedUnit.GetComponent<StorageSection> ().flares + " flares";

				} else if (flareSectionTargeted == CombatManager.ShipSectionTargeted.EngineSection) {

					flareText.text = targetedUnit.GetComponent<Ship> ().shipType + " " + targetedUnit.GetComponent<Ship> ().shipName +
						" is being attacked with targeted " + flareTorpedoType.ToString () + " Torpedo, targeting Engine Section.  Engine Section has " +
					targetedUnit.GetComponent<EngineSection> ().shieldsCurrent.ToString () + " shields.  " + targetedUnit.GetComponent<Ship> ().shipType +
					" " + targetedUnit.GetComponent<Ship> ().shipName +	" has " + targetedUnit.GetComponent<StorageSection> ().flares + " flares";

				}

			}
			//the else condition is that FlareMode is automatic
			else if (targetedUnit.GetComponent<StorageSection> ().flareMode == StorageSection.FlareMode.Auto) {

				//define variable for expected value destroyed by attack
				float expectedDestroyedValue = 0.0f;

				//first, check if the attack is untargeted
				if (flareSectionTargeted == CombatManager.ShipSectionTargeted.Untargeted) {

					//call calculate destroyed EV function
					expectedDestroyedValue = CalculateUntargetedDestroyedEV (targetedUnit, numberDice, 0);

					//now we have the expected value we stand to lose from the torpedo attack, given no attempt to stop with flares
					//what I want to do is to use flares up to the point where the expected value we stand to lose is driven below the cost
					//of the flares spent to thwart the torpedo

					//we need to set up a loop capped by the number of flares the targeted unit has on board
					//flare counter will track how many flares we are checking to use
					//possibleExpectedDestroyedValue will recalculate the EV based on using an additional flare
					//the marginalEVSavings is the amount we would reduce the destroyed EV by launching a marginal flare
					int flareCounter = 0;
					float possibleExpectedDestroyedValue = 0f;
					float marginalEVSavings = 0f;


					//the loop will compare what we are spending in cost of flares vs the expectedDestroyedValue.  It will calculate
					//the new cost and the new EV based on using an additional flare, and if using that additional flare is a net financial
					//gain, we will keep looping until it is no longer worth it or we have used all our flares
					do {

						//increment the flareCounter -for the first loop through, this will be checking using our first flare
						flareCounter++;

						//get what the EV would be based on using that many flares
						possibleExpectedDestroyedValue = CalculateUntargetedDestroyedEV (targetedUnit, numberDice, flareCounter);

						//calculate the marginal savings by comparing the possible EV against the current EV
						marginalEVSavings = expectedDestroyedValue - possibleExpectedDestroyedValue;

						//Debug.Log("flareCounter = " + flareCounter + ", marginalEVSavings = " + marginalEVSavings);

						//check if using that flare saves more than the cost of the flare.  If so, update the number of flares we use
						if (PurchaseManager.costFlare < marginalEVSavings && flareCounter < targetedUnit.GetComponent<StorageSection> ().flares) {

							flaresUsed = flareCounter;

							//also, set the EV to the possible EV we just tested so it is updated for the next loop
							expectedDestroyedValue = possibleExpectedDestroyedValue;

						}

						//we will continue to loop if the margin was profitable and we still have more flares to use
						//note that because the flareCounter condition is less than, we will not test a loop for more flares than we have
					} while (PurchaseManager.costFlare < marginalEVSavings && flareCounter < targetedUnit.GetComponent<StorageSection> ().flares) ;

					//we now have decided how many flares to use - we can invoke an event 
					//I believe we should be able to use the same onClickedYes event that the manual mode calls

					//populate flareEventData for passing via event
					FlareEventData flareEventData = new FlareEventData (this);

					//invoke the onClickedYes event
					OnUseFlaresYes.Invoke (flareEventData);

				}
				//the else condition is that it is a targeted attack
				else {

					//call the calculate destroyed EV function for targeted attacks
					expectedDestroyedValue = CalculateTargetedDestroyedEV (targetedUnit, numberDice, 0, flareSectionTargeted);

					//now it should be the same logic as before checking the margins, except we use the targetedEV function instead

					//now we have the expected value we stand to lose from the torpedo attack, given no attempt to stop with flares
					//what I want to do is to use flares up to the point where the expected value we stand to lose is driven below the cost
					//of the flares spent to thwart the torpedo

					//we need to set up a loop capped by the number of flares the targeted unit has on board
					//flare counter will track how many flares we are checking to use
					//possibleExpectedDestroyedValue will recalculate the EV based on using an additional flare
					//the marginalEVSavings is the amount we would reduce the destroyed EV by launching a marginal flare
					int flareCounter = 0;
					float possibleExpectedDestroyedValue = 0f;
					float marginalEVSavings = 0f;


					//the loop will compare what we are spending in cost of flares vs the expectedDestroyedValue.  It will calculate
					//the new cost and the new EV based on using an additional flare, and if using that additional flare is a net financial
					//gain, we will keep looping until it is no longer worth it or we have used all our flares
					do {

						//increment the flareCounter -for the first loop through, this will be checking using our first flare
						flareCounter++;

						//get what the EV would be based on using that many flares
						possibleExpectedDestroyedValue = CalculateTargetedDestroyedEV (targetedUnit, numberDice, flareCounter, flareSectionTargeted);

						//calculate the marginal savings by comparing the possible EV against the current EV
						marginalEVSavings = expectedDestroyedValue - possibleExpectedDestroyedValue;

						//Debug.Log("flareCounter = " + flareCounter + ", marginalEVSavings = " + marginalEVSavings);

						//check if using that flare saves more than the cost of the flare.  If so, update the number of flares we use
						if (PurchaseManager.costFlare < marginalEVSavings && flareCounter < targetedUnit.GetComponent<StorageSection> ().flares) {

							flaresUsed = flareCounter;

							//also, set the EV to the possible EV we just tested so it is updated for the next loop
							expectedDestroyedValue = possibleExpectedDestroyedValue;

						}

						//we will continue to loop if the margin was profitable and we still have more flares to use
						//note that because the flareCounter condition is less than, we will not test a loop for more flares than we have
					} while (PurchaseManager.costFlare < marginalEVSavings && flareCounter < targetedUnit.GetComponent<StorageSection> ().flares) ;

					//we now have decided how many flares to use - we can invoke an event 
					//I believe we should be able to use the same onClickedYes event that the manual mode calls

					//populate flareEventData for passing via event
					FlareEventData flareEventData = new FlareEventData (this);

					//invoke the onClickedYes event
					OnUseFlaresYes.Invoke (flareEventData);

				}

			}

		}
		//the else condition is if the targeted unit is a starbase
		else if (targetedUnit.GetComponent<Starbase> () == true){

			//check if flare mode is manual or automatic
			if (targetedUnit.GetComponent<StarbaseStorageSection2> ().flareMode == StarbaseStorageSection2.FlareMode.Manual) {
				
				//show the panel
				flareMenuPanel.gameObject.SetActive(true);

				//set the border color
				SetBorderColor(targetedUnit.owner.color);

				//set the flare count toggle to zero to start
				flareCount.GetComponentInChildren<TextMeshProUGUI> ().text = "0";

				//set the useFlares button status
				SetUseFlaresButtonInteractabilityStatus();

				OnShowFlarePanel.Invoke ();

				//update the text to list the targeted unit
				//ShipSectionTargeted.Untargeted maps to BaseSectionTargeted.Untargeted
				if (flareSectionTargeted == CombatManager.ShipSectionTargeted.Untargeted) {

					flareText.text =  "Starbase " + targetedUnit.GetComponent<Starbase> ().baseName +
						" is being attacked with untargeted " + flareTorpedoType.ToString () + " Torpedo.  Starbase " + targetedUnit.GetComponent<Starbase> ().baseName +
						" has " + targetedUnit.GetComponent<StarbaseStorageSection2> ().flares + " flares";

				} 
				//ShipSectionTargeted.PhasorSection maps to BaseSectionTargeted.PhasorSection1
				else if (flareSectionTargeted == CombatManager.ShipSectionTargeted.PhasorSection) {

					flareText.text = "Starbase " + targetedUnit.GetComponent<Starbase> ().baseName +
						" is being attacked with targeted " + flareTorpedoType.ToString () + " Torpedo, targeting Phasor Section 1.  Phasor Section 1 has " +
						targetedUnit.GetComponent<StarbasePhasorSection1> ().shieldsCurrent.ToString () + " shields.  Starbase " +
						targetedUnit.GetComponent<Starbase> ().baseName +	" has " + targetedUnit.GetComponent<StarbaseStorageSection2> ().flares + " flares";

				} 
				//ShipSectionTargeted.TorpedoSection maps to BaseSectionTargeted.PhasorSection2
				else if (flareSectionTargeted == CombatManager.ShipSectionTargeted.TorpedoSection) {

					flareText.text = "Starbase " + targetedUnit.GetComponent<Starbase> ().baseName +
						" is being attacked with targeted " + flareTorpedoType.ToString () + " Torpedo, targeting Phasor Section 2.  Phasor Section 2 has " +
						targetedUnit.GetComponent<StarbasePhasorSection2> ().shieldsCurrent.ToString () + " shields.  Starbase " +
						targetedUnit.GetComponent<Starbase> ().baseName +	" has " + targetedUnit.GetComponent<StarbaseStorageSection2> ().flares + " flares";

				} 
				//ShipSectionTargeted.StorageSection maps to BaseSectionTargeted.TorpedoSection
				else if (flareSectionTargeted == CombatManager.ShipSectionTargeted.StorageSection) {

					flareText.text = "Starbase " + targetedUnit.GetComponent<Starbase> ().baseName +
						" is being attacked with targeted " + flareTorpedoType.ToString () + " Torpedo, targeting Torpedo Section.  Torpedo Section has " +
						targetedUnit.GetComponent<StarbaseTorpedoSection> ().shieldsCurrent.ToString () + " shields.  Starbase " +
						targetedUnit.GetComponent<Starbase> ().baseName +	" has " + targetedUnit.GetComponent<StarbaseStorageSection2> ().flares + " flares";

				} 
				//ShipSectionTargeted.CrewSection maps to BaseSectionTargeted.CrewSection
				else if (flareSectionTargeted == CombatManager.ShipSectionTargeted.CrewSection) {

					flareText.text = "Starbase " + targetedUnit.GetComponent<Starbase> ().baseName +
						" is being attacked with targeted " + flareTorpedoType.ToString () + " Torpedo, targeting Crew Section.  Crew Section has " +
						targetedUnit.GetComponent<StarbaseCrewSection> ().shieldsCurrent.ToString () + " shields.  Starbase " +
						targetedUnit.GetComponent<Starbase> ().baseName +	" has " + targetedUnit.GetComponent<StarbaseStorageSection2> ().flares + " flares";

				} 
				//ShipSectionTargeted.EngineSection maps to BaseSectionTargeted.StorageSection1
				else if (flareSectionTargeted == CombatManager.ShipSectionTargeted.EngineSection) {

					flareText.text = "Starbase " + targetedUnit.GetComponent<Starbase> ().baseName +
						" is being attacked with targeted " + flareTorpedoType.ToString () + " Torpedo, targeting Storage Section 1.  Storage Section 1 has " +
						targetedUnit.GetComponent<StarbaseStorageSection1> ().shieldsCurrent.ToString () + " shields.  Starbase " +
						targetedUnit.GetComponent<Starbase> ().baseName +	" has " + targetedUnit.GetComponent<StarbaseStorageSection2> ().flares + " flares";

				}
				//ShipSectionTargeted.Miss maps to BaseSectionTargeted.StorageSection2
				else if (flareSectionTargeted == CombatManager.ShipSectionTargeted.Miss) {

					flareText.text = "Starbase " + targetedUnit.GetComponent<Starbase> ().baseName +
						" is being attacked with targeted " + flareTorpedoType.ToString () + " Torpedo, targeting Storage Section 2.  Storage Section 2 has " +
						targetedUnit.GetComponent<StarbaseStorageSection2> ().shieldsCurrent.ToString () + " shields.  Starbase " +
						targetedUnit.GetComponent<Starbase> ().baseName +	" has " + targetedUnit.GetComponent<StarbaseStorageSection2> ().flares + " flares";

				}

			}
			//the else condition is that FlareMode is automatic
			else if (targetedUnit.GetComponent<StarbaseStorageSection2> ().flareMode == StarbaseStorageSection2.FlareMode.Auto) {

				//define variable for expected value destroyed by attack
				float expectedDestroyedValue = 0.0f;

				//first, check if the attack is untargeted
				if (flareSectionTargeted == CombatManager.ShipSectionTargeted.Untargeted) {

					//call calculate destroyed EV function
					expectedDestroyedValue = CalculateUntargetedDestroyedEV (targetedUnit, numberDice, 0);

					//now we have the expected value we stand to lose from the torpedo attack, given no attempt to stop with flares
					//what I want to do is to use flares up to the point where the expected value we stand to lose is driven below the cost
					//of the flares spent to thwart the torpedo

					//we need to set up a loop capped by the number of flares the targeted unit has on board
					//flare counter will track how many flares we are checking to use
					//possibleExpectedDestroyedValue will recalculate the EV based on using an additional flare
					//the marginalEVSavings is the amount we would reduce the destroyed EV by launching a marginal flare
					int flareCounter = 0;
					float possibleExpectedDestroyedValue = 0f;
					float marginalEVSavings = 0f;


					//the loop will compare what we are spending in cost of flares vs the expectedDestroyedValue.  It will calculate
					//the new cost and the new EV based on using an additional flare, and if using that additional flare is a net financial
					//gain, we will keep looping until it is no longer worth it or we have used all our flares
					do {

						//increment the flareCounter -for the first loop through, this will be checking using our first flare
						flareCounter++;

						//get what the EV would be based on using that many flares
						possibleExpectedDestroyedValue = CalculateUntargetedDestroyedEV (targetedUnit, numberDice, flareCounter);

						//calculate the marginal savings by comparing the possible EV against the current EV
						marginalEVSavings = expectedDestroyedValue - possibleExpectedDestroyedValue;

						//Debug.Log("flareCounter = " + flareCounter + ", marginalEVSavings = " + marginalEVSavings);

						//check if using that flare saves more than the cost of the flare.  If so, update the number of flares we use
						if (PurchaseManager.costFlare < marginalEVSavings && flareCounter < targetedUnit.GetComponent<StorageSection> ().flares) {

							flaresUsed = flareCounter;

							//also, set the EV to the possible EV we just tested so it is updated for the next loop
							expectedDestroyedValue = possibleExpectedDestroyedValue;

						}

						//we will continue to loop if the margin was profitable and we still have more flares to use
						//note that because the flareCounter condition is less than, we will not test a loop for more flares than we have
					} while (PurchaseManager.costFlare < marginalEVSavings && flareCounter < targetedUnit.GetComponent<StorageSection> ().flares) ;

					//we now have decided how many flares to use - we can invoke an event 
					//I believe we should be able to use the same onClickedYes event that the manual mode calls

					//populate flareEventData for passing via event
					FlareEventData flareEventData = new FlareEventData (this);

					//invoke the onClickedYes event
					OnUseFlaresYes.Invoke (flareEventData);

				}
				//the else condition is that it is a targeted attack
				else {

					//call the calculate destroyed EV function for targeted attacks
					expectedDestroyedValue = CalculateTargetedDestroyedEV (targetedUnit, numberDice, 0, flareSectionTargeted);

					//now it should be the same logic as before checking the margins, except we use the targetedEV function instead

					//now we have the expected value we stand to lose from the torpedo attack, given no attempt to stop with flares
					//what I want to do is to use flares up to the point where the expected value we stand to lose is driven below the cost
					//of the flares spent to thwart the torpedo

					//we need to set up a loop capped by the number of flares the targeted unit has on board
					//flare counter will track how many flares we are checking to use
					//possibleExpectedDestroyedValue will recalculate the EV based on using an additional flare
					//the marginalEVSavings is the amount we would reduce the destroyed EV by launching a marginal flare
					int flareCounter = 0;
					float possibleExpectedDestroyedValue = 0f;
					float marginalEVSavings = 0f;


					//the loop will compare what we are spending in cost of flares vs the expectedDestroyedValue.  It will calculate
					//the new cost and the new EV based on using an additional flare, and if using that additional flare is a net financial
					//gain, we will keep looping until it is no longer worth it or we have used all our flares
					do {

						//increment the flareCounter -for the first loop through, this will be checking using our first flare
						flareCounter++;

						//get what the EV would be based on using that many flares
						possibleExpectedDestroyedValue = CalculateTargetedDestroyedEV (targetedUnit, numberDice, flareCounter, flareSectionTargeted);

						//calculate the marginal savings by comparing the possible EV against the current EV
						marginalEVSavings = expectedDestroyedValue - possibleExpectedDestroyedValue;

						//Debug.Log("flareCounter = " + flareCounter + ", marginalEVSavings = " + marginalEVSavings);

						//check if using that flare saves more than the cost of the flare.  If so, update the number of flares we use
						if (PurchaseManager.costFlare < marginalEVSavings && flareCounter < targetedUnit.GetComponent<StorageSection> ().flares) {

							flaresUsed = flareCounter;

							//also, set the EV to the possible EV we just tested so it is updated for the next loop
							expectedDestroyedValue = possibleExpectedDestroyedValue;

						}

						//we will continue to loop if the margin was profitable and we still have more flares to use
						//note that because the flareCounter condition is less than, we will not test a loop for more flares than we have
					} while (PurchaseManager.costFlare < marginalEVSavings && flareCounter < targetedUnit.GetComponent<StorageSection> ().flares) ;

					//we now have decided how many flares to use - we can invoke an event 
					//I believe we should be able to use the same onClickedYes event that the manual mode calls

					//populate flareEventData for passing via event
					FlareEventData flareEventData = new FlareEventData (this);

					//invoke the onClickedYes event
					OnUseFlaresYes.Invoke (flareEventData);

				}

			}

		}

	}

	//this function returns the number of flares that would be used by an auto-calculation
	private int AutoCalculateFlaresUsed(CombatUnit attackingUnit, CombatUnit targetedUnit, FlareTorpedoType type, 
		FlareTargetedType targetType, CombatManager.ShipSectionTargeted shipSectionTargeted, int numberDice ){

		//variable to hold the number of flares to use
		int flaresCalcToUse = 0;

		//check if the targeted unit is a ship
		if (targetedUnit.GetComponent<Ship> () == true) {
			
			//define variable for expected value destroyed by attack
			float expectedDestroyedValue = 0.0f;

			//first, check if the attack is untargeted
			if (shipSectionTargeted == CombatManager.ShipSectionTargeted.Untargeted) {

				//call calculate destroyed EV function
				expectedDestroyedValue = CalculateUntargetedDestroyedEV (targetedUnit, numberDice, 0);

				//now we have the expected value we stand to lose from the torpedo attack, given no attempt to stop with flares
				//what I want to do is to use flares up to the point where the expected value we stand to lose is driven below the cost
				//of the flares spent to thwart the torpedo

				//we need to set up a loop capped by the number of flares the targeted unit has on board
				//flare counter will track how many flares we are checking to use
				//possibleExpectedDestroyedValue will recalculate the EV based on using an additional flare
				//the marginalEVSavings is the amount we would reduce the destroyed EV by launching a marginal flare
				int flareCounter = 0;
				float possibleExpectedDestroyedValue = 0f;
				float marginalEVSavings = 0f;


				//the loop will compare what we are spending in cost of flares vs the expectedDestroyedValue.  It will calculate
				//the new cost and the new EV based on using an additional flare, and if using that additional flare is a net financial
				//gain, we will keep looping until it is no longer worth it or we have used all our flares
				do {

					//increment the flareCounter -for the first loop through, this will be checking using our first flare
					flareCounter++;

					//get what the EV would be based on using that many flares
					possibleExpectedDestroyedValue = CalculateUntargetedDestroyedEV (targetedUnit, numberDice, flareCounter);

					//calculate the marginal savings by comparing the possible EV against the current EV
					marginalEVSavings = expectedDestroyedValue - possibleExpectedDestroyedValue;

					//Debug.Log("flareCounter = " + flareCounter + ", marginalEVSavings = " + marginalEVSavings);

					//check if using that flare saves more than the cost of the flare.  If so, update the number of flares we use
					if (PurchaseManager.costFlare < marginalEVSavings && flareCounter < targetedUnit.GetComponent<StorageSection> ().flares) {

						flaresCalcToUse = flareCounter;

						//also, set the EV to the possible EV we just tested so it is updated for the next loop
						expectedDestroyedValue = possibleExpectedDestroyedValue;

					}

					//we will continue to loop if the margin was profitable and we still have more flares to use
					//note that because the flareCounter condition is less than, we will not test a loop for more flares than we have
				} while (PurchaseManager.costFlare < marginalEVSavings && flareCounter < targetedUnit.GetComponent<StorageSection> ().flares) ;

				//we now have decided how many flares to use - we can return the value
				return flaresCalcToUse;

			}
			//the else condition is that it is a targeted attack
			else {

				//call the calculate destroyed EV function for targeted attacks
				expectedDestroyedValue = CalculateTargetedDestroyedEV (targetedUnit, numberDice, 0, shipSectionTargeted);

				//now it should be the same logic as before checking the margins, except we use the targetedEV function instead

				//now we have the expected value we stand to lose from the torpedo attack, given no attempt to stop with flares
				//what I want to do is to use flares up to the point where the expected value we stand to lose is driven below the cost
				//of the flares spent to thwart the torpedo

				//we need to set up a loop capped by the number of flares the targeted unit has on board
				//flare counter will track how many flares we are checking to use
				//possibleExpectedDestroyedValue will recalculate the EV based on using an additional flare
				//the marginalEVSavings is the amount we would reduce the destroyed EV by launching a marginal flare
				int flareCounter = 0;
				float possibleExpectedDestroyedValue = 0f;
				float marginalEVSavings = 0f;


				//the loop will compare what we are spending in cost of flares vs the expectedDestroyedValue.  It will calculate
				//the new cost and the new EV based on using an additional flare, and if using that additional flare is a net financial
				//gain, we will keep looping until it is no longer worth it or we have used all our flares
				do {

					//increment the flareCounter -for the first loop through, this will be checking using our first flare
					flareCounter++;

					//get what the EV would be based on using that many flares
					possibleExpectedDestroyedValue = CalculateTargetedDestroyedEV (targetedUnit, numberDice, flareCounter, shipSectionTargeted);

					//calculate the marginal savings by comparing the possible EV against the current EV
					marginalEVSavings = expectedDestroyedValue - possibleExpectedDestroyedValue;

					//Debug.Log("flareCounter = " + flareCounter + ", marginalEVSavings = " + marginalEVSavings);

					//check if using that flare saves more than the cost of the flare.  If so, update the number of flares we use
					if (PurchaseManager.costFlare < marginalEVSavings && flareCounter < targetedUnit.GetComponent<StorageSection> ().flares) {

						flaresCalcToUse = flareCounter;

						//also, set the EV to the possible EV we just tested so it is updated for the next loop
						expectedDestroyedValue = possibleExpectedDestroyedValue;

					}

					//we will continue to loop if the margin was profitable and we still have more flares to use
					//note that because the flareCounter condition is less than, we will not test a loop for more flares than we have
				} while (PurchaseManager.costFlare < marginalEVSavings && flareCounter < targetedUnit.GetComponent<StorageSection> ().flares) ;

				//we now have decided how many flares to use - we can return the value
				return flaresCalcToUse;

			}

		}
		//the else condition is if the targeted unit is a starbase
		else if (targetedUnit.GetComponent<Starbase> () == true) {

			//define variable for expected value destroyed by attack
			float expectedDestroyedValue = 0.0f;

			//first, check if the attack is untargeted
			if (shipSectionTargeted == CombatManager.ShipSectionTargeted.Untargeted) {

				//call calculate destroyed EV function
				expectedDestroyedValue = CalculateUntargetedDestroyedEV (targetedUnit, numberDice, 0);

				//now we have the expected value we stand to lose from the torpedo attack, given no attempt to stop with flares
				//what I want to do is to use flares up to the point where the expected value we stand to lose is driven below the cost
				//of the flares spent to thwart the torpedo

				//we need to set up a loop capped by the number of flares the targeted unit has on board
				//flare counter will track how many flares we are checking to use
				//possibleExpectedDestroyedValue will recalculate the EV based on using an additional flare
				//the marginalEVSavings is the amount we would reduce the destroyed EV by launching a marginal flare
				int flareCounter = 0;
				float possibleExpectedDestroyedValue = 0f;
				float marginalEVSavings = 0f;


				//the loop will compare what we are spending in cost of flares vs the expectedDestroyedValue.  It will calculate
				//the new cost and the new EV based on using an additional flare, and if using that additional flare is a net financial
				//gain, we will keep looping until it is no longer worth it or we have used all our flares
				do {

					//increment the flareCounter -for the first loop through, this will be checking using our first flare
					flareCounter++;

					//get what the EV would be based on using that many flares
					possibleExpectedDestroyedValue = CalculateUntargetedDestroyedEV (targetedUnit, numberDice, flareCounter);

					//calculate the marginal savings by comparing the possible EV against the current EV
					marginalEVSavings = expectedDestroyedValue - possibleExpectedDestroyedValue;

					//Debug.Log("flareCounter = " + flareCounter + ", marginalEVSavings = " + marginalEVSavings);

					//check if using that flare saves more than the cost of the flare.  If so, update the number of flares we use
					if (PurchaseManager.costFlare < marginalEVSavings && flareCounter < targetedUnit.GetComponent<StorageSection> ().flares) {

						flaresCalcToUse = flareCounter;

						//also, set the EV to the possible EV we just tested so it is updated for the next loop
						expectedDestroyedValue = possibleExpectedDestroyedValue;

					}

					//we will continue to loop if the margin was profitable and we still have more flares to use
					//note that because the flareCounter condition is less than, we will not test a loop for more flares than we have
				} while (PurchaseManager.costFlare < marginalEVSavings && flareCounter < targetedUnit.GetComponent<StorageSection> ().flares) ;

				//we now have decided how many flares to use - we can return the value
				return flaresCalcToUse;

			}
			//the else condition is that it is a targeted attack
			else {

				//call the calculate destroyed EV function for targeted attacks
				expectedDestroyedValue = CalculateTargetedDestroyedEV (targetedUnit, numberDice, 0, shipSectionTargeted);

				//now it should be the same logic as before checking the margins, except we use the targetedEV function instead

				//now we have the expected value we stand to lose from the torpedo attack, given no attempt to stop with flares
				//what I want to do is to use flares up to the point where the expected value we stand to lose is driven below the cost
				//of the flares spent to thwart the torpedo

				//we need to set up a loop capped by the number of flares the targeted unit has on board
				//flare counter will track how many flares we are checking to use
				//possibleExpectedDestroyedValue will recalculate the EV based on using an additional flare
				//the marginalEVSavings is the amount we would reduce the destroyed EV by launching a marginal flare
				int flareCounter = 0;
				float possibleExpectedDestroyedValue = 0f;
				float marginalEVSavings = 0f;


				//the loop will compare what we are spending in cost of flares vs the expectedDestroyedValue.  It will calculate
				//the new cost and the new EV based on using an additional flare, and if using that additional flare is a net financial
				//gain, we will keep looping until it is no longer worth it or we have used all our flares
				do {

					//increment the flareCounter -for the first loop through, this will be checking using our first flare
					flareCounter++;

					//get what the EV would be based on using that many flares
					possibleExpectedDestroyedValue = CalculateTargetedDestroyedEV (targetedUnit, numberDice, flareCounter, shipSectionTargeted);

					//calculate the marginal savings by comparing the possible EV against the current EV
					marginalEVSavings = expectedDestroyedValue - possibleExpectedDestroyedValue;

					//Debug.Log("flareCounter = " + flareCounter + ", marginalEVSavings = " + marginalEVSavings);

					//check if using that flare saves more than the cost of the flare.  If so, update the number of flares we use
					if (PurchaseManager.costFlare < marginalEVSavings && flareCounter < targetedUnit.GetComponent<StorageSection> ().flares) {

						flaresCalcToUse = flareCounter;

						//also, set the EV to the possible EV we just tested so it is updated for the next loop
						expectedDestroyedValue = possibleExpectedDestroyedValue;

					}

					//we will continue to loop if the margin was profitable and we still have more flares to use
					//note that because the flareCounter condition is less than, we will not test a loop for more flares than we have
				} while (PurchaseManager.costFlare < marginalEVSavings && flareCounter < targetedUnit.GetComponent<StorageSection> ().flares) ;

				//we now have decided how many flares to use - we can return the value
				return flaresCalcToUse;

			}  //end targeted attack

		}  //end starbase

		else {

			//only have to do this because i used else-if for starbase
			return flaresCalcToUse;

		}

	}  //end function
		
	//this function handles pressing the up button for the flare quantity
	private void ResolveFlareCountUpButtonPress(){

		//temporary variable to hold the number of flares in flareCount
		int flareCountflares = int.Parse (flareCount.GetComponentInChildren<TextMeshProUGUI> ().text);

		//check if the targeted unit is a ship or a starbase
		if (flareTargetedUnit.GetComponent<Ship> () == true) {
			
			//for an up button press, we want to increase the quantity as long as we're not at the maximum value
			if (flareCountflares < flareTargetedUnit.GetComponent<StorageSection> ().flares) {

				//the current flareCount qty is less than the number of flares on board
				//increase the flareCount by 1
				flareCountflares = flareCountflares + 1;

				//write the new count back to the toggle
				flareCount.GetComponentInChildren<TextMeshProUGUI> ().text = flareCountflares.ToString ();

			}

		} else if (flareTargetedUnit.GetComponent<Starbase> () == true) {

			//for an up button press, we want to increase the quantity as long as we're not at the maximum value
			if (flareCountflares < flareTargetedUnit.GetComponent<StarbaseStorageSection2> ().flares) {

				//the current flareCount qty is less than the number of flares on board
				//increase the flareCount by 1
				flareCountflares = flareCountflares + 1;

				//write the new count back to the toggle
				flareCount.GetComponentInChildren<TextMeshProUGUI> ().text = flareCountflares.ToString ();

			}

		}

		//update the button status
		SetUseFlaresButtonInteractabilityStatus();

	}

	//this function handles pressing the down button for the flare quantity
	private void ResolveFlareCountDownButtonPress(){

		//temporary variable to hold the number of flares in flareCount
		int flareCountflares = int.Parse (flareCount.GetComponentInChildren<TextMeshProUGUI> ().text);

		//check if the targeted unit is a ship or a starbase
		if (flareTargetedUnit.GetComponent<Ship> () == true) {

			//for a down button press, we want to decrease the quantity as long as we're not at the minimum value
			if (flareCountflares > 0) {

				//the current flareCount qty is greater than 0
				//decrease the flareCount by 1
				flareCountflares = flareCountflares - 1;

				//write the new count back to the toggle
				flareCount.GetComponentInChildren<TextMeshProUGUI> ().text = flareCountflares.ToString ();

			}

		} else if (flareTargetedUnit.GetComponent<Starbase> () == true) {

			//for a down button press, we want to decrease the quantity as long as we're not at the minimum value
			if (flareCountflares > 0) {

				//the current flareCount qty is greater than 0
				//decrease the flareCount by 1
				flareCountflares = flareCountflares - 1;

				//write the new count back to the toggle
				flareCount.GetComponentInChildren<TextMeshProUGUI> ().text = flareCountflares.ToString ();

			}

		}

		//update the button status
		SetUseFlaresButtonInteractabilityStatus();

	}

	//this function resolves the autoCalc button press
	private void ResolveAutoCalcFlaresButtonPress(){

		//create a temporary variable for flares to use and call the autocalc function
		int flareCountflares = AutoCalculateFlaresUsed (flareAttackingUnit, flareTargetedUnit, 
			flareTorpedoType, flareTargetedType, flareSectionTargeted, flareNumberDice);

		//write the new count back to the toggle
		flareCount.GetComponentInChildren<TextMeshProUGUI> ().text = flareCountflares.ToString ();

		//update the button status
		SetUseFlaresButtonInteractabilityStatus();

	}


	//this function handles clicking the yes button
	private void ClickedYes(){

		flaresUsed = int.Parse (flareCount.GetComponentInChildren<TextMeshProUGUI> ().text);

		//hide the flare menu panel
		flareMenuPanel.gameObject.SetActive(false);

		//populate flareEventData for passing via event
		FlareEventData flareEventData = new FlareEventData(this);

		//invoke the onClickedYes event
		OnUseFlaresYes.Invoke(flareEventData);


	}


	//this function handles clicking the cancel button
	private void ClickedCancel(){

		//hide the flare menu panel
		flareMenuPanel.gameObject.SetActive(false);

		//populate flareEventData for passing via event
		FlareEventData flareEventData = new FlareEventData(this);

		//invoke the onClickedCancel event
		OnUseFlaresCancel.Invoke(flareEventData);

	}

	//this function sets the interactability of the UseFlaresYes button
	private void SetUseFlaresButtonInteractabilityStatus(){

		//check if the number of flares to be used is greater than zero
		int flaresToUse = int.Parse (flareCount.GetComponentInChildren<TextMeshProUGUI> ().text);

		if (flaresToUse > 0) {

			useFlaresButton.interactable = true;

		} else {

			useFlaresButton.interactable = false;

		}

	}

	//this function calculates the DestroyedEV for an untargeted torpedo attack
	private float CalculateUntargetedDestroyedEV(CombatUnit targetedUnit, int numberDice, int numberFlaresUsed){

		float expectedDestroyedValue = 0.0f;

		//if the attack is untargeted, we need to determine the hit chance on each individual section
		switch (targetedUnit.GetComponent<CombatUnit> ().unitType) {

		case CombatUnit.UnitType.Starship:

			//get the section destroyed EVs:
			expectedDestroyedValue += (1.0f/6.0f) * GetPhasorSectionDestroyedEV (targetedUnit, CombatUnit.UnitType.Starship, numberDice);
			expectedDestroyedValue += (1.0f/6.0f) * GetTorpedoSectionDestroyedEV (targetedUnit, CombatUnit.UnitType.Starship, numberDice);
			expectedDestroyedValue += (1.0f/6.0f) * GetStorageSectionDestroyedEV (targetedUnit, CombatUnit.UnitType.Starship, numberDice, numberFlaresUsed);
			expectedDestroyedValue += (1.0f/6.0f) * GetCrewSectionDestroyedEV (targetedUnit, CombatUnit.UnitType.Starship, numberDice);
			expectedDestroyedValue += (1.0f/6.0f) * GetEngineSectionDestroyedEV (targetedUnit, CombatUnit.UnitType.Starship, numberDice);
			break;

		case CombatUnit.UnitType.Destroyer:

			//get the section destroyed EVs:
			expectedDestroyedValue += (1.0f/6.0f) * GetPhasorSectionDestroyedEV (targetedUnit, CombatUnit.UnitType.Destroyer, numberDice);
			expectedDestroyedValue += (1.0f/6.0f) * GetTorpedoSectionDestroyedEV (targetedUnit, CombatUnit.UnitType.Destroyer, numberDice);
			expectedDestroyedValue += (1.0f/6.0f) * GetStorageSectionDestroyedEV (targetedUnit, CombatUnit.UnitType.Destroyer, numberDice, numberFlaresUsed);
			expectedDestroyedValue += (1.0f/6.0f) * GetEngineSectionDestroyedEV (targetedUnit, CombatUnit.UnitType.Destroyer, numberDice);
			break;

		case CombatUnit.UnitType.BirdOfPrey:

			//get the section destroyed EVs:
			expectedDestroyedValue += (1.0f/6.0f) * GetPhasorSectionDestroyedEV (targetedUnit, CombatUnit.UnitType.BirdOfPrey, numberDice);
			expectedDestroyedValue += (1.0f/6.0f) * GetTorpedoSectionDestroyedEV (targetedUnit, CombatUnit.UnitType.BirdOfPrey, numberDice);
			expectedDestroyedValue += (1.0f/6.0f) * GetEngineSectionDestroyedEV (targetedUnit, CombatUnit.UnitType.BirdOfPrey, numberDice);
			break;


		case CombatUnit.UnitType.Scout:

			//get the section destroyed EVs:
			expectedDestroyedValue += (1.0f/6.0f) * GetPhasorSectionDestroyedEV (targetedUnit, CombatUnit.UnitType.Scout, numberDice);
			expectedDestroyedValue += (1.0f/6.0f) * GetStorageSectionDestroyedEV (targetedUnit, CombatUnit.UnitType.Scout, numberDice, numberFlaresUsed);
			expectedDestroyedValue += (1.0f/6.0f) * GetEngineSectionDestroyedEV (targetedUnit, CombatUnit.UnitType.Scout, numberDice);
			break;

		case CombatUnit.UnitType.Starbase:

			//get the section destroyed EVs:
			expectedDestroyedValue += (1.0f/6.0f) * GetBasePhasorSection1DestroyedEV (targetedUnit, CombatUnit.UnitType.Starbase, numberDice);
			expectedDestroyedValue += (1.0f/6.0f) * GetBasePhasorSection2DestroyedEV (targetedUnit, CombatUnit.UnitType.Starbase, numberDice);
			expectedDestroyedValue += (1.0f/6.0f) * GetBaseTorpedoSectionDestroyedEV (targetedUnit, CombatUnit.UnitType.Starbase, numberDice);
			expectedDestroyedValue += (1.0f/6.0f) * GetBaseCrewSectionDestroyedEV (targetedUnit, CombatUnit.UnitType.Starbase, numberDice);
			expectedDestroyedValue += (1.0f/6.0f) * GetBaseStorageSection1DestroyedEV (targetedUnit, CombatUnit.UnitType.Starbase, numberDice);
			expectedDestroyedValue += (1.0f/6.0f) * GetBaseStorageSection2DestroyedEV (targetedUnit, CombatUnit.UnitType.Starbase, numberDice, numberFlaresUsed);

			break;

		default:

			expectedDestroyedValue += 0.0f;
			break;

		}

		//we need to discount the EV by the likelihood of the flare stopping the torpedo.  Using flares reduces the chance of a hit, which 
		//reduces the expected destroyed value
		//each flare reduces the expected value by a factor of 5/6

		//Debug.Log("Expected before discount = " + expectedDestroyedValue);

		expectedDestroyedValue = expectedDestroyedValue * Mathf.Pow(5.0f/6.0f, (float)numberFlaresUsed);

		//Debug.Log("Expected after discount = " + expectedDestroyedValue);

		return expectedDestroyedValue;

	}

	//this function calculates the DestroyedEV for a targeted torpedo attack
	private float CalculateTargetedDestroyedEV(CombatUnit targetedUnit, int numberDice, int numberFlaresUsed, CombatManager.ShipSectionTargeted shipSectionTargeted){

		float expectedDestroyedValue = 0.0f;

		//first, check if the targeted unit is a ship or a base
		if (targetedUnit.GetComponent<Ship> () == true) {

			//first, do a switch based on the section targeted
			//the function calls will dynamically read the unit type
			switch (shipSectionTargeted) {

			case CombatManager.ShipSectionTargeted.PhasorSection:

				expectedDestroyedValue = GetPhasorSectionDestroyedEV (targetedUnit, targetedUnit.GetComponent<CombatUnit> ().unitType, numberDice);
				break;

			case CombatManager.ShipSectionTargeted.TorpedoSection:

				expectedDestroyedValue = GetTorpedoSectionDestroyedEV (targetedUnit, targetedUnit.GetComponent<CombatUnit> ().unitType, numberDice);
				break;

			case CombatManager.ShipSectionTargeted.StorageSection:

				expectedDestroyedValue = GetStorageSectionDestroyedEV (targetedUnit, targetedUnit.GetComponent<CombatUnit> ().unitType, numberDice, numberFlaresUsed);
				break;

			case CombatManager.ShipSectionTargeted.CrewSection:

				expectedDestroyedValue = GetCrewSectionDestroyedEV (targetedUnit, targetedUnit.GetComponent<CombatUnit> ().unitType, numberDice);
				break;

			case CombatManager.ShipSectionTargeted.EngineSection:

				expectedDestroyedValue = GetEngineSectionDestroyedEV (targetedUnit, targetedUnit.GetComponent<CombatUnit> ().unitType, numberDice);
				break;

			default:

				expectedDestroyedValue = 0.0f;
				break;

			}

			//we need to discount the EV by the likelihood of the flare stopping the torpedo.  Using flares reduces the chance of a hit, which 
			//reduces the expected destroyed value
			//each flare reduces the expected value by a factor of 5/6

			//Debug.Log("Expected before discount = " + expectedDestroyedValue);

			expectedDestroyedValue = expectedDestroyedValue * Mathf.Pow (5.0f / 6.0f, (float)numberFlaresUsed);

			//Debug.Log("Expected after discount = " + expectedDestroyedValue);

			return expectedDestroyedValue;

		}
		//the else condtion is that we have a starbase as the targeted unit
		else {

			//first, do a switch based on the section targeted
			//the function calls will dynamically read the unit type
			switch (shipSectionTargeted) {

			//ShipSectionTargeted.PhasorSection maps to BaseSectionTargeted.PhasorSection1
			case CombatManager.ShipSectionTargeted.PhasorSection:

				expectedDestroyedValue = GetBasePhasorSection1DestroyedEV (targetedUnit, targetedUnit.GetComponent<CombatUnit> ().unitType, numberDice);
				break;
			
			//ShipSectionTargeted.TorpedoSection maps to BaseSectionTargeted.PhasorSection2
			case CombatManager.ShipSectionTargeted.TorpedoSection:

				expectedDestroyedValue = GetBasePhasorSection2DestroyedEV (targetedUnit, targetedUnit.GetComponent<CombatUnit> ().unitType, numberDice);
				break;

			//ShipSectionTargeted.StorageSection maps to BaseSectionTargeted.TorpedoSection
			case CombatManager.ShipSectionTargeted.StorageSection:

				expectedDestroyedValue = GetBaseTorpedoSectionDestroyedEV (targetedUnit, targetedUnit.GetComponent<CombatUnit> ().unitType, numberDice);
				break;

			//ShipSectionTargeted.CrewSection maps to BaseSectionTargeted.CrewSection
			case CombatManager.ShipSectionTargeted.CrewSection:

				expectedDestroyedValue = GetBaseCrewSectionDestroyedEV (targetedUnit, targetedUnit.GetComponent<CombatUnit> ().unitType, numberDice);
				break;

				//ShipSectionTargeted.EngineSection maps to BaseSectionTargeted.StorageSection1
			case CombatManager.ShipSectionTargeted.EngineSection:

				expectedDestroyedValue = GetBaseStorageSection1DestroyedEV (targetedUnit, targetedUnit.GetComponent<CombatUnit> ().unitType, numberDice);
				break;

				//ShipSectionTargeted.Miss maps to BaseSectionTargeted.StorageSection2
			case CombatManager.ShipSectionTargeted.Miss:

				expectedDestroyedValue = GetBaseStorageSection2DestroyedEV (targetedUnit, targetedUnit.GetComponent<CombatUnit> ().unitType, numberDice,numberFlaresUsed);
				break;

			default:

				expectedDestroyedValue = 0.0f;
				break;

			}

			//we need to discount the EV by the likelihood of the flare stopping the torpedo.  Using flares reduces the chance of a hit, which 
			//reduces the expected destroyed value
			//each flare reduces the expected value by a factor of 5/6

			//Debug.Log("Expected before discount = " + expectedDestroyedValue);

			expectedDestroyedValue = expectedDestroyedValue * Mathf.Pow (5.0f / 6.0f, (float)numberFlaresUsed);

			//Debug.Log("Expected after discount = " + expectedDestroyedValue);

			return expectedDestroyedValue;

		}

	}


	//this function returns the total value of a phasor section
	private int GetPhasorSectionValue(CombatUnit targetedUnit){

		int sectionValue = 0;

		if (targetedUnit.GetComponent<PhasorSection> ().phasorRadarShot > 0) {

			sectionValue += targetedUnit.GetComponent<PhasorSection> ().phasorRadarShot * PurchaseManager.costPhasorRadarShot;

		}

		if (targetedUnit.GetComponent<PhasorSection> ().phasorRadarArray == true) {

			sectionValue += PurchaseManager.costPhasorRadarArray;

			}

		if (targetedUnit.GetComponent<PhasorSection> ().xRayKernalUpgrade == true) {

			sectionValue +=  PurchaseManager.costXRayKernel;

			}

		if (targetedUnit.GetComponent<PhasorSection> ().tractorBeam == true) {

			sectionValue +=  PurchaseManager.costTractorBeam;

		}

		return sectionValue;

	}

	//this function returns the total value of a torpedo section
	private int GetTorpedoSectionValue(CombatUnit targetedUnit){

		int sectionValue = 0;

		if (targetedUnit.GetComponent<TorpedoSection> ().torpedoLaserShot > 0) {

			sectionValue += targetedUnit.GetComponent<TorpedoSection> ().torpedoLaserShot * PurchaseManager.costTorpedoLaserShot;

		}

		if (targetedUnit.GetComponent<TorpedoSection> ().lightTorpedos > 0) {

			sectionValue += targetedUnit.GetComponent<TorpedoSection> ().lightTorpedos * PurchaseManager.costLightTorpedo;

		}

		if (targetedUnit.GetComponent<TorpedoSection> ().heavyTorpedos > 0) {

			sectionValue += targetedUnit.GetComponent<TorpedoSection> ().heavyTorpedos * PurchaseManager.costHeavyTorpedo;

		}

		if (targetedUnit.GetComponent<TorpedoSection> ().torpedoLaserGuidanceSystem == true) {

			sectionValue += PurchaseManager.costLaserGuidanceSystem;

		}

		if (targetedUnit.GetComponent<TorpedoSection> ().highPressureTubes == true) {

			sectionValue +=  PurchaseManager.costHighPressureTubes;

		}

		return sectionValue;

	}

	//this function returns the total value of a storage section
	private int GetStorageSectionValue(CombatUnit targetedUnit, int numberFlaresUsed){

		int sectionValue = 0;

		if (targetedUnit.GetComponent<StorageSection> ().dilithiumCrystals > 0) {

			sectionValue += targetedUnit.GetComponent<StorageSection> ().dilithiumCrystals * PurchaseManager.costDilithiumCrystal;

		}

		if (targetedUnit.GetComponent<StorageSection> ().trilithiumCrystals > 0) {

			sectionValue += targetedUnit.GetComponent<StorageSection> ().trilithiumCrystals * PurchaseManager.costTrilithiumCrystal;

		}

		if (targetedUnit.GetComponent<StorageSection> ().flares > 0) {

			//make sure we don't try to use more flares than we have
			if (numberFlaresUsed <= targetedUnit.GetComponent<StorageSection> ().flares) {

				sectionValue += (targetedUnit.GetComponent<StorageSection> ().flares - numberFlaresUsed) * PurchaseManager.costFlare;

			}
			//if we try to use more flares than we have, the value from flares is zero

		}

		if (targetedUnit.GetComponent<StorageSection> ().radarJammingSystem == true) {

			sectionValue += PurchaseManager.costRadarJammingSystem;

		}

		if (targetedUnit.GetComponent<StorageSection> ().laserScatteringSystem == true) {

			sectionValue +=  PurchaseManager.costTorpedoLaserScatteringSystem;

		}

		return sectionValue;

	}

	//this function returns the total value of a crew section
	private int GetCrewSectionValue(CombatUnit targetedUnit){

		int sectionValue = 0;


		if (targetedUnit.GetComponent<CrewSection> ().repairCrew == true) {

			sectionValue += PurchaseManager.costRepairCrew;

		}

		if (targetedUnit.GetComponent<CrewSection> ().shieldEngineeringTeam == true) {

			sectionValue +=  PurchaseManager.costShieldEngineeringTeam;

		}

		if (targetedUnit.GetComponent<CrewSection> ().battleCrew == true) {

			sectionValue +=  PurchaseManager.costBattleCrew;

		}

		return sectionValue;

	}

	//this function returns the total value of an engine section
	private int GetEngineSectionValue(CombatUnit targetedUnit){

		int sectionValue = 0;

		if (targetedUnit.GetComponent<EngineSection> ().warpBooster > 0) {

			sectionValue += targetedUnit.GetComponent<EngineSection> ().warpBooster * PurchaseManager.costWarpBooster;

		}

		if (targetedUnit.GetComponent<EngineSection> ().transwarpBooster > 0) {

			sectionValue += targetedUnit.GetComponent<EngineSection> ().transwarpBooster * PurchaseManager.costTranswarpBooster;

		}


		if (targetedUnit.GetComponent<EngineSection> ().warpDrive == true) {

			sectionValue += PurchaseManager.costWarpDrive;

		}

		if (targetedUnit.GetComponent<EngineSection> ().transwarpDrive == true) {

			sectionValue +=  PurchaseManager.costTranswarpDrive;

		}

		return sectionValue;

	}

	//this function returns the total value of a base phasor section 1
	private int GetBasePhasorSection1Value(CombatUnit targetedUnit){

		int sectionValue = 0;

		if (targetedUnit.GetComponent<StarbasePhasorSection1> ().phasorRadarShot > 0) {

			sectionValue += targetedUnit.GetComponent<StarbasePhasorSection1> ().phasorRadarShot * PurchaseManager.costPhasorRadarShot;

		}

		if (targetedUnit.GetComponent<StarbasePhasorSection1> ().phasorRadarArray == true) {

			sectionValue += PurchaseManager.costPhasorRadarArray;

		}

		return sectionValue;

	}

	//this function returns the total value of a base phasor section 2
	private int GetBasePhasorSection2Value(CombatUnit targetedUnit){

		int sectionValue = 0;

		if (targetedUnit.GetComponent<StarbasePhasorSection2> ().xRayKernalUpgrade == true) {

			sectionValue +=  PurchaseManager.costXRayKernel;

		}

		return sectionValue;

	}

	//this function returns the total value of a base torpedo section
	private int GetBaseTorpedoSectionValue(CombatUnit targetedUnit){

		int sectionValue = 0;

		if (targetedUnit.GetComponent<StarbaseTorpedoSection> ().torpedoLaserShot > 0) {

			sectionValue += targetedUnit.GetComponent<StarbaseTorpedoSection> ().torpedoLaserShot * PurchaseManager.costTorpedoLaserShot;

		}

		if (targetedUnit.GetComponent<StarbaseTorpedoSection> ().lightTorpedos > 0) {

			sectionValue += targetedUnit.GetComponent<StarbaseTorpedoSection> ().lightTorpedos * PurchaseManager.costLightTorpedo;

		}

		if (targetedUnit.GetComponent<StarbaseTorpedoSection> ().heavyTorpedos > 0) {

			sectionValue += targetedUnit.GetComponent<StarbaseTorpedoSection> ().heavyTorpedos * PurchaseManager.costHeavyTorpedo;

		}

		if (targetedUnit.GetComponent<StarbaseTorpedoSection> ().torpedoLaserGuidanceSystem == true) {

			sectionValue += PurchaseManager.costLaserGuidanceSystem;

		}

		if (targetedUnit.GetComponent<StarbaseTorpedoSection> ().highPressureTubes == true) {

			sectionValue +=  PurchaseManager.costHighPressureTubes;

		}

		return sectionValue;

	}


	//this function returns the total value of a base crew section
	private int GetBaseCrewSectionValue(CombatUnit targetedUnit){

		int sectionValue = 0;


		if (targetedUnit.GetComponent<StarbaseCrewSection> ().repairCrew == true) {

			sectionValue += PurchaseManager.costRepairCrew;

		}

		if (targetedUnit.GetComponent<StarbaseCrewSection> ().shieldEngineeringTeam == true) {

			sectionValue +=  PurchaseManager.costShieldEngineeringTeam;

		}

		if (targetedUnit.GetComponent<StarbaseCrewSection> ().battleCrew == true) {

			sectionValue +=  PurchaseManager.costBattleCrew;

		}

		return sectionValue;

	}

	//this function returns the total value of a base storage section 1
	private int GetBaseStorageSection1Value(CombatUnit targetedUnit){

		int sectionValue = 0;

		if (targetedUnit.GetComponent<StarbaseStorageSection1> ().dilithiumCrystals > 0) {

			sectionValue += targetedUnit.GetComponent<StarbaseStorageSection1> ().dilithiumCrystals * PurchaseManager.costDilithiumCrystal;

		}

		if (targetedUnit.GetComponent<StarbaseStorageSection1> ().radarJammingSystem == true) {

			sectionValue += PurchaseManager.costRadarJammingSystem;

		}

		return sectionValue;

	}

	//this function returns the total value of a base storage section 2
	private int GetBaseStorageSection2Value(CombatUnit targetedUnit, int numberFlaresUsed){

		int sectionValue = 0;

		if (targetedUnit.GetComponent<StarbaseStorageSection2> ().trilithiumCrystals > 0) {

			sectionValue += targetedUnit.GetComponent<StarbaseStorageSection2> ().trilithiumCrystals * PurchaseManager.costTrilithiumCrystal;

		}

		if (targetedUnit.GetComponent<StarbaseStorageSection2> ().flares > 0) {

			//make sure we don't try to use more flares than we have
			if (numberFlaresUsed <= targetedUnit.GetComponent<StarbaseStorageSection2> ().flares) {

				sectionValue += (targetedUnit.GetComponent<StarbaseStorageSection2> ().flares - numberFlaresUsed) * PurchaseManager.costFlare;

			}
			//if we try to use more flares than we have, the value from flares is zero

		}

		if (targetedUnit.GetComponent<StarbaseStorageSection2> ().laserScatteringSystem == true) {

			sectionValue +=  PurchaseManager.costTorpedoLaserScatteringSystem;

		}

		return sectionValue;

	}

	//this function returns the probabiltity that a section would be destroyed given the number of dice and the current shields
	private float GetDestroyedProbability(int numberDice, int currentShields){

		float probabilityDestroyed = 0.0f;

		if (numberDice == 3) {

			//check if the current shields are in the dice dictionary
			if (Dice.SumAtLeastProbability3D20.ContainsKey (currentShields) == true) {

				probabilityDestroyed = Dice.SumAtLeastProbability3D20 [currentShields] / 100f;

			}
			//if the key is not in the dictionary, the probabilty is zero
			else {

				probabilityDestroyed = 0.0f;

			}

		} else if (numberDice == 4) {

			//check if the current shields are in the dice dictionary
			if (Dice.SumAtLeastProbability4D20.ContainsKey (currentShields) == true) {

				probabilityDestroyed = Dice.SumAtLeastProbability4D20 [currentShields] / 100f;;

			}
			//if the key is not in the dictionary, the probabilty is zero
			else {

				probabilityDestroyed = 0.0f;

			}


		} else if (numberDice == 5) {

			//check if the current shields are in the dice dictionary
			if (Dice.SumAtLeastProbability5D20.ContainsKey (currentShields) == true) {

				probabilityDestroyed = Dice.SumAtLeastProbability5D20 [currentShields] / 100f;;

			}
			//if the key is not in the dictionary, the probabilty is zero
			else {

				probabilityDestroyed = 0.0f;

			}

		}

		return probabilityDestroyed;


	}

	//this function gets the expected value destroyed of a phasor section
	private float GetPhasorSectionDestroyedEV(CombatUnit targetedUnit, CombatUnit.UnitType unitType, int numberDice){

		float expectedDestroyedValue = 0.0f;

		//we need to check each section to see if it's destroyed, or how much value is at risk
		if (targetedUnit.GetComponent<PhasorSection> ().isDestroyed == false) {

			//there is only value at risk if the section is not destroyed already
			//define variable for section total value
			int sectionValue = GetPhasorSectionValue (targetedUnit);

			//get the current shields level of the section
			int sectionShields = targetedUnit.GetComponent<PhasorSection> ().shieldsCurrent;

			//get the probability that the attack would destroy the section if it hit
			float probabilitySectionDestruction = GetDestroyedProbability(numberDice, sectionShields);

			float shipSectionValue;

			switch (unitType) {

			case CombatUnit.UnitType.Starship:

				shipSectionValue = PurchaseManager.costStarship / 5.0f;
				break;
			case CombatUnit.UnitType.Destroyer:

				shipSectionValue = PurchaseManager.costDestroyer / 4.0f;
				break;
			case CombatUnit.UnitType.BirdOfPrey:

				shipSectionValue = PurchaseManager.costBirdOfPrey / 3.0f;
				break;

			case CombatUnit.UnitType.Scout:

				shipSectionValue = PurchaseManager.costScout / 3.0f;
				break;

			default:
				shipSectionValue = 00f;
				break;
			}

			//calculate the expectedDestroyedValue for this section
			expectedDestroyedValue = probabilitySectionDestruction * (sectionValue + shipSectionValue);

		}

		return expectedDestroyedValue;

	}

	//this function gets the expected value destroyed of a torpedo section
	private float GetTorpedoSectionDestroyedEV(CombatUnit targetedUnit, CombatUnit.UnitType unitType, int numberDice){

		float expectedDestroyedValue = 0.0f;

		//we need to check each section to see if it's destroyed, or how much value is at risk
		if (targetedUnit.GetComponent<TorpedoSection> ().isDestroyed == false) {

			//there is only value at risk if the section is not destroyed already
			//define variable for section total value
			int sectionValue = GetTorpedoSectionValue (targetedUnit);

			//get the current shields level of the section
			int sectionShields = targetedUnit.GetComponent<TorpedoSection> ().shieldsCurrent;

			//get the probability that the attack would destroy the section if it hit
			float probabilitySectionDestruction = GetDestroyedProbability(numberDice, sectionShields);

			float shipSectionValue;

			switch (unitType) {

			case CombatUnit.UnitType.Starship:

				shipSectionValue = PurchaseManager.costStarship / 5.0f;
				break;
			case CombatUnit.UnitType.Destroyer:

				shipSectionValue = PurchaseManager.costDestroyer / 4.0f;
				break;
			case CombatUnit.UnitType.BirdOfPrey:

				shipSectionValue = PurchaseManager.costBirdOfPrey / 3.0f;
				break;

			case CombatUnit.UnitType.Scout:

				shipSectionValue = PurchaseManager.costScout / 3.0f;
				break;

			default:
				shipSectionValue = 00f;
				break;
			}

			//calculate the expectedDestroyedValue for this section
			expectedDestroyedValue = probabilitySectionDestruction * (sectionValue + shipSectionValue);

		}

		return expectedDestroyedValue;

	}

	//this function gets the expected value destroyed of a storage section
	private float GetStorageSectionDestroyedEV(CombatUnit targetedUnit, CombatUnit.UnitType unitType, int numberDice, int numberFlaresUsed){

		float expectedDestroyedValue = 0.0f;

		//we need to check each section to see if it's destroyed, or how much value is at risk
		if (targetedUnit.GetComponent<StorageSection> ().isDestroyed == false) {

			//there is only value at risk if the section is not destroyed already
			//define variable for section total value
			int sectionValue = GetStorageSectionValue (targetedUnit, numberFlaresUsed);

			//get the current shields level of the section
			int sectionShields = targetedUnit.GetComponent<StorageSection> ().shieldsCurrent;

			//get the probability that the attack would destroy the section if it hit
			float probabilitySectionDestruction = GetDestroyedProbability(numberDice, sectionShields);

			float shipSectionValue;

			switch (unitType) {

			case CombatUnit.UnitType.Starship:

				shipSectionValue = PurchaseManager.costStarship / 5.0f;
				break;
			case CombatUnit.UnitType.Destroyer:

				shipSectionValue = PurchaseManager.costDestroyer / 4.0f;
				break;
			case CombatUnit.UnitType.BirdOfPrey:

				shipSectionValue = PurchaseManager.costBirdOfPrey / 3.0f;
				break;

			case CombatUnit.UnitType.Scout:

				shipSectionValue = PurchaseManager.costScout / 3.0f;
				break;

			default:
				shipSectionValue = 00f;
				break;
			}

			//calculate the expectedDestroyedValue for this section
			expectedDestroyedValue = probabilitySectionDestruction * (sectionValue + shipSectionValue);

		}

		return expectedDestroyedValue;

	}

	//this function gets the expected value destroyed of a crew section
	private float GetCrewSectionDestroyedEV(CombatUnit targetedUnit, CombatUnit.UnitType unitType, int numberDice){

		float expectedDestroyedValue = 0.0f;

		//we need to check each section to see if it's destroyed, or how much value is at risk
		if (targetedUnit.GetComponent<CrewSection> ().isDestroyed == false) {

			//there is only value at risk if the section is not destroyed already
			//define variable for section total value
			int sectionValue = GetCrewSectionValue (targetedUnit);

			//get the current shields level of the section
			int sectionShields = targetedUnit.GetComponent<CrewSection> ().shieldsCurrent;

			//get the probability that the attack would destroy the section if it hit
			float probabilitySectionDestruction = GetDestroyedProbability(numberDice, sectionShields);

			float shipSectionValue;

			switch (unitType) {

			case CombatUnit.UnitType.Starship:

				shipSectionValue = PurchaseManager.costStarship / 5.0f;
				break;
			case CombatUnit.UnitType.Destroyer:

				shipSectionValue = PurchaseManager.costDestroyer / 4.0f;
				break;
			case CombatUnit.UnitType.BirdOfPrey:

				shipSectionValue = PurchaseManager.costBirdOfPrey / 3.0f;
				break;

			case CombatUnit.UnitType.Scout:

				shipSectionValue = PurchaseManager.costScout / 3.0f;
				break;

			default:
				shipSectionValue = 00f;
				break;
			}

			//calculate the expectedDestroyedValue for this section
			expectedDestroyedValue = probabilitySectionDestruction * (sectionValue + shipSectionValue);

		}

		return expectedDestroyedValue;

	}

	//this function gets the expected value destroyed of an engine section
	private float GetEngineSectionDestroyedEV(CombatUnit targetedUnit, CombatUnit.UnitType unitType, int numberDice){

		float expectedDestroyedValue = 0.0f;

		//we need to check each section to see if it's destroyed, or how much value is at risk
		if (targetedUnit.GetComponent<EngineSection> ().isDestroyed == false) {

			//there is only value at risk if the section is not destroyed already
			//define variable for section total value
			int sectionValue = GetEngineSectionValue (targetedUnit);

			//get the current shields level of the section
			int sectionShields = targetedUnit.GetComponent<EngineSection> ().shieldsCurrent;

			//get the probability that the attack would destroy the section if it hit
			float probabilitySectionDestruction = GetDestroyedProbability(numberDice, sectionShields);

			float shipSectionValue;

			switch (unitType) {

			case CombatUnit.UnitType.Starship:

				shipSectionValue = PurchaseManager.costStarship / 5.0f;
				break;
			case CombatUnit.UnitType.Destroyer:

				shipSectionValue = PurchaseManager.costDestroyer / 4.0f;
				break;
			case CombatUnit.UnitType.BirdOfPrey:

				shipSectionValue = PurchaseManager.costBirdOfPrey / 3.0f;
				break;

			case CombatUnit.UnitType.Scout:

				shipSectionValue = PurchaseManager.costScout / 3.0f;
				break;

			default:
				shipSectionValue = 00f;
				break;
			}

			//calculate the expectedDestroyedValue for this section
			expectedDestroyedValue = probabilitySectionDestruction * (sectionValue + shipSectionValue);

		}

		return expectedDestroyedValue;

	}

	//this function gets the expected value destroyed of a base phasor section 1
	private float GetBasePhasorSection1DestroyedEV(CombatUnit targetedUnit, CombatUnit.UnitType unitType, int numberDice){

		float expectedDestroyedValue = 0.0f;

		//we need to check each section to see if it's destroyed, or how much value is at risk
		if (targetedUnit.GetComponent<StarbasePhasorSection1> ().isDestroyed == false) {

			//there is only value at risk if the section is not destroyed already
			//define variable for section total value
			int sectionValue = GetBasePhasorSection1Value (targetedUnit);

			//get the current shields level of the section
			int sectionShields = targetedUnit.GetComponent<StarbasePhasorSection1> ().shieldsCurrent;

			//get the probability that the attack would destroy the section if it hit
			float probabilitySectionDestruction = GetDestroyedProbability(numberDice, sectionShields);

			float baseSectionValue = 1000;

						//calculate the expectedDestroyedValue for this section
			expectedDestroyedValue = probabilitySectionDestruction * (sectionValue + baseSectionValue);

		}

		return expectedDestroyedValue;

	}

	//this function gets the expected value destroyed of a base phasor section 2
	private float GetBasePhasorSection2DestroyedEV(CombatUnit targetedUnit, CombatUnit.UnitType unitType, int numberDice){

		float expectedDestroyedValue = 0.0f;

		//we need to check each section to see if it's destroyed, or how much value is at risk
		if (targetedUnit.GetComponent<StarbasePhasorSection2> ().isDestroyed == false) {

			//there is only value at risk if the section is not destroyed already
			//define variable for section total value
			int sectionValue = GetBasePhasorSection2Value (targetedUnit);

			//get the current shields level of the section
			int sectionShields = targetedUnit.GetComponent<StarbasePhasorSection1> ().shieldsCurrent;

			//get the probability that the attack would destroy the section if it hit
			float probabilitySectionDestruction = GetDestroyedProbability(numberDice, sectionShields);

			float baseSectionValue = 1000;

			//calculate the expectedDestroyedValue for this section
			expectedDestroyedValue = probabilitySectionDestruction * (sectionValue + baseSectionValue);

		}

		return expectedDestroyedValue;

	}

	//this function gets the expected value destroyed of a base torpedo section
	private float GetBaseTorpedoSectionDestroyedEV(CombatUnit targetedUnit, CombatUnit.UnitType unitType, int numberDice){

		float expectedDestroyedValue = 0.0f;

		//we need to check each section to see if it's destroyed, or how much value is at risk
		if (targetedUnit.GetComponent<StarbaseTorpedoSection> ().isDestroyed == false) {

			//there is only value at risk if the section is not destroyed already
			//define variable for section total value
			int sectionValue = GetBaseTorpedoSectionValue (targetedUnit);

			//get the current shields level of the section
			int sectionShields = targetedUnit.GetComponent<StarbaseTorpedoSection> ().shieldsCurrent;

			//get the probability that the attack would destroy the section if it hit
			float probabilitySectionDestruction = GetDestroyedProbability(numberDice, sectionShields);

			float baseSectionValue = 1000;

			//calculate the expectedDestroyedValue for this section
			expectedDestroyedValue = probabilitySectionDestruction * (sectionValue + baseSectionValue);

		}

		return expectedDestroyedValue;

	}

	//this function gets the expected value destroyed of a base crew section
	private float GetBaseCrewSectionDestroyedEV(CombatUnit targetedUnit, CombatUnit.UnitType unitType, int numberDice){

		float expectedDestroyedValue = 0.0f;

		//we need to check each section to see if it's destroyed, or how much value is at risk
		if (targetedUnit.GetComponent<StarbaseCrewSection> ().isDestroyed == false) {

			//there is only value at risk if the section is not destroyed already
			//define variable for section total value
			int sectionValue = GetBaseCrewSectionValue (targetedUnit);

			//get the current shields level of the section
			int sectionShields = targetedUnit.GetComponent<StarbaseCrewSection> ().shieldsCurrent;

			//get the probability that the attack would destroy the section if it hit
			float probabilitySectionDestruction = GetDestroyedProbability(numberDice, sectionShields);

			float baseSectionValue = 1000;

			//calculate the expectedDestroyedValue for this section
			expectedDestroyedValue = probabilitySectionDestruction * (sectionValue + baseSectionValue);

		}

		return expectedDestroyedValue;

	}

	//this function gets the expected value destroyed of a base storage section 1
	private float GetBaseStorageSection1DestroyedEV(CombatUnit targetedUnit, CombatUnit.UnitType unitType, int numberDice){

		float expectedDestroyedValue = 0.0f;

		//we need to check each section to see if it's destroyed, or how much value is at risk
		if (targetedUnit.GetComponent<StarbaseStorageSection1> ().isDestroyed == false) {

			//there is only value at risk if the section is not destroyed already
			//define variable for section total value
			int sectionValue = GetBaseStorageSection1Value (targetedUnit);

			//get the current shields level of the section
			int sectionShields = targetedUnit.GetComponent<StarbaseStorageSection1> ().shieldsCurrent;

			//get the probability that the attack would destroy the section if it hit
			float probabilitySectionDestruction = GetDestroyedProbability(numberDice, sectionShields);

			float baseSectionValue = 1000;

			//calculate the expectedDestroyedValue for this section
			expectedDestroyedValue = probabilitySectionDestruction * (sectionValue + baseSectionValue);

		}

		return expectedDestroyedValue;

	}

	//this function gets the expected value destroyed of a base storage section 2
	private float GetBaseStorageSection2DestroyedEV(CombatUnit targetedUnit, CombatUnit.UnitType unitType, int numberDice, int numberFlaresUsed){

		float expectedDestroyedValue = 0.0f;

		//we need to check each section to see if it's destroyed, or how much value is at risk
		if (targetedUnit.GetComponent<StarbaseStorageSection2> ().isDestroyed == false) {

			//there is only value at risk if the section is not destroyed already
			//define variable for section total value
			int sectionValue = GetBaseStorageSection2Value (targetedUnit, numberFlaresUsed);

			//get the current shields level of the section
			int sectionShields = targetedUnit.GetComponent<StarbaseStorageSection2> ().shieldsCurrent;

			//get the probability that the attack would destroy the section if it hit
			float probabilitySectionDestruction = GetDestroyedProbability(numberDice, sectionShields);

			float baseSectionValue = 1000;

			//calculate the expectedDestroyedValue for this section
			expectedDestroyedValue = probabilitySectionDestruction * (sectionValue + baseSectionValue);

		}

		return expectedDestroyedValue;

	}

	//this function sets the border color
	private void SetBorderColor(Player.Color targetedPlayerColor){

		//switch case based on player
		switch (targetedPlayerColor) {

		case Player.Color.Green:

			flareMenuPanel.GetComponent<Image> ().color = GameManager.greenColor;
			break;

		case Player.Color.Purple:

			flareMenuPanel.GetComponent<Image> ().color = GameManager.purpleColor;
			break;

		case Player.Color.Red:

			flareMenuPanel.GetComponent<Image> ().color = GameManager.redColor;
			break;

		case Player.Color.Blue:

			flareMenuPanel.GetComponent<Image> ().color = GameManager.blueColor;
			break;

		default:

			break;

		}

	}

	//function to handle onDestroy
	private void OnDestroy(){

		RemoveAllListeners ();

	}

	//function to remove listeners
	private void RemoveAllListeners(){

		if (useFlaresButton != null) {
			
			//remove listeners for clicking the yes and cancel buttons
			useFlaresButton.onClick.RemoveListener (ClickedYes);

		}

		if (cancelFlaresButton != null) {
			
			cancelFlaresButton.onClick.RemoveListener (ClickedCancel);

		}

		//remove listener for torpedo attacks on ships
		CombatManager.OnLightTorpedoUntargetedAttackShipWithFlares.RemoveListener(lightTorpedoUntargetedAttackWithFlaresResolveFlareUseAction);

		CombatManager.OnHeavyTorpedoUntargetedAttackShipWithFlares.RemoveListener(heavyTorpedoUntargetedAttackWithFlaresResolveFlareUseAction);

		CombatManager.OnLightTorpedoTargetedAttackShipWithFlares.RemoveListener (lightTorpedoTargetedAttackWithFlaresResolveFlareUseAction);

		CombatManager.OnHeavyTorpedoTargetedAttackShipWithFlares.RemoveListener (heavyTorpedoTargetedAttackWithFlaresResolveFlareUseAction);

		//remove listener for torpedo attacks on bases
		//I am casting the base section targeted as a shipSectionTargeted so I can pass it to the same functions
		CombatManager.OnLightTorpedoUntargetedAttackBaseWithFlares.RemoveListener(lightTorpedoUntargetedAttackBaseWithFlaresResolveFlareUseAction);

		CombatManager.OnHeavyTorpedoUntargetedAttackBaseWithFlares.RemoveListener(heavyTorpedoUntargetedAttacBasekWithFlaresResolveFlareUseAction);

		CombatManager.OnLightTorpedoTargetedAttackBaseWithFlares.RemoveListener (lightTorpedoTargetedAttackBaseWithFlaresResolveFlareUseAction);

		CombatManager.OnHeavyTorpedoTargetedAttackBaseWithFlares.RemoveListener (heavyTorpedoTargetedAttackBaseWithFlaresResolveFlareUseAction);

		if (flareCountUpButton != null) {

			//remove listener for the up button
			flareCountUpButton.onClick.RemoveListener (ResolveFlareCountUpButtonPress);

		}

		if (flareCountDownButton != null) {

			//remove listener for the down button
			flareCountDownButton.onClick.RemoveListener (ResolveFlareCountDownButtonPress);

		}

		if (autoCalcFlaresButton != null) {

			//remove listener for the autocalc button 
			autoCalcFlaresButton.onClick.RemoveListener (ResolveAutoCalcFlaresButtonPress);

		}

	}

}
