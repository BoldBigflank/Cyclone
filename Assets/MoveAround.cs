using UnityEngine;
using System.Collections;

public class MoveAround : MonoBehaviour {
	// World Stat
	bool gameIsRunning; // Duplicate
	public LayerMask layerMask;

	// Character stats
//	CharacterController controller;
	GameObject gameController;
	public float startingSpeed = 20.0F;
	public AudioClip crashSound;
	float speed = 270.0F;
	float forwardSpeed = 30.0F;
	public static float score = 0.0F;
	public float startX = 0.0F;
	public float startY = -4.575F;
	Vector3 lastPosition;
	float totalTime;

	float yaw;

	// Use this for initialization
	void Start () {
		Input.simulateMouseWithTouches = true;
		forwardSpeed = 0.0F;
		gameIsRunning = false;
//		controller = GetComponent<CharacterController>();
		gameController = GameObject.FindGameObjectWithTag("GameController");
//		controlByYaw = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(gameIsRunning) {
			totalTime += Time.deltaTime;
			forwardSpeed = startingSpeed + Mathf.Log (1.0F + 0.25F * totalTime)*20.0F;
			int rotateDirection = 0;
			if( Input.touchCount > 0 ){ // Touch control
				Debug.Log ("Touched" + Input.touches[Input.touchCount-1]);
				Touch lastTouch = Input.touches[Input.touchCount-1];
				Vector2 touchPos = lastTouch.position;
				if(touchPos.x < Screen.width/2) rotateDirection = -1;
				else rotateDirection = 1;
				yaw += rotateDirection * speed * Time.deltaTime;
//				transform.RotateAround(Vector3.zero, Vector3.forward, rotateDirection * speed * Time.deltaTime); // Touch Input
			} else {
				// Rotate the sphere around the middle axis with <-/-> and A/D
				yaw += Input.GetAxis("Horizontal") * speed * Time.deltaTime;
//				transform.RotateAround(Vector3.zero, Vector3.forward, Input.GetAxis("Horizontal") * speed * Time.deltaTime); // Arrow keys

			}
			transform.position = new Vector3(0.0F, -4.575F, transform.position.z + forwardSpeed * Time.deltaTime);
			transform.RotateAround(Vector3.zero, Vector3.forward, yaw); 

//			controller.Move(Vector3.forward * forwardSpeed * Time.deltaTime ); // Auto


			// Check for collisions
			Vector3 direction = transform.position - lastPosition;
			
			RaycastHit hit;
			if(Physics.SphereCast (lastPosition, GetComponent<SphereCollider>().radius, direction, out hit, direction.magnitude ))
			{
				Debug.Log("Collided"+ "(" + hit.point.x + ", " + hit.point.y + ", " + hit.point.z + ")");
//				transform.position = hit.point;
				audio.PlayOneShot(crashSound);
				GameOver();
				gameController.SendMessage ("GameOver");
			}
			
			this.lastPosition = transform.position;
		}

	}

	public void SetYaw(float y){
//		controlByYaw = true;
		yaw = y;
	}

	public void setControlByYaw(bool c){
//		controlByYaw = c;
	}

	void Reset(){
		Debug.Log ("MoveAround - Reset");
		lastPosition = new Vector3(startX, startY, 10.0F);
		transform.position = new Vector3(startX, startY, 10.0F);
		forwardSpeed = startingSpeed;
		gameIsRunning = true;
		totalTime = 0;
	}

	void GameOver(){
		forwardSpeed = 0.0F;
		gameIsRunning = false;
	}

	void FixedUpdate(){

	}

	void OnTriggerEnter(Collider obj) {
		if(obj.tag == "Obstacle"){
//			Debug.Log("Collided"+ obj.tag + "(" + obj.transform.position.x + ", " + obj.transform.position.y + ", " + obj.transform.position.z + ")");
//			obj.renderer.material.SetColor ("_Color", Color.black);
//			GameOver();
//			gameController.SendMessage ("GameOver");
		}

	}

}