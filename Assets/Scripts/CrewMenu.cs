using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class CrewMenu : MonoBehaviour {

	//we will need the mouseManager to check the selected unit
	private MouseManager mouseMananger;

	//variable for useItem menu trigger toggle
	public Toggle crewToggle;

	//variable for the system upgrades
	public Toggle repairCrewToggle;
	public Toggle shieldEngineeringTeamToggle;
	public Toggle battleCrewToggle;

	//variable for the targeting dropdown
	public TMP_Dropdown repairTargetingDropdown;

	//variable for the targeting helper text
	public TextMeshProUGUI repairTargetingText;

	//variable to hold the use item button
	public Button useRepairCrewButton;

	//these events are for when the repair crew is used
	public RepairEvent OnUseRepairCrew = new RepairEvent();

	//simple class derived from unityEvent to pass Ship Object
	public class RepairEvent : UnityEvent<CombatUnit, CombatUnit, string>{};

	//unityActions
	private UnityAction<bool> boolSetCrewMenuTogglesAction;
	private UnityAction<int> intSetCrewMenuTogglesAction;
	private UnityAction SetCrewMenuTogglesAndZeroRepairDropdownAction;
	private UnityAction<CombatUnit> combatUnitSetCrewMenuTogglesAction;

	// Use this for initialization
	public void Init () {

		//set the actions
		boolSetCrewMenuTogglesAction = (value) => {SetCrewMenuToggles();};
		intSetCrewMenuTogglesAction = (value) => {SetCrewMenuToggles();};
		SetCrewMenuTogglesAndZeroRepairDropdownAction = () => {
			SetCrewMenuToggles();
			repairTargetingDropdown.value = 0;
		};

		combatUnitSetCrewMenuTogglesAction = (combatUnit) => {SetCrewMenuToggles ();};

		//get the mouseManager
		mouseMananger = GameObject.FindGameObjectWithTag ("MouseManager").GetComponent<MouseManager> ();

		//add an on-click event listener for the main menu useItem toggle
		crewToggle.onValueChanged.AddListener(boolSetCrewMenuTogglesAction);

		//add an event listener for the dropdown value changing
		repairTargetingDropdown.onValueChanged.AddListener(intSetCrewMenuTogglesAction);

		//add an on-click event listener for the use repair crew buttons
		useRepairCrewButton.onClick.AddListener(UseRepairCrew);

		//add an event listener for when a selectedUnit is set and cleared
		//I want to set the dropdown back to the default when the selected unit changes
		mouseMananger.OnSetSelectedUnit.AddListener(SetCrewMenuTogglesAndZeroRepairDropdownAction);
		mouseMananger.OnClearSelectedUnit.AddListener(SetCrewMenuTogglesAndZeroRepairDropdownAction);

		//add an event listener for when a targetedUnit is set and cleared
		//I want to set the dropdown back to the default when the selected unit changes
		mouseMananger.OnSetTargetedUnit.AddListener(SetCrewMenuTogglesAndZeroRepairDropdownAction);
		mouseMananger.OnClearTargetedUnit.AddListener(SetCrewMenuTogglesAndZeroRepairDropdownAction);

		//add a listener for when a purchase is complete
		CrewSection.OnInventoryUpdated.AddListener(combatUnitSetCrewMenuTogglesAction);

	}

	//this function sets the interactability of the crew menu toggles
	private void SetCrewMenuToggles(){

		//this is if the crew menu Toggle has been turned on
		if (crewToggle.isOn == true) {

			//if there is no unit selected, set toggles to not interactable and turn them off
			if (mouseMananger.selectedUnit == null) {

				repairCrewToggle.isOn = false;
				repairCrewToggle.interactable = false;

				shieldEngineeringTeamToggle.isOn = false;
				shieldEngineeringTeamToggle.interactable = false;

				battleCrewToggle.isOn = false;
				battleCrewToggle.interactable = false;


				repairTargetingDropdown.interactable = false;


				useRepairCrewButton.interactable = false;

			}

			//the else condition means that there is a selected unit
			else {

				//now we need to make sure the selected unit is a ship
				//if it's a starbase, we need to handle with slightly diffferent logic
				if (mouseMananger.selectedUnit.GetComponent<Ship> () == null) {

					//if we are here, the selected unit is a starbase
					//now we need to make sure the crew section hasn't been destroyed
					if (mouseMananger.selectedUnit.GetComponentInChildren<StarbaseCrewSection> ().isDestroyed == true) {

						//if it is destroyed, we can't use crew commands
						repairCrewToggle.isOn = false;
						repairCrewToggle.interactable = false;

						shieldEngineeringTeamToggle.isOn = false;
						shieldEngineeringTeamToggle.interactable = false;

						battleCrewToggle.isOn = false;
						battleCrewToggle.interactable = false;


						repairTargetingDropdown.interactable = false;


						useRepairCrewButton.interactable = false;

					}

					//the else condition is that the crew section is not destroyed.  
					else {

						//if the base has the repair crew upgrade, set that toggle to on
						if (mouseMananger.selectedUnit.GetComponentInChildren<StarbaseCrewSection> ().repairCrew == true) {

							repairCrewToggle.isOn = true;
							repairCrewToggle.interactable = false;

						}

						//if the base has the shield engineering team upgrade, set that toggle to on
						if (mouseMananger.selectedUnit.GetComponentInChildren<StarbaseCrewSection> ().shieldEngineeringTeam == true) {

							shieldEngineeringTeamToggle.isOn = true;
							shieldEngineeringTeamToggle.interactable = false;

						}

						//if the base has the battle crew upgrade, set that toggle to on
						if (mouseMananger.selectedUnit.GetComponentInChildren<StarbaseCrewSection> ().battleCrew == true) {

							battleCrewToggle.isOn = true;
							battleCrewToggle.interactable = false;

						}

						//now we need to check to see if we have a unit targeted
						if (mouseMananger.targetedUnit == null) {

							//if there is no targeted unit, we can't choose a section or use a repair crew
							repairTargetingDropdown.interactable = false;

							useRepairCrewButton.interactable = false;

						}

						//the else condition is that we do have a unit targeted
						else {

							//check if the targeted unit is a ship.  if it is a base, we have to handle slightly differently
							if (mouseMananger.targetedUnit.GetComponent<Ship> () == true) {

								//if a ship is targeted, allow the dropdown to be interactable
								repairTargetingDropdown.interactable = true;

								//we can't allow the use repair button to be pressed until a section has been chosen
								if (repairTargetingDropdown.GetComponentInChildren<TextMeshProUGUI> ().text == "Choose Section") {

									useRepairCrewButton.interactable = false;

								}
								//the else condition is that a section has been chosen, so we can use the use repair button
								else {

									//check to see if we've already used the repair crew this turn
									if (mouseMananger.selectedUnit.GetComponentInChildren<StarbaseCrewSection> ().usedRepairCrewThisTurn == true) {

										//if we've already used the repair crew this turn, we can't use it again
										useRepairCrewButton.interactable = false;

									}
									//the else condition is that we have not used the repair crew yet
									else {

										useRepairCrewButton.interactable = true;

									}

								}

							}
							//the else condition is that the targeted unit is a base
							else if (mouseMananger.targetedUnit.GetComponent<Starbase> () == true){

								//if a base is targeted, allow the dropdown to be interactable
								repairTargetingDropdown.interactable = true;

								//we can't allow the use repair button to be pressed until a section has been chosen
								if (repairTargetingDropdown.GetComponentInChildren<TextMeshProUGUI> ().text == "Choose Section") {

									useRepairCrewButton.interactable = false;

								}
								//the else condition is that a section has been chosen, so we can use the use repair button
								else {

									//check to see if we've already used the repair crew this turn
									if (mouseMananger.selectedUnit.GetComponentInChildren<StarbaseCrewSection> ().usedRepairCrewThisTurn == true) {

										//if we've already used the repair crew this turn, we can't use it again
										useRepairCrewButton.interactable = false;

									}
									//the else condition is that we have not used the repair crew yet
									else {

										useRepairCrewButton.interactable = true;

									}

								} //close the else a section has been chosen

							} //close the else the targeted unit is a base

						}  //close else statement that we hae a unit targeted

					}  //close else statement that the selected unit crew section is not destroyed

				}  //close the if the selected unit is a starbase

				//the else condition is that we do have a ship, so we want to allow toggles based on the ship inventory
				else {

					//check to see if the ship has a crew section.  If not, it can't use the crew menu
					if (mouseMananger.selectedUnit.GetComponentInChildren<CrewSection> () == null) {

						repairCrewToggle.isOn = false;
						repairCrewToggle.interactable = false;

						shieldEngineeringTeamToggle.isOn = false;
						shieldEngineeringTeamToggle.interactable = false;

						battleCrewToggle.isOn = false;
						battleCrewToggle.interactable = false;


						repairTargetingDropdown.interactable = false;


						useRepairCrewButton.interactable = false;

					}

					//the else condition is that there is a crew section in the selected unit
					else {

						//now we need to make sure the crew section hasn't been destroyed
						if (mouseMananger.selectedUnit.GetComponentInChildren<CrewSection> ().isDestroyed == true) {

							//if it is destroyed, we can't use crew commands
							repairCrewToggle.isOn = false;
							repairCrewToggle.interactable = false;

							shieldEngineeringTeamToggle.isOn = false;
							shieldEngineeringTeamToggle.interactable = false;

							battleCrewToggle.isOn = false;
							battleCrewToggle.interactable = false;


							repairTargetingDropdown.interactable = false;


							useRepairCrewButton.interactable = false;

						}

						//the else condition is that the crew section is not destroyed.  
						else {

							//if the ship has the repair crew upgrade, set that toggle to on
							if (mouseMananger.selectedUnit.GetComponentInChildren<CrewSection> ().repairCrew == true) {

								repairCrewToggle.isOn = true;
								repairCrewToggle.interactable = false;

							}

							//if the ship has the shield engineering team upgrade, set that toggle to on
							if (mouseMananger.selectedUnit.GetComponentInChildren<CrewSection> ().shieldEngineeringTeam == true) {

								shieldEngineeringTeamToggle.isOn = true;
								shieldEngineeringTeamToggle.interactable = false;

							}

							//if the ship has the battle crew upgrade, set that toggle to on
							if (mouseMananger.selectedUnit.GetComponentInChildren<CrewSection> ().battleCrew == true) {

								battleCrewToggle.isOn = true;
								battleCrewToggle.interactable = false;

							}

							//now we need to check to see if we have a unit targeted
							if (mouseMananger.targetedUnit == null) {

								//if there is no targeted unit, we can't choose a section or use a repair crew
								repairTargetingDropdown.interactable = false;

								useRepairCrewButton.interactable = false;

							}

							//the else condition is that we do have a unit targeted
							else {

								//check if the targeted unit is a ship.  if it is a base, we have to handle slightly differently
								if (mouseMananger.targetedUnit.GetComponent<Ship> () == true) {

									//if a ship is targeted, allow the dropdown to be interactable
									repairTargetingDropdown.interactable = true;

									//we can't allow the use repair button to be pressed until a section has been chosen
									if (repairTargetingDropdown.GetComponentInChildren<TextMeshProUGUI> ().text == "Choose Section") {

										useRepairCrewButton.interactable = false;

									}
									//the else condition is that a section has been chosen, so we can use the use repair button
									else {

										//check to see if we've already used the repair crew this turn
										if (mouseMananger.selectedUnit.GetComponentInChildren<CrewSection> ().usedRepairCrewThisTurn == true) {

											//if we've already used the repair crew this turn, we can't use it again
											useRepairCrewButton.interactable = false;

										}
										//the else condition is that we have not used the repair crew yet
										else {
											
											useRepairCrewButton.interactable = true;

										}

									}

								}
								//the else condition is that the targeted unit is a base
								else if (mouseMananger.targetedUnit.GetComponent<Starbase> () == true){

									//if a base is targeted, allow the dropdown to be interactable
									repairTargetingDropdown.interactable = true;

									//we can't allow the use repair button to be pressed until a section has been chosen
									if (repairTargetingDropdown.GetComponentInChildren<TextMeshProUGUI> ().text == "Choose Section") {

										useRepairCrewButton.interactable = false;

									}
									//the else condition is that a section has been chosen, so we can use the use repair button
									else {

										//check to see if we've already used the repair crew this turn
										if (mouseMananger.selectedUnit.GetComponentInChildren<CrewSection> ().usedRepairCrewThisTurn == true) {

											//if we've already used the repair crew this turn, we can't use it again
											useRepairCrewButton.interactable = false;

										}
										//the else condition is that we have not used the repair crew yet
										else {

											useRepairCrewButton.interactable = true;

										}

									} //close the else a section has been chosen

								} //close the else the targeted unit is a base

							}  //close else statement that we hae a unit targeted

						}  //close else statement that the selected unit crew section is not destroyed

					}  //close else statement that selected unit has a crew section

				}  //close else statement that selected unit is a ship

			}  //close the else statement that there is a selected unit

			SetTargetingText ();
			SetTargetingSectionOptions ();
			repairTargetingDropdown.RefreshShownValue ();
			SetRepairButtonText ();

		}

		//this is if the crew menu toggle has been turned off
		else if (crewToggle.isOn == false) {

			//if we have don't have a selected unit, we can turn the toggles off and make them not interactable
			if (mouseMananger.selectedUnit == null) {

				repairCrewToggle.isOn = false;
				repairCrewToggle.interactable = false;

				shieldEngineeringTeamToggle.isOn = false;
				shieldEngineeringTeamToggle.interactable = false;

				battleCrewToggle.isOn = false;
				battleCrewToggle.interactable = false;


				repairTargetingDropdown.interactable = false;


				useRepairCrewButton.interactable = false;

				SetTargetingText ();
				SetRepairButtonText ();

			}

			//now we can check if we have a targeted unit
			if (mouseMananger.targetedUnit == null) {

				//if there is no targeted unit anymore, we can set the toggles off and make them not interactable
				repairCrewToggle.isOn = false;
				repairCrewToggle.interactable = false;

				shieldEngineeringTeamToggle.isOn = false;
				shieldEngineeringTeamToggle.interactable = false;

				battleCrewToggle.isOn = false;
				battleCrewToggle.interactable = false;


				repairTargetingDropdown.interactable = false;


				useRepairCrewButton.interactable = false;

				SetTargetingText ();
				SetRepairButtonText ();

			}

		}

	}

	//this function sets the helper text for the repair targeting
	private void SetTargetingText(){

		//check if there is a targeted unit
		if (mouseMananger.targetedUnit != null) {

			//check if the targeted unit is a ship
			if (mouseMananger.targetedUnit.GetComponent<Ship> () == true) {

				repairTargetingText.text = ("Choose Section");

				//update the font size if necessary
				UIManager.AutoSizeTextMeshFont(repairTargetingText.GetComponentInChildren<TextMeshProUGUI> ());

			}
			//the else condition is that the targeted unit is not a ship
			else if (mouseMananger.targetedUnit.GetComponent<Starbase> () == true){

				repairTargetingText.text = ("Choose Section");

				//update the font size if necessary
				UIManager.AutoSizeTextMeshFont(repairTargetingText.GetComponentInChildren<TextMeshProUGUI> ());

			}

		}
		//the else condition is that there is no targeted unit
		else {

			//check whether we have a selected unit
			if (mouseMananger.selectedUnit != null) {

				repairTargetingText.text = ("No Unit Targeted");

				//update the font size if necessary
				UIManager.AutoSizeTextMeshFont(repairTargetingText.GetComponentInChildren<TextMeshProUGUI> ());

			}
			//the else condition is that there is no selected unit
			else {

				repairTargetingText.text = ("No Unit Selected");

				//update the font size if necessary
				UIManager.AutoSizeTextMeshFont(repairTargetingText.GetComponentInChildren<TextMeshProUGUI> ());

			}

		}

	}

	//this function will set the available options for the targeting dropdown menu
	private void SetTargetingSectionOptions(){

		//start by clearing the existing dropdown options
		repairTargetingDropdown.ClearOptions();

		//create a list of new dropdown options to populate the choices
		List<TMP_Dropdown.OptionData> dropDownOptions = new List<TMP_Dropdown.OptionData> ();

		//add a dummy option at the top of the list
		dropDownOptions.Add(new TMP_Dropdown.OptionData("Choose Section"));

		//check if there is a targeted unit
		if (mouseMananger.targetedUnit != null) {

			//check if the targeted unit is a ship
			if (mouseMananger.targetedUnit.GetComponent<Ship> () == true) {

				//check if the targeted ship has an engine section
				if (mouseMananger.targetedUnit.GetComponent<EngineSection> () == true) {

					//check if the engine section is destroyed
					if (mouseMananger.targetedUnit.GetComponent<EngineSection> ().isDestroyed == true) {

						//add a dropdown option for the engine section
						dropDownOptions.Add (new TMP_Dropdown.OptionData ("Engine Section"));

					}

				}

				//check if the targeted ship has a phasor section
				if (mouseMananger.targetedUnit.GetComponent<PhasorSection> () == true) {

					//check if the phasor section is destroyed
					if (mouseMananger.targetedUnit.GetComponent<PhasorSection> ().isDestroyed == true) {

						//add a dropdown option for the phasor section
						dropDownOptions.Add (new TMP_Dropdown.OptionData ("Phasor Section"));
	
					}

				}

				//check if the targeted ship has a torpedo section
				if (mouseMananger.targetedUnit.GetComponent<TorpedoSection> () == true) {

					//check if the torpedo section is destroyed
					if (mouseMananger.targetedUnit.GetComponent<TorpedoSection> ().isDestroyed == true) {

						//add a dropdown option for the torpedo section
						dropDownOptions.Add (new TMP_Dropdown.OptionData ("Torpedo Section"));

					}

				}

				//check if the targeted ship has a crew section
				if (mouseMananger.targetedUnit.GetComponent<CrewSection> () == true) {

					//check if the crew section is destroyed
					if (mouseMananger.targetedUnit.GetComponent<CrewSection> ().isDestroyed == true) {

						//add a dropdown option for the crew section
						dropDownOptions.Add (new TMP_Dropdown.OptionData ("Crew Section"));

					}

				}

				//check if the targeted ship has a storage section
				if (mouseMananger.targetedUnit.GetComponent<StorageSection> () == true) {

					//check if the storage section is destroyed
					if (mouseMananger.targetedUnit.GetComponent<StorageSection> ().isDestroyed == true) {

						//add a dropdown option for the storage section
						dropDownOptions.Add (new TMP_Dropdown.OptionData ("Storage Section"));

					}

				}

			}
			//the else condition is that the targeted unit is not a ship, so it's a base
			else if (mouseMananger.targetedUnit.GetComponent<Starbase> () == true){

				//check if the phasor 1 section is destroyed
				if (mouseMananger.targetedUnit.GetComponent<StarbasePhasorSection1> ().isDestroyed == true) {

					//add a dropdown option for the phasor 1 section
					dropDownOptions.Add (new TMP_Dropdown.OptionData ("Phasor Section 1"));

				}

				//check if the phasor 2 section is destroyed
				if (mouseMananger.targetedUnit.GetComponent<StarbasePhasorSection2> ().isDestroyed == true) {

					//add a dropdown option for the phasor 2 section
					dropDownOptions.Add (new TMP_Dropdown.OptionData ("Phasor Section 2"));

				}

				//check if the torpedo section is destroyed
				if (mouseMananger.targetedUnit.GetComponent<StarbaseTorpedoSection> ().isDestroyed == true) {

					//add a dropdown option for the torpedo section
					dropDownOptions.Add (new TMP_Dropdown.OptionData ("Torpedo Section"));

				}

				//check if the crew section is destroyed
				if (mouseMananger.targetedUnit.GetComponent<StarbaseCrewSection> ().isDestroyed == true) {

					//add a dropdown option for the crew section 
					dropDownOptions.Add (new TMP_Dropdown.OptionData ("Crew Section"));

				}

				//check if the storage section 1 is destroyed
				if (mouseMananger.targetedUnit.GetComponent<StarbaseStorageSection1> ().isDestroyed == true) {

					//add a dropdown option for the storage section 1 
					dropDownOptions.Add (new TMP_Dropdown.OptionData ("Storage Section 1"));

				}

				//check if the storage section 2 is destroyed
				if (mouseMananger.targetedUnit.GetComponent<StarbaseStorageSection2> ().isDestroyed == true) {

					//add a dropdown option for the storage section 2 
					dropDownOptions.Add (new TMP_Dropdown.OptionData ("Storage Section 2"));

				}

			}

		}

		//add the options to the dropdown
		repairTargetingDropdown.AddOptions (dropDownOptions);

		//update the font size if necessary
		UIManager.AutoSizeTextMeshFont(repairTargetingDropdown.GetComponentInChildren<TextMeshProUGUI> ());

	}

	//this function will set the repair button text
	private void SetRepairButtonText(){

		//check if we have a selected unit
		if (mouseMananger.selectedUnit != null) {

			//check if the selected unit is a starship
			if (mouseMananger.selectedUnit.GetComponent<Starship> () == true) {
				
				//check if the selected unit has a valid repair use remaining
				if (mouseMananger.selectedUnit.GetComponent<Starship> ().hasRemainingRepairAction == true) {

					//if the repair is still remaining, set the text to Repair
					useRepairCrewButton.GetComponentInChildren<TextMeshProUGUI> ().text =  ("Repair");

					//update the font size if necessary
					UIManager.AutoSizeTextMeshFont (useRepairCrewButton.GetComponentInChildren<TextMeshProUGUI> ());

				} else {

					//if there is no repair remaining, set the text to already used
					useRepairCrewButton.GetComponentInChildren<TextMeshProUGUI> ().text =  ("Already Used");

					//update the font size if necessary
					UIManager.AutoSizeTextMeshFont (useRepairCrewButton.GetComponentInChildren<TextMeshProUGUI> ());

				}

			} else if(mouseMananger.selectedUnit.GetComponent<Starbase> () == true) {

				//else check if we have a starbase

				//check if the selected unit has a valid repair use remaining
				if (mouseMananger.selectedUnit.GetComponent<Starbase> ().hasRemainingRepairAction == true) {

					//if the repair is still remaining, set the text to Repair
					useRepairCrewButton.GetComponentInChildren<TextMeshProUGUI> ().text =  ("Repair");

					//update the font size if necessary
					UIManager.AutoSizeTextMeshFont (useRepairCrewButton.GetComponentInChildren<TextMeshProUGUI> ());

				} else {

					//if there is no repair remaining, set the text to already used
					useRepairCrewButton.GetComponentInChildren<TextMeshProUGUI> ().text =  ("Already Used");

					//update the font size if necessary
					UIManager.AutoSizeTextMeshFont (useRepairCrewButton.GetComponentInChildren<TextMeshProUGUI> ());

				}

			}

		}

	}

	//this function handles using a dilithium crystal
	private void UseRepairCrew(){

		//Debug.Log ("Use Repair Crew!");

		//invoke the OnUseRepairCrew event, with the selected unit, targeted unit, and string of section targeted passed to the event
		OnUseRepairCrew.Invoke(mouseMananger.selectedUnit.GetComponent<CombatUnit>(),
			mouseMananger.targetedUnit.GetComponent<CombatUnit>(),
			repairTargetingDropdown.GetComponentInChildren<TextMeshProUGUI>().text);

		SetCrewMenuToggles ();

	}

	//function for on Destroy
	private void OnDestroy(){

		RemoveAllListeners ();

	}

	//function for removing listeners on Destroy
	private void RemoveAllListeners(){

		if (crewToggle != null) {
			
			crewToggle.onValueChanged.RemoveListener (boolSetCrewMenuTogglesAction);

		}

		if (repairTargetingDropdown != null) {
			
			repairTargetingDropdown.onValueChanged.RemoveListener (intSetCrewMenuTogglesAction);

		}

		if (useRepairCrewButton != null) {
			
			useRepairCrewButton.onClick.RemoveListener (UseRepairCrew);

		}

		if (mouseMananger != null) {
			
			mouseMananger.OnSetSelectedUnit.RemoveListener (SetCrewMenuTogglesAndZeroRepairDropdownAction);
			mouseMananger.OnClearSelectedUnit.RemoveListener (SetCrewMenuTogglesAndZeroRepairDropdownAction);
			mouseMananger.OnSetTargetedUnit.RemoveListener (SetCrewMenuTogglesAndZeroRepairDropdownAction);
			mouseMananger.OnClearTargetedUnit.RemoveListener(SetCrewMenuTogglesAndZeroRepairDropdownAction);

		}

		CrewSection.OnInventoryUpdated.RemoveListener(combatUnitSetCrewMenuTogglesAction);

	}
		
}
