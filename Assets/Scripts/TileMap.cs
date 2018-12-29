using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

//since we're building a mesh with this class, make it require a mesh filter, mesh renderer, and mesh collider
[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(MeshRenderer))]
public class TileMap : MonoBehaviour {

	//Size_x is number of hexes in each row, or at least the first row.  The star trek map uses 1 less hex in even rows
	public int Size_x{

		get;
		private set;

	}

	//Size_z is the number of rows
	public int Size_z{

		get;
		private set;

	}

	//Tile_size is the height of each hexagon.  Hexagons are pointy-topped
	public float Tile_size{

		get;
		private set;

	}

	//Stretch_x is a multiplier factor on the hex width
	public float Stretch_x{

		get;
		private set;

	}

	//Stretch_z is a multiplier factor on the hex height
	public float Stretch_z {

		get;
		private set;

	}

	public Vector2 size {
		get;
		private set;
	}

	public Vector2 origin {
		get;
		private set;
	}

	public Layout layout {
		get;
		private set;
	}

	public float maxHeight {
		get;
		private set;
	}

	public float maxWidth {
		get;
		private set;
	}

	public Dictionary<Hex,HexMapTile> HexMap {

		get;
		private set;

	}

	public float hexHeight {
		get;
		private set;
	}

	public float hexWidth {
		get;
		private set;
	}

	//these lists are public get so that the movement range can exclude them from other colors
	public List<Hex> GreenStartTiles {

		get;
		private set;

	}

	public List<Hex> BlueStartTiles {

		get;
		private set;

	}

	public List<Hex> RedStartTiles {

		get;
		private set;

	}

	public List<Hex> PurpleStartTiles {

		get;
		private set;

	}

	public List<Hex> SunTiles {

		get;
		private set;

	}

	public List<Hex> NeutralStarbaseTiles {

		get;
		private set;

	}

    //this holds the prefab for the textMeshPro hex label
    public GameObject prefabHexLabel;

    //this holds the hex label parent object 
    public GameObject hexLabelParent;

    //this is the list of hex labels
    public List<GameObject> hexLabelList
    {
        get;
        private set;
    }

	// Use this for initialization
	public void Init () {

		Size_x = 32;
		Size_z = 31;
		Tile_size = 1.0f;
		Stretch_x = 1.0f;
		Stretch_z = 1.0f;

		BuildMesh ();

	}

	//this event is for when the CurrentMovementRange property changes
	public static TileMapEvent OnCreateTileMap = new TileMapEvent();

	//simple class derived from unityEvent to pass int variable
	public class TileMapEvent : UnityEvent<TileMap>{};


