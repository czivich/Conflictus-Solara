using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class FileOverwritePrompt : MonoBehaviour {

	//variable to hold the window
	public GameObject fileOverwritePromptWindow;

	//variables to hold the yes and cancel buttons
	public Button fileOverwriteYesButton;
	public Button fileOverwriteCancelButton;

	//variable to hold the prompt text
	public TextMeshProUGUI promptText;

	//variable to hold the save fileName in question
	private string saveFileName;

	//managers
	private UIManager uiManager;

	//event and class to announce fileSaveYes clicked
	public FileSaveEvent OnFileOverwriteYesClicked = new FileSaveEvent();
	public class FileSaveEvent : UnityEvent<string>{};

	//event to announce fileSaveCancel pressed
	public UnityEvent OnFileSaveCancelClicked = new UnityEvent();

	//unityActions
	private UnityAction<string> stringOpenWindowAction;

	// Use this for initialization
	public void Init () {

		//get the manager
		uiManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();

		//set the actions
		stringOpenWindowAction = (fileName) => {OpenWindow(fileName);};

		//add listener to the fileSaveWindow action
		uiManager.GetComponent<FileSaveWindow>().OnFileSaveYesClickedExistingFile.AddListener(stringOpenWindowAction);

		//add listeners to the button presses
		fileOverwriteYesButton.onClick.AddListener(ClickedOverwriteYes);
		fileOverwriteCancelButton.onClick.AddListener(ClickedOverwriteCancel);

		//start with the window closed
		CloseWindow();

	}

	//this function opens the window
	private void OpenWindow(string fileName){

		//cache the filename
		saveFileName = fileName;

		//enable the window
		fileOverwritePromptWindow.SetActive (true);

		//set the prompt text
		promptText.text = ("File " + fileName + " already exists.  Are you sure you want to overwrite it?");

	}

	//this function closes the window
	private void CloseWindow(){

		//diable the window
		fileOverwritePromptWindow.SetActive (false);

	}

	//this function responds to the click yes
	private void ClickedOverwriteYes(){

		//close the window
		CloseWindow ();

		//invoke the yes event
		OnFileOverwriteYesClicked.Invoke(saveFileName);

	}

	//this function responds to the click cencel
	private void ClickedOverwriteCancel(){

		//close the window
		CloseWindow ();

		//invoke the cancel event
		OnFileSaveCancelClicked.Invoke();

	}

	//this function handles OnDestroy
	private void OnDestroy(){

		RemoveAllListeners ();

	}

	//this function removes all listeners
	private void RemoveAllListeners(){

		if (uiManager != null) {
			
			//remove listener to the fileSaveWindow action
			uiManager.GetComponent<FileSaveWindow> ().OnFileSaveYesClickedExistingFile.RemoveListener (stringOpenWindowAction);

		}

		//remove listeners to the button presses

		if (fileOverwriteYesButton != null) {
			
			fileOverwriteYesButton.onClick.RemoveListener (ClickedOverwriteYes);

		}

		if (fileOverwriteCancelButton != null) {
			
			fileOverwriteCancelButton.onClick.RemoveListener (ClickedOverwriteCancel);

		}

	}

}
