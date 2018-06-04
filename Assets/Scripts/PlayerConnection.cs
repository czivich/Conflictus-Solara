using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;

public class PlayerConnection : NetworkBehaviour {

    //manager
    private GameObject networkManager;
    private GameObject uiManager;

    //private GameManager gameManager;


    //this holds the parent object in the hierarchy
    public GameObject parentObject;

    //bools to track if the players are locally controlled
    public bool localGreenPlayer { get; private set; }
    public bool localRedPlayer { get; private set; }
    public bool localPurplePlayer { get; private set; }
    public bool localBluePlayer { get; private set; }

    public static ConnectionEvent OnRequestRPCUpdate = new ConnectionEvent();

    public static ConnectionIntEvent OnUpdateVictoryPlanetCount = new ConnectionIntEvent();
    public static ConnectionBoolEvent OnUpdateTeamStatus = new ConnectionBoolEvent();

    public static ConnectionEvent OnRequestLocalControlGreen = new ConnectionEvent();
    public static ConnectionEvent OnRelinquishLocalControlGreen = new ConnectionEvent();
    public static ConnectionEvent OnSetGreenPlayerToReady = new ConnectionEvent();
    public static ConnectionEvent OnSetGreenPlayerToNotReady = new ConnectionEvent();
    public static ConnectionStringEvent OnUpdateGreenPlayerName = new ConnectionStringEvent();

    public static ConnectionEvent OnRequestLocalControlRed = new ConnectionEvent();
    public static ConnectionEvent OnRelinquishLocalControlRed = new ConnectionEvent();
    public static ConnectionEvent OnSetRedPlayerToReady = new ConnectionEvent();
    public static ConnectionEvent OnSetRedPlayerToNotReady = new ConnectionEvent();
    public static ConnectionStringEvent OnUpdateRedPlayerName = new ConnectionStringEvent();

    public static ConnectionEvent OnRequestLocalControlPurple = new ConnectionEvent();
    public static ConnectionEvent OnRelinquishLocalControlPurple = new ConnectionEvent();
    public static ConnectionEvent OnSetPurplePlayerToReady = new ConnectionEvent();
    public static ConnectionEvent OnSetPurplePlayerToNotReady = new ConnectionEvent();
    public static ConnectionStringEvent OnUpdatePurplePlayerName = new ConnectionStringEvent();

    public static ConnectionEvent OnRequestLocalControlBlue = new ConnectionEvent();
    public static ConnectionEvent OnRelinquishLocalControlBlue = new ConnectionEvent();
    public static ConnectionEvent OnSetBluePlayerToReady = new ConnectionEvent();
    public static ConnectionEvent OnSetBluePlayerToNotReady = new ConnectionEvent();
    public static ConnectionStringEvent OnUpdateBluePlayerName = new ConnectionStringEvent();

    //class for passing connections
    public class ConnectionEvent : UnityEvent<PlayerConnection, NetworkInstanceId> { };

    //class for passing connections with a string
    public class ConnectionStringEvent : UnityEvent<PlayerConnection, NetworkInstanceId, string> { };

    //class for passing connections with an int
    public class ConnectionIntEvent : UnityEvent<PlayerConnection, NetworkInstanceId, int> { };

    //class for passing connections with a bool
    public class ConnectionBoolEvent : UnityEvent<PlayerConnection, NetworkInstanceId, bool> { };

    //unityActions
    private UnityAction<PlayerConnection, NetworkInstanceId> playerConnectionUpdateRPCAction;
    private UnityAction<int> intUpdateVictoryPlanetCountAction;
    private UnityAction<bool> boolUpdateTeamStatusAction;

    private UnityAction requestLocalControlGreenAction;
    private UnityAction relinquishLocalControlGreenAction;
    private UnityAction setGreenPlayerToReadyAction;
    private UnityAction setGreenPlayerToNotReadyAction;
    private UnityAction<string> stringUpdateGreenPlayerNameAction;

    private UnityAction requestLocalControlRedAction;
    private UnityAction relinquishLocalControlRedAction;
    private UnityAction setRedPlayerToReadyAction;
    private UnityAction setRedPlayerToNotReadyAction;
    private UnityAction<string> stringUpdateRedPlayerNameAction;

