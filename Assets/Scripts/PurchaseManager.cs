﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System;

public class PurchaseManager : MonoBehaviour {

	//these variables hold the purchase costs of various items
	public const int costStarship = 1200;
	public const int costDestroyer = 800;
	public const int costBirdOfPrey = 500;
	public const int costScout = 300;

	public const int costPhasorRadarShot = 125;
	public const int costPhasorRadarArray = 750;
	public const int costXRayKernel = 600;
	public const int costTractorBeam = 100;

	public const int costLightTorpedo = 50;
	public const int costHeavyTorpedo = 75;
	public const int costTorpedoLaserShot = 100;
	public const int costLaserGuidanceSystem = 600;
	public const int costHighPressureTubes = 150;

	public const int costDilithiumCrystal = 50;
	public const int costTrilithiumCrystal = 100;
	public const int costFlare = 25;
	public const int costRadarJammingSystem = 200;
	public const int costTorpedoLaserScatteringSystem = 200;

	public const int costRepairCrew = 200;
	public const int costShieldEngineeringTeam = 600;
	public const int costBattleCrew = 600;

	public const int costWarpBooster = 50;
	public const int costTranswarpBooster = 75;
	public const int costWarpDrive = 300;
	public const int costTranswarpDrive = 500;

	//managers
	private MouseManager mouseManager;
	private GameManager	gameManager;

	//tileMap
	private TileMap tileMap;

	//item groups
	private GameObject phasorSectionItems;
	private GameObject torpedoSectionItems;
	private GameObject storageSectionItems;
	private GameObject crewSectionItems;
	private GameObject engineSectionItems;

	//phasor section inventory displays
	public Toggle phasorRadarShotInventory;
	public Toggle phasorRadarArrayInventory;
	public Toggle xRayKernelInventory;
	public Toggle tractorBeamInventory;

	//torpedo section inventory displays
	public Toggle lightTorpedoInventory;
	public Toggle heavyTorpedoInventory;
	public Toggle torpedoLaserShotInventory;
	public Toggle torpedoLaserGuidanceInventory;
	public Toggle highPressureTubesInventory;

	//storage section inventory displays
	public Toggle dilithiumCrystalInventory;
	public Toggle trilithiumCrystalInventory;
	public Toggle flareInventory;
	public Toggle radarJammingInventory;
	public Toggle laserScatteringInventory;

	//crew section inventory displays
	public Toggle repairCrewInventory;
	public Toggle shieldEngineeringTeamInventory;
	public Toggle battleCrewInventory;

	//engine section inventory displays
	public Toggle warpBoosterInventory;
	public Toggle transwarpBoosterInventory;
	public Toggle warpDriveInventory;
	public Toggle transwarpDriveInventory;

	//phasor section unit prices
	public TextMeshProUGUI phasorRadarShotUnitPrice;
	public TextMeshProUGUI phasorRadarArrayUnitPrice;
	public TextMeshProUGUI xRayKernelUnitPrice;
	public TextMeshProUGUI tractorBeamUnitPrice;

	//torpedo section unit prices
	public TextMeshProUGUI lightTorpedoUnitPrice;
	public TextMeshProUGUI heavyTorpedoUnitPrice;
	public TextMeshProUGUI torpedoLaserShotUnitPrice;
	public TextMeshProUGUI torpedoLaserGuidanceSystemUnitPrice;
	public TextMeshProUGUI highPressureTubesUnitPrice;

	//storage section unit prices
	public TextMeshProUGUI dilithiumCrystalUnitPrice;
	public TextMeshProUGUI trilithiumCrystalUnitPrice;
	public TextMeshProUGUI flareUnitPrice;
	public TextMeshProUGUI radarJammingSystemUnitPrice;
	public TextMeshProUGUI laserScatteringSystemUnitPrice;

	//crew section unit prices
	public TextMeshProUGUI repairCrewUnitPrice;
	public TextMeshProUGUI shieldEngineeringTeamUnitPrice;
	public TextMeshProUGUI AdditionalBattleCrewUnitPrice;

	//engine section unit prices
	public TextMeshProUGUI warpBoosterUnitPrice;
	public TextMeshProUGUI transwarpBoosterUnitPrice;
	public TextMeshProUGUI warpDriveUnitPrice;
	public TextMeshProUGUI transwarpDriveUnitPrice;

	//up and down buttons
	public Button phasorRadarShotUpButton;
	public Button phasorRadarShotDownButton;
	public Button phasorRadarArrayUpButton;
	public Button phasorRadarArrayDownButton;
	public Button xRayKernelUpButton;
	public Button xRayKernelDownButton;
	public Button tractorBeamUpButton;
	public Button tractorBeamDownButton;

	public Button lightTorpedoUpButton;
	public Button lightTorpedoDownButton;
	public Button heavyTorpedoUpButton;
	public Button heavyTorpedoDownButton;
	public Button torpedoLaserShotUpButton;
	public Button torpedoLaserShotDownButton;
	public Button torpedoLaserGuidanceUpButton;
	public Button torpedoLaserGuidanceDownButton;
	public Button highPressureTubesUpButton;
	public Button highPressureTubesDownButton;

	public Button dilithiumCrystalUpButton;
	public Button dilithiumCrystalDownButton;
	public Button trilithiumCrystalUpButton;
	public Button trilithiumCrystalDownButton;
	public Button flareUpButton;
	public Button flareDownButton;
	public Button radarJammingSystemUpButton;
	public Button radarJammingSystemDownButton;
	public Button laserScatteringSystemUpButton;
	public Button laserScatteringSystemDownButton;

	public Button repairCrewUpButton;
	public Button repairCrewDownButton;
	public Button shieldEngineeringTeamUpButton;
	public Button shieldEngineeringTeamDownButton;
	public Button battleCrewUpButton;
	public Button battleCrewDownButton;

	public Button warpBoosterUpButton;
	public Button warpBoosterDownButton;
	public Button transwarpBoosterUpButton;
	public Button transwarpBoosterDownButton;
	public Button warpDriveUpButton;
	public Button warpDriveDownButton;
	public Button transwarpDriveUpButton;
	public Button transwarpDriveDownButton;

	//phasor section purchase qty displays
	public Toggle phasorRadarShotQuantityToBuy;
	public Toggle phasorRadarArrayQuantityToBuy;
	public Toggle xRayKernelQuantityToBuy;
	public Toggle tractorBeamQuantityToBuy;

	//torpedo section purchase qty displays
	public Toggle lightTorpedoQuantityToBuy;
	public Toggle heavyTorpedoQuantityToBuy;
	public Toggle torpedoLaserShotQuantityToBuy;
	public Toggle torpedoLaserGuidanceQuantityToBuy;
	public Toggle highPressureTubesQuantityToBuy;

	//storage section purchase qty displays
	public Toggle dilithiumCrystalQuantityToBuy;
	public Toggle trilithiumCrystalQuantityToBuy;
	public Toggle flareQuantityToBuy;
	public Toggle radarJammingQuantityToBuy;
	public Toggle laserScatteringQuantityToBuy;

	//crew section purchase qty displays
	public Toggle repairCrewQuantityToBuy;
	public Toggle shieldEngineeringTeamQuantityToBuy;
	public Toggle battleCrewQuantityToBuy;

	//engine section purchase qty displays
	public Toggle warpBoosterQuantityToBuy;
	public Toggle transwarpBoosterQuantityToBuy;
	public Toggle warpDriveQuantityToBuy;
	public Toggle transwarpDriveQuantityToBuy;

	//phasor section purchase value displays
	public Toggle phasorRadarShotValueToBuy;
	public Toggle phasorRadarArrayValueToBuy;
	public Toggle xRayKernelValueToBuy;
	public Toggle tractorBeamValueToBuy;

	//torpedo section purchase value displays
	public Toggle lightTorpedoValueToBuy;
	public Toggle heavyTorpedoValueToBuy;
	public Toggle torpedoLaserShotValueToBuy;
	public Toggle torpedoLaserGuidanceValueToBuy;
	public Toggle highPressureTubesValueToBuy;

	//storage section purchase value displays
	public Toggle dilithiumCrystalValueToBuy;
	public Toggle trilithiumCrystalValueToBuy;
	public Toggle flareValueToBuy;
	public Toggle radarJammingValueToBuy;
	public Toggle laserScatteringValueToBuy;

	//crew section purchase value displays
	public Toggle repairCrewValueToBuy;
	public Toggle shieldEngineeringTeamValueToBuy;
	public Toggle battleCrewValueToBuy;

	//engine section purchase value displays
	public Toggle warpBoosterValueToBuy;
	public Toggle transwarpBoosterValueToBuy;
	public Toggle warpDriveValueToBuy;
	public Toggle transwarpDriveValueToBuy;

	//toggle for ship cost
	public Toggle shipCost;

	public TextMeshProUGUI shipCostTextItemPurchase;

	//toggle for item cost
	public Toggle itemCost;

	//toggle for total cost
	public Toggle totalCost;


	//this button is for the purchase items button
	public Button openPurchaseItemsButton;

	//this button is for making the purchase
	public Button purchaseItemsButton;

	//this button is for clearing the item cart
	public Button clearItemCartButton;

	//this button is for cancelling out of the purchase items
	public Button cancelPurchaseItemsButton;

	//gameObject to hold the purchaseItemPanel
	public GameObject purchaseItemsPanel;


	//scout row stuff
	public Toggle scoutQtyAlreadyPurchasedToggle;
	public Toggle scoutMaxQtyToggle;
	public Button scoutSelectButton;
	public TextMeshProUGUI scoutCostText;
	public RawImage scoutRawImage;
	public Transform scoutPhasorShieldRow;
	public Transform scoutTorpedoShieldRow;
	public Transform scoutStorageShieldRow;
	public Transform scoutCrewShieldRow;
	public Transform scoutEngineShieldRow;
	public Transform scoutPhasorStartingItemsRow;
	public Transform scoutTorpedoStartingItemsRow;
	public Transform scoutStorageStartingItemsRow;
	public Transform scoutCrewStartingItemsRow;
	public Transform scoutEngineStartingItemsRow;

	//birdofprey row stuff
	public Toggle birdOfPreyQtyAlreadyPurchasedToggle;
	public Toggle birdOfPreyMaxQtyToggle;
	public Button birdOfPreySelectButton;
	public TextMeshProUGUI birdOfPreyCostText;
	public RawImage birdOfPreyRawImage;
	public Transform birdOfPreyPhasorShieldRow;
	public Transform birdOfPreyTorpedoShieldRow;
	public Transform birdOfPreyStorageShieldRow;
	public Transform birdOfPreyCrewShieldRow;
	public Transform birdOfPreyEngineShieldRow;
	public Transform birdOfPreyPhasorStartingItemsRow;
	public Transform birdOfPreyTorpedoStartingItemsRow;
	public Transform birdOfPreyStorageStartingItemsRow;
	public Transform birdOfPreyCrewStartingItemsRow;
	public Transform birdOfPreyEngineStartingItemsRow;

	//destroyer row stuff
	public Toggle destroyerQtyAlreadyPurchasedToggle;
	public Toggle destroyerMaxQtyToggle;
	public Button destroyerSelectButton;
	public TextMeshProUGUI destroyerCostText;
	public RawImage destroyerRawImage;
	public Transform destroyerPhasorShieldRow;
	public Transform destroyerTorpedoShieldRow;
	public Transform destroyerStorageShieldRow;
	public Transform destroyerCrewShieldRow;
	public Transform destroyerEngineShieldRow;
	public Transform destroyerPhasorStartingItemsRow;
	public Transform destroyerTorpedoStartingItemsRow;
	public Transform destroyerStorageStartingItemsRow;
	public Transform destroyerCrewStartingItemsRow;
	public Transform destroyerEngineStartingItemsRow;

	//starship row stuff
	public Toggle starshipQtyAlreadyPurchasedToggle;
	public Toggle starshipMaxQtyToggle;
	public Button starshipSelectButton;
	public TextMeshProUGUI starshipCostText;
	public RawImage starshipRawImage;
	public Transform starshipPhasorShieldRow;
	public Transform starshipTorpedoShieldRow;
	public Transform starshipStorageShieldRow;
	public Transform starshipCrewShieldRow;
	public Transform starshipEngineShieldRow;
	public Transform starshipPhasorStartingItemsRow;
	public Transform starshipTorpedoStartingItemsRow;
	public Transform starshipStorageStartingItemsRow;
	public Transform starshipCrewStartingItemsRow;
	public Transform starshipEngineStartingItemsRow;

	public Toggle shipCostToggle;
	public Toggle itemCostToggle;
	public Toggle totalCostToggle;

	public TMP_InputField shipNameInput;

	//this flag lets the purchase items know if we're outfitting a ship
	private bool outfittingShip = false;

	//this holds the type of ship which was selected
	private string selectedShipType;

	//button for the open purchaseShip panel
	public Button openPurchaseShipButton;

	//button for the cancel purchaseShip panel
	public Button cancelPurchaseShipButton;

	//button for the purchase ship button
	public Button purchaseShipButton;

	//gameObject to hold the purchaseShipPanel
	public GameObject purchaseShipPanel;

	//event to annonce purchases
	public PurchaseItemEvent OnPurchaseItems = new PurchaseItemEvent();

	public class PurchaseItemEvent : UnityEvent<Dictionary<string,int>,int,CombatUnit>{}; 

	//event to announce outfitted ship is ready to be placed
	public OutfittedShipEvent OnOutfittedShip = new OutfittedShipEvent();

	public class OutfittedShipEvent : UnityEvent<Dictionary<string,int>,int,CombatUnit.UnitType>{}; 

	//event to send tile and rawImage
	public rawImageEvent OnSetPurchaseShipIcon = new rawImageEvent();

	public class rawImageEvent : UnityEvent<GraphicsManager.MyUnit,RawImage>{}; 

	//color for selected button tab
	private Color selectedButtonColor = new Color (240.0f / 255.0f, 240.0f / 255.0f, 20.0f / 255.0f, 255.0f / 255.0f);

	//unityActions
	private UnityAction openPurchaseItemsPanelAction;
	private UnityAction setOpenButtonStatusAction;
	private UnityAction<Player> playerSetOpenButtonStatusAction;
	private UnityAction<GameManager.ActionMode> actionModeSetOpenButtonStatusAction;
	private UnityAction resolvePurchaseAction;
	private UnityAction resolvePhasorRadarShotUpAction;
	private UnityAction resolvePhasorRadarArrayUpAction;
	private UnityAction resolveXRayKernelUpAction;
	private UnityAction resolveTractorBeamUpAction;
	private UnityAction cancelObsoletePhasorRadarShotAction;
	private UnityAction resolveLightTorpedoUpAction;
	private UnityAction resolveHeavyTorpedoUpAction;
	private UnityAction resolveTorpedoLaserShotUpAction;
	private UnityAction resolveTorpedoLaserGuidanceUpAction;
	private UnityAction resolveHighPressureTubesUpAction;
	private UnityAction cancelObsoleteTorpedoLaserShotAction;
	private UnityAction resolveDilithiumCrystalUpAction;
	private UnityAction resolveTrilithiumCrystalUpAction;
	private UnityAction resolveFlareUpAction;
	private UnityAction resolveRadarJammingSystemUpAction;
	private UnityAction resolveLaserScatteringSystemUpAction;
	private UnityAction resolveRepairCrewUpAction;
	private UnityAction resolveShieldEngineeringUpAction;
	private UnityAction resolveBattleCrewUpAction;
	private UnityAction resolveWarpBoosterUpAction;
	private UnityAction resolveTranswarpBoosterUpAction;
	private UnityAction resolveWarpDriveUpAction;
	private UnityAction resolveTranswarpDriveUpAction;
	private UnityAction cancelObsoleteWarpBoosterAction;
	private UnityAction cancelObsoleteTranswarpBoosterAction;
	private UnityAction resolvePhasorRadarShotDownAction;
	private UnityAction resolvePhasorRadarArrayDownAction;
	private UnityAction resolveXRayKernelDownAction;
	private UnityAction resolveTractorBeamDownAction;
	private UnityAction undoCancelObsoletePhasorRadarShotAction;
	private UnityAction resolveLightTorpedoDownAction;
	private UnityAction resolveHeavyTorpedoDownAction;
	private UnityAction resolveTorpedoLaserShotDownAction;
	private UnityAction resolveTorpedoLaserGuidanceDownAction;
	private UnityAction resolveHighPressureTubesDownAction;
	private UnityAction undoCancelObsoleteTorpedoLaserShotAction;
	private UnityAction resolveDilithiumCrystalDownAction;
	private UnityAction resolveTrilithiumCrystalDownAction;
	private UnityAction resolveFlareDownAction;
	private UnityAction resolveRadarJammingSystemDownAction;
	private UnityAction resolveLaserScatteringSystemDownAction;
	private UnityAction resolveRepairCrewDownAction;
	private UnityAction resolveShieldEngineeringDownAction;
	private UnityAction resolveBattleCrewDownAction;
	private UnityAction resolveWarpBoosterDownAction;
	private UnityAction resolveTranswarpBoosterDownAction;
	private UnityAction resolveWarpDriveDownAction;
	private UnityAction resolveTranswarpDriveDownAction;
	private UnityAction undoCancelObsoleteWarpBoosterAction;
	private UnityAction undoCancelObsoleteTranswarpBoosterAction;
	private UnityAction clearItemCartAction;
	private UnityAction<Player> playerSetPurchaseShipButtonAction;
	private UnityAction<GameManager.ActionMode> actionModeSetPurchaseShipButtonAction;
	private UnityAction purchaseShipButtonAction;
	private UnityAction purchaseScoutButtonAction;
	private UnityAction purchaseBirdOfPreyButtonAction;
	private UnityAction purchaseDestroyerButtonAction;
	private UnityAction purchaseStarshipButtonAction;



	// Use this for initialization
	public void Init () {

		//get the manager
		mouseManager = GameObject.FindGameObjectWithTag ("MouseManager").GetComponent<MouseManager> ();
		gameManager = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameManager> ();

		//get the tileMap
		tileMap = GameObject.FindGameObjectWithTag ("TileMap").GetComponent<TileMap> ();

		//find the purchase items sub section groups
		//added the getChild(0) when I added the outer panel object
		phasorSectionItems = purchaseItemsPanel.transform.GetChild(0).Find("PhasorSectionItems").gameObject;
		torpedoSectionItems = purchaseItemsPanel.transform.GetChild(0).Find("TorpedoSectionItems").gameObject;
		storageSectionItems = purchaseItemsPanel.transform.GetChild(0).Find("StorageSectionItems").gameObject;
		crewSectionItems = purchaseItemsPanel.transform.GetChild(0).Find("CrewSectionItems").gameObject;
		engineSectionItems = purchaseItemsPanel.transform.GetChild(0).Find("EngineSectionItems").gameObject;

		//set actions
		SetActions();

		//add listeners
		AddAllListeners();

		//set the unit prices
		SetUnitPrices();

		//update the totals
		CalculateTotalItemCost();

		//close the panel to start
		ClosePurchaseItemsPanel();

		//set the section data once at initialization
		SetAvailableSectionData();

		//close the purchase ship panel
		CancelPurchaseShipPanel();
		
	}

