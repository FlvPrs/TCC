using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FatherSing_Grow : MonoBehaviour {

	public Transform targetToGrow;
	public AudioSource sing;
	public GameObject forceChangeWP;

	private bool startGrow = false;
	private Quaternion final;

	void Start(){
		final = Quaternion.Euler(Vector3.zero);
	}

	void Update () {
		if(startGrow){
			targetToGrow.rotation = Quaternion.Slerp (targetToGrow.rotation, final, Time.deltaTime);

			if (targetToGrow.localEulerAngles.z >= 357f) {
				sing.Stop ();
				forceChangeWP.SetActive (true);
			}

			if (targetToGrow.localEulerAngles.z >= 359.95f) {
				gameObject.SetActive (false);
			}
		}
	}

	void OnTriggerEnter(Collider col){
		sing.Play();
		startGrow = true;
	}
}