    private UnityAction requestLocalControlPurpleAction;
    private UnityAction relinquishLocalControlPurpleAction;
    private UnityAction setPurplePlayerToReadyAction;
    private UnityAction setPurplePlayerToNotReadyAction;
    private UnityAction<string> stringUpdatePurplePlayerNameAction;

    private UnityAction requestLocalControlBlueAction;
    private UnityAction relinquishLocalControlBlueAction;
    private UnityAction setBluePlayerToReadyAction;
    private UnityAction setBluePlayerToNotReadyAction;
    private UnityAction<string> stringUpdateBluePlayerNameAction;


    // Use this for initialization
    private void Start () {


        //get the manager
        networkManager = GameObject.FindGameObjectWithTag("NetworkManager");
        uiManager = GameObject.FindGameObjectWithTag("UIManager");

        //name the name in hierarchy
        this.name = "PlayerConnection" + this.GetComponent<NetworkIdentity>().netId.ToString();

        //set the parent for the hierarchy
        this.transform.SetParent(networkManager.transform.Find("PlayerConnections").transform);

        //get the gameManager
        //gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        if (this.isLocalPlayer == true)

        {
            //set actions
            SetActions();

            //add listeners
            AddEventListeners();

        }


    }

    //this function sets unityActions
    private void SetActions()
    {
        playerConnectionUpdateRPCAction = (playerConnection, netId) => { ResolveRPCUpdate(playerConnection, netId); };
        intUpdateVictoryPlanetCountAction = (newCount) => { CmdUpdateVictoryPlanetCount(this.gameObject, this.netId, newCount); };
        boolUpdateTeamStatusAction = (newTeamStatus) => { CmdUpdateTeamStatus(this.gameObject, this.netId, newTeamStatus); };

        requestLocalControlGreenAction = () => { CmdRequestLocalControlGreen(this.gameObject, this.netId); };
        relinquishLocalControlGreenAction = () => { CmdRelinquishLocalControlGreen(this.gameObject, this.netId); };
        setGreenPlayerToReadyAction = () => { CmdSetGreenPlayerToReady(this.gameObject, this.netId); };
        setGreenPlayerToNotReadyAction = () => { CmdSetGreenPlayerToNotReady(this.gameObject, this.netId); };
        stringUpdateGreenPlayerNameAction = (newName) => { CmdUpdateGreenPlayerName(this.gameObject, this.netId, newName); };

        requestLocalControlRedAction = () => { CmdRequestLocalControlRed(this.gameObject, this.netId); };
        relinquishLocalControlRedAction = () => { CmdRelinquishLocalControlRed(this.gameObject, this.netId); };
        setRedPlayerToReadyAction = () => { CmdSetRedPlayerToReady(this.gameObject, this.netId); };
        setRedPlayerToNotReadyAction = () => { CmdSetRedPlayerToNotReady(this.gameObject, this.netId); };
        stringUpdateRedPlayerNameAction = (newName) => { CmdUpdateRedPlayerName(this.gameObject, this.netId, newName); };

        requestLocalControlPurpleAction = () => { CmdRequestLocalControlPurple(this.gameObject, this.netId); };
        relinquishLocalControlPurpleAction = () => { CmdRelinquishLocalControlPurple(this.gameObject, this.netId); };
        setPurplePlayerToReadyAction = () => { CmdSetPurplePlayerToReady(this.gameObject, this.netId); };
        setPurplePlayerToNotReadyAction = () => { CmdSetPurplePlayerToNotReady(this.gameObject, this.netId); };
        stringUpdatePurplePlayerNameAction = (newName) => { CmdUpdatePurplePlayerName(this.gameObject, this.netId, newName); };

        requestLocalControlBlueAction = () => { CmdRequestLocalControlBlue(this.gameObject, this.netId); };
        relinquishLocalControlBlueAction = () => { CmdRelinquishLocalControlBlue (this.gameObject, this.netId); };
        setBluePlayerToReadyAction = () => { CmdSetBluePlayerToReady(this.gameObject, this.netId); };
        setBluePlayerToNotReadyAction = () => { CmdSetBluePlayerToNotReady(this.gameObject, this.netId); };
        stringUpdateBluePlayerNameAction = (newName) => { CmdUpdateBluePlayerName(this.gameObject, this.netId, newName); };

    }

