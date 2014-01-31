using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SocialPlatforms;

public class GameController : MonoBehaviour {
	// Game Variables
	public Font gameFont;

	bool gameIsRunning;
	string playButtonText = "Play";
	string playerName = "";
	GameObject player;
	GameObject mainCamera;
	GameObject playButton;
	public static float score = 0.0F;
	public AudioClip crashSound;

	// Styles
	public GUIStyle timeStyle;
	public GUIStyle labelStyle;
	public GUIStyle buttonStyle;

	// World creation stats
	public TextAsset levelsAsset;
	public Rigidbody cylinder;
	public Texture[] textures;
	int distancePerSection = 425;

	int maxSegments = 7;
	List<Rigidbody> segments;
	float drawnLevelPosition = 0.0F;
	float drawnPiecePosition = 0.0F;
	float drawDistance = 218.0F;
	Transform target;

	public Rigidbody obstacle;
	public Rigidbody rotatingObstacle;

	public List<Rigidbody> obstacles;

	// Color stuff
	float colorDuration = 1.0F;
	List<Color> colors;
	Color currentColor;
	int colorIndex;
	float tColor;
	Light cameraLight;

	// Game over button hidden
	float playButtonDelay = 1.0F;
	float playButtonTime = 0.0F;

	// Sphero
	bool streaming = false;

	private SpheroAccelerometerData.Acceleration acceleration = new SpheroAccelerometerData.Acceleration();

	private float pitch = 0.0f;
	private float roll = 0.0f;
	private float yaw = 0.0f;

	private float q0 = 1.0f;
	private float q1 = 1.0f;
	private float q2 = 1.0f;
	private float q3 = 1.0f;

	Sphero sphero;
	SpheroDeviceNotification Message;

	void StartGame(){
		player.SendMessage("Reset");
		player.GetComponent<ParticleSystem>().Clear();
		player.GetComponent<ParticleSystem>().Play();

		// Reset if starting a new game
		GameObject[] segmentsToDelete = GameObject.FindGameObjectsWithTag("LevelSegment");
		foreach(GameObject s in segmentsToDelete){
			segments.Remove (s.rigidbody);
			Destroy(s);
		}

		GameObject[] obstaclesToDelete = GameObject.FindGameObjectsWithTag("Obstacle");
		foreach(GameObject o in obstaclesToDelete){
			obstacles.Remove (o.rigidbody);
			Destroy (o);
		}

		score = 0.0F;
		drawnLevelPosition = 0.0F;
		drawnPiecePosition = 80.0F;
		SetColor(Color.white);

		// Change button text if starting over
		playButton.SetActive(false);
		playButtonText = "Play Again";

		gameIsRunning = true;
		if(sphero != null){
			sphero.EnableControllerStreaming(60, 1,
				SpheroDataStreamingMask.AccelerometerFilteredAll |
				SpheroDataStreamingMask.QuaternionAll |
				SpheroDataStreamingMask.IMUAnglesFilteredAll);
		}
	}

	void Awake() {
		Application.targetFrameRate = 60;
	}
	
	// Use this for initialization
	void Start () {

		gameIsRunning = false;
		// Find the player object
		player = GameObject.FindGameObjectWithTag("Player");
		mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
		playButton = GameObject.FindGameObjectWithTag("PlayButton");
		target = GameObject.FindGameObjectWithTag ("Player").transform;
		tColor = 0.0F;

		segments = new List<Rigidbody>();

		cameraLight = mainCamera.GetComponent<Light>();
		colors = new List<Color>();
		colors.Add(Color.red);
		colors.Add(Color.yellow);
		colors.Add(Color.green);
		colors.Add(Color.cyan);
		colors.Add(Color.blue);
		colors.Add(Color.magenta);

		colorIndex = 0;

		// Style stuff
		labelStyle.font = gameFont;
		timeStyle.font = gameFont;
		buttonStyle.font = gameFont;

		labelStyle.fontSize = Screen.height/15;
		timeStyle.fontSize = Screen.height/15;
		buttonStyle.fontSize = Screen.height/15;


		// Sphero
		Sphero[] ConnectedSpheros = SpheroProvider.GetSharedProvider().GetConnectedSpheros();
		if(ConnectedSpheros.Length > 0) sphero = ConnectedSpheros[0];
		else sphero = null;
		SpheroDeviceMessenger.SharedInstance.NotificationReceived += ReceiveNotificationMessage;

		// Player Prefs
		if(!PlayerPrefs.HasKey("name")){
			PlayerPrefs.SetString("name", "Player");
		}
		if(!PlayerPrefs.HasKey ("sound")){
			PlayerPrefs.SetInt ("sound", 1);
		}

		playerName = PlayerPrefs.GetString("name");

	}


