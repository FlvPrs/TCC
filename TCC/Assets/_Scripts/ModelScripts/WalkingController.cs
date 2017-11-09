﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FacingDirection{
	North,
	East,
	South,
	West
}


public class WalkingController : Controller {

	//Movement information
	Vector3 walkVelocity;
	FacingDirection facing = FacingDirection.South;
	float adjVertVelocity;
	float jumpPressTime;
	Vector3 jumpInertia;
	bool isFlying;
	bool isOnLedge;
	[HideInInspector]
	public bool externalForceAdded;
	Vector3 externalForce;
	int flyStamina;
	float cameraRotation;
	float sanfonaStrength;
	float jumpTriggerStrength;
	float singPressTime;
	bool stopGravity;
	Transform holdOrientation;
	Transform orientation;

	float maxJumpCooldown = 0.2f;
	float jumpCooldown = 0f;

	float maxClimbAngle = 50f;
	bool isClimbing = false;

	//Settings
	public float walkSpeed = 5f;
	public float jumpSpeed = 8.3f;
	public float interactDuration = 0.1f;
	public float attackDamage = 5f;
	public float gravity = 10.0f;
	public float fallMultiplier = 2.5f;
	//public float lowJumpMultiplier = 2f;
	[Range (0.4f, 7f)]
	public float glideStrength = 5f;
	[Range (0, 1)]
	public float aerialCtrl = 0.1f;

	public int maxFlyStamina = 4;

	public float maxFallVelocity = 70f;
	[HideInInspector]
	public float currentFallVelocity;

	public LayerMask raycastMask = -1;

	public WalkingStates walkStates;

	//Delegates and events
	public delegate void FacingChangeHandler (FacingDirection fd);
	public static event FacingChangeHandler OnFacingChange;
//	public delegate void HitboxEventHandler (float dur, float sec, ActionType act);
//	public static event HitboxEventHandler OnInteract;

	private AnimationForward anim;

	public GameObject asas;

	protected override void Start() {
		base.Start ();
		if(OnFacingChange != null){
			OnFacingChange (facing);
		}
		flyStamina = maxFlyStamina;
		maxFallVelocity = -maxFallVelocity;

		if (orientation == null)
			orientation = transform;

		holdOrientation = orientation;

		anim = GetComponentInChildren<AnimationForward> ();
	}

	public override void ReadInput (InputData data) {

		//prevWalkVelocity = walkVelocity;
		ResetMovementToZero ();

		bool axis0 = false;
		bool axis1 = false;

		//Set vertical movement
		if(data.axes[0] != 0f){
			walkVelocity += orientation.forward * data.axes [0] * walkSpeed;
			axis0 = true;
		}

		//Set horizontal movement
		if(data.axes[1] != 0f){
			walkVelocity += orientation.right * data.axes [1] * walkSpeed;
			axis1 = true;
		}

		//Set Sanfona
		if(data.axes[2] != 0f){
			sanfonaStrength = data.axes [2];
		}

		//Set camera rotation
//		if(data.axes[3] != 0f){
//			cameraRotation = data.axes [3];
//		}

		//Check vertical Jump on Controller
//		if(data.axes[4] != 0f){
//			jumpTriggerStrength = data.axes [4];
//			if (jumpPressTime == 0f) {
//				if (Grounded() && jumpCooldown <= 0f) {
//					adjVertVelocity = jumpSpeed;
//					jumpCooldown = maxJumpCooldown;
//					jumpInertia = walkVelocity;
//				} else if (!stopGravity && flyStamina > 0) {
//					isFlying = true;
//					adjVertVelocity = jumpSpeed;
//					jumpInertia = walkVelocity;
//					flyStamina--;
//				}
//			}
//			jumpPressTime += Time.deltaTime;
//		} else {
//			jumpPressTime = 0f;
//		}
		if(data.buttons[0]){
			jumpTriggerStrength = 1f;
			if (jumpPressTime == 0f) {
				if (Grounded() && jumpCooldown <= 0f) {
					adjVertVelocity = jumpSpeed;
					jumpCooldown = maxJumpCooldown;
					jumpInertia = walkVelocity;
				} else if (!stopGravity && !isClimbing && flyStamina > 0) {
					isFlying = true;
					adjVertVelocity = jumpSpeed;
					jumpInertia = walkVelocity;
					flyStamina--;
				}
			}
			jumpPressTime += Time.deltaTime;
		} else {
			jumpPressTime = 0f;
		}

		//Check Sing on Controller
		if(data.axes[5] != 0){
			if(singPressTime == 0f){
				birdSingCtrl.StartClarinet (true, data.axes [5]);
			}
			birdSingCtrl.UpdateSoundVolume (data.axes [5]);
			singPressTime += Time.deltaTime;
		} else {
			singPressTime = 0f;
			birdSingCtrl.StartClarinet (false, 0);
		}

//		//Check if Interact Button is pressed
//		if(data.buttons[1] == true){
//			if(OnInteract != null){
//				OnInteract (interactDuration, 0, ActionType.Interact);
//			}
//		}
//
//		//Check if Attack Button is pressed
//		if(data.buttons[2] == true){
//			if(OnInteract != null){
//				OnInteract (interactDuration, attackDamage, ActionType.Attack);
//			}
//		}

		//Change facing
		if (axis0 || axis1) {
			walkStates.IS_WALKING = true;
			//ChangeFacing (axis0, axis1, data);
			anim.ChangeForward(walkVelocity.normalized);
		} else {
			walkStates.IS_WALKING = false;
		}

		newInput = true;

	}

