using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrenResources : MonoBehaviour {

	PlanetInfo info;
	[SerializeField] float collectionSpeed = 2f;
    [SerializeField] int pointsGained = 4;
	private const string Name = "_Lerp";
	public float lava;
	public float water;
	public float ores;
	public float maxLava = 150f;
	public float maxWater = 150f;
	public float maxOres = 150f;
	public bool FullLava {
		get {
			return lava >= maxLava;
		}
	}
	public bool FullWater {
		get {
			return water >= maxWater;
		}
	}
	public bool FullOres {
		get {
			return ores >= maxOres;
		}
	}
	public float PlanetHealth {
		get {
			return (lava + water + ores) / (maxLava + maxWater + maxOres);
		}
	}
	public Material mat;
	public float life = 0f;
	bool gotBonus = false;

	private void Awake() {
        mat = GetComponentInChildren<Renderer>().material;
    }




	public void AddResources(PlayerResources playerResources) {
		if((playerResources.HasOres || playerResources.HasLava || playerResources.HasWater) && (!FullLava || !FullOres || !FullWater)) {
            float collected = collectionSpeed * Time.deltaTime;
			AddOres(playerResources,collected);
            AddLava(playerResources,collected);
			AddWater(playerResources,collected);
			// curveTime += collectionSpeed * Time.deltaTime;
			// curveTime = Mathf.Clamp(curveTime, 0, 1f);
			life = (lava + water + ores) / (maxLava + maxWater + maxOres);
			mat.SetFloat(Name, life);
			if(life > 0) {
				//print(curveTime);
				GameManager.Gm.AddScore(pointsGained);
			}
			if(FullLava && FullOres && FullWater && !gotBonus) {
				print("FINISHED");
				GameManager.Gm.AddScore(pointsGained * 100);
				SpriteManager.instance.UseParticles(transform.position, "Celebrate",info);
				gotBonus = true;
			}
		}
	}

	private void AddLava(PlayerResources playerResources,float collected) {
		if(playerResources.HasLava && !FullLava) {
			// float collected = Mathf.SmoothStep(0, playerResources.Lava, curve.Evaluate(curveTime));
			playerResources.Lava -= collected;
			lava += collected;
			if(playerResources.Lava < 0.5f) {
				playerResources.Lava = 0f;
				lava = Mathf.Clamp(Mathf.Ceil(lava), 0, maxLava);
			}
		}
		else {//FULL
			lava = Mathf.Clamp(lava, 0, maxLava);
			playerResources.Lava = Mathf.Ceil(playerResources.Lava);
		}
	}


	private void AddWater(PlayerResources playerResources,float collected) {
		if(playerResources.HasWater && !FullWater) {
			// float collected = Mathf.SmoothStep(0, playerResources.Water, curve.Evaluate(curveTime));
			playerResources.Water -= collected;
			water += collected;
			if(playerResources.Water < 0.5f) {
				playerResources.Water = 0f;
				water = Mathf.Clamp(Mathf.Ceil(water), 0, maxWater);
			}
		}
		else {//FULL
			water = Mathf.Clamp(water, 0, maxWater);
			playerResources.Water = Mathf.Ceil(playerResources.Water);
		}
	}


	private void AddOres(PlayerResources playerResources,float collected) {
		if(playerResources.HasOres && !FullOres) {
			// float collected = Mathf.SmoothStep(0, playerResources.Ores, curve.Evaluate(curveTime));
			playerResources.Ores -= collected;
			ores += collected;
			if(playerResources.Ores < 0.5f) {
				playerResources.Ores = 0f;
				ores = Mathf.Clamp(Mathf.Ceil(ores), 0, maxOres);
			}
		}
		else {//FULL
			ores = Mathf.Clamp(ores, 0, maxOres);
			playerResources.Ores = Mathf.Ceil(playerResources.Ores);
		}
	}

	public void Setup(PlanetInfo info) {
		this.info = info;
		maxLava = info.maxLava;
		maxOres = info.maxOres;
		maxWater = info.maxWater;
		mat.SetFloat(Name, life);

	}
}
