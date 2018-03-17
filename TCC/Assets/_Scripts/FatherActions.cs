using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FSM_WalkStates { SimpleWalk, FollowingPlayer, GuidingPlayer, Flying }
public enum FSM_IdleStates { Inactive, LookingAtPlayer, RandomWalk, Gliding, Jumping }


public class FatherActions : AgentFather {

	protected HeightState currentState;
	protected PlayerSongs currentSong;

	[HideInInspector]
	public float flySeconds, jumpHeight, timeToJumpApex, sustainDuration;
	[HideInInspector]
	public bool allowFlySlowFall;

	protected override void Start (){
		base.Start ();

		currentState = HeightState.Default;
		currentSong = PlayerSongs.Empty;
	}

	#region ========== Debug Variables ==========
	public bool jump;
	public bool fly;
	public bool randomWalk;
	public bool moveToPlayer;
	public bool guidePlayer;
	public bool followPlayer;
	public bool esticado, alturaDefault, abaixado;
	public bool singSingle, singSingleRepeat, singSustainNote, singPartitura;
	public bool singEstorvo;

	bool isMovingNavMesh;
	bool isMovingRB;
	#endregion

	protected override void Update (){
		base.Update ();

//		#region ========== Temporary Code ==========
//		if (Input.GetMouseButton(0)) {
//			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//			RaycastHit hit;
//			if (Physics.Raycast(ray, out hit, 200)) {
//				isMovingNavMesh = true;
//				isMovingRB = false;
//				currentTargetPos = hit.point;
//			}
//		}
//		if (Input.GetMouseButton(1)) {
//			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//			RaycastHit hit;
//			if (Physics.Raycast(ray, out hit, 200)) {
//				isMovingRB = true;
//				isMovingNavMesh = false;
//				currentTargetPos = hit.point;
//			}
//		}
//
//		if (moveToPlayer || followPlayer){
//			isMovingNavMesh = false;
//			isMovingRB = false;
//		}
//
//		if(isMovingNavMesh && !(jump || fly)){
//			if(!guidePlayer)
//				MoveHere (currentTargetPos);
//			else
//				GuidePlayerTo (currentTargetPos, 10f, 20f);
//		} 
//		else if (isMovingRB) {
//			MoveHereWithRB (currentTargetPos);
//			isMovingRB = !CheckArrivedOnDestination (true);
//		}
//
//		if(randomWalk){
//			if(CheckArrivedOnDestination()){
//				currentTargetPos = RandomDestination (player.position, 15f);
//				MoveHere (currentTargetPos);
//			}
//		}
//
//		if(moveToPlayer){
//			MoveToPlayer ();
//			moveToPlayer = !CheckArrivedOnDestination ();
//		}
//
//		if(followPlayer){
//			FollowPlayer(12f, 6f);
//		}
//
//		if(jump){
//			JumpAndFall ();
//		}
//		if (isJumping)
//			jump = true;
//
//		if(fly){
//			JumpAndHold (5f, true, 10, 0.8f);
//		}
//		if (isFlying)
//			fly = true;
//
//		if(esticado){
//			esticado = alturaDefault = abaixado = false;
//			ChangeHeight(HeightState.High, 3f);
//		} else if (alturaDefault) {
//			esticado = alturaDefault = abaixado = false;
//			ChangeHeight(HeightState.Default);
//		} else if (abaixado) {
//			esticado = alturaDefault = abaixado = false;
//			ChangeHeight(HeightState.Low);
//		}
//
//		if(singSingle){
//			singSingle = false;
//			Sing_SingleNote ();
//		}
//
//		if (singSingleRepeat){
//			singSingleRepeat = false;
//			Sing_SingleNoteRepeat (3);
//		}
//		if(isRepeatingNote){
//			singSingleRepeat = false;
//		}
//
//		if (singSustainNote){
//			Sing_SustainedNote (5f);
//		}
//		if(isSustainingNote){
//			singSustainNote = true;
//		}
//
//		if(singPartitura){
//			singPartitura = false;
//			PartituraInfo[] partituraTeste = new PartituraInfo[]
//			{
//				new PartituraInfo(HeightState.High, true, 0.5f, 2f),
//				new PartituraInfo(HeightState.High),
//				new PartituraInfo(HeightState.Low, false, 1.5f),
//				new PartituraInfo(HeightState.Default, true, 2f, 1f),
//				new PartituraInfo(HeightState.Low, false)
//			};
//			Sing_Partitura(partituraTeste);
//		}
//
//		if(singEstorvo){
//			singEstorvo = false;
//			TocarMusicaSimples(FatherSongSimple.Estorvo);
//		}
//
//		#endregion

		if(isJumping){
			JumpAndFall (jumpHeight, timeToJumpApex);
		} else if (isFlying) {
			JumpAndHold (flySeconds, allowFlySlowFall, jumpHeight, timeToJumpApex);
		}

		if(isSustainingNote){
			Sing_SustainedNote (sustainDuration);
		}


		UpdateHeightCollider (currentState);
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
	}


	#region ---------------------------------- ACTIONS ----------------------------------
	public void Stay (){
		isWalking = false;
	}

