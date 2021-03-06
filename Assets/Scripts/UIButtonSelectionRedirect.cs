﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;

[RequireComponent(typeof(Button))]
public class UIButtonSelectionRedirect : MonoBehaviour, IPointerClickHandler,ISubmitHandler {

	//this is the selectable that we want to redirect selection to when we click this button
	public Selectable redirectSelectable;

	public static RedirectEvent OnClickedButtonForRedirect = new RedirectEvent();

	//simple class so I can have my event pass a gameObject
	public class RedirectEvent : UnityEvent<Selectable>{};

	//this function is called when the pointer clicks on the object
	public void OnPointerClick(PointerEventData pointerEventData){

		//check if the object is interactable and active
		if (this.GetComponent<Selectable> ().IsInteractable () == true && this.GetComponent<Selectable> ().IsActive () == true) {

			//invoke the redirect event
			OnClickedButtonForRedirect.Invoke (redirectSelectable);

		}

	}

	public void OnSubmit(BaseEventData eventData){

		//check if the object is interactable and active
		if (this.GetComponent<Selectable> ().IsInteractable () == true && this.GetComponent<Selectable> ().IsActive () == true) {
			
			//invoke the redirect event
			OnClickedButtonForRedirect.Invoke (redirectSelectable);

		}

	}

}
