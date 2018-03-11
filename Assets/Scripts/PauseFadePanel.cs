using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PauseFadePanel : MonoBehaviour {

	//managers
	private GameManager gameManager;
	private UIManager uiManager;
	private MouseManager mouseManager;

	//variable to hold the open status panel button
	public Button openStatusPanelButton;

	//variable to hold the close status panel button
	public Button closeStatusPanelButton;

	//variable to hold the open purchase item button
	public Button openPurchaseItemsPanelButton;

	//variable to hold the close purchase item button
	public Button closePurchaseItemsPanelButton;

	//variable to hold the purchase items button
	public Button purchaseItemsButton;

	//variable to hold the open purchase ship button
	public Button openPurchaseShipPanelButton;

	//variable to hold the cancel purchase ship button
	public Button cancelPurchaseShipPanelButton;

	//variables to hold the rename ship buttons
	public Button renameShipMenuButton;
	public Button renameShipYesButton;
	public Button renameShipCancelButton;

	//variable to hold the pauseFadePanel
	public GameObject pauseFadePanel;

	//unityActions
	private UnityAction<Hex> hexEnablePauseFadePanelAction; 
	private UnityAction disablePauseFadePanelAction; 
	private UnityAction<NameNewShip.NewUnitEventData> newUnitDataDisablePauseFadePanelAction; 
	private UnityAction enablePauseFadePanelAction; 
	private UnityAction<FlareManager.FlareEventData> flareDataDisablePauseFadePanelAction; 
	private UnityAction<string> stringDisablePauseFadePanelAction; 
	private UnityAction<GameManager.ActionMode> actionModeDisablePauseFadePanelAction; 
	private UnityAction<CombatUnit,string,GameManager.ActionMode> renamedUnitDisablePauseFadePanelAction; 


	// Use this for initialization
	public void Init () {

		mouseManager = GameObject.FindGameObjectWithTag("MouseManager").GetComponent<MouseManager>();
		uiManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();

		//set the actions
		hexEnablePauseFadePanelAction = (hex) => {EnablePauseFadePanel();};
		disablePauseFadePanelAction = () => {DisablePauseFadePanel();};
		newUnitDataDisablePauseFadePanelAction = (purchaseEventData) => {DisablePauseFadePanel();};
		enablePauseFadePanelAction = () => {EnablePauseFadePanel();};
		flareDataDisablePauseFadePanelAction = (data) => {DisablePauseFadePanel();};
		stringDisablePauseFadePanelAction = (fileName) => {DisablePauseFadePanel();};
		actionModeDisablePauseFadePanelAction = (actionMode) => {DisablePauseFadePanel();};
		renamedUnitDisablePauseFadePanelAction = (combatUnit,unitName,actionMode) => {DisablePauseFadePanel();};

		//add listeners for the statusPanel buttons
		openStatusPanelButton.onClick.AddListener(EnablePauseFadePanel);
		closeStatusPanelButton.onClick.AddListener(DisablePauseFadePanel);

		//add listeners for the purchaseItems buttons
		openPurchaseItemsPanelButton.onClick.AddListener(EnablePauseFadePanel);
		closePurchaseItemsPanelButton.onClick.AddListener(DisablePauseFadePanel);
		purchaseItemsButton.onClick.AddListener(DisablePauseFadePanel);

		//add listeners for the purchase ship buttons
		openPurchaseShipPanelButton.onClick.AddListener(EnablePauseFadePanel);
		cancelPurchaseShipPanelButton.onClick.AddListener(DisablePauseFadePanel);

		//add listener for picking location of new unit
		mouseManager.OnPlacedNewUnit.AddListener(hexEnablePauseFadePanelAction);

		//add listeners for either purchasing or canceling from the name new unit panel
		uiManager.GetComponent<NameNewShip>().OnCanceledPurchase.AddListener(disablePauseFadePanelAction);
		uiManager.GetComponent<NameNewShip>().OnPurchasedNewShip.AddListener(newUnitDataDisablePauseFadePanelAction);

		//add listeners for the rename ship buttons
		renameShipMenuButton.onClick.AddListener(EnablePauseFadePanel);
		uiManager.GetComponent<RenameShip> ().OnRenameUnit.AddListener (renamedUnitDisablePauseFadePanelAction);
		uiManager.GetComponent<RenameShip> ().OnRenameCancel.AddListener (actionModeDisablePauseFadePanelAction);


		//add listeners for flare use panel
		uiManager.GetComponent<FlareManager>().OnShowFlarePanel.AddListener(enablePauseFadePanelAction);
		uiManager.GetComponent<FlareManager>().OnUseFlaresCancel.AddListener(flareDataDisablePauseFadePanelAction);
		uiManager.GetComponent<FlareManager>().OnUseFlaresYes.AddListener(flareDataDisablePauseFadePanelAction);

		//add listeners for FileSaveWindow buttons
		uiManager.GetComponent<FileSaveWindow>().closeFileSaveWindowButton.onClick.AddListener(disablePauseFadePanelAction);
		uiManager.GetComponent<FileSaveWindow>().fileSaveCancelButton.onClick.AddListener(disablePauseFadePanelAction);
		uiManager.GetComponent<FileSaveWindow>().OnFileSaveYesClickedExistingFile.AddListener(stringDisablePauseFadePanelAction);
		uiManager.GetComponent<FileSaveWindow>().OnFileSaveYesClickedNewFile.AddListener(stringDisablePauseFadePanelAction);
		uiManager.GetComponent<FileSaveWindow> ().OnOpenFileSaveWindow.AddListener (enablePauseFadePanelAction);

		//add listeners for the overwrite prompt buttons
		uiManager.GetComponent<FileOverwritePrompt>().OnFileSaveCancelClicked.AddListener(enablePauseFadePanelAction);

		//add listeners for the FileLoadWindow buttons
		uiManager.GetComponent<FileLoadWindow>().closeFileLoadWindowButton.onClick.AddListener(disablePauseFadePanelAction);
		uiManager.GetComponent<FileLoadWindow>().fileLoadCancelButton.onClick.AddListener(disablePauseFadePanelAction);
		uiManager.GetComponent<FileLoadWindow>().OnFileLoadYesClicked.AddListener(stringDisablePauseFadePanelAction);
		uiManager.GetComponent<FileLoadWindow> ().OnOpenFileLoadWindow.AddListener (enablePauseFadePanelAction);

		//add listeners for the delete prompt buttons
		uiManager.GetComponent<FileDeletePrompt>().OnFileDeleteCancelClicked.AddListener(enablePauseFadePanelAction);

		//add listeners for the cutscene panel
		uiManager.GetComponent<CutsceneManager>().OnOpenCutsceneDisplayPanel.AddListener(enablePauseFadePanelAction);
		uiManager.GetComponent<CutsceneManager>().OnCloseCutsceneDisplayPanel.AddListener(disablePauseFadePanelAction);

		//add listener for opening the exit game prompt
		uiManager.GetComponent<ExitGamePrompt>().exitGameButton.onClick.AddListener(enablePauseFadePanelAction);

		//add listener for answering the exit game prompt
		uiManager.GetComponent<ExitGamePrompt>().OnExitGameYesClicked.AddListener(disablePauseFadePanelAction);
		uiManager.GetComponent<ExitGamePrompt>().OnExitGameCancelClicked.AddListener(disablePauseFadePanelAction);

		//add listeners for the settings window open button
		uiManager.GetComponent<Settings>().settingsMenuButton.onClick.AddListener(EnablePauseFadePanel);

		//add listener for settings window exit buttons
		uiManager.GetComponent<Settings>().exitButton.onClick.AddListener(DisablePauseFadePanel);
		uiManager.GetComponent<Settings>().acceptButton.onClick.AddListener(DisablePauseFadePanel);
						
	}

	//this function turns on the pauseFadePanel
	private void EnablePauseFadePanel(){

		//Debug.Log ("Enable Pause Fade Panel");

		pauseFadePanel.SetActive (true);

	}

	//this function turns off the pauseFadePanel
	private void DisablePauseFadePanel(){

		//Debug.Log ("Disable Pause Fade Panel");

		pauseFadePanel.SetActive (false);

	}

	//this function handles OnDestroy
	private void OnDestroy(){

		RemoveAllListeners ();

	}

	//this function removes all listeners
	private void RemoveAllListeners(){

		if (openStatusPanelButton != null) {
			
			//remove listeners for the statusPanel buttons
			openStatusPanelButton.onClick.RemoveListener (EnablePauseFadePanel);

		}

		if (closeStatusPanelButton != null) {
			
			closeStatusPanelButton.onClick.RemoveListener (DisablePauseFadePanel);

		}

		if (openPurchaseItemsPanelButton != null) {
			
			//remove listeners for the purchaseItems buttons
			openPurchaseItemsPanelButton.onClick.RemoveListener (EnablePauseFadePanel);

		}

		if (closePurchaseItemsPanelButton != null) {
			
			closePurchaseItemsPanelButton.onClick.RemoveListener (DisablePauseFadePanel);

		}

		if (purchaseItemsButton != null) {
			
			purchaseItemsButton.onClick.RemoveListener (DisablePauseFadePanel);

		}

		if (openPurchaseShipPanelButton != null) {
			
			//remove listeners for the purchase ship buttons
			openPurchaseShipPanelButton.onClick.RemoveListener (EnablePauseFadePanel);

		}

		if (cancelPurchaseShipPanelButton != null) {
			
			cancelPurchaseShipPanelButton.onClick.RemoveListener (DisablePauseFadePanel);

		}

		if (mouseManager != null) {
			
			//remove listener for picking location of new unit
			mouseManager.OnPlacedNewUnit.RemoveListener (hexEnablePauseFadePanelAction);

		}

		if (uiManager != null) {
			
			//remove listeners for either purchasing or canceling from the name new unit panel
			uiManager.GetComponent<NameNewShip> ().OnCanceledPurchase.RemoveListener (disablePauseFadePanelAction);
			uiManager.GetComponent<NameNewShip> ().OnPurchasedNewShip.RemoveListener (newUnitDataDisablePauseFadePanelAction);

			//remove listeners for flare use panel
			uiManager.GetComponent<FlareManager> ().OnShowFlarePanel.RemoveListener (enablePauseFadePanelAction);
			uiManager.GetComponent<FlareManager> ().OnUseFlaresCancel.RemoveListener (flareDataDisablePauseFadePanelAction);
			uiManager.GetComponent<FlareManager> ().OnUseFlaresYes.RemoveListener (flareDataDisablePauseFadePanelAction);

			//remove listeners for FileSaveWindow buttons
			if (uiManager.GetComponent<FileSaveWindow> ().closeFileSaveWindowButton != null) {
				
				uiManager.GetComponent<FileSaveWindow> ().closeFileSaveWindowButton.onClick.RemoveListener (disablePauseFadePanelAction);

			}

			if (uiManager.GetComponent<FileSaveWindow> ().fileSaveCancelButton != null) {
				
				uiManager.GetComponent<FileSaveWindow> ().fileSaveCancelButton.onClick.RemoveListener (disablePauseFadePanelAction);

			}

			uiManager.GetComponent<FileSaveWindow>().OnFileSaveYesClickedExistingFile.RemoveListener(stringDisablePauseFadePanelAction);
			uiManager.GetComponent<FileSaveWindow>().OnFileSaveYesClickedNewFile.RemoveListener(stringDisablePauseFadePanelAction);
			uiManager.GetComponent<FileSaveWindow> ().OnOpenFileSaveWindow.RemoveListener (enablePauseFadePanelAction);

			//remove listeners for the overwrite prompt buttons
			uiManager.GetComponent<FileOverwritePrompt>().OnFileSaveCancelClicked.RemoveListener(enablePauseFadePanelAction);

			//remove listeners for the FileLoadWindow buttons
			if (uiManager.GetComponent<FileLoadWindow> ().closeFileLoadWindowButton != null) {
				
				uiManager.GetComponent<FileLoadWindow> ().closeFileLoadWindowButton.onClick.RemoveListener (disablePauseFadePanelAction);

			}

			if (uiManager.GetComponent<FileLoadWindow> ().fileLoadCancelButton != null) {
				
				uiManager.GetComponent<FileLoadWindow> ().fileLoadCancelButton.onClick.RemoveListener (disablePauseFadePanelAction);

			}

			uiManager.GetComponent<FileLoadWindow>().OnFileLoadYesClicked.RemoveListener(stringDisablePauseFadePanelAction);
			uiManager.GetComponent<FileLoadWindow> ().OnOpenFileLoadWindow.RemoveListener (enablePauseFadePanelAction);

			//remove listeners for the delete prompt buttons
			uiManager.GetComponent<FileDeletePrompt>().OnFileDeleteCancelClicked.RemoveListener(enablePauseFadePanelAction);

			uiManager.GetComponent<RenameShip> ().OnRenameUnit.RemoveListener (renamedUnitDisablePauseFadePanelAction);
			uiManager.GetComponent<RenameShip> ().OnRenameCancel.RemoveListener (actionModeDisablePauseFadePanelAction);

			//remove listeners for the cutscene panel
			uiManager.GetComponent<CutsceneManager>().OnOpenCutsceneDisplayPanel.RemoveListener(enablePauseFadePanelAction);
			uiManager.GetComponent<CutsceneManager>().OnCloseCutsceneDisplayPanel.RemoveListener(disablePauseFadePanelAction);

			if (uiManager.GetComponent<ExitGamePrompt> ().exitGameButton != null) {
				
				//remove listener for opening the exit game prompt
				uiManager.GetComponent<ExitGamePrompt> ().exitGameButton.onClick.RemoveListener (enablePauseFadePanelAction);

			}

			//remove listener for answering the exit game prompt
			uiManager.GetComponent<ExitGamePrompt>().OnExitGameYesClicked.RemoveListener(disablePauseFadePanelAction);
			uiManager.GetComponent<ExitGamePrompt>().OnExitGameCancelClicked.RemoveListener(disablePauseFadePanelAction);

		}

		if (renameShipMenuButton != null) {
			
			//remove listeners for the rename ship buttons
			renameShipMenuButton.onClick.RemoveListener (EnablePauseFadePanel);

		}

		if (uiManager.GetComponent<Settings> ().settingsMenuButton != null) {

			//remove listeners for the settings window open button
			uiManager.GetComponent<Settings> ().settingsMenuButton.onClick.RemoveListener (EnablePauseFadePanel);

		}

		if (uiManager.GetComponent<Settings> ().exitButton != null) {

			//remove listener for settings window exit buttons
			uiManager.GetComponent<Settings> ().exitButton.onClick.RemoveListener (DisablePauseFadePanel);

		}

		if (uiManager.GetComponent<Settings> ().acceptButton != null) {

			//remove listener for settings window accept buttons
			uiManager.GetComponent<Settings> ().acceptButton.onClick.RemoveListener (DisablePauseFadePanel);

		}
	
	}

}
