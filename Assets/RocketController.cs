using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketController : MonoBehaviour {

	public float impactLimit = 1f;
	public float damageConst = 0.8f;
	public float thrustUse = 4f;
	public Rigidbody2D rigid;
	public Collider2D col;
	public float turnSpeed;
	public float thrust;
	public bool queThrust;
	public bool queRot;
	public bool canRotate = true;
	public float velocityLimit = 12f;
	public Vector2 velocity;
	Planet planetOn;
	public Planet PlanetOn {
		set {
			planetOn = value;
			OnPlayerChangePlanet();
		}
		get {
			return planetOn;
		}
	}
	public PlayerResources pResources;
	public ParticleSystem thrustParticle;
	[SerializeField]Vector2Int currentCell;
	public CanvasBarren bCanvas;
	public ResourceText rCanvas;
	public delegate void EnterPlanet();
	public event EnterPlanet OnPlayerChangePlanet;

	private void Awake() {
		thrustParticle.Stop();
		rigid = GetComponent<Rigidbody2D>();
		pResources = GetComponent<PlayerResources>();
	}


	private void Update() {
		if(Input.GetButton("Thrust")) {
			queThrust = true;
		}
		if(Input.GetButtonDown("Thrust") && pResources.HasFuel) {
			thrustParticle.Play();
			SoundManager.instance.PlaySound2D("rocketthrust");
		}
		if(Input.GetButtonUp("Thrust") || !pResources.HasFuel) {
			thrustParticle.Stop();
			SoundManager.instance.StopSound("rocketthrust");
		}
		if(Input.GetButton("Horizontal")) {
			queRot = true;
		}
		if(!pResources.HasFuel) {
			StartCoroutine(Countdown());
		}
	}

	IEnumerator Countdown() {
		yield return new WaitForSeconds(4f);
		pResources.PlayerHit(10f);
	}

	private void FixedUpdate() {
		velocity = rigid.velocity;
		//print(velocity.sqrMagnitude);
		if(queThrust && pResources.HasFuel) {
			pResources.SubFuel(thrustUse * Time.fixedDeltaTime);
			rigid.AddForce(transform.up * thrust, ForceMode2D.Force);
			rigid.velocity = Vector2.ClampMagnitude(rigid.velocity, velocityLimit);
			queThrust = false;
			if(PlanetOn) {
			}
		}
		if(canRotate && queRot) {
			float angle = 0;
			angle -= Input.GetAxis("Horizontal") * turnSpeed;
			rigid.MoveRotation(rigid.rotation + angle);
			queRot = false;
		}
	}

	private void OnCollisionEnter2D(Collision2D collision) {
		float hitAmount = collision.relativeVelocity.sqrMagnitude;
		if(hitAmount > (impactLimit * impactLimit)) {
			if(collision.contacts.Length>0) {
				SpriteManager.instance.UseExplosion(collision.contacts[0].point);
			}
			hitAmount = hitAmount * damageConst;
			//print("hit " + hitAmount);
			pResources.PlayerHit(hitAmount);
		}
	}

	public void InstaDeath() {
		pResources.PlayerHit(100f);
	}

	public void SetCell(int row,int col) {
		//print(row + "" + col);
		currentCell.x = row;
		currentCell.y = col;
		rCanvas.ChangeCell(SpaceGenerator.cells[currentCell.x, currentCell.y]);
		bCanvas.ChangeCell(SpaceGenerator.cells[currentCell.x, currentCell.y]);
	}
}
