using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instrumento_FatherSing : MonoBehaviour, ISongListener {

	public FatherActions fatherActions;
	public HeightState noteHeight;

	bool fatherSang = false;

	public void Start(){
		fatherActions = FindObjectOfType<FatherActions> ();
	}

	public void DetectSong (PlayerSongs song, bool isSingingSomething, bool isFather = false, HeightState height = HeightState.Default){
		if (!isFather || fatherSang)
			return;
		fatherSang = true;
		switch (noteHeight) {
		case HeightState.Low:
			fatherActions.ChangeHeight (HeightState.Low);
			fatherActions.Sing_Partitura (new PartituraInfo[] { new PartituraInfo (HeightState.Low, false, 0.1f) } );
			break;
		case HeightState.Default:
			fatherActions.ChangeHeight (HeightState.Default);
			fatherActions.Sing_Partitura (new PartituraInfo[] { new PartituraInfo (HeightState.Default, false, 0.1f) } );
			break;
		case HeightState.High:
			fatherActions.ChangeHeight (HeightState.High);
			fatherActions.Sing_Partitura (new PartituraInfo[] { new PartituraInfo (HeightState.High, false, 0.1f) } );
			break;
		default:
			break;
		}
	}

	void OnDisable (){
		fatherSang = false;
	}
}
