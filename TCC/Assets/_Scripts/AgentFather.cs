using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentFather : MonoBehaviour {

	public Transform player;
	protected NavMeshAgent nmAgent;
	protected Transform agentTransform;
	protected Rigidbody rb;
	protected Animator animCtrl;

	#region ========== Debug Variables ==========
	public Transform targetReference;
	#endregion

	protected virtual void Start (){
		nmAgent = GetComponent<NavMeshAgent> ();
		agentTransform = GetComponent<Transform> ();
		rb = GetComponent<Rigidbody> ();
		animCtrl = GetComponentInChildren<Animator> ();
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
	// ==================================

	// ---------- Control Vars ----------
	float distToTarget;
	protected Vector3 currentTargetPos;
	protected float distToPlayer;
	protected bool isGuiding;
	protected bool isFollowingPlayer;
	protected bool isJumping; 
	protected bool isFlying;
	protected float counter_Fly = 0f;
	protected float counter_Height = 0f;
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
}
