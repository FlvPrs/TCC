using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent (typeof(PlayerWalkInput))]
public class WalkingController : MonoBehaviour {

	public float maxJumpHeight = 4;
	public float minJumpHeight = 1;
	public float timeToJumpApex = .4f;
	float accelerationTimeAirborne = .3f;
	float accelerationTimeGrounded = .02f;
	public float moveSpeed = 10;
	public float TimeToFall = 0.5f;

	public float wallSlideSpeedMax = 3;

	//ErickHiga Variables
	private float timerSecondJumpPower = 5.0f;
	public float fruitJumpPower;
	private bool startCDBonusJump;
	private bool CDBonusJump = false;
	[HideInInspector]
	public bool hasBonusJump_2;///erick higa teste poder temporario

	float gravity;
	float maxJumpVelocity;
	float minJumpVelocity;
	[SerializeField]
	Vector3 velocity;
	float velocityXSmoothing;
	float velocityZSmoothing;

	Vector3 directionalInput;
	//Vector3 rawInput;

	float sanfonaStrength;
	Vector3 jumpInertia;

	public bool automaticOrientation;
	Transform holdOrientation;
	[HideInInspector]
	public Transform orientation;
	public Transform collTransform;

	Transform myT;
	Rigidbody rb;
	[SerializeField]
	BoxCollider coll;
	HUDScript hudScript;
	Transform playerT;
	BirdStatureCtrl birdHeightCtrl;
	BirdSingCtrl birdSingCtrl;
	public Animator animCtrl;

	//PlayerPrefs.SetInt "health",100;
	//Movement information
	bool startedFly;
	bool isFlying;
	bool isOnLedge;
	[HideInInspector]
	public bool externalForceAdded;
	Vector3 externalForce;
	int flyStamina;
	float cameraRotation;
	float jumpTriggerStrength;
	public float singHoldTreshold = 0.24f;
	public float singHoldTime;
	bool stopGravity = false;

	//[HideInInspector]
	public bool canStartSing = true;

	float timeOnAir = 0f;

	//Settings
	[Range (0.4f, 7f)]
	public float glideStrength = 5f;
	[Range (0, 1)]
	public float aerialCtrl = 0.1f;

	public int maxFlyStamina = 4;

	public float maxFallVelocity = 70f;

	bool canFly;

	[HideInInspector]
	public float currentFallVelocity;

	public LayerMask raycastMask = -1;

	public WalkingStates walkStates;

	private AnimationForward anim;

	public GameObject asas;
	public GameObject bonusJumpParticle;

	[HideInInspector]
	public bool hasBonusJump;

	private float SpeedMovementSlowTimer;
	private bool VelocidadeDiminuidaNoVoo;

	[HideInInspector]
	public bool playerInputStartGame;
	[HideInInspector]
	public bool playerCanMove;

	[HideInInspector]
	public bool holdHeight = false;

	[HideInInspector]
	public bool isFallingToDeath = false;

	private bool isGrounded;
	private bool collisionAbove;
	private bool holdingJump = false;
	public float secondJumpStrengthMultiplier = 0.9f;

	//Raycast settings
	const float skinWidth = .03f;
	const float distBetweenRays = .25f;
	int verticalRayCount;
	float verticalRaySpacing;
	Transform bottomFrontLeft;
	Transform topFrontLeft;


	float raycastMoveDistance;
	public float maxSlopeAngle = 50;

	bool holdVelocity;


	void Awake(){
		rb = GetComponent<Rigidbody> ();
		myT = GetComponent<Transform> ();
		playerT = myT.Find ("PlayerCharacter").GetComponent <Transform> ();
		birdHeightCtrl = GetComponent<BirdStatureCtrl> ();
		birdSingCtrl = GetComponent<BirdSingCtrl> ();
		hudScript = GameObject.FindObjectOfType<HUDScript> ();
		anim = GetComponentInChildren<AnimationForward> ();

		bottomFrontLeft = collTransform.Find ("bottomFrontLeft").GetComponent<Transform> ();
		topFrontLeft = collTransform.Find ("topFrontLeft").GetComponent<Transform> ();

		if (orientation == null)
			orientation = myT;

		playerCanMove = true; //Por padrão, pode mover. Porem, outro script pode alterar seu valor.
	}

