using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSongInteractionsCtrl : MonoBehaviour {

//	public LayerMask interactLayers;
//	public Collider[] hitColliders;
//
//	void Update (){
//		Physics.OverlapSphereNonAlloc(transform.position, 10, hitColliders, interactLayers);
//	}

	public bool isSingingSomething;
	public PlayerSongs currentSong;

	void Start (){
		currentSong = PlayerSongs.Empty;
	}

	void OnTriggerStay (Collider col){
		if(col.GetComponent<ISongListener> () != null)
			col.GetComponent<ISongListener> ().DetectSong(currentSong);
	}
}
