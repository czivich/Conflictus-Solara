using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Events;

public class NetworkInterface : MonoBehaviour {

    //variable for the networkManager
    public NetworkManager networkManager;

    //manager
    private GameObject uiManager;

    //events
    public ConnectionEvent OnCreateLANGameAsHost = new ConnectionEvent();
    public ConnectionEvent OnCreateLANGameAsServer = new ConnectionEvent();

    //class for passing connections
    public class ConnectionEvent : UnityEvent<LANConnectionInfo> { };

    //unityActions
    private UnityAction<LANConnectionInfo> newLANGameCreatedAction;

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
    }

    //this function adds event listeners
    private void AddEventListeners()
    {
        //add listener for creating LAN game as host
        uiManager.GetComponent<NewLANGameWindow>().OnCreateNewLANGame.AddListener(newLANGameCreatedAction);

    }

    //this function resolves the create LAN game click
    private void ResolveCreateLANGame(LANConnectionInfo connectionInfo)
    {

        Debug.Log(networkManager.networkAddress.ToString());

        //check if the lan game configuration is host
        if (uiManager.GetComponent<NewLANGameWindow>().localHost == true)
        {

            //we are configured to host
            CreateLANGameAsHost();

            //invoke the event
            OnCreateLANGameAsHost.Invoke(connectionInfo);

        }
        else
        {
            //the else condition is that localHost is false, so we are a dedicated server
            CreateLANGameAsServer();

            //invoke the event
            OnCreateLANGameAsServer.Invoke(connectionInfo);

        }
        
    }

    //this function creates the LAN game as a host
    private void CreateLANGameAsHost()
    {

        //call the startHost function
        networkManager.StartHost();

    }

    //this function creates the LAN game as a server
    private void CreateLANGameAsServer()
    {

        //call the startHost function
        networkManager.StartServer();

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

    }

}
