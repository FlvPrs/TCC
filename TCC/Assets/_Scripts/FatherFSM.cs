using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FatherFSM : MonoBehaviour {

	private FatherActions fatherActions;

	void Start () {
		fatherActions = GetComponent<FatherActions> ();
	}

	void Update () {
		
	}


}
