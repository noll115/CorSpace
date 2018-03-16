using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IObstacle {

	Planet ParentPlanet { get; }


	void Setup(Planet planet,Vector3 pos, Vector3 upVec,Vector3 scale);
	
}
