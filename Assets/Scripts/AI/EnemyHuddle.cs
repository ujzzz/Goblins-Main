using UnityEngine;
using System.Collections;

public class EnemyHuddle : MonoBehaviour {
	public GameObject leader;
	Animator anim;
	float moveSpeed; 
	float awareness; 
	float bravery;
	bool ready = false;
	EnemyResetAnims reset;
	GameObject[] enemies;
	Vector3 randomDir;
	bool sawPlayer = false;
	bool sawBody = false;

	float timer;
	float maxTime = 300f;

	// Use this for initialization
	void Start () {
		Invoke("GetStats",0.11f); // waits 2 seconds on purpose before turning on ready because need some time before they start to huddle
		anim = GetComponent<Animator>();
		reset = GetComponent<EnemyResetAnims> ();
		randomDir = new Vector3 (Random.Range (transform.position.x - 30, transform.position.x + 30), 0, Random.Range (transform.position.z - 30, transform.position.z + 30));  //random waypoint nearby
		if (randomDir.x > 50 || randomDir.x < -50 || randomDir.z > 50 || randomDir.z < -50) { //make sure random waypoint is not off the map, if it is, just choose some other random place (NOT GREAT!!!)
			randomDir = new Vector3 (Random.Range (-50, 50), 0, Random.Range (-50, 50)); 
		}
	}

 	//Update is called once per frame
	void Update () {
		if (ready) { // waits 2 seconds on purpose before turning on ready because need some time before they start to huddle
			if (GetComponent<EnemyManager>().amLeader) { //if I'm the leader
				WhatHappened();
				if (sawPlayer) {
					GetComponent<EnemyManager> ().state = "searchPlayer";
				} else if (sawBody && !sawPlayer) {
					GetComponent<EnemyManager> ().state = "findBody";
				} else if (timer > (maxTime * (10 - bravery))) {
					GetComponent<EnemyManager> ().state = "searchPlayer";
				}
			} else { //if I'm not the leader
				HuddleAroundLeader (); 
			}
		}
	}

	void WhatHappened() {
		for (int i = 0; i < enemies.Length; i++) { //cycles through all the buddies
			if (Vector3.Distance (enemies [i].transform.position, transform.position) < 6f) { // and if buddy is within hearing range
				if (enemies [i].GetComponent<EnemyLookOut> ().lastDeadBody != Vector3.zero) {
					sawBody = true;
				} else if (enemies [i].GetComponent<EnemyLookOut> ().lastPlayerPos != Vector3.zero) {
					sawPlayer = true;
				} else {
					timer++;
				}
			}
		}
	}

	// Update is called once per frame
//	void Update () {
//		if (ready) { // waits 2 seconds on purpose before turning on ready because need some time before they start to huddle
//			if (GetComponent<EnemyManager>().amLeader) { //if I'm the leader
//				CheckTroopLocation (); //check if everyone is around
//				if (Vector3.Distance (transform.position, randomDir) > 2f) { // run to random waypoint as a way of looking for othres (NOT GREAT LOGIC!)
//					reset.ResetAnimation ();
//					anim.SetBool ("isRun", true);
//					Vector3 dir = randomDir - transform.position;
//					anim.speed = 1f + (moveSpeed / 10); //adjusts animation speed to this player's unique moveSpeed
//					transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation (dir), moveSpeed * Time.deltaTime); //rotates toward the random waypoint direction
//					transform.Translate (0, 0, moveSpeed * 2 * Time.deltaTime); // move toward the random waypoint
//				} else { //once reach waypoint, create new one
//					randomDir = new Vector3 (Random.Range (transform.position.x - 30, transform.position.x + 30), 0, Random.Range (transform.position.z - 30, transform.position.z + 30));
//					if (randomDir.x > 50 || randomDir.x < -50 || randomDir.z > 50 || randomDir.z < -50) {
//						randomDir = new Vector3 (Random.Range (-50, 50), 0, Random.Range (-50, 50));
//					}
//				}
//			} else { //if I'm not the leader
//				HuddleAroundLeader (); 
//			}
//		}
//	}
//		
//	void CheckTroopLocation (){
//		Vector3 pos = Vector3.zero; //resets everytime its run to filter out old data
//		int herdSize = 0; 
//		foreach (GameObject enemy in enemies) { // goes through each enemy and
//			if (enemy != this.gameObject) { //don't add the value of this enemy's own info
//				pos += enemy.transform.position; //add that player's position to the herd knowledge 
//				}
//			}
//		if (Vector3.Distance((pos / enemies.Length),transform.position) < 35f) {
//			GetComponent<EnemyManager> ().state = "searchPlayer";
//		} 
//	}

	void HuddleAroundLeader () {
		if (leader.GetComponent<EnemyManager> ().state == "searchPlayer") { //if leader decides to look for player
			GetComponent<EnemyManager> ().state = "searchPlayer"; //then switch yourself to look for player
		}
		if (leader.GetComponent<EnemyManager> ().state == "findBody") {
			GetComponent<EnemyManager> ().state = "findBody";
		} else { //otherwise just stay close to the leader
			if ((Vector3.Distance (transform.position, leader.transform.position)) > 5f) { //stay within range of leader
				Vector3 dir = leader.transform.position - transform.position; //set direction toward leader
				dir.y = 0;
				reset = GetComponent<EnemyResetAnims> ();
				anim.SetBool ("isRun", true);
				anim.speed = 1f + (moveSpeed / 10); //walk at a normal speed
				transform.forward = Vector3.RotateTowards (transform.forward, dir, moveSpeed * Time.deltaTime, 0); // rotate toward leader
				transform.Translate (0, 0, moveSpeed * Time.deltaTime); // move toward the leader
			} else { // when near leader, turn your back toward him and watch out
				Vector3 bootyDirection = -(leader.transform.position - transform.position); // turn back to leader 
				bootyDirection.y = 0; 
				transform.forward = bootyDirection; // turn away from leader
				reset = GetComponent<EnemyResetAnims> ();
				anim.SetBool ("isIdle", true);
			}
		}
	}

	void GetStats () {
		moveSpeed = transform.GetComponent<EnemyStats> ().moveSpeed; //pulls from the randomly generated stat for this enemy
		awareness = transform.GetComponent<EnemyStats> ().awareness; //pulls from the randomly generated stat for this enemy
		bravery = transform.GetComponent<EnemyStats> ().bravery; //pulls from the randomly generated stat for this enemy
		leader = GetComponent<EnemyManager> ().leader; //i lumped this in here because it kind of fits
		enemies = GetComponent<EnemyManager> ().enemies;
		ready = true;
	}
}
 