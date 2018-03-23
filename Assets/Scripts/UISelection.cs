using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


[RequireComponent(typeof(Selectable))]
public class UISelection : MonoBehaviour, IPointerEnterHandler, IDeselectHandler, ISelectHandler, IPointerExitHandler
{

	private Color32 mouseOverHighlightColor = new Color32 (160, 255, 255, 255);
	private Color32 defaultColor = new Color32 (255, 255, 255, 255);

	//use this for initialization
	private void Start(){

		//set the selectedState image color to match the highlighted color
		this.GetComponent<Selectable>().gameObject.FindComponentInChildWithTag<Image> ("SelectedUI").color = mouseOverHighlightColor;

	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		//if (!EventSystem.current.alreadySelecting)
		//	EventSystem.current.SetSelectedGameObject(this.gameObject);

		//check if the Selectable is interactable
		if (this.GetComponent<Selectable> ().interactable == true) {
			
			//set the color to the mouseOverColor
			this.GetComponent<Image> ().color = mouseOverHighlightColor;

		}

	}

	public void OnPointerExit(PointerEventData eventData)
	{

		//set the color to the default
		this.GetComponent<Image>().color = defaultColor;

	}

	public void OnSelect(BaseEventData eventData)
	{
		//enable the selectUI image
		this.GetComponent<Selectable>().gameObject.FindComponentInChildWithTag<Image> ("SelectedUI").enabled = true;

	}

	public void OnDeselect(BaseEventData eventData)
	{
		//this.GetComponent<Selectable>().OnPointerExit(null);

		//disable the selectUI image
		this.gameObject.FindComponentInChildWithTag<Image> ("SelectedUI").enabled = false;

	}
}




