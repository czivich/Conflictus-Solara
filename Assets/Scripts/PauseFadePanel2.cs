using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PauseFadePanel2 : MonoBehaviour {

	private GameObject uiManager;

	//variable to hold the pauseFadePanel
	public GameObject pauseFadePanel;

	//variable to hold the main scene
	private Scene mainScene;

	//unityActions
	private UnityAction<string> stringEnablePauseFadePanelAction;
	private UnityAction<string> stringDisablePauseFadePanelAction;

	// Use this for initialization
	public void Init () {

		//get the manager
		uiManager = GameObject.FindGameObjectWithTag("UIManager");

		//set the actions
		stringEnablePauseFadePanelAction = (fileName) => {EnablePauseFadePanel();};
		stringDisablePauseFadePanelAction = (fileName) => {DisablePauseFadePanel();};

		//get the main scene
		mainScene = SceneManager.GetSceneByName("Main");

		//check if we are in the main scene
		if (SceneManager.GetActiveScene () == mainScene) {

			//add listener to the file save window overwrite prompt action
			uiManager.GetComponent<FileSaveWindow> ().OnFileSaveYesClickedExistingFile.AddListener (stringEnablePauseFadePanelAction);

			//add listeners to the overwrite prompt buttons
			uiManager.GetComponent<FileOverwritePrompt> ().OnFileOverwriteYesClicked.AddListener (stringDisablePauseFadePanelAction);
			uiManager.GetComponent<FileOverwritePrompt> ().OnFileSaveCancelClicked.AddListener (DisablePauseFadePanel);

		}

		//add listener to the file load window delete prompt action
		uiManager.GetComponent<FileLoadWindow>().OnFileDeleteYesClicked.AddListener(stringEnablePauseFadePanelAction);

		//add listeners to the delete prompt buttons
		uiManager.GetComponent<FileDeletePrompt>().OnFileDeleteYesClicked.AddListener(stringDisablePauseFadePanelAction);
		uiManager.GetComponent<FileDeletePrompt> ().OnFileDeleteCancelClicked.AddListener (DisablePauseFadePanel);

		//DisablePauseFadePanel ();

	}
	

	//this function turns on the pauseFadePanel
	private void EnablePauseFadePanel(){

		pauseFadePanel.SetActive (true);

	}

	//this function turns off the pauseFadePanel
	private void DisablePauseFadePanel(){

		pauseFadePanel.SetActive (false);

	}

	//this function handles OnDestroy
	private void OnDestroy(){

		RemoveAllListeners ();

	}

	//this function removes all listeners
	private void RemoveAllListeners(){

		if(uiManager != null){

			if (uiManager.GetComponent<FileSaveWindow> () != null) {
				
				//remove listener to the file save window overwrite prompt action
				uiManager.GetComponent<FileSaveWindow> ().OnFileSaveYesClickedExistingFile.RemoveListener (stringEnablePauseFadePanelAction);

			}

			if (uiManager.GetComponent<FileOverwritePrompt> () != null) {
				
				//remove listeners to the overwrite prompt buttons
				uiManager.GetComponent<FileOverwritePrompt> ().OnFileOverwriteYesClicked.RemoveListener (stringDisablePauseFadePanelAction);
				uiManager.GetComponent<FileOverwritePrompt> ().OnFileSaveCancelClicked.RemoveListener (DisablePauseFadePanel);


			}

			//remove listener to the file load window delete prompt action
			uiManager.GetComponent<FileLoadWindow>().OnFileDeleteYesClicked.RemoveListener(stringEnablePauseFadePanelAction);

			//remove listeners to the delete prompt buttons
			uiManager.GetComponent<FileDeletePrompt>().OnFileDeleteYesClicked.RemoveListener(stringDisablePauseFadePanelAction);
			uiManager.GetComponent<FileDeletePrompt> ().OnFileDeleteCancelClicked.RemoveListener (DisablePauseFadePanel);

		}

	}

}
