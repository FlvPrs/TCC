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

	public IEnumerator ReturnToSpawn(Vector3 pos){
		player.GetComponent<WalkingController> ().SetVelocityTo (Vector3.zero, true);
		fatherOldPos = actualPai.position;
		actualPai.position = paiRespawnPoint.position;
		paiCanShow = false;
		actualPai.GetComponent<UnityEngine.AI.NavMeshAgent> ().enabled = false;
		actualPai.GetComponent<FatherFSM> ().StartRespawn ();
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
		actualPai.GetComponent<UnityEngine.AI.NavMeshAgent> ().enabled = true;
		player.GetComponent<WalkingController> ().SetVelocityTo (Vector3.zero, false);
		yield return new WaitForSeconds (1f);
		paiCanShow = true;
		actualPai.GetComponent<FatherFSM> ().ReturnToPosAfterRespawn (fatherOldPos);
		yield return new WaitForSeconds (3f);
		flyingPai.SetActive (false);
		goUp = false;
	}

	public void ReturnToSpawnAlone(Vector3 pos){
		player.GetComponent<WalkingController> ().SetVelocityTo (Vector3.zero, false);
		player.position = pos + Vector3.up * 2f;
	}
}
