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

    //variables to hold the local server/client state
    //private bool localIsServer;
    //private bool localIsClient;
    //private LANConnectionInfo localLANConnectionInfo;

    //variable for timout duration
    private float timeout = 5.0f;

    //dictionary to hold all game entries
    public Dictionary<LANConnectionInfo, float> lanGames { get; private set; }

    //string for custom game data
    //format to be ipAddress, port,gameName, teams, green alive, greenTaken, red alive, redTaken, purple alive, purpleTaken, blue alive,
    //blueTaken, green planets, red planets, purple planets, blue planets, year
    //example "192.168.1.75, 7777:false:true:true:true:false:true:false:true:false:2:3:2:2:2208:"
    
    //event for announcing discovery of a network game
    public LANGameDiscoveryEvent OnDiscoveredLANGame = new LANGameDiscoveryEvent();

    //event for updating a previously discovered LAN game
    public LANGameDiscoveryEvent OnReceivedUpdateToLANGame = new LANGameDiscoveryEvent();

    //event for announcing time out of a LAN game
    public LANGameDiscoveryEvent OnLANGameTimedOut = new LANGameDiscoveryEvent();


    //event class for announcing a new game discovery
    public class LANGameDiscoveryEvent : UnityEvent<LANConnectionInfo> { };


    //unityActions
    private UnityAction<LANConnectionInfo> dedicatedServerAction;
    private UnityAction<LANConnectionInfo> localHostAction;
    private UnityAction localClientAction;

    //coroutine for checking for games
    private Coroutine discoveryCoroutine;

    //placeholder connection info
    LANConnectionInfo placeholderConnectionInfo;

	// Use this for initialization
	public void Init () {

        //get the manager
        uiManager = GameObject.FindGameObjectWithTag("UIManager");
        networkManager = GameObject.FindGameObjectWithTag("NetworkManager");

        //set the dictionary
        lanGames = new Dictionary<LANConnectionInfo, float>();

        //set the placeholder
        placeholderConnectionInfo = new LANConnectionInfo("", 0, "", false,false, false, false, false, false, false, false, false, 0, 0, 0, 0, 0, 0);

        //set actions
        SetActions();

        //add event listeners
        AddEventListeners();

    }

    //this function sets the unityActions
    private void SetActions()
    {
        dedicatedServerAction = (connectionInfo) => { StartDiscovery(true, false, connectionInfo); };
        localHostAction = (connectionInfo) => { StartDiscovery(true, true, connectionInfo); };
        localClientAction = () => { StartDiscovery(false, true, placeholderConnectionInfo); };

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

        //add listeners for lobby updates
        NetworkLobbyLAN.OnUpdateGameName.AddListener(UpdateBroadcastMessageFromLobbyInfo);
        NetworkLobbyLAN.OnUpdateTeamsEnabled.AddListener(UpdateBroadcastMessageFromLobbyInfo);
        NetworkLobbyLAN.OnUpdateVictoryPlanets.AddListener(UpdateBroadcastMessageFromLobbyInfo);
        NetworkLobbyLAN.OnUpdateGameYear.AddListener(UpdateBroadcastMessageFromLobbyInfo);
        NetworkLobbyLAN.OnUpdateGreenPlayerName.AddListener(UpdateBroadcastMessageFromLobbyInfo);
        NetworkLobbyLAN.OnUpdateRedPlayerName.AddListener(UpdateBroadcastMessageFromLobbyInfo);
        NetworkLobbyLAN.OnUpdatePurplePlayerName.AddListener(UpdateBroadcastMessageFromLobbyInfo);
        NetworkLobbyLAN.OnUpdateBluePlayerName.AddListener(UpdateBroadcastMessageFromLobbyInfo);
        NetworkLobbyLAN.OnUpdateGreenPlayerPlanets.AddListener(UpdateBroadcastMessageFromLobbyInfo);
        NetworkLobbyLAN.OnUpdateRedPlayerPlanets.AddListener(UpdateBroadcastMessageFromLobbyInfo);
        NetworkLobbyLAN.OnUpdatePurplePlayerPlanets.AddListener(UpdateBroadcastMessageFromLobbyInfo);
        NetworkLobbyLAN.OnUpdateBluePlayerPlanets.AddListener(UpdateBroadcastMessageFromLobbyInfo);
        NetworkLobbyLAN.OnUpdateGreenPlayerReady.AddListener(UpdateBroadcastMessageFromLobbyInfo);
        NetworkLobbyLAN.OnUpdateRedPlayerReady.AddListener(UpdateBroadcastMessageFromLobbyInfo);
        NetworkLobbyLAN.OnUpdatePurplePlayerReady.AddListener(UpdateBroadcastMessageFromLobbyInfo);
        NetworkLobbyLAN.OnUpdateBluePlayerReady.AddListener(UpdateBroadcastMessageFromLobbyInfo);
        NetworkLobbyLAN.OnUpdateGreenPlayerConnection.AddListener(UpdateBroadcastMessageFromLobbyInfo);
        NetworkLobbyLAN.OnUpdateRedPlayerConnection.AddListener(UpdateBroadcastMessageFromLobbyInfo);
        NetworkLobbyLAN.OnUpdatePurplePlayerConnection.AddListener(UpdateBroadcastMessageFromLobbyInfo);
        NetworkLobbyLAN.OnUpdateBluePlayerConnection.AddListener(UpdateBroadcastMessageFromLobbyInfo);
        NetworkLobbyLAN.OnUpdateGreenPlayerAlive.AddListener(UpdateBroadcastMessageFromLobbyInfo);
        NetworkLobbyLAN.OnUpdateRedPlayerAlive.AddListener(UpdateBroadcastMessageFromLobbyInfo);
        NetworkLobbyLAN.OnUpdatePurplePlayerAlive.AddListener(UpdateBroadcastMessageFromLobbyInfo);
        NetworkLobbyLAN.OnUpdateBluePlayerAlive.AddListener(UpdateBroadcastMessageFromLobbyInfo);

}

    //use this for initialization
    public void StartDiscovery(bool initAsServer, bool initAsClient, LANConnectionInfo connectionInfo)
    {

        //cache the local info in case we need to restart discovery
        //localIsClient = initAsClient;
        //localIsServer = initAsServer;
        //localLANConnectionInfo = connectionInfo;
        
        //initialize the base class
        base.Initialize();

        //check if we are a server
        if(initAsServer == true)
        {

            //set the broadcast data based on the passed connection info
            broadcastData = connectionInfo.broadcastDataString;

            StartAsServer();

            Debug.Log("broadcastData = " + broadcastData);

            StartCoroutine(BroadcastMessage());
        }

        //check if we are a client
        else if (initAsClient == true)
        {
            StartAsClient();

            //start the coroutine
            discoveryCoroutine = StartCoroutine(CLeanUpExpiredEntires());

        }

    }

    private IEnumerator BroadcastMessage()
    {

        while (true)
        {
            Debug.Log("broadcastData = " + broadcastData);

            yield return new WaitForSeconds(timeout);
        }
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
        if (lanGames.ContainsKey(receivedInfo) == false)
        {

            //flag to see if we find an update
            bool foundUpdate = false;

            //key to edit if we find an update
            LANConnectionInfo keyToUpdate = receivedInfo;

            //check if this is an update of an existing game
            //we will consider it the same game if the ipAddress, port, and name are all the same
            foreach (KeyValuePair<LANConnectionInfo, float> entry in lanGames)
            {
                if (entry.Key.ipAddress == receivedInfo.ipAddress 
                    && entry.Key.port == receivedInfo.port
                    && entry.Key.gameName == receivedInfo.gameName)
                {
                    //we have an existing game for this ipAddress, port, and name
                    //invoke the update event
                    OnReceivedUpdateToLANGame.Invoke(receivedInfo);

                    //store this entry
                    keyToUpdate = entry.Key;

                    //set the found update flag
                    foundUpdate = true;
                }
            }

            //check if we found an update
            if (foundUpdate == true)
            {
                //if we found an update, we want to remove the old entry from the dictionary and add the new one
                lanGames.Remove(keyToUpdate);

                //add the new entry
                lanGames.Add(receivedInfo, Time.time + timeout);
            }
            else
            {
               
                Debug.Log("received info from address = " + receivedInfo.ipAddress + ", " + receivedInfo.port);
                Debug.Log("received data = " + data);
                //the key is not in the dictionary.  We need to add it
                lanGames.Add(receivedInfo, Time.time + timeout);

                //invoke the discovered game event
                OnDiscoveredLANGame.Invoke(receivedInfo);

            }

        }
        else
        {
            //the else is that the key is already in the dictionary
            //we need to extend the time of that entry
            lanGames[receivedInfo] = Time.time + timeout;

        }

    }

    //this function updates the broadcast data from the network lobby
    private void UpdateBroadcastMessageFromLobbyInfo()
    {
        //check if we are the server
        if(isServer == true)
        {

            //define the taken states
            bool greenPlayerTaken;
            bool redPlayerTaken;
            bool purplePlayerTaken;
            bool bluePlayerTaken;

            if (networkManager.GetComponentInChildren<NetworkLobbyLAN>().greenPlayerConnection == null)
            {
                greenPlayerTaken = false;
            }
            else
            {
                greenPlayerTaken = true;

            }

            if (networkManager.GetComponentInChildren<NetworkLobbyLAN>().redPlayerConnection == null)
            {
                redPlayerTaken = false;
            }
            else
            {
                redPlayerTaken = true;

            }

            if (networkManager.GetComponentInChildren<NetworkLobbyLAN>().purplePlayerConnection == null)
            {
                purplePlayerTaken = false;
            }
            else
            {
                purplePlayerTaken = true;

            }

            if (networkManager.GetComponentInChildren<NetworkLobbyLAN>().bluePlayerConnection == null)
            {
                bluePlayerTaken = false;
            }
            else
            {
                bluePlayerTaken = true;

            }

            //update the broadcast message
            broadcastData = new LANConnectionInfo(
                NetworkManager.singleton.networkAddress.ToString(),
                NetworkManager.singleton.networkPort,
                networkManager.GetComponentInChildren<NetworkLobbyLAN>().gameName,
                networkManager.GetComponentInChildren<NetworkLobbyLAN>().teamsEnabled,
                networkManager.GetComponentInChildren<NetworkLobbyLAN>().greenPlayerAlive,
                greenPlayerTaken,
                networkManager.GetComponentInChildren<NetworkLobbyLAN>().redPlayerAlive,
                redPlayerTaken,
                networkManager.GetComponentInChildren<NetworkLobbyLAN>().purplePlayerAlive,
                purplePlayerTaken,
                networkManager.GetComponentInChildren<NetworkLobbyLAN>().bluePlayerAlive,
                bluePlayerTaken,
                networkManager.GetComponentInChildren<NetworkLobbyLAN>().greenPlayerPlanets,
                networkManager.GetComponentInChildren<NetworkLobbyLAN>().redPlayerPlanets,
                networkManager.GetComponentInChildren<NetworkLobbyLAN>().purplePlayerPlanets,
                networkManager.GetComponentInChildren<NetworkLobbyLAN>().bluePlayerPlanets,
                networkManager.GetComponentInChildren<NetworkLobbyLAN>().gameYear,
                networkManager.GetComponentInChildren<NetworkLobbyLAN>().victoryPlanets
                ).broadcastDataString;

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

        //remove listeners for lobby updates
        NetworkLobbyLAN.OnUpdateGameName.RemoveListener(UpdateBroadcastMessageFromLobbyInfo);
        NetworkLobbyLAN.OnUpdateTeamsEnabled.RemoveListener(UpdateBroadcastMessageFromLobbyInfo);
        NetworkLobbyLAN.OnUpdateVictoryPlanets.RemoveListener(UpdateBroadcastMessageFromLobbyInfo);
        NetworkLobbyLAN.OnUpdateGameYear.RemoveListener(UpdateBroadcastMessageFromLobbyInfo);
        NetworkLobbyLAN.OnUpdateGreenPlayerName.RemoveListener(UpdateBroadcastMessageFromLobbyInfo);
        NetworkLobbyLAN.OnUpdateRedPlayerName.RemoveListener(UpdateBroadcastMessageFromLobbyInfo);
        NetworkLobbyLAN.OnUpdatePurplePlayerName.RemoveListener(UpdateBroadcastMessageFromLobbyInfo);
        NetworkLobbyLAN.OnUpdateBluePlayerName.RemoveListener(UpdateBroadcastMessageFromLobbyInfo);
        NetworkLobbyLAN.OnUpdateGreenPlayerPlanets.RemoveListener(UpdateBroadcastMessageFromLobbyInfo);
        NetworkLobbyLAN.OnUpdateRedPlayerPlanets.RemoveListener(UpdateBroadcastMessageFromLobbyInfo);
        NetworkLobbyLAN.OnUpdatePurplePlayerPlanets.RemoveListener(UpdateBroadcastMessageFromLobbyInfo);
        NetworkLobbyLAN.OnUpdateBluePlayerPlanets.RemoveListener(UpdateBroadcastMessageFromLobbyInfo);
        NetworkLobbyLAN.OnUpdateGreenPlayerReady.RemoveListener(UpdateBroadcastMessageFromLobbyInfo);
        NetworkLobbyLAN.OnUpdateRedPlayerReady.RemoveListener(UpdateBroadcastMessageFromLobbyInfo);
        NetworkLobbyLAN.OnUpdatePurplePlayerReady.RemoveListener(UpdateBroadcastMessageFromLobbyInfo);
        NetworkLobbyLAN.OnUpdateBluePlayerReady.RemoveListener(UpdateBroadcastMessageFromLobbyInfo);
        NetworkLobbyLAN.OnUpdateGreenPlayerConnection.RemoveListener(UpdateBroadcastMessageFromLobbyInfo);
        NetworkLobbyLAN.OnUpdateRedPlayerConnection.RemoveListener(UpdateBroadcastMessageFromLobbyInfo);
        NetworkLobbyLAN.OnUpdatePurplePlayerConnection.RemoveListener(UpdateBroadcastMessageFromLobbyInfo);
        NetworkLobbyLAN.OnUpdateBluePlayerConnection.RemoveListener(UpdateBroadcastMessageFromLobbyInfo);
        NetworkLobbyLAN.OnUpdateGreenPlayerAlive.RemoveListener(UpdateBroadcastMessageFromLobbyInfo);
        NetworkLobbyLAN.OnUpdateRedPlayerAlive.RemoveListener(UpdateBroadcastMessageFromLobbyInfo);
        NetworkLobbyLAN.OnUpdatePurplePlayerAlive.RemoveListener(UpdateBroadcastMessageFromLobbyInfo);
        NetworkLobbyLAN.OnUpdateBluePlayerAlive.RemoveListener(UpdateBroadcastMessageFromLobbyInfo);

    }
  	
}
