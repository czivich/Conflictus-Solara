using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UIMainPanel : MonoBehaviour {

	//variable for the side panel
	public GameObject[] uiMainPanel;

	// Use this for initialization
	public void Init () {

		//set the UISidePanel size
		SetUIMainPanelSize();

		//add listeners
		AddEventListeners();

	}

	//this function sets the UISidePanel size
	private void SetUIMainPanelSize(){

		//loop through the panels
		for (int i = 0; i < uiMainPanel.Length; i++) {

			//set the anchor points based on the main camera viewport rect
			uiMainPanel[i].GetComponent<RectTransform> ().anchorMax = new Vector2 (Camera.main.rect.width, 1.0f);

			//rebuild the layout immediately so that the sizing updates
			LayoutRebuilder.ForceRebuildLayoutImmediate (uiMainPanel[i].GetComponent<RectTransform> ());

			//force the canvas to update so it resizes itself before the end of the frame
			Canvas.ForceUpdateCanvases ();

		}

		//Debug.Log ("setup ui main panel");

	}

	//this function adds event listeners
	private void AddEventListeners(){

		//add listener for camera setup change
		GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameManager> ().OnChangedMainCameraSetup.AddListener (SetUIMainPanelSize);

	}

	//this function handles on destroy
	private void OnDestroy(){

		RemoveEventListeners ();

	}

	//this function removes event listeners
	private void RemoveEventListeners(){

		if (GameObject.FindGameObjectWithTag ("GameManager") != null) {

			//remove listener for camera setup change
			GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameManager> ().OnChangedMainCameraSetup.RemoveListener (SetUIMainPanelSize);

		}

	}

}
