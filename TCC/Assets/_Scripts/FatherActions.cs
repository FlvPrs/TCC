using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FatherActions : AgentFather {

	protected override void Start(){
		base.Start ();
	}

	// ---------------------------------------------------------------------

	void Stay (){

	}

	void MoveHere (Vector3 pos){
		MoveAgentOnNavMesh (pos);
	}

	void MoveToPlayer (bool followAfter = false){
		MoveAgentOnNavMesh (player.position);
	}

	void GuidePlayerTo (Vector3 pos){

	}

	void FollowPlayer (){

	}

	void RandomWalk (Vector3 areaCenter){

	}

	void JumpAndFall (){

	}

	void JumpAndGlide (float seconds = 0f){

	}

	void ChangeHeight (HeightState height, float seconds = 0f){

	}

	void Sing_SingleNote (){

	}

	void Sing_SingleNoteRepeat (int times = 0){

	}

	void Sing_Partitura (int partitura, float intervalBetweenNotes = 1f){

	}
}
