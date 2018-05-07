using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planta_Carnivora : PlantaBehaviour {

	SphereCollider coll;

	enum DirectionReference {Up, Forward, Right}
	[SerializeField]
	DirectionReference facingRef;
	Vector3 facingDirection;

	[SerializeField]
	float attackRange_default = 3f;
	float attackRange_irritado;

	[SerializeField]
	bool fechada, comendo;
	float foodDir = 0f;
	Transform foodContainer;
	ICarnivoraEdible currentFood;

	public LayerMask foodMask;
	public float shootingStrength = 30f;

	#region DELETAR
	GameObject indicadorDeSono;
	#endregion

	protected override void Awake ()
	{
		base.Awake ();

		foodContainer = plantaTransform.Find ("FoodContainer");
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

		//Debug.DrawRay (transform.position, plantaTransform.up * 10f);

		#region DELETAR
		if(currentState != Planta_CurrentState.Dormindo && !fechada){
			indicadorDeSono.SetActive (false);
		} else {
			indicadorDeSono.SetActive (true);
		}

		if (comendo) {
			if(Input.GetKeyDown(KeyCode.Keypad0)){
				ReleaseFood ();
			} else if (Input.GetKeyDown(KeyCode.Keypad1)) {
				ShootFood ();
			}
		}
		#endregion
	}

	void Attack (Transform food){
		fechada = comendo = true;

		coll.radius = attackRange_default;
		currentState = Planta_CurrentState.DefaultState;

		foodDir = Mathf.Sign (Vector3.Dot ((food.position - plantaTransform.position), facingDirection));
		switch (facingRef) {
		case DirectionReference.Up:
			foodContainer.localPosition = Vector3.up * foodDir;
			break;
		case DirectionReference.Forward:
			foodContainer.localPosition = Vector3.forward * foodDir;
			break;
		case DirectionReference.Right:
			foodContainer.localPosition = Vector3.right * foodDir;
			break;
		default:
			break;
		}
		
		food.position = foodContainer.position;
	}

	void ReleaseFood (){
		fechada = true;
		comendo = false;
		currentFood.Carnivora_Release ();
		currentFood = null;
	}

	void ShootFood (){
		fechada = comendo = false;
		currentFood.Carnivora_Shoot (foodDir * facingDirection * shootingStrength);
		currentFood = null;
	}


	protected override void DefaultState ()
	{
		if(currentState == Planta_CurrentState.Irritado){
			return;
		} else if(currentState == Planta_CurrentState.Dormindo){
			Acordar ();
		}

		base.DefaultState ();
	}

	protected override void Crescer ()
	{
		if(currentState == Planta_CurrentState.Irritado || fechada){
			return;
		}

		base.Crescer ();
	}
	protected override void Encolher ()
	{
		if(currentState == Planta_CurrentState.Irritado || fechada){
			return;
		}

		base.Encolher ();
	}

	protected override void Distrair ()
	{
		if(currentState == Planta_CurrentState.Irritado || fechada){
			return;
		}

		base.Distrair ();
	}

	protected override void Dormir ()
	{
		if(currentState == Planta_CurrentState.Irritado || fechada){
			return;
		}

		fechada = true;

		base.Dormir ();
	}
	protected override void Acordar ()
	{
		//Se eu estou aqui é pq eu ja sei que (currentState == Dormindo).

		fechada = false;

		base.Acordar ();
	}

//	protected override void ChamarAtencao ()
//	{
//		base.ChamarAtencao ();
//
//		fechada = false;
//	}

	protected override void Irritar ()
	{
		if(currentState == Planta_CurrentState.Dormindo){
			Acordar ();
		}

		base.Irritar (); //Tudo depois desta linha só roda uma vez.

		coll.radius = attackRange_irritado;

		if(comendo){
			ShootFood ();
		}
	}
	protected override void Acalmar ()
	{
		if(currentState == Planta_CurrentState.Dormindo){
			Acordar ();
		}
		
		base.Acalmar ();

		//comidaContainer_Pos.localScale = comidaContainer_Neg.localScale = Vector3.one * attackRange_default;
		coll.radius = attackRange_default;

		if(comendo){
			print ("Comeu");
			ReleaseFood ();
		}
	}


	protected override void OnTriggerEnter (Collider col)
	{
		base.OnTriggerEnter (col);

		//Este IF checa se a layer de 'col' é uma das layer contidas na layer mask fornecida e, se não for, return.
		if((foodMask.value & 1<<col.gameObject.layer) == 0){ //TODO: Descobrir o que diabos cada coisa dessa linha significa.
			return;
		}

		if (!fechada && (currentState == Planta_CurrentState.DefaultState || currentState == Planta_CurrentState.Irritado)) {
			if (col.GetComponent<ICarnivoraEdible> () != null) {
				currentFood = col.GetComponent<ICarnivoraEdible> ();
				currentFood.Carnivora_GetReadyToBeEaten ();
				Attack (col.transform);
			} else if (col.GetComponentInParent<ICarnivoraEdible> () != null) {
				currentFood = col.GetComponentInParent<ICarnivoraEdible> ();
				currentFood.Carnivora_GetReadyToBeEaten ();
				Attack (col.transform.parent);
			}
		}
	}

	protected override void OnTriggerStay (Collider col)
	{
		base.OnTriggerStay (col);

		//Este IF checa se a layer de 'col' é uma das layer contidas na layer mask fornecida e, se não for, return.
		if((foodMask.value & 1<<col.gameObject.layer) == 0){ //TODO: Descobrir o que diabos cada coisa dessa linha significa.
			return;
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
