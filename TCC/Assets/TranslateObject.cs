using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranslateObject : MonoBehaviour {

	public bool startAtCurrentPos;
	public Vector3 originalPos;
	public Vector3 destination;
	public Transform destinationTransform; //Opcional.
	public float seconds; //O nome é tempo, mas na verdade esta var armazenará a velocidade.
	public bool translateLocal;
	public bool easeOut;

	public bool startMove;

	float currentLerpTime;

	Transform myT;

	void Start () {
		myT = GetComponent<Transform> ();

		if (startAtCurrentPos)
			originalPos = myT.position;

		if (destinationTransform != null)
			destination = destinationTransform.position;
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
	public void StartMove(){
		startMove = true;
		if (destinationTransform != null)
			destination = destinationTransform.position;
	}
}
