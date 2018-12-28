using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CustomNetworkMigrationManager : NetworkMigrationManager {

	// Use this for initialization
	public void Init () {
		
	}

    //override the callback for disconnecting from host as client - this will allow us to implement custom host migration
    protected override void OnClientDisconnectedFromHost(NetworkConnection conn, out SceneChangeOption sceneChange)
    {
        //call the base fuction
        base.OnClientDisconnectedFromHost(conn, out sceneChange);


    }
}
