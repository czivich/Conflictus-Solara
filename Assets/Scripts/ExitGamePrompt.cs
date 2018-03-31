using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ExitGamePrompt : MonoBehaviour {

	//manager
	private GameManager gameManager;

	//scene index
	private int mainSceneIndex = 1;

	//variable to hold the window
	public GameObject exitGamePromptWindow;

	//button to launch the exit game prompt
	public Button exitGameButton;

	//variables to hold the yes and cancel buttons
	public Button exitGameYesButton;
	public Button exitGameCancelButton;

	//variable to hold the prompt text
	public TextMeshProUGUI promptText;

	//event to announce exit prompt opened
	public UnityEvent OnExitGamePromptOpened = new UnityEvent();


	//event to announce exitGameCancel pressed
	public UnityEvent OnExitGameYesClicked = new UnityEvent();
	public UnityEvent OnExitGameCancelClicked = new UnityEvent();

	//unityAction
	private UnityAction<GameManager.ActionMode> actionModeSetButtonStatusAction;

	// Use this for initialization
	public void Init () {

		//set the actions
		actionModeSetButtonStatusAction = (actionMode) => {SetExitButtonStatus(actionMode);};

		//get the gameManager if we are in the main scene
		if (SceneManager.GetActiveScene ().buildIndex == mainSceneIndex) {

			//get the gameManager
			gameManager = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameManager> ();

			//add listener for actionMode change
			gameManager.OnActionModeChange.AddListener(actionModeSetButtonStatusAction);

		}


		//add listeners to the button presses
		exitGameYesButton.onClick.AddListener(ClickedExitYes);
		exitGameCancelButton.onClick.AddListener(ClickedExitCancel);
		exitGameButton.onClick.AddListener (OpenWindow);

		//start with the window closed
		CloseWindow();

	}

	//this function opens the window
	private void OpenWindow(){

		//enable the window
		exitGamePromptWindow.SetActive (true);

		//set the prompt text
		promptText.text = ("Are you sure you want to exit the game?");

		//update the font size if necessary
		UIManager.AutoSizeTextMeshFont(promptText);

		OnExitGamePromptOpened.Invoke ();

	}

	//this function closes the window
	private void CloseWindow(){

		//diable the window
		exitGamePromptWindow.SetActive (false);

	}

	//this function sets the exit button status
	private void SetExitButtonStatus(GameManager.ActionMode actionMode){

		//check if we are in a locked action mode
		if (UIManager.lockMenuActionModes.Contains (actionMode)) {

			exitGameButton.interactable = false;

		} else {

			//if the mode isn't locked, the button can be interactable
			exitGameButton.interactable = true;

		}

	}

	//this function responds to the click yes
	private void ClickedExitYes(){

		//close the window
		CloseWindow ();

		//invoke the yes event
		OnExitGameYesClicked.Invoke();

	}

	//this function responds to the click cencel
	private void ClickedExitCancel(){

		//close the window
		CloseWindow ();

		//invoke the cancel event
		OnExitGameCancelClicked.Invoke();

	}

	//this function handles OnDestroy
	private void OnDestroy(){

		RemoveAllListeners ();

	}

	//this function removes all listeners
	private void RemoveAllListeners(){

		//remove listeners to the button presses

		if (exitGameYesButton != null) {

			exitGameYesButton.onClick.RemoveListener (ClickedExitYes);

		}

		if (exitGameCancelButton != null) {

			exitGameCancelButton.onClick.RemoveListener (ClickedExitCancel);

		}

		if (exitGameButton != null) {

			exitGameButton.onClick.RemoveListener (OpenWindow);

		}

		if (gameManager != null) {

			//remove listener for actionMode change
			gameManager.OnActionModeChange.RemoveListener (actionModeSetButtonStatusAction);

		}

	}

}

