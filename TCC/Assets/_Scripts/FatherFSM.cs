using System.Collections;
using System.Collections.Generic;
using UnityEngine;
						//		0			1				2		3		4			5				6			   7           8
public enum FatherStates { Inactive, LookingAtPlayer, RandomWalk, Gliding, Jumping, SimpleWalk, FollowingPlayer, GuidingPlayer, Flying }
public enum FatherSongType { None, Partitura, MusicaSimples, MusicaComSustain }
public enum StateChangers { Arrived, Timer, DistanceToPlayer, ExternalTrigger, TimerRestart_TriggerAdvance }


public class FatherFSM : MonoBehaviour {

	//public enum FatherConditions { Disposto, Irritado, Cansado, Machucado, MuitoMachucado }

	//[HideInInspector]
	public FatherStates currentState;
	[HideInInspector]
	public RoteiroPai.Roteiro currentStateInfo;
	//[HideInInspector]
	public bool changeState;

	StateChangers currentStateChanger;

	//public Vector3[] destinations;

	private FatherActions fatherActions;

	private bool playerIsRespawning;
	private bool isReturningFromRespawn;

	//Trigger variables
	public bool externalTriggerActivated;
	float checkDistance, checkTimer;

	void Start () {
		fatherActions = GetComponent<FatherActions> ();
	}

	void Update () {
		if (playerIsRespawning) {
			if (isReturningFromRespawn) {
				if(fatherActions.CheckArrivedOnDestination ()){
					isReturningFromRespawn = false;
					Invoke ("EndRespawnBehaviour", 1f);
				}
			}
			return;
		}

		changeState = false;

		switch (currentStateChanger) {
		case StateChangers.Arrived:
			if (currentState != FatherStates.Flying && fatherActions.CheckArrivedOnDestination ()) {
				changeState = true;
			} else if (currentState == FatherStates.Flying && fatherActions.CheckArrivedOnDestination(true)) {
				fatherActions.stopHoldFly = true;
				currentStateChanger = StateChangers.Timer;
				checkTimer = 3f;
			}
			break;
		case StateChangers.DistanceToPlayer:
			if (fatherActions.DistToPlayer () <= checkDistance) {
				changeState = true;
			}
			break;
		case StateChangers.Timer:
			if (checkTimer > 0f){
				checkTimer -= Time.deltaTime;
			} else {
				checkTimer = 0f;
				changeState = true;
			}
			break;
		case StateChangers.ExternalTrigger:
			if(externalTriggerActivated){
				changeState = true;
				externalTriggerActivated = false;
			}
			break;
		case StateChangers.TimerRestart_TriggerAdvance:
			if (checkTimer > 0f){
				checkTimer -= Time.deltaTime;
			} else {
				checkTimer = 0f;
				RoteiroPai.RestartRoteiroAt (8);
				break;
			}

			if(externalTriggerActivated){
				changeState = true;
				externalTriggerActivated = false;
			}
			break;
		default:
			changeState = false;
			break;
		}

		if(changeState){
			fatherActions.ClearActions ();
		}


		switch (currentState) {
		case FatherStates.LookingAtPlayer:
			fatherActions.LookAtPlayer ();
			break;
		case FatherStates.RandomWalk:
			fatherActions.StartRandomWalk (currentStateInfo.areaCenter.position, currentStateInfo.areaRadius);
			break;
		case FatherStates.SimpleWalk:
			fatherActions.MoveHere (currentStateInfo.destination.position);
			break;
		case FatherStates.FollowingPlayer:
			fatherActions.FollowPlayer ();
			break;
		case FatherStates.GuidingPlayer:
			fatherActions.GuidePlayerTo (currentStateInfo.destination.position);
			break;
		case FatherStates.Flying:
			if(!fatherActions.CheckArrivedOnDestination(true)){
				fatherActions.MoveHereWithRB (currentStateInfo.destination.position);
			} else {
				if (!fatherActions.CheckArrivedOnDestination()) {
					fatherActions.MoveHere (currentStateInfo.destination.position);
				}
			}
			break;
		default:
			break;
		}
	}


	public void SetStateChanger (StateChangers trigger = StateChangers.Arrived, float triggerDetail = 5f){

		//Maybe not necessary, but just to be sure...
		if (currentState == FatherStates.Flying)
			trigger = StateChangers.Arrived;

		switch (trigger) {
		case StateChangers.Arrived:
			currentStateChanger = StateChangers.Arrived;
			break;
		case StateChangers.DistanceToPlayer:
			currentStateChanger = StateChangers.DistanceToPlayer;
			checkDistance = triggerDetail;
			break;
		case StateChangers.Timer:
			currentStateChanger = StateChangers.Timer;
			checkTimer = triggerDetail;
			break;
		case StateChangers.ExternalTrigger:
			currentStateChanger = StateChangers.ExternalTrigger;
			externalTriggerActivated = false;
			break;
		case StateChangers.TimerRestart_TriggerAdvance:
			currentStateChanger = StateChangers.TimerRestart_TriggerAdvance;
			externalTriggerActivated = false;
			checkTimer = triggerDetail;
			break;
		default:
			break;
		}
	}

	public void StartInactivity (){
		fatherActions.Stay ();
	}
	public void StartFlyingTowards (Vector3 destination){
		fatherActions.MoveHereWithRB (destination);
	}
	public void StartJump(float jHeight, float timeToApex){
		fatherActions.jumpHeight = jHeight;
		fatherActions.timeToJumpApex = timeToApex;

		fatherActions.JumpAndFall (jHeight, timeToApex);
	}
	public void StartFly(float seconds, bool allowSlowFalling, float jHeight, float timeToApex){
		fatherActions.flySeconds = seconds;
		fatherActions.allowFlySlowFall = allowSlowFalling;
		fatherActions.jumpHeight = jHeight;
		fatherActions.timeToJumpApex = timeToApex;

		fatherActions.JumpAndHold (seconds, allowSlowFalling, jHeight, timeToApex);
	}
	public void StartSimpleSong(FatherSongSimple song){
		fatherActions.TocarMusicaSimples (song);
	}
	public void StartSustainSong(FatherSongSustain song, float duration){
		fatherActions.TocarMusicaComSustain (song, duration);
	}
	public void StartPartitura(PartituraInfo[] partitura){
		fatherActions.Sing_Partitura (partitura);
	}

	public void ChangeHeight(HeightState height){
		fatherActions.ChangeHeight (height);
	}

	//=-----------------------------------------------------------------=

	public void StartRespawn (){
		playerIsRespawning = true;
		fatherActions.stopUpdate = true;
	}

	public void ReturnToPosAfterRespawn (Vector3 pos){
		isReturningFromRespawn = true;
		fatherActions.stopUpdate = false;
		fatherActions.MoveHere (pos);
	}

	void EndRespawnBehaviour (){
		RoteiroPai.RestartRoteiroAt ();
		playerIsRespawning = false;
	}
}