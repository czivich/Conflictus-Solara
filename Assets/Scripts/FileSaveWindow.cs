using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class FileSaveWindow : MonoBehaviour {

	//this variable holds the fileSaveWindowPanel
	public GameObject fileSaveWindow;

	//need a variable for the button which will open the file save window (sometimes)
	public Button openFileSaveWindowButton;

	//need a variable for the save As button which will open the file save window
	public Button openFileSaveAsWindowButton;

	//button to close the fileSaveWindow
	public Button closeFileSaveWindowButton;

	//variable to hold the fileList
	public GameObject fileList;

	//variable to hold the input text field
	public TMP_InputField fileNameInputField;

	//variable to hold the Save button
	public Button fileSaveYesButton;

	//variable to hold the save cancel button
	public Button fileSaveCancelButton;

	//variable to hold the fileListItemPrefab
	public GameObject fileListItemPrefab;

	//variable to hold the placeholder text
	public TextMeshProUGUI placeholderText;

	//variable to hold the file save names
	public string[] fileSaveNames {

		get;
		private set;

	}

	//managers
	private UIManager uiManager;
	private GameManager gameManager;

	//event to announce opening FileSaveWindow
	public UnityEvent OnOpenFileSaveWindow;
	public UnityEvent OnCloseFileSaveWindow;


	//event to announce fileSaveCancel pressed
	public UnityEvent OnFileSaveCancelClicked = new UnityEvent();

	//event and class to announce fileSaveYes clicked
	public FileSaveEvent OnFileSaveYesClickedExistingFile = new FileSaveEvent();
	public FileSaveEvent OnFileSaveYesClickedNewFile = new FileSaveEvent();
	public FileSaveEvent OnFileSaveYesClickedSameFile = new FileSaveEvent();

	public class FileSaveEvent : UnityEvent<string>{};

	//unityActions
	private UnityAction<string> stringUpdatedInputFieldAction;
	private UnityAction<string> stringEndedInputEditAction;
	private UnityAction<string> stringCloseWindowAction;
	private UnityAction<GameManager.ActionMode> actionModeSetButtonStatusAction;

	// Use this for initialization
	public void Init () {

		//get the managers
		uiManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
		gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

		//set the actions
		stringUpdatedInputFieldAction = (value) => {UpdatedInputField();};
		stringEndedInputEditAction = (inputText) => {EndedInputEdit(inputText);};
		stringCloseWindowAction = (fileName) => {CloseFileSaveWindow();};
		actionModeSetButtonStatusAction = (actionMode) => {SetSaveButtonStatus (actionMode);};

		//add a listener for clicking the open window button
		openFileSaveWindowButton.onClick.AddListener(ResolveSaveButtonClick);

		//add a listener for clicking the SaveAs open window button
		openFileSaveAsWindowButton.onClick.AddListener(OpenFileSaveWindow);

		//add a listener for clicking the close window X button
		closeFileSaveWindowButton.onClick.AddListener(ClickedFileSaveCancel);

		//add a listener for clicking the cancel save button
		fileSaveCancelButton.onClick.AddListener(ClickedFileSaveCancel);

		//add listener for the inputField value changed
		fileNameInputField.onValueChanged.AddListener(stringUpdatedInputFieldAction);

		//add listener for onEndEdit for the input field
		fileNameInputField.onEndEdit.AddListener(stringEndedInputEditAction);

		//add a listener for clicking the yes save button
		fileSaveYesButton.onClick.AddListener(ClickedFileSaveYes);

		//add a listener for the overwrite yes button
		uiManager.GetComponent<FileOverwritePrompt>().OnFileOverwriteYesClicked.AddListener(stringCloseWindowAction);

		//add a listener for action mode changes
		gameManager.OnActionModeChange.AddListener(actionModeSetButtonStatusAction);

		//start with the window closed
		CloseFileSaveWindow ();


	}

	//this function resolves whether the window needs to open on the save button press
	private void ResolveSaveButtonClick(){

		//check if there is a current file in the fileManager
		if (uiManager.GetComponent<FileManager> ().currentFileName != null) {

			//there is a current file name
			//this means we should just save over the current when "Save" is clicked without opening the file save window
			OnFileSaveYesClickedSameFile.Invoke (uiManager.GetComponent<FileManager> ().currentFileName);

		} else {

			//the else condition is that there is no current file name
			//in this case, clicking save needs to behave the same as Save As, so we need to open the panel
			OpenFileSaveWindow();

		}

	}
	
	//this function opens the fileSaveWindow
	private void OpenFileSaveWindow(){
		
		//enable the window object
		fileSaveWindow.SetActive(true);

		//when we open the window, we want to get the existing save files
		GetExistingSaveFiles();

		//set the placeholder text
		placeholderText.text = ("Enter File Name...");

		//start with the fileSaveYesButton not interactable
		fileSaveYesButton.interactable = false;

		//invoke the open window event
		OnOpenFileSaveWindow.Invoke ();

		//Debug.Log ("Open filesave");

	}

	//this function opens the fileSaveWindow
	private void CloseFileSaveWindow(){

		//disable the window object
		fileSaveWindow.SetActive(false);

		//clear the file list
		ClearFileNames();

		//clear the text input box
		fileNameInputField.text = "";

		OnCloseFileSaveWindow.Invoke ();

	}

	//this function gets the existing save files
	private void GetExistingSaveFiles(){
		
		//check if the saves directory exists
		if (Directory.Exists (FileManager.FileSaveBasePath ()) == false) {

			//the save directory does not exist.  we need to create it
			//if it doesn't exist, create it
			Directory.CreateDirectory (FileManager.FileSaveBasePath ());

		}

		//get an array of strings for all the exising save files
		string[] existingSaves = Directory.GetFiles (FileManager.FileSaveBasePath ());

		//store the existing saves
		fileSaveNames = existingSaves;

		//trim the fileSaveNames
		for (int i = 0; i < fileSaveNames.Length; i++) {

			fileSaveNames [i] = Path.GetFileNameWithoutExtension (fileSaveNames [i]);

		}

		//now for each file name, we want to create a fileSaveList object
		foreach (string saveFileName in existingSaves) {

			//instantiate an object from the prefab
			GameObject fileListItem = (GameObject)Instantiate (fileListItemPrefab);

			//run the fileListItem Init()
			fileListItem.GetComponent<FileListItem> ().Init ();

			//Set the parent of the new fileListItem
			fileListItem.transform.SetParent (fileList.transform);

			//set the scale of the new item
			fileListItem.transform.localScale = Vector3.one;

			//set the text in the fileListItem to match the fileName
			fileListItem.GetComponentInChildren<TextMeshProUGUI> ().text = (Path.GetFileNameWithoutExtension (saveFileName));

		}

	}

	//this function clears out file names
	private void ClearFileNames(){

		//destroy all children of the filelist
		for(int i = 0; i < fileList.transform.childCount; i++){
			
			GameObject.Destroy (fileList.transform.GetChild (i).gameObject);

		}

	}

	//this function will handle when inputs are made into the text input field
	private void UpdatedInputField(){

		//this function will be called whenever the input field value is changed
		//check if the current field value is blank
		if (fileNameInputField.text != "") {

			//if the value is not null, then we can allow the yes button to be clicked
			fileSaveYesButton.interactable = true;

		}
		//the else condition is that the text input is null
		else {

			//if the value is null, then we cannot allow the yes button to be clicked
			fileSaveYesButton.interactable = false;

		}

	}

	//this function will respond to ending input edit on the rename field
	private void EndedInputEdit(string inputText){

		//check if the user has hit enter with a valid rename string entered
		if (inputText != "" && (Input.GetKey (KeyCode.Return) == true || Input.GetKey (KeyCode.KeypadEnter) == true)) {

			//if so, treat that as hitting the yes button
			ClickedFileSaveYes();

		}

	}

	//this function handles clicking the FileSaveYesButton
	private void ClickedFileSaveYes(){

		//check if the entered file save name is a new name or if it is one that is in the list of existing saves
		if (System.Array.Exists (fileSaveNames, element => element == fileNameInputField.text) == true) {

			//the file name entered is an existing file
			//we should prompt the user to confirm they want to overwrite it
			OnFileSaveYesClickedExistingFile.Invoke(fileNameInputField.text);

		} else {

			//this is a new file name, we can save it and exit the window
			OnFileSaveYesClickedNewFile.Invoke(fileNameInputField.text);

			//close the window
			CloseFileSaveWindow ();

		}
			
	}

	//this function sets the save button status
	//it has since been updated to also update the previousUnitButton status
	private void SetSaveButtonStatus(GameManager.ActionMode actionMode){

		//check if we are in a locked action mode
		if (UIManager.lockMenuActionModes.Contains (actionMode)) {

			openFileSaveWindowButton.interactable = false;
			openFileSaveAsWindowButton.interactable = false;

		} else {

			//if the mode isn't locked, the button can be interactable
			openFileSaveWindowButton.interactable = true;
			openFileSaveAsWindowButton.interactable = true;


		}

	}

	//this function handles clicking the FileSaveCancelButton
	private void ClickedFileSaveCancel(){

		//close the window
		CloseFileSaveWindow ();

		//invoke the cancel button event
		OnFileSaveCancelClicked.Invoke ();

	}

	//this function handles OnDestroy
	private void OnDestroy(){

		RemoveAllListeners ();

	}

	//this function removes all listeners
	private void RemoveAllListeners(){

		if (openFileSaveWindowButton != null) {
			
			//remove a listener for clicking the open window button
			openFileSaveWindowButton.onClick.RemoveListener (ResolveSaveButtonClick);

		}

		if (openFileSaveAsWindowButton != null) {
			
			//remove a listener for clicking the SaveAs open window button
			openFileSaveAsWindowButton.onClick.RemoveListener(OpenFileSaveWindow);

		}

		if (closeFileSaveWindowButton != null) {
			
			//remove a listener for clicking the close window X button
			closeFileSaveWindowButton.onClick.RemoveListener (ClickedFileSaveCancel);

		}

		if (fileSaveCancelButton != null) {
			
			//remove a listener for clicking the cancel save button
			fileSaveCancelButton.onClick.RemoveListener (ClickedFileSaveCancel);

		}

		if (fileNameInputField != null) {
			

			//remove listener for the inputField value changed
			fileNameInputField.onValueChanged.RemoveListener (stringUpdatedInputFieldAction);

			//remove listener for onEndEdit for the input field
			fileNameInputField.onEndEdit.RemoveListener (stringEndedInputEditAction);

		}

		if (fileSaveYesButton != null) {

			//remove a listener for clicking the yes save button
			fileSaveYesButton.onClick.RemoveListener (ClickedFileSaveYes);

		}

		if (uiManager != null) {

			//remove a listener for the overwrite yes button
			uiManager.GetComponent<FileOverwritePrompt> ().OnFileOverwriteYesClicked.RemoveListener (stringCloseWindowAction);

		}

		if (gameManager != null) {

			//remove a listener for action mode changes
			gameManager.OnActionModeChange.RemoveListener(actionModeSetButtonStatusAction);

		}

	}

}
