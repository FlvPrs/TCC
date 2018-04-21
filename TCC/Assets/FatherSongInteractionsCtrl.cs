using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FatherSongInteractionsCtrl : MonoBehaviour {

	public bool isSingingSomething;
	public PlayerSongs currentSong;

	void Start (){
		currentSong = PlayerSongs.Empty;
	}

	void OnTriggerStay (Collider col){
		if(col.GetComponent<ISongListener> () != null)
			col.GetComponent<ISongListener> ().DetectSong(currentSong, isSingingSomething, true);
	}
}