	private void BuildMesh(){

		//this vertex and triangle code generates a hex grid with pointy top hexes, every 2nd row is 1 hex shorter like Star Trek game
		//this code was built using the Hex classes.

		//number of tiles is based on 1/2 full rows, 1/2 rows that are one shorter.  Use modulus to account for odd number.
		int numberTiles = Size_z / 2 * Size_x + Size_z / 2 * (Size_x - 1) + Size_z % 2 * Size_x;

		//each hex has 7 vertices, which make 6 triangles
		int numberTriangles = numberTiles * 6;
		int numberVertices = numberTiles * 7;

		//generate the mesh data
		Vector3[] vertices = new Vector3[numberVertices];
		Vector3[] normals  = new Vector3[numberVertices];
		Vector2[] uv = new Vector2[numberVertices];

		int[] triangles = new int[numberTriangles * 3];

		//logic to define locations of all hex vertices
		hexHeight = Tile_size;						//height of pointy-top hex
		hexWidth = Tile_size * Mathf.Sqrt(3.0f)/2.0f;	//long leg = sqrt(3)/2 * hypotenuse 30-60-90 triangle

		//apply stretch factors
		//note that applying the same stretch factor to both x and z is effectively the same as changing the tile size
		hexHeight = hexHeight * Stretch_z;
		hexWidth = hexWidth * Stretch_x;


		//the max values are used for the UV mapping.
		//maxHeight is the top pixel row of the last row of hexagons
		//maxWidth is the last pixel column on the rightmost edge of the last hexagon in the first row
		maxHeight = (hexHeight * (.25f + .75f * Size_z));
		maxWidth = Size_x * hexWidth;

		//create hex array
		//hexArray is an array of Hex class objects.  There will be one hex for each tile on the board.
		//the array index will be incremented starting with the first hex in the first row
		//then it will index across the first row, then to the first hex in the 2nd row, across the 2nd row, etc
		Hex[] hexArray = new Hex[numberTiles];


		//the dictionary will (I think) be useful for game logic later.  It will store each hex tile on the map in cube coordinates
		//the thought is that I can pass it cube coordinates (q,r) and it will return the hex at that coordinates.
		//note that the s value (q, r, s) is calculated from q and r on the fly as q + r + s = 0.
		//the advantage of the dictionary is it will only contain valid game coordinates without any null values like a regular array

		//the old HexMap was a dictionary of Hexes
		//Dictionary<Vector2,Hex> HexMap = new Dictionary<Vector2,Hex> ();

		//the new dictionary is a dictionary of HexMapTiles - the tile will store game data in addition to just the hex location
		//Dictionary<Hex,HexMapTile> HexMap = new Dictionary<Hex,HexMapTile> ();
		HexMap = new Dictionary<Hex,HexMapTile> ();

		//hex 0,0,0 will be lower left
		//this nested loop runs through rows then columns.  Because the q value of the first hex in each row changes, the r_offset
		//is used to change the starting value of the loop.  That same offset is applied to the max value of the for loop.
		//the additional shortRowOffset will account for the even rows having one less hex
		//the hexArray is filled in by index, each hex by row, then by column
		//the hexMap dictionary is filled with (q,r) values to store the game map
		//it's possible one of hexArray and hexMap will ultimately not be needed


		//I need to set up some lists of initial tiles for the gameboard, so that the appropriate tiletypes can be assigned later
		//these lists represent all of the non-unit tiles that start in the game
		GreenStartTiles = new List<Hex>();
		GreenStartTiles.Add (new Hex (31, 0, -31));

		PurpleStartTiles = new List<Hex>();
		PurpleStartTiles.Add (new Hex (16, 30, -46));

		RedStartTiles = new List<Hex>();
		RedStartTiles.Add (new Hex (-15, 30, -15));

		BlueStartTiles = new List<Hex>();
		BlueStartTiles.Add (new Hex (0, 0, 0));

		List<Hex> RedWormholeTiles = new List<Hex>();
		RedWormholeTiles.Add (new Hex (-12, 24, -12));
		RedWormholeTiles.Add (new Hex (-9, 30, -21));
		RedWormholeTiles.Add (new Hex (25, 0, -25));
		RedWormholeTiles.Add (new Hex (28, 6, -34));

		List<Hex> BlueWormholeTiles = new List<Hex>();
		BlueWormholeTiles.Add (new Hex (19, 24, -43));
		BlueWormholeTiles.Add (new Hex (10, 30, -40));
		BlueWormholeTiles.Add (new Hex (-3, 6, -3));
		BlueWormholeTiles.Add (new Hex (6, 0, -6));

		List<Hex> MercuryTiles = new List<Hex>();
		MercuryTiles.Add (new Hex (7, 17, -24));

		List<Hex> VenusTiles = new List<Hex>();
		VenusTiles.Add (new Hex (5, 15, -20));

		List<Hex> EarthTiles = new List<Hex>();
		EarthTiles.Add (new Hex (10, 11, -21));

		List<Hex> MarsTiles = new List<Hex>();
		MarsTiles.Add (new Hex (13, 15, -28));

		List<Hex> JupiterTiles = new List<Hex>();
		JupiterTiles.Add (new Hex (-1, 24, -23));

		List<Hex> SaturnTiles = new List<Hex>();
		SaturnTiles.Add (new Hex (8, 5, -13));

		List<Hex> UranusTiles = new List<Hex>();
		UranusTiles.Add (new Hex (19, 4, -23));

		List<Hex> NeptuneTiles = new List<Hex>();
		NeptuneTiles.Add (new Hex (8, 27, -35));

		List<Hex> PlutoTiles = new List<Hex>();
		PlutoTiles.Add (new Hex (-5, 15, -10));

		List<Hex> CharonTiles = new List<Hex>();
		CharonTiles.Add (new Hex (22, 15, -37));

        List<Hex> ErisTiles = new List<Hex>();
        ErisTiles.Add(new Hex(1, 29, -30));

        List<Hex> CeresTiles = new List<Hex>();
        CeresTiles.Add(new Hex(15, 1, -16));

        List<Hex> AsteroidTiles = new List<Hex>();
		AsteroidTiles.Add (new Hex (3, 24, -27));
		AsteroidTiles.Add (new Hex (3, 23, -26));
		AsteroidTiles.Add (new Hex (4, 23, -27));
		AsteroidTiles.Add (new Hex (5, 23, -28));
		AsteroidTiles.Add (new Hex (6, 23, -29));

		AsteroidTiles.Add (new Hex (6, 21, -27));
		AsteroidTiles.Add (new Hex (7, 21, -28));
		AsteroidTiles.Add (new Hex (8, 21, -29));
		AsteroidTiles.Add (new Hex (9, 20, -29));

		AsteroidTiles.Add (new Hex (11, 20, -31));
		AsteroidTiles.Add (new Hex (11, 19, -30));
		AsteroidTiles.Add (new Hex (12, 18, -30));
		AsteroidTiles.Add (new Hex (13, 18, -31));
		AsteroidTiles.Add (new Hex (13, 17, -30));

		AsteroidTiles.Add (new Hex (15, 16, -31));
		AsteroidTiles.Add (new Hex (16, 15, -31));
		AsteroidTiles.Add (new Hex (16, 14, -30));
		AsteroidTiles.Add (new Hex (16, 13, -29));

		AsteroidTiles.Add (new Hex (15, 12, -27));
		AsteroidTiles.Add (new Hex (15, 11, -26));
		AsteroidTiles.Add (new Hex (14, 11, -25));
		AsteroidTiles.Add (new Hex (15, 10, -25));
		AsteroidTiles.Add (new Hex (15, 9, -24));

		AsteroidTiles.Add (new Hex (15, 7, -22));
		AsteroidTiles.Add (new Hex (14, 7, -21));
		AsteroidTiles.Add (new Hex (13, 8, -21));
		AsteroidTiles.Add (new Hex (13, 7, -20));
		AsteroidTiles.Add (new Hex (12, 8, -20));
		AsteroidTiles.Add (new Hex (12, 7, -19));

		AsteroidTiles.Add (new Hex (10, 8, -18));
		AsteroidTiles.Add (new Hex (9, 8, -17));
		AsteroidTiles.Add (new Hex (8, 9, -17));
		AsteroidTiles.Add (new Hex (8, 8, -16));
		AsteroidTiles.Add (new Hex (7, 9, -16));
		AsteroidTiles.Add (new Hex (7, 8, -15));

		AsteroidTiles.Add (new Hex (5, 10, -15));
		AsteroidTiles.Add (new Hex (4, 11, -15));
		AsteroidTiles.Add (new Hex (4, 12, -16));
		AsteroidTiles.Add (new Hex (3, 12, -15));
		AsteroidTiles.Add (new Hex (2, 13, -15));

		AsteroidTiles.Add (new Hex (3, 14, -17));
		AsteroidTiles.Add (new Hex (2, 15, -17));
		AsteroidTiles.Add (new Hex (1, 16, -17));
		AsteroidTiles.Add (new Hex (2, 16, -18));
		AsteroidTiles.Add (new Hex (2, 17, -19));

		AsteroidTiles.Add (new Hex (1, 19, -20));
		AsteroidTiles.Add (new Hex (1, 20, -21));
		AsteroidTiles.Add (new Hex (1, 21, -22));
		AsteroidTiles.Add (new Hex (2, 21, -23));
		AsteroidTiles.Add (new Hex (2, 22, -24));
		AsteroidTiles.Add (new Hex (1, 23, -24));
	
		SunTiles = new List<Hex>();
		SunTiles.Add (new Hex (8, 15, -23));

		List<Hex> SunRayTilesUpRight = new List<Hex>();
		SunRayTilesUpRight.Add (new Hex (8, 16, -24));

		List<Hex> SunRayTilesRight = new List<Hex>();
		SunRayTilesRight.Add (new Hex (9, 15, -24));


		List<Hex> SunRayTilesDownRight = new List<Hex>();
		SunRayTilesDownRight.Add (new Hex (9, 14, -23));

		List<Hex> SunRayTilesDownLeft = new List<Hex>();
		SunRayTilesDownLeft.Add (new Hex (8, 14, -22));

		List<Hex> SunRayTilesLeft = new List<Hex>();
		SunRayTilesLeft.Add (new Hex (7, 15, -22));

		List<Hex> SunRayTilesUpLeft = new List<Hex>();
		SunRayTilesUpLeft.Add (new Hex (7, 16, -23));

		NeutralStarbaseTiles = new List<Hex>();
		NeutralStarbaseTiles.Add (new Hex (2, 9, -11));
		NeutralStarbaseTiles.Add (new Hex (20, 9, -29));
		NeutralStarbaseTiles.Add (new Hex (14, 21, -35));
		NeutralStarbaseTiles.Add (new Hex (-4, 21, -17));

        //create hexLabelList
        hexLabelList = new List<GameObject>();

		//int q, r;
		int hexArrayIndex = 0;
		for (int r = 0; r < Size_z; r++) {
			int r_offset = Mathf.FloorToInt (r / 2);
			int r_shortRowOffset = r % 2;
			for (int q = - r_offset; q < Size_x - r_offset - r_shortRowOffset; q++) {
				hexArray [hexArrayIndex] = new Hex(q, r, -q - r);

				//determine the tileType that should go in this location based on the lists above
				HexMapTile.TileType currentHexTileType;
				if (GreenStartTiles.Contains (hexArray [hexArrayIndex])) {
					currentHexTileType = HexMapTile.TileType.GreenStart;
				}
				else if (PurpleStartTiles.Contains (hexArray [hexArrayIndex])) {
					currentHexTileType = HexMapTile.TileType.PurpleStart;
				} 
				else if (RedStartTiles.Contains (hexArray [hexArrayIndex])) {
					currentHexTileType = HexMapTile.TileType.RedStart;
				}
				else if (BlueStartTiles.Contains (hexArray [hexArrayIndex])) {
					currentHexTileType = HexMapTile.TileType.BlueStart;
				} 
				else if (RedWormholeTiles.Contains (hexArray [hexArrayIndex])) {
					currentHexTileType = HexMapTile.TileType.RedWormhole;
				} 
				else if (BlueWormholeTiles.Contains (hexArray [hexArrayIndex])) {
					currentHexTileType = HexMapTile.TileType.BlueWormhole;
				} 
				else if (MercuryTiles.Contains (hexArray [hexArrayIndex])) {
					currentHexTileType = HexMapTile.TileType.Mercury;
				} 
				else if (VenusTiles.Contains (hexArray [hexArrayIndex])) {
					currentHexTileType = HexMapTile.TileType.Venus;
				} 
				else if (EarthTiles.Contains (hexArray [hexArrayIndex])) {
					currentHexTileType = HexMapTile.TileType.Earth;
				} 
				else if (MarsTiles.Contains (hexArray [hexArrayIndex])) {
					currentHexTileType = HexMapTile.TileType.Mars;
				} 
				else if (JupiterTiles.Contains (hexArray [hexArrayIndex])) {
					currentHexTileType = HexMapTile.TileType.Jupiter;
				} 
				else if (SaturnTiles.Contains (hexArray [hexArrayIndex])) {
					currentHexTileType = HexMapTile.TileType.Saturn;
				} 
				else if (UranusTiles.Contains (hexArray [hexArrayIndex])) {
					currentHexTileType = HexMapTile.TileType.Uranus;
				} 
				else if (NeptuneTiles.Contains (hexArray [hexArrayIndex])) {
					currentHexTileType = HexMapTile.TileType.Neptune;
				} 
				else if (PlutoTiles.Contains (hexArray [hexArrayIndex])) {
					currentHexTileType = HexMapTile.TileType.Pluto;
				} 
				else if (CharonTiles.Contains (hexArray [hexArrayIndex])) {
					currentHexTileType = HexMapTile.TileType.Charon;
				}
                else if (ErisTiles.Contains(hexArray[hexArrayIndex]))
                {
                    currentHexTileType = HexMapTile.TileType.Eris;
                }
                else if (CeresTiles.Contains(hexArray[hexArrayIndex]))
                {
                    currentHexTileType = HexMapTile.TileType.Ceres;
                }
                else if (AsteroidTiles.Contains (hexArray [hexArrayIndex])) {
					currentHexTileType = HexMapTile.TileType.Asteroid;
				} 
				else if (SunTiles.Contains (hexArray [hexArrayIndex])) {
					currentHexTileType = HexMapTile.TileType.Sun;
				} 
				else if (SunRayTilesUpRight.Contains (hexArray [hexArrayIndex])) {
					currentHexTileType = HexMapTile.TileType.SunRayUpRight;
				} 
				else if (SunRayTilesRight.Contains (hexArray [hexArrayIndex])) {
					currentHexTileType = HexMapTile.TileType.SunRayRight;
				} 
				else if (SunRayTilesDownRight.Contains (hexArray [hexArrayIndex])) {
					currentHexTileType = HexMapTile.TileType.SunRayDownRight;
				} 
				else if (SunRayTilesDownLeft.Contains (hexArray [hexArrayIndex])) {
					currentHexTileType = HexMapTile.TileType.SunRayDownLeft;
				} 
				else if (SunRayTilesLeft.Contains (hexArray [hexArrayIndex])) {
					currentHexTileType = HexMapTile.TileType.SunRayLeft;
				} 
				else if (SunRayTilesUpLeft.Contains (hexArray [hexArrayIndex])) {
					currentHexTileType = HexMapTile.TileType.SunRayUpLeft;
				} 
				else if (NeutralStarbaseTiles.Contains (hexArray [hexArrayIndex])) {
					currentHexTileType = HexMapTile.TileType.NeutralStarbase;
				} 
				else {
					currentHexTileType = HexMapTile.TileType.EmptySpace;
				}


				//the new dictionary is a dictionary of HexMapTiles - the tile will store game data in addition to just the hex location
				HexMap.Add(hexArray [hexArrayIndex],new HexMapTile(hexArray [hexArrayIndex],hexArrayIndex,this,currentHexTileType));
				//Debug.Log("hexArrayIndex + " + hexArrayIndex + ", tiletype: " + currentHexTileType);
				hexArrayIndex++;

			}
		}

		//size is taken from the hex code taken from RedBlobGames.  It's a bit confusing, but I didn't want to exclude it, plus a lot
		//of the class functions take it as an argument.
		//size.x is the length of the side of a hexagon in the x direction
		//size.y is the length of the side of a hexagon in the z direction
		//origin is the (x, z) point location for the center of the first hexagon (first row, first column)
		size = new Vector2 (hexWidth * 2.0f / Mathf.Sqrt(3.0f) / 2.0f, hexHeight/2.0f);
		origin = new Vector2 (hexWidth / 2.0f, hexHeight / 2.0f);

		//layout defines a map configuration based on hex orientation, size, and origin point.
		//Layout does not actually create a map - it doesn't know how many rows and columns.
		//Instead, it is taking information about the geometry of any given hex, and it's class functions
		//return information about points in the hexagon based on the geometry relationships passed to it
		layout = new Layout (Layout.pointy,size,origin);

		//this loop gives the (x, z) coordinates of the vertices in the mesh.  
		//Layout.PolygonCenterAndCorners returns a list of 7 points - first the center, then the 6 corners
		//I am using the hexArray to pass Hex objects to the Layout methods
		//I could also use the dictionary to do this, but I would have to set up the same nested loop used to create the dictionary,
		//since I would need to use math logic to make sure I'm passing it the (q, r) coordinates in order again.
		for (int i = 0; i < hexArray.Length; i++) {
			//center first, then 6 surrounding vertex points, starting at 12, go clockwise
			Layout.PolygonCenterAndCornersV3(layout,hexArray[i]).CopyTo(vertices,7 * i);		
		}

		//define normals and UV
		//normals are always straight up for this 2D application
		//uv index is the vertex (x, z) divided by the max values - this will map u,v to (0,0) and (x = 0, z = 0)
		//and (1,1) at (x = max, z = max).  Note that both (0,0) and (1,1) are actually outside the mesh
		//because the corners of the hexagons cut these off.
		for (int i = 0; i < vertices.Length; i++) {
			
			normals [i] = Vector3.up;

		}

		/*
		//this loop assigns uv coordiates for each hex based on the TileType, using the Tile Atlas
		//the GameGraphics.SetUVsOfMapTile returns a Vector2 array size 7, which represent the UV coordinates of a hex
		for (int i = 0; i < hexArray.Length; i++) {
			uv = GraphicsManager.SetUVsOfMapTile (HexMap [hexArray [i]], uv, 7 * i);

		}
		*/

		//Debug.Log ("Done Vertices!");	


		//the triangle loop builds mesh triangles from the vertices.  Triangles are formed from the origin point 
		//at each hex as the first coordinate of each triangle.  Each hex is made up of 6 triangles, starting with
		//the upper-right one and looping clockwise.  
		//Note that the last triangle needs to use the top point again to close the hexagon loop.
		for (int i = 0; i < numberTiles; i++) {
			int triangleOffset = 18 * i;
			int vertexOffset = 7 * i;
			triangles [triangleOffset + 0] = vertexOffset;
			triangles [triangleOffset + 1] = vertexOffset + 1;
			triangles [triangleOffset + 2] = vertexOffset + 2;

			triangles [triangleOffset + 3] = vertexOffset;
			triangles [triangleOffset + 4] = vertexOffset + 2;
			triangles [triangleOffset + 5] = vertexOffset + 3;

			triangles [triangleOffset + 6] = vertexOffset;
			triangles [triangleOffset + 7] = vertexOffset + 3;
			triangles [triangleOffset + 8] = vertexOffset + 4;

			triangles [triangleOffset + 9] = vertexOffset;
			triangles [triangleOffset + 10] = vertexOffset + 4;
			triangles [triangleOffset + 11] = vertexOffset + 5;

			triangles [triangleOffset + 12] = vertexOffset;
			triangles [triangleOffset + 13] = vertexOffset + 5;
			triangles [triangleOffset + 14] = vertexOffset + 6;

			triangles [triangleOffset + 15] = vertexOffset;
			triangles [triangleOffset + 16] = vertexOffset + 6;
			triangles [triangleOffset + 17] = vertexOffset + 1;
		}
			
		//Debug.Log ("Done Triangles!");


		//now that vetices, triangles, normals, and uvs are created, we can instantiate a mesh
		//and assign it these properties
		Mesh mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.normals = normals;
		mesh.uv = uv;


		//assign a mesh filter, mesh renderer, and mesh collider
		MeshFilter mesh_filter = GetComponent<MeshFilter> ();
		//MeshRenderer mesh_renderer = GetComponent<MeshRenderer> ();
		MeshCollider mesh_collider = GetComponent<MeshCollider> ();


		//assign our mesh to the mesh filter
		//assign our mesh to the mesh collider
		mesh_filter.mesh = mesh;
		//mesh_collider.sharedMesh.Clear ();
		mesh_collider.sharedMesh = mesh;

        //Debug.Log ("Done Mesh!");

        //create the hex labels
        CreateHexLabels(hexArray);


        //invoke the create tilemap event
        OnCreateTileMap.Invoke(this);

	}

