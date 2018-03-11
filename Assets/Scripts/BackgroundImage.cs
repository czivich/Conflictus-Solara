using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class BackgroundImage : MonoBehaviour {

	//we will need to access the TileMap to make sure we make the background image large enough
	private TileMap tileMap;

	//variable to control the parallax effect
	private float parallax;

	//variable to hold the mesh renderer
	private MeshRenderer meshRenderer;

	//variable to hold the material
	private Material backgroundMaterial;

	//variable to hold the background UV offset
	private Vector2 UVoffset;

	//vector2 to hold the offset to the center of the map
	private Vector3 mapCenterOffset;

	//vector3 to hold the center of the map hex
	private Hex mapCenterHex = new Hex(8, 15, -23);

	// Use this for initialization
	public void Init (float parallaxInput) {

		parallax = parallaxInput;

		tileMap = GameObject.FindGameObjectWithTag("TileMap").GetComponent<TileMap> ();
		Vector2 origin = tileMap.origin;
		float maxWidth = tileMap.maxWidth;
		float maxHeight = tileMap.maxHeight;


		//get the renderer and material
		meshRenderer = this.GetComponent<MeshRenderer>();
		backgroundMaterial = meshRenderer.material;

		//get the map center offset
		mapCenterOffset = new Vector3(
			tileMap.HexToWorldCoordinates(mapCenterHex).x,
			tileMap.HexToWorldCoordinates(mapCenterHex).y,
			tileMap.HexToWorldCoordinates(mapCenterHex).z
		);



		//let's create a background map so my actual map can have clear tiles and be see-through.  It will just be 1 quad.
		//I will have the background image be 25% larger than the actual map in all directions
		//y = -1 to make it below my actual map
		Vector3[] backgroundVertices = new Vector3 [4];
		backgroundVertices[0] = new Vector3 (origin.x - maxWidth / 4.0f, -1.0f, origin.y - maxHeight / 4.0f);  //lower left
		backgroundVertices[1] = new Vector3 (origin.x + maxWidth *1.25f, -1.0f, origin.y - maxHeight / 4.0f);  //lower right
		backgroundVertices[2] = new Vector3 (origin.x - maxWidth / 4.0f, -1.0f, origin.y + maxHeight * 1.25f);  //upper left
		backgroundVertices[3] = new Vector3 (origin.x + maxWidth *1.25f, -1.0f, origin.y + maxHeight * 1.25f);  //upper right

		int[] backgroundTriangles =new int[6];
		backgroundTriangles [0] = 0;   //lower left
		backgroundTriangles [1] = 2;   //upper left
		backgroundTriangles [2] = 1;   //lower right
		backgroundTriangles [3] = 2;   //upper left
		backgroundTriangles [4] = 3;   //upper right
		backgroundTriangles [5] = 1;   //lower right

		Vector2[] backgroundUV = new Vector2[4];
		backgroundUV [0] = new Vector2 (0.0f, 0.0f);	//lower left
		backgroundUV [1] = new Vector2 (1.0f, 0.0f);	//lower right
		backgroundUV [2] = new Vector2 (0.0f, 1.0f);	//upper left
		backgroundUV [3] = new Vector2 (1.0f, 1.0f);	//upper right

		//all background normals are set to up since this is a flat 2D game
		Vector3[] backgroundNormals = new Vector3[4];
		backgroundNormals[0] = Vector3.up;
		backgroundNormals[1] = Vector3.up;
		backgroundNormals[2] = Vector3.up;
		backgroundNormals[3] = Vector3.up;

		//now that vetices, triangles, normals, and uvs are created, we can instantiate a mesh
		//and assign it these properties
		Mesh backgroundMesh = new Mesh();
		backgroundMesh.vertices = backgroundVertices;
		backgroundMesh.triangles = backgroundTriangles;
		backgroundMesh.normals = backgroundNormals;
		backgroundMesh.uv = backgroundUV;


		//assign a mesh filter, mesh renderer, and mesh collider
		MeshFilter mesh_filter = GetComponent<MeshFilter> ();
		//MeshRenderer mesh_renderer = GetComponent<MeshRenderer> ();

		//assign our mesh to the mesh filter
		mesh_filter.mesh = backgroundMesh;

		return;

	}

	//the update function will handle the parallax movement
	private void Update(){

		if (backgroundMaterial != null) {
			
			//grab the current offset
			UVoffset = backgroundMaterial.mainTextureOffset;

			//set the new x and y values based on the main camera position
			//I am adjusting for the map center, so the offset is checking the camera relative to the center of the map
			//diving by the localScale should not matter since the scale is 1
			//dividing by the parallax means that the larger the parallax value, the less the background will move
			UVoffset.x = (Camera.main.transform.position.x - mapCenterOffset.x) / Camera.main.transform.localScale.x / parallax; 
			UVoffset.y = (Camera.main.transform.position.z - mapCenterOffset.z) / Camera.main.transform.localScale.z / parallax;

			//assign the uv offset back to the material
			backgroundMaterial.mainTextureOffset = UVoffset;

		}

	}

}
