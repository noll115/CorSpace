using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetResources : MonoBehaviour {

	[SerializeField] int pointLost = 2;
	[SerializeField] int pointsGained = 4;
	public float fuel;
	public float lava;
	public float water;
	public float ores;
	public float maxFuel;
	public float maxLava;
	public float maxWater;
	public float maxOres;
	public bool HasFuel {
		get { return fuel > 0; }
	}
	public bool HasLava {
		get { return lava > 0; }
	}
	public bool HasWater {
		get { return water > 0; }
	}
	public bool HasOres {
		get { return ores > 0; }
	}
	public float collectionSpeed;
	public AnimationCurve curve;
	float curveTime = 0f;
	public float life = 1f;
	Material mat;
	private const string Name = "_Lerp";
	bool lostPoints = false;

	private void Awake() {
		mat = GetComponentInChildren<Renderer>().material;
	}

	public void Setup(PlanetInfo info) {
		this.fuel = info.fuelAmount;
		this.lava = info.lavaAmount;
		this.water = info.waterAmount;
		this.ores = info.oresAmount;
		maxFuel = fuel;
		maxLava = lava;
		maxWater = water;
		maxOres = ores;
		life = (fuel + lava + water + ores) / (maxFuel + maxLava + maxWater + maxOres);
		mat.SetFloat(Name, life);
		info.lifeAmount = life;
	}




	public void SubtractResources(PlayerResources presources) {
		if(!presources.MaxedFuel || !presources.MaxedOres || !presources.MaxedWater || !presources.MaxedLava) {
			SubtractFuel(presources);
			SubtractLava(presources);
			SubtractWater(presources);
			SubtractOres(presources);
			life = (fuel + lava + water + ores) / (maxFuel + maxLava + maxWater + maxOres);
			curveTime += collectionSpeed * Time.deltaTime;
			curveTime = Mathf.Clamp(curveTime, 0, 1f);
			mat.SetFloat(Name, life);
			if(life < 0.5f) {
				GameManager.Gm.AddScore(-pointLost);
			}
			else {
				GameManager.Gm.AddScore(pointsGained);
			}
			if(life <= 0 && !lostPoints) {
				GameManager.Gm.AddScore(-(pointLost * 100));
				lostPoints = true;
			}
		}
	}

	private void SubtractOres(PlayerResources presources) {
		if(HasOres && !presources.MaxedOres) {
			float resourceLost = Mathf.Lerp(0, ores, curve.Evaluate(curveTime));
			ores -= resourceLost;
			presources.Ores += resourceLost;
			if(ores < 0.5f || presources.MaxedOres) {
				ores = Mathf.Floor(ores);
				presources.Ores = Mathf.Ceil(presources.Ores);
			}
		}
	}

	private void SubtractWater(PlayerResources presources) {
		if(HasWater&& !presources.MaxedWater) {
			float resourceLost = Mathf.Lerp(0, water, curve.Evaluate(curveTime));
			water -= resourceLost;
			presources.Water += resourceLost;
			if(water < 0.5f || presources.MaxedWater) {
				water = Mathf.Floor(water);
				presources.Water = Mathf.Ceil(presources.Water);
			}
		}
	}

	private void SubtractLava(PlayerResources presources) {
		if(HasLava && !presources.MaxedLava) {
			float resourceLost = Mathf.Lerp(0, lava, curve.Evaluate(curveTime));
			lava -= resourceLost;
			presources.Lava += resourceLost;
			if(lava < 0.5f || presources.MaxedLava) {
				lava = Mathf.Floor(lava);
				presources.Lava = Mathf.Ceil(presources.Lava);
			}
		}
	}

	private void SubtractFuel(PlayerResources presources) {
		if(HasFuel && !presources.MaxedFuel) {
			float resourceLost = Mathf.Lerp(0, fuel, curve.Evaluate(curveTime));
			fuel -= resourceLost;
			presources.Fuel += resourceLost;
			if(fuel < 0.5f || presources.MaxedFuel) {
				fuel = Mathf.Floor(fuel);
				presources.Fuel = Mathf.Ceil(presources.Fuel);
			}
		}
	}
}
