using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class UINavigationMain : MonoBehaviour {

	public static bool blockPointerClickFlag { get; private set;}

	//this will hold the eventSystem in the scene
	public EventSystem eventSystem;

	//uiManager
	private GameObject uiManager;
	private GameManager gameManager;
	private MouseManager mouseManager;


	//this enum will keep track of the current UI State
	public enum UIState{

		Selection,
		MoveMenu,
		PhaserMenu,
		TorpedoMenu,
		TractorBeamMenu,
		UseItem,
		Crew,
		Cloaking,
		BuyItem,
		BuyShip,
		OutfitShip,
		PlaceNewShip,
		NameNewShip,
		RenameUnit,
		EndTurn,
		UseFlares,
		CombatCutscene,
		Status,
		SaveLocalGame,
		FileOverwritePrompt,
		LoadLocalGame,
		FileDeletePrompt,
		Settings,
		ExitGamePrompt,
		VictoryPanel,

	}

	//this bool will wait a frame when needed
	private int delaySaveFilesWindowCount = 0;
	private int delayLoadFilesWindowCount = 0;
	private int delaySetInitialSelectablesCount = 0;
	private int delayReturnToSelectableCount = 0;
	private int delayReturnToFirstSelectableCount = 0;


	private Selectable returnSelectable;
	//private UIState returnUIState;

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

				//Debug.Log ("UIState = " + currentUIState.ToString ());

				//invoke the event
				OnUIStateChange.Invoke();

			}

		}


	}

	//these public arrays are groups of selectables arrays that make up each UI state
	private Selectable[][] SelectionGroup;
	private Selectable[][] MoveMenuGroup;
	private Selectable[][] PhaserMenuGroup;
	private Selectable[][] TorpedoMenuGroup;
	private Selectable[][] TractorBeamMenuGroup;
	private Selectable[][] UseItemMenuGroup;
	private Selectable[][] CrewMenuGroup;
	private Selectable[][] CloakingMenuGroup;
	private Selectable[][] BuyItemGroup;
	private Selectable[][] BuyShipGroup;
	private Selectable[][] OutfitShipGroup;
	private Selectable[][] PlaceNewShipGroup;
	private Selectable[][] NameNewShipGroup;
	private Selectable[][] RenameShipGroup;
	private Selectable[][] EndTurnGroup;
	private Selectable[][] UseFlaresGroup;
	private Selectable[][] StatusGroup;
	private Selectable[][] SaveLocalGameGroup;
	private Selectable[][] FileOverwritePromptGroup;
	private Selectable[][] LoadLocalGameGroup;
	private Selectable[][] FileDeletePromptGroup;
	private Selectable[][] SettingsGroup;
	private Selectable[][] ExitGamePromptGroup;
	private Selectable[][] VictoryPanelGroup;

	//these public arrays will hold the different selectables arrays
	public Selectable[]	ActionMenuButtons;
	public Selectable[] ChatInputField;
	public Selectable[]	FileMenuButtons;
	public Selectable[] NextUnitButtons;
	public Selectable[] StatusButton;

	public Selectable[] MoveMenuButtons;

	public Selectable[] PhaserRadarShotButton;
	public Selectable[] PhaserTargetingDropdown;
	public Selectable[] PhaserFireButton;

	public Selectable[] TorpedoLaserShotButton;
	public Selectable[] TorpedoTargetingDropdown;
	public Selectable[] TorpedoFireButtons;

	public Selectable[] TractorBeamEngageButton;

	public Selectable[] FlareModeButtons;
	public Selectable[] ItemTargetingDropdown;
	public Selectable[] CrystalButtons;

	public Selectable[] CrewTargetingDropdown;
	public Selectable[] CrewRepairButton;

	public Selectable[] CloakingDeviceEngageButton;

	public Selectable[] BuyPhaserRadarShotButton;
	public Selectable[] BuyPhaserRadarArrayButton;
	public Selectable[] BuyXRayKernelButton;
	public Selectable[] BuyTractorBeamButton;
	public Selectable[] BuyLightTorpedoButton;
	public Selectable[] BuyHeavyTorpedoButton;
	public Selectable[] BuyTorpedoLaserShotButton;
	public Selectable[] BuyTorpedoLaserGuidanceButton;
	public Selectable[] BuyHighPressureTubesButton;
	public Selectable[] BuyDilithiumCrystalButton;
	public Selectable[] BuyTrilithiumCrystalButton;
	public Selectable[] BuyFlareButton;
	public Selectable[] BuyRadarJammingButton;
	public Selectable[] BuyLaserScatteringButton;
	public Selectable[] BuyRepairCrewButton;
	public Selectable[] BuyShieldEngineeringTeamButton;
	public Selectable[] BuyAdditionalBattleCrewButton;
	public Selectable[] BuyWarpBoosterButton;
	public Selectable[] BuyTranswarpBoosterButton;
	public Selectable[] BuyWarpDriveButton;
	public Selectable[] BuyTranswarpDriveButton;
	public Selectable[] BuyItemButtonRow;

	public Selectable[]	BuyShipsButtons;
	public Selectable[] BuyShipButtonRow;

	public Selectable[] PlaceNewShipButtons;

	public Selectable[] NameNewShipInputField;
	public Selectable[] NameNewShipButtons;

	public Selectable[] RenameShipInputField;
	public Selectable[] RenameShipButtons;

	public Selectable[] EndTurnButtons;

	public Selectable[] FlareCountDisplay;
	public Selectable[] FlareAutoCalcButton;
	public Selectable[] UseFlaresButtonRow;

	public Selectable[] StatusButtons;

	public Selectable[] SaveLocalGameFiles;
	public Selectable[] SaveLocalGameInputField;
	public Selectable[] SaveLocalGameButtonsRow;

	public Selectable[] LoadLocalGameFiles;
	public Selectable[] LoadLocalGameButtonsRow;

	public Selectable[] FileDeleteButtons;

	public Selectable[] FileOverwriteButtons;

	public Selectable[] SettingsResolutionDropdown;
	public Selectable[] SettingsResolutionApply;
	public Selectable[] SettingsFullScreenToggle;
	public Selectable[] SettingsMouseZoomInversion;
	public Selectable[] SettingsZoomSensitivity;
	public Selectable[] SettingsScrollSensitivity;
	public Selectable[] SettingsMusicVolume;
	public Selectable[] SettingsSfxVolume;
	public Selectable[] SettingsButtonsRow;

	public Selectable[] ExitGameButtons;

	public Selectable[] VictoryPanelButtons;


	//these hold the sliders so I can reference them
	public Slider ZoomSlider;
	public Slider ScrollSlider;
	public Slider MusicVolumeSlider;
	public Slider SfxVolumeSlider;

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

	//this flag is for a new turn
	private bool isNewTurn = false;

	//this stores the kind of combat we enter
	private UIState combatType;

	//event to announce UIState change
	public UnityEvent OnUIStateChange = new UnityEvent();
	public UnityEvent OnSelectablesChange = new UnityEvent();

	public UnityEvent OnCancelClearTargetedUnit = new UnityEvent();
	public UnityEvent OnCancelClearSelectedUnit = new UnityEvent();


	public SaveFileSelectedEvent OnSelectSaveFile = new SaveFileSelectedEvent();
	public class SaveFileSelectedEvent : UnityEvent<GameObject>{}


	//this event announces a successful pointer enter valid selectable
	public UnityEvent OnPointerEnterValidSelectable = new UnityEvent();

	//this event announces a successful pointer click valid selectable
	public UnityEvent OnPointerClickValidSelectable = new UnityEvent();

	//this event announces a cancellation of a window using the escape key
	public UnityEvent OnCloseWindowWithCancel = new UnityEvent();

	//unityActions
	private UnityAction<Player> NewTurnSetInitialSelectablesAction;
	private UnityAction<bool> MoveToggleSetUIStateAction;
	private UnityAction<bool> PhaserToggleSetUIStateAction;
	private UnityAction<bool> TorpedoToggleSetUIStateAction;
	private UnityAction<bool> TractorBeamToggleSetUIStateAction;
	private UnityAction<bool> UseItemToggleSetUIStateAction;
	private UnityAction<bool> CrewToggleSetUIStateAction;
	private UnityAction<bool> CloakingToggleSetUIStateAction;
	private UnityAction BuyItemSetUIStateAction;
	private UnityAction BuyShipSetUIStateAction;
	private UnityAction<Hex> NameNewShipSetUIStateAction;
	private UnityAction RenameShipSetUIStateAction;
	private UnityAction<bool> EndTurnSetUIStateAction;

	private UnityAction UseFlaresSetUIStateAction;
	private UnityAction CutsceneSetUIStateAction;

	private UnityAction StatusSetUIStateAction;
	private UnityAction CancelStatusAction;

	private UnityAction CancelEndTurnPromptAction;
	private UnityAction AcceptEndTurnPromptAction;
	private UnityAction CancelPurchaseItemAction;
	private UnityAction AcceptPurchaseItemAction;
	private UnityAction CancelPurchaseShipAction;
	private UnityAction AcceptPurchaseShipAction;

	private UnityAction CancelPlaceNewShipAction;

	private UnityAction CancelNameNewShipAction;
	private UnityAction<NameNewShip.NewUnitEventData> AcceptNameNewShipAction;
	private UnityAction<FlareManager.FlareEventData> UseFlaresYesAction;
	private UnityAction<FlareManager.FlareEventData> UseFlaresCancelAction;
	private UnityAction CancelRenameUnitAction;
	private UnityAction AcceptRenameUnitAction;

	private UnityAction<Selectable> SelectableSetSelectionGroupsAction;

	private UnityAction OpenSaveLocalGameWindowAction;
	private UnityAction CloseSaveLocalGameWindowAction;

	private UnityAction<string> OpenFileOverwritePromptAction;
	private UnityAction<string> StringReturnToFileSaveWindowAction;

	private UnityAction OpenLoadLocalGameWindowAction;
	private UnityAction CloseLoadLocalGameWindowAction;

	private UnityAction<string> OpenFileDeletePromptAction;
	private UnityAction<string> StringReturnToFileLoadWindowAction;

	private UnityAction OpenSettingsWindowAction;
	private UnityAction CloseSettingsWindowAction;

	private UnityAction OpenExitGamePromptAction;
	private UnityAction CloseExitGamePromptAction;

	private UnityAction OpenVictoryPanelAction;
	private UnityAction CloseVictoryPanelAction;

	private UnityAction SelectableSetNavigationRulesAction;

	private UnityAction<string> InputFieldEndEditIgnoreEscapeAction;

	private UnityAction<Ship> TractorBeamShipSetUIStateAction;
	private UnityAction ClearSetInitialSelectablesAction;
	private UnityAction<CombatUnit> CombatUnitSetUIStateAction;

	private UnityAction<Selectable> PointerClickResolveBlockAction;

	//these set the combat states
	private UnityAction<CombatUnit,CombatUnit,string> PhaserAttackSetCombatStateToPhaserAction;
	private UnityAction<CombatUnit,CombatUnit,string> TorpedoAttackSetCombatStateToTorpedoAction;
	private UnityAction ReturnToSelectablesFromCombatAction;

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
		gameManager = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameManager> ();
		mouseManager = GameObject.FindGameObjectWithTag ("MouseManager").GetComponent<MouseManager> ();

		//set the actions
		SetUnityActions ();

		//add event listeners
		AddEventListeners();

		//defome the selectable groups
		DefineSelectablesGroups();

		//set the initial UIState
		CurrentUIState = UIState.Selection;

		//invoke the event to start, since it won't fire because the enum defaults to the Selection
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

		//check if we need to wait a frame for the save local game file list to destroy the old list and repopulate a new list
		if (delaySaveFilesWindowCount > 0) {

			delaySaveFilesWindowCount--;

			if (delaySaveFilesWindowCount == 0) {

				CurrentUIState = UIState.SaveLocalGame;
				return;

			}

		}

		//check if we need to wait a frame for the set selectables
		if (delaySetInitialSelectablesCount > 0) {

			delaySetInitialSelectablesCount--;

			if (delaySetInitialSelectablesCount == 0) {

				SetInitialCurrentSelectables ();
				return;

			}

		}

		//check if we need to wait a frame for the first return selectables
		if (delayReturnToFirstSelectableCount > 0) {

			delayReturnToFirstSelectableCount--;

			if (delayReturnToFirstSelectableCount == 0) {

				//zero out the competing counter
				delayReturnToSelectableCount = 0;

				ReturnToFirstSelectableInCurrentSelectables ();
				return;

			}

		} else

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

				//Debug.Log ("test1");

				//the else check is if we have nothing selected or if we have something selected that's not our selectable in memory

				//now check if our memory isn't null
				if (CurrentSelectables != null && CurrentSelectables [currentSelectionIndex] != null) {

					//Debug.Log ("test2");
					//check if the object in memory is either non-interactable or active - ie it is an invalid selection
					if (CurrentSelectables [currentSelectionIndex].IsInteractable () == false || CurrentSelectables [currentSelectionIndex].IsActive () == false) {

						//Debug.Log ("test3");
						//check if we've got shift held for backwards cycling
						if (Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift)) {

							//Debug.Log ("test4");
							//advance to the next group backwards since our current memory is invalid
							AdvanceSelectableGroup (true);

							return;

						} else{

							//Debug.Log ("test5");
							//advance to the next group forward since our current memory is invalid
							AdvanceSelectableGroup (false);

							return;

						}

					} else {

						//Debug.Log ("test6");

						//check if we are in a dropdown - if so, we don't want to set the selected object
						if (CurrentSelectables [currentSelectionIndex].gameObject.GetComponent<TMP_Dropdown> () == true) {

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

				} else if (CurrentUIState == UIState.SaveLocalGame && CurrentSelectables == SaveLocalGameFiles) {

					//here we are selecting a lower file in the scroll rect
					FitCurrentSelectableIntoScrollRect ();

					//call the select save file event, passing the current selectable object
					OnSelectSaveFile.Invoke(CurrentSelectables[currentSelectionIndex].gameObject);

				}

			} else if(CurrentUIState == UIState.PhaserMenu && CurrentSelectables == PhaserTargetingDropdown){

				//adjust the dropdown value
				AdjustDropdownValueDown(PhaserTargetingDropdown[0].GetComponent<TMP_Dropdown>());

			} else if(CurrentUIState == UIState.TorpedoMenu && CurrentSelectables == TorpedoTargetingDropdown){

				//adjust the dropdown value
				AdjustDropdownValueDown(TorpedoTargetingDropdown[0].GetComponent<TMP_Dropdown>());

			} else if(CurrentUIState == UIState.UseItem && CurrentSelectables == ItemTargetingDropdown){

				//adjust the dropdown value
				AdjustDropdownValueDown(ItemTargetingDropdown[0].GetComponent<TMP_Dropdown>());

			} else if(CurrentUIState == UIState.Crew && CurrentSelectables == CrewTargetingDropdown){

				//adjust the dropdown value
				AdjustDropdownValueDown(CrewTargetingDropdown[0].GetComponent<TMP_Dropdown>());

			} else if(CurrentUIState == UIState.BuyItem && CurrentSelectables == BuyPhaserRadarShotButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().phaserRadarShotDownButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.BuyItem && CurrentSelectables == BuyPhaserRadarArrayButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().phaserRadarArrayDownButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.BuyItem && CurrentSelectables == BuyXRayKernelButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().xRayKernelDownButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.BuyItem && CurrentSelectables == BuyTractorBeamButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().tractorBeamDownButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.BuyItem && CurrentSelectables == BuyLightTorpedoButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().lightTorpedoDownButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.BuyItem && CurrentSelectables == BuyHeavyTorpedoButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().heavyTorpedoDownButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.BuyItem && CurrentSelectables == BuyTorpedoLaserShotButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().torpedoLaserShotDownButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.BuyItem && CurrentSelectables == BuyTorpedoLaserGuidanceButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().torpedoLaserGuidanceDownButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.BuyItem && CurrentSelectables == BuyHighPressureTubesButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().highPressureTubesDownButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.BuyItem && CurrentSelectables == BuyDilithiumCrystalButton){

					//click the button
				uiManager.GetComponent<PurchaseManager> ().dilithiumCrystalDownButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.BuyItem && CurrentSelectables == BuyTrilithiumCrystalButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().trilithiumCrystalDownButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.BuyItem && CurrentSelectables == BuyFlareButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().flareDownButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.BuyItem && CurrentSelectables == BuyRadarJammingButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().radarJammingSystemDownButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.BuyItem && CurrentSelectables == BuyLaserScatteringButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().laserScatteringSystemDownButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.BuyItem && CurrentSelectables == BuyRepairCrewButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().repairCrewDownButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.BuyItem && CurrentSelectables == BuyShieldEngineeringTeamButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().shieldEngineeringTeamDownButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.BuyItem && CurrentSelectables == BuyAdditionalBattleCrewButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().battleCrewDownButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.BuyItem && CurrentSelectables == BuyWarpBoosterButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().warpBoosterDownButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.BuyItem && CurrentSelectables == BuyTranswarpBoosterButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().transwarpBoosterDownButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.BuyItem && CurrentSelectables == BuyWarpDriveButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().warpDriveDownButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.BuyItem && CurrentSelectables == BuyTranswarpDriveButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().transwarpDriveDownButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.OutfitShip && CurrentSelectables == BuyPhaserRadarShotButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().phaserRadarShotDownButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.OutfitShip && CurrentSelectables == BuyPhaserRadarArrayButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().phaserRadarArrayDownButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.OutfitShip && CurrentSelectables == BuyXRayKernelButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().xRayKernelDownButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.OutfitShip && CurrentSelectables == BuyTractorBeamButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().tractorBeamDownButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.OutfitShip && CurrentSelectables == BuyLightTorpedoButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().lightTorpedoDownButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.OutfitShip && CurrentSelectables == BuyHeavyTorpedoButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().heavyTorpedoDownButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.OutfitShip && CurrentSelectables == BuyTorpedoLaserShotButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().torpedoLaserShotDownButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.OutfitShip && CurrentSelectables == BuyTorpedoLaserGuidanceButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().torpedoLaserGuidanceDownButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.OutfitShip && CurrentSelectables == BuyHighPressureTubesButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().highPressureTubesDownButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.OutfitShip && CurrentSelectables == BuyDilithiumCrystalButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().dilithiumCrystalDownButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.OutfitShip && CurrentSelectables == BuyTrilithiumCrystalButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().trilithiumCrystalDownButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.OutfitShip && CurrentSelectables == BuyFlareButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().flareDownButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.OutfitShip && CurrentSelectables == BuyRadarJammingButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().radarJammingSystemDownButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.OutfitShip && CurrentSelectables == BuyLaserScatteringButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().laserScatteringSystemDownButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.OutfitShip && CurrentSelectables == BuyRepairCrewButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().repairCrewDownButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.OutfitShip && CurrentSelectables == BuyShieldEngineeringTeamButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().shieldEngineeringTeamDownButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.OutfitShip && CurrentSelectables == BuyAdditionalBattleCrewButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().battleCrewDownButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.OutfitShip && CurrentSelectables == BuyWarpBoosterButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().warpBoosterDownButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.OutfitShip && CurrentSelectables == BuyTranswarpBoosterButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().transwarpBoosterDownButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.OutfitShip && CurrentSelectables == BuyWarpDriveButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().warpDriveDownButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.OutfitShip && CurrentSelectables == BuyTranswarpDriveButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().transwarpDriveDownButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.UseFlares && CurrentSelectables == FlareCountDisplay){

				//click the button
				uiManager.GetComponent<FlareManager> ().flareCountDownButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.Settings && CurrentSelectables == SettingsResolutionDropdown){

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

				} else if (CurrentUIState == UIState.SaveLocalGame && CurrentSelectables == SaveLocalGameFiles) {

					//here we are selecting a lower file in the scroll rect
					FitCurrentSelectableIntoScrollRect ();

					//call the select save file event, passing the current selectable object
					OnSelectSaveFile.Invoke(CurrentSelectables[currentSelectionIndex].gameObject);

				}

			} else if(CurrentUIState == UIState.PhaserMenu && CurrentSelectables == PhaserTargetingDropdown){

				//adjust the dropdown value
				AdjustDropdownValueUp(PhaserTargetingDropdown[0].GetComponent<TMP_Dropdown>());

			} else if(CurrentUIState == UIState.TorpedoMenu && CurrentSelectables == TorpedoTargetingDropdown){

				//adjust the dropdown value
				AdjustDropdownValueUp(TorpedoTargetingDropdown[0].GetComponent<TMP_Dropdown>());

			} else if(CurrentUIState == UIState.UseItem && CurrentSelectables == ItemTargetingDropdown){

				//adjust the dropdown value
				AdjustDropdownValueUp(ItemTargetingDropdown[0].GetComponent<TMP_Dropdown>());

			} else if(CurrentUIState == UIState.Crew && CurrentSelectables == CrewTargetingDropdown){

				//adjust the dropdown value
				AdjustDropdownValueUp(CrewTargetingDropdown[0].GetComponent<TMP_Dropdown>());

			} else if(CurrentUIState == UIState.BuyItem && CurrentSelectables == BuyPhaserRadarShotButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().phaserRadarShotUpButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.BuyItem && CurrentSelectables == BuyPhaserRadarArrayButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().phaserRadarArrayUpButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.BuyItem && CurrentSelectables == BuyXRayKernelButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().xRayKernelUpButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.BuyItem && CurrentSelectables == BuyTractorBeamButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().tractorBeamUpButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.BuyItem && CurrentSelectables == BuyLightTorpedoButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().lightTorpedoUpButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.BuyItem && CurrentSelectables == BuyHeavyTorpedoButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().heavyTorpedoUpButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.BuyItem && CurrentSelectables == BuyTorpedoLaserShotButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().torpedoLaserShotUpButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.BuyItem && CurrentSelectables == BuyTorpedoLaserGuidanceButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().torpedoLaserGuidanceUpButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.BuyItem && CurrentSelectables == BuyHighPressureTubesButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().highPressureTubesUpButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.BuyItem && CurrentSelectables == BuyDilithiumCrystalButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().dilithiumCrystalUpButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.BuyItem && CurrentSelectables == BuyTrilithiumCrystalButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().trilithiumCrystalUpButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.BuyItem && CurrentSelectables == BuyFlareButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().flareDownButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.BuyItem && CurrentSelectables == BuyRadarJammingButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().radarJammingSystemUpButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.BuyItem && CurrentSelectables == BuyLaserScatteringButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().laserScatteringSystemUpButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.BuyItem && CurrentSelectables == BuyRepairCrewButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().repairCrewUpButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.BuyItem && CurrentSelectables == BuyShieldEngineeringTeamButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().shieldEngineeringTeamUpButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.BuyItem && CurrentSelectables == BuyAdditionalBattleCrewButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().battleCrewUpButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.BuyItem && CurrentSelectables == BuyWarpBoosterButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().warpBoosterUpButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.BuyItem && CurrentSelectables == BuyTranswarpBoosterButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().transwarpBoosterUpButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.BuyItem && CurrentSelectables == BuyWarpDriveButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().warpDriveUpButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.BuyItem && CurrentSelectables == BuyTranswarpDriveButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().transwarpDriveUpButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.OutfitShip && CurrentSelectables == BuyPhaserRadarShotButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().phaserRadarShotUpButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.OutfitShip && CurrentSelectables == BuyPhaserRadarArrayButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().phaserRadarArrayUpButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.OutfitShip && CurrentSelectables == BuyXRayKernelButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().xRayKernelUpButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.OutfitShip && CurrentSelectables == BuyTractorBeamButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().tractorBeamUpButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.OutfitShip && CurrentSelectables == BuyLightTorpedoButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().lightTorpedoUpButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.OutfitShip && CurrentSelectables == BuyHeavyTorpedoButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().heavyTorpedoUpButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.OutfitShip && CurrentSelectables == BuyTorpedoLaserShotButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().torpedoLaserShotUpButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.OutfitShip && CurrentSelectables == BuyTorpedoLaserGuidanceButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().torpedoLaserGuidanceUpButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.OutfitShip && CurrentSelectables == BuyHighPressureTubesButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().highPressureTubesUpButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.OutfitShip && CurrentSelectables == BuyDilithiumCrystalButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().dilithiumCrystalUpButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.OutfitShip && CurrentSelectables == BuyTrilithiumCrystalButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().trilithiumCrystalUpButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.OutfitShip && CurrentSelectables == BuyFlareButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().flareDownButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.OutfitShip && CurrentSelectables == BuyRadarJammingButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().radarJammingSystemUpButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.OutfitShip && CurrentSelectables == BuyLaserScatteringButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().laserScatteringSystemUpButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.OutfitShip && CurrentSelectables == BuyRepairCrewButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().repairCrewUpButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.OutfitShip && CurrentSelectables == BuyShieldEngineeringTeamButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().shieldEngineeringTeamUpButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.OutfitShip && CurrentSelectables == BuyAdditionalBattleCrewButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().battleCrewUpButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.OutfitShip && CurrentSelectables == BuyWarpBoosterButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().warpBoosterUpButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.OutfitShip && CurrentSelectables == BuyTranswarpBoosterButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().transwarpBoosterUpButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.OutfitShip && CurrentSelectables == BuyWarpDriveButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().warpDriveUpButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.OutfitShip && CurrentSelectables == BuyTranswarpDriveButton){

				//click the button
				uiManager.GetComponent<PurchaseManager> ().transwarpDriveUpButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.UseFlares && CurrentSelectables == FlareCountDisplay){

				//click the button
				uiManager.GetComponent<FlareManager> ().flareCountUpButton.onClick.Invoke();

			} else if(CurrentUIState == UIState.Settings && CurrentSelectables == SettingsResolutionDropdown){

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
			if (CurrentSelectables [currentSelectionIndex] != null &&
			    CurrentSelectables [currentSelectionIndex].GetComponent<TMP_Dropdown> () == true &&
			    CurrentSelectables [currentSelectionIndex].transform.Find ("Dropdown List") != null) {

				//hide the list
				CurrentSelectables [currentSelectionIndex].GetComponent<TMP_Dropdown> ().Hide ();


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

				//check if we are in the load files or save files - if we are here, we don't want tab to cycle through the files
				if (CurrentSelectables == LoadLocalGameFiles || CurrentSelectables == SaveLocalGameFiles) {
					
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

					} //check if the next group is save files
					else if (GetNextSelectableGroup (true) == SaveLocalGameFiles) {

						//directly set the selectable group
						CurrentSelectables = SaveLocalGameFiles;
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
				if (CurrentSelectables == LoadLocalGameFiles || CurrentSelectables == SaveLocalGameFiles) {

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

					} //check if the next group is save files
					else if (GetNextSelectableGroup (false) == SaveLocalGameFiles) {

						//directly set the selectable group
						CurrentSelectables = SaveLocalGameFiles;
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

			if (CurrentUIState == UIState.LoadLocalGame) {

				//invoke the event
				//OnCloseWindowWithCancel.Invoke();

				//cancel out of the menu
				uiManager.GetComponent<FileLoadWindow> ().closeFileLoadWindowButton.onClick.Invoke ();


			} 

			else if (CurrentUIState == UIState.SaveLocalGame && ignoreEscape == false) {

				//cancel out of the menu
				uiManager.GetComponent<FileSaveWindow> ().closeFileSaveWindowButton.onClick.Invoke ();

				//invoke the event
				//OnCloseWindowWithCancel.Invoke();

			} 

			else if (CurrentUIState == UIState.FileDeletePrompt) {

				//cancel out of the menu
				uiManager.GetComponent<FileDeletePrompt> ().fileDeleteCancelButton.onClick.Invoke ();

				//invoke the event
				//OnCloseWindowWithCancel.Invoke();

			} 

			else if (CurrentUIState == UIState.FileOverwritePrompt) {

				//cancel out of the menu
				uiManager.GetComponent<FileOverwritePrompt> ().fileOverwriteCancelButton.onClick.Invoke ();

				//invoke the event
				//OnCloseWindowWithCancel.Invoke();

			}

			else if (CurrentUIState == UIState.Settings) {

				//cancel out of the menu
				uiManager.GetComponent<Settings> ().exitButton.onClick.Invoke ();

				//invoke the event
				//OnCloseWindowWithCancel.Invoke();

			} 

			else if (CurrentUIState == UIState.ExitGamePrompt) {

				//cancel out of the menu
				uiManager.GetComponent<ExitGamePrompt> ().exitGameCancelButton.onClick.Invoke ();

				//invoke the event
				//OnCloseWindowWithCancel.Invoke();

			} 

			else if (CurrentUIState == UIState.VictoryPanel) {

				//cancel out of the menu
				uiManager.GetComponent<VictoryPanel> ().continuePlayingButton.onClick.Invoke ();

				//invoke the event
				//OnCloseWindowWithCancel.Invoke();

			} 

			else if (CurrentUIState == UIState.Status) {

				//cancel out of the menu
				uiManager.GetComponent<StatusPanel> ().closeButton.onClick.Invoke ();

				//invoke the event
				//OnCloseWindowWithCancel.Invoke();

			} 

			else if (CurrentUIState == UIState.UseFlares) {

				//cancel out of the menu
				uiManager.GetComponent<FlareManager> ().cancelFlaresButton.onClick.Invoke ();

				//invoke the event
				//OnCloseWindowWithCancel.Invoke();

			} 

			else if (CurrentUIState == UIState.RenameUnit && ignoreEscape == false) {

				//cancel out of the menu
				uiManager.GetComponent<RenameShip> ().cancelButton.onClick.Invoke ();

				//invoke the event
				//OnCloseWindowWithCancel.Invoke();

			} 	

			else if (CurrentUIState == UIState.PlaceNewShip) {

				//cancel out of the menu
				uiManager.GetComponent<InstructionPanel> ().cancelButton.onClick.Invoke ();

				//invoke the event
				//OnCloseWindowWithCancel.Invoke();

			} 

			else if (CurrentUIState == UIState.NameNewShip && ignoreEscape == false) {

				//cancel out of the menu
				uiManager.GetComponent<NameNewShip> ().cancelButton.onClick.Invoke ();

				//invoke the event
				//OnCloseWindowWithCancel.Invoke();

			} 

			else if (CurrentUIState == UIState.BuyItem) {

				//cancel out of the menu
				uiManager.GetComponent<PurchaseManager> ().cancelPurchaseItemsButton.onClick.Invoke ();

				//invoke the event
				//OnCloseWindowWithCancel.Invoke();

			} 

			else if (CurrentUIState == UIState.OutfitShip) {

				//cancel out of the menu
				uiManager.GetComponent<PurchaseManager> ().cancelPurchaseItemsButton.onClick.Invoke ();

				//invoke the event
				//OnCloseWindowWithCancel.Invoke();

			} 

			else if (CurrentUIState == UIState.BuyShip) {

				//cancel out of the menu
				uiManager.GetComponent<PurchaseManager> ().cancelPurchaseShipButton.onClick.Invoke ();

				//invoke the event
				//OnCloseWindowWithCancel.Invoke();

			} 

			else if (CurrentUIState == UIState.EndTurn) {

				//cancel out of the menu
				uiManager.GetComponent<EndTurnDropDown> ().endTurnDropDownPanelCancelButton.onClick.Invoke ();

				//invoke the event
				//OnCloseWindowWithCancel.Invoke();

			} 

			else if (CurrentUIState == UIState.MoveMenu && gameManager.CurrentActionMode != GameManager.ActionMode.Animation && 
				uiManager.GetComponent<CutsceneManager>().cutscenePanel.activeInHierarchy == false) {

				//check if we have a targeted unit
				if (mouseManager.targetedUnit != null) {

					//clear the targeted unit
					OnCancelClearTargetedUnit.Invoke ();

				} else if (mouseManager.selectedUnit != null) {

					//else check if we have a selected unit
					OnCancelClearSelectedUnit.Invoke ();

				} else {

					//else we want to cancel out of the menu by clicking the toggle
					uiManager.GetComponent<MoveToggle> ().moveToggle.isOn = false;

					//invoke the event to trigger sound
					OnCloseWindowWithCancel.Invoke();

				}

			} 

			else if (CurrentUIState == UIState.PhaserMenu && gameManager.CurrentActionMode != GameManager.ActionMode.Animation && 
				uiManager.GetComponent<CutsceneManager>().cutscenePanel.activeInHierarchy == false) {

				//check if we have a targeted unit
				if (mouseManager.targetedUnit != null) {

					//clear the targeted unit
					OnCancelClearTargetedUnit.Invoke ();

				} else if (mouseManager.selectedUnit != null) {

					//else check if we have a selected unit
					OnCancelClearSelectedUnit.Invoke ();

				} else {

					//else we want to cancel out of the menu by clicking the toggle
					uiManager.GetComponent<PhaserToggle> ().phaserToggle.isOn = false;

					//invoke the event to trigger sound
					OnCloseWindowWithCancel.Invoke();
				}

			} 

			else if (CurrentUIState == UIState.TorpedoMenu && gameManager.CurrentActionMode != GameManager.ActionMode.Animation && 
				uiManager.GetComponent<CutsceneManager>().cutscenePanel.activeInHierarchy == false) {

				//check if we have a targeted unit
				if (mouseManager.targetedUnit != null) {

					//clear the targeted unit
					OnCancelClearTargetedUnit.Invoke ();

				} else if (mouseManager.selectedUnit != null) {

					//else check if we have a selected unit
					OnCancelClearSelectedUnit.Invoke ();

				} else {

					//else we want to cancel out of the menu by clicking the toggle
					uiManager.GetComponent<TorpedoToggle> ().torpedoToggle.isOn = false;

					//invoke the event to trigger sound
					OnCloseWindowWithCancel.Invoke();

				}

			} 

			else if (CurrentUIState == UIState.TractorBeamMenu && gameManager.CurrentActionMode != GameManager.ActionMode.Animation && 
				uiManager.GetComponent<CutsceneManager>().cutscenePanel.activeInHierarchy == false) {

				//check if we have a targeted unit
				if (mouseManager.targetedUnit != null) {

					//clear the targeted unit
					OnCancelClearTargetedUnit.Invoke ();

				} else if (mouseManager.selectedUnit != null) {

					//else check if we have a selected unit
					OnCancelClearSelectedUnit.Invoke ();

				} else {

					//else we want to cancel out of the menu by clicking the toggle
					uiManager.GetComponent<TractorBeamToggle> ().tractorBeamToggle.isOn = false;

					//invoke the event to trigger sound
					OnCloseWindowWithCancel.Invoke();

				}

			} 

			else if (CurrentUIState == UIState.UseItem && gameManager.CurrentActionMode != GameManager.ActionMode.Animation && 
				uiManager.GetComponent<CutsceneManager>().cutscenePanel.activeInHierarchy == false) {

				//check if we have a targeted unit
				if (mouseManager.targetedUnit != null) {

					//clear the targeted unit
					OnCancelClearTargetedUnit.Invoke ();

				} else if (mouseManager.selectedUnit != null) {

					//else check if we have a selected unit
					OnCancelClearSelectedUnit.Invoke ();

				} else {

					//else we want to cancel out of the menu by clicking the toggle
					uiManager.GetComponent<UseItemToggle> ().useItemToggle.isOn = false;

					//invoke the event to trigger sound
					OnCloseWindowWithCancel.Invoke();

				}

			} 

			else if (CurrentUIState == UIState.Crew && gameManager.CurrentActionMode != GameManager.ActionMode.Animation && 
				uiManager.GetComponent<CutsceneManager>().cutscenePanel.activeInHierarchy == false) {

				//check if we have a targeted unit
				if (mouseManager.targetedUnit != null) {

					//clear the targeted unit
					OnCancelClearTargetedUnit.Invoke ();

				} else if (mouseManager.selectedUnit != null) {

					//else check if we have a selected unit
					OnCancelClearSelectedUnit.Invoke ();

				} else {

					//else we want to cancel out of the menu by clicking the toggle
					uiManager.GetComponent<CrewToggle> ().crewToggle.isOn = false;

					//invoke the event to trigger sound
					OnCloseWindowWithCancel.Invoke();

				}

			} 

			else if (CurrentUIState == UIState.Cloaking && gameManager.CurrentActionMode != GameManager.ActionMode.Animation && 
				uiManager.GetComponent<CutsceneManager>().cutscenePanel.activeInHierarchy == false) {

				//check if we have a targeted unit
				if (mouseManager.targetedUnit != null) {

					//clear the targeted unit
					OnCancelClearTargetedUnit.Invoke ();

				} else if (mouseManager.selectedUnit != null) {

					//else check if we have a selected unit
					OnCancelClearSelectedUnit.Invoke ();

				} else {

					//else we want to cancel out of the menu by clicking the toggle
					uiManager.GetComponent<CloakingDeviceToggle> ().cloakingDeviceToggle.isOn = false;

					//invoke the event to trigger sound
					OnCloseWindowWithCancel.Invoke();

				}

			} 

			else if (CurrentUIState == UIState.Selection && gameManager.CurrentActionMode != GameManager.ActionMode.Animation && 
				uiManager.GetComponent<CutsceneManager>().cutscenePanel.activeInHierarchy == false) {

				//check if we have a targeted unit
				if (mouseManager.targetedUnit != null) {

					//clear the targeted unit
					OnCancelClearTargetedUnit.Invoke ();

				} else if (mouseManager.selectedUnit != null) {

					//else check if we have a selected unit
					OnCancelClearSelectedUnit.Invoke ();

				} 

			} 

		}

		//check if we are pressing the enter key
		if (Input.GetKeyDown (KeyCode.Return) == true) {

			//check if we are currently on a file for load
			if (CurrentUIState == UIState.LoadLocalGame && CurrentSelectables == LoadLocalGameFiles) {

				//call the load game button on-click
				uiManager.GetComponent<FileLoadWindow> ().fileLoadYesButton.onClick.Invoke ();

			} else if (CurrentUIState == UIState.SaveLocalGame && CurrentSelectables == SaveLocalGameFiles) {

				//check if we are currently on a file for save
				//call the save button click
				uiManager.GetComponent<FileSaveWindow> ().fileSaveYesButton.onClick.Invoke ();

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

		//check if we are pressing the comma key (less than) 
		if (Input.GetKeyDown (KeyCode.Comma) == true) {

			//check if the previous unit button is interactable and enabled and in the current group and it's not cutscene
			if (uiManager.GetComponent<NextUnit>().previousUnitButton.IsInteractable() == true && 
				uiManager.GetComponent<NextUnit>().previousUnitButton.IsActive() == true && 
				SelectableInGroup(uiManager.GetComponent<NextUnit>().previousUnitButton.GetComponent<Selectable>()) == true && 
				uiManager.GetComponent<CutsceneManager>().cutscenePanel.activeInHierarchy == false) {

				//call the previous button on-click
				uiManager.GetComponent<NextUnit>().previousUnitButton.onClick.Invoke();

			}

		}

		//check if we are pressing the period key (greater than) 
		if (Input.GetKeyDown (KeyCode.Period) == true) {

			//check if the previous unit button is interactable and enabled and in the current group
			if (uiManager.GetComponent<NextUnit>().nextUnitButton.IsInteractable() == true && 
				uiManager.GetComponent<NextUnit>().nextUnitButton.IsActive() == true && 
				SelectableInGroup(uiManager.GetComponent<NextUnit>().nextUnitButton.GetComponent<Selectable>()) == true &&
				uiManager.GetComponent<CutsceneManager>().cutscenePanel.activeInHierarchy == false) {

				//call the next button on-click
				uiManager.GetComponent<NextUnit>().nextUnitButton.onClick.Invoke();

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

		} else if (CurrentUIState == UIState.SaveLocalGame) {

			//check if the current selected object is a file list item

			if (CurrentSelectables == SaveLocalGameFiles) {

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

		NewTurnSetInitialSelectablesAction = (player) => {

			//set the new turn flag to true
			isNewTurn = true;

			returnSelectable = ActionMenuButtons[10];

			//SetInitialCurrentSelectables ();
			delayReturnToFirstSelectableCount = 2;
		
		};

		MoveToggleSetUIStateAction = (toggleState) => {

			if (toggleState == true) {

				CurrentUIState = UIState.MoveMenu;

			} else {

				CurrentUIState = UIState.Selection;

			}
			;

		};

		PhaserToggleSetUIStateAction = (toggleState) => {

			if (toggleState == true) {

				CurrentUIState = UIState.PhaserMenu;

			} else {

				CurrentUIState = UIState.Selection;

			}
			;

		};

		TorpedoToggleSetUIStateAction = (toggleState) => {

			if (toggleState == true) {

				CurrentUIState = UIState.TorpedoMenu;

			} else {

				//check if the toggle is being turned off due to game state change for FlareMode
				//if so, we don't want to go back to selection state
				if(uiManager.GetComponent<FlareManager>().flareMenuPanel.gameObject.activeInHierarchy == true){

					//Debug.Log("Do Nothing");

				}else{
						
				CurrentUIState = UIState.Selection;

				}

			}
			;

		};

		TractorBeamToggleSetUIStateAction = (toggleState) => {

			if (toggleState == true) {

				CurrentUIState = UIState.TractorBeamMenu;

			} else {

				CurrentUIState = UIState.Selection;

			}
			;

		};

		UseItemToggleSetUIStateAction = (toggleState) => {

			if (toggleState == true) {

				CurrentUIState = UIState.UseItem;

			} else {

				CurrentUIState = UIState.Selection;

			}
			;

		};

		CrewToggleSetUIStateAction = (toggleState) => {

			if (toggleState == true) {

				CurrentUIState = UIState.Crew;

			} else {

				CurrentUIState = UIState.Selection;

			}
			;

		};

		CloakingToggleSetUIStateAction = (toggleState) => {

			if (toggleState == true) {

				CurrentUIState = UIState.Cloaking;

			} else {

				CurrentUIState = UIState.Selection;

			}
			;

		};

		BuyItemSetUIStateAction = () => {

			CurrentUIState = UIState.BuyItem;

		};

		BuyShipSetUIStateAction = () => {

			CurrentUIState = UIState.BuyShip;

		};

		NameNewShipSetUIStateAction = (hex) => {

			CurrentUIState = UIState.NameNewShip;

		};

		RenameShipSetUIStateAction = () => {

			CurrentUIState = UIState.RenameUnit;

		};

		EndTurnSetUIStateAction = (toggleState) => {

			if (toggleState == true) {

				CurrentUIState = UIState.EndTurn;

			} else {

				//check if the turn phase is purchase
				//if(gameManager.currentTurnPhase == GameManager.TurnPhase.PurchasePhase){

					CurrentUIState = UIState.Selection;

					returnSelectable = ActionMenuButtons[10];

					//set currentSelectables to null so that when we go back to selection case we don't try to restore the chat input
					//CurrentSelectables = null;

					delayReturnToSelectableCount = 2;

				//}
			}
			;

		};

		UseFlaresSetUIStateAction = () => {

			CurrentUIState = UIState.UseFlares;

		};

		CutsceneSetUIStateAction = () => {

			CurrentUIState = UIState.CombatCutscene;

		};

		StatusSetUIStateAction = () => {

			CurrentUIState = UIState.Status;

		};

		CancelStatusAction = () => {

			//CurrentUIState = UIState.Selection;
			DetermineUIState();

			//returnUIState = UIState.Selection;
			returnSelectable = StatusButton[0];

			//returnSelectable = null;
			delayReturnToSelectableCount = 2;

		};


		CancelEndTurnPromptAction = () => {

			//returnUIState = UIState.Selection;
			returnSelectable = ActionMenuButtons[10];

			//returnSelectable = null;
			delayReturnToSelectableCount = 2;

			//set the initial UIState
			//CurrentUIState = UIState.Selection;

			//invoke the event to start, since it won't fire because the enum defaults to the Selection
			//OnUIStateChange.Invoke();
		
		};

		AcceptEndTurnPromptAction = () => {

			//returnUIState = UIState.Selection;
			//returnSelectable = null;

			//returnSelectable = null;
			//delayReturnToSelectableCount = 2;

			//set the initial UIState
			//CurrentUIState = UIState.Selection;

			//invoke the event to start, since it won't fire because the enum defaults to the Selection
			//OnUIStateChange.Invoke();

		};

		CancelPurchaseItemAction = () => {

			//check if we are coming from the BuyItem state or the OutfitShip state
			if(CurrentUIState == UIState.BuyItem){

				//CurrentUIState = UIState.Selection;
				DetermineUIState();

				//returnUIState = UIState.Selection;
				returnSelectable = ActionMenuButtons[7];

				//returnSelectable = null;
				delayReturnToSelectableCount = 2;

			} else{

				//the else is that we are coming from outfit ship state

				//CurrentUIState = UIState.Selection;
				DetermineUIState();

				//returnUIState = UIState.Selection;
				returnSelectable = ActionMenuButtons[8];

				//returnSelectable = null;
				delayReturnToSelectableCount = 2;

			}
				
		};

		AcceptPurchaseItemAction = () => {

			//check if we are coming from the BuyItem state or the OutfitShip state
			if(CurrentUIState == UIState.BuyItem){

				//CurrentUIState = UIState.Selection;
				DetermineUIState();

				//returnUIState = UIState.Selection;
				returnSelectable = ActionMenuButtons[7];

				//returnSelectable = null;
				delayReturnToSelectableCount = 2;

			} else{

				//the else is that we are coming from outfit ship state
				CurrentUIState = UIState.PlaceNewShip;

			}

		};

		CancelPurchaseShipAction = () => {

			//CurrentUIState = UIState.Selection;
			DetermineUIState();

			//returnUIState = UIState.Selection;
			returnSelectable = ActionMenuButtons[8];

			//returnSelectable = null;
			delayReturnToSelectableCount = 2;

		};

		AcceptPurchaseShipAction = () => {

			//Debug.Log("outfitship");

			CurrentUIState = UIState.OutfitShip;



		};

		CancelPlaceNewShipAction = () => {

			//CurrentUIState = UIState.Selection;
			DetermineUIState();

			//returnUIState = UIState.Selection;
			returnSelectable = ActionMenuButtons[8];

			//returnSelectable = null;
			delayReturnToSelectableCount = 2;

		};



		CancelNameNewShipAction = () => {

			//CurrentUIState = UIState.Selection;
			DetermineUIState();

			//returnUIState = UIState.Selection;
			returnSelectable = ActionMenuButtons[8];

			//returnSelectable = null;
			delayReturnToSelectableCount = 2;

		};

		AcceptNameNewShipAction = (newUnitEventData) => {

			//CurrentUIState = UIState.Selection;
			DetermineUIState();

			//returnUIState = UIState.Selection;
			returnSelectable = ActionMenuButtons[8];

			//returnSelectable = null;
			delayReturnToSelectableCount = 2;

		};

		CancelRenameUnitAction = () => {

			//CurrentUIState = UIState.Selection;
			DetermineUIState();

			//returnUIState = UIState.Selection;
			returnSelectable = ActionMenuButtons[9];

			//returnSelectable = null;
			delayReturnToSelectableCount = 2;

		};

		AcceptRenameUnitAction = () => {

			//CurrentUIState = UIState.Selection;
			DetermineUIState();

			//returnUIState = UIState.Selection;
			returnSelectable = ActionMenuButtons[9];

			//returnSelectable = null;
			delayReturnToSelectableCount = 2;

		};


		UseFlaresYesAction = (flareData) => {

			CurrentUIState = UIState.CombatCutscene;

		};

		UseFlaresCancelAction = (flareData) => {

			CurrentUIState = UIState.CombatCutscene;

		};


		TractorBeamShipSetUIStateAction = (ship) => {SetInitialCurrentSelectables ();};

		CombatUnitSetUIStateAction = (combatUnit) => {delaySetInitialSelectablesCount = 1;};

		SelectableSetSelectionGroupsAction = (selectable) => {SetSelectionIndexFromPointerClick (selectable);};

		//save window actions
		OpenSaveLocalGameWindowAction = () => {CurrentUIState = UIState.SaveLocalGame;};

		CloseSaveLocalGameWindowAction = () => {

			//CurrentUIState = UIState.Selection;
			DetermineUIState();

			//returnUIState = UIState.Selection;
			returnSelectable = FileMenuButtons[0];

			//returnSelectable = null;
			delayReturnToSelectableCount = 2;

		};

		//overwrite window actions
		OpenFileOverwritePromptAction = (fileName) => {CurrentUIState = UIState.FileOverwritePrompt;};

		StringReturnToFileSaveWindowAction = (fileName) => {delaySaveFilesWindowCount = 2;};

		//load window actions
		OpenLoadLocalGameWindowAction = () => {CurrentUIState = UIState.LoadLocalGame;};

		CloseLoadLocalGameWindowAction = () => {

			//CurrentUIState = UIState.Selection;
			DetermineUIState();

			//returnUIState = UIState.Selection;
			returnSelectable = FileMenuButtons[1];

			//returnSelectable = null;
			delayReturnToSelectableCount = 2;

		};


		//delete window actions
		OpenFileDeletePromptAction = (fileName) => {CurrentUIState = UIState.FileDeletePrompt;};

		StringReturnToFileLoadWindowAction = (fileName) => {delayLoadFilesWindowCount = 2;};



		SelectableSetNavigationRulesAction = () => {SetNavigationRulesForSelectables();};

		ClearSetInitialSelectablesAction = () => {delaySetInitialSelectablesCount = 1;};

		OpenSettingsWindowAction = () => {CurrentUIState = UIState.Settings;}; 

		CloseSettingsWindowAction = () => {

			//CurrentUIState = UIState.Selection;
			DetermineUIState();

			//returnUIState = UIState.Selection;
			returnSelectable = FileMenuButtons[3];

			//returnSelectable = null;
			delayReturnToSelectableCount = 2;

		};

		OpenExitGamePromptAction = () => {CurrentUIState = UIState.ExitGamePrompt;};

		CloseExitGamePromptAction = () => {

			//CurrentUIState = UIState.Selection;
			DetermineUIState();

			//returnUIState = UIState.Selection;
			returnSelectable = FileMenuButtons[2];

			//returnSelectable = null;
			delayReturnToSelectableCount = 2;

		};

		OpenVictoryPanelAction = () => {CurrentUIState = UIState.VictoryPanel;};

		CloseVictoryPanelAction = () => {

			//CurrentUIState = UIState.Selection;
			DetermineUIState();

			//returnUIState = UIState.Selection;
			//returnSelectable = FileMenuButtons[2];

			//returnSelectable = null;
			//delayReturnToSelectableCount = 2;

		};

		InputFieldEndEditIgnoreEscapeAction = (eventString) => {ignoreEscape = true;};

		PointerClickResolveBlockAction = (selectable) => {ResolvePointerClickBlock (selectable);};

		PhaserAttackSetCombatStateToPhaserAction = (attackingUnit, targetedUnit, text) => {combatType = UIState.PhaserMenu;};
		TorpedoAttackSetCombatStateToTorpedoAction = (attackingUnit, targetedUnit, text) => {combatType = UIState.TorpedoMenu;};

		ReturnToSelectablesFromCombatAction = () => {

			CurrentUIState = combatType;

			SetInitialCurrentSelectables();

		};

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

		//add listener for a pointer click selection
		UISelection.OnSetSelectedGameObject.AddListener(SelectableSetSelectionGroupsAction);

		//add listener for a pointer click
		UISelection.OnClickedSelectable.AddListener(PointerClickResolveBlockAction);

		//add listener for move toggle
		uiManager.GetComponent<MoveToggle>().moveToggle.onValueChanged.AddListener(MoveToggleSetUIStateAction);

		//add listener for phaser toggle
		uiManager.GetComponent<PhaserToggle>().phaserToggle.onValueChanged.AddListener(PhaserToggleSetUIStateAction);

		//add listener for torpedo toggle
		uiManager.GetComponent<TorpedoToggle>().torpedoToggle.onValueChanged.AddListener(TorpedoToggleSetUIStateAction);

		//add listener for tractor beam toggle
		uiManager.GetComponent<TractorBeamToggle>().tractorBeamToggle.onValueChanged.AddListener(TractorBeamToggleSetUIStateAction);

		//add listener for use item toggle
		uiManager.GetComponent<UseItemToggle>().useItemToggle.onValueChanged.AddListener(UseItemToggleSetUIStateAction);

		//add listener for crew toggle
		uiManager.GetComponent<CrewToggle>().crewToggle.onValueChanged.AddListener(CrewToggleSetUIStateAction);

		//add listener for cloaking toggle
		uiManager.GetComponent<CloakingDeviceToggle>().cloakingDeviceToggle.onValueChanged.AddListener(CloakingToggleSetUIStateAction);

		//add listeners for purchase item menu
		uiManager.GetComponent<PurchaseManager>().openPurchaseItemsButton.onClick.AddListener(BuyItemSetUIStateAction);
		uiManager.GetComponent<PurchaseManager>().cancelPurchaseItemsButton.onClick.AddListener(CancelPurchaseItemAction);
		uiManager.GetComponent<PurchaseManager>().purchaseItemsButton.onClick.AddListener(AcceptPurchaseItemAction);

		//add listeners for purchase ship menu
		uiManager.GetComponent<PurchaseManager>().OnOpenPurchaseShip.AddListener(BuyShipSetUIStateAction);
		uiManager.GetComponent<PurchaseManager>().OnReturnToPurchaseShip.AddListener(BuyShipSetUIStateAction);
		uiManager.GetComponent<PurchaseManager>().cancelPurchaseShipButton.onClick.AddListener(CancelPurchaseShipAction);
		uiManager.GetComponent<PurchaseManager>().purchaseShipButton.onClick.AddListener(AcceptPurchaseShipAction);

		//add listeners for placing new unit
		uiManager.GetComponent<InstructionPanel>().OnCancelPlaceUnit.AddListener(CancelPlaceNewShipAction);
		uiManager.GetComponent<InstructionPanel>().OnReturnToOutfitShip.AddListener(AcceptPurchaseShipAction);

		//add listeners for naming new unit
		mouseManager.OnPlacedNewUnit.AddListener(NameNewShipSetUIStateAction);
		uiManager.GetComponent<NameNewShip> ().OnPurchasedNewShip.AddListener (AcceptNameNewShipAction);
		uiManager.GetComponent<NameNewShip> ().OnCanceledPurchase.AddListener (CancelNameNewShipAction);
		uiManager.GetComponent<NameNewShip> ().OnReturnToPlaceUnit.AddListener (AcceptPurchaseItemAction);


		//add listener for end turn toggle
		uiManager.GetComponent<EndTurnToggle>().endTurnToggle.onValueChanged.AddListener(EndTurnSetUIStateAction);

		//add listener for disengaging tractor beam
		uiManager.GetComponent<TractorBeamMenu>().OnTurnOffTractorBeamToggle.AddListener(TractorBeamShipSetUIStateAction);

		//add listener for disengaging cloaking device
		uiManager.GetComponent<CloakingDeviceMenu>().OnTurnOffCloakingDevice.AddListener(CombatUnitSetUIStateAction);
		uiManager.GetComponent<CloakingDeviceMenu>().OnTurnOnCloakingDevice.AddListener(CombatUnitSetUIStateAction);

		//add listener for rename unit
		uiManager.GetComponent<RenameShip>().renameActionButton.onClick.AddListener(RenameShipSetUIStateAction);
		uiManager.GetComponent<RenameShip>().yesButton.onClick.AddListener(AcceptRenameUnitAction);
		uiManager.GetComponent<RenameShip>().cancelButton.onClick.AddListener(CancelRenameUnitAction);


		//add listeners for end turn drop down
		uiManager.GetComponent<EndTurnDropDown>().OnCancelEndTurnPrompt.AddListener(CancelEndTurnPromptAction);
		uiManager.GetComponent<EndTurnDropDown>().OnAcceptEndTurnPrompt.AddListener(AcceptEndTurnPromptAction);


		//add listeners for mouse manager actions
		mouseManager.OnSetTargetedUnit.AddListener(SetInitialCurrentSelectables);
		mouseManager.OnClearTargetedUnit.AddListener(ClearSetInitialSelectablesAction);
		mouseManager.OnSetSelectedUnit.AddListener(SetInitialCurrentSelectables);
		mouseManager.OnClearSelectedUnit.AddListener(ClearSetInitialSelectablesAction);

		//add listeners for status panel
		uiManager.GetComponent<StatusPanel>().openButton.onClick.AddListener(StatusSetUIStateAction);
		uiManager.GetComponent<StatusPanel>().closeButton.onClick.AddListener(CancelStatusAction);

		//add listener for save local game
		uiManager.GetComponent<FileSaveWindow>().OnOpenFileSaveWindow.AddListener(OpenSaveLocalGameWindowAction);

		//add listeners for exiting the file save window back to the main menu
		uiManager.GetComponent<FileSaveWindow>().OnCloseFileSaveWindow.AddListener(CloseSaveLocalGameWindowAction);

		//add listener for entering the file overwrite prompt
		uiManager.GetComponent<FileSaveWindow>().OnFileSaveYesClickedExistingFile.AddListener(OpenFileOverwritePromptAction);


		//add listeners for exiting the file overwrite prompt back to the save file window
		uiManager.GetComponent<FileOverwritePrompt>().OnFileOverwriteYesClicked.AddListener(StringReturnToFileSaveWindowAction);
		uiManager.GetComponent<FileOverwritePrompt>().OnFileSaveCancelClicked.AddListener(OpenSaveLocalGameWindowAction);


		//add listener for load local game
		uiManager.GetComponent<FileLoadWindow>().OnOpenFileLoadWindow.AddListener(OpenLoadLocalGameWindowAction);

		//add listener for setting selectables
		OnSelectablesChange.AddListener(SelectableSetNavigationRulesAction);

		//add listeners for exiting the file load window back to the main menu
		uiManager.GetComponent<FileLoadWindow>().closeFileLoadWindowButton.onClick.AddListener(CloseLoadLocalGameWindowAction);
		uiManager.GetComponent<FileLoadWindow>().fileLoadCancelButton.onClick.AddListener(CloseLoadLocalGameWindowAction);

		//add listener for entering the file delete prompt
		uiManager.GetComponent<FileLoadWindow>().OnFileDeleteYesClicked.AddListener(OpenFileDeletePromptAction);

		//add listeners for exiting the file delete prompt back to the load file window
		uiManager.GetComponent<FileDeletePrompt>().OnFileDeleteYesClicked.AddListener(StringReturnToFileLoadWindowAction);
		uiManager.GetComponent<FileDeletePrompt>().OnFileDeleteCancelClicked.AddListener(OpenLoadLocalGameWindowAction);

		//add listeners for entering the settings menu
		uiManager.GetComponent<Settings>().OnSettingsWindowOpened.AddListener(OpenSettingsWindowAction);

		//add listeners for exiting the settings menu
		uiManager.GetComponent<Settings>().acceptButton.onClick.AddListener(CloseSettingsWindowAction);
		uiManager.GetComponent<Settings>().exitButton.onClick.AddListener(CloseSettingsWindowAction);

		//add listener for entering the exit game prompt
		uiManager.GetComponent<ExitGamePrompt>().OnExitGamePromptOpened.AddListener(OpenExitGamePromptAction);

		//add listener for exiting the exit game prompt
		uiManager.GetComponent<ExitGamePrompt>().OnExitGameYesClicked.AddListener(CloseExitGamePromptAction);
		uiManager.GetComponent<ExitGamePrompt>().OnExitGameCancelClicked.AddListener(CloseExitGamePromptAction);

		//add listener for new or loaded turn
		gameManager.OnNewTurn.AddListener(NewTurnSetInitialSelectablesAction);
		gameManager.OnLoadedTurn.AddListener (NewTurnSetInitialSelectablesAction);

		//add listeners for attacks starting
		PhaserSection.OnFirePhasers.AddListener(PhaserAttackSetCombatStateToPhaserAction);
		StarbasePhaserSection1.OnFirePhasers.AddListener(PhaserAttackSetCombatStateToPhaserAction);
		StarbasePhaserSection2.OnFirePhasers.AddListener(PhaserAttackSetCombatStateToPhaserAction);
		TorpedoSection.OnFireLightTorpedo.AddListener (TorpedoAttackSetCombatStateToTorpedoAction);
		TorpedoSection.OnFireHeavyTorpedo.AddListener (TorpedoAttackSetCombatStateToTorpedoAction);
		StarbaseTorpedoSection.OnFireLightTorpedo.AddListener (TorpedoAttackSetCombatStateToTorpedoAction);
		StarbaseTorpedoSection.OnFireHeavyTorpedo.AddListener (TorpedoAttackSetCombatStateToTorpedoAction);

		//add listeners for flare use 
		uiManager.GetComponent<FlareManager>().OnShowFlarePanel.AddListener(UseFlaresSetUIStateAction);
		uiManager.GetComponent<FlareManager>().OnUseFlaresYes.AddListener(UseFlaresYesAction);
		uiManager.GetComponent<FlareManager>().OnUseFlaresCancel.AddListener(UseFlaresCancelAction);

		//add listener for Cutscene starting
		uiManager.GetComponent<CutsceneManager>().OnOpenCutsceneDisplayPanel.AddListener(CutsceneSetUIStateAction);

		//add listener for Cutscene ending
		uiManager.GetComponent<CutsceneManager>().OnCloseCutsceneDisplayPanel.AddListener(ReturnToSelectablesFromCombatAction);

		//add listeners telling us the input fields just ended edit, so we can ignore hitting escape
		uiManager.GetComponent<RenameShip> ().renameInputField.onEndEdit.AddListener (InputFieldEndEditIgnoreEscapeAction);
		uiManager.GetComponent<NameNewShip> ().shipNameInputField.onEndEdit.AddListener (InputFieldEndEditIgnoreEscapeAction);
		uiManager.GetComponent<FileSaveWindow> ().fileNameInputField.onEndEdit.AddListener (InputFieldEndEditIgnoreEscapeAction);

		//listeners for new turn
		//gameManager.OnBeginMainPhase.AddListener(BeginNewTurnPhaseAction);
		//gameManager.OnNewTurn.AddListener(BeginNewTurnPhaseAction);

		//add listener for the redirect click
		UIButtonSelectionRedirect.OnClickedButtonForRedirect.AddListener(ButtonRedirectSetSelectedObjectAction);

		//add listener to the UI selection pointer enter
		UISelection.OnPointerEnterSelectable.AddListener(OnPointerEnterTestAction);

		//add listener to the UI selection pointer enter
		UISelectionNonGroup.OnPointerEnterSelectable.AddListener(OnPointerEnterNonGroupTestAction);

		//add listener to the UI selection pointer click
		UISelectionNonGroup.OnPointerClickSelectable.AddListener(OnPointerClickNonGroupTestAction);

		//add listeners for victory panel
		uiManager.GetComponent<VictoryPanel>().OnOpenVictoryPanel.AddListener(OpenVictoryPanelAction);
		uiManager.GetComponent<VictoryPanel>().OnCloseVictoryPanel.AddListener(CloseVictoryPanelAction);

	}

	//this function defines the selectables groups
	private void DefineSelectablesGroups(){

		SelectionGroup = new Selectable[5][];
		SelectionGroup [0] = ActionMenuButtons;
		SelectionGroup [1] = ChatInputField;
		SelectionGroup [2] = FileMenuButtons;
		SelectionGroup [3] = NextUnitButtons;
		SelectionGroup [4] = StatusButton;

		MoveMenuGroup = new Selectable[6][];
		MoveMenuGroup [0] = MoveMenuButtons;
		MoveMenuGroup [1] = ActionMenuButtons;
		MoveMenuGroup [2] = ChatInputField;
		MoveMenuGroup [3] = FileMenuButtons;
		MoveMenuGroup [4] = NextUnitButtons;
		MoveMenuGroup [5] = StatusButton;

		PhaserMenuGroup = new Selectable[8][];
		PhaserMenuGroup [0] = PhaserRadarShotButton;
		PhaserMenuGroup [1] = PhaserTargetingDropdown;
		PhaserMenuGroup [2] = PhaserFireButton;
		PhaserMenuGroup [3] = ActionMenuButtons;
		PhaserMenuGroup [4] = ChatInputField;
		PhaserMenuGroup [5] = FileMenuButtons;
		PhaserMenuGroup [6] = NextUnitButtons;
		PhaserMenuGroup [7] = StatusButton;

		TorpedoMenuGroup = new Selectable[8][];
		TorpedoMenuGroup [0] = TorpedoLaserShotButton;
		TorpedoMenuGroup [1] = TorpedoTargetingDropdown;
		TorpedoMenuGroup [2] = TorpedoFireButtons;
		TorpedoMenuGroup [3] = ActionMenuButtons;
		TorpedoMenuGroup [4] = ChatInputField;
		TorpedoMenuGroup [5] = FileMenuButtons;
		TorpedoMenuGroup [6] = NextUnitButtons;
		TorpedoMenuGroup [7] = StatusButton;

		TractorBeamMenuGroup = new Selectable[6][];
		TractorBeamMenuGroup [0] = TractorBeamEngageButton;
		TractorBeamMenuGroup [1] = ActionMenuButtons;
		TractorBeamMenuGroup [2] = ChatInputField;
		TractorBeamMenuGroup [3] = FileMenuButtons;
		TractorBeamMenuGroup [4] = NextUnitButtons;
		TractorBeamMenuGroup [5] = StatusButton;

		UseItemMenuGroup = new Selectable[8][];
		UseItemMenuGroup [0] = FlareModeButtons;
		UseItemMenuGroup [1] = ItemTargetingDropdown;
		UseItemMenuGroup [2] = CrystalButtons;
		UseItemMenuGroup [3] = ActionMenuButtons;
		UseItemMenuGroup [4] = ChatInputField;
		UseItemMenuGroup [5] = FileMenuButtons;
		UseItemMenuGroup [6] = NextUnitButtons;
		UseItemMenuGroup [7] = StatusButton;

		CrewMenuGroup = new Selectable[7][];
		CrewMenuGroup [0] = CrewTargetingDropdown;
		CrewMenuGroup [1] = CrewRepairButton;
		CrewMenuGroup [2] = ActionMenuButtons;
		CrewMenuGroup [3] = ChatInputField;
		CrewMenuGroup [4] = FileMenuButtons;
		CrewMenuGroup [5] = NextUnitButtons;
		CrewMenuGroup [6] = StatusButton;

		CloakingMenuGroup = new Selectable[6][];
		CloakingMenuGroup [0] = CloakingDeviceEngageButton;
		CloakingMenuGroup [1] = ActionMenuButtons;
		CloakingMenuGroup [2] = ChatInputField;
		CloakingMenuGroup [3] = FileMenuButtons;
		CloakingMenuGroup [4] = NextUnitButtons;
		CloakingMenuGroup [5] = StatusButton;

		BuyItemGroup = new Selectable[22][];
		BuyItemGroup [0] = BuyPhaserRadarShotButton;
		BuyItemGroup [1] = BuyPhaserRadarArrayButton;
		BuyItemGroup [2] = BuyXRayKernelButton;
		BuyItemGroup [3] = BuyTractorBeamButton;
		BuyItemGroup [4] = BuyLightTorpedoButton;
		BuyItemGroup [5] = BuyHeavyTorpedoButton;
		BuyItemGroup [6] = BuyTorpedoLaserShotButton;
		BuyItemGroup [7] = BuyTorpedoLaserGuidanceButton;
		BuyItemGroup [8] = BuyHighPressureTubesButton;
		BuyItemGroup [9] = BuyDilithiumCrystalButton;
		BuyItemGroup [10] = BuyTrilithiumCrystalButton;
		BuyItemGroup [11] = BuyFlareButton;
		BuyItemGroup [12] = BuyRadarJammingButton;
		BuyItemGroup [13] = BuyLaserScatteringButton;
		BuyItemGroup [14] = BuyRepairCrewButton;
		BuyItemGroup [15] = BuyShieldEngineeringTeamButton;
		BuyItemGroup [16] = BuyAdditionalBattleCrewButton;
		BuyItemGroup [17] = BuyWarpBoosterButton;
		BuyItemGroup [18] = BuyTranswarpBoosterButton;
		BuyItemGroup [19] = BuyWarpDriveButton;
		BuyItemGroup [20] = BuyTranswarpDriveButton;
		BuyItemGroup [21] = BuyItemButtonRow;

		BuyShipGroup = new Selectable[2][];
		BuyShipGroup [0] = BuyShipsButtons;
		BuyShipGroup [1] = BuyShipButtonRow;

		OutfitShipGroup = new Selectable[22][];
		OutfitShipGroup [0] = BuyPhaserRadarShotButton;
		OutfitShipGroup [1] = BuyPhaserRadarArrayButton;
		OutfitShipGroup [2] = BuyXRayKernelButton;
		OutfitShipGroup [3] = BuyTractorBeamButton;
		OutfitShipGroup [4] = BuyLightTorpedoButton;
		OutfitShipGroup [5] = BuyHeavyTorpedoButton;
		OutfitShipGroup [6] = BuyTorpedoLaserShotButton;
		OutfitShipGroup [7] = BuyTorpedoLaserGuidanceButton;
		OutfitShipGroup [8] = BuyHighPressureTubesButton;
		OutfitShipGroup [9] = BuyDilithiumCrystalButton;
		OutfitShipGroup [10] = BuyTrilithiumCrystalButton;
		OutfitShipGroup [11] = BuyFlareButton;
		OutfitShipGroup [12] = BuyRadarJammingButton;
		OutfitShipGroup [13] = BuyLaserScatteringButton;
		OutfitShipGroup [14] = BuyRepairCrewButton;
		OutfitShipGroup [15] = BuyShieldEngineeringTeamButton;
		OutfitShipGroup [16] = BuyAdditionalBattleCrewButton;
		OutfitShipGroup [17] = BuyWarpBoosterButton;
		OutfitShipGroup [18] = BuyTranswarpBoosterButton;
		OutfitShipGroup [19] = BuyWarpDriveButton;
		OutfitShipGroup [20] = BuyTranswarpDriveButton;
		OutfitShipGroup [21] = BuyItemButtonRow;

		PlaceNewShipGroup = new Selectable[1][];
		PlaceNewShipGroup [0] = PlaceNewShipButtons;

		NameNewShipGroup = new Selectable[2][];
		NameNewShipGroup [0] = NameNewShipInputField;
		NameNewShipGroup [1] = NameNewShipButtons;

		RenameShipGroup = new Selectable[2][];
		RenameShipGroup [0] = RenameShipInputField;
		RenameShipGroup [1] = RenameShipButtons;

		EndTurnGroup = new Selectable[1][];
		EndTurnGroup [0] = EndTurnButtons;

		UseFlaresGroup = new Selectable[3][];
		UseFlaresGroup[0] = FlareCountDisplay;
		UseFlaresGroup[1] = FlareAutoCalcButton;
		UseFlaresGroup[2] = UseFlaresButtonRow;

		StatusGroup = new Selectable[1][];
		StatusGroup [0] = StatusButtons;

		SaveLocalGameGroup = new Selectable[3][];
		SaveLocalGameGroup [0] = SaveLocalGameFiles;
		SaveLocalGameGroup [1] = SaveLocalGameInputField;
		SaveLocalGameGroup [2] = SaveLocalGameButtonsRow;

		FileOverwritePromptGroup = new Selectable[1][];
		FileOverwritePromptGroup [0] = FileOverwriteButtons;

		LoadLocalGameGroup = new Selectable[2][];
		LoadLocalGameGroup [0] = LoadLocalGameFiles;
		LoadLocalGameGroup [1] = LoadLocalGameButtonsRow;

		FileDeletePromptGroup = new Selectable[1][];
		FileDeletePromptGroup [0] = FileDeleteButtons;

		SettingsGroup = new Selectable[9][];
		SettingsGroup[0] = SettingsResolutionDropdown;
		SettingsGroup[1] = SettingsResolutionApply;
		SettingsGroup[2] = SettingsFullScreenToggle;
		SettingsGroup[3] = SettingsMouseZoomInversion;
		SettingsGroup[4] = SettingsZoomSensitivity;
		SettingsGroup[5] = SettingsScrollSensitivity;
		SettingsGroup[6] = SettingsMusicVolume;
		SettingsGroup[7] = SettingsSfxVolume;
		SettingsGroup[8] = SettingsButtonsRow;

		ExitGamePromptGroup = new Selectable[1][];
		ExitGamePromptGroup [0] = ExitGameButtons;

		VictoryPanelGroup = new Selectable[1][];
		VictoryPanelGroup [0] = VictoryPanelButtons;

		//Debug.Log ("Define Selectables");

	}

	//this function defines the navigation rules for the currentSelectables
	private void SetNavigationRulesForSelectables(){

		//define rules based on what the current selectable is
		if (CurrentSelectables == ActionMenuButtons) {

			horizontalCycling = false;
			verticalCycling = true;
			selectablesWrap = true;

		} else if (CurrentSelectables == ChatInputField) {

			horizontalCycling = false;
			verticalCycling = false;
			selectablesWrap = false;

		} else if (CurrentSelectables == FileMenuButtons) {

			horizontalCycling = true;
			verticalCycling = false;
			selectablesWrap = true;

		} else if (CurrentSelectables == NextUnitButtons) {

			horizontalCycling = true;
			verticalCycling = false;
			selectablesWrap = true;

		} else if (CurrentSelectables == StatusButton) {

			horizontalCycling = false;
			verticalCycling = false;
			selectablesWrap = false;

		} else if (CurrentSelectables == MoveMenuButtons) {

			horizontalCycling = false;
			verticalCycling = true;
			selectablesWrap = false;


		} else if (CurrentSelectables == PhaserRadarShotButton) {

			horizontalCycling = false;
			verticalCycling = true;
			selectablesWrap = false;

		} else if (CurrentSelectables == PhaserTargetingDropdown) {

			horizontalCycling = false;
			verticalCycling = false;
			selectablesWrap = false;

		} else if (CurrentSelectables == PhaserFireButton) {

			horizontalCycling = false;
			verticalCycling = false;
			selectablesWrap = false;

		} else if (CurrentSelectables == TorpedoLaserShotButton) {

			horizontalCycling = false;
			verticalCycling = true;
			selectablesWrap = false;

		} else if (CurrentSelectables == TorpedoTargetingDropdown) {

			horizontalCycling = false;
			verticalCycling = false;
			selectablesWrap = false;

		} else if (CurrentSelectables == TorpedoFireButtons) {

			horizontalCycling = false;
			verticalCycling = true;
			selectablesWrap = false;

		} else if (CurrentSelectables == TorpedoFireButtons) {

			horizontalCycling = false;
			verticalCycling = true;
			selectablesWrap = false;

		} else if (CurrentSelectables == TractorBeamEngageButton) {

			horizontalCycling = false;
			verticalCycling = false;
			selectablesWrap = false;

		} else if (CurrentSelectables == FlareModeButtons) {

			horizontalCycling = true;
			verticalCycling = false;
			selectablesWrap = true;

		} else if (CurrentSelectables == ItemTargetingDropdown) {

			horizontalCycling = false;
			verticalCycling = false;
			selectablesWrap = false;

		} else if (CurrentSelectables == CrystalButtons) {

			horizontalCycling = false;
			verticalCycling = true;
			selectablesWrap = false;

		} else if (CurrentSelectables == CrewTargetingDropdown) {

			horizontalCycling = false;
			verticalCycling = false;
			selectablesWrap = false;

		} else if (CurrentSelectables == CrewRepairButton) {

			horizontalCycling = false;
			verticalCycling = false;
			selectablesWrap = false;

		} else if (CurrentSelectables == CloakingDeviceEngageButton) {

			horizontalCycling = false;
			verticalCycling = false;
			selectablesWrap = false;

		} else if (CurrentSelectables == BuyPhaserRadarShotButton) {

			horizontalCycling = false;
			verticalCycling = false;
			selectablesWrap = false;

		} else if (CurrentSelectables == BuyPhaserRadarArrayButton) {

			horizontalCycling = false;
			verticalCycling = false;
			selectablesWrap = false;

		} else if (CurrentSelectables == BuyXRayKernelButton) {

			horizontalCycling = false;
			verticalCycling = false;
			selectablesWrap = false;

		} else if (CurrentSelectables == BuyTractorBeamButton) {

			horizontalCycling = false;
			verticalCycling = false;
			selectablesWrap = false;

		} else if (CurrentSelectables == BuyLightTorpedoButton) {

			horizontalCycling = false;
			verticalCycling = false;
			selectablesWrap = false;

		} else if (CurrentSelectables == BuyHeavyTorpedoButton) {

			horizontalCycling = false;
			verticalCycling = false;
			selectablesWrap = false;

		} else if (CurrentSelectables == BuyTorpedoLaserShotButton) {

			horizontalCycling = false;
			verticalCycling = false;
			selectablesWrap = false;

		} else if (CurrentSelectables == BuyTorpedoLaserGuidanceButton) {

			horizontalCycling = false;
			verticalCycling = false;
			selectablesWrap = false;

		} else if (CurrentSelectables == BuyHighPressureTubesButton) {

			horizontalCycling = false;
			verticalCycling = false;
			selectablesWrap = false;

		} else if (CurrentSelectables == BuyDilithiumCrystalButton) {

			horizontalCycling = false;
			verticalCycling = false;
			selectablesWrap = false;

		} else if (CurrentSelectables == BuyTrilithiumCrystalButton) {

			horizontalCycling = false;
			verticalCycling = false;
			selectablesWrap = false;

		} else if (CurrentSelectables == BuyFlareButton) {

			horizontalCycling = false;
			verticalCycling = false;
			selectablesWrap = false;

		} else if (CurrentSelectables == BuyRadarJammingButton) {

			horizontalCycling = false;
			verticalCycling = false;
			selectablesWrap = false;

		} else if (CurrentSelectables == BuyLaserScatteringButton) {

			horizontalCycling = false;
			verticalCycling = false;
			selectablesWrap = false;

		} else if (CurrentSelectables == BuyRepairCrewButton) {

			horizontalCycling = false;
			verticalCycling = false;
			selectablesWrap = false;

		} else if (CurrentSelectables == BuyShieldEngineeringTeamButton) {

			horizontalCycling = false;
			verticalCycling = false;
			selectablesWrap = false;

		} else if (CurrentSelectables == BuyAdditionalBattleCrewButton) {

			horizontalCycling = false;
			verticalCycling = false;
			selectablesWrap = false;

		} else if (CurrentSelectables == BuyWarpBoosterButton) {

			horizontalCycling = false;
			verticalCycling = false;
			selectablesWrap = false;

		} else if (CurrentSelectables == BuyTranswarpBoosterButton) {

			horizontalCycling = false;
			verticalCycling = false;
			selectablesWrap = false;

		} else if (CurrentSelectables == BuyWarpDriveButton) {

			horizontalCycling = false;
			verticalCycling = false;
			selectablesWrap = false;

		} else if (CurrentSelectables == BuyTranswarpDriveButton) {

			horizontalCycling = false;
			verticalCycling = false;
			selectablesWrap = false;

		} else if (CurrentSelectables == BuyItemButtonRow) {

			horizontalCycling = true;
			verticalCycling = false;
			selectablesWrap = true;

		} else if (CurrentSelectables == BuyShipsButtons) {

			horizontalCycling = false;
			verticalCycling = true;
			selectablesWrap = true;

		} else if (CurrentSelectables == BuyShipButtonRow) {

			horizontalCycling = true;
			verticalCycling = false;
			selectablesWrap = true;

		} else if (CurrentSelectables == PlaceNewShipButtons) {

			horizontalCycling = true;
			verticalCycling = false;
			selectablesWrap = true;

		} else if (CurrentSelectables == NameNewShipInputField) {

			horizontalCycling = false;
			verticalCycling = false;
			selectablesWrap = false;

		} else if (CurrentSelectables == NameNewShipButtons) {

			horizontalCycling = true;
			verticalCycling = false;
			selectablesWrap = true;

		} else if (CurrentSelectables == EndTurnButtons) {

			horizontalCycling = true;
			verticalCycling = false;
			selectablesWrap = true;

		} else if (CurrentSelectables == FlareCountDisplay) {

			horizontalCycling = false;
			verticalCycling = false;
			selectablesWrap = false;

		}

		else if (CurrentSelectables == FlareAutoCalcButton) {

			horizontalCycling = false;
			verticalCycling = false;
			selectablesWrap = false;

		}

		else if (CurrentSelectables == UseFlaresButtonRow) {

			horizontalCycling = true;
			verticalCycling = false;
			selectablesWrap = true;

		} else if (CurrentSelectables == RenameShipInputField) {

			horizontalCycling = false;
			verticalCycling = false;
			selectablesWrap = false;

		} else if (CurrentSelectables == RenameShipButtons) {

			horizontalCycling = true;
			verticalCycling = false;
			selectablesWrap = true;

		} else if (CurrentSelectables == StatusButtons) {

			horizontalCycling = true;
			verticalCycling = false;
			selectablesWrap = true;

		}
		else if (CurrentSelectables == SaveLocalGameFiles) {

			horizontalCycling = false;
			verticalCycling = true;
			selectablesWrap = false;

		} else if (CurrentSelectables == SaveLocalGameInputField){

			horizontalCycling = false;
			verticalCycling = false;
			selectablesWrap = false;

		}else if (CurrentSelectables == SaveLocalGameButtonsRow){

			horizontalCycling = true;
			verticalCycling = false;
			selectablesWrap = true;

		} else if (CurrentSelectables == FileOverwriteButtons){

			horizontalCycling = true;
			verticalCycling = false;
			selectablesWrap = true;

		}
		else if (CurrentSelectables == LoadLocalGameFiles) {

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

		} else if (CurrentSelectables == SettingsResolutionDropdown){

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

		} else if (CurrentSelectables == SettingsButtonsRow){

			horizontalCycling = true;
			verticalCycling = false;
			selectablesWrap = true;

		} else if (CurrentSelectables == ExitGameButtons){

			horizontalCycling = true;
			verticalCycling = false;
			selectablesWrap = true;

		} else if (CurrentSelectables == VictoryPanelButtons){

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

		case UIState.Selection:

			//Debug.Log ("case selection");

			//set the current selectables group to match the UI state
			currentSelectablesGroup = SelectionGroup;

			//check if this is a new turn
			if (isNewTurn == true) {

				//turn off the flag
				isNewTurn = false;

				//clear the selectable
				eventSystem.SetSelectedGameObject(null);

				//find the first array in the group that has an interactable selectable
				potentialCurrentSelectionGroupIndex = FindFirstInteractableArrayIndex (currentSelectablesGroup);

				//Debug.Log ("potential is "+ potentialCurrentSelectionGroupIndex);

				//set the selectable array that contains an interactable
				if (potentialCurrentSelectionGroupIndex != -1) {

					//set the currentSelectionGroupIndex
					currentSelectionGroupIndex = potentialCurrentSelectionGroupIndex;

					//set the currentSelectables based on the index returned
					CurrentSelectables = currentSelectablesGroup [currentSelectionGroupIndex];

				}

				break;

			}

			//check if the current selectable is within the new selectables group
			//if the current selectable is within the new group, we don't want set the selectables to the first interactable option,
			//we want to leave it where it is
			//but we will need to know what the group index is within the context of the new group

			//only do this if there is a current selectable
			else if (EventSystem.current.currentSelectedGameObject != null) {

				//Debug.Log ("current not null");

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

				//Debug.Log ("current is null");

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

				//Debug.Log ("potential is "+ potentialCurrentSelectionGroupIndex);

				//set the selectable array that contains an interactable
				if (potentialCurrentSelectionGroupIndex != -1) {

					//set the currentSelectionGroupIndex
					currentSelectionGroupIndex = potentialCurrentSelectionGroupIndex;

					//set the currentSelectables based on the index returned
					CurrentSelectables = currentSelectablesGroup [currentSelectionGroupIndex];

				}

			}

			break;

		case UIState.MoveMenu:

			//set the current selectables group to match the UI state
			currentSelectablesGroup = MoveMenuGroup;

			//find the first array in the group that has an interactable selectable
			potentialCurrentSelectionGroupIndex = FindFirstInteractableArrayIndex (currentSelectablesGroup);

			//check if the potential group index is 0, which would indicate the move dropdown
			if (potentialCurrentSelectionGroupIndex != 0) {

				//if the index is not zero, that means we can't be on the dropdown, because there are no interactable selectables in the dropdown
				//in this case, we want to stay on the toggle that turned on the dropdown
				//check to make sure that current selected object is in the new group

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

						//set the selectable array that contains an interactable
						if (potentialCurrentSelectionGroupIndex != -1) {

							//set the currentSelectionGroupIndex
							currentSelectionGroupIndex = potentialCurrentSelectionGroupIndex;

							//set the currentSelectables based on the index returned
							CurrentSelectables = currentSelectablesGroup [currentSelectionGroupIndex];

							//check if the current selectables is the action menu buttons
							if (CurrentSelectables == ActionMenuButtons) {

								if (CurrentSelectables [0].interactable == true) {

									//set the index
									currentSelectionIndex = 0;

									//set the current selectable to the move toggle
									eventSystem.SetSelectedGameObject (CurrentSelectables [currentSelectionIndex].gameObject);

									//return from the funcction
									return;

								}

							}

						}

					}

				} else {

					//Debug.Log ("current selected is null");

					//if there is no current selectable, we need to find a new valid selectable

					//set the selectable array that contains an interactable
					if (potentialCurrentSelectionGroupIndex != -1) {

						//set the currentSelectionGroupIndex
						currentSelectionGroupIndex = potentialCurrentSelectionGroupIndex;

						//set the currentSelectables based on the index returned
						CurrentSelectables = currentSelectablesGroup [currentSelectionGroupIndex];

						//check if the current selectables is the action menu buttons
						if (CurrentSelectables == ActionMenuButtons) {

							if (CurrentSelectables [0].interactable == true) {

								//set the index
								currentSelectionIndex = 0;

								//set the current selectable to the move toggle
								eventSystem.SetSelectedGameObject (CurrentSelectables [currentSelectionIndex].gameObject);

								//return from the funcction
								return;

							}

						}

					}

				}

			} else {

				//the else is that the potential is group zero

				//set the currentSelectionGroupIndex
				currentSelectionGroupIndex = potentialCurrentSelectionGroupIndex;

				//set the currentSelectables based on the index returned
				CurrentSelectables = currentSelectablesGroup [currentSelectionGroupIndex];

			}

			break;

		case UIState.PhaserMenu:

			//Debug.Log ("case phaser");

			//reset the phaser targeting dropdown value - this prevents a bug where if a radar array system is equipped,
			//the dropdown remembers the previous selected section, but it doesn't count it and doesn't allow firing
			PhaserTargetingDropdown[0].GetComponent<TMP_Dropdown>().value = 0;

			//set the current selectables group to match the UI state
			currentSelectablesGroup = PhaserMenuGroup;

			//find the first array in the group that has an interactable selectable
			potentialCurrentSelectionGroupIndex = FindFirstInteractableArrayIndex (currentSelectablesGroup);

			//Debug.Log ("phaser potentialCurrentSelectionGroupIndex = " + potentialCurrentSelectionGroupIndex);


			//check if the potential group index is 0, which would indicate the phaser dropdown
			if (potentialCurrentSelectionGroupIndex != 0) {

				//if the index is not zero, that means we can't be on the dropdown, because there are no interactable selectables in the dropdown
				//in this case, we want to stay on the toggle that turned on the dropdown
				//check to make sure that current selected object is in the new group

				//only do this if there is a current selectable
				if (EventSystem.current.currentSelectedGameObject != null) {

					//Debug.Log ("current selected not null");

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

						//set the selectable array that contains an interactable
						if (potentialCurrentSelectionGroupIndex != -1) {

							//set the currentSelectionGroupIndex
							currentSelectionGroupIndex = potentialCurrentSelectionGroupIndex;

							//set the currentSelectables based on the index returned
							CurrentSelectables = currentSelectablesGroup [currentSelectionGroupIndex];

							//check if the current selectables is the action menu buttons
							if (CurrentSelectables == ActionMenuButtons) {

								if (CurrentSelectables [1].interactable == true) {

									//set the index
									currentSelectionIndex = 1;

									//set the current selectable to the phaser toggle
									eventSystem.SetSelectedGameObject (CurrentSelectables [currentSelectionIndex].gameObject);

									//return from the funcction
									return;

								}

							}

						}

					}

				} else {

					//Debug.Log ("current selected is null");

					//if there is no current selectable, we need to find a new valid selectable

					//set the selectable array that contains an interactable
					if (potentialCurrentSelectionGroupIndex != -1) {

						//set the currentSelectionGroupIndex
						currentSelectionGroupIndex = potentialCurrentSelectionGroupIndex;

						//set the currentSelectables based on the index returned
						CurrentSelectables = currentSelectablesGroup [currentSelectionGroupIndex];

						//check if the current selectables is the action menu buttons
						if (CurrentSelectables == ActionMenuButtons) {

							if (CurrentSelectables [1].interactable == true) {

								//set the index
								currentSelectionIndex = 1;

								//set the current selectable to the phaser toggle
								eventSystem.SetSelectedGameObject (CurrentSelectables [currentSelectionIndex].gameObject);

								//return from the funcction
								return;

							}

						}

					}

				}

			} else {

				//the else is that the potential is group zero

				//set the currentSelectionGroupIndex
				currentSelectionGroupIndex = potentialCurrentSelectionGroupIndex;

				//set the currentSelectables based on the index returned
				CurrentSelectables = currentSelectablesGroup [currentSelectionGroupIndex];

			}

			break;

		case UIState.TorpedoMenu:

			//reset the torpedo targeting dropdown value - this prevents a bug where if a laser guidance system is equipped,
			//the dropdown remembers the previous selected section, but it doesn't count it and doesn't allow firing
			TorpedoTargetingDropdown[0].GetComponent<TMP_Dropdown>().value = 0;

			//set the current selectables group to match the UI state
			currentSelectablesGroup = TorpedoMenuGroup;

			//find the first array in the group that has an interactable selectable
			potentialCurrentSelectionGroupIndex = FindFirstInteractableArrayIndex (currentSelectablesGroup);

			//Debug.Log ("Torpedo potentialCurrentSelectionGroupIndex = " + potentialCurrentSelectionGroupIndex);

			//check if the potential group index is 0, which would indicate the torpedo dropdown
			if (potentialCurrentSelectionGroupIndex != 0) {

				//if the index is not zero, that means we can't be on the dropdown, because there are no interactable selectables in the dropdown
				//in this case, we want to stay on the toggle that turned on the dropdown
				//check to make sure that current selected object is in the new group

				//only do this if there is a current selectable
				if (EventSystem.current.currentSelectedGameObject != null) {

					//Debug.Log ("torpedo current selected not null");

					for (int i = 0; i < currentSelectablesGroup.Length; i++) {

						//check if the current selectable is within the array at the ith index
						if (currentSelectablesGroup [i].Contains (EventSystem.current.currentSelectedGameObject.GetComponent<Selectable> ())) {

							//set the currentSelectionGroupIndex
							currentSelectionGroupIndex = i;

							//Debug.Log ("torpedo currentSelectionGroupIndex match at i = " + i);

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

						//Debug.Log ("current selectable not in group");

						//if the current is not in the group, we need to find a new valid selectable

						//set the selectable array that contains an interactable
						if (potentialCurrentSelectionGroupIndex != -1) {

							//set the currentSelectionGroupIndex
							currentSelectionGroupIndex = potentialCurrentSelectionGroupIndex;

							//set the currentSelectables based on the index returned
							CurrentSelectables = currentSelectablesGroup [currentSelectionGroupIndex];

							//check if the current selectables is the action menu buttons
							if (CurrentSelectables == ActionMenuButtons) {

								if (CurrentSelectables [2].IsInteractable() == true) {

									//Debug.Log ("torpedo current selectables 2 is interactable");

									//set the index
									currentSelectionIndex = 2;

									//set the current selectable to the torpedo toggle
									eventSystem.SetSelectedGameObject (CurrentSelectables [currentSelectionIndex].gameObject);

									//return from the funcction
									return;

								}

							}

						}

					}

				} else {

					//Debug.Log ("current selected is null");

					//if there is no current selectable, we need to find a new valid selectable

					//set the selectable array that contains an interactable
					if (potentialCurrentSelectionGroupIndex != -1) {

						//set the currentSelectionGroupIndex
						currentSelectionGroupIndex = potentialCurrentSelectionGroupIndex;

						//set the currentSelectables based on the index returned
						CurrentSelectables = currentSelectablesGroup [currentSelectionGroupIndex];

						//check if the current selectables is the action menu buttons
						if (CurrentSelectables == ActionMenuButtons) {

							if (CurrentSelectables [2].interactable == true) {

								//Debug.Log ("current selectables 2 is interactable");

								//set the index
								currentSelectionIndex = 2;

								//set the current selectable to the torpedo toggle
								eventSystem.SetSelectedGameObject (CurrentSelectables [currentSelectionIndex].gameObject);

								//return from the funcction
								return;

							}

						}

					}

				}

			} else {

				//the else is that the potential is group zero

				//Debug.Log ("potential is group zero");


				//set the currentSelectionGroupIndex
				currentSelectionGroupIndex = potentialCurrentSelectionGroupIndex;

				//set the currentSelectables based on the index returned
				CurrentSelectables = currentSelectablesGroup [currentSelectionGroupIndex];

			}

			break;

		case UIState.TractorBeamMenu:

			//set the current selectables group to match the UI state
			currentSelectablesGroup = TractorBeamMenuGroup;

			//find the first array in the group that has an interactable selectable
			potentialCurrentSelectionGroupIndex = FindFirstInteractableArrayIndex (currentSelectablesGroup);

			//Debug.Log ("potentialCurrentSelectionGroupIndex" + potentialCurrentSelectionGroupIndex);

			//check if the potential group index is 0, which would indicate the torpedo dropdown
			if (potentialCurrentSelectionGroupIndex != 0) {

				//if the index is not zero, that means we can't be on the dropdown, because there are no interactable selectables in the dropdown
				//in this case, we want to stay on the toggle that turned on the dropdown
				//check to make sure that current selected object is in the new group

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

						//set the selectable array that contains an interactable
						if (potentialCurrentSelectionGroupIndex != -1) {

							//set the currentSelectionGroupIndex
							currentSelectionGroupIndex = potentialCurrentSelectionGroupIndex;

							//set the currentSelectables based on the index returned
							CurrentSelectables = currentSelectablesGroup [currentSelectionGroupIndex];

							//check if the current selectables is the action menu buttons
							if (CurrentSelectables == ActionMenuButtons) {

								if (CurrentSelectables [3].IsInteractable() == true) {

									//Debug.Log ("torpedo current selectables 2 is interactable");

									//set the index
									currentSelectionIndex = 3;

									//set the current selectable to the torpedo toggle
									eventSystem.SetSelectedGameObject (CurrentSelectables [currentSelectionIndex].gameObject);

									//return from the funcction
									return;

								}

							}

						}

					}

				} else {

					//Debug.Log ("current selected is null");

					//if there is no current selectable, we need to find a new valid selectable

					//set the selectable array that contains an interactable
					if (potentialCurrentSelectionGroupIndex != -1) {

						//set the currentSelectionGroupIndex
						currentSelectionGroupIndex = potentialCurrentSelectionGroupIndex;

						//set the currentSelectables based on the index returned
						CurrentSelectables = currentSelectablesGroup [currentSelectionGroupIndex];

						//check if the current selectables is the action menu buttons
						if (CurrentSelectables == ActionMenuButtons) {

							if (CurrentSelectables [3].interactable == true) {

								//Debug.Log ("current selectables 2 is interactable");

								//set the index
								currentSelectionIndex = 3;

								//set the current selectable to the torpedo toggle
								eventSystem.SetSelectedGameObject (CurrentSelectables [currentSelectionIndex].gameObject);

								//return from the funcction
								return;

							}

						}

					}

				}

			} else {

				//the else is that the potential is group zero

				//set the currentSelectionGroupIndex
				currentSelectionGroupIndex = potentialCurrentSelectionGroupIndex;

				//set the currentSelectables based on the index returned
				CurrentSelectables = currentSelectablesGroup [currentSelectionGroupIndex];

			}

			break;

		case UIState.UseItem:

			//reset the targeting dropdown value - this prevents a bug where if a system is equipped,
			//the dropdown remembers the previous selected section, but it doesn't count it and doesn't allow firing
			ItemTargetingDropdown[0].GetComponent<TMP_Dropdown>().value = 0;

			//set the current selectables group to match the UI state
			currentSelectablesGroup = UseItemMenuGroup;

			//find the first array in the group that has an interactable selectable
			potentialCurrentSelectionGroupIndex = FindFirstInteractableArrayIndex (currentSelectablesGroup);

			//Debug.Log ("potentialCurrentSelectionGroupIndex" + potentialCurrentSelectionGroupIndex);

			//check if the potential group index is 0, which would indicate the torpedo dropdown
			if (potentialCurrentSelectionGroupIndex != 0) {

				//if the index is not zero, that means we can't be on the dropdown, because there are no interactable selectables in the dropdown
				//in this case, we want to stay on the toggle that turned on the dropdown
				//check to make sure that current selected object is in the new group

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

						//set the selectable array that contains an interactable
						if (potentialCurrentSelectionGroupIndex != -1) {

							//set the currentSelectionGroupIndex
							currentSelectionGroupIndex = potentialCurrentSelectionGroupIndex;

							//set the currentSelectables based on the index returned
							CurrentSelectables = currentSelectablesGroup [currentSelectionGroupIndex];

							//check if the current selectables is the action menu buttons
							if (CurrentSelectables == ActionMenuButtons) {

								if (CurrentSelectables [4].IsInteractable() == true) {

									//Debug.Log ("torpedo current selectables 2 is interactable");

									//set the index
									currentSelectionIndex = 4;

									//set the current selectable to the torpedo toggle
									eventSystem.SetSelectedGameObject (CurrentSelectables [currentSelectionIndex].gameObject);

									//return from the funcction
									return;

								}

							}

						}

					}

				} else {

					//Debug.Log ("current selected is null");

					//if there is no current selectable, we need to find a new valid selectable

					//set the selectable array that contains an interactable
					if (potentialCurrentSelectionGroupIndex != -1) {

						//set the currentSelectionGroupIndex
						currentSelectionGroupIndex = potentialCurrentSelectionGroupIndex;

						//set the currentSelectables based on the index returned
						CurrentSelectables = currentSelectablesGroup [currentSelectionGroupIndex];

						//check if the current selectables is the action menu buttons
						if (CurrentSelectables == ActionMenuButtons) {

							if (CurrentSelectables [4].interactable == true) {

								//Debug.Log ("current selectables 2 is interactable");

								//set the index
								currentSelectionIndex = 4;

								//set the current selectable to the torpedo toggle
								eventSystem.SetSelectedGameObject (CurrentSelectables [currentSelectionIndex].gameObject);

								//return from the funcction
								return;

							}

						}

					}

				}

			} else {

				//the else is that the potential is group zero

				//set the currentSelectionGroupIndex
				currentSelectionGroupIndex = potentialCurrentSelectionGroupIndex;

				//set the currentSelectables based on the index returned
				CurrentSelectables = currentSelectablesGroup [currentSelectionGroupIndex];

			}

			break;

		case UIState.Crew:

			//reset the targeting dropdown value - this prevents a bug where if a system is equipped,
			//the dropdown remembers the previous selected section, but it doesn't count it and doesn't allow firing
			CrewTargetingDropdown[0].GetComponent<TMP_Dropdown>().value = 0;

			//set the current selectables group to match the UI state
			currentSelectablesGroup = CrewMenuGroup;

			//find the first array in the group that has an interactable selectable
			potentialCurrentSelectionGroupIndex = FindFirstInteractableArrayIndex (currentSelectablesGroup);

			//Debug.Log ("potentialCurrentSelectionGroupIndex" + potentialCurrentSelectionGroupIndex);

			//check if the potential group index is 0, which would indicate the torpedo dropdown
			if (potentialCurrentSelectionGroupIndex != 0) {

				//if the index is not zero, that means we can't be on the dropdown, because there are no interactable selectables in the dropdown
				//in this case, we want to stay on the toggle that turned on the dropdown
				//check to make sure that current selected object is in the new group

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

						//set the selectable array that contains an interactable
						if (potentialCurrentSelectionGroupIndex != -1) {

							//set the currentSelectionGroupIndex
							currentSelectionGroupIndex = potentialCurrentSelectionGroupIndex;

							//set the currentSelectables based on the index returned
							CurrentSelectables = currentSelectablesGroup [currentSelectionGroupIndex];

							//check if the current selectables is the action menu buttons
							if (CurrentSelectables == ActionMenuButtons) {

								if (CurrentSelectables [5].IsInteractable() == true) {

									//Debug.Log ("torpedo current selectables 2 is interactable");

									//set the index
									currentSelectionIndex = 5;

									//set the current selectable to the torpedo toggle
									eventSystem.SetSelectedGameObject (CurrentSelectables [currentSelectionIndex].gameObject);

									//return from the funcction
									return;

								}

							}

						}

					}

				} else {

					//Debug.Log ("current selected is null");

					//if there is no current selectable, we need to find a new valid selectable

					//set the selectable array that contains an interactable
					if (potentialCurrentSelectionGroupIndex != -1) {

						//set the currentSelectionGroupIndex
						currentSelectionGroupIndex = potentialCurrentSelectionGroupIndex;

						//set the currentSelectables based on the index returned
						CurrentSelectables = currentSelectablesGroup [currentSelectionGroupIndex];

						//check if the current selectables is the action menu buttons
						if (CurrentSelectables == ActionMenuButtons) {

							if (CurrentSelectables [5].interactable == true) {

								//Debug.Log ("current selectables 2 is interactable");

								//set the index
								currentSelectionIndex = 5;

								//set the current selectable to the torpedo toggle
								eventSystem.SetSelectedGameObject (CurrentSelectables [currentSelectionIndex].gameObject);

								//return from the funcction
								return;

							}

						}

					}

				}

			} else {

				//the else is that the potential is group zero

				//set the currentSelectionGroupIndex
				currentSelectionGroupIndex = potentialCurrentSelectionGroupIndex;

				//set the currentSelectables based on the index returned
				CurrentSelectables = currentSelectablesGroup [currentSelectionGroupIndex];

			}

			break;

		case UIState.Cloaking:

			//set the current selectables group to match the UI state
			currentSelectablesGroup = CloakingMenuGroup;

			//find the first array in the group that has an interactable selectable
			potentialCurrentSelectionGroupIndex = FindFirstInteractableArrayIndex (currentSelectablesGroup);

			//Debug.Log ("potentialCurrentSelectionGroupIndex" + potentialCurrentSelectionGroupIndex);

			//check if the potential group index is 0, which would indicate the torpedo dropdown
			if (potentialCurrentSelectionGroupIndex != 0) {

				//if the index is not zero, that means we can't be on the dropdown, because there are no interactable selectables in the dropdown
				//in this case, we want to stay on the toggle that turned on the dropdown
				//check to make sure that current selected object is in the new group

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

						//set the selectable array that contains an interactable
						if (potentialCurrentSelectionGroupIndex != -1) {

							//set the currentSelectionGroupIndex
							currentSelectionGroupIndex = potentialCurrentSelectionGroupIndex;

							//set the currentSelectables based on the index returned
							CurrentSelectables = currentSelectablesGroup [currentSelectionGroupIndex];

							//check if the current selectables is the action menu buttons
							if (CurrentSelectables == ActionMenuButtons) {

								if (CurrentSelectables [6].IsInteractable() == true) {

									//Debug.Log ("torpedo current selectables 2 is interactable");

									//set the index
									currentSelectionIndex = 6;

									//set the current selectable to the torpedo toggle
									eventSystem.SetSelectedGameObject (CurrentSelectables [currentSelectionIndex].gameObject);

									//return from the funcction
									return;

								}

							}

						}

					}

				} else {

					//Debug.Log ("current selected is null");

					//if there is no current selectable, we need to find a new valid selectable

					//set the selectable array that contains an interactable
					if (potentialCurrentSelectionGroupIndex != -1) {

						//set the currentSelectionGroupIndex
						currentSelectionGroupIndex = potentialCurrentSelectionGroupIndex;

						//set the currentSelectables based on the index returned
						CurrentSelectables = currentSelectablesGroup [currentSelectionGroupIndex];

						//check if the current selectables is the action menu buttons
						if (CurrentSelectables == ActionMenuButtons) {

							if (CurrentSelectables [6].interactable == true) {

								//Debug.Log ("current selectables 2 is interactable");

								//set the index
								currentSelectionIndex = 6;

								//set the current selectable to the torpedo toggle
								eventSystem.SetSelectedGameObject (CurrentSelectables [currentSelectionIndex].gameObject);

								//return from the funcction
								return;

							}

						}

					}

				}

			} else {

				//the else is that the potential is group zero

				//set the currentSelectionGroupIndex
				currentSelectionGroupIndex = potentialCurrentSelectionGroupIndex;

				//set the currentSelectables based on the index returned
				CurrentSelectables = currentSelectablesGroup [currentSelectionGroupIndex];

			}

			break;

		case UIState.BuyItem:

			//set the current selectables group to match the UI state
			currentSelectablesGroup = BuyItemGroup;

			//find the first array in the group that has an interactable selectable
			potentialCurrentSelectionGroupIndex = FindFirstInteractableArrayIndex (currentSelectablesGroup);

			//Debug.Log ("potentialCurrentSelectionGroupIndex" + potentialCurrentSelectionGroupIndex);

			//check if the potential group index is not -1, which would be the error return
			if (potentialCurrentSelectionGroupIndex != -1) {

				//set the currentSelectionGroupIndex
				currentSelectionGroupIndex = potentialCurrentSelectionGroupIndex;

				//set the currentSelectables based on the index returned
				CurrentSelectables = currentSelectablesGroup [currentSelectionGroupIndex];

			}

			break;

		case UIState.BuyShip:

			//set the current selectables group to match the UI state
			currentSelectablesGroup = BuyShipGroup;

			//find the first array in the group that has an interactable selectable
			potentialCurrentSelectionGroupIndex = FindFirstInteractableArrayIndex (currentSelectablesGroup);

			//Debug.Log ("potentialCurrentSelectionGroupIndex" + potentialCurrentSelectionGroupIndex);

			//check if the potential group index is not -1, which would be the error return
			if (potentialCurrentSelectionGroupIndex != -1) {
				
					//set the currentSelectionGroupIndex
					currentSelectionGroupIndex = potentialCurrentSelectionGroupIndex;

					//set the currentSelectables based on the index returned
					CurrentSelectables = currentSelectablesGroup [currentSelectionGroupIndex];

			}


			//check if we are returning or if we are coming to this menu fresh
			if (uiManager.GetComponent<PurchaseManager> ().returningBackToPurchaseShip == true && 
				potentialCurrentSelectionGroupIndex == 0) {

				//if we are returning, we want to set the initial selection to match the previously selected ship
				//do a switch case on the selected ship type string
				switch (uiManager.GetComponent<PurchaseManager> ().selectedShipType) {

				case "Scout":

					//set the current selectable to the scout button
					currentSelectionIndex = 0;

					eventSystem.SetSelectedGameObject(CurrentSelectables[currentSelectionIndex].gameObject);

					return;

				case "Bird Of Prey":

					//set the current selectable to the bird of prey button
					currentSelectionIndex = 1;

					eventSystem.SetSelectedGameObject(CurrentSelectables[currentSelectionIndex].gameObject);

					return;


				case "Destroyer":

				//set the current selectable to the destroyer button
				currentSelectionIndex = 2;

					eventSystem.SetSelectedGameObject(CurrentSelectables[currentSelectionIndex].gameObject);

				return;

				case "Starship":

				//set the current selectable to the starship button
				currentSelectionIndex = 3;

					eventSystem.SetSelectedGameObject(CurrentSelectables[currentSelectionIndex].gameObject);

				return;

				default:

					break;

				}

			}

			break;

		case UIState.OutfitShip:

			//set the current selectables group to match the UI state
			currentSelectablesGroup = OutfitShipGroup;

			//find the first array in the group that has an interactable selectable
			potentialCurrentSelectionGroupIndex = FindFirstInteractableArrayIndex (currentSelectablesGroup);

			//Debug.Log ("potentialCurrentSelectionGroupIndex" + potentialCurrentSelectionGroupIndex);

			//check if the potential group index is not -1, which would be the error return
			if (potentialCurrentSelectionGroupIndex != -1) {

				//set the currentSelectionGroupIndex
				currentSelectionGroupIndex = potentialCurrentSelectionGroupIndex;

				//set the currentSelectables based on the index returned
				CurrentSelectables = currentSelectablesGroup [currentSelectionGroupIndex];

			}

			break;

		case UIState.PlaceNewShip:

			//set the current selectables group to match the UI state
			currentSelectablesGroup = PlaceNewShipGroup;

			//find the first array in the group that has an interactable selectable
			potentialCurrentSelectionGroupIndex = FindFirstInteractableArrayIndex (currentSelectablesGroup);

			//Debug.Log ("potentialCurrentSelectionGroupIndex" + potentialCurrentSelectionGroupIndex);

			//check if the potential group index is not -1, which would be the error return
			if (potentialCurrentSelectionGroupIndex != -1) {

				//set the currentSelectionGroupIndex
				currentSelectionGroupIndex = potentialCurrentSelectionGroupIndex;

				//set the currentSelectables based on the index returned
				CurrentSelectables = currentSelectablesGroup [currentSelectionGroupIndex];

			}

			break;

		case UIState.NameNewShip:

			//set the current selectables group to match the UI state
			currentSelectablesGroup = NameNewShipGroup;

			//find the first array in the group that has an interactable selectable
			potentialCurrentSelectionGroupIndex = FindFirstInteractableArrayIndex (currentSelectablesGroup);

			//Debug.Log ("potentialCurrentSelectionGroupIndex" + potentialCurrentSelectionGroupIndex);

			//check if the potential group index is not -1, which would be the error return
			if (potentialCurrentSelectionGroupIndex != -1) {

				//set the currentSelectionGroupIndex
				currentSelectionGroupIndex = potentialCurrentSelectionGroupIndex;

				//set the currentSelectables based on the index returned
				CurrentSelectables = currentSelectablesGroup [currentSelectionGroupIndex];

			}

			break;

		case UIState.RenameUnit:

			//set the current selectables group to match the UI state
			currentSelectablesGroup = RenameShipGroup;

			//find the first array in the group that has an interactable selectable
			potentialCurrentSelectionGroupIndex = FindFirstInteractableArrayIndex (currentSelectablesGroup);

			//Debug.Log ("potentialCurrentSelectionGroupIndex" + potentialCurrentSelectionGroupIndex);

			//check if the potential group index is not -1, which would be the error return
			if (potentialCurrentSelectionGroupIndex != -1) {

				//set the currentSelectionGroupIndex
				currentSelectionGroupIndex = potentialCurrentSelectionGroupIndex;

				//set the currentSelectables based on the index returned
				CurrentSelectables = currentSelectablesGroup [currentSelectionGroupIndex];

			}

			break;

		case UIState.EndTurn:

			//Debug.Log ("End Turn");

			//set the current selectables group to match the UI state
			currentSelectablesGroup = EndTurnGroup;

			//find the first array in the group that has an interactable selectable
			potentialCurrentSelectionGroupIndex = FindFirstInteractableArrayIndex (currentSelectablesGroup);

			//Debug.Log ("potentialCurrentSelectionGroupIndex" + potentialCurrentSelectionGroupIndex);

			//check if the potential group index is not -1, which would be the error return
			if (potentialCurrentSelectionGroupIndex != -1) {

				//set the currentSelectionGroupIndex
				currentSelectionGroupIndex = potentialCurrentSelectionGroupIndex;

				//set the currentSelectables based on the index returned
				CurrentSelectables = currentSelectablesGroup [currentSelectionGroupIndex];

			}

			break;

		case UIState.UseFlares:

			//Debug.Log ("Use Flares");

			//set the current selectables group to match the UI state
			currentSelectablesGroup = UseFlaresGroup;

			//find the first array in the group that has an interactable selectable
			potentialCurrentSelectionGroupIndex = FindFirstInteractableArrayIndex (currentSelectablesGroup);

			//Debug.Log ("potentialCurrentSelectionGroupIndex" + potentialCurrentSelectionGroupIndex);

			//check if the potential group index is not -1, which would be the error return
			if (potentialCurrentSelectionGroupIndex != -1) {

				//set the currentSelectionGroupIndex
				currentSelectionGroupIndex = potentialCurrentSelectionGroupIndex;

				//set the currentSelectables based on the index returned
				CurrentSelectables = currentSelectablesGroup [currentSelectionGroupIndex];

			}

			break;

		case UIState.CombatCutscene:

			//Debug.Log ("cutscene");

			return;

		case UIState.Status:

			//Debug.Log ("status");

			//set the current selectables group to match the UI state
			currentSelectablesGroup = StatusGroup;

			//find the first array in the group that has an interactable selectable
			potentialCurrentSelectionGroupIndex = FindFirstInteractableArrayIndex (currentSelectablesGroup);

			//Debug.Log ("potentialCurrentSelectionGroupIndex" + potentialCurrentSelectionGroupIndex);

			//check if the potential group index is not -1, which would be the error return
			if (potentialCurrentSelectionGroupIndex != -1) {

				//set the currentSelectionGroupIndex
				currentSelectionGroupIndex = potentialCurrentSelectionGroupIndex;

				//set the currentSelectables based on the index returned
				CurrentSelectables = currentSelectablesGroup [currentSelectionGroupIndex];

			}

			break;

		case UIState.SaveLocalGame:

			//reset inputs so it doesn't carry over from a previous ui state
			Input.ResetInputAxes();

			//need to get the game files after the window opens
			//set the size of the LoadLocalGameFiles array
			SaveLocalGameFiles = new Selectable[uiManager.GetComponent<FileSaveWindow> ().fileList.transform.childCount];

			//Debug.Log ("SaveLocalGameFiles.Length = " + SaveLocalGameFiles.Length);

			//loop through the filelist
			for (int i = 0; i < uiManager.GetComponent<FileSaveWindow> ().fileList.transform.childCount; i++) {

				SaveLocalGameFiles [i] = uiManager.GetComponent<FileSaveWindow> ().fileList.transform.GetChild (i).GetComponent<Selectable> ();

			}

			//redefine Selectable Groups since we have dynamically changed the LoadLocalGameFiles
			DefineSelectablesGroups();

			//Debug.Log ("LoadLocalGameFiles length = " + LoadLocalGameFiles.Length);

			//set the current selectables group to match the UI state
			currentSelectablesGroup = SaveLocalGameGroup;

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

				if (CurrentSelectables == SaveLocalGameFiles) {

					//Debug.Log ("currentSelectionIndex = " + currentSelectionIndex);

					//set the scrollbar to the top
					CurrentSelectables [0].transform.parent.parent.GetComponent<ScrollRect> ().verticalNormalizedPosition = 
					1.0f * CurrentSelectables [0].transform.parent.parent.GetComponent<ScrollRect> ().verticalNormalizedPosition;

				}

				//fit the object in the scroll window
				//FitCurrentSelectableIntoScrollRect();

			}

			//check if the previousFileIndex is a valid selectable
			if (CurrentSelectables == SaveLocalGameFiles && previousFileIndex < CurrentSelectables.Length) {

				//set the current selectable to the previous index
				//set the Selected object
				eventSystem.SetSelectedGameObject (CurrentSelectables [previousFileIndex].gameObject);

				//store the index of the current selection
				currentSelectionIndex = previousFileIndex;

				//fit the object in the scroll window
				FitCurrentSelectableIntoScrollRect();

				//Debug.Log ("Set selectable to previous index");

				//call the set save file event
				OnSelectSaveFile.Invoke(CurrentSelectables[currentSelectionIndex].gameObject);

				//return from the function
				return;

			} else if (CurrentSelectables == SaveLocalGameFiles && (previousFileIndex - 1)< CurrentSelectables.Length) {

				//else check if one less than the index is valid - for example if we were the highest index and deleted that file

				//set the current selectable to the previous index - 1
				//set the Selected object
				eventSystem.SetSelectedGameObject (CurrentSelectables [previousFileIndex -1].gameObject);

				//store the index of the current selection
				currentSelectionIndex = previousFileIndex - 1;

				//fit the object in the scroll window
				FitCurrentSelectableIntoScrollRect();

				//call the set save file event
				OnSelectSaveFile.Invoke(CurrentSelectables[currentSelectionIndex].gameObject);

				//return from the function
				return;

			}

			break;

		case UIState.FileOverwritePrompt:

			//clear the saveLocalGameFiles
			SaveLocalGameFiles = null;

			//set the current selectables group to match the UI state
			currentSelectablesGroup = FileOverwritePromptGroup;

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

				//Debug.Log ("current selectables length = " + CurrentSelectables.Length);

				if (CurrentSelectables == LoadLocalGameFiles) {
					
					//set the scrollbar to the top
					CurrentSelectables [0].transform.parent.parent.GetComponent<ScrollRect> ().verticalNormalizedPosition = 
					1.0f * CurrentSelectables [0].transform.parent.parent.GetComponent<ScrollRect> ().verticalNormalizedPosition;

				}
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

		case UIState.Settings:

			//Debug.Log ("Settings");

			//set the current selectables group to match the UI state
			currentSelectablesGroup = SettingsGroup;

			//find the first array in the group that has an interactable selectable
			potentialCurrentSelectionGroupIndex = FindFirstInteractableArrayIndex (currentSelectablesGroup);

			//Debug.Log ("potentialCurrentSelectionGroupIndex = "+ potentialCurrentSelectionGroupIndex);

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

		case UIState.VictoryPanel:

			//set the current selectables group to match the UI state
			currentSelectablesGroup = VictoryPanelGroup;

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


		//note this will only run if we haven't returned out early after finding both indexes and setting the selectables array
		//check if there is an interactable selectable in currentSelectables
		int potentialCurrentSelectionIndex = FindFirstInteractableIndex (CurrentSelectables);


		//Debug.Log ("potentialCurrentSelectionIndex = " + potentialCurrentSelectionIndex);

		//the Find function will return -1 if it can't find a vaild selectable in the array
		if (potentialCurrentSelectionIndex != -1) {

			//set the Selected object
			eventSystem.SetSelectedGameObject (CurrentSelectables [potentialCurrentSelectionIndex].gameObject);

			//Debug.Log ("eventSystem.currentSelectedGameObject.name = " + eventSystem.currentSelectedGameObject.name);

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
			//Debug.Log("selectableGroup [i].Length = " +selectableGroup [i].Length);

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
			if (selectables [i].IsInteractable() == true && selectables [i].IsActive() == true ) {

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
			if (selectables [i].IsInteractable() == true && selectables [i].IsActive() == true) {

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
					if (CurrentSelectables [potentialIndex].IsInteractable () == true && CurrentSelectables [potentialIndex].IsActive () == true) {

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
					if (CurrentSelectables [potentialIndex].IsInteractable () == true && CurrentSelectables [potentialIndex].IsActive () == true) {

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

				//eventSystem.SetSelectedGameObject (returnSelectable.gameObject);

				//AdvanceSelectable (false);
			}

		} else {

			eventSystem.SetSelectedGameObject (null);
		}

		//CurrentUIState = returnUIState;

		OnUIStateChange.Invoke ();


	}

	//this function returns to the first selectable in the group
	private void ReturnToFirstSelectableInCurrentSelectables(){

		if (returnSelectable != null) {

			//find the returnSelectable in the group
			for (int i = 0; i < currentSelectablesGroup.Length; i++) {

				if (currentSelectablesGroup [i].Contains (returnSelectable)) {

					//Debug.Log ("found return selectable group");

					//set the current index to the first available
					currentSelectionIndex = FindFirstInteractableIndex (currentSelectablesGroup [i]);

					//set the selectable to the first available in the group
					eventSystem.SetSelectedGameObject (currentSelectablesGroup [i] [currentSelectionIndex].gameObject);

					//break out of the for loop
					break;

				}

			}

		} else {

			eventSystem.SetSelectedGameObject (null);
		}

		//CurrentUIState = returnUIState;

		OnUIStateChange.Invoke ();


	}


	//this function determines the UI state after returning from a window based on what toggle is active in the action menu
	private void DetermineUIState(){

		//check the action menu buttons
		if (uiManager.GetComponent<MoveToggle> ().moveToggle.isOn == true) {

			CurrentUIState = UIState.MoveMenu;

		} else if (uiManager.GetComponent<PhaserToggle> ().phaserToggle.isOn == true) {

			CurrentUIState = UIState.PhaserMenu;

		} else if (uiManager.GetComponent<TorpedoToggle> ().torpedoToggle.isOn == true) {

			CurrentUIState = UIState.TorpedoMenu;

		} else if (uiManager.GetComponent<TractorBeamToggle> ().tractorBeamToggle.isOn == true) {

			CurrentUIState = UIState.TractorBeamMenu;

		} else if (uiManager.GetComponent<UseItemToggle> ().useItemToggle.isOn == true) {

			CurrentUIState = UIState.UseItem;

		} else if (uiManager.GetComponent<CrewToggle> ().crewToggle.isOn == true) {

			CurrentUIState = UIState.Crew;

		} else if (uiManager.GetComponent<CloakingDeviceToggle> ().cloakingDeviceToggle.isOn == true) {

			CurrentUIState = UIState.Cloaking;

		} else if (uiManager.GetComponent<EndTurnToggle> ().endTurnToggle.isOn == true) {

			CurrentUIState = UIState.EndTurn;

		} else {

			CurrentUIState = UIState.Selection;

		}

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

			//remove listener for move toggle
			uiManager.GetComponent<MoveToggle>().moveToggle.onValueChanged.RemoveListener(MoveToggleSetUIStateAction);

			//remove listener for phaser toggle
			uiManager.GetComponent<PhaserToggle>().phaserToggle.onValueChanged.RemoveListener(PhaserToggleSetUIStateAction);

			//remove listener for phaser toggle
			uiManager.GetComponent<TorpedoToggle>().torpedoToggle.onValueChanged.RemoveListener(TorpedoToggleSetUIStateAction);

			//remove listener for tractor beam toggle
			uiManager.GetComponent<TractorBeamToggle>().tractorBeamToggle.onValueChanged.RemoveListener(TractorBeamToggleSetUIStateAction);

			//remove listener for disengaging tractor beam
			uiManager.GetComponent<TractorBeamMenu>().OnTurnOffTractorBeamToggle.RemoveListener(TractorBeamShipSetUIStateAction);

			//remove listener for use item toggle
			uiManager.GetComponent<UseItemToggle>().useItemToggle.onValueChanged.RemoveListener(UseItemToggleSetUIStateAction);

			//remove listener for crew toggle
			uiManager.GetComponent<CrewToggle>().crewToggle.onValueChanged.RemoveListener(CrewToggleSetUIStateAction);

			//remove listener for cloaking toggle
			uiManager.GetComponent<CloakingDeviceToggle>().cloakingDeviceToggle.onValueChanged.RemoveListener(CloakingToggleSetUIStateAction);

			//remove listener for end turn toggle
			uiManager.GetComponent<EndTurnToggle>().endTurnToggle.onValueChanged.RemoveListener(EndTurnSetUIStateAction);

			//remove listener for disengaging cloaking device
			uiManager.GetComponent<CloakingDeviceMenu>().OnTurnOffCloakingDevice.RemoveListener(CombatUnitSetUIStateAction);
			uiManager.GetComponent<CloakingDeviceMenu>().OnTurnOnCloakingDevice.RemoveListener(CombatUnitSetUIStateAction);

			//remove listeners for purchase item menu
			uiManager.GetComponent<PurchaseManager>().openPurchaseItemsButton.onClick.RemoveListener(BuyItemSetUIStateAction);
			uiManager.GetComponent<PurchaseManager>().cancelPurchaseItemsButton.onClick.RemoveListener(CancelPurchaseItemAction);
			uiManager.GetComponent<PurchaseManager>().purchaseItemsButton.onClick.RemoveListener(AcceptPurchaseItemAction);

			//remove listeners for purchase ship menu
			uiManager.GetComponent<PurchaseManager>().OnOpenPurchaseShip.RemoveListener(BuyShipSetUIStateAction);
			uiManager.GetComponent<PurchaseManager>().OnReturnToPurchaseShip.RemoveListener(BuyShipSetUIStateAction);
			uiManager.GetComponent<PurchaseManager>().cancelPurchaseShipButton.onClick.RemoveListener(CancelPurchaseShipAction);
			uiManager.GetComponent<PurchaseManager>().purchaseShipButton.onClick.RemoveListener(AcceptPurchaseShipAction);

			//remove listeners for placing new unit
			uiManager.GetComponent<InstructionPanel>().OnCancelPlaceUnit.RemoveListener(CancelPlaceNewShipAction);
			uiManager.GetComponent<InstructionPanel>().OnReturnToOutfitShip.RemoveListener(AcceptPurchaseShipAction);

			//remove listeners for name new ship menu
			uiManager.GetComponent<NameNewShip> ().OnPurchasedNewShip.RemoveListener (AcceptNameNewShipAction);
			uiManager.GetComponent<NameNewShip> ().OnCanceledPurchase.RemoveListener (CancelNameNewShipAction);
			uiManager.GetComponent<NameNewShip> ().OnReturnToPlaceUnit.RemoveListener (AcceptPurchaseItemAction);

			//remove listener for rename unit
			uiManager.GetComponent<RenameShip>().renameActionButton.onClick.RemoveListener(RenameShipSetUIStateAction);
			uiManager.GetComponent<RenameShip>().yesButton.onClick.RemoveListener(AcceptRenameUnitAction);
			uiManager.GetComponent<RenameShip>().cancelButton.onClick.RemoveListener(CancelRenameUnitAction);

			//remove listeners for end turn drop down
			uiManager.GetComponent<EndTurnDropDown>().OnCancelEndTurnPrompt.RemoveListener(CancelEndTurnPromptAction);
			uiManager.GetComponent<EndTurnDropDown>().OnAcceptEndTurnPrompt.RemoveListener(AcceptEndTurnPromptAction);

			//remove listeners for status panel
			uiManager.GetComponent<StatusPanel>().openButton.onClick.RemoveListener(StatusSetUIStateAction);
			uiManager.GetComponent<StatusPanel>().closeButton.onClick.RemoveListener(CancelStatusAction);

			//remove listeners for exiting the file save window back to the main menu
			uiManager.GetComponent<FileSaveWindow>().OnCloseFileSaveWindow.RemoveListener(CloseSaveLocalGameWindowAction);

			//remove listener for entering the file overwrite prompt
			uiManager.GetComponent<FileSaveWindow>().OnFileSaveYesClickedExistingFile.RemoveListener(OpenFileOverwritePromptAction);

			//remove listeners for exiting the file overwrite prompt back to the save file window
			uiManager.GetComponent<FileOverwritePrompt>().OnFileOverwriteYesClicked.RemoveListener(StringReturnToFileSaveWindowAction);
			uiManager.GetComponent<FileOverwritePrompt>().OnFileSaveCancelClicked.RemoveListener(OpenSaveLocalGameWindowAction);


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

			//remove listeners for entering the settings menu
			uiManager.GetComponent<Settings>().OnSettingsWindowOpened.RemoveListener(OpenSettingsWindowAction);

			//remove listeners for exiting the settings menu
			uiManager.GetComponent<Settings>().acceptButton.onClick.RemoveListener(CloseSettingsWindowAction);
			uiManager.GetComponent<Settings>().exitButton.onClick.RemoveListener(CloseSettingsWindowAction);

			//remove listener for entering the exit game prompt
			uiManager.GetComponent<ExitGamePrompt>().OnExitGamePromptOpened.RemoveListener(OpenExitGamePromptAction);

			//remove listener for exiting the exit game prompt
			uiManager.GetComponent<ExitGamePrompt>().OnExitGameYesClicked.RemoveListener(CloseExitGamePromptAction);
			uiManager.GetComponent<ExitGamePrompt>().OnExitGameCancelClicked.RemoveListener(CloseExitGamePromptAction);

			//remove listeners for flare use 
			uiManager.GetComponent<FlareManager>().OnShowFlarePanel.RemoveListener(UseFlaresSetUIStateAction);
			uiManager.GetComponent<FlareManager>().OnUseFlaresYes.RemoveListener(UseFlaresYesAction);
			uiManager.GetComponent<FlareManager>().OnUseFlaresCancel.RemoveListener(UseFlaresCancelAction);

			//remove listener for Cutscene starting
			uiManager.GetComponent<CutsceneManager>().OnOpenCutsceneDisplayPanel.RemoveListener(CutsceneSetUIStateAction);

			//remove listener for Cutscene ending
			uiManager.GetComponent<CutsceneManager>().OnCloseCutsceneDisplayPanel.RemoveListener(ReturnToSelectablesFromCombatAction);

			//remove listeners telling us the input fields just ended edit, so we can ignore hitting escape
			uiManager.GetComponent<RenameShip> ().renameInputField.onEndEdit.RemoveListener (InputFieldEndEditIgnoreEscapeAction);
			uiManager.GetComponent<NameNewShip> ().shipNameInputField.onEndEdit.RemoveListener (InputFieldEndEditIgnoreEscapeAction);

			//remove listeners for victory panel
			uiManager.GetComponent<VictoryPanel>().OnOpenVictoryPanel.RemoveListener(OpenVictoryPanelAction);
			uiManager.GetComponent<VictoryPanel>().OnCloseVictoryPanel.RemoveListener(CloseVictoryPanelAction);

		}

		if (gameManager != null) {

			//remove listener for new or loaded turn
			gameManager.OnNewTurn.RemoveListener (NewTurnSetInitialSelectablesAction);
			gameManager.OnLoadedTurn.RemoveListener (NewTurnSetInitialSelectablesAction);


		}

		if (mouseManager != null) {

			//remove listeners for mouse manager actions
			mouseManager.OnSetTargetedUnit.RemoveListener (SetInitialCurrentSelectables);
			mouseManager.OnClearTargetedUnit.RemoveListener (ClearSetInitialSelectablesAction);
			mouseManager.OnSetSelectedUnit.RemoveListener (SetInitialCurrentSelectables);
			mouseManager.OnClearSelectedUnit.RemoveListener (ClearSetInitialSelectablesAction);


			//remove listener for placing new unit
			mouseManager.OnPlacedNewUnit.RemoveListener(NameNewShipSetUIStateAction);

		}

		//remove listener for a pointer click selection
		UISelection.OnSetSelectedGameObject.RemoveListener(SelectableSetSelectionGroupsAction);

		//remove listener for a pointer click
		UISelection.OnClickedSelectable.RemoveListener(PointerClickResolveBlockAction);

		//remove listeners for attacks starting
		PhaserSection.OnFirePhasers.RemoveListener(PhaserAttackSetCombatStateToPhaserAction);
		StarbasePhaserSection1.OnFirePhasers.RemoveListener(PhaserAttackSetCombatStateToPhaserAction);
		StarbasePhaserSection2.OnFirePhasers.RemoveListener(PhaserAttackSetCombatStateToPhaserAction);
		TorpedoSection.OnFireLightTorpedo.RemoveListener (TorpedoAttackSetCombatStateToTorpedoAction);
		TorpedoSection.OnFireHeavyTorpedo.RemoveListener (TorpedoAttackSetCombatStateToTorpedoAction);
		StarbaseTorpedoSection.OnFireLightTorpedo.RemoveListener (TorpedoAttackSetCombatStateToTorpedoAction);
		StarbaseTorpedoSection.OnFireHeavyTorpedo.RemoveListener (TorpedoAttackSetCombatStateToTorpedoAction);

		//remove listener for the redirect click
		UIButtonSelectionRedirect.OnClickedButtonForRedirect.RemoveListener(ButtonRedirectSetSelectedObjectAction);

		//remove listener to the UI selection pointer enter
		UISelection.OnPointerEnterSelectable.RemoveListener(OnPointerEnterTestAction);

		//remove listener to the UI selection pointer enter
		UISelectionNonGroup.OnPointerEnterSelectable.RemoveListener(OnPointerEnterNonGroupTestAction);

		//remove listener to the UI selection pointer click
		UISelectionNonGroup.OnPointerClickSelectable.RemoveListener(OnPointerClickNonGroupTestAction);

	}

}