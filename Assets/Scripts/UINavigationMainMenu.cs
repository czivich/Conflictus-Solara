using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UINavigationMainMenu : MonoBehaviour {

	//this will hold the eventSystem in the scene
	public EventSystem eventSystem;

	//uiManager
	private GameObject uiManager;

	//this enum will keep track of the current UI State
	public enum UIState{

		MainMenu,
		NewLocalGame,
		LoadLocalGame,

	}

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
	private Selectable[][] MainMenuGroup;
	private Selectable[][] NewLocalGameGroup;
	private Selectable[][] LoadLocalGameGroup;

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


	//public scroll bars
	public Scrollbar LoadGameScrollBar;


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

	//event to announce UIState change
	public UnityEvent OnUIStateChange = new UnityEvent();
	public UnityEvent OnSelectablesChange = new UnityEvent();


	//unityActions
	private UnityAction OpenNewLocalGameWindowAction;
	private UnityAction<Selectable> SelectableSetSelectionGroupsAction;
	private UnityAction ReturnToMainMenuAction;
	private UnityAction OpenLoadLocalGameWindowAction;
	private UnityAction SelectableSetNavigationRulesAction;

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

			}

		}

		//check if the right arrow is being pressed
		if (Input.GetKeyDown (KeyCode.RightArrow)) {

			//check if the horizontal cycling is on
			if (horizontalCycling == true) {

				//if the horizontal cycling is on, advance the selection
				AdvanceSelectable (false);

			} 

		}

		//check if the left arrow is being pressed
		if (Input.GetKeyDown (KeyCode.LeftArrow)) {

			//check if the horizontal cycling is on
			if (horizontalCycling == true) {

				//if the horizontal cycling is on, advance the selection
				AdvanceSelectable(true);

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

		SelectableSetSelectionGroupsAction = (selectable) => {SetSelectionIndexFromPointerClick (selectable);};

		ReturnToMainMenuAction = () => {CurrentUIState = UIState.MainMenu;};

		OpenLoadLocalGameWindowAction = () => {CurrentUIState = UIState.LoadLocalGame;};

		SelectableSetNavigationRulesAction = () => {SetNavigationRulesForSelectables();};

	}

	//this function adds event listeners
	private void AddEventListeners(){

		//add listener for UIState change
		OnUIStateChange.AddListener(SetInitialCurrentSelectables);

		//add listener for new local game
		uiManager.GetComponent<ConfigureLocalGameWindow>().newLocalGameButton.onClick.AddListener(OpenNewLocalGameWindowAction);

		//add listener for a pointer click selection
		UISelection.OnSetSelectedGameObject.AddListener(SelectableSetSelectionGroupsAction);

		//add listeners for exiting the new local game window to the main menu
		uiManager.GetComponent<ConfigureLocalGameWindow>().cancelButton.onClick.AddListener(ReturnToMainMenuAction);
		uiManager.GetComponent<ConfigureLocalGameWindow>().exitWindowButton.onClick.AddListener(ReturnToMainMenuAction);

		//add listener for load local game
		uiManager.GetComponent<FileLoadWindow>().OnOpenFileLoadWindow.AddListener(OpenLoadLocalGameWindowAction);

		//add listener for setting selectables
		OnSelectablesChange.AddListener(SelectableSetNavigationRulesAction);

		//add listeners for exiting the file load window back to the main menu
		uiManager.GetComponent<FileLoadWindow>().closeFileLoadWindowButton.onClick.AddListener(ReturnToMainMenuAction);
		uiManager.GetComponent<FileLoadWindow>().fileLoadCancelButton.onClick.AddListener(ReturnToMainMenuAction);

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

		} 

	}

	//this function sets the current selectables based on the UI state
	private void SetInitialCurrentSelectables(){

		//local variable to hold the potential index of the selection group which has a valid interactable selectable
		int potentialCurrentSelectionGroupIndex;

		//switch case based on UI state
		switch (CurrentUIState) {

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

				//set the scrollbar to the top
				CurrentSelectables [currentSelectionIndex].transform.parent.parent.GetComponent<ScrollRect> ().verticalNormalizedPosition = 1.0f;

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

		//we need to find the selectableClicked within the currentSelectables
		for (int j = 0; j < CurrentSelectables.Length; j++) {

			//check if the selectableClicked matches the currentSelectable
			if (CurrentSelectables [j] == selectableClicked) {

				//set the currentSelectionIndex
				currentSelectionIndex = j;

				//break out of the for loop
				break;

			}

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
			
			//remove listener for new local game
			uiManager.GetComponent<ConfigureLocalGameWindow> ().newLocalGameButton.onClick.RemoveListener (OpenNewLocalGameWindowAction);

			//remove listeners for exiting the new local game window to the main menu
			uiManager.GetComponent<ConfigureLocalGameWindow>().cancelButton.onClick.RemoveListener(ReturnToMainMenuAction);
			uiManager.GetComponent<ConfigureLocalGameWindow>().exitWindowButton.onClick.RemoveListener(ReturnToMainMenuAction);

			//remove listener for load local game
			uiManager.GetComponent<FileLoadWindow>().OnOpenFileLoadWindow.RemoveListener(OpenLoadLocalGameWindowAction);

			//remove listeners for exiting the file load window back to the main menu
			uiManager.GetComponent<FileLoadWindow>().closeFileLoadWindowButton.onClick.RemoveListener(ReturnToMainMenuAction);
			uiManager.GetComponent<FileLoadWindow>().fileLoadCancelButton.onClick.RemoveListener(ReturnToMainMenuAction);
		}

		//remove listener for a pointer click selection
		UISelection.OnSetSelectedGameObject.RemoveListener(SelectableSetSelectionGroupsAction);

	}

}