    //this function creates the hexLabels
    private void CreateHexLabels(Hex[] hexArray)
    {
        //iterate through each hex
        for(int i = 0; i < hexArray.Length; i++)
        {
            //create a hex label at each hex center
            GameObject newLabel = Instantiate(prefabHexLabel, HexToWorldCoordinates(hexArray[i]), Quaternion.identity, hexLabelParent.transform);

            //update the text label for the new label
            newLabel.GetComponentInChildren<TextMeshPro>().text = OffsetCoord.OffsetCoordToString(OffsetCoord.RoffsetFromCube(OffsetCoord.ODD, hexArray[i]));

            hexLabelList.Add(newLabel);

        }
    }

	//let'd create a helper function that converts hex coordinates to worldspace coordinates
	public Vector3 HexToWorldCoordinates(Hex hex){

		//define a vector3 for local XYZ coordinates and convert the hex to local space
		Vector3 localXYZCoordinates = Layout.HexToPixelV3(layout, hex);

		//next, I convert those local coordinates to world coordinates
		Vector3 worldXYZCoordinates = new Vector3 ((transform.localToWorldMatrix * (localXYZCoordinates)).x + transform.position.x,
			(transform.localToWorldMatrix * (localXYZCoordinates)).y + transform.position.y,
			(transform.localToWorldMatrix * (localXYZCoordinates)).z + transform.position.z);

		return worldXYZCoordinates;

	}

