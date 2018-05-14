using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeOutRespawn : MonoBehaviour {

	public bool fadeToEndLevel;
	public bool fatherRetunsPlayer = true;

	[Range(0f, 5f)]
	public float fadeToBlack_Duration = 0.5f;
	[Range(0f, 5f)]
	public float fadeFromBlack_Duration = 0.5f;

	//private float duration = 0f;

	public Image blackScrn;
	private Color color = Color.black;

	private bool fadeOut_ToBlack, fadeIn_FromBlack;

	public PlayerRespawnCtrl playerRespawn;

	public CamPriorityController camCtrl;
	public int[] spawnCamIndex;

	// Use this for initialization
	void Start () {
		fadeToBlack_Duration *= 60f;
		fadeFromBlack_Duration *= 60f;
		color.a = 0f;
	}
	
	// Update is called once per frame
	void Update () {
		if((!fadeOut_ToBlack && !fadeIn_FromBlack) || (fadeOut_ToBlack && fadeIn_FromBlack)){
			return;
		}

		if (fadeOut_ToBlack && color.a < 1f) {
			color.a += 1f / fadeToBlack_Duration;
			blackScrn.color = color;
		} else {
			fadeOut_ToBlack = false;
		}

		if(fadeIn_FromBlack && color.a > 0f){
			color.a -= 1f / fadeFromBlack_Duration;
			blackScrn.color = color;
		} else {
			fadeIn_FromBlack = false;
		}
	}

	public void StartFade (Transform player, Vector3 respawnPosition, int spawnIndex) {
		StartCoroutine (ReverseFade (player, respawnPosition, spawnIndex));
	}

	IEnumerator ReverseFade(Transform player, Vector3 respawnPosition, int spawnIndex) {
		color.a = 0f;
		fadeOut_ToBlack = true;
		fadeIn_FromBlack = false;

		while(fadeOut_ToBlack){
			yield return new WaitForSeconds (0.1f);
		}

		color.a = 1f;
		fadeOut_ToBlack = false;

		if (fadeToEndLevel) {
			yield return new WaitForSeconds (3f);
			//EndGame.Restart ();
			EndGame.ChangeLevel ();
			yield return null;
		}

		FindObjectOfType<MenuControllerInGame> ().TrocaMenus (5);

		player.GetComponent<PlayerCollisionsCtrl> ().RestartVenenoTimer ();

		//player.position = respawnPosition;
		if(fatherRetunsPlayer)
			StartCoroutine(playerRespawn.ReturnToSpawn (respawnPosition));
		else
			playerRespawn.ReturnToSpawnAlone (respawnPosition);

		yield return new WaitForSeconds (1f);
		fadeIn_FromBlack = true;

		while(fadeIn_FromBlack){
			yield return new WaitForSeconds (0.1f);
		}

		color.a = 0f;
		fadeOut_ToBlack = false;
		fadeIn_FromBlack = false;


		if (fatherRetunsPlayer) {
			yield return new WaitForSeconds (3f);
		}
		camCtrl.ChangeCameraTo (spawnCamIndex [spawnIndex]);
	}
}
