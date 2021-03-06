﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Events;
using TMPro;

public class MessageManager : MonoBehaviour {

	//this class will handle listening to all events that should generate messages and post the message to the log

	//these are the UI elements required - public so they can be hooked up in inspector
	public RectTransform chatContent;
	public TextMeshProUGUI chatTextPrefab;
	public ScrollRect chatLog;

	//define colors for message builders
	//private static string greenPlayerColor = "00ff00ff";
    private static string greenPlayerColor = "008200ff";

    //private static string purplePlayerColor = "9933ffff";
    private static string purplePlayerColor = "6300C8ff";

    //private static string redPlayerColor = "ff0000ff";
    private static string redPlayerColor = "f00000ff";

    //private static string bluePlayerColor = "0080ffff";
    private static string bluePlayerColor = "005BB5ff";

    //variables to hold the managers
    private GameManager gameManager;
	private UIManager uiManager;
	private MouseManager mouseManager;

	//set up an integer to index messages added to the log
	private static int messageLogID;
	public static int MessageLogID {
		get{

			return messageLogID;

		}

		private set{

			messageLogID = value;

		}

	}

	//variable for maximum number of messages to have in messageLog
	private static int maxMessages = 1000;

	//unityActions
	private UnityAction<Player> playerSendTurnPhaseMessageAction;
	private UnityAction<string> stringSendNewChatMessageAction;
	private UnityAction<Ship,Hex,Hex> movementSendNewShipMovementMessageAction;
	private UnityAction<Ship> shipSendTractorBeamPrimedMessageAction;
	private UnityAction<Ship> shipSendTractorBeamEngagedMessageAction;
	private UnityAction<Ship> shipSendTractorBeamDisengagedMessageAction;
	private UnityAction<Ship> shipSendUseWarpBoosterMessageAction;
	private UnityAction<Ship> shipSendUseTranswarpBoosterMessageAction;
	private UnityAction<CombatUnit,CombatUnit,String> firePhasersSendPhaserAttackMessageAction;
	private UnityAction<CombatUnit,CombatUnit,int> phasersSendPhaserAttackHitShipPhaserSectionMessageAction;
	private UnityAction<CombatUnit,CombatUnit,int> phasersSendPhaserAttackHitShipTorpedoSectionMessageAction;
	private UnityAction<CombatUnit,CombatUnit,int> phasersSendPhaserAttackHitShipStorageSectionMessageAction;
	private UnityAction<CombatUnit,CombatUnit,int> phasersSendPhaserAttackHitShipCrewSectionMessageAction;
	private UnityAction<CombatUnit,CombatUnit,int> phasersSendPhaserAttackHitShipEngineSectionMessageAction;
	//private UnityAction<CombatUnit,CombatUnit> phasersSendPhaserAttackMissMessageAction;
	private UnityAction<CombatUnit,CombatUnit,int> phasersSendPhaserAttackHitBasePhaserSection1MessageAction;
	private UnityAction<CombatUnit,CombatUnit,int> phasersSendPhaserAttackHitBasePhaserSection2MessageAction;
	private UnityAction<CombatUnit,CombatUnit,int> phasersSendPhaserAttackHitBaseTorpedoSectionMessageAction;
	private UnityAction<CombatUnit,CombatUnit,int> phasersSendPhaserAttackHitBaseCrewSectionMessageAction;
	private UnityAction<CombatUnit,CombatUnit,int> phasersSendPhaserAttackHitBaseStorageSection1MessageAction;
	private UnityAction<CombatUnit,CombatUnit,int> phasersSendPhaserAttackHitBaseStorageSection2MessageAction;
	private UnityAction<CombatUnit,CombatUnit,String> fireLightTorpedoSendTorpedoAttackMessageAction;
	private UnityAction<CombatUnit,CombatUnit,String> fireHeavyTorpedoSendTorpedoAttackMessageAction;
	private UnityAction<FlareManager.FlareEventData> flareSendUsedFlaresYesMessageAction;
	private UnityAction<FlareManager.FlareEventData> flareSendUsedFlaresNoMessageAction;
	//private UnityAction<CombatUnit,CombatUnit, CombatManager.ShipSectionTargeted,int> flaresSendFlaresDefeatLightTorpedoMessageAction;
	//private UnityAction<CombatUnit,CombatUnit, CombatManager.ShipSectionTargeted,int> flaresSendFlaresDefeatHeavyTorpedoMessageAction;
	private UnityAction<CombatUnit,CombatUnit,int> flaresSendFlaresFailLightTorpedoMessageAction;
	private UnityAction<CombatUnit,CombatUnit,int> flaresSendFlaresFailHeavyTorpedoMessageAction;
	private UnityAction<CombatUnit,CombatUnit,int> torpedoSendLightTorpedoAttackHitShipPhaserSectionMessageAction;
	private UnityAction<CombatUnit,CombatUnit,int> torpedoSendLightTorpedoAttackHitShipTorpedoSectionMessageAction;
	private UnityAction<CombatUnit,CombatUnit,int> torpedoSendLightTorpedoAttackHitShipStorageSectionMessageAction;
	private UnityAction<CombatUnit,CombatUnit,int> torpedoSendLightTorpedoAttackHitShipCrewSectionMessageAction;
	private UnityAction<CombatUnit,CombatUnit,int> torpedoSendLightTorpedoAttackHitShipEngineSectionMessageAction;
	private UnityAction<CombatUnit,CombatUnit,int> torpedoSendHeavyTorpedoAttackHitShipPhaserSectionMessageAction;
	private UnityAction<CombatUnit,CombatUnit,int> torpedoSendHeavyTorpedoAttackHitShipTorpedoSectionMessageAction;
	private UnityAction<CombatUnit,CombatUnit,int> torpedoSendHeavyTorpedoAttackHitShipStorageSectionMessageAction;
	private UnityAction<CombatUnit,CombatUnit,int> torpedoSendHeavyTorpedoAttackHitShipCrewSectionMessageAction;
	private UnityAction<CombatUnit,CombatUnit,int> torpedoSendHeavyTorpedoAttackHitShipEngineSectionMessageAction;
	//private UnityAction<CombatUnit,CombatUnit> torpedoSendLightTorpedoAttackMissMessageAction;
	//private UnityAction<CombatUnit,CombatUnit> torpedoSendHeavyTorpedoAttackMissMessageAction;

	private UnityAction<CombatUnit,CombatUnit,int> torpedoSendLightTorpedoAttackHitBasePhaserSection1MessageAction;
	private UnityAction<CombatUnit,CombatUnit,int> torpedoSendLightTorpedoAttackHitBasePhaserSection2MessageAction;
	private UnityAction<CombatUnit,CombatUnit,int> torpedoSendLightTorpedoAttackHitBaseTorpedoSectionMessageAction;
	private UnityAction<CombatUnit,CombatUnit,int> torpedoSendLightTorpedoAttackHitBaseCrewSectionMessageAction;
	private UnityAction<CombatUnit,CombatUnit,int> torpedoSendLightTorpedoAttackHitBaseStorageSection1MessageAction;
	private UnityAction<CombatUnit,CombatUnit,int> torpedoSendLightTorpedoAttackHitBaseStorageSection2MessageAction;
	private UnityAction<CombatUnit,CombatUnit,int> torpedoSendHeavyTorpedoAttackHitBasePhaserSection1MessageAction;
	private UnityAction<CombatUnit,CombatUnit,int> torpedoSendHeavyTorpedoAttackHitBasePhaserSection2MessageAction;
	private UnityAction<CombatUnit,CombatUnit,int> torpedoSendHeavyTorpedoAttackHitBaseTorpedoSectionMessageAction;
	private UnityAction<CombatUnit,CombatUnit,int> torpedoSendHeavyTorpedoAttackHitBaseCrewSectionMessageAction;
	private UnityAction<CombatUnit,CombatUnit,int> torpedoSendHeavyTorpedoAttackHitBaseStorageSection1MessageAction;
	private UnityAction<CombatUnit,CombatUnit,int> torpedoSendHeavyTorpedoAttackHitBaseStorageSection2MessageAction;

	private UnityAction<CombatUnit,CombatUnit,CombatManager.CrystalType,int> crystalSendCrystalUsedOnShipPhaserSectionMessageAction;
	private UnityAction<CombatUnit,CombatUnit,CombatManager.CrystalType,int> crystalSendCrystalUsedOnShipTorpedoSectionMessageAction;
	private UnityAction<CombatUnit,CombatUnit,CombatManager.CrystalType,int> crystalSendCrystalUsedOnShipStorageSectionMessageAction;
	private UnityAction<CombatUnit,CombatUnit,CombatManager.CrystalType,int> crystalSendCrystalUsedOnShipCrewSectionMessageAction;
	private UnityAction<CombatUnit,CombatUnit,CombatManager.CrystalType,int> crystalSendCrystalUsedOnShipEngineSectionMessageAction;

	private UnityAction<CombatUnit,CombatUnit,CombatManager.CrystalType,int> crystalSendCrystalUsedOnBasePhaserSection1MessageAction;
	private UnityAction<CombatUnit,CombatUnit,CombatManager.CrystalType,int> crystalSendCrystalUsedOnBasePhaserSection2MessageAction;
	private UnityAction<CombatUnit,CombatUnit,CombatManager.CrystalType,int> crystalSendCrystalUsedOnBaseTorpedoSectionMessageAction;
	private UnityAction<CombatUnit,CombatUnit,CombatManager.CrystalType,int> crystalSendCrystalUsedOnBaseCrewSectionMessageAction;
	private UnityAction<CombatUnit,CombatUnit,CombatManager.CrystalType,int> crystalSendCrystalUsedOnBaseStorageSection1MessageAction;
	private UnityAction<CombatUnit,CombatUnit,CombatManager.CrystalType,int> crystalSendCrystalUsedOnBaseStorageSection2MessageAction;

	private UnityAction<CombatUnit,CombatUnit> repairSendRepairCrewUsedOnShipPhaserSectionMessageAction;
	private UnityAction<CombatUnit,CombatUnit> repairSendRepairCrewUsedOnShipTorpedoSectionMessageAction;
	private UnityAction<CombatUnit,CombatUnit> repairSendRepairCrewUsedOnShipStorageSectionMessageAction;
	private UnityAction<CombatUnit,CombatUnit> repairSendRepairCrewUsedOnShipCrewSectionMessageAction;
	private UnityAction<CombatUnit,CombatUnit> repairSendRepairCrewUsedOnShipEngineSectionMessageAction;

	private UnityAction<CombatUnit,CombatUnit> repairSendRepairCrewUsedOnBasePhaserSection1MessageAction;
	private UnityAction<CombatUnit,CombatUnit> repairSendRepairCrewUsedOnBasePhaserSection2MessageAction;
	private UnityAction<CombatUnit,CombatUnit> repairSendRepairCrewUsedOnBaseTorpedoSectionMessageAction;
	private UnityAction<CombatUnit,CombatUnit> repairSendRepairCrewUsedOnBaseCrewSectionMessageAction;
	private UnityAction<CombatUnit,CombatUnit> repairSendRepairCrewUsedOnBaseStorageSection1MessageAction;
	private UnityAction<CombatUnit,CombatUnit> repairSendRepairCrewUsedOnBaseStorageSection2MessageAction;

	private UnityAction<CombatUnit> combatUnitSendShipPhaserSectionDestroyedMessageAction;
	private UnityAction<CombatUnit> combatUnitSendShipTorpedoSectionDestroyedMessageAction;
	private UnityAction<CombatUnit> combatUnitSendShipStorageSectionDestroyedMessageAction;
	private UnityAction<CombatUnit> combatUnitSendShipCrewSectionDestroyedMessageAction;
	private UnityAction<CombatUnit> combatUnitSendShipEngineSectionDestroyedMessageAction;

	private UnityAction<CombatUnit> combatUnitSendBasePhaserSection1DestroyedMessageAction;
	private UnityAction<CombatUnit> combatUnitSendBasePhaserSection2DestroyedMessageAction;
	private UnityAction<CombatUnit> combatUnitSendBaseTorpedoSectionDestroyedMessageAction;
	private UnityAction<CombatUnit> combatUnitSendBaseCrewSectionDestroyedMessageAction;
	private UnityAction<CombatUnit> combatUnitSendBaseStorageSection1DestroyedMessageAction;
	private UnityAction<CombatUnit> combatUnitSendBaseStorageSection2DestroyedMessageAction;

	private UnityAction<CombatUnit> combatUnitSendShipDestroyedMessageAction;
	private UnityAction<CombatUnit> combatUnitSendBaseDestroyedMessageAction;

	private UnityAction<CombatUnit,string> combatUnitSendUnitRenamedMessageAction;

	private UnityAction<Player,int> playerSendIncomeMessageAction;

	private UnityAction<string,Player> playerSendPlanetColonizedMessageAction;

	private UnityAction<Dictionary<string,int>,int,CombatUnit> purchaseSendPurchaseMessageAction;

	private UnityAction<NameNewShip.NewUnitEventData> purchaseShipSendPurchaseMessageAction;

	private UnityAction<CombatUnit,int> combatUnitSendSunburstDamageMessageAction;

	private UnityAction<Player> playerSendLoadedTurnAction;

	private UnityAction<FileManager.SaveGameData> saveDataSendSavedGameAction;

	private UnityAction<string> stringSendDeletedFileMessageAction;

	//additional actions after adding cutscene manager
	private UnityAction<CombatUnit,CombatUnit,int> cutscenePhasersSendPhaserAttackMissMessageAction;
	private UnityAction<CombatUnit,CombatUnit,int> cutsceneTorpedoSendLightTorpedoAttackMissMessageAction;
	private UnityAction<CombatUnit,CombatUnit,int> cutsceneTorpedoSendHeavyTorpedoAttackMissMessageAction;
	private UnityAction<CombatUnit,CombatUnit,int> cutsceneFlaresSendFlaresDefeatLightTorpedoMessageAction;
	private UnityAction<CombatUnit,CombatUnit,int> cutsceneFlaresSendFlaresDefeatHeavyTorpedoMessageAction;

	//action for a player being killed
	private UnityAction<Player> killPlayerAction;

