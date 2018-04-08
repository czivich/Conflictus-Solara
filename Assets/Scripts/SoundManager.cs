using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SoundManager : MonoBehaviour {

	//managers
	private UIManager uiManager;

	//audioclips
	public AudioClip clipPhasorFire;
	public AudioClip clipXRayFire;
	public AudioClip clipPhasorHit;


	//audioSource
	private AudioSource audioPhasorFire;
	private AudioSource audioXRayFire;
	private AudioSource audioPhasorHit;


	//unityActions
	private UnityAction phasorFireAction;
	private UnityAction xRayFireAction;
	private UnityAction phasorHitAction;


	// Use this for initialization
	public void Init () {

		//get the managers
		uiManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();

		//create the audiosources
		AddAudioSources();

		//set unity actions
		SetUnityActions ();

		//add event listeners
		AddEventListeners();

	}

	//this function sets the actions
	private void SetUnityActions(){

		phasorFireAction = () => {audioPhasorFire.Play ();};
		xRayFireAction = () => {audioXRayFire.Play ();};
		phasorHitAction = () => {audioPhasorHit.Play ();};

	}

	//this function adds event listeners
	private void AddEventListeners(){

		//add listener for phasor fire
		uiManager.GetComponent<CutsceneManager>().OnFirePhasors.AddListener(phasorFireAction);

		//add listener for xray fire
		uiManager.GetComponent<CutsceneManager>().OnFireXRay.AddListener(xRayFireAction);

		//add listener for phasor hit
		uiManager.GetComponent<CutsceneManager>().OnPhasorHit.AddListener(phasorHitAction);

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

		audioPhasorFire = AddAudio (clipPhasorFire, false, false, 0.1f);
		audioXRayFire = AddAudio (clipXRayFire, false, false, 0.1f);
		audioPhasorHit = AddAudio (clipPhasorHit, false, false, 0.5f);
	}

	//this function handles on destroy
	private void OnDestroy(){

		RemoveEventListeners ();

	}

	//this function removes event listeners
	private void RemoveEventListeners(){

		if (uiManager != null) {
			
			//remove listener for phasor fire
			uiManager.GetComponent<CutsceneManager> ().OnFirePhasors.RemoveListener (phasorFireAction);

			//remove listener for xray fire
			uiManager.GetComponent<CutsceneManager>().OnFireXRay.RemoveListener(xRayFireAction);

			//remove listener for phasor hit
			uiManager.GetComponent<CutsceneManager>().OnPhasorHit.RemoveListener(phasorHitAction);

		}

	}

}
