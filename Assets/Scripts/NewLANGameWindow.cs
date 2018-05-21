using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using System.IO;
using UnityEngine.Networking;

public class NewLANGameWindow : MonoBehaviour {

    //variable to hold the window
    public GameObject newLANGameWindow;

    //variable to hold the button that opens the panel
    public Button newLANGameButton;

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

    //variable to hold the LAN server buttons
    public Button hostButton;
    public Button serverButton;

    //variable to hold the game name input field
    public TMP_InputField gameNameInputField;
    public TextMeshProUGUI gameNamePlaceholder;

    //variable to hold the Team row
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

    //variables for the local control buttons
    public Button localGreenPlayerButton;
    public Button localRedPlayerButton;
    public Button localPurplePlayerButton;
    public Button localBluePlayerButton;

    //variables to hold the default player names
    private string defaultGreenPlayerName = "Green Player";
    private string defaultRedPlayerName = "Red Player";
    private string defaultPurplePlayerName = "Purple Player";
    private string defaultBluePlayerName = "Blue Player";

    //variables to hold the default game name
    private string defaulGameName = "defaultGame";

    //variable to hold the team state
    public bool teamsEnabled
    {

        get;
        private set;

    }

    //variable to hold the number of planets
    public int planetCount
    {

        get;
        private set;

    }

    //variables to hold the default planet counts
    private int defaultPlanetsTeamsYes = 8;
    private int defaultPlanetsTeamsNo = 7;

    //variables to hold the min and max planet values
    private int minPlanetValue = 6;
    private int maxPlanetValue = 10;

    //variable to hold the host state
    public bool localHost
    {

        get;
        private set;

    }

    //variable to hold the player names
    public string greenPlayerName
    {

        get;
        private set;

    }

    public string redPlayerName
    {

        get;
        private set;

    }

    public string purplePlayerName
    {

        get;
        private set;

    }

    public string bluePlayerName
    {

        get;
        private set;

    }

    //variable to hold the game name
    public string gameName
    {

        get;
        private set;

    }

    //bools to hold the local control statue
    public bool localControlGreen
    {

        get;
        private set;

    }

    public bool localControlRed
    {

        get;
        private set;

    }

    public bool localControlPurple
    {

        get;
        private set;

    }

    public bool localControlBlue
    {

        get;
        private set;

    }

    //color for selected button tab
    private Color selectedButtonColor = new Color(240.0f / 255.0f, 240.0f / 255.0f, 20.0f / 255.0f, 255.0f / 255.0f);

    //event for creating a new game
    public ConnectionEvent OnCreateNewLANGame = new ConnectionEvent();

    //event class for passing lan connection info
    public class ConnectionEvent : UnityEvent<LANConnectionInfo> { };

    //events for opening and closing the window
    public UnityEvent OnOpenPanel = new UnityEvent();
    public UnityEvent OnClosePanel = new UnityEvent();


    // Use this for initialization
    public void Init()
    {

        //add event listeners
        AddEventListeners();

        //set the default player names
        SetDefaultPlayerNames();

        //set the default game name
        SetDefaultGameName();

        //set the default team state
        SetTeamsToNo();

        //set the default server state
        SetServerToHost();

        //set the create game button status
        SetCreateGameButtonStatus();

        //start with the panel closed
        newLANGameWindow.SetActive(false);

    }

