
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuControllerInGame : MonoBehaviour {

	public WalkingController player;
	public int opcaoMenu1, opcaoMenu2, opcaoMenu3, opcaoMenuPause, opcaoMenuMorte, volumeMusica, volumeEfeitos;
	public int saveSlot1, saveSlot2, saveSlot3, volumeAlterado;
	private bool onMenu1, onMenu2, onMenu3, onMenu4, onMenu5, onMenuDeath,onPause, controlandoVolume, musicaEfeito, inGame;
	private bool podeEnter, jogando, controleOpcaoEixo, deleteSave;
	public GameObject menu1, menu2, menu3, menu4, menu5, menuPause, menuMorte, telaFundo, seta;
	public GameObject VM1, VM2, VM3, VM4, VM5, VM6, VM7, VM8;
	public GameObject VE1, VE2, VE3, VE4, VE5, VE6, VE7, VE8;
	public static SaveInformations save;

	// Use this for initialization
	void Start () {
		GetSaveVariables ();
		if (volumeAlterado != 2) {
			volumeMusica = 8;
			volumeEfeitos = 8;
		}

		imagensVolume ();
		TrocaMenus (1);
		onPause = false;
		inGame = false;

		podeEnter = true;
		controleOpcaoEixo = true;
	}
	
	// Update is called once per frame
	void Update () {
		
		if (Input.GetKeyDown (KeyCode.Return)||Input.GetKeyDown(KeyCode.JoystickButton0)) {
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
				} else if (onMenu2) {
					if (!deleteSave) {
						if (opcaoMenu2 == 1) {
							if (saveSlot1 == 0) {
								SaveInformations.SaveSlot (1, 1);
								TrocaMenus (6);
								inGame = true;
								player.playerInputStartGame = true;
								print ("acionei");
							} else {
								print ("ja tem save");
							}
						} else if (opcaoMenu2 == 2) {
							if (saveSlot2 == 0) {
								SaveInformations.SaveSlot (2, 1);
								TrocaMenus (6);
							}
						} else if (opcaoMenu2 == 3) {
							if (saveSlot3 == 0) {
								SaveInformations.SaveSlot (3, 1);
								TrocaMenus (6);
							}
						}
					} else if (deleteSave) {
						if (opcaoMenu2 == 1) {
							if (SaveInformations.saveSlot1 == 0) {
								SaveInformations.SaveSlot (1, 0);
								TrocaMenus (2);
							}
						} else if (opcaoMenu2 == 2) {
							if (SaveInformations.saveSlot2 == 0) {
								SaveInformations.SaveSlot (2, 0);
								TrocaMenus (2);
							}
						} else if (opcaoMenu2 == 3) {
							if (SaveInformations.saveSlot3 == 0) {
								SaveInformations.SaveSlot (3, 0);
								TrocaMenus (2);
							}
						}
					}
				} else if (onMenu3) {
					if (opcaoMenu3 == 1) {
						controlandoVolume = true;
					}
				} else if (onPause) {
					if (opcaoMenuPause == 1) {
						TrocaMenus (6);
					} else if (opcaoMenuPause == 2) {
						TrocaMenus (3);
					} else if (opcaoMenuPause == 3) {
						TrocaMenus (7);
					} else if (opcaoMenuPause == 4) {
						TrocaMenus (4);
					} else if (opcaoMenuPause == 5) {
						TrocaMenus (1);
						inGame = false;
					} else if (opcaoMenuPause == 6) {
						Application.Quit ();
					}
				} 
				else if (onMenuDeath) {
					if (opcaoMenuMorte == 1) {
						//continuar o jogo
					} else if (opcaoMenuMorte == 2) {
						TrocaMenus (1);
						//ou carregar a mesma cena
					} else if (opcaoMenuMorte == 3) {
						Application.Quit ();
					}
				}
			}
			StartCoroutine ("EnterAvailable");
		}
		if (Input.GetKeyDown (KeyCode.UpArrow) || Input.GetAxisRaw ("L_Joystick_Y") > 0.2f) {
			if (controleOpcaoEixo) {
				if (onMenu1) {
					if (opcaoMenu1 > 1) {
						opcaoMenu1--;
					}
				} else if (onMenu3) {
					if (!controlandoVolume) {
						if (opcaoMenu3 > 1) {
							opcaoMenu3--;
						} 
					} else if (controlandoVolume) {
						musicaEfeito = !musicaEfeito;
					}
				} else if (onPause) {
					if (opcaoMenuPause > 1) {
						opcaoMenuPause--;
					}
				} else if (onMenu2) {
					deleteSave = !deleteSave;
				} else if (onMenuDeath) {
					if (opcaoMenuMorte > 1) {
						opcaoMenuMorte--;
					}
				}
			}
			StartCoroutine ("TrocaOpcaoEixo");
		}
		if (Input.GetKeyDown (KeyCode.DownArrow) || Input.GetAxisRaw ("L_Joystick_Y") < -0.2f) {
			if (controleOpcaoEixo) {
				if (onMenu1) {
					if (opcaoMenu1 < 4) {
						opcaoMenu1++;
					}
				} else if (onMenu3) {
					if (!controlandoVolume) {
						if (opcaoMenu3 < 2) {
							opcaoMenu3++;
						}
					} else if (controlandoVolume) {
						musicaEfeito = !musicaEfeito;
					}
				} else if (onPause) {
					if (opcaoMenuPause < 6) {
						opcaoMenuPause++;
					}
				}else if (onMenu2) {
					deleteSave = !deleteSave;
				}else if (onMenuDeath) {
					if (opcaoMenuMorte < 2) {
						opcaoMenuMorte++;
					}
				}
			}
			StartCoroutine ("TrocaOpcaoEixo");
		}
		if (Input.GetKeyDown (KeyCode.LeftArrow) || Input.GetAxisRaw ("L_Joystick_X") < -0.2f) {
			if (controleOpcaoEixo) {
				if (onMenu2) {
					if (opcaoMenu2 > 1) {
						opcaoMenu2--;
					}
				} else if (onMenu3) {
					if (controlandoVolume) {
						if (musicaEfeito && volumeMusica > 1) {
							SaveInformations.VolumeAlteradoF ();
							volumeMusica--;
							imagensVolume ();
						} else if (!musicaEfeito && volumeEfeitos > 1) {
							SaveInformations.VolumeAlteradoF ();
							volumeEfeitos--;
							imagensVolume ();
						}
					}
				}
			}
			StartCoroutine ("TrocaOpcaoEixo");
		}
		if (Input.GetKeyDown (KeyCode.RightArrow) || Input.GetAxisRaw ("L_Joystick_X") > 0.2f) {
			if (controleOpcaoEixo) {
				if (onMenu2) {
					if (opcaoMenu2 < 3) {
						opcaoMenu2++;
					}
				} else if (onMenu3) {
					if (controlandoVolume) {
						if (musicaEfeito && volumeMusica < 8) {
							SaveInformations.VolumeAlteradoF ();
							volumeMusica++;
							imagensVolume ();
						} else if (!musicaEfeito && volumeEfeitos < 8) {
							SaveInformations.VolumeAlteradoF ();
							volumeEfeitos++;
							imagensVolume ();
						}
					}
				}
			}
			StartCoroutine ("TrocaOpcaoEixo");
		}
		if (Input.GetKeyDown (KeyCode.Escape)||Input.GetKeyDown(KeyCode.JoystickButton1)) {
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
			if (onMenu2) {
				TrocaMenus (1);

			} else if (onMenu3) {
				if (controlandoVolume) {
					controlandoVolume = false;
					SaveInformations.SaveVolume (volumeMusica, volumeEfeitos);
				} else if (!inGame) {
					TrocaMenus (1);
				} else if (inGame) {
					TrocaMenus (0);
				}
			} else if (onMenu4) {
				if (inGame) {
					TrocaMenus (0);
				} else if (!inGame) {
					TrocaMenus (1);
				}
			} else if (onMenu5) {
				TrocaMenus (0);
			} else if (onMenuDeath) {
				TrocaMenus (6);
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
		if(Input.GetKeyDown(KeyCode.O)){
			SaveInformations.SaveSlot (1, 0);
			SaveInformations.SaveSlot (2, 0);
			SaveInformations.SaveSlot (3, 0);

			saveSlot1 = PlayerPrefs.GetInt ("saveSlot1");
			saveSlot2 = PlayerPrefs.GetInt ("saveSlot2");
			saveSlot3 = PlayerPrefs.GetInt ("saveSlot3");
		}
	}

	IEnumerator EnterAvailable(){
		podeEnter = false;
		yield return new WaitForSecondsRealtime (0.1f);
		podeEnter = true;
	}

	IEnumerator TrocaOpcaoEixo(){
		print ("negativo");
		controleOpcaoEixo = false;
		yield return new WaitForSecondsRealtime (0.2f);
		controleOpcaoEixo = true;
	}

	void GetSaveVariables(){
		saveSlot1 = PlayerPrefs.GetInt ("saveSlot1");
		saveSlot2 = PlayerPrefs.GetInt ("saveSlot2");
		saveSlot3 = PlayerPrefs.GetInt ("saveSlot3");

		volumeMusica = PlayerPrefs.GetInt ("volumeMusicaS");
		volumeEfeitos = PlayerPrefs.GetInt ("volumeEfeitoS");
		volumeAlterado = PlayerPrefs.GetInt ("volumeAlterado");
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
			menu5.SetActive (false);
			menuMorte.SetActive (false);
			menuPause.SetActive (true);
			telaFundo.SetActive (true);

			onPause = true;
			onMenu1 = false;
			onMenu2 = false;
			onMenu3 = false;
			onMenu4 = false;
			onMenu5 = false;
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
			menu5.SetActive (false);
			menuMorte.SetActive (false);
			menuPause.SetActive (false);
			telaFundo.SetActive (true);

			onPause = false;
			onMenu1 = true;
			onMenu2 = false;
			onMenu3 = false;
			onMenu4 = false;
			onMenu5 = false;
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
			menu5.SetActive (false);
			menuMorte.SetActive (false);
			menuPause.SetActive (false);
			telaFundo.SetActive (true);

			onPause = false;
			onMenu1 = false;
			onMenu2 = true;
			onMenu3 = false;
			onMenu4 = false;
			onMenu5 = false;
			onMenuDeath = false;
			jogando = false;
			deleteSave = false;
			//inGame = false;

			opcaoMenu2 = 1;
			player.playerCanMove = false;
			imagensVolume ();
			break;

		case 3://menuOpçõesConfigurações

			menu1.SetActive (false);
			menu2.SetActive (false);
			menu3.SetActive (true);
			menu4.SetActive (false);
			menu5.SetActive (false);
			menuMorte.SetActive (false);
			menuPause.SetActive (false);
			telaFundo.SetActive (true);

			onPause = false;
			onMenu1 = false;
			onMenu2 = false;
			onMenu3 = true;
			onMenu4 = false;
			onMenu5 = false;
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
			menu5.SetActive (false);
			menuMorte.SetActive (false);
			menuPause.SetActive (false);
			telaFundo.SetActive (true);

			onPause = false;
			onMenu1 = false;
			onMenu2 = false;
			onMenu3 = false;
			onMenu4 = true;
			onMenu5 = false;
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
			menu5.SetActive (false);
			menuMorte.SetActive (true);
			menuPause.SetActive (false);
			telaFundo.SetActive (true);

			onPause = false;
			onMenu1 = false;
			onMenu2 = false;
			onMenu3 = false;
			onMenu4 = false;
			onMenu5 = false;
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
			menu5.SetActive (false);
			menuMorte.SetActive (false);
			menuPause.SetActive (false);
			telaFundo.SetActive (false);

			onPause = false;
			onMenu1 = false;
			onMenu2 = false;
			onMenu3 = false;
			onMenu4 = false;
			onMenu5 = false;
			onMenuDeath = false;
			jogando = true;
			//inGame = true;

			player.playerCanMove = true;

			Time.timeScale = 1f;
			break;

		case 7://controleImagem

			menu1.SetActive (false);
			menu2.SetActive (false);
			menu3.SetActive (false);
			menu4.SetActive (false);
			menu5.SetActive (true);
			menuMorte.SetActive (false);
			menuPause.SetActive (false);
			telaFundo.SetActive (true);


			onPause = false;
			onMenu1 = false;
			onMenu2 = false;
			onMenu3 = false;
			onMenu4 = false;
			onMenu5 = true;
			onMenuDeath = false;
			jogando = true;

			player.playerCanMove = false;
			break;

		}
		
	}
}
