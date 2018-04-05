using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationForward : MonoBehaviour {

	public Transform objToRotate;
	private Animator anim;
	private Transform t;

	void Awake(){
		anim = GetComponent<Animator> ();
		t = GetComponent<Transform> ();
	}

	void Start(){
	}

	public void ChangeForward(Vector3 dir){
		Vector3 newDir = dir;
		newDir.y = 0f;

		t.forward = newDir;
		//if (objToRotate)
		objToRotate.forward = newDir;
	}

	void OnAnimatorMove(){
		//transform.parent.rotation = anim.rootRotation;

		transform.parent.position += anim.deltaPosition;
		//anim.stabilizeFeet = true;
	}
}
