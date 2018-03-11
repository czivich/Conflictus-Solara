using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class PlayerStatus : MonoBehaviour {

	//managers
	private GameManager gameManager;

	//variables to hold the player status textMeshPro objects
	public TextMeshProUGUI playerMoneyTitle;
	public TextMeshProUGUI playerMoneyValue;
	public TextMeshProUGUI playerPlanetsTitle;
	public TextMeshProUGUI playerPlanetsValue;
	public TextMeshProUGUI playerName;

	//unityActions
	private UnityAction<Player> playerSetPlayerStatusForNewTurnAction;
	private UnityAction<Player,int> playerUpdateMoneyValueAction;


	// Use this for initialization
	public void Init () {

		//get the managers
		gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

		//set the actions
		playerSetPlayerStatusForNewTurnAction = (player) => {SetPlayerStatusForNewTurn(player.color);};
		playerUpdateMoneyValueAction = (player,valueChange) => {UpdateMoneyValue();};

		//add listener for the end turn event
		gameManager.OnNewTurn.AddListener(playerSetPlayerStatusForNewTurnAction);
		gameManager.OnLoadedTurn.AddListener(playerSetPlayerStatusForNewTurnAction);

		//add listener for player money changing
		Player.OnPlayerMoneyChange.AddListener(playerUpdateMoneyValueAction);


	}

	//this function updates the player status for a new player when it becomes that player's turn
	private void SetPlayerStatusForNewTurn(Player.Color playerColor){

		//update the color of the text
		UpdateTextColor(playerColor);

		//update the money value
		UpdateMoneyValue();

		//update the planet value
		UpdatePlanetValue();

		//update the player name
		UpdatePlayerName();

	}

	//this function updates the text colors
	private void UpdateTextColor(Player.Color playerColor){

		//set the text color based on the player color
		switch (playerColor) {

		case Player.Color.Green:

			playerMoneyTitle.color = GameManager.greenColor;
			playerMoneyValue.color = GameManager.greenColor;
			playerPlanetsTitle.color = GameManager.greenColor;
			playerPlanetsValue.color = GameManager.greenColor;
			playerName.color = GameManager.greenColor;
			break;

		case Player.Color.Purple:

			playerMoneyTitle.color = GameManager.purpleColor;
			playerMoneyValue.color = GameManager.purpleColor;
			playerPlanetsTitle.color = GameManager.purpleColor;
			playerPlanetsValue.color = GameManager.purpleColor;
			playerName.color = GameManager.purpleColor;
			break;

		case Player.Color.Red:

			playerMoneyTitle.color = GameManager.redColor;
			playerMoneyValue.color = GameManager.redColor;
			playerPlanetsTitle.color = GameManager.redColor;
			playerPlanetsValue.color = GameManager.redColor;
			playerName.color = GameManager.redColor;
			break;

		case Player.Color.Blue:

			playerMoneyTitle.color = GameManager.blueColor;
			playerMoneyValue.color = GameManager.blueColor;
			playerPlanetsTitle.color = GameManager.blueColor;
			playerPlanetsValue.color = GameManager.blueColor;
			playerName.color = GameManager.blueColor;
			break;

		default:

			playerMoneyTitle.color = Color.white;
			playerMoneyValue.color = Color.white;
			playerPlanetsTitle.color = Color.white;
			playerPlanetsValue.color = Color.white;
			playerName.color = Color.white;
			break;

		}

	}

	//this function will update the money value to match the current player's balance
	private void UpdateMoneyValue(){

		//Debug.Log ("update money");

		//get the current player's money and convert to string
		playerMoneyValue.text = gameManager.currentTurnPlayer.playerMoney.ToString ();

	}

	//this function will update the planet value to match the current player's balance
	private void UpdatePlanetValue(){

		//get the current player's money and convert to string
		playerPlanetsValue.text = gameManager.currentTurnPlayer.playerPlanets.ToString ();

	}

	//this function will update the name display to the current player name
	private void UpdatePlayerName(){

		//get the current player's money and convert to string
		playerName.text = gameManager.currentTurnPlayer.playerName;

	}

	//function to handle OnDestroy
	private void OnDestroy(){

		RemoveAllListeners ();

	}

	//this function removes all listeners
	private void RemoveAllListeners(){

		if (gameManager != null) {
			
			//remove listener for the end turn event
			gameManager.OnNewTurn.RemoveListener (playerSetPlayerStatusForNewTurnAction);
			gameManager.OnLoadedTurn.RemoveListener (playerSetPlayerStatusForNewTurnAction);

		}

		//remove listener for player money changing
		Player.OnPlayerMoneyChange.RemoveListener (playerUpdateMoneyValueAction);

	}

}
