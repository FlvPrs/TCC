using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRespawnCtrl : MonoBehaviour {

	public Transform player;
	public Animator anim;

	public GameObject flyingPai;

	public Transform actualPai;
	public Transform paiRespawnPoint;
	private Vector3 fatherOldPos;

	public bool fatherRetunsPlayer = true;
	public bool fatherReturnsAlone = false;
	public Transform[] FatherSpawnPoints;

	private bool isReturning;
	private bool goUp;
	private bool goDown;
	private bool paiCanShow;

	// Use this for initialization
	void Start () {
		if (fatherRetunsPlayer) {
			anim.SetBool ("IsReturningPlayer", true);
			flyingPai.SetActive (false);
			paiCanShow = true;
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (fatherRetunsPlayer) {
			if (isReturning) {
				player.localPosition = Vector3.zero;
			}
			if (!paiCanShow) {
				actualPai.position = paiRespawnPoint.position;
			}
			if (goUp) {
				transform.Translate (Vector3.up * 0.5f);
			}
			if (goDown) {
				transform.Translate (Vector3.down * 0.1f);
			}
		}
	}

	public IEnumerator ReturnToSpawn(Vector3 pos, bool isFatherSuiciding = false){
		player.GetComponent<WalkingController> ().SetVelocityTo (Vector3.zero, true);
		fatherOldPos = actualPai.position;
		actualPai.position = paiRespawnPoint.position;
		paiCanShow = false;
		if (!isFatherSuiciding) {
			actualPai.GetComponent<UnityEngine.AI.NavMeshAgent> ().enabled = false;
			actualPai.GetComponent<FatherFSM> ().StartRespawn ();
		} else {
			Renderer rend = actualPai.GetChild(1).GetChild(0).GetComponent<Renderer>();
			rend.material.SetColor ("_Color", new Color (0.392f, 0.862f, 0.862f));
			actualPai.GetChild(1).Find("l_Asa").gameObject.SetActive(false);
			actualPai.GetChild(1).Find("r_Asa").gameObject.SetActive(false);
			actualPai.GetChild (1).GetComponent<Animator> ().SetBool ("isDying", true);
			Quaternion deadRotation = actualPai.rotation;
			deadRotation.eulerAngles = new Vector3 (-21.463f, 5.437f, -49.114f);
			actualPai.rotation = deadRotation;
		}
		isReturning = true;
		flyingPai.SetActive (true);
		player.SetParent (transform);
		transform.position = pos + Vector3.up * 15f;
		player.localPosition = Vector3.zero;
		goDown = true;
		yield return new WaitForSeconds (2f);
		player.SetParent (null);
		goDown = false;
		goUp = true;
		isReturning = false;
		if (!isFatherSuiciding) {
			actualPai.GetComponent<UnityEngine.AI.NavMeshAgent> ().enabled = true;
		}
		player.GetComponent<WalkingController> ().SetVelocityTo (Vector3.zero, false);
		yield return new WaitForSeconds (1f);
		paiCanShow = true;
		if (!isFatherSuiciding) {
			actualPai.GetComponent<FatherFSM> ().ReturnToPosAfterRespawn (fatherOldPos);
			yield return new WaitForSeconds (3f);
		}
		flyingPai.SetActive (false);
		goUp = false;
	}

	public void ReturnToSpawnAlone(Vector3 pos){
		player.GetComponent<WalkingController> ().SetVelocityTo (Vector3.zero, false);
		player.position = pos + Vector3.up * 2f;
		if(fatherReturnsAlone){
			StartCoroutine (actualPai.GetComponent<FatherFSM> ().RespawnAlone (FatherSpawnPoints[player.GetComponent<PlayerCollisionsCtrl>().currentSpawnPoint].position));
		}
		actualPai.GetComponent<FatherActions> ().StopHug ();
	}
}
