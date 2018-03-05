using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Audio;

public class AgentFather : MonoBehaviour {

	public Transform player;
	protected NavMeshAgent nmAgent;
	protected Transform agentTransform;
	protected Rigidbody rb;
	protected Animator animCtrl;
	protected BoxCollider coll;

	#region ========== Debug Variables ==========
	public Transform targetReference;
	protected AudioSource sing;
	[SerializeField]
	protected AudioSource singSustain;

	public AudioMixerSnapshot clarinetDefault;
	public AudioMixerSnapshot clarinetLow;
	public AudioMixerSnapshot clarinetHigh;
	#endregion

	protected virtual void Start (){
		nmAgent = GetComponent<NavMeshAgent> ();
		agentTransform = GetComponent<Transform> ();
		rb = GetComponent<Rigidbody> ();
		animCtrl = GetComponentInChildren<Animator> ();
		coll = GetComponent<BoxCollider> ();

		#region ========== Temporary Code ==========
		sing = GetComponent<AudioSource> ();
		#endregion
	}

	protected virtual void Update (){
		distToPlayer = Vector3.Distance (player.position, agentTransform.position);
		targetReference.position = currentTargetPos;
	}

	// ---------------------------------------------------------------------

	// ============ Settings ============
	public float moveSpeed = 6f;
	public float minDistToTarget = 5f;
	public float angularSpeed = 2f;
	public float CollHeight_Low = 2f;
	public float CollHeight_Default = 4.5f;
	public float CollHeight_High = 7f;

	//TODO: Confirmar com Uiris
	protected float singleNoteMinimumDuration = 0.5f;
	// ==================================

	// ---------- Control Vars ----------
	float distToTarget;
	protected Vector3 currentTargetPos;
	protected float distToPlayer;
	protected bool isGuiding;
	protected bool isFollowingPlayer;
	protected bool isJumping; 
	protected bool isFlying;
	protected bool isRepeatingNote;
	protected bool isSustainingNote;
	protected float counter_Fly = 0f;
	protected float counter_Height = 0f;
	protected int counter_SingRepeat = 0;
	protected float counter_SingSustain = 0f;
	// ----------------------------------

	protected float gravity;
	protected float oldPosY;

	protected void MoveAgentWithRB (Vector3 target){
		if (!nmAgent.isStopped) {
			nmAgent.isStopped = true;
			rb.isKinematic = false;
		}

		currentTargetPos = target;

		distToTarget = Vector3.Distance (target, agentTransform.position);

		agentTransform.rotation = Quaternion.Slerp(agentTransform.rotation, Quaternion.LookRotation((target - agentTransform.position), Vector3.up), Time.deltaTime * angularSpeed);

		agentTransform.eulerAngles = new Vector3(0, agentTransform.eulerAngles.y, 0);

		//rb.velocity = agentTransform.forward * moveSpeed;
		rb.MovePosition(agentTransform.position + agentTransform.forward * Time.deltaTime * moveSpeed);
	}

	protected void MoveAgentOnNavMesh (Vector3 target){
		if (nmAgent.isStopped) {
			nmAgent.isStopped = false;
			rb.isKinematic = true;
		}

		currentTargetPos = target;

		nmAgent.SetDestination(target);

		distToTarget = Vector3.Distance (target, agentTransform.position);
	}

	protected bool CheckArrivedOnDestination (bool movingWithRB = false){
		//Se estiver andando com NavMesh --------------------------------
		if(!movingWithRB){
			if (!nmAgent.pathPending){
				if (nmAgent.remainingDistance <= nmAgent.stoppingDistance){
					if (!nmAgent.hasPath || nmAgent.velocity.sqrMagnitude == 0f){
						// Done
						return true;
					}
				}
			}
			return false;
		}
		//Se estiver andando com Rigidbody ------------------------------
		else {
			if (distToTarget <= minDistToTarget){
				rb.velocity = Vector3.zero;
				rb.isKinematic = true;
				return true;
			} else {
				return false;
			}
		}
	}

	protected void CalculateJump (out bool jumpOrFly, float jHeight = 5f, float timeToApex = 0.3f){
		gravity = -(2 * jHeight) / Mathf.Pow (timeToApex, 2);
		float jumpVelocity = Mathf.Abs(gravity) * timeToApex;

		nmAgent.enabled = false;
		rb.isKinematic = false;
		oldPosY = agentTransform.position.y;
		rb.velocity = new Vector3 (rb.velocity.x, jumpVelocity, rb.velocity.z);

		jumpOrFly = true;
	}

	protected void UpdateHeightCollider (HeightState currHeight) {
		float height = CollHeight_Default;

		switch (currHeight) {
		case HeightState.Default:
			height = CollHeight_Default;
			break;
		case HeightState.High:
			height = CollHeight_High;
			break;
		case HeightState.Low:
			height = CollHeight_Low;
			break;
		default:
			break;
		}

		float newYValue = Mathf.Lerp (coll.size.y, height, 0.2f);
		coll.size = new Vector3 (coll.size.x, newYValue, coll.size.z);
		coll.center = new Vector3 (coll.center.x, newYValue / 2f, coll.center.z);
	}
}

public enum FatherSongSimple {
	Empty,
	Estorvo,
	Serenidade,
	Ninar,
	Alegria
}

public enum FatherSongSustain {
	Empty,
	Amizade,
	Crescimento,
	Encolhimento,
}