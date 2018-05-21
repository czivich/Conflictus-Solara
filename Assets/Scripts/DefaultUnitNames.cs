using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultUnitNames : MonoBehaviour {

	//this class will store the default unit names for ships and bases
	public string[] blueScoutNames {

		get;
		private set;

	}

	public string[] blueBirdOfPreyNames {

		get;
		private set;

	}

	public string[] blueDestroyerNames {

		get;
		private set;

	}

	public string[] blueStarshipNames {

		get;
		private set;

	}

	public string blueStarbaseName {

		get;
		private set;

	}

	public string[] redScoutNames {

		get;
		private set;

	}

	public string[] redBirdOfPreyNames {

		get;
		private set;

	}

	public string[] redDestroyerNames {

		get;
		private set;

	}

	public string[] redStarshipNames {

		get;
		private set;

	}

	public string redStarbaseName {

		get;
		private set;

	}

	public string[] purpleScoutNames {

		get;
		private set;

	}

	public string[] purpleBirdOfPreyNames {

		get;
		private set;

	}

	public string[] purpleDestroyerNames {

		get;
		private set;

	}

	public string[] purpleStarshipNames {

		get;
		private set;

	}

	public string purpleStarbaseName {

		get;
		private set;

	}

	public string[] greenScoutNames {

		get;
		private set;

	}

	public string[] greenBirdOfPreyNames {

		get;
		private set;

	}

	public string[] greenDestroyerNames {

		get;
		private set;

	}

	public string[] greenStarshipNames {

		get;
		private set;

	}

	public string greenStarbaseName {

		get;
		private set;

	}

	// Use this for initialization
	public void Init () {
		
		blueScoutNames = new string[GameManager.maxShipsPerClass];
		blueScoutNames [0] = "Dauntless";
		blueScoutNames [1] = "Courage";
		blueScoutNames [2] = "Reliant";
		blueScoutNames [3] = "Bravery";
		blueScoutNames [4] = "Fortitude";

		blueBirdOfPreyNames = new string[GameManager.maxShipsPerClass];
		blueBirdOfPreyNames [0] = "Oblivion";
		blueBirdOfPreyNames [1] = "Perdition";
		blueBirdOfPreyNames [2] = "Abyss";
		blueBirdOfPreyNames [3] = "Templar";
		blueBirdOfPreyNames [4] = "Nether";

		blueDestroyerNames = new string[GameManager.maxShipsPerClass];
		blueDestroyerNames [0] = "Determination";
		blueDestroyerNames [1] = "Victory";
		blueDestroyerNames [2] = "Carnage";
		blueDestroyerNames [3] = "Typhoon";
		blueDestroyerNames [4] = "Avenger";

		blueStarshipNames = new string[GameManager.maxShipsPerClass];
		blueStarshipNames [0] = "Exemplar";
		blueStarshipNames [1] = "Paragon";
		blueStarshipNames [2] = "Dreadnought";
		blueStarshipNames [3] = "Dominion";
		blueStarshipNames [4] = "Excalibur";

		blueStarbaseName = "Citadel";

		redScoutNames = new string[GameManager.maxShipsPerClass];
		redScoutNames [0] = "Destiny";
		redScoutNames [1] = "Ingenuity";
		redScoutNames [2] = "Civilization";
		redScoutNames [3] = "Inquisitor";
		redScoutNames [4] = "Eureka";

		redBirdOfPreyNames = new string[GameManager.maxShipsPerClass];
		redBirdOfPreyNames [0] = "Bandit";
		redBirdOfPreyNames [1] = "Corsair";
		redBirdOfPreyNames [2] = "Rogue";
		redBirdOfPreyNames [3] = "Barbosa";
		redBirdOfPreyNames [4] = "Drake";

		redDestroyerNames = new string[GameManager.maxShipsPerClass];
		redDestroyerNames [0] = "Independence";
		redDestroyerNames [1] = "Freedom";
		redDestroyerNames [2] = "Adamant";
		redDestroyerNames [3] = "Sovereign";
		redDestroyerNames [4] = "Titan";

		redStarshipNames = new string[GameManager.maxShipsPerClass];
		redStarshipNames [0] = "Gallant";
		redStarshipNames [1] = "Resolute";
		redStarshipNames [2] = "Endeavour";
		redStarshipNames [3] = "Intrepid";
		redStarshipNames [4] = "Valiant";

		redStarbaseName = "Bastille";

		purpleScoutNames = new string[GameManager.maxShipsPerClass];
		purpleScoutNames [0] = "Odyssey";
		purpleScoutNames [1] = "Argonaut";
		purpleScoutNames [2] = "Minerva";
		purpleScoutNames [3] = "Prometheus";
		purpleScoutNames [4] = "Hermes";

		purpleBirdOfPreyNames = new string[GameManager.maxShipsPerClass];
		purpleBirdOfPreyNames [0] = "Orion";
		purpleBirdOfPreyNames [1] = "Artemis";
		purpleBirdOfPreyNames [2] = "Ares";
		purpleBirdOfPreyNames [3] = "Phobos";
		purpleBirdOfPreyNames [4] = "Deimos";

		purpleDestroyerNames = new string[GameManager.maxShipsPerClass];
		purpleDestroyerNames [0] = "Minotaur";
		purpleDestroyerNames [1] = "Chimera";
		purpleDestroyerNames [2] = "Cerberus";
		purpleDestroyerNames [3] = "Manticore";
		purpleDestroyerNames [4] = "Hydra";

		purpleStarshipNames = new string[GameManager.maxShipsPerClass];
		purpleStarshipNames [0] = "Maximus";
		purpleStarshipNames [1] = "Colossus";
		purpleStarshipNames [2] = "Achilles";
		purpleStarshipNames [3] = "Agamemnon";
		purpleStarshipNames [4] = "Perseus";

		purpleStarbaseName = "Acropolis";

		greenScoutNames = new string[GameManager.maxShipsPerClass];
		greenScoutNames [0] = "Voyager";
		greenScoutNames [1] = "Discovery";
		greenScoutNames [2] = "Horizon";
		greenScoutNames [3] = "Magellan";
		greenScoutNames [4] = "New Dawn";

		greenBirdOfPreyNames = new string[GameManager.maxShipsPerClass];
		greenBirdOfPreyNames [0] = "Wraith";
		greenBirdOfPreyNames [1] = "Phantom";
		greenBirdOfPreyNames [2] = "Spectre";
		greenBirdOfPreyNames [3] = "Revenant";
		greenBirdOfPreyNames [4] = "Shade";

		greenDestroyerNames = new string[GameManager.maxShipsPerClass];
		greenDestroyerNames [0] = "Vengeance";
		greenDestroyerNames [1] = "Retribution";
		greenDestroyerNames [2] = "Judgment";
		greenDestroyerNames [3] = "Nemesis";
		greenDestroyerNames [4] = "Inferno";

		greenStarshipNames = new string[GameManager.maxShipsPerClass];
		greenStarshipNames [0] = "Apocalypse";
		greenStarshipNames [1] = "Armageddon";
		greenStarshipNames [2] = "Ragnarok";
		greenStarshipNames [3] = "Cataclysm";
		greenStarshipNames [4] = "Catastrophe";

		greenStarbaseName = "Rampart";
				
	}

}
