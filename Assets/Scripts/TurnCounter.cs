using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class TurnCounter : MonoBehaviour {

	//we will need to access the game manager
	private GameManager gameManager;

	//the UI elements are public so they can be hooked up in the inspector
	//variable for the turn counter turn phase text element
	public TextMeshProUGUI turnPhaseText;

	//variable for the turn color text element
	public TextMeshProUGUI turnColorText;

	//variable for the game year text element
	public TextMeshProUGUI gameYearText;

	//variable for the panel background image
	public Image turnCounterBackgroundImage;

	//variable to control the alpha of the turn counter background color
	private float turnCounterAlpha = 255.0f;

	//variables to hold the turn counter colors
	private Color greenColor;
	private Color purpleColor;
	private Color redColor;
	private Color blueColor;

	//unityActions
	private UnityAction<Player> playerUpdateTurnCounterAction;

	// Use this for initialization
	public void Init () {

		//set the actions
		playerUpdateTurnCounterAction = (player) => {UpdateTurnCounter();};

		//get the manager
		gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

		//set the values of the colors
		greenColor = new Color (131.0f / 255.0f, 214.0f / 255.0f, 131.0f / 255.0f, turnCounterAlpha / 255.0f);
		purpleColor = new Color (180.0f / 255.0f, 147.0f / 255.0f, 214.0f / 255.0f, turnCounterAlpha / 255.0f);
		redColor = new Color (214.0f / 255.0f, 130.0f / 255.0f, 130.0f / 255.0f, turnCounterAlpha / 255.0f);
		blueColor = new Color (130.0f / 255.0f, 172.0f / 255.0f, 214.0f / 255.0f, turnCounterAlpha / 255.0f);

		//add listeners to the gameManager turn events
		gameManager.OnNewTurn.AddListener(playerUpdateTurnCounterAction);
		gameManager.OnBeginMainPhase.AddListener(playerUpdateTurnCounterAction);

		//add listener for a loaded turn
		gameManager.OnLoadedTurn.AddListener(playerUpdateTurnCounterAction);


	}

	//this function will update the turn counter text with the current game state info
	private void UpdateTurnCounter(){

		//Debug.Log ("UpdateTurnCounter");

		//update the turn phase text
		if (gameManager.currentTurnPhase == GameManager.TurnPhase.MainPhase) {

			turnPhaseText.text = "Main Phase";

		} else if (gameManager.currentTurnPhase == GameManager.TurnPhase.PurchasePhase) {

			turnPhaseText.text = "Purchase Phase";

		}

		//update the turn color text
		if (gameManager.currentTurn == Player.Color.Green) {

			turnColorText.text = "Green Turn";
			turnCounterBackgroundImage.color = greenColor;
		} 

		else if(gameManager.currentTurn == Player.Color.Purple) {

			turnColorText.text = "Purple Turn";
			turnCounterBackgroundImage.color = purpleColor;

		} 

		else if(gameManager.currentTurn == Player.Color.Red) {

			turnColorText.text = "Red Turn";
			turnCounterBackgroundImage.color = redColor;

		} 

		else if(gameManager.currentTurn == Player.Color.Blue) {

			turnColorText.text = "Blue Turn";
			turnCounterBackgroundImage.color = blueColor;

		} 

		//update the game year
		gameYearText.text = gameManager.gameYear + " AD";

	}

	//this function handles OnDestroy
	private void OnDestroy(){

		RemoveAllListeners ();

	}

	//this function removes all listeners
	private void RemoveAllListeners(){

		if (gameManager != null) {
			
			//remove listeners to the gameManager turn events
			gameManager.OnNewTurn.RemoveListener (playerUpdateTurnCounterAction);
			gameManager.OnBeginMainPhase.RemoveListener (playerUpdateTurnCounterAction);

			//remove listener for a loaded turn
			gameManager.OnLoadedTurn.RemoveListener (playerUpdateTurnCounterAction);

		}

	}

}
