using UnityEngine;
using System.Collections;

public class EnemySearchPlayer : MonoBehaviour {

	// 1) react if arrow whizzes by or if someone dies nearby and figure out where shot came from
	// 2) guys who haven't seen dead body don't go looking for the body. sometimes they just stand still in idle (glitch in moving out of from huddle state)
	// 3) once they reach attacking range of the player, they're not actually attacking but just keep running.. should switch to a funny animation
	// 4) need to react to new information (dead bodies, shots whizzing by, new player sighting) more dynamically
	// 5) spruce up the lighting. im tired of looking at this sad ass green
	// 6) spawn shrooms 
	// 7) make enemies goto the shrooms instead of just trees
	// 8) work on bow and arrow
	// 9) figure out what to do if leader is dead (switch to next leader?)
	// 10) arrow should onnly be insta-kill in some parts of the body
	///// a) better physics
	///// b) make arrow stick to whever it hits
	// 9) add field of view cones (better ones)
	// 10) add sound exclamations or whatever


	Animator anim;
	GameObject player;
	GameObject leader;
	GameObject[] enemies;
	float moveSpeed; 
	float awareness; 
	float bravery; 
	bool amLeader;
	Vector3 herdKnowledgePlayerPos;
	EnemyResetAnims reset;
	float viewAngle;
	float viewDistance;
	Vector3 myDirection; //this is the direction non-leader enemies will go, which includes a random offset to make sure they don't all walk in EXACT same direction
	float offset = 15; //offset for myDirection
	bool ready = false;
	float groundSize;