	//let's also create a function that converts a worldspace coordinate to a hex coordinate
	//this could be used for raycast hits
	public Hex WorldToRoundedHexCoordinates (Vector3 worldPoint){

		//first, I need to translate the world point to a local point in the tileMap
		Vector3 localXYZCoordinates;
		localXYZCoordinates.x = (transform.worldToLocalMatrix * (worldPoint - transform.position)).x;
		localXYZCoordinates.y = (transform.worldToLocalMatrix * (worldPoint - transform.position)).y;
		localXYZCoordinates.z = (transform.worldToLocalMatrix * (worldPoint - transform.position)).z;

		//next, I need to convert these local coordinates to a fractional hex (a point is likely to be between hexes)
		FractionalHex fractionalHex = Layout.PixelToHexV3(layout, localXYZCoordinates);

		//next, I need to round that to the nearest hex on the map
		Hex worldToRoundedHex = FractionalHex.HexRound(fractionalHex);

		return worldToRoundedHex;

	}


	//I am moving the pathfinding functions to tilemap where it can be called as a public function.
	//this function will generate the path from our current hex to the target hex
	//the function needs to be passed the starting hex, the target hex, and a list of hexes to limit the path to (such as the reachable hexes)
	//this is public because it is not changing the tileMap - it is just returning information based on the tileMap geometry
	public List<Hex> GeneratePath(Hex startingHex, Hex targetHex, List<Hex> eligibleHexes) {

		//this function will require using Djikstra's algorithm for pathfinding
		//first, we need to get a list of all hexes in our graph.  This could be the entire map,
		//but for my purposes I can limit it to the reachable hexes.

		//we can limit the list to tiles within the list passed to the function
		List<Hex> graph = new List<Hex> (eligibleHexes);

		//I can then set up a list "Q" which is the unvisited hexes
		List<Hex> Q = new List<Hex>();

		//next, we need 2 dictionaries, distance and previous
		//distance will have key of a hex and a value of a float which tracks the distance from the starting point to that hex
		//previous will have a key of a hex, and a value of hex, which is the hex right before that hex that gets there with the shortest distance

		Dictionary<Hex,float> distance = new Dictionary<Hex,float>();
		Dictionary<Hex,Hex> previous = new Dictionary<Hex,Hex> ();

		//next, I can set up the source (starting point) hex and destination (target) hex

		Hex source = startingHex;
		Hex target = targetHex;

		//next, I can initialize the source hex
		distance [source] = 0.0f;
		previous [source] = null;

		//next, I can initialiize the rest of the unvisited hexes with infinite distance and no previous, and add them to Q
		foreach (Hex v in graph) {
			//I can skip source because I already initialized it
			if (v != source) {
				distance [v] = Mathf.Infinity;
				previous [v] = null;
			}

			Q.Add (v);
		}

		//My graph doesn't include the source - I need to explicitly add it
		Q.Add(source);

		//next, I set up a while loop to iterate while there are still unvisited hexes in Q
		while (Q.Count > 0) {
			//u will be the hex in Q with the smallest distance
			Hex u = null;

			//loop over all hexes in Q
			foreach (Hex possibleU in Q) {
				//if we have no u yet, make the first hex checked u
				//if we already have a u, check if the possibleU is shorter than the current u
				//if it is, make that the new u
				if (u == null || distance [possibleU] < distance [u]) {
					u = possibleU;
				}


			}


			//if u is our target, we have found the shortest path to the target and can exit the while loop
			if (u == target) {
				break;
			}

			//next, we can remove u from Q - it has now been visited
			Q.Remove (u);

			//next, we need to check the neighbors v of u and update the distance for all neighbors
			//find the neighbors - we have to account for wormhole warps
			List<Hex> neighborsU = new List<Hex> ();

			//we can use the Hex neighbors function to get a list of all neighbors on the map
			List<Hex> possibleNeighborsU = NeighborHexes (u,false);

			//then I need to downselect those to limit it to just the ones in our movement range
			foreach (Hex possibleNeighborU in possibleNeighborsU) {

				if (graph.Contains (possibleNeighborU)) {

					neighborsU.Add (possibleNeighborU);
				}
			}

			//next, we need to check the neighbors v of u and update the distance for all neighbors
			foreach (Hex v in neighborsU) {

				//set distance alt for the hex to the current distance plus the cost to enter the neighbor hex
				//for my game, these should all be distance of 1.  The impassable tiles should not be in the neighbor pool
				float alt = distance [u] + HexMap [u].movementCost;

				//check if this alt distance is lower than the current stored distance at the neighbor
				//this would indicate we found a shorter path to that hex
				if (alt < distance [v]) {

					//if we found a shorter distance, set the distance for that neighbor to the alt ditance
					distance [v] = alt;

					//also set the previous for that neighbor to the current hex
					previous [v] = u;

				}
			}

		}

		//since target has a previous, we can build a path
		List<Hex> CurrentPath = new List<Hex> ();

		Hex current = target;


		//if we get there, then either we found the shortest route to the target, or there is no valid path
		if (previous [target] == null) {

			//there is no path from our source to our target
			return CurrentPath;


		}

		while (current != null) {

			CurrentPath.Add (current);
			current = previous [current];

			//Debug.Log ("current = (" + current.q + ", " + current.r + ", " + current.s + ")");
		}

		//right now, CurrentPath defines a list from the target to the source
		//to get the source to the target, I need to reverse it
		CurrentPath.Reverse();

		return CurrentPath;

	}