    //this function adds event listeners
    private void AddEventListeners()
    {
        //add listener for lobby requesting RPC update
        NetworkLobbyLAN.OnRequestRPCUpdate.AddListener(playerConnectionUpdateRPCAction);

        //add listener for updating the victory planet count
        uiManager.GetComponent<LobbyLANGamePanel>().OnUpdateVictoryPlanetCount.AddListener(intUpdateVictoryPlanetCountAction);

        //add listener for updating the team status
        uiManager.GetComponent<LobbyLANGamePanel>().OnUpdateTeamsEnabled.AddListener(boolUpdateTeamStatusAction);

        //add listener for requesting local control of green
        uiManager.GetComponent<LobbyLANGamePanel>().OnRequestLocalControlGreen.AddListener(requestLocalControlGreenAction);

        //add listener for relinquishing local control of green
        uiManager.GetComponent<LobbyLANGamePanel>().OnRelinquishLocalControlGreen.AddListener(relinquishLocalControlGreenAction);

        //add listener for setting green to ready
        uiManager.GetComponent<LobbyLANGamePanel>().OnGreenPlayerReady.AddListener(setGreenPlayerToReadyAction);

        //add listener for setting green to not ready
        uiManager.GetComponent<LobbyLANGamePanel>().OnGreenPlayerNotReady.AddListener(setGreenPlayerToNotReadyAction);

        //add listener for updating the green name
        uiManager.GetComponent<LobbyLANGamePanel>().OnEnterGreenPlayerName.AddListener(stringUpdateGreenPlayerNameAction);


        //add listener for requesting local control of red
        uiManager.GetComponent<LobbyLANGamePanel>().OnRequestLocalControlRed.AddListener(requestLocalControlRedAction);

        //add listener for relinquishing local control of red
        uiManager.GetComponent<LobbyLANGamePanel>().OnRelinquishLocalControlRed.AddListener(relinquishLocalControlRedAction);

        //add listener for setting red to ready
        uiManager.GetComponent<LobbyLANGamePanel>().OnRedPlayerReady.AddListener(setRedPlayerToReadyAction);

        //add listener for setting red to not ready
        uiManager.GetComponent<LobbyLANGamePanel>().OnRedPlayerNotReady.AddListener(setRedPlayerToNotReadyAction);

        //add listener for updating the red name
        uiManager.GetComponent<LobbyLANGamePanel>().OnEnterRedPlayerName.AddListener(stringUpdateRedPlayerNameAction);


        //add listener for requesting local control of purple
        uiManager.GetComponent<LobbyLANGamePanel>().OnRequestLocalControlPurple.AddListener(requestLocalControlPurpleAction);

        //add listener for relinquishing local control of purple
        uiManager.GetComponent<LobbyLANGamePanel>().OnRelinquishLocalControlPurple.AddListener(relinquishLocalControlPurpleAction);

        //add listener for setting purple to ready
        uiManager.GetComponent<LobbyLANGamePanel>().OnPurplePlayerReady.AddListener(setPurplePlayerToReadyAction);

        //add listener for setting purple to not ready
        uiManager.GetComponent<LobbyLANGamePanel>().OnPurplePlayerNotReady.AddListener(setPurplePlayerToNotReadyAction);

        //add listener for updating the purple name
        uiManager.GetComponent<LobbyLANGamePanel>().OnEnterPurplePlayerName.AddListener(stringUpdatePurplePlayerNameAction);


        //add listener for requesting local control of blue
        uiManager.GetComponent<LobbyLANGamePanel>().OnRequestLocalControlBlue.AddListener(requestLocalControlBlueAction);

        //add listener for relinquishing local control of blue
        uiManager.GetComponent<LobbyLANGamePanel>().OnRelinquishLocalControlBlue.AddListener(relinquishLocalControlBlueAction);

        //add listener for setting blue to ready
        uiManager.GetComponent<LobbyLANGamePanel>().OnBluePlayerReady.AddListener(setBluePlayerToReadyAction);

        //add listener for setting blue to not ready
        uiManager.GetComponent<LobbyLANGamePanel>().OnBluePlayerNotReady.AddListener(setBluePlayerToNotReadyAction);

        //add listener for updating the blue name
        uiManager.GetComponent<LobbyLANGamePanel>().OnEnterBluePlayerName.AddListener(stringUpdateBluePlayerNameAction);


        //add listeners for the lobby player connections being updated
        NetworkLobbyLAN.OnUpdateGreenPlayerConnection.AddListener(UpdatePlayerControlStatus);
        NetworkLobbyLAN.OnUpdateRedPlayerConnection.AddListener(UpdatePlayerControlStatus);
        NetworkLobbyLAN.OnUpdatePurplePlayerConnection.AddListener(UpdatePlayerControlStatus);
        NetworkLobbyLAN.OnUpdateBluePlayerConnection.AddListener(UpdatePlayerControlStatus);

        //add listener for client stopping
        networkManager.GetComponent<NetworkInterface>().OnStopClient.AddListener(ResolveStopClient);

    }

