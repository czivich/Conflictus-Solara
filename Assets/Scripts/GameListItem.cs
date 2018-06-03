using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class GameListItem : MonoBehaviour {

    //manager
    private GameObject networkManager;

    //variables for list item elements
    public Button gameSelectButton;
    public TextMeshProUGUI teamStatus;
    public Image greenPlayerDot;
    public Image redPlayerDot;
    public Image purplePlayerDot;
    public Image bluePlayerDot;
    public TextMeshProUGUI greenPlayerPlanets;
    public TextMeshProUGUI redPlayerPlanets;
    public TextMeshProUGUI purplePlayerPlanets;
    public TextMeshProUGUI bluePlayerPlanets;
    public TextMeshProUGUI victoryPlanets;
    public TextMeshProUGUI currentYear;

    //variables that define the game state
    public LANConnectionInfo lanConnectionInfo { get; private set; }

    //variables to hold colors
    private Color32 greenColorAvailable = new Color32(0, 64, 0, 255);
    private Color32 redColorAvailable = new Color32(64, 0, 0, 255);
    private Color32 purpleColorAvailable = new Color32(38, 17, 64, 255);
    private Color32 blueColorAvailable = new Color32(0, 32, 64, 255);

    private Color32 greenColorTaken = new Color32(0, 255, 0, 255);
    private Color32 redColorTaken = new Color32(255, 0, 0, 255);
    private Color32 purpleColorTaken = new Color32(153, 51, 255, 255);
    private Color32 blueColorTaken = new Color32(0, 128, 255, 255);

    private Color32 playerIsDeadColor = new Color32(0, 0, 0, 255);

    //event for clicking the game button
    public static ConnectionEvent OnJoinLANGame = new ConnectionEvent();

    //event class for passing lan connection info
    public class ConnectionEvent : UnityEvent<LANConnectionInfo>{ };

    //unityActions
    private UnityAction<LANConnectionInfo> connectionUpdateAction;

    // Use this for initialization
    public void Init(LANConnectionInfo connectionInfo)
    {
        //get the manager
        networkManager = GameObject.FindGameObjectWithTag("NetworkManager");

        //set the connection info
        SetConnectionInfo(connectionInfo);

        //set the UI Display
        SetUIDisplay();

        //set the player connection status
        SetPlayerConnectionStatus();

        //set actions
        SetActions();

        //add listeners
        AddEventListeners();

    }

    //this function sets the unityActions
    private void SetActions()
    {
        connectionUpdateAction = (connectionInfo) => { UpdateGameListItem(connectionInfo); };
    }

    //this function adds event listeners
    private void AddEventListeners()
    {
        //add listener for receiving an update
        networkManager.GetComponent<LocalNetworkDiscovery>().OnReceivedUpdateToLANGame.AddListener(connectionUpdateAction);

        //add listener for clicking the game button
        gameSelectButton.onClick.AddListener(JoinGame);
    }

    //this function sets the connection info
    private void SetConnectionInfo(LANConnectionInfo connectionInfo)
    {
        this.lanConnectionInfo = connectionInfo;
    }

    //this function sets the game data
    private void SetUIDisplay()
    {
        //set the button text to match the game name
        gameSelectButton.GetComponentInChildren<TextMeshProUGUI>().text = lanConnectionInfo.gameName;

        //set the teams enabled text to match the connection info
        if(lanConnectionInfo.teamsEnabled == true)
        {
            teamStatus.text = "Yes";
        }
        else
        {
            teamStatus.text = "No";
        }

        //set the player dots based on alive status and connection status
        SetPlayerConnectionStatus();

        //set the planet counts
        greenPlayerPlanets.text = lanConnectionInfo.greenPlayerPlanets.ToString();
        redPlayerPlanets.text = lanConnectionInfo.redPlayerPlanets.ToString();
        purplePlayerPlanets.text = lanConnectionInfo.purplePlayerPlanets.ToString();
        bluePlayerPlanets.text = lanConnectionInfo.bluePlayerPlanets.ToString();

        //set the victory planets
        victoryPlanets.text = lanConnectionInfo.victoryPlanets.ToString();

        //set the game year
        currentYear.text = lanConnectionInfo.gameYear.ToString();

    }

    //this function sets the player connection status
    private void SetPlayerConnectionStatus()
    {
        //set the player dots based on alive status and connection status
        if (lanConnectionInfo.greenPlayerIsAlive == true)
        {
            if (lanConnectionInfo.greenPlayerIsTaken == true)
            {
                //color the dot brightly
                greenPlayerDot.color = greenColorTaken;
            }
            else
            {
                //color the dot darkly
                greenPlayerDot.color = greenColorAvailable;
            }
        }
        else
        {
            //color the dot black
            greenPlayerDot.color = playerIsDeadColor;
        }

        //set the player dots based on alive status and connection status
        if (lanConnectionInfo.redPlayerIsAlive == true)
        {
            if (lanConnectionInfo.redPlayerIsTaken == true)
            {
                //color the dot brightly
                redPlayerDot.color = redColorTaken;
            }
            else
            {
                //color the dot darkly
                redPlayerDot.color = redColorAvailable;
            }
        }
        else
        {
            //color the dot black
            redPlayerDot.color = playerIsDeadColor;
        }

        //set the player dots based on alive status and connection status
        if (lanConnectionInfo.purplePlayerIsAlive == true)
        {
            if (lanConnectionInfo.purplePlayerIsTaken == true)
            {
                //color the dot brightly
                purplePlayerDot.color = purpleColorTaken;
            }
            else
            {
                //color the dot darkly
                purplePlayerDot.color = purpleColorAvailable;
            }
        }
        else
        {
            //color the dot black
            purplePlayerDot.color = playerIsDeadColor;
        }

        //set the player dots based on alive status and connection status
        if (lanConnectionInfo.bluePlayerIsAlive == true)
        {
            if (lanConnectionInfo.bluePlayerIsTaken == true)
            {
                //color the dot brightly
                bluePlayerDot.color = blueColorTaken;
            }
            else
            {
                //color the dot darkly
                bluePlayerDot.color = blueColorAvailable;
            }
        }
        else
        {
            //color the dot black
            bluePlayerDot.color = playerIsDeadColor;
        }
    }
    
    //this function updates the GameListItem based on new connection info
    private void UpdateGameListItem(LANConnectionInfo newConnectionInfo)
    {
        //check if this game list item matches the ipAddress, port, and game name
        if(this.lanConnectionInfo.ipAddress == newConnectionInfo.ipAddress &&
            this.lanConnectionInfo.port == newConnectionInfo.port &&
            this.lanConnectionInfo.gameName == newConnectionInfo.gameName)
        {
            //update this game list item to match the updated connection info
            //set the connection info
            SetConnectionInfo(newConnectionInfo);

            //set the UI Display
            SetUIDisplay();

            //set the player connection status
            SetPlayerConnectionStatus();

        }

    }

    //this function joins a game
    private void JoinGame()
    {
    
        //invoke the event
        OnJoinLANGame.Invoke(this.lanConnectionInfo);
    }
    
    //this function handles onDestroy
    private void OnDestroy()
    {
        RemoveEventListeners();
    }

    //this function removes event listeners
    private void RemoveEventListeners()
    {
        if (networkManager != null)
        {
            //remove listener for receiving an update
            networkManager.GetComponent<LocalNetworkDiscovery>().OnReceivedUpdateToLANGame.RemoveListener(connectionUpdateAction);

        }

        if(gameSelectButton != null)
        {
            //remove listener for clicking the game button
            gameSelectButton.onClick.RemoveListener(JoinGame);
        }
    }
}
