using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System;


public class HighScoreManager : MonoBehaviour {

	//const string privateCode = "_gFd3qpuUkK1H05Ox8cW9gfGTnEM3bhU-zohtBRSDeoQ";
	//const string publicCode = "5aa86e8a012b2e10689634c8";
	//const string webURL = "http://dreamlo.com/lb/";
	[SerializeField] Button uploadButton;
	[SerializeField] TMP_InputField input;
	[SerializeField] GameObject canvas;
	[SerializeField] EntryText[] textEntries;
	[SerializeField] GameObject refreshingTxt;
	[SerializeField] string statisticName = "CorSpace Scoreboard";
	UserAccountInfo accountInfo;

	private void Awake() {
		if (uploadButton) {
			uploadButton.onClick.AddListener(SetHighScore);
		}

	}


	public void SetHighScore() {
		uploadButton.gameObject.SetActive(false);
		if (!PlayFabClientAPI.IsClientLoggedIn()) {
			var login = new LoginWithCustomIDRequest() {
				CustomId = SoundManager.instance.playerID,
				CreateAccount = true
			};
			PlayFabClientAPI.LoginWithCustomID(login, x => LoggedIn(input.text), y => print("FAILED LOGGING IN: " + y.ErrorMessage));
		}
		else {
			LoggedIn(input.text);
		}
	}

	void LoggedIn(string playerName) {
		if (accountInfo == null) {
			var info = new GetAccountInfoRequest();
			PlayFabClientAPI.GetAccountInfo(info, x => { accountInfo = x.AccountInfo; SetUserTitle(x.AccountInfo, playerName); RefreshHighScore(); }, y => print("FAILED GETTING INFO: " + y.ErrorMessage));
		}
		else {
			RefreshHighScore();
			SetUserTitle(accountInfo, playerName);
		}

	}

	void SetUserTitle(UserAccountInfo info, string playerName) {
		var dis = new UpdateUserTitleDisplayNameRequest() {
			DisplayName = playerName
		};
		//print(playerName);
		PlayFabClientAPI.UpdateUserTitleDisplayName(dis, x => SetScore(info.PlayFabId), y => print("FAILED USERDISPLAYNAME: " + y.ErrorMessage));
	}

	void SetScore(string playerID) {
		var stat = new PlayFab.ServerModels.UpdatePlayerStatisticsRequest() {
			Statistics = new List<PlayFab.ServerModels.StatisticUpdate>(){new PlayFab.ServerModels.StatisticUpdate(){
				StatisticName = this.statisticName,Value = GameManager.Gm.score
	}
	},
			PlayFabId = playerID
		};
		PlayFabServerAPI.UpdatePlayerStatistics(stat, x => { StartCoroutine(GetHighScoresRefresh()); }, y => print("FAILED UPDATING STATS: " + y.ErrorMessage));
	}

	void RefreshHighScore() {
		if (!refreshingTxt.activeSelf) {
			refreshingTxt.SetActive(true);
			for (int i = 0; i < textEntries.Length; i++) {
				textEntries[i].gameObject.SetActive(false);
			}
		}
		else {
			refreshingTxt.SetActive(false);
			for (int i = 0; i < textEntries.Length; i++) {
				textEntries[i].gameObject.SetActive(true);
			}
		}

	}

	IEnumerator GetHighScoresRefresh() {
		canvas.SetActive(true);
		var r = new PlayFab.ServerModels.GetLeaderboardRequest {
			StatisticName = this.statisticName,
			MaxResultsCount = 5
		};
		yield return new WaitForSeconds(0.2f);
		PlayFabServerAPI.GetLeaderboard(r, x => { print("Got new LeaderBoard"); LeaderBoardResult(x); RefreshHighScore(); }, y => { print("FAILED LEADERBOARD: " + y.ErrorMessage); });
	}




	public void GetHighScores() {
		canvas.SetActive(true);
		var r = new PlayFab.ServerModels.GetLeaderboardRequest {
			StatisticName = this.statisticName,
			MaxResultsCount = 5
		};
		PlayFabServerAPI.GetLeaderboard(r, x => { print("Got new LeaderBoard"); LeaderBoardResult(x); }, y => { print("FAILED LEADERBOARD: " + y.ErrorMessage); });

	}


	void LeaderBoardResult(PlayFab.ServerModels.GetLeaderboardResult s) {
		var leaderBoard = s.Leaderboard;
		for (int i = 0; i < leaderBoard.Count; i++) {
			textEntries[i].Setup(i + 1, leaderBoard[i].DisplayName, leaderBoard[i].StatValue);
		}
	}

	//public void UploadScore() {
	//	//print(input.text + ":" + GameManager.Gm.score);
	//	uploadButton.gameObject.SetActive(false);
	//	UploadNewScore(input.text, GameManager.Gm.score);
	//}

	//void UploadNewScore(string name, int score) {
	//	StartCoroutine(UploadNewHighScore(name, score));
	//}

	//IEnumerator UploadNewHighScore(string name, int score) {
	//	WWW www = new WWW(webURL + privateCode + "/add/" + WWW.EscapeURL(name) + "/" + score);
	//	yield return www;

	//	if (string.IsNullOrEmpty(www.error)) {
	//		//print("uploaded");
	//		StartCoroutine(DownloadHighScores());
	//	}
	//	else {
	//		print("Failed" + www.error);
	//	}
	//}

	//public void GetHighScores() {
	//	StartCoroutine(DownloadHighScores());
	//}

	//IEnumerator DownloadHighScores() {
	//	WWW www = new WWW(webURL + publicCode + "/pipe/5");
	//	yield return www;
	//	//print(www.text);

	//	if (string.IsNullOrEmpty(www.error)) {
	//		FormatHighScore(www.text);
	//		canvas.SetActive(true);

	//	}
	//	else {
	//		print("Failed to download" + www.error);
	//	}
	//}

	//void FormatHighScore(string text) {
	//	string[] entriesStr = text.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
	//	Entry[] entries = new Entry[entriesStr.Length];
	//	for (int i = 0; i < entriesStr.Length; i++) {
	//		string[] entryInfo = entriesStr[i].Split(new char[] { '|' });
	//		print(entryInfo[0]);
	//		print(entryInfo[1]);
	//		entries[i].setup(entryInfo);
	//	}

	//	FillEntries(entries);
	//}

	//void FillEntries(Entry[] highscores) {
	//	for (int i = 0; i < highscores.Length; i++) {
	//		textEntries[i].Setup(i + 1, highscores[i].name, highscores[i].score);
	//	}
	//}



}
[System.Serializable]
public struct Entry {
	public string name;
	public int score;
	public void Setup(string[] entry) {
		name = entry[0];
		score = int.Parse(entry[1]);
	}

}
