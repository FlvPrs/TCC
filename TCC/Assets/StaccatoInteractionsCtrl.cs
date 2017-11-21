using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaccatoInteractionsCtrl : MonoBehaviour {

	[HideInInspector]
	public int partitura = 000;
	[HideInInspector]
	public HeightState currentHeight = HeightState.Default;

	void OnTriggerEnter(Collider col){
		col.GetComponent<IStaccatoInteractable> ().Interact (currentHeight, partitura);
	}

	void OnTriggerStay(Collider col){

	}

	void OnTriggerExit(Collider col){

	}
}
