using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FatherConditions { Disposto, Debilitado, Machucado, MuitoMachucado }

public class Father_DebilitadoCtrl : MonoBehaviour {

	bool isInverno;

	BoxCollider dadColl;

	FatherActions fatherActions;
	UnityEngine.AI.NavMeshAgent nmAgent;

	public FatherConditions currentDisposition;

	[HideInInspector]
	public bool carregadoPorKiwis;
	int numeroDeKiwis = 0;

	int frutasComidas = 0;

	float askHealingCooldown = 0f;

	public float delayFirstAskBy = 0f;
	public bool canBeCarried = true;

	private List<NPC_Kiwi> kiwis;

	private BalaoFeedback_Ctrl balaoFeedback;

	void Awake (){
		askHealingCooldown = delayFirstAskBy;
		kiwis = new List<NPC_Kiwi> (2);

		balaoFeedback = transform.Find ("BalaoFeedback").GetComponent<BalaoFeedback_Ctrl> ();
	}

	// Use this for initialization
	void Start () {
		fatherActions = GetComponent<FatherActions> ();
		nmAgent = GetComponent<UnityEngine.AI.NavMeshAgent> ();
		dadColl = GetComponent<BoxCollider> ();

		if (UnityEngine.SceneManagement.SceneManager.GetActiveScene ().name == "Ato4")
			isInverno = true;
	}
	
	// Update is called once per frame
	void Update () {
		fatherActions.currentDisposition = currentDisposition;

		CanBeCarriedByKiwis ();

		switch (currentDisposition) {
		case FatherConditions.Disposto: //Andando normal
			tag = "NPC_Pai";
			if (carregadoPorKiwis) {
				//transform.GetComponentInParent<NPC_Kiwi> ().SoltarObjeto ();
			}
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
				askHealingCooldown = 0f;
				tag = "NPC_Pai";
				for (int i = 0; i < kiwis.Count; i++) {
					kiwis [i].SoltarObjeto ();
				}
				//transform.GetComponentInParent<NPC_Kiwi> ().SoltarObjeto ();
			}
			if(isInverno)
				AskForFruit (3);
			else
				AskForFruit ();
			break;
		default:
			break;
		}

//		if (carregadoPorKiwis)
//			fatherActions.LookAtPlayer ();

		fatherActions.animCtrl.SetBool ("beingCarriedByKiwis", carregadoPorKiwis);
	}

	public void CanBeCarriedByKiwis (){
		if (carregadoPorKiwis || !canBeCarried)
			return;
		
		if(numeroDeKiwis > 1){
			nmAgent.enabled = false;
			carregadoPorKiwis = true;

			for (int i = 0; i < kiwis.Count; i++) {
				StartCoroutine (kiwis [i].PegarPai (transform));
			}
		}
	}

	public void ResetNumeroDeKiwis (){
		numeroDeKiwis = 0;
	}

	public void StopCarriedByKiwis (){
		carregadoPorKiwis = false;
		for (int i = 0; i < kiwis.Count; i++) {
			kiwis.RemoveAt(i);
		}
		numeroDeKiwis = 0;
		transform.SetParent (null);
		nmAgent.enabled = true;
	}

	void AskForFruit (int numberOfFruits = 1){
		//TODO colocar balão no HUD. 
			//Se for Debilitado, o balão some depois de um tempo.
			//Se for Machucado, o balão some depois de um tempo, depois aparece de novo,...
			//Se for MuitoMachucado, o balão nunca some.

		CancelInvoke("ResetHealingCooldown");
		if(askHealingCooldown > 0f){
			askHealingCooldown -= Time.deltaTime;
		} else {
			fatherActions.animCtrl.SetTrigger ("askForHealing");
			balaoFeedback.ShowBalaoCura (numberOfFruits, frutasComidas);
			askHealingCooldown = (currentDisposition == FatherConditions.MuitoMachucado) ? 8f : (currentDisposition == FatherConditions.Machucado) ? 20f : 70f;
		}

		if(frutasComidas >= numberOfFruits){
			frutasComidas = 0;
			if(currentDisposition != FatherConditions.Debilitado)
				currentDisposition = FatherConditions.Debilitado;
			else
				currentDisposition = FatherConditions.Disposto;
		}

		Invoke ("ResetHealingCooldown", 0.5f);
	}

	void ResetHealingCooldown (){
		askHealingCooldown = 0f;
	}

	public void Revigorar (GameObject fruta){
		if (currentDisposition == FatherConditions.MuitoMachucado || currentDisposition == FatherConditions.Machucado) {
			frutasComidas++;
			Destroy (fruta);
		} else if (currentDisposition == FatherConditions.Debilitado) {
			currentDisposition = FatherConditions.Disposto;
			Destroy (fruta);
		}

		StartCoroutine ("BlinkCollider");
	}

	IEnumerator BlinkCollider (){
		dadColl.enabled = false;
		yield return new WaitForSeconds (0.25f);
		dadColl.enabled = true;
	}

//	void OnTriggerEnter (Collider col){
//		if(col.GetComponent<NPC_Kiwi>() != null){
//			for (int i = 0; i < kiwis.Length; i++) {
//				if (kiwis [i] == null) {
//					if (col.GetComponent<NPC_Kiwi> ().CanCarryPai ()) {
//						kiwis [i] = col.GetComponent<NPC_Kiwi> ();
//						numeroDeKiwis++;
//					}
//				}
//			}
//		}
//	}

	void OnTriggerStay (Collider col){
		if(col.GetComponent<NPC_Kiwi>() != null){
			if (kiwis.Count < 2) {
				if (!kiwis.Contains (col.GetComponent<NPC_Kiwi> ())) {
					if (col.GetComponent<NPC_Kiwi> ().CanCarryPai ()) {
						kiwis.Add (col.GetComponent<NPC_Kiwi> ());
						numeroDeKiwis++;
					}
				}
			}
		}

		if (!carregadoPorKiwis) {
			if (col.CompareTag ("Wind")) {
				nmAgent.Move (col.transform.up * 0.02f);
			} else if (col.CompareTag ("Wind2")) {
				nmAgent.Move (col.transform.up * 0.05f);
			}
		}
	}

	void OnTriggerExit (Collider col){
		if(col.GetComponent<NPC_Kiwi>() != null){
			if (kiwis.Contains (col.GetComponent<NPC_Kiwi> ())) {
				kiwis.Remove (col.GetComponent<NPC_Kiwi> ());
			}
		}
	}
}