	// Use this for initialization
	public void Init () {

		//get the managers
		gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
		uiManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
		mouseManager = GameObject.FindGameObjectWithTag("MouseManager").GetComponent<MouseManager>();

		//set the actions
		playerSendTurnPhaseMessageAction = (player) => {SendTurnPhaseMessage();};

		stringSendNewChatMessageAction = (newMessage) => {SendNewChatMessage(newMessage);};

		movementSendNewShipMovementMessageAction = (ship, startingHex, endingHex) => {SendNewShipMovementMessage(ship,startingHex,endingHex);};

		shipSendTractorBeamPrimedMessageAction = (ship) => {SendTractorBeamPrimedMessage(ship);};
		shipSendTractorBeamEngagedMessageAction = (ship) => {SendTractorBeamEngagedMessage(ship);};
		shipSendTractorBeamDisengagedMessageAction = (ship) => {SendTractorBeamDisengagedMessage(ship);};

		shipSendUseWarpBoosterMessageAction = (ship) => {SendUseWarpBoosterMessage(ship);};
		shipSendUseTranswarpBoosterMessageAction = (ship) => {SendUseTranswarpBoosterMessage(ship);};

		firePhasersSendPhaserAttackMessageAction = (attackingUnit, targetedUnit, sectionTargeted) => {SendPhaserAttackMessage(attackingUnit,targetedUnit);};
		phasersSendPhaserAttackHitShipPhaserSectionMessageAction = (attackingUnit, targetedUnit, damage) => {SendPhaserAttackHitShipPhaserSectionMessage(attackingUnit, targetedUnit, damage);};
		phasersSendPhaserAttackHitShipTorpedoSectionMessageAction = (attackingUnit, targetedUnit, damage) => {SendPhaserAttackHitShipTorpedoSectionMessage(attackingUnit, targetedUnit, damage);};
		phasersSendPhaserAttackHitShipStorageSectionMessageAction = (attackingUnit, targetedUnit, damage) => {SendPhaserAttackHitShipStorageSectionMessage(attackingUnit, targetedUnit, damage);};
		phasersSendPhaserAttackHitShipCrewSectionMessageAction = (attackingUnit, targetedUnit, damage) => {SendPhaserAttackHitShipCrewSectionMessage(attackingUnit, targetedUnit, damage);};
		phasersSendPhaserAttackHitShipEngineSectionMessageAction = (attackingUnit, targetedUnit, damage) => {SendPhaserAttackHitShipEngineSectionMessage(attackingUnit, targetedUnit, damage);};
		//phasersSendPhaserAttackMissMessageAction = (attackingUnit, targetedUnit) => {SendPhaserAttackMissMessage(attackingUnit, targetedUnit);};

		cutscenePhasersSendPhaserAttackMissMessageAction = (attackingUnit, targetedUnit, damage) => {SendPhaserAttackMissMessage(attackingUnit, targetedUnit);};

		phasersSendPhaserAttackHitBasePhaserSection1MessageAction = (attackingUnit, targetedUnit, damage) => {SendPhaserAttackHitBasePhaserSection1Message(attackingUnit, targetedUnit, damage);};
		phasersSendPhaserAttackHitBasePhaserSection2MessageAction = (attackingUnit, targetedUnit, damage) => {SendPhaserAttackHitBasePhaserSection2Message(attackingUnit, targetedUnit, damage);};
		phasersSendPhaserAttackHitBaseTorpedoSectionMessageAction = (attackingUnit, targetedUnit, damage) => {SendPhaserAttackHitBaseTorpedoSectionMessage(attackingUnit, targetedUnit, damage);};
		phasersSendPhaserAttackHitBaseCrewSectionMessageAction = (attackingUnit, targetedUnit, damage) => {SendPhaserAttackHitBaseCrewSectionMessage(attackingUnit, targetedUnit, damage);};
		phasersSendPhaserAttackHitBaseStorageSection1MessageAction = (attackingUnit, targetedUnit, damage) => {SendPhaserAttackHitBaseStorageSection1Message(attackingUnit, targetedUnit, damage);};
		phasersSendPhaserAttackHitBaseStorageSection2MessageAction = (attackingUnit, targetedUnit, damage) => {SendPhaserAttackHitBaseStorageSection2Message(attackingUnit, targetedUnit, damage);};

		fireLightTorpedoSendTorpedoAttackMessageAction = (attackingUnit,targetedUnit,sectionTargeted) => {SendTorpedoAttackMessage(attackingUnit,targetedUnit, "Light");};
		fireHeavyTorpedoSendTorpedoAttackMessageAction = (attackingUnit,targetedUnit,sectionTargeted) => {SendTorpedoAttackMessage(attackingUnit,targetedUnit, "Heavy");};

		flareSendUsedFlaresYesMessageAction = (flareEventData) => {SendUsedFlaresYesMessage (flareEventData.targetedUnit,flareEventData.numberFlaresUsed);};
		flareSendUsedFlaresNoMessageAction = (flareEventData) => {SendUsedFlaresNoMessage (flareEventData.targetedUnit);};

		//flaresSendFlaresDefeatLightTorpedoMessageAction = (attackingUnit, targetedUnit, sectionTargeted, numberFlaresUsed) => {SendFlaresDefeatTorpedoMessage("Light");};
		//flaresSendFlaresDefeatHeavyTorpedoMessageAction = (attackingUnit, targetedUnit, sectionTargeted, numberFlaresUsed) => {SendFlaresDefeatTorpedoMessage("Heavy");};

		cutsceneFlaresSendFlaresDefeatLightTorpedoMessageAction = (attackingUnit, targetedUnit, numberFlaresUsed) => {SendFlaresDefeatTorpedoMessage("Light");};
		cutsceneFlaresSendFlaresDefeatHeavyTorpedoMessageAction = (attackingUnit, targetedUnit, numberFlaresUsed) => {SendFlaresDefeatTorpedoMessage("Heavy");};

		flaresSendFlaresFailLightTorpedoMessageAction = (attackingUnit, targetedUnit, numberFlaresUsed) => {SendFlaresFailMessage("Light");};
		flaresSendFlaresFailHeavyTorpedoMessageAction = (attackingUnit, targetedUnit, numberFlaresUsed) => {SendFlaresFailMessage("Heavy");};

		torpedoSendLightTorpedoAttackHitShipPhaserSectionMessageAction = (attackingUnit, targetedUnit, damage) => {SendTorpedoAttackHitShipPhaserSectionMessage(attackingUnit, targetedUnit, damage, "Light");};
		torpedoSendHeavyTorpedoAttackHitShipPhaserSectionMessageAction = (attackingUnit, targetedUnit, damage) => {SendTorpedoAttackHitShipPhaserSectionMessage(attackingUnit, targetedUnit, damage, "Heavy");};
		torpedoSendLightTorpedoAttackHitShipTorpedoSectionMessageAction = (attackingUnit, targetedUnit, damage) => {SendTorpedoAttackHitShipTorpedoSectionMessage(attackingUnit, targetedUnit, damage, "Light");};
		torpedoSendHeavyTorpedoAttackHitShipTorpedoSectionMessageAction = (attackingUnit, targetedUnit, damage) => {SendTorpedoAttackHitShipTorpedoSectionMessage(attackingUnit, targetedUnit, damage, "Heavy");};
		torpedoSendLightTorpedoAttackHitShipStorageSectionMessageAction = (attackingUnit, targetedUnit, damage) => {SendTorpedoAttackHitShipStorageSectionMessage(attackingUnit, targetedUnit, damage, "Light");};
		torpedoSendHeavyTorpedoAttackHitShipStorageSectionMessageAction = (attackingUnit, targetedUnit, damage) => {SendTorpedoAttackHitShipStorageSectionMessage(attackingUnit, targetedUnit, damage, "Heavy");};
		torpedoSendLightTorpedoAttackHitShipCrewSectionMessageAction = (attackingUnit, targetedUnit, damage) => {SendTorpedoAttackHitShipCrewSectionMessage(attackingUnit, targetedUnit, damage, "Light");};
		torpedoSendHeavyTorpedoAttackHitShipCrewSectionMessageAction = (attackingUnit, targetedUnit, damage) => {SendTorpedoAttackHitShipCrewSectionMessage(attackingUnit, targetedUnit, damage, "Heavy");};
		torpedoSendLightTorpedoAttackHitShipEngineSectionMessageAction = (attackingUnit, targetedUnit, damage) => {SendTorpedoAttackHitShipEngineSectionMessage(attackingUnit, targetedUnit, damage, "Light");};
		torpedoSendHeavyTorpedoAttackHitShipEngineSectionMessageAction = (attackingUnit, targetedUnit, damage) => {SendTorpedoAttackHitShipEngineSectionMessage(attackingUnit, targetedUnit, damage, "Heavy");};
	
		//torpedoSendLightTorpedoAttackMissMessageAction = (attackingUnit, targetedUnit) => {SendTorpedoAttackMissMessage(attackingUnit, targetedUnit, "Light");};
		//torpedoSendHeavyTorpedoAttackMissMessageAction = (attackingUnit, targetedUnit) => {SendTorpedoAttackMissMessage(attackingUnit, targetedUnit, "Heavy");};

		cutsceneTorpedoSendLightTorpedoAttackMissMessageAction = (attackingUnit, targetedUnit, damage) => {SendTorpedoAttackMissMessage(attackingUnit, targetedUnit, "Light");};
		cutsceneTorpedoSendHeavyTorpedoAttackMissMessageAction = (attackingUnit, targetedUnit, damage) => {SendTorpedoAttackMissMessage(attackingUnit, targetedUnit, "Heavy");};

		torpedoSendLightTorpedoAttackHitBasePhaserSection1MessageAction = (attackingUnit, targetedUnit, damage) => {SendTorpedoAttackHitBasePhaserSection1Message(attackingUnit, targetedUnit, damage, "Light");};
		torpedoSendHeavyTorpedoAttackHitBasePhaserSection1MessageAction = (attackingUnit, targetedUnit, damage) => {SendTorpedoAttackHitBasePhaserSection1Message(attackingUnit, targetedUnit, damage, "Heavy");};
		torpedoSendLightTorpedoAttackHitBasePhaserSection2MessageAction = (attackingUnit, targetedUnit, damage) => {SendTorpedoAttackHitBasePhaserSection2Message(attackingUnit, targetedUnit, damage, "Light");};
		torpedoSendHeavyTorpedoAttackHitBasePhaserSection2MessageAction = (attackingUnit, targetedUnit, damage) => {SendTorpedoAttackHitBasePhaserSection2Message(attackingUnit, targetedUnit, damage, "Heavy");};
		torpedoSendLightTorpedoAttackHitBaseTorpedoSectionMessageAction = (attackingUnit, targetedUnit, damage) => {SendTorpedoAttackHitBaseTorpedoSectionMessage(attackingUnit, targetedUnit, damage, "Light");};
		torpedoSendHeavyTorpedoAttackHitBaseTorpedoSectionMessageAction = (attackingUnit, targetedUnit, damage) => {SendTorpedoAttackHitBaseTorpedoSectionMessage(attackingUnit, targetedUnit, damage, "Heavy");};
		torpedoSendLightTorpedoAttackHitBaseCrewSectionMessageAction = (attackingUnit, targetedUnit, damage) => {SendTorpedoAttackHitBaseCrewSectionMessage(attackingUnit, targetedUnit, damage, "Light");};
		torpedoSendHeavyTorpedoAttackHitBaseCrewSectionMessageAction = (attackingUnit, targetedUnit, damage) => {SendTorpedoAttackHitBaseCrewSectionMessage(attackingUnit, targetedUnit, damage, "Heavy");};
		torpedoSendLightTorpedoAttackHitBaseStorageSection1MessageAction = (attackingUnit, targetedUnit, damage) => {SendTorpedoAttackHitBaseStorageSection1Message(attackingUnit, targetedUnit, damage, "Light");};
		torpedoSendHeavyTorpedoAttackHitBaseStorageSection1MessageAction = (attackingUnit, targetedUnit, damage) => {SendTorpedoAttackHitBaseStorageSection1Message(attackingUnit, targetedUnit, damage, "Heavy");};
		torpedoSendLightTorpedoAttackHitBaseStorageSection2MessageAction = (attackingUnit, targetedUnit, damage) => {SendTorpedoAttackHitBaseStorageSection2Message(attackingUnit, targetedUnit, damage, "Light");};
		torpedoSendHeavyTorpedoAttackHitBaseStorageSection2MessageAction = (attackingUnit, targetedUnit, damage) => {SendTorpedoAttackHitBaseStorageSection2Message(attackingUnit, targetedUnit, damage, "Heavy");};

		crystalSendCrystalUsedOnShipPhaserSectionMessageAction = (selectedUnit, targetedUnit, crystalType, shieldsHealed) => {SendCrystalUsedMessage(selectedUnit,targetedUnit,crystalType,shieldsHealed,"Phaser Section");};
		crystalSendCrystalUsedOnShipTorpedoSectionMessageAction = (selectedUnit, targetedUnit, crystalType, shieldsHealed) => {SendCrystalUsedMessage(selectedUnit,targetedUnit,crystalType,shieldsHealed,"Torpedo Section");};
		crystalSendCrystalUsedOnShipStorageSectionMessageAction = (selectedUnit, targetedUnit, crystalType, shieldsHealed) => {SendCrystalUsedMessage(selectedUnit,targetedUnit,crystalType,shieldsHealed,"Storage Section");};
		crystalSendCrystalUsedOnShipCrewSectionMessageAction = (selectedUnit, targetedUnit, crystalType, shieldsHealed) => {SendCrystalUsedMessage(selectedUnit,targetedUnit,crystalType,shieldsHealed,"Crew Section");};
		crystalSendCrystalUsedOnShipEngineSectionMessageAction = (selectedUnit, targetedUnit, crystalType, shieldsHealed) => {SendCrystalUsedMessage(selectedUnit,targetedUnit,crystalType,shieldsHealed,"Engine Section");};

		crystalSendCrystalUsedOnBasePhaserSection1MessageAction = (selectedUnit, targetedUnit, crystalType, shieldsHealed) => {SendCrystalUsedMessage(selectedUnit,targetedUnit,crystalType,shieldsHealed,"Phaser Section 1");};
		crystalSendCrystalUsedOnBasePhaserSection2MessageAction = (selectedUnit, targetedUnit, crystalType, shieldsHealed) => {SendCrystalUsedMessage(selectedUnit,targetedUnit,crystalType,shieldsHealed,"Phaser Section 2");};
		crystalSendCrystalUsedOnBaseTorpedoSectionMessageAction = (selectedUnit, targetedUnit, crystalType, shieldsHealed) => {SendCrystalUsedMessage(selectedUnit,targetedUnit,crystalType,shieldsHealed,"Torpedo Section");};
		crystalSendCrystalUsedOnBaseCrewSectionMessageAction = (selectedUnit, targetedUnit, crystalType, shieldsHealed) => {SendCrystalUsedMessage(selectedUnit,targetedUnit,crystalType,shieldsHealed,"Crew Section");};
		crystalSendCrystalUsedOnBaseStorageSection1MessageAction = (selectedUnit, targetedUnit, crystalType, shieldsHealed) => {SendCrystalUsedMessage(selectedUnit,targetedUnit,crystalType,shieldsHealed,"Storage Section 1");};
		crystalSendCrystalUsedOnBaseStorageSection2MessageAction = (selectedUnit, targetedUnit, crystalType, shieldsHealed) => {SendCrystalUsedMessage(selectedUnit,targetedUnit,crystalType,shieldsHealed,"Storage Section 2");};

		repairSendRepairCrewUsedOnShipPhaserSectionMessageAction = (selectedUnit, targetedUnit) => {SendRepairCrewUsedMessage(selectedUnit,targetedUnit,"Phaser Section");};
		repairSendRepairCrewUsedOnShipTorpedoSectionMessageAction = (selectedUnit, targetedUnit) => {SendRepairCrewUsedMessage(selectedUnit,targetedUnit,"Torpedo Section");};
		repairSendRepairCrewUsedOnShipStorageSectionMessageAction = (selectedUnit, targetedUnit) => {SendRepairCrewUsedMessage(selectedUnit,targetedUnit,"Storage Section");};
		repairSendRepairCrewUsedOnShipCrewSectionMessageAction = (selectedUnit, targetedUnit) => {SendRepairCrewUsedMessage(selectedUnit,targetedUnit,"Crew Section");};
		repairSendRepairCrewUsedOnShipEngineSectionMessageAction = (selectedUnit, targetedUnit) => {SendRepairCrewUsedMessage(selectedUnit,targetedUnit,"Engine Section");};

		repairSendRepairCrewUsedOnBasePhaserSection1MessageAction = (selectedUnit, targetedUnit) => {SendRepairCrewUsedMessage(selectedUnit,targetedUnit,"Phaser Section 1");};
		repairSendRepairCrewUsedOnBasePhaserSection2MessageAction = (selectedUnit, targetedUnit) => {SendRepairCrewUsedMessage(selectedUnit,targetedUnit,"Phaser Section 2");};
		repairSendRepairCrewUsedOnBaseTorpedoSectionMessageAction = (selectedUnit, targetedUnit) => {SendRepairCrewUsedMessage(selectedUnit,targetedUnit,"Torpedo Section");};
		repairSendRepairCrewUsedOnBaseCrewSectionMessageAction = (selectedUnit, targetedUnit) => {SendRepairCrewUsedMessage(selectedUnit,targetedUnit,"Crew Section");};
		repairSendRepairCrewUsedOnBaseStorageSection1MessageAction = (selectedUnit, targetedUnit) => {SendRepairCrewUsedMessage(selectedUnit,targetedUnit,"Storage Section 1");};
		repairSendRepairCrewUsedOnBaseStorageSection2MessageAction = (selectedUnit, targetedUnit) => {SendRepairCrewUsedMessage(selectedUnit,targetedUnit,"Storage Section 2");};

		combatUnitSendShipPhaserSectionDestroyedMessageAction = (combatUnit) => {SendShipPhaserSectionDestroyedMessage(combatUnit);};
		combatUnitSendShipTorpedoSectionDestroyedMessageAction = (combatUnit) => {SendShipTorpedoSectionDestroyedMessage(combatUnit);};
		combatUnitSendShipStorageSectionDestroyedMessageAction = (combatUnit) => {SendShipStorageSectionDestroyedMessage(combatUnit);};
		combatUnitSendShipCrewSectionDestroyedMessageAction = (combatUnit) => {SendShipCrewSectionDestroyedMessage(combatUnit);};
		combatUnitSendShipEngineSectionDestroyedMessageAction = (combatUnit) => {SendShipEngineSectionDestroyedMessage(combatUnit);};

		combatUnitSendBasePhaserSection1DestroyedMessageAction = (combatUnit) => {SendBasePhaserSection1DestroyedMessage(combatUnit);};
		combatUnitSendBasePhaserSection2DestroyedMessageAction = (combatUnit) => {SendBasePhaserSection2DestroyedMessage (combatUnit);};
		combatUnitSendBaseTorpedoSectionDestroyedMessageAction = (combatUnit) => {SendBaseTorpedoSectionDestroyedMessage(combatUnit);};
		combatUnitSendBaseCrewSectionDestroyedMessageAction = (combatUnit) => {SendBaseCrewSectionDestroyedMessage(combatUnit);};
		combatUnitSendBaseStorageSection1DestroyedMessageAction = (combatUnit) => {SendBaseStorageSection1DestroyedMessage(combatUnit);};
		combatUnitSendBaseStorageSection2DestroyedMessageAction = (combatUnit) => {SendBaseStorageSection2DestroyedMessage(combatUnit);};

		combatUnitSendShipDestroyedMessageAction = (combatUnit) => {SendShipDestroyedMessage(combatUnit);};
		combatUnitSendBaseDestroyedMessageAction = (combatUnit) => {SendBaseDestroyedMessage(combatUnit);};

		combatUnitSendUnitRenamedMessageAction = (combatUnit, oldName) => {SendUnitRenamedMessage(combatUnit,oldName);};

		playerSendIncomeMessageAction = (currentTurnPlayer,turnIncome) => {SendPlayerIncomeMessage(currentTurnPlayer,turnIncome);};

		playerSendPlanetColonizedMessageAction = (planetColonized,playerColonizing) => {SendPlanetColonizationMessage(planetColonized,playerColonizing);};

		purchaseSendPurchaseMessageAction = (purchasedItems,purchasedValue,combatUnit) => {SendPurchaseItemsMessage(purchasedItems,purchasedValue,combatUnit);};
	
		purchaseShipSendPurchaseMessageAction = (newUnitData) => {SendPurchaseShipMessage(newUnitData);};

		combatUnitSendSunburstDamageMessageAction = (combatUnit, sunburstDamage) => {SendSunburstDamageMessage(combatUnit,sunburstDamage);};

		playerSendLoadedTurnAction = (player) => {SendLoadedTurnPhaseMessage ();};

		saveDataSendSavedGameAction = (saveGameData) => {SendSaveGameMessage(saveGameData);};

		stringSendDeletedFileMessageAction = (fileName) => {SendDeletedFileMessage (fileName);};

		killPlayerAction = (player) => {SendPlayerEliminatedMessage(player);};


		//add listener for the start of a new turn.  The onNewTurn passes a player, but we don't need it
		gameManager.OnNewTurn.AddListener(playerSendTurnPhaseMessageAction);

		//add listener for the start of the main phase.  The OnBeginMainPhase passes a player, but we don't need it
		gameManager.OnBeginMainPhase.AddListener(playerSendTurnPhaseMessageAction);

		//add listener for when a new chat message is submitted
		uiManager.GetComponent<TextInput>().OnNewChatMessageSubmitted.AddListener(stringSendNewChatMessageAction);

		//add a listener for when a ship movement is completed
		EngineSection.OnMoveFromToFinish.AddListener(movementSendNewShipMovementMessageAction);

		//add listeners for the tractor beam
		PhaserSection.OnPrimeTractorBeam.AddListener(shipSendTractorBeamPrimedMessageAction);
		PhaserSection.OnEngageTractorBeam.AddListener(shipSendTractorBeamEngagedMessageAction);
		PhaserSection.OnDisengageTractorBeam.AddListener(shipSendTractorBeamDisengagedMessageAction);

		//add listeners for warp boosters
		EngineSection.OnUseWarpBooster.AddListener(shipSendUseWarpBoosterMessageAction);
		EngineSection.OnUseTranswarpBooster.AddListener(shipSendUseTranswarpBoosterMessageAction);

		//add listeners for ship phaser attacks
		PhaserSection.OnFirePhasers.AddListener(firePhasersSendPhaserAttackMessageAction);

		/*
		//these are commented out so that I can replace them with listeners to cutscene events
		CombatManager.OnPhaserAttackHitShipPhaserSection.AddListener(phasersSendPhaserAttackHitShipPhaserSectionMessageAction);
		CombatManager.OnPhaserAttackHitShipTorpedoSection.AddListener(phasersSendPhaserAttackHitShipTorpedoSectionMessageAction);
		CombatManager.OnPhaserAttackHitShipStorageSection.AddListener(phasersSendPhaserAttackHitShipStorageSectionMessageAction);
		CombatManager.OnPhaserAttackHitShipCrewSection.AddListener(phasersSendPhaserAttackHitShipCrewSectionMessageAction);
		CombatManager.OnPhaserAttackHitShipEngineSection.AddListener(phasersSendPhaserAttackHitShipEngineSectionMessageAction);
		CombatManager.OnPhaserAttackMissShip.AddListener(phasersSendPhaserAttackMissMessageAction);
		*/

		CutsceneManager.OnPhaserHitShipPhaserSection.AddListener (phasersSendPhaserAttackHitShipPhaserSectionMessageAction);
		CutsceneManager.OnPhaserHitShipTorpedoSection.AddListener (phasersSendPhaserAttackHitShipTorpedoSectionMessageAction);
		CutsceneManager.OnPhaserHitShipStorageSection.AddListener (phasersSendPhaserAttackHitShipStorageSectionMessageAction);
		CutsceneManager.OnPhaserHitShipCrewSection.AddListener (phasersSendPhaserAttackHitShipCrewSectionMessageAction);
		CutsceneManager.OnPhaserHitShipEngineSection.AddListener (phasersSendPhaserAttackHitShipEngineSectionMessageAction);
		CutsceneManager.OnPhaserMissShip.AddListener (cutscenePhasersSendPhaserAttackMissMessageAction);


		//add listeners for base phaser attacks
		StarbasePhaserSection1.OnFirePhasers.AddListener(firePhasersSendPhaserAttackMessageAction);
		StarbasePhaserSection2.OnFirePhasers.AddListener(firePhasersSendPhaserAttackMessageAction);

		/*
		//these are commented out so that I can replace them with listeners to cutscene events
		CombatManager.OnPhaserAttackHitBasePhaserSection1.AddListener(phasersSendPhaserAttackHitBasePhaserSection1MessageAction);
		CombatManager.OnPhaserAttackHitBasePhaserSection2.AddListener(phasersSendPhaserAttackHitBasePhaserSection2MessageAction);
		CombatManager.OnPhaserAttackHitBaseTorpedoSection.AddListener(phasersSendPhaserAttackHitBaseTorpedoSectionMessageAction);
		CombatManager.OnPhaserAttackHitBaseCrewSection.AddListener(phasersSendPhaserAttackHitBaseCrewSectionMessageAction);
		CombatManager.OnPhaserAttackHitBaseStorageSection1.AddListener(phasersSendPhaserAttackHitBaseStorageSection1MessageAction);
		CombatManager.OnPhaserAttackHitBaseStorageSection2.AddListener(phasersSendPhaserAttackHitBaseStorageSection2MessageAction);
		CombatManager.OnPhaserAttackMissBase.AddListener(phasersSendPhaserAttackMissMessageAction);
		*/

		CutsceneManager.OnPhaserHitBasePhaserSection1.AddListener (phasersSendPhaserAttackHitBasePhaserSection1MessageAction);
		CutsceneManager.OnPhaserHitBasePhaserSection2.AddListener (phasersSendPhaserAttackHitBasePhaserSection2MessageAction);
		CutsceneManager.OnPhaserHitBaseTorpedoSection.AddListener (phasersSendPhaserAttackHitBaseTorpedoSectionMessageAction);
		CutsceneManager.OnPhaserHitBaseCrewSection.AddListener (phasersSendPhaserAttackHitBaseCrewSectionMessageAction);
		CutsceneManager.OnPhaserHitBaseStorageSection1.AddListener (phasersSendPhaserAttackHitBaseStorageSection1MessageAction);
		CutsceneManager.OnPhaserHitBaseStorageSection2.AddListener (phasersSendPhaserAttackHitBaseStorageSection2MessageAction);
		CutsceneManager.OnPhaserMissBase.AddListener (cutscenePhasersSendPhaserAttackMissMessageAction);


		//add listeners for torpedo attacks
		TorpedoSection.OnFireLightTorpedo.AddListener(fireLightTorpedoSendTorpedoAttackMessageAction);
		TorpedoSection.OnFireHeavyTorpedo.AddListener(fireHeavyTorpedoSendTorpedoAttackMessageAction);
		StarbaseTorpedoSection.OnFireLightTorpedo.AddListener(fireLightTorpedoSendTorpedoAttackMessageAction);
		StarbaseTorpedoSection.OnFireHeavyTorpedo.AddListener(fireHeavyTorpedoSendTorpedoAttackMessageAction);

		uiManager.GetComponent<FlareManager> ().OnUseFlaresYes.AddListener (flareSendUsedFlaresYesMessageAction);
		uiManager.GetComponent<FlareManager> ().OnUseFlaresCancel.AddListener (flareSendUsedFlaresNoMessageAction);

		/*
		//these are commented out so that I can replace them with listeners to cutscene events
		CombatManager.OnLightTorpedoAttackDefeatedByFlares.AddListener(flaresSendFlaresDefeatLightTorpedoMessageAction);
		CombatManager.OnHeavyTorpedoAttackDefeatedByFlares.AddListener(flaresSendFlaresDefeatHeavyTorpedoMessageAction);
		CombatManager.OnLightTorpedoAttackFlaresFailed.AddListener(flaresSendFlaresFailLightTorpedoMessageAction);
		CombatManager.OnHeavyTorpedoAttackFlaresFailed.AddListener(flaresSendFlaresFailHeavyTorpedoMessageAction);
		*/

		CutsceneManager.OnLightTorpedoFlareSuccess.AddListener (cutsceneFlaresSendFlaresDefeatLightTorpedoMessageAction);
		CutsceneManager.OnHeavyTorpedoFlareSuccess.AddListener (cutsceneFlaresSendFlaresDefeatHeavyTorpedoMessageAction);
		CutsceneManager.OnLightTorpedoFlareFailure.AddListener (flaresSendFlaresFailLightTorpedoMessageAction);
		CutsceneManager.OnHeavyTorpedoFlareFailure.AddListener (flaresSendFlaresFailHeavyTorpedoMessageAction);
			
		//add listeners for ship torpedo attacks
		/*
		//these are commented out so that I can replace them with listeners to cutscene events
		CombatManager.OnLightTorpedoAttackHitShipPhaserSection.AddListener(torpedoSendLightTorpedoAttackHitShipPhaserSectionMessageAction);
		CombatManager.OnHeavyTorpedoAttackHitShipPhaserSection.AddListener(torpedoSendHeavyTorpedoAttackHitShipPhaserSectionMessageAction);
		CombatManager.OnLightTorpedoAttackHitShipTorpedoSection.AddListener(torpedoSendLightTorpedoAttackHitShipTorpedoSectionMessageAction);
		CombatManager.OnHeavyTorpedoAttackHitShipTorpedoSection.AddListener(torpedoSendHeavyTorpedoAttackHitShipTorpedoSectionMessageAction);
		CombatManager.OnLightTorpedoAttackHitShipStorageSection.AddListener(torpedoSendLightTorpedoAttackHitShipStorageSectionMessageAction);
		CombatManager.OnHeavyTorpedoAttackHitShipStorageSection.AddListener(torpedoSendHeavyTorpedoAttackHitShipStorageSectionMessageAction);
		CombatManager.OnLightTorpedoAttackHitShipCrewSection.AddListener(torpedoSendLightTorpedoAttackHitShipCrewSectionMessageAction);
		CombatManager.OnHeavyTorpedoAttackHitShipCrewSection.AddListener(torpedoSendHeavyTorpedoAttackHitShipCrewSectionMessageAction);
		CombatManager.OnLightTorpedoAttackHitShipEngineSection.AddListener(torpedoSendLightTorpedoAttackHitShipEngineSectionMessageAction);
		CombatManager.OnHeavyTorpedoAttackHitShipEngineSection.AddListener(torpedoSendHeavyTorpedoAttackHitShipEngineSectionMessageAction);
		CombatManager.OnLightTorpedoAttackMissShip.AddListener(torpedoSendLightTorpedoAttackMissMessageAction);
		CombatManager.OnHeavyTorpedoAttackMissShip.AddListener(torpedoSendHeavyTorpedoAttackMissMessageAction);
		*/

		CutsceneManager.OnLightTorpedoHitShipPhaserSection.AddListener (torpedoSendLightTorpedoAttackHitShipPhaserSectionMessageAction);
		CutsceneManager.OnHeavyTorpedoHitShipPhaserSection.AddListener (torpedoSendHeavyTorpedoAttackHitShipPhaserSectionMessageAction);
		CutsceneManager.OnLightTorpedoHitShipTorpedoSection.AddListener (torpedoSendLightTorpedoAttackHitShipTorpedoSectionMessageAction);
		CutsceneManager.OnHeavyTorpedoHitShipTorpedoSection.AddListener (torpedoSendHeavyTorpedoAttackHitShipTorpedoSectionMessageAction);
		CutsceneManager.OnLightTorpedoHitShipStorageSection.AddListener (torpedoSendLightTorpedoAttackHitShipStorageSectionMessageAction);
		CutsceneManager.OnHeavyTorpedoHitShipStorageSection.AddListener (torpedoSendHeavyTorpedoAttackHitShipStorageSectionMessageAction);
		CutsceneManager.OnLightTorpedoHitShipCrewSection.AddListener (torpedoSendLightTorpedoAttackHitShipCrewSectionMessageAction);
		CutsceneManager.OnHeavyTorpedoHitShipCrewSection.AddListener (torpedoSendHeavyTorpedoAttackHitShipCrewSectionMessageAction);
		CutsceneManager.OnLightTorpedoHitShipEngineSection.AddListener (torpedoSendLightTorpedoAttackHitShipEngineSectionMessageAction);
		CutsceneManager.OnHeavyTorpedoHitShipEngineSection.AddListener (torpedoSendHeavyTorpedoAttackHitShipEngineSectionMessageAction);
		CutsceneManager.OnLightTorpedoMissShip.AddListener (cutsceneTorpedoSendLightTorpedoAttackMissMessageAction);
		CutsceneManager.OnHeavyTorpedoMissShip.AddListener (cutsceneTorpedoSendHeavyTorpedoAttackMissMessageAction);


		//add listeners for base torpedo attacks
		/*
		//these are commented out so that I can replace them with listeners to cutscene events
		CombatManager.OnLightTorpedoAttackHitBasePhaserSection1.AddListener(torpedoSendLightTorpedoAttackHitBasePhaserSection1MessageAction);
		CombatManager.OnHeavyTorpedoAttackHitBasePhaserSection1.AddListener(torpedoSendHeavyTorpedoAttackHitBasePhaserSection1MessageAction);
		CombatManager.OnLightTorpedoAttackHitBasePhaserSection2.AddListener (torpedoSendLightTorpedoAttackHitBasePhaserSection2MessageAction);
		CombatManager.OnHeavyTorpedoAttackHitBasePhaserSection2.AddListener(torpedoSendHeavyTorpedoAttackHitBasePhaserSection2MessageAction);
		CombatManager.OnLightTorpedoAttackHitBaseTorpedoSection.AddListener(torpedoSendLightTorpedoAttackHitBaseTorpedoSectionMessageAction);
		CombatManager.OnHeavyTorpedoAttackHitBaseTorpedoSection.AddListener(torpedoSendHeavyTorpedoAttackHitBaseTorpedoSectionMessageAction);
		CombatManager.OnLightTorpedoAttackHitBaseCrewSection.AddListener(torpedoSendLightTorpedoAttackHitBaseCrewSectionMessageAction);
		CombatManager.OnHeavyTorpedoAttackHitBaseCrewSection.AddListener(torpedoSendHeavyTorpedoAttackHitBaseCrewSectionMessageAction);
		CombatManager.OnLightTorpedoAttackHitBaseStorageSection1.AddListener(torpedoSendLightTorpedoAttackHitBaseStorageSection1MessageAction);
		CombatManager.OnHeavyTorpedoAttackHitBaseStorageSection1.AddListener(torpedoSendHeavyTorpedoAttackHitBaseStorageSection1MessageAction);
		CombatManager.OnLightTorpedoAttackHitBaseStorageSection2.AddListener(torpedoSendLightTorpedoAttackHitBaseStorageSection2MessageAction);
		CombatManager.OnHeavyTorpedoAttackHitBaseStorageSection2.AddListener(torpedoSendHeavyTorpedoAttackHitBaseStorageSection2MessageAction);
		CombatManager.OnLightTorpedoAttackMissBase.AddListener(torpedoSendLightTorpedoAttackMissMessageAction);
		CombatManager.OnHeavyTorpedoAttackMissBase.AddListener(torpedoSendHeavyTorpedoAttackMissMessageAction);
		*/

		CutsceneManager.OnLightTorpedoHitBasePhaserSection1.AddListener (torpedoSendLightTorpedoAttackHitBasePhaserSection1MessageAction);
		CutsceneManager.OnHeavyTorpedoHitBasePhaserSection1.AddListener (torpedoSendHeavyTorpedoAttackHitBasePhaserSection1MessageAction);
		CutsceneManager.OnLightTorpedoHitBasePhaserSection2.AddListener (torpedoSendLightTorpedoAttackHitBasePhaserSection2MessageAction);
		CutsceneManager.OnHeavyTorpedoHitBasePhaserSection2.AddListener (torpedoSendHeavyTorpedoAttackHitBasePhaserSection2MessageAction);
		CutsceneManager.OnLightTorpedoHitBaseTorpedoSection.AddListener (torpedoSendLightTorpedoAttackHitBaseTorpedoSectionMessageAction);
		CutsceneManager.OnHeavyTorpedoHitBaseTorpedoSection.AddListener (torpedoSendHeavyTorpedoAttackHitBaseTorpedoSectionMessageAction);
		CutsceneManager.OnLightTorpedoHitBaseCrewSection.AddListener (torpedoSendLightTorpedoAttackHitBaseCrewSectionMessageAction);
		CutsceneManager.OnHeavyTorpedoHitBaseCrewSection.AddListener (torpedoSendHeavyTorpedoAttackHitBaseCrewSectionMessageAction);
		CutsceneManager.OnLightTorpedoHitBaseStorageSection1.AddListener (torpedoSendLightTorpedoAttackHitBaseStorageSection1MessageAction);
		CutsceneManager.OnHeavyTorpedoHitBaseStorageSection1.AddListener (torpedoSendHeavyTorpedoAttackHitBaseStorageSection1MessageAction);
		CutsceneManager.OnLightTorpedoHitBaseStorageSection2.AddListener (torpedoSendLightTorpedoAttackHitBaseStorageSection2MessageAction);
		CutsceneManager.OnHeavyTorpedoHitBaseStorageSection2.AddListener (torpedoSendHeavyTorpedoAttackHitBaseStorageSection2MessageAction);
		CutsceneManager.OnLightTorpedoMissBase.AddListener (cutsceneTorpedoSendLightTorpedoAttackMissMessageAction);
		CutsceneManager.OnHeavyTorpedoMissBase.AddListener (cutsceneTorpedoSendHeavyTorpedoAttackMissMessageAction);

		//add listeners for using crystals to heal ships
		CombatManager.OnCrystalUsedOnShipPhaserSection.AddListener(crystalSendCrystalUsedOnShipPhaserSectionMessageAction);
		CombatManager.OnCrystalUsedOnShipTorpedoSection.AddListener(crystalSendCrystalUsedOnShipTorpedoSectionMessageAction);
		CombatManager.OnCrystalUsedOnShipStorageSection.AddListener(crystalSendCrystalUsedOnShipStorageSectionMessageAction);
		CombatManager.OnCrystalUsedOnShipCrewSection.AddListener(crystalSendCrystalUsedOnShipCrewSectionMessageAction);
		CombatManager.OnCrystalUsedOnShipEngineSection.AddListener(crystalSendCrystalUsedOnShipEngineSectionMessageAction);

		//add listeners for using crystals to heal bases
		CombatManager.OnCrystalUsedOnBasePhaserSection1.AddListener(crystalSendCrystalUsedOnBasePhaserSection1MessageAction);
		CombatManager.OnCrystalUsedOnBasePhaserSection2.AddListener(crystalSendCrystalUsedOnBasePhaserSection2MessageAction);
		CombatManager.OnCrystalUsedOnBaseTorpedoSection.AddListener(crystalSendCrystalUsedOnBaseTorpedoSectionMessageAction);
		CombatManager.OnCrystalUsedOnBaseCrewSection.AddListener(crystalSendCrystalUsedOnBaseCrewSectionMessageAction);
		CombatManager.OnCrystalUsedOnBaseStorageSection1.AddListener(crystalSendCrystalUsedOnBaseStorageSection1MessageAction);
		CombatManager.OnCrystalUsedOnBaseStorageSection2.AddListener(crystalSendCrystalUsedOnBaseStorageSection2MessageAction);

		//add listeners for using repair crews on ships
		CombatManager.OnRepairCrewUsedOnShipPhaserSection.AddListener(repairSendRepairCrewUsedOnShipPhaserSectionMessageAction);
		CombatManager.OnRepairCrewUsedOnShipTorpedoSection.AddListener(repairSendRepairCrewUsedOnShipTorpedoSectionMessageAction);
		CombatManager.OnRepairCrewUsedOnShipStorageSection.AddListener(repairSendRepairCrewUsedOnShipStorageSectionMessageAction);
		CombatManager.OnRepairCrewUsedOnShipCrewSection.AddListener(repairSendRepairCrewUsedOnShipCrewSectionMessageAction);
		CombatManager.OnRepairCrewUsedOnShipEngineSection.AddListener(repairSendRepairCrewUsedOnShipEngineSectionMessageAction);

		//add listeners for using repair crews on bases
		CombatManager.OnRepairCrewUsedOnBasePhaserSection1.AddListener(repairSendRepairCrewUsedOnBasePhaserSection1MessageAction);
		CombatManager.OnRepairCrewUsedOnBasePhaserSection2.AddListener(repairSendRepairCrewUsedOnBasePhaserSection2MessageAction);
		CombatManager.OnRepairCrewUsedOnBaseTorpedoSection.AddListener(repairSendRepairCrewUsedOnBaseTorpedoSectionMessageAction);
		CombatManager.OnRepairCrewUsedOnBaseCrewSection.AddListener(repairSendRepairCrewUsedOnBaseCrewSectionMessageAction);
		CombatManager.OnRepairCrewUsedOnBaseStorageSection1.AddListener(repairSendRepairCrewUsedOnBaseStorageSection1MessageAction);
		CombatManager.OnRepairCrewUsedOnBaseStorageSection2.AddListener(repairSendRepairCrewUsedOnBaseStorageSection2MessageAction);

		//add listeners for ship sections being destroyed
		PhaserSection.OnPhaserSectionDestroyed.AddListener(combatUnitSendShipPhaserSectionDestroyedMessageAction);
		TorpedoSection.OnTorpedoSectionDestroyed.AddListener(combatUnitSendShipTorpedoSectionDestroyedMessageAction);
		StorageSection.OnStorageSectionDestroyed.AddListener(combatUnitSendShipStorageSectionDestroyedMessageAction);
		CrewSection.OnCrewSectionDestroyed.AddListener(combatUnitSendShipCrewSectionDestroyedMessageAction);
		EngineSection.OnEngineSectionDestroyed.AddListener(combatUnitSendShipEngineSectionDestroyedMessageAction);

		//add listeners for base sections being destroyed
		StarbasePhaserSection1.OnPhaserSection1Destroyed.AddListener(combatUnitSendBasePhaserSection1DestroyedMessageAction);
		StarbasePhaserSection2.OnPhaserSection2Destroyed.AddListener(combatUnitSendBasePhaserSection2DestroyedMessageAction);
		StarbaseTorpedoSection.OnTorpedoSectionDestroyed.AddListener(combatUnitSendBaseTorpedoSectionDestroyedMessageAction);
		StarbaseCrewSection.OnCrewSectionDestroyed.AddListener(combatUnitSendBaseCrewSectionDestroyedMessageAction);
		StarbaseStorageSection1.OnStorageSection1Destroyed.AddListener(combatUnitSendBaseStorageSection1DestroyedMessageAction);
		StarbaseStorageSection2.OnStorageSection2Destroyed.AddListener(combatUnitSendBaseStorageSection2DestroyedMessageAction);

		//add listener for ship being destroyed
		Ship.OnShipDestroyed.AddListener(combatUnitSendShipDestroyedMessageAction);

		//add listener for base being destroyed
		Starbase.OnBaseDestroyed.AddListener(combatUnitSendBaseDestroyedMessageAction);

		//add listener for ship being renamed
		Ship.OnShipRenamed.AddListener(combatUnitSendUnitRenamedMessageAction);

		//add listener for base being renamed
		Starbase.OnBaseRenamed.AddListener(combatUnitSendUnitRenamedMessageAction);

		//add listener for income collection
		Player.OnCollectIncome.AddListener(playerSendIncomeMessageAction);

		//add listener for planet owner change
		ColonyManager.OnPlanetOwnerChanged.AddListener(playerSendPlanetColonizedMessageAction);

		//add listener for purchasing items
		uiManager.GetComponent<PurchaseManager>().OnPurchaseItems.AddListener(purchaseSendPurchaseMessageAction);

		//add listener for purchasing a ship
		uiManager.GetComponent<NameNewShip>().OnPurchasedNewShip.AddListener(purchaseShipSendPurchaseMessageAction);

		//add listener for sunburst damage
		Sunburst.OnSunburstDamageDealt.AddListener(combatUnitSendSunburstDamageMessageAction);

		//add listener for loaded game
		gameManager.OnLoadedTurn.AddListener(playerSendLoadedTurnAction);

		//add listener for saved game
		uiManager.GetComponent<FileManager>().OnSendSaveGameDataFromSave.AddListener(saveDataSendSavedGameAction);

		//add listener for deleted file
		uiManager.GetComponent<FileLoadWindow>().OnFileConfirmedDelete.AddListener(stringSendDeletedFileMessageAction);

		//add listener for player being eliminated
		gameManager.OnKillPlayer.AddListener(killPlayerAction);

	}


