using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class TractorBeamMenu : MonoBehaviour {

	//we will need the mouseManager to check the selected unit
	private MouseManager mouseMananger;

	//variable for tractor beam trigger toggle
	public Toggle tractorBeamToggle;

	//variable for Tractor Beam Menu useTractorBeam toggle
	public Toggle useTractorBeamToggle;

	//variable to hold the system upgrade toggle
	public Toggle tractorBeamUpgradeToggle;


	//these events are for when the toggles are clicked
	public TractorBeamEvent OnTurnOnTractorBeamToggle = new TractorBeamEvent();
	public TractorBeamEvent OnTurnOffTractorBeamToggle = new TractorBeamEvent();

	//simple class derived from unityEvent to pass Ship Object
	public class TractorBeamEvent : UnityEvent<Ship>{};

	//unityActions
	private UnityAction<bool> boolSetTractorBeamTogglesAction;
	private UnityAction<bool> boolUseTractorBeamAction;
	private UnityAction<CombatUnit> combatUnitSetTractorBeamTogglesAction;


	// Use this for initialization
	public void Init () {

		//set actions
		boolSetTractorBeamTogglesAction = (value) => {SetTractorBeamToggles();};
		boolUseTractorBeamAction = (value) => {UseTractorBeam();};
		combatUnitSetTractorBeamTogglesAction = (combatUnit) => {SetTractorBeamToggles();};

		//get the mouseManager
		mouseMananger = GameObject.FindGameObjectWithTag("MouseManager").GetComponent<MouseManager>();

		//add an on-click event listener for the main menu tractor beam toggle
		tractorBeamToggle.onValueChanged.AddListener(boolSetTractorBeamTogglesAction);

		//add an on-click event listener for the tractor beam engage toggle
		useTractorBeamToggle.onValueChanged.AddListener(boolUseTractorBeamAction);

		//add an event listener for when a selectedUnit is set and cleared
		mouseMananger.OnSetSelectedUnit.AddListener(SetTractorBeamToggles);
		mouseMananger.OnClearSelectedUnit.AddListener(SetTractorBeamToggles);

		//add an event listener for when a targetedUnit is set and cleared
		mouseMananger.OnSetTargetedUnit.AddListener(SetTractorBeamToggles);
		mouseMananger.OnClearTargetedUnit.AddListener(SetTractorBeamToggles);

		//add a listener for when a purchase is complete
		StorageSection.OnInventoryUpdated.AddListener(combatUnitSetTractorBeamTogglesAction);

	}
	

	//set tractor beam toggles will set the menu interactability and on/off status based on the selected/targeted unit state
	private void SetTractorBeamToggles(){

		//this is if the tractor beam Toggle has been turned on
		if (tractorBeamToggle.isOn == true) {

			//if there is no unit selected, set toggles to not interactable and turn them off
			if (mouseMananger.selectedUnit == null) {

				useTractorBeamToggle.isOn = false;
				useTractorBeamToggle.interactable = false;

				tractorBeamUpgradeToggle.isOn = false;
				tractorBeamUpgradeToggle.interactable = false;

			}

			//the else condition means that there is a selected unit
			else {

				//now we need to make sure the selected unit is a ship
				//if it's a starbase, we need to set toggles to not interactable
				if (mouseMananger.selectedUnit.GetComponent<Ship> () == null) {

					useTractorBeamToggle.interactable = false;

					tractorBeamUpgradeToggle.isOn = false;
					tractorBeamUpgradeToggle.interactable = false;

				}

				//the else condition is that we do have a ship, so we want to allow toggles based on the ship inventory
				else {

					//check to see if the ship has a phasor section.  If not, it can't use tractor beams
					if (mouseMananger.selectedUnit.GetComponentInChildren<PhasorSection> () == null) {

						useTractorBeamToggle.interactable = false;

						tractorBeamUpgradeToggle.isOn = false;
						tractorBeamUpgradeToggle.interactable = false;

					}

					//the else condition is that there is a phasor section in the selected unit
					else {

						//now we need to make sure the phasor section hasn't been destroyed
						if (mouseMananger.selectedUnit.GetComponentInChildren<PhasorSection> ().isDestroyed == true) {

							//if it is destroyed, we can't use tractor beam
							useTractorBeamToggle.interactable = false;

							tractorBeamUpgradeToggle.isOn = false;
							tractorBeamUpgradeToggle.interactable = false;

						}

						//the else condition is that the phasor section is not destroyed.  
						else {

							//Now we need to check if it has a tractor beam item.
							if (mouseMananger.selectedUnit.GetComponentInChildren<PhasorSection> ().tractorBeam == false) {

								//if it doesn't have a tractor beam, it can's use it
								useTractorBeamToggle.interactable = false;

								tractorBeamUpgradeToggle.isOn = false;
								tractorBeamUpgradeToggle.interactable = false;

							}

							//the else condition is that we do have a tractor beam item
							else {

								//if we have the tractor beam item, sohow it as a system upgrade
								tractorBeamUpgradeToggle.isOn = true;
								tractorBeamUpgradeToggle.interactable = false;

								//now we need to check to see if we've already used phasors this turn
								if (mouseMananger.selectedUnit.GetComponentInChildren<PhasorSection> ().usedPhasorsThisTurn == true) {

									//if we've already used phasors, but the tractor beam is still engaged, that means we're in the middle of using the tractor beam
									//this means that we should still allow the tractor beam to be turned off
									if (mouseMananger.selectedUnit.GetComponentInChildren<PhasorSection> ().tractorBeamIsEngaged == true) {

										//now we need to check to see if we have a unit targeted
										if (mouseMananger.targetedUnit == null) {

											//if there is no targeted unit, we can't use the tractor beam
											//in fact, we turned it off
											useTractorBeamToggle.isOn = false;
											useTractorBeamToggle.interactable = false;

										}

										//the else condition is that we do have a unit targeted
										else {

											//if we have a valid target, we can use the tractor beam
											useTractorBeamToggle.interactable = true;

										}

									} 

									//the else condition is that the tractor beam is not currently engaged
									else {
										
										//if we've already used phasors, we can't use the tractor beam
										useTractorBeamToggle.interactable = false;

									}

								}

								//the else condition is that we have not used phasors yet
								else {

									//now we need to check to see if we have a unit targeted
									if (mouseMananger.targetedUnit == null) {

										//if there is no targeted unit, we can't use the tractor beam
										//we also need to turn it off
										useTractorBeamToggle.isOn = false;
										useTractorBeamToggle.interactable = false;

									}

									//the else condition is that we do have a unit targeted
									else {

										//if we have a valid target, we can use the tractor beam
										useTractorBeamToggle.interactable = true;

									}

								}

							}

						}

					}
				
				}

			}

		}
			
		//this is if the tractor beam Toggle has been turned off
		else if (tractorBeamToggle.isOn == false) {

			//if we have don't have a selected unit, we can turn the useTractorBeam toggle off
			if (mouseMananger.selectedUnit == null) {
				
				//we want to clear the use tractor beam toggle off
				useTractorBeamToggle.isOn = false;

				tractorBeamUpgradeToggle.isOn = false;
				tractorBeamUpgradeToggle.interactable = false;

			}

			//now we can check if we have a targeted unit
			if (mouseMananger.targetedUnit == null) {

				//if there is no targeted unit anymore, we can set the useTractorBeamToggle to false
				useTractorBeamToggle.isOn = false;



			}

		}

	}

	//this private function is called when the use tractor beam toggle is clicked
	private void UseTractorBeam(){

		//first, we need to check to see if we are turning the toggle on or off
		//this is the condition if we are turning it on
		if (useTractorBeamToggle.isOn == true) {

			//invoke the OnTurnOnTractorBeamToggle event
			OnTurnOnTractorBeamToggle.Invoke(mouseMananger.selectedUnit.GetComponent<Ship>());

			//update the toggle text to say disengage
			useTractorBeamToggle.GetComponentInChildren<TextMeshProUGUI> ().text = ("Disengage");

			//update the font size if necessary
			UIManager.AutoSizeTextMeshFont(useTractorBeamToggle.GetComponentInChildren<TextMeshProUGUI> ());

		}

		//this is the condition if we are turning it off
		else if (useTractorBeamToggle.isOn == false) {

			//if we have no targeted unit, we can disengage the tractor beam
			if (mouseMananger.targetedUnit != null) {

				//invoke the OnTurnOffTractorBeamToggle event
				OnTurnOffTractorBeamToggle.Invoke(mouseMananger.selectedUnit.GetComponent<Ship>());

			}

			//check to see if we have a selected unit
			else if (mouseMananger.selectedUnit != null) {

				//invoke the OnTurnOffTractorBeamToggle event
				OnTurnOffTractorBeamToggle.Invoke(mouseMananger.selectedUnit.GetComponent<Ship>());


			}

			//reset the tractor beam toggles after disengaging
			SetTractorBeamToggles();

			//we want to update the toggle text to say engage again
			useTractorBeamToggle.GetComponentInChildren<TextMeshProUGUI> ().text = ("Engage");

			//update the font size if necessary
			UIManager.AutoSizeTextMeshFont(useTractorBeamToggle.GetComponentInChildren<TextMeshProUGUI> ());

		}

	}

	//this function handles OnDestroy
	private void OnDestroy(){

		RemoveAllListeners ();

	}

	//this function removes all listeners
	private void RemoveAllListeners(){

		if (tractorBeamToggle != null) {

			//remove an on-click event listener for the main menu tractor beam toggle
			tractorBeamToggle.onValueChanged.RemoveListener (boolSetTractorBeamTogglesAction);

		}

		if (useTractorBeamToggle != null) {

			//remove an on-click event listener for the tractor beam engage toggle
			useTractorBeamToggle.onValueChanged.RemoveListener (boolUseTractorBeamAction);

		}

		if (mouseMananger != null) {

			//remove an event listener for when a selectedUnit is set and cleared
			mouseMananger.OnSetSelectedUnit.RemoveListener (SetTractorBeamToggles);
			mouseMananger.OnClearSelectedUnit.RemoveListener (SetTractorBeamToggles);

			//remove an event listener for when a targetedUnit is set and cleared
			mouseMananger.OnSetTargetedUnit.RemoveListener (SetTractorBeamToggles);
			mouseMananger.OnClearTargetedUnit.RemoveListener (SetTractorBeamToggles);

		}

		//remove a listener for when a purchase is complete
		StorageSection.OnInventoryUpdated.RemoveListener(combatUnitSetTractorBeamTogglesAction);

	}

}
