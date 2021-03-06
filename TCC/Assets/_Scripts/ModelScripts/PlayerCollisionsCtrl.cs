﻿using System.Collections;
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

	bool isOnShelter;

	public Transform[] spawnPoints;
	[HideInInspector]
	public int currentSpawnPoint = 0;

	private float timeToDieByVeneno = 2f;
	public float poisoningTimer = 0f;
	[HideInInspector]
	public bool venenoDecay, venenoIncrease;
	private bool iminentFakeDeath = false;
	private bool diedByVeneno = false;
	bool onFakeDeathSang = false;

	VignettePulse venenoFX;
	public BlinkScreen blink;

	GameObject tip_VenenoAskForHelp;

	VenenoCtrl currentVeneno;

	FatherActions father;

	public GameObject MDL_BirdoFilho;
	[HideInInspector]
	public AudioSource[] hugAudioSources;
	public AudioClip[] venenoHurtClips;


	void Awake(){
		playerCtrl = GetComponent<WalkingController> ();

		camPOI = GetComponentInChildren<CameraFollowPOI> ();

		originalGlideStrength = playerCtrl.glideStrength;

		venenoFX = FindObjectOfType<VignettePulse> ();
		blink = FindObjectOfType<BlinkScreen> ();

		father = FindObjectOfType<FatherActions> ();

		tip_VenenoAskForHelp = GameObject.Find ("MenuFakeDeath");

		hugAudioSources = new AudioSource[2];

		for (int i = 0; i < hugAudioSources.Length; i++) {
			hugAudioSources [i] = MDL_BirdoFilho.GetComponents<AudioSource> () [i];
			hugAudioSources [i].volume = 0.02f;
		}
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

	float blinkAt = 0f;
	void Update (){
		if (playerCtrl.beinghugged) {
			for (int i = 0; i < hugAudioSources.Length; i++) {
				hugAudioSources [i].volume += (hugAudioSources [0].volume < 1) ? Time.deltaTime*0.002f : 0f;
			}
		} else {
			for (int i = 0; i < hugAudioSources.Length; i++) {
				hugAudioSources [i].Stop ();
				hugAudioSources [i].volume = 0.02f;
			}
		}

		if(poisoningTimer >= 0f && !diedByVeneno){
			if (venenoDecay) {
				poisoningTimer -= Time.deltaTime;
				if (poisoningTimer <= 0f) {
					RestartVenenoTimer ();
				}
				if (poisoningTimer > 0f && poisoningTimer < 0.5f)
					blinkAt = 0.25f;
				else if (poisoningTimer >= 0.5f && poisoningTimer < 1f)
					blinkAt = 0.5f;
				else if (poisoningTimer >= 1f)
					blinkAt = 0.75f;
				
			} else if (venenoIncrease) {
				poisoningTimer += (!playerCtrl.beinghugged) ? Time.deltaTime : Time.deltaTime * 0.25f;

				if (playerCtrl.walkStates.TOCANDO_NOTAS) {
					currentVeneno.DisableNavMeshCarve ();
//					if (Vector3.Distance (father.transform.position, transform.position) <= 10f) {
//						playerCtrl.StartHug ();
//					}
				}
				if(poisoningTimer > timeToDieByVeneno * blinkAt){
					blink.Blink ();
					blinkAt += 0.25f;
					if (!(poisoningTimer > 0.5f && poisoningTimer < 1f)) {
						GetComponent<AudioSource> ().clip = venenoHurtClips [Random.Range (0, venenoHurtClips.Length)];
						GetComponent<AudioSource> ().Play ();
					}
				}
			}

			if (venenoFX != null) {
//				venenoFX.maxIntensity = 0.5f + (poisoningTimer * 0.1f);
//				venenoFX.speedModifier = 1f + (poisoningTimer);
				venenoFX.maxIntensity = poisoningTimer * 0.5f;
				venenoFX.start = (poisoningTimer != 0);
			}

			if(poisoningTimer > timeToDieByVeneno){
				blinkAt = 0f;

				diedByVeneno = true;
				if(iminentFakeDeath){
					FindObjectOfType<MenuControllerInGame> ().TrocaMenus (8);
				} else {
					FadeOutRespawn[] respawn = FindObjectsOfType<FadeOutRespawn> ();
					for (int i = 0; i < respawn.Length; i++) {
						if(!respawn[i].fadeToEndLevel){
							respawn [i].StartFade (transform, spawnPoints [currentSpawnPoint].position, currentSpawnPoint);
							break;
						}
					}
				}
			}
		} else {
			blinkAt = 0f;

			poisoningTimer = 0f;
			if (!diedByVeneno)
				venenoFX.start = false;
			else if (iminentFakeDeath && playerCtrl.walkStates.TOCANDO_NOTAS) {
					onFakeDeathSang = true;
			}

			if(onFakeDeathSang){
				if (venenoFX.maxIntensity > 0.4f)
					venenoFX.maxIntensity -= 1 * Time.deltaTime;
				else
					onFakeDeathSang = false;
			} else {
				if (venenoFX.maxIntensity < 0.9f)
					venenoFX.maxIntensity += 0.2f * Time.deltaTime;
			}
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

		if (col.CompareTag ("Veneno") || col.CompareTag ("VenenoFakeDeath")) {
			currentVeneno = col.GetComponentInParent<VenenoCtrl> ();
			if(!playerCtrl.beinghugged)
				tip_VenenoAskForHelp.SetActive (true);
		}
	}

	void OnTriggerStay(Collider col){
		if(col.CompareTag("Shelter")){
			CancelInvoke ("NotOnShelterAnymore");
			isOnShelter = true;
			Invoke ("NotOnShelterAnymore", 0.2f);
		}

		if (!isOnShelter) {
			if (col.CompareTag ("Wind")) {
				//playerCtrl.ContinuousExternalForce (col.transform.up * windForce, true, false);
				Vector3 wind = (playerCtrl.beinghugged) ? col.transform.up * (windForce * 0.05f) : col.transform.up * windForce;
				playerCtrl.AddContinuousExternalForce (wind);
			} else if (col.CompareTag ("Wind2")) {
				//playerCtrl.ContinuousExternalForce (col.transform.up * windForce, true, false);
				Vector3 wind = (playerCtrl.beinghugged) ? col.transform.up * (windForce * 0.1f) : col.transform.up * windForce * 2f;
				playerCtrl.AddContinuousExternalForce (wind);
			} else if (col.CompareTag ("WindCurrent")) {
				centerOfWind = oldWindPoint.position + Vector3.Project (transform.position - oldWindPoint.position, oldWindPoint.forward);

				if (canCenterWind)
					transform.position = Vector3.MoveTowards (transform.position, centerOfWind, 0.1f);

				playerCtrl.animCtrl.SetBool ("isOnWind", true);
				playerCtrl.ContinuousExternalForce ((Vector3.ClampMagnitude (avrgCurrentMagnitude, 1f) * windcurrentForce) / currentCount, true, true);
			}
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
			StopCoroutine (ClearPoison ());
			venenoDecay = false;
			venenoIncrease = true;
		} else if (col.CompareTag("VenenoFakeDeath")) {
			StopCoroutine (ClearPoison ());
			venenoDecay = false;
			venenoIncrease = true;
			iminentFakeDeath = true;
		}
	}

	void NotOnShelterAnymore (){
		isOnShelter = false;
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
				playerCtrl.animCtrl.SetBool ("isOnWind", false);
			}
		}

		if(col.CompareTag("StretchTrigger") || col.CompareTag("SquashTrigger")){
			playerCtrl.holdHeight = false;
		}

		if(col.CompareTag("Veneno") || col.CompareTag("VenenoFakeDeath")){
			StartCoroutine (ClearPoison (currentVeneno));
			//currentVeneno = null;
			iminentFakeDeath = false;
			tip_VenenoAskForHelp.SetActive (false);
		}
	}

	IEnumerator ClearPoison (VenenoCtrl veneno = null){
		venenoDecay = false;
		venenoIncrease = false;
		yield return new WaitForSeconds (1f);
		if(veneno != null)
			veneno.ResetNavMeshCarve ();
		venenoDecay = true;

		tip_VenenoAskForHelp.SetActive (false);
	}
}
