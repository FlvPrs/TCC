using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DelayNavMeshBake : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Invoke ("DelayedNavMeshBake", 0.2f);
	}

	void DelayedNavMeshBake (){
		GetComponent<NavMeshSurface> ().BuildNavMesh ();
	}
}
