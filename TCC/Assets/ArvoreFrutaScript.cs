using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArvoreFrutaScript : MonoBehaviour, ISustainInteractable {

	public HeightState interactableState;
	public GameObject fruta;

	public float holdNote = 5f;

	private float holding = 0f;


	public void Interact (HeightState currentHeight) {
		if (currentHeight != interactableState) {
			holding = 0f;
			StopAllCoroutines ();
			return;
		}

		StopAllCoroutines ();

		if(holding < holdNote){
			holding += Time.deltaTime;
		} else {
			holding = 0f;
			fruta.SetActive (true);
		}

		StartCoroutine ("StoppedHolding");
	}

	IEnumerator StoppedHolding(){
		yield return new WaitForSeconds (0.2f);
		holding = 0f;
	}
}
