using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;


public class LANGameList : MonoBehaviour {

    //manager
    private GameObject uiManager;
    private GameObject networkManager;

    //variable to hold the panel
    public GameObject gameListPanel;

    //variable to hold the game list item parent
    public GameObject gameListItemParent;

    //variable for the join lan game button
    public Button joinLANGameButton;

    //variable for the close button and cancel button
    public Button closeButton;
    public Button cancelButton;

    //variable to hold the game prefab
    public GameListItem prefabGameListItem;

    //events for opening and closing the panel
    public UnityEvent OnOpenPanel = new UnityEvent();
    public UnityEvent OnClosePanel = new UnityEvent();

    //event for adding a lan game item
    public UnityEvent OnAddedLANGameListItem = new UnityEvent();

    //unityactions
    private UnityAction<LANConnectionInfo> connectionInfoCreateGameListItemAction;
    private UnityAction<LANConnectionInfo> connectionInfoDeleteGameListItemAction;


    // Use this for initialization
    public void Init () {
        
        //get the manager
        uiManager = GameObject.FindGameObjectWithTag("UIManager");
        networkManager = GameObject.FindGameObjectWithTag("NetworkManager");

        //set actions
        SetActions();

        //add event listeners
        AddEventListeners();
    }

	
    //this function sets the unity actions
    private void SetActions()
    {
        connectionInfoCreateGameListItemAction = (connectionInfo) => { CreateGameListItem(connectionInfo); };
        connectionInfoDeleteGameListItemAction = (connectionInfo) => { DeleteGameListItem(connectionInfo); };

    }

    //this function adds event listeners
    private void AddEventListeners()
    {
        //add listener for clicking the join button
        joinLANGameButton.onClick.AddListener(OpenWindow);

        //add listener for the close and cancel buttons
        closeButton.onClick.AddListener(CloseWindow);
        cancelButton.onClick.AddListener(CloseWindow);

        //add listener for discovering a new network game
        networkManager.GetComponent<LocalNetworkDiscovery>().OnDiscoveredLANGame.AddListener(connectionInfoCreateGameListItemAction);

        //add listener for a network game timing out
        networkManager.GetComponent<LocalNetworkDiscovery>().OnLANGameTimedOut.AddListener(connectionInfoDeleteGameListItemAction);

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
        //clear all objects from the game list
        ClearLANGamesFromList();

        //disable the panel
        gameListPanel.SetActive(false);

        //invoke the event
        OnClosePanel.Invoke();

    }

    //this function creates the gameListItems based on discovered LAN games
    private void CreateGameListItem(LANConnectionInfo connectionInfo)
    {
        //instantiate a game list item
        GameListItem newGameListItem = (GameListItem)Instantiate(prefabGameListItem);

        //run the new item Init();
        newGameListItem.Init(connectionInfo);

        //set the parent object
        newGameListItem.transform.SetParent(gameListItemParent.transform);

        //set the scale
        newGameListItem.transform.localScale = Vector3.one;

        //set the scrollbar to the top
        StartCoroutine(ScrollToTop());

        //invoke the event for having added an item
        OnAddedLANGameListItem.Invoke();

    }

    //this coroutine sets the scrollbar to the top
    private IEnumerator ScrollToTop()
    {
        yield return new WaitForEndOfFrame();
        gameListItemParent.transform.parent.GetComponent<ScrollRect>().gameObject.SetActive(true);
        gameListItemParent.transform.parent.GetComponent<ScrollRect>().verticalNormalizedPosition = 1f;

    }

    //this coroutine sets the scrollbar to the bottom
    private IEnumerator ScrollToBottom()
    {
        yield return new WaitForEndOfFrame();
        gameListItemParent.transform.parent.GetComponent<ScrollRect>().gameObject.SetActive(true);
        gameListItemParent.transform.parent.GetComponent<ScrollRect>().verticalNormalizedPosition = 0f;
    }

    //this function deletes a specific GameListItem
    private void DeleteGameListItem(LANConnectionInfo connectionInfo)
    {
        //we need to find the GameListItem that has the passed connectionInfo signature

        //get the number of gameList children
        int gameItemChildrenCount = gameListItemParent.transform.childCount;

        //loop through the children
        for(int i = 0; i < gameItemChildrenCount; i++)
        {
            //check if the ipAddress and port match what was passed to the function
            if(gameListItemParent.transform.GetChild(i).GetComponent<GameListItem>().lanConnectionInfo.ipAddress == connectionInfo.ipAddress &&
                gameListItemParent.transform.GetChild(i).GetComponent<GameListItem>().lanConnectionInfo.port == connectionInfo.port)
            {

                //if these match, then we can delete the item, and break out of the loop
                Destroy(gameListItemParent.transform.GetChild(i).gameObject);
                break;

            }

        }

    }

    //this function clears all objects out of the lan game list
    private void ClearLANGamesFromList()
    {
        //loop through the children of the parent
        for(int i = 0; i < gameListItemParent.transform.childCount; i++)
        {
            //destroy each object
            Destroy(gameListItemParent.transform.GetChild(i).gameObject);

        }
        
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

        if(closeButton != null)
        {
            //remove listener for the close and cancel buttons
            closeButton.onClick.RemoveListener(CloseWindow);
        }

        if (cancelButton != null)
        {
            //remove listener for the close and cancel buttons
            cancelButton.onClick.RemoveListener(CloseWindow);
        }

        if(networkManager != null)
        {
            //remove listener for discovering a new network game
            networkManager.GetComponent<LocalNetworkDiscovery>().OnDiscoveredLANGame.RemoveListener(connectionInfoCreateGameListItemAction);

            //remove listener for a network game timing out
            networkManager.GetComponent<LocalNetworkDiscovery>().OnLANGameTimedOut.RemoveListener(connectionInfoDeleteGameListItemAction);

        }
    }
}

