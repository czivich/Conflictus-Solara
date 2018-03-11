using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class MoveMenu : MonoBehaviour {

	//we will need the mouseManager to check the selected unit
	private MouseManager mouseMananger;

	//the UI elements are all public so they can be hooked up in the inspector
	//variable for Move trigger toggle
	public Toggle moveToggle;

	//variable for Move Menu Warp Booster toogle
	public Toggle useWarpBoosterToggle;

	//variable for Move Menu Transwarp Booster toogle
	public Toggle useTranswarpBoosterToggle;

	//variable for Move Menu Warp Drive toggle
	public Toggle warpDriveToggle;

	//variable for Move Menu Transwarp Drive toggle
	public Toggle transwarpDriveToggle;

	//variables for the item counts
	public Toggle warpBoosterCount;
	public Toggle transwarpBoosterCount;

	//these events are for when the booster toggles are clicked
	public MoveEvent OnTurnOnTranswarpBoosterToggle = new MoveEvent();
	public MoveEvent OnTurnOnWarpBoosterToggle = new MoveEvent();
	public MoveEvent OnTurnOffTranswarpBoosterToggle = new MoveEvent();
	public MoveEvent OnTurnOffWarpBoosterToggle = new MoveEvent();

	//simple class derived from unityEvent to pass Ship Object
	public class MoveEvent : UnityEvent<Ship>{};

	//unityActions
	private UnityAction<bool> boolSetMoveToggleAction;
	private UnityAction<bool> boolUseTranswarpBoosterAction;
	private UnityAction<bool> boolUseWarpBoosterAction;
	private UnityAction<Ship> shipSetMoveTogglesAction;
	private UnityAction<CombatUnit> combatUnitSetMoveTogglesAction;


	// Use this for initialization
	public void Init () {
		
		//get the mouseManager
		mouseMananger = GameObject.FindGameObjectWithTag("MouseManager").GetComponent<MouseManager>();

		//set the actions
		boolSetMoveToggleAction = (value) => {SetMoveToggles();};
		boolUseTranswarpBoosterAction = (value) => {UseTranswarpBooster();};
		boolUseWarpBoosterAction = (value) => {UseWarpBooster();};
		shipSetMoveTogglesAction = (ship) => {

			//Debug.Log(ship.shipName + " shipSetMoveTogglesAction");

			if(ship.GetComponent<EngineSection>().isBeingTowed == false){
				
				//Debug.Log(ship.shipName + " isBeingTowed = false");
				SetMoveToggles();
			}
		};
			
		combatUnitSetMoveTogglesAction = (combatUnit) => {SetMoveToggles ();};

		//add an on-click event listener for the main menu move toggle
		moveToggle.onValueChanged.AddListener(boolSetMoveToggleAction);

		//add an on-click event listener for the transwarp booster toggle
		useTranswarpBoosterToggle.onValueChanged.AddListener(boolUseTranswarpBoosterAction);

		//add an on-click event listener for the warp booster toggle
		useWarpBoosterToggle.onValueChanged.AddListener(boolUseWarpBoosterAction);

		//add an event listener for when a selectedUnit is set and cleared
		mouseMananger.OnSetSelectedUnit.AddListener(SetMoveToggles);
		mouseMananger.OnClearSelectedUnit.AddListener(SetMoveToggles);

		//add an event listener for when a ship has finished moving
		EngineSection.OnMoveFinish.AddListener(shipSetMoveTogglesAction);

		//add a listener for when a purchase is complete
		EngineSection.OnInventoryUpdated.AddListener(combatUnitSetMoveTogglesAction);

	}
		

	//this void will set the move toggles when the drop down menu is activated
	private void SetMoveToggles(){

		//Debug.Log ("SetMoveToggles");

		//this is if the move Toggle has been turned on
		if (moveToggle.isOn == true) {

			useWarpBoosterToggle.isOn = false;
			useTranswarpBoosterToggle.isOn = false;
			warpDriveToggle.isOn = false;
			transwarpDriveToggle.isOn = false;

			//if there is no unit selected, set toggles to not interactable
			if (mouseMananger.selectedUnit == null) {

				useWarpBoosterToggle.interactable = false;
				useTranswarpBoosterToggle.interactable = false;
				warpDriveToggle.interactable = false;
				transwarpDriveToggle.interactable = false;
				warpBoosterCount.interactable = false;
				transwarpBoosterCount.interactable = false;

			}

			//the else condition means that there is a selected unit
			else {

				//now we need to make sure the selected unit is a ship
				//if it's a starbase, we need to set toggles to not interactable
				if (mouseMananger.selectedUnit.GetComponent<Ship> () == null) {

					useWarpBoosterToggle.interactable = false;
					useTranswarpBoosterToggle.interactable = false;
					warpDriveToggle.interactable = false;
					transwarpDriveToggle.interactable = false;
					warpBoosterCount.interactable = false;
					transwarpBoosterCount.interactable = false;

				}

				//the else condition is that we do have a ship, so we want to allow toggles based on the ship inventory
				else {

					//set the counts based on ship inventory
					warpBoosterCount.GetComponentInChildren<TextMeshProUGUI>().text = mouseMananger.selectedUnit.GetComponentInChildren<EngineSection> ().warpBooster.ToString();
					transwarpBoosterCount.GetComponentInChildren<TextMeshProUGUI>().text = mouseMananger.selectedUnit.GetComponentInChildren<EngineSection> ().transwarpBooster.ToString();

					//if the ship has transwarp drive, set that toggle to on and make all lesser toggles not interactable
					if (mouseMananger.selectedUnit.GetComponentInChildren<EngineSection> ().transwarpDrive == true) {

						transwarpDriveToggle.isOn = true;

						transwarpDriveToggle.interactable = false;
						warpDriveToggle.interactable = false;
						useTranswarpBoosterToggle.interactable = false;
						useWarpBoosterToggle.interactable = false;
						warpBoosterCount.interactable = false;
						transwarpBoosterCount.interactable = false;

					}

					//if no transwarp drive, check for warp drive
					else if (mouseMananger.selectedUnit.GetComponentInChildren<EngineSection> ().warpDrive == true) {

						warpDriveToggle.isOn = true;

						//still want to allow a transwarp booster to be used
						transwarpDriveToggle.interactable = false;
						warpDriveToggle.interactable = false;
						useWarpBoosterToggle.interactable = false;
						transwarpBoosterCount.interactable = false;

					}

					//if neither drive is present, allow boosters if the ship has them
					else {

						//make both drives not interactable
						transwarpDriveToggle.interactable = false;
						warpDriveToggle.interactable = false;

						//if we've already used a transwarp booster, we want to set it to on and not interactable, and regular warp booster not interactable
						if (mouseMananger.selectedUnit.GetComponentInChildren<EngineSection> ().usedTranswarpBoosterThisTurn == true) {

							useTranswarpBoosterToggle.isOn = true;
							useTranswarpBoosterToggle.interactable = false;

							useWarpBoosterToggle.interactable = false;

							warpBoosterCount.interactable = false;
							transwarpBoosterCount.interactable = false;

						}

						//else if we've already used a warp booster, we want to set it to on and not interactable
						else if (mouseMananger.selectedUnit.GetComponentInChildren<EngineSection> ().usedWarpBoosterThisTurn == true) {

							useWarpBoosterToggle.isOn = true;
							useWarpBoosterToggle.interactable = false;

							warpBoosterCount.interactable = false;

							//if the ship has no transwarp boosters, set to not interactable
							if (mouseMananger.selectedUnit.GetComponentInChildren<EngineSection> ().transwarpBooster == 0) {

								useTranswarpBoosterToggle.interactable = false;

								transwarpBoosterCount.interactable = false;

							} else if (mouseMananger.selectedUnit.GetComponentInChildren<EngineSection> ().transwarpBooster > 0) {

								useTranswarpBoosterToggle.interactable = true;

								transwarpBoosterCount.interactable = true;


							}

						}

						//the else case here is that we haven't used a booster yet this turn
						else {

							//if the ship has no transwarp boosters, set to not interactable
							if (mouseMananger.selectedUnit.GetComponentInChildren<EngineSection> ().transwarpBooster == 0) {

								useTranswarpBoosterToggle.interactable = false;

								transwarpBoosterCount.interactable = false;


							} else if (mouseMananger.selectedUnit.GetComponentInChildren<EngineSection> ().transwarpBooster > 0) {
								
								useTranswarpBoosterToggle.interactable = true;

								transwarpBoosterCount.interactable = true;


							}

							//if the ship has no warp boosters, set to not interactable
							if (mouseMananger.selectedUnit.GetComponentInChildren<EngineSection> ().warpBooster == 0) {

								useWarpBoosterToggle.interactable = false;

								warpBoosterCount.interactable = false;


							} else if (mouseMananger.selectedUnit.GetComponentInChildren<EngineSection> ().warpBooster > 0) {

								useWarpBoosterToggle.interactable = true;

								warpBoosterCount.interactable = true;


							}
						
						}

					}

				}

			}
				
		} 

		//this is if the move Toggle has been turned off
		else if (moveToggle.isOn == false) {

			//we want to clear the warp booster and transwarp booster toggles
			useWarpBoosterToggle.isOn = false;
			useTranswarpBoosterToggle.isOn = false;

			//we want to clear the warp drive and transwarp drive toggles
			warpDriveToggle.isOn = false;
			transwarpDriveToggle.isOn = false;

		}

	}

	//this function is called when the transwarp booster toggle is clicked
	private void UseTranswarpBooster(){

		//first, we need to check if we're turning it on or turning it off
		//if we are turning it on, we want to turn off the useWarpBoosterToggle
		if (useTranswarpBoosterToggle.isOn == true) {

			//need to check to make sure we actually have a unit selected
			if (mouseMananger.selectedUnit != null) {

				//turn off the warp booster toggle if we haven't already used it this turn
				if (mouseMananger.selectedUnit.GetComponentInChildren<EngineSection> ().usedWarpBoosterThisTurn == false) {
				
					useWarpBoosterToggle.isOn = false;

				}
					
				//invoke the OnTurnOnTranswarpBoosterToggle event
				OnTurnOnTranswarpBoosterToggle.Invoke(mouseMananger.selectedUnit.GetComponent<Ship>());

			}

		}

		//the else case is we are turning it off - in this case, we want to restore the movement ranges
		else {

			//need to check to make sure we actually have a unit selected
			if (mouseMananger.selectedUnit != null) {

				//invoke the OnTurnOffTranswarpBoosterToggle event
				OnTurnOffTranswarpBoosterToggle.Invoke(mouseMananger.selectedUnit.GetComponent<Ship>());

			}

		}

	}

	//this function is called when the warp booster toggle is clicked
	private void UseWarpBooster(){

		//first, we need to check if we're turning it on or turning it off
		//if we are turning it on, we want to turn off the useTranswarpBoosterToggle
		if (useWarpBoosterToggle.isOn == true) {

			//need to check to make sure we actually have a unit selected
			if (mouseMananger.selectedUnit != null) {

				//turn off the transwarp booster toggle if we haven't already used it this turn
				if (mouseMananger.selectedUnit.GetComponentInChildren<EngineSection> ().usedTranswarpBoosterThisTurn == false) {
				
					useTranswarpBoosterToggle.isOn = false;

				}

				//invoke the OnTurnOnWarpBoosterToggle event
				OnTurnOnWarpBoosterToggle.Invoke(mouseMananger.selectedUnit.GetComponent<Ship>());

			}

		}

		//the else case is we are turning it off - in this case, we want to restore the movement ranges
		else {

			//need to check to make sure we actually have a unit selected
			if (mouseMananger.selectedUnit != null) {

				//invoke the OnTurnOffWarpBoosterToggle event
				OnTurnOffWarpBoosterToggle.Invoke(mouseMananger.selectedUnit.GetComponent<Ship>());

			}

		}

	}

	//function to handle OnDestroy
	private void OnDestroy(){

		RemoveAllListeners ();

	}

	//function to remove all listeners
	private void RemoveAllListeners(){

		if (moveToggle != null) {
			
			//remove an on-click event listener for the main menu move toggle
			moveToggle.onValueChanged.RemoveListener (boolSetMoveToggleAction);

		}

		if (useTranswarpBoosterToggle != null) {
			
			//remove an on-click event listener for the transwarp booster toggle
			useTranswarpBoosterToggle.onValueChanged.RemoveListener (boolUseTranswarpBoosterAction);

		}

		if (useWarpBoosterToggle != null) {
			
			//remove an on-click event listener for the warp booster toggle
			useWarpBoosterToggle.onValueChanged.RemoveListener (boolUseWarpBoosterAction);

		}

		if (mouseMananger != null) {
			
			//remove an event listener for when a selectedUnit is set and cleared
			mouseMananger.OnSetSelectedUnit.RemoveListener (SetMoveToggles);
			mouseMananger.OnClearSelectedUnit.RemoveListener (SetMoveToggles);

		}

		//remove an event listener for when a ship has finished moving
		EngineSection.OnMoveFinish.RemoveListener(shipSetMoveTogglesAction);

		//remove a listener for when a purchase is complete
		EngineSection.OnInventoryUpdated.RemoveListener(combatUnitSetMoveTogglesAction);

	}

}
