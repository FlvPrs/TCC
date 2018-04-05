using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalancoCtrl : MonoBehaviour {

	private float originalRot_X;
	private float originalRot_Z;
	private Vector3 original_Up;

	private Transform player;

	void Update(){
		if (player == null)
			return;

		//player.up = transform.up;

		//player.eulerAngles = new Vector3 (originalRot_X, player.eulerAngles.y, originalRot_Z);
	}

	void OnTriggerEnter(Collider col){
		if(col.CompareTag("Player")){
			originalRot_X = col.transform.eulerAngles.x;
			originalRot_Z = col.transform.eulerAngles.z;
			original_Up = col.transform.up;
			player = col.transform;
			player.SetParent (transform);
		}
	}

	void OnTriggerExit(Collider col){
		if(col.CompareTag("Player")){
			player.SetParent (null);
			player.eulerAngles = new Vector3 (originalRot_X, col.transform.eulerAngles.y, originalRot_Z);
			player = null;
		}
	}
}
