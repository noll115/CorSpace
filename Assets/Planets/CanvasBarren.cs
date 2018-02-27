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
	public PlanetInfo[] closestPlanets = new PlanetInfo[3];
	public float[] closestPlanetDist = new float[3];
	public Color lifeBarColor = new Color(0, 0, 0, 1);
	public GameObject info;
	public float turnOnDist;

	private void Awake() {
		player = GameObject.FindGameObjectWithTag("Player").GetComponentInParent<RocketController>();
	}

	private void Update() {
		if (planets != null) {
			float closestDist = Mathf.Infinity;
			for (int i = 0; i < planets.Count; i++) {
				float dist = (player.transform.position - planets[i].transform.position).sqrMagnitude;
				if (dist < closestDist) {
					closestDist = dist;
					//closestPlanetDist[0] = dist;
					//closestPlanets[0] = planets[i].planetInfo;
					for (int j = 0; j < closestPlanets.Length; j++) {
						if (dist < closestPlanetDist[j]) {
							if (closestPlanets[1 + j]) {
								closestPlanets[1 + j] = closestPlanets[j];
								closestPlanetDist[1 + j] = closestPlanetDist[j];
							}
							closestPlanetDist[j] = dist;
							closestPlanets[j] = planets[i].planetInfo;
							return;
						}
					}
					/*TODO: Create an array of the top three closests planets
					Compare planet dist with dists in array 
					starting from top. If dist less dist at array move it down one.
					*/
				}
			}
			if (closestPlanets[0] != null && closestPlanetDist[0] < turnOnDist * turnOnDist) {
				info.SetActive(true);
				float scale = 1 - (closestPlanetDist[0] / 1000f);
				scale = Mathf.Clamp(scale, 0, 1);
				transform.localScale = new Vector3(scale, scale, 1);
				transform.position = closestPlanets[0].position + (player.transform.position - closestPlanets[0].position) / 2;
				UpdateBars(closestPlanets[0]);
			}
			else {
				info.SetActive(false);
			}
		}
	}



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