	//this function returns a list of hexes that are neighbors to the hex passed to the function
	//targeting mode was added as an argument to allow tiles that have combat units as neighbors if we are targeting
	private List<Hex> NeighborHexes (Hex currentHex, bool targetingMode){

		//create a list of hexes neighboring the current hex
		List<Hex> neighborHexes = new List<Hex>();
		for (int i = 0; i < 6; i++) {

			//check to see if the added hex is actually part of the HexMap before addding it to the list
			//this gets rid of hexes that would be off the map
			if (HexMap.ContainsKey(Hex.Neighbor (currentHex, i)) == true) {

				//if we are in targeting mode, we want to allow an adjacent combat unit tile to be a valid neighbor
				if (targetingMode == true) {

					//check to see that the neighbor hex is actually passable OR is a combat unit
					if (HexMap [Hex.Neighbor (currentHex, i)].isPassable == true ||HexMap [Hex.Neighbor (currentHex, i)].tileCombatUnit != null  ) {

						neighborHexes.Add (Hex.Neighbor (currentHex, i));
						//Debug.Log ("added neighbor targetingMode is true");
					}

				}

				//else if we are not in targeting mode, we want to block any impassable tiles, including ships
				else if (targetingMode == false) {

					//check to see that the neighbor hex is actually passable - if it is not, like an asteroid, it doesn't get added
					if (HexMap [Hex.Neighbor (currentHex, i)].isPassable == true) {

						neighborHexes.Add (Hex.Neighbor (currentHex, i));
						//Debug.Log ("added neighbor targetingMode is false");
					}

				}

			}

			//check to see if we need to add blue wormhole neighbors
			if (HexMap [currentHex].tileType == HexMapTile.TileType.BlueWormhole) {

				//loop over each entry in the HexMap dictionary
				foreach (KeyValuePair<Hex,HexMapTile> entry in HexMap) {

					//check to see if a given entry is a blue wormhole
					if (entry.Value.tileType == HexMapTile.TileType.BlueWormhole) {

						//if we are in targeting mode, we want to allow an adjacent combat unit tile to be a valid neighbor
						if (targetingMode == true) {

							//check to see if that entry is passable OR there is a combat unit there
							if (entry.Value.isPassable == true || entry.Value.tileCombatUnit != null) {

								//add to neighbors with the rest of neighbors
								neighborHexes.Add (entry.Key);
							}

						}

						//else if we are not in targeting mode, we want to block any impassable tiles, including ships
						else if (targetingMode == false) {

							//check to see if that entry is passable (it could be blocked by a ship)
							if (entry.Value.isPassable == true) {

								//add to neighbors with the rest of neighbors
								neighborHexes.Add (entry.Key);
							}

						}

					}

				}
			}

			//check to see if we need to add red wormhole nieghbors
			if (HexMap [currentHex].tileType == HexMapTile.TileType.RedWormhole) {

				//loop over each entry in the HexMap dictionary
				foreach (KeyValuePair<Hex,HexMapTile> entry in HexMap) {

					//check to see if a given entry is a blue wormhole
					if (entry.Value.tileType == HexMapTile.TileType.RedWormhole) {

						//if we are in targeting mode, we want to allow an adjacent combat unit tile to be a valid neighbor
						if (targetingMode == true) {

							//check to see if that entry is passable OR there is a combat unit there
							if (entry.Value.isPassable == true || entry.Value.tileCombatUnit != null) {

								//add to neighbors with the rest of neighbors
								neighborHexes.Add (entry.Key);
							}

						}

						//else if we are not in targeting mode, we want to block any impassable tiles, including ships
						else if (targetingMode == false) {

							//check to see if that entry is passable (it could be blocked by a ship)
							if (entry.Value.isPassable == true) {

								//add to neighbors with the rest of neighbors
								neighborHexes.Add (entry.Key);
							}

						}
					}
				}
			}
		}

		return neighborHexes;

	}

