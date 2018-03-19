using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System;

public class StatusPanel : MonoBehaviour {
	
	//variable to hold the StatusPanel
	public GameObject statusPanel;

	//variable to hold the close button
	public Button closeButton;

	//variable to hold the open button
	public Button openButton;

	//variables to hold the tab buttons
	public Button planetStatusButton;
	public Button moneyStatusButton;
	public Button shipStatusButton;

	//enum to store the state of the status panel
	public enum PanelState{

		planetStatus,
		moneyStatus,
		shipStatus,

	}

	//variable to hold the panel state
	private PanelState statusPanelState;
	public PanelState StatusPanelState{

		get {

			return statusPanelState;

		}

		private set {

			if (value == statusPanelState) {

				return;

			} else {

				statusPanelState = value;

				OnStatusPanelStateChange.Invoke(statusPanelState);

			}

		}

	}

	//color for selected button tab
	private Color selectedButtonColor = new Color (240.0f / 255.0f, 240.0f / 255.0f, 20.0f / 255.0f, 255.0f / 255.0f); 

	//variable to hold the tileMap
	//private TileMap tileMap;

	//variable to hold the gameManager
	private GameManager gameManager;

	//variable to hold the colonyManager
	private ColonyManager colonyManager;

	//variable to hold the planetStatusPanel
	private GameObject planetStatusPanel;

	//variable to hold the  moneyStatusPanel
	private GameObject moneyStatusPanel;

	//variable to hold the shipStatusPanel
	private GameObject shipStatusPanel;

	//variable to hold the team planet row
	public GameObject teamPlanetTotalRow;
	public TextMeshProUGUI teamPlanetRedGreenTotalText;
	public TextMeshProUGUI teamPlanetBluePurpleTotalText;

	//variables to hold the raw images
	public RawImage scoutRawImage;
	public RawImage birdOfPreyRawImage;
	public RawImage destroyerRawImage;
	public RawImage starshipRawImage;

	//arrays to hold the TMPro objexts for the names
	private TextMeshProUGUI[] scoutSectionNames;
	private TextMeshProUGUI[] birdOfPreySectionNames;
	private TextMeshProUGUI[] destroyerSectionNames;
	private TextMeshProUGUI[] starshipSectionNames;
	private TextMeshProUGUI[] fleetTotalNames;

	//event to announce the panelStatus has changed
	public PanelStateEvent OnStatusPanelStateChange = new PanelStateEvent();

	//classs derived from unityEvent to pass the action mode
	public class PanelStateEvent : UnityEvent<PanelState>{};

	//event to send tile and rawImage
	public rawImageEvent OnSetPlanetIcon = new rawImageEvent();

	public class rawImageEvent : UnityEvent<HexMapTile.TileType,RawImage>{}; 

	//event to send tile and rawImage
	public rawImageShipEvent OnSetShipIcon = new rawImageShipEvent();

	public class rawImageShipEvent : UnityEvent<GraphicsManager.MyUnit,RawImage>{}; 

	//unityActions
	private UnityAction<PanelState> panelStateResolvePanelStateChangeAction;
	private UnityAction<GameManager.ActionMode> actionModeSetStatusPanelOpenButtonStatusAction;

