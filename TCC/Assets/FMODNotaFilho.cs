using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FMODNotaFilho : MonoBehaviour {

	public WalkingController nota;

	[FMODUnity.EventRef]
	FMOD.Studio.EventInstance TubaFilhoEvent;
	FMOD.Studio.ParameterInstance SustainNoteParameter;

	bool isTocando = false;

	void Start(){		
		
	
		TubaFilhoEvent = FMODUnity.RuntimeManager.CreateInstance ("event:/TubaFilho");
		TubaFilhoEvent.getParameter ("SustainNote", out SustainNoteParameter);
	
		TubaFilhoEvent.set3DAttributes (FMODUnity.RuntimeUtils.To3DAttributes(gameObject, GetComponent<Rigidbody>()));
		FMODUnity.RuntimeManager.AttachInstanceToGameObject(TubaFilhoEvent, transform, GetComponent<Rigidbody>());
	}

	void Update ()
	{
		atualizaNotaFilho (nota);
		//SustainNoteParameter.setValue(atualizaNotaFilho(nota));
	}

	public void atualizaNotaFilho (WalkingController atua){

		if (atua.walkStates.TOCANDO_STACCATO && !isTocando) {
			SustainNoteParameter.setValue (0.49f);
			TubaFilhoEvent.start ();
			isTocando = true;
			StartCoroutine ("TestSegurando", atua);
			print ("IzComeçanu");
		} else if (!atua.walkStates.TOCANDO_STACCATO && isTocando) {
			TubaFilhoEvent.stop (FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
			isTocando = false;
			StopAllCoroutines ();
			print ("LogoÇoltou");
		}
	}
		
	IEnumerator TestSegurando(WalkingController atua){
		yield return new WaitForSeconds (0.3f);

		while (true) {
			if (atua.walkStates.TOCANDO_SUSTAIN) {
				SustainNoteParameter.setValue (1f);
				print ("Apertanu");
				yield return new WaitForSeconds (0.2f);
			} else {
				print ("Çoltou");
				SustainNoteParameter.setValue (0.75f);
				yield return null;
			}
		}
	}
}
