using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RoteiroPai : MonoBehaviour {

	public FatherFSM fatherFSM;

	public List<Roteiro> roteiro;

	public int currentState;

	void Start () {
//		fatherActions.GuidePlayerTo (destinations [0]);
//		fatherActions.MoveHere (destinations [1]);
//		fatherActions.LookAtPlayer ();
//		//trigger
//		fatherActions.MoveHere (destinations [2]);
//		//if(fatherActions.DistToPlayer() < 5f)
//		fatherActions.Sing_Partitura(new PartituraInfo[]{new PartituraInfo(HeightState.High, true, 0, 3f)});
//		fatherActions.MoveHere (destinations [3]);
//		//if(fatherActions.DistToPlayer() < 5f)
//		fatherActions.MoveHere (destinations [4]);
//		fatherActions.GuidePlayerTo (destinations [5]);
//		//cooldown = 5f;
//		//while(!plantaChegouNoTopo)
//		//cooldown -= Time.deltaTime;
//		//if(playerCtrl.walkStates.CURR_SONG == PlayerSongs.Crescimento || cooldown <= 0f)
//		fatherActions.Sing_Partitura(new PartituraInfo[]{new PartituraInfo(HeightState.High, true, 0, 3f)});
//		//endWhile
//		fatherActions.MoveHere (destinations [6]);
//		fatherActions.ChangeHeight (HeightState.High);
//		//if(fatherActions.DistToPlayer() < 4f)
//		fatherActions.ChangeHeight (HeightState.Default);
//		fatherActions.MoveHere (destinations [7]);
//		fatherActions.ChangeHeight (HeightState.Low);
//		//if(fatherActions.DistToPlayer() < 3f)
//		fatherActions.ChangeHeight (HeightState.Default);
//		fatherActions.MoveHere (destinations [8]);
//		PartituraInfo[] partitura = new PartituraInfo[]
//		{
//			new PartituraInfo(HeightState.Low),
//			new PartituraInfo(HeightState.Default),
//			new PartituraInfo(HeightState.High)
//		};
//		fatherActions.Sing_Partitura(partitura);
//		fatherActions.MoveHere (destinations [9]);

		StartCoroutine ("UpdateRoteiro", 0);
	}

	IEnumerator UpdateRoteiro (int index){
		for (int i = index; i < roteiro.Count; i++) {

			currentState = i;
			fatherFSM.currentState = roteiro[i].state;

			fatherFSM.SetStateChanger (roteiro [i].stateChanger, roteiro [i].triggerDetail);

			fatherFSM.currentStateInfo = roteiro [i];

			switch (roteiro[i].state) {
			case FatherStates.Inactive:
				fatherFSM.StartInactivity ();
				break;
			case FatherStates.Gliding:
				fatherFSM.StartFly (roteiro [i].secondsFlying, roteiro [i].allowSlowFalling, roteiro [i].jumpHeight, roteiro [i].timeToJumpApex);
				break;
			case FatherStates.Jumping:
				fatherFSM.StartJump (roteiro [i].jumpHeight, roteiro [i].timeToJumpApex);
				break;
			case FatherStates.Flying:
				fatherFSM.StartFly (roteiro [i].secondsFlying, roteiro [i].allowSlowFalling, roteiro [i].jumpHeight, roteiro [i].timeToJumpApex);
				break;
			default:
				break;
			}

			yield return new WaitForSeconds (0.1f);
			yield return new WaitUntil (() => fatherFSM.changeState == true); //This is a Lambda Expression. Check it out later, it seems pretty cool!
		}
	}

	[System.Serializable]
	public class Roteiro
	{
		[Tooltip("Give a name to this state. Optional.")]
		public string name;
		public FatherStates state;

		[HideInInspector]
		public bool showAdvanced;
		[HideInInspector]
		public bool show;

		//SimpleWalk, GuidePlayer e Flying
		public Transform destination;

		//Flying e Gliding
		public float secondsFlying = 0f;
		public bool allowSlowFalling = false;

		//Flying, Gliding e Jumping
		public float jumpHeight = 5f;
		public float timeToJumpApex = 0.3f;

		//RandomWalk
		public Transform areaCenter;
		public float areaRadius;

		public StateChangers stateChanger;
		[Tooltip("May be optional, depending on StateChanger")]
		public float triggerDetail;
	}
}
