using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Dice {

	//this class will be used resolve die rolls for the game

	//these probability tables are taken from AnyDice.com
	public static readonly Dictionary <int,float> SumAtLeastProbability3D20 = new Dictionary<int, float>(){

		{1,	100.00f},
		{2,	100.00f},
		{3,	100.00f},
		{4,	99.99f},
		{5,	99.95f},	
		{6,	99.88f},
		{7,	99.75f},	
		{8,	99.56f},	
		{9,	99.30f},	
		{10, 98.95f},	
		{11, 98.50f},	
		{12, 97.94f},	
		{13, 97.25f},	
		{14, 96.42f},	
		{15, 95.45f},	
		{16, 94.31f},	
		{17, 93.00f},	
		{18, 91.50f},	
		{19, 89.80f},	
		{20, 87.89f},	
		{21, 85.75f},	
		{22, 83.38f},	
		{23, 80.75f},	
		{24, 77.90f},	
		{25, 74.85f},	
		{26, 71.63f},	
		{27, 68.25f},	
		{28, 64.75f},	
		{29, 61.15f},	
		{30, 57.48f},	
		{31, 53.75f},	
		{32, 50.00f},	
		{33, 46.25f},	
		{34, 42.53f},	
		{35, 38.85f},	
		{36, 35.25f},	
		{37, 31.75f},	
		{38, 28.38f},	
		{39, 25.15f},	
		{40, 22.10f},	
		{41, 19.25f},	
		{42, 16.63f},	
		{43, 14.25f},	
		{44, 12.11f},	
		{45, 10.20f},	
		{46, 8.50f},	
		{47, 7.00f},	
		{48, 5.69f},	
		{49, 4.55f},	
		{50, 3.58f},	
		{51, 2.75f},	
		{52, 2.06f},	
		{53, 1.50f},	
		{54, 1.05f},	
		{55, 0.70f},	
		{56, 0.44f},	
		{57, 0.25f},	
		{58, 0.13f},	
		{59, 0.05f},	
		{60, 0.01f},

	};

	public static readonly Dictionary <int,float> SumAtLeastProbability4D20 = new Dictionary<int, float>(){

		{1,	100.00f},
		{2,	100.00f},
		{3,	100.00f},
		{4,	100.00f},	
		{5,	100.00f},
		{6,	100.00f},	
		{7,	99.99f},	
		{8,	99.98f},	
		{9,	99.96f},	
		{10,99.92f},	
		{11,99.87f},	
		{12,99.79f},	
		{13,99.69f},	
		{14,99.55f},	
		{15,99.37f},	
		{16,99.15f},	
		{17,98.86f},	
		{18,98.51f},	
		{19,98.09f},	
		{20,97.58f},	
		{21,96.97f},	
		{22,96.26f},	
		{23,95.43f},	
		{24,94.47f},	
		{25,93.36f},	
		{26,92.11f},	
		{27,90.69f},	
		{28,89.12f},	
		{29,87.38f},	
		{30,85.47f},	
		{31,83.40f},	
		{32,81.16f},	
		{33,78.76f},	
		{34,76.21f},	
		{35,73.52f},	
		{36,70.69f},	
		{37,67.73f},	
		{38,64.67f},	
		{39,61.52f},	
		{40,58.28f},	
		{41,54.99f},	
		{42,51.67f},	
		{43,48.33f},	
		{44,45.01f},	
		{45,41.72f},	
		{46,38.48f},	
		{47,35.33f},	
		{48,32.27f},	
		{49,29.31f},	
		{50,26.48f},	
		{51,23.79f},	
		{52,21.24f},	
		{53,18.84f},	
		{54,16.60f},	
		{55,14.53f},	
		{56,12.62f},	
		{57,10.88f},	
		{58,9.31f},	
		{59,7.89f},	
		{60,6.64f},	
		{61,5.53f},	
		{62,4.57f},	
		{63,3.74f},	
		{64,3.03f},	
		{65,2.42f},	
		{66,1.91f},	
		{67,1.49f},	
		{68,1.14f},	
		{69,0.85f},	
		{70,0.63f},	
		{71,0.45f},	
		{72,0.31f},	
		{73,0.21f},	
		{74,0.13f},	
		{75,0.08f},	
		{76,0.04f},	
		{77,0.02f},	
		{78,0.01f},	
		{79,0.00f},	
		{80,0.00f},
	};

	public static readonly Dictionary <int,float> SumAtLeastProbability5D20 = new Dictionary<int, float>(){

		{1,	100.00f},
		{2,	100.00f},
		{3,	100.00f},
		{4,	100.00f},
		{5,	100.00f},	
		{6,	100.00f},	
		{7,	100.00f},	
		{8,	100.00f},	
		{9,	100.00f},	
		{10,100.00f},	
		{11,99.99f},	
		{12,99.99f},	
		{13,99.98f},	
		{14,99.96f},	
		{15,99.94f},	
		{16,99.91f},	
		{17,99.86f},	
		{18,99.81f},	
		{19,99.73f},	
		{20,99.64f},	
		{21,99.52f},	
		{22,99.36f},	
		{23,99.18f},	
		{24,98.95f},	
		{25,98.67f},	
		{26,98.34f},	
		{27,97.95f},	
		{28,97.48f},	
		{29,96.94f},	
		{30,96.31f},	
		{31,95.59f},	
		{32,94.76f},	
		{33,93.83f},	
		{34,92.78f},	
		{35,91.62f},	
		{36,90.32f},	
		{37,88.90f},	
		{38,87.35f},	
		{39,85.65f},	
		{40,83.82f},	
		{41,81.86f},	
		{42,79.76f},	
		{43,77.53f},	
		{44,75.18f},	
		{45,72.70f},	
		{46,70.12f},	
		{47,67.44f},	
		{48,64.67f},	
		{49,61.83f},	
		{50,58.93f},	
		{51,55.98f},	
		{52,53.00f},	
		{53,50.00f},	
		{54,47.00f},	
		{55,44.02f},	
		{56,41.07f},	
		{57,38.17f},	
		{58,35.33f},	
		{59,32.56f},	
		{60,29.88f},	
		{61,27.30f},	
		{62,24.82f},	
		{63,22.47f},	
		{64,20.24f},	
		{65,18.14f},	
		{66,16.18f},	
		{67,14.35f},	
		{68,12.65f},	
		{69,11.10f},	
		{70,9.68f},	
		{71,8.38f},	
		{72,7.22f},	
		{73,6.17f},	
		{74,5.24f},	
		{75,4.41f},	
		{76,3.69f},	
		{77,3.06f},	
		{78,2.52f},	
		{79,2.05f},	
		{80,1.66f},	
		{81,1.33f},	
		{82,1.05f},	
		{83,0.82f},	
		{84,0.64f},	
		{85,0.48f},	
		{86,0.36f},	
		{87,0.27f},	
		{88,0.19f},	
		{89,0.14f},	
		{90,0.09f},	
		{91,0.06f},	
		{92,0.04f},	
		{93,0.02f},
		{94,0.01f},	
		{95,0.01f},	
		{96,0.00f},	
		{97,0.00f},	
		{98,0.00f},	
		{99,0.00f},	
		{100,0.00f},
	};

	//this function will return the total sum of a number of dice rolled
	public static int DiceRollSum(int numberOfDice, int sidesOfDice){

		//initialize the sum to 0
		int diceSum = 0;

		//loop through once for each die to be rolled, adding a random number with range equal to the sides of the dice specified
		for(int i = 0; i < numberOfDice; i++){
			
			diceSum += Random.Range (1, sidesOfDice + 1);

		}

        //Debug.Log("DiceRollSum numberOfDice = " + numberOfDice + ", sidesOfDice = " + sidesOfDice + ", diceSum = " + diceSum);

		return diceSum;
	}

	//this function will roll a number of dice, and will return true if one of those rolls contains the target number, and false if it does not
	public static bool DiceRollContains(int numberOfDice, int sidesOfDice, int targetNumber){

		int dieRoll;
		bool hitTarget = false;

		//loop through once for each die to be rolled, generating a random number with range equal to the sides of the dice specified
		for(int i = 0; i < numberOfDice; i++){

			dieRoll = Random.Range (1, sidesOfDice + 1);

			//Debug.Log ("Target number is " + targetNumber + ", Die Roll is " + dieRoll);

			//check if the roll hit the target number
			if (dieRoll == targetNumber) {

				hitTarget = true;

			}

		}

		return hitTarget;

	}


}
