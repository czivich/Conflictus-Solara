using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerConnection : NetworkBehaviour {

    //manager
    //private GameManager gameManager;

    //this variable is for what player this connection is controlling
    //private Player playerControlled;

    //bools to track if the players are locally controlled
    public bool localGreenPlayer { get; private set; }
    public bool localRedPlayer { get; private set; }
    public bool localPurplePlayer { get; private set; }
    public bool localBluePlayer { get; private set; }


    // Use this for initialization
    public void Init () {

        //get the gameManager
        //gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        //playerControlled = gameManager.greenPlayer;

        //this.name = "greenPlayerConnection";
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
