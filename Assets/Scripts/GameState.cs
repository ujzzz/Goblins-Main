using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameState : MonoBehaviour {

	bool gameStarted = false;

	[SerializeField]
	Text gameStateText;

	[SerializeField]
	GameObject player;
	[SerializeField]
	TreeSpawner treeSpawner;
	[SerializeField]
	EnemySpawner enemySpawner;
	[SerializeField]
	camMov camMov;

	// Use this for initialization
	void Start () {
		
		gameStarted = false;
		player.SetActive (false);
		treeSpawner.enabled = false;
		enemySpawner.enabled = false;
		camMov.enabled = false;

	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyUp(KeyCode.Space)) {
			StartGame();
		}
			
		if (Input.GetKeyUp(KeyCode.Tab)){
			EndGame();
		}
			
	
	}

	void StartGame () {
		gameStarted = true;
		player.SetActive (true);
		treeSpawner.enabled = true;
		enemySpawner.enabled = true;
		camMov.enabled = true;
	}

	void EndGame () {
		gameStarted = false;
		player.SetActive (false);
		treeSpawner.enabled = false;
		enemySpawner.enabled = false;
		camMov.enabled = false;
	}
}
