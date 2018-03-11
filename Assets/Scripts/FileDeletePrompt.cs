using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class FileDeletePrompt : MonoBehaviour {

	//variable to hold the window
	public GameObject fileDeletePromptWindow;

	//variables to hold the yes and cancel buttons
	public Button fileDeleteYesButton;
	public Button fileDeleteCancelButton;

	//variable to hold the prompt text
	public TextMeshProUGUI promptText;

	//variable to hold the save fileName in question
	private string deleteFileName;

	//managers
	private GameObject uiManager;

	//event and class to announce fileSaveYes clicked
	public FileDeleteEvent OnFileDeleteYesClicked = new FileDeleteEvent();
	public class FileDeleteEvent : UnityEvent<string>{};

	//event to announce fileDeleteCancel pressed
	public UnityEvent OnFileDeleteCancelClicked = new UnityEvent();

	//unityActions
	private UnityAction<string> stringOpenWindowAction;

	// Use this for initialization
	public void Init () {

		//get the manager
		uiManager = GameObject.FindGameObjectWithTag("UIManager");

		//set the actions
		stringOpenWindowAction = (fileName) => {OpenWindow(fileName);};

		//add listener to the fileLoadWindow action
		uiManager.GetComponent<FileLoadWindow>().OnFileDeleteYesClicked.AddListener(stringOpenWindowAction);

		//add listeners to the button presses
		fileDeleteYesButton.onClick.AddListener(ClickedDeleteYes);
		fileDeleteCancelButton.onClick.AddListener(ClickedDeleteCancel);

		//start with the window closed
		CloseWindow();

	}

	//this function opens the window
	private void OpenWindow(string fileName){

		//cache the filename
		deleteFileName = fileName;

		//enable the window
		fileDeletePromptWindow.SetActive (true);

		//set the prompt text
		promptText.text = ("Are you sure you want to delete the file " + fileName + "?");

	}

	//this function closes the window
	private void CloseWindow(){

		//diable the window
		fileDeletePromptWindow.SetActive (false);

	}

	//this function responds to the click yes
	private void ClickedDeleteYes(){

		//close the window
		CloseWindow ();

		//invoke the yes event
		OnFileDeleteYesClicked.Invoke(deleteFileName);

	}

	//this function responds to the click cencel
	private void ClickedDeleteCancel(){

		//close the window
		CloseWindow ();

		//invoke the cancel event
		OnFileDeleteCancelClicked.Invoke();

	}

	//this function handles OnDestroy
	private void OnDestroy(){

		RemoveAllListeners ();

	}

	//this function removes all listeners
	private void RemoveAllListeners(){

		if (uiManager != null) {

			//remove listener to the fileSaveWindow action
			uiManager.GetComponent<FileLoadWindow>().OnFileDeleteYesClicked.RemoveListener(stringOpenWindowAction);

		}

		//remove listeners to the button presses

		if (fileDeleteYesButton != null) {

			fileDeleteYesButton.onClick.RemoveListener (ClickedDeleteYes);

		}

		if (fileDeleteCancelButton != null) {

			fileDeleteCancelButton.onClick.RemoveListener (ClickedDeleteCancel);

		}

	}


}
