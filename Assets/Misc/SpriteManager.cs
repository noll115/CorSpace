using System.Collections.Generic;
using System;
using UnityEngine;


public class SpriteManager : MonoBehaviour {

	public static SpriteManager instance;
	[SerializeField] SpawnableParticles[] spawnableParticles;
	Pooler<ParticleSystem>[] particleSystems;
	String explosion = "Explosion";


	private void Awake() {
		if (instance != null) {
			Destroy(gameObject);
			return;
		}
		instance = this;
		particleSystems = new Pooler<ParticleSystem>[spawnableParticles.Length];
		for (int i = 0; i < spawnableParticles.Length; i++) {
			GameObject particleGO = spawnableParticles[i].particleGO;
			int numOfInst = spawnableParticles[i].NumOfInstances;
			particleSystems[i] = new Pooler<ParticleSystem>(particleGO, numOfInst);
		}

	}

	public void UseParticles(Vector2 pos, Vector3 upDir, string tag, PlanetInfo pi = null) {
		int index = FindIndex(tag);
		ParticleSystem ps;
		try
		{
			ps = particleSystems[index].GetFromPool();
		}
		catch (System.IndexOutOfRangeException)
		{
			Debug.LogWarning("No particle of " + tag);
			throw;
		}
		
		if (ps != null && !pi) {//regular
			ps.transform.position = pos;
			ps.transform.up = upDir;
			ps.gameObject.SetActive(true);
			ps.Play();
		}
		else if (ps != null) {//planets
			var m = ps.main;
			//print(pi.spriteInfo.baseColor);
			m.startColor = pi.spriteInfo.baseColor;
			ps.transform.position = pos;
			ps.transform.localScale = pi.size;
			ps.gameObject.SetActive(true);
			ps.Play();
		}
		else {
			Debug.LogWarning("There is no particle with tag: " + tag);
		}
	}

	private int FindIndex(string tag) {
		for (int i = 0; i < spawnableParticles.Length; i++) {
			if (spawnableParticles[i].tag == tag) {
				return i;
			}
		}
		return -1;
	}

	public void UseExplosion(Vector3 pos, int scale = -1) {
		int index = FindIndex(explosion);
		ParticleSystem ps = particleSystems[index].GetFromPool();
		int exp = UnityEngine.Random.Range(1, 3);
		ps.transform.position = pos;
		if (scale == -1) {
			scale = UnityEngine.Random.Range(3, 6);
		}
		ps.transform.localScale = new Vector3(scale, scale, scale);
		ps.gameObject.SetActive(true);
		ps.Play();
		SoundManager.instance.PlaySound3D("explosion" + exp, pos);

	}

}
[System.Serializable]
public class SpawnableParticles{
	public String tag;
	public GameObject particleGO;
	public int NumOfInstances;
}
