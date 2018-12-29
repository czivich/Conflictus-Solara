using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class UINavigationMainMenu : MonoBehaviour {

	public static bool blockPointerClickFlag { get; private set;}

	//this will hold the eventSystem in the scene
	public EventSystem eventSystem;

	//uiManager
	private GameObject uiManager;

	//this enum will keep track of the current UI State
	public enum UIState{

		MainMenu,
		NewLocalGame,
		LoadLocalGame,
        NewLANGame,
        JoinLANGame,
		FileDeletePrompt,
		Settings,
		About,
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

				//set the flag
				//blockPointerClickFlag = true;

				//invoke the event
				OnUIStateChange.Invoke();

			}

		}


	}

	//these public arrays are groups of selectables arrays that make up each UI state
	private Selectable[][] MainMenuGroup;
	private Selectable[][] NewLocalGameGroup;
	private Selectable[][] LoadLocalGameGroup;
    private Selectable[][] NewLANGameGroup;
    private Selectable[][] JoinLANGameGroup;
    private Selectable[][] FileDeletePromptGroup;
	private Selectable[][] SettingsGroup;
	private Selectable[][] AboutGroup;
	private Selectable[][] ExitGamePromptGroup;

	//these public arrays will hold the different selectables arrays
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

    public Selectable[] NewLANGameTeams;
    public Selectable[] NewLANGamePlanets;
    public Selectable[] NewLANGameServerButtons;
    public Selectable[] NewLANGameRoomName;
    public Selectable[] NewLANGameGreenPlayerLocalButton;
    public Selectable[] NewLANGameGreenPlayer;
    public Selectable[] NewLANGameRedPlayerLocalButton;
    public Selectable[] NewLANGameRedPlayer;
    public Selectable[] NewLANGamePurplePlayerLocalButton;
    public Selectable[] NewLANGamePurplePlayer;
    public Selectable[] NewLANGameBluePlayerLocalButton;
    public Selectable[] NewLANGameBluePlayer;
    public Selectable[] NewLANGameButtonsRow;

    public Selectable[] JoinLANGameGameSelectButtons;
    public Selectable[] JoinLANGameButtonsRow;

    public Selectable[] FileDeleteButtons;

	public Selectable[] SettingsResolutionDropdown;
	public Selectable[] SettingsResolutionApply;
	public Selectable[] SettingsFullScreenToggle;
	public Selectable[] SettingsMouseZoomInversion;
	public Selectable[] SettingsZoomSensitivity;
	public Selectable[] SettingsScrollSensitivity;
	public Selectable[] SettingsMusicVolume;
	public Selectable[] SettingsSfxVolume;
    public Selectable[] SettingsHexLabelDisplay;
    public Selectable[] SettingsButtonsRow;

	public Selectable[] AboutGameButtons;

	public Selectable[] ExitGameButtons;

	//these hold the sliders so I can reference them
	public Slider ZoomSlider;
	public Slider ScrollSlider;
	public Slider MusicVolumeSlider;
	public Slider SfxVolumeSlider;

	//this selectable is the one we want to return to at the main menu
	private Selectable returnSelectable;

	//this counter tracks whether we need to return to the return selectable
	private int delayReturnToSelectableCount = 0;

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

	//this event announces a successful pointer enter valid selectable
	public UnityEvent OnPointerEnterValidSelectable = new UnityEvent();

	//this event announces a successful pointer click valid selectable
	public UnityEvent OnPointerClickValidSelectable = new UnityEvent();



	//unityActions
	private UnityAction OpenNewLocalGameWindowAction;
	private UnityAction CloseNewLocalGameWindowAction;

	private UnityAction OpenLoadLocalGameWindowAction;
	private UnityAction CloseLoadLocalGameWindowAction;

	private UnityAction<string> OpenFileDeletePromptAction;
	private UnityAction<string> StringReturnToFileLoadWindowAction;

    private UnityAction OpenNewLANGameWindowAction;
    private UnityAction CloseNewLANGameWindowAction;

    private UnityAction OpenLANGameListWindowAction;
    private UnityAction CloseLANGameListWindowAction;

    private UnityAction OpenSettingsWindowAction;
	private UnityAction CloseSettingsWindowAction;

	private UnityAction OpenAboutPanelAction;
	private UnityAction CloseAboutPanelAction;

	private UnityAction OpenExitGamePromptAction;
	private UnityAction CloseExitGamePromptAction;

	private UnityAction SelectableSetNavigationRulesAction;
	private UnityAction<Selectable> SelectableSetSelectionGroupsAction;

	private UnityAction<string> InputFieldEndEditIgnoreEscapeAction;
	private UnityAction<Selectable> PointerClickResolveBlockAction;

	//this action catches a redirect from a clicked up/down button to the selectable count
	private UnityAction<Selectable> ButtonRedirectSetSelectedObjectAction;

	//this action listens for onpointerenter events
	private UnityAction<Selectable> OnPointerEnterTestAction;

	//this action listens for onpointerenter events
	private UnityAction<Selectable> OnPointerEnterNonGroupTestAction;

	//this action listens for onpointerclick events
	private UnityAction<Selectable> OnPointerClickNonGroupTestAction;

	// Use this for initialization
	public void Init () {

		//get the uiManager
		uiManager = GameObject.FindGameObjectWithTag("UIManager");

		//set the actions
		SetUnityActions ();

		//add event listeners
		AddEventListeners();

		//defome the selectable groups
		DefineSelectablesGroups();

		//set the initial UIState
		CurrentUIState = UIState.MainMenu;

		//invoke the event to start, since it won't fire because the enum defaults to the mainMenu
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
			if (eventSystem.currentSelectedGameObject == null || eventSystem.currentSelectedGameObject != CurrentSelectables [currentSelectionIndex].gameObject) {

				//the else check is if we have nothing selected or if we have something selected that's not our selectable in memory

				//now check if our memory isn't null
				if (CurrentSelectables != null && CurrentSelectables [currentSelectionIndex] != null) {

					//check if the object in memory is either non-interactable or active - ie it is an invalid selection
					if (CurrentSelectables [currentSelectionIndex].IsInteractable () == false || CurrentSelectables [currentSelectionIndex].IsActive () == false) {

						//check if we've got shift held for backwards cycling
						if (Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift)) {

							//advance to the next group backwards since our current memory is invalid
							AdvanceSelectableGroup (true);

							return;

						} else{

							//advance to the next group forward since our current memory is invalid
							AdvanceSelectableGroup (false);

							return;

						}

					} else {
						
						//Debug.Log ("test6");

						//check if we are in a dropdown - if so, we don't want to set the selected object
						if (CurrentSelectables [currentSelectionIndex].gameObject.GetComponent<TMP_Dropdown> () == true) {

							//do nothing

							//invoke the pointer enter event to trigger a sound
							OnPointerEnterValidSelectable.Invoke();

						} else {
							//set the selected object to the one in memory because the memory object is valid
							eventSystem.SetSelectedGameObject (CurrentSelectables [currentSelectionIndex].gameObject);

							return;

						}

					}

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

			} else if(CurrentUIState == UIState.NewLocalGame && CurrentSelectables == NewLocalGamePlanets){

				uiManager.GetComponent<ConfigureLocalGameWindow> ().decreasePlanetsButton.onClick.Invoke();


			}
            else if (CurrentUIState == UIState.NewLANGame && CurrentSelectables == NewLANGamePlanets)
            {

                uiManager.GetComponent<NewLANGameWindow>().decreasePlanetsButton.onClick.Invoke();


            }
            else if(CurrentUIState == UIState.Settings && CurrentSelectables == SettingsResolutionDropdown){

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
			if (eventSystem.currentSelectedGameObject == null || eventSystem.currentSelectedGameObject != CurrentSelectables [currentSelectionIndex].gameObject) {

				//the else check is if we have nothing selected or if we have something selected that's not our selectable in memory

				//now check if our memory isn't null
				if (CurrentSelectables != null && CurrentSelectables [currentSelectionIndex] != null) {

					//check if the object in memory is either non-interactable or active - ie it is an invalid selection
					if (CurrentSelectables [currentSelectionIndex].IsInteractable () == false || CurrentSelectables [currentSelectionIndex].IsActive () == false) {

						//check if we've got shift held for backwards cycling
						if (Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift)) {

							//advance to the next group backwards since our current memory is invalid
							AdvanceSelectableGroup (true);

							return;

						} else{

							//advance to the next group forward since our current memory is invalid
							AdvanceSelectableGroup (false);

							return;

						}

					} else {
						
						//Debug.Log ("test6");

						//check if we are in a dropdown - if so, we don't want to set the selected object
						if (CurrentSelectables [currentSelectionIndex].gameObject.GetComponent<TMP_Dropdown> () == true) {

							//do nothing

							//invoke the pointer enter event to trigger a sound
							OnPointerEnterValidSelectable.Invoke();

						} else {
							//set the selected object to the one in memory because the memory object is valid
							eventSystem.SetSelectedGameObject (CurrentSelectables [currentSelectionIndex].gameObject);

							return;

						}

					}

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

			}else if(CurrentUIState == UIState.NewLocalGame && CurrentSelectables == NewLocalGamePlanets){

				uiManager.GetComponent<ConfigureLocalGameWindow> ().increasePlanetsButton.onClick.Invoke();

			}
            else if (CurrentUIState == UIState.NewLANGame && CurrentSelectables == NewLANGamePlanets)
            {

                uiManager.GetComponent<NewLANGameWindow>().increasePlanetsButton.onClick.Invoke();

            }
            else if (CurrentUIState == UIState.Settings && CurrentSelectables == SettingsResolutionDropdown){

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
			if (eventSystem.currentSelectedGameObject == null || eventSystem.currentSelectedGameObject != CurrentSelectables [currentSelectionIndex].gameObject) {

				//the else check is if we have nothing selected or if we have something selected that's not our selectable in memory

				//now check if our memory isn't null
				if (CurrentSelectables != null && CurrentSelectables [currentSelectionIndex] != null) {

					//check if the object in memory is either non-interactable or active - ie it is an invalid selection
					if (CurrentSelectables [currentSelectionIndex].IsInteractable () == false || CurrentSelectables [currentSelectionIndex].IsActive () == false) {

						//check if we've got shift held for backwards cycling
						if (Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift)) {

							//advance to the next group backwards since our current memory is invalid
							AdvanceSelectableGroup (true);

							return;

						} else{

							//advance to the next group forward since our current memory is invalid
							AdvanceSelectableGroup (false);

							return;

						}

					} else {

						//set the selected object to the one in memory because the memory object is valid
						eventSystem.SetSelectedGameObject (CurrentSelectables [currentSelectionIndex].gameObject);

						return;

					}

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
			if (eventSystem.currentSelectedGameObject == null || eventSystem.currentSelectedGameObject != CurrentSelectables [currentSelectionIndex].gameObject) {

				//the else check is if we have nothing selected or if we have something selected that's not our selectable in memory

				//now check if our memory isn't null
				if (CurrentSelectables != null && CurrentSelectables [currentSelectionIndex] != null) {

					//check if the object in memory is either non-interactable or active - ie it is an invalid selection
					if (CurrentSelectables [currentSelectionIndex].IsInteractable () == false || CurrentSelectables [currentSelectionIndex].IsActive () == false) {

						//check if we've got shift held for backwards cycling
						if (Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift)) {

							//advance to the next group backwards since our current memory is invalid
							AdvanceSelectableGroup (true);

							return;

						} else{

							//advance to the next group forward since our current memory is invalid
							AdvanceSelectableGroup (false);

							return;

						}

					} else {

						//set the selected object to the one in memory because the memory object is valid
						eventSystem.SetSelectedGameObject (CurrentSelectables [currentSelectionIndex].gameObject);

						return;

					}

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
			if (CurrentSelectables[currentSelectionIndex] != null && 
				CurrentSelectables[currentSelectionIndex].GetComponent<TMP_Dropdown> () == true &&
				CurrentSelectables[currentSelectionIndex].transform.Find ("Dropdown List") != null) {

				//hide the list
				CurrentSelectables[currentSelectionIndex].GetComponent<TMP_Dropdown> ().Hide();


			} else if (eventSystem.currentSelectedGameObject == null || eventSystem.currentSelectedGameObject != CurrentSelectables [currentSelectionIndex].gameObject) {

				//the else check is if we have nothing selected or if we have something selected that's not our selectable in memory

				//now check if our memory isn't null
				if (CurrentSelectables != null && CurrentSelectables [currentSelectionIndex] != null) {

					//check if the object in memory is either non-interactable or active - ie it is an invalid selection
					if (CurrentSelectables [currentSelectionIndex].IsInteractable () == false || CurrentSelectables [currentSelectionIndex].IsActive () == false) {

						//check if we've got shift held for backwards cycling
						if (Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift)) {

							//advance to the next group backwards since our current memory is invalid
							AdvanceSelectableGroup (true);

							return;

						} else{

							//advance to the next group forward since our current memory is invalid
							AdvanceSelectableGroup (false);

							return;

						}

					} else {

						//set the selected object to the one in memory because the memory object is valid
						eventSystem.SetSelectedGameObject (CurrentSelectables [currentSelectionIndex].gameObject);

						return;

					}

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

			} else if (CurrentUIState == UIState.NewLANGame && ignoreEscape == false) {

                //cancel out of the menu
                uiManager.GetComponent<NewLANGameWindow>().cancelButton.onClick.Invoke();

            }
            else if (CurrentUIState == UIState.JoinLANGame && ignoreEscape == false)
            {

                //cancel out of the menu
                uiManager.GetComponent<LANGameList>().cancelButton.onClick.Invoke();

            }
            else if (CurrentUIState == UIState.Settings) {

				//cancel out of the menu
				uiManager.GetComponent<Settings> ().exitButton.onClick.Invoke ();

			} else if (CurrentUIState == UIState.About) {

				//cancel out of the menu
				uiManager.GetComponent<About> ().cancelButton.onClick.Invoke ();

			} else if (CurrentUIState == UIState.ExitGamePrompt) {

				//cancel out of the menu
				uiManager.GetComponent<ExitGamePrompt> ().exitGameCancelButton.onClick.Invoke ();

			} else if (CurrentUIState == UIState.MainMenu) {

				//cancel out of the menu and launch the exit prompt
				uiManager.GetComponent<ExitGamePrompt> ().exitGameButton.onClick.Invoke ();

			}

		}

		//check if we are pressing the enter key
		if (Input.GetKeyDown (KeyCode.Return) == true) {

			//check if we are currently on a file for load
			if (CurrentUIState == UIState.LoadLocalGame && CurrentSelectables == LoadLocalGameFiles) {

				//call the load game button on-click
				uiManager.GetComponent<FileLoadWindow>().fileLoadYesButton.onClick.Invoke();

			}

			//check if we are in a dropdown - if so, we don't want to set the selected object
			else if (CurrentSelectables [currentSelectionIndex].gameObject.GetComponent<TMP_Dropdown> () == true) {

				//invoke the pointer click event to trigger a sound
				OnPointerClickValidSelectable.Invoke ();


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

		OpenNewLocalGameWindowAction = () => {CurrentUIState = UIState.NewLocalGame;};

		CloseNewLocalGameWindowAction = () => {

			CurrentUIState = UIState.MainMenu;

			//returnUIState = UIState.Selection;
			returnSelectable = MainMenuButtons[0];

			//returnSelectable = null;
			delayReturnToSelectableCount = 2;

		};


		OpenLoadLocalGameWindowAction = () => {CurrentUIState = UIState.LoadLocalGame;};

		CloseLoadLocalGameWindowAction = () => {

			CurrentUIState = UIState.MainMenu;

			//returnUIState = UIState.Selection;
			returnSelectable = MainMenuButtons[1];

			//returnSelectable = null;
			delayReturnToSelectableCount = 2;

		};


		OpenFileDeletePromptAction = (fileName) => {CurrentUIState = UIState.FileDeletePrompt;};

		StringReturnToFileLoadWindowAction = (fileName) => {delayLoadFilesWindowCount = 2;};


        OpenNewLANGameWindowAction = () => { CurrentUIState = UIState.NewLANGame; };

        CloseNewLANGameWindowAction = () => {

            CurrentUIState = UIState.MainMenu;

            //returnUIState = UIState.Selection;
            returnSelectable = MainMenuButtons[2];

            //returnSelectable = null;
            delayReturnToSelectableCount = 2;

        };

        OpenLANGameListWindowAction = () => { CurrentUIState = UIState.JoinLANGame; };

        CloseLANGameListWindowAction = () => {

            CurrentUIState = UIState.MainMenu;

            //returnUIState = UIState.Selection;
            returnSelectable = MainMenuButtons[4];

            //returnSelectable = null;
            delayReturnToSelectableCount = 2;

        };

        OpenSettingsWindowAction = () => {CurrentUIState = UIState.Settings;}; 

		CloseSettingsWindowAction = () => {

			CurrentUIState = UIState.MainMenu;

			//returnUIState = UIState.Selection;
			returnSelectable = MainMenuButtons[8];

			//returnSelectable = null;
			delayReturnToSelectableCount = 2;

		};

		OpenAboutPanelAction = () => {CurrentUIState = UIState.About;};

		CloseAboutPanelAction = () => {

			CurrentUIState = UIState.MainMenu;

			//returnUIState = UIState.Selection;
			returnSelectable = MainMenuButtons[9];

			//returnSelectable = null;
			delayReturnToSelectableCount = 2;

		};


		OpenExitGamePromptAction = () => {CurrentUIState = UIState.ExitGamePrompt;};

		CloseExitGamePromptAction = () => {

			CurrentUIState = UIState.MainMenu;

			//returnUIState = UIState.Selection;
			returnSelectable = MainMenuButtons[10];

			//returnSelectable = null;
			delayReturnToSelectableCount = 2;

		};


		SelectableSetNavigationRulesAction = () => {SetNavigationRulesForSelectables();};

		SelectableSetSelectionGroupsAction = (selectable) => {SetSelectionIndexFromPointerClick (selectable);};


		InputFieldEndEditIgnoreEscapeAction = (eventString) => {ignoreEscape = true;};


		PointerClickResolveBlockAction = (selectable) => {ResolvePointerClickBlock (selectable);};

		//this action sets the group index and selection index to the selectable that we redirect to, then sets the selected object to it
		ButtonRedirectSetSelectedObjectAction = (redirectSelectable) => {

			//check if the redirect selectable is interactable and enabled
			if(redirectSelectable.IsInteractable() == true && redirectSelectable.IsActive() == true){

				//set the indexes as if we had clicked the redirect target
				SetSelectionIndexFromPointerClick(redirectSelectable);

				//set the current selected game object to the redirected target
				eventSystem.SetSelectedGameObject(redirectSelectable.gameObject);

			}

		};

		//this action checks if the selectable is valid
		OnPointerEnterTestAction = (testSelectable) => {CheckOnPointerEnter(testSelectable);};

		//this action checks if the selectable is valid
		OnPointerEnterNonGroupTestAction = (testSelectable) => {CheckOnPointerEnterNonGroup(testSelectable);};

		//this action checks if the selectable is valid
		OnPointerClickNonGroupTestAction = (testSelectable) => {CheckOnPointerClickNonGroup(testSelectable);};

	}

	//this function adds event listeners
	private void AddEventListeners(){

		//add listener for UIState change
		OnUIStateChange.AddListener(SetInitialCurrentSelectables);

		//add listener for new local game
		uiManager.GetComponent<ConfigureLocalGameWindow>().newLocalGameButton.onClick.AddListener(OpenNewLocalGameWindowAction);

		//add listeners for exiting the new local game window to the main menu
		uiManager.GetComponent<ConfigureLocalGameWindow>().cancelButton.onClick.AddListener(CloseNewLocalGameWindowAction);
		uiManager.GetComponent<ConfigureLocalGameWindow>().exitWindowButton.onClick.AddListener(CloseNewLocalGameWindowAction);

		//add listener for load local game
		uiManager.GetComponent<FileLoadWindow>().OnOpenFileLoadWindow.AddListener(OpenLoadLocalGameWindowAction);

		//add listeners for exiting the file load window back to the main menu
		uiManager.GetComponent<FileLoadWindow>().closeFileLoadWindowButton.onClick.AddListener(CloseLoadLocalGameWindowAction);
		uiManager.GetComponent<FileLoadWindow>().fileLoadCancelButton.onClick.AddListener(CloseLoadLocalGameWindowAction);

		//add listener for entering the file delete prompt
		uiManager.GetComponent<FileLoadWindow>().OnFileDeleteYesClicked.AddListener(OpenFileDeletePromptAction);

		//add listeners for exiting the file delete prompt back to the load file window
		uiManager.GetComponent<FileDeletePrompt>().OnFileDeleteYesClicked.AddListener(StringReturnToFileLoadWindowAction);
		uiManager.GetComponent<FileDeletePrompt>().OnFileDeleteCancelClicked.AddListener(OpenLoadLocalGameWindowAction);

        //add listener for new LAN game
        uiManager.GetComponent<NewLANGameWindow>().OnOpenPanel.AddListener(OpenNewLANGameWindowAction);

        //add listeners for exiting the new LAN game window to the main menu
        uiManager.GetComponent<NewLANGameWindow>().cancelButton.onClick.AddListener(CloseNewLANGameWindowAction);
        uiManager.GetComponent<NewLANGameWindow>().exitWindowButton.onClick.AddListener(CloseNewLANGameWindowAction);

        //add listeners for join LAN Game
        uiManager.GetComponent<LANGameList>().OnOpenPanel.AddListener(OpenLANGameListWindowAction);
        uiManager.GetComponent<LANGameList>().cancelButton.onClick.AddListener(CloseLANGameListWindowAction);
        uiManager.GetComponent<LANGameList>().closeButton.onClick.AddListener(CloseLANGameListWindowAction);

        //add listener for adding a LAN game to list
        uiManager.GetComponent<LANGameList>().OnAddedLANGameListItem.AddListener(SetLANGameListSelectables);

        //change this once the LAN window goes to a lobby
        uiManager.GetComponent<NewLANGameWindow>().createGameButton.onClick.AddListener(CloseNewLANGameWindowAction);

        //add listeners for entering the settings menu
        uiManager.GetComponent<Settings>().OnSettingsWindowOpened.AddListener(OpenSettingsWindowAction);

		//add listeners for exiting the settings menu
		uiManager.GetComponent<Settings>().acceptButton.onClick.AddListener(CloseSettingsWindowAction);
		uiManager.GetComponent<Settings>().exitButton.onClick.AddListener(CloseSettingsWindowAction);

		//add listeners for the about panel
		uiManager.GetComponent<About>().OnOpenAboutPanel.AddListener(OpenAboutPanelAction);
		uiManager.GetComponent<About>().OnCloseAboutPanel.AddListener(CloseAboutPanelAction);

		//add listener for entering the exit game prompt
		uiManager.GetComponent<ExitGamePrompt>().OnExitGamePromptOpened.AddListener(OpenExitGamePromptAction);

		//add listener for exiting the exit game prompt
		uiManager.GetComponent<ExitGamePrompt>().OnExitGameYesClicked.AddListener(CloseExitGamePromptAction);
		uiManager.GetComponent<ExitGamePrompt>().OnExitGameCancelClicked.AddListener(CloseExitGamePromptAction);

		//add listeners telling us the input fields just ended edit, so we can ignore hitting escape
		uiManager.GetComponent<ConfigureLocalGameWindow> ().greenPlayerInputField.onEndEdit.AddListener (InputFieldEndEditIgnoreEscapeAction);
		uiManager.GetComponent<ConfigureLocalGameWindow> ().redPlayerInputField.onEndEdit.AddListener (InputFieldEndEditIgnoreEscapeAction);
		uiManager.GetComponent<ConfigureLocalGameWindow> ().purplePlayerInputField.onEndEdit.AddListener (InputFieldEndEditIgnoreEscapeAction);
		uiManager.GetComponent<ConfigureLocalGameWindow> ().bluePlayerInputField.onEndEdit.AddListener (InputFieldEndEditIgnoreEscapeAction);

		//add listener for setting selectables
		OnSelectablesChange.AddListener(SelectableSetNavigationRulesAction);

		//add listener for a pointer click selection
		UISelection.OnSetSelectedGameObject.AddListener(SelectableSetSelectionGroupsAction);

		//add listener for a pointer click
		UISelection.OnClickedSelectable.AddListener(PointerClickResolveBlockAction);

		//add listener for the redirect click
		UIButtonSelectionRedirect.OnClickedButtonForRedirect.AddListener(ButtonRedirectSetSelectedObjectAction);

		//add listener to the UI selection pointer enter
		UISelection.OnPointerEnterSelectable.AddListener(OnPointerEnterTestAction);

		//add listener to the UI selection pointer enter
		UISelectionNonGroup.OnPointerEnterSelectable.AddListener(OnPointerEnterNonGroupTestAction);

		//add listener to the UI selection pointer click
		UISelectionNonGroup.OnPointerClickSelectable.AddListener(OnPointerClickNonGroupTestAction);

	}

	//this function defines the selectables groups
	private void DefineSelectablesGroups(){

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

        NewLANGameGroup = new Selectable[13][];
        NewLANGameGroup[0] = NewLANGameTeams;
        NewLANGameGroup[1] = NewLANGamePlanets;
        NewLANGameGroup[2] = NewLANGameServerButtons;
        NewLANGameGroup[3] = NewLANGameRoomName;
        NewLANGameGroup[4] = NewLANGameGreenPlayerLocalButton;
        NewLANGameGroup[5] = NewLANGameGreenPlayer;
        NewLANGameGroup[6] = NewLANGameRedPlayerLocalButton;
        NewLANGameGroup[7] = NewLANGameRedPlayer;
        NewLANGameGroup[8] = NewLANGamePurplePlayerLocalButton;
        NewLANGameGroup[9] = NewLANGamePurplePlayer;
        NewLANGameGroup[10] = NewLANGameBluePlayerLocalButton;
        NewLANGameGroup[11] = NewLANGameBluePlayer;
        NewLANGameGroup[12] = NewLANGameButtonsRow;

        JoinLANGameGroup = new Selectable[2][];
        JoinLANGameGroup[0] = JoinLANGameGameSelectButtons;
        JoinLANGameGroup[1] = JoinLANGameButtonsRow;


        SettingsGroup = new Selectable[10][];
		SettingsGroup[0] = SettingsResolutionDropdown;
		SettingsGroup[1] = SettingsResolutionApply;
		SettingsGroup[2] = SettingsFullScreenToggle;
		SettingsGroup[3] = SettingsMouseZoomInversion;
		SettingsGroup[4] = SettingsZoomSensitivity;
		SettingsGroup[5] = SettingsScrollSensitivity;
		SettingsGroup[6] = SettingsMusicVolume;
		SettingsGroup[7] = SettingsSfxVolume;
        SettingsGroup[8] = SettingsHexLabelDisplay;
		SettingsGroup[9] = SettingsButtonsRow;

		AboutGroup = new Selectable[1][];
		AboutGroup [0] = AboutGameButtons;

		ExitGamePromptGroup = new Selectable[1][];
		ExitGamePromptGroup [0] = ExitGameButtons;

		//Debug.Log ("Define Selectables");

	}

	//this function defines the navigation rules for the currentSelectables
	private void SetNavigationRulesForSelectables(){
		
		//define rules based on what the current selectable is
		if (CurrentSelectables == MainMenuButtons) {

			horizontalCycling = false;
			verticalCycling = true;
			selectablesWrap = true;

		}else if (CurrentSelectables == NewLocalGameTeams) {

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

		}
        else if (CurrentSelectables == NewLANGameTeams)
        {

            horizontalCycling = true;
            verticalCycling = false;
            selectablesWrap = true;

        }
        else if (CurrentSelectables == NewLANGamePlanets)
        {

            horizontalCycling = false;
            verticalCycling = false;
            selectablesWrap = true;

        }
        else if (CurrentSelectables == NewLANGameServerButtons)
        {

            horizontalCycling = true;
            verticalCycling = false;
            selectablesWrap = true;

        }
        else if (CurrentSelectables == NewLANGameRoomName)
        {

            horizontalCycling = false;
            verticalCycling = false;
            selectablesWrap = true;

        }
        else if (CurrentSelectables == NewLANGameGreenPlayerLocalButton)
        {

            horizontalCycling = false;
            verticalCycling = false;
            selectablesWrap = false;

        }
        else if (CurrentSelectables == NewLANGameGreenPlayer)
        {

            horizontalCycling = false;
            verticalCycling = false;
            selectablesWrap = true;

        }
        else if (CurrentSelectables == NewLANGameRedPlayerLocalButton)
        {

            horizontalCycling = false;
            verticalCycling = false;
            selectablesWrap = false;

        }
        else if (CurrentSelectables == NewLANGameRedPlayer)
        {

            horizontalCycling = false;
            verticalCycling = false;
            selectablesWrap = true;

        }
        else if (CurrentSelectables == NewLANGamePurplePlayerLocalButton)
        {

            horizontalCycling = false;
            verticalCycling = false;
            selectablesWrap = false;

        }
        else if (CurrentSelectables == NewLANGamePurplePlayer)
        {

            horizontalCycling = false;
            verticalCycling = false;
            selectablesWrap = true;

        }
        else if (CurrentSelectables == NewLANGameBluePlayerLocalButton)
        {

            horizontalCycling = false;
            verticalCycling = false;
            selectablesWrap = false;

        }
        else if (CurrentSelectables == NewLANGameBluePlayer)
        {

            horizontalCycling = false;
            verticalCycling = false;
            selectablesWrap = true;

        }
        else if (CurrentSelectables == NewLANGameButtonsRow)
        {

            horizontalCycling = true;
            verticalCycling = false;
            selectablesWrap = true;

        }
        else if (CurrentSelectables == JoinLANGameGameSelectButtons)
        {

            horizontalCycling = false;
            verticalCycling = true;
            selectablesWrap = true;

        }
        else if (CurrentSelectables == JoinLANGameButtonsRow)
        {

            horizontalCycling = true;
            verticalCycling = false;
            selectablesWrap = true;

        }
        else if (CurrentSelectables == SettingsResolutionDropdown){

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

		}else if (CurrentSelectables == SettingsHexLabelDisplay)
        {

            horizontalCycling = false;
            verticalCycling = false;
            selectablesWrap = false;

        } else if (CurrentSelectables == SettingsButtonsRow){

			horizontalCycling = true;
			verticalCycling = false;
			selectablesWrap = true;

		} else if (CurrentSelectables == AboutGameButtons){

			horizontalCycling = false;
			verticalCycling = false;
			selectablesWrap = false;

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

		case UIState.MainMenu:

			//set the current selectables group to match the UI state
			currentSelectablesGroup = MainMenuGroup;


			//check if the current selectable is within the new selectables group
			//if the current selectable is within the new group, we don't want set the selectables to the first interactable option,
			//we want to leave it where it is
			//but we will need to know what the group index is within the context of the new group

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

				//if there is no current selectable, we need to find a new valid selectable

				//check if our last selectable is valid
				if (CurrentSelectables != null && CurrentSelectables [currentSelectionIndex] != null &&
					CurrentSelectables [currentSelectionIndex].IsActive () == true && CurrentSelectables [currentSelectionIndex].IsInteractable () == true) {

					//Debug.Log ("valid previous");

					//if the last selectable is still valid, set the current to the last
					eventSystem.SetSelectedGameObject(CurrentSelectables [currentSelectionIndex].gameObject);

					return;
				}


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
			DefineSelectablesGroups ();

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
				CurrentSelectables [0].transform.parent.parent.GetComponent<ScrollRect> ().verticalNormalizedPosition = 
					1.0f*CurrentSelectables [0].transform.parent.parent.GetComponent<ScrollRect> ().verticalNormalizedPosition;

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

            case UIState.NewLANGame:

                //set the current selectables group to match the UI state
                currentSelectablesGroup = NewLANGameGroup;

                //find the first array in the group that has an interactable selectable
                potentialCurrentSelectionGroupIndex = FindFirstInteractableArrayIndex(currentSelectablesGroup);

                //set the selectable array that contains an interactable
                if (potentialCurrentSelectionGroupIndex != -1)
                {

                    //set the currentSelectionGroupIndex
                    currentSelectionGroupIndex = potentialCurrentSelectionGroupIndex;

                    //set the currentSelectables based on the index returned
                    CurrentSelectables = currentSelectablesGroup[currentSelectionGroupIndex];

                }

                break;

            case UIState.JoinLANGame:

                //need to get the game files after the window opens
                SetLANGameListSelectables();


                //find the first array in the group that has an interactable selectable
                potentialCurrentSelectionGroupIndex = FindFirstInteractableArrayIndex(currentSelectablesGroup);

                //set the selectable array that contains an interactable
                if (potentialCurrentSelectionGroupIndex != -1)
                {

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

		case UIState.About:

			//set the current selectables group to match the UI state
			currentSelectablesGroup = AboutGroup;

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

			//Debug.Log ("SetSelectedObject SetInitialCurrentSelectables " + CurrentSelectables [potentialCurrentSelectionIndex].gameObject.name);

			//store the index of the current selection
			currentSelectionIndex = potentialCurrentSelectionIndex;

		}

	}

    //this function sets the selectables in the LAN game list
    private void SetLANGameListSelectables()
    {

        //set the size of the JoinLANGameGameSelectButtons array
        JoinLANGameGameSelectButtons = new Selectable[uiManager.GetComponent<LANGameList>().gameListItemParent.transform.childCount];

        //loop through the list
        for (int i = 0; i < uiManager.GetComponent<LANGameList>().gameListItemParent.transform.childCount; i++)
        {

            JoinLANGameGameSelectButtons[i] = uiManager.GetComponent<LANGameList>().gameListItemParent.transform.GetChild(i).GetComponentInChildren<Selectable>();

        }

        //redefine Selectable Groups since we have dynamically changed the LoadLocalGameFiles
        DefineSelectablesGroups();


        //set the current selectables group to match the UI state
        currentSelectablesGroup = JoinLANGameGroup;

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
		if (clickedSelectable != null && currentSelectablesGroup != null) {

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

			//check if the return selectable is interactable and active
			if (returnSelectable.IsInteractable () == true && returnSelectable.IsActive () == true) {

				eventSystem.SetSelectedGameObject (returnSelectable.gameObject);

			} else {

				//find the returnSelectable in the group
				for (int i = 0; i < currentSelectablesGroup.Length; i++) {

					if (currentSelectablesGroup [i].Contains (returnSelectable)) {

						//Debug.Log ("found return selectable group");

						for (int j = 0; j < currentSelectablesGroup [i].Length; j++) {

							if (currentSelectablesGroup [i] [j] == returnSelectable) {

								//Debug.Log ("found return selectable");

								//set the selectable to the first available after the return selectable
								eventSystem.SetSelectedGameObject (currentSelectablesGroup [i] [FindFirstInteractableIndex (currentSelectablesGroup [i])].gameObject);

							}

						}

					}

				}
			}

		} else {

			eventSystem.SetSelectedGameObject (null);
		}

		//CurrentUIState = returnUIState;

		OnUIStateChange.Invoke ();

	}

	//this function checks if a selectable is in the current group
	private bool SelectableInGroup(Selectable testSelectable){

		//find the returnSelectable in the group
		for (int i = 0; i < currentSelectablesGroup.Length; i++) {

			if (currentSelectablesGroup [i].Contains (testSelectable)) {

				return true;

			}

		}

		//if we didn't return from the for loop, we can return false
		return false;

	}

	//this function checks if the passed selectable is in the current group, and if it is valid
	private bool CheckValidSelectable(Selectable testSelectable){

		//check if the selectable is in the group, is active, and is interactable
		if (SelectableInGroup (testSelectable) == true && testSelectable.IsInteractable () == true && testSelectable.IsActive() == true) {

			return true;

		} else {

			return false;

		}

	}

	//this function checks if a OnPointerEnter is valid, and if so, fires an event
	private void CheckOnPointerEnter(Selectable testSelectable){

		//check validity
		if (CheckValidSelectable (testSelectable) == true) {

			OnPointerEnterValidSelectable.Invoke ();

		}

	}

	//this function checks if a OnPointerEnter is valid, and if so, fires an event
	private void CheckOnPointerEnterNonGroup(Selectable testSelectable){

		//check validity
		if (testSelectable.IsInteractable () == true && testSelectable.IsActive() == true) {

			OnPointerEnterValidSelectable.Invoke ();

		}

	}

	//this function checks if a OnPointerEnter is valid, and if so, fires an event
	private void CheckOnPointerClickNonGroup(Selectable testSelectable){

		//fire event
		OnPointerClickValidSelectable.Invoke ();

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
			
			//remove listener for new local game
			uiManager.GetComponent<ConfigureLocalGameWindow> ().newLocalGameButton.onClick.RemoveListener (OpenNewLocalGameWindowAction);

			//remove listeners for exiting the new local game window to the main menu
			uiManager.GetComponent<ConfigureLocalGameWindow>().cancelButton.onClick.RemoveListener(CloseNewLocalGameWindowAction);
			uiManager.GetComponent<ConfigureLocalGameWindow>().exitWindowButton.onClick.RemoveListener(CloseNewLocalGameWindowAction);

			//remove listener for load local game
			uiManager.GetComponent<FileLoadWindow>().OnOpenFileLoadWindow.RemoveListener(OpenLoadLocalGameWindowAction);

			//remove listeners for exiting the file load window back to the main menu
			uiManager.GetComponent<FileLoadWindow>().closeFileLoadWindowButton.onClick.RemoveListener(CloseLoadLocalGameWindowAction);
			uiManager.GetComponent<FileLoadWindow>().fileLoadCancelButton.onClick.RemoveListener(CloseLoadLocalGameWindowAction);

			//remove listener for entering the file delete prompt
			uiManager.GetComponent<FileLoadWindow>().OnFileDeleteYesClicked.RemoveListener(OpenFileDeletePromptAction);

			//remove listeners for exiting the file delete prompt back to the load file window
			uiManager.GetComponent<FileDeletePrompt>().OnFileDeleteYesClicked.RemoveListener(StringReturnToFileLoadWindowAction);
			uiManager.GetComponent<FileDeletePrompt>().OnFileDeleteCancelClicked.RemoveListener(OpenLoadLocalGameWindowAction);

            //remove listener for new LAN game
            uiManager.GetComponent<NewLANGameWindow>().OnOpenPanel.RemoveListener(OpenNewLANGameWindowAction);

            //remove listeners for exiting the new LAN game window to the main menu
            uiManager.GetComponent<NewLANGameWindow>().cancelButton.onClick.RemoveListener(CloseNewLANGameWindowAction);
            uiManager.GetComponent<NewLANGameWindow>().exitWindowButton.onClick.RemoveListener(CloseNewLANGameWindowAction);

            //remove listeners for join LAN Game
            uiManager.GetComponent<LANGameList>().OnOpenPanel.RemoveListener(OpenLANGameListWindowAction);
            uiManager.GetComponent<LANGameList>().cancelButton.onClick.RemoveListener(CloseLANGameListWindowAction);
            uiManager.GetComponent<LANGameList>().closeButton.onClick.RemoveListener(CloseLANGameListWindowAction);

            //remove listener for adding a LAN game to list
            uiManager.GetComponent<LANGameList>().OnAddedLANGameListItem.RemoveListener(SetLANGameListSelectables);

            //remove this once the LAN window goes to a lobby
            uiManager.GetComponent<NewLANGameWindow>().createGameButton.onClick.RemoveListener(CloseNewLANGameWindowAction);

            //remove listeners for entering the settings menu
            uiManager.GetComponent<Settings>().OnSettingsWindowOpened.RemoveListener(OpenSettingsWindowAction);

			//remove listeners for exiting the settings menu
			uiManager.GetComponent<Settings>().acceptButton.onClick.RemoveListener(CloseSettingsWindowAction);
			uiManager.GetComponent<Settings>().exitButton.onClick.RemoveListener(CloseSettingsWindowAction);

			//remove listeners for the about panel
			uiManager.GetComponent<About>().OnOpenAboutPanel.RemoveListener(OpenAboutPanelAction);
			uiManager.GetComponent<About>().OnCloseAboutPanel.RemoveListener(CloseAboutPanelAction);

			//remove listener for entering the exit game prompt
			uiManager.GetComponent<ExitGamePrompt>().OnExitGamePromptOpened.RemoveListener(OpenExitGamePromptAction);

			//remove listener for exiting the exit game prompt
			uiManager.GetComponent<ExitGamePrompt>().OnExitGameYesClicked.RemoveListener(CloseExitGamePromptAction);
			uiManager.GetComponent<ExitGamePrompt>().OnExitGameCancelClicked.RemoveListener(CloseExitGamePromptAction);

			//remove listeners telling us the input fields just ended edit, so we can ignore hitting escape
			uiManager.GetComponent<ConfigureLocalGameWindow> ().greenPlayerInputField.onEndEdit.RemoveListener (InputFieldEndEditIgnoreEscapeAction);
			uiManager.GetComponent<ConfigureLocalGameWindow> ().redPlayerInputField.onEndEdit.RemoveListener (InputFieldEndEditIgnoreEscapeAction);
			uiManager.GetComponent<ConfigureLocalGameWindow> ().purplePlayerInputField.onEndEdit.RemoveListener (InputFieldEndEditIgnoreEscapeAction);
			uiManager.GetComponent<ConfigureLocalGameWindow> ().bluePlayerInputField.onEndEdit.RemoveListener (InputFieldEndEditIgnoreEscapeAction);

		}

		//remove listener for a pointer click selection
		UISelection.OnSetSelectedGameObject.RemoveListener(SelectableSetSelectionGroupsAction);

		//remove listener for a pointer click
		UISelection.OnClickedSelectable.RemoveListener(PointerClickResolveBlockAction);

		//remove listener for the redirect click
		UIButtonSelectionRedirect.OnClickedButtonForRedirect.RemoveListener(ButtonRedirectSetSelectedObjectAction);

	}

}
