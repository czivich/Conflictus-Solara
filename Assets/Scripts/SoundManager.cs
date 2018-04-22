using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour {

	//managers
	private UIManager uiManager;
	private GameManager gameManager;
	private MouseManager mouseManager;

	//audioclips
	public AudioClip clipMainBackgroundMusic;

	public AudioClip clipPhasorFire;
	public AudioClip clipXRayFire;
	public AudioClip clipPhasorHit;
	public AudioClip clipSelectableHover;
	public AudioClip clipSelectableSubmit;
	public AudioClip clipSelectUnit;
	public AudioClip clipTargetUnit;
	public AudioClip clipEngineDrone;
	public AudioClip clipWarpFadeOut;
	public AudioClip clipFireWeapon;
	public AudioClip clipTractorBeam;
	public AudioClip clipDilithiumCrystal;
	public AudioClip clipTrilithiumCrystal;
	public AudioClip clipCloak;
	public AudioClip clipUncloak;
	public AudioClip clipRepair;
	public AudioClip clipNewShip;
	public AudioClip clipNewColony;
	public AudioClip clipSunburstDamage;
	public AudioClip clipChargePhasor;
	public AudioClip clipChargeXRay;

	//audioSource
	private AudioSource audioMainBackgroundMusic;

	private AudioSource audioPhasorFire;
	private AudioSource audioXRayFire;
	private AudioSource audioPhasorHit;
	private AudioSource audioSelectableHover;
	private AudioSource audioSelectableSubmit;
	private AudioSource audioSelectUnit;
	private AudioSource audioTargetUnit;
	private AudioSource audioEngineDrone;
	private AudioSource audioWarpFadeOut;
	private AudioSource audioWarpFadeOutTowed;
	private AudioSource audioFireWeapon;
	private AudioSource audioTractorBeam;
	private AudioSource audioDilithiumCrystal;
	private AudioSource audioTrilithiumCrystal;
	private AudioSource audioCloak;
	private AudioSource audioUncloak;
	private AudioSource audioRepair;
	private AudioSource audioNewShip;
	private AudioSource audioNewColony;
	private AudioSource audioSunburstDamage;
	private AudioSource audioChargePhasor;
	private AudioSource audioChargeXRay;

	//this array will hold all the sfxAudioSources
	private AudioSource[] sfxAudioSources;

	//this bool flag determines whether we should be fading in background music
	private bool isFadingInMusic;
	private bool isFadingOutMusic;

	//this variable controls how long the fade in time is for background music
	private float fadeInTime = 5.0f;

	//this variable keeps track of the timer for fading in
	private float fadeInTimer = 0.0f;

	//these variables are for fading out
	//fade out time should match the scene transition fade panel time
	private float fadeOutTime = 1.5f;
	private float fadeOutTimer = 0.0f;

	//this tracks the music volume level
	private float musicVolumeLevel;

	//this tracks the sound effects volume level
	private float sfxVolumeLevel;

	//this tracks the cooldown timer
	private float cooldownTimer;

	//this is the cooldown limit
	private float cooldownLimit = 0.125f;

	//this is an initial timer to block UI effects at the scene startup
	private float startupTimer;
	private float startupLimit = 1.0f;

	//this bool controls whether we need to play a UI selection sound
	private bool playUISelectionSound;
	private bool playUISubmitSound;
	private bool playSelectUnitSound;
	private bool playTargetUnitSound;

	//this bool controls blocking sound effects - i can set this to delay the block by a frame
	private bool blockSfx;

	//I'm going to use mathf.smoothdamp to ramp into and out of the engine sound effect
	//these variables will control the engine sound
	private float engineFadeDuration;
	private float engineSmoothDampVelocity;
	private float currentEngineVolume;
	private float targetEngineVolume;
	private bool shouldUpdateEngineVolume;

	//these variables will control the tractor beam sound
	private float tractorBeamFadeDuration;
	private float tractorBeamSmoothDampVelocity;
	private float currentTractorBeamVolume;
	private float targetTractorBeamVolume;
	private bool shouldUpdateTractorBeamVolume;

	//unityActions
	private UnityAction startMainBackgroundAction;
	private UnityAction<Player> playerStartMainBackgroundAction;
	private UnityAction<int> intChangeMusicVolumeAction;
	private UnityAction<int> intChangeSfxVolumeAction;
	private UnityAction phasorFireAction;
	private UnityAction xRayFireAction;
	private UnityAction phasorHitAction;
	private UnityAction selectableHoverAction;
	//private UnityAction<Selectable> selectableSetSelectableHoverAction;
	private UnityAction<Selectable> selectableSetSelectableSubmitAction;
	private UnityAction playSubmitAction;
	private UnityAction playSelectUnitAction;
	private UnityAction<Toggle> togglePlaySubmitAction;
	private UnityAction playTargetUnitAction;
	private UnityAction playHoverUnitAction;
	private UnityAction fadeOutMainBackgroundAction;
	private UnityAction<Ship> startEngineSoundAction;
	private UnityAction<Ship> stopEngineSoundAction;
	private UnityAction<Ship> warpFadeOutSoundAction;
	private UnityAction<CombatUnit, CombatUnit, string> fireWeaponAction;
	private UnityAction<Ship> startTractorBeamSoundAction;
	private UnityAction<Ship> stopTractorBeamSoundAction;
	private UnityAction<CombatUnit, CombatUnit, string> useDilithiumCrystalAction;
	private UnityAction<CombatUnit, CombatUnit, string> useTrilithiumCrystalAction;
	private UnityAction<CombatUnit> cloakAction;
	private UnityAction<CombatUnit> uncloakAction;
	private UnityAction<CombatUnit, CombatUnit, string> useRepairAction;
	private UnityAction<Player> createNewUnitAction;
	private UnityAction<string, Player> createNewColonyAction;
	private UnityAction<CombatUnit, int> sunburstDamageAction;
	private UnityAction chargePhasorAction;
	private UnityAction chargeXRayAction;

	// Use this for initialization
	public void Init () {

		//get the managers
		uiManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
		gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
		mouseManager = GameObject.FindGameObjectWithTag("MouseManager").GetComponent<MouseManager>();

		//create the audiosources
		AddAudioSources();

		//get the music volume level
		GetMusicVolumeLevel();

		//set unity actions
		SetUnityActions ();

		//add event listeners
		AddEventListeners();

	}

	private void Update(){

		//check the statup timer
		if (startupTimer < startupLimit) {

			startupTimer += Time.deltaTime;

			//clear the flags
			playTargetUnitSound = false;
			playSelectUnitSound = false;
			playUISelectionSound = false;
			playUISubmitSound = false;

			return;

		}

		//check if the block flag is set
		if (blockSfx == true) {

			//keep the sfx flags set to false so no menu sounds while we fade out
			playTargetUnitSound = false;
			playSelectUnitSound = false;
			playUISelectionSound = false;
			playUISubmitSound = false;

		}

		//check if we are supposed to be fading in
		if (isFadingInMusic == true) {

			//check if the fade in timer is less than the value
			if (fadeInTimer <= fadeInTime) {

				//scale the volume proportional to the time elapsed
				audioMainBackgroundMusic.volume = fadeInTimer / fadeInTime * musicVolumeLevel;

				fadeInTimer += Time.deltaTime;

			} else {

				//here, the timer has elapsed more than the time, so we should be at max volume
				audioMainBackgroundMusic.volume = musicVolumeLevel;

				//turn off the flag
				isFadingInMusic = false;

			}

		}

		//check if we are supposed to be fading out
		if (isFadingOutMusic == true) {

			//check if the fade out timer is less than the value
			if (fadeOutTimer <= fadeOutTime) {

				//scale the volume proportional to the time elapsed
				audioMainBackgroundMusic.volume = (1.0f - fadeOutTimer / fadeOutTime) * musicVolumeLevel;

				fadeOutTimer += Time.deltaTime;

			} else {

				//here, the timer has elapsed more than the time, so we should be at 0 volume
				audioMainBackgroundMusic.volume = 0.0f;

			}

			//set the block flag
			blockSfx = true;

		}

		//check if we need to play a UI sound
		if (playUISelectionSound == true || playUISubmitSound == true || playSelectUnitSound == true || playTargetUnitSound == true) {

			//call the function to determine which to play
			PlayUISound ();

		}

		//check if we should be changing an engine sound
		if (shouldUpdateEngineVolume == true) {

			//smoothDamp the volume to the target
			float newVolume = Mathf.SmoothDamp (currentEngineVolume, targetEngineVolume, ref engineSmoothDampVelocity, engineFadeDuration);

			//set the engine volume to the new value
			currentEngineVolume = newVolume;
			audioEngineDrone.volume = currentEngineVolume;

			//check if we are near the target
			if (Mathf.Abs (currentEngineVolume - targetEngineVolume) < .01f) {

				//we are close enough to the target that we want to snap it to the final value

				//set the engine volume to the final value
				currentEngineVolume = targetEngineVolume;
				audioEngineDrone.volume = currentEngineVolume;

				//clear the shouldUpdate flag
				shouldUpdateEngineVolume = false;

			}

		}

		//check if we should be changing a tractor beam sound
		if (shouldUpdateTractorBeamVolume == true) {

			//smoothDamp the volume to the target
			float newVolume = Mathf.SmoothDamp (currentTractorBeamVolume, targetTractorBeamVolume, ref tractorBeamSmoothDampVelocity, tractorBeamFadeDuration);

			//set the engine volume to the new value
			currentTractorBeamVolume = newVolume;
			audioTractorBeam.volume = currentTractorBeamVolume;

			//check if we are near the target
			if (Mathf.Abs (currentTractorBeamVolume - targetTractorBeamVolume) < .01f) {

				//we are close enough to the target that we want to snap it to the final value

				//set the engine volume to the final value
				currentTractorBeamVolume = targetTractorBeamVolume;
				audioTractorBeam.volume = currentTractorBeamVolume;

				//clear the shouldUpdate flag
				shouldUpdateTractorBeamVolume = false;

			}

		}

		cooldownTimer += Time.deltaTime;

	}

	//this function sets the actions
	private void SetUnityActions(){

		startMainBackgroundAction = () => {StartMainBackgroundMusic();};
		playerStartMainBackgroundAction = (player) => {StartMainBackgroundMusic();};
		intChangeMusicVolumeAction = (volumeLevel) => {GetMusicVolumeLevel();};
		intChangeSfxVolumeAction = (volumeLevel) => {GetSfxVolumeLevel();};
		phasorFireAction = () => {audioPhasorFire.Play ();};
		xRayFireAction = () => {audioXRayFire.Play ();};
		phasorHitAction = () => {audioPhasorHit.PlayDelayed(.05f);};   //delay .125 seconds
		selectableHoverAction = () => {playUISelectionSound = true;};
		//selectableSetSelectableHoverAction = (selectable) => {PlayAudioSelectableHover();};
		//selectableSetSelectableHoverAction = (selectable) => {playUISelectionSound = true;};
		//selectableSetSelectableSubmitAction = (selectable) => {PlayAudioSelectableSubmit();};
		selectableSetSelectableSubmitAction = (selectable) => {playUISubmitSound = true;};
		playSubmitAction = () => {playUISubmitSound = true;};
		playSelectUnitAction = () => {playSelectUnitSound = true;};
		togglePlaySubmitAction = (toggle) => {playUISubmitSound = true;};
		playTargetUnitAction = () => {playTargetUnitSound = true;};
		playHoverUnitAction = () => {

			//check if the hovered object is a combat unit
			if(mouseManager.hoveredObject.GetComponent<CombatUnit>() == true){

				playUISelectionSound = true;

			}
				
		};

		fadeOutMainBackgroundAction = () => {StartFadingOutMainBackgroundMusic();};

		startEngineSoundAction = (movingShip) => {StartEngineSound ();};
		stopEngineSoundAction = (movingShip) => {StopEngineSound ();};
		warpFadeOutSoundAction = (movingShip) => {PlayWarpFadeOutSound(movingShip);};

		fireWeaponAction = (attackingUnit, targetedUnit, sectionTargeted) => {PlayFireWeaponSound ();};

		startTractorBeamSoundAction = (movingShip) => {StartTractorBeamSound ();};
		stopTractorBeamSoundAction = (movingShip) => {StopTractorBeamSound ();};

		useDilithiumCrystalAction = (attackingUnit, targetedUnit, sectionTargeted) => {PlayDilithiumCrystalSound ();};
		useTrilithiumCrystalAction = (attackingUnit, targetedUnit, sectionTargeted) => {PlayTrilithiumCrystalSound ();};

		cloakAction = (cloakingUnit) => {PlayCloakSound ();};
		uncloakAction = (cloakingUnit) => {PlayUncloakSound ();};

		useRepairAction = (attackingUnit, targetedUnit, sectionTargeted) => {PlayRepairSound ();};

		createNewUnitAction = (currentPlayer) => {PlayNewShipSound ();};

		createNewColonyAction = (planetName, currentPlayer) => {

			//only play the sound if this isn't the first turn - like if we just loaded
			if (gameManager.firstTurnHasHappened == true) {

				PlayNewColonySound ();
			}

		};

		sunburstDamageAction = (damagedUnit, damage) => {PlaySunburstDamageSound();};

		chargePhasorAction = () => {PlayChargePhasorSound();};
		chargeXRayAction = () => {PlayChargeXRaySound();};

	}

	//this function adds event listeners
	private void AddEventListeners(){

		//add listener for new scene startup
		gameManager.OnSceneStartup.AddListener(startMainBackgroundAction);

		//add listener for loaded game
		gameManager.OnLoadedTurn.AddListener(playerStartMainBackgroundAction);

		//add listener for music volume change
		uiManager.GetComponent<Settings>().OnChangeMusicVolume.AddListener(intChangeMusicVolumeAction);

		//add listener for sfx volume change
		uiManager.GetComponent<Settings>().OnChangeSfxVolume.AddListener(intChangeSfxVolumeAction);

		//add listener for phasor fire
		uiManager.GetComponent<CutsceneManager>().OnFirePhasors.AddListener(phasorFireAction);

		//add listener for xray fire
		uiManager.GetComponent<CutsceneManager>().OnFireXRay.AddListener(xRayFireAction);

		//add listener for phasor hit
		uiManager.GetComponent<CutsceneManager>().OnPhasorHit.AddListener(phasorHitAction);

		//add listener for selectable hover
		uiManager.GetComponent<UINavigationMain>().OnPointerEnterValidSelectable.AddListener(selectableHoverAction);

		//add listener for clicking selectable
		UISelection.OnClickedSelectable.AddListener(selectableSetSelectableSubmitAction);

		//add listener for submitting selectable
		UISelection.OnSubmitSelectable.AddListener(selectableSetSelectableSubmitAction);

		//add listener for selecting selectable
		UISelection.OnSelectedSelectable.AddListener(selectableSetSelectableSubmitAction);

		//add listener for cancelling out of a menu
		uiManager.GetComponent<UINavigationMain>().OnCloseWindowWithCancel.AddListener(playSubmitAction);

		//add listener for clicking a nongroup selectable
		uiManager.GetComponent<UINavigationMain>().OnPointerClickValidSelectable.AddListener(playSubmitAction);

		//add listener for setting selected unit
		mouseManager.OnSetSelectedUnit.AddListener(playSelectUnitAction);

		//add listener for clearing selected unit
		mouseManager.OnClearSelectedUnit.AddListener(playSelectUnitAction);

		//add listener for entering end turn prompt
		uiManager.GetComponent<EndTurnDropDown>().OnEnterEndTurnPrompt.AddListener(togglePlaySubmitAction);

		//add listener for input field submit
		UISelection.OnEndEditSelectable.AddListener(playSubmitAction);

		//add listener for setting target unit
		mouseManager.OnSetTargetedUnit.AddListener(playTargetUnitAction);

		//add listener for clearing target unit
		mouseManager.OnClearTargetedUnit.AddListener(playTargetUnitAction);

		//add listener for hovering over a combat unit
		mouseManager.OnSetHoveredObject.AddListener(playHoverUnitAction);

		//add listener for exiting the main menu scene
		gameManager.OnBeginSceneExit.AddListener(fadeOutMainBackgroundAction);

		//add listeners for movement starting and stopping
		EngineSection.OnMoveStart.AddListener(startEngineSoundAction);
		EngineSection.OnMoveFinish.AddListener(stopEngineSoundAction);

		//add listener for warp start
		EngineSection.OnStartWarpFadeOut.AddListener(warpFadeOutSoundAction);

		//add listener for movement resuming
		EngineSection.OnResumeMovementAfterFadeIn.AddListener(startEngineSoundAction);

		//add listener for movement waiting for a towed unit warping
		EngineSection.OnWaitForTowedUnitAfterWarping.AddListener(stopEngineSoundAction);

		//add listeners for firing weapons
		uiManager.GetComponent<PhasorMenu>().OnFirePhasors.AddListener(fireWeaponAction);
		uiManager.GetComponent<TorpedoMenu>().OnFireLightTorpedo.AddListener(fireWeaponAction);
		uiManager.GetComponent<TorpedoMenu>().OnFireHeavyTorpedo.AddListener(fireWeaponAction);

		//add listeners for tractor beam starting and stopping
		uiManager.GetComponent<TractorBeamMenu>().OnTurnOnTractorBeamToggle.AddListener(startTractorBeamSoundAction);
		uiManager.GetComponent<TractorBeamMenu>().OnTurnOffTractorBeamToggle.AddListener(stopTractorBeamSoundAction);

		//add listeners for crystal use
		uiManager.GetComponent<UseItemMenu>().OnUseDilithiumCrystal.AddListener(useDilithiumCrystalAction);
		uiManager.GetComponent<UseItemMenu>().OnUseTrilithiumCrystal.AddListener(useTrilithiumCrystalAction);

		//add listeners for cloaking device
		uiManager.GetComponent<CloakingDeviceMenu>().OnTurnOnCloakingDevice.AddListener(cloakAction);
		uiManager.GetComponent<CloakingDeviceMenu>().OnTurnOffCloakingDevice.AddListener(uncloakAction);

		//add listener for repair crew
		uiManager.GetComponent<CrewMenu>().OnUseRepairCrew.AddListener(useRepairAction);

		//add listener for new unit created
		gameManager.OnNewUnitCreated.AddListener(createNewUnitAction);

		//add listener for new colony
		ColonyManager.OnPlanetOwnerChanged.AddListener(createNewColonyAction);

		//add listener for taking sunburst damage
		Sunburst.OnSunburstDamageDealt.AddListener(sunburstDamageAction);

		//add listeners for charging phasors
		uiManager.GetComponent<CutsceneManager>().OnChargePhasors.AddListener(chargePhasorAction);
		uiManager.GetComponent<CutsceneManager>().OnChargeXRay.AddListener(chargeXRayAction);

	}

	//this function creates a new audioSource component for the passed clip
	private AudioSource AddAudio(AudioClip clip, bool loop, bool playAwake, float vol) { 
		
		AudioSource newAudio = gameObject.AddComponent<AudioSource>();
		newAudio.clip = clip; 
		newAudio.loop = loop;
		newAudio.playOnAwake = playAwake;
		newAudio.volume = vol; 
		return newAudio; 

	}

	//this function adds the audiosources
	private void AddAudioSources(){

		audioMainBackgroundMusic = AddAudio (clipMainBackgroundMusic, true, false, 1.0f);

		audioPhasorFire = AddAudio (clipPhasorFire, false, false, 1.0f);
		audioXRayFire = AddAudio (clipXRayFire, false, false, 1.0f);
		audioPhasorHit = AddAudio (clipPhasorHit, false, false, 1.0f);
		audioSelectableHover = AddAudio (clipSelectableHover, false, false, 1.0f);
		audioSelectableSubmit = AddAudio (clipSelectableSubmit, false, false, 1.0f);
		audioSelectUnit = AddAudio (clipSelectUnit, false, false, 1.0f);
		audioTargetUnit = AddAudio (clipTargetUnit, false, false, 1.0f);
		audioEngineDrone = AddAudio (clipEngineDrone, true, false, 1.0f);
		audioWarpFadeOut = AddAudio(clipWarpFadeOut, false, false, 1.0f);
		audioWarpFadeOutTowed = AddAudio(clipWarpFadeOut, false, false, 1.0f);
		audioFireWeapon = AddAudio(clipFireWeapon, false, false, 1.0f);
		audioTractorBeam = AddAudio (clipTractorBeam, true, false, 1.0f);
		audioDilithiumCrystal = AddAudio(clipDilithiumCrystal, false, false, 1.0f);
		audioTrilithiumCrystal = AddAudio(clipTrilithiumCrystal, false, false, 1.0f);
		audioCloak = AddAudio(clipCloak, false, false, 1.0f);
		audioUncloak = AddAudio(clipUncloak, false, false, 1.0f);
		audioRepair = AddAudio(clipRepair, false, false, 1.0f);
		audioNewShip = AddAudio(clipNewShip, false, false, 1.0f);
		audioNewColony = AddAudio(clipNewColony, false, false, 1.0f);
		audioSunburstDamage = AddAudio(clipSunburstDamage, false, false, 1.0f);
		audioChargePhasor = AddAudio(clipChargePhasor, false, false, 1.0f);
		audioChargeXRay = AddAudio(clipChargeXRay, false, false, 1.0f);


		//fill up the sfx array
		sfxAudioSources = new AudioSource[22];
		sfxAudioSources [0] = audioPhasorFire;
		sfxAudioSources [1] = audioXRayFire;
		sfxAudioSources [2] = audioPhasorHit;
		sfxAudioSources [3] = audioSelectableHover;
		sfxAudioSources [4] = audioSelectableSubmit;
		sfxAudioSources [5] = audioSelectUnit;
		sfxAudioSources [6] = audioTargetUnit;
		sfxAudioSources [7] = audioEngineDrone;
		sfxAudioSources [8] = audioWarpFadeOut;
		sfxAudioSources [9] = audioWarpFadeOutTowed;
		sfxAudioSources [10] = audioFireWeapon;
		sfxAudioSources [11] = audioTractorBeam;
		sfxAudioSources [12] = audioDilithiumCrystal;
		sfxAudioSources [13] = audioTrilithiumCrystal;
		sfxAudioSources [14] = audioCloak;
		sfxAudioSources [15] = audioUncloak;
		sfxAudioSources [16] = audioRepair;
		sfxAudioSources [17] = audioNewShip;
		sfxAudioSources [18] = audioNewColony;
		sfxAudioSources [19] = audioSunburstDamage;
		sfxAudioSources [20] = audioChargePhasor;
		sfxAudioSources [21] = audioChargeXRay;


	}

	//this function starts the main background music
	private void StartMainBackgroundMusic(){

		//turn on the fading in flag
		isFadingInMusic = true;

		//start playin the clip
		audioMainBackgroundMusic.Play();

		//set the volume to zero
		audioMainBackgroundMusic.volume = 0.0f;

		//reset the timer
		fadeInTimer = 0.0f;


	}

	//this function starts fading out the main background music
	private void StartFadingOutMainBackgroundMusic(){

		//turn on the fading out flag
		isFadingOutMusic = true;

		//turn off the fading in flag (should be off already, unless we go really quickly)
		isFadingInMusic = false;

		//set the music value to the current volume (in case we call this while we are still fading in)
		musicVolumeLevel = audioMainBackgroundMusic.volume;

		//reset the timer
		fadeOutTimer = 0.0f;


	}

	//this function gets the music volume level
	private void GetMusicVolumeLevel(){

		//get the volume from settings
		musicVolumeLevel = uiManager.GetComponent<Settings>().musicVolumeSlider.value / uiManager.GetComponent<Settings>().musicVolumeSlider.maxValue;

		//clamp the value to 0 to 1
		Mathf.Clamp(musicVolumeLevel,0.0f,1.0f);

		//set the volume level
		audioMainBackgroundMusic.volume = musicVolumeLevel;

	}

	//this function gets the sfx volume level
	private void GetSfxVolumeLevel(){

		//get the volume from settings
		sfxVolumeLevel = uiManager.GetComponent<Settings>().sfxVolumeSlider.value / uiManager.GetComponent<Settings>().sfxVolumeSlider.maxValue;

		//clamp the value to 0 to 1
		Mathf.Clamp(sfxVolumeLevel,0.0f,1.0f);

		//set the volume level
		for (int i = 0; i < sfxAudioSources.Length; i++) {

			sfxAudioSources [i].volume = sfxVolumeLevel;

		}
			
	}

	//this function plays the selectable hover sound
	private void PlayAudioSelectableHover(){

		//check if the timer is greater than the limit
		if (cooldownTimer >= cooldownLimit) {

			//play the clip
			audioSelectableHover.Play();

			//reset the timer
			cooldownTimer = 0.0f;

		}

	}

	//this function plays the selectable submit sound
	private void PlayAudioSelectableSubmit(){

		//check if the timer is greater than the limit
		if (cooldownTimer >= cooldownLimit) {

			//play the clip
			audioSelectableSubmit.Play();

			//reset the timer
			cooldownTimer = 0.0f;

		}

	}

	//this function plays the select unit sound
	private void PlayAudioSelectUnit(){

		//check if the timer is greater than the limit
		if (cooldownTimer >= cooldownLimit) {

			//play the clip
			audioSelectUnit.Play();

			//reset the timer
			cooldownTimer = 0.0f;

		}

	}

	//this function plays the target unit sound
	private void PlayAudioTargetUnit(){

		//check if the timer is greater than the limit
		if (cooldownTimer >= cooldownLimit) {

			//play the clip
			audioTargetUnit.Play();

			//reset the timer
			cooldownTimer = 0.0f;

		}

	}

	//this function plays the appropriate UI selection Sound
	private void PlayUISound(){

		//check if the unit target flag is true
		if (playTargetUnitSound == true) {

			//play the sound
			PlayAudioTargetUnit ();

			//clear the flags
			playTargetUnitSound = false;
			playSelectUnitSound = false;
			playUISubmitSound = false;
			playUISelectionSound = false;


		}

		//check if the unit select flag is true
		else if (playSelectUnitSound == true) {

			//play the sound
			PlayAudioSelectUnit ();

			//clear the flags
			playTargetUnitSound = false;
			playSelectUnitSound = false;
			playUISubmitSound = false;
			playUISelectionSound = false;


		}

		//check if the UI Submit flag is true - we want that to take priority
		else if (playUISubmitSound == true) {

			//play the sound
			PlayAudioSelectableSubmit ();

			//clear the flags
			playTargetUnitSound = false;
			playSelectUnitSound = false;
			playUISubmitSound = false;
			playUISelectionSound = false;

		} else if (playUISelectionSound == true) {

			PlayAudioSelectableHover ();

			//clear the flag
			playTargetUnitSound = false;
			playSelectUnitSound = false;
			playUISelectionSound = false;
			playUISubmitSound = false;

		}

	}

	//this function starts the engine sound
	private void StartEngineSound(){

		//set the shouldUpdate flag to true
		shouldUpdateEngineVolume = true;

		//set the target volume
		targetEngineVolume = sfxVolumeLevel;

		//play the sound
		audioEngineDrone.Play();

		//set the fade duration - I like it being a litle longer on stop than start
		engineFadeDuration = 0.2f;

	}

	//this function stops the engine sound
	private void StopEngineSound(){

		//set the shouldUpdate flag to true
		shouldUpdateEngineVolume = true;

		//set the target volume
		targetEngineVolume = 0.0f;

		//set the fade duration - I like it being a litle longer on stop than start
		engineFadeDuration = 0.4f;

	}

	//this function plays the warp fade out sound
	private void PlayWarpFadeOutSound(Ship ship){

		//stop the engine
		StopEngineSound();

		//check if the ship is being towed or not
		if (ship.GetComponent<EngineSection> ().isBeingTowed == true) {

			//if it is being towed, we want to play the towed instance of the audio source
			//I have two of these so that they can both play overlapped when towing
			//if it was the same sound, it would cut off the first play and restart when the second ship warped
			audioWarpFadeOutTowed.PlayDelayed (.10f);

		} else {

			//the else is this ship is not being towed, so just play the regular sound

			//play the sound
			audioWarpFadeOut.PlayDelayed (.10f);

		}

	}

	//this function starts the tractor beam sound
	private void StartTractorBeamSound(){

		//set the shouldUpdate flag to true
		shouldUpdateTractorBeamVolume = true;

		//set the target volume
		targetTractorBeamVolume = sfxVolumeLevel;

		//play the sound
		audioTractorBeam.Play();

		//set the fade duration - I like it being a litle longer on stop than start
		tractorBeamFadeDuration = 0.2f;

	}

	//this function stops the tractor beam sound
	private void StopTractorBeamSound(){

		//set the shouldUpdate flag to true
		shouldUpdateTractorBeamVolume = true;

		//set the target volume
		targetTractorBeamVolume = 0.0f;

		//set the fade duration - I like it being a litle longer on stop than start
		tractorBeamFadeDuration = 0.4f;

	}

	//this function plays the fire weapon sound
	private void PlayFireWeaponSound(){

		//play the sound
		audioFireWeapon.Play();

	}

	//this function plays the dilithium crystal sound
	private void PlayDilithiumCrystalSound(){

		//play the sound
		audioDilithiumCrystal.Play();

	}

	//this function plays the trilithium crystal sound
	private void PlayTrilithiumCrystalSound(){

		//play the sound
		audioTrilithiumCrystal.Play();

	}

	//this function plays the cloaking sound
	private void PlayCloakSound(){

		//play the sound
		audioCloak.Play();

	}

	//this function plays the uncloaking sound
	private void PlayUncloakSound(){

		//play the sound
		audioUncloak.Play();

	}

	//this function plays the repair sound
	private void PlayRepairSound(){

		//play the sound
		audioRepair.Play();

	}

	//this function plays the new ship sound
	private void PlayNewShipSound(){

		//play the sound
		audioNewShip.Play();

	}

	//this function plays the new colony sound
	private void PlayNewColonySound(){

		//play the sound
		audioNewColony.Play();

	}

	//this function plays the new sunburst damage sound
	private void PlaySunburstDamageSound(){

		//play the sound
		audioSunburstDamage.Play();

	}

	//this function plays the new charge phasor sound
	private void PlayChargePhasorSound(){

		//play the sound
		audioChargePhasor.Play();

	}

	//this function plays the new charge xray sound
	private void PlayChargeXRaySound(){

		//play the sound
		audioChargeXRay.Play();

	}

	//this function handles on destroy
	private void OnDestroy(){

		RemoveEventListeners ();

	}

	//this function removes event listeners
	private void RemoveEventListeners(){

		if (uiManager != null) {

			//remove listener for music volume change
			uiManager.GetComponent<Settings>().OnChangeMusicVolume.RemoveListener(intChangeMusicVolumeAction);

			//remove listener for sfx volume change
			uiManager.GetComponent<Settings>().OnChangeSfxVolume.RemoveListener(intChangeSfxVolumeAction);

			//remove listener for phasor fire
			uiManager.GetComponent<CutsceneManager> ().OnFirePhasors.RemoveListener (phasorFireAction);

			//remove listener for xray fire
			uiManager.GetComponent<CutsceneManager>().OnFireXRay.RemoveListener(xRayFireAction);

			//remove listener for phasor hit
			uiManager.GetComponent<CutsceneManager>().OnPhasorHit.RemoveListener(phasorHitAction);

			//remove listener for selectable hover
			uiManager.GetComponent<UINavigationMain>().OnPointerEnterValidSelectable.RemoveListener(selectableHoverAction);

			//remove listener for cancelling out of a menu
			uiManager.GetComponent<UINavigationMain>().OnCloseWindowWithCancel.RemoveListener(playSubmitAction);

			//remove listener for clicking a nongroup selectable
			uiManager.GetComponent<UINavigationMain>().OnPointerClickValidSelectable.RemoveListener(playSubmitAction);

			//remove listener for entering end turn prompt
			uiManager.GetComponent<EndTurnDropDown>().OnEnterEndTurnPrompt.RemoveListener(togglePlaySubmitAction);

			//remove listeners for firing weapons
			uiManager.GetComponent<PhasorMenu>().OnFirePhasors.RemoveListener(fireWeaponAction);
			uiManager.GetComponent<TorpedoMenu>().OnFireLightTorpedo.RemoveListener(fireWeaponAction);
			uiManager.GetComponent<TorpedoMenu>().OnFireHeavyTorpedo.RemoveListener(fireWeaponAction);

			//remove listeners for tractor beam starting and stopping
			uiManager.GetComponent<TractorBeamMenu>().OnTurnOnTractorBeamToggle.RemoveListener(startTractorBeamSoundAction);
			uiManager.GetComponent<TractorBeamMenu>().OnTurnOffTractorBeamToggle.RemoveListener(stopTractorBeamSoundAction);

			//remove listeners for crystal use
			uiManager.GetComponent<UseItemMenu>().OnUseDilithiumCrystal.RemoveListener(useDilithiumCrystalAction);
			uiManager.GetComponent<UseItemMenu>().OnUseTrilithiumCrystal.RemoveListener(useTrilithiumCrystalAction);

			//remove listeners for cloaking device
			uiManager.GetComponent<CloakingDeviceMenu>().OnTurnOnCloakingDevice.RemoveListener(cloakAction);
			uiManager.GetComponent<CloakingDeviceMenu>().OnTurnOffCloakingDevice.RemoveListener(uncloakAction);

			//remove listener for repair crew
			uiManager.GetComponent<CrewMenu>().OnUseRepairCrew.RemoveListener(useRepairAction);

			//remove listeners for charging phasors
			uiManager.GetComponent<CutsceneManager>().OnChargePhasors.RemoveListener(chargePhasorAction);
			uiManager.GetComponent<CutsceneManager>().OnChargeXRay.RemoveListener(chargeXRayAction);

		}

		if (gameManager != null) {

			//remove listener for new scene startup
			gameManager.OnSceneStartup.RemoveListener (startMainBackgroundAction);

			//remove listener for loaded game
			gameManager.OnLoadedTurn.RemoveListener(playerStartMainBackgroundAction);

			//remove listener for exiting the main menu scene
			gameManager.OnBeginSceneExit.RemoveListener(fadeOutMainBackgroundAction);

			//remove listener for new unit created
			gameManager.OnNewUnitCreated.RemoveListener(createNewUnitAction);

		}

		if (mouseManager != null) {


			//remove listener for setting selected unit
			mouseManager.OnSetSelectedUnit.RemoveListener (playSelectUnitAction);

			//remove listener for clearin selected unit
			mouseManager.OnClearSelectedUnit.RemoveListener (playSelectUnitAction);

			//remove listener for setting target unit
			mouseManager.OnSetTargetedUnit.RemoveListener(playTargetUnitAction);

			//remove listener for clearing target unit
			mouseManager.OnClearTargetedUnit.RemoveListener(playTargetUnitAction);

			//remove listener for hovering over a combat unit
			mouseManager.OnSetHoveredObject.RemoveListener(playHoverUnitAction);

		}

		//remove listener for clicking selectable
		UISelection.OnClickedSelectable.RemoveListener(selectableSetSelectableSubmitAction);

		//remove listener for submitting selectable
		UISelection.OnSubmitSelectable.RemoveListener(selectableSetSelectableSubmitAction);

		//remove listener for selecting selectable
		UISelection.OnSelectedSelectable.RemoveListener(selectableSetSelectableSubmitAction);

		//remove listener for input field submit
		UISelection.OnEndEditSelectable.RemoveListener(playSubmitAction);

		//remove listeners for movement starting and stopping
		EngineSection.OnMoveStart.RemoveListener(startEngineSoundAction);
		EngineSection.OnMoveFinish.RemoveListener(stopEngineSoundAction);

		//remove listener for warp start
		EngineSection.OnStartWarpFadeOut.RemoveListener(warpFadeOutSoundAction);

		//remove listener for movement resuming
		EngineSection.OnResumeMovementAfterFadeIn.RemoveListener(startEngineSoundAction);

		//remove listener for movement waiting for a towed unit warping
		EngineSection.OnWaitForTowedUnitAfterWarping.RemoveListener(stopEngineSoundAction);

		//remove listener for new colony
		ColonyManager.OnPlanetOwnerChanged.RemoveListener(createNewColonyAction);

		//remove listener for taking sunburst damage
		Sunburst.OnSunburstDamageDealt.RemoveListener(sunburstDamageAction);

	}

}
