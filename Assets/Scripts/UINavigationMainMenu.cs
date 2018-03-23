using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UINavigationMainMenu : MonoBehaviour {

	//this will hold the eventSystem in the scene
	public EventSystem eventSystem;

	//this enum will keep track of the current UI State
	public enum UIState{

		MainMenu,

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

	//these public arrays will hold the different groups of selectables
	public Selectable[] MainMenuButtons;


	//this array will hold the selectables which are currently being navigated
	private Selectable[] currentSelectables;

	//this int holds the index of the current selection within the currentSelectables array
	private int currentSelectionIndex;

	//these bools determine whether vertical and horizontal key inputs will cycle through the selectables
	private bool horizontalCycling = false;
	private bool verticalCycling = false;

	//this bool determines whether the selectables wrap around
	private bool selectablesWrap = false;

	//event to announce UIState change
	public UnityEvent OnUIStateChange = new UnityEvent();

	// Use this for initialization
	public void Init () {

		//add event listeners
		AddEventListeners();

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
		
	}

	//this function adds event listeners
	private void AddEventListeners(){

		//add listener for UIState change
		OnUIStateChange.AddListener(SetCurrentSelectables);

	}

	//this function sets the current selectables based on the UI state
	private void SetCurrentSelectables(){

		//switch case based on UI state
		switch (CurrentUIState) {

		case UIState.MainMenu:

			currentSelectables = MainMenuButtons;

			horizontalCycling = false;
			verticalCycling = true;
			selectablesWrap = true;

			break;

		default:

			currentSelectables = MainMenuButtons;

			horizontalCycling = false;
			verticalCycling = true;
			selectablesWrap = true;

			break;

		}

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

	}

}
