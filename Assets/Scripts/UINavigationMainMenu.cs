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

	//these public arrays will hold the different selectables arrays
	public Selectable[] MainMenuButtons;
	public Selectable[] NewLocalGameTeams;
	public Selectable[] NewLocalGamePlanets;
	public Selectable[] NewLocalGameGreenPlayer;
	public Selectable[] NewLocalGameRedPlayer;
	public Selectable[] NewLocalGamePurplePlayer;
	public Selectable[] NewLocalGameBluePlayer;
	public Selectable[] NewLocalGameButtonsRow;


	//this array will hold the selectables which are currently being navigated
	private Selectable[][] currentSelectablesGroup;
	private Selectable[] currentSelectables;

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

	//unityActions
	private UnityAction OpenNewLocalGameWindowAction;

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

			}

		}

		//check if the up arrow is being pressed
		if (Input.GetKeyDown (KeyCode.UpArrow)) {

			//check if the vertical cycling is on
			if (verticalCycling == true) {

				//if the vertical cycling is on, advance the selection
				AdvanceSelectable(true);

			}

		}

		//check if the right arrow is being pressed
		if (Input.GetKeyDown (KeyCode.RightArrow)) {

			//check if the horizontal cycling is on
			if (horizontalCycling == true) {

				//if the horizontal cycling is on, advance the selection
				AdvanceSelectable(false);

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

			//advance to the next group
			AdvanceSelectableGroup(false);

		}
		
	}

	//this function sets the UnityActions
	private void SetUnityActions(){

		OpenNewLocalGameWindowAction = () => {CurrentUIState = UIState.NewLocalGame;};

	}

	//this function adds event listeners
	private void AddEventListeners(){

		//add listener for UIState change
		OnUIStateChange.AddListener(SetCurrentSelectables);

		//add listener for new local game
		uiManager.GetComponent<ConfigureLocalGameWindow>().newLocalGameButton.onClick.AddListener(OpenNewLocalGameWindowAction);

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

	}

	//this function sets the current selectables based on the UI state
	private void SetCurrentSelectables(){

		//local variable to hold the potential index of the selection group which has a valid interactable selectable
		int potentialCurrentSelectionGroupIndex;

		//switch case based on UI state
		switch (CurrentUIState) {

		case UIState.MainMenu:

			currentSelectablesGroup = MainMenuGroup;

			potentialCurrentSelectionGroupIndex = FindFirstInteractableArrayIndex (currentSelectablesGroup);

			//set the selectable array that contains an interactable
			if (potentialCurrentSelectionGroupIndex != -1) {

				//set the currentSelectables based on the index returned
				currentSelectables = currentSelectablesGroup[potentialCurrentSelectionGroupIndex];

				//define rules based on what the current selectable is
				if (currentSelectables == MainMenuButtons) {

					horizontalCycling = false;
					verticalCycling = true;
					selectablesWrap = true;
				
				}

			}

			break;

		case UIState.NewLocalGame:

			currentSelectablesGroup = NewLocalGameGroup;

			potentialCurrentSelectionGroupIndex = FindFirstInteractableArrayIndex (currentSelectablesGroup);

			//set the selectable array that contains an interactable
			if (potentialCurrentSelectionGroupIndex != -1) {

				//set the currentSelectables based on the index returned
				currentSelectables = currentSelectablesGroup[potentialCurrentSelectionGroupIndex];

				//define rules based on what the current selectable is
				if (currentSelectables == NewLocalGameTeams) {

					horizontalCycling = true;
					verticalCycling = false;
					selectablesWrap = true;
				
				} else if (currentSelectables == NewLocalGamePlanets){

					horizontalCycling = false;
					verticalCycling = false;
					selectablesWrap = true;

				} else if (currentSelectables == NewLocalGameGreenPlayer){

					horizontalCycling = false;
					verticalCycling = false;
					selectablesWrap = true;

				} else if (currentSelectables == NewLocalGameRedPlayer){

					horizontalCycling = false;
					verticalCycling = false;
					selectablesWrap = true;

				} else if (currentSelectables == NewLocalGamePurplePlayer){

					horizontalCycling = false;
					verticalCycling = false;
					selectablesWrap = true;

				} else if (currentSelectables == NewLocalGameBluePlayer){

					horizontalCycling = false;
					verticalCycling = false;
					selectablesWrap = true;

				} else if (currentSelectables == NewLocalGameButtonsRow){

					horizontalCycling = false;
					verticalCycling = false;
					selectablesWrap = true;

				}

			}

			break;

		default:

			break;

		}

		//check if there is an interactable selectable in currentSelectables
		int potentialCurrentSelectionIndex = FindFirstInteractableIndex (currentSelectables);

		//the Find function will return -1 if it can't find a vaild selectable in the array
		if (potentialCurrentSelectionIndex != -1) {

			//set the Selected object
			eventSystem.SetSelectedGameObject (currentSelectables [potentialCurrentSelectionIndex].gameObject);

			//store the index of the current selection
			currentSelectionIndex = potentialCurrentSelectionIndex;

		}

		/*
		//set the default selected selectable to the first in the current selectables array that is interactable
		for (int i = 0; i < currentSelectables.Length; i++) {

			//check if the first option is interactable
			if (currentSelectables [i].IsInteractable() == true) {

				//set the Selected object
				eventSystem.SetSelectedGameObject (currentSelectables [i].gameObject);

				//store the index of the current selection
				currentSelectionIndex = i;

				//break out of the for loop
				break;

			}

		}
		*/

	}

	//this function finds the index of the first selectable array within a group which contains a valid interactable selectable
	private int FindFirstInteractableArrayIndex(Selectable[][] selectableGroup){

		int returnInt;

		for (int i = 0; i < selectableGroup.Length; i++) {

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

		for (int i = 0; i < selectables.Length; i++) {

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

	//this function advances the selectable group within the array
	private void AdvanceSelectableGroup(bool reverseDirection){

		//variable to hold the potential index of the next selectable
		int potentialGroupIndex;
		int potentialIndex;

		//check if we are reversing selection order or not
		if (reverseDirection == true) {

			potentialGroupIndex = currentSelectionGroupIndex - 1;

		} else {

			potentialGroupIndex = currentSelectionGroupIndex + 1;

		}

		Debug.Log ("potentialGroupIndex = " + potentialGroupIndex);

		//loop through all possible selectable groups
		for (int i = 0; i < currentSelectablesGroup.Length; i++) {

			//check if we are reversing selection order or not
			if (reverseDirection == true) {

				//check if the next index is greater than or equal to  0
				if (potentialGroupIndex  >= 0) {

					//we know the potential index is in the array bounds
					//check if the potential index is interactable
					potentialIndex = FindFirstInteractableIndex(currentSelectablesGroup [potentialGroupIndex]);
					if (potentialIndex != -1) {

						//set the currentSelectables
						currentSelectables = currentSelectablesGroup[potentialGroupIndex];

						//the potential index is interactable
						//set the currentSelection to the selectable at the potential index
						eventSystem.SetSelectedGameObject (currentSelectables [potentialIndex].gameObject);

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
					Debug.Log ("potentialIndex = " + potentialIndex);
					if (potentialIndex != -1) {

						//set the currentSelectables
						currentSelectables = currentSelectablesGroup[potentialGroupIndex];

						//the potential index is interactable
						//set the currentSelection to the selectable at the potential index
						eventSystem.SetSelectedGameObject (currentSelectables [potentialIndex].gameObject);

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
		for (int i = 0; i < currentSelectables.Length; i++) {

			//check if we are reversing selection order or not
			if (reverseDirection == true) {

				//check if the next index is greater than or equal to  0
				if (potentialIndex  >= 0) {

					//we know the potential index is in the array bounds
					//check if the potential index is interactable
					if (currentSelectables [potentialIndex].IsInteractable () == true) {

						//the potential index is interactable
						//set the currentSelection to the selectable at the potential index
						eventSystem.SetSelectedGameObject (currentSelectables [potentialIndex].gameObject);

						//cache the index
						currentSelectionIndex = potentialIndex;

						//break out of the for loop
						break;

					}

				} else if (selectablesWrap == true) {

					//if wrapping is enabled, and we are out of bounds, we can set the potential index to the array length
					potentialIndex = currentSelectables.Length;

				}

				//increment the potential index
				potentialIndex--;

			} else {

				//check if the next index is less than the length
				if (potentialIndex < currentSelectables.Length) {

					//we know the potential index is in the array bounds
					//check if the potential index is interactable
					if (currentSelectables [potentialIndex].IsInteractable () == true) {

						//the potential index is interactable
						//set the currentSelection to the selectable at the potential index
						eventSystem.SetSelectedGameObject (currentSelectables [potentialIndex].gameObject);

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

	//this function handles on destroy
	private void OnDestroy(){

		//remove all event listeners
		RemoveEventListeners();

	}

	//this function removes all event listeners
	private void RemoveEventListeners(){

		//remove listener for UIState change
		OnUIStateChange.RemoveListener(SetCurrentSelectables);

		if (uiManager != null) {
			
			//remove listener for new local game
			uiManager.GetComponent<ConfigureLocalGameWindow> ().newLocalGameButton.onClick.RemoveListener (OpenNewLocalGameWindowAction);

		}

	}

}
