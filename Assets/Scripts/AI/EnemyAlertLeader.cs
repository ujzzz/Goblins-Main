using UnityEngine;
using System.Collections;

public class EnemyAlertLeader : MonoBehaviour {

	GameObject leader;
	Animator anim;
	float moveSpeed; 
	bool ready = false;
	EnemyResetAnims reset;

	// Use this for initialization
	void Start () {
		Invoke("GetStats",0.11f); // wait 1 second until all scripts are loaded
		anim = GetComponent<Animator>();
		leader = GetComponent<EnemyManager> ().leader;
		reset = GetComponent<EnemyResetAnims> ();
	}

	void Update () {
		if (ready) {
			AlertLeader ();
		}
		if (GetComponent<EnemyManager> ().amLeader) {
			GetComponent<EnemyManager> ().state = "huddle";
		}
	}

	void AlertLeader () {
		if ((Vector3.Distance (transform.position, GetComponent<EnemyManager>().leaderPos)) > 5f) { //keep running at the last known position of leader unless you are close to it 
			Vector3 dir = GetComponent<EnemyManager>().leaderPos - transform.position; //set direction toward last known leader position
			//Debug.DrawRay(transform.position,dir);
			dir.y = 0;
			reset.ResetAnimation ();
			anim.SetBool ("isRun", true);
			anim.speed = 1f + (moveSpeed / 10); //walk at a normal speed
			transform.forward = Vector3.RotateTowards (transform.forward, dir, 2 * moveSpeed * Time.deltaTime, 0); // rotate toward last kown leader position
			transform.Translate (0, 0, moveSpeed * 2 * Time.deltaTime); // move toward the last known leader position
		} else { // after reaching last known leader position
			if (Vector3.Distance (transform.position, leader.transform.position) > 5f) {
				GetComponent<EnemyManager> ().state = "findLeader";
			} else {
				GoToHuddle ();
				reset.ResetAnimation ();
//				anim.SetBool ("isAlert", true); // plays the alert leader animation (not great right now)
//				Invoke("GoToHuddle",2f); // wait 2 second to let Alert animation finish playing
			}
		}
	}

	void GetStats () {
		moveSpeed = transform.GetComponent<EnemyStats> ().moveSpeed; //pulls from the randomly generated stat for this enemy
		ready = true;
	}

	void GoToHuddle () {
		GetComponent<EnemyManager> ().state = "huddle";
	}
}