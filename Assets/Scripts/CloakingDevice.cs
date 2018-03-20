using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CloakingDevice : MonoBehaviour {

	//variables for the managers
	private GameManager gameManager;
	private UIManager uiManager;

	//variable to hold the cloaking status
	public bool isCloaked {

		get;
		private set;

	}

	//variable to track whether a cloaking action was taken this turn already
	public bool usedCloakingDeviceThisTurn {

		get;
		private set;

	}

	//events for engaging and disengaging the cloaking device
	public static CloakingEvent OnEngageCloakingDevice = new CloakingEvent();
	public static CloakingEvent OnDisengageCloakingDevice = new CloakingEvent();

	//class to handle sending cloak events
	public class CloakingEvent : UnityEvent<CombatUnit>{};

	//unityEvent actions for event listeners
	private UnityAction<Player.Color> colorEndTurnAction;
	private UnityAction<CombatUnit> combatUnitCombatUnitDestroyedAction;
	private UnityAction<CombatUnit> combatUnitEngageCloakingDeviceAction;
	private UnityAction<CombatUnit> combatUnitDisengageCloakingDeviceAction;
	private UnityAction<CombatUnit,FileManager.SaveGameData> saveDataResolveLoadedUnitAction;

	// Use this for initialization
	public void Init () {
	
		//set the actions
		colorEndTurnAction = (currentTurn) => {EndTurn(currentTurn);};
		combatUnitCombatUnitDestroyedAction = (unitDestroyed) => {CombatUnitDestroyed (unitDestroyed);};
		combatUnitEngageCloakingDeviceAction = (combatUnit) => {EngageCloakingDevice(combatUnit);};
		combatUnitDisengageCloakingDeviceAction = (combatUnit) => {DisengageCloakingDevice (combatUnit);};
		saveDataResolveLoadedUnitAction = (combatUnit,saveGameData) => {ResolveLoadedUnit(combatUnit,saveGameData);};

		//get the managers
		gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
		uiManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();


		//initialize the cloaking status
		isCloaked = false;
		usedCloakingDeviceThisTurn = false;

		//add listener for ending turn
		gameManager.OnEndTurn.AddListener(colorEndTurnAction);

		//add listener for ship being destroyed
		Ship.OnShipDestroyed.AddListener(combatUnitCombatUnitDestroyedAction);

		//add listener for engaging cloaking device
		uiManager.GetComponent<CloakingDeviceMenu>().OnTurnOnCloakingDevice.AddListener(combatUnitEngageCloakingDeviceAction);
		uiManager.GetComponent<CloakingDeviceMenu>().OnTurnOffCloakingDevice.AddListener(combatUnitDisengageCloakingDeviceAction);

		//add listener for loading a unit
		//add listener for creating unit from load
		CombatUnit.OnCreateLoadedUnit.AddListener(saveDataResolveLoadedUnitAction);

	}

	//this function handles turning on the cloaking device
	private void EngageCloakingDevice(CombatUnit combatUnit){

		//check if the combat unit that was passed matches the combat unit this component is attached to
		if (this.GetComponent<CombatUnit> () == combatUnit) {

			if (this.usedCloakingDeviceThisTurn == false) {
			
				//update variables
				this.isCloaked = true;
				this.usedCloakingDeviceThisTurn = true;

				//invoke the OnEngage event
				OnEngageCloakingDevice.Invoke (this.GetComponent<CombatUnit> ());
			
			}
			//the else condition is that we tried to engage the device after already using it - this would be a double use and is a logic error
			else {

				Debug.LogError ("We tried to engage the cloaking device after already using the cloaking device this turn");

			}

		}

	}

	//this function handles turning off the cloaking device
	private void DisengageCloakingDevice(CombatUnit combatUnit){

		//check if the combat unit that was passed matches the combat unit this component is attached to
		if (this.GetComponent<CombatUnit> () == combatUnit) {

			if (this.usedCloakingDeviceThisTurn == false) {

				//update variables
				this.isCloaked = false;
				this.usedCloakingDeviceThisTurn = true;

				//invoke the OnDisengage event
				OnDisengageCloakingDevice.Invoke (this.GetComponent<CombatUnit> ());

			}
			//the else condition is that we tried to engage the device after already using it - this would be a double use and is a logic error
			else {

				Debug.LogError ("We tried to disengage the cloaking device after already using the cloaking device this turn");

			}

		}

	}

	//this function handles end of turn cleanup
	private void EndTurn(Player.Color currentTurn){

		//check if the color passed to the function matches the owner color
		//if so, our turn is ending, and we need to reset stuff
		if (currentTurn == this.GetComponent<Ship>().owner.color) {

			this.usedCloakingDeviceThisTurn = false;

		}

	}

	//this function resolves the loaded unit
	private void ResolveLoadedUnit(CombatUnit combatUnit, FileManager.SaveGameData saveGameData){
		
		//first, check that the unit matches
		if (this.GetComponent<CombatUnit> () == combatUnit) {

			//if we have the right unit, use the saveGameData to set values
			//need to find the index of the player colors from the saveGameData file
			for (int i = 0; i < GameManager.numberPlayers; i++) {

				//check if the color matches
				if (saveGameData.playerColor [i] == combatUnit.owner.color) {

					//we have found the right index for this player
					//we know that this has to be a bird of prey, since only a bird of prey has a cloaking device

					//set the cloaking device status
					this.isCloaked = saveGameData.birdOfPreyIsCloaked [i, combatUnit.serialNumber];
					this.usedCloakingDeviceThisTurn = saveGameData.birdOfPreyUsedCloakingDeviceThisTurn [i, combatUnit.serialNumber];

					if (this.isCloaked == true) {
						
						//invoke the event
						OnEngageCloakingDevice.Invoke (this.GetComponent<CombatUnit> ());

					}

					if (this.isCloaked == false) {

						//invoke the event
						OnDisengageCloakingDevice.Invoke (this.GetComponent<CombatUnit> ());

					}

				}

			}

		}

	}

	//this function will remove all listeners when the combat unit is destroyed
	private void CombatUnitDestroyed(CombatUnit combatUnitDestroyed){

		//check if the passed combat unit is this combat unit
		if (this.GetComponent<CombatUnit> () == combatUnitDestroyed) {

			RemoveAllListeners ();

		}

	}

	//private function to remove listeners if object is destroyed
	private void OnDestroy(){

		RemoveAllListeners ();

	}

	//this function removes all listeners
	private void RemoveAllListeners(){

		//remove listeners for ending turn
		if (gameManager != null) {
			
			gameManager.OnEndTurn.RemoveListener (colorEndTurnAction);

		}

		//remove listener for ship being destroyed
		Ship.OnShipDestroyed.RemoveListener(combatUnitCombatUnitDestroyedAction);

		//remove listener for engaging cloaking device
		if (uiManager != null) {
			
			uiManager.GetComponent<CloakingDeviceMenu> ().OnTurnOnCloakingDevice.RemoveListener (combatUnitEngageCloakingDeviceAction);
			uiManager.GetComponent<CloakingDeviceMenu> ().OnTurnOffCloakingDevice.RemoveListener (combatUnitDisengageCloakingDeviceAction);

		}

	}

}
