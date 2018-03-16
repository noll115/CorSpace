using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceCell : MonoBehaviour {

	public BoxCollider2D box2d;
	public List<Planet> planets;
	public int cellRow,cellCol;

	public void Setup(int row,int col,BoxCollider2D bx,List<Planet> planets, Vector3 cellPos) {
		this.cellRow = row;
		this.cellCol = col;
		box2d = bx;
		this.planets = planets;
		transform.position = cellPos;
		ActivatePlanets(false);
	}


	void ActivatePlanets(bool active) {
		for (int i = 0; i < planets.Count; i++){
			planets[i].gameObject.SetActive(active);
		}
	}

	private void OnTriggerExit2D(Collider2D collision) {
		if(collision.CompareTag("Player") && !collision.GetComponentInParent<RocketController>().pResources.dead) {
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
