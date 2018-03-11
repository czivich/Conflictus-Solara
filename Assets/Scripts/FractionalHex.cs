using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct FractionalHex {

	//constructor requires q, r, s cubic coordinates- these define the hex location on the map
	public FractionalHex(float q, float r, float s){
		this.q = q;  //q is positive to the up-right and negative to the down-left for pointy-top hexes and flat-top hexes
		this.r = r;  //r is positive to the down-right and negative to the up-left for pointy-top hexes and flat-top hexes
		this.s = s;  //s is positive to the right for pointy-top hexes and positive to up for flat-top hexes
	}

	//the hex coordinates cannot be edited - that would make it a different hex
	public readonly float q;
	public readonly float r;
	public readonly float s;

	//this function takes a fractional hex and rounds it to the nearest integer hex
	static public Hex HexRound(FractionalHex h)
	{

		int q = (int)(Mathf.Round(h.q));
		int r = (int)(Mathf.Round(h.r));
		int s = (int)(Mathf.Round(h.s));


		float q_diff = Mathf.Abs(q - h.q);
		float r_diff = Mathf.Abs(r - h.r);
		float s_diff = Mathf.Abs(s - h.s);

		if (q_diff > r_diff && q_diff > s_diff)
		{
			q = -r - s;
		}
		else
			if (r_diff > s_diff)
			{
				r = -q - s;
			}
			else
			{
				s = -q - r;
			}

		return new Hex (q, r, s);

	}

	//this function takes a fractional hex and returns a vector 3 that is the q, r, s for a new hex
	static public Vector3 HexRoundV3(FractionalHex h)
	{

		int q = (int)(Mathf.Round(h.q));
		int r = (int)(Mathf.Round(h.r));
		int s = (int)(Mathf.Round(h.s));


		float q_diff = Mathf.Abs(q - h.q);
		float r_diff = Mathf.Abs(r - h.r);
		float s_diff = Mathf.Abs(s - h.s);

		if (q_diff > r_diff && q_diff > s_diff)
		{
			q = -r - s;
		}
		else
			if (r_diff > s_diff)
			{
				r = -q - s;
			}
			else
			{
				s = -q - r;
			}

		return new Vector3 (q, r, s);

	}

	//this function takes 2 hexes (fractional) and linear interpolates between them - called by the line draw function
	static public FractionalHex HexLerp(FractionalHex a, FractionalHex b, float t){
		return new FractionalHex(a.q * (1 - t) + b.q * t, a.r * (1 - t) + b.r * t, a.s * (1 - t) + b.s * t);
	}

	//this function I think returns a list of hexagons that are the nearest straight-line path from Hex a to Hex b
	static public List<Hex> HexLinedraw(Hex a, Hex b){
		int N = Hex.Distance(a, b);
		FractionalHex a_nudge = new FractionalHex(a.q + 0.000001f, a.r + 0.000001f, a.s - 0.000002f);
		FractionalHex b_nudge = new FractionalHex(b.q + 0.000001f, b.r + 0.000001f, b.s - 0.000002f);
		List<Hex> results = new List<Hex>{};
		float step = 1.0f / Mathf.Max(N, 1);
		for (int i = 0; i <= N; i++)
		{
			results.Add(FractionalHex.HexRound(FractionalHex.HexLerp(a_nudge, b_nudge, step * i)));
		}
		return results;
	}

}
