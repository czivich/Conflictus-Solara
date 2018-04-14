using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using TMPro;

public class UISelectionNonGroup : MonoBehaviour,IPointerEnterHandler,IPointerClickHandler {
	
	public static SelectionEvent OnPointerEnterSelectable = new SelectionEvent();

	public static SelectionEvent OnPointerClickSelectable = new SelectionEvent();

	//simple class so I can have my event pass a gameObject
	public class SelectionEvent : UnityEvent<Selectable>{};

	private void Start(){

		//check if this object is a button
		if (this.GetComponent<Button> () == true) {

			//add listener for button onClick
			this.GetComponent<Button>().onClick.AddListener(ResolveButtonPress);

		}


	}

	public void OnPointerEnter(PointerEventData eventData){

		//invoke the event
		OnPointerEnterSelectable.Invoke(this.GetComponent<Selectable>());

	}

	public void OnPointerClick(PointerEventData eventData){

		//check if this is a dropdown item
		if (this.transform.parent.parent.parent.name == "Dropdown List") {
			
			//invoke the event
			OnPointerClickSelectable.Invoke (this.GetComponent<Selectable> ());

		}

	}

	private void ResolveButtonPress(){

		//invoke the event
		OnPointerClickSelectable.Invoke(this.GetComponent<Selectable>());

	}

	//this function handles onDestroy
	private void OnDestroy(){

		RemoveEventListeners ();

	}

	//this function removes listeners
	private void RemoveEventListeners(){

		if (this.GetComponent<Button> () == true) {

			//remove listener for button onClick
			this.GetComponent<Button> ().onClick.RemoveListener (ResolveButtonPress);
		
		}

	}

}
