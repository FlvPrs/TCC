using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleChangeTriggerCollider : MonoBehaviour {

	//public Transform[] triggerColliders; //Shelters colliders.
	public string triggerTag;
	ParticleSystem.TriggerModule psTrigger;
	List<Collider> colliders;

	void Start () {
		psTrigger = GetComponentInChildren<ParticleSystem> ().trigger;
		colliders = new List<Collider> ();
	}

	void Update () {
		for (int i = 0; i < psTrigger.maxColliderCount; i++) {
			if (i < colliders.Count && colliders [i] != null)
				psTrigger.SetCollider (i, colliders [i]);
			else
				psTrigger.SetCollider(i, null);
		}
	}

	void OnTriggerEnter(Collider col){
		if(col.CompareTag(triggerTag) && !colliders.Contains(col.GetComponent<Collider> ())){
			colliders.Add (col.GetComponent<Collider> ());
		}
	}
	void OnTriggerExit(Collider col){
		if(col.CompareTag(triggerTag) && colliders.Contains(col.GetComponent<Collider> ())){
			colliders.Remove (col.GetComponent<Collider> ());
		}
	}
}
