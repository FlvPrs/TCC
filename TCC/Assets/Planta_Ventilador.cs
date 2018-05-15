using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planta_Ventilador : PlantaBehaviour {

	public GameObject MDL_Aberta, MDL_Fechada, vento;
	public bool ventiladorAutomatico = false;
	public bool startAngry = false;
	public float auto_OnOffTimer = 2f;
	[Range(0.01f, 10f)]
	public float auto_OnOffRatio = 1f; 	//É multiplicado por auto_onOffTimer. O resultado indica quanto tempo ficará ligada.
										//1 significa o mesmo tempo ligado e desligado. 
										//2 significa que ficará ligado 2 vezes mais do que desligado. 
										//0.5 significa que ficará 2 vezes mais desligado do que ligado.
	public float delayAutoBy = 0f;
	float delayTimer;
	bool canStartVentiladorAutomatico;
	float ventiladorCooldown;

	public bool fechada = true;

	Quaternion originalOrientation;

	public float angularSpeed = 2f;
	public LayerMask layerMask;
	public Transform headJoint;

	float allowStopMoving_Timer = 0f;

	public AudioSource sustainAudioSource;
	public AudioClip[] abrirFechar_Clips;
	public AudioClip ventoNormal_Clip, irritar_Clip, ventoIrritado_Clip;

	protected override void Awake ()
	{
		base.Awake ();

		//vento = plantaTransform.Find ("Vento").gameObject;

		originalOrientation = headJoint.rotation;
		canStartVentiladorAutomatico = true;

		if(startAngry){
			vento.tag = "Wind2";
			sustainAudioSource.clip = ventoIrritado_Clip;
			sustainAudioSource.Play ();
		} else {
			vento.tag = "Wind";
			sustainAudioSource.clip = ventoNormal_Clip;
			sustainAudioSource.Play ();
		}

		delayTimer = delayAutoBy;

		if (ventiladorAutomatico && fechada)
			ventiladorCooldown = auto_OnOffRatio * auto_OnOffTimer + Time.deltaTime;
	}

	protected override void Update ()
	{
		if (delayTimer > 0f)
			delayTimer -= Time.deltaTime;
		else
			delayTimer = 0f;
		
		base.Update ();

		vento.SetActive (!fechada);
		MDL_Aberta.SetActive (!fechada);
		MDL_Fechada.SetActive (fechada);

		if (timer >= 1f && (currentState == Planta_CurrentState.Seguindo || currentState == Planta_CurrentState.Irritado)) {
			Invoke ("PararDeSeguir", 2f);
		}

		headJoint.localEulerAngles = new Vector3 (headJoint.localEulerAngles.x, headJoint.localEulerAngles.y, 0);
	}

	protected override void DefaultState ()
	{
		if(currentState == Planta_CurrentState.Irritado){
			Irritar ();
			return;
		}

		base.DefaultState ();

		//headJoint.rotation = Quaternion.Slerp(headJoint.rotation, originalOrientation, Time.deltaTime * angularSpeed);

		if (delayTimer == 0f) {
			if (ventiladorAutomatico && canStartVentiladorAutomatico) {
				ventiladorCooldown += Time.deltaTime;
				if (ventiladorCooldown <= auto_OnOffRatio * auto_OnOffTimer || auto_OnOffTimer == 0f) {
					if(fechada){
						simpleAudioSource.clip = abrirFechar_Clips[0];
						simpleAudioSource.Play ();
						StartCoroutine(WaitForSimpleClipToEnd (simpleAudioSource.clip.length));
					}
					fechada = false;
				} else if (ventiladorCooldown <= auto_OnOffTimer + (auto_OnOffRatio * auto_OnOffTimer)) {
					if(!fechada){
						simpleAudioSource.clip = abrirFechar_Clips[1];
						simpleAudioSource.Play ();
						StartCoroutine(WaitForSimpleClipToEnd (simpleAudioSource.clip.length));
					}
					fechada = true;
				} else {
					ventiladorCooldown = 0f;
				}
			} else {
				ventiladorCooldown = 0f;
			}
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

		Vector3 dir = currentInteractionAgent.position - headJoint.position;

		if (Vector3.Dot (-Vector3.up, dir.normalized) < 0f) {
			headJoint.rotation = Quaternion.Slerp (headJoint.rotation, Quaternion.LookRotation (dir), Time.deltaTime * angularSpeed);
		} else {
			dir.y = 0;
			headJoint.rotation = Quaternion.Slerp (headJoint.rotation, Quaternion.LookRotation (dir), Time.deltaTime * angularSpeed);
		}
	}

	protected override void PararDeSeguir ()
	{
		CancelInvoke ("PararDeSeguir");
		base.PararDeSeguir ();

		sustainAudioSource.clip = ventoNormal_Clip;
		sustainAudioSource.Play ();

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
		if(!fechada){
			simpleAudioSource.clip = abrirFechar_Clips[1];
			simpleAudioSource.Play ();
			StartCoroutine(WaitForSimpleClipToEnd (simpleAudioSource.clip.length));
		}
		fechada = true;
	}

	protected override void Irritar ()
	{
		if(currentState != Planta_CurrentState.Irritado){
			sustainAudioSource.clip = ventoIrritado_Clip;
			sustainAudioSource.Play ();
			simpleAudioSource.clip = irritar_Clip;
			simpleAudioSource.Play ();
			StartCoroutine(WaitForSimpleClipToEnd (simpleAudioSource.clip.length));
		}

		currentState = Planta_CurrentState.Irritado;
		//base.Irritar ();

		//vento.tag = "Wind2";

		//DefaultState ();

		StopCoroutine ("ReturnToOriginalOrientation");

		canStartVentiladorAutomatico = false;

		if (allowStopMoving_Timer < 1f) {
			allowStopMoving_Timer += Time.deltaTime;
		} else {
			if (playerIsMakingNoise) {
				allowStopMoving_Timer = 0f;
				PararDeSeguir ();
				return;
			}
		}

		Vector3 dir = headJoint.position - currentInteractionAgent.position;

		if (Vector3.Dot (-Vector3.up, dir.normalized) < 0f) {
			headJoint.rotation = Quaternion.Slerp (headJoint.rotation, Quaternion.LookRotation (dir), Time.deltaTime * angularSpeed);
		} else {
			dir.y = 0;
			headJoint.rotation = Quaternion.Slerp (headJoint.rotation, Quaternion.LookRotation (dir), Time.deltaTime * angularSpeed);
		}
	}
	protected override void Acalmar ()
	{
		base.Acalmar ();

		sustainAudioSource.clip = ventoNormal_Clip;
		sustainAudioSource.Play ();

		//vento.tag = "Wind";

		//canStartVentiladorAutomatico = false; 

		DefaultState ();
	}

	protected override void Dormir ()
	{
		base.Dormir ();

		if(!fechada){
			simpleAudioSource.clip = abrirFechar_Clips[1];
			simpleAudioSource.Play ();
			StartCoroutine(WaitForSimpleClipToEnd (simpleAudioSource.clip.length));
		}
		fechada = true;
	}
	protected override void Acordar ()
	{
		base.Acordar ();

		canStartVentiladorAutomatico = true;

		if(fechada){
			sustainAudioSource.clip = ventoNormal_Clip;
			sustainAudioSource.Play ();
			simpleAudioSource.clip = abrirFechar_Clips[0];
			simpleAudioSource.Play ();
			StartCoroutine(WaitForSimpleClipToEnd (simpleAudioSource.clip.length));
		}
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

		if (col.CompareTag ("Player") && currentState != Planta_CurrentState.Dormindo && (!ventiladorAutomatico || currentState == Planta_CurrentState.Seguindo || currentState == Planta_CurrentState.Irritado)) {
			StopCoroutine ("Close");
			if(fechada){
				simpleAudioSource.clip = abrirFechar_Clips[0];
				simpleAudioSource.Play ();
				StartCoroutine(WaitForSimpleClipToEnd (simpleAudioSource.clip.length));
			}
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
