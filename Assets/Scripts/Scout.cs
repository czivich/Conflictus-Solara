using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scout : Ship {

	//this child class is for Scout ships
	//starting values for items on Scout
	//engine section values
	public static readonly int startingWarpBooster = 3;
	public static readonly int startingTranswarpBooster = 0;
	public static readonly bool startingWarpDrive = false;
	public static readonly bool startingTranswarpDrive = false;
	public static readonly int engineSectionShieldsMax = 80;

	//phaser section values
	public static readonly int startingPhaserRadarShot = 0;
	public static readonly bool startingXRayKernelUpgrade = false;
	public static readonly bool startingPhaserRadarArray = false;
	public static readonly bool startingTractorBeam = false;
	public static readonly int phaserSectionShieldsMax = 80;

	//storage section values
	public static readonly int startingDilithiumCrystals = 2;
	public static readonly int startingTrilithiumCrystals = 0;
	public static readonly bool startingRadarJammingSystem = false;
	public static readonly bool startingLaserScatteringSystem = false;
	public static readonly int startingFlares = 1;
	public static readonly int storageSectionShieldsMax = 80;

	//Use this for initialization
	protected override void OnInitLevel2 () {

		//intialize the sections
		this.GetComponent<PhaserSection>().Init();
		this.GetComponent<StorageSection>().Init();
		this.GetComponent<EngineSection>().Init();

	}

}