    //this function adds event listeners
    private void AddEventListeners()
    {

        //add a listener for the trigger button to open the window
        newLANGameButton.onClick.AddListener(OpenWindow);

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

        //add a listener for clicking the host button
        hostButton.onClick.AddListener(SetServerToHost);

        //add a listener for clicking the server button
        serverButton.onClick.AddListener(SetServerToDedicated);

        //add listener for clicking the local green button
        localGreenPlayerButton.onClick.AddListener(ResolveLocalGreenButtonClick);

        //add listener for clicking the local red button
        localRedPlayerButton.onClick.AddListener(ResolveLocalRedButtonClick);

        //add listener for clicking the local purple button
        localPurplePlayerButton.onClick.AddListener(ResolveLocalPurpleButtonClick);

        //add listener for clicking the local blue button
        localBluePlayerButton.onClick.AddListener(ResolveLocalBlueButtonClick);

        //add a listener for clicking the createNewGameButton
        createGameButton.onClick.AddListener(CreateNewGame);

        //add a listener for clicking the cancel button
        cancelButton.onClick.AddListener(CloseWindow);

    }

    //this function will open the window
    private void OpenWindow()
    {

        newLANGameWindow.SetActive(true);

        OnOpenPanel.Invoke();

    }

    //this function will close the window
    private void CloseWindow()
    {

        newLANGameWindow.SetActive(false);

        OnClosePanel.Invoke();

    }

    //this function sets the default player names
    private void SetDefaultPlayerNames()
    {

        greenPlayerPlaceholder.text = defaultGreenPlayerName;
        redPlayerPlaceholder.text = defaultRedPlayerName;
        purplePlayerPlaceholder.text = defaultPurplePlayerName;
        bluePlayerPlaceholder.text = defaultBluePlayerName;

    }

    //this function sets the default game name
    private void SetDefaultGameName()
    {

        gameNamePlaceholder.text = defaulGameName;

    }

    //this function will set the team state to yes
    private void SetTeamsToYes()
    {

        //set the teams enabled flag
        teamsEnabled = true;

        //highlight the yes button
        HighlightButton(teamsYesButton);

        //unhighlight the no button
        UnhighlightButton(teamsNoButton);

        //enable the team rows
        teamOneRow.SetActive(true);
        teamTwoRow.SetActive(true);

        //enable the divider line, disable the space
        teamDividerLine.SetActive(true);
        teamDividerSpace.SetActive(false);

        //disable the spaces around the team 1 row
        spaceAboveTeamOneRow.SetActive(false);
        spaceBelowTeamOneRow.SetActive(false);

        //set the default planets
        UpdatePlanetCount(defaultPlanetsTeamsYes);

    }

    //this function will set the team state to no
    private void SetTeamsToNo()
    {

        //set the teams enabled flag
        teamsEnabled = false;

        //highlight the no button
        HighlightButton(teamsNoButton);

        //unhighlight the yes button
        UnhighlightButton(teamsYesButton);

        //disable the team rows
        teamOneRow.SetActive(false);
        teamTwoRow.SetActive(false);

        //disable the divider line, enable the space
        teamDividerLine.SetActive(false);
        teamDividerSpace.SetActive(false);

        //enable the spaces around the team 1 row
        spaceAboveTeamOneRow.SetActive(true);
        spaceBelowTeamOneRow.SetActive(true);

        //set the default planets
        UpdatePlanetCount(defaultPlanetsTeamsNo);

    }

    //this function will respond to the planet up button
    private void ResolvePlanetUpButtonPress()
    {

        //check if the current planet value is below the maximum
        if (planetCount < maxPlanetValue)
        {

            //increment the planet count
            UpdatePlanetCount(planetCount + 1);

        }

    }

    //this function will respond to the planet down button
    private void ResolvePlanetDownButtonPress()
    {

        //check if the current planet value is below the maximum
        if (planetCount > minPlanetValue)
        {

            //decrement the planet count
            UpdatePlanetCount(planetCount - 1);

        }

    }

    //this function sets the planet count and updates the toggle display value
    private void UpdatePlanetCount(int newValue)
    {

        planetCount = newValue;
        planetCountToggle.GetComponentInChildren<TextMeshProUGUI>().text = planetCount.ToString();

    }

