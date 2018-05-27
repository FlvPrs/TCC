using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveInformations : MonoBehaviour {
	public static SaveInformations instance;
	public static int saveSlot1, saveSlot2, saveSlot3, volumeMusicaS, volumeEfeitosS, volumeAlterado;
	public static int SlotAtual;

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

	void Update(){
		//print (SlotAtual);
		if (Input.GetKeyDown (KeyCode.J)) {
			SceneManager.LoadScene (2);
		}
		if (Input.GetKeyDown (KeyCode.K)) {
			SceneManager.LoadScene (3);
		}
		if (Input.GetKeyDown (KeyCode.L)) {
			SceneManager.LoadScene (4);
		}
		if (Input.GetKeyDown (KeyCode.H)) {
			SceneManager.LoadScene (1);
		}
		//print ("leluia");

	}

	public static void SaveSlot(int slot, int fase ) {
		if (slot == 0) {
			return;
		} else if (slot == 1) {
			PlayerPrefs.SetInt ("saveSlot1", fase);
		} else if (slot == 2) {
			PlayerPrefs.SetInt ("saveSlot2", fase);
		} else if (slot == 3) {
			PlayerPrefs.SetInt ("saveSlot3", fase);
		}
		PlayerPrefs.Save ();
	}
	public static void SaveVolume(int musica, int efeito){
		PlayerPrefs.SetInt ("volumeMusicaS", musica);
		PlayerPrefs.SetInt ("volumeEfeitoS", efeito);
		PlayerPrefs.Save ();
	}

	public static void VolumeAlteradoF(){
		PlayerPrefs.SetInt ("volumeAlterado", 2);
	}

	public static void EscolhendoSlot(int slot){
		SlotAtual = slot;
	}

	public static void SalvaCena(int faseAtual){
		if (faseAtual != 1) {
			SaveSlot (SlotAtual, faseAtual);
		}
	}


}
