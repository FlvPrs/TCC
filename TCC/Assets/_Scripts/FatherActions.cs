using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FSM_WalkStates { SimpleWalk, FollowingPlayer, GuidingPlayer, Flying }
public enum FSM_IdleStates { Inactive, LookingAtPlayer, RandomWalk, Gliding, Jumping }


public class FatherActions : AgentFather, IPlatformMovable {

	public FatherSongInteractionsCtrl songInteractionCollider;
	protected HeightState currentState;
	public PlayerSongs currentSong;

	[HideInInspector]
	public float flySeconds, jumpHeight, timeToJumpApex, sustainDuration;
	[HideInInspector]
	public bool allowFlySlowFall;

	[HideInInspector]
	public bool stopUpdate;

	[HideInInspector]
	public bool isCarregadoPorKiwis;

	float defaultStoppingDist;
	//float defaultNMSpeed;

	float default_HugStartTimer = 2f;
	float hugStart_Timer;

	bool sonCalledMe = false;
	public bool canStartCasualHug = false;
	public bool automaticHug = false;
	public bool neverHug = false;

	protected override void Start (){
		base.Start ();

		currentState = HeightState.Default;
		currentSong = PlayerSongs.Empty;

		defaultStoppingDist = nmAgent.stoppingDistance;
		//defaultNMSpeed = nmAgent.speed;

		hugStart_Timer = default_HugStartTimer;
	}

	protected override void Update (){
		if (stopUpdate) {
//			if (isFalling) {
//				base.Update ();
//				nmAgent.speed = 6f;
//			}
			return;
		}

		base.Update ();

		if(isJumping){
			JumpAndFall (jumpHeight, timeToJumpApex);
		} else if (isFlying) {
			JumpAndHold (flySeconds, allowFlySlowFall, jumpHeight, timeToJumpApex);
		}

		if(isSustainingNote){
			Sing_SustainedNote (sustainDuration);
		}
//		else {
//			sustainColl.SetActive (false);
//		}

		UpdateHeightCollider (currentState);

		//if(!isCarregadoPorKiwis)
			songInteractionCollider.currentSong = currentSong;
//		else
//			songInteractionCollider.currentSong = PlayerSongs.Alegria;

		currentStamina = maxStamina - timeMoving;

		if (currentDisposition != FatherConditions.MuitoMachucado) {
			if (playerCtrl.callingFather && !neverHug) {
				playerCtrl.callingFather = false;
				sonCalledMe = true;
				StartHugSon ();
			}

			if (canStartCasualHug) {
				if (playerCtrl.walkStates.TOCANDO_NOTAS) {
					canStartCasualHug = false;
					Invoke ("StartHugSon", 0.1f);
				}
			}

			if (goingToHugSon) {
				if (!hugging) {
					nmAgent.stoppingDistance = 0.1f;
					MoveToPlayer ();
					if (CheckArrivedOnDestination ()) {
						BecomePlayersChild ();
					}
				} else { //Se ja está abraçando...
					isWalking = playerCtrl.walkStates.IS_WALKING;
					LookAtPlayer (true);
					if (playerCtrl.CheckStopHug ()) {
						StopHug ();
					}
				}
			} else {
				nmAgent.stoppingDistance = defaultStoppingDist;
			}
		}

		animCtrl.SetBool ("hugging", hugging);
	}

	#region --------------------------------- TRIGGERS ---------------------------------
	public bool stopHoldFly;
	bool stopSustainNote;
	bool stopRepeatingNote;

	#endregion


	public void ClearActions (){
		isWalking = isRandomWalking = isGuiding = isFollowingPlayer = false;
		stopHoldFly = stopRepeatingNote = stopSustainNote = false;
		nmAgent.isStopped = true;
		//sustainColl.SetActive (false);
		//staccatoColl.SetActive (false);
		currentSong = PlayerSongs.Empty;
		Invoke ("ClearAgain", 0.1f);
	}

	void ClearAgain (){
		isWalking = isRandomWalking = isGuiding = isFollowingPlayer = false;
		stopHoldFly = stopRepeatingNote = stopSustainNote = false;
		nmAgent.isStopped = true;
	}


	#region ---------------------------------- ACTIONS ----------------------------------
	public void Stay (){
		isWalking = false;
	}