	//function to set the unityActions
	private void SetActions(){

		//set the actions
		openPurchaseItemsPanelAction = () => {

			//set the flag to false when the panel is opened this way
			outfittingShip = false;

			//can clear the selected ship type
			selectedShipType = null;

			OpenPurchaseItemsPanel();};

		setOpenButtonStatusAction = () => {SetOpenButtonStatus();};
		playerSetOpenButtonStatusAction = (player) => {SetOpenButtonStatus();};
		actionModeSetOpenButtonStatusAction = (actionMode) => {SetOpenButtonStatus();};
		resolvePurchaseAction = () => {

			//check if the flag is set
			if(outfittingShip == false){

				//resolve the purchase based on the selected unit
				ResolvePurchase(mouseManager.selectedUnit.GetComponent<CombatUnit>());

			}
			else{

				//else we are outfitting a ship
				ResolvePurchase(null);

			}

		};

		resolvePhasorRadarShotUpAction = () => {ResolveUpButtonPress(phasorRadarShotQuantityToBuy,costPhasorRadarShot,phasorRadarShotValueToBuy,false);};
		resolvePhasorRadarArrayUpAction = () => {ResolveUpButtonPress(phasorRadarArrayQuantityToBuy,costPhasorRadarArray,phasorRadarArrayValueToBuy,true);};
		resolveXRayKernelUpAction = () => {ResolveUpButtonPress(xRayKernelQuantityToBuy,costXRayKernel,xRayKernelValueToBuy,true);};
		resolveTractorBeamUpAction = () => {ResolveUpButtonPress(tractorBeamQuantityToBuy,costTractorBeam,tractorBeamValueToBuy,true);};
		cancelObsoletePhasorRadarShotAction = () => {

			CancelPurchaseOfObsoletedItem(phasorRadarShotQuantityToBuy,phasorRadarShotValueToBuy);

			//update the totals
			CalculateTotalItemCost();

			//set the purchase button status
			SetPurchaseButtonStatus ();

		};

		resolveLightTorpedoUpAction = () => {ResolveUpButtonPress(lightTorpedoQuantityToBuy,costLightTorpedo,lightTorpedoValueToBuy,false);};
		resolveHeavyTorpedoUpAction = () => {ResolveUpButtonPress(heavyTorpedoQuantityToBuy,costHeavyTorpedo,heavyTorpedoValueToBuy,false);};
		resolveTorpedoLaserShotUpAction = () => {ResolveUpButtonPress(torpedoLaserShotQuantityToBuy,costTorpedoLaserShot,torpedoLaserShotValueToBuy,false);};
		resolveTorpedoLaserGuidanceUpAction = () => {ResolveUpButtonPress(torpedoLaserGuidanceQuantityToBuy,costLaserGuidanceSystem,torpedoLaserGuidanceValueToBuy,true);};
		resolveHighPressureTubesUpAction = () => {ResolveUpButtonPress(highPressureTubesQuantityToBuy,costHighPressureTubes,highPressureTubesValueToBuy,true);};
		cancelObsoleteTorpedoLaserShotAction = () => {

			CancelPurchaseOfObsoletedItem(torpedoLaserShotQuantityToBuy,torpedoLaserShotValueToBuy);

			//update the totals
			CalculateTotalItemCost();

			//set the purchase button status
			SetPurchaseButtonStatus ();

		};

		resolveDilithiumCrystalUpAction = () => {ResolveUpButtonPress(dilithiumCrystalQuantityToBuy,costDilithiumCrystal,dilithiumCrystalValueToBuy,false);};
		resolveTrilithiumCrystalUpAction = () => {ResolveUpButtonPress(trilithiumCrystalQuantityToBuy,costTrilithiumCrystal,trilithiumCrystalValueToBuy,false);};
		resolveFlareUpAction = () => {ResolveUpButtonPress(flareQuantityToBuy,costFlare,flareValueToBuy,false);};
		resolveRadarJammingSystemUpAction = () => {ResolveUpButtonPress(radarJammingQuantityToBuy,costRadarJammingSystem,radarJammingValueToBuy,true);};
		resolveLaserScatteringSystemUpAction = () => {ResolveUpButtonPress(laserScatteringQuantityToBuy,costTorpedoLaserScatteringSystem,laserScatteringValueToBuy,true);};

		resolveRepairCrewUpAction = () => {ResolveUpButtonPress(repairCrewQuantityToBuy,costRepairCrew,repairCrewValueToBuy,true);};
		resolveShieldEngineeringUpAction = () => {ResolveUpButtonPress(shieldEngineeringTeamQuantityToBuy,costShieldEngineeringTeam,shieldEngineeringTeamValueToBuy,true);};
		resolveBattleCrewUpAction = () => {ResolveUpButtonPress(battleCrewQuantityToBuy,costBattleCrew,battleCrewValueToBuy,true);};

		resolveWarpBoosterUpAction = () => {ResolveUpButtonPress(warpBoosterQuantityToBuy,costWarpBooster,warpBoosterValueToBuy,false);};
		resolveTranswarpBoosterUpAction = () => {ResolveUpButtonPress(transwarpBoosterQuantityToBuy,costTranswarpBooster,transwarpBoosterValueToBuy,false);};
		resolveWarpDriveUpAction = () => {ResolveUpButtonPress(warpDriveQuantityToBuy,costWarpDrive,warpDriveValueToBuy,true);};
		resolveTranswarpDriveUpAction = () => {ResolveUpButtonPress(transwarpDriveQuantityToBuy,costTranswarpDrive,transwarpDriveValueToBuy,true);};
		cancelObsoleteWarpBoosterAction = () => {

			CancelPurchaseOfObsoletedItem(warpBoosterQuantityToBuy,warpBoosterValueToBuy);

			//update the totals
			CalculateTotalItemCost();

			//set the purchase button status
			SetPurchaseButtonStatus ();

		};

		cancelObsoleteTranswarpBoosterAction = () => {

			CancelPurchaseOfObsoletedItem(warpBoosterQuantityToBuy,warpBoosterValueToBuy);
			CancelPurchaseOfObsoletedItem(transwarpBoosterQuantityToBuy,transwarpBoosterValueToBuy);
			CancelPurchaseOfObsoletedItem(warpDriveQuantityToBuy,warpDriveValueToBuy);

			//update the totals
			CalculateTotalItemCost();

			//set the purchase button status
			SetPurchaseButtonStatus ();

		};

		resolvePhasorRadarShotDownAction = () => {ResolveDownButtonPress(phasorRadarShotQuantityToBuy,costPhasorRadarShot,phasorRadarShotValueToBuy);};
		resolvePhasorRadarArrayDownAction = () => {ResolveDownButtonPress(phasorRadarArrayQuantityToBuy,costPhasorRadarArray,phasorRadarArrayValueToBuy);};
		resolveXRayKernelDownAction = () => {ResolveDownButtonPress(xRayKernelQuantityToBuy,costXRayKernel,xRayKernelValueToBuy);};
		resolveTractorBeamDownAction = () => {ResolveDownButtonPress(tractorBeamQuantityToBuy,costTractorBeam,tractorBeamValueToBuy);};
		undoCancelObsoletePhasorRadarShotAction = () => {

			//only want to allow the obsoleted item to be bought again if the trigger item is not in inventory
			if(phasorRadarArrayInventory.GetComponentInChildren<TextMeshProUGUI>().text == "0"){

				UndoCancelPurchaseOfObsoletedItem(phasorRadarShotQuantityToBuy,phasorRadarShotValueToBuy);

			}};

		resolveLightTorpedoDownAction = () => {ResolveDownButtonPress(lightTorpedoQuantityToBuy,costLightTorpedo,lightTorpedoValueToBuy);};
		resolveHeavyTorpedoDownAction = () => {ResolveDownButtonPress(heavyTorpedoQuantityToBuy,costHeavyTorpedo,heavyTorpedoValueToBuy);};
		resolveTorpedoLaserShotDownAction = () => {ResolveDownButtonPress(torpedoLaserShotQuantityToBuy,costTorpedoLaserShot,torpedoLaserShotValueToBuy);};
		resolveTorpedoLaserGuidanceDownAction = () => {ResolveDownButtonPress(torpedoLaserGuidanceQuantityToBuy,costLaserGuidanceSystem,torpedoLaserGuidanceValueToBuy);};
		resolveHighPressureTubesDownAction = () => {ResolveDownButtonPress(highPressureTubesQuantityToBuy,costHighPressureTubes,highPressureTubesValueToBuy);};
		undoCancelObsoleteTorpedoLaserShotAction = () => {

			//only want to allow the obsoleted item to be bought again if the trigger item is not in inventory
			if(torpedoLaserGuidanceInventory.GetComponentInChildren<TextMeshProUGUI>().text == "0"){

				UndoCancelPurchaseOfObsoletedItem(torpedoLaserShotQuantityToBuy,torpedoLaserShotValueToBuy);

			}};

		resolveDilithiumCrystalDownAction = () => {ResolveDownButtonPress(dilithiumCrystalQuantityToBuy,costDilithiumCrystal,dilithiumCrystalValueToBuy);};
		resolveTrilithiumCrystalDownAction = () => {ResolveDownButtonPress(trilithiumCrystalQuantityToBuy,costTrilithiumCrystal,trilithiumCrystalValueToBuy);};
		resolveFlareDownAction = () => {ResolveDownButtonPress(flareQuantityToBuy,costFlare,flareValueToBuy);};
		resolveRadarJammingSystemDownAction = () => {ResolveDownButtonPress(radarJammingQuantityToBuy,costRadarJammingSystem,radarJammingValueToBuy);};
		resolveLaserScatteringSystemDownAction = () => {ResolveDownButtonPress(laserScatteringQuantityToBuy,costTorpedoLaserScatteringSystem,laserScatteringValueToBuy);};

		resolveRepairCrewDownAction = () => {ResolveDownButtonPress(repairCrewQuantityToBuy,costRepairCrew,repairCrewValueToBuy);};
		resolveShieldEngineeringDownAction = () => {ResolveDownButtonPress(shieldEngineeringTeamQuantityToBuy,costShieldEngineeringTeam,shieldEngineeringTeamValueToBuy);};
		resolveBattleCrewDownAction = () => {ResolveDownButtonPress(battleCrewQuantityToBuy,costBattleCrew,battleCrewValueToBuy);};
		resolveWarpBoosterDownAction = () => {ResolveDownButtonPress(warpBoosterQuantityToBuy,costWarpBooster,warpBoosterValueToBuy);};
		resolveTranswarpBoosterDownAction = () => {ResolveDownButtonPress(transwarpBoosterQuantityToBuy,costTranswarpBooster,transwarpBoosterValueToBuy);};
		resolveWarpDriveDownAction = () => {ResolveDownButtonPress(warpDriveQuantityToBuy,costWarpDrive,warpDriveValueToBuy);};
		resolveTranswarpDriveDownAction = () => {ResolveDownButtonPress(transwarpDriveQuantityToBuy,costTranswarpDrive,transwarpDriveValueToBuy);};
		undoCancelObsoleteWarpBoosterAction = () => {

			//only want to allow the obsoleted item to be bought again if the trigger item is not in inventory
			if(warpDriveInventory.GetComponentInChildren<TextMeshProUGUI>().text == "0"){

				UndoCancelPurchaseOfObsoletedItem(warpBoosterQuantityToBuy,warpBoosterValueToBuy);

			}};

		undoCancelObsoleteTranswarpBoosterAction = () => {

			//only want to allow the obsoleted item to be bought again if the trigger item is not in inventory
			if(transwarpDriveInventory.GetComponentInChildren<TextMeshProUGUI>().text == "0"){

				UndoCancelPurchaseOfObsoletedItem(warpBoosterQuantityToBuy,warpBoosterValueToBuy);
				UndoCancelPurchaseOfObsoletedItem(transwarpBoosterQuantityToBuy,transwarpBoosterValueToBuy);
				UndoCancelPurchaseOfObsoletedItem(warpDriveQuantityToBuy,warpDriveValueToBuy);

			}};

		clearItemCartAction = () => {

			ClearPurchasedItems(phasorRadarShotQuantityToBuy,phasorRadarShotValueToBuy);
			ClearPurchasedItems(phasorRadarArrayQuantityToBuy,phasorRadarArrayValueToBuy);
			ClearPurchasedItems(xRayKernelQuantityToBuy,xRayKernelValueToBuy);
			ClearPurchasedItems(tractorBeamQuantityToBuy,tractorBeamValueToBuy);

			//only want to allow the obsoleted item to be bought again if the trigger item is not in inventory
			if(phasorRadarArrayInventory.GetComponentInChildren<TextMeshProUGUI>().text == "0"){

				UndoCancelPurchaseOfObsoletedItem(phasorRadarShotQuantityToBuy,phasorRadarShotValueToBuy);

			}

			ClearPurchasedItems(lightTorpedoQuantityToBuy,lightTorpedoValueToBuy);
			ClearPurchasedItems(heavyTorpedoQuantityToBuy,heavyTorpedoValueToBuy);
			ClearPurchasedItems(torpedoLaserShotQuantityToBuy,torpedoLaserShotValueToBuy);
			ClearPurchasedItems(torpedoLaserGuidanceQuantityToBuy,torpedoLaserGuidanceValueToBuy);
			ClearPurchasedItems(highPressureTubesQuantityToBuy,highPressureTubesValueToBuy);

			//only want to allow the obsoleted item to be bought again if the trigger item is not in inventory
			if(torpedoLaserGuidanceInventory.GetComponentInChildren<TextMeshProUGUI>().text == "0"){

				UndoCancelPurchaseOfObsoletedItem(torpedoLaserShotQuantityToBuy,torpedoLaserShotValueToBuy);

			}

			ClearPurchasedItems(dilithiumCrystalQuantityToBuy,dilithiumCrystalValueToBuy);
			ClearPurchasedItems(trilithiumCrystalQuantityToBuy,trilithiumCrystalValueToBuy);
			ClearPurchasedItems(flareQuantityToBuy,flareValueToBuy);
			ClearPurchasedItems(radarJammingQuantityToBuy,radarJammingValueToBuy);
			ClearPurchasedItems(laserScatteringQuantityToBuy,laserScatteringValueToBuy);

			ClearPurchasedItems(repairCrewQuantityToBuy,repairCrewValueToBuy);
			ClearPurchasedItems(shieldEngineeringTeamQuantityToBuy,shieldEngineeringTeamValueToBuy);
			ClearPurchasedItems(battleCrewQuantityToBuy,battleCrewValueToBuy);

			ClearPurchasedItems(warpBoosterQuantityToBuy,warpBoosterValueToBuy);
			ClearPurchasedItems(transwarpBoosterQuantityToBuy,transwarpBoosterValueToBuy);
			ClearPurchasedItems(warpDriveQuantityToBuy,warpDriveValueToBuy);
			ClearPurchasedItems(transwarpDriveQuantityToBuy,transwarpDriveValueToBuy);

			//only want to allow the obsoleted item to be bought again if the trigger item is not in inventory
			if(warpDriveInventory.GetComponentInChildren<TextMeshProUGUI>().text == "0"){

				UndoCancelPurchaseOfObsoletedItem(warpBoosterQuantityToBuy,warpBoosterValueToBuy);

			}

			//only want to allow the obsoleted item to be bought again if the trigger item is not in inventory
			if(transwarpDriveInventory.GetComponentInChildren<TextMeshProUGUI>().text == "0"){

				UndoCancelPurchaseOfObsoletedItem(warpBoosterQuantityToBuy,warpBoosterValueToBuy);
				UndoCancelPurchaseOfObsoletedItem(transwarpBoosterQuantityToBuy,transwarpBoosterValueToBuy);
				UndoCancelPurchaseOfObsoletedItem(warpDriveQuantityToBuy,warpDriveValueToBuy);

			}

			//update the totals
			CalculateTotalItemCost();

			//set the purchase button status
			SetPurchaseButtonStatus ();

		};

		playerSetPurchaseShipButtonAction = (player) => {SetPurchaseShipMenuButton(player);};

		actionModeSetPurchaseShipButtonAction = (actionMode) => {SetPurchaseShipMenuButton(gameManager.currentTurnPlayer);};

		purchaseShipButtonAction = () => {

			//set the flag to false when the panel is opened this way
			outfittingShip = true;

			CancelPurchaseShipPanel();

			OpenPurchaseItemsPanel();};

		purchaseScoutButtonAction = () => {

			HighlightButton(scoutSelectButton);
			UnhighlightButton(birdOfPreySelectButton);
			UnhighlightButton(destroyerSelectButton);
			UnhighlightButton(starshipSelectButton);

			SetShipCostToggleValue(costScout);

			selectedShipType = "Scout";

			purchaseShipButton.interactable = true;


		};

		purchaseBirdOfPreyButtonAction = () => {

			HighlightButton(birdOfPreySelectButton);
			UnhighlightButton(scoutSelectButton);
			UnhighlightButton(destroyerSelectButton);
			UnhighlightButton(starshipSelectButton);

			SetShipCostToggleValue(costBirdOfPrey);

			selectedShipType = "Bird Of Prey";

			purchaseShipButton.interactable = true;


		};

		purchaseDestroyerButtonAction = () => {

			HighlightButton(destroyerSelectButton);
			UnhighlightButton(birdOfPreySelectButton);
			UnhighlightButton(scoutSelectButton);
			UnhighlightButton(starshipSelectButton);

			SetShipCostToggleValue(costDestroyer);

			selectedShipType = "Destroyer";

			purchaseShipButton.interactable = true;


		};

		purchaseStarshipButtonAction = () => {

			HighlightButton(starshipSelectButton);
			UnhighlightButton(birdOfPreySelectButton);
			UnhighlightButton(destroyerSelectButton);
			UnhighlightButton(scoutSelectButton);

			SetShipCostToggleValue(costStarship);

			selectedShipType = "Starship";

			purchaseShipButton.interactable = true;


		};

	}

