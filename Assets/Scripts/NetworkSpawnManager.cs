using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;

public class NetworkSpawnManager : NetworkBehaviour {
    
    //manager
    private GameObject uiManager;
    private GameObject networkManager;

    //gameObject for network lobby prefab
    public GameObject networkLobbyLANPrefab;
    private GameObject networkLobbyObject;
    

    //unity actions

    // Use this for initialization
    public void Init () {

        //get the manager
        uiManager = GameObject.FindGameObjectWithTag("UIManager");
        networkManager = GameObject.FindGameObjectWithTag("NetworkManager");
        
        //set the name
        this.name = "NetworkSpawnManager";

        //set actions
        SetActions();

        //add event listeners
        AddEventListeners();

        //spawn the lobby object
        SpawnNetworkLobbyLANObject();

    }


    //this function sets unityActions
    private void SetActions()
    {

    }

    //this function adds event listeners
    private void AddEventListeners()
    {

    }

    //this function spawns the networkLobbyLAN object
    private void SpawnNetworkLobbyLANObject()
    {

        //check if this is the server
        if (isServer == true)
        {
            //instantiate the lobby object on the server
            networkLobbyObject = Instantiate(networkLobbyLANPrefab);

            //spawn the object
            //NetworkServer.SpawnWithClientAuthority(networkLobbyObject,connectionToClient);
            NetworkServer.Spawn(networkLobbyObject);

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

    }
}