	//--- Esta função deve rodar todo frame ---
	public void LookAtPlayer (){
		agentTransform.rotation = Quaternion.Slerp(agentTransform.rotation, Quaternion.LookRotation((player.position - agentTransform.position), Vector3.up), Time.deltaTime * angularSpeed);
		agentTransform.eulerAngles = new Vector3(0, agentTransform.eulerAngles.y, 0);
	}

	//Util para se movimentar no chao
	public void MoveHere (Vector3 pos){
		MoveAgentOnNavMesh (pos);
	}

	//Util quando estiver no ar
	public void MoveHereWithRB (Vector3 pos){
		MoveAgentWithRB (pos);
	}

	public void MoveToPlayer (){
		MoveAgentOnNavMesh (player.position);
	}

	//--- Esta função deve rodar todo frame ---
	//SE <player> estiver DENTRO do raio <startDistance>, COMECE a Andar até <pos>
	//SE <player> estiver FORA do raio <stopDistance>, PARE de andar
	public void GuidePlayerTo (Vector3 pos, float startDistance = 6f, float stopDistance = 15f){
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
		} else {
			nmAgent.isStopped = true;
		}
	}

	//--- Esta função deve rodar todo frame ---
	//SE <player> estiver FORA do raio <startDistance>, ANDE até o <player>
	//SE <player> estiver DENTRO do raio <stopDistance>, PARE de andar
	public void FollowPlayer (float startDistance = 12f, float stopDistance = 6f){
		if (distToPlayer >= startDistance){
			//Follow
			isFollowingPlayer = true;
		} else if (distToPlayer <= stopDistance) {
			//Wait
			isFollowingPlayer = false;
		}

		if(isFollowingPlayer){
			nmAgent.isStopped = false;
			MoveToPlayer ();
		} else {
			nmAgent.isStopped = true;
		}
	}

	public void StartRandomWalk(Vector3 areaCenter, float areaRadius){
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
		if (!isJumping) {
			CalculateJump (out isJumping, jHeight, timeToApex);
		} else if (agentTransform.position.y <= oldPosY){
			isJumping = false;
			jump = false;
			nmAgent.enabled = true;
			rb.isKinematic = true;
		} else {
			rb.velocity += Vector3.up * gravity * Time.deltaTime;
		}
	}

	//--- Após ser chamada uma vez, enquanto isFlying for true, esta função deve rodar todo frame ---
	public void JumpAndHold (float seconds = 0f, bool allowSlowFalling = false, float jHeight = 5f, float timeToApex = 0.3f){
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
				fly = false;
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
			clarinetHigh.TransitionTo (0.01f);
			//endTODO

			animCtrl.SetFloat ("Height", 0.9f);
			break;
		case HeightState.Default:
			//TODO: Delete this. Apenas para teste
			clarinetDefault.TransitionTo (0.01f);
			//endTODO

			animCtrl.SetFloat ("Height", 0f);
			break;
		case HeightState.Low:
			//TODO: Delete this. Apenas para teste
			clarinetLow.TransitionTo (0.01f);
			//endTODO

			animCtrl.SetFloat ("Height", -0.9f);
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
		clarinetDefault.TransitionTo (0.01f);
		//endTODO

		animCtrl.SetFloat ("Height", 0f);
	}

	public void Sing_SingleNote (){
		sing.Play ();
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
		Sing_SingleNote ();

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
		}

		if(duration > 0f){ //Se foi definido um tempo limite...
			if(counter_SingSustain >= duration){ //Se já cantou por <duration>s, pare.
				isSustainingNote = false;
				singSustainNote = false;
				counter_SingSustain = 0f;
				singSustain.Stop ();
			}
			else { //Se ainda não alcançou <duration>s, aumente o counter.
				counter_SingSustain += Time.deltaTime;
			}
		}
		else { //Se NÃO foi definido um tempo limite...
			if(stopSustainNote){
				isSustainingNote = false;
				stopSustainNote = false;
				singSustainNote = false;
				counter_SingSustain = 0f;
				singSustain.Stop ();
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

	public void TocarMusicaSimples (FatherSongSimple song) {
		sing.Stop ();
		singSustain.Stop ();

		switch (song) {
		case FatherSongSimple.Alegria: //DISTRAIR
			//TODO: Definir melodia especifica
			break;
		case FatherSongSimple.Estorvo: //IRRITAR
			ChangeHeight (HeightState.High, singleNoteMinimumDuration * 6f);
			Sing_SingleNoteRepeat (5);
			break;
		case FatherSongSimple.Serenidade: //ACALMAR
			//TODO: Definir melodia especifica
			break;
		case FatherSongSimple.Ninar: //DORMIR
			//TODO: Definir melodia especifica
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
			ChangeHeight (HeightState.Default);
			Sing_SustainedNote (duration);
			break;
		case FatherSongSustain.Crescimento: //CRESCER
			ChangeHeight (HeightState.High);
			Sing_SustainedNote (duration);
			break;
		case FatherSongSustain.Encolhimento: //ENCOLHER
			ChangeHeight (HeightState.Low);
			Sing_SustainedNote (duration);
			break;
		default:
			Debug.LogWarning("BUG: O pai não conhece essa musica com sustain");
			break;
		}
	}

	#endregion
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