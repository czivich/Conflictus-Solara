using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using TMPro;

public class UISelectionNonGroup : MonoBehaviour,IPointerEnterHandler {
	
	public static SelectionEvent OnPointerEnterSelectable = new SelectionEvent();

	//simple class so I can have my event pass a gameObject
	public class SelectionEvent : UnityEvent<Selectable>{};


	public void OnPointerEnter(PointerEventData eventData){

		//invoke the event
		OnPointerEnterSelectable.Invoke(this.GetComponent<Selectable>());

	}

}
