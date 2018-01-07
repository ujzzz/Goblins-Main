using UnityEngine;
using System.Collections;

public class enemyGoalSeek : MonoBehaviour {

	Animator anim;
	public GameObject[] allGoals;
	public GameObject currentGoal;
	float goalSize;
	float moveSpeed;
	float awareness;
	bool ready; //this is just a thing that waits until everything is ready and loaded before moving the enemy
	EnemyResetAnims reset;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
		goalSize = GameObject.Find ("GameManager").GetComponent<TreeSpawner> ().spawnSize * 0.8f; //decides what kind of tree they should look for by biggest size available, and adds a 20% buffer
		Invoke("GetStats",0.1f); // waits 100 milliseconds until all scripts are loaded
		Invoke("FindGoals", 0.1f); //waits 100 milliseconds until all trees are spawned
		Invoke("FindBiggest", 0.2f); //waits 200 milliseconds cuz i had trouble doing this too quickly [that's what she said]
		reset = GetComponent<EnemyResetAnims> ();
	}

	private void GetStats () {
		moveSpeed = transform.GetComponent<EnemyStats> ().moveSpeed; //pulls from the randomly generated stat for this enemy
		awareness = transform.GetComponent<EnemyStats> ().awareness; //pulls from the randomly generated stat for this enemy
	}

	// make a list of all spawned trees
	private void FindGoals (){
		allGoals = GameObject.FindGameObjectsWithTag ("Tree"); //finds every object with the Tree tag
	}
		
	//find the biggest tree nearby
	private void FindBiggest () {
		currentGoal = allGoals[0]; //sets the first tree to zero just for initialization purposes
		for (int i = 0; i < allGoals.Length; i++) { //run a loop until we go through every tree in the array
			if (Vector3.Distance (transform.position, allGoals [i].transform.position) < awareness * 1.5) { // if the tree is nearby; distance determined by enemy's awareness level
				if (allGoals [i].transform.localScale.x > (goalSize * 0.8) && allGoals[i].transform.localScale.x > currentGoal.transform.localScale.x) { //and if the tree is of a decent size and bigger than current selection is there
					currentGoal = allGoals [i]; //then set this tree to be the goal
				}
			}
		}
		ready = true; //tells Update method that everything is ready to move the enemy
	}

	void Update () {
		if (ready) {
			MoveToGoal (); //constantly runs to find the target tree
			if (Random.Range (0, 7000) < 1) { // every 7000 frames they might go and randomly find a new tree cuz they tired of grinding this one
				NewTarget ();
			}
		}
	}

	private void MoveToGoal () {
		Vector3 direction = currentGoal.transform.position - transform.position;//sets the direction toward the target tree
		direction.y = 0;
		transform.rotation = Quaternion.Slerp(transform.rotation,Quaternion.LookRotation(direction),(moveSpeed/2) * Time.deltaTime); // this turns toward the tree
		Vector3 bootyDirection = -direction; // this is for the twerk lel
		bootyDirection.y = 0; // make sure that booty isn't pointed too high or too low
		if (Vector3.Distance (transform.position, currentGoal.transform.position) < 5) { // if the enemy is within range of the tree, start twerking
			transform.forward = bootyDirection; // this makes him turn his back to the tree so he grind it. I can't make them turn slowly tho dunno why even tho it works for grinding player
			reset.ResetAnimation ();
			anim.SetBool ("isTwerk", true);
			anim.speed = Random.Range (1f, 1.2f); // twerk at different speeds
		} else {
			transform.Translate (0, 0, (moveSpeed * 0.6f) * Time.deltaTime); // this moves him toward the tree
			reset.ResetAnimation ();
			anim.SetBool ("isWalking", true);
			anim.speed = 0.8f + (moveSpeed/10); //walk at a normal speed
		}
	}

	// this finds them a random new target (not by tree size)
	public void NewTarget () {
		int i = Random.Range (0, allGoals.Length); // just ramdomly choose one tree from all the possibly targets
		currentGoal = allGoals [i];
		reset.ResetAnimation ();
		anim.SetBool ("isWalking", true);
	}
}
