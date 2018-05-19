using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameListItemGameData {

    //variables that define the game state
    public string gameName { get; private set; }
    public bool teamsEnabled { get; private set; }
    public bool greenPlayerIsAlive { get; private set; }
    public bool redPlayerIsAlive { get; private set; }
    public bool purplePlayerIsAlive { get; private set; }
    public bool bluePlayerIsAlive { get; private set; }
    public int greenPlayerPlanetCount { get; private set; }
    public int redPlayerPlanetCount { get; private set; }
    public int purplePlayerPlanetCount { get; private set; }
    public int bluePlayerPlanetCount { get; private set; }
    public int gameYear { get; private set; }

    //function to get the GameListItemData from a SaveGameData
    public static GameListItemGameData GameListItemGameDataFromSaveGameData(FileManager.SaveGameData saveGameData)
    {

        GameListItemGameData newGameListItemGameData = new GameListItemGameData();

        newGameListItemGameData.gameName = saveGameData.currentFileName;
        newGameListItemGameData.teamsEnabled = saveGameData.teamsEnabled;
        newGameListItemGameData.gameYear = saveGameData.gameYear;

        //loop through the player colors to find the matching color index
        for (int i = 0; i < GameManager.numberPlayers; i++)
        {

            //switch case on the color at this index
            switch (saveGameData.playerColor[i])
            {
                case Player.Color.Green:

                    newGameListItemGameData.greenPlayerIsAlive = saveGameData.playerIsAlive[i];
                    newGameListItemGameData.greenPlayerPlanetCount = saveGameData.playerPlanets[i];
                    break;

                case Player.Color.Red:

                    newGameListItemGameData.redPlayerIsAlive = saveGameData.playerIsAlive[i];
                    newGameListItemGameData.redPlayerPlanetCount = saveGameData.playerPlanets[i];
                    break;

                case Player.Color.Purple:

                    newGameListItemGameData.purplePlayerIsAlive = saveGameData.playerIsAlive[i];
                    newGameListItemGameData.purplePlayerPlanetCount = saveGameData.playerPlanets[i];
                    break;

                case Player.Color.Blue:

                    newGameListItemGameData.bluePlayerIsAlive = saveGameData.playerIsAlive[i];
                    newGameListItemGameData.bluePlayerPlanetCount = saveGameData.playerPlanets[i];
                    break;

                default:

                    break;

            }

        }

        return newGameListItemGameData;

    }

}
