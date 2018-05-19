using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameListItem : MonoBehaviour {

    //manager
    private GameObject uiManager;

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
    public TextMeshProUGUI currentYear;

    //variables that define the game state
    public LANConnectionInfo lanConnectionInfo { get; private set; }
    public string gameName { get; private set; }
    public bool teamsEnabled { get; private set; }
    public bool greenPlayerIsAlive { get; private set; }
    public bool greenPlayerIsConnected { get; private set; }
    public bool redPlayerIsAlive { get; private set; }
    public bool redlayerIsConnected { get; private set; }
    public bool purplePlayerIsAlive { get; private set; }
    public bool purplePlayerIsConnected { get; private set; }
    public bool bluePlayerIsAlive { get; private set; }
    public bool bluePlayerIsConnected { get; private set; }
    public int greenPlayerPlanetCount { get; private set; }
    public int redPlayerPlanetCount { get; private set; }
    public int purplePlayerPlanetCount { get; private set; }
    public int bluePlayerPlanetCount { get; private set; }
    public int gameYear { get; private set; }



    //variables to hold colors
    private Color32 greenColorAvailable = new Color32(0, 255, 0, 255);
    private Color32 redColorAvailable = new Color32(255, 0, 0, 255);
    private Color32 purpleColorAvailable = new Color32(153, 51, 255, 255);
    private Color32 blueColorAvailable = new Color32(0, 128, 255, 255);

    private Color32 greenColorTaken = new Color32(0, 64, 0, 255);
    private Color32 redColorTaken = new Color32(64, 0, 0, 255);
    private Color32 purpleColorTaken = new Color32(38, 17, 64, 255);
    private Color32 blueColorTaken = new Color32(0, 32, 64, 255);

    private Color32 playerIsDead = new Color32(0, 0, 0, 255);

    // Use this for initialization
    public void Init(LANConnectionInfo connectionInfo)
    {
        //get the manager
        uiManager = GameObject.FindGameObjectWithTag("UIManager");

        //set the connection info
        SetConnectionInfo(connectionInfo);

        //set the gameData
        //SetGameData(gameData);

        //set the player connection status
        SetPlayerConnectionStatus();

    }

    //this function adds event listeners
    private void AddEventListeners()
    {

    }

    //this function sets the connection info
    private void SetConnectionInfo(LANConnectionInfo connectionInfo)
    {
        this.lanConnectionInfo = connectionInfo;
    }

    //this function sets the game data
    private void SetGameData(GameListItemGameData gameData)
    {
        this.gameName = gameData.gameName;
        this.teamsEnabled = gameData.teamsEnabled;
        this.gameYear = gameData.gameYear;
        this.greenPlayerIsAlive = gameData.greenPlayerIsAlive;
        this.redPlayerIsAlive = gameData.redPlayerIsAlive;
        this.purplePlayerIsAlive = gameData.purplePlayerIsAlive;
        this.bluePlayerIsAlive = gameData.bluePlayerIsAlive;
        this.greenPlayerPlanetCount = gameData.greenPlayerPlanetCount;
        this.redPlayerPlanetCount = gameData.redPlayerPlanetCount;
        this.purplePlayerPlanetCount = gameData.purplePlayerPlanetCount;
        this.bluePlayerPlanetCount = gameData.bluePlayerPlanetCount;
        this.gameYear = gameData.gameYear;

    }

    //this function sets the player connection status
    private void SetPlayerConnectionStatus()
    {

    }
    
    
    //this function handles onDestroy
    private void OnDestroy()
    {
        RemoveEventListeners();
    }

    //this function removes event listeners
    private void RemoveEventListeners()
    {

    }
}
