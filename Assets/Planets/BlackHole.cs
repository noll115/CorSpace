using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : Planet {

	protected override void FixedUpdate(){

	}


	protected override void OnTriggerStay2D(Collider2D other) {
		if (other.CompareTag("Player")) {
			Transform player = con.transform;
			Vector3 dist = (transform.position - player.position);
			float scale = dist.sqrMagnitude*1.5f / playerInitPos;
			scale = Mathf.Clamp(scale, 0f, 1f);
			con.transform.localScale = new Vector3(scale, scale, scale);
		}
	}

	protected override void OnTriggerEnter2D(Collider2D collision) {
		if (collision.CompareTag("Player")) {
			con = collision.GetComponentInParent<RocketController>();
			playerInitPos = (transform.position - con.transform.position).sqrMagnitude;
		}
		if (collision.CompareTag("Asteroid")) {
			collision.GetComponentInParent<Asteroid>().hit = true;
		}
	}

	protected override void OnTriggerExit2D(Collider2D collision) {
		if (collision.CompareTag("Player")) {
			con.transform.localScale = Vector3.one;
			con = null;
		}
	}

	protected override void OnCollisionEnter2D(Collision2D collision) {
		if (collision.collider.CompareTag("Player")) {
			con.pResources.HealthChange(-1000);
		}
	}

	protected override void OnCollisionStay2D(Collision2D collision) {

	}

	protected override void OnCollisionExit2D(Collision2D collision) {

	}

}