    //this function handles a request for Lobby info
    private void ResolveRPCUpdate(PlayerConnection playerConnection, NetworkInstanceId netId)
    {

        //check if this is the player connection and is the local player connection
        if (playerConnection == this && playerConnection.isLocalPlayer == true)
        {
            
            CmdRequestRPCUpdate(playerConnection.gameObject, netId);
        }
    }

    //this function updates the player control status
    private void UpdatePlayerControlStatus()
    {
        if(networkManager.GetComponentInChildren<NetworkLobbyLAN>().greenPlayerConnection == this)
        {
            localGreenPlayer = true;
        }
        else
        {
            localGreenPlayer = false;
        }


        if (networkManager.GetComponentInChildren<NetworkLobbyLAN>().redPlayerConnection == this)
        {
            localRedPlayer = true;
        }
        else
        {
            localRedPlayer = false;
        }


        if (networkManager.GetComponentInChildren<NetworkLobbyLAN>().purplePlayerConnection == this)
        {
            localPurplePlayer = true;
        }
        else
        {
            localPurplePlayer = false;
        }


        if (networkManager.GetComponentInChildren<NetworkLobbyLAN>().bluePlayerConnection == this)
        {
            localBluePlayer = true;
        }
        else
        {
            localBluePlayer = false;
        }
        
    }

    //this function checks to see what players we are controlling and relinquishes any of them when this player connection is destroyed
    private void RelinquishAllPlayerControl()
    {
        if(localGreenPlayer == true)
        {
            CmdRelinquishLocalControlGreen(this.gameObject, this.netId);
        }

        if (localRedPlayer == true)
        {
            CmdRelinquishLocalControlRed(this.gameObject, this.netId);
        }

        if (localPurplePlayer == true)
        {
            CmdRelinquishLocalControlPurple(this.gameObject, this.netId);
        }

        if (localBluePlayer == true)
        {
            CmdRelinquishLocalControlBlue(this.gameObject, this.netId);
        }
    }

    //this function resolves stopping the client
    private void ResolveStopClient()
    {
        if (this.isLocalPlayer == true)
        {
            RelinquishAllPlayerControl();
        }
    }

    //this command requests an RPC update for the requested object
    [Command]
    private void CmdRequestRPCUpdate(GameObject playerConnectionGameObject, NetworkInstanceId netId)
    {
        //invoke the event on the server side
        OnRequestRPCUpdate.Invoke(playerConnectionGameObject.GetComponent<PlayerConnection>(), netId);
    }

    //this function updates the victory planet count
    [Command]
    private void CmdUpdateVictoryPlanetCount(GameObject requestingPlayerConnectionGameObject, NetworkInstanceId requestingNetId, int newPlanetCount)
    {
        //invoke an event updating the planet count
        OnUpdateVictoryPlanetCount.Invoke(requestingPlayerConnectionGameObject.GetComponent<PlayerConnection>(), requestingNetId, newPlanetCount);
    }

    //this function updates the team enabled status
    [Command]
    private void CmdUpdateTeamStatus(GameObject requestingPlayerConnectionGameObject, NetworkInstanceId requestingNetId, bool newTeamStatus)
    {
        //invoke event updating the team status
        OnUpdateTeamStatus.Invoke(requestingPlayerConnectionGameObject.GetComponent<PlayerConnection>(), requestingNetId, newTeamStatus);
    }


    //this function requests local control of the green player
    [Command]
    private void CmdRequestLocalControlGreen(GameObject requestingPlayerConnectionGameObject, NetworkInstanceId requestingNetId)
    {
        //invoke an event requesting control of the green player
        OnRequestLocalControlGreen.Invoke(requestingPlayerConnectionGameObject.GetComponent<PlayerConnection>(), requestingNetId);
    }