	//this fucntion adds all listeners
	private void AddAllListeners(){

		//add a listener to the cancel button on-click
		cancelPurchaseItemsButton.onClick.AddListener (ClosePurchaseItemsPanel);

		//add a listener to the open button on-click
		openPurchaseItemsButton.onClick.AddListener (openPurchaseItemsPanelAction);

		//add listeners to changes in the selected unit
		mouseManager.OnSetSelectedUnit.AddListener(setOpenButtonStatusAction);
		mouseManager.OnClearSelectedUnit.AddListener(setOpenButtonStatusAction);

		//add listener for a new turn starting
		gameManager.OnNewTurn.AddListener(playerSetOpenButtonStatusAction);

		//add listener for a loaded turn starting
		gameManager.OnLoadedTurn.AddListener(playerSetOpenButtonStatusAction);

		//add listener for mode change
		gameManager.OnActionModeChange.AddListener(actionModeSetOpenButtonStatusAction);

		//add listener for the purchase button
		purchaseItemsButton.onClick.AddListener(resolvePurchaseAction);

		//add listeners for up button presses
		//phasor buttons
		phasorRadarShotUpButton.onClick.AddListener(resolvePhasorRadarShotUpAction);
		phasorRadarArrayUpButton.onClick.AddListener(resolvePhasorRadarArrayUpAction);
		xRayKernelUpButton.onClick.AddListener(resolveXRayKernelUpAction);
		tractorBeamUpButton.onClick.AddListener(resolveTractorBeamUpAction);
		phasorRadarArrayUpButton.onClick.AddListener(cancelObsoletePhasorRadarShotAction);

		//torpedo buttons
		lightTorpedoUpButton.onClick.AddListener(resolveLightTorpedoUpAction);
		heavyTorpedoUpButton.onClick.AddListener(resolveHeavyTorpedoUpAction);
		torpedoLaserShotUpButton.onClick.AddListener(resolveTorpedoLaserShotUpAction);
		torpedoLaserGuidanceUpButton.onClick.AddListener(resolveTorpedoLaserGuidanceUpAction);
		highPressureTubesUpButton.onClick.AddListener(resolveHighPressureTubesUpAction);
		torpedoLaserGuidanceUpButton.onClick.AddListener(cancelObsoleteTorpedoLaserShotAction);

		//storage buttons
		dilithiumCrystalUpButton.onClick.AddListener(resolveDilithiumCrystalUpAction);
		trilithiumCrystalUpButton.onClick.AddListener(resolveTrilithiumCrystalUpAction);
		flareUpButton.onClick.AddListener(resolveFlareUpAction);
		radarJammingSystemUpButton.onClick.AddListener(resolveRadarJammingSystemUpAction);
		laserScatteringSystemUpButton.onClick.AddListener(resolveLaserScatteringSystemUpAction);

		//crew buttons
		repairCrewUpButton.onClick.AddListener(resolveRepairCrewUpAction);
		shieldEngineeringTeamUpButton.onClick.AddListener(resolveShieldEngineeringUpAction);
		battleCrewUpButton.onClick.AddListener(resolveBattleCrewUpAction);

		//engine buttons
		warpBoosterUpButton.onClick.AddListener(resolveWarpBoosterUpAction);
		transwarpBoosterUpButton.onClick.AddListener(resolveTranswarpBoosterUpAction);
		warpDriveUpButton.onClick.AddListener(resolveWarpDriveUpAction);
		transwarpDriveUpButton.onClick.AddListener(resolveTranswarpDriveUpAction);
		warpDriveUpButton.onClick.AddListener(cancelObsoleteWarpBoosterAction);
		transwarpDriveUpButton.onClick.AddListener(cancelObsoleteTranswarpBoosterAction);

		//add listeners for down button presses
		//phasor buttons
		phasorRadarShotDownButton.onClick.AddListener(resolvePhasorRadarShotDownAction);
		phasorRadarArrayDownButton.onClick.AddListener(resolvePhasorRadarArrayDownAction);
		xRayKernelDownButton.onClick.AddListener(resolveXRayKernelDownAction);
		tractorBeamDownButton.onClick.AddListener(resolveTractorBeamDownAction);
		phasorRadarArrayDownButton.onClick.AddListener(undoCancelObsoletePhasorRadarShotAction);

		//torpedo buttons
		lightTorpedoDownButton.onClick.AddListener(resolveLightTorpedoDownAction);
		heavyTorpedoDownButton.onClick.AddListener(resolveHeavyTorpedoDownAction);
		torpedoLaserShotDownButton.onClick.AddListener(resolveTorpedoLaserShotDownAction);
		torpedoLaserGuidanceDownButton.onClick.AddListener(resolveTorpedoLaserGuidanceDownAction);
		highPressureTubesDownButton.onClick.AddListener(resolveHighPressureTubesDownAction);
		torpedoLaserGuidanceDownButton.onClick.AddListener(undoCancelObsoleteTorpedoLaserShotAction);

		//storage buttons
		dilithiumCrystalDownButton.onClick.AddListener(resolveDilithiumCrystalDownAction);
		trilithiumCrystalDownButton.onClick.AddListener(resolveTrilithiumCrystalDownAction);
		flareDownButton.onClick.AddListener(resolveFlareDownAction);
		radarJammingSystemDownButton.onClick.AddListener(resolveRadarJammingSystemDownAction);
		laserScatteringSystemDownButton.onClick.AddListener(resolveLaserScatteringSystemDownAction);

		//crew buttons
		repairCrewDownButton.onClick.AddListener(resolveRepairCrewDownAction);
		shieldEngineeringTeamDownButton.onClick.AddListener(resolveShieldEngineeringDownAction);
		battleCrewDownButton.onClick.AddListener(resolveBattleCrewDownAction);

		//engine buttons
		warpBoosterDownButton.onClick.AddListener(resolveWarpBoosterDownAction);
		transwarpBoosterDownButton.onClick.AddListener(resolveTranswarpBoosterDownAction);
		warpDriveDownButton.onClick.AddListener(resolveWarpDriveDownAction);
		transwarpDriveDownButton.onClick.AddListener(resolveTranswarpDriveDownAction);
		warpDriveDownButton.onClick.AddListener(undoCancelObsoleteWarpBoosterAction);

		transwarpDriveDownButton.onClick.AddListener(undoCancelObsoleteTranswarpBoosterAction);

		//add listeners for clear items button
		clearItemCartButton.onClick.AddListener (clearItemCartAction);

		///////////////////////////
		/// This is the PurchaseShips section
		/////////////////////////// 

		//add listener for opening the purchase ship panel
		openPurchaseShipButton.onClick.AddListener (OpenPurchaseShipPanel);

		//add listener for canceling the purchase ship panel
		cancelPurchaseShipButton.onClick.AddListener (CancelPurchaseShipPanel);

		//add a listener for starting a new turn
		gameManager.OnNewTurn.AddListener (playerSetPurchaseShipButtonAction);
		gameManager.OnBeginMainPhase.AddListener (playerSetPurchaseShipButtonAction);
		gameManager.OnNewUnitCreated.AddListener (playerSetPurchaseShipButtonAction);

		//add listener for loading a turn
		gameManager.OnLoadedTurn.AddListener (playerSetPurchaseShipButtonAction);

		//add listener for action mode change
		gameManager.OnActionModeChange.AddListener (actionModeSetPurchaseShipButtonAction);

		//add a listener to the outfit purchase button on-click
		purchaseShipButton.onClick.AddListener (purchaseShipButtonAction);

		//add listener for the scout selection button
		scoutSelectButton.onClick.AddListener (purchaseScoutButtonAction);

		//add listener for the bid selection button
		birdOfPreySelectButton.onClick.AddListener (purchaseBirdOfPreyButtonAction);

		//add listener for the destroyer selection button
		destroyerSelectButton.onClick.AddListener (purchaseDestroyerButtonAction);

		//add listener for the starship selection button
		starshipSelectButton.onClick.AddListener (purchaseStarshipButtonAction);

	}

	
	//this function will close the panel
	private void ClosePurchaseItemsPanel(){

		//disable the status panel
		purchaseItemsPanel.SetActive (false);

	}

	//this function will open the panel
	private void OpenPurchaseItemsPanel(){

		//enable the status panel
		purchaseItemsPanel.SetActive (true);

		//set the border color
		SetPurchaseItemBorderColor(gameManager.currentTurnPlayer.color);

		//set the available sections
		SetAvailableSections();

		//hit the clear button
		clearItemCartButton.onClick.Invoke();

		//check the flag
		if (outfittingShip == true) {

			//set the ship cost toggle value to the purchase ship ship cost
			shipCost.gameObject.SetActive (true);
			shipCostTextItemPurchase.gameObject.SetActive (true);
			shipCost.GetComponentInChildren<TextMeshProUGUI> ().text = (shipCostToggle.GetComponentInChildren<TextMeshProUGUI> ().text);

		} else {

			//the else is that we are not outfitting a ship, and the ship cost should not even be shown
			shipCost.GetComponentInChildren<TextMeshProUGUI> ().text = ("0");
			shipCost.gameObject.SetActive (false);
			shipCostTextItemPurchase.gameObject.SetActive (false);

		}

		//hit the clear button
		clearItemCartButton.onClick.Invoke();

		//set the button status
		SetPurchaseButtonStatus ();

	}


	//this function sets the interactability status of the open button
	private void SetOpenButtonStatus(){

		//first check the mode we're in
		if (UIManager.lockMenuActionModes.Contains(gameManager.CurrentActionMode)){

			openPurchaseItemsButton.interactable = false;
		}

		//check if there is a selected unit
		else if (mouseManager.selectedUnit != null) {

			//check if the selected unit is owned by the current turn player
			if (mouseManager.selectedUnit.GetComponent<CombatUnit> ().owner == gameManager.currentTurnPlayer) {

				//if we are adjacent, allow the button to be interactable
				if (tileMap.AdjacentToTileType(mouseManager.selectedUnit.GetComponent<CombatUnit> ().currentLocation,HexMapTile.TileType.NeutralStarbase) == true) {

					openPurchaseItemsButton.interactable = true;

				} else {

					//if we are not adjacent, the button is not interactable
					openPurchaseItemsButton.interactable = false;

				}

			} else {

				//if it is owned by a different player, the button is not interactable
				openPurchaseItemsButton.interactable = false;

			}

		} else {

			//if there is not a selected unit, the button is not interactable
			openPurchaseItemsButton.interactable = false;

		}

	}


	//this function sets the unit prices
	private void SetUnitPrices(){

		//set the unit prices
		phasorRadarShotUnitPrice.text = (costPhasorRadarShot.ToString());
		phasorRadarArrayUnitPrice.text = (costPhasorRadarArray.ToString());
		xRayKernelUnitPrice.text = (costXRayKernel.ToString());
		tractorBeamUnitPrice.text = (costTractorBeam.ToString());

		lightTorpedoUnitPrice.text = (costLightTorpedo.ToString());
		heavyTorpedoUnitPrice.text = (costHeavyTorpedo.ToString());
		torpedoLaserShotUnitPrice.text = (costTorpedoLaserShot.ToString());
		torpedoLaserGuidanceSystemUnitPrice.text = (costLaserGuidanceSystem.ToString());
		highPressureTubesUnitPrice.text = (costHighPressureTubes.ToString());

		dilithiumCrystalUnitPrice.text = (costDilithiumCrystal.ToString());
		trilithiumCrystalUnitPrice.text = (costTrilithiumCrystal.ToString());
		flareUnitPrice.text = (costFlare.ToString());
		radarJammingSystemUnitPrice.text = (costRadarJammingSystem.ToString());
		laserScatteringSystemUnitPrice.text = (costTorpedoLaserScatteringSystem.ToString());

		repairCrewUnitPrice.text = (costRepairCrew.ToString());
		shieldEngineeringTeamUnitPrice.text = (costShieldEngineeringTeam.ToString());
		AdditionalBattleCrewUnitPrice.text = (costBattleCrew.ToString());

		warpBoosterUnitPrice.text = (costWarpBooster.ToString());
		transwarpBoosterUnitPrice.text = (costTranswarpBooster.ToString());
		warpDriveUnitPrice.text = (costWarpDrive.ToString());
		transwarpDriveUnitPrice.text = (costTranswarpDrive.ToString());

	}

	//this function will check what kind of ship is selected and display the appropriate section options
	private void SetAvailableSections(){

		//the reference unit is what will be used to determine the avaialbe sections
		GameObject referenceUnit;

		//check if the flag is set
		if (outfittingShip == false) {

			if (mouseManager.selectedUnit == null) {

				Debug.LogError ("Somehow we got to the PurchaseItemsPanel without a selected unit!");
				return;

			}

			referenceUnit = mouseManager.selectedUnit;

		} else {

			//else the flag is true, and we are basing the reference unit on a ship being newly purchased
			//switch case based on the type
			switch (selectedShipType) {

			case "Scout":
				
				referenceUnit = gameManager.prefabScout.gameObject;
				break;

			case "Bird Of Prey":

				referenceUnit = gameManager.prefabBirdOfPrey.gameObject;
				break;

			case "Destroyer":

				referenceUnit = gameManager.prefabDestroyer.gameObject;
				break;

			case "Starship":

				referenceUnit = gameManager.prefabStarship.gameObject;
				break;

			default:
				Debug.LogError ("Somehow we got to the PurchaseItemsPanel without a selected unit!");

				return;
				//break;

			}

		}

		//check if the selected unit has a phasor section
		if (referenceUnit.GetComponent<PhasorSection> () == true) {

			//check if the phasor section is not destroyed
			if (referenceUnit.GetComponent<PhasorSection> ().isDestroyed == false) {

				//if the phasor section exists and is not destroyed, it should appear in the panel
				phasorSectionItems.SetActive (true);



				//update the inventory display
				SetPhasorSectionCurrentInventory(referenceUnit.GetComponent<CombatUnit>());

				//block any potentially obsolete inventory
				BlockPurchaseOfObsoletePhasorItems(referenceUnit.GetComponent<CombatUnit>());

			} else {

				//if the phasor section is destroyed, it should not appear in the panel
				phasorSectionItems.SetActive (false);

			}

		} else {

			//if the phasor section is not present, it should not appear in the panel
			phasorSectionItems.SetActive (false);

		}

		//check if the selected unit has a torpedo section
		if (referenceUnit.GetComponent<TorpedoSection> () == true) {

			//check if the torpedo section is not destroyed
			if (referenceUnit.GetComponent<TorpedoSection> ().isDestroyed == false) {

				//if the torpedo section exists and is not destroyed, it should appear in the panel
				torpedoSectionItems.SetActive (true);

				//update the inventory display
				SetTorpedoSectionCurrentInventory(referenceUnit.GetComponent<CombatUnit>());

				//block any potentially obsolete inventory
				BlockPurchaseOfObsoleteTorpedoItems(referenceUnit.GetComponent<CombatUnit>());

			} else {

				//if the torpedo section is destroyed, it should not appear in the panel
				torpedoSectionItems.SetActive (false);

			}

		} else {

			//if the torpedo section is not present, it should not appear in the panel
			torpedoSectionItems.SetActive (false);

		}

		//check if the selected unit has a storage section
		if (referenceUnit.GetComponent<StorageSection> () == true) {

			//check if the storage section is not destroyed
			if (referenceUnit.GetComponent<StorageSection> ().isDestroyed == false) {

				//if the torpedo section exists and is not destroyed, it should appear in the panel
				storageSectionItems.SetActive (true);

				//update the inventory display
				SetStorageSectionCurrentInventory(referenceUnit.GetComponent<CombatUnit>());

				//block any potentially obsolete inventory
				BlockPurchaseOfObsoleteStorageItems(referenceUnit.GetComponent<CombatUnit>());

			} else {

				//if the storage section is destroyed, it should not appear in the panel
				storageSectionItems.SetActive (false);

			}

		} else {

			//if the storage section is not present, it should not appear in the panel
			storageSectionItems.SetActive (false);

		}

		//check if the selected unit has a crew section
		if (referenceUnit.GetComponent<CrewSection> () == true) {

			//check if the crew section is not destroyed
			if (referenceUnit.GetComponent<CrewSection> ().isDestroyed == false) {

				//if the crew section exists and is not destroyed, it should appear in the panel
				crewSectionItems.SetActive (true);

				//update the inventory display
				SetCrewSectionCurrentInventory(referenceUnit.GetComponent<CombatUnit>());

				//block any potentially obsolete inventory
				BlockPurchaseOfObsoleteCrewItems(referenceUnit.GetComponent<CombatUnit>());

			} else {

				//if the crew section is destroyed, it should not appear in the panel
				crewSectionItems.SetActive (false);

			}

		} else {

			//if the crew section is not present, it should not appear in the panel
			crewSectionItems.SetActive (false);

		}

		//check if the selected unit has an engine section
		if (referenceUnit.GetComponent<EngineSection> () == true) {

			//check if the engine section is not destroyed
			if (referenceUnit.GetComponent<EngineSection> ().isDestroyed == false) {

				//if the engine section exists and is not destroyed, it should appear in the panel
				engineSectionItems.SetActive (true);

				//update the inventory display
				SetEngineSectionCurrentInventory(referenceUnit.GetComponent<CombatUnit>());

				//block any potentially obsolete inventory
				BlockPurchaseOfObsoleteEngineItems(referenceUnit.GetComponent<CombatUnit>());

			} else {

				//if the engine section is destroyed, it should not appear in the panel
				engineSectionItems.SetActive (false);

			}

		} else {

			//if the engine section is not present, it should not appear in the panel
			engineSectionItems.SetActive (false);

		}

	}

