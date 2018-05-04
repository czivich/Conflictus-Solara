using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ParticleManager : MonoBehaviour {

	//variable to hold the parent gameObject
	public GameObject particleEffectsCollector;

	//variables to hold the particle effect object prefabs
	public GameObject prefabSunParticleEffect;
	public GameObject prefabNeutralStarbaseLightsEffect;

	// Use this for initialization
	public void Init () {
		
	}

	//this function adds event listeners
	private void AddEventListeners(){


	}

	//this function handles OnDestroy
	private void OnDestroy(){

		RemoveEventListeners ();

	}

	//this function removes event listeners
	private void RemoveEventListeners(){



	}

}
