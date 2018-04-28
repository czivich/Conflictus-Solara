﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System;

public class UIManager : MonoBehaviour {

	//variable for the canvas
	public Canvas canvas;

	//this array holds all the inactive UI elements - hook them up in the inspector
	public GameObject[] inactiveUIObjects;

	//variable for the default main camera screen width
	public readonly float defaultMainCameraWidth = .75f;

	//object to hold the panel
	public GameObject uiAlertPanel;

	//variable for alert text string
	public string uiAlertPanelText {get; private set;}

	//unityActions
	private UnityAction<CombatUnit,CombatUnit,CombatManager.CrystalType,int> crystalHealShipPhasorSectionAction;
	private UnityAction<CombatUnit,CombatUnit,CombatManager.CrystalType,int> crystalHealShipTorpedoSectionAction;
	private UnityAction<CombatUnit,CombatUnit,CombatManager.CrystalType,int> crystalHealShipStorageSectionAction;
	private UnityAction<CombatUnit,CombatUnit,CombatManager.CrystalType,int> crystalHealShipCrewSectionAction;
	private UnityAction<CombatUnit,CombatUnit,CombatManager.CrystalType,int> crystalHealShipEngineSectionAction;

	private UnityAction<CombatUnit,CombatUnit,CombatManager.CrystalType,int> crystalHealBasePhasorSection1Action;
	private UnityAction<CombatUnit,CombatUnit,CombatManager.CrystalType,int> crystalHealBasePhasorSection2Action;
	private UnityAction<CombatUnit,CombatUnit,CombatManager.CrystalType,int> crystalHealBaseTorpedoSectionAction;
	private UnityAction<CombatUnit,CombatUnit,CombatManager.CrystalType,int> crystalHealBaseCrewSectionAction;
	private UnityAction<CombatUnit,CombatUnit,CombatManager.CrystalType,int> crystalHealBaseStorageSection1Action;
	private UnityAction<CombatUnit,CombatUnit,CombatManager.CrystalType,int> crystalHealBaseStorageSection2Action;

	private UnityAction<CombatUnit,CombatUnit> repairCrewShipPhasorSectionAction;
	private UnityAction<CombatUnit,CombatUnit> repairCrewShipTorpedoSectionAction;
	private UnityAction<CombatUnit,CombatUnit> repairCrewShipStorageSectionAction;
	private UnityAction<CombatUnit,CombatUnit> repairCrewShipCrewSectionAction;
	private UnityAction<CombatUnit,CombatUnit> repairCrewShipEngineSectionAction;

	private UnityAction<CombatUnit,CombatUnit> repairCrewBasePhasorSection1Action;
	private UnityAction<CombatUnit,CombatUnit> repairCrewBasePhasorSection2Action;
	private UnityAction<CombatUnit,CombatUnit> repairCrewBaseTorpedoSectionAction;
	private UnityAction<CombatUnit,CombatUnit> repairCrewBaseCrewSectionAction;
	private UnityAction<CombatUnit,CombatUnit> repairCrewBaseStorageSection1Action;
	private UnityAction<CombatUnit,CombatUnit> repairCrewBaseStorageSection2Action;

	//this list is all of the action modes that we want to lock out the main action buttons/toggles in
	public static List<GameManager.ActionMode> lockMenuActionModes = new List<GameManager.ActionMode>() {

		GameManager.ActionMode.FlareMode,
		GameManager.ActionMode.Rename,
		GameManager.ActionMode.EndTurn,
		GameManager.ActionMode.Animation,
		GameManager.ActionMode.PlaceNewUnit,

	};

	//use this for initialization
	public void Init(){
		
		//disable the pauseFadePanels
		foreach(GameObject go in GameObject.FindGameObjectsWithTag("PauseFadePanel")){
			
			go.SetActive(false);

		}

		//autosize the text
		//AutoSizeAllText ();

		//make the alert panel inactive
		uiAlertPanel.SetActive(false);

		//initialize the alert panel
		uiAlertPanel.GetComponent<UIAlertPanel>().Init();

		//set the unity actions
		SetUnityActions();

		//add event listeners
		AddEventListeners();

	}

