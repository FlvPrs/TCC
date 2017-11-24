using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CogumeloJumpCtrl : MonoBehaviour {

	public float jumpForce = 10f;

	public WalkingController player;

	void OnTriggerEnter(Collider col){
		if(col.CompareTag("Player")){
			player.externalForceAdded = true;
			//col.GetComponentInParent<AudioSource> ().Play ();
			Vector3 dir = col.transform.up * jumpForce;
			player.AddExternalForce (dir, 0.5f);
		}
	}
}
