using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControleMenu : MonoBehaviour {
	private float timerToSkip;
	private int numeroImagem;
	public GameObject BBLogo, Cimologo, Joystick, aviso1, aviso2;
	// Use this for initialization
	void Start () {
		numeroImagem = 1;
	}
	
	// Update is called once per frame
	void Update () {
		timerToSkip += Time.deltaTime * 1;
		if (timerToSkip >= 7f) {
				numeroImagem++;
				timerToSkip = 0f;
		}

		if (numeroImagem == 1) {
			BBLogo.SetActive (true);
			Cimologo.SetActive (false);
			Joystick.SetActive (false);
			aviso1.SetActive (false);
			aviso2.SetActive (false);
		} else if (numeroImagem == 2) {
			BBLogo.SetActive (false);
			Cimologo.SetActive (true);
			Joystick.SetActive (false);
			aviso1.SetActive (false);
			aviso2.SetActive (false);
		}else if (numeroImagem == 3) {
			BBLogo.SetActive (false);
			Cimologo.SetActive (true);
			Joystick.SetActive (true);
			aviso1.SetActive (true);
			aviso2.SetActive (false);
		}else if (numeroImagem == 4) {
			BBLogo.SetActive (false);
			Cimologo.SetActive (true);
			Joystick.SetActive (false);
			aviso1.SetActive (false);
			aviso2.SetActive (true);
		}
		if (numeroImagem == 5) {
			SceneManager.LoadScene (2);
		}

	}

}
