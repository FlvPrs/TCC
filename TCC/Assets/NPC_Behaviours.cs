using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPC_Behaviours : MonoBehaviour {

	public bool randomizeOnStart;
	public bool walk, reactToPlayerHeight, reactToPlayerSing, followPlayerOnSing;

	public WalkingController playerCtrl;

	public bool squashOnTrigger, stretchOnTrigger; //Only work if reactToPlayerHeight.

	public SkinnedMeshRenderer birdBody;
	public Material[] NPC_materials;

	private RandomWalk rndWalk;
	private AudioSource sing;
	private Animator anim;
	private NavMeshAgent agent;

	private bool canSingAgain = true;
	private float waitTime = 1f;

	private bool isFollowingPlayer;

	// Use this for initialization
	void Start () {
		if(randomizeOnStart){
			do {
				walk = (Random.Range (0, 10) >= 5) ? true : false;
				reactToPlayerHeight = (Random.Range (0, 10) >= 5) ? true : false;
				reactToPlayerSing = (Random.Range (0, 10) >= 5) ? true : false;

				if(reactToPlayerHeight){
					squashOnTrigger = (Random.Range (0, 10) >= 5) ? true : false;
					stretchOnTrigger = !squashOnTrigger;
				}
			} 
			while (!walk && !reactToPlayerHeight && !reactToPlayerSing);
		}

		if(!reactToPlayerHeight){
			squashOnTrigger = false;
			stretchOnTrigger = false;
		}

		rndWalk = GetComponent<RandomWalk> ();
		sing = GetComponent<AudioSource> ();
		anim = GetComponent<Animator> ();
		agent = GetComponent<NavMeshAgent> ();

		if(!reactToPlayerSing)
			followPlayerOnSing = reactToPlayerSing;

		rndWalk.enabled = walk;
		anim.SetBool("isWalking", walk);
		anim.SetBool ("isGrounded", true);

		if(reactToPlayerHeight && reactToPlayerSing){
			birdBody.material = NPC_materials[3];
		}
		else if(reactToPlayerHeight){
			birdBody.material = NPC_materials[2];
		}
		else if(reactToPlayerSing){
			birdBody.material = NPC_materials[1];
		}
		else {
			birdBody.material = NPC_materials[0];
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (isFollowingPlayer && !agent.hasPath) {
			if (walk) {
				anim.SetBool ("isWalking", walk);
				rndWalk.PauseWalk (!walk);
			} else {
				anim.SetBool ("isWalking", false);
			}
			isFollowingPlayer = false;
		}

		if (!playerCtrl.walkStates.TOCANDO_FLOREIO && !playerCtrl.walkStates.TOCANDO_STACCATO && !playerCtrl.walkStates.TOCANDO_SUSTAIN) {
			waitTime -= Time.deltaTime;
		} else {
			if (followPlayerOnSing && (playerCtrl.transform.position - transform.position).magnitude < 15f) {
				anim.SetBool("isWalking", true);
				agent.isStopped = false;
				agent.SetDestination (playerCtrl.transform.position);
				rndWalk.PauseWalk(true);
				isFollowingPlayer = true;
			}
		}
	}

	IEnumerator ReactToSing(){
		while (waitTime > 0f) {
			yield return new WaitForSeconds (1f);
			canSingAgain = false;
		}
		anim.SetBool ("isSinging", true);
		sing.Play ();

		yield return new WaitForSeconds (1f);
		anim.SetBool ("isSinging", false);
		canSingAgain = true;
	}

	void ReactToHeight(){
		if (stretchOnTrigger) {
			//Run stretch animation
		} else if (squashOnTrigger) {
			//Run squash animation
		} else {
			switch (playerCtrl.walkStates.CURR_HEIGHT_STATE) {
			case HeightState.Default:
				//Do nothing
				anim.SetFloat ("Height", 0f);
				break;

			case HeightState.High:
				//Run squash animation
				anim.SetFloat ("Height", -1f);
				break;

			case HeightState.Low:
				//Run stretch animation
				anim.SetFloat ("Height", 1f);
				break;

			default:
				break;
			}
		}
	}

	void OnTriggerEnter(Collider col){
		if(col.CompareTag("Player")){
			if(walk)
				rndWalk.PauseWalk(false);
		}
	}

	void OnTriggerStay(Collider col){
		if(col.CompareTag("Player")){
			anim.SetBool("isWalking", false);
			agent.isStopped = true;

			if(reactToPlayerHeight && !(squashOnTrigger || stretchOnTrigger)){
				ReactToHeight ();
			}

			if (reactToPlayerSing) {
				StopCoroutine (ReactToSing ());
				if (canSingAgain && (playerCtrl.walkStates.TOCANDO_FLOREIO || playerCtrl.walkStates.TOCANDO_STACCATO || playerCtrl.walkStates.TOCANDO_SUSTAIN)){
					waitTime = 1f;
					StartCoroutine(ReactToSing ());
				}
			}

			if(squashOnTrigger){
				anim.SetFloat ("Height", -1f);
			} else if (stretchOnTrigger) {
				anim.SetFloat ("Height", 1f);
			}
		}
	}

	void OnTriggerExit(Collider col){
		if(col.CompareTag("Player")){
			if(walk){
				agent.isStopped = false;
				anim.SetBool("isWalking", true);
			}

			if(squashOnTrigger || stretchOnTrigger){
				anim.SetFloat ("Height", 0f);
			}
		}
	}
}









/*
 *
 * public Color color_Basic, color_Height, color_Sing, color_Full;
 *
 * Mesh mesh = gameObject.GetComponent<MeshFilter>().mesh;
 *
 * Color[] colours = mesh.colors;
 *
 * for(int i = 0; i < colours.Length; i++)
 * {
 *	colours[i] = color_Full;//the new colour you want to set it to.
 * }
 *
 */