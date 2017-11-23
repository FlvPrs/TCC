using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FatherRemoteWPAdvance : MonoBehaviour {

	public FatherPath father;

	void OnTriggerEnter(Collider col){
		if(col.CompareTag("Player")){
			if (father.state == FatherPath.FSMStates.Idle || father.state == FatherPath.FSMStates.WaypointBehaviour) {
				father.state = FatherPath.FSMStates.Path;
				gameObject.SetActive (false);
			} else {
				StartCoroutine ("WaitForAgentStop");
			}
		}
	}

	void OnTriggerStay(Collider col){
		if(col.gameObject == father.gameObject){
			if (father.state == FatherPath.FSMStates.Idle || father.state == FatherPath.FSMStates.WaypointBehaviour) {
				father.state = FatherPath.FSMStates.Path;
				gameObject.SetActive (false);
			}
//			else {
//				StartCoroutine ("WaitForAgentStop");
//			}
		}
	}

	IEnumerator WaitForAgentStop(){
		yield return new WaitForSeconds (0.5f);

		while(father.state != FatherPath.FSMStates.Idle && father.state != FatherPath.FSMStates.WaypointBehaviour){
			yield return new WaitForSeconds (0.5f);
		}

		father.state = FatherPath.FSMStates.Path;
		gameObject.SetActive (false);
	}
}
