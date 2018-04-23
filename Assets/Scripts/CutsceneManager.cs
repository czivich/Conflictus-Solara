using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using TMPro;

public class CutsceneManager : MonoBehaviour {

	//variable to hold the tileMap
	private TileMap tileMap;

	//variable to hold the tilemap animation parent
	public GameObject tileMapAnimationParent;

	//variable to hold the cutscene panel
	public GameObject cutsceneDisplay;
	public GameObject cutscenePanel;

	//variables to hold the attacking unit and targeted unit parent objects
	public GameObject attackingUnitParent;
	public GameObject targetedUnitParent;
	public GameObject explosionsParent;
	public GameObject torpedoParent;
	public GameObject flareParent;

	//tmpro objects to hold the display labels
	public TextMeshProUGUI attackingUnitLabel;
	public TextMeshProUGUI targetedUnitLabel;
	public TextMeshProUGUI statusLabel;


	//variables to hold the animation prefabs
	public UIAnimation prefabPhasorHit;
	public UIAnimation prefabXRayHit;
	public UIAnimation prefabPhasorMuzzle;
	public UIAnimation prefabXRayMuzzle;
	public UIAnimation prefabLightTorpedo;
	public UIAnimation prefabHeavyTorpedo;
	public UIAnimation prefabTorpedoMuzzle;
	public UIAnimation prefabLightTorpedoHit;
	public UIAnimation prefabHeavyTorpedoHit;
	public UIAnimation prefabFlare;
	public UIAnimation prefabSectionExplostion;
	public UIAnimation prefabShipExplosion;

	public TileMapUnitExplosion prefabTileMapUnitExplosion;

	//variables to hold the section objects for the cutscene units
	public GameObject[] attackingUnitSections = new GameObject[6];
	public GameObject[] targetedUnitSections = new GameObject[6];

	//variables to hold the sprite objects
	public Sprite greenPhasorSection;
	public Sprite greenTorpedoSection;
	public Sprite greenStorageSection;
	public Sprite greenCrewSection;
	public Sprite greenEngineSection;

	public Sprite greenPhasorSectionDestroyed;
	public Sprite greenTorpedoSectionDestroyed;
	public Sprite greenStorageSectionDestroyed;
	public Sprite greenCrewSectionDestroyed;
	public Sprite greenEngineSectionDestroyed;

	public Sprite greenBaseStorageSection;
	public Sprite greenBaseMiddleSection;
	public Sprite greenBasePhasorSection;

	public Sprite greenBaseStorageSectionDestroyed;
	public Sprite greenBaseMiddleSectionDestroyed;
	public Sprite greenBasePhasorSectionDestroyed;

	public Sprite purplePhasorSection;
	public Sprite purpleTorpedoSection;
	public Sprite purpleStorageSection;
	public Sprite purpleCrewSection;
	public Sprite purpleEngineSection;

	public Sprite purplePhasorSectionDestroyed;
	public Sprite purpleTorpedoSectionDestroyed;
	public Sprite purpleStorageSectionDestroyed;
	public Sprite purpleCrewSectionDestroyed;
	public Sprite purpleEngineSectionDestroyed;

	public Sprite purpleBaseStorageSection;
	public Sprite purpleBaseMiddleSection;
	public Sprite purpleBasePhasorSection;

	public Sprite purpleBaseStorageSectionDestroyed;
	public Sprite purpleBaseMiddleSectionDestroyed;
	public Sprite purpleBasePhasorSectionDestroyed;

	public Sprite redPhasorSection;
	public Sprite redTorpedoSection;
	public Sprite redStorageSection;
	public Sprite redCrewSection;
	public Sprite redEngineSection;

	public Sprite redPhasorSectionDestroyed;
	public Sprite redTorpedoSectionDestroyed;
	public Sprite redStorageSectionDestroyed;
	public Sprite redCrewSectionDestroyed;
	public Sprite redEngineSectionDestroyed;

	public Sprite redBaseStorageSection;
	public Sprite redBaseMiddleSection;
	public Sprite redBasePhasorSection;

	public Sprite redBaseStorageSectionDestroyed;
	public Sprite redBaseMiddleSectionDestroyed;
	public Sprite redBasePhasorSectionDestroyed;

	public Sprite bluePhasorSection;
	public Sprite blueTorpedoSection;
	public Sprite blueStorageSection;
	public Sprite blueCrewSection;
	public Sprite blueEngineSection;

	public Sprite bluePhasorSectionDestroyed;
	public Sprite blueTorpedoSectionDestroyed;
	public Sprite blueStorageSectionDestroyed;
	public Sprite blueCrewSectionDestroyed;
	public Sprite blueEngineSectionDestroyed;

	public Sprite blueBaseStorageSection;
	public Sprite blueBaseMiddleSection;
	public Sprite blueBasePhasorSection;

	public Sprite blueBaseStorageSectionDestroyed;
	public Sprite blueBaseMiddleSectionDestroyed;
	public Sprite blueBasePhasorSectionDestroyed;

	//variable to hold the phasor lines holder parent object
	public GameObject phasorLinesHolder;

	//variable to hold the phasor line renderers
	private UILineRenderer[] phasorLineRenderer;

	//variable to hold number of points per phasor shot
	private int phasorPointsPerLine = 2;

	//I may want to have the manager cache the phasor starting and ending points
	private Vector2[] phasorLineStartPoints;
	private Vector2[] phasorLineEndPoints;

	//I need some variables to hold vector2 offsets for various unit sections
	private Vector2 starbasePhasorSection1AttackOffset;
	private Vector2 starbasePhasorSection2AttackOffset;
	private Vector2 shipPhasorSectionAttackOffset;
	private Vector2 starbaseTorpedoSectionAttackOffset;
	private Vector2 shipTorpedoSectionAttackOffset;
	private Vector2 shipFlareSpawnOffset;
	private Vector2 starbaseFlareSpawnOffset;

	//these offsets are for where the phasor lines hit the section
	private Vector2 basePhasorSection1TargetedOffset;
	private Vector2 basePhasorSection1TargetedOffsetSpacing;
	private Vector2 basePhasorSection2TargetedOffset;
	private Vector2 basePhasorSection2TargetedOffsetSpacing;
	private Vector2 baseTorpedoSectionTargetedOffset;
	private Vector2 baseTorpedoSectionTargetedOffsetSpacing;
	private Vector2 baseCrewSectionTargetedOffset;
	private Vector2 baseCrewSectionTargetedOffsetSpacing;
	private Vector2 baseStorageSection1TargetedOffset;
	private Vector2 baseStorageSection1TargetedOffsetSpacing;
	private Vector2 baseStorageSection2TargetedOffset;
	private Vector2 baseStorageSection2TargetedOffsetSpacing;

	private Vector2 shipPhasorSectionTargetedOffset;
	private Vector2 shipPhasorSectionTargetedOffsetSpacing;
	private Vector2 shipTorpedoSectionTargetedOffset;
	private Vector2 shipTorpedoSectionTargetedOffsetSpacing;
	private Vector2 shipStorageSectionTargetedOffset;
	private Vector2 shipStorageSectionTargetedOffsetSpacing;
	private Vector2 shipCrewSectionTargetedOffset;
	private Vector2 shipCrewSectionTargetedOffsetSpacing;
	private Vector2 shipEngineSectionTargetedOffset;
	private Vector2 shipEngineSectionTargetedOffsetSpacing;

	//the plan is to reuse the section sprites in different locations depending on the ship
	//in order to do this, I will need to store there transform positions depending on the type of ship
	private Vector3[] starshipSpritePosition = new Vector3[6];
	private Vector3[] destroyerSpritePosition = new Vector3[6];
	private Vector3[] birdOfPreySpritePosition = new Vector3[6];
	private Vector3[] scoutSpritePosition = new Vector3[6];
	private Vector3[] starbaseSpritePosition = new Vector3[6];

	//these vector3s are the scales to be applied to sprites
	private Vector3[] shipSpriteScale = new Vector3[6];
	private Vector3[] baseSpriteScale = new Vector3[6];

	//it's going to be way better to organize this as a state machine
	public enum AnimationState{
		Inactive,
		EnteringArena,
		WaitingAfterEntering,
		FiringPhasors,
		FiringTorpedos,
		SectionExplosion,
		UnitExplosion,
		PreparingToClose,

	}

	//variable to keep track of when we are in the first frame of a new animation state
	private bool isNewAnimationState = false;

	//variable to keep track of time spent in a given animation state
	private float currentAnimationStateTimer;

	//variables to track destinations of the combat unit parents
	private Vector3 attackingUnitDestination;
	private Vector3 targetedUnitDestination;

	//this variable holds the animnation state
	private AnimationState _currentAnimationState;
	public AnimationState CurrentAnimationState {

		get{

			return _currentAnimationState;

		}
		private set {

			if (value == _currentAnimationState) {

				return;

			} else {

				_currentAnimationState = value;

				//set the isNewAnimationState flag
				isNewAnimationState = true;

				//set the current animation state timer to zero
				currentAnimationStateTimer = 0.0f;

			}

		}

	}

	//the cutscene manager is going to need to cache information about the combat taking place
	private CombatUnit combatAttackingUnit;
	private CombatUnit combatTargetedUnit;
	private CombatManager.ShipSectionTargeted combatShipSectionTargeted;
	private CombatManager.BaseSectionTargeted combatBaseSectionTargeted;
	private CombatManager.AttackType combatAttackType;
	private int combatDamage;
	private bool isPhasorHit = false;
	private bool hasXRayKernel = false;
	private bool isTorpedoHit = false;
	private bool combatWillDestroySection;
	private bool combatWillDestroyUnit;

	//variable to store string for display panel


	//variable to control how fast the ships enter the arena, in pixels per second
	private float enteringSpeed = 400f;

	//variable to set the starting position in the arena
	private float attackingUnitStartingXPosition = -400f;
	private float targetedUnitStartingXPosition = 400f;

	//variables to control the start times for motion of the combat units
	private float attackingUnitStartTime = 0.5f;
	private float targetedUnitStartTime = 1.5f;
	private bool attackingUnitHasArrived = false;
	private bool targetedUnitHasArrived = false;
	private float bothArrivedWaitTime = 2f;

	//variable to control how fast the phasor line fires
	private float timeForPhasorToReachTarget = .25f;

	//[ColorUsageAttribute(true,true,0f,8f,0.125f,3f)]
	//public Color colour;

	//variables to hold the phasor attack colors
	private Color normalPhasorColor = new Color(

		255f/255f,
		180f/255f,
		0f/255f,
		255f/255f

	);

	/*
	private Color xRayPhasorColor = new Color(

		6.0f,
		0.0f,
		4.182f,
		1.0f

	);
	*/

	private Color xRayPhasorColor = new Color(

		255f/255f,
		0f/255f,
		180f/255f,
		255f/255f

	);


	//variables to help keep track of phasor line timing
	private float[] percentPhasorLineStartRendered;
	private float[] percentPhasorLineEndRendered;
	private float[] phasorLineStartTimes;
	private float phasorLinePulseLength = .25f;
	private float closePanelWaitTime = 3.5f;

	//variable to hold the torpedo destination position
	private Vector2 torpedoDestinationPosition;
	private Vector2 torpedoStartingPosition;
	private Vector2 torpedoCurrentPosition;

	//variable to control torpedo speed, in pixels per second
	private float torpedoSpeed = 75f;

	//variable to track whether torpedo has arrived
	private bool torpedoHasArrived = false;

	//variables to track flare use
	private bool flaresUsed;
	private int numberFlaresUsed;
	private bool flaresSuccessful;
	private Vector2 flareClusterPoint;
	private Vector2[] flareDestinationPosition;
	private Vector2 flareSpawnPoint;
	private float flareSpawnTime = 2.5f;
	private bool flaresLaunched;
	private float flareSpeed = 100f;
	private bool[] flareArrived;
	private bool torpedoReroutedToFlare;
	private bool torpedoPastAllFlares;

	//variables to track section explosions
	private Vector2[] sectionDestroyedExplosions;
	private float explosionStartTime = 1.0f;
	private float explosionStartDuration = 2.0f;
	private float explosionAnimationDuration = 4.0f;
	private bool updatedDestroyedGraphic = false;
	private float[] explosionCooldown;
	private float explosionCooldownThreshold = 1.0f;

	//variables to track unit explosions
	private float unitExplosionStartTime = 2.5f;
	private float unitSmallExplosionStartTime = 1.0f;
	private float unitExplosionAnimationDuration = 6.0f;
	private bool detonatedUnitExplosion = false;
	//private bool detonatedUnitSmallExplosion = false;
	private List<Vector2> unitDestroyedExplosions;

	//unityEvent to signal opening and closing of display panel
	public UnityEvent OnOpenCutsceneDisplayPanel = new UnityEvent();
	public UnityEvent OnCloseCutsceneDisplayPanel = new UnityEvent();

	//these events are for firing phasors
	public UnityEvent OnFirePhasors = new UnityEvent();
	public UnityEvent OnFireXRay = new UnityEvent();
	public UnityEvent OnPhasorHit = new UnityEvent();

	//these events are for charging sounds
	public UnityEvent OnChargePhasors = new UnityEvent();
	public UnityEvent OnChargeXRay = new UnityEvent();
	public UnityEvent OnChargeLightTorpedo = new UnityEvent();
	public UnityEvent OnChargeHeavyTorpedo = new UnityEvent();

	//these events are for firing torpedos
	public UnityEvent OnFireLightTorpedo = new UnityEvent();
	public UnityEvent OnFireHeavyTorpedo = new UnityEvent();


	//this event is for the explosion sound
	public UnityEvent OnCreateExplosion = new UnityEvent();

	//these events are for getting in between combat manager events and downstream events
	public static CombatCutsceneEvent OnPhasorHitShipPhasorSection = new CombatCutsceneEvent();
	public static CombatCutsceneEvent OnPhasorHitShipTorpedoSection = new CombatCutsceneEvent();
	public static CombatCutsceneEvent OnPhasorHitShipStorageSection = new CombatCutsceneEvent();
	public static CombatCutsceneEvent OnPhasorHitShipCrewSection = new CombatCutsceneEvent();
	public static CombatCutsceneEvent OnPhasorHitShipEngineSection = new CombatCutsceneEvent();
	public static CombatCutsceneEvent OnPhasorMissShip = new CombatCutsceneEvent();

	public static CombatCutsceneEvent OnPhasorHitBasePhasorSection1 = new CombatCutsceneEvent();
	public static CombatCutsceneEvent OnPhasorHitBasePhasorSection2 = new CombatCutsceneEvent();
	public static CombatCutsceneEvent OnPhasorHitBaseTorpedoSection = new CombatCutsceneEvent();
	public static CombatCutsceneEvent OnPhasorHitBaseCrewSection = new CombatCutsceneEvent();
	public static CombatCutsceneEvent OnPhasorHitBaseStorageSection1 = new CombatCutsceneEvent();
	public static CombatCutsceneEvent OnPhasorHitBaseStorageSection2 = new CombatCutsceneEvent();
	public static CombatCutsceneEvent OnPhasorMissBase = new CombatCutsceneEvent();

	public static CombatCutsceneEvent OnLightTorpedoHitShipPhasorSection = new CombatCutsceneEvent();
	public static CombatCutsceneEvent OnLightTorpedoHitShipTorpedoSection = new CombatCutsceneEvent();
	public static CombatCutsceneEvent OnLightTorpedoHitShipStorageSection = new CombatCutsceneEvent();
	public static CombatCutsceneEvent OnLightTorpedoHitShipCrewSection = new CombatCutsceneEvent();
	public static CombatCutsceneEvent OnLightTorpedoHitShipEngineSection = new CombatCutsceneEvent();
	public static CombatCutsceneEvent OnLightTorpedoMissShip = new CombatCutsceneEvent();

	public static CombatCutsceneEvent OnLightTorpedoHitBasePhasorSection1 = new CombatCutsceneEvent();
	public static CombatCutsceneEvent OnLightTorpedoHitBasePhasorSection2 = new CombatCutsceneEvent();
	public static CombatCutsceneEvent OnLightTorpedoHitBaseTorpedoSection = new CombatCutsceneEvent();
	public static CombatCutsceneEvent OnLightTorpedoHitBaseCrewSection = new CombatCutsceneEvent();
	public static CombatCutsceneEvent OnLightTorpedoHitBaseStorageSection1 = new CombatCutsceneEvent();
	public static CombatCutsceneEvent OnLightTorpedoHitBaseStorageSection2 = new CombatCutsceneEvent();
	public static CombatCutsceneEvent OnLightTorpedoMissBase = new CombatCutsceneEvent();

	public static CombatCutsceneEvent OnHeavyTorpedoHitShipPhasorSection = new CombatCutsceneEvent();
	public static CombatCutsceneEvent OnHeavyTorpedoHitShipTorpedoSection = new CombatCutsceneEvent();
	public static CombatCutsceneEvent OnHeavyTorpedoHitShipStorageSection = new CombatCutsceneEvent();
	public static CombatCutsceneEvent OnHeavyTorpedoHitShipCrewSection = new CombatCutsceneEvent();
	public static CombatCutsceneEvent OnHeavyTorpedoHitShipEngineSection = new CombatCutsceneEvent();
	public static CombatCutsceneEvent OnHeavyTorpedoMissShip = new CombatCutsceneEvent();

	public static CombatCutsceneEvent OnHeavyTorpedoHitBasePhasorSection1 = new CombatCutsceneEvent();
	public static CombatCutsceneEvent OnHeavyTorpedoHitBasePhasorSection2 = new CombatCutsceneEvent();
	public static CombatCutsceneEvent OnHeavyTorpedoHitBaseTorpedoSection = new CombatCutsceneEvent();
	public static CombatCutsceneEvent OnHeavyTorpedoHitBaseCrewSection = new CombatCutsceneEvent();
	public static CombatCutsceneEvent OnHeavyTorpedoHitBaseStorageSection1 = new CombatCutsceneEvent();
	public static CombatCutsceneEvent OnHeavyTorpedoHitBaseStorageSection2 = new CombatCutsceneEvent();
	public static CombatCutsceneEvent OnHeavyTorpedoMissBase = new CombatCutsceneEvent();

	public static CombatCutsceneEvent OnLightTorpedoFlareSuccess = new CombatCutsceneEvent();
	public static CombatCutsceneEvent OnHeavyTorpedoFlareSuccess = new CombatCutsceneEvent();

	public static CombatCutsceneEvent OnLightTorpedoFlareFailure = new CombatCutsceneEvent();
	public static CombatCutsceneEvent OnHeavyTorpedoFlareFailure = new CombatCutsceneEvent();




	//these events are for getting in between combat manager events and downstream events
	public class CombatCutsceneEvent : UnityEvent<CombatUnit,CombatUnit,int>{}; 

	//unity actions for events
	private UnityAction<CombatUnit,CombatUnit,int> PhasorHitShipPhasorSectionAction;
	private UnityAction<CombatUnit,CombatUnit,int> PhasorHitShipTorpedoSectionAction;
	private UnityAction<CombatUnit,CombatUnit,int> PhasorHitShipStorageSectionAction;
	private UnityAction<CombatUnit,CombatUnit,int> PhasorHitShipCrewSectionAction;
	private UnityAction<CombatUnit,CombatUnit,int> PhasorHitShipEngineSectionAction;
	private UnityAction<CombatUnit,CombatUnit> PhasorMissShipAction;

	private UnityAction<CombatUnit,CombatUnit,int> PhasorHitBasePhasorSection1Action;
	private UnityAction<CombatUnit,CombatUnit,int> PhasorHitBasePhasorSection2Action;
	private UnityAction<CombatUnit,CombatUnit,int> PhasorHitBaseTorpedoSectionAction;
	private UnityAction<CombatUnit,CombatUnit,int> PhasorHitBaseCrewSectionAction;
	private UnityAction<CombatUnit,CombatUnit,int> PhasorHitBaseStorageSection1Action;
	private UnityAction<CombatUnit,CombatUnit,int> PhasorHitBaseStorageSection2Action;
	private UnityAction<CombatUnit,CombatUnit> PhasorMissBaseAction;

	private UnityAction<CombatUnit,CombatUnit,int> LightTorpedoHitShipPhasorSectionAction;
	private UnityAction<CombatUnit,CombatUnit,int> LightTorpedoHitShipTorpedoSectionAction;
	private UnityAction<CombatUnit,CombatUnit,int> LightTorpedoHitShipStorageSectionAction;
	private UnityAction<CombatUnit,CombatUnit,int> LightTorpedoHitShipCrewSectionAction;
	private UnityAction<CombatUnit,CombatUnit,int> LightTorpedoHitShipEngineSectionAction;
	private UnityAction<CombatUnit,CombatUnit> LightTorpedoMissShipAction;

	private UnityAction<CombatUnit,CombatUnit,int> LightTorpedoHitBasePhasorSection1Action;
	private UnityAction<CombatUnit,CombatUnit,int> LightTorpedoHitBasePhasorSection2Action;
	private UnityAction<CombatUnit,CombatUnit,int> LightTorpedoHitBaseTorpedoSectionAction;
	private UnityAction<CombatUnit,CombatUnit,int> LightTorpedoHitBaseCrewSectionAction;
	private UnityAction<CombatUnit,CombatUnit,int> LightTorpedoHitBaseStorageSection1Action;
	private UnityAction<CombatUnit,CombatUnit,int> LightTorpedoHitBaseStorageSection2Action;
	private UnityAction<CombatUnit,CombatUnit> LightTorpedoMissBaseAction;

	private UnityAction<CombatUnit,CombatUnit,int> HeavyTorpedoHitShipPhasorSectionAction;
	private UnityAction<CombatUnit,CombatUnit,int> HeavyTorpedoHitShipTorpedoSectionAction;
	private UnityAction<CombatUnit,CombatUnit,int> HeavyTorpedoHitShipStorageSectionAction;
	private UnityAction<CombatUnit,CombatUnit,int> HeavyTorpedoHitShipCrewSectionAction;
	private UnityAction<CombatUnit,CombatUnit,int> HeavyTorpedoHitShipEngineSectionAction;
	private UnityAction<CombatUnit,CombatUnit> HeavyTorpedoMissShipAction;

	private UnityAction<CombatUnit,CombatUnit,int> HeavyTorpedoHitBasePhasorSection1Action;
	private UnityAction<CombatUnit,CombatUnit,int> HeavyTorpedoHitBasePhasorSection2Action;
	private UnityAction<CombatUnit,CombatUnit,int> HeavyTorpedoHitBaseTorpedoSectionAction;
	private UnityAction<CombatUnit,CombatUnit,int> HeavyTorpedoHitBaseCrewSectionAction;
	private UnityAction<CombatUnit,CombatUnit,int> HeavyTorpedoHitBaseStorageSection1Action;
	private UnityAction<CombatUnit,CombatUnit,int> HeavyTorpedoHitBaseStorageSection2Action;
	private UnityAction<CombatUnit,CombatUnit> HeavyTorpedoMissBaseAction;

	private UnityAction<CombatUnit,CombatUnit,CombatManager.ShipSectionTargeted,int> LightTorpedoFlareSuccessAction;
	private UnityAction<CombatUnit,CombatUnit,CombatManager.ShipSectionTargeted,int> HeavyTorpedoFlareSuccessAction;
	private UnityAction<CombatUnit,CombatUnit,int> LightTorpedoFlareFailureAction;
	private UnityAction<CombatUnit,CombatUnit,int> HeavyTorpedoFlareFailureAction;

	// Use this for initialization
	public void Init () {

		//get the tileMap
		tileMap = GameObject.FindGameObjectWithTag("TileMap").GetComponent<TileMap>();

		//initialize the sprite positions
		starshipSpritePosition[0] = new Vector3 (0,0,0);		//phasor
		starshipSpritePosition[1] = new Vector3 (0,0,0);		//torpedo
		starshipSpritePosition[2] = new Vector3 (0,0,0);		//storage
		starshipSpritePosition[3] = new Vector3 (0,0,0);		//crew
		starshipSpritePosition[4] = new Vector3 (0,0,0);		//engine
		starshipSpritePosition[5] = new Vector3 (0,0,0);		//the 6th sprite should be inactive for a starship

		destroyerSpritePosition[0] = new Vector3 (0,0,0);		//phasor
		destroyerSpritePosition[1] = new Vector3 (0,0,0);		//torpedo
		destroyerSpritePosition[2] = new Vector3 (0,0,0);		//storage
		destroyerSpritePosition[3] = new Vector3 (0,0,0);		//the 4th sprite should be inactive for a destroyer
		destroyerSpritePosition[4] = new Vector3 (0,-32,0);		//engine
		destroyerSpritePosition[5] = new Vector3 (0,0,0);		//the 6th sprite should be inactive for a destroyer

		birdOfPreySpritePosition[0] = new Vector3 (0,0,0);		//phasor
		birdOfPreySpritePosition[1] = new Vector3 (0,0,0);		//torpedo
		birdOfPreySpritePosition[2] = new Vector3 (0,0,0);		//the 3rd sprite should be inactive for a bird of prey
		birdOfPreySpritePosition[3] = new Vector3 (0,0,0);		//the 4th sprite should be inactive for a bird of prey
		birdOfPreySpritePosition[4] = new Vector3 (81,-52,0);	//engine
		birdOfPreySpritePosition[5] = new Vector3 (0,0,0);		//the 6th sprite should be inactive for a bird of prey

		scoutSpritePosition[0] = new Vector3 (0,-29,0);			//phasor
		scoutSpritePosition[1] = new Vector3 (0,0,0);			//the 2nd sprite should be inactive for a scout
		scoutSpritePosition[2] = new Vector3 (0,0,0);			//storage
		scoutSpritePosition[3] = new Vector3 (0,0,0);			//the 4th sprite should be inactive for a scout
		scoutSpritePosition[4] = new Vector3 (0,-32,0);			//engine
		scoutSpritePosition[5] = new Vector3 (0,0,0);			//the 6th sprite should be inactive for a scout

		starbaseSpritePosition[0] = new Vector3 (0,0,0);		//phasor 1
		starbaseSpritePosition[1] = new Vector3 (0,0,0);		//phasor 2
		starbaseSpritePosition[2] = new Vector3 (0,0,0);		//torpedo
		starbaseSpritePosition[3] = new Vector3 (0,0,0);		//crew
		starbaseSpritePosition[4] = new Vector3 (0,0,0);		//storage 1
		starbaseSpritePosition[5] = new Vector3 (0,0,0);		//storage 2

		//initialize the sprite scales
		shipSpriteScale[0] = new Vector3 (1,1,1);
		shipSpriteScale[1] = new Vector3 (1,1,1);
		shipSpriteScale[2] = new Vector3 (1,1,1);
		shipSpriteScale[3] = new Vector3 (1,1,1);
		shipSpriteScale[4] = new Vector3 (1,1,1);
		shipSpriteScale[5] = new Vector3 (1,1,1);

		baseSpriteScale[0] = new Vector3 (1,1,1);
		baseSpriteScale[1] = new Vector3 (1,-1,1);
		baseSpriteScale[2] = new Vector3 (1,1,1);
		baseSpriteScale[3] = new Vector3 (1,-1,1);
		baseSpriteScale[4] = new Vector3 (1,1,1);
		baseSpriteScale[5] = new Vector3 (1,-1,1);


		//set up the phasor line array size
		phasorLineRenderer = new UILineRenderer[phasorLinesHolder.transform.childCount];
		phasorLineStartPoints = new Vector2[phasorLinesHolder.transform.childCount];
		phasorLineEndPoints = new Vector2[phasorLinesHolder.transform.childCount];

		percentPhasorLineStartRendered = new float[phasorLinesHolder.transform.childCount];
		percentPhasorLineEndRendered = new float[phasorLinesHolder.transform.childCount];
		phasorLineStartTimes = new float[phasorLinesHolder.transform.childCount];

		//set the start times for phasor line drawing
		phasorLineStartTimes[0] = 0.0f;
		phasorLineStartTimes[1] = 0.5f;
		phasorLineStartTimes[2] = 1.0f;
		phasorLineStartTimes[3] = 1.5f;
		phasorLineStartTimes[4] = 2.0f;


		//assign the phasorLineRenderers
		for (int i = 0; i < phasorLineRenderer.Length; i++) {

			//assign the renderers
			phasorLineRenderer [i] = phasorLinesHolder.transform.GetChild (i).GetComponent<UILineRenderer> ();
			phasorLineRenderer [i].Points = new Vector2[phasorPointsPerLine];

			//set the percentages to zero to start
			percentPhasorLineStartRendered[i] = 0.0f;
			percentPhasorLineEndRendered[i] = 0.0f;

			//start with the renderer gameobjects disabled
			phasorLineRenderer[i].gameObject.SetActive(false);

		}

		//initialize the offsets
		starbasePhasorSection1AttackOffset = new Vector2(23,100);
		starbasePhasorSection2AttackOffset = new Vector2(23,-100);
		shipPhasorSectionAttackOffset = new Vector2(123,24);

		starbaseTorpedoSectionAttackOffset = new Vector2(75,36);
		shipTorpedoSectionAttackOffset = new Vector2(48,-8);

		shipFlareSpawnOffset = new Vector2 (55, -30);
		starbaseFlareSpawnOffset = new Vector2 (86, -12);

		basePhasorSection1TargetedOffset = new Vector2 (-40, 60);
		basePhasorSection1TargetedOffsetSpacing = new Vector2 (20, 0);
		basePhasorSection2TargetedOffset = new Vector2 (-40, -60);
		basePhasorSection2TargetedOffsetSpacing = new Vector2 (20, 0);
		baseTorpedoSectionTargetedOffset = new Vector2 (-60, 36);
		baseTorpedoSectionTargetedOffsetSpacing = new Vector2 (30, 0);
		baseCrewSectionTargetedOffset = new Vector2 (-60, -36);
		baseCrewSectionTargetedOffsetSpacing = new Vector2 (30, 0);
		baseStorageSection1TargetedOffset = new Vector2 (-70, 11);
		baseStorageSection1TargetedOffsetSpacing = new Vector2 (35, 0);
		baseStorageSection2TargetedOffset = new Vector2 (-70, -11);
		baseStorageSection2TargetedOffsetSpacing = new Vector2 (35, 0);

		shipPhasorSectionTargetedOffset = new Vector2(-110, 23);
		shipPhasorSectionTargetedOffsetSpacing = new Vector2(20, 0);
		shipTorpedoSectionTargetedOffset = new Vector2(-40, 7);
		shipTorpedoSectionTargetedOffsetSpacing = new Vector2(5, -5);
		shipStorageSectionTargetedOffset = new Vector2(-45, -30);
		shipStorageSectionTargetedOffsetSpacing = new Vector2(20, 0);
		shipCrewSectionTargetedOffset = new Vector2(40, -13);
		shipCrewSectionTargetedOffsetSpacing = new Vector2(5, 5);
		shipEngineSectionTargetedOffset = new Vector2(35, 23);
		shipEngineSectionTargetedOffsetSpacing = new Vector2(20, 0);


		//start with the cutscene panel disabled
		CloseCutsceneDisplayPanel();

		//set the actions
		PhasorHitShipPhasorSectionAction = (attackingUnit,targetedUnit,damage) => 
		{ResolvePhasorHit(attackingUnit,targetedUnit,CombatManager.ShipSectionTargeted.PhasorSection,damage);};

		PhasorHitShipTorpedoSectionAction = (attackingUnit,targetedUnit,damage) => 
		{ResolvePhasorHit(attackingUnit,targetedUnit,CombatManager.ShipSectionTargeted.TorpedoSection,damage);};

		PhasorHitShipStorageSectionAction = (attackingUnit,targetedUnit,damage) => 
		{ResolvePhasorHit(attackingUnit,targetedUnit,CombatManager.ShipSectionTargeted.StorageSection,damage);};

		PhasorHitShipCrewSectionAction = (attackingUnit,targetedUnit,damage) => 
		{ResolvePhasorHit(attackingUnit,targetedUnit,CombatManager.ShipSectionTargeted.CrewSection,damage);};

		PhasorHitShipEngineSectionAction = (attackingUnit,targetedUnit,damage) => 
		{ResolvePhasorHit(attackingUnit,targetedUnit,CombatManager.ShipSectionTargeted.EngineSection,damage);};

		PhasorMissShipAction = (attackingUnit,targetedUnit) => {ResolvePhasorMiss(attackingUnit,targetedUnit);};


		PhasorHitBasePhasorSection1Action = (attackingUnit,targetedUnit,damage) => 
		{ResolvePhasorHit(attackingUnit,targetedUnit,(CombatManager.ShipSectionTargeted)CombatManager.BaseSectionTargeted.PhasorSection1,damage);};

		PhasorHitBasePhasorSection2Action = (attackingUnit,targetedUnit,damage) => 
		{ResolvePhasorHit(attackingUnit,targetedUnit,(CombatManager.ShipSectionTargeted)CombatManager.BaseSectionTargeted.PhasorSection2,damage);};

		PhasorHitBaseTorpedoSectionAction = (attackingUnit,targetedUnit,damage) => 
		{ResolvePhasorHit(attackingUnit,targetedUnit,(CombatManager.ShipSectionTargeted)CombatManager.BaseSectionTargeted.TorpedoSection,damage);};

		PhasorHitBaseCrewSectionAction = (attackingUnit,targetedUnit,damage) => 
		{ResolvePhasorHit(attackingUnit,targetedUnit,(CombatManager.ShipSectionTargeted)CombatManager.BaseSectionTargeted.CrewSection,damage);};

		PhasorHitBaseStorageSection1Action = (attackingUnit,targetedUnit,damage) => 
		{ResolvePhasorHit(attackingUnit,targetedUnit,(CombatManager.ShipSectionTargeted)CombatManager.BaseSectionTargeted.StorageSection1,damage);};

		PhasorHitBaseStorageSection2Action = (attackingUnit,targetedUnit,damage) => 
		{ResolvePhasorHit(attackingUnit,targetedUnit,(CombatManager.ShipSectionTargeted)CombatManager.BaseSectionTargeted.StorageSection2,damage);};

		PhasorMissBaseAction = (attackingUnit,targetedUnit) => {ResolvePhasorMiss(attackingUnit,targetedUnit);};


		LightTorpedoHitShipPhasorSectionAction = (attackingUnit,targetedUnit,damage) => 
		{ResolveTorpedoHit(attackingUnit,targetedUnit,CombatManager.ShipSectionTargeted.PhasorSection,CombatManager.AttackType.LightTorpedo,damage);};

		LightTorpedoHitShipTorpedoSectionAction = (attackingUnit,targetedUnit,damage) => 
		{ResolveTorpedoHit(attackingUnit,targetedUnit,CombatManager.ShipSectionTargeted.TorpedoSection,CombatManager.AttackType.LightTorpedo,damage);};

		LightTorpedoHitShipStorageSectionAction = (attackingUnit,targetedUnit,damage) => 
		{ResolveTorpedoHit(attackingUnit,targetedUnit,CombatManager.ShipSectionTargeted.StorageSection,CombatManager.AttackType.LightTorpedo,damage);};

		LightTorpedoHitShipCrewSectionAction = (attackingUnit,targetedUnit,damage) => 
		{ResolveTorpedoHit(attackingUnit,targetedUnit,CombatManager.ShipSectionTargeted.CrewSection,CombatManager.AttackType.LightTorpedo,damage);};

		LightTorpedoHitShipEngineSectionAction = (attackingUnit,targetedUnit,damage) => 
		{ResolveTorpedoHit(attackingUnit,targetedUnit,CombatManager.ShipSectionTargeted.EngineSection,CombatManager.AttackType.LightTorpedo,damage);};

		LightTorpedoMissShipAction = (attackingUnit,targetedUnit) => {ResolveTorpedoMiss(attackingUnit,targetedUnit,CombatManager.AttackType.LightTorpedo);};


		LightTorpedoHitBasePhasorSection1Action = (attackingUnit,targetedUnit,damage) => 
		{ResolveTorpedoHit(attackingUnit,targetedUnit,(CombatManager.ShipSectionTargeted)CombatManager.BaseSectionTargeted.PhasorSection1,CombatManager.AttackType.LightTorpedo,damage);};

		LightTorpedoHitBasePhasorSection2Action = (attackingUnit,targetedUnit,damage) => 
		{ResolveTorpedoHit(attackingUnit,targetedUnit,(CombatManager.ShipSectionTargeted)CombatManager.BaseSectionTargeted.PhasorSection2,CombatManager.AttackType.LightTorpedo,damage);};

		LightTorpedoHitBaseTorpedoSectionAction = (attackingUnit,targetedUnit,damage) => 
		{ResolveTorpedoHit(attackingUnit,targetedUnit,(CombatManager.ShipSectionTargeted)CombatManager.BaseSectionTargeted.TorpedoSection,CombatManager.AttackType.LightTorpedo,damage);};

		LightTorpedoHitBaseCrewSectionAction = (attackingUnit,targetedUnit,damage) => 
		{ResolveTorpedoHit(attackingUnit,targetedUnit,(CombatManager.ShipSectionTargeted)CombatManager.BaseSectionTargeted.CrewSection,CombatManager.AttackType.LightTorpedo,damage);};

		LightTorpedoHitBaseStorageSection1Action = (attackingUnit,targetedUnit,damage) => 
		{ResolveTorpedoHit(attackingUnit,targetedUnit,(CombatManager.ShipSectionTargeted)CombatManager.BaseSectionTargeted.StorageSection1,CombatManager.AttackType.LightTorpedo,damage);};
	
		LightTorpedoHitBaseStorageSection2Action = (attackingUnit,targetedUnit,damage) => 
		{ResolveTorpedoHit(attackingUnit,targetedUnit,(CombatManager.ShipSectionTargeted)CombatManager.BaseSectionTargeted.StorageSection2,CombatManager.AttackType.LightTorpedo,damage);};

		LightTorpedoMissBaseAction = (attackingUnit,targetedUnit) => {ResolveTorpedoMiss(attackingUnit,targetedUnit,CombatManager.AttackType.LightTorpedo);};


		HeavyTorpedoHitShipPhasorSectionAction = (attackingUnit,targetedUnit,damage) => 
		{ResolveTorpedoHit(attackingUnit,targetedUnit,CombatManager.ShipSectionTargeted.PhasorSection,CombatManager.AttackType.HeavyTorpedo,damage);};

		HeavyTorpedoHitShipTorpedoSectionAction = (attackingUnit,targetedUnit,damage) => 
		{ResolveTorpedoHit(attackingUnit,targetedUnit,CombatManager.ShipSectionTargeted.TorpedoSection,CombatManager.AttackType.HeavyTorpedo,damage);};

		HeavyTorpedoHitShipStorageSectionAction = (attackingUnit,targetedUnit,damage) => 
		{ResolveTorpedoHit(attackingUnit,targetedUnit,CombatManager.ShipSectionTargeted.StorageSection,CombatManager.AttackType.HeavyTorpedo,damage);};

		HeavyTorpedoHitShipCrewSectionAction = (attackingUnit, targetedUnit, damage) => 
		{ResolveTorpedoHit (attackingUnit, targetedUnit, CombatManager.ShipSectionTargeted.CrewSection, CombatManager.AttackType.HeavyTorpedo, damage);};
			
		HeavyTorpedoHitShipEngineSectionAction = (attackingUnit,targetedUnit,damage) => 
		{ResolveTorpedoHit(attackingUnit,targetedUnit,CombatManager.ShipSectionTargeted.EngineSection,CombatManager.AttackType.HeavyTorpedo,damage);};

		HeavyTorpedoMissShipAction = (attackingUnit,targetedUnit) => {ResolveTorpedoMiss(attackingUnit,targetedUnit,CombatManager.AttackType.HeavyTorpedo);};


		HeavyTorpedoHitBasePhasorSection1Action = (attackingUnit,targetedUnit,damage) => 
		{ResolveTorpedoHit(attackingUnit,targetedUnit,(CombatManager.ShipSectionTargeted)CombatManager.BaseSectionTargeted.PhasorSection1,CombatManager.AttackType.HeavyTorpedo,damage);};

		HeavyTorpedoHitBasePhasorSection2Action = (attackingUnit,targetedUnit,damage) => 
		{ResolveTorpedoHit(attackingUnit,targetedUnit,(CombatManager.ShipSectionTargeted)CombatManager.BaseSectionTargeted.PhasorSection2,CombatManager.AttackType.HeavyTorpedo,damage);};

		HeavyTorpedoHitBaseTorpedoSectionAction = (attackingUnit,targetedUnit,damage) => 
		{ResolveTorpedoHit(attackingUnit,targetedUnit,(CombatManager.ShipSectionTargeted)CombatManager.BaseSectionTargeted.TorpedoSection,CombatManager.AttackType.HeavyTorpedo,damage);};

		HeavyTorpedoHitBaseCrewSectionAction = (attackingUnit,targetedUnit,damage) => 
		{ResolveTorpedoHit(attackingUnit,targetedUnit,(CombatManager.ShipSectionTargeted)CombatManager.BaseSectionTargeted.CrewSection,CombatManager.AttackType.HeavyTorpedo,damage);};

		HeavyTorpedoHitBaseStorageSection1Action = (attackingUnit,targetedUnit,damage) => 
		{ResolveTorpedoHit(attackingUnit,targetedUnit,(CombatManager.ShipSectionTargeted)CombatManager.BaseSectionTargeted.StorageSection1,CombatManager.AttackType.HeavyTorpedo,damage);};

		HeavyTorpedoHitBaseStorageSection2Action = (attackingUnit,targetedUnit,damage) => 
		{ResolveTorpedoHit(attackingUnit,targetedUnit,(CombatManager.ShipSectionTargeted)CombatManager.BaseSectionTargeted.StorageSection2,CombatManager.AttackType.HeavyTorpedo,damage);};

		HeavyTorpedoMissBaseAction = (attackingUnit,targetedUnit) => {ResolveTorpedoMiss(attackingUnit,targetedUnit,CombatManager.AttackType.HeavyTorpedo);};


		LightTorpedoFlareSuccessAction = (attackingUnit, targetedUnit, sectionTargeted, numberFlares) => {
			CatchFlareResults (numberFlares, true);
			ResolveTorpedoFlareIntercept(attackingUnit,targetedUnit,CombatManager.AttackType.LightTorpedo,sectionTargeted);		
		};

		HeavyTorpedoFlareSuccessAction = (attackingUnit, targetedUnit, sectionTargeted, numberFlares) => {
			CatchFlareResults (numberFlares, true);
			ResolveTorpedoFlareIntercept(attackingUnit,targetedUnit,CombatManager.AttackType.HeavyTorpedo,sectionTargeted);		
		};

		LightTorpedoFlareFailureAction = (attackingUnit, targetedUnit, numberFlares) => {CatchFlareResults (numberFlares, false);};
		
		HeavyTorpedoFlareFailureAction = (attackingUnit, targetedUnit, numberFlares) => {CatchFlareResults (numberFlares, false);};

		//add a listener for a phasor attack
		CombatManager.OnPhasorAttackHitShipPhasorSection.AddListener(PhasorHitShipPhasorSectionAction);
		CombatManager.OnPhasorAttackHitShipTorpedoSection.AddListener(PhasorHitShipTorpedoSectionAction);
		CombatManager.OnPhasorAttackHitShipStorageSection.AddListener(PhasorHitShipStorageSectionAction);
		CombatManager.OnPhasorAttackHitShipCrewSection.AddListener(PhasorHitShipCrewSectionAction);
		CombatManager.OnPhasorAttackHitShipEngineSection.AddListener(PhasorHitShipEngineSectionAction);
		CombatManager.OnPhasorAttackMissShip.AddListener(PhasorMissShipAction);

		CombatManager.OnPhasorAttackHitBasePhasorSection1.AddListener(PhasorHitBasePhasorSection1Action);
		CombatManager.OnPhasorAttackHitBasePhasorSection2.AddListener(PhasorHitBasePhasorSection2Action);
		CombatManager.OnPhasorAttackHitBaseTorpedoSection.AddListener(PhasorHitBaseTorpedoSectionAction);
		CombatManager.OnPhasorAttackHitBaseCrewSection.AddListener(PhasorHitBaseCrewSectionAction);
		CombatManager.OnPhasorAttackHitBaseStorageSection1.AddListener(PhasorHitBaseStorageSection1Action);
		CombatManager.OnPhasorAttackHitBaseStorageSection2.AddListener(PhasorHitBaseStorageSection2Action);
		CombatManager.OnPhasorAttackMissBase.AddListener(PhasorMissBaseAction);

		CombatManager.OnLightTorpedoAttackHitShipPhasorSection.AddListener(LightTorpedoHitShipPhasorSectionAction);
		CombatManager.OnLightTorpedoAttackHitShipTorpedoSection.AddListener(LightTorpedoHitShipTorpedoSectionAction);
		CombatManager.OnLightTorpedoAttackHitShipStorageSection.AddListener(LightTorpedoHitShipStorageSectionAction);
		CombatManager.OnLightTorpedoAttackHitShipCrewSection.AddListener(LightTorpedoHitShipCrewSectionAction);
		CombatManager.OnLightTorpedoAttackHitShipEngineSection.AddListener(LightTorpedoHitShipEngineSectionAction);
		CombatManager.OnLightTorpedoAttackMissShip.AddListener(LightTorpedoMissShipAction);

		CombatManager.OnLightTorpedoAttackHitBasePhasorSection1.AddListener(LightTorpedoHitBasePhasorSection1Action);
		CombatManager.OnLightTorpedoAttackHitBasePhasorSection2.AddListener(LightTorpedoHitBasePhasorSection2Action);
		CombatManager.OnLightTorpedoAttackHitBaseTorpedoSection.AddListener(LightTorpedoHitBaseTorpedoSectionAction);
		CombatManager.OnLightTorpedoAttackHitBaseCrewSection.AddListener(LightTorpedoHitBaseCrewSectionAction);
		CombatManager.OnLightTorpedoAttackHitBaseStorageSection1.AddListener(LightTorpedoHitBaseStorageSection1Action);
		CombatManager.OnLightTorpedoAttackHitBaseStorageSection2.AddListener(LightTorpedoHitBaseStorageSection2Action);
		CombatManager.OnLightTorpedoAttackMissBase.AddListener(LightTorpedoMissBaseAction);

		CombatManager.OnHeavyTorpedoAttackHitShipPhasorSection.AddListener(HeavyTorpedoHitShipPhasorSectionAction);
		CombatManager.OnHeavyTorpedoAttackHitShipTorpedoSection.AddListener(HeavyTorpedoHitShipTorpedoSectionAction);
		CombatManager.OnHeavyTorpedoAttackHitShipStorageSection.AddListener(HeavyTorpedoHitShipStorageSectionAction);
		CombatManager.OnHeavyTorpedoAttackHitShipCrewSection.AddListener(HeavyTorpedoHitShipCrewSectionAction);
		CombatManager.OnHeavyTorpedoAttackHitShipEngineSection.AddListener(HeavyTorpedoHitShipEngineSectionAction);
		CombatManager.OnHeavyTorpedoAttackMissShip.AddListener(HeavyTorpedoMissShipAction);

		CombatManager.OnHeavyTorpedoAttackHitBasePhasorSection1.AddListener(HeavyTorpedoHitBasePhasorSection1Action);
		CombatManager.OnHeavyTorpedoAttackHitBasePhasorSection2.AddListener(HeavyTorpedoHitBasePhasorSection2Action);
		CombatManager.OnHeavyTorpedoAttackHitBaseTorpedoSection.AddListener(HeavyTorpedoHitBaseTorpedoSectionAction);
		CombatManager.OnHeavyTorpedoAttackHitBaseCrewSection.AddListener(HeavyTorpedoHitBaseCrewSectionAction);
		CombatManager.OnHeavyTorpedoAttackHitBaseStorageSection1.AddListener(HeavyTorpedoHitBaseStorageSection1Action);
		CombatManager.OnHeavyTorpedoAttackHitBaseStorageSection2.AddListener(HeavyTorpedoHitBaseStorageSection2Action);
		CombatManager.OnHeavyTorpedoAttackMissBase.AddListener(HeavyTorpedoMissBaseAction);

		CombatManager.OnLightTorpedoAttackDefeatedByFlares.AddListener (LightTorpedoFlareSuccessAction);
		CombatManager.OnHeavyTorpedoAttackDefeatedByFlares.AddListener (HeavyTorpedoFlareSuccessAction);
		CombatManager.OnLightTorpedoAttackFlaresFailed.AddListener (LightTorpedoFlareFailureAction);
		CombatManager.OnHeavyTorpedoAttackFlaresFailed.AddListener (HeavyTorpedoFlareFailureAction);

		//start with the panel closed
		CloseCutsceneDisplayPanel();

	}
	
