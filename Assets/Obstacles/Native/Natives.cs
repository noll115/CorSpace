using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Natives : Obstacle {

	public float fireCoolDown = 0.5f;
	public float timer;
	public float pelletSpeed;
	public GameObject pellet;
	public Transform barrelEnd;
	public Transform barrel;
	static Pooler<Pellet> pooler;
	public Transform target;
	public Rigidbody2D targetrigid;
	public bool firstShot = true;
	private void Awake() {
		if (pooler == null) {
			pooler = new Pooler<Pellet>(pellet, 3);
		}
	}

	private void OnTriggerEnter2D(Collider2D other) {
		if (other.CompareTag("Player")) {
			target = other.transform.parent;
			targetrigid = other.GetComponentInParent<Rigidbody2D>();
		}
	}

	private void OnTriggerExit2D(Collider2D other) {
		if (other.CompareTag("Player")) {
			target = null;
			firstShot = true;
		}
	}

	private void OnTriggerStay2D(Collider2D other) {
		
	}

	private void Update() {
		if (target) {
			Vector3 dir = (target.position+ (Vector3)targetrigid.velocity ) - barrelEnd.position;
			float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90;
			barrel.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
			if (timer >= fireCoolDown || firstShot) {
				Pellet p = pooler.GetFromPool();
				p.Shoot(barrelEnd.position,barrelEnd.up * pelletSpeed);
				Debug.DrawRay(barrelEnd.position, dir, Color.red, 10);
				timer = 0f;
				firstShot = false;
			}
			else {
				timer += Time.deltaTime;
			}
		}
	}


}
