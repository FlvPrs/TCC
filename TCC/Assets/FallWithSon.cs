using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallWithSon : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	void OnTriggerEnter(Collider colisor){
		if(colisor.name == "PlayerCollider"){
			gameObject.transform.SetParent (colisor.transform);
			//gameObject.GetComponent<CapsuleCollider> ().isTrigger = true;
		}
	}
}
