using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instrumento_DevCheat : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Comma)){
			FindObjectOfType<InstrumentoBehaviour> ().CHEAT_FinishInstrumento = true;
			enabled = false;
		}
	}
}
