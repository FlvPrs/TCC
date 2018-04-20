using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planta_Ventilador : PlantaBehaviour {

	GameObject vento;

	bool fechada = true;

	public LayerMask layerMask;

	protected override void Awake ()
	{
		base.Awake ();

		vento = plantaTransform.Find ("Vento").gameObject;
	}

	protected override void DefaultState ()
	{
		base.DefaultState ();

		if(fechada){
			vento.SetActive (false);
		} else {
			vento.SetActive (true);
		}
	}

	IEnumerator Close (){
		yield return new WaitForSeconds (2f);
		fechada = true;
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
