using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;

public class UIAlertPanel : MonoBehaviour {

	//variable for the managers
	private GameManager gameManager;
	private UIManager uiManager;

	//variable to hold the alert counter
	private float alertTimer;
	private float alertTimerLimit = 3.0f;

	//Use this for initialization
	public void Init(){

		//get the manager
		gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
		uiManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();

	}

	// Use this for initialization
	private void OnEnable () {

		//set the border color
		SetBorderColor();

		//set the alert text
		SetAlertText();

		//reset the timer
		alertTimer = 0.0f;
		
	}
	
	// Update is called once per frame
	private void Update () {

		//update the timer
		alertTimer += Time.deltaTime;

		//check if the timer has exceeded the limit
		if (alertTimer >= alertTimerLimit) {

			//close the panel
			this.gameObject.SetActive(false);

		}
		
	}

	//this function sets the border color
	private void SetBorderColor(){

		//set the border color to match the current turn player
		switch (gameManager.currentTurn) {

		case Player.Color.Green:

			this.GetComponent<Image> ().color = GameManager.greenColor;

			break;

		case Player.Color.Purple:

			this.GetComponent<Image> ().color = GameManager.purpleColor;

			break;

		case Player.Color.Red:

			this.GetComponent<Image> ().color = GameManager.redColor;

			break;

		case Player.Color.Blue:

			this.GetComponent<Image> ().color = GameManager.blueColor;

			break;

		default:

			break;

		}

	}

	//this function sets the alert text
	private void SetAlertText(){

		//set the text
		this.GetComponentInChildren<TextMeshProUGUI>().text = uiManager.uiAlertPanelText;

	}

}