    //this function relinquishes local control of the green player
    [Command]
    private void CmdRelinquishLocalControlGreen(GameObject requestingPlayerConnectionGameObject, NetworkInstanceId requestingNetId)
    {
        //invoke an event relinquishing control of the green player
        OnRelinquishLocalControlGreen.Invoke(requestingPlayerConnectionGameObject.GetComponent<PlayerConnection>(), requestingNetId);
    }

    //this function updates the name of the green player
    [Command]
    private void CmdUpdateGreenPlayerName(GameObject requestingPlayerConnectionGameObject, NetworkInstanceId requestingNetId, string newName)
    {
        //invoke an event renaming the player
        OnUpdateGreenPlayerName.Invoke(requestingPlayerConnectionGameObject.GetComponent<PlayerConnection>(), requestingNetId, newName);
    }

    //this function requests setting the green player to ready
    [Command]
    private void CmdSetGreenPlayerToReady(GameObject requestingPlayerConnectionGameObject, NetworkInstanceId requestingNetId)
    {
        //invoke an event setting green to ready
        OnSetGreenPlayerToReady.Invoke(requestingPlayerConnectionGameObject.GetComponent<PlayerConnection>(), requestingNetId);
    }

    //this function requests setting the green player to not ready
    [Command]
    private void CmdSetGreenPlayerToNotReady(GameObject requestingPlayerConnectionGameObject, NetworkInstanceId requestingNetId)
    {
        //invoke an event setting green to ready
        OnSetGreenPlayerToNotReady.Invoke(requestingPlayerConnectionGameObject.GetComponent<PlayerConnection>(), requestingNetId);
    }


    //this function requests local control of the red player
    [Command]
    private void CmdRequestLocalControlRed(GameObject requestingPlayerConnectionGameObject, NetworkInstanceId requestingNetId)
    {
        //invoke an event requesting control of the red player
        OnRequestLocalControlRed.Invoke(requestingPlayerConnectionGameObject.GetComponent<PlayerConnection>(), requestingNetId);
    }

    //this function relinquishes local control of the red player
    [Command]
    private void CmdRelinquishLocalControlRed(GameObject requestingPlayerConnectionGameObject, NetworkInstanceId requestingNetId)
    {
        //invoke an event relinquishing control of the red player
        OnRelinquishLocalControlRed.Invoke(requestingPlayerConnectionGameObject.GetComponent<PlayerConnection>(), requestingNetId);
    }

    //this function updates the name of the red player
    [Command]
    private void CmdUpdateRedPlayerName(GameObject requestingPlayerConnectionGameObject, NetworkInstanceId requestingNetId, string newName)
    {
        //invoke an event renaming the player
        OnUpdateRedPlayerName.Invoke(requestingPlayerConnectionGameObject.GetComponent<PlayerConnection>(), requestingNetId, newName);
    }

    //this function requests setting the red player to ready
    [Command]
    private void CmdSetRedPlayerToReady(GameObject requestingPlayerConnectionGameObject, NetworkInstanceId requestingNetId)
    {
        //invoke an event setting red to ready
        OnSetRedPlayerToReady.Invoke(requestingPlayerConnectionGameObject.GetComponent<PlayerConnection>(), requestingNetId);
    }

    //this function requests setting the red player to not ready
    [Command]
    private void CmdSetRedPlayerToNotReady(GameObject requestingPlayerConnectionGameObject, NetworkInstanceId requestingNetId)
    {
        //invoke an event setting red to ready
        OnSetRedPlayerToNotReady.Invoke(requestingPlayerConnectionGameObject.GetComponent<PlayerConnection>(), requestingNetId);
    }


    //this function requests local control of the purple player
    [Command]
    private void CmdRequestLocalControlPurple(GameObject requestingPlayerConnectionGameObject, NetworkInstanceId requestingNetId)
    {
        //invoke an event requesting control of the purple player
        OnRequestLocalControlPurple.Invoke(requestingPlayerConnectionGameObject.GetComponent<PlayerConnection>(), requestingNetId);
    }

    //this function relinquishes local control of the purple player
    [Command]
    private void CmdRelinquishLocalControlPurple(GameObject requestingPlayerConnectionGameObject, NetworkInstanceId requestingNetId)
    {
        //invoke an event relinquishing control of the purple player
        OnRelinquishLocalControlPurple.Invoke(requestingPlayerConnectionGameObject.GetComponent<PlayerConnection>(), requestingNetId);
    }

