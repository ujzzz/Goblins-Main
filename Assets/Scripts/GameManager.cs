using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public bool playerAlive = true;

	// Use this for initialization
	void Start () {
	
	}

	void OnTriggerEnter(Collider other) {
		if (other.CompareTag ("Enemy") && playerAlive == true) {
			playerAlive = false;
		}
	}
}
