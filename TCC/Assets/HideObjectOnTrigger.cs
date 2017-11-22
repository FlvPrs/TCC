using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideObjectOnTrigger : MonoBehaviour {

	public GameObject obj;
	public string collTag;

	void OnTriggerEnter(Collider col){
		if(col.CompareTag(collTag)){
			obj.SetActive (false);
		}
	}
}
