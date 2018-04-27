using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMapUnitExplosion : MonoBehaviour {

	//variable to hold the loop time for the animation
	private float loopTime;

	//variable to hold the frame time
	private float frameTime;

	//variable to hold the current frame index
	private int currentFrame;

	//variable to control whether animation should loop or destroy after 1 loop
	private bool isSingleLoop;

	//variable to hold the material rows
	private int materialRows;
	private int materialColumns;

	//variable to keep track of the number of frames in the sheet
	private int materialFrames;

	//array to hold the vector2 UVs for the hexagon flat points
	private Vector2[] hexagonFlatUVs;

	private Vector2 centerOffset;

	private float timeSinceAnimationStarted;

	//create a vector3 offset so that tiles appear above the tilemap
	private static Vector3 mapOffset = new Vector3(0.0f, 0.035f, 0.0f);

	//this function is used to instantiate an instance
	public static void CreateTileMapUnitExplosion(TileMapUnitExplosion prefab, HexMapTile hexMapTile, int matRows, int matColumns, float animationTime, Vector3 localScale, Transform parent, bool isSingleLoop){

		//cache data about the prefab and tile passed to the function
		TileMap tileMap = hexMapTile.tileMap;

		//covert the hex we want to create the unit at to the local coordinate system
		Vector3 unitLocalCoordinatesV3 = Layout.HexToPixelV3 (tileMap.layout, hexMapTile.hexLocation);

		//convert the local coordinates to world coordinates to instantiate the unit at
		Vector3 unitWorldCoordinatesV3 = new Vector3 ((tileMap.transform.localToWorldMatrix * (unitLocalCoordinatesV3)).x + tileMap.transform.position.x,
			(tileMap.transform.localToWorldMatrix * (unitLocalCoordinatesV3)).y + tileMap.transform.position.y,
			(tileMap.transform.localToWorldMatrix * (unitLocalCoordinatesV3)).z + tileMap.transform.position.z);

		//instantiate the new object
		//UIAnimation newUIAnimation = Instantiate (prefab, animationParent,false);
		TileMapUnitExplosion newTileMapUnitExplosion = SimplePool.Spawn(prefab.gameObject,unitWorldCoordinatesV3 + mapOffset,Quaternion.identity).GetComponent<TileMapUnitExplosion>();

		//set the parent
		newTileMapUnitExplosion.transform.SetParent(parent,false);

		//set the new object scale
		newTileMapUnitExplosion.transform.localScale = localScale;

		//set the isSingleLoop flag
		newTileMapUnitExplosion.isSingleLoop = isSingleLoop;

		//call the Init function 
		newTileMapUnitExplosion.Init(animationTime, matRows, matColumns);

	}

	// Use this for initialization
	private void Init (float animationTime, int matRows, int matColumns) {

		//set the rows and columns
		materialRows = matRows;
		materialColumns = matColumns;

		//calculate the number of frames
		materialFrames = materialRows * materialColumns;

		//set the hexagon UVs
		SetHexagonFlatUVs();

		//set the loop time
		loopTime = animationTime;

		//calculate the frame time
		frameTime = loopTime / materialFrames;

		//set the current frame to zero
		currentFrame = 0;

		//set the UVs to display the correct frame
		AssignObjectGraphics(currentFrame);

		//set the time to zero
		timeSinceAnimationStarted = 0.0f;

	}

	// Update is called once per frame
	private void Update () {

		timeSinceAnimationStarted += Time.deltaTime;

		//calculate the current frame
		currentFrame = Mathf.CeilToInt(timeSinceAnimationStarted / frameTime);

		//calculate the new frame to show
		//check if the current frame plus the frames to advance is greater than the number of frames -1
		//we use the -1 because for example an 11 x 11 grid will have 121 frames, which are frames 0 - 120, there is no frame 121
		if (currentFrame >= materialFrames - 1) {
			
			currentFrame = currentFrame % materialFrames;

			//check if we should be looping or not
			if (this.isSingleLoop == true) {

				currentFrame = 0;

				//if we should only loop once, destroy this gameobject
				//GameObject.Destroy(this.gameObject);
				SimplePool.Despawn(this.gameObject);

			}

		} 

		//set the UVs to display the correct frame
		AssignObjectGraphics(currentFrame);

	}

	//this function sets the hexagon flat UVs
	private void SetHexagonFlatUVs(){

		hexagonFlatUVs = new Vector2[7];

		hexagonFlatUVs[0] = new Vector2(0.0f / materialColumns, 0.0f / materialRows);												//center UV
		hexagonFlatUVs[4] = new Vector2(0.0f / materialColumns, 0.5f / materialRows);												//up UV
		hexagonFlatUVs[3] = new Vector2(Mathf.Sqrt (3.0f) / 4.0f / materialColumns, 0.25f / materialRows);							//up-right UV
		hexagonFlatUVs[1] = new Vector2(Mathf.Sqrt (3.0f) / 4.0f / materialColumns, -0.25f / materialRows);							//down-right UV
		hexagonFlatUVs[2] = new Vector2(0.0f / materialColumns, -0.5f / materialRows);												//down UV
		hexagonFlatUVs[6] = new Vector2(-Mathf.Sqrt (3.0f) / 4.0f / materialColumns, -0.25f / materialRows);						//down-left UV
		hexagonFlatUVs[5] = new Vector2(-Mathf.Sqrt (3.0f) / 4.0f / materialColumns, 0.25f / materialRows);							//up-left UV

		centerOffset = new Vector2 (0.5f, 0.5f);

	}

	//this function takes a frame index number and returns the proper UV coordinates for the HexagonFlat model for that frame
	private Vector2[] GetUVsFromFrameIndex (int frameIndex){

		//the frame index is 0 at the top left of the image, then increases across the row, then moves down the sheet
		//I need to convert the frame index to an X, Y vector2

		//this is an integer division to determine what row the frame is in
		int rowNumberFromTop = frameIndex / materialRows;

		//this is an integer modulus to determine what column the frame is in
		int colNumberFromLeft = frameIndex % materialColumns;

		//the atlas coordinate is where the frame is on the material grid, counting 0,0 as bottom left
		Vector2 atlasCoordinate = new Vector2 (colNumberFromLeft, materialRows - rowNumberFromTop - 1);

		//the atlas UVs take the atlas coordinate and center offset and add a vector to the points of the hexagons
		//for each index.  There are 7 vertices for each hexagon.  Note that through trial and error the vertices
		//for this hexagon are 0-4-3-1-2-6-5.  The import from blender did some funny things with the vertices
		Vector2[] atlasUVs = new Vector2[7];

		for (int i = 0; i < atlasUVs.Length; i++) {

			//the UV is the the atlas coordinate plus the center offset plus the hexagon flat UV
			atlasUVs [i] = new Vector2 (atlasCoordinate.x / materialColumns + centerOffset.x / materialColumns + hexagonFlatUVs [i].x,
				atlasCoordinate.y / materialRows + centerOffset.y / materialRows + hexagonFlatUVs [i].y);	

		}

		//return the atlasUVs to the method call
		return atlasUVs;
	}

	//function to assign graphics to a gameObject
	private void AssignObjectGraphics(int frameIndex){

		//start by getting the mesh of the object
		Mesh mesh = this.GetComponentInChildren<MeshFilter>().mesh;

		//get the UVs from the object
		Vector2[] newGameObjectUVs = new Vector2[mesh.vertices.Length];

		//map the UVs to the appropriate image from the TileAtlas material
		newGameObjectUVs = GetUVsFromFrameIndex (frameIndex);
		mesh.uv = newGameObjectUVs;

		return;

	}

}
