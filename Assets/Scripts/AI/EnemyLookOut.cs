using UnityEngine;
using System.Collections;

public class EnemyLookOut : MonoBehaviour {

	GameObject player;
	GameObject[] enemies;
	float awareness; 
	float moveSpeed;
	public Vector3 lastPlayerPos;
	public Vector3 lastDeadBody;
	bool ready = false;
	public bool ohShit = false;
	bool closerLook = false;
	bool arrowHuh = false;
	bool arrowHitOhShit = false;
	GameObject leader;
	public string states;
	Animator anim;
	public float viewAngle = 75f;
	public float viewDistance = 18f;
	EnemyResetAnims reset;
	Vector3 arrowDirection;

	// Use this for initialization
	void Start () {
		Invoke("GetStats",0.2f); // wait 1 second until all scripts are loaded
		anim = GetComponent<Animator> ();
		reset = GetComponent<EnemyResetAnims> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (ready) {
			DiscoverPlayer ();
			BuddyAlert ();
			HeardArrowHitting ();

			if (!ohShit) { //so they only respond to arrows whizzing by or findnig dead bodies if they aren't already on alert. Not sure if this is best. Maybe even if you're alerted, you can still use new information to help you find the goblin?
				ArrowAlert (); 
				DeadBodyAlert ();
			}
			if (closerLook && !ohShit) { //if notices something and not already alerted, then go and take a closer look
				CloserLook ();
			}
			if (!GetComponent<EnemyManager> ().amLeader) { //if you are not leader, keep track of where leader is
				WhereLeader (); 
			}
			if (arrowHitOhShit && !ohShit && GetComponent<EnemyManager>().state != "takeCover") { //if people are aware someone shot an arrow but they do not yet know its a player
				Invoke("ArrowOhShit",(Random.Range(1f,3f))); //go take cover
			}
			if (arrowHuh && !ohShit) { //if an arrow whizzes by and you're not already alret, go take a closer look
				Invoke("ArrowHuh",0.8f);
			}
		}
	}

	// enemies become alert if you walk in their field of view
	void DiscoverPlayer () {
		Vector3 dir = player.transform.position - transform.position; //  angle toward player
		dir.y = 0;
		float angle = Vector3.Angle (dir, transform.forward); // angle toward player
		if (Random.Range (0, 20/awareness) < 1 ) { // tiny chance might not see
			if (Vector3.Distance (player.transform.position, transform.position) < (viewDistance + awareness * 2) && angle < (viewAngle + awareness) && !ohShit) { // if see something far away 
				lastPlayerPos = player.transform.position;
				closerLook = true;
				GetComponent<EnemyManager> ().state = "suspicious"; // set state to suspicious
			}
			// if distance is close enough and within field of view then. Also I set distance vision at 8 + awareness/3 and distance angle at 60 degrees + awareness. mess around as necessary
			if (Vector3.Distance (player.transform.position, transform.position) < (viewDistance + (awareness / 2)) && angle < (viewAngle + awareness)) {
				if (GetComponent<EnemyManager> ().amLeader) {
					if (GetComponent<EnemyManager> ().state == "searchPlayer" || GetComponent<EnemyManager> ().state == "attackPlayer") {
					//don't do anything if enemy is already searching for player or attacking him	
					} 
					else { //otherwise goto huddle
						GetComponent<EnemyManager> ().state = "huddle"; // if you're the leader, then start searching for the player by yourself
					}
				} else { //if not the leader
					if (GetComponent<EnemyManager> ().state == "searchPlayer" || GetComponent<EnemyManager> ().state == "attackPlayer") {
						//don't do anything if enemy is already searching for player or attacking him
					} 
					else { //otherwise alert leader
					GetComponent<EnemyManager> ().state = "alertLeader"; // if you're not the leader, go alert the leader
					}
				}
				lastPlayerPos = player.transform.position;
				ohShit = true;
			}
		}
	}



