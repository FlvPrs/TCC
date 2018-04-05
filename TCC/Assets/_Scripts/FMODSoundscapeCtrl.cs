using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;

public class FMODSoundscapeCtrl : MonoBehaviour {

	public Rigidbody filhochato;
	public Transform filhochato2;

	[FMODUnity.EventRef]

	public string VentoBrisa;
	FMOD.Studio.EventInstance AudioBrisa;
	// Use this for initialization
	void Start () {
		VentoBrisa = "event:/Ambiente FX/Vento Geral";
		AudioBrisa = FMODUnity.RuntimeManager.CreateInstance (VentoBrisa);
		//FMODUnity.RuntimeManager.PlayOneShot ("event:/Vento Geral", transform.position);


	}


	
	// Update is called once per frame
	void Update () {



		FMOD.Studio.PLAYBACK_STATE playing;
		AudioBrisa.getPlaybackState (out playing);
		if (playing != FMOD.Studio.PLAYBACK_STATE.PLAYING) {	
			

			AudioBrisa.start ();		

			AudioBrisa.set3DAttributes (FMODUnity.RuntimeUtils.To3DAttributes(transform, GetComponent<Rigidbody>()));
			//FMODUnity.RuntimeManager.AttachInstanceToGameObject(AudioBrisa, filhochato2, filhochato);


		}

		
	}
}