    //this function will set the server state to host
    private void SetServerToHost()
    {

        //set the server flag
        localHost = true;

        //highlight the host button
        HighlightButton(hostButton);

        //unhighlight the server button
        UnhighlightButton(serverButton);

        //make the local control buttons interactable
        localGreenPlayerButton.interactable = true;
        localRedPlayerButton.interactable = true;
        localPurplePlayerButton.interactable = true;
        localBluePlayerButton.interactable = true;

        //set player name input field interactability
        SetPlayerNameInputFieldStatus();

        //set the create game button status
        SetCreateGameButtonStatus();

    }

    //this function will set the server state to dedicated
    private void SetServerToDedicated()
    {

        //set the server flag
        localHost = false;

        //highlight the server button
        HighlightButton(serverButton);

        //unhighlight the host button
        UnhighlightButton(hostButton);

        //unhighlight the local buttons
        UnhighlightButton(localGreenPlayerButton);
        UnhighlightButton(localRedPlayerButton);
        UnhighlightButton(localPurplePlayerButton);
        UnhighlightButton(localBluePlayerButton);

        //make the local control buttons not interactable
        localGreenPlayerButton.interactable = false;
        localRedPlayerButton.interactable = false;
        localPurplePlayerButton.interactable = false;
        localBluePlayerButton.interactable = false;

        //clear the local control flags
        localControlGreen = false;
        localControlRed = false;
        localControlPurple = false;
        localControlBlue = false;

        //set player name input field interactability
        SetPlayerNameInputFieldStatus();

        //set the create game button status
        SetCreateGameButtonStatus();

    }

    //this function sets the player name input field interactability
    private void SetPlayerNameInputFieldStatus()
    {
        SetGreenPlayerInputStatus();
        SetRedPlayerInputStatus();
        SetPurplePlayerInputStatus();
        SetBluePlayerInputStatus();

    }

    //this function sets the green player input field status
    private void SetGreenPlayerInputStatus()
    {

        //check if the flag is set
        if(localControlGreen == true)
        {
            greenPlayerInputField.interactable = true;

        }
        else
        {
            greenPlayerInputField.interactable = false;
        }

    }

    //this function sets the red player input field status
    private void SetRedPlayerInputStatus()
    {

        //check if the flag is set
        if (localControlRed == true)
        {
            redPlayerInputField.interactable = true;

        }
        else
        {
            redPlayerInputField.interactable = false;
        }

    }

    //this function sets the purple player input field status
    private void SetPurplePlayerInputStatus()
    {

        //check if the flag is set
        if (localControlPurple == true)
        {
            purplePlayerInputField.interactable = true;

        }
        else
        {
            purplePlayerInputField.interactable = false;
        }

    }

    //this function sets the blue player input field status
    private void SetBluePlayerInputStatus()
    {

        //check if the flag is set
        if (localControlBlue == true)
        {
            bluePlayerInputField.interactable = true;

        }
        else
        {
            bluePlayerInputField.interactable = false;
        }

    }

    //this function resolves a localGreen button click
    private void ResolveLocalGreenButtonClick()
    {

        //check if the player flag is local
        if(localControlGreen == true)
        {

            //if there is control, we need to turn it off
            localControlGreen = false;

            //update the input field status
            SetGreenPlayerInputStatus();

            //unhighlight the button
            UnhighlightButton(localGreenPlayerButton);

        }
        else
        {
            //the else condition is that we are turning it on
            localControlGreen = true;

            //update the input field status
            SetGreenPlayerInputStatus();

            //highlight the button
            HighlightButton(localGreenPlayerButton);

        }

        //set the create game button status
        SetCreateGameButtonStatus();

    }

    //this function resolves a localRed button click
    private void ResolveLocalRedButtonClick()
    {

        //check if the player flag is local
        if (localControlRed == true)
        {

            //if there is control, we need to turn it off
            localControlRed = false;

            //update the input field status
            SetRedPlayerInputStatus();

            //unhighlight the button
            UnhighlightButton(localRedPlayerButton);

        }
        else
        {
            //the else condition is that we are turning it on
            localControlRed = true;

            //update the input field status
            SetRedPlayerInputStatus();

            //highlight the button
            HighlightButton(localRedPlayerButton);
        }

        //set the create game button status
        SetCreateGameButtonStatus();

    }

