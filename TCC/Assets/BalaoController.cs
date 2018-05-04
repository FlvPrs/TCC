using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalaoController : MonoBehaviour {

	//	0 - Default
	//	1 - High
	//	2 - Low

	public Sprite[] notasComeco, notasMeio;
	public Sprite notaFim;

	private SpriteRenderer[] spritePositions;

	int currentIndex = 0;

	bool balaoAtivo = false;
	float cooldown = 2f;

	void Awake (){
		spritePositions = new SpriteRenderer[5];

		for (int i = 0; i < spritePositions.Length; i++) {
			spritePositions [i] = transform.GetChild (i).GetComponent<SpriteRenderer> ();
			spritePositions [i].sprite = null;
		}
	}

	void Update (){
		if (!balaoAtivo)
			return;
		
		if (cooldown > 0f) {
			cooldown -= Time.deltaTime;
		} else {
			balaoAtivo = false;
			Invoke ("Hide_BalaoNotas", 3f);
		}
	}

	public void Show_BalaoNotas (HeightState height){
		cooldown = 2f;
		CancelInvoke ();

		if (currentIndex == 0) {
			for (int i = 0; i < spritePositions.Length; i++) {
				spritePositions [i].sprite = null;
			}
			balaoAtivo = true;
			spritePositions [currentIndex].sprite = notasComeco [(int)height];
		} else {
			spritePositions [currentIndex].sprite = notasMeio [(int)height];
		}

		spritePositions [++currentIndex].sprite = notaFim;

		if (currentIndex >= 4)
			currentIndex = 0;
	}

	public void Hide_BalaoNotas (){
		for (int i = 0; i < spritePositions.Length; i++) {
			spritePositions [i].sprite = null;
		}

		currentIndex = 0;
		cooldown = 0f;
	}
}
