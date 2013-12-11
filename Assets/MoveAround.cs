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
			}
			
			// Rotate the sphere around the middle axis with <-/-> and A/D
			//		transform.RotateAround(Vector3.zero, Vector3.forward, Input.GetAxis("Horizontal") * speed); // Arrow keys
			transform.RotateAround(Vector3.zero, Vector3.forward, rotateDirection * speed * Time.deltaTime); // Touch Input
			
			// Move the sphere forward with ^/v for now
			
			float curSpeed = forwardSpeed * Input.GetAxis("Vertical");
			
			controller.Move(Vector3.forward * forwardSpeed * Time.deltaTime ); // Auto
			//		controller.Move(Vector3.forward * curSpeed * Time.deltaTime ); // Manual forward movement
		}





	}

	void Reset(){
		Debug.Log ("MoveAround - Reset");
		transform.position = new Vector3(0.0F, -4.5F, 10.0F);
		forwardSpeed = startingSpeed;
		gameIsRunning = true;
	}

	void GameOver(){
		forwardSpeed = 0.0F;
		gameIsRunning = false;
	}

	int GetScore(){
		return 666;
	}

	void OnTriggerEnter(Collider obj) {
		if(obj.tag == "Obstacle"){
			Debug.Log("Collided"+ obj.tag + "(" + obj.transform.position.x + ", " + obj.transform.position.y + ", " + obj.transform.position.z + ")");
			obj.renderer.material.SetColor ("_Color", Color.black);
			GameOver();
			gameController.SendMessage ("GameOver");
		}
		//	if (obj.gameObject.name == "Shark") {
		//		//reset shark
		//		obj.gameObject.transform.rotation = Quaternion.identity;
		//		obj.gameObject.transform.position = new Vector3(20f, -3f, 8f);
		//		Destroy(this.gameObject);
		//	}
	}

	void OnGUI(){

	}

}



/*
 * var speed : float = 3.0;
 * var rotateSpeed : float = 3.0;
 * function Update () {
 *     var controller : CharacterController = GetComponent(CharacterController);
 *     // Rotate around y - axis
 *     transform.Rotate(0, Input.GetAxis ("Horizontal") * rotateSpeed, 0);
 * 
 *     // Move forward / backward
 *     var forward : Vector3 = transform.TransformDirection(Vector3.forward);
 *     var curSpeed : float = speed * Input.GetAxis ("Vertical");
 *     controller.SimpleMove(forward * curSpeed);
 * }
 * @script RequireComponent(CharacterController)
 */