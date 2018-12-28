using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HexMapTile{

	//manager
	private GameManager gameManager;

	//boolean value for whether tile is passable
	public bool isPassable {
		get;
		private set;
	}

	//variable to hold the combat unit located at this tile
	public CombatUnit tileCombatUnit {

		get;
		private set;

	}

	//Hex class location
	public Hex hexLocation {
		get;
		private set;
	}

	//hex array index for use with UV manipulation
	public int hexIndex {
		get;
		private set;
	}

	//tilemap object that tile is associated with
	public TileMap tileMap {
		get;
		private set;
	}

	//movement cost of a specific tile
	//for this game, any passable tile = 1
	public float movementCost{
		get;
		private set;
	}

	//bool to keep track of whether the tile is a planet
	public bool isPlanet {

		get;
		private set;

	}

	//bool to keep track of whether the tile is a planet
	public bool isColonized {

		get;
		private set;

	}

	//public color to keep track of which player has colonized the planet
	public Player.Color colonizingColor {

		get;
		private set;

	}
		

	//this enum will define the types of tiles that can exist in the map
	public enum TileType {
		EmptySpace,
		GreenStart,
		PurpleStart,
		RedStart,
		BlueStart,
		Sun,
		SunRayUpRight,
		SunRayRight,
		SunRayDownRight,
		SunRayDownLeft,
		SunRayLeft,
		SunRayUpLeft,
		Asteroid,
		Mercury,
		Venus,
		Earth,
		Mars,
		Jupiter,
		Saturn,
		Uranus,
		Neptune,
		Pluto,
		Charon,
        Eris,
        Ceres,
		MercuryLow,
		VenusLow,
		EarthLow,
		MarsLow,
		JupiterLow,
		SaturnLow,
		UranusLow,
		NeptuneLow,
		PlutoLow,
		CharonLow,
        ErisLow,
        CeresLow,
		RedWormhole,
		BlueWormhole,
		NeutralStarbase,
	}

	//each HexMapTile will have a tile type property
	private TileType _tileType;
	public TileType tileType {
		get { return _tileType; }
		private set {
			_tileType = value;

			switch (_tileType) {
			case TileType.Asteroid:
				isPassable = false;
				break;
			case TileType.NeutralStarbase:
				isPassable = false;
				break;
			case TileType.Sun:
				isPassable = false;
				break;
			default:
				break;
			}
		}
	}

	//this event is used to announce a planet has been colonized
	public static ColonizePlanetEvent OnColonizeNewPlanet = new ColonizePlanetEvent();

	//simple class derived from unityEvent to pass hexMapTile
	public class ColonizePlanetEvent : UnityEvent<HexMapTile,Player.Color>{};

	//unityActions
	private UnityAction<Ship> shipUpdatePassableOnMoveStartAction;
	private UnityAction<Ship> shipUpdatePassableOnMoveFinishAction;
	private UnityAction<CombatUnit> combatUnitUpdatePassableOnNewUnitAction;
	private UnityAction<CombatUnit,FileManager.SaveGameData> combatUnitLoadedUpdatePassableOnNewUnitAction;
	private UnityAction<CombatUnit> combatUnitTemporarilyMakeTargetPassableAction;
	private UnityAction<CombatUnit> combatUnitTemporarilyMakeTargetImpassableAction;
	private UnityAction<CombatUnit> combatUnitClearUnitFromTileAction;
	private UnityAction<Player.Color> colorCheckPlanetOccupationStatusAction;
	private UnityAction<HexMapTile,Player.Color> hexMapTileLoadColonyAction;

	// Use this constructor for initialization
	public HexMapTile(Hex hex, int hexIndex, TileMap _tileMap, TileType tileType){
		//when a HexMapTile is instantiated, set it's hex location based on the hex argument
		this.hexLocation = hex;
		this.hexIndex = hexIndex;
		this.tileMap = _tileMap;
		this.tileType = tileType;
		this.tileCombatUnit = null;
		this.isColonized = false;

		switch (tileType) {
		case TileType.EmptySpace:
			this.movementCost = 1.0f;
			this.isPlanet = false;
			this.isPassable = true;
			break;
		case TileType.GreenStart:
			this.movementCost = 1.0f;
			this.isPlanet = false;
			this.isPassable = true;
			break;
		case TileType.PurpleStart:
			this.movementCost = 1.0f;
			this.isPlanet = false;
			this.isPassable = true;
			break;
		case TileType.RedStart:
			this.movementCost = 1.0f;
			this.isPlanet = false;
			this.isPassable = true;
			break;
		case TileType.BlueStart:
			this.movementCost = 1.0f;
			this.isPlanet = false;
			this.isPassable = true;
			break;
		case TileType.Sun:
			this.movementCost = Mathf.Infinity;
			this.isPlanet = false;
			this.isPassable = false;
			break;
		case TileType.SunRayUpRight:
			this.movementCost = 1.0f;
			this.isPlanet = false;
			this.isPassable = true;
			break;
		case TileType.SunRayRight:
			this.movementCost = 1.0f;
			this.isPlanet = false;
			this.isPassable = true;
			break;
		case TileType.SunRayDownRight:
			this.movementCost = 1.0f;
			this.isPlanet = false;
			this.isPassable = true;
			break;
		case TileType.SunRayDownLeft:
			this.movementCost = 1.0f;
			this.isPlanet = false;
			this.isPassable = true;
			break;
		case TileType.SunRayLeft:
			this.movementCost = 1.0f;
			this.isPlanet = false;
			this.isPassable = true;
			break;
		case TileType.SunRayUpLeft:
			this.movementCost = 1.0f;
			this.isPlanet = false;
			this.isPassable = true;
			break;
		case TileType.Asteroid:
			this.movementCost = Mathf.Infinity;
			this.isPlanet = false;
			this.isPassable = false;
			break;
		case TileType.Mercury:
			this.movementCost = 1.0f;
			this.isPlanet = true;
			this.isPassable = true;
			break;
		case TileType.Venus:
			this.movementCost = 1.0f;
			this.isPlanet = true;
			this.isPassable = true;
			break;
		case TileType.Earth:
			this.movementCost = 1.0f;
			this.isPlanet = true;
			this.isPassable = true;
			break;
		case TileType.Mars:
			this.movementCost = 1.0f;
			this.isPlanet = true;
			this.isPassable = true;
			break;
		case TileType.Jupiter:
			this.movementCost = 1.0f;
			this.isPlanet = true;
			this.isPassable = true;
			break;
		case TileType.Saturn:
			this.movementCost = 1.0f;
			this.isPlanet = true;
			this.isPassable = true;
			break;
		case TileType.Uranus:
			this.movementCost = 1.0f;
			this.isPlanet = true;
			this.isPassable = true;
			break;
		case TileType.Neptune:
			this.movementCost = 1.0f;
			this.isPlanet = true;
			this.isPassable = true;
			break;
		case TileType.Pluto:
			this.movementCost = 1.0f;
			this.isPlanet = true;
			this.isPassable = true;
			break;
		case TileType.Charon:
			this.movementCost = 1.0f;
			this.isPlanet = true;
            this.isPassable = true;
			break;
        case TileType.Eris:
            this.movementCost = 1.0f;
            this.isPlanet = true;
            this.isPassable = true;
            break;
        case TileType.Ceres:
            this.movementCost = 1.0f;
            this.isPlanet = true;
            this.isPassable = true;
            break;
            case TileType.MercuryLow:
			this.movementCost = 1.0f;
			this.isPlanet = true;
			this.isPassable = true;
			break;
		case TileType.VenusLow:
			this.movementCost = 1.0f;
			this.isPlanet = true;
			this.isPassable = true;
			break;
		case TileType.EarthLow:
			this.movementCost = 1.0f;
			this.isPlanet = true;
			this.isPassable = true;
			break;
		case TileType.MarsLow:
			this.movementCost = 1.0f;
			this.isPlanet = true;
			break;
		case TileType.JupiterLow:
			this.movementCost = 1.0f;
			this.isPlanet = true;
			this.isPassable = true;
			break;
		case TileType.SaturnLow:
			this.movementCost = 1.0f;
			this.isPlanet = true;
			this.isPassable = true;
			break;
		case TileType.UranusLow:
			this.movementCost = 1.0f;
			this.isPlanet = true;
			this.isPassable = true;
			break;
		case TileType.NeptuneLow:
			this.movementCost = 1.0f;
			this.isPlanet = true;
			this.isPassable = true;
			break;
		case TileType.PlutoLow:
			this.movementCost = 1.0f;
			this.isPlanet = true;
			this.isPassable = true;
			break;
		case TileType.CharonLow:
			this.movementCost = 1.0f;
			this.isPlanet = true;
			this.isPassable = true;
			break;
        case TileType.ErisLow:
            this.movementCost = 1.0f;
            this.isPlanet = true;
            this.isPassable = true;
            break;
        case TileType.CeresLow:
            this.movementCost = 1.0f;
            this.isPlanet = true;
            this.isPassable = true;
            break;
            case TileType.BlueWormhole:
			this.movementCost = 1.0f;
			this.isPlanet = false;
			this.isPassable = true;
			break;
		case TileType.RedWormhole:
			this.movementCost = 1.0f;
			this.isPlanet = false;
			this.isPassable = true;
			break;
		case TileType.NeutralStarbase:
			this.movementCost = Mathf.Infinity;
			this.isPlanet = false;
			this.isPassable = false;
			break;
		default:
			this.movementCost = 1.0f;
			this.isPlanet = false;
			break;

		}


		//get a reference to the gameManager
		gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

		//set the actions
		shipUpdatePassableOnMoveStartAction = (ship) => {UpdatePassableOnMoveStart(ship);};
		shipUpdatePassableOnMoveFinishAction = (ship) => {UpdatePassableOnMoveFinish(ship);};
		combatUnitUpdatePassableOnNewUnitAction = (combatUnit) => {UpdatePassableOnNewUnit(combatUnit);};
		combatUnitLoadedUpdatePassableOnNewUnitAction = (combatUnit,saveGameData) => {UpdatePassableOnNewUnit(combatUnit);};
		combatUnitTemporarilyMakeTargetPassableAction = (combatUnit) => {TemporaryMakeTargetPassable(combatUnit);};
		combatUnitTemporarilyMakeTargetImpassableAction = (combatUnit) => {TemporaryMakeTargetImpassable(combatUnit);};
		combatUnitClearUnitFromTileAction = (combatUnit) => {ClearUnitFromTile(combatUnit);};
		colorCheckPlanetOccupationStatusAction = (endTurnColor) => {CheckPlanetOccupationStatus(endTurnColor);};
		hexMapTileLoadColonyAction = (hexMapTile,playerColor) => {LoadColony(hexMapTile,playerColor);};

		//add event listeners for ship movement start
		EngineSection.OnMoveStart.AddListener(shipUpdatePassableOnMoveStartAction);

		//add event listeners for ship movement finish
		EngineSection.OnMoveFinishTileStatus.AddListener(shipUpdatePassableOnMoveFinishAction);

		//add listener for unit creation to update passable data
		CombatUnit.OnCreateUnit.AddListener(combatUnitUpdatePassableOnNewUnitAction);
		CombatUnit.OnCreateLoadedUnit.AddListener(combatUnitLoadedUpdatePassableOnNewUnitAction);

		//add listener for starting to determine a targeting path
		CombatManager.OnStartTargetPath.AddListener(combatUnitTemporarilyMakeTargetPassableAction);
		CombatManager.OnEndTargetPath.AddListener(combatUnitTemporarilyMakeTargetImpassableAction);

		//add listener for a base being destroyed
		Starbase.OnBaseDestroyed.AddListener(combatUnitClearUnitFromTileAction);

		//add listener for a ship being destroyed
		Ship.OnShipDestroyed.AddListener(combatUnitClearUnitFromTileAction);

		//add a listener for end of turn
		gameManager.OnEndTurn.AddListener(colorCheckPlanetOccupationStatusAction);

		//add a listener for loading a colony
		gameManager.OnLoadColony.AddListener(hexMapTileLoadColonyAction);

	}

	//function to update passability based on a ship starting movement
	private void UpdatePassableOnMoveStart(Ship ship){

		//check if the hexMapTile matches the starting hex
		if (this.hexLocation == ship.currentLocation) {

			//make the starting hex passable and clear the has combat unit flag
			this.isPassable = true;

			this.tileCombatUnit = null;

		}

	}


	//function to update passability based on a ship finishing movement
	private void UpdatePassableOnMoveFinish(Ship ship){

		//check if the hexMapTile matches the ending hex
		if (this.hexLocation == ship.currentLocation) {

			//make the ending hex not passable and set the has combat unit flag
			this.isPassable = false;

			this.tileCombatUnit = ship.GetComponent<CombatUnit> ();

		}

	}

	//function to update passability based on a new unit being created in the world
	private void UpdatePassableOnNewUnit(CombatUnit combatUnit){

		//check if the new unit was created in this tile
		if (this.hexLocation == combatUnit.currentLocation) {

			//if the unit was created in this hex, update passability flags
			this.isPassable = false;

			this.tileCombatUnit = combatUnit;

		}

	}

	//function that will make a hex occupied by a target unit passable
	private void TemporaryMakeTargetPassable(CombatUnit combatUnit){

		//check if the unit passed to the function is in this tile
		if (this.hexLocation == combatUnit.currentLocation) {

			//make the current hex location passable even though there's a unit there
			this.isPassable = true;

			this.tileCombatUnit = null;

		}

	}

	//function that will make a hex occupied by a target unit impassable again
	private void TemporaryMakeTargetImpassable(CombatUnit combatUnit){

		//check if the unit passed to the function is in this tile
		if (this.hexLocation == combatUnit.currentLocation) {

			//make the current hex location impassable again
			this.isPassable = false;

			this.tileCombatUnit = combatUnit;

		}

	}

	//function that will make a hex passable again if the combat unit in that location is destroyed
	private void ClearUnitFromTile(CombatUnit combatUnit){

		//check if the unit passed to the function is in this tile
		if (this.hexLocation == combatUnit.currentLocation) {

			//make the current hex location passable again
			this.isPassable = true;

			this.tileCombatUnit = null;

		}

	}

	//this function checks the planet occupation status
	private void CheckPlanetOccupationStatus(Player.Color endingTurnColor){

		//check if the current tile is a planet - if not, do nothing
		if (this.isPlanet == true) {

			//Debug.Log (this.tileType + " (" + this.hexLocation.q + "," + this.hexLocation.r + "," + this.hexLocation.s + ")" );

			//if this is a planet, check to see if there is a unit on it
			if (this.tileCombatUnit != null) {

				//if there is a unit, check to see if it is the ending turn color
				if (this.tileCombatUnit.owner.color == endingTurnColor) {

					//check to see if the planet has already been colonized
					if (this.isColonized == false) {

						//colonize the planet
						this.ColonizeNewPlanet (endingTurnColor);

					}
					//the else condition is that the planet had been colonized already
					else if (this.isColonized == true) {

						//check if the colony color is different than the unit color
						//if they are the same, we want to do nothing
						if (this.tileCombatUnit.owner.color != this.colonizingColor) {

							//colonize the planet
							this.RecolonizePlanet (endingTurnColor);

						}

					}

				}

			}

		}

	}

	//this function colonizes a new planet
	private void ColonizeNewPlanet(Player.Color newColonyColor){

		//flag the planet as colonized
		this.isColonized = true;

		//set the colonizing color
		this.colonizingColor = newColonyColor;

		//update the tileType to the low text version
		switch (this.tileType) {

		case TileType.Mercury:
			this.tileType = TileType.MercuryLow;
			break;

		case TileType.Venus:
			this.tileType = TileType.VenusLow;
			break;

		case TileType.Earth:
			this.tileType = TileType.EarthLow;
			break;

		case TileType.Mars:
			this.tileType = TileType.MarsLow;
			break;

		case TileType.Jupiter:
			this.tileType = TileType.JupiterLow;
			break;

		case TileType.Saturn:
			this.tileType = TileType.SaturnLow;
			break;

		case TileType.Uranus:
			this.tileType = TileType.UranusLow;
			break;

		case TileType.Neptune:
			this.tileType = TileType.NeptuneLow;
			break;

		case TileType.Pluto:
			this.tileType = TileType.PlutoLow;
			break;

		case TileType.Charon:
			this.tileType = TileType.CharonLow;
			break;

        case TileType.Eris:
            this.tileType = TileType.ErisLow;
            break;

        case TileType.Ceres:
            this.tileType = TileType.CeresLow;
            break;

            default:
			break;

		}

		//Debug.Log (this.tileType + " color " + newColonyColor.ToString ());

		//invoke the colonize new planet event
		OnColonizeNewPlanet.Invoke(this, newColonyColor);

	}

	//this function recolonizes a planet that had a colony
	private void RecolonizePlanet(Player.Color newColonyColor){

		//set the colonizing color
		this.colonizingColor = newColonyColor;

		//invoke the colonize new planet event
		OnColonizeNewPlanet.Invoke(this, newColonyColor);

	}

	//this method gets the planet string from the tileType
	public string GetPlanetString(){

		string planetString;

		//use a switch case to convert hexMapTile to planet string
		switch (this.tileType) {

		case HexMapTile.TileType.Mercury:
			planetString = "Mercury";
			break;

		case HexMapTile.TileType.MercuryLow:
			planetString = "Mercury";
			break;

		case HexMapTile.TileType.Venus:
			planetString = "Venus";
			break;

		case HexMapTile.TileType.VenusLow:
			planetString = "Venus";
			break;

		case HexMapTile.TileType.Earth:
			planetString = "Earth";
			break;

		case HexMapTile.TileType.EarthLow:
			planetString = "Earth";
			break;

		case HexMapTile.TileType.Mars:
			planetString = "Mars";
			break;

		case HexMapTile.TileType.MarsLow:
			planetString = "Mars";
			break;

		case HexMapTile.TileType.Jupiter:
			planetString = "Jupiter";
			break;

		case HexMapTile.TileType.JupiterLow:
			planetString = "Jupiter";
			break;

		case HexMapTile.TileType.Saturn:
			planetString = "Saturn";
			break;

		case HexMapTile.TileType.SaturnLow:
			planetString = "Saturn";
			break;

		case HexMapTile.TileType.Uranus:
			planetString = "Uranus";
			break;

		case HexMapTile.TileType.UranusLow:
			planetString = "Uranus";
			break;

		case HexMapTile.TileType.Neptune:
			planetString = "Neptune";
			break;

		case HexMapTile.TileType.NeptuneLow:
			planetString = "Neptune";
			break;

		case HexMapTile.TileType.Pluto:
			planetString = "Pluto";
			break;

		case HexMapTile.TileType.PlutoLow:
			planetString = "Pluto";
			break;

		case HexMapTile.TileType.Charon:
			planetString = "Charon";
			break;

		case HexMapTile.TileType.CharonLow:
			planetString = "Charon";
			break;

        case HexMapTile.TileType.Eris:
            planetString = "Eris";
            break;

        case HexMapTile.TileType.ErisLow:
            planetString = "Eris";
            break;

        case HexMapTile.TileType.Ceres:
            planetString = "Ceres";
            break;

        case HexMapTile.TileType.CeresLow:
            planetString = "Ceres";
            break;

            default:
			planetString = "Not A Planet!";
			break;

		}

		return planetString;

	}

	//this function loads colonies from a save file
	private void LoadColony(HexMapTile hexMapTile, Player.Color playerColor){

		//check if the passed tile matches this tile
		if (this == hexMapTile) {

			this.ColonizeNewPlanet (playerColor);

		}

	}

	//function to handle OnDestroy
	private void OnDestroy(){

		RemoveAllListeners ();

	}

	//function to remove all listeners
	private void RemoveAllListeners(){

		//remove event listeners for ship movement start
		EngineSection.OnMoveStart.RemoveListener (shipUpdatePassableOnMoveStartAction);

		//remove event listeners for ship movement finish
		EngineSection.OnMoveFinishTileStatus.RemoveListener (shipUpdatePassableOnMoveFinishAction);

		//remove listener for unit creation to update passable data
		CombatUnit.OnCreateUnit.RemoveListener (combatUnitUpdatePassableOnNewUnitAction);
		CombatUnit.OnCreateLoadedUnit.RemoveListener (combatUnitLoadedUpdatePassableOnNewUnitAction);

		//remove listener for starting to determine a targeting path
		CombatManager.OnStartTargetPath.RemoveListener (combatUnitTemporarilyMakeTargetPassableAction);
		CombatManager.OnEndTargetPath.RemoveListener (combatUnitTemporarilyMakeTargetImpassableAction);

		//remove listener for a base being destroyed
		Starbase.OnBaseDestroyed.RemoveListener (combatUnitClearUnitFromTileAction);

		//remove listener for a ship being destroyed
		Ship.OnShipDestroyed.RemoveListener (combatUnitClearUnitFromTileAction);

		if (gameManager != null) {
			
			//remove a listener for end of turn
			gameManager.OnEndTurn.RemoveListener (colorCheckPlanetOccupationStatusAction);

			//remove a listener for loading a colony
			gameManager.OnLoadColony.RemoveListener (hexMapTileLoadColonyAction);

		}

	}

}
