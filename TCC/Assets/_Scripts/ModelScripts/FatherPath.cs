using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FatherPath : MonoBehaviour {

    public GameObject[] target;

	public enum FatherEmotions { Alegria, Tristeza, Medo, Raiva };
	public enum FatherAttitudes { None, Apressado, Distante, Carinhoso };
	public FatherEmotions currentEmotion = FatherEmotions.Alegria;
	public FatherAttitudes currentAttitude = FatherAttitudes.None;

	public FatherBehaviour currentBehaviour = FatherBehaviour.None;

    public enum FSMStates { Idle, Path, FollowingPlayer, GuidingPlayer };
    public FSMStates state = FSMStates.Idle;

    private NavMeshAgent agent;
    public int nextWaypoint;
	public int currentWayPoint;

	public Transform filho;

	//[HideInInspector]
	public bool esperaFilho = true;
	[HideInInspector]
	public bool wait = false;
	[HideInInspector]
	public bool holdWPBehaviour;

	private Vector3	dir;
	private Transform t;
	private Animator animCtrl;

	private FatherHeightCtrl fatherHeight;
	private FatherSingCtrl fatherSing;

	public float minDistanceToWaypoint = 2f;
	private float distToWP = 0f;

	public bool isGuidingPlayer = false;
	private float maxTimeWaitingBeforeCall = 8f;
	private float timeWaiting = 0f;
	private float maxTimeBeforeFollow = 5f;
	private float timeBeforeFollow = 0f;
	private float maxDistToPlayer = 45f;
	private float maxDistToGuide = 12f;
	private float minDistToPlayer = 8f;
	private float distToPlayer = 0f;

	// Use this for initialization
	void Start () {
        nextWaypoint = 0;
		currentWayPoint = 0;
        agent = GetComponent<NavMeshAgent>();
		t = GetComponent<Transform> ();
		animCtrl = GetComponentInChildren<Animator> ();
		fatherHeight = GetComponent<FatherHeightCtrl> ();
		fatherSing = GetComponent<FatherSingCtrl> ();

		switch (currentAttitude) {

		case FatherAttitudes.Apressado:
			//state = FSMStates.Idle;
			maxTimeWaitingBeforeCall = 4f;
			maxDistToPlayer = 20f;
			break;
		case FatherAttitudes.Carinhoso:
			//state = FSMStates.Idle;
			maxTimeWaitingBeforeCall = 7f;
			maxDistToPlayer = 30f;
			break;
		case FatherAttitudes.Distante:
			//state = FSMStates.Idle;
			maxTimeWaitingBeforeCall = 12f;
			maxDistToPlayer = 60f;
			break;

		default:
			maxTimeWaitingBeforeCall = 8f;
			maxDistToPlayer = 45f;
			break;
		}
	}
	
	// Update is called once per frame
	void Update () {
		distToPlayer = Vector3.Distance (filho.position, t.position);

		if (distToPlayer > maxDistToPlayer) { //Se o jogador estiver longe demais...
			if (timeBeforeFollow >= maxTimeBeforeFollow) //Espera um pouco. Se o tempo de espera acabou...
				state = FSMStates.FollowingPlayer; //Começa a seguir o player.
			else
				timeBeforeFollow += Time.deltaTime;
		} else if (isGuidingPlayer) {
			timeBeforeFollow = 0f;
			state = FSMStates.GuidingPlayer;
		}

		if(agent.isOnOffMeshLink)
			animCtrl.SetBool ("isGrounded", false);
		else
			animCtrl.SetBool ("isGrounded", true);

		animCtrl.SetBool ("isWalking", true);

        switch (state)
        {
        case FSMStates.Idle: 
			animCtrl.SetBool ("isWalking", false);
			IdleSimulator(); 
			break;
		case FSMStates.Path:
			timeWaiting = 0f;
			timeBeforeFollow = 0f;
			PathFinder(); 
			break;
		case FSMStates.FollowingPlayer:
			timeBeforeFollow = 0f;
			FollowPlayer_State ();
			break;
		case FSMStates.GuidingPlayer:
			timeBeforeFollow = 0f;
			GuidePlayer_State ();
			break;


            default: print("BUG: state should never be on default clause"); break;
        }
	}

	#region Idle
    private void IdleSimulator()
    {
		switch (currentAttitude) {

		case FatherAttitudes.Apressado:
			//state = FSMStates.Idle;
			maxTimeWaitingBeforeCall = 4f;
			maxDistToPlayer = 20f;
			break;
		case FatherAttitudes.Carinhoso:
			//state = FSMStates.Idle;
			maxTimeWaitingBeforeCall = 7f;
			maxDistToPlayer = 30f;
			break;
		case FatherAttitudes.Distante:
			//state = FSMStates.Idle;
			maxTimeWaitingBeforeCall = 12f;
			maxDistToPlayer = 60f;
			break;

		default:
			maxTimeWaitingBeforeCall = 8f;
			maxDistToPlayer = 45f;
			break;
		}

//		switch (currentBehaviour) {
//		case FatherBehaviour.Squash:
//			//do smt;
//			break;
//		case FatherBehaviour.Stretch:
//			//do smt;
//			break;
//		case FatherBehaviour.Sing_Default:
//			//do smt;
//			break;
//		case FatherBehaviour.Sing_Squashed:
//			//do smt;
//			break;
//		case FatherBehaviour.Sing_Stretched:
//			//do smt;
//			break;
//
//		default:
//			state = FSMStates.Idle;
//			break;
//		}

		if(currentAttitude != FatherAttitudes.Distante && !agent.hasPath){
			LookAtPlayer ();
		}
		if(!agent.hasPath){
			if(timeWaiting < maxTimeWaitingBeforeCall){
				timeWaiting += Time.deltaTime;
			} else {
				timeWaiting = -2f;
				StartCoroutine(CallPlayer ());
			}
		}
    }
	#endregion

	#region WayPoint Pathfinding
    private void PathFinder()
    {
		distToWP = Vector3.Distance (target[nextWaypoint].transform.position, t.position);
		if (distToWP <= minDistanceToWaypoint) {
			esperaFilho = true;
			ChangeWaypoint (false);
			return;
		}

        agent.SetDestination(target[nextWaypoint].transform.position);
    }

	public void ChangeWaypoint(bool goToNearest)
    {
		currentBehaviour = target [nextWaypoint].GetComponent<FatherWPBehaviour> ().behaviour;
		if(currentBehaviour != FatherBehaviour.None){
			return;
		}

		currentWayPoint = nextWaypoint;

		if (nextWaypoint >= target.Length - 1)
		{
			//nextWaypoint = 0;
			state = FSMStates.Idle;
			return;
		}

		if (esperaFilho || wait) {
			state = FSMStates.Idle;
		} else
			esperaFilho = true;
		
		if(goToNearest){
			float minDist =  99999;
			int minI = 0;
			for (int i = 0; i < target.Length; i++) {
				float dist = Vector3.Distance (target[i].transform.position, t.position);
				if (dist < minDist) {
					minDist = dist;
					minI = i;
				}
			}
			//target[nextWaypoint].SetActive(false);
			nextWaypoint = minI;
			//target[nextWaypoint].SetActive(true);
		} else {
			//target[nextWaypoint].SetActive(false);
			nextWaypoint++;
			//target[nextWaypoint].SetActive(true);
		}
    }
	#endregion

	#region LookingAtPlayer
	private void LookAtPlayer(){
		dir = filho.position - t.position;
		transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir, Vector3.up), Time.deltaTime * 2f);
	}
	#endregion

	#region CallingPlayer
	private IEnumerator CallPlayer(){
		fatherSing.StartClarinet_Sustain (true);
		yield return new WaitForSeconds (2f);
		fatherSing.StartClarinet_Sustain (false);
	}
	#endregion

	#region FollowingPlayer
	private void FollowPlayer_State(){
		timeBeforeFollow = 0f;
		agent.SetDestination(filho.position);

		if (distToPlayer < minDistToPlayer) {
			//isGuidingPlayer = true;
			nextWaypoint = currentWayPoint;
			ChangeWaypoint (true);
			state = FSMStates.Idle;
		}
	}
	#endregion

	#region GuidingPlayer
	private void GuidePlayer_State(){
		if(distToPlayer < maxDistToGuide){
			agent.isStopped = false;
			agent.SetDestination(target[nextWaypoint].transform.position);
		} else {
			agent.isStopped = true;
		}
	}
	#endregion
}