	void Start () {
		isGrounded = true;
		collisionAbove = false;
		flyStamina = maxFlyStamina;
		maxFallVelocity = -maxFallVelocity;
		startCDBonusJump = false;
		CDBonusJump = false;

		hasBonusJump_2 = false;
		fruitJumpPower = 0.75f;

		gravity = -(2 * maxJumpHeight) / Mathf.Pow (timeToJumpApex, 2);
		maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
		minJumpVelocity = Mathf.Sqrt (2 * Mathf.Abs (gravity) * minJumpHeight);

		holdOrientation = orientation;

		playerInputStartGame = false;

		bonusJumpParticle.SetActive (false);

		CalculateRaySpacing ();
	}


	public bool TOCANDO;
	public bool SEGURANDO;

	void FixedUpdate(){
		TOCANDO = walkStates.TOCANDO_NOTAS;
		SEGURANDO = walkStates.SEGURANDO_NOTA;

		if (isGrounded) {
			startCDBonusJump = false;
			//print ("estou no chao");
		}

		//print (startCDBonusJump);
		#region power up fruit limitado
		if(hasBonusJump_2){
			if (startCDBonusJump) {
				CDBonusJump = true;
			}
			fruitJumpPower = 1.3f;
			if(CDBonusJump){
				timerSecondJumpPower -= 1 *Time.deltaTime;
				//print("comecou contagem regressiva");
			}
			if(timerSecondJumpPower <= 0){
				timerSecondJumpPower = 5.0f;
				hasBonusJump_2 = false;
				fruitJumpPower = 0.75f;
				CDBonusJump = false;
				//print("acabou contagem");
			}
		}
		#endregion

		#region Reduzindo Velocidade No Pulo
		if(VelocidadeDiminuidaNoVoo){
			if(secondJumpStrengthMultiplier > 0.2f){
				moveSpeed = 15.0f;
				SpeedMovementSlowTimer += 1*Time.deltaTime;
			}else{
				moveSpeed = 10.0f;
			}
			if(SpeedMovementSlowTimer >= 0.2f){
				SpeedMovementSlowTimer=0;
				moveSpeed = 10.0f;
				VelocidadeDiminuidaNoVoo = false;
			}
		}

		#endregion

		if(!holdVelocity)
			CalculateVelocity ();

		isGrounded = Grounded (out collisionAbove);

		if (velocity.y <= 0 && isGrounded) {
			velocity = Vector3.up * velocity.y; //Pq o movimento é a partir da animação
		}

		if (directionalInput == Vector3.zero) {
			if (holdOrientation != orientation) {
				orientation = holdOrientation;
			}
		}

		animCtrl.SetBool ("isWalking", walkStates.IS_WALKING);

		//TODO: Provavelmente tem um jeito facil bem mais otimizado de fazer isso...
		HeightState oldState = walkStates.CURR_HEIGHT_STATE;
		walkStates.CURR_HEIGHT_STATE = birdHeightCtrl.currentHeightState;
		if(walkStates.TOCANDO_NOTAS){
			if (oldState != walkStates.CURR_HEIGHT_STATE && singHoldTime > 0f) {
				if (walkStates.SEGURANDO_NOTA) {
					GetComponent<FMODFilhoImplementacao> ().canStartSustain = true;
					birdSingCtrl.SustainNote ();
				} else {
					FindObjectOfType<PlayerWalkInput> ().singPressTime = 0f;
					GetComponent<FMODFilhoImplementacao> ().canStartStaccato = true;
					birdSingCtrl.SingNote ();
				}
			}
		}

		walkStates.IS_GROUNDED = isGrounded;
		animCtrl.SetBool ("isGrounded", walkStates.IS_GROUNDED);


		if(isGrounded) {//-------------Se eu estou no chão--------------------------
			rb.velocity = velocity;
			jumpInertia = Vector3.zero;

			asas.SetActive (false);
			canFly = false;
			animCtrl.SetTrigger ("CanJump");
			if(!holdHeight)
				birdHeightCtrl.UpdateHeight (sanfonaStrength, animCtrl);
			timeOnAir = 0f;

		} 
		else {		//--------------Se eu não estou no chão---------------------		
			jumpInertia += new Vector3(velocity.x * aerialCtrl, 0, velocity.z * aerialCtrl);
			jumpInertia = Vector3.ClampMagnitude (jumpInertia, moveSpeed);
			rb.velocity = new Vector3 (jumpInertia.x, velocity.y, jumpInertia.z);

			//Debug.DrawRay (myT.position, rb.velocity.normalized * 5, Color.magenta);

			if (secondJumpStrengthMultiplier > 0.0f) {
				secondJumpStrengthMultiplier -= 0.3f*Time.deltaTime;
			} else {
				secondJumpStrengthMultiplier = 0.0f;
			}
			//print (secondJumpStrengthMultiplier);

			#region Pulo Duplo limiter
			if (timeOnAir >= 0.15f) { //Só permite pulo duplo após 0.15s no ar
				canFly = true;
				asas.SetActive (true);
				if (flyStamina > 0 || hasBonusJump) {
					animCtrl.SetTrigger ("CanFly");
				}
				timeOnAir = 0.15f;
			} else {
				timeOnAir += Time.deltaTime;
			}
			#endregion

			//TODO: Erick deixou comentado. Manter?
			if (startedFly) { //startedFly deve estar ativo durante um unico frame
				startedFly = false;
				//animCtrl.SetBool ("IsFlying", startedFly);
			}
			//

			if (!holdHeight) {
				sanfonaStrength = 0;
				CalculateRaySpacing ();
				birdHeightCtrl.UpdateHeight (sanfonaStrength, animCtrl); //Não permite mudar altura no ar
			}
		}
		//------------------------------------------------------------------

		#region Glide Ctrl
		bool isGliding = false;

		if (!isGrounded && rb.velocity.y < 0 && holdingJump) {	//Queda com Glide
			isGliding = true;
			velocity += Vector3.up * Mathf.Abs (velocity.y) * 4f * glideStrength * Time.deltaTime;
		}

		walkStates.IS_GLIDING = isGliding;
		animCtrl.SetBool ("IsGliding", isGliding);

		if(!isGrounded && !isGliding)
			animCtrl.SetTrigger ("CanStartGlide");
		#endregion

		walkStates.IS_FALLING_MAX = LimitFallVelocity ();

		currentFallVelocity = rb.velocity.y;


		if(rb.velocity.y >= 0.05f)
			animCtrl.SetTrigger ("CanBeginFall");

		animCtrl.SetFloat ("VelocityY", currentFallVelocity);

		if(walkStates.TOCANDO_NOTAS)
			animCtrl.SetBool ("IsSinging", true);
		else
			animCtrl.SetBool ("IsSinging", false);


		if(hasBonusJump){
			bonusJumpParticle.SetActive (true);
		} else{
			bonusJumpParticle.SetActive (false);
		}

		print (Mathf.Abs(Vector3.Dot(Camera.main.transform.forward, Vector3.down)));
	}

