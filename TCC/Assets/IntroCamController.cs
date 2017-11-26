﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


public class IntroCamController : MonoBehaviour {

	[NoSaveDuringPlay]
	public bool activateStartCam = true;
	[NoSaveDuringPlay]
	public WalkingController playerCtrl;
	private float defaultSpeed;

	private CinemachineTrackedDolly camTrack;
	public float duration = 300f;
	private bool towards;
	private float startTime;

	private bool startGame;

	public GameObject pressButtonTxt;

	[HideInInspector]
	public static bool playerRegainedCtrl;

	// Use this for initialization
	void Start () {
		playerRegainedCtrl = false;

		if(activateStartCam)
			pressButtonTxt.SetActive (true);
		else
			pressButtonTxt.SetActive (false);

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

		defaultSpeed = playerCtrl.walkSpeed;
	}
	
	// Update is called once per frame
	void Update () {
		playerCtrl.walkSpeed = 0;

		if (playerCtrl.playerInputStartGame) {
			activateStartCam = false;
			pressButtonTxt.SetActive (false);
		}
		
		if (activateStartCam) {
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
				playerCtrl.walkSpeed = defaultSpeed;
				enabled = false;
			}
			else
				camTrack.m_PathPosition = Mathf.SmoothStep (camTrack.m_PathPosition, 0f, t);
		}
	}
}