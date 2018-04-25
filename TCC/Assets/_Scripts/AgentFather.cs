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
	//public GameObject staccatoColl, sustainColl;
	[SerializeField]
	GameObject l_wing, r_wing;
	[HideInInspector]
	public float timeMoving;
	[Range(1f, 300f)]
	public float maxStamina;
	public float currentStamina;

	[HideInInspector]
	public FatherConditions currentDisposition;

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

		//staccatoColl.SetActive (false);
		//sustainColl.SetActive (false);

		timeMoving = 0f;

		currentStamina = maxStamina;

		#region ========== Temporary Code ==========
		sing = GetComponent<AudioSource> ();
		#endregion
	}

	protected virtual void Update (){
		distToPlayer = Vector3.Distance (player.position, agentTransform.position);
		targetReference.position = currentTargetPos;

		if(!nmAgent.isActiveAndEnabled){
			bool isOnAir = (isJumping || isFlying);
			animCtrl.SetBool ("IsOnOffMeshLink", isOnAir);
			if(isOnAir){
				if (!IsInvoking ())
					Invoke ("openWings", 0.02f);
			}
		} else {
			if (nmAgent.isOnOffMeshLink) {
				animCtrl.SetBool ("IsOnOffMeshLink", true);
				animCtrl.SetBool ("isGrounded", false);
				openWings ();
			} else {
				animCtrl.SetTrigger ("EnteredOffMeshLink");
				animCtrl.SetBool ("IsOnOffMeshLink", false);
				animCtrl.SetBool ("isGrounded", true);
				openWings (false);
			}
		}

		string walkAnim = "isWalking";
		switch (currentDisposition) {
		case FatherConditions.Debilitado:
			//TODO trocar animação
			break;
		case FatherConditions.Machucado:
			//TODO trocar animação
			break;
		case FatherConditions.MuitoMachucado:
			//TODO trocar animação
			break;
		default:
			break;
		}

		if (rb.isKinematic) {
			animCtrl.SetBool (walkAnim, (nmAgent.enabled && !nmAgent.isStopped)? isWalking : false);
		} else {
			animCtrl.SetBool (walkAnim, isWalking);
		}
	}

	protected void openWings (bool open){
		l_wing.SetActive (open);
		r_wing.SetActive (open);
	}
	void openWings (){
		l_wing.SetActive (true);
		r_wing.SetActive (true);
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
	protected bool isWalking;
	protected bool isRandomWalking;
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
		if (currentDisposition == FatherConditions.Machucado || currentDisposition == FatherConditions.MuitoMachucado)
			return;

		if (!nmAgent.isStopped) {
			nmAgent.isStopped = true;
			rb.isKinematic = false;
		}

		isWalking = true;

		currentTargetPos = target;

		distToTarget = Vector3.Distance (target, agentTransform.position);

		agentTransform.rotation = Quaternion.Slerp(agentTransform.rotation, Quaternion.LookRotation((target - agentTransform.position), Vector3.up), Time.deltaTime * angularSpeed);

		agentTransform.eulerAngles = new Vector3(0, agentTransform.eulerAngles.y, 0);

		//rb.velocity = agentTransform.forward * moveSpeed;
		rb.MovePosition(agentTransform.position + agentTransform.forward * Time.deltaTime * moveSpeed);

		CheckArrivedOnDestination (true);
	}

	protected void MoveAgentOnNavMesh (Vector3 target){
		if (currentDisposition == FatherConditions.Machucado || currentDisposition == FatherConditions.MuitoMachucado) {
			//nmAgent.isStopped = true;
			return;
		}
		
		if (nmAgent.isStopped) {
			nmAgent.isStopped = false;
			rb.isKinematic = true;
		}

		isWalking = true;

		currentTargetPos = target;

		nmAgent.SetDestination(target);

		distToTarget = Vector3.Distance (target, agentTransform.position);

		CheckArrivedOnDestination ();
	}

	public bool CheckArrivedOnDestination (bool movingWithRB = false){
		//Se estiver andando com NavMesh --------------------------------
		if(!movingWithRB){
			if (!nmAgent.pathPending && !nmAgent.isStopped){
				if (nmAgent.remainingDistance <= nmAgent.stoppingDistance){
					if (!nmAgent.hasPath || nmAgent.velocity.sqrMagnitude == 0f){
						// Done
						isWalking = false;
						return true;
					}
				}
			}
			isWalking = true;
			return false;
		}
		//Se estiver andando com Rigidbody ------------------------------
		else {
			if(rb.isKinematic){
				return true;
			}
			if (distToTarget <= minDistToTarget){
				rb.velocity = Vector3.zero;
				rb.isKinematic = true;
				isWalking = false;
				return true;
			} else {
				isWalking = true;
				return false;
			}
		}
	}

	public float DistToPlayer (){
		return Vector3.Distance (player.position, agentTransform.position);
	}

	protected void CalculateJump (out bool jumpOrFly, float jHeight = 5f, float timeToApex = 0.3f){
		isWalking = false;
		animCtrl.SetBool ("isGrounded", false);

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
	Estorvo,
	Serenidade,
	Ninar,
	Alegria
}

public enum FatherSongSustain {
	Amizade,
	Crescimento,
	Encolhimento,
}