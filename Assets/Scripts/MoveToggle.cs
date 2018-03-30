using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class MoveToggle : MonoBehaviour {

	//variables for the managers
	private MouseManager mouseManager;
	private GameManager gameManager;

	//the move toggle - public so it can be assigned in the inspector
	public Toggle moveToggle;

	//this event is for turning on the move toggle
	public ToggleEvent OnTurnedOnMoveToggle = new ToggleEvent();

	//this event is for turning off the move toggle
	public UnityEvent OnTurnedOffMoveToggle = new UnityEvent();

	//simple class derived from unityEvent to pass Toggle Object
	public class ToggleEvent : UnityEvent<Toggle>{};

	//unityActions
	private UnityAction<bool> boolClickMoveToggleAction;
	private UnityAction<CombatUnit> combatUnitSetMoveToggleAction;
	private UnityAction<GameManager.ActionMode> actionModeSetMoveToggleAction;


	// Use this for initialization
	public void Init () {

		//get the managers
		gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
		mouseManager = GameObject.FindGameObjectWithTag("MouseManager").GetComponent<MouseManager>();

		//set the actions
		boolClickMoveToggleAction = (value) => {ClickMoveToggle();};
		combatUnitSetMoveToggleAction = (targetedUnit) => {SetMoveToggle();};
		actionModeSetMoveToggleAction = (actionMode) => {SetMoveToggle();};

		//add an onValueChanged event listener 
		moveToggle.onValueChanged.AddListener(boolClickMoveToggleAction);

		//add a listener to the selected unit event
		mouseManager.OnSetSelectedUnit.AddListener(SetMoveToggle);
		mouseManager.OnClearSelectedUnit.AddListener(SetMoveToggle);

		//add a listener for the repair crew event
		EngineSection.OnEngineSectionRepaired.AddListener(combatUnitSetMoveToggleAction);

		//add listener for the action mode changing
		gameManager.OnActionModeChange.AddListener(actionModeSetMoveToggleAction);

		//set the toggle at startup
		SetMoveToggle();

	}

	private void ClickMoveToggle(){

		//check if we turned the toggle on
		if (moveToggle.isOn == true) {

			//Debug.Log ("We turned on the toggle");

			//invoke the OnTurnedOnMoveToggle event, passing the moveToggle
			OnTurnedOnMoveToggle.Invoke(moveToggle);

		}

		else if (moveToggle.isOn == false) {

			//Debug.Log ("We turned the toggle off");

			//invoke the OnTurnedOffMoveToggle event
			OnTurnedOffMoveToggle.Invoke ();

		}

	}

	//this function will set the interactability of the move toggle based on the unit selected
	private void SetMoveToggle(){

		//check if we are in Movement mode
		if (gameManager.CurrentActionMode != GameManager.ActionMode.Movement) {

			//if we are not in movement mode, turn off the move toggle
			moveToggle.isOn = false;

		}
		//the else condition is that we are in movement mode
		else {

			//if we are in movement mode, turn on the move toggle
			moveToggle.interactable = true;
			moveToggle.isOn = true;
		}

		//the move toggle should not be interactable in certain gameManager actionModes
		if (UIManager.lockMenuActionModes.Contains(gameManager.CurrentActionMode)|| gameManager.currentTurnPhase != GameManager.TurnPhase.MainPhase) {

			moveToggle.interactable = false;
			moveToggle.isOn = false;

		}
		//the else condition is that we are in a mode where the toggle is allowed to be interactable
		else {

			//if the selectedUnit is null (has been cleared)
			if (mouseManager.selectedUnit == null) {

				//enable the move toggle if we are in selection mode
				moveToggle.interactable = true;

			}

		//the else condition is that we do have a selected unit
		else if (mouseManager.selectedUnit != null) {

				//check if the selected unit is owned by the current turn player
				if (mouseManager.selectedUnit.GetComponent<CombatUnit> ().owner == gameManager.currentTurnPlayer) {

					//check if the selected unit is a ship
					if (mouseManager.selectedUnit.GetComponent<Ship> () == true) {

						//check if the ship has an engine section
						if (mouseManager.selectedUnit.GetComponentInChildren<EngineSection> () == true) {

							//if we have an engine section, check if the section has been destroyed
							if (mouseManager.selectedUnit.GetComponentInChildren<EngineSection> ().isDestroyed == false) {

								//we are not destroyed, so we can allow the toggle to be interacted with
								moveToggle.interactable = true;

							}

							//the else condition is that the section is destroyed
							else {

								//if the toggle is currently on, turn it off
								if (moveToggle.isOn == true) {

									moveToggle.isOn = false;

								}

								//make the move menu not interactable
								moveToggle.interactable = false;

							}

						}

						//the else condition is that we don't have an engine section
						else {

							//if the toggle is currently on, turn it off
							if (moveToggle.isOn == true) {

								moveToggle.isOn = false;

							}

							//make the move menu not interactable
							moveToggle.interactable = false;

						}

					}

					//the else condition is that the selected unit is not a ship
					else if(mouseManager.selectedUnit.GetComponent<Starbase>() == true){

						//if the toggle is currently on, turn it off
						moveToggle.isOn = false;

						//make the move menu not interactable
						moveToggle.interactable = false;

					}

				}
				//the else condition is that we have selected someone else's unit
				else {

					//if the toggle is currently on, turn it off
					if (moveToggle.isOn == true) {

						moveToggle.isOn = false;

					}

					//make the move menu not interactable
					moveToggle.interactable = false;

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

		if (moveToggle != null) {
			
			//remove an onValueChanged event listener 
			moveToggle.onValueChanged.RemoveListener (boolClickMoveToggleAction);

		}

		if (mouseManager != null) {
			
			//remove a listener to the selected unit event
			mouseManager.OnSetSelectedUnit.RemoveListener (SetMoveToggle);
			mouseManager.OnClearSelectedUnit.RemoveListener (SetMoveToggle);

		}

		//remove a listener for the repair crew event
		EngineSection.OnEngineSectionRepaired.RemoveListener (combatUnitSetMoveToggleAction);

		if (gameManager != null) {
			
			//remove listener for the action mode changing
			gameManager.OnActionModeChange.RemoveListener (actionModeSetMoveToggleAction);

		}

	}
		
}
