using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


public class IntroCamController : MonoBehaviour {

	public quedaCollider fallctrl;

	[FMODUnity.EventRef]
	public string audioIntro, audioTrilha;
	FMOD.Studio.EventInstance musicaIntro, musicaTema;
	public FMOD.Studio.PLAYBACK_STATE playingIntro;
	public FMOD.Studio.PLAYBACK_STATE playingTema;


	[NoSaveDuringPlay]
	public bool activateStartCam = true;
	[NoSaveDuringPlay]
	public WalkingController playerCtrl;

	private CinemachineTrackedDolly camTrack;
	public float duration = 300f;
	private bool towards;
	private float startTime;

	private bool startGame;

	//public GameObject pressButtonTxt;

	[HideInInspector]
	public static bool playerRegainedCtrl;

	// Use this for initialization
	void Start () {
		// fmod

		audioIntro = "event:/Musica/Tema 1/Intro";
		musicaIntro = FMODUnity.RuntimeManager.CreateInstance (audioIntro);

		audioTrilha = "event:/Musica/Tema 1/Muzika";
		musicaTema = FMODUnity.RuntimeManager.CreateInstance (audioTrilha);

		playerRegainedCtrl = false;

//		if(activateStartCam)
//			pressButtonTxt.SetActive (true);
//		else
//			pressButtonTxt.SetActive (false);

		camTrack = GetComponent<CinemachineVirtualCamera> ().GetCinemachineComponent<CinemachineTrackedDolly>();
		camTrack.m_PathPosition = 1f;

		startTime = Time.time;

		if (!activateStartCam) {
			GetComponent<CinemachineVirtualCamera> ().m_Priority = 0;
			enabled = false;
		}
		else {
			GetComponent<CinemachineVirtualCamera> ().m_Priority = 99;
		}
			
		playerCtrl.playerCanMove = false;
	}
	
	// Update is called once per frame
	void Update () {

		musicaTema.getPlaybackState (out playingTema);
		//////print ("muzika tema iz " + playingTema);
		if ( fallctrl.af == true){
			////print ("o af é igual a: " + fallctrl.af); 
			musicaTema.stop (FMOD.Studio.STOP_MODE.IMMEDIATE);
			////print ("parou");

		}

		if (playerCtrl.playerInputStartGame) {
			musicaIntro.stop (FMOD.Studio.STOP_MODE.IMMEDIATE);

			activateStartCam = false;
			//pressButtonTxt.SetActive (false);



			if (playingTema != FMOD.Studio.PLAYBACK_STATE.PLAYING && !activateStartCam && fallctrl.af == false) {
				musicaTema.start ();
			}



		}
		
		if (activateStartCam) {
			musicaIntro.getPlaybackState (out playingIntro);

			if (playingIntro != FMOD.Studio.PLAYBACK_STATE.PLAYING) {
				musicaIntro.start ();
			}

			if (camTrack.m_PathPosition >= 1.9f && towards) {
				startTime = Time.time;
				towards = false;
			} else if (camTrack.m_PathPosition <= 1.1f && !towards) {
				startTime = Time.time;
				towards = true;
			}

			float t = (Time.time - startTime) / duration;

			if (towards) {
				camTrack.m_PathPosition = Mathf.SmoothStep (camTrack.m_PathPosition, 2f, t);
			} else {
				camTrack.m_PathPosition = Mathf.SmoothStep (camTrack.m_PathPosition, 1f, t);
			}
		} else {
			if(!startGame){
				startGame = true;
				startTime = Time.time;
			}

			float t = (Time.time - startTime) / duration * 10f;

			if (camTrack.m_PathPosition <= 0.1f) {
				GetComponent<CinemachineVirtualCamera> ().m_Priority = 0;
				playerRegainedCtrl = true;
				playerCtrl.playerCanMove = true;
				//enabled = false;
			}
			else
				camTrack.m_PathPosition = Mathf.SmoothStep (camTrack.m_PathPosition, 0f, t);
		}

//		if(!playerCtrl.playerInputStartGame && Input.anyKeyDown)
//			playerCtrl.playerInputStartGame = true;
	}

	void OnDisable (){
		musicaTema.stop (FMOD.Studio.STOP_MODE.IMMEDIATE);
		musicaIntro.stop (FMOD.Studio.STOP_MODE.IMMEDIATE);
	}
}