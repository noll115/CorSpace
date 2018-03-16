using UnityEngine;
using System.Collections.Generic;

public class PlanetRadar : MonoBehaviour {

	public List<Planet> planetsNearby = new List<Planet>();

    
	private void OnTriggerEnter2D(Collider2D other) {
		if (other.CompareTag("Planet")) {
			planetsNearby.Add(other.GetComponentInParent<Planet>());
		}
	}


	private void OnTriggerExit2D(Collider2D other) {
		if (other.CompareTag("Planet")) {
			planetsNearby.Remove(other.GetComponentInParent<Planet>());
		}
	}


	public void NewCell(){
		planetsNearby.Clear();
	}


}