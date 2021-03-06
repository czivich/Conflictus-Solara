using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ColonyManager : MonoBehaviour {

	//managers
	private GameManager gameManager;

	//prefab for colony object
	public Colony prefabColony;

	//parent object in hierarchy
	private GameObject coloniesParent;

	//tilemap
	private TileMap tileMap;

	//dictionary to keep track of who owns which planet
	public Dictionary<string,Player> planetOwners {

		get;
		private set;

	}

	//event to annonce when the planet dictionary has been updated
	public static PlanetUpdateEvent OnPlanetOwnerChanged = new PlanetUpdateEvent();

	public class PlanetUpdateEvent : UnityEvent<string,Player>{}; 

	//create a vector3 offset so that the colony is above the planet but below units
	private static Vector3 mapOffset = new Vector3(0.0f, 0.005f, 0.0f);

	//unityActions
	private UnityAction<HexMapTile,Player.Color> tileColonizedUpdatePlanetDictionaryAction;

	// Use this for initialization
	public void Init () {

		//set the action
		tileColonizedUpdatePlanetDictionaryAction = (hexMapTile,color) => {UpdatePlanetDictionary(hexMapTile,color);};

		//get the parent object
		coloniesParent = GameObject.Find("Colonies");

		//get the gameManager
		gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

		//get the tileMap
		tileMap = GameObject.FindGameObjectWithTag("TileMap").GetComponent<TileMap>();

		//loop through the tileMap HexMap dictionary
		foreach (KeyValuePair<Hex,HexMapTile> entry in tileMap.HexMap) {

			//check if the entry is a planet
			if (entry.Value.isPlanet == true) {

				//create a colony where the planet is
				CreateColony(entry.Value);

			}

		}

		planetOwners = new Dictionary<string, Player>();

		//populate the dictionary
		planetOwners.Add("Mercury",null);
		planetOwners.Add("Venus",null);
		planetOwners.Add("Earth",null);
		planetOwners.Add("Mars",null);
		planetOwners.Add("Jupiter",null);
		planetOwners.Add("Saturn",null);
		planetOwners.Add("Uranus",null);
		planetOwners.Add("Neptune",null);
		planetOwners.Add("Pluto",null);
		planetOwners.Add("Charon",null);
        planetOwners.Add("Eris", null);
        planetOwners.Add("Ceres", null);

        //add listener for settling a new colony
        HexMapTile.OnColonizeNewPlanet.AddListener(tileColonizedUpdatePlanetDictionaryAction);

	}

	private void CreateColony(HexMapTile hexMapTile){

		//cache data about the prefab and tile passed to the function
		Hex hexLocation = hexMapTile.hexLocation;
		TileMap tileMap = hexMapTile.tileMap;
		Layout layout = tileMap.layout;

		//convert the hex location to world coordinates to instantiate the unit at
		Vector3 unitWorldCoordinatesV3 = tileMap.HexToWorldCoordinates(hexLocation) + ColonyManager.mapOffset;

		Colony newColony;
		newColony = Instantiate (prefabColony, unitWorldCoordinatesV3, Quaternion.identity);

		newColony.Init (hexMapTile);

		//set the parent object in the hierarchy
		newColony.transform.parent = coloniesParent.transform;

		//scale the colony
		SetColonySize(newColony,layout);

	}

	//this method adjusts the colony size to match the scale of the map tiles
	private static void SetColonySize(Colony newColony, Layout layout){

		Vector3 colonyScale = new Vector3 (layout.size.x * 2.0f, 1.0f, layout.size.y * 2.0f);
		newColony.transform.localScale = colonyScale;

	}

	//this method updates the planet dictionary
	private void UpdatePlanetDictionary(HexMapTile hexMapTile, Player.Color playerColor){

		string planetString;

		//use a switch case to convert hexMapTile to planet string
		switch (hexMapTile.tileType) {

		case HexMapTile.TileType.Mercury:
			planetString = "Mercury";
			planetOwners [planetString] = gameManager.GetPlayerFromColor (playerColor);
			break;

		case HexMapTile.TileType.MercuryLow:
			planetString = "Mercury";
			planetOwners [planetString] = gameManager.GetPlayerFromColor (playerColor);
			break;

		case HexMapTile.TileType.Venus:
			planetString = "Venus";
			planetOwners [planetString] = gameManager.GetPlayerFromColor (playerColor);
			break;

		case HexMapTile.TileType.VenusLow:
			planetString = "Venus";
			planetOwners [planetString] = gameManager.GetPlayerFromColor (playerColor);
			break;

		case HexMapTile.TileType.Earth:
			planetString = "Earth";
			planetOwners [planetString] = gameManager.GetPlayerFromColor (playerColor);
			break;

		case HexMapTile.TileType.EarthLow:
			planetString = "Earth";
			planetOwners [planetString] = gameManager.GetPlayerFromColor (playerColor);
			break;

		case HexMapTile.TileType.Mars:
			planetString = "Mars";
			planetOwners [planetString] = gameManager.GetPlayerFromColor (playerColor);
			break;

		case HexMapTile.TileType.MarsLow:
			planetString = "Mars";
			planetOwners [planetString] = gameManager.GetPlayerFromColor (playerColor);
			break;

		case HexMapTile.TileType.Jupiter:
			planetString = "Jupiter";
			planetOwners [planetString] = gameManager.GetPlayerFromColor (playerColor);
			break;

		case HexMapTile.TileType.JupiterLow:
			planetString = "Jupiter";
			planetOwners [planetString] = gameManager.GetPlayerFromColor (playerColor);
			break;

		case HexMapTile.TileType.Saturn:
			planetString = "Saturn";
			planetOwners [planetString] = gameManager.GetPlayerFromColor (playerColor);
			break;

		case HexMapTile.TileType.SaturnLow:
			planetString = "Saturn";
			planetOwners [planetString] = gameManager.GetPlayerFromColor (playerColor);
			break;

		case HexMapTile.TileType.Uranus:
			planetString = "Uranus";
			planetOwners [planetString] = gameManager.GetPlayerFromColor (playerColor);
			break;

		case HexMapTile.TileType.UranusLow:
			planetString = "Uranus";
			planetOwners [planetString] = gameManager.GetPlayerFromColor (playerColor);
			break;

		case HexMapTile.TileType.Neptune:
			planetString = "Neptune";
			planetOwners [planetString] = gameManager.GetPlayerFromColor (playerColor);
			break;

		case HexMapTile.TileType.NeptuneLow:
			planetString = "Neptune";
			planetOwners [planetString] = gameManager.GetPlayerFromColor (playerColor);
			break;

		case HexMapTile.TileType.Pluto:
			planetString = "Pluto";
			planetOwners [planetString] = gameManager.GetPlayerFromColor (playerColor);
			break;

		case HexMapTile.TileType.PlutoLow:
			planetString = "Pluto";
			planetOwners [planetString] = gameManager.GetPlayerFromColor (playerColor);
			break;

		case HexMapTile.TileType.Charon:
			planetString = "Charon";
			planetOwners [planetString] = gameManager.GetPlayerFromColor (playerColor);
			break;

		case HexMapTile.TileType.CharonLow:
			planetString = "Charon";
			planetOwners [planetString] = gameManager.GetPlayerFromColor (playerColor);
			break;

        case HexMapTile.TileType.Eris:
            planetString = "Eris";
            planetOwners[planetString] = gameManager.GetPlayerFromColor(playerColor);
            break;

        case HexMapTile.TileType.ErisLow:
            planetString = "Eris";
            planetOwners[planetString] = gameManager.GetPlayerFromColor(playerColor);
            break;

        case HexMapTile.TileType.Ceres:
            planetString = "Ceres";
            planetOwners[planetString] = gameManager.GetPlayerFromColor(playerColor);
            break;

        case HexMapTile.TileType.CeresLow:
            planetString = "Ceres";
            planetOwners[planetString] = gameManager.GetPlayerFromColor(playerColor);
            break;

            default:
			planetString = "BLANK";
			break;

		}

		//invoke the planet update event
		OnPlanetOwnerChanged.Invoke(planetString,gameManager.GetPlayerFromColor (playerColor));

	}

	//this public function checks how many planets a player controls
	public int PlanetsControlledByPlayer(Player player){

		int numberPlanets = 0;

		//iterate through planet dictionary
		foreach (KeyValuePair<string,Player> entry in planetOwners) {

			//check if the player value matches the player passed to the function
			if (entry.Value == player) {

				//increment the number of planets
				numberPlanets++;

			}

		}

		return numberPlanets;

	}

	//function to remove listeners on destroy
	private void OnDestroy(){
		
		RemoveAllListeners ();

	}

	//function to remove all listeners
	private void RemoveAllListeners(){

		//remove listener for settling a new colony
		HexMapTile.OnColonizeNewPlanet.RemoveListener(tileColonizedUpdatePlanetDictionaryAction);

	}

}