	#region Input Control

	public void SetDirectionalInput(Vector3 input){
		//directionalInput = orientation.TransformVector(input);

		Vector3 orientationForward;
		float dot = Vector3.Dot(Camera.main.transform.forward, Vector3.down);
		if (dot != 0f)
			orientationForward = (Mathf.Abs (dot) <= 0.7f) ? Camera.main.transform.forward : (dot > 0f) ? Camera.main.transform.up : -Camera.main.transform.up;
		else
			orientationForward = Vector3.forward;
		orientationForward = new Vector3 (orientationForward.x, 0, orientationForward.z);
		orientationForward = (orientationForward).normalized * input.z;

		Vector3 orientationRight = Camera.main.transform.right;
		orientationRight = new Vector3 (orientationRight.x, 0, orientationRight.z);
		orientationRight = (orientationRight).normalized * input.x;

		directionalInput = (orientationForward + orientationRight).normalized;
	}

	public void OnJumpInputDown(){

		startCDBonusJump = true;

		if(isGrounded)	//------ Se eu estou no chão ----------------------------------------------------------------------------------------------
		{
			//			if(controller.collisions.slidingDownMaxSlope){
			//				if(directionalInput.x != -Mathf.Sign(controller.collisions.slopeNormal.x)){ // not jumping against max slope
			//					velocity.y = maxJumpVelocity * controller.collisions.slopeNormal.y;
			//					velocity.x = maxJumpVelocity * controller.collisions.slopeNormal.x;
			//				}
			//			} 
			//			else {
			//				velocity.y = maxJumpVelocity;
			//			}

			secondJumpStrengthMultiplier = fruitJumpPower;
			velocity.y = maxJumpVelocity;
			jumpInertia = velocity;


		} 
		else if (canFly && !stopGravity && (flyStamina > 0 || hasBonusJump))	//------ Se eu estou no ar e consigo voar -------------------------
		{
			startedFly = true;
			animCtrl.SetTrigger ("StartFly");
			isFlying = true;
			rb.velocity = Vector3.zero;
			velocity.y = maxJumpVelocity * secondJumpStrengthMultiplier;
			jumpInertia = velocity;
			VelocidadeDiminuidaNoVoo = true;

			/* if (flyStamina > 0) {
				flyStamina--;
			} else {
				hasBonusJump = false;
			} */
		}
		else if (isFallingToDeath) {	//------ Se eu estou caindo sem chance de recuperar -------------------------------------------------------
			animCtrl.SetTrigger ("FakeFly");
		}

		animCtrl.SetBool ("IsFlying", startedFly);
	}

