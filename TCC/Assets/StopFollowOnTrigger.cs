using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopFollowOnTrigger : MonoBehaviour {

	public NPC_Tartaruga turtle;

	void OnTriggerStay (Collider col){
		turtle.StopFollow ();
	}
}
