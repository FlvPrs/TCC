using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCamOnTrigger : MonoBehaviour {

	public CamPriorityController camCtrl;
	public int newCamIndex, newCamIndexTimer;
	public bool exitAsDifferentCam;
	public int exitCamIndex;
	private int oldIndex;

	public bool change_LookAt;
	public Transform newLookAt_Ref;
	public bool lookBeyondPoint;
	public Transform beyondLookAt_Obj;
	public bool tempo, activator;
	public float changeCamTimerInicial;
	private float changeCamTimerAtual;
	private bool soltaTempo;

	void Start(){
		changeCamTimerAtual = changeCamTimerInicial;
		soltaTempo = false;
	}

	void Update(){
		if (tempo) {
			if (soltaTempo) {
				changeCamTimerAtual -= Time.deltaTime * 1;
			}
			if (changeCamTimerAtual <= 0) {
				oldIndex = camCtrl.currentCam;
				camCtrl.ChangeCameraTo (newCamIndexTimer);
				soltaTempo = false;
				changeCamTimerAtual = changeCamTimerInicial;
			}
		}
	}

	void TrocaCameraTime(){
		oldIndex = camCtrl.currentCam;
		camCtrl.ChangeCameraTo (newCamIndex);
	}

	void OnTriggerEnter(Collider col){
		if (col.CompareTag ("Player")) {
			if (!tempo) {
				if (change_LookAt) {
					if (exitAsDifferentCam) {
						oldIndex = camCtrl.currentCam;
						camCtrl.ChangeCameraTo (newCamIndex);
					}

					if (lookBeyondPoint) {
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
			}else if(tempo){
				if (activator) {
					if (changeCamTimerAtual > 0) {
						soltaTempo = true;
					}
				}else if(!activator){
					oldIndex = camCtrl.currentCam;
					camCtrl.ChangeCameraTo (newCamIndex);
					if (changeCamTimerAtual > 0) {
						soltaTempo = true;
					}

				}
			}
		}
	}

	void OnTriggerStay(Collider col){
		if (!tempo) {
			if (col.CompareTag ("Player") && !change_LookAt) {
				camCtrl.ChangeCameraTo (newCamIndex);
			}
		}
	}

	void OnTriggerExit(Collider col){
		if(!tempo){

			if (camCtrl.currentCam != newCamIndex || change_LookAt)
				return;

			if(exitAsDifferentCam)
				camCtrl.ChangeCameraTo (exitCamIndex);
			else
				camCtrl.ChangeCameraTo (oldIndex);
		}
	}
}
