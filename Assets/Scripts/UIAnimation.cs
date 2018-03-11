using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAnimation : MonoBehaviour {

	//array to hold all the sprite images for the explosion
	public Sprite[] spriteSheet;

	//variable to hold the loop time for the animation
	private float loopTime;

	//variable to hold the frame time
	private float frameTime;

	//variable to hold the image to be displayed
	private Image displayedImage;

	//variable for number of frames to advance during update
	private int framesToAdvance;

	//variable to hold the current frame index
	private int currentFrame;

	//variable to control whether animation should loop or destroy after 1 loop
	private bool isSingleLoop;

	//this function is used to instantiate an instance
	public static void CreateUIAmination(UIAnimation prefab, float animationTime, Vector3 animationLocalScale, Vector3 animationLocalPosition, Transform animationParent, bool isSingleLoop){

		//instantiate the new object
		//UIAnimation newUIAnimation = Instantiate (prefab, animationParent,false);
		UIAnimation newUIAnimation = SimplePool.Spawn(prefab.gameObject,Vector3.zero,Quaternion.identity).GetComponent<UIAnimation>();

		//set the parent
		newUIAnimation.transform.SetParent(animationParent,false);

		//set the local position
		newUIAnimation.transform.localPosition = animationLocalPosition;

		//set the new object scale
		newUIAnimation.transform.localScale = animationLocalScale;

		//set the isSingleLoop flag
		newUIAnimation.isSingleLoop = isSingleLoop;

		//call the Init function 
		newUIAnimation.Init(animationTime);

	}

	// Use this for initialization
	private void Init (float animationTime) {

		//set the loop time
		loopTime = animationTime;

		//calculate the frame time
		frameTime = loopTime / spriteSheet.Length;

		//get the displayed image
		displayedImage = this.GetComponentInChildren<Image>();

		//set the current frame to zero
		currentFrame = 0;

		//set the displayed image to the new frame
		displayedImage.sprite = spriteSheet[currentFrame];
		
	}
	
	// Update is called once per frame
	private void Update () {

		//calculate the number of frames to advance
		framesToAdvance = Mathf.CeilToInt(Time.deltaTime / frameTime);

		//calculate the new frame to show
		if (currentFrame + framesToAdvance >= spriteSheet.Length) {

			currentFrame = 0;

			//check if we should be looping or not
			if (this.isSingleLoop == true) {

				//if we should only loop once, destroy this gameobject
				//GameObject.Destroy(this.gameObject);
				SimplePool.Despawn(this.gameObject);

			}

		} else {

			currentFrame += framesToAdvance;

		}

		//set the displayed image to the new frame
		displayedImage.sprite = spriteSheet[currentFrame];
		
	}

}
