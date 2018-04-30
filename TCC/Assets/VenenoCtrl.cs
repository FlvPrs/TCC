using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VenenoCtrl : MonoBehaviour {

	Transform t;
	Rigidbody rb;
	bool startUpdate;

	public float initialRadius = 0.5f;
	public float maxRadius = 2.5f;
	public bool canGrow = true;
	public bool canDissipate = true;
	public float dissipateTimer = 10f;
	float growAmount = 0.5f;
	public bool carveNavMesh = true;

	public bool collideWithWind = true;

	public bool canDisable = false;

	public int yIndex = 0;

	void Awake () {
		t = GetComponent<Transform> ();
		rb = GetComponent<Rigidbody> ();

		GetComponent<UnityEngine.AI.NavMeshObstacle> ().enabled = false;

		Invoke ("StartUpdate", 0.01f);
	}

	void StartUpdate (){
		t.localScale = new Vector3 (initialRadius * 2f, t.localScale.y, initialRadius * 2f);
		if(yIndex != 0)
			t.localPosition = Vector3.up * 5f * yIndex;

		if (canDissipate) {
			//Destroy (gameObject, dissipateTimer);
			Invoke ("DisableVeneno", dissipateTimer);
		}

		if (canGrow)
			growAmount = ((maxRadius - initialRadius) * 0.2f);

		if (carveNavMesh)
			GetComponent<UnityEngine.AI.NavMeshObstacle> ().enabled = true;

		if (!collideWithWind)
			rb.isKinematic = true;

		startUpdate = true;
	}

	void Update () {
		if (!startUpdate)
			return;

		if (canGrow) {
			t.localScale += new Vector3 (growAmount * Time.deltaTime, 0, growAmount * Time.deltaTime);
			if (t.localScale.x >= maxRadius) {
				startUpdate = false;
				if (canDisable)
					enabled = false;
			}
		} else if (canDisable) {
			enabled = false;
		}
	}

	public void DisableVeneno (){
		startUpdate = false;
		gameObject.SetActive (false);
	}

	public void ResetVeneno (){
		rb.velocity = Vector3.zero;
		//t.localPosition = Vector3.zero;
		StartUpdate ();
	}

//	public void MurcharPlanta (GameObject planta){
//		
//	}

	void OnTriggerStay(Collider col){
		if(col.CompareTag("Wind") && collideWithWind){
			AddExternalForce (col.transform.up);
		} else if (col.CompareTag("Wind2") && collideWithWind) {
			AddExternalForce (col.transform.up * 2f);
		} else if (col.GetComponent<PlantaBehaviour>() != null){
			col.GetComponent<PlantaBehaviour> ().MurcharPlanta ();
		}
	}

	void AddExternalForce (Vector3 dir){
		//t.Translate (dir * Time.deltaTime);
		//dir.y = 0f;
		if(rb.velocity.magnitude < 2f)
			rb.velocity += dir * 0.05f;
	}
}
