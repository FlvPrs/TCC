using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public abstract class Controller : MonoBehaviour {

	//TODO: Add method ReadInput
	public abstract void ReadInput (InputData data);

	public bool presentWhenInactive = true;

	protected Transform myT;
	protected Rigidbody rb;
	protected Collider coll;
	protected Camera cam;
	protected HUDScript hudScript;
	protected bool newInput;
	protected Transform playerT;
	protected BirdStatureCtrl birdHeightCtrl;

	void Awake(){
		rb = GetComponent<Rigidbody> ();
		myT = GetComponent<Transform> ();
		coll = GetComponent<Collider> ();
		playerT = myT.Find ("PlayerCharacter").GetComponent <Transform> ();

		birdHeightCtrl = GetComponent<BirdStatureCtrl> ();

		cam = GetComponentInChildren<Camera> ();
		hudScript = GameObject.FindObjectOfType<HUDScript> ();
	}

	protected virtual void Start(){
		if(InputManager.ins.controller != this){
			Deactivate ();
		}
	}

	public virtual void Activate(){
		InputManager.ins.controller.Deactivate ();
		InputManager.ins.controller = this;
		cam.gameObject.SetActive (true);
		if(!presentWhenInactive){
			gameObject.SetActive (true);
		}
	}

	public virtual void Deactivate(){
		cam.gameObject.SetActive (false);
		if(!presentWhenInactive){
			gameObject.SetActive (false);
		}
	}
}
