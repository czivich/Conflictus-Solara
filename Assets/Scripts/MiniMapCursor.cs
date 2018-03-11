using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class MiniMapCursor : MonoBehaviour {

	private MeshFilter miniMapMeshFilter;
	private Mesh miniMapCursorMesh;

	private static Vector3 mapOffset = new Vector3(0.0f, 0.025f, 0.0f);

	private float lineWidth = 0.3f;

	//variable to hold the viewport aspect ratio
	private float viewportAspectratio;

	public Vector3[] miniMapCursorVertices {

		get;
		private set;

	}

	//this event is for when the CurrentMovementRange property changes
	public static MiniMapCursorEvent OnCreateMiniMapCursor = new MiniMapCursorEvent();

	//simple class derived from unityEvent to pass a miniMap cursor
	public class MiniMapCursorEvent : UnityEvent<MiniMapCursor>{};

	// Use this for initialization
	public void Init () {

		//the vertices will be based on the main camera, transform, orthographic size, and aspect ratio
		//vertices start at lower left corner, go across row then up in column

		int numberTiles = 9;
		int numberVertices = numberTiles * 4;

		//viewportAspectratio = Camera.main.aspect * (Camera.main.rect.width / Camera.main.rect.height);
		viewportAspectratio = Camera.main.aspect;

		//these vertices are based on a 9-slice area

		miniMapCursorVertices = new Vector3 [numberVertices];
		miniMapCursorVertices[0] = new Vector3 ( -(Camera.main.orthographicSize * viewportAspectratio), MiniMapCursor.mapOffset.y, - Camera.main.orthographicSize);  //lower left
		miniMapCursorVertices[1] = new Vector3 ( -(Camera.main.orthographicSize * viewportAspectratio) + lineWidth, MiniMapCursor.mapOffset.y, - Camera.main.orthographicSize);  
		miniMapCursorVertices[2] = new Vector3 ( -(Camera.main.orthographicSize * viewportAspectratio), MiniMapCursor.mapOffset.y, - Camera.main.orthographicSize + lineWidth);  
		miniMapCursorVertices[3] = new Vector3 ( -(Camera.main.orthographicSize * viewportAspectratio) + lineWidth, MiniMapCursor.mapOffset.y, - Camera.main.orthographicSize + lineWidth);

		miniMapCursorVertices[4] = new Vector3 ( -(Camera.main.orthographicSize * viewportAspectratio) + lineWidth, MiniMapCursor.mapOffset.y, - Camera.main.orthographicSize);  
		miniMapCursorVertices[5] = new Vector3 ( (Camera.main.orthographicSize * viewportAspectratio) - lineWidth, MiniMapCursor.mapOffset.y, - Camera.main.orthographicSize);
		miniMapCursorVertices[6] = new Vector3 ( -(Camera.main.orthographicSize * viewportAspectratio) + lineWidth, MiniMapCursor.mapOffset.y, - Camera.main.orthographicSize + lineWidth);  
		miniMapCursorVertices[7] = new Vector3 ( (Camera.main.orthographicSize * viewportAspectratio) - lineWidth, MiniMapCursor.mapOffset.y, - Camera.main.orthographicSize + lineWidth);

		miniMapCursorVertices[8] = new Vector3 ( (Camera.main.orthographicSize * viewportAspectratio) - lineWidth, MiniMapCursor.mapOffset.y, - Camera.main.orthographicSize);
		miniMapCursorVertices[9] = new Vector3 ( (Camera.main.orthographicSize * viewportAspectratio), MiniMapCursor.mapOffset.y, - Camera.main.orthographicSize);  //lower right
		miniMapCursorVertices[10] = new Vector3 ( (Camera.main.orthographicSize * viewportAspectratio) - lineWidth, MiniMapCursor.mapOffset.y, - Camera.main.orthographicSize + lineWidth);
		miniMapCursorVertices[11] = new Vector3 ( (Camera.main.orthographicSize * viewportAspectratio), MiniMapCursor.mapOffset.y, - Camera.main.orthographicSize + lineWidth);

		miniMapCursorVertices[12] = new Vector3 ( -(Camera.main.orthographicSize * viewportAspectratio), MiniMapCursor.mapOffset.y, - Camera.main.orthographicSize + lineWidth);
		miniMapCursorVertices[13] = new Vector3 ( -(Camera.main.orthographicSize * viewportAspectratio) + lineWidth, MiniMapCursor.mapOffset.y, - Camera.main.orthographicSize + lineWidth);  
		miniMapCursorVertices[14] = new Vector3 ( -(Camera.main.orthographicSize * viewportAspectratio), MiniMapCursor.mapOffset.y, Camera.main.orthographicSize - lineWidth);
		miniMapCursorVertices[15] = new Vector3 ( -(Camera.main.orthographicSize * viewportAspectratio) + lineWidth, MiniMapCursor.mapOffset.y, Camera.main.orthographicSize - lineWidth);

		miniMapCursorVertices[16] = new Vector3 ( -(Camera.main.orthographicSize * viewportAspectratio) + lineWidth, MiniMapCursor.mapOffset.y, - Camera.main.orthographicSize + lineWidth);  
		miniMapCursorVertices[17] = new Vector3 ( (Camera.main.orthographicSize * viewportAspectratio) - lineWidth, MiniMapCursor.mapOffset.y, - Camera.main.orthographicSize + lineWidth);
		miniMapCursorVertices[18] = new Vector3 ( -(Camera.main.orthographicSize * viewportAspectratio) + lineWidth, MiniMapCursor.mapOffset.y, Camera.main.orthographicSize - lineWidth);
		miniMapCursorVertices[19] = new Vector3 ( (Camera.main.orthographicSize * viewportAspectratio) - lineWidth, MiniMapCursor.mapOffset.y, Camera.main.orthographicSize - lineWidth);

		miniMapCursorVertices[20] = new Vector3 ( (Camera.main.orthographicSize * viewportAspectratio) - lineWidth, MiniMapCursor.mapOffset.y, - Camera.main.orthographicSize + lineWidth);
		miniMapCursorVertices[21] = new Vector3 ( (Camera.main.orthographicSize * viewportAspectratio), MiniMapCursor.mapOffset.y, - Camera.main.orthographicSize + lineWidth);
		miniMapCursorVertices[22] = new Vector3 ( (Camera.main.orthographicSize * viewportAspectratio) - lineWidth, MiniMapCursor.mapOffset.y, Camera.main.orthographicSize - lineWidth);
		miniMapCursorVertices[23] = new Vector3 ( (Camera.main.orthographicSize * viewportAspectratio), MiniMapCursor.mapOffset.y, Camera.main.orthographicSize - lineWidth);

		miniMapCursorVertices[24] = new Vector3 ( -(Camera.main.orthographicSize * viewportAspectratio), MiniMapCursor.mapOffset.y, Camera.main.orthographicSize - lineWidth);
		miniMapCursorVertices[25] = new Vector3 ( -(Camera.main.orthographicSize * viewportAspectratio) + lineWidth, MiniMapCursor.mapOffset.y, Camera.main.orthographicSize - lineWidth);
		miniMapCursorVertices[26] = new Vector3 ( -(Camera.main.orthographicSize * viewportAspectratio), MiniMapCursor.mapOffset.y, Camera.main.orthographicSize);  //upper left
		miniMapCursorVertices[27] = new Vector3 ( -(Camera.main.orthographicSize * viewportAspectratio) + lineWidth, MiniMapCursor.mapOffset.y, Camera.main.orthographicSize);

		miniMapCursorVertices[28] = new Vector3 ( -(Camera.main.orthographicSize * viewportAspectratio) + lineWidth, MiniMapCursor.mapOffset.y, Camera.main.orthographicSize - lineWidth);
		miniMapCursorVertices[29] = new Vector3 ( (Camera.main.orthographicSize * viewportAspectratio) - lineWidth, MiniMapCursor.mapOffset.y, Camera.main.orthographicSize - lineWidth);
		miniMapCursorVertices[30] = new Vector3 ( -(Camera.main.orthographicSize * viewportAspectratio) + lineWidth, MiniMapCursor.mapOffset.y, Camera.main.orthographicSize);
		miniMapCursorVertices[31] = new Vector3 ( (Camera.main.orthographicSize * viewportAspectratio) - lineWidth, MiniMapCursor.mapOffset.y, Camera.main.orthographicSize);

		miniMapCursorVertices[32] = new Vector3 ( (Camera.main.orthographicSize * viewportAspectratio) - lineWidth, MiniMapCursor.mapOffset.y, Camera.main.orthographicSize - lineWidth);
		miniMapCursorVertices[33] = new Vector3 ( (Camera.main.orthographicSize * viewportAspectratio), MiniMapCursor.mapOffset.y, Camera.main.orthographicSize - lineWidth);
		miniMapCursorVertices[34] = new Vector3 ( (Camera.main.orthographicSize * viewportAspectratio) - lineWidth, MiniMapCursor.mapOffset.y, Camera.main.orthographicSize);
		miniMapCursorVertices[35] = new Vector3 ( (Camera.main.orthographicSize * viewportAspectratio), MiniMapCursor.mapOffset.y, Camera.main.orthographicSize);  //upper right

		int[] miniMapCursorTriangles =new int[numberTiles * 6];

		for (int i = 0; i < numberTiles; i++) {
			int triangleOffset = 6 * i;
			int vertexOffset = 4 * i;
			miniMapCursorTriangles [triangleOffset + 0] = vertexOffset;
			miniMapCursorTriangles [triangleOffset + 1] = vertexOffset + 2;
			miniMapCursorTriangles [triangleOffset + 2] = vertexOffset + 1;

			miniMapCursorTriangles [triangleOffset + 3] = vertexOffset + 1;
			miniMapCursorTriangles [triangleOffset + 4] = vertexOffset + 2;
			miniMapCursorTriangles [triangleOffset + 5] = vertexOffset + 3;

		}
					
		//all background normals are set to up since this is a flat 2D game
		Vector3[] miniMapCursorNormals = new Vector3[numberVertices];

		for(int i = 0; i < numberVertices; i++){

			miniMapCursorNormals[i] = Vector3.up;

		}

		//now that vetices, triangles, normals, and uvs are created, we can instantiate a mesh
		//and assign it these properties
		miniMapCursorMesh = new Mesh();
		miniMapCursorMesh.vertices = miniMapCursorVertices;
		miniMapCursorMesh.triangles = miniMapCursorTriangles;
		miniMapCursorMesh.normals = miniMapCursorNormals;

		//assign a mesh filter
		miniMapMeshFilter = GetComponent<MeshFilter> ();

		//assign our mesh to the mesh filter
		miniMapMeshFilter.mesh = miniMapCursorMesh;

		//invoke the event
		OnCreateMiniMapCursor.Invoke(this);

		//after invoking the event, re-cache the mesh, since the graphics manager will have assigned UVs
		miniMapCursorMesh = miniMapMeshFilter.mesh;
		return;

	}
	
	// Update is called once per frame
	private void Update () {

		//during update, we need to resize the mesh based on the main camera
		miniMapCursorVertices[0] = new Vector3 ( -(Camera.main.orthographicSize * viewportAspectratio), MiniMapCursor.mapOffset.y, - Camera.main.orthographicSize);  //lower left
		miniMapCursorVertices[1] = new Vector3 ( -(Camera.main.orthographicSize * viewportAspectratio) + lineWidth, MiniMapCursor.mapOffset.y, - Camera.main.orthographicSize);  
		miniMapCursorVertices[2] = new Vector3 ( -(Camera.main.orthographicSize * viewportAspectratio), MiniMapCursor.mapOffset.y, - Camera.main.orthographicSize + lineWidth);  
		miniMapCursorVertices[3] = new Vector3 ( -(Camera.main.orthographicSize * viewportAspectratio) + lineWidth, MiniMapCursor.mapOffset.y, - Camera.main.orthographicSize + lineWidth);

		miniMapCursorVertices[4] = new Vector3 ( -(Camera.main.orthographicSize * viewportAspectratio) + lineWidth, MiniMapCursor.mapOffset.y, - Camera.main.orthographicSize);  
		miniMapCursorVertices[5] = new Vector3 ( (Camera.main.orthographicSize * viewportAspectratio) - lineWidth, MiniMapCursor.mapOffset.y, - Camera.main.orthographicSize);
		miniMapCursorVertices[6] = new Vector3 ( -(Camera.main.orthographicSize * viewportAspectratio) + lineWidth, MiniMapCursor.mapOffset.y, - Camera.main.orthographicSize + lineWidth);  
		miniMapCursorVertices[7] = new Vector3 ( (Camera.main.orthographicSize * viewportAspectratio) - lineWidth, MiniMapCursor.mapOffset.y, - Camera.main.orthographicSize + lineWidth);

		miniMapCursorVertices[8] = new Vector3 ( (Camera.main.orthographicSize * viewportAspectratio) - lineWidth, MiniMapCursor.mapOffset.y, - Camera.main.orthographicSize);
		miniMapCursorVertices[9] = new Vector3 ( (Camera.main.orthographicSize * viewportAspectratio), MiniMapCursor.mapOffset.y, - Camera.main.orthographicSize);  //lower right
		miniMapCursorVertices[10] = new Vector3 ( (Camera.main.orthographicSize * viewportAspectratio) - lineWidth, MiniMapCursor.mapOffset.y, - Camera.main.orthographicSize + lineWidth);
		miniMapCursorVertices[11] = new Vector3 ( (Camera.main.orthographicSize * viewportAspectratio), MiniMapCursor.mapOffset.y, - Camera.main.orthographicSize + lineWidth);

		miniMapCursorVertices[12] = new Vector3 ( -(Camera.main.orthographicSize * viewportAspectratio), MiniMapCursor.mapOffset.y, - Camera.main.orthographicSize + lineWidth);
		miniMapCursorVertices[13] = new Vector3 ( -(Camera.main.orthographicSize * viewportAspectratio) + lineWidth, MiniMapCursor.mapOffset.y, - Camera.main.orthographicSize + lineWidth);  
		miniMapCursorVertices[14] = new Vector3 ( -(Camera.main.orthographicSize * viewportAspectratio), MiniMapCursor.mapOffset.y, Camera.main.orthographicSize - lineWidth);
		miniMapCursorVertices[15] = new Vector3 ( -(Camera.main.orthographicSize * viewportAspectratio) + lineWidth, MiniMapCursor.mapOffset.y, Camera.main.orthographicSize - lineWidth);

		miniMapCursorVertices[16] = new Vector3 ( -(Camera.main.orthographicSize * viewportAspectratio) + lineWidth, MiniMapCursor.mapOffset.y, - Camera.main.orthographicSize + lineWidth);  
		miniMapCursorVertices[17] = new Vector3 ( (Camera.main.orthographicSize * viewportAspectratio) - lineWidth, MiniMapCursor.mapOffset.y, - Camera.main.orthographicSize + lineWidth);
		miniMapCursorVertices[18] = new Vector3 ( -(Camera.main.orthographicSize * viewportAspectratio) + lineWidth, MiniMapCursor.mapOffset.y, Camera.main.orthographicSize - lineWidth);
		miniMapCursorVertices[19] = new Vector3 ( (Camera.main.orthographicSize * viewportAspectratio) - lineWidth, MiniMapCursor.mapOffset.y, Camera.main.orthographicSize - lineWidth);

		miniMapCursorVertices[20] = new Vector3 ( (Camera.main.orthographicSize * viewportAspectratio) - lineWidth, MiniMapCursor.mapOffset.y, - Camera.main.orthographicSize + lineWidth);
		miniMapCursorVertices[21] = new Vector3 ( (Camera.main.orthographicSize * viewportAspectratio), MiniMapCursor.mapOffset.y, - Camera.main.orthographicSize + lineWidth);
		miniMapCursorVertices[22] = new Vector3 ( (Camera.main.orthographicSize * viewportAspectratio) - lineWidth, MiniMapCursor.mapOffset.y, Camera.main.orthographicSize - lineWidth);
		miniMapCursorVertices[23] = new Vector3 ( (Camera.main.orthographicSize * viewportAspectratio), MiniMapCursor.mapOffset.y, Camera.main.orthographicSize - lineWidth);

		miniMapCursorVertices[24] = new Vector3 ( -(Camera.main.orthographicSize * viewportAspectratio), MiniMapCursor.mapOffset.y, Camera.main.orthographicSize - lineWidth);
		miniMapCursorVertices[25] = new Vector3 ( -(Camera.main.orthographicSize * viewportAspectratio) + lineWidth, MiniMapCursor.mapOffset.y, Camera.main.orthographicSize - lineWidth);
		miniMapCursorVertices[26] = new Vector3 ( -(Camera.main.orthographicSize * viewportAspectratio), MiniMapCursor.mapOffset.y, Camera.main.orthographicSize);  //upper left
		miniMapCursorVertices[27] = new Vector3 ( -(Camera.main.orthographicSize * viewportAspectratio) + lineWidth, MiniMapCursor.mapOffset.y, Camera.main.orthographicSize);

		miniMapCursorVertices[28] = new Vector3 ( -(Camera.main.orthographicSize * viewportAspectratio) + lineWidth, MiniMapCursor.mapOffset.y, Camera.main.orthographicSize - lineWidth);
		miniMapCursorVertices[29] = new Vector3 ( (Camera.main.orthographicSize * viewportAspectratio) - lineWidth, MiniMapCursor.mapOffset.y, Camera.main.orthographicSize - lineWidth);
		miniMapCursorVertices[30] = new Vector3 ( -(Camera.main.orthographicSize * viewportAspectratio) + lineWidth, MiniMapCursor.mapOffset.y, Camera.main.orthographicSize);
		miniMapCursorVertices[31] = new Vector3 ( (Camera.main.orthographicSize * viewportAspectratio) - lineWidth, MiniMapCursor.mapOffset.y, Camera.main.orthographicSize);

		miniMapCursorVertices[32] = new Vector3 ( (Camera.main.orthographicSize * viewportAspectratio) - lineWidth, MiniMapCursor.mapOffset.y, Camera.main.orthographicSize - lineWidth);
		miniMapCursorVertices[33] = new Vector3 ( (Camera.main.orthographicSize * viewportAspectratio), MiniMapCursor.mapOffset.y, Camera.main.orthographicSize - lineWidth);
		miniMapCursorVertices[34] = new Vector3 ( (Camera.main.orthographicSize * viewportAspectratio) - lineWidth, MiniMapCursor.mapOffset.y, Camera.main.orthographicSize);
		miniMapCursorVertices[35] = new Vector3 ( (Camera.main.orthographicSize * viewportAspectratio), MiniMapCursor.mapOffset.y, Camera.main.orthographicSize);  //upper right

		//asign the vertices
		miniMapCursorMesh.vertices = miniMapCursorVertices;

		//assign our mesh to the mesh filter
		miniMapMeshFilter.mesh = miniMapCursorMesh;

		//we also need to update the transform to match the main camera
		this.transform.position = new Vector3(Camera.main.transform.position.x, MiniMapCursor.mapOffset.y, Camera.main.transform.position.z);

	}

}
