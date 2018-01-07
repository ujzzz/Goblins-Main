using UnityEngine;
using System.Collections;

public class EnemyResetAnims : MonoBehaviour {
	private Animator anim;

	void Start () {
		anim = GetComponent<Animator> ();
	}

	// Use this for initialization
	public void ResetAnimation () {
		anim.SetBool ("isWalking", false);
		anim.SetBool ("isIdle", false);
		anim.SetBool ("isTwerk", false);
		anim.SetBool ("isRun", false);
		anim.SetBool ("isCrouch", false);
		anim.SetBool ("isAlert", false);
	}
}