	//this function returns true if you are adjacent to the requested tileType
	public bool AdjacentToTileType (Hex currentHex, HexMapTile.TileType targetTileType){

		bool isAdjacentToTargetTileType = false;

		for (int i = 0; i < 6; i++) {

			//check to see if the added hex is actually part of the HexMap before addding it to the list
			//this gets rid of hexes that would be off the map
			if (HexMap.ContainsKey(Hex.Neighbor (currentHex, i)) == true) {

				//check to see that the neighbor hex is the requested type
				if (HexMap [Hex.Neighbor (currentHex, i)].tileType == targetTileType) {

					isAdjacentToTargetTileType = true;
				
				}

			}

			//check to see if we need to add blue wormhole neighbors
			if (HexMap [currentHex].tileType == HexMapTile.TileType.BlueWormhole) {

				//loop over each entry in the HexMap dictionary
				foreach (KeyValuePair<Hex,HexMapTile> entry in HexMap) {

					//check to see if a given entry is a blue wormhole
					if (entry.Value.tileType == HexMapTile.TileType.BlueWormhole) {

						//check if the neighbor exists
						if(HexMap.ContainsKey(Hex.Neighbor (currentHex, i)) == true){
							
							//check to see that the neighbor hex is the requested type
							if (HexMap [Hex.Neighbor (currentHex, i)].tileType == targetTileType) {

								isAdjacentToTargetTileType = true;

							}

						}

					}

				}

			}

			//check to see if we need to add red wormhole nieghbors
			if (HexMap [currentHex].tileType == HexMapTile.TileType.RedWormhole) {

				//loop over each entry in the HexMap dictionary
				foreach (KeyValuePair<Hex,HexMapTile> entry in HexMap) {

					//check to see if a given entry is a red wormhole
					if (entry.Value.tileType == HexMapTile.TileType.RedWormhole) {

						//check if the neighbor exists
						if (HexMap.ContainsKey (Hex.Neighbor (currentHex, i)) == true) {

							//check to see that the neighbor hex is the requested type
							if (HexMap [Hex.Neighbor (currentHex, i)].tileType == targetTileType) {

								isAdjacentToTargetTileType = true;

							}

						}

					}
				}
			}
		}

		return isAdjacentToTargetTileType;

	}