	//this function sets the unityActions
	private void SetUnityActions(){

		crystalHealShipPhasorSectionAction = (selectedUnit, targetedUnit, crystalType, shieldsHealed) => {
			SetAlertString (shieldsHealed + " Shields Restored to Phasor Section!");
			uiAlertPanel.SetActive(false);
			uiAlertPanel.SetActive(true);

		};

		crystalHealShipTorpedoSectionAction = (selectedUnit, targetedUnit, crystalType, shieldsHealed) => {
			SetAlertString (shieldsHealed + " Shields Restored to Torpedo Section!");
			uiAlertPanel.SetActive(false);
			uiAlertPanel.SetActive(true);

		};

		crystalHealShipStorageSectionAction = (selectedUnit, targetedUnit, crystalType, shieldsHealed) => {
			SetAlertString (shieldsHealed + " Shields Restored to Storage Section!");
			uiAlertPanel.SetActive(false);
			uiAlertPanel.SetActive(true);

		};

		crystalHealShipCrewSectionAction = (selectedUnit, targetedUnit, crystalType, shieldsHealed) => {
			SetAlertString (shieldsHealed + " Shields Restored to Crew Section!");
			uiAlertPanel.SetActive(false);
			uiAlertPanel.SetActive(true);

		};

		crystalHealShipEngineSectionAction = (selectedUnit, targetedUnit, crystalType, shieldsHealed) => {
			SetAlertString (shieldsHealed + " Shields Restored to Engine Section!");
			uiAlertPanel.SetActive(false);
			uiAlertPanel.SetActive(true);

		};



		crystalHealBasePhasorSection1Action = (selectedUnit, targetedUnit, crystalType, shieldsHealed) => {
			SetAlertString (shieldsHealed + " Shields Restored to Phasor Section 1!");
			uiAlertPanel.SetActive(false);
			uiAlertPanel.SetActive(true);

		};

		crystalHealBasePhasorSection2Action = (selectedUnit, targetedUnit, crystalType, shieldsHealed) => {
			SetAlertString (shieldsHealed + " Shields Restored to Phasor Section 2!");
			uiAlertPanel.SetActive(false);
			uiAlertPanel.SetActive(true);

		};

		crystalHealBaseTorpedoSectionAction = (selectedUnit, targetedUnit, crystalType, shieldsHealed) => {
			SetAlertString (shieldsHealed + " Shields Restored to Torpedo Section!");
			uiAlertPanel.SetActive(false);
			uiAlertPanel.SetActive(true);

		};

		crystalHealBaseCrewSectionAction = (selectedUnit, targetedUnit, crystalType, shieldsHealed) => {
			SetAlertString (shieldsHealed + " Shields Restored to Crew Section!");
			uiAlertPanel.SetActive(false);
			uiAlertPanel.SetActive(true);

		};

		crystalHealBaseStorageSection1Action = (selectedUnit, targetedUnit, crystalType, shieldsHealed) => {
			SetAlertString (shieldsHealed + " Shields Restored to Storage Section 1!");
			uiAlertPanel.SetActive(false);
			uiAlertPanel.SetActive(true);

		};

		crystalHealBaseStorageSection2Action = (selectedUnit, targetedUnit, crystalType, shieldsHealed) => {
			SetAlertString (shieldsHealed + " Shields Restored to Storage Section 2!");
			uiAlertPanel.SetActive(false);
			uiAlertPanel.SetActive(true);

		};


		repairCrewShipPhasorSectionAction = (selectedUnit, targetedUnit) => {
			SetAlertString ("Phasor Section Repaired!");
			uiAlertPanel.SetActive(false);
			uiAlertPanel.SetActive(true);

		};

		repairCrewShipTorpedoSectionAction = (selectedUnit, targetedUnit) => {
			SetAlertString ("Torpedo Section Repaired!");
			uiAlertPanel.SetActive(false);
			uiAlertPanel.SetActive(true);

		};

		repairCrewShipStorageSectionAction = (selectedUnit, targetedUnit) => {
			SetAlertString ("Storage Section Repaired!");
			uiAlertPanel.SetActive(false);
			uiAlertPanel.SetActive(true);

		};

		repairCrewShipCrewSectionAction = (selectedUnit, targetedUnit) => {
			SetAlertString ("Crew Section Repaired!");
			uiAlertPanel.SetActive(false);
			uiAlertPanel.SetActive(true);

		};

		repairCrewShipEngineSectionAction = (selectedUnit, targetedUnit) => {
			SetAlertString ("Engine Section Repaired!");
			uiAlertPanel.SetActive(false);
			uiAlertPanel.SetActive(true);

		};



		repairCrewBasePhasorSection1Action = (selectedUnit, targetedUnit) => {
			SetAlertString ("Phasor Section 1 Repaired!");
			uiAlertPanel.SetActive(false);
			uiAlertPanel.SetActive(true);

		};

		repairCrewBasePhasorSection2Action = (selectedUnit, targetedUnit) => {
			SetAlertString ("Phasor Section 2 Repaired!");
			uiAlertPanel.SetActive(false);
			uiAlertPanel.SetActive(true);

		};

		repairCrewBaseTorpedoSectionAction = (selectedUnit, targetedUnit) => {
			SetAlertString ("Torpedo Section Repaired!");
			uiAlertPanel.SetActive(false);
			uiAlertPanel.SetActive(true);

		};

		repairCrewBaseCrewSectionAction = (selectedUnit, targetedUnit) => {
			SetAlertString ("Crew Section Repaired!");
			uiAlertPanel.SetActive(false);
			uiAlertPanel.SetActive(true);

		};

		repairCrewBaseStorageSection1Action = (selectedUnit, targetedUnit) => {
			SetAlertString ("Storage Section 1 Repaired!");
			uiAlertPanel.SetActive(false);
			uiAlertPanel.SetActive(true);

		};

		repairCrewBaseStorageSection2Action = (selectedUnit, targetedUnit) => {
			SetAlertString ("Storage Section 2 Repaired!");
			uiAlertPanel.SetActive(false);
			uiAlertPanel.SetActive(true);

		};

	}

