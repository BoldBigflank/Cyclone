using UnityEngine;
using System.Collections;

public class MoveAround : MonoBehaviour {
	// World Stat
	bool gameIsRunning; // Duplicate

	// Character stats
	CharacterController controller;
	GameObject gameController;
	public float startingSpeed = 30.0F;
	float speed = 180.0F;
	float forwardSpeed = 30.0F;
	public static float score = 0.0F;
	public float startX = 0.0F;
	public float startY = -4.5F;

	// Use this for initialization
	void Start () {
		Input.simulateMouseWithTouches = true;
		forwardSpeed = 0.0F;
		gameIsRunning = false;
		controller = GetComponent<CharacterController>();
		gameController = GameObject.FindGameObjectWithTag("GameController");
	}
	
	// Update is called once per frame
	void Update () {
		if(gameIsRunning) {
			forwardSpeed += Time.deltaTime;
			int rotateDirection = 0;
			if( Input.GetMouseButton(0) ){
				Vector3 mousePos = Input.mousePosition;
				if(mousePos.x < Screen.width/2) rotateDirection = -1;
				if(mousePos.x >= Screen.width/2) rotateDirection = 1;
				transform.RotateAround(Vector3.zero, Vector3.forward, rotateDirection * speed * Time.deltaTime); // Touch Input
			} else {
				// Rotate the sphere around the middle axis with <-/-> and A/D
				transform.RotateAround(Vector3.zero, Vector3.forward, Input.GetAxis("Horizontal") * speed * Time.deltaTime); // Arrow keys

			}

			controller.Move(Vector3.forward * forwardSpeed * Time.deltaTime ); // Auto

			//		float curSpeed = forwardSpeed * Input.GetAxis("Vertical"); // Manual forward movement
			//		controller.Move(Vector3.forward * curSpeed * Time.deltaTime ); 
		}

	}

	void Reset(){
		Debug.Log ("MoveAround - Reset");
		transform.position = new Vector3(startX, startY, 10.0F);
		forwardSpeed = startingSpeed;
		gameIsRunning = true;
	}

	void GameOver(){
		forwardSpeed = 0.0F;
		gameIsRunning = false;
	}

	void OnTriggerEnter(Collider obj) {
		if(obj.tag == "Obstacle"){
			Debug.Log("Collided"+ obj.tag + "(" + obj.transform.position.x + ", " + obj.transform.position.y + ", " + obj.transform.position.z + ")");
			obj.renderer.material.SetColor ("_Color", Color.black);
			GameOver();
			gameController.SendMessage ("GameOver");
		}

	}

}