using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerResources : MonoBehaviour {

	public float playerHealth = 10;
    [SerializeField]private float maxPlayerHealth = 10;
    [SerializeField] private float fuel = 100;
	[SerializeField] private float water = 0;
	[SerializeField] private float lava = 0;
	[SerializeField] private float ores = 0;
	 public float maxFuel = 100f;
	 public float maxLava = 100f;
	 public float maxWater = 100f;
	 public float maxOres = 100f;
	public bool invincible;
	public bool HasFuel {
		get {
			UpdateFuel(fuel);
			return fuel > 0;
		}
	}
	public bool HasWater {
		get {
			OnResourceChange(this);
			return Water > 0; }
	}
	public bool HasLava {
		get {
			OnResourceChange(this);
			return Lava > 0; }
	}
	public bool HasOres {
		get {
			OnResourceChange(this);
			return Ores > 0; }
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
	public bool MaxedOres {
		get {
			return Ores >= maxOres;
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
	public float Ores {
		get {
			return ores;
		}

		set {
			ores = value;
			ores = Mathf.Clamp(ores, 0, maxOres);
			OnResourceChange(this);
		}
	}
	public ParticleSystem ps;
	public Image healthbar;

	public delegate void UseFuel(float fuel);
	public event UseFuel UpdateFuel;

	public delegate void PlayerDied();
	public event PlayerDied OnPlayerDeath;

	public delegate void ChangeResources(PlayerResources r);
	public event ChangeResources OnResourceChange;


	public void HealthChange(float hitAmount) {
		playerHealth += hitAmount;
        playerHealth = Mathf.Clamp(playerHealth, 0, maxPlayerHealth);
        var em = ps.emission;
		em.rateOverTime = (1- (playerHealth/10 )) * 10;
		healthbar.transform.localScale = new Vector3(playerHealth / 10, 1, 1);

		if(playerHealth <= 0 && !invincible) {
			SpriteManager.instance.UseExplosion(transform.position, 9);
			OnPlayerDeath();
		}
	}

	public void SubFuel(float amount) {
		fuel -= amount;
		fuel = Mathf.Clamp(fuel, 0, 100);
	}



}