	//this function uses a flood fill to return a list of all hexes that are within a given movement range of a starting hex
	//this function returns a list of hexMapTiles that are in range of a given hex
	//this is public because it is not changing the tileMap - it is just returning information based on the tileMap geometry
	public List<Hex> ReachableTiles(Hex startingHex, int MovementRange){

		//create a list of visited hexes
		List<Hex> visitedHexes = new List<Hex> ();

		//add the starting hex to the visited hexes
		visitedHexes.Add (startingHex);

		//call function to get all valid neighbor hexes to the starting hex
		List<Hex> neighborHexes = NeighborHexes (startingHex,false);

		//loop through equal to movement range
		for (int k = 1; k <= MovementRange; k++) {

			//create a list of neighbors called next neighbors
			List<Hex> nextNeighborHexes = new List<Hex> ();

			//loop through each hex in the initial neighbors
			foreach (Hex hex in neighborHexes) {

				//check to make sure that each hex hasn't been visited already
				if (visitedHexes.Contains (hex) == false) {

					//check to make sure that the hex is valid in the hexMap
					if (HexMap.ContainsKey(hex) == true) {

						//if it hasn't been visited yet, add it to the visited list
						visitedHexes.Add (hex);

						//get all the neighbors of this unvisited neighbor hex
						List<Hex> neighborsOfNeighborHexes = NeighborHexes (hex, false);

						//add all of those neighbors of neighbor to the next neighbor list
						foreach (Hex neighbor in neighborsOfNeighborHexes) {

							nextNeighborHexes.Add (neighbor);

						}

					}

				}

			}

			//clear the current neighbors after it's been completely looped through
			neighborHexes.Clear();

			//set the current neighbors equal to the next neighbors, so it can loop through
			//those at the next movement range point
			neighborHexes = nextNeighborHexes;

		}

		//remove the starting hex from visited hexes at the end
		//this will prevent the unit from highlighting itself
		visitedHexes.Remove (startingHex);

		return visitedHexes;


	}

