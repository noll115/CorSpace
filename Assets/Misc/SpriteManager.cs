using System.Collections.Generic;
using UnityEngine;
using System;
public class SpriteManager : MonoBehaviour {

	public static SpriteManager instance;
	[SerializeField]
	string[] explosions;
	[SerializeField] Particle[] spawnableParticles;
	public int numOfInstances;
	public GameObject animObj;
	Queue<Animator> animators;


	private void Awake() {
		if (instance != null) {
			Destroy(gameObject);
			return;
		}
		instance = this;
		animators = new Queue<Animator>();
		for (int i = 0; i < numOfInstances; i++) {
			Animator anim = Instantiate(animObj).GetComponent<Animator>();
			anim.gameObject.SetActive(false);
			animators.Enqueue(anim);
		}
		for (int i = 0; i < spawnableParticles.Length; i++)
		{
			spawnableParticles[i].pSystem = Instantiate(spawnableParticles[i].go).GetComponent<ParticleSystem>();
			spawnableParticles[i].pSystem.gameObject.SetActive(false);
		}

	}

	public void UseParticles(Vector2 pos, string tag, PlanetInfo pi = null) {
		Particle ps = Array.Find(spawnableParticles, p => p.particleTag == tag);
		if (ps !=null && !pi) {
			ps.pSystem.transform.position = pos;
			ps.pSystem.gameObject.SetActive(true);
			ps.pSystem.Play();
		}
		else if(ps!=null){
			var m = ps.pSystem.main;
			m.startColor = pi.spriteInfo.baseColor;
			ps.pSystem.transform.position = pos;
			ps.pSystem.transform.localScale = pi.size;
			ps.pSystem.gameObject.SetActive(true);
			ps.pSystem.Play();
		}
		else {
			Debug.LogWarning("There is no particle with tag: " + tag);
		}
	}


	public void UseExplosion(Vector3 pos, int scale = -1) {
		Animator anim = animators.Dequeue();
		int rnd = UnityEngine.Random.Range(0, explosions.Length);
		int exp = UnityEngine.Random.Range(1, 3);
		SoundManager.instance.PlaySound3D("explosion" + exp, pos);
		anim.transform.position = pos;
		if (scale != -1) {
			anim.transform.localScale = new Vector3(scale, scale);
		}
		anim.transform.Rotate(Vector3.forward, UnityEngine.Random.Range(0, 360));
		anim.gameObject.SetActive(true);
		anim.Play(explosions[rnd]);
		animators.Enqueue(anim);

	}

}
[System.Serializable]
public class Particle {
	public string particleTag;
	public GameObject go;
	[HideInInspector]public ParticleSystem pSystem;
}
