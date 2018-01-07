using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreCounter : MonoBehaviour {

	public static int humansRemaining;
	Text scoreCounterText;

	// Use this for initialization
	void Start () {
		Invoke ("CountHumans", 0.2f);
		scoreCounterText = GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		scoreCounterText.text = "Humans Remaining in Area: " + humansRemaining;
	}

	void CountHumans() {
		humansRemaining = GameObject.FindGameObjectsWithTag ("Enemy").Length;
	}
}
