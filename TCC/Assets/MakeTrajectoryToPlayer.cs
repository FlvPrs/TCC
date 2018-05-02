using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeTrajectoryToPlayer : MonoBehaviour {

	Transform myT;
	int count;
	Transform[] myChildren;
	Vector3[] myChildrenOriginalPos;
	Transform player;

	void Start () {
		count = transform.childCount;
		myChildren = new Transform[count];
		myChildrenOriginalPos = new Vector3[count];
		myT = GetComponent<Transform> ();
		player = FindObjectOfType<WalkingController> ().transform;

		for (int i = 0; i < count; i++) {
			myChildren [i] = transform.GetChild (i);
			myChildrenOriginalPos [i] = myChildren [i].position;
		}
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 dir = (player.position + Vector3.up * 2f) - myT.position;
		dir.y = 0;

		for (int i = 0; i < count; i++) {
			myChildren [i].position = myChildrenOriginalPos [i] + (dir * (i + 1) / count);
		}
	}
}
