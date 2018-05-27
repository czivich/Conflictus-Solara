using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;

public class PlayerConnection : NetworkBehaviour {

    //manager
    private GameObject networkManager;
    //private GameManager gameManager;

    //this variable is for what player this connection is controlling
    //private Player playerControlled;

    //this holds the parent object in the hierarchy
    public GameObject parentObject;

    //bools to track if the players are locally controlled
    public bool localGreenPlayer { get; private set; }
    public bool localRedPlayer { get; private set; }
    public bool localPurplePlayer { get; private set; }
    public bool localBluePlayer { get; private set; }

    public static ConnectionEvent OnRequestRPCUpdate = new ConnectionEvent();
    public static ConnectionEvent OnRequestLocalAuthority = new ConnectionEvent();

    //class for passing connections
    public class ConnectionEvent : UnityEvent<PlayerConnection, NetworkInstanceId> { };

    //unityActions
    private UnityAction<PlayerConnection, NetworkInstanceId> playerConnectionUpdateRPCAction;

    // Use this for initialization
    private void Start () {


        //get the manager
        networkManager = GameObject.FindGameObjectWithTag("NetworkManager");

        //name the name in hierarchy
        this.name = "PlayerConnection" + this.GetComponent<NetworkIdentity>().netId.ToString();

        //set the parent for the hierarchy
        this.transform.SetParent(networkManager.transform.Find("PlayerConnections").transform);

        //get the gameManager
        //gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        //playerControlled = gameManager.greenPlayer;

        //this.name = "greenPlayerConnection";
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
    }

    //this function adds event listeners
    private void AddEventListeners()
    {
        //add listener for lobby requesting RPC update
        NetworkLobbyLAN.OnRequestRPCUpdate.AddListener(playerConnectionUpdateRPCAction);
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

    //this function handles a request for Lobby info
    private void ResolveLobbyRequest(PlayerConnection playerConnection, NetworkInstanceId netId)
    {
        //check if this is the player connection and is the local player connection
        if (playerConnection == this && playerConnection.isLocalPlayer == true)
        {
            CmdRequestLocalAuthority(playerConnection.gameObject, netId);
        }
    }

    //this command requests an RPC update for the requested object
    [Command]
    private void CmdRequestRPCUpdate(GameObject playerConnectionGameObject, NetworkInstanceId netId)
    {
        //invoke the event on the server side
        OnRequestRPCUpdate.Invoke(playerConnectionGameObject.GetComponent<PlayerConnection>(), netId);
    }

    //this command requests local authority for the requested object
    [Command]
    private void CmdRequestLocalAuthority(GameObject playerConnectionGameObject, NetworkInstanceId netId)
    {
        //invoke the event on the server side
        OnRequestLocalAuthority.Invoke(playerConnectionGameObject.GetComponent<PlayerConnection>(), netId);
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

        }
    }
}
