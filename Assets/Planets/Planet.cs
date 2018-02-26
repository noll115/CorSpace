using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D), typeof(PointEffector2D), typeof(CircleCollider2D))]
[SelectionBase]
public class Planet : MonoBehaviour {

	static GameObject parent;
	public SpriteRenderer planetSprite;
	public float planetRad = 2.95f;
	public PlanetInfo planetInfo;
	public RocketController con;
	public LayerMask mask;
	public bool playerOnPlanet;
	public bool playerAction;
	public PointEffector2D pEffector;
	public Rigidbody2D rigid;
	public CircleCollider2D trig;
	public GameObject Asteroid;
	public Planets planetType;
	[Range(0, 10)] public int numOfAsteroids;
	Asteroid[] asteroids;
	public float rot;
	public float rotSpeed;
	public Vector3 initPos;
	public List<Transform> obsOnPlanet;
	bool madeJoints = false;

	private void Awake() {
		if(!parent) {
			parent = new GameObject("Planets");
		}
		transform.SetParent(parent.transform, true);
		pEffector = GetComponent<PointEffector2D>();
		rigid = GetComponent<Rigidbody2D>();
		trig = GetComponent<CircleCollider2D>();
		planetRad = planetRad * transform.localScale.x;
	}


	private void FixedUpdate() {
		rot += rotSpeed * Time.deltaTime;
		rot %= 360;
		rigid.rotation = rot;
		for (int i = 0; i < obsOnPlanet.Count; i++)
		{
			obsOnPlanet[i].transform.position = transform.position + new Vector3(Mathf.Cos(rot*Mathf.Deg2Rad)*planetRad, Mathf.Sin(rot*Mathf.Deg2Rad)*planetRad);
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
		asteroids = new Asteroid[p.numOfAsteroids];
		if(numOfAsteroids > 0) {
			SpawnAstroids(numOfAsteroids, asteroids);
		}
	}



	private void SpawnAstroids(int numOfAstroids, Asteroid[] asteroids) {
		float asteroidAngle = (Mathf.PI * 2) / numOfAstroids;
		Vector3 asteroidLoc;
		float asteroidRange;
		float rndOffset;
		Asteroid asteroid;
		float scale;
		int asteroidsNum = 0;
		for(float angle = 0;angle < (Mathf.PI * 2);angle += asteroidAngle) {
			rndOffset = Random.Range(-asteroidAngle / 2, asteroidAngle / 2);
			asteroidRange = Random.Range(trig.radius * transform.localScale.x, (trig.radius + 2f) * transform.localScale.x);
			asteroidLoc = new Vector2(Mathf.Cos(angle + rndOffset), Mathf.Sin(angle + rndOffset)).normalized;
			asteroidLoc *= asteroidRange;
			asteroid = Instantiate(Asteroid, transform.position + asteroidLoc, Quaternion.identity).GetComponent<Asteroid>();
			scale = Random.Range(0.4f, 1f);
			asteroid.SetUp(this, angle, scale);
			asteroids[asteroidsNum] = asteroid;
			asteroidsNum++;
		}
	}


	protected virtual void OnTriggerStay2D(Collider2D collision) {
		if(con) {
			Transform player = con.transform;
			Vector3 dist = (transform.position - player.position);
			RaycastHit2D hit = Physics2D.Raycast(player.position, dist.normalized, dist.sqrMagnitude, mask);
			//print(hit.collider);
			//Debug.DrawLine(player.position, player.position + dist.normalized * dist.sqrMagnitude);
			Debug.DrawRay(player.position, -player.up);
			Debug.DrawRay(hit.point, hit.normal);
			if(hit && !Input.GetButton("Thrust")) {
				RotatePlayer(hit, dist);
			}
		}
	}


	private void RotatePlayer(RaycastHit2D hit, Vector3 dist) {
		float angleDif = Vector2.SignedAngle(con.transform.up, hit.normal);
		float playerDist = 1 - dist.sqrMagnitude / initPos.sqrMagnitude;
		con.rigid.rotation = Mathf.Lerp(con.rigid.rotation, angleDif + con.rigid.rotation, playerDist / 10);
		if(playerOnPlanet && Mathf.Abs(angleDif) < 2 && con.velocity.sqrMagnitude < 4 && !Input.GetButton("Thrust")) {
			//RotateWithPlanet(con);
			if(!madeJoints) {
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
		if(collision.CompareTag("Player")) {
			con = collision.GetComponentInParent<RocketController>();
			initPos = (transform.position - con.transform.position);
			con.PlanetOn = this;
		}
		if(collision.CompareTag("Asteroid")) {
			GetComponentInParent<Asteroid>().hit = true;
		}
	}

	private void OnTriggerExit2D(Collider2D collision) {
		if(collision.CompareTag("Player")) {
			con.PlanetOn = null;
			con = null;
		}
	}

	protected virtual void OnCollisionEnter2D(Collision2D collision) {
		if(collision.collider.CompareTag("Player") && con) {
			playerOnPlanet = true;
			SoundManager.instance.PlaySound3D("landsound", collision.contacts[0].point);
			//print("colliding");
		}
	}

	private void OnCollisionStay2D(Collision2D collision) {
		if(Input.GetButton("Thrust")) {
			if(madeJoints) {
				con.DisableJoints();
			}
		}
	}



	private void OnCollisionExit2D(Collision2D collision) {
		if(collision.collider.CompareTag("Player")) {
			playerOnPlanet = false;
			if(con) {
				con.canRotate = true;
			}
		}
	}

	public virtual void PlayerAction(PlayerResources presource) {
		print("DO SOMETHING");
	}

	private void OnDisable() {
		for(int i = 0;i < asteroids.Length;i++) {
			if(asteroids[i]) {
				asteroids[i].gameObject.SetActive(false);
			}
		}
		for (int i = 0; i < obsOnPlanet.Count; i++) {
			obsOnPlanet[i].gameObject.SetActive(false);
		}
	}

	private void OnEnable() {
		if(asteroids != null) {
			for(int i = 0;i < asteroids.Length;i++) {
				asteroids[i].gameObject.SetActive(true);
			}
		}
		if (obsOnPlanet != null) {
			for (int i = 0; i < obsOnPlanet.Count; i++) {
				obsOnPlanet[i].gameObject.SetActive(true);
			}
		}
	}


}
