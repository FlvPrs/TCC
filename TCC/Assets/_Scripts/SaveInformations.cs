using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveInformations : MonoBehaviour {
	public static SaveInformations instance;
	public static int saveSlot1, saveSlot2, saveSlot3, volumeMusica, volumeEfeitos;
	// Use this for initialization
	void Awake() 
	{
		if(instance == null)
		{
			instance = this;
		}
		else
		{
			if (this != instance) {
				Destroy (this.gameObject);
			} else {
				DontDestroyOnLoad (instance);
			}
		}
	}

	public static void SaveSlot(int fase, int slot) {
		if (slot == 1) {
			PlayerPrefs.SetInt ("saveSlot1", fase);
		} else if (slot == 2) {
			PlayerPrefs.SetInt ("saveSlot2", fase);
		} else if (slot == 3) {
			PlayerPrefs.SetInt ("saveSlot3", fase);
		}
		PlayerPrefs.Save ();
	}
	public static void SaveVolume(int musica, int efeito){
		PlayerPrefs.SetInt ("volumeMusica", musica);
		PlayerPrefs.SetInt ("volumeEfeito", efeito);
		PlayerPrefs.Save ();
	}
}
