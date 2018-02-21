using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentFather : MonoBehaviour {

	public Transform player;
	protected NavMeshAgent nmAgent;
	protected Transform agentTransform;
	protected Rigidbody rb;

	protected virtual void Start (){
		nmAgent = GetComponent<NavMeshAgent> ();
		agentTransform = GetComponent<Transform> ();
		rb = GetComponent<Rigidbody> ();
	}

	// ---------------------------------------------------------------------

	// ============ Settings ============
	public float moveSpeed = 6f;
	public float minDistToTarget = 2f;
	public float angularSpeed = 2f;
	// ==================================

	// ---------- Control Vars ----------
	float distToTarget;
	// ----------------------------------

	protected void MoveAgentWithRB (Vector3 target){
		if (!nmAgent.isStopped) {
			nmAgent.isStopped = true;
			rb.isKinematic = false;
		}

		distToTarget = Vector3.Distance (target, agentTransform.position);

		transform.rotation = Quaternion.Slerp(agentTransform.rotation, Quaternion.LookRotation((target - agentTransform.position), Vector3.up), Time.deltaTime * angularSpeed);

		rb.velocity = agentTransform.forward * moveSpeed;
	}

	protected void MoveAgentOnNavMesh (Vector3 target){
		if (nmAgent.isStopped) {
			nmAgent.isStopped = false;
			rb.isKinematic = true;
		}
		
		nmAgent.SetDestination(target);

		distToTarget = nmAgent.remainingDistance;
	}

	protected bool CheckStopMoving (){
		if (distToTarget <= minDistToTarget || /*nmAgent.pathStatus == NavMeshPathStatus.PathPartial ||*/ nmAgent.pathStatus == NavMeshPathStatus.PathInvalid) {
			//nmAgent.ResetPath ();
			rb.velocity = Vector3.zero;
			rb.isKinematic = true;
			return true;
		} else {
			return false;
		}
	}
}
