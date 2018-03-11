using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class ConfigureLocalGameWindow : MonoBehaviour {

	//variable to hold the window
	public GameObject configureLocalGameWindow;

	//variable to hold the button that opens the panel
	public Button newLocalGameButton;

	//variable to hold the exit window button
	public Button exitWindowButton;

	//variables to hold the team buttons
	public Button teamsYesButton;
	public Button teamsNoButton;

	//variables to hold the planet victory up and down buttons
	public Button increasePlanetsButton;
	public Button decreasePlanetsButton;

	//variable to hold the planet count toggle
	public Toggle planetCountToggle;

	//variable to hold the Team roww
	public GameObject teamOneRow;
	public GameObject teamTwoRow;

	//variable to hold the blank spaces above and below teamOneRow
	public GameObject spaceAboveTeamOneRow;
	public GameObject spaceBelowTeamOneRow;

	//variable to hold the input fields
	public TMP_InputField greenPlayerInputField;
	public TMP_InputField redPlayerInputField;
	public TMP_InputField purplePlayerInputField;
	public TMP_InputField bluePlayerInputField;

	public TextMeshProUGUI greenPlayerPlaceholder;
	public TextMeshProUGUI redPlayerPlaceholder;	
	public TextMeshProUGUI purplePlayerPlaceholder;
	public TextMeshProUGUI bluePlayerPlaceholder;

	//variable to hold the team divider line
	public GameObject teamDividerLine;

	//variable to hold the team divider space
	public GameObject teamDividerSpace;

	//variable to hold the createGame button
	public Button createGameButton;

	//variable to hold the cancelButton
	public Button cancelButton;

	//variables to hold the default player names
	private string defaultGreenPlayerName = "Green Player";
	private string defaultRedPlayerName = "Red Player";
	private string defaultPurplePlayerName = "Purple Player";
	private string defaultBluePlayerName = "Blue Player";

	//variable to hold the team state
	public bool teamsEnabled {

		get;
		private set;

	}

	//variable to hold the number of planets
	public int planetCount {

		get;
		private set;

	}

	//variables to hold the default planet counts
	private int defaultPlanetsTeamsYes = 8;
	private int defaultPlanetsTeamsNo = 7;

	//variables to hold the min and max planet values
	private int minPlanetValue = 6;
	private int maxPlanetValue = 10;

	//variable to hold the player names
	public string greenPlayerName {

		get;
		private set;
	
	}

	public string redPlayerName{

		get;
		private set;

	}

	public string purplePlayerName{

		get;
		private set;

	}

	public string bluePlayerName{

		get;
		private set;

	}

	//color for selected button tab
	private Color selectedButtonColor = new Color (240.0f / 255.0f, 240.0f / 255.0f, 20.0f / 255.0f, 255.0f / 255.0f); 

	//event for creating a new game
	public UnityEvent OnCreateNewGame = new UnityEvent();


	// Use this for initialization
	public void Init () {

		//add event listeners
		AddEventListeners();

		//set the default player names
		SetDefaultPlayerNames();

		//set the default team state
		SetTeamsToNo();
				
	}

	//this function adds event listeners
	private void AddEventListeners(){
		
		//add a listener for the trigger button to open the window
		newLocalGameButton.onClick.AddListener(OpenWindow);

		//add a listener for the exit button to close the window
		exitWindowButton.onClick.AddListener(CloseWindow);

		//add a listener for clicking the teamsYesButton
		teamsYesButton.onClick.AddListener(SetTeamsToYes);

		//add a listener for clicking the teamsNoButton
		teamsNoButton.onClick.AddListener(SetTeamsToNo);

		//add a listener for clicking the planet up button
		increasePlanetsButton.onClick.AddListener(ResolvePlanetUpButtonPress);

		//add a listener for clicking the planet down button
		decreasePlanetsButton.onClick.AddListener(ResolvePlanetDownButtonPress);

		//add a listener for clicking the createNewGameButton
		createGameButton.onClick.AddListener(CreateNewGame);

		//add a listener for clicking the cancel button
		cancelButton.onClick.AddListener(CloseWindow);

		//set the default player names
		SetDefaultPlayerNames();

		//set the default team state
		SetTeamsToNo();

	}

	//this function will open the window
	private void OpenWindow(){

		configureLocalGameWindow.SetActive (true);

	}

	//this function will close the window
	private void CloseWindow(){

		configureLocalGameWindow.SetActive (false);

	}

	//this function sets the default player names
	private void SetDefaultPlayerNames(){

		greenPlayerPlaceholder.text = defaultGreenPlayerName;
		redPlayerPlaceholder.text = defaultRedPlayerName;
		purplePlayerPlaceholder.text = defaultPurplePlayerName;
		bluePlayerPlaceholder.text = defaultBluePlayerName;

	}

	//this function will set the team state to yes
	private void SetTeamsToYes(){

		//set the teams enabled flag
		teamsEnabled = true;

		//highlight the yes button
		HighlightButton(teamsYesButton);

		//unhighlight the no button
		UnhighlightButton(teamsNoButton);

		//enable the team rows
		teamOneRow.SetActive(true);
		teamTwoRow.SetActive (true);

		//enable the divider line, disable the space
		teamDividerLine.SetActive(true);
		teamDividerSpace.SetActive (false);	

		//disable the spaces around the team 1 row
		spaceAboveTeamOneRow.SetActive(false);
		spaceBelowTeamOneRow.SetActive (false);

		//set the default planets
		UpdatePlanetCount(defaultPlanetsTeamsYes);

	}

	//this function will set the team state to no
	private void SetTeamsToNo(){

		//set the teams enabled flag
		teamsEnabled = false;

		//highlight the no button
		HighlightButton(teamsNoButton);

		//unhighlight the yes button
		UnhighlightButton(teamsYesButton);

		//disable the team rows
		teamOneRow.SetActive(false);
		teamTwoRow.SetActive (false);

		//disable the divider line, enable the space
		teamDividerLine.SetActive(false);
		teamDividerSpace.SetActive (false);	

		//enable the spaces around the team 1 row
		spaceAboveTeamOneRow.SetActive(true);
		spaceBelowTeamOneRow.SetActive (true);

		//set the default planets
		UpdatePlanetCount(defaultPlanetsTeamsNo);

	}

	//this function will respond to the planet up button
	private void ResolvePlanetUpButtonPress(){

		//check if the current planet value is below the maximum
		if (planetCount < maxPlanetValue) {

			//increment the planet count
			UpdatePlanetCount(planetCount + 1);

		}

	}

	//this function will respond to the planet down button
	private void ResolvePlanetDownButtonPress(){

		//check if the current planet value is below the maximum
		if (planetCount > minPlanetValue) {

			//decrement the planet count
			UpdatePlanetCount(planetCount - 1);

		}

	}

	//this function sets the planet count and updates the toggle display value
	private void UpdatePlanetCount(int newValue){
		
		planetCount = newValue;
		planetCountToggle.GetComponentInChildren<TextMeshProUGUI> ().text = planetCount.ToString ();

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

	//this function will resolve creating the new game
	private void CreateNewGame(){

		//we need to set the player names based on the input fields
		if (greenPlayerInputField.text == "") {

			//set the green player name to the placeholder
			greenPlayerName = greenPlayerPlaceholder.text;

		} else {

			//set the green player name to the input text
			greenPlayerName = greenPlayerInputField.text;

		}


		if (redPlayerInputField.text == "") {

			//set the green player name to the placeholder
			redPlayerName = redPlayerPlaceholder.text;

		} else {

			//set the green player name to the input text
			redPlayerName = redPlayerInputField.text;

		}


		if (purplePlayerInputField.text == "") {

			//set the green player name to the placeholder
			purplePlayerName = purplePlayerPlaceholder.text;

		} else {

			//set the green player name to the input text
			purplePlayerName = purplePlayerInputField.text;

		}


		if (bluePlayerInputField.text == "") {

			//set the green player name to the placeholder
			bluePlayerName = bluePlayerPlaceholder.text;

		} else {

			//set the green player name to the input text
			bluePlayerName = bluePlayerInputField.text;

		}

		//close the window
		CloseWindow();

		//invoke the new game event
		OnCreateNewGame.Invoke();

	}

	//this function resolves OnDestroy
	private void OnDestroy(){

		RemoveAllListeners ();

	}

	//this function removes all event listeners
	private void RemoveAllListeners(){

		if (newLocalGameButton != null) {
			
			//remove a listener for the trigger button to open the window
			newLocalGameButton.onClick.RemoveListener (OpenWindow);

		}

		if (exitWindowButton != null) {
			
			//remove a listener for the exit button to close the window
			exitWindowButton.onClick.RemoveListener (CloseWindow);

		}

		if (teamsYesButton != null) {
			
			//remove a listener for clicking the teamsYesButton
			teamsYesButton.onClick.RemoveListener (SetTeamsToYes);

		}

		if (teamsNoButton != null) {
			
			//remove a listener for clicking the teamsNoButton
			teamsNoButton.onClick.RemoveListener (SetTeamsToNo);

		}

		if (increasePlanetsButton != null) {
			
			//remove a listener for clicking the planet up button
			increasePlanetsButton.onClick.RemoveListener (ResolvePlanetUpButtonPress);


		}

		if (decreasePlanetsButton != null) {
			
			//remove a listener for clicking the planet down button
			decreasePlanetsButton.onClick.RemoveListener (ResolvePlanetDownButtonPress);

		}

		if (createGameButton != null) {
			
			//remove a listener for clicking the createNewGameButton
			createGameButton.onClick.RemoveListener (CreateNewGame);

		}

		if (cancelButton != null) {
			
			//remove a listener for clicking the cancel button
			cancelButton.onClick.RemoveListener (CloseWindow);

		}

	}

}
