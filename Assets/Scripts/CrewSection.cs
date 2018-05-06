using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CrewSection : MonoBehaviour {

	//variables for the managers
	private UIManager uiManager;
	private GameManager gameManager;
	private MouseManager mouseManager;

	//variable to hold the reference to the tileMap
	private TileMap tileMap;

	//variables that the crew section must manage
	public bool repairCrew {

		get;
		private set;

	}

	public bool shieldEngineeringTeam {

		get;
		private set;

	}

	public bool battleCrew {

		get;
		private set;

	}

	//bool to track whether the section is destroyed or not
	public bool isDestroyed {

		get;
		private set;

	}

	//variable to hold the shields value of the section
	public int shieldsMax {

		get;
		private set;

	}

	public int shieldsCurrent {

		get;
		private set;

	}

	//variable to hold whether the repair crew has already been used this turn
	public bool usedRepairCrewThisTurn {

		get;
		private set;

	}

	//this is the list of hexes that the crew can reach
	public List<Hex> TargetableCrewHexes {
		get;
		private set;
	}

	//define a variable to control crew range
	public int crewRange {

		get;
		private set;

	}

	//event to announce the section was destroyed
	public static CrewSectionDestroyedEvent OnCrewSectionDestroyed = new CrewSectionDestroyedEvent();

	//event to announce the section was repaired
	public static CrewSectionDestroyedEvent OnCrewSectionRepaired = new CrewSectionDestroyedEvent();

	//event to announce repair crew was used
	public static UseRepairCrewEvent OnUseRepairCrew = new UseRepairCrewEvent();

	//class for event to announce section was destroyed
	public class CrewSectionDestroyedEvent : UnityEvent<CombatUnit>{};

	//class for passing data to crystal events
	public class UseRepairCrewEvent : UnityEvent<CombatUnit,CombatUnit,string>{};

	//event to announce damage was taken
	public static UnityEvent OnCrewDamageTaken = new UnityEvent();

	//event to announce inventory updated
	public static InventoryUpdatedEvent OnInventoryUpdated = new InventoryUpdatedEvent();
	public class InventoryUpdatedEvent : UnityEvent<CombatUnit>{};

	//unityActions
	private UnityAction<Player.Color> playerColorEndTurnAction;
	private UnityAction<CombatUnit,CombatUnit,int> attackHitTakeDamageAction;
	private UnityAction<CombatUnit,CombatUnit,CombatManager.CrystalType,int> crystalUsedHealDamageAction;
	private UnityAction<CombatUnit,CombatUnit,string> combatUnitUsedRepairCrewAction;
	private UnityAction calculateCrewRangeMouseManagerSelectedUnitAction;
	private UnityAction<Ship> shipCalculateCrewRangeAction;
	private UnityAction<CombatUnit,CombatUnit> repairCrewUsedOnSectionAction;
	private UnityAction<CombatUnit> combatUnitCombatUnitDestroyedAction;
	private UnityAction<Dictionary<string,int>,int,CombatUnit> purchaseAddPurchaseItemsAction;
	private UnityAction<CombatUnit,int> incidentalTakeDamageAction;
	private UnityAction<CombatUnit,FileManager.SaveGameData> saveDataResolveLoadedUnitAction;


	// Use this for initialization
	public void Init () {

		//set the actions
		playerColorEndTurnAction = (color) => {EndTurn(color);};
		attackHitTakeDamageAction = (attackingUnit, targetedUnit, damage) => {TakeDamage(targetedUnit,damage);};
		crystalUsedHealDamageAction = (selectedUnit,targetedUnit,crystalType,shieldsHealed) => {HealDamage(targetedUnit,shieldsHealed);};
		combatUnitUsedRepairCrewAction = (selectedUnit, targetedUnit, sectionTargetedString) => {
			UseRepairCrew (selectedUnit, targetedUnit, sectionTargetedString);
		};

		calculateCrewRangeMouseManagerSelectedUnitAction = () => {CalculateCrewRange(mouseManager.selectedUnit);};
		shipCalculateCrewRangeAction = (ship) => {CalculateCrewRange(ship.gameObject);};
		repairCrewUsedOnSectionAction = (selectedUnit,targetedUnit) => {RepairSection(targetedUnit);};
		combatUnitCombatUnitDestroyedAction = (combatUnit) => {CombatUnitDestroyed(combatUnit);};
		purchaseAddPurchaseItemsAction = (purchasedItems,purchasedValue,combatUnit) => {AddPurchasedItems(purchasedItems,combatUnit);};
		incidentalTakeDamageAction = (combatUnit, damage) => {TakeDamage(combatUnit,damage);};
		saveDataResolveLoadedUnitAction = (combatUnit,saveGameData) => {ResolveLoadedUnit(combatUnit,saveGameData);};

		//get the managers
		uiManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
		gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
		mouseManager = GameObject.FindGameObjectWithTag("MouseManager").GetComponent<MouseManager>();

		//find tileMap in the game
		tileMap = GameObject.FindGameObjectWithTag ("TileMap").GetComponent<TileMap> ();

		//set used repair crew to false
		this.usedRepairCrewThisTurn = false;

		//set the starting inventory based on the type of ship we have
		switch (this.GetComponent<CombatUnit> ().unitType) {

		case CombatUnit.UnitType.Starship:
			this.repairCrew = Starship.startingRepairCrew;
			this.shieldEngineeringTeam = Starship.startingShieldEngineeringTeam;
			this.battleCrew = Starship.startingBattleCrew;
			this.shieldsMax = Starship.crewSectionShieldsMax;
			this.shieldsCurrent = shieldsMax;
			break;
		case CombatUnit.UnitType.Destroyer:
			Debug.LogError ("A Crew Section is Attached to a Destroyer");
			this.repairCrew = false;
			this.shieldEngineeringTeam = false;
			this.battleCrew = false;
			this.shieldsMax = 40;
			this.shieldsCurrent = shieldsMax;
			break;
		case CombatUnit.UnitType.BirdOfPrey:
			Debug.LogError ("A Crew Section is Attached to a Bird of Prey");
			this.repairCrew = false;
			this.shieldEngineeringTeam = false;
			this.battleCrew = false;
			this.shieldsMax = 40;
			this.shieldsCurrent = shieldsMax;
			break;
		case CombatUnit.UnitType.Scout:
			Debug.LogError ("A Crew Section is Attached to a Scout");
			this.repairCrew = false;
			this.shieldEngineeringTeam = false;
			this.battleCrew = false;
			this.shieldsMax = 40;
			this.shieldsCurrent = shieldsMax;
			break;
		default:
			this.repairCrew = false;
			this.shieldEngineeringTeam = false;
			this.battleCrew = false;
			this.shieldsMax = 40;
			this.shieldsCurrent = shieldsMax;
			break;

		}

		//add listener for end turn
		gameManager.OnEndTurn.AddListener(playerColorEndTurnAction);

		//add listener for getting hit by phaser attack
		//CombatManager.OnPhaserAttackHitShipCrewSection.AddListener(attackHitTakeDamageAction);
		CutsceneManager.OnPhaserHitShipCrewSection.AddListener(attackHitTakeDamageAction);

		//add listener for getting hit by torpedo attack
		//CombatManager.OnLightTorpedoAttackHitShipCrewSection.AddListener(attackHitTakeDamageAction);
		//CombatManager.OnHeavyTorpedoAttackHitShipCrewSection.AddListener(attackHitTakeDamageAction);
		CutsceneManager.OnLightTorpedoHitShipCrewSection.AddListener(attackHitTakeDamageAction);
		CutsceneManager.OnHeavyTorpedoHitShipCrewSection.AddListener(attackHitTakeDamageAction);

		//add listener for getting healed by a crystal
		CombatManager.OnCrystalUsedOnShipCrewSection.AddListener(crystalUsedHealDamageAction);

		//add listeners for using repair crew
		uiManager.GetComponent<CrewMenu> ().OnUseRepairCrew.AddListener (combatUnitUsedRepairCrewAction);

		//add listener to the ealy setSelectedUnit so range calculations can occur before other listeners react
		mouseManager.OnSetSelectedUnitEarly.AddListener(calculateCrewRangeMouseManagerSelectedUnitAction);

		//add listener for finishing movement - we want to recalculate the ranges if we move
		EngineSection.OnMoveFinish.AddListener(shipCalculateCrewRangeAction);

		//add listener for getting repaired by a repair crew
		CombatManager.OnRepairCrewUsedOnShipCrewSection.AddListener(repairCrewUsedOnSectionAction);

		//add listener for the combat unit being destroyed
		Ship.OnShipDestroyed.AddListener(combatUnitCombatUnitDestroyedAction);

		//add listener for purchasing items
		uiManager.GetComponent<PurchaseManager>().OnPurchaseItems.AddListener(purchaseAddPurchaseItemsAction);

		//check if this unit came with outfitted items
		if (this.GetComponent<CombatUnit> ().outfittedItemsAtPurchase != null) {

			//add the items
			AddPurchasedItems(this.GetComponent<CombatUnit> ().outfittedItemsAtPurchase,this.GetComponent<CombatUnit> ());

		}

		//add listener for sunburst damage
		Sunburst.OnSunburstDamageDealt.AddListener(incidentalTakeDamageAction);

		//add listener for creating unit from load
		CombatUnit.OnCreateLoadedUnit.AddListener(saveDataResolveLoadedUnitAction);

	}
	
	//this function is called to use a repair crew
	private void UseRepairCrew(CombatUnit selectedUnit, CombatUnit targetedUnit, string sectionTargetedString){

		//first, check if this is the selected unit
		if (this.GetComponent<CombatUnit> () == selectedUnit) {

			//this is the selected unit - we can set the usedRepairCrew flag to true
			this.usedRepairCrewThisTurn = true;

			//invoke the useRepairCrew event
			OnUseRepairCrew.Invoke(selectedUnit,targetedUnit,sectionTargetedString);

		}

	}

	//this function will calculate all hexes a ship can reach in range
	private void CalculateCrewRange(GameObject gameObject){

		//check if the gameObject passed to the function is a ship
		if (gameObject.GetComponent<Ship> () == true) {

			//if it is a ship, check if it is this ship
			if (gameObject.GetComponent<Ship> () == this.GetComponent<Ship>()) {

				//get a list of all hex locations that are reachable
				//set last argument to true so it includes the hex that the ship is on
				TargetableCrewHexes = tileMap.TargetableTiles (this.GetComponent<CombatUnit>().currentLocation,crewRange, true);

				//create a list of hexes to exclude from targeting
				List<Hex> HexesToRemoveFromCrewTargeting = new List<Hex>();

				//check the targetable hexes and remove any that are occupied by cloaked combat units
				foreach (Hex hex in TargetableCrewHexes) {

					//check if the tile has a cloaked unit
					if (tileMap.HexMap [hex].tileCombatUnit != null && tileMap.HexMap [hex].tileCombatUnit.GetComponent<CloakingDevice>() == true &&
						tileMap.HexMap [hex].tileCombatUnit.GetComponent<CloakingDevice>().isCloaked == true) {

						//if it does, remove it from the targetable hexes
						HexesToRemoveFromCrewTargeting.Add(hex);

					}

				}

				//remove the hexes from the targetable hexes
				foreach (Hex hex in HexesToRemoveFromCrewTargeting) {

					TargetableCrewHexes.Remove (hex);

				}

			}

		}

	}

	//this function will handle taking damage if the section is hit by an attack
	private void TakeDamage(CombatUnit targetedUnit, int damage){

		//first, check if the unit that the crew section is attached to is the targeted unit that got hit
		if (this.GetComponent<CombatUnit> () == targetedUnit) {

			//this is the targeted unit - we can reduce the shields by the damage
			this.shieldsCurrent -= damage;

			//check if this puts shields at or below zero - if so, set to zero and call DestroySection
			if (shieldsCurrent <= 0) {

				shieldsCurrent = 0;
				this.DestroySection ();

			}

			OnCrewDamageTaken.Invoke ();

		}

	}

	//this function will handle healing damage if the section has a crystal used on it
	private void HealDamage(CombatUnit targetedUnit, int shieldsHealed){

		//first, check if the unit that the engine section is attached to is the targeted unit that got hit
		if (this.GetComponent<CombatUnit> () == targetedUnit) {

			//this is the targeted unit - we can increase the shields by the amount of healing
			this.shieldsCurrent += shieldsHealed;

			//check if this puts shields at or above max - if so, set to max
			if (shieldsCurrent > shieldsMax) {

				shieldsCurrent = shieldsMax;
				Debug.LogError ("We somehow healed this section for more than max shields");

			}

			OnCrewDamageTaken.Invoke ();


		}

	}

	//this function will handle repairing the section from a repair crew
	private void RepairSection(CombatUnit targetedUnit){

		//first, check if the unit that the section is attached to is the targeted unit that got hit
		if (this.GetComponent<CombatUnit> () == targetedUnit) {

			//check if the section wasn't destroyed - that would be a logic error
			if (this.isDestroyed == false) {

				Debug.LogError ("We somehow tried to repair a section that wasn't destroyed");

			}

			//this is the targeted unit - we can repair the section and restore the shields to 1
			this.isDestroyed = false;
			this.shieldsCurrent = 1;

			//invoke the repaired section event
			OnCrewSectionRepaired.Invoke(this.GetComponent<CombatUnit>());

		}

	}

	//this function handles the section taking lethal damage and being destroyed
	private void DestroySection (){

		//if the section is destroyed, all the inventory on this section is removed
		this.repairCrew = false;
		this.shieldEngineeringTeam = false;
		this.battleCrew = false;

		//set the isDestroyed flag to true
		this.isDestroyed = true;

		//invoke the destroyed section event
		OnCrewSectionDestroyed.Invoke(this.GetComponent<CombatUnit>());

	}

	//this function will clean up variables at end of turn
	private void EndTurn(Player.Color currentTurn){

		//check if the color passed to the function matches the owner color
		//if so, our turn is ending, and we need to reset stuff
		if (currentTurn == this.GetComponent<Ship>().owner.color) {

			this.usedRepairCrewThisTurn = false;

		}

	}

	//this function will remove all listeners when the combat unit is destroyed
	private void CombatUnitDestroyed(CombatUnit combatUnitDestroyed){

		//check if the passed combat unit is this combat unit
		if (this.GetComponent<CombatUnit> () == combatUnitDestroyed) {
			
			RemoveAllListeners ();

		}

	}

	//this function handles adding items via purchase
	private void AddPurchasedItems(Dictionary<string,int> purchasedItems, CombatUnit combatUnit){

		//check if this is the unit passed
		if (combatUnit == this.GetComponent<CombatUnit> ()) {
			
			if (purchasedItems.ContainsKey ("Repair Crew")) {

				//increase the quantity by the value
				this.repairCrew = true;

			}

			if (purchasedItems.ContainsKey ("Shield Engineering Team")) {

				//increase the quantity by the value
				this.shieldEngineeringTeam = true;

			}

			if (purchasedItems.ContainsKey ("Additional Battle Crew")) {

				//increase the quantity by the value
				this.battleCrew = true;

			}

			//invoke event
			OnInventoryUpdated.Invoke (this.GetComponent<CombatUnit>());

		}

	}

	//this function resolves a loaded unit
	private void ResolveLoadedUnit(CombatUnit combatUnit, FileManager.SaveGameData saveGameData){

		//first, check that the unit matches
		if (this.GetComponent<CombatUnit> () == combatUnit) {

			//if we have the right unit, use the saveGameData to set values
			//need to find the index of the player colors from the saveGameData file
			for (int i = 0; i < GameManager.numberPlayers; i++) {

				//check if the color matches
				if (saveGameData.playerColor [i] == combatUnit.owner.color) {

					//we have found the right index for this player
					//now we need to check what kind of ship we're attached to
					switch (combatUnit.unitType) {

					case CombatUnit.UnitType.Starship:

						this.isDestroyed = saveGameData.starshipCrewSectionIsDestroyed [i, combatUnit.serialNumber];
						this.shieldsCurrent = saveGameData.starshipCrewSectionShieldsCurrent [i, combatUnit.serialNumber];
						this.repairCrew = saveGameData.starshipRepairCrew [i, combatUnit.serialNumber];
						this.shieldEngineeringTeam = saveGameData.starshipShieldEngineeringTeam [i, combatUnit.serialNumber];
						this.battleCrew = saveGameData.starshipBattleCrew [i, combatUnit.serialNumber];
						this.usedRepairCrewThisTurn = saveGameData.starshipUsedRepairCrewThisTurn [i, combatUnit.serialNumber];

						break;

					default:

						break;

					}

				}

			}

		}

	}

	//this function handles OnDestroy
	private void OnDestroy(){

		RemoveAllListeners ();

	}

	//this function removes all listeners
	private void RemoveAllListeners(){

		if (gameManager != null) {

			//remove listener for end turn
			gameManager.OnEndTurn.RemoveListener (playerColorEndTurnAction);

		}

		//remove listener for getting hit by phaser attack
		//CombatManager.OnPhaserAttackHitShipCrewSection.RemoveListener(attackHitTakeDamageAction);
		CutsceneManager.OnPhaserHitShipCrewSection.RemoveListener(attackHitTakeDamageAction);

		//remove listener for getting hit by torpedo attack
		//CombatManager.OnLightTorpedoAttackHitShipCrewSection.RemoveListener(attackHitTakeDamageAction);
		//CombatManager.OnHeavyTorpedoAttackHitShipCrewSection.RemoveListener(attackHitTakeDamageAction);
		CutsceneManager.OnLightTorpedoHitShipCrewSection.RemoveListener(attackHitTakeDamageAction);
		CutsceneManager.OnHeavyTorpedoHitShipCrewSection.RemoveListener(attackHitTakeDamageAction);

		//remove listener for getting healed by a crystal
		CombatManager.OnCrystalUsedOnShipCrewSection.RemoveListener(crystalUsedHealDamageAction);

		if (uiManager != null) {

			//remove listeners for using repair crew
			uiManager.GetComponent<CrewMenu> ().OnUseRepairCrew.RemoveListener (combatUnitUsedRepairCrewAction);

			//remove listener for purchasing items
			uiManager.GetComponent<PurchaseManager>().OnPurchaseItems.RemoveListener(purchaseAddPurchaseItemsAction);

		}

		if (mouseManager != null) {
			
			//remove listener to the ealy setSelectedUnit so range calculations can occur before other listeners react
			mouseManager.OnSetSelectedUnitEarly.RemoveListener (calculateCrewRangeMouseManagerSelectedUnitAction);

		}

		//remove listener for finishing movement - we want to recalculate the ranges if we move
		EngineSection.OnMoveFinish.RemoveListener(shipCalculateCrewRangeAction);

		//remove listener for getting repaired by a repair crew
		CombatManager.OnRepairCrewUsedOnShipCrewSection.RemoveListener(repairCrewUsedOnSectionAction);

		//remove listener for the combat unit being destroyed
		Ship.OnShipDestroyed.RemoveListener(combatUnitCombatUnitDestroyedAction);

		//remove listener for sunburst damage
		Sunburst.OnSunburstDamageDealt.RemoveListener(incidentalTakeDamageAction);

		//remove listener for creating unit from load
		CombatUnit.OnCreateLoadedUnit.RemoveListener(saveDataResolveLoadedUnitAction);

	}

}