	//this function sets the current inventory of the phasor section items
	private void SetPhasorSectionCurrentInventory(CombatUnit combatUnit){

		//check the flag
		if (outfittingShip == false) {
			
			//set the inventories to match the passed unit inventory
			phasorRadarShotInventory.GetComponentInChildren<TextMeshProUGUI> ().text = (combatUnit.GetComponent<PhasorSection> ().phasorRadarShot.ToString ());

			//check if the unit has a phasor radar array
			if (combatUnit.GetComponent<PhasorSection> ().phasorRadarArray == true) {

				phasorRadarArrayInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("1");

			} else {

				phasorRadarArrayInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("0");

			}

			//check if the unit has an X-ray kernel
			if (combatUnit.GetComponent<PhasorSection> ().xRayKernalUpgrade == true) {

				xRayKernelInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("1");

			} else {

				xRayKernelInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("0");

			}

			//check if the unit has a tractor beam
			if (combatUnit.GetComponent<PhasorSection> ().tractorBeam == true) {

				tractorBeamInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("1");

			} else {

				tractorBeamInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("0");

			}

		} else {

			//this is if we are outfitting a ship.  In this case instead of checking inventory, we need to check the default values
			//do a switch case based on type of combat unit
			switch (selectedShipType) {

			case "Scout":

				phasorRadarShotInventory.GetComponentInChildren<TextMeshProUGUI> ().text = (Scout.startingPhasorRadarShot.ToString());

				if (Scout.startingPhasorRadarArray == true) {

					phasorRadarArrayInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("1");

				} else {

					phasorRadarArrayInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("0");

				}

				if (Scout.startingXRayKernelUpgrade == true) {

					xRayKernelInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("1");

				} else {

					xRayKernelInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("0");

				}

				if (Scout.startingTractorBeam == true) {

					tractorBeamInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("1");

				} else {

					tractorBeamInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("0");

				}

				break;

			case "Bird Of Prey":

				phasorRadarShotInventory.GetComponentInChildren<TextMeshProUGUI> ().text = (BirdOfPrey.startingPhasorRadarShot.ToString());

				if (BirdOfPrey.startingPhasorRadarArray == true) {

					phasorRadarArrayInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("1");

				} else {

					phasorRadarArrayInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("0");

				}

				if (BirdOfPrey.startingXRayKernelUpgrade == true) {

					xRayKernelInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("1");

				} else {

					xRayKernelInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("0");

				}

				if (BirdOfPrey.startingTractorBeam == true) {

					tractorBeamInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("1");

				} else {

					tractorBeamInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("0");

				}

				break;

			case "Destroyer":

				phasorRadarShotInventory.GetComponentInChildren<TextMeshProUGUI> ().text = (Destroyer.startingPhasorRadarShot.ToString());

				if (Destroyer.startingPhasorRadarArray == true) {

					phasorRadarArrayInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("1");

				} else {

					phasorRadarArrayInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("0");

				}

				if (Destroyer.startingXRayKernelUpgrade == true) {

					xRayKernelInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("1");

				} else {

					xRayKernelInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("0");

				}

				if (Destroyer.startingTractorBeam == true) {

					tractorBeamInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("1");

				} else {

					tractorBeamInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("0");

				}

				break;

			case "Starship":

				phasorRadarShotInventory.GetComponentInChildren<TextMeshProUGUI> ().text = (Starship.startingPhasorRadarShot.ToString());

				if (Starship.startingPhasorRadarArray == true) {

					phasorRadarArrayInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("1");

				} else {

					phasorRadarArrayInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("0");

				}

				if (Starship.startingXRayKernelUpgrade == true) {

					xRayKernelInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("1");

				} else {

					xRayKernelInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("0");

				}

				if (Starship.startingTractorBeam == true) {

					tractorBeamInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("1");

				} else {

					tractorBeamInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("0");

				}

				break;

			default:

				phasorRadarShotInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("0");
				phasorRadarArrayInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("0");
				xRayKernelInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("0");
				tractorBeamInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("0");

				break;

			}

		}

	}

	//this function sets the current inventory of the torpedo section items
	private void SetTorpedoSectionCurrentInventory(CombatUnit combatUnit){

		//check the flag
		if (outfittingShip == false) {

			//set the inventories to match the passed unit inventory
			lightTorpedoInventory.GetComponentInChildren<TextMeshProUGUI> ().text = (combatUnit.GetComponent<TorpedoSection> ().lightTorpedos.ToString ());
			heavyTorpedoInventory.GetComponentInChildren<TextMeshProUGUI> ().text = (combatUnit.GetComponent<TorpedoSection> ().heavyTorpedos.ToString ());
			torpedoLaserShotInventory.GetComponentInChildren<TextMeshProUGUI> ().text = (combatUnit.GetComponent<TorpedoSection> ().torpedoLaserShot.ToString ());

				//check if the unit has a torpedo laser guidance system
			if (combatUnit.GetComponent<TorpedoSection> ().torpedoLaserGuidanceSystem == true) {

				torpedoLaserGuidanceInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("1");

			} else {

				torpedoLaserGuidanceInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("0");

			}

			//check if the unit has high pressure tubes
			if (combatUnit.GetComponent<TorpedoSection> ().highPressureTubes == true) {

				highPressureTubesInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("1");

			} else {

				highPressureTubesInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("0");

			}

		} else {

			//this is if we are outfitting a ship.  In this case instead of checking inventory, we need to check the default values
			//do a switch case based on type of combat unit
			switch (selectedShipType) {

			case "Bird Of Prey":

				lightTorpedoInventory.GetComponentInChildren<TextMeshProUGUI> ().text = (BirdOfPrey.startingLightTorpedos.ToString ());
				heavyTorpedoInventory.GetComponentInChildren<TextMeshProUGUI> ().text = (BirdOfPrey.startingHeavyTorpedos.ToString ());
				torpedoLaserShotInventory.GetComponentInChildren<TextMeshProUGUI> ().text = (BirdOfPrey.startingTorpedoLaserShot.ToString ());

				//check if the unit has a torpedo laser guidance system
				if (BirdOfPrey.startingTorpedoLaserGuidanceSystem == true) {

					torpedoLaserGuidanceInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("1");

				} else {

					torpedoLaserGuidanceInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("0");

				}

				//check if the unit has high pressure tubes
				if (BirdOfPrey.startingHighPressureTubes == true) {

					highPressureTubesInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("1");

				} else {

					highPressureTubesInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("0");

				}

				break;

			case "Destroyer":

				lightTorpedoInventory.GetComponentInChildren<TextMeshProUGUI> ().text = (Destroyer.startingLightTorpedos.ToString ());
				heavyTorpedoInventory.GetComponentInChildren<TextMeshProUGUI> ().text = (Destroyer.startingHeavyTorpedos.ToString ());
				torpedoLaserShotInventory.GetComponentInChildren<TextMeshProUGUI> ().text = (Destroyer.startingTorpedoLaserShot.ToString ());

				//check if the unit has a torpedo laser guidance system
				if (Destroyer.startingTorpedoLaserGuidanceSystem == true) {

					torpedoLaserGuidanceInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("1");

				} else {

					torpedoLaserGuidanceInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("0");;

				}

				//check if the unit has high pressure tubes
				if (Destroyer.startingHighPressureTubes == true) {

					highPressureTubesInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("1");

				} else {

					highPressureTubesInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("0");

				}

				break;

			case "Starship":

				lightTorpedoInventory.GetComponentInChildren<TextMeshProUGUI> ().text = (Starship.startingLightTorpedos.ToString ());
				heavyTorpedoInventory.GetComponentInChildren<TextMeshProUGUI> ().text = (Starship.startingHeavyTorpedos.ToString ());
				torpedoLaserShotInventory.GetComponentInChildren<TextMeshProUGUI> ().text = (Starship.startingTorpedoLaserShot.ToString ());

				//check if the unit has a torpedo laser guidance system
				if (Starship.startingTorpedoLaserGuidanceSystem == true) {

					torpedoLaserGuidanceInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("1");

				} else {

					torpedoLaserGuidanceInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("0");

				}

				//check if the unit has high pressure tubes
				if (Starship.startingHighPressureTubes == true) {

					highPressureTubesInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("1");

				} else {

					highPressureTubesInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("0");

				}

				break;

			default:

				lightTorpedoInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("0");
				heavyTorpedoInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("0");
				torpedoLaserShotInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("0");
				torpedoLaserGuidanceInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("0");
				highPressureTubesInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("0");

				break;

			}

		}

	}

	//this function sets the current inventory of the storage section items
	private void SetStorageSectionCurrentInventory(CombatUnit combatUnit){

		//check the flag
		if (outfittingShip == false) {

			//set the inventories to match the passed unit inventory
			dilithiumCrystalInventory.GetComponentInChildren<TextMeshProUGUI> ().text = (combatUnit.GetComponent<StorageSection> ().dilithiumCrystals.ToString ());
			trilithiumCrystalInventory.GetComponentInChildren<TextMeshProUGUI> ().text = (combatUnit.GetComponent<StorageSection> ().trilithiumCrystals.ToString ());
			flareInventory.GetComponentInChildren<TextMeshProUGUI> ().text = (combatUnit.GetComponent<StorageSection> ().flares.ToString ());

			//check if the unit has a radar jamming system
			if (combatUnit.GetComponent<StorageSection> ().radarJammingSystem == true) {

				radarJammingInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("1");

			} else {

				radarJammingInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("0");

			}

			//check if the unit has a laser scattering system
			if (combatUnit.GetComponent<StorageSection> ().laserScatteringSystem == true) {

				laserScatteringInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("1");

			} else {

				laserScatteringInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("0");

			}

		} else {

			//this is if we are outfitting a ship.  In this case instead of checking inventory, we need to check the default values
			//do a switch case based on type of combat unit
			switch (selectedShipType) {

			case "Scout":

				dilithiumCrystalInventory.GetComponentInChildren<TextMeshProUGUI> ().text = (Scout.startingDilithiumCrystals.ToString ());
				trilithiumCrystalInventory.GetComponentInChildren<TextMeshProUGUI> ().text = (Scout.startingTrilithiumCrystals.ToString ());
				flareInventory.GetComponentInChildren<TextMeshProUGUI> ().text = (Scout.startingFlares.ToString ());

				//check if the unit has a radar jamming system
				if (Scout.startingRadarJammingSystem == true) {

					radarJammingInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("1");

				} else {

					radarJammingInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("0");

				}

				//check if the unit has a laser scattering system
				if (Scout.startingLaserScatteringSystem == true) {

					laserScatteringInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("1");

				} else {

					laserScatteringInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("0");

				}

				break;

			case "Destroyer":

				dilithiumCrystalInventory.GetComponentInChildren<TextMeshProUGUI> ().text = (Destroyer.startingDilithiumCrystals.ToString ());
				trilithiumCrystalInventory.GetComponentInChildren<TextMeshProUGUI> ().text = (Destroyer.startingTrilithiumCrystals.ToString ());
				flareInventory.GetComponentInChildren<TextMeshProUGUI> ().text = (Destroyer.startingFlares.ToString ());

				//check if the unit has a radar jamming system
				if (Destroyer.startingRadarJammingSystem == true) {

					radarJammingInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("1");

				} else {

					radarJammingInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("0");

				}

				//check if the unit has a laser scattering system
				if (Destroyer.startingLaserScatteringSystem == true) {

					laserScatteringInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("1");

				} else {

					laserScatteringInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("0");

				}

				break;

			case "Starship":

				dilithiumCrystalInventory.GetComponentInChildren<TextMeshProUGUI> ().text = (Starship.startingDilithiumCrystals.ToString ());
				trilithiumCrystalInventory.GetComponentInChildren<TextMeshProUGUI> ().text = (Starship.startingTrilithiumCrystals.ToString ());
				flareInventory.GetComponentInChildren<TextMeshProUGUI> ().text = (Starship.startingFlares.ToString ());

				//check if the unit has a radar jamming system
				if (Starship.startingRadarJammingSystem == true) {

					radarJammingInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("1");

				} else {

					radarJammingInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("0");

				}

				//check if the unit has a laser scattering system
				if (Starship.startingLaserScatteringSystem == true) {

					laserScatteringInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("1");

				} else {

					laserScatteringInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("0");

				}

				break;

			default:

				dilithiumCrystalInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("0");
				trilithiumCrystalInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("0");
				flareInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("0");
				radarJammingInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("0");
				laserScatteringInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("0");

				break;

			}

		}

	}

	//this function sets the current inventory of the crew section items
	private void SetCrewSectionCurrentInventory(CombatUnit combatUnit){

		//check the flag
		if (outfittingShip == false) {
			
			//check if the unit has a repair crew
			if (combatUnit.GetComponent<CrewSection> ().repairCrew == true) {

				repairCrewInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("1");

			} else {

				repairCrewInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("0");

			}

			//check if the unit has a shield engineering team
			if (combatUnit.GetComponent<CrewSection> ().shieldEngineeringTeam == true) {

				shieldEngineeringTeamInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("1");

			} else {

				shieldEngineeringTeamInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("0");

			}

			//check if the unit has a battle crew
			if (combatUnit.GetComponent<CrewSection> ().battleCrew == true) {

				battleCrewInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("1");

			} else {

				battleCrewInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("0");

			}

		} else {

			//this is if we are outfitting a ship.  In this case instead of checking inventory, we need to check the default values
			//do a switch case based on type of combat unit
			switch (selectedShipType) {

			case "Starship":

				//check if the unit has a repair crew
				if (Starship.startingRepairCrew == true) {

					repairCrewInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("1");

				} else {

					repairCrewInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("0");

				}

				//check if the unit has a shield engineering team
				if (Starship.startingShieldEngineeringTeam == true) {

					shieldEngineeringTeamInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("1");

				} else {

					shieldEngineeringTeamInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("0");

				}

				//check if the unit has a battle crew
				if (Starship.startingBattleCrew == true) {

					battleCrewInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("1");

				} else {

					battleCrewInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("0");

				}

				break;

			default:

				repairCrewInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("0");
				shieldEngineeringTeamInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("0");
				battleCrewInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("0");

				break;

			}

		}

	}

	//this function sets the current inventory of the engine section items
	private void SetEngineSectionCurrentInventory(CombatUnit combatUnit){

		//check the flag
		if (outfittingShip == false) {
			
			//set the inventories to match the passed unit inventory
			warpBoosterInventory.GetComponentInChildren<TextMeshProUGUI> ().text = (combatUnit.GetComponent<EngineSection> ().warpBooster.ToString ());
			transwarpBoosterInventory.GetComponentInChildren<TextMeshProUGUI> ().text = (combatUnit.GetComponent<EngineSection> ().transwarpBooster.ToString ());

			//check if the unit has warp drive
			if (combatUnit.GetComponent<EngineSection> ().warpDrive == true) {

				warpDriveInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("1");

			} else {

				warpDriveInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("0");

			}

			//check if the unit has transwarp drive
			if (combatUnit.GetComponent<EngineSection> ().transwarpDrive == true) {

				transwarpDriveInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("1");

			} else {

				transwarpDriveInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("0");

			}

		} else {

			//this is if we are outfitting a ship.  In this case instead of checking inventory, we need to check the default values
			//do a switch case based on type of combat unit
			switch (selectedShipType) {

			case "Scout":

				warpBoosterInventory.GetComponentInChildren<TextMeshProUGUI> ().text = (Scout.startingWarpBooster.ToString ());
				transwarpBoosterInventory.GetComponentInChildren<TextMeshProUGUI> ().text = (Scout.startingTranswarpBooster.ToString ());

				//check if the unit has warp drive
				if (Scout.startingWarpDrive == true) {

					warpDriveInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("1");

				} else {

					warpDriveInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("0");

				}

				//check if the unit has transwarp drive
				if (Scout.startingTranswarpDrive == true) {

					transwarpDriveInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("1");

				} else {

					transwarpDriveInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("0");

				}

				break;

			case "Bird Of Prey":

				warpBoosterInventory.GetComponentInChildren<TextMeshProUGUI> ().text = (BirdOfPrey.startingWarpBooster.ToString ());
				transwarpBoosterInventory.GetComponentInChildren<TextMeshProUGUI> ().text = (BirdOfPrey.startingTranswarpBooster.ToString ());

				//check if the unit has warp drive
				if (BirdOfPrey.startingWarpDrive == true) {

					warpDriveInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("1");

				} else {

					warpDriveInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("0");

				}

				//check if the unit has transwarp drive
				if (BirdOfPrey.startingTranswarpDrive == true) {

					transwarpDriveInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("1");

				} else {

					transwarpDriveInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("0");

				}

				break;

			case "Destroyer":

				warpBoosterInventory.GetComponentInChildren<TextMeshProUGUI> ().text = (Destroyer.startingWarpBooster.ToString ());
				transwarpBoosterInventory.GetComponentInChildren<TextMeshProUGUI> ().text = (Destroyer.startingTranswarpBooster.ToString ());

				//check if the unit has warp drive
				if (Destroyer.startingWarpDrive == true) {

					warpDriveInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("1");

				} else {

					warpDriveInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("0");

				}

				//check if the unit has transwarp drive
				if (Destroyer.startingTranswarpDrive == true) {

					transwarpDriveInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("1");

				} else {

					transwarpDriveInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("0");

				}

				break;

			case "Starship":

				warpBoosterInventory.GetComponentInChildren<TextMeshProUGUI> ().text = (Starship.startingWarpBooster.ToString ());
				transwarpBoosterInventory.GetComponentInChildren<TextMeshProUGUI> ().text = (Starship.startingTranswarpBooster.ToString ());

				//check if the unit has warp drive
				if (Starship.startingWarpDrive == true) {

					warpDriveInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("1");

				} else {

					warpDriveInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("0");

				}

				//check if the unit has transwarp drive
				if (Starship.startingTranswarpDrive == true) {

					transwarpDriveInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("1");

				} else {

					transwarpDriveInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("0");;

				}

				break;

			default:

				warpBoosterInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("0");
				transwarpBoosterInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("0");
				warpDriveInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("0");
				transwarpDriveInventory.GetComponentInChildren<TextMeshProUGUI> ().text = ("0");

				break;

			}

		}

	}

	//this function adjusts the purchase qty based on an up button press
	private void ResolveUpButtonPress(Toggle qtyToBuyToggle, int unitPrice, Toggle valueToBuyToggle, bool isPermanentUpgrade){

		//parse the string to convert to an int
		int qtyToBuy = 0;
		Int32.TryParse (qtyToBuyToggle.GetComponentInChildren<TextMeshProUGUI> ().text, out qtyToBuy);

		//check if the item is a permanent upgrade
		if (isPermanentUpgrade == true) {

			//if it is a permanent upgrade, we want to cap the qtyToBuy at 1, or in other words, only increment if it's 0;
			if (qtyToBuy == 0) {

				qtyToBuy++;

			}
			//else do nothing

		} else {

			//this else is if it is not a permanent upgrade.  In this case, we can always increment
			qtyToBuy++;

			//cap the qty at 99 for aesthetics
			if (qtyToBuy > 99) {

				qtyToBuy = 99;

			}

		}

		//check if the qtyToBuyToggle is interactable
		if (qtyToBuyToggle.interactable == true) {
			
			//update the qtyToBuy string to the new qty
			qtyToBuyToggle.GetComponentInChildren<TextMeshProUGUI> ().text = (qtyToBuy.ToString ());

		}

		//check if the valueToBuyToggle is interactable
		if (valueToBuyToggle.interactable == true) {
			
			//update the valueToBuy string
			valueToBuyToggle.GetComponentInChildren<TextMeshProUGUI> ().text = ((qtyToBuy * unitPrice).ToString ());

		}

		//update the totals
		CalculateTotalItemCost();

		//set the purchase button status
		SetPurchaseButtonStatus ();

	}

