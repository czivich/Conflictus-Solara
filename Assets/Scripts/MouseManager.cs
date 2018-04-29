using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class MouseManager : MonoBehaviour {


	private Vector3 localCoordinates;			//localCoordinates are the x,y,z relative to the game board

	private TileMap tileMap;					//we will need to refer to the TileMap to get the Layout
	private HexCursor hexCursor;				//we will need to refer to the HexCursor to move it around based on mouse inputs
	private TargetingCursor targetingCursor;	//this is the targeting cursor
	private Hex localHex;						//this is the current hex being returned by the raycast

	private Camera miniMapCamera;				//this is the minimap camera

	public GameObject hoveredObject {				//this is the current unit the mouse is hovering over
		get;
		private set;
	}

	public GameObject selectedUnit {			//this is the current selected unit	
		get;
		private set;
	}

	public GameObject targetedUnit {				//this is the current unit that has been targeted
		get;
		private set;
	}

	private float mouseClickTimer;						//this will count the time between mouseDown and mouseUp clicks
	private float mouseClickTimeThreshold = 0.25f;		//this is how long a click must be held before it becomes a drag
	private float mouseClickDistanceThreshold = .25f;	//this is how far away a click must be to count as a drag
	private float dragSpeed = 40.0f;					//this is the speed the camera will move when dragging

	private Vector3 mousePosition;				//store the current mouse position

	private bool mouseDragging = false;			//flag to indicate whether we're dragging or not

	private Vector3 dragAnchor;					//this will be the drag point for moving the map around

	private GameManager gameManager;			//we will need to access the gameManager

	private UIManager uiManager;					//we will need the UI Manager

	public GameObject pauseFadePanel;		//we will need the pauseFadePanel
	public GameObject pauseFadePanel2;		//we will need the pauseFadePanel

	//these variables are cached for update mouse raycasts
	private Vector3 viewPortPoint;
	private Ray ray; 
	private RaycastHit hitInfo;

	//private variable to hold world coordinates
	private Vector3 worldRoundedCoordinatesV3;

	//cached gameobject to check raycast hits
	private GameObject gameObjectHit;

	//cached value for mouse dragging
	private Vector3 dragOffset;

	//this variable controls the zoom direction
	private int mouseZoomDirection = 1;

	//this variable will control how fast the camera zooms in and out
	private float zoomSpeed;

	//use event system to track setting and clearing targeted unit
	public UnityEvent OnSetTargetedUnit = new UnityEvent();
	public UnityEvent OnClearTargetedUnit = new UnityEvent();

	//use the event system to track setting and clearing the selected unit
	public UnityEvent OnSetSelectedUnit = new UnityEvent();
	public UnityEvent OnSetSelectedUnitEarly = new UnityEvent();
	public UnityEvent OnClearSelectedUnit = new UnityEvent();

	//use the event system to track setting and clearing the hovered object
	public UnityEvent OnSetHoveredObject = new UnityEvent();
	public UnityEvent OnClearHoveredObject = new UnityEvent();

	//create an event for when you've selected someone else's unit
	public UnityEvent OnSelectForeignUnit = new UnityEvent();

	//this event is for when the CurrentMovementRange property changes
	public MoveEvent OnSignalMovement = new MoveEvent();

	//simple class derived from unityEvent to pass Ship Object, another ship object, and a destination hex
	public class MoveEvent : UnityEvent<Ship,Hex,Ship>{};

	//this event is for when a new unit is placed
	public NewUnitPlacementEvent OnPlacedNewUnit = new NewUnitPlacementEvent();

	//simple class derived from unityEvent for a new unit placement
	public class NewUnitPlacementEvent : UnityEvent<Hex>{};

	//event to flag an invalid action mode for the newly selected unit
	public UnityEvent OnInvalidActionModeForSelectedUnit = new UnityEvent();

	//unityActions
	private UnityAction<GameObject> nextUnitSetSelectedUnitAction;
	private UnityAction<Player.Color> colorClearSelectedUnitAction;
	private UnityAction<Ship> shipClearTargetedUnitAction;
	private UnityAction<CombatUnit> combatUnitCheckTargetedUnitAction;
	private UnityAction<Dictionary<string,int>,int,CombatUnit.UnitType> purchaseClearSelectedUnitAction;
	private UnityAction<FlareManager.FlareEventData> flareDataSetTargetedUnitAction;
	private UnityAction<bool> boolSetMouseZoomDirectionAction;
	private UnityAction<int> intSetMouseZoomSensitivityAction;
	private UnityAction<CombatUnit,CombatUnit,string> useRepairCrewClearSelectedUnitAction;
	private UnityAction<CombatUnit,CombatUnit,string> useCrystalClearSelectedUnitAction;

	public void Init(){
		
		//during the start, find the minimap camera
		miniMapCamera = GameObject.FindGameObjectWithTag ("MiniMapCamera").GetComponent<Camera> ();

		//grab the gameManager
		gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

		//grab the UIManager
		uiManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();

		//get the pauseFadePanel
		//pauseFadePanel = GameObject.FindGameObjectsWithTag("PauseFadePanel");

		//set the actions
		nextUnitSetSelectedUnitAction = (nextUnit) => {SetSelectedUnit(nextUnit);};
		colorClearSelectedUnitAction = (currentTurn) => {ClearSelectedUnit();};
		shipClearTargetedUnitAction = (ship) => {ClearTargetedUnit();};
		combatUnitCheckTargetedUnitAction = (combatUnit) => {CheckTargetedUnit(combatUnit);};
		purchaseClearSelectedUnitAction = (purchasedItems,purchaseCost,unitType) => {ClearSelectedUnit();};
		flareDataSetTargetedUnitAction = (flareEventData) => {SetTargetedUnit (flareEventData.targetedUnit.gameObject);};
		boolSetMouseZoomDirectionAction = ((isInverted) => {SetMouseZoomDirection (isInverted);});
		intSetMouseZoomSensitivityAction = ((zoomSensitivity) => {SetMouseZoomSensitivity (zoomSensitivity);});
		useRepairCrewClearSelectedUnitAction = (selectedUnit,targetedUnit,sectionTargeted) => {ClearSelectedUnit();};
		useCrystalClearSelectedUnitAction = (selectedUnit,targetedUnit,sectionTargeted) => {ClearSelectedUnit();};

		//add a listener to the NextUnit button event
		uiManager.GetComponent<NextUnit>().SetNextUnit.AddListener(nextUnitSetSelectedUnitAction);

		//add a listener for the EndTurn event - we don't need the currentTurn to pass to the clearSelectedUnit
		gameManager.OnEndTurn.AddListener(colorClearSelectedUnitAction);

		//add a listener for the static DisengageTractorBeam event so we can clear the targeted unit when it happens
		//the event passes the ship that disengaged the tractor beam, but we don't need it to clear the targeted unit
		PhasorSection.OnDisengageTractorBeam.AddListener(shipClearTargetedUnitAction);

		//add a listener for turning off the tractor beam toggle so we can clear the targeted unit
		uiManager.GetComponent<TractorBeamToggle>().OnTurnedOffTractorBeamToggleWhileNotEngaged.AddListener(ClearTargetedUnit);

		//add a listener for turning off the phasor attack toggle so we can clear the targeted unit
		uiManager.GetComponent<PhasorToggle>().OnTurnedOffPhasorToggle.AddListener(ClearTargetedUnit);

		//add a listener for turning off the torpedo attack toggle so we can clear the targeted unit
		uiManager.GetComponent<TorpedoToggle>().OnTurnedOffTorpedoToggle.AddListener(ClearTargetedUnit);

		//add a listener for turning off the crew toggle so we can clear the targeted unit
		uiManager.GetComponent<CrewToggle>().OnTurnedOffCrewToggle.AddListener(ClearTargetedUnit);

		//add a listener for turning off the useItem toggle so we can clear the targeted unit
		uiManager.GetComponent<UseItemToggle>().OnTurnedOffUseItemToggle.AddListener(ClearTargetedUnit);

		//add a listener for coming out of flare mode
		uiManager.GetComponent<FlareManager> ().OnUseFlaresYes.AddListener (flareDataSetTargetedUnitAction);
		uiManager.GetComponent<FlareManager> ().OnUseFlaresCancel.AddListener (flareDataSetTargetedUnitAction);

		//add listener for ship getting destroyed
		Ship.OnShipDestroyed.AddListener(combatUnitCheckTargetedUnitAction);

		//add listener for base getting destroyed
		Starbase.OnBaseDestroyed.AddListener(combatUnitCheckTargetedUnitAction);

		//add a listener for placing a unit
		uiManager.GetComponent<PurchaseManager>().OnOutfittedShip.AddListener(purchaseClearSelectedUnitAction);

		//add listener for mouse zoom direction
		uiManager.GetComponent<Settings>().OnChangeMouseZoomInversion.AddListener(boolSetMouseZoomDirectionAction);

		//add listener for mouse zoom speed
		uiManager.GetComponent<Settings>().OnChangeMouseZoomSensitivity.AddListener(intSetMouseZoomSensitivityAction);

		//add listeners for keyboard cancels
		uiManager.GetComponent<UINavigationMain>().OnCancelClearTargetedUnit.AddListener(ClearTargetedUnit);
		uiManager.GetComponent<UINavigationMain>().OnCancelClearSelectedUnit.AddListener(ClearSelectedUnit);

		//add listener for closing cutscene
		uiManager.GetComponent<CutsceneManager>().OnCloseCutsceneDisplayPanel.AddListener(ClearSelectedUnit);

		//add listener for using repair crew
		uiManager.GetComponent<CrewMenu>().OnUseRepairCrew.AddListener(useRepairCrewClearSelectedUnitAction);
			
		//add listener for using crystals
		uiManager.GetComponent<UseItemMenu>().OnUseDilithiumCrystal.AddListener(useCrystalClearSelectedUnitAction);
		uiManager.GetComponent<UseItemMenu>().OnUseTrilithiumCrystal.AddListener(useCrystalClearSelectedUnitAction);

	}

	// Update is called once per frame
	private void Update () {

		//this if checks if _tileMap has been set, and if not, gets component, then gets the layout
		if(tileMap == null){
			
			tileMap = GameObject.FindGameObjectWithTag("TileMap").GetComponent<TileMap>();

		}

		//only run the GameObject.Find FindCursor function if it hasn't already been found yet
		if (hexCursor == null) {

			//find hexCursor
			hexCursor = GameObject.FindGameObjectWithTag("HexCursor").GetComponent<HexCursor>();

		}

		//this if checks if _tileMap has been set, and if not, gets component, then gets the layout
		if(targetingCursor == null){

			targetingCursor = GameObject.FindGameObjectWithTag("TargetingCursor").GetComponent<TargetingCursor>();

		}

			
		//cache the mouse position for the current frame
		mousePosition = Input.mousePosition;

		//check to find out if our mouse position is over the main camera viewport rect
		if (mousePosition.x >= Camera.main.pixelRect.xMin && mousePosition.x <= Camera.main.pixelRect.xMax && mousePosition.y >= Camera.main.pixelRect.yMin && mousePosition.y <= Camera.main.pixelRect.yMax) {

			//only do mouse management if we are not over a UI element
			if (EventSystem.current.IsPointerOverGameObject () == false) {
				
				//check to see if we have clicked the mouse button down this frame
				//if so, we want to start the mouseClickTimer
				if (Input.GetMouseButton (0) == true) {

					//increment the timer by the amount of time since last frame
					mouseClickTimer += Time.deltaTime;

				} 
				
				//use viewportPointToRay instead of ScreenPoint so that stuff behind the sidebar gui isn't affected
				viewPortPoint = Camera.main.ScreenToViewportPoint (Input.mousePosition);
				ray = Camera.main.ViewportPointToRay (viewPortPoint);

				//if ray hits a collider, return hitInfo.  Ray is shot to infinity
				//checking that viewport point is within the viewport will make sure things have to be on camera to work
				if (Physics.Raycast (ray, out hitInfo, Mathf.Infinity) && viewPortPoint.x <= 1.0f && viewPortPoint.x >= 0.0f && viewPortPoint.y <= 1.0f && viewPortPoint.y >= 0.0f) {

					//check if we are not dragging and if we clicked down on this frame
					if (mouseDragging == false && Input.GetMouseButtonDown (0) == true) {
					
						//set dragAnchor equal to the raycast hit point
						dragAnchor = hitInfo.point;

						//set the dragging flag to true
						mouseDragging = true;

						//Debug.Log ("mousedrag on");

					}

					//set localHex to the rounded hex we are hovered over
					localHex = tileMap.WorldToRoundedHexCoordinates (hitInfo.point);

					//this line takes the local hex and converts that point back to world space
					worldRoundedCoordinatesV3 = tileMap.HexToWorldCoordinates (localHex);

					//only move the hex cursor if we're not holding down the mouse button
					if (Input.GetMouseButton (0) == false) {
					
						//set the selection hex transform position to that rounded world coordinates
						//the additional vector3 offsets the hexcursor up above units
						hexCursor.transform.position = new Vector3 (worldRoundedCoordinatesV3.x, hexCursor.transform.position.y, worldRoundedCoordinatesV3.z);

						//rotation to match game board rotation - it was already in the right position, just not rotated
						hexCursor.transform.rotation = tileMap.transform.rotation;

						//move the targeting cursor to be in the same location as the hex cursor
						//set the targeting hex transform position to that rounded world coordinates
						//the additional vector3 offsets the hexcursor up above units
						targetingCursor.transform.position = new Vector3 (worldRoundedCoordinatesV3.x, targetingCursor.transform.position.y, worldRoundedCoordinatesV3.z);

						//rotation to match game board rotation - it was already in the right position, just not rotated
						targetingCursor.transform.rotation = tileMap.transform.rotation;


						//depending on the mode and where the mouse is, we need to determine which cursor to show
						//in selection mode, we will always want the hex cursor turned on and targeting cursor turned off
						if (gameManager.CurrentActionMode == GameManager.ActionMode.Selection) {

							hexCursor.GetComponentInChildren<MeshRenderer> ().enabled = true;
							targetingCursor.GetComponentInChildren<MeshRenderer> ().enabled = false;

						}

						//in Movement mode, we will always want the hex cursor turned on and targeting cursor turned off
						else if (gameManager.CurrentActionMode == GameManager.ActionMode.Movement) {

							hexCursor.GetComponentInChildren<MeshRenderer> ().enabled = true;
							targetingCursor.GetComponentInChildren<MeshRenderer> ().enabled = false;

						}

						//in tractor beam mode, we will want the targeting cursor visible if we are hovering within the targeting range
						else if (gameManager.CurrentActionMode == GameManager.ActionMode.TractorBeam) {

							//we need to determine if we are currently showing a range
							//first, check if we have a combat unit selected
							if (selectedUnit != null) {

								//next, check if the selected unit is a ship.  Only a ship will be able to use a tractor beam
								if (selectedUnit.GetComponent<Ship> () == true) {

									//finally, check if the local hex we are mousing over is contained in the tractor beam range
									//if so, we want to be using the targeting cursor
									if (selectedUnit.GetComponent<PhasorSection> ().TargetableTractorBeamHexes.Contains (localHex) == true) {

										hexCursor.GetComponentInChildren<MeshRenderer> ().enabled = false;
										targetingCursor.GetComponentInChildren<MeshRenderer> ().enabled = true;

									}

									//if we are not inside the range, we want to be using the regular hex cursor
									else {

										hexCursor.GetComponentInChildren<MeshRenderer> ().enabled = true;
										targetingCursor.GetComponentInChildren<MeshRenderer> ().enabled = false;

									}

								}

								//else, if the selected unit is not a ship, we want to use the regular cursor
								else if (selectedUnit.GetComponent<Starbase> () == true) {

									hexCursor.GetComponentInChildren<MeshRenderer> ().enabled = true;
									targetingCursor.GetComponentInChildren<MeshRenderer> ().enabled = false;

								}

							}

							//else, if the selected unit is null, we want to use the regular cursor
							else {

								hexCursor.GetComponentInChildren<MeshRenderer> ().enabled = true;
								targetingCursor.GetComponentInChildren<MeshRenderer> ().enabled = false;

							}

						} else if (gameManager.CurrentActionMode == GameManager.ActionMode.PhasorAttack) {

							//we need to determine if we are currently showing a range
							//first, check if we have a combat unit selected
							if (selectedUnit != null) {

								//next, check if the selected unit is a ship.  We will handle starbases slightly differently
								if (selectedUnit.GetComponent<Ship> () == true) {

									//finally, check if the local hex we are mousing over is contained in the phasor attack range
									//if so, we want to be using the targeting cursor
									if (selectedUnit.GetComponent<PhasorSection> ().TargetablePhasorHexes.Contains (localHex) == true) {

										hexCursor.GetComponentInChildren<MeshRenderer> ().enabled = false;
										targetingCursor.GetComponentInChildren<MeshRenderer> ().enabled = true;

									}

									//if we are not inside the range, we want to be using the regular hex cursor
									else {

										hexCursor.GetComponentInChildren<MeshRenderer> ().enabled = true;
										targetingCursor.GetComponentInChildren<MeshRenderer> ().enabled = false;

									}

								}

								//else, if the selected unit is not a ship
								else if (selectedUnit.GetComponent<Starbase> () == true) {

									//finally, check if the local hex we are mousing over is contained in the phasor attack range
									//if so, we want to be using the targeting cursor
									//we can check either starbase phasor section for the targetable range
									if (selectedUnit.GetComponent<StarbasePhasorSection1> ().TargetablePhasorHexes.Contains (localHex) == true) {

										hexCursor.GetComponentInChildren<MeshRenderer> ().enabled = false;
										targetingCursor.GetComponentInChildren<MeshRenderer> ().enabled = true;

									}

									//if we are not inside the range, we want to be using the regular hex cursor
									else {

										hexCursor.GetComponentInChildren<MeshRenderer> ().enabled = true;
										targetingCursor.GetComponentInChildren<MeshRenderer> ().enabled = false;

									}

								}

							}

							//else, if the selected unit is null, we want to use the regular cursor

							else {

								hexCursor.GetComponentInChildren<MeshRenderer> ().enabled = true;
								targetingCursor.GetComponentInChildren<MeshRenderer> ().enabled = false;

							}

						} else if (gameManager.CurrentActionMode == GameManager.ActionMode.TorpedoAttack) {

							//we need to determine if we are currently showing a range
							//first, check if we have a combat unit selected
							if (selectedUnit != null) {

								//next, check if the selected unit is a ship.  We will handle starbases slightly differently
								if (selectedUnit.GetComponent<Ship> () == true) {

									//finally, check if the local hex we are mousing over is contained in the torpedo attack range
									//if so, we want to be using the targeting cursor
									if (selectedUnit.GetComponent<TorpedoSection> ().TargetableTorpedoHexes.Contains (localHex) == true) {

										hexCursor.GetComponentInChildren<MeshRenderer> ().enabled = false;
										targetingCursor.GetComponentInChildren<MeshRenderer> ().enabled = true;

									}

									//if we are not inside the range, we want to be using the regular hex cursor
									else {

										hexCursor.GetComponentInChildren<MeshRenderer> ().enabled = true;
										targetingCursor.GetComponentInChildren<MeshRenderer> ().enabled = false;

									}

								}

								//else, if the selected unit is not a ship
								else if (selectedUnit.GetComponent<Starbase> () == true) {

									//finally, check if the local hex we are mousing over is contained in the torpedo attack range
									//if so, we want to be using the targeting cursor
									if (selectedUnit.GetComponent<StarbaseTorpedoSection> ().TargetableTorpedoHexes.Contains (localHex) == true) {

										hexCursor.GetComponentInChildren<MeshRenderer> ().enabled = false;
										targetingCursor.GetComponentInChildren<MeshRenderer> ().enabled = true;

									}

									//if we are not inside the range, we want to be using the regular hex cursor
									else {

										hexCursor.GetComponentInChildren<MeshRenderer> ().enabled = true;
										targetingCursor.GetComponentInChildren<MeshRenderer> ().enabled = false;

									}

								}

							}

							//else, if the selected unit is null, we want to use the regular cursor

							else {

								hexCursor.GetComponentInChildren<MeshRenderer> ().enabled = true;
								targetingCursor.GetComponentInChildren<MeshRenderer> ().enabled = false;

							}

						} else if (gameManager.CurrentActionMode == GameManager.ActionMode.ItemUse) {

							//we need to determine if we are currently showing a range
							//first, check if we have a combat unit selected
							if (selectedUnit != null) {

								//next, check if the selected unit is a ship.  We will handle starbases slightly differently
								if (selectedUnit.GetComponent<Ship> () == true) {

									//finally, check if the local hex we are mousing over is contained in the item use range
									//if so, we want to be using the targeting cursor
									if (selectedUnit.GetComponent<StorageSection> ().TargetableItemHexes.Contains (localHex) == true) {

										hexCursor.GetComponentInChildren<MeshRenderer> ().enabled = false;
										targetingCursor.GetComponentInChildren<MeshRenderer> ().enabled = true;

									}

									//if we are not inside the range, we want to be using the regular hex cursor
									else {

										hexCursor.GetComponentInChildren<MeshRenderer> ().enabled = true;
										targetingCursor.GetComponentInChildren<MeshRenderer> ().enabled = false;

									}

								}

								//else, if the selected unit is not a ship
								else if (selectedUnit.GetComponent<Starbase> () == true) {

									//finally, check if the local hex we are mousing over is contained in the item use range
									//if so, we want to be using the targeting cursor
									//we can key off of either starbase storage section
									if (selectedUnit.GetComponent<StarbaseStorageSection1> ().TargetableItemHexes.Contains (localHex) == true) {

										hexCursor.GetComponentInChildren<MeshRenderer> ().enabled = false;
										targetingCursor.GetComponentInChildren<MeshRenderer> ().enabled = true;

									}

									//if we are not inside the range, we want to be using the regular hex cursor
									else {

										hexCursor.GetComponentInChildren<MeshRenderer> ().enabled = true;
										targetingCursor.GetComponentInChildren<MeshRenderer> ().enabled = false;

									}

								}

							}

							//else, if the selected unit is null, we want to use the regular cursor

							else {

								hexCursor.GetComponentInChildren<MeshRenderer> ().enabled = true;
								targetingCursor.GetComponentInChildren<MeshRenderer> ().enabled = false;

							}


						} else if (gameManager.CurrentActionMode == GameManager.ActionMode.Crew) {

							//we need to determine if we are currently showing a range
							//first, check if we have a combat unit selected
							if (selectedUnit != null) {

								//next, check if the selected unit is a ship.  We will handle starbases slightly differently
								if (selectedUnit.GetComponent<Ship> () == true) {

									//finally, check if the local hex we are mousing over is contained in the item use range
									//if so, we want to be using the targeting cursor
									if (selectedUnit.GetComponent<CrewSection> ().TargetableCrewHexes.Contains (localHex) == true) {

										hexCursor.GetComponentInChildren<MeshRenderer> ().enabled = false;
										targetingCursor.GetComponentInChildren<MeshRenderer> ().enabled = true;

									}

									//if we are not inside the range, we want to be using the regular hex cursor
									else {

										hexCursor.GetComponentInChildren<MeshRenderer> ().enabled = true;
										targetingCursor.GetComponentInChildren<MeshRenderer> ().enabled = false;

									}

								}

								//else, if the selected unit is not a ship
								else if (selectedUnit.GetComponent<Starbase> () == true) {

									//finally, check if the local hex we are mousing over is contained in the item use range
									//if so, we want to be using the targeting cursor
									if (selectedUnit.GetComponent<StarbaseCrewSection> ().TargetableCrewHexes.Contains (localHex) == true) {

										hexCursor.GetComponentInChildren<MeshRenderer> ().enabled = false;
										targetingCursor.GetComponentInChildren<MeshRenderer> ().enabled = true;

									}

									//if we are not inside the range, we want to be using the regular hex cursor
									else {

										hexCursor.GetComponentInChildren<MeshRenderer> ().enabled = true;
										targetingCursor.GetComponentInChildren<MeshRenderer> ().enabled = false;

									}

								}

							}

							//else, if the selected unit is null, we want to use the regular cursor

							else {

								hexCursor.GetComponentInChildren<MeshRenderer> ().enabled = true;
								targetingCursor.GetComponentInChildren<MeshRenderer> ().enabled = false;

							}

						}  //end the crew action mode check
						//in cloaking mode, we will always want the hex cursor turned on and targeting cursor turned off
						else if (gameManager.CurrentActionMode == GameManager.ActionMode.Cloaking) {

							hexCursor.GetComponentInChildren<MeshRenderer> ().enabled = true;
							targetingCursor.GetComponentInChildren<MeshRenderer> ().enabled = false;

						}

						//in placeunit mode, we will always want the hex cursor turned on and targeting cursor turned off
						else if (gameManager.CurrentActionMode == GameManager.ActionMode.PlaceNewUnit) {

							hexCursor.GetComponentInChildren<MeshRenderer> ().enabled = true;
							targetingCursor.GetComponentInChildren<MeshRenderer> ().enabled = false;

						}
					}  //end the action mode elses

					//code to select a unit if it was clicked on with the left mouse button

					//i started putting combat units into collectors, so we need to drop down 1 level to get the game object I want
					//if I am in a collector, that means my collider hit was actually a hexagon, which is 1 level down from the object
					//so I need to just go up 1 level
					if (hitInfo.collider.transform.root.gameObject.CompareTag( "Collector")) {

						gameObjectHit = hitInfo.collider.transform.parent.gameObject;

					} else {

						gameObjectHit = hitInfo.collider.transform.root.gameObject;

					}

					//if the raycast hit something, set the hovered object to what was hit
					if (gameObjectHit != null) {

						SetHoveredObject (gameObjectHit);

					}

					//calculate the distance between the drag anchor and the current raycast hit point
					//if these are sufficiently far away from each other, we won't count that as a click
					float clickDistance = Vector3.Magnitude (dragAnchor - hitInfo.point);



					//this next section of code handles click events in the main viewport
					//the click events fire on GetMouseButtonUp

					//we are checking 3 things to allow a click event 
					//the getMouseButtonUp must have fired this frame
					//and we have to have released the mouse button quickly enough after pressing it down that we're not in drag mode
					//and we have to have released the click with the mouse still close enough to the position where it was clicked down
					//if all of these are true, allow the click event to happen
					if (Input.GetMouseButtonUp (0) == true && mouseClickTimer <= mouseClickTimeThreshold && clickDistance <= mouseClickDistanceThreshold) {

						//this first section covers when we are in Selection Mode
						if (gameManager.CurrentActionMode == GameManager.ActionMode.Selection) {

							//if we are hovering over nothing and click, clear the current selection
							if (hoveredObject == null) {

								ClearSelectedUnit ();

							}

							//check if we are clicking on the currenet selected unit.  we want to be able to clear the unit by clicking a 2nd time on it
							else if (hoveredObject == selectedUnit) {

								ClearSelectedUnit ();
								//Debug.Log ("hovered unit == selectedUnit, clear selected unit");

							}

							//if we are here, we have clicked on something that's not another combat unit
							else if (hoveredObject.GetComponent<CombatUnit> () == false) {

								//clear the selected unit once we click a non-unit
								ClearSelectedUnit ();
								//Debug.Log ("hovered unit == not combat unit, clear selected unit");

							}

							//if we are here, we clicked on something that is another combat unit
							else {

								//if the hoveredUnit is a ship that's moving, we don't want to be able to select it while it's moving
								//because that could screw up the movement points math
								//check to see if the hovered unit is a ship first
								if (hoveredObject.GetComponentInChildren<Ship> () == true) {

									//before setting selected unit, verify that hovered unit is not moving - that would screw things up
									if (hoveredObject.GetComponentInChildren<EngineSection> ().isMoving == false) {

										//Debug.Log ("Found Combat Unit!");
										SetSelectedUnit (hoveredObject);

									}

								} 

								//this else must be a non-ship combat unit
								else if (hoveredObject.GetComponentInChildren<Starbase> () == true) {

									//Debug.Log ("Found Combat Unit!");
									SetSelectedUnit (hoveredObject);

								}

							}

						}

						//check if we are in Movement Mode- we need to handle 4 conditions:
						//clicked on the selected unit
						//clicked on another combat unit
						//clicked on an empty tile within movement range
						//clicked on an empty tile outside movement range
						else if (gameManager.CurrentActionMode == GameManager.ActionMode.Movement) {

							//check if a ship is moving - we don't want to allow click events if a ship is moving
							if (gameManager.aShipIsMoving == false) {

								//check if we are clicking on the currenet selected unit.  we want to be able to clear the unit by clicking a 2nd time on it
								if (hoveredObject == null || hoveredObject == selectedUnit) {

								
									ClearSelectedUnit ();

									//Debug.Log ("hovered unit == selectedUnit, clear selected unit");
						
									//at this check, we know we're not hovering over the selected unit, so if it's a combat unit it's a different unit
								} else if (hoveredObject.GetComponent<CombatUnit> () == true) {

									//if the hoveredUnit is a ship that's moving, we don't want to be able to select it while it's moving
									//because that could screw up the movement points math
									//check to see if the hovered unit is a ship first
									if (hoveredObject.GetComponentInChildren<Ship> () == true) {

										//before setting selected unit, verify that hovered unit is not moving - that would screw things up
										if (hoveredObject.GetComponentInChildren<EngineSection> ().isMoving == false) {

											//Debug.Log ("Found Combat Unit!");
											SetSelectedUnit (hoveredObject);

										}

									} 

									//this else must be a non-ship combat unit, which would be a starbase
									else if (hoveredObject.GetComponentInChildren<Starbase> () == true) {

										//Debug.Log ("Found Combat Unit!");
										SetSelectedUnit (hoveredObject);

									}

								}
							
								//if we are here, we have clicked on something that's not another combat unit
								else if (hoveredObject.GetComponent<CombatUnit> () == false) {

									//check to make sure the selected unit is not null
									if (selectedUnit != null) {

										//check if the selected unit is a ship - it can only move if it is a ship
										if (selectedUnit.GetComponent<Ship> () == true) {

											//check if the ship is showing movement range and we have clicked within that range
											if (selectedUnit.GetComponent<EngineSection> ().ReachableHexes.Contains (localHex) == true) {

												//check to see if the selected unit has a tractor beam engaged
												if (selectedUnit.GetComponent<PhasorSection> ().tractorBeamIsPrimed == true) {

													//call the moveShip method with a towed unit and no unit towing us
													//selectedUnit.GetComponent<Ship> ().MoveShip (localHex, targetedUnit.GetComponent<Ship> (), null);

													//invoke on OnSignalMove event, indicating a towed unit coming along for the ride
													OnSignalMovement.Invoke (selectedUnit.GetComponent<Ship> (), localHex, targetedUnit.GetComponent<Ship> ());

												}

												//the else condition is that the selected unit does not have a tractor beam engaged.
												else {
											
													//moving the ship breaks any targeting in place
													ClearTargetedUnit ();
									
													//call the moveShip method without a towed unit
													//selectedUnit.GetComponent<Ship> ().MoveShip (localHex, null, null);

													//invoke on OnSignalMove event, without any towed unit
													OnSignalMovement.Invoke (selectedUnit.GetComponent<Ship> (), localHex, null);


												}

											}
											//at this else, we've clicked a tile outside the reachable hexes
											else {

												//clear the selected unit once we click a non-unit
												ClearSelectedUnit ();

											}

										}  //end if the selected unit is a ship

									}  //end if the selected unit is not null

								}  //end if the hovered object is not a combat unit

							}  //end if a ship is not moving

						} //end if action mode is movement

						//this next section handles mouse click events when we are in TractorBeam mode
						else if (gameManager.CurrentActionMode == GameManager.ActionMode.TractorBeam) {

							//check if we are clicking on the currenet selected unit.  we want to be able to clear the unit by clicking a 2nd time on it
							if (hoveredObject == null || hoveredObject == selectedUnit) {
							
								ClearSelectedUnit ();
								//Debug.Log ("hovered unit == selectedUnit, clear selected unit");

								//at this check, we know we're not hovering over the selected unit, so if it's a combat unit it's a different unit
							} else if (hoveredObject.GetComponent<CombatUnit> () == true) {

								//if the hoveredUnit is a ship that's moving, we don't want to be able to select it while it's moving
								//because that could screw up the movement points math
								//check to see if the hovered unit is a ship first
								if (hoveredObject.GetComponentInChildren<Ship> () == true) {

									//before setting selected unit, verify that hovered unit is not moving - that would screw things up
									if (hoveredObject.GetComponentInChildren<EngineSection> ().isMoving == false) {

										//check if we currently have a selected unit or not
										if (selectedUnit == null) {
										
											SetSelectedUnit (hoveredObject);

										}

										//else, we do have a selected unit
										else {
										
											//check if the unit we're hovering over is inside the targetable hexes
											if (selectedUnit.GetComponent<PhasorSection> ().TargetableTractorBeamHexes.Contains (localHex) == true) {

												//check if the hovered object is already the targeted unit
												if (targetedUnit == hoveredObject) {

													//if so, clear it
													ClearTargetedUnit ();

												} else {


													//target the ship
													SetTargetedUnit (hoveredObject);

												}

											}
											//if the unit we're hovering over is outside the targetable hexes, then we want to select it
											else if (selectedUnit.GetComponent<PhasorSection> ().TargetableTractorBeamHexes.Contains (localHex) == false) {

												//Debug.Log ("Found Combat Unit!");
												SetSelectedUnit (hoveredObject);

											}

										}

									}

								} 

								//this else must be a non-ship combat unit, which would be a starbase
								else if (hoveredObject.GetComponentInChildren<Starbase> () == true) {

									//Debug.Log ("Found Combat Unit!");
									SetSelectedUnit (hoveredObject);

								}

							}

							//if we are here, we have clicked on something that's not another combat unit
							else if (hoveredObject.GetComponent<CombatUnit> () == false) {

								//clear the selected unit once we click a non-unit
								ClearSelectedUnit ();
								//Debug.Log ("hovered unit == not combat unit, clear selected unit");

							}
							
						} 
						//this next section covers click events if we are in phasor attack mode
						else if (gameManager.CurrentActionMode == GameManager.ActionMode.PhasorAttack) {

							//check if we are clicking on the currenet selected unit.  we want to be able to clear the unit by clicking a 2nd time on it
							if (hoveredObject == null || hoveredObject == selectedUnit) {

								ClearSelectedUnit ();

								//at this check, we know we're not hovering over the selected unit, so if it's a combat unit it's a different unit
							} else if (hoveredObject.GetComponent<CombatUnit> () == true) {

								//if the hoveredUnit is a ship that's moving, we don't want to be able to select it while it's moving
								//because that could screw up the movement points math
								//check to see if the hovered unit is a ship first
								if (hoveredObject.GetComponentInChildren<Ship> () == true) {

									//before setting selected unit, verify that hovered unit is not moving - that would screw things up
									if (hoveredObject.GetComponentInChildren<EngineSection> ().isMoving == false) {

										//check if we currently have a selected unit or not
										if (selectedUnit == null) {

											SetSelectedUnit (hoveredObject);

										}

										//else, we do have a selected unit
										else {

											//check if the selected unit is a ship
											if (selectedUnit.GetComponent<Ship> () == true) {

												//check if the unit we're hovering over is inside the targetable hexes
												if (selectedUnit.GetComponent<PhasorSection> ().TargetablePhasorHexes.Contains (localHex) == true) {


													//check if the hovered object is already the targeted unit
													if (targetedUnit == hoveredObject) {

														//if so, clear it
														ClearTargetedUnit ();

													} else {


														//target the ship with phasors
														SetTargetedUnit (hoveredObject);

													}

												}

												//if the unit we're hovering over is outside the targetable hexes, then we want to select it
												else if (selectedUnit.GetComponent<PhasorSection> ().TargetablePhasorHexes.Contains (localHex) == false) {

													SetSelectedUnit (hoveredObject);

												}

											}
											//the else condition is that the selected unit is a starbase
											else if (selectedUnit.GetComponent<Starbase> () == true) {

												//check if the unit we're hovering over is inside the targetable hexes
												//we can use either starbase phasor section to check
												if (selectedUnit.GetComponent<StarbasePhasorSection1> ().TargetablePhasorHexes.Contains (localHex) == true) {

													//check if the hovered object is already the targeted unit
													if (targetedUnit == hoveredObject) {

														//if so, clear it
														ClearTargetedUnit ();

													} else {


														//target the ship with phasors
														SetTargetedUnit (hoveredObject);

													}

												}

												//if the unit we're hovering over is outside the targetable hexes, then we want to select it
												else if (selectedUnit.GetComponent<StarbasePhasorSection1> ().TargetablePhasorHexes.Contains (localHex) == false) {

													SetSelectedUnit (hoveredObject);

												}

											}

										}

									}

								} 

								//this else must be a non-ship combat unit, which would be a starbase
								else if (hoveredObject.GetComponentInChildren<Starbase> () == true) {
									
									//check if we currently have a selected unit or not
									if (selectedUnit == null) {

										SetSelectedUnit (hoveredObject);

									}

									//else, we do have a selected unit
									else {

										//check if the selected unit is a ship
										if (selectedUnit.GetComponent<Ship> () == true) {

											//check if the unit we're hovering over is inside the targetable hexes
											if (selectedUnit.GetComponent<PhasorSection> ().TargetablePhasorHexes.Contains (localHex) == true) {

												//check if the hovered object is already the targeted unit
												if (targetedUnit == hoveredObject) {

													//if so, clear it
													ClearTargetedUnit ();

												} else {


													//target the ship
													SetTargetedUnit (hoveredObject);

												}

											}

											//if the unit we're hovering over is outside the targetable hexes, then we want to select it
											else if (selectedUnit.GetComponent<PhasorSection> ().TargetablePhasorHexes.Contains (localHex) == false) {

												SetSelectedUnit (hoveredObject);

											}

										}
										//the else condition is that the selected unit is a starbase
										else if (selectedUnit.GetComponent<Starbase> () == true) {

											//check if the unit we're hovering over is inside the targetable hexes
											//we can use either starbase phasor section to check
											if (selectedUnit.GetComponent<StarbasePhasorSection1> ().TargetablePhasorHexes.Contains (localHex) == true) {

												//check if the hovered object is already the targeted unit
												if (targetedUnit == hoveredObject) {

													//if so, clear it
													ClearTargetedUnit ();

												} else {


													//target the ship
													SetTargetedUnit (hoveredObject);

												}

											}

											//if the unit we're hovering over is outside the targetable hexes, then we want to select it
											else if (selectedUnit.GetComponent<StarbasePhasorSection1> ().TargetablePhasorHexes.Contains (localHex) == false) {

												SetSelectedUnit (hoveredObject);

											}

										}

									}

								}

							}

							//if we are here, we have clicked on something that's not another combat unit
							else if (hoveredObject.GetComponent<CombatUnit> () == false) {

								//clear the selected unit once we click a non-unit
								ClearSelectedUnit ();

							}

						}
						//this next section covers click events if we are in torpedo attack mode
						else if (gameManager.CurrentActionMode == GameManager.ActionMode.TorpedoAttack) {

							//check if we are clicking on the currenet selected unit.  we want to be able to clear the unit by clicking a 2nd time on it
							if (hoveredObject == null || hoveredObject == selectedUnit) {

								ClearSelectedUnit ();

								//at this check, we know we're not hovering over the selected unit, so if it's a combat unit it's a different unit
							} else if (hoveredObject.GetComponent<CombatUnit> () == true) {

								//if the hoveredUnit is a ship that's moving, we don't want to be able to select it while it's moving
								//because that could screw up the movement points math
								//check to see if the hovered unit is a ship first
								if (hoveredObject.GetComponentInChildren<Ship> () == true) {

									//before setting selected unit, verify that hovered unit is not moving - that would screw things up
									if (hoveredObject.GetComponentInChildren<EngineSection> ().isMoving == false) {

										//check if we currently have a selected unit or not
										if (selectedUnit == null) {

											SetSelectedUnit (hoveredObject);

										}

										//else, we do have a selected unit
										else {

											//check if the selected unit is a ship
											if (selectedUnit.GetComponent<Ship> () == true) {

												//check if the unit we're hovering over is inside the targetable hexes
												if (selectedUnit.GetComponent<TorpedoSection> ().TargetableTorpedoHexes.Contains (localHex) == true) {

													//check if the hovered object is already the targeted unit
													if (targetedUnit == hoveredObject) {

														//if so, clear it
														ClearTargetedUnit ();

													} else {


														//target the ship
														SetTargetedUnit (hoveredObject);

													}

												}

												//if the unit we're hovering over is outside the targetable hexes, then we want to select it
												else if (selectedUnit.GetComponent<TorpedoSection> ().TargetableTorpedoHexes.Contains (localHex) == false) {

													SetSelectedUnit (hoveredObject);

												}

											}
											//the else condition is that the selected unit is a starbase
											else if (selectedUnit.GetComponent<Starbase> () == true) {
												
												//check if the unit we're hovering over is inside the targetable hexes
												if (selectedUnit.GetComponent<StarbaseTorpedoSection> ().TargetableTorpedoHexes.Contains (localHex) == true) {

													//check if the hovered object is already the targeted unit
													if (targetedUnit == hoveredObject) {

														//if so, clear it
														ClearTargetedUnit ();

													} else {


														//target the ship
														SetTargetedUnit (hoveredObject);

													}

												}

												//if the unit we're hovering over is outside the targetable hexes, then we want to select it
												else if (selectedUnit.GetComponent<StarbaseTorpedoSection> ().TargetableTorpedoHexes.Contains (localHex) == false) {

													SetSelectedUnit (hoveredObject);

												}

											}

										}

									}

								} 

								//this else must be a non-ship combat unit, which would be a starbase
								else if (hoveredObject.GetComponentInChildren<Starbase> () == true) {
									
									//check if we currently have a selected unit or not
									if (selectedUnit == null) {

										SetSelectedUnit (hoveredObject);

									}

									//else, we do have a selected unit
									else {

										//check if the selected unit is a ship
										if (selectedUnit.GetComponent<Ship> () == true) {

											//check if the unit we're hovering over is inside the targetable hexes
											if (selectedUnit.GetComponent<TorpedoSection> ().TargetableTorpedoHexes.Contains (localHex) == true) {

												//check if the hovered object is already the targeted unit
												if (targetedUnit == hoveredObject) {

													//if so, clear it
													ClearTargetedUnit ();

												} else {


													//target the ship
													SetTargetedUnit (hoveredObject);

												}

											}

											//if the unit we're hovering over is outside the targetable hexes, then we want to select it
											else if (selectedUnit.GetComponent<TorpedoSection> ().TargetableTorpedoHexes.Contains (localHex) == false) {

												SetSelectedUnit (hoveredObject);

											}

										}
										//the else condition is that the selected unit is a starbase
										else if (selectedUnit.GetComponent<Starbase> () == true) {

											//check if the unit we're hovering over is inside the targetable hexes
											if (selectedUnit.GetComponent<StarbaseTorpedoSection> ().TargetableTorpedoHexes.Contains (localHex) == true) {

												//check if the hovered object is already the targeted unit
												if (targetedUnit == hoveredObject) {

													//if so, clear it
													ClearTargetedUnit ();

												} else {


													//target the ship
													SetTargetedUnit (hoveredObject);

												}

											}

											//if the unit we're hovering over is outside the targetable hexes, then we want to select it
											else if (selectedUnit.GetComponent<StarbaseTorpedoSection> ().TargetableTorpedoHexes.Contains (localHex) == false) {

												SetSelectedUnit (hoveredObject);

											}

										}

									}

								}

							}

							//if we are here, we have clicked on something that's not another combat unit
							else if (hoveredObject.GetComponent<CombatUnit> () == false) {

								//clear the selected unit once we click a non-unit
								ClearSelectedUnit ();

							}

						}
						//this next section covers click events if we are in item use mode
						else if (gameManager.CurrentActionMode == GameManager.ActionMode.ItemUse) {

							//check if we are clicking on the currenet selected unit
							//because a ship can use an item on itself, we don't want to clear the selected unit if we click on it like 
							//we would for an attack.  Instead, the behaviour we want is to make the selected unit the targeted unit
							//then, if we click on the unit again while it's already the targeted unit, we would clear it from selected and targeted status
							if (hoveredObject == null) {

								ClearSelectedUnit ();

							} else if (hoveredObject == selectedUnit && hoveredObject == targetedUnit) {

								ClearSelectedUnit ();

								//at this check, we know we're hovering over a valid target, although it could be ourself
							} else if (hoveredObject.GetComponent<CombatUnit> () == true) {

								//if the hoveredUnit is a ship that's moving, we don't want to be able to select it while it's moving
								//because that could screw up the movement points math
								//check to see if the hovered unit is a ship first
								if (hoveredObject.GetComponentInChildren<Ship> () == true) {

									//before setting selected unit, verify that hovered unit is not moving - that would screw things up
									if (hoveredObject.GetComponentInChildren<EngineSection> ().isMoving == false) {

										//check if we currently have a selected unit or not
										if (selectedUnit == null) {

											SetSelectedUnit (hoveredObject);

										}

										//else, we do have a selected unit
										else {

											//check if the selected unit is a ship
											if (selectedUnit.GetComponent<Ship> () == true) {

												//check if the unit we're hovering over is inside the targetable hexes
												if (selectedUnit.GetComponent<StorageSection> ().TargetableItemHexes.Contains (localHex) == true) {

													//check if the hovered object is already the targeted unit
													if (targetedUnit == hoveredObject) {

														//if so, clear it
														ClearTargetedUnit ();

													} else {


														//target the ship
														SetTargetedUnit (hoveredObject);

													}

												}

												//if the unit we're hovering over is outside the targetable hexes, then we want to select it
												else if (selectedUnit.GetComponent<StorageSection> ().TargetableItemHexes.Contains (localHex) == false) {

													SetSelectedUnit (hoveredObject);

												}

											}
											//the else condition is that the selected unit is a starbase
											else if (selectedUnit.GetComponent<Starbase> () == true) {

												//check if the unit we're hovering over is inside the targetable hexes
												//we can use either starbase storage section to check the range
												if (selectedUnit.GetComponent<StarbaseStorageSection1> ().TargetableItemHexes.Contains (localHex) == true) {

													//check if the hovered object is already the targeted unit
													if (targetedUnit == hoveredObject) {

														//if so, clear it
														ClearTargetedUnit ();

													} else {


														//target the ship
														SetTargetedUnit (hoveredObject);

													}

												}

												//if the unit we're hovering over is outside the targetable hexes, then we want to select it
												else if (selectedUnit.GetComponent<StarbaseStorageSection1> ().TargetableItemHexes.Contains (localHex) == false) {

													SetSelectedUnit (hoveredObject);

												}

											}

										}

									}

								} 

								//this else must be a non-ship combat unit, which would be a starbase
								else if (hoveredObject.GetComponentInChildren<Starbase> () == true) {
									//check if we currently have a selected unit or not
									if (selectedUnit == null) {

										SetSelectedUnit (hoveredObject);

									}

									//else, we do have a selected unit
									else {
										
										//check if the selected unit is a ship
										if (selectedUnit.GetComponent<Ship> () == true) {

											//check if the unit we're hovering over is inside the targetable hexes
											if (selectedUnit.GetComponent<StorageSection> ().TargetableItemHexes.Contains (localHex) == true) {

												//check if the hovered object is already the targeted unit
												if (targetedUnit == hoveredObject) {

													//if so, clear it
													ClearTargetedUnit ();

												} else {


													//target the ship
													SetTargetedUnit (hoveredObject);

												}

											}

											//if the unit we're hovering over is outside the targetable hexes, then we want to select it
											else if (selectedUnit.GetComponent<StorageSection> ().TargetableItemHexes.Contains (localHex) == false) {

												SetSelectedUnit (hoveredObject);

											}

										}
										//the else condition is that the selected unit is a starbase
										else if (selectedUnit.GetComponent<Starbase> () == true) {

											//check if the unit we're hovering over is inside the targetable hexes
											//we can use either starbase storage section to check the range
											if (selectedUnit.GetComponent<StarbaseStorageSection1> ().TargetableItemHexes.Contains (localHex) == true) {

												//check if the hovered object is already the targeted unit
												if (targetedUnit == hoveredObject) {

													//if so, clear it
													ClearTargetedUnit ();

												} else {


													//target the ship
													SetTargetedUnit (hoveredObject);

												}

											}

											//if the unit we're hovering over is outside the targetable hexes, then we want to select it
											else if (selectedUnit.GetComponent<StarbaseStorageSection1> ().TargetableItemHexes.Contains (localHex) == false) {

												SetSelectedUnit (hoveredObject);

											}

										}

									}

								}

							}

							//if we are here, we have clicked on something that's not another combat unit
							else if (hoveredObject.GetComponent<CombatUnit> () == false) {

								//clear the selected unit once we click a non-unit
								ClearSelectedUnit ();

							}

						}
						//this next section covers click events if we are in crew mode
						else if (gameManager.CurrentActionMode == GameManager.ActionMode.Crew) {

							//check if we are clicking on the currenet selected unit
							//because a ship can use repair crew on itself, we don't want to clear the selected unit if we click on it like 
							//we would for an attack.  Instead, the behaviour we want is to make the selected unit the targeted unit
							//then, if we click on the unit again while it's already the targeted unit, we would clear it from selected and targeted status
							if (hoveredObject == null) {

								ClearSelectedUnit ();

							} else if (hoveredObject == selectedUnit && hoveredObject == targetedUnit) {

								ClearSelectedUnit ();

								//at this check, we know we're hovering over a valid target, although it could be ourself
							} else if (hoveredObject.GetComponent<CombatUnit> () == true) {

								//if the hoveredUnit is a ship that's moving, we don't want to be able to select it while it's moving
								//because that could screw up the movement points math
								//check to see if the hovered unit is a ship first
								if (hoveredObject.GetComponentInChildren<Ship> () == true) {

									//before setting selected unit, verify that hovered unit is not moving - that would screw things up
									if (hoveredObject.GetComponentInChildren<EngineSection> ().isMoving == false) {

										//check if we currently have a selected unit or not
										if (selectedUnit == null) {

											SetSelectedUnit (hoveredObject);

										}

										//else, we do have a selected unit
										else {

											//check if the selected unit is a ship
											if (selectedUnit.GetComponent<Ship> () == true) {

												//check if the unit we're hovering over is inside the targetable hexes
												if (selectedUnit.GetComponent<CrewSection> ().TargetableCrewHexes.Contains (localHex) == true) {

													//check if the hovered object is already the targeted unit
													if (targetedUnit == hoveredObject) {

														//if so, clear it
														ClearTargetedUnit ();

													} else {


														//target the ship
														SetTargetedUnit (hoveredObject);

													}

												}

												//if the unit we're hovering over is outside the targetable hexes, then we want to select it
												else if (selectedUnit.GetComponent<CrewSection> ().TargetableCrewHexes.Contains (localHex) == false) {

													SetSelectedUnit (hoveredObject);

												}

											}
											//the else condition is that the selected unit is a starbase
											else if (selectedUnit.GetComponent<Starbase> () == true) {

												//check if the unit we're hovering over is inside the targetable hexes
												if (selectedUnit.GetComponent<StarbaseCrewSection> ().TargetableCrewHexes.Contains (localHex) == true) {

													//check if the hovered object is already the targeted unit
													if (targetedUnit == hoveredObject) {

														//if so, clear it
														ClearTargetedUnit ();

													} else {


														//target the ship
														SetTargetedUnit (hoveredObject);

													}

												}

												//if the unit we're hovering over is outside the targetable hexes, then we want to select it
												else if (selectedUnit.GetComponent<StarbaseCrewSection> ().TargetableCrewHexes.Contains (localHex) == false) {

													SetSelectedUnit (hoveredObject);

												}

											}

										}

									}

								} 

								//this else must be a non-ship combat unit, which would be a starbase
								else if (hoveredObject.GetComponentInChildren<Starbase> () == true) {

									//check if we currently have a selected unit or not
									if (selectedUnit == null) {

										SetSelectedUnit (hoveredObject);

									}

									//else, we do have a selected unit
									else {

										//check if the selected unit is a ship
										if (selectedUnit.GetComponent<Ship> () == true) {

											//check if the unit we're hovering over is inside the targetable hexes
											if (selectedUnit.GetComponent<CrewSection> ().TargetableCrewHexes.Contains (localHex) == true) {

												//check if the hovered object is already the targeted unit
												if (targetedUnit == hoveredObject) {

													//if so, clear it
													ClearTargetedUnit ();

												} else {


													//target the ship
													SetTargetedUnit (hoveredObject);

												}

											}

											//if the unit we're hovering over is outside the targetable hexes, then we want to select it
											else if (selectedUnit.GetComponent<CrewSection> ().TargetableCrewHexes.Contains (localHex) == false) {

												SetSelectedUnit (hoveredObject);

											}

										}
										//the else condition is that the selected unit is a starbase
										else if (selectedUnit.GetComponent<Starbase> () == true) {

											//check if the unit we're hovering over is inside the targetable hexes
											if (selectedUnit.GetComponent<StarbaseCrewSection> ().TargetableCrewHexes.Contains (localHex) == true) {

												//check if the hovered object is already the targeted unit
												if (targetedUnit == hoveredObject) {

													//if so, clear it
													ClearTargetedUnit ();

												} else {


													//target the ship
													SetTargetedUnit (hoveredObject);

												}

											}

											//if the unit we're hovering over is outside the targetable hexes, then we want to select it
											else if (selectedUnit.GetComponent<StarbaseCrewSection> ().TargetableCrewHexes.Contains (localHex) == false) {

												SetSelectedUnit (hoveredObject);

											}

										}

									}

								}

							}

							//if we are here, we have clicked on something that's not another combat unit
							else if (hoveredObject.GetComponent<CombatUnit> () == false) {

								//clear the selected unit once we click a non-unit
								ClearSelectedUnit ();

							}

						} 
						//the cloaking mode should behave just like the selection mode
						else if (gameManager.CurrentActionMode == GameManager.ActionMode.Cloaking) {

							//if we are hovering over nothing and click, clear the current selection
							if (hoveredObject == null) {

								ClearSelectedUnit ();

							}

							//check if we are clicking on the currenet selected unit.  we want to be able to clear the unit by clicking a 2nd time on it
							else if (hoveredObject == selectedUnit) {

								ClearSelectedUnit ();
								//Debug.Log ("hovered unit == selectedUnit, clear selected unit");

							}

							//if we are here, we have clicked on something that's not another combat unit
							else if (hoveredObject.GetComponent<CombatUnit> () == false) {

								//clear the selected unit once we click a non-unit
								ClearSelectedUnit ();
								//Debug.Log ("hovered unit == not combat unit, clear selected unit");

							}

							//if we are here, we clicked on something that is another combat unit
							else {

								//if the hoveredUnit is a ship that's moving, we don't want to be able to select it while it's moving
								//because that could screw up the movement points math
								//check to see if the hovered unit is a ship first
								if (hoveredObject.GetComponentInChildren<Ship> () == true) {

									//before setting selected unit, verify that hovered unit is not moving - that would screw things up
									if (hoveredObject.GetComponentInChildren<EngineSection> ().isMoving == false) {

										//Debug.Log ("Found Combat Unit!");
										SetSelectedUnit (hoveredObject);

									}

								} 

								//this else must be a non-ship combat unit
								else if (hoveredObject.GetComponentInChildren<Starbase> () == true) {

									//Debug.Log ("Found Combat Unit!");
									SetSelectedUnit (hoveredObject);

								}

							}

						}

						//the place new unit mode should only respond to clicking on a range tile
						else if (gameManager.CurrentActionMode == GameManager.ActionMode.PlaceNewUnit) {

							switch (gameManager.currentTurn) {

							case Player.Color.Green:

								foreach (Hex possibleHex in tileMap.GreenStartTiles) {

									//check if the start tile is empty
									if (tileMap.HexMap [possibleHex].tileCombatUnit == null) {

										//if it is, check if the local hex (where we clicked) is the possibleHex
										if (possibleHex == localHex) {

											//invoke the placeUnitEvent
											OnPlacedNewUnit.Invoke (localHex);

										}

									} else if (tileMap.ReachableTiles (possibleHex, 1).Count > 0) {

										//check if at least one neighbor tile is open
										//if it is, create a range tile in each available neighbor
										foreach (Hex neighborHex in tileMap.ReachableTiles (possibleHex, 1)) {

											//create a range tile in open neighbor tiles
											if (neighborHex == localHex) {

												//invoke the placeUnitEvent
												OnPlacedNewUnit.Invoke (localHex);

											}

										}

									} 

								}

								break;

							case Player.Color.Purple:

								foreach (Hex possibleHex in tileMap.PurpleStartTiles) {
									//check if the start tile is empty
									if (tileMap.HexMap [possibleHex].tileCombatUnit == null) {

										//if it is, check if the local hex (where we clicked) is the possibleHex
										if (possibleHex == localHex) {

											//invoke the placeUnitEvent
											OnPlacedNewUnit.Invoke (localHex);

										}

									} else if (tileMap.ReachableTiles (possibleHex, 1).Count > 0) {

										//check if at least one neighbor tile is open
										//if it is, create a range tile in each available neighbor
										foreach (Hex neighborHex in tileMap.ReachableTiles (possibleHex, 1)) {

											//create a range tile in open neighbor tiles
											if (neighborHex == localHex) {

												//invoke the placeUnitEvent
												OnPlacedNewUnit.Invoke (localHex);

											}

										}

									} 

								}

								break;

							case Player.Color.Red:

								foreach (Hex possibleHex in tileMap.RedStartTiles) {

									//check if the start tile is empty
									if (tileMap.HexMap [possibleHex].tileCombatUnit == null) {

										//if it is, check if the local hex (where we clicked) is the possibleHex
										if (possibleHex == localHex) {

											//invoke the placeUnitEvent
											OnPlacedNewUnit.Invoke (localHex);

										}

									} else if (tileMap.ReachableTiles (possibleHex, 1).Count > 0) {

										//check if at least one neighbor tile is open
										//if it is, create a range tile in each available neighbor
										foreach (Hex neighborHex in tileMap.ReachableTiles (possibleHex, 1)) {

											//create a range tile in open neighbor tiles
											if (neighborHex == localHex) {

												//invoke the placeUnitEvent
												OnPlacedNewUnit.Invoke (localHex);

											}

										}

									} 

								}

								break;

							case Player.Color.Blue:

								foreach (Hex possibleHex in tileMap.BlueStartTiles) {

									//check if the start tile is empty
									if (tileMap.HexMap [possibleHex].tileCombatUnit == null) {

										//if it is, check if the local hex (where we clicked) is the possibleHex
										if (possibleHex == localHex) {

											//invoke the placeUnitEvent
											OnPlacedNewUnit.Invoke (localHex);

										}

									} else if (tileMap.ReachableTiles (possibleHex, 1).Count > 0) {

										//check if at least one neighbor tile is open
										//if it is, create a range tile in each available neighbor
										foreach (Hex neighborHex in tileMap.ReachableTiles (possibleHex, 1)) {

											//create a range tile in open neighbor tiles
											if (neighborHex == localHex) {

												//invoke the placeUnitEvent
												OnPlacedNewUnit.Invoke (localHex);

											}

										}

									} 

								}

								break;

							default:

								break;

							}

						}


					}	//this closese the click event if

					//this section of code allows the mouse scroll wheel to zoom in and zoom out
					AdjustMainCameraZoom (worldRoundedCoordinatesV3);

				}     //this closes the raycast hit if within the main viewport

				//this block checks if we are dragging.  If so, we want to manipulate the camera
				if (mouseDragging == true) {

					//set the drag offset - this is the difference from where we started dragging to where we our cursor is now
					dragOffset = dragAnchor - hitInfo.point;

					//move the camera position based on the drag offset
					Camera.main.transform.position = Vector3.MoveTowards (Camera.main.transform.position, Camera.main.transform.position + dragOffset, dragSpeed * Time.deltaTime);
					//Camera.main.transform.position = Camera.main.transform.position + dragOffset;

					//clamp the camera position based on the map edge
					ClampCamera (tileMap);

				}

			} //this closes the eventSystem if
				
		}   //this closes the within the main viewport if

			
		//this else if checks if we are in the minimap camera viewport
		else if (mousePosition.x >= miniMapCamera.pixelRect.xMin && mousePosition.x <= miniMapCamera.pixelRect.xMax && mousePosition.y >= miniMapCamera.pixelRect.yMin && mousePosition.y <= miniMapCamera.pixelRect.yMax) {

			//we are in the minimap camera viewport
			//Debug.Log("MiniMap Viewport");

			//check if the pause fade panel is enabled - we only want to be able to move the mini-map if it is off
			if (pauseFadePanel.activeSelf == false && pauseFadePanel2.activeSelf == false) {

				//use viewportPointToRay instead of ScreenPoint so that stuff behind the sidebar gui isn't affected
				viewPortPoint = miniMapCamera.ScreenToViewportPoint (Input.mousePosition);
				ray = miniMapCamera.ViewportPointToRay (viewPortPoint);

				//if ray hits a collider, return hitInfo.  Ray is shot to infinity
				//checking that viewport point is within the viewport will make sure things have to be on camera to work
				if (Physics.Raycast (ray, out hitInfo, Mathf.Infinity) && viewPortPoint.x <= 1.0f && viewPortPoint.x >= 0.0f && viewPortPoint.y <= 1.0f && viewPortPoint.y >= 0.0f) {

					//check to see if we've clicked the mouse button over the minimap
					if (Input.GetMouseButton (0) == true) {

						//set main camera transform position equal to the raycast hit point location
						Camera.main.transform.position = hitInfo.point;

						//clamp the camera position based on the map edge
						ClampCamera (tileMap);

					}

					//adjust the main camera zoom if the mouse wheel axis is being adjusted
					AdjustMainCameraZoom (new Vector3 (hitInfo.point.x, Camera.main.transform.position.y, hitInfo.point.z));
					
				}

			}
		
		}  //this closes the within the minimap viewport if
		else {

			//if the mouse is not over the main camera viewport, clear the hovered object
			ClearHoveredObject ();

		}

		//this section resets the mouse drag counters if we released the button
		//check to see if we've released the mouse button this frame
		if (Input.GetMouseButtonUp (0) == true) {

			//check to see if we're dragging
			if (mouseDragging == true) {

				//release the dragging
				mouseDragging = false;

				//Debug.Log ("mousedrag off");

				//reset the timer
				mouseClickTimer = 0.0f;

			}

		}

		//check if the mouse button is not held down
		if (Input.GetMouseButton (0) == false) {

			//if the button is not down, dragging should be off
			mouseDragging = false;

		}

	}

	//this method sets the object we are hovering over with the mouse
	private void SetHoveredObject (GameObject gameObjectHit){
		
		//check that the current hovered unit is not null
		//if it is null, it sets the hovered unit to what we hit with the raycast
		//if it isn't null, it checks if the current hovered unit is the same as the game object hit
		//if they're the same, it doesn't change anything and returns
		//but if they are different, it first clears the current hovered unit variable
		//then sets the current hovered unit to the game object hit
		if (hoveredObject != null) {
			if (gameObjectHit == hoveredObject) {
				
				return;
			}

			ClearHoveredObject ();

		}

		//set the hovered object
		hoveredObject = gameObjectHit;

		//invoke the OnSetHoveredObject event
		OnSetHoveredObject.Invoke();
	}

	//this method clears the hovered unit. 
	private void ClearHoveredObject(){
		
		//It checks to see if the hovered unit is already null
		//if it is, it returns without doing anything.
		//if it isn't, it sets it to null
		if (hoveredObject == null) {
			
			return;

		}

		//clear the hovered object
		hoveredObject = null;

		//invoke the OnClearHoveredObject event
		OnClearHoveredObject.Invoke();

	}


	//this method sets the object we have selected
	private void SetSelectedUnit(GameObject localHoveredObject){

		//Debug.Log ("SetSelectedUnit");

		//check that the current selected unit is not null
		//if it is null, it sets the selected unit to what we are hovering over
		//if it isn't null, it checks if the current selected unit is the same as what we are hovering over
		//if they're the same, it doesn't change anything and returns
		//but if they are different, it first clears the current selected unit variable
		//then sets the current selected unit to the unit we are hovering over
		if (selectedUnit != null) {
			if (localHoveredObject == selectedUnit) {
				
				return;

			}

			ClearSelectedUnit ();

		}

		//only do selection if it's a combat unit - we don't want to grab the tilemap itself
		if (localHoveredObject.GetComponent<CombatUnit>() == true) {

			//set the selected object to be the hovered object
			selectedUnit = localHoveredObject;

			//we want to lock out the action menu if the selected combat unit is not the current player's unit
			//TODO - I am pretty sure this check can be deleted once all the individual toggle scripts are in place
			if (selectedUnit.GetComponent<CombatUnit> ().owner != gameManager.currentTurnPlayer) {

				//invoke the OnSelectForeignUnit event
				OnSelectForeignUnit.Invoke();

			}
				
		}

		//we need to check if the unit is eligible to be in the current action mode, or whether we need to break out of the current action mode
		switch (gameManager.CurrentActionMode) {

		case GameManager.ActionMode.Movement:

			//check if the unit has an engine section
			if (selectedUnit.GetComponent<EngineSection> () == null) {

				//invoke the invalid event
				OnInvalidActionModeForSelectedUnit.Invoke ();


			}

			break;

		case GameManager.ActionMode.PhasorAttack:

			//check if the unit has a phasor section
			if (selectedUnit.GetComponent<PhasorSection> () == null && selectedUnit.GetComponent<StarbasePhasorSection1> () == null 
				&& selectedUnit.GetComponent<StarbasePhasorSection2> () == null) {

				//invoke the invalid event
				OnInvalidActionModeForSelectedUnit.Invoke ();


			}

			break;

		case GameManager.ActionMode.TorpedoAttack:

			//check if the unit has a torpedo section
			if (selectedUnit.GetComponent<TorpedoSection> () == null && selectedUnit.GetComponent<StarbaseTorpedoSection> () == null) {

				//invoke the invalid event
				OnInvalidActionModeForSelectedUnit.Invoke ();


			}

			break;

		case GameManager.ActionMode.TractorBeam:

			//check if the unit has a phasor section
			if (selectedUnit.GetComponent<PhasorSection> () == null && selectedUnit.GetComponent<StarbasePhasorSection1> () == null 
				&& selectedUnit.GetComponent<StarbasePhasorSection2> () == null) {

				//invoke the invalid event
				OnInvalidActionModeForSelectedUnit.Invoke ();


			}

			break;

		case GameManager.ActionMode.ItemUse:

			//check if the unit has a storage section
			if (selectedUnit.GetComponent<StorageSection> () == null && selectedUnit.GetComponent<StarbaseStorageSection1> () == null 
				&& selectedUnit.GetComponent<StarbaseStorageSection2> () == null) {

				//invoke the invalid event
				OnInvalidActionModeForSelectedUnit.Invoke ();


			}

			break;

		case GameManager.ActionMode.Crew:

			//check if the unit has a torpedo section
			if (selectedUnit.GetComponent<CrewSection> () == null && selectedUnit.GetComponent<StarbaseCrewSection> () == null) {

				//invoke the invalid event
				OnInvalidActionModeForSelectedUnit.Invoke ();


			}

			break;

		case GameManager.ActionMode.Cloaking:

			//check if the unit has a cloaking device
			if (selectedUnit.GetComponent<CloakingDevice> () == null) {

				//invoke the invalid event
				OnInvalidActionModeForSelectedUnit.Invoke ();


			}

			break;

		default:

			break;

		}




		//i am pretty sure that whenever we set a new selected unit, I want to clear the targeted unit if there is one
		if (targetedUnit != null) {
			
			ClearTargetedUnit ();

		}
		//Debug.Log ("Set Selected Unit to" + selectedUnit.GetComponent<Starbase>().baseName);
		//invoke the OnSetSelectedUnit event
		OnSetSelectedUnitEarly.Invoke();
		OnSetSelectedUnit.Invoke();

	}

	//this method clears the selected unit. 
	private void ClearSelectedUnit(){

		//Debug.Log ("ClearSelectedUnit");


		//if we are clearing the selected unit, we will also want to clear the targeted unit if there is one
		if (targetedUnit != null) {
			
			ClearTargetedUnit ();

		}

		//It checks to see if the selected unit is already null
		//if it is, it returns without doing anything.
		//if it isn't, it sets it to null
		if (selectedUnit == null) {
			
			return;

		}
			
		selectedUnit = null;

		//invoke the OnClearSelectedUnit event
		OnClearSelectedUnit.Invoke();

	}

	//create a function to set the targeted unit
	private void SetTargetedUnit(GameObject hoveredObject){

		//first check to make sure the targetedUnit is not null, meaning there is a current targeted unit
		if (targetedUnit != null) {

			//if the hovered object is already the targeted unit, keep it that way and return
			if (hoveredObject == targetedUnit) {

				return;

			}

			//if the hovered object is not the same as the targeted, first we want to clear the current targeted unit
			ClearTargetedUnit ();

		}
			

		//only do targeting if it's a combat unit - we don't want to grab the tilemap itself
		if (hoveredObject.GetComponentInChildren<CombatUnit> () == true) {

			//set the targeted object to be the hovered object
			targetedUnit = hoveredObject;
			//Debug.Log ("Unit has been targeted");

		}
			
		//invoke the OnSetTargetedUnit Event
		OnSetTargetedUnit.Invoke();
			
	}

	//create a function to clear the targeted unit
	private void ClearTargetedUnit(){

		//Debug.Log ("Called ClearTargetedUnit");

		//It checks to see if the targeted unit is already null
		//if it is, it returns without doing anything.  if it isn't, it sets it to null
		if (targetedUnit == null) {

			//invoke the OnClearTargetedUnit Event
			//OnClearTargetedUnit.Invoke();

			return;

		}

		targetedUnit = null;

		//invoke the OnClearTargetedUnit Event
		OnClearTargetedUnit.Invoke();

	}

	//this function checks if the passed unit is the targeted unit, and if so, clears it
	private void CheckTargetedUnit(CombatUnit combatUnit){

		//check if the targeted unit has been destroyed
		if (targetedUnit == null) {

			ClearTargetedUnit ();

		} else if (targetedUnit.GetComponent<CombatUnit>() == combatUnit) {

			ClearTargetedUnit ();

		}

	}

	//create a function for manipulating the camera zoom
	private void AdjustMainCameraZoom(Vector3 currentMouseWorldCoordinates){

		//the min camera size is a min of 2 values
		//for normal aspect ratios, it will be 1/2 the default size
		//but for super-wide aspect ratios, we have to allow it to get smaller
		//so that the width is equal to the map width
		float minMainCameraSize = Mathf.Min (GameManager.mainCameraDefaultSize / 2.0f, tileMap.maxWidth / Camera.main.aspect / 2.0f);

		//the max camera size is a min of 2 values
		//it checks to make sure that neither the x direction or z direction grow outside the map limits
		float maxMainCameraSize = Mathf.Min (tileMap.maxHeight / 2.0f * (1.0f + tileMap.hexHeight / tileMap.maxHeight), tileMap.maxWidth / Camera.main.aspect / 2.0f * (1.0f + tileMap.hexWidth / tileMap.maxWidth));

		//these floats are used for the calculation which moves the camera toward the zoom target as we zoom in
		float oldOrthographicSize = Camera.main.orthographicSize;
		float oldCameraWorldPositionZ = Camera.main.transform.position.z;
		float oldCameraWorldPositionX = Camera.main.transform.position.x;

		//this line actually changes the size of the zoom
		Camera.main.orthographicSize = Mathf.Clamp (Camera.main.orthographicSize - 
			Camera.main.orthographicSize * Input.GetAxis ("Mouse ScrollWheel") * zoomSpeed * mouseZoomDirection, minMainCameraSize, maxMainCameraSize);

		//here is the math that gets to the new camera positions
		//oldDeltaZ / oldOrthoSize = newDeltaZ / newOrthoSize
		//newDeltaZ = oldDeltaZ * newOrthoSize/oldOrthoSize
		//(newCamPosZ - MouseWorldZ) = (oldCamPosZ - mouseWorldZ) * newOrthoSize/oldOrthoSize
		//newCamPosZ = (oldCamPosZ - mouseWorldZ) * newOrthoSize/oldOrthoSize + MouseWorldZ
		float newCameraWorldPositionZ = (oldCameraWorldPositionZ - currentMouseWorldCoordinates.z) * Camera.main.orthographicSize / oldOrthographicSize + currentMouseWorldCoordinates.z;
		float newCameraWorldPositionX = (oldCameraWorldPositionX - currentMouseWorldCoordinates.x) * Camera.main.orthographicSize / oldOrthographicSize + currentMouseWorldCoordinates.x;

		//I only want to move towards the target on zooming in - therefore scrollwheel has to be positive
		if (Input.GetAxis ("Mouse ScrollWheel") > .01f && Camera.main.orthographicSize > minMainCameraSize) {

			//what I want is that whatever point my mouse is over, when I zoom in, my mouse is still in that same world coordinate spot
			Vector3 cameraMoveTowardsPosition = new Vector3 (newCameraWorldPositionX, Camera.main.transform.position.y, newCameraWorldPositionZ);

			//Camera.main.transform.position = Vector3.MoveTowards (Camera.main.transform.position, cameraMoveTowardsPosition, dragSpeed * Time.deltaTime);
			Camera.main.transform.position = cameraMoveTowardsPosition;

		}
			
		//now we need to pull the camera back towards the center as it zooms out, so we don't see too much of the off-map area
		//check if the camera is at the top border limit
		if (Camera.main.transform.position.z >= tileMap.maxHeight + tileMap.origin.y - Camera.main.orthographicSize) {

			//move camera back towards the middle as it zooms out
			Camera.main.transform.position = new Vector3 (Camera.main.transform.position.x, Camera.main.transform.position.y, tileMap.maxHeight + tileMap.origin.y - Camera.main.orthographicSize);

		}

		//check if the camera is at the bottom border limit
		if (Camera.main.transform.position.z <= tileMap.origin.y + Camera.main.orthographicSize - tileMap.hexHeight) {

			//move camera back towards the middle as it zooms out
			Camera.main.transform.position = new Vector3 (Camera.main.transform.position.x, Camera.main.transform.position.y,tileMap.origin.y + Camera.main.orthographicSize - tileMap.hexHeight);

		}

		//check if the camera is at the right border limit
		if (Camera.main.transform.position.x >= tileMap.maxWidth + tileMap.origin.x - Camera.main.orthographicSize * Camera.main.aspect) {

			//move camera back towards the middle as it zooms out
			Camera.main.transform.position = new Vector3 (tileMap.maxWidth + tileMap.origin.x - Camera.main.orthographicSize * Camera.main.aspect, Camera.main.transform.position.y,Camera.main.transform.position.z);

		}

		//check if the camera is at the left border limit
		if (Camera.main.transform.position.x <= tileMap.origin.x + (Camera.main.orthographicSize * Camera.main.aspect) - tileMap.hexWidth) {

			//move camera back towards the middle as it zooms out
			Camera.main.transform.position = new Vector3 (tileMap.origin.x + (Camera.main.orthographicSize * Camera.main.aspect) - tileMap.hexWidth, Camera.main.transform.position.y,Camera.main.transform.position.z);

		}

	}

	//this function clamps the camera position so it doesn't go outside the map bounds
	private void ClampCamera(TileMap tileMap){

		//set minimum and maximum camera x and z values
		//these are calculated based on the size of the map so that the camera can only show 1 hex beyond the board in any direction
		float cameraMaxZ = tileMap.maxHeight + tileMap.origin.y - Camera.main.orthographicSize;
		float cameraMinZ = tileMap.origin.y + Camera.main.orthographicSize - tileMap.hexHeight;
		float cameraMaxX = tileMap.maxWidth + tileMap.origin.x - Camera.main.orthographicSize * Camera.main.aspect;
		float cameraMinX = tileMap.origin.x + (Camera.main.orthographicSize  * Camera.main.aspect) - tileMap.hexWidth;

		//Clamp the camera position so it can't get too close to the edge
		Camera.main.transform.position = new Vector3(Mathf.Clamp(Camera.main.transform.position.x,cameraMinX,cameraMaxX),
			Mathf.Clamp(Camera.main.transform.position.y,10.0f,10.0f),
			Mathf.Clamp(Camera.main.transform.position.z,cameraMinZ,cameraMaxZ));

	}

	//this function sets the mouse zoom direction
	private void SetMouseZoomDirection(bool isInverted){

		if (isInverted == true) {

			mouseZoomDirection = -1;

		} else {

			mouseZoomDirection = 1;

		}

	}

	//this function sets the mouse zoom sensitivity
	private void SetMouseZoomSensitivity(int mouseSensitivity){

		//set the zoom speed
		//the 50.0f is based on what I felt like gave a good default zoom speed
		zoomSpeed = mouseSensitivity / 32.0f;

	}

	//function to handle onDestroy
	private void OnDestroy(){

		RemoveAllListeners();

	}

	//function to remove all listeners
	private void RemoveAllListeners(){

		if (uiManager != null) {

			//remove a listener to the NextUnit button event
			uiManager.GetComponent<NextUnit> ().SetNextUnit.RemoveListener (nextUnitSetSelectedUnitAction);

			//remove a listener for turning off the tractor beam toggle so we can clear the targeted unit
			uiManager.GetComponent<TractorBeamToggle> ().OnTurnedOffTractorBeamToggleWhileNotEngaged.RemoveListener (ClearTargetedUnit);

			//remove a listener for turning off the phasor attack toggle so we can clear the targeted unit
			uiManager.GetComponent<PhasorToggle> ().OnTurnedOffPhasorToggle.RemoveListener (ClearTargetedUnit);

			//remove a listener for turning off the torpedo attack toggle so we can clear the targeted unit
			uiManager.GetComponent<TorpedoToggle> ().OnTurnedOffTorpedoToggle.RemoveListener (ClearTargetedUnit);

			//remove a listener for turning off the crew toggle so we can clear the targeted unit
			uiManager.GetComponent<CrewToggle> ().OnTurnedOffCrewToggle.RemoveListener (ClearTargetedUnit);

			//remove a listener for turning off the useItem toggle so we can clear the targeted unit
			uiManager.GetComponent<UseItemToggle> ().OnTurnedOffUseItemToggle.RemoveListener (ClearTargetedUnit);

			//remove a listener for placing a unit
			uiManager.GetComponent<PurchaseManager> ().OnOutfittedShip.RemoveListener (purchaseClearSelectedUnitAction);

			//remove a listener for coming out of flare mode
			uiManager.GetComponent<FlareManager> ().OnUseFlaresYes.RemoveListener (flareDataSetTargetedUnitAction);
			uiManager.GetComponent<FlareManager> ().OnUseFlaresCancel.RemoveListener (flareDataSetTargetedUnitAction);

			//remove listener for mouse zoom direction
			uiManager.GetComponent<Settings> ().OnChangeMouseZoomInversion.RemoveListener (boolSetMouseZoomDirectionAction);

			//remove listener for mouse zoom speed
			uiManager.GetComponent<Settings>().OnChangeMouseZoomSensitivity.RemoveListener(intSetMouseZoomSensitivityAction);

			//remove listeners for keyboard cancels
			uiManager.GetComponent<UINavigationMain>().OnCancelClearTargetedUnit.RemoveListener(ClearTargetedUnit);
			uiManager.GetComponent<UINavigationMain>().OnCancelClearSelectedUnit.RemoveListener(ClearSelectedUnit);

			//remove listener for closing cutscene
			uiManager.GetComponent<CutsceneManager>().OnCloseCutsceneDisplayPanel.RemoveListener(ClearSelectedUnit);

			//remove listener for using repair crew
			uiManager.GetComponent<CrewMenu>().OnUseRepairCrew.RemoveListener(useRepairCrewClearSelectedUnitAction);

			//remove listener for using crystals
			uiManager.GetComponent<UseItemMenu>().OnUseDilithiumCrystal.RemoveListener(useCrystalClearSelectedUnitAction);
			uiManager.GetComponent<UseItemMenu>().OnUseTrilithiumCrystal.RemoveListener(useCrystalClearSelectedUnitAction);
		}

		if (gameManager != null) {
			
			//remove a listener for the EndTurn event - we don't need the currentTurn to pass to the clearSelectedUnit
			gameManager.OnEndTurn.RemoveListener (colorClearSelectedUnitAction);

		}

		//remove a listener for the static DisengageTractorBeam event so we can clear the targeted unit when it happens
		//the event passes the ship that disengaged the tractor beam, but we don't need it to clear the targeted unit
		PhasorSection.OnDisengageTractorBeam.RemoveListener (shipClearTargetedUnitAction);

		//remove listener for ship getting destroyed
		Ship.OnShipDestroyed.RemoveListener(combatUnitCheckTargetedUnitAction);

		//remove listener for base getting destroyed
		Starbase.OnBaseDestroyed.RemoveListener(combatUnitCheckTargetedUnitAction);

	}
		
}

