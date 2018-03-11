using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using TMPro;

public class FileLoadListItem : MonoBehaviour,IPointerClickHandler {

	private Color selectedColor = new Color( 0f, 1f, 1f, 1f);
	private Color notSelectedColor = new Color( 1f, 1f, 1f, 0f);

	//class and event to announce a highlighted file
	public static FileSelectedEvent OnLoadFileSelected = new FileSelectedEvent();
	public class FileSelectedEvent : UnityEvent<FileLoadListItem>{}

	//UnityActions
	private UnityAction<FileLoadListItem> fileListItemSetHighlightedStateAction;

	//this function handles initialization
	public void Init(){

		//set the actions
		fileListItemSetHighlightedStateAction = (fileLoadListItem) => {SetHighlightedState(fileLoadListItem);};

		//add a listener for the OnLoadFileSelectedEvent
		FileLoadListItem.OnLoadFileSelected.AddListener(fileListItemSetHighlightedStateAction);

	}

	//this function handles the pointer click
	public void OnPointerClick(PointerEventData eventData){

		//invoke the OnSaveFileSelectedEvent
		OnLoadFileSelected.Invoke(this);

	}

	//this function responds to the action
	private void SetHighlightedState(FileLoadListItem fileLoadListItem){

		//check if the passed fileListItem is this
		if (this == fileLoadListItem) {

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

		//remove a listener for the OnLoadFileSelectedEvent
		FileLoadListItem.OnLoadFileSelected.RemoveListener(fileListItemSetHighlightedStateAction);

	}

}

