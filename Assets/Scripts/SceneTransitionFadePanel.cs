using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneTransitionFadePanel : MonoBehaviour {

	//variable to hold the fade panel
	public GameObject fadePanel;

	//variable to hold the UIManager
	private GameObject UIManager;

	//variable to hold the GameManager
	private GameManager gameManager;

	//variables for the fade values
	private float fullyFadedIn = 0.0f;
	private float fullyFadedOut = 1.0f;

	//variables for the fade transitions
	private float currentFade;
	private float targetFade;

	//variable for the fade time duration
	private float shortFadeDuration = 1.5f;
	private float longFadeDuration = 3.0f;

	//variable to hold the fade duration
	private float fadeDuration;

	//variable for velocity used by smoothDamp
	private float smoothDampVelocity;

	//variable to hold the fadePanelColor
	private Color fadePanelColor;

	//this flag indicates whether the panel should be updating
	private bool shouldUpdate;

	//unity events for fade complete
	public UnityEvent OnFadeInComplete = new UnityEvent();
	public UnityEvent OnFadeOutComplete = new UnityEvent();

	//unityActions
	private UnityAction<bool> boolFadeInAction;
	private UnityAction blankFadeInAction;


	// Use this for initialization
	public void Init () {

		//get the UIManager
		UIManager = GameObject.FindGameObjectWithTag("UIManager");

		//check if we are in the main game scene
		if(SceneManager.GetActiveScene ().buildIndex == 1){

			//get the gameManager
			gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

		}

		//cache the color
		fadePanelColor = fadePanel.GetComponent<Image>().color;

		//set the initial fade duration
		fadeDuration = shortFadeDuration;

		//set the actions
		SetActions();

		//add listeners
		AddEventListeners();

	}
	
	// Update is called once per frame
	void Update () {

		//only update if the shouldUpdate is true
		if (shouldUpdate == true) {

			//smoothDamp the alpha to the target
			float newAlpha = Mathf.SmoothDamp (currentFade, targetFade, ref smoothDampVelocity, fadeDuration);

			//update the color alpha
			currentFade = newAlpha;

			//set the color alpha to the current fade
			fadePanelColor.a = currentFade;

			fadePanel.GetComponent<Image> ().color = fadePanelColor;

			//check if we are near the target
			if (Mathf.Abs (currentFade - targetFade) < .01f) {

				//if we are, disable the panel
				//fadePanel.SetActive(false);

				//if we are, set the color to the target
				fadePanelColor.a = targetFade;

				fadePanel.GetComponent<Image> ().color = fadePanelColor;

				//check if we are fading in
				if (Mathf.Abs (targetFade - fullyFadedIn) < .01f) {

					//if we are fading in, we want to turn off the panel
					fadePanel.SetActive (false);

					//set the shouldUpdate to false
					shouldUpdate = false;

					//invoke the fadeInComplete event
					OnFadeInComplete.Invoke ();

				} else {

					//the else condition is that we are fading out

					//set the shouldUpdate to false
					shouldUpdate = false;

					//invoke the fadeOutComplete event
					OnFadeOutComplete.Invoke ();

				}

			}

		}
		
	}

	//this function starts a fade in
	private void StartFadeIn(bool useLongFade){

		//set the fade duration
		SetFadeDuration(useLongFade);

		//set shouldUpdate to true
		shouldUpdate = true;

		//enable the fade panel
		fadePanel.SetActive(true);

		//set current fade to fully faded out
		currentFade = fullyFadedOut;

		//set the target fade to fully faded in
		targetFade = fullyFadedIn;

		//set the color alpha to the current fade
		fadePanelColor.a = currentFade;

		//for fading in, make the panel not a raycast target so we can click menu items as it fades in
		fadePanel.GetComponent<Image>().raycastTarget = false;

	}

	//this function starts a fade out
	private void StartFadeOut(){

		//set the fade duration - it should always be short for a fade out
		SetFadeDuration(false);

		//set shouldUpdate to true
		shouldUpdate = true;

		//enable the fade panel
		fadePanel.SetActive(true);

		//set current fade to fully faded in
		currentFade = fullyFadedIn;

		//set the target fade to fully faded out
		targetFade = fullyFadedOut;

		//set the color alpha to the current fade
		fadePanelColor.a = currentFade;

		//for fading out, make the panel a raycast target so we can't click things after we've decided to exit the scene
		fadePanel.GetComponent<Image>().raycastTarget = true;

	}

	//this function sets the screen to black
	private void SetToBlack(){

		//set shouldUpdate to false
		shouldUpdate = false;

		//enable the fade panel
		fadePanel.SetActive(true);

		//set current fade to fully faded out
		currentFade = fullyFadedOut;

		//set the color alpha to the current fade
		fadePanelColor.a = currentFade;

		//set the panel color
		fadePanel.GetComponent<Image> ().color = fadePanelColor;

	}

	//this function adjusts the fade duration
	private void SetFadeDuration(bool useLongFade){

		if (useLongFade == true) {

			fadeDuration = longFadeDuration;

		} else {

			fadeDuration = shortFadeDuration;

		}

	}

	//this function sets the UnityActions
	private void SetActions(){

		boolFadeInAction = (isStartup) => {StartFadeIn (isStartup);};
		blankFadeInAction = () => {StartFadeIn (false);};

	}

	//this function adds event listeners
	private void AddEventListeners(){

		//check the active scene index
		if (SceneManager.GetActiveScene ().buildIndex == 0) {

			//we are in the main menu scene

			//add listener for scene startup
			UIManager.GetComponent<MainMenuManager>().OnSceneStartup.AddListener(boolFadeInAction);

			//add listener for exiting the main menu scene
			UIManager.GetComponent<MainMenuManager>().OnBeginSceneExit.AddListener(StartFadeOut);

		} else if(SceneManager.GetActiveScene ().buildIndex == 1){

			//we are in the main game scene

			//add listener for scene startup
			gameManager.OnSceneStartup.AddListener(blankFadeInAction);

			//add listener for loading start
			gameManager.OnStartLoadingSceneStartup.AddListener(SetToBlack);

			//add listener for exiting the main menu scene
			gameManager.OnBeginSceneExit.AddListener(StartFadeOut);

		}

	}

	//this function removes event listeners
	private void RemoveAllListeners(){

		//check the active scene index
		if (UIManager.GetComponent<MainMenuManager>() != null) {

			//remove listener for scene startup
			UIManager.GetComponent<MainMenuManager>().OnSceneStartup.RemoveListener(boolFadeInAction);

			//remove listener for exiting the main menu scene
			UIManager.GetComponent<MainMenuManager>().OnBeginSceneExit.RemoveListener(StartFadeOut);

		}

		if(gameManager != null){

			//we are in the main game scene

			//remove listener for scene startup
			gameManager.OnSceneStartup.RemoveListener(blankFadeInAction);

			//remove listener for loading start
			gameManager.OnStartLoadingSceneStartup.RemoveListener(SetToBlack);

			//remove listener for exiting the main menu scene
			gameManager.OnBeginSceneExit.RemoveListener(StartFadeOut);

		}

	}

	//this function handles on Destroy
	private void OnDestroy(){

		RemoveAllListeners ();

	}

}
