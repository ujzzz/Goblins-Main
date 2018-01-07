using UnityEngine;
using System.Collections;

// I stole a lot of this shit from video *Unity3D Character Controller - Camera Orbit* 

public class camMov : MonoBehaviour {
	[SerializeField]
	public Transform target;
	public float lookSmooth = -0.09f;
	[SerializeField]
	private float cameraFollowSpeed = 10f;
	private float zoom;

	[System.Serializable]
	public class PositionSettings {
		public Vector3 targetPosOffset = new Vector3 (0, 0, 0);
		public float lookSmooth = 100f;
		public float distanceFromTarget = -15;
		public float zoomSmooth = 2;
		public float maxZoom = -2;
		public float minZoom = -40;
	}
	[System.Serializable]
	public class OrbitSettings {
		public float xRotation = -20;
		public float yRotation = 180;
		public float maxRotation = -25;
		public float minRotation = 85;
		public float vOrbitSmooth = 250;
		public float hOrbitSmooth = 250;
	}

	[System.Serializable]
	public class InputSettings {
		public string ORBIT_HORIZONTAL_SNAP = "OrbitHorizontalSnap";
		public string ORBIT_HORIZONTAL = "OrbitHorizontal";
		public string ORBIT_VERTICAL = "OrbitVertical";
		public string ZOOM = "Mouse ScrollWheel";
	}

	public PositionSettings position = new PositionSettings();
	public OrbitSettings orbit = new OrbitSettings();
	public InputSettings input = new InputSettings();

	Vector3 targetPos = Vector3.zero;
	Vector3 destination = Vector3.zero;
	CharacterController charController;
	float vOrbitInput, hOrbitInput, zoomInput, hOrbitSnapInput;

	// Use this for initialization
	void Start () {
		targetPos = target.position + position.targetPosOffset;
		destination = Quaternion.Euler (orbit.xRotation, orbit.yRotation, 0) * -Vector3.forward * position.distanceFromTarget;
		//destination = Quaternion.Euler (orbit.xRotation, orbit.yRotation + target.eulerAngles.y, 0) * -Vector3.forward * position.distanceFromTarget;
		destination += targetPos;
		transform.position = destination;
		}

	void GetInput () {
		vOrbitInput = Input.GetAxisRaw (input.ORBIT_VERTICAL);
		hOrbitInput = Input.GetAxisRaw (input.ORBIT_HORIZONTAL);
		hOrbitSnapInput = Input.GetAxisRaw (input.ORBIT_HORIZONTAL_SNAP);
		zoomInput = Input.GetAxisRaw (input.ZOOM);
	}

	// Doing this in LateUpdate so camera is less jittery in Perspective mode (because it follows the player frame by frame)
	void LateUpdate () {
		GetInput ();
		OrbitTarget ();
		ZoomInOnTarget ();
		MoveToTarget ();
		LookAtTarget ();
		OrbitTarget ();

		if (Input.GetAxisRaw ("Mouse ScrollWheel") != 0) {
			zoom = Input.GetAxisRaw ("Mouse ScrollWheel");
			if (zoom < 0) {
				Camera.main.orthographicSize = Camera.main.orthographicSize + 0.5f;
				zoom = 0;
			}
			else {
				Camera.main.orthographicSize = Camera.main.orthographicSize - 0.5f;
				zoom = 0;
			}
		}

		//if you want to change between perspective and orthographic cameras, press o and p
		if (Input.GetKeyDown ("o")) {
			Camera.main.orthographic = true;
		}
		if (Input.GetKeyDown ("p")) {
			Camera.main.orthographic = false;
		}
	}

	void MoveToTarget (){
		targetPos = target.position + position.targetPosOffset;
		destination = Quaternion.Euler (orbit.xRotation, orbit.yRotation, 0) * -Vector3.forward * position.distanceFromTarget;
		//destination = Quaternion.Euler (orbit.xRotation, orbit.yRotation + target.eulerAngles.y, 0) * -Vector3.forward * position.distanceFromTarget;
		destination += targetPos;
		transform.position = Vector3.Lerp (transform.position, destination, cameraFollowSpeed * Time.deltaTime);
	}
	
	void LookAtTarget () {
		Quaternion targetRotation = Quaternion.LookRotation (targetPos - transform.position);
		transform.rotation = Quaternion.Lerp (transform.rotation, targetRotation, position.lookSmooth * Time.deltaTime);
		}

	void OrbitTarget(){
		if (hOrbitSnapInput > 0) {
			orbit.yRotation = -180;
		}

		orbit.xRotation += -vOrbitInput * orbit.vOrbitSmooth * Time.deltaTime;
		orbit.yRotation += -hOrbitInput * orbit.hOrbitSmooth * Time.deltaTime;

		if (orbit.xRotation > orbit.maxRotation) {
			orbit.xRotation = orbit.maxRotation;
		}
		if (orbit.xRotation > orbit.minRotation) {
			orbit.xRotation = orbit.minRotation;
		}
	}

	void ZoomInOnTarget(){
		// the zoom is weird in orthographic mode, so I don't active it then
		if (Camera.main.orthographic == false) {
			position.distanceFromTarget += zoomInput * position.zoomSmooth;
			if (position.distanceFromTarget > position.maxZoom) {
				position.distanceFromTarget = position.maxZoom;
			}
			if (position.distanceFromTarget < position.minZoom) {
				position.distanceFromTarget = position.minZoom;
			}
		}
	}
}