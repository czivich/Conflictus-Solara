using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PhasorToggle : MonoBehaviour {

	//the tractor beam toggle
	public Toggle phasorToggle;

	//variables for the managers
	private MouseManager mouseManager;
	private GameManager gameManager;

	//this event is for turning on and off the tractor beam toggle
	public ToggleEvent OnTurnedOnPhasorToggle = new ToggleEvent();
	public UnityEvent OnTurnedOffPhasorToggle = new UnityEvent();

	//simple class derived from unityEvent to pass Toggle Object
	public class ToggleEvent : UnityEvent<Toggle>{};

	//unityActions
	private UnityAction<bool> boolClickPhasorToggleAction;
	private UnityAction<CombatUnit> combatUnitSetPhasorToggleAction;
	private UnityAction<GameManager.ActionMode> actionModeSetPhasorToggleAction;


	// Use this for initialization
	public void Init () {

		//get the managers
		gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
		mouseManager = GameObject.FindGameObjectWithTag("MouseManager").GetComponent<MouseManager>();

		//set the actions
		boolClickPhasorToggleAction = (value) => {ClickPhasorToggle();};
		combatUnitSetPhasorToggleAction = (targetedUnit) => {SetPhasorToggle();};
		actionModeSetPhasorToggleAction = (actionMode) => {SetPhasorToggle();};

		//add an onValueChanged event listener 
		phasorToggle.onValueChanged.AddListener(boolClickPhasorToggleAction);

		//add a listener to the selected unit event
		mouseManager.OnSetSelectedUnit.AddListener(SetPhasorToggle);
		mouseManager.OnClearSelectedUnit.AddListener(SetPhasorToggle);

		//add a listener for the repair section event
		PhasorSection.OnPhasorSectionRepaired.AddListener(combatUnitSetPhasorToggleAction);
		StarbasePhasorSection1.OnPhasorSection1Repaired.AddListener(combatUnitSetPhasorToggleAction);
		StarbasePhasorSection2.OnPhasorSection2Repaired.AddListener(combatUnitSetPhasorToggleAction);


		//add listener for the action mode changing
		gameManager.OnActionModeChange.AddListener(actionModeSetPhasorToggleAction);

		//set toggle at startup
		SetPhasorToggle();

	}

	private void ClickPhasorToggle(){

		//check if we turned the toggle on
		if (phasorToggle.isOn == true) {

			//Debug.Log ("We turned on the toggle");

			//invoke the OnTurnedOnPhasorToggle event, passing the phasorToggle
			OnTurnedOnPhasorToggle.Invoke(phasorToggle);

		}

		else if (phasorToggle.isOn == false) {

			//Debug.Log ("We turned the toggle off");

			//invoke the OnTurnedOffPhasorToggle event
			OnTurnedOffPhasorToggle.Invoke();


		}

	}

	//this function will set the state of the phasor toggle
	private void SetPhasorToggle(){

		//check if we are in phasor attack mode
		if (gameManager.CurrentActionMode != GameManager.ActionMode.PhasorAttack) {

			//if we are not in phasor attack  mode, turn off the phasor toggle
			phasorToggle.isOn = false;

		}
		//the else condition is that we are in phasor attack mode
		else {

			//if we are in phasor attack mode, turn on the phasor attack toggle
			phasorToggle.isOn = true;
		}

		//the phasor attack toggle should not be interactable in certain gameManager actionModes
		if (UIManager.lockMenuActionModes.Contains(gameManager.CurrentActionMode) || gameManager.currentTurnPhase != GameManager.TurnPhase.MainPhase) {

			phasorToggle.interactable = false;
			phasorToggle.isOn = false;

		}
		//the else condition is that we are in a mode where the toggle is allowed to be interactable
		else {

			//if the selectedUnit is null (has been cleared)
			if (mouseManager.selectedUnit == null) {

				//enable the phasor toggle if we are in selection mode
				phasorToggle.interactable = true;

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

								phasorToggle.interactable = true;

							}

							//the else condition is the section is destroyed
							else {

								//if the toggle is currently on, turn it off
								if (phasorToggle.isOn == true) {

									phasorToggle.isOn = false;

								}

								//make the phasor menu not interactable
								phasorToggle.interactable = false;

							}

						}

						//the else condition is that we don't have a phasor section
						else {

							//if the toggle is currently on, turn it off
							if (phasorToggle.isOn == true) {

								phasorToggle.isOn = false;

							}

							//make the phasor menu not interactable
							phasorToggle.interactable = false;

						}

					}

					//the else condition is that the selected unit is not a ship
					else if(mouseManager.selectedUnit.GetComponent<Starbase> () == true){

						//check if the starbase has at least one phasor section that is not destroyed
						if (mouseManager.selectedUnit.GetComponent<StarbasePhasorSection1> ().isDestroyed == false ||
						    mouseManager.selectedUnit.GetComponent<StarbasePhasorSection2> ().isDestroyed == false) {

							//if at least 1 phasor section is not destroyed, make the toggle interactable
							phasorToggle.interactable = true;

						}
						//the else condition is that both phasor sections are destroyed
						else {

							//if both sections are destroyed, the toggle is not interactable
							phasorToggle.isOn = false;
							phasorToggle.interactable = false;

						}

					}

				}

				//the else condition is that we selected someone else's combat unit
				else {

					//if the toggle is currently on, turn it off
					if (phasorToggle.isOn == true) {

						phasorToggle.isOn = false;

					}

					//make the phasor menu not interactable
					phasorToggle.interactable = false;

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

		if (phasorToggle != null) {
			
			//remove an onValueChanged event listener 
			phasorToggle.onValueChanged.RemoveListener (boolClickPhasorToggleAction);

		}

		if (mouseManager != null) {
			
			//remove a listener to the selected unit event
			mouseManager.OnSetSelectedUnit.RemoveListener (SetPhasorToggle);
			mouseManager.OnClearSelectedUnit.RemoveListener (SetPhasorToggle);

		}

		//remove a listener for the repair section event
		PhasorSection.OnPhasorSectionRepaired.RemoveListener (combatUnitSetPhasorToggleAction);
		StarbasePhasorSection1.OnPhasorSection1Repaired.RemoveListener (combatUnitSetPhasorToggleAction);
		StarbasePhasorSection2.OnPhasorSection2Repaired.RemoveListener (combatUnitSetPhasorToggleAction);

		if (gameManager != null) {
			
			//remove listener for the action mode changing
			gameManager.OnActionModeChange.RemoveListener (actionModeSetPhasorToggleAction);

		}

	}

}
