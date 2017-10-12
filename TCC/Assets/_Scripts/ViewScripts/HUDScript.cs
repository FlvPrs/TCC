using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDScript : MonoBehaviour {

	public Text wingsL;
	public Text wingsR;

	void Awake(){
		wingsL.gameObject.SetActive (false);
		wingsR.gameObject.SetActive (false);
	}

	public void UpdateWingUI(bool showUp, int stamina){
		if(!showUp){
			wingsL.gameObject.SetActive (false);
			wingsR.gameObject.SetActive (false);
			return;
		}
		wingsL.gameObject.SetActive (true);
		wingsR.gameObject.SetActive (true);

		string txtL, txtR;

		switch (stamina) {
		case 4:
			txtL = "((((";
			txtR = "))))";
			break;
		case 3:
			txtL = "(((";
			txtR = ")))";
			break;
		case 2:
			txtL = "((";
			txtR = "))";
			break;
		case 1:
			txtL = "(";
			txtR = ")";
			break;
		default:
			txtL = txtR = "";
			break;
		}

		wingsL.text = txtL;
		wingsR.text = txtR;
	}
}