	//this function adds event listeners
	private void AddEventListeners(){

		//add listeners for using crystals to heal ships
		CombatManager.OnCrystalUsedOnShipPhasorSection.AddListener(crystalHealShipPhasorSectionAction);
		CombatManager.OnCrystalUsedOnShipTorpedoSection.AddListener(crystalHealShipTorpedoSectionAction);
		CombatManager.OnCrystalUsedOnShipStorageSection.AddListener(crystalHealShipStorageSectionAction);
		CombatManager.OnCrystalUsedOnShipCrewSection.AddListener(crystalHealShipCrewSectionAction);
		CombatManager.OnCrystalUsedOnShipEngineSection.AddListener(crystalHealShipEngineSectionAction);

		//add listeners for using crystals to heal bases
		CombatManager.OnCrystalUsedOnBasePhasorSection1.AddListener(crystalHealBasePhasorSection1Action);
		CombatManager.OnCrystalUsedOnBasePhasorSection2.AddListener(crystalHealBasePhasorSection2Action);
		CombatManager.OnCrystalUsedOnBaseTorpedoSection.AddListener(crystalHealBaseTorpedoSectionAction);
		CombatManager.OnCrystalUsedOnBaseCrewSection.AddListener(crystalHealBaseCrewSectionAction);
		CombatManager.OnCrystalUsedOnBaseStorageSection1.AddListener(crystalHealBaseStorageSection1Action);
		CombatManager.OnCrystalUsedOnBaseStorageSection2.AddListener(crystalHealBaseStorageSection2Action);

		//add listeners for using repair crews on ships
		CombatManager.OnRepairCrewUsedOnShipPhasorSection.AddListener(repairCrewShipPhasorSectionAction);
		CombatManager.OnRepairCrewUsedOnShipTorpedoSection.AddListener(repairCrewShipTorpedoSectionAction);
		CombatManager.OnRepairCrewUsedOnShipStorageSection.AddListener(repairCrewShipStorageSectionAction);
		CombatManager.OnRepairCrewUsedOnShipCrewSection.AddListener(repairCrewShipCrewSectionAction);
		CombatManager.OnRepairCrewUsedOnShipEngineSection.AddListener(repairCrewShipEngineSectionAction);

		//add listeners for using repair crews on bases
		CombatManager.OnRepairCrewUsedOnBasePhasorSection1.AddListener(repairCrewBasePhasorSection1Action);
		CombatManager.OnRepairCrewUsedOnBasePhasorSection2.AddListener(repairCrewBasePhasorSection2Action);
		CombatManager.OnRepairCrewUsedOnBaseTorpedoSection.AddListener(repairCrewBaseTorpedoSectionAction);
		CombatManager.OnRepairCrewUsedOnBaseCrewSection.AddListener(repairCrewBaseCrewSectionAction);
		CombatManager.OnRepairCrewUsedOnBaseStorageSection1.AddListener(repairCrewBaseStorageSection1Action);
		CombatManager.OnRepairCrewUsedOnBaseStorageSection2.AddListener(repairCrewBaseStorageSection2Action);

	}
		
