using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum HeightState {
	High,
	Default,
	Low
}

//This is a temporary solution.
//Not usable when final character is finally implemented.
//Instead of Scale, then we'll use actual animation.
public class BirdStatureCtrl : MonoBehaviour {

	public AudioMixerSnapshot clarinetDefault;
	public AudioMixerSnapshot clarinetLow;
	public AudioMixerSnapshot clarinetHigh;
	public Gradient hpColor;

	private AudioSource clarinet;

	private Material playerMat;

	[Range(0.1f, 0.9f)]
	public float minStature = 0.55f;
	[Range(1.1f, 2f)]
	public float maxStature = 2f;

	[HideInInspector]
	public HeightState currentState;

	float defaultHeight = 1f;
	float currentHeight;
	float currentSize;
	float minDifference;
	float maxDifference;

	float currentAir = 5f;
	float maxAir = 10f;

	private Transform t;

	void Awake(){
		currentState = HeightState.Default;
		t = GetComponent<Transform> ();
		clarinet = GetComponent<AudioSource> ();
		currentHeight = defaultHeight;
		currentSize = defaultHeight;
		maxDifference = maxStature - defaultHeight;
		minDifference = defaultHeight - minStature;

		playerMat = GetComponentInChildren<MeshRenderer> ().material;
	}

	void Update(){
		Vector3 newScale = t.localScale;
		newScale.y = currentHeight;
		newScale.x = newScale.z = currentSize;
		t.localScale = newScale;
		if (t.localScale.y >= maxStature - 0.2f * maxStature) {
			currentState = HeightState.High;
			clarinetHigh.TransitionTo (0.01f);
		}
		if (t.localScale.y <= minStature + 0.2f * minStature) {
			currentState = HeightState.Low;
			clarinetLow.TransitionTo (0.01f);
		}

		if(clarinet.isPlaying && currentAir > 0){
			currentAir -= Time.deltaTime;
		} else if(currentAir < maxAir/2f) {
			currentAir += Time.deltaTime;
		}
		UpdateColor ();
	}

	public void UpdateHeight(float strength){
		if(strength == 0f){
			currentHeight = defaultHeight;
			currentSize = defaultHeight;
			currentState = HeightState.Default;
			clarinetDefault.TransitionTo (0.01f);
			return;
		}
		float tempStrength = -strength;

		currentHeight = defaultHeight;
		currentHeight += (((Mathf.Abs (strength) + tempStrength) / -2) * minDifference) + (((Mathf.Abs (strength) - tempStrength) /2) * maxDifference);
		currentSize = defaultHeight;
		currentSize -= (((Mathf.Abs (strength) + tempStrength) / -2) * minDifference) + (((Mathf.Abs (strength) - tempStrength) /2) * maxDifference * 0.65f) * 0.75f;
	}

	public void StartClarinet(bool start, float volume){
		if (start && currentAir >= maxAir/2f) {
			clarinet.Play ();
		} else {
			clarinet.Stop ();
		}
		
		UpdateSoundVolume (volume);
	}

	public void UpdateSoundVolume(float volume){
		clarinet.volume = volume;
	}

	public void UpdateColor(){
		playerMat.color = hpColor.Evaluate (currentAir / maxAir);
	}
}
