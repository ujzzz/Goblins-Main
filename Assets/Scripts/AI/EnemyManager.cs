using UnityEngine;
using System.Collections;

public class EnemyManager : MonoBehaviour {

	public GameObject[] enemies; //other fellow enemies
	public GameObject leader; // the leader
	public GameObject player; // the player
	public string state; // sates = normal, suspicious; findLeader, alertLeader, huddle, searchPlayer, attackPlayer, flee
	public bool amLeader;
	public Vector3 leaderPos;
	public bool alive = true;
	bool ready = false;
	public GameObject ground;
	public float morale;
	bool counted = false;

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player"); //identify the player
		enemies = GameObject.FindGameObjectsWithTag ("Enemy"); //identifies the fellow enemies
		ground = GameObject.FindGameObjectWithTag ("Ground"); //identifies the ground (for calculating size of map)
		morale = GetComponent<EnemyStats>().morale;
		GetComponent<EnemyLookOut> ().enabled = true; //turn on basic enemy look-out behavior at the outset
		Invoke("FindLeader",0.11f); // wait 1 second until all scripts are loaded
		state = "normal";
	}
	
	// Update is called once per frame
	void Update () {
		if (ready) {
			if (!alive) {
				state = "dead";
				GetComponent<EnemyLookOut> ().enabled = false;
				if (!counted) {
					ScoreCounter.humansRemaining--;
					counted = true;
				} else {
				}
			}

			if (morale < 0) {
				state = "fled";
				print ("ran away");
				GetComponent<EnemyLookOut> ().enabled = false;
				if (!counted) {
					ScoreCounter.humansRemaining--;
					counted = true;
				} else {
				}
			}
			if (state == "normal") {
				GetComponent<enemyGoalSeek> ().enabled = true;
			} else {GetComponent<enemyGoalSeek> ().enabled = false;}
			if (state == "takeCover") {
				GetComponent<EnemyTakeCover> ().enabled = true; //run to take cover
			} else {GetComponent<EnemyTakeCover> ().enabled = false;}
			if (state == "alertLeader") {
				GetComponent<EnemyAlertLeader> ().enabled = true; //run to alert the leader
			} else {GetComponent<EnemyAlertLeader> ().enabled = false;}
			if (state == "huddle") {
				GetComponent<EnemyHuddle> ().enabled = true;
			} else {GetComponent<EnemyHuddle> ().enabled = false;}
			if (state == "findLeader") {
				GetComponent<EnemyFindLeader> ().enabled = true;
			} else {GetComponent<EnemyFindLeader> ().enabled = false;}
			if (state == "findBody") {
				GetComponent<EnemyFindBody> ().enabled = true;
			} else {GetComponent<EnemyFindBody> ().enabled = false;}
			if (state == "searchPlayer") {
				GetComponent<EnemySearchPlayer> ().enabled = true;
			} else {GetComponent<EnemySearchPlayer> ().enabled = false;}
			if (state == "attackPlayer") {
				GetComponent<EnemyAttackPlayer> ().enabled = true;
			} else {GetComponent<EnemyAttackPlayer> ().enabled = false;}
			if (state == "flee") {
			}
//			if (leader.GetComponent<EnemyManager> ().state == "dead") { //if leader is dead, then find a new leader
//				print("he dead");
//				FindLeader ();
//			}
		}
	}

	void FindLeader () {
		leader = enemies[0]; //sets the first leader to zero just for initialization purposes
		for (int i = 0; i < enemies.Length; i++) { //run a loop until we go through every enemy in the array
			if (enemies[i].GetComponent<EnemyManager> ().alive == true) {
				if (leader.GetComponent<EnemyStats> ().bravery < enemies [i].GetComponent<EnemyStats> ().bravery) { //check bravery of every enemy versus every other and make sure he's not dead
					leader = enemies [i]; // the one with the highest bravery becomes the leader
				}
			}
		}
		if (leader == gameObject) {
			amLeader = true;
		}
		if (leaderPos == Vector3.zero) { //set the position of the leader only at the very outset of the level (when the enemies are spawning)
			leaderPos = leader.transform.position;
		}
		ready = true;

	}
}