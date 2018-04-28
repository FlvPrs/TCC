using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonMushroom_Ctrl : MonoBehaviour {

	public float jumpForce = 10f;

	public float venenoInitialRadius = 0.5f;
	public float venenoMaxRadius = 6f;
	public bool startWithVeneno = false;
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

	WalkingController player;
	public GameObject venenoPrefab;

	private Animator mushAnimCtrl;

	private GameObject[] venenos;

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
		}

		if(startWithVeneno){
			System.Random randomNumberGen = new System.Random(GetInstanceID());

			float rndY = randomNumberGen.Next (40, 75) * 0.01f;
			for (int i = 0; i < transform.parent.childCount; i++) {
				transform.parent.GetChild (i).localScale = new Vector3 (transform.parent.GetChild (i).localScale.x, rndY, transform.parent.GetChild (i).localScale.z);
			}

			if (createVenenoOverTime && venenoCanDissipate) {
				StartCoroutine ("SpawnMoreVeneno", randomNumberGen);
			} else {
				for (int i = 0; i < venenos.Length; i++) {
					if(!venenos[i].activeSelf){
						venenos [i].SetActive (true);
						venenos [i].GetComponent<VenenoCtrl> ().ResetVeneno ();
						break;
					} else if (i >= venenos.Length - 1) {
						venenos [0].SetActive (true);
						venenos [0].GetComponent<VenenoCtrl> ().ResetVeneno ();
						break;
					}
				}
			}
		}
	}

	IEnumerator SpawnMoreVeneno (System.Random rndGen){
		yield return new WaitForSeconds (rndGen.Next(20) * 0.1f);

		while (true) {
			for (int i = 0; i < venenos.Length; i++) {
				if(!venenos[i].activeSelf){
					venenos [i].SetActive (true);
					venenos [i].GetComponent<VenenoCtrl> ().ResetVeneno ();
					break;
				}
				else if (i >= venenos.Length - 1) {
					venenos [0].SetActive (true);
					venenos [0].GetComponent<VenenoCtrl> ().ResetVeneno ();
					break;
				}
			}

			yield return new WaitForSeconds (venenoDissipateTimer * 0.5f);
		}
	}

	void OnTriggerEnter(Collider col){
		mushAnimCtrl.SetTrigger ("boing");
		if(col.CompareTag("Player")){
			Vector3 dir = col.transform.up * jumpForce;
			player.AddExternalForce (dir, 0.01f);
			//mushAnimCtrl.SetTrigger ("boing");
			for (int i = 0; i < venenos.Length; i++) {
				if(!venenos[i].activeSelf){
					venenos [i].SetActive (true);
					venenos [i].GetComponent<VenenoCtrl> ().ResetVeneno ();
					break;
				}
				else if (i >= venenos.Length - 1) {
					venenos [0].SetActive (true);
					venenos [0].GetComponent<VenenoCtrl> ().ResetVeneno ();
					break;
				}
			}
		}
	}
}