	// Use this for initialization
	public void Init () {
		
		//get the tileMap
		//tileMap = GameObject.FindGameObjectWithTag("TileMap").GetComponent<TileMap>();

		//get the colonyManager
		colonyManager = GameObject.FindGameObjectWithTag("ColonyManager").GetComponent<ColonyManager>();

		//get the gameManager
		gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

		//get the planet status panel
		planetStatusPanel = statusPanel.transform.GetChild(0).Find("PlanetStatusPanel").gameObject;

		//get the money status panel
		moneyStatusPanel = statusPanel.transform.GetChild(0).Find("MoneyStatusPanel").gameObject;

		//get the ship status panel
		shipStatusPanel = statusPanel.transform.GetChild(0).Find("ShipStatusPanel").gameObject;


		//Set the planet icons
		SetPlanetIcons();

		//check if this is a team game
		if (gameManager.teamsEnabled == true) {

			//enable the team row
			teamPlanetTotalRow.SetActive (true);

		} else {

			//disable the team row
			teamPlanetTotalRow.SetActive (false);

		}

		//initialize the ship name string arrays
		scoutSectionNames = shipStatusPanel.transform.Find("ScoutSection").Find("ScoutSectionContent").GetComponentsInChildren<TextMeshProUGUI>();
		birdOfPreySectionNames = shipStatusPanel.transform.Find("BirdOfPreySection").Find("BirdOfPreySectionContent").GetComponentsInChildren<TextMeshProUGUI>();
		destroyerSectionNames = shipStatusPanel.transform.Find("DestroyerSection").Find("DestroyerSectionContent").GetComponentsInChildren<TextMeshProUGUI>();
		starshipSectionNames = shipStatusPanel.transform.Find("StarshipSection").Find("StarshipSectionContent").GetComponentsInChildren<TextMeshProUGUI>();
		fleetTotalNames = shipStatusPanel.transform.Find("TotalsRow").Find("TotalSectionContent").GetComponentsInChildren<TextMeshProUGUI>();


		//set the default to ship status, so that clicking the planet status triggers a change
		StatusPanelState = PanelState.shipStatus;

		//set the actions
		panelStateResolvePanelStateChangeAction = (panelState) => {ResolveStatusPanelStateChange(panelState);};
		actionModeSetStatusPanelOpenButtonStatusAction = (actionMode) => {SetStatusPanelOpenButtonStatus(actionMode);};

		//add a listener to the close button on-click
		closeButton.onClick.AddListener(CloseStatusPanel);

		//add a listener to the open button on-click
		openButton.onClick.AddListener(OpenStatusPanel);

		//add a listener for the planet status button on-click
		planetStatusButton.onClick.AddListener(SetStateToPlanetStatus);

		//add a listener for the money status button on-click
		moneyStatusButton.onClick.AddListener(SetStateToMoneyStatus);

		//add a listener for the ship status button on-click
		shipStatusButton.onClick.AddListener(SetStateToShipStatus);

		//add a listener to state changes
		OnStatusPanelStateChange.AddListener(panelStateResolvePanelStateChangeAction);

		//add a listener for actionMode changes
		gameManager.OnActionModeChange.AddListener(actionModeSetStatusPanelOpenButtonStatusAction);

		//close the panel to start
		CloseStatusPanel();
		
	}

	//this function sets the state to planet status
	private void SetStateToPlanetStatus(){

		StatusPanelState = PanelState.planetStatus;

	}

	//this function sets the state to money status
	private void SetStateToMoneyStatus(){

		StatusPanelState = PanelState.moneyStatus;

	}

	//this function sets the state to ship status
	private void SetStateToShipStatus(){

		StatusPanelState = PanelState.shipStatus;

	}

	//this function administers state changes
	private void ResolveStatusPanelStateChange(PanelState panelState){

		//Debug.Log ("In Resolve - panelState is " + panelState.ToString ());

		//use a switch case to resolve the panelState
		switch (panelState) {

		case PanelState.planetStatus:

			//activate the planet status panel
			planetStatusPanel.SetActive(true);

			//deactivate the other panels
			moneyStatusPanel.SetActive(false);
			shipStatusPanel.SetActive(false);


			//call updatePlanetStatus
			UpdatePlanetStatus ();

			//turn the button yellow
			HighlightButton(planetStatusButton);

			//turn the other buttons default
			UnhighlightButton(moneyStatusButton);
			UnhighlightButton(shipStatusButton);

			break;

		case PanelState.moneyStatus:

			//activate the money status panel
			moneyStatusPanel.SetActive(true);

			//deactivate the other panels
			planetStatusPanel.SetActive(false);
			shipStatusPanel.SetActive(false);

			//call updateMoneyStatus
			UpdateMoneyStatus();

			//turn the button yellow
			HighlightButton(moneyStatusButton);

			//turn the other buttons default
			UnhighlightButton(planetStatusButton);
			UnhighlightButton(shipStatusButton);

			break;

		case PanelState.shipStatus:

			//activate the ship status panel
			shipStatusPanel.SetActive(true);

			//deactivate the other panels
			planetStatusPanel.SetActive(false);
			moneyStatusPanel.SetActive(false);

			//call updateShipStatus
			UpdateShipStatus();

			//turn the button yellow
			HighlightButton(shipStatusButton);

			//turn the other buttons default
			UnhighlightButton(planetStatusButton);
			UnhighlightButton(moneyStatusButton);

			break;

		default:

			break;

		}



	}

