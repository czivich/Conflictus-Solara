using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PhaserToggle : MonoBehaviour {

	//the tractor beam toggle
	public Toggle phaserToggle;

	//variables for the managers
	private MouseManager mouseManager;
	private GameManager gameManager;

	//this event is for turning on and off the tractor beam toggle
	public ToggleEvent OnTurnedOnPhaserToggle = new ToggleEvent();
	public UnityEvent OnTurnedOffPhaserToggle = new UnityEvent();

	//simple class derived from unityEvent to pass Toggle Object
	public class ToggleEvent : UnityEvent<Toggle>{};

	//unityActions
	private UnityAction<bool> boolClickPhaserToggleAction;
	private UnityAction<CombatUnit> combatUnitSetPhaserToggleAction;
	private UnityAction<GameManager.ActionMode> actionModeSetPhaserToggleAction;


	// Use this for initialization
	public void Init () {

		//get the managers
		gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
		mouseManager = GameObject.FindGameObjectWithTag("MouseManager").GetComponent<MouseManager>();

		//set the actions
		boolClickPhaserToggleAction = (value) => {ClickPhaserToggle();};
		combatUnitSetPhaserToggleAction = (targetedUnit) => {SetPhaserToggle();};
		actionModeSetPhaserToggleAction = (actionMode) => {SetPhaserToggle();};

		//add an onValueChanged event listener 
		phaserToggle.onValueChanged.AddListener(boolClickPhaserToggleAction);

		//add a listener to the selected unit event
		mouseManager.OnSetSelectedUnit.AddListener(SetPhaserToggle);
		mouseManager.OnClearSelectedUnit.AddListener(SetPhaserToggle);

		//add a listener for the repair section event
		PhaserSection.OnPhaserSectionRepaired.AddListener(combatUnitSetPhaserToggleAction);
		StarbasePhaserSection1.OnPhaserSection1Repaired.AddListener(combatUnitSetPhaserToggleAction);
		StarbasePhaserSection2.OnPhaserSection2Repaired.AddListener(combatUnitSetPhaserToggleAction);


		//add listener for the action mode changing
		gameManager.OnActionModeChange.AddListener(actionModeSetPhaserToggleAction);

		//set toggle at startup
		SetPhaserToggle();

	}

	private void ClickPhaserToggle(){

		//check if we turned the toggle on
		if (phaserToggle.isOn == true) {

			//Debug.Log ("We turned on the toggle");

			//invoke the OnTurnedOnPhaserToggle event, passing the phaserToggle
			OnTurnedOnPhaserToggle.Invoke(phaserToggle);

		}

		else if (phaserToggle.isOn == false) {

			//Debug.Log ("We turned the toggle off");

			//invoke the OnTurnedOffPhaserToggle event
			OnTurnedOffPhaserToggle.Invoke();


		}

	}

	//this function will set the state of the phaser toggle
	private void SetPhaserToggle(){

		//check if we are in phaser attack mode
		if (gameManager.CurrentActionMode != GameManager.ActionMode.PhaserAttack) {

			//if we are not in phaser attack  mode, turn off the phaser toggle
			phaserToggle.isOn = false;

		}
		//the else condition is that we are in phaser attack mode
		else {

			//if we are in phaser attack mode, turn on the phaser attack toggle
			phaserToggle.interactable = true;
			phaserToggle.isOn = true;
		}

		//the phaser attack toggle should not be interactable in certain gameManager actionModes
		if (UIManager.lockMenuActionModes.Contains(gameManager.CurrentActionMode) || gameManager.currentTurnPhase != GameManager.TurnPhase.MainPhase) {

			phaserToggle.interactable = false;
			phaserToggle.isOn = false;

		}
		//the else condition is that we are in a mode where the toggle is allowed to be interactable
		else {

			//if the selectedUnit is null (has been cleared)
			if (mouseManager.selectedUnit == null) {

				//enable the phaser toggle if we are in selection mode
				phaserToggle.interactable = true;

			}

		//the else condition is that we do have a selected unit
		else if (mouseManager.selectedUnit != null) {

				//check if the selected unit is owned by the current player
				if (mouseManager.selectedUnit.GetComponent<CombatUnit> ().owner == gameManager.currentTurnPlayer) {

					//check if the selected unit is a ship
					if (mouseManager.selectedUnit.GetComponent<Ship> () == true) {

						//check if the ship has a phaser section
						if (mouseManager.selectedUnit.GetComponentInChildren<PhaserSection> () == true) {

							//check if the section is not destroyed
							if (mouseManager.selectedUnit.GetComponentInChildren<PhaserSection> ().isDestroyed == false) {

								phaserToggle.interactable = true;

							}

							//the else condition is the section is destroyed
							else {

								//if the toggle is currently on, turn it off
								if (phaserToggle.isOn == true) {

									phaserToggle.isOn = false;

								}

								//make the phaser menu not interactable
								phaserToggle.interactable = false;

							}

						}

						//the else condition is that we don't have a phaser section
						else {

							//if the toggle is currently on, turn it off
							if (phaserToggle.isOn == true) {

								phaserToggle.isOn = false;

							}

							//make the phaser menu not interactable
							phaserToggle.interactable = false;

						}

					}

					//the else condition is that the selected unit is not a ship
					else if(mouseManager.selectedUnit.GetComponent<Starbase> () == true){

						//check if the starbase has at least one phaser section that is not destroyed
						if (mouseManager.selectedUnit.GetComponent<StarbasePhaserSection1> ().isDestroyed == false ||
						    mouseManager.selectedUnit.GetComponent<StarbasePhaserSection2> ().isDestroyed == false) {

							//if at least 1 phaser section is not destroyed, make the toggle interactable
							phaserToggle.interactable = true;

						}
						//the else condition is that both phaser sections are destroyed
						else {

							//if both sections are destroyed, the toggle is not interactable
							phaserToggle.isOn = false;
							phaserToggle.interactable = false;

						}

					}

				}

				//the else condition is that we selected someone else's combat unit
				else {

					//if the toggle is currently on, turn it off
					if (phaserToggle.isOn == true) {

						phaserToggle.isOn = false;

					}

					//make the phaser menu not interactable
					phaserToggle.interactable = false;

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

		if (phaserToggle != null) {
			
			//remove an onValueChanged event listener 
			phaserToggle.onValueChanged.RemoveListener (boolClickPhaserToggleAction);

		}

		if (mouseManager != null) {
			
			//remove a listener to the selected unit event
			mouseManager.OnSetSelectedUnit.RemoveListener (SetPhaserToggle);
			mouseManager.OnClearSelectedUnit.RemoveListener (SetPhaserToggle);

		}

		//remove a listener for the repair section event
		PhaserSection.OnPhaserSectionRepaired.RemoveListener (combatUnitSetPhaserToggleAction);
		StarbasePhaserSection1.OnPhaserSection1Repaired.RemoveListener (combatUnitSetPhaserToggleAction);
		StarbasePhaserSection2.OnPhaserSection2Repaired.RemoveListener (combatUnitSetPhaserToggleAction);

		if (gameManager != null) {
			
			//remove listener for the action mode changing
			gameManager.OnActionModeChange.RemoveListener (actionModeSetPhaserToggleAction);

		}

	}

}