    //this function resolves a localPurple button click
    private void ResolveLocalPurpleButtonClick()
    {

        //check if the player flag is local
        if (localControlPurple == true)
        {

            //if there is control, we need to turn it off
            localControlPurple = false;

            //update the input field status
            SetPurplePlayerInputStatus();

            //unhighlight the button
            UnhighlightButton(localPurplePlayerButton);

        }
        else
        {
            //the else condition is that we are turning it on
            localControlPurple = true;

            //update the input field status
            SetPurplePlayerInputStatus();

            //highlight the button
            HighlightButton(localPurplePlayerButton);

        }

        //set the create game button status
        SetCreateGameButtonStatus();

    }

    //this function resolves a localBlue button click
    private void ResolveLocalBlueButtonClick()
    {

        //check if the player flag is local
        if (localControlBlue == true)
        {

            //if there is control, we need to turn it off
            localControlBlue = false;

            //update the input field status
            SetBluePlayerInputStatus();

            //unhighlight the button
            UnhighlightButton(localBluePlayerButton);

        }
        else
        {
            //the else condition is that we are turning it on
            localControlBlue = true;

            //update the input field status
            SetBluePlayerInputStatus();

            //highlight the button
            HighlightButton(localBluePlayerButton);

        }

        //set the create game button status
        SetCreateGameButtonStatus();

    }

    //this function sets the create game interactability status
    private void SetCreateGameButtonStatus()
    {
        //we cannot create the game if we are a local host, but haven't selected any players for local control
        if (localHost == true && localControlGreen == false && localControlRed == false && localControlPurple == false &&
            localControlBlue == false)
        {

            //button cannot be interactable
            createGameButton.interactable = false;

        }
        else
        {
            createGameButton.interactable = true;
        }

    }

    //this function highlights a button passed to it
    private void HighlightButton(Button highlightedButton)
    {

        ColorBlock colorBlock = highlightedButton.colors;
        colorBlock.normalColor = selectedButtonColor;
        colorBlock.highlightedColor = selectedButtonColor;
        highlightedButton.colors = colorBlock;

    }

    //this function unhighlights a button passed to it
    private void UnhighlightButton(Button unhighlightedButton)
    {

        ColorBlock colorBlock = unhighlightedButton.colors;
        colorBlock = ColorBlock.defaultColorBlock;
        unhighlightedButton.colors = colorBlock;

    }

    //this function will resolve creating the new game
    private void CreateNewGame()
    {

        //we need to set the player names based on the input fields
        if (greenPlayerInputField.text == "")
        {

            //set the green player name to the placeholder
            greenPlayerName = greenPlayerPlaceholder.text;

        }
        else
        {

            //set the green player name to the input text
            greenPlayerName = greenPlayerInputField.text;

        }


        if (redPlayerInputField.text == "")
        {

            //set the red player name to the placeholder
            redPlayerName = redPlayerPlaceholder.text;

        }
        else
        {

            //set the red player name to the input text
            redPlayerName = redPlayerInputField.text;

        }


        if (purplePlayerInputField.text == "")
        {

            //set the purple player name to the placeholder
            purplePlayerName = purplePlayerPlaceholder.text;

        }
        else
        {

            //set the purple player name to the input text
            purplePlayerName = purplePlayerInputField.text;

        }


        if (bluePlayerInputField.text == "")
        {

            //set the blue player name to the placeholder
            bluePlayerName = bluePlayerPlaceholder.text;

        }
        else
        {

            //set the blue player name to the input text
            bluePlayerName = bluePlayerInputField.text;

        }

        if (gameNameInputField.text == "")
        {

            //set the game name to the placeholder
            gameName = gameNamePlaceholder.text;

        }
        else
        {

            //set the game name to the input text
            gameName = gameNameInputField.text;

        }
    

        //close the window
        CloseWindow();

        //invoke the new game event
        OnCreateNewLANGame.Invoke(CurrentNewGameConnectionInfo());

    }

