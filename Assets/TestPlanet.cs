using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlanet : MonoBehaviour {

	Rigidbody2D rigid;
	RocketController con;

	private void Awake() {
		rigid = GetComponent<Rigidbody2D>();
	}

	private void Update() {
		rigid.rotation += 2 * Time.deltaTime;
	}

	private void OnCollisionEnter2D(Collision2D collision) {
		print("Collide");
		if(collision.collider.CompareTag("Player")) {
			con = collision.collider.GetComponentInParent<RocketController>();
			con.MakeJoints(rigid);
		}
	}

	private void OnCollisionStay2D(Collision2D collision) {
		if(con && Input.GetButton("Thrust")) {
			con.DisableJoints();
		}
	}

	private void OnCollisionExit2D(Collision2D collision) {
		if(collision.collider.CompareTag("Player")) {
			con = null;
		}
	}
}
