using UnityEngine;

public class Mountain : MonoBehaviour, IObstacle {

	Planet planetOn;
	public Planet ParentPlanet {
		get {
			return planetOn;
		}
	}

	public void OnCollisionEnter2D(Collision2D collision) {
		if(collision.contacts.Length > 0) {
			if(collision.collider.CompareTag("Player") ) {
				collision.collider.GetComponentInParent<RocketController>().pResources.HealthChange(-10f);
				AddExplosionForce(collision.rigidbody, 10, collision.contacts[0].point, 4);
				SpriteManager.instance.UseExplosion(collision.contacts[0].point,5);
			}
		}
	}

	public void Setup(Planet planet,Vector3 pos, Vector3 upVec,Vector3 scale) {
		planetOn = planet;
		GetComponent<SpriteRenderer>().color = planet.planetInfo.spriteInfo.baseColor;
        transform.position = pos;
		transform.up = upVec;
		transform.localScale = scale;
	}

    public void AddExplosionForce(Rigidbody2D body, float explosionForce, Vector3 explosionPosition, float explosionRadius) {
		Vector3 dir = (body.transform.position - explosionPosition);
		float wearoff = 1 - (dir.magnitude / explosionRadius);
		wearoff = Mathf.Clamp(wearoff, 0f, 1f);
		body.AddForce(dir.normalized * explosionForce * wearoff, ForceMode2D.Impulse);
	}
}