using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FatherChangeDisposition_Trigger : MonoBehaviour {

	public FatherConditions condition;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter (Collider col){
		if(col.GetComponent<Father_DebilitadoCtrl>() != null){
			col.GetComponent<Father_DebilitadoCtrl> ().currentDisposition = condition;
			gameObject.SetActive (false);
		}
	}
}
