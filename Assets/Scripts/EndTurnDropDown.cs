using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class EndTurnDropDown : MonoBehaviour {

	//variables to hold the managers
	private GameManager gameManager;

	//these UI elements are public so they can be hooked up in the inspector
	//the end turn button is what we want to trigger the opening of our submenu
	public RectTransform endTurnToggleRect;

	//we need the action menu panel because we want the drop down menu panel size to be based on it
	public RectTransform actionMenuPanel;

	//this is the panel for our drop down menu
	public RectTransform endTurnDropDownPanel;

	//this is the vertical layout group used in the action menu - I will need to grab it's spacing and padding
	public VerticalLayoutGroup actionMenuVerticalLayoutGroup;

	//these are the rows we want to have in our drop down panel
	public LayoutElement endTurnDropDownPanelButtonRow;
	public LayoutElement endTurnDropDownPanelTextRow;

	//variable to hold the button for saying yes on the dropdown
	public Button endTurnDropDownPanelYesButton;

	//variable to hold the button for saying cancel on the dropdown
	public Button endTurnDropDownPanelCancelButton;

	//variable to hold the text prompt in the dropdown menu
	public TextMeshProUGUI endTurnDropDownPanelText;

	//variable to hold the spacing between the main action menu and the dropdown
	private float panelSpacing = 10.0f;

	//event to announce we're in the end turn drop down menu mode
	public ToggleEvent OnEnterEndTurnPrompt = new ToggleEvent();

	//event to announce we've cancelled out of the end turn prompt
	public UnityEvent OnCancelEndTurnPrompt = new UnityEvent();

	//event to announce we've clicked the end turn button
	public UnityEvent OnAcceptEndTurnPrompt = new UnityEvent();

	//simple class derived from unityEvent to pass Toggle Object
	public class ToggleEvent : UnityEvent<Toggle>{};

	//unityActions
	private UnityAction<bool> boolShowDropDownPanelAction;


	// Use this for initialization
	public void Init () {

		//set actions
		boolShowDropDownPanelAction = (value) => {ShowDropDownPanel();};

		//get the managers
		gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

		//set the panel size
		SetPanelSize ();

		//add an on-click event listener for the main menu end turn button
		endTurnToggleRect.GetComponent<Toggle>().onValueChanged.AddListener(boolShowDropDownPanelAction);

		//add listeners for the yes and cancel buttons on our dropdown
		endTurnDropDownPanelYesButton.onClick.AddListener(ClickedDropdownYes);
		endTurnDropDownPanelCancelButton.onClick.AddListener(ClickedDropdownCancel);

		//start with the panel inactive
		endTurnDropDownPanel.gameObject.SetActive(false);

	}

	//this function will expand and show the dropdown panel
	private void ShowDropDownPanel(){

		//the listener will trigger this every time the value of the toggle changes
		//in reality, all I want is for it to trigger when we are turning it on
		//so first, I will check to see if it is on
		//if it is, execute code, otherwise, return

		if (endTurnToggleRect.GetComponentInParent<Toggle> ().isOn == true) {

			//set the panel size
			SetPanelSize ();

			//activate the panel
			endTurnDropDownPanel.gameObject.SetActive(true);

			//update the dropdown panel text
			UpdateDropDownPanelText ();

			//invoke the OnEnterEndTurnPrompt event
			OnEnterEndTurnPrompt.Invoke(endTurnToggleRect.GetComponentInParent<Toggle>());
		
		}

	}

	//this function will collapse and hide the dropdown panel
	private void CollapseDropDownPanel(){

		//set the panel to inactive
		endTurnDropDownPanel.gameObject.SetActive(false);

	}

	//this function runs when we click the yes option on the dropdown
	private void ClickedDropdownYes(){

		//collapse the dropdown panel
		CollapseDropDownPanel();

		//turn off the end turn button
		endTurnToggleRect.GetComponentInParent<Toggle>().isOn = false;

		//invoke the OnAcceptEndTurnPrompt event
		OnAcceptEndTurnPrompt.Invoke();

	}

	//this function runs when we click the cancel option on the dropdown
	private void ClickedDropdownCancel(){

		//collapse the dropdown panel
		CollapseDropDownPanel();

		//turn off the end turn button
		endTurnToggleRect.GetComponentInParent<Toggle>().isOn = false;

		//invoke the OnCancelEndTurnPrompt event
		OnCancelEndTurnPrompt.Invoke();

	}

	//this function updates the text on the dropdown panel
	private void UpdateDropDownPanelText(){

		//update the end turn button text
		if (gameManager.currentTurnPhase == GameManager.TurnPhase.MainPhase) {

			endTurnDropDownPanelText.text = ("End Turn?");

			//update the font size if necessary
			UIManager.AutoSizeTextMeshFont(endTurnDropDownPanelText);


		} else if (gameManager.currentTurnPhase == GameManager.TurnPhase.PurchasePhase) {

			endTurnDropDownPanelText.text = ("End Purchase Phase?");

			//update the font size if necessary
			UIManager.AutoSizeTextMeshFont(endTurnDropDownPanelText);

		}

	}

	//this function will set the size of the dropdown based on the screen size
	private void SetPanelSize(){

		//to start, update the canvas so things size appropriately
		Canvas.ForceUpdateCanvases ();

		//set the drop down panel to start 20 pixels to the right of the action menu panel
		endTurnDropDownPanel.anchoredPosition3D = new Vector3 (actionMenuPanel.rect.width + panelSpacing, 0.0f, 0.0f);

		//set the size of the drop down
		//width is same as action menu panel
		//height is bottom padding + top padding + spacing + 2X button height
		endTurnDropDownPanel.sizeDelta = new Vector2 (actionMenuPanel.rect.width - panelSpacing, (actionMenuVerticalLayoutGroup.padding.bottom) 
			+ (2.0f * endTurnToggleRect.rect.height) + (1.0f * actionMenuVerticalLayoutGroup.spacing) + (actionMenuVerticalLayoutGroup.padding.top));

		//set preferred height of the dropdown rows to match the main action menu panel buttons
		endTurnDropDownPanelButtonRow.preferredHeight = endTurnToggleRect.rect.height;

		endTurnDropDownPanelTextRow.preferredHeight = endTurnToggleRect.rect.height;

		Canvas.ForceUpdateCanvases ();

	}

	//function to handle on destroy
	private void OnDestroy(){

		RemoveAllListeners ();

	}

	//function to remove listeners
	private void RemoveAllListeners(){

		//remove an on-click event listener for the main menu end turn button
		if (endTurnToggleRect != null) {
			
			endTurnToggleRect.GetComponent<Toggle> ().onValueChanged.RemoveListener (boolShowDropDownPanelAction);

		}

		//remove listeners for the yes and cancel buttons on our dropdown
		if (endTurnDropDownPanelYesButton != null) {
			
			endTurnDropDownPanelYesButton.onClick.RemoveListener (ClickedDropdownYes);

		}

		if (endTurnDropDownPanelCancelButton != null) {
			
			endTurnDropDownPanelCancelButton.onClick.RemoveListener (ClickedDropdownCancel);

		}

	}
		
}