    //this function updates the name of the purple player
    [Command]
    private void CmdUpdatePurplePlayerName(GameObject requestingPlayerConnectionGameObject, NetworkInstanceId requestingNetId, string newName)
    {
        //invoke an event renaming the player
        OnUpdatePurplePlayerName.Invoke(requestingPlayerConnectionGameObject.GetComponent<PlayerConnection>(), requestingNetId, newName);
    }

    //this function requests setting the purple player to ready
    [Command]
    private void CmdSetPurplePlayerToReady(GameObject requestingPlayerConnectionGameObject, NetworkInstanceId requestingNetId)
    {
        //invoke an event setting purple to ready
        OnSetPurplePlayerToReady.Invoke(requestingPlayerConnectionGameObject.GetComponent<PlayerConnection>(), requestingNetId);
    }

    //this function requests setting the purple player to not ready
    [Command]
    private void CmdSetPurplePlayerToNotReady(GameObject requestingPlayerConnectionGameObject, NetworkInstanceId requestingNetId)
    {
        //invoke an event setting purple to ready
        OnSetPurplePlayerToNotReady.Invoke(requestingPlayerConnectionGameObject.GetComponent<PlayerConnection>(), requestingNetId);
    }


    //this function requests local control of the blue player
    [Command]
    private void CmdRequestLocalControlBlue(GameObject requestingPlayerConnectionGameObject, NetworkInstanceId requestingNetId)
    {
        //invoke an event requesting control of the blue player
        OnRequestLocalControlBlue.Invoke(requestingPlayerConnectionGameObject.GetComponent<PlayerConnection>(), requestingNetId);
    }

    //this function relinquishes local control of the blue player
    [Command]
    private void CmdRelinquishLocalControlBlue(GameObject requestingPlayerConnectionGameObject, NetworkInstanceId requestingNetId)
    {
        //invoke an event relinquishing control of the blue player
        OnRelinquishLocalControlBlue.Invoke(requestingPlayerConnectionGameObject.GetComponent<PlayerConnection>(), requestingNetId);
    }

    //this function updates the name of the blue player
    [Command]
    private void CmdUpdateBluePlayerName(GameObject requestingPlayerConnectionGameObject, NetworkInstanceId requestingNetId, string newName)
    {
        //invoke an event renaming the player
        OnUpdateBluePlayerName.Invoke(requestingPlayerConnectionGameObject.GetComponent<PlayerConnection>(), requestingNetId, newName);
    }

    //this function requests setting the blue player to ready
    [Command]
    private void CmdSetBluePlayerToReady(GameObject requestingPlayerConnectionGameObject, NetworkInstanceId requestingNetId)
    {
        //invoke an event setting red to ready
        OnSetBluePlayerToReady.Invoke(requestingPlayerConnectionGameObject.GetComponent<PlayerConnection>(), requestingNetId);
    }

    //this function requests setting the blue player to not ready
    [Command]
    private void CmdSetBluePlayerToNotReady(GameObject requestingPlayerConnectionGameObject, NetworkInstanceId requestingNetId)
    {
        //invoke an event setting blue to ready
        OnSetBluePlayerToNotReady.Invoke(requestingPlayerConnectionGameObject.GetComponent<PlayerConnection>(), requestingNetId);
    }


    //this function handles on destroy
    private void OnDestroy()
    {
        RemoveEventListeners();
    }

