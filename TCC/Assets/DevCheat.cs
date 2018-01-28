using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevCheat : MonoBehaviour {

	public bool slowDown_a1;
	private float originalTimeScale;

	// Use this for initialization
	void Start () {
		originalTimeScale = Time.timeScale;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Alpha1)){
			slowDown_a1 = !slowDown_a1;
		}


		if(slowDown_a1){
			Time.timeScale = 0.2f;
		} else {
			Time.timeScale = originalTimeScale;
		}
	}
}
