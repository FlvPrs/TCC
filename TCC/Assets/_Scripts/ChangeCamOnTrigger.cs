using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCamOnTrigger : MonoBehaviour {

	public CamPriorityController camCtrl;
	public int newCamIndex;
	public bool exitAsDifferentCam;
	public int exitCamIndex;
	private int oldIndex;

	public bool change_LookAt;
	public Transform newLookAt_Ref;
	public bool lookBeyondPoint;
	public Transform beyondLookAt_Obj;

	void OnTriggerEnter(Collider col){
		if(col.CompareTag("Player")){
			if (change_LookAt) {
				if (exitAsDifferentCam) {
					oldIndex = camCtrl.currentCam;
					camCtrl.ChangeCameraTo (newCamIndex);
				}

				if(lookBeyondPoint){
					beyondLookAt_Obj.position = newLookAt_Ref.position;
					beyondLookAt_Obj.position += newLookAt_Ref.position - camCtrl.CM_Vcams [camCtrl.currentCam].transform.position;
					camCtrl.CM_Vcams [camCtrl.currentCam].LookAt = beyondLookAt_Obj;
				} else {
					camCtrl.CM_Vcams [camCtrl.currentCam].LookAt = newLookAt_Ref;
				}
			} else {
				oldIndex = camCtrl.currentCam;
				camCtrl.ChangeCameraTo (newCamIndex);
			}
		}
	}

	void OnTriggerStay(Collider col){
		if(col.CompareTag("Player") && !change_LookAt){
			camCtrl.ChangeCameraTo (newCamIndex);
		}
	}

	void OnTriggerExit(Collider col){
		if (camCtrl.currentCam != newCamIndex || change_LookAt)
			return;

		if(exitAsDifferentCam)
			camCtrl.ChangeCameraTo (exitCamIndex);
		else
			camCtrl.ChangeCameraTo (oldIndex);
	}
}
