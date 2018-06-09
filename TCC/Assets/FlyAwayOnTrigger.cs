using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FlyWithWind))]
public class FlyAwayOnTrigger : MonoBehaviour {

	public bool onEnter, onExit;

	void OnTriggerEnter (Collider col){
		if (!onEnter)
			return;

		if (col.CompareTag ("Player")) {
			GetComponent<FlyWithWind> ().startFlyAway = true;
			enabled = false;
		}
	}
	void OnTriggerExit (Collider col){
		if (!onExit)
			return;

		if (col.CompareTag ("Player")) {
			GetComponent<FlyWithWind> ().startFlyAway = true;
			enabled = false;
		}
	}
}
