using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FatherActions : AgentFather {

	protected HeightState currentState;

	protected override void Start (){
		base.Start ();
	}

	#region ========== Debug Variables ==========
	public bool jump;
	public bool fly;
	public bool randomWalk;
	public bool moveToPlayer;
	public bool guidePlayer;
	public bool followPlayer;
	public bool esticado, alturaDefault, abaixado;

	bool isMovingNavMesh;
	bool isMovingRB;
	#endregion



	protected override void Update (){
		base.Update ();

		#region ========== Temporary Code ==========
		if (Input.GetMouseButton(0)) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, 200)) {
				isMovingNavMesh = true;
				isMovingRB = false;
				currentTargetPos = hit.point;
			}
		}
		if (Input.GetMouseButton(1)) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, 200)) {
				isMovingRB = true;
				isMovingNavMesh = false;
				currentTargetPos = hit.point;
			}
		}

		if (moveToPlayer || followPlayer){
			isMovingNavMesh = false;
			isMovingRB = false;
		}

		if(isMovingNavMesh && !(jump || fly)){
			if(!guidePlayer)
				MoveHere (currentTargetPos);
			else
				GuidePlayerTo (currentTargetPos, 10f, 20f);
		} 
		else if (isMovingRB) {
			MoveHereWithRB (currentTargetPos);
			isMovingRB = !CheckArrivedOnDestination (true);
		}

		if(randomWalk){
			if(CheckArrivedOnDestination()){
				currentTargetPos = RandomDestination (player.position, 15f);
				MoveHere (currentTargetPos);
			}
		}

		if(moveToPlayer){
			MoveToPlayer ();
			moveToPlayer = !CheckArrivedOnDestination ();
		}

		if(followPlayer){
			FollowPlayer(12f, 6f);
		}

		if(jump){
			JumpAndFall ();
		}
		if (isJumping)
			jump = true;

		if(fly){
			JumpAndHold (5f, true, 10, 0.8f);
		}
		if (isFlying)
			fly = true;

		if(esticado){
			esticado = alturaDefault = abaixado = false;
			ChangeHeight(HeightState.High, 3f);
		} else if (alturaDefault) {
			esticado = alturaDefault = abaixado = false;
			ChangeHeight(HeightState.Default);
		} else if (abaixado) {
			esticado = alturaDefault = abaixado = false;
			ChangeHeight(HeightState.Low);
		}
		#endregion
	}

	#region --------------------------------- TRIGGERS ---------------------------------
	bool stopHoldFly;

	#endregion


	#region ---------------------------------- ACTIONS ----------------------------------
	void Stay (){

	}

	//Util para se movimentar no chao
	void MoveHere (Vector3 pos){
		MoveAgentOnNavMesh (pos);
	}

	//Util quando estiver no ar
	void MoveHereWithRB (Vector3 pos){
		MoveAgentWithRB (pos);
	}

	void MoveToPlayer (){
		MoveAgentOnNavMesh (player.position);
	}

	//SE <player> estiver DENTRO do raio <startDistance>, COMECE a Andar até <pos>
	//SE <player> estiver FORA do raio <stopDistance>, PARE de andar
	void GuidePlayerTo (Vector3 pos, float startDistance, float stopDistance){
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

	//SE <player> estiver FORA do raio <startDistance>, ANDE até o <player>
	//SE <player> estiver DENTRO do raio <stopDistance>, PARE de andar
	void FollowPlayer (float startDistance, float stopDistance){
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

	Vector3 RandomDestination (Vector3 areaCenter, float areaRadius){
		Vector2 circleRand = new Vector2 (areaCenter.x, areaCenter.z) + (areaRadius * Random.insideUnitCircle);
		Vector3 dest = new Vector3 (circleRand.x, areaCenter.y, circleRand.y);

		//GameObject destinationSphere = GameObject.CreatePrimitive (PrimitiveType.Sphere) as GameObject;
		//destinationSphere.transform.position = dest;
		//Destroy (destinationSphere, 2f);

		return dest;
	}

	void JumpAndFall (float jHeight = 5f, float timeToApex = 0.3f){
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

	void JumpAndHold (float seconds = 0f, bool allowSlowFalling = false, float jHeight = 5f, float timeToApex = 0.3f){
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

	void ChangeHeight (HeightState height, float seconds = 0f){
		switch (height) {
		case HeightState.High:
			animCtrl.SetFloat ("Height", 0.9f);
			break;
		case HeightState.Default:
			animCtrl.SetFloat ("Height", 0f);
			break;
		case HeightState.Low:
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
		animCtrl.SetFloat ("Height", 0f);
	}

	void Sing_SingleNote (){

	}

	void Sing_SingleNoteRepeat (int times = 0){

	}

	void Sing_Partitura (int partitura, float intervalBetweenNotes = 1f){

	}
	#endregion
}