	//--- Esta função deve rodar todo frame ---
	public void LookAtPlayer (bool isHugging = false){
		Vector3 playerPos = (!isHugging) ? player.position : (player.position + player.forward * 2f);
		targetReference.position = playerPos;
		agentTransform.rotation = Quaternion.Slerp(agentTransform.rotation, Quaternion.LookRotation((playerPos - agentTransform.position), Vector3.up), Time.deltaTime * angularSpeed);
		agentTransform.eulerAngles = new Vector3(0, agentTransform.eulerAngles.y, 0);
	}

	//Util para se movimentar no chao
	public void MoveHere (Vector3 pos){
		MoveAgentOnNavMesh (pos);
		timeMoving += Time.deltaTime;
	}

	//Util quando estiver no ar
	public void MoveHereWithRB (Vector3 pos){
		MoveAgentWithRB (pos);
		timeMoving += Time.deltaTime;
	}

	public void MoveToPlayer (){
		MoveAgentOnNavMesh (player.position);
		timeMoving += Time.deltaTime;
	}

	//--- Esta função deve rodar todo frame ---
	//SE <player> estiver DENTRO do raio <startDistance>, COMECE a Andar até <pos>
	//SE <player> estiver FORA do raio <stopDistance>, PARE de andar
	public void GuidePlayerTo (Vector3 pos, float startDistance = 16f, float stopDistance = 22f){
		currentTargetPos = pos;
		if (distToPlayer <= startDistance){
			//Guide
			isGuiding = true;
		} else if (distToPlayer >= stopDistance){
			//Wait
			isGuiding = false;
		}

		if (isGuiding /*|| nmAgent.isOnOffMeshLink*/) {
			nmAgent.isStopped = false;
			MoveAgentOnNavMesh(pos);
			timeMoving += Time.deltaTime;
		} else {
			nmAgent.isStopped = true;
		}
	}

	//--- Esta função deve rodar todo frame ---
	//SE <player> estiver FORA do raio <startDistance>, ANDE até o <player>
	//SE <player> estiver DENTRO do raio <stopDistance>, PARE de andar
	public void FollowPlayer (float startDistance = 8f, float stopDistance = 5f){
		if (distToPlayer >= startDistance){
			//Follow
			isFollowingPlayer = true;
			if(!sonCalledMe)
				goingToHugSon = false;
		} else if (distToPlayer <= stopDistance) {
			//Wait
			isFollowingPlayer = false;
			if(sonCalledMe)
				goingToHugSon = true;
		}

		if (!goingToHugSon) {
			if (isFollowingPlayer) {
				nmAgent.isStopped = false;
				MoveToPlayer ();
				timeMoving += Time.deltaTime;
				canStartCasualHug = false;
				if(automaticHug)
					CancelInvoke ("StartHugSon");
			} else {
				nmAgent.isStopped = true;
				canStartCasualHug = true;
				if(automaticHug)
					Invoke ("StartHugSon", hugStart_Timer);
			}
		}
	}


	bool goingToHugSon = false;
	public void StartHugSon (){
		if (neverHug)
			return;
		
		CancelInvoke ("ResetHugStartTimer");
		CancelInvoke ("StartHugSon");
		canStartCasualHug = false;
		hugStart_Timer = default_HugStartTimer;

		goingToHugSon = true;
		nmAgent.isStopped = false;
	}
	public void BecomePlayersChild (){
		if (neverHug)
			return;
		
		nmAgent.enabled = false;
		isWalking = true;
		agentTransform.SetParent (player);
		agentTransform.localPosition = -Vector3.up * 0.5f;
		playerCtrl.StartHug ();
		hugging = true;
		sonCalledMe = false;
	}
	public void StopHug (bool enableNavMesh = true){
		if (enableNavMesh) {
			nmAgent.enabled = true;
		}
		playerCtrl.beinghugged = false;
		agentTransform.SetParent (null);
		hugging = false;
		goingToHugSon = false;
		hugStart_Timer = 6f;
		Invoke ("ResetHugStartTimer", hugStart_Timer);
	}
	void ResetHugStartTimer (){
		hugStart_Timer = default_HugStartTimer;
	}


