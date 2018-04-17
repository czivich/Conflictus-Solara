using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RangeTile : MonoBehaviour {

	//variable the image used for the cursor
	public GraphicsManager.MyUnit currentTileImage {

		get;
		private set;

	}

	//variable for the managers
	private MouseManager mouseManager;
	//private UIManager uiManager;
	private GameManager gameManager;

	//variable for the tileMap
	private TileMap tileMap;

	//variable for the prefab is public so it can be hooked up in the inspector
	public RangeTile rangeTilePrefab;

	//variable for the rangeTile parent
	private Transform rangeParent;

	//create a public enum to capture the type of range tile
	public enum RangeTileType{

		Movement,
		Attack,
		TractorBeam,
		Item,
		Crew,

	}

	//create a variable to track the RangeTileType
	//public RangeTileType rangeTileType;

	//create a vector3 offset so that tiles appear above the tilemap
	private static Vector3 mapOffset = new Vector3(0.0f, 0.005f, 0.0f);


	//declare public event for creating the range tile
	//make it static so we don't need to listen to every unit instance
	public static CreateRangeTileEvent OnCreateRangeTile = new CreateRangeTileEvent();

	//simple class derived from unityEvent to pass CombatUnit Object
	public class CreateRangeTileEvent : UnityEvent<RangeTile>{};

	//unityActions
	private UnityAction setRangeTilesAction;
	private UnityAction<GameManager.ActionMode> actionModeSetRangeTilesAction;
	private UnityAction<Ship> shipClearRangeTilesAction;
	private UnityAction<Ship> shipShipHasArrivedAction;
	private UnityAction<Ship> shipSetRangeTilesAction;
	private UnityAction<CombatUnit> combatUnitSetRangeTilesAction;


	// Use this for initialization
	public void Init () {

		//get the managers
		mouseManager = GameObject.FindGameObjectWithTag("MouseManager").GetComponent<MouseManager>();
		gameManager = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameManager> ();
		//uiManager = GameObject.FindGameObjectWithTag ("UIManager").GetComponent<UIManager> ();

		//get the tileMap
		tileMap = GameObject.FindGameObjectWithTag("TileMap").GetComponent<TileMap>();

		//get the rangeParent
		rangeParent = GameObject.FindGameObjectWithTag ("RangeParent").transform;

		//set the actions
		setRangeTilesAction = () => {SetRangeTiles(gameManager.CurrentActionMode);};
		actionModeSetRangeTilesAction = (currentActionMode) => {SetRangeTiles(currentActionMode);};
		shipClearRangeTilesAction = (ship) => {ClearAllRangeTiles();};
		shipShipHasArrivedAction = (ship) => {ShipHasArrived(ship);};
		shipSetRangeTilesAction = (ship) => {SetRangeTiles(gameManager.CurrentActionMode);};
		combatUnitSetRangeTilesAction = (combatUnit) => {SetRangeTiles(gameManager.CurrentActionMode);};

		//add listener for OnClearSelectedUnit
		mouseManager.OnClearSelectedUnit.AddListener(ClearAllRangeTiles);

		//add listener for OnSetSelectedUnit
		mouseManager.OnSetSelectedUnit.AddListener(setRangeTilesAction);

		//add listener for actionMode change
		gameManager.OnActionModeChange.AddListener(actionModeSetRangeTilesAction);

		//add listener for a ship starting to move
		EngineSection.OnMoveStart.AddListener(shipClearRangeTilesAction);

		//add listener for a ship completing a move
		EngineSection.OnMoveFinish.AddListener(shipShipHasArrivedAction);

		//add listener for a ship range refresh
		EngineSection.OnRefreshRange.AddListener(shipSetRangeTilesAction);

		//add listeners for ship and bases getting destroyed
		//add listener for base being destroyed to recalculate range
		Starbase.OnBaseDestroyedLate.AddListener(combatUnitSetRangeTilesAction);

		//add listener for ship being destroyed to recalculate range
		Ship.OnShipDestroyedLate.AddListener(combatUnitSetRangeTilesAction);

	}

	private static void CreateRangeTile(RangeTile prefabRangeTile, HexMapTile hexMapTile,Transform parentTransform, RangeTileType rangeTileType){

		//cache data about the prefab and tile passed to the function
		Hex hexLocation = hexMapTile.hexLocation;
		TileMap tileMap = hexMapTile.tileMap;
		Layout layout = tileMap.layout;

		//covert the hex we want to create the unit at to the local coordinate system
		Vector3 unitLocalCoordinatesV3 = Layout.HexToPixelV3 (layout, hexLocation);

		//convert the local coordinates to world coordinates to instantiate the unit at
		Vector3 unitWorldCoordinatesV3 = new Vector3 ((tileMap.transform.localToWorldMatrix * (unitLocalCoordinatesV3)).x + tileMap.transform.position.x,
			(tileMap.transform.localToWorldMatrix * (unitLocalCoordinatesV3)).y + tileMap.transform.position.y,
			(tileMap.transform.localToWorldMatrix * (unitLocalCoordinatesV3)).z + tileMap.transform.position.z);

		RangeTile newRangeTile;

		//the new vector3 is to make sure the movement range tiles appear just above the tilemap to prevent z fighting
		//newRangeTile = Instantiate (prefabRangeTile, unitWorldCoordinatesV3 + RangeTile.mapOffset, Quaternion.identity);

		//let's try this with the new object pooling script
		newRangeTile = SimplePool.Spawn(prefabRangeTile.gameObject,unitWorldCoordinatesV3 + RangeTile.mapOffset, Quaternion.identity).GetComponent<RangeTile>();

		//set current tile image based on the passed RangeTileType
		if (rangeTileType == RangeTileType.Movement) {

			newRangeTile.currentTileImage = GraphicsManager.MyUnit.MovementRange;

		} 

		else if (rangeTileType == RangeTileType.Attack) {

			newRangeTile.currentTileImage = GraphicsManager.MyUnit.AttackRange;

		} 

		else if (rangeTileType == RangeTileType.TractorBeam) {

			newRangeTile.currentTileImage = GraphicsManager.MyUnit.TractorBeamRange;

		} 

		else if (rangeTileType == RangeTileType.Item) {

			newRangeTile.currentTileImage = GraphicsManager.MyUnit.ItemRange;

		} 

		else if (rangeTileType == RangeTileType.Crew) {

			newRangeTile.currentTileImage = GraphicsManager.MyUnit.RepairRange;

		} 

		//if we are here, we passed a tile type that we're not checking for
		else {

			//newRangeTile.currentTileImage = null;
			//Debug.LogError ("Range Tile creation was passed an unrecognized TileType");

		}

		//set parent in hierarchy to avoid clutter
		newRangeTile.transform.parent = parentTransform.transform;

		//scale the tile
		SetTileSize(newRangeTile,layout);

		//invoke the create range tile event
		OnCreateRangeTile.Invoke(newRangeTile);

		//turn off renderer (I only want to turn on renderer later when something is selected
		//newRangeTile.GetComponentInChildren<MeshRenderer>().enabled = false;

	}

	//this method adjusts the tile size to match the scale of the map tiles
	private static void SetTileSize(RangeTile newRangeTile, Layout layout){
		Vector3 tileScale = new Vector3 (layout.size.x * 2.0f, 1.0f, layout.size.y * 2.0f);
		newRangeTile.transform.localScale = tileScale;
	}

	//this function destroys all RangeTiles
	private static void ClearAllRangeTiles(){

		//define an array of all movement range tiles
		GameObject[] rangeTiles = GameObject.FindGameObjectsWithTag ("RangeTile");
		//loop through all movement range tiles and destroy them
		foreach (GameObject rangeTile in rangeTiles) {

			//destroy them to clear the map
			//Destroy (rangeTile);
			//let's try this with the new object pooling script
			SimplePool.Despawn(rangeTile);

		}

	}

	//function to handle range tile processing based on GameManager ActionMode
	private void SetRangeTiles(GameManager.ActionMode currentActionMode){
		

		//if the action mode is selection, we want to clear all range tiles
		if (currentActionMode == GameManager.ActionMode.Selection) {

			RangeTile.ClearAllRangeTiles ();


		} else if (currentActionMode == GameManager.ActionMode.Movement) {

			//check if there is a selected unit
			if (mouseManager.selectedUnit != null && mouseManager.selectedUnit.GetComponent<EngineSection>().isMoving == false) {

				//Debug.Log ("SetRangeTiles at frame " + Time.frameCount+ " " + mouseManager.selectedUnit.GetComponent<Ship>().shipName);

				//first, we want to clear all existing range tiles
				RangeTile.ClearAllRangeTiles ();

				//create a rangeTile for each hex in the reachable hexes range
				//we can safely assume there will be an engine section if we are in movement mode
				foreach (Hex hex in mouseManager.selectedUnit.GetComponent<EngineSection>().ReachableHexes) {

					//create a movement range tile at each reachable hex
					RangeTile.CreateRangeTile (rangeTilePrefab, tileMap.HexMap [hex], rangeParent, RangeTile.RangeTileType.Movement);

				}

			}

		} else if (currentActionMode == GameManager.ActionMode.TractorBeam) {

			//check if there is a selected unit
			if (mouseManager.selectedUnit != null) {
				
				//first, we want to clear all existing range tiles
				RangeTile.ClearAllRangeTiles ();

				//check if the selected unit is a ship
				if (mouseManager.selectedUnit.GetComponent<Ship> () == true) {

					//create a rangeTile for each hex in the targetable hexes range
					//we can safely assume there will be a phasor section if we are in movement mode
					foreach (Hex hex in mouseManager.selectedUnit.GetComponent<PhasorSection>().TargetableTractorBeamHexes) {

						//create a movement range tile at each targetable hex
						RangeTile.CreateRangeTile (rangeTilePrefab, tileMap.HexMap [hex], rangeParent, RangeTile.RangeTileType.TractorBeam);

					}

				}

			}

		} else if (currentActionMode == GameManager.ActionMode.PhasorAttack) {


			//check if there is a selected unit
			if (mouseManager.selectedUnit != null) {

				//first, we want to clear all existing range tiles
				RangeTile.ClearAllRangeTiles ();

				//check if the selected unit is a ship
				if (mouseManager.selectedUnit.GetComponent<Ship> () == true) {

					//create a rangeTile for each hex in the targetable hexes range
					foreach (Hex hex in mouseManager.selectedUnit.GetComponent<PhasorSection>().TargetablePhasorHexes) {

						//create a movement range tile at each targetable hex
						RangeTile.CreateRangeTile (rangeTilePrefab, tileMap.HexMap [hex], rangeParent, RangeTile.RangeTileType.Attack);

					}

				}
				//the else condition is that we have a starbase
				else if (mouseManager.selectedUnit.GetComponent<Starbase> () == true) {

					//create a rangeTile for each hex in the targetable hexes range
					//we can grab from either phasor section - they will both be updating even if they are destroyed
					foreach (Hex hex in mouseManager.selectedUnit.GetComponent<StarbasePhasorSection1>().TargetablePhasorHexes) {

						//create a movement range tile at each targetable hex
						RangeTile.CreateRangeTile (rangeTilePrefab, tileMap.HexMap [hex], rangeParent, RangeTile.RangeTileType.Attack);

					}

				}

			}

		} else if (currentActionMode == GameManager.ActionMode.TorpedoAttack) {


			//check if there is a selected unit
			if (mouseManager.selectedUnit != null) {

				//first, we want to clear all existing range tiles
				RangeTile.ClearAllRangeTiles ();

				//check if the selected unit is a ship
				if (mouseManager.selectedUnit.GetComponent<Ship> () == true) {

					//create a rangeTile for each hex in the targetable hexes range
					foreach (Hex hex in mouseManager.selectedUnit.GetComponent<TorpedoSection>().TargetableTorpedoHexes) {

						//create a movement range tile at each targetable hex
						RangeTile.CreateRangeTile (rangeTilePrefab, tileMap.HexMap [hex], rangeParent, RangeTile.RangeTileType.Attack);

					}

				}
				//the else condition is that we have a starbase
				else if (mouseManager.selectedUnit.GetComponent<Starbase> () == true) {

					//create a rangeTile for each hex in the targetable hexes range
					foreach (Hex hex in mouseManager.selectedUnit.GetComponent<StarbaseTorpedoSection>().TargetableTorpedoHexes) {

						//create a movement range tile at each targetable hex
						RangeTile.CreateRangeTile (rangeTilePrefab, tileMap.HexMap [hex], rangeParent, RangeTile.RangeTileType.Attack);

					}

				}

			}

		} else if (currentActionMode == GameManager.ActionMode.ItemUse) {
			
			//check if there is a selected unit
			if (mouseManager.selectedUnit != null) {

				//first, we want to clear all existing range tiles
				RangeTile.ClearAllRangeTiles ();

				//check if the selected unit is a ship
				if (mouseManager.selectedUnit.GetComponent<Ship> () == true) {
					
					//create a rangeTile for each hex in the targetable hexes range
					foreach (Hex hex in mouseManager.selectedUnit.GetComponent<StorageSection>().TargetableItemHexes) {

						//create a movement range tile at each targetable hex
						RangeTile.CreateRangeTile (rangeTilePrefab, tileMap.HexMap [hex], rangeParent, RangeTile.RangeTileType.Item);

					}

				}
				//the else condition is that we have a starbase
				else if (mouseManager.selectedUnit.GetComponent<Starbase> () == true) {

					//create a rangeTile for each hex in the targetable hexes range
					//we can grab from either storage section - they will both be updating even if they are destroyed
					foreach (Hex hex in mouseManager.selectedUnit.GetComponent<StarbaseStorageSection1>().TargetableItemHexes) {

						//create a movement range tile at each targetable hex
						RangeTile.CreateRangeTile (rangeTilePrefab, tileMap.HexMap [hex], rangeParent, RangeTile.RangeTileType.Item);

					}

				}

			}

		} else if (currentActionMode == GameManager.ActionMode.Crew) {

			//check if there is a selected unit
			if (mouseManager.selectedUnit != null) {

				//first, we want to clear all existing range tiles
				RangeTile.ClearAllRangeTiles ();

				//check if the selected unit is a ship
				if (mouseManager.selectedUnit.GetComponent<Ship> () == true) {

					//create a rangeTile for each hex in the targetable hexes range
					foreach (Hex hex in mouseManager.selectedUnit.GetComponent<CrewSection>().TargetableCrewHexes) {

						//create a movement range tile at each targetable hex
						RangeTile.CreateRangeTile (rangeTilePrefab, tileMap.HexMap [hex], rangeParent, RangeTile.RangeTileType.Crew);

					}

				}
				//the else condition is that we have a starbase
				else if (mouseManager.selectedUnit.GetComponent<Starbase> () == true) {

					//create a rangeTile for each hex in the targetable hexes range
					foreach (Hex hex in mouseManager.selectedUnit.GetComponent<StarbaseCrewSection>().TargetableCrewHexes) {

						//create a movement range tile at each targetable hex
						RangeTile.CreateRangeTile (rangeTilePrefab, tileMap.HexMap [hex], rangeParent, RangeTile.RangeTileType.Crew);

					}

				}

			}

		} else if (currentActionMode == GameManager.ActionMode.EndTurn) {

			ClearAllRangeTiles ();

		} else if (currentActionMode == GameManager.ActionMode.Cloaking) {

			ClearAllRangeTiles ();


		} else if (currentActionMode == GameManager.ActionMode.PlaceNewUnit) {

			//first we want to clear all range tiles
			ClearAllRangeTiles ();

			//now, we need to determine where to place tiles depending on who the current player is
			//check if the home tile or adjacent tiles are available to place a ship
			//switch case based on player color
			switch (gameManager.currentTurn) {

			case Player.Color.Green:

				foreach (Hex possibleHex in tileMap.GreenStartTiles) {

					//check if the start tile is empty
					if (tileMap.HexMap [possibleHex].tileCombatUnit == null) {

						//if it is, create a range tile there
						RangeTile.CreateRangeTile (rangeTilePrefab, tileMap.HexMap [possibleHex], rangeParent, RangeTile.RangeTileType.Movement);

					} else if (tileMap.ReachableTiles (possibleHex, 1).Count > 0) {

						//check if at least one neighbor tile is open
						//if it is, create a range tile in each available neighbor
						foreach (Hex neighborHex in tileMap.ReachableTiles (possibleHex, 1)) {

							//create a range tile in open neighbor tiles
							RangeTile.CreateRangeTile (rangeTilePrefab, tileMap.HexMap [neighborHex], rangeParent, RangeTile.RangeTileType.Movement);

						}

					} 

				}

				break;

			case Player.Color.Purple:

				foreach (Hex possibleHex in tileMap.PurpleStartTiles) {

					//check if the start tile is empty
					if (tileMap.HexMap [possibleHex].tileCombatUnit == null) {

						//if it is, create a range tile there
						RangeTile.CreateRangeTile (rangeTilePrefab, tileMap.HexMap [possibleHex], rangeParent, RangeTile.RangeTileType.Movement);

					} else if (tileMap.ReachableTiles (possibleHex, 1).Count > 0) {

						//check if at least one neighbor tile is open
						//if it is, create a range tile in each available neighbor
						foreach (Hex neighborHex in tileMap.ReachableTiles (possibleHex, 1)) {

							//create a range tile in open neighbor tiles
							RangeTile.CreateRangeTile (rangeTilePrefab, tileMap.HexMap [neighborHex], rangeParent, RangeTile.RangeTileType.Movement);

						}

					} 

				}

				break;

			case Player.Color.Red:

				foreach (Hex possibleHex in tileMap.RedStartTiles) {

					//check if the start tile is empty
					if (tileMap.HexMap [possibleHex].tileCombatUnit == null) {

						//if it is, create a range tile there
						RangeTile.CreateRangeTile (rangeTilePrefab, tileMap.HexMap [possibleHex], rangeParent, RangeTile.RangeTileType.Movement);

					} else if (tileMap.ReachableTiles (possibleHex, 1).Count > 0) {

						//check if at least one neighbor tile is open
						//if it is, create a range tile in each available neighbor
						foreach (Hex neighborHex in tileMap.ReachableTiles (possibleHex, 1)) {

							//create a range tile in open neighbor tiles
							RangeTile.CreateRangeTile (rangeTilePrefab, tileMap.HexMap [neighborHex], rangeParent, RangeTile.RangeTileType.Movement);

						}

					} 

				}

				break;

			case Player.Color.Blue:

				foreach (Hex possibleHex in tileMap.BlueStartTiles) {

					//check if the start tile is empty
					if (tileMap.HexMap [possibleHex].tileCombatUnit == null) {

						//if it is, create a range tile there
						RangeTile.CreateRangeTile (rangeTilePrefab, tileMap.HexMap [possibleHex], rangeParent, RangeTile.RangeTileType.Movement);

					} else if (tileMap.ReachableTiles (possibleHex, 1).Count > 0) {

						//check if at least one neighbor tile is open
						//if it is, create a range tile in each available neighbor
						foreach (Hex neighborHex in tileMap.ReachableTiles (possibleHex, 1)) {

							//create a range tile in open neighbor tiles
							RangeTile.CreateRangeTile (rangeTilePrefab, tileMap.HexMap [neighborHex], rangeParent, RangeTile.RangeTileType.Movement);

						}

					} 

				}

				break;

			default:

				break;

			}

		}

		//TODO - add logic for more action modes as they are added

	}

	//function to handle displaying movement tiles when a ship has arrived
	private void ShipHasArrived(Ship ship){

		//Debug.Log ("call RangeTile ShipHasArrived");
		//this function should only ever be called when the selected unit is a ship, but we'll check anyway
		if (mouseManager.selectedUnit.GetComponent<Ship> () != null) {

			//check to make sure the ship that arrived is the selected unit
			//we don't want to do anything if the ship was towed to get there
			if (ship == mouseManager.selectedUnit.GetComponent<Ship> ()) {

				Debug.Log("shipHasArrived at frame " + Time.frameCount + " " + ship.shipName);

				//first, we want to clear all existing range tiles
				RangeTile.ClearAllRangeTiles ();

				//we want to show the remaining movement range at this point
				//create a rangeTile for each hex in the reachable hexes range
				foreach (Hex hex in ship.GetComponent<EngineSection>().ReachableHexes) {

					//create a movement range tile at each reachable hex
					RangeTile.CreateRangeTile (rangeTilePrefab, tileMap.HexMap [hex], rangeParent, RangeTile.RangeTileType.Movement);

				}

			}
		
		}
	
	}

	//this function handles OnDestroy
	private void OnDestroy(){

		//only one instance of RangeTile actually has Init() called - it's the one attached to the RangeParent.  The rest never have listeners added
		//so, we only need to remove listeners from the one attached to RangeParent
		if (this.transform == rangeParent) {

			RemoveAllListeners ();

		}

	}

	//this function removes all listeners
	private void RemoveAllListeners(){

		if (mouseManager != null) {
			
			//remove listener for OnClearSelectedUnit
			mouseManager.OnClearSelectedUnit.RemoveListener (ClearAllRangeTiles);

			//remove listener for OnSetSelectedUnit
			mouseManager.OnSetSelectedUnit.RemoveListener(setRangeTilesAction);

		}

		if (gameManager != null) {

			//remove listener for actionMode change
			gameManager.OnActionModeChange.RemoveListener (actionModeSetRangeTilesAction);

		}

		//remove listener for a ship starting to move
		EngineSection.OnMoveStart.RemoveListener(shipClearRangeTilesAction);

		//remove listener for a ship completing a move
		EngineSection.OnMoveFinish.RemoveListener(shipShipHasArrivedAction);

		//remove listener for a ship range refresh
		EngineSection.OnRefreshRange.RemoveListener(shipSetRangeTilesAction);

		//remove listeners for ship and bases getting destroyed
		//remove listener for base being destroyed to recalculate range
		Starbase.OnBaseDestroyedLate.RemoveListener(combatUnitSetRangeTilesAction);

		//remove listener for ship being destroyed to recalculate range
		Ship.OnShipDestroyedLate.RemoveListener(combatUnitSetRangeTilesAction);

	}

}
