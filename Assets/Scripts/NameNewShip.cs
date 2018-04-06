using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class NameNewShip : MonoBehaviour {

	//managers
	private GameManager gameManager;
	private UIManager uiManager;
	private MouseManager mouseManager;

	//these variables are for storing information for newly created units
	private Dictionary<string,int> newUnitOutfittedItems = new Dictionary<string,int>();
	private int newUnitTotalPrice;
	private CombatUnit.UnitType newUnitType;
	private Hex newUnitHexLocation;
	private string newUnitName;

	//object to hold the panel
	public GameObject nameNewShipPanel;

	//TMPro for placeholder text
	public TextMeshProUGUI defaultShipName;
	public TextMeshProUGUI enteredShipName;

	//TMPro input field
	public TMP_InputField shipNameInputField;

	//objects for the buttons
	public Button purchaseButton;
	public Button cancelButton;
	public Button backButton;

	//event to cancel the purchase
	public UnityEvent OnCanceledPurchase = new UnityEvent();

	//event to return to place unit
	public UnityEvent OnReturnToPlaceUnit = new UnityEvent();

	//event to announce a new ship has been purchased
	public PurchasedNewShipEvent OnPurchasedNewShip = new PurchasedNewShipEvent();

	public class PurchasedNewShipEvent : UnityEvent<NewUnitEventData>{}; 

	//create a class for newUnitEvent so I can pass more than 4 parameters via event
	public class NewUnitEventData{

		private NameNewShip parent;

		public Dictionary<string,int> newUnitOutfittedItems {get; private set;}
		public int newUnitTotalPrice {get; private set;}
		public CombatUnit.UnitType newUnitType {get; private set;}
		public Hex newUnitHexLocation {get; private set;}
		public string newUnitName {get; private set;}

		public NewUnitEventData(NameNewShip parent){

			this.newUnitOutfittedItems = parent.newUnitOutfittedItems;
			this.newUnitTotalPrice = parent.newUnitTotalPrice;
			this.newUnitType = parent.newUnitType;
			this.newUnitHexLocation = parent.newUnitHexLocation;
			this.newUnitName = parent.newUnitName;

		}

	}

	//unityActions
	private UnityAction<Dictionary<string,int>,int,CombatUnit.UnitType> purchaseStoreNewUnitDataAction;
	private UnityAction<Hex> hexResolvePlacedNewUnitAction;
	private UnityAction<string> stringResolvedEnteredNameAction;

	// Use this for initialization
	public void Init () {

		//managers
		mouseManager = GameObject.FindGameObjectWithTag("MouseManager").GetComponent<MouseManager>();
		uiManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
		gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

		//set the actions
		purchaseStoreNewUnitDataAction = (purchasedItems,purchaseCost,unitType) => {StoreNewUnitData(purchasedItems,purchaseCost,unitType);};
		hexResolvePlacedNewUnitAction = (hex) => {ResolvePlacedNewUnit(hex);};
		stringResolvedEnteredNameAction = (inputText) => {ResolveEnteredName(inputText);};

		//add listener for outfitting a ship
		uiManager.GetComponent<PurchaseManager>().OnOutfittedShip.AddListener(purchaseStoreNewUnitDataAction);

		//add listener for choosing new ship location
		mouseManager.OnPlacedNewUnit.AddListener(hexResolvePlacedNewUnitAction);

		//add listeners to the button presses
		purchaseButton.onClick.AddListener(ResolvePurchaseButtonClick);
		cancelButton.onClick.AddListener (ResolveCancelButtonClick);
		backButton.onClick.AddListener (ResolveBackButtonClick);


		//add listener for the input submit event
		shipNameInputField.onEndEdit.AddListener(stringResolvedEnteredNameAction);

		//start with the panel closed
		CloseNameNewShipPanel();
		
	}

	//this function opens the instruction panel
	private void OpenNameNewShipPanel(){

		nameNewShipPanel.SetActive (true);

	}

	//this function closes the instruction panel
	private void CloseNameNewShipPanel(){

		nameNewShipPanel.SetActive (false);

	}

	//this function stores the information for a new unit being outfitted
	private void StoreNewUnitData(Dictionary<string,int> purchasedItems, int purchaseValue, CombatUnit.UnitType unitType){

		newUnitOutfittedItems = purchasedItems;
		newUnitTotalPrice = purchaseValue;
		newUnitType = unitType;

	}

	//this function is called when the new unit is placed
	private void ResolvePlacedNewUnit(Hex newUnitHex){

		//store the newUnitHex
		newUnitHexLocation = newUnitHex;

		//open the panel
		OpenNameNewShipPanel();

		//set the border color
		SetBorderColor(gameManager.currentTurnPlayer.color);

		//clear the entered ship name
		//enteredShipName.text = null;

		//clear any old user input name
		newUnitName = null;

		//set the text string
		//switch case based on current player
		switch (gameManager.currentTurn) {

		case Player.Color.Green:

			//switch case on unit type
			switch (newUnitType) {

			case CombatUnit.UnitType.Scout:

				defaultShipName.text = uiManager.GetComponent<DefaultUnitNames> ().greenScoutNames [gameManager.currentTurnPlayer.playerScoutPurchased];
				break;

			case CombatUnit.UnitType.BirdOfPrey:

				defaultShipName.text = uiManager.GetComponent<DefaultUnitNames> ().greenBirdOfPreyNames [gameManager.currentTurnPlayer.playerBirdOfPreyPurchased];
				break;

			case CombatUnit.UnitType.Destroyer:

				defaultShipName.text = uiManager.GetComponent<DefaultUnitNames> ().greenDestroyerNames [gameManager.currentTurnPlayer.playerDestroyerPurchased];
				break;

			case CombatUnit.UnitType.Starship:

				defaultShipName.text = uiManager.GetComponent<DefaultUnitNames> ().greenStarshipNames [gameManager.currentTurnPlayer.playerStarshipPurchased];
				break;

			default:
				
				defaultShipName.text = "DefaultShip";
				break;

			}

			break;

		case Player.Color.Purple:

			//switch case on unit type
			switch (newUnitType) {

			case CombatUnit.UnitType.Scout:

				defaultShipName.text = uiManager.GetComponent<DefaultUnitNames> ().purpleScoutNames [gameManager.currentTurnPlayer.playerScoutPurchased];
				break;

			case CombatUnit.UnitType.BirdOfPrey:

				defaultShipName.text = uiManager.GetComponent<DefaultUnitNames> ().purpleBirdOfPreyNames [gameManager.currentTurnPlayer.playerBirdOfPreyPurchased];
				break;

			case CombatUnit.UnitType.Destroyer:

				defaultShipName.text = uiManager.GetComponent<DefaultUnitNames> ().purpleDestroyerNames [gameManager.currentTurnPlayer.playerDestroyerPurchased];
				break;

			case CombatUnit.UnitType.Starship:

				defaultShipName.text = uiManager.GetComponent<DefaultUnitNames> ().purpleStarshipNames [gameManager.currentTurnPlayer.playerStarshipPurchased];
				break;

			default:

				defaultShipName.text = "DefaultShip";
				break;

			}

			break;

		case Player.Color.Red:

			//switch case on unit type
			switch (newUnitType) {

			case CombatUnit.UnitType.Scout:

				defaultShipName.text = uiManager.GetComponent<DefaultUnitNames> ().redScoutNames [gameManager.currentTurnPlayer.playerScoutPurchased];
				break;

			case CombatUnit.UnitType.BirdOfPrey:

				defaultShipName.text = uiManager.GetComponent<DefaultUnitNames> ().redBirdOfPreyNames [gameManager.currentTurnPlayer.playerBirdOfPreyPurchased];
				break;

			case CombatUnit.UnitType.Destroyer:

				defaultShipName.text = uiManager.GetComponent<DefaultUnitNames> ().redDestroyerNames [gameManager.currentTurnPlayer.playerDestroyerPurchased];
				break;

			case CombatUnit.UnitType.Starship:

				defaultShipName.text = uiManager.GetComponent<DefaultUnitNames> ().redStarshipNames [gameManager.currentTurnPlayer.playerStarshipPurchased];
				break;

			default:

				defaultShipName.text = "DefaultShip";
				break;

			}

			break;

		case Player.Color.Blue:

			//switch case on unit type
			switch (newUnitType) {

			case CombatUnit.UnitType.Scout:

				defaultShipName.text = uiManager.GetComponent<DefaultUnitNames> ().blueScoutNames [gameManager.currentTurnPlayer.playerScoutPurchased];
				break;

			case CombatUnit.UnitType.BirdOfPrey:

				defaultShipName.text = uiManager.GetComponent<DefaultUnitNames> ().blueBirdOfPreyNames [gameManager.currentTurnPlayer.playerBirdOfPreyPurchased];
				break;

			case CombatUnit.UnitType.Destroyer:

				defaultShipName.text = uiManager.GetComponent<DefaultUnitNames> ().blueDestroyerNames [gameManager.currentTurnPlayer.playerDestroyerPurchased];
				break;

			case CombatUnit.UnitType.Starship:

				defaultShipName.text = uiManager.GetComponent<DefaultUnitNames> ().blueStarshipNames [gameManager.currentTurnPlayer.playerStarshipPurchased];
				break;

			default:

				defaultShipName.text = "DefaultShip";
				break;

			}

			break;

		default:

			defaultShipName.text = "DefaultShip";
			break;

		}


		//activate the text input field again
		shipNameInputField.ActivateInputField ();
		shipNameInputField.Select ();

		shipNameInputField.text = "";

	}

	//this function responds to clicking the purchase button
	private void ResolvePurchaseButtonClick(){

		//check if a user has input a ship name
		if (newUnitName.Length == 0) {

			//if there is no user input, set the ship name to the default
			newUnitName = defaultShipName.text;

		}

		//create a new Unit Data instance
		NewUnitEventData newUnitEventData = new NewUnitEventData(this);

		//invoke a purchase unit event
		OnPurchasedNewShip.Invoke(newUnitEventData);

		//close the panel
		CloseNameNewShipPanel();

	}

	//this function responds to clicking the cancel button
	private void ResolveCancelButtonClick(){

		//invoke a purchase unit event
		OnCanceledPurchase.Invoke();

		//close the panel
		CloseNameNewShipPanel();

	}

	//this function responds to clicking the back button
	private void ResolveBackButtonClick(){

		//invoke a return event
		OnReturnToPlaceUnit.Invoke();

		//close the panel
		CloseNameNewShipPanel();

	}

	//this function responds to a user entering a ship name
	private void ResolveEnteredName(string userInput){

		//set the new unit name to the user input
		newUnitName = userInput;

		//check if the user has hit enter with a valid rename string entered
		if (userInput != "" && (Input.GetKey (KeyCode.Return) == true || Input.GetKey (KeyCode.KeypadEnter) == true)) {

			//if so, treat that as hitting the yes button
			purchaseButton.onClick.Invoke();

		}

	}

	//this function sets the border color
	private void SetBorderColor(Player.Color targetedPlayerColor){

		//switch case based on player
		switch (targetedPlayerColor) {

		case Player.Color.Green:

			nameNewShipPanel.GetComponent<Image> ().color = GameManager.greenColor;
			break;

		case Player.Color.Purple:

			nameNewShipPanel.GetComponent<Image> ().color = GameManager.purpleColor;
			break;

		case Player.Color.Red:

			nameNewShipPanel.GetComponent<Image> ().color = GameManager.redColor;
			break;

		case Player.Color.Blue:

			nameNewShipPanel.GetComponent<Image> ().color = GameManager.blueColor;
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

		if (uiManager != null) {
			
			//remove listener for outfitting a ship
			uiManager.GetComponent<PurchaseManager> ().OnOutfittedShip.RemoveListener (purchaseStoreNewUnitDataAction);

		}

		if (mouseManager != null) {
			
			//remove listener for choosing new ship location
			mouseManager.OnPlacedNewUnit.RemoveListener (hexResolvePlacedNewUnitAction);

		}

		if (purchaseButton != null) {
			
			//remove listeners to the button presses
			purchaseButton.onClick.RemoveListener (ResolvePurchaseButtonClick);
		}

		if (cancelButton != null) {
			
			cancelButton.onClick.RemoveListener (ResolveCancelButtonClick);

		}

		if (shipNameInputField != null) {
			
			//remove listener for the input submit event
			shipNameInputField.onEndEdit.RemoveListener (stringResolvedEnteredNameAction);

		}

	}

}
