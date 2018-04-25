using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonMushroom_Ctrl : MonoBehaviour {

	public float jumpForce = 10f;

	public float venenoInitialRadius = 0.5f;
	public float venenoMaxRadius = 2.5f;
	public bool venenoCanGrow = true;
	public bool venenoCanDissipate = true;

	WalkingController player;
	public GameObject venenoPrefab;

	private Animator mushAnimCtrl;

	void Awake(){
		mushAnimCtrl = GetComponentInParent<Animator> ();
		player = FindObjectOfType<WalkingController> ();
	}

	void OnTriggerEnter(Collider col){
		if(col.CompareTag("Player")){
			//player.externalForceAdded = true;
			//col.GetComponentInParent<AudioSource> ().Play ();
			Vector3 dir = col.transform.up * jumpForce;
			player.AddExternalForce (dir, 0.01f);
			mushAnimCtrl.SetTrigger ("boing");
			GameObject obj = Instantiate (venenoPrefab, transform.parent);
			VenenoCtrl objCtrl = obj.GetComponent<VenenoCtrl> ();
			objCtrl.canDissipate = venenoCanDissipate;
			objCtrl.canGrow = venenoCanGrow;
			objCtrl.initialRadius = venenoInitialRadius;
			objCtrl.maxRadius = venenoMaxRadius;
		}
	}
}
