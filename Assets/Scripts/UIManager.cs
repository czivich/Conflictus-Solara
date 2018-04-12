using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System;

public class UIManager : MonoBehaviour {

	//variable for the canvas
	public Canvas canvas;

	//this array holds all the inactive UI elements - hook them up in the inspector
	public GameObject[] inactiveUIObjects;

	//variable for the default main camera screen width
	public readonly float defaultMainCameraWidth = .75f;

	//this list is all of the action modes that we want to lock out the main action buttons/toggles in
	public static List<GameManager.ActionMode> lockMenuActionModes = new List<GameManager.ActionMode>() {

		GameManager.ActionMode.FlareMode,
		GameManager.ActionMode.Rename,
		GameManager.ActionMode.EndTurn,
		GameManager.ActionMode.Animation,
		GameManager.ActionMode.PlaceNewUnit,

	};

	//use this for initialization
	public void Init(){
		
		//disable the pauseFadePanels
		foreach(GameObject go in GameObject.FindGameObjectsWithTag("PauseFadePanel")){
			
			go.SetActive(false);

		}

		//autosize the text
		//AutoSizeAllText ();

	}
		
	//this function sets the text sizes for the current resolution
	private void AutoSizeAllText(){

		//iterate through each inactive UI element, activating them
		for (int i = 0; i < inactiveUIObjects.Length; i++) {

			//activate the object
			inactiveUIObjects[i].SetActive(true);

			//rebuild the layout immediately so that the sizing updates
			LayoutRebuilder.ForceRebuildLayoutImmediate(inactiveUIObjects[i].GetComponent<RectTransform>());

		}

		//force the canvas to update so it resizes itself before the end of the frame
		Canvas.ForceUpdateCanvases ();

		//get all the textMeshPro UI texts in the canvas
		foreach (TextMeshProUGUI textElement in canvas.gameObject.GetComponentsInChildren<TextMeshProUGUI>(true)){

			//enable auto-size
			textElement.enableAutoSizing = true;

			//force the mesh to update
			textElement.ForceMeshUpdate(false);

			//cache the font size
			float textSize = textElement.fontSize;

			//disable auto-size
			textElement.enableAutoSizing = false;

			//set the text size to the auto-size value
			textElement.fontSize = textSize;

		}

		//iterate through the inactive UI elements and set them inactive again
		for (int i = 0; i < inactiveUIObjects.Length; i++) {

			//activate the object
			inactiveUIObjects[i].SetActive(false);

		}

	}

	//this static function will autoSize a single textmeshpro element
	public static void AutoSizeTextMeshFont(TextMeshProUGUI textElement){

		//enable auto-size
		textElement.enableAutoSizing = true;

		//force the mesh to update
		//textElement.ForceMeshUpdate(false);

		//cache the font size
		float textSize = textElement.fontSize;

		//disable auto-size
		textElement.enableAutoSizing = false;

		//set the text size to the auto-size value
		textElement.fontSize = textSize;

	}
						
}
