using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class RoteiroPai : MonoBehaviour {

	public FatherFSM fatherFSM;

	public List<Roteiro> roteiro;

	public int currentState;

	private RoteiroSongsList songsList;

	public static RoteiroPai instance;
	void Awake() 
	{
		if(instance == null)
		{
			instance = this;
		}
		else
		{
			if(this != instance)
				Destroy(this.gameObject);
		}
	}

	void Start () {
		songsList = GetComponentInChildren<RoteiroSongsList>();

		StartCoroutine ("UpdateRoteiro", 0);
	}

	IEnumerator UpdateRoteiro (int index){
		yield return new WaitForSeconds (0.1f);

		for (int i = index; i < roteiro.Count; i++) {

			currentState = i;
			fatherFSM.currentState = roteiro [i].state;

			fatherFSM.SetStateChanger (roteiro [i].stateChanger, roteiro [i].triggerDetail);

			fatherFSM.currentStateInfo = roteiro [i];

			fatherFSM.ChangeHeight (roteiro[i].startingHeight);

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

			switch (roteiro[i].songType) {
			case FatherSongType.Partitura:
				fatherFSM.StartPartitura (songsList.listaDePartituras [roteiro [i].songIndex].partitura);
				break;
			case FatherSongType.MusicaSimples:
				fatherFSM.StartSimpleSong (roteiro [i].simpleSong);
				break;
			case FatherSongType.MusicaComSustain:
				fatherFSM.StartSustainSong (roteiro [i].sustainSong, roteiro [i].duration);
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

		public FatherSongType songType;

		//SongType Partitura
		public int songIndex;

		//SongType Simples
		public FatherSongSimple simpleSong;

		//SongType Sustain
		public FatherSongSustain sustainSong;
		public float duration;

		[Tooltip("Will be overriden by any trigger or if singing")]
		public HeightState startingHeight;

		public StateChangers stateChanger;
		[Tooltip("May be optional, depending on StateChanger")]
		public float triggerDetail;
	}


	/// <summary>
	/// Restarts the roteiro at currentState. Useful for repeating states while waiting for an external trigger.
	/// </summary>
	public static void RestartRoteiroAt (){
		instance.StopAllCoroutines ();
		instance.StartCoroutine ("UpdateRoteiro", instance.currentState);
	}

	/// <summary>
	/// Restarts the roteiro at index.
	/// </summary>
	/// <param name="index">Index of the state.</param>
	public static void RestartRoteiroAt (int index){
		instance.StopAllCoroutines ();
		instance.StartCoroutine ("UpdateRoteiro", index);
	}
}
