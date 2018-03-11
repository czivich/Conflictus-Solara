using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Layout {

	//layout contructor requires an orientation, vector2 size, and vector2 origin
	//orientation should always just pass .flat or .pointy
	public Layout(Orientation orientation, Vector2 size, Vector2 origin)
	{
		this.orientation = orientation;
		this.size = size;					//size is the x,y size of the final map
		this.origin = origin;
	}
	public readonly Orientation orientation;
	public readonly Vector2 size;
	public readonly Vector2 origin;
	static public Orientation pointy = new Orientation(Mathf.Sqrt(3.0f), Mathf.Sqrt(3.0f) / 2.0f, 0.0f, 3.0f / 2.0f, Mathf.Sqrt(3.0f) / 3.0f, -1.0f / 3.0f, 0.0f, 2.0f / 3.0f, 1.5f);
	static public Orientation flat = new Orientation(3.0f / 2.0f, 0.0f, Mathf.Sqrt(3.0f) / 2.0f, Mathf.Sqrt(3.0f), 2.0f / 3.0f, 0.0f, -1.0f / 3.0f, Mathf.Sqrt(3.0f) / 3.0f, 1.0f);

	//HexToPixel takes a layout and a hex and converts the center of the hex to screen x,y coordinates
	static public Vector2 HexToPixelV2(Layout layout, Hex h)
	{
		Orientation M = layout.orientation;
		Vector2 size = layout.size;
		Vector2 origin = layout.origin;
		float x = (M.f0 * h.q + M.f1 * h.r) * size.x;
		float y = (M.f2 * h.q + M.f3 * h.r) * size.y;
		return new Vector2(x + origin.x, y + origin.y);
	}

	//this pseudo-polymorphed method returns a vector3 instead of a vector2, with y = 0
	static public Vector3 HexToPixelV3(Layout layout, Hex h)
	{
		Orientation M = layout.orientation;
		Vector2 size = layout.size;
		Vector2 origin = layout.origin;
		float x = (M.f0 * h.q + M.f1 * h.r) * size.x;
		float y = (M.f2 * h.q + M.f3 * h.r) * size.y;
		return new Vector3(x + origin.x, 0.0f, y + origin.y);
	}

	//PixelToHex takes the x,y coordinate on screen, and returns the fractional hex where that point is
	//That fractional hex could then be rounded off to give the nearest actual hex
	//it works by taking the layout and the point
	//it then pulls out the orientation, size, and origin from the layout
	//it creates a vector2 pt which shifts for the origin/size and uses the matrix to return the q,r coordinates
	static public FractionalHex PixelToHexV2(Layout layout, Vector2 p)
	{
		Orientation M = layout.orientation;
		Vector2 size = layout.size;
		Vector2 origin = layout.origin;
		Vector2 pt = new Vector2((p.x - origin.x) / size.x, (p.y - origin.y) / size.y);
		float q = M.b0 * pt.x + M.b1 * pt.y;
		float r = M.b2 * pt.x + M.b3 * pt.y;
		return new FractionalHex(q, r, -q - r);
	}

	//this pseudo-polymorphed FractionalHex method takes a vector3 instead of a vector2, with the y value equal to 0.
	//this will prevent having to create go-between conversions from vector2 to vector3.
	static public FractionalHex PixelToHexV3(Layout layout, Vector3 p){
		Orientation M = layout.orientation;
		Vector2 size = layout.size;
		Vector2 origin = layout.origin;
		Vector2 pt = new Vector2 ((p.x - origin.x) / size.x, (p.z - origin.y) / size.y);
		float q = M.b0 * pt.x + M.b1 * pt.y;
		float r = M.b2 * pt.x + M.b3 * pt.y;
		return new FractionalHex (q, r, -q - r);
	}


	//this function returns a vector2 with the offset from a center to a corner point:
	//0 is up for pointy-top
	//1 is up-right
	//2 is down-right
	//3 is down
	//4 is down-left
	//5 is up-left
	static public Vector2 HexCornerOffsetV2(Layout layout, int corner)
	{
		Orientation M = layout.orientation;
		Vector2 size = layout.size;
		float angle = 2.0f * Mathf.PI * (M.start_angle - corner) / 6;
		return new Vector2(size.x * Mathf.Cos(angle), size.y * Mathf.Sin(angle));
	}

	//this pseudo-polymorphed method returns a vector3 instead of a vector2, with the y = 0.
	static public Vector3 HexCornerOffsetV3(Layout layout, int corner){
		Orientation M = layout.orientation;
		Vector2 size = layout.size;
		float angle = 2.0f * Mathf.PI * (M.start_angle - corner) / 6;
		return new Vector3(size.x * Mathf.Cos(angle), 0.0f, size.y * Mathf.Sin(angle));
	}

	//this function returns a list of vector2 coordinates for the 6 corners in x,y space for a hex h, given layout
	//I'd like to make a copy of this function which also returns the center point in addition to the 6 corners
	static public List<Vector2> PolygonCornersV2(Layout layout, Hex h)
	{
		List<Vector2> corners = new List<Vector2>{};
		Vector2 center = Layout.HexToPixelV2(layout, h);
		for (int i = 0; i < 6; i++)
		{
			Vector2 offset = Layout.HexCornerOffsetV2(layout, i);
			corners.Add(new Vector2(center.x + offset.x, center.y + offset.y));
		}
		return corners;
	}
	//this pseudo-polymorphed method returns a list of vector3 instead of vector2, with y = 0;
	static public List<Vector3> PolygonCornersV3(Layout layout, Hex h)
	{
		List<Vector3> corners = new List<Vector3>{};
		Vector3 center = Layout.HexToPixelV3(layout, h);
		for (int i = 0; i < 6; i++)
		{
			Vector3 offset = Layout.HexCornerOffsetV3(layout, i);
			corners.Add(new Vector3(center.x + offset.x, 0.0f, center.z + offset.z));
		}
		return corners;
	}

	//this function returns a list of vector2 coordinates for the center point + 6 corners in x,y space for a hex h, given layout
	static public List<Vector2> PolygonCenterAndCornersV2(Layout layout, Hex h)
	{
		List<Vector2> vertices = new List<Vector2>{};
		Vector2 center = Layout.HexToPixelV2(layout, h);
		vertices.Add (center);
		for (int i = 1; i < 7; i++)
		{
			Vector2 offset = Layout.HexCornerOffsetV2(layout, i-1);
			vertices.Add(new Vector2(center.x + offset.x, center.y + offset.y));
		}
		return vertices;
	}
	//this pseudo-polymorphed method returns a list of vector3 instead of vector2, with y = 0;
	static public List<Vector3> PolygonCenterAndCornersV3(Layout layout, Hex h)
	{
		List<Vector3> vertices = new List<Vector3>{};
		Vector3 center = Layout.HexToPixelV3(layout, h);
		vertices.Add (center);
		for (int i = 1; i < 7; i++)
		{
			Vector3 offset = Layout.HexCornerOffsetV3(layout, i-1);
			vertices.Add(new Vector3(center.x + offset.x, 0.0f, center.z + offset.z));
		}
		return vertices;
	}
}
