using UnityEngine.UI;
using UnityEngine;
using System;
[Serializable]
public class PlayerResources {

	public float playersCurrentHealth = 10;
	public float maxPlayerHealth = 10;
	[SerializeField] private float fuel = 100;
	[SerializeField] private float water = 0;
	[SerializeField] private float lava = 0;
	public float maxFuel = 100f;
	public float maxLava = 100f;
	public float maxWater = 100f;
	public bool invincible;
	public bool dead;
	public ParticleSystem givingParticle;
	public ParticleSystem takingParticle;
	public bool HasFuel {
		get {
			UpdateFuel(fuel);
			return fuel > 0;
		}
	}
	public bool HasWater {
		get {
			OnResourceChange(this);
			return Water > 0;
		}
	}
	public bool HasLava {
		get {
			OnResourceChange(this);
			return Lava > 0;
		}
	}
	public bool MaxedFuel {
		get {
			return fuel >= maxFuel;
		}
	}
	public bool MaxedWater {
		get { return Water >= maxWater; }
	}
	public bool MaxedLava {
		get { return Lava >= maxLava; }
	}
	public bool MaxedHealth {
		get {
			return playersCurrentHealth >= maxPlayerHealth;
		}
	}
	public float Fuel {
		get { return fuel; }
		set {
			fuel = value;
			fuel = Mathf.Clamp(fuel, 0, maxFuel);
			UpdateFuel(fuel);
		}
	}
	public float Water {
		get {
			return water;
		}

		set {
			water = value;
			water = Mathf.Clamp(water, 0, maxWater);
			OnResourceChange(this);
		}
	}
	public float Lava {
		get {
			return lava;
		}

		set {
			lava = value;
			lava = Mathf.Clamp(lava, 0, maxLava);
			OnResourceChange(this);
		}
	}
	public float Health {
		get {
			return playersCurrentHealth;
		}

		set {
			playersCurrentHealth = value;
			playersCurrentHealth = Mathf.Clamp(playersCurrentHealth, 0, maxPlayerHealth);
			HealthChange(0);
			OnResourceChange(this);
		}
	}

	public ParticleSystem ps;

	public delegate void UseFuel(float fuel);
	public event UseFuel UpdateFuel;

	public Action OnPlayerDeath;
	public Action<PlayerResources> OnResourceChange;
	public Action<float> OnHealthChange;
	ParticleSystem.EmissionModule em;

	public void Setup(){
		em = ps.emission;
		playersCurrentHealth = maxPlayerHealth;
	}

	public void HealthChange(float hitAmount) {
		playersCurrentHealth += hitAmount;
		playersCurrentHealth = Mathf.Clamp(playersCurrentHealth, 0, maxPlayerHealth);
		em.rateOverTime = (1 - (playersCurrentHealth / 10)) * 10;
		OnHealthChange(playersCurrentHealth);

		if (playersCurrentHealth <= 0 && !invincible) {
			dead = true;
			OnPlayerDeath();
		}
	}

	public void SubFuel(float amount) {
		fuel -= amount;
		fuel = Mathf.Clamp(fuel, 0, 100);
	}

	public void PlayTaking() {
		if (!takingParticle.isPlaying) {
			takingParticle.Play();
		}
	}

	public void PlayGiving() {
		if (!givingParticle.isPlaying) {
			givingParticle.Play();
		}
	}

	public void StopTaking() {
		if (takingParticle.isPlaying) {
			takingParticle.Stop();
		}
	}

	public void StopGiving() {
		if (givingParticle.isPlaying) {
			givingParticle.Stop();
		}
	}



}
