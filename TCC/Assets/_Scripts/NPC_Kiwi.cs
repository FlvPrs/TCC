using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Kiwi : NPCBehaviour {

	Rigidbody rb;

	float defaultStopDist;

	float distToPlayer;
	float timer_StartPatrulha = 0;
	float timer_Distraido = 0;
	float timer_PegarObjeto = 0;
	Vector3 patrulhaStartPos;
	bool fugindo, patrulhando, isOnArbusto, podePegarObj, isOnWind;

	List<Transform> collObjects = new List<Transform> ();
	Transform objetoCarregado;

	protected override void Awake ()
	{
		base.Awake ();

		rb = GetComponent<Rigidbody> ();

		defaultStopDist = nmAgent.stoppingDistance;
	}

	protected override void Update ()
	{
		if (isOnWind)
			return;

		distToPlayer = Vector3.Distance (player.position, npcTransform.position);

		base.Update ();

		if (currentState != NPC_CurrentState.DefaultState)
			ResetDefaultBehaviour ();

		if(timer_PegarObjeto > 0f){
			timer_PegarObjeto -= Time.deltaTime;
			podePegarObj = false;
		} else {
			timer_PegarObjeto = 0f;
			podePegarObj = true;
		}

		if(!isOnArbusto){
			nmAgent.stoppingDistance = defaultStopDist;
		}
	}

	protected override void DefaultState ()
	{
		if(currentState == NPC_CurrentState.Distraido){
			Distrair ();
			return;
		}

		base.DefaultState ();

		if (!isOnArbusto) {
			//Normalmente, Foge do player quando este se aproxima.
			if (distToPlayer < 7f) {
				patrulhando = false;
				timer_StartPatrulha = 0;
				currentSong = PlayerSongs.Empty;

				if (objetoCarregado != null)
					SoltarObjeto ();

				if (!fugindo) {
					fugindo = true;
					FleeFromPlayer ();
				} else {
					if (!nmAgent.pathPending && !nmAgent.isStopped) {
						if (nmAgent.remainingDistance <= nmAgent.stoppingDistance) {
							if (!nmAgent.hasPath || nmAgent.velocity.sqrMagnitude == 0f) {
								FleeFromPlayer ();
							}
						}
					}
				}

			} else {
				fugindo = false;
			}

			//Se ficar 10s parado, anda pra um lugar aleatorio perto de onde está
			if (timer_StartPatrulha >= 7f) {

				if (!patrulhando) {
					patrulhando = true;
					patrulhaStartPos = npcTransform.position;
				}

				if (!nmAgent.pathPending && !nmAgent.isStopped) {
					if (nmAgent.remainingDistance <= nmAgent.stoppingDistance) {
						if (!nmAgent.hasPath || nmAgent.velocity.sqrMagnitude == 0f) {
//							if (timer_StartPatrulha < 20f) {
							Vector2 circleRand = new Vector2 (patrulhaStartPos.x, patrulhaStartPos.z) + (20f * Random.insideUnitCircle);
							Vector3 dest = new Vector3 (circleRand.x, npcTransform.position.y, circleRand.y);
							nmAgent.SetDestination (dest);
//							} else {
//								nmAgent.SetDestination (patrulhaStartPos);
//								if (!nmAgent.pathPending && !nmAgent.isStopped) {
//									if (nmAgent.remainingDistance <= nmAgent.stoppingDistance) {
//										if (!nmAgent.hasPath || nmAgent.velocity.sqrMagnitude == 0f) {
//											timer_StartPatrulha = 0f;
//										}
//									}
//								}
//							}
						}
					}
				}
			}
			timer_StartPatrulha += Time.deltaTime;
		} 
		else { //Se ele estiver dentro de um Arbusto...
			nmAgent.stoppingDistance = 0f;
			timer_StartPatrulha = 0f;
		}
	}

	void FleeFromPlayer (){
		Vector3 fleeDirection = npcTransform.position + (npcTransform.position - player.position).normalized * 10f;
		Vector2 circleRand = new Vector2 (fleeDirection.x, fleeDirection.z) + (5f * Random.insideUnitCircle);
		Vector3 dest = new Vector3 (circleRand.x, npcTransform.position.y, circleRand.y);
		nmAgent.SetDestination (dest);
	}

	void ResetDefaultBehaviour (){
		patrulhando = fugindo = false;
		timer_StartPatrulha = 0;
		isOnArbusto = false;
	}

	protected override void Seguir ()
	{
		if(currentState != NPC_CurrentState.Distraido && currentState != NPC_CurrentState.Seguindo){
			DefaultState ();
			return;
		}

		base.Seguir ();
	}

	protected override void Distrair ()
	{
		//base.Distrair faz a função retornar se já estiver distraído.
		//base.Distrair ();

		if (currentState != NPC_CurrentState.Distraido){
			currentState = NPC_CurrentState.Distraido;
			timer_Distraido = 15f;
		}

		if(timer_Distraido > 0f){
			timer_Distraido -= Time.deltaTime;
		} else {
			timer_Distraido = 0f;
			currentState = NPC_CurrentState.DefaultState;
		}

	}

	protected override void Irritar ()
	{
		//TODO: O que ficar "Agitado" significa?
		//Agitado -> 10s -> Normal

		if (objetoCarregado != null)
			SoltarObjeto ();

		base.Irritar ();
	}

	protected override void Acalmar ()
	{
		if(currentState != NPC_CurrentState.Distraido && currentState != NPC_CurrentState.Calmo){
			DefaultState ();
			return;
		}

		base.Acalmar ();
	}


	IEnumerator PegarObjeto (){
		yield return new WaitForSeconds (0.25f);

		float dist = 1000f;
		int index = 0;
		for (int i = 0; i < collObjects.Count; i++) {
			float temp = Vector3.Distance (npcTransform.position, collObjects [i].position);
			if(temp < dist){
				dist = temp;
				index = i;
			}
		}

		if(index < collObjects.Count)
			CarregarObjeto (collObjects[index]);
	}

	void CarregarObjeto (Transform obj){
		objetoCarregado = obj;

		objetoCarregado.SetParent (npcTransform);
		objetoCarregado.localPosition = new Vector3 (0, 2, 0);
	}

	void SoltarObjeto (){
		objetoCarregado.SetParent (null);
		objetoCarregado = null;
		timer_PegarObjeto = 2f;
	}

	void OnTriggerEnter (Collider col){
		if(col.CompareTag("Fruta") || col.CompareTag("Semente") || col.CompareTag("PaiDebilitado")){
			if (objetoCarregado == null && podePegarObj) {
				if(!collObjects.Contains(col.transform)){
					collObjects.Add (col.transform);
					StopCoroutine ("PegarObjeto");
					StartCoroutine ("PegarObjeto");
				}
			}
		}

		if(col.CompareTag("Arbusto")){
			if(currentState == NPC_CurrentState.DefaultState && !isOnArbusto){
				nmAgent.SetDestination (col.transform.position);
				isOnArbusto = true;
			}
		}
	}
	void OnTriggerStay (Collider col){
		if(col.CompareTag("Wind")){
			isOnWind = true;
			nmAgent.enabled = false;
			rb.isKinematic = false;
			rb.velocity = col.transform.up * 30f;
		} else if(col.CompareTag("Wind2")){
			isOnWind = true;
			nmAgent.enabled = false;
			rb.isKinematic = false;
			rb.velocity = col.transform.up * 60f;
		}
	}
	void OnTriggerExit (Collider col){
		if(col.CompareTag("Fruta") || col.CompareTag("Semente") || col.CompareTag("PaiDebilitado")){
			if(collObjects.Contains(col.transform)){
				collObjects.Remove (col.transform);
			}
		}

		if(col.CompareTag("Arbusto")){
			isOnArbusto = false;
		}

		if(col.CompareTag("Wind") || col.CompareTag("Wind2")){
			isOnWind = false;
			nmAgent.enabled = true;
			if (!nmAgent.isOnNavMesh)
				nmAgent.enabled = false;
			else {
				nmAgent.SetDestination (npcTransform.position + (col.transform.up * 5f));
			}
			rb.isKinematic = true;
			rb.velocity = Vector3.zero;
		}
	}
}
