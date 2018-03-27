﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class UINavigationMain : MonoBehaviour {

	public static bool blockPointerClickFlag { get; private set;}

	//this will hold the eventSystem in the scene
	public EventSystem eventSystem;

	//uiManager
	private GameObject uiManager;
	private GameManager gameManager;

	//this enum will keep track of the current UI State
	public enum UIState{

		Selection,
		MoveMenu,
		MainMenu,
		NewLocalGame,
		LoadLocalGame,
		FileDeletePrompt,
		Settings,
		ExitGamePrompt,

	}

	//this bool will wait a frame when needed
	private int delayLoadFilesWindowCount = 0;

	//this keeps track of what the current UI state is
	private UIState currentUIState;
	public UIState CurrentUIState {

		get {

			return currentUIState;

		}

		private set {

			if (value == currentUIState) {

				return;

			} else {

				currentUIState = value;

				//invoke the event
				OnUIStateChange.Invoke();

			}

		}


	}

	//these public arrays are groups of selectables arrays that make up each UI state
	private Selectable[][] SelectionGroup;
	private Selectable[][] MoveMenuGroup;

	private Selectable[][] MainMenuGroup;
	private Selectable[][] NewLocalGameGroup;
	private Selectable[][] LoadLocalGameGroup;
	private Selectable[][] FileDeletePromptGroup;
	private Selectable[][] SettingsGroup;
	private Selectable[][] ExitGamePromptGroup;

	//these public arrays will hold the different selectables arrays
	public Selectable[]	ActionMenuButtons;
	public Selectable[] ChatInputField;
	public Selectable[]	FileMenuButtons;
	public Selectable[] NextUnitButtons;
	public Selectable[] StatusButton;

	public Selectable[] MoveMenuButtons;

	public Selectable[] MainMenuButtons;

	public Selectable[] NewLocalGameTeams;
	public Selectable[] NewLocalGamePlanets;
	public Selectable[] NewLocalGameGreenPlayer;
	public Selectable[] NewLocalGameRedPlayer;
	public Selectable[] NewLocalGamePurplePlayer;
	public Selectable[] NewLocalGameBluePlayer;
	public Selectable[] NewLocalGameButtonsRow;

	public Selectable[] LoadLocalGameFiles;
	public Selectable[] LoadLocalGameButtonsRow;

	public Selectable[] FileDeleteButtons;

	public Selectable[] SettingsResolutionDropdown;
	public Selectable[] SettingsResolutionApply;
	public Selectable[] SettingsFullScreenToggle;
	public Selectable[] SettingsMouseZoomInversion;
	public Selectable[] SettingsZoomSensitivity;
	public Selectable[] SettingsScrollSensitivity;
	public Selectable[] SettingsMusicVolume;
	public Selectable[] SettingsSfxVolume;
	public Selectable[] SettingsButtonsRow;

	public Selectable[] ExitGameButtons;

	//these hold the sliders so I can reference them
	public Slider ZoomSlider;
	public Slider ScrollSlider;
	public Slider MusicVolumeSlider;
	public Slider SfxVolumeSlider;

	//this array will hold the selectables which are currently being navigated
	private Selectable[][] currentSelectablesGroup;

	private Selectable[] currentSelectables;
	public Selectable[] CurrentSelectables{

		get {

			return currentSelectables;

		}

		private set {

			if (value == currentSelectables) {

				return;

			} else {

				currentSelectables = value;

				//invoke the event
				OnSelectablesChange.Invoke();

			}

		}

	}

	//this int holds the index of the current selection within the currentSelectables array
	private int currentSelectionGroupIndex;
	private int currentSelectionIndex;

	//these bools determine whether vertical and horizontal key inputs will cycle through the selectables
	private bool horizontalCycling = false;
	private bool verticalCycling = false;

	//this bool determines whether the selectables wrap around
	private bool selectablesWrap = false;

	//this flag is for whether the cancel button should be ignored for exiting menus
	private bool ignoreEscape = false;

	//event to announce UIState change
	public UnityEvent OnUIStateChange = new UnityEvent();
	public UnityEvent OnSelectablesChange = new UnityEvent();


	//unityActions
	private UnityAction<Player> NewTurnSetInitialSelectablesAction;
	private UnityAction<bool> MoveToggleSetUIStateAction;
	private UnityAction OpenNewLocalGameWindowAction;
	private UnityAction<Selectable> SelectableSetSelectionGroupsAction;
	private UnityAction ReturnToMainMenuAction;
	private UnityAction OpenLoadLocalGameWindowAction;
	private UnityAction SelectableSetNavigationRulesAction;
	private UnityAction<string> OpenFileDeletePromptAction;
	private UnityAction<string> StringReturnToFileLoadWindowAction;
	private UnityAction OpenSettingsWindowAction;
	private UnityAction OpenExitGamePromptAction;
	private UnityAction<string> InputFieldEndEditIgnoreEscapeAction;


	// Use this for initialization
	public void Init () {

		//get the uiManager
		uiManager = GameObject.FindGameObjectWithTag("UIManager");
		gameManager = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameManager> ();

		//set the actions
		SetUnityActions ();

		//add event listeners
		AddEventListeners();

		//defome the selectable groups
		DefineSelectablesGroups();

		//set the initial UIState
		CurrentUIState = UIState.Selection;

		//invoke the event to start, since it won't fire because the enum defaults to the Selection
		OnUIStateChange.Invoke();

	}

	// Update is called once per frame
	private void Update () {

		//check if we need to wait a frame for the load local game file list to destroy the old list and repopulate a new list
		if (delayLoadFilesWindowCount > 0) {

			delayLoadFilesWindowCount--;

			if (delayLoadFilesWindowCount == 0) {

				CurrentUIState = UIState.LoadLocalGame;
				return;

			}

		}

		//check if the down arrow is being pressed
		if (Input.GetKeyDown (KeyCode.DownArrow)) {

			//check if the vertical cycling is on
			if (verticalCycling == true) {

				//if the vertical cycling is on, advance the selection
				AdvanceSelectable(false);

				if (CurrentUIState == UIState.LoadLocalGame && CurrentSelectables == LoadLocalGameFiles) {

					//here we are selecting a lower file in the scroll rect
					FitCurrentSelectableIntoScrollRect ();

				}

			} else if(CurrentUIState == UIState.NewLocalGame && CurrentSelectables == NewLocalGamePlanets){

				uiManager.GetComponent<ConfigureLocalGameWindow> ().decreasePlanetsButton.onClick.Invoke();


			} else if(CurrentUIState == UIState.Settings && CurrentSelectables == SettingsResolutionDropdown){

				//adjust the dropdown value
				AdjustDropdownValueDown(SettingsResolutionDropdown[0].GetComponent<TMP_Dropdown>());

			} else if(CurrentUIState == UIState.Settings && CurrentSelectables == SettingsZoomSensitivity){

				//adjust the dropdown value
				AdjustSliderValueDown(ZoomSlider);

			} else if(CurrentUIState == UIState.Settings && CurrentSelectables == SettingsScrollSensitivity){

				//adjust the dropdown value
				AdjustSliderValueDown(ScrollSlider);

			} else if(CurrentUIState == UIState.Settings && CurrentSelectables == SettingsMusicVolume){

				//adjust the dropdown value
				AdjustSliderValueDown(MusicVolumeSlider);

			} else if(CurrentUIState == UIState.Settings && CurrentSelectables == SettingsSfxVolume){

				//adjust the dropdown value
				AdjustSliderValueDown(SfxVolumeSlider);

			} 

		}

		//check if the up arrow is being pressed
		if (Input.GetKeyDown (KeyCode.UpArrow)) {

			//check if the vertical cycling is on
			if (verticalCycling == true) {

				//if the vertical cycling is on, advance the selection
				AdvanceSelectable(true);

				if (CurrentUIState == UIState.LoadLocalGame && CurrentSelectables == LoadLocalGameFiles) {

					//here we are selecting a lower file in the scroll rect

					FitCurrentSelectableIntoScrollRect ();

				}

			}else if(CurrentUIState == UIState.NewLocalGame && CurrentSelectables == NewLocalGamePlanets){

				uiManager.GetComponent<ConfigureLocalGameWindow> ().increasePlanetsButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.Settings && CurrentSelectables == SettingsResolutionDropdown){

				//adjust the dropdown value
				AdjustDropdownValueUp(SettingsResolutionDropdown[0].GetComponent<TMP_Dropdown>());

			} else if(CurrentUIState == UIState.Settings && CurrentSelectables == SettingsZoomSensitivity){

				//adjust the dropdown value
				AdjustSliderValueUp(ZoomSlider);

			} else if(CurrentUIState == UIState.Settings && CurrentSelectables == SettingsScrollSensitivity){

				//adjust the dropdown value
				AdjustSliderValueUp(ScrollSlider);

			} else if(CurrentUIState == UIState.Settings && CurrentSelectables == SettingsMusicVolume){

				//adjust the dropdown value
				AdjustSliderValueUp(MusicVolumeSlider);

			} else if(CurrentUIState == UIState.Settings && CurrentSelectables == SettingsSfxVolume){

				//adjust the dropdown value
				AdjustSliderValueUp(SfxVolumeSlider);

			} 

		}

		//check if the right arrow is being pressed
		if (Input.GetKeyDown (KeyCode.RightArrow)) {

			//check if the horizontal cycling is on
			if (horizontalCycling == true) {

				//if the horizontal cycling is on, advance the selection
				AdvanceSelectable (false);

			} else if(CurrentUIState == UIState.Settings && CurrentSelectables == SettingsZoomSensitivity){

				//adjust the dropdown value
				AdjustSliderValueUp(ZoomSlider);

			} else if(CurrentUIState == UIState.Settings && CurrentSelectables == SettingsScrollSensitivity){

				//adjust the dropdown value
				AdjustSliderValueUp(ScrollSlider);

			} else if(CurrentUIState == UIState.Settings && CurrentSelectables == SettingsMusicVolume){

				//adjust the dropdown value
				AdjustSliderValueUp(MusicVolumeSlider);

			} else if(CurrentUIState == UIState.Settings && CurrentSelectables == SettingsSfxVolume){

				//adjust the dropdown value
				AdjustSliderValueUp(SfxVolumeSlider);

			} 

		}

		//check if the left arrow is being pressed
		if (Input.GetKeyDown (KeyCode.LeftArrow)) {

			//check if the horizontal cycling is on
			if (horizontalCycling == true) {

				//if the horizontal cycling is on, advance the selection
				AdvanceSelectable(true);

			} else if(CurrentUIState == UIState.Settings && CurrentSelectables == SettingsZoomSensitivity){

				//adjust the dropdown value
				AdjustSliderValueDown(ZoomSlider);

			} else if(CurrentUIState == UIState.Settings && CurrentSelectables == SettingsScrollSensitivity){

				//adjust the dropdown value
				AdjustSliderValueDown(ScrollSlider);

			} else if(CurrentUIState == UIState.Settings && CurrentSelectables == SettingsMusicVolume){

				//adjust the dropdown value
				AdjustSliderValueDown(MusicVolumeSlider);

			} else if(CurrentUIState == UIState.Settings && CurrentSelectables == SettingsSfxVolume){

				//adjust the dropdown value
				AdjustSliderValueDown(SfxVolumeSlider);

			} 

		}

		//check if the shift is being pressed
		if (Input.GetKeyDown (KeyCode.Tab)) {

			//check if we are also holdin down a shift key
			//if we are holding a shift key, we want tab to cycle backwards
			if (Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift)) {

				//check if we are in the load files - if we are here, we don't want tab to cycle through the files
				if (CurrentSelectables == LoadLocalGameFiles) {

					//advance to the next group
					AdvanceSelectableGroup (true);

				}

				//check if we are at the first interactable element of the selectables array
				else if (currentSelectionIndex == FindFirstInteractableIndex(CurrentSelectables)) {

					//advance to the next group
					AdvanceSelectableGroup (true);

					//check if we are in the load files after advancing groups
					if (CurrentSelectables == LoadLocalGameFiles) {

						//if so, we want to make sure we fit the selectable in the scrollrect
						FitCurrentSelectableIntoScrollRect ();

					}

				} else {

					//the else condition is that we are not at the end of the selectables array
					//in this case, we just want tab to advance to the next element in the current group
					AdvanceSelectable (true);

				}

			} else {

				//the else is that a shift is not held down, and we should cycle forward

				//check if we are in the load files - if we are here, we don't want tab to cycle through the files
				if (CurrentSelectables == LoadLocalGameFiles) {

					//advance to the next group
					AdvanceSelectableGroup (false);

				}

				//check if we are at the last interactable element of the selectables array
				else if (currentSelectionIndex == FindLastInteractableIndex	(CurrentSelectables)) {

					//advance to the next group
					AdvanceSelectableGroup (false);

					//check if we are in the load files after advancing groups
					if (CurrentSelectables == LoadLocalGameFiles) {

						//if so, we want to make sure we fit the selectable in the scrollrect
						FitCurrentSelectableIntoScrollRect ();

					}

				} else {

					//the else condition is that we are not at the end of the selectables array
					//in this case, we just want tab to advance to the next element in the current group
					AdvanceSelectable (false);

				}

			}

		}

		//check if the escape key is being pressed
		if (Input.GetKeyDown (KeyCode.Escape)) {

			//check if we are in the new game window
			if (CurrentUIState == UIState.NewLocalGame && ignoreEscape == false) {

				//cancel out of the menu
				uiManager.GetComponent<ConfigureLocalGameWindow> ().cancelButton.onClick.Invoke ();

			} else if (CurrentUIState == UIState.LoadLocalGame) {

				//cancel out of the menu
				uiManager.GetComponent<FileLoadWindow> ().closeFileLoadWindowButton.onClick.Invoke ();

			} else if (CurrentUIState == UIState.FileDeletePrompt) {

				//cancel out of the menu
				uiManager.GetComponent<FileDeletePrompt> ().fileDeleteCancelButton.onClick.Invoke ();

			} else if (CurrentUIState == UIState.Settings) {

				//cancel out of the menu
				uiManager.GetComponent<Settings> ().exitButton.onClick.Invoke ();

			} else if (CurrentUIState == UIState.ExitGamePrompt) {

				//cancel out of the menu
				uiManager.GetComponent<ExitGamePrompt> ().exitGameCancelButton.onClick.Invoke ();

			} else if (CurrentUIState == UIState.MainMenu) {

				//cancel out of the menu and launch the exit prompt
				uiManager.GetComponent<ExitGamePrompt> ().exitGameButton.onClick.Invoke ();

			}

		}

		//at the end of a frame, we can stop ignoring escape
		//if we still need to ignore it, the input field events will trigger again
		ignoreEscape = false;
		/*
		//check if the current selected object is what it is supposed to be
		if (EventSystem.current.currentSelectedGameObject != CurrentSelectables [currentSelectionIndex]) {

			//set the current selected object to what it's supposed to be
			EventSystem.current.SetSelectedGameObject (CurrentSelectables [currentSelectionIndex].gameObject);

		}

		*/

		blockPointerClickFlag = false;

	}

	//this function will make sure that the selectable stays within view of a scrolling menu
	private void FitCurrentSelectableIntoScrollRect(){

		//we need to check if we need to lower the scrollbar value so the new selection stays on screen

		//get the current selectable local position
		float currentSelectableLocalY = CurrentSelectables[currentSelectionIndex].GetComponent<RectTransform>().localPosition.y;

		float currentSelectableLocalYTop = CurrentSelectables [currentSelectionIndex].GetComponent<RectTransform> ().localPosition.y +
			CurrentSelectables [currentSelectionIndex].GetComponent<RectTransform> ().rect.height;

		//get the file content panel local position
		float contentPanelLocalY = CurrentSelectables[currentSelectionIndex].transform.parent.GetComponent<RectTransform>().localPosition.y;

		//get the file content panel height
		float contentPanelHeight = CurrentSelectables[currentSelectionIndex].transform.parent.GetComponent<RectTransform>().rect.height;

		//get the viewport height
		float viewportHeight = CurrentSelectables[currentSelectionIndex].transform.parent.parent.GetComponent<RectTransform>().rect.height;

		//get the viewport offset
		float viewportOffset = contentPanelLocalY - viewportHeight / 2;

		//check if the current position of the selected item is greater than the height plus current offset of the panel
		if (-currentSelectableLocalY > viewportHeight + viewportOffset) {

			//adjust the content panel localY to fix the current selectable on screen
			CurrentSelectables [currentSelectionIndex].transform.parent.parent.GetComponent<ScrollRect> ().verticalNormalizedPosition = 
				1- (-currentSelectableLocalY - viewportHeight) / (contentPanelHeight - viewportHeight);

		}

		//check if the current position of the selected item is greater than the height plus current offset of the panel
		if (-currentSelectableLocalYTop < viewportOffset) {

			//adjust the content panel localY to fix the current selectable on screen
			CurrentSelectables [currentSelectionIndex].transform.parent.parent.GetComponent<ScrollRect> ().verticalNormalizedPosition = 
				1- (-currentSelectableLocalYTop) / (contentPanelHeight - viewportHeight);

		}

	}

	//this function sets the UnityActions
	private void SetUnityActions(){

		NewTurnSetInitialSelectablesAction = (player) => {SetInitialCurrentSelectables ();};

		MoveToggleSetUIStateAction = (toggleState) => {

			if (toggleState == true) {

				CurrentUIState = UIState.MoveMenu;

			} else {

				CurrentUIState = UIState.Selection;

			}
			;

		};

		OpenNewLocalGameWindowAction = () => {CurrentUIState = UIState.NewLocalGame;};

		SelectableSetSelectionGroupsAction = (selectable) => {SetSelectionIndexFromPointerClick (selectable);};

		ReturnToMainMenuAction = () => {CurrentUIState = UIState.MainMenu;};

		OpenLoadLocalGameWindowAction = () => {CurrentUIState = UIState.LoadLocalGame;};

		SelectableSetNavigationRulesAction = () => {SetNavigationRulesForSelectables();};

		OpenFileDeletePromptAction = (fileName) => {CurrentUIState = UIState.FileDeletePrompt;};

		StringReturnToFileLoadWindowAction = (fileName) => {delayLoadFilesWindowCount = 2;};

		OpenSettingsWindowAction = () => {CurrentUIState = UIState.Settings;}; 

		OpenExitGamePromptAction = () => {CurrentUIState = UIState.ExitGamePrompt;};

		InputFieldEndEditIgnoreEscapeAction = (eventString) => {ignoreEscape = true;};

	}

	//this function adds event listeners
	private void AddEventListeners(){

		//add listener for UIState change
		OnUIStateChange.AddListener(SetInitialCurrentSelectables);

		//add listener for a pointer click selection
		UISelection.OnSetSelectedGameObject.AddListener(SelectableSetSelectionGroupsAction);

		//add listener for move toggle
		uiManager.GetComponent<MoveToggle>().moveToggle.onValueChanged.AddListener(MoveToggleSetUIStateAction);

		//add listener for load local game
		uiManager.GetComponent<FileLoadWindow>().OnOpenFileLoadWindow.AddListener(OpenLoadLocalGameWindowAction);

		//add listener for setting selectables
		OnSelectablesChange.AddListener(SelectableSetNavigationRulesAction);

		//add listeners for exiting the file load window back to the main menu
		uiManager.GetComponent<FileLoadWindow>().closeFileLoadWindowButton.onClick.AddListener(ReturnToMainMenuAction);
		uiManager.GetComponent<FileLoadWindow>().fileLoadCancelButton.onClick.AddListener(ReturnToMainMenuAction);

		//add listener for entering the file delete prompt
		uiManager.GetComponent<FileLoadWindow>().OnFileDeleteYesClicked.AddListener(OpenFileDeletePromptAction);

		//add listeners for exiting the file delete prompt back to the load file window
		uiManager.GetComponent<FileDeletePrompt>().OnFileDeleteYesClicked.AddListener(StringReturnToFileLoadWindowAction);
		uiManager.GetComponent<FileDeletePrompt>().OnFileDeleteCancelClicked.AddListener(OpenLoadLocalGameWindowAction);

		//add listeners for entering the settings menu
		uiManager.GetComponent<Settings>().settingsMenuButton.onClick.AddListener(OpenSettingsWindowAction);

		//add listeners for exiting the settings menu
		uiManager.GetComponent<Settings>().acceptButton.onClick.AddListener(ReturnToMainMenuAction);
		uiManager.GetComponent<Settings>().exitButton.onClick.AddListener(ReturnToMainMenuAction);

		//add listener for entering the exit game prompt
		uiManager.GetComponent<ExitGamePrompt>().exitGameButton.onClick.AddListener(OpenExitGamePromptAction);

		//add listener for exiting the exit game prompt
		uiManager.GetComponent<ExitGamePrompt>().OnExitGameYesClicked.AddListener(ReturnToMainMenuAction);
		uiManager.GetComponent<ExitGamePrompt>().OnExitGameCancelClicked.AddListener(ReturnToMainMenuAction);

		//add listener for new or loaded turn
		gameManager.OnNewTurn.AddListener(NewTurnSetInitialSelectablesAction);
		gameManager.OnLoadedTurn.AddListener (NewTurnSetInitialSelectablesAction);

	}

	//this function defines the selectables groups
	private void DefineSelectablesGroups(){

		SelectionGroup = new Selectable[5][];
		SelectionGroup [0] = ActionMenuButtons;
		SelectionGroup [1] = ChatInputField;
		SelectionGroup [2] = FileMenuButtons;
		SelectionGroup [3] = NextUnitButtons;
		SelectionGroup [4] = StatusButton;

		MoveMenuGroup = new Selectable[6][];
		MoveMenuGroup [0] = MoveMenuButtons;
		MoveMenuGroup [1] = ActionMenuButtons;
		MoveMenuGroup [2] = ChatInputField;
		MoveMenuGroup [3] = FileMenuButtons;
		MoveMenuGroup [4] = NextUnitButtons;
		MoveMenuGroup [5] = StatusButton;

		MainMenuGroup = new Selectable[1][];
		MainMenuGroup [0] = MainMenuButtons;

		NewLocalGameGroup = new Selectable[7][];
		NewLocalGameGroup [0] = NewLocalGameTeams;
		NewLocalGameGroup [1] = NewLocalGamePlanets;
		NewLocalGameGroup [2] = NewLocalGameGreenPlayer;
		NewLocalGameGroup [3] = NewLocalGameRedPlayer;
		NewLocalGameGroup [4] = NewLocalGamePurplePlayer;
		NewLocalGameGroup [5] = NewLocalGameBluePlayer;
		NewLocalGameGroup [6] = NewLocalGameButtonsRow;

		LoadLocalGameGroup = new Selectable[2][];
		LoadLocalGameGroup [0] = LoadLocalGameFiles;
		LoadLocalGameGroup [1] = LoadLocalGameButtonsRow;

		FileDeletePromptGroup = new Selectable[1][];
		FileDeletePromptGroup [0] = FileDeleteButtons;

		SettingsGroup = new Selectable[9][];
		SettingsGroup[0] = SettingsResolutionDropdown;
		SettingsGroup[1] = SettingsResolutionApply;
		SettingsGroup[2] = SettingsFullScreenToggle;
		SettingsGroup[3] = SettingsMouseZoomInversion;
		SettingsGroup[4] = SettingsZoomSensitivity;
		SettingsGroup[5] = SettingsScrollSensitivity;
		SettingsGroup[6] = SettingsMusicVolume;
		SettingsGroup[7] = SettingsSfxVolume;
		SettingsGroup[8] = SettingsButtonsRow;

		ExitGamePromptGroup = new Selectable[1][];
		ExitGamePromptGroup [0] = ExitGameButtons;

		//Debug.Log ("Define Selectables");

	}

	//this function defines the navigation rules for the currentSelectables
	private void SetNavigationRulesForSelectables(){

		//define rules based on what the current selectable is
		if (CurrentSelectables == ActionMenuButtons) {

			horizontalCycling = false;
			verticalCycling = true;
			selectablesWrap = true;

		} else if (CurrentSelectables == ChatInputField) {

			horizontalCycling = false;
			verticalCycling = false;
			selectablesWrap = false;

		} else if (CurrentSelectables == FileMenuButtons) {

			horizontalCycling = true;
			verticalCycling = false;
			selectablesWrap = true;

		} else if (CurrentSelectables == NextUnitButtons) {

			horizontalCycling = true;
			verticalCycling = false;
			selectablesWrap = true;

		} else if (CurrentSelectables == StatusButton) {

			horizontalCycling = false;
			verticalCycling = false;
			selectablesWrap = false;

		} else if (CurrentSelectables == MoveMenuButtons) {

			horizontalCycling = false;
			verticalCycling = true;
			selectablesWrap = false;

		}else if (CurrentSelectables == MainMenuButtons) {

			horizontalCycling = false;
			verticalCycling = true;
			selectablesWrap = true;

		} else if (CurrentSelectables == NewLocalGameTeams) {

			horizontalCycling = true;
			verticalCycling = false;
			selectablesWrap = true;

		} else if (CurrentSelectables == NewLocalGamePlanets){

			horizontalCycling = false;
			verticalCycling = false;
			selectablesWrap = true;

		} else if (CurrentSelectables == NewLocalGameGreenPlayer){

			horizontalCycling = false;
			verticalCycling = false;
			selectablesWrap = true;

		} else if (CurrentSelectables == NewLocalGameRedPlayer){

			horizontalCycling = false;
			verticalCycling = false;
			selectablesWrap = true;

		} else if (CurrentSelectables == NewLocalGamePurplePlayer){

			horizontalCycling = false;
			verticalCycling = false;
			selectablesWrap = true;

		} else if (CurrentSelectables == NewLocalGameBluePlayer){

			horizontalCycling = false;
			verticalCycling = false;
			selectablesWrap = true;

		} else if (CurrentSelectables == NewLocalGameButtonsRow){

			horizontalCycling = true;
			verticalCycling = false;
			selectablesWrap = true;

		} else if (CurrentSelectables == LoadLocalGameFiles) {

			horizontalCycling = false;
			verticalCycling = true;
			selectablesWrap = false;

		} else if (CurrentSelectables == LoadLocalGameButtonsRow){

			horizontalCycling = true;
			verticalCycling = false;
			selectablesWrap = true;

		} else if (CurrentSelectables == FileDeleteButtons){

			horizontalCycling = true;
			verticalCycling = false;
			selectablesWrap = true;

		} else if (CurrentSelectables == SettingsResolutionDropdown){

			horizontalCycling = false;
			verticalCycling = false;
			selectablesWrap = false;

		} else if (CurrentSelectables == SettingsResolutionApply){

			horizontalCycling = false;
			verticalCycling = false;
			selectablesWrap = false;

		} else if (CurrentSelectables == SettingsFullScreenToggle){

			horizontalCycling = false;
			verticalCycling = false;
			selectablesWrap = false;

		} else if (CurrentSelectables == SettingsMouseZoomInversion){

			horizontalCycling = false;
			verticalCycling = false;
			selectablesWrap = false;

		} else if (CurrentSelectables == SettingsZoomSensitivity){

			horizontalCycling = false;
			verticalCycling = false;
			selectablesWrap = false;

		} else if (CurrentSelectables == SettingsScrollSensitivity){

			horizontalCycling = false;
			verticalCycling = false;
			selectablesWrap = false;

		} else if (CurrentSelectables == SettingsMusicVolume){

			horizontalCycling = false;
			verticalCycling = false;
			selectablesWrap = false;

		} else if (CurrentSelectables == SettingsSfxVolume){

			horizontalCycling = false;
			verticalCycling = false;
			selectablesWrap = false;

		} else if (CurrentSelectables == SettingsButtonsRow){

			horizontalCycling = true;
			verticalCycling = false;
			selectablesWrap = true;

		} else if (CurrentSelectables == ExitGameButtons){

			horizontalCycling = true;
			verticalCycling = false;
			selectablesWrap = true;

		} 

	}

	//this function sets the current selectables based on the UI state
	private void SetInitialCurrentSelectables(){

		//local variable to hold the potential index of the selection group which has a valid interactable selectable
		int potentialCurrentSelectionGroupIndex;

		//switch case based on UI state
		switch (CurrentUIState) {

		case UIState.Selection:
			
			//set the current selectables group to match the UI state
			currentSelectablesGroup = SelectionGroup;

			//find the first array in the group that has an interactable selectable
			potentialCurrentSelectionGroupIndex = FindFirstInteractableArrayIndex (currentSelectablesGroup);

			//set the selectable array that contains an interactable
			if (potentialCurrentSelectionGroupIndex != -1) {

				//set the currentSelectionGroupIndex
				currentSelectionGroupIndex = potentialCurrentSelectionGroupIndex;

				//set the currentSelectables based on the index returned
				CurrentSelectables = currentSelectablesGroup[currentSelectionGroupIndex];

			}

			break;

		case UIState.MoveMenu:

			//set the current selectables group to match the UI state
			currentSelectablesGroup = MoveMenuGroup;

			//find the first array in the group that has an interactable selectable
			potentialCurrentSelectionGroupIndex = FindFirstInteractableArrayIndex (currentSelectablesGroup);

			//set the selectable array that contains an interactable
			if (potentialCurrentSelectionGroupIndex != -1) {

				//set the currentSelectionGroupIndex
				currentSelectionGroupIndex = potentialCurrentSelectionGroupIndex;

				//set the currentSelectables based on the index returned
				CurrentSelectables = currentSelectablesGroup[currentSelectionGroupIndex];

			}

			break;

		case UIState.MainMenu:

			//set the current selectables group to match the UI state
			currentSelectablesGroup = MainMenuGroup;

			//find the first array in the group that has an interactable selectable
			potentialCurrentSelectionGroupIndex = FindFirstInteractableArrayIndex (currentSelectablesGroup);

			//set the selectable array that contains an interactable
			if (potentialCurrentSelectionGroupIndex != -1) {

				//set the currentSelectionGroupIndex
				currentSelectionGroupIndex = potentialCurrentSelectionGroupIndex;

				//set the currentSelectables based on the index returned
				CurrentSelectables = currentSelectablesGroup[currentSelectionGroupIndex];

			}

			break;

		case UIState.NewLocalGame:

			//set the current selectables group to match the UI state
			currentSelectablesGroup = NewLocalGameGroup;

			//find the first array in the group that has an interactable selectable
			potentialCurrentSelectionGroupIndex = FindFirstInteractableArrayIndex (currentSelectablesGroup);

			//set the selectable array that contains an interactable
			if (potentialCurrentSelectionGroupIndex != -1) {

				//set the currentSelectionGroupIndex
				currentSelectionGroupIndex = potentialCurrentSelectionGroupIndex;

				//set the currentSelectables based on the index returned
				CurrentSelectables = currentSelectablesGroup[currentSelectionGroupIndex];

			}

			break;

		case UIState.LoadLocalGame:

			//need to get the game files after the window opens
			//set the size of the LoadLocalGameFiles array
			LoadLocalGameFiles = new Selectable[uiManager.GetComponent<FileLoadWindow> ().fileList.transform.childCount];

			//Debug.Log ("LoadLocalGameFiles.Length = " + LoadLocalGameFiles.Length);

			//loop through the filelist
			for (int i = 0; i < uiManager.GetComponent<FileLoadWindow> ().fileList.transform.childCount; i++) {

				LoadLocalGameFiles [i] = uiManager.GetComponent<FileLoadWindow> ().fileList.transform.GetChild (i).GetComponent<Selectable> ();

			}

			//redefine Selectable Groups since we have dynamically changed the LoadLocalGameFiles
			DefineSelectablesGroups();

			//Debug.Log ("LoadLocalGameFiles length = " + LoadLocalGameFiles.Length);

			//set the current selectables group to match the UI state
			currentSelectablesGroup = LoadLocalGameGroup;

			//find the first array in the group that has an interactable selectable
			potentialCurrentSelectionGroupIndex = FindFirstInteractableArrayIndex (currentSelectablesGroup);

			//Debug.Log ("potentialCurrentSelectionGroupIndex = " + potentialCurrentSelectionGroupIndex);

			//set the selectable array that contains an interactable
			if (potentialCurrentSelectionGroupIndex != -1) {

				//set the currentSelectionGroupIndex
				currentSelectionGroupIndex = potentialCurrentSelectionGroupIndex;

				//set the currentSelectables based on the index returned
				CurrentSelectables = currentSelectablesGroup[currentSelectionGroupIndex];

				//Debug.Log ("current selectables length = " + CurrentSelectables.Length);

				//set the scrollbar to the top
				CurrentSelectables [currentSelectionIndex].transform.parent.parent.GetComponent<ScrollRect> ().verticalNormalizedPosition = 1.0f;

			}

			break;

		case UIState.FileDeletePrompt:

			//clear the loadLocalGameFiles
			LoadLocalGameFiles = null;

			//set the current selectables group to match the UI state
			currentSelectablesGroup = FileDeletePromptGroup;

			//find the first array in the group that has an interactable selectable
			potentialCurrentSelectionGroupIndex = FindFirstInteractableArrayIndex (currentSelectablesGroup);

			//set the selectable array that contains an interactable
			if (potentialCurrentSelectionGroupIndex != -1) {

				//set the currentSelectionGroupIndex
				currentSelectionGroupIndex = potentialCurrentSelectionGroupIndex;

				//set the currentSelectables based on the index returned
				CurrentSelectables = currentSelectablesGroup[currentSelectionGroupIndex];

			}

			break;

		case UIState.Settings:

			//set the current selectables group to match the UI state
			currentSelectablesGroup = SettingsGroup;

			//find the first array in the group that has an interactable selectable
			potentialCurrentSelectionGroupIndex = FindFirstInteractableArrayIndex (currentSelectablesGroup);

			//set the selectable array that contains an interactable
			if (potentialCurrentSelectionGroupIndex != -1) {

				//set the currentSelectionGroupIndex
				currentSelectionGroupIndex = potentialCurrentSelectionGroupIndex;

				//set the currentSelectables based on the index returned
				CurrentSelectables = currentSelectablesGroup[currentSelectionGroupIndex];

			}

			break;

		case UIState.ExitGamePrompt:

			//set the current selectables group to match the UI state
			currentSelectablesGroup = ExitGamePromptGroup;

			//find the first array in the group that has an interactable selectable
			potentialCurrentSelectionGroupIndex = FindFirstInteractableArrayIndex (currentSelectablesGroup);

			//set the selectable array that contains an interactable
			if (potentialCurrentSelectionGroupIndex != -1) {

				//set the currentSelectionGroupIndex
				currentSelectionGroupIndex = potentialCurrentSelectionGroupIndex;

				//set the currentSelectables based on the index returned
				CurrentSelectables = currentSelectablesGroup[currentSelectionGroupIndex];

			}

			break;

		default:

			break;

		}

		//check if there is an interactable selectable in currentSelectables
		int potentialCurrentSelectionIndex = FindFirstInteractableIndex (CurrentSelectables);

		//the Find function will return -1 if it can't find a vaild selectable in the array
		if (potentialCurrentSelectionIndex != -1) {

			//set the Selected object
			eventSystem.SetSelectedGameObject (CurrentSelectables [potentialCurrentSelectionIndex].gameObject);

			//Debug.Log (eventSystem.currentSelectedGameObject.name);

			//store the index of the current selection
			currentSelectionIndex = potentialCurrentSelectionIndex;

		}

	}

	//this function finds the index of the first selectable array within a group which contains a valid interactable selectable
	private int FindFirstInteractableArrayIndex(Selectable[][] selectableGroup){

		int returnInt;

		//Debug.Log ("SelectableGroup.Length = " + selectableGroup.Length);

		for (int i = 0; i < selectableGroup.Length; i++) {

			//Debug.Log ("selectableGroup[" + i + "]FirstInteractableIndex = " + FindFirstInteractableIndex(selectableGroup [i]));


			//check if the first option has an interactable selectable
			if (FindFirstInteractableIndex(selectableGroup [i]) != -1) {

				//return the return int
				returnInt = i;
				return returnInt;

			}

		}

		//if we get here, we iterated through the entire array without returning a valid index
		return -1;

	}

	//this function finds the index of the first selectable in a selectables array that is interactable
	private int FindFirstInteractableIndex(Selectable[] selectables){

		int returnInt;

		//Debug.Log ("selectables.Length = " + selectables.Length);

		for (int i = 0; i < selectables.Length; i++) {

			//Debug.Log ("selectables[" + i + "] IsInteractable = " + selectables [i].IsInteractable ().ToString());

			//check if the first option is interactable
			if (selectables [i].IsInteractable() == true) {

				//return the return int
				returnInt = i;
				return returnInt;

			}

		}

		//if we get here, we iterated through the entire array without returning a valid index
		return -1;

	}

	//this function finds the index of the last selectable in a selectables array that is interactable
	private int FindLastInteractableIndex(Selectable[] selectables){

		int returnInt;

		for (int i = selectables.Length - 1; i >= 0; i--) {

			//check if the last option is interactable
			if (selectables [i].IsInteractable() == true) {

				//return the return int
				returnInt = i;
				return returnInt;

			}

		}

		//if we get here, we iterated through the entire array without returning a valid index
		return -1;

	}

	//this function advances the selectable group within the array
	private void AdvanceSelectableGroup(bool reverseDirection){

		//we can have special handling for when there is only one element in the group
		//if we are advancing the group with only one group, we can just quickly set the 
		//current selectable to the first or last element, depending on direction
		if (currentSelectablesGroup.Length == 1) {

			AdvanceSelectable (reverseDirection);

			//return from the function
			return;

		}


		//variable to hold the potential index of the next selectable
		int potentialGroupIndex;
		int potentialIndex;

		//check if we are reversing selection order or not
		if (reverseDirection == true) {

			potentialGroupIndex = currentSelectionGroupIndex - 1;

		} else {

			potentialGroupIndex = currentSelectionGroupIndex + 1;

		}

		//Debug.Log ("potentialGroupIndex = " + potentialGroupIndex);

		//loop through all possible selectable groups
		for (int i = 0; i < currentSelectablesGroup.Length; i++) {

			//check if we are reversing selection order or not
			if (reverseDirection == true) {

				//check if the next index is greater than or equal to  0
				if (potentialGroupIndex  >= 0) {

					//we know the potential index is in the array bounds
					//check if the potential index is interactable
					//we want the last index since we are going backwards
					potentialIndex = FindLastInteractableIndex(currentSelectablesGroup [potentialGroupIndex]);
					if (potentialIndex != -1) {

						//set the currentSelectables
						CurrentSelectables = currentSelectablesGroup[potentialGroupIndex];

						//the potential index is interactable
						//set the currentSelection to the selectable at the potential index
						eventSystem.SetSelectedGameObject (CurrentSelectables [potentialIndex].gameObject);

						//cache the index
						currentSelectionIndex = potentialIndex;

						//cache the group indes
						currentSelectionGroupIndex = potentialGroupIndex;

						//break out of the for loop
						break;

					}

				} else {

					//wrap around is assumed for tabbing through groups
					//if wrapping is enabled, and we are out of bounds, we can set the potential index to the array length
					potentialGroupIndex = currentSelectablesGroup.Length;

				}

				//increment the potential group index
				potentialGroupIndex--;

			} else {

				//check if the next index is less than the length
				if (potentialGroupIndex < currentSelectablesGroup.Length) {

					//we know the potential index is in the array bounds
					//check if the potential index is interactable
					potentialIndex = FindFirstInteractableIndex(currentSelectablesGroup [potentialGroupIndex]);

					//Debug.Log ("potentialIndex = " + potentialIndex);

					if (potentialIndex != -1) {

						//set the currentSelectables
						CurrentSelectables = currentSelectablesGroup[potentialGroupIndex];

						//the potential index is interactable
						//set the currentSelection to the selectable at the potential index
						eventSystem.SetSelectedGameObject (CurrentSelectables [potentialIndex].gameObject);

						//cache the index
						currentSelectionIndex = potentialIndex;

						//cache the group indes
						currentSelectionGroupIndex = potentialGroupIndex;

						//break out of the for loop
						break;

					}

				} else {

					//wrap around is assumed for tabbing through groups
					//if wrapping is enabled, and we are out of bounds, we can set the potential index to zero
					//we want it to be zero after indexing, so it should be -1 for now
					potentialGroupIndex = -1;

				}

				//increment the potential index
				potentialGroupIndex++;

			}

		}

	}

	//this function advances the selectable within the array
	private void AdvanceSelectable(bool reverseDirection){

		//variable to hold the potential index of the next selectable
		int potentialIndex;

		//check if we are reversing selection order or not
		if (reverseDirection == true) {

			potentialIndex = currentSelectionIndex - 1;

		} else {

			potentialIndex = currentSelectionIndex + 1;

		}

		//loop through all possible selectables
		for (int i = 0; i < CurrentSelectables.Length; i++) {

			//check if we are reversing selection order or not
			if (reverseDirection == true) {

				//check if the next index is greater than or equal to  0
				if (potentialIndex  >= 0) {

					//we know the potential index is in the array bounds
					//check if the potential index is interactable
					if (CurrentSelectables [potentialIndex].IsInteractable () == true) {

						//the potential index is interactable
						//set the currentSelection to the selectable at the potential index
						eventSystem.SetSelectedGameObject (CurrentSelectables [potentialIndex].gameObject);

						//cache the index
						currentSelectionIndex = potentialIndex;

						//break out of the for loop
						break;

					}

				} else if (selectablesWrap == true) {

					//if wrapping is enabled, and we are out of bounds, we can set the potential index to the array length
					potentialIndex = CurrentSelectables.Length;

				}

				//increment the potential index
				potentialIndex--;

			} else {

				//check if the next index is less than the length
				if (potentialIndex < CurrentSelectables.Length) {

					//we know the potential index is in the array bounds
					//check if the potential index is interactable
					if (CurrentSelectables [potentialIndex].IsInteractable () == true) {

						//the potential index is interactable
						//set the currentSelection to the selectable at the potential index
						eventSystem.SetSelectedGameObject (CurrentSelectables [potentialIndex].gameObject);

						//cache the index
						currentSelectionIndex = potentialIndex;

						//break out of the for loop
						break;

					}

				} else if (selectablesWrap == true) {

					//if wrapping is enabled, and we are out of bounds, we can set the potential index to zero
					//we want it to be zero after indexing, so it should be -1 for now
					potentialIndex = -1;

				}

				//increment the potential index
				potentialIndex++;

			}

		}

	}

	//this function sets the current selection group index and selection index from a pointer click
	private void SetSelectionIndexFromPointerClick(Selectable selectableClicked){

		//loop through all the selectables arrays within the current group
		for (int i = 0; i < currentSelectablesGroup.Length; i++) {

			//we need to find the selectableClicked within the selectable array
			for (int j = 0; j < currentSelectablesGroup[i].Length; j++) {

				//check if the selectableClicked matches the currentSelectable
				if (currentSelectablesGroup[i] [j] == selectableClicked) {

					//set the currentSelectionGroupIndex
					currentSelectionGroupIndex = i;

					//set the currentSelectables
					CurrentSelectables = currentSelectablesGroup[i];

					//set the currentSelectionIndex
					currentSelectionIndex = j;

					//break out of the for loop
					break;

				}

			}

		}

	}

	//this function adjusts the dropdown selection downward
	private void AdjustDropdownValueDown(TMP_Dropdown dropdown){

		//int to track the current dropdown index
		int dropdownIndex;

		//set the dropdownIndex to the current dropdown value
		dropdownIndex = dropdown.value;

		//check if the next index is within range
		if (dropdownIndex + 1 < dropdown.options.Count) {

			//we have room to increase the index
			dropdownIndex++;

			//set the dropdown value to the new index
			dropdown.value = dropdownIndex;

		}

	}

	//this function adjusts the dropdown selection upward
	private void AdjustDropdownValueUp(TMP_Dropdown dropdown){

		//int to track the current dropdown index
		int dropdownIndex;

		//set the dropdownIndex to the current dropdown value
		dropdownIndex = dropdown.value;

		//check if the next index is within range
		if (dropdownIndex - 1 >= 0) {

			//we have room to decrease the index
			dropdownIndex--;

			//set the dropdown value to the new index
			dropdown.value = dropdownIndex;

		}

	}

	//this function adjusts the slider selection downward
	private void AdjustSliderValueDown(Slider slider){

		//int to track the current slider value
		int sliderValue;

		//set the slider value to the current value
		sliderValue = (int)slider.value;

		//check if the next index is within range
		if (sliderValue - 1 >= slider.minValue) {

			//we have room to decrease the value
			sliderValue--;

			//set the slider value to the new value
			slider.value = sliderValue;

		}

	}

	//this function adjusts the slider selection upward
	private void AdjustSliderValueUp(Slider slider){

		//int to track the current slider value
		int sliderValue;

		//set the slider value to the current value
		sliderValue = (int)slider.value;

		//check if the next index is within range
		if (sliderValue + 1 <= slider.maxValue) {

			//we have room to increase the value
			sliderValue++;

			//set the slider value to the new value
			slider.value = sliderValue;

		}

	}


	//this function handles on destroy
	private void OnDestroy(){

		//remove all event listeners
		RemoveEventListeners();

	}

	//this function removes all event listeners
	private void RemoveEventListeners(){

		//remove listener for UIState change
		OnUIStateChange.RemoveListener(SetInitialCurrentSelectables);

		//remove listener for setting selectables
		OnSelectablesChange.RemoveListener(SelectableSetNavigationRulesAction);

		if (uiManager != null) {

			//remove listener for move toggle
			uiManager.GetComponent<MoveToggle>().moveToggle.onValueChanged.RemoveListener(MoveToggleSetUIStateAction);

			//remove listener for load local game
			uiManager.GetComponent<FileLoadWindow>().OnOpenFileLoadWindow.RemoveListener(OpenLoadLocalGameWindowAction);

			//remove listeners for exiting the file load window back to the main menu
			uiManager.GetComponent<FileLoadWindow>().closeFileLoadWindowButton.onClick.RemoveListener(ReturnToMainMenuAction);
			uiManager.GetComponent<FileLoadWindow>().fileLoadCancelButton.onClick.RemoveListener(ReturnToMainMenuAction);

			//remove listener for entering the file delete prompt
			uiManager.GetComponent<FileLoadWindow>().OnFileDeleteYesClicked.RemoveListener(OpenFileDeletePromptAction);

			//remove listeners for exiting the file delete prompt back to the load file window
			uiManager.GetComponent<FileDeletePrompt>().OnFileDeleteYesClicked.RemoveListener(StringReturnToFileLoadWindowAction);
			uiManager.GetComponent<FileDeletePrompt>().OnFileDeleteCancelClicked.RemoveListener(OpenLoadLocalGameWindowAction);

			//remove listeners for entering the settings menu
			uiManager.GetComponent<Settings>().settingsMenuButton.onClick.RemoveListener(OpenSettingsWindowAction);

			//remove listeners for exiting the settings menu
			uiManager.GetComponent<Settings>().acceptButton.onClick.RemoveListener(ReturnToMainMenuAction);
			uiManager.GetComponent<Settings>().exitButton.onClick.RemoveListener(ReturnToMainMenuAction);

			//remove listener for entering the exit game prompt
			uiManager.GetComponent<ExitGamePrompt>().exitGameButton.onClick.RemoveListener(OpenExitGamePromptAction);

			//remove listener for exiting the exit game prompt
			uiManager.GetComponent<ExitGamePrompt>().OnExitGameYesClicked.RemoveListener(ReturnToMainMenuAction);
			uiManager.GetComponent<ExitGamePrompt>().OnExitGameCancelClicked.RemoveListener(ReturnToMainMenuAction);


		}

		if (gameManager != null) {

			//remove listener for new or loaded turn
			gameManager.OnNewTurn.RemoveListener (NewTurnSetInitialSelectablesAction);
			gameManager.OnLoadedTurn.RemoveListener (NewTurnSetInitialSelectablesAction);


		}

		//remove listener for a pointer click selection
		UISelection.OnSetSelectedGameObject.RemoveListener(SelectableSetSelectionGroupsAction);

	}

}