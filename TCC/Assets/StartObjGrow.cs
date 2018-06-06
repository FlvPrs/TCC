using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartObjGrow : MonoBehaviour {

	public GrowObj[] objsToGrow;

	void OnTriggerEnter (Collider col) {
		if(col.CompareTag("Player")){
			for (int i = 0; i < objsToGrow.Length; i++) {
				objsToGrow [i].startGrow = true;
			}
			enabled = false;
		}
	}
}
