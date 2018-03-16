using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;
using TMPro;
using Cinemachine;
using UnityEngine.SceneManagement;

public class ExplainManager : MonoBehaviour {

	public PlayableDirector director;
	public TimelineAsset[] assets;
	public Transform player;
	public Transform dangerPlanet;
	public Transform dangerPointer;
	public Transform safePlanet;
	public Transform safePointer;

	public Canvas mainCanvas;
	public Image resourceLifeBar;
	public Image barrenLifeBar;
	public Transition transition;
	[SerializeField] States sceneSate = States.FINDPLANET;
	public CinemachineVirtualCamera cam1;
	public CinemachineVirtualCamera cam2;
	public CinemachineVirtualCamera cam3;
	public SpriteRenderer livePlanet;
	public SpriteRenderer deadPlanet;
	Material deadplanetMat;
	Material livePlanetMat;
	Coroutine explain3Coroutine;
	Coroutine explain2Coroutine;

	enum States : int {
		FINDPLANET = 0, COLLECTPLANET = 1, GIVEPLANET = 2
	}

	private void Awake() {
		director = GameObject.FindObjectOfType<PlayableDirector>();
		director.Play(assets[(int)sceneSate]);
		deadplanetMat = deadPlanet.material;
		livePlanetMat = livePlanet.material;
		deadplanetMat.SetFloat("_CutOff", 0);
		DetermineCam();
	}
	private void Start() {
		transition.FromBlack();
	}

	private void Update() {
		if (Input.GetButtonDown("Thrust")) {
			PlayNextScene();
		}
		switch (sceneSate) {
			case States.FINDPLANET:
				UsePointers();
				break;
			case States.COLLECTPLANET:
				if (explain2Coroutine == null) {
					explain2Coroutine = StartCoroutine(SubtractPlanet());
				}
				break;
			case States.GIVEPLANET:
				if (explain3Coroutine == null) {
					explain3Coroutine = StartCoroutine(FillPlanet());
				}
				break;
		}
	}

	public void PlayNextScene() {
		sceneSate++;
		sceneSate = (States)Mathf.Clamp((int)sceneSate, 0, assets.Length);
		if ((int)sceneSate < assets.Length) {
			director.Play(assets[(int)sceneSate]);
			DetermineCam();
		}
		else {
			transition.ToBlack();
			StartCoroutine(StartLoading());
		}
	}

	private void DetermineCam() {
		switch (sceneSate) {
			case States.FINDPLANET:
				cam1.Priority = 3;
				cam2.Priority = 1;
				cam3.Priority = 1;


				break;
			case States.COLLECTPLANET:
				cam1.Priority = 1;
				cam2.Priority = 3;
				cam3.Priority = 1;

				break;
			case States.GIVEPLANET:
				cam1.Priority = 1;
				cam2.Priority = 1;
				cam3.Priority = 3;

				break;
		}
	}

	void UsePointers() {
		Vector3 neededdir = safePlanet.position - player.position;
		float angle = Mathf.Atan2(neededdir.y, neededdir.x) * Mathf.Rad2Deg;
		safePointer.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
		neededdir = dangerPlanet.position - player.position;
		angle = Mathf.Atan2(neededdir.y, neededdir.x) * Mathf.Rad2Deg;
		dangerPointer.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
	}

	IEnumerator StartLoading() {
		bool animended = false;
		while (!animended) {
			if (!transition.inTransition) {
				animended = true;
			}
			yield return null;
		}
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
	}

	IEnumerator FillPlanet() {
		float fill = 0;
		double playTime = assets[2].duration + Time.time;
		float r = 1 - fill;
		float g = fill;
		barrenLifeBar.color = new Color(r, g, 0, 1);
		barrenLifeBar.rectTransform.localScale = new Vector3(fill, 1, 1);
		while (deadplanetMat.GetFloat("_CutOff") < 1 && Time.time < playTime) {
			r = 1 - fill;
			g = fill;
			barrenLifeBar.color = new Color(r, g, 0, 1);
			barrenLifeBar.rectTransform.localScale = new Vector3(fill, 1, 1);
			deadplanetMat.SetFloat("_CutOff", fill);
			fill += Time.deltaTime * 0.2f;
			yield return new WaitForEndOfFrame();
		}
	}

	IEnumerator SubtractPlanet() {
		float fill = 1;
		yield return new WaitForSeconds(1f);
		double playTime = assets[1].duration + Time.time;
		while (livePlanetMat.GetFloat("_CutOff") > 0.5f && Time.time < playTime) {
			float r = 1 - fill;
			float g = fill;
			resourceLifeBar.color = new Color(r, g, 0, 1);
			resourceLifeBar.rectTransform.localScale = new Vector3(fill, 1, 1);
			livePlanetMat.SetFloat("_CutOff", fill*2);
			fill -= Time.deltaTime * 0.2f;
			yield return new WaitForEndOfFrame();
		}
	}
}
