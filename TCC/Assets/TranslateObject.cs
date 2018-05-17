using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranslateObject : MonoBehaviour {

	public Vector3 originalPos;
	public Vector3 destination;
	public float seconds; //O nome é tempo, mas na verdade esta var armazenará a velocidade.
	public bool translateLocal;
	public bool easeOut;

	public bool startMove;

	float currentLerpTime;

	Transform myT;

	void Start () {
		myT = GetComponent<Transform> ();
	}

	void Update () {
		if (!startMove)
			return;

		currentLerpTime += Time.deltaTime;
		if (currentLerpTime > seconds) {
			currentLerpTime = seconds;
		}

		float perc = currentLerpTime / seconds;
		if(easeOut)
			perc = Mathf.Sin(perc * Mathf.PI * 0.5f); //Pra dar Ease Out

		if(translateLocal){
			myT.localPosition = Vector3.Lerp (originalPos, destination, perc);
		} else {
			myT.position = Vector3.Lerp (originalPos, destination, perc);
		}

		if (perc == 1f)
			enabled = false;
	}
}
