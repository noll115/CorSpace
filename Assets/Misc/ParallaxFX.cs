using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxFX : MonoBehaviour {

	public float Damp;
	public RocketController con;
	public Material mat;
	public string txt = "_MainTex";

	private void Awake() {
		mat = GetComponent<Renderer>().material;
	}

	private void LateUpdate() {
		mat.SetTextureOffset(txt,-con.transform.position / Damp );
	}

}
