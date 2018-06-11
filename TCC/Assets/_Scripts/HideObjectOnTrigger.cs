using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideObjectOnTrigger : MonoBehaviour {

	public bool showInsteadOfHide;
	public GameObject obj;
	public bool disableChangeCam;
	public ChangeCamOnTrigger component;

	public string collTag;


//	void Start(){
//		obj.SetActive (!showInsteadOfHide);
//	}

	void OnTriggerEnter(Collider col){
		if(col.CompareTag(collTag)){
			if (disableChangeCam)
				component.enabled = false;
			obj.SetActive (showInsteadOfHide);
		}
	}
}