	void UpdateScoreBy(float i){
		score += i;
	}

	void SetColor(){
		colorIndex++;
		SetColor (colors[colorIndex % colors.Count]);
	}

	void SetColor(Color c){
		currentColor = c;
		tColor = 0.0F;
	}

	// Update is called once per frame
	void Update () {
		// Updating the color
		if (tColor < 1){ // if end color not reached yet...
			tColor += Time.deltaTime / colorDuration; // advance timer at the right speed
			Color endColor = (gameIsRunning) ? colors[(colorIndex+1)%colors.Count] : Color.black;
			Color newColor = Color.Lerp(currentColor, endColor, tColor);
			cameraLight.color = newColor;
			foreach (Rigidbody segment in segments){
				segment.renderer.material.SetColor ("_Color", newColor);
			}
			foreach (Rigidbody obstacle in obstacles){
				obstacle.renderer.material.SetColor ("_Color", newColor);
			}

			// Particle system color
			player.GetComponent<ParticleSystem>().startColor = newColor;

			// Sphero light color
			if(sphero != null) sphero.SetRGBLED(newColor.r, newColor.g, newColor.b);  
		}
		if(gameIsRunning){
			score += Time.deltaTime;
			// Create the new cylinders if necessary
			if(target.transform.position.z > drawnLevelPosition - drawDistance){
				
				// Create the cylinder
				Rigidbody instantiatedCylinder = (Rigidbody) Instantiate(cylinder, Vector3.forward * (drawnLevelPosition + 25.0F), Quaternion.Euler(0.0f, 0.0f, 12.0f) );
				instantiatedCylinder.renderer.material.mainTexture = textures[ (int)instantiatedCylinder.transform.position.z / distancePerSection % textures.Length ]; // Random.Range(0,textures.Length)
				segments.Add (instantiatedCylinder);
				drawnLevelPosition += 50.0F;

				// Change the lamp's color
				if(tColor >= 1.0F) SetColor(); // Start the new color
				
				
				
			}

			if(target.transform.position.z > drawnPiecePosition - drawDistance) {
				float basePosition = drawnPiecePosition;
				// Add a pattern
				int baseAngle = Random.Range (0, 8)  * 45;
				int flip = (Random.Range(0, 2) == 1) ? 1: -1;
				JSONObject j = new JSONObject(levelsAsset.ToString());
				// Pick a pattern
				JSONObject pattern = j.list[Random.Range(0, j.list.Count)];

				// Decide whether the pattern is going to be rotating
				int rotating = Random.Range (0,2);
				int direction = Random.Range(0,2); // 

				foreach(JSONObject piece in pattern.GetField("pieces").list){
					int angle = flip * (int)piece.GetField("angle").n;
					int depth = (int)piece.GetField ("depth").n;

					Rigidbody instantiatedObstacle;
					if(rotating == 1){
						instantiatedObstacle = (Rigidbody) Instantiate(rotatingObstacle, new Vector3(0.0F, 0.0F, basePosition + depth ), Quaternion.Euler( 90.0F, 0.0F, 0.0F ));
						if(direction == 0) instantiatedObstacle.GetComponent<RotatingObstacle>().direction = -1;
					} else {
						instantiatedObstacle = (Rigidbody) Instantiate(obstacle, new Vector3(0.0F, 0.0F, basePosition + depth ), Quaternion.Euler( 90.0F, 0.0F, 0.0F ));
					}
					instantiatedObstacle.transform.RotateAround(Vector3.zero, Vector3.forward, baseAngle + angle);
					obstacles.Add(instantiatedObstacle);
				}

				drawnPiecePosition += pattern.GetField("depth").n;
			}
			
			// Remove segments we've passed
			
			if( segments.Count > maxSegments ){
				
				//			GameObject[] LevelSegments = GameObject.FindGameObjectsWithTag("LevelSegment");
				if(segments.Count > 0 ){
					Debug.Log ("Time to delete", segments[0]);
					Rigidbody segmentToDestroy = segments[0];
					
					segments.Remove(segmentToDestroy);
					
					Destroy ( segmentToDestroy.gameObject);
					
				}

				if(obstacles.Count > 0 ){

					List<Rigidbody> obstaclesToRemove = new List<Rigidbody>();
					foreach(Rigidbody obstacle in obstacles){
						if(obstacle.gameObject.transform.position.z < mainCamera.transform.position.z) { 
							obstaclesToRemove.Add (obstacle);
						}
					}
					foreach (Rigidbody obstacle in obstaclesToRemove){
						obstacles.Remove(obstacle);
						Destroy (obstacle.gameObject);
					}

				}
			}
		}

		if(playButtonTime > 0.0F){
			playButtonTime -= Time.deltaTime;
			if(playButtonTime <= 0.0F){
				playButton.SetActive (true);
			}
		}

		// Sphero streaming
		if (!streaming && 
		    SpheroProvider.GetSharedProvider().GetConnectedSpheros().Length  > 0) 
		{
			// Setup streaming for the first time once a Sphero is connected.
			//// Register the event handler call back with the SpheroDeviceMessenger
			SpheroDeviceMessenger.SharedInstance.AsyncDataReceived += ReceiveAsyncMessage;        
			//// Get the currently connected Sphero
			Sphero[] spheros =
				SpheroProvider.GetSharedProvider().GetConnectedSpheros();
			sphero = spheros[0];
			//// Enable data streaming for controller app. This method turns off stabilization (disables the wheel motors), 
			//// turn on the back LED (negative x axis reference), and sets data streaming at 20 samples/sec (400/20), 
			//// a single sample per packet sent, and turns on accelerometer, quaternion, and IMU (attitude) sampling.
			sphero.EnableControllerStreaming(60, 1,
                   SpheroDataStreamingMask.AccelerometerFilteredAll |
                   SpheroDataStreamingMask.QuaternionAll |
                   SpheroDataStreamingMask.IMUAnglesFilteredAll);
			
			streaming = true;
		}  
		if(Message != null){
			
			Sphero notifiedSphero = SpheroProvider.GetSharedProvider().GetSphero(Message.RobotID);
			if( Message.NotificationType == SpheroDeviceNotification.SpheroNotificationType.DISCONNECTED ) {
				notifiedSphero.ConnectionState = Sphero.Connection_State.Disconnected;
				streaming = false;
				Application.LoadLevel("NoSpheroConnectedScene");
//				player.GetComponent<MoveAround>().setControlByYaw(false);
			}

		}
	}
	
