using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CogumeloJumpCtrl : MonoBehaviour {

	public float jumpForce = 10f;

	void OnTriggerEnter(Collider col){
		if(col.CompareTag("Player")){
			col.GetComponent<WalkingController>().externalForceAdded = true;
			//col.GetComponentInParent<AudioSource> ().Play ();
			Vector3 dir = col.transform.up * jumpForce;
			col.GetComponent<WalkingController>().AddExternalForce (dir, 0.5f);
		}
	}
}
