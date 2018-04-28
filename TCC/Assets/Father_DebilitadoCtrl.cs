using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FatherConditions { Disposto, Debilitado, Machucado, MuitoMachucado }

public class Father_DebilitadoCtrl : MonoBehaviour {

	FatherActions fatherActions;
	UnityEngine.AI.NavMeshAgent nmAgent;

	public FatherConditions currentDisposition;

	[HideInInspector]
	public bool carregadoPorKiwis;
	int numeroDeKiwis = 0;

	int frutasComidas = 0;

	// Use this for initialization
	void Start () {
		fatherActions = GetComponent<FatherActions> ();
		nmAgent = GetComponent<UnityEngine.AI.NavMeshAgent> ();
	}
	
	// Update is called once per frame
	void Update () {
		fatherActions.currentDisposition = currentDisposition;

		switch (currentDisposition) {
		case FatherConditions.Disposto: //Andando normal
			tag = "NPC_Pai";
			break;
		case FatherConditions.Debilitado: //Andando com dificuldade
			tag = "PaiDebilitado";
			break;
		case FatherConditions.Machucado: //Parado
			tag = "PaiDebilitado";
			AskForFruit ();
			break;
		case FatherConditions.MuitoMachucado: //Parado quase morto
			if (carregadoPorKiwis) {
				tag = "NPC_Pai";
				StopCarriedByKiwis ();
			}
			AskForFruit (3);
			break;
		default:
			break;
		}

		if (carregadoPorKiwis)
			fatherActions.LookAtPlayer ();
	}

	public bool CanBeCarriedByKiwis (){
		if (carregadoPorKiwis)
			return false;
		
		numeroDeKiwis++;
		if(numeroDeKiwis >= 2){
			nmAgent.enabled = false;
			return true;
		}

		return false;
	}

	public void ResetNumeroDeKiwis (){
		numeroDeKiwis = 0;
	}

	public void StopCarriedByKiwis (){
		carregadoPorKiwis = false;
		numeroDeKiwis = 0;
		transform.SetParent (null);
		nmAgent.enabled = true;
	}

	void AskForFruit (int numberOfFruits = 1){
		//TODO colocar balão no HUD. 
			//Se for Debilitado, o balão some depois de um tempo.
			//Se for Machucado, o balão some depois de um tempo, depois aparece de novo,...
			//Se for MuitoMachucado, o balão nunca some.
		if(frutasComidas >= numberOfFruits){
			frutasComidas = 0;
			currentDisposition 	= FatherConditions.Debilitado;
		}
	}

	public void Revigorar (GameObject fruta){
		if (currentDisposition == FatherConditions.MuitoMachucado || currentDisposition == FatherConditions.Machucado) {
			frutasComidas++;
			Destroy (fruta);
		} else if (currentDisposition == FatherConditions.Debilitado) {
			currentDisposition = FatherConditions.Disposto;
			Destroy (fruta);
		}
	}

	void OnTriggerStay (Collider col){
		if (!carregadoPorKiwis) {
			if (col.CompareTag ("Wind")) {
				nmAgent.Move (col.transform.up * 0.02f);
			} else if (col.CompareTag ("Wind2")) {
				nmAgent.Move (col.transform.up * 0.05f);
			}
		}
	}
}
