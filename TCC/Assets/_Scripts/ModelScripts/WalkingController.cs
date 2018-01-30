using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent (typeof(PlayerWalkInput))]
public class WalkingController : MonoBehaviour {

	public float maxJumpHeight = 4;
	public float minJumpHeight = 1;
	public float timeToJumpApex = .4f;
	float accelerationTimeAirborne = .2f;
	float accelerationTimeGrounded = .02f;
	public float moveSpeed = 6;

	public float wallSlideSpeedMax = 3;

	float gravity;
	float maxJumpVelocity;
	float minJumpVelocity;
	[SerializeField]
	Vector3 velocity;
	float velocityXSmoothing;
	float velocityZSmoothing;

	Vector3 directionalInput;


	#region variables
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
	#endregion

	#region walkCtrl variables
	//Movement information
	bool isFlying;
	bool isOnLedge;
	[HideInInspector]
	public bool externalForceAdded;
	Vector3 externalForce;
	int flyStamina;
	float cameraRotation;
	float jumpTriggerStrength;
	float singPressTime;
	bool stopGravity = false;

	[HideInInspector]
	public bool canPlayStaccato = true;
	[HideInInspector]
	public bool canPlayFloreio = true;

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

	[HideInInspector]
	public bool playerInputStartGame;
	[HideInInspector]
	public bool playerCanMove;

	[HideInInspector]
	public bool holdHeight = false;

	[HideInInspector]
	public bool isFallingToDeath = false;
	#endregion

	private bool isGrounded;
	private bool collisionAbove;
	private bool holdingJump = false;
	private float secondJumpStrengthMultiplier = 0.9f;

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

		gravity = -(2 * maxJumpHeight) / Mathf.Pow (timeToJumpApex, 2);
		maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
		minJumpVelocity = Mathf.Sqrt (2 * Mathf.Abs (gravity) * minJumpHeight);

		holdOrientation = orientation;

		playerInputStartGame = false;

		bonusJumpParticle.SetActive (false);

		CalculateRaySpacing ();
	}

	void LateUpdate(){
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
		if (oldState != walkStates.CURR_HEIGHT_STATE) {
			canPlayStaccato = true;
			canPlayFloreio = true;
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
			jumpInertia += new Vector3(velocity.x, 0, velocity.z) + externalForce;
			jumpInertia = Vector3.ClampMagnitude (jumpInertia, moveSpeed * 1.5f);
			rb.velocity = new Vector3 (jumpInertia.x, velocity.y, jumpInertia.z);

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

			if (isFlying) //isFlying deve estar ativo durante um unico frame
				isFlying = false;

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

		if(walkStates.TOCANDO_FLOREIO || walkStates.TOCANDO_STACCATO || walkStates.TOCANDO_SUSTAIN)
			animCtrl.SetBool ("IsSinging", true);
		else
			animCtrl.SetBool ("IsSinging", false);


		if(hasBonusJump){
			bonusJumpParticle.SetActive (true);
		} else{
			bonusJumpParticle.SetActive (false);
		}

		//HandleWallSliding ();
	}

	#region Input Control

	public void SetDirectionalInput(Vector3 input){
		directionalInput = input;
	}

	public void OnJumpInputDown(){

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

			velocity.y = maxJumpVelocity;
			jumpInertia = velocity;
		} 
		else if (canFly && !stopGravity && (flyStamina > 0 || hasBonusJump))	//------ Se eu estou no ar e consigo voar -------------------------
		{
			isFlying = true;
			rb.velocity = Vector3.zero;
			velocity.y = maxJumpVelocity * secondJumpStrengthMultiplier;
			jumpInertia = velocity;
			if (flyStamina > 0) {
				flyStamina--;
			} else {
				hasBonusJump = false;
			}
		}
		else if (isFallingToDeath) {	//------ Se eu estou caindo sem chance de recuperar -------------------------------------------------------
			animCtrl.SetTrigger ("FakeFly");
		}

		animCtrl.SetBool ("IsFlying", isFlying);
	}

	public void OnJumpInputHold(){
		holdingJump = true;
	}

	public void OnJumpInputUp(){
		if (velocity.y > minJumpVelocity && velocity.y <= maxJumpVelocity) {
			velocity.y = minJumpVelocity;
		}
		holdingJump = false;
	}

	public void SetSanfonaStrength(float strength){
		if (!isGrounded)
			return;

		sanfonaStrength = strength;
		
		CalculateRaySpacing ();
	}

	public void OnFloreioInputDown(){		
		walkStates.TOCANDO_FLOREIO = true;
		canPlayFloreio = false;
		birdSingCtrl.StartClarinet_Floreio();
	}

	public void OnStaccatoInputDown(){
		walkStates.TOCANDO_STACCATO = true;
		canPlayStaccato = false;
		birdSingCtrl.StartClarinet_Staccato ();
	}

	public void SetCheatState(bool activateCheat){
		maxFlyStamina = (activateCheat) ? 10 : 1;
	}

	#endregion

	//	void HandleWallSliding () {
	//		bool wallSliding = false;
	//		if((controller.collisions.front) && !controller.collisions.below && velocity.y < 0){
	//			wallSliding = true;
	//
	//			if(velocity.y < -wallSlideSpeedMax){
	//				velocity.y = -wallSlideSpeedMax;
	//			}
	//		}
	//	}

	void CalculateVelocity () {
		float targetVelocityX = directionalInput.x * moveSpeed;
		float targetVelocityZ = directionalInput.z * moveSpeed;
		velocity.x = Mathf.SmoothDamp (velocity.x, targetVelocityX, ref velocityXSmoothing, (isGrounded) ? accelerationTimeGrounded : accelerationTimeAirborne);
		velocity.z = Mathf.SmoothDamp (velocity.z, targetVelocityZ, ref velocityZSmoothing, (isGrounded) ? accelerationTimeGrounded : accelerationTimeAirborne);

		if (!stopGravity)
			velocity.y += gravity * Time.deltaTime;

		if(directionalInput.x != 0 || directionalInput.z != 0){
			walkStates.IS_WALKING = true;
			anim.ChangeForward(directionalInput.normalized);
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
		velocity = force;
	}

	public void ChangeJumpHeight (float newMaxHeight, float newMinHeight) {
		maxJumpHeight = newMaxHeight;
		minJumpHeight = newMinHeight;

		gravity = -(2 * maxJumpHeight) / Mathf.Pow (timeToJumpApex, 2);
		maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
		minJumpVelocity = Mathf.Sqrt (2 * Mathf.Abs (gravity) * minJumpHeight);
	}

	public void ChangeOrientationToCamera(Transform t, bool changedCam){
		if (changedCam && velocity != Vector3.zero && !automaticOrientation)
			holdOrientation = t;
		else if(holdOrientation == orientation || automaticOrientation)
			orientation = t;
	}

	public struct WalkingStates
	{
		public bool IS_WALKING;
		public bool IS_GROUNDED;
		public bool IS_GLIDING;
		public bool IS_FALLING_MAX;
		public HeightState CURR_HEIGHT_STATE;
		public bool TOCANDO_SUSTAIN;
		public bool TOCANDO_STACCATO; //Fica true apenas no frame que começou a tocar
		public bool TOCANDO_FLOREIO; //Fica true apenas no frame que começou a tocar
	}
}
