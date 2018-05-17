using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NetworkInterface : MonoBehaviour {

    //variable for the networkManager
    public NetworkManager networkManager;

    //manager
    private GameObject uiManager;

	// Use this for initialization
	public void Init () {

        //get the manager
        uiManager = GameObject.FindGameObjectWithTag("UIManager");

        //add event listeners
        AddEventListeners();
		
	}

    //this function adds event listeners
    private void AddEventListeners()
    {

        //add listener for creating LAN game as host
        uiManager.GetComponent<NewLANGameWindow>().OnCreateNewLANGame.AddListener(ResolveCreateLANGame);

    }

    //this function resolves the create LAN game click
    private void ResolveCreateLANGame()
    {

        //set the network address
        networkManager.networkAddress = uiManager.GetComponent<NewLANGameWindow>().roomName;

        Debug.Log(networkManager.networkAddress.ToString());

        //check if the lan game configuration is host
        if (uiManager.GetComponent<NewLANGameWindow>().localHost == true)
        {

            //we are configured to host
            CreateLANGameAsHost();

        }
        else
        {
            //the else condition is that localHost is false, so we are a dedicated server
            CreateLANGameAsServer();

        }
        
    }

    //this function creates the LAN game as a host
    private void CreateLANGameAsHost()
    {

        //call the startHost function
        networkManager.StartHost();

        //set the network address
        networkManager.networkAddress = uiManager.GetComponent<NewLANGameWindow>().roomName;

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
            uiManager.GetComponent<NewLANGameWindow>().OnCreateNewLANGame.RemoveListener(ResolveCreateLANGame);

        }

    }

}