    //this function generates a LANConnectionInfo from the current game setup
    private LANConnectionInfo CurrentNewGameConnectionInfo()
    {

        return new LANConnectionInfo(
            NetworkManager.singleton.networkAddress.ToString(),
            NetworkManager.singleton.networkPort,
            gameName,
            teamsEnabled,
            true,   //since this is a new game, the player is alive
            localControlGreen,
            true,   //since this is a new game, the player is alive
            localControlRed,
            true,   //since this is a new game, the player is alive
            localControlPurple,
            true,   //since this is a new game, the player is alive
            localControlBlue,
            0,  //since this is a new game, planets are zero
            0,  //since this is a new game, planets are zero
            0,  //since this is a new game, planets are zero
            0,  //since this is a new game, planets are zero
            planetCount,
            GameManager.startingGameYear
            );
    }

    //this function resolves OnDestroy
    private void OnDestroy()
    {

        RemoveAllListeners();

    }

    //this function removes all event listeners
    private void RemoveAllListeners()
    {

        if (newLANGameButton != null)
        {

            //remove a listener for the trigger button to open the window
            newLANGameButton.onClick.RemoveListener(OpenWindow);

        }

        if (exitWindowButton != null)
        {

            //remove a listener for the exit button to close the window
            exitWindowButton.onClick.RemoveListener(CloseWindow);

        }

        if (teamsYesButton != null)
        {

            //remove a listener for clicking the teamsYesButton
            teamsYesButton.onClick.RemoveListener(SetTeamsToYes);

        }

        if (teamsNoButton != null)
        {

            //remove a listener for clicking the teamsNoButton
            teamsNoButton.onClick.RemoveListener(SetTeamsToNo);

        }

        if (increasePlanetsButton != null)
        {

            //remove a listener for clicking the planet up button
            increasePlanetsButton.onClick.RemoveListener(ResolvePlanetUpButtonPress);


        }

        if (decreasePlanetsButton != null)
        {

            //remove a listener for clicking the planet down button
            decreasePlanetsButton.onClick.RemoveListener(ResolvePlanetDownButtonPress);

        }

        if (createGameButton != null)
        {

            //remove a listener for clicking the createNewGameButton
            createGameButton.onClick.RemoveListener(CreateNewGame);

        }

        if (cancelButton != null)
        {

            //remove a listener for clicking the cancel button
            cancelButton.onClick.RemoveListener(CloseWindow);

        }

        if(hostButton != null)
        {
            //remove a listener for clicking the host button
            hostButton.onClick.RemoveListener(SetServerToHost);
            
        }

        if(serverButton != null)
        {
            //remove a listener for clicking the server button
            serverButton.onClick.RemoveListener(SetServerToDedicated);

        }

        if(localGreenPlayerButton != null)
        {
            //remove listener for clicking the local green button
            localGreenPlayerButton.onClick.RemoveListener(ResolveLocalGreenButtonClick);

        }

        if (localRedPlayerButton != null)
        {

            //remove listener for clicking the local red button
            localRedPlayerButton.onClick.RemoveListener(ResolveLocalRedButtonClick);

        }

        if (localPurplePlayerButton != null)
        {

            //remove listener for clicking the local purple button
            localPurplePlayerButton.onClick.RemoveListener(ResolveLocalPurpleButtonClick);

        }

        if (localBluePlayerButton != null)
        {

            //remove listener for clicking the local blue button
            localBluePlayerButton.onClick.RemoveListener(ResolveLocalBlueButtonClick);

        }

    }

}
