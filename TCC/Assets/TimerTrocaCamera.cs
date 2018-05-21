using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerTrocaCamera : MonoBehaviour {
	ChangeCamOnTrigger trocaCamera;
	CamPriorityController camCtrl;
	public int newCamIndex;
	public bool exitAsDifferentCam;
	public int exitCamIndex;
	private int oldIndex;
	// Use this for initialization
	void Start () {
		//camCtrl.ChangeCameraTo (newCamIndex);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
