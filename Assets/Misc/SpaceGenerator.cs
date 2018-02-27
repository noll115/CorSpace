using System.Collections.Generic;
using UnityEngine;

public class SpaceGenerator : MonoBehaviour {

	public SpawnablePlanet[] spawnablePlanets;
	public static SpaceCell[,] cells = new SpaceCell[3, 3];
	public float planetRadius = 2.95f;
	public int cellx;
	public int celly;
	public int NumOfCells;
	public int cellSize = 200;
	public int maxPlanetsSpawned;
	public GameObject barrenPlanet;
	public GameObject resourcePlanet;
	public GameObject blackHole;
	public GameObject sun;
	public LayerMask planetMask;
	public LayerMask playerMask;
	public GameObject[] Obstacles;
	public GameObject obstacleParent;
	GameObject[] obsParents;


	[ContextMenu("Generate Cell")]
	public void Generate() {
		GameObject Parent = new GameObject("Cells");
		int row = 0;
		for (int i = 1; i > -2; i--) {
			int col = 0;
			for (int j = -1; j < 2; j++) {
				List<BarrenPlanet> barrenPlanets = new List<BarrenPlanet>();
				List<ResourcePlanet> resourcePlanets = new List<ResourcePlanet>();
				SpaceCell Cell = new GameObject("Cell " + row + ":" + col).AddComponent<SpaceCell>();
				Cell.transform.SetParent(Parent.transform, true);
				Vector3 cellPos = new Vector3(cellSize * j, cellSize * i);
				cells[row, col] = Cell;
				BoxCollider2D box = Cell.gameObject.AddComponent<BoxCollider2D>();
				box.isTrigger = true;
				box.size = new Vector2(cellSize, cellSize);
				CreatePlanets(barrenPlanets, resourcePlanets, cellPos);
				Cell.Setup(row, col, box, barrenPlanets, resourcePlanets, cellPos);
				col++;
			}
			row++;
		}
		//for(int i = 0;i < cells.GetLength(0);i++) {
		//	for(int j = 0;j < cells.GetLength(1);j++) {
		//		print(cells[i,j].name);
		//	}
		//}
	}



	public void CreatePlanets(List<BarrenPlanet> bPlanets, List<ResourcePlanet> rPlanets, Vector2 cellPos) {
		PlanetInfo pi;
		ResourcePlanet rp;
		BarrenPlanet bp;
		for (int i = 0; i < maxPlanetsSpawned; i++) {
			pi = GeneratePlanetInfo(cellPos);
			switch (pi.planetType) {
				case Planets.RESOURCE:
					rp = GameObject.Instantiate(resourcePlanet).GetComponent<ResourcePlanet>();
					rPlanets.Add(rp);
					rPlanets[rPlanets.Count - 1].SetUpPlanet(pi);
					rp.planetSprite.sprite = RndPlanetSprite(pi, rp);
					break;
				case Planets.BARREN:
					bp = GameObject.Instantiate(barrenPlanet).GetComponent<BarrenPlanet>();
					bPlanets.Add(bp);
					bPlanets[bPlanets.Count - 1].SetUpPlanet(pi);
					bp.planetSprite.sprite = RndPlanetSprite(pi, bp);
					break;
				case Planets.BLACKHOLE:
					break;
				case Planets.SUN:
					break;
				default:
					break;
			}
		}

	}
	[ContextMenu("Generate Planet")]
	public PlanetInfo GeneratePlanetInfo(Vector2 cell) {
		PlanetInfo pi = ScriptableObject.CreateInstance<PlanetInfo>();
		float size = Random.Range(1f, 1.5f);
		pi.size = new Vector3(size, size, 1);
		pi.planetType = GeneratePlanetType();
		pi.startRot = Random.Range(0, 360f);
		if (pi.planetType == Planets.RESOURCE) {
			pi.lavaAmount = Random.Range(100, 151);
			pi.maxLava = pi.lavaAmount;
			pi.waterAmount = Random.Range(100, 151);
			pi.maxWater = pi.waterAmount;
			pi.oresAmount = Random.Range(100, 151);
			pi.maxOres = pi.oresAmount;
			pi.fuelAmount = Random.Range(60, 100);
			pi.maxFuel = pi.fuelAmount;
		}
		else {
			pi.maxOres = 50;
			pi.maxLava = 50;
			pi.maxWater = 50;
		}
		pi.numOfAsteroids = Random.Range(0, 5);
		pi.rotSpeed = Random.Range(2f, 5f);
		pi.position = PosInSquare(cell, pi.size);
		return pi;
	}

	Vector2 PosInSquare(Vector2 cellPos, Vector3 planetSize) {
		bool goodPos = false;
		Vector2 pos = Vector2.zero;
		while (!goodPos) {
			pos = cellPos;
			int xpos = Random.Range(-cellSize / 2 + (cellSize / 8), cellSize / 2 - (cellSize / 8));
			int ypos = Random.Range(-cellSize / 2 + (cellSize / 8), cellSize / 2 - (cellSize / 8));
			pos += new Vector2(xpos, ypos);
			if (!Physics2D.CircleCast(pos, 15, Vector2.zero, 0, planetMask) && !Physics2D.CircleCast(pos, 10, Vector2.zero, 0, playerMask)) {
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
	[ContextMenu("Generate PlanetWObs")]
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
					obstacleObj.GetComponent<Obstacle>().Setup(spawnablePlanet,anglePos,hit.normal,planet.transform.localScale);
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
					obstacleObj1.GetComponent<Obstacle>().Setup(spawnablePlanet,anglePos,hit.normal,planet.transform.localScale);
					obstacleObj2.GetComponent<Obstacle>().Setup(spawnablePlanet,anglePos,hit.normal,planet.transform.localScale);
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
		if (rndPlnt > 0.6f) {
			planetType = Planets.BARREN;
		}
		else {
			planetType = Planets.RESOURCE;
		}
		//else if(rndPlnt >= 0.15f) {
		//	planetType = Planets.BLACKHOLE;
		//	//spawn blackhole
		//}
		//else {
		//	planetType = Planets.SUN;
		//	//spawn sun
		//}
		return planetType;
	}

}

public enum Planets {
	RESOURCE, BARREN, BLACKHOLE, SUN
}
