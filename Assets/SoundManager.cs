﻿using System;
using UnityEngine;

public class SoundManager : MonoBehaviour {

	public static SoundManager instance;

	public Sound[] sounds;


	private void Awake() {
		if(instance!= null) {
			Destroy(gameObject);
			return;
		}
		instance = this;
		for(int i = 0;i < sounds.Length;i++) {
			sounds[i].source = gameObject.AddComponent<AudioSource>();
			sounds[i].source.clip = sounds[i].audio;
			sounds[i].source.loop = sounds[i].loop;
			sounds[i].source.volume = sounds[i].volume;
			sounds[i].source.pitch = sounds[i].pitch;
			sounds[i].source.spatialBlend = sounds[i].spatialBlend;
		}
	}

	public void PlaySound2D(string name) {
		Sound s = Array.Find(sounds, sound => sound.SoundName == name);
		if(s!= null) {
			s.source.Play();
		}
		else {
			Debug.LogWarning("There is no sound named " + name);
		}
	}

	public void PlaySound3D(string name, Vector3 pos) {
		Sound s = Array.Find(sounds, sound => sound.SoundName == name);
		if(s != null) {
			s.source.Play();
		}
		else {
			Debug.LogWarning("There is no sound named " + name);
		}
		transform.position = pos;
	}

	public void StopSound(string name) {
		Sound s = Array.Find(sounds, sound => sound.SoundName == name);
		if(s != null) {
			s.source.Stop();
		}
		else {
			Debug.LogWarning("There is no sound named " + name);
		}
	}

	public void StopAllSounds() {
		for(int i = 0;i < sounds.Length;i++) {
			sounds[i].source.Stop();
		}
	}




}
[System.Serializable]
public class Sound {

	public string SoundName;
	public AudioClip audio;
	public bool loop;

	[Range(0f, 1f)]
	public float volume;

	[Range(.1f, 3f)]
	public float pitch;

	[HideInInspector]
	public AudioSource source;
	[Range(0, 1)]
	public float spatialBlend;

}
