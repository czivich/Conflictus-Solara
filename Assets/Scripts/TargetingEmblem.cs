using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TargetingEmblem : MonoBehaviour {

	//create a vector3 offset so that the targeting cursor is above units
	private static Vector3 mapOffset = new Vector3(0.0f, 0.03f, 0.0f);

	//variable for the parent in the hierarchy
	private GameObject emblemParent;

	//variables for the managers
	private MouseManager mouseManager;
	private GameManager gameManager;

	//initialize the image used for the cursor
	private GraphicsManager.MyUnit defaultTileImage = GraphicsManager.MyUnit.RedCrosshair;	
	public GraphicsManager.MyUnit currentTileImage {

		get;
		private set;

	}

	//variable to hold the known targetedUnit
	private GameObject targetedUnit;


	//declare public event for creating the emblem
	//make it static so we don't need to listen to every unit instance
	public static TargetingEmblemEvent OnCreateTargetingEmblem = new TargetingEmblemEvent();
	public static TargetingEmblemEvent OnShowTargetingEmblem = new TargetingEmblemEvent();

	//simple class derived from unityEvent to pass CombatUnit Object
	public class TargetingEmblemEvent : UnityEvent<TargetingEmblem>{};

	//unityActions
	private UnityAction<Ship> shipMoveTargetingEmblemAction;

	// Use this for initialization
	public void Init () {

		//set the actions
		shipMoveTargetingEmblemAction = (ship) => {MoveTargetingEmblem(ship);};

		//find the managers
		mouseManager = GameObject.FindGameObjectWithTag ("MouseManager").GetComponent<MouseManager>();
		gameManager = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameManager>();

		//find the parent
		emblemParent = GameObject.FindGameObjectWithTag("EmblemParent");

		//add listener for Setting and Clearing targeted units
		mouseManager.OnSetTargetedUnit.AddListener(ShowTargetingEmblem);
		mouseManager.OnClearTargetedUnit.AddListener(HideTargetingEmblem);

		//set the parent
		this.transform.SetParent(emblemParent.transform);

		//add a listener to the static ship event for when the targeted ship moves
		EngineSection.OnMoveTargetedShip.AddListener(shipMoveTargetingEmblemAction);
						
	}
	
	// Update is called once per frame
	private void Update () {

		//check if our targeted unit is null.  if it is, return without doing anything
		if (targetedUnit == null) {

			return;

		}

		//else if we do have a targeted unit, update the position to match the targeted unit
		else {

			//set the emblem coordinates to match the targeted unit if they aren't already the same
			if (this.transform.position != targetedUnit.transform.position) {
				
				this.transform.position = new Vector3 (targetedUnit.transform.position.x, this.transform.position.y, targetedUnit.transform.position.z);

			}

		}

	}

	//this function is used to instantiate a new targeting emblem
	public static void CreateTargetingEmblem(TargetingEmblem prefabTargetingEmblem, HexMapTile hexMapTile){

		//cache data about the prefab and tile passed to the function
		Hex hexLocation = hexMapTile.hexLocation;
		TileMap tileMap = hexMapTile.tileMap;
		Layout layout = tileMap.layout;

		//convert the hex location to world coordinates to instantiate the unit at
		Vector3 unitWorldCoordinatesV3 = tileMap.HexToWorldCoordinates(hexLocation) + TargetingEmblem.mapOffset;

		TargetingEmblem newTargetingEmblem;
		newTargetingEmblem = Instantiate (prefabTargetingEmblem, unitWorldCoordinatesV3, Quaternion.identity);
		newTargetingEmblem.currentTileImage = newTargetingEmblem.defaultTileImage;

		//scale the cursor
		SetEmblemSize(newTargetingEmblem,layout);

		//turn off renderer (I only want to turn on renderer later when something is selected
		newTargetingEmblem.GetComponentInChildren<MeshRenderer>().enabled = false;

		//invoke the on create event
		OnCreateTargetingEmblem.Invoke(newTargetingEmblem);

		//call Init
		newTargetingEmblem.Init();

	}

	//this method adjusts the emblem size to match the scale of the map tiles
	private static void SetEmblemSize(TargetingEmblem newTargetingEmblem, Layout layout){

		Vector3 emblemScale = new Vector3 (layout.size.x * 2.0f, 1.0f, layout.size.y * 2.0f);
		newTargetingEmblem.transform.localScale = emblemScale;

	}

	//this is the function that is called when a targeted unit is set
	private void ShowTargetingEmblem(){

		//check if the targetedUnit is null.  It shouldn't be if we're calling this function, but check anyway
		if (mouseManager.targetedUnit == null) {

			//if so, clear our targeted unit, turn off the mesh renderer, and return
			targetedUnit = null;
			this.GetComponentInChildren<MeshRenderer> ().enabled = false;

			return;

		}

		//the else condition is that there is a mouseManager targeted unit, which there always should be
		else {

			//set our targeted unit to the mouseManager targeted unit
			targetedUnit = mouseManager.targetedUnit;

			//set the graphic depending on the gameManager ActionMode
			if (gameManager.CurrentActionMode == GameManager.ActionMode.TractorBeam) {

				//set the current tile image
				currentTileImage = GraphicsManager.MyUnit.YellowCrosshair;

			}

			else if (gameManager.CurrentActionMode == GameManager.ActionMode.PhasorAttack) {

				//set the current tile image
				currentTileImage = GraphicsManager.MyUnit.RedCrosshair;

			}

			else if (gameManager.CurrentActionMode == GameManager.ActionMode.TorpedoAttack) {

				//set the current tile image
				currentTileImage = GraphicsManager.MyUnit.RedCrosshair;

			}

			else if (gameManager.CurrentActionMode == GameManager.ActionMode.ItemUse) {

				//set the current tile image
				currentTileImage = GraphicsManager.MyUnit.GreenCrosshair;

			}

			else if (gameManager.CurrentActionMode == GameManager.ActionMode.Crew) {

				//set the current tile image
				currentTileImage = GraphicsManager.MyUnit.BlueCrosshair;

			}

			//TODO - when we have defined other targeting action modes like phasor attack and torpedo attack, need to add them here

			//invoke the onShow event
			OnShowTargetingEmblem.Invoke(this);

			//update the location of the targeting emblem
			this.transform.position = new Vector3 (targetedUnit.transform.position.x, this.transform.position.y, targetedUnit.transform.position.z);

			//turn on the mesh renderer if it is not on already
			this.GetComponentInChildren<MeshRenderer> ().enabled = true;

		}

	}

	//this is the function that is called when the mouse manager is clearing the targeted unit
	private void HideTargetingEmblem(){

		//Debug.Log ("hide targeting emblem");

		//clear our targeted unit
		targetedUnit = null;

		//turn off the mesh renderer
		this.GetComponentInChildren<MeshRenderer> ().enabled = false;

	}

	//this function moves the position of the targeting emblem
	//it takes the targeted ship we want the emblem to follow as an argument
	private void MoveTargetingEmblem(Ship ship){

		//set the position of the targeting emblem x and z coordinates to match the targeted ship
		this.transform.position = new Vector3(ship.transform.position.x, this.transform.position.y,ship.transform.position.z);

	}

	//this function handles onDestroy
	private void OnDestroy(){

		RemoveAllListeners ();

	}

	//this function removes all listeners
	private void RemoveAllListeners(){

		//remove listener for Setting and Clearing targeted units
		if (mouseManager != null) {
			
			mouseManager.OnSetTargetedUnit.RemoveListener (ShowTargetingEmblem);
			mouseManager.OnClearTargetedUnit.RemoveListener (HideTargetingEmblem);

		}

		//remove a listener to the static ship event for when the targeted ship moves
		EngineSection.OnMoveTargetedShip.RemoveListener(shipMoveTargetingEmblemAction);

	}

}