	// Update is called once per frame
	void Update () {

		//do a switch case on the current animation state
		switch (CurrentAnimationState) {

		case AnimationState.EnteringArena:

			//check if this is the first time through the animation state
			if (isNewAnimationState == true) {

				//set the attacking unit destination location
				attackingUnitDestination = new Vector3 (attackingUnitStartingXPosition,
					attackingUnitParent.transform.localPosition.y,
					attackingUnitParent.transform.localPosition.z);

				//set the targeted unit destination location
				targetedUnitDestination = new Vector3 (targetedUnitStartingXPosition,
					targetedUnitParent.transform.localPosition.y,
					targetedUnitParent.transform.localPosition.z);

				//clear the display flags
				attackingUnitLabel.text = ("");
				targetedUnitLabel.text = ("");
				statusLabel.text = ("");

				//clear the hasArrived flags
				attackingUnitHasArrived = false;
				targetedUnitHasArrived = false;	

				//center the camera on the unit
				CenterCameraOnUnit(combatTargetedUnit);
				
				//clear the first time flag
				isNewAnimationState = false;

			}

			//check if the current animation state elapsed timer is greater than the attacking unit start time
			if (currentAnimationStateTimer >= attackingUnitStartTime) {

				//check if the attacking unit has arrived
				if (attackingUnitHasArrived == false) {
					
					//if it has not arrived, we should be moving the attacking unit towards the destination
					attackingUnitParent.transform.localPosition = Vector3.MoveTowards (attackingUnitParent.transform.localPosition, 
						attackingUnitDestination,
						enteringSpeed * Time.deltaTime);

					//check if the attacking unit is close enough to the finish to snap it there
					if (Vector3.Distance (attackingUnitParent.transform.localPosition, attackingUnitDestination) < .1f) {

						//if they are close, teleport to destination
						attackingUnitParent.transform.localPosition = attackingUnitDestination;

						//set the display text once the unit arrives
						if (combatAttackingUnit.GetComponent<Ship> () == true) {
							
							attackingUnitLabel.text = (Ship.GetShipTypeFromUnitType(combatAttackingUnit.unitType) + " " +
								combatAttackingUnit.GetComponent<Ship> ().shipName);

							//update the font size if necessary
							UIManager.AutoSizeTextMeshFont(attackingUnitLabel);

						} else {

							attackingUnitLabel.text = ("Starbase " + combatAttackingUnit.GetComponent<Starbase> ().baseName);

							//update the font size if necessary
							UIManager.AutoSizeTextMeshFont(attackingUnitLabel);

						}

						//set the label text color
						switch (combatAttackingUnit.owner.color) {

						case Player.Color.Green:

							attackingUnitLabel.color = GameManager.greenColor;
							break;

						case Player.Color.Purple:

							attackingUnitLabel.color = GameManager.purpleColor;
							break;

						case Player.Color.Red:

							attackingUnitLabel.color = GameManager.redColor;
							break;

						case Player.Color.Blue:

							attackingUnitLabel.color = GameManager.blueColor;
							break;

						default:
							break;

						}

						//set the hasArrived flag to true
						attackingUnitHasArrived = true;

					}

				}

			}

			//check if the current animation state elapsed timer is greater than the targeted unit start time
			if (currentAnimationStateTimer >= targetedUnitStartTime) {

				//check if the targeted unit has arrived
				if (targetedUnitHasArrived == false) {
					
					//if it has not arrived, we should be moving the targeted unit towards the destination
					targetedUnitParent.transform.localPosition = Vector3.MoveTowards (targetedUnitParent.transform.localPosition, 
						targetedUnitDestination,
						enteringSpeed * Time.deltaTime);

					//check if the targeted unit is close enough to the finish to snap it there
					if (Vector3.Distance (targetedUnitParent.transform.localPosition, targetedUnitDestination) < .1f) {

						//if they are close, teleport to destination
						targetedUnitParent.transform.localPosition = targetedUnitDestination;

						//set the display text once the unit arrives
						if (combatTargetedUnit.GetComponent<Ship> () == true) {

							targetedUnitLabel.text = (Ship.GetShipTypeFromUnitType(combatTargetedUnit.unitType) + " " + 
								combatTargetedUnit.unitName);

							//update the font size if necessary
							UIManager.AutoSizeTextMeshFont(targetedUnitLabel);

						} else {

							targetedUnitLabel.text = ("Starbase " + combatTargetedUnit.GetComponent<Starbase> ().baseName);

							//update the font size if necessary
							UIManager.AutoSizeTextMeshFont(targetedUnitLabel);

						}

						//set the label text color
						switch (combatTargetedUnit.owner.color) {

						case Player.Color.Green:

							targetedUnitLabel.color = GameManager.greenColor;
							break;

						case Player.Color.Purple:

							targetedUnitLabel.color = GameManager.purpleColor;
							break;

						case Player.Color.Red:

							targetedUnitLabel.color = GameManager.redColor;
							break;

						case Player.Color.Blue:

							targetedUnitLabel.color = GameManager.blueColor;
							break;

						default:
							break;

						}

						//set the hasArrived flag to true
						targetedUnitHasArrived = true;

					}

				}

			}

			//check if both units have arrived
			if (attackingUnitHasArrived == true && targetedUnitHasArrived == true) {

				//if both have arrived, we can move ahead to the WaitingAfterEntering animationState
				CurrentAnimationState = AnimationState.WaitingAfterEntering;

			}

			//increment the timer
			currentAnimationStateTimer += Time.deltaTime;

			break;


		case AnimationState.WaitingAfterEntering:

			//check if this is the first time through the animation state
			if (isNewAnimationState == true) {

				//clear the first time flag
				isNewAnimationState = false;

				//check if this is a phasor attack
				switch (combatAttackType) { 
			
				case CombatManager.AttackType.Phasor:

					//check if the attacking unit has the x-ray upgrade
					if (combatAttackingUnit.GetComponent<Ship> () == true) {

						//check for upgrade
						if (combatAttackingUnit.GetComponent<PhasorSection> ().xRayKernalUpgrade == true) {

							//invoke the x-ray charge event
							OnChargeXRay.Invoke ();

						} else {

							//invoke the normal phasor charge event
							OnChargePhasors.Invoke();

						}

					} else if (combatAttackingUnit.GetComponent<Starbase> () == true) {

						//check for upgrade
						if (combatAttackingUnit.GetComponent<StarbasePhasorSection2> ().xRayKernalUpgrade == true) {

							//invoke the x-ray charge event
							OnChargeXRay.Invoke ();

						} else {

							//invoke the normal phasor charge event
							OnChargePhasors.Invoke();

						}

					}

					break;

				case CombatManager.AttackType.LightTorpedo:

					//invoke the torpedo charge event
					OnChargeLightTorpedo.Invoke ();

					break;

				case CombatManager.AttackType.HeavyTorpedo:

					//invoke the torpedo charge event
					OnChargeHeavyTorpedo.Invoke ();
					break;

				default:

					break;

				}

			}

			//check if the current animation state elapsed timer is greater than the wait time
			if (currentAnimationStateTimer >= bothArrivedWaitTime) {

				//if we have waited long enough, we need to move to the next animation state, based on the attack type
				//check the attack type
				switch (combatAttackType) {

				case CombatManager.AttackType.Phasor:

					CurrentAnimationState = AnimationState.FiringPhasors;

					break;

				case CombatManager.AttackType.LightTorpedo:

					CurrentAnimationState = AnimationState.FiringTorpedos;
					break;

				case CombatManager.AttackType.HeavyTorpedo:

					CurrentAnimationState = AnimationState.FiringTorpedos;

					break;

				default:

					//close the cutscene display - something is wrong
					CloseCutsceneDisplayPanel();
					Debug.LogError ("Don't have a valid attack type at WaitingAfterEntering");

					break;

				}

			}

			//increment the timer
			currentAnimationStateTimer += Time.deltaTime;

			break;


		case AnimationState.FiringPhasors:

			//check if this is the first time through the animation state
			if (isNewAnimationState == true) {

				//if it is a new animation, start by setting the start point to the cached start point
				//and the end point to the start point
				for (int i = 0; i < phasorLineRenderer.Length; i++) {

					phasorLineRenderer [i].Points [0] = phasorLineStartPoints [i];
					phasorLineRenderer [i].Points [1] = phasorLineRenderer [i].Points [0];

					//set dirty so the renderer redraws them
					phasorLineRenderer [i].SetAllDirty ();

					//set the start point percentages to zero
					percentPhasorLineStartRendered [i] = 0.0f;

					//set the end point percentages to zero
					percentPhasorLineEndRendered [i] = 0.0f;

					//check if the attacking unit is a ship
					if (combatAttackingUnit.GetComponent<Ship> () == true) {

						//check if the attacking unit has an x-ray kernel
						if (combatAttackingUnit.GetComponent<PhasorSection> ().xRayKernalUpgrade == true) {

							//set the line renderer color to the x-ray upgrade color
							phasorLineRenderer [i].color = xRayPhasorColor;

							hasXRayKernel = true;

						} else {

							//the else condition is there is no x-ray upgrade
							//set the line renderer color to the normal color
							phasorLineRenderer [i].color = normalPhasorColor;

							hasXRayKernel = false;

						}

					} else {

						//the else condition is that the attacking unit is a starbase
						//check if the attacking unit has an x-ray kernel
						if (combatAttackingUnit.GetComponent<StarbasePhasorSection2> ().xRayKernalUpgrade == true) {

							//set the line renderer color to the x-ray upgrade color
							phasorLineRenderer [i].color = xRayPhasorColor;

							hasXRayKernel = true;

						} else {

							//the else condition is there is no x-ray upgrade
							//set the line renderer color to the normal color
							phasorLineRenderer [i].color = normalPhasorColor;

							hasXRayKernel = false;

						}

					}

				}

				//clear the first time flag
				isNewAnimationState = false;

			}

			//we need to loop through each phasor line and determine which ones should be moving
			for (int i = 0; i < phasorLineRenderer.Length; i++) {

				//check if the phasor start time is less than or equal to the elapsed time
				if (currentAnimationStateTimer >= phasorLineStartTimes [i]) {

					//activate the line renderer
					phasorLineRenderer [i].gameObject.SetActive (true);

					//check if this phasor line is just starting
					//we can tell because the end percentage will be zero
					if (percentPhasorLineEndRendered [i] < .001f) {

						//if it is just starting, spawn a muzzle blast
						//check for x ray kernel
						if (hasXRayKernel == true) {

							//create a x-ray muzzle animation at the start point
							UIAnimation.CreateUIAmination (prefabXRayMuzzle, phasorLinePulseLength, new Vector3 (3f, 3f, 3f),
								phasorLineStartPoints [i], explosionsParent.transform, true);

							//invoke the fire xray event
							OnFireXRay.Invoke ();

						} else {

							//create a phasor muzzle animation at the start point
							UIAnimation.CreateUIAmination (prefabPhasorMuzzle, phasorLinePulseLength, new Vector3 (2f, 2f, 2f),
								phasorLineStartPoints [i], explosionsParent.transform, true);

							//invoke the fire phasors event
							OnFirePhasors.Invoke ();

						}


					}

					//check if the elapsed time is long enough that this phasor line should stop
					//in other words, the start point starts going toward the end point
					if (currentAnimationStateTimer >= phasorLineStartTimes [i] + phasorLinePulseLength && percentPhasorLineStartRendered [i] < 1.0f) {

						//since we have exceeded the pulse length time, we need to be moving the start point
						//increment the percentage end rendered based on delta time
						percentPhasorLineStartRendered [i] += Time.deltaTime / timeForPhasorToReachTarget;

						//clamp the percentage to 1.0f
						percentPhasorLineStartRendered [i] = Mathf.Clamp (percentPhasorLineStartRendered [i], 0.0f, 1.0f);

					}

					//check if the end point percentage is less than 1
					if (percentPhasorLineEndRendered [i] < 1.0f) {

						//increment the percentage end rendered based on delta time
						percentPhasorLineEndRendered [i] += Time.deltaTime / timeForPhasorToReachTarget;

						//check if we are now at or over 1.0f
						if (percentPhasorLineEndRendered [i] >= 1.0f) {

							//this means we just reached the final end point this frame
							//clamp it to 1.0f
							percentPhasorLineEndRendered [i] = 1.0f;

							//check if this is a phasor hit animation
							if (isPhasorHit == true) {

								//check if the attacking unit has X-ray kernel
								if (hasXRayKernel == true) {

									//create a x-ray hit animation at the end point
									UIAnimation.CreateUIAmination (prefabXRayHit, 0.5f, new Vector3 (1f, 1f, 1f),
										phasorLineEndPoints [i], explosionsParent.transform, true);

									//invoke the hit event
									OnPhasorHit.Invoke();

								} else {
									
									//create a phasor hit animation at the end point
									UIAnimation.CreateUIAmination (prefabPhasorHit, 0.5f, new Vector3 (1f, 1f, 1f),
										phasorLineEndPoints [i], explosionsParent.transform, true);

									//invoke the hit event
									OnPhasorHit.Invoke();

								}

							}

						}

					}

					//update the line renderer start point positions
					phasorLineRenderer [i].Points [0] = phasorLineStartPoints [i] +
					(phasorLineEndPoints [i] - phasorLineStartPoints [i]) * percentPhasorLineStartRendered [i];

					//update the line renderer end point positions
					phasorLineRenderer [i].Points [1] = phasorLineStartPoints [i] +
					(phasorLineEndPoints [i] - phasorLineStartPoints [i]) * percentPhasorLineEndRendered [i];

					//set dirty so the renderer redraws them
					phasorLineRenderer [i].SetAllDirty ();

				}

			}

			//check if the last line start and end points are at 100% rendered.  If so, we can break out of the isFiring animation state
			if (percentPhasorLineStartRendered [phasorLineRenderer.Length - 1] == 1.0f && percentPhasorLineEndRendered [phasorLineRenderer.Length - 1] == 1.0f) {

				//check if this is a hit
				if (isPhasorHit == true) {

					//set the status text
					if (combatTargetedUnit.GetComponent<Starbase> () == true) {

						//convert the baseSectionTargeted to a string
						statusLabel.text = (combatDamage + " Damage to " + CombatManager.GetBaseSectionString (combatBaseSectionTargeted) + "!");
					
						//update the font size if necessary
						UIManager.AutoSizeTextMeshFont(statusLabel);

					} else {

						//the else is that the targeted unit is a ship
						//convert the shipSectionTargeted to a string
						statusLabel.text = (combatDamage + " Damage to " + CombatManager.GetShipSectionString (combatShipSectionTargeted) + "!");

						//update the font size if necessary
						UIManager.AutoSizeTextMeshFont(statusLabel);

					}

				} else {

					//set the status text
					statusLabel.text = ("Phasor Attack Misses!");

					//update the font size if necessary
					UIManager.AutoSizeTextMeshFont(statusLabel);

				}

				//check if the section is being destroyed
				if (combatWillDestroySection == true) {
					
					CurrentAnimationState = AnimationState.SectionExplosion;

				} else {

					//if the section is not to be destroyed, go straight to the preparing to close
					CurrentAnimationState = AnimationState.PreparingToClose;

				}

			}

			//increment the timer
			currentAnimationStateTimer += Time.deltaTime;

			break;

		case AnimationState.FiringTorpedos:

			//check if this is the first time through the animation state
			if (isNewAnimationState == true) {

				//set the torpedo parent position to the torpedoStartingPoint
				torpedoParent.transform.localPosition = torpedoStartingPosition;

				//check if this is a light or heavy torpedo attack
				if (combatAttackType == CombatManager.AttackType.LightTorpedo) {

					//spawn a light torpedo animation attached to the parent
					UIAnimation.CreateUIAmination (prefabLightTorpedo, .25f, new Vector3 (.75f, .75f, .75f), Vector3.zero, torpedoParent.transform, false);

					//spawn a torpedo muzzle blast animation
					UIAnimation.CreateUIAmination (prefabTorpedoMuzzle, .5f, new Vector3 (4f, 4f, 4f), torpedoStartingPosition, explosionsParent.transform, true);

					//invoke the fire torpedo event
					OnFireLightTorpedo.Invoke();

				} else if (combatAttackType == CombatManager.AttackType.HeavyTorpedo) {

					//spawn a heavy torpedo animation attached to the parent
					UIAnimation.CreateUIAmination (prefabHeavyTorpedo, .25f, new Vector3 (.75f, .75f, .75f), Vector3.zero, torpedoParent.transform, false);

					//spawn a torpedo muzzle blast animation
					UIAnimation.CreateUIAmination (prefabTorpedoMuzzle, .5f, new Vector3 (4f, 4f, 4f), torpedoStartingPosition, explosionsParent.transform, true);

					//invoke the fire torpedo event
					OnFireHeavyTorpedo.Invoke();

				}

				//check if there should be flares as part of this animation
				if (flaresUsed == true) {

					//if flares were used, we need to set the flare destinations
					SetFlarePositions(numberFlaresUsed,flareClusterPoint);

					//calculate the flare spawn point
					SetFlareSpawnPoint(combatTargetedUnit);

					//initialize the flaresArrived array size.  It will default to all falses
					flareArrived = new bool[numberFlaresUsed];

				}

				//clear the has arrived flag
				torpedoHasArrived = false;

				//clear the flag
				isNewAnimationState = false;

			}

			//we need to move the torpedo towards it's destination, if it has not already arrived
			if (torpedoHasArrived == false) {
				
				//if it has not arrived, we should be moving the torpedo parent towards the destination
				torpedoParent.transform.localPosition = Vector3.MoveTowards (torpedoParent.transform.localPosition, 
					torpedoDestinationPosition,
					torpedoSpeed * Time.deltaTime);


				//check if the torpedo is now close - if it is, we should teleport to the destination
				if (Vector3.Distance (torpedoParent.transform.localPosition, torpedoDestinationPosition) < .1f) {

					//set the position to the destination
					torpedoParent.transform.localPosition = torpedoDestinationPosition;

					//set the hasArrived flag
					torpedoHasArrived = true;

					//despawn the torpedo animation
					GameObject activeChild = null;

					for (int i = 0; i < torpedoParent.transform.childCount; i++) {

						if (torpedoParent.transform.GetChild (i).gameObject.activeSelf == true) {

							activeChild = torpedoParent.transform.GetChild (i).gameObject;

						}

					}

					if (activeChild != null) {
						
						SimplePool.Despawn (activeChild);

					}

					//check if the torpedo is a hit
					if (isTorpedoHit == true) {

						//check if this is a light torpedo attack
						if (combatAttackType == CombatManager.AttackType.LightTorpedo) {

							//play the light torpedo explosion
							UIAnimation.CreateUIAmination (prefabLightTorpedoHit, 0.5f, new Vector3 (1.5f, 1.5f, 1.5f),
								torpedoDestinationPosition, explosionsParent.transform, true);


						} else if (combatAttackType == CombatManager.AttackType.HeavyTorpedo) {

							//play the heavy torpedo explosion
							UIAnimation.CreateUIAmination (prefabHeavyTorpedoHit, 0.5f, new Vector3 (1.5f, 1.5f, 1.5f),
								torpedoDestinationPosition, explosionsParent.transform, true);

						}

						//set the status text
						if (combatTargetedUnit.GetComponent<Starbase> () == true) {

							//convert the baseSectionTargeted to a string
							statusLabel.text = (combatDamage + " Damage to " + CombatManager.GetBaseSectionString (combatBaseSectionTargeted) + "!");

							//update the font size if necessary
							UIManager.AutoSizeTextMeshFont(statusLabel);

						} else {

							//the else is that the targeted unit is a ship
							//convert the shipSectionTargeted to a string
							statusLabel.text = (combatDamage + " Damage to " + CombatManager.GetShipSectionString (combatShipSectionTargeted) + "!");

							//update the font size if necessary
							UIManager.AutoSizeTextMeshFont(statusLabel);
						}

					} else {

						//the else condition is that this is not a torpedo hit - it is a miss

						//check if there are flares used
						if (flaresUsed == false) {

							//set the status text
							statusLabel.text = ("Torpedo Attack Misses!");

							//update the font size if necessary
							UIManager.AutoSizeTextMeshFont(statusLabel);

						} else {
							
							//the else condition is that we did use flares
							//check if flares were successful
							if (flaresSuccessful == true) {

								//Debug.Log ("flares were successful");
								//check if this is a light torpedo attack
								if (combatAttackType == CombatManager.AttackType.LightTorpedo) {

									//play the light torpedo explosion
									UIAnimation.CreateUIAmination (prefabLightTorpedoHit, 0.5f, new Vector3 (1.5f, 1.5f, 1.5f),
										torpedoDestinationPosition, explosionsParent.transform, true);


								} else if (combatAttackType == CombatManager.AttackType.HeavyTorpedo) {

									//play the heavy torpedo explosion
									UIAnimation.CreateUIAmination (prefabHeavyTorpedoHit, 0.5f, new Vector3 (1.5f, 1.5f, 1.5f),
										torpedoDestinationPosition, explosionsParent.transform, true);

								}

								//despawn the flares
								for (int i = 0; i < flareParent.transform.childCount; i++) {

									if (flareParent.transform.GetChild (i).gameObject.activeSelf == true) {

										//if the flare child is active, despawn it
										SimplePool.Despawn(flareParent.transform.GetChild (i).gameObject);

									}

								}

								//set the status label
								statusLabel.text = ("Flare Intercepts Torpedo!");

								//update the font size if necessary
								UIManager.AutoSizeTextMeshFont(statusLabel);

							} else {
								
								//the else condition is that the flares were not successful, but the torpedo still missed

								//set the status text
								statusLabel.text = ("Torpedo Attack Misses!");

								//update the font size if necessary
								UIManager.AutoSizeTextMeshFont(statusLabel);

							}

						}

					}

					//advance the animation state
					//check if the section is being destroyed
					if (combatWillDestroySection == true) {

						CurrentAnimationState = AnimationState.SectionExplosion;

					} else {

						//if the section is not to be destroyed, go straight to the preparing to close
						CurrentAnimationState = AnimationState.PreparingToClose;

					}

				} 

				//check if the elapsed time has exceeded the flare launch time
				if (flaresUsed == true) {

					if (currentAnimationStateTimer >= flareSpawnTime) {

						//check if the flares have launched already
						if (flaresLaunched == false) {

							//spawn a flare for each flare used
							for (int i = 0; i < flareDestinationPosition.Length; i++) {

								//spawn the flares
								UIAnimation.CreateUIAmination (prefabFlare, .125f, new Vector3 (.25f, .25f, .25f), flareSpawnPoint, flareParent.transform, false);

							}

							//spawn the flare muzzle blast - we can use the torpedo muzzle prefab, but smaller
							UIAnimation.CreateUIAmination (prefabTorpedoMuzzle, .5f, new Vector3 (2f, 2f, 2f), flareSpawnPoint, explosionsParent.transform, true);

							//set the status text to show flares launched
							if (numberFlaresUsed == 1) {
								
								statusLabel.text = (numberFlaresUsed + " flare launched!");

								//update the font size if necessary
								UIManager.AutoSizeTextMeshFont(statusLabel);

							} else {

								statusLabel.text = (numberFlaresUsed + " flares launched!");

								//update the font size if necessary
								UIManager.AutoSizeTextMeshFont(statusLabel);

							}

							//set the spawned flag to true
							flaresLaunched = true;

						} else {

							//the else condition is that the flares have already launched
							//check if they have all arrived, by seeing if the flareArrived array contains a false
							if (System.Array.Exists (flareArrived, element => element == false)) {

								int activeLoopCount = 0;

								//get an array of the active flare children
								for (int i = 0; i < flareParent.transform.childCount; i++) {

									if (flareParent.transform.GetChild (i).gameObject.activeSelf == true) {

										//this is an active child
										flareParent.transform.GetChild (i).localPosition = Vector3.MoveTowards (flareParent.transform.GetChild (i).localPosition, 
											flareDestinationPosition [activeLoopCount],
											flareSpeed * Time.deltaTime); 

										//check if the flare is now close - if it is, we should teleport to the destination
										if (Vector3.Distance (flareParent.transform.GetChild (i).localPosition, flareDestinationPosition [activeLoopCount]) < .1f) {

											//set the position to the destination
											flareParent.transform.GetChild (i).localPosition = flareDestinationPosition [activeLoopCount];

											//set the arrived flag
											flareArrived [activeLoopCount] = true;

										}

										activeLoopCount++;

									}

								}

							} else {

								//the else condition is that all of the flares have arrived at their location
								//in this case, we need to check if the flares are supposed to be successful or not
								if (flaresSuccessful == true) {

									//if the flare is successful, we will reroute the torpedo to hit a flare
									//check if we've already rerouted the flare
									if (torpedoReroutedToFlare == false) {

										//reroute the torpedo
										torpedoDestinationPosition = flareDestinationPosition [Random.Range (0, flareDestinationPosition.Length)];

										//set the reroute flag to true
										torpedoReroutedToFlare = true;

									}

								} else {

									//the else is that the flares are unsuccessful
									//check if the torpedo has moved past all the flares
									if (torpedoPastAllFlares == false) {

										float closestFlareX = 0f;

										//get an array of the active flare children
										for (int i = 0; i < flareParent.transform.childCount; i++) {

											if (flareParent.transform.GetChild (i).gameObject.activeSelf == true) {

												//this is an active child

												//check if the x is greater than the closest flare x
												if (flareParent.transform.GetChild (i).transform.localPosition.x > closestFlareX) {

													//set the closest X
													closestFlareX = flareParent.transform.GetChild (i).transform.localPosition.x;

												}

											}

										}

										//the + 30 means that the torpedo has to be a bit past the flare before
										//it gets announced as a miss
										if (torpedoParent.transform.localPosition.x > closestFlareX + 30) {

											//the torpedo has moved past all the flares
											//we can now tell the status box the flares have missed
											statusLabel.text = ("Flares Unsuccessful!");

											//update the font size if necessary
											UIManager.AutoSizeTextMeshFont(statusLabel);

											//set the flag
											torpedoPastAllFlares = true;

										}

									}

								}

							}

						}

					}

				}

			} else {

				//if the torpedo has arrived, advance the animation state
				//check if the section is being destroyed
				if (combatWillDestroySection == true) {

					CurrentAnimationState = AnimationState.SectionExplosion;

				} else {

					//if the section is not to be destroyed, go straight to the preparing to close
					CurrentAnimationState = AnimationState.PreparingToClose;

				}

			}

			//increment the timer
			currentAnimationStateTimer += Time.deltaTime;

			break;

		case AnimationState.SectionExplosion:

			//check if this is the first time through the animation state
			if (isNewAnimationState == true) {

				//check if the targeted unit is a base
				if (combatTargetedUnit.unitType == CombatUnit.UnitType.Starbase) {

					//populate the section explosion
					SetSectionDestroyedExplosionPoints (combatTargetedUnit, (CombatManager.ShipSectionTargeted)combatBaseSectionTargeted);

				} else {

					//the else condition is we have a ship targeted
					//populate the section explosion
					SetSectionDestroyedExplosionPoints (combatTargetedUnit, combatShipSectionTargeted);

				}

				//set the cooldown array size
				explosionCooldown = new float[sectionDestroyedExplosions.Length];

				//randomly seed the cooldown array
				for (int i = 0; i < explosionCooldown.Length; i++) {

					explosionCooldown [i] = Random.Range (0, explosionCooldownThreshold);

				}

				//set the updated graphic flag
				updatedDestroyedGraphic = false;

				//clear the flag
				isNewAnimationState = false;

			}

			//check if the elapsed time is greater than the explosion start time
			if (currentAnimationStateTimer >= explosionStartTime && currentAnimationStateTimer < explosionStartTime + explosionStartDuration) {

				//start setting off explosions
				for (int i = 0; i < sectionDestroyedExplosions.Length; i++) {

					//randomly determine if we should set off an explosion
					if (Random.Range (0, 2) == 0) {

						//check if the cooldown is less than the threshold
						if (explosionCooldown [i] < explosionCooldownThreshold) {

							//create an explosion
							UIAnimation.CreateUIAmination (prefabSectionExplostion, explosionAnimationDuration, new Vector3 (1f, 1f, 1f), sectionDestroyedExplosions [i], explosionsParent.transform, true);

							//invoke the event
							OnCreateExplosion.Invoke ();

							//add a threshold to the cooldown
							explosionCooldown[i] += explosionCooldownThreshold;

						}

					}

					//subtract delta time from the cooldown
					explosionCooldown[i] -= Time.deltaTime;

				}

			} else if (currentAnimationStateTimer >= explosionStartTime + explosionStartDuration && 
				currentAnimationStateTimer < explosionStartTime + explosionStartDuration + explosionAnimationDuration) {

				//check if we have updated the graphic already
				if (updatedDestroyedGraphic == false) {

					//update the graphic to the destroyed graphic
					//check if the targetedUnit is a base
					if (combatTargetedUnit.unitType == CombatUnit.UnitType.Starbase) {

						//pass the base section targeted
						//update the graphic to the destroyed graphic
						InsertDestroyedSectionGraphic (combatTargetedUnit, (CombatManager.ShipSectionTargeted)combatBaseSectionTargeted);

						//update the status label
						statusLabel.text = (CombatManager.GetBaseSectionString(combatBaseSectionTargeted) + " Destroyed!!");

						//update the font size if necessary
						UIManager.AutoSizeTextMeshFont(statusLabel);

					} else {

						//the else condition is it is a ship
						//update the graphic to the destroyed graphic
						InsertDestroyedSectionGraphic (combatTargetedUnit, combatShipSectionTargeted);

						//update the status label
						statusLabel.text = (CombatManager.GetShipSectionString(combatShipSectionTargeted) + " Destroyed!!");

						//update the font size if necessary
						UIManager.AutoSizeTextMeshFont(statusLabel);

					}

					//set the flag
					updatedDestroyedGraphic = true;

				}

			} else if (currentAnimationStateTimer >= explosionStartTime + explosionStartDuration + explosionAnimationDuration) {

				//all explosions should be done at this point
				//we can advance the animation state
				//check if the unit should be destroyed
				if (combatWillDestroyUnit == true) {
					
					CurrentAnimationState = AnimationState.UnitExplosion;

				} else {

					CurrentAnimationState = AnimationState.PreparingToClose;

				}



			}

			//increment the timer
			currentAnimationStateTimer += Time.deltaTime;

			break;

		case AnimationState.UnitExplosion:

			//check if this is the first time through the animation state
			if (isNewAnimationState == true) {

				//set the detonated flag to false
				detonatedUnitExplosion = false;

				//set the detonated flag to false
				//detonatedUnitSmallExplosion = false;

				//set the small explosion positions
				SetUnitDestroyedExplosionPoints(combatTargetedUnit);

				//set the cooldown array size
				explosionCooldown = new float[unitDestroyedExplosions.Count];

				//randomly seed the cooldown array
				for (int i = 0; i < explosionCooldown.Length; i++) {

					explosionCooldown [i] = Random.Range (0, explosionCooldownThreshold);

				}

				//clear the flag
				isNewAnimationState = false;

			}

			//check if elapsed time is greater than small explosion animation start time
			if (currentAnimationStateTimer >= unitSmallExplosionStartTime && currentAnimationStateTimer < unitExplosionStartTime) {

				/*
				//check if we have already detonated the small unit explosion
				if (detonatedUnitSmallExplosion == false) {

					foreach (Vector2 vector2 in unitDestroyedExplosions) {

						//create an explosion
						UIAnimation.CreateUIAmination (prefabSectionExplostion, explosionAnimationDuration, new Vector3 (1f, 1f, 1f), vector2, explosionsParent.transform, true);

					}

					//set the detonated to true
					detonatedUnitSmallExplosion = true;

				}
				*/

				//start setting off explosions
				for (int i = 0; i < unitDestroyedExplosions.Count; i++) {

					//randomly determine if we should set off an explosion
					if (Random.Range (0, 2) == 0) {

						//check if the cooldown is less than the threshold
						if (explosionCooldown [i] < explosionCooldownThreshold) {

							//create an explosion
							UIAnimation.CreateUIAmination (prefabSectionExplostion, explosionAnimationDuration, new Vector3 (1f, 1f, 1f), unitDestroyedExplosions [i], explosionsParent.transform, true);

							//add a threshold to the cooldown
							explosionCooldown [i] += explosionCooldownThreshold;

						}

					}

					//subtract delta time from the cooldown
					explosionCooldown [i] -= Time.deltaTime;

				}

			}

			//check if elapsed time is greater than animation start time
			if (currentAnimationStateTimer >= unitExplosionStartTime) {

				//check if we have already detonated the unit explosion
				if (detonatedUnitExplosion == false) {

					//delete the graphics
					RemoveAllTargetedUnitGraphics();

					//set off an big explosion
					UIAnimation.CreateUIAmination (prefabShipExplosion, unitExplosionAnimationDuration, new Vector3 (4f, 4f, 4f), targetedUnitParent.transform.localPosition, explosionsParent.transform, true);

					//set the detonated to true
					detonatedUnitExplosion = true;

					//update the status label
					//check if the unit is a starbase
					if (combatTargetedUnit.unitType == CombatUnit.UnitType.Starbase) {

						statusLabel.text = (combatTargetedUnit.GetComponent<Starbase> ().baseName + " Completely Destroyed!!");

						//update the font size if necessary
						UIManager.AutoSizeTextMeshFont(statusLabel);

					} else {

						//the else condition is that we have a ship
						statusLabel.text = (combatTargetedUnit.GetComponent<Ship> ().shipName + " Completely Destroyed!!");

						//update the font size if necessary
						UIManager.AutoSizeTextMeshFont(statusLabel);

					}

				}

			}

			//check if the elapsed time is enough that the explosion should be done
			if (currentAnimationStateTimer >= unitExplosionStartTime + unitExplosionAnimationDuration) {

				//advance the animations state
				CurrentAnimationState = AnimationState.PreparingToClose;

			}

			//increment the timer
			currentAnimationStateTimer += Time.deltaTime;

			break;

		case AnimationState.PreparingToClose:

			//check if this is the first time through the animation state
			if (isNewAnimationState == true) {


				//clear the flag
				isNewAnimationState = false;

			}

			//check if the current animation state elapsed timer is greater than the wait time
			if (currentAnimationStateTimer >= closePanelWaitTime) {

				//we need to fire off events for the combat
				//switch case on attack type
				switch (combatAttackType) {

				case CombatManager.AttackType.Phasor:

					//check if the targeted unit is a base
					if (combatTargetedUnit.unitType == CombatUnit.UnitType.Starbase) {

						//check if it is a hit or miss
						if (isPhasorHit == true) {

							//switch case on section targeted
							switch (combatBaseSectionTargeted) {

							case CombatManager.BaseSectionTargeted.PhasorSection1:

								OnPhasorHitBasePhasorSection1.Invoke (combatAttackingUnit, combatTargetedUnit, combatDamage);
								break;

							case CombatManager.BaseSectionTargeted.PhasorSection2:

								OnPhasorHitBasePhasorSection2.Invoke (combatAttackingUnit, combatTargetedUnit, combatDamage);
								break;

							case CombatManager.BaseSectionTargeted.TorpedoSection:

								OnPhasorHitBaseTorpedoSection.Invoke (combatAttackingUnit, combatTargetedUnit, combatDamage);
								break;

							case CombatManager.BaseSectionTargeted.CrewSection:

								OnPhasorHitBaseCrewSection.Invoke (combatAttackingUnit, combatTargetedUnit, combatDamage);
								break;

							case CombatManager.BaseSectionTargeted.StorageSection1:

								OnPhasorHitBaseStorageSection1.Invoke (combatAttackingUnit, combatTargetedUnit, combatDamage);
								break;

							case CombatManager.BaseSectionTargeted.StorageSection2:

								OnPhasorHitBaseStorageSection2.Invoke (combatAttackingUnit, combatTargetedUnit, combatDamage);
								break;

							default:

								break;

							}

						} else {

							//the else condition is it is a phasor miss
							OnPhasorMissBase.Invoke (combatAttackingUnit, combatTargetedUnit, 0);

						}

					} else {

						//the else condition here is that the targeted unit is not a base, so it is a ship

						//check if it is a hit or miss
						if (isPhasorHit == true) {

							//switch case on section targeted
							switch (combatShipSectionTargeted) {

							case CombatManager.ShipSectionTargeted.PhasorSection:

								OnPhasorHitShipPhasorSection.Invoke (combatAttackingUnit, combatTargetedUnit, combatDamage);
								break;

							case CombatManager.ShipSectionTargeted.TorpedoSection:

								OnPhasorHitShipTorpedoSection.Invoke (combatAttackingUnit, combatTargetedUnit, combatDamage);
								break;

							case CombatManager.ShipSectionTargeted.StorageSection:

								OnPhasorHitShipStorageSection.Invoke (combatAttackingUnit, combatTargetedUnit, combatDamage);
								break;

							case CombatManager.ShipSectionTargeted.CrewSection:

								OnPhasorHitShipCrewSection.Invoke (combatAttackingUnit, combatTargetedUnit, combatDamage);
								break;

							case CombatManager.ShipSectionTargeted.EngineSection:

								OnPhasorHitShipEngineSection.Invoke (combatAttackingUnit, combatTargetedUnit, combatDamage);
								break;

							default:

								break;

							}

						} else {

							//the else condition is that it is a phasor miss
							OnPhasorMissShip.Invoke (combatAttackingUnit, combatTargetedUnit, 0);

						}

					}

					break;

				case CombatManager.AttackType.LightTorpedo:

					//check if flares were used
					if (flaresUsed == true) {

						//check if the flare was successful
						if (flaresSuccessful == true) {

							OnLightTorpedoFlareSuccess.Invoke (combatAttackingUnit, combatTargetedUnit, numberFlaresUsed);

						} else {

							//the else condition is that the flares were not successful
							OnLightTorpedoFlareFailure.Invoke (combatAttackingUnit, combatTargetedUnit, numberFlaresUsed);

						}

					}

					//check if the targeted unit is a base
					if (combatTargetedUnit.unitType == CombatUnit.UnitType.Starbase) {

						//check if it is a hit or miss
						if (isTorpedoHit == true) {

							//switch case on section targeted
							switch (combatBaseSectionTargeted) {

							case CombatManager.BaseSectionTargeted.PhasorSection1:

								OnLightTorpedoHitBasePhasorSection1.Invoke (combatAttackingUnit, combatTargetedUnit, combatDamage);
								break;

							case CombatManager.BaseSectionTargeted.PhasorSection2:

								OnLightTorpedoHitBasePhasorSection2.Invoke (combatAttackingUnit, combatTargetedUnit, combatDamage);
								break;

							case CombatManager.BaseSectionTargeted.TorpedoSection:

								OnLightTorpedoHitBaseTorpedoSection.Invoke (combatAttackingUnit, combatTargetedUnit, combatDamage);
								break;

							case CombatManager.BaseSectionTargeted.CrewSection:

								OnLightTorpedoHitBaseCrewSection.Invoke (combatAttackingUnit, combatTargetedUnit, combatDamage);
								break;

							case CombatManager.BaseSectionTargeted.StorageSection1:

								OnLightTorpedoHitBaseStorageSection1.Invoke (combatAttackingUnit, combatTargetedUnit, combatDamage);
								break;

							case CombatManager.BaseSectionTargeted.StorageSection2:

								OnLightTorpedoHitBaseStorageSection2.Invoke (combatAttackingUnit, combatTargetedUnit, combatDamage);
								break;

							default:

								break;

							}

						} else {

							//the else condition is that the torpedo missed
							OnLightTorpedoMissBase.Invoke (combatAttackingUnit, combatTargetedUnit, 0);

						}

					} else {

						//the else condition is that the targeted unit is not a base, it is a ship

						//check if it is a hit or miss
						if (isTorpedoHit == true) {

							//switch case on section targeted
							switch (combatShipSectionTargeted) {

							case CombatManager.ShipSectionTargeted.PhasorSection:

								OnLightTorpedoHitShipPhasorSection.Invoke (combatAttackingUnit, combatTargetedUnit, combatDamage);
								break;

							case CombatManager.ShipSectionTargeted.TorpedoSection:

								OnLightTorpedoHitShipTorpedoSection.Invoke (combatAttackingUnit, combatTargetedUnit, combatDamage);
								break;

							case CombatManager.ShipSectionTargeted.StorageSection:

								OnLightTorpedoHitShipStorageSection.Invoke (combatAttackingUnit, combatTargetedUnit, combatDamage);
								break;

							case CombatManager.ShipSectionTargeted.CrewSection:

								OnLightTorpedoHitShipCrewSection.Invoke (combatAttackingUnit, combatTargetedUnit, combatDamage);
								break;

							case CombatManager.ShipSectionTargeted.EngineSection:

								OnLightTorpedoHitShipEngineSection.Invoke (combatAttackingUnit, combatTargetedUnit, combatDamage);
								break;

							default:

								break;

							}

						} else {

							//the else condition is that it is a phasor miss
							OnLightTorpedoMissShip.Invoke (combatAttackingUnit, combatTargetedUnit, 0);

						}

					}

					break;

				case CombatManager.AttackType.HeavyTorpedo:

					//check if flares were used
					if (flaresUsed == true) {

						//check if the flare was successful
						if (flaresSuccessful == true) {

							OnHeavyTorpedoFlareSuccess.Invoke (combatAttackingUnit, combatTargetedUnit, numberFlaresUsed);

						} else {

							//the else condition is that the flares were not successful
							OnHeavyTorpedoFlareFailure.Invoke (combatAttackingUnit, combatTargetedUnit, numberFlaresUsed);

						}

					}

					//check if the targeted unit is a base
					if (combatTargetedUnit.unitType == CombatUnit.UnitType.Starbase) {

						//check if it is a hit or miss
						if (isTorpedoHit == true) {

							//switch case on section targeted
							switch (combatBaseSectionTargeted) {

							case CombatManager.BaseSectionTargeted.PhasorSection1:

								OnHeavyTorpedoHitBasePhasorSection1.Invoke (combatAttackingUnit, combatTargetedUnit, combatDamage);
								break;

							case CombatManager.BaseSectionTargeted.PhasorSection2:

								OnHeavyTorpedoHitBasePhasorSection2.Invoke (combatAttackingUnit, combatTargetedUnit, combatDamage);
								break;

							case CombatManager.BaseSectionTargeted.TorpedoSection:

								OnHeavyTorpedoHitBaseTorpedoSection.Invoke (combatAttackingUnit, combatTargetedUnit, combatDamage);
								break;

							case CombatManager.BaseSectionTargeted.CrewSection:

								OnHeavyTorpedoHitBaseCrewSection.Invoke (combatAttackingUnit, combatTargetedUnit, combatDamage);
								break;

							case CombatManager.BaseSectionTargeted.StorageSection1:

								OnHeavyTorpedoHitBaseStorageSection1.Invoke (combatAttackingUnit, combatTargetedUnit, combatDamage);
								break;

							case CombatManager.BaseSectionTargeted.StorageSection2:

								OnHeavyTorpedoHitBaseStorageSection2.Invoke (combatAttackingUnit, combatTargetedUnit, combatDamage);
								break;

							default:

								break;

							}

						} else {

							//the else condition is that the torpedo missed
							OnHeavyTorpedoMissBase.Invoke (combatAttackingUnit, combatTargetedUnit, 0);

						}

					} else {

						//the else condition is that the targeted unit is not a base, it is a ship

						//check if it is a hit or miss
						if (isTorpedoHit == true) {

							//switch case on section targeted
							switch (combatShipSectionTargeted) {

							case CombatManager.ShipSectionTargeted.PhasorSection:

								OnHeavyTorpedoHitShipPhasorSection.Invoke (combatAttackingUnit, combatTargetedUnit, combatDamage);
								break;

							case CombatManager.ShipSectionTargeted.TorpedoSection:

								OnHeavyTorpedoHitShipTorpedoSection.Invoke (combatAttackingUnit, combatTargetedUnit, combatDamage);
								break;

							case CombatManager.ShipSectionTargeted.StorageSection:

								OnHeavyTorpedoHitShipStorageSection.Invoke (combatAttackingUnit, combatTargetedUnit, combatDamage);
								break;

							case CombatManager.ShipSectionTargeted.CrewSection:

								OnHeavyTorpedoHitShipCrewSection.Invoke (combatAttackingUnit, combatTargetedUnit, combatDamage);
								break;

							case CombatManager.ShipSectionTargeted.EngineSection:

								OnHeavyTorpedoHitShipEngineSection.Invoke (combatAttackingUnit, combatTargetedUnit, combatDamage);
								break;

							default:

								break;

							}

						} else {

							//the else condition is that it is a phasor miss
							OnHeavyTorpedoMissShip.Invoke (combatAttackingUnit, combatTargetedUnit, 0);

						}

					}

					break;

				default:

					break;

				}

				//if this was a torpedo attack, clean up some data
				if (combatAttackType == CombatManager.AttackType.LightTorpedo || combatAttackType == CombatManager.AttackType.HeavyTorpedo) {

					//wipe out any old flare data
					flaresUsed = false;
					flaresSuccessful = false;
					numberFlaresUsed = 0;
					flaresLaunched = false;
					flareArrived = null;

					//despawn the flares
					for (int i = 0; i < flareParent.transform.childCount; i++) {

						if (flareParent.transform.GetChild (i).gameObject.activeSelf == true) {

							//if the flare child is active, despawn it
							SimplePool.Despawn (flareParent.transform.GetChild (i).gameObject);

						}

					}

					torpedoReroutedToFlare = false;
					torpedoPastAllFlares = false;

				}

				//set the animation state to inactive
				CurrentAnimationState = AnimationState.Inactive;

				//turn off the cutscene panel
				CloseCutsceneDisplayPanel();

				//create a tileMap explosion if the unit is destroyed
				//TODO - remove this if it works elsewhere
				/*
				if (combatWillDestroyUnit == true) {
					
					TileMapUnitExplosion.CreateTileMapUnitExplosion (prefabTileMapUnitExplosion, 
						tileMap.HexMap [combatTargetedUnit.currentLocation], 16, 16, 20.0f, new Vector3 (2.0f, 2.0f, 2.0f), tileMapAnimationParent.transform, true);

				}

				*/

			}

			//increment the timer
			currentAnimationStateTimer += Time.deltaTime;

			break;

		default:

			//if we have no animationState, do nothing

			break;

		}

		//force the canvas to redraw
		Canvas.ForceUpdateCanvases ();
		
	}

