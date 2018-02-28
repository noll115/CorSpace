using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class PlanetCanvasController : MonoBehaviour {

	public List<BarrenPlanet> planetsB;
    public List<ResourcePlanet> planetsR;

	public PlanetInfo[] closestPlanets = new PlanetInfo[3];
	public float[] closestPlanetDist = { Mathf.Infinity, Mathf.Infinity, Mathf.Infinity };
	public float turnOnDist;

	
	public GameObject resourceCanvas;
	public Transform player;
	public TextMeshProUGUI fuelNum;
	public TextMeshProUGUI waterNum;
	public TextMeshProUGUI lavaNum;
	public TextMeshProUGUI oresNum;
	public Image lifeBarR;
	public float finalDist;
	public Color lifeCol = new Color(0, 0, 0, 1);

	
	public GameObject barrenCanvas;
	public Image lavaBar;
	public Image waterBar;
	public Image oresBar;
	public Image lifeBarB;
	public Color lifeBarColor = new Color(0, 0, 0, 1);



	private void Awake() {
		player = GameObject.FindGameObjectWithTag("Player").transform;
	}

	private void Update() {
		if (planetsB != null && planetsR != null) {
			//ComparePlanetDist(planetsB);
			//ComparePlanetDist(planetsR);
			planetsB = planetsB.OrderBy(x => Vector2.SqrMagnitude(player.position - x.transform.position)).ToList();
			planetsR = planetsR.OrderBy(x => Vector2.SqrMagnitude(player.position - x.transform.position)).ToList();
			//TODO:Find more efficient way to order close planets;
		}
	}



	private void ComparePlanetDist<T>(List<T> listOfPlanets) where T : Planet {
		for (int i = 0; i < listOfPlanets.Count; i++) {
			float dist = (player.position - listOfPlanets[i].transform.position).sqrMagnitude;
			if (dist < closestPlanetDist[2]) {
				for (int j = 0; j < closestPlanets.Length; j++) {
					if (dist < closestPlanetDist[j] && listOfPlanets[i].planetInfo != closestPlanets[j]) {
						if (j + 1 < closestPlanets.Length) {
							closestPlanets[j + 1] = closestPlanets[j];
							closestPlanetDist[j + 1] = closestPlanetDist[j];
						}
						closestPlanetDist[j] = dist;
						closestPlanets[j] = listOfPlanets[i].planetInfo;
						break;
					}
				}
			}
		}
	}


	public void SetNums(PlanetInfo info) {
		fuelNum.SetText(Mathf.Floor(info.fuelAmount).ToString());
		lavaNum.SetText(Mathf.Floor(info.lavaAmount).ToString());
		oresNum.SetText(Mathf.Floor(info.oresAmount).ToString());
		waterNum.SetText(Mathf.Floor(info.waterAmount).ToString());
		lifeCol.r = 1 - info.lifeAmount;
		lifeCol.g = info.lifeAmount;
		lifeBarR.color = lifeCol;
		lifeBarR.rectTransform.localScale = new Vector3(info.lifeAmount, 1, 1);
	}

	public void UpdateBars(PlanetInfo info) {
		lavaBar.rectTransform.localScale = new Vector3(info.lavaAmount / info.maxLava, 1, 1);
		waterBar.rectTransform.localScale = new Vector3(info.waterAmount / info.maxWater, 1, 1);
		oresBar.rectTransform.localScale = new Vector3(info.oresAmount / info.maxOres, 1, 1);
		lifeBarB.rectTransform.localScale = new Vector3(info.lifeAmount, 1, 1);
		lifeBarColor.r = 1 - (info.lifeAmount);
		lifeBarColor.g = (info.lifeAmount);
		lifeBarB.color = lifeBarColor;
	}

	public void ChangeCell(SpaceCell cell) {
		planetsR = cell.resourcePlanets;
		planetsB = cell.barrenPlanets;
	}

}