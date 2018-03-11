using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Colony : MonoBehaviour {

	//event to announce creation of colony
	public static ColonyEvent OnCreateColony = new ColonyEvent();

	//event to announce settling of a colony
	public static ColonyEvent OnSettleColony = new ColonyEvent();

	//simple class derived from unityEvent to pass Colony Object
	public class ColonyEvent : UnityEvent<Colony>{}; 

	//initialize the image used for the cursor
	private GraphicsManager.MyUnit defaultTileImage = GraphicsManager.MyUnit.Blank;	
	public GraphicsManager.MyUnit currentTileImage {

		get;
		private set;

	}

	public HexMapTile hexMapTile {

		get;
		private set;

	}

	//unityActions
	private UnityAction<HexMapTile,Player.Color> tileColonySettledAction;

	// Use this for initialization
	public void Init (HexMapTile newHexMapTile) {

		//set the actions
		tileColonySettledAction = (hexMapTilePassed,color) => {ColonySettled(hexMapTilePassed,color);};

		//set the current tile image to the default tile image
		currentTileImage = defaultTileImage;

		//set the hexMapTile
		hexMapTile = newHexMapTile;

		//invoke the create colony event
		OnCreateColony.Invoke(this);

		//add listener for settling a new colony
		HexMapTile.OnColonizeNewPlanet.AddListener(tileColonySettledAction);
		
	}

	//this function responds to a new colony being settled
	private void ColonySettled(HexMapTile settledHexMapTile, Player.Color settlingColor){
		

		//first, check if this colony object is located where the settled colony is
		if (this.hexMapTile == settledHexMapTile) {
			
			//create a temporary variable to hold the image of the colony
			GraphicsManager.MyUnit settlingColonyImage;

			//switch case to pick the right image based on the settling color
			switch (settlingColor) {

			case Player.Color.Green:
				settlingColonyImage = GraphicsManager.MyUnit.GreenFactory;
				break;

			case Player.Color.Red:
				settlingColonyImage = GraphicsManager.MyUnit.RedFactory;
				break;

			case Player.Color.Blue:
				settlingColonyImage = GraphicsManager.MyUnit.BlueFactory;
				break;

			case Player.Color.Purple:
				settlingColonyImage = GraphicsManager.MyUnit.PurpleFactory;
				break;

			default:
				settlingColonyImage = GraphicsManager.MyUnit.Blank;
				break;

			}

			//set the image to the settling image
			this.currentTileImage = settlingColonyImage;

			//invoke the settling colony event
			OnSettleColony.Invoke(this);

		}

	}

	//function for removing listeners ondestroy
	private void OnDestroy(){

		RemoveAllListeners ();

	}

	//function to remove all listeners
	private void RemoveAllListeners(){

		HexMapTile.OnColonizeNewPlanet.RemoveListener(tileColonySettledAction);

	}

}
