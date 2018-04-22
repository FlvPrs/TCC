using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VenenoCtrl : MonoBehaviour {

	Transform t;
	bool startUpdate;

	public float initialRadius = 0.5f;
	public float maxRadius = 2.5f;
	public bool canGrow = true;
	public bool canDissipate = true;
	float growAmount = 0.5f;

	void Awake () {
		t = GetComponent<Transform> ();

		Invoke ("StartUpdate", 0.01f);
	}

	void StartUpdate (){
		t.localScale = new Vector3 (initialRadius * 2f, t.localScale.y, initialRadius * 2f);

		if(canDissipate)
			Destroy (gameObject, 10f);

		if (canGrow)
			growAmount = ((maxRadius - initialRadius) * 0.2f);

		startUpdate = true;
	}

	void Update () {
		if (!startUpdate)
			return;

		if (canGrow) {
			t.localScale += new Vector3 (growAmount * Time.deltaTime, 0, growAmount * Time.deltaTime);
			if (t.localScale.x >= maxRadius)
				startUpdate = false;
		}
	}

	void OnTriggerStay(Collider col){
		if(col.CompareTag("Wind")){
			AddExternalForce (col.transform.up);
		} else if (col.CompareTag("Wind2")) {
			AddExternalForce (col.transform.up * 2f);
		}
	}

	void AddExternalForce (Vector3 dir){
		t.Translate (dir * Time.deltaTime);
	}
}
