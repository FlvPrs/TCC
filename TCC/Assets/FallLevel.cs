using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FlyWithWind))]
public class FallLevel : MonoBehaviour {
	public float shakeSpeed = 50.0f;
	public float amountShake = 0.5f, timeShakingTofall = 2.0f;
	private bool startShake;
	private Transform t;
	private Rigidbody rb;
	private Rigidbody otherPlat_Rb;

	public GameObject otherPlat;

	private FlyWithWind flyWind;
	private FlyWithWind otherPlat_FlyWind;
	private TranslateObject move;
	private TranslateObject otherPlat_Move;

	bool canFallAllTheWay = false;

	// Use this for initialization
	void Start () {
		startShake = false;
		t = GetComponent<Transform> ();
		rb = GetComponent<Rigidbody> ();

		flyWind = GetComponent<FlyWithWind> ();
		move = GetComponent<TranslateObject> ();

		if (otherPlat != null) {
			otherPlat_FlyWind = otherPlat.GetComponent<FlyWithWind> ();
			otherPlat_Move = otherPlat.GetComponent<TranslateObject> ();
			otherPlat_Rb = otherPlat.GetComponent<Rigidbody> ();
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (startShake) {
			StartShake ();
		}
	}

	void OnTriggerEnter(Collider colisor){
		if (colisor.name == "PlayerCollider"&& !startShake) {
			startShake = true;
			StartCoroutine ("FallPlatform");
		}
	}
	void OnTriggerExit(Collider colisor){
		if (colisor.name == "PlayerCollider" && !startShake) {
			startShake = true;
			StartCoroutine ("FallPlatform");
		}
	}

	void StartShake(){
		//StartCoroutine ("FallPlatform");
		t.position = new Vector3 (t.position.x, t.position.y, t.position.z + Mathf.Sin (Time.time * shakeSpeed) * amountShake);

		flyWind.magnitude = Vector3.one * 0.5f;
		flyWind.frequency = Vector3.one * 100f;
		if (otherPlat != null) {
			otherPlat_FlyWind.magnitude = Vector3.one * 0.3f;
			otherPlat_FlyWind.frequency = Vector3.one * 50f;
		}
	}

	IEnumerator FallPlatform(){
		yield return new WaitForSeconds (timeShakingTofall);
		startShake = false;

		if(!canFallAllTheWay){
			move.startMove = true;
			if(otherPlat != null){
				otherPlat_Move.startMove = true;
			}
			canFallAllTheWay = true;
		} else {
			move.enabled = false;
			rb.useGravity = true;
			rb.isKinematic = false;
			rb.AddForce (Vector3.down* 2.0f, ForceMode.Impulse);
			flyWind.magnitude = Vector3.one * 4f;
			flyWind.frequency = Vector3.one * 0.5f;
			flyWind.startFlyAway = true;

			if(otherPlat != null){
				otherPlat_Move.enabled = false;
				otherPlat_Rb.useGravity = true;
				otherPlat_Rb.isKinematic = false;
				otherPlat_Rb.AddForce (Vector3.down* 1.25f, ForceMode.Impulse);
				otherPlat_FlyWind.magnitude = Vector3.one * 4f;
				otherPlat_FlyWind.frequency = Vector3.one * 0.5f;
				otherPlat_FlyWind.startFlyAway = true;
			}
		}

//		GetComponent<Rigidbody> ().useGravity = true;
//		GetComponent<Rigidbody> ().isKinematic = false;
//		GetComponent<Rigidbody> ().AddForce (Vector3.down* 2.0f, ForceMode.Impulse);

//		if (GetComponent<FlyWithWind> () != null)
//			GetComponent<FlyWithWind> ().startFlyAway = true;
	}
}
