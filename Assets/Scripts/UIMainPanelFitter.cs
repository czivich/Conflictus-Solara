using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIMainPanelFitter : MonoBehaviour {

	//uiManager
	private GameObject uiManager;

	//array to hold the UI panels that will need fitted to the main screen
	public GameObject[] UIPanels;

	//variable to hold the canvas
	public GameObject canvas;

	//variable to hold the main camera width as a percentage of the screen width
	private float mainCameraWidth;

	//variable to hold the canvas width in pixels
	private float canvasWidthInPixels;

	//variable to hold the mainCameraWidth in pixels
	private float mainCameraWidthInPixels;

	//flag to check whether canvas needs adjusted
	private bool needsToAdjustSize = false;


	//I had to include a frame delay from when I change the resolution until i resize the UI.
	//this seems dumb but necessary - I don't understand why
	private int frameCounter = 0;
	private int frameDelay = 2;

	//scene index
	private int mainMenuSceneBuildIndex = 0;
	private int mainSceneBuildIndex = 1;

	// Use this for initialization
	public void Init () {

		//get the manager
		uiManager = GameObject.FindGameObjectWithTag("UIManager");

		//add listeners
		AddEventListeners();

		//get the screen width
		canvasWidthInPixels = canvas.GetComponent<RectTransform>().rect.width;

		//Debug.Log ("canvasWidthInPixels = " + canvasWidthInPixels);

		//set the UI panel scales to default
		SetPanelScalesToDefault();

		//get the main camera area size
		GetMainCameraAreaSize ();

		//adjust the UI panel positions
		AdjustUIPanelPositions();
		
	}

	private void Update(){

		if (frameCounter > 0) {

			frameCounter--;

		}

		//check if we need to adjust size
		else if (needsToAdjustSize == true) {
			
			//get the screen width
			canvasWidthInPixels = canvas.GetComponent<RectTransform>().rect.width;

			//set the UI panel scales to default
			SetPanelScalesToDefault ();

			//get the main camera area size
			GetMainCameraAreaSize ();

			//adjust the UI panel positions
			AdjustUIPanelPositions ();

			//turn off the flag
			needsToAdjustSize = false;

		}

	}

	//add event listeners
	private void AddEventListeners(){

		//check if we are in the main menu scene
		if (SceneManager.GetActiveScene ().buildIndex == mainMenuSceneBuildIndex) {
			
			if (uiManager.GetComponent<Settings> () != null) {
			
				//add listener for changing resolution
				uiManager.GetComponent<Settings> ().OnChangeResolution.AddListener (SetNeedToAdjustFlagToTrue);

			}

		} else if (SceneManager.GetActiveScene ().buildIndex == mainSceneBuildIndex) {

			//else check if we are in the main scene

			if (GameObject.FindGameObjectWithTag ("GameManager") != null) {

				GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameManager> ().OnChangedMainCameraSetup.AddListener (SetNeedToAdjustFlagToTrue);

			}

		}

	}

	//this function sets the adjust flag to true
	private void SetNeedToAdjustFlagToTrue(){

		//set the frame delay
		frameCounter = frameDelay;

		//set the flag
		needsToAdjustSize = true;

	}

	//this function gets the main camera area size
	private void GetMainCameraAreaSize(){

		//get the main camera width
		mainCameraWidth = Camera.main.rect.width;

		//calculate the main camera width in pixels
		mainCameraWidthInPixels = mainCameraWidth * canvasWidthInPixels;

		//Debug.Log ("mainCameraWidthInPixels = " + mainCameraWidthInPixels);

	}

	//this function adjusts position of UIPanels if they will be off screen
	private void AdjustUIPanelPositions(){

		//loop through the UI panels
		for (int i = 0; i < UIPanels.Length; i++) {

			//check if the panel size exceeds the main camera area
			if (UIPanels [i].GetComponent<RectTransform> ().rect.width > mainCameraWidthInPixels) {

				//the panel is too large to fit inside the main camera area
				//check if it will also be too large to fit in the screen
				if (UIPanels [i].GetComponent<RectTransform> ().rect.width > canvasWidthInPixels) {

					//the panel is too wide to fit on the screen
					//we will have to scale it down
					float scaleFactor = canvasWidthInPixels / UIPanels [i].GetComponent<RectTransform> ().rect.width;

					//scale the panel down to exactly fit on the screen
					UIPanels [i].transform.localScale = new Vector3 (scaleFactor, scaleFactor, scaleFactor);

				} else {

					//the else condition is that the panel fits on the screen, but is too large to fit in the main camera window
					//in this case, we want to slide it to the right so it fits on screen, even if that means it overlaps the UI side panel

					//calculate how many pixels we are off the edge of the screen by
					float pixelsToMove = (UIPanels [i].GetComponent<RectTransform> ().rect.width - mainCameraWidthInPixels) / 2.0f;

					//calculate how much of the main camera area percentage the anchor x must move to get those pixels back on screen
					float anchorToMove = pixelsToMove / mainCameraWidthInPixels;

					//move the anchor min x and anchor max x by the anchorToMove value
					UIPanels [i].GetComponent<RectTransform> ().anchorMin = new Vector2(0.5f + anchorToMove, UIPanels [i].GetComponent<RectTransform> ().anchorMin.y);
					UIPanels [i].GetComponent<RectTransform> ().anchorMax = new Vector2(0.5f + anchorToMove, UIPanels [i].GetComponent<RectTransform> ().anchorMax.y);

				}

			}

		}

	}

	//this function restores the local scale of the panels to 1
	private void SetPanelScalesToDefault(){

		//loop through the UI panels
		for (int i = 0; i < UIPanels.Length; i++) {

			UIPanels [i].transform.localScale = Vector3.one;

		}

	}

	//this function handles OnDestroy
	private void OnDestroy(){

		RemoveEventListeners ();

	}

	//this function removes event listeners
	private void RemoveEventListeners(){

		if (uiManager.GetComponent<Settings> () != null) {

			//remove listener for changing resolution
			uiManager.GetComponent<Settings> ().OnChangeResolution.RemoveListener (SetNeedToAdjustFlagToTrue);

		}

		if (GameObject.FindGameObjectWithTag ("GameManager") != null) {

			GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameManager> ().OnChangedMainCameraSetup.RemoveListener (SetNeedToAdjustFlagToTrue);

		}

	}

}