	//this function highlights a button passed to it
	private void HighlightButton(Button highlightedButton){

		ColorBlock colorBlock = highlightedButton.colors;
		colorBlock.normalColor = selectedButtonColor;
		colorBlock.highlightedColor = selectedButtonColor;
		highlightedButton.colors = colorBlock;

	}

	//this function unhighlights a button passed to it
	private void UnhighlightButton(Button unhighlightedButton){

		ColorBlock colorBlock = unhighlightedButton.colors;
		colorBlock = ColorBlock.defaultColorBlock;
		unhighlightedButton.colors = colorBlock;

	}

	//this function sets the planet icons in the status panel
	private void SetPlanetIcons(){

		//iterate through all children of the planetStatusPanel
		foreach (RawImage rawImage in planetStatusPanel.GetComponentsInChildren<RawImage>()) {

			//get the planet name from the table - need double parent based on hierarchy structure
			string planetText = rawImage.transform.parent.parent.GetComponentInChildren<TextMeshProUGUI> ().text;

			//need to convert planet X to planetX
			if (planetText == "Planet X") {

				planetText = "PlanetX";

			}

			//invoke the rawImage event, converting the planetText string to the tileType enum
			OnSetPlanetIcon.Invoke((HexMapTile.TileType)Enum.Parse(typeof(HexMapTile.TileType),planetText,true),rawImage);

			//Debug.Log (planetText);

		}

	}

	//this function will close the status panel
	private void CloseStatusPanel(){

		//disable the status panel
		statusPanel.SetActive (false);

	}

	//this function will open the status panel
	private void OpenStatusPanel(){

		//enable the status panel
		statusPanel.SetActive (true);

		//set the border color
		SetBorderColor(gameManager.currentTurnPlayer.color);

		//set the ship icons
		SetShipIcons();

		//click the planet button
		planetStatusButton.onClick.Invoke();

		//update the planet status
		UpdatePlanetStatus ();

	}

