using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallLevel : MonoBehaviour {
	public float shakeSpeed = 50.0f;
	public float amountShake = 0.5f, timeShakingTofall = 2.0f;
	private bool startShake;
	private Transform t;
	// Use this for initialization
	void Start () {
		startShake = false;
		t = GetComponent<Transform> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (startShake) {
			StartShake ();
		}
	}

	void OnTriggerEnter(Collider colisor){
		if (colisor.name == "PlayerCollider") {
			startShake = true;
		}
	}
	void StartShake(){
		StartCoroutine ("FallPlatform");
		t.position = new Vector3 (t.position.x, t.position.y, t.position.z + Mathf.Sin (Time.time * shakeSpeed) * amountShake);
		if (GetComponent<FlyWithWind> () != null){
			GetComponent<FlyWithWind> ().magnitude = Vector3.one * 2f;
			GetComponent<FlyWithWind> ().frequency = Vector3.one * 4f;
		}
	}

	IEnumerator FallPlatform(){
		yield return new WaitForSeconds (timeShakingTofall);
		startShake = false;
		GetComponent<Rigidbody> ().useGravity = true;
		GetComponent<Rigidbody> ().isKinematic = false;
		GetComponent<Rigidbody> ().AddForce (Vector3.down* 2.0f, ForceMode.Impulse);
//		if (GetComponent<FlyWithWind> () != null)
//			GetComponent<FlyWithWind> ().startFlyAway = true;
	}
}
