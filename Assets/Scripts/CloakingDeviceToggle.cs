using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class CloakingDeviceToggle : MonoBehaviour {

	//the action menu toggle
	public Toggle cloakingDeviceToggle;

	//variables for the managers
	private MouseManager mouseManager;
	private GameManager gameManager;

	//these event are for turning on and off the cloaking device toggle
	public UnityEvent OnTurnedOnCloakingDeviceToggle = new UnityEvent();
	public UnityEvent OnTurnedOffCloakingDeviceToggle = new UnityEvent();

	//unityActions
	private UnityAction<GameManager.ActionMode> actionModeSetCloakingDeviceToggle;
	private UnityAction<bool> boolClickCloakingDeviceToggle;

	// Use this for initialization
	public void Init () {

		//set the actions
		actionModeSetCloakingDeviceToggle = (actionMode) => {SetCloakingDeviceToggle();};
		boolClickCloakingDeviceToggle = (value) => {ClickCloakingDeviceToggle ();};

		//get the managers
		gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
		mouseManager = GameObject.FindGameObjectWithTag("MouseManager").GetComponent<MouseManager>();

		//add an onValueChanged event listener 
		cloakingDeviceToggle.onValueChanged.AddListener(boolClickCloakingDeviceToggle);

		//add a listener to the selected unit event
		mouseManager.OnSetSelectedUnit.AddListener(SetCloakingDeviceToggle);
		mouseManager.OnClearSelectedUnit.AddListener(SetCloakingDeviceToggle);

		//add listener for the action mode changing
		gameManager.OnActionModeChange.AddListener(actionModeSetCloakingDeviceToggle);

		//set the toggle at startup
		SetCloakingDeviceToggle();

	}

	//this function handles clicking on the action menu toggle
	private void ClickCloakingDeviceToggle(){

		//check if we turned the toggle on
		if (cloakingDeviceToggle.isOn == true) {

			//Debug.Log ("We turned on the toggle");

			//invoke the turned on event
			OnTurnedOnCloakingDeviceToggle.Invoke();

		}

		else if (cloakingDeviceToggle.isOn == false) {

			//Debug.Log ("We turned the toggle off");

			//invoke the turned off event
			OnTurnedOffCloakingDeviceToggle.Invoke();

		}

	}

	//this function will set the state of the cloaking device toggle
	private void SetCloakingDeviceToggle(){

		//Debug.Log ("Call SetCloakingDeviceToggle");

		//check if we are in cloaking device mode
		if (gameManager.CurrentActionMode != GameManager.ActionMode.Cloaking) {

			//if we are not in cloaking mode, turn off the cloaking toggle
			cloakingDeviceToggle.isOn = false;

		}
		//the else condition is that we are in cloaking device mode
		else {

			//if we are in cloaking mode, turn on the cloaking toggle
			cloakingDeviceToggle.interactable = true;
			cloakingDeviceToggle.isOn = true;
		}

		//the cloaking toggle should not be interactable in certain gameManager actionModes
		if (UIManager.lockMenuActionModes.Contains(gameManager.CurrentActionMode) || gameManager.currentTurnPhase != GameManager.TurnPhase.MainPhase) {

			cloakingDeviceToggle.interactable = false;
			cloakingDeviceToggle.isOn = false;

		}
		//the else condition is that we are in a mode where the toggle is allowed to be interactable
		else {

			//if the selectedUnit is null (has been cleared)
			if (mouseManager.selectedUnit == null) {

				//enable the cloaking toggle if we are in selection mode
				cloakingDeviceToggle.interactable = true;

			}

			//the else condition is that we do have a selected unit
			else if (mouseManager.selectedUnit != null) {

				//check if the selected unit is owned by the current player
				if (mouseManager.selectedUnit.GetComponent<CombatUnit> ().owner == gameManager.currentTurnPlayer) {

					//check if the selected unit has a cloaking device
					if (mouseManager.selectedUnit.GetComponent<CloakingDevice> () == true) {

						//if the selected unit has a cloaking device, the toggle can be interactable
						cloakingDeviceToggle.interactable = true;

					}
					//the else condition is that the selected unit does not have a cloaking device
					else {

						//if the selected unit does not have a cloaking device, the toggle can't be interactable
						cloakingDeviceToggle.interactable = false;
						cloakingDeviceToggle.isOn = false;

					}

				}

				//the else condition is that we selected someone else's ship
				else {

					//turn the toggle off
					cloakingDeviceToggle.isOn = false;

					//make the cloaking device menu not interactable
					cloakingDeviceToggle.interactable = false;

				}

			}

		}

	}

	//function to remove listeners if gameobject is destroyed
	private void OnDestroy(){

		RemoveAllListeners ();

	}

	//this function removes all listeners
	private void RemoveAllListeners(){

		if (cloakingDeviceToggle != null) {
			
			cloakingDeviceToggle.onValueChanged.RemoveListener (boolClickCloakingDeviceToggle);

		}

		if (mouseManager != null) {
			
			mouseManager.OnSetSelectedUnit.RemoveListener (SetCloakingDeviceToggle);
			mouseManager.OnClearSelectedUnit.RemoveListener (SetCloakingDeviceToggle);

		}

		if (gameManager != null) {
			
			gameManager.OnActionModeChange.RemoveListener (actionModeSetCloakingDeviceToggle);

		}

	}

}
