using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Sunburst : MonoBehaviour {

	//variables for the managers
	private GameManager gameManager;

	//variable for the tilemap
	private TileMap tileMap;

	//variable to hold the sunburst damage value
	public static readonly int sunburstDamage = 10;

	//event to announce ship was renamed
	public static SunburstDamageEvent OnSunburstDamageDealt = new SunburstDamageEvent();

	//class for event to announce ship was renamed
	public class SunburstDamageEvent : UnityEvent<CombatUnit, int>{};

	//unityActions
	private UnityAction<Player.Color> playerColorCheckForSunburstDamageAction;

	// Use this for initialization
	public void Init () {

		//get the manager
		gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

		//get the tilemap
		tileMap = GameObject.FindGameObjectWithTag("TileMap").GetComponent<TileMap>();

		//set the action
		playerColorCheckForSunburstDamageAction = (playerColor) => {CheckForSunburstDamage(playerColor);};

		//add listener for end turn event
		gameManager.OnEndTurn.AddListener(playerColorCheckForSunburstDamageAction);
		
	}

	//this function checks if the unit ended the turn in a sunburst zone
	private void CheckForSunburstDamage(Player.Color playerColor){

		//check if there is a ship component on the gameobject - there should be
		if (this.GetComponent<Ship> () == true) {

			//check if the ship is owned by the player color passed to the function
			if (this.GetComponent<Ship> ().owner.color == playerColor) {

				//check if the ship is in a sun ray tile
				if (tileMap.HexMap [this.GetComponent<Ship> ().currentLocation].tileType == HexMapTile.TileType.SunRay) {

					//if this ship is in a sunray tile, invoke the event
					OnSunburstDamageDealt.Invoke (this.GetComponent<CombatUnit> (), sunburstDamage);

				}

			}

		}

	}

	//this function handles OnDestroy
	private void OnDestroy(){

		RemoveAllListeners ();

	}

	//this function removes all listeners
	private void RemoveAllListeners(){

		//remove listener for end turn event
		if (gameManager != null) {
			
			gameManager.OnEndTurn.RemoveListener (playerColorCheckForSunburstDamageAction);

		}

	}

}
