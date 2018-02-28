using UnityEngine;
using System.Collections.Generic;

public class PointerObjs : MonoBehaviour {

	Transform parent;
	public Transform[] pointers = new Transform[3];

	private void Awake() {
		parent = transform.parent;
	}

	public void updatePointers(List<Planet> nearbyPlanets) {

		for (int i = 0; i < pointers.Length; i++) {
			Vector2 dir, neededdir;
			if (nearbyPlanets[i]) {
				dir = pointers[i].position - parent.position;
				neededdir = nearbyPlanets[i].initPos - parent.position;
				float angleDif = Vector2.Angle(dir, neededdir);
				pointers[i].RotateAround(parent.position, Vector3.forward, angleDif);
			}
		}

	}

}