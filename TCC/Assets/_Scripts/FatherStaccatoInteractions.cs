﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FatherStaccatoInteractions : MonoBehaviour {

	FatherActions father;
	PlayerSongs fatherSong;

	void Awake (){
		father = GetComponentInParent<FatherActions> ();
	}

	void Update (){
		fatherSong = father.currentSong;
	}

	void OnTriggerEnter (Collider col){
		if(col.GetComponent<IFatherStaccatoInteractable> () != null)
			col.GetComponent<IFatherStaccatoInteractable> ().FatherStaccatoInteraction (fatherSong);
	}
}
