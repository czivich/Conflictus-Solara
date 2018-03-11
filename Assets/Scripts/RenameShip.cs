using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class RenameShip : MonoBehaviour {

	//variables for the managers
	private MouseManager mouseManager;
	private GameManager gameManager;

	//variables to hold the UI elements are public so they can be hooked up in the inspector
	public RectTransform renameUnitPanel;
	public TMP_InputField renameInputField;
	public Button yesButton;
	public Button cancelButton;
	public TextMeshProUGUI placeHolderText;

	//variable to hold the action menu button that triggers the rename popup panel to appear
	public Button renameActionButton;

	//variable to hold the actionMode we were in when the rename button was pressed
	private GameManager.ActionMode previousActionMode;

	//this event will be for switching to rename mode
	public UnityEvent OnEnterRenameMode = new UnityEvent();

	//this event will fire for renaming a ship
	public RenameEvent OnRenameUnit= new RenameEvent();

	//this event will be for cancelling the rename
	public RenameCancelEvent OnRenameCancel = new RenameCancelEvent();

	//class for events to pass a combat unit and a string
	public class RenameEvent : UnityEvent<CombatUnit,string,GameManager.ActionMode>{}
	public class RenameCancelEvent : UnityEvent<GameManager.ActionMode>{}

	//unityActions
	private UnityAction<string> stringUpdatedInputFieldAction;
	private UnityAction<string> stringEndedInputEditAction;
	private UnityAction<GameManager.ActionMode> actionModeSetRenameButtonAction;

	//use this for initialization
	public void Init(){

		//get the managers
		mouseManager = GameObject.FindGameObjectWithTag("MouseManager").GetComponent<MouseManager>();
		gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

		//set a character limit for input field
		renameInputField.characterLimit = 16;

		//start with the panel inactive
		renameUnitPanel.gameObject.SetActive(false);

		//set the actions
		stringUpdatedInputFieldAction = (value) => {UpdatedInputField();};
		stringEndedInputEditAction = (inputText) => {EndedInputEdit(inputText);};
		actionModeSetRenameButtonAction = (actionMode) => {SetRenameActionButton();};

		//add listener for clicking the trigger button from the action menu
		renameActionButton.onClick.AddListener(ClickedRenameActionButton);

		//add listener for clicking the renameYes button
		yesButton.onClick.AddListener(ClickedRenameYes);

		//add listener for clicking the renameCancel button
		cancelButton.onClick.AddListener(ClickedRenameCancel);

		//add listener for the inputField value changed
		renameInputField.onValueChanged.AddListener(stringUpdatedInputFieldAction);

		//add listener for entering the rename mode
		OnEnterRenameMode.AddListener(ShowRenamePanel);

		//add a listener to the selected unit event
		mouseManager.OnSetSelectedUnit.AddListener(SetRenameActionButton);
		mouseManager.OnClearSelectedUnit.AddListener(SetRenameActionButton);

		//add listener for onEndEdit for the input field
		renameInputField.onEndEdit.AddListener(stringEndedInputEditAction);

		//add listener for actionMode changing
		gameManager.OnActionModeChange.AddListener(actionModeSetRenameButtonAction);

		//call set rename button to start
		SetRenameActionButton();

	}

	//this function will set the state of the rename action button
	private void SetRenameActionButton(){
		
		//the rename action button should not be interactable in certain gameManager actionModes
		if (UIManager.lockMenuActionModes.Contains(gameManager.CurrentActionMode)|| gameManager.currentTurnPhase != GameManager.TurnPhase.MainPhase) {

			renameActionButton.interactable = false;

		}
		//the else condition is that we are in a mode where the button is allowed to be interactable
		else {

			//check if we have a selected unit
			if (mouseManager.selectedUnit != null) {

				//check if we are the owner of the selected unit
				if (mouseManager.selectedUnit.GetComponent<CombatUnit> ().owner == gameManager.currentTurnPlayer) {
				
					//if we have a selected unit and we are the owner, we can rename the unit
					renameActionButton.interactable = true;

				}
				//the else condition is we are not the owner
				else {

					//if we are not the owner, disable the button
					renameActionButton.interactable = false;

				}

			}
			//the else condition is that we have no selected unit
			else {

				//if there is no selected unit, the button should not be clickable
				renameActionButton.interactable = false;

			}

		}

	}

	//this function will be called when we click the trigger button
	private void ClickedRenameActionButton(){

		//cache the previous actionMode
		previousActionMode = gameManager.CurrentActionMode;

		//invoke the event that changes us to rename mode
		OnEnterRenameMode.Invoke();

	}

	//this function will show the rename panel
	private void ShowRenamePanel(){

		//activate the panel
		renameUnitPanel.gameObject.SetActive(true);

		//set the border color
		SetBorderColor(gameManager.currentTurnPlayer.color);

		//set the input field text to null
		renameInputField.text = "";

		//initially, the yes button should be disabled until an input is made in the text input box
		yesButton.interactable = false;

		//set the placeholder text to the current unit name
		//check if we have a ship
		if (mouseManager.selectedUnit.GetComponent<Ship> () == true) {
			
			placeHolderText.text = (mouseManager.selectedUnit.GetComponent<Ship> ().shipName);

			//update the font size if necessary
			UIManager.AutoSizeTextMeshFont(placeHolderText);

		}
		//the else condition is that we have a starbase
		else if (mouseManager.selectedUnit.GetComponent<Starbase> () == true) {

			placeHolderText.text = (mouseManager.selectedUnit.GetComponent<Starbase> ().baseName);

			//update the font size if necessary
			UIManager.AutoSizeTextMeshFont(placeHolderText);

		}

		//activate the text input field
		renameInputField.ActivateInputField ();
		renameInputField.Select ();

	}

	//this function will handle when the renameYes button is clicked
	private void ClickedRenameYes(){

		//if the yes button is clicked, we want to change the name of the selected unit

		//invoke event to change the ship name
		OnRenameUnit.Invoke(mouseManager.selectedUnit.GetComponent<CombatUnit>(),renameInputField.text,previousActionMode);

		//close the panel
		renameUnitPanel.gameObject.SetActive(false);

	}


	//this function will handle when the renameCancel button is clicked
	private void ClickedRenameCancel(){

		//invoke the renameCancel event
		OnRenameCancel.Invoke(previousActionMode);

		//close the panel
		renameUnitPanel.gameObject.SetActive(false);

	}


	//this function will handle when inputs are made into the text input field
	private void UpdatedInputField(){

		//this function will be called whenever the input field value is changed
		//check if the current field value is blank
		if (renameInputField.text != "") {

			//if the value is not null, then we can allow the yes button to be clicked
			yesButton.interactable = true;

		}
		//the else condition is that the text input is null
		else {

			//if the value is null, then we cannot allow the yes button to be clicked
			yesButton.interactable = false;

		}

	}

	//this function will respond to ending input edit on the rename field
	private void EndedInputEdit(string inputText){

		//check if the user has hit enter with a valid rename string entered
		if (inputText != "" && (Input.GetKey (KeyCode.Return) == true || Input.GetKey (KeyCode.KeypadEnter) == true)) {

			//if so, treat that as hitting the yes button
			ClickedRenameYes();

		}

	}

	//this function sets the border color
	private void SetBorderColor(Player.Color playerColor){

		//switch case based on player
		switch (playerColor) {

		case Player.Color.Green:

			renameUnitPanel.GetComponent<Image> ().color = GameManager.greenColor;
			break;

		case Player.Color.Purple:

			renameUnitPanel.GetComponent<Image> ().color = GameManager.purpleColor;
			break;

		case Player.Color.Red:

			renameUnitPanel.GetComponent<Image> ().color = GameManager.redColor;
			break;

		case Player.Color.Blue:

			renameUnitPanel.GetComponent<Image> ().color = GameManager.blueColor;
			break;

		default:

			break;

		}

	}

	//this function handles OnDestroy
	private void OnDestroy(){

		RemoveAllListeners ();

	}

	//this function removes all listeners
	private void RemoveAllListeners(){

		if (renameActionButton != null) {
			
			//remove listener for clicking the trigger button from the action menu
			renameActionButton.onClick.RemoveListener (ClickedRenameActionButton);

		}

		if (yesButton != null) {
			
			//remove listener for clicking the renameYes button
			yesButton.onClick.RemoveListener (ClickedRenameYes);

		}

		if (cancelButton != null) {
			
			//remove listener for clicking the renameCancel button
			cancelButton.onClick.RemoveListener (ClickedRenameCancel);

		}

		if (renameInputField != null) {
			
			//remove listener for the inputField value changed
			renameInputField.onValueChanged.RemoveListener (stringUpdatedInputFieldAction);

			//remove listener for onEndEdit for the input field
			renameInputField.onEndEdit.RemoveListener (stringEndedInputEditAction);
		}
			
		//remove listener for entering the rename mode
		OnEnterRenameMode.RemoveListener (ShowRenamePanel);

		if (mouseManager != null) {
			
			//remove a listener to the selected unit event
			mouseManager.OnSetSelectedUnit.RemoveListener (SetRenameActionButton);
			mouseManager.OnClearSelectedUnit.RemoveListener (SetRenameActionButton);

		}

		if (gameManager != null) {
			
			//remove listener for actionMode changing
			gameManager.OnActionModeChange.RemoveListener (actionModeSetRenameButtonAction);

		}

	}

}
