using UnityEngine;
using System.Collections;

public class MoveAround : MonoBehaviour {
	// Character stats
	CharacterController controller;
	float speed = 180.0F;
	float forwardSpeed = 30.0F;

	// Use this for initialization
	void Start () {
		Input.simulateMouseWithTouches = true;
	}
	
	// Update is called once per frame
	void Update () {
		CharacterController controller = GetComponent<CharacterController>();

		int rotateDirection = 0;
		if( Input.GetMouseButton(0) ){
			Vector3 mousePos = Input.mousePosition;
			if(mousePos.x < Screen.width/2) rotateDirection = -1;
			else rotateDirection = 1;
		}

		// Rotate the sphere around the middle axis with <-/-> and A/D
//		transform.RotateAround(Vector3.zero, Vector3.forward, Input.GetAxis("Horizontal") * speed); // Arrow keys
		transform.RotateAround(Vector3.zero, Vector3.forward, rotateDirection * speed * Time.deltaTime); // Touch Input

		// Move the sphere forward with ^/v for now
		Vector3 forward = transform.TransformDirection(Vector3.forward); // Not sure if should use this
		float curSpeed = forwardSpeed * Input.GetAxis("Vertical");

//		controller.Move(Vector3.forward * forwardSpeed * Time.deltaTime ); // Auto
		controller.Move(Vector3.forward * curSpeed * Time.deltaTime ); // Manual forward movement



	}

	void OnTriggerEnter(Collider obj) {
		if(obj.tag == "Obstacle"){
			Debug.Log("Collided"+ obj.tag + "(" + obj.transform.position.x + ", " + obj.transform.position.y + ", " + obj.transform.position.z + ")");
			obj.renderer.material.SetColor ("_Color", Color.black);
		}
		//	if (obj.gameObject.name == "Shark") {
		//		//reset shark
		//		obj.gameObject.transform.rotation = Quaternion.identity;
		//		obj.gameObject.transform.position = new Vector3(20f, -3f, 8f);
		//		Destroy(this.gameObject);
		//	}
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