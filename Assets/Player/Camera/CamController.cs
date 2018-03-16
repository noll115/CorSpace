using Cinemachine;
using System.Collections;
using UnityEngine;

public class CamController : MonoBehaviour {

	public static CamController instance;
	private Camera cam;
	public RocketController player;
	public CinemachineVirtualCamera mainCam;
	public CinemachineVirtualCamera groupCam;
	public CinemachineVirtualCamera startCam;
	CinemachineTargetGroup group;
	CinemachineBasicMultiChannelPerlin mainCamNoise;
	CinemachineBasicMultiChannelPerlin groupCamNoise;
	Coroutine currentCoroutine;
	public Plane[] camPlanes;


	private void Awake() {
		if (instance != null && instance != this) {
			Destroy(gameObject);
		}
		instance = this;
		cam = GetComponent<Camera>();
		camPlanes = GeometryUtility.CalculateFrustumPlanes(cam);
		group = groupCam.GetComponent<CinemachineTargetGroup>();
		player.OnPlayerChangePlanet += ChangeCam;
		mainCamNoise = mainCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
		groupCamNoise = groupCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

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
		if (player.PlanetOn) {
			group.m_Targets[1].target = player.PlanetOn.transform;
			groupCam.Priority = 2;
			mainCam.Priority = 1;
		}
		else {
			groupCam.Priority = 1;
			mainCam.Priority = 2;
		}
	}

	public void ShakeCam(float shakeIntensity = 5f, float shakeTiming = 0.5f) {
		if (currentCoroutine != null) {
			StopCoroutine(currentCoroutine);
			Noise(0, 0);
		}
		currentCoroutine = StartCoroutine(_ProcessShake(shakeIntensity, shakeTiming));
	}

	private IEnumerator _ProcessShake(float shakeIntensity = 5f, float shakeTiming = 0.5f) {
		Noise(1, shakeIntensity);
		yield return new WaitForSeconds(shakeTiming);
		Noise(0, 0);
	}

	private void Noise(float amplitudeGain, float frequencyGain) {
		groupCamNoise.m_AmplitudeGain = amplitudeGain;
		mainCamNoise.m_AmplitudeGain = amplitudeGain;
		groupCamNoise.m_FrequencyGain = frequencyGain;
		mainCamNoise.m_FrequencyGain = frequencyGain;

	}
}
