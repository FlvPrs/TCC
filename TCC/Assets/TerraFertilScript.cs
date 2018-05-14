using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerraFertilScript : MonoBehaviour {
	private bool podeReceberSemente;
	// Use this for initialization
	void Start () {
		podeReceberSemente = true;
	}

	// Update is called once per frame
	void Update () {
		if (gameObject.transform.childCount > 2) {
			TesteAtivaPlanta ();
		}
	}
	void OnTriggerEnter(Collider colisor){
		if (podeReceberSemente == true) {
//			if (colisor.name == "SementePlantaPlataforma") {
//				gameObject.transform.GetChild (0).gameObject.SetActive (true);
//				colisor.gameObject.SetActive (false);
//				podeReceberSemente = false;
//			}
//			if (colisor.name == "SementePlantaCura") {
//				gameObject.transform.GetChild (1).gameObject.SetActive (true);
//				colisor.gameObject.SetActive (false);
//				podeReceberSemente = false;
//			}
		}
	}
	void TesteAtivaPlanta(){
		if (transform.Find("SementePlantaPlataforma")) {
			gameObject.transform.GetChild (0).gameObject.SetActive (true);
			gameObject.transform.GetChild (2).gameObject.SetActive (true);
			podeReceberSemente = false;
			print (gameObject.transform.childCount);
		}
		else if (transform.Find("SementePlantaCura")) {
			gameObject.transform.GetChild (1).gameObject.SetActive (true);
			gameObject.transform.GetChild (2).gameObject.SetActive (true);
			podeReceberSemente = false;
			print (gameObject.transform.childCount);
		}
	}
}
