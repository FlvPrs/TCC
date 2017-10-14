using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCamOnTrigger : MonoBehaviour {

	public Cinemachine.CinemachineVirtualCamera cvCamera;

	private Cinemachine.CinemachineVirtualCamera oldCam = null;
	private int activeCamPriority;


	void Update () {
		
	}

	void OnTriggerEnter(Collider col){
		if(col.CompareTag("Player")){
			if (oldCam == null) {
				oldCam = Camera.main.GetComponent<Cinemachine.CinemachineBrain> ().ActiveVirtualCamera as Cinemachine.CinemachineVirtualCamera;
				activeCamPriority = oldCam.Priority;
			}

			cvCamera.Priority = activeCamPriority + 1;

		}
	}
}
