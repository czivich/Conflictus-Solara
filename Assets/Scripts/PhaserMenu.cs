using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class PhaserMenu : MonoBehaviour {

	//we will need the mouseManager to check the selected unit
	private MouseManager mouseMananger;

	//variable for phaser trigger toggle
	public Toggle phaserToggle;

	//variable for phaser Menu phaser radar shot toggle
	public Toggle usePhaserRadarShotToggle;

	//variable for the system upgrages
	public Toggle phaserRadarArrayToggle;
	public Toggle phaserXRayKernelToggle;

	//variable to hold the fire button
	public Button phaserFireButton;

	//variables for the item counts
	public Toggle phaserRadarShotCountToggle;

	//variable for the targeting dropdown
	public TMP_Dropdown phaserTargetingDropdown;

	//variable for the targeting helper text
	public TextMeshProUGUI phaserTargetingText;


	//these events are for when the phasers are fired are clicked
	public PhaserEvent OnFirePhasers = new PhaserEvent();

	//simple class derived from unityEvent to pass combat units
	public class PhaserEvent : UnityEvent<CombatUnit, CombatUnit, string>{};

	//unityActions
	private UnityAction<bool> boolSetPhaserMenuToggleAction;
	private UnityAction<bool> boolSetPhaserMenuToggleDropdownZeroAction;
	private UnityAction<int> intSetPhaserMenuToggleAction;
	private UnityAction<CombatUnit> combatUnitSetPhaserMenuToggleAction;


	// Use this for initialization
	public void Init () {

		//get the mouseManager
		mouseMananger = GameObject.FindGameObjectWithTag("MouseManager").GetComponent<MouseManager>();

		//set the actions
		boolSetPhaserMenuToggleAction = (value) => {SetPhaserMenuToggles();};
		boolSetPhaserMenuToggleDropdownZeroAction = (value) => {
			SetPhaserMenuToggles();
			phaserTargetingDropdown.value = 0;		//I want the menu to go back to "Choose Section" when we toggle targeting
		};

		intSetPhaserMenuToggleAction = (value) => {SetPhaserMenuToggles();};
		combatUnitSetPhaserMenuToggleAction = (combatUnit) => {SetPhaserMenuToggles();};


		//add an on-click event listener for the main menu phaser toggle
		phaserToggle.onValueChanged.AddListener(boolSetPhaserMenuToggleAction);

		//add an on-click event listener for the phaser radar shot toggle
		usePhaserRadarShotToggle.onValueChanged.AddListener(boolSetPhaserMenuToggleDropdownZeroAction);

		//add an event listener for the dropdown value changing
		phaserTargetingDropdown.onValueChanged.AddListener(intSetPhaserMenuToggleAction);

		//add an on-click event listener for the fire phasers toggle
		phaserFireButton.onClick.AddListener(FirePhasers);

		//add an event listener for when a selectedUnit is set and cleared
		mouseMananger.OnSetSelectedUnit.AddListener(SetPhaserMenuToggles);
		mouseMananger.OnClearSelectedUnit.AddListener(SetPhaserMenuToggles);

		//add an event listener for when a targetedUnit is set and cleared
		mouseMananger.OnSetTargetedUnit.AddListener(SetPhaserMenuToggles);
		mouseMananger.OnClearTargetedUnit.AddListener(SetPhaserMenuToggles);

		//add a listener for when a purchase is complete
		PhaserSection.OnInventoryUpdated.AddListener(combatUnitSetPhaserMenuToggleAction);
		
	}

	//this function sets the interactability of the phaser menu toggles
	private void SetPhaserMenuToggles(){

		//this is if the Phaser Attack menu Toggle has been turned on
		if (phaserToggle.isOn == true) {

			//if there is no unit selected, set toggles to not interactable and turn them off
			if (mouseMananger.selectedUnit == null) {

				usePhaserRadarShotToggle.isOn = false;
				usePhaserRadarShotToggle.interactable = false;

				phaserRadarShotCountToggle.isOn = false;
				phaserRadarShotCountToggle.interactable = false;

				phaserRadarArrayToggle.isOn = false;
				phaserRadarArrayToggle.interactable = false;

				phaserXRayKernelToggle.isOn = false;
				phaserXRayKernelToggle.interactable = false;



				phaserTargetingDropdown.interactable = false;

				phaserFireButton.interactable = false;

			}

			//the else condition means that there is a selected unit
			else {

				//now we need to make sure the selected unit is a ship
				//if it's a starbase, we need to handle with slightly diffferent logic
				if (mouseMananger.selectedUnit.GetComponent<Ship> () == null) {

					//if we are here, that means that the selected unit is a starbase
					//check to see if both phaser sections have been destroyed
					if (mouseMananger.selectedUnit.GetComponent<StarbasePhaserSection1> ().isDestroyed == true &&
					    mouseMananger.selectedUnit.GetComponent<StarbasePhaserSection2> ().isDestroyed == true) {

						//if both are destroyed, we can't use phaser commands
						usePhaserRadarShotToggle.isOn = false;
						usePhaserRadarShotToggle.interactable = false;

						phaserRadarShotCountToggle.isOn = false;
						phaserRadarShotCountToggle.interactable = false;

						phaserRadarArrayToggle.isOn = false;
						phaserRadarArrayToggle.interactable = false;

						phaserXRayKernelToggle.isOn = false;
						phaserXRayKernelToggle.interactable = false;

						phaserTargetingDropdown.interactable = false;

						phaserFireButton.interactable = false;

					}
					//the else condition is that at least one starbase phaser section is still alive
					else {

						//set the counts based on ship inventory
						phaserRadarShotCountToggle.GetComponentInChildren<TextMeshProUGUI>().text = mouseMananger.selectedUnit.GetComponentInChildren<StarbasePhaserSection1> ().phaserRadarShot.ToString();

						//if the base has the X-ray kernel upgrade, set that toggle to on
						if (mouseMananger.selectedUnit.GetComponentInChildren<StarbasePhaserSection2> ().xRayKernalUpgrade == true) {

							phaserXRayKernelToggle.isOn = true;
							phaserXRayKernelToggle.interactable = false;

						}

						//if the starbase has the radar array upgrade, set that toggle to on and disable radar shot single use items
						if (mouseMananger.selectedUnit.GetComponentInChildren<StarbasePhaserSection1> ().phaserRadarArray == true) {

							phaserRadarArrayToggle.isOn = true;
							phaserRadarArrayToggle.interactable = false;

							usePhaserRadarShotToggle.isOn = false;
							usePhaserRadarShotToggle.interactable = false;

							phaserRadarShotCountToggle.isOn = false;
							phaserRadarShotCountToggle.interactable = false;

						}

						//now we need to check to see if we've already used phasers this turn
						if (mouseMananger.selectedUnit.GetComponentInChildren<StarbasePhaserSection1> ().usedPhasersThisTurn == true ||
						    mouseMananger.selectedUnit.GetComponentInChildren<StarbasePhaserSection2> ().usedPhasersThisTurn == true) {

							//if we've already used phasers, we can't use a radar shot or use the fire button
							usePhaserRadarShotToggle.isOn = false;
							usePhaserRadarShotToggle.interactable = false;

							phaserRadarShotCountToggle.isOn = false;
							phaserRadarShotCountToggle.interactable = false;

							phaserTargetingDropdown.interactable = false;

							phaserFireButton.interactable = false;

						}
						//check if we've used torpedos and don't have an additional battle crew - we only get 1 attack per turn without the crew
						else if (mouseMananger.selectedUnit.GetComponent<StarbaseTorpedoSection> ().usedTorpedosThisTurn == true &&
						        mouseMananger.selectedUnit.GetComponent<StarbaseCrewSection> ().battleCrew == false) {

							//if we've already used torpedos and don't have an additional battle crew, we can't use a radar shot or use the fire button
							usePhaserRadarShotToggle.isOn = false;
							usePhaserRadarShotToggle.interactable = false;

							phaserRadarShotCountToggle.isOn = false;
							phaserRadarShotCountToggle.interactable = false;

							phaserTargetingDropdown.interactable = false;

							phaserFireButton.interactable = false;

						}
						//the else condition is that we have not used phasers yet and still have a valid attack
						else {

							//we can now set the phaser radar shot toggle based on inventory
							if (mouseMananger.selectedUnit.GetComponentInChildren<StarbasePhaserSection1> ().phaserRadarShot <= 0) {

								//if radar shot == 0, we have none and cannot use it
								usePhaserRadarShotToggle.isOn = false;
								usePhaserRadarShotToggle.interactable = false;

								phaserRadarShotCountToggle.isOn = false;
								phaserRadarShotCountToggle.interactable = false;

							}
							//the else condition is that the radar shot quantity is non-zero
							else {
								
								//check if we have a radar array
								if (mouseMananger.selectedUnit.GetComponentInChildren<StarbasePhaserSection1> ().phaserRadarArray == true) {

									//if we have the array, the shot can't be used
									usePhaserRadarShotToggle.interactable = false;
									phaserRadarShotCountToggle.interactable = false;

								} else {

									usePhaserRadarShotToggle.interactable = true;
									phaserRadarShotCountToggle.interactable = true;

								}

							}

							//now we need to check to see if we have a unit targeted
							if (mouseMananger.targetedUnit == null) {

								//if there is no targeted unit, we can't fire the phasers
								phaserTargetingDropdown.interactable = false;

								phaserFireButton.interactable = false;

							}

							//the else condition is that we do have a unit targeted
							else {

								//check if the targeted unit has a cloaking device turned on
								if (mouseMananger.targetedUnit.GetComponent<CloakingDevice> () && mouseMananger.targetedUnit.GetComponent<CloakingDevice> ().isCloaked == true) {

									//if it is cloaked, we can't allow using targeting
									usePhaserRadarShotToggle.interactable = false;

									phaserRadarShotCountToggle.interactable = false;

									//check if we have radar shot enabled - if we do, we need to turn it off
									if (usePhaserRadarShotToggle.isOn == true) {

										usePhaserRadarShotToggle.isOn = false;
										phaserTargetingDropdown.interactable = false;

									}
									//the else condition is that we do not have a targeting device enabled
									else {

										phaserTargetingDropdown.interactable = false;

									}

									//if the targeted unit is cloaked, we can allow the fire phasers button to be pressed
									phaserFireButton.interactable = true;

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
												usePhaserRadarShotToggle.interactable = false;

												phaserRadarShotCountToggle.interactable = false;

												//check if we have radar shot enabled - if we do, we need to turn it off
												if (usePhaserRadarShotToggle.isOn == true) {

													usePhaserRadarShotToggle.isOn = false;
													phaserTargetingDropdown.interactable = false;

												}
												//the else condition is that we do not have a targeting device enabled
												else {

													phaserTargetingDropdown.interactable = false;

												}

												//if the targeted ship has jamming, we can allow the fire phasers button to be pressed
												phaserFireButton.interactable = true;

											}
											//the else condition is that there is no jamming on the targeted unit
											else {

												//we can now set the phaser radar shot toggle based on inventory
												if (mouseMananger.selectedUnit.GetComponentInChildren<StarbasePhaserSection1> ().phaserRadarShot <= 0) {

													//if radar shot == 0, we have none and cannot use it
													usePhaserRadarShotToggle.isOn = false;
													usePhaserRadarShotToggle.interactable = false;

													phaserRadarShotCountToggle.isOn = false;
													phaserRadarShotCountToggle.interactable = false;

												}
												//the else condition is that the radar shot quantity is non-zero
												else {

													//check if we have a radar array
													if (mouseMananger.selectedUnit.GetComponentInChildren<StarbasePhaserSection1> ().phaserRadarArray == true) {

														//if we have the array, the shot can't be used
														usePhaserRadarShotToggle.interactable = false;
														phaserRadarShotCountToggle.interactable = false;

													} else {

														usePhaserRadarShotToggle.interactable = true;
														phaserRadarShotCountToggle.interactable = true;

													}

												}

												//check if we have targeting device enabled
												if (phaserRadarArrayToggle.isOn == true || usePhaserRadarShotToggle.isOn == true) {

													phaserTargetingDropdown.interactable = true;

													//if we have targeting enabled, we can't allow the fire button to be pressed until a section has been chosen
													if (phaserTargetingDropdown.GetComponentInChildren<TextMeshProUGUI>().text == "Choose Section") {

														phaserFireButton.interactable = false;

													}
													//the else condition is that a section has been chosen, so we can use the fire button
													else {

														phaserFireButton.interactable = true;

													}


												}
												//the else condition is that we do not have a targeting device enabled
												else {

													phaserTargetingDropdown.interactable = false;

													phaserFireButton.interactable = true;

												}

											}

										}
										//the else condition is that the targeted ship does not have a storage section
										else {

											//if there is no storage section, there can be no jamming
											//we can now set the phaser radar shot toggle based on inventory
											if (mouseMananger.selectedUnit.GetComponentInChildren<StarbasePhaserSection1> ().phaserRadarShot <= 0) {

												//if radar shot == 0, we have none and cannot use it
												usePhaserRadarShotToggle.isOn = false;
												usePhaserRadarShotToggle.interactable = false;

												phaserRadarShotCountToggle.isOn = false;
												phaserRadarShotCountToggle.interactable = false;

											}
											//the else condition is that the radar shot quantity is non-zero
											else {

												//check if we have a radar array
												if (mouseMananger.selectedUnit.GetComponentInChildren<StarbasePhaserSection1> ().phaserRadarArray == true) {

													//if we have the array, the shot can't be used
													usePhaserRadarShotToggle.interactable = false;
													phaserRadarShotCountToggle.interactable = false;

												} else {

													usePhaserRadarShotToggle.interactable = true;
													phaserRadarShotCountToggle.interactable = true;

												}

											}

											//check if we have targeting device enabled
											if (phaserRadarArrayToggle.isOn == true || usePhaserRadarShotToggle.isOn == true) {

												phaserTargetingDropdown.interactable = true;

												//if we have targeting enabled, we can't allow the fire button to be pressed until a section has been chosen
												if (phaserTargetingDropdown.GetComponentInChildren<TextMeshProUGUI>().text == "Choose Section") {

													phaserFireButton.interactable = false;

												}
												//the else condition is that a section has been chosen, so we can use the fire button
												else {

													phaserFireButton.interactable = true;

												}

											}
											//the else condition is that we do not have a targeting device enabled
											else {

												phaserTargetingDropdown.interactable = false;

												phaserFireButton.interactable = true;

											}

										}

									}
									//the else condition is that the targeted unit is a base
									else if(mouseMananger.targetedUnit.GetComponent<Starbase>() == true){

										//now check if the targeted ship has radar jamming
										if (mouseMananger.targetedUnit.GetComponent<StarbaseStorageSection1> ().radarJammingSystem == true) {

											//if it does, we can't allow using targeting
											usePhaserRadarShotToggle.interactable = false;

											phaserRadarShotCountToggle.interactable = false;

											//check if we have radar shot enabled - if we do, we need to turn it off
											if (usePhaserRadarShotToggle.isOn == true) {

												usePhaserRadarShotToggle.isOn = false;
												phaserTargetingDropdown.interactable = false;

											}
											//the else condition is that we do not have a targeting device enabled
											else {

												phaserTargetingDropdown.interactable = false;

											}

											//if the targeted ship has jamming, we can allow the fire phasers button to be pressed
											phaserFireButton.interactable = true;

										}
										//the else condition is that there is no jamming on the targeted unit
										else {

											//we can now set the phaser radar shot toggle based on inventory
											if (mouseMananger.selectedUnit.GetComponentInChildren<StarbasePhaserSection1> ().phaserRadarShot <= 0) {

												//if radar shot == 0, we have none and cannot use it
												usePhaserRadarShotToggle.isOn = false;
												usePhaserRadarShotToggle.interactable = false;

												phaserRadarShotCountToggle.isOn = false;
												phaserRadarShotCountToggle.interactable = false;

											}
											//the else condition is that the radar shot quantity is non-zero
											else {

												//check if we have a radar array
												if (mouseMananger.selectedUnit.GetComponentInChildren<StarbasePhaserSection1> ().phaserRadarArray == true) {

													//if we have the array, the shot can't be used
													usePhaserRadarShotToggle.interactable = false;
													phaserRadarShotCountToggle.interactable = false;

												} else {

													usePhaserRadarShotToggle.interactable = true;
													phaserRadarShotCountToggle.interactable = true;

												}

											}

											//check if we have targeting device enabled
											if (phaserRadarArrayToggle.isOn == true || usePhaserRadarShotToggle.isOn == true) {

												phaserTargetingDropdown.interactable = true;

												//if we have targeting enabled, we can't allow the fire button to be pressed until a section has been chosen
												if (phaserTargetingDropdown.GetComponentInChildren<TextMeshProUGUI>().text == "Choose Section") {

													phaserFireButton.interactable = false;

												}
												//the else condition is that a section has been chosen, so we can use the fire button
												else {

													phaserFireButton.interactable = true;

												}


											}
											//the else condition is that we do not have a targeting device enabled
											else {

												phaserTargetingDropdown.interactable = false;

												phaserFireButton.interactable = true;

											}

										} //end else there is no jamming

									}  //end else if the targeted unit is a base

								} //end else the target is not cloaked

							} //end else we do have a unit targeted

						} // end else we have not used phasers yet

					} //end else we have at least 1 phaser section alive

				} //end if the selected unit is a starbase

				//the else condition is that we do have a ship, so we want to allow toggles based on the ship inventory
				else {

					//check to see if the ship has a phaser section.  If not, it can't use phaser attacks
					if (mouseMananger.selectedUnit.GetComponentInChildren<PhaserSection> () == null) {

						usePhaserRadarShotToggle.isOn = false;
						usePhaserRadarShotToggle.interactable = false;

						phaserRadarShotCountToggle.isOn = false;
						phaserRadarShotCountToggle.interactable = false;

						phaserRadarArrayToggle.isOn = false;
						phaserRadarArrayToggle.interactable = false;

						phaserXRayKernelToggle.isOn = false;
						phaserXRayKernelToggle.interactable = false;

						phaserTargetingDropdown.interactable = false;

						phaserFireButton.interactable = false;

					}

					//the else condition is that there is a phaser section in the selected unit
					else {

						//now we need to make sure the phaser section hasn't been destroyed
						if (mouseMananger.selectedUnit.GetComponentInChildren<PhaserSection> ().isDestroyed == true) {

							//if it is destroyed, we can't use phaser commands
							usePhaserRadarShotToggle.isOn = false;
							usePhaserRadarShotToggle.interactable = false;

							phaserRadarShotCountToggle.isOn = false;
							phaserRadarShotCountToggle.interactable = false;

							phaserRadarArrayToggle.isOn = false;
							phaserRadarArrayToggle.interactable = false;

							phaserXRayKernelToggle.isOn = false;
							phaserXRayKernelToggle.interactable = false;

							phaserTargetingDropdown.interactable = false;

							phaserFireButton.interactable = false;

						}

						//the else condition is that the phaser section is not destroyed.  
						else {

							//set the counts based on ship inventory
							phaserRadarShotCountToggle.GetComponentInChildren<TextMeshProUGUI>().text = mouseMananger.selectedUnit.GetComponentInChildren<PhaserSection> ().phaserRadarShot.ToString();

							//if the ship has the X-ray kernel upgrade drive, set that toggle to on
							if (mouseMananger.selectedUnit.GetComponentInChildren<PhaserSection> ().xRayKernalUpgrade == true) {

								phaserXRayKernelToggle.isOn = true;
								phaserXRayKernelToggle.interactable = false;

							}

							//if the ship has the radar array upgrade, set that toggle to on and disable radar shot single use items
							if (mouseMananger.selectedUnit.GetComponentInChildren<PhaserSection> ().phaserRadarArray == true) {

								phaserRadarArrayToggle.isOn = true;
								phaserRadarArrayToggle.interactable = false;

								usePhaserRadarShotToggle.isOn = false;
								usePhaserRadarShotToggle.interactable = false;

								phaserRadarShotCountToggle.isOn = false;
								phaserRadarShotCountToggle.interactable = false;

							}

							//now we need to check to see if we've already used phasers this turn
							if (mouseMananger.selectedUnit.GetComponentInChildren<PhaserSection> ().usedPhasersThisTurn == true) {

								//if we've already used phasers, we can't use a radar shot or use the fire button
								usePhaserRadarShotToggle.isOn = false;
								usePhaserRadarShotToggle.interactable = false;

								phaserRadarShotCountToggle.isOn = false;
								phaserRadarShotCountToggle.interactable = false;

								phaserTargetingDropdown.interactable = false;

								phaserFireButton.interactable = false;

							}
							//check if we've used torpedos and don't have an additional battle crew - we only get 1 attack per turn without the crew
							else if (mouseMananger.selectedUnit.GetComponent<TorpedoSection> () == true &&
								mouseMananger.selectedUnit.GetComponent<TorpedoSection> ().usedTorpedosThisTurn == true &&

								(mouseMananger.selectedUnit.GetComponent<CrewSection> () == false ||

								(mouseMananger.selectedUnit.GetComponent<CrewSection> () == true &&
										mouseMananger.selectedUnit.GetComponent<CrewSection> ().battleCrew == false))) {

								//if we've already used torpedos and don't have an additional battle crew, we can't use a radar shot or use the fire button
								usePhaserRadarShotToggle.isOn = false;
								usePhaserRadarShotToggle.interactable = false;

								phaserRadarShotCountToggle.isOn = false;
								phaserRadarShotCountToggle.interactable = false;

								phaserTargetingDropdown.interactable = false;

								phaserFireButton.interactable = false;

							}
							//the else condition is that we have not used phasers yet
							else {

								//we can now set the phaser radar shot toggle based on inventory
								if (mouseMananger.selectedUnit.GetComponentInChildren<PhaserSection> ().phaserRadarShot <= 0) {

									//if radar shot == 0, we have none and cannot use it
									usePhaserRadarShotToggle.isOn = false;
									usePhaserRadarShotToggle.interactable = false;

									phaserRadarShotCountToggle.isOn = false;
									phaserRadarShotCountToggle.interactable = false;

								}
								//the else condition is that the radar shot quantity is non-zero
								else {

									//check if we have a radar array
									if (mouseMananger.selectedUnit.GetComponentInChildren<PhaserSection> ().phaserRadarArray == true) {

										//if we have the array, the shot can't be used
										usePhaserRadarShotToggle.interactable = false;
										phaserRadarShotCountToggle.interactable = false;

									} else {

										usePhaserRadarShotToggle.interactable = true;
										phaserRadarShotCountToggle.interactable = true;

									}

								}

								//now we need to check to see if we have a unit targeted
								if (mouseMananger.targetedUnit == null) {

									//if there is no targeted unit, we can't fire the phasers
									phaserTargetingDropdown.interactable = false;

									phaserFireButton.interactable = false;

								}

								//the else condition is that we do have a unit targeted
								else {

									//check if the targeted unit has a cloaking device turned on
									if (mouseMananger.targetedUnit.GetComponent<CloakingDevice> () && mouseMananger.targetedUnit.GetComponent<CloakingDevice> ().isCloaked == true) {

										//if it is cloaked, we can't allow using targeting
										usePhaserRadarShotToggle.interactable = false;

										phaserRadarShotCountToggle.interactable = false;

										//check if we have radar shot enabled - if we do, we need to turn it off
										if (usePhaserRadarShotToggle.isOn == true) {

											usePhaserRadarShotToggle.isOn = false;
											phaserTargetingDropdown.interactable = false;

										}
										//the else condition is that we do not have a targeting device enabled
										else {

											phaserTargetingDropdown.interactable = false;

										}

										//if the targeted unit is cloaked, we can allow the fire phasers button to be pressed 
										//as long as we are not cloaked
										if (mouseMananger.selectedUnit.GetComponent<CloakingDevice> () == true &&
										    mouseMananger.selectedUnit.GetComponent<CloakingDevice> ().isCloaked == true) {

											//we are cloaked and cannot fire
											phaserFireButton.interactable = false;

										} else {
											
											phaserFireButton.interactable = true;

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
													usePhaserRadarShotToggle.interactable = false;

													phaserRadarShotCountToggle.interactable = false;

													//check if we have radar shot enabled - if we do, we need to turn it off
													if (usePhaserRadarShotToggle.isOn == true) {

														usePhaserRadarShotToggle.isOn = false;
														phaserTargetingDropdown.interactable = false;

													}
													//the else condition is that we do not have a targeting device enabled
													else {

														phaserTargetingDropdown.interactable = false;

													}

													//if the targeted ship has jamming, we can allow the fire phasers button to be pressed
													//as long as we are not cloaked
													if (mouseMananger.selectedUnit.GetComponent<CloakingDevice> () == true &&
														mouseMananger.selectedUnit.GetComponent<CloakingDevice> ().isCloaked == true) {

														//we are cloaked and cannot fire
														phaserFireButton.interactable = false;

													} else {

														phaserFireButton.interactable = true;

													}

												}
												//the else condition is that there is no jamming on the targeted unit
												else {

													//we can now set the phaser radar shot toggle based on inventory
													if (mouseMananger.selectedUnit.GetComponentInChildren<PhaserSection> ().phaserRadarShot <= 0) {

														//if radar shot == 0, we have none and cannot use it
														usePhaserRadarShotToggle.isOn = false;
														usePhaserRadarShotToggle.interactable = false;

														phaserRadarShotCountToggle.isOn = false;
														phaserRadarShotCountToggle.interactable = false;

													}
													//the else condition is that the radar shot quantity is non-zero
													else {
														
														//check if we have a radar array
														if (mouseMananger.selectedUnit.GetComponentInChildren<PhaserSection> ().phaserRadarArray == true) {

															//if we have the array, the shot can't be used
															usePhaserRadarShotToggle.interactable = false;
															phaserRadarShotCountToggle.interactable = false;

														} else {

															usePhaserRadarShotToggle.interactable = true;
															phaserRadarShotCountToggle.interactable = true;

														}

													}

													//check if we have targeting device enabled
													if (phaserRadarArrayToggle.isOn == true || usePhaserRadarShotToggle.isOn == true) {

														phaserTargetingDropdown.interactable = true;

														//if we have targeting enabled, we can't allow the fire button to be pressed until a section has been chosen
														if (phaserTargetingDropdown.GetComponentInChildren<TextMeshProUGUI> ().text == "Choose Section") {

															phaserFireButton.interactable = false;

														}
														//the else condition is that a section has been chosen, so we can use the fire button
														//as long as we are not cloaked
														else {

															if (mouseMananger.selectedUnit.GetComponent<CloakingDevice> () == true &&
																mouseMananger.selectedUnit.GetComponent<CloakingDevice> ().isCloaked == true) {

																//we are cloaked and cannot fire
																phaserFireButton.interactable = false;

															} else {

																phaserFireButton.interactable = true;

															}

														}


													}
													//the else condition is that we do not have a targeting device enabled
													else {

														phaserTargetingDropdown.interactable = false;

														//check if we are cloaked

														if (mouseMananger.selectedUnit.GetComponent<CloakingDevice> () == true &&
															mouseMananger.selectedUnit.GetComponent<CloakingDevice> ().isCloaked == true) {

															//we are cloaked and cannot fire
															phaserFireButton.interactable = false;

														} else {

															phaserFireButton.interactable = true;

														}

													}

												}

											}
											//the else condition is that the targeted ship does not have a storage section
											else {

												//we can now set the phaser radar shot toggle based on inventory
												if (mouseMananger.selectedUnit.GetComponentInChildren<PhaserSection> ().phaserRadarShot <= 0) {

													//if radar shot == 0, we have none and cannot use it
													usePhaserRadarShotToggle.isOn = false;
													usePhaserRadarShotToggle.interactable = false;

													phaserRadarShotCountToggle.isOn = false;
													phaserRadarShotCountToggle.interactable = false;

												}
												//the else condition is that the radar shot quantity is non-zero
												else {
													
													//check if we have a radar array
													if (mouseMananger.selectedUnit.GetComponentInChildren<PhaserSection> ().phaserRadarArray == true) {

														//if we have the array, the shot can't be used
														usePhaserRadarShotToggle.interactable = false;
														phaserRadarShotCountToggle.interactable = false;

													} else {

														usePhaserRadarShotToggle.interactable = true;
														phaserRadarShotCountToggle.interactable = true;

													}

												}

												//check if we have targeting device enabled
												if (phaserRadarArrayToggle.isOn == true || usePhaserRadarShotToggle.isOn == true) {

													phaserTargetingDropdown.interactable = true;

													//if we have targeting enabled, we can't allow the fire button to be pressed until a section has been chosen
													if (phaserTargetingDropdown.GetComponentInChildren<TextMeshProUGUI> ().text == "Choose Section") {

														phaserFireButton.interactable = false;

													}
													//the else condition is that a section has been chosen, so we can use the fire button
													//as long as we are not cloaked
													else {

														if (mouseMananger.selectedUnit.GetComponent<CloakingDevice> () == true &&
															mouseMananger.selectedUnit.GetComponent<CloakingDevice> ().isCloaked == true) {

															//we are cloaked and cannot fire
															phaserFireButton.interactable = false;

														} else {

															phaserFireButton.interactable = true;

														}

													}
											
												}
												//the else condition is that we do not have a targeting device enabled
												else {

													phaserTargetingDropdown.interactable = false;

													//check if we are cloaked

													if (mouseMananger.selectedUnit.GetComponent<CloakingDevice> () == true &&
														mouseMananger.selectedUnit.GetComponent<CloakingDevice> ().isCloaked == true) {

														//we are cloaked and cannot fire
														phaserFireButton.interactable = false;

													} else {

														phaserFireButton.interactable = true;

													}

												}

											}

										}
										//the else condition is that the targeted unit is a base
										else if (mouseMananger.targetedUnit.GetComponent<Starbase> () == true) {

											//now check if the targeted ship has radar jamming
											if (mouseMananger.targetedUnit.GetComponent<StarbaseStorageSection1> ().radarJammingSystem == true) {

												//if it does, we can't allow using targeting
												usePhaserRadarShotToggle.interactable = false;

												phaserRadarShotCountToggle.interactable = false;

												//check if we have radar shot enabled - if we do, we need to turn it off
												if (usePhaserRadarShotToggle.isOn == true) {

													usePhaserRadarShotToggle.isOn = false;
													phaserTargetingDropdown.interactable = false;

												}
												//the else condition is that we do not have a targeting device enabled
												else {

													phaserTargetingDropdown.interactable = false;

												}

												//if the targeted ship has jamming, we can allow the fire phasers button to be pressed
												//as long as we are not cloaked
												if (mouseMananger.selectedUnit.GetComponent<CloakingDevice> () == true &&
													mouseMananger.selectedUnit.GetComponent<CloakingDevice> ().isCloaked == true) {

													//we are cloaked and cannot fire
													phaserFireButton.interactable = false;

												} else {

													phaserFireButton.interactable = true;

												}

											}
											//the else condition is that there is no jamming on the targeted unit
											else {

												//we can now set the phaser radar shot toggle based on inventory
												if (mouseMananger.selectedUnit.GetComponentInChildren<PhaserSection> ().phaserRadarShot <= 0) {

													//if radar shot == 0, we have none and cannot use it
													usePhaserRadarShotToggle.isOn = false;
													usePhaserRadarShotToggle.interactable = false;

													phaserRadarShotCountToggle.isOn = false;
													phaserRadarShotCountToggle.interactable = false;

												}
												//the else condition is that the radar shot quantity is non-zero
												else {
													
													//check if we have a radar array
													if (mouseMananger.selectedUnit.GetComponentInChildren<PhaserSection> ().phaserRadarArray == true) {

														//if we have the array, the shot can't be used
														usePhaserRadarShotToggle.interactable = false;
														phaserRadarShotCountToggle.interactable = false;

													} else {

														usePhaserRadarShotToggle.interactable = true;
														phaserRadarShotCountToggle.interactable = true;

													}

												}

												//check if we have targeting device enabled
												if (phaserRadarArrayToggle.isOn == true || usePhaserRadarShotToggle.isOn == true) {

													phaserTargetingDropdown.interactable = true;

													//if we have targeting enabled, we can't allow the fire button to be pressed until a section has been chosen
													if (phaserTargetingDropdown.GetComponentInChildren<TextMeshProUGUI> ().text == "Choose Section") {

														phaserFireButton.interactable = false;

													}
													//the else condition is that a section has been chosen, so we can use the fire button
													//as long as we are not cloaked
													else {

														if (mouseMananger.selectedUnit.GetComponent<CloakingDevice> () == true &&
															mouseMananger.selectedUnit.GetComponent<CloakingDevice> ().isCloaked == true) {

															//we are cloaked and cannot fire
															phaserFireButton.interactable = false;

														} else {

															phaserFireButton.interactable = true;

														}

													}


												}
												//the else condition is that we do not have a targeting device enabled
												else {

													phaserTargetingDropdown.interactable = false;

													//check if we are cloaked
													if (mouseMananger.selectedUnit.GetComponent<CloakingDevice> () == true &&
														mouseMananger.selectedUnit.GetComponent<CloakingDevice> ().isCloaked == true) {

														//we are cloaked and cannot fire
														phaserFireButton.interactable = false;

													} else {

														phaserFireButton.interactable = true;

													}

												}

											} //end else there is no jamming

										}  //end else if the targeted unit is a base

									}  //end else if the target is uncloaked

								} //end else we do have a unit targeted

							} //end else we have not used phasers yet

						} //end else the phaser section is not destroyed

					}  //end else the selected unit has a phaser section

				} //end else the selected unit is a ship

			} //end else there is a selected unit

			SetTargetingText ();
			SetTargetingSectionOptions ();
			phaserTargetingDropdown.RefreshShownValue ();
			SetFireButtonText ();


		} //end if the phaser attack toggle has been turned on

		//this is if the phaser attack has been turned off
		else if (phaserToggle.isOn == false) {

			//if we have don't have a selected unit, we can turn the toggles off and make them not interactable
			if (mouseMananger.selectedUnit == null) {

				usePhaserRadarShotToggle.isOn = false;
				usePhaserRadarShotToggle.interactable = false;

				phaserRadarShotCountToggle.isOn = false;
				phaserRadarShotCountToggle.interactable = false;

				phaserRadarArrayToggle.isOn = false;
				phaserRadarArrayToggle.interactable = false;

				phaserXRayKernelToggle.isOn = false;
				phaserXRayKernelToggle.interactable = false;

				phaserTargetingDropdown.interactable = false;

				phaserFireButton.interactable = false;

				SetTargetingText ();
				SetFireButtonText ();


			}

			//now we can check if we have a targeted unit
			if (mouseMananger.targetedUnit == null) {

				//if there is no targeted unit anymore, we can set the toggles off and make them not interactable
				usePhaserRadarShotToggle.isOn = false;
				usePhaserRadarShotToggle.interactable = false;

				phaserRadarShotCountToggle.isOn = false;
				phaserRadarShotCountToggle.interactable = false;

				phaserRadarArrayToggle.isOn = false;
				phaserRadarArrayToggle.interactable = false;

				phaserXRayKernelToggle.isOn = false;
				phaserXRayKernelToggle.interactable = false;

				phaserTargetingDropdown.interactable = false;

				phaserFireButton.interactable = false;

				SetTargetingText ();
				SetFireButtonText ();

			}

		}

	}

	//this function sets the helper text for the phaser targeting
	private void SetTargetingText(){

		//check if there is a targeted unit
		if (mouseMananger.targetedUnit != null) {

			//check if targeted unit is cloaked
			if (mouseMananger.targetedUnit.GetComponent<CloakingDevice> () == true && mouseMananger.targetedUnit.GetComponent<CloakingDevice> ().isCloaked == true) {

				phaserTargetingText.text = ("Target Is Cloaked!");

				//update the font size if necessary
				UIManager.AutoSizeTextMeshFont(phaserTargetingText);

			}
			//the else condition is that the targeted unit is not cloaked
			else{
				
				//check if the targeted unit is a ship
				if (mouseMananger.targetedUnit.GetComponent<Ship> () == true) {

					//check if the targeted ship has a storage section
					if (mouseMananger.targetedUnit.GetComponent<StorageSection> () == true) {

						//check if the targeted ship has radar jamming
						if (mouseMananger.targetedUnit.GetComponent<StorageSection> ().radarJammingSystem == true) {

							phaserTargetingText.text = ("Target Has Jamming!");

							//update the font size if necessary
							UIManager.AutoSizeTextMeshFont(phaserTargetingText);

						}
						//the else condition is that the targeted unit does not have jamming
						else {

							//check if we have targeting device enabled
							//note that there must be a selected unit in order for there to be a targeted unit
							if (phaserRadarArrayToggle.isOn == true || usePhaserRadarShotToggle.isOn == true) {

								//Debug.Log ("Choose Section 1");

								phaserTargetingText.text = ("Choose Section");

								//update the font size if necessary
								UIManager.AutoSizeTextMeshFont(phaserTargetingText);

							}
							//the else condition is that we do not have a targeting device enabled
							else {

								phaserTargetingText.text = ("No Targeting Active");

								//update the font size if necessary
								UIManager.AutoSizeTextMeshFont(phaserTargetingText);

							}

						}

					}
					//the else condition is that the targeted ship has no storage section, which means it can't have jamming
					else {

						//check if we have targeting device enabled
						//note that there must be a selected unit in order for there to be a targeted unit
						if (phaserRadarArrayToggle.isOn == true || usePhaserRadarShotToggle.isOn == true) {

							//Debug.Log ("Choose Section 2");


							phaserTargetingText.text = ("Choose Section");

							//update the font size if necessary
							UIManager.AutoSizeTextMeshFont(phaserTargetingText);

						}
						//the else condition is that we do not have a targeting device enabled
						else {

							phaserTargetingText.text = ("No Targeting Active");

							//update the font size if necessary
							UIManager.AutoSizeTextMeshFont(phaserTargetingText);

						}

					}

				}
				//the else condition is that the targeted unit is not a ship
				else if (mouseMananger.targetedUnit.GetComponent<Starbase> () == true) {

					//check if the targeted ship has radar jamming
					if (mouseMananger.targetedUnit.GetComponent<StarbaseStorageSection1> ().radarJammingSystem == true) {

						phaserTargetingText.text = ("Target Has Jamming!");

						//update the font size if necessary
						UIManager.AutoSizeTextMeshFont(phaserTargetingText);

					}
					//the else condition is that the targeted unit does not have jamming
					else {

						//check if we have targeting device enabled
						//note that there must be a selected unit in order for there to be a targeted unit
						if (phaserRadarArrayToggle.isOn == true || usePhaserRadarShotToggle.isOn == true) {

							//Debug.Log ("Choose Section 3");


							phaserTargetingText.text = ("Choose Section");

							//update the font size if necessary
							UIManager.AutoSizeTextMeshFont(phaserTargetingText);

						}
						//the else condition is that we do not have a targeting device enabled
						else {

							phaserTargetingText.text = ("No Targeting Active");

							//update the font size if necessary
							UIManager.AutoSizeTextMeshFont(phaserTargetingText);

						}

					}

				}  //end else the targeted unit is a starbase
			} //end else the targeted unit is not cloaked
		} //end if there is a targeted unit
		//the else condition is that there is no targeted unit
		else {

			//check whether we have a selected unit
			if (mouseMananger.selectedUnit != null) {

				phaserTargetingText.text = ("No Unit Targeted");

				//update the font size if necessary
				UIManager.AutoSizeTextMeshFont(phaserTargetingText);

			}
			//the else condition is that there is no selected unit
			else {

				phaserTargetingText.text = ("No Unit Selected");

				//update the font size if necessary
				UIManager.AutoSizeTextMeshFont(phaserTargetingText);

			}
				
		}

	}

	//this function will set the available options for the targeting dropdown menu
	private void SetTargetingSectionOptions(){

		//start by clearing the existing dropdown options
		phaserTargetingDropdown.ClearOptions();

		//create a list of new dropdown options to populate the choices
		List<TMP_Dropdown.OptionData> dropDownOptions = new List<TMP_Dropdown.OptionData> ();

		//add a dummy option at the top of the list
		dropDownOptions.Add(new TMP_Dropdown.OptionData("Choose Section"));

		//check if there is a targeted unit
		if (mouseMananger.targetedUnit != null) {

			//check if we have a targeting device enabled
			if (phaserRadarArrayToggle.isOn == true || usePhaserRadarShotToggle.isOn == true) {

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

					//check if the targeted ship has a phaser section
					if (mouseMananger.targetedUnit.GetComponent<PhaserSection> () == true) {

						//check if the phaser section is not destroyed
						if (mouseMananger.targetedUnit.GetComponent<PhaserSection> ().isDestroyed == false) {

							//add a dropdown option for the phaser section
							dropDownOptions.Add (new TMP_Dropdown.OptionData ("Phaser Section"));

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

					//check if the phaser 1 section is not destroyed
					if (mouseMananger.targetedUnit.GetComponent<StarbasePhaserSection1> ().isDestroyed == false) {

						//add a dropdown option for the phaser section 1
						dropDownOptions.Add (new TMP_Dropdown.OptionData ("Phaser Section 1"));

					}

					//check if the phaser 2 section is not destroyed
					if (mouseMananger.targetedUnit.GetComponent<StarbasePhaserSection2> ().isDestroyed == false) {

						//add a dropdown option for the phaser section 1
						dropDownOptions.Add (new TMP_Dropdown.OptionData ("Phaser Section 2"));

					}

					//check if the torpedo section is not destroyed
					if (mouseMananger.targetedUnit.GetComponent<StarbaseTorpedoSection> ().isDestroyed == false) {

						//add a dropdown option for the phaser section 1
						dropDownOptions.Add (new TMP_Dropdown.OptionData ("Torpedo Section"));

					}


					//check if the crew is not destroyed
					if (mouseMananger.targetedUnit.GetComponent<StarbaseCrewSection> ().isDestroyed == false) {

						//add a dropdown option for the phaser section 1
						dropDownOptions.Add (new TMP_Dropdown.OptionData ("Crew Section"));

					}

					//check if the storage section 1 is not destroyed
					if (mouseMananger.targetedUnit.GetComponent<StarbaseStorageSection1> ().isDestroyed == false) {

						//add a dropdown option for the phaser section 1
						dropDownOptions.Add (new TMP_Dropdown.OptionData ("Storage Section 1"));

					}

					//check if the storage section 2 is not destroyed
					if (mouseMananger.targetedUnit.GetComponent<StarbaseStorageSection2> ().isDestroyed == false) {

						//add a dropdown option for the phaser section 1
						dropDownOptions.Add (new TMP_Dropdown.OptionData ("Storage Section 2"));

					}

				}

			}
			//the else condition is that we do not have a targeting device enabled

		}
			
		//add the options to the dropdown
		phaserTargetingDropdown.AddOptions (dropDownOptions);

		//update the font size if necessary
		UIManager.AutoSizeTextMeshFont(phaserTargetingDropdown.GetComponentInChildren<TextMeshProUGUI> ());

	}

	//this function will set the fire button text
	private void SetFireButtonText(){

		//check if we have a selected unit
		if (mouseMananger.selectedUnit != null) {

			//check if the selected unit has a valid phaser attack remaining
			if (mouseMananger.selectedUnit.GetComponent<CombatUnit> ().hasRemainingPhaserAttack == true) {

				//check if the selected unit is a bird of prey and is cloaked
				if (mouseMananger.selectedUnit.GetComponent<CloakingDevice> () == true) {

					if (mouseMananger.selectedUnit.GetComponent<CloakingDevice> ().isCloaked == true) {

						//if the attack is still remaining, set the text to Cloaked
						phaserFireButton.GetComponentInChildren<TextMeshProUGUI> ().text = ("Cloaked");

						//update the font size if necessary
						UIManager.AutoSizeTextMeshFont (phaserFireButton.GetComponentInChildren<TextMeshProUGUI> ());

					} else {

						//if the attack is still remaining, set the text to Fire Phasers
						phaserFireButton.GetComponentInChildren<TextMeshProUGUI> ().text = ("Fire Phasers");

						//update the font size if necessary
						UIManager.AutoSizeTextMeshFont (phaserFireButton.GetComponentInChildren<TextMeshProUGUI> ());

					}

				} else {

					//if the attack is still remaining, set the text to Fire Phasers
					phaserFireButton.GetComponentInChildren<TextMeshProUGUI> ().text = ("Fire Phasers");

					//update the font size if necessary
					UIManager.AutoSizeTextMeshFont (phaserFireButton.GetComponentInChildren<TextMeshProUGUI> ());

				}

			}
			//else, we want to check if the attack is not remaining because we already fired phasers
			//this is a slightly different check for ships and bases
			else if ((mouseMananger.selectedUnit.GetComponent<PhaserSection> () == true &&
			         mouseMananger.selectedUnit.GetComponent<PhaserSection> ().usedPhasersThisTurn == true) ||

			         (mouseMananger.selectedUnit.GetComponent<StarbasePhaserSection1> () == true &&
			         (mouseMananger.selectedUnit.GetComponent<StarbasePhaserSection1> ().usedPhasersThisTurn == true ||
			         mouseMananger.selectedUnit.GetComponent<StarbasePhaserSection2> ().usedPhasersThisTurn == true))) {

				//if we already fired phasers, set the text to already fired
				phaserFireButton.GetComponentInChildren<TextMeshProUGUI> ().text = ("Already Fired");

				//update the font size if necessary
				UIManager.AutoSizeTextMeshFont(phaserFireButton.GetComponentInChildren<TextMeshProUGUI> ());

			}
			//the else condition is that the attack is not remaining and we did not fire phasers, which means we must have fired a torpedo
			//and don't have double-attack capability
			else {

				phaserFireButton.GetComponentInChildren<TextMeshProUGUI> ().text = ("Already Attacked");

				//update the font size if necessary
				UIManager.AutoSizeTextMeshFont(phaserFireButton.GetComponentInChildren<TextMeshProUGUI> ());

			}

		} else {

			phaserFireButton.GetComponentInChildren<TextMeshProUGUI> ().text = ("Fire Phasers");

			//update the font size if necessary
			UIManager.AutoSizeTextMeshFont(phaserFireButton.GetComponentInChildren<TextMeshProUGUI> ());

		}

	}


	//this function handles firing the phasers
	private void FirePhasers(){

		//Debug.Log ("Fire Phasers!");

		//invoke the OnFirePhasers event, with the targeted unit passed to the event
		OnFirePhasers.Invoke(mouseMananger.selectedUnit.GetComponent<CombatUnit>(),
			mouseMananger.targetedUnit.GetComponent<CombatUnit>(),
			phaserTargetingDropdown.GetComponentInChildren<TextMeshProUGUI>().text);

		SetPhaserMenuToggles ();

	}

	//this function handles OnDestroy
	private void OnDestroy(){

		RemoveAllListeners();

	}

	//this function removes all listeners
	private void RemoveAllListeners(){

		if (phaserToggle != null) {
			
			//remove an on-click event listener for the main menu phaser toggle
			phaserToggle.onValueChanged.RemoveListener (boolSetPhaserMenuToggleAction);

		}

		if (usePhaserRadarShotToggle != null) {
			
			//remove an on-click event listener for the phaser radar shot toggle
			usePhaserRadarShotToggle.onValueChanged.RemoveListener (boolSetPhaserMenuToggleDropdownZeroAction);

		}

		if (phaserTargetingDropdown != null) {
			
			//remove an event listener for the dropdown value changing
			phaserTargetingDropdown.onValueChanged.RemoveListener (intSetPhaserMenuToggleAction);

		}

		if (phaserFireButton != null) {
			
			//remove an on-click event listener for the fire phasers toggle
			phaserFireButton.onClick.RemoveListener (FirePhasers);

		}

		if (mouseMananger != null) {
			
			//remove an event listener for when a selectedUnit is set and cleared
			mouseMananger.OnSetSelectedUnit.RemoveListener (SetPhaserMenuToggles);
			mouseMananger.OnClearSelectedUnit.RemoveListener (SetPhaserMenuToggles);

			//remove an event listener for when a targetedUnit is set and cleared
			mouseMananger.OnSetTargetedUnit.RemoveListener (SetPhaserMenuToggles);
			mouseMananger.OnClearTargetedUnit.RemoveListener (SetPhaserMenuToggles);

		}

		//remove a listener for when a purchase is complete
		PhaserSection.OnInventoryUpdated.RemoveListener (combatUnitSetPhaserMenuToggleAction);

	}

}
