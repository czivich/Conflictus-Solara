using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ParticleManager : MonoBehaviour {

	//variable for tileMap
	TileMap tileMap;

	//variable to hold the parent gameObject
	public GameObject particleEffectsCollector;

	//variables to hold the particle effect object prefabs
	public GameObject prefabSunParticleEffect;
	public GameObject prefabNeutralStarbaseLightsEffect;

	//create a vector3 offset so that combat units can exist just above the tilemap
	public static Vector3 mapOffset = new Vector3(0.0f, 0.1f, 0.0f);

	//unityActions
	private UnityAction<HexMapTile> createHexMapTileAction;

	// Use this for initialization
	public void Init () {

		//get the tileMap
		tileMap = GameObject.FindGameObjectWithTag("TileMap").GetComponent<TileMap>();

		//set unityActions
		SetUnityActions();

		//add event listeners
		AddEventListeners();

		//create particleEffects
		CreateParticleEffects();

	}

	//this function sets unityActions
	private void SetUnityActions(){


	}

	//this function creates the particle effects
	private void CreateParticleEffects(){

		//loop through all sun tiles
		for (int i = 0; i < tileMap.SunTiles.Count; i++) {

			//create the effect
			CreateParticleEffect (prefabSunParticleEffect, particleEffectsCollector, tileMap.HexMap [tileMap.SunTiles [i]]);

		}

		//loop through the neutral starbase tiles
		for (int i = 0; i < tileMap.NeutralStarbaseTiles.Count; i++) {

			//create the effect
			CreateParticleEffect (prefabNeutralStarbaseLightsEffect, particleEffectsCollector, tileMap.HexMap [tileMap.NeutralStarbaseTiles [i]]);

		}

	}

	//this function adds event listeners
	private void AddEventListeners(){

	}

	//this function handles OnDestroy
	private void OnDestroy(){

		RemoveEventListeners ();

	}

	//this function creates a particle effect in the world
	private void CreateParticleEffect(GameObject prefabParticleEffect, GameObject parentObject, HexMapTile hexMapTile){

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

		//instantiate a prefab
		GameObject newParticleObject = Instantiate(prefabParticleEffect,unitWorldCoordinatesV3,Quaternion.identity);

		//set the parent
		newParticleObject.transform.SetParent(parentObject.transform);

	}

	//this function removes event listeners
	private void RemoveEventListeners(){

	}

}
