using UnityEngine;

public class BarrenPlanet : Planet {

	public BarrenResources bResources;


	public override void SetUpPlanet(PlanetInfo p) {
		base.SetUpPlanet(p);
		bResources = GetComponent<BarrenResources>();
		bResources.Setup(p);
	}


	public override void PlayerAction(PlayerResources presource) {
		bResources.AddResources(presource);
		planetInfo.UpdateData(bResources);
	}


	public override void AstroidHit(){
		bResources.AstroidHit();
		planetInfo.UpdateData(bResources);
	} 


}