	//this function adds a string to the text log
	private void AddMessageToLog(string newMessage){

		//only add message if loadedGame flag is false
		if (GameManager.loadedGame == false) {

			//instantiate a text prefab in the chat content
			TextMeshProUGUI newText;
			newText = Instantiate (chatTextPrefab) as TextMeshProUGUI;

			//set parent
			newText.transform.SetParent (chatContent.transform, false);

			//set text for the new text box
			newText.text = newMessage;

			//increment messageLogID
			MessageManager.messageLogID++;

			//if the message ID is greater than the max allowed, we need to get rid of the oldest message
			if (MessageManager.MessageLogID > MessageManager.maxMessages) {

				//destroy the first child of the chat log content
				GameObject.Destroy (chatContent.GetChild (0).gameObject);

			}

			//update the canvas
			Canvas.ForceUpdateCanvases ();

			//scroll chat log to the bottom
			chatLog.verticalNormalizedPosition = 0.0f;

		}

	}

	//this function is a helper function to build rich-text strings for the chat log
	//it takes a string input, and inserts the player color at the front of the string in brackets and in the color
	private string AddPlayerColorToMessage(Player player, string message){

		//build the output message
		string outputMessage = InsertColoredPlayerColor(player) + " " + message;

		//return the output message
		return outputMessage;

	}

