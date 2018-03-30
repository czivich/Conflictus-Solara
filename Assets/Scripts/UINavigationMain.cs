using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class UINavigationMain : MonoBehaviour {

	public static bool blockPointerClickFlag { get; private set;}

	//this will hold the eventSystem in the scene
	public EventSystem eventSystem;

	//uiManager
	private GameObject uiManager;
	private GameManager gameManager;
	private MouseManager mouseManager;


	//this enum will keep track of the current UI State
	public enum UIState{

		Selection,
		MoveMenu,
		PhasorMenu,
		TorpedoMenu,
		TractorBeamMenu,
		UseItem,
		Crew,
		Cloaking,
		BuyItem,
		BuyShip,
		RenameUnit,
		EndTurn,
		LoadLocalGame,
		FileDeletePrompt,
		Settings,
		ExitGamePrompt,

	}

	//this bool will wait a frame when needed
	private int delayLoadFilesWindowCount = 0;
	private int delaySetInitialSelectablesCount = 0;
	private int delayReturnToSelectableCount = 0;

	private Selectable returnSelectable;
	private UIState returnUIState;

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

				//set the flag
				//blockPointerClickFlag = true;

				//invoke the event
				OnUIStateChange.Invoke();

			}

		}


	}

	//these public arrays are groups of selectables arrays that make up each UI state
	private Selectable[][] SelectionGroup;
	private Selectable[][] MoveMenuGroup;
	private Selectable[][] PhasorMenuGroup;
	private Selectable[][] TorpedoMenuGroup;
	private Selectable[][] TractorBeamMenuGroup;
	private Selectable[][] UseItemMenuGroup;
	private Selectable[][] CrewMenuGroup;
	private Selectable[][] CloakingMenuGroup;
	private Selectable[][] EndTurnGroup;

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

	public Selectable[] PhasorRadarShotButton;
	public Selectable[] PhasorTargetingDropdown;
	public Selectable[] PhasorFireButton;

	public Selectable[] TorpedoLaserShotButton;
	public Selectable[] TorpedoTargetingDropdown;
	public Selectable[] TorpedoFireButtons;

	public Selectable[] TractorBeamEngageButton;

	public Selectable[] FlareModeButtons;
	public Selectable[] ItemTargetingDropdown;
	public Selectable[] CrystalButtons;

	public Selectable[] CrewTargetingDropdown;
	public Selectable[] CrewRepairButton;

	public Selectable[] CloakingDeviceEngageButton;


	public Selectable[] EndTurnButtons;



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

	//this int stores the index of the selected file when leaving the file load list
	private int previousFileIndex;

	//event to announce UIState change
	public UnityEvent OnUIStateChange = new UnityEvent();
	public UnityEvent OnSelectablesChange = new UnityEvent();

	//unityActions
	private UnityAction<Player> NewTurnSetInitialSelectablesAction;
	private UnityAction<bool> MoveToggleSetUIStateAction;
	private UnityAction<bool> PhasorToggleSetUIStateAction;
	private UnityAction<bool> TorpedoToggleSetUIStateAction;
	private UnityAction<bool> TractorBeamToggleSetUIStateAction;
	private UnityAction<bool> UseItemToggleSetUIStateAction;
	private UnityAction<bool> CrewToggleSetUIStateAction;
	private UnityAction<bool> CloakingToggleSetUIStateAction;
	private UnityAction<bool> EndTurnSetUIStateAction;
	private UnityAction CancelEndTurnPromptAction;
	private UnityAction AcceptEndTurnPromptAction;


	private UnityAction<Selectable> SelectableSetSelectionGroupsAction;
	private UnityAction ReturnToSelectionAction;
	private UnityAction OpenLoadLocalGameWindowAction;
	private UnityAction SelectableSetNavigationRulesAction;
	private UnityAction<string> OpenFileDeletePromptAction;
	private UnityAction<string> StringReturnToFileLoadWindowAction;
	private UnityAction OpenSettingsWindowAction;
	private UnityAction OpenExitGamePromptAction;
	private UnityAction<string> InputFieldEndEditIgnoreEscapeAction;
	private UnityAction<Ship> TractorBeamShipSetUIStateAction;
	private UnityAction ClearSetInitialSelectablesAction;
	private UnityAction<CombatUnit> CombatUnitSetUIStateAction;

	private UnityAction<Selectable> PointerClickResolveBlockAction;


	// Use this for initialization
	public void Init () {

		//get the uiManager
		uiManager = GameObject.FindGameObjectWithTag("UIManager");
		gameManager = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameManager> ();
		mouseManager = GameObject.FindGameObjectWithTag ("MouseManager").GetComponent<MouseManager> ();

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

		//check if we need to wait a frame for the set selectables
		if (delaySetInitialSelectablesCount > 0) {

			delaySetInitialSelectablesCount--;

			if (delaySetInitialSelectablesCount == 0) {

				SetInitialCurrentSelectables ();
				return;

			}

		}

		//check if we need to wait a frame for the return selectables
		if (delayReturnToSelectableCount > 0) {

			delayReturnToSelectableCount--;

			if (delayReturnToSelectableCount == 0) {

				ReturnToSelectable ();
				return;

			}

		}
		//check if the down arrow is being pressed
		if (Input.GetKeyDown (KeyCode.DownArrow)) {

			//this checks if we have lost our selectable and goes back to it instead of advancing to the next one
			if (eventSystem.currentSelectedGameObject == null) {

				if (CurrentSelectables != null && CurrentSelectables [currentSelectionIndex] != null) {

					eventSystem.SetSelectedGameObject (CurrentSelectables [currentSelectionIndex].gameObject);

					return;

				}

			}

			//check if the vertical cycling is on
			else if (verticalCycling == true) {

				//if the vertical cycling is on, advance the selection
				AdvanceSelectable(false);

				if (CurrentUIState == UIState.LoadLocalGame && CurrentSelectables == LoadLocalGameFiles) {

					//here we are selecting a lower file in the scroll rect
					FitCurrentSelectableIntoScrollRect ();

				}

			} else if(CurrentUIState == UIState.PhasorMenu && CurrentSelectables == PhasorTargetingDropdown){

				//adjust the dropdown value
				AdjustDropdownValueDown(PhasorTargetingDropdown[0].GetComponent<TMP_Dropdown>());

			} else if(CurrentUIState == UIState.TorpedoMenu && CurrentSelectables == TorpedoTargetingDropdown){

				//adjust the dropdown value
				AdjustDropdownValueDown(TorpedoTargetingDropdown[0].GetComponent<TMP_Dropdown>());

			} else if(CurrentUIState == UIState.UseItem && CurrentSelectables == ItemTargetingDropdown){

				//adjust the dropdown value
				AdjustDropdownValueDown(ItemTargetingDropdown[0].GetComponent<TMP_Dropdown>());

			} else if(CurrentUIState == UIState.Crew && CurrentSelectables == CrewTargetingDropdown){

				//adjust the dropdown value
				AdjustDropdownValueDown(CrewTargetingDropdown[0].GetComponent<TMP_Dropdown>());

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

			//this checks if we have lost our selectable and goes back to it instead of advancing to the next one
			if (eventSystem.currentSelectedGameObject == null) {

				if (CurrentSelectables != null && CurrentSelectables [currentSelectionIndex] != null) {

					eventSystem.SetSelectedGameObject (CurrentSelectables [currentSelectionIndex].gameObject);

					return;

				}

			}

			//check if the vertical cycling is on
			else if (verticalCycling == true) {

				//if the vertical cycling is on, advance the selection
				AdvanceSelectable(true);

				if (CurrentUIState == UIState.LoadLocalGame && CurrentSelectables == LoadLocalGameFiles) {

					//here we are selecting a lower file in the scroll rect

					FitCurrentSelectableIntoScrollRect ();

				}

			} else if(CurrentUIState == UIState.PhasorMenu && CurrentSelectables == PhasorTargetingDropdown){

				//adjust the dropdown value
				AdjustDropdownValueUp(PhasorTargetingDropdown[0].GetComponent<TMP_Dropdown>());

			} else if(CurrentUIState == UIState.TorpedoMenu && CurrentSelectables == TorpedoTargetingDropdown){

				//adjust the dropdown value
				AdjustDropdownValueUp(TorpedoTargetingDropdown[0].GetComponent<TMP_Dropdown>());

			} else if(CurrentUIState == UIState.UseItem && CurrentSelectables == ItemTargetingDropdown){

				//adjust the dropdown value
				AdjustDropdownValueUp(ItemTargetingDropdown[0].GetComponent<TMP_Dropdown>());

			} else if(CurrentUIState == UIState.Crew && CurrentSelectables == CrewTargetingDropdown){

				//adjust the dropdown value
				AdjustDropdownValueUp(CrewTargetingDropdown[0].GetComponent<TMP_Dropdown>());

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

			//this checks if we have lost our selectable and goes back to it instead of advancing to the next one
			if (eventSystem.currentSelectedGameObject == null) {

				if (CurrentSelectables != null && CurrentSelectables [currentSelectionIndex] != null) {

					eventSystem.SetSelectedGameObject (CurrentSelectables [currentSelectionIndex].gameObject);

					return;

				}

			}

			//check if the horizontal cycling is on
			else if (horizontalCycling == true) {

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

			//this checks if we have lost our selectable and goes back to it instead of advancing to the next one
			if (eventSystem.currentSelectedGameObject == null) {

				if (CurrentSelectables != null && CurrentSelectables [currentSelectionIndex] != null) {

					eventSystem.SetSelectedGameObject (CurrentSelectables [currentSelectionIndex].gameObject);

					return;

				}

			}

			//check if the horizontal cycling is on
			else if (horizontalCycling == true) {

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

			//check if we are in a dropdown
			if (CurrentSelectables [currentSelectionIndex] != null &&
			    CurrentSelectables [currentSelectionIndex].GetComponent<TMP_Dropdown> () == true &&
			    CurrentSelectables [currentSelectionIndex].transform.Find ("Dropdown List") != null) {

				//hide the list
				CurrentSelectables [currentSelectionIndex].GetComponent<TMP_Dropdown> ().Hide ();


			} else if (eventSystem.currentSelectedGameObject == null) {

				if (CurrentSelectables != null && CurrentSelectables [currentSelectionIndex] != null) {

					eventSystem.SetSelectedGameObject (CurrentSelectables [currentSelectionIndex].gameObject);

					return;

				}

			}

			//check if we are also holdin down a shift key
			//if we are holding a shift key, we want tab to cycle backwards
			else if (Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift)) {

				//check if we are in the load files - if we are here, we don't want tab to cycle through the files
				if (CurrentSelectables == LoadLocalGameFiles) {
					
					//store the file index
					previousFileIndex = currentSelectionIndex;

					//advance to the next group
					AdvanceSelectableGroup (true);

				}

				//check if we are at the first interactable element of the selectables array
				else if (currentSelectionIndex == FindFirstInteractableIndex(CurrentSelectables)) {

					//check if the next group is loadfiles
					if (GetNextSelectableGroup (true) == LoadLocalGameFiles) {

						//directly set the selectable group
						CurrentSelectables = LoadLocalGameFiles;
						currentSelectionGroupIndex = 0;
						EventSystem.current.SetSelectedGameObject (CurrentSelectables [previousFileIndex].gameObject);
						currentSelectionIndex = previousFileIndex;

					} else {

						//advance to the next group
						AdvanceSelectableGroup (true);

					}

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

					//store the file index
					previousFileIndex = currentSelectionIndex;

					//advance to the next group
					AdvanceSelectableGroup (false);

				}

				//check if we are at the last interactable element of the selectables array
				else if (currentSelectionIndex == FindLastInteractableIndex	(CurrentSelectables)) {

					//check if the next group is loadfiles
					if (GetNextSelectableGroup (false) == LoadLocalGameFiles) {

						//directly set the selectable group
						CurrentSelectables = LoadLocalGameFiles;
						currentSelectionGroupIndex = 0;
						EventSystem.current.SetSelectedGameObject (CurrentSelectables [previousFileIndex].gameObject);
						currentSelectionIndex = previousFileIndex;

					} else {

						//advance to the next group
						AdvanceSelectableGroup (false);

					}

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

			if (CurrentUIState == UIState.LoadLocalGame) {

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

			} 

		}

		//check if we are pressing the enter key
		if (Input.GetKeyDown (KeyCode.Return) == true) {

			//check if we are currently on a file for load
			if (CurrentUIState == UIState.LoadLocalGame && CurrentSelectables == LoadLocalGameFiles) {

				//call the load game button on-click
				uiManager.GetComponent<FileLoadWindow>().fileLoadYesButton.onClick.Invoke();

			}

		}

		//check if we are pressing the delete key
		if (Input.GetKeyDown (KeyCode.Delete) == true) {

			//check if we are currently on a file for load
			if (CurrentUIState == UIState.LoadLocalGame && CurrentSelectables == LoadLocalGameFiles) {

				//call the delete file button on-click
				uiManager.GetComponent<FileLoadWindow>().fileDeleteYesButton.onClick.Invoke();

			}

		}


		//at the end of a frame, we can stop ignoring escape
		//if we still need to ignore it, the input field events will trigger again
		ignoreEscape = false;

		//check if the current selected object is a file list item
		if (CurrentUIState == UIState.LoadLocalGame) {

			if (CurrentSelectables == LoadLocalGameFiles) {

				//cache the previous index
				previousFileIndex = currentSelectionIndex;

			}

		}

		//this will prevent the uncontrollable event order from screwing up the selectCurrentObject
		//blockPointerClickFlag = false;

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

		PhasorToggleSetUIStateAction = (toggleState) => {

			if (toggleState == true) {

				CurrentUIState = UIState.PhasorMenu;

			} else {

				CurrentUIState = UIState.Selection;

			}
			;

		};

		TorpedoToggleSetUIStateAction = (toggleState) => {

			if (toggleState == true) {

				CurrentUIState = UIState.TorpedoMenu;

			} else {

				CurrentUIState = UIState.Selection;

			}
			;

		};

		TractorBeamToggleSetUIStateAction = (toggleState) => {

			if (toggleState == true) {

				CurrentUIState = UIState.TractorBeamMenu;

			} else {

				CurrentUIState = UIState.Selection;

			}
			;

		};

		UseItemToggleSetUIStateAction = (toggleState) => {

			if (toggleState == true) {

				CurrentUIState = UIState.UseItem;

			} else {

				CurrentUIState = UIState.Selection;

			}
			;

		};

		CrewToggleSetUIStateAction = (toggleState) => {

			if (toggleState == true) {

				CurrentUIState = UIState.Crew;

			} else {

				CurrentUIState = UIState.Selection;

			}
			;

		};

		CloakingToggleSetUIStateAction = (toggleState) => {

			if (toggleState == true) {

				CurrentUIState = UIState.Cloaking;

			} else {

				CurrentUIState = UIState.Selection;

			}
			;

		};

		EndTurnSetUIStateAction = (toggleState) => {

			if (toggleState == true) {

				CurrentUIState = UIState.EndTurn;

			} else {

				CurrentUIState = UIState.Selection;

			}
			;

		};

		CancelEndTurnPromptAction = () => {

			//returnUIState = UIState.Selection;
			returnSelectable = ActionMenuButtons[10];

			//returnSelectable = null;
			delayReturnToSelectableCount = 2;

			//set the initial UIState
			//CurrentUIState = UIState.Selection;

			//invoke the event to start, since it won't fire because the enum defaults to the Selection
			//OnUIStateChange.Invoke();
		
		};

		AcceptEndTurnPromptAction = () => {

			//returnUIState = UIState.Selection;
			returnSelectable = null;

			//returnSelectable = null;
			delayReturnToSelectableCount = 2;

			//set the initial UIState
			//CurrentUIState = UIState.Selection;

			//invoke the event to start, since it won't fire because the enum defaults to the Selection
			//OnUIStateChange.Invoke();

		};

		TractorBeamShipSetUIStateAction = (ship) => {SetInitialCurrentSelectables ();};

		CombatUnitSetUIStateAction = (combatUnit) => {delaySetInitialSelectablesCount = 1;};

		SelectableSetSelectionGroupsAction = (selectable) => {SetSelectionIndexFromPointerClick (selectable);};

		ReturnToSelectionAction = () => {CurrentUIState = UIState.Selection;};

		OpenLoadLocalGameWindowAction = () => {CurrentUIState = UIState.LoadLocalGame;};

		SelectableSetNavigationRulesAction = () => {SetNavigationRulesForSelectables();};

		OpenFileDeletePromptAction = (fileName) => {CurrentUIState = UIState.FileDeletePrompt;};

		StringReturnToFileLoadWindowAction = (fileName) => {delayLoadFilesWindowCount = 2;};

		ClearSetInitialSelectablesAction = () => {delaySetInitialSelectablesCount = 1;};

		OpenSettingsWindowAction = () => {CurrentUIState = UIState.Settings;}; 

		OpenExitGamePromptAction = () => {CurrentUIState = UIState.ExitGamePrompt;};

		InputFieldEndEditIgnoreEscapeAction = (eventString) => {ignoreEscape = true;};

		PointerClickResolveBlockAction = (selectable) => {ResolvePointerClickBlock (selectable);};

	}

	//this function adds event listeners
	private void AddEventListeners(){

		//add listener for UIState change
		OnUIStateChange.AddListener(SetInitialCurrentSelectables);

		//add listener for a pointer click selection
		UISelection.OnSetSelectedGameObject.AddListener(SelectableSetSelectionGroupsAction);

		//add listener for a pointer click
		UISelection.OnClickedSelectable.AddListener(PointerClickResolveBlockAction);

		//add listener for move toggle
		uiManager.GetComponent<MoveToggle>().moveToggle.onValueChanged.AddListener(MoveToggleSetUIStateAction);

		//add listener for phasor toggle
		uiManager.GetComponent<PhasorToggle>().phasorToggle.onValueChanged.AddListener(PhasorToggleSetUIStateAction);

		//add listener for torpedo toggle
		uiManager.GetComponent<TorpedoToggle>().torpedoToggle.onValueChanged.AddListener(TorpedoToggleSetUIStateAction);

		//add listener for tractor beam toggle
		uiManager.GetComponent<TractorBeamToggle>().tractorBeamToggle.onValueChanged.AddListener(TractorBeamToggleSetUIStateAction);

		//add listener for use item toggle
		uiManager.GetComponent<UseItemToggle>().useItemToggle.onValueChanged.AddListener(UseItemToggleSetUIStateAction);

		//add listener for crew toggle
		uiManager.GetComponent<CrewToggle>().crewToggle.onValueChanged.AddListener(CrewToggleSetUIStateAction);

		//add listener for cloaking toggle
		uiManager.GetComponent<CloakingDeviceToggle>().cloakingDeviceToggle.onValueChanged.AddListener(CloakingToggleSetUIStateAction);

		//add listener for end turn toggle
		uiManager.GetComponent<EndTurnToggle>().endTurnToggle.onValueChanged.AddListener(EndTurnSetUIStateAction);

		//add listener for disengaging tractor beam
		uiManager.GetComponent<TractorBeamMenu>().OnTurnOffTractorBeamToggle.AddListener(TractorBeamShipSetUIStateAction);

		//add listener for disengaging cloaking device
		uiManager.GetComponent<CloakingDeviceMenu>().OnTurnOffCloakingDevice.AddListener(CombatUnitSetUIStateAction);
		uiManager.GetComponent<CloakingDeviceMenu>().OnTurnOnCloakingDevice.AddListener(CombatUnitSetUIStateAction);

		//add listeners for end turn drop down
		uiManager.GetComponent<EndTurnDropDown>().OnCancelEndTurnPrompt.AddListener(CancelEndTurnPromptAction);
		uiManager.GetComponent<EndTurnDropDown>().OnAcceptEndTurnPrompt.AddListener(AcceptEndTurnPromptAction);


		//add listeners for mouse manager actions
		mouseManager.OnSetTargetedUnit.AddListener(SetInitialCurrentSelectables);
		mouseManager.OnClearTargetedUnit.AddListener(ClearSetInitialSelectablesAction);
		mouseManager.OnSetSelectedUnit.AddListener(SetInitialCurrentSelectables);
		mouseManager.OnClearSelectedUnit.AddListener(ClearSetInitialSelectablesAction);

		//add listener for load local game
		uiManager.GetComponent<FileLoadWindow>().OnOpenFileLoadWindow.AddListener(OpenLoadLocalGameWindowAction);

		//add listener for setting selectables
		OnSelectablesChange.AddListener(SelectableSetNavigationRulesAction);

		//add listeners for exiting the file load window back to the main menu
		uiManager.GetComponent<FileLoadWindow>().closeFileLoadWindowButton.onClick.AddListener(ReturnToSelectionAction);
		uiManager.GetComponent<FileLoadWindow>().fileLoadCancelButton.onClick.AddListener(ReturnToSelectionAction);

		//add listener for entering the file delete prompt
		uiManager.GetComponent<FileLoadWindow>().OnFileDeleteYesClicked.AddListener(OpenFileDeletePromptAction);

		//add listeners for exiting the file delete prompt back to the load file window
		uiManager.GetComponent<FileDeletePrompt>().OnFileDeleteYesClicked.AddListener(StringReturnToFileLoadWindowAction);
		uiManager.GetComponent<FileDeletePrompt>().OnFileDeleteCancelClicked.AddListener(OpenLoadLocalGameWindowAction);

		//add listeners for entering the settings menu
		uiManager.GetComponent<Settings>().settingsMenuButton.onClick.AddListener(OpenSettingsWindowAction);

		//add listeners for exiting the settings menu
		uiManager.GetComponent<Settings>().acceptButton.onClick.AddListener(ReturnToSelectionAction);
		uiManager.GetComponent<Settings>().exitButton.onClick.AddListener(ReturnToSelectionAction);

		//add listener for entering the exit game prompt
		uiManager.GetComponent<ExitGamePrompt>().exitGameButton.onClick.AddListener(OpenExitGamePromptAction);

		//add listener for exiting the exit game prompt
		uiManager.GetComponent<ExitGamePrompt>().OnExitGameYesClicked.AddListener(ReturnToSelectionAction);
		uiManager.GetComponent<ExitGamePrompt>().OnExitGameCancelClicked.AddListener(ReturnToSelectionAction);

		//add listener for new or loaded turn
		gameManager.OnNewTurn.AddListener(NewTurnSetInitialSelectablesAction);
		gameManager.OnLoadedTurn.AddListener (NewTurnSetInitialSelectablesAction);

		//add listener for Cutscene ending
		uiManager.GetComponent<CutsceneManager>().OnCloseCutsceneDisplayPanel.AddListener(SetInitialCurrentSelectables);

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

		PhasorMenuGroup = new Selectable[8][];
		PhasorMenuGroup [0] = PhasorRadarShotButton;
		PhasorMenuGroup [1] = PhasorTargetingDropdown;
		PhasorMenuGroup [2] = PhasorFireButton;
		PhasorMenuGroup [3] = ActionMenuButtons;
		PhasorMenuGroup [4] = ChatInputField;
		PhasorMenuGroup [5] = FileMenuButtons;
		PhasorMenuGroup [6] = NextUnitButtons;
		PhasorMenuGroup [7] = StatusButton;

		TorpedoMenuGroup = new Selectable[8][];
		TorpedoMenuGroup [0] = TorpedoLaserShotButton;
		TorpedoMenuGroup [1] = TorpedoTargetingDropdown;
		TorpedoMenuGroup [2] = TorpedoFireButtons;
		TorpedoMenuGroup [3] = ActionMenuButtons;
		TorpedoMenuGroup [4] = ChatInputField;
		TorpedoMenuGroup [5] = FileMenuButtons;
		TorpedoMenuGroup [6] = NextUnitButtons;
		TorpedoMenuGroup [7] = StatusButton;

		TractorBeamMenuGroup = new Selectable[6][];
		TractorBeamMenuGroup [0] = TractorBeamEngageButton;
		TractorBeamMenuGroup [1] = ActionMenuButtons;
		TractorBeamMenuGroup [2] = ChatInputField;
		TractorBeamMenuGroup [3] = FileMenuButtons;
		TractorBeamMenuGroup [4] = NextUnitButtons;
		TractorBeamMenuGroup [5] = StatusButton;

		UseItemMenuGroup = new Selectable[8][];
		UseItemMenuGroup [0] = FlareModeButtons;
		UseItemMenuGroup [1] = ItemTargetingDropdown;
		UseItemMenuGroup [2] = CrystalButtons;
		UseItemMenuGroup [3] = ActionMenuButtons;
		UseItemMenuGroup [4] = ChatInputField;
		UseItemMenuGroup [5] = FileMenuButtons;
		UseItemMenuGroup [6] = NextUnitButtons;
		UseItemMenuGroup [7] = StatusButton;

		CrewMenuGroup = new Selectable[7][];
		CrewMenuGroup [0] = CrewTargetingDropdown;
		CrewMenuGroup [1] = CrewRepairButton;
		CrewMenuGroup [2] = ActionMenuButtons;
		CrewMenuGroup [3] = ChatInputField;
		CrewMenuGroup [4] = FileMenuButtons;
		CrewMenuGroup [5] = NextUnitButtons;
		CrewMenuGroup [6] = StatusButton;

		CloakingMenuGroup = new Selectable[6][];
		CloakingMenuGroup [0] = CloakingDeviceEngageButton;
		CloakingMenuGroup [1] = ActionMenuButtons;
		CloakingMenuGroup [2] = ChatInputField;
		CloakingMenuGroup [3] = FileMenuButtons;
		CloakingMenuGroup [4] = NextUnitButtons;
		CloakingMenuGroup [5] = StatusButton;

		EndTurnGroup = new Selectable[1][];
		EndTurnGroup [0] = EndTurnButtons;


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


		} else if (CurrentSelectables == PhasorRadarShotButton) {

			horizontalCycling = false;
			verticalCycling = true;
			selectablesWrap = false;

		} else if (CurrentSelectables == PhasorTargetingDropdown) {

			horizontalCycling = false;
			verticalCycling = false;
			selectablesWrap = false;

		} else if (CurrentSelectables == PhasorFireButton) {

			horizontalCycling = false;
			verticalCycling = false;
			selectablesWrap = false;

		} else if (CurrentSelectables == TorpedoLaserShotButton) {

			horizontalCycling = false;
			verticalCycling = true;
			selectablesWrap = false;

		} else if (CurrentSelectables == TorpedoTargetingDropdown) {

			horizontalCycling = false;
			verticalCycling = false;
			selectablesWrap = false;

		} else if (CurrentSelectables == TorpedoFireButtons) {

			horizontalCycling = false;
			verticalCycling = true;
			selectablesWrap = false;

		} else if (CurrentSelectables == TorpedoFireButtons) {

			horizontalCycling = false;
			verticalCycling = true;
			selectablesWrap = false;

		} else if (CurrentSelectables == TractorBeamEngageButton) {

			horizontalCycling = false;
			verticalCycling = false;
			selectablesWrap = false;

		} else if (CurrentSelectables == FlareModeButtons) {

			horizontalCycling = true;
			verticalCycling = false;
			selectablesWrap = true;

		} else if (CurrentSelectables == ItemTargetingDropdown) {

			horizontalCycling = false;
			verticalCycling = false;
			selectablesWrap = false;

		} else if (CurrentSelectables == CrystalButtons) {

			horizontalCycling = false;
			verticalCycling = true;
			selectablesWrap = false;

		} else if (CurrentSelectables == CrewTargetingDropdown) {

			horizontalCycling = false;
			verticalCycling = false;
			selectablesWrap = false;

		} else if (CurrentSelectables == CrewRepairButton) {

			horizontalCycling = false;
			verticalCycling = false;
			selectablesWrap = false;

		} else if (CurrentSelectables == CloakingDeviceEngageButton) {

			horizontalCycling = false;
			verticalCycling = false;
			selectablesWrap = false;

		} else if (CurrentSelectables == EndTurnButtons) {

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

		//bool to determine if the current selectable is in the new group
		bool currentSelectableInNewGroup = false;

		//switch case based on UI state
		switch (CurrentUIState) {

		case UIState.Selection:

			//Debug.Log ("case selection");

			//set the current selectables group to match the UI state
			currentSelectablesGroup = SelectionGroup;


			//check if the current selectable is within the new selectables group
			//if the current selectable is within the new group, we don't want set the selectables to the first interactable option,
			//we want to leave it where it is
			//but we will need to know what the group index is within the context of the new group

			//only do this if there is a current selectable
			if (EventSystem.current.currentSelectedGameObject != null) {

				//Debug.Log ("current not null");

				for (int i = 0; i < currentSelectablesGroup.Length; i++) {

					//check if the current selectable is within the array at the ith index
					if (currentSelectablesGroup [i].Contains (EventSystem.current.currentSelectedGameObject.GetComponent<Selectable> ())) {

						//set the currentSelectionGroupIndex
						currentSelectionGroupIndex = i;

						//set the currentSelectables based on the index returned
						CurrentSelectables = currentSelectablesGroup [currentSelectionGroupIndex];

						//set the flag
						currentSelectableInNewGroup = true;

						//break out of the for loop
						break;
					}

				}

				//check if the current selectable was found in the new group
				if (currentSelectableInNewGroup == true) {

					//set the index based on the selectable's location in the new group
					for (int i = 0; i < currentSelectables.Length; i++) {

						//check if the current selectable is within the array at the ith index
						if (currentSelectables [i] == eventSystem.currentSelectedGameObject.GetComponent<Selectable> ()) {

							//set the currentSelectionIndex
							currentSelectionIndex = i;

							//we have the group index and the selectable index set, and we have the currentSelectable set
							//we can return from the method
							return;

						}

					}

				} else {

					//if the current is not in the group, we need to find a new valid selectable

					//find the first array in the group that has an interactable selectable
					potentialCurrentSelectionGroupIndex = FindFirstInteractableArrayIndex (currentSelectablesGroup);

					//set the selectable array that contains an interactable
					if (potentialCurrentSelectionGroupIndex != -1) {

						//set the currentSelectionGroupIndex
						currentSelectionGroupIndex = potentialCurrentSelectionGroupIndex;

						//set the currentSelectables based on the index returned
						CurrentSelectables = currentSelectablesGroup [currentSelectionGroupIndex];

					}

				}

			} else {

				//Debug.Log ("current is null");

				//if there is no current selectable, we need to find a new valid selectable

				//find the first array in the group that has an interactable selectable
				potentialCurrentSelectionGroupIndex = FindFirstInteractableArrayIndex (currentSelectablesGroup);

				//Debug.Log ("potential is "+ potentialCurrentSelectionGroupIndex);

				//set the selectable array that contains an interactable
				if (potentialCurrentSelectionGroupIndex != -1) {

					//set the currentSelectionGroupIndex
					currentSelectionGroupIndex = potentialCurrentSelectionGroupIndex;

					//set the currentSelectables based on the index returned
					CurrentSelectables = currentSelectablesGroup [currentSelectionGroupIndex];

				}

			}

			break;

		case UIState.MoveMenu:

			//set the current selectables group to match the UI state
			currentSelectablesGroup = MoveMenuGroup;

			//find the first array in the group that has an interactable selectable
			potentialCurrentSelectionGroupIndex = FindFirstInteractableArrayIndex (currentSelectablesGroup);

			//check if the potential group index is 0, which would indicate the move dropdown
			if (potentialCurrentSelectionGroupIndex != 0) {

				//if the index is not zero, that means we can't be on the dropdown, because there are no interactable selectables in the dropdown
				//in this case, we want to stay on the toggle that turned on the dropdown
				//check to make sure that current selected object is in the new group

				//only do this if there is a current selectable
				if (EventSystem.current.currentSelectedGameObject != null) {

					for (int i = 0; i < currentSelectablesGroup.Length; i++) {

						//check if the current selectable is within the array at the ith index
						if (currentSelectablesGroup [i].Contains (EventSystem.current.currentSelectedGameObject.GetComponent<Selectable> ())) {

							//set the currentSelectionGroupIndex
							currentSelectionGroupIndex = i;

							//set the currentSelectables based on the index returned
							CurrentSelectables = currentSelectablesGroup [currentSelectionGroupIndex];

							//set the flag
							currentSelectableInNewGroup = true;

							//break out of the for loop
							break;
						}

					}

					//check if the current selectable was found in the new group
					if (currentSelectableInNewGroup == true) {

						//set the index based on the selectable's location in the new group
						for (int i = 0; i < currentSelectables.Length; i++) {

							//check if the current selectable is within the array at the ith index
							if (currentSelectables [i] == eventSystem.currentSelectedGameObject.GetComponent<Selectable> ()) {

								//set the currentSelectionIndex
								currentSelectionIndex = i;

								//we have the group index and the selectable index set, and we have the currentSelectable set
								//we can return from the method
								return;

							}

						}

					} else {

						//if the current is not in the group, we need to find a new valid selectable

						//set the selectable array that contains an interactable
						if (potentialCurrentSelectionGroupIndex != -1) {

							//set the currentSelectionGroupIndex
							currentSelectionGroupIndex = potentialCurrentSelectionGroupIndex;

							//set the currentSelectables based on the index returned
							CurrentSelectables = currentSelectablesGroup [currentSelectionGroupIndex];

							//check if the current selectables is the action menu buttons
							if (CurrentSelectables == ActionMenuButtons) {

								if (CurrentSelectables [0].interactable == true) {

									//set the index
									currentSelectionIndex = 0;

									//set the current selectable to the move toggle
									eventSystem.SetSelectedGameObject (CurrentSelectables [currentSelectionIndex].gameObject);

									//return from the funcction
									return;

								}

							}

						}

					}

				} else {

					//Debug.Log ("current selected is null");

					//if there is no current selectable, we need to find a new valid selectable

					//set the selectable array that contains an interactable
					if (potentialCurrentSelectionGroupIndex != -1) {

						//set the currentSelectionGroupIndex
						currentSelectionGroupIndex = potentialCurrentSelectionGroupIndex;

						//set the currentSelectables based on the index returned
						CurrentSelectables = currentSelectablesGroup [currentSelectionGroupIndex];

						//check if the current selectables is the action menu buttons
						if (CurrentSelectables == ActionMenuButtons) {

							if (CurrentSelectables [0].interactable == true) {

								//set the index
								currentSelectionIndex = 0;

								//set the current selectable to the move toggle
								eventSystem.SetSelectedGameObject (CurrentSelectables [currentSelectionIndex].gameObject);

								//return from the funcction
								return;

							}

						}

					}

				}

			} else {

				//the else is that the potential is group zero

				//set the currentSelectionGroupIndex
				currentSelectionGroupIndex = potentialCurrentSelectionGroupIndex;

				//set the currentSelectables based on the index returned
				CurrentSelectables = currentSelectablesGroup [currentSelectionGroupIndex];

			}

			break;

		case UIState.PhasorMenu:

			//Debug.Log ("case phasor");

			//set the current selectables group to match the UI state
			currentSelectablesGroup = PhasorMenuGroup;

			//find the first array in the group that has an interactable selectable
			potentialCurrentSelectionGroupIndex = FindFirstInteractableArrayIndex (currentSelectablesGroup);

			//Debug.Log ("phasor potentialCurrentSelectionGroupIndex = " + potentialCurrentSelectionGroupIndex);


			//check if the potential group index is 0, which would indicate the phasor dropdown
			if (potentialCurrentSelectionGroupIndex != 0) {

				//if the index is not zero, that means we can't be on the dropdown, because there are no interactable selectables in the dropdown
				//in this case, we want to stay on the toggle that turned on the dropdown
				//check to make sure that current selected object is in the new group

				//only do this if there is a current selectable
				if (EventSystem.current.currentSelectedGameObject != null) {

					//Debug.Log ("current selected not null");

					for (int i = 0; i < currentSelectablesGroup.Length; i++) {

						//check if the current selectable is within the array at the ith index
						if (currentSelectablesGroup [i].Contains (EventSystem.current.currentSelectedGameObject.GetComponent<Selectable> ())) {

							//set the currentSelectionGroupIndex
							currentSelectionGroupIndex = i;

							//set the currentSelectables based on the index returned
							CurrentSelectables = currentSelectablesGroup [currentSelectionGroupIndex];

							//set the flag
							currentSelectableInNewGroup = true;

							//break out of the for loop
							break;
						}

					}

					//check if the current selectable was found in the new group
					if (currentSelectableInNewGroup == true) {

						//set the index based on the selectable's location in the new group
						for (int i = 0; i < currentSelectables.Length; i++) {

							//check if the current selectable is within the array at the ith index
							if (currentSelectables [i] == eventSystem.currentSelectedGameObject.GetComponent<Selectable> ()) {

								//set the currentSelectionIndex
								currentSelectionIndex = i;

								//we have the group index and the selectable index set, and we have the currentSelectable set
								//we can return from the method
								return;

							}

						}

					} else {

						//if the current is not in the group, we need to find a new valid selectable

						//set the selectable array that contains an interactable
						if (potentialCurrentSelectionGroupIndex != -1) {

							//set the currentSelectionGroupIndex
							currentSelectionGroupIndex = potentialCurrentSelectionGroupIndex;

							//set the currentSelectables based on the index returned
							CurrentSelectables = currentSelectablesGroup [currentSelectionGroupIndex];

							//check if the current selectables is the action menu buttons
							if (CurrentSelectables == ActionMenuButtons) {

								if (CurrentSelectables [1].interactable == true) {

									//set the index
									currentSelectionIndex = 1;

									//set the current selectable to the phasor toggle
									eventSystem.SetSelectedGameObject (CurrentSelectables [currentSelectionIndex].gameObject);

									//return from the funcction
									return;

								}

							}

						}

					}

				} else {

					//Debug.Log ("current selected is null");

					//if there is no current selectable, we need to find a new valid selectable

					//set the selectable array that contains an interactable
					if (potentialCurrentSelectionGroupIndex != -1) {

						//set the currentSelectionGroupIndex
						currentSelectionGroupIndex = potentialCurrentSelectionGroupIndex;

						//set the currentSelectables based on the index returned
						CurrentSelectables = currentSelectablesGroup [currentSelectionGroupIndex];

						//check if the current selectables is the action menu buttons
						if (CurrentSelectables == ActionMenuButtons) {

							if (CurrentSelectables [1].interactable == true) {

								//set the index
								currentSelectionIndex = 1;

								//set the current selectable to the phasor toggle
								eventSystem.SetSelectedGameObject (CurrentSelectables [currentSelectionIndex].gameObject);

								//return from the funcction
								return;

							}

						}

					}

				}

			} else {

				//the else is that the potential is group zero

				//set the currentSelectionGroupIndex
				currentSelectionGroupIndex = potentialCurrentSelectionGroupIndex;

				//set the currentSelectables based on the index returned
				CurrentSelectables = currentSelectablesGroup [currentSelectionGroupIndex];

			}

			break;

		case UIState.TorpedoMenu:

			//set the current selectables group to match the UI state
			currentSelectablesGroup = TorpedoMenuGroup;

			//find the first array in the group that has an interactable selectable
			potentialCurrentSelectionGroupIndex = FindFirstInteractableArrayIndex (currentSelectablesGroup);

			//Debug.Log ("Torpedo potentialCurrentSelectionGroupIndex = " + potentialCurrentSelectionGroupIndex);

			//check if the potential group index is 0, which would indicate the torpedo dropdown
			if (potentialCurrentSelectionGroupIndex != 0) {

				//if the index is not zero, that means we can't be on the dropdown, because there are no interactable selectables in the dropdown
				//in this case, we want to stay on the toggle that turned on the dropdown
				//check to make sure that current selected object is in the new group

				//only do this if there is a current selectable
				if (EventSystem.current.currentSelectedGameObject != null) {

					//Debug.Log ("torpedo current selected not null");

					for (int i = 0; i < currentSelectablesGroup.Length; i++) {

						//check if the current selectable is within the array at the ith index
						if (currentSelectablesGroup [i].Contains (EventSystem.current.currentSelectedGameObject.GetComponent<Selectable> ())) {

							//set the currentSelectionGroupIndex
							currentSelectionGroupIndex = i;

							//Debug.Log ("torpedo currentSelectionGroupIndex match at i = " + i);

							//set the currentSelectables based on the index returned
							CurrentSelectables = currentSelectablesGroup [currentSelectionGroupIndex];

							//set the flag
							currentSelectableInNewGroup = true;

							//break out of the for loop
							break;
						}

					}

					//check if the current selectable was found in the new group
					if (currentSelectableInNewGroup == true) {

						//set the index based on the selectable's location in the new group
						for (int i = 0; i < currentSelectables.Length; i++) {

							//check if the current selectable is within the array at the ith index
							if (currentSelectables [i] == eventSystem.currentSelectedGameObject.GetComponent<Selectable> ()) {

								//set the currentSelectionIndex
								currentSelectionIndex = i;

								//we have the group index and the selectable index set, and we have the currentSelectable set
								//we can return from the method
								return;

							}

						}

					} else {

						//Debug.Log ("current selectable not in group");

						//if the current is not in the group, we need to find a new valid selectable

						//set the selectable array that contains an interactable
						if (potentialCurrentSelectionGroupIndex != -1) {

							//set the currentSelectionGroupIndex
							currentSelectionGroupIndex = potentialCurrentSelectionGroupIndex;

							//set the currentSelectables based on the index returned
							CurrentSelectables = currentSelectablesGroup [currentSelectionGroupIndex];

							//check if the current selectables is the action menu buttons
							if (CurrentSelectables == ActionMenuButtons) {

								if (CurrentSelectables [2].IsInteractable() == true) {

									//Debug.Log ("torpedo current selectables 2 is interactable");

									//set the index
									currentSelectionIndex = 2;

									//set the current selectable to the torpedo toggle
									eventSystem.SetSelectedGameObject (CurrentSelectables [currentSelectionIndex].gameObject);

									//return from the funcction
									return;

								}

							}

						}

					}

				} else {

					//Debug.Log ("current selected is null");

					//if there is no current selectable, we need to find a new valid selectable

					//set the selectable array that contains an interactable
					if (potentialCurrentSelectionGroupIndex != -1) {

						//set the currentSelectionGroupIndex
						currentSelectionGroupIndex = potentialCurrentSelectionGroupIndex;

						//set the currentSelectables based on the index returned
						CurrentSelectables = currentSelectablesGroup [currentSelectionGroupIndex];

						//check if the current selectables is the action menu buttons
						if (CurrentSelectables == ActionMenuButtons) {

							if (CurrentSelectables [2].interactable == true) {

								//Debug.Log ("current selectables 2 is interactable");

								//set the index
								currentSelectionIndex = 2;

								//set the current selectable to the torpedo toggle
								eventSystem.SetSelectedGameObject (CurrentSelectables [currentSelectionIndex].gameObject);

								//return from the funcction
								return;

							}

						}

					}

				}

			} else {

				//the else is that the potential is group zero

				//set the currentSelectionGroupIndex
				currentSelectionGroupIndex = potentialCurrentSelectionGroupIndex;

				//set the currentSelectables based on the index returned
				CurrentSelectables = currentSelectablesGroup [currentSelectionGroupIndex];

			}

			break;

		case UIState.TractorBeamMenu:

			//set the current selectables group to match the UI state
			currentSelectablesGroup = TractorBeamMenuGroup;

			//find the first array in the group that has an interactable selectable
			potentialCurrentSelectionGroupIndex = FindFirstInteractableArrayIndex (currentSelectablesGroup);

			//Debug.Log ("potentialCurrentSelectionGroupIndex" + potentialCurrentSelectionGroupIndex);

			//check if the potential group index is 0, which would indicate the torpedo dropdown
			if (potentialCurrentSelectionGroupIndex != 0) {

				//if the index is not zero, that means we can't be on the dropdown, because there are no interactable selectables in the dropdown
				//in this case, we want to stay on the toggle that turned on the dropdown
				//check to make sure that current selected object is in the new group

				//only do this if there is a current selectable
				if (EventSystem.current.currentSelectedGameObject != null) {
					
					for (int i = 0; i < currentSelectablesGroup.Length; i++) {

						//check if the current selectable is within the array at the ith index
						if (currentSelectablesGroup [i].Contains (EventSystem.current.currentSelectedGameObject.GetComponent<Selectable> ())) {

							//set the currentSelectionGroupIndex
							currentSelectionGroupIndex = i;

							//set the currentSelectables based on the index returned
							CurrentSelectables = currentSelectablesGroup [currentSelectionGroupIndex];

							//set the flag
							currentSelectableInNewGroup = true;

							//break out of the for loop
							break;
						}

					}

					//check if the current selectable was found in the new group
					if (currentSelectableInNewGroup == true) {

						//set the index based on the selectable's location in the new group
						for (int i = 0; i < currentSelectables.Length; i++) {

							//check if the current selectable is within the array at the ith index
							if (currentSelectables [i] == eventSystem.currentSelectedGameObject.GetComponent<Selectable> ()) {

								//set the currentSelectionIndex
								currentSelectionIndex = i;

								//we have the group index and the selectable index set, and we have the currentSelectable set
								//we can return from the method
								return;

							}

						}

					} else {

						//if the current is not in the group, we need to find a new valid selectable

						//set the selectable array that contains an interactable
						if (potentialCurrentSelectionGroupIndex != -1) {

							//set the currentSelectionGroupIndex
							currentSelectionGroupIndex = potentialCurrentSelectionGroupIndex;

							//set the currentSelectables based on the index returned
							CurrentSelectables = currentSelectablesGroup [currentSelectionGroupIndex];

							//check if the current selectables is the action menu buttons
							if (CurrentSelectables == ActionMenuButtons) {

								if (CurrentSelectables [3].IsInteractable() == true) {

									//Debug.Log ("torpedo current selectables 2 is interactable");

									//set the index
									currentSelectionIndex = 3;

									//set the current selectable to the torpedo toggle
									eventSystem.SetSelectedGameObject (CurrentSelectables [currentSelectionIndex].gameObject);

									//return from the funcction
									return;

								}

							}

						}

					}

				} else {

					//Debug.Log ("current selected is null");

					//if there is no current selectable, we need to find a new valid selectable

					//set the selectable array that contains an interactable
					if (potentialCurrentSelectionGroupIndex != -1) {

						//set the currentSelectionGroupIndex
						currentSelectionGroupIndex = potentialCurrentSelectionGroupIndex;

						//set the currentSelectables based on the index returned
						CurrentSelectables = currentSelectablesGroup [currentSelectionGroupIndex];

						//check if the current selectables is the action menu buttons
						if (CurrentSelectables == ActionMenuButtons) {

							if (CurrentSelectables [3].interactable == true) {

								//Debug.Log ("current selectables 2 is interactable");

								//set the index
								currentSelectionIndex = 3;

								//set the current selectable to the torpedo toggle
								eventSystem.SetSelectedGameObject (CurrentSelectables [currentSelectionIndex].gameObject);

								//return from the funcction
								return;

							}

						}

					}

				}

			} else {

				//the else is that the potential is group zero

				//set the currentSelectionGroupIndex
				currentSelectionGroupIndex = potentialCurrentSelectionGroupIndex;

				//set the currentSelectables based on the index returned
				CurrentSelectables = currentSelectablesGroup [currentSelectionGroupIndex];

			}

			break;

		case UIState.UseItem:

			//set the current selectables group to match the UI state
			currentSelectablesGroup = UseItemMenuGroup;

			//find the first array in the group that has an interactable selectable
			potentialCurrentSelectionGroupIndex = FindFirstInteractableArrayIndex (currentSelectablesGroup);

			//Debug.Log ("potentialCurrentSelectionGroupIndex" + potentialCurrentSelectionGroupIndex);

			//check if the potential group index is 0, which would indicate the torpedo dropdown
			if (potentialCurrentSelectionGroupIndex != 0) {

				//if the index is not zero, that means we can't be on the dropdown, because there are no interactable selectables in the dropdown
				//in this case, we want to stay on the toggle that turned on the dropdown
				//check to make sure that current selected object is in the new group

				//only do this if there is a current selectable
				if (EventSystem.current.currentSelectedGameObject != null) {

					for (int i = 0; i < currentSelectablesGroup.Length; i++) {

						//check if the current selectable is within the array at the ith index
						if (currentSelectablesGroup [i].Contains (EventSystem.current.currentSelectedGameObject.GetComponent<Selectable> ())) {

							//set the currentSelectionGroupIndex
							currentSelectionGroupIndex = i;

							//set the currentSelectables based on the index returned
							CurrentSelectables = currentSelectablesGroup [currentSelectionGroupIndex];

							//set the flag
							currentSelectableInNewGroup = true;

							//break out of the for loop
							break;
						}

					}

					//check if the current selectable was found in the new group
					if (currentSelectableInNewGroup == true) {

						//set the index based on the selectable's location in the new group
						for (int i = 0; i < currentSelectables.Length; i++) {

							//check if the current selectable is within the array at the ith index
							if (currentSelectables [i] == eventSystem.currentSelectedGameObject.GetComponent<Selectable> ()) {

								//set the currentSelectionIndex
								currentSelectionIndex = i;

								//we have the group index and the selectable index set, and we have the currentSelectable set
								//we can return from the method
								return;

							}

						}

					} else {

						//if the current is not in the group, we need to find a new valid selectable

						//set the selectable array that contains an interactable
						if (potentialCurrentSelectionGroupIndex != -1) {

							//set the currentSelectionGroupIndex
							currentSelectionGroupIndex = potentialCurrentSelectionGroupIndex;

							//set the currentSelectables based on the index returned
							CurrentSelectables = currentSelectablesGroup [currentSelectionGroupIndex];

							//check if the current selectables is the action menu buttons
							if (CurrentSelectables == ActionMenuButtons) {

								if (CurrentSelectables [4].IsInteractable() == true) {

									//Debug.Log ("torpedo current selectables 2 is interactable");

									//set the index
									currentSelectionIndex = 4;

									//set the current selectable to the torpedo toggle
									eventSystem.SetSelectedGameObject (CurrentSelectables [currentSelectionIndex].gameObject);

									//return from the funcction
									return;

								}

							}

						}

					}

				} else {

					//Debug.Log ("current selected is null");

					//if there is no current selectable, we need to find a new valid selectable

					//set the selectable array that contains an interactable
					if (potentialCurrentSelectionGroupIndex != -1) {

						//set the currentSelectionGroupIndex
						currentSelectionGroupIndex = potentialCurrentSelectionGroupIndex;

						//set the currentSelectables based on the index returned
						CurrentSelectables = currentSelectablesGroup [currentSelectionGroupIndex];

						//check if the current selectables is the action menu buttons
						if (CurrentSelectables == ActionMenuButtons) {

							if (CurrentSelectables [4].interactable == true) {

								//Debug.Log ("current selectables 2 is interactable");

								//set the index
								currentSelectionIndex = 4;

								//set the current selectable to the torpedo toggle
								eventSystem.SetSelectedGameObject (CurrentSelectables [currentSelectionIndex].gameObject);

								//return from the funcction
								return;

							}

						}

					}

				}

			} else {

				//the else is that the potential is group zero

				//set the currentSelectionGroupIndex
				currentSelectionGroupIndex = potentialCurrentSelectionGroupIndex;

				//set the currentSelectables based on the index returned
				CurrentSelectables = currentSelectablesGroup [currentSelectionGroupIndex];

			}

			break;

		case UIState.Crew:

			//set the current selectables group to match the UI state
			currentSelectablesGroup = CrewMenuGroup;

			//find the first array in the group that has an interactable selectable
			potentialCurrentSelectionGroupIndex = FindFirstInteractableArrayIndex (currentSelectablesGroup);

			//Debug.Log ("potentialCurrentSelectionGroupIndex" + potentialCurrentSelectionGroupIndex);

			//check if the potential group index is 0, which would indicate the torpedo dropdown
			if (potentialCurrentSelectionGroupIndex != 0) {

				//if the index is not zero, that means we can't be on the dropdown, because there are no interactable selectables in the dropdown
				//in this case, we want to stay on the toggle that turned on the dropdown
				//check to make sure that current selected object is in the new group

				//only do this if there is a current selectable
				if (EventSystem.current.currentSelectedGameObject != null) {

					for (int i = 0; i < currentSelectablesGroup.Length; i++) {

						//check if the current selectable is within the array at the ith index
						if (currentSelectablesGroup [i].Contains (EventSystem.current.currentSelectedGameObject.GetComponent<Selectable> ())) {

							//set the currentSelectionGroupIndex
							currentSelectionGroupIndex = i;

							//set the currentSelectables based on the index returned
							CurrentSelectables = currentSelectablesGroup [currentSelectionGroupIndex];

							//set the flag
							currentSelectableInNewGroup = true;

							//break out of the for loop
							break;
						}

					}

					//check if the current selectable was found in the new group
					if (currentSelectableInNewGroup == true) {

						//set the index based on the selectable's location in the new group
						for (int i = 0; i < currentSelectables.Length; i++) {

							//check if the current selectable is within the array at the ith index
							if (currentSelectables [i] == eventSystem.currentSelectedGameObject.GetComponent<Selectable> ()) {

								//set the currentSelectionIndex
								currentSelectionIndex = i;

								//we have the group index and the selectable index set, and we have the currentSelectable set
								//we can return from the method
								return;

							}

						}

					} else {

						//if the current is not in the group, we need to find a new valid selectable

						//set the selectable array that contains an interactable
						if (potentialCurrentSelectionGroupIndex != -1) {

							//set the currentSelectionGroupIndex
							currentSelectionGroupIndex = potentialCurrentSelectionGroupIndex;

							//set the currentSelectables based on the index returned
							CurrentSelectables = currentSelectablesGroup [currentSelectionGroupIndex];

							//check if the current selectables is the action menu buttons
							if (CurrentSelectables == ActionMenuButtons) {

								if (CurrentSelectables [5].IsInteractable() == true) {

									//Debug.Log ("torpedo current selectables 2 is interactable");

									//set the index
									currentSelectionIndex = 5;

									//set the current selectable to the torpedo toggle
									eventSystem.SetSelectedGameObject (CurrentSelectables [currentSelectionIndex].gameObject);

									//return from the funcction
									return;

								}

							}

						}

					}

				} else {

					//Debug.Log ("current selected is null");

					//if there is no current selectable, we need to find a new valid selectable

					//set the selectable array that contains an interactable
					if (potentialCurrentSelectionGroupIndex != -1) {

						//set the currentSelectionGroupIndex
						currentSelectionGroupIndex = potentialCurrentSelectionGroupIndex;

						//set the currentSelectables based on the index returned
						CurrentSelectables = currentSelectablesGroup [currentSelectionGroupIndex];

						//check if the current selectables is the action menu buttons
						if (CurrentSelectables == ActionMenuButtons) {

							if (CurrentSelectables [5].interactable == true) {

								//Debug.Log ("current selectables 2 is interactable");

								//set the index
								currentSelectionIndex = 5;

								//set the current selectable to the torpedo toggle
								eventSystem.SetSelectedGameObject (CurrentSelectables [currentSelectionIndex].gameObject);

								//return from the funcction
								return;

							}

						}

					}

				}

			} else {

				//the else is that the potential is group zero

				//set the currentSelectionGroupIndex
				currentSelectionGroupIndex = potentialCurrentSelectionGroupIndex;

				//set the currentSelectables based on the index returned
				CurrentSelectables = currentSelectablesGroup [currentSelectionGroupIndex];

			}

			break;

		case UIState.Cloaking:

			//set the current selectables group to match the UI state
			currentSelectablesGroup = CloakingMenuGroup;

			//find the first array in the group that has an interactable selectable
			potentialCurrentSelectionGroupIndex = FindFirstInteractableArrayIndex (currentSelectablesGroup);

			//Debug.Log ("potentialCurrentSelectionGroupIndex" + potentialCurrentSelectionGroupIndex);

			//check if the potential group index is 0, which would indicate the torpedo dropdown
			if (potentialCurrentSelectionGroupIndex != 0) {

				//if the index is not zero, that means we can't be on the dropdown, because there are no interactable selectables in the dropdown
				//in this case, we want to stay on the toggle that turned on the dropdown
				//check to make sure that current selected object is in the new group

				//only do this if there is a current selectable
				if (EventSystem.current.currentSelectedGameObject != null) {

					for (int i = 0; i < currentSelectablesGroup.Length; i++) {

						//check if the current selectable is within the array at the ith index
						if (currentSelectablesGroup [i].Contains (EventSystem.current.currentSelectedGameObject.GetComponent<Selectable> ())) {

							//set the currentSelectionGroupIndex
							currentSelectionGroupIndex = i;

							//set the currentSelectables based on the index returned
							CurrentSelectables = currentSelectablesGroup [currentSelectionGroupIndex];

							//set the flag
							currentSelectableInNewGroup = true;

							//break out of the for loop
							break;
						}

					}

					//check if the current selectable was found in the new group
					if (currentSelectableInNewGroup == true) {

						//set the index based on the selectable's location in the new group
						for (int i = 0; i < currentSelectables.Length; i++) {

							//check if the current selectable is within the array at the ith index
							if (currentSelectables [i] == eventSystem.currentSelectedGameObject.GetComponent<Selectable> ()) {

								//set the currentSelectionIndex
								currentSelectionIndex = i;

								//we have the group index and the selectable index set, and we have the currentSelectable set
								//we can return from the method
								return;

							}

						}

					} else {

						//if the current is not in the group, we need to find a new valid selectable

						//set the selectable array that contains an interactable
						if (potentialCurrentSelectionGroupIndex != -1) {

							//set the currentSelectionGroupIndex
							currentSelectionGroupIndex = potentialCurrentSelectionGroupIndex;

							//set the currentSelectables based on the index returned
							CurrentSelectables = currentSelectablesGroup [currentSelectionGroupIndex];

							//check if the current selectables is the action menu buttons
							if (CurrentSelectables == ActionMenuButtons) {

								if (CurrentSelectables [6].IsInteractable() == true) {

									//Debug.Log ("torpedo current selectables 2 is interactable");

									//set the index
									currentSelectionIndex = 6;

									//set the current selectable to the torpedo toggle
									eventSystem.SetSelectedGameObject (CurrentSelectables [currentSelectionIndex].gameObject);

									//return from the funcction
									return;

								}

							}

						}

					}

				} else {

					//Debug.Log ("current selected is null");

					//if there is no current selectable, we need to find a new valid selectable

					//set the selectable array that contains an interactable
					if (potentialCurrentSelectionGroupIndex != -1) {

						//set the currentSelectionGroupIndex
						currentSelectionGroupIndex = potentialCurrentSelectionGroupIndex;

						//set the currentSelectables based on the index returned
						CurrentSelectables = currentSelectablesGroup [currentSelectionGroupIndex];

						//check if the current selectables is the action menu buttons
						if (CurrentSelectables == ActionMenuButtons) {

							if (CurrentSelectables [6].interactable == true) {

								//Debug.Log ("current selectables 2 is interactable");

								//set the index
								currentSelectionIndex = 6;

								//set the current selectable to the torpedo toggle
								eventSystem.SetSelectedGameObject (CurrentSelectables [currentSelectionIndex].gameObject);

								//return from the funcction
								return;

							}

						}

					}

				}

			} else {

				//the else is that the potential is group zero

				//set the currentSelectionGroupIndex
				currentSelectionGroupIndex = potentialCurrentSelectionGroupIndex;

				//set the currentSelectables based on the index returned
				CurrentSelectables = currentSelectablesGroup [currentSelectionGroupIndex];

			}

			break;

		case UIState.EndTurn:

			//set the current selectables group to match the UI state
			currentSelectablesGroup = EndTurnGroup;

			//find the first array in the group that has an interactable selectable
			potentialCurrentSelectionGroupIndex = FindFirstInteractableArrayIndex (currentSelectablesGroup);

			//Debug.Log ("potentialCurrentSelectionGroupIndex" + potentialCurrentSelectionGroupIndex);

			//check if the potential group index is 0, which would indicate the torpedo dropdown
			if (potentialCurrentSelectionGroupIndex != 0) {

				//if the index is not zero, that means we can't be on the dropdown, because there are no interactable selectables in the dropdown
				//in this case, we want to stay on the toggle that turned on the dropdown
				//check to make sure that current selected object is in the new group

				//only do this if there is a current selectable
				if (EventSystem.current.currentSelectedGameObject != null) {

					for (int i = 0; i < currentSelectablesGroup.Length; i++) {

						//check if the current selectable is within the array at the ith index
						if (currentSelectablesGroup [i].Contains (EventSystem.current.currentSelectedGameObject.GetComponent<Selectable> ())) {

							//set the currentSelectionGroupIndex
							currentSelectionGroupIndex = i;

							//set the currentSelectables based on the index returned
							CurrentSelectables = currentSelectablesGroup [currentSelectionGroupIndex];

							//set the flag
							currentSelectableInNewGroup = true;

							//break out of the for loop
							break;
						}

					}

					//check if the current selectable was found in the new group
					if (currentSelectableInNewGroup == true) {

						//set the index based on the selectable's location in the new group
						for (int i = 0; i < currentSelectables.Length; i++) {

							//check if the current selectable is within the array at the ith index
							if (currentSelectables [i] == eventSystem.currentSelectedGameObject.GetComponent<Selectable> ()) {

								//set the currentSelectionIndex
								currentSelectionIndex = i;

								//we have the group index and the selectable index set, and we have the currentSelectable set
								//we can return from the method
								return;

							}

						}

					} else {

						//if the current is not in the group, we need to find a new valid selectable

						//set the selectable array that contains an interactable
						if (potentialCurrentSelectionGroupIndex != -1) {

							//set the currentSelectionGroupIndex
							currentSelectionGroupIndex = potentialCurrentSelectionGroupIndex;

							//set the currentSelectables based on the index returned
							CurrentSelectables = currentSelectablesGroup [currentSelectionGroupIndex];

							//check if the current selectables is the action menu buttons
							if (CurrentSelectables == ActionMenuButtons) {

								if (CurrentSelectables [10].IsInteractable() == true) {

									//Debug.Log ("torpedo current selectables 2 is interactable");

									//set the index
									currentSelectionIndex = 10;

									//set the current selectable to the torpedo toggle
									eventSystem.SetSelectedGameObject (CurrentSelectables [currentSelectionIndex].gameObject);

									//return from the funcction
									return;

								}

							}

						}

					}

				} else {

					//Debug.Log ("current selected is null");

					//if there is no current selectable, we need to find a new valid selectable

					//set the selectable array that contains an interactable
					if (potentialCurrentSelectionGroupIndex != -1) {

						//set the currentSelectionGroupIndex
						currentSelectionGroupIndex = potentialCurrentSelectionGroupIndex;

						//set the currentSelectables based on the index returned
						CurrentSelectables = currentSelectablesGroup [currentSelectionGroupIndex];

						//check if the current selectables is the action menu buttons
						if (CurrentSelectables == ActionMenuButtons) {

							if (CurrentSelectables [10].interactable == true) {

								//Debug.Log ("current selectables 2 is interactable");

								//set the index
								currentSelectionIndex = 10;

								//set the current selectable to the torpedo toggle
								eventSystem.SetSelectedGameObject (CurrentSelectables [currentSelectionIndex].gameObject);

								//return from the funcction
								return;

							}

						}

					}

				}

			} else {

				//the else is that the potential is group zero

				//set the currentSelectionGroupIndex
				currentSelectionGroupIndex = potentialCurrentSelectionGroupIndex;

				//set the currentSelectables based on the index returned
				CurrentSelectables = currentSelectablesGroup [currentSelectionGroupIndex];

			}

			break;


		case UIState.LoadLocalGame:

			//reset inputs so it doesn't carry over from a previous ui state
			Input.ResetInputAxes();

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
				CurrentSelectables [currentSelectionIndex].transform.parent.parent.GetComponent<ScrollRect> ().verticalNormalizedPosition = 
					1.0f*CurrentSelectables [currentSelectionIndex].transform.parent.parent.GetComponent<ScrollRect> ().verticalNormalizedPosition;

				//fit the object in the scroll window
				//FitCurrentSelectableIntoScrollRect();

			}

			//check if the previousFileIndex is a valid selectable
			if (CurrentSelectables == LoadLocalGameFiles && previousFileIndex < CurrentSelectables.Length) {

				//set the current selectable to the previous index
				//set the Selected object
				eventSystem.SetSelectedGameObject (CurrentSelectables [previousFileIndex].gameObject);

				//store the index of the current selection
				currentSelectionIndex = previousFileIndex;

				//fit the object in the scroll window
				FitCurrentSelectableIntoScrollRect();

				//Debug.Log ("Set selectable to previous index");

				//return from the function
				return;

			} else if (CurrentSelectables == LoadLocalGameFiles && (previousFileIndex - 1)< CurrentSelectables.Length) {

				//else check if one less than the index is valid - for example if we were the highest index and deleted that file

				//set the current selectable to the previous index - 1
				//set the Selected object
				eventSystem.SetSelectedGameObject (CurrentSelectables [previousFileIndex -1].gameObject);

				//store the index of the current selection
				currentSelectionIndex = previousFileIndex - 1;

				//fit the object in the scroll window
				FitCurrentSelectableIntoScrollRect();

				//return from the function
				return;

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


		//note this will only run if we haven't returned out early after finding both indexes and setting the selectables array
		//check if there is an interactable selectable in currentSelectables
		int potentialCurrentSelectionIndex = FindFirstInteractableIndex (CurrentSelectables);


		//Debug.Log ("potentialCurrentSelectionIndex = " + potentialCurrentSelectionIndex);

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
			//Debug.Log("selectableGroup [i].Length = " +selectableGroup [i].Length);

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

	//this function gets the next selectable group
	private Selectable[] GetNextSelectableGroup(bool reverseDirection){

		//we can have special handling for when there is only one element in the group
		//if we are advancing the group with only one group, we can just quickly set the 
		//current selectable to the first or last element, depending on direction
		if (currentSelectablesGroup.Length == 1) {

			//return from the function
			return CurrentSelectables;

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

						//we know there is an interactable selectable, so the potential index is valid

						return currentSelectablesGroup [potentialGroupIndex];
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

						//we know there is an interactable selectable, so the potential index is valid

						return currentSelectablesGroup [potentialGroupIndex];

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

		//if we haven't found a valid return path in the for loops, return the current
		//return from the function
		return CurrentSelectables;

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

	//this function resolves a pointer click to see if it should be blocked or not
	private void ResolvePointerClickBlock(Selectable clickedSelectable){

		//check if the clicked selectable is in the new group
		//only do this if there is a clicked selectable
		if (clickedSelectable != null) {

			for (int i = 0; i < currentSelectablesGroup.Length; i++) {

				//check if the current selectable is within the array at the ith index
				if (currentSelectablesGroup [i].Contains (clickedSelectable)) {

					//if the clicked selectable is in the group, we can allow the set selection to occur

					//set the flag
					blockPointerClickFlag = false;

					//break out of the for loop and exit the function
					return;
				}

			}

			//if we did not find the clickedSelectable in the CurrentSelectablesGroup, that means that what we clicked 
			//on is not something we want to set as the selected object

			//set the block flag
			blockPointerClickFlag = true;

			return;

		}

	}

	//this function returns to a specific selectable
	private void ReturnToSelectable(){

		if (returnSelectable != null) {
			
			eventSystem.SetSelectedGameObject (returnSelectable.gameObject);

		} else {

			eventSystem.SetSelectedGameObject (null);
		}

		CurrentUIState = returnUIState;

		OnUIStateChange.Invoke ();


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

			//remove listener for phasor toggle
			uiManager.GetComponent<PhasorToggle>().phasorToggle.onValueChanged.RemoveListener(PhasorToggleSetUIStateAction);

			//remove listener for phasor toggle
			uiManager.GetComponent<TorpedoToggle>().torpedoToggle.onValueChanged.RemoveListener(TorpedoToggleSetUIStateAction);

			//remove listener for tractor beam toggle
			uiManager.GetComponent<TractorBeamToggle>().tractorBeamToggle.onValueChanged.RemoveListener(TractorBeamToggleSetUIStateAction);

			//remove listener for disengaging tractor beam
			uiManager.GetComponent<TractorBeamMenu>().OnTurnOffTractorBeamToggle.RemoveListener(TractorBeamShipSetUIStateAction);

			//remove listener for use item toggle
			uiManager.GetComponent<UseItemToggle>().useItemToggle.onValueChanged.RemoveListener(UseItemToggleSetUIStateAction);

			//remove listener for crew toggle
			uiManager.GetComponent<CrewToggle>().crewToggle.onValueChanged.RemoveListener(CrewToggleSetUIStateAction);

			//remove listener for cloaking toggle
			uiManager.GetComponent<CloakingDeviceToggle>().cloakingDeviceToggle.onValueChanged.RemoveListener(CloakingToggleSetUIStateAction);

			//remove listener for end turn toggle
			uiManager.GetComponent<EndTurnToggle>().endTurnToggle.onValueChanged.RemoveListener(EndTurnSetUIStateAction);

			//remove listener for disengaging cloaking device
			uiManager.GetComponent<CloakingDeviceMenu>().OnTurnOffCloakingDevice.RemoveListener(CombatUnitSetUIStateAction);
			uiManager.GetComponent<CloakingDeviceMenu>().OnTurnOnCloakingDevice.RemoveListener(CombatUnitSetUIStateAction);

			//remove listeners for end turn drop down
			uiManager.GetComponent<EndTurnDropDown>().OnCancelEndTurnPrompt.RemoveListener(CancelEndTurnPromptAction);
			uiManager.GetComponent<EndTurnDropDown>().OnAcceptEndTurnPrompt.RemoveListener(AcceptEndTurnPromptAction);

			//remove listener for load local game
			uiManager.GetComponent<FileLoadWindow>().OnOpenFileLoadWindow.RemoveListener(OpenLoadLocalGameWindowAction);

			//remove listeners for exiting the file load window back to the main menu
			uiManager.GetComponent<FileLoadWindow>().closeFileLoadWindowButton.onClick.RemoveListener(ReturnToSelectionAction);
			uiManager.GetComponent<FileLoadWindow>().fileLoadCancelButton.onClick.RemoveListener(ReturnToSelectionAction);

			//remove listener for entering the file delete prompt
			uiManager.GetComponent<FileLoadWindow>().OnFileDeleteYesClicked.RemoveListener(OpenFileDeletePromptAction);

			//remove listeners for exiting the file delete prompt back to the load file window
			uiManager.GetComponent<FileDeletePrompt>().OnFileDeleteYesClicked.RemoveListener(StringReturnToFileLoadWindowAction);
			uiManager.GetComponent<FileDeletePrompt>().OnFileDeleteCancelClicked.RemoveListener(OpenLoadLocalGameWindowAction);

			//remove listeners for entering the settings menu
			uiManager.GetComponent<Settings>().settingsMenuButton.onClick.RemoveListener(OpenSettingsWindowAction);

			//remove listeners for exiting the settings menu
			uiManager.GetComponent<Settings>().acceptButton.onClick.RemoveListener(ReturnToSelectionAction);
			uiManager.GetComponent<Settings>().exitButton.onClick.RemoveListener(ReturnToSelectionAction);

			//remove listener for entering the exit game prompt
			uiManager.GetComponent<ExitGamePrompt>().exitGameButton.onClick.RemoveListener(OpenExitGamePromptAction);

			//remove listener for exiting the exit game prompt
			uiManager.GetComponent<ExitGamePrompt>().OnExitGameYesClicked.RemoveListener(ReturnToSelectionAction);
			uiManager.GetComponent<ExitGamePrompt>().OnExitGameCancelClicked.RemoveListener(ReturnToSelectionAction);

			//remove listener for Cutscene ending
			uiManager.GetComponent<CutsceneManager>().OnCloseCutsceneDisplayPanel.RemoveListener(SetInitialCurrentSelectables);

		}

		if (gameManager != null) {

			//remove listener for new or loaded turn
			gameManager.OnNewTurn.RemoveListener (NewTurnSetInitialSelectablesAction);
			gameManager.OnLoadedTurn.RemoveListener (NewTurnSetInitialSelectablesAction);


		}

		if (mouseManager != null) {

			//remove listeners for mouse manager actions
			mouseManager.OnSetTargetedUnit.RemoveListener (SetInitialCurrentSelectables);
			mouseManager.OnClearTargetedUnit.RemoveListener (ClearSetInitialSelectablesAction);
			mouseManager.OnSetSelectedUnit.RemoveListener (SetInitialCurrentSelectables);
			mouseManager.OnClearSelectedUnit.RemoveListener (ClearSetInitialSelectablesAction);

		}

		//remove listener for a pointer click selection
		UISelection.OnSetSelectedGameObject.RemoveListener(SelectableSetSelectionGroupsAction);

		//remove listener for a pointer click
		UISelection.OnClickedSelectable.RemoveListener(PointerClickResolveBlockAction);

	}

}