using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerraFertilScript : MonoBehaviour {
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (gameObject.transform.childCount > 1) {
			gameObject.transform.GetChild(0).gameObject.SetActive(true);
			gameObject.transform.GetChild(1).gameObject.SetActive(false);
		}
	}
}
