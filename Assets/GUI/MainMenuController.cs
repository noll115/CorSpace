using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Cinemachine;

public class MainMenuController : MonoBehaviour {

	public ParticleSystem thrust;
	public ParticleSystem landing;
	public Animator rocketAnim;
	public Button playButton;
	public Transition transition;
	Animator buttonAnim;
	public Image title;
	public Transform planet;
	float angle;
	public float turnSpeed;
	public CinemachineVirtualCamera mainMenuCam;
	public CinemachineVirtualCamera startCam;
	public CinemachineVirtualCamera scoreCam;
	public float minDist;
	public bool close;
	public SpriteRenderer[] planetRenderers;
	public int[] planetRenderesmat;
	bool landed;
	bool inScore;
	[SerializeField] GameObject mainMenuCanvas;
	[SerializeField] GameObject highScoreCanvas;
	HighScoreManager hsmanager;


	private void Awake() {
		buttonAnim = playButton.GetComponent<Animator>();
		hsmanager = GetComponent<HighScoreManager>();
	}

	private void Start() {
		transition.FromBlack();
		for (int i = 0; i < planetRenderers.Length; i++) {
			planetRenderers[i].material.SetFloat("_CutOff", planetRenderesmat[i]);
		}
		SoundManager.instance.PlaySound2D("mainmenu");
	}

	private void Update() {
		angle += turnSpeed * Time.deltaTime % 360;
		planet.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
		if (landed) {
			if (Input.GetButtonDown("Thrust") && !inScore) {
				buttonAnim.SetTrigger("Pressed");
				Launch();
			}
			if (Input.GetButtonDown("Horizontal")) {
				if (!inScore) {
					scoreCam.Priority = 2;
					hsmanager.GetHighScores();
					mainMenuCanvas.SetActive(false);
				}
				else {
					scoreCam.Priority = 0;
					highScoreCanvas.gameObject.SetActive(false);
					mainMenuCanvas.SetActive(true);
					buttonAnim.SetTrigger("Fade");
				}
				inScore = !inScore;

			}
		}
	}

	public void StartParicles() {
		thrust.Play();

	}

	public void EndParticles() {
		thrust.Stop();
		ChangeCams();
	}

	public IEnumerator FadeInTexts() {
		StartCoroutine(TitleFrom0Alpha());
		yield return new WaitForSeconds(2f);
		landed = true;
		buttonAnim.SetTrigger("Fade");

	}

	public void ChangeCams() {
		mainMenuCam.Priority = 1;
		startCam.Priority = 0;
	}

	public void OnLand() {
		StartCoroutine(FadeInTexts());
		landing.Play();

	}


	public void Launch() {
		landed = false;
		StartCoroutine(TitleFrom1Alpha());
		buttonAnim.SetTrigger("Fade");
		thrust.Play();
		mainMenuCam.Follow = null;
		rocketAnim.SetBool("ButtonPressed", true);
	}

	public void LoadLevel() {
		StartCoroutine(LoadScene());
	}


	private IEnumerator LoadScene() {
		// The Application loads the Scene in the background at the same time as the current Scene.
		//This is particularly good for creating loading screens. You could also load the Scene by build //number.
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
		asyncLoad.allowSceneActivation = false;

		//Wait until the last operation fully loads to return anything
		while (!asyncLoad.isDone) {
			if (asyncLoad.progress >= 0.9f) {
				StartCoroutine(CamFollow());
				while (!close) {
					yield return null;
				}
				asyncLoad.allowSceneActivation = true;
			}
			yield return null;
		}
	}

	private IEnumerator CamFollow() {
		transition.ToBlack();
		while (Vector3.SqrMagnitude(transform.position - mainMenuCam.transform.position) > minDist * minDist && !transition.inTransition) {
			Vector3.SqrMagnitude(transform.position - mainMenuCam.transform.position);
			mainMenuCam.transform.position = Vector3.MoveTowards(mainMenuCam.transform.position, transform.position, 2 * Time.deltaTime);
			yield return null;
		}
		close = true;
		yield return null;
	}

	IEnumerator TitleFrom0Alpha() {
		while (title.color.a < 1) {
			title.color = new Color(1, 1, 1, title.color.a + Time.deltaTime);
			yield return null;
		}
	}

	IEnumerator TitleFrom1Alpha() {
		while (title.color.a > 0) {
			title.color = new Color(1, 1, 1, title.color.a - Time.deltaTime);
			yield return null;
		}
	}
}
