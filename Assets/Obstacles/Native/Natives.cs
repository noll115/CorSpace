using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Natives : MonoBehaviour,Obstacle {

	public float fireCoolDown = 0.5f;
	public float burstTimer = 1f;
	public int pelletburst = 3;
	public float timer;
	public float pelletSpeed;
	public GameObject pellet;
	public Transform turnPoint;
	public Transform barrelEnd;
	static Pooler<Pellet> pooler;
	public Transform target;
	public Rigidbody2D targetrigid;
	public bool firstShot = true;
	public bool firing = false;

	private void Awake() {
		if (pooler == null) {
			pooler = new Pooler<Pellet>(pellet, 30);
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

	public void Setup(SpawnablePlanet planet,Vector3 pos, Vector3 upVec,Vector3 scale){
		SpriteRenderer r = GetComponent<SpriteRenderer>();
		r.color = planet.baseColor;
		r = GetComponentInChildren<SpriteRenderer>();
		r.color = planet.secondaryColor;
		transform.position = pos;
		transform.up = upVec;
		transform.localScale = scale;
	}

	private void Update() {
		if (target) {
			Vector3 dir = (target.position + (Vector3)targetrigid.velocity) - turnPoint.position;
			float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90;
			//Debug.Log(angle);
			turnPoint.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
			if (timer >= burstTimer || firstShot) {
				StartCoroutine(FireBurst());
				Debug.DrawRay(barrelEnd.position, dir, Color.red, 10);
				timer = 0f;
				firstShot = false;
			}
			else if(!firing) {
				timer += Time.deltaTime;
			}
		}
	}

	IEnumerator FireBurst(){
		firing = true;
		int numShot = 0;
		while (numShot < pelletburst) {
			Pellet p = pooler.GetFromPool();
			p.Shoot(barrelEnd.position, barrelEnd.up * pelletSpeed, barrelEnd.up);
			numShot++;
			yield return new WaitForSeconds(fireCoolDown);
		}
		firing = false;
	}

}
