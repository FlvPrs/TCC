using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPOI : MonoBehaviour {

	public Transform[] target;

	//public float maxDistanceToPOI = 20f;
	public float[] maxDistancetoPOI;

	public int currentTarget;

	//private int defaultTarget = 0;

	private Transform myT;

	void Start () {
		myT = GetComponent<Transform> ();
	}

	void Update () {
		if (target == null)
			return;

		Vector3 dir = target[currentTarget].position - myT.position;
		//Debug.DrawRay (myT.position, dir);

		if (dir.magnitude < maxDistancetoPOI[currentTarget]) {
			if (Vector3.Angle (myT.forward, dir) <= 60f) {
				Quaternion newRot = myT.rotation;
				newRot = Quaternion.Euler (-dir.magnitude, newRot.eulerAngles.y, newRot.eulerAngles.z);
				myT.rotation = newRot;
			} else {
				Quaternion newRot = myT.rotation;
				newRot = Quaternion.Euler (0, newRot.eulerAngles.y, newRot.eulerAngles.z);
				myT.rotation = newRot;
			}
		} else if (dir.magnitude > (maxDistancetoPOI[currentTarget] * 2f)) {
			Quaternion newRot = myT.rotation;
			newRot = Quaternion.Euler (0, newRot.eulerAngles.y, newRot.eulerAngles.z);
			myT.rotation = newRot;
		}

//		Vector3 dir = target.position - Camera.main.transform.position;
//		dir = dir.normalized * maxDistanteToPOI;
//		RaycastHit hitInfo;
//		if(Physics.Raycast(Camera.main.transform.position, dir, out hitInfo, maxDistanteToPOI)) 
//		{
//			Debug.Log(hitInfo.collider.name+", "+hitInfo.collider.tag);
//		}
//		Debug.DrawRay (Camera.main.transform.position, dir, Color.magenta);
	}
}