	public void StartRandomWalk(Vector3 areaCenter, float areaRadius){
		timeMoving += Time.deltaTime;

		if(isRandomWalking){
			if (CheckArrivedOnDestination ()) {
				Vector3 newPos = RandomDestination (areaCenter, areaRadius);
				MoveHere (newPos);
			}
		} else {
			isRandomWalking = true;
			currentTargetPos = RandomDestination (areaCenter, areaRadius);
			MoveHere (currentTargetPos);
		}
	}

	Vector3 RandomDestination (Vector3 areaCenter, float areaRadius){
		Vector2 circleRand = new Vector2 (areaCenter.x, areaCenter.z) + (areaRadius * Random.insideUnitCircle);
		Vector3 dest = new Vector3 (circleRand.x, areaCenter.y, circleRand.y);

		//GameObject destinationSphere = GameObject.CreatePrimitive (PrimitiveType.Sphere) as GameObject;
		//destinationSphere.transform.position = dest;
		//Destroy (destinationSphere, 2f);

		return dest;
	}

	//--- Após ser chamada uma vez, enquanto isJumping for true, esta função deve rodar todo frame ---
	public void JumpAndFall (float jHeight = 5f, float timeToApex = 0.3f){
		timeMoving += Time.deltaTime;

		if (!isJumping) {
			CalculateJump (out isJumping, jHeight, timeToApex);
		} else if (agentTransform.position.y <= oldPosY){
			isJumping = false;
			//jump = false;
			nmAgent.enabled = true;
			rb.isKinematic = true;
		} else {
			rb.velocity += Vector3.up * gravity * Time.deltaTime;
		}
	}

	//--- Após ser chamada uma vez, enquanto isFlying for true, esta função deve rodar todo frame ---
	public void JumpAndHold (float seconds = 0f, bool allowSlowFalling = false, float jHeight = 5f, float timeToApex = 0.3f){
		timeMoving += Time.deltaTime;

		//Se eu acabei de começar o pulo
		if (!isFlying) {
			counter_Fly = 0f;
			CalculateJump (out isFlying, jHeight, timeToApex);
		} 
		//Se eu estiver subindo
		else if (Mathf.Sign (rb.velocity.y) > 0){
			rb.velocity += Vector3.up * gravity * Time.deltaTime;
		} 
		//Se eu não estiver mais subindo
		else {
			//Se seconds for igual a 0, ele fica no ar até o trigger ser ativado externamente
			//Caso contrario, ele começa a descer quando counter chegar em seconds
			if (seconds > 0f) {
				counter_Fly += Time.deltaTime;
			}

			if(counter_Fly >= seconds || stopHoldFly){
				rb.velocity += Vector3.up * gravity * Time.deltaTime;
			} 
			else if(allowSlowFalling){
				rb.velocity = new Vector3 (rb.velocity.x, gravity * Time.deltaTime, rb.velocity.z);
			}

			//Se eu voltei para a altura original, retoma o funcionamento normal
			if (agentTransform.position.y <= oldPosY){
				isFlying = false;
				//fly = false;
				stopHoldFly = false;
				nmAgent.enabled = true;
				rb.isKinematic = true;
			}
		}
	}

	public void ChangeHeight (HeightState height, float seconds = 0f){
		switch (height) {
		case HeightState.High:
			//TODO: Delete this. Apenas para teste
			//clarinetHigh.TransitionTo (0.01f);
			//endTODO

			animCtrl.SetFloat ("Height", 1f);
			break;
		case HeightState.Default:
			//TODO: Delete this. Apenas para teste
			//clarinetDefault.TransitionTo (0.01f);
			//endTODO

			animCtrl.SetFloat ("Height", 0f);
			break;
		case HeightState.Low:
			//TODO: Delete this. Apenas para teste
			//clarinetLow.TransitionTo (0.01f);
			//endTODO

			animCtrl.SetFloat ("Height", -1f);
			break;
		default:
			break;
		}

		currentState = height;

		if (seconds > 0){
			StartCoroutine ("ReturnToDefaultHeight", seconds);
		} else {
			StopCoroutine ("ReturnToDefaultHeight");
		}
	}

	IEnumerator ReturnToDefaultHeight(float seconds){
		yield return new WaitForSeconds (seconds);

		currentState = HeightState.Default;

		//TODO: Delete this. Apenas para teste
		//clarinetDefault.TransitionTo (0.01f);
		//end TODO

		animCtrl.SetFloat ("Height", 0f);
	}