	//this function sets the text sizes for the current resolution
	private void AutoSizeAllText(){

		//iterate through each inactive UI element, activating them
		for (int i = 0; i < inactiveUIObjects.Length; i++) {

			//activate the object
			inactiveUIObjects[i].SetActive(true);

			//rebuild the layout immediately so that the sizing updates
			LayoutRebuilder.ForceRebuildLayoutImmediate(inactiveUIObjects[i].GetComponent<RectTransform>());

		}

		//force the canvas to update so it resizes itself before the end of the frame
		Canvas.ForceUpdateCanvases ();

		//get all the textMeshPro UI texts in the canvas
		foreach (TextMeshProUGUI textElement in canvas.gameObject.GetComponentsInChildren<TextMeshProUGUI>(true)){

			//enable auto-size
			textElement.enableAutoSizing = true;

			//force the mesh to update
			textElement.ForceMeshUpdate(false);

			//cache the font size
			float textSize = textElement.fontSize;

			//disable auto-size
			textElement.enableAutoSizing = false;

			//set the text size to the auto-size value
			textElement.fontSize = textSize;

		}

		//iterate through the inactive UI elements and set them inactive again
		for (int i = 0; i < inactiveUIObjects.Length; i++) {

			//activate the object
			inactiveUIObjects[i].SetActive(false);

		}

	}

	//this static function will autoSize a single textmeshpro element
	public static void AutoSizeTextMeshFont(TextMeshProUGUI textElement){

		//enable auto-size
		textElement.enableAutoSizing = true;

		//force the mesh to update
		textElement.ForceMeshUpdate(false);

		//cache the font size
		float textSize = textElement.fontSize;

		//disable auto-size
		textElement.enableAutoSizing = false;

		//set the text size to the auto-size value
		textElement.fontSize = textSize;

	}

	//this function sets the alert string
	private void SetAlertString(string newString){

		uiAlertPanelText = newString;

	}

	//this function handles onDestroy
	private void OnDestroy(){

		RemoveEventListeners ();

	}

	//this function removes event listeners
	private void RemoveEventListeners(){

		//remove listeners for using crystals to heal ships
		CombatManager.OnCrystalUsedOnShipPhasorSection.RemoveListener(crystalHealShipPhasorSectionAction);
		CombatManager.OnCrystalUsedOnShipTorpedoSection.RemoveListener(crystalHealShipTorpedoSectionAction);
		CombatManager.OnCrystalUsedOnShipStorageSection.RemoveListener(crystalHealShipStorageSectionAction);
		CombatManager.OnCrystalUsedOnShipCrewSection.RemoveListener(crystalHealShipCrewSectionAction);
		CombatManager.OnCrystalUsedOnShipEngineSection.RemoveListener(crystalHealShipEngineSectionAction);

		//remove listeners for using crystals to heal bases
		CombatManager.OnCrystalUsedOnBasePhasorSection1.RemoveListener(crystalHealBasePhasorSection1Action);
		CombatManager.OnCrystalUsedOnBasePhasorSection2.RemoveListener(crystalHealBasePhasorSection2Action);
		CombatManager.OnCrystalUsedOnBaseTorpedoSection.RemoveListener(crystalHealBaseTorpedoSectionAction);
		CombatManager.OnCrystalUsedOnBaseCrewSection.RemoveListener(crystalHealBaseCrewSectionAction);
		CombatManager.OnCrystalUsedOnBaseStorageSection1.RemoveListener(crystalHealBaseStorageSection1Action);
		CombatManager.OnCrystalUsedOnBaseStorageSection2.RemoveListener(crystalHealBaseStorageSection2Action);

		//remove listeners for using repair crews on ships
		CombatManager.OnRepairCrewUsedOnShipPhasorSection.RemoveListener(repairCrewShipPhasorSectionAction);
		CombatManager.OnRepairCrewUsedOnShipTorpedoSection.RemoveListener(repairCrewShipTorpedoSectionAction);
		CombatManager.OnRepairCrewUsedOnShipStorageSection.RemoveListener(repairCrewShipStorageSectionAction);
		CombatManager.OnRepairCrewUsedOnShipCrewSection.RemoveListener(repairCrewShipCrewSectionAction);
		CombatManager.OnRepairCrewUsedOnShipEngineSection.RemoveListener(repairCrewShipEngineSectionAction);

		//remove listeners for using repair crews on bases
		CombatManager.OnRepairCrewUsedOnBasePhasorSection1.RemoveListener(repairCrewBasePhasorSection1Action);
		CombatManager.OnRepairCrewUsedOnBasePhasorSection2.RemoveListener(repairCrewBasePhasorSection2Action);
		CombatManager.OnRepairCrewUsedOnBaseTorpedoSection.RemoveListener(repairCrewBaseTorpedoSectionAction);
		CombatManager.OnRepairCrewUsedOnBaseCrewSection.RemoveListener(repairCrewBaseCrewSectionAction);
		CombatManager.OnRepairCrewUsedOnBaseStorageSection1.RemoveListener(repairCrewBaseStorageSection1Action);
		CombatManager.OnRepairCrewUsedOnBaseStorageSection2.RemoveListener(repairCrewBaseStorageSection2Action);

	}						
}
