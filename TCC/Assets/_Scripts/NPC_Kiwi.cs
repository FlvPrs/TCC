using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class NPC_Kiwi : NPCBehaviour, ICarnivoraEdible {

	Rigidbody rb;

	float defaultStopDist;

	float distToPlayer;
	float timer_StartPatrulha = 0;
	float timer_Distraido = 0;
	float timer_PegarObjeto = 0;
	Vector3 patrulhaStartPos;
	bool fugindo, patrulhando, isOnArbusto, podePegarObj, isOnWind, isCarregandoPai;

	List<Transform> collObjects = new List<Transform> ();
	Transform objetoCarregado;

	bool stopUpdate;
	Vector3 spawnPoint;
	Quaternion initialOrientation;

	private AudioSource simpleAudioSource;
	private AudioSource sustainAudioSource;

	public AudioClip[] carregando_Clips;
	public AudioClip[] seguindo_Clips;
	public AudioClip freeWalk_Clip;
	public AudioClip[] ventoStates_Clip;
	bool canStartVentoLoop = true;
	int currClipIndex = 0;
	bool canChangeSimpleClip = true;

	bool movingToPai = false;

	protected override void Awake ()
	{
		base.Awake ();

		rb = GetComponent<Rigidbody> ();
		simpleAudioSource = GetComponent<AudioSource>();
		simpleAudioSource.loop = false;
		sustainAudioSource = transform.Find("AudioSourceLoop").GetComponent<AudioSource>();
		sustainAudioSource.loop = true;

		if (npcTransform.parent != null && npcTransform.parent.name == "NPC_SpawnPoint")
			spawnPoint = npcTransform.parent.position;
		else
			spawnPoint = npcTransform.position;
		
		initialOrientation = npcTransform.rotation;

		defaultStopDist = nmAgent.stoppingDistance;

		for (int i = 0; i < npcTransform.childCount; i++) {
			if(npcTransform.GetChild(i).GetComponent<Animator>() != null)
				npcTransform.GetChild(i).GetComponent<Animator>().SetFloat ("idleStartAt", Random.Range (0f, 1f));
		}
	}

	protected override void Update ()
	{
		for (int i = 0; i < npcTransform.childCount; i++) {
			if(npcTransform.GetChild(i).GetComponent<Animator>() != null)
				npcTransform.GetChild(i).GetComponent<Animator>().SetBool ("isWalking", (nmAgent.velocity != Vector3.zero));
		}

		if (stopUpdate)
			return;

		if (isOnWind)
			return;

		if(nmAgent.velocity != Vector3.zero && canChangeSimpleClip){
			if(currentState == NPC_CurrentState.DefaultState){
				if (simpleAudioSource.clip != freeWalk_Clip) {
					simpleAudioSource.clip = freeWalk_Clip;
				}
				simpleAudioSource.Play ();
				StartCoroutine(WaitForSimpleClipToEnd (freeWalk_Clip.length - 0.03f));
			} else if (currentState == NPC_CurrentState.Seguindo && !isCarregandoPai) {
				simpleAudioSource.clip = seguindo_Clips [currClipIndex];
				currClipIndex++;
				if (currClipIndex >= seguindo_Clips.Length)
					currClipIndex = 0;
				simpleAudioSource.Play ();
				StartCoroutine(WaitForSimpleClipToEnd (simpleAudioSource.clip.length));
			} else if (isCarregandoPai) {
				simpleAudioSource.clip = carregando_Clips [currClipIndex];
				currClipIndex++;
				if (currClipIndex >= carregando_Clips.Length)
					currClipIndex = 0;
				simpleAudioSource.Play ();
				StartCoroutine(WaitForSimpleClipToEnd (simpleAudioSource.clip.length - 0.05f));
			}
		}
			

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

		if(!isOnArbusto && !movingToPai){
			nmAgent.stoppingDistance = defaultStopDist;
		}

		if (!podePegarObj && objetoCarregado == null){
			SoltarObjeto ();
			collObjects.Capacity = 0;
			timer_PegarObjeto = 0f;
		}
	}

	protected override void DefaultState ()
	{
		if(currentState == NPC_CurrentState.Distraido){
			Distrair ();
			return;
		} else if (currentState == NPC_CurrentState.Seguindo) {
			Seguir ();
			return;
		}

		base.DefaultState ();

		if (!isOnArbusto) {
			//Normalmente, Foge do player quando este se aproxima.
			if (distToPlayer < 7f && !isCarregandoPai) {
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
			if (timer_StartPatrulha >= 7f && !isCarregandoPai) {

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
			nmAgent.stoppingDistance = 0.5f;
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
		if (!isCarregandoPai && currentState != NPC_CurrentState.Distraido && currentState != NPC_CurrentState.Seguindo) {
			DefaultState ();
			return;
		} else if(movingToPai) {
			Distrair ();
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
			if(collObjects [i] == null){
				collObjects.RemoveAt (i);
				collObjects.Capacity--;
				continue;
			}
			float temp = Vector3.Distance (npcTransform.position, collObjects [i].position);
			if(temp < dist){
				dist = temp;
				index = i;
			}
		}

		if(index < collObjects.Count)
			CarregarObjeto (collObjects[index]);
	}

	public bool CanCarryPai (){
		if (!isOnWind && podePegarObj && objetoCarregado == null && currentState == NPC_CurrentState.Seguindo) {
			return true;
		} else {
			return false;
		}
	}
	public IEnumerator PegarPai (Transform pai){
		movingToPai = true;
		nmAgent.stoppingDistance = 1f;
		nmAgent.SetDestination (pai.position);
		yield return new WaitForSeconds (0.5f);
		while (movingToPai) {
			if (!nmAgent.pathPending && !nmAgent.isStopped) {
				if (nmAgent.remainingDistance <= nmAgent.stoppingDistance) {
					if (!nmAgent.hasPath || nmAgent.velocity.sqrMagnitude == 0f) {
						movingToPai = false;
						nmAgent.stoppingDistance = defaultStopDist;
					}
				}
			}
			yield return new WaitForSeconds (0.25f);
		}
		CarregarObjeto (pai);
	}

	void CarregarObjeto (Transform obj){
		objetoCarregado = obj;

		if(objetoCarregado.CompareTag("PaiDebilitado")){
			isCarregandoPai = true;
		} else if (objetoCarregado.CompareTag("Fruta")) {
			objetoCarregado.GetComponent<FrutaDeCura_Controller> ().Freeze ();
		}

		objetoCarregado.SetParent (npcTransform);
		objetoCarregado.localPosition = new Vector3 (0, 0.3f, 0);
	}

	public void SoltarObjeto (){
		if(isCarregandoPai){
			objetoCarregado.GetComponent<Father_DebilitadoCtrl> ().StopCarriedByKiwis ();
		} else if (objetoCarregado != null) {
			if (objetoCarregado.CompareTag("Fruta")) {
				objetoCarregado.GetComponent<FrutaDeCura_Controller> ().UnFreeze ();
			}
			objetoCarregado.SetParent (null);
		}
		objetoCarregado = null;
		isCarregandoPai = false;
		timer_PegarObjeto = 2f;
		collObjects.Clear ();
		collObjects.Capacity = 0;
	}

	void OnTriggerEnter (Collider col){
		if (stopUpdate)
			return;

		if(col.CompareTag("Fruta") || col.CompareTag("Semente")){
			if (objetoCarregado == null && podePegarObj) {
				if(!collObjects.Contains(col.transform)){
					collObjects.Add (col.transform);
					StopCoroutine ("PegarObjeto");
					StartCoroutine ("PegarObjeto");
				}
			}
		}
//		if (col.CompareTag("PaiDebilitado")) {
//			if (objetoCarregado == null && podePegarObj && col.GetComponent<Father_DebilitadoCtrl> ().CanBeCarriedByKiwis (this)) {
//				if(!collObjects.Contains(col.transform)){
//					collObjects.Add (col.transform);
//					StopCoroutine ("PegarObjeto");
//					StartCoroutine ("PegarObjeto");
//				}
//			}
//		}

		if(col.CompareTag("Arbusto") && !isCarregandoPai){
			if(currentState == NPC_CurrentState.DefaultState && !isOnArbusto){
				nmAgent.SetDestination (col.transform.position);
				isOnArbusto = true;
			}
		}

		if ((col.CompareTag ("Wind") || col.CompareTag ("Wind2")) && canStartVentoLoop) {
			if(objetoCarregado != null)
				SoltarObjeto ();

			canStartVentoLoop = false;

			simpleAudioSource.clip = ventoStates_Clip [0];
			sustainAudioSource.clip = ventoStates_Clip [1];
			canStartVentoLoop = true;
			simpleAudioSource.Play ();
			StartCoroutine(WaitForSimpleClipToEnd (simpleAudioSource.clip.length));

		}
	}
	void OnTriggerStay (Collider col){
		if(col.CompareTag("Wind")){
			StopCoroutine ("ResetNavMeshAfterWind");
			isOnWind = true;
			nmAgent.enabled = false;
			rb.isKinematic = false;
			rb.velocity = (col.transform.up * 25f) + (Vector3.down * 5f);

//			if (canChangeSimpleClip && canStartVentoLoop)
//			{
//				simpleAudioSource.clip = ventoStates_Clip [2];
//				canStartVentoLoop = false;
//				//sustainAudioSource.Play();
//			}

			StartCoroutine ("ResetNavMeshAfterWind", col.transform.up);
		} else if(col.CompareTag("Wind2")){
			StopCoroutine ("ResetNavMeshAfterWind");
			isOnWind = true;
			nmAgent.enabled = false;
			rb.isKinematic = false;
			rb.velocity = (col.transform.up * 50f) + (Vector3.down * 5f);

//			if (canChangeSimpleClip && canStartVentoLoop)
//			{
//				simpleAudioSource.clip = ventoStates_Clip [2];
//				canStartVentoLoop = false;
//				//sustainAudioSource.Play();
//			}

			StartCoroutine ("ResetNavMeshAfterWind", col.transform.up);
		}
	}
	void OnTriggerExit (Collider col){
		if(col.CompareTag("Fruta") || col.CompareTag("Semente") || col.CompareTag("PaiDebilitado")){
			if(collObjects.Contains(col.transform)){
				collObjects.Remove (col.transform);
			}
		}
//		if(col.CompareTag("PaiDebilitado")){
//			col.GetComponent<Father_DebilitadoCtrl> ().ResetNumeroDeKiwis ();
//			if(collObjects.Contains(col.transform)){
//				collObjects.Remove (col.transform);
//			}
//		}

		if(col.CompareTag("Arbusto")){
			isOnArbusto = false;
		}
	}

	IEnumerator WaitForSimpleClipToEnd (float duration){
		canChangeSimpleClip = false;
		yield return new WaitForSeconds (duration);
		canChangeSimpleClip = true;
	}

	IEnumerator ResetNavMeshAfterWind (Vector3 windUp){
		yield return new WaitForSeconds (2f);
		simpleAudioSource.clip = ventoStates_Clip [2];
		isOnWind = false;
		rb.angularVelocity = Vector3.zero;
		npcTransform.rotation = initialOrientation;
		nmAgent.enabled = true;
		if (!nmAgent.isOnNavMesh) {
			nmAgent.enabled = false;
			//enabled = false;
			//Destroy (gameObject, 5f);
			stopUpdate = true;
			int i = 0;
			while (i < 12) {
				yield return new WaitForSeconds (0.5f);
				nmAgent.enabled = true;
				if (!nmAgent.isOnNavMesh) {
					nmAgent.enabled = false;
				} else {
					sustainAudioSource.Stop ();
					simpleAudioSource.Play ();
					StartCoroutine(WaitForSimpleClipToEnd (simpleAudioSource.clip.length));

					rb.velocity = Vector3.zero;
					rb.angularVelocity = Vector3.zero;
					rb.isKinematic = true;
					npcTransform.rotation = initialOrientation;
					stopUpdate = false;
					yield break;
				}
				i++;
			}
			rb.velocity = Vector3.zero;
			rb.angularVelocity = Vector3.zero;
			rb.isKinematic = true;
			npcTransform.rotation = initialOrientation;
			yield return new WaitForSeconds (0.1f);
			nmAgent.enabled = true;
			if (!nmAgent.isOnNavMesh) {
				nmAgent.enabled = false;
				npcTransform.position = spawnPoint;
				nmAgent.enabled = true;
			}
			stopUpdate = false;
		}
		else {
			sustainAudioSource.Stop ();
			simpleAudioSource.Play ();
			StartCoroutine(WaitForSimpleClipToEnd (simpleAudioSource.clip.length));
			stopUpdate = false;
			rb.velocity = Vector3.zero;
			rb.isKinematic = true;
			nmAgent.enabled = true;
			nmAgent.SetDestination (npcTransform.position + (windUp * 5f));
		}

		canStartVentoLoop = true;
	}

	public void OnMovingPlat (bool enableNavMesh, Transform plat){
		nmAgent.enabled = enableNavMesh;
		npcTransform.parent = plat;
		//rb.isKinematic = enableNavMesh;
		stopUpdate = !enableNavMesh;
	}


	#region Carnivora Interface
	[HideInInspector]
	public bool eatenByCarnivora = false;

	public void Carnivora_GetReadyToBeEaten (){
		//print ("EATEN");
		stopUpdate = true;
		ResetDefaultBehaviour ();
		currentState = NPC_CurrentState.DefaultState;
		if(objetoCarregado != null)
			SoltarObjeto ();
		nmAgent.enabled = false;
		rb.isKinematic = true;
		rb.velocity = Vector3.zero;
	}
	public void Carnivora_Release (){
		//print ("released");
		stopUpdate = false;
		nmAgent.enabled = true;
		//timer_StartPatrulha = 7f;
		FleeFromPlayer ();
		DefaultState ();
	}

	public void Carnivora_Shoot (Vector3 dir){
		//print ("SHOOT!");
		nmAgent.enabled = false;
		rb.isKinematic = false;
		rb.velocity = (dir * .5f);
		StartCoroutine ("ResetNavMeshAfterWind", dir);
	}
	#endregion
}
