using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WormholeRotation : MonoBehaviour {

	//variable for the manager
	private GameManager gameManager;

	//create a vector3 offset so that the rotating object is just below the map
	private static Vector3 mapOffset = new Vector3(0.0f, -0.01f, 0.0f);

	//variable to control speed of rotation
	private float rotationsPerSecond = .25f;

	//enum to hold wormhole color
	public enum WormholeColor{

		Red,
		Blue,
	}

	public WormholeColor wormholeColor {

		get;
		private set;

	}

	public GraphicsManager.MyUnit currentTileImage {

		get;
		private set;

	}

	//declare public event for creating the object
	//make it static so we don't need to listen to every unit instance
	public static WormholeEvent OnCreateRotatingWormhole = new WormholeEvent();

	//simple class derived from unityEvent to pass CombatUnit Object
	public class WormholeEvent : UnityEvent<WormholeRotation>{};

	// Use this for initialization
	public void Init () {
		
	}
	
	// Update is called once per frame
	void Update () {

		//during the update, we want the graphic to rotate
		transform.Rotate(Vector3.up * Time.deltaTime * 360f * rotationsPerSecond);
		
	}

	public static void createRotatingWormhole(WormholeRotation prefabWormholeRotation, HexMapTile hexMapTile, WormholeColor wormholeColor){

		//cache data about the prefab and tile passed to the function
		//Hex hexLocation = hexMapTile.hexLocation;
		//TileMap tileMap = hexMapTile.tileMap;
		//Layout layout = tileMap.layout;

		//convert the hex location to world coordinates to instantiate the unit at
		Vector3 unitWorldCoordinatesV3 = hexMapTile.tileMap.HexToWorldCoordinates(hexMapTile.hexLocation) + WormholeRotation.mapOffset;

		WormholeRotation newWormholeRotation;
		newWormholeRotation = Instantiate (prefabWormholeRotation, unitWorldCoordinatesV3, Quaternion.identity);

		//set the tileImage
		switch (wormholeColor) {

		case WormholeColor.Red:

			newWormholeRotation.currentTileImage = GraphicsManager.MyUnit.RedWormhole;
			break;

		case WormholeColor.Blue:

			newWormholeRotation.currentTileImage = GraphicsManager.MyUnit.BlueWormhole;
			break;

			default:
			newWormholeRotation.currentTileImage = GraphicsManager.MyUnit.Blank;
			break;

		}

		//scale the cursor
		SetObjectSize(newWormholeRotation,hexMapTile.tileMap.layout);

		//set the parent
		newWormholeRotation.transform.SetParent(GameObject.Find("Wormholes").transform);

		//invoke the onCreate event
		OnCreateRotatingWormhole.Invoke(newWormholeRotation);

		//call init
		newWormholeRotation.Init();

	}

	//this method adjusts the cursor size to match the scale of the map tiles
	private static void SetObjectSize(WormholeRotation newWormholeRotation, Layout layout){

		Vector3 cursorScale = new Vector3 (layout.size.x * 2.0f, 1.0f, layout.size.y * 2.0f);
		newWormholeRotation.transform.localScale = cursorScale;

	}

}
