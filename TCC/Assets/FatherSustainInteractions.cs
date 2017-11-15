using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FatherSustainInteractions : MonoBehaviour {

//	[HideInInspector]
//	public HeightState currentHeight = HeightState.Default;
	[HideInInspector]
	public bool canAdvance = false;

	void OnTriggerEnter(Collider col){

	}

	void OnTriggerStay(Collider col){
		canAdvance = col.GetComponent<IFatherInteractable> ().FatherInteraction ();
	}

	void OnTriggerExit(Collider col){

	}
}
