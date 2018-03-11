using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct OffsetCoord {

	public OffsetCoord(int col, int row){
		this.col = col;
		this.row = row;
	}

	public readonly int col;
	public readonly int row;
	static public int EVEN = 1;
	static public int ODD = -1;

	//this method takes a hex and offset (my game is odd) and returns the offset coordinates of the hex
	//this q-type is for flat-top hexes
	static public OffsetCoord QoffsetFromCube(int offset, Hex h)
	{
		int col = h.q;
		int row = h.r + (int)((h.q + offset * (h.q & 1)) / 2);
		return new OffsetCoord(col, row);
	}

	//this method takes an offset and a pair of offset coordinates and returns the cubic coordinates hex
	//this q-type is for flat-top hexes
	static public Hex QoffsetToCube(int offset, OffsetCoord h)
	{
		int q = h.col;
		int r = h.row - (int)((h.col + offset * (h.col & 1)) / 2);
		int s = -q - r;
		return new Hex(q, r, s);
	}

	//this method takes a hex and offset (my game is odd) and returns the offset coordinates of the hex
	//this r-type is for pointy-top hexes
	static public OffsetCoord RoffsetFromCube(int offset, Hex h)
	{
		int col = h.q + (int)((h.r + offset * (h.r & 1)) / 2);
		int row = h.r;
		return new OffsetCoord(col, row);
	}

	//this method takes an offset and a pair of offset coordinates and returns the cubic coordinates hex
	//this r-type is for pointy-top hexes
	static public Hex RoffsetToCube(int offset, OffsetCoord h)
	{
		int q = h.col - (int)((h.row + offset * (h.row & 1)) / 2);
		int r = h.row;
		int s = -q - r;
		return new Hex(q, r, s);
	}

	//this function takes an offset coordinate hex location and converts it to a string output
	static public string OffsetCoordToString(OffsetCoord offsetCoord){

		string outputString = "(" + offsetCoord.row + ", " + offsetCoord.col + ")";

		return outputString;

	}



}
