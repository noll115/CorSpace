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
	public PlanetCanvasController planetCanvas;
	public delegate void EnterPlanet();
	public event EnterPlanet OnPlayerChangePlanet;
	public float rayX;
	public float rayY;
	public LayerMask planetLayer;
	RaycastHit2D[] hits = new RaycastHit2D[2];
	DistanceJoint2D[] joints;

	private void OnDrawGizmos() {
		//joints = GetComponents<DistanceJoint2D>();
		Vector3 ray1Pos = new Vector3();
		ray1Pos = transform.position + ((transform.right) * rayX) + ((transform.up)) * rayY;
		Vector3 ray2Pos = new Vector3();
		ray2Pos = transform.position - ((transform.right) * rayX) + ((transform.up)) * rayY;
		Gizmos.DrawRay(ray1Pos, -transform.up);
		Gizmos.DrawRay(ray2Pos, -transform.up);
		//joints[0].anchor = transform.InverseTransformPoint(transform.position + ((transform.right) * rayX) + (((transform.up)) * rayY));
		//joints[1].anchor = transform.InverseTransformPoint(transform.position + ((transform.right) * -rayX) + (((transform.up)) * rayY));
	}

	private void Awake() {
		thrustParticle.Stop();
		rigid = GetComponent<Rigidbody2D>();
		pResources = GetComponent<PlayerResources>();
		joints = GetComponents<DistanceJoint2D>();
		joints[0].anchor = transform.InverseTransformPoint(transform.position + ((transform.right) * rayX) + (((transform.up)) * rayY));
		joints[1].anchor = transform.InverseTransformPoint(transform.position + ((transform.right) * -rayX) + (((transform.up)) * rayY));
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
			rigid.angularVelocity = 0;
			queRot = true;
		}
		if(!pResources.HasFuel) {
			StartCoroutine(Countdown());
		}
	}

	IEnumerator Countdown() {
		yield return new WaitForSeconds(4f);
		pResources.HealthChange(-10f);
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
			hitAmount = Mathf.Sqrt(hitAmount) * damageConst;
			//print("hit " + hitAmount);
			pResources.HealthChange(-hitAmount);
		}
	}

	public void InstaDeath() {
		pResources.HealthChange(-100f);
	}

	public void MakeJoints(Rigidbody2D rigid) {
		joints[0].enabled = true;
		joints[1].enabled = true;
		Vector2 ray1 = transform.position + ((transform.right) * rayX) + ((transform.up)) * rayY;
		Vector2 ray2 = transform.position - ((transform.right) * rayX) + ((transform.up)) * rayY;
		hits[0] = Physics2D.Raycast(ray1, -transform.up, 1f,planetLayer );
		hits[1] = Physics2D.Raycast(ray2, -transform.up, 1f, planetLayer);
		joints[0].connectedBody = rigid;
		joints[1].connectedBody = rigid;
		joints[0].connectedAnchor = rigid.transform.InverseTransformPoint( hits[0].point);
		joints[1].connectedAnchor = rigid.transform.InverseTransformPoint(hits[1].point);
	}

	public void DisableJoints() {
		joints[0].enabled = false;
		joints[1].enabled = false;
		joints[0].connectedBody = null;
		joints[1].connectedBody = null;
	}

	public void SetCell(int row,int col) {
		//print(row + "" + col);
		currentCell.x = row;
		currentCell.y = col;
		planetCanvas.ChangeCell(SpaceGenerator.cells[currentCell.x, currentCell.y]);
	}
}
