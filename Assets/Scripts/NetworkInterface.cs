using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Events;

public class NetworkInterface : MonoBehaviour {

    //variable for the networkManager
    public CustomNetworkManager customNetworkManager;

    //variable for SpawnManager prefab
    public GameObject spawnManagerPrefab;

    //variable for the created spawn Manager
    private NetworkSpawnManager spawnManager;

    //manager
    private GameObject uiManager;

    //events
    public ConnectionEvent OnCreateLANGameAsHost = new ConnectionEvent();
    public ConnectionEvent OnCreateLANGameAsServer = new ConnectionEvent();
    public ConnectionEvent OnJoinLANGameAsClient = new ConnectionEvent();

    //class for passing connections
    public class ConnectionEvent : UnityEvent<LANConnectionInfo> { };

    //unityActions
    private UnityAction<LANConnectionInfo> newLANGameCreatedAction;
    private UnityAction<LANConnectionInfo> joinedLANGameAction;

    // Use this for initialization
    public void Init () {

        //get the manager
        uiManager = GameObject.FindGameObjectWithTag("UIManager");

        //set actions
        SetActions();

        //add event listeners
        AddEventListeners();
		
	}

    //this function sets the unityActions
    private void SetActions()
    {
        newLANGameCreatedAction = (connectionInfo) => { ResolveCreateLANGame(connectionInfo); };
        joinedLANGameAction = (connectionInfo) => { ResolveJoinLANGame(connectionInfo); };
    }

    //this function adds event listeners
    private void AddEventListeners()
    {
        //add listener for creating LAN game as host
        uiManager.GetComponent<NewLANGameWindow>().OnCreateNewLANGame.AddListener(newLANGameCreatedAction);

        //add listener for clicking a join LAN Game button
        GameListItem.OnJoinLANGame.AddListener(joinedLANGameAction);

    }

    //this function resolves the create LAN game click
    private void ResolveCreateLANGame(LANConnectionInfo connectionInfo)
    {

        //check if the lan game configuration is host
        if (uiManager.GetComponent<NewLANGameWindow>().localHost == true)
        {

            //we are configured to host
            CreateLANGameAsHost();

            //invoke the event
            OnCreateLANGameAsHost.Invoke(connectionInfo);

            //create a spawn manager
            spawnManager = Instantiate(spawnManagerPrefab, customNetworkManager.transform).GetComponent<NetworkSpawnManager>();

            NetworkServer.Spawn(spawnManager.gameObject);

            //run Init
            spawnManager.Init();
        }
        else
        {
            //the else condition is that localHost is false, so we are a dedicated server
            CreateLANGameAsServer();

            //invoke the event
            OnCreateLANGameAsServer.Invoke(connectionInfo);

            //create a spawn manager
            spawnManager = Instantiate(spawnManagerPrefab, customNetworkManager.transform).GetComponent<NetworkSpawnManager>();

            NetworkServer.Spawn(spawnManager.gameObject);

            //run Init
            spawnManager.Init();

        }
        
    }

    //this function resolves a join LAN game click
    private void ResolveJoinLANGame(LANConnectionInfo connectionInfo)
    {

        //invoke the event
        OnJoinLANGameAsClient.Invoke(connectionInfo);

        //connect as client
        customNetworkManager.StartClient();

    }

    //this function creates the LAN game as a host
    private void CreateLANGameAsHost()
    {

        //call the startHost function
        customNetworkManager.StartHost();

    }

    //this function creates the LAN game as a server
    private void CreateLANGameAsServer()
    {

        //call the startHost function
        customNetworkManager.StartServer();

    }

    //this function handles OnDestroy
    private void OnDestroy()
    {
        RemoveEventListeners();   
    }

    //this function removes all event listeners
    private void RemoveEventListeners()
    {
        if (uiManager != null)
        {
            //remove listener for creating LAN game as host
            uiManager.GetComponent<NewLANGameWindow>().OnCreateNewLANGame.RemoveListener(newLANGameCreatedAction);

        }

        //remove listener for clicking a join LAN Game button
        GameListItem.OnJoinLANGame.RemoveListener(joinedLANGameAction);

    }

}
