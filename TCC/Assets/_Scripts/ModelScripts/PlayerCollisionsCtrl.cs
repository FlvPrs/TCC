using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionsCtrl : MonoBehaviour {

	public float drumForce = 30f;
	public float windForce = 30f;
	public float windcurrentForce = 800f;

	private int currentCount;
	private Vector3 avrgCurrentMagnitude = Vector3.zero;
	private Vector3 currentMomentum = Vector3.zero;

	//private Vector3 respawnPoint;

	private WalkingController playerCtrl;
	private CameraFollowPOI camPOI;

	private float originalGlideStrength;

	public GameObject blackScrn;

	private Transform oldWindPoint;
	private Vector3 nextWindPoint;
	private Vector3 centerOfWind;
	private bool canCenterWind;

	public Transform[] spawnPoints;
	private int currentSpawnPoint = 0;

	private float timeToDieByVeneno = 2f;
	[SerializeField]
	private float poisoningTimer = 0f;
	private bool venenoDecay, venenoIncrease;
	private bool iminentFakeDeath = false;
	private bool diedByVeneno = false;

	void Awake(){
		playerCtrl = GetComponent<WalkingController> ();

		camPOI = GetComponentInChildren<CameraFollowPOI> ();

		originalGlideStrength = playerCtrl.glideStrength;
	}

//	void Update(){
//		RaycastHit hit;
//
//		Debug.DrawRay (transform.position + transform.up * 0.1f, Vector3.down * (0.2f), Color.green);
//
//		if(Physics.Raycast(transform.position + transform.up * 0.1f, Vector3.down, out hit, 0.2f)){
//			if(hit.transform.CompareTag("PlatAlto")){
//				if(playerCtrl.walkStates.CURR_HEIGHT_STATE != HeightState.High){
//					StartCoroutine ("FallThroughCollider", hit.collider);
//				}
//			} else if(hit.transform.CompareTag("PlatBaixo")){
//				if(playerCtrl.walkStates.CURR_HEIGHT_STATE != HeightState.Low){
//					StartCoroutine ("FallThroughCollider", hit.collider);
//				}
//			}
//		}
//	}

	void Update (){
		if(poisoningTimer >= 0f && !diedByVeneno){
			if(venenoDecay)
				poisoningTimer -= Time.deltaTime;
			else if (venenoIncrease)
				poisoningTimer += Time.deltaTime;

			if(poisoningTimer > timeToDieByVeneno){
				//TODO Die Birdo Die
				diedByVeneno = true;
				if(iminentFakeDeath){
					FindObjectOfType<MenuControllerInGame> ().TrocaMenus (8);
				} else {
					FindObjectOfType<FadeOutRespawn> ().StartFade (transform, spawnPoints[currentSpawnPoint].position, currentSpawnPoint);
				}
			}
		} else {
			poisoningTimer = 0f;
		}
	}

	public void RestartVenenoTimer (){
		poisoningTimer = 0;
		diedByVeneno = venenoDecay = venenoIncrease = false;
	}

	IEnumerator FallThroughCollider(Collider col){
		col.enabled = false;
		yield return new WaitForSeconds (1.0f);
		col.enabled = true;
	}

	IEnumerator waitToRecenterWind(){
		yield return new WaitForSeconds (0.2f);
		canCenterWind = true;
	}

//	IEnumerator FallingCamera(){
//		yield return new WaitForSeconds (2.0f);
//		cvCamera_TopViewFalling.Priority = 21;
//	}


	void OnTriggerEnter(Collider col){
//		if(col.CompareTag("Cogumelo")){ //DEIXA COMENTADO
//			playerCtrl.externalForceAdded = true;
//			//col.GetComponentInParent<AudioSource> ().Play ();
//			Vector3 dir = col.transform.up * drumForce;
//			playerCtrl.AddExternalForce (dir, 0.5f);
//		}

		if(col.CompareTag("PowerUp")){
			col.gameObject.SetActive (false);
			//playerCtrl.ResetFlyStamina ();
			playerCtrl.hasBonusJump = true;
			playerCtrl.hasBonusJump_2 = true;
		}

		if(col.CompareTag("WindCurrent")){
			canCenterWind = false;

			currentCount++;
//			if (currentCount == 1) {
//				playerCtrl.BypassGravity (true);
//			}

			oldWindPoint = col.transform;
			avrgCurrentMagnitude += col.transform.forward;
			currentMomentum = col.transform.forward * (windcurrentForce / 100);
			nextWindPoint = col.GetComponent<WaypointControl> ().next.position;
			StartCoroutine(waitToRecenterWind());
		}

//		if(col.CompareTag("Falling_Trigger")){
//			playerCtrl.animCtrl.SetTrigger ("startDeathFall");
//		}

//		if(col.CompareTag("Falling_Trigger")){
//			playerCtrl.glideStrength = 0f;
//			//cvCamera_Falling.Priority = 20;
//			//transform.LookAt (GameObject.Find ("Father").transform);
//			//StartCoroutine ("FallingCamera");
//
//			col.gameObject.SetActive (false);
//		}
//
//		if(col.CompareTag("StopFalling")){
//			playerCtrl.glideStrength = originalGlideStrength;
//			cvCamera_Falling.Priority = original_FallCamPriority;
//			cvCamera_TopViewFalling.Priority = original_TopFallCamPriority;
//			blackScrn.SetActive (true);
//		}

		if(col.CompareTag("Respawn")){
			currentSpawnPoint = int.Parse(col.name);
			//respawnPoint = col.transform.position;
		}
		if(col.CompareTag("DeathTrigger")){
			//transform.position = respawnPoint;
			col.GetComponent<FadeOutRespawn> ().StartFade (transform, spawnPoints[currentSpawnPoint].position, currentSpawnPoint);
		}

		if(col.CompareTag("LookAt")){
			int num = int.Parse(col.name.TrimStart ("LookAt_".ToCharArray ()));
			camPOI.currentTarget = num;
		}

		if(col.CompareTag("Wind") || col.CompareTag("Wind2")){
			playerCtrl.ZeroExternalForce (true);
		}
	}

	void OnTriggerStay(Collider col){
		if(col.CompareTag("Wind")){
			//playerCtrl.ContinuousExternalForce (col.transform.up * windForce, true, false);
			playerCtrl.AddContinuousExternalForce(col.transform.up * windForce);
		} else if(col.CompareTag("Wind2")){
			//playerCtrl.ContinuousExternalForce (col.transform.up * windForce, true, false);
			playerCtrl.AddContinuousExternalForce(col.transform.up * windForce * 2f);
		} else if(col.CompareTag("WindCurrent")){
			centerOfWind = oldWindPoint.position + Vector3.Project(transform.position - oldWindPoint.position, oldWindPoint.forward);

			if(canCenterWind)
				transform.position = Vector3.MoveTowards (transform.position, centerOfWind, 0.1f);

			playerCtrl.ContinuousExternalForce ((Vector3.ClampMagnitude(avrgCurrentMagnitude, 1f) * windcurrentForce) / currentCount, true, true);
		}

		if(col.CompareTag("StretchTrigger")){
			if (playerCtrl.walkStates.CURR_HEIGHT_STATE == HeightState.High)
				playerCtrl.holdHeight = true;
		}
		if(col.CompareTag("SquashTrigger")){
			if (playerCtrl.walkStates.CURR_HEIGHT_STATE == HeightState.Low)
				playerCtrl.holdHeight = true;
		}
			
		if(col.CompareTag("Veneno")){
			venenoDecay = false;
			venenoIncrease = true;
		} else if (col.CompareTag("VenenoFakeDeath")) {
			venenoDecay = false;
			venenoIncrease = true;
			iminentFakeDeath = true;
		}
	}

	void OnTriggerExit(Collider col){
		if(col.CompareTag("Wind") || col.CompareTag("Wind2")){
			playerCtrl.ZeroExternalForce ();
		}

		if(col.CompareTag("WindCurrent")){
			//playerCtrl.ContinuousExternalForce (Vector3.zero, false, false);
			//playerCtrl.BypassGravity (false);
			currentCount--;
			avrgCurrentMagnitude -= col.transform.forward;
			if(currentCount < 1){
				playerCtrl.AddExternalForce (currentMomentum, 0.5f);
			}
		}

		if(col.CompareTag("StretchTrigger") || col.CompareTag("SquashTrigger")){
			playerCtrl.holdHeight = false;
		}

		if(col.CompareTag("Veneno") || col.CompareTag("VenenoFakeDeath")){
			StartCoroutine (ClearPoison ());
			iminentFakeDeath = false;
		}
	}

	IEnumerator ClearPoison (){
		venenoDecay = false;
		venenoIncrease = false;
		yield return new WaitForSeconds (3f);
		venenoDecay = true;
	}
}
