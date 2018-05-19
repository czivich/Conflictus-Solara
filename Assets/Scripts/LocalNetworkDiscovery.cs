using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using UnityEngine.Events;

public class LocalNetworkDiscovery : NetworkDiscovery {

    //manager
    private GameObject uiManager;
    private GameObject networkManager;


    //variable for timout duration
    private float timeout = 5.0f;

    //dictionary to hold all game entries
    public Dictionary<LANConnectionInfo, float> lanGames { get; private set; }

    //string for custom game data
    //format to be port,gameName, green alive, greenTaken, red alive, redTaken, purple alive, purpleTaken, blue alive, blueTaken,
    //green planets, red planets, purple planets, blue planets, year
    //example "7777,Y,Y,Y,N,Y,N,Y,N,2,3,2,2,2208"
    //private string gameData;

    //event for announcing discovery of a network game
    public LANGameDiscoveryEvent OnDiscoveredLANGame = new LANGameDiscoveryEvent();

    //event for updating a previously discovered LAN game

    //event for announcin time out of a LAN game
    public LANGameDiscoveryEvent OnLANGameTimedOut = new LANGameDiscoveryEvent();


    //event class for announcing a new game discovery
    public class LANGameDiscoveryEvent : UnityEvent<LANConnectionInfo> { };


    //unityActions
    private UnityAction dedicatedServerAction;
    private UnityAction localHostAction;
    private UnityAction localClientAction;

    //coroutine for checking for games
    private Coroutine discoveryCoroutine;

	// Use this for initialization
	public void Init () {

        /*
        isServer = true;
        base.Initialize();
       // StartAsClient();
        //temp
        gameData = "7777,DogGame,Y,Y,Y,N,Y,N,Y,N,2,3,2,2,2208";

        StartBroadcast();

        */
        // gameData = "7777,DogGame,Y,Y,Y,N,Y,N,Y,N,2,3,2,2,2208";
        //Init(true, true);

        //get the manager
        uiManager = GameObject.FindGameObjectWithTag("UIManager");
        networkManager = GameObject.FindGameObjectWithTag("NetworkManager");

        //set the dictionary
        lanGames = new Dictionary<LANConnectionInfo, float>();

        //set actions
        SetActions();

        //add event listeners
        AddEventListeners();

    }

    //this function sets the unityActions
    private void SetActions()
    {
        dedicatedServerAction = () => { StartDiscovery(true, false); };
        localHostAction = () => { StartDiscovery(true, true); };
        localClientAction = () => { StartDiscovery(false, true); };

    }

    //this function adds event listeners
    private void AddEventListeners()
    {
        //add listener for starting new network game as dedicated server
        networkManager.GetComponent<NetworkInterface>().OnCreateLANGameAsServer.AddListener(dedicatedServerAction);

        //add listener for starting new network game as local host
        networkManager.GetComponent<NetworkInterface>().OnCreateLANGameAsHost.AddListener(localHostAction);

        //add listener for looking to join a lan game
        uiManager.GetComponent<LANGameList>().OnOpenPanel.AddListener(localClientAction);

        //add listener for closing the LANGameList window
        uiManager.GetComponent<LANGameList>().OnClosePanel.AddListener(StopDiscovery);

    }

    //use this for initialization
    public void StartDiscovery(bool initAsServer, bool initAsClient)
    {
        /*
        //check if we are initializing as the server
        if(initAsServer == true)
        {
            //set the isServer flag
            isServer = true;

        }
        else
        {
            //set the isServer flag
            isServer = false;
        }

        //check if we are a client
        if (initAsClient == true)
        {
            //set the isServer flag
            isClient = true;

        }
        else
        {
            //set the isServer flag
            isClient = false;
        }
        */

        //initialize the base class
        base.Initialize();
        /*
        //check if we are a server
        if(isServer == true)
        {
            //start the broadcast
            StartBroadcast();

        }
        else if(isClient == true)
        {
            //start the coroutine
            StartCoroutine(CLeanUpExpiredEntires());
        }

    */

        //check if we are a server
        if(initAsServer == true)
        {
            StartAsServer();
        }

        //check if we are a client
        if (initAsClient == true)
        {
            StartAsClient();

            //start the coroutine
            discoveryCoroutine = StartCoroutine(CLeanUpExpiredEntires());

        }

    }

