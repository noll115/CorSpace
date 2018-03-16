using UnityEngine;
using UnityEngine.UI;
using System.Collections;
[ExecuteInEditMode]
public class Transition : MonoBehaviour {

	[SerializeField] AnimationCurve transitionCurve;
	Material mat;
	public bool inTransition;
	[SerializeField]float transitionTime = 1;
	string transitionName = "_Transit";

	private void Awake() {
		mat = GetComponent<Image>().material;
	}

	[ContextMenu("Start trans")]
	public void ToBlack(float transitionSpeed = 0.7f) {
		
		StartCoroutine(StartTrans(transitionSpeed));
	}

	[ContextMenu("End Trans")]
	public void FromBlack(float transitionSpeed = 0.7f) {
		StartCoroutine(EndTrans(transitionSpeed));
	}

	IEnumerator StartTrans(float transitionSpeed) {
		inTransition = true;
		float curveX = 0;
		while (transitionTime < 1) {
			transitionTime = transitionCurve.Evaluate(curveX);
			mat.SetFloat(transitionName, transitionTime);
			curveX += Time.deltaTime * transitionSpeed;
			yield return null;
		}
		
		inTransition = false;
	}

	IEnumerator EndTrans(float transitionSpeed) {
		inTransition = true;
		float curveX = 1;
		while (transitionTime > 0) {
			transitionTime = transitionCurve.Evaluate(curveX);
			mat.SetFloat(transitionName, transitionTime);
			curveX -= Time.deltaTime* transitionSpeed;
			yield return null;
		}
		inTransition = false;
	}



}
