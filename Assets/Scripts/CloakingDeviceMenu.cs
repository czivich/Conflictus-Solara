using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class CloakingDeviceMenu : MonoBehaviour {

	//we will need the mouseManager to check the selected unit
	private MouseManager mouseMananger;

	//variable for cloaking device trigger toggle
	public Toggle cloakingDeviceToggle;

	//variable for Cloaking Device Menu cloaking device status toggle
	public Toggle cloakingDeviceStatusToggle;

	//variable to hold the trigger button
	public Button useCloakingDeviceButton;

	//these events are for when the toggles are clicked
	public CloakingEvent OnTurnOnCloakingDevice = new CloakingEvent();
	public CloakingEvent OnTurnOffCloakingDevice = new CloakingEvent();

	//simple class derived from unityEvent to pass Ship Object
	public class CloakingEvent : UnityEvent<CombatUnit>{};

	//unityActions for event listeners
	private UnityAction<bool> boolSetCloakingDeviceTogglesAction;

	// Use this for initialization
	public void Init () {

		//set the actions
		boolSetCloakingDeviceTogglesAction = (value) => {SetCloakingDeviceToggles();};

		//get the mouseManager
		mouseMananger = GameObject.FindGameObjectWithTag("MouseManager").GetComponent<MouseManager>();

		//add an on-click event listener for the main menu cloaking device toggle
		cloakingDeviceToggle.onValueChanged.AddListener(boolSetCloakingDeviceTogglesAction);

		//add an on-click event listener for the use cloaking device button
		useCloakingDeviceButton.onClick.AddListener(UseCloakingDevice);

		//add an event listener for when a selectedUnit is set and cleared
		mouseMananger.OnSetSelectedUnit.AddListener(SetCloakingDeviceToggles);
		mouseMananger.OnClearSelectedUnit.AddListener(SetCloakingDeviceToggles);

	}
	
	//set cloaking device toggles will set the menu interactability and on/off status based on the selected/targeted unit state
	private void SetCloakingDeviceToggles(){
		
		//the status toggle is always non interactable
		cloakingDeviceStatusToggle.interactable = false;

		//this is if the tractor beam Toggle has been turned on
		if (cloakingDeviceToggle.isOn == true) {

			//if there is no unit selected, set toggles to not interactable and turn them off
			if (mouseMananger.selectedUnit == null) {

				cloakingDeviceStatusToggle.isOn = false;

				useCloakingDeviceButton.interactable = false;

			}

			//the else condition means that there is a selected unit
			else {
				
				//check to see if the selected unit has a cloaking device.  If not, it can't use this menu
				if (mouseMananger.selectedUnit.GetComponentInChildren<CloakingDevice> () == null) {

					useCloakingDeviceButton.interactable = false;

				}

				//the else condition is that there is a cloaking device in the selected unit
				else {
					
					//we can set the state of the toggle based on the state of the device on the selected unit
					if (mouseMananger.selectedUnit.GetComponent<CloakingDevice> ().isCloaked == true) {

						cloakingDeviceStatusToggle.isOn = true;

					}
					//the else condition is that the cloaking device is off
					else if (mouseMananger.selectedUnit.GetComponent<CloakingDevice> ().isCloaked == false) {

						cloakingDeviceStatusToggle.isOn = false;

					}

					//now we need to check to see if we've already used the cloaking device this turn
					if (mouseMananger.selectedUnit.GetComponent<CloakingDevice> ().usedCloakingDeviceThisTurn == true) {

						//if we've already used the cloaking device, we need to make the button not interactable
						useCloakingDeviceButton.interactable = false;

					}

					//the else condition is that we have not the cloaking device yet this turn
					else {

						//we can make the button interactable
						useCloakingDeviceButton.interactable = true;

					}

				}

			}

		}

		//this is if the cloaking device Toggle has been turned off
		else if (cloakingDeviceToggle.isOn == false) {

			//make the menu items not interactable
			cloakingDeviceStatusToggle.isOn = false;

			useCloakingDeviceButton.interactable = false;

		}

		//update the button text
		UpdateButtonText();

		//update the cloaking device status toggle text
		UpdateCloakingDeviceStatusToggleText();

	}

	//this function updates the button text in the cloaking device menu
	private void UpdateButtonText(){

		//check if we have a selected unit
		if (mouseMananger.selectedUnit != null) {

			//check if the selected unit has a cloaking device
			if (mouseMananger.selectedUnit.GetComponent<CloakingDevice> () == true) {

				//check if the unit has a remaining cloak action
				if (mouseMananger.selectedUnit.GetComponent<BirdOfPrey> ().hasRemainingCloakAction == true) {

					//check whether the selected unit has the cloaking device on
					if (mouseMananger.selectedUnit.GetComponent<CloakingDevice> ().isCloaked == true) {

						//the button text should say Deactivate if the cloaking device is currently on
						useCloakingDeviceButton.GetComponentInChildren<TextMeshProUGUI> ().text = ("Deactivate");

						//update the font size if necessary
						UIManager.AutoSizeTextMeshFont (useCloakingDeviceButton.GetComponentInChildren<TextMeshProUGUI> ());

					}
				//the else condition is that the selected unit has the cloaking device off
				//check whether the selected unit has the cloaking device on
				else if (mouseMananger.selectedUnit.GetComponent<CloakingDevice> ().isCloaked == false) {

						//the button text should say Activate if the cloaking device is currently off
						useCloakingDeviceButton.GetComponentInChildren<TextMeshProUGUI> ().text = ("Activate");

						//update the font size if necessary
						UIManager.AutoSizeTextMeshFont (useCloakingDeviceButton.GetComponentInChildren<TextMeshProUGUI> ());

					}

				} else {

					//the else condition is that we do not have a remaining cloak action

					//the button text should say already used
					useCloakingDeviceButton.GetComponentInChildren<TextMeshProUGUI> ().text = ("Already Used");

					//update the font size if necessary
					UIManager.AutoSizeTextMeshFont (useCloakingDeviceButton.GetComponentInChildren<TextMeshProUGUI> ());

				}

			}

		}

	}

	//this private function is called when the use cloaking device button is clicked
	private void UseCloakingDevice(){

		//first, we need to check to see if the selected unit is cloaked or not when the button is pressed
		//this is the condition if we are currently not cloaked
		if (cloakingDeviceStatusToggle.isOn == false) {

			//invoke the OnTurnOnCloakingDevice event
			OnTurnOnCloakingDevice.Invoke(mouseMananger.selectedUnit.GetComponent<CombatUnit>());

			//update the font size if necessary
			UIManager.AutoSizeTextMeshFont(useCloakingDeviceButton.GetComponentInChildren<TextMeshProUGUI> ());

			//reset the menu toggles after pressing the button
			SetCloakingDeviceToggles();

			//update the cloaking device status toggle text
			UpdateCloakingDeviceStatusToggleText();

		}

		//this is the condition if we are cloaked and are turning it off
		else if (cloakingDeviceStatusToggle.isOn == true) {

			//invoke the OnTurnOffCloakingDevice event
			OnTurnOffCloakingDevice.Invoke(mouseMananger.selectedUnit.GetComponent<CombatUnit>());

			//update the font size if necessary
			UIManager.AutoSizeTextMeshFont(useCloakingDeviceButton.GetComponentInChildren<TextMeshProUGUI> ());

			//reset the menu toggles after pressing the button
			SetCloakingDeviceToggles();

			//update the cloaking device status toggle text
			UpdateCloakingDeviceStatusToggleText();

		}

	}

	//this function updates the cloaking device status toggle display
	private void UpdateCloakingDeviceStatusToggleText(){

		//check if we have a selected unit and it is cloaked
		if (mouseMananger.selectedUnit != null &&
		    mouseMananger.selectedUnit.GetComponent<CloakingDevice> () == true
		    && mouseMananger.selectedUnit.GetComponent<CloakingDevice> ().isCloaked == true) {

			//update the toggle text to say Cloaked
			cloakingDeviceStatusToggle.GetComponentInChildren<TextMeshProUGUI> ().text = ("Cloaked");

		} else {

			//the else condition is that we either don't have a selected unit, or we have no cloaking device, or we are not cloaked

			//update the toggle text to say Uncloaked
			cloakingDeviceStatusToggle.GetComponentInChildren<TextMeshProUGUI> ().text = ("Uncloaked");

		}

	}

	//this function removes all listeners if the gameobject is destroyed
	private void OnDestroy(){

		RemoveAllListeners ();

	}

	//function to remove all listeners
	private void RemoveAllListeners(){

		if (cloakingDeviceToggle != null) {
			
			cloakingDeviceToggle.onValueChanged.RemoveListener (boolSetCloakingDeviceTogglesAction);

		}

		if (useCloakingDeviceButton != null) {
			
			useCloakingDeviceButton.onClick.RemoveListener (UseCloakingDevice);

		}

		if (mouseMananger != null) {
			
			mouseMananger.OnSetSelectedUnit.RemoveListener (SetCloakingDeviceToggles);
			mouseMananger.OnClearSelectedUnit.RemoveListener (SetCloakingDeviceToggles);

		}

	}

}
