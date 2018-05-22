using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetAnimBool : MonoBehaviour {

	public Animator animCtrl;
	public string boolName;
	public bool setBoolTo;
	public bool setTrigger;
	public string collTag;
	public bool setOnAwake;

	void Awake (){
		if(setOnAwake){
			if(!setTrigger){
				animCtrl.SetBool (boolName, setBoolTo);
			} else {
				animCtrl.SetTrigger (boolName);
			}
			enabled = false;
		}
	}

	void Start (){
		
	}

	void OnTriggerEnter (Collider col){
		if(col.CompareTag(collTag) && !setOnAwake){
			if(!setTrigger){
				animCtrl.SetBool (boolName, setBoolTo);
			} else {
				animCtrl.SetTrigger (boolName);
			}
		}
	}
}
