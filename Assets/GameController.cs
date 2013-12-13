using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {
	// Game Variables
	public Font gameFont;

	bool gameIsRunning;
	string playButtonText = "Play";
	GameObject player;
	GameObject mainCamera;
	public static float score = 0.0F;


	// World creation stats
	public Rigidbody cylinder;
	public Texture[] textures;

	int maxSegments = 5;
	List<Rigidbody> segments;
	float drawnPosition = 0.0F;
	float drawDistance = 168.0F;
	Transform target;

	public List<Rigidbody> obstacles;

	// Color stuff
	float colorDuration = 1.0F;
	List<Color> colors;
	int colorIndex;
	float tColor;
	Light cameraLight;

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

		// Reset if starting a new game
		GameObject[] segmentsToDelete = GameObject.FindGameObjectsWithTag("LevelSegment");
		foreach(GameObject s in segmentsToDelete){
			segments.Remove (s.rigidbody);
			Destroy(s);
		}

		GameObject[] obstaclesToDelete = GameObject.FindGameObjectsWithTag("Obstacle");
		foreach(GameObject o in obstaclesToDelete){
			Destroy (o);
		}

		score = 0.0F;
		drawnPosition = 0.0F;


		// Change button text if starting over
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

		// Sphero
		Sphero[] ConnectedSpheros = SpheroProvider.GetSharedProvider().GetConnectedSpheros();
		if(ConnectedSpheros.Length > 0) sphero = ConnectedSpheros[0];
		else sphero = null;
		SpheroDeviceMessenger.SharedInstance.NotificationReceived += ReceiveNotificationMessage;
	}

	void UpdateScoreBy(float i){
		score += i;
	}

	void SetColor(){
		colorIndex++;
		tColor = 0.0F;
	}

	// Update is called once per frame
	void Update () {
		// Updating the color
		if (tColor < 1){ // if end color not reached yet...
			tColor += Time.deltaTime / colorDuration; // advance timer at the right speed
//			light.color = Color.Lerp(selection, endColor, tColor);
			Color newColor = Color.Lerp(colors[colorIndex%colors.Count], colors[(colorIndex+1)%colors.Count], tColor);
			cameraLight.color = newColor;
			foreach (Rigidbody segment in segments){
				segment.renderer.material.SetColor ("_Color", newColor);
			}

			// Sphero light color
			if(sphero != null) sphero.SetRGBLED(newColor.r, newColor.g, newColor.b);  
		}
		if(gameIsRunning){
			score += Time.deltaTime;
			// Create the new cylinders if necessary
			if(target.transform.position.z > drawnPosition - drawDistance){
				
				// Create the cylinder
				Rigidbody instantiatedCylinder = (Rigidbody) Instantiate(cylinder, Vector3.forward * (drawnPosition + 25.0F), transform.rotation);
				instantiatedCylinder.GetComponent<MeshRenderer>().material.mainTexture = textures[Random.Range(0,textures.Length) ];
				segments.Add (instantiatedCylinder);
				drawnPosition += 50.0F;
				
				// Add an obstacle
				int angle = Random.Range (0, 8)  * 45;
				Rigidbody instantiatedObstacle = (Rigidbody) Instantiate(obstacles[0], new Vector3(-5.0F, -5.0F, drawnPosition ), Quaternion.Euler( 0.0F, 0.0F, 0.0F ));
				instantiatedObstacle.transform.RotateAround(Vector3.zero, Vector3.forward, angle);

				// Change the lamp's color
				if(tColor >= 1.0F) SetColor(); // Start the new color
				
				
				
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
			}

		}
	}
	
	void GameOver(){
		gameIsRunning = false;
		SpheroDeviceMessenger.SharedInstance.NotificationReceived -= ReceiveNotificationMessage;
		if(sphero != null) sphero.DisableControllerStreaming();
	}

	void OnGUI(){
		GUI.skin.button.font = gameFont;
		GUI.skin.label.font = gameFont;

		if(!gameIsRunning){
			if (GUI.Button(new Rect(10, 10, 150, 100), playButtonText)){
				StartGame ();
			}
			if (score > 0){
				GUI.skin.label.alignment = TextAnchor.LowerCenter;
				GUI.Label(new Rect(Screen.width/2, Screen.height/2, 300, 100), "Your Score: " + score.ToString ("F2") + " seconds");
			}
				
		} else {
			GUI.skin.label.alignment = TextAnchor.LowerLeft;
			GUI.Label(new Rect(Screen.width/2, Screen.height * 0.9F, 200, 50), "Score: " + score.ToString("F2"));
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
		// to instance variables, which are updated on the screen in the OnGUI method.
		SpheroDeviceSensorsAsyncData message = (SpheroDeviceSensorsAsyncData)eventArgs.Message;
		SpheroDeviceSensorsData sensorsData = message.Frames[0];
		
		acceleration = sensorsData.AccelerometerData.Normalized;
		
		pitch = sensorsData.AttitudeData.Pitch;
		roll = sensorsData.AttitudeData.Roll;
		yaw = sensorsData.AttitudeData.Yaw;
		if(gameIsRunning){
			player.transform.position = new Vector3(0.0F, -4.5F, player.transform.position.z);
			player.transform.RotateAround(Vector3.zero, Vector3.forward, yaw); 
		}

		q0 = sensorsData.QuaternionData.Q0;
		q1 = sensorsData.QuaternionData.Q1;
		q2 = sensorsData.QuaternionData.Q2;
		q3 = sensorsData.QuaternionData.Q3; 
	}
}
