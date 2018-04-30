using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGame : MonoBehaviour {

	private Vector3 player_levelStartPosition;
	
	// Update is called once per frame
//	void Update () {
////		if (Input.GetKeyDown (KeyCode.Escape))
////			Quit ();
//	}

	public static void ChangeLevel (int lvlIndex = -1){
		if(lvlIndex == -1){
			int index = SceneManager.GetActiveScene ().buildIndex;
			if (index < SceneManager.sceneCount)
				index++;
			else
				index = 0;
			SceneManager.LoadScene (index);
		} else {
			SceneManager.LoadScene (lvlIndex);
		}

	}

	public static void Restart(){
		SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
	}

	public static void Quit(){
		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#else
		Application.Quit ();
		#endif
	}
}
