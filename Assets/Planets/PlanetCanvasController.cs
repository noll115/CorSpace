using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;

public class PlanetCanvasController : MonoBehaviour {

	public List<Planet> nearbyPlanets;
	public float turnOnDist;


	public GameObject resourceCanvas;
	public RocketController player;
	public TextMeshProUGUI fuelNum;
	public TextMeshProUGUI waterNum;
	public TextMeshProUGUI lavaNum;
	public TextMeshProUGUI oresNum;
	public Image lifeBarR;
	public Color lifeCol = new Color(0, 0, 0, 1);


	public GameObject barrenCanvas;
	public Image lavaBar;
	public Image waterBar;
	public Image lifeBarB;
	public Color lifeBarColor = new Color(0, 0, 0, 1);
	WaitForSeconds ws = new WaitForSeconds(0.5f);
	Comparison<Planet> comparison;


	private void Awake() {
		comparison = delegate (Planet a, Planet b) {
			if (a && b) {
				return Vector2.SqrMagnitude(player.transform.position - a.transform.position)
				.CompareTo(Vector2.SqrMagnitude(player.transform.position - b.transform.position));
			}
			else {
				if (a) {
					return 1;
				}
				else if (b) {
					return -1;
				}
				else {
					return 0;
				}
			}
		};
		turnOnDist *= turnOnDist;
		nearbyPlanets = player.GetComponentInChildren<PlanetRadar>().planetsNearby;
		StartCoroutine(OrderList());
	}

	IEnumerator OrderList() {
		while (true) {
			nearbyPlanets.Sort(comparison);
			yield return ws;
		}
	}


	private void Update() {
		UpdateCanvases();
	}


	private void UpdateCanvases() {
		bool bCanvasUsed = false;
		bool rCanvasUsed = false;
		for (int i = 0; i < 2; i++) {
			if (nearbyPlanets.Count > i) {
				switch (nearbyPlanets[i].planetType) {
					case Planets.RESOURCE:
						if (!rCanvasUsed) {
							rCanvasUsed = true;
							SetNums(nearbyPlanets[i].planetInfo);
						}
						break;
					case Planets.BARREN:
						if (!bCanvasUsed) {
							bCanvasUsed = true;
							UpdateBars(nearbyPlanets[i].planetInfo);
						}
						break;
					default:
						break;
				}
			}
			if (!bCanvasUsed) barrenCanvas.SetActive(false);
			if (!rCanvasUsed) resourceCanvas.SetActive(false);
		}
	}





	public void SetNums(PlanetInfo info) {
		Vector3 playerPos = player.transform.position;
		float dist = Vector2.SqrMagnitude(playerPos - info.position);
		if (dist < turnOnDist) {
			Vector3 dest;
			if (!player.PlanetOn) {
				dest = info.position + (playerPos - info.position) / 2;
			}
			else {
				dest = info.position;
			}
			resourceCanvas.SetActive(true);
			float scale = 1 - dist / turnOnDist;
			scale = Mathf.Clamp(scale, 0, 1);
			resourceCanvas.transform.localScale = new Vector3(scale, scale, 1);
			resourceCanvas.transform.position = Vector3.Lerp(resourceCanvas.transform.position, dest, 4 * Time.deltaTime);
			fuelNum.SetText(Mathf.Floor(info.fuelAmount).ToString());
			lavaNum.SetText(Mathf.Floor(info.lavaAmount).ToString());
			oresNum.SetText(Mathf.Floor(info.oresAmount).ToString());
			waterNum.SetText(Mathf.Floor(info.waterAmount).ToString());
			lifeCol.r = 1 - info.lifeAmount;
			lifeCol.g = info.lifeAmount;
			lifeBarR.color = lifeCol;
			lifeBarR.rectTransform.localScale = new Vector3(info.lifeAmount, 1, 1);
		}
		else {
			resourceCanvas.SetActive(false);
		}
	}

	public void UpdateBars(PlanetInfo info) {
		Vector3 playerPos = player.transform.position;
		float dist = Vector2.SqrMagnitude(playerPos - info.position);
		if (dist < turnOnDist) {
			Vector3 dest;
			if (!player.PlanetOn) {
				dest = info.position + (playerPos - info.position) / 2;
			}
			else {
				dest = info.position;
			}
			barrenCanvas.SetActive(true);
			float scale = 1 - dist / turnOnDist;
			scale = Mathf.Clamp(scale, 0, 1);
			barrenCanvas.transform.localScale = new Vector3(scale, scale, 1);
			barrenCanvas.transform.position = Vector3.Lerp(barrenCanvas.transform.position, dest, 4 * Time.deltaTime);
			lavaBar.rectTransform.localScale = new Vector3(info.lavaAmount / info.maxLava, 1, 1);
			waterBar.rectTransform.localScale = new Vector3(info.waterAmount / info.maxWater, 1, 1);
			lifeBarB.rectTransform.localScale = new Vector3(info.lifeAmount, 1, 1);
			lifeBarColor.r = 1 - (info.lifeAmount);
			lifeBarColor.g = (info.lifeAmount);
			lifeBarB.color = lifeBarColor;
		}
		else {
			barrenCanvas.SetActive(false);
		}
	}

}