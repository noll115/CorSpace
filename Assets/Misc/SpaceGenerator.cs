using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpaceGenerator {

	public SpawnablePlanet[] spawnablePlanets;
	public static SpaceCell[,] cells;
	public const float planetRadius = 2.95f;
	[Tooltip("Only Odd Numbers")] public int NumOfCells;
	public int cellSize = 200;
	public int maxPlanetsSpawned;
	public GameObject barrenPlanet;
	public GameObject resourcePlanet;
	public GameObject blackHole;
	public GameObject sun;
	public LayerMask planetMask;
	public LayerMask playerMask;
	public GameObject[] Obstacles;
	private GameObject obstacleParent;
	private GameObject[] obsParents;


	public void Generate() {
		cells = new SpaceCell[NumOfCells, NumOfCells];
		GameObject Parent = new GameObject("Cells");
		int row = 0;
		for (int i = NumOfCells / 2; i >= -NumOfCells / 2; i--) {
			int col = 0;
			for (int j = -NumOfCells / 2; j <= NumOfCells / 2; j++) {
				List<Planet> planets = new List<Planet>();
				SpaceCell Cell = new GameObject("Cell " + row + ":" + col).AddComponent<SpaceCell>();
				Cell.transform.SetParent(Parent.transform, true);
				Vector3 cellPos = new Vector3(cellSize * j, cellSize * i);
				cells[row, col] = Cell;
				BoxCollider2D box = Cell.gameObject.AddComponent<BoxCollider2D>();
				box.isTrigger = true;
				box.size = new Vector2(cellSize, cellSize);
				GenerateBigPlanets(planets, cellPos);
				CreatePlanets(planets, cellPos);
				Cell.Setup(row, col, box, planets, cellPos);
				col++;
			}
			row++;
		}
	}

	private void GenerateBigPlanets(List<Planet> planets, Vector2 cellPos) {
		float numOfLargePlanets = Mathf.Round((float)cellSize / 100);
		int rndPlnt;
		Planet p;
		PlanetInfo pi;
		for (int i = 0; i < numOfLargePlanets; i++) {
			rndPlnt = Random.Range(0, 2);
			pi = ScriptableObject.CreateInstance<PlanetInfo>();
			float size = Random.Range(1f, 1.5f);
			pi.size = new Vector3(size, size, 1);
			pi.planetType = GeneratePlanetType();
			pi.startRot = Random.Range(0, 360f);
			if (rndPlnt == 0) {
				//spawn Sun
				pi.planetType = Planets.SUN;
				p = GameObject.Instantiate(sun).GetComponent<Planet>();
				planets.Add(p);
				pi.position = PosInSquare(cellPos, p.trig.bounds.extents.x);
				planets[planets.Count - 1].SetUpPlanet(pi);
			}
			else {
				//spawnBlackHole
				pi.planetType = Planets.BLACKHOLE;
				p = GameObject.Instantiate(blackHole).GetComponent<Planet>();
				planets.Add(p);
				pi.position = PosInSquare(cellPos, p.trig.bounds.extents.x);
				planets[planets.Count - 1].SetUpPlanet(pi);
			}
		}
	}



	public void CreatePlanets(List<Planet> planets, Vector2 cellPos) {
		PlanetInfo pi;
		Planet p;
		for (int i = 0; i < maxPlanetsSpawned; i++) {
			pi = GeneratePlanetInfo(cellPos);
			switch (pi.planetType) {
				case Planets.RESOURCE:
					p = GameObject.Instantiate(resourcePlanet).GetComponent<ResourcePlanet>();
					planets.Add(p);
					pi.position = PosInSquare(cellPos, p.trig.bounds.extents.x);
					planets[planets.Count - 1].SetUpPlanet(pi);
					p.planetSprite.sprite = RndPlanetSprite(pi, p);
					break;
				case Planets.BARREN:
					p = GameObject.Instantiate(barrenPlanet).GetComponent<BarrenPlanet>();
					planets.Add(p);
					pi.position = PosInSquare(cellPos, p.trig.bounds.extents.x);
					planets[planets.Count - 1].SetUpPlanet(pi);
					p.planetSprite.sprite = RndPlanetSprite(pi, p);
					break;
				case Planets.BLACKHOLE:
					p = GameObject.Instantiate(blackHole).GetComponent<Planet>();
					planets.Add(p);
					pi.position = PosInSquare(cellPos, p.trig.bounds.extents.x);
					planets[planets.Count - 1].SetUpPlanet(pi);
					break;
				case Planets.SUN:
					p = GameObject.Instantiate(sun).GetComponent<Planet>();
					planets.Add(p);
					pi.position = PosInSquare(cellPos, p.trig.bounds.extents.x);
					planets[planets.Count - 1].SetUpPlanet(pi);
					break;
				default:
					break;
			}
		}

	}


	public PlanetInfo GeneratePlanetInfo(Vector2 cell) {
		PlanetInfo pi = ScriptableObject.CreateInstance<PlanetInfo>();
		float size = Random.Range(1f, 1.5f);
		pi.size = new Vector3(size, size, 1);
		pi.planetType = GeneratePlanetType();
		pi.startRot = Random.Range(0, 360f);
		switch (pi.planetType) {
			case Planets.RESOURCE:
				pi.lavaAmount = Random.Range(100, 151);
				pi.maxLava = pi.lavaAmount;
				pi.waterAmount = Random.Range(100, 151);
				pi.maxWater = pi.waterAmount;
				pi.oresAmount = Random.Range(50, 70);
				pi.maxOres = pi.oresAmount;
				pi.fuelAmount = Random.Range(60, 100);
				pi.maxFuel = pi.fuelAmount;
				pi.numOfAsteroids = Random.Range(1, 5);
				break;
			case Planets.BARREN:
				pi.maxLava = 50;
				pi.maxWater = 50;
				pi.numOfAsteroids = Random.Range(0, 5);
				break;
			case Planets.BLACKHOLE:
			case Planets.SUN:
				break;
		}

		pi.rotSpeed = Random.Range(2f, 5f);
		return pi;
	}

	Vector2 PosInSquare(Vector2 cellPos, float trigRadius) {
		bool goodPos = false;
		Vector2 pos = Vector2.zero;
		while (!goodPos) {
			pos = cellPos;
			int xpos = Random.Range(-cellSize / 2 + (cellSize / 10), cellSize / 2 - (cellSize / 10));
			int ypos = Random.Range(-cellSize / 2 + (cellSize / 10), cellSize / 2 - (cellSize / 10));
			pos += new Vector2(xpos, ypos);
			if (!Physics2D.CircleCast(pos, trigRadius * 2, Vector2.zero, 0, planetMask) && !Physics2D.CircleCast(pos, trigRadius * 2, Vector2.zero, 0, playerMask)) {
				goodPos = true;
			}
		}
		return pos;
	}


	public Sprite RndPlanetSprite(PlanetInfo pi, Planet rp) {
		int rnd = Random.Range(0, spawnablePlanets.Length);
		SpawnablePlanet spawnable = spawnablePlanets[rnd];
		pi.spriteInfo = spawnable;
		rp.planetSprite.sprite = spawnable.Planet;
		GenerateObstacles(pi, rp, spawnable);
		return spawnable.Planet;

	}


	public void GenerateObstacles(PlanetInfo pi, Planet planet, SpawnablePlanet spawnablePlanet) {
		CreateObstacleParentObjs();
		float localplanetRad = planetRadius * planet.transform.localScale.x;
		int maxNumOfObs = Random.Range(2, 6);
		int numOfObsSpawned = 0;
		float sections = (Mathf.PI * 2) / 6;
		int[] anglesUsed = new int[maxNumOfObs];
		while (numOfObsSpawned < maxNumOfObs) {
			bool obstaclesInWay = false;
			RaycastHit2D hit;
			int rndObstacle = Random.Range(0, Obstacles.Length);
			int rndAngle = Random.Range(1, 7);
			Vector3 anglePos = pi.position + new Vector3(Mathf.Cos(sections * rndAngle) * localplanetRad, Mathf.Sin(sections * rndAngle) * localplanetRad);
			Vector3 dir = (pi.position - anglePos).normalized;
			for (int i = 0; i < anglesUsed.Length; i++) {
				if (anglesUsed[i] == rndAngle) {
					obstaclesInWay = true;
				}
			}
			if (!obstaclesInWay) {
				int numOfObsAtAngle = Random.Range(0, 2);
				if (numOfObsAtAngle == 1) {
					//spawn 1
					Transform obstacleObj = GameObject.Instantiate(Obstacles[rndObstacle]).transform;
					planet.obsOnPlanet.Add(obstacleObj);
					hit = Physics2D.Raycast(anglePos - dir, dir, anglePos.sqrMagnitude, planetMask);
					obstacleObj.GetComponent<IObstacle>().Setup(planet, anglePos, hit.normal, planet.transform.localScale);
					obstacleObj.transform.SetParent(obsParents[rndObstacle].transform, true);
					anglesUsed[numOfObsSpawned] = rndAngle;
				}
				else {
					//spawn 2
					Transform obstacleObj1 = GameObject.Instantiate(Obstacles[rndObstacle]).transform;
					planet.obsOnPlanet.Add(obstacleObj1);
					Transform obstacleObj2 = GameObject.Instantiate(Obstacles[rndObstacle]).transform;
					planet.obsOnPlanet.Add(obstacleObj2);
					float extentsX = obstacleObj1.GetComponent<SpriteRenderer>().bounds.extents.x;
					hit = Physics2D.Raycast(anglePos - dir, dir, anglePos.sqrMagnitude, planetMask);
					obstacleObj1.GetComponent<IObstacle>().Setup(planet, anglePos, hit.normal, planet.transform.localScale);
					obstacleObj2.GetComponent<IObstacle>().Setup(planet, anglePos, hit.normal, planet.transform.localScale);
					obstacleObj1.transform.position += obstacleObj1.transform.right * extentsX * 2f;
					Vector2 dir1 = (planet.transform.position - obstacleObj1.transform.position);
					hit = Physics2D.Raycast(obstacleObj1.transform.position, dir1.normalized, anglePos.sqrMagnitude, planetMask);
					obstacleObj1.transform.up = hit.normal;
					obstacleObj1.transform.position -= obstacleObj1.transform.up * 0.07f;
					obstacleObj1.transform.SetParent(obsParents[rndObstacle].transform, true);
					obstacleObj2.transform.SetParent(obsParents[rndObstacle].transform, true);
					anglesUsed[numOfObsSpawned] = rndAngle;
				}
				numOfObsSpawned++;
			}
		}
	}

	private void CreateObstacleParentObjs() {
		if (!obstacleParent) {
			obstacleParent = new GameObject("Obstacles");
			obsParents = new GameObject[Obstacles.Length];
			for (int i = 0; i < Obstacles.Length; i++) {
				obsParents[i] = new GameObject(Obstacles[i].name + "s");
				obsParents[i].transform.SetParent(obstacleParent.transform, true);
			}
		}
	}

	public Planets GeneratePlanetType() {
		float rndPlnt = Random.value;
		Planets planetType;
		if (rndPlnt >= 0.6f) {
			planetType = Planets.BARREN;
		}
		else {
			planetType = Planets.RESOURCE;
		}
		return planetType;
	}

}

public enum Planets {
	RESOURCE, BARREN, BLACKHOLE, SUN
}
