using UnityEngine;
using System.Collections;

public class ShroomSpawner : MonoBehaviour {
	[SerializeField]
	GameObject shroomPrefab;
	[SerializeField]
	Transform ground;
	float maxSpawnArea;
	float shroomAmount;
	float positionX;
	float positionZ;
	Vector3 shroomPosition;


	// Use this for initialization
	void Start () {
		maxSpawnArea = ground.localScale.x / 2; //makes maximum spawn area no bigger than the ground area
		shroomAmount = ground.localScale.x / 10; //adjusts how much tree to the size of the map
	}
	
	// Update is called once per frame
	void Update () {
		while (shroomAmount> 0) {
			// decides where to spawn, but within range of map
			RandomizeSpawn ();

			// create the damn thing
			Instantiate(shroomPrefab, shroomPosition, Quaternion.identity);
		}
	}

	void RandomizeSpawn () {
		positionX = Random.Range (-maxSpawnArea, maxSpawnArea);
		positionZ = Random.Range (-maxSpawnArea, maxSpawnArea);
		shroomPosition = new Vector3 (positionX, 1.43f, positionZ);
	}
}
