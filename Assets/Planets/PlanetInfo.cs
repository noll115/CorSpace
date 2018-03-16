using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlanetInfo : ScriptableObject {

	public SpawnablePlanet spriteInfo;
	public Vector3 position;
	public Vector3 size;
	public float startRot;
	public float rotSpeed;
	public Planets planetType;
	public float maxLava = 0f;
	public float maxWater = 0f;
	public float maxOres = 0f;
	public float maxFuel = 0f;
	public float maxLife = 1f;
	public float lavaAmount = 0f;
	public float waterAmount = 0f;
	public float oresAmount = 0f;
	public float fuelAmount = 0f;
	public float lifeAmount = 0f;
	public Color planetCol;
	[Range(0,10)]
	public int numOfAsteroids;


	public void UpdateData(BarrenResources resource) {
		lavaAmount = resource.lava;
		waterAmount = resource.water;
		//oresAmount = resource.ores;
		lifeAmount = resource.life;
	}

	public void UpdateData(PlanetResources resource) {
		lavaAmount = resource.lava;
		waterAmount = resource.water;
		oresAmount = resource.ores;
		fuelAmount = resource.fuel;
		lifeAmount = resource.life;
	}


}

