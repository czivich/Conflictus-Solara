using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class MovementText : MonoBehaviour {

	//this is the TextMeshPro object which holds the movement text
	public TextMeshPro movementText;

	//unityActions
	private UnityAction<int> intUpdateMovementRangeAction;

	// Use this for initialization
	//made this Awake instead of start so it would be listening when the ship start function runs
	public void Init () {

		//set the actions
		intUpdateMovementRangeAction = (range) => {UpdateMovementValue(range);};

		//add a listener to the OnMovementRangeChange event
		this.gameObject.GetComponent<EngineSection>().OnMovementRangeChange.AddListener(intUpdateMovementRangeAction);

	}

	//this function will update the text display of remaining movement points
	private void UpdateMovementValue(int range){

		//update the movement value
		movementText.text = range.ToString();

	}

	//this function handles OnDestroy
	private void OnDestroy(){

		RemoveAllListeners ();

	}

	//this function removes all listeners
	private void RemoveAllListeners(){

		//remove a listener to the OnMovementRangeChange event
		this.gameObject.GetComponent<EngineSection>().OnMovementRangeChange.RemoveListener(intUpdateMovementRangeAction);

	}

}