	//Method that will look below our character and see if there is a collider
	bool Grounded(){
		RaycastHit[] hit = new RaycastHit[9];

		bool ray1 = Physics.Raycast(myT.position + myT.up * 0.1f, Vector3.down, out hit[0], 0.25f, raycastMask);
		Debug.DrawRay (myT.position + myT.up * 0.1f, Vector3.down * (0.25f));

		bool ray2 = Physics.Raycast (myT.position + myT.up * 0.1f + (Vector3.Scale (myT.forward / 2, myT.localScale)), Vector3.down, out hit[1], 0.25f, raycastMask);
		Debug.DrawRay (myT.position + myT.up * 0.1f + (Vector3.Scale (myT.forward / 2, myT.localScale)), Vector3.down * (0.25f));
		bool ray3 = Physics.Raycast(myT.position + myT.up * 0.1f - (Vector3.Scale (myT.forward / 2, myT.localScale)), Vector3.down, out hit[2], 0.25f, raycastMask);
		Debug.DrawRay (myT.position + myT.up * 0.1f - (Vector3.Scale (myT.forward / 2, myT.localScale)), Vector3.down * (0.25f));
		bool ray4 = Physics.Raycast(myT.position + myT.up * 0.1f + (Vector3.Scale (myT.right / 2, myT.localScale)), Vector3.down, out hit[3], 0.25f, raycastMask);
		Debug.DrawRay (myT.position + myT.up * 0.1f + (Vector3.Scale (myT.right / 2, myT.localScale)), Vector3.down * (0.25f));
		bool ray5 = Physics.Raycast(myT.position + myT.up * 0.1f - (Vector3.Scale (myT.right / 2, myT.localScale)), Vector3.down, out hit[4], 0.25f, raycastMask);
		Debug.DrawRay (myT.position + myT.up * 0.1f - (Vector3.Scale (myT.right / 2, myT.localScale)), Vector3.down * (0.25f));

		bool ray6 = Physics.Raycast (myT.position + myT.up * 0.1f + (Vector3.Scale ((myT.forward - myT.right) / 2, myT.localScale)), Vector3.down, out hit[5], 0.25f, raycastMask);
		Debug.DrawRay (myT.position + myT.up * 0.1f + (Vector3.Scale ((myT.forward - myT.right) / 2, myT.localScale)), Vector3.down * (0.25f));
		bool ray7 = Physics.Raycast (myT.position + myT.up * 0.1f - (Vector3.Scale ((myT.forward - myT.right) / 2, myT.localScale)), Vector3.down, out hit[6], 0.25f, raycastMask);
		Debug.DrawRay (myT.position + myT.up * 0.1f - (Vector3.Scale ((myT.forward - myT.right) / 2, myT.localScale)), Vector3.down * (0.25f));
		bool ray8 = Physics.Raycast (myT.position + myT.up * 0.1f + (Vector3.Scale ((myT.right + myT.forward) / 2, myT.localScale)), Vector3.down, out hit[7], 0.25f, raycastMask);
		Debug.DrawRay (myT.position + myT.up * 0.1f + (Vector3.Scale ((myT.right + myT.forward) / 2, myT.localScale)), Vector3.down * (0.25f));
		bool ray9 = Physics.Raycast (myT.position + myT.up * 0.1f - (Vector3.Scale ((myT.right + myT.forward) / 2, myT.localScale)), Vector3.down, out hit[8], 0.25f, raycastMask);
		Debug.DrawRay (myT.position + myT.up * 0.1f - (Vector3.Scale ((myT.right + myT.forward) / 2, myT.localScale)), Vector3.down * (0.25f));

//		for (int i = 0; i < hit.Length; i++) {
//			Debug.DrawRay (hit [i].point, hit [i].normal, Color.blue);
//		}

		if (ray1 || ray2 || ray3 || ray4 || ray5 || ray6 || ray7 || ray8 || ray9) {
			bool climbing = false;
			//float angle = 0f;
			//Vector3 dir = Vector3.zero;
			for (int i = 0; i < hit.Length; i++) {
				if (hit [i].normal == Vector3.zero)
					continue;

				if (!hit [i].collider.CompareTag("Untagged")){
					climbing = false;
					break;
				}
				
				float slopeAngle = Vector3.Angle (hit [i].normal, Vector3.up);
				if(slopeAngle >= maxClimbAngle){
					climbing = true;
//					if (slopeAngle > angle) {
//						angle = slopeAngle;
//						//dir = hit [i].normal;
//					}
				} else if(slopeAngle < maxClimbAngle) {
					climbing = false;
					break;
				}
			}

			isClimbing = climbing;

			if (climbing) {
				//rb.AddForce (dir * rb.mass * 2f);
				//rb.AddForce (new Vector3 (0, -gravity * rb.mass * 100f/angle, 0));
				return false;
			}

			flyStamina = maxFlyStamina;
			hudScript.UpdateWingUI (false, flyStamina);
			return true;
		}

		isClimbing = false;
		hudScript.UpdateWingUI (true, flyStamina);
		return false;
	}

