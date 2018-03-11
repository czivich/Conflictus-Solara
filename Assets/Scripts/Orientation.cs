using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Orientation {

	//this is a helper class which stores the matrix and inverse matrix for converting from hex coordinates to screen coordinates
	//it also includes the starting angle.  
	//I have made it so that the pointy-top starting angle is 90 deg, so that the fist point is the top point
	//I have made it so that the flat top starting angle is 60 deg, so that the first point is up-right
	public Orientation(float f0, float f1, float f2, float f3, float b0, float b1, float b2, float b3, float start_angle){
		this.f0 = f0;
		this.f1 = f1;
		this.f2 = f2;
		this.f3 = f3;
		this.b0 = b0;
		this.b1 = b1;
		this.b2 = b2;
		this.b3 = b3;
		this.start_angle = start_angle;
	}

	public readonly float f0;
	public readonly float f1;
	public readonly float f2;
	public readonly float f3;
	public readonly float b0;
	public readonly float b1;
	public readonly float b2;
	public readonly float b3;
	public readonly float start_angle;
		

}