	// Use this for initialization
	void Start () {
		Invoke("GetStats",0.12f); // wait 1 second until all scripts are loaded
		reset = GetComponent<EnemyResetAnims> ();
		anim = GetComponent<Animator>();
		player = GetComponent<EnemyManager>().player;
		amLeader = GetComponent<EnemyManager> ().amLeader;
		enemies = GetComponent<EnemyManager> ().enemies;
		viewAngle = GetComponent<EnemyLookOut> ().viewAngle;
		viewDistance = GetComponent<EnemyLookOut> ().viewDistance;
		myDirection = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update () {
		if (ready) {
			HerdKnowledge ();
			DiscoverPlayer ();
			if (amLeader) {
				if (herdKnowledgePlayerPos == Vector3.zero | (Vector3.Distance (herdKnowledgePlayerPos, transform.position) < 15f)) { //if no one knows where the player is (scenario in case they just found a body)
					SearchPlayer (); //then just act like a random dude
				} else {
					SearchPlayerLeader (); //otherwise if someone has seen where the player is, then use herdknowledge of that location to go there
				}
			} else {
				SearchPlayer ();
				LeaderAttack ();
			}
		}
	}

	void SearchPlayerLeader () {
		Vector3 direction = herdKnowledgePlayerPos - transform.position; //he determines angle toward the herd's last known player position
		direction.y = 0;
		Debug.DrawRay (transform.position, direction);
		transform.forward = Vector3.RotateTowards(transform.forward,direction, (moveSpeed/2) * Time.deltaTime,0); // rotate toward herd's last known player position
		reset.ResetAnimation ();
		anim.SetBool ("isCrouch", true);
		anim.speed = 1f + (moveSpeed/10); //walk at speed appropriate to this guy's moveSpeed
		transform.Translate (0, 0, moveSpeed / 2 * Time.deltaTime); // move toward herd's last known player position
	}

	void SearchPlayer () {
		if (Random.Range (0, 1000) < 4) { // switch up my offset a bit every now and then
			myDirection.x = myDirection.x + Random.Range (-offset, offset);
			if (myDirection.x < -groundSize) {myDirection.x = -groundSize;} // limited to how far they go so they don't go off the map (this assume groundSize is same for X and Y)
			if (myDirection.x > groundSize) {myDirection.x = groundSize;	}
			myDirection.z = myDirection.z + Random.Range (-offset, offset);
			if (myDirection.z < -groundSize) {myDirection.z = -groundSize;}
			if (myDirection.z > groundSize) {myDirection.z = groundSize;}
		}
		Vector3 dir = (myDirection - transform.position);
		dir.y = 0;
		Debug.DrawRay (transform.position, dir);
		if (Vector3.Distance (myDirection, transform.position) < 1f) {
			reset.ResetAnimation ();
			anim.SetBool ("isIdle", true);
		} else {
			transform.forward = Vector3.RotateTowards(transform.forward,dir, (moveSpeed/2) * Time.deltaTime,0); // rotate toward herd's last known player position
			reset.ResetAnimation ();
			anim.SetBool ("isCrouch", true);
			anim.speed = 1f + (moveSpeed/10); //walk at speed appropriate to this guy's moveSpeed
			transform.Translate (0, 0, moveSpeed / 2 * Time.deltaTime); // move toward herd's last known player position
		}
	}

	void HerdKnowledge () {
		Vector3 pos = Vector3.zero; //resets everytime its run to filter out old data
		int herdSize = 0; 
			foreach (GameObject enemy in enemies) { // goes through each enemy and
			if (enemy != this.gameObject) { //don't add the value of this enemy's own info
				if (Vector3.Distance (enemy.transform.position, transform.position) < (awareness * 2.5f)) { //checks to see that enemy is within hearing distance
					if (enemy.GetComponent<EnemyLookOut>().lastPlayerPos != Vector3.zero) { //as long as an enemy has seen the player
						pos += enemy.GetComponent<EnemyLookOut> ().lastPlayerPos; //add that player's position to the herd knowledge 
						herdSize++; //and increase number of players who have seen the player
					}
				}
			}
		}
		if (herdSize > 0) { //as long as one person has seen player
			herdKnowledgePlayerPos = pos / herdSize; // make the herd knowledge of player position be equal to average of all the different sightings
			if (GetComponent<EnemyLookOut> ().lastPlayerPos == Vector3.zero) { //if you haven't yet seen the player
				GetComponent<EnemyLookOut> ().lastPlayerPos = herdKnowledgePlayerPos; //make your last known position equal to the herd knowledge
			}
			if (myDirection == Vector3.zero) { //if you don't know where to go, at first go to where you first saw the player
				myDirection = GetComponent<EnemyLookOut> ().lastPlayerPos; 
			}
			if (Random.Range (0, 5000) < awareness) { //randomly updates on where everyone is headed to. varies depending on awareness
				myDirection = herdKnowledgePlayerPos; 

			}
		}
	}

	void DiscoverPlayer () {
		Vector3 dir = player.transform.position - transform.position; //  angle toward player
		dir.y = 0;
		float angle = Vector3.Angle (dir, transform.forward); // angle toward player
		if (Vector3.Distance (player.transform.position, transform.position) < (viewDistance + (awareness / 2)) && angle < (viewAngle + awareness)) {
			if (bravery > 7) { // if he's brave enough
				GetComponent<EnemyManager> ().state = "attackPlayer"; // go and attack him
			} else {
				GetComponent<EnemyManager> ().state = "alertLeader"; // otherwise go and alert leader
			}
			GetComponent<EnemyLookOut>().lastPlayerPos = player.transform.position; //and remember where you saw the player
		}
	}

	void LeaderAttack () { //if leader attacks, then even if you don't see the player just run where the leader is running to 
		if (Vector3.Distance (transform.position, leader.transform.position) < awareness * 2f) { // if leader is within vicinity 
			if (leader.GetComponent<EnemyManager>().state == "attackPlayer") { //and he goes to attack player
				GetComponent<EnemyManager> ().state = "attackPlayer"; // then you also go attack player (even if you can't see him(
				GetComponent<EnemyLookOut>().lastPlayerPos = leader.GetComponent<EnemyLookOut>().lastPlayerPos; // and run toward where leader is running at
			}
		}
	}

	void GetStats () {
		moveSpeed = GetComponent<EnemyStats> ().moveSpeed; //pulls from the randomly generated stat for this enemy
		awareness = GetComponent<EnemyStats> ().awareness; //pulls from the randomly generated stat for this enemy
		bravery = GetComponent<EnemyStats> ().bravery; //pulls from the randomly generated stat for this enemy
		leader = GetComponent<EnemyManager>().leader;
		groundSize = (GetComponent<EnemyManager> ().ground.transform.localScale.x) / 2 - 5f;
		ready = true;
	}
}