	public void OnJumpInputHold(){
		holdingJump = true;
	}

	public void OnJumpInputUp(){
		if (velocity.y > minJumpVelocity && velocity.y <= maxJumpVelocity && !startedFly) {
			velocity.y = minJumpVelocity;
		}
		holdingJump = false;

		if(isFlying){
			isFlying = false;
		}
	}

	public void SetSanfonaStrength(float strength){
		if (!isGrounded)
			return;

		sanfonaStrength = strength;

		CalculateRaySpacing ();
	}

	public void OnSingInputDown(){
		walkStates.TOCANDO_NOTAS = true;
		birdSingCtrl.SingNote();
	}
	public void OnSingInputHold(){ //É sempre chamada apenas 1x, antes de <singleNoteMinimumDuration>s, se o player estiver segurando o botao de canto
		walkStates.SEGURANDO_NOTA = true;
		birdSingCtrl.SustainNote();
	}
	//	public void OnSingInputUp(){
	//		walkStates.SEGURANDO_NOTA = false;
	//		//canStartSing = true;
	//		//birdSingCtrl.StopSing();
	//	}

	public void SetCheatState(bool activateCheat){
		maxFlyStamina = (activateCheat) ? 10 : 1;
	}

	#endregion

	void CalculateVelocity () {
		float targetVelocityX = directionalInput.x * moveSpeed;
		float targetVelocityZ = directionalInput.z * moveSpeed;
		velocity.x = Mathf.SmoothDamp (velocity.x, targetVelocityX, ref velocityXSmoothing, (isGrounded) ? accelerationTimeGrounded : accelerationTimeAirborne);
		velocity.z = Mathf.SmoothDamp (velocity.z, targetVelocityZ, ref velocityZSmoothing, (isGrounded) ? accelerationTimeGrounded : accelerationTimeAirborne);

		if (!stopGravity)
			velocity.y += gravity * Time.deltaTime;

		if(directionalInput.x != 0 || directionalInput.z != 0){
			walkStates.IS_WALKING = true;
			//			if(isGrounded)
			anim.ChangeForward(directionalInput.normalized);
			//			else
			//				anim.ChangeForward ((myT.forward + rb.velocity).normalized);
			Vector3 clampedAnimSpeed = new Vector3 (directionalInput.x, 0, directionalInput.z);
			clampedAnimSpeed = Vector3.ClampMagnitude (clampedAnimSpeed, 1f);
			animCtrl.SetFloat ("WalkVelocity", clampedAnimSpeed.magnitude);
		} else {
			walkStates.IS_WALKING = false;
		}
	}

	bool LimitFallVelocity () {
		bool isFallingHard = false;

		if (!isGrounded) {
			if(rb.velocity.y < maxFallVelocity){
				Vector3 clampedVelocity = rb.velocity;
				clampedVelocity.y = maxFallVelocity;
				rb.velocity = clampedVelocity;
				isFallingHard = true;
			}
		}
		return isFallingHard;
	}


	bool Grounded(out bool collAbove){
		bool collBelow = false;
		collAbove = false;

		float lowestAngle = 360f;
		bool climbingSlope = false;
		bool canStopGravity = false;
		Vector3 normalDir = Vector3.zero;

		float directionY = Mathf.Sign (velocity.y);
		float rayLength = 1f + skinWidth;

		for (int i = 0; i < verticalRayCount; i++) {
			for (int j = 0; j < verticalRayCount; j++) {
				Vector3 rayOrigin = (directionY == -1) ? bottomFrontLeft.position : topFrontLeft.position;
				rayOrigin += bottomFrontLeft.right * (verticalRaySpacing * j);
				rayOrigin -= bottomFrontLeft.forward * (verticalRaySpacing * i);
				RaycastHit hit;
				bool hitSomething = Physics.Raycast (rayOrigin, myT.up * directionY, out hit, rayLength, raycastMask);

				Debug.DrawRay (rayOrigin, myT.up * directionY * rayLength, Color.red);

				if(hitSomething){
					float slopeAngle = Vector3.Angle (hit.normal, Vector3.up);
					//Debug.DrawRay (hit.point, hit.normal * 5, Color.black);
					if (slopeAngle < lowestAngle) {
						lowestAngle = slopeAngle;
						normalDir = hit.normal;
					}

					raycastMoveDistance = hit.distance - skinWidth;

					if(Mathf.Abs(raycastMoveDistance) <= 0.05f){ //Se deu hit mas ainda estou longe, mantenha a velocidade Y. Caso contrario...
						//float timeBeforeStop = raycastMoveDistance / velocity.y;
						velocity.y = 0;
					}

					bool collNotGround = false;

					if(hit.transform.gameObject.layer == 16){ //Se for a layer NotGround, reseta gravidade e não considera chão.
						collBelow = false;
						collAbove = false;
						collNotGround = true;
						ForceGravity(velocity.y);
					}

					if (!collNotGround) { 
						collBelow = (directionY == -1);
						collAbove = (directionY == 1);
					}
				}
			}
		}

		if(lowestAngle >= maxSlopeAngle){
			collBelow = false;
			Vector3 refVector = Vector3.Cross (normalDir, Vector3.up);
			Vector3 perpendicular = Vector3.Cross(normalDir, refVector);
			rb.AddForce (perpendicular * 2f, ForceMode.VelocityChange);
			Debug.DrawRay (myT.position, perpendicular * 5, Color.magenta);
		}

		if(collBelow)
			ResetFlyStamina ();
		if (collAbove)
			ResetGravity ();

		return collBelow;
	}

