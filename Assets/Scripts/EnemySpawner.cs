using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour {

	[SerializeField]
	GameObject enemyPrefab; //loads enemy prefab
	GameObject player;
	[SerializeField]
	Transform ground; //identifies ground

	[SerializeField]
	float maxEnemy; 
	[SerializeField]
	float minEnemy;
	float enemyNumber;

	Vector3 spawnPoint; // initial spawn point 
	Vector3 lastSpawnPoint; //subsequent spawn point
	float positionX;
	float positionZ;

	[SerializeField]
	float spawnRange;

	// Use this for initialization
	void Start () {
		enemyNumber = Random.Range(0f,maxEnemy - minEnemy); //randomizes how many players to spawn
		player = GameObject.FindGameObjectWithTag ("Player"); //identify the player
		SpawnPoint ();
		Spawner ();
	
	}

	void SpawnPoint () {
		positionZ = player.transform.position.z * -1; // x position is opposite of the player (need to adjust later if player spwan point is randomized)
		positionX = Random.Range (ground.localScale.x / 2, ground.localScale.x / -2) - maxEnemy; // z position is maximum of the ground, with a buffer I randomly decided on the maxEnemy
		spawnPoint = new Vector3 (positionX, 0.8f, positionZ);
		lastSpawnPoint = spawnPoint;
	}

	void Spawner () {
		while (enemyNumber < maxEnemy) { //spawn them shits
			Instantiate (enemyPrefab, lastSpawnPoint, Quaternion.identity);
			enemyNumber++;
			EnemyHuddleGeneration (); //run a randomizer so the next guy is spawned in a diff place
		}
	}

	void EnemyHuddleGeneration () {
		//moves the next spawn point away by the size of the enemyPrefab in a random direction, for both x and z separately
		lastSpawnPoint.z = lastSpawnPoint.z + enemyPrefab.transform.localScale.z * Random.Range(-spawnRange,spawnRange);
		lastSpawnPoint.x = lastSpawnPoint.x + enemyPrefab.transform.localScale.x * Random.Range(-spawnRange,spawnRange);

		// just in case X is off the ground grid, adjust it down by a random amount i decided on (0.6f)
		if (lastSpawnPoint.x > (ground.localScale.x / 2) || lastSpawnPoint.x < (ground.localScale.x / -2)) {
			lastSpawnPoint.x = Random.Range((ground.localScale.x / -2), (ground.localScale.x / 2));
		}
		// just in case Z is off the ground grid, adjust it down by a random amount i decided on (0.6f)
		if (lastSpawnPoint.z > (ground.localScale.z / 2) || lastSpawnPoint.z < (ground.localScale.z / -2)) {
			//lastSpawnPoint.z = lastSpawnPoint.z * 0.4f;
			lastSpawnPoint.z = Random.Range((ground.localScale.z / -2), (ground.localScale.z / 2));
		}
	}
}
