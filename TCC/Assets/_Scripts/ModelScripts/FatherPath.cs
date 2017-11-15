using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FatherPath : MonoBehaviour {

    public GameObject[] target;

    public enum FSMStates { Idle, Path};
    public FSMStates state = FSMStates.Idle;

    private NavMeshAgent agent;
    public int currentWayPoint;

	public Transform filho;

	[HideInInspector]
	public bool esperaFilho = true;
	[HideInInspector]
	public bool wait = false;

	private Vector3	dir;
	private Transform t;
	private Animator animCtrl;

	// Use this for initialization
	void Start () {
        currentWayPoint = 0;
        agent = GetComponent<NavMeshAgent>();
		t = GetComponent<Transform> ();
		animCtrl = GetComponentInChildren<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {
        switch (state)
        {
            case FSMStates.Idle: 
				animCtrl.SetBool ("isGrounded", true);
				animCtrl.SetBool ("isWalking", false);
				IdleSimulator(); 
				break;
            case FSMStates.Path:
				animCtrl.SetBool ("isWalking", true);
				PathFinder(); 
				break;

            default: print("BUG: state should never be on default clause"); break;
        }
	}

    private void IdleSimulator()
    {
		if (!agent.hasPath) {
			dir = filho.position - t.position;
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir, Vector3.up), Time.deltaTime * 2f);
		}
    }

    private void PathFinder()
    {
        agent.SetDestination(target[currentWayPoint].transform.position);
        
		if(agent.isOnOffMeshLink)
			animCtrl.SetBool ("isGrounded", false);
		else
			animCtrl.SetBool ("isGrounded", true);
    }

    public void ChangeWaypoint()
    {
		if (currentWayPoint >= target.Length - 1)
		{
			//currentWayPoint = 0;
			state = FSMStates.Idle;
			return;
		}

		if (esperaFilho || wait) {
			state = FSMStates.Idle;
		}
		
        target[currentWayPoint + 1].SetActive(true);
        target[currentWayPoint].SetActive(false);
        currentWayPoint++;
    }
}
