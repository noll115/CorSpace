using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResourceText : MonoBehaviour {

	List<ResourcePlanet> planets;
	public RocketController player;
	public TextMeshProUGUI fuelNum;
	public TextMeshProUGUI waterNum;
	public TextMeshProUGUI lavaNum;
	public TextMeshProUGUI oresNum;
	public Image lifeBar;
	public GameObject info;
	public float turnOnDist;
	public float finalDist;
	public PlanetInfo closestPlanet;
	public Color lifeCol = new Color(0, 0, 0, 1);

	private void Awake() {
		player = GameObject.FindGameObjectWithTag("Player").GetComponentInParent<RocketController>();
	}

	private void Update() {
		if(planets != null) {
			float closestDist = Mathf.Infinity;
			for(int i = 0;i < planets.Count;i++) {
				float dist = (planets[i].transform.position - player.transform.position).sqrMagnitude;
				if(dist < closestDist) {
					closestDist = dist;
					finalDist = dist;
					closestPlanet = planets[i].planetInfo;
				}
			}
			if(closestPlanet != null && finalDist < turnOnDist * turnOnDist) {
				info.SetActive(true);
				float scale = 1 - (finalDist / (turnOnDist * turnOnDist));
				scale = Mathf.Clamp(scale, 0, 1);
				transform.localScale = new Vector3(scale, scale, 1);
				transform.position = closestPlanet.position + (player.transform.position - closestPlanet.position) / 2;
				SetNums(closestPlanet);
			}
			else {
				info.SetActive(false);
			}
		}
	}



	//TODO: Create checking list of planets see whats closer


	public void SetNums(PlanetInfo info) {
		fuelNum.SetText(Mathf.Floor(info.fuelAmount).ToString());
		lavaNum.SetText(Mathf.Floor(info.lavaAmount).ToString());
		oresNum.SetText(Mathf.Floor(info.oresAmount).ToString());
		waterNum.SetText(Mathf.Floor(info.waterAmount).ToString());
		lifeCol.r = 1- info.lifeAmount;
		lifeCol.g = info.lifeAmount;
		lifeBar.color = lifeCol;
		lifeBar.rectTransform.localScale = new Vector3(info.lifeAmount, 1, 1);
	}

	public void ChangeCell(SpaceCell cell) {
		planets = cell.resourcePlanets;
	}


}
