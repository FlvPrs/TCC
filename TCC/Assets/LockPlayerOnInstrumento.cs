using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockPlayerOnInstrumento : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter (Collider col){
		if(col.CompareTag("Player")){
			col.transform.parent.position = transform.position;
			//col.GetComponentInParent<WalkingController>().SetVelocityTo (Vector3.zero, true);
			FindObjectOfType<PlayerWalkInput> ().disableMovement = true;
		}
	}
	void OnTriggerExit (Collider col){
		if(col.CompareTag("Player")){
			FindObjectOfType<PlayerWalkInput> ().disableMovement = false;
		}
	}
}
