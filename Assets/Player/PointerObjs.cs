using UnityEngine;
using System.Collections.Generic;

public class PointerObjs : MonoBehaviour {

	Transform parent;
	public Transform[] pointers = new Transform[3];
	public List<Planet> planetsNearby;
	public float turnOfDist;

	private void Awake() {
		parent = transform.parent;
		this.planetsNearby = transform.parent.GetComponentInChildren<PlanetRadar>().planetsNearby;
	}

	private void LateUpdate() {
		updatePointers();
	}

	public void updatePointers() {
		for (int i = 0; i < pointers.Length; i++) {
			Vector2 neededdir;
			if (i < planetsNearby.Count) {
				neededdir = planetsNearby[i].transform.position - parent.position;
				if (neededdir.sqrMagnitude > turnOfDist * turnOfDist) {
					pointers[i].gameObject.SetActive(true);
					float angle = Mathf.Atan2(neededdir.y, neededdir.x) * Mathf.Rad2Deg;
					pointers[i].rotation = Quaternion.AngleAxis(angle, Vector3.forward);
				}
				else{
					pointers[i].gameObject.SetActive(false);
				}
			}
			else {
				pointers[i].gameObject.SetActive(false);
			}
		}
	}



}