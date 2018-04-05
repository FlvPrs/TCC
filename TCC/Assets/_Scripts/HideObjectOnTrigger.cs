using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideObjectOnTrigger : MonoBehaviour {

	public bool showInsteadOfHide;
	public GameObject obj;
	public string collTag;

	void Start(){
		obj.SetActive (!showInsteadOfHide);
	}

	void OnTriggerEnter(Collider col){
		if(col.CompareTag(collTag)){
			obj.SetActive (showInsteadOfHide);
		}
	}
}
