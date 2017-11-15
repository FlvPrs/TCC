using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FatherWPBehaviour : MonoBehaviour {

	public enum FatherBehaviour
	{
		None,
		Stretch,
		Squash,
		Sing_Default,
		Sing_Stretched,
		Sing_Squashed
	}

	public FatherBehaviour behaviour;
	public float behaviourTime = 0f;
	public bool ignorePlayer = false;

	void Start () {
		if(behaviour == FatherBehaviour.None)
			enabled = false;
	}

	public void StartBehaviour(FatherPath path, FatherHeightCtrl height, FatherSingCtrl sing){
		switch (behaviour) {

		case FatherBehaviour.Squash:
			path.wait = ignorePlayer;
			StartCoroutine (WaitForChangeHeight(height, -0.9f));
			StartCoroutine (StopBehaviour(path, height));
			break;
		case FatherBehaviour.Stretch:
			path.wait = ignorePlayer;
			StartCoroutine (WaitForChangeHeight(height, 0.9f));
			StartCoroutine (StopBehaviour(path, height));
			break;

		case FatherBehaviour.Sing_Default:
			path.wait = ignorePlayer;
			sing.StartClarinet_Sustain (true);
			StartCoroutine (StopBehaviour(path, sing));
			break;

		case FatherBehaviour.Sing_Squashed:
			path.wait = ignorePlayer;
			StartCoroutine (WaitForChangeHeight(height, -0.9f, sing));
			StartCoroutine (StopBehaviour(path, height, sing));
			break;
		case FatherBehaviour.Sing_Stretched:
			path.wait = ignorePlayer;
			StartCoroutine (WaitForChangeHeight(height, 0.9f, sing));
			StartCoroutine (StopBehaviour(path, height, sing));
			break;


		default:
			path.ChangeWaypoint ();
			enabled = false;
			break;
		}
	}
		


	IEnumerator WaitForChangeHeight(FatherHeightCtrl height, float strength){
		yield return new WaitForSeconds (0.25f);
		height.UpdateHeight (strength);
	}
	IEnumerator WaitForChangeHeight(FatherHeightCtrl height, float strength, FatherSingCtrl sing){
		yield return new WaitForSeconds (0.25f);
		height.UpdateHeight (strength);
		sing.StartClarinet_Sustain (true);
	}


	IEnumerator StopBehaviour(FatherPath path, FatherSingCtrl sing){
		yield return new WaitForSeconds (behaviourTime);
		sing.StartClarinet_Sustain (false);
		path.wait = false;
		path.ChangeWaypoint ();
	}
	IEnumerator StopBehaviour(FatherPath path, FatherHeightCtrl height){
		yield return new WaitForSeconds (behaviourTime);
		height.UpdateHeight (0f);
		path.wait = false;
		path.ChangeWaypoint ();
	}
	IEnumerator StopBehaviour(FatherPath path, FatherHeightCtrl height, FatherSingCtrl sing){
		yield return new WaitForSeconds (behaviourTime);
		height.UpdateHeight (0f);
		sing.StartClarinet_Sustain (false);
		path.wait = false;
		path.ChangeWaypoint ();
	}
}
