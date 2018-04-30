using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planta_Cura : PlantaBehaviour {

	bool fechada = true;
	Vector3 frutaInitPos;
	public int numeroDeFrutas = 3;
	public Transform frutaContainer;
	public GameObject frutaPrefab, plantaFechada, plantaAberta;

	void Start (){
		frutaInitPos = frutaContainer.position;

		plantaFechada.SetActive (fechada);
		plantaAberta.SetActive (!fechada);
	}

	protected override void Update ()
	{
		base.Update ();

		plantaFechada.SetActive (fechada);
		plantaAberta.SetActive (!fechada);
	}

	protected override void Crescer ()
	{
		print ("Cresça!");
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
		}
	}

	protected override void Encolher ()
	{
		base.Encolher ();

		if(!fechada){
			fechada = true;
			//TODO: Fazer fechar por animação
		}
	}

	IEnumerator DropFruta (){
		GameObject fruta = Instantiate (frutaPrefab);
		fruta.transform.position = frutaInitPos;
		yield return new WaitForSeconds (1f);
		fruta.GetComponent<FrutaDeCura_Controller> ().CairDaPlanta ();
	}
}
