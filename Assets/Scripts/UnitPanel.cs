using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class UnitPanel : MonoBehaviour {

	//managers
	private MouseManager mouseManager;
	private GameManager gameManager;
	private UIManager uiManager;

	//variable to hold the raw image that shows the highlighted unit
	public RawImage highlightedUnitImage;

	//variable to hold the highlighted unit
	private CombatUnit highlightedUnit;

	//variables to hold positions for text items
	private Vector3 shipPhaserTextLocation = new Vector3(-7,-40,0);
	private Vector3 shipTorpedoTextLocation = new Vector3(7,-40,0);

	private Vector3 basePhaserTextLocation = new Vector3(-13,-36,0);
	private Vector3 baseTorpedoTextLocation = new Vector3(13,-36,0);

	private Vector3 shipNameTextLocation = new Vector3(0,-23,0);
	private Vector3 baseNameTextLocation = new Vector3(0,0,0);

	private Vector3 starshipMoveTextLocation = new Vector3(-32,-8,0);
	private Vector3 destroyerMoveTextLocation = new Vector3(-32,10,0);
	private Vector3 birdOfPreyMoveTextLocation = new Vector3(-36,-8,0);
	private Vector3 scoutMoveTextLocation = new Vector3(-32,16,0);

	private Vector3 starshipRepairTextLocation = new Vector3(32,-8,0);
	private Vector3 starbaseRepairTextLocation = new Vector3(32,16,0);

	private Vector3 birdOfPreyCloakTextLocation = new Vector3(32,-8,0);

	//variable to hold the unit panel rect transform
	public RectTransform unitPanel;

	//variable to hold the minimap border rect transform
	public RectTransform miniMapBorder;

	//variables to hold text overlays on the highlighted unit
	public TextMeshProUGUI shipName;
	public TextMeshProUGUI torpedoText;
	public TextMeshProUGUI phaserText;
	public TextMeshProUGUI movementText;
	public TextMeshProUGUI specialText;
	public TextMeshProUGUI highlightedUnitPanelTitle;


	//these variables hold the shield level indicators
	public TextMeshProUGUI shields1Section;
	public TextMeshProUGUI shields2Section;
	public TextMeshProUGUI shields3Section;
	public TextMeshProUGUI shields4Section;
	public TextMeshProUGUI shields5Section;
	public TextMeshProUGUI shields6Section;

	public Slider shields1Slider;
	public Slider shields2Slider;
	public Slider shields3Slider;
	public Slider shields4Slider;
	public Slider shields5Slider;
	public Slider shields6Slider;

	public TextMeshProUGUI shields1Value;
	public TextMeshProUGUI shields2Value;
	public TextMeshProUGUI shields3Value;
	public TextMeshProUGUI shields4Value;
	public TextMeshProUGUI shields5Value;
	public TextMeshProUGUI shields6Value;

	public Image shields1Fill;
	public Image shields2Fill;
	public Image shields3Fill;
	public Image shields4Fill;
	public Image shields5Fill;
	public Image shields6Fill;

	public Image baseNameBackground;

	//these variables hold the transition points for the color of the shield bars
	private const float greenThreshold = .50f;
	private const float yellowThreshold = .25f;

	//these variables hold the section display groups
	public GameObject shipPhaserSection;
	public GameObject shipTorpedoSection;
	public GameObject shipStorageSection;
	public GameObject shipCrewSection;
	public GameObject shipEngineSection;

	public GameObject basePhaserSection1;
	public GameObject basePhaserSection2;
	public GameObject baseTorpedoSection;
	public GameObject baseCrewSection;
	public GameObject baseStorageSection1;
	public GameObject baseStorageSection2;

	//these variables hold the text mesh pro objects which display inventory values for ships
	public TextMeshProUGUI shipPhaserRadarShotText;
	public TextMeshProUGUI shipPhaserRadarArrayText;
	public TextMeshProUGUI shipXRayKernelText;
	public TextMeshProUGUI shipTractorBeamText;

	public TextMeshProUGUI shipLightTorpedoText;
	public TextMeshProUGUI shipHeavyTorpedoText;
	public TextMeshProUGUI shipTorpedoLaserShotText;
	public TextMeshProUGUI shipTorpedoLaserGuidanceText;
	public TextMeshProUGUI shipHighPressureTubesText;

	public TextMeshProUGUI shipDilithiumCrystalText;
	public TextMeshProUGUI shipTrilithiumCrystalText;
	public TextMeshProUGUI shipFlareText;
	public TextMeshProUGUI shipRadarJammingText;
	public TextMeshProUGUI shipLaserScatteringText;

	public TextMeshProUGUI shipRepairCrewText;
	public TextMeshProUGUI shipShieldEngineeringTeamText;
	public TextMeshProUGUI shipBattleCrewText;

	public TextMeshProUGUI shipWarpBoosterText;
	public TextMeshProUGUI shipTranswarpBoosterText;
	public TextMeshProUGUI shipWarpDriveText;
	public TextMeshProUGUI shipTranswarpDriveText;

	//these variables hold the text mesh pro objects which display inventory values for bases
	public TextMeshProUGUI basePhaserRadarShotText;
	public TextMeshProUGUI basePhaserRadarArrayText;

	public TextMeshProUGUI baseXRayKernelText;

	public TextMeshProUGUI baseLightTorpedoText;
	public TextMeshProUGUI baseHeavyTorpedoText;
	public TextMeshProUGUI baseTorpedoLaserShotText;
	public TextMeshProUGUI baseTorpedoLaserGuidanceText;
	public TextMeshProUGUI baseHighPressureTubesText;

	public TextMeshProUGUI baseDilithiumCrystalText;
	public TextMeshProUGUI baseRadarJammingText;

	public TextMeshProUGUI baseTrilithiumCrystalText;
	public TextMeshProUGUI baseFlareText;
	public TextMeshProUGUI baseLaserScatteringText;

	public TextMeshProUGUI baseRepairCrewText;
	public TextMeshProUGUI baseShieldEngineeringTeamText;
	public TextMeshProUGUI baseBattleCrewText;


	//variables to hold colors
	private Color greenColor = new Color (
		130.0f / 255.0f,
		214.0f / 255.0f,
		130.0f / 255.0f,
		255.0f / 255.0f);
	
	private Color purpleColor = new Color (
		180.0f / 255.0f, 
		147.0f / 255.0f, 
		214.0f / 255.0f, 
		255.0f / 255.0f);
	
	private Color redColor = new Color (
		214.0f / 255.0f, 
		130.0f / 255.0f, 
		130.0f / 255.0f, 
		255.0f  / 255.0f);
	
	private Color blueColor = new Color (
		130.0f / 255.0f, 
		172.0f / 255.0f, 
		214.0f / 255.0f, 
		255.0f / 255.0f);

	private Color displayTextColor = new Color (
		0.0f / 255.0f,
		225.0f / 255.0f,
		200.0f / 255.0f,
		255.0f / 255.0f);

	private Color displayTextDestroyedSectionColor = new Color (
		255.0f / 255.0f,
		0.0f / 255.0f,
		0.0f / 255.0f,
		255.0f / 255.0f);


	//event to announce setting highlighted unit
	public HighlightedUnitEvent OnSetHighlightedUnit = new HighlightedUnitEvent();

	//class to pass CombatUnit and RawImage to GraphicsManager
	public class HighlightedUnitEvent : UnityEvent<CombatUnit, RawImage>{};

	//unityActions
	private UnityAction<Ship,Hex,Hex> shipMoveSetHighlightedUnitAction;
	private UnityAction<Ship> shipSetHighlightedUnitAction;
	private UnityAction<CombatUnit,CombatUnit,int> attackSetHighlightedUnitAction;
	//private UnityAction<CombatUnit,CombatUnit,CombatManager.ShipSectionTargeted,int> attackSectionTargetedSetHighlightedUnitAction;
	private UnityAction<CombatUnit,CombatUnit> attackMissSetHighlightedUnitAction;
	private UnityAction<CombatUnit,CombatUnit,string> attackFiredSetHighlightedUnitAction;
	private UnityAction<CombatUnit,CombatUnit,CombatManager.CrystalType,int> crystalUsedSetHighlightedUnitAction;
	private UnityAction<CombatUnit> combatUnitHighlightedUnitAction;
	private UnityAction<Player> playerSetBorderColorAction;
	private UnityAction<CombatUnit,string> combatUnitSetUnitTextDisplayAction;
	private UnityAction<NameNewShip.NewUnitEventData> nameNewShipSetHighlightedUnitAction;


	// Use this for initialization
	public void Init () {
		
		//get the managers
		mouseManager = GameObject.FindGameObjectWithTag("MouseManager").GetComponent<MouseManager>();
		gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
		uiManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();

		//set the actions
		shipMoveSetHighlightedUnitAction = (ship, startingHex, endingHex) => {SetHighlightedUnit();};
		shipSetHighlightedUnitAction = (ship) => {SetHighlightedUnit();};
		attackSetHighlightedUnitAction = (attackingUnit, targetedUnit, damage) => {SetHighlightedUnit();};
		//attackSectionTargetedSetHighlightedUnitAction = (attackingUnit, targetedUnit, sectionTargeted, damage) => {SetHighlightedUnit();};
		attackMissSetHighlightedUnitAction = (attackingUnit, targetedUnit) => {SetHighlightedUnit();};
		attackFiredSetHighlightedUnitAction = (attackingUnit,targetedUnit,sectionTargeted) => {SetHighlightedUnit();};
		crystalUsedSetHighlightedUnitAction = (selectedUnit, targetedUnit, crystalType, shieldsHealed) => {SetHighlightedUnit();};
		combatUnitHighlightedUnitAction = (combatUnit) => {SetHighlightedUnit();};
		playerSetBorderColorAction = (player) => {SetUnitPanelBorder(player.color);};
		combatUnitSetUnitTextDisplayAction = (combatUnit, oldName)=> {SetUnitTextDisplay();};
		nameNewShipSetHighlightedUnitAction = (data) => {SetHighlightedUnit();};

		//add listeners for mouseManager setting targeted, selected, and hovered objects
		mouseManager.OnSetHoveredObject.AddListener(SetHighlightedUnit);
		mouseManager.OnClearHoveredObject.AddListener (SetHighlightedUnit);
		mouseManager.OnSetTargetedUnit.AddListener (SetHighlightedUnit);
		mouseManager.OnClearTargetedUnit.AddListener (SetHighlightedUnit);
		mouseManager.OnSetSelectedUnit.AddListener (SetHighlightedUnit);
		mouseManager.OnClearSelectedUnit.AddListener (SetHighlightedUnit);

		//add listeners for events
		//add a listener for when a ship movement is completed
		EngineSection.OnMoveFromToFinish.AddListener(shipMoveSetHighlightedUnitAction);

		//add listeners for warp boosters
		EngineSection.OnUseWarpBooster.AddListener(shipSetHighlightedUnitAction);
		EngineSection.OnUseTranswarpBooster.AddListener(shipSetHighlightedUnitAction);

		//add listeners for phaser attacks that hit ships
		/*
		//these are commented out to be replaced by cutscene events
		CombatManager.OnPhaserAttackHitShipPhaserSection.AddListener(attackSetHighlightedUnitAction);
		CombatManager.OnPhaserAttackHitShipTorpedoSection.AddListener(attackSetHighlightedUnitAction);
		CombatManager.OnPhaserAttackHitShipStorageSection.AddListener(attackSetHighlightedUnitAction);
		CombatManager.OnPhaserAttackHitShipCrewSection.AddListener(attackSetHighlightedUnitAction);
		CombatManager.OnPhaserAttackHitShipEngineSection.AddListener(attackSetHighlightedUnitAction);
		CombatManager.OnPhaserAttackMissShip.AddListener(attackMissSetHighlightedUnitAction);
		*/

		//add listeners for phaser attacks that hit ships
		CutsceneManager.OnPhaserHitShipPhaserSection.AddListener (attackSetHighlightedUnitAction);
		CutsceneManager.OnPhaserHitShipTorpedoSection.AddListener (attackSetHighlightedUnitAction);
		CutsceneManager.OnPhaserHitShipStorageSection.AddListener (attackSetHighlightedUnitAction);
		CutsceneManager.OnPhaserHitShipCrewSection.AddListener (attackSetHighlightedUnitAction);
		CutsceneManager.OnPhaserHitShipEngineSection.AddListener (attackSetHighlightedUnitAction);
		CutsceneManager.OnPhaserMissShip.AddListener (attackSetHighlightedUnitAction);


		//add listeners for phaser attacks that hit bases
		/*
		//these are commented out to be replaced by cutscene events
		CombatManager.OnPhaserAttackHitBasePhaserSection1.AddListener(attackSetHighlightedUnitAction);
		CombatManager.OnPhaserAttackHitBasePhaserSection2.AddListener(attackSetHighlightedUnitAction);
		CombatManager.OnPhaserAttackHitBaseTorpedoSection.AddListener(attackSetHighlightedUnitAction);
		CombatManager.OnPhaserAttackHitBaseCrewSection.AddListener(attackSetHighlightedUnitAction);
		CombatManager.OnPhaserAttackHitBaseStorageSection1.AddListener(attackSetHighlightedUnitAction);
		CombatManager.OnPhaserAttackHitBaseStorageSection2.AddListener(attackSetHighlightedUnitAction);
		CombatManager.OnPhaserAttackMissBase.AddListener(attackMissSetHighlightedUnitAction);
		*/

		//add listeners for phaser attacks that hit bases
		CutsceneManager.OnPhaserHitBasePhaserSection1.AddListener (attackSetHighlightedUnitAction);
		CutsceneManager.OnPhaserHitBasePhaserSection2.AddListener (attackSetHighlightedUnitAction);
		CutsceneManager.OnPhaserHitBaseTorpedoSection.AddListener (attackSetHighlightedUnitAction);
		CutsceneManager.OnPhaserHitBaseCrewSection.AddListener (attackSetHighlightedUnitAction);
		CutsceneManager.OnPhaserHitBaseStorageSection1.AddListener (attackSetHighlightedUnitAction);
		CutsceneManager.OnPhaserHitBaseStorageSection2.AddListener (attackSetHighlightedUnitAction);
		CutsceneManager.OnPhaserMissBase.AddListener (attackSetHighlightedUnitAction);

		//add listeners for torpedo attacks by ships
		TorpedoSection.OnFireLightTorpedo.AddListener(attackFiredSetHighlightedUnitAction);
		TorpedoSection.OnFireHeavyTorpedo.AddListener(attackFiredSetHighlightedUnitAction);

		//add listeners for torpedo attacks by bases
		StarbaseTorpedoSection.OnFireLightTorpedo.AddListener(attackFiredSetHighlightedUnitAction);
		StarbaseTorpedoSection.OnFireHeavyTorpedo.AddListener(attackFiredSetHighlightedUnitAction);

		//add listeners for flare successes and failures
		/*
		//these are commented out to be replaced by cutscene events
		CombatManager.OnLightTorpedoAttackDefeatedByFlares.AddListener(attackSectionTargetedSetHighlightedUnitAction);
		CombatManager.OnHeavyTorpedoAttackDefeatedByFlares.AddListener(attackSectionTargetedSetHighlightedUnitAction);
		CombatManager.OnLightTorpedoAttackFlaresFailed.AddListener(attackSetHighlightedUnitAction);
		CombatManager.OnHeavyTorpedoAttackFlaresFailed.AddListener(attackSetHighlightedUnitAction);
		*/

		//add listeners for flare successes and failures
		CutsceneManager.OnLightTorpedoFlareSuccess.AddListener (attackSetHighlightedUnitAction);
		CutsceneManager.OnHeavyTorpedoFlareSuccess.AddListener (attackSetHighlightedUnitAction);
		CutsceneManager.OnLightTorpedoFlareFailure.AddListener (attackSetHighlightedUnitAction);
		CutsceneManager.OnHeavyTorpedoFlareFailure.AddListener (attackSetHighlightedUnitAction);

		//add listeners for ships hit by torpedo attacks
		/*
		//these are commented out to be replaced by cutscene events
		CombatManager.OnLightTorpedoAttackHitShipPhaserSection.AddListener(attackSetHighlightedUnitAction);
		CombatManager.OnHeavyTorpedoAttackHitShipPhaserSection.AddListener(attackSetHighlightedUnitAction);
		CombatManager.OnLightTorpedoAttackHitShipTorpedoSection.AddListener(attackSetHighlightedUnitAction);
		CombatManager.OnHeavyTorpedoAttackHitShipTorpedoSection.AddListener(attackSetHighlightedUnitAction);
		CombatManager.OnLightTorpedoAttackHitShipStorageSection.AddListener(attackSetHighlightedUnitAction);
		CombatManager.OnHeavyTorpedoAttackHitShipStorageSection.AddListener(attackSetHighlightedUnitAction);
		CombatManager.OnLightTorpedoAttackHitShipCrewSection.AddListener(attackSetHighlightedUnitAction);
		CombatManager.OnHeavyTorpedoAttackHitShipCrewSection.AddListener(attackSetHighlightedUnitAction);
		CombatManager.OnLightTorpedoAttackHitShipEngineSection.AddListener(attackSetHighlightedUnitAction);
		CombatManager.OnHeavyTorpedoAttackHitShipEngineSection.AddListener(attackSetHighlightedUnitAction);
		CombatManager.OnLightTorpedoAttackMissShip.AddListener(attackMissSetHighlightedUnitAction);
		CombatManager.OnHeavyTorpedoAttackMissShip.AddListener(attackMissSetHighlightedUnitAction);
		*/

		//add listeners for ships hit by torpedo attacks
		CutsceneManager.OnLightTorpedoHitShipPhaserSection.AddListener (attackSetHighlightedUnitAction);
		CutsceneManager.OnHeavyTorpedoHitShipPhaserSection.AddListener (attackSetHighlightedUnitAction);
		CutsceneManager.OnLightTorpedoHitShipTorpedoSection.AddListener (attackSetHighlightedUnitAction);
		CutsceneManager.OnHeavyTorpedoHitShipTorpedoSection.AddListener (attackSetHighlightedUnitAction);
		CutsceneManager.OnLightTorpedoHitShipStorageSection.AddListener (attackSetHighlightedUnitAction);
		CutsceneManager.OnHeavyTorpedoHitShipStorageSection.AddListener (attackSetHighlightedUnitAction);
		CutsceneManager.OnLightTorpedoHitShipCrewSection.AddListener (attackSetHighlightedUnitAction);
		CutsceneManager.OnHeavyTorpedoHitShipCrewSection.AddListener (attackSetHighlightedUnitAction);
		CutsceneManager.OnLightTorpedoHitShipEngineSection.AddListener (attackSetHighlightedUnitAction);
		CutsceneManager.OnHeavyTorpedoHitShipEngineSection.AddListener (attackSetHighlightedUnitAction);
		CutsceneManager.OnLightTorpedoMissBase.AddListener (attackSetHighlightedUnitAction);
		CutsceneManager.OnHeavyTorpedoMissBase.AddListener (attackSetHighlightedUnitAction);


		//add listeners for bases hit by torpedo attacks
		/*
		//these are commented out to be replaced by cutscene events
		CombatManager.OnLightTorpedoAttackHitBasePhaserSection1.AddListener(attackSetHighlightedUnitAction);
		CombatManager.OnHeavyTorpedoAttackHitBasePhaserSection1.AddListener(attackSetHighlightedUnitAction);
		CombatManager.OnLightTorpedoAttackHitBasePhaserSection2.AddListener(attackSetHighlightedUnitAction);
		CombatManager.OnHeavyTorpedoAttackHitBasePhaserSection2.AddListener(attackSetHighlightedUnitAction);
		CombatManager.OnLightTorpedoAttackHitBaseTorpedoSection.AddListener(attackSetHighlightedUnitAction);
		CombatManager.OnHeavyTorpedoAttackHitBaseTorpedoSection.AddListener(attackSetHighlightedUnitAction);
		CombatManager.OnLightTorpedoAttackHitBaseCrewSection.AddListener(attackSetHighlightedUnitAction);
		CombatManager.OnHeavyTorpedoAttackHitBaseCrewSection.AddListener(attackSetHighlightedUnitAction);
		CombatManager.OnLightTorpedoAttackHitBaseStorageSection1.AddListener(attackSetHighlightedUnitAction);
		CombatManager.OnHeavyTorpedoAttackHitBaseStorageSection1.AddListener(attackSetHighlightedUnitAction);
		CombatManager.OnLightTorpedoAttackHitBaseStorageSection2.AddListener(attackSetHighlightedUnitAction);
		CombatManager.OnHeavyTorpedoAttackHitBaseStorageSection2.AddListener(attackSetHighlightedUnitAction);
		CombatManager.OnLightTorpedoAttackMissBase.AddListener(attackMissSetHighlightedUnitAction);
		CombatManager.OnHeavyTorpedoAttackMissBase.AddListener(attackMissSetHighlightedUnitAction);
		*/

		//add listeners for bases hit by torpedo attacks
		CutsceneManager.OnLightTorpedoHitBasePhaserSection1.AddListener (attackSetHighlightedUnitAction);
		CutsceneManager.OnHeavyTorpedoHitBasePhaserSection1.AddListener (attackSetHighlightedUnitAction);
		CutsceneManager.OnLightTorpedoHitBasePhaserSection2.AddListener (attackSetHighlightedUnitAction);
		CutsceneManager.OnHeavyTorpedoHitBasePhaserSection2.AddListener (attackSetHighlightedUnitAction);
		CutsceneManager.OnLightTorpedoHitBaseTorpedoSection.AddListener (attackSetHighlightedUnitAction);
		CutsceneManager.OnHeavyTorpedoHitBaseTorpedoSection.AddListener (attackSetHighlightedUnitAction);
		CutsceneManager.OnLightTorpedoHitBaseCrewSection.AddListener (attackSetHighlightedUnitAction);
		CutsceneManager.OnHeavyTorpedoHitBaseCrewSection.AddListener (attackSetHighlightedUnitAction);
		CutsceneManager.OnLightTorpedoHitBaseStorageSection1.AddListener (attackSetHighlightedUnitAction);
		CutsceneManager.OnHeavyTorpedoHitBaseStorageSection1.AddListener (attackSetHighlightedUnitAction);
		CutsceneManager.OnLightTorpedoHitBaseStorageSection2.AddListener (attackSetHighlightedUnitAction);
		CutsceneManager.OnHeavyTorpedoHitBaseStorageSection2.AddListener (attackSetHighlightedUnitAction);
		CutsceneManager.OnLightTorpedoMissBase.AddListener (attackSetHighlightedUnitAction);
		CutsceneManager.OnHeavyTorpedoMissBase.AddListener (attackSetHighlightedUnitAction);

		//add listeners for using crystals to heal ships
		CombatManager.OnCrystalUsedOnShipPhaserSection.AddListener(crystalUsedSetHighlightedUnitAction);
		CombatManager.OnCrystalUsedOnShipTorpedoSection.AddListener(crystalUsedSetHighlightedUnitAction);
		CombatManager.OnCrystalUsedOnShipStorageSection.AddListener(crystalUsedSetHighlightedUnitAction);
		CombatManager.OnCrystalUsedOnShipCrewSection.AddListener(crystalUsedSetHighlightedUnitAction);
		CombatManager.OnCrystalUsedOnShipEngineSection.AddListener(crystalUsedSetHighlightedUnitAction);

		//add listeners for using crystals to heal bases
		CombatManager.OnCrystalUsedOnBasePhaserSection1.AddListener(crystalUsedSetHighlightedUnitAction);
		CombatManager.OnCrystalUsedOnBasePhaserSection2.AddListener(crystalUsedSetHighlightedUnitAction);
		CombatManager.OnCrystalUsedOnBaseTorpedoSection.AddListener(crystalUsedSetHighlightedUnitAction);
		CombatManager.OnCrystalUsedOnBaseCrewSection.AddListener(crystalUsedSetHighlightedUnitAction);
		CombatManager.OnCrystalUsedOnBaseStorageSection1.AddListener(crystalUsedSetHighlightedUnitAction);
		CombatManager.OnCrystalUsedOnBaseStorageSection2.AddListener(crystalUsedSetHighlightedUnitAction);

		//add listeners for using repair crews on ships
		CombatManager.OnRepairCrewUsedOnShipPhaserSection.AddListener(attackMissSetHighlightedUnitAction);
		CombatManager.OnRepairCrewUsedOnShipTorpedoSection.AddListener(attackMissSetHighlightedUnitAction);
		CombatManager.OnRepairCrewUsedOnShipStorageSection.AddListener(attackMissSetHighlightedUnitAction);
		CombatManager.OnRepairCrewUsedOnShipCrewSection.AddListener(attackMissSetHighlightedUnitAction);
		CombatManager.OnRepairCrewUsedOnShipEngineSection.AddListener(attackMissSetHighlightedUnitAction);

		//add listeners for using repair crews on bases
		CombatManager.OnRepairCrewUsedOnBasePhaserSection1.AddListener(attackMissSetHighlightedUnitAction);
		CombatManager.OnRepairCrewUsedOnBasePhaserSection2.AddListener(attackMissSetHighlightedUnitAction);
		CombatManager.OnRepairCrewUsedOnBaseTorpedoSection.AddListener(attackMissSetHighlightedUnitAction);
		CombatManager.OnRepairCrewUsedOnBaseCrewSection.AddListener(attackMissSetHighlightedUnitAction);
		CombatManager.OnRepairCrewUsedOnBaseStorageSection1.AddListener(attackMissSetHighlightedUnitAction);
		CombatManager.OnRepairCrewUsedOnBaseStorageSection2.AddListener(attackMissSetHighlightedUnitAction);

		//add listeners for ship sections being destroyed
		PhaserSection.OnPhaserSectionDestroyed.AddListener(combatUnitHighlightedUnitAction);
		TorpedoSection.OnTorpedoSectionDestroyed.AddListener(combatUnitHighlightedUnitAction);
		StorageSection.OnStorageSectionDestroyed.AddListener(combatUnitHighlightedUnitAction);
		CrewSection.OnCrewSectionDestroyed.AddListener(combatUnitHighlightedUnitAction);
		EngineSection.OnEngineSectionDestroyed.AddListener(combatUnitHighlightedUnitAction);

		//add listeners for base sections being destroyed
		StarbasePhaserSection1.OnPhaserSection1Destroyed.AddListener(combatUnitHighlightedUnitAction);
		StarbasePhaserSection2.OnPhaserSection2Destroyed.AddListener(combatUnitHighlightedUnitAction);
		StarbaseTorpedoSection.OnTorpedoSectionDestroyed.AddListener(combatUnitHighlightedUnitAction);
		StarbaseCrewSection.OnCrewSectionDestroyed.AddListener(combatUnitHighlightedUnitAction);
		StarbaseStorageSection1.OnStorageSection1Destroyed.AddListener(combatUnitHighlightedUnitAction);
		StarbaseStorageSection2.OnStorageSection2Destroyed.AddListener(combatUnitHighlightedUnitAction);

		//add listeners for ship sections being repaired
		PhaserSection.OnPhaserSectionRepaired.AddListener(combatUnitHighlightedUnitAction);
		TorpedoSection.OnTorpedoSectionRepaired.AddListener(combatUnitHighlightedUnitAction);
		StorageSection.OnStorageSectionRepaired.AddListener(combatUnitHighlightedUnitAction);
		CrewSection.OnCrewSectionRepaired.AddListener(combatUnitHighlightedUnitAction);
		EngineSection.OnEngineSectionRepaired.AddListener(combatUnitHighlightedUnitAction);

		//add listeners for base sections being repaired
		StarbasePhaserSection1.OnPhaserSection1Repaired.AddListener(combatUnitHighlightedUnitAction);
		StarbasePhaserSection2.OnPhaserSection2Repaired.AddListener(combatUnitHighlightedUnitAction);
		StarbaseTorpedoSection.OnTorpedoSectionRepaired.AddListener(combatUnitHighlightedUnitAction);
		StarbaseCrewSection.OnCrewSectionRepaired.AddListener(combatUnitHighlightedUnitAction);
		StarbaseStorageSection1.OnStorageSection1Repaired.AddListener(combatUnitHighlightedUnitAction);
		StarbaseStorageSection2.OnStorageSection2Repaired.AddListener(combatUnitHighlightedUnitAction);

		//add listener for ship being destroyed
		Ship.OnShipDestroyed.AddListener(combatUnitHighlightedUnitAction);

		//add listener for base being destroyed
		Starbase.OnBaseDestroyed.AddListener(combatUnitHighlightedUnitAction);

		//add listeners for damage being taken by ships
		PhaserSection.OnPhaserDamageTaken.AddListener(SetHighlightedUnit);
		TorpedoSection.OnTorpedoDamageTaken.AddListener(SetHighlightedUnit);
		StorageSection.OnStorageDamageTaken.AddListener(SetHighlightedUnit);
		CrewSection.OnCrewDamageTaken.AddListener(SetHighlightedUnit);
		EngineSection.OnEngineDamageTaken.AddListener(SetHighlightedUnit);

		//add listeners for damage being taken by bases
		StarbasePhaserSection1.OnPhaserDamageTaken.AddListener(SetHighlightedUnit);
		StarbasePhaserSection2.OnPhaserDamageTaken.AddListener(SetHighlightedUnit);
		StarbaseTorpedoSection.OnTorpedoDamageTaken.AddListener(SetHighlightedUnit);
		StarbaseCrewSection.OnCrewDamageTaken.AddListener(SetHighlightedUnit);
		StarbaseStorageSection1.OnStorageDamageTaken.AddListener(SetHighlightedUnit);
		StarbaseStorageSection2.OnStorageDamageTaken.AddListener(SetHighlightedUnit);

		//add listener for the end turn event
		gameManager.OnNewTurn.AddListener(playerSetBorderColorAction);
		gameManager.OnLoadedTurn.AddListener(playerSetBorderColorAction);

		//add listener for the ship rename event
		Ship.OnShipRenamed.AddListener(combatUnitSetUnitTextDisplayAction);

		//add listener for the base rename event
		Starbase.OnBaseRenamed.AddListener(combatUnitSetUnitTextDisplayAction);

		//add listeners for engaging and disengaging cloaking
		CloakingDevice.OnEngageCloakingDevice.AddListener(combatUnitHighlightedUnitAction);
		CloakingDevice.OnDisengageCloakingDevice.AddListener(combatUnitHighlightedUnitAction);

		//add listener for updating attack status
		CombatUnit.OnUpdateAttackStatus.AddListener(combatUnitHighlightedUnitAction);

		//add listener for updating repair usage status
		Starship.OnUpdateRepairStatus.AddListener(combatUnitHighlightedUnitAction);
		Starbase.OnUpdateRepairStatus.AddListener(combatUnitHighlightedUnitAction);

		//add listener for updating cloaking status
		BirdOfPrey.OnUpdateCloakingStatus.AddListener(combatUnitHighlightedUnitAction);

		//add listeners for purchasing items
		PhaserSection.OnInventoryUpdated.AddListener(combatUnitHighlightedUnitAction);
		TorpedoSection.OnInventoryUpdated.AddListener(combatUnitHighlightedUnitAction);
		StorageSection.OnInventoryUpdated.AddListener(combatUnitHighlightedUnitAction);
		CrewSection.OnInventoryUpdated.AddListener(combatUnitHighlightedUnitAction);
		EngineSection.OnInventoryUpdated.AddListener(combatUnitHighlightedUnitAction);

		//add listener for purchasing a ship
		uiManager.GetComponent<NameNewShip>().OnPurchasedNewShip.AddListener(nameNewShipSetHighlightedUnitAction);

		//make the sliders non interactable - they are for display only
		shields1Slider.interactable = false;
		shields2Slider.interactable = false;
		shields3Slider.interactable = false;
		shields4Slider.interactable = false;
		shields5Slider.interactable = false;
		shields6Slider.interactable = false;

		//start the shields display at blank
		//scale the blank sections to 0
		shields1Section.rectTransform.localScale = Vector3.zero;
		shields2Section.rectTransform.localScale = Vector3.zero;
		shields3Section.rectTransform.localScale = Vector3.zero;
		shields4Section.rectTransform.localScale = Vector3.zero;
		shields5Section.rectTransform.localScale = Vector3.zero;
		shields6Section.rectTransform.localScale = Vector3.zero;

		//scale the blank sections to 0
		shields1Slider.fillRect.localScale = Vector3.zero;
		shields2Slider.fillRect.localScale = Vector3.zero;
		shields3Slider.fillRect.localScale = Vector3.zero;
		shields4Slider.fillRect.localScale = Vector3.zero;
		shields5Slider.fillRect.localScale = Vector3.zero;
		shields6Slider.fillRect.localScale = Vector3.zero;

		//scale the blank sections to 0
		shields1Value.rectTransform.localScale = Vector3.zero;
		shields2Value.rectTransform.localScale = Vector3.zero;
		shields3Value.rectTransform.localScale = Vector3.zero;
		shields4Value.rectTransform.localScale = Vector3.zero;
		shields5Value.rectTransform.localScale = Vector3.zero;
		shields6Value.rectTransform.localScale = Vector3.zero;

		//scale the ship overlays to zero to start
		highlightedUnitPanelTitle.rectTransform.localScale = Vector3.zero;
		shipName.rectTransform.localScale = Vector3.zero;
		torpedoText.rectTransform.localScale = Vector3.zero;
		phaserText.rectTransform.localScale = Vector3.zero;
		movementText.rectTransform.localScale = Vector3.zero;
		specialText.rectTransform.localScale = Vector3.zero;

		baseNameBackground.rectTransform.localScale = Vector3.zero;

		//scale the image based on resolution
		//ScaleHighlightedUnitImage();

	}
	

	//this function sets the highlighted unit
	private void SetHighlightedUnit(){

		//check if there is currently a hovered object
		if (mouseManager.hoveredObject != null) {
			
			if (mouseManager.hoveredObject.GetComponent<CombatUnit> () == true) {

				//set the highlighted unit to the hovered unit
				highlightedUnit = mouseManager.hoveredObject.GetComponent<CombatUnit> ();

			}
			//the else condition is that there is not a hovered object combat unit.  In this case, check if we have a targeted unit
			else if (mouseManager.targetedUnit != null) {

				//set the highlighted unit to the targeted unit
				highlightedUnit = mouseManager.targetedUnit.GetComponent<CombatUnit> ();

			}
			//if there is no hovered unit or targeted unit, we want to set the highlighted unit to the selected unit
			else if (mouseManager.selectedUnit != null) {

				//set the highlighted unit to the selected unit
				highlightedUnit = mouseManager.selectedUnit.GetComponent<CombatUnit> ();

			}
			//the else condition is that we don't have a unit hovered, targeted, or selected.
			//in this case, set the highlighted unit to null
			else {

				highlightedUnit = null;

			}

		}
		//the else condition is that there is not a hovered object.  In this case, check if we have a targeted unit
		else if (mouseManager.targetedUnit != null) {

			//set the highlighted unit to the targeted unit
			highlightedUnit = mouseManager.targetedUnit.GetComponent<CombatUnit> ();

		}
		//if there is no hovered unit or targeted unit, we want to set the highlighted unit to the selected unit
		else if (mouseManager.selectedUnit != null) {

			//set the highlighted unit to the selected unit
			highlightedUnit = mouseManager.selectedUnit.GetComponent<CombatUnit> ();

		}
		//the else condition is that we don't have a unit hovered, targeted, or selected.
		//in this case, set the highlighted unit to null
		else {

			highlightedUnit = null;

		}
			
		//invoke the OnSetHighlightedUnit event
		OnSetHighlightedUnit.Invoke(highlightedUnit, highlightedUnitImage);

		//update the displayed shields
		SetShieldsDisplay ();
		SetUnitTextDisplay ();
		SetTitleTextColor ();

		//update the section displays
		SetSectionDislplays();

	}


	//this function will set the shields display for the highlighted unit
	private void SetShieldsDisplay(){

		if (highlightedUnit != null) {

			//first, we need to check what kind of unit we have
			switch (highlightedUnit.unitType) {

			case CombatUnit.UnitType.Starship:

			//section Texts
			//scale the known sections
				shields1Section.rectTransform.localScale = Vector3.one;
				shields2Section.rectTransform.localScale = Vector3.one;
				shields3Section.rectTransform.localScale = Vector3.one;
				shields4Section.rectTransform.localScale = Vector3.one;
				shields5Section.rectTransform.localScale = Vector3.one;
		
			//assign the known sections
				shields1Section.text = ("Engine");
				shields2Section.text = ("Phaser");
				shields3Section.text = ("Torpedo");
				shields4Section.text = ("Crew");
				shields5Section.text = ("Storage");

				//update the font size if necessary
				UIManager.AutoSizeTextMeshFont (shields1Section);
				UIManager.AutoSizeTextMeshFont (shields2Section);
				UIManager.AutoSizeTextMeshFont (shields3Section);
				UIManager.AutoSizeTextMeshFont (shields4Section);
				UIManager.AutoSizeTextMeshFont (shields5Section);

			//scale the blank sections to 0
				shields6Section.rectTransform.localScale = Vector3.zero;

			//sliders
			//scale the known sections to 1
				shields1Slider.fillRect.localScale = Vector3.one;
				shields2Slider.fillRect.localScale = Vector3.one;
				shields3Slider.fillRect.localScale = Vector3.one;
				shields4Slider.fillRect.localScale = Vector3.one;
				shields5Slider.fillRect.localScale = Vector3.one;

			//set the slider range to the shield range
				shields1Slider.maxValue = (float)highlightedUnit.GetComponent<EngineSection> ().shieldsMax;
				shields2Slider.maxValue = (float)highlightedUnit.GetComponent<PhaserSection> ().shieldsMax;
				shields3Slider.maxValue = (float)highlightedUnit.GetComponent<TorpedoSection> ().shieldsMax;
				shields4Slider.maxValue = (float)highlightedUnit.GetComponent<CrewSection> ().shieldsMax;
				shields5Slider.maxValue = (float)highlightedUnit.GetComponent<StorageSection> ().shieldsMax;

				shields1Slider.value = (float)highlightedUnit.GetComponent<EngineSection> ().shieldsCurrent;
				shields2Slider.value = (float)highlightedUnit.GetComponent<PhaserSection> ().shieldsCurrent;
				shields3Slider.value = (float)highlightedUnit.GetComponent<TorpedoSection> ().shieldsCurrent;
				shields4Slider.value = (float)highlightedUnit.GetComponent<CrewSection> ().shieldsCurrent;
				shields5Slider.value = (float)highlightedUnit.GetComponent<StorageSection> ().shieldsCurrent;

			//scale the blank sections to 0
				shields6Slider.fillRect.localScale = Vector3.zero;

			//current shields
			//scale the known sections to 1
				shields1Value.rectTransform.localScale = Vector3.one;
				shields2Value.rectTransform.localScale = Vector3.one;
				shields3Value.rectTransform.localScale = Vector3.one;
				shields4Value.rectTransform.localScale = Vector3.one;
				shields5Value.rectTransform.localScale = Vector3.one;

			//assign the known current shields
				shields1Value.text = (highlightedUnit.GetComponent<EngineSection> ().shieldsCurrent.ToString ());
				shields2Value.text = (highlightedUnit.GetComponent<PhaserSection> ().shieldsCurrent.ToString ());
				shields3Value.text = (highlightedUnit.GetComponent<TorpedoSection> ().shieldsCurrent.ToString ());
				shields4Value.text = (highlightedUnit.GetComponent<CrewSection> ().shieldsCurrent.ToString ());
				shields5Value.text = (highlightedUnit.GetComponent<StorageSection> ().shieldsCurrent.ToString ());

			//scale the blank sections to 0
				shields6Value.rectTransform.localScale = Vector3.zero;

			//color the shield bars
				SetShieldBarColor (shields1Fill, shields1Slider.value, shields1Slider.maxValue);
				SetShieldBarColor (shields2Fill, shields2Slider.value, shields2Slider.maxValue);
				SetShieldBarColor (shields3Fill, shields3Slider.value, shields3Slider.maxValue);
				SetShieldBarColor (shields4Fill, shields4Slider.value, shields4Slider.maxValue);
				SetShieldBarColor (shields5Fill, shields5Slider.value, shields5Slider.maxValue);

				//color code the section names and shield values
				if (highlightedUnit.GetComponent<EngineSection> ().isDestroyed == false) {

					shields1Section.color = displayTextColor;
					shields1Value.color = displayTextColor;

				} else {

					shields1Section.color = displayTextDestroyedSectionColor;
					shields1Value.color = displayTextDestroyedSectionColor;

				}

				if (highlightedUnit.GetComponent<PhaserSection> ().isDestroyed == false) {

					shields2Section.color = displayTextColor;
					shields2Value.color = displayTextColor;

				} else {

					shields2Section.color = displayTextDestroyedSectionColor;
					shields2Value.color = displayTextDestroyedSectionColor;

				}

				if (highlightedUnit.GetComponent<TorpedoSection> ().isDestroyed == false) {

					shields3Section.color = displayTextColor;
					shields3Value.color = displayTextColor;

				} else {

					shields3Section.color = displayTextDestroyedSectionColor;
					shields3Value.color = displayTextDestroyedSectionColor;

				}

				if (highlightedUnit.GetComponent<CrewSection> ().isDestroyed == false) {

					shields4Section.color = displayTextColor;
					shields4Value.color = displayTextColor;

				} else {

					shields4Section.color = displayTextDestroyedSectionColor;
					shields4Value.color = displayTextDestroyedSectionColor;

				}

				if (highlightedUnit.GetComponent<StorageSection> ().isDestroyed == false) {

					shields5Section.color = displayTextColor;
					shields5Value.color = displayTextColor;

				} else {

					shields5Section.color = displayTextDestroyedSectionColor;
					shields5Value.color = displayTextDestroyedSectionColor;

				}

				break;

			case CombatUnit.UnitType.Destroyer:

			//section Texts
			//scale the known sections
				shields1Section.rectTransform.localScale = Vector3.one;
				shields2Section.rectTransform.localScale = Vector3.one;
				shields3Section.rectTransform.localScale = Vector3.one;
				shields4Section.rectTransform.localScale = Vector3.one;

			//assign the known sections
				shields1Section.text = ("Engine");
				shields2Section.text = ("Phaser");
				shields3Section.text = ("Torpedo");
				shields4Section.text = ("Storage");

				//update the font size if necessary
				UIManager.AutoSizeTextMeshFont (shields1Section);
				UIManager.AutoSizeTextMeshFont (shields2Section);
				UIManager.AutoSizeTextMeshFont (shields3Section);
				UIManager.AutoSizeTextMeshFont (shields4Section);

			//scale the blank sections to 0
				shields5Section.rectTransform.localScale = Vector3.zero;
				shields6Section.rectTransform.localScale = Vector3.zero;


			//sliders
			//scale the known sections to 1
				shields1Slider.fillRect.localScale = Vector3.one;
				shields2Slider.fillRect.localScale = Vector3.one;
				shields3Slider.fillRect.localScale = Vector3.one;
				shields4Slider.fillRect.localScale = Vector3.one;

			//set the slider range to the shield range
				shields1Slider.maxValue = (float)highlightedUnit.GetComponent<EngineSection> ().shieldsMax;
				shields2Slider.maxValue = (float)highlightedUnit.GetComponent<PhaserSection> ().shieldsMax;
				shields3Slider.maxValue = (float)highlightedUnit.GetComponent<TorpedoSection> ().shieldsMax;
				shields4Slider.maxValue = (float)highlightedUnit.GetComponent<StorageSection> ().shieldsMax;

				shields1Slider.value = (float)highlightedUnit.GetComponent<EngineSection> ().shieldsCurrent;
				shields2Slider.value = (float)highlightedUnit.GetComponent<PhaserSection> ().shieldsCurrent;
				shields3Slider.value = (float)highlightedUnit.GetComponent<TorpedoSection> ().shieldsCurrent;
				shields4Slider.value = (float)highlightedUnit.GetComponent<StorageSection> ().shieldsCurrent;

			//scale the blank sections to 0
				shields5Slider.fillRect.localScale = Vector3.zero;
				shields6Slider.fillRect.localScale = Vector3.zero;

			//current shields
			//scale the known sections to 1
				shields1Value.rectTransform.localScale = Vector3.one;
				shields2Value.rectTransform.localScale = Vector3.one;
				shields3Value.rectTransform.localScale = Vector3.one;
				shields4Value.rectTransform.localScale = Vector3.one;

			//assign the known current shields
				shields1Value.text = (highlightedUnit.GetComponent<EngineSection> ().shieldsCurrent.ToString ());
				shields2Value.text = (highlightedUnit.GetComponent<PhaserSection> ().shieldsCurrent.ToString ());
				shields3Value.text = (highlightedUnit.GetComponent<TorpedoSection> ().shieldsCurrent.ToString ());
				shields4Value.text = (highlightedUnit.GetComponent<StorageSection> ().shieldsCurrent.ToString ());

			//scale the blank sections to 0
				shields5Value.rectTransform.localScale = Vector3.zero;
				shields6Value.rectTransform.localScale = Vector3.zero;

			//color the shield bars
				SetShieldBarColor (shields1Fill, shields1Slider.value, shields1Slider.maxValue);
				SetShieldBarColor (shields2Fill, shields2Slider.value, shields2Slider.maxValue);
				SetShieldBarColor (shields3Fill, shields3Slider.value, shields3Slider.maxValue);
				SetShieldBarColor (shields4Fill, shields4Slider.value, shields4Slider.maxValue);

				//color code the section names and shield values
				if (highlightedUnit.GetComponent<EngineSection> ().isDestroyed == false) {

					shields1Section.color = displayTextColor;
					shields1Value.color = displayTextColor;

				} else {

					shields1Section.color = displayTextDestroyedSectionColor;
					shields1Value.color = displayTextDestroyedSectionColor;

				}

				if (highlightedUnit.GetComponent<PhaserSection> ().isDestroyed == false) {

					shields2Section.color = displayTextColor;
					shields2Value.color = displayTextColor;

				} else {

					shields2Section.color = displayTextDestroyedSectionColor;
					shields2Value.color = displayTextDestroyedSectionColor;

				}

				if (highlightedUnit.GetComponent<TorpedoSection> ().isDestroyed == false) {

					shields3Section.color = displayTextColor;
					shields3Value.color = displayTextColor;

				} else {

					shields3Section.color = displayTextDestroyedSectionColor;
					shields3Value.color = displayTextDestroyedSectionColor;

				}

				if (highlightedUnit.GetComponent<StorageSection> ().isDestroyed == false) {

					shields4Section.color = displayTextColor;
					shields4Value.color = displayTextColor;

				} else {

					shields4Section.color = displayTextDestroyedSectionColor;
					shields4Value.color = displayTextDestroyedSectionColor;

				}

				break;

			case CombatUnit.UnitType.BirdOfPrey:

			//section Texts
			//scale the known sections
				shields1Section.rectTransform.localScale = Vector3.one;
				shields2Section.rectTransform.localScale = Vector3.one;
				shields3Section.rectTransform.localScale = Vector3.one;

			//assign the known sections
				shields1Section.text = ("Engine");
				shields2Section.text = ("Phaser");
				shields3Section.text = ("Torpedo");

				//update the font size if necessary
				UIManager.AutoSizeTextMeshFont (shields1Section);
				UIManager.AutoSizeTextMeshFont (shields2Section);
				UIManager.AutoSizeTextMeshFont (shields3Section);

			//scale the blank sections to 0
				shields4Section.rectTransform.localScale = Vector3.zero;
				shields5Section.rectTransform.localScale = Vector3.zero;
				shields6Section.rectTransform.localScale = Vector3.zero;


			//sliders
			//scale the known sections to 1
				shields1Slider.fillRect.localScale = Vector3.one;
				shields2Slider.fillRect.localScale = Vector3.one;
				shields3Slider.fillRect.localScale = Vector3.one;

			//set the slider range to the shield range
				shields1Slider.maxValue = (float)highlightedUnit.GetComponent<EngineSection> ().shieldsMax;
				shields2Slider.maxValue = (float)highlightedUnit.GetComponent<PhaserSection> ().shieldsMax;
				shields3Slider.maxValue = (float)highlightedUnit.GetComponent<TorpedoSection> ().shieldsMax;

				shields1Slider.value = (float)highlightedUnit.GetComponent<EngineSection> ().shieldsCurrent;
				shields2Slider.value = (float)highlightedUnit.GetComponent<PhaserSection> ().shieldsCurrent;
				shields3Slider.value = (float)highlightedUnit.GetComponent<TorpedoSection> ().shieldsCurrent;

			//scale the blank sections to 0
				shields4Slider.fillRect.localScale = Vector3.zero;
				shields5Slider.fillRect.localScale = Vector3.zero;
				shields6Slider.fillRect.localScale = Vector3.zero;

			//current shields
			//scale the known sections to 1
				shields1Value.rectTransform.localScale = Vector3.one;
				shields2Value.rectTransform.localScale = Vector3.one;
				shields3Value.rectTransform.localScale = Vector3.one;


			//assign the known current shields
				shields1Value.text = (highlightedUnit.GetComponent<EngineSection> ().shieldsCurrent.ToString ());
				shields2Value.text = (highlightedUnit.GetComponent<PhaserSection> ().shieldsCurrent.ToString ());
				shields3Value.text = (highlightedUnit.GetComponent<TorpedoSection> ().shieldsCurrent.ToString ());

			//scale the blank sections to 0
				shields4Value.rectTransform.localScale = Vector3.zero;
				shields5Value.rectTransform.localScale = Vector3.zero;
				shields6Value.rectTransform.localScale = Vector3.zero;

			//color the shield bars
				SetShieldBarColor (shields1Fill, shields1Slider.value, shields1Slider.maxValue);
				SetShieldBarColor (shields2Fill, shields2Slider.value, shields2Slider.maxValue);
				SetShieldBarColor (shields3Fill, shields3Slider.value, shields3Slider.maxValue);

				//color code the section names and shield values
				if (highlightedUnit.GetComponent<EngineSection> ().isDestroyed == false) {

					shields1Section.color = displayTextColor;
					shields1Value.color = displayTextColor;

				} else {

					shields1Section.color = displayTextDestroyedSectionColor;
					shields1Value.color = displayTextDestroyedSectionColor;

				}

				if (highlightedUnit.GetComponent<PhaserSection> ().isDestroyed == false) {

					shields2Section.color = displayTextColor;
					shields2Value.color = displayTextColor;

				} else {

					shields2Section.color = displayTextDestroyedSectionColor;
					shields2Value.color = displayTextDestroyedSectionColor;

				}

				if (highlightedUnit.GetComponent<TorpedoSection> ().isDestroyed == false) {

					shields3Section.color = displayTextColor;
					shields3Value.color = displayTextColor;

				} else {

					shields3Section.color = displayTextDestroyedSectionColor;
					shields3Value.color = displayTextDestroyedSectionColor;

				}

				break;

			case CombatUnit.UnitType.Scout:

			//section Texts
			//scale the known sections
				shields1Section.rectTransform.localScale = Vector3.one;
				shields2Section.rectTransform.localScale = Vector3.one;
				shields3Section.rectTransform.localScale = Vector3.one;

			//assign the known sections
				shields1Section.text = ("Engine");
				shields2Section.text = ("Phaser");
				shields3Section.text = ("Storage");

				//update the font size if necessary
				UIManager.AutoSizeTextMeshFont (shields1Section);
				UIManager.AutoSizeTextMeshFont (shields2Section);
				UIManager.AutoSizeTextMeshFont (shields3Section);

			//scale the blank sections to 0
				shields4Section.rectTransform.localScale = Vector3.zero;
				shields5Section.rectTransform.localScale = Vector3.zero;
				shields6Section.rectTransform.localScale = Vector3.zero;


			//sliders
			//scale the known sections to 1
				shields1Slider.fillRect.localScale = Vector3.one;
				shields2Slider.fillRect.localScale = Vector3.one;
				shields3Slider.fillRect.localScale = Vector3.one;

			//set the slider range to the shield range
				shields1Slider.maxValue = (float)highlightedUnit.GetComponent<EngineSection> ().shieldsMax;
				shields2Slider.maxValue = (float)highlightedUnit.GetComponent<PhaserSection> ().shieldsMax;
				shields3Slider.maxValue = (float)highlightedUnit.GetComponent<StorageSection> ().shieldsMax;

				shields1Slider.value = (float)highlightedUnit.GetComponent<EngineSection> ().shieldsCurrent;
				shields2Slider.value = (float)highlightedUnit.GetComponent<PhaserSection> ().shieldsCurrent;
				shields3Slider.value = (float)highlightedUnit.GetComponent<StorageSection> ().shieldsCurrent;

			//scale the blank sections to 0
				shields4Slider.fillRect.localScale = Vector3.zero;
				shields5Slider.fillRect.localScale = Vector3.zero;
				shields6Slider.fillRect.localScale = Vector3.zero;

			//current shields
			//scale the known sections to 1
				shields1Value.rectTransform.localScale = Vector3.one;
				shields2Value.rectTransform.localScale = Vector3.one;
				shields3Value.rectTransform.localScale = Vector3.one;


			//assign the known current shields
				shields1Value.text = (highlightedUnit.GetComponent<EngineSection> ().shieldsCurrent.ToString ());
				shields2Value.text = (highlightedUnit.GetComponent<PhaserSection> ().shieldsCurrent.ToString ());
				shields3Value.text = (highlightedUnit.GetComponent<StorageSection> ().shieldsCurrent.ToString ());

			//scale the blank sections to 0
				shields4Value.rectTransform.localScale = Vector3.zero;
				shields5Value.rectTransform.localScale = Vector3.zero;
				shields6Value.rectTransform.localScale = Vector3.zero;

			//color the shield bars
				SetShieldBarColor (shields1Fill, shields1Slider.value, shields1Slider.maxValue);
				SetShieldBarColor (shields2Fill, shields2Slider.value, shields2Slider.maxValue);
				SetShieldBarColor (shields3Fill, shields3Slider.value, shields3Slider.maxValue);

				//color code the section names and shield values
				if (highlightedUnit.GetComponent<EngineSection> ().isDestroyed == false) {

					shields1Section.color = displayTextColor;
					shields1Value.color = displayTextColor;

				} else {

					shields1Section.color = displayTextDestroyedSectionColor;
					shields1Value.color = displayTextDestroyedSectionColor;

				}

				if (highlightedUnit.GetComponent<PhaserSection> ().isDestroyed == false) {

					shields2Section.color = displayTextColor;
					shields2Value.color = displayTextColor;

				} else {

					shields2Section.color = displayTextDestroyedSectionColor;
					shields2Value.color = displayTextDestroyedSectionColor;

				}

				if (highlightedUnit.GetComponent<StorageSection> ().isDestroyed == false) {

					shields3Section.color = displayTextColor;
					shields3Value.color = displayTextColor;

				} else {

					shields3Section.color = displayTextDestroyedSectionColor;
					shields3Value.color = displayTextDestroyedSectionColor;

				}


				break;

			case CombatUnit.UnitType.Starbase:

			//section Texts
			//scale the known sections
				shields1Section.rectTransform.localScale = Vector3.one;
				shields2Section.rectTransform.localScale = Vector3.one;
				shields3Section.rectTransform.localScale = Vector3.one;
				shields4Section.rectTransform.localScale = Vector3.one;
				shields5Section.rectTransform.localScale = Vector3.one;
				shields6Section.rectTransform.localScale = Vector3.one;

			//assign the known sections
				shields1Section.text = ("Phaser 1");
				shields2Section.text = ("Phaser 2");
				shields3Section.text = ("Torpedo");
				shields4Section.text = ("Crew");
				shields5Section.text = ("Storage 1");
				shields6Section.text = ("Storage 2");

				//update the font size if necessary
				UIManager.AutoSizeTextMeshFont (shields1Section);
				UIManager.AutoSizeTextMeshFont (shields2Section);
				UIManager.AutoSizeTextMeshFont (shields3Section);
				UIManager.AutoSizeTextMeshFont (shields4Section);
				UIManager.AutoSizeTextMeshFont (shields5Section);
				UIManager.AutoSizeTextMeshFont (shields6Section);


			//sliders
			//scale the known sections to 1
				shields1Slider.fillRect.localScale = Vector3.one;
				shields2Slider.fillRect.localScale = Vector3.one;
				shields3Slider.fillRect.localScale = Vector3.one;
				shields4Slider.fillRect.localScale = Vector3.one;
				shields5Slider.fillRect.localScale = Vector3.one;
				shields6Slider.fillRect.localScale = Vector3.one;

			//set the slider range to the shield range
				shields1Slider.maxValue = (float)highlightedUnit.GetComponent<StarbasePhaserSection1> ().shieldsMax;
				shields2Slider.maxValue = (float)highlightedUnit.GetComponent<StarbasePhaserSection2> ().shieldsMax;
				shields3Slider.maxValue = (float)highlightedUnit.GetComponent<StarbaseTorpedoSection> ().shieldsMax;
				shields4Slider.maxValue = (float)highlightedUnit.GetComponent<StarbaseCrewSection> ().shieldsMax;
				shields5Slider.maxValue = (float)highlightedUnit.GetComponent<StarbaseStorageSection1> ().shieldsMax;
				shields6Slider.maxValue = (float)highlightedUnit.GetComponent<StarbaseStorageSection2> ().shieldsMax;

				shields1Slider.value = (float)highlightedUnit.GetComponent<StarbasePhaserSection1> ().shieldsCurrent;
				shields2Slider.value = (float)highlightedUnit.GetComponent<StarbasePhaserSection2> ().shieldsCurrent;
				shields3Slider.value = (float)highlightedUnit.GetComponent<StarbaseTorpedoSection> ().shieldsCurrent;
				shields4Slider.value = (float)highlightedUnit.GetComponent<StarbaseCrewSection> ().shieldsCurrent;
				shields5Slider.value = (float)highlightedUnit.GetComponent<StarbaseStorageSection1> ().shieldsCurrent;
				shields6Slider.value = (float)highlightedUnit.GetComponent<StarbaseStorageSection2> ().shieldsCurrent;

			//current shields
			//scale the known sections to 1
				shields1Value.rectTransform.localScale = Vector3.one;
				shields2Value.rectTransform.localScale = Vector3.one;
				shields3Value.rectTransform.localScale = Vector3.one;
				shields4Value.rectTransform.localScale = Vector3.one;
				shields5Value.rectTransform.localScale = Vector3.one;
				shields6Value.rectTransform.localScale = Vector3.one;

			//assign the known current shields
				shields1Value.text = (highlightedUnit.GetComponent<StarbasePhaserSection1> ().shieldsCurrent.ToString ());
				shields2Value.text = (highlightedUnit.GetComponent<StarbasePhaserSection2> ().shieldsCurrent.ToString ());
				shields3Value.text = (highlightedUnit.GetComponent<StarbaseTorpedoSection> ().shieldsCurrent.ToString ());
				shields4Value.text = (highlightedUnit.GetComponent<StarbaseCrewSection> ().shieldsCurrent.ToString ());
				shields5Value.text = (highlightedUnit.GetComponent<StarbaseStorageSection1> ().shieldsCurrent.ToString ());
				shields6Value.text = (highlightedUnit.GetComponent<StarbaseStorageSection2> ().shieldsCurrent.ToString ());

			//color the shield bars
				SetShieldBarColor (shields1Fill, shields1Slider.value, shields1Slider.maxValue);
				SetShieldBarColor (shields2Fill, shields2Slider.value, shields2Slider.maxValue);
				SetShieldBarColor (shields3Fill, shields3Slider.value, shields3Slider.maxValue);
				SetShieldBarColor (shields4Fill, shields4Slider.value, shields4Slider.maxValue);
				SetShieldBarColor (shields5Fill, shields5Slider.value, shields5Slider.maxValue);
				SetShieldBarColor (shields6Fill, shields6Slider.value, shields6Slider.maxValue);

				//color code the section names and shield values
				if (highlightedUnit.GetComponent<StarbasePhaserSection1> ().isDestroyed == false) {

					shields1Section.color = displayTextColor;
					shields1Value.color = displayTextColor;

				} else {

					shields1Section.color = displayTextDestroyedSectionColor;
					shields1Value.color = displayTextDestroyedSectionColor;

				}

				if (highlightedUnit.GetComponent<StarbasePhaserSection2> ().isDestroyed == false) {

					shields2Section.color = displayTextColor;
					shields2Value.color = displayTextColor;

				} else {

					shields2Section.color = displayTextDestroyedSectionColor;
					shields2Value.color = displayTextDestroyedSectionColor;

				}

				if (highlightedUnit.GetComponent<StarbaseTorpedoSection> ().isDestroyed == false) {

					shields3Section.color = displayTextColor;
					shields3Value.color = displayTextColor;

				} else {

					shields3Section.color = displayTextDestroyedSectionColor;
					shields3Value.color = displayTextDestroyedSectionColor;

				}

				if (highlightedUnit.GetComponent<StarbaseCrewSection> ().isDestroyed == false) {

					shields4Section.color = displayTextColor;
					shields4Value.color = displayTextColor;

				} else {

					shields4Section.color = displayTextDestroyedSectionColor;
					shields4Value.color = displayTextDestroyedSectionColor;

				}

				if (highlightedUnit.GetComponent<StarbaseStorageSection1> ().isDestroyed == false) {

					shields5Section.color = displayTextColor;
					shields5Value.color = displayTextColor;

				} else {

					shields5Section.color = displayTextDestroyedSectionColor;
					shields5Value.color = displayTextDestroyedSectionColor;

				}

				if (highlightedUnit.GetComponent<StarbaseStorageSection2> ().isDestroyed == false) {

					shields5Section.color = displayTextColor;
					shields5Value.color = displayTextColor;

				} else {

					shields6Section.color = displayTextDestroyedSectionColor;
					shields6Value.color = displayTextDestroyedSectionColor;

				}


				break;

			default:

			//scale the blank sections to 0
				shields1Section.rectTransform.localScale = Vector3.zero;
				shields2Section.rectTransform.localScale = Vector3.zero;
				shields3Section.rectTransform.localScale = Vector3.zero;
				shields4Section.rectTransform.localScale = Vector3.zero;
				shields5Section.rectTransform.localScale = Vector3.zero;
				shields6Section.rectTransform.localScale = Vector3.zero;

			//scale the blank sections to 0
				shields1Slider.fillRect.localScale = Vector3.zero;
				shields2Slider.fillRect.localScale = Vector3.zero;
				shields3Slider.fillRect.localScale = Vector3.zero;
				shields4Slider.fillRect.localScale = Vector3.zero;
				shields5Slider.fillRect.localScale = Vector3.zero;
				shields6Slider.fillRect.localScale = Vector3.zero;

			//scale the blank sections to 0
				shields1Value.rectTransform.localScale = Vector3.zero;
				shields2Value.rectTransform.localScale = Vector3.zero;
				shields3Value.rectTransform.localScale = Vector3.zero;
				shields4Value.rectTransform.localScale = Vector3.zero;
				shields5Value.rectTransform.localScale = Vector3.zero;
				shields6Value.rectTransform.localScale = Vector3.zero;

				break;
			}

		}
		//the else condition is the highlighted unit is null
		else {

			//scale the blank sections to 0
			shields1Section.rectTransform.localScale = Vector3.zero;
			shields2Section.rectTransform.localScale = Vector3.zero;
			shields3Section.rectTransform.localScale = Vector3.zero;
			shields4Section.rectTransform.localScale = Vector3.zero;
			shields5Section.rectTransform.localScale = Vector3.zero;
			shields6Section.rectTransform.localScale = Vector3.zero;

			//scale the blank sections to 0
			shields1Slider.fillRect.localScale = Vector3.zero;
			shields2Slider.fillRect.localScale = Vector3.zero;
			shields3Slider.fillRect.localScale = Vector3.zero;
			shields4Slider.fillRect.localScale = Vector3.zero;
			shields5Slider.fillRect.localScale = Vector3.zero;
			shields6Slider.fillRect.localScale = Vector3.zero;

			//scale the blank sections to 0
			shields1Value.rectTransform.localScale = Vector3.zero;
			shields2Value.rectTransform.localScale = Vector3.zero;
			shields3Value.rectTransform.localScale = Vector3.zero;
			shields4Value.rectTransform.localScale = Vector3.zero;
			shields5Value.rectTransform.localScale = Vector3.zero;
			shields6Value.rectTransform.localScale = Vector3.zero;

		}

	}


	//this function will set the unit text display for the highlighted unit
	private void SetUnitTextDisplay(){

		//check if the highlighted unit is null
		if (highlightedUnit != null) {

			//first, we need to check what kind of unit we have
			switch (highlightedUnit.unitType) {

			case CombatUnit.UnitType.Starship:

				highlightedUnitPanelTitle.rectTransform.localScale = Vector3.one;
				highlightedUnitPanelTitle.text = (highlightedUnit.GetComponent<Ship> ().shipType + " " + highlightedUnit.GetComponent<Ship> ().shipName);

				//update the font size if necessary
				UIManager.AutoSizeTextMeshFont (highlightedUnitPanelTitle);

				shipName.rectTransform.localScale = Vector3.one;
				shipName.rectTransform.localPosition = shipNameTextLocation;
				shipName.text = (highlightedUnit.GetComponent<Ship> ().shipName);

				//update the font size if necessary
				UIManager.AutoSizeTextMeshFont (shipName);

				baseNameBackground.rectTransform.localScale = Vector3.zero;

				torpedoText.rectTransform.localScale = Vector3.one;
				torpedoText.rectTransform.localPosition = shipTorpedoTextLocation;

				//check attack status
				if (highlightedUnit.hasRemainingTorpedoAttack == true) {

					torpedoText.text = ("T");

				}
				else if( highlightedUnit.hasRemainingTorpedoAttack == false) {

					torpedoText.text = ("");

				}

				phaserText.rectTransform.localScale = Vector3.one;
				phaserText.rectTransform.localPosition = shipPhaserTextLocation;

				//check attack status
				if (highlightedUnit.hasRemainingPhaserAttack == true) {

					phaserText.text = ("P");

				}
				else if( highlightedUnit.hasRemainingPhaserAttack == false) {

					phaserText.text = ("");

				}

				movementText.rectTransform.localScale = Vector3.one;
				movementText.text = (highlightedUnit.GetComponent<EngineSection> ().CurrentMovementRange.ToString ());
				movementText.rectTransform.localPosition = starshipMoveTextLocation;


				specialText.rectTransform.localScale = Vector3.one;
				specialText.rectTransform.localPosition = starshipRepairTextLocation;

				//check repair status
				if (highlightedUnit.GetComponent<Starship>().hasRemainingRepairAction == true) {

					specialText.text = ("R");

				}
				else if( highlightedUnit.GetComponent<Starship>().hasRemainingRepairAction == false) {

					specialText.text = ("");

				}


				break;

			case CombatUnit.UnitType.Destroyer:

				highlightedUnitPanelTitle.rectTransform.localScale = Vector3.one;
				highlightedUnitPanelTitle.text = (highlightedUnit.GetComponent<Ship> ().shipType + " " + highlightedUnit.GetComponent<Ship> ().shipName);

				//update the font size if necessary
				UIManager.AutoSizeTextMeshFont (highlightedUnitPanelTitle);

				shipName.rectTransform.localScale = Vector3.one;
				shipName.rectTransform.localPosition = shipNameTextLocation;
				shipName.text = (highlightedUnit.GetComponent<Ship> ().shipName);

				//update the font size if necessary
				UIManager.AutoSizeTextMeshFont (shipName);

				baseNameBackground.rectTransform.localScale = Vector3.zero;

				torpedoText.rectTransform.localScale = Vector3.one;
				torpedoText.rectTransform.localPosition = shipTorpedoTextLocation;

				//check attack status
				if (highlightedUnit.hasRemainingTorpedoAttack == true) {

					torpedoText.text = ("T");

				}
				else if( highlightedUnit.hasRemainingTorpedoAttack == false) {

					torpedoText.text = ("");

				}

				phaserText.rectTransform.localScale = Vector3.one;
				phaserText.rectTransform.localPosition = shipPhaserTextLocation;

				//check attack status
				if (highlightedUnit.hasRemainingPhaserAttack == true) {

					phaserText.text = ("P");

				}
				else if( highlightedUnit.hasRemainingPhaserAttack == false) {

					phaserText.text = ("");

				}

				movementText.rectTransform.localScale = Vector3.one;
				movementText.text = (highlightedUnit.GetComponent<EngineSection> ().CurrentMovementRange.ToString ());
				movementText.rectTransform.localPosition = destroyerMoveTextLocation;

				specialText.rectTransform.localScale = Vector3.zero;
				break;

			case CombatUnit.UnitType.BirdOfPrey:

				highlightedUnitPanelTitle.rectTransform.localScale = Vector3.one;
				highlightedUnitPanelTitle.text = (highlightedUnit.GetComponent<Ship> ().shipType + " " + highlightedUnit.GetComponent<Ship> ().shipName);

				//update the font size if necessary
				UIManager.AutoSizeTextMeshFont (highlightedUnitPanelTitle);

				shipName.rectTransform.localScale = Vector3.one;
				shipName.rectTransform.localPosition = shipNameTextLocation;
				shipName.text = (highlightedUnit.GetComponent<Ship> ().shipName);

				//update the font size if necessary
				UIManager.AutoSizeTextMeshFont (shipName);

				baseNameBackground.rectTransform.localScale = Vector3.zero;

				torpedoText.rectTransform.localScale = Vector3.one;
				torpedoText.rectTransform.localPosition = shipTorpedoTextLocation;

				//check attack status
				if (highlightedUnit.hasRemainingTorpedoAttack == true) {

					torpedoText.text = ("T");

				}
				else if( highlightedUnit.hasRemainingTorpedoAttack == false) {

					torpedoText.text = ("");

				}

				phaserText.rectTransform.localScale = Vector3.one;
				phaserText.rectTransform.localPosition = shipPhaserTextLocation;

				//check attack status
				if (highlightedUnit.hasRemainingPhaserAttack == true) {

					phaserText.text = ("P");

				}
				else if( highlightedUnit.hasRemainingPhaserAttack == false) {

					phaserText.text = ("");

				}

				movementText.rectTransform.localScale = Vector3.one;
				movementText.text = (highlightedUnit.GetComponent<EngineSection> ().CurrentMovementRange.ToString ());
				movementText.rectTransform.localPosition = birdOfPreyMoveTextLocation;


				specialText.rectTransform.localScale = Vector3.one;
				specialText.rectTransform.localPosition = birdOfPreyCloakTextLocation;

				//check cloaking status
				if (highlightedUnit.GetComponent<BirdOfPrey>().hasRemainingCloakAction == true) {

					specialText.text = ("C");

				}
				else if( highlightedUnit.GetComponent<BirdOfPrey>().hasRemainingCloakAction == false) {

					specialText.text = ("");

				}
				break;

			case CombatUnit.UnitType.Scout:

				highlightedUnitPanelTitle.rectTransform.localScale = Vector3.one;
				highlightedUnitPanelTitle.text = (highlightedUnit.GetComponent<Ship> ().shipType + " " + highlightedUnit.GetComponent<Ship> ().shipName);

				//update the font size if necessary
				UIManager.AutoSizeTextMeshFont (highlightedUnitPanelTitle);

				baseNameBackground.rectTransform.localScale = Vector3.zero;

				shipName.rectTransform.localScale = Vector3.one;
				shipName.rectTransform.localPosition = shipNameTextLocation;
				shipName.text = (highlightedUnit.GetComponent<Ship> ().shipName);

				//update the font size if necessary
				UIManager.AutoSizeTextMeshFont (shipName);

				torpedoText.rectTransform.localScale = Vector3.zero;

				phaserText.rectTransform.localScale = Vector3.one;
				phaserText.rectTransform.localPosition = shipPhaserTextLocation;

				//check attack status
				if (highlightedUnit.hasRemainingPhaserAttack == true) {

					phaserText.text = ("P");

				}
				else if( highlightedUnit.hasRemainingPhaserAttack == false) {

					phaserText.text = ("");

				}

				movementText.rectTransform.localScale = Vector3.one;
				movementText.text = (highlightedUnit.GetComponent<EngineSection> ().CurrentMovementRange.ToString ());
				movementText.rectTransform.localPosition = scoutMoveTextLocation;

				specialText.rectTransform.localScale = Vector3.zero;
				break;

			case CombatUnit.UnitType.Starbase:

				highlightedUnitPanelTitle.rectTransform.localScale = Vector3.one;
				highlightedUnitPanelTitle.text =  ("Starbase " + highlightedUnit.GetComponent<Starbase> ().baseName);

				//update the font size if necessary
				UIManager.AutoSizeTextMeshFont (highlightedUnitPanelTitle);

				shipName.rectTransform.localScale = Vector3.one;
				shipName.rectTransform.localPosition = baseNameTextLocation;
				shipName.text = (highlightedUnit.GetComponent<Starbase> ().baseName);

				//update the font size if necessary
				UIManager.AutoSizeTextMeshFont (shipName);

				baseNameBackground.rectTransform.localScale = Vector3.one;

				torpedoText.rectTransform.localScale = Vector3.one;
				torpedoText.rectTransform.localPosition = baseTorpedoTextLocation;

				//check attack status
				if (highlightedUnit.hasRemainingTorpedoAttack == true) {

					torpedoText.text = ("T");

				}
				else if( highlightedUnit.hasRemainingTorpedoAttack == false) {

					torpedoText.text = ("");

				}

				phaserText.rectTransform.localScale = Vector3.one;
				phaserText.rectTransform.localPosition = basePhaserTextLocation;

				//check attack status
				if (highlightedUnit.hasRemainingPhaserAttack == true) {

					phaserText.text = ("P");

				}
				else if( highlightedUnit.hasRemainingPhaserAttack == false) {

					phaserText.text = ("");

				}


				movementText.rectTransform.localScale = Vector3.zero;

				specialText.rectTransform.localScale = Vector3.one;
				specialText.rectTransform.localPosition = starbaseRepairTextLocation;

				//check repair status
				if (highlightedUnit.GetComponent<Starbase>().hasRemainingRepairAction == true) {

					specialText.text = ("R");

				}
				else if( highlightedUnit.GetComponent<Starbase>().hasRemainingRepairAction == false) {

					specialText.text = ("");

				}

				break;

			default:

				highlightedUnitPanelTitle.rectTransform.localScale = Vector3.zero;

				shipName.rectTransform.localScale = Vector3.zero;

				torpedoText.rectTransform.localScale = Vector3.zero;

				phaserText.rectTransform.localScale = Vector3.zero;

				movementText.rectTransform.localScale = Vector3.zero;

				specialText.rectTransform.localScale = Vector3.zero;

				baseNameBackground.rectTransform.localScale = Vector3.zero;


				break;

			}

		}
		//the else condition is that the highlighted unit is null
		else {

			highlightedUnitPanelTitle.rectTransform.localScale = Vector3.zero;

			shipName.rectTransform.localScale = Vector3.zero;

			torpedoText.rectTransform.localScale = Vector3.zero;

			phaserText.rectTransform.localScale = Vector3.zero;

			movementText.rectTransform.localScale = Vector3.zero;

			specialText.rectTransform.localScale = Vector3.zero;

			baseNameBackground.rectTransform.localScale = Vector3.zero;

		}

	}

	//this function will set the color of the shield bar fill based on the shield level
	private void SetShieldBarColor(Image shieldBarImage, float shieldsCurrent, float shieldsMax){

		//check if shields are above the greenThreshold
		if (shieldsCurrent / shieldsMax >= greenThreshold) {
			
			shieldBarImage.color = new Color (0f, 200f / 255f, 0f);
		}
		//else check if shields are above the yellow threshold
		else if (shieldsCurrent / shieldsMax >= yellowThreshold) {

			shieldBarImage.color = new Color (1f, 1f, 0f);

		}
		//the else condition is we are below the yellow threshold, so color red
		else {
			
			shieldBarImage.color = new Color (1f, 0f, 0f);

		}


	}


	//this function will set the border color for the highlighted unit panel
	private void SetUnitPanelBorder(Player.Color playerColor){

		//set the border color based on the player color
		switch (playerColor) {

		case Player.Color.Green:

			unitPanel.GetComponent<Image> ().color = greenColor;
			miniMapBorder.GetComponent<Image> ().color = greenColor;
			break;

		case Player.Color.Purple:

			unitPanel.GetComponent<Image> ().color = purpleColor;
			miniMapBorder.GetComponent<Image> ().color = purpleColor;
			break;

		case Player.Color.Red:

			unitPanel.GetComponent<Image> ().color = redColor;
			miniMapBorder.GetComponent<Image> ().color = redColor;
			break;

		case Player.Color.Blue:

			unitPanel.GetComponent<Image> ().color = blueColor;
			miniMapBorder.GetComponent<Image> ().color = blueColor;
			break;

		default:

			unitPanel.GetComponent<Image>().color = Color.white;
			miniMapBorder.GetComponent<Image> ().color = Color.white;
			break;
		
		}

	}

	//this function will set the color of the title on the highlighted panel 
	private void SetTitleTextColor(){

		//check if highlighted unit is null
		if (highlightedUnit != null) {

			switch (highlightedUnit.owner.color) {

			case Player.Color.Blue:

				highlightedUnitPanelTitle.color = blueColor;
				break;

			case Player.Color.Green:

				highlightedUnitPanelTitle.color = greenColor;
				break;

			case Player.Color.Red:

				highlightedUnitPanelTitle.color = redColor;
				break;

			case Player.Color.Purple:

				highlightedUnitPanelTitle.color = purpleColor;
				break;

			default:
				highlightedUnitPanelTitle.color = Color.white;
				break;

			}

		}

	}

	//this function controls setting which sections to display
	private void SetSectionDislplays(){

		//first check if the highlighted unit is null
		if (highlightedUnit == null) {

			//hide all the section displays
			shipPhaserSection.SetActive (false);
			shipTorpedoSection.SetActive (false);
			shipStorageSection.SetActive (false);
			shipCrewSection.SetActive (false);
			shipEngineSection.SetActive (false);

			basePhaserSection1.SetActive (false);
			basePhaserSection2.SetActive (false);
			baseTorpedoSection.SetActive (false);
			baseCrewSection.SetActive (false);
			baseStorageSection1.SetActive (false);
			baseStorageSection2.SetActive (false);

		} else if (highlightedUnit.GetComponent<Ship> () == true) {

			//the else condition is that the highlighted unit is a ship

			//deactivate the base stuff
			basePhaserSection1.SetActive (false);
			basePhaserSection2.SetActive (false);
			baseTorpedoSection.SetActive (false);
			baseCrewSection.SetActive (false);
			baseStorageSection1.SetActive (false);
			baseStorageSection2.SetActive (false);

			//check if the ship has a phaser section
			if (highlightedUnit.GetComponent<PhaserSection> () == true) {

				//activate the phaser section
				shipPhaserSection.SetActive (true);

				//update the phaser section
				UpdateShipPhaserSection (highlightedUnit);

			} else {

				shipPhaserSection.SetActive (false);

			}

			//check if the ship has a torpedo section
			if (highlightedUnit.GetComponent<TorpedoSection> () == true) {

				//activate the torpedo section
				shipTorpedoSection.SetActive (true);

				//update the torpedo section
				UpdateShipTorpedoSection (highlightedUnit);

			} else {

				shipTorpedoSection.SetActive (false);

			}

			//check if the ship has a storage section
			if (highlightedUnit.GetComponent<StorageSection> () == true) {

				//activate the storage section
				shipStorageSection.SetActive (true);

				//update the storage section
				UpdateShipStorageSection (highlightedUnit);

			} else {

				shipStorageSection.SetActive (false);

			}

			//check if the ship has a crew section
			if (highlightedUnit.GetComponent<CrewSection> () == true) {

				//activate the crew section
				shipCrewSection.SetActive (true);

				//update the crew section
				UpdateShipCrewSection (highlightedUnit);

			} else {

				shipCrewSection.SetActive (false);

			}

			//check if the ship has a engine section
			if (highlightedUnit.GetComponent<EngineSection> () == true) {

				//activate the engine section
				shipEngineSection.SetActive (true);

				//update the engine section
				UpdateShipEngineSection (highlightedUnit);

			} else {

				shipEngineSection.SetActive (false);

			}

		} else if (highlightedUnit.GetComponent<Starbase> () == true) {

			//the else condition is that the highlighted unit is a starbase

			//hide all the ship section displays
			shipPhaserSection.SetActive (false);
			shipTorpedoSection.SetActive (false);
			shipStorageSection.SetActive (false);
			shipCrewSection.SetActive (false);
			shipEngineSection.SetActive (false);

			//check if the base has a phaser 1 section
			if (highlightedUnit.GetComponent<StarbasePhaserSection1> () == true) {

				//activate the phaser 1 section
				basePhaserSection1.SetActive (true);

				//update the phaser 1 section
				UpdateBasePhaserSection1 (highlightedUnit);

			} else {

				basePhaserSection1.SetActive (false);

			}

			//check if the base has a phaser 2 section
			if (highlightedUnit.GetComponent<StarbasePhaserSection2> () == true) {

				//activate the phaser 2 section
				basePhaserSection2.SetActive (true);

				//update the phaser 2 section
				UpdateBasePhaserSection2 (highlightedUnit);

			} else {

				basePhaserSection2.SetActive (false);

			}

			//check if the base has a torpedo section
			if (highlightedUnit.GetComponent<StarbaseTorpedoSection> () == true) {

				//activate the torpedo section
				baseTorpedoSection.SetActive (true);

				//update the torpedo section
				UpdateBaseTorpedoSection (highlightedUnit);

			} else {

				baseTorpedoSection.SetActive (false);

			}

			//check if the base has a crew section
			if (highlightedUnit.GetComponent<StarbaseCrewSection> () == true) {

				//activate the crew section
				baseCrewSection.SetActive (true);

				//update the crew section
				UpdateBaseCrewSection (highlightedUnit);

			} else {

				baseCrewSection.SetActive (false);

			}

			//check if the base has a storage 1 section
			if (highlightedUnit.GetComponent<StarbaseStorageSection1> () == true) {

				//activate the storage 1 section
				baseStorageSection1.SetActive (true);

				//update the storage 1 section
				UpdateBaseStorageSection1 (highlightedUnit);

			} else {

				baseStorageSection1.SetActive (false);

			}

			//check if the base has a storage 2 section
			if (highlightedUnit.GetComponent<StarbaseStorageSection2> () == true) {

				//activate the storage 2 section
				baseStorageSection2.SetActive (true);

				//update the storage 2 section
				UpdateBaseStorageSection2 (highlightedUnit);

			} else {

				baseStorageSection2.SetActive (false);

			}

		}

	}

	//this function updates the phaser section values
	private void UpdateShipPhaserSection(CombatUnit combatUnit){

		//check if the section is destroyed
		if (combatUnit.GetComponent<PhaserSection> ().isDestroyed == false) {

			//get all the textmeshpro objects in the section
			foreach (TextMeshProUGUI tmpro in shipPhaserSection.GetComponentsInChildren<TextMeshProUGUI>()) {

				tmpro.color = displayTextColor;

			}

			//get all images in the section
			foreach (Image image in shipPhaserSection.GetComponentsInChildren<Image>()) {

				image.color = displayTextColor;

			}

		} else {
			
			//get all the textmeshpro objects in the section
			foreach (TextMeshProUGUI tmpro in shipPhaserSection.GetComponentsInChildren<TextMeshProUGUI>()) {

				tmpro.color = displayTextDestroyedSectionColor;

			}

			//get all images in the section
			foreach (Image image in shipPhaserSection.GetComponentsInChildren<Image>()) {

				image.color = displayTextDestroyedSectionColor;

			}

		}

		//update the inventory values
		shipPhaserRadarShotText.text = (combatUnit.GetComponent<PhaserSection>().phaserRadarShot.ToString());

		//update the font size if necessary
		UIManager.AutoSizeTextMeshFont (shipPhaserRadarShotText);

		if (combatUnit.GetComponent<PhaserSection> ().phaserRadarArray == true) {

			shipPhaserRadarArrayText.text = ("1");

		} else {

			shipPhaserRadarArrayText.text = ("0");

		}

		if (combatUnit.GetComponent<PhaserSection> ().xRayKernalUpgrade == true) {

			shipXRayKernelText.text = ("1");

		} else {

			shipXRayKernelText.text = ("0");

		}

		if (combatUnit.GetComponent<PhaserSection> ().tractorBeam == true) {

			shipTractorBeamText.text = ("1");

		} else {

			shipTractorBeamText.text = ("0");

		}

	}

	//this function updates the torpedo section values
	private void UpdateShipTorpedoSection(CombatUnit combatUnit){

		//check if the section is destroyed
		if (combatUnit.GetComponent<TorpedoSection> ().isDestroyed == false) {

			//get all the textmeshpro objects in the section
			foreach (TextMeshProUGUI tmpro in shipTorpedoSection.GetComponentsInChildren<TextMeshProUGUI>()) {

				tmpro.color = displayTextColor;

			}

			//get all images in the section
			foreach (Image image in shipTorpedoSection.GetComponentsInChildren<Image>()) {

				image.color = displayTextColor;

			}

		} else {

			//get all the textmeshpro objects in the section
			foreach (TextMeshProUGUI tmpro in shipTorpedoSection.GetComponentsInChildren<TextMeshProUGUI>()) {

				tmpro.color = displayTextDestroyedSectionColor;

			}

			//get all images in the section
			foreach (Image image in shipTorpedoSection.GetComponentsInChildren<Image>()) {

				image.color = displayTextDestroyedSectionColor;

			}

		}

		//update the inventory values
		shipLightTorpedoText.text = (combatUnit.GetComponent<TorpedoSection>().lightTorpedos.ToString());
		shipHeavyTorpedoText.text = (combatUnit.GetComponent<TorpedoSection>().heavyTorpedos.ToString());
		shipTorpedoLaserShotText.text = (combatUnit.GetComponent<TorpedoSection>().torpedoLaserShot.ToString());

		//update the font size if necessary
		UIManager.AutoSizeTextMeshFont (shipLightTorpedoText);
		UIManager.AutoSizeTextMeshFont (shipHeavyTorpedoText);
		UIManager.AutoSizeTextMeshFont (shipTorpedoLaserShotText);


		if (combatUnit.GetComponent<TorpedoSection> ().torpedoLaserGuidanceSystem == true) {

			shipTorpedoLaserGuidanceText.text = ("1");

		} else {

			shipTorpedoLaserGuidanceText.text = ("0");

		}

		if (combatUnit.GetComponent<TorpedoSection> ().highPressureTubes == true) {

			shipHighPressureTubesText.text = ("1");

		} else {

			shipHighPressureTubesText.text = ("0");

		}

	}

	//this function updates the storage section values
	private void UpdateShipStorageSection(CombatUnit combatUnit){

		//check if the section is destroyed
		if (combatUnit.GetComponent<StorageSection> ().isDestroyed == false) {

			//get all the textmeshpro objects in the section
			foreach (TextMeshProUGUI tmpro in shipStorageSection.GetComponentsInChildren<TextMeshProUGUI>()) {

				tmpro.color = displayTextColor;

			}

			//get all images in the section
			foreach (Image image in shipStorageSection.GetComponentsInChildren<Image>()) {

				image.color = displayTextColor;

			}

		} else {

			//get all the textmeshpro objects in the section
			foreach (TextMeshProUGUI tmpro in shipStorageSection.GetComponentsInChildren<TextMeshProUGUI>()) {

				tmpro.color = displayTextDestroyedSectionColor;

			}

			//get all images in the section
			foreach (Image image in shipStorageSection.GetComponentsInChildren<Image>()) {

				image.color = displayTextDestroyedSectionColor;

			}

		}

		//update the inventory values
		shipDilithiumCrystalText.text = (combatUnit.GetComponent<StorageSection>().dilithiumCrystals.ToString());
		shipTrilithiumCrystalText.text = (combatUnit.GetComponent<StorageSection>().trilithiumCrystals.ToString());
		shipFlareText.text = (combatUnit.GetComponent<StorageSection>().flares.ToString());

		//update the font size if necessary
		UIManager.AutoSizeTextMeshFont (shipDilithiumCrystalText);
		UIManager.AutoSizeTextMeshFont (shipTrilithiumCrystalText);
		UIManager.AutoSizeTextMeshFont (shipFlareText);

		if (combatUnit.GetComponent<StorageSection> ().radarJammingSystem == true) {

			shipRadarJammingText.text = ("1");

		} else {

			shipRadarJammingText.text = ("0");

		}

		if (combatUnit.GetComponent<StorageSection> ().laserScatteringSystem == true) {

			shipLaserScatteringText.text = ("1");

		} else {

			shipLaserScatteringText.text = ("0");

		}

	}

	//this function updates the crew section values
	private void UpdateShipCrewSection(CombatUnit combatUnit){

		//check if the section is destroyed
		if (combatUnit.GetComponent<CrewSection> ().isDestroyed == false) {

			//get all the textmeshpro objects in the section
			foreach (TextMeshProUGUI tmpro in shipCrewSection.GetComponentsInChildren<TextMeshProUGUI>()) {

				tmpro.color = displayTextColor;

			}

			//get all images in the section
			foreach (Image image in shipCrewSection.GetComponentsInChildren<Image>()) {

				image.color = displayTextColor;

			}

		} else {

			//get all the textmeshpro objects in the section
			foreach (TextMeshProUGUI tmpro in shipCrewSection.GetComponentsInChildren<TextMeshProUGUI>()) {

				tmpro.color = displayTextDestroyedSectionColor;

			}

			//get all images in the section
			foreach (Image image in shipCrewSection.GetComponentsInChildren<Image>()) {

				image.color = displayTextDestroyedSectionColor;

			}

		}


		//update the inventory values
		if (combatUnit.GetComponent<CrewSection> ().repairCrew == true) {

			shipRepairCrewText.text = ("1");

		} else {

			shipRepairCrewText.text = ("0");

		}

		if (combatUnit.GetComponent<CrewSection> ().shieldEngineeringTeam == true) {

			shipShieldEngineeringTeamText.text = ("1");

		} else {

			shipShieldEngineeringTeamText.text = ("0");

		}

		if (combatUnit.GetComponent<CrewSection> ().battleCrew == true) {

			shipBattleCrewText.text = ("1");

		} else {

			shipBattleCrewText.text = ("0");

		}

	}

	//this function updates the engine section values
	private void UpdateShipEngineSection(CombatUnit combatUnit){
		
		//check if the section is destroyed
		if (combatUnit.GetComponent<EngineSection> ().isDestroyed == false) {

			//get all the textmeshpro objects in the section
			foreach (TextMeshProUGUI tmpro in shipEngineSection.GetComponentsInChildren<TextMeshProUGUI>()) {

				tmpro.color = displayTextColor;

			}

			//get all images in the section
			foreach (Image image in shipEngineSection.GetComponentsInChildren<Image>()) {

				image.color = displayTextColor;

			}

		} else {

			//get all the textmeshpro objects in the section
			foreach (TextMeshProUGUI tmpro in shipEngineSection.GetComponentsInChildren<TextMeshProUGUI>()) {

				tmpro.color = displayTextDestroyedSectionColor;

			}

			//get all images in the section
			foreach (Image image in shipEngineSection.GetComponentsInChildren<Image>()) {

				image.color = displayTextDestroyedSectionColor;

			}

		}

		//update the inventory values
		shipWarpBoosterText.text = (combatUnit.GetComponent<EngineSection>().warpBooster.ToString());
		shipTranswarpBoosterText.text = (combatUnit.GetComponent<EngineSection>().transwarpBooster.ToString());

		//update the font size if necessary
		UIManager.AutoSizeTextMeshFont (shipWarpBoosterText);
		UIManager.AutoSizeTextMeshFont (shipTranswarpBoosterText);

		if (combatUnit.GetComponent<EngineSection> ().warpDrive == true) {

			shipWarpDriveText.text = ("1");

		} else {

			shipWarpDriveText.text = ("0");

		}

		if (combatUnit.GetComponent<EngineSection> ().transwarpDrive == true) {

			shipTranswarpDriveText.text = ("1");

		} else {

			shipTranswarpDriveText.text = ("0");

		}

	}

	//this function updates the base phaser section values
	private void UpdateBasePhaserSection1(CombatUnit combatUnit){

		//check if the section is destroyed
		if (combatUnit.GetComponent<StarbasePhaserSection1> ().isDestroyed == false) {

			//get all the textmeshpro objects in the section
			foreach (TextMeshProUGUI tmpro in basePhaserSection1.GetComponentsInChildren<TextMeshProUGUI>()) {

				tmpro.color = displayTextColor;

			}

			//get all images in the section
			foreach (Image image in basePhaserSection1.GetComponentsInChildren<Image>()) {

				image.color = displayTextColor;

			}

		} else {

			//get all the textmeshpro objects in the section
			foreach (TextMeshProUGUI tmpro in basePhaserSection1.GetComponentsInChildren<TextMeshProUGUI>()) {

				tmpro.color = displayTextDestroyedSectionColor;

			}

			//get all images in the section
			foreach (Image image in basePhaserSection1.GetComponentsInChildren<Image>()) {

				image.color = displayTextDestroyedSectionColor;

			}

		}

		//update the inventory values
		basePhaserRadarShotText.text = (combatUnit.GetComponent<StarbasePhaserSection1>().phaserRadarShot.ToString());

		//update the font size if necessary
		UIManager.AutoSizeTextMeshFont (basePhaserRadarShotText);

		if (combatUnit.GetComponent<StarbasePhaserSection1> ().phaserRadarArray == true) {

			basePhaserRadarArrayText.text = ("1");

		} else {

			basePhaserRadarArrayText.text = ("0");

		}

	}

	//this function updates the base phaser section values
	private void UpdateBasePhaserSection2(CombatUnit combatUnit){

		//check if the section is destroyed
		if (combatUnit.GetComponent<StarbasePhaserSection2> ().isDestroyed == false) {

			//get all the textmeshpro objects in the section
			foreach (TextMeshProUGUI tmpro in basePhaserSection2.GetComponentsInChildren<TextMeshProUGUI>()) {

				tmpro.color = displayTextColor;

			}

			//get all images in the section
			foreach (Image image in basePhaserSection2.GetComponentsInChildren<Image>()) {

				image.color = displayTextColor;

			}

		} else {

			//get all the textmeshpro objects in the section
			foreach (TextMeshProUGUI tmpro in basePhaserSection2.GetComponentsInChildren<TextMeshProUGUI>()) {

				tmpro.color = displayTextDestroyedSectionColor;

			}

			//get all images in the section
			foreach (Image image in basePhaserSection2.GetComponentsInChildren<Image>()) {

				image.color = displayTextDestroyedSectionColor;

			}

		}

		//update the inventory values
		if (combatUnit.GetComponent<StarbasePhaserSection2> ().xRayKernalUpgrade == true) {

			baseXRayKernelText.text = ("1");

		} else {

			baseXRayKernelText.text = ("0");

		}

	}

	//this function updates the torpedo section values
	private void UpdateBaseTorpedoSection(CombatUnit combatUnit){

		//check if the section is destroyed
		if (combatUnit.GetComponent<StarbaseTorpedoSection> ().isDestroyed == false) {

			//get all the textmeshpro objects in the section
			foreach (TextMeshProUGUI tmpro in baseTorpedoSection.GetComponentsInChildren<TextMeshProUGUI>()) {

				tmpro.color = displayTextColor;

			}

			//get all images in the section
			foreach (Image image in baseTorpedoSection.GetComponentsInChildren<Image>()) {

				image.color = displayTextColor;

			}

		} else {

			//get all the textmeshpro objects in the section
			foreach (TextMeshProUGUI tmpro in baseTorpedoSection.GetComponentsInChildren<TextMeshProUGUI>()) {

				tmpro.color = displayTextDestroyedSectionColor;

			}

			//get all images in the section
			foreach (Image image in baseTorpedoSection.GetComponentsInChildren<Image>()) {

				image.color = displayTextDestroyedSectionColor;

			}

		}

		//update the inventory values
		baseLightTorpedoText.text = (combatUnit.GetComponent<StarbaseTorpedoSection>().lightTorpedos.ToString());
		baseHeavyTorpedoText.text = (combatUnit.GetComponent<StarbaseTorpedoSection>().heavyTorpedos.ToString());
		baseTorpedoLaserShotText.text = (combatUnit.GetComponent<StarbaseTorpedoSection>().torpedoLaserShot.ToString());

		//update the font size if necessary
		UIManager.AutoSizeTextMeshFont (baseLightTorpedoText);
		UIManager.AutoSizeTextMeshFont (baseHeavyTorpedoText);
		UIManager.AutoSizeTextMeshFont (baseTorpedoLaserShotText);


		if (combatUnit.GetComponent<StarbaseTorpedoSection> ().torpedoLaserGuidanceSystem == true) {

			baseTorpedoLaserGuidanceText.text = ("1");

		} else {

			baseTorpedoLaserGuidanceText.text = ("0");

		}

		if (combatUnit.GetComponent<StarbaseTorpedoSection> ().highPressureTubes == true) {

			baseHighPressureTubesText.text = ("1");

		} else {

			baseHighPressureTubesText.text = ("0");

		}

	}

	//this function updates the crew section values
	private void UpdateBaseCrewSection(CombatUnit combatUnit){

		//check if the section is destroyed
		if (combatUnit.GetComponent<StarbaseCrewSection> ().isDestroyed == false) {

			//get all the textmeshpro objects in the section
			foreach (TextMeshProUGUI tmpro in baseCrewSection.GetComponentsInChildren<TextMeshProUGUI>()) {

				tmpro.color = displayTextColor;

			}

			//get all images in the section
			foreach (Image image in baseCrewSection.GetComponentsInChildren<Image>()) {

				image.color = displayTextColor;

			}

		} else {

			//get all the textmeshpro objects in the section
			foreach (TextMeshProUGUI tmpro in baseCrewSection.GetComponentsInChildren<TextMeshProUGUI>()) {

				tmpro.color = displayTextDestroyedSectionColor;

			}

			//get all images in the section
			foreach (Image image in baseCrewSection.GetComponentsInChildren<Image>()) {

				image.color = displayTextDestroyedSectionColor;

			}

		}

		//update the inventory values
		if (combatUnit.GetComponent<StarbaseCrewSection> ().repairCrew == true) {

			baseRepairCrewText.text = ("1");

		} else {

			baseRepairCrewText.text = ("0");

		}

		if (combatUnit.GetComponent<StarbaseCrewSection> ().shieldEngineeringTeam == true) {

			baseShieldEngineeringTeamText.text = ("1");

		} else {

			baseShieldEngineeringTeamText.text = ("0");

		}

		if (combatUnit.GetComponent<StarbaseCrewSection> ().battleCrew == true) {

			baseBattleCrewText.text = ("1");

		} else {

			baseBattleCrewText.text = ("0");

		}

	}

	//this function updates the storage section values
	private void UpdateBaseStorageSection1(CombatUnit combatUnit){

		//check if the section is destroyed
		if (combatUnit.GetComponent<StarbaseStorageSection1> ().isDestroyed == false) {

			//get all the textmeshpro objects in the section
			foreach (TextMeshProUGUI tmpro in baseStorageSection1.GetComponentsInChildren<TextMeshProUGUI>()) {

				tmpro.color = displayTextColor;

			}

			//get all images in the section
			foreach (Image image in baseStorageSection1.GetComponentsInChildren<Image>()) {

				image.color = displayTextColor;

			}

		} else {

			//get all the textmeshpro objects in the section
			foreach (TextMeshProUGUI tmpro in baseStorageSection1.GetComponentsInChildren<TextMeshProUGUI>()) {

				tmpro.color = displayTextDestroyedSectionColor;

			}

			//get all images in the section
			foreach (Image image in baseStorageSection1.GetComponentsInChildren<Image>()) {

				image.color = displayTextDestroyedSectionColor;

			}

		}

		//update the inventory values
		baseDilithiumCrystalText.text = (combatUnit.GetComponent<StarbaseStorageSection1>().dilithiumCrystals.ToString());

		//update the font size if necessary
		UIManager.AutoSizeTextMeshFont (baseDilithiumCrystalText);

		if (combatUnit.GetComponent<StarbaseStorageSection1> ().radarJammingSystem == true) {

			baseRadarJammingText.text = ("1");

		} else {

			baseRadarJammingText.text = ("0");

		}

	}

	//this function updates the storage section values
	private void UpdateBaseStorageSection2(CombatUnit combatUnit){

		//check if the section is destroyed
		if (combatUnit.GetComponent<StarbaseStorageSection2> ().isDestroyed == false) {

			//get all the textmeshpro objects in the section
			foreach (TextMeshProUGUI tmpro in baseStorageSection2.GetComponentsInChildren<TextMeshProUGUI>()) {

				tmpro.color = displayTextColor;

			}

			//get all images in the section
			foreach (Image image in baseStorageSection2.GetComponentsInChildren<Image>()) {

				image.color = displayTextColor;

			}

		} else {

			//get all the textmeshpro objects in the section
			foreach (TextMeshProUGUI tmpro in baseStorageSection2.GetComponentsInChildren<TextMeshProUGUI>()) {

				tmpro.color = displayTextDestroyedSectionColor;

			}

			//get all images in the section
			foreach (Image image in baseStorageSection2.GetComponentsInChildren<Image>()) {

				image.color = displayTextDestroyedSectionColor;

			}

		}

		//update the inventory values
		baseTrilithiumCrystalText.text = (combatUnit.GetComponent<StarbaseStorageSection2>().trilithiumCrystals.ToString());
		baseFlareText.text = (combatUnit.GetComponent<StarbaseStorageSection2>().flares.ToString());

		//update the font size if necessary
		UIManager.AutoSizeTextMeshFont (baseTrilithiumCrystalText);
		UIManager.AutoSizeTextMeshFont (baseFlareText);

		if (combatUnit.GetComponent<StarbaseStorageSection2> ().laserScatteringSystem == true) {

			baseLaserScatteringText.text = ("1");

		} else {

			baseLaserScatteringText.text = ("0");

		}

	}

	//this function scales the unit panel raw image of the highlighted unit based on the reference resolution
	private void ScaleHighlightedUnitImage(){

		//get the reference resolution of our canvas
		Vector2 referenceResolutionV2 = uiManager.canvas.GetComponent<CanvasScaler> ().referenceResolution;

		//convert that to a width/height value
		float referenceResolutionFloat = referenceResolutionV2.x / referenceResolutionV2.y;

		//get the current resolution of our canvas
		float canvasResolutionFloat = uiManager.canvas.GetComponent<RectTransform>().rect.width / uiManager.canvas.GetComponent<RectTransform>().rect.height;

		float scaleFactor;

		//check if the current resolution is bigger than the reference resolution
		if (canvasResolutionFloat >= referenceResolutionFloat) {

			scaleFactor = referenceResolutionFloat / canvasResolutionFloat;

			//scale the raw image rect transform down 
			highlightedUnitImage.gameObject.GetComponent<RectTransform>().localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);

		} else {
			
			//the else is that the reference is bigger
			scaleFactor = canvasResolutionFloat / referenceResolutionFloat;

			//scale the raw image rect transform down 
			highlightedUnitImage.gameObject.GetComponent<RectTransform>().localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);

		}

	}

	//this function handles on destroy
	private void OnDestroy(){

		RemoveAllListeners ();

	}

	//this function removes all listeners
	private void RemoveAllListeners(){

		if (mouseManager != null) {
			
			//remove listeners for mouseManager setting targeted, selected, and hovered objects
			mouseManager.OnSetHoveredObject.RemoveListener (SetHighlightedUnit);
			mouseManager.OnClearHoveredObject.RemoveListener (SetHighlightedUnit);
			mouseManager.OnSetTargetedUnit.RemoveListener (SetHighlightedUnit);
			mouseManager.OnClearTargetedUnit.RemoveListener (SetHighlightedUnit);
			mouseManager.OnSetSelectedUnit.RemoveListener (SetHighlightedUnit);
			mouseManager.OnClearSelectedUnit.RemoveListener (SetHighlightedUnit);

		}

		//remove listeners for events
		//remove a listener for when a ship movement is completed
		EngineSection.OnMoveFromToFinish.RemoveListener (shipMoveSetHighlightedUnitAction);

		//remove listeners for warp boosters
		EngineSection.OnUseWarpBooster.RemoveListener (shipSetHighlightedUnitAction);
		EngineSection.OnUseTranswarpBooster.RemoveListener (shipSetHighlightedUnitAction);

		//remove listeners for phaser attacks that hit ships
		/*
		//these are commented out replaced by cutscene events
		CombatManager.OnPhaserAttackHitShipPhaserSection.RemoveListener (attackSetHighlightedUnitAction);
		CombatManager.OnPhaserAttackHitShipTorpedoSection.RemoveListener (attackSetHighlightedUnitAction);
		CombatManager.OnPhaserAttackHitShipStorageSection.RemoveListener (attackSetHighlightedUnitAction);
		CombatManager.OnPhaserAttackHitShipCrewSection.RemoveListener (attackSetHighlightedUnitAction);
		CombatManager.OnPhaserAttackHitShipEngineSection.RemoveListener (attackSetHighlightedUnitAction);
		CombatManager.OnPhaserAttackMissShip.RemoveListener (attackMissSetHighlightedUnitAction);
		*/

		//remove listeners for phaser attacks that hit ships
		CutsceneManager.OnPhaserHitShipPhaserSection.RemoveListener (attackSetHighlightedUnitAction);
		CutsceneManager.OnPhaserHitShipTorpedoSection.RemoveListener (attackSetHighlightedUnitAction);
		CutsceneManager.OnPhaserHitShipStorageSection.RemoveListener (attackSetHighlightedUnitAction);
		CutsceneManager.OnPhaserHitShipCrewSection.RemoveListener (attackSetHighlightedUnitAction);
		CutsceneManager.OnPhaserHitShipEngineSection.RemoveListener (attackSetHighlightedUnitAction);
		CutsceneManager.OnPhaserMissShip.RemoveListener (attackSetHighlightedUnitAction);

		//remove listeners for phaser attacks that hit bases
		/*
		//these are commented out replaced by cutscene events
		CombatManager.OnPhaserAttackHitBasePhaserSection1.RemoveListener (attackSetHighlightedUnitAction);
		CombatManager.OnPhaserAttackHitBasePhaserSection2.RemoveListener (attackSetHighlightedUnitAction);
		CombatManager.OnPhaserAttackHitBaseTorpedoSection.RemoveListener (attackSetHighlightedUnitAction);
		CombatManager.OnPhaserAttackHitBaseCrewSection.RemoveListener (attackSetHighlightedUnitAction);
		CombatManager.OnPhaserAttackHitBaseStorageSection1.RemoveListener (attackSetHighlightedUnitAction);
		CombatManager.OnPhaserAttackHitBaseStorageSection2.RemoveListener (attackSetHighlightedUnitAction);
		CombatManager.OnPhaserAttackMissBase.RemoveListener (attackMissSetHighlightedUnitAction);
		*/

		//remove listeners for phaser attacks that hit bases
		CutsceneManager.OnPhaserHitBasePhaserSection1.RemoveListener (attackSetHighlightedUnitAction);
		CutsceneManager.OnPhaserHitBasePhaserSection2.RemoveListener (attackSetHighlightedUnitAction);
		CutsceneManager.OnPhaserHitBaseTorpedoSection.RemoveListener (attackSetHighlightedUnitAction);
		CutsceneManager.OnPhaserHitBaseCrewSection.RemoveListener (attackSetHighlightedUnitAction);
		CutsceneManager.OnPhaserHitBaseStorageSection1.RemoveListener (attackSetHighlightedUnitAction);
		CutsceneManager.OnPhaserHitBaseStorageSection2.RemoveListener (attackSetHighlightedUnitAction);
		CutsceneManager.OnPhaserMissBase.RemoveListener (attackSetHighlightedUnitAction);

		//remove listeners for torpedo attacks by ships
		TorpedoSection.OnFireLightTorpedo.RemoveListener (attackFiredSetHighlightedUnitAction);
		TorpedoSection.OnFireHeavyTorpedo.RemoveListener (attackFiredSetHighlightedUnitAction);

		//remove listeners for torpedo attacks by bases
		StarbaseTorpedoSection.OnFireLightTorpedo.RemoveListener (attackFiredSetHighlightedUnitAction);
		StarbaseTorpedoSection.OnFireHeavyTorpedo.RemoveListener (attackFiredSetHighlightedUnitAction);

		//remove listeners for flare successes and failures
		/*
		//these are commented out replaced by cutscene events
		CombatManager.OnLightTorpedoAttackDefeatedByFlares.RemoveListener (attackSectionTargetedSetHighlightedUnitAction);
		CombatManager.OnHeavyTorpedoAttackDefeatedByFlares.RemoveListener (attackSectionTargetedSetHighlightedUnitAction);
		CombatManager.OnLightTorpedoAttackFlaresFailed.RemoveListener (attackSetHighlightedUnitAction);
		CombatManager.OnHeavyTorpedoAttackFlaresFailed.RemoveListener (attackSetHighlightedUnitAction);
		*/

		//remove listeners for flare successes and failures
		CutsceneManager.OnLightTorpedoFlareSuccess.RemoveListener (attackSetHighlightedUnitAction);
		CutsceneManager.OnHeavyTorpedoFlareSuccess.RemoveListener (attackSetHighlightedUnitAction);
		CutsceneManager.OnLightTorpedoFlareFailure.RemoveListener (attackSetHighlightedUnitAction);
		CutsceneManager.OnHeavyTorpedoFlareFailure.RemoveListener (attackSetHighlightedUnitAction);


		//remove listeners for ships hit by torpedo attacks
		/*
		//these are commented out replaced by cutscene events
		CombatManager.OnLightTorpedoAttackHitShipPhaserSection.RemoveListener (attackSetHighlightedUnitAction);
		CombatManager.OnHeavyTorpedoAttackHitShipPhaserSection.RemoveListener (attackSetHighlightedUnitAction);
		CombatManager.OnLightTorpedoAttackHitShipTorpedoSection.RemoveListener (attackSetHighlightedUnitAction);
		CombatManager.OnHeavyTorpedoAttackHitShipTorpedoSection.RemoveListener (attackSetHighlightedUnitAction);
		CombatManager.OnLightTorpedoAttackHitShipStorageSection.RemoveListener (attackSetHighlightedUnitAction);
		CombatManager.OnHeavyTorpedoAttackHitShipStorageSection.RemoveListener (attackSetHighlightedUnitAction);
		CombatManager.OnLightTorpedoAttackHitShipCrewSection.RemoveListener (attackSetHighlightedUnitAction);
		CombatManager.OnHeavyTorpedoAttackHitShipCrewSection.RemoveListener (attackSetHighlightedUnitAction);
		CombatManager.OnLightTorpedoAttackHitShipEngineSection.RemoveListener (attackSetHighlightedUnitAction);
		CombatManager.OnHeavyTorpedoAttackHitShipEngineSection.RemoveListener (attackSetHighlightedUnitAction);
		CombatManager.OnLightTorpedoAttackMissShip.RemoveListener (attackMissSetHighlightedUnitAction);
		CombatManager.OnHeavyTorpedoAttackMissShip.RemoveListener (attackMissSetHighlightedUnitAction);
		*/

		//remove listeners for ships hit by torpedo attacks
		CutsceneManager.OnLightTorpedoHitShipPhaserSection.RemoveListener (attackSetHighlightedUnitAction);
		CutsceneManager.OnHeavyTorpedoHitShipPhaserSection.RemoveListener (attackSetHighlightedUnitAction);
		CutsceneManager.OnLightTorpedoHitShipTorpedoSection.RemoveListener (attackSetHighlightedUnitAction);
		CutsceneManager.OnHeavyTorpedoHitShipTorpedoSection.RemoveListener (attackSetHighlightedUnitAction);
		CutsceneManager.OnLightTorpedoHitShipStorageSection.RemoveListener (attackSetHighlightedUnitAction);
		CutsceneManager.OnHeavyTorpedoHitShipStorageSection.RemoveListener (attackSetHighlightedUnitAction);
		CutsceneManager.OnLightTorpedoHitShipCrewSection.RemoveListener (attackSetHighlightedUnitAction);
		CutsceneManager.OnHeavyTorpedoHitShipCrewSection.RemoveListener (attackSetHighlightedUnitAction);
		CutsceneManager.OnLightTorpedoHitShipEngineSection.RemoveListener (attackSetHighlightedUnitAction);
		CutsceneManager.OnHeavyTorpedoHitShipEngineSection.RemoveListener (attackSetHighlightedUnitAction);
		CutsceneManager.OnLightTorpedoMissBase.RemoveListener (attackSetHighlightedUnitAction);
		CutsceneManager.OnHeavyTorpedoMissBase.RemoveListener (attackSetHighlightedUnitAction);

		//remove listeners for bases hit by torpedo attacks
		/*
		//these are commented out replaced by cutscene events
		CombatManager.OnLightTorpedoAttackHitBasePhaserSection1.RemoveListener (attackSetHighlightedUnitAction);
		CombatManager.OnHeavyTorpedoAttackHitBasePhaserSection1.RemoveListener (attackSetHighlightedUnitAction);
		CombatManager.OnLightTorpedoAttackHitBasePhaserSection2.RemoveListener (attackSetHighlightedUnitAction);
		CombatManager.OnHeavyTorpedoAttackHitBasePhaserSection2.RemoveListener (attackSetHighlightedUnitAction);
		CombatManager.OnLightTorpedoAttackHitBaseTorpedoSection.RemoveListener (attackSetHighlightedUnitAction);
		CombatManager.OnHeavyTorpedoAttackHitBaseTorpedoSection.RemoveListener (attackSetHighlightedUnitAction);
		CombatManager.OnLightTorpedoAttackHitBaseCrewSection.RemoveListener (attackSetHighlightedUnitAction);
		CombatManager.OnHeavyTorpedoAttackHitBaseCrewSection.RemoveListener (attackSetHighlightedUnitAction);
		CombatManager.OnLightTorpedoAttackHitBaseStorageSection1.RemoveListener (attackSetHighlightedUnitAction);
		CombatManager.OnHeavyTorpedoAttackHitBaseStorageSection1.RemoveListener (attackSetHighlightedUnitAction);
		CombatManager.OnLightTorpedoAttackHitBaseStorageSection2.RemoveListener (attackSetHighlightedUnitAction);
		CombatManager.OnHeavyTorpedoAttackHitBaseStorageSection2.RemoveListener (attackSetHighlightedUnitAction);
		CombatManager.OnLightTorpedoAttackMissBase.RemoveListener (attackMissSetHighlightedUnitAction);
		CombatManager.OnHeavyTorpedoAttackMissBase.RemoveListener (attackMissSetHighlightedUnitAction);
		*/

		//remove listeners for bases hit by torpedo attacks
		CutsceneManager.OnLightTorpedoHitBasePhaserSection1.RemoveListener (attackSetHighlightedUnitAction);
		CutsceneManager.OnHeavyTorpedoHitBasePhaserSection1.RemoveListener (attackSetHighlightedUnitAction);
		CutsceneManager.OnLightTorpedoHitBasePhaserSection2.RemoveListener (attackSetHighlightedUnitAction);
		CutsceneManager.OnHeavyTorpedoHitBasePhaserSection2.RemoveListener (attackSetHighlightedUnitAction);
		CutsceneManager.OnLightTorpedoHitBaseTorpedoSection.RemoveListener (attackSetHighlightedUnitAction);
		CutsceneManager.OnHeavyTorpedoHitBaseTorpedoSection.RemoveListener (attackSetHighlightedUnitAction);
		CutsceneManager.OnLightTorpedoHitBaseCrewSection.RemoveListener (attackSetHighlightedUnitAction);
		CutsceneManager.OnHeavyTorpedoHitBaseCrewSection.RemoveListener (attackSetHighlightedUnitAction);
		CutsceneManager.OnLightTorpedoHitBaseStorageSection1.RemoveListener (attackSetHighlightedUnitAction);
		CutsceneManager.OnHeavyTorpedoHitBaseStorageSection1.RemoveListener (attackSetHighlightedUnitAction);
		CutsceneManager.OnLightTorpedoHitBaseStorageSection2.RemoveListener (attackSetHighlightedUnitAction);
		CutsceneManager.OnHeavyTorpedoHitBaseStorageSection2.RemoveListener (attackSetHighlightedUnitAction);
		CutsceneManager.OnLightTorpedoMissBase.RemoveListener (attackSetHighlightedUnitAction);
		CutsceneManager.OnHeavyTorpedoMissBase.RemoveListener (attackSetHighlightedUnitAction);

		//remove listeners for using crystals to heal ships
		CombatManager.OnCrystalUsedOnShipPhaserSection.RemoveListener (crystalUsedSetHighlightedUnitAction);
		CombatManager.OnCrystalUsedOnShipTorpedoSection.RemoveListener (crystalUsedSetHighlightedUnitAction);
		CombatManager.OnCrystalUsedOnShipStorageSection.RemoveListener (crystalUsedSetHighlightedUnitAction);
		CombatManager.OnCrystalUsedOnShipCrewSection.RemoveListener (crystalUsedSetHighlightedUnitAction);
		CombatManager.OnCrystalUsedOnShipEngineSection.RemoveListener (crystalUsedSetHighlightedUnitAction);

		//remove listeners for using crystals to heal bases
		CombatManager.OnCrystalUsedOnBasePhaserSection1.RemoveListener (crystalUsedSetHighlightedUnitAction);
		CombatManager.OnCrystalUsedOnBasePhaserSection2.RemoveListener (crystalUsedSetHighlightedUnitAction);
		CombatManager.OnCrystalUsedOnBaseTorpedoSection.RemoveListener (crystalUsedSetHighlightedUnitAction);
		CombatManager.OnCrystalUsedOnBaseCrewSection.RemoveListener (crystalUsedSetHighlightedUnitAction);
		CombatManager.OnCrystalUsedOnBaseStorageSection1.RemoveListener (crystalUsedSetHighlightedUnitAction);
		CombatManager.OnCrystalUsedOnBaseStorageSection2.RemoveListener (crystalUsedSetHighlightedUnitAction);

		//remove listeners for using repair crews on ships
		CombatManager.OnRepairCrewUsedOnShipPhaserSection.RemoveListener (attackMissSetHighlightedUnitAction);
		CombatManager.OnRepairCrewUsedOnShipTorpedoSection.RemoveListener (attackMissSetHighlightedUnitAction);
		CombatManager.OnRepairCrewUsedOnShipStorageSection.RemoveListener (attackMissSetHighlightedUnitAction);
		CombatManager.OnRepairCrewUsedOnShipCrewSection.RemoveListener (attackMissSetHighlightedUnitAction);
		CombatManager.OnRepairCrewUsedOnShipEngineSection.RemoveListener (attackMissSetHighlightedUnitAction);

		//remove listeners for using repair crews on bases
		CombatManager.OnRepairCrewUsedOnBasePhaserSection1.RemoveListener (attackMissSetHighlightedUnitAction);
		CombatManager.OnRepairCrewUsedOnBasePhaserSection2.RemoveListener (attackMissSetHighlightedUnitAction);
		CombatManager.OnRepairCrewUsedOnBaseTorpedoSection.RemoveListener (attackMissSetHighlightedUnitAction);
		CombatManager.OnRepairCrewUsedOnBaseCrewSection.RemoveListener (attackMissSetHighlightedUnitAction);
		CombatManager.OnRepairCrewUsedOnBaseStorageSection1.RemoveListener (attackMissSetHighlightedUnitAction);
		CombatManager.OnRepairCrewUsedOnBaseStorageSection2.RemoveListener (attackMissSetHighlightedUnitAction);

		//remove listeners for ship sections being destroyed
		PhaserSection.OnPhaserSectionDestroyed.RemoveListener (combatUnitHighlightedUnitAction);
		TorpedoSection.OnTorpedoSectionDestroyed.RemoveListener (combatUnitHighlightedUnitAction);
		StorageSection.OnStorageSectionDestroyed.RemoveListener (combatUnitHighlightedUnitAction);
		CrewSection.OnCrewSectionDestroyed.RemoveListener (combatUnitHighlightedUnitAction);
		EngineSection.OnEngineSectionDestroyed.RemoveListener (combatUnitHighlightedUnitAction);

		//remove listeners for base sections being destroyed
		StarbasePhaserSection1.OnPhaserSection1Destroyed.RemoveListener (combatUnitHighlightedUnitAction);
		StarbasePhaserSection2.OnPhaserSection2Destroyed.RemoveListener (combatUnitHighlightedUnitAction);
		StarbaseTorpedoSection.OnTorpedoSectionDestroyed.RemoveListener (combatUnitHighlightedUnitAction);
		StarbaseCrewSection.OnCrewSectionDestroyed.RemoveListener (combatUnitHighlightedUnitAction);
		StarbaseStorageSection1.OnStorageSection1Destroyed.RemoveListener (combatUnitHighlightedUnitAction);
		StarbaseStorageSection2.OnStorageSection2Destroyed.RemoveListener (combatUnitHighlightedUnitAction);

		//remove listeners for ship sections being repaired
		PhaserSection.OnPhaserSectionRepaired.RemoveListener (combatUnitHighlightedUnitAction);
		TorpedoSection.OnTorpedoSectionRepaired.RemoveListener (combatUnitHighlightedUnitAction);
		StorageSection.OnStorageSectionRepaired.RemoveListener (combatUnitHighlightedUnitAction);
		CrewSection.OnCrewSectionRepaired.RemoveListener (combatUnitHighlightedUnitAction);
		EngineSection.OnEngineSectionRepaired.RemoveListener (combatUnitHighlightedUnitAction);

		//remove listeners for base sections being repaired
		StarbasePhaserSection1.OnPhaserSection1Repaired.RemoveListener (combatUnitHighlightedUnitAction);
		StarbasePhaserSection2.OnPhaserSection2Repaired.RemoveListener (combatUnitHighlightedUnitAction);
		StarbaseTorpedoSection.OnTorpedoSectionRepaired.RemoveListener (combatUnitHighlightedUnitAction);
		StarbaseCrewSection.OnCrewSectionRepaired.RemoveListener (combatUnitHighlightedUnitAction);
		StarbaseStorageSection1.OnStorageSection1Repaired.RemoveListener (combatUnitHighlightedUnitAction);
		StarbaseStorageSection2.OnStorageSection2Repaired.RemoveListener (combatUnitHighlightedUnitAction);

		//remove listener for ship being destroyed
		Ship.OnShipDestroyed.RemoveListener (combatUnitHighlightedUnitAction);

		//remove listener for base being destroyed
		Starbase.OnBaseDestroyed.RemoveListener (combatUnitHighlightedUnitAction);

		//remove listeners for damage being taken by ships
		PhaserSection.OnPhaserDamageTaken.RemoveListener (SetHighlightedUnit);
		TorpedoSection.OnTorpedoDamageTaken.RemoveListener (SetHighlightedUnit);
		StorageSection.OnStorageDamageTaken.RemoveListener (SetHighlightedUnit);
		CrewSection.OnCrewDamageTaken.RemoveListener (SetHighlightedUnit);
		EngineSection.OnEngineDamageTaken.RemoveListener (SetHighlightedUnit);

		//remove listeners for damage being taken by bases
		StarbasePhaserSection1.OnPhaserDamageTaken.RemoveListener (SetHighlightedUnit);
		StarbasePhaserSection2.OnPhaserDamageTaken.RemoveListener (SetHighlightedUnit);
		StarbaseTorpedoSection.OnTorpedoDamageTaken.RemoveListener (SetHighlightedUnit);
		StarbaseCrewSection.OnCrewDamageTaken.RemoveListener (SetHighlightedUnit);
		StarbaseStorageSection1.OnStorageDamageTaken.RemoveListener (SetHighlightedUnit);
		StarbaseStorageSection2.OnStorageDamageTaken.RemoveListener (SetHighlightedUnit);

		if (gameManager != null) {

			//remove listener for the end turn event
			gameManager.OnNewTurn.RemoveListener (playerSetBorderColorAction);
			gameManager.OnLoadedTurn.RemoveListener (playerSetBorderColorAction);

		}

		//remove listener for the ship rename event
		Ship.OnShipRenamed.RemoveListener (combatUnitSetUnitTextDisplayAction);

		//remove listener for the base rename event
		Starbase.OnBaseRenamed.RemoveListener (combatUnitSetUnitTextDisplayAction);

		//remove listeners for engaging and disengaging cloaking
		CloakingDevice.OnEngageCloakingDevice.RemoveListener (combatUnitHighlightedUnitAction);
		CloakingDevice.OnDisengageCloakingDevice.RemoveListener (combatUnitHighlightedUnitAction);

		//remove listener for updating attack status
		CombatUnit.OnUpdateAttackStatus.RemoveListener (combatUnitHighlightedUnitAction);

		//remove listener for updating repair usage status
		Starship.OnUpdateRepairStatus.RemoveListener (combatUnitHighlightedUnitAction);
		Starbase.OnUpdateRepairStatus.RemoveListener (combatUnitHighlightedUnitAction);

		//remove listener for updating cloaking status
		BirdOfPrey.OnUpdateCloakingStatus.RemoveListener (combatUnitHighlightedUnitAction);

		//remove listeners for purchasing items
		PhaserSection.OnInventoryUpdated.RemoveListener (combatUnitHighlightedUnitAction);
		TorpedoSection.OnInventoryUpdated.RemoveListener (combatUnitHighlightedUnitAction);
		StorageSection.OnInventoryUpdated.RemoveListener (combatUnitHighlightedUnitAction);
		CrewSection.OnInventoryUpdated.RemoveListener (combatUnitHighlightedUnitAction);
		EngineSection.OnInventoryUpdated.RemoveListener (combatUnitHighlightedUnitAction);

		if (uiManager != null) {
			
			//remove listener for purchasing a ship
			uiManager.GetComponent<NameNewShip> ().OnPurchasedNewShip.RemoveListener (nameNewShipSetHighlightedUnitAction);

		}

	}

}
