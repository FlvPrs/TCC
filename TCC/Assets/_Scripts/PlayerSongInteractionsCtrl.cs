using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSongInteractionsCtrl : MonoBehaviour {

	public LayerMask interactLayers;
	public Collider[] hitColliders;

	void Update (){
		Physics.OverlapSphereNonAlloc(transform.position, 10, hitColliders, interactLayers);
	}
}
