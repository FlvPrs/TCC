using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingController : MonoBehaviour {

	public int cam1_Index;
	public int cam2_Index;
	public CamPriorityController camCtrl;
	public WalkingController playerCtrl;

	bool isFalling;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
//	void Update () {
//		if (isFalling) {
//			//playerCtrl.ResetFlyStamina ();
//			playerCtrl.animCtrl.SetBool ("IsFallingToDeath", true);
//		}
//	}

	void OnTriggerEnter(Collider col){
		if(col.CompareTag("Player")){
			isFalling = true;
			camCtrl.ChangeCameraTo (cam1_Index);
			playerCtrl.isFallingToDeath = true;
			playerCtrl.glideStrength = 0f;
			playerCtrl.ChangeJumpHeight (0f);
			playerCtrl.animCtrl.SetTrigger ("startDeathFall");
			playerCtrl.animCtrl.SetBool ("IsFallingToDeath", true);
			StartCoroutine (ChangeToNextCam (cam2_Index));
		}
	}

	IEnumerator ChangeToNextCam(int nextIndex){
		yield return new WaitForSeconds (1f);
		camCtrl.ChangeCameraTo (nextIndex);
		//gameObject.SetActive (false);
	}
}
