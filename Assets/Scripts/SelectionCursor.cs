using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SelectionCursor : MonoBehaviour {

	private float cursorFrameTime = 0.25f;			//this is the time to hold the current animation frame before updating
	private float cursorFrameCounter = 0.0f;		//this is the counter for how much time has passed

	private MouseManager mouseManager;

	//variable for the parent in the hierarchy
	private GameObject emblemParent;

	//create a vector3 offset so that the selection cursor is above units
	private static Vector3 mapOffset = new Vector3(0.0f, 0.015f, 0.0f);

	//initialize the image used for the cursor
	private GraphicsManager.MyUnit defaultTileImage = GraphicsManager.MyUnit.SelectionCursor;	
	public GraphicsManager.MyUnit currentTileImage {

		get;
		private set;

	}

	//variable to hold the cursor mesh renderer
	private MeshRenderer cursorMeshRender;

	//declare public event for creating the cursor
	//make it static so we don't need to listen to every unit instance
	public static SelectionCursorEvent OnCreateSelectionCursor = new SelectionCursorEvent();
	public static SelectionCursorEvent OnUpdateSelectionCursor = new SelectionCursorEvent();

	//simple class derived from unityEvent to pass CombatUnit Object
	public class SelectionCursorEvent : UnityEvent<SelectionCursor>{};

	//action variables for use with event listeners
	private UnityAction<Ship> hideSelectionCursorAction;
	private UnityAction<Ship> showSelectionCursorAction;

	// Use this for initialization
	public void Init () {
		
		//set the actions
		hideSelectionCursorAction = (ship) => {HideSelectionCursor();};
		showSelectionCursorAction = (ship) => {ShowSelectionCursor();};

		//cache the mesh renderer for use during update
		cursorMeshRender = this.GetComponentInChildren<MeshRenderer>();

		//Debug.Log ("cursorMeshRenderer = " + cursorMeshRender.name);

		//find the mouseManager
		mouseManager = GameObject.FindGameObjectWithTag ("MouseManager").GetComponent<MouseManager>();

		//find the parent
		emblemParent = GameObject.FindGameObjectWithTag("EmblemParent");

		//set the parent
		this.transform.SetParent(emblemParent.transform);

		//add listener for Setting and Clearing selected units
		mouseManager.OnSetSelectedUnit.AddListener(ShowSelectionCursor);
		mouseManager.OnClearSelectedUnit.AddListener(HideSelectionCursor);

		//add listeners for the movement start and end event
		EngineSection.OnMoveStart.AddListener(hideSelectionCursorAction);
		EngineSection.OnMoveFinish.AddListener(showSelectionCursorAction);


	}

	// Update is called once per frame
	private void Update () {

		//only cycle the graphics if the mesh renderer is enabled and the cursor is visible - otherwise it's a waste of processing
		if (cursorMeshRender.enabled == true) {

			cursorFrameCounter += Time.deltaTime;	//increment counter by deltaTime since last frame

			//if enough time has passed, increment the frame animation, then reset the counter
			if (cursorFrameCounter > cursorFrameTime) {
				this.IncrementCursorAnimation ();
				cursorFrameCounter = 0.0f;
			}

		}

	}

	//this function is used to instantiate a new selectionCursor
	public static void createSelectionCursor(SelectionCursor prefabSelectionCursor, HexMapTile hexMapTile){

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

		SelectionCursor newSelectionCursor;
		newSelectionCursor = Instantiate (prefabSelectionCursor, unitWorldCoordinatesV3 + mapOffset, Quaternion.identity);
		newSelectionCursor.currentTileImage = newSelectionCursor.defaultTileImage;

		//scale the cursor
		SetCursorSize(newSelectionCursor,layout);

		//intialize the cursor
		newSelectionCursor.Init ();

		//turn off renderer (I only want to turn on renderer later when something is selected
		newSelectionCursor.GetComponentInChildren<MeshRenderer>().enabled = false;

		//invoke the OnCreate event
		OnCreateSelectionCursor.Invoke(newSelectionCursor);


		//add listeners for the movement start and end event
		//use the lamda to take the ship input but do nothing with it
		//these are commented out because at least for now, I want to move the selection cursor with the ship instead of hiding it while the ship is moving
		//EngineSection.OnMoveStart.AddListener((ship) => {newSelectionCursor.HideSelectionCursor();});
		//EngineSection.OnMoveFinish.AddListener((ship) => {newSelectionCursor.ShowSelectionCursor();});

		//add a listener to the static ship event for when the selected ship moves
		//Ship.OnMoveSelectedShip.AddListener((ship) => {newSelectionCursor.MoveSelectionCursor(ship);});

	}

	//this method adjusts the cursor size to match the scale of the map tiles
	private static void SetCursorSize(SelectionCursor newSelectionCursor, Layout layout){
		Vector3 cursorScale = new Vector3 (layout.size.x * 2.0f, 1.0f, layout.size.y * 2.0f);
		newSelectionCursor.transform.localScale = cursorScale;
	}

	//this function will cycle the displayed tileAtlas image between the frames when called
	private void IncrementCursorAnimation(){

		//define what tileAtlas images are in the animation
		GraphicsManager.MyUnit CursorFrame1 = GraphicsManager.MyUnit.SelectionCursor;
		GraphicsManager.MyUnit CursorFrame2 = GraphicsManager.MyUnit.SelectionCursor2;

		//check to see what the current frame is, and if it's frame 1, set it to frame 2
		//if it is frame 2, set it to frame 1
		if (currentTileImage == CursorFrame1) {
			currentTileImage = CursorFrame2;

			//invoke the OnUpdate event
			OnUpdateSelectionCursor.Invoke(this);

		}
		else if (currentTileImage == CursorFrame2) {
			currentTileImage = CursorFrame1;

			//invoke the OnUpdate event
			OnUpdateSelectionCursor.Invoke(this);
		}

	}

	//this is the function that is called when a targeted unit is set
	private void ShowSelectionCursor(){

		//check if the selected unit is null.  It shouldn't be if we're calling this function, but check anyway
		if (mouseManager.selectedUnit == null) {

			//if so, turn off the mesh renderer, and return
			cursorMeshRender.enabled = false;

			Debug.LogError ("Tried to call ShowSelectionCursor() while selectedUnit was null");

			return;

		}

		//the else condition is that there is a mouseManager selected unit, which there always should be
		else {

			//move the selection cursor to the selected object
			this.transform.position = new Vector3(mouseManager.selectedUnit.transform.position.x, this.transform.position.y, mouseManager.selectedUnit.transform.position.z);

			//Debug.Log ("enable");

			//turn on the mesh renderer if it is not on already
			cursorMeshRender.enabled = true;

		}

	}

	//this is the function that is called when the mouse manager is clearing the targeted unit
	private void HideSelectionCursor(){


		//Debug.Log ("disable");
		//turn off the mesh renderer
		cursorMeshRender.enabled = false;

	}

	//this function moves the selection cursor
	private void MoveSelectionCursor(Ship selectedShip){

		//check if the selected unit is null.  It shouldn't be if we're calling this function, but check anyway
		if (selectedShip == null) {

			//if so, turn off the mesh renderer, and return
			cursorMeshRender.enabled = false;

			Debug.LogError ("Tried to call MoveSelectionCursor() while selectedShip was null");

			return;

		}

		//the else condition is that there is a mouseManager selected unit, which there always should be
		else {

			//move the selection cursor to the selected object
			this.transform.position = new Vector3(selectedShip.transform.position.x, this.transform.position.y, selectedShip.transform.position.z);

			//turn on the mesh renderer if it is not on already
			cursorMeshRender.enabled = true;

		}

	}

	//function to handle on destroy
	private void OnDestroy(){

		RemoveAllListeners ();

	}

	//this function removes all listeners
	private void RemoveAllListeners(){

		if (mouseManager != null) {
			
			mouseManager.OnSetSelectedUnit.RemoveListener (ShowSelectionCursor);
			mouseManager.OnClearSelectedUnit.RemoveListener (HideSelectionCursor);
		
		}

		EngineSection.OnMoveFinish.RemoveListener (showSelectionCursorAction);
		EngineSection.OnMoveStart.RemoveListener (hideSelectionCursorAction);

	}

}
