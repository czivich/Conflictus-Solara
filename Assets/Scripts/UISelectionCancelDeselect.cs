using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;


[RequireComponent(typeof(Selectable))]
public class UISelectionCancelDeselect : MonoBehaviour,ISubmitHandler{

	// Use this for initialization
	void Start () {
		
	}
	
	public void OnSubmit(BaseEventData eventData){
		//this.GetComponent<Selectable>().OnPointerExit(null);

		//I want to disable the selection indication when we submit
		//disable the selectUI image
		this.gameObject.FindComponentInChildWithTag<Image> ("SelectedUI").enabled = false;

	}

}
