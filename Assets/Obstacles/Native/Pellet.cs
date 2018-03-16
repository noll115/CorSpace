using UnityEngine;
using System.Collections;

public class Pellet : MonoBehaviour {

	[SerializeField]float lifeTime = 4f;
	WaitForSeconds s;
	Rigidbody2D rigid;
	TrailRenderer trailRenderer;
	Vector2 pos;
	Vector2 vel;
	Coroutine decay;

	private void Awake() {
		rigid = GetComponent<Rigidbody2D>();
		trailRenderer = GetComponent<TrailRenderer>();
		trailRenderer.enabled = false;
		s = new WaitForSeconds(lifeTime);
	}

    public void Shoot(Vector3 pos,Vector2 vel,Vector2 up){
		trailRenderer.enabled = true;
		transform.up = up;
		this.pos = pos;
		this.vel = vel;
		if (!gameObject.activeSelf) {
			gameObject.SetActive(true);
		}
        else{
			OnEnable();
		}
		if (decay == null) {
			decay = StartCoroutine(lifeTimeCo());
		}
		else{
			StopCoroutine(decay);
			decay = StartCoroutine(lifeTimeCo());
		}
	}

	IEnumerator lifeTimeCo(){
		yield return s;
		gameObject.SetActive(false);
	}



    private void OnEnable() {
		rigid.position = pos;
		rigid.velocity = vel;
	}

    private void OnCollisionEnter2D(Collision2D other) {
		if (other.collider.CompareTag("Player")) {
			other.collider.GetComponentInParent<RocketController>().pResources.HealthChange(-10);
			SpriteManager.instance.UseExplosion(transform.position);
			trailRenderer.enabled = false;
		}
		gameObject.SetActive(false);
	}
}