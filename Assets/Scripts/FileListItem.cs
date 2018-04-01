using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using TMPro;


public class FileListItem : MonoBehaviour,IPointerClickHandler {

	//manager
	private UIManager uiManager;

	//variable to hold the input field that we want to copy the filename text to
	public TMP_InputField fileNameInputField;

	private Color selectedColor = new Color32 (160, 255, 255, 255);
	private Color notSelectedColor = new Color32( 255, 255, 255, 0);

	//class and event to announce a highlighted file
	public static FileSelectedEvent OnSaveFileSelected = new FileSelectedEvent();
	public class FileSelectedEvent : UnityEvent<FileListItem>{}

	//UnityActions
	private UnityAction<FileListItem> fileListItemSetHighlightedStateAction;
	private UnityAction<string> stringSetHighlightedStatusAction;
	private UnityAction<GameObject> setSelectedFileListItemAction;

	//this function handles initialization
	public void Init(){

		//get the manager
		uiManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();

		//get the fileNameInputField
		fileNameInputField = GameObject.FindGameObjectWithTag ("UIManager").GetComponent<UIManager> ().GetComponent<FileSaveWindow> ().fileNameInputField;

		//set the actions
		fileListItemSetHighlightedStateAction = (fileListItem) => {SetHighlightedState(fileListItem);};
		stringSetHighlightedStatusAction = (fileName) => {SetHighlightedStatus(fileName);};
		setSelectedFileListItemAction = (gameObject) => {OnSelectFile(gameObject);};

		//add a listener for the OnSaveFileSelectedEvent
		FileListItem.OnSaveFileSelected.AddListener(fileListItemSetHighlightedStateAction);

		//add a listener for the inputField changing
		uiManager.GetComponent<FileSaveWindow>().fileNameInputField.onValueChanged.AddListener(stringSetHighlightedStatusAction);

		//add a listener for the file getting selected
		uiManager.GetComponent<UINavigationMain>().OnSelectSaveFile.AddListener(setSelectedFileListItemAction);

	}

	//this function handles the pointer click
	public void OnPointerClick(PointerEventData eventData){

		//copy the file name text to the input field
		fileNameInputField.text = transform.GetComponentInChildren<TextMeshProUGUI> ().text;

		//invoke the OnSaveFileSelectedEvent
		OnSaveFileSelected.Invoke(this);

	}

	//this function handles the selection from arrow keys
	public void OnSelectFile(GameObject gameObject){

		//check if this is the gameObject
		if (this.gameObject == gameObject) {

			//copy the file name text to the input field
			fileNameInputField.text = transform.GetComponentInChildren<TextMeshProUGUI> ().text;

			//invoke the OnSaveFileSelectedEvent
			OnSaveFileSelected.Invoke (this);

		}

	}

	//this function responds to the action
	private void SetHighlightedState(FileListItem fileListItem){

		//check if the passed fileListItem is this
		if (this == fileListItem) {

			//set the panel color to the selectedColor
			this.gameObject.GetComponent<Image> ().color = selectedColor;

		} else {

			//else we want to set the panel to the unselected color
			this.gameObject.GetComponent<Image> ().color = notSelectedColor;

		}

	}

	//this function responds to the action
	private void SetHighlightedStatus(string fileName){

		//check if the string matches the filename
		if (this.transform.GetComponentInChildren<TextMeshProUGUI> ().text == fileName) {

			//set the panel color to the selectedColor
			this.gameObject.GetComponent<Image> ().color = selectedColor;

		} else {

			//else we want to set the panel to the unselected color
			this.gameObject.GetComponent<Image> ().color = notSelectedColor;

		}

	}

	//this function handles OnDestroy
	private void OnDestroy(){

		RemoveAllListeners ();

	}

	//this function removes all listeners
	private void RemoveAllListeners(){

		//remove a listener for the OnSaveFileSelectedEvent
		FileListItem.OnSaveFileSelected.RemoveListener(fileListItemSetHighlightedStateAction);

		if (uiManager != null) {
			
			//remove a listener for the inputField changing
			uiManager.GetComponent<FileSaveWindow> ().fileNameInputField.onValueChanged.RemoveListener (stringSetHighlightedStatusAction);

			//remove a listener for the file getting selected
			uiManager.GetComponent<UINavigationMain>().OnSelectSaveFile.RemoveListener(setSelectedFileListItemAction);

		}

	}

}