	//this function will update the planet ownership status
	private void UpdatePlanetStatus(){

		//get the column headers
		int numberPlayers = 4;

		string[] columnPlayer = new string[numberPlayers];

		//iterate through the 4 columns that have the player data
		for (int i = 0; i < numberPlayers; i++) {

			//store the string of the column header, have to use i+1 so we skip the first column which has the planet names
			//first getChild gets us to the PlayerHeadingsRow, 2nd getChild gets us to the TextMeshPro objects
			columnPlayer[i] = planetStatusPanel.transform.GetChild(0).GetChild(i + 1).GetComponent<TextMeshProUGUI>().text;

		}

		int numberPlanets = 10;

		//iterate through the rows that have the planet data
		for (int i = 0; i < numberPlanets; i++) {

			//get the name of the planet on this row
			string planetName = planetStatusPanel.transform.GetChild (i + 1).GetChild (0).GetComponent<TextMeshProUGUI> ().text;

			//if the planet is planetX, fix the string
			if (planetName == "Planet X") {

				planetName = "PlanetX";

			}

			string planetOwnerColor;

			//check to see if the planet has an owner
			if (colonyManager.planetOwners [planetName] != null) {

				//look up the owner of the planet and convert to a string
				planetOwnerColor = colonyManager.planetOwners [planetName].color.ToString ();


			}
			//else if there is no owner, set the string to null
			else {

				planetOwnerColor = null;

			}

			//iterate through the player columns and check if the owner matches the header
			for (int j = 0; j < numberPlayers; j++) {

				//check if the column header matches the owner
				if (columnPlayer [j] == planetOwnerColor) {

					//if the owner matches, enable the rawImage in that location
					planetStatusPanel.transform.GetChild (i + 1).GetChild (j + 1).GetComponentInChildren<RawImage> ().GetComponentInChildren<RawImage> ().enabled = true;

				}
				//else if they don't match, disable the rawImage
				else {

					planetStatusPanel.transform.GetChild (i + 1).GetChild (j + 1).GetComponentInChildren<RawImage> ().GetComponentInChildren<RawImage> ().enabled = false;

				}

			}

		}

		//iterate through the 4 columns that have the player data for the totals row
		for (int i = 0; i < numberPlayers; i++) {

			//store the string of the column header, have to use i+1 so we skip the first column which has the planet names
			//first getChild gets us to the totals row, 2nd getChild gets us to the TextMeshPro objects

			//check if teams are enabled
			if (gameManager.teamsEnabled == true) {

				//if teams are enabled, don't list the victory planets at the individual player level
				planetStatusPanel.transform.GetChild (numberPlanets + 2).GetChild (i + 1).GetComponent<TextMeshProUGUI> ().text = ( 
				    colonyManager.PlanetsControlledByPlayer (gameManager.GetPlayerFromColor (columnPlayer [i])).ToString ());
				
			
			} else {
				
				//if teams are not enabled, list individual victory planets
				planetStatusPanel.transform.GetChild (numberPlanets + 2).GetChild (i + 1).GetComponent<TextMeshProUGUI> ().text = ( 
				    colonyManager.PlanetsControlledByPlayer (gameManager.GetPlayerFromColor (columnPlayer [i])).ToString ()
				    + "/" + gameManager.victoryPlanets.ToString ());

			}

			//resize the font if necessary
			UIManager.AutoSizeTextMeshFont(planetStatusPanel.transform.GetChild(numberPlanets + 2).GetChild(i + 1).GetComponent<TextMeshProUGUI>());

		}

		//set the team totals
		if (gameManager.teamsEnabled == true) {

			teamPlanetRedGreenTotalText.text = ((colonyManager.PlanetsControlledByPlayer(gameManager.greenPlayer) +
				colonyManager.PlanetsControlledByPlayer(gameManager.redPlayer)).ToString()
				+ "/" + gameManager.victoryPlanets.ToString());
			
			teamPlanetBluePurpleTotalText.text = ((colonyManager.PlanetsControlledByPlayer(gameManager.purplePlayer) + 
				colonyManager.PlanetsControlledByPlayer(gameManager.bluePlayer)).ToString ()
				+ "/" + gameManager.victoryPlanets.ToString());

		}

	}

