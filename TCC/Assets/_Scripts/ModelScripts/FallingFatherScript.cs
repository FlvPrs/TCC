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
		downForce = filho.maxFallVelocity * 2f;
	}
	
	// Update is called once per frame
	void Update () {
		if (!fallingTrigger.activeSelf) {
			
			return;
		}

		if (distSonToFloor.vectorDistance < 400f){
			saveSon = true;
		}

		if(!underTheSon) {
			if (saveSon) {

				if (tocou ) {
					fatherFall.start ();
					tocou = false;
					af = true;
				}

				
				if (myDistToSon.distAxis.y < 0) { //Se o pai estiver acima de (20 unidade abaixo do filho)
					underTheSon = false;
					float son_TimeTillCrash = distSonToFloor.vectorDistance / filho.currentFallVelocity;
					float dad_TimeTillSon = myDistToSon.vectorDistance / downForce;

					if (dad_TimeTillSon > son_TimeTillCrash + 2f) { //O +3 garante que o pai vai chegar 3s antes do filho
						downForce++;
					} else if (dad_TimeTillSon < son_TimeTillCrash + 2f) {
						downForce--;
					}
				} else {
					underTheSon = true;
				}
			} else {
				downForce = filho.currentFallVelocity;
			}
		}

		if(underTheSon){
			downForce ++;

			float xAmount = Mathf.Sign (myDistToSon.distAxis.x);
			float zAmount = Mathf.Sign (myDistToSon.distAxis.z);

			transform.Translate (xAmount * 0.35f, 0, zAmount * 0.35f, Space.World);
		}

		rb.velocity = Vector3.up * downForce;
	}
}
