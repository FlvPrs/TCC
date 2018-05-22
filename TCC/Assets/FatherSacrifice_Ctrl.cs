using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FatherSacrifice_Ctrl : MonoBehaviour {

	public Transform ripPosition;
	public GameObject real_Pai;
	public Transform sacrifice_Pai;
	public Transform[] waypointsToSaveSon;
	WaypointControl[] wpInformation;
	int currentWayPoint = 0;

	public CamPriorityController camCtrl;
	public int sacrificeCamIndex;

	Rigidbody sacrificePaiRB;

	bool startSacrifice = false;

	float vooCooldown = 0f;

	bool canSlowTime = false;
	float timeAcceleration = 0f;

	WalkingController player;

	void Awake () {
		real_Pai.SetActive (true);
		sacrifice_Pai.gameObject.SetActive (false);

		sacrificePaiRB = sacrifice_Pai.GetComponent<Rigidbody> ();

		sacrifice_Pai.transform.position = waypointsToSaveSon [0].position;

		wpInformation = new WaypointControl[waypointsToSaveSon.Length];
		for (int i = 0; i < wpInformation.Length; i++) {
			wpInformation [i] = waypointsToSaveSon [i].GetComponent<WaypointControl> ();
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (!startSacrifice)
			return;

		ContinueAVoar ();

		if (wpInformation [currentWayPoint].next != null) {
			Vector3 dir = wpInformation [currentWayPoint].next.position - sacrifice_Pai.position;

			sacrifice_Pai.rotation = Quaternion.Slerp (sacrifice_Pai.rotation, Quaternion.LookRotation (dir, Vector3.up), Time.deltaTime * 10f);
			sacrifice_Pai.eulerAngles = new Vector3 (0, sacrifice_Pai.eulerAngles.y, 0);

			//if (vooCooldown > 0f) {
				//Time.timeScale = 0.8f + timeAcceleration;
				//vooCooldown -= Time.deltaTime;
				//sacrifice_Pai.Translate (dir.normalized * (0.1f + timeAcceleration));
			sacrifice_Pai.Translate (dir.normalized * 0.6f);
			//} else if (canSlowTime) {
				//Time.timeScale = 0.1f;
			//}

			if (Vector3.Distance (wpInformation [currentWayPoint].next.position, sacrifice_Pai.position) < 1.1f)
				currentWayPoint++;
		} else {
			//TODO Fim
			//Time.timeScale = 1f;
			GetComponent<FadeOutRespawn_Sacrifice> ().StartFade (FindObjectOfType<WalkingController> ().transform, ripPosition.position);
			enabled = false;
		}
	}

	public void CheckSingToStartSacrifice (){
		player = FindObjectOfType<WalkingController> ();
		InvokeRepeating ("CheckSing", Time.deltaTime, Time.deltaTime);
	}

	void CheckSing (){
		if(player.walkStates.TOCANDO_NOTAS){
			CancelInvoke ("CheckSing");
			StartSacrifice ();
		}
	}

	public void StartSacrifice (){
		real_Pai.SetActive (false);
		sacrifice_Pai.gameObject.SetActive (true);
		camCtrl.ChangeCameraTo (sacrificeCamIndex);
		startSacrifice = true;
		Invoke ("AllowSlowTime", 2f);
	}

	public void ContinueAVoar (){
		if (vooCooldown > 0f)
			timeAcceleration += 0.005f;
		else
			timeAcceleration = 0f;
		
		vooCooldown = 0.5f;
	}

	void AllowSlowTime (){
		canSlowTime = true;
	}
}
