using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerBackgroundMusic : MonoBehaviour {

	public static int playerProgress = 0;

	public int positionNumber;


	// Use this for initialization
	void Start () {
		playerProgress = 0;
	}


	void Update(){
		switch (playerProgress) {
		case 1:
			//musiquinha bacaninha
			break;

		case 2:
			//musiquinha diferentinha
			break;

		//Etc...

		default: //Se não for nenhuma das que vc colocou acima...
			//musiquinha default
			break;
		}
	}


	void OnTriggerEnter(Collider col){
		if(col.CompareTag("Player")){
			playerProgress = positionNumber;
		}
	}


//	void OnTriggerStay(Collider col){
//		if(col.CompareTag("Player")){
//			playerProgress = positionNumber;
//		}
//	}


	void OnTriggerExit(Collider col){
		playerProgress = 0;
	}
}
