using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class NPC_Kiwi_Scripted : NPCBehaviour {

	Rigidbody rb;

	float defaultStopDist;

	float timer_PegarObjeto = 0;
	Vector3 patrulhaStartPos;
	bool podePegarObj, isOnWind, isCarregandoPai;

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

	public bool invernoBehaviour;

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
				float freeWalkRepeatTime = (!invernoBehaviour) ? freeWalk_Clip.length - 0.03f : (freeWalk_Clip.length - 0.03f) / 3f;
				StartCoroutine(WaitForSimpleClipToEnd (freeWalkRepeatTime));
			} else if (currentState == NPC_CurrentState.Seguindo && !isCarregandoPai) {
				simpleAudioSource.clip = seguindo_Clips [currClipIndex];
				currClipIndex++;
				if (currClipIndex >= seguindo_Clips.Length)
					currClipIndex = 0;
				simpleAudioSource.Play ();
				float seguindoRepeatTime = (!invernoBehaviour) ? simpleAudioSource.clip.length : simpleAudioSource.clip.length / 3f;
				StartCoroutine(WaitForSimpleClipToEnd (seguindoRepeatTime));
			} else if (isCarregandoPai) {
				simpleAudioSource.clip = carregando_Clips [currClipIndex];
				simpleAudioSource.Play ();
				StartCoroutine(WaitForSimpleClipToEnd (carregando_Clips [currClipIndex].length + 0.05f));
				currClipIndex++;
				if (currClipIndex >= carregando_Clips.Length)
					currClipIndex = 0;
			}
		}

		//base.Update ();

		if(timer_PegarObjeto > 0f){
			timer_PegarObjeto -= Time.deltaTime;
			podePegarObj = false;
		} else {
			timer_PegarObjeto = 0f;
			podePegarObj = true;
		}

		if (!podePegarObj && objetoCarregado == null){
			SoltarObjeto ();
			collObjects.Capacity = 0;
			timer_PegarObjeto = 0f;
		}

		WaypointMovement ();
	}



	public bool startWPBehaviour;
	public Transform[] waypoints;
	int currentWaypoint = 0;

	public void WaypointMovement (){
		if (!startWPBehaviour)
			return;
		
		float dist = Vector3.Distance(waypoints [currentWaypoint].position, npcTransform.position);
		if(dist < 0.5f){
			currentWaypoint++;
			if(currentWaypoint >= waypoints.Length){
				startWPBehaviour = false;
				return;
			}
		}

		nmAgent.SetDestination (waypoints [currentWaypoint].position);
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
		if (!isOnWind && podePegarObj && objetoCarregado == null && currentState == NPC_CurrentState.Seguindo && !invernoBehaviour) {
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

	public void SoltarObjeto (bool calledByFather = false){
		if(isCarregandoPai && !calledByFather){
			objetoCarregado.GetComponent<Father_DebilitadoCtrl> ().StopCarriedByKiwis ();
		} else if (!isCarregandoPai && objetoCarregado != null) {
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

		if(col.CompareTag("Fruta") && !invernoBehaviour){
			if (objetoCarregado == null && podePegarObj) {
				if(!collObjects.Contains(col.transform)){
					collObjects.Add (col.transform);
					StopCoroutine ("PegarObjeto");
					StartCoroutine ("PegarObjeto");
				}
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
			StartCoroutine ("ResetNavMeshAfterWind", col.transform.up);
		} else if(col.CompareTag("Wind2")){
			StopCoroutine ("ResetNavMeshAfterWind");
			isOnWind = true;
			nmAgent.enabled = false;
			rb.isKinematic = false;
			rb.velocity = (col.transform.up * 50f) + (Vector3.down * 5f);
			StartCoroutine ("ResetNavMeshAfterWind", col.transform.up);
		}
	}
	void OnTriggerExit (Collider col){
		if(col.CompareTag("Fruta") || col.CompareTag("Semente") || col.CompareTag("PaiDebilitado")){
			if(collObjects.Contains(col.transform)){
				collObjects.Remove (col.transform);
			}
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
}
