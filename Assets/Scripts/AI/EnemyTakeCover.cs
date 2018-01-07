using UnityEngine;
using System.Collections;

public class EnemyTakeCover : MonoBehaviour {

	Animator anim;
	GameObject player;
	GameObject hidingTree;
	GameObject[] trees;
	GameObject[] enemies;
	float moveSpeed; 
	float awareness; 
	float bravery; 
	EnemyResetAnims reset;
	float groundSize;
	bool ready = false;
	float offset = 15; //offset for random direction to run away into
	Vector3 runDir;
	Vector3 hidingSpot;
	bool betterHide = false;
	bool readyToHide = false;
	float counter;
	bool hideBehindBuddy;

	float timer;
	float maxTime = 300f;

	// Use this for initialization
	void Start () {
		Invoke("GetStats",0.12f); // wait 1 second until all scripts are loaded
		reset = GetComponent<EnemyResetAnims> ();
		anim = GetComponent<Animator>();
		player = GetComponent<EnemyManager>().player;
		enemies = GetComponent<EnemyManager> ().enemies;
		trees = GameObject.FindGameObjectsWithTag ("Tree");
		hidingSpot = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update () {
		if (ready) {
			WhereToHide ();
		}

		if (timer > (maxTime * (10 - bravery))) {//once finished taking cover, go find leader (depends on how brave the soldier is)
			if (GetComponent<EnemyManager> ().amLeader) { //if I'm the leader
				GetComponent<EnemyLookOut> ().ohShit = true; //turn on alert
				GetComponent<EnemyManager> ().state = "huddle";  // and go to huddle
				timer = 50f;
				hidingSpot = Vector3.zero;
			} else {
				GetComponent<EnemyLookOut> ().ohShit = true; //turn on alert
				GetComponent<EnemyManager> ().state = "alertLeader";  //and go alert leader
				timer = 50f;
				hidingSpot = Vector3.zero;
			}
		}
	}

//	void WhereToRun () {
//		runDir = -player.transform.position; //random place opposite of where arrow came from (player)
//		runDir.y = 0f;
//		runDir.x = runDir.x + Random.Range (-offset, offset);
//		if (runDir.x < -groundSize) {
//			runDir.x = -groundSize;
//		} // limited to how far they go so they don't go off the map (this assume groundSize is same for X and Y)
//		if (runDir.x > groundSize) {
//			runDir.x = groundSize;
//		}
//		runDir.z = runDir.z + Random.Range (-offset, offset);
//		if (runDir.z < -groundSize) {
//			runDir.z = -groundSize;
//		}
//		if (runDir.z > groundSize) {
//			runDir.z = groundSize;
//		}
//	}
//
//	void RunNigga (){
//		Debug.DrawRay (transform.position, runDir);
//		transform.forward = Vector3.RotateTowards (transform.forward, runDir, moveSpeed * Time.deltaTime, 0); // rotate aware from where arrow came from
//		reset.ResetAnimation ();
//		anim.SetBool ("isRun", true);
//		anim.speed = 1f + (moveSpeed / 10); //animate at normal speed
//		transform.Translate (0, 0, moveSpeed * Time.deltaTime); // run in opposite direction of arrow
//		counter++;
//		print (counter);
//		if (counter > (500 - (awareness * bravery * 20))) { // decides how long enemy panics for until they decide to go hide
//			betterHide = true;
//		}
//	}

	void WhereToHide() {
		if (hidingSpot != Vector3.zero) {
			Hide ();
		} else if (!hideBehindBuddy) {
			hidingTree = trees [0];
			for (int i = 0; i < trees.Length; i++) { //cycles through all the trees
				if ((Vector3.Distance (hidingTree.transform.position, transform.position) > (Vector3.Distance (trees [i].transform.position, transform.position)))) { //find the nearest tree
					hidingTree = trees [i];
				}
			}
			hidingSpot = hidingTree.transform.position; //set the nearest tree to be your hiding spot
			hidingSpot.y = 0f;
			if (player.transform.position.x > 0) { //add an offset to the hiding spot so you're on the opposite side of where the arrow came from (player)
				hidingSpot.x = hidingSpot.x - 0.5f;
			} else {
				hidingSpot.x = hidingSpot.x + 0.5f;
			}
			if (player.transform.position.z > 0) {
				hidingSpot.z = hidingSpot.z - 0.5f;
			} else {
				hidingSpot.z = hidingSpot.z + 0.5f;
			}
		}
	}

	void Hide () {
		if (Vector3.Distance (hidingSpot, transform.position) < 1f) { //if you're at hiding spot

			for (int i = 0; i < enemies.Length; i++) { //cycles through all the buddies
				if (enemies [i].gameObject != this.gameObject) { //scan through everyone but yo'self
					if (Vector3.Distance (transform.position, enemies [i].transform.position) < 1f) { // to avoid them bunching up, if youre too close to one
						hidingTree = enemies[i];
						hideBehindBuddy = true;
//						hidingSpot.x = hidingSpot.x * 1.5f; //just move away a bit
//						hidingSpot.z = hidingSpot.z * 1.5f; //just move away a bit
//						Vector3 dir = (hidingSpot - transform.position);
//						transform.forward = Vector3.RotateTowards (transform.forward, dir, moveSpeed * Time.deltaTime, 0); // rotate to hiding spot
//						reset.ResetAnimation ();
//						anim.SetBool ("isRun", true);
//						anim.speed = 1f + (moveSpeed / 10); //animate at normal speed
//						transform.Translate (0, 0, moveSpeed * Time.deltaTime); // run to hiding spot
						}
					}
				}
			transform.forward = Vector3.RotateTowards (transform.forward, player.transform.position, moveSpeed * Time.deltaTime, 0); // rotate toward where arrow came from (player) and wait
			reset.ResetAnimation ();
			anim.SetBool ("isIdle", true);
			timer++;
		} else {
			Vector3 dir = (hidingSpot - transform.position);
			dir.y = 0f;
			transform.forward = Vector3.RotateTowards (transform.forward, dir, moveSpeed * Time.deltaTime * 2, 0); // rotate to hiding spot
			reset.ResetAnimation ();
			anim.SetBool ("isRun", true);
			anim.speed = 1f + (moveSpeed / 10); //animate at normal speed
			transform.Translate (0, 0, moveSpeed * Time.deltaTime); // run to hiding spot
		}
	}

	void GetStats () {
		moveSpeed = GetComponent<EnemyStats> ().moveSpeed; //pulls from the randomly generated stat for this enemy
		awareness = GetComponent<EnemyStats> ().awareness; //pulls from the randomly generated stat for this enemy
		bravery = GetComponent<EnemyStats> ().bravery; //pulls from the randomly generated stat for this enemy
		groundSize = (GetComponent<EnemyManager> ().ground.transform.localScale.x) / 2 - 5f;
		ready = true;
	}
}
