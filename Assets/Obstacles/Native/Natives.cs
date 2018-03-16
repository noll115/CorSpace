using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Natives : MonoBehaviour, IObstacle {

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
	bool firstTime = true;
	[SerializeField]float firstTimeBurstTimer = 0.7f;
	float firstTimeBurst;
	public bool firing = false;
	public LineRenderer lr;
	Planet planetOn;

	public Planet ParentPlanet {
		get {
			return planetOn;
		}
	}

	private void Awake() {
		if (pooler == null) {
			pooler = new Pooler<Pellet>(pellet, 20);
		}
		else if (!pooler.GetFromPool()) {
			pooler = new Pooler<Pellet>(pellet, 20);
		}
		firstTimeBurst = firstTimeBurstTimer;

	}

	private void Start() {
		lr = GetComponent<LineRenderer>();
		lr.SetPosition(0, transform.position);
		lr.SetPosition(1, transform.position);

	}


	private void OnTriggerEnter2D(Collider2D other) {
		if (other.CompareTag("Player")) {
			target = other.transform.parent;
		}
	}

	private void OnTriggerStay2D(Collider2D other) {
		if (target) {
			lr.SetPosition(0, transform.position);
			lr.SetPosition(1, target.position);
			Vector3 dir = (target.position) - turnPoint.position;
			float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90;
			//Debug.Log(angle);
			turnPoint.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
			if (timer >= burstTimer || (firstTimeBurstTimer<=0 && firstTime) ) {
				firstTime = false;
				StartCoroutine(FireBurst());
				Debug.DrawRay(barrelEnd.position, dir, Color.red, 10);
				timer = 0f;
			}
			else if(!firing && firstTime){
				firstTimeBurstTimer -= Time.deltaTime;
			}
			else if (!firing) {
				timer += Time.deltaTime;
			}
		}
	}

	private void OnTriggerExit2D(Collider2D other) {
		if (other.CompareTag("Player")) {
			target = null;
			lr.SetPosition(1, transform.position);
			firstTime = true;
			firstTimeBurstTimer = firstTimeBurst;
		}
		
	}

	public void Setup(Planet planet, Vector3 pos, Vector3 upVec, Vector3 scale) {
		planetOn = planet;
		SpriteRenderer r = GetComponent<SpriteRenderer>();
		pelletburst = Random.Range(1, 3);
		r.color = planet.planetInfo.spriteInfo.baseColor;
		r = GetComponentInChildren<SpriteRenderer>();
		r.color = planet.planetInfo.spriteInfo.secondaryColor;
		transform.position = pos;
		transform.up = upVec;
		transform.localScale = scale;
	}


	IEnumerator FireBurst() {
		firing = true;
		int numShot = 0;
		while (numShot < pelletburst) {
			Pellet p = pooler.GetFromPool();
			SoundManager.instance.PlaySound3D("shootsound", barrelEnd.position);
			SpriteManager.instance.UseParticles(barrelEnd.position,Vector3.zero, "NativeShoot");
			p.Shoot(barrelEnd.position, barrelEnd.up * pelletSpeed, barrelEnd.up);
			numShot++;
			yield return new WaitForSeconds(fireCoolDown);
		}
		firing = false;
	}

}
