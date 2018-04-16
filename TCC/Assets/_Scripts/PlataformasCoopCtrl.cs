using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlataformasCoopCtrl : MonoBehaviour, ISongListener, IFatherSustainInteractable {

	public Platforms[] plataformas;

	public HeightState interactableState;

	public GameObject fatherExternalTrigger;

	private Vector3[] startingPos;
	private Vector3[] originalPos;
	private Vector3[] son_Distance;
	private Vector3[] dad_Distance;
	private Vector3[] son_deltaY;
	private Vector3[] dad_deltaY;

	bool playerSing = false;
	bool dadSing = false;
	float dad_singTime;

	void Start(){
		dad_singTime = 0f;

		startingPos = new Vector3[plataformas.Length];
		originalPos = new Vector3[plataformas.Length];
		son_Distance = new Vector3[plataformas.Length];
		dad_Distance = new Vector3[plataformas.Length];
		son_deltaY = new Vector3[plataformas.Length];
		dad_deltaY = new Vector3[plataformas.Length];

		for (int i = 0; i < startingPos.Length; i++) {
			startingPos [i] = plataformas [i].platform.localPosition;
			originalPos [i] = plataformas [i].platform.localPosition;
			son_Distance [i] = Vector3.up * ((plataformas [i].newYPos - startingPos [i].y) * (1 - plataformas [i].dadToSonRate));
			dad_Distance [i] = Vector3.up * ((plataformas [i].newYPos - startingPos [i].y) * plataformas [i].dadToSonRate);
			son_deltaY [i] = Vector3.zero;
			dad_deltaY [i] = Vector3.zero;
		}

		fatherExternalTrigger.SetActive (false);
	}

	void Update(){
		if (playerSing && dadSing)
			return;
		else {
			if (!playerSing) {
				for (int i = 0; i < plataformas.Length; i++) {
					Vector3 oldPos = plataformas [i].platform.localPosition;
					plataformas [i].platform.localPosition = Vector3.MoveTowards (plataformas [i].platform.localPosition, originalPos [i] + dad_deltaY [i], 0.02f);
					son_deltaY [i] += plataformas [i].platform.localPosition - oldPos;
				}
			}
			if (!dadSing) {
				for (int i = 0; i < plataformas.Length; i++) {
					Vector3 oldPos = plataformas [i].platform.localPosition;
					plataformas [i].platform.localPosition = Vector3.MoveTowards (plataformas [i].platform.localPosition, originalPos [i] + son_deltaY [i], 0.02f);
					dad_deltaY [i] += plataformas [i].platform.localPosition - oldPos;
				}
			}
		}
	}

	public void DetectSong (PlayerSongs song, bool isSingingSomething, bool isFather = false){
		if(song == PlayerSongs.Crescimento){
			for (int i = 0; i < plataformas.Length; i++) {
				Vector3 newPos = originalPos [i] + son_Distance [i] + dad_deltaY [i];

				Vector3 oldPos = plataformas [i].platform.localPosition;
				plataformas [i].platform.localPosition = Vector3.MoveTowards (plataformas [i].platform.localPosition, newPos, 0.2f);
				son_deltaY [i] += plataformas [i].platform.localPosition - oldPos;
			}
		}
	}

//	public void Interact(HeightState currentHeight){
//		if (currentHeight != interactableState) 
//			return;
//
//		//StartCoroutine ("InteractionStopped");
//
//		for (int i = 0; i < plataformas.Length; i++) {
//			Vector3 newPos = originalPos [i] + son_Distance [i] + dad_deltaY [i];
//
//			Vector3 oldPos = plataformas [i].platform.localPosition;
//			plataformas [i].platform.localPosition = Vector3.MoveTowards (plataformas [i].platform.localPosition, newPos, 0.2f);
//			son_deltaY [i] += plataformas [i].platform.localPosition - oldPos;
//		}
//	}

	public void FatherSustainInteraction(PlayerSongs song){
		if (dad_singTime <= 3f) {
			dad_singTime += Time.deltaTime;

			if (dad_singTime >= 0) {
				dadSing = true;
			} else {
				dadSing = false;
			}
		} else {
			dadSing = false;
			dad_singTime = -2f;
		}

		int chegou = 0;

		for (int i = 0; i < plataformas.Length; i++) {
			Vector3 newPos = originalPos [i] + dad_Distance [i] + son_deltaY [i];

			if (plataformas [i].platform.localPosition.y >= plataformas [i].newYPos - 0.1f) {
				chegou++;
				continue;
			} else {
				Vector3 oldPos = plataformas [i].platform.localPosition;
				plataformas [i].platform.localPosition = Vector3.MoveTowards (plataformas [i].platform.localPosition, newPos, 0.1f);
				dad_deltaY [i] += plataformas [i].platform.localPosition - oldPos;
			}
		}

		if (chegou >= plataformas.Length) {
			fatherExternalTrigger.SetActive (true);
			enabled = false;
		}
	}

	IEnumerator InteractionStopped(){
		playerSing = true;
		yield return new WaitForSeconds (0.2f);
		playerSing = false;
	}
}


[System.Serializable]
public class Platforms
{
	public Transform platform;
	public float newYPos;
	[Range(0f, 1f)]
	public float dadToSonRate; //0.5f significa que a plataforma sobe a mesma quantidade para ambos. Um numero menor faz a plataforma subir mais com o pai, e vice-versa.
}