	IEnumerator ResetSingingSomething (){
		//if(!isCarregadoPorKiwis)
			songInteractionCollider.isSingingSomething = true;
		yield return new WaitForSeconds (0.2f);
		//if(!isCarregadoPorKiwis)
			songInteractionCollider.isSingingSomething = false;
	}
		
	public void Sing_SingleNote (bool repeatNote = false, int index = 0){
		if (!repeatNote) {
			sing.clip = staccatoSounds [(int)currentState].clip [noteIndexes [(int)currentState]];
			noteIndexes [(int)currentState]++;
			if (noteIndexes [(int)currentState] > 3 || noteIndexes [(int)currentState] < 0) {
				noteIndexes [(int)currentState] = 0;
			}
		} else {
			sing.clip = staccatoSounds [(int)currentState].clip [index];
		}
		sing.Play ();
		balaoNotasCtrl.Show_BalaoNotas (currentState);
		//staccatoColl.SetActive (true);
		//if(!isCarregadoPorKiwis)
			songInteractionCollider.isSingingSomething = true;

		CancelInvoke("ResetNoteIndex");
		Invoke ("ResetNoteIndex", 2f);

		CancelInvoke("HideStaccatoColl");
		Invoke ("HideStaccatoColl", 0.4f);

		animCtrl.SetTrigger ("sing");
	}

	void HideStaccatoColl (){
		//staccatoColl.SetActive (false);
		currentSong = PlayerSongs.Empty;
		//if(!isCarregadoPorKiwis)
			songInteractionCollider.isSingingSomething = false;
	}

	void ResetNoteIndex (){
		for (int i = 0; i < noteIndexes.Length; i++) {
			noteIndexes [i] = 0;
		}
	}

	//--- Após ser chamada uma vez, enquanto isRepeatingNote for true, esta função roda ~automaticamente~ uma vez a cada <singleNoteMinimumDuration> segundos ---
	public void Sing_SingleNoteRepeat (int times = 0){
		isRepeatingNote = true;
		StopCoroutine ("RepeatNote");

		if (times > 0){ //Se foi especificada a quantidade de vezes que deve tocar...
			if (counter_SingRepeat < times){ //Se ainda não tocou <times> vezes,
				counter_SingRepeat++; //aumenta o counter,
				StartCoroutine ("RepeatNote", times); //toca a nota e roda esta função novamente.
			} 
			else { //Se ja tocou <times> vezes, reseta o counter e não toca mais.
				counter_SingRepeat = 0;
				isRepeatingNote = false;
			}
		} 
		else if (!stopRepeatingNote) { //Caso contrario, enquanto o <trigger> for falso...
			StartCoroutine ("RepeatNote", times); //toca a nota e roda esta função novamente.
		} else { //Caso o <trigger> seja ativado...
			stopRepeatingNote = false;
			counter_SingRepeat = 0;
			isRepeatingNote = false;
		}
	}
	IEnumerator RepeatNote (int times){
		Sing_SingleNote (true);

		yield return new WaitForSeconds (singleNoteMinimumDuration);
		Sing_SingleNoteRepeat (times);
	}

	//--- Após ser chamada uma vez, enquanto isSustainingNote for true, esta função deve rodar todo frame ---
	public void Sing_SustainedNote (float duration = 0f){
		if(!isSustainingNote){ //Se ainda nao começou a cantar, cante.
			isSustainingNote = true;
			singSustain.Play ();
			counter_SingSustain = 0f; //Inicia o counter.
			sustainDuration = duration;
			//sustainColl.SetActive (true);
			//if(!isCarregadoPorKiwis)
				songInteractionCollider.isSingingSomething = true;
		}

		if(duration > 0f){ //Se foi definido um tempo limite...
			if(counter_SingSustain >= duration){ //Se já cantou por <duration>s, pare.
				isSustainingNote = false;
				//singSustainNote = false;
				counter_SingSustain = 0f;
				singSustain.Stop ();
				//sustainColl.SetActive (false);
				currentSong = PlayerSongs.Empty;
				//if(!isCarregadoPorKiwis)
					songInteractionCollider.isSingingSomething = false;
			}
			else { //Se ainda não alcançou <duration>s, aumente o counter.
				counter_SingSustain += Time.deltaTime;
			}
		}
		else { //Se NÃO foi definido um tempo limite...
			if(stopSustainNote){
				isSustainingNote = false;
				stopSustainNote = false;
				//singSustainNote = false;
				counter_SingSustain = 0f;
				singSustain.Stop ();
				//sustainColl.SetActive (false);
				currentSong = PlayerSongs.Empty;
				//if(!isCarregadoPorKiwis)
					songInteractionCollider.isSingingSomething = false;
			}
		}
	}

