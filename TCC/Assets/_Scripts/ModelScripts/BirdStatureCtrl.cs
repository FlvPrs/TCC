﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class BirdStatureCtrl : MonoBehaviour {

	public AudioMixerSnapshot clarinetDefault;
	public AudioMixerSnapshot clarinetLow;
	public AudioMixerSnapshot clarinetHigh;

	[Range(0.1f, 0.9f)]
	public float minStature = 0.5f;
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
	public BoxCollider coll;

	void Awake(){
		currentHeightState = HeightState.Default;
		t = GetComponent<Transform> ();
		//coll = GetComponent<BoxCollider> ();
		currentHeight = defaultHeight;
		currentSize = defaultHeight;
		maxDifference = maxStature - defaultHeight;
		minDifference = defaultHeight - minStature;
	}

	void Update(){
		Vector3 newScale = t.localScale;
		newScale.y = currentHeight * 2;
		newScale.x = newScale.z = currentSize;
		coll.size = newScale;
		coll.center = new Vector3(coll.center.x, currentHeight, coll.center.z);

		//t.localScale = newScale;
		if (coll.size.y >= (defaultHeight * 2f) + maxDifference / 2f) {
			currentHeightState = HeightState.High;
			clarinetHigh.TransitionTo (0.01f);
		}
		else if (coll.size.y <= (defaultHeight * 2f) - minDifference / 2f) {
			currentHeightState = HeightState.Low;
			clarinetLow.TransitionTo (0.01f);
		} 
		else {
			currentHeightState = HeightState.Default;
			clarinetDefault.TransitionTo (0.01f);
		}
	}

	public void UpdateHeight(float strength, Animator anim){
		anim.SetFloat ("Stature", strength);
		anim.SetLayerWeight (1, Mathf.Clamp(Mathf.Abs(strength), 0f, 0.8f));
		anim.SetLayerWeight (2, Mathf.Clamp(Mathf.Abs(strength), 0f, 0.8f));

		if(strength == 0f){
			currentHeight = defaultHeight;
			currentSize = defaultHeight;
			currentHeightState = HeightState.Default;
			clarinetDefault.TransitionTo (0.01f);
			return;
		}
		//return;

		float tempStrength = -strength;

		currentHeight = defaultHeight;
		currentHeight += (((Mathf.Abs (strength) + tempStrength) / -2) * minDifference) + (((Mathf.Abs (strength) - tempStrength) /2) * maxDifference);
		currentSize = defaultHeight;
		currentSize -= (((Mathf.Abs (strength) + tempStrength) / -2) * minDifference) + (((Mathf.Abs (strength) - tempStrength) /2) * maxDifference);
	}
}

public enum HeightState {
	Default,
	High,
	Low
}