    //this function is a helper function to build rich-text strings for the chat log
    //it takes a string input, and inserts the player name at the front of the string in brackets and in the color
    private string AddPlayerNameToMessage(Player player, string message)
    {

        //build the output message
        string outputMessage = InsertColoredPlayerName(player) + " " + message;

        //return the output message
        return outputMessage;

    }

    //this helper function retuns a rich text formatted player color inside brackets
    private string InsertColoredPlayerColor(Player player){

		//convert playerColor to the associated rich text color
		string playerTextColor;

		switch (player.color) {

		case Player.Color.Blue:
			playerTextColor = MessageManager.bluePlayerColor;
			break;
		case Player.Color.Green:
			playerTextColor = MessageManager.greenPlayerColor;
			break;
		case Player.Color.Purple:
			playerTextColor = MessageManager.purplePlayerColor;
			break;
		case Player.Color.Red:
			playerTextColor = MessageManager.redPlayerColor;
			break;
		default:
			playerTextColor = "000000ff";
			break;

		}

		string formattedPlayerColor = "<color=#" + playerTextColor + ">[" + player.color + "]</color>";

		return formattedPlayerColor;

	}

    //this helper function retuns a rich text formatted player name inside brackets
    private string InsertColoredPlayerName(Player player)
    {

        //convert playerColor to the associated rich text color
        string playerTextColor;

        switch (player.color)
        {

            case Player.Color.Blue:
                playerTextColor = MessageManager.bluePlayerColor;
                break;
            case Player.Color.Green:
                playerTextColor = MessageManager.greenPlayerColor;
                break;
            case Player.Color.Purple:
                playerTextColor = MessageManager.purplePlayerColor;
                break;
            case Player.Color.Red:
                playerTextColor = MessageManager.redPlayerColor;
                break;
            default:
                playerTextColor = "000000ff";
                break;

        }

        string formattedPlayerName = "<color=#" + playerTextColor + ">[" + player.playerName + "]</color>";

        return formattedPlayerName;

    }

    private void SendTurnPhaseMessage(){

		//create variable for message log input
		string newMessage;

		//define the message depending on what phase it is
		if (gameManager.currentTurnPhase == GameManager.TurnPhase.PurchasePhase) {

			newMessage = "Begin Purchase Phase";

		} else if (gameManager.currentTurnPhase == GameManager.TurnPhase.MainPhase) {

			newMessage = "Begin Main Phase";

		} else {

			newMessage = "";

		}

		//build the rich text string to pass to the message log
		newMessage = AddPlayerColorToMessage(gameManager.currentTurnPlayer,newMessage);

		//pass the message to the message log
		AddMessageToLog(newMessage);

	}

	//this function will take a chat message string and upload it to the chat log
	private void SendNewChatMessage(string userInput){

		//add the player color prefix to the string
		userInput = AddPlayerNameToMessage(gameManager.currentTurnPlayer,userInput);

		//upload the completed string to the log
		AddMessageToLog(userInput);

	}

	//this function will send a new ship movement message to the chat log
	private void SendNewShipMovementMessage(Ship ship, Hex startingHex, Hex endingHex){

		//this string looks complicated but it's really not
		//I'm converting the movementStartingHex and EndingHex to offset coordinates, then converting those to strings
		string newMessage;

		//I want to check to see if the ship is being towed - if so, we want the message string to read slightly differently
		if (ship.GetComponent<EngineSection>().isBeingTowed == false) {

			newMessage = ship.shipType + " " + ship.shipName + " moves from " + OffsetCoord.OffsetCoordToString (OffsetCoord.RoffsetFromCube (OffsetCoord.ODD, startingHex)) +
			" to " + OffsetCoord.OffsetCoordToString (OffsetCoord.RoffsetFromCube (OffsetCoord.ODD, endingHex));

		}
		//the else condition is that the ship isBeingTowed is true
		else {

			newMessage = ship.shipType + " " + ship.shipName + " is towed by tractor beam from " + OffsetCoord.OffsetCoordToString (OffsetCoord.RoffsetFromCube (OffsetCoord.ODD, startingHex)) +
				" to " + OffsetCoord.OffsetCoordToString (OffsetCoord.RoffsetFromCube (OffsetCoord.ODD, endingHex));

		}
			
		//add the player color prefix to the string
		newMessage = AddPlayerColorToMessage(gameManager.currentTurnPlayer,newMessage);

		//upload the completed string to the log
		AddMessageToLog(newMessage);

	}

	//this function will send a message when the tractor beam is primed
	private void SendTractorBeamPrimedMessage(Ship ship){

		string newMessage = ship.shipType + " " + ship.shipName + " targets " + InsertColoredPlayerColor(mouseManager.targetedUnit.GetComponent<CombatUnit>().owner) + 
			" " + mouseManager.targetedUnit.GetComponent<Ship> ().shipType + " " + mouseManager.targetedUnit.GetComponent<Ship> ().shipName 
			+ " with Tractor Beam";

		//add the player color prefix to the string
		newMessage = AddPlayerColorToMessage(gameManager.currentTurnPlayer,newMessage);

		//upload the completed string to the log
		AddMessageToLog(newMessage);

	}

	//this function will send a message when the tractor beam is engaged and used
	private void SendTractorBeamEngagedMessage(Ship ship){

		string newMessage = ship.shipType + " " + ship.shipName + " engages Tractor Beam on " + InsertColoredPlayerColor(mouseManager.targetedUnit.GetComponent<CombatUnit>().owner) + 
			" " + mouseManager.targetedUnit.GetComponent<Ship> ().shipType + " " + mouseManager.targetedUnit.GetComponent<Ship> ().shipName;

		//add the player color prefix to the string
		newMessage = AddPlayerColorToMessage(gameManager.currentTurnPlayer,newMessage);

		//upload the completed string to the log
		AddMessageToLog(newMessage);

	}

	//this function will send a message when the tractor beam is disengaged
	private void SendTractorBeamDisengagedMessage(Ship ship){

		string newMessage = ship.shipType + " " + ship.shipName + " disengages Tractor Beam";

		//add the player color prefix to the string
		newMessage = AddPlayerColorToMessage(gameManager.currentTurnPlayer,newMessage);

		//upload the completed string to the log
		AddMessageToLog(newMessage);

	}

	//this function will send a message when a ship uses a warp booster
	private void SendUseWarpBoosterMessage(Ship ship){

		string newMessage = ship.shipType + " " + ship.shipName + " uses Warp Booster";

		//add the player color prefix to the string
		newMessage = AddPlayerColorToMessage(gameManager.currentTurnPlayer,newMessage);

		//upload the completed string to the log
		AddMessageToLog(newMessage);

	}

	//this function will send a message when a ship uses a transwarp booster
	private void SendUseTranswarpBoosterMessage(Ship ship){

		string newMessage = ship.shipType + " " + ship.shipName + " uses Transwarp Booster";

		//add the player color prefix to the string
		newMessage = AddPlayerColorToMessage(gameManager.currentTurnPlayer,newMessage);

		//upload the completed string to the log
		AddMessageToLog(newMessage);

	}

	//this function will send a message when a ship attacks with phasers
	private void SendPhaserAttackMessage(CombatUnit attackingUnit, CombatUnit targetedUnit){

		string newMessage = "";

		//check if the attacking unit is a ship
		if (attackingUnit.GetComponent<Ship> () == true) {

			//check if the targeted unit is a ship
			if (targetedUnit.GetComponent <Ship> () == true) {
				
				newMessage = attackingUnit.GetComponent<Ship> ().shipType + " " + attackingUnit.GetComponent<Ship> ().shipName + " fires phasers at " +
				InsertColoredPlayerColor (targetedUnit.GetComponent<CombatUnit> ().owner) +
				" " + targetedUnit.GetComponent<Ship> ().shipType + " " + targetedUnit.GetComponent<Ship> ().shipName;

			}
			//the else condition is that the targeted unit is a base
			else {
				
				newMessage = attackingUnit.GetComponent<Ship> ().shipType + " " + attackingUnit.GetComponent<Ship> ().shipName + " fires phasers at " +
				InsertColoredPlayerColor (targetedUnit.GetComponent<CombatUnit> ().owner) +
				" Starbase " + targetedUnit.GetComponent<Starbase> ().baseName;

			}

		}
		//the else condtion is that the attacking unit is a base
		else {

			//check if the targeted unit is a ship
			if (targetedUnit.GetComponent <Ship> () == true) {

				newMessage = "Starbase " + attackingUnit.GetComponent<Starbase> ().baseName + " fires phasers at " +
					InsertColoredPlayerColor (targetedUnit.GetComponent<CombatUnit> ().owner) +
					" " + targetedUnit.GetComponent<Ship> ().shipType + " " + targetedUnit.GetComponent<Ship> ().shipName;

			}
			//the else condition is that the targeted unit is a base
			else {

				newMessage = "Starbase " + attackingUnit.GetComponent<Starbase> ().baseName + " fires phasers at " +
					InsertColoredPlayerColor (targetedUnit.GetComponent<CombatUnit> ().owner) +
					" Starbase " + targetedUnit.GetComponent<Starbase> ().baseName;

			}

		}

		//add the player color prefix to the string
		newMessage = AddPlayerColorToMessage(gameManager.currentTurnPlayer,newMessage);

		//upload the completed string to the log
		AddMessageToLog(newMessage);

	}

	//this function will send a message when a ship phaser attack hits a ship phaser section
	private void SendPhaserAttackHitShipPhaserSectionMessage(CombatUnit attackingUnit, CombatUnit targetedUnit, int damage){

		string newMessage = "";

		//check if the attacking unit is a ship
		if (attackingUnit.GetComponent<Ship> () == true) {

			newMessage = attackingUnit.GetComponent<Ship> ().shipType + " " + attackingUnit.GetComponent<Ship> ().shipName + " hits " +
				InsertColoredPlayerColor (targetedUnit.GetComponent<CombatUnit> ().owner) +
				" " + targetedUnit.GetComponent<Ship> ().shipType + " " + targetedUnit.GetComponent<Ship> ().shipName
				+ " Phaser Section and deals " + damage + " damage!";

			//add the player color prefix to the string
			newMessage = AddPlayerColorToMessage (gameManager.currentTurnPlayer, newMessage);

			//upload the completed string to the log
			AddMessageToLog (newMessage);

		}

		else if(attackingUnit.GetComponent<Starbase> () == true) {

			newMessage = "Starbase " + attackingUnit.GetComponent<Starbase> ().baseName + " hits " +
				InsertColoredPlayerColor (targetedUnit.GetComponent<CombatUnit> ().owner) +
				" " + targetedUnit.GetComponent<Ship> ().shipType + " " + targetedUnit.GetComponent<Ship> ().shipName
				+ " Phaser Section and deals " + damage + " damage!";

			//add the player color prefix to the string
			newMessage = AddPlayerColorToMessage (gameManager.currentTurnPlayer, newMessage);

			//upload the completed string to the log
			AddMessageToLog (newMessage);

		}

	}

	//this function will send a message when a ship phaser attack hits a ship torpedo section
	private void SendPhaserAttackHitShipTorpedoSectionMessage(CombatUnit attackingUnit, CombatUnit targetedUnit, int damage){

		string newMessage = "";

		//check if the attacking unit is a ship
		if (attackingUnit.GetComponent<Ship> () == true) {

			newMessage = attackingUnit.GetComponent<Ship> ().shipType + " " + attackingUnit.GetComponent<Ship> ().shipName + " hits " +
				InsertColoredPlayerColor (targetedUnit.GetComponent<CombatUnit> ().owner) +
				" " + targetedUnit.GetComponent<Ship> ().shipType + " " + targetedUnit.GetComponent<Ship> ().shipName
				+ " Torpedo Section and deals " + damage + " damage!";

			//add the player color prefix to the string
			newMessage = AddPlayerColorToMessage (gameManager.currentTurnPlayer, newMessage);

			//upload the completed string to the log
			AddMessageToLog (newMessage);

		}

		else if(attackingUnit.GetComponent<Starbase> () == true) {

			newMessage = "Starbase " + attackingUnit.GetComponent<Starbase> ().baseName + " hits " +
				InsertColoredPlayerColor (targetedUnit.GetComponent<CombatUnit> ().owner) +
				" " + targetedUnit.GetComponent<Ship> ().shipType + " " + targetedUnit.GetComponent<Ship> ().shipName
				+ " Torpedo Section and deals " + damage + " damage!";

			//add the player color prefix to the string
			newMessage = AddPlayerColorToMessage (gameManager.currentTurnPlayer, newMessage);

			//upload the completed string to the log
			AddMessageToLog (newMessage);

		}

	}

	//this function will send a message when a ship phaser attack hits a ship storage section
	private void SendPhaserAttackHitShipStorageSectionMessage(CombatUnit attackingUnit, CombatUnit targetedUnit, int damage){

		string newMessage = "";

		//check if the attacking unit is a ship
		if (attackingUnit.GetComponent<Ship> () == true) {

			newMessage = attackingUnit.GetComponent<Ship> ().shipType + " " + attackingUnit.GetComponent<Ship> ().shipName + " hits " +
				InsertColoredPlayerColor (targetedUnit.GetComponent<CombatUnit> ().owner) +
				" " + targetedUnit.GetComponent<Ship> ().shipType + " " + targetedUnit.GetComponent<Ship> ().shipName
				+ " Storage Section and deals " + damage + " damage!";

			//add the player color prefix to the string
			newMessage = AddPlayerColorToMessage (gameManager.currentTurnPlayer, newMessage);

			//upload the completed string to the log
			AddMessageToLog (newMessage);

		}

		else if(attackingUnit.GetComponent<Starbase> () == true) {

			newMessage = "Starbase " + attackingUnit.GetComponent<Starbase> ().baseName + " hits " +
				InsertColoredPlayerColor (targetedUnit.GetComponent<CombatUnit> ().owner) +
				" " + targetedUnit.GetComponent<Ship> ().shipType + " " + targetedUnit.GetComponent<Ship> ().shipName
				+ " Storage Section and deals " + damage + " damage!";

			//add the player color prefix to the string
			newMessage = AddPlayerColorToMessage (gameManager.currentTurnPlayer, newMessage);

			//upload the completed string to the log
			AddMessageToLog (newMessage);

		}

	}

	//this function will send a message when a ship phaser attack hits a ship crew section
	private void SendPhaserAttackHitShipCrewSectionMessage(CombatUnit attackingUnit, CombatUnit targetedUnit, int damage){

		string newMessage = "";

		//check if the attacking unit is a ship
		if (attackingUnit.GetComponent<Ship> () == true) {

			newMessage = attackingUnit.GetComponent<Ship> ().shipType + " " + attackingUnit.GetComponent<Ship> ().shipName + " hits " +
				InsertColoredPlayerColor (targetedUnit.GetComponent<CombatUnit> ().owner) +
				" " + targetedUnit.GetComponent<Ship> ().shipType + " " + targetedUnit.GetComponent<Ship> ().shipName
				+ " Crew Section and deals " + damage + " damage!";

			//add the player color prefix to the string
			newMessage = AddPlayerColorToMessage (gameManager.currentTurnPlayer, newMessage);

			//upload the completed string to the log
			AddMessageToLog (newMessage);

		}

		else if(attackingUnit.GetComponent<Starbase> () == true) {

			newMessage = "Starbase " + attackingUnit.GetComponent<Starbase> ().baseName + " hits " +
				InsertColoredPlayerColor (targetedUnit.GetComponent<CombatUnit> ().owner) +
				" " + targetedUnit.GetComponent<Ship> ().shipType + " " + targetedUnit.GetComponent<Ship> ().shipName
				+ " Crew Section and deals " + damage + " damage!";

			//add the player color prefix to the string
			newMessage = AddPlayerColorToMessage (gameManager.currentTurnPlayer, newMessage);

			//upload the completed string to the log
			AddMessageToLog (newMessage);

		}

	}