    //this function starts broadcasting
    private void StartBroadcast()
    {
        //first stop any broadcast that is in progress
        StopBroadcast();

        //set the isServer flag again because StopBroadcast clears it


        //set the broadcast data
        //broadcastData = gameData;

        //call the base initialize function
        base.Initialize();

        //start the base as server
        //base.StartAsServer();

        
    }

    //this function stops discovery activities
    private void StopDiscovery()
    {

        //stop the coroutine
        StopCoroutine(discoveryCoroutine);

        //call the NetworkDiscovery StopBroadcast()
        StopBroadcast();

        //clear the lan games
        lanGames.Clear();

    }

    //this is a coroutine to check for expired entries in the game list
    //i can't do this in update because I can't override Update() because it isn't virtual
    private IEnumerator CLeanUpExpiredEntires()
    {

        //run a continuous loop
        while (true)
        {

            //create a list of dictionary keys
            List<LANConnectionInfo> keys = lanGames.Keys.ToList();

            //loop through the keys
            foreach (LANConnectionInfo key in keys)
            {

                Debug.Log("Coroutine LAN Connection info = " + key.ipAddress + ", " + key.port);

                //check if the value for the key is less than the current time
                if (lanGames[key] <= Time.time)
                {
                    //this key has timed out and should be removed from the dictionary
                    lanGames.Remove(key);

                    //invoke the timed out event
                    OnLANGameTimedOut.Invoke(key);

                }
                                
            }

            //Debug.Log("Coroutine BroadcastData = " + broadcastData.ToString());

            //wait for another timout cycle to run the coroutine again
            yield return new WaitForSeconds(timeout);

        }
        
    }


    //override OnReceivedBroadcast
    public override void OnReceivedBroadcast(string fromAddress, string data)
    {

        //call the base function
        base.OnReceivedBroadcast(fromAddress, data);

        //get the connection info for the received data
        LANConnectionInfo receivedInfo = new LANConnectionInfo(fromAddress, data);

        //check if the receivedInfo is in the dictionary
        if(lanGames.ContainsKey(receivedInfo) == false)
        {
            Debug.Log("received info = " + receivedInfo.ipAddress + ", " + receivedInfo.port);

            //the key is not in the dictionary.  We need to add it
            lanGames.Add(receivedInfo, Time.time + timeout);

            //invoke the discovered game event
            OnDiscoveredLANGame.Invoke(receivedInfo);

        }
        else
        {
            //the else is that the key is already in the dictionary
            //we need to extend the time of that entry
            lanGames[receivedInfo] = Time.time + timeout;

        }

    }

    //this function handles OnDestroy
    private void OnDestroy()
    {
        RemoveEventListeners();
    }

    //this function removes event listeners
    private void RemoveEventListeners()
    {
        if (uiManager != null)
        {

            //remove listener for starting new network game as dedicated server
            networkManager.GetComponent<NetworkInterface>().OnCreateLANGameAsServer.RemoveListener(dedicatedServerAction);

            //remove listener for starting new network game as local host
            networkManager.GetComponent<NetworkInterface>().OnCreateLANGameAsHost.RemoveListener(localHostAction);

            //remove listener for looking to join a lan game
            uiManager.GetComponent<LANGameList>().OnOpenPanel.RemoveListener(localClientAction);

            //remove listener for closing the LANGameList window
            uiManager.GetComponent<LANGameList>().OnClosePanel.RemoveListener(StopDiscovery);

        }

    }
  	

}