	//create a slightly modified reachableTiles list for targeting
	//this is public so it can be accessed by phaser and torpedo sections
	//this is public because it is not changing the tileMap - it is just returning information based on the tileMap geometry
	public List<Hex> TargetableTiles(Hex startingHex, int targetingRange, bool includeStartingHex){

		//the approach here is to first calculate all tiles that we could move to at (range - 1).
		//once we have that list, we will expand 1 hex in all directions allowing movement onto units, but not other obstacles
		List<Hex> reachableMinusOneTiles = new List<Hex>();

		//use our movement function to get the tiles within n-1 range
		reachableMinusOneTiles = ReachableTiles (startingHex,targetingRange - 1);

		//reachable tiles does not return our own hex, but for the targeting logic to work I need to temporarily include it
		reachableMinusOneTiles.Add(startingHex);

		//create a list of visited hexes that starts with all our n-1 tiles
		List<Hex> visitedHexes = new List<Hex> (reachableMinusOneTiles);

		foreach (Hex hex in reachableMinusOneTiles) {

			List<Hex> neighborHexes = NeighborHexes (hex, true);

			foreach (Hex neighborHex in neighborHexes) {

				//check to make sure that each hex hasn't been visited already
				if (visitedHexes.Contains (neighborHex) == false) {

					//check to make sure that the hex is valid in the hexMap
					if (HexMap.ContainsKey (neighborHex) == true) {

						//if it hasn't been visited yet, add it to the visited list
						visitedHexes.Add (neighborHex);

					}

				}

			}

		}

		//now that we're at the end, I don't want our targeting range to include our current hex
		//if it still contains it, remove it if the includeStart is set to false
		if(visitedHexes.Contains(startingHex) && includeStartingHex == false){

			visitedHexes.Remove(startingHex);
		}

		return visitedHexes;

	}


		
}