	//this function will send a message when a ship phaser attack hits a ship engine section
	private void SendPhaserAttackHitShipEngineSectionMessage(CombatUnit attackingUnit, CombatUnit targetedUnit, int damage){

		string newMessage = "";

		//check if the attacking unit is a ship
		if (attackingUnit.GetComponent<Ship> () == true) {

			newMessage = attackingUnit.GetComponent<Ship> ().shipType + " " + attackingUnit.GetComponent<Ship> ().shipName + " hits " +
				InsertColoredPlayerColor (targetedUnit.GetComponent<CombatUnit> ().owner) +
				" " + targetedUnit.GetComponent<Ship> ().shipType + " " + targetedUnit.GetComponent<Ship> ().shipName
				+ " Engine Section and deals " + damage + " damage!";

			//add the player color prefix to the string
			newMessage = AddPlayerColorToMessage (gameManager.currentTurnPlayer, newMessage);

			//upload the completed string to the log
			AddMessageToLog (newMessage);

		}

		else if(attackingUnit.GetComponent<Starbase> () == true) {

			newMessage = "Starbase " + attackingUnit.GetComponent<Starbase> ().baseName + " hits " +
				InsertColoredPlayerColor (targetedUnit.GetComponent<CombatUnit> ().owner) +
				" " + targetedUnit.GetComponent<Ship> ().shipType + " " + targetedUnit.GetComponent<Ship> ().shipName
				+ " Engine Section and deals " + damage + " damage!";

			//add the player color prefix to the string
			newMessage = AddPlayerColorToMessage (gameManager.currentTurnPlayer, newMessage);

			//upload the completed string to the log
			AddMessageToLog (newMessage);

		}

	}

	//this function will send a message when a ship phaser attack misses
	private void SendPhaserAttackMissMessage(CombatUnit attackingUnit, CombatUnit targetedUnit){

		string newMessage = "Phaser attack misses!";

		//add the player color prefix to the string
		newMessage = AddPlayerColorToMessage(gameManager.currentTurnPlayer,newMessage);

		//upload the completed string to the log
		AddMessageToLog(newMessage);

	}

	//this function will send a message when a phaser attack hits a base phaser section 1
	private void SendPhaserAttackHitBasePhaserSection1Message(CombatUnit attackingUnit, CombatUnit targetedUnit, int damage){

		string newMessage = "";

		//check if the attacking unit is a ship
		if (attackingUnit.GetComponent<Ship> () == true) {

			newMessage = attackingUnit.GetComponent<Ship> ().shipType + " " + attackingUnit.GetComponent<Ship> ().shipName + " hits " +
			                   InsertColoredPlayerColor (targetedUnit.GetComponent<CombatUnit> ().owner) +
			                   " Starbase " + targetedUnit.GetComponent<Starbase> ().baseName
			                   + " Phaser Section 1 and deals " + damage + " damage!";

			//add the player color prefix to the string
			newMessage = AddPlayerColorToMessage (gameManager.currentTurnPlayer, newMessage);

			//upload the completed string to the log
			AddMessageToLog (newMessage);

		}

		else if(attackingUnit.GetComponent<Starbase> () == true) {

			newMessage = "Starbase " + attackingUnit.GetComponent<Starbase> ().baseName + " hits " +
				InsertColoredPlayerColor (targetedUnit.GetComponent<CombatUnit> ().owner) +
				" Starbase " + targetedUnit.GetComponent<Starbase> ().baseName
				+ " Phaser Section 1 and deals " + damage + " damage!";

			//add the player color prefix to the string
			newMessage = AddPlayerColorToMessage (gameManager.currentTurnPlayer, newMessage);

			//upload the completed string to the log
			AddMessageToLog (newMessage);

		}

	}

	//this function will send a message when a phaser attack hits a base phaser section 2
	private void SendPhaserAttackHitBasePhaserSection2Message(CombatUnit attackingUnit, CombatUnit targetedUnit, int damage){

		string newMessage = "";

		//check if the attacking unit is a ship
		if (attackingUnit.GetComponent<Ship> () == true) {

			newMessage = attackingUnit.GetComponent<Ship> ().shipType + " " + attackingUnit.GetComponent<Ship> ().shipName + " hits " +
				InsertColoredPlayerColor (targetedUnit.GetComponent<CombatUnit> ().owner) +
				" Starbase " + targetedUnit.GetComponent<Starbase> ().baseName
				+ " Phaser Section 2 and deals " + damage + " damage!";

			//add the player color prefix to the string
			newMessage = AddPlayerColorToMessage (gameManager.currentTurnPlayer, newMessage);

			//upload the completed string to the log
			AddMessageToLog (newMessage);

		}

		else if(attackingUnit.GetComponent<Starbase> () == true) {

			newMessage = "Starbase " + attackingUnit.GetComponent<Starbase> ().baseName + " hits " +
				InsertColoredPlayerColor (targetedUnit.GetComponent<CombatUnit> ().owner) +
				" Starbase " + targetedUnit.GetComponent<Starbase> ().baseName
				+ " Phaser Section 2 and deals " + damage + " damage!";

			//add the player color prefix to the string
			newMessage = AddPlayerColorToMessage (gameManager.currentTurnPlayer, newMessage);

			//upload the completed string to the log
			AddMessageToLog (newMessage);

		}

	}

	//this function will send a message when a phaser attack hits a base torpedo section
	private void SendPhaserAttackHitBaseTorpedoSectionMessage(CombatUnit attackingUnit, CombatUnit targetedUnit, int damage){

		string newMessage = "";

		//check if the attacking unit is a ship
		if (attackingUnit.GetComponent<Ship> () == true) {

			newMessage = attackingUnit.GetComponent<Ship> ().shipType + " " + attackingUnit.GetComponent<Ship> ().shipName + " hits " +
				InsertColoredPlayerColor (targetedUnit.GetComponent<CombatUnit> ().owner) +
				" Starbase " + targetedUnit.GetComponent<Starbase> ().baseName
				+ " Torpedo Section and deals " + damage + " damage!";

			//add the player color prefix to the string
			newMessage = AddPlayerColorToMessage (gameManager.currentTurnPlayer, newMessage);

			//upload the completed string to the log
			AddMessageToLog (newMessage);

		}

		else if(attackingUnit.GetComponent<Starbase> () == true) {

			newMessage = "Starbase " + attackingUnit.GetComponent<Starbase> ().baseName + " hits " +
				InsertColoredPlayerColor (targetedUnit.GetComponent<CombatUnit> ().owner) +
				" Starbase " + targetedUnit.GetComponent<Starbase> ().baseName
				+ " Torpedo Section and deals " + damage + " damage!";

			//add the player color prefix to the string
			newMessage = AddPlayerColorToMessage (gameManager.currentTurnPlayer, newMessage);

			//upload the completed string to the log
			AddMessageToLog (newMessage);

		}

	}

	//this function will send a message when a phaser attack hits a base crew section
	private void SendPhaserAttackHitBaseCrewSectionMessage(CombatUnit attackingUnit, CombatUnit targetedUnit, int damage){

		string newMessage = "";

		//check if the attacking unit is a ship
		if (attackingUnit.GetComponent<Ship> () == true) {

			newMessage = attackingUnit.GetComponent<Ship> ().shipType + " " + attackingUnit.GetComponent<Ship> ().shipName + " hits " +
				InsertColoredPlayerColor (targetedUnit.GetComponent<CombatUnit> ().owner) +
				" Starbase " + targetedUnit.GetComponent<Starbase> ().baseName
				+ " Crew Section and deals " + damage + " damage!";

			//add the player color prefix to the string
			newMessage = AddPlayerColorToMessage (gameManager.currentTurnPlayer, newMessage);

			//upload the completed string to the log
			AddMessageToLog (newMessage);

		}

		else if(attackingUnit.GetComponent<Starbase> () == true) {

			newMessage = "Starbase " + attackingUnit.GetComponent<Starbase> ().baseName + " hits " +
				InsertColoredPlayerColor (targetedUnit.GetComponent<CombatUnit> ().owner) +
				" Starbase " + targetedUnit.GetComponent<Starbase> ().baseName
				+ " Crew Section and deals " + damage + " damage!";

			//add the player color prefix to the string
			newMessage = AddPlayerColorToMessage (gameManager.currentTurnPlayer, newMessage);

			//upload the completed string to the log
			AddMessageToLog (newMessage);

		}

	}

	//this function will send a message when a phaser attack hits a base storage section 1
	private void SendPhaserAttackHitBaseStorageSection1Message(CombatUnit attackingUnit, CombatUnit targetedUnit, int damage){

		string newMessage = "";

		//check if the attacking unit is a ship
		if (attackingUnit.GetComponent<Ship> () == true) {

			newMessage = attackingUnit.GetComponent<Ship> ().shipType + " " + attackingUnit.GetComponent<Ship> ().shipName + " hits " +
				InsertColoredPlayerColor (targetedUnit.GetComponent<CombatUnit> ().owner) +
				" Starbase " + targetedUnit.GetComponent<Starbase> ().baseName
				+ " Storage Section 1 and deals " + damage + " damage!";

			//add the player color prefix to the string
			newMessage = AddPlayerColorToMessage (gameManager.currentTurnPlayer, newMessage);

			//upload the completed string to the log
			AddMessageToLog (newMessage);

		}

		else if(attackingUnit.GetComponent<Starbase> () == true) {

			newMessage = "Starbase " + attackingUnit.GetComponent<Starbase> ().baseName + " hits " +
				InsertColoredPlayerColor (targetedUnit.GetComponent<CombatUnit> ().owner) +
				" Starbase " + targetedUnit.GetComponent<Starbase> ().baseName
				+ " Storage Section 1 and deals " + damage + " damage!";

			//add the player color prefix to the string
			newMessage = AddPlayerColorToMessage (gameManager.currentTurnPlayer, newMessage);

			//upload the completed string to the log
			AddMessageToLog (newMessage);

		}

	}

	//this function will send a message when a phaser attack hits a base storage section 2
	private void SendPhaserAttackHitBaseStorageSection2Message(CombatUnit attackingUnit, CombatUnit targetedUnit, int damage){

		string newMessage = "";

		//check if the attacking unit is a ship
		if (attackingUnit.GetComponent<Ship> () == true) {

			newMessage = attackingUnit.GetComponent<Ship> ().shipType + " " + attackingUnit.GetComponent<Ship> ().shipName + " hits " +
				InsertColoredPlayerColor (targetedUnit.GetComponent<CombatUnit> ().owner) +
				" Starbase " + targetedUnit.GetComponent<Starbase> ().baseName
				+ " Storage Section 2 and deals " + damage + " damage!";

			//add the player color prefix to the string
			newMessage = AddPlayerColorToMessage (gameManager.currentTurnPlayer, newMessage);

			//upload the completed string to the log
			AddMessageToLog (newMessage);

		}

		else if(attackingUnit.GetComponent<Starbase> () == true) {

			newMessage = "Starbase " + attackingUnit.GetComponent<Starbase> ().baseName + " hits " +
				InsertColoredPlayerColor (targetedUnit.GetComponent<CombatUnit> ().owner) +
				" Starbase " + targetedUnit.GetComponent<Starbase> ().baseName
				+ " Storage Section 2 and deals " + damage + " damage!";

			//add the player color prefix to the string
			newMessage = AddPlayerColorToMessage (gameManager.currentTurnPlayer, newMessage);

			//upload the completed string to the log
			AddMessageToLog (newMessage);

		}

	}

	//this function will send a message when a ship attacks with torpedos
	private void SendTorpedoAttackMessage(CombatUnit attackingUnit, CombatUnit targetedUnit, string torpedoType){

		string newMessage = "";

		//check if attacking unit is a ship
		if (attackingUnit.GetComponent<Ship> () == true) {

			//check if the targeted umit is a ship
			if (targetedUnit.GetComponent<Ship>() == true) {

				newMessage = attackingUnit.GetComponent<Ship> ().shipType + " " + attackingUnit.GetComponent<Ship> ().shipName + " fires " + torpedoType +
				" Torpedo at " + InsertColoredPlayerColor (targetedUnit.GetComponent<CombatUnit> ().owner) +
				" " + targetedUnit.GetComponent<Ship> ().shipType + " " + targetedUnit.GetComponent<Ship> ().shipName;

			}
			//the else condition is that the targeted unit is a base
			else {

				newMessage = attackingUnit.GetComponent<Ship> ().shipType + " " + attackingUnit.GetComponent<Ship> ().shipName + " fires " + torpedoType +
				" Torpedo at " + InsertColoredPlayerColor (targetedUnit.GetComponent<CombatUnit> ().owner) +
				" Starbase " + targetedUnit.GetComponent<Starbase> ().baseName;

			}

		}
		//the else condition is that the attacking unit is a base
		else {

			//check if the targeted umit is a ship
			if (targetedUnit.GetComponent<Ship>() == true) {

				newMessage = "Starbase " + attackingUnit.GetComponent<Starbase> ().baseName + " fires " + torpedoType +
					" Torpedo at " + InsertColoredPlayerColor (targetedUnit.GetComponent<CombatUnit> ().owner) +
					" " + targetedUnit.GetComponent<Ship> ().shipType + " " + targetedUnit.GetComponent<Ship> ().shipName;

			}
			//the else condition is that the targeted unit is a base
			else {

				newMessage = "Starbase " + attackingUnit.GetComponent<Starbase> ().baseName + " fires " + torpedoType +
					" Torpedo at " + InsertColoredPlayerColor (targetedUnit.GetComponent<CombatUnit> ().owner) +
					" Starbase " + targetedUnit.GetComponent<Starbase> ().baseName;

			}

		}

		//add the player color prefix to the string
		newMessage = AddPlayerColorToMessage(gameManager.currentTurnPlayer,newMessage);

		//upload the completed string to the log
		AddMessageToLog(newMessage);

	}

	//this function will send a message when a ship deploys defensive flares
	private void SendUsedFlaresYesMessage(CombatUnit targetedUnit, int numberFlares){

		string newMessage;

		//check if the targeted unit is a ship
		if (targetedUnit.GetComponent<Ship> () == true) {

			//the grammar is slightly different if we have 1 flare vs >1 flares
			if (numberFlares == 1) {
			
				newMessage = InsertColoredPlayerColor (targetedUnit.GetComponent<CombatUnit> ().owner) + " " + targetedUnit.GetComponent<Ship> ().shipType +
				" " + targetedUnit.GetComponent<Ship> ().shipName + " uses " + numberFlares.ToString () + " Flare";

			} else {

				newMessage = InsertColoredPlayerColor (targetedUnit.GetComponent<CombatUnit> ().owner) + " " + targetedUnit.GetComponent<Ship> ().shipType +
				" " + targetedUnit.GetComponent<Ship> ().shipName + " uses " + numberFlares.ToString () + " Flares";

			}

		}
		//the else condition is that the targeted unit is a base
		else {

			//the grammar is slightly different if we have 1 flare vs >1 flares
			if (numberFlares == 1) {

				newMessage = InsertColoredPlayerColor (targetedUnit.GetComponent<CombatUnit> ().owner) + " Starbase " +
					targetedUnit.GetComponent<Starbase> ().baseName + " uses " + numberFlares.ToString () + " Flare";

			} else {

				newMessage = InsertColoredPlayerColor (targetedUnit.GetComponent<CombatUnit> ().owner) + " Starbase " +
					targetedUnit.GetComponent<Starbase> ().baseName + " uses " + numberFlares.ToString () + " Flares";

			}

		}

		//upload the completed string to the log
		AddMessageToLog(newMessage);

	}

	//this function will send a message when a ship chooses not to deploy defensive flares
	private void SendUsedFlaresNoMessage(CombatUnit targetedUnit){

		string newMessage;

		//check if targeted unit is a ship
		if (targetedUnit.GetComponent<Ship> () == true) {

			newMessage = InsertColoredPlayerColor (targetedUnit.GetComponent<CombatUnit> ().owner) + " " + targetedUnit.GetComponent<Ship> ().shipType +
			" " + targetedUnit.GetComponent<Ship> ().shipName + " does not use any flares";

		}
		//the else condition is that the targeted unit is a base
		else {

			newMessage = InsertColoredPlayerColor (targetedUnit.GetComponent<CombatUnit> ().owner) + " Starbase " +
				targetedUnit.GetComponent<Starbase> ().baseName + " does not use any flares";

		}

		//upload the completed string to the log
		AddMessageToLog(newMessage);

	}

	//this function will send a message when flares defeat an incoming torpedo
	private void SendFlaresDefeatTorpedoMessage(string torpedoType){

		string newMessage = "Flares defeat " + torpedoType + " Torpedo!";

		//upload the completed string to the log
		AddMessageToLog(newMessage);

	}

	//this function will send a message when flares fail to defeat an incoming torpedo
	private void SendFlaresFailMessage(string torpedoType){

		string newMessage = "Flares fail to stop " + torpedoType + " Torpedo!";

		//upload the completed string to the log
		AddMessageToLog(newMessage);

	}

	//this function will send a message when a ship torpedo attack hits a ship phaser section
	private void SendTorpedoAttackHitShipPhaserSectionMessage(CombatUnit attackingUnit, CombatUnit targetedUnit, int damage, string torpedoType){

		string newMessage = "";

		//check if the attacking unit is a ship
		if (attackingUnit.GetComponent<Ship> () == true) {

			newMessage = attackingUnit.GetComponent<Ship> ().shipType + " " + attackingUnit.GetComponent<Ship> ().shipName + " hits " +
			InsertColoredPlayerColor (targetedUnit.GetComponent<CombatUnit> ().owner) +
			" " + targetedUnit.GetComponent<Ship> ().shipType + " " + targetedUnit.GetComponent<Ship> ().shipName
			+ " Phaser Section with " + torpedoType + " Torpedo and deals " + damage + " damage!";

		}
		//the else condition is that the attacking unit is a base
		else {

			newMessage = "Starbase " + attackingUnit.GetComponent<Starbase> ().baseName + " hits " +
				InsertColoredPlayerColor (targetedUnit.GetComponent<CombatUnit> ().owner) +
				" " + targetedUnit.GetComponent<Ship> ().shipType + " " + targetedUnit.GetComponent<Ship> ().shipName
				+ " Phaser Section with " + torpedoType + " Torpedo and deals " + damage + " damage!";

		}

		//add the player color prefix to the string
		newMessage = AddPlayerColorToMessage(gameManager.currentTurnPlayer,newMessage);

		//upload the completed string to the log
		AddMessageToLog(newMessage);

	}

	//this function will send a message when a ship torpedo attack hits a ship torpedo section
	private void SendTorpedoAttackHitShipTorpedoSectionMessage(CombatUnit attackingUnit, CombatUnit targetedUnit, int damage, string torpedoType){

		string newMessage = "";

		//check if the attacking unit is a ship
		if (attackingUnit.GetComponent<Ship> () == true) {

			newMessage = attackingUnit.GetComponent<Ship> ().shipType + " " + attackingUnit.GetComponent<Ship> ().shipName + " hits " +
				InsertColoredPlayerColor (targetedUnit.GetComponent<CombatUnit> ().owner) +
				" " + targetedUnit.GetComponent<Ship> ().shipType + " " + targetedUnit.GetComponent<Ship> ().shipName
				+ " Torpedo Section with " + torpedoType + " Torpedo and deals " + damage + " damage!";

		}
		//the else condition is that the attacking unit is a base
		else {

			newMessage = "Starbase " + attackingUnit.GetComponent<Starbase> ().baseName + " hits " +
				InsertColoredPlayerColor (targetedUnit.GetComponent<CombatUnit> ().owner) +
				" " + targetedUnit.GetComponent<Ship> ().shipType + " " + targetedUnit.GetComponent<Ship> ().shipName
				+ " Torpedo Section with " + torpedoType + " Torpedo and deals " + damage + " damage!";

		}

		//add the player color prefix to the string
		newMessage = AddPlayerColorToMessage(gameManager.currentTurnPlayer,newMessage);

		//upload the completed string to the log
		AddMessageToLog(newMessage);


	}

