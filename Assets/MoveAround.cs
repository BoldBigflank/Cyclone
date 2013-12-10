using UnityEngine;
using System.Collections;

public class MoveAround : MonoBehaviour {
	// Character stats
	CharacterController controller;
	float speed = 3.0F;
	float forwardSpeed = 30.0F;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		CharacterController controller = GetComponent<CharacterController>();

		// Rotate the sphere around the middle axis with <-/-> and A/D
		transform.RotateAround(Vector3.zero, Vector3.forward, Input.GetAxis("Horizontal") * speed);

		// Move the sphere forward with ^/v for now
		Vector3 forward = transform.TransformDirection(Vector3.forward); // Not sure if should use this
		float curSpeed = forwardSpeed * Input.GetAxis("Vertical");

//		controller.Move(Vector3.forward * forwardSpeed * Time.deltaTime );
		controller.Move(Vector3.forward * curSpeed * Time.deltaTime );


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