using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationForward : MonoBehaviour {

	private Animator anim;

	void Awake(){
		WalkingController.OnFacingChange += RefreshFacing;
		anim = GetComponent<Animator> ();
	}

	void Start(){
		
	}

	void RefreshFacing(FacingDirection fd){
		switch (fd) {
		case FacingDirection.North:
			//transform.localPosition = Vector3.forward * offset;
			transform.localEulerAngles = new Vector3(0, 0, 0);
			break;
		case FacingDirection.East:
			//transform.localPosition = Vector3.right * offset;
			transform.localEulerAngles = new Vector3(0, 90, 0);
			break;
		case FacingDirection.West:
			//transform.localPosition = Vector3.left * offset;
			transform.localEulerAngles = new Vector3(0, -90, 0);
			break;
		default:
			//transform.localPosition = Vector3.back * offset;
			transform.localEulerAngles = new Vector3(0, 180, 0);
			break;
		}
	}

	void OnAnimatorMove(){
		//transform.parent.rotation = anim.rootRotation;
		transform.parent.position += anim.deltaPosition;
	}
}
