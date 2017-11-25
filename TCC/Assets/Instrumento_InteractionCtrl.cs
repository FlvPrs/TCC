using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instrumento_InteractionCtrl : MonoBehaviour, IStaccatoInteractable {

	public HeightState pitch;
	private bool canInteract;

	[HideInInspector]
	public bool interactionDone;

	private ParticleSystem particle_Yay, particle_Nay;

	// Use this for initialization
	void Start () {
		particle_Yay = transform.Find ("Particle_Acerto").GetComponentInChildren<ParticleSystem> ();
		particle_Nay = transform.Find ("Particle_Erro").GetComponentInChildren<ParticleSystem> ();
	}

	public void Interact (HeightState height){
		if(canInteract && height == pitch){
			interactionDone = true;
			//gameObject.SetActive (false);
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
