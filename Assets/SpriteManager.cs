using System.Collections.Generic;
using UnityEngine;

public class SpriteManager : MonoBehaviour {

	public static SpriteManager instance;
	[SerializeField]
	string[] explosions;
	public int numOfInstances;
	public GameObject animObj;
	Queue<Animator> animators;


	private void Awake() {
		if(instance != null) {
			Destroy(gameObject);
			return;
		}
		instance = this;
		animators = new Queue<Animator>();
		for(int i = 0;i < numOfInstances;i++) {
			Animator anim = Instantiate(animObj).GetComponent<Animator>();
			anim.gameObject.SetActive(false);
			animators.Enqueue(anim);
		}
	}

	public void UseExplosion(Vector3 pos, int scale = -1) {
		Animator anim = animators.Dequeue();
		int rnd = Random.Range(0, explosions.Length);
		int exp = Random.Range(1, 3);
		SoundManager.instance.PlaySound3D("explosion" + exp,pos);
		anim.transform.position = pos;
		if(scale != -1) {
			anim.transform.localScale = new Vector3(scale, scale);
		}
		anim.transform.Rotate(Vector3.forward, Random.Range(0, 360));
		anim.gameObject.SetActive(true);
		anim.Play(explosions[rnd]);
		animators.Enqueue(anim);

	}

}