	//this function resolves an up button press on a permanent item which should cancel purchases of obsoleted items
	private void CancelPurchaseOfObsoletedItem(Toggle qtyToBuyToggle, Toggle valueToBuyToggle){

		//set the qtyToBuy and valueToBuy to 0
		qtyToBuyToggle.GetComponentInChildren<TextMeshProUGUI>().text = ("0");
		valueToBuyToggle.GetComponentInChildren<TextMeshProUGUI>().text = ("0");

		//make those toggles not interactible
		qtyToBuyToggle.interactable = false;
		valueToBuyToggle.interactable = false;

	}

	//this function adjusts the purchase qty based on a down button press
	private void ResolveDownButtonPress(Toggle qtyToBuyToggle, int unitPrice, Toggle valueToBuyToggle){

		//parse the string to convert to an int
		int qtyToBuy = 0;
		Int32.TryParse (qtyToBuyToggle.GetComponentInChildren<TextMeshProUGUI> ().text, out qtyToBuy);

		//decrement the qtyToBuy
		qtyToBuy--;

		//check if the qty is now less than zero, if so set to zero
		if (qtyToBuy < 0) {

			qtyToBuy = 0;

		}

		//check if the qtyToBuyToggle is interactable
		if (qtyToBuyToggle.interactable == true) {

			//update the qtyToBuy string to the new qty
			qtyToBuyToggle.GetComponentInChildren<TextMeshProUGUI> ().text = (qtyToBuy.ToString ());

		}

		//check if the valueToBuyToggle is interactable
		if (valueToBuyToggle.interactable == true) {

			//update the valueToBuy string
			valueToBuyToggle.GetComponentInChildren<TextMeshProUGUI> ().text = ((qtyToBuy * unitPrice).ToString ());

		}

		//update the totals
		CalculateTotalItemCost();

		//set the purchase button status
		SetPurchaseButtonStatus ();

	}

	//this function resolves a down button press on a permanent item which should undo canceling purchases of obsoleted items
	private void UndoCancelPurchaseOfObsoletedItem(Toggle qtyToBuyToggle, Toggle valueToBuyToggle){

		//make those toggles interactible
		qtyToBuyToggle.interactable = true;
		valueToBuyToggle.interactable = true;

	}

	//this function prevents purchasing obsolete phasor items based on current inventory
	private void BlockPurchaseOfObsoletePhasorItems(CombatUnit combatUnit){

		//check if the unit has a phasor radar array
		if (combatUnit.GetComponent<PhasorSection> ().phasorRadarArray == true) {

			//need to block buying phasor radar shot and phasor radar array
			CancelPurchaseOfObsoletedItem(phasorRadarShotQuantityToBuy,phasorRadarShotValueToBuy);
			CancelPurchaseOfObsoletedItem(phasorRadarArrayQuantityToBuy,phasorRadarArrayValueToBuy);

		}

		//check if the unit has an Xray kernel
		if (combatUnit.GetComponent<PhasorSection> ().xRayKernalUpgrade == true) {

			//need to block buying x-ray kernel
			CancelPurchaseOfObsoletedItem(xRayKernelQuantityToBuy,xRayKernelValueToBuy);

		}

		//check if the unit has a tractor beam
		if (combatUnit.GetComponent<PhasorSection> ().tractorBeam == true) {

			//need to block buying x-ray kernel
			CancelPurchaseOfObsoletedItem(tractorBeamQuantityToBuy,tractorBeamValueToBuy);

		}

	}

	//this function prevents purchasing obsolete torpedo items based on current inventory
	private void BlockPurchaseOfObsoleteTorpedoItems(CombatUnit combatUnit){

		//check if the unit has a torpedo laser guidance system
		if (combatUnit.GetComponent<TorpedoSection> ().torpedoLaserGuidanceSystem == true) {

			//need to block buying torpedo laser shot and guidance system
			CancelPurchaseOfObsoletedItem(torpedoLaserShotQuantityToBuy,torpedoLaserShotValueToBuy);
			CancelPurchaseOfObsoletedItem(torpedoLaserGuidanceQuantityToBuy,torpedoLaserGuidanceValueToBuy);

		}

		//check if the unit has high pressure tubes
		if (combatUnit.GetComponent<TorpedoSection> ().highPressureTubes == true) {

			//need to block buying high pressure tubes
			CancelPurchaseOfObsoletedItem(highPressureTubesQuantityToBuy,highPressureTubesValueToBuy);

		}

	}

	//this function prevents purchasing obsolete storage items based on current inventory
	private void BlockPurchaseOfObsoleteStorageItems(CombatUnit combatUnit){
		
		//check if the unit has radar jamming
		if (combatUnit.GetComponent<StorageSection> ().radarJammingSystem == true) {

			//need to block buying another radar jamming system
			CancelPurchaseOfObsoletedItem(radarJammingQuantityToBuy,radarJammingValueToBuy);

		}

		//check if the unit has a torpedo laser scattering system
		if (combatUnit.GetComponent<StorageSection> ().laserScatteringSystem == true) {

			//need to block buying a laser scattering system
			CancelPurchaseOfObsoletedItem(laserScatteringQuantityToBuy,laserScatteringValueToBuy);

		}

	}

	//this function prevents purchasing obsolete crew items based on current inventory
	private void BlockPurchaseOfObsoleteCrewItems(CombatUnit combatUnit){

		//check if the unit has a repair crew
		if (combatUnit.GetComponent<CrewSection> ().repairCrew == true) {

			//need to block buying another repair crew
			CancelPurchaseOfObsoletedItem(repairCrewQuantityToBuy,repairCrewValueToBuy);

		}

		//check if the unit has a shield engineering team
		if (combatUnit.GetComponent<CrewSection> ().shieldEngineeringTeam == true) {

			//need to block buying a shield engineering team
			CancelPurchaseOfObsoletedItem(shieldEngineeringTeamQuantityToBuy,shieldEngineeringTeamValueToBuy);

		}

		//check if the unit has a battle crew
		if (combatUnit.GetComponent<CrewSection> ().battleCrew == true) {

			//need to block buying a battle crew
			CancelPurchaseOfObsoletedItem(battleCrewQuantityToBuy,battleCrewValueToBuy);

		}

	}

	//this function prevents purchasing obsolete engine items based on current inventory
	private void BlockPurchaseOfObsoleteEngineItems(CombatUnit combatUnit){

		//check if the unit has a warp drive
		if (combatUnit.GetComponent<EngineSection> ().warpDrive == true) {

			//need to block buying warp booster and warp drive
			CancelPurchaseOfObsoletedItem(warpBoosterQuantityToBuy,warpBoosterValueToBuy);
			CancelPurchaseOfObsoletedItem(warpDriveQuantityToBuy,warpDriveValueToBuy);

		}

		//check if the unit has a transwarp drive
		if (combatUnit.GetComponent<EngineSection> ().transwarpDrive == true) {

			//need to block buying warp booster, transwarp booster, and warp drive
			CancelPurchaseOfObsoletedItem(warpBoosterQuantityToBuy,warpBoosterValueToBuy);
			CancelPurchaseOfObsoletedItem(warpDriveQuantityToBuy,warpDriveValueToBuy);
			CancelPurchaseOfObsoletedItem(warpDriveQuantityToBuy,warpDriveValueToBuy);

		}

	}

	//this function will clear the current cart for purchased items
	private void ClearPurchasedItems(Toggle quantityToBuyToggle, Toggle valueToBuyToggle){

		//set the values to zero
		quantityToBuyToggle.GetComponentInChildren<TextMeshProUGUI>().text = ("0");
		valueToBuyToggle.GetComponentInChildren<TextMeshProUGUI>().text = ("0");

	}

	//this function calculates the total value of all items
	private void CalculateTotalItemCost(){

		int totalItemCost = 0;

		totalItemCost = Int32.Parse (phasorRadarShotValueToBuy.GetComponentInChildren<TextMeshProUGUI> ().text) +
			Int32.Parse (phasorRadarArrayValueToBuy.GetComponentInChildren<TextMeshProUGUI> ().text) +
			Int32.Parse (xRayKernelValueToBuy.GetComponentInChildren<TextMeshProUGUI> ().text) +
			Int32.Parse (tractorBeamValueToBuy.GetComponentInChildren<TextMeshProUGUI> ().text) +
			Int32.Parse (lightTorpedoValueToBuy.GetComponentInChildren<TextMeshProUGUI> ().text) +
			Int32.Parse (heavyTorpedoValueToBuy.GetComponentInChildren<TextMeshProUGUI> ().text) +
			Int32.Parse (torpedoLaserShotValueToBuy.GetComponentInChildren<TextMeshProUGUI> ().text) +
			Int32.Parse (torpedoLaserGuidanceValueToBuy.GetComponentInChildren<TextMeshProUGUI> ().text) +
			Int32.Parse (highPressureTubesValueToBuy.GetComponentInChildren<TextMeshProUGUI> ().text) +
			Int32.Parse (dilithiumCrystalValueToBuy.GetComponentInChildren<TextMeshProUGUI> ().text) +
			Int32.Parse (trilithiumCrystalValueToBuy.GetComponentInChildren<TextMeshProUGUI> ().text) +
			Int32.Parse (flareValueToBuy.GetComponentInChildren<TextMeshProUGUI> ().text) +
			Int32.Parse (radarJammingValueToBuy.GetComponentInChildren<TextMeshProUGUI> ().text) +
			Int32.Parse (laserScatteringValueToBuy.GetComponentInChildren<TextMeshProUGUI> ().text) +
			Int32.Parse (repairCrewValueToBuy.GetComponentInChildren<TextMeshProUGUI> ().text) +
			Int32.Parse (shieldEngineeringTeamValueToBuy.GetComponentInChildren<TextMeshProUGUI> ().text) +
			Int32.Parse (battleCrewValueToBuy.GetComponentInChildren<TextMeshProUGUI> ().text) +
			Int32.Parse (warpBoosterValueToBuy.GetComponentInChildren<TextMeshProUGUI> ().text) +
			Int32.Parse (transwarpBoosterValueToBuy.GetComponentInChildren<TextMeshProUGUI> ().text) +
			Int32.Parse (warpDriveValueToBuy.GetComponentInChildren<TextMeshProUGUI> ().text) +
			Int32.Parse (transwarpDriveValueToBuy.GetComponentInChildren<TextMeshProUGUI> ().text);

		//output the cost to the total toggles
		itemCost.GetComponentInChildren<TextMeshProUGUI> ().text = (totalItemCost.ToString ());

		//total = ship + item
		totalCost.GetComponentInChildren<TextMeshProUGUI> ().text = ((Int32.Parse (shipCost.GetComponentInChildren<TextMeshProUGUI> ().text) + 
			Int32.Parse (itemCost.GetComponentInChildren<TextMeshProUGUI> ().text)).ToString ());

	}

	//this function handles the purchased item bundle
	private void ResolvePurchase(CombatUnit combatUnit){


		//create a dictionary to store the purchased items
		Dictionary<string,int> purchasedItems = new Dictionary<string,int> ();

		//check if there are any purchased items
		if (Int32.Parse (phasorRadarShotQuantityToBuy.GetComponentInChildren<TextMeshProUGUI> ().text) > 0) {

			purchasedItems.Add ("Phasor Radar Shot", Int32.Parse (phasorRadarShotQuantityToBuy.GetComponentInChildren<TextMeshProUGUI> ().text));

		}

		if (Int32.Parse (phasorRadarArrayQuantityToBuy.GetComponentInChildren<TextMeshProUGUI> ().text) > 0) {

			purchasedItems.Add ("Phasor Radar Array", Int32.Parse (phasorRadarArrayQuantityToBuy.GetComponentInChildren<TextMeshProUGUI> ().text));

		}

		if (Int32.Parse (xRayKernelQuantityToBuy.GetComponentInChildren<TextMeshProUGUI> ().text) > 0) {

			purchasedItems.Add ("X-Ray Kernel Upgrade", Int32.Parse (xRayKernelQuantityToBuy.GetComponentInChildren<TextMeshProUGUI> ().text));

		}

		if (Int32.Parse (tractorBeamQuantityToBuy.GetComponentInChildren<TextMeshProUGUI> ().text) > 0) {

			purchasedItems.Add ("Tractor Beam", Int32.Parse (tractorBeamQuantityToBuy.GetComponentInChildren<TextMeshProUGUI> ().text));

		}

		if (Int32.Parse (lightTorpedoQuantityToBuy.GetComponentInChildren<TextMeshProUGUI> ().text) > 0) {

			purchasedItems.Add ("Light Torpedo", Int32.Parse (lightTorpedoQuantityToBuy.GetComponentInChildren<TextMeshProUGUI> ().text));

		}

		if (Int32.Parse (heavyTorpedoQuantityToBuy.GetComponentInChildren<TextMeshProUGUI> ().text) > 0) {

			purchasedItems.Add ("Heavy Torpedo", Int32.Parse (heavyTorpedoQuantityToBuy.GetComponentInChildren<TextMeshProUGUI> ().text));

		}

		if (Int32.Parse (torpedoLaserShotQuantityToBuy.GetComponentInChildren<TextMeshProUGUI> ().text) > 0) {

			purchasedItems.Add ("Torpedo Laser Shot", Int32.Parse (torpedoLaserShotQuantityToBuy.GetComponentInChildren<TextMeshProUGUI> ().text));

		}

		if (Int32.Parse (torpedoLaserGuidanceQuantityToBuy.GetComponentInChildren<TextMeshProUGUI> ().text) > 0) {

			purchasedItems.Add ("Torpedo Laser Guidance System", Int32.Parse (torpedoLaserGuidanceQuantityToBuy.GetComponentInChildren<TextMeshProUGUI> ().text));

		}

		if (Int32.Parse (highPressureTubesQuantityToBuy.GetComponentInChildren<TextMeshProUGUI> ().text) > 0) {

			purchasedItems.Add ("High Pressure Tubes", Int32.Parse (highPressureTubesQuantityToBuy.GetComponentInChildren<TextMeshProUGUI> ().text));

		}

		if (Int32.Parse (dilithiumCrystalQuantityToBuy.GetComponentInChildren<TextMeshProUGUI> ().text) > 0) {

			purchasedItems.Add ("Dilithium Crystal", Int32.Parse (dilithiumCrystalQuantityToBuy.GetComponentInChildren<TextMeshProUGUI> ().text));

		}

		if (Int32.Parse (trilithiumCrystalQuantityToBuy.GetComponentInChildren<TextMeshProUGUI> ().text) > 0) {

			purchasedItems.Add ("Trilithium Crystal", Int32.Parse (trilithiumCrystalQuantityToBuy.GetComponentInChildren<TextMeshProUGUI> ().text));

		}

		if (Int32.Parse (flareQuantityToBuy.GetComponentInChildren<TextMeshProUGUI> ().text) > 0) {

			purchasedItems.Add ("Flare", Int32.Parse (flareQuantityToBuy.GetComponentInChildren<TextMeshProUGUI> ().text));

		}

		if (Int32.Parse (radarJammingQuantityToBuy.GetComponentInChildren<TextMeshProUGUI> ().text) > 0) {

			purchasedItems.Add ("Radar Jamming System", Int32.Parse (radarJammingQuantityToBuy.GetComponentInChildren<TextMeshProUGUI> ().text));

		}

		if (Int32.Parse (laserScatteringQuantityToBuy.GetComponentInChildren<TextMeshProUGUI> ().text) > 0) {

			purchasedItems.Add ("Laser Scattering System", Int32.Parse (laserScatteringQuantityToBuy.GetComponentInChildren<TextMeshProUGUI> ().text));

		}

		if (Int32.Parse (repairCrewQuantityToBuy.GetComponentInChildren<TextMeshProUGUI> ().text) > 0) {

			purchasedItems.Add ("Repair Crew", Int32.Parse (repairCrewQuantityToBuy.GetComponentInChildren<TextMeshProUGUI> ().text));

		}

		if (Int32.Parse (shieldEngineeringTeamQuantityToBuy.GetComponentInChildren<TextMeshProUGUI> ().text) > 0) {

			purchasedItems.Add ("Shield Engineering Team", Int32.Parse (shieldEngineeringTeamQuantityToBuy.GetComponentInChildren<TextMeshProUGUI> ().text));

		}

		if (Int32.Parse (battleCrewQuantityToBuy.GetComponentInChildren<TextMeshProUGUI> ().text) > 0) {

			purchasedItems.Add ("Additional Battle Crew", Int32.Parse (battleCrewQuantityToBuy.GetComponentInChildren<TextMeshProUGUI> ().text));

		}

		if (Int32.Parse (warpBoosterQuantityToBuy.GetComponentInChildren<TextMeshProUGUI> ().text) > 0) {

			purchasedItems.Add ("Warp Booster", Int32.Parse (warpBoosterQuantityToBuy.GetComponentInChildren<TextMeshProUGUI> ().text));

		}

		if (Int32.Parse (transwarpBoosterQuantityToBuy.GetComponentInChildren<TextMeshProUGUI> ().text) > 0) {

			purchasedItems.Add ("Transwarp Booster", Int32.Parse (transwarpBoosterQuantityToBuy.GetComponentInChildren<TextMeshProUGUI> ().text));

		}

		if (Int32.Parse (warpDriveQuantityToBuy.GetComponentInChildren<TextMeshProUGUI> ().text) > 0) {

			purchasedItems.Add ("Warp Drive", Int32.Parse (warpDriveQuantityToBuy.GetComponentInChildren<TextMeshProUGUI> ().text));

		}

		if (Int32.Parse (transwarpDriveQuantityToBuy.GetComponentInChildren<TextMeshProUGUI> ().text) > 0) {

			purchasedItems.Add ("Transwarp Drive", Int32.Parse (transwarpDriveQuantityToBuy.GetComponentInChildren<TextMeshProUGUI> ().text));

		}


		//check the flag
		if (outfittingShip == false) {

			//invoke the purchase item event
			OnPurchaseItems.Invoke (purchasedItems, Int32.Parse (totalCost.GetComponentInChildren<TextMeshProUGUI> ().text), combatUnit);

		} else {

			//the else is if we are outfitting a ship
			//do a switch case with what type of ship is being bought
			switch (selectedShipType) {

			case "Scout":

				OnOutfittedShip.Invoke (purchasedItems, Int32.Parse (totalCost.GetComponentInChildren<TextMeshProUGUI> ().text), CombatUnit.UnitType.Scout);
				break;

			case "Bird Of Prey":

				OnOutfittedShip.Invoke (purchasedItems, Int32.Parse (totalCost.GetComponentInChildren<TextMeshProUGUI> ().text), CombatUnit.UnitType.BirdOfPrey);
				break;

			case "Destroyer":

				OnOutfittedShip.Invoke (purchasedItems, Int32.Parse (totalCost.GetComponentInChildren<TextMeshProUGUI> ().text), CombatUnit.UnitType.Destroyer);
				break;

			case "Starship":

				OnOutfittedShip.Invoke (purchasedItems, Int32.Parse (totalCost.GetComponentInChildren<TextMeshProUGUI> ().text), CombatUnit.UnitType.Starship);
				break;

			default:

				break;

			}


		}

		//close the panel
		ClosePurchaseItemsPanel();

	}

