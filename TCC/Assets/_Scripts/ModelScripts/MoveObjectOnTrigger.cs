using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObjectOnTrigger : MonoBehaviour {

	public Transform target;
	public Vector3 distanceLocal;
	Vector3 endPos;
	bool canMove = false;

	void Start(){
		if (target == null)
			target = this.transform;

		endPos = target.position + distanceLocal;
	}

	void Update () {
		if (!canMove)
			return;

		Vector3 pos = Vector3.Lerp (target.position, endPos, 2f * Time.deltaTime);
		target.position = pos;

		if (Vector3.Distance (target.position, endPos) < 0.2f)
			canMove = false;
	}

	void OnTriggerEnter(Collider col){
		if(col.CompareTag("Player")){
			canMove = true;
		}
	}
}
