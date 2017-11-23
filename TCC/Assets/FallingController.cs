using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingController : MonoBehaviour {

	public int cam1_Index;
	public int cam2_Index;
	public CamPriorityController camCtrl;
	public WalkingController playerCtrl;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider col){
		if(col.CompareTag("Player")){
			camCtrl.ChangeCameraTo (cam1_Index);
			playerCtrl.glideStrength = 0f;
			StartCoroutine (ChangeToNextCam (cam2_Index));
		}
	}

	IEnumerator ChangeToNextCam(int nextIndex){
		yield return new WaitForSeconds (1f);
		camCtrl.ChangeCameraTo (nextIndex);
		gameObject.SetActive (false);
	}
}
