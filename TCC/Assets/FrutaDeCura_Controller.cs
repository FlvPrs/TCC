using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FrutaDeCura_Controller : MonoBehaviour, ICarnivoraEdible {

	Rigidbody rb;

	public Vector3 forceDir;

	void Awake (){
		rb = GetComponent<Rigidbody> ();
		rb.useGravity = false;
		rb.isKinematic = true;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void CairDaPlanta (){
		if (forceDir.magnitude == 0f)
			forceDir = Vector3.forward;
		rb.useGravity = true;
		rb.isKinematic = false;
		rb.AddForce (forceDir.normalized * 60f);
		Invoke ("Freeze", 2f);
	}

	void Freeze (){
		rb.velocity = Vector3.zero;
		rb.useGravity = false;
		rb.isKinematic = true;
	}


	#region Carnivora Interface
	public void Carnivora_GetReadyToBeEaten (){
		
	}
	public void Carnivora_Release (){
		
	}
	public void Carnivora_Shoot (Vector3 dir){
		
	}
	#endregion

	void OnTriggerEnter (Collider col){
		if (col.CompareTag("PaiDebilitado")) {
			col.GetComponent<Father_DebilitadoCtrl> ().Revigorar (gameObject);
		} else if (col.GetComponent<PlantaBehaviour>() != null) {
			col.GetComponent<PlantaBehaviour> ().RevigorarPlanta (gameObject);
		}
	}
}