	//this function opens the display panel
	private void OpenCutsceneDisplayPanel(){

		cutsceneDisplay.SetActive (true);

		OnOpenCutsceneDisplayPanel.Invoke ();

	}

	//this function closese the display panel
	private void CloseCutsceneDisplayPanel(){

		cutsceneDisplay.SetActive (false);

		OnCloseCutsceneDisplayPanel.Invoke ();

	}

	//this function resolves a phasor attack into what the cutscene needs to know to process the animation
	private void ResolvePhasorHit(CombatUnit attackingUnit, CombatUnit targetedUnit, CombatManager.ShipSectionTargeted shipSectionTargeted, int damage){

		//this function will trigger from an event launched from combat manager where a ship is getting hit by a phasor attack
		//start by caching the combat data
		combatAttackingUnit = attackingUnit;
		combatTargetedUnit = targetedUnit;
		combatDamage = damage;
		combatAttackType = CombatManager.AttackType.Phasor;
		isPhasorHit = true;

		//set the initial sprites
		SetInitialCombatUnitSprites(attackingUnit, targetedUnit);

		//check if the targeted unit is a ship
		if (targetedUnit.GetComponent<Ship> () == true) {

			//take the section targeted as the shipSectionTargeted
			combatShipSectionTargeted = shipSectionTargeted;

			combatWillDestroySection = WillDestroyShipSection (targetedUnit, shipSectionTargeted, damage); 

			//if the section will be destroyed, we need to check if the entire ship will be destroyed too
			if (combatWillDestroySection == true) {

				combatWillDestroyUnit = WillDestroyEntireShip (targetedUnit, shipSectionTargeted);

			}

		} else {

			//the else condition is that the targeted unit is a starbase
			//cast the shipSectionTargeted as the cached baseSectionTargeted
			combatBaseSectionTargeted = (CombatManager.BaseSectionTargeted)shipSectionTargeted;

			combatWillDestroySection = WillDestroyBaseSection (targetedUnit, combatBaseSectionTargeted, damage); 

			//if the section will be destroyed, we need to check if the entire base will be destroyed too
			if (combatWillDestroySection == true) {

				combatWillDestroyUnit = WillDestroyEntireBase (targetedUnit, combatBaseSectionTargeted);

			}

		}

		//set the combat units vertical positions
		SetCombatUnitsVerticalPosition(attackingUnit,targetedUnit);

		//we will first need to determine the starting points and ending points in the phasor lines
		SetPhasorLineStartingPoints(attackingUnit);
		SetPhasorLineEndingPoints (targetedUnit, shipSectionTargeted);

		//add some randomness to the end points
		AddNoiseToPhasorLineEndPoints();

		//position the units off camera to start
		PositionCombatUnitsOffCamera();

		//because this is a hit, we want the phasor lines to render over the targeted unit sprites, so it should be below in the hierarchy
		phasorLinesHolder.transform.SetSiblingIndex(2);

		//set the current animation state
		CurrentAnimationState = AnimationState.EnteringArena;

		//show the cutscene panel
		OpenCutsceneDisplayPanel();

	}

	//this function resolves a phasor attack miss into what the cutscene needs to know to process the animation
	private void ResolvePhasorMiss(CombatUnit attackingUnit, CombatUnit targetedUnit){

		//start by caching the combat data
		combatAttackingUnit = attackingUnit;
		combatTargetedUnit = targetedUnit;
		combatAttackType = CombatManager.AttackType.Phasor;
		isPhasorHit = false;
		combatWillDestroySection = false;
		combatWillDestroyUnit = false;

		//set the initial sprites
		SetInitialCombatUnitSprites(attackingUnit, targetedUnit);

		//set the combat units vertical positions
		SetCombatUnitsVerticalPosition(attackingUnit,targetedUnit);

		//we know the outcome of the attack.  Now we need to set up the presentation.
		//we will use the UI Line Renderer for the phasor attack animation
		//we will first need to determine the starting points and ending points in the phasor lines
		SetPhasorLineStartingPoints(attackingUnit);

		//we need to pick a dummy section targeted, even though it's a miss.
		//I want to randomly pick one of the sections that is alive in the target
		CombatManager.ShipSectionTargeted dummySectionTargeted = randomSectionTargeted(targetedUnit);

		//cache the section targeted
		if (targetedUnit.GetComponent<Ship> () == true) {
			
			combatShipSectionTargeted = dummySectionTargeted;

		} else {

			//the else condition is that the targeted unit is a base
			combatBaseSectionTargeted = (CombatManager.BaseSectionTargeted)dummySectionTargeted;

		}

		//this will set the ending points to finish on the dummy section
		SetPhasorLineEndingPoints (targetedUnit, dummySectionTargeted);

		//add some randomness to the end points
		AddNoiseToPhasorLineEndPoints();

		//we now want to extend the end lines to the edge of the panel
		ExtendPhasorEndPointsToPanel();

		//position the units off camera to start
		PositionCombatUnitsOffCamera();

		//because this is a miss, we want the phasor lines to render below the targeted unit sprites, so it should be above in the hierarchy
		phasorLinesHolder.transform.SetSiblingIndex(1);

		//set the current animation state
		CurrentAnimationState = AnimationState.EnteringArena;

		//show the cutscene panel
		OpenCutsceneDisplayPanel();

	}

	//this function resolves a torpedo hit
	private void ResolveTorpedoHit(CombatUnit attackingUnit, CombatUnit targetedUnit, CombatManager.ShipSectionTargeted shipSectionTargeted, 
		CombatManager.AttackType attackType, int damage){

		//start by caching the combat data
		combatAttackingUnit = attackingUnit;
		combatTargetedUnit = targetedUnit;
		combatDamage = damage;
		combatAttackType = attackType;
		isTorpedoHit = true;

		//set the initial sprites
		SetInitialCombatUnitSprites(attackingUnit, targetedUnit);

		//check if the targeted unit is a ship
		if (targetedUnit.GetComponent<Ship> () == true) {

			//take the section targeted as the shipSectionTargeted
			combatShipSectionTargeted = shipSectionTargeted;

			combatWillDestroySection = WillDestroyShipSection (targetedUnit, shipSectionTargeted, damage); 

			//if the section will be destroyed, we need to check if the entire ship will be destroyed too
			if (combatWillDestroySection == true) {

				combatWillDestroyUnit = WillDestroyEntireShip (targetedUnit, shipSectionTargeted);

			}

		} else {

			//the else condition is that the targeted unit is a starbase
			//cast the shipSectionTargeted as the cached baseSectionTargeted
			combatBaseSectionTargeted = (CombatManager.BaseSectionTargeted)shipSectionTargeted;

			combatWillDestroySection = WillDestroyBaseSection (targetedUnit, combatBaseSectionTargeted, damage); 

			//if the section will be destroyed, we need to check if the entire base will be destroyed too
			if (combatWillDestroySection == true) {

				combatWillDestroyUnit = WillDestroyEntireBase (targetedUnit, combatBaseSectionTargeted);

			}

		}

		//set the combat units vertical positions
		SetCombatUnitsVerticalPosition(attackingUnit,targetedUnit);

		//set the torpedo starting point
		SetTorpedoStartingPosition (attackingUnit);

		//set the torpedo destination point
		SetTorpedoDestinationPosition (targetedUnit, shipSectionTargeted);

		//set the flare cluster point in case we do have flares
		SetFlareClusterPoint(torpedoStartingPosition,torpedoDestinationPosition);

		//position the units off camera to start
		//note that this needs to happen after we cache the starting and ending torpedo positions
		PositionCombatUnitsOffCamera();

		//add randomness to the torpedo end point
		AddNoiseToTorpedoDestinationPosition ();

		//set the current animation state
		CurrentAnimationState = AnimationState.EnteringArena;

		//show the cutscene panel
		OpenCutsceneDisplayPanel();

	}

	//this function resolves a torpedo miss
	private void ResolveTorpedoMiss(CombatUnit attackingUnit, CombatUnit targetedUnit, CombatManager.AttackType attackType){

		//start by caching the combat data
		combatAttackingUnit = attackingUnit;
		combatTargetedUnit = targetedUnit;
		combatAttackType = attackType;
		isTorpedoHit = false;
		combatWillDestroySection = false;
		combatWillDestroyUnit = false;

		//set the initial sprites
		SetInitialCombatUnitSprites(attackingUnit, targetedUnit);

		//we need to pick a dummy section targeted, even though it's a miss.
		//I want to randomly pick one of the sections that is alive in the target
		CombatManager.ShipSectionTargeted dummySectionTargeted = randomSectionTargeted(targetedUnit);

		//cache the section targeted
		if (targetedUnit.GetComponent<Ship> () == true) {

			combatShipSectionTargeted = dummySectionTargeted;

		} else {

			//the else condition is that the targeted unit is a base
			combatBaseSectionTargeted = (CombatManager.BaseSectionTargeted)dummySectionTargeted;

		}


		//set the combat units vertical positions
		SetCombatUnitsVerticalPosition(attackingUnit,targetedUnit);

		//set the torpedo starting point
		SetTorpedoStartingPosition (attackingUnit);

		//set the torpedo destination point
		SetTorpedoDestinationPosition (targetedUnit, dummySectionTargeted);

		//set the flare cluster point in case we do have flares
		SetFlareClusterPoint(torpedoStartingPosition,torpedoDestinationPosition);

		//we now want to extend the destination to the edge of the panel
		ExtendTorpedoDestinationToPanel();

		//position the units off camera to start
		//note that this needs to happen after we cache the starting and ending torpedo positions
		PositionCombatUnitsOffCamera();

		//add randomness to the torpedo end point
		AddNoiseToTorpedoDestinationPosition ();

		//set the current animation state
		CurrentAnimationState = AnimationState.EnteringArena;

		//show the cutscene panel
		OpenCutsceneDisplayPanel();

	}

	//this function resolves a torpedo flare intercept event
	private void ResolveTorpedoFlareIntercept(CombatUnit attackingUnit, CombatUnit targetedUnit, CombatManager.AttackType attackType,CombatManager.ShipSectionTargeted sectionTargeted){

		//start by caching the combat data
		combatAttackingUnit = attackingUnit;
		combatTargetedUnit = targetedUnit;
		combatAttackType = attackType;
		isTorpedoHit = false;
		combatWillDestroySection = false;
		combatWillDestroyUnit = false;

		//set the initial sprites
		SetInitialCombatUnitSprites(attackingUnit, targetedUnit);


		//set the combat units vertical positions
		SetCombatUnitsVerticalPosition(attackingUnit,targetedUnit);

		//set the torpedo starting point
		SetTorpedoStartingPosition (attackingUnit);

		//if the section targeted is untargeted, we need to choose a random section
		if (sectionTargeted == CombatManager.ShipSectionTargeted.Untargeted) {

			//we need to pick a dummy section targeted, even though it's a miss.
			//I want to randomly pick one of the sections that is alive in the target
			CombatManager.ShipSectionTargeted dummySectionTargeted = randomSectionTargeted (targetedUnit);

			//cache the section targeted
			if (targetedUnit.GetComponent<Ship> () == true) {

				combatShipSectionTargeted = dummySectionTargeted;

			} else {

				//the else condition is that the targeted unit is a base
				combatBaseSectionTargeted = (CombatManager.BaseSectionTargeted)dummySectionTargeted;

			}

			//set the torpedo destination point
			SetTorpedoDestinationPosition (targetedUnit, dummySectionTargeted);

		} else {

			//the else condition is that there is a valid section targeted.  We can use it in this case

			//set the torpedo destination point
			SetTorpedoDestinationPosition (targetedUnit, sectionTargeted);

		}

		//set the flare cluster point in case we do have flares
		SetFlareClusterPoint(torpedoStartingPosition,torpedoDestinationPosition);

		//we now want to extend the destination to the edge of the panel
		ExtendTorpedoDestinationToPanel();

		//position the units off camera to start
		//note that this needs to happen after we cache the starting and ending torpedo positions
		PositionCombatUnitsOffCamera();

		//add randomness to the torpedo end point
		AddNoiseToTorpedoDestinationPosition ();

		//set the current animation state
		CurrentAnimationState = AnimationState.EnteringArena;

		//show the cutscene panel
		OpenCutsceneDisplayPanel();

	}

	//this function sets the starting combat unit appearances depending on the attacking unit and targeted unit engaged in battle
	private void SetInitialCombatUnitSprites(CombatUnit attackingUnit, CombatUnit targetedUnit){

		//assign sprites for attacking unit
		AssignUnitSprites(attackingUnit, attackingUnitSections);

		//assign sprites for targeted unit
		AssignUnitSprites(targetedUnit, targetedUnitSections);

	}

