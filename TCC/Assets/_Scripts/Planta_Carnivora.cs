using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planta_Carnivora : PlantaBehaviour {

	//Transform comidaContainer_Pos, comidaContainer_Neg;
	SphereCollider coll;

	enum DirectionReference {Up, Forward, Right}
	[SerializeField]
	DirectionReference facingRef;
	Vector3 facingDirection;

	[SerializeField]
	float attackRange_default = 3f;
	float attackRange_irritado;

	bool fechada;
	float foodDir = 0f;
	Transform foodContainer;

	#region DELETAR
	GameObject indicadorDeSono;
	#endregion

	protected override void Awake ()
	{
		base.Awake ();

		foodContainer = plantaTransform.Find ("FoodContainer");
//		comidaContainer_Pos = plantaTransform.Find("ComidaContainer_Pos");
//		comidaContainer_Neg = plantaTransform.Find("ComidaContainer_Neg");
	}

	void Start (){
		indicadorDeSono = GameObject.CreatePrimitive (PrimitiveType.Cylinder);
		indicadorDeSono.transform.position = plantaTransform.position + (Vector3.up * 5);

		coll = GetComponent<SphereCollider> ();
		attackRange_irritado = attackRange_default * 2f;

		fechada = false;

		switch (facingRef) {
		case DirectionReference.Up:
			facingDirection = plantaTransform.up;
			break;
		case DirectionReference.Forward:
			facingDirection = plantaTransform.forward;
			break;
		case DirectionReference.Right:
			facingDirection = plantaTransform.right;
			break;
		default:
			break;
		}
	}

	protected override void Update ()
	{
		base.Update ();

		if(fechada){
			
		}

		#region DELETAR
		if(currentState != Planta_CurrentState.Dormindo){
			indicadorDeSono.SetActive (false);
		} else {
			indicadorDeSono.SetActive (true);
		}
		#endregion
	}

	void Attack (Transform food){
		fechada = true;
		//Vector3 dirToFood = (food.transform.position - plantaTransform.position);
		foodDir = Mathf.Sign (Vector3.Dot ((food.position - plantaTransform.position), facingDirection));
		foodContainer.localPosition = foodDir * facingDirection;
		//TODO: Mandar fechar pro lado certo. Prender comida.

//		switch (type) {
//		case FoodType.Player:
//
//			break;
//		case FoodType.NPC:
//
//			break;
//		case FoodType.Planta:
//
//			break;
//		case FoodType.Pai:
//
//			break;
//		default:
//			break;
//		}


	}


	protected override void DefaultState ()
	{
		if(currentState == Planta_CurrentState.Irritado){
			return;
		}

		base.DefaultState ();

		//CheckForFood ();
	}

	protected override void Crescer ()
	{
		if(currentState == Planta_CurrentState.Irritado){
			return;
		}

		base.Crescer ();
	}

	protected override void Distrair ()
	{
		if(currentState == Planta_CurrentState.Irritado){
			return;
		}

		base.Distrair ();
	}

	protected override void Dormir ()
	{
		if(currentState == Planta_CurrentState.Irritado){
			return;
		}

		base.Dormir ();
	}
	protected override void Acordar ()
	{
		if(currentState == Planta_CurrentState.Irritado){
			return;
		}

		base.Acordar ();
	}

	protected override void Irritar ()
	{
//		if(currentState == Planta_CurrentState.Irritado){
//			CheckForFood ();
//			return;
//		}
		base.Irritar ();
		//currentState = Planta_CurrentState.Irritado;

		//comidaContainer_Pos.localScale = comidaContainer_Neg.localScale = Vector3.one * attackRange_irritado;
		coll.radius = attackRange_irritado;
	}
	protected override void Acalmar ()
	{
		base.Acalmar ();

		//comidaContainer_Pos.localScale = comidaContainer_Neg.localScale = Vector3.one * attackRange_default;
		coll.radius = attackRange_default;

	}

	protected override void OnTriggerStay (Collider col)
	{
		base.OnTriggerStay (col);

		if (!fechada && (currentState == Planta_CurrentState.DefaultState || currentState == Planta_CurrentState.Irritado)) {
//			if (col.CompareTag("Player")) {
//				Attack (FoodType.Player, col.gameObject);
//			} else if (col.GetComponent<NPCBehaviour> () != null) {
//				Attack (FoodType.NPC, col.gameObject);
//			} else if (col.GetComponent<PlantaBehaviour> () != null) {
//				Attack (FoodType.Planta, col.gameObject);
//			} else if (col.CompareTag("NPC_Pai")) {
//				Attack (FoodType.Pai, col.gameObject);
//			}

			if (col.GetComponent<ICarnivoraEdible> () != null) {
				col.GetComponent<ICarnivoraEdible> ().Carnivora_GetReadyToBeEaten ();
				Attack (col.transform);
			}
		}
	}

//	enum FoodType
//	{
//		NPC,
//		Planta,
//		Player,
//		Pai
//	}
}
