using UnityEngine;
using System.Collections;

public class MoveAround : MonoBehaviour {
	// World Stat
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
	float slerpTime;

	float yaw;
	int reverseControls;

	// Use this for initialization
	void Start () {
		Input.simulateMouseWithTouches = true;
		forwardSpeed = 0.0F;
//		controller = GetComponent<CharacterController>();
		gameController = GameObject.FindGameObjectWithTag("GameController");
		if(!PlayerPrefs.HasKey ("reverse")){
			PlayerPrefs.SetInt ("reverse", 1);
		}
		reverseControls = PlayerPrefs.GetInt("reverse");
		slerpTime = 0.0F;
	}
	
	// Update is called once per frame
	void Update () {
		if(GameController.gameIsRunning) {
			totalTime += Time.deltaTime;
			forwardSpeed = startingSpeed + Mathf.Log (1.0F + 0.25F * totalTime)*20.0F;
			int rotateDirection = 0;
			if( Input.touchCount > 0 ){ // Touch control
				Touch lastTouch = Input.touches[Input.touchCount-1];
				Vector2 touchPos = lastTouch.position;
				if(touchPos.x < Screen.width/2) rotateDirection = -1;
				else rotateDirection = 1;

				rotateDirection = rotateDirection * reverseControls; // Reverse controls
				yaw += rotateDirection * speed * Time.deltaTime;
//				transform.RotateAround(Vector3.zero, Vector3.forward, rotateDirection * speed * Time.deltaTime); // Touch Input
			} else {
				// Rotate the sphere around the middle axis with <-/-> and A/D
				int axisDirection = 0;
				if(Input.GetAxis("Horizontal") > 0.1F) axisDirection = 1;
				if(Input.GetAxis("Horizontal") < -0.1F) axisDirection = -1;
				yaw += axisDirection * speed * Time.deltaTime * reverseControls;
//				transform.RotateAround(Vector3.zero, Vector3.forward, Input.GetAxis("Horizontal") * speed * Time.deltaTime); // Arrow keys

			}

			// Get the old position

			transform.position = new Vector3(0.0F, -4.575F, transform.position.z + forwardSpeed * Time.deltaTime);
			transform.localRotation = new Quaternion();
			transform.RotateAround(Vector3.zero, Vector3.forward, yaw); 

			// Slerp the new position
			if(slerpTime > 0){
				Vector3 center = (lastPosition + transform.position) * 0.5F;
				center -= new Vector3(0,0,lastPosition.z);
				Vector3 startRelCenter = lastPosition - center;
				Vector3 endRelCenter = transform.position - center;

				transform.position = Vector3.Slerp (startRelCenter, endRelCenter, (0.03F-slerpTime)/0.03F) ;
				transform.position += center;
				slerpTime -= Time.deltaTime;
				if(slerpTime<0.0F) slerpTime = 0.0F;
			}

			// Check for collisions
			Vector3 direction = transform.position - lastPosition;
			
			RaycastHit hit;
			if(Physics.SphereCast (lastPosition, GetComponent<SphereCollider>().radius, direction, out hit, direction.magnitude ))
			{
//				Debug.Log("Collided"+ "(" + hit.point.x + ", " + hit.point.y + ", " + hit.point.z + ")");
//				transform.position = hit.point;
				GameObject g = hit.collider.gameObject;
//				iTween.PunchScale(hit.collider.gameObject, 0.5F * g.transform.localScale, 1.0F);
				GetComponent<AudioSource>().PlayOneShot(crashSound);
				GameOver();
				gameController.SendMessage ("GameOver");
			}
			
			this.lastPosition = transform.position;
		}

	}

	public void SetYaw(float y){
//		controlByYaw = true;
		slerpTime = 0.03F;
		yaw = y;
	}

	public void setControlByYaw(bool c){
//		controlByYaw = c;
	}

	void Reset(){
		transform.localScale = 2.0F * Vector3.one;
//		Debug.Log ("MoveAround - Reset");
		lastPosition = new Vector3(startX, startY, 10.0F);
		transform.position = new Vector3(startX, startY, 10.0F);
//		iTween.ScaleTo(gameObject, 0.8F*Vector3.one, 1.0F);
		gameObject.transform.localScale = 0.8F*Vector3.one;
		forwardSpeed = startingSpeed;
		totalTime = 0;
		reverseControls = PlayerPrefs.GetInt("reverse");
	}

	void GameOver(){
		forwardSpeed = 0.0F;
		GameController.gameIsRunning = false;
//		iTween.ShakeScale (gameObject, 2*Vector3.one, 0.6F);
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