	//this function takes a combat unit and a set of images and assigns the appropriate sprites based on the unit
	private void AssignUnitSprites(CombatUnit combatUnit, GameObject[] unitSections){

		//run a switch case on the attacking unit to determine sprite positions and scale
		switch (combatUnit.unitType) {

		case CombatUnit.UnitType.Starbase:

			//set the transform positions and scales
			for (int i = 0; i < 6; i++) {

				unitSections [i].transform.localPosition = starbaseSpritePosition [i];
				unitSections [i].transform.localScale = baseSpriteScale [i];

			}

			//set the active sections
			unitSections [0].SetActive (true);
			unitSections [1].SetActive (true);
			unitSections [2].SetActive (true);
			unitSections [3].SetActive (true);
			unitSections [4].SetActive (true);
			unitSections [5].SetActive (true);

			//to set the images, we need to know the owner color as well as whether the section is destroyed
			switch (combatUnit.owner.color) {

			case Player.Color.Green:

				if (combatUnit.GetComponent<StarbasePhasorSection1> ().isDestroyed == true) {

					unitSections [0].GetComponent<Image> ().sprite = greenBasePhasorSectionDestroyed;

				} else {

					unitSections [0].GetComponent<Image> ().sprite = greenBasePhasorSection;

				}


				if (combatUnit.GetComponent<StarbasePhasorSection2> ().isDestroyed == true) {

					unitSections [1].GetComponent<Image> ().sprite = greenBasePhasorSectionDestroyed;

				} else {

					unitSections [1].GetComponent<Image> ().sprite = greenBasePhasorSection;

				}


				if (combatUnit.GetComponent<StarbaseTorpedoSection> ().isDestroyed == true) {

					unitSections [2].GetComponent<Image> ().sprite = greenBaseMiddleSectionDestroyed;

				} else {

					unitSections [2].GetComponent<Image> ().sprite = greenBaseMiddleSection;

				}


				if (combatUnit.GetComponent<StarbaseCrewSection> ().isDestroyed == true) {

					unitSections [3].GetComponent<Image> ().sprite = greenBaseMiddleSectionDestroyed;

				} else {

					unitSections [3].GetComponent<Image> ().sprite = greenBaseMiddleSection;

				}


				if (combatUnit.GetComponent<StarbaseStorageSection1> ().isDestroyed == true) {

					unitSections [4].GetComponent<Image> ().sprite = greenBaseStorageSectionDestroyed;

				} else {

					unitSections [4].GetComponent<Image> ().sprite = greenBaseStorageSection;

				}

				if (combatUnit.GetComponent<StarbaseStorageSection2> ().isDestroyed == true) {

					unitSections [5].GetComponent<Image> ().sprite = greenBaseStorageSectionDestroyed;

				} else {

					unitSections [5].GetComponent<Image> ().sprite = greenBaseStorageSection;

				}

				break;


			case Player.Color.Purple:

				if (combatUnit.GetComponent<StarbasePhasorSection1> ().isDestroyed == true) {

					unitSections [0].GetComponent<Image> ().sprite = purpleBasePhasorSectionDestroyed;

				} else {

					unitSections [0].GetComponent<Image> ().sprite = purpleBasePhasorSection;

				}


				if (combatUnit.GetComponent<StarbasePhasorSection2> ().isDestroyed == true) {

					unitSections [1].GetComponent<Image> ().sprite = purpleBasePhasorSectionDestroyed;

				} else {

					unitSections [1].GetComponent<Image> ().sprite = purpleBasePhasorSection;

				}


				if (combatUnit.GetComponent<StarbaseTorpedoSection> ().isDestroyed == true) {

					unitSections [2].GetComponent<Image> ().sprite = purpleBaseMiddleSectionDestroyed;

				} else {

					unitSections [2].GetComponent<Image> ().sprite = purpleBaseMiddleSection;

				}


				if (combatUnit.GetComponent<StarbaseCrewSection> ().isDestroyed == true) {

					unitSections [3].GetComponent<Image> ().sprite = purpleBaseMiddleSectionDestroyed;

				} else {

					unitSections [3].GetComponent<Image> ().sprite = purpleBaseMiddleSection;

				}


				if (combatUnit.GetComponent<StarbaseStorageSection1> ().isDestroyed == true) {

					unitSections [4].GetComponent<Image> ().sprite = purpleBaseStorageSectionDestroyed;

				} else {

					unitSections [4].GetComponent<Image> ().sprite = purpleBaseStorageSection;

				}

				if (combatUnit.GetComponent<StarbaseStorageSection2> ().isDestroyed == true) {

					unitSections [5].GetComponent<Image> ().sprite = purpleBaseStorageSectionDestroyed;

				} else {

					unitSections [5].GetComponent<Image> ().sprite = purpleBaseStorageSection;

				}

				break;


			case Player.Color.Red:

				if (combatUnit.GetComponent<StarbasePhasorSection1> ().isDestroyed == true) {

					unitSections [0].GetComponent<Image> ().sprite = redBasePhasorSectionDestroyed;

				} else {

					unitSections [0].GetComponent<Image> ().sprite = redBasePhasorSection;

				}


				if (combatUnit.GetComponent<StarbasePhasorSection2> ().isDestroyed == true) {

					unitSections [1].GetComponent<Image> ().sprite = redBasePhasorSectionDestroyed;

				} else {

					unitSections [1].GetComponent<Image> ().sprite = redBasePhasorSection;

				}


				if (combatUnit.GetComponent<StarbaseTorpedoSection> ().isDestroyed == true) {

					unitSections [2].GetComponent<Image> ().sprite = redBaseMiddleSectionDestroyed;

				} else {

					unitSections [2].GetComponent<Image> ().sprite = redBaseMiddleSection;

				}


				if (combatUnit.GetComponent<StarbaseCrewSection> ().isDestroyed == true) {

					unitSections [3].GetComponent<Image> ().sprite = redBaseMiddleSectionDestroyed;

				} else {

					unitSections [3].GetComponent<Image> ().sprite = redBaseMiddleSection;

				}


				if (combatUnit.GetComponent<StarbaseStorageSection1> ().isDestroyed == true) {

					unitSections [4].GetComponent<Image> ().sprite = redBaseStorageSectionDestroyed;

				} else {

					unitSections [4].GetComponent<Image> ().sprite = redBaseStorageSection;

				}

				if (combatUnit.GetComponent<StarbaseStorageSection2> ().isDestroyed == true) {

					unitSections [5].GetComponent<Image> ().sprite = redBaseStorageSectionDestroyed;

				} else {

					unitSections [5].GetComponent<Image> ().sprite = redBaseStorageSection;

				}

				break;

			case Player.Color.Blue:

				if (combatUnit.GetComponent<StarbasePhasorSection1> ().isDestroyed == true) {

					unitSections [0].GetComponent<Image> ().sprite = blueBasePhasorSectionDestroyed;

				} else {

					unitSections [0].GetComponent<Image> ().sprite = blueBasePhasorSection;

				}


				if (combatUnit.GetComponent<StarbasePhasorSection2> ().isDestroyed == true) {

					unitSections [1].GetComponent<Image> ().sprite = blueBasePhasorSectionDestroyed;

				} else {

					unitSections [1].GetComponent<Image> ().sprite = blueBasePhasorSection;

				}


				if (combatUnit.GetComponent<StarbaseTorpedoSection> ().isDestroyed == true) {

					unitSections [2].GetComponent<Image> ().sprite = blueBaseMiddleSectionDestroyed;

				} else {

					unitSections [2].GetComponent<Image> ().sprite = blueBaseMiddleSection;

				}


				if (combatUnit.GetComponent<StarbaseCrewSection> ().isDestroyed == true) {

					unitSections [3].GetComponent<Image> ().sprite = blueBaseMiddleSectionDestroyed;

				} else {

					unitSections [3].GetComponent<Image> ().sprite = blueBaseMiddleSection;

				}


				if (combatUnit.GetComponent<StarbaseStorageSection1> ().isDestroyed == true) {

					unitSections [4].GetComponent<Image> ().sprite = blueBaseStorageSectionDestroyed;

				} else {

					unitSections [4].GetComponent<Image> ().sprite = blueBaseStorageSection;

				}

				if (combatUnit.GetComponent<StarbaseStorageSection2> ().isDestroyed == true) {

					unitSections [5].GetComponent<Image> ().sprite = blueBaseStorageSectionDestroyed;

				} else {

					unitSections [5].GetComponent<Image> ().sprite = blueBaseStorageSection;

				}

				break;

			default:

				unitSections [0].GetComponent<Image> ().sprite = null;
				unitSections [1].GetComponent<Image> ().sprite = null;
				unitSections [2].GetComponent<Image> ().sprite = null;
				unitSections [3].GetComponent<Image> ().sprite = null;
				unitSections [4].GetComponent<Image> ().sprite = null;
				unitSections [5].GetComponent<Image> ().sprite = null;
				break;

			}

			break;

		case CombatUnit.UnitType.Starship:

			//set the transform positions and scales
			for (int i = 0; i < 6; i++) {

				unitSections [i].transform.localPosition = starshipSpritePosition [i];
				unitSections [i].transform.localScale = shipSpriteScale [i];

			}

			//set the active sections
			unitSections [0].SetActive (true);
			unitSections [1].SetActive (true);
			unitSections [2].SetActive (true);
			unitSections [3].SetActive (true);
			unitSections [4].SetActive (true);
			unitSections [5].SetActive (false);

			//to set the images, we need to know the owner color as well as whether the section is destroyed
			switch (combatUnit.owner.color) {

			case Player.Color.Green:

				if (combatUnit.GetComponent<PhasorSection> ().isDestroyed == true) {

					unitSections [0].GetComponent<Image> ().sprite = greenPhasorSectionDestroyed;

				} else {

					unitSections [0].GetComponent<Image> ().sprite = greenPhasorSection;

				}

				if (combatUnit.GetComponent<TorpedoSection> ().isDestroyed == true) {

					unitSections [1].GetComponent<Image> ().sprite = greenTorpedoSectionDestroyed;

				} else {

					unitSections [1].GetComponent<Image> ().sprite = greenTorpedoSection;

				}

				if (combatUnit.GetComponent<StorageSection> ().isDestroyed == true) {

					unitSections [2].GetComponent<Image> ().sprite = greenStorageSectionDestroyed;

				} else {

					unitSections [2].GetComponent<Image> ().sprite = greenStorageSection;

				}

				if (combatUnit.GetComponent<CrewSection> ().isDestroyed == true) {

					unitSections [3].GetComponent<Image> ().sprite = greenCrewSectionDestroyed;

				} else {

					unitSections [3].GetComponent<Image> ().sprite = greenCrewSection;

				}

				if (combatUnit.GetComponent<EngineSection> ().isDestroyed == true) {

					unitSections [4].GetComponent<Image> ().sprite = greenEngineSectionDestroyed;

				} else {

					unitSections [4].GetComponent<Image> ().sprite = greenEngineSection;

				}

				unitSections [5].GetComponent<Image> ().sprite = null;

				break;

			case Player.Color.Purple:

				if (combatUnit.GetComponent<PhasorSection> ().isDestroyed == true) {

					unitSections [0].GetComponent<Image> ().sprite = purplePhasorSectionDestroyed;

				} else {

					unitSections [0].GetComponent<Image> ().sprite = purplePhasorSection;

				}

				if (combatUnit.GetComponent<TorpedoSection> ().isDestroyed == true) {

					unitSections [1].GetComponent<Image> ().sprite = purpleTorpedoSectionDestroyed;

				} else {

					unitSections [1].GetComponent<Image> ().sprite = purpleTorpedoSection;

				}

				if (combatUnit.GetComponent<StorageSection> ().isDestroyed == true) {

					unitSections [2].GetComponent<Image> ().sprite = purpleStorageSectionDestroyed;

				} else {

					unitSections [2].GetComponent<Image> ().sprite = purpleStorageSection;

				}

				if (combatUnit.GetComponent<CrewSection> ().isDestroyed == true) {

					unitSections [3].GetComponent<Image> ().sprite = purpleCrewSectionDestroyed;

				} else {

					unitSections [3].GetComponent<Image> ().sprite = purpleCrewSection;

				}

				if (combatUnit.GetComponent<EngineSection> ().isDestroyed == true) {

					unitSections [4].GetComponent<Image> ().sprite = purpleEngineSectionDestroyed;

				} else {

					unitSections [4].GetComponent<Image> ().sprite = purpleEngineSection;

				}

				unitSections [5].GetComponent<Image> ().sprite = null;

				break;

			case Player.Color.Red:

				if (combatUnit.GetComponent<PhasorSection> ().isDestroyed == true) {

					unitSections [0].GetComponent<Image> ().sprite = redPhasorSectionDestroyed;

				} else {

					unitSections [0].GetComponent<Image> ().sprite = redPhasorSection;

				}

				if (combatUnit.GetComponent<TorpedoSection> ().isDestroyed == true) {

					unitSections [1].GetComponent<Image> ().sprite = redTorpedoSectionDestroyed;

				} else {

					unitSections [1].GetComponent<Image> ().sprite = redTorpedoSection;

				}

				if (combatUnit.GetComponent<StorageSection> ().isDestroyed == true) {

					unitSections [2].GetComponent<Image> ().sprite = redStorageSectionDestroyed;

				} else {

					unitSections [2].GetComponent<Image> ().sprite = redStorageSection;

				}

				if (combatUnit.GetComponent<CrewSection> ().isDestroyed == true) {

					unitSections [3].GetComponent<Image> ().sprite = redCrewSectionDestroyed;

				} else {

					unitSections [3].GetComponent<Image> ().sprite = redCrewSection;

				}

				if (combatUnit.GetComponent<EngineSection> ().isDestroyed == true) {

					unitSections [4].GetComponent<Image> ().sprite = redEngineSectionDestroyed;

				} else {

					unitSections [4].GetComponent<Image> ().sprite = redEngineSection;

				}

				unitSections [5].GetComponent<Image> ().sprite = null;

				break;


			case Player.Color.Blue:

				if (combatUnit.GetComponent<PhasorSection> ().isDestroyed == true) {

					unitSections [0].GetComponent<Image> ().sprite = bluePhasorSectionDestroyed;

				} else {

					unitSections [0].GetComponent<Image> ().sprite = bluePhasorSection;

				}

				if (combatUnit.GetComponent<TorpedoSection> ().isDestroyed == true) {

					unitSections [1].GetComponent<Image> ().sprite = blueTorpedoSectionDestroyed;

				} else {

					unitSections [1].GetComponent<Image> ().sprite = blueTorpedoSection;

				}

				if (combatUnit.GetComponent<StorageSection> ().isDestroyed == true) {

					unitSections [2].GetComponent<Image> ().sprite = blueStorageSectionDestroyed;

				} else {

					unitSections [2].GetComponent<Image> ().sprite = blueStorageSection;

				}

				if (combatUnit.GetComponent<CrewSection> ().isDestroyed == true) {

					unitSections [3].GetComponent<Image> ().sprite = blueCrewSectionDestroyed;

				} else {

					unitSections [3].GetComponent<Image> ().sprite = blueCrewSection;

				}

				if (combatUnit.GetComponent<EngineSection> ().isDestroyed == true) {

					unitSections [4].GetComponent<Image> ().sprite = blueEngineSectionDestroyed;

				} else {

					unitSections [4].GetComponent<Image> ().sprite = blueEngineSection;

				}

				unitSections [5].GetComponent<Image> ().sprite = null;

				break;

			default:

				unitSections [0].GetComponent<Image> ().sprite = null;
				unitSections [1].GetComponent<Image> ().sprite = null;
				unitSections [2].GetComponent<Image> ().sprite = null;
				unitSections [3].GetComponent<Image> ().sprite = null;
				unitSections [4].GetComponent<Image> ().sprite = null;
				unitSections [5].GetComponent<Image> ().sprite = null;
				break;

			}

			break;

		case CombatUnit.UnitType.Destroyer:

			//set the transform positions and scales
			for (int i = 0; i < 6; i++) {

				unitSections [i].transform.localPosition = destroyerSpritePosition [i];
				unitSections [i].transform.localScale = shipSpriteScale [i];

			}

			//set the active sections
			unitSections [0].SetActive (true);
			unitSections [1].SetActive (true);
			unitSections [2].SetActive (true);
			unitSections [3].SetActive (false);
			unitSections [4].SetActive (true);
			unitSections [5].SetActive (false);

			//to set the images, we need to know the owner color as well as whether the section is destroyed
			switch (combatUnit.owner.color) {

			case Player.Color.Green:

				if (combatUnit.GetComponent<PhasorSection> ().isDestroyed == true) {

					unitSections [0].GetComponent<Image> ().sprite = greenPhasorSectionDestroyed;

				} else {

					unitSections [0].GetComponent<Image> ().sprite = greenPhasorSection;

				}

				if (combatUnit.GetComponent<TorpedoSection> ().isDestroyed == true) {

					unitSections [1].GetComponent<Image> ().sprite = greenTorpedoSectionDestroyed;

				} else {

					unitSections [1].GetComponent<Image> ().sprite = greenTorpedoSection;

				}

				if (combatUnit.GetComponent<StorageSection> ().isDestroyed == true) {

					unitSections [2].GetComponent<Image> ().sprite = greenStorageSectionDestroyed;

				} else {

					unitSections [2].GetComponent<Image> ().sprite = greenStorageSection;

				}

				unitSections [3].GetComponent<Image> ().sprite = null;

				if (combatUnit.GetComponent<EngineSection> ().isDestroyed == true) {

					unitSections [4].GetComponent<Image> ().sprite = greenEngineSectionDestroyed;

				} else {

					unitSections [4].GetComponent<Image> ().sprite = greenEngineSection;

				}

				unitSections [5].GetComponent<Image> ().sprite = null;

				break;

			case Player.Color.Purple:

				if (combatUnit.GetComponent<PhasorSection> ().isDestroyed == true) {

					unitSections [0].GetComponent<Image> ().sprite = purplePhasorSectionDestroyed;

				} else {

					unitSections [0].GetComponent<Image> ().sprite = purplePhasorSection;

				}

				if (combatUnit.GetComponent<TorpedoSection> ().isDestroyed == true) {

					unitSections [1].GetComponent<Image> ().sprite = purpleTorpedoSectionDestroyed;

				} else {

					unitSections [1].GetComponent<Image> ().sprite = purpleTorpedoSection;

				}

				if (combatUnit.GetComponent<StorageSection> ().isDestroyed == true) {

					unitSections [2].GetComponent<Image> ().sprite = purpleStorageSectionDestroyed;

				} else {

					unitSections [2].GetComponent<Image> ().sprite = purpleStorageSection;

				}

				unitSections [3].GetComponent<Image> ().sprite = null;

				if (combatUnit.GetComponent<EngineSection> ().isDestroyed == true) {

					unitSections [4].GetComponent<Image> ().sprite = purpleEngineSectionDestroyed;

				} else {

					unitSections [4].GetComponent<Image> ().sprite = purpleEngineSection;

				}

				unitSections [5].GetComponent<Image> ().sprite = null;

				break;

			case Player.Color.Red:

				if (combatUnit.GetComponent<PhasorSection> ().isDestroyed == true) {

					unitSections [0].GetComponent<Image> ().sprite = redPhasorSectionDestroyed;

				} else {

					unitSections [0].GetComponent<Image> ().sprite = redPhasorSection;

				}

				if (combatUnit.GetComponent<TorpedoSection> ().isDestroyed == true) {

					unitSections [1].GetComponent<Image> ().sprite = redTorpedoSectionDestroyed;

				} else {

					unitSections [1].GetComponent<Image> ().sprite = redTorpedoSection;

				}

				if (combatUnit.GetComponent<StorageSection> ().isDestroyed == true) {

					unitSections [2].GetComponent<Image> ().sprite = redStorageSectionDestroyed;

				} else {

					unitSections [2].GetComponent<Image> ().sprite = redStorageSection;

				}

				unitSections [3].GetComponent<Image> ().sprite = null;

				if (combatUnit.GetComponent<EngineSection> ().isDestroyed == true) {

					unitSections [4].GetComponent<Image> ().sprite = redEngineSectionDestroyed;

				} else {

					unitSections [4].GetComponent<Image> ().sprite = redEngineSection;

				}

				unitSections [5].GetComponent<Image> ().sprite = null;

				break;


			case Player.Color.Blue:

				if (combatUnit.GetComponent<PhasorSection> ().isDestroyed == true) {

					unitSections [0].GetComponent<Image> ().sprite = bluePhasorSectionDestroyed;

				} else {

					unitSections [0].GetComponent<Image> ().sprite = bluePhasorSection;

				}

				if (combatUnit.GetComponent<TorpedoSection> ().isDestroyed == true) {

					unitSections [1].GetComponent<Image> ().sprite = blueTorpedoSectionDestroyed;

				} else {

					unitSections [1].GetComponent<Image> ().sprite = blueTorpedoSection;

				}

				if (combatUnit.GetComponent<StorageSection> ().isDestroyed == true) {

					unitSections [2].GetComponent<Image> ().sprite = blueStorageSectionDestroyed;

				} else {

					unitSections [2].GetComponent<Image> ().sprite = blueStorageSection;

				}

				unitSections [3].GetComponent<Image> ().sprite = null;

				if (combatUnit.GetComponent<EngineSection> ().isDestroyed == true) {

					unitSections [4].GetComponent<Image> ().sprite = blueEngineSectionDestroyed;

				} else {

					unitSections [4].GetComponent<Image> ().sprite = blueEngineSection;

				}

				unitSections [5].GetComponent<Image> ().sprite = null;

				break;

			default:

				unitSections [0].GetComponent<Image> ().sprite = null;
				unitSections [1].GetComponent<Image> ().sprite = null;
				unitSections [2].GetComponent<Image> ().sprite = null;
				unitSections [3].GetComponent<Image> ().sprite = null;
				unitSections [4].GetComponent<Image> ().sprite = null;
				unitSections [5].GetComponent<Image> ().sprite = null;
				break;

			}

			break;

		case CombatUnit.UnitType.BirdOfPrey:

			//set the transform positions and scales
			for (int i = 0; i < 6; i++) {

				unitSections [i].transform.localPosition = birdOfPreySpritePosition [i];
				unitSections [i].transform.localScale = shipSpriteScale [i];

			}

			//set the active sections
			unitSections [0].SetActive (true);
			unitSections [1].SetActive (true);
			unitSections [2].SetActive (false);
			unitSections [3].SetActive (false);
			unitSections [4].SetActive (true);
			unitSections [5].SetActive (false);

			//to set the images, we need to know the owner color as well as whether the section is destroyed
			switch (combatUnit.owner.color) {

			case Player.Color.Green:

				if (combatUnit.GetComponent<PhasorSection> ().isDestroyed == true) {

					unitSections [0].GetComponent<Image> ().sprite = greenPhasorSectionDestroyed;

				} else {

					unitSections [0].GetComponent<Image> ().sprite = greenPhasorSection;

				}

				if (combatUnit.GetComponent<TorpedoSection> ().isDestroyed == true) {

					unitSections [1].GetComponent<Image> ().sprite = greenTorpedoSectionDestroyed;

				} else {

					unitSections [1].GetComponent<Image> ().sprite = greenTorpedoSection;

				}

				unitSections [2].GetComponent<Image> ().sprite = null;

				unitSections [3].GetComponent<Image> ().sprite = null;

				if (combatUnit.GetComponent<EngineSection> ().isDestroyed == true) {

					unitSections [4].GetComponent<Image> ().sprite = greenEngineSectionDestroyed;

				} else {

					unitSections [4].GetComponent<Image> ().sprite = greenEngineSection;

				}

				unitSections [5].GetComponent<Image> ().sprite = null;

				break;

			case Player.Color.Purple:

				if (combatUnit.GetComponent<PhasorSection> ().isDestroyed == true) {

					unitSections [0].GetComponent<Image> ().sprite = purplePhasorSectionDestroyed;

				} else {

					unitSections [0].GetComponent<Image> ().sprite = purplePhasorSection;

				}

				if (combatUnit.GetComponent<TorpedoSection> ().isDestroyed == true) {

					unitSections [1].GetComponent<Image> ().sprite = purpleTorpedoSectionDestroyed;

				} else {

					unitSections [1].GetComponent<Image> ().sprite = purpleTorpedoSection;

				}

				unitSections [2].GetComponent<Image> ().sprite = null;

				unitSections [3].GetComponent<Image> ().sprite = null;

				if (combatUnit.GetComponent<EngineSection> ().isDestroyed == true) {

					unitSections [4].GetComponent<Image> ().sprite = purpleEngineSectionDestroyed;

				} else {

					unitSections [4].GetComponent<Image> ().sprite = purpleEngineSection;

				}

				unitSections [5].GetComponent<Image> ().sprite = null;

				break;

			case Player.Color.Red:

				if (combatUnit.GetComponent<PhasorSection> ().isDestroyed == true) {

					unitSections [0].GetComponent<Image> ().sprite = redPhasorSectionDestroyed;

				} else {

					unitSections [0].GetComponent<Image> ().sprite = redPhasorSection;

				}

				if (combatUnit.GetComponent<TorpedoSection> ().isDestroyed == true) {

					unitSections [1].GetComponent<Image> ().sprite = redTorpedoSectionDestroyed;

				} else {

					unitSections [1].GetComponent<Image> ().sprite = redTorpedoSection;

				}

				unitSections [2].GetComponent<Image> ().sprite = null;

				unitSections [3].GetComponent<Image> ().sprite = null;

				if (combatUnit.GetComponent<EngineSection> ().isDestroyed == true) {

					unitSections [4].GetComponent<Image> ().sprite = redEngineSectionDestroyed;

				} else {

					unitSections [4].GetComponent<Image> ().sprite = redEngineSection;

				}

				unitSections [5].GetComponent<Image> ().sprite = null;

				break;


			case Player.Color.Blue:

				if (combatUnit.GetComponent<PhasorSection> ().isDestroyed == true) {

					unitSections [0].GetComponent<Image> ().sprite = bluePhasorSectionDestroyed;

				} else {

					unitSections [0].GetComponent<Image> ().sprite = bluePhasorSection;

				}

				if (combatUnit.GetComponent<TorpedoSection> ().isDestroyed == true) {

					unitSections [1].GetComponent<Image> ().sprite = blueTorpedoSectionDestroyed;

				} else {

					unitSections [1].GetComponent<Image> ().sprite = blueTorpedoSection;

				}

				unitSections [2].GetComponent<Image> ().sprite = null;

				unitSections [3].GetComponent<Image> ().sprite = null;

				if (combatUnit.GetComponent<EngineSection> ().isDestroyed == true) {

					unitSections [4].GetComponent<Image> ().sprite = blueEngineSectionDestroyed;

				} else {

					unitSections [4].GetComponent<Image> ().sprite = blueEngineSection;

				}

				unitSections [5].GetComponent<Image> ().sprite = null;

				break;

			default:

				unitSections [0].GetComponent<Image> ().sprite = null;
				unitSections [1].GetComponent<Image> ().sprite = null;
				unitSections [2].GetComponent<Image> ().sprite = null;
				unitSections [3].GetComponent<Image> ().sprite = null;
				unitSections [4].GetComponent<Image> ().sprite = null;
				unitSections [5].GetComponent<Image> ().sprite = null;
				break;

			}

			break;

		case CombatUnit.UnitType.Scout:

			//set the transform positions and scales
			for (int i = 0; i < 6; i++) {

				unitSections [i].transform.localPosition = scoutSpritePosition [i];
				unitSections [i].transform.localScale = shipSpriteScale [i];

			}

			//set the active sections
			unitSections [0].SetActive (true);
			unitSections [1].SetActive (false);
			unitSections [2].SetActive (true);
			unitSections [3].SetActive (false);
			unitSections [4].SetActive (true);
			unitSections [5].SetActive (false);

			//to set the images, we need to know the owner color as well as whether the section is destroyed
			switch (combatUnit.owner.color) {

			case Player.Color.Green:

				if (combatUnit.GetComponent<PhasorSection> ().isDestroyed == true) {

					unitSections [0].GetComponent<Image> ().sprite = greenPhasorSectionDestroyed;

				} else {

					unitSections [0].GetComponent<Image> ().sprite = greenPhasorSection;

				}

				unitSections [1].GetComponent<Image> ().sprite = null;

				if (combatUnit.GetComponent<StorageSection> ().isDestroyed == true) {

					unitSections [2].GetComponent<Image> ().sprite = greenStorageSectionDestroyed;

				} else {

					unitSections [2].GetComponent<Image> ().sprite = greenStorageSection;

				}

				unitSections [3].GetComponent<Image> ().sprite = null;

				if (combatUnit.GetComponent<EngineSection> ().isDestroyed == true) {

					unitSections [4].GetComponent<Image> ().sprite = greenEngineSectionDestroyed;

				} else {

					unitSections [4].GetComponent<Image> ().sprite = greenEngineSection;

				}

				unitSections [5].GetComponent<Image> ().sprite = null;

				break;

			case Player.Color.Purple:

				if (combatUnit.GetComponent<PhasorSection> ().isDestroyed == true) {

					unitSections [0].GetComponent<Image> ().sprite = purplePhasorSectionDestroyed;

				} else {

					unitSections [0].GetComponent<Image> ().sprite = purplePhasorSection;

				}

				unitSections [1].GetComponent<Image> ().sprite = null;

				if (combatUnit.GetComponent<StorageSection> ().isDestroyed == true) {

					unitSections [2].GetComponent<Image> ().sprite = purpleStorageSectionDestroyed;

				} else {

					unitSections [2].GetComponent<Image> ().sprite = purpleStorageSection;

				}

				unitSections [3].GetComponent<Image> ().sprite = null;

				if (combatUnit.GetComponent<EngineSection> ().isDestroyed == true) {

					unitSections [4].GetComponent<Image> ().sprite = purpleEngineSectionDestroyed;

				} else {

					unitSections [4].GetComponent<Image> ().sprite = purpleEngineSection;

				}

				unitSections [5].GetComponent<Image> ().sprite = null;

				break;

			case Player.Color.Red:

				if (combatUnit.GetComponent<PhasorSection> ().isDestroyed == true) {

					unitSections [0].GetComponent<Image> ().sprite = redPhasorSectionDestroyed;

				} else {

					unitSections [0].GetComponent<Image> ().sprite = redPhasorSection;

				}

				unitSections [1].GetComponent<Image> ().sprite = null;

				if (combatUnit.GetComponent<StorageSection> ().isDestroyed == true) {

					unitSections [2].GetComponent<Image> ().sprite = redStorageSectionDestroyed;

				} else {

					unitSections [2].GetComponent<Image> ().sprite = redStorageSection;

				}

				unitSections [3].GetComponent<Image> ().sprite = null;

				if (combatUnit.GetComponent<EngineSection> ().isDestroyed == true) {

					unitSections [4].GetComponent<Image> ().sprite = redEngineSectionDestroyed;

				} else {

					unitSections [4].GetComponent<Image> ().sprite = redEngineSection;

				}

				unitSections [5].GetComponent<Image> ().sprite = null;

				break;


			case Player.Color.Blue:

				if (combatUnit.GetComponent<PhasorSection> ().isDestroyed == true) {

					unitSections [0].GetComponent<Image> ().sprite = bluePhasorSectionDestroyed;

				} else {

					unitSections [0].GetComponent<Image> ().sprite = bluePhasorSection;

				}

				unitSections [1].GetComponent<Image> ().sprite = null;

				if (combatUnit.GetComponent<StorageSection> ().isDestroyed == true) {

					unitSections [2].GetComponent<Image> ().sprite = blueStorageSectionDestroyed;

				} else {

					unitSections [2].GetComponent<Image> ().sprite = blueStorageSection;

				}

				unitSections [3].GetComponent<Image> ().sprite = null;

				if (combatUnit.GetComponent<EngineSection> ().isDestroyed == true) {

					unitSections [4].GetComponent<Image> ().sprite = blueEngineSectionDestroyed;

				} else {

					unitSections [4].GetComponent<Image> ().sprite = blueEngineSection;

				}

				unitSections [5].GetComponent<Image> ().sprite = null;

				break;

			default:

				unitSections [0].GetComponent<Image> ().sprite = null;
				unitSections [1].GetComponent<Image> ().sprite = null;
				unitSections [2].GetComponent<Image> ().sprite = null;
				unitSections [3].GetComponent<Image> ().sprite = null;
				unitSections [4].GetComponent<Image> ().sprite = null;
				unitSections [5].GetComponent<Image> ().sprite = null;
				break;

			}

			break;

		default:

			unitSections [0].GetComponent<Image> ().sprite = null;
			unitSections [1].GetComponent<Image> ().sprite = null;
			unitSections [2].GetComponent<Image> ().sprite = null;
			unitSections [3].GetComponent<Image> ().sprite = null;
			unitSections [4].GetComponent<Image> ().sprite = null;
			unitSections [5].GetComponent<Image> ().sprite = null;
			break;

		}

	}

	//this function will check if the targeted ship section to be hit will be destroyed
	private bool WillDestroyShipSection(CombatUnit targetedUnit, CombatManager.ShipSectionTargeted shipSectionTargeted, int damage){

		//this is the return value
		bool willBeDestroyed = false;

		//run a switch case on the section targeted
		switch (shipSectionTargeted) {

		case CombatManager.ShipSectionTargeted.PhasorSection:

			//check if the section targeted is null - this represents a logic error somewhere upstream
			if (targetedUnit.GetComponent<PhasorSection> () == null) {

				Debug.LogError ("Function passed an attack to a phasor section for a combat unit without a phasor section");

			}

			//check if the damage is greater than or equal to the remaining shields
			else if (damage >= targetedUnit.GetComponent<PhasorSection> ().shieldsCurrent) {

				willBeDestroyed = true;

			}

			break;

		case CombatManager.ShipSectionTargeted.TorpedoSection:

			//check if the section targeted is null - this represents a logic error somewhere upstream
			if (targetedUnit.GetComponent<TorpedoSection> () == null) {

				Debug.LogError ("Function passed an attack to a torpedo section for a combat unit without a torpedo section");

			}

			//check if the damage is greater than or equal to the remaining shields
			else if (damage >= targetedUnit.GetComponent<TorpedoSection> ().shieldsCurrent) {

				willBeDestroyed = true;

			}

			break;

		case CombatManager.ShipSectionTargeted.StorageSection:

			//check if the section targeted is null - this represents a logic error somewhere upstream
			if (targetedUnit.GetComponent<StorageSection> () == null) {

				Debug.LogError ("Function passed an attack to a storage section for a combat unit without a storage section");

			}

			//check if the damage is greater than or equal to the remaining shields
			else if (damage >= targetedUnit.GetComponent<StorageSection> ().shieldsCurrent) {

				willBeDestroyed = true;

			}

			break;

		case CombatManager.ShipSectionTargeted.CrewSection:

			//check if the section targeted is null - this represents a logic error somewhere upstream
			if (targetedUnit.GetComponent<CrewSection> () == null) {

				Debug.LogError ("Function passed an attack to a crew section for a combat unit without a crew section");

			}

			//check if the damage is greater than or equal to the remaining shields
			else if (damage >= targetedUnit.GetComponent<CrewSection> ().shieldsCurrent) {

				willBeDestroyed = true;

			}

			break;

		case CombatManager.ShipSectionTargeted.EngineSection:

			//check if the section targeted is null - this represents a logic error somewhere upstream
			if (targetedUnit.GetComponent<EngineSection> () == null) {

				Debug.LogError ("Function passed an attack to an engine section for a combat unit without an engine section");

			}

			//check if the damage is greater than or equal to the remaining shields
			else if (damage >= targetedUnit.GetComponent<EngineSection> ().shieldsCurrent) {

				willBeDestroyed = true;

			}
			break;

		default:

			Debug.LogError("could not match ship section targeted");
			break;

		}

		//return the destroy status
		return willBeDestroyed;

	}

	//this function will check if the targeted base section to be hit will be destroyed
	private bool WillDestroyBaseSection(CombatUnit targetedUnit, CombatManager.BaseSectionTargeted baseSectionTargeted, int damage){

		//this is the return value
		bool willBeDestroyed = false;

		//run a switch case on the section targeted
		switch (baseSectionTargeted) {

		case CombatManager.BaseSectionTargeted.PhasorSection1:

			//check if the section targeted is null - this represents a logic error somewhere upstream
			if (targetedUnit.GetComponent<StarbasePhasorSection1> () == null) {

				Debug.LogError ("Function passed an attack to a base phasor section 1 for a combat unit without a base phasor section 1");

			}

			//check if the damage is greater than or equal to the remaining shields
			else if (damage >= targetedUnit.GetComponent<StarbasePhasorSection1> ().shieldsCurrent) {

				willBeDestroyed = true;

			}

			break;

		case CombatManager.BaseSectionTargeted.PhasorSection2:

			//check if the section targeted is null - this represents a logic error somewhere upstream
			if (targetedUnit.GetComponent<StarbasePhasorSection2> () == null) {

				Debug.LogError ("Function passed an attack to a base phasor section 2 for a combat unit without a base phasor section 2");

			}

			//check if the damage is greater than or equal to the remaining shields
			else if (damage >= targetedUnit.GetComponent<StarbasePhasorSection2> ().shieldsCurrent) {

				willBeDestroyed = true;

			}

			break;

		case CombatManager.BaseSectionTargeted.TorpedoSection:

			//check if the section targeted is null - this represents a logic error somewhere upstream
			if (targetedUnit.GetComponent<StarbaseTorpedoSection> () == null) {

				Debug.LogError ("Function passed an attack to a starbase torpedo section for a combat unit without a starbase torpedo section");

			}

			//check if the damage is greater than or equal to the remaining shields
			else if (damage >= targetedUnit.GetComponent<StarbaseTorpedoSection> ().shieldsCurrent) {

				willBeDestroyed = true;

			}

			break;

		case CombatManager.BaseSectionTargeted.CrewSection:

			//check if the section targeted is null - this represents a logic error somewhere upstream
			if (targetedUnit.GetComponent<StarbaseCrewSection> () == null) {

				Debug.LogError ("Function passed an attack to a starbase crew section for a combat unit without a starbase crew section");

			}

			//check if the damage is greater than or equal to the remaining shields
			else if (damage >= targetedUnit.GetComponent<StarbaseCrewSection> ().shieldsCurrent) {

				willBeDestroyed = true;

			}

			break;

		case CombatManager.BaseSectionTargeted.StorageSection1:

			//check if the section targeted is null - this represents a logic error somewhere upstream
			if (targetedUnit.GetComponent<StarbaseStorageSection1> () == null) {

				Debug.LogError ("Function passed an attack to a starbase storage section 1 for a combat unit without a starbase storage section 1");

			}

			//check if the damage is greater than or equal to the remaining shields
			else if (damage >= targetedUnit.GetComponent<StarbaseStorageSection1> ().shieldsCurrent) {

				willBeDestroyed = true;

			}

			break;
				
		case CombatManager.BaseSectionTargeted.StorageSection2:

			//check if the section targeted is null - this represents a logic error somewhere upstream
			if (targetedUnit.GetComponent<StarbaseStorageSection2> () == null) {

				Debug.LogError ("Function passed an attack to a starbase storage section 2 for a combat unit without a starbase storage section 2");

			}

			//check if the damage is greater than or equal to the remaining shields
			else if (damage >= targetedUnit.GetComponent<StarbaseStorageSection2> ().shieldsCurrent) {

				willBeDestroyed = true;

			}

			break;

		default:

			Debug.LogError("could not match base section targeted");
			break;

		}

		//return the destroy status
		return willBeDestroyed;

	}
		
