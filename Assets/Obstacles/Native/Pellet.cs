using UnityEngine;
using System.Collections;

public class Pellet : MonoBehaviour {
	Rigidbody2D rigid;
	Vector2 pos;
	Vector2 vel;

	private void Awake() {
		rigid = GetComponentInParent<Rigidbody2D>();
	}

    public void Shoot(Vector3 pos,Vector2 vel){
		this.pos = pos;
		this.vel = vel;
		if (!gameObject.activeSelf) {
			gameObject.SetActive(true);
		}
        else{
			OnEnable();
		}

	}

    private void OnBecameInvisible() {
		gameObject.SetActive(false);
	}


    private void OnEnable() {
        rigid.position = pos;
		rigid.velocity = vel;
    }

    private void OnCollisionEnter2D(Collision2D other) {
		gameObject.SetActive(false);
	}
}