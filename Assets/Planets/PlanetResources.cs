using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetResources : MonoBehaviour {

	public float collectionSpeed = 2f;
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

	public float life = 1f;
	Material mat;
	private const string Name = "_CutOff";
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
		if ((!presources.MaxedFuel || !presources.MaxedHealth || !presources.MaxedWater || !presources.MaxedLava) && (HasFuel || HasWater || HasLava || HasOres)) {
			presources.PlayTaking();
			float collected = collectionSpeed * Time.deltaTime;
			SubtractFuel(presources, collected);
			SubtractLava(presources, collected);
			SubtractWater(presources, collected);
			SubtractOres(presources, collected);
			CalculateLife();
		}
		else {
			presources.StopTaking();
		}
	}

	private void CalculateLife() {
		life = (fuel + lava + water + ores) / (maxFuel + maxLava + maxWater + maxOres);
		mat.SetFloat(Name, life * 2);
		if (life < 0.5f) {
			GameManager.Gm.AddScore(-pointLost);
		}
		else {
			GameManager.Gm.AddScore(pointsGained);
		}
		if (life <= 0 && !lostPoints) {
			GameManager.Gm.AddScore(-(pointLost * 100));
			GameManager.Gm.NumOfPlanetsDead++;
			lostPoints = true;
		}
	}

	private void SubtractOres(PlayerResources presources, float collected) {
		if (HasOres && !presources.MaxedHealth) {
			ores -= collected;
			presources.Health += collected;
			if (ores < 0.5f || presources.MaxedHealth) {
				ores = Mathf.Floor(ores);
				presources.Health = Mathf.Ceil(presources.Health);
			}
		}
	}

	private void SubtractWater(PlayerResources presources, float collected) {
		if (HasWater && !presources.MaxedWater) {
			water -= collected;
			presources.Water += collected;
			if (water < 0.5f || presources.MaxedWater) {
				water = Mathf.Floor(water);
				presources.Water = Mathf.Ceil(presources.Water);
			}
		}
	}

	private void SubtractLava(PlayerResources presources, float collected) {
		if (HasLava && !presources.MaxedLava) {
			lava -= collected;
			presources.Lava += collected;
			if (lava < 0.5f || presources.MaxedLava) {
				lava = Mathf.Floor(lava);
				presources.Lava = Mathf.Ceil(presources.Lava);
			}
		}
	}

	private void SubtractFuel(PlayerResources presources, float collected) {
		if (HasFuel && !presources.MaxedFuel) {
			fuel -= collected;
			presources.Fuel += collected;
			if (fuel < 0.5f || presources.MaxedFuel) {
				fuel = Mathf.Floor(fuel);
				presources.Fuel = Mathf.Ceil(presources.Fuel);
			}
		}
	}

	public void AstroidHit() {
		lava -= lava / 3;
		water -= water / 3;
		fuel -= fuel / 3;
		ores -= ores / 3;
		lava = Mathf.Clamp(lava, 0, maxLava);
		water = Mathf.Clamp(water, 0, maxWater);
		fuel = Mathf.Clamp(fuel, 0, maxFuel);
		ores = Mathf.Clamp(ores, 0, maxOres);
		StartCoroutine(PlanetHit());
		CalculateLife();
	}


	IEnumerator PlanetHit() {
		mat.SetFloat("_Flash", 1);
		yield return new WaitForSeconds(0.1f);
		mat.SetFloat("_Flash", 0);
	}
}
