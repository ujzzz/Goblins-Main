using UnityEngine;
using System.Collections;

public class enemyChat : MonoBehaviour {


	private Animator anim;
	private int chat;
	public GameObject[] chatPartner;
	public GameObject player;
	[SerializeField]
	private float rotSpeed = 4f; // how fast they rotate
	[SerializeField]
	private float runSpeed = 4f; // how fast they run
	public enemyGoalSeek enemyGoalSeek; // this is just to disable their normal tree seeking behavior
	[SerializeField]
	private float enemySightDistance; // enemy field of view distance
	[SerializeField]
	private float enemySightAngle; // enemy field of view angle
	private bool alert; // sets the state
	[SerializeField]
	private float playerRange; // distance at which they start twerking
	private Vector3 bootyDirection;

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player"); // identify the player
		anim = GetComponent<Animator> ();
		enemyGoalSeek = GetComponent<enemyGoalSeek> (); // pull up the tree seeking script

		// this is code I used to have them turn to a random person and start chatting during their free time
//		chatPartner = GameObject.FindGameObjectsWithTag ("Enemy");
//		int i = Random.Range (0, chatPartner.Length);
//		transform.rotation = Quaternion.LookRotation (chatPartner[i].transform.position - transform.position);
	}

	// Update is called once per frame
	void Update () {
		DiscoverPlayer ();
		if (alert) { //if they're in the alert state
			if (Vector3.Distance (transform.position, player.transform.position) > playerRange) { // and if the dude is within range, start twerking
				SeekPlayer();
			} else {
				BootyDance ();
			}
		}
	}

	// enemies start rapping at you
	public void DiscoverPlayer () {

		Vector3 direction = player.transform.position - transform.position; //  angle toward player
		direction.y = 0;
		float angle = Vector3.Angle (direction, transform.forward); // angle toward player
		if (Vector3.Distance(player.transform.position, transform.position) < enemySightDistance && angle < enemySightAngle) { // if distance is close enough and within field of view then...
			alert = true; 
		}
	}

	public void SeekPlayer () {

		Vector3 direction = player.transform.position - transform.position; // angle toward player
		direction.y = 0;
		enemyGoalSeek.enabled = false; // disable the normal tree seeking behavior
		anim.SetBool ("isWalking", true);
		anim.speed = 1.6f;
		anim.SetBool ("isIdle", false);
		anim.SetBool ("isTwerk", false);
		transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation (direction), rotSpeed * Time.deltaTime); // rotate toward the player
		transform.Translate (0, 0, runSpeed * Time.deltaTime); // move toward the player
	}

	public void BootyDance () {

		Vector3 direction = player.transform.position - transform.position;
		direction.y = 0;
		bootyDirection = -direction; // this is for the twerk lel
		bootyDirection.y = 0; // make sure that booty isn't pointed too high or too low
		transform.forward = Vector3.RotateTowards(transform.forward,bootyDirection,rotSpeed * Time.deltaTime,0); // rotate booty toward player
		anim.speed = 1f;
		anim.SetBool("isTwerk",true);
		anim.SetBool ("isWalking", false);
		anim.SetBool ("isIdle", false);
	}
}