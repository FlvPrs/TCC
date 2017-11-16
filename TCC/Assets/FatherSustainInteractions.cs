using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FatherSustainInteractions : MonoBehaviour {

//	[HideInInspector]
//	public HeightState currentHeight = HeightState.Default;
	[HideInInspector]
	public bool canAdvance = false;
	[HideInInspector]
	public bool stopSing = false;

	void OnTriggerEnter(Collider col){

	}

	void OnTriggerStay(Collider col){
		canAdvance = col.GetComponent<IFatherInteractable> ().FatherInteraction (out stopSing);
	}

	void OnTriggerExit(Collider col){

	}
}