	//this function will update the money status for players when called
	private void UpdateMoneyStatus(){
		
		//get the column headers
		int numberPlayers = 4;

		string[] columnPlayer = new string[numberPlayers];

		//iterate through the 4 columns that have the player data
		for (int i = 0; i < numberPlayers; i++) {

			//store the string of the column header, have to use i+1 so we skip the first column which has the planet names
			//first getChild gets us to the PlayerHeadingsRow, 2nd getChild gets us to the TextMeshPro objects
			columnPlayer[i] = moneyStatusPanel.transform.GetChild(0).GetChild(i + 1).GetComponent<TextMeshProUGUI>().text;

		}

		//iterate through the players
		for (int i = 0; i < numberPlayers; i++) {

			//set the current cash for each player
			//cache the cash value
			string cashString = gameManager.GetPlayerFromColor(columnPlayer[i]).playerMoney.ToString();

			//first getChild takes us to the cash row
			//2nd getChild takes us to the player columns within the row
			moneyStatusPanel.transform.GetChild(1).GetComponentsInChildren<TextMeshProUGUI>()[i + 1].text = (cashString);

			//store the length of the cash string
			int cashStringLength = cashString.Length;


			//set the planet income for each player
			//cache the planet income value
			string planetIncomeString = (gameManager.GetPlayerFromColor(columnPlayer[i]).playerPlanets * Player.GetPlanetValue()).ToString();

			//store the length of the income string
			int planetIncomeStringLength = planetIncomeString.Length;

			//first getChild takes us to the planet income row
			//2nd getChild takes us to the player columns within the row
			moneyStatusPanel.transform.GetChild(2).GetComponentsInChildren<TextMeshProUGUI>()[i + 1].text = ( "+" + planetIncomeString);


			//set the starbase income for each player
			//cache the planet income value
			string starbaseIncomeString = gameManager.GetPlayerFromColor(columnPlayer[i]).GetPlayerStarbaseIncome().ToString();

			//store the length of the income string
			int starbaseIncomeStringLength = starbaseIncomeString.Length;

			//first getChild takes us to the starbase income row
			//2nd getChild takes us to the player columns within the row
			moneyStatusPanel.transform.GetChild(3).GetComponentsInChildren<TextMeshProUGUI>()[i + 1].text = ("+" + starbaseIncomeString);

			//set the next turn cash for each player
			//cache the next turn cash value
			string nextTurnCashString = (gameManager.GetPlayerFromColor(columnPlayer[i]).playerMoney + 
				gameManager.GetPlayerFromColor(columnPlayer[i]).playerPlanets * Player.GetPlanetValue() + 
				gameManager.GetPlayerFromColor(columnPlayer[i]).GetPlayerStarbaseIncome()).ToString();

			//store the length of the nexty turn cash string
			int nextTurnCashStringLength = nextTurnCashString.Length;

			//first getChild takes us to the next turn cash row
			//2nd getChild takes us to the player columns within the row
			moneyStatusPanel.transform.GetChild(5).GetComponentsInChildren<TextMeshProUGUI>()[i + 1].text = (nextTurnCashString);

		}

	}

	//this function sets the openStatusPanel button status
	private void SetStatusPanelOpenButtonStatus(GameManager.ActionMode actionMode){

		//check if we are in a locked action mode
		if (UIManager.lockMenuActionModes.Contains (actionMode)) {

			openButton.interactable = false;

		} else {

			//if the mode isn't locked, the button can be interactable
			openButton.interactable = true;

		}



	}

	//this function sets the border color
	private void SetBorderColor(Player.Color playerColor){

		//switch case based on player
		switch (playerColor) {

		case Player.Color.Green:

			statusPanel.GetComponent<Image> ().color = GameManager.greenColor;
			break;

		case Player.Color.Purple:

			statusPanel.GetComponent<Image> ().color = GameManager.purpleColor;
			break;

		case Player.Color.Red:

			statusPanel.GetComponent<Image> ().color = GameManager.redColor;
			break;

		case Player.Color.Blue:

			statusPanel.GetComponent<Image> ().color = GameManager.blueColor;
			break;

		default:

			break;

		}

	}

	//this function sets the ship picture icons in the purchase ship panel
	private void SetShipIcons(){

		//store the player color as a string
		string playerColorString = gameManager.currentTurnPlayer.color.ToString();

		//invoke events to set the graphics
		OnSetShipIcon.Invoke((GraphicsManager.MyUnit)Enum.Parse(typeof(GraphicsManager.MyUnit),playerColorString+"Scout",true),scoutRawImage);
		OnSetShipIcon.Invoke((GraphicsManager.MyUnit)Enum.Parse(typeof(GraphicsManager.MyUnit),playerColorString+"Bird",true),birdOfPreyRawImage);
		OnSetShipIcon.Invoke((GraphicsManager.MyUnit)Enum.Parse(typeof(GraphicsManager.MyUnit),playerColorString+"Destroyer",true),destroyerRawImage);
		OnSetShipIcon.Invoke((GraphicsManager.MyUnit)Enum.Parse(typeof(GraphicsManager.MyUnit),playerColorString+"Starship",true),starshipRawImage);

	}

