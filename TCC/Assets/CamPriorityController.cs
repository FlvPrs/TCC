using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Toda camera que não estiver sendo utilizada possuirá prioridade 0;
//A camera que estiver ativa possuirá prioridade 10.

public class CamPriorityController : MonoBehaviour {

	public WalkingController player;
	public Cinemachine.CinemachineVirtualCamera[] CM_Vcams;
	public int startingCam = 0;

	//[HideInInspector]
	public int currentCam;
	//private int[] defaultCamPriorities;

	void Awake(){
		currentCam = startingCam;

		//defaultCamPriorities = new int[CM_Vcams.Length];

		for (int i = 0; i < CM_Vcams.Length; i++) {
			//defaultCamPriorities [i] = CM_Vcams [i].Priority;

			if (i != currentCam)
				CM_Vcams [i].Priority = 0;
			else
				CM_Vcams [i].Priority = 10;
		}
	}

	void Update(){
		player.ChangeOrientationToCamera (CM_Vcams [currentCam].transform, false);
	}

	public void ChangeCameraTo(int newCamIndex){
		if (currentCam == newCamIndex)
			return;
		CM_Vcams [newCamIndex].Priority = 10;
		CM_Vcams [currentCam].Priority = 0;

		currentCam = newCamIndex;

		player.ChangeOrientationToCamera (CM_Vcams [currentCam].transform, true);
	}
}
