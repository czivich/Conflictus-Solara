using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;


public class LANGameList : MonoBehaviour {

    //manager
    private GameObject uiManager;

    //variable to hold the panel
    public GameObject gameListPanel;

    //variable to hold the game list item parent
    public GameObject gameListItemParent;

    //variable for the join lan game button
    public Button joinLANGameButton;

    //variable to hold the game prefab
    public GameListItem prefabGameListItem;

    //events for opening and closing the panel
    public UnityEvent OnOpenPanel = new UnityEvent();
    public UnityEvent OnClosePanel = new UnityEvent();

	// Use this for initialization
	public void Init () {
        
        //get the manager
        uiManager = GameObject.FindGameObjectWithTag("UIManager");

        //add event listeners
        AddEventListeners();
    }
	
    //this function adds event listeners
    private void AddEventListeners()
    {
        //add listener for clicking the join button
        joinLANGameButton.onClick.AddListener(OpenWindow);
    }

    //this function opens the panel
    private void OpenWindow()
    {
        //enable the panel
        gameListPanel.SetActive(true);

        //invoke the event
        OnOpenPanel.Invoke();

    }

    //this function closes the panel
    private void CloseWindow()
    {
        //disable the panel
        gameListPanel.SetActive(false);

        //invoke the event
        OnClosePanel.Invoke();

    }

    //this function creates the gameListItems based on discovered LAN games
    private void CreateGameListItem(LANConnectionInfo connectionInfo, GameListItemGameData lanGameData)
    {
        //instantiate a game list item
        GameListItem newGameListItem = (GameListItem)Instantiate(prefabGameListItem);

        //run the new item Init();
        newGameListItem.Init(connectionInfo, lanGameData);

    }

    //this function handles onDestroy
    private void OnDestroy()
    {
        RemoveEventListeners();
    }
    
    //this function removes event listeners
    private void RemoveEventListeners()
    {
        if(joinLANGameButton != null)
        {
            //remove listener for clicking the join game button
            joinLANGameButton.onClick.RemoveListener(OpenWindow);
        }
    }
}

