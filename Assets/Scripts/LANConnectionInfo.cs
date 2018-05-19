using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct LANConnectionInfo {

    //variables
    public string ipAddress;
    public int port;
    //public string gameName;

    /*
    public bool greenPlayerIsAlive;
    public bool greenPlayerIsTaken;
    public bool redPlayerIsAlive;
    public bool redPlayerIsTaken;
    public bool purplePlayerIsAlive;
    public bool purplePlayerIsTaken;
    public bool bluePlayerIsAlive;
    public bool bluePlayerIsTaken;
    public int greenPlayerPlanets;
    public int redPlayerPlanets;
    public int purplePlayerPlanets;
    public int bluePlayerPlanets;
    public int gameYear;
    */

    //contructor
    public LANConnectionInfo(string fromAddress, string data)
    {
        //temporary variable to hold the parts of the fromAddress string
        string[] fromAddressParts = fromAddress.Split(new char[] { ':' });

        //the string at [3] is the ipAddress
        ipAddress = fromAddressParts[3];

        //temporary variable to hold the parts of the data string
        string[] dataParts = data.Split(new char[] { ':' });

        //set the variables from the data string
        System.Int32.TryParse(dataParts[2], out port);

        //gameName = dataParts[1];
        /*
        if (dataParts[2] == "Y")
        {
            greenPlayerIsAlive = true;
        }
        else
        {
            greenPlayerIsAlive = false;
        }

        if (dataParts[3] == "Y")
        {
            greenPlayerIsTaken = true;
        }
        else
        {
            greenPlayerIsTaken = false;
        }

        if (dataParts[4] == "Y")
        {
            redPlayerIsAlive = true;
        }
        else
        {
            redPlayerIsAlive = false;
        }

        if (dataParts[5] == "Y")
        {
            redPlayerIsTaken = true;
        }
        else
        {
            redPlayerIsTaken = false;
        }

        if (dataParts[6] == "Y")
        {
            purplePlayerIsAlive = true;
        }
        else
        {
            purplePlayerIsAlive = false;
        }

        if (dataParts[7] == "Y")
        {
            purplePlayerIsTaken = true;
        }
        else
        {
            purplePlayerIsTaken = false;
        }

        if (dataParts[8] == "Y")
        {
            bluePlayerIsAlive = true;
        }
        else
        {
            bluePlayerIsAlive = false;
        }

        if (dataParts[9] == "Y")
        {
            bluePlayerIsTaken = true;
        }
        else
        {
            bluePlayerIsTaken = false;

        }

        greenPlayerPlanets = System.Int32.Parse(dataParts[10]);
        redPlayerPlanets = System.Int32.Parse(dataParts[11]);
        purplePlayerPlanets = System.Int32.Parse(dataParts[12]);
        bluePlayerPlanets = System.Int32.Parse(dataParts[13]);

        gameYear = System.Int32.Parse(dataParts[14]);
        */
    }

}
