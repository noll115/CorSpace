using Cinemachine;
using System.Collections;
using UnityEngine;

public class CamController : MonoBehaviour {

	public float quickShakeSpeed;
	public RocketController player;
	public CinemachineVirtualCamera mainCam;
	public CinemachineVirtualCamera groupCam;
	public CinemachineVirtualCamera startCam;
	public CinemachineTargetGroup group;


	private void Awake() {
		player.OnPlayerChangePlanet += ChangeCam;
	}

	private void Start() {
		StartCoroutine(SwitchCam());
	}

	IEnumerator SwitchCam() {
		yield return new WaitForSeconds(0.7f);
		startCam.Priority = 0;
		mainCam.Priority = 2;
	}




	void ChangeCam() {
		if(player.PlanetOn) {
			group.m_Targets[1].target = player.PlanetOn.transform;
			groupCam.Priority = 2;
			mainCam.Priority = 1;
		}
		else {
			groupCam.Priority = 1;
			mainCam.Priority = 2;
		}
	}
}
