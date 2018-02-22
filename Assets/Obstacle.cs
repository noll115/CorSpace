using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour {

	
	public float explosionForce;
	public float explosionRadius;


	private void OnCollisionEnter2D(Collision2D collision) {
		if(collision.contacts.Length > 0) {
			if(collision.collider.CompareTag("Player") ) {
				collision.collider.GetComponentInParent<PlayerResources>().PlayerHit(1f);
				AddExplosionForce(collision.rigidbody, 10, collision.contacts[0].point, 4);
				SpriteManager.instance.UseExplosion(collision.contacts[0].point, 3);
			}
		}
	}








	public void AddExplosionForce(Rigidbody2D body, float explosionForce, Vector3 explosionPosition, float explosionRadius) {
		Vector3 dir = (body.transform.position - explosionPosition);
		float wearoff = 1 - (dir.magnitude / explosionRadius);
		wearoff = Mathf.Clamp(wearoff, 0f, 1f);
		body.AddForce(dir.normalized * explosionForce * wearoff, ForceMode2D.Impulse);
	}
}
