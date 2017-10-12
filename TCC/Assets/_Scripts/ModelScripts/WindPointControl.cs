using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(WaypointControl))]
public class WindPointControl : MonoBehaviour {

	private WaypointControl wpCtrl;
	private BoxCollider coll;
	private Vector3 dir;

	void Awake(){
		wpCtrl = GetComponent<WaypointControl> ();
		coll = GetComponent<BoxCollider> ();
		if(!wpCtrl.next){
			coll.enabled = false;
			return;
		}

		dir = wpCtrl.next.position - transform.position;

		transform.LookAt (wpCtrl.next, Vector3.up);

		Vector3 newPos = Vector3.forward * (dir.magnitude / 2);
		Vector3 newScale = Vector3.one * 2f;
		newScale.z = dir.magnitude + (dir.magnitude * 0.2f);

		coll.center = newPos;
		coll.size = newScale;
	}
}