	//this function will send a message when a ship torpedo attack hits a ship storage section
	private void SendTorpedoAttackHitShipStorageSectionMessage(CombatUnit attackingUnit, CombatUnit targetedUnit, int damage, string torpedoType){

		string newMessage = "";

		//check if the attacking unit is a ship
		if (attackingUnit.GetComponent<Ship> () == true) {

			newMessage = attackingUnit.GetComponent<Ship> ().shipType + " " + attackingUnit.GetComponent<Ship> ().shipName + " hits " +
				InsertColoredPlayerColor (targetedUnit.GetComponent<CombatUnit> ().owner) +
				" " + targetedUnit.GetComponent<Ship> ().shipType + " " + targetedUnit.GetComponent<Ship> ().shipName
				+ " Storage Section with " + torpedoType + " Torpedo and deals " + damage + " damage!";

		}
		//the else condition is that the attacking unit is a base
		else {

			newMessage = "Starbase " + attackingUnit.GetComponent<Starbase> ().baseName + " hits " +
				InsertColoredPlayerColor (targetedUnit.GetComponent<CombatUnit> ().owner) +
				" " + targetedUnit.GetComponent<Ship> ().shipType + " " + targetedUnit.GetComponent<Ship> ().shipName
				+ " Storage Section with " + torpedoType + " Torpedo and deals " + damage + " damage!";

		}

		//add the player color prefix to the string
		newMessage = AddPlayerColorToMessage(gameManager.currentTurnPlayer,newMessage);

		//upload the completed string to the log
		AddMessageToLog(newMessage);


	}

	//this function will send a message when a ship torpedo attack hits a ship crew section
	private void SendTorpedoAttackHitShipCrewSectionMessage(CombatUnit attackingUnit, CombatUnit targetedUnit, int damage, string torpedoType){

		string newMessage = "";

		//check if the attacking unit is a ship
		if (attackingUnit.GetComponent<Ship> () == true) {

			newMessage = attackingUnit.GetComponent<Ship> ().shipType + " " + attackingUnit.GetComponent<Ship> ().shipName + " hits " +
				InsertColoredPlayerColor (targetedUnit.GetComponent<CombatUnit> ().owner) +
				" " + targetedUnit.GetComponent<Ship> ().shipType + " " + targetedUnit.GetComponent<Ship> ().shipName
				+ " Crew Section with " + torpedoType + " Torpedo and deals " + damage + " damage!";

		}
		//the else condition is that the attacking unit is a base
		else {

			newMessage = "Starbase " + attackingUnit.GetComponent<Starbase> ().baseName + " hits " +
				InsertColoredPlayerColor (targetedUnit.GetComponent<CombatUnit> ().owner) +
				" " + targetedUnit.GetComponent<Ship> ().shipType + " " + targetedUnit.GetComponent<Ship> ().shipName
				+ " Crew Section with " + torpedoType + " Torpedo and deals " + damage + " damage!";

		}

		//add the player color prefix to the string
		newMessage = AddPlayerColorToMessage(gameManager.currentTurnPlayer,newMessage);

		//upload the completed string to the log
		AddMessageToLog(newMessage);


	}

	//this function will send a message when a ship torpedo attack hits a ship torpedo section
	private void SendTorpedoAttackHitShipEngineSectionMessage(CombatUnit attackingUnit, CombatUnit targetedUnit, int damage, string torpedoType){

		string newMessage = "";

		//check if the attacking unit is a ship
		if (attackingUnit.GetComponent<Ship> () == true) {

			newMessage = attackingUnit.GetComponent<Ship> ().shipType + " " + attackingUnit.GetComponent<Ship> ().shipName + " hits " +
				InsertColoredPlayerColor (targetedUnit.GetComponent<CombatUnit> ().owner) +
				" " + targetedUnit.GetComponent<Ship> ().shipType + " " + targetedUnit.GetComponent<Ship> ().shipName
				+ " Engine Section with " + torpedoType + " Torpedo and deals " + damage + " damage!";

		}
		//the else condition is that the attacking unit is a base
		else {

			newMessage = "Starbase " + attackingUnit.GetComponent<Starbase> ().baseName + " hits " +
				InsertColoredPlayerColor (targetedUnit.GetComponent<CombatUnit> ().owner) +
				" " + targetedUnit.GetComponent<Ship> ().shipType + " " + targetedUnit.GetComponent<Ship> ().shipName
				+ " Engine Section with " + torpedoType + " Torpedo and deals " + damage + " damage!";

		}

		//add the player color prefix to the string
		newMessage = AddPlayerColorToMessage(gameManager.currentTurnPlayer,newMessage);

		//upload the completed string to the log
		AddMessageToLog(newMessage);


	}

	//this function will send a message when a ship torpedo attack hits a base phaser section 1
	private void SendTorpedoAttackHitBasePhaserSection1Message(CombatUnit attackingUnit, CombatUnit targetedUnit, int damage, string torpedoType){

		string newMessage = "";

		//check if the attacking unit is a ship
		if (attackingUnit.GetComponent<Ship> () == true) {

			newMessage = attackingUnit.GetComponent<Ship> ().shipType + " " + attackingUnit.GetComponent<Ship> ().shipName + " hits " +
				InsertColoredPlayerColor (targetedUnit.GetComponent<CombatUnit> ().owner) +
				" Starbase " + targetedUnit.GetComponent<Starbase> ().baseName
				+ " Phaser Section 1 with " + torpedoType + " Torpedo and deals " + damage + " damage!";

		}
		//the else condition is that the attacking unit is a base
		else {

			newMessage = "Starbase " + attackingUnit.GetComponent<Starbase> ().baseName + " hits " +
				InsertColoredPlayerColor (targetedUnit.GetComponent<CombatUnit> ().owner) +
				" Starbase " + targetedUnit.GetComponent<Starbase> ().baseName
				+ " Phaser Section 1 with " + torpedoType + " Torpedo and deals " + damage + " damage!";

		}

		//add the player color prefix to the string
		newMessage = AddPlayerColorToMessage(gameManager.currentTurnPlayer,newMessage);

		//upload the completed string to the log
		AddMessageToLog(newMessage);

	}

	//this function will send a message when a ship torpedo attack hits a base phaser section 2
	private void SendTorpedoAttackHitBasePhaserSection2Message(CombatUnit attackingUnit, CombatUnit targetedUnit, int damage, string torpedoType){

		string newMessage = "";

		//check if the attacking unit is a ship
		if (attackingUnit.GetComponent<Ship> () == true) {

			newMessage = attackingUnit.GetComponent<Ship> ().shipType + " " + attackingUnit.GetComponent<Ship> ().shipName + " hits " +
				InsertColoredPlayerColor (targetedUnit.GetComponent<CombatUnit> ().owner) +
				" Starbase " + targetedUnit.GetComponent<Starbase> ().baseName
				+ " Phaser Section 2 with " + torpedoType + " Torpedo and deals " + damage + " damage!";

		}
		//the else condition is that the attacking unit is a base
		else {

			newMessage = "Starbase " + attackingUnit.GetComponent<Starbase> ().baseName + " hits " +
				InsertColoredPlayerColor (targetedUnit.GetComponent<CombatUnit> ().owner) +
				" Starbase " + targetedUnit.GetComponent<Starbase> ().baseName
				+ " Phaser Section 2 with " + torpedoType + " Torpedo and deals " + damage + " damage!";

		}

		//add the player color prefix to the string
		newMessage = AddPlayerColorToMessage(gameManager.currentTurnPlayer,newMessage);

		//upload the completed string to the log
		AddMessageToLog(newMessage);

	}

	//this function will send a message when a ship torpedo attack misses
	private void SendTorpedoAttackMissMessage(CombatUnit attackingUnit, CombatUnit targetedUnit, string torpedoType){

		string newMessage = torpedoType + " Torpedo attack misses!";

		//add the player color prefix to the string
		newMessage = AddPlayerColorToMessage(gameManager.currentTurnPlayer,newMessage);

		//upload the completed string to the log
		AddMessageToLog(newMessage);

	}

	//this function will send a message when a ship torpedo attack hits a base torpedo
	private void SendTorpedoAttackHitBaseTorpedoSectionMessage(CombatUnit attackingUnit, CombatUnit targetedUnit, int damage, string torpedoType){

		string newMessage = "";

		//check if the attacking unit is a ship
		if (attackingUnit.GetComponent<Ship> () == true) {

			newMessage = attackingUnit.GetComponent<Ship> ().shipType + " " + attackingUnit.GetComponent<Ship> ().shipName + " hits " +
				InsertColoredPlayerColor (targetedUnit.GetComponent<CombatUnit> ().owner) +
				" Starbase " + targetedUnit.GetComponent<Starbase> ().baseName
				+ " Torpedo Section with " + torpedoType + " Torpedo and deals " + damage + " damage!";

		}
		//the else condition is that the attacking unit is a base
		else {

			newMessage = "Starbase " + attackingUnit.GetComponent<Starbase> ().baseName + " hits " +
				InsertColoredPlayerColor (targetedUnit.GetComponent<CombatUnit> ().owner) +
				" Starbase " + targetedUnit.GetComponent<Starbase> ().baseName
				+ " Torpedo Section with " + torpedoType + " Torpedo and deals " + damage + " damage!";

		}

		//add the player color prefix to the string
		newMessage = AddPlayerColorToMessage(gameManager.currentTurnPlayer,newMessage);

		//upload the completed string to the log
		AddMessageToLog(newMessage);

	}

	//this function will send a message when a ship torpedo attack hits a base crew section
	private void SendTorpedoAttackHitBaseCrewSectionMessage(CombatUnit attackingUnit, CombatUnit targetedUnit, int damage, string torpedoType){

		string newMessage = "";

		//check if the attacking unit is a ship
		if (attackingUnit.GetComponent<Ship> () == true) {

			newMessage = attackingUnit.GetComponent<Ship> ().shipType + " " + attackingUnit.GetComponent<Ship> ().shipName + " hits " +
				InsertColoredPlayerColor (targetedUnit.GetComponent<CombatUnit> ().owner) +
				" Starbase " + targetedUnit.GetComponent<Starbase> ().baseName
				+ " Creq Section with " + torpedoType + " Torpedo and deals " + damage + " damage!";

		}
		//the else condition is that the attacking unit is a base
		else {

			newMessage = "Starbase " + attackingUnit.GetComponent<Starbase> ().baseName + " hits " +
				InsertColoredPlayerColor (targetedUnit.GetComponent<CombatUnit> ().owner) +
				" Starbase " + targetedUnit.GetComponent<Starbase> ().baseName
				+ " Crew Section with " + torpedoType + " Torpedo and deals " + damage + " damage!";

		}

		//add the player color prefix to the string
		newMessage = AddPlayerColorToMessage(gameManager.currentTurnPlayer,newMessage);

		//upload the completed string to the log
		AddMessageToLog(newMessage);

	}

	//this function will send a message when a ship torpedo attack hits a base storage section 1
	private void SendTorpedoAttackHitBaseStorageSection1Message(CombatUnit attackingUnit, CombatUnit targetedUnit, int damage, string torpedoType){

		string newMessage = "";

		//check if the attacking unit is a ship
		if (attackingUnit.GetComponent<Ship> () == true) {

			newMessage = attackingUnit.GetComponent<Ship> ().shipType + " " + attackingUnit.GetComponent<Ship> ().shipName + " hits " +
				InsertColoredPlayerColor (targetedUnit.GetComponent<CombatUnit> ().owner) +
				" Starbase " + targetedUnit.GetComponent<Starbase> ().baseName
				+ " Storage Section 1 with " + torpedoType + " Torpedo and deals " + damage + " damage!";

		}
		//the else condition is that the attacking unit is a base
		else {

			newMessage = "Starbase " + attackingUnit.GetComponent<Starbase> ().baseName + " hits " +
				InsertColoredPlayerColor (targetedUnit.GetComponent<CombatUnit> ().owner) +
				" Starbase " + targetedUnit.GetComponent<Starbase> ().baseName
				+ " Storage Section 1 with " + torpedoType + " Torpedo and deals " + damage + " damage!";

		}

		//add the player color prefix to the string
		newMessage = AddPlayerColorToMessage(gameManager.currentTurnPlayer,newMessage);

		//upload the completed string to the log
		AddMessageToLog(newMessage);

	}

	//this function will send a message when a ship torpedo attack hits a base storage section 2
	private void SendTorpedoAttackHitBaseStorageSection2Message(CombatUnit attackingUnit, CombatUnit targetedUnit, int damage, string torpedoType){

		string newMessage = "";

		//check if the attacking unit is a ship
		if (attackingUnit.GetComponent<Ship> () == true) {

			newMessage = attackingUnit.GetComponent<Ship> ().shipType + " " + attackingUnit.GetComponent<Ship> ().shipName + " hits " +
				InsertColoredPlayerColor (targetedUnit.GetComponent<CombatUnit> ().owner) +
				" Starbase " + targetedUnit.GetComponent<Starbase> ().baseName
				+ " Storage Section 2 with " + torpedoType + " Torpedo and deals " + damage + " damage!";

		}
		//the else condition is that the attacking unit is a base
		else {

			newMessage = "Starbase " + attackingUnit.GetComponent<Starbase> ().baseName + " hits " +
				InsertColoredPlayerColor (targetedUnit.GetComponent<CombatUnit> ().owner) +
				" Starbase " + targetedUnit.GetComponent<Starbase> ().baseName
				+ " Storage Section 2 with " + torpedoType + " Torpedo and deals " + damage + " damage!";

		}

		//add the player color prefix to the string
		newMessage = AddPlayerColorToMessage(gameManager.currentTurnPlayer,newMessage);

		//upload the completed string to the log
		AddMessageToLog(newMessage);

	}

	//this function will send a message when a ship phaser section is destroyed
	private void SendShipPhaserSectionDestroyedMessage(CombatUnit combatUnit){

		string newMessage = InsertColoredPlayerColor(combatUnit.GetComponent<CombatUnit>().owner)+ " " + 
			combatUnit.GetComponent<Ship> ().shipType + " " + combatUnit.GetComponent<Ship> ().shipName + 
			" Phaser Section was Destroyed!!";

		//upload the completed string to the log
		AddMessageToLog(newMessage);

	}


	//this function will send a message when a ship torpedo section is destroyed
	private void SendShipTorpedoSectionDestroyedMessage(CombatUnit combatUnit){

		string newMessage = InsertColoredPlayerColor(combatUnit.GetComponent<CombatUnit>().owner)+ " " + 
			combatUnit.GetComponent<Ship> ().shipType + " " + combatUnit.GetComponent<Ship> ().shipName + 
			" Torpedo Section was Destroyed!!";

		//upload the completed string to the log
		AddMessageToLog(newMessage);

	}

	//this function will send a message when a ship storage section is destroyed
	private void SendShipStorageSectionDestroyedMessage(CombatUnit combatUnit){

		string newMessage = InsertColoredPlayerColor(combatUnit.GetComponent<CombatUnit>().owner)+ " " + 
			combatUnit.GetComponent<Ship> ().shipType + " " + combatUnit.GetComponent<Ship> ().shipName + 
			" Storage Section was Destroyed!!";

		//upload the completed string to the log
		AddMessageToLog(newMessage);

	}

	//this function will send a message when a ship crew section is destroyed
	private void SendShipCrewSectionDestroyedMessage(CombatUnit combatUnit){

		string newMessage = InsertColoredPlayerColor(combatUnit.GetComponent<CombatUnit>().owner)+ " " + 
			combatUnit.GetComponent<Ship> ().shipType + " " + combatUnit.GetComponent<Ship> ().shipName + 
			" Crew Section was Destroyed!!";

		//upload the completed string to the log
		AddMessageToLog(newMessage);

	}

	//this function will send a message when a ship engine section is destroyed
	private void SendShipEngineSectionDestroyedMessage(CombatUnit combatUnit){

		string newMessage = InsertColoredPlayerColor(combatUnit.GetComponent<CombatUnit>().owner)+ " " + 
			combatUnit.GetComponent<Ship> ().shipType + " " + combatUnit.GetComponent<Ship> ().shipName + 
			" Engine Section was Destroyed!!";

		//upload the completed string to the log
		AddMessageToLog(newMessage);

	}

	//this function will send a message when a ship is destroyed
	private void SendShipDestroyedMessage(CombatUnit combatUnit){

		string newMessage = InsertColoredPlayerColor(combatUnit.GetComponent<CombatUnit>().owner)+ " " + 
			combatUnit.GetComponent<Ship> ().shipType + " " + combatUnit.GetComponent<Ship> ().shipName + 
			" was Completely Destroyed!!!";

		//upload the completed string to the log
		AddMessageToLog(newMessage);

	}

	//this function will send a message when a base phaser section 1 is destroyed
	private void SendBasePhaserSection1DestroyedMessage(CombatUnit combatUnit){

		string newMessage = InsertColoredPlayerColor(combatUnit.GetComponent<CombatUnit>().owner) + 
		" Starbase " + combatUnit.GetComponent<Starbase> ().baseName + 
			" Phaser Section 1 was Destroyed!!";

		//upload the completed string to the log
		AddMessageToLog(newMessage);

	}

	//this function will send a message when a base phaser section 2 is destroyed
	private void SendBasePhaserSection2DestroyedMessage(CombatUnit combatUnit){

		string newMessage = InsertColoredPlayerColor(combatUnit.GetComponent<CombatUnit>().owner) + 
			" Starbase " + combatUnit.GetComponent<Starbase> ().baseName + 
			" Phaser Section 2 was Destroyed!!";

		//upload the completed string to the log
		AddMessageToLog(newMessage);

	}

	//this function will send a message when a base torpedo section is destroyed
	private void SendBaseTorpedoSectionDestroyedMessage(CombatUnit combatUnit){

		string newMessage = InsertColoredPlayerColor(combatUnit.GetComponent<CombatUnit>().owner) + 
			" Starbase " + combatUnit.GetComponent<Starbase> ().baseName + 
			" Torpedo Section was Destroyed!!";

		//upload the completed string to the log
		AddMessageToLog(newMessage);

	}

	//this function will send a message when a base crew section is destroyed
	private void SendBaseCrewSectionDestroyedMessage(CombatUnit combatUnit){

		string newMessage = InsertColoredPlayerColor(combatUnit.GetComponent<CombatUnit>().owner) + 
			" Starbase " + combatUnit.GetComponent<Starbase> ().baseName + 
			" Crew Section was Destroyed!!";

		//upload the completed string to the log
		AddMessageToLog(newMessage);

	}

