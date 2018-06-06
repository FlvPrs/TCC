using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour {

	[System.Serializable]
	public struct Axis
	{
		public bool x, y, z;
	}

	public Transform daddy;
	public Axis ignoreMovementAxis;
	public Axis ignoreRotationAxis;

	private Transform myT;

	Vector3 originalRefPosition;
	Vector3 myOriginalPosition;

	Quaternion originalRefRotation;
	Quaternion myOriginalRotation;

	void Start () {
		myT = GetComponent<Transform> ();
		originalRefPosition = daddy.position;
		myOriginalPosition = myT.position;
	}

	void LateUpdate () {
		Vector3 currRefPos = daddy.position - originalRefPosition;
		currRefPos = new Vector3(
			(!ignoreMovementAxis.x) ? currRefPos.x : 0,
			(!ignoreMovementAxis.y) ? currRefPos.y : 0,
			(!ignoreMovementAxis.z) ? currRefPos.z : 0
		);
		myT.position = myOriginalPosition + currRefPos;

		if (ignoreRotationAxis.x && ignoreRotationAxis.y && ignoreRotationAxis.z)
			return;

		Quaternion currRefRot = daddy.rotation;
		currRefRot.eulerAngles = new Vector3(
			(!ignoreRotationAxis.x) ? currRefRot.x : 0,
			(!ignoreRotationAxis.y) ? currRefRot.y : 0,
			(!ignoreRotationAxis.z) ? currRefRot.z : 0
		);
		myT.rotation = currRefRot;
	}
}
