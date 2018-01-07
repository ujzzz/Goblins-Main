﻿using UnityEngine;
using System.Collections;

//i ripped this off some website

public class EnemyDrawFOV : MonoBehaviour {

	int quality = 15;
	Mesh mesh;
	public Material material;
	float angle_fov = 60;
	float dist_min = 5;
	float dist_max = 15;

	// Use this for initialization
	void Start () {
		angle_fov = 60 + GetComponent<EnemyStats>().awareness;
		dist_max = 15 + (GetComponent<EnemyStats>().awareness/3);
		mesh = new Mesh ();
		mesh.vertices = new Vector3[4 * quality];
		mesh.triangles = new int[3 * 2 * quality];

		Vector3[] normals = new Vector3[4 * quality];
		Vector2[] uv = new Vector2[4 * quality];

		for (int i = 0; i < uv.Length; i++){
			uv [i] = new Vector2 (0, 0);
		}

		for (int i = 0; i < normals.Length; i++){
			normals [i] = new Vector3 (0, 1, 0);
		}

		mesh.uv = uv;
		mesh.normals = normals;
	
	}
	
	// Update is called once per frame
	void Update () {

		float angle_lookat = GetEnemyAngle ();
		float angle_start = angle_lookat - angle_fov;
		float angle_end = angle_lookat + angle_fov;
		float angle_delta = (angle_end - angle_start) / quality;

		float angle_curr = angle_start;
		float angle_next = angle_start + angle_delta;

		Vector3 pos_curr_min = Vector3.zero;
		Vector3 pos_curr_max = Vector3.zero;

		Vector3 pos_next_min = Vector3.zero;
		Vector3 pos_next_max = Vector3.zero;

		Vector3[] vertices = new Vector3[4 * quality];
		int[] triangles = new int[3 * 2 * quality];

		for (int i = 0; i < quality; i++) {
			Vector3 sphere_curr = new Vector3 (
				                      Mathf.Sin (Mathf.Deg2Rad * (angle_curr)), 0,
				                      Mathf.Cos (Mathf.Deg2Rad * (angle_curr)));

			Vector3 sphere_next = new Vector3 (
				Mathf.Sin (Mathf.Deg2Rad * (angle_next)), 0,
				Mathf.Cos (Mathf.Deg2Rad * (angle_next)));

			pos_curr_min = transform.position + sphere_curr * dist_min;
			pos_curr_max = transform.position + sphere_curr * dist_max;

			pos_next_min = transform.position + sphere_next * dist_min;
			pos_next_max = transform.position + sphere_next * dist_max;

			int a = 4 * i;
			int b = 4 * i + 1;
			int c = 4 * i + 2;
			int d = 4 * i + 3;

			vertices [a] = pos_curr_min;
			vertices [c] = pos_curr_max;
			vertices [b] = pos_next_max;
			vertices [d] = pos_next_min;

			triangles [6 * i] = a;
			triangles [6 * i + 1] = b;
			triangles [6 * i + 2] = c;
			triangles [6 * i + 3] = c;
			triangles [6 * i + 4] = d;
			triangles [6 * i + 5] = a;

			angle_curr += angle_delta;
			angle_next += angle_delta;
		}

		mesh.vertices = vertices;
		mesh.triangles = triangles;

		Graphics.DrawMesh (mesh, Vector3.zero, Quaternion.identity, material, 0);
	
	}

	float GetEnemyAngle (){
		return 90 - Mathf.Rad2Deg * Mathf.Atan2 (transform.forward.z, transform.forward.x);
	}
}
