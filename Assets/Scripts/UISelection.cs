using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using TMPro;

[RequireComponent(typeof(Selectable))]
public class UISelection : MonoBehaviour, IPointerEnterHandler, IDeselectHandler, ISelectHandler, IPointerExitHandler, IPointerClickHandler,ISubmitHandler{
	
	//make it static so that subscribers don't have to subscribe to every selectable
	public static SelectionEvent OnSetSelectedGameObject = new SelectionEvent();

	public static SelectionEvent OnClickedSelectable = new SelectionEvent();

	public static SelectionEvent OnPointerEnterSelectable = new SelectionEvent();

	public static SelectionEvent OnSelectedSelectable = new SelectionEvent();

	public static SelectionEvent OnSubmitSelectable = new SelectionEvent();


	//simple class so I can have my event pass a gameObject
	public class SelectionEvent : UnityEvent<Selectable>{};

	private Color32 mouseOverHighlightColor = new Color32 (160, 255, 255, 255);
	//private Color32 defaultColor = new Color32 (255, 255, 255, 255);

	//variables to hold the scene indexes
	private int mainMenuSceneIndex = 0;
	//private int mainSceneIndex = 1;

	//use this for initialization
	private void Start(){

		//set the selectedState image color to match the highlighted color
		this.GetComponent<Selectable>().gameObject.FindComponentInChildWithTag<Image> ("SelectedUI").color = mouseOverHighlightColor;

	}

	public void OnPointerEnter(PointerEventData eventData){
			
		//invoke the event
		OnPointerEnterSelectable.Invoke(this.GetComponent<Selectable>());

	}

	public void OnPointerExit(PointerEventData eventData){

		//set the color to the default
		//this.GetComponent<Image>().color = defaultColor;

	}

	public void OnSelect(BaseEventData eventData){
		//enable the selectUI image
		this.GetComponent<Selectable>().gameObject.FindComponentInChildWithTag<Image> ("SelectedUI").enabled = true;

		//Debug.Log ("Selected " + this.gameObject.name);

		//invoke the event
		OnSelectedSelectable.Invoke(this.GetComponent<Selectable>());

	}

	public void OnDeselect(BaseEventData eventData){
		//this.GetComponent<Selectable>().OnPointerExit(null);

		//disable the selectUI image
		this.gameObject.FindComponentInChildWithTag<Image> ("SelectedUI").enabled = false;
		//Debug.Log ("Deselect " + this.gameObject.name);

	}

	public void OnSubmit(BaseEventData eventData){
		//this.GetComponent<Selectable>().OnPointerExit(null);

		//I want to disable the selection indication when we submit
		//disable the selectUI image
		//this.gameObject.FindComponentInChildWithTag<Image> ("SelectedUI").enabled = false;

		//Debug.Log ("OnSubmit " + this.gameObject.name);

		//invoke the event
		OnSubmitSelectable.Invoke(this.GetComponent<Selectable>());

	}


	public void OnPointerClick(PointerEventData pointerEventData){

		//Debug.Log ("OnPointerClick");

		//check if the clicked object is interactable
		if (this.GetComponent<Selectable> ().IsInteractable () == true) {

			//announce we clicked on a selectable
			OnClickedSelectable.Invoke (this.GetComponent<Selectable> ());

			//check the current scene
			if (SceneManager.GetActiveScene ().buildIndex == mainMenuSceneIndex) {

				if (UINavigationMainMenu.blockPointerClickFlag == false) {

					//select the object
					EventSystem.current.SetSelectedGameObject (this.gameObject);

					//Debug.Log ("SetSelectedObject onPointerClick " + this.gameObject.name);

					//invoke the select object event
					OnSetSelectedGameObject.Invoke (this.GetComponent<Selectable> ());

				} else {

					//Debug.Log ("Blocked!");

				}

			} else {

				if (UINavigationMain.blockPointerClickFlag == false) {

					//select the object
					EventSystem.current.SetSelectedGameObject (this.gameObject);

					//Debug.Log ("SetSelectedObject onPointerClick " + this.gameObject.name);

					//invoke the select object event
					OnSetSelectedGameObject.Invoke (this.GetComponent<Selectable> ());

				} else {

					//Debug.Log ("Blocked!");

				}

			}

			//invoke the select object event
			//OnSetSelectedGameObject.Invoke (this.GetComponent<Selectable> ());

		}

	}

	public void OnEnable(){
		
		//I want to have these all disabled until specifically turned on - this prevents
		//them from accidentally still being on after exiting a menu and then coming back to the menu
		//disable the selectUI image
		this.gameObject.FindComponentInChildWithTag<Image> ("SelectedUI").enabled = false;

	}

}




