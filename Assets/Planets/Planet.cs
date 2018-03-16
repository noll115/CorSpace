using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D), typeof(PointEffector2D), typeof(CircleCollider2D))]
[SelectionBase]
public class Planet : MonoBehaviour {

	static GameObject parent;
	public SpriteRenderer planetSprite;
	public PlanetInfo planetInfo;
	protected RocketController con;
	public LayerMask mask;
	public bool playerOnPlanet;
	public bool playerAction;
	[HideInInspector]public Rigidbody2D rigid;
	[HideInInspector]public CircleCollider2D trig;
	public GameObject Asteroid;
	public Planets planetType;
	[Range(0, 10)] public int numOfAsteroids;
	Transform[] asteroids;
	public float rot;
	public float rotSpeed;
	protected float playerInitPos;
	[HideInInspector]public List<Transform> obsOnPlanet;
	ContactPoint2D[] contactPoints = new ContactPoint2D[1];
	bool madeJoints = false;

	protected virtual void Awake() {
		if (!parent) {
			parent = new GameObject("Planets");
		}
		transform.SetParent(parent.transform, true);
		rigid = GetComponent<Rigidbody2D>();
		trig = GetComponent<CircleCollider2D>();
	}


	protected virtual void FixedUpdate() {
		rot += rotSpeed * Time.deltaTime;
		rot %= 360;
		rigid.rotation = rot;
		for (int i = 0; i < obsOnPlanet.Count; i++) {
			obsOnPlanet[i].transform.RotateAround(transform.position, Vector3.forward, rotSpeed * Time.deltaTime);
		}
	}

	public virtual void SetUpPlanet(PlanetInfo p) {
		planetInfo = p;
		transform.SetPositionAndRotation(p.position, Quaternion.identity);
		transform.localScale = p.size;
		rotSpeed = p.rotSpeed;
		planetType = p.planetType;
		rot = p.startRot;
		numOfAsteroids = p.numOfAsteroids;
		asteroids = new Transform[p.numOfAsteroids];
		if (numOfAsteroids > 0) {
			SpawnAstroids(numOfAsteroids, asteroids);
		}
	}



	private void SpawnAstroids(int numOfAstroids, Transform[] asteroids) {
		float asteroidAngle = (Mathf.PI * 2) / numOfAstroids;
		Vector3 asteroidLoc;
		float asteroidRange;
		float rndOffset;
		Asteroid asteroid;
		float scale;
		int asteroidsNum = 0;
		for (float angle = 0; angle < (Mathf.PI * 2); angle += asteroidAngle) {
			rndOffset = Random.Range(-asteroidAngle / 2, asteroidAngle / 2);
			asteroidRange = Random.Range((trig.radius * transform.localScale.x)-2f, (trig.radius) * transform.localScale.x);
			asteroidLoc = new Vector2(Mathf.Cos(angle + rndOffset), Mathf.Sin(angle + rndOffset)).normalized;
			asteroidLoc *= asteroidRange;
			asteroid = Instantiate(Asteroid).GetComponent<Asteroid>();
			scale = Random.Range(0.4f, 1f);
			asteroid.Setup(this, transform.position + asteroidLoc, angle, scale);
			asteroids[asteroidsNum] = asteroid.transform;
			asteroidsNum++;
		}
	}


	protected virtual void OnTriggerStay2D(Collider2D collision) {
		if (con) {
			Transform player = con.transform;
			Vector3 dist = (transform.position - player.position);
			RaycastHit2D hit = Physics2D.Raycast(player.position, dist.normalized, dist.sqrMagnitude, mask);
			//print(hit.collider);
			//Debug.DrawLine(player.position, player.position + dist.normalized * dist.sqrMagnitude);
			Debug.DrawRay(player.position, -player.up);
			Debug.DrawRay(hit.point, hit.normal);
			if (hit && !Input.GetButton("Thrust") && !Input.GetButton("Horizontal")) {
				RotatePlayer(hit, dist);
			}
		}
	}


	private void RotatePlayer(RaycastHit2D hit, Vector3 dist) {
		float angleDif = Vector2.SignedAngle(con.transform.up, hit.normal) ;
		float playerDist = 1 - dist.sqrMagnitude / playerInitPos;
		//print(playerDist);
		con.rigid.rotation = Mathf.Lerp(con.rigid.rotation, (angleDif*Time.deltaTime) + con.rigid.rotation, playerDist );
		if (playerOnPlanet && Mathf.Abs(angleDif) < 2 && con.rigid.velocity.sqrMagnitude < 4 && !Input.GetButton("Thrust")) {
			//RotateWithPlanet(con);
			if (!madeJoints) {
				con.MakeJoints(rigid);
				madeJoints = !madeJoints;
			}
			con.canRotate = false;
			playerAction = true;
			PlayerAction(con.pResources);
		}
		else {
			
			madeJoints = false;
			con.canRotate = true;
			playerAction = false;
		}
	}

	protected virtual void OnTriggerEnter2D(Collider2D collision) {
		if (collision.CompareTag("Player")) {
			con = collision.GetComponentInParent<RocketController>();
			playerInitPos = (transform.position - con.transform.position).sqrMagnitude;
			con.PlanetOn = this;
		}
	}

	protected virtual void OnTriggerExit2D(Collider2D collision) {
		if (collision.CompareTag("Player")) {
			con.PlanetOn = null;
			con = null;
		}
	}

	protected virtual void OnCollisionEnter2D(Collision2D collision) {
		if (collision.collider.CompareTag("Player") && con) {
			collision.GetContacts(contactPoints);
			playerOnPlanet = true;
			SoundManager.instance.PlaySound3D("landsound", contactPoints[0].point);
			//print("colliding");
		}
	}

	protected virtual void OnCollisionStay2D(Collision2D collision) {
		if (Input.GetButton("Thrust")) {
			if (madeJoints) {
				con.DisableJoints();
			}
		}
	}



	protected virtual void OnCollisionExit2D(Collision2D collision) {
		if (collision.collider.CompareTag("Player")) {
			playerOnPlanet = false;
			if (con) {
				con.canRotate = true;
			}
		}
	}

	public virtual void AstroidHit(){
		
	}

	public virtual void PlayerAction(PlayerResources presource) {
		print("DO SOMETHING");
	}

	private void OnDisable() {
		for (int i = 0; i < asteroids.Length; i++) {
			if (asteroids[i])
				asteroids[i].gameObject.SetActive(false);
		}
		for (int i = 0; i < obsOnPlanet.Count; i++) {
			if (obsOnPlanet[i])
				obsOnPlanet[i].gameObject.SetActive(false);
		}
	}

	private void OnEnable() {
		if (asteroids != null) {
			for (int i = 0; i < asteroids.Length; i++) {
				if (asteroids[i])
					asteroids[i].gameObject.SetActive(true);
			}
		}
		if (obsOnPlanet != null) {
			for (int i = 0; i < obsOnPlanet.Count; i++) {
				if (obsOnPlanet[i])
					obsOnPlanet[i].gameObject.SetActive(true);
			}
		}
	}

	public void RemoveAstroid(Asteroid asteroid) {
		for (int i = 0; i < asteroids.Length; i++) {
			if (asteroid == asteroids[i]) {
				asteroids[i] = null;
				return;
			}
		}
	}


}
