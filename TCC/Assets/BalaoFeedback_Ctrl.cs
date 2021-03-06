﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum balaoTypes
{
	cansado, errou, ouvindo, kiwi, amor
}

public class BalaoFeedback_Ctrl : MonoBehaviour {

	public Sprite cansado, errou, ouvindo, kiwi, amorHug;
	public Sprite[] cura1, cura2, cura3;

	private SpriteRenderer spritePosition;

	// Use this for initialization
	void Start () {
		spritePosition = GetComponent<SpriteRenderer> ();
	}

	public void ShowBalao (balaoTypes type, float waitBeforeClear = 1f){
		CancelInvoke ("HideBalaoFeedback");

		switch (type) {
		case balaoTypes.cansado:
			spritePosition.sprite = cansado;
			break;
		case balaoTypes.errou:
			spritePosition.sprite = errou;
			break;
		case balaoTypes.ouvindo:
			spritePosition.sprite = ouvindo;
			break;
		case balaoTypes.amor:
			spritePosition.sprite = amorHug;
			break;

		default:
			break;
		}

		Invoke ("HideBalaoFeedback", waitBeforeClear);
	}

	public void ShowBalaoCura (int totalDeFrutas, int quantasTem){
		CancelInvoke ("HideBalaoFeedback");

		switch (totalDeFrutas) {
		case 1:
			spritePosition.sprite = cura1[quantasTem];
			break;
		case 2:
			spritePosition.sprite = cura2[quantasTem];
			break;
		case 3:
			spritePosition.sprite = cura3[quantasTem];
			break;

		default:
			break;
		}

		Invoke ("HideBalaoFeedback", 3f);
	}

	public void HideBalaoFeedback (){
		spritePosition.sprite = null;
	}
}
