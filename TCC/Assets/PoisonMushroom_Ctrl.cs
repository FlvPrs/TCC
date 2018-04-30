using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonMushroom_Ctrl : MonoBehaviour {

	public float jumpForce = 10f;

	public float venenoInitialRadius = 0.5f;
	public float venenoMaxRadius = 6f;
	public bool startWithVeneno = false;
	[Range(1, 4)]
	public int numberOfVenenos = 1;
	public bool venenoCanGrow = true;
	public bool venenoCanDissipate = true;
	[Range(1f, 20f)]
	public float venenoDissipateTimer = 10f;
	public bool venenoCarvesNavMesh = true;

	/// <summary>
	/// Only relevant if startWithVeneno and VenenoCanDissipate are true.
	/// </summary>
	public bool createVenenoOverTime = false;

	public bool venenoCollidesWithWind = true;

	/// <summary>
	/// If there's more than one venenoPivot child, place them upwards.
	/// </summary>
	public bool spreadUp = false;

	public bool alwaysBoing = false;

	WalkingController player;
	public GameObject venenoPrefab;

	private Animator mushAnimCtrl;

	private GameObject[] venenos;

	bool canUpdate = true;

	void Awake(){
		mushAnimCtrl = GetComponentInParent<Animator> ();
		player = FindObjectOfType<WalkingController> ();

		Transform venenosGroup = transform.Find ("Venenos");
		venenos = new GameObject[venenosGroup.childCount];
		for (int i = 0; i < venenosGroup.childCount; i++) {
			venenos [i] = venenosGroup.GetChild (i).gameObject;
			venenos [i].SetActive (false);
			VenenoCtrl objCtrl = venenos [i].GetComponent<VenenoCtrl> ();
			objCtrl.collideWithWind = venenoCollidesWithWind;
			objCtrl.canDissipate = venenoCanDissipate;
			objCtrl.dissipateTimer = venenoDissipateTimer;
			objCtrl.canGrow = venenoCanGrow;
			objCtrl.initialRadius = venenoInitialRadius;
			objCtrl.maxRadius = venenoMaxRadius;
			objCtrl.carveNavMesh = venenoCarvesNavMesh;
			if (spreadUp)
				objCtrl.yIndex = i;
			if (!venenoCollidesWithWind && !venenoCanDissipate)
				objCtrl.canDisable = true;
		}

		canUpdate = false;
		float delay = Random.Range (0f, 4f);

		if(startWithVeneno){
			//System.Random randomNumberGen = new System.Random(GetInstanceID());

//			float rndY = randomNumberGen.Next (40, 75) * 0.01f;
//			for (int i = 0; i < transform.parent.childCount; i++) {
//				transform.parent.localScale = new Vector3 (transform.parent.localScale.x, rndY, transform.parent.localScale.z);
//			}

			//float delay = randomNumberGen.Next (40) * 0.1f;
			if (createVenenoOverTime && venenoCanDissipate) {
				StartCoroutine ("SpawnMoreVeneno", delay);
			} else {
				StartCoroutine ("AwakeVeneno", delay);
			}
		} else {
			StartCoroutine ("DelayUpdate", delay);
		}
	}

	void Update (){
		if(alwaysBoing && canUpdate)
			mushAnimCtrl.SetTrigger ("boing");
	}

	IEnumerator DelayUpdate (float delay){
		yield return new WaitForSeconds (delay);
		canUpdate = true;
	}

	IEnumerator AwakeVeneno (float delay){
		yield return new WaitForSeconds (delay);
		canUpdate = true;
		SpawnVeneno (numberOfVenenos);
	}

	IEnumerator SpawnMoreVeneno (float delay){
		yield return new WaitForSeconds (delay);
		canUpdate = true;

		while (true) {
			SpawnVeneno ();

			yield return new WaitForSeconds (venenoDissipateTimer / numberOfVenenos);
		}
	}

	void SpawnVeneno (int amountAtOnce = 1){
		for (int i = 0; i < venenos.Length; i++) {
			if(!venenos[i].activeSelf){
				venenos [i].SetActive (true);
				venenos [i].GetComponent<VenenoCtrl> ().ResetVeneno ();
				if(i + 1 >= amountAtOnce)
					break;
			}
//			else if (i >= venenos.Length - 1) {
//				venenos [0].SetActive (true);
//				venenos [0].GetComponent<VenenoCtrl> ().ResetVeneno ();
//				break;
//			}
		}
	}

	void OnTriggerEnter(Collider col){
		if(col.CompareTag("Player")){
			mushAnimCtrl.SetTrigger ("boing");
			Vector3 dir = col.transform.up * jumpForce;
			player.AddExternalForce (dir, 0.01f);
			SpawnVeneno ();
		}
	}
}
