using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class About : MonoBehaviour {

	//variable to hold the panel object
	public GameObject aboutPanel;

	//variable to hold the About button in the main menu
	public Button aboutButton;

	//variable to hold the cancel button
	public Button cancelButton;

	//variable to hold the close button
	public Button closeButton;

	//events to announce opening and closing of about panel
	public UnityEvent OnOpenAboutPanel = new UnityEvent();
	public UnityEvent OnCloseAboutPanel = new UnityEvent();

	//actions
	private UnityAction openAboutPanelAction;
	private UnityAction closeAboutPanelAction;

	// Use this for initialization
	public void Init () {

		//set the actions
		SetUnityActions();

		//add listeners
		AddEventListeners();

		//start with the panel closed
		ClosePanel();
		
	}

	//this function sets the Unity actions
	private void SetUnityActions(){

		openAboutPanelAction = () => {OpenPanel ();};

		closeAboutPanelAction = () => {ClosePanel ();};

	}

	//this function adds event listeners
	private void AddEventListeners(){

		//add listener for open and close buttons
		aboutButton.onClick.AddListener(openAboutPanelAction);
		cancelButton.onClick.AddListener (closeAboutPanelAction);
		closeButton.onClick.AddListener (closeAboutPanelAction);

	}

	//this function opens the panel
	private void OpenPanel(){

		if (aboutPanel.activeInHierarchy == false) {

			aboutPanel.SetActive (true);

			//invoke the event
			OnOpenAboutPanel.Invoke ();

		}

	}

	//this function closes the panel
	private void ClosePanel(){

		if (aboutPanel.activeInHierarchy == true) {

			aboutPanel.SetActive (false);

			//invoke the event
			OnCloseAboutPanel.Invoke ();

		}

	}

	//this function handles the OnDestroy
	private void OnDestroy(){

		RemoveEventListeners ();

	}


	//this function removes event listeners
	private void RemoveEventListeners(){

		if (aboutButton != null) {

			//remove listener for open and close buttons
			aboutButton.onClick.RemoveListener (openAboutPanelAction);

		}

		if (cancelButton != null) {

			//remove listener for open and close buttons
			cancelButton.onClick.RemoveListener (closeAboutPanelAction);

		}

		if (closeButton != null) {

			//remove listener for open and close buttons
			closeButton.onClick.RemoveListener (closeAboutPanelAction);

		}

	}

}
