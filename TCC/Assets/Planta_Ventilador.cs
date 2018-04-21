using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planta_Ventilador : PlantaBehaviour {

	public GameObject vento;

	bool fechada = true;

	Quaternion originalOrientation;

	public float angularSpeed = 2f;
	public LayerMask layerMask;
	public Transform headJoint;

	protected override void Awake ()
	{
		base.Awake ();

		//vento = plantaTransform.Find ("Vento").gameObject;

		originalOrientation = headJoint.rotation;
	}

	protected override void Update ()
	{
		base.Update ();

		if(fechada){
			vento.SetActive (false);
		} else {
			vento.SetActive (true);
		}

		if(timer >= 1f){
			Invoke ("PararDeSeguir", 2f);
		}
	}

	protected override void DefaultState ()
	{
		base.DefaultState ();

		headJoint.rotation = Quaternion.Slerp(headJoint.rotation, originalOrientation, Time.deltaTime * angularSpeed);
	}

	protected override void Seguir ()
	{
		base.Seguir ();

		if (Vector3.Dot (-Vector3.up, (currentInteractionAgent.position - headJoint.position).normalized) < 0.333f) {
			headJoint.rotation = Quaternion.Slerp (headJoint.rotation, Quaternion.LookRotation (currentInteractionAgent.position - headJoint.position), Time.deltaTime * angularSpeed);
		}
	}

	protected override void PararDeSeguir ()
	{
		base.PararDeSeguir ();
	}

	IEnumerator Close (){
		yield return new WaitForSeconds (3f);
		fechada = true;
	}

	protected override void Irritar ()
	{
		base.Irritar ();

		vento.tag = "Wind2";
	}
	protected override void Acalmar ()
	{
		base.Acalmar ();

		vento.tag = "Wind";
	}

	protected override void OnTriggerEnter (Collider col)
	{
		base.OnTriggerEnter (col);

		if((layerMask.value & 1<<col.gameObject.layer) == 0){
			return;
		}

		if(col.CompareTag("Player")){
			StopCoroutine ("Close");
			fechada = false;
		}
	}

	protected override void OnTriggerExit (Collider col)
	{
		base.OnTriggerExit (col);

		if(col.CompareTag("Player")){
			StartCoroutine ("Close");
		}
	}
}
