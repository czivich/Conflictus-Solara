using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HexLabel : MonoBehaviour
{

    //variable to hold the hex label parent object
    private GameObject hexLabelParent;

    //variable to hold the tileMap
    private TileMap tileMap;

    //variable to hold the UIManager
    private UIManager uiManager;

    //action for listening for label display event
    private UnityAction<bool> boolToggleHexLabelDisplayAction;

    //Use this to initialize the script
    public void Init()
    {
        //get the tileMap
        tileMap = GameObject.FindGameObjectWithTag("TileMap").GetComponent<TileMap>();

        //set the parent object
        hexLabelParent = tileMap.hexLabelParent;

        //get the UIManager
        uiManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
        
        //set the actions
        SetActions();

        //add listeners
        AddListeners();

    }

    //this function sets the actions
    private void SetActions()
    {
        //define hex label display action
        boolToggleHexLabelDisplayAction = (newValue) =>
        {
            //check if the value is true - we are turning on the labels
            if(newValue == true)
            {
                EnableLabels();
            }
            else
            {
                //the else condition is that we are turning off the labels
                DisableLabels();
            }
        };

    }

    //this function adds listeners
    private void AddListeners()
    {
        //add listener for the UIManager label event
        uiManager.GetComponent<Settings>().OnChangeHexLabelDisplay.AddListener(boolToggleHexLabelDisplayAction);
    }


    //this function turns on the labels
    private void EnableLabels()
    {
        hexLabelParent.SetActive(true);
    }

    //this function turns off the labels
    private void DisableLabels()
    {
        hexLabelParent.SetActive(false);
    }

    //this function handles OnDestroy
    private void OnDestroy()
    {
        RemoveListeners();
    }

    //this function removes all listeners
    private void RemoveListeners()
    {
        //remove listener for the UIManager label event
        uiManager.GetComponent<Settings>().OnChangeHexLabelDisplay.RemoveListener(boolToggleHexLabelDisplayAction);
    }


}