	//this function will send a message when a base storage section 1 is destroyed
	private void SendBaseStorageSection1DestroyedMessage(CombatUnit combatUnit){

		string newMessage = InsertColoredPlayerColor(combatUnit.GetComponent<CombatUnit>().owner) + 
			" Starbase " + combatUnit.GetComponent<Starbase> ().baseName + 
			" Storage Section 1 was Destroyed!!";

		//upload the completed string to the log
		AddMessageToLog(newMessage);

	}

	//this function will send a message when a base storage section 2 is destroyed
	private void SendBaseStorageSection2DestroyedMessage(CombatUnit combatUnit){

		string newMessage = InsertColoredPlayerColor(combatUnit.GetComponent<CombatUnit>().owner) + 
			" Starbase " + combatUnit.GetComponent<Starbase> ().baseName + 
			" Storage Section 2 was Destroyed!!";

		//upload the completed string to the log
		AddMessageToLog(newMessage);

	}

	//this function will send a message when a base is destroyed
	private void SendBaseDestroyedMessage(CombatUnit combatUnit){

		string newMessage = InsertColoredPlayerColor(combatUnit.GetComponent<CombatUnit>().owner) + 
			" Starbase " + combatUnit.GetComponent<Starbase> ().baseName + 
			" was Completely Destroyed!!!";

		//upload the completed string to the log
		AddMessageToLog(newMessage);

	}


	//this function will send a message when a ship uses a crystal to heal damage
	private void SendCrystalUsedMessage(CombatUnit selectedUnit, CombatUnit targetedUnit, CombatManager.CrystalType crystalType, int shieldsHealed, string shipSectionString){

		//define a string to display the type of crystal in our message
		string crystalString = "";

		//check what type of crystal is being used, and set the string appropriately
		if (crystalType == CombatManager.CrystalType.Dilithium) {
			
			crystalString = "Dilithium Crystal";

		} else if (crystalType == CombatManager.CrystalType.Trilithium) {

			crystalString = "Trilithium Crystal";

		}

		string newMessage = "";

		//check if the selected unit is a ship
		if (selectedUnit.GetComponent<Ship> () == true) {

			//check if the targeted unit is a ship
			if (targetedUnit.GetComponent<Ship> () == true) {
				
				newMessage = selectedUnit.GetComponent<Ship> ().shipType + " " + selectedUnit.GetComponent<Ship> ().shipName + " uses " +
				crystalString + " on " + InsertColoredPlayerColor (targetedUnit.GetComponent<CombatUnit> ().owner) +
				" " + targetedUnit.GetComponent<Ship> ().shipType + " " + targetedUnit.GetComponent<Ship> ().shipName
				+ " " + shipSectionString + ".  Crystal heals shields by " + shieldsHealed.ToString () + "!";

			}
			//the else condition is that the targeted unit is a base
			else {
				
				newMessage = selectedUnit.GetComponent<Ship> ().shipType + " " + selectedUnit.GetComponent<Ship> ().shipName + " uses " +
				crystalString + " on " + InsertColoredPlayerColor (targetedUnit.GetComponent<CombatUnit> ().owner) +
				" Starbase " + targetedUnit.GetComponent<Starbase> ().baseName
				+ " " + shipSectionString + ".  Crystal heals shields by " + shieldsHealed.ToString () + "!";

			}

		}
		//the else condition is that the selected unit is a base
		else {

			//check if the targeted unit is a ship
			if (targetedUnit.GetComponent<Ship> () == true) {

				newMessage = "Starbase " + selectedUnit.GetComponent<Starbase> ().baseName + " uses " +
					crystalString + " on " + InsertColoredPlayerColor (targetedUnit.GetComponent<CombatUnit> ().owner) +
					" " + targetedUnit.GetComponent<Ship> ().shipType + " " + targetedUnit.GetComponent<Ship> ().shipName
					+ " " + shipSectionString + ".  Crystal heals shields by " + shieldsHealed.ToString () + "!";

			}
			//the else condition is that the targeted unit is a base
			else {

				newMessage = "Starbase " + selectedUnit.GetComponent<Starbase> ().baseName + " uses " +
					crystalString + " on " + InsertColoredPlayerColor (targetedUnit.GetComponent<CombatUnit> ().owner) +
					" Starbase " + targetedUnit.GetComponent<Starbase> ().baseName
					+ " " + shipSectionString + ".  Crystal heals shields by " + shieldsHealed.ToString () + "!";

			}

		}

		//add the player color prefix to the string
		newMessage = AddPlayerColorToMessage(gameManager.currentTurnPlayer,newMessage);

		//upload the completed string to the log
		AddMessageToLog(newMessage);

	}

	//this function will send a message when a ship uses a repair crew to repair a section
	private void SendRepairCrewUsedMessage(CombatUnit selectedUnit, CombatUnit targetedUnit, string shipSectionString){

		string newMessage = "";


		//check if the selected unit is a ship
		if (selectedUnit.GetComponent<Ship> () == true) {

			//check if the targeted unit is a ship
			if (targetedUnit.GetComponent<Ship> () == true) {
				
				newMessage = selectedUnit.GetComponent<Ship> ().shipType + " " + selectedUnit.GetComponent<Ship> ().shipName +
				" uses Repair Crew on " + InsertColoredPlayerColor (targetedUnit.GetComponent<CombatUnit> ().owner) +
				" " + targetedUnit.GetComponent<Ship> ().shipType + " " + targetedUnit.GetComponent<Ship> ().shipName
				+ " " + shipSectionString;

			}
			//the else condition is that the targeted unit is a base
			else {

				newMessage = selectedUnit.GetComponent<Ship> ().shipType + " " + selectedUnit.GetComponent<Ship> ().shipName +
				" uses Repair Crew on " + InsertColoredPlayerColor (targetedUnit.GetComponent<CombatUnit> ().owner) +
				" Starbase " + targetedUnit.GetComponent<Starbase> ().baseName
				+ " " + shipSectionString;

			}

		}
		//the else condition is that the selected unit is a base
		else {
			
			//check if the targeted unit is a ship
			if (targetedUnit.GetComponent<Ship> () == true) {

				newMessage = "Starbase " + selectedUnit.GetComponent<Starbase> ().baseName +
					" uses Repair Crew on " + InsertColoredPlayerColor (targetedUnit.GetComponent<CombatUnit> ().owner) +
					" " + targetedUnit.GetComponent<Ship> ().shipType + " " + targetedUnit.GetComponent<Ship> ().shipName
					+ " " + shipSectionString;

			}
			//the else condition is that the targeted unit is a base
			else {

				newMessage = "Starbase " + selectedUnit.GetComponent<Starbase> ().baseName +
					" uses Repair Crew on " + InsertColoredPlayerColor (targetedUnit.GetComponent<CombatUnit> ().owner) +
					" Starbase " + targetedUnit.GetComponent<Starbase> ().baseName
					+ " " + shipSectionString;

			}

		}

		//add the player color prefix to the string
		newMessage = AddPlayerColorToMessage(gameManager.currentTurnPlayer,newMessage);

		//upload the completed string to the log
		AddMessageToLog(newMessage);

	}

	//this function will send a message when a combat unit is renamed
	private void SendUnitRenamedMessage(CombatUnit selectedUnit, string oldName){

		string newMessage = "";

		//check if the selected unit is a ship
		if (selectedUnit.GetComponent<Ship> () == true) {

			newMessage = selectedUnit.GetComponent<Ship> ().shipType + " " + oldName + " has been rechristened as " +
			selectedUnit.GetComponent<Ship> ().shipName + "!";

		}
		//the else condition is that the selected unit is a base
		else {

			newMessage = "Starbase " + oldName + " has been rechristened as " +
				selectedUnit.GetComponent<Starbase> ().baseName + "!";

		}

		//add the player color prefix to the string
		newMessage = AddPlayerColorToMessage(gameManager.currentTurnPlayer,newMessage);

		//upload the completed string to the log
		AddMessageToLog(newMessage);

	}

	//this function sends a message to the log announcing the player's income collection
	private void SendPlayerIncomeMessage(Player currentTurnPlayer, int turnIncome){

		//create variable for message log input
		string newMessage;

		newMessage = "Collects income of " + turnIncome + " and now has " + currentTurnPlayer.playerMoney + " total";

		//build the rich text string to pass to the message log
		newMessage = AddPlayerNameToMessage(gameManager.currentTurnPlayer,newMessage);

		//pass the message to the message log
		AddMessageToLog(newMessage);

	}

	//this function sends a message to the log announcing a planet colonization
	private void SendPlanetColonizationMessage(string planetColonized, Player colonizingPlayer){

		//Debug.Log ("colonize");

		//create variable for message log input
		string newMessage;

		newMessage = planetColonized + " has been colonized by " + InsertColoredPlayerName(colonizingPlayer);

		//build the rich text string to pass to the message log
		newMessage = AddPlayerColorToMessage(gameManager.currentTurnPlayer,newMessage);

		//pass the message to the message log
		AddMessageToLog(newMessage);

	}

	//this function sends a message for when a ship purchases items
	private void SendPurchaseItemsMessage(Dictionary<string,int> purchasedItems, int purchasedValue, CombatUnit combatUnit){

		//create variable for message log input
		string newMessage;

		newMessage = combatUnit.GetComponent<Ship> ().shipType + " " + combatUnit.GetComponent<Ship> ().shipName + " has purchased the following items: \n";

		foreach(KeyValuePair<string,int> entry in purchasedItems){

			newMessage += entry.Value.ToString () + "X " + entry.Key + "\n";

		}

		newMessage += "for a total cost of " + purchasedValue.ToString ();

		//build the rich text string to pass to the message log
		newMessage = AddPlayerColorToMessage(gameManager.currentTurnPlayer,newMessage);

		//pass the message to the message log
		AddMessageToLog(newMessage);

	}

	//this function sends a message for when a player buys a ship
	private void SendPurchaseShipMessage(NameNewShip.NewUnitEventData newUnitData){

		//create variable for message log input
		string newMessage;

		//check if there were additional upgrades purchased
		if (newUnitData.newUnitOutfittedItems.Count > 0) {

			newMessage = "has purchased new " + Ship.GetShipTypeFromUnitType (newUnitData.newUnitType) +
			" " + newUnitData.newUnitName + " with the following additional items: \n";

			foreach (KeyValuePair<string,int> entry in newUnitData.newUnitOutfittedItems) {

				newMessage += entry.Value.ToString () + "X " + entry.Key + "\n";

			}

		} else {

			//the else is if there are no outfitted items
			newMessage = "has purchased new " + Ship.GetShipTypeFromUnitType (newUnitData.newUnitType) +
			" " + newUnitData.newUnitName + " ";

		}

		newMessage += "for a total cost of " + newUnitData.newUnitTotalPrice.ToString ();

		//build the rich text string to pass to the message log
		newMessage = AddPlayerNameToMessage(gameManager.currentTurnPlayer,newMessage);

		//pass the message to the message log
		AddMessageToLog(newMessage);

	}

	//this function will send a message when a ship takes sunburst damage
	private void SendSunburstDamageMessage(CombatUnit combatUnit, int damage){

		string newMessage = "";

		//check if the unit is a ship
		if (combatUnit.GetComponent<Ship> () == true) {

			newMessage = combatUnit.GetComponent<Ship> ().shipType + " " + combatUnit.GetComponent<Ship> ().shipName + " is too close to the sun and takes " +
				 + damage + " damage per section!";

			//add the player color prefix to the string
			newMessage = AddPlayerColorToMessage (gameManager.currentTurnPlayer, newMessage);

			//upload the completed string to the log
			AddMessageToLog (newMessage);

		}

	}

	//this function handles OnDestroy
	private void OnDestroy(){

		RemoveAllListeners ();

	}

