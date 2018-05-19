using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayBGM : MonoBehaviour {

	[FMODUnity.EventRef]
	public string audioIntro, audioTrilha;
	FMOD.Studio.EventInstance musicaIntro, musicaTema;
	public FMOD.Studio.PLAYBACK_STATE playingIntro;
	public FMOD.Studio.PLAYBACK_STATE playingTema;

	// Use this for initialization
	void Start () {
		// fmod
		audioTrilha = "event:/Musica/Muzika";
		musicaTema = FMODUnity.RuntimeManager.CreateInstance (audioTrilha);
	}
	
	// Update is called once per frame
	void Update () {
		if (playingTema != FMOD.Studio.PLAYBACK_STATE.PLAYING) {
			musicaTema.start ();
		}
	}
}
