using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UseItemToggle : MonoBehaviour {

	//the menu trigger toggle
	public Toggle useItemToggle;

	//variables for the managers
	private MouseManager mouseManager;
	private GameManager gameManager;

	//this event is for turning on and off the use item toggle
	public ToggleEvent OnTurnedOnUseItemToggle = new ToggleEvent();
	public UnityEvent OnTurnedOffUseItemToggle = new UnityEvent();

	//simple class derived from unityEvent to pass Toggle Object
	public class ToggleEvent : UnityEvent<Toggle>{};

	//unityActions
	private UnityAction<bool> boolClickUseItemToggleAction;
	private UnityAction<CombatUnit> combatUnitSetUseItemToggleAction;
	private UnityAction<GameManager.ActionMode> actionModeSetUseItemToggleAction;


	// Use this for initialization
	public void Init () {

		//get the managers
		gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
		mouseManager = GameObject.FindGameObjectWithTag("MouseManager").GetComponent<MouseManager>();

		//set actions
		boolClickUseItemToggleAction = (value) => {ClickUseItemToggle();};
		combatUnitSetUseItemToggleAction = (targetedUnit) => {SetUseItemToggle();};
		actionModeSetUseItemToggleAction = (actionMode) => {SetUseItemToggle();};

		//add an onValueChanged event listener 
		useItemToggle.onValueChanged.AddListener(boolClickUseItemToggleAction);

		//add a listener to the selected unit event
		mouseManager.OnSetSelectedUnit.AddListener(SetUseItemToggle);
		mouseManager.OnClearSelectedUnit.AddListener(SetUseItemToggle);

		//add a listener to the repair section event
		StorageSection.OnStorageSectionRepaired.AddListener(combatUnitSetUseItemToggleAction);
		StarbaseStorageSection1.OnStorageSection1Repaired.AddListener(combatUnitSetUseItemToggleAction);
		StarbaseStorageSection2.OnStorageSection2Repaired.AddListener(combatUnitSetUseItemToggleAction);

		//add listener for the action mode changing
		gameManager.OnActionModeChange.AddListener(actionModeSetUseItemToggleAction);

		//set the toggle at startup
		SetUseItemToggle();

	}
	
	private void ClickUseItemToggle(){

		//check if we turned the toggle on
		if (useItemToggle.isOn == true) {

			//Debug.Log ("We turned on the toggle");

			//invoke the OnTurnedOnUseItemToggle event, passing the toggle
			OnTurnedOnUseItemToggle.Invoke(useItemToggle);

		}

		else if (useItemToggle.isOn == false) {

			//Debug.Log ("We turned the toggle off");

			//invoke the OnTurnedOffUseItemToggle event
			OnTurnedOffUseItemToggle.Invoke();


		}

	}

	//this function will set the state of the useItem toggle
	private void SetUseItemToggle(){

		//check if we are in use item mode
		if (gameManager.CurrentActionMode != GameManager.ActionMode.ItemUse) {

			//if we are not in use item mode, turn off the item use toggle
			useItemToggle.isOn = false;

		}
		//the else condition is that we are in use item mode
		else {

			//if we are in use item mode, turn on the use item toggle
			useItemToggle.interactable = true;
			useItemToggle.isOn = true;
		}

		//the use item toggle should not be interactable in certain gameManager actionModes
		if (UIManager.lockMenuActionModes.Contains(gameManager.CurrentActionMode)|| gameManager.currentTurnPhase != GameManager.TurnPhase.MainPhase) {

			useItemToggle.interactable = false;
			useItemToggle.isOn = false;

		}
		//the else condition is that we are in a mode where the toggle is allowed to be interactable
		else {

			//if the selectedUnit is null (has been cleared)
			if (mouseManager.selectedUnit == null) {

				//enable the useItem toggle if we are in selection mode
				useItemToggle.interactable = true;

			}

			//the else condition is that we do have a selected unit
			else if (mouseManager.selectedUnit != null) {

				//check if the selected unit is owned by the current player
				if (mouseManager.selectedUnit.GetComponent<CombatUnit> ().owner == gameManager.currentTurnPlayer) {

					//check if the selected unit is a ship
					if (mouseManager.selectedUnit.GetComponent<Ship> () == true) {

						//check if the ship has a storage section
						if (mouseManager.selectedUnit.GetComponentInChildren<StorageSection> () == true) {

							//check if the section is not destroyed
							if (mouseManager.selectedUnit.GetComponentInChildren<StorageSection> ().isDestroyed == false) {

								useItemToggle.interactable = true;

							}

							//the else condition is the section is destroyed
							else {

								//if the toggle is currently on, turn it off
								if (useItemToggle.isOn == true) {

									useItemToggle.isOn = false;

								}

								//make the useItem menu not interactable
								useItemToggle.interactable = false;

							}

						}

						//the else condition is that we don't have a storage section
						else {

							//if the toggle is currently on, turn it off
							if (useItemToggle.isOn == true) {

								useItemToggle.isOn = false;

							}

							//make the useItem menu not interactable
							useItemToggle.interactable = false;

						}

					}

					//the else condition is that the selected unit is not a ship
					else if(mouseManager.selectedUnit.GetComponent<Starbase>()==true) {

						//check if the storage sections are not both destroyed
						if (mouseManager.selectedUnit.GetComponentInChildren<StarbaseStorageSection1> ().isDestroyed == false &&
						    mouseManager.selectedUnit.GetComponentInChildren<StarbaseStorageSection2> ().isDestroyed == false) {

							//if at least 1 storage section is alive, we can make the toggle interactable
							useItemToggle.interactable = true;

						}
						//the else condition is that both storage sections are destroyed
						else {

							useItemToggle.isOn = false;
							useItemToggle.interactable = false;

						}

					}

				}

				//the else condition is that we selected someone else's combat unit
				else {

					//if the toggle is currently on, turn it off
					if (useItemToggle.isOn == true) {

						useItemToggle.isOn = false;

					}

					//make the useItem menu not interactable
					useItemToggle.interactable = false;

				}

			}

		}

	}

	//this function handles OnDestroy
	private void OnDestroy(){

		RemoveAllListeners ();

	}

	//this function removes all listeners
	private void RemoveAllListeners(){

		if (useItemToggle != null) {
			
			//remove an onValueChanged event listener 
			useItemToggle.onValueChanged.RemoveListener (boolClickUseItemToggleAction);

		}

		if (mouseManager != null) {

			//remove a listener to the selected unit event
			mouseManager.OnSetSelectedUnit.RemoveListener (SetUseItemToggle);
			mouseManager.OnClearSelectedUnit.RemoveListener (SetUseItemToggle);

		}

		//remove a listener to the repair section event
		StorageSection.OnStorageSectionRepaired.RemoveListener (combatUnitSetUseItemToggleAction);
		StarbaseStorageSection1.OnStorageSection1Repaired.RemoveListener (combatUnitSetUseItemToggleAction);
		StarbaseStorageSection2.OnStorageSection2Repaired.RemoveListener (combatUnitSetUseItemToggleAction);

		if (gameManager != null) {

			//remove listener for the action mode changing
			gameManager.OnActionModeChange.RemoveListener (actionModeSetUseItemToggleAction);

		}

	}

}
