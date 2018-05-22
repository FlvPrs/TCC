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

	public bool carregadoPorKiwis;
	int numeroDeKiwis = 0;

	int frutasComidas = 0;

	float askHealingCooldown = 0f;
	float askKiwiCooldown = 0f;

	public float delayFirstAskBy = 0f;
	public float delayAllAsksBy = 0f;
	public bool canBeCarriedByDefault = true;



	bool canBeCarried = true;

	private List<NPC_Kiwi> kiwis;

	private BalaoFeedback_Ctrl balaoFeedback;

	void Awake (){
		askHealingCooldown = delayFirstAskBy;
		askKiwiCooldown = delayFirstAskBy;
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
			canBeCarried = false;
			break;
		case FatherConditions.Debilitado: //Andando com dificuldade
			canBeCarried = false;
			break;
		case FatherConditions.Machucado: //Parado, dependendo do caso
			canBeCarried = true;
			if(!carregadoPorKiwis)
				AskForKiwi ();
			break;
		case FatherConditions.MuitoMachucado: //Parado quase morto
			canBeCarried = false;
			if (carregadoPorKiwis) {
				askHealingCooldown = 0f;
				for (int i = 0; i < kiwis.Count; i++) {
					kiwis [i].SoltarObjeto (true);
				}
				StopCarriedByKiwis ();
			}
			if(isInverno)
				AskForFruit (3);
			else
				AskForFruit ();
			break;
		default:
			break;
		}

		if (carregadoPorKiwis)
			fatherActions.LookAtPlayer ();

		fatherActions.animCtrl.SetBool ("beingCarriedByKiwis", carregadoPorKiwis);
	}

	public void CanBeCarriedByKiwis (){
		if (carregadoPorKiwis || !canBeCarriedByDefault || !canBeCarried)
			return;

		if (numeroDeKiwis < 0)
			numeroDeKiwis = 0;

		if(numeroDeKiwis > 1){
			nmAgent.enabled = false;
			carregadoPorKiwis = true;

			for (int i = 0; i < kiwis.Count; i++) {
				StartCoroutine (kiwis [i].PegarPai (transform));
			}
		}
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

	void AskForKiwi (){
		CancelInvoke("ResetKiwiBalaoCooldown");
		if(askKiwiCooldown > 0f){
			askKiwiCooldown -= Time.deltaTime;

		} else {
			fatherActions.animCtrl.SetTrigger ("askForHealing");
			balaoFeedback.ShowBalao(balaoTypes.kiwi);
			//askHealingCooldown = (currentDisposition == FatherConditions.MuitoMachucado) ? 8f : (currentDisposition == FatherConditions.Machucado) ? 20f : 70f;
			askKiwiCooldown = 8f + delayAllAsksBy;
		}

		Invoke ("ResetKiwiBalaoCooldown", 0.5f);
	}
	void ResetKiwiBalaoCooldown (){
		askKiwiCooldown = 0f;
	}

	void AskForFruit (int numberOfFruits = 1){
		CancelInvoke("ResetHealingCooldown");
		if(askHealingCooldown > 0f){
			askHealingCooldown -= Time.deltaTime;

		} else {
			fatherActions.animCtrl.SetTrigger ("askForHealing");
			balaoFeedback.ShowBalaoCura (numberOfFruits, frutasComidas);
			//askHealingCooldown = (currentDisposition == FatherConditions.MuitoMachucado) ? 8f : (currentDisposition == FatherConditions.Machucado) ? 20f : 70f;
			askHealingCooldown = 8f + delayAllAsksBy;
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

		StopCarriedByKiwis ();

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
//				else if (!col.GetComponent<NPC_Kiwi> ().CanCarryPai ()) {
//					kiwis.Remove (col.GetComponent<NPC_Kiwi> ());
//					numeroDeKiwis--;
//				}
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
				numeroDeKiwis--;
			}
		}
	}
}
