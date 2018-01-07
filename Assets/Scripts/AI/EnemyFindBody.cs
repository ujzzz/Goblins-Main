using UnityEngine;
using System.Collections;

public class EnemyFindBody : MonoBehaviour {
	Animator anim;
	GameObject leader;
	GameObject[] enemies;
	EnemyResetAnims reset;
	float moveSpeed; 
	float awareness; 
	float bravery; 
	bool ready = false;
	Vector3 deadBodyPos;
	public Vector3 randomDir;
	bool cantFindAnyone = false;

	// Use this for initialization
	void Start () {
		Invoke("GetStats",0.12f); // wait 1 second until all scripts are loaded
		reset = GetComponent<EnemyResetAnims> ();
		anim = GetComponent<Animator>();
		deadBodyPos = GetComponent<EnemyLookOut> ().lastDeadBody;
		randomDir = new Vector3 (Random.Range (-50, 50), 0f,Random.Range(-50,50)); // create a random direction
		print (deadBodyPos);
	}
	
	// Update is called once per frame
	void Update () {
		if (ready) {
			if (deadBodyPos != Vector3.zero) {
				RunToBody ();
			} else {
				AskSomeone ();
			}
			PlayerAlert ();
			if (cantFindAnyone) {
				FindSomebody ();
			}
		}

		if (Random.Range(0, 1000) < 1) {//updates random direction every few seconds as enemy is frantically running and looking for the leader
			randomDir = new Vector3 (Random.Range (-50, 50), 0f,Random.Range(-50,50)); // create a random direction
		}

		if (Vector3.Distance (transform.position, randomDir) < 5f) { //if you run to the place and leader is not around
			randomDir = new Vector3 (Random.Range (-50, 50), 0f,Random.Range(-50,50)); // create a new random direction
		}
	}

	void RunToBody () {
			Vector3 dir = (deadBodyPos - transform.position);
			dir.y = 0;
			Debug.DrawRay (transform.position, dir);
			transform.forward = Vector3.RotateTowards(transform.forward,dir, moveSpeed * Time.deltaTime,0); // rotate toward herd's last known player position
			reset.ResetAnimation ();
			anim.SetBool ("isRun", true);
			anim.speed = 1f + (moveSpeed/10); //walk at speed appropriate to this guy's moveSpeed
			transform.Translate (0, 0, moveSpeed * 2 * Time.deltaTime); // move toward herd's last known player position
		if (Vector3.Distance (transform.position, deadBodyPos) < 2f) {
			GetComponent<EnemyManager> ().state = "searchPlayer";
		}
	}

	void AskSomeone () {
		for (int i = 0; i < enemies.Length; i++) { //cycles through all the buddies
			if (enemies [i].GetComponent<EnemyLookOut> ().lastDeadBody != Vector3.zero) { // if a buddy saw a dead body
				if (Vector3.Distance (enemies [i].transform.position, transform.position) < (awareness + 10)) { // and if buddy is within hearing range
					cantFindAnyone = false;
					deadBodyPos = enemies [i].GetComponent<EnemyLookOut> ().lastDeadBody; // then go to where that buddy saw the dead body
				}
			}
		}
		if (deadBodyPos == Vector3.zero) {
			cantFindAnyone = true;
		}

	}

	void PlayerAlert () {
		for (int i = 0; i < enemies.Length; i++) { //cycles through all the buddies
			if (enemies [i].GetComponent<EnemyLookOut> ().lastPlayerPos != Vector3.zero) { // if a buddy saw the player
				if (Vector3.Distance (enemies [i].transform.position, transform.position) < awareness) { // and if buddy is within hearing range
					GetComponent<EnemyManager>().state = "searchPlayer";
				}
			}
		}
	}

	void FindSomebody () {
		Vector3 dir = randomDir - transform.position; //set direction toward random direction
		reset.ResetAnimation();
		anim.SetBool ("isRun", true);
		anim.speed = 1f + (moveSpeed / 10); //adjusts animation speed to this player's unique moveSpeed
		transform.rotation = Quaternion.Slerp(transform.rotation,Quaternion.LookRotation(dir),moveSpeed * Time.deltaTime); //rotates toward the new random direction
		transform.Translate (0, 0, moveSpeed * 2 * Time.deltaTime); // move toward the random direction
	}
		
	void GetStats () {
		moveSpeed = GetComponent<EnemyStats> ().moveSpeed; //pulls from the randomly generated stat for this enemy
		awareness = GetComponent<EnemyStats> ().awareness; //pulls from the randomly generated stat for this enemy
		bravery = GetComponent<EnemyStats> ().bravery; //pulls from the randomly generated stat for this enemy
		leader = GetComponent<EnemyManager>().leader;
		enemies = GetComponent<EnemyManager> ().enemies;
		ready = true;
	}
}
