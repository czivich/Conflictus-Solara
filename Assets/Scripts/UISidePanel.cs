using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UISidePanel : MonoBehaviour {
	
	//variable for the side panel
	public GameObject uiSidePanel;

	// Use this for initialization
	public void Init () {

		//set the UISidePanel size
		SetUISidePanelSize();

		//add event listeners
		AddEventListeners();
				
	}

	//this function sets the UISidePanel size
	private void SetUISidePanelSize(){
		
		//set the anchor points based on the main camera viewport rect
		uiSidePanel.GetComponent<RectTransform>().anchorMin = new Vector2(Camera.main.rect.width, 0.0f);

		//rebuild the layout immediately so that the sizing updates
		LayoutRebuilder.ForceRebuildLayoutImmediate(uiSidePanel.GetComponent<RectTransform>());

		//force the canvas to update so it resizes itself before the end of the frame
		Canvas.ForceUpdateCanvases ();

		//Debug.Log ("setup ui side panel");

	}

	//this function adds event listeners
	private void AddEventListeners(){

		//add listener for camera setup change
		GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameManager> ().OnChangedMainCameraSetup.AddListener (SetUISidePanelSize);

	}

	//this function handles on destroy
	private void OnDestroy(){

		RemoveEventListeners ();

	}

	//this function removes event listeners
	private void RemoveEventListeners(){

		if (GameObject.FindGameObjectWithTag ("GameManager") != null) {

			//remove listener for camera setup change
			GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameManager> ().OnChangedMainCameraSetup.RemoveListener (SetUISidePanelSize);

		}

	}

}