	//this function will check whether upon destroying a ship section the entire ship will be destroyed (i.e. all other sections
	//are already destroyed)
	private bool WillDestroyEntireShip(CombatUnit targetedUnit, CombatManager.ShipSectionTargeted shipSectionToBeDestroyed){

		//variable to return
		bool willBeCompletelyDestroyed = false;

		//first, check that we have a ship - this function should only be called on ships
		if (targetedUnit.GetComponent<Ship> () == null) {

			Debug.LogError ("Called WillDestroyEntireShip() on a non-Ship unit");
			return willBeCompletelyDestroyed;

		}

		//do a switch case on the unit type
		switch (targetedUnit.unitType) {

		case CombatUnit.UnitType.Starship:

			//do a switch case on the section to be destroyed
			switch (shipSectionToBeDestroyed) {

			case CombatManager.ShipSectionTargeted.PhasorSection:

				//check if any other sections are alive
				if (targetedUnit.GetComponent<TorpedoSection> ().isDestroyed == false ||
				    targetedUnit.GetComponent<StorageSection> ().isDestroyed == false ||
				    targetedUnit.GetComponent<CrewSection> ().isDestroyed == false ||
				    targetedUnit.GetComponent<EngineSection> ().isDestroyed == false) {

					//if at least one of those sections is alive, the unit will not be completely destroyed
					willBeCompletelyDestroyed = false;

				} else {

					//the else condition is that all other sections are destroyed, so this will completely destroy the unit
					willBeCompletelyDestroyed = true;

				}

				break;

			case CombatManager.ShipSectionTargeted.TorpedoSection:

				//check if any other sections are alive
				if (targetedUnit.GetComponent<PhasorSection> ().isDestroyed == false ||
				    targetedUnit.GetComponent<StorageSection> ().isDestroyed == false ||
				    targetedUnit.GetComponent<CrewSection> ().isDestroyed == false ||
				    targetedUnit.GetComponent<EngineSection> ().isDestroyed == false) {

					//if at least one of those sections is alive, the unit will not be completely destroyed
					willBeCompletelyDestroyed = false;

				} else {

					//the else condition is that all other sections are destroyed, so this will completely destroy the unit
					willBeCompletelyDestroyed = true;

				}

				break;

			case CombatManager.ShipSectionTargeted.StorageSection:

				//check if any other sections are alive
				if (targetedUnit.GetComponent<PhasorSection> ().isDestroyed == false ||
				    targetedUnit.GetComponent<TorpedoSection> ().isDestroyed == false ||
				    targetedUnit.GetComponent<CrewSection> ().isDestroyed == false ||
				    targetedUnit.GetComponent<EngineSection> ().isDestroyed == false) {

					//if at least one of those sections is alive, the unit will not be completely destroyed
					willBeCompletelyDestroyed = false;

				} else {

					//the else condition is that all other sections are destroyed, so this will completely destroy the unit
					willBeCompletelyDestroyed = true;

				}

				break;

			case CombatManager.ShipSectionTargeted.CrewSection:

				//check if any other sections are alive
				if (targetedUnit.GetComponent<PhasorSection> ().isDestroyed == false ||
				    targetedUnit.GetComponent<TorpedoSection> ().isDestroyed == false ||
				    targetedUnit.GetComponent<StorageSection> ().isDestroyed == false ||
				    targetedUnit.GetComponent<EngineSection> ().isDestroyed == false) {

					//if at least one of those sections is alive, the unit will not be completely destroyed
					willBeCompletelyDestroyed = false;

				} else {

					//the else condition is that all other sections are destroyed, so this will completely destroy the unit
					willBeCompletelyDestroyed = true;

				}

				break;

			case CombatManager.ShipSectionTargeted.EngineSection:

				//check if any other sections are alive
				if (targetedUnit.GetComponent<PhasorSection> ().isDestroyed == false ||
				    targetedUnit.GetComponent<TorpedoSection> ().isDestroyed == false ||
				    targetedUnit.GetComponent<StorageSection> ().isDestroyed == false ||
				    targetedUnit.GetComponent<CrewSection> ().isDestroyed == false) {

					//if at least one of those sections is alive, the unit will not be completely destroyed
					willBeCompletelyDestroyed = false;

				} else {

					//the else condition is that all other sections are destroyed, so this will completely destroy the unit
					willBeCompletelyDestroyed = true;

				}

				break;

			default:

				Debug.LogError ("Could not match section targeted");
				willBeCompletelyDestroyed = false;
				break;

			}

			break;

		case CombatUnit.UnitType.Destroyer:

			//do a switch case on the section to be destroyed
			switch (shipSectionToBeDestroyed) {

			case CombatManager.ShipSectionTargeted.PhasorSection:

				//check if any other sections are alive
				if (targetedUnit.GetComponent<TorpedoSection> ().isDestroyed == false ||
				    targetedUnit.GetComponent<StorageSection> ().isDestroyed == false ||
				    targetedUnit.GetComponent<EngineSection> ().isDestroyed == false) {

					//if at least one of those sections is alive, the unit will not be completely destroyed
					willBeCompletelyDestroyed = false;

				} else {

					//the else condition is that all other sections are destroyed, so this will completely destroy the unit
					willBeCompletelyDestroyed = true;

				}

				break;

			case CombatManager.ShipSectionTargeted.TorpedoSection:

				//check if any other sections are alive
				if (targetedUnit.GetComponent<PhasorSection> ().isDestroyed == false ||
				    targetedUnit.GetComponent<StorageSection> ().isDestroyed == false ||
				    targetedUnit.GetComponent<EngineSection> ().isDestroyed == false) {

					//if at least one of those sections is alive, the unit will not be completely destroyed
					willBeCompletelyDestroyed = false;

				} else {

					//the else condition is that all other sections are destroyed, so this will completely destroy the unit
					willBeCompletelyDestroyed = true;

				}

				break;

			case CombatManager.ShipSectionTargeted.StorageSection:

				//check if any other sections are alive
				if (targetedUnit.GetComponent<PhasorSection> ().isDestroyed == false ||
				    targetedUnit.GetComponent<TorpedoSection> ().isDestroyed == false ||
				    targetedUnit.GetComponent<EngineSection> ().isDestroyed == false) {

					//if at least one of those sections is alive, the unit will not be completely destroyed
					willBeCompletelyDestroyed = false;

				} else {

					//the else condition is that all other sections are destroyed, so this will completely destroy the unit
					willBeCompletelyDestroyed = true;

				}

				break;

			case CombatManager.ShipSectionTargeted.EngineSection:

				//check if any other sections are alive
				if (targetedUnit.GetComponent<PhasorSection> ().isDestroyed == false ||
				    targetedUnit.GetComponent<TorpedoSection> ().isDestroyed == false ||
				    targetedUnit.GetComponent<StorageSection> ().isDestroyed == false) {

					//if at least one of those sections is alive, the unit will not be completely destroyed
					willBeCompletelyDestroyed = false;

				} else {

					//the else condition is that all other sections are destroyed, so this will completely destroy the unit
					willBeCompletelyDestroyed = true;

				}

				break;

			default:

				Debug.LogError ("Could not match section targeted");
				willBeCompletelyDestroyed = false;
				break;

			}

			break;

		case CombatUnit.UnitType.BirdOfPrey:

			//do a switch case on the section to be destroyed
			switch (shipSectionToBeDestroyed) {

			case CombatManager.ShipSectionTargeted.PhasorSection:

				//check if any other sections are alive
				if (targetedUnit.GetComponent<TorpedoSection> ().isDestroyed == false ||
				    targetedUnit.GetComponent<EngineSection> ().isDestroyed == false) {

					//if at least one of those sections is alive, the unit will not be completely destroyed
					willBeCompletelyDestroyed = false;

				} else {

					//the else condition is that all other sections are destroyed, so this will completely destroy the unit
					willBeCompletelyDestroyed = true;

				}

				break;

			case CombatManager.ShipSectionTargeted.TorpedoSection:

				//check if any other sections are alive
				if (targetedUnit.GetComponent<PhasorSection> ().isDestroyed == false ||
				    targetedUnit.GetComponent<EngineSection> ().isDestroyed == false) {

					//if at least one of those sections is alive, the unit will not be completely destroyed
					willBeCompletelyDestroyed = false;

				} else {

					//the else condition is that all other sections are destroyed, so this will completely destroy the unit
					willBeCompletelyDestroyed = true;

				}

				break;

			case CombatManager.ShipSectionTargeted.EngineSection:

				//check if any other sections are alive
				if (targetedUnit.GetComponent<PhasorSection> ().isDestroyed == false ||
				    targetedUnit.GetComponent<TorpedoSection> ().isDestroyed == false) {

					//if at least one of those sections is alive, the unit will not be completely destroyed
					willBeCompletelyDestroyed = false;

				} else {

					//the else condition is that all other sections are destroyed, so this will completely destroy the unit
					willBeCompletelyDestroyed = true;

				}

				break;

			default:

				Debug.LogError ("Could not match section targeted");
				willBeCompletelyDestroyed = false;
				break;

			}

			break;

		case CombatUnit.UnitType.Scout:

			//do a switch case on the section to be destroyed
			switch (shipSectionToBeDestroyed) {

			case CombatManager.ShipSectionTargeted.PhasorSection:

				//check if any other sections are alive
				if (targetedUnit.GetComponent<StorageSection> ().isDestroyed == false ||
				    targetedUnit.GetComponent<EngineSection> ().isDestroyed == false) {

					//if at least one of those sections is alive, the unit will not be completely destroyed
					willBeCompletelyDestroyed = false;

				} else {

					//the else condition is that all other sections are destroyed, so this will completely destroy the unit
					willBeCompletelyDestroyed = true;

				}

				break;

			case CombatManager.ShipSectionTargeted.StorageSection:

				//check if any other sections are alive
				if (targetedUnit.GetComponent<PhasorSection> ().isDestroyed == false ||
				    targetedUnit.GetComponent<EngineSection> ().isDestroyed == false) {

					//if at least one of those sections is alive, the unit will not be completely destroyed
					willBeCompletelyDestroyed = false;

				} else {

					//the else condition is that all other sections are destroyed, so this will completely destroy the unit
					willBeCompletelyDestroyed = true;

				}

				break;

			case CombatManager.ShipSectionTargeted.EngineSection:

				//check if any other sections are alive
				if (targetedUnit.GetComponent<PhasorSection> ().isDestroyed == false ||
				    targetedUnit.GetComponent<StorageSection> ().isDestroyed == false) {

					//if at least one of those sections is alive, the unit will not be completely destroyed
					willBeCompletelyDestroyed = false;

				} else {

					//the else condition is that all other sections are destroyed, so this will completely destroy the unit
					willBeCompletelyDestroyed = true;

				}

				break;

			default:

				Debug.LogError ("Could not match section targeted");
				willBeCompletelyDestroyed = false;
				break;

			}

			break;

		default:

			Debug.LogError ("Could not match unit type");
			willBeCompletelyDestroyed = false;
			break;

		}

		return willBeCompletelyDestroyed;

	}

	//this function will check whether upon destroying a base section the entire base will be destroyed (i.e. all other sections
	//are already destroyed)
	private bool WillDestroyEntireBase(CombatUnit targetedUnit, CombatManager.BaseSectionTargeted baseSectionToBeDestroyed){

		//variable to return
		bool willBeCompletelyDestroyed = false;

		//first, check that we have a starbase - this function should only be called on starbases
		if (targetedUnit.GetComponent<Starbase> () == null) {

			Debug.LogError ("Called WillDestroyEntireBase() on a non-Starbase unit");
			return willBeCompletelyDestroyed;

		}

		//do a switch case on the section to be destroyed
		switch (baseSectionToBeDestroyed) {

		case CombatManager.BaseSectionTargeted.PhasorSection1:

			//check if any other sections are alive
			if (targetedUnit.GetComponent<StarbasePhasorSection2> ().isDestroyed == false ||
				targetedUnit.GetComponent<StarbaseTorpedoSection> ().isDestroyed == false ||
				targetedUnit.GetComponent<StarbaseCrewSection> ().isDestroyed == false ||
				targetedUnit.GetComponent<StarbaseStorageSection1> ().isDestroyed == false ||
				targetedUnit.GetComponent<StarbaseStorageSection2> ().isDestroyed == false) {

				//if at least one of those sections is alive, the unit will not be completely destroyed
				willBeCompletelyDestroyed = false;

			} else {

				//the else condition is that all other sections are destroyed, so this will completely destroy the unit
				willBeCompletelyDestroyed = true;

			}

			break;

		case CombatManager.BaseSectionTargeted.PhasorSection2:

			//check if any other sections are alive
			if (targetedUnit.GetComponent<StarbasePhasorSection1> ().isDestroyed == false ||
				targetedUnit.GetComponent<StarbaseTorpedoSection> ().isDestroyed == false ||
				targetedUnit.GetComponent<StarbaseCrewSection> ().isDestroyed == false ||
				targetedUnit.GetComponent<StarbaseStorageSection1> ().isDestroyed == false ||
				targetedUnit.GetComponent<StarbaseStorageSection2> ().isDestroyed == false) {

				//if at least one of those sections is alive, the unit will not be completely destroyed
				willBeCompletelyDestroyed = false;

			} else {

				//the else condition is that all other sections are destroyed, so this will completely destroy the unit
				willBeCompletelyDestroyed = true;

			}

			break;

		case CombatManager.BaseSectionTargeted.TorpedoSection:

			//check if any other sections are alive
			if (targetedUnit.GetComponent<StarbasePhasorSection1> ().isDestroyed == false ||
				targetedUnit.GetComponent<StarbasePhasorSection2> ().isDestroyed == false ||
				targetedUnit.GetComponent<StarbaseCrewSection> ().isDestroyed == false ||
				targetedUnit.GetComponent<StarbaseStorageSection1> ().isDestroyed == false ||
				targetedUnit.GetComponent<StarbaseStorageSection2> ().isDestroyed == false) {

				//if at least one of those sections is alive, the unit will not be completely destroyed
				willBeCompletelyDestroyed = false;

			} else {

				//the else condition is that all other sections are destroyed, so this will completely destroy the unit
				willBeCompletelyDestroyed = true;

			}

			break;

		case CombatManager.BaseSectionTargeted.CrewSection:

			//check if any other sections are alive
			if (targetedUnit.GetComponent<StarbasePhasorSection1> ().isDestroyed == false ||
				targetedUnit.GetComponent<StarbasePhasorSection2> ().isDestroyed == false ||
				targetedUnit.GetComponent<StarbaseTorpedoSection> ().isDestroyed == false ||
				targetedUnit.GetComponent<StarbaseStorageSection1> ().isDestroyed == false ||
				targetedUnit.GetComponent<StarbaseStorageSection2> ().isDestroyed == false) {

				//if at least one of those sections is alive, the unit will not be completely destroyed
				willBeCompletelyDestroyed = false;

			} else {

				//the else condition is that all other sections are destroyed, so this will completely destroy the unit
				willBeCompletelyDestroyed = true;

			}

			break;

		case CombatManager.BaseSectionTargeted.StorageSection1:

			//check if any other sections are alive
			if (targetedUnit.GetComponent<StarbasePhasorSection1> ().isDestroyed == false ||
				targetedUnit.GetComponent<StarbasePhasorSection2> ().isDestroyed == false ||
				targetedUnit.GetComponent<StarbaseTorpedoSection> ().isDestroyed == false ||
				targetedUnit.GetComponent<StarbaseCrewSection> ().isDestroyed == false ||
				targetedUnit.GetComponent<StarbaseStorageSection2> ().isDestroyed == false) {

				//if at least one of those sections is alive, the unit will not be completely destroyed
				willBeCompletelyDestroyed = false;

			} else {

				//the else condition is that all other sections are destroyed, so this will completely destroy the unit
				willBeCompletelyDestroyed = true;

			}

			break;

		case CombatManager.BaseSectionTargeted.StorageSection2:

			//check if any other sections are alive
			if (targetedUnit.GetComponent<StarbasePhasorSection1> ().isDestroyed == false ||
				targetedUnit.GetComponent<StarbasePhasorSection2> ().isDestroyed == false ||
				targetedUnit.GetComponent<StarbaseTorpedoSection> ().isDestroyed == false ||
				targetedUnit.GetComponent<StarbaseCrewSection> ().isDestroyed == false ||
				targetedUnit.GetComponent<StarbaseStorageSection1> ().isDestroyed == false) {

				//if at least one of those sections is alive, the unit will not be completely destroyed
				willBeCompletelyDestroyed = false;

			} else {

				//the else condition is that all other sections are destroyed, so this will completely destroy the unit
				willBeCompletelyDestroyed = true;

			}

			break;

		default:

			Debug.LogError ("Could not match section targeted");
			willBeCompletelyDestroyed = false;
			break;

		}

		return willBeCompletelyDestroyed;

	}

	//this function populates the phasor origin points for an attack, based on the attacking unit
	private void SetPhasorLineStartingPoints(CombatUnit attackingUnit){

		//where the phasor shot originates from depends on the kind of unit
		switch (attackingUnit.unitType) {

		case CombatUnit.UnitType.Starbase:

			//for a starbase, we have 2 phasor sections, so we need to determine which one will fire.  We will fire from the first one
			//(on top), unless it is destroyed
			if (attackingUnit.GetComponent<StarbasePhasorSection1> ().isDestroyed == false) {

				//we need to snap the starting X-Y coordinates of the phasor shots to the right pixel in the sprite
				//this is also relative to the attacking unit parent object transform position
				//all lines will have the same starting point
				for (int i = 0; i < phasorLineRenderer.Length; i++) {

					//the start point adjusts for the attackingUnitParent, then the section sprite, then finally an offset
					phasorLineRenderer [i].Points [0] = new Vector2 (
						attackingUnitParent.transform.localPosition.x + attackingUnitSections [0].transform.localPosition.x + starbasePhasorSection1AttackOffset.x,
						attackingUnitParent.transform.localPosition.y + attackingUnitSections [0].transform.localPosition.y + starbasePhasorSection1AttackOffset.y);

					//cache the start point
					phasorLineStartPoints[i] = phasorLineRenderer [i].Points [0];

					//inactivate the renderer
					phasorLineRenderer [i].gameObject.SetActive(false);

				}

			} else {

				//the else condition is that the Phasor1 section is destroyed.  In this case, we assume that we are shooting from phasor 2
				for (int i = 0; i < phasorLineRenderer.Length; i++) {

					//the start point adjusts for the attackingUnitParent, then the section sprite, then finally an offset
					phasorLineRenderer [i].Points [0] = new Vector2 (
						attackingUnitParent.transform.localPosition.x + attackingUnitSections [1].transform.localPosition.x + starbasePhasorSection2AttackOffset.x,
						attackingUnitParent.transform.localPosition.y + attackingUnitSections [1].transform.localPosition.y + starbasePhasorSection2AttackOffset.y);

					//cache the start point
					phasorLineStartPoints[i] = phasorLineRenderer [i].Points [0];

					//inactivate the renderer
					phasorLineRenderer [i].gameObject.SetActive(false);

				}

			}

			break;

		case CombatUnit.UnitType.Starship:

			//we need to snap the starting X-Y coordinates of the phasor shots to the right pixel in the sprite
			//this is also relative to the attacking unit parent object transform position
			//all lines will have the same starting point
			for (int i = 0; i < phasorLineRenderer.Length; i++) {

				//the start point adjusts for the attackingUnitParent, then the section sprite, then finally an offset
				phasorLineRenderer [i].Points [0] = new Vector2 (
					attackingUnitParent.transform.localPosition.x + attackingUnitSections [0].transform.localPosition.x + shipPhasorSectionAttackOffset.x,
					attackingUnitParent.transform.localPosition.y + attackingUnitSections [0].transform.localPosition.y + shipPhasorSectionAttackOffset.y);

				//cache the start point
				phasorLineStartPoints[i] = phasorLineRenderer [i].Points [0];

				//inactivate the renderer
				phasorLineRenderer [i].gameObject.SetActive(false);

			}

			break;

		case CombatUnit.UnitType.Destroyer:

			//we need to snap the starting X-Y coordinates of the phasor shots to the right pixel in the sprite
			//this is also relative to the attacking unit parent object transform position
			//all lines will have the same starting point
			for (int i = 0; i < phasorLineRenderer.Length; i++) {

				//the start point adjusts for the attackingUnitParent, then the section sprite, then finally an offset
				phasorLineRenderer [i].Points [0] = new Vector2 (
					attackingUnitParent.transform.localPosition.x + attackingUnitSections [0].transform.localPosition.x + shipPhasorSectionAttackOffset.x,
					attackingUnitParent.transform.localPosition.y + attackingUnitSections [0].transform.localPosition.y + shipPhasorSectionAttackOffset.y);

				//cache the start point
				phasorLineStartPoints[i] = phasorLineRenderer [i].Points [0];

				//inactivate the renderer
				phasorLineRenderer [i].gameObject.SetActive(false);

			}

			break;

		case CombatUnit.UnitType.BirdOfPrey:

			//we need to snap the starting X-Y coordinates of the phasor shots to the right pixel in the sprite
			//this is also relative to the attacking unit parent object transform position
			//all lines will have the same starting point
			for (int i = 0; i < phasorLineRenderer.Length; i++) {

				//the start point adjusts for the attackingUnitParent, then the section sprite, then finally an offset
				phasorLineRenderer [i].Points [0] = new Vector2 (
					attackingUnitParent.transform.localPosition.x + attackingUnitSections [0].transform.localPosition.x + shipPhasorSectionAttackOffset.x,
					attackingUnitParent.transform.localPosition.y + attackingUnitSections [0].transform.localPosition.y + shipPhasorSectionAttackOffset.y);

				//cache the start point
				phasorLineStartPoints[i] = phasorLineRenderer [i].Points [0];

				//inactivate the renderer
				phasorLineRenderer [i].gameObject.SetActive(false);

			}

			break;

		case CombatUnit.UnitType.Scout:

			//we need to snap the starting X-Y coordinates of the phasor shots to the right pixel in the sprite
			//this is also relative to the attacking unit parent object transform position
			//all lines will have the same starting point
			for (int i = 0; i < phasorLineRenderer.Length; i++) {

				//the start point adjusts for the attackingUnitParent, then the section sprite, then finally an offset
				phasorLineRenderer [i].Points [0] = new Vector2 (
					attackingUnitParent.transform.localPosition.x + attackingUnitSections [0].transform.localPosition.x + shipPhasorSectionAttackOffset.x,
					attackingUnitParent.transform.localPosition.y + attackingUnitSections [0].transform.localPosition.y + shipPhasorSectionAttackOffset.y);

				//cache the start point
				phasorLineStartPoints[i] = phasorLineRenderer [i].Points [0];

				//inactivate the renderer
				phasorLineRenderer [i].gameObject.SetActive(false);

			}

			break;

		default:

			break;


		}

	}

