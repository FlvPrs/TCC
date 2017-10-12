using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionsCtrl : MonoBehaviour {

	public float drumForce = 30f;
	public float windForce = 30f;
	public float windcurrentForce = 400f;

	private int currentCount;
	private Vector3 avrgCurrentMagnitude = Vector3.zero;
	private Vector3 currentMomentum = Vector3.zero;

	private Vector3 respawnPoint;

	private WalkingController playerCtrl;
	private BirdStatureCtrl birdHeight;
	private CameraFollowPOI camPOI;

	private Cinemachine.CinemachineVirtualCamera cvCamera_Falling;
	private Cinemachine.CinemachineVirtualCamera cvCamera_TopViewFalling;

	private float originalGlideStrength;
	private int original_FallCamPriority;
	private int original_TopFallCamPriority;

	public GameObject blackScrn;

	void Awake(){
		playerCtrl = GetComponent<WalkingController> ();
		birdHeight = GetComponent<BirdStatureCtrl> ();
		cvCamera_Falling = GameObject.Find ("CM vcam1 - Falling").GetComponent<Cinemachine.CinemachineVirtualCamera> ();
		cvCamera_TopViewFalling = GameObject.Find ("CM vcam2 - TopFalling").GetComponent<Cinemachine.CinemachineVirtualCamera> ();

		camPOI = GetComponentInChildren<CameraFollowPOI> ();

		originalGlideStrength = playerCtrl.glideStrength;
		original_FallCamPriority = cvCamera_Falling.Priority;
		original_TopFallCamPriority = cvCamera_TopViewFalling.Priority;
	}

	void Update(){
		RaycastHit hit;

		Debug.DrawRay (transform.position + transform.up * 0.1f, Vector3.down * (0.2f), Color.green);

		if(Physics.Raycast(transform.position + transform.up * 0.1f, Vector3.down, out hit, 0.2f)){
			if(hit.transform.CompareTag("PlatAlto")){
				if(birdHeight.currentState != HeightState.High){
					StartCoroutine ("FallThroughCollider", hit.collider);
				}
			} else if(hit.transform.CompareTag("PlatBaixo")){
				if(birdHeight.currentState != HeightState.Low){
					StartCoroutine ("FallThroughCollider", hit.collider);
				}
			}
		}
	}

	IEnumerator FallThroughCollider(Collider col){
		col.enabled = false;
		yield return new WaitForSeconds (1.0f);
		col.enabled = true;
	}

	IEnumerator FallingCamera(){
		yield return new WaitForSeconds (2.0f);
		cvCamera_TopViewFalling.Priority = 12;
	}


	void OnTriggerEnter(Collider col){
		if(col.CompareTag("Ledge")){
			Vector3 dir = col.transform.GetChild(0).transform.position - col.transform.position;
			playerCtrl.StartCoroutine("GrabLedge", dir);
		}

//		if(col.CompareTag("Drum")){
//			playerCtrl.externalForceAdded = true;
//			col.GetComponentInParent<AudioSource> ().Play ();
//			Vector3 dir = col.transform.up * drumForce;
//			playerCtrl.AddExternalForce (dir, 0.5f);
//		}

		if(col.CompareTag("PowerUp")){
			col.gameObject.SetActive (false);
			playerCtrl.ResetFlyStamina ();
		}

		if(col.CompareTag("WindCurrent")){
			currentCount++;
			if(currentCount == 1)
				playerCtrl.BypassGravity (true);
			avrgCurrentMagnitude += col.transform.forward;
			currentMomentum = col.transform.forward * (windcurrentForce / 100);
		}

		if(col.CompareTag("Falling_Trigger")){
			playerCtrl.glideStrength = 0.01f;
			cvCamera_Falling.Priority = 11;
			//transform.LookAt (GameObject.Find ("Father").transform);
			StartCoroutine ("FallingCamera");
			col.gameObject.SetActive (false);
		}

		if(col.CompareTag("StopFalling")){
			playerCtrl.glideStrength = originalGlideStrength;
			cvCamera_Falling.Priority = original_FallCamPriority;
			cvCamera_TopViewFalling.Priority = original_TopFallCamPriority;
			blackScrn.SetActive (true);
		}

		if(col.CompareTag("Respawn")){
			respawnPoint = col.transform.position;
		}
		if(col.CompareTag("DeathTrigger")){
			transform.position = respawnPoint;
		}

		if(col.CompareTag("LookAt")){
			int num = int.Parse(col.name.TrimStart ("LookAt_".ToCharArray ()));
			camPOI.currentTarget = num;
		}
	}

	void OnTriggerStay(Collider col){
		if(col.CompareTag("Wind")){
			playerCtrl.ContinuousExternalForce (col.transform.up * windForce, false, false);
		}
		if(col.CompareTag("WindCurrent")){
			playerCtrl.ContinuousExternalForce ((Vector3.ClampMagnitude(avrgCurrentMagnitude, 1f) * windcurrentForce) / currentCount, true, false);
		}
	}

	void OnTriggerExit(Collider col){
		if(col.CompareTag("Wind")){
			playerCtrl.ContinuousExternalForce (Vector3.zero, false, false);
		}
		if(col.CompareTag("WindCurrent")){
			playerCtrl.ContinuousExternalForce (Vector3.zero, false, false);
			playerCtrl.BypassGravity (false);
			currentCount--;
			avrgCurrentMagnitude -= col.transform.forward;
			if(currentCount < 1){
				playerCtrl.AddExternalForce (currentMomentum, 0.5f);
			}
		}
	}
}
