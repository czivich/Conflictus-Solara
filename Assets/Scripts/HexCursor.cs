using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HexCursor : MonoBehaviour {

	//create a vector3 offset so that the cursor can exist just above the combat units
	private static Vector3 mapOffset = new Vector3(0.0f, 0.02f, 0.0f);

	//variable for the parent in the hierarchy
	private GameObject emblemParent;

	//declare public event for creating the cursor
	//make it static so we don't need to listen to every unit instance
	public static CreateHexCursorEvent OnCreateHexCursor = new CreateHexCursorEvent();

	//simple class derived from unityEvent to pass CombatUnit Object
	public class CreateHexCursorEvent : UnityEvent<HexCursor>{};

	//use this for initialization
	public void Init(){

		//find the parent
		emblemParent = GameObject.FindGameObjectWithTag("EmblemParent");

		//set the parent
		this.gameObject.transform.SetParent(emblemParent.transform);

	}

	public static void createHexCursor(HexCursor prefabHexCursor,HexMapTile hexMapTile){

		//cache data about the prefab and tile passed to the function
		Hex hexLocation = hexMapTile.hexLocation;
		TileMap tileMap = hexMapTile.tileMap;
		Layout layout = tileMap.layout;

		//covert the hex we want to create the cursor at to the local coordinate system
		Vector3 unitLocalCoordinatesV3 = Layout.HexToPixelV3 (layout, hexLocation);

		//convert the local coordinates to world coordinates to instantiate the cursor at
		Vector3 unitWorldCoordinatesV3 = new Vector3 ((tileMap.transform.localToWorldMatrix * (unitLocalCoordinatesV3)).x + tileMap.transform.position.x,
			(tileMap.transform.localToWorldMatrix * (unitLocalCoordinatesV3)).y + tileMap.transform.position.y,
			(tileMap.transform.localToWorldMatrix * (unitLocalCoordinatesV3)).z + tileMap.transform.position.z);

		HexCursor newHexCursor;
		newHexCursor = Instantiate (prefabHexCursor, unitWorldCoordinatesV3 + HexCursor.mapOffset, Quaternion.identity);

		//scale the cursor
		SetCursorSize(newHexCursor,layout);

		//run init
		newHexCursor.Init();

		//invoke the create cursor event
		OnCreateHexCursor.Invoke(newHexCursor);


	}

	//this method adjusts the hex cursor size to match the scale of the map tiles
	private static void SetCursorSize(HexCursor newHexCursor, Layout layout){
		Vector3 cursorScale = new Vector3 (layout.size.x * 2.0f, 1.0f, layout.size.y * 2.0f);
		newHexCursor.transform.localScale = cursorScale;
	}
		
}
