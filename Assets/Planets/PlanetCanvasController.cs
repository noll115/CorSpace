using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlanetCanvasController : MonoBehaviour {

	public List<Planet> nearbyPlanets;
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
	WaitForSeconds ws = new WaitForSeconds(0.5f);


	private void Awake() {
		nearbyPlanets = player.GetComponentInChildren<PlanetRadar>().planetsNearby;
		StartCoroutine(OrderList());
	}

	IEnumerator OrderList() {
		while (true) {
			nearbyPlanets.OrderBy(x=>Vector2.SqrMagnitude(player.position - x.transform.position));
			yield return ws;
		}
	}


	private void UpdateCanvas(Planet firstClose,Planet sndClose){

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

}