	void FixedUpdate(){
		//float cameraY = Input.GetAxis ("Mouse X") * GameConstants.MOUSE_SENSITIVITY * Time.deltaTime; 
		//float cameraY = cameraRotation * GameConstants.MOUSE_SENSITIVITY * Time.deltaTime;

		//myT.Rotate (0, cameraY, 0);

		if(jumpCooldown > 0){
			jumpCooldown -= Time.deltaTime;
		}
	}

	//Always called after Updates are called
	void LateUpdate() {

		// if(!newInput || isClimbing){
		if(!newInput){
			//prevWalkVelocity = walkVelocity;
			ResetMovementToZero ();
			jumpPressTime = 0f;
			singPressTime = 0f;
			birdSingCtrl.StartClarinet (false, 0);
			walkStates.IS_WALKING = false;

			if(holdOrientation != orientation)
				orientation = holdOrientation;
		}


		if(isClimbing){
			jumpPressTime = 0f;
			adjVertVelocity = 0f;
		}


		animCtrl.SetBool ("isWalking", walkStates.IS_WALKING);

		birdHeightCtrl.UpdateHeight (sanfonaStrength);
		walkStates.CURR_HEIGHT_STATE = birdHeightCtrl.currentHeightState;

		if(externalForceAdded){
			externalForceAdded = false;
			externalForce = Vector3.zero;
			//newInput = false;
			//return;
		}

		bool isGrounded = true;
		if (!Grounded())
			isGrounded = false;
		
		walkStates.IS_GROUNDED = isGrounded;
		animCtrl.SetBool ("isGrounded", walkStates.IS_GROUNDED);

		if(jumpPressTime > 0.2f && !isGrounded)
			asas.SetActive (true);
		else
			asas.SetActive (false);

		//print (isGrounded);
			
		if (!isGrounded) {
			jumpInertia += (walkVelocity * aerialCtrl) + externalForce;

			if(!walkStates.IS_GLIDING)
				jumpInertia = Vector3.ClampMagnitude (jumpInertia, walkSpeed);
			else
				jumpInertia = Vector3.ClampMagnitude (jumpInertia, walkSpeed * 1.5f);

			if (!isFlying)
				adjVertVelocity += rb.velocity.y;
			else
				isFlying = false;
			
			if (!isClimbing)
				rb.velocity = new Vector3 (jumpInertia.x, adjVertVelocity, jumpInertia.z);
			else {
				//rb.AddForce (new Vector3 (jumpInertia.x * 0.25f, adjVertVelocity, jumpInertia.z * 0.25f), ForceMode.Acceleration);
				rb.velocity = new Vector3 (jumpInertia.x * 0.35f, adjVertVelocity, jumpInertia.z * 0.35f);
				rb.AddForce (Vector3.up * -gravity);
			}
			
		} else {
			walkVelocity = Vector3.ClampMagnitude (walkVelocity, walkSpeed);
			rb.velocity = new Vector3 (walkVelocity.x, rb.velocity.y + adjVertVelocity, walkVelocity.z);
		}

		bool isGliding = false;

		if (rb.velocity.y < 0 && jumpPressTime == 0) { //Queda normal
			rb.velocity += Vector3.up * -gravity * (fallMultiplier - 1) * Time.deltaTime;
		} else if (rb.velocity.y < 0 && jumpPressTime > 0 && !isClimbing) {	//Queda com Glide
			isGliding = true;
			rb.velocity += Vector3.up * Mathf.Abs (rb.velocity.y) * glideStrength * jumpTriggerStrength * Time.deltaTime;
		} 
//		else if (rb.velocity.y > 0 && jumpPressTime == 0) {	//Pulo baixo (o pulo alto é o default)
//			rb.velocity += Vector3.up * -gravity * ((lowJumpMultiplier) - 1) * Time.deltaTime;
//		}

		walkStates.IS_GLIDING = isGliding;

		bool isFallingHard = false;

		//Aplicando a Gravidade
		if (!stopGravity) {
			if (!isGrounded) {
				if(rb.velocity.y < maxFallVelocity){
					Vector3 clampedVelocity = rb.velocity;
					clampedVelocity.y = maxFallVelocity;
					rb.velocity = clampedVelocity;
					isFallingHard = true;
				} else {
//					if(!isClimbing)
						rb.AddForce (new Vector3 (0, -gravity * rb.mass, 0));
//					else
//						rb.AddForce (new Vector3 (0, -gravity * rb.mass, 0), ForceMode.VelocityChange);
				}
			} 
//			else if (isGrounded && !newInput && rb.velocity.y < 0) {
//				rb.velocity = new Vector3 (rb.velocity.x, 0, rb.velocity.z);
//			}
		}
			
		walkStates.IS_FALLING_MAX = isFallingHard;

		currentFallVelocity = rb.velocity.y;

		newInput = false;
	}

