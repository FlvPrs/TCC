using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuControllerInGame : MonoBehaviour {

	public WalkingController player;
	public int opcaoMenu1, opcaoMenu2, opcaoMenu3, opcaoMenuPause, opcaoMenuMorte, volumeMusica, volumeEfeitos;
	private float tempo;
	private bool onMenu1, onMenu2, onMenu3, onMenu4, onMenuDeath,onPause, controlandoVolume, musicaEfeito, inGame;
	public GameObject menu1, menu2, menu3, menu4, menuPause, menuMorte, telaFundo, seta;
	public GameObject VM1, VM2, VM3, VM4, VM5, VM6, VM7, VM8;
	public GameObject VE1, VE2, VE3, VE4, VE5, VE6, VE7, VE8;
	public static SaveInformations save;

	// Use this for initialization
	void Start () {
		onMenu1 = true;
		TrocaMenus (1);
		opcaoMenu1 = 1;
		onPause = false;

		tempo = Time.fixedDeltaTime;

	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Escape) && inGame) {
			onPause = !onPause;
			chamaPause ();
		}
		#region controleMenuInicial
		if (onMenu1) {//menuInicial
			if (Input.GetKeyDown (KeyCode.DownArrow) && opcaoMenu1 < 4) {
				opcaoMenu1++;
			}
			if (Input.GetKeyDown (KeyCode.UpArrow) && opcaoMenu1 > 1) {
				opcaoMenu1--;
			}
			if (Input.GetKeyDown (KeyCode.KeypadEnter)) {
				if (opcaoMenu1 == 1) {
					TrocaMenus (2);
				} else if (opcaoMenu1 == 2) {
					TrocaMenus (3);
				} else if (opcaoMenu1 == 3) {
					TrocaMenus (4);
				} else if (opcaoMenu1 == 4) {
					Application.Quit ();
				}
			}
		}
		#endregion
		#region MenuSlots
		if (onMenu2) {//EscolheSave
			if (Input.GetKeyDown (KeyCode.LeftArrow) && opcaoMenu2 > 1) {
				opcaoMenu2--;
			}
			if (Input.GetKeyDown (KeyCode.RightArrow) && opcaoMenu2 < 3) {
				opcaoMenu2++;
			}
			if (Input.GetKeyDown (KeyCode.KeypadEnter)) {
				if (opcaoMenu2 == 1) {
					if (SaveInformations.saveSlot1 == 0) {
						SaveInformations.SaveSlot (1, 1);
						TrocaMenus (6);
						print("acionei");
					}
				} else if (opcaoMenu2 == 2) {
					if (SaveInformations.saveSlot2 == 0) {
						SaveInformations.SaveSlot (1, 2);
						TrocaMenus (6);
					}
				} else if (opcaoMenu2 == 3) {
					if (SaveInformations.saveSlot3 == 0) {
						SaveInformations.SaveSlot (1, 3);
						TrocaMenus (6);
					}
				}
			}
			if (Input.GetKeyDown (KeyCode.Escape)) {
				TrocaMenus (1);
			}
		}
		#endregion
		#region MenuVolume
		if (onMenu3) {
			if (Input.GetKeyDown (KeyCode.UpArrow) && opcaoMenu3 > 1) {
				if (!controlandoVolume) {
					opcaoMenu3--;
				}
			}
			if (Input.GetKeyDown (KeyCode.UpArrow) && opcaoMenu3 < 2) {
				if (!controlandoVolume) {
					opcaoMenu3++;
				}
			}
			if (Input.GetKeyDown (KeyCode.KeypadEnter)) {
				if (opcaoMenu3 == 1) {
					controlandoVolume = true;
				}
			}
			if (Input.GetKeyDown (KeyCode.Escape)) {
				if (controlandoVolume) {
					controlandoVolume = false;
					SaveInformations.SaveVolume (volumeMusica, volumeEfeitos);
				} else {
					TrocaMenus (1);
				}
			}
			if (controlandoVolume) {
				if (Input.GetKeyDown (KeyCode.RightArrow))
				if (musicaEfeito && volumeMusica <8) {
					volumeMusica++;
					imagensVolume ();
				} else if (!musicaEfeito && volumeEfeitos <8) {
					volumeEfeitos++;
					imagensVolume ();
				}
				if (Input.GetKeyDown (KeyCode.LeftArrow)) {
					if (musicaEfeito && volumeMusica >1) {
						volumeMusica--;
						imagensVolume ();
					} else if (!musicaEfeito && volumeEfeitos >1) {
						volumeEfeitos--;
						imagensVolume ();
					}
				}
				if (Input.GetKeyDown (KeyCode.UpArrow) || Input.GetKeyDown (KeyCode.DownArrow)) {
					musicaEfeito = !musicaEfeito;
				}

			}
		}
		#endregion
	
