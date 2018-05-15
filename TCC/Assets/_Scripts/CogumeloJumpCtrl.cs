using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CogumeloJumpCtrl : MonoBehaviour {

	public float jumpForce = 10f;

	public WalkingController player;

	private Animator mushAnimCtrl;

	public AudioClip[] boing_Clips;
	AudioSource simpleAudioSource;
	int clipIndex = 0;

	void Awake(){
		mushAnimCtrl = GetComponentInParent<Animator> ();
		player = FindObjectOfType<WalkingController> ();
		simpleAudioSource = GetComponent<AudioSource> ();
	}

	void OnTriggerEnter(Collider col){
		if (col.CompareTag ("Player")) {
			simpleAudioSource.clip = boing_Clips [clipIndex];
			simpleAudioSource.Play ();
			clipIndex++;
			if (clipIndex >= boing_Clips.Length)
				clipIndex = 0;
			//player.externalForceAdded = true;
			//col.GetComponentInParent<AudioSource> ().Play ();
			Vector3 dir = col.transform.up * jumpForce;
			player.AddExternalForce (dir, 0.01f);
			mushAnimCtrl.SetTrigger ("boing");
		}
	}
}
