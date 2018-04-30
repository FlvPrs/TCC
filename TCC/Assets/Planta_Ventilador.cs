using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planta_Ventilador : PlantaBehaviour {

	public GameObject vento;
	public bool ventiladorAutomatico = false;
	public bool startAngry = false;
	public float auto_OnOffTimer = 2f;
	bool canStartVentiladorAutomatico;
	float ventiladorCooldown;

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
		canStartVentiladorAutomatico = true;

		if(startAngry){
			vento.tag = "Wind2";
		}
	}

	protected override void Update ()
	{
		base.Update ();

		if(fechada){
			vento.SetActive (false);
		} else {
			vento.SetActive (true);
		}

		if(timer >= 1f && currentState == Planta_CurrentState.Seguindo){
			Invoke ("PararDeSeguir", 2f);
		}
	}

	protected override void DefaultState ()
	{
		base.DefaultState ();

		//headJoint.rotation = Quaternion.Slerp(headJoint.rotation, originalOrientation, Time.deltaTime * angularSpeed);

		if(ventiladorAutomatico && canStartVentiladorAutomatico){
			ventiladorCooldown += Time.deltaTime;
			if(ventiladorCooldown <= auto_OnOffTimer || auto_OnOffTimer == 0f){
				fechada = false;
			} else if (ventiladorCooldown <= auto_OnOffTimer * 2f) {
				fechada = true;
			} else {
				ventiladorCooldown = 0f;
			}
		} else {
			ventiladorCooldown = 0f;
		}
	}

	protected override void Seguir ()
	{
		base.Seguir ();

		StopCoroutine ("ReturnToOriginalOrientation");

		canStartVentiladorAutomatico = false;

		if (playerIsMakingNoise) {
			PararDeSeguir ();
			return;
		}

		if (Vector3.Dot (-Vector3.up, (currentInteractionAgent.position - headJoint.position).normalized) < 0f) {
			headJoint.rotation = Quaternion.Slerp (headJoint.rotation, Quaternion.LookRotation (currentInteractionAgent.position - headJoint.position), Time.deltaTime * angularSpeed);
		} else {
			Vector3 dir = currentInteractionAgent.position - headJoint.position;
			dir.y = 0;
			headJoint.rotation = Quaternion.Slerp (headJoint.rotation, Quaternion.LookRotation (dir), Time.deltaTime * angularSpeed);
		}
	}

	protected override void PararDeSeguir ()
	{
		CancelInvoke ("PararDeSeguir");

		base.PararDeSeguir ();

		StartCoroutine ("ReturnToOriginalOrientation");
	}

	IEnumerator ReturnToOriginalOrientation (){
		yield return new WaitForSeconds (10f);
		while (Quaternion.Dot(headJoint.rotation, originalOrientation) < 0.9999f) {
			headJoint.rotation = Quaternion.Slerp(headJoint.rotation, originalOrientation, Time.deltaTime * angularSpeed);
			yield return new WaitForSeconds(Time.deltaTime);
		}
		canStartVentiladorAutomatico = true;
	}

	IEnumerator Close (){
		yield return new WaitForSeconds (3f);
		fechada = true;
	}

	protected override void Irritar ()
	{
		base.Irritar ();

		vento.tag = "Wind2";

		DefaultState ();
	}
	protected override void Acalmar ()
	{
		base.Acalmar ();

		vento.tag = "Wind";

		//canStartVentiladorAutomatico = false; 

		DefaultState ();
	}

	protected override void Dormir ()
	{
		base.Dormir ();

		fechada = true;
	}
	protected override void Acordar ()
	{
		base.Acordar ();

		canStartVentiladorAutomatico = true;
		fechada = false;
		ventiladorCooldown = 0f;
	}

//	protected override void OnTriggerEnter (Collider col)
//	{
//		base.OnTriggerEnter (col);
//
//		if((layerMask.value & 1<<col.gameObject.layer) == 0){
//			return;
//		}
//
//		if (col.CompareTag ("Player") && (!ventiladorAutomatico || currentState == Planta_CurrentState.Seguindo)) {
//			StopCoroutine ("Close");
//			fechada = false;
//		}
//	}

	protected override void OnTriggerStay (Collider col)
	{
		base.OnTriggerStay (col);

		if((layerMask.value & 1<<col.gameObject.layer) == 0){
			return;
		}

		if (col.CompareTag ("Player") && (!ventiladorAutomatico || currentState == Planta_CurrentState.Seguindo)) {
			StopCoroutine ("Close");
			fechada = false;
		}
	}

	protected override void OnTriggerExit (Collider col)
	{
		base.OnTriggerExit (col);

		if(col.CompareTag("Player") && !ventiladorAutomatico){
			StartCoroutine ("Close");
		}
	}
}
