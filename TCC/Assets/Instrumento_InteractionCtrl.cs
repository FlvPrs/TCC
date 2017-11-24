using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instrumento_InteractionCtrl : MonoBehaviour, IStaccatoInteractable {

	public HeightState pitch;
	private bool canInteract;

	[HideInInspector]
	public bool interactionDone;

	// Use this for initialization
	void Start () {
		
	}

	public void Interact (HeightState height){
		if(canInteract && height == pitch){
			interactionDone = true;
			gameObject.SetActive (false);
		}
	}

	void OnTriggerStay(Collider col){
		if (col.CompareTag ("Player")) {
			canInteract = true;
		}
	}

	void OnTriggerExit(Collider col){
		if (col.CompareTag ("Player")){
			canInteract = false;
		}
	}
}