	//this function updates the ship status panel
	private void UpdateShipStatus(){

		//first, we need to find out what player is in what column
		//get the column headers
		int numberPlayers = 4;

		string[] columnPlayer = new string[numberPlayers];

		//iterate through the 4 columns that have the player data
		for (int i = 0; i < numberPlayers; i++) {

			//store the string of the column header, have to use i+1 so we skip the first column which has the planet names
			//first getChild gets us to the PlayerHeadingsRow, 2nd getChild gets us to the TextMeshPro objects
			columnPlayer[i] = shipStatusPanel.transform.GetChild(0).GetChild(i + 1).GetComponent<TextMeshProUGUI>().text;

		}

		//next, we need to 
		//iterate through the players
		for (int i = 0; i < numberPlayers; i++) {

			//keep track of dead ships
			int deadShips = 0;

			//iterate through the max number of ships
			for (int j = 0; j < GameManager.maxShipsPerClass; j++) {

				//check if the ships purchased has been assigned
				if (gameManager.GetPlayerFromColor (columnPlayer [i]).playerScoutNamesPurchased [j] != null) {

					//if it has been, copy it over
					scoutSectionNames [GameManager.maxShipsPerClass * i + j].text = (gameManager.GetPlayerFromColor (columnPlayer [i]).playerScoutNamesPurchased [j]);

					//check if the ship is dead
					if (gameManager.GetPlayerFromColor (columnPlayer [i]).playerScoutPurchasedAlive [j] == false) {

						//if it is dead, strike thru
						scoutSectionNames [GameManager.maxShipsPerClass * i + j].text = ("<s><color=#ff0000>" + scoutSectionNames [GameManager.maxShipsPerClass * i + j].text + "</s></color>");

						deadShips++;

					}

					//resize the font if necessary
					UIManager.AutoSizeTextMeshFont(scoutSectionNames [GameManager.maxShipsPerClass * i + j]);

				} else {

					//else, if it is not assigned, set it to blank - leave a space so the height doesn't go to zero
					scoutSectionNames [GameManager.maxShipsPerClass * i + j].text = (" ");

				}

				//check if the ships purchased has been assigned
				if (gameManager.GetPlayerFromColor (columnPlayer [i]).playerBirdOfPreyNamesPurchased [j] != null) {

					//if it has been, copy it over
					birdOfPreySectionNames [GameManager.maxShipsPerClass * i + j].text = (gameManager.GetPlayerFromColor (columnPlayer [i]).playerBirdOfPreyNamesPurchased [j]);

					//check if the ship is dead
					if (gameManager.GetPlayerFromColor (columnPlayer [i]).playerBirdOfPreyPurchasedAlive [j] == false) {

						//if it is dead, strike thru
						birdOfPreySectionNames [GameManager.maxShipsPerClass * i + j].text = ("<s><color=#ff0000>" + birdOfPreySectionNames [GameManager.maxShipsPerClass * i + j].text + "</s></color>");

						deadShips++;

					}

					//resize the font if necessary
					UIManager.AutoSizeTextMeshFont(birdOfPreySectionNames [GameManager.maxShipsPerClass * i + j]);

				}else {

					//else, if it is not assigned, set it to blank
					birdOfPreySectionNames [GameManager.maxShipsPerClass * i + j].text = (" ");

				}

				//check if the ships purchased has been assigned
				if (gameManager.GetPlayerFromColor (columnPlayer [i]).playerDestroyerNamesPurchased [j] != null) {

					//if it has been, copy it over
					destroyerSectionNames [GameManager.maxShipsPerClass * i + j].text = (gameManager.GetPlayerFromColor (columnPlayer [i]).playerDestroyerNamesPurchased [j]);

					//check if the ship is dead
					if (gameManager.GetPlayerFromColor (columnPlayer [i]).playerDestroyerPurchasedAlive [j] == false) {

						//if it is dead, strike thru
						destroyerSectionNames [GameManager.maxShipsPerClass * i + j].text = ("<s><color=#ff0000>" + destroyerSectionNames [GameManager.maxShipsPerClass * i + j].text + "</s></color>");

						deadShips++;

					}

					//resize the font if necessary
					UIManager.AutoSizeTextMeshFont(destroyerSectionNames [GameManager.maxShipsPerClass * i + j]);

				}else {

					//else, if it is not assigned, set it to blank
					destroyerSectionNames [GameManager.maxShipsPerClass * i + j].text = (" ");

				}

				//check if the ships purchased has been assigned
				if (gameManager.GetPlayerFromColor (columnPlayer [i]).playerStarshipNamesPurchased [j] != null) {

					//if it has been, copy it over
					starshipSectionNames [GameManager.maxShipsPerClass * i + j].text = (gameManager.GetPlayerFromColor (columnPlayer [i]).playerStarshipNamesPurchased [j]);

					//check if the ship is dead
					if (gameManager.GetPlayerFromColor (columnPlayer [i]).playerStarshipPurchasedAlive [j] == false) {

						//if it is dead, strike thru
						starshipSectionNames [GameManager.maxShipsPerClass * i + j].text = ("<s><color=#ff0000>" + starshipSectionNames [GameManager.maxShipsPerClass * i + j].text + "</s></color>");

						deadShips++;

					}

					//resize the font if necessary
					UIManager.AutoSizeTextMeshFont(starshipSectionNames [GameManager.maxShipsPerClass * i + j]);

				}else {

					//else, if it is not assigned, set it to blank
					starshipSectionNames [GameManager.maxShipsPerClass * i + j].text = (" ");

				}

			}

			//the +1 gets to the ship totals
			fleetTotalNames [i + 1].text = ((gameManager.GetPlayerFromColor (columnPlayer [i]).playerScoutPurchased +
			gameManager.GetPlayerFromColor (columnPlayer [i]).playerBirdOfPreyPurchased +
			gameManager.GetPlayerFromColor (columnPlayer [i]).playerDestroyerPurchased +
			gameManager.GetPlayerFromColor (columnPlayer [i]).playerStarshipPurchased - 
				deadShips).ToString ());

		}

	}

