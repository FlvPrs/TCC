using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayAudioOnRaycastHit : MonoBehaviour {

	public LayerMask raycastMask = -1;

	[Tooltip("If more than one, it will cycle.")]
	public AudioClip[] audio_Clips;

	[Tooltip("Optional. If null, direction will default to Vector3.down")]
	public Transform direction;

	public float rayLength = 1f;

	AudioSource simpleAudioSource;
	Transform t;

	int currentClip = 0;
	Vector3 dir;
	bool canPlay = false;

	// Use this for initialization
	void Start () {
		simpleAudioSource = GetComponent<AudioSource> ();
		t = GetComponent<Transform> ();

		dir = (direction != null) ? direction.position : Vector3.down;
	}
	
	// Update is called once per frame
	void Update () {
		bool hitSomething = Physics.Raycast (t.position, dir, rayLength, raycastMask);
		Debug.DrawRay (t.position, dir * rayLength, Color.red);

		if (hitSomething && canPlay) {
			canPlay = false;
			if(currentClip >= audio_Clips.Length){
				currentClip = 0;
			}
			simpleAudioSource.clip = audio_Clips [currentClip++];
			simpleAudioSource.Play ();
		} else if (!hitSomething) {
			canPlay = true;
		}
	}
}
