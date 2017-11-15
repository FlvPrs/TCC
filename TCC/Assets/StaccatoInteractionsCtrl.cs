using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaccatoInteractionsCtrl : MonoBehaviour {

	[HideInInspector]
	public int partitura = 000;

	void OnTriggerEnter(Collider col){
		col.GetComponent<IStaccatoInteractable> ().Interact (partitura);
	}

	void OnTriggerStay(Collider col){

	}

	void OnTriggerExit(Collider col){

	}
}