	public void Sing_Partitura (PartituraInfo[] partitura){
		StopCoroutine("PlayPartitura");
		StartCoroutine ("PlayPartitura", partitura);
	}
	IEnumerator PlayPartitura (PartituraInfo[] partitura){
		for (int i = 0; i < partitura.Length; i++) {
			ChangeHeight (partitura [i].Height);
			if(partitura[i].Sustain){
				Sing_SustainedNote (partitura [i].SustainTime);
				yield return new WaitForSeconds (partitura [i].WaitTimeBeforeNext + partitura [i].SustainTime);
			} else {
				Sing_SingleNote ();
				yield return new WaitForSeconds (partitura [i].WaitTimeBeforeNext);
			}
		}
		ChangeHeight (HeightState.Default);
	}

	IEnumerator BroadcastSong (PlayerSongs song){
		yield return new WaitForSeconds (3f);
		currentSong = song;
	}

	public void TocarMusicaSimples (FatherSongSimple song) {
		sing.Stop ();
		singSustain.Stop ();

		switch (song) {
		case FatherSongSimple.Alegria: //DISTRAIR
			PartituraInfo[] alegria = new PartituraInfo[] {
				new PartituraInfo (HeightState.Default),
				new PartituraInfo (HeightState.Default),
				new PartituraInfo (HeightState.High),
				new PartituraInfo (HeightState.Default),
			};
			Sing_Partitura (alegria);
			StartCoroutine ("BroadcastSong", PlayerSongs.Alegria);
			break;
		case FatherSongSimple.Estorvo: //IRRITAR
			ChangeHeight (HeightState.High, singleNoteMinimumDuration * 6f);
			Sing_SingleNoteRepeat (4);
			StartCoroutine ("BroadcastSong", PlayerSongs.Estorvo);
			break;
		case FatherSongSimple.Serenidade: //ACALMAR
			PartituraInfo[] serenidade = new PartituraInfo[] {
				new PartituraInfo(HeightState.Low),
				new PartituraInfo(HeightState.Default),
				new PartituraInfo(HeightState.Default),
				new PartituraInfo(HeightState.Low),
			};
			Sing_Partitura (serenidade);
			StartCoroutine ("BroadcastSong", PlayerSongs.Serenidade);
			break;
		case FatherSongSimple.Ninar: //DORMIR
			PartituraInfo[] ninar = new PartituraInfo[] {
				new PartituraInfo(HeightState.Default),
				new PartituraInfo(HeightState.Default),
				new PartituraInfo(HeightState.Low),
				new PartituraInfo(HeightState.Low),
			};
			Sing_Partitura (ninar);
			StartCoroutine ("BroadcastSong", PlayerSongs.Ninar);
			break;
		default:
			Debug.LogWarning("BUG: O pai não conhece essa musica simples");
			break;
		}
	}

