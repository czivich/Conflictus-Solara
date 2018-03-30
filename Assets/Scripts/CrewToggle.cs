using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class CrewToggle : MonoBehaviour {

	//the menu trigger toggle
	public Toggle crewToggle;

	//variables for the managers
	private MouseManager mouseManager;
	private GameManager gameManager;

	//this event is for turning on and off the use item toggle
	public ToggleEvent OnTurnedOnCrewToggle = new ToggleEvent();
	public UnityEvent OnTurnedOffCrewToggle = new UnityEvent();

	//simple class derived from unityEvent to pass Toggle Object
	public class ToggleEvent : UnityEvent<Toggle>{};

	//unityActions
	private UnityAction<bool> boolClickCrewToggleAction;
	private UnityAction<CombatUnit> combatUnitClickCrewToggleAction;
	private UnityAction<GameManager.ActionMode> actionModeClickCrewToggleAction;


	// Use this for initialization
	public void Init () {

		//set actions
		boolClickCrewToggleAction = (value) => {ClickCrewToggle();};
		combatUnitClickCrewToggleAction = (targetedUnit) => {SetCrewToggle();};
		actionModeClickCrewToggleAction = (actionMode) => {SetCrewToggle();};

		//get the managers
		gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
		mouseManager = GameObject.FindGameObjectWithTag("MouseManager").GetComponent<MouseManager>();

		//add an onValueChanged event listener 
		crewToggle.onValueChanged.AddListener(boolClickCrewToggleAction);

		//add a listener to the selected unit event
		mouseManager.OnSetSelectedUnit.AddListener(SetCrewToggle);
		mouseManager.OnClearSelectedUnit.AddListener(SetCrewToggle);

		//add a listener to the repair section event
		CrewSection.OnCrewSectionRepaired.AddListener(combatUnitClickCrewToggleAction);
		StarbaseCrewSection.OnCrewSectionRepaired.AddListener(combatUnitClickCrewToggleAction);

		//add listener for the action mode changing
		gameManager.OnActionModeChange.AddListener(actionModeClickCrewToggleAction);

		//set the toggle at startup
		SetCrewToggle();

	}

	private void ClickCrewToggle(){

		//check if we turned the toggle on
		if (crewToggle.isOn == true) {

			//Debug.Log ("We turned on the toggle");

			//invoke the OnTurnedOnCrewToggle event, passing the toggle
			OnTurnedOnCrewToggle.Invoke(crewToggle);

		}

		else if (crewToggle.isOn == false) {

			//Debug.Log ("We turned the toggle off");

			//invoke the OnTurnedOffCrewToggle event
			OnTurnedOffCrewToggle.Invoke();


		}

	}

	//this function will set the state of the crew toggle
	private void SetCrewToggle(){
		
		//check if we are in crew mode
		if (gameManager.CurrentActionMode != GameManager.ActionMode.Crew) {

			//if we are not in crew mode, turn off the crew toggle
			crewToggle.isOn = false;

		}
		//the else condition is that we are in crew mode
		else {

			//if we are in crew mode, turn on the crew toggle
			crewToggle.interactable = true;
			crewToggle.isOn = true;
		}

		//the crew toggle should not be interactable in certain gameManager actionModes
		if (UIManager.lockMenuActionModes.Contains(gameManager.CurrentActionMode)|| gameManager.currentTurnPhase != GameManager.TurnPhase.MainPhase) {

			crewToggle.interactable = false;
			crewToggle.isOn = false;

		}
		//the else condition is that we are in a mode where the toggle is allowed to be interactable
		else {

			//if the selectedUnit is null (has been cleared)
			if (mouseManager.selectedUnit == null) {

				//enable the crew toggle if we are in selection mode
				crewToggle.interactable = true;

			}

			//the else condition is that we do have a selected unit
			else if (mouseManager.selectedUnit != null) {

				//check if the selected unit is owned by the current player
				if (mouseManager.selectedUnit.GetComponent<CombatUnit> ().owner == gameManager.currentTurnPlayer) {

					//check if the selected unit is a ship
					if (mouseManager.selectedUnit.GetComponent<Ship> () == true) {

						//check if the ship has a crew section
						if (mouseManager.selectedUnit.GetComponentInChildren<CrewSection> () == true) {

							//check if the section is not destroyed
							if (mouseManager.selectedUnit.GetComponentInChildren<CrewSection> ().isDestroyed == false) {

								crewToggle.interactable = true;

							}

							//the else condition is the section is destroyed
							else {

								//if the toggle is currently on, turn it off
								if (crewToggle.isOn == true) {

									crewToggle.isOn = false;

								}

								//make the crew menu not interactable
								crewToggle.interactable = false;

							}

						}

						//the else condition is that we don't have a crew section
						else {

							//if the toggle is currently on, turn it off
							if (crewToggle.isOn == true) {

								crewToggle.isOn = false;

							}

							//make the crew menu not interactable
							crewToggle.interactable = false;

						}

					}

					//the else condition is that the selected unit is not a ship
					else if (mouseManager.selectedUnit.GetComponent<Starbase> () == true){

						//check if the starbase has the crew section destroyed
						if (mouseManager.selectedUnit.GetComponent<StarbaseCrewSection> ().isDestroyed == false) {

							//if the crew section is not destroyed, make it interactable
							crewToggle.interactable = true;

						}
						//the else condition is that the crew section is destroyed
						else {

							//if the crew section is destroyed, the toggle is not interactable
							crewToggle.isOn = false;
							crewToggle.interactable = false;

						}

					}

				}

				//the else condition is that we selected someone else's combat unit
				else {

					//if the toggle is currently on, turn it off
					if (crewToggle.isOn == true) {

						crewToggle.isOn = false;

					}

					//make the crew menu not interactable
					crewToggle.interactable = false;

				}

			}

		}

	}

	//this function handles onDestroy
	private void OnDestroy(){

		RemoveAllListeners ();

	}

	//function to remove listeners
	private void RemoveAllListeners(){

		if (crewToggle != null) {

			//remove an onValueChanged event listener 
			crewToggle.onValueChanged.RemoveListener (boolClickCrewToggleAction);

		}

		if (mouseManager != null) {

			//remove a listener to the selected unit event
			mouseManager.OnSetSelectedUnit.RemoveListener (SetCrewToggle);
			mouseManager.OnClearSelectedUnit.RemoveListener (SetCrewToggle);

		}

		//remove a listener to the repair section event
		CrewSection.OnCrewSectionRepaired.RemoveListener (combatUnitClickCrewToggleAction);
		StarbaseCrewSection.OnCrewSectionRepaired.RemoveListener (combatUnitClickCrewToggleAction);

		if (gameManager != null) {
			
			//remove listener for the action mode changing
			gameManager.OnActionModeChange.RemoveListener (actionModeClickCrewToggleAction);

		}

	}

}