	//this function removes all event listeners
	private void RemoveAllListeners(){

		if (gameManager != null) {

			//remove listener for the start of a new turn.  The onNewTurn passes a player, but we don't need it
			gameManager.OnNewTurn.RemoveListener (playerSendTurnPhaseMessageAction);

			//remove listener for the start of the main phase.  The OnBeginMainPhase passes a player, but we don't need it
			gameManager.OnBeginMainPhase.RemoveListener (playerSendTurnPhaseMessageAction);

			//remove listener for loading a turn
			gameManager.OnLoadedTurn.RemoveListener(playerSendLoadedTurnAction);

			//remove listener for player being eliminated
			gameManager.OnKillPlayer.RemoveListener(killPlayerAction);

		}

		if (uiManager != null) {

			//remove listener for when a new chat message is submitted
			uiManager.GetComponent<TextInput>().OnNewChatMessageSubmitted.RemoveListener(stringSendNewChatMessageAction);

			uiManager.GetComponent<FlareManager> ().OnUseFlaresYes.RemoveListener (flareSendUsedFlaresYesMessageAction);
			uiManager.GetComponent<FlareManager> ().OnUseFlaresCancel.RemoveListener (flareSendUsedFlaresNoMessageAction);

			//remove listener for purchasing items
			uiManager.GetComponent<PurchaseManager>().OnPurchaseItems.RemoveListener(purchaseSendPurchaseMessageAction);

			//remove listener for purchasing a ship
			uiManager.GetComponent<NameNewShip>().OnPurchasedNewShip.RemoveListener(purchaseShipSendPurchaseMessageAction);

			//remove listener for saved game
			uiManager.GetComponent<FileManager>().OnSendSaveGameDataFromSave.RemoveListener(saveDataSendSavedGameAction);

			//remove listener for deleted file
			uiManager.GetComponent<FileLoadWindow>().OnFileConfirmedDelete.RemoveListener(stringSendDeletedFileMessageAction);

		}



		//remove a listener for when a ship movement is completed
		EngineSection.OnMoveFromToFinish.RemoveListener(movementSendNewShipMovementMessageAction);

		//remove listeners for the tractor beam
		PhaserSection.OnPrimeTractorBeam.RemoveListener(shipSendTractorBeamPrimedMessageAction);
		PhaserSection.OnEngageTractorBeam.RemoveListener(shipSendTractorBeamEngagedMessageAction);
		PhaserSection.OnDisengageTractorBeam.RemoveListener(shipSendTractorBeamDisengagedMessageAction);

		//remove listeners for warp boosters
		EngineSection.OnUseWarpBooster.RemoveListener(shipSendUseWarpBoosterMessageAction);
		EngineSection.OnUseTranswarpBooster.RemoveListener(shipSendUseTranswarpBoosterMessageAction);

		//remove listeners for ship phaser attacks
		/*
		//these are commented out to replace with cutscene events
		PhaserSection.OnFirePhasers.RemoveListener(firePhasersSendPhaserAttackMessageAction);
		CombatManager.OnPhaserAttackHitShipPhaserSection.RemoveListener(phasersSendPhaserAttackHitShipPhaserSectionMessageAction);
		CombatManager.OnPhaserAttackHitShipTorpedoSection.RemoveListener(phasersSendPhaserAttackHitShipTorpedoSectionMessageAction);
		CombatManager.OnPhaserAttackHitShipStorageSection.RemoveListener(phasersSendPhaserAttackHitShipStorageSectionMessageAction);
		CombatManager.OnPhaserAttackHitShipCrewSection.RemoveListener(phasersSendPhaserAttackHitShipCrewSectionMessageAction);
		CombatManager.OnPhaserAttackHitShipEngineSection.RemoveListener(phasersSendPhaserAttackHitShipEngineSectionMessageAction);
		CombatManager.OnPhaserAttackMissShip.RemoveListener(phasersSendPhaserAttackMissMessageAction);
		*/

		CutsceneManager.OnPhaserHitShipPhaserSection.RemoveListener (phasersSendPhaserAttackHitShipPhaserSectionMessageAction);
		CutsceneManager.OnPhaserHitShipTorpedoSection.RemoveListener (phasersSendPhaserAttackHitShipTorpedoSectionMessageAction);
		CutsceneManager.OnPhaserHitShipStorageSection.RemoveListener (phasersSendPhaserAttackHitShipStorageSectionMessageAction);
		CutsceneManager.OnPhaserHitShipCrewSection.RemoveListener (phasersSendPhaserAttackHitShipCrewSectionMessageAction);
		CutsceneManager.OnPhaserHitShipEngineSection.RemoveListener (phasersSendPhaserAttackHitShipEngineSectionMessageAction);
		CutsceneManager.OnPhaserMissShip.RemoveListener (cutscenePhasersSendPhaserAttackMissMessageAction);

		//remove listeners for base phaser attacks
		/*
		//these are commented out to replace with cutscene events
		StarbasePhaserSection1.OnFirePhasers.RemoveListener(firePhasersSendPhaserAttackMessageAction);
		StarbasePhaserSection2.OnFirePhasers.RemoveListener(firePhasersSendPhaserAttackMessageAction);
		CombatManager.OnPhaserAttackHitBasePhaserSection1.RemoveListener(phasersSendPhaserAttackHitBasePhaserSection1MessageAction);
		CombatManager.OnPhaserAttackHitBasePhaserSection2.RemoveListener(phasersSendPhaserAttackHitBasePhaserSection2MessageAction);
		CombatManager.OnPhaserAttackHitBaseTorpedoSection.RemoveListener(phasersSendPhaserAttackHitBaseTorpedoSectionMessageAction);
		CombatManager.OnPhaserAttackHitBaseCrewSection.RemoveListener(phasersSendPhaserAttackHitBaseCrewSectionMessageAction);
		CombatManager.OnPhaserAttackHitBaseStorageSection1.RemoveListener(phasersSendPhaserAttackHitBaseStorageSection1MessageAction);
		CombatManager.OnPhaserAttackHitBaseStorageSection2.RemoveListener(phasersSendPhaserAttackHitBaseStorageSection2MessageAction);
		CombatManager.OnPhaserAttackMissBase.RemoveListener(phasersSendPhaserAttackMissMessageAction);
		*/

		CutsceneManager.OnPhaserHitBasePhaserSection1.RemoveListener (phasersSendPhaserAttackHitBasePhaserSection1MessageAction);
		CutsceneManager.OnPhaserHitBasePhaserSection2.RemoveListener (phasersSendPhaserAttackHitBasePhaserSection2MessageAction);
		CutsceneManager.OnPhaserHitBaseTorpedoSection.RemoveListener (phasersSendPhaserAttackHitBaseTorpedoSectionMessageAction);
		CutsceneManager.OnPhaserHitBaseCrewSection.RemoveListener (phasersSendPhaserAttackHitBaseCrewSectionMessageAction);
		CutsceneManager.OnPhaserHitBaseStorageSection1.RemoveListener (phasersSendPhaserAttackHitBaseStorageSection1MessageAction);
		CutsceneManager.OnPhaserHitBaseStorageSection2.RemoveListener (phasersSendPhaserAttackHitBaseStorageSection2MessageAction);
		CutsceneManager.OnPhaserMissBase.RemoveListener (cutscenePhasersSendPhaserAttackMissMessageAction);

		//remove listeners for torpedo attacks
		TorpedoSection.OnFireLightTorpedo.RemoveListener(fireLightTorpedoSendTorpedoAttackMessageAction);
		TorpedoSection.OnFireHeavyTorpedo.RemoveListener(fireHeavyTorpedoSendTorpedoAttackMessageAction);
		StarbaseTorpedoSection.OnFireLightTorpedo.RemoveListener(fireLightTorpedoSendTorpedoAttackMessageAction);
		StarbaseTorpedoSection.OnFireHeavyTorpedo.RemoveListener(fireHeavyTorpedoSendTorpedoAttackMessageAction);

		/*
		//these are commented out to replace with cutscene events
		CombatManager.OnLightTorpedoAttackDefeatedByFlares.RemoveListener(flaresSendFlaresDefeatLightTorpedoMessageAction);
		CombatManager.OnHeavyTorpedoAttackDefeatedByFlares.RemoveListener(flaresSendFlaresDefeatHeavyTorpedoMessageAction);
		CombatManager.OnLightTorpedoAttackFlaresFailed.RemoveListener(flaresSendFlaresFailLightTorpedoMessageAction);
		CombatManager.OnHeavyTorpedoAttackFlaresFailed.RemoveListener(flaresSendFlaresFailHeavyTorpedoMessageAction);
		*/

		CutsceneManager.OnLightTorpedoFlareSuccess.RemoveListener (cutsceneFlaresSendFlaresDefeatLightTorpedoMessageAction);
		CutsceneManager.OnHeavyTorpedoFlareSuccess.RemoveListener (cutsceneFlaresSendFlaresDefeatHeavyTorpedoMessageAction);
		CutsceneManager.OnLightTorpedoFlareFailure.RemoveListener (flaresSendFlaresFailLightTorpedoMessageAction);
		CutsceneManager.OnHeavyTorpedoFlareFailure.RemoveListener (flaresSendFlaresFailHeavyTorpedoMessageAction);

		//remove listeners for ship torpedo attacks
		/*
		//these are commented out to replace with cutscene events
		CombatManager.OnLightTorpedoAttackHitShipPhaserSection.RemoveListener(torpedoSendLightTorpedoAttackHitShipPhaserSectionMessageAction);
		CombatManager.OnHeavyTorpedoAttackHitShipPhaserSection.RemoveListener(torpedoSendHeavyTorpedoAttackHitShipPhaserSectionMessageAction);
		CombatManager.OnLightTorpedoAttackHitShipTorpedoSection.RemoveListener(torpedoSendLightTorpedoAttackHitShipTorpedoSectionMessageAction);
		CombatManager.OnHeavyTorpedoAttackHitShipTorpedoSection.RemoveListener(torpedoSendHeavyTorpedoAttackHitShipTorpedoSectionMessageAction);
		CombatManager.OnLightTorpedoAttackHitShipStorageSection.RemoveListener(torpedoSendLightTorpedoAttackHitShipStorageSectionMessageAction);
		CombatManager.OnHeavyTorpedoAttackHitShipStorageSection.RemoveListener(torpedoSendHeavyTorpedoAttackHitShipStorageSectionMessageAction);
		CombatManager.OnLightTorpedoAttackHitShipCrewSection.RemoveListener(torpedoSendLightTorpedoAttackHitShipCrewSectionMessageAction);
		CombatManager.OnHeavyTorpedoAttackHitShipCrewSection.RemoveListener(torpedoSendHeavyTorpedoAttackHitShipCrewSectionMessageAction);
		CombatManager.OnLightTorpedoAttackHitShipEngineSection.RemoveListener(torpedoSendLightTorpedoAttackHitShipEngineSectionMessageAction);
		CombatManager.OnHeavyTorpedoAttackHitShipEngineSection.RemoveListener(torpedoSendHeavyTorpedoAttackHitShipEngineSectionMessageAction);
		CombatManager.OnLightTorpedoAttackMissShip.RemoveListener(torpedoSendLightTorpedoAttackMissMessageAction);
		CombatManager.OnHeavyTorpedoAttackMissShip.RemoveListener(torpedoSendHeavyTorpedoAttackMissMessageAction);
		*/

		CutsceneManager.OnLightTorpedoHitShipPhaserSection.RemoveListener (torpedoSendLightTorpedoAttackHitShipPhaserSectionMessageAction);
		CutsceneManager.OnHeavyTorpedoHitShipPhaserSection.RemoveListener (torpedoSendHeavyTorpedoAttackHitShipPhaserSectionMessageAction);
		CutsceneManager.OnLightTorpedoHitShipTorpedoSection.RemoveListener (torpedoSendLightTorpedoAttackHitShipTorpedoSectionMessageAction);
		CutsceneManager.OnHeavyTorpedoHitShipTorpedoSection.RemoveListener (torpedoSendHeavyTorpedoAttackHitShipTorpedoSectionMessageAction);
		CutsceneManager.OnLightTorpedoHitShipStorageSection.RemoveListener (torpedoSendLightTorpedoAttackHitShipStorageSectionMessageAction);
		CutsceneManager.OnHeavyTorpedoHitShipStorageSection.RemoveListener (torpedoSendHeavyTorpedoAttackHitShipStorageSectionMessageAction);
		CutsceneManager.OnLightTorpedoHitShipCrewSection.RemoveListener (torpedoSendLightTorpedoAttackHitShipCrewSectionMessageAction);
		CutsceneManager.OnHeavyTorpedoHitShipCrewSection.RemoveListener (torpedoSendHeavyTorpedoAttackHitShipCrewSectionMessageAction);
		CutsceneManager.OnLightTorpedoHitShipEngineSection.RemoveListener (torpedoSendLightTorpedoAttackHitShipEngineSectionMessageAction);
		CutsceneManager.OnHeavyTorpedoHitShipEngineSection.RemoveListener (torpedoSendHeavyTorpedoAttackHitShipEngineSectionMessageAction);
		CutsceneManager.OnLightTorpedoMissShip.RemoveListener (cutsceneTorpedoSendLightTorpedoAttackMissMessageAction);
		CutsceneManager.OnHeavyTorpedoMissShip.RemoveListener (cutsceneTorpedoSendHeavyTorpedoAttackMissMessageAction);

		//remove listeners for base torpedo attacks
		/*
		//these are commented out to replace with cutscene events
		CombatManager.OnLightTorpedoAttackHitBasePhaserSection1.RemoveListener(torpedoSendLightTorpedoAttackHitBasePhaserSection1MessageAction);
		CombatManager.OnHeavyTorpedoAttackHitBasePhaserSection1.RemoveListener(torpedoSendHeavyTorpedoAttackHitBasePhaserSection1MessageAction);
		CombatManager.OnLightTorpedoAttackHitBasePhaserSection2.RemoveListener (torpedoSendLightTorpedoAttackHitBasePhaserSection2MessageAction);
		CombatManager.OnHeavyTorpedoAttackHitBasePhaserSection2.RemoveListener(torpedoSendHeavyTorpedoAttackHitBasePhaserSection2MessageAction);
		CombatManager.OnLightTorpedoAttackHitBaseTorpedoSection.RemoveListener(torpedoSendLightTorpedoAttackHitBaseTorpedoSectionMessageAction);
		CombatManager.OnHeavyTorpedoAttackHitBaseTorpedoSection.RemoveListener(torpedoSendHeavyTorpedoAttackHitBaseTorpedoSectionMessageAction);
		CombatManager.OnLightTorpedoAttackHitBaseCrewSection.RemoveListener(torpedoSendLightTorpedoAttackHitBaseCrewSectionMessageAction);
		CombatManager.OnHeavyTorpedoAttackHitBaseCrewSection.RemoveListener(torpedoSendHeavyTorpedoAttackHitBaseCrewSectionMessageAction);
		CombatManager.OnLightTorpedoAttackHitBaseStorageSection1.RemoveListener(torpedoSendLightTorpedoAttackHitBaseStorageSection1MessageAction);
		CombatManager.OnHeavyTorpedoAttackHitBaseStorageSection1.RemoveListener(torpedoSendHeavyTorpedoAttackHitBaseStorageSection1MessageAction);
		CombatManager.OnLightTorpedoAttackHitBaseStorageSection2.RemoveListener(torpedoSendLightTorpedoAttackHitBaseStorageSection2MessageAction);
		CombatManager.OnHeavyTorpedoAttackHitBaseStorageSection2.RemoveListener(torpedoSendHeavyTorpedoAttackHitBaseStorageSection2MessageAction);
		CombatManager.OnLightTorpedoAttackMissBase.RemoveListener(torpedoSendLightTorpedoAttackMissMessageAction);
		CombatManager.OnHeavyTorpedoAttackMissBase.RemoveListener(torpedoSendHeavyTorpedoAttackMissMessageAction);
		*/

		CutsceneManager.OnLightTorpedoHitBasePhaserSection1.RemoveListener (torpedoSendLightTorpedoAttackHitBasePhaserSection1MessageAction);
		CutsceneManager.OnHeavyTorpedoHitBasePhaserSection1.RemoveListener (torpedoSendHeavyTorpedoAttackHitBasePhaserSection1MessageAction);
		CutsceneManager.OnLightTorpedoHitBasePhaserSection2.RemoveListener (torpedoSendLightTorpedoAttackHitBasePhaserSection2MessageAction);
		CutsceneManager.OnHeavyTorpedoHitBasePhaserSection2.RemoveListener (torpedoSendHeavyTorpedoAttackHitBasePhaserSection2MessageAction);
		CutsceneManager.OnLightTorpedoHitBaseTorpedoSection.RemoveListener (torpedoSendLightTorpedoAttackHitBaseTorpedoSectionMessageAction);
		CutsceneManager.OnHeavyTorpedoHitBaseTorpedoSection.RemoveListener (torpedoSendHeavyTorpedoAttackHitBaseTorpedoSectionMessageAction);
		CutsceneManager.OnLightTorpedoHitBaseCrewSection.RemoveListener (torpedoSendLightTorpedoAttackHitBaseCrewSectionMessageAction);
		CutsceneManager.OnHeavyTorpedoHitBaseCrewSection.RemoveListener (torpedoSendHeavyTorpedoAttackHitBaseCrewSectionMessageAction);
		CutsceneManager.OnLightTorpedoHitBaseStorageSection1.RemoveListener (torpedoSendLightTorpedoAttackHitBaseStorageSection1MessageAction);
		CutsceneManager.OnHeavyTorpedoHitBaseStorageSection1.RemoveListener (torpedoSendHeavyTorpedoAttackHitBaseStorageSection1MessageAction);
		CutsceneManager.OnLightTorpedoHitBaseStorageSection2.RemoveListener (torpedoSendLightTorpedoAttackHitBaseStorageSection2MessageAction);
		CutsceneManager.OnHeavyTorpedoHitBaseStorageSection2.RemoveListener (torpedoSendHeavyTorpedoAttackHitBaseStorageSection2MessageAction);
		CutsceneManager.OnLightTorpedoMissBase.RemoveListener (cutsceneTorpedoSendLightTorpedoAttackMissMessageAction);
		CutsceneManager.OnHeavyTorpedoMissBase.RemoveListener (cutsceneTorpedoSendHeavyTorpedoAttackMissMessageAction);

		//remove listeners for using crystals to heal ships
		CombatManager.OnCrystalUsedOnShipPhaserSection.RemoveListener(crystalSendCrystalUsedOnShipPhaserSectionMessageAction);
		CombatManager.OnCrystalUsedOnShipTorpedoSection.RemoveListener(crystalSendCrystalUsedOnShipTorpedoSectionMessageAction);
		CombatManager.OnCrystalUsedOnShipStorageSection.RemoveListener(crystalSendCrystalUsedOnShipStorageSectionMessageAction);
		CombatManager.OnCrystalUsedOnShipCrewSection.RemoveListener(crystalSendCrystalUsedOnShipCrewSectionMessageAction);
		CombatManager.OnCrystalUsedOnShipEngineSection.RemoveListener(crystalSendCrystalUsedOnShipEngineSectionMessageAction);

		//remove listeners for using crystals to heal bases
		CombatManager.OnCrystalUsedOnBasePhaserSection1.RemoveListener(crystalSendCrystalUsedOnBasePhaserSection1MessageAction);
		CombatManager.OnCrystalUsedOnBasePhaserSection2.RemoveListener(crystalSendCrystalUsedOnBasePhaserSection2MessageAction);
		CombatManager.OnCrystalUsedOnBaseTorpedoSection.RemoveListener(crystalSendCrystalUsedOnBaseTorpedoSectionMessageAction);
		CombatManager.OnCrystalUsedOnBaseCrewSection.RemoveListener(crystalSendCrystalUsedOnBaseCrewSectionMessageAction);
		CombatManager.OnCrystalUsedOnBaseStorageSection1.RemoveListener(crystalSendCrystalUsedOnBaseStorageSection1MessageAction);
		CombatManager.OnCrystalUsedOnBaseStorageSection2.RemoveListener(crystalSendCrystalUsedOnBaseStorageSection2MessageAction);

		//remove listeners for using repair crews on ships
		CombatManager.OnRepairCrewUsedOnShipPhaserSection.RemoveListener(repairSendRepairCrewUsedOnShipPhaserSectionMessageAction);
		CombatManager.OnRepairCrewUsedOnShipTorpedoSection.RemoveListener(repairSendRepairCrewUsedOnShipTorpedoSectionMessageAction);
		CombatManager.OnRepairCrewUsedOnShipStorageSection.RemoveListener(repairSendRepairCrewUsedOnShipStorageSectionMessageAction);
		CombatManager.OnRepairCrewUsedOnShipCrewSection.RemoveListener(repairSendRepairCrewUsedOnShipCrewSectionMessageAction);
		CombatManager.OnRepairCrewUsedOnShipEngineSection.RemoveListener(repairSendRepairCrewUsedOnShipEngineSectionMessageAction);

		//remove listeners for using repair crews on bases
		CombatManager.OnRepairCrewUsedOnBasePhaserSection1.RemoveListener(repairSendRepairCrewUsedOnBasePhaserSection1MessageAction);
		CombatManager.OnRepairCrewUsedOnBasePhaserSection2.RemoveListener(repairSendRepairCrewUsedOnBasePhaserSection2MessageAction);
		CombatManager.OnRepairCrewUsedOnBaseTorpedoSection.RemoveListener(repairSendRepairCrewUsedOnBaseTorpedoSectionMessageAction);
		CombatManager.OnRepairCrewUsedOnBaseCrewSection.RemoveListener(repairSendRepairCrewUsedOnBaseCrewSectionMessageAction);
		CombatManager.OnRepairCrewUsedOnBaseStorageSection1.RemoveListener(repairSendRepairCrewUsedOnBaseStorageSection1MessageAction);
		CombatManager.OnRepairCrewUsedOnBaseStorageSection2.RemoveListener(repairSendRepairCrewUsedOnBaseStorageSection2MessageAction);

		//remove listeners for ship sections being destroyed
		PhaserSection.OnPhaserSectionDestroyed.RemoveListener(combatUnitSendShipPhaserSectionDestroyedMessageAction);
		TorpedoSection.OnTorpedoSectionDestroyed.RemoveListener(combatUnitSendShipTorpedoSectionDestroyedMessageAction);
		StorageSection.OnStorageSectionDestroyed.RemoveListener(combatUnitSendShipStorageSectionDestroyedMessageAction);
		CrewSection.OnCrewSectionDestroyed.RemoveListener(combatUnitSendShipCrewSectionDestroyedMessageAction);
		EngineSection.OnEngineSectionDestroyed.RemoveListener(combatUnitSendShipEngineSectionDestroyedMessageAction);

		//remove listeners for base sections being destroyed
		StarbasePhaserSection1.OnPhaserSection1Destroyed.RemoveListener(combatUnitSendBasePhaserSection1DestroyedMessageAction);
		StarbasePhaserSection2.OnPhaserSection2Destroyed.RemoveListener(combatUnitSendBasePhaserSection2DestroyedMessageAction);
		StarbaseTorpedoSection.OnTorpedoSectionDestroyed.RemoveListener(combatUnitSendBaseTorpedoSectionDestroyedMessageAction);
		StarbaseCrewSection.OnCrewSectionDestroyed.RemoveListener(combatUnitSendBaseCrewSectionDestroyedMessageAction);
		StarbaseStorageSection1.OnStorageSection1Destroyed.RemoveListener(combatUnitSendBaseStorageSection1DestroyedMessageAction);
		StarbaseStorageSection2.OnStorageSection2Destroyed.RemoveListener(combatUnitSendBaseStorageSection2DestroyedMessageAction);

		//remove listener for ship being destroyed
		Ship.OnShipDestroyed.RemoveListener(combatUnitSendShipDestroyedMessageAction);

		//remove listener for base being destroyed
		Starbase.OnBaseDestroyed.RemoveListener(combatUnitSendBaseDestroyedMessageAction);

		//remove listener for ship being renamed
		Ship.OnShipRenamed.RemoveListener(combatUnitSendUnitRenamedMessageAction);

		//remove listener for base being renamed
		Starbase.OnBaseRenamed.RemoveListener(combatUnitSendUnitRenamedMessageAction);

		//remove listener for income collection
		Player.OnCollectIncome.RemoveListener(playerSendIncomeMessageAction);

		//remove listener for planet owner change
		ColonyManager.OnPlanetOwnerChanged.RemoveListener(playerSendPlanetColonizedMessageAction);

		//remove listener for sunburst damage
		Sunburst.OnSunburstDamageDealt.RemoveListener(combatUnitSendSunburstDamageMessageAction);


	}

	private void SendLoadedTurnPhaseMessage(){

		//create variable for message log input
		string newMessage;

		//define the message depending on what phase it is
		if (gameManager.currentTurnPhase == GameManager.TurnPhase.PurchasePhase) {

			newMessage = "Begin Purchase Phase";

		} else if (gameManager.currentTurnPhase == GameManager.TurnPhase.MainPhase) {

			newMessage = "Begin Main Phase";

		} else {

			newMessage = "";

		}

		//build the rich text string to pass to the message log
		newMessage = AddPlayerColorToMessage(gameManager.currentTurnPlayer,newMessage);


		//add the loaded game part of the new message to the beginning
		newMessage = "Loaded Game " + "\"" + GameManager.loadedGameData.currentFileName + "\"" + "\n" + newMessage;

		//pass the message to the message log
		AddMessageToLog(newMessage);

	}

	private void SendSaveGameMessage(FileManager.SaveGameData saveGameData){

		//create variable for message log input
		string newMessage;

		//add the loaded game part of the new message to the beginning
		newMessage = "Saved Game " + "\"" + saveGameData.currentFileName + "\"";

		//pass the message to the message log
		AddMessageToLog(newMessage);

	}

	private void SendDeletedFileMessage(string fileName){

		//create variable for message log input
		string newMessage;

		//add the loaded game part of the new message to the beginning
		newMessage = "Deleted Game " + "\"" + fileName + "\"";

		//pass the message to the message log
		AddMessageToLog(newMessage);

	}

	private void SendPlayerEliminatedMessage(Player player){

		//create variable for message log input
		string newMessage;

		//add the loaded game part of the new message to the beginning
		newMessage = player.playerName + "has been eliminated from the game!";

		//pass the message to the message log
		AddMessageToLog(newMessage);

	}

}
