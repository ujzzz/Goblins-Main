using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

	public Animator anim;
	public Rigidbody rBody;
	public CharacterController controller;
	private Vector3 movementZ;
	private Vector3 movement;
	[SerializeField]
	private float frontThrust;
	[SerializeField]
	private float backThrust;
	[SerializeField]
	private float sideThrust;
	private Vector3 mousePoint;
	[SerializeField]
	private float rotateSpeed;
	private Vector3 targetRotation;
	private float gravity = 20f;
	[SerializeField]
	private float diagonalSlow;

	// Use this for initialization
	void Start () {
		// grabs the goblin animator and physics to manipulate later
		anim = GetComponent<Animator> ();
		rBody = GetComponent<Rigidbody> ();
		controller = GetComponent<CharacterController> ();
	}

	void Update() {
		// moves based on input
		PlayerMove ();
	}

	void FixedUpdate () {
		// if left mouse button clicked then prepares player to rotate
		PlayerRotate ();

		// and actually do the whole rotation. I put this here because if I put it inside component it will stop running once key is not pressed anymore
		transform.forward = Vector3.Lerp (transform.forward, targetRotation, rotateSpeed * Time.deltaTime);
	}

	private void PlayerMove () {
		// collect movement input
		// so I had to split up MovementZ and MovementX because if I didn't, when the player moves diagonally (presses both X and Z), it doubles the speed and I didn't know how to fix
		if (Input.GetKey ("w")) {
			movementZ = new Vector3 (0f, 0f, Input.GetAxisRaw ("Vertical"));
			// not sure if Normalize does anything actually, but just in case the Vector3 value goes over 1 this resets it back to 1 (que Brian Mcknight song I start back at one)
			movementZ.Normalize ();
			movementZ = transform.TransformDirection (movementZ);
			// this just some shit I got from the Unity tutorial. Brings the mothafucka back to earth
			movementZ.y -= gravity * Time.deltaTime;
			// ok this is more diagonal adjustment shit. If he's going diaognal, it is still too fast for some reason, so I had to slow the muthafucka down some more
			if (movement != Vector3.zero) {
				movementZ *= (frontThrust / diagonalSlow);
			} else {
				movementZ *= frontThrust;
			}
			controller.Move (movementZ * Time.deltaTime);
			anim.SetBool ("Forward", true);
		} else {
			anim.SetBool ("Forward", false);
		}
		if (Input.GetKey ("s")) {
			movementZ = new Vector3 (0f, 0f, Input.GetAxisRaw ("Vertical"));
			movementZ.Normalize ();
			movementZ = transform.TransformDirection (movementZ);
			movementZ.y -= gravity * Time.deltaTime;
			if (movement != Vector3.zero) {
				movementZ *= (backThrust / diagonalSlow);
			} else {
				movementZ *= backThrust;
			}
			controller.Move (movementZ * Time.deltaTime);
			anim.SetBool ("Back", true);
		} else {
			anim.SetBool ("Back", false);
		}
		if (Input.GetKey ("a")) {
			movement = new Vector3 (Input.GetAxisRaw ("Horizontal"), 0f, 0f);
			movement.Normalize ();
			movement = transform.TransformDirection (movement);
			movement.y -= gravity * Time.deltaTime;
			if (movementZ != Vector3.zero) {
				movement *= (sideThrust / diagonalSlow);
			} else {
				movement *= sideThrust;
			}
			controller.Move (movement * Time.deltaTime);
			anim.SetBool ("Left", true);
		} else {
			anim.SetBool ("Left", false);
		}
		if (Input.GetKey ("d")) {
			movement = new Vector3 (Input.GetAxisRaw ("Horizontal"), 0f, 0f);
			movement.Normalize ();
			movement = transform.TransformDirection (movement);
			movement.y -= gravity * Time.deltaTime;
			if (movementZ != Vector3.zero) {
				movement *= (sideThrust / diagonalSlow);
			} else {
				movement *= sideThrust;
			}
			controller.Move (movement * Time.deltaTime);
			anim.SetBool ("Right", true);
		} else {
			anim.SetBool ("Right", false);
		}
		movement = Vector3.zero;
		movementZ = Vector3.zero;

	}

	private void PlayerRotate () {
			if (Input.GetMouseButtonDown (0)) {
				//get the point in world where mouse is touching
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				RaycastHit hit;
				// get the exact physical point
				if (Physics.Raycast (ray, out hit)) {
					mousePoint = hit.point;
					// move that point a bit above ground
					mousePoint.y = 0.25f;
				}
				// figure uut where mouse click is on the map screen relative to the goblin
				targetRotation = (mousePoint - transform.position);
			}
	}
}
