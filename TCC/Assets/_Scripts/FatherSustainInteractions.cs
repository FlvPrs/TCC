using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FatherSustainInteractions : MonoBehaviour {

	FatherActions father;
	PlayerSongs fatherSong;

	void Awake (){
		father = GetComponentInParent<FatherActions> ();
	}

	void Update (){
		fatherSong = father.currentSong;
	}

	void OnTriggerStay (Collider col){
		if(col.CompareTag("PaiCanInteract"))
			col.GetComponent<IFatherSustainInteractable> ().FatherSustainInteraction (fatherSong);
	}
}