//		if (onMenu1) {
//			player.playerCanMove = false;
//		} else {
//			player.playerCanMove = true;
//		}

//		if (onPause) {
//			Time.fixedDeltaTime = 0f;
//		} else {
//			player.playerCanMove = true;
//			Time.fixedDeltaTime = 1f;
//		}

		if (Input.GetKeyDown (KeyCode.T)) {
			onMenu1 = false;
			TrocaMenus (6);
		}
	}


	void imagensVolume(){
		if (volumeMusica == 1) {
			VM1.SetActive (true);VM2.SetActive (false);VM3.SetActive (false);VM4.SetActive (false);
			VM5.SetActive (false);VM6.SetActive (false);VM7.SetActive (false);VM8.SetActive (false);
		}else if (volumeMusica == 2) {
			VM1.SetActive (true);VM2.SetActive (true);VM3.SetActive (false);VM4.SetActive (false);
			VM5.SetActive (false);VM6.SetActive (false);VM7.SetActive (false);VM8.SetActive (false);
		}else if (volumeMusica == 3) {
			VM1.SetActive (true);VM2.SetActive (true);VM3.SetActive (true);VM4.SetActive (false);
			VM5.SetActive (false);VM6.SetActive (false);VM7.SetActive (false);VM8.SetActive (false);
		}else if (volumeMusica == 4) {
			VM1.SetActive (true);VM2.SetActive (true);VM3.SetActive (true);VM4.SetActive (true);
			VM5.SetActive (false);VM6.SetActive (false);VM7.SetActive (false);VM8.SetActive (false);
		}else if (volumeMusica == 5) {
			VM1.SetActive (true);VM2.SetActive (true);VM3.SetActive (true);VM4.SetActive (true);
			VM5.SetActive (true);VM6.SetActive (false);VM7.SetActive (false);VM8.SetActive (false);
		}else if (volumeMusica == 6) {
			VM1.SetActive (true);VM2.SetActive (true);VM3.SetActive (true);VM4.SetActive (true);
			VM5.SetActive (true);VM6.SetActive (true);VM7.SetActive (false);VM8.SetActive (false);
		}else if (volumeMusica == 7) {
			VM1.SetActive (true);VM2.SetActive (true);VM3.SetActive (true);VM4.SetActive (true);
			VM5.SetActive (true);VM6.SetActive (true);VM7.SetActive (true);VM8.SetActive (false);
		}else if (volumeMusica == 8) {
			VM1.SetActive (true);VM2.SetActive (true);VM3.SetActive (true);VM4.SetActive (true);
			VM5.SetActive (true);VM6.SetActive (true);VM7.SetActive (true);VM8.SetActive (true);
		}

		if (volumeEfeitos == 1) {
			VE1.SetActive (true);VE2.SetActive (false);VE3.SetActive (false);VE4.SetActive (false);
			VE5.SetActive (false);VE6.SetActive (false);VE7.SetActive (false);VE8.SetActive (false);
		}else if (volumeEfeitos == 2) {
			VE1.SetActive (true);VE2.SetActive (true);VE3.SetActive (false);VE4.SetActive (false);
			VE5.SetActive (false);VE6.SetActive (false);VE7.SetActive (false);VE8.SetActive (false);
		}else if (volumeEfeitos == 3) {
			VE1.SetActive (true);VE2.SetActive (true);VE3.SetActive (true);VE4.SetActive (false);
			VE5.SetActive (false);VE6.SetActive (false);VE7.SetActive (false);VE8.SetActive (false);
		}else if (volumeEfeitos == 4) {
			VE1.SetActive (true);VE2.SetActive (true);VE3.SetActive (true);VE4.SetActive (true);
			VE5.SetActive (false);VE6.SetActive (false);VE7.SetActive (false);VE8.SetActive (false);
		}else if (volumeEfeitos == 5) {
			VE1.SetActive (true);VE2.SetActive (true);VE3.SetActive (true);VE4.SetActive (true);
			VE5.SetActive (true);VE6.SetActive (false);VE7.SetActive (false);VE8.SetActive (false);
		}else if (volumeEfeitos == 6) {
			VE1.SetActive (true);VE2.SetActive (true);VE3.SetActive (true);VE4.SetActive (true);
			VE5.SetActive (true);VE6.SetActive (true);VE7.SetActive (false);VE8.SetActive (false);
		}else if (volumeEfeitos == 7) {
			VE1.SetActive (true);VE2.SetActive (true);VE3.SetActive (true);VE4.SetActive (true);
			VE5.SetActive (true);VE6.SetActive (true);VE7.SetActive (true);VE8.SetActive (false);
		}else if (volumeEfeitos == 8) {
			VE1.SetActive (true);VE2.SetActive (true);VE3.SetActive (true);VE4.SetActive (true);
			VE5.SetActive (true);VE6.SetActive (true);VE7.SetActive (true);VE8.SetActive (true);
		}

	}

	void chamaPause(){
		if (onPause == true) {
			TrocaMenus (0);
		} else if (onPause == false) {
			TrocaMenus (6);
		}
	}


	public void TrocaMenus(int numeroMenu){
		switch (numeroMenu) {
		case 0:// menuPausa
			
			menu1.SetActive (false);
			menu2.SetActive (false);
			menu3.SetActive (false);
			menu4.SetActive (false);
			menuMorte.SetActive (false);
			menuPause.SetActive (true);
			telaFundo.SetActive (true);

			onPause = true;
			onMenu1 = false;
			onMenu2 = false;
			onMenu3 = false;
			onMenu4 = false;
			onMenuDeath = false;
			inGame = true;

			opcaoMenuPause = 1;
			player.playerCanMove = false;
			//Time.fixedDeltaTime = 0f;
			Time.timeScale = 0f;
			break;

		case 1://menuInicial

			menu1.SetActive (true);
			menu2.SetActive (false);
			menu3.SetActive (false);
			menu4.SetActive (false);
			menuMorte.SetActive (false);
			menuPause.SetActive (false);
			telaFundo.SetActive (true);

			onPause = false;
			onMenu1 = true;
			onMenu2 = false;
			onMenu3 = false;
			onMenu4 = false;
			onMenuDeath = false;
			inGame = false;

			opcaoMenu1 = 1;
			player.playerCanMove = false;
			break;

		case 2://menuEscolherSave

			menu1.SetActive (false);
			menu2.SetActive (true);
			menu3.SetActive (false);
			menu4.SetActive (false);
			menuMorte.SetActive (false);
			menuPause.SetActive (false);
			telaFundo.SetActive (true);

			onPause = false;
			onMenu1 = false;
			onMenu2 = true;
			onMenu3 = false;
			onMenu4 = false;
			onMenuDeath = false;
			inGame = false;

			opcaoMenu2 = 1;
			player.playerCanMove = false;
			break;

		case 3://menuOpçõesConfigurações

			menu1.SetActive (false);
			menu2.SetActive (false);
			menu3.SetActive (true);
			menu4.SetActive (false);
			menuMorte.SetActive (false);
			menuPause.SetActive (false);
			telaFundo.SetActive (true);

			onPause = false;
			onMenu1 = false;
			onMenu2 = false;
			onMenu3 = true;
			onMenu4 = false;
			onMenuDeath = false;
			inGame = false;

			opcaoMenu3 = 1;
			player.playerCanMove = false;
			controlandoVolume = false;
			musicaEfeito = true;
			break;

			case 4://menuCreditos

			menu1.SetActive (false);
			menu2.SetActive (false);
			menu3.SetActive (false);
			menu4.SetActive (true);
			menuMorte.SetActive (false);
			menuPause.SetActive (false);
			telaFundo.SetActive (true);

			onPause = false;
			onMenu1 = false;
			onMenu2 = false;
			onMenu3 = false;
			onMenu4 = true;
			onMenuDeath = false;
			inGame = false;

			player.playerCanMove = false;
			break;

		case 5://menuMorte

			menu1.SetActive (false);
			menu2.SetActive (false);
			menu3.SetActive (false);
			menu4.SetActive (false);
			menuMorte.SetActive (true);
			menuPause.SetActive (false);
			telaFundo.SetActive (true);

			onPause = false;
			onMenu1 = false;
			onMenu2 = false;
			onMenu3 = false;
			onMenu4 = false;
			onMenuDeath = true;
			inGame = false;

			opcaoMenuMorte = 1;
			player.playerCanMove = false;
			break;

		case 6://Nenhum

			menu1.SetActive (false);
			menu2.SetActive (false);
			menu3.SetActive (false);
			menu4.SetActive (false);
			menuMorte.SetActive (false);
			menuPause.SetActive (false);
			telaFundo.SetActive (false);

			onPause = false;
			onMenu1 = false;
			onMenu2 = false;
			onMenu3 = false;
			onMenu4 = false;
			onMenuDeath = false;
			inGame = true;

			player.playerCanMove = true;
			//Time.fixedDeltaTime = tempo;
			Time.timeScale = 1f;
			break;

		}
		
	}
}
