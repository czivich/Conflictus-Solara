using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerConnection : NetworkBehaviour {

    //manager
    private GameManager gameManager;

    //this variable is for what player this connection is controlling
    private Player playerControlled;

	// Use this for initialization
	public void Init () {

        //get the gameManager
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        playerControlled = gameManager.greenPlayer;

        this.name = "greenPlayerConnection";
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
