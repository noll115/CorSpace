using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Text;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour {

	static GameManager gameManager;
	public static GameManager Gm {
		get {
			if (gameManager == null) {
				gameManager = new GameObject("GameManager").AddComponent<GameManager>();
				return gameManager;
			}
			else {
				return gameManager;
			}
		}
	}
	public RocketController player;
	public SpaceGenerator spaceGenerator;
	public bool generateOnStart;
	public int score;
	public int NumOfPlanetsRevived;
	public int NumOfPlanetsDead;
	public TextMeshProUGUI scoreNum;
	public Button retryButton;
	public Button quitButton;
	public GameObject scoreGUI;
	public GameObject[] gOs;
	public Transition transition;
	HighScoreManager hsManager;
	StringBuilder sb;


	private void Awake() {
		if (gameManager != null && gameManager != this) {
			Destroy(gameObject);
			return;
		}
		else {
			gameManager = this;
		}
		if (generateOnStart) {
			spaceGenerator.Generate();
		}
		retryButton.onClick.AddListener(LoadLevel);
#if UNITY_STANDALONE
		quitButton.onClick.AddListener(Quit);
#endif
		hsManager = GetComponent<HighScoreManager>();
		sb = new StringBuilder(score.ToString("00000"));
	}

	private void Start() {
		player = FindObjectOfType<RocketController>();
		player.pResources.OnPlayerDeath += PlayerDead;
		transition.FromBlack();
	}

	public void AddScore(int incScore) {
		if (incScore < 0) {
			scoreNum.color = Color.red;
		}
		else {
			scoreNum.color = Color.green;
		}
		this.score += incScore;
		sb.Remove(0, sb.Length);
		sb.Append(score.ToString("00000"));
		scoreNum.SetText(sb);
	}


	public void PlayerDead() {
		SoundManager.instance.StopAllSounds();
		SoundManager.instance.PlaySound2D("lose");
		StartCoroutine(StartFade());
	}
	IEnumerator StartFade() {
		for (int i = 0; i < gOs.Length; i++) {
			gOs[i].SetActive(false);
		}
		yield return new WaitForSeconds(1f);
		hsManager.GetHighScores();
		retryButton.gameObject.SetActive(true);
#if UNITY_STANDALONE
		quitButton.gameObject.SetActive(true);
#endif
		hsManager.gameObject.SetActive(true);
		scoreGUI.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, -384);
	}

	public void LoadLevel() {
		transition.ToBlack();
		StartCoroutine(StartLoading());
	}

	IEnumerator StartLoading() {
		bool animended = false;
		while (!animended) {
			if (!transition.inTransition) {
				animended = true;
			}
			yield return null;
		}
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	public void Quit() {
		Application.Quit();
	}


}



