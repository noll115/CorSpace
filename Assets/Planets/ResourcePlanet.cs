using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcePlanet : Planet {

	public PlanetResources resources;

	public override void SetUpPlanet(PlanetInfo p) {
		base.SetUpPlanet(p);
		resources = GetComponent<PlanetResources>();
		resources.Setup(p);
	}

	public override void PlayerAction(PlayerResources pResources) {
		resources.SubtractResources(pResources);
		planetInfo.UpdateData(resources);
	}

	private void OnBecameVisible() {

	}


}
