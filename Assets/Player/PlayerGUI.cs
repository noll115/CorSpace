using UnityEngine;
using UnityEngine.UI;

public class PlayerGUI : MonoBehaviour {

	public Image fuelBar;
	public Image lavaBar;
	public Image waterBar;
	public Image healthbar;
	PlayerResources playerResources;
	float maxPlayerHealth;
	Vector3 fuelAmount = new Vector3(0,1);
	Vector3 lavaAmount = new Vector3(0,1);
	Vector3 waterAmount = new Vector3(0,1);


	private void Awake() {
		playerResources = FindObjectOfType<RocketController>().pResources;
		playerResources.UpdateFuel += UpdateFuelGUI;
		playerResources.OnResourceChange += UpdateResourceBars;
		playerResources.OnHealthChange += UpdateHealthBar;
		maxPlayerHealth = playerResources.maxPlayerHealth;
	}

	void UpdateHealthBar(float playerHealth){
		healthbar.transform.localScale = new Vector3(playerHealth / maxPlayerHealth, 1, 1);
	}


	void UpdateFuelGUI(float fuel) {
		fuelAmount.x = fuel / 100f;
		//print(fuelAmount);
		fuelBar.rectTransform.localScale = fuelAmount;
	}

	public void UpdateResourceBars(PlayerResources p) {
		lavaAmount.x = p.Lava / p.maxLava;
		waterAmount.x = p.Water / p.maxWater;
		lavaBar.rectTransform.localScale = lavaAmount;
		waterBar.rectTransform.localScale = waterAmount;
	}

}
