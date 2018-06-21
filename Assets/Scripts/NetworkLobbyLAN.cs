using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;

public class NetworkLobbyLAN : NetworkBehaviour {

    //managers
    private GameObject uiManager;
    private GameObject networkManager;

    //this holds the game name
    private string _gameName;
    public string gameName {
        get
        {
            return _gameName;
        }
        private set
        {
            if (value == _gameName)
            {
                return;
            }
            else
            {
                _gameName = value;
                if (this.isServer == true)
                {
                    RpcUpdateGameName(_gameName);
                }
                OnUpdateGameName.Invoke();
            }
        }
    }

    //these hold which player connection is controlling each player
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
                Debug.Log("GreenPlayerConnectionSet");
                if (this.isServer == true)
                {
                    Debug.Log("GreenPlayerConnectionSetOnServer");
                    if (_greenPlayerConnection != null)
                    {
                        Debug.Log("GreenPlayerConnectionSetOnServerSendRPC");
                        RpcUpdateGreenPlayerConnection(_greenPlayerConnection.gameObject);
                    }
                    else
                    {
                        RpcUpdateGreenPlayerConnection(null);
                    }
                }
                OnUpdateGreenPlayerConnection.Invoke();
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
                if (this.isServer == true)
                {
                    if (_redPlayerConnection != null)
                    {
                        RpcUpdateRedPlayerConnection(_redPlayerConnection.gameObject);
                    }
                    else
                    {
                        RpcUpdateRedPlayerConnection(null);
                    }
                }
                OnUpdateRedPlayerConnection.Invoke();
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
                if (this.isServer == true)
                {
                    if (_purplePlayerConnection != null)
                    {
                        RpcUpdatePurplePlayerConnection(_purplePlayerConnection.gameObject);
                    }
                    else
                    {
                        RpcUpdatePurplePlayerConnection(null);
                    }
                }
                OnUpdatePurplePlayerConnection.Invoke();
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
                if (this.isServer == true)
                {
                    if (_bluePlayerConnection != null)
                    {
                        RpcUpdateBluePlayerConnection(_bluePlayerConnection.gameObject);
                    }
                    else
                    {
                        RpcUpdateBluePlayerConnection(null);
                    }
                }
                OnUpdateBluePlayerConnection.Invoke();
            }
        }
    }

    //variable to hold the team status
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
                if (this.isServer == true)
                {
                    RpcUpdateTeamsEnabled(_teamsEnabled);
                }
                OnUpdateTeamsEnabled.Invoke();
            }
        }
    }

    //variable to hold the victory planets
    private int _victoryPlanets;
    public int victoryPlanets
    {
        get
        {
            return _victoryPlanets;
        }
        private set
        {
            if (value == _victoryPlanets)
            {
                return;
            }
            else
            {
                _victoryPlanets = value;
                if (this.isServer == true)
                {
                    RpcUpdateVictoryPlanets(_victoryPlanets);
                }
                OnUpdateVictoryPlanets.Invoke();
            }
        }
    }

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
                if (this.isServer == true)
                {
                    RpcUpdateGameYear(_gameYear);
                }
                OnUpdateGameYear.Invoke();
            }
        }
    }

    //variables to hold the player names
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
                if (this.isServer == true)
                {
                    RpcUpdateGreenPlayerName(_greenPlayerName);
                }
                OnUpdateGreenPlayerName.Invoke();
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
                if (this.isServer == true)
                {
                    RpcUpdateRedPlayerName(_redPlayerName);
                }
                OnUpdateRedPlayerName.Invoke();
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
                if (this.isServer == true)
                {
                    RpcUpdatePurplePlayerName(_purplePlayerName);
                }
                OnUpdatePurplePlayerName.Invoke();
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
                if (this.isServer == true)
                {
                    RpcUpdateBluePlayerName(_bluePlayerName);
                }
                OnUpdateBluePlayerName.Invoke();
            }
        }
    }

    //variables to hold the number of planets each player has
    private int _greenPlayerPlanets;
    public int greenPlayerPlanets
    {
        get
        {
            return _greenPlayerPlanets;
        }
        private set
        {
            if (value == _greenPlayerPlanets)
            {
                return;
            }
            else
            {
                _greenPlayerPlanets = value;
                if (this.isServer == true)
                {
                    RpcUpdateGreenPlayerPlanets(_greenPlayerPlanets);
                }
                OnUpdateGreenPlayerPlanets.Invoke();
            }
        }
    }

    private int _redPlayerPlanets;
    public int redPlayerPlanets
    {
        get
        {
            return _redPlayerPlanets;
        }
        private set
        {
            if (value == _redPlayerPlanets)
            {
                return;
            }
            else
            {
                _redPlayerPlanets = value;
                if (this.isServer == true)
                {
                    RpcUpdateRedPlayerPlanets(_redPlayerPlanets);
                }
                OnUpdateRedPlayerPlanets.Invoke();
            }
        }
    }

    private int _purplePlayerPlanets;
    public int purplePlayerPlanets
    {
        get
        {
            return _purplePlayerPlanets;
        }
        private set
        {
            if (value == _purplePlayerPlanets)
            {
                return;
            }
            else
            {
                _purplePlayerPlanets = value;
                if (this.isServer == true)
                {
                    RpcUpdatePurplePlayerPlanets(_purplePlayerPlanets);
                }
                OnUpdatePurplePlayerPlanets.Invoke();
            }
        }
    }

    private int _bluePlayerPlanets;
    public int bluePlayerPlanets
    {
        get
        {
            return _bluePlayerPlanets;
        }
        private set
        {
            if (value == _bluePlayerPlanets)
            {
                return;
            }
            else
            {
                _bluePlayerPlanets = value;
                if (this.isServer == true)
                {
                    RpcUpdateBluePlayerPlanets(_bluePlayerPlanets);
                }
                OnUpdateBluePlayerPlanets.Invoke();
            }
        }
    }

    //variables to track whether a given player color is ready
    private bool _greenPlayerReady;
    public bool greenPlayerReady
    {
        get
        {
            return _greenPlayerReady;
        }
        private set
        {
            if (value == _greenPlayerReady)
            {
                return;
            }
            else
            {
                _greenPlayerReady = value;
                if (this.isServer == true)
                {
                    RpcUpdateGreenPlayerReady(_greenPlayerReady);
                }
                OnUpdateGreenPlayerReady.Invoke();
            }
        }
    }

    private bool _redPlayerReady;
    public bool redPlayerReady
    {
        get
        {
            return _redPlayerReady;
        }
        private set
        {
            if (value == _redPlayerReady)
            {
                return;
            }
            else
            {
                _redPlayerReady = value;
                if (this.isServer == true)
                {
                    RpcUpdateRedPlayerReady(_redPlayerReady);
                }
                OnUpdateRedPlayerReady.Invoke();
            }
        }
    }

    private bool _purplePlayerReady;
    public bool purplePlayerReady
    {
        get
        {
            return _purplePlayerReady;
        }
        private set
        {
            if (value == _purplePlayerReady)
            {
                return;
            }
            else
            {
                _purplePlayerReady = value;
                if (this.isServer == true)
                {
                    RpcUpdatePurplePlayerReady(_purplePlayerReady);
                }
                OnUpdatePurplePlayerReady.Invoke();
            }
        }
    }

    private bool _bluePlayerReady;
    public bool bluePlayerReady
    {
        get
        {
            return _bluePlayerReady;
        }
        private set
        {
            if (value == _bluePlayerReady)
            {
                return;
            }
            else
            {
                _bluePlayerReady = value;
                if (this.isServer == true)
                {
                    RpcUpdateBluePlayerReady(_bluePlayerReady);
                }
                OnUpdateBluePlayerReady.Invoke();
            }
        }
    }

    //variables to track whether a given player color is alive
    private bool _greenPlayerAlive;
    public bool greenPlayerAlive
    {
        get
        {
            return _greenPlayerAlive;
        }
        private set
        {
            if (value == _greenPlayerAlive)
            {
                return;
            }
            else
            {
                _greenPlayerAlive = value;
                if (this.isServer == true)
                {
                    RpcUpdateGreenPlayerAlive(_greenPlayerAlive);
                }
                OnUpdateGreenPlayerAlive.Invoke();
            }
        }
    }

    private bool _redlayerAlive;
    public bool redPlayerAlive
    {
        get
        {
            return _redlayerAlive;
        }
        private set
        {
            if (value == _redlayerAlive)
            {
                return;
            }
            else
            {
                _redlayerAlive = value;
                if (this.isServer == true)
                {
                    RpcUpdateRedPlayerAlive(_redlayerAlive);
                }
                OnUpdateRedPlayerAlive.Invoke();
            }
        }
    }

    private bool _purplePlayerAlive;
    public bool purplePlayerAlive
    {
        get
        {
            return _purplePlayerAlive;
        }
        private set
        {
            if (value == _purplePlayerAlive)
            {
                return;
            }
            else
            {
                _purplePlayerAlive = value;
                if (this.isServer == true)
                {
                    RpcUpdatePurplePlayerAlive(_purplePlayerAlive);
                }
                OnUpdatePurplePlayerAlive.Invoke();
            }
        }
    }

    private bool _bluePlayerAlive;
    public bool bluePlayerAlive
    {
        get
        {
            return _bluePlayerAlive;
        }
        private set
        {
            if (value == _bluePlayerAlive)
            {
                return;
            }
            else
            {
                _bluePlayerAlive = value;
                if (this.isServer == true)
                {
                    RpcUpdateBluePlayerAlive(_bluePlayerAlive);
                }
                OnUpdateBluePlayerAlive.Invoke();
            }
        }
    }



    //this flag is for the 1 frame delay
    private bool delayFrame = false;

    //this flag is for getting the startup game state as a host
    private bool isNewGame = false;

    //this is the local player connection on this machine
    public PlayerConnection localPlayerConnection { get; private set; }

    public static ConnectionEvent OnRequestRPCUpdate = new ConnectionEvent();

    public static ConnectionInfoEvent OnStartAsServer = new ConnectionInfoEvent();

    //class for passing connections
    public class ConnectionEvent : UnityEvent<PlayerConnection, NetworkInstanceId> { };

    //class for passing LAN Connection Info
    public class ConnectionInfoEvent : UnityEvent<LANConnectionInfo> { };


    //these events fire off when the RPCs have updated values, so the client can update the UI
    public static UnityEvent OnUpdateGameName = new UnityEvent();
    public static UnityEvent OnUpdateTeamsEnabled = new UnityEvent();
    public static UnityEvent OnUpdateVictoryPlanets = new UnityEvent();
    public static UnityEvent OnUpdateGameYear = new UnityEvent();
    public static UnityEvent OnUpdateGreenPlayerName = new UnityEvent();
    public static UnityEvent OnUpdateRedPlayerName = new UnityEvent();
    public static UnityEvent OnUpdatePurplePlayerName = new UnityEvent();
    public static UnityEvent OnUpdateBluePlayerName = new UnityEvent();
    public static UnityEvent OnUpdateGreenPlayerPlanets = new UnityEvent();
    public static UnityEvent OnUpdateRedPlayerPlanets = new UnityEvent();
    public static UnityEvent OnUpdatePurplePlayerPlanets = new UnityEvent();
    public static UnityEvent OnUpdateBluePlayerPlanets = new UnityEvent();
    public static UnityEvent OnUpdateGreenPlayerReady = new UnityEvent();
    public static UnityEvent OnUpdateRedPlayerReady = new UnityEvent();
    public static UnityEvent OnUpdatePurplePlayerReady = new UnityEvent();
    public static UnityEvent OnUpdateBluePlayerReady = new UnityEvent();
    public static UnityEvent OnUpdateGreenPlayerConnection = new UnityEvent();
    public static UnityEvent OnUpdateRedPlayerConnection = new UnityEvent();
    public static UnityEvent OnUpdatePurplePlayerConnection = new UnityEvent();
    public static UnityEvent OnUpdateBluePlayerConnection = new UnityEvent();
    public static UnityEvent OnUpdateGreenPlayerAlive = new UnityEvent();
    public static UnityEvent OnUpdateRedPlayerAlive = new UnityEvent();
    public static UnityEvent OnUpdatePurplePlayerAlive = new UnityEvent();
    public static UnityEvent OnUpdateBluePlayerAlive = new UnityEvent();

    public static UnityEvent OnFinishedInitialGameSetup = new UnityEvent();

    //unityActions
    private UnityAction<PlayerConnection, NetworkInstanceId> playerConnectionUpdateRPCAction;

    private UnityAction<PlayerConnection, NetworkInstanceId, int> playerConnectionUpdateVictoryPlanetCountAction;
    private UnityAction<PlayerConnection, NetworkInstanceId, bool> playerConnectionUpdateTeamStatusAction;

    private UnityAction<PlayerConnection, NetworkInstanceId> playerConnectionRequestControlGreenAction;
    private UnityAction<PlayerConnection, NetworkInstanceId> playerConnectionRelinquishControlGreenAction;
    private UnityAction<PlayerConnection, NetworkInstanceId, string> playerConnectionRenameGreenAction;
    private UnityAction<PlayerConnection, NetworkInstanceId> playerConnectionSetGreenToReadyAction;
    private UnityAction<PlayerConnection, NetworkInstanceId> playerConnectionSetGreenToNotReadyAction;

    private UnityAction<PlayerConnection, NetworkInstanceId> playerConnectionRequestControlRedAction;
    private UnityAction<PlayerConnection, NetworkInstanceId> playerConnectionRelinquishControlRedAction;
    private UnityAction<PlayerConnection, NetworkInstanceId, string> playerConnectionRenameRedAction;
    private UnityAction<PlayerConnection, NetworkInstanceId> playerConnectionSetRedToReadyAction;
    private UnityAction<PlayerConnection, NetworkInstanceId> playerConnectionSetRedToNotReadyAction;

    private UnityAction<PlayerConnection, NetworkInstanceId> playerConnectionRequestControlPurpleAction;
    private UnityAction<PlayerConnection, NetworkInstanceId> playerConnectionRelinquishControlPurpleAction;
    private UnityAction<PlayerConnection, NetworkInstanceId, string> playerConnectionRenamePurpleAction;
    private UnityAction<PlayerConnection, NetworkInstanceId> playerConnectionSetPurpleToReadyAction;
    private UnityAction<PlayerConnection, NetworkInstanceId> playerConnectionSetPurpleToNotReadyAction;

    private UnityAction<PlayerConnection, NetworkInstanceId> playerConnectionRequestControlBlueAction;
    private UnityAction<PlayerConnection, NetworkInstanceId> playerConnectionRelinquishControlBlueAction;
    private UnityAction<PlayerConnection, NetworkInstanceId, string> playerConnectionRenameBlueAction;
    private UnityAction<PlayerConnection, NetworkInstanceId> playerConnectionSetBlueToReadyAction;
    private UnityAction<PlayerConnection, NetworkInstanceId> playerConnectionSetBlueToNotReadyAction;

    private UnityAction<PlayerConnection, NetworkInstanceId> playerConnectionServerPlayerStartAction;


    // Use this for initialization
    private void Start () {

        Debug.Log("Start");

        //get the managers
        uiManager = GameObject.FindGameObjectWithTag("UIManager");
        networkManager = GameObject.FindGameObjectWithTag("NetworkManager");

        //set the parent
        this.transform.SetParent(networkManager.transform);

        //set the name
        this.name = "NetworkLobbyLAN";

        //set the 1 frame delay
        //need to delay running this by 1 frame because on the server, the lobby LAN is created
        //before the player connections, so it comes over first.  I need the player connections to 
        //already exist when this start runs
        delayFrame = true;

    }

    //this function is called once per frame
    private void Update()
    {
        if(delayFrame == true)
        {
            Debug.Log("FirstUpdate");

            //set the local player connection
            SetLocalPlayerConnection();

            //set actions
            SetActions();

            //add listeners
            AddEventListeners();

            //check if we are the server
            if (isServer == true)
            {
                //TODO: check if this is a new game or not
                //get the data from the new game setup window
                //GetDataFromNewGameSetup();
                isNewGame = true;
            }
            else
            {
                //get the initial data
                OnRequestRPCUpdate.Invoke(localPlayerConnection, this.GetComponent<NetworkIdentity>().netId);
            }

            delayFrame = false;
        }

    }

    //this function sets unityActions
    private void SetActions()
    {
        playerConnectionUpdateRPCAction = (playerConnection, netId) => {

            //check if the netID matches this object
            if (netId == this.GetComponent<NetworkIdentity>().netId)
            {
                Debug.Log("server update RPCs");
                UpdateAllRPCs();
            }

        };

        playerConnectionUpdateVictoryPlanetCountAction = (playerConnection, netId, newPlanetCount) => {

            ResolveRequestUpdateVictoryPlanetCount(playerConnection, netId, newPlanetCount);

        };

        playerConnectionUpdateTeamStatusAction = (playerConnection, netId, newTeamStatus) => {

            ResolveRequestUpdateTeamStatus(playerConnection, netId, newTeamStatus);

        };



        playerConnectionRequestControlGreenAction = (playerConnection, netId) => {

            ResolveRequestLocalControlGreen(playerConnection, netId);
            
        };

        playerConnectionRelinquishControlGreenAction = (playerConnection, netId) => {

            ResolveRelinquishLocalControlGreen(playerConnection, netId);

        };

        playerConnectionRenameGreenAction = (playerConnection, netId, newName) => {

            ResolveUpdateGreenPlayerName(playerConnection, netId, newName);

        };

        playerConnectionSetGreenToReadyAction = (playerConnection, netId) => {

            ResolveSetGreenToReady(playerConnection, netId);

        };

        playerConnectionSetGreenToNotReadyAction = (playerConnection, netId) => {

            ResolveSetGreenToNotReady(playerConnection, netId);

        };



        playerConnectionRequestControlRedAction = (playerConnection, netId) => {

            ResolveRequestLocalControlRed(playerConnection, netId);

        };

        playerConnectionRelinquishControlRedAction = (playerConnection, netId) => {

            ResolveRelinquishLocalControlRed(playerConnection, netId);

        };

        playerConnectionRenameRedAction = (playerConnection, netId, newName) => {

            ResolveUpdateRedPlayerName(playerConnection, netId, newName);

        };

        playerConnectionSetRedToReadyAction = (playerConnection, netId) => {

            ResolveSetRedToReady(playerConnection, netId);

        };

        playerConnectionSetRedToNotReadyAction = (playerConnection, netId) => {

            ResolveSetRedToNotReady(playerConnection, netId);

        };


        playerConnectionRequestControlPurpleAction = (playerConnection, netId) => {

            ResolveRequestLocalControlPurple(playerConnection, netId);

        };

        playerConnectionRelinquishControlPurpleAction = (playerConnection, netId) => {

            ResolveRelinquishLocalControlPurple(playerConnection, netId);

        };

        playerConnectionRenamePurpleAction = (playerConnection, netId, newName) => {

            ResolveUpdatePurplePlayerName(playerConnection, netId, newName);

        };

        playerConnectionSetPurpleToReadyAction = (playerConnection, netId) => {

            ResolveSetPurpleToReady(playerConnection, netId);

        };

        playerConnectionSetPurpleToNotReadyAction = (playerConnection, netId) => {

            ResolveSetPurpleToNotReady(playerConnection, netId);

        };


        playerConnectionRequestControlBlueAction = (playerConnection, netId) => {

            ResolveRequestLocalControlBlue(playerConnection, netId);

        };

        playerConnectionRelinquishControlBlueAction = (playerConnection, netId) => {

            ResolveRelinquishLocalControlBlue(playerConnection, netId);

        };

        playerConnectionRenameBlueAction = (playerConnection, netId, newName) => {

            ResolveUpdateBluePlayerName(playerConnection, netId, newName);

        };

        playerConnectionSetBlueToReadyAction = (playerConnection, netId) => {

            ResolveSetBlueToReady(playerConnection, netId);

        };

        playerConnectionSetBlueToNotReadyAction = (playerConnection, netId) => {

            ResolveSetBlueToNotReady(playerConnection, netId);

        };

        playerConnectionServerPlayerStartAction = (playerConnection, netId) => {

            ResolveServerPlayerStart(playerConnection, netId);

        };

    }


    //this function adds event listeners
    private void AddEventListeners()
    {
        //add listener for requesting RPC update
        PlayerConnection.OnRequestRPCUpdate.AddListener(playerConnectionUpdateRPCAction);

        //add listener for updating victory planet count
        PlayerConnection.OnUpdateVictoryPlanetCount.AddListener(playerConnectionUpdateVictoryPlanetCountAction);

        //add listener for updating the team status
        PlayerConnection.OnUpdateTeamStatus.AddListener(playerConnectionUpdateTeamStatusAction);


        //add listener for player requesting control of green
        PlayerConnection.OnRequestLocalControlGreen.AddListener(playerConnectionRequestControlGreenAction);

        //add listener for player relinquishing control of green
        PlayerConnection.OnRelinquishLocalControlGreen.AddListener(playerConnectionRelinquishControlGreenAction);
        
        //add listener for player setting green to ready
        PlayerConnection.OnSetGreenPlayerToReady.AddListener(playerConnectionSetGreenToReadyAction);

        //add listener for player setting green to not ready
        PlayerConnection.OnSetGreenPlayerToNotReady.AddListener(playerConnectionSetGreenToNotReadyAction);

        //add listener for player updating green name
        PlayerConnection.OnUpdateGreenPlayerName.AddListener(playerConnectionRenameGreenAction);


        //add listener for player requesting control of red
        PlayerConnection.OnRequestLocalControlRed.AddListener(playerConnectionRequestControlRedAction);

        //add listener for player relinquishing control of red
        PlayerConnection.OnRelinquishLocalControlRed.AddListener(playerConnectionRelinquishControlRedAction);
  
        //add listener for player setting red to ready
        PlayerConnection.OnSetRedPlayerToReady.AddListener(playerConnectionSetRedToReadyAction);

        //add listener for player setting red to not ready
        PlayerConnection.OnSetRedPlayerToNotReady.AddListener(playerConnectionSetRedToNotReadyAction);

        //add listener for player updating red name
        PlayerConnection.OnUpdateRedPlayerName.AddListener(playerConnectionRenameRedAction);


        //add listener for player requesting control of purple
        PlayerConnection.OnRequestLocalControlPurple.AddListener(playerConnectionRequestControlPurpleAction);

        //add listener for player relinquishing control of purple
        PlayerConnection.OnRelinquishLocalControlPurple.AddListener(playerConnectionRelinquishControlPurpleAction);

        //add listener for player setting purple to ready
        PlayerConnection.OnSetPurplePlayerToReady.AddListener(playerConnectionSetPurpleToReadyAction);

        //add listener for player setting purple to not ready
        PlayerConnection.OnSetPurplePlayerToNotReady.AddListener(playerConnectionSetPurpleToNotReadyAction);

        //add listener for player updating purple name
        PlayerConnection.OnUpdatePurplePlayerName.AddListener(playerConnectionRenamePurpleAction);


        //add listener for player requesting control of blue
        PlayerConnection.OnRequestLocalControlBlue.AddListener(playerConnectionRequestControlBlueAction);

        //add listener for player relinquishing control of blue
        PlayerConnection.OnRelinquishLocalControlBlue.AddListener(playerConnectionRelinquishControlBlueAction);

        //add listener for player setting blue to ready
        PlayerConnection.OnSetBluePlayerToReady.AddListener(playerConnectionSetBlueToReadyAction);

        //add listener for player setting blue to not ready
        PlayerConnection.OnSetBluePlayerToNotReady.AddListener(playerConnectionSetBlueToNotReadyAction);

        //add listener for player updating blue name
        PlayerConnection.OnUpdateBluePlayerName.AddListener(playerConnectionRenameBlueAction);

        //add listener for server player starting
        PlayerConnection.OnServerPlayerStart.AddListener(playerConnectionServerPlayerStartAction);

    }

    //this function resolves a server player starting
    private void ResolveServerPlayerStart(PlayerConnection requestingPlayerConnection, NetworkInstanceId requestingNetId)
    {
        SetLocalPlayerConnection();
        GetDataFromNewGameSetup();
    }

    //this function overrides the OnStartServer
    public override void OnStartServer()
    {
        //call the base function
        base.OnStartServer();

        if (this.isServer == true)
        {
            Debug.Log("OnStartServer Override");
            //networkManager = GameObject.FindGameObjectWithTag("NetworkManager");
            //uiManager = GameObject.FindGameObjectWithTag("UIManager");
            //SetLocalPlayerConnection();
            //GetDataFromNewGameSetup();

            //invoke the event that we're starting the server
            OnStartAsServer.Invoke(GetConnectionInfoForCurrentLobby());
        }

    }

    //this function returns a connectionInfo for the current lobby setup
    private LANConnectionInfo GetConnectionInfoForCurrentLobby()
    {
        //define the taken states
        bool greenPlayerTaken;
        bool redPlayerTaken;
        bool purplePlayerTaken;
        bool bluePlayerTaken;

        if (this.greenPlayerConnection == null)
        {
            greenPlayerTaken = false;
        }
        else
        {
            greenPlayerTaken = true;

        }

        if (this.redPlayerConnection == null)
        {
            redPlayerTaken = false;
        }
        else
        {
            redPlayerTaken = true;

        }

        if (this.purplePlayerConnection == null)
        {
            purplePlayerTaken = false;
        }
        else
        {
            purplePlayerTaken = true;

        }

        if (this.bluePlayerConnection == null)
        {
            bluePlayerTaken = false;
        }
        else
        {
            bluePlayerTaken = true;

        }

        //update the broadcast message
        return new LANConnectionInfo(
            NetworkManager.singleton.networkAddress.ToString(),
            NetworkManager.singleton.networkPort,
            this.gameName,
            this.teamsEnabled,
            this.greenPlayerAlive,
            greenPlayerTaken,
            this.redPlayerAlive,
            redPlayerTaken,
            this.purplePlayerAlive,
            purplePlayerTaken,
            this.bluePlayerAlive,
            bluePlayerTaken,
            this.greenPlayerPlanets,
            this.redPlayerPlanets,
            this.purplePlayerPlanets,
            this.bluePlayerPlanets,
            this.victoryPlanets,
            this.gameYear
            );
    }

    //this function sets the local player connection
    private void SetLocalPlayerConnection()
    {
        Debug.Log("SetLocalPlayerConnection");
        Debug.Log("connections = " + networkManager.transform.Find("PlayerConnections").transform.childCount);
        //loop through all game objects in the PlayerConnections object
        for (int i = 0; i < networkManager.transform.Find("PlayerConnections").transform.childCount; i++)
        {
            //check if the Player Connection is local
            if(networkManager.transform.Find("PlayerConnections").transform.GetChild(i).GetComponent<PlayerConnection>().isLocalPlayer == true)
            {
                //set the local player connection
                localPlayerConnection = networkManager.transform.Find("PlayerConnections").transform.GetChild(i).GetComponent<PlayerConnection>();
                Debug.Log("LocalPlayerConnection = " + localPlayerConnection.gameObject.name);
            }

        }
                
    }

    //this function gets data from the server game creation
    private void GetDataFromNewGameSetup()
    {
        //check if this is a new game
        if (isNewGame == true)
        {
            Debug.Log("Get Data from Setup");
            isNewGame = false;

            gameName = uiManager.GetComponent<NewLANGameWindow>().gameName;
            teamsEnabled = uiManager.GetComponent<NewLANGameWindow>().teamsEnabled;
            victoryPlanets = uiManager.GetComponent<NewLANGameWindow>().planetCount;
            gameYear = GameManager.startingGameYear;

            greenPlayerName = uiManager.GetComponent<NewLANGameWindow>().greenPlayerName;
            redPlayerName = uiManager.GetComponent<NewLANGameWindow>().redPlayerName;
            purplePlayerName = uiManager.GetComponent<NewLANGameWindow>().purplePlayerName;
            bluePlayerName = uiManager.GetComponent<NewLANGameWindow>().bluePlayerName;

            //for a new game setup, no players will be ready to start
            greenPlayerReady = false;
            redPlayerReady = false;
            purplePlayerReady = false;
            bluePlayerReady = false;

            //for a new game, no players will have planets
            greenPlayerPlanets = 0;
            redPlayerPlanets = 0;
            purplePlayerPlanets = 0;
            bluePlayerPlanets = 0;

            //for a new game, all players will be alive
            greenPlayerAlive = true;
            redPlayerAlive = true;
            purplePlayerAlive = true;
            bluePlayerAlive = true;

            //determine which players are under host control
            if (uiManager.GetComponent<NewLANGameWindow>().localControlGreen == true)
            {
                greenPlayerConnection = localPlayerConnection;
            }
            if (uiManager.GetComponent<NewLANGameWindow>().localControlRed == true)
            {
                redPlayerConnection = localPlayerConnection;
            }
            if (uiManager.GetComponent<NewLANGameWindow>().localControlPurple == true)
            {
                purplePlayerConnection = localPlayerConnection;
            }
            if (uiManager.GetComponent<NewLANGameWindow>().localControlBlue == true)
            {
                bluePlayerConnection = localPlayerConnection;
            }

            Debug.Log("Get Data from Setup Finished");
            OnFinishedInitialGameSetup.Invoke();

        }

    }

    //this function resolves a request from a player connection (should be the host) to update the planet count
    private void ResolveRequestUpdateVictoryPlanetCount(PlayerConnection requestingPlayerConnection, NetworkInstanceId requestingNetId, int newPlanetCount)
    {
        //check to make sure this is the server
        if(isServer == true)
        {

            //check if the requesting connection is the server
            if(requestingPlayerConnection.isServer == true)
            {
                //check if the new value is within the limits
                if (newPlanetCount <= uiManager.GetComponent<LobbyLANGamePanel>().maxPlanetValue &&
                    newPlanetCount >= uiManager.GetComponent<LobbyLANGamePanel>().minPlanetValue)
                {
                    //update the planet count
                    victoryPlanets = newPlanetCount;
                }
                else
                {
                    Debug.LogError("Attempted to change the victory planets to an invalid value");
                }
            }
            else
            {
                Debug.LogError("A non-server player connection tried to change the victory planet count");
            }

        }
    }

    //this function resolves a request from a player connection to change the team status
    private void ResolveRequestUpdateTeamStatus(PlayerConnection requestingPlayerConnection, NetworkInstanceId requestingNetId, bool newTeamStatus)
    {
        Debug.Log("requestingPlayerConnection is " + requestingPlayerConnection.gameObject.name + " new team status is " + newTeamStatus);
        //check to make sure this is the server
        if (isServer == true)
        {

            //check if the requesting connection is the server
            if (requestingPlayerConnection.isServer == true)
            {
                //update the team status
                teamsEnabled = newTeamStatus;
            }
            else
            {
                Debug.LogError("A non-server player connection tried to change the team status");
            }

        }
    }

    //this function resolves a request from a player connection to take local control of green
    private void ResolveRequestLocalControlGreen(PlayerConnection requestingPlayerConnection, NetworkInstanceId requestingNetId)
    {

        //this function should only be able to be called on the server, because the event that triggers it from
        //playerConnection is a command.  Nevertheless, we'll check to make sure we are the server
        if (isServer == true)
        {
            //check if the green player is available
            if (greenPlayerConnection == null)
            {
                //there is no green player connection currently, so it is available
                Debug.Log("NetworkLobbyLAN SetGreenPlayerConnection");
                greenPlayerConnection = requestingPlayerConnection;

            }
            else
            {
                //else the green player is already taken, so we should not allow the requesting player connection to have it
                Debug.LogError("Client Requested Control of Green when it was already taken");
            }

        }

    }

    //this function resolves a request from a player connection to give up local control of green
    private void ResolveRelinquishLocalControlGreen(PlayerConnection requestingPlayerConnection, NetworkInstanceId requestingNetId)
    {

        //this function should only be able to be called on the server, because the event that triggers it from
        //playerConnection is a command.  Nevertheless, we'll check to make sure we are the server
        if (isServer == true)
        {
            //check if the green player is available
            if (greenPlayerConnection == null)
            {
                //there is no green player connection currently, so we can't give it up
                Debug.LogError("Client Relinquished Control of Green when it was not taken");
                
            }
            else
            {
                //else the green player is taken, so we should allow the requesting player connection to give it up if they are the current controller
                if(greenPlayerConnection == requestingPlayerConnection)
                {
                    Debug.Log("set green player to null");
                    //the requesting player is the current controller - we can give it up
                    greenPlayerConnection = null;

                    //rename the player to default
                    greenPlayerName = uiManager.GetComponent<LobbyLANGamePanel>().defaultGreenPlayerName;
                }
                else
                {
                    //the else condition is that a different player owns green - the requesting player can't make him give it up
                    Debug.LogError("Client Relinquished Green when a different player controlled green");
                }

            }

        }

    }

    //this function resolves a request to set green to ready
    private void ResolveSetGreenToReady(PlayerConnection requestingPlayerConnection, NetworkInstanceId requestingNetId)
    {
        //check if this is the server
        if (isServer == true)
        {
            //check to make sure the green player connection is controlled by the requesting player connection
            if (greenPlayerConnection == requestingPlayerConnection)
            {

                //check to make sure the player is not already ready
                if(greenPlayerReady == false)
                {
                    //set the green player to ready
                    greenPlayerReady = true;
                }
                else
                {
                    Debug.LogError("Green player tried to set to ready when it was already ready");
                }

            }
            else
            {
                Debug.LogError("A connection that didn't control green tried to change ready status");
            }
        }
    }

    //this function resolves a request to set green to not ready
    private void ResolveSetGreenToNotReady(PlayerConnection requestingPlayerConnection, NetworkInstanceId requestingNetId)
    {
        //check if this is the server
        if (isServer == true)
        {
            //check to make sure the green player connection is controlled by the requesting player connection
            if (greenPlayerConnection == requestingPlayerConnection)
            {

                //check to make sure the player is ready
                if (greenPlayerReady == true)
                {
                    //set the green player to not ready
                    greenPlayerReady = false;
                }
                else
                {
                    Debug.LogError("Green player tried to set to not ready when it was already not ready");
                }

            }
            else
            {
                Debug.LogError("A connection that didn't control green tried to change ready status");
            }
        }
    }

    //this function resolves a request to update the green player name
    private void ResolveUpdateGreenPlayerName(PlayerConnection requestingPlayerConnection, NetworkInstanceId requestingNetId, string newName)
    {
        //check if this is the server
        if(isServer == true)
        {
            //check to make sure the green player connection is controlled by the requesting player connection
            if(greenPlayerConnection == requestingPlayerConnection)
            {
                //rename the player
                greenPlayerName = newName;
            }
            else
            {
                Debug.LogError("A connection that didn't control green tried to change green name");
            }
        }
    }


    //this function resolves a request from a player connection to take local control of red
    private void ResolveRequestLocalControlRed(PlayerConnection requestingPlayerConnection, NetworkInstanceId requestingNetId)
    {

        //this function should only be able to be called on the server, because the event that triggers it from
        //playerConnection is a command.  Nevertheless, we'll check to make sure we are the server
        if (isServer == true)
        {
            //check if the red player is available
            if (redPlayerConnection == null)
            {
                //there is no red player connection currently, so it is available
                redPlayerConnection = requestingPlayerConnection;

            }
            else
            {
                //else the red player is already taken, so we should not allow the requesting player connection to have it
                Debug.LogError("Client Requested Control of Red when it was already taken");
            }

        }

    }

    //this function resolves a request from a player connection to give up local control of red
    private void ResolveRelinquishLocalControlRed(PlayerConnection requestingPlayerConnection, NetworkInstanceId requestingNetId)
    {

        //this function should only be able to be called on the server, because the event that triggers it from
        //playerConnection is a command.  Nevertheless, we'll check to make sure we are the server
        if (isServer == true)
        {
            //check if the red player is available
            if (redPlayerConnection == null)
            {
                //there is no red player connection currently, so we can't give it up
                Debug.LogError("Client Relinquished Control of Red when it was not taken");

            }
            else
            {
                //else the green player is taken, so we should allow the requesting player connection to give it up if they are the current controller
                if (redPlayerConnection == requestingPlayerConnection)
                {
                    //the requesting player is the current controller - we can give it up
                    redPlayerConnection = null;

                    //rename the player to default
                    redPlayerName = uiManager.GetComponent<LobbyLANGamePanel>().defaultRedPlayerName;
                }
                else
                {
                    //the else condition is that a different player owns green - the requesting player can't make him give it up
                    Debug.LogError("Client Relinquished Red when a different player controlled red");
                }

            }

        }

    }

    //this function resolves a request to set red to ready
    private void ResolveSetRedToReady(PlayerConnection requestingPlayerConnection, NetworkInstanceId requestingNetId)
    {
        //check if this is the server
        if (isServer == true)
        {
            //check to make sure the red player connection is controlled by the requesting player connection
            if (redPlayerConnection == requestingPlayerConnection)
            {

                //check to make sure the player is not already ready
                if (redPlayerReady == false)
                {
                    //set the red player to ready
                    redPlayerReady = true;
                }
                else
                {
                    Debug.LogError("Red player tried to set to ready when it was already ready");
                }

            }
            else
            {
                Debug.LogError("A connection that didn't control red tried to change ready status");
            }
        }
    }

    //this function resolves a request to set red to not ready
    private void ResolveSetRedToNotReady(PlayerConnection requestingPlayerConnection, NetworkInstanceId requestingNetId)
    {
        //check if this is the server
        if (isServer == true)
        {
            //check to make sure the red player connection is controlled by the requesting player connection
            if (redPlayerConnection == requestingPlayerConnection)
            {

                //check to make sure the player is ready
                if (redPlayerReady == true)
                {
                    //set the red player to not ready
                    redPlayerReady = false;
                }
                else
                {
                    Debug.LogError("Red player tried to set to not ready when it was already not ready");
                }

            }
            else
            {
                Debug.LogError("A connection that didn't control red tried to change ready status");
            }
        }
    }

    //this function resolves a request to update the red player name
    private void ResolveUpdateRedPlayerName(PlayerConnection requestingPlayerConnection, NetworkInstanceId requestingNetId, string newName)
    {
        //check if this is the server
        if (isServer == true)
        {
            //check to make sure the red player connection is controlled by the requesting player connection
            if (redPlayerConnection == requestingPlayerConnection)
            {
                //rename the player
                redPlayerName = newName;
            }
            else
            {
                Debug.LogError("A connection that didn't control red tried to change red name");
            }
        }
    }



    //this function resolves a request from a player connection to take local control of purple
    private void ResolveRequestLocalControlPurple(PlayerConnection requestingPlayerConnection, NetworkInstanceId requestingNetId)
    {

        //this function should only be able to be called on the server, because the event that triggers it from
        //playerConnection is a command.  Nevertheless, we'll check to make sure we are the server
        if (isServer == true)
        {
            //check if the purple player is available
            if (purplePlayerConnection == null)
            {
                //there is no red player connection currently, so it is available
                purplePlayerConnection = requestingPlayerConnection;

            }
            else
            {
                //else the purple player is already taken, so we should not allow the requesting player connection to have it
                Debug.LogError("Client Requested Control of Purple when it was already taken");
            }

        }

    }

    //this function resolves a request from a player connection to give up local control of purple
    private void ResolveRelinquishLocalControlPurple(PlayerConnection requestingPlayerConnection, NetworkInstanceId requestingNetId)
    {

        //this function should only be able to be called on the server, because the event that triggers it from
        //playerConnection is a command.  Nevertheless, we'll check to make sure we are the server
        if (isServer == true)
        {
            //check if the purple player is available
            if (purplePlayerConnection == null)
            {
                //there is no purple player connection currently, so we can't give it up
                Debug.LogError("Client Relinquished Control of Purple when it was not taken");

            }
            else
            {
                //else the purple player is taken, so we should allow the requesting player connection to give it up if they are the current controller
                if (purplePlayerConnection == requestingPlayerConnection)
                {
                    //the requesting player is the current controller - we can give it up
                    purplePlayerConnection = null;

                    //rename the player to default
                    purplePlayerName = uiManager.GetComponent<LobbyLANGamePanel>().defaultPurplePlayerName;
                }
                else
                {
                    //the else condition is that a different player owns purple - the requesting player can't make him give it up
                    Debug.LogError("Client Relinquished Purple when a different player controlled purple");
                }

            }

        }

    }

    //this function resolves a request to set purple to ready
    private void ResolveSetPurpleToReady(PlayerConnection requestingPlayerConnection, NetworkInstanceId requestingNetId)
    {
        //check if this is the server
        if (isServer == true)
        {
            //check to make sure the purple player connection is controlled by the requesting player connection
            if (purplePlayerConnection == requestingPlayerConnection)
            {

                //check to make sure the player is not already ready
                if (purplePlayerReady == false)
                {
                    //set the red player to ready
                    purplePlayerReady = true;
                }
                else
                {
                    Debug.LogError("Purple player tried to set to ready when it was already ready");
                }

            }
            else
            {
                Debug.LogError("A connection that didn't control purple tried to change ready status");
            }
        }
    }

    //this function resolves a request to set purple to not ready
    private void ResolveSetPurpleToNotReady(PlayerConnection requestingPlayerConnection, NetworkInstanceId requestingNetId)
    {
        //check if this is the server
        if (isServer == true)
        {
            //check to make sure the purple player connection is controlled by the requesting player connection
            if (purplePlayerConnection == requestingPlayerConnection)
            {

                //check to make sure the player is ready
                if (purplePlayerReady == true)
                {
                    //set the purple player to not ready
                    purplePlayerReady = false;
                }
                else
                {
                    Debug.LogError("Purple player tried to set to not ready when it was already not ready");
                }

            }
            else
            {
                Debug.LogError("A connection that didn't control purple tried to change ready status");
            }
        }
    }

    //this function resolves a request to update the purple player name
    private void ResolveUpdatePurplePlayerName(PlayerConnection requestingPlayerConnection, NetworkInstanceId requestingNetId, string newName)
    {
        //check if this is the server
        if (isServer == true)
        {
            //check to make sure the purple player connection is controlled by the requesting player connection
            if (purplePlayerConnection == requestingPlayerConnection)
            {
                //rename the player
                purplePlayerName = newName;
            }
            else
            {
                Debug.LogError("A connection that didn't control purple tried to change purple name");
            }
        }
    }


    //this function resolves a request from a player connection to take local control of blue
    private void ResolveRequestLocalControlBlue(PlayerConnection requestingPlayerConnection, NetworkInstanceId requestingNetId)
    {

        //this function should only be able to be called on the server, because the event that triggers it from
        //playerConnection is a command.  Nevertheless, we'll check to make sure we are the server
        if (isServer == true)
        {
            //check if the blue player is available
            if (bluePlayerConnection == null)
            {
                //there is no red player connection currently, so it is available
                bluePlayerConnection = requestingPlayerConnection;

            }
            else
            {
                //else the blue player is already taken, so we should not allow the requesting player connection to have it
                Debug.LogError("Client Requested Control of Blue when it was already taken");
            }

        }

    }

    //this function resolves a request from a player connection to give up local control of blue
    private void ResolveRelinquishLocalControlBlue(PlayerConnection requestingPlayerConnection, NetworkInstanceId requestingNetId)
    {

        //this function should only be able to be called on the server, because the event that triggers it from
        //playerConnection is a command.  Nevertheless, we'll check to make sure we are the server
        if (isServer == true)
        {
            //check if the blue player is available
            if (bluePlayerConnection == null)
            {
                //there is no blue player connection currently, so we can't give it up
                Debug.LogError("Client Relinquished Control of Blue when it was not taken");

            }
            else
            {
                //else the blue player is taken, so we should allow the requesting player connection to give it up if they are the current controller
                if (bluePlayerConnection == requestingPlayerConnection)
                {
                    //the requesting player is the current controller - we can give it up
                    bluePlayerConnection = null;

                    //rename the player to default
                    bluePlayerName = uiManager.GetComponent<LobbyLANGamePanel>().defaultBluePlayerName;
                }
                else
                {
                    //the else condition is that a different player owns blue - the requesting player can't make him give it up
                    Debug.LogError("Client Relinquished Blue when a different player controlled blue");
                }

            }

        }

    }

    //this function resolves a request to set blue to ready
    private void ResolveSetBlueToReady(PlayerConnection requestingPlayerConnection, NetworkInstanceId requestingNetId)
    {
        //check if this is the server
        if (isServer == true)
        {
            //check to make sure the blue player connection is controlled by the requesting player connection
            if (bluePlayerConnection == requestingPlayerConnection)
            {

                //check to make sure the player is not already ready
                if (bluePlayerReady == false)
                {
                    //set the blue player to ready
                    bluePlayerReady = true;
                }
                else
                {
                    Debug.LogError("Blue player tried to set to ready when it was already ready");
                }

            }
            else
            {
                Debug.LogError("A connection that didn't control blue tried to change ready status");
            }
        }
    }

    //this function resolves a request to set blue to not ready
    private void ResolveSetBlueToNotReady(PlayerConnection requestingPlayerConnection, NetworkInstanceId requestingNetId)
    {
        //check if this is the server
        if (isServer == true)
        {
            //check to make sure the blue player connection is controlled by the requesting player connection
            if (bluePlayerConnection == requestingPlayerConnection)
            {

                //check to make sure the player is ready
                if (bluePlayerReady == true)
                {
                    //set the blue player to not ready
                    bluePlayerReady = false;
                }
                else
                {
                    Debug.LogError("Blue player tried to set to not ready when it was already not ready");
                }

            }
            else
            {
                Debug.LogError("A connection that didn't control blue tried to change ready status");
            }
        }
    }

    //this function resolves a request to update the blue player name
    private void ResolveUpdateBluePlayerName(PlayerConnection requestingPlayerConnection, NetworkInstanceId requestingNetId, string newName)
    {
        //check if this is the server
        if (isServer == true)
        {
            //check to make sure the blue player connection is controlled by the requesting player connection
            if (bluePlayerConnection == requestingPlayerConnection)
            {
                //rename the player
                bluePlayerName = newName;
            }
            else
            {
                Debug.LogError("A connection that didn't control blue tried to change blue name");
            }
        }
    }

    //this function updates the game name
    [ClientRpc]
    private void RpcUpdateGameName(string serverGameName)
    {
        gameName = serverGameName;
    }

    //this function updates the teams
    [ClientRpc]
    private void RpcUpdateTeamsEnabled(bool serverTeamsEnabled)
    {
        teamsEnabled = serverTeamsEnabled;
    }

    //this function updates the victory planets
    [ClientRpc]
    private void RpcUpdateVictoryPlanets(int serverVictoryPlanets)
    {
        victoryPlanets = serverVictoryPlanets;
    }

    //this function updates the game year
    [ClientRpc]
    private void RpcUpdateGameYear(int serverGameYear)
    {
        gameYear = serverGameYear;
    }

    //this function updates the green player name
    [ClientRpc]
    private void RpcUpdateGreenPlayerName(string serverGreenPlayerName)
    {
        greenPlayerName = serverGreenPlayerName;
    }

    //this function updates the red player name
    [ClientRpc]
    private void RpcUpdateRedPlayerName(string serverRedPlayerName)
    {
        redPlayerName = serverRedPlayerName;
    }

    //this function updates the purple player name
    [ClientRpc]
    private void RpcUpdatePurplePlayerName(string serverPurplePlayerName)
    {
        purplePlayerName = serverPurplePlayerName;
    }

    //this function updates the blue player name
    [ClientRpc]
    private void RpcUpdateBluePlayerName(string serverBluePlayerName)
    {
        bluePlayerName = serverBluePlayerName;
    }

    //this function updates the green player planets
    [ClientRpc]
    private void RpcUpdateGreenPlayerPlanets(int serverGreenPlayerPlanets)
    {
        greenPlayerPlanets = serverGreenPlayerPlanets;
    }

    //this function updates the red player planets
    [ClientRpc]
    private void RpcUpdateRedPlayerPlanets(int serverRedPlayerPlanets)
    {
        redPlayerPlanets = serverRedPlayerPlanets;
    }

    //this function updates the purple player planets
    [ClientRpc]
    private void RpcUpdatePurplePlayerPlanets(int serverPurplePlayerPlanets)
    {
        purplePlayerPlanets = serverPurplePlayerPlanets;
    }

    //this function updates the blue player planets
    [ClientRpc]
    private void RpcUpdateBluePlayerPlanets(int serverBluePlayerPlanets)
    {
        bluePlayerPlanets = serverBluePlayerPlanets;
    }

    //this function updates the green player ready status
    [ClientRpc]
    private void RpcUpdateGreenPlayerReady(bool serverGreenPlayerReady)
    {
        greenPlayerReady = serverGreenPlayerReady;
    }

    //this function updates the red player ready status
    [ClientRpc]
    private void RpcUpdateRedPlayerReady(bool serverRedPlayerReady)
    {
        redPlayerReady = serverRedPlayerReady;
    }

    //this function updates the purple player ready status
    [ClientRpc]
    private void RpcUpdatePurplePlayerReady(bool serverPurplePlayerReady)
    {
        purplePlayerReady = serverPurplePlayerReady;
    }

    //this function updates the blue player ready status
    [ClientRpc]
    private void RpcUpdateBluePlayerReady(bool serverBluePlayerReady)
    {
        bluePlayerReady = serverBluePlayerReady;
    }

    //this function updates the green player alive status
    [ClientRpc]
    private void RpcUpdateGreenPlayerAlive(bool serverGreenPlayerAlive)
    {
        greenPlayerAlive = serverGreenPlayerAlive;
    }

    //this function updates the red player alive status
    [ClientRpc]
    private void RpcUpdateRedPlayerAlive(bool serverRedPlayerAlive)
    {
        redPlayerAlive = serverRedPlayerAlive;
    }

    //this function updates the purple player alive status
    [ClientRpc]
    private void RpcUpdatePurplePlayerAlive(bool serverPurplePlayerAlive)
    {
        purplePlayerAlive = serverPurplePlayerAlive;
    }

    //this function updates the blue player alive status
    [ClientRpc]
    private void RpcUpdateBluePlayerAlive(bool serverBluePlayerAlive)
    {
        bluePlayerAlive = serverBluePlayerAlive;
    }

    //this function updates the green player connection
    [ClientRpc]
    private void RpcUpdateGreenPlayerConnection(GameObject serverGreenPlayerConnectionObject)
    {
        if (serverGreenPlayerConnectionObject != null)
        {
            Debug.Log("NetworkLobbyLAN RpcUpdateGreenPlayerConnection");
            greenPlayerConnection = serverGreenPlayerConnectionObject.GetComponent<PlayerConnection>();
        }
        else
        {
            greenPlayerConnection = null;
        }
    }

    //this function updates the red player connection
    [ClientRpc]
    private void RpcUpdateRedPlayerConnection(GameObject serverRedPlayerConnectionObject)
    {
        if (serverRedPlayerConnectionObject != null)
        {
            redPlayerConnection = serverRedPlayerConnectionObject.GetComponent<PlayerConnection>();
        }
        else
        {
            redPlayerConnection = null;
        }
    }

    //this function updates the purple player connection
    [ClientRpc]
    private void RpcUpdatePurplePlayerConnection(GameObject serverPurplePlayerConnectionObject)
    {
        if (serverPurplePlayerConnectionObject != null)
        {
            purplePlayerConnection = serverPurplePlayerConnectionObject.GetComponent<PlayerConnection>();
        }
        else
        {
            purplePlayerConnection = null;
        }
    }

    //this function updates the blue player connection
    [ClientRpc]
    private void RpcUpdateBluePlayerConnection(GameObject serverBluePlayerConnectionObject)
    {
        if (serverBluePlayerConnectionObject != null)
        {
            bluePlayerConnection = serverBluePlayerConnectionObject.GetComponent<PlayerConnection>();
        }
        else
        {
            bluePlayerConnection = null;
        }
    }


    //this is a request initial status from the server
    private void UpdateAllRPCs()
    {
        //the server calls all the RPCs
        RpcUpdateGameName(gameName);
        RpcUpdateTeamsEnabled(teamsEnabled);
        RpcUpdateVictoryPlanets(victoryPlanets);
        RpcUpdateGameYear(gameYear);

        RpcUpdateGreenPlayerName(greenPlayerName);
        RpcUpdateRedPlayerName(redPlayerName);
        RpcUpdatePurplePlayerName(purplePlayerName);
        RpcUpdateBluePlayerName(bluePlayerName);

        RpcUpdateGreenPlayerPlanets(greenPlayerPlanets);
        RpcUpdateRedPlayerPlanets(redPlayerPlanets);
        RpcUpdatePurplePlayerPlanets(purplePlayerPlanets);
        RpcUpdateBluePlayerPlanets(bluePlayerPlanets);

        RpcUpdateGreenPlayerReady(greenPlayerReady);
        RpcUpdateRedPlayerReady(redPlayerReady);
        RpcUpdatePurplePlayerReady(purplePlayerReady);
        RpcUpdateBluePlayerReady(bluePlayerReady);

        RpcUpdateGreenPlayerAlive(greenPlayerAlive);
        RpcUpdateRedPlayerAlive(redPlayerAlive);
        RpcUpdatePurplePlayerAlive(purplePlayerAlive);
        RpcUpdateBluePlayerAlive(bluePlayerAlive);


        if (greenPlayerConnection != null)
        {
            RpcUpdateGreenPlayerConnection(greenPlayerConnection.gameObject);
        }
        else
        {
            RpcUpdateGreenPlayerConnection(null);
        }

        if (redPlayerConnection != null)
        {
            RpcUpdateRedPlayerConnection(redPlayerConnection.gameObject);
        }
        else
        {
            RpcUpdateRedPlayerConnection(null);
        }

        if (purplePlayerConnection != null)
        {
            RpcUpdatePurplePlayerConnection(purplePlayerConnection.gameObject);
        }
        else
        {
            RpcUpdatePurplePlayerConnection(null);
        }

        if (bluePlayerConnection != null)
        {
            RpcUpdateBluePlayerConnection(bluePlayerConnection.gameObject);
        }
        else
        {
            RpcUpdateBluePlayerConnection(null);
        }

    }


    //this function handles on destroy
    private void OnDestroy()
    {
        RemoveEventListeners();
    }

    //this function removes event listeners
    private void RemoveEventListeners()
    {
        //remove listener for requesting RPC update
        PlayerConnection.OnRequestRPCUpdate.RemoveListener(playerConnectionUpdateRPCAction);

        //remove listener for updating victory planet count
        PlayerConnection.OnUpdateVictoryPlanetCount.RemoveListener(playerConnectionUpdateVictoryPlanetCountAction);

        //remove listener for updating the team status
        PlayerConnection.OnUpdateTeamStatus.RemoveListener(playerConnectionUpdateTeamStatusAction);

        //remove listener for player requesting control of green
        PlayerConnection.OnRequestLocalControlGreen.RemoveListener(playerConnectionRequestControlGreenAction);

        //remove listener for player relinquishing control of green
        PlayerConnection.OnRelinquishLocalControlGreen.RemoveListener(playerConnectionRelinquishControlGreenAction);

        //remove listener for player setting green to ready
        PlayerConnection.OnSetGreenPlayerToReady.RemoveListener(playerConnectionSetGreenToReadyAction);

        //remove listener for player setting green to not ready
        PlayerConnection.OnSetGreenPlayerToNotReady.RemoveListener(playerConnectionSetGreenToNotReadyAction);

        //remove listener for player updating green name
        PlayerConnection.OnUpdateGreenPlayerName.RemoveListener(playerConnectionRenameGreenAction);


        //remove listener for player requesting control of red
        PlayerConnection.OnRequestLocalControlRed.RemoveListener(playerConnectionRequestControlRedAction);

        //remove listener for player relinquishing control of red
        PlayerConnection.OnRelinquishLocalControlRed.RemoveListener(playerConnectionRelinquishControlRedAction);

        //remove listener for player setting red to ready
        PlayerConnection.OnSetRedPlayerToReady.RemoveListener(playerConnectionSetRedToReadyAction);

        //remove listener for player setting red to not ready
        PlayerConnection.OnSetRedPlayerToNotReady.RemoveListener(playerConnectionSetRedToNotReadyAction);

        //remove listener for player updating red name
        PlayerConnection.OnUpdateRedPlayerName.RemoveListener(playerConnectionRenameRedAction);


        //remove listener for player requesting control of purple
        PlayerConnection.OnRequestLocalControlPurple.RemoveListener(playerConnectionRequestControlPurpleAction);

        //remove listener for player relinquishing control of purple
        PlayerConnection.OnRelinquishLocalControlPurple.RemoveListener(playerConnectionRelinquishControlPurpleAction);

        //remove listener for player setting purple to ready
        PlayerConnection.OnSetPurplePlayerToReady.RemoveListener(playerConnectionSetPurpleToReadyAction);

        //remove listener for player setting purple to not ready
        PlayerConnection.OnSetPurplePlayerToNotReady.RemoveListener(playerConnectionSetPurpleToNotReadyAction);

        //remove listener for player updating purple name
        PlayerConnection.OnUpdatePurplePlayerName.RemoveListener(playerConnectionRenamePurpleAction);


        //remove listener for player requesting control of blue
        PlayerConnection.OnRequestLocalControlBlue.RemoveListener(playerConnectionRequestControlBlueAction);

        //remove listener for player relinquishing control of blue
        PlayerConnection.OnRelinquishLocalControlBlue.RemoveListener(playerConnectionRelinquishControlBlueAction);

        //remove listener for player setting blue to ready
        PlayerConnection.OnSetBluePlayerToReady.RemoveListener(playerConnectionSetBlueToReadyAction);

        //remove listener for player setting blue to not ready
        PlayerConnection.OnSetBluePlayerToNotReady.RemoveListener(playerConnectionSetBlueToNotReadyAction);

        //remove listener for player updating blue name
        PlayerConnection.OnUpdateBluePlayerName.RemoveListener(playerConnectionRenameBlueAction);

        //remove listener for server player starting
        PlayerConnection.OnServerPlayerStart.RemoveListener(playerConnectionServerPlayerStartAction);

    }

}
