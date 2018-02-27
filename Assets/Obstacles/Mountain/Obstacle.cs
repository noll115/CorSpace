using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Obstacle {


	void Setup(SpawnablePlanet planet,Vector3 pos, Vector3 upVec,Vector3 scale);
	
}
