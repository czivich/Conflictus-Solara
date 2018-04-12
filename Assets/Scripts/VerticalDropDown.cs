using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Events;

public class VerticalDropDown : MonoBehaviour {

	//the UI elements are public so they can be hooked up in the inspector
	//the end turn button is what we want to trigger the opening of our submenu
	public RectTransform triggerToggleRect;

	//we need the action menu panel because we want the drop down menu panel size to be based on it
	public RectTransform actionMenuPanel;

	//this is the panel for our drop down menu
	public RectTransform dropDownPanel;

	//need a variable to hold the content rect transform
	public RectTransform dropDownPanelContent;

	//need a variable to hold the action menu content rect transform
	public RectTransform actionMenuPanelContent;

	//variable to hold the target y scale
	private float targetScaleX;

	//variable to hold the spacing between the main action menu and the dropdown
	private float panelSpacing = 10.0f;

	//int to hold the number of toggles in the dropdown menu
	private int numberToggles;

	//unityActions
	private UnityAction<bool> boolResolveDropdownPanelAction;

	// Use this for initialization
	public void Init () {

		//set actions
		boolResolveDropdownPanelAction = (value) => {ResolveDropDownOpenStatus();};

		//get the number of toggles in the dropdown
		numberToggles = dropDownPanelContent.childCount;

		//set the panel size
		SetPanelSize();

		//add an on-click event listener for the main menu trigger toggle
		triggerToggleRect.GetComponent<Toggle>().onValueChanged.AddListener(boolResolveDropdownPanelAction);

		//start the panel disabled
		dropDownPanel.gameObject.SetActive(false);

	}

	//this function checks whether we should be showing or hiding the dropdown
	private void ResolveDropDownOpenStatus(){

		//Debug.Log ("resolve");

		//check if the trigger is on or off
		if (triggerToggleRect.GetComponent<Toggle> ().isOn == false) {

			//close the panel
			CollapseDropDownPanel ();

		} else {

			//open the panel
			ShowDropDownPanel ();

		}

	}

	//this function will expand and show the dropdown panel
	private void ShowDropDownPanel(){

		//enable the panel
		dropDownPanel.gameObject.SetActive(true);

	}

	//this function will collapse and hide the dropdown panel
	private void CollapseDropDownPanel(){

		//disable the panel
		dropDownPanel.gameObject.SetActive(false);
	}


		
	//this function will set the size of the dropdown based on the screen size
	private void SetPanelSize(){

		//to start, update the canvas so things size appropriately
		//Canvas.ForceUpdateCanvases ();

		//set the drop down panel to start 20 pixels to the right of the action menu panel
		dropDownPanel.anchoredPosition3D = new Vector3 (actionMenuPanel.rect.width + panelSpacing, 0.0f, 0.0f);

		//set content vertical layout group padding and spacing based on actionMenuPanel layout info
		dropDownPanelContent.gameObject.GetComponent<VerticalLayoutGroup>().padding.left = actionMenuPanelContent.gameObject.GetComponent<VerticalLayoutGroup>().padding.left;
		dropDownPanelContent.gameObject.GetComponent<VerticalLayoutGroup>().padding.right = actionMenuPanelContent.gameObject.GetComponent<VerticalLayoutGroup>().padding.right;
		dropDownPanelContent.gameObject.GetComponent<VerticalLayoutGroup>().padding.top = actionMenuPanelContent.gameObject.GetComponent<VerticalLayoutGroup>().padding.top;
		dropDownPanelContent.gameObject.GetComponent<VerticalLayoutGroup>().padding.bottom = actionMenuPanelContent.gameObject.GetComponent<VerticalLayoutGroup>().padding.bottom;
		dropDownPanelContent.gameObject.GetComponent<VerticalLayoutGroup>().spacing = actionMenuPanelContent.gameObject.GetComponent<VerticalLayoutGroup>().spacing;

		//set the size of the drop down
		//width is same as action menu panel
		//height is bottom padding + top padding + spacing + 2X button height
		dropDownPanel.sizeDelta = new Vector2 (actionMenuPanel.rect.width - panelSpacing, (actionMenuPanelContent.gameObject.GetComponent<VerticalLayoutGroup>().padding.bottom) + (numberToggles * triggerToggleRect.rect.height) + ((numberToggles -1 ) * actionMenuPanelContent.gameObject.GetComponent<VerticalLayoutGroup>().spacing) + (actionMenuPanelContent.gameObject.GetComponent<VerticalLayoutGroup>().padding.top));

		//Canvas.ForceUpdateCanvases ();

	}

	//this function handles OnDestroy
	private void OnDestroy(){

		RemoveAllListeners ();

	}

	//this function removes all listeners
	private void RemoveAllListeners(){

		if (triggerToggleRect != null) {

			//remove an on-click event listener for the main menu trigger toggle
			triggerToggleRect.GetComponentInParent<Toggle> ().onValueChanged.RemoveListener (boolResolveDropdownPanelAction);

		}

	}

}
