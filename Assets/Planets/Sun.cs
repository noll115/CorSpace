using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun : Planet {


	protected override void OnTriggerStay2D(Collider2D other) {
		
	}

	protected override void OnTriggerEnter2D(Collider2D collision){
		if (collision.CompareTag("Player")) {
			con = collision.GetComponentInParent<RocketController>();
		}
		if (collision.CompareTag("Asteroid")) {
			collision.GetComponentInParent<Asteroid>().hit = true;
		}
	}

	protected override void OnCollisionEnter2D(Collision2D collision){
		if(collision.collider.CompareTag("Player")){
			con.pResources.HealthChange(-con.pResources.maxPlayerHealth);
		}
	}

	protected override void OnCollisionStay2D(Collision2D collision){

	}

	protected override void OnCollisionExit2D(Collision2D collision){

	}
}
