using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class LobbyLANGamePanel : MonoBehaviour {

    //managers
    private GameObject uiManager;
    private GameObject networkManager;

    //variable to hold the window
    public GameObject lobbyLANGameWindow;

    //variable to hold the game name title
    public TextMeshProUGUI gameNameTitleText;

    //variable to hold the exit window button
    public Button exitWindowButton;

    //variables to hold the team buttons
    public Button teamsYesButton;
    public Button teamsNoButton;

    //variable for the team value
    public TextMeshProUGUI teamStatusText;

    //variables to hold the planet victory up and down buttons
    public Button increasePlanetsButton;
    public Button decreasePlanetsButton;

    //variable to hold the planet count toggle
    public Toggle planetCountToggle;

    //variable to hold the planet value text
    public TextMeshProUGUI planetValueText;

    //variable to hold the game year
    public TextMeshProUGUI gameYearText;

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

    //variables to hold the player names text
    public TextMeshProUGUI greenPlayerNameText;
    public TextMeshProUGUI redPlayerNameText;
    public TextMeshProUGUI purplePlayerNameText;
    public TextMeshProUGUI bluePlayerNameText;

    //variable to hold the team divider line
    public GameObject teamDividerLine;

    //variable to hold the team divider space
    public GameObject teamDividerSpace;

    //variable to hold the leaveLobby button
    public Button exitLobbyButton;

    //variables for the local control buttons
    public Button localGreenPlayerButton;
    public Button localRedPlayerButton;
    public Button localPurplePlayerButton;
    public Button localBluePlayerButton;

    //variables to hold the local player texts
    public TextMeshProUGUI localGreenPlayerText;
    public TextMeshProUGUI localRedPlayerText;
    public TextMeshProUGUI localPurplePlayerText;
    public TextMeshProUGUI localBluePlayerText;

    //variables for the player planets
    public TextMeshProUGUI greenPlanetsText;
    public TextMeshProUGUI redPlanetsText;
    public TextMeshProUGUI purplePlanetsText;
    public TextMeshProUGUI bluePlanetsText;

    //variables for the ready buttons
    public Button readyGreenPlayerButton;
    public Button readyRedPlayerButton;
    public Button readyPurplePlayerButton;
    public Button readyBluePlayerButton;

    //variables to hold the ready player texts
    public TextMeshProUGUI readyGreenPlayerText;
    public TextMeshProUGUI readyRedPlayerText;
    public TextMeshProUGUI readyPurplePlayerText;
    public TextMeshProUGUI readyBluePlayerText;

    //variables to hold the default player names
    private string defaultGreenPlayerName = "Green Player";
    private string defaultRedPlayerName = "Red Player";
    private string defaultPurplePlayerName = "Purple Player";
    private string defaultBluePlayerName = "Blue Player";

    //variable to hold the team state
    private bool _teamsEnabled;
    public bool teamsEnabled
    {
        get
        {
            return _teamsEnabled;
        }
        private set
        {
            if (value == _teamsEnabled)
            {
                return;
            }
            else
            {
                _teamsEnabled = value;
                if (_teamsEnabled == true)
                {
                    teamStatusText.text = "Yes";
                }
                else
                {
                    teamStatusText.text = "No";
                }
            }
        }
    }

    //variable to hold the number of planets
    private int _planetCount;
    public int planetCount
    {
        get
        {
            return _planetCount;
        }
        private set
        {
            if (value == _planetCount)
            {
                return;
            }
            else
            {
                _planetCount = value;
                planetValueText.text = _planetCount.ToString();
            }
        }
    }

    //variables to hold the default planet counts
    private int defaultPlanetsTeamsYes = 8;
    private int defaultPlanetsTeamsNo = 7;

    //variables to hold the min and max planet values
    private int minPlanetValue = 6;
    private int maxPlanetValue = 10;

    //variable to hold the game year
    private int _gameYear;
    public int gameYear
    {
        get
        {
            return _gameYear;
        }
        private set
        {
            if (value == _gameYear)
            {
                return;
            }
            else
            {
                _gameYear = value;
                gameYearText.text = _gameYear.ToString();
            }
        }
    }

    //variable to hold the host state
    public bool localHost
    {

        get;
        private set;

    }

    //variable to hold the player names
    private string _greenPlayerName;
    public string greenPlayerName
    {
        get
        {
            return _greenPlayerName;
        }
        private set
        {
            if (value == _greenPlayerName)
            {
                return;
            }
            else
            {
                _greenPlayerName = value;
                greenPlayerNameText.text = _greenPlayerName;
                greenPlayerInputField.text = _greenPlayerName;
            }
        }
    }

    private string _redPlayerName;
    public string redPlayerName
    {
        get
        {
            return _redPlayerName;
        }
        private set
        {
            if (value == _redPlayerName)
            {
                return;
            }
            else
            {
                _redPlayerName = value;
                redPlayerNameText.text = _redPlayerName;
                redPlayerInputField.text = _redPlayerName;
            }
        }
    }

    private string _purplePlayerName;
    public string purplePlayerName
    {
        get
        {
            return _purplePlayerName;
        }
        private set
        {
            if (value == _purplePlayerName)
            {
                return;
            }
            else
            {
                _purplePlayerName = value;
                purplePlayerNameText.text = _purplePlayerName;
                purplePlayerInputField.text = _purplePlayerName;
            }
        }
    }

    private string _bluePlayerName;
    public string bluePlayerName
    {
        get
        {
            return _bluePlayerName;
        }
        private set
        {
            if (value == _bluePlayerName)
            {
                return;
            }
            else
            {
                _bluePlayerName = value;
                bluePlayerNameText.text = _bluePlayerName;
                bluePlayerInputField.text = _bluePlayerName;
            }
        }
    }

    //variable to hold the game name
    private string _gameName;
    public string gameName
    {
        get
        {
            return _gameName;
        }
        private set
        {
            if(value == _gameName)
            {
                return;
            }
            else
            {
                _gameName = value;
                gameNameTitleText.text = _gameName + " Lobby";
            }
        }
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

    //bools to hold the ready state
    private bool _readyGreen;
    public bool readyGreen
    {
        get
        {
            return _readyGreen;
        }
        private set
        {
            if (value == _readyGreen)
            {
                return;
            }
            else
            {
                _readyGreen = value;
                if (_readyGreen == true)
                {
                    readyGreenPlayerText.text = "Ready";
                }
                else
                {
                    readyGreenPlayerText.text = "Waiting";
                }
            }
        }
    }

    private bool _readyRed;
    public bool readyRed
    {
        get
        {
            return _readyRed;
        }
        private set
        {
            if (value == _readyRed)
            {
                return;
            }
            else
            {
                _readyRed = value;
                if (_readyRed == true)
                {
                    readyRedPlayerText.text = "Ready";
                }
                else
                {
                    readyRedPlayerText.text = "Waiting";
                }
            }
        }
    }

    private bool _readyPurple;
    public bool readyPurple
    {
        get
        {
            return _readyPurple;
        }
        private set
        {
            if (value == _readyPurple)
            {
                return;
            }
            else
            {
                _readyPurple = value;
                if (_readyPurple == true)
                {
                    readyPurplePlayerText.text = "Ready";
                }
                else
                {
                    readyPurplePlayerText.text = "Waiting";
                }
            }
        }
    }

    private bool _readyBlue;
    public bool readyBlue
    {
        get
        {
            return _readyBlue;
        }
        private set
        {
            if (value == _readyBlue)
            {
                return;
            }
            else
            {
                _readyBlue = value;
                if (_readyBlue == true)
                {
                    readyBluePlayerText.text = "Ready";
                }
                else
                {
                    readyBluePlayerText.text = "Waiting";
                }
            }
        }
    }

    //these bools track whether a player is already taken
    public bool greenPlayerIsTaken
    {

        get;
        private set;

    }

    public bool redPlayerIsTaken
    {

        get;
        private set;

    }

    public bool purplePlayerIsTaken
    {

        get;
        private set;

    }

    public bool bluePlayerIsTaken
    {

        get;
        private set;

    }

    //these bools track whether a player is alive and should be part of the game
    public bool greenPlayerIsAlive
    {

        get;
        private set;

    }

    public bool redPlayerIsAlive
    {

        get;
        private set;

    }

    public bool purplePlayerIsAlive
    {

        get;
        private set;

    }

    public bool bluePlayerIsAlive
    {

        get;
        private set;

    }

    //these variables hold the player connections
    private PlayerConnection _greenPlayerConnection;
    public PlayerConnection greenPlayerConnection
    {
        get
        {
            return _greenPlayerConnection;
        }
        private set
        {
            if (value == _greenPlayerConnection)
            {
                return;
            }
            else
            {
                _greenPlayerConnection = value;

            }
        }
    }

    private PlayerConnection _redPlayerConnection;
    public PlayerConnection redPlayerConnection
    {
        get
        {
            return _redPlayerConnection;
        }
        private set
        {
            if (value == _redPlayerConnection)
            {
                return;
            }
            else
            {
                _redPlayerConnection = value;

            }
        }
    }

    private PlayerConnection _purplePlayerConnection;
    public PlayerConnection purplePlayerConnection
    {
        get
        {
            return _purplePlayerConnection;
        }
        private set
        {
            if (value == _purplePlayerConnection)
            {
                return;
            }
            else
            {
                _purplePlayerConnection = value;

            }
        }
    }

    private PlayerConnection _bluePlayerConnection;
    public PlayerConnection bluePlayerConnection
    {
        get
        {
            return _bluePlayerConnection;
        }
        private set
        {
            if (value == _bluePlayerConnection)
            {
                return;
            }
            else
            {
                _bluePlayerConnection = value;

            }
        }
    }

    public bool isServer
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

    //unityActions
    private UnityAction<LANConnectionInfo> createLANGameAction;
    private UnityAction<LANConnectionInfo> joinLANGameAction;

    // Use this for initialization
    public void Init()
    {

        //get the managers
        uiManager = GameObject.FindGameObjectWithTag("UIManager");
        networkManager = GameObject.FindGameObjectWithTag("NetworkManager");

        //set actions
        SetActions();

        //add event listeners
        AddEventListeners();

        //set the default player names
        SetDefaultPlayerNames();

        //set the default team state
        SetTeamsToNo();

        //set the default server state
        SetServerToHost();

        //set the create game button status
        SetCreateGameButtonStatus();

        //start with the panel closed
        lobbyLANGameWindow.SetActive(false);

    }

    //this function sets the actions
    private void SetActions()
    {
        createLANGameAction = (connectionInfo) => {

            //we create the game, so we are the server
            isServer = true;
            ResolveEnterLobby(connectionInfo);

        };

        joinLANGameAction = (connectionInfo) => {

            //we join the game, so we are not the server
            isServer = false;
            ResolveEnterLobby(connectionInfo);

        };

    }

    //this function adds event listeners
    private void AddEventListeners()
    {

        //add a listener for the trigger button to open the window
        GameListItem.OnJoinLANGame.AddListener(joinLANGameAction);

        //add a listener for creating the LAN game as host or server
        uiManager.GetComponent<NewLANGameWindow>().OnCreateNewLANGame.AddListener(createLANGameAction);

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

        //add listener for clicking the local green button
        localGreenPlayerButton.onClick.AddListener(ResolveLocalGreenButtonClick);

        //add listener for clicking the local red button
        localRedPlayerButton.onClick.AddListener(ResolveLocalRedButtonClick);

        //add listener for clicking the local purple button
        localPurplePlayerButton.onClick.AddListener(ResolveLocalPurpleButtonClick);

        //add listener for clicking the local blue button
        localBluePlayerButton.onClick.AddListener(ResolveLocalBlueButtonClick);

        //add a listener for clicking the cancel button
        exitLobbyButton.onClick.AddListener(CloseWindow);

        //add listener for game name update
        NetworkLobbyLAN.OnUpdateGameName.AddListener(GetGameName);

        //add listener for team status update
        NetworkLobbyLAN.OnUpdateTeamsEnabled.AddListener(GetTeamStatus);

        //add listener for team victory planets update
        NetworkLobbyLAN.OnUpdateVictoryPlanets.AddListener(GetVictoryPlanets);

        //add listener for game year update
        NetworkLobbyLAN.OnUpdateGameYear.AddListener(GetGameYear);

        //add listener for green name update
        NetworkLobbyLAN.OnUpdateGreenPlayerName.AddListener(GetGreenPlayerName);

        //add listener for red name update
        NetworkLobbyLAN.OnUpdateRedPlayerName.AddListener(GetRedPlayerName);

        //add listener for purple name update
        NetworkLobbyLAN.OnUpdatePurplePlayerName.AddListener(GetPurplePlayerName);

        //add listener for blue name update
        NetworkLobbyLAN.OnUpdateBluePlayerName.AddListener(GetBluePlayerName);

        //add listener for green planets update
        NetworkLobbyLAN.OnUpdateGreenPlayerPlanets.AddListener(GetGreenPlayerPlanets);

        //add listener for red planets update
        NetworkLobbyLAN.OnUpdateRedPlayerPlanets.AddListener(GetRedPlayerPlanets);

        //add listener for purple planets update
        NetworkLobbyLAN.OnUpdatePurplePlayerPlanets.AddListener(GetPurplePlayerPlanets);

        //add listener for blue planets update
        NetworkLobbyLAN.OnUpdateBluePlayerPlanets.AddListener(GetBluePlayerPlanets);

        //add listener for green ready update
        NetworkLobbyLAN.OnUpdateGreenPlayerReady.AddListener(GetGreenPlayerReadyStatus);
       
        //add listener for red ready update
        NetworkLobbyLAN.OnUpdateRedPlayerReady.AddListener(GetRedPlayerReadyStatus);

        //add listener for purple ready update
        NetworkLobbyLAN.OnUpdatePurplePlayerReady.AddListener(GetPurplePlayerReadyStatus);

        //add listener for blue ready update
        NetworkLobbyLAN.OnUpdateBluePlayerReady.AddListener(GetBluePlayerReadyStatus);

        //add listener for green connection update
        NetworkLobbyLAN.OnUpdateGreenPlayerConnection.AddListener(GetGreenPlayerConnection);

        //add listener for red connection update
        NetworkLobbyLAN.OnUpdateRedPlayerConnection.AddListener(GetRedPlayerConnection);

        //add listener for purple connection update
        NetworkLobbyLAN.OnUpdatePurplePlayerConnection.AddListener(GetPurplePlayerConnection);

        //add listener for blue connection update
        NetworkLobbyLAN.OnUpdateBluePlayerConnection.AddListener(GetBluePlayerConnection);

        //add listener for ready buttons
        readyGreenPlayerButton.onClick.AddListener(ResolveReadyGreenButtonClick);
        readyRedPlayerButton.onClick.AddListener(ResolveReadyRedButtonClick);
        readyPurplePlayerButton.onClick.AddListener(ResolveReadyPurpleButtonClick);
        readyBluePlayerButton.onClick.AddListener(ResolveReadyBlueButtonClick);
    }

    //this function will open the window
    private void OpenWindow()
    {

        lobbyLANGameWindow.SetActive(true);

        OnOpenPanel.Invoke();

    }

    //this function will close the window
    private void CloseWindow()
    {

        lobbyLANGameWindow.SetActive(false);

        OnClosePanel.Invoke();

    }

    //this function resolves joining the game
    private void ResolveEnterLobby(LANConnectionInfo connectionInfo)
    {
        //set the fields from the connection info
        gameName = connectionInfo.gameName;

        //check if the connection has teams enabled
        if(connectionInfo.teamsEnabled == true)
        {
            SetTeamsToYes();
        }
        else
        {
            SetTeamsToNo();
        }
        
        planetCount = connectionInfo.victoryPlanets;
        gameYear = connectionInfo.gameYear;

        //set the player availability status
        greenPlayerIsTaken = connectionInfo.greenPlayerIsTaken;
        redPlayerIsTaken = connectionInfo.redPlayerIsTaken;
        purplePlayerIsTaken = connectionInfo.purplePlayerIsTaken;
        bluePlayerIsTaken = connectionInfo.bluePlayerIsTaken;

        SetGreenPlayerRowByAvailability();
        SetRedPlayerRowByAvailability();
        SetPurplePlayerRowByAvailability();
        SetBluePlayerRowByAvailability();

        //resolve the host status
        ResolveHostStatus();
        
        //open the window
        OpenWindow();
    }

    //this function sets the default player names
    private void SetDefaultPlayerNames()
    {

        greenPlayerPlaceholder.text = defaultGreenPlayerName;
        redPlayerPlaceholder.text = defaultRedPlayerName;
        purplePlayerPlaceholder.text = defaultPurplePlayerName;
        bluePlayerPlaceholder.text = defaultBluePlayerName;

        greenPlayerNameText.text = defaultGreenPlayerName;
        redPlayerNameText.text = defaultRedPlayerName;
        purplePlayerNameText.text = defaultPurplePlayerName;
        bluePlayerNameText.text = defaultBluePlayerName;

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
        planetCount = defaultPlanetsTeamsYes;

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
        planetCount = defaultPlanetsTeamsNo;

    }

    //this function will respond to the planet up button
    private void ResolvePlanetUpButtonPress()
    {

        //check if the current planet value is below the maximum
        if (planetCount < maxPlanetValue)
        {

            //increment the planet count
            planetCount++;

        }

    }

    //this function will respond to the planet down button
    private void ResolvePlanetDownButtonPress()
    {

        //check if the current planet value is below the maximum
        if (planetCount > minPlanetValue)
        {

            //decrement the planet count
            planetCount--;

        }

    }

    //this function will set the server state to host
    private void SetServerToHost()
    {

        //set the server flag
        localHost = true;

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
        if (localControlGreen == true)
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
        if (localControlGreen == true)
        {

            //if there is control, we need to turn it off
            localControlGreen = false;

            //update the input field status
            SetGreenPlayerInputStatus();

            //make the ready button not interactable
            readyGreenPlayerButton.interactable = false;

            //unhighlight the button
            UnhighlightButton(localGreenPlayerButton);

        }
        else
        {
            //the else condition is that we are turning it on
            localControlGreen = true;

            //update the input field status
            SetGreenPlayerInputStatus();

            //make the ready button interactable
            readyGreenPlayerButton.interactable = true;

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

            //make the ready button not interactable
            readyRedPlayerButton.interactable = false;

            //unhighlight the button
            UnhighlightButton(localRedPlayerButton);

        }
        else
        {
            //the else condition is that we are turning it on
            localControlRed = true;

            //update the input field status
            SetRedPlayerInputStatus();

            //make the ready button interactable
            readyRedPlayerButton.interactable = true;

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

            //make the ready button not interactable
            readyPurplePlayerButton.interactable = false;

            //unhighlight the button
            UnhighlightButton(localPurplePlayerButton);

        }
        else
        {
            //the else condition is that we are turning it on
            localControlPurple = true;

            //update the input field status
            SetPurplePlayerInputStatus();

            //make the ready button interactable
            readyPurplePlayerButton.interactable = true;

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

            //make the ready button not interactable
            readyBluePlayerButton.interactable = false;

            //unhighlight the button
            UnhighlightButton(localBluePlayerButton);

        }
        else
        {
            //the else condition is that we are turning it on
            localControlBlue = true;

            //update the input field status
            SetBluePlayerInputStatus();

            //make the ready button interactable
            readyBluePlayerButton.interactable = true;

            //highlight the button
            HighlightButton(localBluePlayerButton);

        }

        //set the create game button status
        SetCreateGameButtonStatus();

    }
    
    //this function resolves a ready green button click
    private void ResolveReadyGreenButtonClick()
    {
        
        //check if the player flag is ready
        if (readyGreen == true)
        {

            //we are turning it off
            readyGreen = false;

            //update the input field status
            greenPlayerInputField.interactable = true;

            //make the local button interactable
            localGreenPlayerButton.interactable = true;

            //unhighlight the button
            UnhighlightButton(readyGreenPlayerButton);

        }
        else
        {
            //the else condition is that we are turning it on
            readyGreen = true;

            //update the input field status
            greenPlayerInputField.interactable = false;

            //make the local button not interactable
            localGreenPlayerButton.interactable = false;

            //highlight the button
            HighlightButton(readyGreenPlayerButton);

        }

    }

    //this function resolves a ready red button click
    private void ResolveReadyRedButtonClick()
    {

        //check if the player flag is ready
        if (readyRed == true)
        {

            //we are turning it off
            readyRed = false;

            //update the input field status
            redPlayerInputField.interactable = true;

            //make the local button interactable
            localRedPlayerButton.interactable = true;

            //unhighlight the button
            UnhighlightButton(readyRedPlayerButton);

        }
        else
        {
            //the else condition is that we are turning it on
            readyRed = true;

            //update the input field status
            redPlayerInputField.interactable = false;

            //make the local button not interactable
            localRedPlayerButton.interactable = false;

            //highlight the button
            HighlightButton(readyRedPlayerButton);

        }

    }

    //this function resolves a ready purple button click
    private void ResolveReadyPurpleButtonClick()
    {

        //check if the player flag is ready
        if (readyPurple == true)
        {

            //we are turning it off
            readyPurple = false;

            //update the input field status
            purplePlayerInputField.interactable = true;

            //make the local button interactable
            localPurplePlayerButton.interactable = true;

            //unhighlight the button
            UnhighlightButton(readyPurplePlayerButton);

        }
        else
        {
            //the else condition is that we are turning it on
            readyPurple = true;

            //update the input field status
            purplePlayerInputField.interactable = false;

            //make the local button not interactable
            localPurplePlayerButton.interactable = false;

            //highlight the button
            HighlightButton(readyPurplePlayerButton);

        }

    }

    //this function resolves a ready blue button click
    private void ResolveReadyBlueButtonClick()
    {

        //check if the player flag is ready
        if (readyBlue == true)
        {

            //we are turning it off
            readyBlue = false;

            //update the input field status
            bluePlayerInputField.interactable = true;

            //make the local button interactable
            localBluePlayerButton.interactable = true;

            //unhighlight the button
            UnhighlightButton(readyBluePlayerButton);

        }
        else
        {
            //the else condition is that we are turning it on
            readyBlue = true;

            //update the input field status
            bluePlayerInputField.interactable = false;

            //make the local button not interactable
            localBluePlayerButton.interactable = false;

            //highlight the button
            HighlightButton(readyBluePlayerButton);

        }

    }

    //this function sets the create game interactability status
    private void SetCreateGameButtonStatus()
    {

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

        //close the window
        CloseWindow();

    }

    //this function updates the window based on whether we are a host or just a client
    private void ResolveHostStatus()
    {
        //check if we are the server
        if(this.isServer == true)
        {
            //since we are the server, we should activate the team buttons and planet count buttons,
            //and hide the static text displays
            teamsYesButton.gameObject.SetActive(true);
            teamsNoButton.gameObject.SetActive(true);
            teamStatusText.gameObject.SetActive(false);

            increasePlanetsButton.gameObject.SetActive(true);
            decreasePlanetsButton.gameObject.SetActive(true);
            planetCountToggle.gameObject.SetActive(true);
            planetValueText.gameObject.SetActive(false);

        }
        else
        {
            //we are only a client if we are not the server
            //we should disable the buttons, and only have static displays active
            teamsYesButton.gameObject.SetActive(false);
            teamsNoButton.gameObject.SetActive(false);
            teamStatusText.gameObject.SetActive(true);

            increasePlanetsButton.gameObject.SetActive(false);
            decreasePlanetsButton.gameObject.SetActive(false);
            planetCountToggle.gameObject.SetActive(false);
            planetValueText.gameObject.SetActive(true);

        }
    }

    //this function sets the green player row based on availability status
    private void SetGreenPlayerRowByAvailability()
    {
        //check if we are the server
        if (this.isServer == true)
        {
            //we are the server - this means that taken players are taken by us
            if (greenPlayerIsTaken == true)
            {
                //turn on the local button
                localGreenPlayerButton.gameObject.SetActive(true);
                localGreenPlayerButton.interactable = true;

                //highlight the local button
                HighlightButton(localGreenPlayerButton);

                //set the local flag to true
                localControlGreen = true;

                //turn off the local text
                localGreenPlayerText.gameObject.SetActive(false);

                //turn on the input field
                greenPlayerInputField.gameObject.SetActive(true);
                greenPlayerInputField.interactable = true;

                //turn off the static player name
                greenPlayerNameText.gameObject.SetActive(false);

                //turn on the player ready button
                readyGreenPlayerButton.gameObject.SetActive(true);
                readyGreenPlayerButton.interactable = true;

                //turn off the static ready text
                readyGreenPlayerText.gameObject.SetActive(false);

            }
            else
            {
                //turn on the local button
                localGreenPlayerButton.gameObject.SetActive(true);
                localGreenPlayerButton.interactable = true;

                //unhighlight the local button
                UnhighlightButton(localGreenPlayerButton);

                //set the local flag
                localControlGreen = false;

                //turn off the local text
                localGreenPlayerText.gameObject.SetActive(false);

                //turn on the input field
                greenPlayerInputField.gameObject.SetActive(true);
                greenPlayerInputField.interactable = false;

                //turn off the static player name
                greenPlayerNameText.gameObject.SetActive(false);

                //turn on the player ready button
                readyGreenPlayerButton.gameObject.SetActive(true);
                readyGreenPlayerButton.interactable = false;

                //turn off the static ready text
                readyGreenPlayerText.gameObject.SetActive(false);

            }
        }
        else
        {
            if (greenPlayerIsTaken == true)
            {
                //the player is under control by someone on the network
                //turn off the local button
                localGreenPlayerButton.gameObject.SetActive(false);

                //turn on the local text and set it to network
                localGreenPlayerText.gameObject.SetActive(true);
                localGreenPlayerText.text = "Network";

                //set the local flag
                localControlGreen = false;

                //turn off the input field
                greenPlayerInputField.gameObject.SetActive(false);

                //turn on the static player name
                greenPlayerNameText.gameObject.SetActive(true);

                //turn off the player ready button
                readyGreenPlayerButton.gameObject.SetActive(false);

                //turn on the static ready text
                readyGreenPlayerText.gameObject.SetActive(true);

            }
            else
            {
                //the player is not under control by someone on the network
                //turn on the local button
                localGreenPlayerButton.gameObject.SetActive(true);
                localGreenPlayerButton.interactable = true;

                //unhighlight the local button
                UnhighlightButton(localGreenPlayerButton);

                //set the local flag
                localControlGreen = false;

                //turn off the local text
                localGreenPlayerText.gameObject.SetActive(false);

                //turn on the input field
                greenPlayerInputField.gameObject.SetActive(true);
                greenPlayerInputField.interactable = false;

                //turn off the static player name
                greenPlayerNameText.gameObject.SetActive(false);

                //turn on the player ready button
                readyGreenPlayerButton.gameObject.SetActive(true);
                readyGreenPlayerButton.interactable = false;

                //turn off the static ready text
                readyGreenPlayerText.gameObject.SetActive(false);
            }

        }

    }

    //this function sets the red player row based on availability status
    private void SetRedPlayerRowByAvailability()
    {
        //check if we are the server
        if (this.isServer == true)
        {
            //we are the server - this means that taken players are taken by us
            if (redPlayerIsTaken == true)
            {
                //turn on the local button
                localRedPlayerButton.gameObject.SetActive(true);
                localRedPlayerButton.interactable = true;

                //highlight the local button
                HighlightButton(localRedPlayerButton);

                //set local flag
                localControlRed = true;

                //turn off the local text
                localRedPlayerText.gameObject.SetActive(false);

                //turn on the input field
                redPlayerInputField.gameObject.SetActive(true);
                redPlayerInputField.interactable = true;

                //turn off the static player name
                redPlayerNameText.gameObject.SetActive(false);

                //turn on the player ready button
                readyRedPlayerButton.gameObject.SetActive(true);
                readyRedPlayerButton.interactable = true;

                //turn off the static ready text
                readyRedPlayerText.gameObject.SetActive(false);

            }
            else
            {
                //turn on the local button
                localRedPlayerButton.gameObject.SetActive(true);
                localRedPlayerButton.interactable = true;

                //unhighlight the local button
                UnhighlightButton(localRedPlayerButton);

                //set local flag
                localControlRed = false;

                //turn off the local text
                localRedPlayerText.gameObject.SetActive(false);

                //turn on the input field
                redPlayerInputField.gameObject.SetActive(true);
                redPlayerInputField.interactable = false;

                //turn off the static player name
                redPlayerNameText.gameObject.SetActive(false);

                //turn on the player ready button
                readyRedPlayerButton.gameObject.SetActive(true);
                readyRedPlayerButton.interactable = false;

                //turn off the static ready text
                readyRedPlayerText.gameObject.SetActive(false);

            }
        }
        else
        {
            if (redPlayerIsTaken == true)
            {
                //the player is under control by someone on the network
                //turn off the local button
                localRedPlayerButton.gameObject.SetActive(false);

                //turn on the local text and set it to network
                localRedPlayerText.gameObject.SetActive(true);
                localRedPlayerText.text = "Network";

                //set local flag
                localControlRed = false;

                //turn off the input field
                redPlayerInputField.gameObject.SetActive(false);

                //turn on the static player name
                redPlayerNameText.gameObject.SetActive(true);

                //turn off the player ready button
                readyRedPlayerButton.gameObject.SetActive(false);

                //turn on the static ready text
                readyRedPlayerText.gameObject.SetActive(true);

            }
            else
            {
                //the player is not under control by someone on the network
                //turn on the local button
                localRedPlayerButton.gameObject.SetActive(true);
                localRedPlayerButton.interactable = true;

                //unhighlight the local button
                UnhighlightButton(localRedPlayerButton);

                //set local flag
                localControlRed = false;

                //turn off the local text
                localRedPlayerText.gameObject.SetActive(false);

                //turn on the input field
                redPlayerInputField.gameObject.SetActive(true);
                redPlayerInputField.interactable = false;

                //turn off the static player name
                redPlayerNameText.gameObject.SetActive(false);

                //turn on the player ready button
                readyRedPlayerButton.gameObject.SetActive(true);
                readyRedPlayerButton.interactable = false;

                //turn off the static ready text
                readyRedPlayerText.gameObject.SetActive(false);
            }

        }

    }


    //this function sets the purple player row based on availability status
    private void SetPurplePlayerRowByAvailability()
    {
        //check if we are the server
        if (this.isServer == true)
        {
            //we are the server - this means that taken players are taken by us
            if (purplePlayerIsTaken == true)
            {
                //turn on the local button
                localPurplePlayerButton.gameObject.SetActive(true);
                localPurplePlayerButton.interactable = true;

                //highlight the local button
                HighlightButton(localPurplePlayerButton);

                //set local flag
                localControlPurple = true;

                //turn off the local text
                localPurplePlayerText.gameObject.SetActive(false);

                //turn on the input field
                purplePlayerInputField.gameObject.SetActive(true);
                purplePlayerInputField.interactable = true;

                //turn off the static player name
                purplePlayerNameText.gameObject.SetActive(false);

                //turn on the player ready button
                readyPurplePlayerButton.gameObject.SetActive(true);
                readyPurplePlayerButton.interactable = true;

                //turn off the static ready text
                readyPurplePlayerText.gameObject.SetActive(false);

            }
            else
            {
                //turn on the local button
                localPurplePlayerButton.gameObject.SetActive(true);
                localPurplePlayerButton.interactable = true;

                //unhighlight the local button
                UnhighlightButton(localPurplePlayerButton);

                //set local flag
                localControlPurple = false;

                //turn off the local text
                localPurplePlayerText.gameObject.SetActive(false);

                //turn on the input field
                purplePlayerInputField.gameObject.SetActive(true);
                purplePlayerInputField.interactable = false;

                //turn off the static player name
                purplePlayerNameText.gameObject.SetActive(false);

                //turn on the player ready button
                readyPurplePlayerButton.gameObject.SetActive(true);
                readyPurplePlayerButton.interactable = false;

                //turn off the static ready text
                readyPurplePlayerText.gameObject.SetActive(false);

            }
        }
        else
        {
            if (purplePlayerIsTaken == true)
            {
                //the player is under control by someone on the network
                //turn off the local button
                localPurplePlayerButton.gameObject.SetActive(false);

                //turn on the local text and set it to network
                localPurplePlayerText.gameObject.SetActive(true);
                localPurplePlayerText.text = "Network";

                //set local flag
                localControlPurple = false;

                //turn off the input field
                purplePlayerInputField.gameObject.SetActive(false);

                //turn on the static player name
                purplePlayerNameText.gameObject.SetActive(true);

                //turn off the player ready button
                readyPurplePlayerButton.gameObject.SetActive(false);

                //turn on the static ready text
                readyPurplePlayerText.gameObject.SetActive(true);

            }
            else
            {
                //the player is not under control by someone on the network
                //turn on the local button
                localPurplePlayerButton.gameObject.SetActive(true);
                localPurplePlayerButton.interactable = true;

                //unhighlight the local button
                UnhighlightButton(localPurplePlayerButton);

                //set local flag
                localControlPurple = false;

                //turn off the local text
                localPurplePlayerText.gameObject.SetActive(false);

                //turn on the input field
                purplePlayerInputField.gameObject.SetActive(true);
                purplePlayerInputField.interactable = false;

                //turn off the static player name
                purplePlayerNameText.gameObject.SetActive(false);

                //turn on the player ready button
                readyPurplePlayerButton.gameObject.SetActive(true);
                readyPurplePlayerButton.interactable = false;

                //turn off the static ready text
                readyPurplePlayerText.gameObject.SetActive(false);
            }

        }

    }


    //this function sets the blue player row based on availability status
    private void SetBluePlayerRowByAvailability()
    {
        //check if we are the server
        if (this.isServer == true)
        {
            //we are the server - this means that taken players are taken by us
            if (bluePlayerIsTaken == true)
            {
                //turn on the local button
                localBluePlayerButton.gameObject.SetActive(true);
                localBluePlayerButton.interactable = true;

                //highlight the local button
                HighlightButton(localBluePlayerButton);

                //set local flag
                localControlBlue = true;

                //turn off the local text
                localBluePlayerText.gameObject.SetActive(false);

                //turn on the input field
                bluePlayerInputField.gameObject.SetActive(true);
                bluePlayerInputField.interactable = true;

                //turn off the static player name
                bluePlayerNameText.gameObject.SetActive(false);

                //turn on the player ready button
                readyBluePlayerButton.gameObject.SetActive(true);
                readyBluePlayerButton.interactable = true;

                //turn off the static ready text
                readyBluePlayerText.gameObject.SetActive(false);

            }
            else
            {
                //turn on the local button
                localBluePlayerButton.gameObject.SetActive(true);
                localBluePlayerButton.interactable = true;

                //unhighlight the local button
                UnhighlightButton(localBluePlayerButton);

                //set local flag
                localControlBlue = false;

                //turn off the local text
                localBluePlayerText.gameObject.SetActive(false);

                //turn on the input field
                bluePlayerInputField.gameObject.SetActive(true);
                bluePlayerInputField.interactable = false;

                //turn off the static player name
                bluePlayerNameText.gameObject.SetActive(false);

                //turn on the player ready button
                readyBluePlayerButton.gameObject.SetActive(true);
                readyBluePlayerButton.interactable = false;

                //turn off the static ready text
                readyBluePlayerText.gameObject.SetActive(false);

            }
        }
        else
        {
            if (bluePlayerIsTaken == true)
            {
                //the player is under control by someone on the network
                //turn off the local button
                localBluePlayerButton.gameObject.SetActive(false);

                //turn on the local text and set it to network
                localBluePlayerText.gameObject.SetActive(true);
                localBluePlayerText.text = "Network";

                //set local flag
                localControlBlue = false;

                //turn off the input field
                bluePlayerInputField.gameObject.SetActive(false);

                //turn on the static player name
                bluePlayerNameText.gameObject.SetActive(true);

                //turn off the player ready button
                readyBluePlayerButton.gameObject.SetActive(false);

                //turn on the static ready text
                readyBluePlayerText.gameObject.SetActive(true);

            }
            else
            {
                //the player is not under control by someone on the network
                //turn on the local button
                localBluePlayerButton.gameObject.SetActive(true);
                localBluePlayerButton.interactable = true;

                //unhighlight the local button
                UnhighlightButton(localBluePlayerButton);

                //set local flag
                localControlBlue = false;

                //turn off the local text
                localBluePlayerText.gameObject.SetActive(false);

                //turn on the input field
                bluePlayerInputField.gameObject.SetActive(true);
                bluePlayerInputField.interactable = false;

                //turn off the static player name
                bluePlayerNameText.gameObject.SetActive(false);

                //turn on the player ready button
                readyBluePlayerButton.gameObject.SetActive(true);
                readyBluePlayerButton.interactable = false;

                //turn off the static ready text
                readyBluePlayerText.gameObject.SetActive(false);
            }

        }

    }

    //this function updates the game name
    private void GetGameName()
    {
        gameName = networkManager.GetComponentInChildren<NetworkLobbyLAN>().gameName;
    }

    //this function updates the team status
    private void GetTeamStatus()
    {
        teamsEnabled = networkManager.GetComponentInChildren<NetworkLobbyLAN>().teamsEnabled;
    }

    //this function updates the victory planets
    private void GetVictoryPlanets()
    {
        planetCount = networkManager.GetComponentInChildren<NetworkLobbyLAN>().victoryPlanets;
    }

    //this function updates the game year
    private void GetGameYear()
    {
        gameYear = networkManager.GetComponentInChildren<NetworkLobbyLAN>().gameYear;
    }

    //this function updates the green player name
    private void GetGreenPlayerName()
    {
        greenPlayerName = networkManager.GetComponentInChildren<NetworkLobbyLAN>().greenPlayerName;
    }

    //this function updates the red player name
    private void GetRedPlayerName()
    {
        redPlayerName = networkManager.GetComponentInChildren<NetworkLobbyLAN>().redPlayerName;
    }

    //this function updates the purple player name
    private void GetPurplePlayerName()
    {
        purplePlayerName = networkManager.GetComponentInChildren<NetworkLobbyLAN>().purplePlayerName;
    }

    //this function updates the blue player name
    private void GetBluePlayerName()
    {
        bluePlayerName = networkManager.GetComponentInChildren<NetworkLobbyLAN>().bluePlayerName;
    }

    //this function updates the green player planets
    private void GetGreenPlayerPlanets()
    {
        greenPlanetsText.text = networkManager.GetComponentInChildren<NetworkLobbyLAN>().greenPlayerPlanets.ToString();
    }

    //this function updates the red player planets
    private void GetRedPlayerPlanets()
    {
        redPlanetsText.text = networkManager.GetComponentInChildren<NetworkLobbyLAN>().redPlayerPlanets.ToString();
    }

    //this function updates the purple player planets
    private void GetPurplePlayerPlanets()
    {
        purplePlanetsText.text = networkManager.GetComponentInChildren<NetworkLobbyLAN>().purplePlayerPlanets.ToString();
    }

    //this function updates the blue player planets
    private void GetBluePlayerPlanets()
    {
        bluePlanetsText.text = networkManager.GetComponentInChildren<NetworkLobbyLAN>().bluePlayerPlanets.ToString();
    }

    //this function updates the green player ready status
    private void GetGreenPlayerReadyStatus()
    {
        readyGreen = networkManager.GetComponentInChildren<NetworkLobbyLAN>().greenPlayerReady;
    }

    //this function updates the red player ready status
    private void GetRedPlayerReadyStatus()
    {
        readyRed = networkManager.GetComponentInChildren<NetworkLobbyLAN>().redPlayerReady;
    }

    //this function updates the purple player ready status
    private void GetPurplePlayerReadyStatus()
    {
        readyPurple = networkManager.GetComponentInChildren<NetworkLobbyLAN>().purplePlayerReady;
    }

    //this function updates the blue player ready status
    private void GetBluePlayerReadyStatus()
    {
        readyBlue = networkManager.GetComponentInChildren<NetworkLobbyLAN>().bluePlayerReady;
    }

    //this function gets the green connection
    private void GetGreenPlayerConnection()
    {
        greenPlayerConnection = networkManager.GetComponentInChildren<NetworkLobbyLAN>().greenPlayerConnection;
    }

    //this function gets the red connection
    private void GetRedPlayerConnection()
    {
        redPlayerConnection = networkManager.GetComponentInChildren<NetworkLobbyLAN>().redPlayerConnection;
    }

    //this function gets the purple connection
    private void GetPurplePlayerConnection()
    {
        purplePlayerConnection = networkManager.GetComponentInChildren<NetworkLobbyLAN>().purplePlayerConnection;
    }

    //this function gets the blue connection
    private void GetBluePlayerConnection()
    {
        bluePlayerConnection = networkManager.GetComponentInChildren<NetworkLobbyLAN>().bluePlayerConnection;
    }

    //this function resolves OnDestroy
    private void OnDestroy()
    {

        RemoveAllListeners();

    }

    //this function removes all event listeners
    private void RemoveAllListeners()
    {


        //remove a listener for the trigger button to open the window
        GameListItem.OnJoinLANGame.RemoveListener(joinLANGameAction);

        if(uiManager != null)
        {
            //remove a listener for creating the LAN game as host or server
            uiManager.GetComponent<NewLANGameWindow>().OnCreateNewLANGame.RemoveListener(createLANGameAction);
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

        if (exitLobbyButton != null)
        {

            //remove a listener for clicking the cancel button
            exitLobbyButton.onClick.RemoveListener(CloseWindow);

        }

        if (localGreenPlayerButton != null)
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

        //remove listener for game name update
        NetworkLobbyLAN.OnUpdateGameName.RemoveListener(GetGameName);

        //remove listener for team status update
        NetworkLobbyLAN.OnUpdateTeamsEnabled.RemoveListener(GetTeamStatus);

        //remove listener for team victory planets update
        NetworkLobbyLAN.OnUpdateVictoryPlanets.RemoveListener(GetVictoryPlanets);

        //remove listener for game year update
        NetworkLobbyLAN.OnUpdateGameYear.RemoveListener(GetGameYear);

        //remove listener for green name update
        NetworkLobbyLAN.OnUpdateGreenPlayerName.RemoveListener(GetGreenPlayerName);

        //remove listener for red name update
        NetworkLobbyLAN.OnUpdateRedPlayerName.RemoveListener(GetRedPlayerName);

        //remove listener for purple name update
        NetworkLobbyLAN.OnUpdatePurplePlayerName.RemoveListener(GetPurplePlayerName);

        //remove listener for blue name update
        NetworkLobbyLAN.OnUpdateBluePlayerName.RemoveListener(GetBluePlayerName);

        //remove listener for green planets update
        NetworkLobbyLAN.OnUpdateGreenPlayerPlanets.RemoveListener(GetGreenPlayerPlanets);

        //remove listener for red planets update
        NetworkLobbyLAN.OnUpdateRedPlayerPlanets.RemoveListener(GetRedPlayerPlanets);

        //remove listener for purple planets update
        NetworkLobbyLAN.OnUpdatePurplePlayerPlanets.RemoveListener(GetPurplePlayerPlanets);

        //remove listener for blue planets update
        NetworkLobbyLAN.OnUpdateBluePlayerPlanets.RemoveListener(GetBluePlayerPlanets);

        //remove listener for green ready update
        NetworkLobbyLAN.OnUpdateGreenPlayerReady.RemoveListener(GetGreenPlayerReadyStatus);

        //remove listener for red ready update
        NetworkLobbyLAN.OnUpdateRedPlayerReady.RemoveListener(GetRedPlayerReadyStatus);

        //remove listener for purple ready update
        NetworkLobbyLAN.OnUpdatePurplePlayerReady.RemoveListener(GetPurplePlayerReadyStatus);

        //remove listener for blue ready update
        NetworkLobbyLAN.OnUpdateBluePlayerReady.RemoveListener(GetBluePlayerReadyStatus);

        //remove listener for green connection update
        NetworkLobbyLAN.OnUpdateGreenPlayerConnection.RemoveListener(GetGreenPlayerConnection);

        //remove listener for red connection update
        NetworkLobbyLAN.OnUpdateRedPlayerConnection.RemoveListener(GetRedPlayerConnection);

        //remove listener for purple connection update
        NetworkLobbyLAN.OnUpdatePurplePlayerConnection.RemoveListener(GetPurplePlayerConnection);

        //remove listener for blue connection update
        NetworkLobbyLAN.OnUpdateBluePlayerConnection.RemoveListener(GetBluePlayerConnection);

        if(readyGreenPlayerButton != null)
        {
            //remove listener for ready buttons
            readyGreenPlayerButton.onClick.RemoveListener(ResolveReadyGreenButtonClick);
        }

        if (readyRedPlayerButton != null)
        {
            //remove listener for ready buttons
            readyRedPlayerButton.onClick.RemoveListener(ResolveReadyRedButtonClick);
        }

        if (readyPurplePlayerButton != null)
        {
            //remove listener for ready buttons
            readyPurplePlayerButton.onClick.RemoveListener(ResolveReadyPurpleButtonClick);
        }

        if (readyBluePlayerButton != null)
        {
            //remove listener for ready buttons
            readyBluePlayerButton.onClick.RemoveListener(ResolveReadyBlueButtonClick);
        }

    }
}