	void ForceGravity(float newYVelocity){
		velocity.y = newYVelocity;
	}
	void ResetGravity(float gravMultiplier = 1f){
		velocity.y = gravity * gravMultiplier * Time.deltaTime;
	}

	void CalculateRaySpacing(){
		float boundsWidth = coll.size.x - (skinWidth * 2);

		verticalRayCount = Mathf.RoundToInt (boundsWidth / distBetweenRays);
		verticalRaySpacing = boundsWidth / (verticalRayCount -1);

		bottomFrontLeft.localPosition = new Vector3 (-0.5f + sanfonaStrength/4f + skinWidth, skinWidth, 0.5f - sanfonaStrength/4f - skinWidth);
	}

	public void SetVelocityTo(Vector3 newVelocity, bool hold){
		velocity = newVelocity;
		rb.velocity = jumpInertia = velocity;
		holdVelocity = hold;
	}

	public void BypassGravity(bool stopGrav){
		if (stopGrav) {
			rb.velocity = Vector3.zero;
			velocity.y = 0f;
		}

		stopGravity = stopGrav;
	}

	public void ResetFlyStamina(){
		flyStamina = maxFlyStamina;
	}

	public void ContinuousExternalForce(Vector3 force, bool ignoreGravity, bool ignoreInput){
		if(ignoreGravity){
			BypassGravity (true);
		}
		if(ignoreInput){
			velocity = Vector3.zero;
			jumpInertia = Vector3.zero;
		} else {
			jumpInertia = jumpInertia * 0.6f;
		}

		rb.AddForce (force, ForceMode.Force);

		if (!holdingJump) {
			Vector3 counterForce = -Vector3.up * (force.magnitude * 0.2f);
			rb.AddForce (counterForce, ForceMode.Force);
		}
	}

	public void AddExternalForce(Vector3 force, float duration){
		externalForceAdded = true;
		//velocity = force;
		SetVelocityTo(force, false);
		secondJumpStrengthMultiplier = fruitJumpPower + 0.15f;
	}

	public void ChangeJumpHeight (float newMaxHeight, float newMinHeight) {
		maxJumpHeight = newMaxHeight;
		minJumpHeight = newMinHeight;

		gravity = -(2 * maxJumpHeight) / Mathf.Pow (timeToJumpApex, 2);
		maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
		minJumpVelocity = Mathf.Sqrt (2 * Mathf.Abs (gravity) * minJumpHeight);
	}

	public void ChangeOrientationToCamera(Transform t, bool changedCam){
		
		Transform newT = t;
		newT.eulerAngles = new Vector3 (0, newT.eulerAngles.y, 0);

		if (changedCam && velocity != Vector3.zero && !automaticOrientation)
			holdOrientation = newT;
		else if(holdOrientation == orientation || automaticOrientation)
			orientation = newT;
	}

	public struct WalkingStates
	{
		public bool IS_WALKING;
		public bool IS_GROUNDED;
		public bool IS_GLIDING;
		public bool IS_FALLING_MAX;
		public HeightState CURR_HEIGHT_STATE;
		public bool TOCANDO_NOTAS; //Fica true apenas no frame que começou a tocar
		public bool SEGURANDO_NOTA;
		public PlayerSongs CURR_SONG;
	}
}