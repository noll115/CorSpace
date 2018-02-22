using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : Obstacle {

	static GameObject astroidParent;
	Rigidbody2D rigid;
	public Planet planetOrbiting;
	public bool hit;
	Vector2 dist;
	public float distMag;
	public float circlePos;
	public float speed;

	private void Awake() {
		rigid = GetComponent<Rigidbody2D>();
		if(!astroidParent) {
			astroidParent = new GameObject("Asteroids");
		}
		transform.SetParent(astroidParent.transform, true);
	}

	public void SetUp(Planet planetOrb, float CirleRad, float scale) {
		transform.localScale = new Vector3(scale, scale);
		circlePos = CirleRad;
		planetOrbiting = planetOrb;
		dist = transform.position - planetOrb.transform.position;
		distMag = dist.magnitude;
		speed = (Mathf.PI/5)/Random.Range(1,distMag) ;
		//print(distMag);
	}

	private void FixedUpdate() {
		if(!hit) {
			if(planetOrbiting) {
				circlePos = (circlePos + (speed * Time.fixedDeltaTime)) % (Mathf.PI*2);
				rigid.MovePosition(new Vector3(Mathf.Cos(circlePos ) * distMag, Mathf.Sin(circlePos ) * distMag) + planetOrbiting.transform.position);
			}
		}
		else {
			Destroy(gameObject, 7f);
		}
	}

	private void OnCollisionEnter2D(Collision2D collision) {
		if(collision.contacts.Length > 0) {
			if(collision.collider.CompareTag("Player") || collision.collider.CompareTag("Asteroid")) {
				rigid.bodyType = RigidbodyType2D.Dynamic;
				hit = true;
				float speed = collision.relativeVelocity.magnitude * 1.5f;
				Vector3 colDir = collision.contacts[0].normal;
				collision.rigidbody.AddForce(-colDir * speed*1.5f, ForceMode2D.Impulse);
				rigid.AddForce(colDir * speed, ForceMode2D.Impulse);
				planetOrbiting = null;

			}
			else if(collision.collider.CompareTag("Planet")) {
				SpriteManager.instance.UseExplosion(collision.contacts[0].point, 5);
				Destroy(gameObject);
			}
		}
	}
}
