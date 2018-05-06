using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public abstract class Ship : CombatUnit {

	//this child class will cover all ships

	//variable for the UIManager
	//private UIManager uiManager;

	//define a shiptype property that other classes can read
	public string shipType {

		get;
		private set;

	}

		//variable to store the ship name
	public string shipName{

		get;
		private set;

	}

	//public TextMeshPros to allow the texts on the unit
	public TextMeshPro shipNameText;
	public TextMeshPro movementText;
	public TextMeshPro phasersText;


	//event to announce the ship was destroyed
	public static ShipDestroyedEvent OnShipDestroyed = new ShipDestroyedEvent();
	public static ShipDestroyedEvent OnShipDestroyedLate = new ShipDestroyedEvent();


	//event to announce ship was renamed
	public static ShipRenamedEvent OnShipRenamed = new ShipRenamedEvent();

	//class for event to announce ship was destroyed
	public class ShipDestroyedEvent : UnityEvent<CombatUnit>{};

	//class for event to announce ship was renamed
	public class ShipRenamedEvent : UnityEvent<CombatUnit, string>{};

	//unityActions
	private UnityAction<CombatUnit> combatUnitCheckShipDestroyedAction;
	private UnityAction<CombatUnit,string,GameManager.ActionMode> combatUnitRenameChangeShipNameAction;
	private UnityAction<CombatUnit> combatUnitUpdatePhaserAttackTextAction;
	private UnityAction resolveAllUnitsLoadedAction;

	//Use this for initialization
	protected override void OnInit () {

		//get the UIManager
		//uiManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();

		//check if there is a movementText script
		if (this.GetComponent<MovementText> () == true) {

			//run the Init
			this.GetComponent<MovementText> ().Init ();

		}

		//assign the shipNameText
		shipNameText.text = this.unitName;

		//set the shipName
		shipName = shipNameText.text;

		//initialize the shipType string
		shipType = this.GetShipType();

		//set the actions
		combatUnitCheckShipDestroyedAction = (combatUnit) => {CheckShipDestroyed(combatUnit);};
		combatUnitRenameChangeShipNameAction = (combatUnit, newName, previousActionMode) => {ChangeShipName(combatUnit, newName);};
		combatUnitUpdatePhaserAttackTextAction = (combatUnit) => {UpdatePhaserAttackText(combatUnit);};
		resolveAllUnitsLoadedAction = () => {ResolveAllUnitsLoaded();};

		//add listeners for various sections of the ship getting destroyed
		PhaserSection.OnPhaserSectionDestroyed.AddListener(combatUnitCheckShipDestroyedAction);
		TorpedoSection.OnTorpedoSectionDestroyed.AddListener(combatUnitCheckShipDestroyedAction);
		StorageSection.OnStorageSectionDestroyed.AddListener(combatUnitCheckShipDestroyedAction);
		CrewSection.OnCrewSectionDestroyed.AddListener(combatUnitCheckShipDestroyedAction);
		EngineSection.OnEngineSectionDestroyed.AddListener(combatUnitCheckShipDestroyedAction);

		//add listener for the rename event
		uiManager.GetComponent<RenameShip>().OnRenameUnit.AddListener(combatUnitRenameChangeShipNameAction);

		//add listener for the update attack status event
		CombatUnit.OnUpdateAttackStatus.AddListener(combatUnitUpdatePhaserAttackTextAction);

		//check if there is a sunburst script
		if (this.GetComponent<Sunburst> () == true) {

			//run the Init
			this.GetComponent<Sunburst> ().Init ();

		}

		//add listener for all units done being loaded
		gameManager.OnAllLoadedUnitsCreated.AddListener(resolveAllUnitsLoadedAction);

		//run OnInitLevel2 so subclasses will run it
		OnInitLevel2();

	}

	//this is required so that subclasses inheriting from Ship (starbase and ship) can run OnInitLevel2 instead of start
	protected virtual void OnInitLevel2(){
		

	}
	
	//this function gets the ship type
	private string GetShipType(){

		//create a variable to hold the output
		string shipTypeString;

		//check to see what component the ship has to determine the ship type
		if (this.GetComponent<Starship> () == true) {

			shipTypeString = "Starship";

		} else if (this.GetComponent<Destroyer> () == true) {

			shipTypeString = "Destroyer";

		} else if (this.GetComponent<BirdOfPrey> () == true) {

			shipTypeString = "Bird of Prey";

		} else if (this.GetComponent<Scout> () == true) {

			shipTypeString = "Scout";

		} else {

			shipTypeString = "";

		}

		return shipTypeString;

	}

	//this function takes a combat unit UnitType and converts to a ship type string
	public static string GetShipTypeFromUnitType(CombatUnit.UnitType unitType){
		
		//create a variable to hold the output
		string shipTypeString;

		//switch case
		switch (unitType) {

		case CombatUnit.UnitType.Scout:

			shipTypeString = "Scout";

			break;

		case CombatUnit.UnitType.BirdOfPrey:

			shipTypeString = "Bird Of Prey";

			break;

		case CombatUnit.UnitType.Destroyer:

			shipTypeString = "Destroyer";

			break;

		case CombatUnit.UnitType.Starbase:

			shipTypeString = "Starbase";

			break;

		default:

			shipTypeString = "";

			break;


		}

		return shipTypeString;

	}

	//this function checks if the ship is destroyed when a section gets destroyed
	private void CheckShipDestroyed(CombatUnit combatUnit){

		//first, check if the section destroyed was on this combat unit
		if (this.GetComponent<CombatUnit> () == combatUnit) {

			//find out what type of ship we have
			switch (combatUnit.unitType) {

			case CombatUnit.UnitType.Starship:
				//if all sections are destroyed
				if (combatUnit.GetComponent<PhaserSection> ().isDestroyed == true &&
				    combatUnit.GetComponent<TorpedoSection> ().isDestroyed == true &&
				    combatUnit.GetComponent<StorageSection> ().isDestroyed == true &&
				    combatUnit.GetComponent<CrewSection> ().isDestroyed == true &&
				    combatUnit.GetComponent<EngineSection> ().isDestroyed == true) {

					//invoke the ShipDestroyed event
					OnShipDestroyed.Invoke (combatUnit);

					//remove all listeners
					RemoveAllListeners ();

					//destroy this gameobject
					Destroy(this.gameObject);

					//invoke the ShipDestroyed event
					OnShipDestroyedLate.Invoke (combatUnit);

				}
				break;
			case CombatUnit.UnitType.Destroyer:
				//if all sections are destroyed
				if (combatUnit.GetComponent<PhaserSection> ().isDestroyed == true &&
					combatUnit.GetComponent<TorpedoSection> ().isDestroyed == true &&
					combatUnit.GetComponent<StorageSection> ().isDestroyed == true &&
					combatUnit.GetComponent<EngineSection> ().isDestroyed == true) {

					//invoke the ShipDestroyed event
					OnShipDestroyed.Invoke (combatUnit);

					//remove all listeners
					RemoveAllListeners ();

					//destroy this gameobject
					Destroy(this.gameObject);

					//invoke the ShipDestroyed event
					OnShipDestroyedLate.Invoke (combatUnit);

				}
				break;

			case CombatUnit.UnitType.BirdOfPrey:
				//if all sections are destroyed
				if (combatUnit.GetComponent<PhaserSection> ().isDestroyed == true &&
					combatUnit.GetComponent<TorpedoSection> ().isDestroyed == true &&
					combatUnit.GetComponent<EngineSection> ().isDestroyed == true) {

					//invoke the ShipDestroyed event
					OnShipDestroyed.Invoke (combatUnit);

					//remove all listeners
					RemoveAllListeners ();

					//destroy this gameobject
					Destroy(this.gameObject);

					//invoke the ShipDestroyed event
					OnShipDestroyedLate.Invoke (combatUnit);

				}
				break;

			case CombatUnit.UnitType.Scout:
				//if all sections are destroyed
				if (combatUnit.GetComponent<PhaserSection> ().isDestroyed == true &&
					combatUnit.GetComponent<StorageSection> ().isDestroyed == true &&
					combatUnit.GetComponent<EngineSection> ().isDestroyed == true) {

					//invoke the ShipDestroyed event
					OnShipDestroyed.Invoke (combatUnit);

					//remove all listeners
					RemoveAllListeners ();

					//destroy this gameobject
					Destroy(this.gameObject);

					//invoke the ShipDestroyed event
					OnShipDestroyedLate.Invoke (combatUnit);

				}
				break;

			default:
				break;
			}

		}

	}

	//this function changes the ship name
	private void ChangeShipName(CombatUnit combatUnit, string newName){

		//check if this is the unit that is being changed
		if (this.GetComponent<CombatUnit> () == combatUnit) {

			//cache the old ship name
			string oldShipName = this.shipName;

			//update the ship name
			this.shipName = newName;

			//update the shipName text
			this.shipNameText.text = newName;

			//invoke the onRenamed event
			OnShipRenamed.Invoke(this.GetComponent<CombatUnit> (),oldShipName);

		}

	}

	//this function updates the phaser attack text
	private void UpdatePhaserAttackText(CombatUnit combatUnit){

		//check if this is the combat unit that was passed
		if (this.GetComponent<CombatUnit> () == combatUnit) {

			//check if we have a valid phaser attack
			if (this.GetComponent<CombatUnit> ().hasRemainingPhaserAttack == true) {

				phasersText.text = "P";
			} else if (this.GetComponent<CombatUnit> ().hasRemainingPhaserAttack == false) {

				phasersText.text = "";

			}

		}

	}

	//this function resolves an all units loaded signal
	private void ResolveAllUnitsLoaded(){

		UpdatePhaserAttackText (this.GetComponent<CombatUnit> ());

	}

	//this function handles OnDestroy
	private void OnDestroy(){

		RemoveAllListeners ();

	}

	//this function removes all listeners
	private void RemoveAllListeners(){

		//remove listeners for various sections of the ship getting destroyed
		PhaserSection.OnPhaserSectionDestroyed.RemoveListener (combatUnitCheckShipDestroyedAction);
		TorpedoSection.OnTorpedoSectionDestroyed.RemoveListener (combatUnitCheckShipDestroyedAction);
		StorageSection.OnStorageSectionDestroyed.RemoveListener (combatUnitCheckShipDestroyedAction);
		CrewSection.OnCrewSectionDestroyed.RemoveListener (combatUnitCheckShipDestroyedAction);
		EngineSection.OnEngineSectionDestroyed.RemoveListener (combatUnitCheckShipDestroyedAction);

		if (uiManager != null) {
			
			//remove listener for the rename event
			uiManager.GetComponent<RenameShip> ().OnRenameUnit.RemoveListener (combatUnitRenameChangeShipNameAction);

		}

		//remove listener for the update attack status event
		CombatUnit.OnUpdateAttackStatus.RemoveListener (combatUnitUpdatePhaserAttackTextAction);

		if (gameManager != null) {

			//remove listener for all units done being loaded
			gameManager.OnAllLoadedUnitsCreated.RemoveListener (resolveAllUnitsLoadedAction);

		}

	}

}
