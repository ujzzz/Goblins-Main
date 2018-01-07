using UnityEngine;
using System.Collections;

public class arrowLogic : MonoBehaviour {

	Rigidbody rb;
	public bool flying = false;
	bool dead = false;
	public GameObject mainCamera;
	[SerializeField]
	public GameObject arrowCamera;
	[SerializeField]
	GameObject massCenter;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();
		mainCamera = GameObject.FindGameObjectWithTag ("MainCamera");
//		mainCamera.tag = "StandbyCamera";
//		arrowCamera.tag = "MainCamera";

		rb.centerOfMass = massCenter.transform.position; //the center mass of the arrow is its tip (just the tip!)
	}

	void FixedUpdate () {
		//this is really not cool way to do it cuz it's processor intense
		if (rb.velocity.magnitude > 1f) { //kind of arbitrary number i assigned to tell if it's flying fast or not
			flying = true;
		} else {
			flying = false;
		}

//		if (Input.GetMouseButtonDown (1)) { //if user wants to fire another arrow...
//			SwitchCameraBack (); //changes back to main camera
//		} else if (dead) { //if arrow is dead...
//					
//			Invoke ("SwitchCameraBack", 0.1f); //changes back to main camera
//		} 

		if (flying) {
			//rotates the arrow accordingly to gravity (this is kind of a cheap method I think and not 100% accurate but wahtevz)
			rb.rotation = Quaternion.LookRotation(rb.velocity);
		}

		//transform.forward = Vector3.Slerp(transform.forward, rb.velocity.normalized, Time.deltaTime);

		arrowCamera.transform.LookAt (transform); //keeps the camera looking at the arrow
	}

	void OnCollisionEnter (Collision other) {
		if (other.gameObject.tag == "Player") { //added this to be sure you can't fucking shoot yourself cuz youre not a tard
		} else {
			if (flying == true) {  //and if it was flying fast, then create a hitSound so people will hear that an arrow just hit something and be able to figure out where it came from
				GameObject hitSound = new GameObject ("hitSound");
				hitSound.transform.position = transform.position;
				hitSound.transform.rotation = transform.rotation;
				hitSound.transform.parent = transform;
				hitSound.tag = "arrowHitSound";
				Destroy (hitSound, 3f);
			}
			rb.isKinematic = true; //turns off gravity
			//rb.velocity = rb.velocity - rb.velocity;
			//rb.useGravity = false;
			rb.detectCollisions = false; //stops it from colliding with anything else
			transform.position = other.contacts [0].point; //sets its position to where it collided (this doesn't work well because if it hits an animated object, it doesn't keep moving [for inanimate its fine])
			transform.parent = other.transform; //makes it a part of the object it hit
			dead = true; //just letting the script know we're dead (i should just delete the arrow after a while... maybe)
			Invoke ("DeadArrow", 0.2f);
		}
	}

//	void SwitchCameraBack () { //doing this as a separate void so I can invoke a delay
//		//switch camera back to player 
//		arrowCamera.tag = "ArrowCamera";
//		mainCamera.tag = "MainCamera";
//		arrowCamera.SetActive (false);
//		// and turn off this script
//		this.enabled = false;
//		
//	}

	void DeadArrow() { //making this a separate method cuz I don't know how else to delay something besides Invoke
		transform.tag = "deadArrow";		
	}
}