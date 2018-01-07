using UnityEngine;
using System.Collections;

public class TreeSpawner : MonoBehaviour {

	[SerializeField]
	GameObject treePrefab;
	[SerializeField]
	public Transform ground;
	float treeAmount;
	[SerializeField]
	float treeAmountMultiplier;
	[SerializeField]
	float spawnArea = 3f;
	[SerializeField]
	public float spawnSize = 5f;
	float lastX = 0f;
	float lastZ = 0f;
	float maxSpawnArea;
	float positionX;
	float positionZ;
	Vector3 treePosition;

	// Use this for initialization
	void Start () {
		maxSpawnArea = ground.localScale.x / 2; //makes maximum spawn area no bigger than the ground area
		treeAmount = ground.localScale.x * treeAmountMultiplier; //adjusts how much tree to the size of the map
	}
	
	// Update is called once per frame
	void Update () {
		while (treeAmount > 0) {
			// decides where to spawn, but within range of the last spawn position
			RandomizeSpawn ();
			SpawnPointChecker ();
			TreeSize ();

			// create the damn thing
			Instantiate(treePrefab, treePosition, Quaternion.identity);
		}
	}

	private void RandomizeSpawn () {
		positionX = lastX + Random.Range (-spawnArea, spawnArea);
		positionZ = lastZ + Random.Range (-spawnArea, spawnArea);
	}

	private void SpawnPointChecker () {
		// if spawn point outside of map
		if (positionX > maxSpawnArea || positionX < -maxSpawnArea || positionZ > maxSpawnArea || positionZ < -maxSpawnArea) {
			// then select new random spawn point
			positionX = Random.Range (-maxSpawnArea, maxSpawnArea);
			positionZ = Random.Range (-maxSpawnArea, maxSpawnArea);
			treePosition = new Vector3 (positionX, 1.43f, positionZ);
		} 
		// otherwise its all good
		else { treePosition = new Vector3 (positionX, 1.43f, positionZ); }

		// stores spawn point to a field which we use to decide next spawn point
		lastX = positionX;
		lastZ = positionZ;
	}

	private void TreeSize () {
		// creates random size for the tree
		float scaleSize = Random.Range (3f, spawnSize);
		Vector3 treeSize = new Vector3 (scaleSize, scaleSize, scaleSize);
		treePrefab.transform.localScale = treeSize;

		// lowers total amount of tree left to spawn
		treeAmount = treeAmount - scaleSize;
	}
}
