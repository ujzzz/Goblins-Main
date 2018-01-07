using UnityEngine;
using System.Collections;

public class EnemyStats : MonoBehaviour {

	public float moveSpeed; // this is what other scripts will use for speed
	public float awareness; // this is what other scripts will use for awareness 
	public float bravery; // who is the leader
	[SerializeField]
	private float maxMoveSpeed; //this is what you can set
	[SerializeField]
	private float minMoveSpeed; //this is what you can set
	[SerializeField]
	private float minAwareness; //this is what you can set
	[SerializeField]
	private float maxAwareness; //this is what you can set
	[SerializeField]
	private float minBravery; //this is what you can set
	[SerializeField]
	public float maxBravery; //this is what you can set; this is public so that I can calculate just how brave a certain enemy is
	public float morale = 100f;

	// picks a random number between the allowed min and max for each stat 
	void Start () {
		moveSpeed = Random.Range (minMoveSpeed, maxMoveSpeed); 
		awareness = Random.Range (minAwareness, maxAwareness);
		bravery = Random.Range (minBravery, maxBravery);
	}

}