	public void TocarMusicaComSustain (FatherSongSustain song, float duration = 0f) {
		sing.Stop ();
		singSustain.Stop ();

		switch (song) {
		case FatherSongSustain.Amizade: //SEGUIR
//			ChangeHeight (HeightState.Default);
//			Sing_SustainedNote (duration);
			PartituraInfo[] amizade = new PartituraInfo[] {
				new PartituraInfo(HeightState.Low),
				new PartituraInfo(HeightState.Default),
				new PartituraInfo(HeightState.Low),
				new PartituraInfo(HeightState.Default),
			};
			Sing_Partitura (amizade);
			StartCoroutine ("BroadcastSong", PlayerSongs.Amizade);
			break;
		case FatherSongSustain.Crescimento: //CRESCER
//			ChangeHeight (HeightState.High);
//			Sing_SustainedNote (duration);
			PartituraInfo[] crescimento = new PartituraInfo[] {
				new PartituraInfo(HeightState.High),
				new PartituraInfo(HeightState.High),
				new PartituraInfo(HeightState.High),
				new PartituraInfo(HeightState.High),
			};
			Sing_Partitura (crescimento);
			StartCoroutine ("BroadcastSong", PlayerSongs.Crescimento);
			break;
		case FatherSongSustain.Encolhimento: //ENCOLHER
//			ChangeHeight (HeightState.Low);
//			Sing_SustainedNote (duration);
			PartituraInfo[] encolhimento = new PartituraInfo[] {
				new PartituraInfo(HeightState.Low),
				new PartituraInfo(HeightState.Low),
				new PartituraInfo(HeightState.Low),
				new PartituraInfo(HeightState.Low),
			};
			Sing_Partitura (encolhimento);
			StartCoroutine ("BroadcastSong", PlayerSongs.Encolhimento);
			break;
		default:
			Debug.LogWarning("BUG: O pai não conhece essa musica com sustain");
			break;
		}
	}

	#endregion

//	bool currentNavMeshState;
//	bool nmWasActive;

	public void OnMovingPlat (bool enableNavMesh, Transform plat){
		if (isCarregadoPorKiwis || hugging) {
			return;
		}

//		if(nmAgent.isActiveAndEnabled){
//			nmWasActive = true;
//			currentNavMeshState = true;
//		} else {
//			currentNavMeshState = false;
//		}
//
//		if(enableNavMesh){
//			if (nmWasActive && !currentNavMeshState)
//				nmAgent.enabled = true;
//		} else {
//			nmAgent.enabled = false;
//		}

		nmAgent.enabled = enableNavMesh;
		agentTransform.parent = plat;
		//rb.isKinematic = enableNavMesh;
		stopUpdate = !enableNavMesh;
	}

	public void IsFalling (bool enableNavMesh, bool fallWithPlat = false, Transform plat = null){
		nmAgent.enabled = enableNavMesh;
		//rb.isKinematic = enableNavMesh;
		//rb.useGravity = !enableNavMesh;
		stopUpdate = !enableNavMesh;
		ClearActions ();

		if(!enableNavMesh && fallWithPlat){
			agentTransform.parent = plat;
		} else {
			agentTransform.parent = null;
		}
	}

	public void RemoteForceAnimBool (string anim, bool moving){
		animCtrl.SetBool (anim, moving);
	}

//	public bool isFalling = false;
//	public void IsFalling (bool enableNavMesh, bool moveWithRB, Vector3 dest){
//		isFalling = !enableNavMesh;
//
//		nmAgent.enabled = enableNavMesh;
//		rb.isKinematic = enableNavMesh;
//		rb.useGravity = !enableNavMesh;
//		rb.AddForce (Vector3.down);
//		stopUpdate = !enableNavMesh;
//
//		if(!enableNavMesh && moveWithRB){
//			StartCoroutine (MoveWhileFalling (dest));
//		}
//	}
//
//	IEnumerator MoveWhileFalling (Vector3 dest){
//		while (isFalling) {
//			if (!CheckArrivedOnDestination (true)) {
//				agentTransform.rotation = Quaternion.Slerp (agentTransform.rotation, Quaternion.LookRotation ((dest - agentTransform.position), Vector3.up), Time.deltaTime * angularSpeed);
//				agentTransform.eulerAngles = new Vector3 (0, agentTransform.eulerAngles.y, 0);
//				rb.MovePosition (agentTransform.position + agentTransform.forward * Time.deltaTime * 6f);
//				print ("indo");
//				yield return new WaitForSeconds (Time.deltaTime);
//			}
//		}
//	}
}


[System.Serializable]
public class PartituraInfo
{
	public HeightState Height;
	public bool Sustain;

	public float WaitTimeBeforeNext;
	public float SustainTime;

	public PartituraInfo (){
		Height = HeightState.Default;
		Sustain = false;
		WaitTimeBeforeNext = 1f;
		SustainTime = 2f;
	}

	public PartituraInfo (HeightState height, bool sustain = false, float waitTimeBeforeNext = 1f, float sustainTime = 2f){
		Height = height;
		Sustain = sustain;
		WaitTimeBeforeNext = waitTimeBeforeNext;
		SustainTime = sustainTime;
	}
}