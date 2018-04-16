
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuControllerInGame : MonoBehaviour {

	public WalkingController player;
	public int opcaoMenu1, opcaoMenu2, opcaoMenu3, opcaoMenuPause, opcaoMenuMorte, volumeMusica, volumeEfeitos;
	private float tempo, timeToEnter;
	private bool onMenu1, onMenu2, onMenu3, onMenu4, onMenuDeath,onPause, controlandoVolume, musicaEfeito, inGame;
	private bool podeEnter, jogando;
	public GameObject menu1, menu2, menu3, menu4, menuPause, menuMorte, telaFundo, seta;
	public GameObject VM1, VM2, VM3, VM4, VM5, VM6, VM7, VM8;
	public GameObject VE1, VE2, VE3, VE4, VE5, VE6, VE7, VE8;
	public static SaveInformations save;

	// Use this for initialization
	void Start () {
		TrocaMenus (1);
		onPause = false;
		inGame = false;
		tempo = Time.fixedDeltaTime;
	}
	
	// Update is called once per frame
	void Update () {
		timeToEnter += Time.deltaTime * 1f;
		if (timeToEnter >= 0.3f) {
			podeEnter = true;
		} else {
			podeEnter = false;
		}
		if (Input.GetKeyDown (KeyCode.Return)||Input.GetKeyDown(KeyCode.JoystickButton0)) {
			timeToEnter = 0.0f;
			if (podeEnter) {
				if (onMenu1) {
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
				else if (onMenu2) {
					if (opcaoMenu2 == 1) {
						if (SaveInformations.saveSlot1 == 0) {
							SaveInformations.SaveSlot (1, 1);
							TrocaMenus (6);
							inGame = true;
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
				else if (onMenu3) {
					if (opcaoMenu3 == 1) {
						controlandoVolume = true;
					}
				}
				else if (onPause) {
					if (opcaoMenuPause == 1) {
						TrocaMenus (6);
					} else if (opcaoMenuPause == 2) {
						TrocaMenus (3);
					}else if (opcaoMenuPause == 3) {
						TrocaMenus (4);
					}else if (opcaoMenuPause == 4) {
						TrocaMenus (1);
						inGame = false;
					}else if (opcaoMenuPause == 5) {
						Application.Quit ();
					}
				}
			}
		}
		if(Input.GetKeyDown(KeyCode.UpArrow)){
			if (onMenu1) {
				if (opcaoMenu1 > 1) {
					opcaoMenu1--;
				}
			}
			else if (onMenu3) {
				if (!controlandoVolume) {
					if (opcaoMenu3 > 1) {
						opcaoMenu3--;
					} 
				}else if (controlandoVolume) {
					musicaEfeito = !musicaEfeito;
				}
			}
			else if (onPause) {
				if (opcaoMenuPause > 1) {
					opcaoMenuPause--;
				}
			}
		}
		if (Input.GetKeyDown (KeyCode.DownArrow)) {
			if (onMenu1) {
				if (opcaoMenu1 < 4) {
					opcaoMenu1++;
				}
			}
			else if (onMenu3) {
				if (!controlandoVolume) {
					if (opcaoMenu3 < 2) {
						opcaoMenu3++;
					}
				} else if (controlandoVolume) {
					musicaEfeito = !musicaEfeito;
				}
			}
			else if (onPause) {
				if (opcaoMenuPause < 5) {
					opcaoMenuPause++;
				}
			}
		}
		if(Input.GetKeyDown(KeyCode.LeftArrow)){
			if (onMenu2) {
				if (opcaoMenu2 > 1) {
					opcaoMenu2--;
				}
			}
			else if (onMenu3) {
				if (controlandoVolume) {
					if (musicaEfeito && volumeMusica >1) {
						volumeMusica--;
						imagensVolume ();
					} else if (!musicaEfeito && volumeEfeitos >1) {
						volumeEfeitos--;
						imagensVolume ();
					}
				}
			}
		}
		if (Input.GetKeyDown (KeyCode.RightArrow)) {
			if (onMenu2) {
				if (opcaoMenu2 < 3) {
					opcaoMenu2++;
				}
			}
			else if (onMenu3) {
				if (controlandoVolume) {
					if (musicaEfeito && volumeMusica <8) {
						volumeMusica++;
						imagensVolume ();
					} else if (!musicaEfeito && volumeEfeitos <8) {
						volumeEfeitos++;
						imagensVolume ();
					}
				}
			}
		}
		if (Input.GetKeyDown (KeyCode.Escape)||Input.GetKeyDown(KeyCode.JoystickButton1)) {
			if(onMenu2){
				TrocaMenus (1);

			}
			else if (onMenu3) {
				if (controlandoVolume) {
					controlandoVolume = false;
					SaveInformations.SaveVolume (volumeMusica, volumeEfeitos);
				} else if (!inGame) {
					TrocaMenus (1);
				} else if (inGame) {
					TrocaMenus (0);
				}
			}
			else if (onMenu4) {
				if (inGame) {
					TrocaMenus (0);
				} else if (!inGame) {
					TrocaMenus (1);
				}
			}
		}
		if (Input.GetKeyDown (KeyCode.JoystickButton7)) {
			if (inGame) {
				if (onPause) {
					TrocaMenus (6);
					print ("jogando");
				}
				else if (jogando) {
					TrocaMenus (0);
					print ("pause");
				}
			}
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
			jogando = false;
			//inGame = true;

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
			jogando = false;
			//inGame = false;

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
			jogando = false;
			//inGame = false;

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
			jogando = false;
			//inGame = false;

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
			jogando = false;
			//inGame = false;

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
			jogando = false;
			//inGame = false;

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
			jogando = true;
			//inGame = true;

			player.playerCanMove = true;
			//Time.fixedDeltaTime = tempo;
			Time.timeScale = 1f;
			break;

		}
		
	}
}
