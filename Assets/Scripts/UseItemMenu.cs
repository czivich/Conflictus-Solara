using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class UseItemMenu : MonoBehaviour {

	//we will need the mouseManager to check the selected unit
	private MouseManager mouseMananger;

	//variable for useItem menu trigger toggle
	public Toggle useItemToggle;

	//variable for the system upgrades
	public Toggle phasorRadarJammimgToggle;
	public Toggle torpedoLaserScatteringToggle;

	//variables for the flareModes
	public Button flareModeManualButton;
	public Button flareModeAutoButton;

	//variable for the flare count
	public Toggle flareCountToggle;

	//variable for the targeting dropdown
	public TMP_Dropdown crystalTargetingDropdown;

	//variable for the targeting helper text
	public TextMeshProUGUI crystalTargetingText;

	//variable to hold the use item buttons
	public Button useDilithiumCrystalButton;
	public Button useTrilithiumCrystalButton;

	//variables for crystal counts
	public Toggle dilithiumCrystalCountToggle;
	public Toggle trilithiumCrystalCountToggle;

	//color for selected button tab
	private Color selectedButtonColor = new Color (240.0f / 255.0f, 240.0f / 255.0f, 20.0f / 255.0f, 255.0f / 255.0f); 

	//these events are for when the crystals are used
	public CrystalEvent OnUseDilithiumCrystal = new CrystalEvent();
	public CrystalEvent OnUseTrilithiumCrystal = new CrystalEvent();

	//these events are for setting the flareModes
	public FlareModeEvent OnSetFlareModeToManual = new FlareModeEvent();
	public FlareModeEvent OnSetFlareModeToAuto = new FlareModeEvent();

	//simple class derived from unityEvent to pass Ship Object
	public class CrystalEvent : UnityEvent<CombatUnit, CombatUnit, string>{};

	//class to pass combat unit for setting flareMode
	public class FlareModeEvent : UnityEvent<CombatUnit>{};

	//unityActions
	private UnityAction<bool> boolSetUseItemMenuTogglesAction;
	private UnityAction<int> intSetUseItemMenuTogglesAction;
	private UnityAction setUseItemMenuTogglesDropdownZeroAction;
	private UnityAction clickSetFlareModeToManualAction;
	private UnityAction clickSetFlareModeToAutoAction;
	private UnityAction<CombatUnit> combatUnitSetUseItemMenuTogglesAction;


	// Use this for initialization
	public void Init () {

		//get the mouseManager
		mouseMananger = GameObject.FindGameObjectWithTag ("MouseManager").GetComponent<MouseManager> ();

		//set the actions
		boolSetUseItemMenuTogglesAction = (value) => {SetUseItemMenuToggles();};
		intSetUseItemMenuTogglesAction = (value) => {SetUseItemMenuToggles();};
		setUseItemMenuTogglesDropdownZeroAction = () => {
			SetUseItemMenuToggles();
			crystalTargetingDropdown.value = 0;
		};
		clickSetFlareModeToManualAction = () => {SetFlareModeToManual();};
		clickSetFlareModeToAutoAction = () => {SetFlareModeToAuto();};
		combatUnitSetUseItemMenuTogglesAction = (combatUnit) => {SetUseItemMenuToggles();};

		//add an on-click event listener for the main menu useItem toggle
		useItemToggle.onValueChanged.AddListener(boolSetUseItemMenuTogglesAction);

		//add an event listener for the dropdown value changing
		crystalTargetingDropdown.onValueChanged.AddListener(intSetUseItemMenuTogglesAction);

		//add an on-click event listener for the useCrystals buttons
		useDilithiumCrystalButton.onClick.AddListener(UseDilithiumCrystal);
		useTrilithiumCrystalButton.onClick.AddListener(UseTrilithiumCrystal);

		//add an event listener for when a selectedUnit is set and cleared
		//I want to set the dropdown back to the default when the selected unit changes
		mouseMananger.OnSetSelectedUnit.AddListener(setUseItemMenuTogglesDropdownZeroAction);
		mouseMananger.OnClearSelectedUnit.AddListener(setUseItemMenuTogglesDropdownZeroAction);

		//add an event listener for when a targetedUnit is set and cleared
		//I want to set the dropdown back to the default when the selected unit changes
		mouseMananger.OnSetTargetedUnit.AddListener(setUseItemMenuTogglesDropdownZeroAction);
		mouseMananger.OnClearTargetedUnit.AddListener(setUseItemMenuTogglesDropdownZeroAction);

		//add an on-click event listener for the flareMode manual
		flareModeManualButton.onClick.AddListener(clickSetFlareModeToManualAction);

		//add an on-click event listener for the flareMode automatic
		flareModeAutoButton.onClick.AddListener(clickSetFlareModeToAutoAction);

		//add a listener for when a purchase is complete
		StorageSection.OnInventoryUpdated.AddListener(combatUnitSetUseItemMenuTogglesAction);

	}

	//this function sets the interactability of the useItem menu toggles
	private void SetUseItemMenuToggles(){

		//this is if the useItem menu Toggle has been turned on
		if (useItemToggle.isOn == true) {

			//update the targeting dropdown options
			SetTargetingSectionOptions ();

			//if there is no unit selected, set toggles to not interactable and turn them off
			if (mouseMananger.selectedUnit == null) {

				phasorRadarJammimgToggle.isOn = false;
				phasorRadarJammimgToggle.interactable = false;

				torpedoLaserScatteringToggle.isOn = false;
				torpedoLaserScatteringToggle.interactable = false;


				flareModeManualButton.interactable = false;
				flareModeAutoButton.interactable = false;

				flareCountToggle.isOn = false;
				flareCountToggle.interactable = false;


				crystalTargetingDropdown.interactable = false;

				useDilithiumCrystalButton.interactable = false;
				dilithiumCrystalCountToggle.isOn = false;
				dilithiumCrystalCountToggle.interactable = false;

				useTrilithiumCrystalButton.interactable = false;
				trilithiumCrystalCountToggle.isOn = false;
				trilithiumCrystalCountToggle.interactable = false;

			}

			//the else condition means that there is a selected unit
			else {

				//now we need to make sure the selected unit is a ship
				//if it's a starbase, we need to handle with slightly diffferent logic
				if (mouseMananger.selectedUnit.GetComponent<Ship> () == null) {

					//this section is if we have a starbase selected
					//now we need to check if storage section 1 is destroyed
					if (mouseMananger.selectedUnit.GetComponentInChildren<StarbaseStorageSection1> ().isDestroyed == true) {

						//if it is destroyed, we can't use useItem commands
						phasorRadarJammimgToggle.isOn = false;
						phasorRadarJammimgToggle.interactable = false;

						useDilithiumCrystalButton.interactable = false;
						dilithiumCrystalCountToggle.isOn = false;
						dilithiumCrystalCountToggle.interactable = false;

					}

					//the else condition is that the storage section 1 is not destroyed.  
					else {

						//set the counts based on inventory
						dilithiumCrystalCountToggle.GetComponentInChildren<TextMeshProUGUI>().text = mouseMananger.selectedUnit.GetComponentInChildren<StarbaseStorageSection1> ().dilithiumCrystals.ToString();

						//if the ship has the radar jamming upgrade, set that toggle to on
						if (mouseMananger.selectedUnit.GetComponentInChildren<StarbaseStorageSection1> ().radarJammingSystem == true) {

							phasorRadarJammimgToggle.isOn = true;
							phasorRadarJammimgToggle.interactable = false;

						}

						//now we need to check to see if we have a unit targeted
						if (mouseMananger.targetedUnit == null) {

							//if there is no targeted unit, we can't choose a section or use a crystal
							crystalTargetingDropdown.interactable = false;

							useDilithiumCrystalButton.interactable = false;
							dilithiumCrystalCountToggle.isOn = false;
							dilithiumCrystalCountToggle.interactable = false;

						}

						//the else condition is that we do have a unit targeted
						else {

							//check if the targeted unit is a ship.  if it is a base, we have to handle slightly differently
							if (mouseMananger.targetedUnit.GetComponent<Ship> () == true) {

								//if a ship is targeted, allow the dropdown to be interactable
								crystalTargetingDropdown.interactable = true;

								//we can't allow the use crystals buttons to be pressed until a section has been chosen
								if (crystalTargetingDropdown.GetComponentInChildren<TextMeshProUGUI> ().text == "Choose Section") {

									useDilithiumCrystalButton.interactable = false;
									dilithiumCrystalCountToggle.isOn = false;
									dilithiumCrystalCountToggle.interactable = false;

								}
								//the else condition is that a section has been chosen, so we can use the use crystals buttons
								else {

									//check if we have crystal inventory to allow usage
									//check if we have dilithium crystals
									if (mouseMananger.selectedUnit.GetComponent<StarbaseStorageSection1> ().dilithiumCrystals > 0) {

										useDilithiumCrystalButton.interactable = true;
										dilithiumCrystalCountToggle.isOn = true;
										dilithiumCrystalCountToggle.interactable = true;

									}
									//the else condition is that we do not have any dilithium crystals, and so the button can't be used
									else {

										useDilithiumCrystalButton.interactable = false;
										dilithiumCrystalCountToggle.isOn = false;
										dilithiumCrystalCountToggle.interactable = false;

									}

								}

							}
							//the else condition is that the targeted unit is a base
							else if (mouseMananger.targetedUnit.GetComponent<Starbase> () == true){

								//if a base is targeted, allow the dropdown to be interactable
								crystalTargetingDropdown.interactable = true;

								//we can't allow the use crystals buttons to be pressed until a section has been chosen
								if (crystalTargetingDropdown.GetComponentInChildren<TextMeshProUGUI> ().text == "Choose Section") {

									useDilithiumCrystalButton.interactable = false;
									dilithiumCrystalCountToggle.isOn = false;
									dilithiumCrystalCountToggle.interactable = false;

								}
								//the else condition is that a section has been chosen, so we can use the use crystals buttons
								else {

									//check if we have crystal inventory to allow usage
									//check if we have dilithium crystals
									if (mouseMananger.selectedUnit.GetComponent<StarbaseStorageSection1> ().dilithiumCrystals > 0) {

										useDilithiumCrystalButton.interactable = true;
										dilithiumCrystalCountToggle.isOn = true;
										dilithiumCrystalCountToggle.interactable = true;

									}
									//the else condition is that we do not have any dilithium crystals, and so the button can't be used
									else {

										useDilithiumCrystalButton.interactable = false;
										dilithiumCrystalCountToggle.isOn = false;
										dilithiumCrystalCountToggle.interactable = false;

									}

								}  //end else if a section has been chosen

							}  //end else if the targeted unit is a starbase

						}  //close else statement that we hae a unit targeted

					}  //close else statement that the selected unit storage section is not destroyed

					//now we need to check if storage section 2 is destroyed
					if (mouseMananger.selectedUnit.GetComponentInChildren<StarbaseStorageSection2> ().isDestroyed == true) {

						//if it is destroyed, we can't use useItem commands
						torpedoLaserScatteringToggle.isOn = false;
						torpedoLaserScatteringToggle.interactable = false;


						flareModeManualButton.interactable = false;
						flareModeAutoButton.interactable = false;

						flareCountToggle.isOn = false;
						flareCountToggle.interactable = false;

						useTrilithiumCrystalButton.interactable = false;
						trilithiumCrystalCountToggle.isOn = false;
						trilithiumCrystalCountToggle.interactable = false;

					}

					//the else condition is that the storage section 2 is not destroyed.  
					else {

						//set the counts based on ship inventory
						trilithiumCrystalCountToggle.GetComponentInChildren<TextMeshProUGUI>().text = mouseMananger.selectedUnit.GetComponentInChildren<StarbaseStorageSection2> ().trilithiumCrystals.ToString();
						flareCountToggle.GetComponentInChildren<TextMeshProUGUI>().text = mouseMananger.selectedUnit.GetComponentInChildren<StarbaseStorageSection2> ().flares.ToString();

						//the flareCount toggle should always be interactable from this point forward
						flareCountToggle.interactable = true;

						//if the ship has the laser scattering upgrade, set that toggle to on
						if (mouseMananger.selectedUnit.GetComponentInChildren<StarbaseStorageSection2> ().laserScatteringSystem == true) {

							torpedoLaserScatteringToggle.isOn = true;
							torpedoLaserScatteringToggle.interactable = false;

						}

						//we can now set the flare mode toggles based on the section variables
						if (mouseMananger.selectedUnit.GetComponentInChildren<StarbaseStorageSection2> ().flareMode == StarbaseStorageSection2.FlareMode.Manual) {

							//set the manual toggle to on and the auto toggle to off
							HighlightButton(flareModeManualButton);
							flareModeManualButton.interactable = true;

							UnhighlightButton(flareModeAutoButton);
							flareModeAutoButton.interactable = true;


						}
						//the else condition is that the flareMode is Automatic
						else if(mouseMananger.selectedUnit.GetComponentInChildren<StarbaseStorageSection2> ().flareMode == StarbaseStorageSection2.FlareMode.Auto){

							//set the auto toggle to on and the manual toggle to off
							HighlightButton(flareModeAutoButton);
							flareModeManualButton.interactable = true;

							UnhighlightButton(flareModeManualButton);
							flareModeAutoButton.interactable = true;

						}

						//now we need to check to see if we have a unit targeted
						if (mouseMananger.targetedUnit == null) {

							//if there is no targeted unit, we can't choose a section or use a crystal
							crystalTargetingDropdown.interactable = false;

							useTrilithiumCrystalButton.interactable = false;
							trilithiumCrystalCountToggle.isOn = false;
							trilithiumCrystalCountToggle.interactable = false;

						}

						//the else condition is that we do have a unit targeted
						else {

							//check if the targeted unit is a ship.  if it is a base, we have to handle slightly differently
							if (mouseMananger.targetedUnit.GetComponent<Ship> () == true) {

								//if a ship is targeted, allow the dropdown to be interactable
								crystalTargetingDropdown.interactable = true;

								//we can't allow the use crystals buttons to be pressed until a section has been chosen
								if (crystalTargetingDropdown.GetComponentInChildren<TextMeshProUGUI> ().text == "Choose Section") {

									useTrilithiumCrystalButton.interactable = false;
									trilithiumCrystalCountToggle.isOn = false;
									trilithiumCrystalCountToggle.interactable = false;

								}
								//the else condition is that a section has been chosen, so we can use the use crystals buttons
								else {

									//check if we have crystal inventory to allow usage
									//check if we have trilithium crystals
									if (mouseMananger.selectedUnit.GetComponent<StarbaseStorageSection2> ().trilithiumCrystals > 0) {

										useTrilithiumCrystalButton.interactable = true;
										trilithiumCrystalCountToggle.isOn = true;
										trilithiumCrystalCountToggle.interactable = true;

									}
									//the else condition is that we do not have any trilithium crystals, and so the button can't be used
									else {

										useTrilithiumCrystalButton.interactable = false;
										trilithiumCrystalCountToggle.isOn = false;
										trilithiumCrystalCountToggle.interactable = false;

									}

								}

							}
							//the else condition is that the targeted unit is a base
							else if (mouseMananger.targetedUnit.GetComponent<Starbase> () == true){

								//if a base is targeted, allow the dropdown to be interactable
								crystalTargetingDropdown.interactable = true;

								//we can't allow the use crystals buttons to be pressed until a section has been chosen
								if (crystalTargetingDropdown.GetComponentInChildren<TextMeshProUGUI> ().text == "Choose Section") {

									useTrilithiumCrystalButton.interactable = false;
									trilithiumCrystalCountToggle.isOn = false;
									trilithiumCrystalCountToggle.interactable = false;

								}
								//the else condition is that a section has been chosen, so we can use the use crystals buttons
								else {

									//check if we have crystal inventory to allow usage
									//check if we have trilithium crystals
									if (mouseMananger.selectedUnit.GetComponent<StarbaseStorageSection2> ().trilithiumCrystals > 0) {

										useTrilithiumCrystalButton.interactable = true;
										trilithiumCrystalCountToggle.isOn = true;
										trilithiumCrystalCountToggle.interactable = true;

									}
									//the else condition is that we do not have any trilithium crystals, and so the button can't be used
									else {

										useTrilithiumCrystalButton.interactable = false;
										trilithiumCrystalCountToggle.isOn = false;
										trilithiumCrystalCountToggle.interactable = false;

									}

								}  //end else if a section has been chosen

							}  //end else if the targeted unit is a starbase

						}  //close else statement that we hae a unit targeted

					}  //close else statement that the selected unit storage section 2 is not destroyed

				}  //close the if statement that we have a starbase selected

				//the else condition is that we do have a ship, so we want to allow toggles based on the ship inventory
				else {

					//check to see if the ship has a storage section.  If not, it can't use the useItem menu
					if (mouseMananger.selectedUnit.GetComponentInChildren<StorageSection> () == null) {

						phasorRadarJammimgToggle.isOn = false;
						phasorRadarJammimgToggle.interactable = false;

						torpedoLaserScatteringToggle.isOn = false;
						torpedoLaserScatteringToggle.interactable = false;


						flareModeManualButton.interactable = false;
						flareModeAutoButton.interactable = false;

						flareCountToggle.isOn = false;
						flareCountToggle.interactable = false;


						crystalTargetingDropdown.interactable = false;

						useDilithiumCrystalButton.interactable = false;
						dilithiumCrystalCountToggle.isOn = false;
						dilithiumCrystalCountToggle.interactable = false;

						useTrilithiumCrystalButton.interactable = false;
						trilithiumCrystalCountToggle.isOn = false;
						trilithiumCrystalCountToggle.interactable = false;

					}

					//the else condition is that there is a storage section in the selected unit
					else {

						//now we need to make sure the storage section hasn't been destroyed
						if (mouseMananger.selectedUnit.GetComponentInChildren<StorageSection> ().isDestroyed == true) {

							//if it is destroyed, we can't use useItem commands
							phasorRadarJammimgToggle.isOn = false;
							phasorRadarJammimgToggle.interactable = false;

							torpedoLaserScatteringToggle.isOn = false;
							torpedoLaserScatteringToggle.interactable = false;


							flareModeManualButton.interactable = false;
							flareModeAutoButton.interactable = false;

							flareCountToggle.isOn = false;
							flareCountToggle.interactable = false;


							crystalTargetingDropdown.interactable = false;

							useDilithiumCrystalButton.interactable = false;
							dilithiumCrystalCountToggle.isOn = false;
							dilithiumCrystalCountToggle.interactable = false;

							useTrilithiumCrystalButton.interactable = false;
							trilithiumCrystalCountToggle.isOn = false;
							trilithiumCrystalCountToggle.interactable = false;

						}

						//the else condition is that the storage section is not destroyed.  
						else {

							//set the counts based on ship inventory
							dilithiumCrystalCountToggle.GetComponentInChildren<TextMeshProUGUI>().text = mouseMananger.selectedUnit.GetComponentInChildren<StorageSection> ().dilithiumCrystals.ToString();
							trilithiumCrystalCountToggle.GetComponentInChildren<TextMeshProUGUI>().text = mouseMananger.selectedUnit.GetComponentInChildren<StorageSection> ().trilithiumCrystals.ToString();
							flareCountToggle.GetComponentInChildren<TextMeshProUGUI>().text = mouseMananger.selectedUnit.GetComponentInChildren<StorageSection> ().flares.ToString();

							//the flareCount toggle should always be interactable from this point forward
							flareCountToggle.interactable = true;

							//if the ship has the radar jamming upgrade, set that toggle to on
							if (mouseMananger.selectedUnit.GetComponentInChildren<StorageSection> ().radarJammingSystem == true) {

								phasorRadarJammimgToggle.isOn = true;
								phasorRadarJammimgToggle.interactable = false;

							}

							//if the ship has the laser scattering upgrade, set that toggle to on
							if (mouseMananger.selectedUnit.GetComponentInChildren<StorageSection> ().laserScatteringSystem == true) {

								torpedoLaserScatteringToggle.isOn = true;
								torpedoLaserScatteringToggle.interactable = false;

							}
								
							//we can now set the flare mode toggles based on the section variables
							if (mouseMananger.selectedUnit.GetComponentInChildren<StorageSection> ().flareMode == StorageSection.FlareMode.Manual) {

								//set the manual toggle to on and the auto toggle to off
								HighlightButton(flareModeManualButton);
								flareModeManualButton.interactable = true;

								UnhighlightButton(flareModeAutoButton);
								flareModeAutoButton.interactable = true;


							}
							//the else condition is that the flareMode is Automatic
							else if(mouseMananger.selectedUnit.GetComponentInChildren<StorageSection> ().flareMode == StorageSection.FlareMode.Auto){

								//set the auto toggle to on and the manual toggle to off
								HighlightButton(flareModeAutoButton);
								flareModeManualButton.interactable = true;

								UnhighlightButton(flareModeManualButton);
								flareModeAutoButton.interactable = true;

							}

							//now we need to check to see if we have a unit targeted
							if (mouseMananger.targetedUnit == null) {

								//if there is no targeted unit, we can't choose a section or use a crystal
								crystalTargetingDropdown.interactable = false;

								useDilithiumCrystalButton.interactable = false;
								dilithiumCrystalCountToggle.isOn = false;
								dilithiumCrystalCountToggle.interactable = false;

								useTrilithiumCrystalButton.interactable = false;
								trilithiumCrystalCountToggle.isOn = false;
								trilithiumCrystalCountToggle.interactable = false;

							}

							//the else condition is that we do have a unit targeted
							else {

								//check if the targeted unit is a ship.  if it is a base, we have to handle slightly differently
								if (mouseMananger.targetedUnit.GetComponent<Ship> () == true) {

									//if a ship is targeted, allow the dropdown to be interactable
									crystalTargetingDropdown.interactable = true;

									//we can't allow the use crystals buttons to be pressed until a section has been chosen
									if (crystalTargetingDropdown.GetComponentInChildren<TextMeshProUGUI> ().text == "Choose Section") {

										useDilithiumCrystalButton.interactable = false;
										dilithiumCrystalCountToggle.isOn = false;
										dilithiumCrystalCountToggle.interactable = false;

										useTrilithiumCrystalButton.interactable = false;
										trilithiumCrystalCountToggle.isOn = false;
										trilithiumCrystalCountToggle.interactable = false;

									}
									//the else condition is that a section has been chosen, so we can use the use crystals buttons
									else {

										//check if we have crystal inventory to allow usage
										//check if we have dilithium crystals
										if (mouseMananger.selectedUnit.GetComponent<StorageSection> ().dilithiumCrystals > 0) {

											useDilithiumCrystalButton.interactable = true;
											dilithiumCrystalCountToggle.isOn = true;
											dilithiumCrystalCountToggle.interactable = true;

										}
										//the else condition is that we do not have any dilithium crystals, and so the button can't be used
										else {

											useDilithiumCrystalButton.interactable = false;
											dilithiumCrystalCountToggle.isOn = false;
											dilithiumCrystalCountToggle.interactable = false;

										}

										//check if we have trilithium crystals
										if (mouseMananger.selectedUnit.GetComponent<StorageSection> ().trilithiumCrystals > 0) {

											useTrilithiumCrystalButton.interactable = true;
											trilithiumCrystalCountToggle.isOn = true;
											trilithiumCrystalCountToggle.interactable = true;

										}
										//the else condition is that we do not have any trilithium crystals, and so the button can't be used
										else {

											useTrilithiumCrystalButton.interactable = false;
											trilithiumCrystalCountToggle.isOn = false;
											trilithiumCrystalCountToggle.interactable = false;

										}

									}

								}
								//the else condition is that the targeted unit is a base
								else if (mouseMananger.targetedUnit.GetComponent<Starbase> () == true){
									
									//if a base is targeted, allow the dropdown to be interactable
									crystalTargetingDropdown.interactable = true;

									//we can't allow the use crystals buttons to be pressed until a section has been chosen
									if (crystalTargetingDropdown.GetComponentInChildren<TextMeshProUGUI> ().text == "Choose Section") {

										useDilithiumCrystalButton.interactable = false;
										dilithiumCrystalCountToggle.isOn = false;
										dilithiumCrystalCountToggle.interactable = false;

										useTrilithiumCrystalButton.interactable = false;
										trilithiumCrystalCountToggle.isOn = false;
										trilithiumCrystalCountToggle.interactable = false;

									}
									//the else condition is that a section has been chosen, so we can use the use crystals buttons
									else {

										//check if we have crystal inventory to allow usage
										//check if we have dilithium crystals
										if (mouseMananger.selectedUnit.GetComponent<StorageSection> ().dilithiumCrystals > 0) {

											useDilithiumCrystalButton.interactable = true;
											dilithiumCrystalCountToggle.isOn = true;
											dilithiumCrystalCountToggle.interactable = true;

										}
										//the else condition is that we do not have any dilithium crystals, and so the button can't be used
										else {

											useDilithiumCrystalButton.interactable = false;
											dilithiumCrystalCountToggle.isOn = false;
											dilithiumCrystalCountToggle.interactable = false;

										}

										//check if we have trilithium crystals
										if (mouseMananger.selectedUnit.GetComponent<StorageSection> ().trilithiumCrystals > 0) {

											useTrilithiumCrystalButton.interactable = true;
											trilithiumCrystalCountToggle.isOn = true;
											trilithiumCrystalCountToggle.interactable = true;

										}
										//the else condition is that we do not have any trilithium crystals, and so the button can't be used
										else {

											useTrilithiumCrystalButton.interactable = false;
											trilithiumCrystalCountToggle.isOn = false;
											trilithiumCrystalCountToggle.interactable = false;

										}

									}  //end else if a section has been chosen
										
								}  //end else if the targeted unit is a starbase

							}  //close else statement that we hae a unit targeted

						}  //close else statement that the selected unit storage section is not destroyed

					}  //close else statement that selected unit has a storage section

				}  //close else statement that selected unit is a ship

			}  //close the else statement that there is a selected unit

			SetTargetingText ();
			//SetTargetingSectionOptions ();
			crystalTargetingDropdown.RefreshShownValue ();

		}

		//this is if the useItem menu toggle has been turned off
		else if (useItemToggle.isOn == false) {

			//if we have don't have a selected unit, we can turn the toggles off and make them not interactable
			if (mouseMananger.selectedUnit == null) {

				phasorRadarJammimgToggle.isOn = false;
				phasorRadarJammimgToggle.interactable = false;

				torpedoLaserScatteringToggle.isOn = false;
				torpedoLaserScatteringToggle.interactable = false;

				flareModeManualButton.interactable = false;
				flareModeAutoButton.interactable = false;

				flareCountToggle.isOn = false;
				flareCountToggle.interactable = false;


				crystalTargetingDropdown.interactable = false;

				useDilithiumCrystalButton.interactable = false;
				dilithiumCrystalCountToggle.isOn = false;
				dilithiumCrystalCountToggle.interactable = false;

				useTrilithiumCrystalButton.interactable = false;
				trilithiumCrystalCountToggle.isOn = false;
				trilithiumCrystalCountToggle.interactable = false;

				SetTargetingText ();

			}

			//now we can check if we have a targeted unit
			if (mouseMananger.targetedUnit == null) {

				//if there is no targeted unit anymore, we can set the toggles off and make them not interactable
				phasorRadarJammimgToggle.isOn = false;
				phasorRadarJammimgToggle.interactable = false;

				torpedoLaserScatteringToggle.isOn = false;
				torpedoLaserScatteringToggle.interactable = false;

				flareModeManualButton.interactable = false;
				flareModeAutoButton.interactable = false;

				flareCountToggle.isOn = false;
				flareCountToggle.interactable = false;


				crystalTargetingDropdown.interactable = false;

				useDilithiumCrystalButton.interactable = false;
				dilithiumCrystalCountToggle.isOn = false;
				dilithiumCrystalCountToggle.interactable = false;

				useTrilithiumCrystalButton.interactable = false;
				trilithiumCrystalCountToggle.isOn = false;
				trilithiumCrystalCountToggle.interactable = false;

				SetTargetingText ();

			}

		}

	}

	//this function sets the helper text for the crystal targeting
	private void SetTargetingText(){

		//check if there is a targeted unit
		if (mouseMananger.targetedUnit != null) {

			//check if the targeted unit is a ship
			if (mouseMananger.targetedUnit.GetComponent<Ship> () == true) {

				crystalTargetingText.text = ("Choose Section");

				//update the font size if necessary
				UIManager.AutoSizeTextMeshFont(crystalTargetingText.GetComponentInChildren<TextMeshProUGUI> ());

			}
			//the else condition is that the targeted unit is not a ship
			else if (mouseMananger.targetedUnit.GetComponent<Starbase> () == true) {

				crystalTargetingText.text = ("Choose Section");

				//update the font size if necessary
				UIManager.AutoSizeTextMeshFont(crystalTargetingText.GetComponentInChildren<TextMeshProUGUI> ());

			}

		}
		//the else condition is that there is no targeted unit
		else {

			//check whether we have a selected unit
			if (mouseMananger.selectedUnit != null) {

				crystalTargetingText.text = ("No Unit Targeted");

				//update the font size if necessary
				UIManager.AutoSizeTextMeshFont(crystalTargetingText.GetComponentInChildren<TextMeshProUGUI> ());

			}
			//the else condition is that there is no selected unit
			else {

				crystalTargetingText.text = ("No Unit Selected");

				//update the font size if necessary
				UIManager.AutoSizeTextMeshFont(crystalTargetingText.GetComponentInChildren<TextMeshProUGUI> ());

			}

		}

	}

	//this function will set the available options for the targeting dropdown menu
	private void SetTargetingSectionOptions(){

		//start by clearing the existing dropdown options
		crystalTargetingDropdown.ClearOptions();

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

					//check if the engine section is not destroyed
					if (mouseMananger.targetedUnit.GetComponent<EngineSection> ().isDestroyed == false) {

						//check that the phasor section is not at maximum shields already
						if (mouseMananger.targetedUnit.GetComponent<EngineSection> ().shieldsCurrent < mouseMananger.targetedUnit.GetComponent<EngineSection> ().shieldsMax) {

							//add a dropdown option for the engine section
							dropDownOptions.Add (new TMP_Dropdown.OptionData ("Engine Section"));

						}

					}

				}

				//check if the targeted ship has a phasor section
				if (mouseMananger.targetedUnit.GetComponent<PhasorSection> () == true) {

					//check if the phasor section is not destroyed
					if (mouseMananger.targetedUnit.GetComponent<PhasorSection> ().isDestroyed == false) {

						//check that the phasor section is not at maximum shields already
						if (mouseMananger.targetedUnit.GetComponent<PhasorSection> ().shieldsCurrent < mouseMananger.targetedUnit.GetComponent<PhasorSection> ().shieldsMax) {

							//add a dropdown option for the phasor section
							dropDownOptions.Add (new TMP_Dropdown.OptionData ("Phasor Section"));

						}

					}

				}

				//check if the targeted ship has a torpedo section
				if (mouseMananger.targetedUnit.GetComponent<TorpedoSection> () == true) {

					//check if the torpedo section is not destroyed
					if (mouseMananger.targetedUnit.GetComponent<TorpedoSection> ().isDestroyed == false) {

						//check that the phasor section is not at maximum shields already
						if (mouseMananger.targetedUnit.GetComponent<TorpedoSection> ().shieldsCurrent < mouseMananger.targetedUnit.GetComponent<TorpedoSection> ().shieldsMax) {
							
							//add a dropdown option for the torpedo section
							dropDownOptions.Add (new TMP_Dropdown.OptionData ("Torpedo Section"));

						}

					}

				}

				//check if the targeted ship has a crew section
				if (mouseMananger.targetedUnit.GetComponent<CrewSection> () == true) {

					//check if the crew section is not destroyed
					if (mouseMananger.targetedUnit.GetComponent<CrewSection> ().isDestroyed == false) {

						//check that the phasor section is not at maximum shields already
						if (mouseMananger.targetedUnit.GetComponent<CrewSection> ().shieldsCurrent < mouseMananger.targetedUnit.GetComponent<CrewSection> ().shieldsMax) {

							//add a dropdown option for the crew section
							dropDownOptions.Add (new TMP_Dropdown.OptionData ("Crew Section"));

						}

					}

				}

				//check if the targeted ship has a storage section
				if (mouseMananger.targetedUnit.GetComponent<StorageSection> () == true) {

					//check if the storage section is not destroyed
					if (mouseMananger.targetedUnit.GetComponent<StorageSection> ().isDestroyed == false) {

						//check that the phasor section is not at maximum shields already
						if (mouseMananger.targetedUnit.GetComponent<StorageSection> ().shieldsCurrent < mouseMananger.targetedUnit.GetComponent<StorageSection> ().shieldsMax) {
							
							//add a dropdown option for the storage section
							dropDownOptions.Add (new TMP_Dropdown.OptionData ("Storage Section"));

						}

					}

				}

			}
			//the else condition is that the targeted unit is not a ship, so it's a base
			else if (mouseMananger.targetedUnit.GetComponent<Starbase> () == true){

				//check if the phasor section 1 is not destroyed
				if (mouseMananger.targetedUnit.GetComponent<StarbasePhasorSection1> ().isDestroyed == false) {

					//check that the phasor section 1 is not at maximum shields already
					if (mouseMananger.targetedUnit.GetComponent<StarbasePhasorSection1> ().shieldsCurrent < mouseMananger.targetedUnit.GetComponent<StarbasePhasorSection1> ().shieldsMax) {

						//add a dropdown option for the phasor section 1
						dropDownOptions.Add (new TMP_Dropdown.OptionData ("Phasor Section 1"));

					}

				}

				//check if the phasor section 2 is not destroyed
				if (mouseMananger.targetedUnit.GetComponent<StarbasePhasorSection2> ().isDestroyed == false) {

					//check that the phasor section 2 is not at maximum shields already
					if (mouseMananger.targetedUnit.GetComponent<StarbasePhasorSection2> ().shieldsCurrent < mouseMananger.targetedUnit.GetComponent<StarbasePhasorSection2> ().shieldsMax) {

						//add a dropdown option for the phasor section 2
						dropDownOptions.Add (new TMP_Dropdown.OptionData ("Phasor Section 2"));

					}

				}

				//check if the torpedo section is not destroyed
				if (mouseMananger.targetedUnit.GetComponent<StarbaseTorpedoSection> ().isDestroyed == false) {

					//check that the torpedo section is not at maximum shields already
					if (mouseMananger.targetedUnit.GetComponent<StarbaseTorpedoSection> ().shieldsCurrent < mouseMananger.targetedUnit.GetComponent<StarbaseTorpedoSection> ().shieldsMax) {

						//add a dropdown option for the torpedo section
						dropDownOptions.Add (new TMP_Dropdown.OptionData ("Torpedo Section"));

					}

				}

				//check if the crew section is not destroyed
				if (mouseMananger.targetedUnit.GetComponent<StarbaseCrewSection> ().isDestroyed == false) {

					//check that the crew section is not at maximum shields already
					if (mouseMananger.targetedUnit.GetComponent<StarbaseCrewSection> ().shieldsCurrent < mouseMananger.targetedUnit.GetComponent<StarbaseCrewSection> ().shieldsMax) {

						//add a dropdown option for the crew section
						dropDownOptions.Add (new TMP_Dropdown.OptionData ("Crew Section"));

					}

				}

				//check if the storage section 1 is not destroyed
				if (mouseMananger.targetedUnit.GetComponent<StarbaseStorageSection1> ().isDestroyed == false) {

					//check that the storage section 1 is not at maximum shields already
					if (mouseMananger.targetedUnit.GetComponent<StarbaseStorageSection1> ().shieldsCurrent < mouseMananger.targetedUnit.GetComponent<StarbaseStorageSection1> ().shieldsMax) {

						//add a dropdown option for the storage section 1
						dropDownOptions.Add (new TMP_Dropdown.OptionData ("Storage Section 1"));

					}

				}

				//check if the storage section 2 is not destroyed
				if (mouseMananger.targetedUnit.GetComponent<StarbaseStorageSection2> ().isDestroyed == false) {

					//check that the storage section 2 is not at maximum shields already
					if (mouseMananger.targetedUnit.GetComponent<StarbaseStorageSection2> ().shieldsCurrent < mouseMananger.targetedUnit.GetComponent<StarbaseStorageSection2> ().shieldsMax) {

						//add a dropdown option for the storage section 2
						dropDownOptions.Add (new TMP_Dropdown.OptionData ("Storage Section 2"));

					}

				}

			}

		}

		//add the options to the dropdown
		crystalTargetingDropdown.AddOptions (dropDownOptions);

		//update the font size if necessary
		UIManager.AutoSizeTextMeshFont(crystalTargetingDropdown.GetComponentInChildren<TextMeshProUGUI> ());

	}

	//this function handles using a dilithium crystal
	private void UseDilithiumCrystal(){

		//Debug.Log ("Use Dilithium Crsytal!");

		//invoke the OnUseDilithiumCrsytal event, with the selected unit, targeted unit, and string of section targeted passed to the event
		OnUseDilithiumCrystal.Invoke(mouseMananger.selectedUnit.GetComponent<CombatUnit>(),
			mouseMananger.targetedUnit.GetComponent<CombatUnit>(),
			crystalTargetingDropdown.GetComponentInChildren<TextMeshProUGUI>().text);

		SetUseItemMenuToggles ();

	}

	//this function handles using a trilithium crystal
	private void UseTrilithiumCrystal(){

		//Debug.Log ("Use Trilithium Crystal!");

		//invoke the OnUseTrilithiumCrsytal event, with the selected unit, targeted unit, and string of section targeted passed to the event
		OnUseTrilithiumCrystal.Invoke(mouseMananger.selectedUnit.GetComponent<CombatUnit>(),
			mouseMananger.targetedUnit.GetComponent<CombatUnit>(),
			crystalTargetingDropdown.GetComponentInChildren<TextMeshProUGUI>().text);

		SetUseItemMenuToggles ();

	}

	//this function handles setting the flareMode to manual
	private void SetFlareModeToManual(){
		
		//check to make sure we have a unit selected
		if (mouseMananger.selectedUnit != null) {

			//higlight the button
			HighlightButton(flareModeManualButton);

			//unlighlight the other button
			UnhighlightButton(flareModeAutoButton);

			//invoke the event for setting the flaremode, passing the selected unit
			OnSetFlareModeToManual.Invoke (mouseMananger.selectedUnit.GetComponent<CombatUnit> ());

		}

	}

	//this function handles setting the flareMode to auto
	private void SetFlareModeToAuto(){

		//check to make sure we have a unit selected
		if (mouseMananger.selectedUnit != null) {

			//higlight the button
			HighlightButton(flareModeAutoButton);

			//unlighlight the other button
			UnhighlightButton(flareModeManualButton);

			//invoke the event for setting the flaremode, passing the selected unit
			OnSetFlareModeToAuto.Invoke (mouseMananger.selectedUnit.GetComponent<CombatUnit> ());

		}

	}

	//this function highlights a button passed to it
	private void HighlightButton(Button highlightedButton){

		ColorBlock colorBlock = highlightedButton.colors;
		colorBlock.normalColor = selectedButtonColor;
		colorBlock.highlightedColor = selectedButtonColor;
		highlightedButton.colors = colorBlock;

	}

	//this function unhighlights a button passed to it
	private void UnhighlightButton(Button unhighlightedButton){

		ColorBlock colorBlock = unhighlightedButton.colors;
		colorBlock = ColorBlock.defaultColorBlock;
		unhighlightedButton.colors = colorBlock;

	}

	//this function handles OnDestroy
	private void OnDestroy(){

		RemoveAllListeners ();

	}

	//this function removes all listeners
	private void RemoveAllListeners(){

		if (useItemToggle != null) {

			//add an on-click event listener for the main menu useItem toggle
			useItemToggle.onValueChanged.AddListener (boolSetUseItemMenuTogglesAction);

		}

		if (crystalTargetingDropdown != null) {

			//add an event listener for the dropdown value changing
			crystalTargetingDropdown.onValueChanged.AddListener (intSetUseItemMenuTogglesAction);

		}

		if (useDilithiumCrystalButton != null) {

			//add an on-click event listener for the useCrystals buttons
			useDilithiumCrystalButton.onClick.AddListener (UseDilithiumCrystal);

		}

		if(useTrilithiumCrystalButton != null){

			useTrilithiumCrystalButton.onClick.AddListener (UseTrilithiumCrystal);

		}

		if (mouseMananger != null) {

			//add an event listener for when a selectedUnit is set and cleared
			//I want to set the dropdown back to the default when the selected unit changes
			mouseMananger.OnSetSelectedUnit.AddListener (setUseItemMenuTogglesDropdownZeroAction);
			mouseMananger.OnClearSelectedUnit.AddListener (setUseItemMenuTogglesDropdownZeroAction);

			//add an event listener for when a targetedUnit is set and cleared
			//I want to set the dropdown back to the default when the selected unit changes
			mouseMananger.OnSetTargetedUnit.AddListener (setUseItemMenuTogglesDropdownZeroAction);
			mouseMananger.OnClearTargetedUnit.AddListener (setUseItemMenuTogglesDropdownZeroAction);

		}

		if (flareModeManualButton != null) {

			//add an on-click event listener for the flareMode manual
			flareModeManualButton.onClick.AddListener (clickSetFlareModeToManualAction);

		}

		if (flareModeAutoButton != null) {
			
			//add an on-click event listener for the flareMode automatic
			flareModeAutoButton.onClick.AddListener (clickSetFlareModeToAutoAction);

		}

	}

}