	//this keeps the last known position of leader depending on where the enemy saw him
	void WhereLeader () {
		Vector3 dir = leader.transform.position - transform.position; //  angle toward leader
		dir.y = 0;
		float angle = Vector3.Angle (dir, transform.forward); // angle toward leader
		if (Random.Range (0, 20/awareness) < 1 ) { // might or might not notice depending on awareness
			// if distance is close enough and within field of view then. Also I set distance vision at 8 + awareness/3 and distance angle at 60 degrees + awareness. mess around as necessary
			if (Vector3.Distance (leader.transform.position, transform.position) < (viewDistance + awareness * 3) && angle < (viewAngle + awareness)) { //awareness of leader is a significantly higher btw
				GetComponent<EnemyManager> ().leaderPos = leader.transform.position; //updates leader's last position if you see him
				if (GetComponent<EnemyManager> ().state == "findLeader") { //if you are looking for the leader right now and you see him
					GetComponent<EnemyManager> ().state = "alertLeader"; // go and alert him
				}
			}
		}
		if (Vector3.Distance (leader.transform.position, transform.position) < (awareness)) { //if leader is nearby in any direction (you can hear him chatting i guess)
			GetComponent<EnemyManager> ().leaderPos = leader.transform.position; //updates leader's last position if you hear him
		}
	}

	// if buddy in vicinity is looking for leader go look for leader yourself
	void BuddyAlert () {
		for (int i = 0; i < enemies.Length; i++) { //cycles through all the buddies
			if (!ohShit && enemies [i].gameObject != this.gameObject) { //scan through everyone but yo'self
				if (Vector3.Distance (transform.position, enemies [i].transform.position) < (awareness * 2.5f)) { // if within hearing range (which depends on awareness) then...
					if (enemies [i].GetComponent<EnemyLookOut> ().ohShit) { // if the other enemy is looking for leader...
						Invoke ("TurnOnAlert", Random.Range (1f, 3f)); // then help them look for leader (adding small delay so reaction is not instant)
					} else if (enemies [i].GetComponent<EnemyLookOut> ().arrowHitOhShit) { //if anyone nearby is taking cover from an arrow
						arrowHitOhShit = true;
					}
				}
			}
		}
	}

	// go take a closer look
	void CloserLook () {
		Vector3 dir = lastPlayerPos - transform.position; //goes to take a closer look at last known place
		if (Vector3.Distance (transform.position, lastPlayerPos) < 3f) { //if gets close and still no player, then go back to normal behavior
			reset.ResetAnimation();
			anim.SetBool ("isIdle", true);
			Invoke("Hmm",Random.Range(2,10)); // wait 2 seconds wihle looks around. i made it a separate method so i could invoke it after a brief delay
		} else { 
			dir.y = 0;
			reset.ResetAnimation ();
			anim.SetBool ("isCrouch", true);
			anim.speed = 1f + (moveSpeed / 10); //walk at a normal speed
			transform.forward = Vector3.RotateTowards (transform.forward, dir, moveSpeed * Time.deltaTime, 0); // rotate toward last kown leader position
			transform.Translate (0, 0, moveSpeed * Time.deltaTime); // move toward the last known leader position
		}
	}

	//i made this a separate method so i could invoke it after a brief delay at the end of CloserLook ()
	void Hmm () {
		closerLook = false;
		arrowHuh = false;
		if (!ohShit) {
			GetComponent<EnemyManager> ().state = "normal"; // set state back to normal
			//awareness = awareness * 1.1f; // but makes them more careful and more aware
		}
		reset.ResetAnimation();
	}

	// if an arrow whizzes by
	void ArrowAlert () {
		GameObject[] arrows = GameObject.FindGameObjectsWithTag ("Arrow"); //lookup all the game objects that are arrows
		if (arrows.Length > 0) {
			for (int i = 0; i < arrows.Length; i++) { //cycles through all the arrows
				if (arrows [i].GetComponent<Rigidbody> ().velocity.magnitude > 20f) { //if the thing is actually flying quickly
					if (Vector3.Distance (arrows [i].transform.position, transform.position) < awareness) { //if an arrow flies nearby
						reset.ResetAnimation ();
						anim.SetBool ("isIdle", true);
						arrowDirection = -arrows [i].transform.forward; // figure out which direction arrow came from
						arrowDirection.x = 0;
						arrowDirection.y = 0;
						arrowHuh = true; //set alert
						GetComponent<EnemyManager> ().state = "suspicious"; // set state to suspicious
					}
				}
			}
		}
	}

