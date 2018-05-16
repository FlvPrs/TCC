using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planta_Cura : PlantaBehaviour {

	bool fechada = true;
	Vector3 frutaInitPos;
	public int numeroDeFrutas = 3;
	public Transform frutaContainer;
	public GameObject frutaPrefab, plantaFechada, plantaAberta;

	public AudioClip abrindo_Clip, fechando_Clip;

	float aberta_Timer = 0f;

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

		if (!fechada) {
			if (aberta_Timer > 10f)
				Encolher ();
			else
				aberta_Timer += Time.deltaTime;
		}
	}

	protected override void Crescer ()
	{
		//print ("Cresça!");
		if (currentState == Planta_CurrentState.Crescendo)
			return;
		
		base.Crescer ();

		aberta_Timer = 0f;

		//TODO: Fazer abrir por animação
		if(fechada){
			simpleAudioSource.clip = abrindo_Clip;
			simpleAudioSource.Play ();

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

		aberta_Timer = 0f;

		if(!fechada){
			simpleAudioSource.clip = fechando_Clip;
			simpleAudioSource.Play ();

			fechada = true;
			//TODO: Fazer fechar por animação
		}
	}

	IEnumerator DropFruta (){
		GameObject fruta = Instantiate (frutaPrefab);
		fruta.transform.position = frutaInitPos;
		yield return new WaitForSeconds (1f);
		fruta.GetComponent<FrutaDeCura_Controller> ().CairDaPlanta (numeroDeFrutas);
	}
}
