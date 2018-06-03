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
    public static ConnectionEvent OnRequestClientAuthority = new ConnectionEvent();
    public static ConnectionEvent OnRequestActionCommand = new ConnectionEvent();

    public static ConnectionEvent OnRequestLocalControlGreen = new ConnectionEvent();
    public static ConnectionEvent OnRelinquishLocalControlGreen = new ConnectionEvent();
    public static ConnectionEvent OnSetGreenPlayerToReady = new ConnectionEvent();
    public static ConnectionEvent OnSetGreenPlayerToNotReady = new ConnectionEvent();
    public static ConnectionStringEvent OnUpdateGreenPlayerName = new ConnectionStringEvent();

    //class for passing connections
    public class ConnectionEvent : UnityEvent<PlayerConnection, NetworkInstanceId> { };

    //class for passing connections
    public class ConnectionStringEvent : UnityEvent<PlayerConnection, NetworkInstanceId, string> { };

    //unityActions
    private UnityAction<PlayerConnection, NetworkInstanceId> playerConnectionUpdateRPCAction;
    private UnityAction requestLocalControlGreenAction;
    private UnityAction relinquishLocalControlGreenAction;
    private UnityAction setGreenPlayerToReadyAction;
    private UnityAction setGreenPlayerToNotReadyAction;
    private UnityAction<string> stringUpdateGreenPlayerNameAction;

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

        requestLocalControlGreenAction = () => { CmdRequestLocalControlGreen(this.gameObject, this.netId); };
        relinquishLocalControlGreenAction = () => { CmdRelinquishLocalControlGreen(this.gameObject, this.netId); };
        setGreenPlayerToReadyAction = () => { CmdSetGreenPlayerToReady(this.gameObject, this.netId); };
        setGreenPlayerToNotReadyAction = () => { CmdSetGreenPlayerToNotReady(this.gameObject, this.netId); };
        stringUpdateGreenPlayerNameAction = (newName) => { CmdUpdateGreenPlayerName(this.gameObject, this.netId, newName); };

    }

    //this function adds event listeners
    private void AddEventListeners()
    {
        //add listener for lobby requesting RPC update
        NetworkLobbyLAN.OnRequestRPCUpdate.AddListener(playerConnectionUpdateRPCAction);

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

        //add listeners for the lobby player connections being updated
        NetworkLobbyLAN.OnUpdateGreenPlayerConnection.AddListener(UpdatePlayerControlStatus);
        NetworkLobbyLAN.OnUpdateRedPlayerConnection.AddListener(UpdatePlayerControlStatus);
        NetworkLobbyLAN.OnUpdatePurplePlayerConnection.AddListener(UpdatePlayerControlStatus);
        NetworkLobbyLAN.OnUpdateBluePlayerConnection.AddListener(UpdatePlayerControlStatus);

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

    //this command requests an RPC update for the requested object
    [Command]
    private void CmdRequestRPCUpdate(GameObject playerConnectionGameObject, NetworkInstanceId netId)
    {
        //invoke the event on the server side
        OnRequestRPCUpdate.Invoke(playerConnectionGameObject.GetComponent<PlayerConnection>(), netId);
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
            }
            
            //remove listeners for the lobby player connections being updated
            NetworkLobbyLAN.OnUpdateGreenPlayerConnection.RemoveListener(UpdatePlayerControlStatus);
            NetworkLobbyLAN.OnUpdateRedPlayerConnection.RemoveListener(UpdatePlayerControlStatus);
            NetworkLobbyLAN.OnUpdatePurplePlayerConnection.RemoveListener(UpdatePlayerControlStatus);
            NetworkLobbyLAN.OnUpdateBluePlayerConnection.RemoveListener(UpdatePlayerControlStatus);
        }

    }

}