	//this function handles OnDestroy
	private void OnDestroy(){

		RemoveAllListeners ();

	}

	//this function removes all listeners
	private void RemoveAllListeners(){

		if (closeButton != null) {
			
			//remove a listener to the close button on-click
			closeButton.onClick.RemoveListener (CloseStatusPanel);

		}

		if (openButton != null) {
			
			//remove a listener to the open button on-click
			openButton.onClick.RemoveListener (OpenStatusPanel);

		}

		if (planetStatusButton != null) {

			//remove a listener for the planet status button on-click
			planetStatusButton.onClick.RemoveListener (SetStateToPlanetStatus);

		}

		if (moneyStatusButton != null) {
			
			//remove a listener for the money status button on-click
			moneyStatusButton.onClick.RemoveListener (SetStateToMoneyStatus);

		}

		if (shipStatusButton != null) {
			
			//remove a listener for the ship status button on-click
			shipStatusButton.onClick.RemoveListener (SetStateToShipStatus);

		}
			
		//remove a listener to state changes
		OnStatusPanelStateChange.RemoveListener (panelStateResolvePanelStateChangeAction);

		if (gameManager != null) {
			
			//remove a listener for actionMode changes
			gameManager.OnActionModeChange.RemoveListener (actionModeSetStatusPanelOpenButtonStatusAction);

		}

	}

}
