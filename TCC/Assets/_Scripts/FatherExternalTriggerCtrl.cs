using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FatherExternalTriggerCtrl : MonoBehaviour {

	public FatherStates onlyChangeFromState;
	public bool anyState = true;

	void OnTriggerEnter (Collider col){
		if (col.CompareTag("NPC_Pai")) {
			if (anyState || col.GetComponent<FatherFSM> ().currentState == onlyChangeFromState) {
				col.GetComponent<FatherFSM> ().externalTriggerActivated = true;
			}
		}
	}
}
