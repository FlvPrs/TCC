using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerFatherFalling : MonoBehaviour {

	[Tooltip("Requires another object with this set to false in order to reset")]
	public bool toFall = true;
	public bool fallWithThis = false;
	public Transform destination;
	public bool ignoreDestY = true;

	public bool snapXZ_OnStart = false;

	private FatherActions father;
	private Transform father_T;

	private bool interacted = false;
	private bool startMove = false;

	Vector3 originalPos;
	float currentLerpTime;
	public float secondsToArriveOnDestination = 1.5f; //O nome é tempo, mas na verdade esta var armazenará a velocidade.


	// Use this for initialization
	void Start () {
		father = FindObjectOfType<FatherActions> ();
		father_T = father.GetComponent<Transform> ();
	}

//	public IEnumerator StartFatherFall (){
//		if(snapXZ_OnStart && destination != null){
//			father.transform.position = new Vector3 (destination.position.x, father.transform.position.y, destination.position.z);
//		}
//
//		if(father.hugging)
//			father.StopHug ();
//		father.moveSpeed *= 2f;
//		yield return new WaitForSeconds (delay);
//		father.IsFalling (!toFall, (destination != null), (destination != null) ? destination.position : Vector3.zero);
//	}

	void Update (){
		if (!startMove)
			return;

		currentLerpTime += Time.deltaTime;
		if (currentLerpTime > secondsToArriveOnDestination) {
			currentLerpTime = secondsToArriveOnDestination;
		}
		float perc = currentLerpTime / secondsToArriveOnDestination;
		father_T.position = Vector3.Lerp (originalPos, (ignoreDestY) ? new Vector3 (destination.position.x, father.transform.position.y, destination.position.z) : destination.position, perc);

		father_T.rotation = Quaternion.Slerp(father_T.rotation, Quaternion.LookRotation((destination.position - father_T.position), Vector3.up), Time.deltaTime);
		father_T.eulerAngles = new Vector3(0, father_T.eulerAngles.y, 0);

		if (perc == 1f) {
			father.RemoteForceAnimBool ((ignoreDestY) ? "isWalking" : "isFalling", false);
			enabled = false;
		} else {
			father.RemoteForceAnimBool ((ignoreDestY) ? "isWalking" : "isFalling", true);
		}
	}

	void OnTriggerEnter (Collider col){
		if (interacted)
			return;
		
		if(col.GetComponent<FatherActions>()){
			//StartCoroutine (StartFatherFall ());
			//enabled = false;
			if (father.hugging) {
				father.StopHug (false);
			}
			father.IsFalling (!toFall, fallWithThis, transform);

			originalPos = father_T.position;
			if(destination != null){
				if(snapXZ_OnStart){
					father.transform.position = new Vector3 (destination.position.x, father.transform.position.y, destination.position.z);
				}
				startMove = true;
			}
		}
	}
}
