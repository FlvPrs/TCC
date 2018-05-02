using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeOutRespawn_Sacrifice : MonoBehaviour {

	[Range(0f, 5f)]
	public float fadeToBlack_Duration = 0.5f;
	[Range(0f, 5f)]
	public float fadeFromBlack_Duration = 1f;

	//private float duration = 0f;

	public Image blackScrn;
	private Color color = Color.black;

	private bool fadeOut_ToBlack, fadeIn_FromBlack;

	public CamPriorityController camCtrl;
	public int spawnCamIndex;
	public int deadDadCam;

	public PlayerRespawnCtrl dadFlyMeToSpawn;

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

	public void StartFade (Transform player, Vector3 respawnPosition) {
		StartCoroutine (Fade (player, respawnPosition));
	}

	IEnumerator Fade(Transform player, Vector3 respawnPosition) {
		color.a = 0f;
		fadeOut_ToBlack = true;
		fadeIn_FromBlack = false;

		while(fadeOut_ToBlack){
			yield return new WaitForSeconds (0.1f);
		}

		color.a = 1f;
		fadeOut_ToBlack = false;

		FindObjectOfType<MenuControllerInGame> ().TrocaMenus (6); //Tira o menu

		player.GetComponent<PlayerCollisionsCtrl> ().RestartVenenoTimer ();

		camCtrl.ChangeCameraTo (spawnCamIndex);
		StartCoroutine(dadFlyMeToSpawn.ReturnToSpawn (respawnPosition, true));

		yield return new WaitForSeconds (1f);
		fadeIn_FromBlack = true;

		while(fadeIn_FromBlack){
			yield return new WaitForSeconds (0.1f);
		}

		color.a = 0f;
		fadeOut_ToBlack = false;
		fadeIn_FromBlack = false;

		yield return new WaitForSeconds (2f);
		camCtrl.ChangeCameraTo (deadDadCam);

		dadFlyMeToSpawn.actualPai.GetComponentInChildren<Animator> ().SetBool ("isDying", true);
	}
}
