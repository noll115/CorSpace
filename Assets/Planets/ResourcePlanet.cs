using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcePlanet : Planet {

	public PlanetResources pResources;

	public override void SetUpPlanet(PlanetInfo p) {
		base.SetUpPlanet(p);
		pResources = GetComponent<PlanetResources>();
		pResources.Setup(p);
	}

	public override void PlayerAction(PlayerResources pResources) {
		this.pResources.SubtractResources(pResources);
		planetInfo.UpdateData(this.pResources);
	}

	public override void AstroidHit() {
		pResources.AstroidHit();
		planetInfo.UpdateData(pResources);
	}


}
