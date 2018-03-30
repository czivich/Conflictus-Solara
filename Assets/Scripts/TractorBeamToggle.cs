using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class TractorBeamToggle : MonoBehaviour {

	//the tractor beam toggle
	public Toggle tractorBeamToggle;

	//variables for the managers
	private MouseManager mouseManager;
	private GameManager gameManager;

	//this event is for turning on the tractor beam toggle
	public ToggleEvent OnTurnedOnTractorBeamToggle = new ToggleEvent();

	//this event is for turning off the tractor beam toggle
	public UnityEvent OnTurnedOffTractorBeamToggleWhileEngaged = new UnityEvent();
	public UnityEvent OnTurnedOffTractorBeamToggleWhileNotEngaged = new UnityEvent();

	//simple class derived from unityEvent to pass Toggle Object
	public class ToggleEvent : UnityEvent<Toggle>{};

	//unityActions
	private UnityAction<bool> boolClickTractorBeamToggleAction;
	private UnityAction<CombatUnit> combatUnitSetTractorBeamToggleAction;
	private UnityAction<GameManager.ActionMode> actionModeUnitSetTractorBeamToggleAction;


	// Use this for initialization
	public void Init () {

		//set the actions
		boolClickTractorBeamToggleAction = (value) => {ClickTractorBeamToggle();};
		combatUnitSetTractorBeamToggleAction = (targetedUnit) => {SetTractorBeamToggle();};
		actionModeUnitSetTractorBeamToggleAction = (actionMode) => {SetTractorBeamToggle();};

		//get the managers
		gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
		mouseManager = GameObject.FindGameObjectWithTag("MouseManager").GetComponent<MouseManager>();

		//add an onValueChanged event listener 
		tractorBeamToggle.onValueChanged.AddListener(boolClickTractorBeamToggleAction);

		//add a listener to the selected unit event
		mouseManager.OnSetSelectedUnit.AddListener(SetTractorBeamToggle);
		mouseManager.OnClearSelectedUnit.AddListener(SetTractorBeamToggle);

		//add a listener to the repair section event
		PhasorSection.OnPhasorSectionRepaired.AddListener(combatUnitSetTractorBeamToggleAction);
		StarbasePhasorSection1.OnPhasorSection1Repaired.AddListener(combatUnitSetTractorBeamToggleAction);
		StarbasePhasorSection2.OnPhasorSection2Repaired.AddListener(combatUnitSetTractorBeamToggleAction);

		//add listener for the action mode changing
		gameManager.OnActionModeChange.AddListener(actionModeUnitSetTractorBeamToggleAction);

		//set the toggle at startup
		SetTractorBeamToggle();

	}
	
	private void ClickTractorBeamToggle(){

		//check if we turned the toggle on
		if (tractorBeamToggle.isOn == true) {

			//Debug.Log ("We turned on the toggle");

			//invoke the OnTurnedOnTractorBeamToggle event, passing the tractorBeamToggle
			OnTurnedOnTractorBeamToggle.Invoke(tractorBeamToggle);

		}

		else if (tractorBeamToggle.isOn == false) {

			//Debug.Log ("We turned the toggle off");

			//invoke the OnTurnedOffTractorBeamToggle event
			//check if we have the tractor beam primed or not
			if (mouseManager.selectedUnit != null) {

				//check if the selected unit is a ship
				if (mouseManager.selectedUnit.GetComponent<Ship> () == true) {

					//check if the tractor beam is primed
					if (mouseManager.selectedUnit.GetComponent<PhasorSection> ().tractorBeamIsPrimed == true) {

						//invoke the event
						OnTurnedOffTractorBeamToggleWhileEngaged.Invoke ();

					} else {

						//invoke the event
						OnTurnedOffTractorBeamToggleWhileNotEngaged.Invoke ();

					}

				}
				//the else condition is that we have a starbase
				else if (mouseManager.selectedUnit.GetComponent<Starbase> () == true) {

					//invoke the event
					OnTurnedOffTractorBeamToggleWhileNotEngaged.Invoke ();

				}

			}

			//the else condition is that the selectedUnit is null.  We stil want to send the turn off event
			else {

				//invoke the event
				OnTurnedOffTractorBeamToggleWhileNotEngaged.Invoke ();

			}

		}

	}

	//this function will set the state of the tractor beam toggle
	private void SetTractorBeamToggle(){
		
		//check if we are in tractor beam mode
		if (gameManager.CurrentActionMode != GameManager.ActionMode.TractorBeam) {

			//if we are not in tractor beam  mode, turn off the tractor beam toggle
			tractorBeamToggle.isOn = false;

		}
		//the else condition is that we are in tractor beam mode
		else {

			//if we are in tractor beam mode, turn on the tractor beam toggle
			tractorBeamToggle.interactable = true;
			tractorBeamToggle.isOn = true;
		}

		//the tractor beam toggle should not be interactable in certain gameManager actionModes
		if (UIManager.lockMenuActionModes.Contains(gameManager.CurrentActionMode)|| gameManager.currentTurnPhase != GameManager.TurnPhase.MainPhase) {

			tractorBeamToggle.interactable = false;
			tractorBeamToggle.isOn = false;

		}
		//the else condition is that we are in a mode where the toggle is allowed to be interactable
		else {
			
			//if the selectedUnit is null (has been cleared)
			if (mouseManager.selectedUnit == null) {

				//enable the tractor beam toggle if we are in selection mode
				tractorBeamToggle.interactable = true;

			}

		//the else condition is that we do have a selected unit
		else if (mouseManager.selectedUnit != null) {

				//check if the selected unit is owned by the current player
				if (mouseManager.selectedUnit.GetComponent<CombatUnit> ().owner == gameManager.currentTurnPlayer) {

					//check if the selected unit is a ship
					if (mouseManager.selectedUnit.GetComponent<Ship> () == true) {

						//check if the ship has a phasor section
						if (mouseManager.selectedUnit.GetComponentInChildren<PhasorSection> () == true) {

							//check if the section is not destroyed
							if (mouseManager.selectedUnit.GetComponentInChildren<PhasorSection> ().isDestroyed == false) {

								//if the section is not destroyed, check if the ship has a tractor beam
								if (mouseManager.selectedUnit.GetComponentInChildren<PhasorSection> ().tractorBeam == true) {

									//we have a tractor beam, so we can allow the toggle to be interacted with
									tractorBeamToggle.interactable = true;

								}

								//the else condition is we don't have a tractor beam
								else {

									//if the toggle is currently on, turn it off
									if (tractorBeamToggle.isOn == true) {

										tractorBeamToggle.isOn = false;

									}

									//make the tractor beam menu not interactable
									tractorBeamToggle.interactable = false;

								}

							}

							//the else condition is the section is destroyed
							else {

								//if the toggle is currently on, turn it off
								if (tractorBeamToggle.isOn == true) {

									tractorBeamToggle.isOn = false;

								}

								//make the tractor beam menu not interactable
								tractorBeamToggle.interactable = false;

							}

						}

						//the else condition is that we don't have a phasor section
						else {

							//if the toggle is currently on, turn it off
							if (tractorBeamToggle.isOn == true) {

								tractorBeamToggle.isOn = false;

							}

							//make the tractor beam menu not interactable
							tractorBeamToggle.interactable = false;

						}

					}

					//the else condition is that the selected unit is not a ship
					else if (mouseManager.selectedUnit.GetComponent<Starbase> () == true){

						//if the toggle is currently on, turn it off
						tractorBeamToggle.isOn = false;

						//make the tractor beam menu not interactable
						tractorBeamToggle.interactable = false;

					}

				}

				//the else condition is that we selected someone else's ship
				else {

					//if the toggle is currently on, turn it off
					tractorBeamToggle.isOn = false;

					//make the tractor beam menu not interactable
					tractorBeamToggle.interactable = false;

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

		if (tractorBeamToggle != null) {
			
			//remove an onValueChanged event listener 
			tractorBeamToggle.onValueChanged.RemoveListener (boolClickTractorBeamToggleAction);

		}

		if (mouseManager != null) {

			//remove a listener to the selected unit event
			mouseManager.OnSetSelectedUnit.RemoveListener (SetTractorBeamToggle);
			mouseManager.OnClearSelectedUnit.RemoveListener (SetTractorBeamToggle);

		}

		//remove a listener to the repair section event
		PhasorSection.OnPhasorSectionRepaired.RemoveListener(combatUnitSetTractorBeamToggleAction);
		StarbasePhasorSection1.OnPhasorSection1Repaired.RemoveListener(combatUnitSetTractorBeamToggleAction);
		StarbasePhasorSection2.OnPhasorSection2Repaired.RemoveListener(combatUnitSetTractorBeamToggleAction);

		if (gameManager != null) {
			
			//remove listener for the action mode changing
			gameManager.OnActionModeChange.RemoveListener (actionModeUnitSetTractorBeamToggleAction);

		}

	}

}
