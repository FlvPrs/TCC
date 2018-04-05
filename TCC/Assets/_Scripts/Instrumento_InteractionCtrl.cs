using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instrumento_InteractionCtrl : MonoBehaviour, IStaccatoInteractable {

	public HeightState pitch;
	private bool canInteract;

	[HideInInspector]
	public bool interactionDone;

	public ParticleSystem particle_Yay, particle_Nay;

	private InstrumentoCtrl instrumentoCtrl;

	// Use this for initialization
	void Start () {
		instrumentoCtrl = GetComponentInParent<InstrumentoCtrl> ();
	}

	public void Interact (HeightState height){
		if(canInteract){
			interactionDone = true;

			bool correctPitch = false;
			if(height == pitch)
				correctPitch = true;

			instrumentoCtrl.UpdateInstrumento (correctPitch);
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