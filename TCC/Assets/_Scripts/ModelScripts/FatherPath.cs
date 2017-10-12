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

	[HideInInspector]
	public bool esperaFilho = true;

	// Use this for initialization
	void Start () {
        currentWayPoint = 0;
        agent = GetComponent<NavMeshAgent>();
	}
	
	// Update is called once per frame
	void Update () {
        switch (state)
        {
            case FSMStates.Idle: IdleSimulator(); break;
            case FSMStates.Path: PathFinder(); break;

            default: print("BUG: state should never be on default clause"); break;
        }
	}

    private void IdleSimulator()
    {
        //dunno yet
    }

    private void PathFinder()
    {
        agent.SetDestination(target[currentWayPoint].transform.position);
        
    }

    public void ChangeWaypoint()
    {
		if (currentWayPoint >= target.Length - 1)
		{
			//currentWayPoint = 0;
			state = FSMStates.Idle;
			return;
		}

		if(esperaFilho)
			state = FSMStates.Idle;
		
        target[currentWayPoint + 1].SetActive(true);
        target[currentWayPoint].SetActive(false);
        currentWayPoint++;
    }
}