	//this function sets the purchase button status depending on whether the player can afford the items or not
	public void SetPurchaseButtonStatus(){

		//check if there are any items in the cart to buy
		if (Int32.Parse (totalCost.GetComponentInChildren<TextMeshProUGUI> ().text) > 0) {

			//check if the current price exceeds the player budget
			if (Int32.Parse (totalCost.GetComponentInChildren<TextMeshProUGUI> ().text) > gameManager.currentTurnPlayer.playerMoney) {

				//make the purchase button not interactable
				purchaseItemsButton.interactable = false;

				//change the text
				purchaseItemsButton.GetComponentInChildren<TextMeshProUGUI> ().text = ("Insufficient Funds");

			} else {

				//the else condition is that there are sufficient funds
				//make the purchase button interactable
				purchaseItemsButton.interactable = true;

				//change the text
				purchaseItemsButton.GetComponentInChildren<TextMeshProUGUI> ().text = ("Purchase");


			}

		} else {

			//make the button not interactable if there are no items
			purchaseItemsButton.interactable = false;

		}
			
	}

	//this function opens the purchase ships panel
	private void OpenPurchaseShipPanel(){

		//enable the status panel
		purchaseShipPanel.SetActive (true);

		//set the border color
		SetPurchaseShipBorderColor(gameManager.currentTurnPlayer.color);

		//set the purchase info
		SetShipPurchaseInfo();

		//set the ship icons
		SetPurchaseShipIcons();

		//set the initial toggle value for ship cost
		SetShipCostToggleValue(0);

		//set the purchase ship button initially to not interactable
		purchaseShipButton.interactable = false;

		//zero the item cost
		SetPurchaseShipCosts();

		//set the ship select buttons based on money
		SetShipSelectButtons();

	}

	//this function cancels the purchase ships panel
	private void CancelPurchaseShipPanel(){

		//enable the status panel
		purchaseShipPanel.SetActive (false);

	}

	//this function sets the ship class purchase information
	private void SetShipPurchaseInfo(){

		//set quantity of ships purchased
		scoutQtyAlreadyPurchasedToggle.GetComponentInChildren<TextMeshProUGUI>().text = (gameManager.currentTurnPlayer.playerScoutPurchased.ToString());
		birdOfPreyQtyAlreadyPurchasedToggle.GetComponentInChildren<TextMeshProUGUI>().text = (gameManager.currentTurnPlayer.playerBirdOfPreyPurchased.ToString());
		destroyerQtyAlreadyPurchasedToggle.GetComponentInChildren<TextMeshProUGUI>().text = (gameManager.currentTurnPlayer.playerDestroyerPurchased.ToString());
		starshipQtyAlreadyPurchasedToggle.GetComponentInChildren<TextMeshProUGUI>().text = (gameManager.currentTurnPlayer.playerStarshipPurchased.ToString());

		//set the max quantities allowed
		scoutMaxQtyToggle.GetComponentInChildren<TextMeshProUGUI>().text = (GameManager.maxShipsPerClass.ToString());
		birdOfPreyMaxQtyToggle.GetComponentInChildren<TextMeshProUGUI>().text = (GameManager.maxShipsPerClass.ToString());
		destroyerMaxQtyToggle.GetComponentInChildren<TextMeshProUGUI>().text = (GameManager.maxShipsPerClass.ToString());
		starshipMaxQtyToggle.GetComponentInChildren<TextMeshProUGUI>().text = (GameManager.maxShipsPerClass.ToString());

		//set the ship prices
		scoutCostText.text = (costScout.ToString());
		birdOfPreyCostText.text = (costBirdOfPrey.ToString ());
		destroyerCostText.text = (costDestroyer.ToString ());
		starshipCostText.text = (costStarship.ToString ());

	}

	//this function sets the ship picture icons in the purchase ship panel
	private void SetPurchaseShipIcons(){

		//store the player color as a string
		string playerColorString = gameManager.currentTurnPlayer.color.ToString();

		//invoke events to set the graphics
		OnSetPurchaseShipIcon.Invoke((GraphicsManager.MyUnit)Enum.Parse(typeof(GraphicsManager.MyUnit),playerColorString+"Scout",true),scoutRawImage);
		OnSetPurchaseShipIcon.Invoke((GraphicsManager.MyUnit)Enum.Parse(typeof(GraphicsManager.MyUnit),playerColorString+"Bird",true),birdOfPreyRawImage);
		OnSetPurchaseShipIcon.Invoke((GraphicsManager.MyUnit)Enum.Parse(typeof(GraphicsManager.MyUnit),playerColorString+"Destroyer",true),destroyerRawImage);
		OnSetPurchaseShipIcon.Invoke((GraphicsManager.MyUnit)Enum.Parse(typeof(GraphicsManager.MyUnit),playerColorString+"Starship",true),starshipRawImage);

	}

	//this function sets the item cost on the purchase ship panel
	private void SetPurchaseShipCosts(){

		//this is always zero
		itemCostToggle.GetComponentInChildren<TextMeshProUGUI>().text = ("0");

		//the total cost is ship + item
		totalCostToggle.GetComponentInChildren<TextMeshProUGUI> ().text = ((Int32.Parse (shipCostToggle.GetComponentInChildren<TextMeshProUGUI> ().text) +
			Int32.Parse (itemCostToggle.GetComponentInChildren<TextMeshProUGUI> ().text)).ToString ());

	}

	//this function checks which sections a ship type has and only displays the appropriate rows
	private void SetAvailableSectionData(){

		//check if a scout has a phasor section
		if (gameManager.prefabScout.GetComponent<PhasorSection> () == true) {

			//set to active
			scoutPhasorShieldRow.gameObject.SetActive (true);
			scoutPhasorStartingItemsRow.gameObject.SetActive (true);

			//initialize shield info
			scoutPhasorShieldRow.GetChild (1).GetComponent<TextMeshProUGUI> ().text = (Scout.phasorSectionShieldsMax.ToString ());

			//set initial equipment string
			scoutPhasorStartingItemsRow.GetComponentInChildren<TextMeshProUGUI>().text = GetInitialEquipment(CombatUnit.UnitType.Scout, "Phasor");

		} else {

			//else set to inactive
			scoutPhasorShieldRow.gameObject.SetActive (false);
			scoutPhasorStartingItemsRow.gameObject.SetActive (false);

		}

		//check if a scout has a torpedo section
		if (gameManager.prefabScout.GetComponent<TorpedoSection> () == true) {

			//set to active
			scoutTorpedoShieldRow.gameObject.SetActive (true);
			scoutTorpedoStartingItemsRow.gameObject.SetActive (true);

			//initialize shield info
			//scoutTorpedoShieldRow.GetChild (1).GetComponent<TextMeshProUGUI> ().text = (Scout.torpedoSectionShieldsMax.ToString ());

		} else {

			//else set to inactive
			scoutTorpedoShieldRow.gameObject.SetActive (false);
			scoutTorpedoStartingItemsRow.gameObject.SetActive (false);

		}

		//check if a scout has a storage section
		if (gameManager.prefabScout.GetComponent<StorageSection> () == true) {

			//set to active
			scoutStorageShieldRow.gameObject.SetActive (true);
			scoutStorageStartingItemsRow.gameObject.SetActive (true);

			//initialize shield info
			scoutStorageShieldRow.GetChild (1).GetComponent<TextMeshProUGUI> ().text = (Scout.storageSectionShieldsMax.ToString ());

			//set initial equipment string
			scoutStorageStartingItemsRow.GetComponentInChildren<TextMeshProUGUI>().text = (GetInitialEquipment(CombatUnit.UnitType.Scout, "Storage"));

		} else {

			//else set to inactive
			scoutStorageShieldRow.gameObject.SetActive (false);
			scoutStorageStartingItemsRow.gameObject.SetActive (false);

		}

		//check if a scout has a crew section
		if (gameManager.prefabScout.GetComponent<CrewSection> () == true) {

			//set to active
			scoutCrewShieldRow.gameObject.SetActive (true);
			scoutCrewStartingItemsRow.gameObject.SetActive (true);

			//initialize shield info
			//scoutCrewShieldRow.GetChild (1).GetComponent<TextMeshProUGUI> ().text = (Scout.crewSectionShieldsMax.ToString ());

		} else {

			//else set to inactive
			scoutCrewShieldRow.gameObject.SetActive (false);
			scoutCrewStartingItemsRow.gameObject.SetActive (false);

		}

		//check if a scout has an engine section
		if (gameManager.prefabScout.GetComponent<EngineSection> () == true) {

			//set to active
			scoutEngineShieldRow.gameObject.SetActive (true);
			scoutEngineStartingItemsRow.gameObject.SetActive (true);

			//initialize shield info
			scoutEngineShieldRow.GetChild (1).GetComponent<TextMeshProUGUI> ().text = (Scout.engineSectionShieldsMax.ToString ());

			//set initial equipment string
			scoutEngineStartingItemsRow.GetComponentInChildren<TextMeshProUGUI>().text = (GetInitialEquipment(CombatUnit.UnitType.Scout, "Engine"));

		} else {

			//else set to inactive
			scoutEngineShieldRow.gameObject.SetActive (false);
			scoutEngineStartingItemsRow.gameObject.SetActive (false);

		}


		//check if a bird has a phasor section
		if (gameManager.prefabBirdOfPrey.GetComponent<PhasorSection> () == true) {

			//set to active
			birdOfPreyPhasorShieldRow.gameObject.SetActive (true);
			birdOfPreyPhasorStartingItemsRow.gameObject.SetActive (true);

			//initialize shield info
			birdOfPreyPhasorShieldRow.GetChild (1).GetComponent<TextMeshProUGUI> ().text = (BirdOfPrey.phasorSectionShieldsMax.ToString ());

			//set initial equipment string
			birdOfPreyPhasorStartingItemsRow.GetComponentInChildren<TextMeshProUGUI>().text = GetInitialEquipment(CombatUnit.UnitType.BirdOfPrey, "Phasor");

		} else {

			//else set to inactive
			birdOfPreyPhasorShieldRow.gameObject.SetActive (false);
			birdOfPreyPhasorStartingItemsRow.gameObject.SetActive (false);

		}

		//check if a bird has a torpedo section
		if (gameManager.prefabBirdOfPrey.GetComponent<TorpedoSection> () == true) {

			//set to active
			birdOfPreyTorpedoShieldRow.gameObject.SetActive (true);
			birdOfPreyTorpedoStartingItemsRow.gameObject.SetActive (true);

			//initialize shield info
			birdOfPreyTorpedoShieldRow.GetChild (1).GetComponent<TextMeshProUGUI> ().text = (BirdOfPrey.torpedoSectionShieldsMax.ToString ());

			//set initial equipment string
			birdOfPreyTorpedoStartingItemsRow.GetComponentInChildren<TextMeshProUGUI>().text = (GetInitialEquipment(CombatUnit.UnitType.BirdOfPrey, "Torpedo"));

		} else {

			//else set to inactive
			birdOfPreyTorpedoShieldRow.gameObject.SetActive (false);
			birdOfPreyTorpedoStartingItemsRow.gameObject.SetActive (false);

		}

		//check if a bird has a storage section
		if (gameManager.prefabBirdOfPrey.GetComponent<StorageSection> () == true) {

			//set to active
			birdOfPreyStorageShieldRow.gameObject.SetActive (true);
			birdOfPreyStorageStartingItemsRow.gameObject.SetActive (true);

			//initialize shield info
			//birdOfPreyStorageShieldRow.GetChild (1).GetComponent<TextMeshProUGUI> ().text = (BirdOfPrey.storageSectionShieldsMax.ToString ());


		} else {

			//else set to inactive
			birdOfPreyStorageShieldRow.gameObject.SetActive (false);
			birdOfPreyStorageStartingItemsRow.gameObject.SetActive (false);

		}

		//check if a bird has a crew section
		if (gameManager.prefabBirdOfPrey.GetComponent<CrewSection> () == true) {

			//set to active
			birdOfPreyCrewShieldRow.gameObject.SetActive (true);
			birdOfPreyCrewStartingItemsRow.gameObject.SetActive (true);

			//initialize shield info
			//birdOfPreyCrewShieldRow.GetChild (1).GetComponent<TextMeshProUGUI> ().text = (BirdOfPrey.crewSectionShieldsMax.ToString ());

		} else {

			//else set to inactive
			birdOfPreyCrewShieldRow.gameObject.SetActive (false);
			birdOfPreyCrewStartingItemsRow.gameObject.SetActive (false);

		}

		//check if a bird has an engine section
		if (gameManager.prefabBirdOfPrey.GetComponent<EngineSection> () == true) {

			//set to active
			birdOfPreyEngineShieldRow.gameObject.SetActive (true);
			birdOfPreyEngineStartingItemsRow.gameObject.SetActive (true);

			//initialize shield info
			birdOfPreyEngineShieldRow.GetChild (1).GetComponent<TextMeshProUGUI> ().text = (BirdOfPrey.engineSectionShieldsMax.ToString ());

			//set initial equipment string
			birdOfPreyEngineStartingItemsRow.GetComponentInChildren<TextMeshProUGUI>().text = (GetInitialEquipment(CombatUnit.UnitType.BirdOfPrey, "Engine"));

		} else {

			//else set to inactive
			birdOfPreyEngineShieldRow.gameObject.SetActive (false);
			birdOfPreyEngineStartingItemsRow.gameObject.SetActive (false);

		}


		//check if a destroyer has a phasor section
		if (gameManager.prefabDestroyer.GetComponent<PhasorSection> () == true) {

			//set to active
			destroyerPhasorShieldRow.gameObject.SetActive (true);
			destroyerPhasorStartingItemsRow.gameObject.SetActive (true);

			//initialize shield info
			destroyerPhasorShieldRow.GetChild (1).GetComponent<TextMeshProUGUI> ().text = (Destroyer.phasorSectionShieldsMax.ToString ());

			//set initial equipment string
			destroyerPhasorStartingItemsRow.GetComponentInChildren<TextMeshProUGUI>().text = GetInitialEquipment(CombatUnit.UnitType.Destroyer, "Phasor");

		} else {

			//else set to inactive
			destroyerPhasorShieldRow.gameObject.SetActive (false);
			destroyerPhasorStartingItemsRow.gameObject.SetActive (false);

		}

		//check if a destroyer has a torpedo section
		if (gameManager.prefabDestroyer.GetComponent<TorpedoSection> () == true) {

			//set to active
			destroyerTorpedoShieldRow.gameObject.SetActive (true);
			destroyerTorpedoStartingItemsRow.gameObject.SetActive (true);

			//initialize shield info
			destroyerTorpedoShieldRow.GetChild (1).GetComponent<TextMeshProUGUI> ().text = (Destroyer.torpedoSectionShieldsMax.ToString ());

			//set initial equipment string
			destroyerTorpedoStartingItemsRow.GetComponentInChildren<TextMeshProUGUI>().text = (GetInitialEquipment(CombatUnit.UnitType.Destroyer, "Torpedo"));

		} else {

			//else set to inactive
			destroyerTorpedoShieldRow.gameObject.SetActive (false);
			destroyerTorpedoStartingItemsRow.gameObject.SetActive (false);

		}

		//check if a destroyer has a storage section
		if (gameManager.prefabDestroyer.GetComponent<StorageSection> () == true) {

			//set to active
			destroyerStorageShieldRow.gameObject.SetActive (true);
			destroyerStorageStartingItemsRow.gameObject.SetActive (true);

			//initialize shield info
			destroyerStorageShieldRow.GetChild (1).GetComponent<TextMeshProUGUI> ().text = (Destroyer.storageSectionShieldsMax.ToString ());

			//set initial equipment string
			destroyerStorageStartingItemsRow.GetComponentInChildren<TextMeshProUGUI>().text = (GetInitialEquipment(CombatUnit.UnitType.Destroyer, "Storage"));

		} else {

			//else set to inactive
			destroyerStorageShieldRow.gameObject.SetActive (false);
			destroyerStorageStartingItemsRow.gameObject.SetActive (false);

		}

		//check if a destroyer has a crew section
		if (gameManager.prefabDestroyer.GetComponent<CrewSection> () == true) {

			//set to active
			destroyerCrewShieldRow.gameObject.SetActive (true);
			destroyerCrewStartingItemsRow.gameObject.SetActive (true);

			//initialize shield info
			//destroyerCrewShieldRow.GetChild (1).GetComponent<TextMeshProUGUI> ().text = (Destroyer.crewSectionShieldsMax.ToString ());

		} else {

			//else set to inactive
			destroyerCrewShieldRow.gameObject.SetActive (false);
			destroyerCrewStartingItemsRow.gameObject.SetActive (false);

		}

		//check if a destroyer has an engine section
		if (gameManager.prefabDestroyer.GetComponent<EngineSection> () == true) {

			//set to active
			destroyerEngineShieldRow.gameObject.SetActive (true);
			destroyerEngineStartingItemsRow.gameObject.SetActive (true);

			//initialize shield info
			destroyerEngineShieldRow.GetChild (1).GetComponent<TextMeshProUGUI> ().text = (Destroyer.engineSectionShieldsMax.ToString ());

			//set initial equipment string
			destroyerEngineStartingItemsRow.GetComponentInChildren<TextMeshProUGUI>().text = (GetInitialEquipment(CombatUnit.UnitType.Destroyer, "Engine"));

		} else {

			//else set to inactive
			destroyerEngineShieldRow.gameObject.SetActive (false);
			destroyerEngineStartingItemsRow.gameObject.SetActive (false);

		}

		//check if a starship has a phasor section
		if (gameManager.prefabStarship.GetComponent<PhasorSection> () == true) {

			//set to active
			starshipPhasorShieldRow.gameObject.SetActive (true);
			starshipPhasorStartingItemsRow.gameObject.SetActive (true);

			//initialize shield info
			starshipPhasorShieldRow.GetChild (1).GetComponent<TextMeshProUGUI> ().text = (Starship.phasorSectionShieldsMax.ToString ());

			//set initial equipment string
			starshipPhasorStartingItemsRow.GetComponentInChildren<TextMeshProUGUI>().text = GetInitialEquipment(CombatUnit.UnitType.Starship, "Phasor");

		} else {

			//else set to inactive
			starshipPhasorShieldRow.gameObject.SetActive (false);
			starshipPhasorStartingItemsRow.gameObject.SetActive (false);

		}

		//check if a starship has a torpedo section
		if (gameManager.prefabStarship.GetComponent<TorpedoSection> () == true) {

			//set to active
			starshipTorpedoShieldRow.gameObject.SetActive (true);
			starshipTorpedoStartingItemsRow.gameObject.SetActive (true);

			//initialize shield info
			starshipTorpedoShieldRow.GetChild (1).GetComponent<TextMeshProUGUI> ().text = (Starship.torpedoSectionShieldsMax.ToString ());

			//set initial equipment string
			starshipTorpedoStartingItemsRow.GetComponentInChildren<TextMeshProUGUI>().text = (GetInitialEquipment(CombatUnit.UnitType.Starship, "Torpedo"));

		} else {

			//else set to inactive
			starshipTorpedoShieldRow.gameObject.SetActive (false);
			starshipTorpedoStartingItemsRow.gameObject.SetActive (false);

		}

		//check if a starship has a storage section
		if (gameManager.prefabStarship.GetComponent<StorageSection> () == true) {

			//set to active
			starshipStorageShieldRow.gameObject.SetActive (true);
			starshipStorageStartingItemsRow.gameObject.SetActive (true);

			//initialize shield info
			starshipStorageShieldRow.GetChild (1).GetComponent<TextMeshProUGUI> ().text = (Starship.storageSectionShieldsMax.ToString ());

			//set initial equipment string
			starshipStorageStartingItemsRow.GetComponentInChildren<TextMeshProUGUI>().text = (GetInitialEquipment(CombatUnit.UnitType.Starship, "Storage"));

		} else {

			//else set to inactive
			starshipStorageShieldRow.gameObject.SetActive (false);
			starshipStorageStartingItemsRow.gameObject.SetActive (false);

		}

		//check if a starship has a crew section
		if (gameManager.prefabStarship.GetComponent<CrewSection> () == true) {

			//set to active
			starshipCrewShieldRow.gameObject.SetActive (true);
			starshipCrewStartingItemsRow.gameObject.SetActive (true);

			//initialize shield info
			starshipCrewShieldRow.GetChild (1).GetComponent<TextMeshProUGUI> ().text = (Starship.crewSectionShieldsMax.ToString ());

			//set initial equipment string
			starshipCrewStartingItemsRow.GetComponentInChildren<TextMeshProUGUI>().text = (GetInitialEquipment(CombatUnit.UnitType.Starship, "Crew"));

		} else {

			//else set to inactive
			starshipCrewShieldRow.gameObject.SetActive (false);
			starshipCrewStartingItemsRow.gameObject.SetActive (false);

		}

		//check if a starship has an engine section
		if (gameManager.prefabStarship.GetComponent<EngineSection> () == true) {

			//set to active
			starshipEngineShieldRow.gameObject.SetActive (true);
			starshipEngineStartingItemsRow.gameObject.SetActive (true);

			//initialize shield info
			starshipEngineShieldRow.GetChild (1).GetComponent<TextMeshProUGUI> ().text = (Starship.engineSectionShieldsMax.ToString ());

			//set initial equipment string
			starshipEngineStartingItemsRow.GetComponentInChildren<TextMeshProUGUI>().text = (GetInitialEquipment(CombatUnit.UnitType.Starship, "Engine"));

		} else {

			//else set to inactive
			starshipEngineShieldRow.gameObject.SetActive (false);
			starshipEngineStartingItemsRow.gameObject.SetActive (false);

		}


	}

