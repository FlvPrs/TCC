using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Tartaruga : NPCBehaviour {

	AudioSource sustainAudioSource;

	bool isPlaying;

	// Use this for initialization
	protected override void Awake () {
		base.Awake ();

		sustainAudioSource = GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	protected override void Update () {
		base.Update ();

		animCtrl.SetBool ("isWalking", (nmAgent.velocity != Vector3.zero));

		if(currentState == NPC_CurrentState.Seguindo){
			if (!isPlaying) {
				isPlaying = true;
				sustainAudioSource.Play ();
			}
		} else if (isPlaying) {
			isPlaying = false;
			sustainAudioSource.Stop ();
		}

		//print (isPlaying);
	}
}
