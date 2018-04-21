using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planta_Cura : PlantaBehaviour {

	bool fechada = true;
	Vector3 frutaInitPos;
	public int numeroDeFrutas = 3;
	public Transform frutaContainer;
	public GameObject frutaPrefab;

	Transform debug_FechadaIndicator;

	void Start (){
		frutaInitPos = frutaContainer.position;

		debug_FechadaIndicator = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
		debug_FechadaIndicator.GetComponent<SphereCollider> ().enabled = false;
		debug_FechadaIndicator.SetParent (this.transform);
		debug_FechadaIndicator.localPosition = Vector3.zero + Vector3.up * 3f;
		debug_FechadaIndicator.localScale = Vector3.one * 3f;
	}

	protected override void Crescer ()
	{
		if (currentState == Planta_CurrentState.Crescendo)
			return;
		
		base.Crescer ();


		//TODO: Fazer abrir por animação
		if(fechada){
			fechada = false;
			if (numeroDeFrutas > 0) {
				numeroDeFrutas--;
				StartCoroutine ("DropFruta");
			}
			Destroy (debug_FechadaIndicator.gameObject);
		}
	}

	protected override void Encolher ()
	{
		base.Encolher ();

		if(!fechada){
			fechada = true;
			//TODO: Fazer fechar por animação
			debug_FechadaIndicator = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
			debug_FechadaIndicator.GetComponent<SphereCollider> ().enabled = false;
			debug_FechadaIndicator.SetParent (this.transform);
			debug_FechadaIndicator.localPosition = Vector3.zero + Vector3.up * 3f;
			debug_FechadaIndicator.localScale = Vector3.one * 3f;
		}
	}

	IEnumerator DropFruta (){
		GameObject fruta = Instantiate (frutaPrefab);
		fruta.transform.position = frutaInitPos;
		yield return new WaitForSeconds (2f);
		fruta.GetComponent<FrutaDeCura_Controller> ().CairDaPlanta ();
	}
}
