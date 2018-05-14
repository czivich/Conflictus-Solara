using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PauseFadePanelMainMenu : MonoBehaviour {

	private GameObject uiManager;

	//variable to hold the pauseFadePanel
	public GameObject pauseFadePanel;

	//unityActions
	private UnityAction<string> stringDisablePauseFadePanelAction;

	// Use this for initialization
	public void Init () {

		//get the manager
		uiManager = GameObject.FindGameObjectWithTag("UIManager");

		//set the actions
		stringDisablePauseFadePanelAction = (fileName) => {DisablePauseFadePanel();};

		//add listener for ExitGame Button
		uiManager.GetComponent<ExitGamePrompt>().exitGameButton.onClick.AddListener(EnablePauseFadePanel);

		//add listeners to the exit game prompt buttons
		uiManager.GetComponent<ExitGamePrompt> ().OnExitGameCancelClicked.AddListener (DisablePauseFadePanel);
		uiManager.GetComponent<ExitGamePrompt> ().OnExitGameYesClicked.AddListener (DisablePauseFadePanel);

		//add listeners for the file load window button
		uiManager.GetComponent<FileLoadWindow>().OnOpenFileLoadWindow.AddListener(EnablePauseFadePanel);

		//add listener for close file load window
		uiManager.GetComponent<FileLoadWindow>().closeFileLoadWindowButton.onClick.AddListener(DisablePauseFadePanel);
		uiManager.GetComponent<FileLoadWindow>().OnFileLoadCancelClicked.AddListener(DisablePauseFadePanel);

		//add listener for loading a game
		uiManager.GetComponent<FileLoadWindow>().OnFileLoadYesClicked.AddListener(stringDisablePauseFadePanelAction);

		//add listener for triggering the file delete prompt
		uiManager.GetComponent<FileLoadWindow>().OnFileDeleteYesClicked.AddListener(stringDisablePauseFadePanelAction);

		//add listener for resolving the file delete prompt
		uiManager.GetComponent<FileDeletePrompt>().OnFileDeleteCancelClicked.AddListener(EnablePauseFadePanel);
		uiManager.GetComponent<FileDeletePrompt>().OnFileDeleteCancelClicked.AddListener(EnablePauseFadePanel);

		//add listener for opening the configure local game window
		uiManager.GetComponent<ConfigureLocalGameWindow>().newLocalGameButton.onClick.AddListener(EnablePauseFadePanel);

		//add listener for closing the configure local game window
		uiManager.GetComponent<ConfigureLocalGameWindow>().exitWindowButton.onClick.AddListener(DisablePauseFadePanel);

		//add listener for cancelling the configure local game window
		uiManager.GetComponent<ConfigureLocalGameWindow>().cancelButton.onClick.AddListener(DisablePauseFadePanel);

		//add listener for creating a new local game
		uiManager.GetComponent<ConfigureLocalGameWindow>().OnCreateNewGame.AddListener(DisablePauseFadePanel);

		//add listeners for the settings window open button
		uiManager.GetComponent<Settings>().settingsMenuButton.onClick.AddListener(EnablePauseFadePanel);

		//add listener for settings window exit buttons
		uiManager.GetComponent<Settings>().exitButton.onClick.AddListener(DisablePauseFadePanel);
		uiManager.GetComponent<Settings>().acceptButton.onClick.AddListener(DisablePauseFadePanel);

		//add listeners for the about panel
		uiManager.GetComponent<About>().OnOpenAboutPanel.AddListener(EnablePauseFadePanel);
		uiManager.GetComponent<About>().OnCloseAboutPanel.AddListener(DisablePauseFadePanel);

        //add listeners for the new LAN game panel
        uiManager.GetComponent<NewLANGameWindow>().OnOpenPanel.AddListener(EnablePauseFadePanel);
        uiManager.GetComponent<NewLANGameWindow>().OnClosePanel.AddListener(DisablePauseFadePanel);


        //start with the panel disabled
        DisablePauseFadePanel();

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

			if (uiManager.GetComponent<ExitGamePrompt> ().exitGameButton != null) {
				
				//remove listener for ExitGame Button
				uiManager.GetComponent<ExitGamePrompt> ().exitGameButton.onClick.RemoveListener (EnablePauseFadePanel);

			}

			//remove listeners to the exit game prompt buttons
			uiManager.GetComponent<ExitGamePrompt> ().OnExitGameCancelClicked.RemoveListener (DisablePauseFadePanel);
			uiManager.GetComponent<ExitGamePrompt> ().OnExitGameYesClicked.RemoveListener (DisablePauseFadePanel);

			//remove listeners for the file load window button
			uiManager.GetComponent<FileLoadWindow>().OnOpenFileLoadWindow.RemoveListener(EnablePauseFadePanel);

			if (uiManager.GetComponent<FileLoadWindow> ().closeFileLoadWindowButton != null) {
				
				//remove listener for close file load window
				uiManager.GetComponent<FileLoadWindow> ().closeFileLoadWindowButton.onClick.RemoveListener (DisablePauseFadePanel);

			}

			uiManager.GetComponent<FileLoadWindow>().OnFileLoadCancelClicked.RemoveListener(DisablePauseFadePanel);

			//remove listener for loading a game
			uiManager.GetComponent<FileLoadWindow>().OnFileLoadYesClicked.RemoveListener(stringDisablePauseFadePanelAction);

			//remove listener for triggering the file delete prompt
			uiManager.GetComponent<FileLoadWindow>().OnFileDeleteYesClicked.RemoveListener(stringDisablePauseFadePanelAction);

			//remove listener for resolving the file delete prompt
			uiManager.GetComponent<FileDeletePrompt>().OnFileDeleteCancelClicked.RemoveListener(EnablePauseFadePanel);
			uiManager.GetComponent<FileDeletePrompt>().OnFileDeleteCancelClicked.RemoveListener(EnablePauseFadePanel);

			if (uiManager.GetComponent<ConfigureLocalGameWindow> ().newLocalGameButton != null) {
				
				//remove listener for opening the configure local game window
				uiManager.GetComponent<ConfigureLocalGameWindow> ().newLocalGameButton.onClick.RemoveListener (EnablePauseFadePanel);

			}

			if (uiManager.GetComponent<ConfigureLocalGameWindow> ().exitWindowButton != null) {
				
				//remove listener for closing the configure local game window
				uiManager.GetComponent<ConfigureLocalGameWindow> ().exitWindowButton.onClick.RemoveListener (DisablePauseFadePanel);

			}

			if (uiManager.GetComponent<ConfigureLocalGameWindow> ().cancelButton != null) {
				
				//remove listener for cancelling the configure local game window
				uiManager.GetComponent<ConfigureLocalGameWindow> ().cancelButton.onClick.RemoveListener (DisablePauseFadePanel);

			}

			//remove listener for creating a new local game
			uiManager.GetComponent<ConfigureLocalGameWindow>().OnCreateNewGame.RemoveListener(DisablePauseFadePanel);

		}

		if (uiManager.GetComponent<Settings> ().settingsMenuButton != null) {
			
			//add listeners for the settings window open button
			uiManager.GetComponent<Settings> ().settingsMenuButton.onClick.AddListener (EnablePauseFadePanel);

		}

		if (uiManager.GetComponent<Settings> ().exitButton != null) {

			//add listener for settings window exit buttons
			uiManager.GetComponent<Settings> ().exitButton.onClick.AddListener (DisablePauseFadePanel);

		}

		if (uiManager.GetComponent<Settings> ().acceptButton != null) {
			
			uiManager.GetComponent<Settings> ().acceptButton.onClick.AddListener (DisablePauseFadePanel);

		}

		//remove listeners for the about panel
		uiManager.GetComponent<About>().OnOpenAboutPanel.RemoveListener(EnablePauseFadePanel);
		uiManager.GetComponent<About>().OnCloseAboutPanel.RemoveListener(DisablePauseFadePanel);

        //remove listeners for the new LAN game panel
        uiManager.GetComponent<NewLANGameWindow>().OnOpenPanel.RemoveListener(EnablePauseFadePanel);
        uiManager.GetComponent<NewLANGameWindow>().OnClosePanel.RemoveListener(DisablePauseFadePanel);

    }

}
