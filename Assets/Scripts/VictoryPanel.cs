using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class VictoryPanel : MonoBehaviour {

	//managers
	private GameManager gameManager;

	//variable to hold the panel
	public GameObject victoryPanel;

	//variable to hold the text
	public TextMeshProUGUI victoryText;

	//variable to hold the buttons
	public Button continuePlayingButton;
	public Button exitGameButton;

	//events for opening and closing the panel
	public UnityEvent OnOpenVictoryPanel = new UnityEvent();
	public UnityEvent OnCloseVictoryPanel = new UnityEvent();

	//events for clicking the buttons
	public UnityEvent OnContinuePlaying = new UnityEvent();
	public UnityEvent OnExitGame = new UnityEvent();

	//unityActions
	private UnityAction<Player,Player> teamWonGameAction;
	private UnityAction<Player> soloWonGameAction;

	// Use this for initialization
	public void Init () {

		//get the managers
		gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

		//set actions
		SetUnityActions();

		//add event listeners
		AddEventListeners();

	}

	//this function sets unityActions
	private void SetUnityActions(){

		teamWonGameAction = (firstWinningPlayer,secondWinningPlayer) => {
			SetVictoryTextForTeam(firstWinningPlayer,secondWinningPlayer);
			OpenPanel();
		};

		soloWonGameAction = (winningPlayer) => {
			SetVictoryTextForSolo(winningPlayer);
			OpenPanel();
		};

	}

	//this function adds event listeners
	private void AddEventListeners(){

		//add listener for team winning
		gameManager.OnTeamVictory.AddListener(teamWonGameAction);

		//add listener for solo winning
		gameManager.OnSoloVictory.AddListener(soloWonGameAction);

		//add listener for clicking continue playing button
		continuePlayingButton.onClick.AddListener(ResolveContinuePlayingClick);

		//add listener for clicking exit game button
		exitGameButton.onClick.AddListener(ResolveExitGameClick);

	}

	//this function opens the victory panel
	private void OpenPanel(){

		//enable the panel
		victoryPanel.SetActive(true);

		//invoke the event
		OnOpenVictoryPanel.Invoke();

	}

	//this function closes the victory panel
	private void ClosePanel(){

		//disable the panel
		victoryPanel.SetActive(false);

		//invoke the event
		OnCloseVictoryPanel.Invoke();

	}

	//this function resolves a continuePlaying click
	private void ResolveContinuePlayingClick(){

		//close the panel
		ClosePanel();

		//invoke the event
		OnContinuePlaying.Invoke();

	}

	//this function resolves a exit game click
	private void ResolveExitGameClick(){

		//close the panel
		ClosePanel();

		//invoke the event
		OnExitGame.Invoke();

	}

	//this function sets the victory text for a team
	private void SetVictoryTextForTeam(Player firstWinningPlayer, Player secondWinningPlayer){

		victoryText.text = "The Battle for the Solar System has been decided!  The galactic alliance between the " +
		firstWinningPlayer.color.ToString () + " forces controlled by " + firstWinningPlayer.playerName.ToString () +
		" and the " + secondWinningPlayer.color.ToString () + " forces controlled by " + secondWinningPlayer.playerName.ToString () +
		" have emerged victorious in the year " + gameManager.gameYear.ToString () +
		" and have united the solar system.  May the planets enjoy a new era of peace and prosperity under their glorious rule!";

	}

	//this function sets the victory text for a single player
	private void SetVictoryTextForSolo(Player winningPlayer){

		victoryText.text = "The Battle for the Solar System has been decided!  The intrepid " + winningPlayer.color.ToString() + " forces controlled by " 
			+ winningPlayer.playerName.ToString () + " have emerged victorious in the year " + gameManager.gameYear.ToString () +
			" and have united the solar system.  May the planets enjoy a new era of peace and prosperity under the  glorious rule of " + 
			winningPlayer.playerName.ToString () + "!";

	}
	
	//this function handles onDestroy
	private void OnDestroy(){

		RemoveEventListeners ();

	}

	//this function removes event listeners
	private void RemoveEventListeners(){

		if (gameManager != null) {
			
			//remove listener for team winning
			gameManager.OnTeamVictory.RemoveListener (teamWonGameAction);

			//remove listener for solo winning
			gameManager.OnSoloVictory.RemoveListener (soloWonGameAction);

		}

		if (continuePlayingButton != null) {

			//remove listener for clicking continue playing button
			continuePlayingButton.onClick.RemoveListener (ResolveContinuePlayingClick);

		}

		if (exitGameButton != null) {

			//remove listener for clicking exit game button
			exitGameButton.onClick.RemoveListener (ResolveExitGameClick);

		}

	}

}
