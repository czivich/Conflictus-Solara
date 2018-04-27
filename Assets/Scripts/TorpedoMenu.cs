using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class TorpedoMenu : MonoBehaviour {

	//we will need the mouseManager to check the selected unit
	private MouseManager mouseMananger;

	//variable for torpedo menu trigger toggle
	public Toggle torpedoToggle;

	//variable for torpedo menu torpedo laser shot toggle
	public Toggle useTorpedoLaserShotToggle;

	//variable for the system upgrages
	public Toggle torpedoLaserGuidanceToggle;
	public Toggle torpedoHighPressureTubesToggle;

	//variable to hold the fire buttons
	public Button lightTorpedoFireButton;
	public Button heavyTorpedoFireButton;

	//variables for the item counts
	public Toggle torpedoLaserShotCountToggle;
	public Toggle lightTorpedoCountToggle;
	public Toggle heavyTorpedoCountToggle;

	//variable for the targeting dropdown
	public TMP_Dropdown torpedoTargetingDropdown;

	//variable for the targeting helper text
	public TextMeshProUGUI torpedoTargetingText;


	//these events are for when the torpedos are fired
	public TorpedoEvent OnFireLightTorpedo = new TorpedoEvent();
	public TorpedoEvent OnFireHeavyTorpedo = new TorpedoEvent();

	//simple class derived from unityEvent to pass Ship Object
	public class TorpedoEvent : UnityEvent<CombatUnit, CombatUnit, string>{};

	//unityActions
	private UnityAction<bool> boolSetTorpedoMenuTogglesAction;
	private UnityAction<bool> boolSetTorpedoMenuTogglesDropdownZeroAction;
	private UnityAction<int> intSetTorpedoMenuTogglesAction;
	private UnityAction<CombatUnit> combatUnitSetTorpedoMenuTogglesAction;


	// Use this for initialization
	public void Init () {

		//set actions
		boolSetTorpedoMenuTogglesAction = (value) => {SetTorpedoMenuToggles();};
		boolSetTorpedoMenuTogglesDropdownZeroAction = (value) => {
			SetTorpedoMenuToggles();
			torpedoTargetingDropdown.value = 0;		//I want the menu to go back to "Choose Section" when we toggle targeting
		};
		intSetTorpedoMenuTogglesAction = (value) => {SetTorpedoMenuToggles();};

		combatUnitSetTorpedoMenuTogglesAction = (combatUnit) => {SetTorpedoMenuToggles();};


		//get the mouseManager
		mouseMananger = GameObject.FindGameObjectWithTag("MouseManager").GetComponent<MouseManager>();

		//add an on-click event listener for the main menu torpedo toggle
		torpedoToggle.onValueChanged.AddListener(boolSetTorpedoMenuTogglesAction);

		//add an on-click event listener for the torpedo laser shot toggle
		useTorpedoLaserShotToggle.onValueChanged.AddListener(boolSetTorpedoMenuTogglesDropdownZeroAction);

		//add an event listener for the dropdown value changing
		torpedoTargetingDropdown.onValueChanged.AddListener(intSetTorpedoMenuTogglesAction);

		//add an on-click event listener for the fire torpedos buttons
		lightTorpedoFireButton.onClick.AddListener(FireLightTorpedo);
		heavyTorpedoFireButton.onClick.AddListener(FireHeavyTorpedo);

		//add an event listener for when a selectedUnit is set and cleared
		mouseMananger.OnSetSelectedUnit.AddListener(SetTorpedoMenuToggles);
		mouseMananger.OnClearSelectedUnit.AddListener(SetTorpedoMenuToggles);

		//add an event listener for when a targetedUnit is set and cleared
		mouseMananger.OnSetTargetedUnit.AddListener(SetTorpedoMenuToggles);
		mouseMananger.OnClearTargetedUnit.AddListener(SetTorpedoMenuToggles);

		//add a listener for when a purchase is complete
		TorpedoSection.OnInventoryUpdated.AddListener(combatUnitSetTorpedoMenuTogglesAction);

	}

	//this function sets the interactability of the torpedo menu toggles
	private void SetTorpedoMenuToggles(){

		//this is if the Torpedo Attack menu Toggle has been turned on
		if (torpedoToggle.isOn == true) {

			//if there is no unit selected, set toggles to not interactable and turn them off
			if (mouseMananger.selectedUnit == null) {

				useTorpedoLaserShotToggle.isOn = false;
				useTorpedoLaserShotToggle.interactable = false;

				torpedoLaserShotCountToggle.isOn = false;
				torpedoLaserShotCountToggle.interactable = false;

				torpedoLaserGuidanceToggle.isOn = false;
				torpedoLaserGuidanceToggle.interactable = false;

				torpedoHighPressureTubesToggle.isOn = false;
				torpedoHighPressureTubesToggle.interactable = false;



				torpedoTargetingDropdown.interactable = false;

				lightTorpedoFireButton.interactable = false;
				lightTorpedoCountToggle.isOn = false;
				lightTorpedoCountToggle.interactable = false;

				heavyTorpedoFireButton.interactable = false;
				heavyTorpedoCountToggle.isOn = false;
				heavyTorpedoCountToggle.interactable = false;

			}

			//the else condition means that there is a selected unit
			else {

				//now we need to make sure the selected unit is a ship
				//if it's a starbase, we need to handle with slightly diffferent logic
				if (mouseMananger.selectedUnit.GetComponent<Ship> () == null) {

					//if we are here, that means that the selected unit is a starbase
					//now we need to make sure the torpedo section hasn't been destroyed
					if (mouseMananger.selectedUnit.GetComponentInChildren<StarbaseTorpedoSection> ().isDestroyed == true) {

						//if it is destroyed, we can't use torpedo commands
						useTorpedoLaserShotToggle.isOn = false;
						useTorpedoLaserShotToggle.interactable = false;

						torpedoLaserShotCountToggle.isOn = false;
						torpedoLaserShotCountToggle.interactable = false;

						torpedoLaserGuidanceToggle.isOn = false;
						torpedoLaserGuidanceToggle.interactable = false;

						torpedoHighPressureTubesToggle.isOn = false;
						torpedoHighPressureTubesToggle.interactable = false;



						torpedoTargetingDropdown.interactable = false;

						lightTorpedoFireButton.interactable = false;
						lightTorpedoCountToggle.isOn = false;
						lightTorpedoCountToggle.interactable = false;

						heavyTorpedoFireButton.interactable = false;
						heavyTorpedoCountToggle.isOn = false;
						heavyTorpedoCountToggle.interactable = false;


					}

					//the else condition is that the torpedo section is not destroyed.  
					else {

						//set the counts based on starbase inventory
						torpedoLaserShotCountToggle.GetComponentInChildren<TextMeshProUGUI>().text = mouseMananger.selectedUnit.GetComponentInChildren<StarbaseTorpedoSection> ().torpedoLaserShot.ToString();
						lightTorpedoCountToggle.GetComponentInChildren<TextMeshProUGUI>().text = mouseMananger.selectedUnit.GetComponentInChildren<StarbaseTorpedoSection> ().lightTorpedos.ToString();
						heavyTorpedoCountToggle.GetComponentInChildren<TextMeshProUGUI>().text = mouseMananger.selectedUnit.GetComponentInChildren<StarbaseTorpedoSection> ().heavyTorpedos.ToString();


						//if the starbase has the high pressure tubes upgrade, set that toggle to on
						if (mouseMananger.selectedUnit.GetComponentInChildren<StarbaseTorpedoSection> ().highPressureTubes == true) {

							torpedoHighPressureTubesToggle.isOn = true;
							torpedoHighPressureTubesToggle.interactable = false;

						}

						//if the starbase has the laser guidance upgrade, set that toggle to on and disable laser shot single use items
						if (mouseMananger.selectedUnit.GetComponentInChildren<StarbaseTorpedoSection> ().torpedoLaserGuidanceSystem == true) {

							torpedoLaserGuidanceToggle.isOn = true;
							torpedoLaserGuidanceToggle.interactable = false;

							useTorpedoLaserShotToggle.isOn = false;
							useTorpedoLaserShotToggle.interactable = false;

							torpedoLaserShotCountToggle.isOn = false;
							torpedoLaserShotCountToggle.interactable = false;

						}

						//now we need to check to see if we've already used torpedos this turn
						if (mouseMananger.selectedUnit.GetComponentInChildren<StarbaseTorpedoSection> ().usedTorpedosThisTurn == true) {

							//if we've already used torpedos, we can't use a laser shot or use the fire buttons
							useTorpedoLaserShotToggle.isOn = false;
							useTorpedoLaserShotToggle.interactable = false;

							torpedoLaserShotCountToggle.isOn = false;
							torpedoLaserShotCountToggle.interactable = false;

							torpedoTargetingDropdown.interactable = false;

							lightTorpedoFireButton.interactable = false;
							lightTorpedoCountToggle.isOn = false;
							lightTorpedoCountToggle.interactable = false;

							heavyTorpedoFireButton.interactable = false;
							heavyTorpedoCountToggle.isOn = false;
							heavyTorpedoCountToggle.interactable = false;

						}
						//check if we've used phasors and don't have an additional battle crew - we only get 1 attack per turn without the crew
						else if ((mouseMananger.selectedUnit.GetComponent<StarbasePhasorSection1> ().usedPhasorsThisTurn == true ||
							mouseMananger.selectedUnit.GetComponent<StarbasePhasorSection2> ().usedPhasorsThisTurn == true ) &&
							mouseMananger.selectedUnit.GetComponent<StarbaseCrewSection> ().battleCrew == false) {

							//if we've already used phasors and don't have an additional battle crew, we can't use a laser shot or use the fire button
							useTorpedoLaserShotToggle.isOn = false;
							useTorpedoLaserShotToggle.interactable = false;

							torpedoLaserShotCountToggle.isOn = false;
							torpedoLaserShotCountToggle.interactable = false;

							torpedoTargetingDropdown.interactable = false;

							lightTorpedoFireButton.interactable = false;
							lightTorpedoCountToggle.isOn = false;
							lightTorpedoCountToggle.interactable = false;

							heavyTorpedoFireButton.interactable = false;
							heavyTorpedoCountToggle.isOn = false;
							heavyTorpedoCountToggle.interactable = false;

						}
						//the else condition is that we have not used torpedos yet
						else {

							//we can now set the torpedo laser shot toggle based on inventory
							if (mouseMananger.selectedUnit.GetComponentInChildren<StarbaseTorpedoSection> ().torpedoLaserShot <= 0) {

								//if laser shot == 0, we have none and cannot use it
								useTorpedoLaserShotToggle.isOn = false;
								useTorpedoLaserShotToggle.interactable = false;

								torpedoLaserShotCountToggle.isOn = false;
								torpedoLaserShotCountToggle.interactable = false;

							}
							//the else condition is that the laser shot quantity is greater than zero
							else {

								//check if we have a laser guidance system
								if (mouseMananger.selectedUnit.GetComponentInChildren<StarbaseTorpedoSection> ().torpedoLaserGuidanceSystem == true) {

									//if we have the system, the shot can't be used
									useTorpedoLaserShotToggle.interactable = false;
									torpedoLaserShotCountToggle.interactable = false;

								} else {

									useTorpedoLaserShotToggle.interactable = true;
									torpedoLaserShotCountToggle.interactable = true;

								}

							}

							//now we need to check to see if we have a unit targeted
							if (mouseMananger.targetedUnit == null) {

								//if there is no targeted unit, we can't fire torpedos
								torpedoTargetingDropdown.interactable = false;

								lightTorpedoFireButton.interactable = false;
								lightTorpedoCountToggle.isOn = false;
								lightTorpedoCountToggle.interactable = false;

								heavyTorpedoFireButton.interactable = false;
								heavyTorpedoCountToggle.isOn = false;
								heavyTorpedoCountToggle.interactable = false;


							}

							//the else condition is that we do have a unit targeted
							else {

								//check if the targeted unit has a cloaking device turned on
								if (mouseMananger.targetedUnit.GetComponent<CloakingDevice> () && mouseMananger.targetedUnit.GetComponent<CloakingDevice> ().isCloaked == true) {

									//if it is cloaked, we can't allow using targeting
									useTorpedoLaserShotToggle.interactable = false;

									torpedoLaserShotCountToggle.interactable = false;

									//check if we have laser shot enabled - if we do, we need to turn it off
									if (useTorpedoLaserShotToggle.isOn == true) {

										useTorpedoLaserShotToggle.isOn = false;
										torpedoTargetingDropdown.interactable = false;

									}
									//the else condition is that we do not have a targeting device enabled
									else {

										torpedoTargetingDropdown.interactable = false;

									}

									//if the targeted unit is cloaked, we can allow the fire torpedos buttons to be pressed
									//but only if we actually have torpedos in inventory
									//check if we have light torpedos
									if (mouseMananger.selectedUnit.GetComponent<StarbaseTorpedoSection> ().lightTorpedos > 0) {

										//check if we are cloaked
										if (mouseMananger.selectedUnit.GetComponent<CloakingDevice> () == true &&
										    mouseMananger.selectedUnit.GetComponent<CloakingDevice> ().isCloaked == true) {

											//we are cloaked and cannot fire
											lightTorpedoFireButton.interactable = false;
											lightTorpedoCountToggle.isOn = false;
											lightTorpedoCountToggle.interactable = false;

										} else {

											lightTorpedoFireButton.interactable = true;
											lightTorpedoCountToggle.isOn = true;
											lightTorpedoCountToggle.interactable = true;

										}

									}
									//the else condition is that we do not have any light torpedos, and so the button can't be used
									else {

										lightTorpedoFireButton.interactable = false;
										lightTorpedoCountToggle.isOn = false;
										lightTorpedoCountToggle.interactable = false;

									}

									//check if we have heavy torpedos
									if (mouseMananger.selectedUnit.GetComponent<StarbaseTorpedoSection> ().heavyTorpedos > 0) {

										//check if we are cloaked
										if (mouseMananger.selectedUnit.GetComponent<CloakingDevice> () == true &&
										    mouseMananger.selectedUnit.GetComponent<CloakingDevice> ().isCloaked == true) {

											//we are cloaked and cannot fire
											heavyTorpedoFireButton.interactable = false;
											heavyTorpedoCountToggle.isOn = false;
											heavyTorpedoCountToggle.interactable = false;

										} else {

											heavyTorpedoFireButton.interactable = true;
											heavyTorpedoCountToggle.isOn = true;
											heavyTorpedoCountToggle.interactable = true;

										}

									}
									//the else condition is that we do not have any heavy torpedos, and so the button can't be used
									else {

										heavyTorpedoFireButton.interactable = false;
										heavyTorpedoCountToggle.isOn = false;
										heavyTorpedoCountToggle.interactable = false;

									}

								}

								//the else condition is that the targeted unit is not cloaked
								else{
									
									//check if the targeted unit is a ship.  if it is a base, we have to handle slightly differently
									if(mouseMananger.targetedUnit.GetComponent<Ship>() == true){

										//check if the targeted unit has a storage section
										if (mouseMananger.targetedUnit.GetComponent<StorageSection> () == true) {

											//now check if the targeted ship has laser scattering
											if (mouseMananger.targetedUnit.GetComponent<StorageSection> ().laserScatteringSystem == true) {

												//if it does, we can't allow using targeting
												useTorpedoLaserShotToggle.interactable = false;

												torpedoLaserShotCountToggle.interactable = false;

												//check if we have laser shot enabled - if we do, we need to turn it off
												if (useTorpedoLaserShotToggle.isOn == true) {

													useTorpedoLaserShotToggle.isOn = false;
													torpedoTargetingDropdown.interactable = false;

												}
												//the else condition is that we do not have a targeting device enabled
												else {

													torpedoTargetingDropdown.interactable = false;

												}

												//if the targeted ship has scattering, we can allow the fire torpedos buttons to be pressed
												//but only if we actually have torpedos in inventory
												//check if we have light torpedos
												if (mouseMananger.selectedUnit.GetComponent<StarbaseTorpedoSection> ().lightTorpedos > 0) {

													//check if we are cloaked
													if (mouseMananger.selectedUnit.GetComponent<CloakingDevice> () == true &&
														mouseMananger.selectedUnit.GetComponent<CloakingDevice> ().isCloaked == true) {

														//we are cloaked and cannot fire
														lightTorpedoFireButton.interactable = false;
														lightTorpedoCountToggle.isOn = false;
														lightTorpedoCountToggle.interactable = false;

													} else {

														lightTorpedoFireButton.interactable = true;
														lightTorpedoCountToggle.isOn = true;
														lightTorpedoCountToggle.interactable = true;

													}

												}
												//the else condition is that we do not have any light torpedos, and so the button can't be used
												else {

													lightTorpedoFireButton.interactable = false;
													lightTorpedoCountToggle.isOn = false;
													lightTorpedoCountToggle.interactable = false;

												}

												//check if we have heavy torpedos
												if (mouseMananger.selectedUnit.GetComponent<StarbaseTorpedoSection> ().heavyTorpedos > 0) {

													//check if we are cloaked
													if (mouseMananger.selectedUnit.GetComponent<CloakingDevice> () == true &&
														mouseMananger.selectedUnit.GetComponent<CloakingDevice> ().isCloaked == true) {

														//we are cloaked and cannot fire
														heavyTorpedoFireButton.interactable = false;
														heavyTorpedoCountToggle.isOn = false;
														heavyTorpedoCountToggle.interactable = false;

													} else {

														heavyTorpedoFireButton.interactable = true;
														heavyTorpedoCountToggle.isOn = true;
														heavyTorpedoCountToggle.interactable = true;

													}

												}
												//the else condition is that we do not have any heavy torpedos, and so the button can't be used
												else {

													heavyTorpedoFireButton.interactable = false;
													heavyTorpedoCountToggle.isOn = false;
													heavyTorpedoCountToggle.interactable = false;

												}

											}
											//the else condition is that there is no laser scattering on the targeted unit
											else {

												//we can now set the torpedo laser shot toggle based on inventory
												if (mouseMananger.selectedUnit.GetComponentInChildren<StarbaseTorpedoSection> ().torpedoLaserShot <= 0) {

													//if laser shot == 0, we have none and cannot use it
													useTorpedoLaserShotToggle.isOn = false;
													useTorpedoLaserShotToggle.interactable = false;

													torpedoLaserShotCountToggle.isOn = false;
													torpedoLaserShotCountToggle.interactable = false;

												}
												//the else condition is that the laser shot quantity is greater than zero
												else {

													//check if we have a laser guidance system
													if (mouseMananger.selectedUnit.GetComponentInChildren<StarbaseTorpedoSection> ().torpedoLaserGuidanceSystem == true) {

														//if we have the system, the shot can't be used
														useTorpedoLaserShotToggle.interactable = false;
														torpedoLaserShotCountToggle.interactable = false;

													} else {

														useTorpedoLaserShotToggle.interactable = true;
														torpedoLaserShotCountToggle.interactable = true;

													}

												}

												//check if we have targeting device enabled
												if (torpedoLaserGuidanceToggle.isOn == true || useTorpedoLaserShotToggle.isOn == true) {

													torpedoTargetingDropdown.interactable = true;

													//if we have targeting enabled, we can't allow the fire button to be pressed until a section has been chosen
													if (torpedoTargetingDropdown.GetComponentInChildren<TextMeshProUGUI>().text == "Choose Section") {

														lightTorpedoFireButton.interactable = false;
														lightTorpedoCountToggle.isOn = false;
														lightTorpedoCountToggle.interactable = false;

														heavyTorpedoFireButton.interactable = false;
														heavyTorpedoCountToggle.isOn = false;
														heavyTorpedoCountToggle.interactable = false;


													}
													//the else condition is that a section has been chosen, so we can use the fire button if we have inventory
													else {

														//check if we have light torpedos
														if (mouseMananger.selectedUnit.GetComponent<StarbaseTorpedoSection> ().lightTorpedos > 0) {

															//check if we are cloaked
															if (mouseMananger.selectedUnit.GetComponent<CloakingDevice> () == true &&
																mouseMananger.selectedUnit.GetComponent<CloakingDevice> ().isCloaked == true) {

																//we are cloaked and cannot fire
																lightTorpedoFireButton.interactable = false;
																lightTorpedoCountToggle.isOn = false;
																lightTorpedoCountToggle.interactable = false;

															} else {

																lightTorpedoFireButton.interactable = true;
																lightTorpedoCountToggle.isOn = true;
																lightTorpedoCountToggle.interactable = true;

															}

														}
														//the else condition is that we do not have any light torpedos, and so the button can't be used
														else {

															lightTorpedoFireButton.interactable = false;
															lightTorpedoCountToggle.isOn = false;
															lightTorpedoCountToggle.interactable = false;

														}

														//check if we have heavy torpedos
														if (mouseMananger.selectedUnit.GetComponent<StarbaseTorpedoSection> ().heavyTorpedos > 0) {

															//check if we are cloaked
															if (mouseMananger.selectedUnit.GetComponent<CloakingDevice> () == true &&
																mouseMananger.selectedUnit.GetComponent<CloakingDevice> ().isCloaked == true) {

																//we are cloaked and cannot fire
																heavyTorpedoFireButton.interactable = false;
																heavyTorpedoCountToggle.isOn = false;
																heavyTorpedoCountToggle.interactable = false;

															} else {

																heavyTorpedoFireButton.interactable = true;
																heavyTorpedoCountToggle.isOn = true;
																heavyTorpedoCountToggle.interactable = true;

															}

														}
														//the else condition is that we do not have any heavy torpedos, and so the button can't be used
														else {

															heavyTorpedoFireButton.interactable = false;
															heavyTorpedoCountToggle.isOn = false;
															heavyTorpedoCountToggle.interactable = false;

														}

													}

												}
												//the else condition is that we do not have a targeting device enabled
												else {

													torpedoTargetingDropdown.interactable = false;

													//we can fire torpedos if we have inventory

													//check if we have light torpedos
													if (mouseMananger.selectedUnit.GetComponent<StarbaseTorpedoSection> ().lightTorpedos > 0) {

														//check if we are cloaked
														if (mouseMananger.selectedUnit.GetComponent<CloakingDevice> () == true &&
															mouseMananger.selectedUnit.GetComponent<CloakingDevice> ().isCloaked == true) {

															//we are cloaked and cannot fire
															lightTorpedoFireButton.interactable = false;
															lightTorpedoCountToggle.isOn = false;
															lightTorpedoCountToggle.interactable = false;

														} else {

															lightTorpedoFireButton.interactable = true;
															lightTorpedoCountToggle.isOn = true;
															lightTorpedoCountToggle.interactable = true;

														}

													}
													//the else condition is that we do not have any light torpedos, and so the button can't be used
													else {

														lightTorpedoFireButton.interactable = false;
														lightTorpedoCountToggle.isOn = false;
														lightTorpedoCountToggle.interactable = false;

													}

													//check if we have heavy torpedos
													if (mouseMananger.selectedUnit.GetComponent<StarbaseTorpedoSection> ().heavyTorpedos > 0) {

														//check if we are cloaked
														if (mouseMananger.selectedUnit.GetComponent<CloakingDevice> () == true &&
															mouseMananger.selectedUnit.GetComponent<CloakingDevice> ().isCloaked == true) {

															//we are cloaked and cannot fire
															heavyTorpedoFireButton.interactable = false;
															heavyTorpedoCountToggle.isOn = false;
															heavyTorpedoCountToggle.interactable = false;

														} else {

															heavyTorpedoFireButton.interactable = true;
															heavyTorpedoCountToggle.isOn = true;
															heavyTorpedoCountToggle.interactable = true;

														}

													}
													//the else condition is that we do not have any heavy torpedos, and so the button can't be used
													else {

														heavyTorpedoFireButton.interactable = false;
														heavyTorpedoCountToggle.isOn = false;
														heavyTorpedoCountToggle.interactable = false;

													}

												}

											}

										}
										//the else condition is that the targeted ship does not have a storage section
										else {

											//if there is no storage section, there can be no laser scattering
											//we can now set the torpedo laser shot toggle based on inventory
											if (mouseMananger.selectedUnit.GetComponentInChildren<StarbaseTorpedoSection> ().torpedoLaserShot <= 0) {

												//if laser shot == 0, we have none and cannot use it
												useTorpedoLaserShotToggle.isOn = false;
												useTorpedoLaserShotToggle.interactable = false;

												torpedoLaserShotCountToggle.isOn = false;
												torpedoLaserShotCountToggle.interactable = false;

											}
											//the else condition is that the laser shot quantity is greater than zero
											else {

												//check if we have a laser guidance system
												if (mouseMananger.selectedUnit.GetComponentInChildren<StarbaseTorpedoSection> ().torpedoLaserGuidanceSystem == true) {

													//if we have the system, the shot can't be used
													useTorpedoLaserShotToggle.interactable = false;
													torpedoLaserShotCountToggle.interactable = false;

												} else {

													useTorpedoLaserShotToggle.interactable = true;
													torpedoLaserShotCountToggle.interactable = true;

												}

											}

											//check if we have targeting device enabled
											if (torpedoLaserGuidanceToggle.isOn == true || useTorpedoLaserShotToggle.isOn == true) {

												torpedoTargetingDropdown.interactable = true;

												//if we have targeting enabled, we can't allow the fire buttons to be pressed until a section has been chosen
												if (torpedoTargetingDropdown.GetComponentInChildren<TextMeshProUGUI>().text == "Choose Section") {

													lightTorpedoFireButton.interactable = false;
													lightTorpedoCountToggle.isOn = false;
													lightTorpedoCountToggle.interactable = false;

													heavyTorpedoFireButton.interactable = false;
													heavyTorpedoCountToggle.isOn = false;
													heavyTorpedoCountToggle.interactable = false;

												}
												//the else condition is that a section has been chosen, so we can use the fire buttons
												else {

													//check if we have torpedo inventory to allow firing
													//check if we have light torpedos
													if (mouseMananger.selectedUnit.GetComponent<StarbaseTorpedoSection> ().lightTorpedos > 0) {

														//check if we are cloaked
														if (mouseMananger.selectedUnit.GetComponent<CloakingDevice> () == true &&
															mouseMananger.selectedUnit.GetComponent<CloakingDevice> ().isCloaked == true) {

															//we are cloaked and cannot fire
															lightTorpedoFireButton.interactable = false;
															lightTorpedoCountToggle.isOn = false;
															lightTorpedoCountToggle.interactable = false;

														} else {

															lightTorpedoFireButton.interactable = true;
															lightTorpedoCountToggle.isOn = true;
															lightTorpedoCountToggle.interactable = true;

														}

													}
													//the else condition is that we do not have any light torpedos, and so the button can't be used
													else {

														lightTorpedoFireButton.interactable = false;
														lightTorpedoCountToggle.isOn = false;
														lightTorpedoCountToggle.interactable = false;

													}

													//check if we have heavy torpedos
													if (mouseMananger.selectedUnit.GetComponent<StarbaseTorpedoSection> ().heavyTorpedos > 0) {

														//check if we are cloaked
														if (mouseMananger.selectedUnit.GetComponent<CloakingDevice> () == true &&
															mouseMananger.selectedUnit.GetComponent<CloakingDevice> ().isCloaked == true) {

															//we are cloaked and cannot fire
															heavyTorpedoFireButton.interactable = false;
															heavyTorpedoCountToggle.isOn = false;
															heavyTorpedoCountToggle.interactable = false;

														} else {

															heavyTorpedoFireButton.interactable = true;
															heavyTorpedoCountToggle.isOn = true;
															heavyTorpedoCountToggle.interactable = true;

														}

													}
													//the else condition is that we do not have any heavy torpedos, and so the button can't be used
													else {

														heavyTorpedoFireButton.interactable = false;
														heavyTorpedoCountToggle.isOn = false;
														heavyTorpedoCountToggle.interactable = false;

													}

												}

											}
											//the else condition is that we do not have a targeting device enabled
											else {

												torpedoTargetingDropdown.interactable = false;

												//check if we have torpedo inventory to allow firing
												//check if we have light torpedos
												if (mouseMananger.selectedUnit.GetComponent<StarbaseTorpedoSection> ().lightTorpedos > 0) {

													//check if we are cloaked
													if (mouseMananger.selectedUnit.GetComponent<CloakingDevice> () == true &&
														mouseMananger.selectedUnit.GetComponent<CloakingDevice> ().isCloaked == true) {

														//we are cloaked and cannot fire
														lightTorpedoFireButton.interactable = false;
														lightTorpedoCountToggle.isOn = false;
														lightTorpedoCountToggle.interactable = false;

													} else {

														lightTorpedoFireButton.interactable = true;
														lightTorpedoCountToggle.isOn = true;
														lightTorpedoCountToggle.interactable = true;

													}

												}
												//the else condition is that we do not have any light torpedos, and so the button can't be used
												else {

													lightTorpedoFireButton.interactable = false;
													lightTorpedoCountToggle.isOn = false;
													lightTorpedoCountToggle.interactable = false;

												}

												//check if we have heavy torpedos
												if (mouseMananger.selectedUnit.GetComponent<StarbaseTorpedoSection> ().heavyTorpedos > 0) {

													//check if we are cloaked
													if (mouseMananger.selectedUnit.GetComponent<CloakingDevice> () == true &&
														mouseMananger.selectedUnit.GetComponent<CloakingDevice> ().isCloaked == true) {

														//we are cloaked and cannot fire
														heavyTorpedoFireButton.interactable = false;
														heavyTorpedoCountToggle.isOn = false;
														heavyTorpedoCountToggle.interactable = false;

													} else {

														heavyTorpedoFireButton.interactable = true;
														heavyTorpedoCountToggle.isOn = true;
														heavyTorpedoCountToggle.interactable = true;

													}

												}
												//the else condition is that we do not have any heavy torpedos, and so the button can't be used
												else {

													heavyTorpedoFireButton.interactable = false;
													heavyTorpedoCountToggle.isOn = false;
													heavyTorpedoCountToggle.interactable = false;

												}

											}

										}

									}
									//the else condition is that the targeted unit is a base
									else if (mouseMananger.targetedUnit.GetComponent<Starbase>() == true){

										//now check if the targeted starbse has laser scattering
										if (mouseMananger.targetedUnit.GetComponent<StarbaseStorageSection2> ().laserScatteringSystem == true) {

											//if it does, we can't allow using targeting
											useTorpedoLaserShotToggle.interactable = false;

											torpedoLaserShotCountToggle.interactable = false;

											//check if we have laser shot enabled - if we do, we need to turn it off
											if (useTorpedoLaserShotToggle.isOn == true) {

												useTorpedoLaserShotToggle.isOn = false;
												torpedoTargetingDropdown.interactable = false;

											}
											//the else condition is that we do not have a targeting device enabled
											else {

												torpedoTargetingDropdown.interactable = false;

											}

											//if the targeted unit has scattering, we can allow the fire torpedos buttons to be pressed
											//but only if we actually have torpedos in inventory
											//check if we have light torpedos
											if (mouseMananger.selectedUnit.GetComponent<StarbaseTorpedoSection> ().lightTorpedos > 0) {

												//check if we are cloaked
												if (mouseMananger.selectedUnit.GetComponent<CloakingDevice> () == true &&
													mouseMananger.selectedUnit.GetComponent<CloakingDevice> ().isCloaked == true) {

													//we are cloaked and cannot fire
													lightTorpedoFireButton.interactable = false;
													lightTorpedoCountToggle.isOn = false;
													lightTorpedoCountToggle.interactable = false;

												} else {

													lightTorpedoFireButton.interactable = true;
													lightTorpedoCountToggle.isOn = true;
													lightTorpedoCountToggle.interactable = true;

												}

											}
											//the else condition is that we do not have any light torpedos, and so the button can't be used
											else {

												lightTorpedoFireButton.interactable = false;
												lightTorpedoCountToggle.isOn = false;
												lightTorpedoCountToggle.interactable = false;

											}

											//check if we have heavy torpedos
											if (mouseMananger.selectedUnit.GetComponent<StarbaseTorpedoSection> ().heavyTorpedos > 0) {

												//check if we are cloaked
												if (mouseMananger.selectedUnit.GetComponent<CloakingDevice> () == true &&
													mouseMananger.selectedUnit.GetComponent<CloakingDevice> ().isCloaked == true) {

													//we are cloaked and cannot fire
													heavyTorpedoFireButton.interactable = false;
													heavyTorpedoCountToggle.isOn = false;
													heavyTorpedoCountToggle.interactable = false;

												} else {

													heavyTorpedoFireButton.interactable = true;
													heavyTorpedoCountToggle.isOn = true;
													heavyTorpedoCountToggle.interactable = true;

												}

											}
											//the else condition is that we do not have any heavy torpedos, and so the button can't be used
											else {

												heavyTorpedoFireButton.interactable = false;
												heavyTorpedoCountToggle.isOn = false;
												heavyTorpedoCountToggle.interactable = false;

											}

										}
										//the else condition is that there is no laser scattering on the targeted unit
										else {

											//we can now set the torpedo laser shot toggle based on inventory
											if (mouseMananger.selectedUnit.GetComponentInChildren<StarbaseTorpedoSection> ().torpedoLaserShot <= 0) {

												//if laser shot == 0, we have none and cannot use it
												useTorpedoLaserShotToggle.isOn = false;
												useTorpedoLaserShotToggle.interactable = false;

												torpedoLaserShotCountToggle.isOn = false;
												torpedoLaserShotCountToggle.interactable = false;

											}
											//the else condition is that the laser shot quantity is greater than zero
											else {

												//check if we have a laser guidance system
												if (mouseMananger.selectedUnit.GetComponentInChildren<StarbaseTorpedoSection> ().torpedoLaserGuidanceSystem == true) {

													//if we have the system, the shot can't be used
													useTorpedoLaserShotToggle.interactable = false;
													torpedoLaserShotCountToggle.interactable = false;

												} else {

													useTorpedoLaserShotToggle.interactable = true;
													torpedoLaserShotCountToggle.interactable = true;

												}

											}

											//check if we have targeting device enabled
											if (torpedoLaserGuidanceToggle.isOn == true || useTorpedoLaserShotToggle.isOn == true) {

												torpedoTargetingDropdown.interactable = true;

												//if we have targeting enabled, we can't allow the fire button to be pressed until a section has been chosen
												if (torpedoTargetingDropdown.GetComponentInChildren<TextMeshProUGUI>().text == "Choose Section") {

													lightTorpedoFireButton.interactable = false;
													lightTorpedoCountToggle.isOn = false;
													lightTorpedoCountToggle.interactable = false;

													heavyTorpedoFireButton.interactable = false;
													heavyTorpedoCountToggle.isOn = false;
													heavyTorpedoCountToggle.interactable = false;

												}
												//the else condition is that a section has been chosen, so we can use the fire button if we have inventory
												else {

													//check if we have light torpedos
													if (mouseMananger.selectedUnit.GetComponent<StarbaseTorpedoSection> ().lightTorpedos > 0) {

														//check if we are cloaked
														if (mouseMananger.selectedUnit.GetComponent<CloakingDevice> () == true &&
															mouseMananger.selectedUnit.GetComponent<CloakingDevice> ().isCloaked == true) {

															//we are cloaked and cannot fire
															lightTorpedoFireButton.interactable = false;
															lightTorpedoCountToggle.isOn = false;
															lightTorpedoCountToggle.interactable = false;

														} else {

															lightTorpedoFireButton.interactable = true;
															lightTorpedoCountToggle.isOn = true;
															lightTorpedoCountToggle.interactable = true;

														}

													}
													//the else condition is that we do not have any light torpedos, and so the button can't be used
													else {

														lightTorpedoFireButton.interactable = false;
														lightTorpedoCountToggle.isOn = false;
														lightTorpedoCountToggle.interactable = false;

													}

													//check if we have heavy torpedos
													if (mouseMananger.selectedUnit.GetComponent<StarbaseTorpedoSection> ().heavyTorpedos > 0) {

														//check if we are cloaked
														if (mouseMananger.selectedUnit.GetComponent<CloakingDevice> () == true &&
															mouseMananger.selectedUnit.GetComponent<CloakingDevice> ().isCloaked == true) {

															//we are cloaked and cannot fire
															heavyTorpedoFireButton.interactable = false;
															heavyTorpedoCountToggle.isOn = false;
															heavyTorpedoCountToggle.interactable = false;

														} else {

															heavyTorpedoFireButton.interactable = true;
															heavyTorpedoCountToggle.isOn = true;
															heavyTorpedoCountToggle.interactable = true;

														}

													}
													//the else condition is that we do not have any heavy torpedos, and so the button can't be used
													else {

														heavyTorpedoFireButton.interactable = false;
														heavyTorpedoCountToggle.isOn = false;
														heavyTorpedoCountToggle.interactable = false;

													}

												}

											}
											//the else condition is that we do not have a targeting device enabled
											else {

												torpedoTargetingDropdown.interactable = false;

												//we can fire torpedos if we have inventory

												//check if we have light torpedos
												if (mouseMananger.selectedUnit.GetComponent<StarbaseTorpedoSection> ().lightTorpedos > 0) {

													//check if we are cloaked
													if (mouseMananger.selectedUnit.GetComponent<CloakingDevice> () == true &&
														mouseMananger.selectedUnit.GetComponent<CloakingDevice> ().isCloaked == true) {

														//we are cloaked and cannot fire
														lightTorpedoFireButton.interactable = false;
														lightTorpedoCountToggle.isOn = false;
														lightTorpedoCountToggle.interactable = false;

													} else {

														lightTorpedoFireButton.interactable = true;
														lightTorpedoCountToggle.isOn = true;
														lightTorpedoCountToggle.interactable = true;

													}

												}
												//the else condition is that we do not have any light torpedos, and so the button can't be used
												else {

													lightTorpedoFireButton.interactable = false;
													lightTorpedoCountToggle.isOn = false;
													lightTorpedoCountToggle.interactable = false;

												}

												//check if we have heavy torpedos
												if (mouseMananger.selectedUnit.GetComponent<StarbaseTorpedoSection> ().heavyTorpedos > 0) {

													//check if we are cloaked
													if (mouseMananger.selectedUnit.GetComponent<CloakingDevice> () == true &&
														mouseMananger.selectedUnit.GetComponent<CloakingDevice> ().isCloaked == true) {

														//we are cloaked and cannot fire
														heavyTorpedoFireButton.interactable = false;
														heavyTorpedoCountToggle.isOn = false;
														heavyTorpedoCountToggle.interactable = false;

													} else {

														heavyTorpedoFireButton.interactable = true;
														heavyTorpedoCountToggle.isOn = true;
														heavyTorpedoCountToggle.interactable = true;

													}

												}
												//the else condition is that we do not have any heavy torpedos, and so the button can't be used
												else {

													heavyTorpedoFireButton.interactable = false;
													heavyTorpedoCountToggle.isOn = false;
													heavyTorpedoCountToggle.interactable = false;

												}

											}  //end else we do not have targeting device enabled

										} //end else there is no laser scattering on targeted unit

									}  //end else the targeted unit is a base
								}  //end else the targeted unit is not cloaked
							} // end else we have a targeted unit

						}  //end else we have not used torpedos yet

					}  //end else the torpedo section is not destroyed

				} //end if the selected unit is a starbase

				//the else condition is that we do have a ship, so we want to allow toggles based on the ship inventory
				else {

					//check to see if the ship has a torpedo section.  If not, it can't use torpedo attacks
					if (mouseMananger.selectedUnit.GetComponentInChildren<TorpedoSection> () == null) {

						useTorpedoLaserShotToggle.isOn = false;
						useTorpedoLaserShotToggle.interactable = false;

						torpedoLaserShotCountToggle.isOn = false;
						torpedoLaserShotCountToggle.interactable = false;

						torpedoLaserGuidanceToggle.isOn = false;
						torpedoLaserGuidanceToggle.interactable = false;

						torpedoHighPressureTubesToggle.isOn = false;
						torpedoHighPressureTubesToggle.interactable = false;



						torpedoTargetingDropdown.interactable = false;

						lightTorpedoFireButton.interactable = false;
						lightTorpedoCountToggle.isOn = false;
						lightTorpedoCountToggle.interactable = false;

						heavyTorpedoFireButton.interactable = false;
						heavyTorpedoCountToggle.isOn = false;
						heavyTorpedoCountToggle.interactable = false;

					}

					//the else condition is that there is a torpedo section in the selected unit
					else {

						//now we need to make sure the torpedo section hasn't been destroyed
						if (mouseMananger.selectedUnit.GetComponentInChildren<TorpedoSection> ().isDestroyed == true) {

							//if it is destroyed, we can't use torpedo commands
							useTorpedoLaserShotToggle.isOn = false;
							useTorpedoLaserShotToggle.interactable = false;

							torpedoLaserShotCountToggle.isOn = false;
							torpedoLaserShotCountToggle.interactable = false;

							torpedoLaserGuidanceToggle.isOn = false;
							torpedoLaserGuidanceToggle.interactable = false;

							torpedoHighPressureTubesToggle.isOn = false;
							torpedoHighPressureTubesToggle.interactable = false;



							torpedoTargetingDropdown.interactable = false;

							lightTorpedoFireButton.interactable = false;
							lightTorpedoCountToggle.isOn = false;
							lightTorpedoCountToggle.interactable = false;

							heavyTorpedoFireButton.interactable = false;
							heavyTorpedoCountToggle.isOn = false;
							heavyTorpedoCountToggle.interactable = false;

						}

						//the else condition is that the torpedo section is not destroyed.  
						else {

							//set the counts based on ship inventory
							torpedoLaserShotCountToggle.GetComponentInChildren<TextMeshProUGUI>().text = mouseMananger.selectedUnit.GetComponentInChildren<TorpedoSection> ().torpedoLaserShot.ToString();
							lightTorpedoCountToggle.GetComponentInChildren<TextMeshProUGUI>().text = mouseMananger.selectedUnit.GetComponentInChildren<TorpedoSection> ().lightTorpedos.ToString();
							heavyTorpedoCountToggle.GetComponentInChildren<TextMeshProUGUI>().text = mouseMananger.selectedUnit.GetComponentInChildren<TorpedoSection> ().heavyTorpedos.ToString();


							//if the ship has the high pressure tubes upgrade, set that toggle to on
							if (mouseMananger.selectedUnit.GetComponentInChildren<TorpedoSection> ().highPressureTubes == true) {

								torpedoHighPressureTubesToggle.isOn = true;
								torpedoHighPressureTubesToggle.interactable = false;

							}

							//if the ship has the laser guidance upgrade, set that toggle to on and disable laser shot single use items
							if (mouseMananger.selectedUnit.GetComponentInChildren<TorpedoSection> ().torpedoLaserGuidanceSystem == true) {

								torpedoLaserGuidanceToggle.isOn = true;
								torpedoLaserGuidanceToggle.interactable = false;

								useTorpedoLaserShotToggle.isOn = false;
								useTorpedoLaserShotToggle.interactable = false;

								torpedoLaserShotCountToggle.isOn = false;
								torpedoLaserShotCountToggle.interactable = false;

							}

							//now we need to check to see if we've already used torpedos this turn
							if (mouseMananger.selectedUnit.GetComponentInChildren<TorpedoSection> ().usedTorpedosThisTurn == true) {

								//if we've already used torpedos, we can't use a laser shot or use the fire buttons
								useTorpedoLaserShotToggle.isOn = false;
								useTorpedoLaserShotToggle.interactable = false;

								torpedoLaserShotCountToggle.isOn = false;
								torpedoLaserShotCountToggle.interactable = false;

								torpedoTargetingDropdown.interactable = false;

								lightTorpedoFireButton.interactable = false;
								lightTorpedoCountToggle.isOn = false;
								lightTorpedoCountToggle.interactable = false;

								heavyTorpedoFireButton.interactable = false;
								heavyTorpedoCountToggle.isOn = false;
								heavyTorpedoCountToggle.interactable = false;

							}
							//check if we've used phasors and don't have an additional battle crew - we only get 1 attack per turn without the crew
							else if (mouseMananger.selectedUnit.GetComponent<PhasorSection> () == true &&
								mouseMananger.selectedUnit.GetComponent<PhasorSection> ().usedPhasorsThisTurn == true &&

								(mouseMananger.selectedUnit.GetComponent<CrewSection> () == false ||

								(mouseMananger.selectedUnit.GetComponent<CrewSection> () == true &&
										mouseMananger.selectedUnit.GetComponent<CrewSection> ().battleCrew == false))) {

								//if we've already used phasors and don't have an additional battle crew, we can't use a laser shot or use the fire button
								useTorpedoLaserShotToggle.isOn = false;
								useTorpedoLaserShotToggle.interactable = false;

								torpedoLaserShotCountToggle.isOn = false;
								torpedoLaserShotCountToggle.interactable = false;

								torpedoTargetingDropdown.interactable = false;

								lightTorpedoFireButton.interactable = false;
								lightTorpedoCountToggle.isOn = false;
								lightTorpedoCountToggle.interactable = false;

								heavyTorpedoFireButton.interactable = false;
								heavyTorpedoCountToggle.isOn = false;
								heavyTorpedoCountToggle.interactable = false;

							}
							//the else condition is that we have not used torpedos yet
							else {

								//we can now set the torpedo laser shot toggle based on inventory
								if (mouseMananger.selectedUnit.GetComponentInChildren<TorpedoSection> ().torpedoLaserShot <= 0) {

									//if laser shot == 0, we have none and cannot use it
									useTorpedoLaserShotToggle.isOn = false;
									useTorpedoLaserShotToggle.interactable = false;

									torpedoLaserShotCountToggle.isOn = false;
									torpedoLaserShotCountToggle.interactable = false;

								}
								//the else condition is that the radar shot quantity is greater than zero
								else {

									//check if we have a laser guidance system
									if (mouseMananger.selectedUnit.GetComponentInChildren<TorpedoSection> ().torpedoLaserGuidanceSystem == true) {

										//if we have the system, the shot can't be used
										useTorpedoLaserShotToggle.interactable = false;
										torpedoLaserShotCountToggle.interactable = false;

									} else {

										useTorpedoLaserShotToggle.interactable = true;
										torpedoLaserShotCountToggle.interactable = true;

									}

								}

								//now we need to check to see if we have a unit targeted
								if (mouseMananger.targetedUnit == null) {

									//if there is no targeted unit, we can't fire torpedos
									torpedoTargetingDropdown.interactable = false;

									lightTorpedoFireButton.interactable = false;
									lightTorpedoCountToggle.isOn = false;
									lightTorpedoCountToggle.interactable = false;

									heavyTorpedoFireButton.interactable = false;
									heavyTorpedoCountToggle.isOn = false;
									heavyTorpedoCountToggle.interactable = false;

								}

								//the else condition is that we do have a unit targeted
								else {


									//check if the targeted unit has a cloaking device turned on
									if (mouseMananger.targetedUnit.GetComponent<CloakingDevice> () && mouseMananger.targetedUnit.GetComponent<CloakingDevice> ().isCloaked == true) {

										//if it is cloaked, we can't allow using targeting
										useTorpedoLaserShotToggle.interactable = false;

										torpedoLaserShotCountToggle.interactable = false;

										//check if we have laser shot enabled - if we do, we need to turn it off
										if (useTorpedoLaserShotToggle.isOn == true) {

											useTorpedoLaserShotToggle.isOn = false;
											torpedoTargetingDropdown.interactable = false;

										}
										//the else condition is that we do not have a targeting device enabled
										else {

											torpedoTargetingDropdown.interactable = false;

										}

										//if the targeted unit is cloaked, we can allow the fire torpedos buttons to be pressed
										//but only if we actually have torpedos in inventory
										//check if we have light torpedos
										if (mouseMananger.selectedUnit.GetComponent<TorpedoSection> ().lightTorpedos > 0) {

											//check if we are cloaked
											if (mouseMananger.selectedUnit.GetComponent<CloakingDevice> () == true &&
												mouseMananger.selectedUnit.GetComponent<CloakingDevice> ().isCloaked == true) {

												//we are cloaked and cannot fire
												lightTorpedoFireButton.interactable = false;
												lightTorpedoCountToggle.isOn = false;
												lightTorpedoCountToggle.interactable = false;

											} else {

												lightTorpedoFireButton.interactable = true;
												lightTorpedoCountToggle.isOn = true;
												lightTorpedoCountToggle.interactable = true;

											}

										}
										//the else condition is that we do not have any light torpedos, and so the button can't be used
										else {

											lightTorpedoFireButton.interactable = false;
											lightTorpedoCountToggle.isOn = false;
											lightTorpedoCountToggle.interactable = false;

										}

										//check if we have heavy torpedos
										if (mouseMananger.selectedUnit.GetComponent<TorpedoSection> ().heavyTorpedos > 0) {

											//check if we are cloaked
											if (mouseMananger.selectedUnit.GetComponent<CloakingDevice> () == true &&
												mouseMananger.selectedUnit.GetComponent<CloakingDevice> ().isCloaked == true) {

												//we are cloaked and cannot fire
												heavyTorpedoFireButton.interactable = false;
												heavyTorpedoCountToggle.isOn = false;
												heavyTorpedoCountToggle.interactable = false;

											} else {

												heavyTorpedoFireButton.interactable = true;
												heavyTorpedoCountToggle.isOn = true;
												heavyTorpedoCountToggle.interactable = true;

											}

										}
										//the else condition is that we do not have any heavy torpedos, and so the button can't be used
										else {

											heavyTorpedoFireButton.interactable = false;
											heavyTorpedoCountToggle.isOn = false;
											heavyTorpedoCountToggle.interactable = false;

										}

									}

									//the else condition is that the targeted unit is not cloaked
									else{

										//check if the targeted unit is a ship.  if it is a base, we have to handle slightly differently
										if(mouseMananger.targetedUnit.GetComponent<Ship>() == true){

											//check if the targeted unit has a storage section
											if (mouseMananger.targetedUnit.GetComponent<StorageSection> () == true) {

												//now check if the targeted ship has laser scattering
												if (mouseMananger.targetedUnit.GetComponent<StorageSection> ().laserScatteringSystem == true) {

													//if it does, we can't allow using targeting
													useTorpedoLaserShotToggle.interactable = false;

													torpedoLaserShotCountToggle.interactable = false;

													//check if we have laser shot enabled - if we do, we need to turn it off
													if (useTorpedoLaserShotToggle.isOn == true) {

														useTorpedoLaserShotToggle.isOn = false;
														torpedoTargetingDropdown.interactable = false;

													}
													//the else condition is that we do not have a targeting device enabled
													else {

														torpedoTargetingDropdown.interactable = false;

													}

													//if the targeted ship has scattering, we can allow the fire torpedos buttons to be pressed
													//but only if we actually have torpedos in inventory
													//check if we have light torpedos
													if (mouseMananger.selectedUnit.GetComponent<TorpedoSection> ().lightTorpedos > 0) {

														//check if we are cloaked
														if (mouseMananger.selectedUnit.GetComponent<CloakingDevice> () == true &&
															mouseMananger.selectedUnit.GetComponent<CloakingDevice> ().isCloaked == true) {

															//we are cloaked and cannot fire
															lightTorpedoFireButton.interactable = false;
															lightTorpedoCountToggle.isOn = false;
															lightTorpedoCountToggle.interactable = false;

														} else {

															lightTorpedoFireButton.interactable = true;
															lightTorpedoCountToggle.isOn = true;
															lightTorpedoCountToggle.interactable = true;

														}

													}
													//the else condition is that we do not have any light torpedos, and so the button can't be used
													else {

														lightTorpedoFireButton.interactable = false;
														lightTorpedoCountToggle.isOn = false;
														lightTorpedoCountToggle.interactable = false;

													}

													//check if we have heavy torpedos
													if (mouseMananger.selectedUnit.GetComponent<TorpedoSection> ().heavyTorpedos > 0) {

														//check if we are cloaked
														if (mouseMananger.selectedUnit.GetComponent<CloakingDevice> () == true &&
															mouseMananger.selectedUnit.GetComponent<CloakingDevice> ().isCloaked == true) {

															//we are cloaked and cannot fire
															heavyTorpedoFireButton.interactable = false;
															heavyTorpedoCountToggle.isOn = false;
															heavyTorpedoCountToggle.interactable = false;

														} else {

															heavyTorpedoFireButton.interactable = true;
															heavyTorpedoCountToggle.isOn = true;
															heavyTorpedoCountToggle.interactable = true;

														}

													}
													//the else condition is that we do not have any heavy torpedos, and so the button can't be used
													else {

														heavyTorpedoFireButton.interactable = false;
														heavyTorpedoCountToggle.isOn = false;
														heavyTorpedoCountToggle.interactable = false;

													}

												}
												//the else condition is that there is no laser scattering on the targeted unit
												else {

													//we can now set the torpedo laser shot toggle based on inventory
													if (mouseMananger.selectedUnit.GetComponentInChildren<TorpedoSection> ().torpedoLaserShot <= 0) {

														//if laser shot == 0, we have none and cannot use it
														useTorpedoLaserShotToggle.isOn = false;
														useTorpedoLaserShotToggle.interactable = false;

														torpedoLaserShotCountToggle.isOn = false;
														torpedoLaserShotCountToggle.interactable = false;

													}
													//the else condition is that the radar shot quantity is greater than zero
													else {

														//check if we have a laser guidance system
														if (mouseMananger.selectedUnit.GetComponentInChildren<TorpedoSection> ().torpedoLaserGuidanceSystem == true) {

															//if we have the system, the shot can't be used
															useTorpedoLaserShotToggle.interactable = false;
															torpedoLaserShotCountToggle.interactable = false;

														} else {

															useTorpedoLaserShotToggle.interactable = true;
															torpedoLaserShotCountToggle.interactable = true;

														}

													}

													//check if we have targeting device enabled
													if (torpedoLaserGuidanceToggle.isOn == true || useTorpedoLaserShotToggle.isOn == true) {

														torpedoTargetingDropdown.interactable = true;

														//if we have targeting enabled, we can't allow the fire button to be pressed until a section has been chosen
														if (torpedoTargetingDropdown.GetComponentInChildren<TextMeshProUGUI>().text == "Choose Section") {

															lightTorpedoFireButton.interactable = false;
															lightTorpedoCountToggle.isOn = false;
															lightTorpedoCountToggle.interactable = false;

															heavyTorpedoFireButton.interactable = false;
															heavyTorpedoCountToggle.isOn = false;
															heavyTorpedoCountToggle.interactable = false;

														}
														//the else condition is that a section has been chosen, so we can use the fire button if we have inventory
														else {

															//check if we have light torpedos
															if (mouseMananger.selectedUnit.GetComponent<TorpedoSection> ().lightTorpedos > 0) {

																//check if we are cloaked
																if (mouseMananger.selectedUnit.GetComponent<CloakingDevice> () == true &&
																	mouseMananger.selectedUnit.GetComponent<CloakingDevice> ().isCloaked == true) {

																	//we are cloaked and cannot fire
																	lightTorpedoFireButton.interactable = false;
																	lightTorpedoCountToggle.isOn = false;
																	lightTorpedoCountToggle.interactable = false;

																} else {

																	lightTorpedoFireButton.interactable = true;
																	lightTorpedoCountToggle.isOn = true;
																	lightTorpedoCountToggle.interactable = true;

																}

															}
															//the else condition is that we do not have any light torpedos, and so the button can't be used
															else {

																lightTorpedoFireButton.interactable = false;
																lightTorpedoCountToggle.isOn = false;
																lightTorpedoCountToggle.interactable = false;

															}

															//check if we have heavy torpedos
															if (mouseMananger.selectedUnit.GetComponent<TorpedoSection> ().heavyTorpedos > 0) {

																//check if we are cloaked
																if (mouseMananger.selectedUnit.GetComponent<CloakingDevice> () == true &&
																	mouseMananger.selectedUnit.GetComponent<CloakingDevice> ().isCloaked == true) {

																	//we are cloaked and cannot fire
																	heavyTorpedoFireButton.interactable = false;
																	heavyTorpedoCountToggle.isOn = false;
																	heavyTorpedoCountToggle.interactable = false;

																} else {

																	heavyTorpedoFireButton.interactable = true;
																	heavyTorpedoCountToggle.isOn = true;
																	heavyTorpedoCountToggle.interactable = true;

																}

															}
															//the else condition is that we do not have any heavy torpedos, and so the button can't be used
															else {

																heavyTorpedoFireButton.interactable = false;
																heavyTorpedoCountToggle.isOn = false;
																heavyTorpedoCountToggle.interactable = false;

															}

														}
											
													}
													//the else condition is that we do not have a targeting device enabled
													else {

														torpedoTargetingDropdown.interactable = false;

														//we can fire torpedos if we have inventory

														//check if we have light torpedos
														if (mouseMananger.selectedUnit.GetComponent<TorpedoSection> ().lightTorpedos > 0) {

															//check if we are cloaked
															if (mouseMananger.selectedUnit.GetComponent<CloakingDevice> () == true &&
																mouseMananger.selectedUnit.GetComponent<CloakingDevice> ().isCloaked == true) {

																//we are cloaked and cannot fire
																lightTorpedoFireButton.interactable = false;
																lightTorpedoCountToggle.isOn = false;
																lightTorpedoCountToggle.interactable = false;

															} else {

																lightTorpedoFireButton.interactable = true;
																lightTorpedoCountToggle.isOn = true;
																lightTorpedoCountToggle.interactable = true;

															}

														}
														//the else condition is that we do not have any light torpedos, and so the button can't be used
														else {

															lightTorpedoFireButton.interactable = false;
															lightTorpedoCountToggle.isOn = false;
															lightTorpedoCountToggle.interactable = false;

														}

														//check if we have heavy torpedos
														if (mouseMananger.selectedUnit.GetComponent<TorpedoSection> ().heavyTorpedos > 0) {

															//check if we are cloaked
															if (mouseMananger.selectedUnit.GetComponent<CloakingDevice> () == true &&
																mouseMananger.selectedUnit.GetComponent<CloakingDevice> ().isCloaked == true) {

																//we are cloaked and cannot fire
																heavyTorpedoFireButton.interactable = false;
																heavyTorpedoCountToggle.isOn = false;
																heavyTorpedoCountToggle.interactable = false;

															} else {

																heavyTorpedoFireButton.interactable = true;
																heavyTorpedoCountToggle.isOn = true;
																heavyTorpedoCountToggle.interactable = true;

															}

														}
														//the else condition is that we do not have any heavy torpedos, and so the button can't be used
														else {

															heavyTorpedoFireButton.interactable = false;
															heavyTorpedoCountToggle.isOn = false;
															heavyTorpedoCountToggle.interactable = false;

														}

													}

												}

											}
											//the else condition is that the targeted ship does not have a storage section
											else {

												//if there is no storage section, there can be no laser scattering
												//we can now set the torpedo laser shot toggle based on inventory
												if (mouseMananger.selectedUnit.GetComponentInChildren<TorpedoSection> ().torpedoLaserShot <= 0) {

													//if laser shot == 0, we have none and cannot use it
													useTorpedoLaserShotToggle.isOn = false;
													useTorpedoLaserShotToggle.interactable = false;

													torpedoLaserShotCountToggle.isOn = false;
													torpedoLaserShotCountToggle.interactable = false;

												}
												//the else condition is that the radar shot quantity is greater than zero
												else {

													//check if we have a laser guidance system
													if (mouseMananger.selectedUnit.GetComponentInChildren<TorpedoSection> ().torpedoLaserGuidanceSystem == true) {

														//if we have the system, the shot can't be used
														useTorpedoLaserShotToggle.interactable = false;
														torpedoLaserShotCountToggle.interactable = false;

													} else {

														useTorpedoLaserShotToggle.interactable = true;
														torpedoLaserShotCountToggle.interactable = true;

													}

												}

												//check if we have targeting device enabled
												if (torpedoLaserGuidanceToggle.isOn == true || useTorpedoLaserShotToggle.isOn == true) {

													torpedoTargetingDropdown.interactable = true;

													//if we have targeting enabled, we can't allow the fire buttons to be pressed until a section has been chosen
													if (torpedoTargetingDropdown.GetComponentInChildren<TextMeshProUGUI>().text == "Choose Section") {

														lightTorpedoFireButton.interactable = false;
														lightTorpedoCountToggle.isOn = false;
														lightTorpedoCountToggle.interactable = false;

														heavyTorpedoFireButton.interactable = false;
														heavyTorpedoCountToggle.isOn = false;
														heavyTorpedoCountToggle.interactable = false;

													}
													//the else condition is that a section has been chosen, so we can use the fire buttons
													else {

														//check if we have torpedo inventory to allow firing
														//check if we have light torpedos
														if (mouseMananger.selectedUnit.GetComponent<TorpedoSection> ().lightTorpedos > 0) {

															//check if we are cloaked
															if (mouseMananger.selectedUnit.GetComponent<CloakingDevice> () == true &&
																mouseMananger.selectedUnit.GetComponent<CloakingDevice> ().isCloaked == true) {

																//we are cloaked and cannot fire
																lightTorpedoFireButton.interactable = false;
																lightTorpedoCountToggle.isOn = false;
																lightTorpedoCountToggle.interactable = false;

															} else {

																lightTorpedoFireButton.interactable = true;
																lightTorpedoCountToggle.isOn = true;
																lightTorpedoCountToggle.interactable = true;

															}

														}
														//the else condition is that we do not have any light torpedos, and so the button can't be used
														else {

															lightTorpedoFireButton.interactable = false;
															lightTorpedoCountToggle.isOn = false;
															lightTorpedoCountToggle.interactable = false;

														}

														//check if we have heavy torpedos
														if (mouseMananger.selectedUnit.GetComponent<TorpedoSection> ().heavyTorpedos > 0) {

															//check if we are cloaked
															if (mouseMananger.selectedUnit.GetComponent<CloakingDevice> () == true &&
																mouseMananger.selectedUnit.GetComponent<CloakingDevice> ().isCloaked == true) {

																//we are cloaked and cannot fire
																heavyTorpedoFireButton.interactable = false;
																heavyTorpedoCountToggle.isOn = false;
																heavyTorpedoCountToggle.interactable = false;

															} else {

																heavyTorpedoFireButton.interactable = true;
																heavyTorpedoCountToggle.isOn = true;
																heavyTorpedoCountToggle.interactable = true;

															}

														}
														//the else condition is that we do not have any heavy torpedos, and so the button can't be used
														else {

															heavyTorpedoFireButton.interactable = false;
															heavyTorpedoCountToggle.isOn = false;
															heavyTorpedoCountToggle.interactable = false;

														}

													}

												}
												//the else condition is that we do not have a targeting device enabled
												else {

													torpedoTargetingDropdown.interactable = false;

													//check if we have torpedo inventory to allow firing
													//check if we have light torpedos
													if (mouseMananger.selectedUnit.GetComponent<TorpedoSection> ().lightTorpedos > 0) {

														//check if we are cloaked
														if (mouseMananger.selectedUnit.GetComponent<CloakingDevice> () == true &&
															mouseMananger.selectedUnit.GetComponent<CloakingDevice> ().isCloaked == true) {

															//we are cloaked and cannot fire
															lightTorpedoFireButton.interactable = false;
															lightTorpedoCountToggle.isOn = false;
															lightTorpedoCountToggle.interactable = false;

														} else {

															lightTorpedoFireButton.interactable = true;
															lightTorpedoCountToggle.isOn = true;
															lightTorpedoCountToggle.interactable = true;

														}

													}
													//the else condition is that we do not have any light torpedos, and so the button can't be used
													else {

														lightTorpedoFireButton.interactable = false;
														lightTorpedoCountToggle.isOn = false;
														lightTorpedoCountToggle.interactable = false;

													}

													//check if we have heavy torpedos
													if (mouseMananger.selectedUnit.GetComponent<TorpedoSection> ().heavyTorpedos > 0) {

														//check if we are cloaked
														if (mouseMananger.selectedUnit.GetComponent<CloakingDevice> () == true &&
															mouseMananger.selectedUnit.GetComponent<CloakingDevice> ().isCloaked == true) {

															//we are cloaked and cannot fire
															heavyTorpedoFireButton.interactable = false;
															heavyTorpedoCountToggle.isOn = false;
															heavyTorpedoCountToggle.interactable = false;

														} else {

															heavyTorpedoFireButton.interactable = true;
															heavyTorpedoCountToggle.isOn = true;
															heavyTorpedoCountToggle.interactable = true;

														}

													}
													//the else condition is that we do not have any heavy torpedos, and so the button can't be used
													else {

														heavyTorpedoFireButton.interactable = false;
														heavyTorpedoCountToggle.isOn = false;
														heavyTorpedoCountToggle.interactable = false;

													}

												}

											}

										}
										//the else condition is that the targeted unit is a base
										else if (mouseMananger.targetedUnit.GetComponent<Starbase>() == true){

											//now check if the targeted starbse has laser scattering
											if (mouseMananger.targetedUnit.GetComponent<StarbaseStorageSection2> ().laserScatteringSystem == true) {

												//if it does, we can't allow using targeting
												useTorpedoLaserShotToggle.interactable = false;

												torpedoLaserShotCountToggle.interactable = false;

												//check if we have laser shot enabled - if we do, we need to turn it off
												if (useTorpedoLaserShotToggle.isOn == true) {

													useTorpedoLaserShotToggle.isOn = false;
													torpedoTargetingDropdown.interactable = false;

												}
												//the else condition is that we do not have a targeting device enabled
												else {

													torpedoTargetingDropdown.interactable = false;

												}

												//if the targeted unit has scattering, we can allow the fire torpedos buttons to be pressed
												//but only if we actually have torpedos in inventory
												//check if we have light torpedos
												if (mouseMananger.selectedUnit.GetComponent<TorpedoSection> ().lightTorpedos > 0) {

													//check if we are cloaked
													if (mouseMananger.selectedUnit.GetComponent<CloakingDevice> () == true &&
														mouseMananger.selectedUnit.GetComponent<CloakingDevice> ().isCloaked == true) {

														//we are cloaked and cannot fire
														lightTorpedoFireButton.interactable = false;
														lightTorpedoCountToggle.isOn = false;
														lightTorpedoCountToggle.interactable = false;

													} else {

														lightTorpedoFireButton.interactable = true;
														lightTorpedoCountToggle.isOn = true;
														lightTorpedoCountToggle.interactable = true;

													}

												}
												//the else condition is that we do not have any light torpedos, and so the button can't be used
												else {

													lightTorpedoFireButton.interactable = false;
													lightTorpedoCountToggle.isOn = false;
													lightTorpedoCountToggle.interactable = false;

												}

												//check if we have heavy torpedos
												if (mouseMananger.selectedUnit.GetComponent<TorpedoSection> ().heavyTorpedos > 0) {

													//check if we are cloaked
													if (mouseMananger.selectedUnit.GetComponent<CloakingDevice> () == true &&
														mouseMananger.selectedUnit.GetComponent<CloakingDevice> ().isCloaked == true) {

														//we are cloaked and cannot fire
														heavyTorpedoFireButton.interactable = false;
														heavyTorpedoCountToggle.isOn = false;
														heavyTorpedoCountToggle.interactable = false;

													} else {

														heavyTorpedoFireButton.interactable = true;
														heavyTorpedoCountToggle.isOn = true;
														heavyTorpedoCountToggle.interactable = true;

													}

												}
												//the else condition is that we do not have any heavy torpedos, and so the button can't be used
												else {

													heavyTorpedoFireButton.interactable = false;
													heavyTorpedoCountToggle.isOn = false;
													heavyTorpedoCountToggle.interactable = false;

												}

											}
											//the else condition is that there is no laser scattering on the targeted unit
											else {

												//we can now set the torpedo laser shot toggle based on inventory
												if (mouseMananger.selectedUnit.GetComponentInChildren<TorpedoSection> ().torpedoLaserShot <= 0) {

													//if laser shot == 0, we have none and cannot use it
													useTorpedoLaserShotToggle.isOn = false;
													useTorpedoLaserShotToggle.interactable = false;

													torpedoLaserShotCountToggle.isOn = false;
													torpedoLaserShotCountToggle.interactable = false;

												}
												//the else condition is that the radar shot quantity is greater than zero
												else {

													//check if we have a laser guidance system
													if (mouseMananger.selectedUnit.GetComponentInChildren<TorpedoSection> ().torpedoLaserGuidanceSystem == true) {

														//if we have the system, the shot can't be used
														useTorpedoLaserShotToggle.interactable = false;
														torpedoLaserShotCountToggle.interactable = false;

													} else {

														useTorpedoLaserShotToggle.interactable = true;
														torpedoLaserShotCountToggle.interactable = true;

													}

												}

												//check if we have targeting device enabled
												if (torpedoLaserGuidanceToggle.isOn == true || useTorpedoLaserShotToggle.isOn == true) {

													torpedoTargetingDropdown.interactable = true;

													//if we have targeting enabled, we can't allow the fire button to be pressed until a section has been chosen
													if (torpedoTargetingDropdown.GetComponentInChildren<TextMeshProUGUI>().text == "Choose Section") {

														lightTorpedoFireButton.interactable = false;
														lightTorpedoCountToggle.isOn = false;
														lightTorpedoCountToggle.interactable = false;

														heavyTorpedoFireButton.interactable = false;
														heavyTorpedoCountToggle.isOn = false;
														heavyTorpedoCountToggle.interactable = false;

													}
													//the else condition is that a section has been chosen, so we can use the fire button if we have inventory
													else {

														//check if we have light torpedos
														if (mouseMananger.selectedUnit.GetComponent<TorpedoSection> ().lightTorpedos > 0) {

															//check if we are cloaked
															if (mouseMananger.selectedUnit.GetComponent<CloakingDevice> () == true &&
																mouseMananger.selectedUnit.GetComponent<CloakingDevice> ().isCloaked == true) {

																//we are cloaked and cannot fire
																lightTorpedoFireButton.interactable = false;
																lightTorpedoCountToggle.isOn = false;
																lightTorpedoCountToggle.interactable = false;

															} else {

																lightTorpedoFireButton.interactable = true;
																lightTorpedoCountToggle.isOn = true;
																lightTorpedoCountToggle.interactable = true;

															}

														}
														//the else condition is that we do not have any light torpedos, and so the button can't be used
														else {

															lightTorpedoFireButton.interactable = false;
															lightTorpedoCountToggle.isOn = false;
															lightTorpedoCountToggle.interactable = false;

														}

														//check if we have heavy torpedos
														if (mouseMananger.selectedUnit.GetComponent<TorpedoSection> ().heavyTorpedos > 0) {

															//check if we are cloaked
															if (mouseMananger.selectedUnit.GetComponent<CloakingDevice> () == true &&
																mouseMananger.selectedUnit.GetComponent<CloakingDevice> ().isCloaked == true) {

																//we are cloaked and cannot fire
																heavyTorpedoFireButton.interactable = false;
																heavyTorpedoCountToggle.isOn = false;
																heavyTorpedoCountToggle.interactable = false;

															} else {

																heavyTorpedoFireButton.interactable = true;
																heavyTorpedoCountToggle.isOn = true;
																heavyTorpedoCountToggle.interactable = true;

															}

														}
														//the else condition is that we do not have any heavy torpedos, and so the button can't be used
														else {

															heavyTorpedoFireButton.interactable = false;
															heavyTorpedoCountToggle.isOn = false;
															heavyTorpedoCountToggle.interactable = false;

														}

													}

												}
												//the else condition is that we do not have a targeting device enabled
												else {

													torpedoTargetingDropdown.interactable = false;

													//we can fire torpedos if we have inventory

													//check if we have light torpedos
													if (mouseMananger.selectedUnit.GetComponent<TorpedoSection> ().lightTorpedos > 0) {

														//check if we are cloaked
														if (mouseMananger.selectedUnit.GetComponent<CloakingDevice> () == true &&
															mouseMananger.selectedUnit.GetComponent<CloakingDevice> ().isCloaked == true) {

															//we are cloaked and cannot fire
															lightTorpedoFireButton.interactable = false;
															lightTorpedoCountToggle.isOn = false;
															lightTorpedoCountToggle.interactable = false;

														} else {

															lightTorpedoFireButton.interactable = true;
															lightTorpedoCountToggle.isOn = true;
															lightTorpedoCountToggle.interactable = true;

														}

													}
													//the else condition is that we do not have any light torpedos, and so the button can't be used
													else {

														lightTorpedoFireButton.interactable = false;
														lightTorpedoCountToggle.isOn = false;
														lightTorpedoCountToggle.interactable = false;

													}

													//check if we have heavy torpedos
													if (mouseMananger.selectedUnit.GetComponent<TorpedoSection> ().heavyTorpedos > 0) {

														//check if we are cloaked
														if (mouseMananger.selectedUnit.GetComponent<CloakingDevice> () == true &&
															mouseMananger.selectedUnit.GetComponent<CloakingDevice> ().isCloaked == true) {

															//we are cloaked and cannot fire
															heavyTorpedoFireButton.interactable = false;
															heavyTorpedoCountToggle.isOn = false;
															heavyTorpedoCountToggle.interactable = false;

														} else {

															heavyTorpedoFireButton.interactable = true;
															heavyTorpedoCountToggle.isOn = true;
															heavyTorpedoCountToggle.interactable = true;

														}

													}
													//the else condition is that we do not have any heavy torpedos, and so the button can't be used
													else {

														heavyTorpedoFireButton.interactable = false;
														heavyTorpedoCountToggle.isOn = false;
														heavyTorpedoCountToggle.interactable = false;

													}

												}  //end else we do not have targeting device enabled

											} //end else there is no laser scattering on targeted unit

										}  //end else the targeted unit is a base

									}  //end else the targeted unit is not cloaked 

								} // end else we have a targeted unit

							}  //end else we have not used torpedos yet

						}  //end else the torpedo section is not destroyed

					}  //end else the selected unit has a torpedo section

				}  //end else the selected unit is a ship

			} //end else there is a selected unit

			SetTargetingText ();
			SetTargetingSectionOptions ();
			torpedoTargetingDropdown.RefreshShownValue ();
			SetFireButtonText ();

		}  //end if the torpedo attack toggle has been turned on

		//this is if the torpedo attack has been turned off
		else if (torpedoToggle.isOn == false) {

			//if we have don't have a selected unit, we can turn the toggles off and make them not interactable
			if (mouseMananger.selectedUnit == null) {

				useTorpedoLaserShotToggle.isOn = false;
				useTorpedoLaserShotToggle.interactable = false;

				torpedoLaserShotCountToggle.isOn = false;
				torpedoLaserShotCountToggle.interactable = false;

				torpedoLaserGuidanceToggle.isOn = false;
				torpedoLaserGuidanceToggle.interactable = false;

				torpedoHighPressureTubesToggle.isOn = false;
				torpedoHighPressureTubesToggle.interactable = false;



				torpedoTargetingDropdown.interactable = false;

				lightTorpedoFireButton.interactable = false;
				lightTorpedoCountToggle.isOn = false;
				lightTorpedoCountToggle.interactable = false;

				heavyTorpedoFireButton.interactable = false;
				heavyTorpedoCountToggle.isOn = false;
				heavyTorpedoCountToggle.interactable = false;

				SetTargetingText ();
				SetFireButtonText ();
			}

			//now we can check if we have a targeted unit
			if (mouseMananger.targetedUnit == null) {

				//if there is no targeted unit anymore, we can set the toggles off and make them not interactable
				useTorpedoLaserShotToggle.isOn = false;
				useTorpedoLaserShotToggle.interactable = false;

				torpedoLaserShotCountToggle.isOn = false;
				torpedoLaserShotCountToggle.interactable = false;

				torpedoLaserGuidanceToggle.isOn = false;
				torpedoLaserGuidanceToggle.interactable = false;

				torpedoHighPressureTubesToggle.isOn = false;
				torpedoHighPressureTubesToggle.interactable = false;



				torpedoTargetingDropdown.interactable = false;

				lightTorpedoFireButton.interactable = false;
				lightTorpedoCountToggle.isOn = false;
				lightTorpedoCountToggle.interactable = false;

				heavyTorpedoFireButton.interactable = false;
				heavyTorpedoCountToggle.isOn = false;
				heavyTorpedoCountToggle.interactable = false;

				SetTargetingText ();
				SetFireButtonText ();
			}

		}

	}

	//this function sets the helper text for the torpedo targeting
	private void SetTargetingText(){

		//check if there is a targeted unit
		if (mouseMananger.targetedUnit != null) {

			//check if targeted unit is cloaked
			if (mouseMananger.targetedUnit.GetComponent<CloakingDevice> () == true && mouseMananger.targetedUnit.GetComponent<CloakingDevice> ().isCloaked == true) {

				torpedoTargetingText.text = ("Target Is Cloaked!");

				//update the font size if necessary
				UIManager.AutoSizeTextMeshFont(torpedoTargetingText);

			}
			//the else condition is that the targeted unit is not cloaked
			else {

				//check if the targeted unit is a ship
				if (mouseMananger.targetedUnit.GetComponent<Ship> () == true) {

					//check if the targeted ship has a storage section
					if (mouseMananger.targetedUnit.GetComponent<StorageSection> () == true) {

						//check if the targeted ship has laser scattering
						if (mouseMananger.targetedUnit.GetComponent<StorageSection> ().laserScatteringSystem == true) {

							torpedoTargetingText.text = ("Target Has Scattering!");

							//update the font size if necessary
							UIManager.AutoSizeTextMeshFont(torpedoTargetingText);

						}
						//the else condition is that the targeted unit does not have laser scattering
						else {

							//check if we have targeting device enabled
							//note that there must be a selected unit in order for there to be a targeted unit
							if (torpedoLaserGuidanceToggle.isOn == true || useTorpedoLaserShotToggle.isOn == true) {

								torpedoTargetingText.text = ("Choose Section");

								//update the font size if necessary
								UIManager.AutoSizeTextMeshFont(torpedoTargetingText);

							}
							//the else condition is that we do not have a targeting device enabled
							else {

								torpedoTargetingText.text = ("No Targeting Active");

								//update the font size if necessary
								UIManager.AutoSizeTextMeshFont(torpedoTargetingText);

							}

						}

					}
					//the else condition is that the targeted ship has no storage section, which means it can't have laser scattering
					else {

						//check if we have targeting device enabled
						//note that there must be a selected unit in order for there to be a targeted unit
						if (torpedoLaserGuidanceToggle.isOn == true || useTorpedoLaserShotToggle.isOn == true) {

							torpedoTargetingText.text = ("Choose Section");

							//update the font size if necessary
							UIManager.AutoSizeTextMeshFont(torpedoTargetingText);

						}
						//the else condition is that we do not have a targeting device enabled
						else {

							torpedoTargetingText.text = ("No Targeting Active");

							//update the font size if necessary
							UIManager.AutoSizeTextMeshFont(torpedoTargetingText);

						}

					}

				}
				//the else condition is that the targeted unit is not a ship
				else if (mouseMananger.targetedUnit.GetComponent<Starbase> () == true) {

					//check if the targeted ship has laser scattering
					if (mouseMananger.targetedUnit.GetComponent<StarbaseStorageSection2> ().laserScatteringSystem == true) {

						torpedoTargetingText.text = ("Target Has Scattering!");

						//update the font size if necessary
						UIManager.AutoSizeTextMeshFont(torpedoTargetingText);

					}
					//the else condition is that the targeted unit does not have laser scattering
					else {

						//check if we have targeting device enabled
						//note that there must be a selected unit in order for there to be a targeted unit
						if (torpedoLaserGuidanceToggle.isOn == true || useTorpedoLaserShotToggle.isOn == true) {

							torpedoTargetingText.text = ("Choose Section");

							//update the font size if necessary
							UIManager.AutoSizeTextMeshFont(torpedoTargetingText);

						}
						//the else condition is that we do not have a targeting device enabled
						else {

							torpedoTargetingText.text = ("No Targeting Active");

							//update the font size if necessary
							UIManager.AutoSizeTextMeshFont(torpedoTargetingText);

						}

					}

				}

			}

		}
		//the else condition is that there is no targeted unit
		else {

			//check whether we have a selected unit
			if (mouseMananger.selectedUnit != null) {

				torpedoTargetingText.text = ("No Unit Targeted");

				//update the font size if necessary
				UIManager.AutoSizeTextMeshFont(torpedoTargetingText);

			}
			//the else condition is that there is no selected unit
			else {

				torpedoTargetingText.text = ("No Unit Selected");

				//update the font size if necessary
				UIManager.AutoSizeTextMeshFont(torpedoTargetingText);

			}

		}

	}

	//this function will set the available options for the targeting dropdown menu
	private void SetTargetingSectionOptions(){

		//start by clearing the existing dropdown options
		torpedoTargetingDropdown.ClearOptions();

		//create a list of new dropdown options to populate the choices
		List<TMP_Dropdown.OptionData> dropDownOptions = new List<TMP_Dropdown.OptionData> ();

		//add a dummy option at the top of the list
		dropDownOptions.Add(new TMP_Dropdown.OptionData("Choose Section"));

		//check if there is a targeted unit
		if (mouseMananger.targetedUnit != null) {

			//check if we have a targeting device enabled
			if (torpedoLaserGuidanceToggle.isOn == true || useTorpedoLaserShotToggle.isOn == true) {

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
				else if (mouseMananger.targetedUnit.GetComponent<Starbase> () == true)  {

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

		}

		//add the options to the dropdown
		torpedoTargetingDropdown.AddOptions (dropDownOptions);

		//update the font size if necessary
		UIManager.AutoSizeTextMeshFont(torpedoTargetingDropdown.GetComponentInChildren<TextMeshProUGUI> ());

	}

	//this function will set the fire button text
	private void SetFireButtonText(){

		//check if we have a selected unit
		if (mouseMananger.selectedUnit != null) {

			//check if the selected unit has a valid torpedo attack remaining
			if (mouseMananger.selectedUnit.GetComponent<CombatUnit> ().hasRemainingTorpedoAttack == true) {

				//check if the selected unit is cloaked
				if (mouseMananger.selectedUnit.GetComponent<CloakingDevice> () == true &&
				    mouseMananger.selectedUnit.GetComponent<CloakingDevice> ().isCloaked == true) {

					//if we are cloaked, set the text
					lightTorpedoFireButton.GetComponentInChildren<TextMeshProUGUI> ().text = ("Cloaked");
					heavyTorpedoFireButton.GetComponentInChildren<TextMeshProUGUI> ().text = ("Cloaked");

					//update the font size if necessary
					UIManager.AutoSizeTextMeshFont (lightTorpedoFireButton.GetComponentInChildren<TextMeshProUGUI> ());

					//update the font size if necessary
					UIManager.AutoSizeTextMeshFont (heavyTorpedoFireButton.GetComponentInChildren<TextMeshProUGUI> ());

				} else {

					//if the attack is still remaining, set the text to Light Torpedo and Heavy Torpedo
					lightTorpedoFireButton.GetComponentInChildren<TextMeshProUGUI> ().text = ("Light Torpedo");
					heavyTorpedoFireButton.GetComponentInChildren<TextMeshProUGUI> ().text = ("Heavy Torpedo");

					//update the font size if necessary
					UIManager.AutoSizeTextMeshFont (lightTorpedoFireButton.GetComponentInChildren<TextMeshProUGUI> ());

					//update the font size if necessary
					UIManager.AutoSizeTextMeshFont (heavyTorpedoFireButton.GetComponentInChildren<TextMeshProUGUI> ());

				}

			}
			//else, we want to check if the attack is not remaining because we already fired a torpedo
			//this is a slightly different check for ships and bases
			else if ((mouseMananger.selectedUnit.GetComponent<TorpedoSection> () == true &&
				mouseMananger.selectedUnit.GetComponent<TorpedoSection> ().usedTorpedosThisTurn == true) ||

				(mouseMananger.selectedUnit.GetComponent<StarbaseTorpedoSection> () == true &&
					mouseMananger.selectedUnit.GetComponent<StarbaseTorpedoSection> ().usedTorpedosThisTurn == true)) {

				//if we already fired torpedoes, set the text to already fired
				lightTorpedoFireButton.GetComponentInChildren<TextMeshProUGUI> ().text = ("Already Fired");
				heavyTorpedoFireButton.GetComponentInChildren<TextMeshProUGUI> ().text = ("Already Fired");

				//update the font size if necessary
				UIManager.AutoSizeTextMeshFont(lightTorpedoFireButton.GetComponentInChildren<TextMeshProUGUI> ());

				//update the font size if necessary
				UIManager.AutoSizeTextMeshFont(heavyTorpedoFireButton.GetComponentInChildren<TextMeshProUGUI> ());

			}
			//the else condition is that the attack is not remaining and we did not fire torpedos, which means we must have fired phasors
			//and don't have double-attack capability
			else {

				lightTorpedoFireButton.GetComponentInChildren<TextMeshProUGUI> ().text = ("Already Attacked");
				heavyTorpedoFireButton.GetComponentInChildren<TextMeshProUGUI> ().text = ("Already Attacked");

				//update the font size if necessary
				UIManager.AutoSizeTextMeshFont(lightTorpedoFireButton.GetComponentInChildren<TextMeshProUGUI> ());

				//update the font size if necessary
				UIManager.AutoSizeTextMeshFont(heavyTorpedoFireButton.GetComponentInChildren<TextMeshProUGUI> ());

			}

		} else {

			lightTorpedoFireButton.GetComponentInChildren<TextMeshProUGUI> ().text = ("Light Torpedo");
			heavyTorpedoFireButton.GetComponentInChildren<TextMeshProUGUI> ().text = ("Heavy Torpedo");

			//update the font size if necessary
			UIManager.AutoSizeTextMeshFont(lightTorpedoFireButton.GetComponentInChildren<TextMeshProUGUI> ());

			//update the font size if necessary
			UIManager.AutoSizeTextMeshFont(heavyTorpedoFireButton.GetComponentInChildren<TextMeshProUGUI> ());

		}

	}


	//this function handles firing a light torpedo
	private void FireLightTorpedo(){

		//invoke the OnFireLightTorpedo event, with the targeted unit passed to the event
		OnFireLightTorpedo.Invoke(mouseMananger.selectedUnit.GetComponent<CombatUnit>(),
			mouseMananger.targetedUnit.GetComponent<CombatUnit>(),
			torpedoTargetingDropdown.GetComponentInChildren<TextMeshProUGUI>().text);

		SetTorpedoMenuToggles ();

	}

	//this function handles firing a heavy torpedo
	private void FireHeavyTorpedo(){

		//invoke the OnFireHeavyTorpedo event, with the targeted unit passed to the event
		OnFireHeavyTorpedo.Invoke(mouseMananger.selectedUnit.GetComponent<CombatUnit>(),
			mouseMananger.targetedUnit.GetComponent<CombatUnit>(),
			torpedoTargetingDropdown.GetComponentInChildren<TextMeshProUGUI>().text);

		SetTorpedoMenuToggles ();

	}

	//this function handles OnDestroy
	private void OnDestroy(){

		RemoveAllListeners ();

	}

	//this function removes all listeners
	private void RemoveAllListeners(){

		if (torpedoToggle != null) {
			
			//remove an on-click event listener for the main menu torpedo toggle
			torpedoToggle.onValueChanged.RemoveListener (boolSetTorpedoMenuTogglesAction);

		}

		if (useTorpedoLaserShotToggle != null) {
			
			//remove an on-click event listener for the torpedo laser shot toggle
			useTorpedoLaserShotToggle.onValueChanged.RemoveListener (boolSetTorpedoMenuTogglesDropdownZeroAction);

		}

		if (torpedoTargetingDropdown != null) {
			
			//remove an event listener for the dropdown value changing
			torpedoTargetingDropdown.onValueChanged.RemoveListener (intSetTorpedoMenuTogglesAction);

		}

		if (lightTorpedoFireButton != null) {

			//remove an on-click event listener for the fire torpedos buttons
			lightTorpedoFireButton.onClick.RemoveListener (FireLightTorpedo);

		}

		if (heavyTorpedoFireButton != null) {
			
			heavyTorpedoFireButton.onClick.RemoveListener (FireHeavyTorpedo);

		}

		if (mouseMananger != null) {
			
			//remove an event listener for when a selectedUnit is set and cleared
			mouseMananger.OnSetSelectedUnit.RemoveListener (SetTorpedoMenuToggles);
			mouseMananger.OnClearSelectedUnit.RemoveListener (SetTorpedoMenuToggles);

			//remove an event listener for when a targetedUnit is set and cleared
			mouseMananger.OnSetTargetedUnit.RemoveListener (SetTorpedoMenuToggles);
			mouseMananger.OnClearTargetedUnit.RemoveListener (SetTorpedoMenuToggles);

		}

		//remove a listener for when a purchase is complete
		TorpedoSection.OnInventoryUpdated.RemoveListener (combatUnitSetTorpedoMenuTogglesAction);

	}

}