	void ArrowHuh () {
		//transform.forward = Vector3.RotateTowards (transform.forward, arrowDirection, moveSpeed * Time.deltaTime, 0); // rotate to where arrow came from
		reset.ResetAnimation ();
		anim.SetBool ("isIdle", true);
		Invoke("Hmm",(Random.Range(5,10))); //I'm making them wait a litle bit to look around before going back to their biznass
	}

	void HeardArrowHitting () {
		GameObject[] arrowHitSound = GameObject.FindGameObjectsWithTag ("arrowHitSound");
		for (int i = 0; i < arrowHitSound.Length; i++) { //cycles through all the arrow sounds (there should really be just one sound)
			if (Vector3.Distance (arrowHitSound [i].transform.position, transform.position) < (awareness * 2f)) { //if an arrow hit nearby so enemy hears it
				reset.ResetAnimation ();
				anim.SetBool ("isIdle", true);
				GetComponent<EnemyManager> ().state = "suspicious"; // set state to suspicious
				arrowDirection = -arrowHitSound [i].transform.forward; // figure out which direction arrow came from
				arrowDirection.y = 0;
				arrowDirection.x = 0;
				//transform.forward = Vector3.RotateTowards (transform.forward, arrowHitSound[i].transform.forward, moveSpeed * Time.deltaTime, 0); // rotate aware from where arrow came from
				arrowHitOhShit = true;
				Invoke("ArrowOhShit",(Random.Range(0.8f,1.2f))); //I'm making them wait a litle bit to figure out what just happened.. not ideal way to structure it I know
			}
		}
	}

	void ArrowOhShit (){
		GetComponent<EnemyManager> ().state = "takeCover"; // set state to takeCover
	}

	//i made this a separate method so i could invoke it after a brief delay at the end of BuddyAlert ()
	void TurnOnAlert () {
		GetComponent<EnemyManager> ().state = "alertLeader"; 
		ohShit = true;
	}

	// if see dead body
	void DeadBodyAlert () {
		for (int i = 0; i < enemies.Length; i++) { //cycles through all the buddies
			if (!enemies[i].GetComponent<EnemyManager>().alive & !ohShit) { //if they're dead and you aren't already in a state of alert
				Vector3 dir = enemies[i].transform.position - transform.position; //  calculate the angle toward dead buddy
				dir.y = 0;
				float angle = Vector3.Angle (dir, transform.forward);
				if (Vector3.Distance (transform.position, enemies [i].transform.position) < (awareness * 2f) && angle < (viewAngle + awareness)) { // if body is within visual range
					lastDeadBody = enemies[i].transform.position; //remember where you saw the body
				}
			}
		}
		if (lastDeadBody != Vector3.zero && !ohShit) { //if you've actually seen a dead body 
			DeadBodyCloserLook (); //go take a closer look
		}
	}

	void DeadBodyCloserLook () {
		if (Vector3.Distance (lastDeadBody, transform.position) < 2f) { //if you have seen a dead body and it is right in front of you
			TurnOnAlert();
			reset.ResetAnimation ();
			anim.SetBool ("isIdle", true); //stand around for a bit and look at it
			GetComponent<EnemyManager>().morale = GetComponent<EnemyManager>().morale--;
		} else { //otherwise run to the dead body
			Vector3 dir = lastDeadBody - transform.position; //  angle toward enemy
			dir.y = 0;
			reset.ResetAnimation ();
			anim.SetBool ("isRun", true);
			anim.speed = 1f + (moveSpeed / 10); //walk at a normal speed
			transform.forward = Vector3.RotateTowards (transform.forward, dir, moveSpeed * Time.deltaTime, 0); // rotate toward last kown leader position
			transform.Translate (0, 0, moveSpeed * Time.deltaTime); // move toward the last known leader position
		}
	}

	void GetStats () {
		awareness = GetComponent<EnemyStats> ().awareness; //pulls from the randomly generated stat for this enemy
		moveSpeed = GetComponent<EnemyStats> ().moveSpeed; //pulls from the randomly generated stat for this enemy
		player = GetComponent<EnemyManager>().player;
		leader = GetComponent<EnemyManager>().leader;
		enemies = GetComponent<EnemyManager> ().enemies;
		ready = true;
	}
}
