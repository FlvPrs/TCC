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

		endPos = target.localPosition + distanceLocal;
	}

	void Update () {
		if (!canMove)
			return;

		Vector3 pos = Vector3.Lerp (target.localPosition, endPos, 2f * Time.deltaTime);
		target.localPosition = pos;

		if (Vector3.Distance (target.localPosition, endPos) < 0.2f)
			canMove = false;
	}

	void OnTriggerEnter(Collider col){
		if(col.CompareTag("Player")){
			canMove = true;
		}
	}
}
