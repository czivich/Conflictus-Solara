using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class NextUnit : MonoBehaviour {

	//this is the button that will trigger going to the next unit
	//make it public so we can hook it up in the inspector
	public Button nextUnitButton;
	public Button previousUnitButton;

	//variables for managers
	private GameManager gameManager;
	private MouseManager mouseManager;

	//I will need access to the TileMap
	private TileMap tileMap;

	//variable to hold the last known stored currentPlayer
	private Player mostRecentPlayer;

	//variable to hold the player collector object
	private GameObject currentPlayerCollector;

	//variables to hold the 4 player collectors
	private GameObject greenCollector;
	private GameObject purpleCollector;
	private GameObject redCollector;
	private GameObject blueCollector;

	//variable to hold the unit index
	private int unitIndex;

	//event for selecting the next unit
	public NextUnitEvent SetNextUnit = new NextUnitEvent();

	//simple class derived from unityEvent to pass GameObject
	public class NextUnitEvent : UnityEvent<GameObject>{};

	//unityActions
	private UnityAction<Player> playerCenterCameraOnUnitAction;
	private UnityAction<GameManager.ActionMode> actionModeSetNextUnitButtonStatusAction;


	// Use this for initialization
	public void Init () {

		//get the managers
		gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
		mouseManager = GameObject.FindGameObjectWithTag("MouseManager").GetComponent<MouseManager>();

		//get the collectors
		greenCollector = GameObject.Find ("GreenUnits");
		purpleCollector = GameObject.Find ("PurpleUnits");
		redCollector = GameObject.Find ("RedUnits");
		blueCollector = GameObject.Find ("BlueUnits");

		//get the tilemap
		tileMap = GameObject.FindGameObjectWithTag("TileMap").GetComponent<TileMap>();

		//set the actions
		playerCenterCameraOnUnitAction = (player) => {CenterCameraOnUnit(GetCurrentPlayerFirstUnit());};
		actionModeSetNextUnitButtonStatusAction = (actionMode) => {SetNextUnitButtonStatus(actionMode);};

		//set up a listener for when the nextUnitButton is pushed
		nextUnitButton.onClick.AddListener(GetNextUnit);

		// add a listener for when the previousUnitButton is pressed
		previousUnitButton.onClick.AddListener(GetPreviousUnit);

		//set up a listener for when a new turn is starting
		//even though the event includes the player, we don't need it for our get next unit function
		gameManager.OnNewTurn.AddListener(playerCenterCameraOnUnitAction);
		gameManager.OnLoadedTurn.AddListener(playerCenterCameraOnUnitAction);

		//add a listener for actionMode changes
		gameManager.OnActionModeChange.AddListener(actionModeSetNextUnitButtonStatusAction);

	}

	//this is the function that will snap the selected unit to the next available unit
	private void GetNextUnit(){

		//to start, we want to grab the current player
		Player currentPlayer = gameManager.currentTurnPlayer;

		//create an array of units
		CombatUnit[] currentPlayerUnits;

		//check if the current player has changed since the last call
		//if so, we need to re-establish the array of unit
		if (currentPlayer != mostRecentPlayer) {

			//we can now set the mostRecentPlayer to current player
			mostRecentPlayer = currentPlayer;

			//if this is a new player, we can reset the unitIndex
			unitIndex = 0;

			//next, we want to create an array of all combat units owned by that player
			//start by getting the game object collector for the current player
			currentPlayerCollector = GetCurrentPlayerCollector();

			//get a count of how many children units there are under the collector
			int numberOfUnits = currentPlayerCollector.transform.childCount;

			//create an array of units
			currentPlayerUnits = new CombatUnit[numberOfUnits];

			//initialize the array
			for (int i = 0; i < numberOfUnits; i++) {

				//make sure the game object is a CombatUnit (it should be)
				if (currentPlayerCollector.transform.GetChild (i).gameObject.GetComponent<CombatUnit> () == true) {

					currentPlayerUnits [i] = currentPlayerCollector.transform.GetChild (i).gameObject.GetComponent<CombatUnit> ();

				}

			}
				
		}
			

		//the else condition is that we called the function and it is the same player as the mostRecentPlayer
		else {

			//get a count of how many children units there are under the collector
			int numberOfUnits = currentPlayerCollector.transform.childCount;

			//create an array of units
			currentPlayerUnits = new CombatUnit[numberOfUnits];

			//initialize the array
			for (int i = 0; i < numberOfUnits; i++) {

				//make sure the game object is a CombatUnit (it should be)
				if (currentPlayerCollector.transform.GetChild (i).gameObject.GetComponent<CombatUnit> () == true) {

					currentPlayerUnits [i] = currentPlayerCollector.transform.GetChild (i).gameObject.GetComponent<CombatUnit> ();

				}

			}

			//increment the unit index
			//check if incrementing it will exceed the array size - if so, cycle the index back to 0
			if (unitIndex + 1 == currentPlayerUnits.Length) {
				
				unitIndex = 0;

			} else {

				unitIndex++;

			}

		}

		//we want pushing the button to go to the next unit if we currently have the unitIndex unit selected already
		//but only if there are units
		if (currentPlayerUnits.Length > 0) {
			
			if (mouseManager.selectedUnit == currentPlayerUnits [unitIndex].gameObject) {

				//increment the unit index
				//check if incrementing it will exceed the array size - if so, cycle the index back to 0
				if (unitIndex + 1 == currentPlayerUnits.Length) {

					unitIndex = 0;

				} else {

					unitIndex++;

				}

			}

		}

			
		//make sure current player units is not null
		//but only if there are units
		if (currentPlayerUnits.Length > 0) {
			
			if (currentPlayerUnits [unitIndex] != null) {

				//we should have everything we need to select units now
				//set the selected unit equal to the unit at the unit index

				//invoke the SetNextUnit event, passing it the next game object for the current player
				//doing this as an event allows me to keep mouseManager.SetSelectedUnit as a private method
				SetNextUnit.Invoke (currentPlayerUnits [unitIndex].gameObject);

				//set the main camera to snap to the selected unit current hex location
				CenterCameraOnUnit (currentPlayerUnits [unitIndex]);

				/*
				//if the hex cursor is null, grab it
				if (hexCursor == null) {

					hexCursor = GameObject.FindGameObjectWithTag("HexCursor").GetComponent<HexCursor>();

				}

				//set the hexCursor position to the selected unit location
				hexCursor.transform.position = new Vector3(tileMap.HexToWorldCoordinates (currentPlayerUnits [unitIndex].currentLocation).x,
					hexCursor.transform.position.y,
					tileMap.HexToWorldCoordinates (currentPlayerUnits [unitIndex].currentLocation).z);

				*/
			}

		}

	}

	//this is the function that will snap the selected unit to the next available unit, cycling backwards
	private void GetPreviousUnit(){

		//to start, we want to grab the current player
		Player currentPlayer = gameManager.currentTurnPlayer;

		//create an array of units
		CombatUnit[] currentPlayerUnits;

		//check if the current player has changed since the last call
		//if so, we need to re-establish the array of unit
		if (currentPlayer != mostRecentPlayer) {

			//we can now set the mostRecentPlayer to current player
			mostRecentPlayer = currentPlayer;

			//if this is a new player, we can reset the unitIndex
			unitIndex = 0;

			//next, we want to create an array of all combat units owned by that player
			//start by getting the game object collector for the current player
			currentPlayerCollector = GetCurrentPlayerCollector();

			//get a count of how many children units there are under the collector
			int numberOfUnits = currentPlayerCollector.transform.childCount;

			//create an array of units
			currentPlayerUnits = new CombatUnit[numberOfUnits];

			//initialize the array
			for (int i = 0; i < numberOfUnits; i++) {

				//make sure the game object is a CombatUnit (it should be)
				if (currentPlayerCollector.transform.GetChild (i).gameObject.GetComponent<CombatUnit> () == true) {

					currentPlayerUnits [i] = currentPlayerCollector.transform.GetChild (i).gameObject.GetComponent<CombatUnit> ();

				}

			}

		}


		//the else condition is that we called the function and it is the same player as the mostRecentPlayer
		else {

			//get a count of how many children units there are under the collector
			int numberOfUnits = currentPlayerCollector.transform.childCount;

			//create an array of units
			currentPlayerUnits = new CombatUnit[numberOfUnits];

			//initialize the array
			for (int i = 0; i < numberOfUnits; i++) {

				//make sure the game object is a CombatUnit (it should be)
				if (currentPlayerCollector.transform.GetChild (i).gameObject.GetComponent<CombatUnit> () == true) {

					currentPlayerUnits [i] = currentPlayerCollector.transform.GetChild (i).gameObject.GetComponent<CombatUnit> ();

				}

			}

			//decrement the unit index
			//check if decrementing it will drop it below zero - if it will, set it back to the length
			if (unitIndex == 0) {

				unitIndex = currentPlayerUnits.Length - 1;

			} else {

				unitIndex--;

			}

		}

		//we want pushing the button to go to the previous unit if we currently have the unitIndex unit selected already
		//but only if there are units
		if (currentPlayerUnits.Length > 0) {
			
			if (mouseManager.selectedUnit == currentPlayerUnits [unitIndex].gameObject) {

				//decrement the unit index
				//check if decrementing it will drop it below zero - if it will, set it back to the length
				if (unitIndex == 0) {

					unitIndex = currentPlayerUnits.Length - 1;

				} else {

					unitIndex--;

				}

			}

		}


		//make sure current player units is not null
		//but only if there are units
		if (currentPlayerUnits.Length > 0) {
			if (currentPlayerUnits [unitIndex] != null) {

				//we should have everything we need to select units now
				//set the selected unit equal to the unit at the unit index

				//invoke the SetNextUnit event, passing it the next game object for the current player
				//doing this as an event allows me to keep mouseManager.SetSelectedUnit as a private method
				SetNextUnit.Invoke (currentPlayerUnits [unitIndex].gameObject);

				//set the main camera to snap to the selected unit current hex location
				CenterCameraOnUnit (currentPlayerUnits [unitIndex]);

					/*
				//if the hex cursor is null, grab it
				if (hexCursor == null) {

					hexCursor = GameObject.FindGameObjectWithTag("HexCursor").GetComponent<HexCursor>();

				}

				//set the hexCursor position to the selected unit location
				hexCursor.transform.position = new Vector3(tileMap.HexToWorldCoordinates (currentPlayerUnits [unitIndex].currentLocation).x,
					hexCursor.transform.position.y,
					tileMap.HexToWorldCoordinates (currentPlayerUnits [unitIndex].currentLocation).z);

				*/
			}

		}

	}

	//create function to center camera on unit
	private void CenterCameraOnUnit(CombatUnit combatUnit){

		//Debug.Log ("CenterCameraOnUnit");

		//check if we have a unit passed
		if (combatUnit != null) {

			//set the camera limits
			//these are calculated based on the size of the map so that the camera can only show 1 hex beyond the board in any direction
			float cameraMaxZ = tileMap.maxHeight + tileMap.origin.y - Camera.main.orthographicSize;
			float cameraMinZ = tileMap.origin.y + Camera.main.orthographicSize - tileMap.hexHeight;
			float cameraMaxX = tileMap.maxWidth + tileMap.origin.x - Camera.main.orthographicSize * Camera.main.aspect;
			float cameraMinX = tileMap.origin.x + (Camera.main.orthographicSize * Camera.main.aspect) - tileMap.hexWidth;


			//set the main camera to snap to the selected unit current hex location
			Camera.main.transform.position = new Vector3 (Mathf.Clamp (tileMap.HexToWorldCoordinates (combatUnit.currentLocation).x, cameraMinX, cameraMaxX),
				Mathf.Clamp (Camera.main.transform.position.y, 10.0f, 10.0f),
				Mathf.Clamp (tileMap.HexToWorldCoordinates (combatUnit.currentLocation).z, cameraMinZ, cameraMaxZ));

		}

	}

	//create a function to get the current player's first unit
	private CombatUnit GetCurrentPlayerFirstUnit(){

		//get the first unit of the starting player
		GameObject playerCollector = GetCurrentPlayerCollector();

		if (playerCollector.transform.childCount > 0) {

			return playerCollector.transform.GetChild (0).gameObject.GetComponent<CombatUnit> ();

		} else {

			return null;

		}

	}

	//this function returns the current turn player collector
	private GameObject GetCurrentPlayerCollector(){

		//get the first unit of the starting player
		GameObject playerCollector;

		if (gameManager.currentTurnPlayer.color == Player.Color.Blue) {

			playerCollector = blueCollector;

		} else if (gameManager.currentTurnPlayer.color == Player.Color.Green) {

			playerCollector = greenCollector;

		} else if (gameManager.currentTurnPlayer.color == Player.Color.Purple) {

			playerCollector = purpleCollector;

		} else if (gameManager.currentTurnPlayer.color == Player.Color.Red) {

			playerCollector = redCollector;

		} else {

			playerCollector = null;

		}

		//make sure that somehow it's not null
		if (playerCollector != null) {

			return playerCollector;

		}

		else{

			Debug.LogError ("Player Collector was null.");
			return null;

		}

	}

	//this function sets the nextUnit button status
	//it has since been updated to also update the previousUnitButton status
	private void SetNextUnitButtonStatus(GameManager.ActionMode actionMode){

		//check if we are in a locked action mode
		if (UIManager.lockMenuActionModes.Contains (actionMode)) {

			nextUnitButton.interactable = false;
			previousUnitButton.interactable = false;

		} else {

			//if the mode isn't locked, the button can be interactable
			nextUnitButton.interactable = true;
			previousUnitButton.interactable = true;


		}

	}

	//function to handle OnDestroy
	private void OnDestroy(){

		RemoveAllListeners ();

	}

	//function to remove all listeners
	private void RemoveAllListeners(){

		if (nextUnitButton != null) {
			
			//remove listener for when the nextUnitButton is pushed
			nextUnitButton.onClick.RemoveListener (GetNextUnit);

		}

		if (previousUnitButton != null) {
			
			//remove a listener for when the previousUnitButton is pressed
			previousUnitButton.onClick.RemoveListener (GetPreviousUnit);

		}

		if (gameManager != null) {
			
			//remove listener for when a new turn is starting
			//even though the event includes the player, we don't need it for our get next unit function
			gameManager.OnNewTurn.RemoveListener (playerCenterCameraOnUnitAction);
			gameManager.OnLoadedTurn.RemoveListener (playerCenterCameraOnUnitAction);

			//remove listener for actionMode changes
			gameManager.OnActionModeChange.RemoveListener (actionModeSetNextUnitButtonStatusAction);

		}

	}

}
