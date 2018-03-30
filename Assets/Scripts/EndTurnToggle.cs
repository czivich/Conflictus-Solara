using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class EndTurnToggle : MonoBehaviour {

	//variable for the UI button
	public Toggle endTurnToggle;

	//variable for the game manager
	private GameManager gameManager;

	//set variable to hold the end turn toggle text
	private TextMeshProUGUI endTurnToggleText;

	//unityActions
	private UnityAction<Player> playerUpdateEndTurnToggleTextAction;
	private UnityAction<GameManager.ActionMode> actionModeSetEndTurnToggleAction;


	// Use this for initialization
	public void Init () {

		//set the actions
		playerUpdateEndTurnToggleTextAction = (player) => {UpdateEndTurnToggleText();};
		actionModeSetEndTurnToggleAction = (actionMode) => {SetEndTurnToggle ();};

		//get the manager
		gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

		//cache the end turn button text
		endTurnToggleText = endTurnToggle.GetComponentInChildren<TextMeshProUGUI>();

		//add a listener for the OnBeginMainPhase and OnNewTurn events so that we can update the toggle text
		gameManager.OnNewTurn.AddListener(playerUpdateEndTurnToggleTextAction);
		gameManager.OnBeginMainPhase.AddListener(playerUpdateEndTurnToggleTextAction);


		//add listener for the action mode changing
		gameManager.OnActionModeChange.AddListener(actionModeSetEndTurnToggleAction);

		//update the text to start
		UpdateEndTurnToggleText();

	}


	//this function will set the end turn button text based on the current game turn state
	private void UpdateEndTurnToggleText(){

		if (endTurnToggleText == null) {

			//cache the end turn button text
			endTurnToggleText = endTurnToggle.GetComponentInChildren<TextMeshProUGUI> ();

		}

		//update the end turn button text
		if (gameManager.currentTurnPhase == GameManager.TurnPhase.MainPhase) {

			endTurnToggleText.text = ("End Turn");

			//update the font size if necessary
			UIManager.AutoSizeTextMeshFont(endTurnToggleText);


		} else if (gameManager.currentTurnPhase == GameManager.TurnPhase.PurchasePhase) {

			endTurnToggleText.text = ("End Purchase Phase");

			//update the font size if necessary
			UIManager.AutoSizeTextMeshFont(endTurnToggleText);

		}

	}

	//this function will set the interactability of the endTurnToggle
	private void SetEndTurnToggle(){
		
		//check if we are in EndTurn mode
		if (gameManager.CurrentActionMode != GameManager.ActionMode.EndTurn) {

			//if we are not in endTurn mode, turn off the end turn toggle
			endTurnToggle.isOn = false;

		}
		//the else condition is that we are in end turn mode
		else {

			//if we are in end turn mode, turn on the end turn toggle
			endTurnToggle.isOn = true;
		}

		//the end turn toggle should not be interactable in certain gameManager actionModes
		if (UIManager.lockMenuActionModes.Contains(gameManager.CurrentActionMode)) {

			endTurnToggle.interactable = false;

		}
		//the else condition is that we are in a mode where the toggle is allowed to be interactable
		else {

			endTurnToggle.interactable = true;
			//Debug.Log ("endturn toggle interactable true");

		}

	}

	//function to handle on Destroy
	private void OnDestroy(){

		RemoveAllListeners ();

	}

	//function to remove listeners
	private void RemoveAllListeners(){

		if (gameManager != null) {
			
			//remove a listener for the OnBeginMainPhase and OnNewTurn events so that we can update the toggle text
			gameManager.OnNewTurn.RemoveListener (playerUpdateEndTurnToggleTextAction);
			gameManager.OnBeginMainPhase.RemoveListener (playerUpdateEndTurnToggleTextAction);

			//remove listener for the action mode changing
			gameManager.OnActionModeChange.RemoveListener (actionModeSetEndTurnToggleAction);

		}

	}

}
