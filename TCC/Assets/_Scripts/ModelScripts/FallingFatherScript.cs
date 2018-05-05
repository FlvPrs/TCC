using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingFatherScript : MonoBehaviour {

	[FMODUnity.EventRef]
	public string paiQueda;
	FMOD.Studio.EventInstance fatherFall;

	private Rigidbody rb;
	private DistanceMeasure myDistToSon;

	public GameObject fallingTrigger;
	public WalkingController filho;
	public DistanceMeasure distSonToFloor;
	float myDistToFloor;

	private bool saveSon = false;
	private bool underTheSon = false; //(Under the Son!)
	private float downForce;

	public bool tocou;
	public bool af;

	void Start () {
		tocou = true;
		af = false;
		//fmod
		paiQueda = "event:/Pai/PaiQueda";
		fatherFall = FMODUnity.RuntimeManager.CreateInstance (paiQueda);

		rb = GetComponent<Rigidbody> ();
		myDistToSon = GetComponent<DistanceMeasure> ();
		downForce = 0;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (!fallingTrigger.activeSelf) {
			return;
		}

		myDistToFloor = Vector3.Distance (distSonToFloor.transform.position, transform.position);

		if (distSonToFloor.vectorDistance < 400f){
			saveSon = true;
		}

		if (saveSon && !underTheSon) {

			if (tocou) {
				fatherFall.start ();
				tocou = false;
				af = true;
			}

			
			if (myDistToFloor > distSonToFloor.vectorDistance + 5f) { //Se o pai estiver acima do player
				underTheSon = false;
				downForce += 0.1f;
			} else {
				underTheSon = true;
			}
		} else {
			downForce = 0f;
		}

		if(underTheSon){
			downForce = Time.deltaTime;

			float xAmount = Mathf.Sign (myDistToSon.distAxis.x);
			float zAmount = Mathf.Sign (myDistToSon.distAxis.z);

			transform.Translate (xAmount * 0.35f, 0, zAmount * 0.35f, Space.World);
		}

		rb.velocity = Vector3.up * (filho.currentFallVelocity - downForce);
	}
}