	//this function sets the phasor line ending points for a section targeted
	private void SetPhasorLineEndingPoints(CombatUnit targetedUnit, CombatManager.ShipSectionTargeted shipSectionTargeted){

		//do a switch case on the targeted unit type
		switch (targetedUnit.unitType) {

		case CombatUnit.UnitType.Starbase:

			//if we have a starbase, we want to cast the shipSectionTargeted to a baseSectionTargeted, since both are 0-5 enums
			CombatManager.BaseSectionTargeted baseSectionTargeted = (CombatManager.BaseSectionTargeted)shipSectionTargeted;

			//do a switch case on the section targeted
			switch (baseSectionTargeted) {

			case CombatManager.BaseSectionTargeted.PhasorSection1:

				//loop through each of our phasor lines
				for (int i = 0; i < phasorLineRenderer.Length; i++) {

					phasorLineEndPoints [i] = new Vector2 (
						targetedUnitParent.transform.localPosition.x + targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)baseSectionTargeted].transform.localPosition.x
						+ basePhasorSection1TargetedOffset.x + basePhasorSection1TargetedOffsetSpacing.x * i,
						targetedUnitParent.transform.localPosition.y + targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)baseSectionTargeted].transform.localPosition.y
						+ basePhasorSection1TargetedOffset.y + basePhasorSection1TargetedOffsetSpacing.y * i); 
					
				}

				break;

			case CombatManager.BaseSectionTargeted.PhasorSection2:

				//loop through each of our phasor lines
				for (int i = 0; i < phasorLineRenderer.Length; i++) {

					phasorLineEndPoints [i] = new Vector2 (
						targetedUnitParent.transform.localPosition.x + targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)baseSectionTargeted].transform.localPosition.x
						+ basePhasorSection2TargetedOffset.x + basePhasorSection2TargetedOffsetSpacing.x * i,
						targetedUnitParent.transform.localPosition.y + targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)baseSectionTargeted].transform.localPosition.y
						+ basePhasorSection2TargetedOffset.y + basePhasorSection2TargetedOffsetSpacing.y * i); 

				}

				break;

			case CombatManager.BaseSectionTargeted.TorpedoSection:

				//loop through each of our phasor lines
				for (int i = 0; i < phasorLineRenderer.Length; i++) {

					phasorLineEndPoints [i] = new Vector2 (
						targetedUnitParent.transform.localPosition.x + targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)baseSectionTargeted].transform.localPosition.x
						+ baseTorpedoSectionTargetedOffset.x + baseTorpedoSectionTargetedOffsetSpacing.x * i,
						targetedUnitParent.transform.localPosition.y + targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)baseSectionTargeted].transform.localPosition.y
						+ baseTorpedoSectionTargetedOffset.y + baseTorpedoSectionTargetedOffsetSpacing.y * i); 

				}

				break;

			case CombatManager.BaseSectionTargeted.CrewSection:

				//loop through each of our phasor lines
				for (int i = 0; i < phasorLineRenderer.Length; i++) {

					phasorLineEndPoints [i] = new Vector2 (
						targetedUnitParent.transform.localPosition.x + targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)baseSectionTargeted].transform.localPosition.x
						+ baseCrewSectionTargetedOffset.x + baseCrewSectionTargetedOffsetSpacing.x * i,
						targetedUnitParent.transform.localPosition.y + targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)baseSectionTargeted].transform.localPosition.y
						+ baseCrewSectionTargetedOffset.y + baseCrewSectionTargetedOffsetSpacing.y * i); 

				}

				break;

			case CombatManager.BaseSectionTargeted.StorageSection1:

				//loop through each of our phasor lines
				for (int i = 0; i < phasorLineRenderer.Length; i++) {

					phasorLineEndPoints [i] = new Vector2 (
						targetedUnitParent.transform.localPosition.x + targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)baseSectionTargeted].transform.localPosition.x
						+ baseStorageSection1TargetedOffset.x + baseStorageSection1TargetedOffsetSpacing.x * i,
						targetedUnitParent.transform.localPosition.y + targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)baseSectionTargeted].transform.localPosition.y
						+ baseStorageSection1TargetedOffset.y + baseStorageSection1TargetedOffsetSpacing.y * i); 

				}

				break;

			case CombatManager.BaseSectionTargeted.StorageSection2:

				//loop through each of our phasor lines
				for (int i = 0; i < phasorLineRenderer.Length; i++) {

					phasorLineEndPoints [i] = new Vector2 (
						targetedUnitParent.transform.localPosition.x + targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)baseSectionTargeted].transform.localPosition.x
						+ baseStorageSection2TargetedOffset.x + baseStorageSection2TargetedOffsetSpacing.x * i,
						targetedUnitParent.transform.localPosition.y + targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)baseSectionTargeted].transform.localPosition.y
						+ baseStorageSection2TargetedOffset.y + baseStorageSection2TargetedOffsetSpacing.y * i); 

				}

				break;

			default:

				break;

			}

			break;

		case CombatUnit.UnitType.Starship:

			//do a switch case on the section targeted
			switch (shipSectionTargeted) {

			case CombatManager.ShipSectionTargeted.PhasorSection:

				//loop through each of our phasor lines
				for (int i = 0; i < phasorLineRenderer.Length; i++) {

					phasorLineEndPoints [i] = new Vector2 (
						targetedUnitParent.transform.localPosition.x + targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)shipSectionTargeted].transform.localPosition.x
						+ shipPhasorSectionTargetedOffset.x + shipPhasorSectionTargetedOffsetSpacing.x * i,
						targetedUnitParent.transform.localPosition.y + targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)shipSectionTargeted].transform.localPosition.y
						+ shipPhasorSectionTargetedOffset.y + shipPhasorSectionTargetedOffsetSpacing.y * i); 

				}

				break;

			case CombatManager.ShipSectionTargeted.TorpedoSection:

				//loop through each of our phasor lines
				for (int i = 0; i < phasorLineRenderer.Length; i++) {

					phasorLineEndPoints [i] = new Vector2 (
						targetedUnitParent.transform.localPosition.x + targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)shipSectionTargeted].transform.localPosition.x
						+ shipTorpedoSectionTargetedOffset.x + shipTorpedoSectionTargetedOffsetSpacing.x * i,
						targetedUnitParent.transform.localPosition.y + targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)shipSectionTargeted].transform.localPosition.y
						+ shipTorpedoSectionTargetedOffset.y + shipTorpedoSectionTargetedOffsetSpacing.y * i); 

				}

				break;

			case CombatManager.ShipSectionTargeted.StorageSection:

				//loop through each of our phasor lines
				for (int i = 0; i < phasorLineRenderer.Length; i++) {

					phasorLineEndPoints [i] = new Vector2 (
						targetedUnitParent.transform.localPosition.x + targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)shipSectionTargeted].transform.localPosition.x
						+ shipStorageSectionTargetedOffset.x + shipStorageSectionTargetedOffsetSpacing.x * i,
						targetedUnitParent.transform.localPosition.y + targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)shipSectionTargeted].transform.localPosition.y
						+ shipStorageSectionTargetedOffset.y + shipStorageSectionTargetedOffsetSpacing.y * i); 

				}

				break;

			case CombatManager.ShipSectionTargeted.CrewSection:

				//loop through each of our phasor lines
				for (int i = 0; i < phasorLineRenderer.Length; i++) {

					phasorLineEndPoints [i] = new Vector2 (
						targetedUnitParent.transform.localPosition.x + targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)shipSectionTargeted].transform.localPosition.x
						+ shipCrewSectionTargetedOffset.x + shipCrewSectionTargetedOffsetSpacing.x * i,
						targetedUnitParent.transform.localPosition.y + targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)shipSectionTargeted].transform.localPosition.y
						+ shipCrewSectionTargetedOffset.y + shipCrewSectionTargetedOffsetSpacing.y * i); 

				}

				break;

			case CombatManager.ShipSectionTargeted.EngineSection:

				//loop through each of our phasor lines
				for (int i = 0; i < phasorLineRenderer.Length; i++) {

					phasorLineEndPoints [i] = new Vector2 (
						targetedUnitParent.transform.localPosition.x + targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)shipSectionTargeted].transform.localPosition.x
						+ shipEngineSectionTargetedOffset.x + shipEngineSectionTargetedOffsetSpacing.x * i,
						targetedUnitParent.transform.localPosition.y + targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)shipSectionTargeted].transform.localPosition.y
						+ shipEngineSectionTargetedOffset.y + shipEngineSectionTargetedOffsetSpacing.y * i); 

				}

				break;

			default:

				break;

			}

			break;

		case CombatUnit.UnitType.Destroyer:

				//do a switch case on the section targeted
			switch (shipSectionTargeted) {

			case CombatManager.ShipSectionTargeted.PhasorSection:

					//loop through each of our phasor lines
				for (int i = 0; i < phasorLineRenderer.Length; i++) {

					phasorLineEndPoints [i] = new Vector2 (
						targetedUnitParent.transform.localPosition.x + targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)shipSectionTargeted].transform.localPosition.x
						+ shipPhasorSectionTargetedOffset.x + shipPhasorSectionTargetedOffsetSpacing.x * i,
						targetedUnitParent.transform.localPosition.y + targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)shipSectionTargeted].transform.localPosition.y
						+ shipPhasorSectionTargetedOffset.y + shipPhasorSectionTargetedOffsetSpacing.y * i); 

				}

				break;

			case CombatManager.ShipSectionTargeted.TorpedoSection:

					//loop through each of our phasor lines
				for (int i = 0; i < phasorLineRenderer.Length; i++) {

					phasorLineEndPoints [i] = new Vector2 (
						targetedUnitParent.transform.localPosition.x + targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)shipSectionTargeted].transform.localPosition.x
						+ shipTorpedoSectionTargetedOffset.x + shipTorpedoSectionTargetedOffsetSpacing.x * i,
						targetedUnitParent.transform.localPosition.y + targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)shipSectionTargeted].transform.localPosition.y
						+ shipTorpedoSectionTargetedOffset.y + shipTorpedoSectionTargetedOffsetSpacing.y * i); 

				}

				break;

			case CombatManager.ShipSectionTargeted.StorageSection:

					//loop through each of our phasor lines
				for (int i = 0; i < phasorLineRenderer.Length; i++) {

					phasorLineEndPoints [i] = new Vector2 (
						targetedUnitParent.transform.localPosition.x + targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)shipSectionTargeted].transform.localPosition.x
						+ shipStorageSectionTargetedOffset.x + shipStorageSectionTargetedOffsetSpacing.x * i,
						targetedUnitParent.transform.localPosition.y + targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)shipSectionTargeted].transform.localPosition.y
						+ shipStorageSectionTargetedOffset.y + shipStorageSectionTargetedOffsetSpacing.y * i); 

				}

				break;

			case CombatManager.ShipSectionTargeted.EngineSection:

					//loop through each of our phasor lines
				for (int i = 0; i < phasorLineRenderer.Length; i++) {

					phasorLineEndPoints [i] = new Vector2 (
						targetedUnitParent.transform.localPosition.x + targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)shipSectionTargeted].transform.localPosition.x
						+ shipEngineSectionTargetedOffset.x + shipEngineSectionTargetedOffsetSpacing.x * i,
						targetedUnitParent.transform.localPosition.y + targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)shipSectionTargeted].transform.localPosition.y
						+ shipEngineSectionTargetedOffset.y + shipEngineSectionTargetedOffsetSpacing.y * i); 

				}

				break;

			default:

				break;

			}

			break;

		case CombatUnit.UnitType.BirdOfPrey:

			//do a switch case on the section targeted
			switch (shipSectionTargeted) {

			case CombatManager.ShipSectionTargeted.PhasorSection:

				//loop through each of our phasor lines
				for (int i = 0; i < phasorLineRenderer.Length; i++) {

					phasorLineEndPoints [i] = new Vector2 (
						targetedUnitParent.transform.localPosition.x + targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)shipSectionTargeted].transform.localPosition.x
						+ shipPhasorSectionTargetedOffset.x + shipPhasorSectionTargetedOffsetSpacing.x * i,
						targetedUnitParent.transform.localPosition.y + targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)shipSectionTargeted].transform.localPosition.y
						+ shipPhasorSectionTargetedOffset.y + shipPhasorSectionTargetedOffsetSpacing.y * i); 

				}

				break;

			case CombatManager.ShipSectionTargeted.TorpedoSection:

				//loop through each of our phasor lines
				for (int i = 0; i < phasorLineRenderer.Length; i++) {

					phasorLineEndPoints [i] = new Vector2 (
						targetedUnitParent.transform.localPosition.x + targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)shipSectionTargeted].transform.localPosition.x
						+ shipTorpedoSectionTargetedOffset.x + shipTorpedoSectionTargetedOffsetSpacing.x * i,
						targetedUnitParent.transform.localPosition.y + targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)shipSectionTargeted].transform.localPosition.y
						+ shipTorpedoSectionTargetedOffset.y + shipTorpedoSectionTargetedOffsetSpacing.y * i); 

				}

				break;

			case CombatManager.ShipSectionTargeted.EngineSection:

				//loop through each of our phasor lines
				for (int i = 0; i < phasorLineRenderer.Length; i++) {

					phasorLineEndPoints [i] = new Vector2 (
						targetedUnitParent.transform.localPosition.x + targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)shipSectionTargeted].transform.localPosition.x
						+ shipEngineSectionTargetedOffset.x + shipEngineSectionTargetedOffsetSpacing.x * i,
						targetedUnitParent.transform.localPosition.y + targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)shipSectionTargeted].transform.localPosition.y
						+ shipEngineSectionTargetedOffset.y + shipEngineSectionTargetedOffsetSpacing.y * i); 

				}

				break;

			default:

				break;

			}

			break;

		case CombatUnit.UnitType.Scout:

			//do a switch case on the section targeted
			switch (shipSectionTargeted) {

			case CombatManager.ShipSectionTargeted.PhasorSection:

				//loop through each of our phasor lines
				for (int i = 0; i < phasorLineRenderer.Length; i++) {

					phasorLineEndPoints [i] = new Vector2 (
						targetedUnitParent.transform.localPosition.x + targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)shipSectionTargeted].transform.localPosition.x
						+ shipPhasorSectionTargetedOffset.x + shipPhasorSectionTargetedOffsetSpacing.x * i,
						targetedUnitParent.transform.localPosition.y + targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)shipSectionTargeted].transform.localPosition.y
						+ shipPhasorSectionTargetedOffset.y + shipPhasorSectionTargetedOffsetSpacing.y * i); 

				}

				break;

			case CombatManager.ShipSectionTargeted.StorageSection:

				//loop through each of our phasor lines
				for (int i = 0; i < phasorLineRenderer.Length; i++) {

					phasorLineEndPoints [i] = new Vector2 (
						targetedUnitParent.transform.localPosition.x + targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)shipSectionTargeted].transform.localPosition.x
						+ shipStorageSectionTargetedOffset.x + shipStorageSectionTargetedOffsetSpacing.x * i,
						targetedUnitParent.transform.localPosition.y + targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)shipSectionTargeted].transform.localPosition.y
						+ shipStorageSectionTargetedOffset.y + shipStorageSectionTargetedOffsetSpacing.y * i); 

				}

				break;

			case CombatManager.ShipSectionTargeted.EngineSection:

				//loop through each of our phasor lines
				for (int i = 0; i < phasorLineRenderer.Length; i++) {

					phasorLineEndPoints [i] = new Vector2 (
						targetedUnitParent.transform.localPosition.x + targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)shipSectionTargeted].transform.localPosition.x
						+ shipEngineSectionTargetedOffset.x + shipEngineSectionTargetedOffsetSpacing.x * i,
						targetedUnitParent.transform.localPosition.y + targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)shipSectionTargeted].transform.localPosition.y
						+ shipEngineSectionTargetedOffset.y + shipEngineSectionTargetedOffsetSpacing.y * i); 

				}

				break;

			default:

				break;

			}
				
			break;

		default:

			break;

		}

	}

	//this function returns a shipSectionTargeted for a given unit that is randomly one of it's alive sections
	private CombatManager.ShipSectionTargeted randomSectionTargeted(CombatUnit targetedUnit){

		//we will create a list to keep track of the alive sections
		List<CombatManager.ShipSectionTargeted> possibleSections = new List<CombatManager.ShipSectionTargeted>();

		//do a switch case based on unit time
		switch (targetedUnit.unitType) {

		case CombatUnit.UnitType.Starship:

			if (targetedUnit.GetComponent<PhasorSection> ().isDestroyed == false) {

				possibleSections.Add (CombatManager.ShipSectionTargeted.PhasorSection);

			}

			if (targetedUnit.GetComponent<TorpedoSection> ().isDestroyed == false) {

				possibleSections.Add (CombatManager.ShipSectionTargeted.TorpedoSection);

			}

			if (targetedUnit.GetComponent<StorageSection> ().isDestroyed == false) {

				possibleSections.Add (CombatManager.ShipSectionTargeted.StorageSection);

			}

			if (targetedUnit.GetComponent<CrewSection> ().isDestroyed == false) {

				possibleSections.Add (CombatManager.ShipSectionTargeted.CrewSection);

			}

			if (targetedUnit.GetComponent<EngineSection> ().isDestroyed == false) {

				possibleSections.Add (CombatManager.ShipSectionTargeted.EngineSection);

			}

			break;

		case CombatUnit.UnitType.Destroyer:

			if (targetedUnit.GetComponent<PhasorSection> ().isDestroyed == false) {

				possibleSections.Add (CombatManager.ShipSectionTargeted.PhasorSection);

			}

			if (targetedUnit.GetComponent<TorpedoSection> ().isDestroyed == false) {

				possibleSections.Add (CombatManager.ShipSectionTargeted.TorpedoSection);

			}

			if (targetedUnit.GetComponent<StorageSection> ().isDestroyed == false) {

				possibleSections.Add (CombatManager.ShipSectionTargeted.StorageSection);

			}

			if (targetedUnit.GetComponent<EngineSection> ().isDestroyed == false) {

				possibleSections.Add (CombatManager.ShipSectionTargeted.EngineSection);

			}

			break;

		case CombatUnit.UnitType.BirdOfPrey:

			if (targetedUnit.GetComponent<PhasorSection> ().isDestroyed == false) {

				possibleSections.Add (CombatManager.ShipSectionTargeted.PhasorSection);

			}

			if (targetedUnit.GetComponent<TorpedoSection> ().isDestroyed == false) {

				possibleSections.Add (CombatManager.ShipSectionTargeted.TorpedoSection);

			}

			if (targetedUnit.GetComponent<EngineSection> ().isDestroyed == false) {

				possibleSections.Add (CombatManager.ShipSectionTargeted.EngineSection);

			}

			break;

		case CombatUnit.UnitType.Scout:

			if (targetedUnit.GetComponent<PhasorSection> ().isDestroyed == false) {

				possibleSections.Add (CombatManager.ShipSectionTargeted.PhasorSection);

			}


			if (targetedUnit.GetComponent<StorageSection> ().isDestroyed == false) {

				possibleSections.Add (CombatManager.ShipSectionTargeted.StorageSection);

			}

			if (targetedUnit.GetComponent<EngineSection> ().isDestroyed == false) {

				possibleSections.Add (CombatManager.ShipSectionTargeted.EngineSection);

			}

			break;

		case CombatUnit.UnitType.Starbase:

			if (targetedUnit.GetComponent<StarbasePhasorSection1> ().isDestroyed == false) {

				possibleSections.Add ((CombatManager.ShipSectionTargeted)CombatManager.BaseSectionTargeted.PhasorSection1);

			}

			if (targetedUnit.GetComponent<StarbasePhasorSection2> ().isDestroyed == false) {

				possibleSections.Add ((CombatManager.ShipSectionTargeted)CombatManager.BaseSectionTargeted.PhasorSection2);

			}

			if (targetedUnit.GetComponent<StarbaseTorpedoSection> ().isDestroyed == false) {

				possibleSections.Add ((CombatManager.ShipSectionTargeted)CombatManager.BaseSectionTargeted.TorpedoSection);

			}

			if (targetedUnit.GetComponent<StarbaseCrewSection> ().isDestroyed == false) {

				possibleSections.Add ((CombatManager.ShipSectionTargeted)CombatManager.BaseSectionTargeted.CrewSection);

			}

			if (targetedUnit.GetComponent<StarbaseStorageSection1> ().isDestroyed == false) {

				possibleSections.Add ((CombatManager.ShipSectionTargeted)CombatManager.BaseSectionTargeted.StorageSection1);

			}

			if (targetedUnit.GetComponent<StarbaseStorageSection2> ().isDestroyed == false) {

				possibleSections.Add ((CombatManager.ShipSectionTargeted)CombatManager.BaseSectionTargeted.StorageSection2);

			}

			break;

		default:

			break;

		}

		//now we can choose a random item from the list
		return(possibleSections[Random.Range (0, possibleSections.Count)]);

	}

	//this function will extend the phasor end points to the border of the cutscene panel, to be used for a miss
	private void ExtendPhasorEndPointsToPanel(){

		//floats to determine the xmax and ymin, ymax allowable for the end points
		float xmax;
		float ymin;
		float ymax;

		//the xmax will be half of the cutscene panel width
		xmax = cutscenePanel.GetComponent<RectTransform> ().rect.width / 2.0f;

		//the ymin and ymax will be half of the cutscene panel height
		ymin = - cutscenePanel.GetComponent<RectTransform> ().rect.height / 2.0f;
		ymax = cutscenePanel.GetComponent<RectTransform> ().rect.height / 2.0f;

		//loop through each end point
		for (int i = 0; i < phasorLineEndPoints.Length; i++) {

			//define a temporary vector2 to hold the end point
			Vector2 tempEndPoint;

			//define a temporary variable for line slope
			float lineSlope;

			//calculate the slope to be delta rise / delta run
			lineSlope = (phasorLineEndPoints [i].y - phasorLineStartPoints [i].y) / (phasorLineEndPoints [i].x - phasorLineStartPoints [i].x);

			//take the current end point, and scale it's x value based on the slope
			tempEndPoint = new Vector2 (xmax, phasorLineEndPoints [i].y + (xmax - phasorLineEndPoints [i].x) * lineSlope);

			//check if the tempEndPoint y value is below the min
			if (tempEndPoint.y < ymin) {

				//if the y is less than the ymin, it means our line should have hit the bottom of the window and stopped instead of the right edge
				//in this case, we need to scale the x back to the ymin
				tempEndPoint = new Vector2 (tempEndPoint.x + (ymin - tempEndPoint.y) / lineSlope, ymin);

			} else if (tempEndPoint.y > ymax) {

				//else we want to check if y is greater than the ymax, it means our line should have hit the top of the window and stopped instead of the right edge
				//in this case, we need to scale the x back to the ymax
				tempEndPoint = new Vector2 (tempEndPoint.x + (ymax - tempEndPoint.y) / lineSlope, ymin);

			}

			//now, we can set the phasorLineEndPoints to the tempEndPoint
			phasorLineEndPoints[i] = tempEndPoint;

		}

	}

	//this function will add randomness to the phasor line end points
	private void AddNoiseToPhasorLineEndPoints(){

		//variable to control magnitude of randomness in pixels
		int randomAmplitude = 5;

		for (int i = 0; i < phasorLineEndPoints.Length; i++) {

			//define a temporary Vector2 for the randomized end point
			Vector2 tempEndPoint = new Vector2 (phasorLineEndPoints [i].x + Random.Range (-randomAmplitude, randomAmplitude + 1), 
				                       phasorLineEndPoints [i].y + Random.Range (-randomAmplitude, randomAmplitude + 1));

			//set the actual end point to the temporary end point
			phasorLineEndPoints[i] = tempEndPoint;

		}

	}

	//this function sets the combat units off-screen to start, so they can make a dramatic entrance
	private void PositionCombatUnitsOffCamera(){

		//adjust the attacking unit to the left
		attackingUnitParent.transform.localPosition = new Vector3(-cutscenePanel.GetComponent<RectTransform> ().rect.width / 2.0f -
			attackingUnitParent.GetComponent<RectTransform> ().rect.width / 2.0f,
			attackingUnitParent.transform.localPosition.y,
			attackingUnitParent.transform.localPosition.z);
		
		//do a similar adjustment for the targeted unit to the right
		targetedUnitParent.transform.localPosition = new Vector3(cutscenePanel.GetComponent<RectTransform> ().rect.width / 2.0f +
			targetedUnitParent.GetComponent<RectTransform> ().rect.width / 2.0f,
			targetedUnitParent.transform.localPosition.y,
			targetedUnitParent.transform.localPosition.z);

	}

	//this function sets the combat units starting elevation, so they can make a dramatic entrance
	private void SetCombatUnitsVerticalPosition(CombatUnit attackingUnit, CombatUnit targetedUnit){

		//check if the attacking unit is a starbase
		if (attackingUnit.GetComponent<Starbase> () == true) {

			//if we have a starbase, I don't want to adjust the y
			//set it to zero in case a previous cutscene had moved it
			attackingUnitParent.transform.localPosition = new Vector3(attackingUnitParent.transform.localPosition.x,
				0.0f,
				attackingUnitParent.transform.localPosition.z);

		} else {

			//the else condition is that we don't have a starbase, so it's a ship
			//in this case, I want to adjust the the y
			//for the y, I want to introduce a bit of randomness to the height
			//plus or minus 1/7
			attackingUnitParent.transform.localPosition = new Vector3(attackingUnitParent.transform.localPosition.x,
				Random.Range (-cutscenePanel.GetComponent<RectTransform> ().rect.height / 7.0f, cutscenePanel.GetComponent<RectTransform> ().rect.height / 7.0f),
				attackingUnitParent.transform.localPosition.z);

		}

		//do a similar adjustment for the targeted unit
		//just need to do positive x instead of negative
		//check if the targeted unit is a starbase
		if (targetedUnit.GetComponent<Starbase> () == true) {

			//if we have a starbase, I don't want to adjust the y
			//set it to zero in case a previous cutscene had moved it
			targetedUnitParent.transform.localPosition = new Vector3(targetedUnitParent.transform.localPosition.x,
				0.0f,
				targetedUnitParent.transform.localPosition.z);

		} else {

			//the else condition is that we don't have a starbase, so it's a ship
			//in this case, I want to adjust the y
			//for the y, I want to introduce a bit of randomness to the height
			//plus or minus 1/7
			targetedUnitParent.transform.localPosition = new Vector3(targetedUnitParent.transform.localPosition.x,
				Random.Range (-cutscenePanel.GetComponent<RectTransform> ().rect.height / 7.0f, cutscenePanel.GetComponent<RectTransform> ().rect.height / 7.0f),
				targetedUnitParent.transform.localPosition.z);

		}

	}

	//this function sets the torpedo starting position given an attacking unit
	private void SetTorpedoStartingPosition(CombatUnit attackingUnit){

		//where the torpedo shot originates from depends on the kind of unit
		switch (attackingUnit.unitType) {

		case CombatUnit.UnitType.Starbase:

			//the start point adjusts for the attackingUnitParent, then the section sprite, then finally an offset
			torpedoStartingPosition = new Vector2 (
				attackingUnitParent.transform.localPosition.x + attackingUnitSections [(int)CombatManager.BaseSectionTargeted.TorpedoSection].transform.localPosition.x + starbaseTorpedoSectionAttackOffset.x,
				attackingUnitParent.transform.localPosition.y + attackingUnitSections [(int)CombatManager.BaseSectionTargeted.TorpedoSection].transform.localPosition.y + starbaseTorpedoSectionAttackOffset.y);

			break;

		case CombatUnit.UnitType.Starship:

			//the start point adjusts for the attackingUnitParent, then the section sprite, then finally an offset
			torpedoStartingPosition = new Vector2 (
				attackingUnitParent.transform.localPosition.x + attackingUnitSections [(int)CombatManager.ShipSectionTargeted.TorpedoSection].transform.localPosition.x + shipTorpedoSectionAttackOffset.x,
				attackingUnitParent.transform.localPosition.y + attackingUnitSections [(int)CombatManager.ShipSectionTargeted.TorpedoSection].transform.localPosition.y + shipTorpedoSectionAttackOffset.y);

			break;

		case CombatUnit.UnitType.Destroyer:

			//the start point adjusts for the attackingUnitParent, then the section sprite, then finally an offset
			torpedoStartingPosition = new Vector2 (
				attackingUnitParent.transform.localPosition.x + attackingUnitSections [(int)CombatManager.ShipSectionTargeted.TorpedoSection].transform.localPosition.x + shipTorpedoSectionAttackOffset.x,
				attackingUnitParent.transform.localPosition.y + attackingUnitSections [(int)CombatManager.ShipSectionTargeted.TorpedoSection].transform.localPosition.y + shipTorpedoSectionAttackOffset.y);

			break;

		case CombatUnit.UnitType.BirdOfPrey:

			//the start point adjusts for the attackingUnitParent, then the section sprite, then finally an offset
			torpedoStartingPosition = new Vector2 (
				attackingUnitParent.transform.localPosition.x + attackingUnitSections [(int)CombatManager.ShipSectionTargeted.TorpedoSection].transform.localPosition.x + shipTorpedoSectionAttackOffset.x,
				attackingUnitParent.transform.localPosition.y + attackingUnitSections [(int)CombatManager.ShipSectionTargeted.TorpedoSection].transform.localPosition.y + shipTorpedoSectionAttackOffset.y);

			break;

		case CombatUnit.UnitType.Scout:

			//the start point adjusts for the attackingUnitParent, then the section sprite, then finally an offset
			torpedoStartingPosition = new Vector2 (
				attackingUnitParent.transform.localPosition.x + attackingUnitSections [(int)CombatManager.ShipSectionTargeted.TorpedoSection].transform.localPosition.x + shipTorpedoSectionAttackOffset.x,
				attackingUnitParent.transform.localPosition.y + attackingUnitSections [(int)CombatManager.ShipSectionTargeted.TorpedoSection].transform.localPosition.y + shipTorpedoSectionAttackOffset.y);

			break;

		default:

			break;

		}

	}

	//this function sets the torpedo destination location given a targeted unit and a section targeted
	private void SetTorpedoDestinationPosition(CombatUnit targetedUnit, CombatManager.ShipSectionTargeted shipSectionTargeted){

		//do a switch case on the targeted unit type
		switch (targetedUnit.unitType) {

		case CombatUnit.UnitType.Starbase:

			//if we have a starbase, we want to cast the shipSectionTargeted to a baseSectionTargeted, since both are 0-5 enums
			CombatManager.BaseSectionTargeted baseSectionTargeted = (CombatManager.BaseSectionTargeted)shipSectionTargeted;

			//do a switch case on the section targeted
			switch (baseSectionTargeted) {

			case CombatManager.BaseSectionTargeted.PhasorSection1:

				torpedoDestinationPosition = new Vector2 (
					targetedUnitParent.transform.localPosition.x + targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)baseSectionTargeted].transform.localPosition.x
					+ basePhasorSection1TargetedOffset.x + basePhasorSection1TargetedOffsetSpacing.x * Random.Range(0,phasorLineRenderer.Length),
					targetedUnitParent.transform.localPosition.y + targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)baseSectionTargeted].transform.localPosition.y
					+ basePhasorSection1TargetedOffset.y + basePhasorSection1TargetedOffsetSpacing.y * Random.Range(0,phasorLineRenderer.Length)); 

				break;

			case CombatManager.BaseSectionTargeted.PhasorSection2:

				torpedoDestinationPosition = new Vector2 (
					targetedUnitParent.transform.localPosition.x + targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)baseSectionTargeted].transform.localPosition.x
					+ basePhasorSection2TargetedOffset.x + basePhasorSection2TargetedOffsetSpacing.x * Random.Range(0,phasorLineRenderer.Length),
					targetedUnitParent.transform.localPosition.y + targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)baseSectionTargeted].transform.localPosition.y
					+ basePhasorSection2TargetedOffset.y + basePhasorSection2TargetedOffsetSpacing.y * Random.Range(0,phasorLineRenderer.Length));  

				break;

			case CombatManager.BaseSectionTargeted.TorpedoSection:

				torpedoDestinationPosition = new Vector2 (
					targetedUnitParent.transform.localPosition.x + targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)baseSectionTargeted].transform.localPosition.x
					+ baseTorpedoSectionTargetedOffset.x + baseTorpedoSectionTargetedOffsetSpacing.x * Random.Range(0,phasorLineRenderer.Length),
					targetedUnitParent.transform.localPosition.y + targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)baseSectionTargeted].transform.localPosition.y
					+ baseTorpedoSectionTargetedOffset.y + baseTorpedoSectionTargetedOffsetSpacing.y * Random.Range(0,phasorLineRenderer.Length)); 

				break;

			case CombatManager.BaseSectionTargeted.CrewSection:

				torpedoDestinationPosition = new Vector2 (
					targetedUnitParent.transform.localPosition.x + targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)baseSectionTargeted].transform.localPosition.x
					+ baseCrewSectionTargetedOffset.x + baseCrewSectionTargetedOffsetSpacing.x * Random.Range(0,phasorLineRenderer.Length),
					targetedUnitParent.transform.localPosition.y + targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)baseSectionTargeted].transform.localPosition.y
					+ baseCrewSectionTargetedOffset.y + baseCrewSectionTargetedOffsetSpacing.y * Random.Range(0,phasorLineRenderer.Length)); 

				break;

			case CombatManager.BaseSectionTargeted.StorageSection1:

				torpedoDestinationPosition = new Vector2 (
					targetedUnitParent.transform.localPosition.x + targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)baseSectionTargeted].transform.localPosition.x
					+ baseStorageSection1TargetedOffset.x + baseStorageSection1TargetedOffsetSpacing.x * Random.Range(0,phasorLineRenderer.Length),
					targetedUnitParent.transform.localPosition.y + targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)baseSectionTargeted].transform.localPosition.y
					+ baseStorageSection1TargetedOffset.y + baseStorageSection1TargetedOffsetSpacing.y * Random.Range(0,phasorLineRenderer.Length)); 

				break;

			case CombatManager.BaseSectionTargeted.StorageSection2:

				torpedoDestinationPosition = new Vector2 (
					targetedUnitParent.transform.localPosition.x + targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)baseSectionTargeted].transform.localPosition.x
					+ baseStorageSection2TargetedOffset.x + baseStorageSection2TargetedOffsetSpacing.x * Random.Range(0,phasorLineRenderer.Length),
					targetedUnitParent.transform.localPosition.y + targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)baseSectionTargeted].transform.localPosition.y
					+ baseStorageSection2TargetedOffset.y + baseStorageSection2TargetedOffsetSpacing.y * Random.Range(0,phasorLineRenderer.Length)); 

				break;

			default:

				break;

			}

			break;

		case CombatUnit.UnitType.Starship:

			//do a switch case on the section targeted
			switch (shipSectionTargeted) {

			case CombatManager.ShipSectionTargeted.PhasorSection:

				torpedoDestinationPosition = new Vector2 (
					targetedUnitParent.transform.localPosition.x + targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)shipSectionTargeted].transform.localPosition.x
					+ shipPhasorSectionTargetedOffset.x + shipPhasorSectionTargetedOffsetSpacing.x * Random.Range(0,phasorLineRenderer.Length),
					targetedUnitParent.transform.localPosition.y + targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)shipSectionTargeted].transform.localPosition.y
					+ shipPhasorSectionTargetedOffset.y + shipPhasorSectionTargetedOffsetSpacing.y * Random.Range(0,phasorLineRenderer.Length));

				break;

			case CombatManager.ShipSectionTargeted.TorpedoSection:

				torpedoDestinationPosition = new Vector2 (
					targetedUnitParent.transform.localPosition.x + targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)shipSectionTargeted].transform.localPosition.x
					+ shipTorpedoSectionTargetedOffset.x + shipTorpedoSectionTargetedOffsetSpacing.x * Random.Range(0,phasorLineRenderer.Length),
					targetedUnitParent.transform.localPosition.y + targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)shipSectionTargeted].transform.localPosition.y
					+ shipTorpedoSectionTargetedOffset.y + shipTorpedoSectionTargetedOffsetSpacing.y * Random.Range(0,phasorLineRenderer.Length)); 

				break;

			case CombatManager.ShipSectionTargeted.StorageSection:

				torpedoDestinationPosition = new Vector2 (
					targetedUnitParent.transform.localPosition.x + targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)shipSectionTargeted].transform.localPosition.x
					+ shipStorageSectionTargetedOffset.x + shipStorageSectionTargetedOffsetSpacing.x * Random.Range(0,phasorLineRenderer.Length),
					targetedUnitParent.transform.localPosition.y + targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)shipSectionTargeted].transform.localPosition.y
					+ shipStorageSectionTargetedOffset.y + shipStorageSectionTargetedOffsetSpacing.y * Random.Range(0,phasorLineRenderer.Length)); 

				break;

			case CombatManager.ShipSectionTargeted.CrewSection:

				torpedoDestinationPosition = new Vector2 (
					targetedUnitParent.transform.localPosition.x + targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)shipSectionTargeted].transform.localPosition.x
					+ shipCrewSectionTargetedOffset.x + shipCrewSectionTargetedOffsetSpacing.x * Random.Range(0,phasorLineRenderer.Length),
					targetedUnitParent.transform.localPosition.y + targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)shipSectionTargeted].transform.localPosition.y
					+ shipCrewSectionTargetedOffset.y + shipCrewSectionTargetedOffsetSpacing.y * Random.Range(0,phasorLineRenderer.Length)); 

				break;

			case CombatManager.ShipSectionTargeted.EngineSection:

				torpedoDestinationPosition = new Vector2 (
					targetedUnitParent.transform.localPosition.x + targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)shipSectionTargeted].transform.localPosition.x
					+ shipEngineSectionTargetedOffset.x + shipEngineSectionTargetedOffsetSpacing.x * Random.Range(0,phasorLineRenderer.Length),
					targetedUnitParent.transform.localPosition.y + targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)shipSectionTargeted].transform.localPosition.y
					+ shipEngineSectionTargetedOffset.y + shipEngineSectionTargetedOffsetSpacing.y * Random.Range(0,phasorLineRenderer.Length)); 

				break;

			default:

				break;

			}

			break;

		case CombatUnit.UnitType.Destroyer:

			//do a switch case on the section targeted
			switch (shipSectionTargeted) {

			case CombatManager.ShipSectionTargeted.PhasorSection:

				torpedoDestinationPosition = new Vector2 (
					targetedUnitParent.transform.localPosition.x + targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)shipSectionTargeted].transform.localPosition.x
					+ shipPhasorSectionTargetedOffset.x + shipPhasorSectionTargetedOffsetSpacing.x * Random.Range(0,phasorLineRenderer.Length),
					targetedUnitParent.transform.localPosition.y + targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)shipSectionTargeted].transform.localPosition.y
					+ shipPhasorSectionTargetedOffset.y + shipPhasorSectionTargetedOffsetSpacing.y * Random.Range(0,phasorLineRenderer.Length));


				break;

			case CombatManager.ShipSectionTargeted.TorpedoSection:

				torpedoDestinationPosition = new Vector2 (
					targetedUnitParent.transform.localPosition.x + targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)shipSectionTargeted].transform.localPosition.x
					+ shipTorpedoSectionTargetedOffset.x + shipTorpedoSectionTargetedOffsetSpacing.x * Random.Range(0,phasorLineRenderer.Length),
					targetedUnitParent.transform.localPosition.y + targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)shipSectionTargeted].transform.localPosition.y
					+ shipTorpedoSectionTargetedOffset.y + shipTorpedoSectionTargetedOffsetSpacing.y * Random.Range(0,phasorLineRenderer.Length)); 


				break;

			case CombatManager.ShipSectionTargeted.StorageSection:

				torpedoDestinationPosition = new Vector2 (
					targetedUnitParent.transform.localPosition.x + targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)shipSectionTargeted].transform.localPosition.x
					+ shipStorageSectionTargetedOffset.x + shipStorageSectionTargetedOffsetSpacing.x * Random.Range(0,phasorLineRenderer.Length),
					targetedUnitParent.transform.localPosition.y + targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)shipSectionTargeted].transform.localPosition.y
					+ shipStorageSectionTargetedOffset.y + shipStorageSectionTargetedOffsetSpacing.y * Random.Range(0,phasorLineRenderer.Length)); 


				break;

			case CombatManager.ShipSectionTargeted.EngineSection:

				torpedoDestinationPosition = new Vector2 (
					targetedUnitParent.transform.localPosition.x + targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)shipSectionTargeted].transform.localPosition.x
					+ shipEngineSectionTargetedOffset.x + shipEngineSectionTargetedOffsetSpacing.x * Random.Range(0,phasorLineRenderer.Length),
					targetedUnitParent.transform.localPosition.y + targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)shipSectionTargeted].transform.localPosition.y
					+ shipEngineSectionTargetedOffset.y + shipEngineSectionTargetedOffsetSpacing.y * Random.Range(0,phasorLineRenderer.Length)); 

				break;

			default:

				break;

			}

			break;

		case CombatUnit.UnitType.BirdOfPrey:

			//do a switch case on the section targeted
			switch (shipSectionTargeted) {

			case CombatManager.ShipSectionTargeted.PhasorSection:

				torpedoDestinationPosition = new Vector2 (
					targetedUnitParent.transform.localPosition.x + targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)shipSectionTargeted].transform.localPosition.x
					+ shipPhasorSectionTargetedOffset.x + shipPhasorSectionTargetedOffsetSpacing.x * Random.Range(0,phasorLineRenderer.Length),
					targetedUnitParent.transform.localPosition.y + targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)shipSectionTargeted].transform.localPosition.y
					+ shipPhasorSectionTargetedOffset.y + shipPhasorSectionTargetedOffsetSpacing.y * Random.Range(0,phasorLineRenderer.Length));


				break;

			case CombatManager.ShipSectionTargeted.TorpedoSection:

				torpedoDestinationPosition = new Vector2 (
					targetedUnitParent.transform.localPosition.x + targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)shipSectionTargeted].transform.localPosition.x
					+ shipTorpedoSectionTargetedOffset.x + shipTorpedoSectionTargetedOffsetSpacing.x * Random.Range(0,phasorLineRenderer.Length),
					targetedUnitParent.transform.localPosition.y + targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)shipSectionTargeted].transform.localPosition.y
					+ shipTorpedoSectionTargetedOffset.y + shipTorpedoSectionTargetedOffsetSpacing.y * Random.Range(0,phasorLineRenderer.Length)); 


				break;

			case CombatManager.ShipSectionTargeted.EngineSection:

				torpedoDestinationPosition = new Vector2 (
					targetedUnitParent.transform.localPosition.x + targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)shipSectionTargeted].transform.localPosition.x
					+ shipEngineSectionTargetedOffset.x + shipEngineSectionTargetedOffsetSpacing.x * Random.Range(0,phasorLineRenderer.Length),
					targetedUnitParent.transform.localPosition.y + targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)shipSectionTargeted].transform.localPosition.y
					+ shipEngineSectionTargetedOffset.y + shipEngineSectionTargetedOffsetSpacing.y * Random.Range(0,phasorLineRenderer.Length)); 

				break;

			default:

				break;

			}

			break;

		case CombatUnit.UnitType.Scout:

			//do a switch case on the section targeted
			switch (shipSectionTargeted) {

			case CombatManager.ShipSectionTargeted.PhasorSection:

				torpedoDestinationPosition = new Vector2 (
					targetedUnitParent.transform.localPosition.x + targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)shipSectionTargeted].transform.localPosition.x
					+ shipPhasorSectionTargetedOffset.x + shipPhasorSectionTargetedOffsetSpacing.x * Random.Range(0,phasorLineRenderer.Length),
					targetedUnitParent.transform.localPosition.y + targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)shipSectionTargeted].transform.localPosition.y
					+ shipPhasorSectionTargetedOffset.y + shipPhasorSectionTargetedOffsetSpacing.y * Random.Range(0,phasorLineRenderer.Length));


				break;

			case CombatManager.ShipSectionTargeted.StorageSection:

				torpedoDestinationPosition = new Vector2 (
					targetedUnitParent.transform.localPosition.x + targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)shipSectionTargeted].transform.localPosition.x
					+ shipStorageSectionTargetedOffset.x + shipStorageSectionTargetedOffsetSpacing.x * Random.Range(0,phasorLineRenderer.Length),
					targetedUnitParent.transform.localPosition.y + targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)shipSectionTargeted].transform.localPosition.y
					+ shipStorageSectionTargetedOffset.y + shipStorageSectionTargetedOffsetSpacing.y * Random.Range(0,phasorLineRenderer.Length)); 


				break;

			case CombatManager.ShipSectionTargeted.EngineSection:

				torpedoDestinationPosition = new Vector2 (
					targetedUnitParent.transform.localPosition.x + targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)shipSectionTargeted].transform.localPosition.x
					+ shipEngineSectionTargetedOffset.x + shipEngineSectionTargetedOffsetSpacing.x * Random.Range(0,phasorLineRenderer.Length),
					targetedUnitParent.transform.localPosition.y + targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)shipSectionTargeted].transform.localPosition.y
					+ shipEngineSectionTargetedOffset.y + shipEngineSectionTargetedOffsetSpacing.y * Random.Range(0,phasorLineRenderer.Length)); 

				break;

			default:

				break;

			}

			break;

		default:

			break;

		}

	}

	//this function will add randomness to the torpedo destination position
	private void AddNoiseToTorpedoDestinationPosition(){

		//variable to control magnitude of randomness in pixels
		int randomAmplitude = 5;

		//define a temporary Vector2 for the randomized end point
		Vector2 tempEndPoint = new Vector2 (torpedoDestinationPosition.x + Random.Range (-randomAmplitude, randomAmplitude + 1), 
			torpedoDestinationPosition.y + Random.Range (-randomAmplitude, randomAmplitude + 1));

		//set the actual end point to the temporary end point
		torpedoDestinationPosition = tempEndPoint;

	}

	//this function will extend the torpedo destination to the border of the cutscene panel, to be used for a miss
	private void ExtendTorpedoDestinationToPanel(){

		//floats to determine the xmax and ymin, ymax allowable for the end points
		float xmax;
		float ymin;
		float ymax;

		//the xmax will be half of the cutscene panel width
		xmax = cutscenePanel.GetComponent<RectTransform> ().rect.width / 2.0f;

		//the ymin and ymax will be half of the cutscene panel height
		ymin = - cutscenePanel.GetComponent<RectTransform> ().rect.height / 2.0f;
		ymax = cutscenePanel.GetComponent<RectTransform> ().rect.height / 2.0f;

		//define a temporary vector2 to hold the end point
		Vector2 tempEndPoint;

		//define a temporary variable for line slope
		float lineSlope;

		//calculate the slope to be delta rise / delta run
		lineSlope = (torpedoDestinationPosition.y - torpedoStartingPosition.y) / (torpedoDestinationPosition.x - torpedoStartingPosition.x);

		//take the current end point, and scale it's x value based on the slope
		tempEndPoint = new Vector2 (xmax, torpedoDestinationPosition.y + (xmax - torpedoDestinationPosition.x) * lineSlope);

		//check if the tempEndPoint y value is below the min
		if (tempEndPoint.y < ymin) {

			//if the y is less than the ymin, it means our line should have hit the bottom of the window and stopped instead of the right edge
			//in this case, we need to scale the x back to the ymin
			tempEndPoint = new Vector2 (tempEndPoint.x + (ymin - tempEndPoint.y) / lineSlope, ymin);

		} else if (tempEndPoint.y > ymax) {

			//else we want to check if y is greater than the ymax, it means our line should have hit the top of the window and stopped instead of the right edge
			//in this case, we need to scale the x back to the ymax
			tempEndPoint = new Vector2 (tempEndPoint.x + (ymax - tempEndPoint.y) / lineSlope, ymin);

		}

		//now, we can set the phasorLineEndPoints to the tempEndPoint
		torpedoDestinationPosition = tempEndPoint;

	}

	//this function will catch flare event results and set the appropriate flags so that the 
	//subsequent torpedo events will know whether flares were involved or not
	private void CatchFlareResults(int qtyFlares, bool flaresStopTorpedo){

		//set the flares used flag to true
		flaresUsed = true;

		//set the number of flares used
		numberFlaresUsed = qtyFlares;

		//set the flare result flag
		flaresSuccessful = flaresStopTorpedo;

	}

	//this function sets the flare cluster point that the flares will go around
	private void SetFlareClusterPoint(Vector2 torpedoStart, Vector2 torpedoFinish){

		//use a temporary variable to determine how far between the start and finish the flare cluster should be
		//range will be from 0 to 1
		float flareClusterPercentToFinish = .65f;

		flareClusterPoint = new Vector2 (torpedoStart.x + (torpedoFinish.x - torpedoStart.x) * flareClusterPercentToFinish,
			torpedoStart.y + (torpedoFinish.y - torpedoStart.y) * flareClusterPercentToFinish);

	}

	//this function sets flare positions based on number of flares and ship section targeted
	private void SetFlarePositions(int qtyFlares, Vector2 clusterPoint){

		//set the array size to be the number of flares used
		flareDestinationPosition = new Vector2[qtyFlares];

		//temp variable to control how far away flares can be from the cluster point
		int flareClusterRadius = 20;

		//loop through each element in the array
		for (int i = 0; i < flareDestinationPosition.Length; i++) {

			//temp x
			float tempX = Random.Range((float)-flareClusterRadius,(float)flareClusterRadius);

			//temp y
			float tempY = Random.Range((float)-flareClusterRadius,(float)flareClusterRadius);

			//set the destination position
			flareDestinationPosition[i] = new Vector2(clusterPoint.x + tempX, clusterPoint.y + tempY);

		}

	}

	//this function sets the flare spawn position
	private void SetFlareSpawnPoint(CombatUnit targetedUnit){

		//where the flare originates from depends on the kind of unit
		switch (targetedUnit.unitType) {

		case CombatUnit.UnitType.Starbase:

			//the start point adjusts for the targetedUnitParent, then the section sprite, then finally an offset
			flareSpawnPoint = new Vector2 (
				targetedUnitParent.transform.localPosition.x + targetedUnitParent.transform.localScale.x * (targetedUnitSections [(int)CombatManager.BaseSectionTargeted.StorageSection2].transform.localPosition.x + starbaseFlareSpawnOffset.x),
				targetedUnitParent.transform.localPosition.y + targetedUnitParent.transform.localScale.y * (targetedUnitSections [(int)CombatManager.BaseSectionTargeted.StorageSection2].transform.localPosition.y + starbaseFlareSpawnOffset.y));

			break;

		case CombatUnit.UnitType.Starship:

			//the start point adjusts for the targetedUnitParent, then the section sprite, then finally an offset
			flareSpawnPoint = new Vector2 (
				targetedUnitParent.transform.localPosition.x + targetedUnitParent.transform.localScale.x * (targetedUnitSections [(int)CombatManager.ShipSectionTargeted.StorageSection].transform.localPosition.x + shipFlareSpawnOffset.x),
				targetedUnitParent.transform.localPosition.y + targetedUnitParent.transform.localScale.y * (targetedUnitSections [(int)CombatManager.ShipSectionTargeted.StorageSection].transform.localPosition.y + shipFlareSpawnOffset.y));

			break;

		case CombatUnit.UnitType.Destroyer:

			//the start point adjusts for the targetedUnitParent, then the section sprite, then finally an offset
			flareSpawnPoint = new Vector2 (
				targetedUnitParent.transform.localPosition.x + targetedUnitParent.transform.localScale.x * (targetedUnitSections [(int)CombatManager.ShipSectionTargeted.StorageSection].transform.localPosition.x + shipFlareSpawnOffset.x),
				targetedUnitParent.transform.localPosition.y + targetedUnitParent.transform.localScale.y * (targetedUnitSections [(int)CombatManager.ShipSectionTargeted.StorageSection].transform.localPosition.y + shipFlareSpawnOffset.y));

			break;

		case CombatUnit.UnitType.BirdOfPrey:

			//the start point adjusts for the targetedUnitParent, then the section sprite, then finally an offset
			flareSpawnPoint = new Vector2 (
				targetedUnitParent.transform.localPosition.x + targetedUnitParent.transform.localScale.x * (targetedUnitSections [(int)CombatManager.ShipSectionTargeted.StorageSection].transform.localPosition.x + shipFlareSpawnOffset.x),
				targetedUnitParent.transform.localPosition.y + targetedUnitParent.transform.localScale.y * (targetedUnitSections [(int)CombatManager.ShipSectionTargeted.StorageSection].transform.localPosition.y + shipFlareSpawnOffset.y));

			break;

		case CombatUnit.UnitType.Scout:

			//the start point adjusts for the targetedUnitParent, then the section sprite, then finally an offset
			flareSpawnPoint = new Vector2 (
				targetedUnitParent.transform.localPosition.x + targetedUnitParent.transform.localScale.x * (targetedUnitSections [(int)CombatManager.ShipSectionTargeted.StorageSection].transform.localPosition.x + shipFlareSpawnOffset.x),
				targetedUnitParent.transform.localPosition.y + targetedUnitParent.transform.localScale.y * (targetedUnitSections [(int)CombatManager.ShipSectionTargeted.StorageSection].transform.localPosition.y + shipFlareSpawnOffset.y));

			break;

		default:

			break;

		}

	}

	//this function sets the positions for the explosions for when a section gets destroyed
	private void SetSectionDestroyedExplosionPoints(CombatUnit targetedUnit, CombatManager.ShipSectionTargeted sectionTargeted){

		//variables to control the number of explosions
		int offsetsPerSection = 5;
		int numberOfPointsPerOffset = 2;
		int randomAmplitude = 5;

		//initialize the sectionDestroyed array size
		sectionDestroyedExplosions = new Vector2[offsetsPerSection * numberOfPointsPerOffset];

		//do a switch case on the targeted unit type
		switch (targetedUnit.unitType) {

		case CombatUnit.UnitType.Starbase:

			//if we have a starbase, we want to cast the shipSectionTargeted to a baseSectionTargeted, since both are 0-5 enums
			CombatManager.BaseSectionTargeted baseSectionTargeted = (CombatManager.BaseSectionTargeted)sectionTargeted;

			//do a switch case on the section targeted
			switch (baseSectionTargeted) {

			case CombatManager.BaseSectionTargeted.PhasorSection1:

				//loop through the sectionDestroyedExplosions points
				for (int i = 0; i < sectionDestroyedExplosions.Length; i++) {

					sectionDestroyedExplosions[i] = new Vector2 (
						targetedUnitParent.transform.localPosition.x + 
						targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)baseSectionTargeted].transform.localPosition.x
						+ basePhasorSection1TargetedOffset.x + basePhasorSection1TargetedOffsetSpacing.x * (i / numberOfPointsPerOffset) + 
						Random.Range(-randomAmplitude, randomAmplitude+1),

						targetedUnitParent.transform.localPosition.y + 
						targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)baseSectionTargeted].transform.localPosition.y
						+ basePhasorSection1TargetedOffset.y + basePhasorSection1TargetedOffsetSpacing.y *  (i / numberOfPointsPerOffset) + 
						Random.Range(-randomAmplitude, randomAmplitude+1)); 

				}

				break;

			case CombatManager.BaseSectionTargeted.PhasorSection2:

				//loop through the sectionDestroyedExplosions points
				for (int i = 0; i < sectionDestroyedExplosions.Length; i++) {

					sectionDestroyedExplosions[i] = new Vector2 (
						targetedUnitParent.transform.localPosition.x + 
						targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)baseSectionTargeted].transform.localPosition.x
						+ basePhasorSection2TargetedOffset.x + basePhasorSection2TargetedOffsetSpacing.x * (i / numberOfPointsPerOffset) + 
						Random.Range(-randomAmplitude, randomAmplitude+1),

						targetedUnitParent.transform.localPosition.y + 
						targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)baseSectionTargeted].transform.localPosition.y
						+ basePhasorSection2TargetedOffset.y + basePhasorSection2TargetedOffsetSpacing.y *  (i / numberOfPointsPerOffset) + 
						Random.Range(-randomAmplitude, randomAmplitude+1)); 

				}

				break;

			case CombatManager.BaseSectionTargeted.TorpedoSection:

				//loop through the sectionDestroyedExplosions points
				for (int i = 0; i < sectionDestroyedExplosions.Length; i++) {

					sectionDestroyedExplosions[i] = new Vector2 (
						targetedUnitParent.transform.localPosition.x + 
						targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)baseSectionTargeted].transform.localPosition.x
						+ baseTorpedoSectionTargetedOffset.x + baseTorpedoSectionTargetedOffsetSpacing.x * (i / numberOfPointsPerOffset) + 
						Random.Range(-randomAmplitude, randomAmplitude+1),

						targetedUnitParent.transform.localPosition.y + 
						targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)baseSectionTargeted].transform.localPosition.y
						+ baseTorpedoSectionTargetedOffset.y + baseTorpedoSectionTargetedOffsetSpacing.y *  (i / numberOfPointsPerOffset) + 
						Random.Range(-randomAmplitude, randomAmplitude+1)); 

				}

				break;

			case CombatManager.BaseSectionTargeted.CrewSection:

				//loop through the sectionDestroyedExplosions points
				for (int i = 0; i < sectionDestroyedExplosions.Length; i++) {

					sectionDestroyedExplosions[i] = new Vector2 (
						targetedUnitParent.transform.localPosition.x + 
						targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)baseSectionTargeted].transform.localPosition.x
						+ baseCrewSectionTargetedOffset.x + baseCrewSectionTargetedOffsetSpacing.x * (i / numberOfPointsPerOffset) + 
						Random.Range(-randomAmplitude, randomAmplitude+1),

						targetedUnitParent.transform.localPosition.y + 
						targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)baseSectionTargeted].transform.localPosition.y
						+ baseCrewSectionTargetedOffset.y + baseCrewSectionTargetedOffsetSpacing.y *  (i / numberOfPointsPerOffset) + 
						Random.Range(-randomAmplitude, randomAmplitude+1)); 

				}

				break;

			case CombatManager.BaseSectionTargeted.StorageSection1:

				//loop through the sectionDestroyedExplosions points
				for (int i = 0; i < sectionDestroyedExplosions.Length; i++) {

					sectionDestroyedExplosions[i] = new Vector2 (
						targetedUnitParent.transform.localPosition.x + 
						targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)baseSectionTargeted].transform.localPosition.x
						+ baseStorageSection1TargetedOffset.x + baseStorageSection1TargetedOffsetSpacing.x * (i / numberOfPointsPerOffset) + 
						Random.Range(-randomAmplitude, randomAmplitude+1),

						targetedUnitParent.transform.localPosition.y + 
						targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)baseSectionTargeted].transform.localPosition.y
						+ baseStorageSection1TargetedOffset.y + baseStorageSection1TargetedOffsetSpacing.y *  (i / numberOfPointsPerOffset) + 
						Random.Range(-randomAmplitude, randomAmplitude+1)); 

				}

				break;

			case CombatManager.BaseSectionTargeted.StorageSection2:

				//loop through the sectionDestroyedExplosions points
				for (int i = 0; i < sectionDestroyedExplosions.Length; i++) {

					sectionDestroyedExplosions[i] = new Vector2 (
						targetedUnitParent.transform.localPosition.x + 
						targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)baseSectionTargeted].transform.localPosition.x
						+ baseStorageSection2TargetedOffset.x + baseStorageSection2TargetedOffsetSpacing.x * (i / numberOfPointsPerOffset) + 
						Random.Range(-randomAmplitude, randomAmplitude+1),

						targetedUnitParent.transform.localPosition.y + 
						targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)baseSectionTargeted].transform.localPosition.y
						+ baseStorageSection2TargetedOffset.y + baseStorageSection2TargetedOffsetSpacing.y *  (i / numberOfPointsPerOffset) + 
						Random.Range(-randomAmplitude, randomAmplitude+1)); 

				}

				break;

			default:

				break;

			}

			break;

		case CombatUnit.UnitType.Starship:

			//do a switch case on the section targeted
			switch (sectionTargeted) {

			case CombatManager.ShipSectionTargeted.PhasorSection:

				//loop through the sectionDestroyedExplosions points
				for (int i = 0; i < sectionDestroyedExplosions.Length; i++) {

					sectionDestroyedExplosions[i] = new Vector2 (
						targetedUnitParent.transform.localPosition.x + 
						targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)sectionTargeted].transform.localPosition.x
						+ shipPhasorSectionTargetedOffset.x + shipPhasorSectionTargetedOffsetSpacing.x * (i / numberOfPointsPerOffset) + 
						Random.Range(-randomAmplitude, randomAmplitude+1),

						targetedUnitParent.transform.localPosition.y + 
						targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)sectionTargeted].transform.localPosition.y
						+ shipPhasorSectionTargetedOffset.y + shipPhasorSectionTargetedOffsetSpacing.y *  (i / numberOfPointsPerOffset) + 
						Random.Range(-randomAmplitude, randomAmplitude+1)); 

				}

				break;

			case CombatManager.ShipSectionTargeted.TorpedoSection:

				//loop through the sectionDestroyedExplosions points
				for (int i = 0; i < sectionDestroyedExplosions.Length; i++) {

					sectionDestroyedExplosions[i] = new Vector2 (
						targetedUnitParent.transform.localPosition.x + 
						targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)sectionTargeted].transform.localPosition.x
						+ shipTorpedoSectionTargetedOffset.x + shipTorpedoSectionTargetedOffsetSpacing.x * (i / numberOfPointsPerOffset) + 
						Random.Range(-randomAmplitude, randomAmplitude+1),

						targetedUnitParent.transform.localPosition.y + 
						targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)sectionTargeted].transform.localPosition.y
						+ shipTorpedoSectionTargetedOffset.y + shipTorpedoSectionTargetedOffsetSpacing.y *  (i / numberOfPointsPerOffset) + 
						Random.Range(-randomAmplitude, randomAmplitude+1)); 

				}

				break;

			case CombatManager.ShipSectionTargeted.StorageSection:

				//loop through the sectionDestroyedExplosions points
				for (int i = 0; i < sectionDestroyedExplosions.Length; i++) {

					sectionDestroyedExplosions[i] = new Vector2 (
						targetedUnitParent.transform.localPosition.x + 
						targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)sectionTargeted].transform.localPosition.x
						+ shipStorageSectionTargetedOffset.x + shipStorageSectionTargetedOffsetSpacing.x * (i / numberOfPointsPerOffset) + 
						Random.Range(-randomAmplitude, randomAmplitude+1),

						targetedUnitParent.transform.localPosition.y + 
						targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)sectionTargeted].transform.localPosition.y
						+ shipStorageSectionTargetedOffset.y + shipStorageSectionTargetedOffsetSpacing.y *  (i / numberOfPointsPerOffset) + 
						Random.Range(-randomAmplitude, randomAmplitude+1)); 

				}

				break;

			case CombatManager.ShipSectionTargeted.CrewSection:

				//loop through the sectionDestroyedExplosions points
				for (int i = 0; i < sectionDestroyedExplosions.Length; i++) {

					sectionDestroyedExplosions[i] = new Vector2 (
						targetedUnitParent.transform.localPosition.x + 
						targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)sectionTargeted].transform.localPosition.x
						+ shipCrewSectionTargetedOffset.x + shipCrewSectionTargetedOffsetSpacing.x * (i / numberOfPointsPerOffset) + 
						Random.Range(-randomAmplitude, randomAmplitude+1),

						targetedUnitParent.transform.localPosition.y + 
						targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)sectionTargeted].transform.localPosition.y
						+ shipCrewSectionTargetedOffset.y + shipCrewSectionTargetedOffsetSpacing.y *  (i / numberOfPointsPerOffset) + 
						Random.Range(-randomAmplitude, randomAmplitude+1)); 

				}

				break;

			case CombatManager.ShipSectionTargeted.EngineSection:

				//loop through the sectionDestroyedExplosions points
				for (int i = 0; i < sectionDestroyedExplosions.Length; i++) {

					sectionDestroyedExplosions[i] = new Vector2 (
						targetedUnitParent.transform.localPosition.x + 
						targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)sectionTargeted].transform.localPosition.x
						+ shipEngineSectionTargetedOffset.x + shipEngineSectionTargetedOffsetSpacing.x * (i / numberOfPointsPerOffset) + 
						Random.Range(-randomAmplitude, randomAmplitude+1),

						targetedUnitParent.transform.localPosition.y + 
						targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)sectionTargeted].transform.localPosition.y
						+ shipEngineSectionTargetedOffset.y + shipEngineSectionTargetedOffsetSpacing.y *  (i / numberOfPointsPerOffset) + 
						Random.Range(-randomAmplitude, randomAmplitude+1)); 

				}

				break;

			default:

				break;

			}

			break;

		case CombatUnit.UnitType.Destroyer:

			//do a switch case on the section targeted
			switch (sectionTargeted) {

			case CombatManager.ShipSectionTargeted.PhasorSection:

				//loop through the sectionDestroyedExplosions points
				for (int i = 0; i < sectionDestroyedExplosions.Length; i++) {

					sectionDestroyedExplosions[i] = new Vector2 (
						targetedUnitParent.transform.localPosition.x + 
						targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)sectionTargeted].transform.localPosition.x
						+ shipPhasorSectionTargetedOffset.x + shipPhasorSectionTargetedOffsetSpacing.x * (i / numberOfPointsPerOffset) + 
						Random.Range(-randomAmplitude, randomAmplitude+1),

						targetedUnitParent.transform.localPosition.y + 
						targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)sectionTargeted].transform.localPosition.y
						+ shipPhasorSectionTargetedOffset.y + shipPhasorSectionTargetedOffsetSpacing.y *  (i / numberOfPointsPerOffset) + 
						Random.Range(-randomAmplitude, randomAmplitude+1)); 

				}

				break;

			case CombatManager.ShipSectionTargeted.TorpedoSection:

				//loop through the sectionDestroyedExplosions points
				for (int i = 0; i < sectionDestroyedExplosions.Length; i++) {

					sectionDestroyedExplosions[i] = new Vector2 (
						targetedUnitParent.transform.localPosition.x + 
						targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)sectionTargeted].transform.localPosition.x
						+ shipTorpedoSectionTargetedOffset.x + shipTorpedoSectionTargetedOffsetSpacing.x * (i / numberOfPointsPerOffset) + 
						Random.Range(-randomAmplitude, randomAmplitude+1),

						targetedUnitParent.transform.localPosition.y + 
						targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)sectionTargeted].transform.localPosition.y
						+ shipTorpedoSectionTargetedOffset.y + shipTorpedoSectionTargetedOffsetSpacing.y *  (i / numberOfPointsPerOffset) + 
						Random.Range(-randomAmplitude, randomAmplitude+1)); 

				}

				break;

			case CombatManager.ShipSectionTargeted.StorageSection:

				//loop through the sectionDestroyedExplosions points
				for (int i = 0; i < sectionDestroyedExplosions.Length; i++) {

					sectionDestroyedExplosions[i] = new Vector2 (
						targetedUnitParent.transform.localPosition.x + 
						targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)sectionTargeted].transform.localPosition.x
						+ shipStorageSectionTargetedOffset.x + shipStorageSectionTargetedOffsetSpacing.x * (i / numberOfPointsPerOffset) + 
						Random.Range(-randomAmplitude, randomAmplitude+1),

						targetedUnitParent.transform.localPosition.y + 
						targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)sectionTargeted].transform.localPosition.y
						+ shipStorageSectionTargetedOffset.y + shipStorageSectionTargetedOffsetSpacing.y *  (i / numberOfPointsPerOffset) + 
						Random.Range(-randomAmplitude, randomAmplitude+1)); 

				}

				break;

			case CombatManager.ShipSectionTargeted.EngineSection:

				//loop through the sectionDestroyedExplosions points
				for (int i = 0; i < sectionDestroyedExplosions.Length; i++) {

					sectionDestroyedExplosions[i] = new Vector2 (
						targetedUnitParent.transform.localPosition.x + 
						targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)sectionTargeted].transform.localPosition.x
						+ shipEngineSectionTargetedOffset.x + shipEngineSectionTargetedOffsetSpacing.x * (i / numberOfPointsPerOffset) + 
						Random.Range(-randomAmplitude, randomAmplitude+1),

						targetedUnitParent.transform.localPosition.y + 
						targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)sectionTargeted].transform.localPosition.y
						+ shipEngineSectionTargetedOffset.y + shipEngineSectionTargetedOffsetSpacing.y *  (i / numberOfPointsPerOffset) + 
						Random.Range(-randomAmplitude, randomAmplitude+1)); 

				}

				break;

			default:

				break;

			}

			break;

		case CombatUnit.UnitType.BirdOfPrey:

			//do a switch case on the section targeted
			switch (sectionTargeted) {

			case CombatManager.ShipSectionTargeted.PhasorSection:

				//loop through the sectionDestroyedExplosions points
				for (int i = 0; i < sectionDestroyedExplosions.Length; i++) {

					sectionDestroyedExplosions[i] = new Vector2 (
						targetedUnitParent.transform.localPosition.x + 
						targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)sectionTargeted].transform.localPosition.x
						+ shipPhasorSectionTargetedOffset.x + shipPhasorSectionTargetedOffsetSpacing.x * (i / numberOfPointsPerOffset) + 
						Random.Range(-randomAmplitude, randomAmplitude+1),

						targetedUnitParent.transform.localPosition.y + 
						targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)sectionTargeted].transform.localPosition.y
						+ shipPhasorSectionTargetedOffset.y + shipPhasorSectionTargetedOffsetSpacing.y *  (i / numberOfPointsPerOffset) + 
						Random.Range(-randomAmplitude, randomAmplitude+1)); 

				}

				break;

			case CombatManager.ShipSectionTargeted.TorpedoSection:

				//loop through the sectionDestroyedExplosions points
				for (int i = 0; i < sectionDestroyedExplosions.Length; i++) {

					sectionDestroyedExplosions[i] = new Vector2 (
						targetedUnitParent.transform.localPosition.x + 
						targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)sectionTargeted].transform.localPosition.x
						+ shipTorpedoSectionTargetedOffset.x + shipTorpedoSectionTargetedOffsetSpacing.x * (i / numberOfPointsPerOffset) + 
						Random.Range(-randomAmplitude, randomAmplitude+1),

						targetedUnitParent.transform.localPosition.y + 
						targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)sectionTargeted].transform.localPosition.y
						+ shipTorpedoSectionTargetedOffset.y + shipTorpedoSectionTargetedOffsetSpacing.y *  (i / numberOfPointsPerOffset) + 
						Random.Range(-randomAmplitude, randomAmplitude+1)); 

				}

				break;

			case CombatManager.ShipSectionTargeted.EngineSection:

				//loop through the sectionDestroyedExplosions points
				for (int i = 0; i < sectionDestroyedExplosions.Length; i++) {

					sectionDestroyedExplosions[i] = new Vector2 (
						targetedUnitParent.transform.localPosition.x + 
						targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)sectionTargeted].transform.localPosition.x
						+ shipEngineSectionTargetedOffset.x + shipEngineSectionTargetedOffsetSpacing.x * (i / numberOfPointsPerOffset) + 
						Random.Range(-randomAmplitude, randomAmplitude+1),

						targetedUnitParent.transform.localPosition.y + 
						targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)sectionTargeted].transform.localPosition.y
						+ shipEngineSectionTargetedOffset.y + shipEngineSectionTargetedOffsetSpacing.y *  (i / numberOfPointsPerOffset) + 
						Random.Range(-randomAmplitude, randomAmplitude+1)); 

				}

				break;

			default:

				break;

			}

			break;

		case CombatUnit.UnitType.Scout:

			//do a switch case on the section targeted
			switch (sectionTargeted) {

			case CombatManager.ShipSectionTargeted.PhasorSection:

				//loop through the sectionDestroyedExplosions points
				for (int i = 0; i < sectionDestroyedExplosions.Length; i++) {

					sectionDestroyedExplosions[i] = new Vector2 (
						targetedUnitParent.transform.localPosition.x + 
						targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)sectionTargeted].transform.localPosition.x
						+ shipPhasorSectionTargetedOffset.x + shipPhasorSectionTargetedOffsetSpacing.x * (i / numberOfPointsPerOffset) + 
						Random.Range(-randomAmplitude, randomAmplitude+1),

						targetedUnitParent.transform.localPosition.y + 
						targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)sectionTargeted].transform.localPosition.y
						+ shipPhasorSectionTargetedOffset.y + shipPhasorSectionTargetedOffsetSpacing.y *  (i / numberOfPointsPerOffset) + 
						Random.Range(-randomAmplitude, randomAmplitude+1)); 

				}

				break;

			case CombatManager.ShipSectionTargeted.StorageSection:

				//loop through the sectionDestroyedExplosions points
				for (int i = 0; i < sectionDestroyedExplosions.Length; i++) {

					sectionDestroyedExplosions[i] = new Vector2 (
						targetedUnitParent.transform.localPosition.x + 
						targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)sectionTargeted].transform.localPosition.x
						+ shipStorageSectionTargetedOffset.x + shipStorageSectionTargetedOffsetSpacing.x * (i / numberOfPointsPerOffset) + 
						Random.Range(-randomAmplitude, randomAmplitude+1),

						targetedUnitParent.transform.localPosition.y + 
						targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)sectionTargeted].transform.localPosition.y
						+ shipStorageSectionTargetedOffset.y + shipStorageSectionTargetedOffsetSpacing.y *  (i / numberOfPointsPerOffset) + 
						Random.Range(-randomAmplitude, randomAmplitude+1)); 

				}

				break;

			case CombatManager.ShipSectionTargeted.EngineSection:

				//loop through the sectionDestroyedExplosions points
				for (int i = 0; i < sectionDestroyedExplosions.Length; i++) {

					sectionDestroyedExplosions[i] = new Vector2 (
						targetedUnitParent.transform.localPosition.x + 
						targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)sectionTargeted].transform.localPosition.x
						+ shipEngineSectionTargetedOffset.x + shipEngineSectionTargetedOffsetSpacing.x * (i / numberOfPointsPerOffset) + 
						Random.Range(-randomAmplitude, randomAmplitude+1),

						targetedUnitParent.transform.localPosition.y + 
						targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)sectionTargeted].transform.localPosition.y
						+ shipEngineSectionTargetedOffset.y + shipEngineSectionTargetedOffsetSpacing.y *  (i / numberOfPointsPerOffset) + 
						Random.Range(-randomAmplitude, randomAmplitude+1)); 

				}

				break;

			default:

				break;

			}

			break;

		default:

			break;

		}

	}

	//this function replaces an alive section graphic with a destroyed one
	private void InsertDestroyedSectionGraphic(CombatUnit targetedUnit, CombatManager.ShipSectionTargeted sectionTargeted){

		//run a switch case on the attacking unit to determine sprite positions and scale
		switch (targetedUnit.unitType) {

		case CombatUnit.UnitType.Starbase:

			//for a base, we need to convert the sectionTargeted to a base section targeted
			CombatManager.BaseSectionTargeted baseSectionTargeted = (CombatManager.BaseSectionTargeted)sectionTargeted;

			//to set the images, we need to know the owner color as well
			switch (targetedUnit.owner.color) {

			case Player.Color.Green:

				//now we need to know what section we are supposed to destroy
				switch (baseSectionTargeted) {

				case CombatManager.BaseSectionTargeted.PhasorSection1:

					targetedUnitSections [(int)baseSectionTargeted].GetComponent<Image> ().sprite = greenBasePhasorSectionDestroyed;
					break;

				case CombatManager.BaseSectionTargeted.PhasorSection2:

					targetedUnitSections [(int)baseSectionTargeted].GetComponent<Image> ().sprite = greenBasePhasorSectionDestroyed;
					break;

				case CombatManager.BaseSectionTargeted.TorpedoSection:

					targetedUnitSections [(int)baseSectionTargeted].GetComponent<Image> ().sprite = greenBaseMiddleSectionDestroyed;
					break;

				case CombatManager.BaseSectionTargeted.CrewSection:

					targetedUnitSections [(int)baseSectionTargeted].GetComponent<Image> ().sprite = greenBaseMiddleSectionDestroyed;
					break;

				case CombatManager.BaseSectionTargeted.StorageSection1:

					targetedUnitSections [(int)baseSectionTargeted].GetComponent<Image> ().sprite = greenBaseStorageSectionDestroyed;
					break;

				case CombatManager.BaseSectionTargeted.StorageSection2:

					targetedUnitSections [(int)baseSectionTargeted].GetComponent<Image> ().sprite = greenBaseStorageSectionDestroyed;
					break;

				default:

					break;

				}

				break;

			case Player.Color.Purple:

				//now we need to know what section we are supposed to destroy
				switch (baseSectionTargeted) {

				case CombatManager.BaseSectionTargeted.PhasorSection1:

					targetedUnitSections [(int)baseSectionTargeted].GetComponent<Image> ().sprite = purpleBasePhasorSectionDestroyed;
					break;

				case CombatManager.BaseSectionTargeted.PhasorSection2:

					targetedUnitSections [(int)baseSectionTargeted].GetComponent<Image> ().sprite = purpleBasePhasorSectionDestroyed;
					break;

				case CombatManager.BaseSectionTargeted.TorpedoSection:

					targetedUnitSections [(int)baseSectionTargeted].GetComponent<Image> ().sprite = purpleBaseMiddleSectionDestroyed;
					break;

				case CombatManager.BaseSectionTargeted.CrewSection:

					targetedUnitSections [(int)baseSectionTargeted].GetComponent<Image> ().sprite = purpleBaseMiddleSectionDestroyed;
					break;

				case CombatManager.BaseSectionTargeted.StorageSection1:

					targetedUnitSections [(int)baseSectionTargeted].GetComponent<Image> ().sprite = purpleBaseStorageSectionDestroyed;
					break;

				case CombatManager.BaseSectionTargeted.StorageSection2:

					targetedUnitSections [(int)baseSectionTargeted].GetComponent<Image> ().sprite = purpleBaseStorageSectionDestroyed;
					break;

				default:

					break;

				}

				break;

			case Player.Color.Red:

				//now we need to know what section we are supposed to destroy
				switch (baseSectionTargeted) {

				case CombatManager.BaseSectionTargeted.PhasorSection1:

					targetedUnitSections [(int)baseSectionTargeted].GetComponent<Image> ().sprite = redBasePhasorSectionDestroyed;
					break;

				case CombatManager.BaseSectionTargeted.PhasorSection2:

					targetedUnitSections [(int)baseSectionTargeted].GetComponent<Image> ().sprite = redBasePhasorSectionDestroyed;
					break;

				case CombatManager.BaseSectionTargeted.TorpedoSection:

					targetedUnitSections [(int)baseSectionTargeted].GetComponent<Image> ().sprite = redBaseMiddleSectionDestroyed;
					break;

				case CombatManager.BaseSectionTargeted.CrewSection:

					targetedUnitSections [(int)baseSectionTargeted].GetComponent<Image> ().sprite = redBaseMiddleSectionDestroyed;
					break;

				case CombatManager.BaseSectionTargeted.StorageSection1:

					targetedUnitSections [(int)baseSectionTargeted].GetComponent<Image> ().sprite = redBaseStorageSectionDestroyed;
					break;

				case CombatManager.BaseSectionTargeted.StorageSection2:

					targetedUnitSections [(int)baseSectionTargeted].GetComponent<Image> ().sprite = redBaseStorageSectionDestroyed;
					break;

				default:

					break;

				}

				break;

			case Player.Color.Blue:

				//now we need to know what section we are supposed to destroy
				switch (baseSectionTargeted) {

				case CombatManager.BaseSectionTargeted.PhasorSection1:

					targetedUnitSections [(int)baseSectionTargeted].GetComponent<Image> ().sprite = blueBasePhasorSectionDestroyed;
					break;

				case CombatManager.BaseSectionTargeted.PhasorSection2:

					targetedUnitSections [(int)baseSectionTargeted].GetComponent<Image> ().sprite = blueBasePhasorSectionDestroyed;
					break;

				case CombatManager.BaseSectionTargeted.TorpedoSection:

					targetedUnitSections [(int)baseSectionTargeted].GetComponent<Image> ().sprite = blueBaseMiddleSectionDestroyed;
					break;

				case CombatManager.BaseSectionTargeted.CrewSection:

					targetedUnitSections [(int)baseSectionTargeted].GetComponent<Image> ().sprite = blueBaseMiddleSectionDestroyed;
					break;

				case CombatManager.BaseSectionTargeted.StorageSection1:

					targetedUnitSections [(int)baseSectionTargeted].GetComponent<Image> ().sprite = blueBaseStorageSectionDestroyed;
					break;

				case CombatManager.BaseSectionTargeted.StorageSection2:

					targetedUnitSections [(int)baseSectionTargeted].GetComponent<Image> ().sprite = blueBaseStorageSectionDestroyed;
					break;

				default:

					break;

				}

				break;

			default:

				break;

			}

			break;

		case CombatUnit.UnitType.Starship:

			//to set the images, we need to know the owner color as well
			switch (targetedUnit.owner.color) {

			case Player.Color.Green:

				//now we need to know what section we are supposed to destroy
				switch (sectionTargeted) {

				case CombatManager.ShipSectionTargeted.PhasorSection:

					targetedUnitSections [(int)sectionTargeted].GetComponent<Image> ().sprite = greenPhasorSectionDestroyed;
					break;

				case CombatManager.ShipSectionTargeted.TorpedoSection:

					targetedUnitSections [(int)sectionTargeted].GetComponent<Image> ().sprite = greenTorpedoSectionDestroyed;
					break;

				case CombatManager.ShipSectionTargeted.StorageSection:

					targetedUnitSections [(int)sectionTargeted].GetComponent<Image> ().sprite = greenStorageSectionDestroyed;
					break;

				case CombatManager.ShipSectionTargeted.CrewSection:

					targetedUnitSections [(int)sectionTargeted].GetComponent<Image> ().sprite = greenCrewSectionDestroyed;
					break;

				case CombatManager.ShipSectionTargeted.EngineSection:

					targetedUnitSections [(int)sectionTargeted].GetComponent<Image> ().sprite = greenEngineSectionDestroyed;
					break;

				default:

					break;

				}

				break;

			case Player.Color.Purple:

				//now we need to know what section we are supposed to destroy
				switch (sectionTargeted) {

				case CombatManager.ShipSectionTargeted.PhasorSection:

					targetedUnitSections [(int)sectionTargeted].GetComponent<Image> ().sprite = purplePhasorSectionDestroyed;
					break;

				case CombatManager.ShipSectionTargeted.TorpedoSection:

					targetedUnitSections [(int)sectionTargeted].GetComponent<Image> ().sprite = purpleTorpedoSectionDestroyed;
					break;

				case CombatManager.ShipSectionTargeted.StorageSection:

					targetedUnitSections [(int)sectionTargeted].GetComponent<Image> ().sprite = purpleStorageSectionDestroyed;
					break;

				case CombatManager.ShipSectionTargeted.CrewSection:

					targetedUnitSections [(int)sectionTargeted].GetComponent<Image> ().sprite = purpleCrewSectionDestroyed;
					break;

				case CombatManager.ShipSectionTargeted.EngineSection:

					targetedUnitSections [(int)sectionTargeted].GetComponent<Image> ().sprite = purpleEngineSectionDestroyed;
					break;

				default:

					break;

				}

				break;

			case Player.Color.Red:

				//now we need to know what section we are supposed to destroy
				switch (sectionTargeted) {

				case CombatManager.ShipSectionTargeted.PhasorSection:

					targetedUnitSections [(int)sectionTargeted].GetComponent<Image> ().sprite = redPhasorSectionDestroyed;
					break;

				case CombatManager.ShipSectionTargeted.TorpedoSection:

					targetedUnitSections [(int)sectionTargeted].GetComponent<Image> ().sprite = redTorpedoSectionDestroyed;
					break;

				case CombatManager.ShipSectionTargeted.StorageSection:

					targetedUnitSections [(int)sectionTargeted].GetComponent<Image> ().sprite = redStorageSectionDestroyed;
					break;

				case CombatManager.ShipSectionTargeted.CrewSection:

					targetedUnitSections [(int)sectionTargeted].GetComponent<Image> ().sprite = redCrewSectionDestroyed;
					break;

				case CombatManager.ShipSectionTargeted.EngineSection:

					targetedUnitSections [(int)sectionTargeted].GetComponent<Image> ().sprite = redEngineSectionDestroyed;
					break;

				default:

					break;

				}

				break;

			case Player.Color.Blue:

				//now we need to know what section we are supposed to destroy
				switch (sectionTargeted) {

				case CombatManager.ShipSectionTargeted.PhasorSection:

					targetedUnitSections [(int)sectionTargeted].GetComponent<Image> ().sprite = bluePhasorSectionDestroyed;
					break;

				case CombatManager.ShipSectionTargeted.TorpedoSection:

					targetedUnitSections [(int)sectionTargeted].GetComponent<Image> ().sprite = blueTorpedoSectionDestroyed;
					break;

				case CombatManager.ShipSectionTargeted.StorageSection:

					targetedUnitSections [(int)sectionTargeted].GetComponent<Image> ().sprite = blueStorageSectionDestroyed;
					break;

				case CombatManager.ShipSectionTargeted.CrewSection:

					targetedUnitSections [(int)sectionTargeted].GetComponent<Image> ().sprite = blueCrewSectionDestroyed;
					break;

				case CombatManager.ShipSectionTargeted.EngineSection:

					targetedUnitSections [(int)sectionTargeted].GetComponent<Image> ().sprite = blueEngineSectionDestroyed;
					break;

				default:

					break;

				}

				break;

			default:

				break;

			}

			break;

		case CombatUnit.UnitType.Destroyer:

			//to set the images, we need to know the owner color as well
			switch (targetedUnit.owner.color) {

			case Player.Color.Green:

				//now we need to know what section we are supposed to destroy
				switch (sectionTargeted) {

				case CombatManager.ShipSectionTargeted.PhasorSection:

					targetedUnitSections [(int)sectionTargeted].GetComponent<Image> ().sprite = greenPhasorSectionDestroyed;
					break;

				case CombatManager.ShipSectionTargeted.TorpedoSection:

					targetedUnitSections [(int)sectionTargeted].GetComponent<Image> ().sprite = greenTorpedoSectionDestroyed;
					break;

				case CombatManager.ShipSectionTargeted.StorageSection:

					targetedUnitSections [(int)sectionTargeted].GetComponent<Image> ().sprite = greenStorageSectionDestroyed;
					break;

				case CombatManager.ShipSectionTargeted.EngineSection:

					targetedUnitSections [(int)sectionTargeted].GetComponent<Image> ().sprite = greenEngineSectionDestroyed;
					break;

				default:

					break;

				}

				break;

			case Player.Color.Purple:

				//now we need to know what section we are supposed to destroy
				switch (sectionTargeted) {

				case CombatManager.ShipSectionTargeted.PhasorSection:

					targetedUnitSections [(int)sectionTargeted].GetComponent<Image> ().sprite = purplePhasorSectionDestroyed;
					break;

				case CombatManager.ShipSectionTargeted.TorpedoSection:

					targetedUnitSections [(int)sectionTargeted].GetComponent<Image> ().sprite = purpleTorpedoSectionDestroyed;
					break;

				case CombatManager.ShipSectionTargeted.StorageSection:

					targetedUnitSections [(int)sectionTargeted].GetComponent<Image> ().sprite = purpleStorageSectionDestroyed;
					break;

				case CombatManager.ShipSectionTargeted.EngineSection:

					targetedUnitSections [(int)sectionTargeted].GetComponent<Image> ().sprite = purpleEngineSectionDestroyed;
					break;

				default:

					break;

				}

				break;

			case Player.Color.Red:

				//now we need to know what section we are supposed to destroy
				switch (sectionTargeted) {

				case CombatManager.ShipSectionTargeted.PhasorSection:

					targetedUnitSections [(int)sectionTargeted].GetComponent<Image> ().sprite = redPhasorSectionDestroyed;
					break;

				case CombatManager.ShipSectionTargeted.TorpedoSection:

					targetedUnitSections [(int)sectionTargeted].GetComponent<Image> ().sprite = redTorpedoSectionDestroyed;
					break;

				case CombatManager.ShipSectionTargeted.StorageSection:

					targetedUnitSections [(int)sectionTargeted].GetComponent<Image> ().sprite = redStorageSectionDestroyed;
					break;

				case CombatManager.ShipSectionTargeted.EngineSection:

					targetedUnitSections [(int)sectionTargeted].GetComponent<Image> ().sprite = redEngineSectionDestroyed;
					break;

				default:

					break;

				}

				break;

			case Player.Color.Blue:

				//now we need to know what section we are supposed to destroy
				switch (sectionTargeted) {

				case CombatManager.ShipSectionTargeted.PhasorSection:

					targetedUnitSections [(int)sectionTargeted].GetComponent<Image> ().sprite = bluePhasorSectionDestroyed;
					break;

				case CombatManager.ShipSectionTargeted.TorpedoSection:

					targetedUnitSections [(int)sectionTargeted].GetComponent<Image> ().sprite = blueTorpedoSectionDestroyed;
					break;

				case CombatManager.ShipSectionTargeted.StorageSection:

					targetedUnitSections [(int)sectionTargeted].GetComponent<Image> ().sprite = blueStorageSectionDestroyed;
					break;

				case CombatManager.ShipSectionTargeted.EngineSection:

					targetedUnitSections [(int)sectionTargeted].GetComponent<Image> ().sprite = blueEngineSectionDestroyed;
					break;

				default:

					break;

				}

				break;

			default:

				break;

			}

			break;

		case CombatUnit.UnitType.BirdOfPrey:

			//to set the images, we need to know the owner color as well
			switch (targetedUnit.owner.color) {

			case Player.Color.Green:

				//now we need to know what section we are supposed to destroy
				switch (sectionTargeted) {

				case CombatManager.ShipSectionTargeted.PhasorSection:

					targetedUnitSections [(int)sectionTargeted].GetComponent<Image> ().sprite = greenPhasorSectionDestroyed;
					break;

				case CombatManager.ShipSectionTargeted.TorpedoSection:

					targetedUnitSections [(int)sectionTargeted].GetComponent<Image> ().sprite = greenTorpedoSectionDestroyed;
					break;

				case CombatManager.ShipSectionTargeted.EngineSection:

					targetedUnitSections [(int)sectionTargeted].GetComponent<Image> ().sprite = greenEngineSectionDestroyed;
					break;

				default:

					break;

				}

				break;

			case Player.Color.Purple:

				//now we need to know what section we are supposed to destroy
				switch (sectionTargeted) {

				case CombatManager.ShipSectionTargeted.PhasorSection:

					targetedUnitSections [(int)sectionTargeted].GetComponent<Image> ().sprite = purplePhasorSectionDestroyed;
					break;

				case CombatManager.ShipSectionTargeted.TorpedoSection:

					targetedUnitSections [(int)sectionTargeted].GetComponent<Image> ().sprite = purpleTorpedoSectionDestroyed;
					break;

				case CombatManager.ShipSectionTargeted.EngineSection:

					targetedUnitSections [(int)sectionTargeted].GetComponent<Image> ().sprite = purpleEngineSectionDestroyed;
					break;

				default:

					break;

				}

				break;

			case Player.Color.Red:

				//now we need to know what section we are supposed to destroy
				switch (sectionTargeted) {

				case CombatManager.ShipSectionTargeted.PhasorSection:

					targetedUnitSections [(int)sectionTargeted].GetComponent<Image> ().sprite = redPhasorSectionDestroyed;
					break;

				case CombatManager.ShipSectionTargeted.TorpedoSection:

					targetedUnitSections [(int)sectionTargeted].GetComponent<Image> ().sprite = redTorpedoSectionDestroyed;
					break;

				case CombatManager.ShipSectionTargeted.EngineSection:

					targetedUnitSections [(int)sectionTargeted].GetComponent<Image> ().sprite = redEngineSectionDestroyed;
					break;

				default:

					break;

				}

				break;

			case Player.Color.Blue:

				//now we need to know what section we are supposed to destroy
				switch (sectionTargeted) {

				case CombatManager.ShipSectionTargeted.PhasorSection:

					targetedUnitSections [(int)sectionTargeted].GetComponent<Image> ().sprite = bluePhasorSectionDestroyed;
					break;

				case CombatManager.ShipSectionTargeted.TorpedoSection:

					targetedUnitSections [(int)sectionTargeted].GetComponent<Image> ().sprite = blueTorpedoSectionDestroyed;
					break;

				case CombatManager.ShipSectionTargeted.EngineSection:

					targetedUnitSections [(int)sectionTargeted].GetComponent<Image> ().sprite = blueEngineSectionDestroyed;
					break;

				default:

					break;

				}

				break;

			default:

				break;

			}

			break;

		case CombatUnit.UnitType.Scout:

			//to set the images, we need to know the owner color as well
			switch (targetedUnit.owner.color) {

			case Player.Color.Green:

				//now we need to know what section we are supposed to destroy
				switch (sectionTargeted) {

				case CombatManager.ShipSectionTargeted.PhasorSection:

					targetedUnitSections [(int)sectionTargeted].GetComponent<Image> ().sprite = greenPhasorSectionDestroyed;
					break;

				case CombatManager.ShipSectionTargeted.StorageSection:

					targetedUnitSections [(int)sectionTargeted].GetComponent<Image> ().sprite = greenStorageSectionDestroyed;
					break;

				case CombatManager.ShipSectionTargeted.EngineSection:

					targetedUnitSections [(int)sectionTargeted].GetComponent<Image> ().sprite = greenEngineSectionDestroyed;
					break;

				default:

					break;

				}

				break;

			case Player.Color.Purple:

				//now we need to know what section we are supposed to destroy
				switch (sectionTargeted) {

				case CombatManager.ShipSectionTargeted.PhasorSection:

					targetedUnitSections [(int)sectionTargeted].GetComponent<Image> ().sprite = purplePhasorSectionDestroyed;
					break;

				case CombatManager.ShipSectionTargeted.StorageSection:

					targetedUnitSections [(int)sectionTargeted].GetComponent<Image> ().sprite = purpleStorageSectionDestroyed;
					break;

				case CombatManager.ShipSectionTargeted.EngineSection:

					targetedUnitSections [(int)sectionTargeted].GetComponent<Image> ().sprite = purpleEngineSectionDestroyed;
					break;

				default:

					break;

				}

				break;

			case Player.Color.Red:

				//now we need to know what section we are supposed to destroy
				switch (sectionTargeted) {

				case CombatManager.ShipSectionTargeted.PhasorSection:

					targetedUnitSections [(int)sectionTargeted].GetComponent<Image> ().sprite = redPhasorSectionDestroyed;
					break;

				case CombatManager.ShipSectionTargeted.StorageSection:

					targetedUnitSections [(int)sectionTargeted].GetComponent<Image> ().sprite = redStorageSectionDestroyed;
					break;

				case CombatManager.ShipSectionTargeted.EngineSection:

					targetedUnitSections [(int)sectionTargeted].GetComponent<Image> ().sprite = redEngineSectionDestroyed;
					break;

				default:

					break;

				}

				break;

			case Player.Color.Blue:

				//now we need to know what section we are supposed to destroy
				switch (sectionTargeted) {

				case CombatManager.ShipSectionTargeted.PhasorSection:

					targetedUnitSections [(int)sectionTargeted].GetComponent<Image> ().sprite = bluePhasorSectionDestroyed;
					break;

				case CombatManager.ShipSectionTargeted.StorageSection:

					targetedUnitSections [(int)sectionTargeted].GetComponent<Image> ().sprite = blueStorageSectionDestroyed;
					break;

				case CombatManager.ShipSectionTargeted.EngineSection:

					targetedUnitSections [(int)sectionTargeted].GetComponent<Image> ().sprite = blueEngineSectionDestroyed;
					break;

				default:

					break;

				}

				break;

			default:

				break;

			}

			break;

		default:

			break;

		}

	}

	//this function sets the positions for the explosions for when a unit gets destroyed
	private void SetUnitDestroyedExplosionPoints(CombatUnit targetedUnit){

		//variables to control the number of explosions
		int offsetsPerSection = 5;
		int numberOfPointsPerOffset = 2;
		int randomAmplitude = 5;

		//initialize the sectionDestroyed array size
		unitDestroyedExplosions = new List<Vector2>();

		//do a switch case on the targeted unit type
		switch (targetedUnit.unitType) {

		case CombatUnit.UnitType.Starbase:

			//if we have a starbase, we want to set points on all sections
			for (int i = 0; i < offsetsPerSection * numberOfPointsPerOffset; i++) {

				unitDestroyedExplosions.Add (new Vector2 (
					targetedUnitParent.transform.localPosition.x +
					targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)CombatManager.BaseSectionTargeted.PhasorSection1].transform.localPosition.x
					+ basePhasorSection1TargetedOffset.x + basePhasorSection1TargetedOffsetSpacing.x * (i / numberOfPointsPerOffset) +
					Random.Range (-randomAmplitude, randomAmplitude + 1),

					targetedUnitParent.transform.localPosition.y +
					targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)CombatManager.BaseSectionTargeted.PhasorSection1].transform.localPosition.y
					+ basePhasorSection1TargetedOffset.y + basePhasorSection1TargetedOffsetSpacing.y * (i / numberOfPointsPerOffset) +
					Random.Range (-randomAmplitude, randomAmplitude + 1)));


				unitDestroyedExplosions.Add (new Vector2 (
					targetedUnitParent.transform.localPosition.x +
					targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)CombatManager.BaseSectionTargeted.PhasorSection2].transform.localPosition.x
					+ basePhasorSection2TargetedOffset.x + basePhasorSection2TargetedOffsetSpacing.x * (i / numberOfPointsPerOffset) +
					Random.Range (-randomAmplitude, randomAmplitude + 1),

					targetedUnitParent.transform.localPosition.y +
					targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)CombatManager.BaseSectionTargeted.PhasorSection2].transform.localPosition.y
					+ basePhasorSection2TargetedOffset.y + basePhasorSection2TargetedOffsetSpacing.y * (i / numberOfPointsPerOffset) +
					Random.Range (-randomAmplitude, randomAmplitude + 1)));


				unitDestroyedExplosions.Add (new Vector2 (
					targetedUnitParent.transform.localPosition.x +
					targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)CombatManager.BaseSectionTargeted.TorpedoSection].transform.localPosition.x
					+ baseTorpedoSectionTargetedOffset.x + baseTorpedoSectionTargetedOffsetSpacing.x * (i / numberOfPointsPerOffset) +
					Random.Range (-randomAmplitude, randomAmplitude + 1),

					targetedUnitParent.transform.localPosition.y +
					targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)CombatManager.BaseSectionTargeted.TorpedoSection].transform.localPosition.y
					+ baseTorpedoSectionTargetedOffset.y + baseTorpedoSectionTargetedOffsetSpacing.y * (i / numberOfPointsPerOffset) +
					Random.Range (-randomAmplitude, randomAmplitude + 1)));


				unitDestroyedExplosions.Add (new Vector2 (
					targetedUnitParent.transform.localPosition.x +
					targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)CombatManager.BaseSectionTargeted.CrewSection].transform.localPosition.x
					+ baseCrewSectionTargetedOffset.x + baseCrewSectionTargetedOffsetSpacing.x * (i / numberOfPointsPerOffset) +
					Random.Range (-randomAmplitude, randomAmplitude + 1),

					targetedUnitParent.transform.localPosition.y +
					targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)CombatManager.BaseSectionTargeted.CrewSection].transform.localPosition.y
					+ baseCrewSectionTargetedOffset.y + baseCrewSectionTargetedOffsetSpacing.y * (i / numberOfPointsPerOffset) +
					Random.Range (-randomAmplitude, randomAmplitude + 1)));

				unitDestroyedExplosions.Add (new Vector2 (
					targetedUnitParent.transform.localPosition.x +
					targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)CombatManager.BaseSectionTargeted.StorageSection1].transform.localPosition.x
					+ baseStorageSection1TargetedOffset.x + baseStorageSection1TargetedOffsetSpacing.x * (i / numberOfPointsPerOffset) +
					Random.Range (-randomAmplitude, randomAmplitude + 1),

					targetedUnitParent.transform.localPosition.y +
					targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)CombatManager.BaseSectionTargeted.StorageSection1].transform.localPosition.y
					+ baseStorageSection1TargetedOffset.y + baseStorageSection1TargetedOffsetSpacing.y * (i / numberOfPointsPerOffset) +
					Random.Range (-randomAmplitude, randomAmplitude + 1)));

				unitDestroyedExplosions.Add (new Vector2 (
					targetedUnitParent.transform.localPosition.x +
					targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)CombatManager.BaseSectionTargeted.StorageSection2].transform.localPosition.x
					+ baseStorageSection2TargetedOffset.x + baseStorageSection2TargetedOffsetSpacing.x * (i / numberOfPointsPerOffset) +
					Random.Range (-randomAmplitude, randomAmplitude + 1),

					targetedUnitParent.transform.localPosition.y +
					targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)CombatManager.BaseSectionTargeted.StorageSection2].transform.localPosition.y
					+ baseStorageSection2TargetedOffset.y + baseStorageSection2TargetedOffsetSpacing.y * (i / numberOfPointsPerOffset) +
					Random.Range (-randomAmplitude, randomAmplitude + 1)));
				
			}
				
			break;

		case CombatUnit.UnitType.Starship:
			
			for (int i = 0; i < offsetsPerSection * numberOfPointsPerOffset; i++) {

				unitDestroyedExplosions.Add (new Vector2 (
					targetedUnitParent.transform.localPosition.x +
					targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)CombatManager.ShipSectionTargeted.PhasorSection].transform.localPosition.x
					+ shipPhasorSectionTargetedOffset.x + shipPhasorSectionTargetedOffsetSpacing.x * (i / numberOfPointsPerOffset) +
					Random.Range (-randomAmplitude, randomAmplitude + 1),

					targetedUnitParent.transform.localPosition.y +
					targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)CombatManager.ShipSectionTargeted.PhasorSection].transform.localPosition.y
					+ shipPhasorSectionTargetedOffset.y + shipPhasorSectionTargetedOffsetSpacing.y * (i / numberOfPointsPerOffset) +
					Random.Range (-randomAmplitude, randomAmplitude + 1)));


				unitDestroyedExplosions.Add (new Vector2 (
					targetedUnitParent.transform.localPosition.x +
					targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)CombatManager.ShipSectionTargeted.TorpedoSection].transform.localPosition.x
					+ shipTorpedoSectionTargetedOffset.x + shipTorpedoSectionTargetedOffsetSpacing.x * (i / numberOfPointsPerOffset) +
					Random.Range (-randomAmplitude, randomAmplitude + 1),

					targetedUnitParent.transform.localPosition.y +
					targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)CombatManager.ShipSectionTargeted.TorpedoSection].transform.localPosition.y
					+ shipTorpedoSectionTargetedOffset.y + shipTorpedoSectionTargetedOffsetSpacing.y * (i / numberOfPointsPerOffset) +
					Random.Range (-randomAmplitude, randomAmplitude + 1)));


				unitDestroyedExplosions.Add (new Vector2 (
					targetedUnitParent.transform.localPosition.x +
					targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)CombatManager.ShipSectionTargeted.StorageSection].transform.localPosition.x
					+ shipStorageSectionTargetedOffset.x + shipStorageSectionTargetedOffsetSpacing.x * (i / numberOfPointsPerOffset) +
					Random.Range (-randomAmplitude, randomAmplitude + 1),

					targetedUnitParent.transform.localPosition.y +
					targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)CombatManager.ShipSectionTargeted.StorageSection].transform.localPosition.y
					+ shipStorageSectionTargetedOffset.y + shipStorageSectionTargetedOffsetSpacing.y * (i / numberOfPointsPerOffset) +
					Random.Range (-randomAmplitude, randomAmplitude + 1)));


				unitDestroyedExplosions.Add (new Vector2 (
					targetedUnitParent.transform.localPosition.x +
					targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)CombatManager.ShipSectionTargeted.CrewSection].transform.localPosition.x
					+ shipCrewSectionTargetedOffset.x + shipCrewSectionTargetedOffsetSpacing.x * (i / numberOfPointsPerOffset) +
					Random.Range (-randomAmplitude, randomAmplitude + 1),

					targetedUnitParent.transform.localPosition.y +
					targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)CombatManager.ShipSectionTargeted.CrewSection].transform.localPosition.y
					+ shipCrewSectionTargetedOffset.y + shipCrewSectionTargetedOffsetSpacing.y * (i / numberOfPointsPerOffset) +
					Random.Range (-randomAmplitude, randomAmplitude + 1)));


				unitDestroyedExplosions.Add (new Vector2 (
					targetedUnitParent.transform.localPosition.x +
					targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)CombatManager.ShipSectionTargeted.EngineSection].transform.localPosition.x
					+ shipEngineSectionTargetedOffset.x + shipEngineSectionTargetedOffsetSpacing.x * (i / numberOfPointsPerOffset) +
					Random.Range (-randomAmplitude, randomAmplitude + 1),

					targetedUnitParent.transform.localPosition.y +
					targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)CombatManager.ShipSectionTargeted.EngineSection].transform.localPosition.y
					+ shipEngineSectionTargetedOffset.y + shipEngineSectionTargetedOffsetSpacing.y * (i / numberOfPointsPerOffset) +
					Random.Range (-randomAmplitude, randomAmplitude + 1)));

			}

			break;

		case CombatUnit.UnitType.Destroyer:

			for (int i = 0; i < offsetsPerSection * numberOfPointsPerOffset; i++) {

				unitDestroyedExplosions.Add (new Vector2 (
					targetedUnitParent.transform.localPosition.x +
					targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)CombatManager.ShipSectionTargeted.PhasorSection].transform.localPosition.x
					+ shipPhasorSectionTargetedOffset.x + shipPhasorSectionTargetedOffsetSpacing.x * (i / numberOfPointsPerOffset) +
					Random.Range (-randomAmplitude, randomAmplitude + 1),

					targetedUnitParent.transform.localPosition.y +
					targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)CombatManager.ShipSectionTargeted.PhasorSection].transform.localPosition.y
					+ shipPhasorSectionTargetedOffset.y + shipPhasorSectionTargetedOffsetSpacing.y * (i / numberOfPointsPerOffset) +
					Random.Range (-randomAmplitude, randomAmplitude + 1)));


				unitDestroyedExplosions.Add (new Vector2 (
					targetedUnitParent.transform.localPosition.x +
					targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)CombatManager.ShipSectionTargeted.TorpedoSection].transform.localPosition.x
					+ shipTorpedoSectionTargetedOffset.x + shipTorpedoSectionTargetedOffsetSpacing.x * (i / numberOfPointsPerOffset) +
					Random.Range (-randomAmplitude, randomAmplitude + 1),

					targetedUnitParent.transform.localPosition.y +
					targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)CombatManager.ShipSectionTargeted.TorpedoSection].transform.localPosition.y
					+ shipTorpedoSectionTargetedOffset.y + shipTorpedoSectionTargetedOffsetSpacing.y * (i / numberOfPointsPerOffset) +
					Random.Range (-randomAmplitude, randomAmplitude + 1)));


				unitDestroyedExplosions.Add (new Vector2 (
					targetedUnitParent.transform.localPosition.x +
					targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)CombatManager.ShipSectionTargeted.StorageSection].transform.localPosition.x
					+ shipStorageSectionTargetedOffset.x + shipStorageSectionTargetedOffsetSpacing.x * (i / numberOfPointsPerOffset) +
					Random.Range (-randomAmplitude, randomAmplitude + 1),

					targetedUnitParent.transform.localPosition.y +
					targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)CombatManager.ShipSectionTargeted.StorageSection].transform.localPosition.y
					+ shipStorageSectionTargetedOffset.y + shipStorageSectionTargetedOffsetSpacing.y * (i / numberOfPointsPerOffset) +
					Random.Range (-randomAmplitude, randomAmplitude + 1)));


				unitDestroyedExplosions.Add (new Vector2 (
					targetedUnitParent.transform.localPosition.x +
					targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)CombatManager.ShipSectionTargeted.EngineSection].transform.localPosition.x
					+ shipEngineSectionTargetedOffset.x + shipEngineSectionTargetedOffsetSpacing.x * (i / numberOfPointsPerOffset) +
					Random.Range (-randomAmplitude, randomAmplitude + 1),

					targetedUnitParent.transform.localPosition.y +
					targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)CombatManager.ShipSectionTargeted.EngineSection].transform.localPosition.y
					+ shipEngineSectionTargetedOffset.y + shipEngineSectionTargetedOffsetSpacing.y * (i / numberOfPointsPerOffset) +
					Random.Range (-randomAmplitude, randomAmplitude + 1)));

			}

			break;

		case CombatUnit.UnitType.BirdOfPrey:

			for (int i = 0; i < offsetsPerSection * numberOfPointsPerOffset; i++) {

				unitDestroyedExplosions.Add (new Vector2 (
					targetedUnitParent.transform.localPosition.x +
					targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)CombatManager.ShipSectionTargeted.PhasorSection].transform.localPosition.x
					+ shipPhasorSectionTargetedOffset.x + shipPhasorSectionTargetedOffsetSpacing.x * (i / numberOfPointsPerOffset) +
					Random.Range (-randomAmplitude, randomAmplitude + 1),

					targetedUnitParent.transform.localPosition.y +
					targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)CombatManager.ShipSectionTargeted.PhasorSection].transform.localPosition.y
					+ shipPhasorSectionTargetedOffset.y + shipPhasorSectionTargetedOffsetSpacing.y * (i / numberOfPointsPerOffset) +
					Random.Range (-randomAmplitude, randomAmplitude + 1)));


				unitDestroyedExplosions.Add (new Vector2 (
					targetedUnitParent.transform.localPosition.x +
					targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)CombatManager.ShipSectionTargeted.TorpedoSection].transform.localPosition.x
					+ shipTorpedoSectionTargetedOffset.x + shipTorpedoSectionTargetedOffsetSpacing.x * (i / numberOfPointsPerOffset) +
					Random.Range (-randomAmplitude, randomAmplitude + 1),

					targetedUnitParent.transform.localPosition.y +
					targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)CombatManager.ShipSectionTargeted.TorpedoSection].transform.localPosition.y
					+ shipTorpedoSectionTargetedOffset.y + shipTorpedoSectionTargetedOffsetSpacing.y * (i / numberOfPointsPerOffset) +
					Random.Range (-randomAmplitude, randomAmplitude + 1)));


				unitDestroyedExplosions.Add (new Vector2 (
					targetedUnitParent.transform.localPosition.x +
					targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)CombatManager.ShipSectionTargeted.EngineSection].transform.localPosition.x
					+ shipEngineSectionTargetedOffset.x + shipEngineSectionTargetedOffsetSpacing.x * (i / numberOfPointsPerOffset) +
					Random.Range (-randomAmplitude, randomAmplitude + 1),

					targetedUnitParent.transform.localPosition.y +
					targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)CombatManager.ShipSectionTargeted.EngineSection].transform.localPosition.y
					+ shipEngineSectionTargetedOffset.y + shipEngineSectionTargetedOffsetSpacing.y * (i / numberOfPointsPerOffset) +
					Random.Range (-randomAmplitude, randomAmplitude + 1)));

			}

			break;


		case CombatUnit.UnitType.Scout:

			for (int i = 0; i < offsetsPerSection * numberOfPointsPerOffset; i++) {

				unitDestroyedExplosions.Add (new Vector2 (
					targetedUnitParent.transform.localPosition.x +
					targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)CombatManager.ShipSectionTargeted.PhasorSection].transform.localPosition.x
					+ shipPhasorSectionTargetedOffset.x + shipPhasorSectionTargetedOffsetSpacing.x * (i / numberOfPointsPerOffset) +
					Random.Range (-randomAmplitude, randomAmplitude + 1),

					targetedUnitParent.transform.localPosition.y +
					targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)CombatManager.ShipSectionTargeted.PhasorSection].transform.localPosition.y
					+ shipPhasorSectionTargetedOffset.y + shipPhasorSectionTargetedOffsetSpacing.y * (i / numberOfPointsPerOffset) +
					Random.Range (-randomAmplitude, randomAmplitude + 1)));


				unitDestroyedExplosions.Add (new Vector2 (
					targetedUnitParent.transform.localPosition.x +
					targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)CombatManager.ShipSectionTargeted.StorageSection].transform.localPosition.x
					+ shipStorageSectionTargetedOffset.x + shipStorageSectionTargetedOffsetSpacing.x * (i / numberOfPointsPerOffset) +
					Random.Range (-randomAmplitude, randomAmplitude + 1),

					targetedUnitParent.transform.localPosition.y +
					targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)CombatManager.ShipSectionTargeted.StorageSection].transform.localPosition.y
					+ shipStorageSectionTargetedOffset.y + shipStorageSectionTargetedOffsetSpacing.y * (i / numberOfPointsPerOffset) +
					Random.Range (-randomAmplitude, randomAmplitude + 1)));


				unitDestroyedExplosions.Add (new Vector2 (
					targetedUnitParent.transform.localPosition.x +
					targetedUnitParent.transform.localScale.x * targetedUnitSections [(int)CombatManager.ShipSectionTargeted.EngineSection].transform.localPosition.x
					+ shipEngineSectionTargetedOffset.x + shipEngineSectionTargetedOffsetSpacing.x * (i / numberOfPointsPerOffset) +
					Random.Range (-randomAmplitude, randomAmplitude + 1),

					targetedUnitParent.transform.localPosition.y +
					targetedUnitParent.transform.localScale.y * targetedUnitSections [(int)CombatManager.ShipSectionTargeted.EngineSection].transform.localPosition.y
					+ shipEngineSectionTargetedOffset.y + shipEngineSectionTargetedOffsetSpacing.y * (i / numberOfPointsPerOffset) +
					Random.Range (-randomAmplitude, randomAmplitude + 1)));

			}

			break;


		default:

			break;

		}

	}

	//this function will remove all targeted unit graphics
	private void RemoveAllTargetedUnitGraphics(){

		for (int i = 0; i < targetedUnitSections.Length; i++) {

			targetedUnitSections [i].SetActive (false);

		}

	}

	//create function to center camera on unit
	private void CenterCameraOnUnit(CombatUnit combatUnit){

		//check if we have a unit passed
		if (combatUnit != null) {

			//set the camera limits
			//these are calculated based on the size of the map so that the camera can only show 1 hex beyond the board in any direction
			float cameraMaxZ = tileMap.maxHeight + tileMap.origin.y - Camera.main.orthographicSize;
			float cameraMinZ = tileMap.origin.y + Camera.main.orthographicSize - tileMap.hexHeight;
			float cameraMaxX = tileMap.maxWidth + tileMap.origin.x - Camera.main.orthographicSize * Camera.main.aspect;
			float cameraMinX = tileMap.origin.x + (Camera.main.orthographicSize * Camera.main.aspect) - tileMap.hexWidth;


			//set the main camera to snap to the selected unit current hex location
			Camera.main.transform.position = new Vector3 (Mathf.Clamp (tileMap.HexToWorldCoordinates (combatUnit.currentLocation).x, cameraMinX, cameraMaxX),
				Mathf.Clamp (Camera.main.transform.position.y, 10.0f, 10.0f),
				Mathf.Clamp (tileMap.HexToWorldCoordinates (combatUnit.currentLocation).z, cameraMinZ, cameraMaxZ));

		}

	}

	//this function handles onDestroy
	private void OnDestroy(){

		RemoveAllListeners ();

	}

	//this function removes all listeners
	private void RemoveAllListeners(){

		//remove a listener
		CombatManager.OnPhasorAttackHitShipPhasorSection.RemoveListener(PhasorHitShipPhasorSectionAction);
		CombatManager.OnPhasorAttackHitShipTorpedoSection.RemoveListener(PhasorHitShipTorpedoSectionAction);
		CombatManager.OnPhasorAttackHitShipStorageSection.RemoveListener(PhasorHitShipStorageSectionAction);
		CombatManager.OnPhasorAttackHitShipCrewSection.RemoveListener(PhasorHitShipCrewSectionAction);
		CombatManager.OnPhasorAttackHitShipEngineSection.RemoveListener(PhasorHitShipEngineSectionAction);
		CombatManager.OnPhasorAttackMissShip.RemoveListener(PhasorMissShipAction);

		CombatManager.OnPhasorAttackHitBasePhasorSection1.RemoveListener(PhasorHitBasePhasorSection1Action);
		CombatManager.OnPhasorAttackHitBasePhasorSection2.RemoveListener(PhasorHitBasePhasorSection2Action);
		CombatManager.OnPhasorAttackHitBaseTorpedoSection.RemoveListener(PhasorHitBaseTorpedoSectionAction);
		CombatManager.OnPhasorAttackHitBaseCrewSection.RemoveListener(PhasorHitBaseCrewSectionAction);
		CombatManager.OnPhasorAttackHitBaseStorageSection1.RemoveListener(PhasorHitBaseStorageSection1Action);
		CombatManager.OnPhasorAttackHitBaseStorageSection2.RemoveListener(PhasorHitBaseStorageSection2Action);
		CombatManager.OnPhasorAttackMissBase.RemoveListener(PhasorMissBaseAction);

		CombatManager.OnLightTorpedoAttackHitShipPhasorSection.RemoveListener(LightTorpedoHitShipPhasorSectionAction);
		CombatManager.OnLightTorpedoAttackHitShipTorpedoSection.RemoveListener(LightTorpedoHitShipTorpedoSectionAction);
		CombatManager.OnLightTorpedoAttackHitShipStorageSection.RemoveListener(LightTorpedoHitShipStorageSectionAction);
		CombatManager.OnLightTorpedoAttackHitShipCrewSection.RemoveListener(LightTorpedoHitShipCrewSectionAction);
		CombatManager.OnLightTorpedoAttackHitShipEngineSection.RemoveListener(LightTorpedoHitShipEngineSectionAction);
		CombatManager.OnLightTorpedoAttackMissShip.RemoveListener(LightTorpedoMissShipAction);

		CombatManager.OnLightTorpedoAttackHitBasePhasorSection1.RemoveListener(LightTorpedoHitBasePhasorSection1Action);
		CombatManager.OnLightTorpedoAttackHitBasePhasorSection2.RemoveListener(LightTorpedoHitBasePhasorSection2Action);
		CombatManager.OnLightTorpedoAttackHitBaseTorpedoSection.RemoveListener(LightTorpedoHitBaseTorpedoSectionAction);
		CombatManager.OnLightTorpedoAttackHitBaseCrewSection.RemoveListener(LightTorpedoHitBaseCrewSectionAction);
		CombatManager.OnLightTorpedoAttackHitBaseStorageSection1.RemoveListener(LightTorpedoHitBaseStorageSection1Action);
		CombatManager.OnLightTorpedoAttackHitBaseStorageSection2.RemoveListener(LightTorpedoHitBaseStorageSection2Action);
		CombatManager.OnLightTorpedoAttackMissBase.RemoveListener(LightTorpedoMissBaseAction);

		CombatManager.OnHeavyTorpedoAttackHitShipPhasorSection.RemoveListener(HeavyTorpedoHitShipPhasorSectionAction);
		CombatManager.OnHeavyTorpedoAttackHitShipTorpedoSection.RemoveListener(HeavyTorpedoHitShipTorpedoSectionAction);
		CombatManager.OnHeavyTorpedoAttackHitShipStorageSection.RemoveListener(HeavyTorpedoHitShipStorageSectionAction);
		CombatManager.OnHeavyTorpedoAttackHitShipCrewSection.RemoveListener(HeavyTorpedoHitShipCrewSectionAction);
		CombatManager.OnHeavyTorpedoAttackHitShipEngineSection.RemoveListener(HeavyTorpedoHitShipEngineSectionAction);
		CombatManager.OnHeavyTorpedoAttackMissShip.RemoveListener(HeavyTorpedoMissShipAction);

		CombatManager.OnHeavyTorpedoAttackHitBasePhasorSection1.RemoveListener(HeavyTorpedoHitBasePhasorSection1Action);
		CombatManager.OnHeavyTorpedoAttackHitBasePhasorSection2.RemoveListener(HeavyTorpedoHitBasePhasorSection2Action);
		CombatManager.OnHeavyTorpedoAttackHitBaseTorpedoSection.RemoveListener(HeavyTorpedoHitBaseTorpedoSectionAction);
		CombatManager.OnHeavyTorpedoAttackHitBaseCrewSection.RemoveListener(HeavyTorpedoHitBaseCrewSectionAction);
		CombatManager.OnHeavyTorpedoAttackHitBaseStorageSection1.RemoveListener(HeavyTorpedoHitBaseStorageSection1Action);
		CombatManager.OnHeavyTorpedoAttackHitBaseStorageSection2.RemoveListener(HeavyTorpedoHitBaseStorageSection2Action);
		CombatManager.OnHeavyTorpedoAttackMissBase.RemoveListener(HeavyTorpedoMissBaseAction);

		CombatManager.OnLightTorpedoAttackDefeatedByFlares.RemoveListener (LightTorpedoFlareSuccessAction);
		CombatManager.OnHeavyTorpedoAttackDefeatedByFlares.RemoveListener (HeavyTorpedoFlareSuccessAction);
		CombatManager.OnLightTorpedoAttackFlaresFailed.RemoveListener (LightTorpedoFlareFailureAction);
		CombatManager.OnHeavyTorpedoAttackFlaresFailed.RemoveListener (HeavyTorpedoFlareFailureAction);

	}

}
