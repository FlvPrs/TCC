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

	public Animator animCtrl;

	public AudioSource sustainAudioSource;
	public AudioClip abrindo_Clip, temaAberta_Clip, temaIrritada_Clip, bote_Clip, temaComendo_Clip, fxComendo_Clip, cuspe_Clip, cuspeForte_Clip;

	protected override void Awake ()
	{
		base.Awake ();

		foodContainer = plantaTransform.Find ("FoodContainer");
		foodContainer.gameObject.SetActive (false);
	}

	void Start (){
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

		if (isBroto || isMurcha) {
			sustainAudioSource.Stop ();
			return;
		}

		if(!fechada && canChangeSimpleClip){
			if (currentState != Planta_CurrentState.Irritado) {
				if (sustainAudioSource.clip != temaAberta_Clip || (!sustainAudioSource.isPlaying)) {
					sustainAudioSource.clip = temaAberta_Clip;
					sustainAudioSource.Play ();
				}
			} else {
				if (sustainAudioSource.clip != temaIrritada_Clip || !sustainAudioSource.isPlaying) {
					sustainAudioSource.clip = temaIrritada_Clip;
					sustainAudioSource.Play ();
				}
			}
		} else if (comendo && canChangeSimpleClip) {
			if (sustainAudioSource.clip != temaComendo_Clip || !sustainAudioSource.isPlaying) {
				sustainAudioSource.clip = temaComendo_Clip;
				sustainAudioSource.Play ();
			}
		} else {
			sustainAudioSource.Stop ();
		}

		animCtrl.SetBool ("fechada", fechada);
		foodContainer.gameObject.SetActive (comendo);

		//Debug.DrawRay (transform.position, plantaTransform.up * 10f);

		#region DELETAR
		if (comendo) {
			if(Input.GetKeyDown(KeyCode.Keypad0)){
				ReleaseFood ();
			} else if (Input.GetKeyDown(KeyCode.Keypad1)) {
				ShootFood ();
			}
		}
		#endregion
	}

	protected override void CrescerBroto ()
	{
		//base.CrescerBroto ();
		simpleAudioSource.clip = cresceBroto_Clip;
		simpleAudioSource.Play ();
		StartCoroutine(WaitForSimpleClipToEnd (cresceBroto_Clip.length));
		MDL_Broto.SetActive (false);
		MDL_Crescida.SetActive (true);
		MDL_Murcha.SetActive (false);
		isBroto = false;
		isMurcha = false;
	}

	void Attack (Transform food){
		simpleAudioSource.clip = bote_Clip;
		simpleAudioSource.Play ();
		StartCoroutine(WaitForSimpleClipToEnd (bote_Clip.length));

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
		fechada = false;
		comendo = true; //TODO: DEIXAR FALSE
		currentFood.Carnivora_Release ();
		currentFood = null;

		simpleAudioSource.clip = cuspe_Clip;
		simpleAudioSource.Play ();
		//StartCoroutine(WaitForSimpleClipToEnd (cuspe_Clip.length));
	}

	void ShootFood (){
		fechada = comendo = false;
		currentFood.Carnivora_Shoot (foodDir * facingDirection * shootingStrength);
		currentFood = null;

		simpleAudioSource.clip = cuspeForte_Clip;
		simpleAudioSource.Play ();
		StartCoroutine(WaitForSimpleClipToEnd (cuspeForte_Clip.length + 0.1f));
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

		simpleAudioSource.clip = bote_Clip;
		simpleAudioSource.Play ();

		fechada = true;

		base.Dormir ();
	}
	protected override void Acordar ()
	{
		//Se eu estou aqui é pq eu ja sei que (currentState == Dormindo).

		fechada = false;

		base.Acordar ();
	}

	protected override void ChamarAtencao ()
	{
		base.ChamarAtencao ();

		if(fechada){
			simpleAudioSource.clip = abrindo_Clip;
			simpleAudioSource.Play ();
		}

		fechada = false;
	}

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
			ReleaseFood ();
		}
	}


	protected override void OnTriggerEnter (Collider col)
	{
		if (isBroto || isMurcha) {
			return;
		}

		base.OnTriggerEnter (col);

		//Este IF checa se a layer de 'col' é uma das layer contidas na layer mask fornecida e, se não for, return.
		if((foodMask.value & 1<<col.gameObject.layer) == 0){ //TODO: Descobrir o que diabos cada coisa dessa linha significa.
			return;
		}

		if (!fechada && !comendo && (currentState == Planta_CurrentState.DefaultState || currentState == Planta_CurrentState.Irritado)) {
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
		if (isBroto || isMurcha) {
			return;
		}

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
