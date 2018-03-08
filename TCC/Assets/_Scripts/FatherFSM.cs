using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FatherFSM : MonoBehaviour {

	public enum FatherConditions { Disposto, Irritado, Cansado, Machucado, MuitoMachucado }
	public enum FSM_States { Idle, Walking }
	public enum FSM_WalkStates { SimpleWalk, FollowingPlayer, GuidingPlayer, Flying }
	public enum FSM_IdleStates { Inactive, LookingAtPlayer, RandomWalk, Gliding, Jumping }

	private FatherActions fatherActions;

	void Start () {
		fatherActions = GetComponent<FatherActions> ();
	}

	void Update () {
		
	}


	public void StartWalkState (){
		
	}

	public void StartIdleState (){
		
	}

}
