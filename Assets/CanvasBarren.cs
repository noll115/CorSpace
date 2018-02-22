using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasBarren : MonoBehaviour {

	public List<BarrenPlanet> planets;
	public Image lavaBar;
	public Image waterBar;
	public Image oresBar;
	public Image lifeBar;
	public RocketController player;
	public PlanetInfo closestPlanet;
	public Color lifeBarColor = new Color(0, 0, 0, 1);
	public GameObject info;
	public float finalDist;
	public float turnOnDist;

	private void Awake() {
		player = GameObject.FindGameObjectWithTag("Player").GetComponentInParent<RocketController>();
	}

	private void Update() {
		if(planets != null) {
			float closestDist = Mathf.Infinity;
			for(int i = 0;i < planets.Count;i++) {
				float dist = (player.transform.position - planets[i].transform.position).sqrMagnitude;
				if(dist < closestDist) {
					closestDist = dist;
					finalDist = dist;
					closestPlanet = planets[i].planetInfo;
				}
			}
			if(closestPlanet != null && finalDist < turnOnDist * turnOnDist) {
				info.SetActive(true);
				float scale = 1 - (finalDist / 1000f);
				scale = Mathf.Clamp(scale, 0, 1);
				transform.localScale = new Vector3(scale, scale, 1);
				transform.position = closestPlanet.position + (player.transform.position - closestPlanet.position) / 2;
				UpdateBars(closestPlanet);
			}
			else {
				info.SetActive(false);
			}
		}
	}


	//TODO: Create checking list of planets see whats closer

	public void UpdateBars(PlanetInfo br) {
		lavaBar.rectTransform.localScale = new Vector3(br.lavaAmount / br.maxLava, 1, 1);
		waterBar.rectTransform.localScale = new Vector3(br.waterAmount / br.maxWater, 1, 1);
		oresBar.rectTransform.localScale = new Vector3(br.oresAmount / br.maxOres, 1, 1);
		lifeBar.rectTransform.localScale = new Vector3(br.lifeAmount, 1, 1);
		lifeBarColor.r = 1 - (br.lifeAmount);
		lifeBarColor.g = (br.lifeAmount);
		lifeBar.color = lifeBarColor;
	}

	public void ChangeCell(SpaceCell cell) {
		planets = cell.barrenPlanets;
	}



}
