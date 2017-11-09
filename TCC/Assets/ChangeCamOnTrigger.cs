using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCamOnTrigger : MonoBehaviour {

	public CamPriorityController camCtrl;
	public int newCamIndex;
	public bool exitAsDifferentCam;
	public int exitCamIndex;
	private int oldIndex;

	void OnTriggerEnter(Collider col){
		if(col.CompareTag("Player")){
			oldIndex = camCtrl.currentCam;
			camCtrl.ChangeCameraTo (newCamIndex);
		}
	}

	void OnTriggerExit(Collider col){
		if (camCtrl.currentCam != newCamIndex)
			return;

		if(exitAsDifferentCam)
			camCtrl.ChangeCameraTo (exitCamIndex);
		else
			camCtrl.ChangeCameraTo (oldIndex);
	}
}
