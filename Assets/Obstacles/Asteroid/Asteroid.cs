using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour {

	static GameObject astroidParent;
	Rigidbody2D rigid;
	public Planet planetOrbiting;
	public bool hit;
	Vector2 dist;
	public float distMag;
	public float circlePos;
	public float speed;
	ContactPoint2D[] contactPoints = new ContactPoint2D[1];
	Vector2 newPos = new Vector2();

	private void Awake() {
		rigid = GetComponent<Rigidbody2D>();
		if (!astroidParent) {
			astroidParent = new GameObject("Asteroids");
		}
		transform.SetParent(astroidParent.transform, true);
	}


	private void FixedUpdate() {
		if (!hit) {
			if (planetOrbiting) {
				circlePos = (circlePos + (speed * Time.fixedDeltaTime)) % (Mathf.PI * 2);
				newPos.Set(Mathf.Cos(circlePos) * distMag, Mathf.Sin(circlePos) * distMag);
				rigid.MovePosition(newPos + planetOrbiting.rigid.position);
			}
		}
	}

	private void OnCollisionEnter2D(Collision2D collision) {
		if (!collision.collider.CompareTag("Asteroid")) {
			collision.GetContacts(contactPoints);
			DetermineCollision(collision);
		}
	}

	private void DetermineCollision(Collision2D collision) {
		SpriteManager.instance.UseExplosion(contactPoints[0].point);
		if (!hit) {
			if (collision.collider.CompareTag("Player")) {
				rigid.bodyType = RigidbodyType2D.Dynamic;
				hit = true;
				float speed = collision.relativeVelocity.magnitude;
				Vector3 colDir = contactPoints[0].normal;
				collision.rigidbody.AddForce(-colDir * speed * 3f, ForceMode2D.Impulse);
				rigid.AddForce(colDir * speed, ForceMode2D.Impulse);
				RemovePlanet();
				Destroy(gameObject, 7f);
			}
		}
		else {
			if (collision.collider.CompareTag("Planet")) {
				collision.collider.GetComponentInParent<Planet>().AstroidHit();
				if (transform.GetChild(0).GetComponent<Renderer>().isVisible) {
					CamController.instance.ShakeCam(0.6f, 0.2f);
				}
				RemovePlanet();
				Destroy(gameObject);
			}
			else if (collision.collider.CompareTag("Player")) {
				collision.collider.GetComponentInParent<RocketController>().pResources.HealthChange(-20);
			}
			else if (collision.collider.CompareTag("Obstacle")) {
				Planet planet = collision.collider.GetComponent<IObstacle>().ParentPlanet;
				planet.AstroidHit();
				RemovePlanet();
				Destroy(gameObject);
			}
		}
		
	}

	private void RemovePlanet() {
		if (planetOrbiting) {
			planetOrbiting.RemoveAstroid(this);
			planetOrbiting = null;
		}
	}

	public void Setup(Planet planet, Vector3 pos, float circleRad, float scale) {
		transform.localScale = new Vector3(scale, scale);
		transform.position = pos;
		circlePos = circleRad;
		planetOrbiting = planet;
		dist = transform.position - planet.transform.position;
		distMag = dist.magnitude;
		speed = (Mathf.PI / 5) / Random.Range(1, distMag) * PosOrNegVel();
		//print(distMag);
	}

	int PosOrNegVel() {
		return (Random.value > 0.5f) ? 1 : -1;
	}
}
