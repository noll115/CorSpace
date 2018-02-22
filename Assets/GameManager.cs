using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;

public class GameManager : MonoBehaviour {

	static GameManager gameManager;
	public static GameManager Gm {
		get {
			if(gameManager == null) {
				gameManager = new GameObject("GameManager").AddComponent<GameManager>();
				return gameManager;
			}
			else {
				return gameManager;
			}
		}
	}
	public PlayerResources player;
	public SpaceGenerator sg;
	public bool generateOnStart;
	public int score;
	public TextMeshProUGUI scoreNum;
	public GameObject button;
	public GameObject scoreGUI;
	public GameObject quitButton;
	public GameObject[] gOs;
	StringBuilder sb;




	private void Awake() {
		player = FindObjectOfType<PlayerResources>();
		if(gameManager != null && gameManager != this) {
			Destroy(gameObject);
			return;
		}
		else {
			gameManager = this;
		}
		sg = GetComponent<SpaceGenerator>();
		player.OnPlayerDeath += PlayerDead;
		if(generateOnStart) {
			sg.Generate();
		}
		sb = new StringBuilder(score.ToString("00000"));
	}

	public void AddScore(int incScore) {
		if(incScore < 0) {
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
		player.gameObject.SetActive(false);
		SoundManager.instance.StopAllSounds();
		SoundManager.instance.PlaySound2D("lose");
		StartCoroutine(StartFade());
	}

	IEnumerator StartFade() {
		for(int i = 0;i < gOs.Length;i++) {
			gOs[i].SetActive(false);
		}
		yield return new WaitForSeconds(1f);
		button.SetActive(true);
		quitButton.SetActive(true);
		scoreGUI.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, -384);
	}

	public void LoadLevel() {
		SceneManager.LoadScene(0);
	}

	public void Quit() {
		Application.Quit();
	}
	

}