	void GameOver(){
		gameIsRunning = false;
		SpheroDeviceMessenger.SharedInstance.NotificationReceived -= ReceiveNotificationMessage;
		if(sphero != null) sphero.DisableControllerStreaming();
		SetColor(Color.white);
		player.GetComponent<ParticleSystem>().Stop();
		playButtonTime = playButtonDelay;

		// Post the score to the database
		ReportScore(score.ToString());
		PlayerPrefs.SetString ("name", playerName);
		PlayerPrefs.Save ();

	}

	private RaycastHit hit;
	private Ray ray;//ray we create when we touch the screen

	void FixedUpdate(){
		if(Input.touchCount == 1) {
			ray = Camera.main.ScreenPointToRay(Input.touches[0].position);
			Debug.DrawLine(ray.origin,ray.direction * 10);
			if (Input.touches[0].phase == TouchPhase.Ended){
				if(Physics.Raycast (ray, out hit, 10.0F)){
					Debug.Log(hit.transform.name);//Object you touched
					if(hit.transform.tag == "PlayButton") StartGame ();
				}
			}
		}
	}

	void OnGUI(){

		if(!gameIsRunning && playButtonTime <= 0.0F){
			playerName = GUI.TextField (new Rect(Screen.width/4, 0, Screen.width/2, Screen.height * 0.1F), playerName, 16, buttonStyle);
//			if (GUI.Button(new Rect(Screen.width/4, 0, Screen.width/2, Screen.height/5), playerName, buttonStyle)){
//
//			}
			if (score > 0){
				GUI.Label(new Rect(0, Screen.height*4/5, Screen.width/2, Screen.height/5), "Time: ", labelStyle);
				GUI.Label(new Rect(Screen.width/2, Screen.height*4/5, Screen.width/2, Screen.height/5), score.ToString("F1") + " s", timeStyle);
			}
				
		} else {
//			GUI.skin.label.alignment = TextAnchor.LowerCenter;
			GUI.Label(new Rect(0, Screen.height*4/5, Screen.width/2, Screen.height/5), "Time: ", labelStyle);
			GUI.Label(new Rect(Screen.width/2, Screen.height*4/5, Screen.width/2, Screen.height/5), score.ToString("F1") + " s", timeStyle);
		}
	}

