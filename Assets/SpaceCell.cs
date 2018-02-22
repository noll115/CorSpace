using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceCell : MonoBehaviour {

	public BoxCollider2D box2d;
	public List<BarrenPlanet> barrenPlanets;
	public List<ResourcePlanet> resourcePlanets;
	public int cellRow,cellCol;

	public void Setup(int row,int col,BoxCollider2D bx, List<BarrenPlanet> bplanets, List<ResourcePlanet> rPlanets, Vector3 cellPos) {
		this.cellRow = row;
		this.cellCol = col;
		box2d = bx;
		barrenPlanets = bplanets;
		resourcePlanets = rPlanets;
		transform.position = cellPos;
		ActivatePlanets(false);
	}


	void ActivatePlanets(bool activate) {
		for(int i = 0;i < barrenPlanets.Count;i++) {
			if(barrenPlanets[i])
				barrenPlanets[i].gameObject.SetActive(activate);
		}
		for(int i = 0;i < resourcePlanets.Count;i++) {
			if(resourcePlanets[i])
				resourcePlanets[i].gameObject.SetActive(activate);
		}
	}

	private void OnTriggerExit2D(Collider2D collision) {
		if(collision.CompareTag("Player")) {
			ActivatePlanets(false);
		}
	}


	private void OnTriggerEnter2D(Collider2D collision) {
		if(collision.CompareTag("Player")) {
			collision.GetComponentInParent<RocketController>().SetCell(cellRow, cellCol);
			ActivatePlanets(true);
		}
	}
}
