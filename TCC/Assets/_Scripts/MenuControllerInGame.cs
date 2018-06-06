
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuControllerInGame : MonoBehaviour {

	public WalkingController player;
	public int opcaoMenu1, opcaoMenu2, opcaoMenu3, opcaoMenuPause, opcaoMenuMorte, volumeMusica, volumeEfeitos, opcaoCerteza;
	public int saveSlot1, saveSlot2, saveSlot3, volumeAlterado;
	private bool onMenu1, onMenu2, onMenu3, onMenu4, onMenu5, onMenuDeath,onPause, controlandoVolume, musicaEfeito, inGame, onSureBox;
	private bool podeEnter, jogando, controleOpcaoEixo, deleteSave;
	public GameObject menu1, menu2, menu3, menu4, menu5, menuPause, menuMorte, menuFakeDeath, telaFundo, seta, certezaBox;
	public GameObject VM1, VM2, VM3, VM4, VM5, VM6, VM7, VM8;
	public GameObject VE1, VE2, VE3, VE4, VE5, VE6, VE7, VE8;
	public GameObject ovo1, ovo2, ovo3, ovoQuebrado1, ovoQuebrado2, ovoQuebrado3;
	public static SaveInformations save;
	public int faseAtual;
	private bool fakeDeathMenu;
	public int faseAtualSave;
	int entersToSavePlayer = 0;
	public GameObject SetaIndicativa1, SetaIndicativa2, SetaIndicativa3, SetaIndicativa4, SetaIndicativa5, SetaIndicativaCertezaBox;
	public Text textoSave1, textoSave2, textoSave3;
	private float timeToMoveAxys;

	private FatherSacrifice_Ctrl fatherSacrificeCtrl;

	// Use this for initialization
	void Start () {
		//GetSaveVariables ();
		GetSaveVariables ();//pega as fases de cada slot
		SaveInformations.SalvaCena (faseAtualSave);
		//print (faseAtualSave);
		TemCertezaBox (false);
		if (volumeAlterado != 2) {
			volumeMusica = 8;
			volumeEfeitos = 8;
		}

		imagensVolume ();

		onPause = false;
		inGame = false;
		TrocaMenus (faseAtual);


		podeEnter = true;
		controleOpcaoEixo = true;
	}
	
	// Update is called once per frame
	void Update () {
		//print (controleOpcaoEixo);
//		if (controleOpcaoEixo == false) {
//			timeToMoveAxys += Time.deltaTime * 1.0f;
//		}
//		if (timeToMoveAxys >= 0.3f) {
//			controleOpcaoEixo = true;
//			timeToMoveAxys = 0;
//		}
		if (Input.GetKeyDown (KeyCode.Return)||Input.GetKeyDown(KeyCode.JoystickButton0)) {
			if (podeEnter) {
				if (!onSureBox) {
					if (onMenu1) {
						if (opcaoMenu1 == 1) {
							TrocaMenus (2);
						} else if (opcaoMenu1 == 2) {
							//TrocaMenus (3);
						} else if (opcaoMenu1 == 3) {
							TrocaMenus (4);
						} else if (opcaoMenu1 == 4) {
							TemCertezaBox (true);
							//Application.Quit ();
						}
					} else if (onMenu2) {
						if (!deleteSave) {
							if (opcaoMenu2 == 1) {
								SaveInformations.EscolhendoSlot (1);
								if (saveSlot1 == 0 || saveSlot1 == 1) {
									SaveInformations.SaveSlot (1, 1);
									TrocaMenus (6);
									inGame = true;
									player.playerInputStartGame = true;

									//print ("acionei");
								} else if (saveSlot1 == 2) {
									SceneManager.LoadScene (2);
								} else if (saveSlot1 == 3) {
									SceneManager.LoadScene (3);
									////print ("ja tem save");
								} else if (saveSlot1 == 4) {
									SceneManager.LoadScene (4);
								}
							} else if (opcaoMenu2 == 2) {
								SaveInformations.EscolhendoSlot (2);
								if (saveSlot2 == 0 || saveSlot2 == 1) {
									SaveInformations.SaveSlot (2, 1);
									TrocaMenus (6);
									inGame = true;
									player.playerInputStartGame = true;
									////print ("acionei");
								} else if (saveSlot2 == 2) {
									SceneManager.LoadScene (2);
									////print ("ja tem save");
								} else if (saveSlot2 == 3) {
									SceneManager.LoadScene (3);
									////print ("ja tem save");
								} else if (saveSlot2 == 4) {
									SceneManager.LoadScene (4);
								}
							} else if (opcaoMenu2 == 3) {
								SaveInformations.EscolhendoSlot (3);
								if (saveSlot3 == 0 || saveSlot3 == 1) {
									SaveInformations.SaveSlot (3, 1);
									TrocaMenus (6);
									inGame = true;
									player.playerInputStartGame = true;
									////print ("acionei");
								} else if (saveSlot3 == 3) {
									SceneManager.LoadScene (3);
									////print ("ja tem save");
								} else if (saveSlot3 == 2) {
									SceneManager.LoadScene (2);
									////print ("ja tem save");
								} else if (saveSlot3 == 4) {
									SceneManager.LoadScene (4);
								}
							} else if (opcaoMenu2 == 4) {
								TrocaMenus (1);
							}
						} else if (deleteSave) {
							if (opcaoMenu2 == 1) {
								TemCertezaBox (true);
//								SaveInformations.SaveSlot (1, 0);
//								GetSaveVariables ();
//								TrocaMenus (2);
							} else if (opcaoMenu2 == 2) {
								TemCertezaBox (true);
//								SaveInformations.SaveSlot (2, 0);
//								GetSaveVariables ();
//								TrocaMenus (2);
							} else if (opcaoMenu2 == 3) {
								TemCertezaBox (true);
//								SaveInformations.SaveSlot (3, 0);
//								GetSaveVariables ();
//								TrocaMenus (2);
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
							//TrocaMenus (3);
						} else if (opcaoMenuPause == 3) {
							TrocaMenus (7);
						} else if (opcaoMenuPause == 4) {
							TrocaMenus (4);
						} else if (opcaoMenuPause == 5) {
							TemCertezaBox (true);
//							SceneManager.LoadScene (1);
						}
//					else if (opcaoMenuPause == 6) {
//						Application.Quit ();
//					}
					} else if (onMenuDeath) {
						if (opcaoMenuMorte == 1) {
							//if(!fakeDeathMenu) {
							Time.timeScale = 1f;
							TrocaMenus (6);
							//}
							//else if(fakeDeathMenu){
							//	PaiCaminhandoParaMorte ();
							//}
						} else if (opcaoMenuMorte == 2) {
							TemCertezaBox (true);
							//SceneManager.LoadScene (1);
						} else if (opcaoMenuMorte == 3) {
							TemCertezaBox (true);
							//Application.Quit ();
						}
					}
				} else if (onSureBox) {
					if (opcaoCerteza == 1) {
						if (onMenu1) {
							if (opcaoMenu1 == 4) {
								Application.Quit ();
							}
						} else if (onMenu2) {
							if (deleteSave) {
								if (opcaoMenu2 == 1) {
									SaveInformations.SaveSlot (1, 0);
									GetSaveVariables ();
									TrocaMenus (2);
								} else if (opcaoMenu2 == 2) {
									SaveInformations.SaveSlot (2, 0);
									GetSaveVariables ();
									TrocaMenus (2);
								} else if (opcaoMenu2 == 3) {
									SaveInformations.SaveSlot (3, 0);
									GetSaveVariables ();
									//saveSlot3 = PlayerPrefs.GetInt ("saveSlot3");
									TrocaMenus (2);
								}
							}
						} else if (onPause) {
							if (opcaoMenuPause == 5) {
								SceneManager.LoadScene (1);
							}
						} else if (onMenuDeath) {
							if (opcaoMenuMorte == 2) {
								SceneManager.LoadScene (1);
							} else if (opcaoMenuMorte == 3) {
								Application.Quit ();
							}
						}
					} else if (opcaoCerteza == 2) {
						TemCertezaBox (false);
					}
				}
			}
			AlteraIndicadorSelecao ();
		
		}
		if (Input.GetKeyDown (KeyCode.UpArrow) || Input.GetAxisRaw ("L_Joystick_Y") > 0.2f) {
			if (controleOpcaoEixo) {
				if (!onSureBox) {
					if (onMenu1) {
						if (opcaoMenu1 > 1) {
							opcaoMenu1--;
						} else if (opcaoMenu1 == 1) {
							opcaoMenu1 = 4;
						}
					} else if (onMenu3) {
						if (!controlandoVolume) {
							if (opcaoMenu3 > 1) {
								opcaoMenu3--;
							} else if (opcaoMenu3 == 1) {
								opcaoMenu3 = 2;
							}
						} else if (controlandoVolume) {
							musicaEfeito = !musicaEfeito;
						}
					} else if (onPause) {
						if (opcaoMenuPause > 1) {
							opcaoMenuPause--;
						} else if (opcaoMenuPause == 1) {
							opcaoMenuPause = 5;
						}
					} else if (onMenu2) {
						deleteSave = !deleteSave;
					} else if (onMenuDeath) {
						if (opcaoMenuMorte > 1) {
							opcaoMenuMorte--;
						} else if (opcaoMenuMorte == 1) {
							opcaoMenuMorte = 3;
						}
					}
					AlteraIndicadorSelecao ();
				}
			}
			if (controleOpcaoEixo == true) {
				StartCoroutine ("TrocaOpcaoEixo");
				controleOpcaoEixo = false;
			}

		
		}
		if (Input.GetKeyDown (KeyCode.DownArrow) || Input.GetAxisRaw ("L_Joystick_Y") < -0.2f) {
			if (controleOpcaoEixo) {
				if (!onSureBox) {
					if (onMenu1) {
						if (opcaoMenu1 < 4) {
							opcaoMenu1++;
						} else if (opcaoMenu1 == 4) {
							opcaoMenu1 = 1;
						}
					} else if (onMenu3) {
						if (!controlandoVolume) {
							if (opcaoMenu3 < 2) {
								opcaoMenu3++;
							} else if (opcaoMenu3 == 2) {
								opcaoMenu3 = 1;
							}
						} else if (controlandoVolume) {
							musicaEfeito = !musicaEfeito;
						}
					} else if (onPause) {
						if (opcaoMenuPause < 5) {
							opcaoMenuPause++;
						} else if (opcaoMenuPause == 5) {
							opcaoMenuPause = 1;
						}
					} else if (onMenu2) {
						deleteSave = !deleteSave;
					} else if (onMenuDeath) {
						if (opcaoMenuMorte < 3) {
							opcaoMenuMorte++;
						} else if (opcaoMenuMorte == 3) {
							opcaoMenuMorte = 1;
						}
					}
					AlteraIndicadorSelecao ();
				}
			}
			if (controleOpcaoEixo == true) {
				StartCoroutine ("TrocaOpcaoEixo");
				controleOpcaoEixo = false;
			}

		
		}
		if (Input.GetKeyDown (KeyCode.LeftArrow) || Input.GetAxisRaw ("L_Joystick_X") < -0.2f) {
			if (controleOpcaoEixo) {
				if (!onSureBox) {
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

				} else if (onSureBox) {
					if (opcaoCerteza > 1) {
						opcaoCerteza--;
					}
				}
				AlteraIndicadorSelecao ();
			}
			if (controleOpcaoEixo == true) {
				StartCoroutine ("TrocaOpcaoEixo");
				controleOpcaoEixo = false;
			}

		

		}
		if (Input.GetKeyDown (KeyCode.RightArrow) || Input.GetAxisRaw ("L_Joystick_X") > 0.2f) {
			if (controleOpcaoEixo) {
				if (!onSureBox) {
					if (onMenu2) {
						if (opcaoMenu2 < 4) {
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

				}else if (onSureBox) {
					if (opcaoCerteza < 2) {
						opcaoCerteza++;
					}
				}
				AlteraIndicadorSelecao ();
			}
			if (controleOpcaoEixo == true) {
				StartCoroutine ("TrocaOpcaoEixo");

				controleOpcaoEixo = false;

			}


		}
		if (Input.GetKeyDown (KeyCode.Escape)||Input.GetKeyDown(KeyCode.JoystickButton1)) {
			if (!onSureBox) {
				if (inGame) {
					if (onPause) {
						TrocaMenus (6);
						////print ("jogando");
					} else if (jogando) {
						TrocaMenus (0);
						////print ("pause");
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
			} else if (onSureBox) {
				TemCertezaBox (false);
			}
			AlteraIndicadorSelecao ();
		}
		if (Input.GetKeyDown (KeyCode.JoystickButton7)) {
			if (!onSureBox) {
				if (inGame) {
					if (onPause) {
						TrocaMenus (6);
						////print ("jogando");
					} else if (jogando) {
						TrocaMenus (0);
						////print ("pause");
					}
				}
			} else if (onSureBox) {
				
					TemCertezaBox (false);
				
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
		yield return new WaitForSecondsRealtime (0.3f);
		controleOpcaoEixo = true;
	}

	void AlteraIndicadorSelecao(){
		if (onMenu1) {
			if (opcaoMenu1 == 1) {
				SetaIndicativa1.transform.localPosition = new Vector3 (-17.6f, 45.0f, 0);
			} else if (opcaoMenu1 == 2) {
				SetaIndicativa1.transform.localPosition = new Vector3 (-17.6f, -45.0f, 0);
			} else if (opcaoMenu1 == 3) {
				SetaIndicativa1.transform.localPosition = new Vector3 (-17.6f, -145.0f, 0);
			} else if (opcaoMenu1 == 4) {
				SetaIndicativa1.transform.localPosition = new Vector3 (-17.6f, -245.0f, 0);
			}
		} else if (onMenu2) {
			if (opcaoMenu2 == 1) {
				SetaIndicativa2.SetActive (true);
				SetaIndicativa3.SetActive (false);
				SetaIndicativa2.transform.localPosition = new Vector3 (-280.0f, -223.0f, 0f);
			} else if (opcaoMenu2 == 2) {
				SetaIndicativa2.SetActive (true);
				SetaIndicativa3.SetActive (false);
				SetaIndicativa2.transform.localPosition = new Vector3 (0.0f, -223.0f, 0f);
			} else if (opcaoMenu2 == 3) {
				SetaIndicativa2.SetActive (true);
				SetaIndicativa3.SetActive (false);
				SetaIndicativa2.transform.localPosition = new Vector3 (280.0f, -223.0f, 0f);
			} else if (opcaoMenu2 == 4) {
				SetaIndicativa2.SetActive (false);
				SetaIndicativa3.SetActive (true);
			}
		} else if (onPause) {
			if (opcaoMenuPause == 1) {
				SetaIndicativa4.transform.localPosition = new Vector3 (13.0f, 120.0f, 0f);
			} else if (opcaoMenuPause == 2) {
				SetaIndicativa4.transform.localPosition = new Vector3 (13.0f, 30.0f, 0f);
			} else if (opcaoMenuPause == 3) {
				SetaIndicativa4.transform.localPosition = new Vector3 (13.0f, -60.0f, 0f);
			} else if (opcaoMenuPause == 4) {
				SetaIndicativa4.transform.localPosition = new Vector3 (13.0f, -150.0f, 0f);
			} else if (opcaoMenuPause == 5) {
				SetaIndicativa4.transform.localPosition = new Vector3 (13.0f, -240.0f, 0f);
			}
		} else if (onMenuDeath) {
			if (opcaoMenuMorte == 1) {
				SetaIndicativa5.transform.localPosition = new Vector3 (-3.0f, -12.0f, 0f);
			} else if (opcaoMenuMorte == 2) {
				SetaIndicativa5.transform.localPosition = new Vector3 (13.0f, -124.0f, 0f);
			} else if (opcaoMenuMorte == 3) {
				SetaIndicativa5.transform.localPosition = new Vector3 (13.0f, -236.0f, 0f);
			}
		}
		if (onSureBox) {
			if (opcaoCerteza == 1) {
				SetaIndicativaCertezaBox.transform.localPosition = new Vector3 (-10.0f, -18.6f, 0);
				print (opcaoCerteza);
			} else if (opcaoCerteza == 2) {
				SetaIndicativaCertezaBox.transform.localPosition = new Vector3 (10.0f, -18.6f, 0);
				print (opcaoCerteza);
			}
		}
	}
	void PaiCaminhandoParaMorte (){
		print ("Morreu");
//		if(entersToSavePlayer == 0){
//			fatherSacrificeCtrl.StartSacrifice ();
//			entersToSavePlayer++;
//			return;
//		}
		//entersToSavePlayer++;
		player.playerCanCanOnlySing = true;
		fatherSacrificeCtrl.CheckSingToStartSacrifice ();
		//fatherSacrificeCtrl.ContinueAVoar ();
//		if(entersToSavePlayer <= 0){
//			MortePai ();
//		}
	}

//	bool CertezaSimNao(bool ){
//
//	}

	void TemCertezaBox(bool certezaAtiva){
		if (certezaAtiva) {
			certezaBox.SetActive (true);
			opcaoCerteza = 1;
			onSureBox = true;

		} else if (!certezaAtiva) {
			certezaBox.SetActive (false);
			onSureBox = false;
//			if (qualMenu == 10) {
//				return;
//			}else if (qualMenu == 0) {
//				onPause = true;
//			} else if (qualMenu == 1) {
//				onMenu1 = true;
//			}else if (qualMenu == 2) {
//				onMenu2 = true;
//			}else if (qualMenu == 3) {
//				onMenu3 = true;
//			}else if (qualMenu == 4) {
//				onMenu4 = true;
//			}else if (qualMenu == 5) {
//				onMenu5 = true;
//			}else if (qualMenu == 6) {
//				onMenuDeath = true;
//			}else if (qualMenu == 7) {
//				jogando = true;
//			}else if (qualMenu == 8) {
//				fakeDeathMenu = true;
//			}

		}
	}

//	void MortePai(){
//		//TODO
//	}


	void GetSaveVariables(){
		saveSlot1 = PlayerPrefs.GetInt ("saveSlot1");
		saveSlot2 = PlayerPrefs.GetInt ("saveSlot2");
		saveSlot3 = PlayerPrefs.GetInt ("saveSlot3");

		//print (saveSlot1);
		//print (saveSlot2);
		//print (saveSlot3);

		if (saveSlot1 == 0) {
			textoSave1.text = "0%";
			ovoQuebrado1.SetActive (false);
			ovo1.SetActive (true);
		}else if (saveSlot1 == 2) {
			textoSave1.text = "25%";
			ovoQuebrado1.SetActive (true);
			ovo1.SetActive (false);
		}else if (saveSlot1 == 3) {
			textoSave1.text = "50%";
			ovoQuebrado1.SetActive (true);
			ovo1.SetActive (false);
		}else if (saveSlot1 == 4) {
			textoSave1.text = "75%";
			ovoQuebrado1.SetActive (true);
			ovo1.SetActive (false);
		}

		if (saveSlot2 == 0) {
			textoSave2.text = "0%";
			ovoQuebrado2.SetActive (false);
			ovo2.SetActive (true);
		}else if (saveSlot2 == 2) {
			textoSave2.text = "25%";
			ovoQuebrado2.SetActive (false);
			ovo2.SetActive (true);
		}else if (saveSlot2 == 3) {
			textoSave2.text = "50%";
			ovoQuebrado2.SetActive (false);
			ovo2.SetActive (true);
		}else if (saveSlot2 == 4) {
			textoSave2.text = "75%";
			ovoQuebrado2.SetActive (false);
			ovo2.SetActive (true);
		}

		if (saveSlot3 == 0) {
			textoSave2.text = "0%";
			ovoQuebrado3.SetActive (false);
			ovo3.SetActive (true);
		}else if (saveSlot3 == 2) {
			textoSave2.text = "25%";
			ovoQuebrado3.SetActive (false);
			ovo3.SetActive (true);
		}else if (saveSlot3 == 3) {
			textoSave2.text = "50%";
			ovoQuebrado3.SetActive (false);
			ovo3.SetActive (true);
		}else if (saveSlot3 == 4) {
			textoSave2.text = "75%";
			ovoQuebrado3.SetActive (false);
			ovo3.SetActive (true);
		}

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
		//print (numeroMenu);
		switch (numeroMenu) {
		case 0:// menuPausa
			
			menu1.SetActive (false);
			menu2.SetActive (false);
			menu3.SetActive (false);
			menu4.SetActive (false);
			menu5.SetActive (false);
			menuMorte.SetActive (false);
			menuFakeDeath.SetActive (false);
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
			fakeDeathMenu = false;
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
			menu5.SetActive (false);
			menuMorte.SetActive (false);
			menuFakeDeath.SetActive (false);
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
			fakeDeathMenu = false;
			inGame = false;

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
			menuFakeDeath.SetActive (false);
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
			fakeDeathMenu = false;
			inGame = false;

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
			menuFakeDeath.SetActive (false);
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
			fakeDeathMenu = false;
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
			menuFakeDeath.SetActive (false);
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
			fakeDeathMenu = false;
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
			menuFakeDeath.SetActive (false);
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
			fakeDeathMenu = false;
			inGame = false;

			opcaoMenuMorte = 1;
			player.playerCanMove = false;
			Time.timeScale = 0f;
			break;

		case 6://Nenhum

			menu1.SetActive (false);
			menu2.SetActive (false);
			menu3.SetActive (false);
			menu4.SetActive (false);
			menu5.SetActive (false);
			menuMorte.SetActive (false);
			menuFakeDeath.SetActive (false);
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
			fakeDeathMenu = false;
			inGame = true;

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
			menuFakeDeath.SetActive (false);
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
			fakeDeathMenu = false;

			player.playerCanMove = false;
			break;

		case 8://fakeDeath

			fatherSacrificeCtrl = FindObjectOfType<FatherSacrifice_Ctrl> ();

			menu1.SetActive (false);
			menu2.SetActive (false);
			menu3.SetActive (false);
			menu4.SetActive (false);
			menu5.SetActive (false);
			menuMorte.SetActive (false);
			menuFakeDeath.SetActive (true);
			menuPause.SetActive (false);
			telaFundo.SetActive (false);


			onPause = false;
			onMenu1 = false;
			onMenu2 = false;
			onMenu3 = false;
			onMenu4 = false;
			onMenu5 = false;
			onMenuDeath = true;
			jogando = false;
			fakeDeathMenu = true;

			opcaoMenuMorte = 1;
			player.playerCanMove = false;
			PaiCaminhandoParaMorte ();
			break;

		}
		
	}
}
