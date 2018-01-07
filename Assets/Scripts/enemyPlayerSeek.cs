//notes for next time:
//3) ideal logic should be for them to bunch up together and search for me since searching alone is scary, so work that in later after you figure #2 out
//4) the dead animation should be ready in mixamo. so just plug it into the animator and substitte for current placeholder animation
//5) you should add a runSpeed and walkSpeed to the enemyStats and all other stats so you don't fidget around with it in every script

using UnityEngine;
using System.Collections;

public class enemyPlayerSeek : MonoBehaviour {
	private Animator anim;
	private int chat;
	public GameObject[] otherEnemies;
	public GameObject player;
	public GameObject leader;
	private float moveSpeed; 
	private float awareness; 
	private float bravery; 
	public bool alert = false; 
	public bool alertByDeath = false;
	public bool seekPlayer = false; 
	public bool alertLeader = false; 
	public bool catchPlayer = false;
	private bool flee = false;
	[SerializeField]
	private float playerRange; // distance at which they start twerking
	public Vector3 lastPlayerPosition;
	public Vector3 herdKnowledgePlayerPosition;
	private Vector3 averageHeading;
	[SerializeField]
	private float buddyDistance;
	public bool leaderZ = false;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
		player = GameObject.FindGameObjectWithTag ("Player"); // identify the player
		otherEnemies = GameObject.FindGameObjectsWithTag ("Enemy");
		Invoke("GetStats",0.2f); // wait 1 second until all scripts are loaded
		Invoke("FindLeader",0.2f); // wait 1 second until all scripts are loaded
	}

	private void GetStats () {
		moveSpeed = GetComponent<EnemyStats> ().moveSpeed; //pulls from the randomly generated stat for this enemy
		awareness = GetComponent<EnemyStats> ().awareness; //pulls from the randomly generated stat for this enemy
		bravery = GetComponent<EnemyStats> ().bravery; //pulls from the randomly generated stat for this enemy
	}

	private void HerdKnowledge () {
		Vector3 herdKnowledgePlayerPos = Vector3.zero;
		int herdSize = 0;
		foreach (GameObject enemy in otherEnemies) {
			if (enemy != this.gameObject) {
				if (enemy.GetComponent<enemyPlayerSeek>().lastPlayerPosition != Vector3.zero) {
					herdKnowledgePlayerPos += enemy.GetComponent<enemyPlayerSeek> ().lastPlayerPosition;
					herdSize++;
				}
			}
		}
		if (herdSize > 0) {
			print (herdKnowledgePlayerPos/herdSize);
			Debug.DrawRay (transform.position, herdKnowledgePlayerPos / herdSize);
			herdKnowledgePlayerPosition = herdKnowledgePlayerPos;
		}
	}

	private void FindLeader () {
		leader = otherEnemies[0]; //sets the first leader to zero just for initialization purposes
		for (int i = 0; i < otherEnemies.Length; i++) { //run a loop until we go through every enemy in the array
			if (leader.GetComponent<EnemyStats> ().bravery < otherEnemies [i].GetComponent<EnemyStats> ().bravery) { //check bravery of every enemy versus every other
				leader = otherEnemies [i]; // the one with the highest bravery becomes the leader
			}
		}
		if (leader == gameObject) {
			leaderZ = true;
			print ("oh shit I'm the leader");
		}
	}

	// Update is called once per frame
	void Update () {
		DiscoverPlayer ();
		HerdKnowledge ();
		if (alert) {
			//FightOrFlight ();
			//if (alertLeader) {
				AlertLeader ();
			}
			if (catchPlayer) {
				CatchPlayer ();
//			}
//			if (flee) {
//				Flee ();
//			}
		} else {
			if (alertByDeath) {
//				SeekOrFlight ();
//				if (alertLeader) {
					AlertLeader ();
				//}
				if (seekPlayer) {
					SeekPlayer ();
				}
			}
		}
		Invoke("BuddyAlert",Random.Range(1f, 100/awareness)); // delay until enemy understands what he heard. delay depends on awareness
		Invoke("BuddyDeadAlert",Random.Range(1f, 100/awareness)); // delay until enemy understands what he heard. delay depends on awareness
	}

	// enemies become alert if you walk in their field of view
	public void DiscoverPlayer () {
		Vector3 directionLook = player.transform.position - transform.position; //  angle toward player
		directionLook.y = 0;
		float angle = Vector3.Angle (directionLook, transform.forward); // angle toward player
		if (Random.Range (0, 20/awareness) < 1 ) { // might or might not notice depending on awareness
			// if distance is close enough and within field of view then. Also I set distance vision at 8 + awareness/3 and distance angle at 60 degrees + awareness. mess around as necessary
			if (Vector3.Distance (player.transform.position, transform.position) < (8 + (awareness/3)) && angle < (60 + awareness)) { 
				gameObject.GetComponent<enemyGoalSeek>().enabled = false; // disable the normal tree seeking behavior
				alert = true; //set state to alert
				alertByDeath = false; // enemy sees player, so don't need to seek them anymore
				lastPlayerPosition = player.transform.position;
			}
		}
	}

	public void CatchPlayer () {
		Vector3 directionPlayer = lastPlayerPosition - transform.position; // angle toward player's last seen position
		directionPlayer.y = 0; // adjusts the Y angle so it doesn't point up or down weirdly
		//Debug.DrawRay (transform.position, directionPlayer);
		anim.SetBool ("isWalking", true);
		anim.SetBool ("isIdle", false);
		anim.SetBool ("isTwerk", false);
		anim.speed = 1f + (moveSpeed/10); //walk at a normal speed
		transform.forward = Vector3.RotateTowards(transform.forward,directionPlayer, moveSpeed * Time.deltaTime,0); // rotate toward player
		transform.Translate (0, 0, moveSpeed * Time.deltaTime); // move toward the player
	}

	private void FightOrFlight () {
		if (bravery > (transform.GetComponent<EnemyStats> ().maxBravery * 0.8)) {
			catchPlayer = true;
//		}
//		if (bravery < (transform.GetComponent<EnemyStats> ().maxBravery * 0.2)) {
//			flee = true;
		} else {
			alertLeader = true;
		}
	}

	private void SeekOrFlight () {
		if (bravery > (transform.GetComponent<EnemyStats> ().maxBravery * 0.8)) {
			seekPlayer = true;
		} else {
			alertLeader = true;
		}
	}

	private void Flee () {
		Vector3 directionFlee = new Vector3 (Random.Range(-50,50), 0);
		//Debug.DrawRay (transform.position, directionFlee);
		anim.SetBool ("isWalking", true);
		anim.SetBool ("isIdle", false);
		anim.SetBool ("isTwerk", false);
		anim.speed = 1f + (moveSpeed/10); //walk at a normal speed
		transform.forward = Vector3.RotateTowards(transform.forward,directionFlee, (moveSpeed/2) * Time.deltaTime,0); // rotate toward flee direction
		transform.Translate (0, 0, moveSpeed * Time.deltaTime); // move toward the player
	}

	private void AlertLeader () {
		if ((Vector3.Distance (transform.position, leader.transform.position)) > 2f) {
			Vector3 directionLeader = leader.transform.position - transform.position; //set direction toward leader
			directionLeader.y = 0;
			//Debug.DrawRay (transform.position, directionLeader);
			anim.SetBool ("isWalking", true);
			anim.SetBool ("isWalking", true);
			anim.SetBool ("isIdle", false);
			anim.SetBool ("isTwerk", false);
			anim.speed = 1f + (moveSpeed / 10); //walk at a normal speed
			transform.forward = Vector3.RotateTowards (transform.forward, directionLeader, (moveSpeed / 2) * Time.deltaTime, 0); // rotate toward leader
			transform.Translate (0, 0, moveSpeed * Time.deltaTime); // move toward the leader
		} else {
			alertLeader = false;
			if (alert) {
				catchPlayer = true;
			}
			if (alertByDeath) {
				seekPlayer = true;
			}
		}
	}

	// checks to see if enemy heard when others alerted him to player's presence
	public void BuddyAlert () {
		for (int i = 0; i < otherEnemies.Length; i++) { //cycles through all the enemies
			if (Vector3.Distance (transform.position, otherEnemies [i].transform.position) < (awareness)) { // if within hearing range, which depends on awareness, then...
				if (otherEnemies [i].GetComponent<enemyPlayerSeek> ().alert) { // if the other enemy is alerted..
					alert = true; //this enemy also becomes alerted
					alertByDeath = false;
					gameObject.GetComponent<enemyGoalSeek>().enabled = false; // disable the normal tree seeking behavior
				}
			}
		}
	}

	public void BuddyDeadAlert () {
		for (int i = 0; i < otherEnemies.Length; i++) { //cycles through all the enemies
			if (Vector3.Distance (transform.position, otherEnemies [i].transform.position) < (awareness)) { // if within hearing range, which depends on awareness, then...
				if (otherEnemies [i].GetComponent<enemyPlayerSeek>().alertByDeath){ // if someone else is dead or freaking out about death
					alertByDeath = true; //enemy becomes alert by a buddy's death
					gameObject.GetComponent<enemyGoalSeek>().enabled = false; // disable the normal tree seeking behavior
				}
			}
		}
	}

	public void SeekPlayer () {	
		if (leader == gameObject) { //if this guy is the leader
			Vector3 direction = herdKnowledgePlayerPosition - transform.position; //he determines angle toward the herd's last known player position
			direction.y = 0;
			Debug.DrawRay (transform.position, direction);
			transform.forward = Vector3.RotateTowards(transform.forward,direction, moveSpeed * Time.deltaTime,0); // rotate toward herd's last known player position
			anim.SetBool ("isTwerk", false);
			anim.SetBool ("isWalking", true);
			transform.Translate (0, 0, moveSpeed * Time.deltaTime); // move toward herd's last known player position
			anim.speed = 1f + (moveSpeed/10); //walk at speed appropriate to this guy's moveSpeed
		}
		else { //if not the leader
			if (Vector3.Distance (leader.transform.position, transform.position) > bravery) { //if far away from leader catch up
				anim.SetBool ("isTwerk", false);
				anim.SetBool ("isWalking", true);
				anim.speed = 1f + (moveSpeed/10); //walk at speed appropriate to this guy's moveSpeed
				transform.Translate (0, 0, moveSpeed * (10/bravery) * Time.deltaTime); // then just move ahead at a speed relevant to your bravery BUT 
			}
			if (Random.Range (0f, 5) < 2) { // every few seconds adjust rotation toward the leader. hopefully this makes for slightly random rotation/flock behavior
				Vector3 direction = leader.transform.forward - transform.position;
				direction.y = 0;
				Debug.DrawRay (transform.position, direction);
				transform.forward = Vector3.RotateTowards(transform.forward,direction, moveSpeed * Time.deltaTime,0); // rotate toward player
			}
		}
	}

	//grinds against the player when within distance (change the grind to attack later, if you want)
	public void BootyDance () {
		GetComponent<enemyPlayerSeek> ().alertByDeath = false;
		Vector3 directionP = player.transform.position - transform.position; // articualtes direction toward player
		directionP.y = 0; // adjusts the Y angle so it doesn't point up/down weirdly
		Vector3 bootyDirection = -directionP; // sets the direction for twerking
		bootyDirection.y = 0; // make sure that booty isn't pointed too high or too low
		transform.forward = Vector3.RotateTowards(transform.forward,bootyDirection, (moveSpeed/2) * Time.deltaTime,0); // rotate booty toward player
		anim.speed = Random.Range(1f,1.2f); // twerk at a random pace
		anim.SetBool("isTwerk",true);
		anim.SetBool ("isWalking", false);
		anim.SetBool ("isIdle", false);
	}
}