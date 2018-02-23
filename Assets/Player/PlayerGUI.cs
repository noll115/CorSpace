using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerGUI : MonoBehaviour {

	public Image fuelBar;
	public Image lavaBar;
	public Image waterBar;
	public Image oreBar;
	public PlayerResources rockCon;
	Vector3 fuelAmount = new Vector3(0,1);
	Vector3 lavaAmount = new Vector3(0,1);
	Vector3 waterAmount = new Vector3(0,1);
	Vector3 oreAmount = new Vector3(0,1);


	private void Awake() {
		rockCon = FindObjectOfType<PlayerResources>();
		rockCon.UpdateFuel += UpdateFuelGUI;
		rockCon.OnResourceChange += UpdateResourceBars;
	}


	void UpdateFuelGUI(float fuel) {
		fuelAmount.x = fuel / 100f;
		//print(fuelAmount);
		fuelBar.rectTransform.localScale = fuelAmount;
	}

	public void UpdateResourceBars(PlayerResources p) {
		lavaAmount.x = p.Lava / p.maxLava;
		waterAmount.x = p.Water / p.maxWater;
		oreAmount.x = p.Ores / p.maxOres;
		lavaBar.rectTransform.localScale = lavaAmount;
		waterBar.rectTransform.localScale = waterAmount;
		oreBar.rectTransform.localScale = oreAmount;
	}

}
