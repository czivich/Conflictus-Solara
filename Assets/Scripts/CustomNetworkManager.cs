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

    //unityActions
    private UnityAction<LANConnectionInfo> joinLANGameSetIPAddressAction;

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

    //this function adds the player connection object when a client joins the game
    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        //create the player connection object
        GameObject playerConnection = (GameObject)Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        
        //add the player for connection
        NetworkServer.AddPlayerForConnection(conn, playerConnection, playerControllerId);

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