	//this function returns a string that is a list of starting equipment for a given ship type and section
	private string GetInitialEquipment(Ship.UnitType unitType, string section){

		string initialEquipment = null;

		//do a switch case on the unit type
		switch (unitType) {

		case CombatUnit.UnitType.Scout:

			//do a switch case on the section
			switch (section) {

			case "Phasor":

				//check if there are radar shots
				if (Scout.startingPhasorRadarShot > 0) {

					initialEquipment += Scout.startingPhasorRadarShot.ToString () + "X Phasor Radar Shot, ";

				}

				//check if there are radar array
				if (Scout.startingPhasorRadarArray == true) {

					initialEquipment += "Phasor Radar Array, ";

				}

				//check if there is x-ray kernel
				if (Scout.startingXRayKernelUpgrade == true) {

					initialEquipment += "X-Ray Kernel Upgrade, ";

				}

				//check if there is tractor beam
				if (Scout.startingTractorBeam == true) {

					initialEquipment += "Tractor Beam";

				}

				break;

			case "Storage":

				//check if there are dilithium crystals
				if (Scout.startingDilithiumCrystals > 0) {

					initialEquipment += Scout.startingDilithiumCrystals.ToString () + "X Dilithium Crystal, ";

				}

				//check if there are trilithium crystals
				if (Scout.startingTrilithiumCrystals > 0) {

					initialEquipment += Scout.startingTrilithiumCrystals.ToString () + "X Trilithium Crystal, ";

				}

				//check if there are flares
				if (Scout.startingFlares > 0) {

					initialEquipment += Scout.startingFlares.ToString () + "X Flare, ";

				}

				//check if there is radar jamming
				if (Scout.startingRadarJammingSystem == true) {

					initialEquipment += "Radar Jamming System, ";

				}

				//check if there is laser scattering
				if (Scout.startingLaserScatteringSystem == true) {

					initialEquipment += "Laser Scattering System";

				}

				break;

			case "Engine":

				//check if there are warp Boosters
				if (Scout.startingWarpBooster > 0) {

					initialEquipment += Scout.startingWarpBooster.ToString () + "X Warp Booster, ";

				}

				//check if there are transwarp Boosters
				if (Scout.startingTranswarpBooster > 0) {

					initialEquipment += Scout.startingTranswarpBooster.ToString () + "X Transwarp Booster, ";

				}

				//check if there is warp drive
				if (Scout.startingWarpDrive == true) {

					initialEquipment += "Warp Drive, ";

				}

				//check if there is transwarp drive
				if (Scout.startingTranswarpDrive == true) {

					initialEquipment += "Transwarp Drive";

				}

				break;

			default:

				break;

			}

			break;

		case CombatUnit.UnitType.BirdOfPrey:

			//do a switch case on the section
			switch (section) {

			case "Phasor":

				//check if there are radar shots
				if (BirdOfPrey.startingPhasorRadarShot > 0) {

					initialEquipment += BirdOfPrey.startingPhasorRadarShot.ToString () + "X Phasor Radar Shot, ";

				}

				//check if there are radar array
				if (BirdOfPrey.startingPhasorRadarArray == true) {

					initialEquipment += "Phasor Radar Array, ";

				}

				//check if there is x-ray kernel
				if (BirdOfPrey.startingXRayKernelUpgrade == true) {

					initialEquipment += "X-Ray Kernel Upgrade, ";

				}

				//check if there is tractor beam
				if (BirdOfPrey.startingTractorBeam == true) {

					initialEquipment += "Tractor Beam";

				}

				break;

			case "Torpedo":

				//check if there are light torpedos
				if (BirdOfPrey.startingLightTorpedos > 0) {

					initialEquipment += BirdOfPrey.startingLightTorpedos .ToString () + "X Light Torpedo, ";

				}

				//check if there are heavy torpedos
				if (BirdOfPrey.startingHeavyTorpedos > 0) {

					initialEquipment += BirdOfPrey.startingHeavyTorpedos.ToString () + "X Heavy Torpedo, ";

				}

				//check if there are laser shots
				if (BirdOfPrey.startingTorpedoLaserShot > 0) {

					initialEquipment += BirdOfPrey.startingTorpedoLaserShot.ToString () + "X Torpedo Laser Shot, ";

				}

				//check if there is laser guidance
				if (BirdOfPrey.startingTorpedoLaserGuidanceSystem == true) {

					initialEquipment += "Torpedo Laser Guidance System, ";

				}

				//check if there is high pressure tubes
				if (BirdOfPrey.startingHighPressureTubes == true) {

					initialEquipment += "High Pressure Tubes";

				}

				break;

			case "Engine":

				//check if there are warp Boosters
				if (BirdOfPrey.startingWarpBooster > 0) {

					initialEquipment += BirdOfPrey.startingWarpBooster.ToString () + "X Warp Booster, ";

				}

				//check if there are transwarp Boosters
				if (BirdOfPrey.startingTranswarpBooster > 0) {

					initialEquipment += BirdOfPrey.startingTranswarpBooster.ToString () + "X Transwarp Booster, ";

				}

				//check if there is warp drive
				if (BirdOfPrey.startingWarpDrive == true) {

					initialEquipment += "Warp Drive, ";

				}

				//check if there is transwarp drive
				if (BirdOfPrey.startingTranswarpDrive == true) {

					initialEquipment += "Transwarp Drive";

				}

				break;

			default:

				break;

			}

			break;

		case CombatUnit.UnitType.Destroyer:

			//do a switch case on the section
			switch (section) {

			case "Phasor":

				//check if there are radar shots
				if (Destroyer.startingPhasorRadarShot > 0) {

					initialEquipment += Destroyer.startingPhasorRadarShot.ToString () + "X Phasor Radar Shot, ";

				}

				//check if there are radar array
				if (Destroyer.startingPhasorRadarArray == true) {

					initialEquipment += "Phasor Radar Array, ";

				}

				//check if there is x-ray kernel
				if (Destroyer.startingXRayKernelUpgrade == true) {

					initialEquipment += "X-Ray Kernel Upgrade, ";

				}

				//check if there is tractor beam
				if (Destroyer.startingTractorBeam == true) {

					initialEquipment += "Tractor Beam";

				}

				break;

			case "Torpedo":

				//check if there are light torpedos
				if (Destroyer.startingLightTorpedos > 0) {

					initialEquipment += Destroyer.startingLightTorpedos .ToString () + "X Light Torpedo, ";

				}

				//check if there are heavy torpedos
				if (Destroyer.startingHeavyTorpedos > 0) {

					initialEquipment += Destroyer.startingHeavyTorpedos.ToString () + "X Heavy Torpedo, ";

				}

				//check if there are laser shots
				if (Destroyer.startingTorpedoLaserShot > 0) {

					initialEquipment += Destroyer.startingTorpedoLaserShot.ToString () + "X Torpedo Laser Shot, ";

				}

				//check if there is laser guidance
				if (Destroyer.startingTorpedoLaserGuidanceSystem == true) {

					initialEquipment += "Torpedo Laser Guidance System, ";

				}

				//check if there is high pressure tubes
				if (Destroyer.startingHighPressureTubes == true) {

					initialEquipment += "High Pressure Tubes";

				}

				break;

			case "Storage":

				//check if there are dilithium crystals
				if (Destroyer.startingDilithiumCrystals > 0) {

					initialEquipment += Destroyer.startingDilithiumCrystals.ToString () + "X Dilithium Crystal, ";

				}

				//check if there are trilithium crystals
				if (Destroyer.startingTrilithiumCrystals > 0) {

					initialEquipment += Destroyer.startingTrilithiumCrystals.ToString () + "X Trilithium Crystal, ";

				}

				//check if there are flares
				if (Destroyer.startingFlares > 0) {

					initialEquipment += Destroyer.startingFlares.ToString () + "X Flare, ";

				}

				//check if there is radar jamming
				if (Destroyer.startingRadarJammingSystem == true) {

					initialEquipment += "Radar Jamming System, ";

				}

				//check if there is laser scattering
				if (Destroyer.startingLaserScatteringSystem == true) {

					initialEquipment += "Laser Scattering System";

				}

				break;

			case "Engine":

				//check if there are warp Boosters
				if (Destroyer.startingWarpBooster > 0) {

					initialEquipment += Destroyer.startingWarpBooster.ToString () + "X Warp Booster, ";

				}

				//check if there are transwarp Boosters
				if (Destroyer.startingTranswarpBooster > 0) {

					initialEquipment += Destroyer.startingTranswarpBooster.ToString () + "X Transwarp Booster, ";

				}

				//check if there is warp drive
				if (Destroyer.startingWarpDrive == true) {

					initialEquipment += "Warp Drive, ";

				}

				//check if there is transwarp drive
				if (Destroyer.startingTranswarpDrive == true) {

					initialEquipment += "Transwarp Drive";

				}

				break;

			default:

				break;

			}

			break;

		case CombatUnit.UnitType.Starship:

			//do a switch case on the section
			switch (section) {

			case "Phasor":

				//check if there are radar shots
				if (Starship.startingPhasorRadarShot > 0) {

					initialEquipment += Starship.startingPhasorRadarShot.ToString () + "X Phasor Radar Shot, ";

				}

				//check if there are radar array
				if (Starship.startingPhasorRadarArray == true) {

					initialEquipment += "Phasor Radar Array, ";

				}

				//check if there is x-ray kernel
				if (Starship.startingXRayKernelUpgrade == true) {

					initialEquipment += "X-Ray Kernel Upgrade, ";

				}

				//check if there is tractor beam
				if (Starship.startingTractorBeam == true) {

					initialEquipment += "Tractor Beam";

				}

				break;

			case "Torpedo":

				//check if there are light torpedos
				if (Starship.startingLightTorpedos > 0) {

					initialEquipment += Starship.startingLightTorpedos .ToString () + "X Light Torpedo, ";

				}

				//check if there are heavy torpedos
				if (Starship.startingHeavyTorpedos > 0) {

					initialEquipment += Starship.startingHeavyTorpedos.ToString () + "X Heavy Torpedo, ";

				}

				//check if there are laser shots
				if (Starship.startingTorpedoLaserShot > 0) {

					initialEquipment += Starship.startingTorpedoLaserShot.ToString () + "X Torpedo Laser Shot, ";

				}

				//check if there is laser guidance
				if (Starship.startingTorpedoLaserGuidanceSystem == true) {

					initialEquipment += "Torpedo Laser Guidance System, ";

				}

				//check if there is high pressure tubes
				if (Starship.startingHighPressureTubes == true) {

					initialEquipment += "High Pressure Tubes";

				}

				break;

			case "Storage":

				//check if there are dilithium crystals
				if (Starship.startingDilithiumCrystals > 0) {

					initialEquipment += Starship.startingDilithiumCrystals.ToString () + "X Dilithium Crystal, ";

				}

				//check if there are trilithium crystals
				if (Starship.startingTrilithiumCrystals > 0) {

					initialEquipment += Starship.startingTrilithiumCrystals.ToString () + "X Trilithium Crystal, ";

				}

				//check if there are flares
				if (Starship.startingFlares > 0) {

					initialEquipment += Starship.startingFlares.ToString () + "X Flare, ";

				}

				//check if there is radar jamming
				if (Starship.startingRadarJammingSystem == true) {

					initialEquipment += "Radar Jamming System, ";

				}

				//check if there is laser scattering
				if (Starship.startingLaserScatteringSystem == true) {

					initialEquipment += "Laser Scattering System";

				}

				break;

			case "Crew":

				//check if there are repair crew
				if (Starship.startingRepairCrew == true ) {

					initialEquipment += "Repair Crew, ";

				}

				//check if there are shield engineering team
				if (Starship.startingShieldEngineeringTeam == true) {

					initialEquipment += "Shield Engineering Team, ";

				}

				//check if there is additional battle crew
				if (Starship.startingBattleCrew == true) {

					initialEquipment += "Additional Battle Crew";

				}

				break;

			case "Engine":

				//check if there are warp Boosters
				if (Starship.startingWarpBooster > 0) {

					initialEquipment += Starship.startingWarpBooster.ToString () + "X Warp Booster, ";

				}

				//check if there are transwarp Boosters
				if (Starship.startingTranswarpBooster > 0) {

					initialEquipment += Starship.startingTranswarpBooster.ToString () + "X Transwarp Booster, ";

				}

				//check if there is warp drive
				if (Starship.startingWarpDrive == true) {

					initialEquipment += "Warp Drive, ";

				}

				//check if there is transwarp drive
				if (Starship.startingTranswarpDrive == true) {

					initialEquipment += "Transwarp Drive";

				}

				break;

			default:

				break;

			}

			break;

		default:

			break;

		}

		//check if the string ends with a comma
		if (initialEquipment != null && initialEquipment.EndsWith (", ") == true) {

			//remove the comma
			initialEquipment = initialEquipment.Remove(initialEquipment.Length -2);

		}

		return initialEquipment;


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

	//this function sets the availability of the shipSelect buttons based on funds available
	private void SetShipSelectButtons(){

		//first, make sure all the buttons are unhighlighted
		UnhighlightButton(scoutSelectButton);
		UnhighlightButton(birdOfPreySelectButton);
		UnhighlightButton(destroyerSelectButton);
		UnhighlightButton(starshipSelectButton);


		//check if the player has enough money for a scout
		if (gameManager.currentTurnPlayer.playerMoney < costScout) {

			//set inactive
			scoutSelectButton.interactable = false;

		} else {

			//check if the player has not maxed out on purchases for the ship class
			if (gameManager.currentTurnPlayer.playerScoutPurchased >= GameManager.maxShipsPerClass) {

				//if the max is reached, the button should be inactive
				scoutSelectButton.interactable = false;

			} else {

				//set actove
				scoutSelectButton.interactable = true;

			}

		}

		//check if the player has enough money for a bird
		if (gameManager.currentTurnPlayer.playerMoney < costBirdOfPrey) {

			//set inactive
			birdOfPreySelectButton.interactable = false;

		} else {

			//check if the player has not maxed out on purchases for the ship class
			if (gameManager.currentTurnPlayer.playerBirdOfPreyPurchased >= GameManager.maxShipsPerClass) {

				//if the max is reached, the button should be inactive
				birdOfPreySelectButton.interactable = false;

			} else {

				//set actove
				birdOfPreySelectButton.interactable = true;

			}

		}

		//check if the player has enough money for a destroyer
		if (gameManager.currentTurnPlayer.playerMoney < costDestroyer) {

			//set inactive
			destroyerSelectButton.interactable = false;

		} else {

			//check if the player has not maxed out on purchases for the ship class
			if (gameManager.currentTurnPlayer.playerDestroyerPurchased >= GameManager.maxShipsPerClass) {

				//if the max is reached, the button should be inactive
				destroyerSelectButton.interactable = false;

			} else {

				//set actove
				destroyerSelectButton.interactable = true;

			}

		}

		//check if the player has enough money for a starship
		if (gameManager.currentTurnPlayer.playerMoney < costStarship) {

			//set inactive
			starshipSelectButton.interactable = false;

		} else {

			//check if the player has not maxed out on purchases for the ship class
			if (gameManager.currentTurnPlayer.playerStarshipPurchased >= GameManager.maxShipsPerClass) {

				//if the max is reached, the button should be inactive
				starshipSelectButton.interactable = false;

			} else {

				//set actove
				starshipSelectButton.interactable = true;

			}

		}


	}

	//this function sets the ship cost toggle value
	private void SetShipCostToggleValue(int shipCost){

		shipCostToggle.GetComponentInChildren<TextMeshProUGUI> ().text = (shipCost.ToString ());

		//update the other numbers
		SetPurchaseShipCosts();

	}

	//this function sets the interactability of the purchase ship main menu button
	private void SetPurchaseShipMenuButton(Player player){

		//first check the mode we're in
		if (UIManager.lockMenuActionModes.Contains(gameManager.CurrentActionMode)){

			openPurchaseShipButton.interactable = false;
		}

		//check if the player is the current turn player
		else if (player == gameManager.currentTurnPlayer) {

			//check if it is the purchase phase
			if (gameManager.currentTurnPhase == GameManager.TurnPhase.PurchasePhase) {

				//check if the home tile or adjacent tiles are available to place a ship
				//switch case based on player color
				switch (player.color) {

				case Player.Color.Green:

					foreach (Hex possibleHex in tileMap.GreenStartTiles) {

						//check if the start tile is empty
						if (tileMap.HexMap [possibleHex].tileCombatUnit == null) {

							//if it is, allow the purchase button to be interactable
							openPurchaseShipButton.interactable = true;

						} else if (tileMap.ReachableTiles (possibleHex, 1).Count > 0) {

							//check if at least one neighbor tile is open
							//if it is, allow the purchase button to be interactable
							openPurchaseShipButton.interactable = true;

						} else {

							//else the neighbors are not open either, so the button should be closed
							openPurchaseShipButton.interactable = false;

							//Debug.Log ("Nowhere to put ships");

						}

					}

					break;

				case Player.Color.Purple:

					foreach (Hex possibleHex in tileMap.PurpleStartTiles) {

						//check if the start tile is empty
						if (tileMap.HexMap [possibleHex].tileCombatUnit == null) {

							//if it is, allow the purchase button to be interactable
							openPurchaseShipButton.interactable = true;

						} else if (tileMap.ReachableTiles (possibleHex, 1).Count > 0) {

							//check if at least one neighbor tile is open
							//if it is, allow the purchase button to be interactable
							openPurchaseShipButton.interactable = true;

						} else {

							//else the neighbors are not open either, so the button should be closed
							openPurchaseShipButton.interactable = false;

							//Debug.Log ("Nowhere to put ships");

						}

					}

					break;

				case Player.Color.Red:

					foreach (Hex possibleHex in tileMap.RedStartTiles) {

						//check if the start tile is empty
						if (tileMap.HexMap [possibleHex].tileCombatUnit == null) {

							//if it is, allow the purchase button to be interactable
							openPurchaseShipButton.interactable = true;

						} else if (tileMap.ReachableTiles (possibleHex, 1).Count > 0) {

							//check if at least one neighbor tile is open
							//if it is, allow the purchase button to be interactable
							openPurchaseShipButton.interactable = true;

						} else {

							//else the neighbors are not open either, so the button should be closed
							openPurchaseShipButton.interactable = false;

							//Debug.Log ("Nowhere to put ships");

						}

					}

					break;

				case Player.Color.Blue:

					foreach (Hex possibleHex in tileMap.BlueStartTiles) {

						//check if the start tile is empty
						if (tileMap.HexMap [possibleHex].tileCombatUnit == null) {

							//if it is, allow the purchase button to be interactable
							openPurchaseShipButton.interactable = true;

						} else if (tileMap.ReachableTiles (possibleHex, 1).Count > 0) {

							//check if at least one neighbor tile is open
							//if it is, allow the purchase button to be interactable
							openPurchaseShipButton.interactable = true;

						} else {

							//else the neighbors are not open either, so the button should be closed
							openPurchaseShipButton.interactable = false;

							//Debug.Log ("Nowhere to put ships");

						}

					}

					break;

				default:

					openPurchaseShipButton.interactable = false;
					break;

				}

			} else {

				openPurchaseShipButton.interactable = false;

			} 

		} else {

			openPurchaseShipButton.interactable = false;

		}

	}

	//this function sets the border color
	private void SetPurchaseShipBorderColor(Player.Color playerColor){

		//switch case based on player
		switch (playerColor) {

		case Player.Color.Green:

			purchaseShipPanel.GetComponent<Image> ().color = GameManager.greenColor;
			break;

		case Player.Color.Purple:

			purchaseShipPanel.GetComponent<Image> ().color = GameManager.purpleColor;
			break;

		case Player.Color.Red:

			purchaseShipPanel.GetComponent<Image> ().color = GameManager.redColor;
			break;

		case Player.Color.Blue:

			purchaseShipPanel.GetComponent<Image> ().color = GameManager.blueColor;
			break;

		default:

			break;

		}

	}

	//this function sets the border color
	private void SetPurchaseItemBorderColor(Player.Color playerColor){

		//switch case based on player
		switch (playerColor) {

		case Player.Color.Green:

			purchaseItemsPanel.GetComponent<Image> ().color = GameManager.greenColor;
			break;

		case Player.Color.Purple:
			
			purchaseItemsPanel.GetComponent<Image> ().color = GameManager.purpleColor;
			break;

		case Player.Color.Red:
			
			purchaseItemsPanel.GetComponent<Image> ().color = GameManager.redColor;
			break;

		case Player.Color.Blue:
			
			purchaseItemsPanel.GetComponent<Image> ().color = GameManager.blueColor;
			break;

		default:

			break;

		}

	}

	//this function handles OnDestroy
	private void OnDestroy(){

		RemoveAllListeners ();

	}

	//this function removes all listeners
	private void RemoveAllListeners(){

		if (cancelPurchaseItemsButton != null) {
			
			//remove a listener to the cancel button on-click
			cancelPurchaseItemsButton.onClick.RemoveListener (ClosePurchaseItemsPanel);

		}

		if (openPurchaseItemsButton != null) {
			
			//remove a listener to the open button on-click
			openPurchaseItemsButton.onClick.RemoveListener (openPurchaseItemsPanelAction);

		}

		if (mouseManager != null) {
			
			//remove listeners to changes in the selected unit
			mouseManager.OnSetSelectedUnit.RemoveListener (setOpenButtonStatusAction);
			mouseManager.OnClearSelectedUnit.RemoveListener (setOpenButtonStatusAction);

		}

		if (gameManager != null) {
			
			//remove listener for a new turn starting
			gameManager.OnNewTurn.RemoveListener (playerSetOpenButtonStatusAction);

			//remove listener for a loaded turn starting
			gameManager.OnLoadedTurn.RemoveListener (playerSetOpenButtonStatusAction);

			//remove listener for mode change
			gameManager.OnActionModeChange.RemoveListener (actionModeSetOpenButtonStatusAction);

		}

		if (purchaseItemsButton != null) {
			
			//remove listener for the purchase button
			purchaseItemsButton.onClick.RemoveListener (resolvePurchaseAction);

		}

		//remove listeners for up button presses
		//phasor buttons

		if (phasorRadarShotUpButton != null) {
			
			phasorRadarShotUpButton.onClick.RemoveListener (resolvePhasorRadarShotUpAction);

		}

		if (phasorRadarArrayUpButton != null) {
			
			phasorRadarArrayUpButton.onClick.RemoveListener (resolvePhasorRadarArrayUpAction);

		}

		if (xRayKernelUpButton != null) {
			
			xRayKernelUpButton.onClick.RemoveListener (resolveXRayKernelUpAction);

		}

		if (tractorBeamUpButton != null) {
			
			tractorBeamUpButton.onClick.RemoveListener (resolveTractorBeamUpAction);

		}

		if (phasorRadarArrayUpButton != null) {
			
			phasorRadarArrayUpButton.onClick.RemoveListener (cancelObsoletePhasorRadarShotAction);

		}

		//torpedo buttons

		if (lightTorpedoUpButton != null) {
			
			lightTorpedoUpButton.onClick.RemoveListener (resolveLightTorpedoUpAction);

		}

		if (heavyTorpedoUpButton != null) {
			
			heavyTorpedoUpButton.onClick.RemoveListener (resolveHeavyTorpedoUpAction);

		}

		if (torpedoLaserShotUpButton != null) {
			
			torpedoLaserShotUpButton.onClick.RemoveListener (resolveTorpedoLaserShotUpAction);

		}

		if (torpedoLaserGuidanceUpButton != null) {
			
			torpedoLaserGuidanceUpButton.onClick.RemoveListener (resolveTorpedoLaserGuidanceUpAction);

		}

		if (highPressureTubesUpButton != null) {
			
			highPressureTubesUpButton.onClick.RemoveListener (resolveHighPressureTubesUpAction);

		}

		if (torpedoLaserGuidanceUpButton != null) {
			
			torpedoLaserGuidanceUpButton.onClick.RemoveListener (cancelObsoleteTorpedoLaserShotAction);

		}


		//storage buttons

		if (dilithiumCrystalUpButton != null) {
			
			dilithiumCrystalUpButton.onClick.RemoveListener (resolveDilithiumCrystalUpAction);

		}

		if (trilithiumCrystalUpButton != null) {
			
			trilithiumCrystalUpButton.onClick.RemoveListener (resolveTrilithiumCrystalUpAction);

		}

		if (flareUpButton != null) {
			
			flareUpButton.onClick.RemoveListener (resolveFlareUpAction);

		}

		if (radarJammingSystemUpButton != null) {
			
			radarJammingSystemUpButton.onClick.RemoveListener (resolveRadarJammingSystemUpAction);

		}

		if (laserScatteringSystemUpButton != null) {
			
			laserScatteringSystemUpButton.onClick.RemoveListener (resolveLaserScatteringSystemUpAction);

		}

		//crew buttons

		if (repairCrewUpButton != null) {
			
			repairCrewUpButton.onClick.RemoveListener (resolveRepairCrewUpAction);

		}

		if (shieldEngineeringTeamUpButton != null) {
			
			shieldEngineeringTeamUpButton.onClick.RemoveListener (resolveShieldEngineeringUpAction);

		}

		if (battleCrewUpButton != null) {
			
			battleCrewUpButton.onClick.RemoveListener (resolveBattleCrewUpAction);

		}

		//engine buttons

		if (warpBoosterUpButton != null) {
			
			warpBoosterUpButton.onClick.RemoveListener (resolveWarpBoosterUpAction);

		}

		if (transwarpBoosterUpButton != null) {
			
			transwarpBoosterUpButton.onClick.RemoveListener (resolveTranswarpBoosterUpAction);

		}

		if (warpDriveUpButton != null) {
			
			warpDriveUpButton.onClick.RemoveListener (resolveWarpDriveUpAction);

		}

		if (transwarpDriveUpButton != null) {
			
			transwarpDriveUpButton.onClick.RemoveListener (resolveTranswarpDriveUpAction);

		}

		if (warpDriveUpButton != null) {
			
			warpDriveUpButton.onClick.RemoveListener (cancelObsoleteWarpBoosterAction);

		}

		if (transwarpDriveUpButton != null) {
			
			transwarpDriveUpButton.onClick.RemoveListener (cancelObsoleteTranswarpBoosterAction);

		}

		//remove listeners for down button presses
		//phasor buttons

		if (phasorRadarShotDownButton != null) {
			
			phasorRadarShotDownButton.onClick.RemoveListener (resolvePhasorRadarShotDownAction);

		}

		if (phasorRadarArrayDownButton != null) {
			
			phasorRadarArrayDownButton.onClick.RemoveListener (resolvePhasorRadarArrayDownAction);

		}

		if (xRayKernelDownButton != null) {
			
			xRayKernelDownButton.onClick.RemoveListener (resolveXRayKernelDownAction);

		}

		if (tractorBeamDownButton != null) {
			
			tractorBeamDownButton.onClick.RemoveListener (resolveTractorBeamDownAction);

		}

		if (phasorRadarArrayDownButton != null) {
			
			phasorRadarArrayDownButton.onClick.RemoveListener (undoCancelObsoletePhasorRadarShotAction);

		}

		//torpedo buttons

		if (lightTorpedoDownButton != null) {
			
			lightTorpedoDownButton.onClick.RemoveListener (resolveLightTorpedoDownAction);

		}

		if (heavyTorpedoDownButton != null) {
			
			heavyTorpedoDownButton.onClick.RemoveListener (resolveHeavyTorpedoDownAction);

		}

		if (torpedoLaserShotDownButton != null) {
			
			torpedoLaserShotDownButton.onClick.RemoveListener (resolveTorpedoLaserShotDownAction);

		}

		if (torpedoLaserGuidanceDownButton != null) {
			
			torpedoLaserGuidanceDownButton.onClick.RemoveListener (resolveTorpedoLaserGuidanceDownAction);

		}

		if (highPressureTubesDownButton != null) {
			
			highPressureTubesDownButton.onClick.RemoveListener (resolveHighPressureTubesDownAction);

		}

		if (torpedoLaserGuidanceDownButton != null) {
			
			torpedoLaserGuidanceDownButton.onClick.RemoveListener (undoCancelObsoleteTorpedoLaserShotAction);

		}

		//storage buttons

		if (dilithiumCrystalDownButton != null) {
			
			dilithiumCrystalDownButton.onClick.RemoveListener (resolveDilithiumCrystalDownAction);

		}

		if (trilithiumCrystalDownButton != null) {
			
			trilithiumCrystalDownButton.onClick.RemoveListener (resolveTrilithiumCrystalDownAction);

		}

		if (flareDownButton != null) {
			
			flareDownButton.onClick.RemoveListener (resolveFlareDownAction);

		}

		if (radarJammingSystemDownButton != null) {
			
			radarJammingSystemDownButton.onClick.RemoveListener (resolveRadarJammingSystemDownAction);

		}

		if (laserScatteringSystemDownButton != null) {
			
			laserScatteringSystemDownButton.onClick.RemoveListener (resolveLaserScatteringSystemDownAction);

		}

		//crew buttons

		if (repairCrewDownButton != null) {
			
			repairCrewDownButton.onClick.RemoveListener (resolveRepairCrewDownAction);

		}

		if (shieldEngineeringTeamDownButton != null) {
			
			shieldEngineeringTeamDownButton.onClick.RemoveListener (resolveShieldEngineeringDownAction);

		}

		if (battleCrewDownButton != null) {
			
			battleCrewDownButton.onClick.RemoveListener (resolveBattleCrewDownAction);

		}

		//engine buttons

		if (warpBoosterDownButton != null) {
			
			warpBoosterDownButton.onClick.RemoveListener (resolveWarpBoosterDownAction);

		}

		if (transwarpBoosterDownButton != null) {
			
			transwarpBoosterDownButton.onClick.RemoveListener (resolveTranswarpBoosterDownAction);

		}

		if (warpDriveDownButton != null) {
			
			warpDriveDownButton.onClick.RemoveListener (resolveWarpDriveDownAction);

		}

		if (transwarpDriveDownButton != null) {
			
			transwarpDriveDownButton.onClick.RemoveListener (resolveTranswarpDriveDownAction);

		}

		if (warpDriveDownButton != null) {
			
			warpDriveDownButton.onClick.RemoveListener (undoCancelObsoleteWarpBoosterAction);

		}

		if (transwarpDriveDownButton != null) {
			
			transwarpDriveDownButton.onClick.RemoveListener (undoCancelObsoleteTranswarpBoosterAction);

		}

		if (clearItemCartButton != null) {
			
			//remove listeners for clear items button
			clearItemCartButton.onClick.RemoveListener (clearItemCartAction);

		}

		///////////////////////////
		/// This is the PurchaseShips section
		/////////////////////////// 

		if (openPurchaseShipButton != null) {
			
			//remove listener for opening the purchase ship panel
			openPurchaseShipButton.onClick.RemoveListener (OpenPurchaseShipPanel);

		}

		if (cancelPurchaseShipButton != null) {
			
			//remove listener for canceling the purchase ship panel
			cancelPurchaseShipButton.onClick.RemoveListener (CancelPurchaseShipPanel);

		}

		if (gameManager != null) {
			
			//remove a listener for starting a new turn
			gameManager.OnNewTurn.RemoveListener (playerSetPurchaseShipButtonAction);
			gameManager.OnBeginMainPhase.RemoveListener (playerSetPurchaseShipButtonAction);
			gameManager.OnNewUnitCreated.RemoveListener (playerSetPurchaseShipButtonAction);

			//remove listener for loading a turn
			gameManager.OnLoadedTurn.RemoveListener (playerSetPurchaseShipButtonAction);

			//remove listener for action mode change
			gameManager.OnActionModeChange.RemoveListener (actionModeSetPurchaseShipButtonAction);

		}

		if (purchaseShipButton != null) {

			//remove a listener to the outfit purchase button on-click
			purchaseShipButton.onClick.RemoveListener (purchaseShipButtonAction);

		}

		if (scoutSelectButton != null) {

			//remove listener for the scout selection button
			scoutSelectButton.onClick.RemoveListener (purchaseScoutButtonAction);

		}

		if (birdOfPreySelectButton != null) {
			
			//remove listener for the bid selection button
			birdOfPreySelectButton.onClick.RemoveListener (purchaseBirdOfPreyButtonAction);

		}

		if (destroyerSelectButton != null) {

			//remove listener for the destroyer selection button
			destroyerSelectButton.onClick.RemoveListener (purchaseDestroyerButtonAction);

		}

		if (starshipSelectButton != null) {
			
			//remove listener for the starship selection button
			starshipSelectButton.onClick.RemoveListener (purchaseStarshipButtonAction);

		}

	}

}
