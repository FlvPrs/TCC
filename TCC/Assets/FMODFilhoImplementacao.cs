using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FMODFilhoImplementacao : MonoBehaviour {

	public WalkingController feedbackBt;

	public bool canStartSustain;
	public bool canStartStaccato;

	[FMODUnity.EventRef]

	public string FilhoAguda, FilhoMedia, FilhoGrave, FilhoAguda2, FilhoMedia2, FilhoGrave2;
	FMOD.Studio.EventInstance filhoMelodyAguda, filhoMelodyMedia, filhoMelodyGrave, filhoMelodyAguda2, filhoMelodyMedia2, filhoMelodyGrave2;




	void Start () {
		
		FilhoAguda = "event:/Filho RB/Filho Nota Padrao Aguda";
		FilhoMedia = "event:/Filho RB/Filho Nota Padrao Media";
		FilhoGrave = "event:/Filho RB/Filho Nota Padrao Grave";
		FilhoAguda2 = "event:/Filho RT/Filho Floreio Agudo";
		FilhoMedia2 = "event:/Filho RT/Filho Floreio Medio";
		FilhoGrave2 = "event:/Filho RT/Filho Floreio Grave";

		filhoMelodyAguda = FMODUnity.RuntimeManager.CreateInstance (FilhoAguda);
		filhoMelodyMedia = FMODUnity.RuntimeManager.CreateInstance (FilhoMedia);
		filhoMelodyGrave = FMODUnity.RuntimeManager.CreateInstance (FilhoGrave);
		filhoMelodyAguda2 = FMODUnity.RuntimeManager.CreateInstance (FilhoAguda2);
		filhoMelodyMedia2 = FMODUnity.RuntimeManager.CreateInstance (FilhoMedia2);
		filhoMelodyGrave2 = FMODUnity.RuntimeManager.CreateInstance (FilhoGrave2);


	}
	
	// Update is called once per frame
	void Update () {
		VerificaNota ();
		VerificaNota2 ();

		filhoMelodyMedia.set3DAttributes (FMODUnity.RuntimeUtils.To3DAttributes(gameObject, GetComponent<Rigidbody>()));
		FMODUnity.RuntimeManager.AttachInstanceToGameObject(filhoMelodyMedia, transform, GetComponent<Rigidbody>());

		filhoMelodyAguda.set3DAttributes (FMODUnity.RuntimeUtils.To3DAttributes(gameObject, GetComponent<Rigidbody>()));
		FMODUnity.RuntimeManager.AttachInstanceToGameObject(filhoMelodyAguda, transform, GetComponent<Rigidbody>());

		filhoMelodyGrave.set3DAttributes (FMODUnity.RuntimeUtils.To3DAttributes(gameObject, GetComponent<Rigidbody>()));
		FMODUnity.RuntimeManager.AttachInstanceToGameObject(filhoMelodyGrave, transform, GetComponent<Rigidbody>());

		filhoMelodyMedia2.set3DAttributes (FMODUnity.RuntimeUtils.To3DAttributes(gameObject, GetComponent<Rigidbody>()));
		FMODUnity.RuntimeManager.AttachInstanceToGameObject(filhoMelodyMedia2, transform, GetComponent<Rigidbody>());

		filhoMelodyAguda2.set3DAttributes (FMODUnity.RuntimeUtils.To3DAttributes(gameObject, GetComponent<Rigidbody>()));
		FMODUnity.RuntimeManager.AttachInstanceToGameObject(filhoMelodyAguda2, transform, GetComponent<Rigidbody>());

		filhoMelodyGrave2.set3DAttributes (FMODUnity.RuntimeUtils.To3DAttributes(gameObject, GetComponent<Rigidbody>()));
		FMODUnity.RuntimeManager.AttachInstanceToGameObject(filhoMelodyGrave2, transform, GetComponent<Rigidbody>());

	}

	public void VerificaNota()
	{
		if (!feedbackBt.walkStates.TOCANDO_NOTAS) {
			canStartStaccato = true;
		} else if (feedbackBt.walkStates.TOCANDO_NOTAS && !feedbackBt.walkStates.SEGURANDO_NOTA && canStartStaccato) {
			canStartStaccato = false;

			FMOD.Studio.PLAYBACK_STATE playing1;
			filhoMelodyMedia.getPlaybackState (out playing1);

			FMOD.Studio.PLAYBACK_STATE playing2;
			filhoMelodyAguda.getPlaybackState (out playing2);

			FMOD.Studio.PLAYBACK_STATE playing3;
			filhoMelodyGrave.getPlaybackState (out playing3);
			
			switch (feedbackBt.walkStates.CURR_HEIGHT_STATE) {

			case HeightState.Default:				
				//if (playing2 != FMOD.Studio.PLAYBACK_STATE.PLAYING && playing1 != FMOD.Studio.PLAYBACK_STATE.PLAYING && playing3 != FMOD.Studio.PLAYBACK_STATE.PLAYING) {
					filhoMelodyMedia.start ();
				//}

				//print ("tocando media");
				break;
			case HeightState.High:				

			//	if (playing2 != FMOD.Studio.PLAYBACK_STATE.PLAYING && playing1 != FMOD.Studio.PLAYBACK_STATE.PLAYING && playing3 != FMOD.Studio.PLAYBACK_STATE.PLAYING) {
					filhoMelodyAguda.start ();
			//	}

				//print ("tocando aguda");

				break;
			case HeightState.Low:
				
				//if (playing2 != FMOD.Studio.PLAYBACK_STATE.PLAYING && playing1 != FMOD.Studio.PLAYBACK_STATE.PLAYING && playing3 != FMOD.Studio.PLAYBACK_STATE.PLAYING) {
					filhoMelodyGrave.start ();
				//}
				//print ("tocando grave");
				break;
			default:
				print ("fez mierda");
				break;
			}
		}
	}


	public void VerificaNota2()	{
		if (!feedbackBt.walkStates.SEGURANDO_NOTA) {
			canStartSustain = true;
		} else if (feedbackBt.walkStates.SEGURANDO_NOTA && canStartSustain) {
			canStartSustain = false;

			FMOD.Studio.PLAYBACK_STATE playing4;
			filhoMelodyMedia2.getPlaybackState (out playing4);

			FMOD.Studio.PLAYBACK_STATE playing5;
			filhoMelodyAguda2.getPlaybackState (out playing5);

			FMOD.Studio.PLAYBACK_STATE playing6;
			filhoMelodyGrave2.getPlaybackState (out playing6);

			switch (feedbackBt.walkStates.CURR_HEIGHT_STATE) {

			case HeightState.Default:				
				//if (playing2 != FMOD.Studio.PLAYBACK_STATE.PLAYING && playing1 != FMOD.Studio.PLAYBACK_STATE.PLAYING && playing3 != FMOD.Studio.PLAYBACK_STATE.PLAYING) {
				filhoMelodyMedia2.start ();
				//}

				print ("tocando media2");
				break;
			case HeightState.High:				

				//	if (playing2 != FMOD.Studio.PLAYBACK_STATE.PLAYING && playing1 != FMOD.Studio.PLAYBACK_STATE.PLAYING && playing3 != FMOD.Studio.PLAYBACK_STATE.PLAYING) {
				filhoMelodyAguda2.start ();
				//	}

				print ("tocando aguda2");

				break;
			case HeightState.Low:

				//if (playing2 != FMOD.Studio.PLAYBACK_STATE.PLAYING && playing1 != FMOD.Studio.PLAYBACK_STATE.PLAYING && playing3 != FMOD.Studio.PLAYBACK_STATE.PLAYING) {
				filhoMelodyGrave2.start ();
				//}
				print ("tocando grave2");
				break;
			default:
				print ("fez mierda2");
				break;
			}
		}
	}

}