	void OnApplicationPause(bool pause) {
		if (pause) {
			// Unregister event handlers when the applications pauses.
			if (streaming) {
				// removes data streaming event handler
				SpheroDeviceMessenger.SharedInstance.AsyncDataReceived -= ReceiveAsyncMessage;        
				// Turns off controller mode data streaming. Stabilization is restored and the back LED is turn off.
				sphero.DisableControllerStreaming();
				streaming = false;
			} 
			// Stop listening for disconnnect notifications.
			SpheroDeviceMessenger.SharedInstance.NotificationReceived -= 
				ReceiveNotificationMessage;
			// Disconnect from the Sphero
			SpheroProvider.GetSharedProvider().DisconnectSpheros();
		}else {
			SpheroDeviceMessenger.SharedInstance.NotificationReceived += ReceiveNotificationMessage;
			streaming = false;
		}
	}
	
	/*
         * Callback to receive connection notifications 
         */
	private void ReceiveNotificationMessage(object sender, SpheroDeviceMessenger.MessengerEventArgs eventArgs)
	{
		// Event handler that listens for disconnects. An example of when one would be received is when Sphero 
		// goes to sleep.
		Message = (SpheroDeviceNotification)eventArgs.Message;

	}

	private void ReceiveAsyncMessage(object sender, SpheroDeviceMessenger.MessengerEventArgs eventArgs)
	{
		// Handler method for the streaming data. This code copies the data values
		// to instance variables, which are updated on the screen in the On  method.
		SpheroDeviceSensorsAsyncData message = (SpheroDeviceSensorsAsyncData)eventArgs.Message;
		SpheroDeviceSensorsData sensorsData = message.Frames[0];
		
		acceleration = sensorsData.AccelerometerData.Normalized;
		
		pitch = sensorsData.AttitudeData.Pitch;
		roll = sensorsData.AttitudeData.Roll;
		yaw = sensorsData.AttitudeData.Yaw;
		if(gameIsRunning){
			 player.GetComponent<MoveAround>().SetYaw(yaw);

//			player.transform.position = new Vector3(0.0F, -4.5F, player.transform.position.z);
//			player.transform.RotateAround(Vector3.zero, Vector3.forward, yaw); 
		}

		q0 = sensorsData.QuaternionData.Q0;
		q1 = sensorsData.QuaternionData.Q1;
		q2 = sensorsData.QuaternionData.Q2;
		q3 = sensorsData.QuaternionData.Q3; 
	}

	private void ReportScore(string s){
		string url = "http://intense-lake-5762.herokuapp.com/leaderboard";
		WWWForm form = new WWWForm();
		string name = PlayerPrefs.GetString("name");
		System.DateTime now = System.DateTime.Now;

		form.AddField ("score",s);
		form.AddField ("name",name);
		form.AddField ("now", now.ToString());
		form.AddField ("key",Md5Sum (name + score + now + "want some salt with that hash"));
		WWW www = new WWW(url, form);
		StartCoroutine(WaitForRequest(www));
	}

	public  string Md5Sum(string strToEncrypt)
	{
		System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
		byte[] bytes = ue.GetBytes(strToEncrypt);
		
		// encrypt bytes
		System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
		byte[] hashBytes = md5.ComputeHash(bytes);
		
		// Convert the encrypted bytes back to a string (base 16)
		string hashString = "";
		
		for (int i = 0; i < hashBytes.Length; i++)
		{
			hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
		}
		
		return hashString.PadLeft(32, '0');
	}
	
	private IEnumerator WaitForRequest(WWW www){
		yield return www;

		if (www.error == null)
		{
			Debug.Log("WWW Ok!: " + www.data);
		} else {
			Debug.Log("WWW Error: "+ www.error);
		}    
	}
}
