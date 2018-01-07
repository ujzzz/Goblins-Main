using UnityEngine;
using System.Collections;

public class arrowHit : MonoBehaviour {

	Animator anim;
	EnemyResetAnims reset;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
		reset = GetComponent<EnemyResetAnims> ();
	}
		
	void OnCollisionEnter (Collision other) {
		if (other.gameObject.tag == "Arrow") {
			reset.ResetAnimation ();
			anim.SetBool ("isDead", true);
			GetComponent<EnemyManager> ().alive = false;
			GetComponent<EnemyManager> ().amLeader = false;
		}
	}
}
