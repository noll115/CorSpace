using System.Collections;
using UnityEngine;
using System;
public class RocketController : MonoBehaviour {

	public float impactLimit = 1f;
	public float damageConst = 0.8f;
	public float thrustUse = 4f;
	[HideInInspector] public Rigidbody2D rigid;
	public float turnSpeed;
	public float thrust;
	public bool queThrust;
	public bool queRot;
	public bool canRotate = true;
	public float velocityLimit = 12f;
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
	PlanetRadar planetRadar;
	public ParticleSystem thrustParticle;
	public ParticleSystem landParticle;
	[SerializeField] Vector2Int currentCell;
	public Action OnPlayerChangePlanet;
	public float rayX;
	public float rayY;
	public LayerMask planetLayer;
	RaycastHit2D[] hits = new RaycastHit2D[2];
	DistanceJoint2D[] joints;
	ContactPoint2D[] contact = new ContactPoint2D[1];
	Coroutine deathCoroutine;

	private void Awake() {
		thrustParticle.Stop();
		pResources.OnPlayerDeath += DisablePlayer;
		rigid = GetComponent<Rigidbody2D>();
		planetRadar = GetComponentInChildren<PlanetRadar>();
		joints = GetComponents<DistanceJoint2D>();
		joints[0].anchor = transform.InverseTransformPoint(transform.position + ((transform.right) * rayX) + (((transform.up)) * rayY));
		joints[1].anchor = transform.InverseTransformPoint(transform.position + ((transform.right) * -rayX) + (((transform.up)) * rayY));
		pResources.Setup();
	}

	private void OnDrawGizmos() {
		Vector3 ray1Pos = new Vector3();
		ray1Pos = transform.position + ((transform.right) * rayX) + ((transform.up)) * rayY;
		Vector3 ray2Pos = new Vector3();
		ray2Pos = transform.position - ((transform.right) * rayX) + ((transform.up)) * rayY;
		Gizmos.DrawRay(ray1Pos, -transform.up);
		Gizmos.DrawRay(ray2Pos, -transform.up);
	}


	private void Update() {

		if (Input.GetButton("Thrust")) {
			queThrust = true;
		}
		if (Input.GetButtonDown("Thrust") && pResources.HasFuel) {
			thrustParticle.Play();
			SoundManager.instance.PlaySound2D("rocketthrust");
		}
		if (Input.GetButtonUp("Thrust") || !pResources.HasFuel) {
			thrustParticle.Stop();
			SoundManager.instance.StopSound("rocketthrust");
		}
		if (Input.GetButton("Horizontal")) {
			rigid.angularVelocity = 0;
			queRot = true;
		}
		if (!pResources.HasFuel && deathCoroutine == null) {
			deathCoroutine = StartCoroutine(Countdown());
		}
	}

	IEnumerator Countdown() {
		yield return new WaitForSeconds(4f);
		pResources.HealthChange(-pResources.maxPlayerHealth);
	}


	private void FixedUpdate() {
		//print(velocity.sqrMagnitude);
		if (queThrust && pResources.HasFuel) {
			pResources.SubFuel(thrustUse * Time.fixedDeltaTime);
			rigid.AddForce(transform.up * thrust, ForceMode2D.Impulse);
			rigid.velocity = Vector2.ClampMagnitude(rigid.velocity, velocityLimit);
			queThrust = false;
		}
		if (canRotate && queRot) {
			float angle = 0;
			angle -= Input.GetAxis("Horizontal") * turnSpeed;
			rigid.MoveRotation(rigid.rotation + angle);
			queRot = false;
		}
	}

	private void OnCollisionEnter2D(Collision2D collision) {

		float hitAmount = collision.relativeVelocity.sqrMagnitude;
		if (!pResources.dead && !collision.collider.CompareTag("Pellet") && hitAmount > (impactLimit * impactLimit)) {
			if (collision.collider.CompareTag("Planet")) {
				collision.GetContacts(contact);
				SpriteManager.instance.UseExplosion(contact[0].point);
				SoundManager.instance.PlaySound3D("explosion" + UnityEngine.Random.Range(1, 2),transform.position);
			}
			CamController.instance.ShakeCam(3, 0.2f);
			hitAmount = Mathf.Sqrt(hitAmount) * damageConst;
			pResources.HealthChange(-hitAmount * 10);
		}
	}


	public void MakeJoints(Rigidbody2D rigid) {
		joints[0].enabled = true;
		joints[1].enabled = true;
		Vector2 ray1 = transform.position + ((transform.right) * rayX) + ((transform.up)) * rayY;
		Vector2 ray2 = transform.position - ((transform.right) * rayX) + ((transform.up)) * rayY;
		hits[0] = Physics2D.Raycast(ray1, -transform.up, 1f, planetLayer);
		hits[1] = Physics2D.Raycast(ray2, -transform.up, 1f, planetLayer);
		joints[0].connectedBody = rigid;
		joints[1].connectedBody = rigid;
		joints[0].connectedAnchor = rigid.transform.InverseTransformPoint(hits[0].point);
		joints[1].connectedAnchor = rigid.transform.InverseTransformPoint(hits[1].point);
		landParticle.Play();
		CamController.instance.ShakeCam(0.3f, 0.2f);
	}

	public void DisableJoints() {
		joints[0].enabled = false;
		joints[1].enabled = false;
		joints[0].connectedBody = null;
		joints[1].connectedBody = null;
		pResources.StopGiving();
		pResources.StopTaking();
	}

	public void SetCell(int row, int col) {
		//print(row + "" + col);
		currentCell.x = row;
		currentCell.y = col;
		planetRadar.NewCell();
	}

	public void DisablePlayer() {
		SpriteManager.instance.UseExplosion(transform.position, 9);
		rigid.simulated = false;
		this.enabled = false;
		this.GetComponentInChildren<SpriteRenderer>().sprite = null;
		pResources.ps.Stop();
		pResources.StopGiving();
		pResources.StopTaking();
		thrustParticle.Stop();
	}
}
