using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TileMapAnimationManager : MonoBehaviour {

	//variable to hold the explosion prefab
	public TileMapUnitExplosion prefabTileMapUnitExplosion;

	//variable to hold the animation parent in the hierarchy
	public GameObject tileMapAnimationParent;

	//variable to hold the tileMap
	private TileMap tileMap;

	//event for explosion
	public UnityEvent OnUnitExplosion = new UnityEvent();

	//unityActions
	private UnityAction<CombatUnit> combatUnitCreateExplosion;

	// Use this for initialization
	public void Init () {

		//get the tileMap
		tileMap = GameObject.FindGameObjectWithTag("TileMap").GetComponent<TileMap>();

		//set the actions
		combatUnitCreateExplosion = (combatUnit) => {CreateExplosion(combatUnit);};

		//add listener for ship getting destroyed
		Ship.OnShipDestroyed.AddListener(combatUnitCreateExplosion);

		//add listener for base getting destroyed
		Starbase.OnBaseDestroyed.AddListener(combatUnitCreateExplosion);
				
	}

	//this function creates an explosion where a combat unit is destroyed
	private void CreateExplosion(CombatUnit combatUnit){

		TileMapUnitExplosion.CreateTileMapUnitExplosion (prefabTileMapUnitExplosion, 
			tileMap.HexMap [combatUnit.currentLocation], 16, 16, 6.0f, new Vector3 (2.0f, 2.0f, 2.0f), tileMapAnimationParent.transform, true);

		//invoke the event
		OnUnitExplosion.Invoke();

	}

	//this function handles OnDestroy
	private void OnDestroy(){

		RemoveAllListeners ();

	}

	//this function removes all listeners
	private void RemoveAllListeners(){

		//remove listener for ship getting destroyed
		Ship.OnShipDestroyed.RemoveListener(combatUnitCreateExplosion);

		//remove listener for base getting destroyed
		Starbase.OnBaseDestroyed.RemoveListener(combatUnitCreateExplosion);

	}

}
