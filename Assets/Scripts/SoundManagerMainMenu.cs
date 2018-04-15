using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SoundManagerMainMenu : MonoBehaviour {

	//managers
	private GameObject uiManager;

	//audioclips
	public AudioClip clipMainBackgroundMusic;

	public AudioClip clipSelectableHover;
	public AudioClip clipSelectableSubmit;


	//audioSource
	private AudioSource audioMainBackgroundMusic;

	private AudioSource audioSelectableHover;
	private AudioSource audioSelectableSubmit;

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
	private float cooldownTimer = 0.13f;

	//this is the cooldown limit
	private float cooldownLimit = 0.125f;

	//this is an initial timer to block UI effects at the scene startup
	private float startupTimer;
	private float startupLimit = 1.0f;

	//this bool controls whether we need to play a UI selection sound
	private bool playUISelectionSound;
	private bool playUISubmitSound;

	//this bool controls blocking sound effects - i can set this to delay the block by a frame
	private bool blockSfx;

	//unityActions
	private UnityAction<bool> startMainBackgroundAction;
	private UnityAction<int> intChangeMusicVolumeAction;
	private UnityAction<int> intChangeSfxVolumeAction;
	private UnityAction selectableHoverAction;
	private UnityAction<Selectable> selectableSetSelectableSubmitAction;
	private UnityAction playSubmitAction;
	//private UnityAction<Toggle> togglePlaySubmitAction;
	private UnityAction fadeOutMainBackgroundAction;


	// Use this for initialization
	public void Init () {

		//get the managers
		uiManager = GameObject.FindGameObjectWithTag("UIManager");

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
			playUISelectionSound = false;
			playUISubmitSound = false;

			return;

		}

		//check if the block flag is set
		if (blockSfx == true) {

			//keep the sfx flags set to false so no menu sounds while we fade out
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
		if (playUISelectionSound == true || playUISubmitSound == true) {

			//call the function to determine which to play
			PlayUISound ();

		}


		cooldownTimer += Time.deltaTime;

	}

	//this function sets the actions
	private void SetUnityActions(){

		startMainBackgroundAction = (startupFlag) => {StartMainBackgroundMusic();};
		intChangeMusicVolumeAction = (volumeLevel) => {GetMusicVolumeLevel();};
		intChangeSfxVolumeAction = (volumeLevel) => {GetSfxVolumeLevel();};
		selectableHoverAction = () => {playUISelectionSound = true;};
		selectableSetSelectableSubmitAction = (selectable) => {playUISubmitSound = true;};
		playSubmitAction = () => {playUISubmitSound = true;};
		//togglePlaySubmitAction = (toggle) => {playUISubmitSound = true;};
		fadeOutMainBackgroundAction = () => {StartFadingOutMainBackgroundMusic();};
			
	}

	//this function adds event listeners
	private void AddEventListeners(){

		//add listener for new scene startup
		uiManager.GetComponent<MainMenuManager>().OnSceneStartup.AddListener(startMainBackgroundAction);

		//add listener for music volume change
		uiManager.GetComponent<Settings>().OnChangeMusicVolume.AddListener(intChangeMusicVolumeAction);

		//add listener for sfx volume change
		uiManager.GetComponent<Settings>().OnChangeSfxVolume.AddListener(intChangeSfxVolumeAction);

		//add listener for selectable hover
		uiManager.GetComponent<UINavigationMainMenu>().OnPointerEnterValidSelectable.AddListener(selectableHoverAction);

		//add listener for clicking selectable
		UISelection.OnClickedSelectable.AddListener(selectableSetSelectableSubmitAction);

		//add listener for submitting selectable
		UISelection.OnSubmitSelectable.AddListener(selectableSetSelectableSubmitAction);

		//add listener for selecting selectable
		UISelection.OnSelectedSelectable.AddListener(selectableSetSelectableSubmitAction);

		//add listener for clicking a nongroup selectable
		uiManager.GetComponent<UINavigationMainMenu>().OnPointerClickValidSelectable.AddListener(playSubmitAction);

		//add listener for input field submit
		UISelection.OnEndEditSelectable.AddListener(playSubmitAction);

		//add listener for exiting the main menu scene
		uiManager.GetComponent<MainMenuManager>().OnBeginSceneExit.AddListener(fadeOutMainBackgroundAction);

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

		audioSelectableHover = AddAudio (clipSelectableHover, false, false, 1.0f);
		audioSelectableSubmit = AddAudio (clipSelectableSubmit, false, false, 1.0f);

		//fill up the sfx array
		sfxAudioSources = new AudioSource[2];
		sfxAudioSources [0] = audioSelectableHover;
		sfxAudioSources [1] = audioSelectableSubmit;

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

	//this function plays the appropriate UI selection Sound
	private void PlayUISound(){

		//check if the UI Submit flag is true - we want that to take priority
		if (playUISubmitSound == true) {

			//play the sound
			PlayAudioSelectableSubmit ();

			//clear the flags
			playUISubmitSound = false;
			playUISelectionSound = false;

		} else if (playUISelectionSound == true) {

			PlayAudioSelectableHover ();

			//clear the flag
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

			//remove listener for new scene startup
			uiManager.GetComponent<MainMenuManager>().OnSceneStartup.RemoveListener(startMainBackgroundAction);

			//remove listener for music volume change
			uiManager.GetComponent<Settings>().OnChangeMusicVolume.RemoveListener(intChangeMusicVolumeAction);

			//remove listener for sfx volume change
			uiManager.GetComponent<Settings>().OnChangeSfxVolume.RemoveListener(intChangeSfxVolumeAction);

			//remove listener for selectable hover
			uiManager.GetComponent<UINavigationMainMenu>().OnPointerEnterValidSelectable.RemoveListener(selectableHoverAction);

			//remove listener for clicking a nongroup selectable
			uiManager.GetComponent<UINavigationMainMenu>().OnPointerClickValidSelectable.RemoveListener(playSubmitAction);

			//remove listener for exiting the main menu scene
			uiManager.GetComponent<MainMenuManager>().OnBeginSceneExit.RemoveListener(fadeOutMainBackgroundAction);
		}

		//remove listener for clicking selectable
		UISelection.OnClickedSelectable.RemoveListener(selectableSetSelectableSubmitAction);

		//remove listener for submitting selectable
		UISelection.OnSubmitSelectable.RemoveListener(selectableSetSelectableSubmitAction);

		//remove listener for selecting selectable
		UISelection.OnSelectedSelectable.RemoveListener(selectableSetSelectableSubmitAction);

		//remove listener for input field submit
		UISelection.OnEndEditSelectable.RemoveListener(playSubmitAction);


	}

}
