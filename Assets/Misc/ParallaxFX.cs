using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[SelectionBase]
public class ParallaxFX : MonoBehaviour {

	public float Damp;
	public Transform obj;
	Material mat;
	string txt = "_MainTex";

	private void Awake() {
		mat = GetComponent<Renderer>().material;
	}

	private void LateUpdate() {
		mat.SetTextureOffset(txt,-obj.transform.position / Damp );
	}

}
