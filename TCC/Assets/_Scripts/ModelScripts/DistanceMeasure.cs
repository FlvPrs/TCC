using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceMeasure : MonoBehaviour {

	public Transform target;
	public float vectorDistance;

	public Vector3 distAxis;

	void Update () {
		vectorDistance = Vector3.Distance (target.position, transform.position);


		float distX = target.position.x - transform.position.x;
		float distY = target.position.y - transform.position.y;
		float distZ = target.position.z - transform.position.z;
		distAxis = new Vector3 (distX, distY, distZ);
	}
}
