using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : Planet {



	protected override void OnTriggerStay2D(Collider2D collision) {

	}

	protected override void OnTriggerEnter2D(Collider2D collision) {
		
	}

	protected override void OnCollisionEnter2D(Collision2D collision) {
		if(collision.collider.CompareTag("Player")) {
			collision.collider.GetComponentInParent<RocketController>().InstaDeath();
		}
	}

}
