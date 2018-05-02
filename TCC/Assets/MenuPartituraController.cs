using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPartituraController : MonoBehaviour {

	Transform partiturasGroup;
	GameObject[] partituras;
	bool menuAberto = false;
	bool apertouDirecional = false;
	int currIndex = 0;

	// Use this for initialization
	void Start () {
		partiturasGroup = transform.GetChild (0);
		partituras = new GameObject[partiturasGroup.childCount];

		for (int i = 0; i < partituras.Length; i++) {
			partituras [i] = partiturasGroup.GetChild (i).gameObject;
			partituras [i].SetActive (false);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetAxis("L_Trigger") != 0 || Input.GetKey(KeyCode.Tab)){
			int oldIndex = currIndex;

			if(!menuAberto){
				currIndex = 0;
			} else {
				#region DirectionInput
				if(Input.GetAxis("DPad_X") <= -0.1f || Input.GetKeyDown(KeyCode.E)){
					if (!apertouDirecional) {
						apertouDirecional = true;
						currIndex++;
					}
				} else if (Input.GetAxis("DPad_X") >= 0.1f || Input.GetKeyDown(KeyCode.Q)) {
					if (!apertouDirecional) {
						apertouDirecional = true;
						currIndex--;
					}
				} else {
					apertouDirecional = false;
				}
				#endregion

				if (currIndex >= partituras.Length)
					currIndex = 0;
				else if (currIndex < 0)
					currIndex = partituras.Length - 1;
			}

			if(oldIndex != currIndex){
				for (int i = 0; i < partituras.Length; i++) {
					partituras [i].SetActive (false);
				}
			}

			partituras [currIndex].SetActive (true);

			menuAberto = true;
		} else {
			menuAberto = false;
		}

		partiturasGroup.gameObject.SetActive (menuAberto);
	}
}
