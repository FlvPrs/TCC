using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FatherExternalTriggerCtrl : MonoBehaviour {

	public FatherStates onlyChangeFromState;
	public bool anyState = true;
	public bool onlyPlayerCanTrigger;
	public bool dontDisableAfterHit;


	void OnTriggerEnter (Collider col){
		if (!onlyPlayerCanTrigger && (col.CompareTag ("NPC_Pai") || col.CompareTag ("PaiDebilitado"))) {
			if (anyState || col.GetComponent<FatherFSM> ().currentState == onlyChangeFromState) {
				col.GetComponent<FatherFSM> ().externalTriggerActivated = true;
				gameObject.SetActive (dontDisableAfterHit);
			}
		}
		else if (onlyPlayerCanTrigger && col.CompareTag("Player")) {
			FatherFSM father = GameObject.FindObjectOfType<FatherFSM> ();
			if (anyState || father.currentState == onlyChangeFromState) {
				father.externalTriggerActivated = true;
				gameObject.SetActive (dontDisableAfterHit);
			}
		}
	}

	void OnTriggerExit (Collider col){
		if (!onlyPlayerCanTrigger && (col.CompareTag ("NPC_Pai") || col.CompareTag ("PaiDebilitado"))) {
			if (anyState || col.GetComponent<FatherFSM> ().currentState == onlyChangeFromState) {
				col.GetComponent<FatherFSM> ().externalTriggerActivated = true;
				gameObject.SetActive (dontDisableAfterHit);
			}
		}
		else if (onlyPlayerCanTrigger && col.CompareTag("Player")) {
			FatherFSM father = GameObject.FindObjectOfType<FatherFSM> ();
			if (anyState || father.currentState == onlyChangeFromState) {
				father.externalTriggerActivated = true;
				gameObject.SetActive (dontDisableAfterHit);
			}
		}
	}
}
