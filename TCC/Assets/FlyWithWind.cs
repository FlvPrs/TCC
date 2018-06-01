using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyWithWind : MonoBehaviour {

	public bool startFlyAway;
	enum DirectionReference {Up, Down, Forward, Back, Right, Left}
	[SerializeField]
	DirectionReference flyDirection;
	Vector3 windDir;
	public Vector3 magnitude = Vector3.one;
	public Vector3 frequency;

	public Collider shelter;

	bool isFlyingAway;

	Transform t;
	Rigidbody rb;
	Quaternion originalRot;
	Quaternion shake;

	// Use this for initialization
	void Start () {
		t = GetComponent<Transform> ();
		rb = GetComponent<Rigidbody> ();

		originalRot = t.rotation;
		shake = originalRot;

		switch (flyDirection) {
		case DirectionReference.Up:
			windDir = t.up;
			break;
		case DirectionReference.Down:
			windDir = -t.up;
			break;
		case DirectionReference.Forward:
			windDir = t.forward;
			break;
		case DirectionReference.Back:
			windDir = -t.forward;
			break;
		case DirectionReference.Right:
			windDir = t.right;
			break;
		case DirectionReference.Left:
			windDir = -t.right;
			break;
		default:
			break;
		}
	}
	
	// Update is called once per frame
	void Update () {

		if (!startFlyAway) {
			shake.eulerAngles = originalRot.eulerAngles + new Vector3 (
				Mathf.Sin (Time.realtimeSinceStartup * frequency.x) * magnitude.x, 
				Mathf.Sin (Time.realtimeSinceStartup * frequency.y) * magnitude.y, 
				Mathf.Sin (Time.realtimeSinceStartup * frequency.z) * magnitude.z
			);
			t.rotation = shake;
		} 
		else if(startFlyAway && !isFlyingAway){
			isFlyingAway = true;
			rb.useGravity = true;
			rb.isKinematic = false;
			rb.constraints = RigidbodyConstraints.None;
			if(GetComponentInChildren<Collider>() != null){
				GetComponentInChildren<Collider> ().enabled = false;
			}
			if(shelter != null){
				shelter.enabled = false;
			}
			rb.velocity = windDir;

			Invoke ("DisableThis", 3f);
		} 
		else {
			rb.velocity += windDir * 10f;
			t.Rotate (((frequency * 0.15f) + Vector3.one) * rb.velocity.magnitude * 0.01f);
		}
	}

	void DisableThis (){
		gameObject.SetActive (false);
	}
}
