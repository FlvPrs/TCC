using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SustainInteractionsCtrl : MonoBehaviour {

	[HideInInspector]
	public HeightState currentHeight = HeightState.Default;

	void OnTriggerEnter(Collider col){
		
	}

	void OnTriggerStay(Collider col){
		if(col.GetComponent<ISustainInteractable> () != null)
			col.GetComponent<ISustainInteractable> ().Interact (currentHeight);
	}

	void OnTriggerExit(Collider col){

	}
}
