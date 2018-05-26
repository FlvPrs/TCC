using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class quedaCollider : MonoBehaviour {

	#region FMOD stuff
	[FMODUnity.EventRef]
	public string paiQueda;
	FMOD.Studio.EventInstance fatherFall;
	public bool tocou;
	public bool af;
	#endregion

	private Vector3 posicaoInicial;
	private Vector3 posicaoQueda;
	public GameObject player;
	public WayPointQueda wayPoint;
	public TranslateObject movePai;

	bool startMove;
	// Use this for initialization
	void Start () {
		#region FMOD stuf
		tocou = true;
		af = false;
		paiQueda = "event:/Pai/PaiQueda";
		fatherFall = FMODUnity.RuntimeManager.CreateInstance (paiQueda);
		#endregion

		posicaoInicial = player.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
//		if(Input.GetKeyDown(KeyCode.H)){
//			player.transform.position = posicaoInicial;
//			movePai.startMove = false;
//		}

		if (startMove) {
			movePai.UpdateDestination (player.transform.position, false, true, false);
		}
	}

	void OnTriggerEnter(Collider colisor){
		if(colisor.name == "PlayerCollider"){
			posicaoQueda = colisor.transform.position;
			wayPoint.GetPlayerPosition (posicaoQueda);
			movePai.StartMove();
			startMove = true;

			if (tocou) {
				fatherFall.start ();
				tocou = false;
				af = true;
			}
		}
	}

}
