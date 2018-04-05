using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FMODMusicaCtrl : MonoBehaviour {
	
	[FMODUnity.EventRef]

	public string Musica;
	FMOD.Studio.EventInstance audioMusica;

	void Start () {
		Musica = "event:/Musica/Full";
		audioMusica = FMODUnity.RuntimeManager.CreateInstance (Musica);

	}
	

	void Update () {
		
	}
}
