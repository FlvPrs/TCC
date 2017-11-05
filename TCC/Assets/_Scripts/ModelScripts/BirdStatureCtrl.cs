using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

//This is a temporary solution.
//Not usable when final character is finally implemented.
//Instead of Scale, then we'll use actual animation.
public class BirdStatureCtrl : MonoBehaviour {

	public AudioMixerSnapshot clarinetDefault;
	public AudioMixerSnapshot clarinetLow;
	public AudioMixerSnapshot clarinetHigh;

	[Range(0.1f, 0.9f)]
	public float minStature = 0.55f;
	[Range(1.1f, 2f)]
	public float maxStature = 2f;

	[HideInInspector]
	public HeightState currentHeightState;

	float defaultHeight = 1f;
	float currentHeight;
	float currentSize;
	float minDifference;
	float maxDifference;

	private Transform t;

	void Awake(){
		currentHeightState = HeightState.Default;
		t = GetComponent<Transform> ();
		currentHeight = defaultHeight;
		currentSize = defaultHeight;
		maxDifference = maxStature - defaultHeight;
		minDifference = defaultHeight - minStature;
	}

	void Update(){
		Vector3 newScale = t.localScale;
		newScale.y = currentHeight;
		newScale.x = newScale.z = currentSize;
		t.localScale = newScale;
		if (t.localScale.y >= defaultHeight + maxDifference / 2f) {
			currentHeightState = HeightState.High;
			clarinetHigh.TransitionTo (0.01f);
		}
		else if (t.localScale.y <= defaultHeight - minDifference / 2f) {
			currentHeightState = HeightState.Low;
			clarinetLow.TransitionTo (0.01f);
		} 
		else {
			currentHeightState = HeightState.Default;
			clarinetDefault.TransitionTo (0.01f);
		}
	}

	public void UpdateHeight(float strength){
		if(strength == 0f){
			currentHeight = defaultHeight;
			currentSize = defaultHeight;
			currentHeightState = HeightState.Default;
			clarinetDefault.TransitionTo (0.01f);
			return;
		}
		float tempStrength = -strength;

		currentHeight = defaultHeight;
		currentHeight += (((Mathf.Abs (strength) + tempStrength) / -2) * minDifference) + (((Mathf.Abs (strength) - tempStrength) /2) * maxDifference);
		currentSize = defaultHeight;
		currentSize -= (((Mathf.Abs (strength) + tempStrength) / -2) * minDifference) + (((Mathf.Abs (strength) - tempStrength) /2) * maxDifference * 0.65f) * 0.75f;
	}
}
