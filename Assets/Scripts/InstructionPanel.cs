using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class InstructionPanel : MonoBehaviour {

	//managers
	private GameManager gameManager;
	private UIManager uiManager;
	private MouseManager mouseManager;

	//object to hold the panel
	public GameObject instructionPanel;

	//buttons
	public Button backButton;
	public Button cancelButton;

	//unityEvents
	public UnityEvent OnReturnToOutfitShip = new UnityEvent();
	public UnityEvent OnCancelPlaceUnit = new UnityEvent();

	//unityActions
	UnityAction<Dictionary<string,int>,int,CombatUnit.UnitType> purchaseSetInstructionsForPlaceNewUnitAction;
	UnityAction<Hex> placeUnitCloseInstructionPanelAction;
	UnityAction returnFromNameNewShipAction;


	// Use this for initialization
	public void Init () {

		//get the manager
		gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
		uiManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
		mouseManager = GameObject.FindGameObjectWithTag("MouseManager").GetComponent<MouseManager>();

		//set the actions
		purchaseSetInstructionsForPlaceNewUnitAction = (purchasedItems,purchaseCost,unitType) => {SetInstructionsForPlaceNewUnit ();};
		placeUnitCloseInstructionPanelAction = (localHex) => {CloseInstructionPanel();};
		returnFromNameNewShipAction = () => {SetInstructionsForPlaceNewUnit ();};

		//add listener for placing new unit
		uiManager.GetComponent<PurchaseManager>().OnOutfittedShip.AddListener (purchaseSetInstructionsForPlaceNewUnitAction);

		//add listener for actually placing the new unit
		mouseManager.OnPlacedNewUnit.AddListener(placeUnitCloseInstructionPanelAction);

		//add listener for returning to placement from name new unit
		uiManager.GetComponent<NameNewShip>().OnReturnToPlaceUnit.AddListener(returnFromNameNewShipAction);

		//add listener for back button
		backButton.onClick.AddListener(ResolveBackButtonClick);

		//add listener for cancel button
		cancelButton.onClick.AddListener(ResolveCancelButtonClick);

		//start with the panel closed
		CloseInstructionPanel();

	}
	
	//this function opens the instruction panel
	private void OpenInstructionPanel(){

		instructionPanel.SetActive (true);

	}

	//this function closes the instruction panel
	private void CloseInstructionPanel(){

		instructionPanel.SetActive (false);

	}

	//this function sets the instruction panel for placing a new unit
	private void SetInstructionsForPlaceNewUnit(){

		//open the panel
		OpenInstructionPanel();

		//set the border color
		SetBorderColor (gameManager.currentTurnPlayer.color);

		//set the text string
		instructionPanel.GetComponentInChildren<TextMeshProUGUI>().text = ("Select Highlighted Tile To Place Ship");

		//update the font size if necessary
		UIManager.AutoSizeTextMeshFont(instructionPanel.GetComponentInChildren<TextMeshProUGUI>());

	}

	//this function sets the border color
	private void SetBorderColor(Player.Color targetedPlayerColor){

		//switch case based on player
		switch (targetedPlayerColor) {

		case Player.Color.Green:

			instructionPanel.GetComponent<Image> ().color = GameManager.greenColor;
			break;

		case Player.Color.Purple:

			instructionPanel.GetComponent<Image> ().color = GameManager.purpleColor;
			break;

		case Player.Color.Red:

			instructionPanel.GetComponent<Image> ().color = GameManager.redColor;
			break;

		case Player.Color.Blue:

			instructionPanel.GetComponent<Image> ().color = GameManager.blueColor;
			break;

		default:

			break;

		}

	}

	//this function resolves the cancel button press
	private void ResolveCancelButtonClick(){

		//close the panel
		CloseInstructionPanel();

		//invokve the event
		OnCancelPlaceUnit.Invoke();

	}

	//this function resolves the back button press
	private void ResolveBackButtonClick(){

		//close the panel
		CloseInstructionPanel();

		//invokve the event
		OnReturnToOutfitShip.Invoke();

	}

	//function for handling onDestroy
	private void OnDestroy(){

		RemoveAllListeners ();

	}

	//function for removing listeners
	private void RemoveAllListeners(){

		if (uiManager != null) {
			
			//remove listener for placing new unit
			uiManager.GetComponent<PurchaseManager> ().OnOutfittedShip.RemoveListener (purchaseSetInstructionsForPlaceNewUnitAction);

			//remove listener for returning to placement from name new unit
			uiManager.GetComponent<NameNewShip>().OnReturnToPlaceUnit.RemoveListener(returnFromNameNewShipAction);

		}

		if (gameManager != null) {

			//remove listener for actually placing the new unit
			mouseManager.OnPlacedNewUnit.RemoveListener (placeUnitCloseInstructionPanelAction);

		}

		if (backButton != null) {

			//remove listener for back button
			backButton.onClick.RemoveListener (ResolveBackButtonClick);

		}

		if (cancelButton != null) {

			//remove listener for cancel button
			cancelButton.onClick.RemoveListener (ResolveCancelButtonClick);

		}

	}

}
