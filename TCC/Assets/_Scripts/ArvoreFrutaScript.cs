using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArvoreFrutaScript : MonoBehaviour, ISongListener {

	public GameObject fruta;

	private Vector3 frutaInitPos;

//	public float holdNote = 5f;
//
//	private float holding = 0f;

	void Start(){
		frutaInitPos = fruta.transform.position;
	}

	public void DetectSong (PlayerSongs song, bool isSingingSomething, bool isFather = false, HeightState height = HeightState.Default){
		if (song == PlayerSongs.Crescimento) {
			if (!fruta.activeSelf) {
				fruta.transform.position = frutaInitPos;
				fruta.SetActive (true);
//				StartCoroutine (HideFruit ());
			}
		}
	}

//	public void Interact (HeightState currentHeight) {
//		if (currentHeight != interactableState) {
//			holding = 0f;
//			StopAllCoroutines ();
//			return;
//		}
//
//		StopAllCoroutines ();
//		StartCoroutine (HideFruit ());
//
//		if(holding < holdNote){
//			holding += Time.deltaTime;
//		} else {
//			holding = 0f;
//			if (!fruta.activeSelf) {
//				fruta.transform.position = frutaInitPos;
//				fruta.SetActive (true);
//				StartCoroutine (HideFruit ());
//			}
//		}
//
//		StartCoroutine ("StoppedHolding");
//	}

//	IEnumerator StoppedHolding(){
//		yield return new WaitForSeconds (0.2f);
//		holding = 0f;
//	}

//	IEnumerator HideFruit(){
//		yield return new WaitForSeconds (3f);
//		fruta.SetActive (false);
//	}
}
