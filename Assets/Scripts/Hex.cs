using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hex {

	//constructor requires q, r, s cubic coordinates- these define the hex location on the map
	public Hex(int q, int r, int s){
		this.q = q;   //q is positive to the right for pointy-top hexes and positive to down-right for flat-top hexes
		this.r = r;	  //r is positive to the down-left for pointy-top hexes and positive to the down-left for flat-top hexes
		this.s = s;   //s is positive to the up-left for pointy-top hexes and positive to up for flat-top hexes
	}

	//the hex coordinates cannot be edited - that would make it a different hex
	/*
	public readonly int q;
	public readonly int r;
	public readonly int s;
	*/
	public int q { get; private set;}
	public int r { get; private set;}
	public int s { get; private set;}

	//this method does hex addition - this could be useful for determining distance or doing movement
	//this is static, so it doesn't require an instance of Hex to be called.  
	//it could be accessed simply as Hex.Add(a, b).
	static public Hex Add(Hex a, Hex b){
		return new Hex(a.q + b.q, a.r + b.r, a.s + b.s);
	}

	//this method does hex subtraction - this could be useful for determining distance or doing movement
	//this is static, so it doesn't require an instance of Hex to be called.  
	//it could be accessed simply as Hex.Subtract(a, b).
	static public Hex Subtract(Hex a, Hex b){
		return new Hex(a.q - b.q, a.r - b.r, a.s - b.s);
	}

	//this method does hex scaling - this could be useful for determining distance or doing movement
	//this is static, so it doesn't require an instance of Hex to be called.  
	//it could be accessed simply as Hex.Scale(a, b).
	static public Hex Scale(Hex a, int k){
		return new Hex(a.q * k, a.r * k, a.s * k);
	}

	//two hexes are equal if the q,r,s coordinates are the same
	/*
	public static bool operator == (Hex a, Hex b){
		return a.q == b.q && a.r == b.r && a.s == b.s;
	}

	//two hexes are not equal if any of q,r,s coordinates are different
	public static bool operator != (Hex a, Hex b){
		return a.q != b.q|| a.r != b.r || a.s != b.s;
	}
	*/
	public static bool operator == (Hex a, Hex b){

		if (System.Object.ReferenceEquals(a, null))
		{

			return System.Object.ReferenceEquals(b,null);
		}

		// Return true if the fields match:
		return a.Equals(b);
	}

	public static bool operator != (Hex a, Hex b){
		return !(a == b);
	}


	//override equals takes an object as an argument
	//in order to return true, the object must be a hex
	//and the object passed must have the same q,r,s (using ==) as a hex cast of the object
	/*
	public override bool Equals(object o){
		return (o is Hex) && this == (Hex)o;
	}
	*/

	public override bool Equals(object o){
		Hex other = (Hex)o;
		if (other == null) {
			return false;
		}
		return (this.q.Equals(other.q) && this.r.Equals(other.r) && this.s.Equals(other.s));
	}

	//I have to admit I don't fully understand this
	//I understand that we need to override the gethashcode method because we've changed what
	//constitutes being equal, so overriding will prevent allowing hash table dictionaries
	//to allow duplicate entries
	 
	//r << 16 shifts the value of r 16 bits to the left (bitwise shift)
	//int(0xFFFF) =65535 = 0000000000000000111111111111111 = 16 bits set to 1
	//int is a 32 bit quantity
	//shifting the value of r 16 bits to the left will make the last 16 bits equal to zero
	//q is a 32 bit int.  
	//q & (bitwise and) 0xFFFF gives just the last 16 bits of q, since the first 16 bits
	//will line up with zeroes in 0xFFFF and will be set to zero by the bitwise and
	//r<<16 will give the last 16 bits of r in the first 16 slots, followed by 16 bits of zero
	//so we will have (16 bits of zero + last 16 bits of q) | (last 16 bits of r + 16 bits of zero)
	//the bitwise or will then give (last 16 bits of r + last 16 bits of q)
	//this will be a unique new value of 32 bits for any q,r pair
	//as long as there aren't 2 pairs of q's and r's with the same last 16 bits.
	//I think, in theory, it would break if a q1 and q2 were for example 0 and 1 followed by 16 zeroes
	//for any realistic sized map, I don't see that happening
	public override int GetHashCode(){
		return q & (int)0xFFFF | r << 16;
	}

	//this is a helper function which is just a list of unit hexes that are all neighbors to 0,0,0
	static public List<Hex> directions = new List<Hex>{
		new Hex(1, 0, -1), //0 is right for pointy-top and down-right for flat-top
		new Hex(1, -1, 0), //1 is down-right for both pointy-top and flat-top
		new Hex(0, -1, 1), //2 is down-left for pointy-top and down for flat-top
		new Hex(-1, 0, 1), //3 is left for pointy-top and up-left for flat-top
		new Hex(-1, 1, 0), //4 is up-left for pointy-top and up-left for flat top
		new Hex(0, 1, -1)  //5 is up-right for pointy-top and up for flat top
	};

	//direction takes an integer as input and returns the hex in that direction from 0,0,0
	//it takes an integer, then calls Hex.directions and passes that integer
	//this then calls the hex on that list based on that integer

	static public Hex Direction(int direction){
		//0 is right for pointy-top and down-right for flat-top
		//1 is up-right for both pointy-top and flat-top
		//2 is up-left for pointy-top and up for flat-top
		//3 is left for pointy-top and up-left for flat-top
		//4 is down-left for pointy-top and down-left for flat top
		//5 is down-right for pointy-top and down for flat top
		return Hex.directions[direction];
	}

	//neighbor takes a hex as an argument and a direction and returns the hex in that direction
	//it takes the input hex, and gets the hex in the specified direction, and adds them, which gives 
	//the hex that is in that direction
	static public Hex Neighbor(Hex hex, int direction){
		return Hex.Add(hex, Hex.Direction(direction));
	}

	//this works just like the directions, but with diagonals instead
	static public List<Hex> diagonals = new List<Hex>{
		new Hex(2, -1, -1),  //0 is up-right diagonal for pointy-top and right diagonal for flat top
		new Hex(1, -2, 1),   //1 is up diagonal for pointy-top and up-right diagonal for flat top
		new Hex(-1, -1, 2),  //2 is up-left diagonal for pointy-top and up-left diagonal for flat top
		new Hex(-2, 1, 1),   //3 is down-left diagonal for pointy-top and left diagonal for flat top
		new Hex(-1, 2, -1),  //4 is down diagonal for pointy-top and down-left diagonal for flat top
		new Hex(1, 1, -2)    //5 is down-right diagonal for pointy-top and down-right diagonal for flat top
	};

	//this works just like neighbor, but with diagonal neighbor instead
	static public Hex DiagonalNeighbor(Hex hex, int direction){
		//0 is up-right diagonal for pointy-top and right diagonal for flat top
		//1 is up diagonal for pointy-top and up-right diagonal for flat top
		//2 is up-left diagonal for pointy-top and up-left diagonal for flat top
		//3 is down-left diagonal for pointy-top and left diagonal for flat top
		//4 is down diagonal for pointy-top and down-left diagonal for flat top
		//5 is down-right diagonal for pointy-top and down-right diagonal for flat top
		return Hex.Add(hex, Hex.diagonals[direction]);
	}

	//length gives the "distance" from 0,0,0 to the hex passed as an argument
	static public int Length(Hex hex){
		return (int)((Mathf.Abs(hex.q) + Mathf.Abs(hex.r) + Mathf.Abs(hex.s)) / 2);
	}

	//this function returns the distance between 2 hexes
	static public int Distance(Hex a, Hex b){
		return Hex.Length(Hex.Subtract(a, b));
	}

	//this function takes a cube coordinate hex location and converts it to a string output
	static public string CubeCoordToString(Hex hex){

		string outputString = "(" + hex.q + ", " + hex.r + ", " + hex.s + ")";

		return outputString;

	}

}
