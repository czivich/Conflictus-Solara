using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;

public class CustomNetworkManager : NetworkManager {

    //manager
    private GameObject uiManager;
    private GameObject networkManager;

    //gameobject parent for connections
    public GameObject playerConnectionParent;

    public static ConnectionEvent OnPlayerDisconnecting = new ConnectionEvent();

    //class for passing connections
    public class ConnectionEvent : UnityEvent<PlayerConnection, NetworkInstanceId> { };

    //unityActions
    private UnityAction<LANConnectionInfo> joinLANGameSetIPAddressAction;

    private float timer;
    private GameObject disconnectObject;
    private NetworkConnection disconnectConn;

	// Use this for initialization
	public void Init () {

        //get the manager
        uiManager = GameObject.FindGameObjectWithTag("UIManager");
        networkManager = GameObject.FindGameObjectWithTag("NetworkManager");

        //set actions
        SetActions();

        //add event listeners
        AddEventListeners();

    }

    private void Update()
    {
        if(timer > 0f)
        {
            timer -= Time.deltaTime;

            if(timer <= 0)
            {
                Debug.Log(disconnectObject.name + " disconnecting");
                //invoke the disconnecting event
                OnPlayerDisconnecting.Invoke(disconnectObject.GetComponent<PlayerConnection>(),
                    disconnectObject.GetComponent<PlayerConnection>().netId);

                NetworkServer.DestroyPlayersForConnection(disconnectConn);
                if (disconnectConn.lastError != NetworkError.Ok)
                {
                    if (LogFilter.logError) { Debug.LogError("ServerDisconnected due to error: " + disconnectConn.lastError); }
                }
            }

        }
    }

    //this function sets unity Actions
    private void SetActions()
    {
        joinLANGameSetIPAddressAction = (connectionInfo) => { SetIPAddressToNetworkConnection(connectionInfo); };
    }

    //this function adds event listeners
    private void AddEventListeners()
    {
        //add listener for opening the lan game window
        uiManager.GetComponent<NewLANGameWindow>().OnOpenPanel.AddListener(SetIPAddressToLocal);

        //add listener for joining a lan game as client
        networkManager.GetComponent<NetworkInterface>().OnJoinLANGameAsClient.AddListener(joinLANGameSetIPAddressAction);
    }

    //this function sets the local ip address
    private void SetIPAddressToLocal()
    {
        //set the network ipAddress to this local computer's address
        networkAddress = GetLocalIPAddress();
        Debug.Log("IP Address = " + networkAddress);
    }

    //this function sets the ipAddress to a networked computer
    private void SetIPAddressToNetworkConnection(LANConnectionInfo connectionInfo)
    {
        //set the network ipAddress to the address in the connection info
        networkAddress = connectionInfo.ipAddress;
        networkPort = connectionInfo.port;
        Debug.Log("IP Address = " + networkAddress);

    }

    //this function overrides startHost
    public override NetworkClient StartHost()
    {
        NetworkClient clientToReturn = base.StartHost();

        SetIPAddressToLocal();

        return clientToReturn;

    }

    //this function adds the player connection object when a client joins the game
    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        //create the player connection object
        GameObject playerConnection = (GameObject)Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        
        //add the player for connection
        NetworkServer.AddPlayerForConnection(conn, playerConnection, playerControllerId);

    }

    //this function removes the player connection object when a client leaves the game
    public override void OnServerRemovePlayer(NetworkConnection conn, PlayerController player)
    {

        Debug.Log("OnServerRemovePlayer");

        //call the base function
        base.OnServerRemovePlayer(conn, player);



    }

    //this function overrides onClientConnect
    
    public override void OnClientConnect(NetworkConnection conn)
    {
        //right now nothing is changed - but this is where I can pass a message in the ClientScene.AddPlayer call

        if (!clientLoadedScene)
        {
            // Ready/AddPlayer is usually triggered by a scene load completing. if no scene was loaded, then Ready/AddPlayer it here instead.
            ClientScene.Ready(conn);
            if (this.autoCreatePlayer)
            {
                ClientScene.AddPlayer(0);
            }
        }
    }
    

    //this function overrides the server disconnect
    public override void OnServerDisconnect(NetworkConnection conn)
    {

        
        Debug.Log("OnServerDisconnect");

        //loop through the playerControllers
        for (int i = 0; i <  conn.playerControllers.Count; i++)
        {
            //check if the controller has a playerConnection
            if(conn.playerControllers[i].gameObject.GetComponent<PlayerConnection>() == true)
            {
                /*
                Debug.Log(conn.playerControllers[i].gameObject.name + " disconnecting");
                //invoke the disconnecting event
                OnPlayerDisconnecting.Invoke(conn.playerControllers[i].gameObject.GetComponent<PlayerConnection>(),
                    conn.playerControllers[i].gameObject.GetComponent<PlayerConnection>().netId);
                */
                Debug.Log("Disconnect Timer Starting!!!");
                timer = 30f;
                disconnectObject = conn.playerControllers[i].gameObject;

            }
            
        }

        //call the base function
        //base.OnServerDisconnect(conn);

        //this is the base function:
        /*
        NetworkServer.DestroyPlayersForConnection(conn);
        if (conn.lastError != NetworkError.Ok)
        {
            if (LogFilter.logError) { Debug.LogError("ServerDisconnected due to error: " + conn.lastError); }
        }
        */
        disconnectConn = conn;

    }

    //this function callback is for when a client stops a connection
    public override void OnStopClient()
    {
        //call the base function
        base.OnStopClient();


    }

    //this function gets the local IP adress
    public static string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        Debug.LogError("No network adapters with an IPv4 address in the system!");
        return null;
    }


    //this function handles onDestroy
    private void OnDestroy()
    {
        RemoveEventListeners();
    }

    //this function removes event listeners
    private void RemoveEventListeners()
    {
        if (uiManager != null)
        {
            //remove listener for opening the lan game window
            uiManager.GetComponent<NewLANGameWindow>().OnOpenPanel.RemoveListener(SetIPAddressToLocal);
        }

        if(networkManager != null)
        {
            //remove listener for joining a lan game as client
            networkManager.GetComponent<NetworkInterface>().OnJoinLANGameAsClient.RemoveListener(joinLANGameSetIPAddressAction);
        }
    }

}
