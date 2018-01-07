using UnityEngine;
using System.Collections;

public class EnemyDrawFieldOfView : MonoBehaviour {

	float meshResolution;
	float angle;

	// Use this for initialization
	void Start () {
		angle = GetComponent<EnemyLookOut> ().viewAngle;
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void DrawFOV () {
		int stepCount = Mathf.RoundToInt (angle * meshResolution);
		float stepAngleSize = angle / stepCount;
	}
}
