using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: Substituir isto por um sistema de save decente.
public class EnableColliderOnTrigger : MonoBehaviour {

	public Collider col;

	void Start(){
		col.enabled = false;
	}

	void OnTriggerEnter(Collider col){
		col.enabled = true;
		enabled = false;
	}
}
