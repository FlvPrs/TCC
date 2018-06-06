using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillFatherOnTrigger : MonoBehaviour {

	bool startFatherDeath;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter (Collider col){
		if(col.GetComponent<FatherActions>() != null){
			FatherActions father = col.GetComponent<FatherActions> ();
			father.StopHug ();
			father.stopUpdate = true;
			col.GetComponent<UnityEngine.AI.NavMeshAgent> ().enabled = false;
			father.animCtrl.SetBool ("isDying", true);
		}
	}
}
