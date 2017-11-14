using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EstatuaCtrl : MonoBehaviour {

	public GameObject estatuonaOrb;
	public GameObject thisOrb;

	private bool activated = false;

	void Start(){
		estatuonaOrb.SetActive (activated);
		thisOrb.SetActive (activated);
	}

//	void Update(){
//		if (!activated)
//			return;
//
//
//	}

	void OnTriggerEnter(Collider col){
		if(col.CompareTag("Player")){
			activated = true;
			estatuonaOrb.SetActive (activated);
			thisOrb.SetActive (activated);
		}
	}
}
