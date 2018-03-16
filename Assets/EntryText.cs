using UnityEngine;
using TMPro;

public class EntryText: MonoBehaviour  {

	[SerializeField] TextMeshProUGUI entryName;
	[SerializeField] TextMeshProUGUI placeNum;
	[SerializeField] TextMeshProUGUI score;

	public void Setup(int placeNum,string name,int score){
		this.entryName.SetText(name);
		this.placeNum.SetText(placeNum.ToString());
		this.score.SetText(score.ToString());
	}
}
