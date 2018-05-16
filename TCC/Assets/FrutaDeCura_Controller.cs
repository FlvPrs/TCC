using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FrutaDeCura_Controller : MonoBehaviour, ICarnivoraEdible {

	[System.Serializable]
	public struct FrutasClips
	{
		public AudioClip frutaFX_Clip;
		public AudioClip frutaGrounded_Clip;
	}

	Transform t;
	Rigidbody rb;
	AudioSource simpleAudioSource;

	public LayerMask raycastMask = -1;
	public Vector3 forceDir;
	public FrutasClips[] frutas_Clips;

	bool ploft = false;
	bool canFreeze = false;
	int myIndex = 0;

	void Awake (){
		t = GetComponent<Transform> ();

		rb = GetComponent<Rigidbody> ();
		rb.useGravity = false;
		rb.isKinematic = true;
	}

	// Use this for initialization
	void Start () {
		simpleAudioSource = GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (canFreeze) {
			bool hitSomething = Physics.Raycast (t.position, Vector3.down, 0.7f, raycastMask);
			//Debug.DrawRay (t.position, Vector3.down * 0.7f, Color.red);

			if(hitSomething && !ploft){
				ploft = true;
				simpleAudioSource.clip = frutas_Clips[myIndex].frutaGrounded_Clip;
				simpleAudioSource.Play ();

				rb.velocity = Vector3.zero;
				rb.useGravity = false;
				rb.isKinematic = true;
				//canFreeze = false;
			} else if (!hitSomething){
				ploft = false;
				rb.useGravity = true;
				rb.isKinematic = false;
			}
		}
	}

	public void Freeze (){
		rb.useGravity = false;
		rb.isKinematic = true;
		rb.velocity = Vector3.zero;
		canFreeze = false;
	}
	public void UnFreeze (){
		canFreeze = true;
	}

	public void CairDaPlanta (int index){
		if (forceDir.magnitude == 0f)
			forceDir = Vector3.forward;
		rb.useGravity = true;
		rb.isKinematic = false;
		rb.AddForce (forceDir.normalized * 90f);
		canFreeze = true;

		myIndex = index;
		simpleAudioSource.clip = frutas_Clips[myIndex].frutaFX_Clip;
		simpleAudioSource.Play ();
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
		if (col.GetComponent<Father_DebilitadoCtrl> () != null) {
			col.GetComponent<Father_DebilitadoCtrl> ().Revigorar (gameObject);
		} else if (col.GetComponent<PlantaBehaviour>() != null) {
			col.GetComponent<PlantaBehaviour> ().RevigorarPlanta (gameObject);
		}
	}
}
