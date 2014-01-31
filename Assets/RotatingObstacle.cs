using UnityEngine;
using System.Collections;

public class RotatingObstacle : MonoBehaviour {
	public float speed = 30.0F;
	public int direction = 1;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.RotateAround(Vector3.zero, Vector3.forward, direction * speed * Time.deltaTime);
	}
}