    //this function removes event listeners
    private void RemoveEventListeners()
    {
        if (this.isLocalPlayer == true)
        {
            //remove listener for lobby requesting RPC update
            NetworkLobbyLAN.OnRequestRPCUpdate.RemoveListener(playerConnectionUpdateRPCAction);

            if(uiManager != null)
            {

                //remove listener for updating the victory planet count
                uiManager.GetComponent<LobbyLANGamePanel>().OnUpdateVictoryPlanetCount.RemoveListener(intUpdateVictoryPlanetCountAction);

                //remove listener for updating the team status
                uiManager.GetComponent<LobbyLANGamePanel>().OnUpdateTeamsEnabled.RemoveListener(boolUpdateTeamStatusAction);


                //remove listener for requesting local control of green
                uiManager.GetComponent<LobbyLANGamePanel>().OnRequestLocalControlGreen.RemoveListener(requestLocalControlGreenAction);

                //remove listener for relinquishing local control of green
                uiManager.GetComponent<LobbyLANGamePanel>().OnRelinquishLocalControlGreen.RemoveListener(relinquishLocalControlGreenAction);

                //remove listener for setting green to ready
                uiManager.GetComponent<LobbyLANGamePanel>().OnGreenPlayerReady.RemoveListener(setGreenPlayerToReadyAction);

                //remove listener for setting green to not ready
                uiManager.GetComponent<LobbyLANGamePanel>().OnGreenPlayerNotReady.RemoveListener(setGreenPlayerToNotReadyAction);

                //remove listener for updating the green name
                uiManager.GetComponent<LobbyLANGamePanel>().OnEnterGreenPlayerName.RemoveListener(stringUpdateGreenPlayerNameAction);



                //remove listener for requesting local control of red
                uiManager.GetComponent<LobbyLANGamePanel>().OnRequestLocalControlRed.RemoveListener(requestLocalControlRedAction);

                //remove listener for relinquishing local control of red
                uiManager.GetComponent<LobbyLANGamePanel>().OnRelinquishLocalControlRed.RemoveListener(relinquishLocalControlRedAction);

                //remove listener for setting red to ready
                uiManager.GetComponent<LobbyLANGamePanel>().OnRedPlayerReady.RemoveListener(setRedPlayerToReadyAction);

                //remove listener for setting red to not ready
                uiManager.GetComponent<LobbyLANGamePanel>().OnRedPlayerNotReady.RemoveListener(setRedPlayerToNotReadyAction);

                //remove listener for updating the red name
                uiManager.GetComponent<LobbyLANGamePanel>().OnEnterRedPlayerName.RemoveListener(stringUpdateRedPlayerNameAction);


                //remove listener for requesting local control of purple
                uiManager.GetComponent<LobbyLANGamePanel>().OnRequestLocalControlPurple.RemoveListener(requestLocalControlPurpleAction);

                //remove listener for relinquishing local control of purple
                uiManager.GetComponent<LobbyLANGamePanel>().OnRelinquishLocalControlPurple.RemoveListener(relinquishLocalControlPurpleAction);

                //remove listener for setting purple to ready
                uiManager.GetComponent<LobbyLANGamePanel>().OnPurplePlayerReady.RemoveListener(setPurplePlayerToReadyAction);

                //remove listener for setting purple to not ready
                uiManager.GetComponent<LobbyLANGamePanel>().OnPurplePlayerNotReady.RemoveListener(setPurplePlayerToNotReadyAction);

                //remove listener for updating the purple name
                uiManager.GetComponent<LobbyLANGamePanel>().OnEnterPurplePlayerName.RemoveListener(stringUpdatePurplePlayerNameAction);


                //remove listener for requesting local control of blue
                uiManager.GetComponent<LobbyLANGamePanel>().OnRequestLocalControlBlue.RemoveListener(requestLocalControlBlueAction);

                //remove listener for relinquishing local control of blue
                uiManager.GetComponent<LobbyLANGamePanel>().OnRelinquishLocalControlBlue.RemoveListener(relinquishLocalControlBlueAction);

                //remove listener for setting blue to ready
                uiManager.GetComponent<LobbyLANGamePanel>().OnBluePlayerReady.RemoveListener(setBluePlayerToReadyAction);

                //remove listener for setting blue to not ready
                uiManager.GetComponent<LobbyLANGamePanel>().OnBluePlayerNotReady.RemoveListener(setBluePlayerToNotReadyAction);

                //remove listener for updating the blue name
                uiManager.GetComponent<LobbyLANGamePanel>().OnEnterBluePlayerName.RemoveListener(stringUpdateBluePlayerNameAction);

            }
            
            //remove listeners for the lobby player connections being updated
            NetworkLobbyLAN.OnUpdateGreenPlayerConnection.RemoveListener(UpdatePlayerControlStatus);
            NetworkLobbyLAN.OnUpdateRedPlayerConnection.RemoveListener(UpdatePlayerControlStatus);
            NetworkLobbyLAN.OnUpdatePurplePlayerConnection.RemoveListener(UpdatePlayerControlStatus);
            NetworkLobbyLAN.OnUpdateBluePlayerConnection.RemoveListener(UpdatePlayerControlStatus);

            if(networkManager != null)
            {
                //remove listener for client stopping
                networkManager.GetComponent<NetworkInterface>().OnStopClient.RemoveListener(ResolveStopClient);
            }
        }

    }

}
