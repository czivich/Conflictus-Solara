using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TargetingCursor : MonoBehaviour {

	//variable for the manager
	private GameManager gameManager;

	//variable for the parent in the hierarchy
	private GameObject emblemParent;

	//create a vector3 offset so that the targeting cursor is above units
	private static Vector3 mapOffset = new Vector3(0.0f, 0.03f, 0.0f);

	//initialize the image used for the cursor
	private GraphicsManager.MyUnit defaultTileImage = GraphicsManager.MyUnit.RedCrosshair;	
	public GraphicsManager.MyUnit currentTileImage {

		get;
		private set;

	}

	//declare public event for creating the cursor
	//make it static so we don't need to listen to every unit instance
	public static TargetingCursorEvent OnCreateTargetingCursor = new TargetingCursorEvent();
	public static TargetingCursorEvent OnUpdateTargetingCursor = new TargetingCursorEvent();

	//simple class derived from unityEvent to pass CombatUnit Object
	public class TargetingCursorEvent : UnityEvent<TargetingCursor>{};

	//unityActions
	private UnityAction<GameManager.ActionMode> actionModeUpdateCursorTypeAction;

	//use this for initialization
	public void Init(){

		//get the manager
		gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

		//set the action
		actionModeUpdateCursorTypeAction = (actionMode) => {UpdateCursorType(actionMode);};

		//find the parent
		emblemParent = GameObject.FindGameObjectWithTag("EmblemParent");

		//set the parent
		this.transform.SetParent(emblemParent.transform);

		//Add a listener for when the CurrentActionMode changes
		gameManager.OnActionModeChange.AddListener(actionModeUpdateCursorTypeAction);


	}


	public static void createTargetingCursor(TargetingCursor prefabTargetingCursor, HexMapTile hexMapTile){

		//cache data about the prefab and tile passed to the function
		Hex hexLocation = hexMapTile.hexLocation;
		TileMap tileMap = hexMapTile.tileMap;
		Layout layout = tileMap.layout;

		//convert the hex location to world coordinates to instantiate the unit at
		Vector3 unitWorldCoordinatesV3 = tileMap.HexToWorldCoordinates(hexLocation) + TargetingCursor.mapOffset;

		TargetingCursor newTargetingCursor;
		newTargetingCursor = Instantiate (prefabTargetingCursor, unitWorldCoordinatesV3, Quaternion.identity);
		newTargetingCursor.currentTileImage = newTargetingCursor.defaultTileImage;

		//scale the cursor
		SetCursorSize(newTargetingCursor,layout);

		//invoke the onCreate targeting cursor event
		OnCreateTargetingCursor.Invoke(newTargetingCursor);

		//call init
		newTargetingCursor.Init();

		//turn off renderer (I only want to turn on renderer later when something is selected
		newTargetingCursor.GetComponentInChildren<MeshRenderer>().enabled = false;

	}

	//this method adjusts the cursor size to match the scale of the map tiles
	private static void SetCursorSize(TargetingCursor newTargetingCursor, Layout layout){
		
		Vector3 cursorScale = new Vector3 (layout.size.x * 2.0f, 1.0f, layout.size.y * 2.0f);
		newTargetingCursor.transform.localScale = cursorScale;

	}

	//this function returns a cursor graohics MyUnit based on the actionMode
	private GraphicsManager.MyUnit ActionModeToCursorType(GameManager.ActionMode currentActionMode){

		GraphicsManager.MyUnit cursorGraphics;

		switch (currentActionMode) {

		case GameManager.ActionMode.TractorBeam:
			cursorGraphics = GraphicsManager.MyUnit.YellowCrosshair;
			break;

		case GameManager.ActionMode.PhaserAttack:
			cursorGraphics = GraphicsManager.MyUnit.RedCrosshair;
			break;

		case GameManager.ActionMode.TorpedoAttack:
			cursorGraphics = GraphicsManager.MyUnit.RedCrosshair;
			break;

		case GameManager.ActionMode.ItemUse:
			cursorGraphics = GraphicsManager.MyUnit.GreenCrosshair;
			break;

		case GameManager.ActionMode.Crew:
			cursorGraphics = GraphicsManager.MyUnit.BlueCrosshair;
			break;

		//TODO - add more cases as we add more action modes

		default:
			cursorGraphics = GraphicsManager.MyUnit.RedCrosshair;

			//in the default case, I want to turn off the renderer
			this.GetComponentInChildren<MeshRenderer>().enabled = false;

			break;

		}

		return cursorGraphics;

	}

	//this function will update the cursor graphics based on the ActionMode
	private void UpdateCursorType(GameManager.ActionMode currentActionMode){

		//first, convert the currentActionMode to a graphics
		this.currentTileImage = ActionModeToCursorType(currentActionMode);

		//invoke the OnUpdate event
		OnUpdateTargetingCursor.Invoke(this);

	}

	//this function handles OnDestroy
	private void OnDestroy(){

		RemoveAllListeners ();

	}

	//this function removes all listeners
	private void RemoveAllListeners(){

		if (gameManager != null) {
			
			gameManager.OnActionModeChange.RemoveListener (actionModeUpdateCursorTypeAction);

		}

	}

}
