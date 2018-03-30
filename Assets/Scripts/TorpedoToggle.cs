using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class TorpedoToggle : MonoBehaviour {

	//the trigger toggle
	public Toggle torpedoToggle;

	//variables for the managers
	private MouseManager mouseManager;
	private GameManager gameManager;

	//this event is for turning on and off the torpedo toggle
	public ToggleEvent OnTurnedOnTorpedoToggle = new ToggleEvent();
	public UnityEvent OnTurnedOffTorpedoToggle = new UnityEvent();

	//simple class derived from unityEvent to pass Toggle Object
	public class ToggleEvent : UnityEvent<Toggle>{};

	//unityActions
	private UnityAction<bool> boolClickTorpedoToggleAction;
	private UnityAction<CombatUnit> combatUnitSetTorpedoToggleAction;
	private UnityAction<GameManager.ActionMode> actionModeSetTorpedoToggleAction;


	// Use this for initialization
	public void Init () {

		//set the actions
		boolClickTorpedoToggleAction = (value) => {ClickTorpedoToggle();};
		combatUnitSetTorpedoToggleAction = (targetedUnit) => {SetTorpedoToggle();};
		actionModeSetTorpedoToggleAction = (actionMode) => {SetTorpedoToggle();};

		//get the managers
		gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
		mouseManager = GameObject.FindGameObjectWithTag("MouseManager").GetComponent<MouseManager>();

		//add an onValueChanged event listener 
		torpedoToggle.onValueChanged.AddListener(boolClickTorpedoToggleAction);

		//add a listener to the selected unit event
		mouseManager.OnSetSelectedUnit.AddListener(SetTorpedoToggle);
		mouseManager.OnClearSelectedUnit.AddListener(SetTorpedoToggle);

		//add a listener to the repair section event
		TorpedoSection.OnTorpedoSectionRepaired.AddListener(combatUnitSetTorpedoToggleAction);
		StarbaseTorpedoSection.OnTorpedoSectionRepaired.AddListener(combatUnitSetTorpedoToggleAction);

		//add listener for the action mode changing
		gameManager.OnActionModeChange.AddListener(actionModeSetTorpedoToggleAction);

		//set the toggle at startup
		SetTorpedoToggle();

	}

	private void ClickTorpedoToggle(){

		//check if we turned the toggle on
		if (torpedoToggle.isOn == true) {

			//Debug.Log ("We turned on the toggle");

			//invoke the OnTurnedOnTorpedoToggle event, passing the torpedoToggle
			OnTurnedOnTorpedoToggle.Invoke(torpedoToggle);

		}

		else if (torpedoToggle.isOn == false) {

			//Debug.Log ("We turned the toggle off");

			//invoke the OnTurnedOffTorpedoToggle event
			OnTurnedOffTorpedoToggle.Invoke();


		}

	}

	//this function will set the state of the torpedo toggle
	private void SetTorpedoToggle(){
		
		//check if we are in torpedo attack mode
		//the flareMode was added later, so that we don't switch off the toggle to resolve a flare
		if (gameManager.CurrentActionMode != GameManager.ActionMode.TorpedoAttack && gameManager.CurrentActionMode != GameManager.ActionMode.FlareMode) {

			//if we are not in torpedo attack  mode, turn off the torpedo toggle
			torpedoToggle.isOn = false;

		}
		//the else condition is that we are in torpedo attack mode
		else {

			//if we are in torpedo attack mode, turn on the torpedo attack toggle
			torpedoToggle.interactable = true;
			torpedoToggle.isOn = true;
		}

		//the torpedo attack toggle should not be interactable in certain gameManager actionModes
		if (UIManager.lockMenuActionModes.Contains(gameManager.CurrentActionMode)|| gameManager.currentTurnPhase != GameManager.TurnPhase.MainPhase) {

			torpedoToggle.interactable = false;
			torpedoToggle.isOn = false;

		}
		//the else condition is that we are in a mode where the toggle is allowed to be interactable
		else {

			//if the selectedUnit is null (has been cleared)
			if (mouseManager.selectedUnit == null) {

				//enable the phasor toggle if we are in selection mode
				torpedoToggle.interactable = true;

			}

			//the else condition is that we do have a selected unit
			else if (mouseManager.selectedUnit != null) {

				//check if the selected unit is owned by the current player
				if (mouseManager.selectedUnit.GetComponent<CombatUnit> ().owner == gameManager.currentTurnPlayer) {

					//check if the selected unit is a ship
					if (mouseManager.selectedUnit.GetComponent<Ship> () == true) {

						//check if the ship has a torpedo section
						if (mouseManager.selectedUnit.GetComponentInChildren<TorpedoSection> () == true) {

							//check if the section is not destroyed
							if (mouseManager.selectedUnit.GetComponentInChildren<TorpedoSection> ().isDestroyed == false) {

								torpedoToggle.interactable = true;

							}

							//the else condition is the section is destroyed
							else {

								//if the toggle is currently on, turn it off
								if (torpedoToggle.isOn == true) {

									torpedoToggle.isOn = false;

								}

								//make the torpedo menu not interactable
								torpedoToggle.interactable = false;

							}

						}

						//the else condition is that we don't have a torpedo section
						else {

							//if the toggle is currently on, turn it off
							if (torpedoToggle.isOn == true) {

								torpedoToggle.isOn = false;

							}

							//make the torpedo menu not interactable
							torpedoToggle.interactable = false;

						}

					}

					//the else condition is that the selected unit is not a ship
					else  if(mouseManager.selectedUnit.GetComponent<Starbase> () == true){

						//check if the starbase torpedo section is not destroyed
						if (mouseManager.selectedUnit.GetComponent<StarbaseTorpedoSection> ().isDestroyed == false) {

							//if the torpedo section is not destroyed, the toggle can be interactable
							torpedoToggle.interactable = true;

						}
						//the else condition is that the torpedo section is destroyed
						else {

							torpedoToggle.isOn = false;
							torpedoToggle.interactable = false;

						}

					}

				}

				//the else condition is that we selected someone else's combat unit
				else {

					//if the toggle is currently on, turn it off
					if (torpedoToggle.isOn == true) {

						torpedoToggle.isOn = false;

					}

					//make the torpedo menu not interactable
					torpedoToggle.interactable = false;

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

		if (torpedoToggle != null) {
			
			//remove an onValueChanged event listener 
			torpedoToggle.onValueChanged.RemoveListener (boolClickTorpedoToggleAction);

		}

		if (mouseManager != null) {
			
			//remove a listener to the selected unit event
			mouseManager.OnSetSelectedUnit.RemoveListener (SetTorpedoToggle);
			mouseManager.OnClearSelectedUnit.RemoveListener (SetTorpedoToggle);

		}

		//remove a listener to the repair section event
		TorpedoSection.OnTorpedoSectionRepaired.RemoveListener (combatUnitSetTorpedoToggleAction);
		StarbaseTorpedoSection.OnTorpedoSectionRepaired.RemoveListener (combatUnitSetTorpedoToggleAction);

		if (gameManager != null) {
			
			//remove listener for the action mode changing
			gameManager.OnActionModeChange.RemoveListener (actionModeSetTorpedoToggleAction);

		}

	}

}