	void ChangeFacing(bool axis0, bool axis1, InputData data){
//		if(axis0){
//			facing = (data.axes[0] > 0) ? FacingDirection.North : FacingDirection.South;
//		} else if(axis1){
//			facing = (data.axes[1] > 0) ? FacingDirection.East : FacingDirection.West;
//		}

		//Call change facing event
		if (OnFacingChange != null) {
			OnFacingChange (facing);
		}
	}

	public IEnumerator GrabLedge(Vector3 dir){
		if(dir.z != 0.0f){
			if (Mathf.Sign (walkVelocity.z) == Mathf.Sign(dir.z))
				isOnLedge = true;
		} else if (dir.x != 0.0f) {
			if(Mathf.Sign(walkVelocity.x) == Mathf.Sign(dir.x))
				isOnLedge = true;
		} else {
			StopCoroutine ("GrabLedge");
		}

		if (isOnLedge) {
			Vector3 up = Vector3.up;
			rb.velocity = Vector3.zero;
			rb.AddForce (up * 10, ForceMode.Impulse);

			yield return new WaitForSeconds (0.1f);
			rb.velocity = Vector3.zero;
			rb.AddForce (dir * 30, ForceMode.Impulse);
			isOnLedge = false;
		}
	}

	public void ResetFlyStamina(){
		flyStamina = maxFlyStamina;
	}

	public void BypassGravity(bool stopGrav){
		if (stopGrav) {
			rb.velocity = Vector3.zero;
		}
		
		stopGravity = stopGrav;
	}

	public void ContinuousExternalForce(Vector3 force, bool ignoreGravity, bool ignoreInput){
		if(ignoreGravity){
			BypassGravity (true);
		}
		if(ignoreInput){
			jumpInertia = Vector3.zero;
		} else {
			jumpInertia = jumpInertia * 0.6f;
		}

		rb.AddForce (force, ForceMode.Force);

		if (jumpPressTime == 0) {
			Vector3 counterForce = -Vector3.up * (force.magnitude * 0.2f);
			rb.AddForce (counterForce, ForceMode.Force);
		}
	}

	public void AddExternalForce(Vector3 force, float duration){
		externalForceAdded = true;
		rb.velocity = Vector3.zero;
		rb.AddForce (force, ForceMode.Impulse);
		externalForce = rb.velocity + force; 
		//StartCoroutine ("RegainControl", duration);
	}

//	IEnumerator RegainControl(float duration){
//		yield return new WaitForSeconds (duration);
//		externalForce = Vector3.zero;
//		//externalForceAdded = false;
//	}


	public void ChangeOrientationToCamera(Transform t, bool changedCam){
		if (changedCam && walkVelocity != Vector3.zero)
			holdOrientation = t;
		else if(holdOrientation == orientation)
			orientation = t;
	}


	void ResetMovementToZero(){
		walkVelocity = Vector3.zero;
		adjVertVelocity = 0f;
		cameraRotation = 0f;
		sanfonaStrength = 0f;
		jumpTriggerStrength = 0f;

		if(jumpInertia != Vector3.zero){
			if (Grounded ())
				jumpInertia = Vector3.zero;
		}
	}

	public struct WalkingStates
	{
		public bool IS_WALKING;
		public bool IS_GROUNDED;
		public bool IS_GLIDING;
		public bool IS_FALLING_MAX;
		public HeightState CURR_HEIGHT_STATE;
		public bool TOCANDO_NOTA;
	}

}