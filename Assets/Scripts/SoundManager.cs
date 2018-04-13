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


	//audioSource
	private AudioSource audioMainBackgroundMusic;

	private AudioSource audioPhasorFire;
	private AudioSource audioXRayFire;
	private AudioSource audioPhasorHit;
	private AudioSource audioSelectableHover;
	private AudioSource audioSelectableSubmit;
	private AudioSource audioSelectUnit;

	//this array will hold all the sfxAudioSources
	private AudioSource[] sfxAudioSources;

	//this bool flag determines whether we should be fading in background music
	private bool isFadingInMusic;

	//this variable controls how long the fade in time is for background music
	private float fadeInTime = 5.0f;

	//this variable keeps track of the timer for fading in
	private float fadeInTimer = 0.0f;

	//this tracks the music volume level
	private float musicVolumeLevel;

	//this tracks the sound effects volume level
	private float sfxVolumeLevel;

	//this tracks the cooldown timer
	private float cooldownTimer;

	//this is the cooldown limit
	private float cooldownLimit = 0.05f;

	//this bool controls whether we need to play a UI selection sound
	bool playUISelectionSound;
	bool playUISubmitSound;
	bool playSelectUnitSound;


	//unityActions
	private UnityAction startMainBackgroundAction;
	private UnityAction<Player> playerStartMainBackgroundAction;
	private UnityAction<int> intChangeMusicVolumeAction;
	private UnityAction<int> intChangeSfxVolumeAction;
	private UnityAction phasorFireAction;
	private UnityAction xRayFireAction;
	private UnityAction phasorHitAction;
	private UnityAction selectableHoverAction;
	private UnityAction<Selectable> selectableSetSelectableHoverAction;
	private UnityAction<Selectable> selectableSetSelectableSubmitAction;
	private UnityAction playSubmitAction;
	private UnityAction playSelectUnitAction;


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

		//check if we need to play a UI sound
		if (playUISelectionSound == true || playUISubmitSound == true || playSelectUnitSound == true) {

			//call the function to determine which to play
			PlayUISound ();

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
		selectableSetSelectableHoverAction = (selectable) => {playUISelectionSound = true;};
		//selectableSetSelectableSubmitAction = (selectable) => {PlayAudioSelectableSubmit();};
		selectableSetSelectableSubmitAction = (selectable) => {playUISubmitSound = true;};
		playSubmitAction = () => {playUISubmitSound = true;};
		playSelectUnitAction = () => {playSelectUnitSound = true;};

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
		UISelection.OnSelectedSelectable.AddListener(selectableSetSelectableHoverAction);

		//add listener for cancelling out of a menu
		uiManager.GetComponent<UINavigationMain>().OnCloseWindowWithCancel.AddListener(playSubmitAction);

		//add listener for clicking a nongroup selectable
		uiManager.GetComponent<UINavigationMain>().OnPointerClickValidSelectable.AddListener(playSubmitAction);

		//add listener for setting selected unit
		mouseManager.OnSetSelectedUnit.AddListener(playSelectUnitAction);

		//add listener for clearin selected unit
		mouseManager.OnClearSelectedUnit.AddListener(playSelectUnitAction);

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


		//fill up the sfx array
		sfxAudioSources = new AudioSource[6];
		sfxAudioSources [0] = audioPhasorFire;
		sfxAudioSources [1] = audioXRayFire;
		sfxAudioSources [2] = audioPhasorHit;
		sfxAudioSources [3] = audioSelectableHover;
		sfxAudioSources [4] = audioSelectableSubmit;
		sfxAudioSources [5] = audioSelectUnit;


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

	//this function plays the appropriate UI selection Sound
	private void PlayUISound(){

		//check if the unit select flag is true
		if (playSelectUnitSound == true) {

			//play the sound
			PlayAudioSelectUnit ();

			//clear the flags
			playSelectUnitSound = false;
			playUISubmitSound = false;
			playUISelectionSound = false;


		}

		//check if the UI Submit flag is true - we want that to take priority
		else if (playUISubmitSound == true) {

			//play the sound
			PlayAudioSelectableSubmit ();

			//clear the flags
			playSelectUnitSound = false;
			playUISubmitSound = false;
			playUISelectionSound = false;

		} else if (playUISelectionSound == true) {

			PlayAudioSelectableHover ();

			//clear the flag
			playSelectUnitSound = false;
			playUISelectionSound = false;
			playUISubmitSound = false;

		}

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

			//remove listener for ui state change
			uiManager.GetComponent<UINavigationMain>().OnUIStateChange.RemoveListener(playSubmitAction);

			//remove listener for cancelling out of a menu
			uiManager.GetComponent<UINavigationMain>().OnCloseWindowWithCancel.RemoveListener(playSubmitAction);

			//remove listener for clicking a nongroup selectable
			uiManager.GetComponent<UINavigationMain>().OnPointerClickValidSelectable.RemoveListener(playSubmitAction);

		}

		if (gameManager != null) {

			//remove listener for new scene startup
			gameManager.OnSceneStartup.RemoveListener (startMainBackgroundAction);

			//remove listener for loaded game
			gameManager.OnLoadedTurn.RemoveListener(playerStartMainBackgroundAction);

		}

		if (mouseManager != null) {


			//remove listener for setting selected unit
			mouseManager.OnSetSelectedUnit.RemoveListener (playSelectUnitAction);

			//remove listener for clearin selected unit
			mouseManager.OnClearSelectedUnit.RemoveListener (playSelectUnitAction);

		}

		//remove listener for clicking selectable
		UISelection.OnClickedSelectable.RemoveListener(selectableSetSelectableSubmitAction);

		//remove listener for submitting selectable
		UISelection.OnSubmitSelectable.RemoveListener(selectableSetSelectableSubmitAction);

		//remove listener for selecting selectable
		UISelection.OnSelectedSelectable.RemoveListener(selectableSetSelectableHoverAction);

	}

}
