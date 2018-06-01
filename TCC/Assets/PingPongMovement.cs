using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PingPongMovement : MonoBehaviour {

	public Vector3 magnitude = Vector3.one;
	public Vector3 frequency;

	Transform t;
	//Rigidbody rb;

	Vector3 originalPos;

	// Use this for initialization
	void Start () {
		t = GetComponent<Transform> ();
		//rb = GetComponent<Rigidbody> ();

		originalPos = t.localPosition;
	}

	// Update is called once per frame
	void Update () {
		t.localPosition = originalPos + new Vector3 (
			Mathf.Sin (Time.realtimeSinceStartup * frequency.x) * magnitude.x, 
			Mathf.Sin (Time.realtimeSinceStartup * frequency.y) * magnitude.y, 
			Mathf.Sin (Time.realtimeSinceStartup * frequency.z) * magnitude.z
		);
	}
}
