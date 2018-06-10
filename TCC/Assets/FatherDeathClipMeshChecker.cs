using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FatherDeathClipMeshChecker : MonoBehaviour {

	public Animator animCtrl;
	public Transform MDL_BirdoPai;

	public LayerMask layerMask = 0;

	void OnTriggerStay (Collider col){
		if ((layerMask.value & 1 << col.gameObject.layer) != 0)
			return;
		
		if(col.CompareTag("Untagged")){
			if(animCtrl.GetBool("isDying")){
				MDL_BirdoPai.Rotate (Vector3.left * 0.1f, Space.Self);
			}
		}
	}
}
