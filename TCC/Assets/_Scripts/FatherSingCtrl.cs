using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FatherSingCtrl : MonoBehaviour {

	[FMODUnity.EventRef]

	public string paiCalling;
	FMOD.Studio.EventInstance audiochamaPai;

	public FatherSustainInteractions sustainCollider;
//	public StaccatoInteractionsCtrl partituraCollider;

	private FatherHeightCtrl heightCtrl;

	private AudioSource sing;

	[HideInInspector]
	public bool canAdvance;

	private bool waitToSing;

	//private bool tocouPartitura;

	void Awake () {

		//FMOD
		paiCalling = "event:/Pai/PaiChamado";
		audiochamaPai = FMODUnity.RuntimeManager.CreateInstance (paiCalling);



		sustainCollider.gameObject.SetActive (false);
//		partituraCollider.gameObject.SetActive (false);

		sing = GetComponent<AudioSource> ();
		heightCtrl = GetComponent<FatherHeightCtrl> ();
	}

//	void Update(){
//		if (!sustainCollider.gameObject.activeInHierarchy) {
//			//sustainCollider.canAdvance = false;
//			//canAdvance = false;
//			return;
//		}
//
//		canAdvance = sustainCollider.canAdvance;
//
//		if (sustainCollider.stopSing && !waitToSing) {
//			waitToSing = true;
//			//sing.Stop ();
//		} else if (!sustainCollider.stopSing && waitToSing) {
//			waitToSing = false;
//			//sing.Play ();
//			audiochamaPai.start();
//		}
//	}

//	public void StartClarinet_Staccato(){
//		if(!sing.isPlaying)
//			sing.Play ();
//
//		StartCoroutine("StopStaccato");
//	}
//
//	IEnumerator StopStaccato(){
//		yield return new WaitForSeconds (0.2f);
//		sing.Stop ();
//	}

	public void StartClarinet_Sustain(bool start){
		if (start) { //ou seja, se ainda tiver 25% de ar disponível
			//sing.Play ();
			//sustainCollider.currentHeight = oldState;
			audiochamaPai.start();

			sustainCollider.gameObject.SetActive (true);
		} else {
			//sing.Stop ();
			sustainCollider.gameObject.SetActive (false);
		}
	}
}
