using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class PhasorMenu : MonoBehaviour {

	//we will need the mouseManager to check the selected unit
	private MouseManager mouseMananger;

	//variable for phasor trigger toggle
	public Toggle phasorToggle;

	//variable for phasor Menu phasor radar shot toggle
	public Toggle usePhasorRadarShotToggle;

	//variable for the system upgrages
	public Toggle phasorRadarArrayToggle;
	public Toggle phasorXRayKernelToggle;

	//variable to hold the fire button
	public Button phasorFireButton;

	//variables for the item counts
	public Toggle phasorRadarShotCountToggle;

	//variable for the targeting dropdown
	public TMP_Dropdown phasorTargetingDropdown;

	//variable for the targeting helper text
	public TextMeshProUGUI phasorTargetingText;


	//these events are for when the phasors are fired are clicked
	public PhasorEvent OnFirePhasors = new PhasorEvent();

	//simple class derived from unityEvent to pass combat units
	public class PhasorEvent : UnityEvent<CombatUnit, CombatUnit, string>{};

	//unityActions
	private UnityAction<bool> boolSetPhasorMenuToggleAction;
	private UnityAction<bool> boolSetPhasorMenuToggleDropdownZeroAction;
	private UnityAction<int> intSetPhasorMenuToggleAction;
	private UnityAction<CombatUnit> combatUnitSetPhasorMenuToggleAction;


	// Use this for initialization
	public void Init () {

		//get the mouseManager
		mouseMananger = GameObject.FindGameObjectWithTag("MouseManager").GetComponent<MouseManager>();

		//set the actions
		boolSetPhasorMenuToggleAction = (value) => {SetPhasorMenuToggles();};
		boolSetPhasorMenuToggleDropdownZeroAction = (value) => {
			SetPhasorMenuToggles();
			phasorTargetingDropdown.value = 0;		//I want the menu to go back to "Choose Section" when we toggle targeting
		};

		intSetPhasorMenuToggleAction = (value) => {SetPhasorMenuToggles();};
		combatUnitSetPhasorMenuToggleAction = (combatUnit) => {SetPhasorMenuToggles();};


		//add an on-click event listener for the main menu phasor toggle
		phasorToggle.onValueChanged.AddListener(boolSetPhasorMenuToggleAction);

		//add an on-click event listener for the phasor radar shot toggle
		usePhasorRadarShotToggle.onValueChanged.AddListener(boolSetPhasorMenuToggleDropdownZeroAction);

		//add an event listener for the dropdown value changing
		phasorTargetingDropdown.onValueChanged.AddListener(intSetPhasorMenuToggleAction);

		//add an on-click event listener for the fire phasors toggle
		phasorFireButton.onClick.AddListener(FirePhasors);

		//add an event listener for when a selectedUnit is set and cleared
		mouseMananger.OnSetSelectedUnit.AddListener(SetPhasorMenuToggles);
		mouseMananger.OnClearSelectedUnit.AddListener(SetPhasorMenuToggles);

		//add an event listener for when a targetedUnit is set and cleared
		mouseMananger.OnSetTargetedUnit.AddListener(SetPhasorMenuToggles);
		mouseMananger.OnClearTargetedUnit.AddListener(SetPhasorMenuToggles);

		//add a listener for when a purchase is complete
		PhasorSection.OnInventoryUpdated.AddListener(combatUnitSetPhasorMenuToggleAction);
		
	}

	//this function sets the interactability of the phasor menu toggles
	private void SetPhasorMenuToggles(){

		//this is if the Phasor Attack menu Toggle has been turned on
		if (phasorToggle.isOn == true) {

			//if there is no unit selected, set toggles to not interactable and turn them off
			if (mouseMananger.selectedUnit == null) {

				usePhasorRadarShotToggle.isOn = false;
				usePhasorRadarShotToggle.interactable = false;

				phasorRadarShotCountToggle.isOn = false;
				phasorRadarShotCountToggle.interactable = false;

				phasorRadarArrayToggle.isOn = false;
				phasorRadarArrayToggle.interactable = false;

				phasorXRayKernelToggle.isOn = false;
				phasorXRayKernelToggle.interactable = false;



				phasorTargetingDropdown.interactable = false;

				phasorFireButton.interactable = false;

			}

			//the else condition means that there is a selected unit
			else {

				//now we need to make sure the selected unit is a ship
				//if it's a starbase, we need to handle with slightly diffferent logic
				if (mouseMananger.selectedUnit.GetComponent<Ship> () == null) {

					//if we are here, that means that the selected unit is a starbase
					//check to see if both phasor sections have been destroyed
					if (mouseMananger.selectedUnit.GetComponent<StarbasePhasorSection1> ().isDestroyed == true &&
					    mouseMananger.selectedUnit.GetComponent<StarbasePhasorSection2> ().isDestroyed == true) {

						//if both are destroyed, we can't use phasor commands
						usePhasorRadarShotToggle.isOn = false;
						usePhasorRadarShotToggle.interactable = false;

						phasorRadarShotCountToggle.isOn = false;
						phasorRadarShotCountToggle.interactable = false;

						phasorRadarArrayToggle.isOn = false;
						phasorRadarArrayToggle.interactable = false;

						phasorXRayKernelToggle.isOn = false;
						phasorXRayKernelToggle.interactable = false;

						phasorTargetingDropdown.interactable = false;

						phasorFireButton.interactable = false;

					}
					//the else condition is that at least one starbase phasor section is still alive
					else {

						//set the counts based on ship inventory
						phasorRadarShotCountToggle.GetComponentInChildren<TextMeshProUGUI>().text = mouseMananger.selectedUnit.GetComponentInChildren<StarbasePhasorSection1> ().phasorRadarShot.ToString();

						//if the base has the X-ray kernel upgrade, set that toggle to on
						if (mouseMananger.selectedUnit.GetComponentInChildren<StarbasePhasorSection2> ().xRayKernalUpgrade == true) {

							phasorXRayKernelToggle.isOn = true;
							phasorXRayKernelToggle.interactable = false;

						}

						//if the starbase has the radar array upgrade, set that toggle to on and disable radar shot single use items
						if (mouseMananger.selectedUnit.GetComponentInChildren<StarbasePhasorSection1> ().phasorRadarArray == true) {

							phasorRadarArrayToggle.isOn = true;
							phasorRadarArrayToggle.interactable = false;

							usePhasorRadarShotToggle.isOn = false;
							usePhasorRadarShotToggle.interactable = false;

							phasorRadarShotCountToggle.isOn = false;
							phasorRadarShotCountToggle.interactable = false;

						}

						//now we need to check to see if we've already used phasors this turn
						if (mouseMananger.selectedUnit.GetComponentInChildren<StarbasePhasorSection1> ().usedPhasorsThisTurn == true ||
						    mouseMananger.selectedUnit.GetComponentInChildren<StarbasePhasorSection2> ().usedPhasorsThisTurn == true) {

							//if we've already used phasors, we can't use a radar shot or use the fire button
							usePhasorRadarShotToggle.isOn = false;
							usePhasorRadarShotToggle.interactable = false;

							phasorRadarShotCountToggle.isOn = false;
							phasorRadarShotCountToggle.interactable = false;

							phasorTargetingDropdown.interactable = false;

							phasorFireButton.interactable = false;

						}
						//check if we've used torpedos and don't have an additional battle crew - we only get 1 attack per turn without the crew
						else if (mouseMananger.selectedUnit.GetComponent<StarbaseTorpedoSection> ().usedTorpedosThisTurn == true &&
						        mouseMananger.selectedUnit.GetComponent<StarbaseCrewSection> ().battleCrew == false) {

							//if we've already used torpedos and don't have an additional battle crew, we can't use a radar shot or use the fire button
							usePhasorRadarShotToggle.isOn = false;
							usePhasorRadarShotToggle.interactable = false;

							phasorRadarShotCountToggle.isOn = false;
							phasorRadarShotCountToggle.interactable = false;

							phasorTargetingDropdown.interactable = false;

							phasorFireButton.interactable = false;

						}
						//the else condition is that we have not used phasors yet and still have a valid attack
						else {

							//we can now set the phasor radar shot toggle based on inventory
							if (mouseMananger.selectedUnit.GetComponentInChildren<StarbasePhasorSection1> ().phasorRadarShot <= 0) {

								//if radar shot == 0, we have none and cannot use it
								usePhasorRadarShotToggle.isOn = false;
								usePhasorRadarShotToggle.interactable = false;

								phasorRadarShotCountToggle.isOn = false;
								phasorRadarShotCountToggle.interactable = false;

							}
							//the else condition is that the radar shot quantity is non-zero
							else {
								
								//check if we have a radar array
								if (mouseMananger.selectedUnit.GetComponentInChildren<StarbasePhasorSection1> ().phasorRadarArray == true) {

									//if we have the array, the shot can't be used
									usePhasorRadarShotToggle.interactable = false;
									phasorRadarShotCountToggle.interactable = false;

								} else {

									usePhasorRadarShotToggle.interactable = true;
									phasorRadarShotCountToggle.interactable = true;

								}

							}

							//now we need to check to see if we have a unit targeted
							if (mouseMananger.targetedUnit == null) {

								//if there is no targeted unit, we can't fire the phasors
								phasorTargetingDropdown.interactable = false;

								phasorFireButton.interactable = false;

							}

							//the else condition is that we do have a unit targeted
							else {

								//check if the targeted unit has a cloaking device turned on
								if (mouseMananger.targetedUnit.GetComponent<CloakingDevice> () && mouseMananger.targetedUnit.GetComponent<CloakingDevice> ().isCloaked == true) {

									//if it is cloaked, we can't allow using targeting
									usePhasorRadarShotToggle.interactable = false;

									phasorRadarShotCountToggle.interactable = false;

									//check if we have radar shot enabled - if we do, we need to turn it off
									if (usePhasorRadarShotToggle.isOn == true) {

										usePhasorRadarShotToggle.isOn = false;
										phasorTargetingDropdown.interactable = false;

									}
									//the else condition is that we do not have a targeting device enabled
									else {

										phasorTargetingDropdown.interactable = false;

									}

									//if the targeted unit is cloaked, we can allow the fire phasors button to be pressed
									phasorFireButton.interactable = true;

								}
								//the else condition is that the targeted unit is not cloaked
								else{

									//check if the targeted unit is a ship.  if it is a base, we have to handle slightly differently
									if(mouseMananger.targetedUnit.GetComponent<Ship>() == true){

										//check if the targeted unit has a storage section
										if (mouseMananger.targetedUnit.GetComponent<StorageSection> () == true) {

											//now check if the targeted ship has radar jamming
											if (mouseMananger.targetedUnit.GetComponent<StorageSection> ().radarJammingSystem == true) {

												//if it does, we can't allow using targeting
												usePhasorRadarShotToggle.interactable = false;

												phasorRadarShotCountToggle.interactable = false;

												//check if we have radar shot enabled - if we do, we need to turn it off
												if (usePhasorRadarShotToggle.isOn == true) {

													usePhasorRadarShotToggle.isOn = false;
													phasorTargetingDropdown.interactable = false;

												}
												//the else condition is that we do not have a targeting device enabled
												else {

													phasorTargetingDropdown.interactable = false;

												}

												//if the targeted ship has jamming, we can allow the fire phasors button to be pressed
												phasorFireButton.interactable = true;

											}
											//the else condition is that there is no jamming on the targeted unit
											else {

												//we can now set the phasor radar shot toggle based on inventory
												if (mouseMananger.selectedUnit.GetComponentInChildren<StarbasePhasorSection1> ().phasorRadarShot <= 0) {

													//if radar shot == 0, we have none and cannot use it
													usePhasorRadarShotToggle.isOn = false;
													usePhasorRadarShotToggle.interactable = false;

													phasorRadarShotCountToggle.isOn = false;
													phasorRadarShotCountToggle.interactable = false;

												}
												//the else condition is that the radar shot quantity is non-zero
												else {

													//check if we have a radar array
													if (mouseMananger.selectedUnit.GetComponentInChildren<StarbasePhasorSection1> ().phasorRadarArray == true) {

														//if we have the array, the shot can't be used
														usePhasorRadarShotToggle.interactable = false;
														phasorRadarShotCountToggle.interactable = false;

													} else {

														usePhasorRadarShotToggle.interactable = true;
														phasorRadarShotCountToggle.interactable = true;

													}

												}

												//check if we have targeting device enabled
												if (phasorRadarArrayToggle.isOn == true || usePhasorRadarShotToggle.isOn == true) {

													phasorTargetingDropdown.interactable = true;

													//if we have targeting enabled, we can't allow the fire button to be pressed until a section has been chosen
													if (phasorTargetingDropdown.GetComponentInChildren<TextMeshProUGUI>().text == "Choose Section") {

														phasorFireButton.interactable = false;

													}
													//the else condition is that a section has been chosen, so we can use the fire button
													else {

														phasorFireButton.interactable = true;

													}


												}
												//the else condition is that we do not have a targeting device enabled
												else {

													phasorTargetingDropdown.interactable = false;

													phasorFireButton.interactable = true;

												}

											}

										}
										//the else condition is that the targeted ship does not have a storage section
										else {

											//if there is no storage section, there can be no jamming
											//we can now set the phasor radar shot toggle based on inventory
											if (mouseMananger.selectedUnit.GetComponentInChildren<StarbasePhasorSection1> ().phasorRadarShot <= 0) {

												//if radar shot == 0, we have none and cannot use it
												usePhasorRadarShotToggle.isOn = false;
												usePhasorRadarShotToggle.interactable = false;

												phasorRadarShotCountToggle.isOn = false;
												phasorRadarShotCountToggle.interactable = false;

											}
											//the else condition is that the radar shot quantity is non-zero
											else {

												//check if we have a radar array
												if (mouseMananger.selectedUnit.GetComponentInChildren<StarbasePhasorSection1> ().phasorRadarArray == true) {

													//if we have the array, the shot can't be used
													usePhasorRadarShotToggle.interactable = false;
													phasorRadarShotCountToggle.interactable = false;

												} else {

													usePhasorRadarShotToggle.interactable = true;
													phasorRadarShotCountToggle.interactable = true;

												}

											}

											//check if we have targeting device enabled
											if (phasorRadarArrayToggle.isOn == true || usePhasorRadarShotToggle.isOn == true) {

												phasorTargetingDropdown.interactable = true;

												//if we have targeting enabled, we can't allow the fire button to be pressed until a section has been chosen
												if (phasorTargetingDropdown.GetComponentInChildren<TextMeshProUGUI>().text == "Choose Section") {

													phasorFireButton.interactable = false;

												}
												//the else condition is that a section has been chosen, so we can use the fire button
												else {

													phasorFireButton.interactable = true;

												}

											}
											//the else condition is that we do not have a targeting device enabled
											else {

												phasorTargetingDropdown.interactable = false;

												phasorFireButton.interactable = true;

											}

										}

									}
									//the else condition is that the targeted unit is a base
									else if(mouseMananger.targetedUnit.GetComponent<Starbase>() == true){

										//now check if the targeted ship has radar jamming
										if (mouseMananger.targetedUnit.GetComponent<StarbaseStorageSection1> ().radarJammingSystem == true) {

											//if it does, we can't allow using targeting
											usePhasorRadarShotToggle.interactable = false;

											phasorRadarShotCountToggle.interactable = false;

											//check if we have radar shot enabled - if we do, we need to turn it off
											if (usePhasorRadarShotToggle.isOn == true) {

												usePhasorRadarShotToggle.isOn = false;
												phasorTargetingDropdown.interactable = false;

											}
											//the else condition is that we do not have a targeting device enabled
											else {

												phasorTargetingDropdown.interactable = false;

											}

											//if the targeted ship has jamming, we can allow the fire phasors button to be pressed
											phasorFireButton.interactable = true;

										}
										//the else condition is that there is no jamming on the targeted unit
										else {

											//we can now set the phasor radar shot toggle based on inventory
											if (mouseMananger.selectedUnit.GetComponentInChildren<StarbasePhasorSection1> ().phasorRadarShot <= 0) {

												//if radar shot == 0, we have none and cannot use it
												usePhasorRadarShotToggle.isOn = false;
												usePhasorRadarShotToggle.interactable = false;

												phasorRadarShotCountToggle.isOn = false;
												phasorRadarShotCountToggle.interactable = false;

											}
											//the else condition is that the radar shot quantity is non-zero
											else {

												//check if we have a radar array
												if (mouseMananger.selectedUnit.GetComponentInChildren<StarbasePhasorSection1> ().phasorRadarArray == true) {

													//if we have the array, the shot can't be used
													usePhasorRadarShotToggle.interactable = false;
													phasorRadarShotCountToggle.interactable = false;

												} else {

													usePhasorRadarShotToggle.interactable = true;
													phasorRadarShotCountToggle.interactable = true;

												}

											}

											//check if we have targeting device enabled
											if (phasorRadarArrayToggle.isOn == true || usePhasorRadarShotToggle.isOn == true) {

												phasorTargetingDropdown.interactable = true;

												//if we have targeting enabled, we can't allow the fire button to be pressed until a section has been chosen
												if (phasorTargetingDropdown.GetComponentInChildren<TextMeshProUGUI>().text == "Choose Section") {

													phasorFireButton.interactable = false;

												}
												//the else condition is that a section has been chosen, so we can use the fire button
												else {

													phasorFireButton.interactable = true;

												}


											}
											//the else condition is that we do not have a targeting device enabled
											else {

												phasorTargetingDropdown.interactable = false;

												phasorFireButton.interactable = true;

											}

										} //end else there is no jamming

									}  //end else if the targeted unit is a base

								} //end else the target is not cloaked

							} //end else we do have a unit targeted

						} // end else we have not used phasors yet

					} //end else we have at least 1 phasor section alive

				} //end if the selected unit is a starbase

				//the else condition is that we do have a ship, so we want to allow toggles based on the ship inventory
				else {

					//check to see if the ship has a phasor section.  If not, it can't use phasor attacks
					if (mouseMananger.selectedUnit.GetComponentInChildren<PhasorSection> () == null) {

						usePhasorRadarShotToggle.isOn = false;
						usePhasorRadarShotToggle.interactable = false;

						phasorRadarShotCountToggle.isOn = false;
						phasorRadarShotCountToggle.interactable = false;

						phasorRadarArrayToggle.isOn = false;
						phasorRadarArrayToggle.interactable = false;

						phasorXRayKernelToggle.isOn = false;
						phasorXRayKernelToggle.interactable = false;

						phasorTargetingDropdown.interactable = false;

						phasorFireButton.interactable = false;

					}

					//the else condition is that there is a phasor section in the selected unit
					else {

						//now we need to make sure the phasor section hasn't been destroyed
						if (mouseMananger.selectedUnit.GetComponentInChildren<PhasorSection> ().isDestroyed == true) {

							//if it is destroyed, we can't use phasor commands
							usePhasorRadarShotToggle.isOn = false;
							usePhasorRadarShotToggle.interactable = false;

							phasorRadarShotCountToggle.isOn = false;
							phasorRadarShotCountToggle.interactable = false;

							phasorRadarArrayToggle.isOn = false;
							phasorRadarArrayToggle.interactable = false;

							phasorXRayKernelToggle.isOn = false;
							phasorXRayKernelToggle.interactable = false;

							phasorTargetingDropdown.interactable = false;

							phasorFireButton.interactable = false;

						}

						//the else condition is that the phasor section is not destroyed.  
						else {

							//set the counts based on ship inventory
							phasorRadarShotCountToggle.GetComponentInChildren<TextMeshProUGUI>().text = mouseMananger.selectedUnit.GetComponentInChildren<PhasorSection> ().phasorRadarShot.ToString();

							//if the ship has the X-ray kernel upgrade drive, set that toggle to on
							if (mouseMananger.selectedUnit.GetComponentInChildren<PhasorSection> ().xRayKernalUpgrade == true) {

								phasorXRayKernelToggle.isOn = true;
								phasorXRayKernelToggle.interactable = false;

							}

							//if the ship has the radar array upgrade, set that toggle to on and disable radar shot single use items
							if (mouseMananger.selectedUnit.GetComponentInChildren<PhasorSection> ().phasorRadarArray == true) {

								phasorRadarArrayToggle.isOn = true;
								phasorRadarArrayToggle.interactable = false;

								usePhasorRadarShotToggle.isOn = false;
								usePhasorRadarShotToggle.interactable = false;

								phasorRadarShotCountToggle.isOn = false;
								phasorRadarShotCountToggle.interactable = false;

							}

							//now we need to check to see if we've already used phasors this turn
							if (mouseMananger.selectedUnit.GetComponentInChildren<PhasorSection> ().usedPhasorsThisTurn == true) {

								//if we've already used phasors, we can't use a radar shot or use the fire button
								usePhasorRadarShotToggle.isOn = false;
								usePhasorRadarShotToggle.interactable = false;

								phasorRadarShotCountToggle.isOn = false;
								phasorRadarShotCountToggle.interactable = false;

								phasorTargetingDropdown.interactable = false;

								phasorFireButton.interactable = false;

							}
							//check if we've used torpedos and don't have an additional battle crew - we only get 1 attack per turn without the crew
							else if (mouseMananger.selectedUnit.GetComponent<TorpedoSection> () == true &&
								mouseMananger.selectedUnit.GetComponent<TorpedoSection> ().usedTorpedosThisTurn == true &&

								(mouseMananger.selectedUnit.GetComponent<CrewSection> () == false ||

								(mouseMananger.selectedUnit.GetComponent<CrewSection> () == true &&
										mouseMananger.selectedUnit.GetComponent<CrewSection> ().battleCrew == false))) {

								//if we've already used torpedos and don't have an additional battle crew, we can't use a radar shot or use the fire button
								usePhasorRadarShotToggle.isOn = false;
								usePhasorRadarShotToggle.interactable = false;

								phasorRadarShotCountToggle.isOn = false;
								phasorRadarShotCountToggle.interactable = false;

								phasorTargetingDropdown.interactable = false;

								phasorFireButton.interactable = false;

							}
							//the else condition is that we have not used phasors yet
							else {

								//we can now set the phasor radar shot toggle based on inventory
								if (mouseMananger.selectedUnit.GetComponentInChildren<PhasorSection> ().phasorRadarShot <= 0) {

									//if radar shot == 0, we have none and cannot use it
									usePhasorRadarShotToggle.isOn = false;
									usePhasorRadarShotToggle.interactable = false;

									phasorRadarShotCountToggle.isOn = false;
									phasorRadarShotCountToggle.interactable = false;

								}
								//the else condition is that the radar shot quantity is non-zero
								else {

									//check if we have a radar array
									if (mouseMananger.selectedUnit.GetComponentInChildren<PhasorSection> ().phasorRadarArray == true) {

										//if we have the array, the shot can't be used
										usePhasorRadarShotToggle.interactable = false;
										phasorRadarShotCountToggle.interactable = false;

									} else {

										usePhasorRadarShotToggle.interactable = true;
										phasorRadarShotCountToggle.interactable = true;

									}

								}

								//now we need to check to see if we have a unit targeted
								if (mouseMananger.targetedUnit == null) {

									//if there is no targeted unit, we can't fire the phasors
									phasorTargetingDropdown.interactable = false;

									phasorFireButton.interactable = false;

								}

								//the else condition is that we do have a unit targeted
								else {

									//check if the targeted unit has a cloaking device turned on
									if (mouseMananger.targetedUnit.GetComponent<CloakingDevice> () && mouseMananger.targetedUnit.GetComponent<CloakingDevice> ().isCloaked == true) {

										//if it is cloaked, we can't allow using targeting
										usePhasorRadarShotToggle.interactable = false;

										phasorRadarShotCountToggle.interactable = false;

										//check if we have radar shot enabled - if we do, we need to turn it off
										if (usePhasorRadarShotToggle.isOn == true) {

											usePhasorRadarShotToggle.isOn = false;
											phasorTargetingDropdown.interactable = false;

										}
										//the else condition is that we do not have a targeting device enabled
										else {

											phasorTargetingDropdown.interactable = false;

										}

										//if the targeted unit is cloaked, we can allow the fire phasors button to be pressed 
										//as long as we are not cloaked
										if (mouseMananger.selectedUnit.GetComponent<CloakingDevice> () == true &&
										    mouseMananger.selectedUnit.GetComponent<CloakingDevice> ().isCloaked == true) {

											//we are cloaked and cannot fire
											phasorFireButton.interactable = false;

										} else {
											
											phasorFireButton.interactable = true;

										}

									}
									//the else condition is that the targeted unit is not cloaked
									else {

										//check if the targeted unit is a ship.  if it is a base, we have to handle slightly differently
										if (mouseMananger.targetedUnit.GetComponent<Ship> () == true) {

											//check if the targeted unit has a storage section
											if (mouseMananger.targetedUnit.GetComponent<StorageSection> () == true) {

												//now check if the targeted ship has radar jamming
												if (mouseMananger.targetedUnit.GetComponent<StorageSection> ().radarJammingSystem == true) {

													//if it does, we can't allow using targeting
													usePhasorRadarShotToggle.interactable = false;

													phasorRadarShotCountToggle.interactable = false;

													//check if we have radar shot enabled - if we do, we need to turn it off
													if (usePhasorRadarShotToggle.isOn == true) {

														usePhasorRadarShotToggle.isOn = false;
														phasorTargetingDropdown.interactable = false;

													}
													//the else condition is that we do not have a targeting device enabled
													else {

														phasorTargetingDropdown.interactable = false;

													}

													//if the targeted ship has jamming, we can allow the fire phasors button to be pressed
													//as long as we are not cloaked
													if (mouseMananger.selectedUnit.GetComponent<CloakingDevice> () == true &&
														mouseMananger.selectedUnit.GetComponent<CloakingDevice> ().isCloaked == true) {

														//we are cloaked and cannot fire
														phasorFireButton.interactable = false;

													} else {

														phasorFireButton.interactable = true;

													}

												}
												//the else condition is that there is no jamming on the targeted unit
												else {

													//we can now set the phasor radar shot toggle based on inventory
													if (mouseMananger.selectedUnit.GetComponentInChildren<PhasorSection> ().phasorRadarShot <= 0) {

														//if radar shot == 0, we have none and cannot use it
														usePhasorRadarShotToggle.isOn = false;
														usePhasorRadarShotToggle.interactable = false;

														phasorRadarShotCountToggle.isOn = false;
														phasorRadarShotCountToggle.interactable = false;

													}
													//the else condition is that the radar shot quantity is non-zero
													else {
														
														//check if we have a radar array
														if (mouseMananger.selectedUnit.GetComponentInChildren<PhasorSection> ().phasorRadarArray == true) {

															//if we have the array, the shot can't be used
															usePhasorRadarShotToggle.interactable = false;
															phasorRadarShotCountToggle.interactable = false;

														} else {

															usePhasorRadarShotToggle.interactable = true;
															phasorRadarShotCountToggle.interactable = true;

														}

													}

													//check if we have targeting device enabled
													if (phasorRadarArrayToggle.isOn == true || usePhasorRadarShotToggle.isOn == true) {

														phasorTargetingDropdown.interactable = true;

														//if we have targeting enabled, we can't allow the fire button to be pressed until a section has been chosen
														if (phasorTargetingDropdown.GetComponentInChildren<TextMeshProUGUI> ().text == "Choose Section") {

															phasorFireButton.interactable = false;

														}
														//the else condition is that a section has been chosen, so we can use the fire button
														//as long as we are not cloaked
														else {

															if (mouseMananger.selectedUnit.GetComponent<CloakingDevice> () == true &&
																mouseMananger.selectedUnit.GetComponent<CloakingDevice> ().isCloaked == true) {

																//we are cloaked and cannot fire
																phasorFireButton.interactable = false;

															} else {

																phasorFireButton.interactable = true;

															}

														}


													}
													//the else condition is that we do not have a targeting device enabled
													else {

														phasorTargetingDropdown.interactable = false;

														//check if we are cloaked

														if (mouseMananger.selectedUnit.GetComponent<CloakingDevice> () == true &&
															mouseMananger.selectedUnit.GetComponent<CloakingDevice> ().isCloaked == true) {

															//we are cloaked and cannot fire
															phasorFireButton.interactable = false;

														} else {

															phasorFireButton.interactable = true;

														}

													}

												}

											}
											//the else condition is that the targeted ship does not have a storage section
											else {

												//we can now set the phasor radar shot toggle based on inventory
												if (mouseMananger.selectedUnit.GetComponentInChildren<PhasorSection> ().phasorRadarShot <= 0) {

													//if radar shot == 0, we have none and cannot use it
													usePhasorRadarShotToggle.isOn = false;
													usePhasorRadarShotToggle.interactable = false;

													phasorRadarShotCountToggle.isOn = false;
													phasorRadarShotCountToggle.interactable = false;

												}
												//the else condition is that the radar shot quantity is non-zero
												else {
													
													//check if we have a radar array
													if (mouseMananger.selectedUnit.GetComponentInChildren<PhasorSection> ().phasorRadarArray == true) {

														//if we have the array, the shot can't be used
														usePhasorRadarShotToggle.interactable = false;
														phasorRadarShotCountToggle.interactable = false;

													} else {

														usePhasorRadarShotToggle.interactable = true;
														phasorRadarShotCountToggle.interactable = true;

													}

												}

												//check if we have targeting device enabled
												if (phasorRadarArrayToggle.isOn == true || usePhasorRadarShotToggle.isOn == true) {

													phasorTargetingDropdown.interactable = true;

													//if we have targeting enabled, we can't allow the fire button to be pressed until a section has been chosen
													if (phasorTargetingDropdown.GetComponentInChildren<TextMeshProUGUI> ().text == "Choose Section") {

														phasorFireButton.interactable = false;

													}
													//the else condition is that a section has been chosen, so we can use the fire button
													//as long as we are not cloaked
													else {

														if (mouseMananger.selectedUnit.GetComponent<CloakingDevice> () == true &&
															mouseMananger.selectedUnit.GetComponent<CloakingDevice> ().isCloaked == true) {

															//we are cloaked and cannot fire
															phasorFireButton.interactable = false;

														} else {

															phasorFireButton.interactable = true;

														}

													}
											
												}
												//the else condition is that we do not have a targeting device enabled
												else {

													phasorTargetingDropdown.interactable = false;

													//check if we are cloaked

													if (mouseMananger.selectedUnit.GetComponent<CloakingDevice> () == true &&
														mouseMananger.selectedUnit.GetComponent<CloakingDevice> ().isCloaked == true) {

														//we are cloaked and cannot fire
														phasorFireButton.interactable = false;

													} else {

														phasorFireButton.interactable = true;

													}

												}

											}

										}
										//the else condition is that the targeted unit is a base
										else if (mouseMananger.targetedUnit.GetComponent<Starbase> () == true) {

											//now check if the targeted ship has radar jamming
											if (mouseMananger.targetedUnit.GetComponent<StarbaseStorageSection1> ().radarJammingSystem == true) {

												//if it does, we can't allow using targeting
												usePhasorRadarShotToggle.interactable = false;

												phasorRadarShotCountToggle.interactable = false;

												//check if we have radar shot enabled - if we do, we need to turn it off
												if (usePhasorRadarShotToggle.isOn == true) {

													usePhasorRadarShotToggle.isOn = false;
													phasorTargetingDropdown.interactable = false;

												}
												//the else condition is that we do not have a targeting device enabled
												else {

													phasorTargetingDropdown.interactable = false;

												}

												//if the targeted ship has jamming, we can allow the fire phasors button to be pressed
												//as long as we are not cloaked
												if (mouseMananger.selectedUnit.GetComponent<CloakingDevice> () == true &&
													mouseMananger.selectedUnit.GetComponent<CloakingDevice> ().isCloaked == true) {

													//we are cloaked and cannot fire
													phasorFireButton.interactable = false;

												} else {

													phasorFireButton.interactable = true;

												}

											}
											//the else condition is that there is no jamming on the targeted unit
											else {

												//we can now set the phasor radar shot toggle based on inventory
												if (mouseMananger.selectedUnit.GetComponentInChildren<PhasorSection> ().phasorRadarShot <= 0) {

													//if radar shot == 0, we have none and cannot use it
													usePhasorRadarShotToggle.isOn = false;
													usePhasorRadarShotToggle.interactable = false;

													phasorRadarShotCountToggle.isOn = false;
													phasorRadarShotCountToggle.interactable = false;

												}
												//the else condition is that the radar shot quantity is non-zero
												else {
													
													//check if we have a radar array
													if (mouseMananger.selectedUnit.GetComponentInChildren<PhasorSection> ().phasorRadarArray == true) {

														//if we have the array, the shot can't be used
														usePhasorRadarShotToggle.interactable = false;
														phasorRadarShotCountToggle.interactable = false;

													} else {

														usePhasorRadarShotToggle.interactable = true;
														phasorRadarShotCountToggle.interactable = true;

													}

												}

												//check if we have targeting device enabled
												if (phasorRadarArrayToggle.isOn == true || usePhasorRadarShotToggle.isOn == true) {

													phasorTargetingDropdown.interactable = true;

													//if we have targeting enabled, we can't allow the fire button to be pressed until a section has been chosen
													if (phasorTargetingDropdown.GetComponentInChildren<TextMeshProUGUI> ().text == "Choose Section") {

														phasorFireButton.interactable = false;

													}
													//the else condition is that a section has been chosen, so we can use the fire button
													//as long as we are not cloaked
													else {

														if (mouseMananger.selectedUnit.GetComponent<CloakingDevice> () == true &&
															mouseMananger.selectedUnit.GetComponent<CloakingDevice> ().isCloaked == true) {

															//we are cloaked and cannot fire
															phasorFireButton.interactable = false;

														} else {

															phasorFireButton.interactable = true;

														}

													}


												}
												//the else condition is that we do not have a targeting device enabled
												else {

													phasorTargetingDropdown.interactable = false;

													//check if we are cloaked
													if (mouseMananger.selectedUnit.GetComponent<CloakingDevice> () == true &&
														mouseMananger.selectedUnit.GetComponent<CloakingDevice> ().isCloaked == true) {

														//we are cloaked and cannot fire
														phasorFireButton.interactable = false;

													} else {

														phasorFireButton.interactable = true;

													}

												}

											} //end else there is no jamming

										}  //end else if the targeted unit is a base

									}  //end else if the target is uncloaked

								} //end else we do have a unit targeted

							} //end else we have not used phasors yet

						} //end else the phasor section is not destroyed

					}  //end else the selected unit has a phasor section

				} //end else the selected unit is a ship

			} //end else there is a selected unit

			SetTargetingText ();
			SetTargetingSectionOptions ();
			phasorTargetingDropdown.RefreshShownValue ();
			SetFireButtonText ();


		} //end if the phasor attack toggle has been turned on

		//this is if the phasor attack has been turned off
		else if (phasorToggle.isOn == false) {

			//if we have don't have a selected unit, we can turn the toggles off and make them not interactable
			if (mouseMananger.selectedUnit == null) {

				usePhasorRadarShotToggle.isOn = false;
				usePhasorRadarShotToggle.interactable = false;

				phasorRadarShotCountToggle.isOn = false;
				phasorRadarShotCountToggle.interactable = false;

				phasorRadarArrayToggle.isOn = false;
				phasorRadarArrayToggle.interactable = false;

				phasorXRayKernelToggle.isOn = false;
				phasorXRayKernelToggle.interactable = false;

				phasorTargetingDropdown.interactable = false;

				phasorFireButton.interactable = false;

				SetTargetingText ();
				SetFireButtonText ();


			}

			//now we can check if we have a targeted unit
			if (mouseMananger.targetedUnit == null) {

				//if there is no targeted unit anymore, we can set the toggles off and make them not interactable
				usePhasorRadarShotToggle.isOn = false;
				usePhasorRadarShotToggle.interactable = false;

				phasorRadarShotCountToggle.isOn = false;
				phasorRadarShotCountToggle.interactable = false;

				phasorRadarArrayToggle.isOn = false;
				phasorRadarArrayToggle.interactable = false;

				phasorXRayKernelToggle.isOn = false;
				phasorXRayKernelToggle.interactable = false;

				phasorTargetingDropdown.interactable = false;

				phasorFireButton.interactable = false;

				SetTargetingText ();
				SetFireButtonText ();

			}

		}

	}

	//this function sets the helper text for the phasor targeting
	private void SetTargetingText(){

		//check if there is a targeted unit
		if (mouseMananger.targetedUnit != null) {

			//check if targeted unit is cloaked
			if (mouseMananger.targetedUnit.GetComponent<CloakingDevice> () == true && mouseMananger.targetedUnit.GetComponent<CloakingDevice> ().isCloaked == true) {

				phasorTargetingText.text = ("Target Is Cloaked!");

				//update the font size if necessary
				UIManager.AutoSizeTextMeshFont(phasorTargetingText);

			}
			//the else condition is that the targeted unit is not cloaked
			else{
				
				//check if the targeted unit is a ship
				if (mouseMananger.targetedUnit.GetComponent<Ship> () == true) {

					//check if the targeted ship has a storage section
					if (mouseMananger.targetedUnit.GetComponent<StorageSection> () == true) {

						//check if the targeted ship has radar jamming
						if (mouseMananger.targetedUnit.GetComponent<StorageSection> ().radarJammingSystem == true) {

							phasorTargetingText.text = ("Target Has Jamming!");

							//update the font size if necessary
							UIManager.AutoSizeTextMeshFont(phasorTargetingText);

						}
						//the else condition is that the targeted unit does not have jamming
						else {

							//check if we have targeting device enabled
							//note that there must be a selected unit in order for there to be a targeted unit
							if (phasorRadarArrayToggle.isOn == true || usePhasorRadarShotToggle.isOn == true) {

								//Debug.Log ("Choose Section 1");

								phasorTargetingText.text = ("Choose Section");

								//update the font size if necessary
								UIManager.AutoSizeTextMeshFont(phasorTargetingText);

							}
							//the else condition is that we do not have a targeting device enabled
							else {

								phasorTargetingText.text = ("No Targeting Active");

								//update the font size if necessary
								UIManager.AutoSizeTextMeshFont(phasorTargetingText);

							}

						}

					}
					//the else condition is that the targeted ship has no storage section, which means it can't have jamming
					else {

						//check if we have targeting device enabled
						//note that there must be a selected unit in order for there to be a targeted unit
						if (phasorRadarArrayToggle.isOn == true || usePhasorRadarShotToggle.isOn == true) {

							//Debug.Log ("Choose Section 2");


							phasorTargetingText.text = ("Choose Section");

							//update the font size if necessary
							UIManager.AutoSizeTextMeshFont(phasorTargetingText);

						}
						//the else condition is that we do not have a targeting device enabled
						else {

							phasorTargetingText.text = ("No Targeting Active");

							//update the font size if necessary
							UIManager.AutoSizeTextMeshFont(phasorTargetingText);

						}

					}

				}
				//the else condition is that the targeted unit is not a ship
				else if (mouseMananger.targetedUnit.GetComponent<Starbase> () == true) {

					//check if the targeted ship has radar jamming
					if (mouseMananger.targetedUnit.GetComponent<StarbaseStorageSection1> ().radarJammingSystem == true) {

						phasorTargetingText.text = ("Target Has Jamming!");

						//update the font size if necessary
						UIManager.AutoSizeTextMeshFont(phasorTargetingText);

					}
					//the else condition is that the targeted unit does not have jamming
					else {

						//check if we have targeting device enabled
						//note that there must be a selected unit in order for there to be a targeted unit
						if (phasorRadarArrayToggle.isOn == true || usePhasorRadarShotToggle.isOn == true) {

							//Debug.Log ("Choose Section 3");


							phasorTargetingText.text = ("Choose Section");

							//update the font size if necessary
							UIManager.AutoSizeTextMeshFont(phasorTargetingText);

						}
						//the else condition is that we do not have a targeting device enabled
						else {

							phasorTargetingText.text = ("No Targeting Active");

							//update the font size if necessary
							UIManager.AutoSizeTextMeshFont(phasorTargetingText);

						}

					}

				}  //end else the targeted unit is a starbase
			} //end else the targeted unit is not cloaked
		} //end if there is a targeted unit
		//the else condition is that there is no targeted unit
		else {

			//check whether we have a selected unit
			if (mouseMananger.selectedUnit != null) {

				phasorTargetingText.text = ("No Unit Targeted");

				//update the font size if necessary
				UIManager.AutoSizeTextMeshFont(phasorTargetingText);

			}
			//the else condition is that there is no selected unit
			else {

				phasorTargetingText.text = ("No Unit Selected");

				//update the font size if necessary
				UIManager.AutoSizeTextMeshFont(phasorTargetingText);

			}
				
		}

	}

	//this function will set the available options for the targeting dropdown menu
	private void SetTargetingSectionOptions(){

		//start by clearing the existing dropdown options
		phasorTargetingDropdown.ClearOptions();

		//create a list of new dropdown options to populate the choices
		List<TMP_Dropdown.OptionData> dropDownOptions = new List<TMP_Dropdown.OptionData> ();

		//add a dummy option at the top of the list
		dropDownOptions.Add(new TMP_Dropdown.OptionData("Choose Section"));

		//check if there is a targeted unit
		if (mouseMananger.targetedUnit != null) {

			//check if we have a targeting device enabled
			if (phasorRadarArrayToggle.isOn == true || usePhasorRadarShotToggle.isOn == true) {

				//check if the targeted unit is a ship
				if (mouseMananger.targetedUnit.GetComponent<Ship> () == true) {

					//check if the targeted ship has an engine section
					if (mouseMananger.targetedUnit.GetComponent<EngineSection> () == true) {

						//check if the engine section is not destroyed
						if (mouseMananger.targetedUnit.GetComponent<EngineSection> ().isDestroyed == false) {

							//add a dropdown option for the engine section
							dropDownOptions.Add (new TMP_Dropdown.OptionData ("Engine Section"));

						}

					}

					//check if the targeted ship has a phasor section
					if (mouseMananger.targetedUnit.GetComponent<PhasorSection> () == true) {

						//check if the phasor section is not destroyed
						if (mouseMananger.targetedUnit.GetComponent<PhasorSection> ().isDestroyed == false) {

							//add a dropdown option for the phasor section
							dropDownOptions.Add (new TMP_Dropdown.OptionData ("Phasor Section"));

						}
					
					}

					//check if the targeted ship has a torpedo section
					if (mouseMananger.targetedUnit.GetComponent<TorpedoSection> () == true) {

						//check if the torpedo section is not destroyed
						if (mouseMananger.targetedUnit.GetComponent<TorpedoSection> ().isDestroyed == false) {

							//add a dropdown option for the torpedo section
							dropDownOptions.Add (new TMP_Dropdown.OptionData ("Torpedo Section"));

						}

					}

					//check if the targeted ship has a crew section
					if (mouseMananger.targetedUnit.GetComponent<CrewSection> () == true) {

						//check if the crew section is not destroyed
						if (mouseMananger.targetedUnit.GetComponent<CrewSection> ().isDestroyed == false) {

							//add a dropdown option for the crew section
							dropDownOptions.Add (new TMP_Dropdown.OptionData ("Crew Section"));

						}

					}

					//check if the targeted ship has a storage section
					if (mouseMananger.targetedUnit.GetComponent<StorageSection> () == true) {

						//check if the storage section is not destroyed
						if (mouseMananger.targetedUnit.GetComponent<StorageSection> ().isDestroyed == false) {

							//add a dropdown option for the storage section
							dropDownOptions.Add (new TMP_Dropdown.OptionData ("Storage Section"));

						}

					}

				}
				//the else condition is that the targeted unit is not a ship, so it's a base
				else if (mouseMananger.targetedUnit.GetComponent<Starbase> () == true){

					//check if the phasor 1 section is not destroyed
					if (mouseMananger.targetedUnit.GetComponent<StarbasePhasorSection1> ().isDestroyed == false) {

						//add a dropdown option for the phasor section 1
						dropDownOptions.Add (new TMP_Dropdown.OptionData ("Phasor Section 1"));

					}

					//check if the phasor 2 section is not destroyed
					if (mouseMananger.targetedUnit.GetComponent<StarbasePhasorSection2> ().isDestroyed == false) {

						//add a dropdown option for the phasor section 1
						dropDownOptions.Add (new TMP_Dropdown.OptionData ("Phasor Section 2"));

					}

					//check if the torpedo section is not destroyed
					if (mouseMananger.targetedUnit.GetComponent<StarbaseTorpedoSection> ().isDestroyed == false) {

						//add a dropdown option for the phasor section 1
						dropDownOptions.Add (new TMP_Dropdown.OptionData ("Torpedo Section"));

					}


					//check if the crew is not destroyed
					if (mouseMananger.targetedUnit.GetComponent<StarbaseCrewSection> ().isDestroyed == false) {

						//add a dropdown option for the phasor section 1
						dropDownOptions.Add (new TMP_Dropdown.OptionData ("Crew Section"));

					}

					//check if the storage section 1 is not destroyed
					if (mouseMananger.targetedUnit.GetComponent<StarbaseStorageSection1> ().isDestroyed == false) {

						//add a dropdown option for the phasor section 1
						dropDownOptions.Add (new TMP_Dropdown.OptionData ("Storage Section 1"));

					}

					//check if the storage section 2 is not destroyed
					if (mouseMananger.targetedUnit.GetComponent<StarbaseStorageSection2> ().isDestroyed == false) {

						//add a dropdown option for the phasor section 1
						dropDownOptions.Add (new TMP_Dropdown.OptionData ("Storage Section 2"));

					}

				}

			}
			//the else condition is that we do not have a targeting device enabled

		}
			
		//add the options to the dropdown
		phasorTargetingDropdown.AddOptions (dropDownOptions);

		//update the font size if necessary
		UIManager.AutoSizeTextMeshFont(phasorTargetingDropdown.GetComponentInChildren<TextMeshProUGUI> ());

	}

	//this function will set the fire button text
	private void SetFireButtonText(){

		//check if we have a selected unit
		if (mouseMananger.selectedUnit != null) {

			//check if the selected unit has a valid phasor attack remaining
			if (mouseMananger.selectedUnit.GetComponent<CombatUnit> ().hasRemainingPhasorAttack == true) {

				//check if the selected unit is a bird of prey and is cloaked
				if (mouseMananger.selectedUnit.GetComponent<CloakingDevice> () == true) {

					if (mouseMananger.selectedUnit.GetComponent<CloakingDevice> ().isCloaked == true) {

						//if the attack is still remaining, set the text to Cloaked
						phasorFireButton.GetComponentInChildren<TextMeshProUGUI> ().text = ("Cloaked");

						//update the font size if necessary
						UIManager.AutoSizeTextMeshFont (phasorFireButton.GetComponentInChildren<TextMeshProUGUI> ());

					} else {

						//if the attack is still remaining, set the text to Fire Phasors
						phasorFireButton.GetComponentInChildren<TextMeshProUGUI> ().text = ("Fire Phasors");

						//update the font size if necessary
						UIManager.AutoSizeTextMeshFont (phasorFireButton.GetComponentInChildren<TextMeshProUGUI> ());

					}

				} else {

					//if the attack is still remaining, set the text to Fire Phasors
					phasorFireButton.GetComponentInChildren<TextMeshProUGUI> ().text = ("Fire Phasors");

					//update the font size if necessary
					UIManager.AutoSizeTextMeshFont (phasorFireButton.GetComponentInChildren<TextMeshProUGUI> ());

				}

			}
			//else, we want to check if the attack is not remaining because we already fired phasors
			//this is a slightly different check for ships and bases
			else if ((mouseMananger.selectedUnit.GetComponent<PhasorSection> () == true &&
			         mouseMananger.selectedUnit.GetComponent<PhasorSection> ().usedPhasorsThisTurn == true) ||

			         (mouseMananger.selectedUnit.GetComponent<StarbasePhasorSection1> () == true &&
			         (mouseMananger.selectedUnit.GetComponent<StarbasePhasorSection1> ().usedPhasorsThisTurn == true ||
			         mouseMananger.selectedUnit.GetComponent<StarbasePhasorSection2> ().usedPhasorsThisTurn == true))) {

				//if we already fired phasors, set the text to already fired
				phasorFireButton.GetComponentInChildren<TextMeshProUGUI> ().text = ("Already Fired");

				//update the font size if necessary
				UIManager.AutoSizeTextMeshFont(phasorFireButton.GetComponentInChildren<TextMeshProUGUI> ());

			}
			//the else condition is that the attack is not remaining and we did not fire phasors, which means we must have fired a torpedo
			//and don't have double-attack capability
			else {

				phasorFireButton.GetComponentInChildren<TextMeshProUGUI> ().text = ("Already Attacked");

				//update the font size if necessary
				UIManager.AutoSizeTextMeshFont(phasorFireButton.GetComponentInChildren<TextMeshProUGUI> ());

			}

		} else {

			phasorFireButton.GetComponentInChildren<TextMeshProUGUI> ().text = ("Fire Phasors");

			//update the font size if necessary
			UIManager.AutoSizeTextMeshFont(phasorFireButton.GetComponentInChildren<TextMeshProUGUI> ());

		}

	}


	//this function handles firing the phasors
	private void FirePhasors(){

		//Debug.Log ("Fire Phasors!");

		//invoke the OnFirePhasors event, with the targeted unit passed to the event
		OnFirePhasors.Invoke(mouseMananger.selectedUnit.GetComponent<CombatUnit>(),
			mouseMananger.targetedUnit.GetComponent<CombatUnit>(),
			phasorTargetingDropdown.GetComponentInChildren<TextMeshProUGUI>().text);

		SetPhasorMenuToggles ();

	}

	//this function handles OnDestroy
	private void OnDestroy(){

		RemoveAllListeners();

	}

	//this function removes all listeners
	private void RemoveAllListeners(){

		if (phasorToggle != null) {
			
			//remove an on-click event listener for the main menu phasor toggle
			phasorToggle.onValueChanged.RemoveListener (boolSetPhasorMenuToggleAction);

		}

		if (usePhasorRadarShotToggle != null) {
			
			//remove an on-click event listener for the phasor radar shot toggle
			usePhasorRadarShotToggle.onValueChanged.RemoveListener (boolSetPhasorMenuToggleDropdownZeroAction);

		}

		if (phasorTargetingDropdown != null) {
			
			//remove an event listener for the dropdown value changing
			phasorTargetingDropdown.onValueChanged.RemoveListener (intSetPhasorMenuToggleAction);

		}

		if (phasorFireButton != null) {
			
			//remove an on-click event listener for the fire phasors toggle
			phasorFireButton.onClick.RemoveListener (FirePhasors);

		}

		if (mouseMananger != null) {
			
			//remove an event listener for when a selectedUnit is set and cleared
			mouseMananger.OnSetSelectedUnit.RemoveListener (SetPhasorMenuToggles);
			mouseMananger.OnClearSelectedUnit.RemoveListener (SetPhasorMenuToggles);

			//remove an event listener for when a targetedUnit is set and cleared
			mouseMananger.OnSetTargetedUnit.RemoveListener (SetPhasorMenuToggles);
			mouseMananger.OnClearTargetedUnit.RemoveListener (SetPhasorMenuToggles);

		}

		//remove a listener for when a purchase is complete
		PhasorSection.OnInventoryUpdated.RemoveListener (combatUnitSetPhasorMenuToggleAction);

	}

}
