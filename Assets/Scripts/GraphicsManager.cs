using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class GraphicsManager : MonoBehaviour {

	//managers
	private UIManager uiManager;

	//constants to hold the number of tiles in our atlas
	//our code assumes it is a square X by X map
	private const float tilesPerRow = 11.0f;


	//this enum will be for uniform material types
	//the hexCursor will use this, and probably also the highlighting movement range
	public enum MyUniformMaterial{
		TransparentYellow,
	}

	//this enum is used for hex-shaped units like ships
	public enum MyUnit{
		BlueStarship,
		BlueDestroyer,
		BlueBird,
		BlueScout,
		BlueBirdCloaked,
		BlueStarbase,
		BlueFactory,
		GreenStarship,
		GreenDestroyer,
		GreenBird,
		GreenScout,
		GreenBirdCloaked,
		GreenStarbase,
		GreenFactory,
		PurpleStarship,
		PurpleDestroyer,
		PurpleBird,
		PurpleScout,
		PurpleBirdCloaked,
		PurpleStarbase,
		PurpleFactory,
		RedStarship,
		RedDestroyer,
		RedBird,
		RedScout,
		RedBirdCloaked,
		RedStarbase,
		RedFactory,
		SelectionCursor,
		SelectionCursor2,
		MovementRange,
		AttackRange,
		TractorBeamRange,
		ItemRange,
		RepairRange,
		RedCrosshair,
		BlueCrosshair,
		YellowCrosshair,
		GreenCrosshair,
		Blank,
		RedWormhole,
		BlueWormhole,
		RedMiniMap,
		BlueMiniMap,
		GreenMiniMap,
		PurpleMiniMap
	}

	public enum MySquareMaterial{
		MiniMapCursorBorder,
		MiniMapCursorBlank,
	}

	//unityActions
	private UnityAction<CombatUnit> combatUnitAssignNewUnitGraphicsAction;
	private UnityAction<CombatUnit,FileManager.SaveGameData> combatUnitLoadedAssignNewUnitGraphicsAction;
	private UnityAction<MiniMapCursor> miniMapCursorAssignCursorGraphicsAction;
	private UnityAction<TileMap> tileMapAssignTileMapGraphicsAction;
	private UnityAction<HexCursor> hexCursorAssignNewCursorGraphicsAction;
	private UnityAction<RangeTile> rangeTileAssignRangeTileGraphicsAction;
	private UnityAction<TargetingEmblem> targetingEmblemAssignTargetingEmblemGraphicsAction;
	private UnityAction<TargetingCursor> targetingCursorAssignTargetingCursorGraphicsAction;
	private UnityAction<SelectionCursor> selectionCursorAssignSelectionCursorGraphicsAction;
	private UnityAction<CombatUnit,RawImage> combatUnitSetRawImageOffsetForUnitAction;
	private UnityAction<HexMapTile,Player.Color> hexMapTileUpdateGraphicTileAction;
	private UnityAction<Colony> colonyAssignColonyGraphicsAction;

	//this was removed when I stopped creating a hexMapTile for the planet icons 
	//private UnityAction<HexMapTile,RawImage> hexMapTileSetRawImageOffsetForTileAction;

	private UnityAction<HexMapTile.TileType,RawImage> hexMapTileTypeSetRawImageOffsetForPlanetIconAction;
	private UnityAction<MyUnit,RawImage> unitSetRawImageOffsetForUnitTypeAction;
	private UnityAction<WormholeRotation> wormholeAssignGraphicsAction;


	//use this function for initialization
	public void Init(){

		//set the actions
		combatUnitAssignNewUnitGraphicsAction = (combatUnit) => {AssignNewUnitGraphics(combatUnit);};
		combatUnitLoadedAssignNewUnitGraphicsAction = (combatUnit,saveGameData) => {AssignNewUnitGraphics(combatUnit);};
		miniMapCursorAssignCursorGraphicsAction = (cursor) => {AssignMiniMapCursorGraphics(cursor);};
		tileMapAssignTileMapGraphicsAction = (tileMap) => {AssignTileMapGraphics(tileMap);};
		hexCursorAssignNewCursorGraphicsAction = (hexCursor) => {AssignNewCursorGraphics(hexCursor);};
		rangeTileAssignRangeTileGraphicsAction = (rangeTile) => {AssignRangeTileGraphics(rangeTile);};
		targetingEmblemAssignTargetingEmblemGraphicsAction = (targetingEmblem) => {AssignTargetingEmblemGraphics(targetingEmblem);};
		targetingCursorAssignTargetingCursorGraphicsAction = (targetingCursor) => {AssignTargetingCursorGraphics(targetingCursor);};
		selectionCursorAssignSelectionCursorGraphicsAction = (selectionCursor) => {AssignSelectionCursorGraphics(selectionCursor);};
		combatUnitSetRawImageOffsetForUnitAction = (combatUnit,rawImage) => {SetRawImageOffsetForUnit(combatUnit,rawImage);};
		hexMapTileUpdateGraphicTileAction = (hexMapTile,newColonyColor) => {UpdateGraphicMapTile(hexMapTile);};
		colonyAssignColonyGraphicsAction = (colony) => {AssignColonyGraphics(colony);};
		//hexMapTileSetRawImageOffsetForTileAction = (hexMapTile,rawImage) => {SetRawImageOffsetForTile(hexMapTile, rawImage);};
		hexMapTileTypeSetRawImageOffsetForPlanetIconAction = (hexMapTileType,rawImage) => {SetRawImageOffsetForPlanetIcon(hexMapTileType, rawImage);};
		unitSetRawImageOffsetForUnitTypeAction = (myUnit,rawImage) => {SetRawImageOffsetForUnitType(myUnit, rawImage);};
		wormholeAssignGraphicsAction = (wormhole) => {AssignWormholeGraphics(wormhole);};

		//get the managers
		uiManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();

		//add a listener for creating a new unit
		CombatUnit.OnCreateUnit.AddListener(combatUnitAssignNewUnitGraphicsAction);
		CombatUnit.OnCreateLoadedUnit.AddListener(combatUnitLoadedAssignNewUnitGraphicsAction);

		//add a listener for creating a minimap cursor
		MiniMapCursor.OnCreateMiniMapCursor.AddListener(miniMapCursorAssignCursorGraphicsAction);

		//add a listener for creating the tilemap
		TileMap.OnCreateTileMap.AddListener(tileMapAssignTileMapGraphicsAction);

		//add a listener for creating a new hexCursor
		HexCursor.OnCreateHexCursor.AddListener(hexCursorAssignNewCursorGraphicsAction);

		//add a listener for creating a new rangeTile
		RangeTile.OnCreateRangeTile.AddListener(rangeTileAssignRangeTileGraphicsAction);

		//add a listener for creating a new targeting emblem
		TargetingEmblem.OnCreateTargetingEmblem.AddListener(targetingEmblemAssignTargetingEmblemGraphicsAction);
		TargetingEmblem.OnShowTargetingEmblem.AddListener(targetingEmblemAssignTargetingEmblemGraphicsAction);

		//add a listener for creating a new targeting cursor
		TargetingCursor.OnCreateTargetingCursor.AddListener(targetingCursorAssignTargetingCursorGraphicsAction);
		TargetingCursor.OnUpdateTargetingCursor.AddListener(targetingCursorAssignTargetingCursorGraphicsAction);

		//add a listener for selection cursor
		SelectionCursor.OnCreateSelectionCursor.AddListener(selectionCursorAssignSelectionCursorGraphicsAction);
		SelectionCursor.OnUpdateSelectionCursor.AddListener(selectionCursorAssignSelectionCursorGraphicsAction);

		//add a listener for the UnitPanel highlighted unit
		uiManager.GetComponent<UnitPanel>().OnSetHighlightedUnit.AddListener(combatUnitSetRawImageOffsetForUnitAction);

		//add a listener for cloaking device events
		CloakingDevice.OnEngageCloakingDevice.AddListener(combatUnitAssignNewUnitGraphicsAction);
		CloakingDevice.OnDisengageCloakingDevice.AddListener(combatUnitAssignNewUnitGraphicsAction);

		//add listener for colonizing a new planet
		HexMapTile.OnColonizeNewPlanet.AddListener(hexMapTileUpdateGraphicTileAction);

		//add listener for creating a new colony
		Colony.OnCreateColony.AddListener(colonyAssignColonyGraphicsAction);

		//add listener for settling a new colony
		Colony.OnSettleColony.AddListener(colonyAssignColonyGraphicsAction);

		//add listener for setting planet icons
		uiManager.GetComponent<StatusPanel>().OnSetPlanetIcon.AddListener(hexMapTileTypeSetRawImageOffsetForPlanetIconAction);

		//add listener to setting purchase ship icons
		uiManager.GetComponent<PurchaseManager>().OnSetPurchaseShipIcon.AddListener(unitSetRawImageOffsetForUnitTypeAction);

		//add listener to status panel
		uiManager.GetComponent<StatusPanel>().OnSetShipIcon.AddListener(unitSetRawImageOffsetForUnitTypeAction);

		//add a listener to wormhole creation
		WormholeRotation.OnCreateRotatingWormhole.AddListener(wormholeAssignGraphicsAction);


	}

	//function to assign graphics to a newly created combatUnit
	private void AssignNewUnitGraphics(CombatUnit combatUnit){

		//define an array of mesh filters
		MeshFilter[] newUnitMeshFilters = ((MeshFilter[])combatUnit.GetComponentsInChildren<MeshFilter> ());

		//get the mesh from the new unit
		Mesh[] newUnitMeshes = new Mesh[newUnitMeshFilters.Length];

		//assign the meshes from the mesh filters
		for (int i = 0; i < newUnitMeshFilters.Length; i++) {

			newUnitMeshes [i] = newUnitMeshFilters [i].mesh;

		}

		//create 2D array of UVs
		Vector2[][] newUnitUVs = new Vector2[newUnitMeshes.Length][];

		//get the UVs from the new unit for the main camera graphics
		newUnitUVs[0] = SetUVsOfUnit (newUnitMeshes[0], GetCombatUnitMyUnit (combatUnit));

		//get the UVs from the new unit for the minimap camera graphics
		newUnitUVs[1] = SetUVsOfUnit (newUnitMeshes[1], GetMiniMapMyUnitFromCombatUnit(combatUnit));

		//assign the new UVs back to the mesh
		for (int i = 0; i < newUnitMeshes.Length; i++) {

			newUnitMeshes [i].uv = newUnitUVs [i];

		}

		return;

	}

	//this function returns the appropriate minimap MyUnit based on the unit color
	private MyUnit GetMiniMapMyUnitFromCombatUnit(CombatUnit combatUnit){

		//variable to return
		MyUnit returnMyUnit;

		//switch case based on combatUnitColor
		switch (combatUnit.owner.color) {
		 
		case Player.Color.Red:
			returnMyUnit = MyUnit.RedMiniMap;
			break;

		case Player.Color.Blue:
			returnMyUnit = MyUnit.BlueMiniMap;
			break;

		case Player.Color.Green:
			returnMyUnit = MyUnit.GreenMiniMap;
			break;

		case Player.Color.Purple:
			returnMyUnit = MyUnit.PurpleMiniMap;
			break;

		default:
			returnMyUnit = MyUnit.Blank;
			break;

		}

		return returnMyUnit;

	}

	//this function sets the graphics for the minimap cursor
	private void AssignMiniMapCursorGraphics(MiniMapCursor cursor){

		//get the mesh from the cursor
		Mesh cursorMesh = cursor.GetComponentInChildren<MeshFilter>().mesh;

		//cache the number of vertices in the mesh
		int numberVertices = cursor.miniMapCursorVertices.Length;

		int numberTiles = numberVertices / 4;

		//create a vector2 array to hold the UV coordinates
		Vector2[] miniMapCursorUV = new Vector2[numberVertices];

		for(int i = 0; i < numberTiles; i++){
			
			if (i == 4) {

				miniMapCursorUV [4 * i + 0] = GraphicsManager.GetUVsFromAtlasForSquareTileObject (GraphicsManager.MySquareMaterial.MiniMapCursorBlank) [0];
				miniMapCursorUV [4 * i + 1] = GraphicsManager.GetUVsFromAtlasForSquareTileObject (GraphicsManager.MySquareMaterial.MiniMapCursorBlank) [1];
				miniMapCursorUV [4 * i + 2] = GraphicsManager.GetUVsFromAtlasForSquareTileObject (GraphicsManager.MySquareMaterial.MiniMapCursorBlank) [2];
				miniMapCursorUV [4 * i + 3] = GraphicsManager.GetUVsFromAtlasForSquareTileObject (GraphicsManager.MySquareMaterial.MiniMapCursorBlank) [3];

			} else {

				miniMapCursorUV [4 * i + 0] = GraphicsManager.GetUVsFromAtlasForSquareTileObject (GraphicsManager.MySquareMaterial.MiniMapCursorBorder) [0];
				miniMapCursorUV [4 * i + 1] = GraphicsManager.GetUVsFromAtlasForSquareTileObject (GraphicsManager.MySquareMaterial.MiniMapCursorBorder) [1];
				miniMapCursorUV [4 * i + 2] = GraphicsManager.GetUVsFromAtlasForSquareTileObject (GraphicsManager.MySquareMaterial.MiniMapCursorBorder) [2];
				miniMapCursorUV [4 * i + 3] = GraphicsManager.GetUVsFromAtlasForSquareTileObject (GraphicsManager.MySquareMaterial.MiniMapCursorBorder) [3];

			}

		}

		cursorMesh.uv = miniMapCursorUV;

		return;

	}

	//this method has the hard-coding for what board tile sprites are where on my Tile Atlas
	//it takes a hexMapTile object as input, and returns the UV coordinates for that location
	//it needs to be able to access the TileType and HexLocation properties
	//it should only need to be called by other methods within this class, which actually interface with other classes
	private static Vector2[] GetUVsFromAtlasForTile (HexMapTile hexMapTile){

		//define a vector2 atlasCoordinate
		Vector2 atlasCoordinate = new Vector2();

		atlasCoordinate = GetAtlasCoordinateForHexMapTile (hexMapTile);

		//the center offset moves us from the atlasCoordinate (which is at the lower left corner of a tile) to the center
		Vector2 centerOffset = new Vector2 (0.5f, 0.5f);

		//the atlas UVs take the atlas coordinate and center offset and add a vector to the points of the hexagons
		//for each index.  There are 7 vertices for each hexagon - the center, then the 6 points starting
		//with the straight-up point and going clockwise
		Vector2[] atlasUVs = new Vector2[7];
		atlasUVs[0] = ((atlasCoordinate + centerOffset) / tilesPerRow);														//center UV
		atlasUVs[1] = ((atlasCoordinate + centerOffset + new Vector2(0.0f, 0.5f)) / tilesPerRow);							//up UV
		atlasUVs[2] = ((atlasCoordinate + centerOffset + new Vector2(Mathf.Sqrt (3.0f) / 4.0f, 0.25f)) / tilesPerRow);		//up-right UV
		atlasUVs[3] = ((atlasCoordinate + centerOffset + new Vector2(Mathf.Sqrt (3.0f) / 4.0f, -0.25f)) / tilesPerRow);		//down-right UV
		atlasUVs[4] = ((atlasCoordinate + centerOffset + new Vector2(0.0f, -0.5f)) / tilesPerRow);							//down UV
		atlasUVs[5] = ((atlasCoordinate + centerOffset + new Vector2(-Mathf.Sqrt (3.0f) / 4.0f, -0.25f)) / tilesPerRow);	//down-left UV
		atlasUVs[6] = ((atlasCoordinate + centerOffset + new Vector2(-Mathf.Sqrt (3.0f) / 4.0f, 0.25f)) / tilesPerRow);		//up-left UV

		//return the atlasUVs to the method call
		return atlasUVs;

	}

	//this function takes a hexmap tile and returns a vector2 for the atlas coordinate
	private static Vector2 GetAtlasCoordinateForHexMapTile(HexMapTile hexMapTile){

		//cache the hexMapTile TileType enum
		HexMapTile.TileType tileType = hexMapTile.tileType;

		//declare atlasCoordinate Vector2 - this is the X-Y coordinate where the tile is on the atlas grid (11 x 11)
		Vector2 atlasCoordinate = GetAtlasCoordinateForHexMapTileType(tileType);

		return atlasCoordinate;

	}

	//this function takes a hexmap tile type and returns a vector2 for the atlas coordinate
	private static Vector2 GetAtlasCoordinateForHexMapTileType(HexMapTile.TileType tileType){

		//declare atlasCoordinate Vector2 - this is the X-Y coordinate where the tile is on the atlas grid (11 x 11)
		Vector2 atlasCoordinate;

		//use switch-cases to define the atlasCoordinate for each TileType
		switch (tileType) {
		case HexMapTile.TileType.RedStart:
			atlasCoordinate = new Vector2 (0, 10);
			break;
		case HexMapTile.TileType.BlueStart:
			atlasCoordinate = new Vector2 (1, 10);
			break;
		case HexMapTile.TileType.GreenStart:
			atlasCoordinate = new Vector2 (2, 10);
			break;
		case HexMapTile.TileType.PurpleStart:
			atlasCoordinate = new Vector2 (3, 10);
			break;
		case HexMapTile.TileType.EmptySpace:
			atlasCoordinate = new Vector2 (4, 10);
			break;
		case HexMapTile.TileType.RedWormhole:
			//atlasCoordinate = new Vector2 (5, 10);
			//replace actual wormhole graphic with blank so we can have rotating wormhole object
			atlasCoordinate = new Vector2 (4, 10);
			break;
		case HexMapTile.TileType.BlueWormhole:
			//atlasCoordinate = new Vector2 (6, 10);
			//replace actual wormhole graphic with blank so we can have rotating wormhole object
			atlasCoordinate = new Vector2 (4, 10);
			break;
		case HexMapTile.TileType.NeutralStarbase:
			atlasCoordinate = new Vector2 (7, 10);
			break;
		case HexMapTile.TileType.Mercury:
			atlasCoordinate = new Vector2 (0, 9);
			break;
		case HexMapTile.TileType.Venus:
			atlasCoordinate = new Vector2 (1, 9);
			break;
		case HexMapTile.TileType.Earth:
			atlasCoordinate = new Vector2 (2, 9);
			break;
		case HexMapTile.TileType.Mars:
			atlasCoordinate = new Vector2 (3, 9);
			break;
		case HexMapTile.TileType.Jupiter:
			atlasCoordinate = new Vector2 (4, 9);
			break;
		case HexMapTile.TileType.Saturn:
			atlasCoordinate = new Vector2 (5, 9);
			break;
		case HexMapTile.TileType.Uranus:
			atlasCoordinate = new Vector2 (6, 9);
			break;
		case HexMapTile.TileType.Neptune:
			atlasCoordinate = new Vector2 (7, 9);
			break;
		case HexMapTile.TileType.Pluto:
			atlasCoordinate = new Vector2 (8, 9);
			break;
		case HexMapTile.TileType.PlanetX:
			atlasCoordinate = new Vector2 (9, 9);
			break;
		case HexMapTile.TileType.MercuryLow:
			atlasCoordinate = new Vector2 (0, 8);
			break;
		case HexMapTile.TileType.VenusLow:
			atlasCoordinate = new Vector2 (1, 8);
			break;
		case HexMapTile.TileType.EarthLow:
			atlasCoordinate = new Vector2 (2, 8);
			break;
		case HexMapTile.TileType.MarsLow:
			atlasCoordinate = new Vector2 (3, 8);
			break;
		case HexMapTile.TileType.JupiterLow:
			atlasCoordinate = new Vector2 (4, 8);
			break;
		case HexMapTile.TileType.SaturnLow:
			atlasCoordinate = new Vector2 (5, 8);
			break;
		case HexMapTile.TileType.UranusLow:
			atlasCoordinate = new Vector2 (6, 8);
			break;
		case HexMapTile.TileType.NeptuneLow:
			atlasCoordinate = new Vector2 (7, 8);
			break;
		case HexMapTile.TileType.PlutoLow:
			atlasCoordinate = new Vector2 (8, 8);
			break;
		case HexMapTile.TileType.PlanetXLow:
			atlasCoordinate = new Vector2 (9, 8);
			break;
		case HexMapTile.TileType.Asteroid:
			//the asteroid case returns a random asteroid tile - there are 22 total tiles
			//this will make the map look somewhat different each time
			//it eliminates the need to specify which asteroid I want to place where
			//and means I don't need to have a different TileType for each asteroid
			atlasCoordinate = new Vector2 (Random.Range (0, 11), Random.Range (6, 8));
			break;
		case HexMapTile.TileType.Sun:
			atlasCoordinate = new Vector2 (0, 5);
			break;
		case HexMapTile.TileType.SunRayUpRight:
			atlasCoordinate = new Vector2 (1, 5);
			break;
		case HexMapTile.TileType.SunRayRight:
			atlasCoordinate = new Vector2 (2, 5);
			break;
		case HexMapTile.TileType.SunRayDownRight:
			atlasCoordinate = new Vector2 (3, 5);
			break;
		case HexMapTile.TileType.SunRayDownLeft:
			atlasCoordinate = new Vector2 (4, 5);
			break;
		case HexMapTile.TileType.SunRayLeft:
			atlasCoordinate = new Vector2 (5, 5);
			break;
		case HexMapTile.TileType.SunRayUpLeft:
			atlasCoordinate = new Vector2 (6, 5);
			break;

		default:
			atlasCoordinate = new Vector2 (4, 10);
			break;
		}

		return atlasCoordinate;

	}



	//this method has the hard-coding for what uniform colors are where on my Tile Atlas
	//it takes a uniform material enum as input, and returns the UV coordinates for that location
	//it should only need to be called by other methods within this class, which actually interface with other classes
	private static Vector2 GetUVsFromAtlasForUniformObject(MyUniformMaterial objectUniformMaterial){
	
		//declare vector2 atlas coordinate - this is the X-Y location on the TileAtlas
		Vector2 atlasCoordinate;

		//use a switch-case to pick the right atlas coordinate for the specified uniform material
		//right now there is only 1 specified, but I expect to add more
		switch (objectUniformMaterial) {
		case MyUniformMaterial.TransparentYellow:
			atlasCoordinate = new Vector2 (10, 4);
			break;
		default:
			atlasCoordinate = new Vector2 (10, 4);
			break;
		}

		//the center offset takes us from the lower left corner of the tile to the center
		Vector2 centerOffset = new Vector2 (0.5f, 0.5f);

		//the atlas UVs vector2 is the uv coordinate in the center of the uniform material tile
		Vector2 atlasUVs;
		atlasUVs = (atlasCoordinate + centerOffset)/ tilesPerRow;

		return atlasUVs;

	}

	//this method has the hard-coding for what square tiles are where on my Tile Atlas
	//it takes a squareTile enum as input, and returns the UV coordinates for that location
	//it assumes there are only 4 UV coordinates
	private static Vector2[] GetUVsFromAtlasForSquareTileObject(MySquareMaterial mySquareMaterial){

		//declare vector2 atlas coordinate - this is the X-Y location on the TileAtlas
		Vector2 atlasCoordinate;

		//use a switch-case to pick the right atlas coordinate for the specified square tile
		//right now there is only 1 specified, but I expect to add more
		switch (mySquareMaterial) {
		case MySquareMaterial.MiniMapCursorBorder:
			atlasCoordinate = new Vector2 (10, 2);
			break;
		case MySquareMaterial.MiniMapCursorBlank:
			atlasCoordinate = new Vector2 (10, 0);
			break;
		default:
			atlasCoordinate = new Vector2 (10, 0);
			break;
		}

		//the center offset takes us from the lower left corner of the tile to the center
		Vector2 centerOffset = new Vector2 (0.5f, 0.5f);

		//the atlas UVs vector2 is the uv coordinate in the center of the uniform material tile
		Vector2[] atlasUVs = new Vector2[4];
		atlasUVs[0] = (atlasCoordinate + centerOffset + new Vector2(-0.4f, -0.4f))/ tilesPerRow;		//lower left
		atlasUVs[1] = (atlasCoordinate + centerOffset + new Vector2(0.4f, -0.4f))/ tilesPerRow;		//lower right
		atlasUVs[2] = (atlasCoordinate + centerOffset + new Vector2(-0.4f, 0.4f))/ tilesPerRow;		//upper left
		atlasUVs[3] = (atlasCoordinate + centerOffset + new Vector2(0.4f, 0.4f))/ tilesPerRow;		//upper right

		return atlasUVs;

	}


	//this method has the hard-coding for what unit sprites are where on my Tile Atlas
	//it takes a unit enum as input, and returns the UV coordinates for that location
	//it should only need to be called by other methods within this class, which actually interface with other classes
	private static Vector2[] GetUVsFromAtlasForUnit (MyUnit unitType){

		//declare atlasCoordinate Vector2 - this is the X-Y coordinate where the tile is on the atlas grid (11 x 11)
		Vector2 atlasCoordinate = GetAtlasCoordinateForMyUnit(unitType);

		//the center offset moves us from the atlasCoordinate (which is at the lower left corner of a tile) to the center
		Vector2 centerOffset = new Vector2 (0.5f, 0.5f);

		//the atlas UVs take the atlas coordinate and center offset and add a vector to the points of the hexagons
		//for each index.  There are 7 vertices for each hexagon.  Note that through trial and error the vertices
		//for this hexagon are 0-4-3-1-2-6-5.  The import from blender did some funny things with the vertices
		Vector2[] atlasUVs = new Vector2[7];
		atlasUVs[0] = ((atlasCoordinate + centerOffset) / tilesPerRow);														//center UV
		atlasUVs[4] = ((atlasCoordinate + centerOffset + new Vector2(0.0f, 0.5f)) / tilesPerRow);								//up UV
		atlasUVs[3] = ((atlasCoordinate + centerOffset + new Vector2(Mathf.Sqrt (3.0f) / 4.0f, 0.25f)) / tilesPerRow);		//up-right UV
		atlasUVs[1] = ((atlasCoordinate + centerOffset + new Vector2(Mathf.Sqrt (3.0f) / 4.0f, -0.25f)) / tilesPerRow);		//down-right UV
		atlasUVs[2] = ((atlasCoordinate + centerOffset + new Vector2(0.0f, -0.5f)) / tilesPerRow);							//down UV
		atlasUVs[6] = ((atlasCoordinate + centerOffset + new Vector2(-Mathf.Sqrt (3.0f) / 4.0f, -0.25f)) / tilesPerRow);		//down-left UV
		atlasUVs[5] = ((atlasCoordinate + centerOffset + new Vector2(-Mathf.Sqrt (3.0f) / 4.0f, 0.25f)) / tilesPerRow);		//up-left UV

		//return the atlasUVs to the method call
		return atlasUVs;
	}

	//this function sets the UVs for a unit picking from a solid color block in the atlas
	//it shrinks down the UVs so that the solid colors can be away from the edges
	private static Vector2[] GetUVsFromAtlasForUnitSolidColor (MyUnit unitType){

		//declare atlasCoordinate Vector2 - this is the X-Y coordinate where the tile is on the atlas grid (11 x 11)
		Vector2 atlasCoordinate = GetAtlasCoordinateForMyUnit (unitType);

		//the center offset moves us from the atlasCoordinate (which is at the lower left corner of a tile) to the center
		Vector2 centerOffset = new Vector2 (0.5f, 0.5f);

		//this is the shrink factor to pull the uvs in away from the edges, since i shrunk the solid colors by a few pixels per side
		float scaleFactor = .8f;

		//the atlas UVs take the atlas coordinate and center offset and add a vector to the points of the hexagons
		//for each index.  There are 7 vertices for each hexagon.  Note that through trial and error the vertices
		//for this he	xagon are 0-4-3-1-2-6-5.  The import from blender did some funny things with the vertices
		Vector2[] atlasUVs = new Vector2[7];
		atlasUVs[0] = ((atlasCoordinate + centerOffset) / tilesPerRow);														//center UV
		atlasUVs[4] = ((atlasCoordinate + centerOffset + new Vector2(0.0f, 0.5f) * scaleFactor) / tilesPerRow);								//up UV
		atlasUVs[3] = ((atlasCoordinate + centerOffset + new Vector2(Mathf.Sqrt (3.0f) / 4.0f, 0.25f) * scaleFactor) / tilesPerRow);		//up-right UV
		atlasUVs[1] = ((atlasCoordinate + centerOffset + new Vector2(Mathf.Sqrt (3.0f) / 4.0f, -0.25f) * scaleFactor) / tilesPerRow);		//down-right UV
		atlasUVs[2] = ((atlasCoordinate + centerOffset + new Vector2(0.0f, -0.5f) * scaleFactor) / tilesPerRow);							//down UV
		atlasUVs[6] = ((atlasCoordinate + centerOffset + new Vector2(-Mathf.Sqrt (3.0f) / 4.0f, -0.25f) * scaleFactor) / tilesPerRow);		//down-left UV
		atlasUVs[5] = ((atlasCoordinate + centerOffset + new Vector2(-Mathf.Sqrt (3.0f) / 4.0f, 0.25f) * scaleFactor) / tilesPerRow);		//up-left UV

		//return the atlasUVs to the method call
		return atlasUVs;
	}

	//this class is meant to be called by other classes, which will pass it the hexMapTile, the Vector2 uv array,
	//and an index value of where in the uv array the current hex uv coordinates start
	//it returns the updated Vector2 uv array, with the 7 uv coordinates set to the appropriate 
	//location in the TileAtlas
	private static Vector2[] SetUVsOfMapTile(HexMapTile hexMapTile,Vector2[] uv, int indexStart){	

		//cache the tile type from the HexMapTile passed to the method
		HexMapTile.TileType tileType = hexMapTile.tileType;

		//cache the hex location from the HexMapTile passed to the method
		Hex hexLocation = hexMapTile.hexLocation;

		//declare a vector2 array to hold the UV coordinates, assign the uvs passed to the function
		Vector2[] hexUV = uv;

		//declare a vector2 array of the atlas UVs, and call the private static method to return those UVs based on the tile passed
		Vector2[] atlasUVs = GraphicsManager.GetUVsFromAtlasForTile (hexMapTile);

		//this loop updates the specific uvs in the array for the tile that was passed to the method
		for (int i = 0; i < 7; i++) {
			hexUV [indexStart + i] = atlasUVs [i];
		}

		//return the entire uv array back to the calling class where it can replace the existing uv array
		return hexUV;

	}

	//this method takes as arguments a hexMapTile and a TileMap, and it updates the graphic for that tile in the tileMap
	//it does this by updating the UV coordinates of that tile to match the tileType
	private static void UpdateGraphicMapTile(HexMapTile _hexMapTile){
		//cache the tile type from the HexMapTile passed to the method
		HexMapTile.TileType tileType = _hexMapTile.tileType;

		//cache the hex index for the hexMapTile passed to the method
		int hexIndex = _hexMapTile.hexIndex;

		//cache the tile map passed to the method
		TileMap tileMap = _hexMapTile.tileMap;

		//cache the tileMap mesh
		Mesh tileMapMesh = tileMap.GetComponent<MeshFilter>().mesh;

		//get the uv array from the tilemap mesh
		Vector2[] tileMapUVs = tileMapMesh.uv;

		//get new UVs for the hexagon being set
		Vector2[] atlasUVs = GraphicsManager.GetUVsFromAtlasForTile (_hexMapTile);

		//this loop updates the specific uvs in the array for the tile that was passed to the method
		for (int i = 0; i < 7; i++) {
			tileMapUVs [7 * hexIndex + i] = atlasUVs [i];

		}

		//update the mesh with the new UVs
		tileMapMesh.uv = tileMapUVs;
	}


	//this class is meant to be called by other classes, which will pass it a mesh and the desired uniform material enum
	//it returns a vector2 array of uvs sized equal to the number of vertices in the mesh
	//all the uvs will be set to a single point in the tileAtlas
	private static Vector2[] SetUVsOfUniformObject(Mesh objectMesh, MyUniformMaterial objectUniformMaterial){

		//cache the vertices of the mesh passed to the method in a vector3 array
		Vector3[] objectVertices = objectMesh.vertices;

		//declare a vector2 array for the uvs.  It needs to have length matching the vertices
		Vector2[] objectUVs = new Vector2[objectMesh.vertices.Length];

		//this loop sets all the uvs equal to the center of the common material tile in the atlas using the private method
		for (int i = 0	; i < objectMesh.vertices.Length; i++) {
			objectUVs [i] = GetUVsFromAtlasForUniformObject (objectUniformMaterial);
		}

		//the entire uv array is returned to the calling class, where it can be set to the mesh.uv
		return objectUVs;
			
	}



	//this class is meant to be called by other classes, which will pass it a mesh and the desired unit enum
	//it returns a vector2 array of uvs sized equal to the number of vertices in the mesh, which should be 7
	private static Vector2[] SetUVsOfUnit(Mesh objectMesh, MyUnit objectUnit){

		//cache the vertices of the mesh passed to the method in a vector3 array
		Vector3[] objectVertices = objectMesh.vertices;

		//declare a vector2 array for the uvs.  It needs to have length matching the vertices
		Vector2[] objectUVs = new Vector2[objectMesh.vertices.Length];

		//declare a vector2 array of the atlas UVs, and call the private static method to return those UVs based on the tile passed
		Vector2[] atlasUVs = GraphicsManager.GetUVsFromAtlasForUnit (objectUnit);

		//this loop sets all the uvs equal to the center of the common material tile in the atlas using the private method
		for (int i = 0	; i < objectMesh.vertices.Length; i++) {
			objectUVs [i] = atlasUVs[i];
		}

		//the entire uv array is returned to the calling class, where it can be set to the mesh.uv
		return objectUVs;

	}

	//this function calls GetUVsFromAtlasForUnitSolidColor instead of GetUVsFromAtlasForUnit to get the uvs
	private static Vector2[] SetUVsOfUnitSolidColor(Mesh objectMesh, MyUnit objectUnit){

		//cache the vertices of the mesh passed to the method in a vector3 array
		Vector3[] objectVertices = objectMesh.vertices;

		//declare a vector2 array for the uvs.  It needs to have length matching the vertices
		Vector2[] objectUVs = new Vector2[objectMesh.vertices.Length];

		//declare a vector2 array of the atlas UVs, and call the private static method to return those UVs based on the tile passed
		Vector2[] atlasUVs = GraphicsManager.GetUVsFromAtlasForUnitSolidColor (objectUnit);

		//this loop sets all the uvs equal to the center of the common material tile in the atlas using the private method
		for (int i = 0	; i < objectMesh.vertices.Length; i++) {
			objectUVs [i] = atlasUVs[i];
		}

		//the entire uv array is returned to the calling class, where it can be set to the mesh.uv
		return objectUVs;

	}

	//this function will initialize the graphics for the tilemap
	private void AssignTileMapGraphics(TileMap tileMap){

		//loop through each hex in the tileMap and assign graphics based on the tile type
		foreach (KeyValuePair<Hex,HexMapTile> entry in tileMap.HexMap){

			//set the graphic of the hexMapTile
			UpdateGraphicMapTile (entry.Value);

		}

	}

	//function to assign graphics to a newly created hexcursor
	private void AssignNewCursorGraphics(HexCursor hexCursor){

		//get the mesh from the new cursor
		Mesh newHexCursorMesh = ((MeshFilter)hexCursor.GetComponentInChildren<MeshFilter> ()).mesh;

		//get the UVs from the new cursor
		Vector2[] newSelectionCursorUVs = new Vector2[newHexCursorMesh.vertices.Length];

		//map the UVs to the appropriate image from the TileAtlas material
		newSelectionCursorUVs = GraphicsManager.SetUVsOfUniformObject (newHexCursorMesh, GraphicsManager.MyUniformMaterial.TransparentYellow);
		newHexCursorMesh.uv = newSelectionCursorUVs;

		return;

	}

	//function to assign graphics to a range tile
	private void AssignRangeTileGraphics(RangeTile tile){

		//start by getting the mesh of the object
		Mesh tileMesh = tile.GetComponentInChildren<MeshFilter>().mesh;

		//also get the type
		MyUnit tileUnit = tile.currentTileImage;

		//get the UVs from the new unit
		Vector2[] newRangeTileUVs = new Vector2[tileMesh.vertices.Length];

		//map the UVs to the appropriate image from the TileAtlas material
		//this will eventually need updated to know what player color to use to set the graphics correctly
		newRangeTileUVs = GraphicsManager.SetUVsOfUnitSolidColor (tileMesh, tileUnit);
		tileMesh.uv = newRangeTileUVs;

		return;

	}

	//function to assign graphics to a targeting emblem
	private void AssignTargetingEmblemGraphics(TargetingEmblem targetingEmblem){

		//start by getting the mesh of the object
		Mesh targetingEmblemMesh = targetingEmblem.GetComponentInChildren<MeshFilter>().mesh;

		//also get the type
		MyUnit targetingEmblemUnitType = targetingEmblem.currentTileImage;

		//get the UVs from the new unit
		Vector2[] newTargetingEmblemUVs = new Vector2[targetingEmblemMesh.vertices.Length];

		//map the UVs to the appropriate image from the TileAtlas material
		//this will eventually need updated to know what player color to use to set the graphics correctly
		newTargetingEmblemUVs = GraphicsManager.SetUVsOfUnit (targetingEmblemMesh, targetingEmblemUnitType);
		targetingEmblemMesh.uv = newTargetingEmblemUVs;

		return;

	}


	//function to assign graphics to a targeting cursor
	private void AssignTargetingCursorGraphics(TargetingCursor targetingCursor){

		//start by getting the mesh of the object
		Mesh targetingCursorMesh = targetingCursor.GetComponentInChildren<MeshFilter>().mesh;

		//also get the type
		MyUnit targetingCursorUnitType = targetingCursor.currentTileImage;

		//get the UVs from the new unit
		Vector2[] newTargetingCursorUVs = new Vector2[targetingCursorMesh.vertices.Length];

		//map the UVs to the appropriate image from the TileAtlas material
		//this will eventually need updated to know what player color to use to set the graphics correctly
		newTargetingCursorUVs = GraphicsManager.SetUVsOfUnit (targetingCursorMesh, targetingCursorUnitType);
		targetingCursorMesh.uv = newTargetingCursorUVs;

		return;

	}

	//function to assign graphics to a selection cursor
	private void AssignSelectionCursorGraphics(SelectionCursor selectionCursor){

		//start by getting the mesh of the object
		Mesh selectionCursorMesh = selectionCursor.GetComponentInChildren<MeshFilter>().mesh;

		//also get the type
		MyUnit selectionCursorUnitType = selectionCursor.currentTileImage;

		//get the UVs from the new unit
		Vector2[] newSelectionCursorUVs = new Vector2[selectionCursorMesh.vertices.Length];

		//map the UVs to the appropriate image from the TileAtlas material
		//this will eventually need updated to know what player color to use to set the graphics correctly
		newSelectionCursorUVs = GraphicsManager.SetUVsOfUnit (selectionCursorMesh, selectionCursorUnitType);
		selectionCursorMesh.uv = newSelectionCursorUVs;

		return;

	}

	//this function will set the RawImage offset based on the unit type
	private void SetRawImageOffsetForUnit(CombatUnit combatUnit, RawImage rawImage){

		//we will need the UnitType for the combat unit
		MyUnit myUnit = GetCombatUnitMyUnit(combatUnit);

		//get the atlas coordinate for the combat unit
		Vector2 atlasCoordinate = GetAtlasCoordinateForMyUnit(myUnit);

		//check to make sure rawImage is not null
		if (rawImage != null) {

			//set the rawImage UV Rect based on the atlas coordinate
			rawImage.uvRect = new Rect (atlasCoordinate.x / tilesPerRow, atlasCoordinate.y / tilesPerRow, 1.0f / tilesPerRow, 1.0f / tilesPerRow);

			//TODO - this should be removed
			/*
			//check if the raw image UV rect X is zero - this sometimes causes a line to appear
			if (rawImage.uvRect.x < .001f) {

				//adjust the X to be just above zero
				rawImage.uvRect = new Rect (-.0005f, rawImage.uvRect.y, rawImage.uvRect.width, rawImage.uvRect.height);

			}

			//check if the raw image UV rect y is zero - this sometimes causes a line to appear
			if (rawImage.uvRect.y < .001f) {

				//adjust the X to be just above zero
				rawImage.uvRect = new Rect (rawImage.uvRect.x, -.0005f, rawImage.uvRect.width, rawImage.uvRect.height);

			}
			*/

		} else {

			Debug.LogError ("The RawImage passed to SetRawImageOffsetForUnit() is null");

		}

	}

	//this function will set the RawImage offset based on the unit type
	private void SetRawImageOffsetForUnitType(MyUnit myUnit, RawImage rawImage){
		
		//get the atlas coordinate for the combat unit
		Vector2 atlasCoordinate = GetAtlasCoordinateForMyUnit(myUnit);

		//check to make sure rawImage is not null
		if (rawImage != null) {
			
			//set the rawImage UV Rect based on the atlas coordinate
			rawImage.uvRect = new Rect (atlasCoordinate.x / tilesPerRow, atlasCoordinate.y / tilesPerRow, 1.0f / tilesPerRow, 1.0f / tilesPerRow);

			//TODO - I think by shrinking the solid colors in the atlas this can be removed
			/*
			//check if the raw image UV rect X is zero - this sometimes causes a line to appear
			if (rawImage.uvRect.x < .001f) {
				
				//adjust the X to be just above zero
				rawImage.uvRect = new Rect (.0005f, rawImage.uvRect.y, rawImage.uvRect.width, rawImage.uvRect.height);

			}

			//check if the raw image UV rect y is zero - this sometimes causes a line to appear
			if (rawImage.uvRect.y < .001f) {

				//adjust the X to be just above zero
				rawImage.uvRect = new Rect (rawImage.uvRect.x, .0005f, rawImage.uvRect.width, rawImage.uvRect.height);

			}
			*/

		} else {

			Debug.LogError ("The RawImage passed to SetRawImageOffsetForUnit() is null");

		}

	}


	//this function will set the RawImage offset based on the tile type
	private void SetRawImageOffsetForTile(HexMapTile hexMapTile, RawImage rawImage){

		//get the atlas coordinate for the tileType
		Vector2 atlasCoordinate = GetAtlasCoordinateForHexMapTile(hexMapTile);

		//check to make sure rawImage is not null
		if (rawImage != null) {

			//set the rawImage UV Rect based on the atlas coordinate
			rawImage.uvRect = new Rect (atlasCoordinate.x / tilesPerRow, atlasCoordinate.y / tilesPerRow, 1.0f / tilesPerRow, 1.0f / tilesPerRow);

		} else {

			Debug.LogError ("The RawImage passed to SetRawImageOffsetForTile() is null");

		}

	}

	//this function will set the RawImage offset for a planet icon
	private void SetRawImageOffsetForPlanetIcon(HexMapTile.TileType planetName, RawImage rawImage){

		//get the atlas coordinate for the tileType
		Vector2 atlasCoordinate = GetAtlasCoordinateForHexMapTileType(planetName);

		//check to make sure rawImage is not null
		if (rawImage != null) {

			//set the rawImage UV Rect based on the atlas coordinate
			rawImage.uvRect = new Rect (atlasCoordinate.x / tilesPerRow, atlasCoordinate.y / tilesPerRow, 1.0f / tilesPerRow, 1.0f / tilesPerRow);

		} else {

			Debug.LogError ("The RawImage passed to SetRawImageOffsetForTile() is null");

		}

	}


	//this function will take a combat unit as input and return a GameGraphics.MyUnit as output
	private MyUnit GetCombatUnitMyUnit(CombatUnit combatUnit){

		MyUnit myUnit;

		//check if the combatUnit is null.  If so, assign myUnit as blank
		if (combatUnit == null) {

			myUnit = MyUnit.Blank;

		}

		//the else condition is that the combatUnit is not null
		else {

			//this will be a nested switch case, checking color and unit type to determine the correct graphics to get from the atlas
			switch (combatUnit.owner.color) {

			case Player.Color.Blue:

				switch (combatUnit.unitType) {

				case CombatUnit.UnitType.Starship:
					myUnit = MyUnit.BlueStarship;
					break;

				case CombatUnit.UnitType.Destroyer:
					myUnit = MyUnit.BlueDestroyer;
					break;

				case CombatUnit.UnitType.BirdOfPrey:

					//check if unit is cloaked
					if (combatUnit.GetComponent<CloakingDevice> ().isCloaked == true) {
						
						myUnit = MyUnit.BlueBirdCloaked;

					}
					//the else condition is that the unit is not cloaked
					else {
						
						myUnit = MyUnit.BlueBird;

					}
					break;

				case CombatUnit.UnitType.Scout:
					myUnit = MyUnit.BlueScout;
					break;

				case CombatUnit.UnitType.Starbase:
					myUnit = MyUnit.BlueStarbase;
					break;

				//the default case will show the blue movement range tile - this will indicate something has gone wrong
				default:
					myUnit = MyUnit.MovementRange;
					Debug.LogError ("We somehow assigned a MyUnit that wasn't valid");
					break;

				}
				break;

			case Player.Color.Green:

				switch (combatUnit.unitType) {

				case CombatUnit.UnitType.Starship:
					myUnit = MyUnit.GreenStarship;
					break;

				case CombatUnit.UnitType.Destroyer:
					myUnit = MyUnit.GreenDestroyer;
					break;

				case CombatUnit.UnitType.BirdOfPrey:
					//check if unit is cloaked
					if (combatUnit.GetComponent<CloakingDevice> ().isCloaked == true) {

						myUnit = MyUnit.GreenBirdCloaked;

					}
					//the else condition is that the unit is not cloaked
					else {

						myUnit = MyUnit.GreenBird;

					}
					break;

				case CombatUnit.UnitType.Scout:
					myUnit = MyUnit.GreenScout;
					break;

				case CombatUnit.UnitType.Starbase:
					myUnit = MyUnit.GreenStarbase;
					break;

				//the default case will show the blue movement range tile - this will indicate something has gone wrong
				default:
					myUnit = MyUnit.MovementRange;
					Debug.LogError ("We somehow assigned a MyUnit that wasn't valid");
					break;

				}
				break;

			case Player.Color.Purple:

				switch (combatUnit.unitType) {

				case CombatUnit.UnitType.Starship:
					myUnit = MyUnit.PurpleStarship;
					break;

				case CombatUnit.UnitType.Destroyer:
					myUnit = MyUnit.PurpleDestroyer;
					break;

				case CombatUnit.UnitType.BirdOfPrey:
					//check if unit is cloaked
					if (combatUnit.GetComponent<CloakingDevice> ().isCloaked == true) {

						myUnit = MyUnit.PurpleBirdCloaked;

					}
					//the else condition is that the unit is not cloaked
					else {

						myUnit = MyUnit.PurpleBird;

					}
					break;

				case CombatUnit.UnitType.Scout:
					myUnit = MyUnit.PurpleScout;
					break;

				case CombatUnit.UnitType.Starbase:
					myUnit = MyUnit.PurpleStarbase;
					break;

				//the default case will show the blue movement range tile - this will indicate something has gone wrong
				default:
					myUnit = MyUnit.MovementRange;
					Debug.LogError ("We somehow assigned a MyUnit that wasn't valid");
					break;

				}
				break;

			case Player.Color.Red:

				switch (combatUnit.unitType) {

				case CombatUnit.UnitType.Starship:
					myUnit = MyUnit.RedStarship;
					break;

				case CombatUnit.UnitType.Destroyer:
					myUnit = MyUnit.RedDestroyer;
					break;

				case CombatUnit.UnitType.BirdOfPrey:
					//check if unit is cloaked
					if (combatUnit.GetComponent<CloakingDevice> ().isCloaked == true) {

						myUnit = MyUnit.RedBirdCloaked;

					}
					//the else condition is that the unit is not cloaked
					else {

						myUnit = MyUnit.RedBird;

					}
					break;

				case CombatUnit.UnitType.Scout:
					myUnit = MyUnit.RedScout;
					break;

				case CombatUnit.UnitType.Starbase:
					myUnit = MyUnit.RedStarbase;
					break;

				//the default case will show the blue movement range tile - this will indicate something has gone wrong
				default:
					myUnit = MyUnit.MovementRange;
					Debug.LogError ("We somehow assigned a MyUnit that wasn't valid");
					break;

				}
				break;

			//the default case will show the blue movement range tile - this will indicate something has gone wrong
			default:
				myUnit = MyUnit.MovementRange;
				Debug.LogError ("We somehow assigned a MyUnit that wasn't valid");
				break;

			}
		}
		return myUnit;

	}

	//this function will get the atlas coordinate for a myUnit type
	private static Vector2 GetAtlasCoordinateForMyUnit(MyUnit myUnit){

		//declare atlasCoordinate Vector2 - this is the X-Y coordinate where the tile is on the atlas grid (11 x 11)
		Vector2 atlasCoordinate;

		//use switch type to determine which atlas location to use
		switch (myUnit){
		case MyUnit.BlueStarship:
			atlasCoordinate = new Vector2 (0, 4);
			break;
		case MyUnit.BlueDestroyer:
			atlasCoordinate = new Vector2 (1, 4);
			break;
		case MyUnit.BlueBird:
			atlasCoordinate = new Vector2 (2, 4);
			break;
		case MyUnit.BlueScout:
			atlasCoordinate = new Vector2 (3, 4);
			break;
		case MyUnit.BlueBirdCloaked:
			atlasCoordinate = new Vector2 (4, 4);
			break;
		case MyUnit.BlueFactory:
			atlasCoordinate = new Vector2 (9, 10);
			break;
		case MyUnit.BlueStarbase:
			atlasCoordinate = new Vector2 (0, 2);
			break;

		case MyUnit.GreenStarship:
			atlasCoordinate = new Vector2 (5, 4);
			break;
		case MyUnit.GreenDestroyer:
			atlasCoordinate = new Vector2 (6, 4);
			break;
		case MyUnit.GreenBird:
			atlasCoordinate = new Vector2 (7, 4);
			break;
		case MyUnit.GreenScout:
			atlasCoordinate = new Vector2 (8, 4);
			break;
		case MyUnit.GreenBirdCloaked:
			atlasCoordinate = new Vector2 (9, 4);
			break;
		case MyUnit.GreenFactory:
			atlasCoordinate = new Vector2 (10, 10);
			break;
		case MyUnit.GreenStarbase:
			atlasCoordinate = new Vector2 (1, 2);
			break;

		case MyUnit.PurpleStarship:
			atlasCoordinate = new Vector2 (0, 3);
			break;
		case MyUnit.PurpleDestroyer:
			atlasCoordinate = new Vector2 (1, 3);
			break;
		case MyUnit.PurpleBird:
			atlasCoordinate = new Vector2 (2, 3);
			break;
		case MyUnit.PurpleScout:
			atlasCoordinate = new Vector2 (3, 3);
			break;
		case MyUnit.PurpleBirdCloaked:
			atlasCoordinate = new Vector2 (4, 3);
			break;
		case MyUnit.PurpleFactory:
			atlasCoordinate = new Vector2 (10, 9);
			break;
		case MyUnit.PurpleStarbase:
			atlasCoordinate = new Vector2 (2, 2);
			break;

		case MyUnit.RedStarship:
			atlasCoordinate = new Vector2 (5, 3);
			break;
		case MyUnit.RedDestroyer:
			atlasCoordinate = new Vector2 (6, 3);
			break;
		case MyUnit.RedBird:
			atlasCoordinate = new Vector2 (7, 3);
			break;
		case MyUnit.RedScout:
			atlasCoordinate = new Vector2 (8, 3);
			break;
		case MyUnit.RedBirdCloaked:
			atlasCoordinate = new Vector2 (9, 3);
			break;
		case MyUnit.RedFactory:
			atlasCoordinate = new Vector2 (10, 8);
			break;
		case MyUnit.RedStarbase:
			atlasCoordinate = new Vector2 (3, 2);
			break;

		case MyUnit.SelectionCursor:
			atlasCoordinate = new Vector2 (4, 2);
			break;
		case MyUnit.SelectionCursor2:
			atlasCoordinate = new Vector2 (5, 2);
			break;
		case MyUnit.MovementRange:
			atlasCoordinate = new Vector2 (10, 3);
			break;
		case MyUnit.AttackRange:
			atlasCoordinate = new Vector2 (10, 1);  			
			break;
		case MyUnit.TractorBeamRange:
			atlasCoordinate = new Vector2 (10, 4);
			break;
		case MyUnit.ItemRange:
			atlasCoordinate = new Vector2 (9, 2);				
			break;
		case MyUnit.RepairRange:
			atlasCoordinate = new Vector2 (9, 1);			
			break;
		case MyUnit.RedCrosshair:
			atlasCoordinate = new Vector2 (7, 5);				
			break;
		case MyUnit.BlueCrosshair:
			atlasCoordinate = new Vector2 (8, 5);				
			break;
		case MyUnit.YellowCrosshair:
			atlasCoordinate = new Vector2 (9, 5);				
			break;
		case MyUnit.GreenCrosshair:
			atlasCoordinate = new Vector2 (8, 2);				
			break;

		case MyUnit.Blank:
			atlasCoordinate = new Vector2 (10, 0);
			break;

		case MyUnit.RedWormhole:
			atlasCoordinate = new Vector2 (0, 1);
			break;

		case MyUnit.BlueWormhole:
			atlasCoordinate = new Vector2 (1, 1);
			break;

		case MyUnit.RedMiniMap:
			atlasCoordinate = new Vector2 (2, 1);
			break;

		case MyUnit.BlueMiniMap:
			atlasCoordinate = new Vector2 (3, 1);
			break;

		case MyUnit.GreenMiniMap:
			atlasCoordinate = new Vector2 (4, 1);
			break;

		case MyUnit.PurpleMiniMap:
			atlasCoordinate = new Vector2 (5, 1);
			break;

		default:
			atlasCoordinate = new Vector2 (8, 10);
			break;
		}

		return atlasCoordinate;

	}

	//function to assign graphics to a colony
	private void AssignColonyGraphics(Colony colony){

		//start by getting the mesh of the object
		Mesh colonyMesh = colony.GetComponentInChildren<MeshFilter>().mesh;

		//also get the type
		MyUnit colonyUnitType = colony.currentTileImage;

		//get the UVs from the new unit
		Vector2[] newColonyUVs = new Vector2[colonyMesh.vertices.Length];

		//map the UVs to the appropriate image from the TileAtlas material
		newColonyUVs = GraphicsManager.SetUVsOfUnit (colonyMesh, colonyUnitType);
		colonyMesh.uv = newColonyUVs;

		return;

	}

	//function to assign graphics to a wormhole
	private void AssignWormholeGraphics(WormholeRotation wormhole){

		//start by getting the mesh of the object
		Mesh wormholeMesh = wormhole.GetComponentInChildren<MeshFilter>().mesh;

		//also get the type
		MyUnit wormholeUnitType = wormhole.currentTileImage;

		//get the UVs from the new unit
		Vector2[] newWormholeUVs = new Vector2[wormholeMesh.vertices.Length];

		//map the UVs to the appropriate image from the TileAtlas material
		newWormholeUVs = GraphicsManager.SetUVsOfUnit (wormholeMesh, wormholeUnitType);
		wormholeMesh.uv = newWormholeUVs;

		return;

	}

	//function to handle OnDestroy
	private void OnDestroy(){

		RemoveAllListeners ();

	}

	//function to remove all listeners
	private void RemoveAllListeners(){

		if (uiManager != null) {

			//remove a listener for the UnitPanel highlighted unit
			uiManager.GetComponent<UnitPanel>().OnSetHighlightedUnit.RemoveListener(combatUnitSetRawImageOffsetForUnitAction);

			//remove listener for setting planet icons
			uiManager.GetComponent<StatusPanel>().OnSetPlanetIcon.RemoveListener(hexMapTileTypeSetRawImageOffsetForPlanetIconAction);

			//remove listener to setting purchase ship icons
			uiManager.GetComponent<PurchaseManager>().OnSetPurchaseShipIcon.RemoveListener(unitSetRawImageOffsetForUnitTypeAction);

			//remove listener to status panel
			uiManager.GetComponent<StatusPanel>().OnSetShipIcon.RemoveListener(unitSetRawImageOffsetForUnitTypeAction);

		}

		//remove a listener for creating a new unit
		CombatUnit.OnCreateUnit.RemoveListener(combatUnitAssignNewUnitGraphicsAction);
		CombatUnit.OnCreateLoadedUnit.RemoveListener(combatUnitLoadedAssignNewUnitGraphicsAction);

		//remove a listener for creating a minimap cursor
		MiniMapCursor.OnCreateMiniMapCursor.RemoveListener(miniMapCursorAssignCursorGraphicsAction);

		//remove a listener for creating the tilemap
		TileMap.OnCreateTileMap.RemoveListener(tileMapAssignTileMapGraphicsAction);

		//remove a listener for creating a new hexCursor
		HexCursor.OnCreateHexCursor.RemoveListener(hexCursorAssignNewCursorGraphicsAction);

		//remove a listener for creating a new rangeTile
		RangeTile.OnCreateRangeTile.RemoveListener(rangeTileAssignRangeTileGraphicsAction);

		//remove a listener for creating a new targeting emblem
		TargetingEmblem.OnCreateTargetingEmblem.RemoveListener(targetingEmblemAssignTargetingEmblemGraphicsAction);
		TargetingEmblem.OnShowTargetingEmblem.RemoveListener(targetingEmblemAssignTargetingEmblemGraphicsAction);

		//remove a listener for creating a new targeting cursor
		TargetingCursor.OnCreateTargetingCursor.RemoveListener(targetingCursorAssignTargetingCursorGraphicsAction);
		TargetingCursor.OnUpdateTargetingCursor.RemoveListener(targetingCursorAssignTargetingCursorGraphicsAction);

		//remove a listener for selection cursor
		SelectionCursor.OnCreateSelectionCursor.RemoveListener(selectionCursorAssignSelectionCursorGraphicsAction);
		SelectionCursor.OnUpdateSelectionCursor.RemoveListener(selectionCursorAssignSelectionCursorGraphicsAction);

		//remove a listener for cloaking device events
		CloakingDevice.OnEngageCloakingDevice.RemoveListener(combatUnitAssignNewUnitGraphicsAction);
		CloakingDevice.OnDisengageCloakingDevice.RemoveListener(combatUnitAssignNewUnitGraphicsAction);

		//remove listener for colonizing a new planet
		HexMapTile.OnColonizeNewPlanet.RemoveListener(hexMapTileUpdateGraphicTileAction);

		//remove listener for creating a new colony
		Colony.OnCreateColony.RemoveListener(colonyAssignColonyGraphicsAction);

		//add listener for settling a new colony
		Colony.OnSettleColony.RemoveListener(colonyAssignColonyGraphicsAction);

		//remove a listener to wormhole creation
		WormholeRotation.OnCreateRotatingWormhole.RemoveListener(wormholeAssignGraphicsAction);

	}

}
