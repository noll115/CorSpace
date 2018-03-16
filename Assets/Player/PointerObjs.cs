using System.Collections.Generic;
using UnityEngine;
public class PointerObjs : MonoBehaviour {

	Transform parent;
	public Transform[] pointers = new Transform[3];
	public Transform[] dangerPointers = new Transform[3];
	public List<Planet> planetsNearby;
	public float turnOnDist;

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
			Planets planetsType;
			Transform pointerToUse;
			if (i < planetsNearby.Count) {
				planetsType = planetsNearby[i].planetType;
				neededdir = planetsNearby[i].transform.position - parent.position;
				pointerToUse = determinePointer(planetsType, i);
				if (neededdir.sqrMagnitude > turnOnDist * turnOnDist) {
					pointerToUse.gameObject.SetActive(true);
					float angle = Mathf.Atan2(neededdir.y, neededdir.x) * Mathf.Rad2Deg;
					pointerToUse.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
				}
				else {
					pointerToUse.gameObject.SetActive(false);
				}
			}
			else {
				pointers[i].gameObject.SetActive(false);
				dangerPointers[i].gameObject.SetActive(false);
			}
		}
	}


	Transform determinePointer(Planets type, int index) {
		Transform t = null;
		switch (type) {
			case Planets.BARREN:
			case Planets.RESOURCE:
				t = pointers[index].transform;
				dangerPointers[index].gameObject.SetActive(false);
				break;
			case Planets.SUN:
			case Planets.BLACKHOLE:
				t = dangerPointers[index].transform;
				pointers[index].gameObject.SetActive(false);
				break;
		}
		return t;
	}





}