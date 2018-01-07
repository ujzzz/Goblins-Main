using UnityEngine;
using System.Collections;

public class shootArrow : MonoBehaviour {

	public Animator anim;
	[SerializeField]
	GameObject arrowPrefab;
	Vector3 arrowOrigin;
	Vector3 arrowTerminal;
	[SerializeField]
	float arrowSpeed;
	float maximumArrowForce = 35f;
	Vector3 arrowSpawn;

	float lerpTime = 1f;
	float currentLerpTime;
	public GameObject mainCamera;
	public GameObject shootCamera;

	//crap for the mouse aim method
	[SerializeField]
	Vector2 mouseLook;
	Vector2 smoothV;
	public float sensitivity = 5f;
	public float smoothing = 2f;

	//stuff for trajectory arc
	public float initialVelocity = 10.0f;
	public float timeResolution = 0.02f;
	public float maxTime = 10f;
	LineRenderer lineRenderer;
	public GameObject trajectoryIndicator;
	GameObject trajectoryIndicatorPosition;

	[SerializeField]
	LayerMask myLayerMask; 

	[SerializeField]
	float arrowForce; // how fast arrow is shot out (make this faster/slower depending on player skill? or give them ability to shoot strong or weak perhaps, somehow)


	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
		// loads the arrow as a resource to be used later
		arrowPrefab = Resources.Load ("Arrow") as GameObject;
		// sets athe minimum amount of time the goblin needs to pull the bow back to aim it and shit.. so you don't insta-fire
		currentLerpTime = -2f;
		lineRenderer = GetComponent<LineRenderer> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (InventoryUI.arrowsLeft > 0) {
			// checks if right mouse button is pressed
			SetArrow ();

			// check if right mouse button is release and fire that shit
			ReleaseArrow ();
		}
	}

	void SetArrow () {
		if (Input.GetMouseButtonDown (1)) {
			transform.GetComponent<PlayerMovement> ().enabled = false;
			anim.SetBool ("Release", false);
			// if button is pressed, find where mouse is pointing to 
//			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
//			RaycastHit hit;
//			// figure out where exactly on game map that line intersects and gets the exact location
//			if (Physics.Raycast (ray, out hit)) {
//				arrowOrigin = hit.point;
//			}
			//turns off main camera so we switch to the shooting one (which is next in the hiearchy)
			mainCamera.SetActive (false);

			//point the mouseAim in the same direction we are facing now (I HAVE NO IDEA WHY THE FUCK MY X AND Y ARE INVERSE BUT IT WORKS)
			mouseLook.x = transform.rotation.eulerAngles.y;
			mouseLook.y = transform.rotation.eulerAngles.x;

		}

		// depending on how long the dude is holding it for, add more strength to the shot
		if (Input.GetMouseButton (1)) {
			
			currentLerpTime += Time.deltaTime;
			if (currentLerpTime > lerpTime)
				currentLerpTime = lerpTime;

			// Lerp the arrow force the longer the player holds down the button.
			float perc = currentLerpTime / lerpTime;
			arrowSpeed = Mathf.Lerp (0f, maximumArrowForce, perc);
			anim.SetBool ("Aim", true);

			//as long as right mouse button is held down run the mouseAim method which is basically an FPS controller for choosing where to shoot arrows
			MouseAim ();
			Trajectory ();
		} 
	}

	private void ReleaseArrow() {
		if (Input.GetMouseButtonUp (1)) {
			
			//once arrow is released, switch the main camera back on (and in the ArrowLogic script, it will switch the Main Camera to the Arrow Camera temporarily. See that script if you wants to fux wit it)
			mainCamera.SetActive(true);
			anim.SetBool ("Release", true);
			anim.SetBool ("Aim", false);


			arrowSpawn = transform.position; 
			arrowSpawn.y += 1f;
			GameObject arrow = Instantiate (arrowPrefab, arrowSpawn, Quaternion.identity) as GameObject; // spawn that arrow at player's location and rotation
			arrow.transform.rotation = Quaternion.Euler(-mouseLook.y,mouseLook.x,0);
			//arrow.transform.rotation = Quaternion.LookRotation(transform.forward,transform.up);
			arrow.GetComponent<Rigidbody> ().AddForce (arrow.transform.forward * arrowForce); //shoot that arrow
			transform.GetComponent<PlayerMovement> ().enabled = true;

			lineRenderer.SetVertexCount(0); // deleting the trajectory line
			trajectoryIndicatorPosition.transform.position = new Vector3 (0f,-10f,0f); //just hiding that trajectory target ball beneath the map 

			InventoryUI.arrowsLeft--;

			// this is the part I used to use before we implemented FPS aim. This might be useful if we go back to top-down strategy mode where you click on a part of the screen and pull back to aim and shoot		 
//			Ray rayEnd = Camera.main.ScreenPointToRay (Input.mousePosition);
//			RaycastHit hitEnd;
//			if (Physics.Raycast (rayEnd, out hitEnd)) {
//				arrowTerminal = hitEnd.point;
//				// sets arrow target to where mouse was originally clicked
//				arrowOrigin = arrowOrigin - transform.position;
//				arrowSpawn = transform.position; 
//				arrowSpawn.y += 1.2f;
//				// creates the arrow right inside the goblin (need to adjust once model is ready so it spawns inside the bow basically)
//				GameObject arrow = Instantiate (arrowPrefab, arrowSpawn, transform.rotation) as GameObject;
//
//				// shoots that shit by applying a physics force that is based on how long the mouse was held down
//				arrowOrigin.y = arrowOrigin.y + 2f;
//				arrow.GetComponent<Rigidbody> ().AddForce (arrow.transform.forward * 1000f);
//				//sets arrow speed back to 0 for the next bow
//				arrowSpeed = 0f;
//				currentLerpTime = 0f;
//			}
		}
	}

	void MouseAim() {
		//this thing just tracks where the mouse is being moved. md stands for mouse delta
		var md = new Vector2 (Input.GetAxisRaw ("Mouse X"), Input.GetAxisRaw ("Mouse Y"));
		//this smoothes it out. I honestly don't know how it works. I got it from Holistic3D's youtube tutorial
		md = Vector2.Scale (md, new Vector2(sensitivity * smoothing, sensitivity * smoothing));
		smoothV.x = Mathf.Lerp (smoothV.x, md.x, 1f / smoothing);
		smoothV.y = Mathf.Lerp (smoothV.y, md.y, 1f / smoothing);
		mouseLook += smoothV;

		//and then I set the player's rotation in the direction the mouse is looking at. I don't know why the fuck my Y and X are inverted, but it works
		transform.rotation = Quaternion.Euler(-mouseLook.y,mouseLook.x,0);

	}

	void Trajectory() {

		Vector3 velocityVector = transform.forward * initialVelocity; //not sure what this does... it like moves forward bit by bit
		lineRenderer.SetVertexCount((int)(maxTime/timeResolution)); //this sets the granularity of the linerender curve
		int index = 0; //this is just to keep count
		Vector3 currentPosition = transform.position; //this is to keep track of where we are in the linerender curve
		currentPosition.y += 1.1f;

		//we do it this way because i assume an arrow won't fly forever, so there's basically a timelimit on it.. i don't know if i actually need this at all
		for (float t = 0.0f; t < maxTime; t += timeResolution) {
			lineRenderer.SetPosition(index,currentPosition); //draws a line at that point, before moving further

			// if linerender hits anything
			RaycastHit hit;
			if (Physics.Raycast (currentPosition,velocityVector,out hit,myLayerMask)) {
				lineRenderer.SetVertexCount (index + 2); //ends the linerender curve
				lineRenderer.SetPosition(index+1,hit.point); //sets the last point

				if (trajectoryIndicatorPosition != null) {
					trajectoryIndicatorPosition.transform.position = hit.point;
				} else {
					trajectoryIndicatorPosition = Instantiate (trajectoryIndicator, hit.point, Quaternion.identity) as GameObject; //create a thingie to indicate where arrow will probably hit
				}
				break; // stop making the line
			}

			currentPosition += velocityVector*timeResolution; // move up current position a bit more forward
			velocityVector += Physics.gravity * timeResolution; //add gravity tot hat shit

			index++; //move index up by one until we run out of time
		}

	}
		
}
	

