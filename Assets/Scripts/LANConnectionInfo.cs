using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct LANConnectionInfo {

    //variables
    public string ipAddress { get; private set; }
    public int port { get; private set; }
    public string gameName { get; private set; }
    public bool teamsEnabled { get; private set; }

    public bool greenPlayerIsAlive { get; private set; }
    public bool greenPlayerIsTaken { get; private set; }
    public bool redPlayerIsAlive { get; private set; }
    public bool redPlayerIsTaken { get; private set; }
    public bool purplePlayerIsAlive { get; private set; }
    public bool purplePlayerIsTaken { get; private set; }
    public bool bluePlayerIsAlive { get; private set; }
    public bool bluePlayerIsTaken { get; private set; }
    public int greenPlayerPlanets { get; private set; }
    public int redPlayerPlanets { get; private set; }
    public int purplePlayerPlanets { get; private set; }
    public int bluePlayerPlanets { get; private set; }
    public int victoryPlanets { get; private set; }
    public int gameYear { get; private set; }

    public string broadcastDataString { get; private set; }


    //contructor for using received broadcast data
    public LANConnectionInfo(string fromAddress, string data)
    {
        Debug.Log("LANConnection data string = " + data);

        //temporary variable to hold the parts of the fromAddress string
        string[] fromAddressParts = fromAddress.Split(new char[] { ':' });

        //the string at [3] is the ipAddress
        ipAddress = fromAddressParts[3];

        //temporary variable to hold the parts of the data string
        string[] dataParts = data.Split(new char[] { ':' });

        //set the variables from the data string
        port = System.Int32.Parse(dataParts[1]);

        gameName = dataParts[2];

        if (dataParts[3] == "true")
        {
            teamsEnabled = true;
        }
        else
        {
            teamsEnabled = false;
        }

        if (dataParts[4] == "true")
        {
            greenPlayerIsAlive = true;
        }
        else
        {
            greenPlayerIsAlive = false;
        }

        if (dataParts[5] == "true")
        {
            greenPlayerIsTaken = true;
        }
        else
        {
            greenPlayerIsTaken = false;
        }

        if (dataParts[6] == "true")
        {
            redPlayerIsAlive = true;
        }
        else
        {
            redPlayerIsAlive = false;
        }

        if (dataParts[7] == "true")
        {
            redPlayerIsTaken = true;
        }
        else
        {
            redPlayerIsTaken = false;
        }

        if (dataParts[8] == "true")
        {
            purplePlayerIsAlive = true;
        }
        else
        {
            purplePlayerIsAlive = false;
        }

        if (dataParts[9] == "true")
        {
            purplePlayerIsTaken = true;
        }
        else
        {
            purplePlayerIsTaken = false;
        }

        if (dataParts[10] == "true")
        {
            bluePlayerIsAlive = true;
        }
        else
        {
            bluePlayerIsAlive = false;
        }

        if (dataParts[11] == "true")
        {
            bluePlayerIsTaken = true;
        }
        else
        {
            bluePlayerIsTaken = false;

        }

        greenPlayerPlanets = System.Int32.Parse(dataParts[12]);
        redPlayerPlanets = System.Int32.Parse(dataParts[13]);
        purplePlayerPlanets = System.Int32.Parse(dataParts[14]);
        bluePlayerPlanets = System.Int32.Parse(dataParts[15]);

        victoryPlanets = System.Int32.Parse(dataParts[16]);
        gameYear = System.Int32.Parse(dataParts[17]);

        this.broadcastDataString = data;

    }

    //this constructor is for populating from a newly created game
    public LANConnectionInfo(string newIPAddress, int newPort, string newGameName, bool newTeamsEnabled, bool newGreenPlayerIsAlive,
        bool newGreenPlayerIsTaken, bool newRedPlayerIsAlive, bool newRedPlayerIsTaken, bool newPurplePlayerIsAlive,
        bool newPurplePlayerIsTaken, bool newBluePlayerIsAlive, bool newBluePlayerIsTaken, int newGreenPlayerPlanets,
        int newRedPlayerPlanets, int newPurplePlayerPlanets, int newBluePlayerPlanets, int newVictoryPlanets, int newGameYear)
    {

        this.ipAddress = newIPAddress;
        this.port = newPort;
        this.gameName = newGameName;
        this.teamsEnabled = newTeamsEnabled;
        this.greenPlayerIsAlive = newGreenPlayerIsAlive;
        this.greenPlayerIsTaken = newGreenPlayerIsTaken;
        this.redPlayerIsAlive = newRedPlayerIsAlive;
        this.redPlayerIsTaken = newRedPlayerIsTaken;
        this.purplePlayerIsAlive = newPurplePlayerIsAlive;
        this.purplePlayerIsTaken = newPurplePlayerIsTaken;
        this.bluePlayerIsAlive = newBluePlayerIsAlive;
        this.bluePlayerIsTaken = newBluePlayerIsTaken;
        this.greenPlayerPlanets = newGreenPlayerPlanets;
        this.redPlayerPlanets = newRedPlayerPlanets;
        this.purplePlayerPlanets = newPurplePlayerPlanets;
        this.bluePlayerPlanets = newBluePlayerPlanets;
        this.gameYear = newGameYear;
        this.victoryPlanets = newVictoryPlanets;

        this.broadcastDataString = ipAddress + ":" +
            port.ToString() + ":" +
            gameName + ":" +
            teamsEnabled.ToString().ToLowerInvariant() + ":" +
            greenPlayerIsAlive.ToString().ToLowerInvariant() + ":" +
            greenPlayerIsTaken.ToString().ToLowerInvariant() + ":" +
            redPlayerIsAlive.ToString().ToLowerInvariant() + ":" +
            redPlayerIsTaken.ToString().ToLowerInvariant() + ":" +
            purplePlayerIsAlive.ToString().ToLowerInvariant() + ":" +
            purplePlayerIsTaken.ToString().ToLowerInvariant() + ":" +
            bluePlayerIsAlive.ToString().ToLowerInvariant() + ":" +
            bluePlayerIsTaken.ToString().ToLowerInvariant() + ":" +
            greenPlayerPlanets.ToString() + ":" +
            redPlayerPlanets.ToString() + ":" +
            purplePlayerPlanets.ToString() + ":" +
            bluePlayerPlanets.ToString() + ":" +
            victoryPlanets.ToString() + ":" +
            gameYear.ToString() + ":";

        Debug.Log("string created = " + broadcastDataString);

    }

}
