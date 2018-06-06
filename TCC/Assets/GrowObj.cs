using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowObj : MonoBehaviour {

	public bool startGrow;

	public float initialRadius = 0.5f;
	public float maxRadius = 5f;
	public float pace = 0.1f;

	Transform t;

	void Start () {
		t = GetComponent<Transform> ();
		t.localScale = new Vector3 (initialRadius, t.localScale.y, initialRadius);
	}

	// Update is called once per frame
	void Update () {
		if (startGrow) {
			if (t.localScale.x < maxRadius) {
				t.localScale += new Vector3 (pace * Time.deltaTime, 0, pace * Time.deltaTime);
			} else {
				enabled = false;
			}
		}
	}
}
