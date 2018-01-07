using UnityEngine;
using System.Collections;

public class EnemyAttackPlayer : MonoBehaviour {
	Animator anim;
	GameObject player;
	float moveSpeed; 
	float awareness; 
	float bravery; 
	EnemyResetAnims reset;
	float viewAngle;
	float viewDistance;
	Vector3 lastPlayerPos;

	// Use this for initialization
	void Start () {
		Invoke("GetStats",0.12f); // wait 1 second until all scripts are loaded
		reset = GetComponent<EnemyResetAnims> ();
		anim = GetComponent<Animator>();
		player = GetComponent<EnemyManager>().player;
		lastPlayerPos = GetComponent<EnemyLookOut> ().lastPlayerPos;
		viewAngle = GetComponent<EnemyLookOut> ().viewAngle;
		viewDistance = GetComponent<EnemyLookOut> ().viewDistance;
	}
	
	// Update is called once per frame
	void Update () {
		DiscoverPlayer ();
		if (Vector3.Distance (lastPlayerPos, transform.position) < 3f) { // if you get to where you thought player 
			if (Vector3.Distance (player.transform.position, transform.position) < 3f) { //and player is there
				reset.ResetAnimation (); 
				anim.SetBool ("isTwerk", true); // then twerk that ass
//			} else {
//				reset.ResetAnimation (); 
//				anim.SetBool ("isIdle", true); // but if he's not there, just look around like an idiot
			}
		} else {
			RunAtPlayer (); // run at the last known player position
		}
	}

	void DiscoverPlayer () {
		Vector3 dir = player.transform.position - transform.position; //  angle toward player
		dir.y = 0;
		float angle = Vector3.Angle (dir, transform.forward); // angle toward player
		if (Vector3.Distance (player.transform.position, transform.position) < (viewDistance + (awareness / 2)) && angle < (viewAngle + awareness)) {
				lastPlayerPos = player.transform.position;
		}
	}

	void RunAtPlayer () {
		Vector3 direction = lastPlayerPos - transform.position; //he determines angle toward last seen player position
		direction.y = 0;
		Debug.DrawRay (transform.position, direction);
		transform.forward = Vector3.RotateTowards(transform.forward,direction, 2 * moveSpeed * Time.deltaTime,0); // rotate toward last seen player position
		reset.ResetAnimation ();
		anim.SetBool ("isRun", true);
		anim.speed = 1f + (moveSpeed/10); //walk at speed appropriate to this guy's moveSpeed
		transform.Translate (0, 0, moveSpeed * 2 * Time.deltaTime); // move toward herd's last known player position
	}

	private void GetStats () {
		moveSpeed = GetComponent<EnemyStats> ().moveSpeed; //pulls from the randomly generated stat for this enemy
		awareness = GetComponent<EnemyStats> ().awareness; //pulls from the randomly generated stat for this enemy
		bravery = GetComponent<EnemyStats> ().bravery; //pulls from the randomly generated stat for this enemy
	}
}
