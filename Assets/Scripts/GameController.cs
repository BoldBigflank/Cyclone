using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SocialPlatforms;

public class GameController : MonoBehaviour {
	// Game Variables
	public Font gameFont;
	public Sprite soundOn;
	public Sprite soundOff;
	public Sprite controlReg;
	public Sprite controlInv;
	public Sprite playTexture;
	public Texture aTexture;
	public Texture bTexture;
	public Texture xTexture;
	public Texture yTexture;

	// UI Stuff
	public Canvas uiCanvas;
	public Image soundsImage;
	public Image controlsImage;
	public Button playButton;
	private EventSystem eventSystem;

	public static bool gameIsRunning;
	public static bool betweenRoundGUI;
	public static bool controllerConnected;
	public static bool headTracking;
	string playerName = "";
	float playerBest = 0.0F;
	bool sound;
	int reverseControls;
	string leaderboardString = "";
	int leaderboardCount;
	bool gameStarted = false;
	public Vector2 scrollPosition = Vector2.zero;

	GameObject player;
	GameObject mainCamera;
//	GameObject playButton;
//	GameObject dicePlusHandler;
	public static float score = 0.0F;
	public AudioClip crashSound;

	// Styles
	public GUIStyle timeStyle;
	public GUIStyle labelStyle;
	public GUIStyle buttonStyle;
	public GUIStyle infoStyle;

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
	public static Color newColor;
	int colorIndex;
	float tColor;
	Light cameraLight;
	Color obstacleColorMix = new Color(0.3F, 0.3F, 0.3F);

	// Game over button hidden
	float playButtonDelay = 1.0F;
	float playButtonTime = 0.0F;
	
	public void StartGame(){
		if(gameIsRunning) return;
		uiCanvas.enabled = false;
		eventSystem.sendNavigationEvents = false;
//		UnityEngine.Apple.TV.Remote.touchesEnabled = true;

		controllerConnected = Input.GetJoystickNames().Length > 0;
		foreach(string s in Input.GetJoystickNames()){
			Debug.Log ("Joystick: " + s);
		}
		player.SendMessage("Reset");
//		player.GetComponent<ParticleSystem>().Clear();
//		player.GetComponent<ParticleSystem>().Play();

//		dicePlusHandler.SetActive(false);

		// Reset if starting a new game
		GameObject[] segmentsToDelete = GameObject.FindGameObjectsWithTag("LevelSegment");
		foreach(GameObject s in segmentsToDelete){
			segments.Remove (s.GetComponent<Rigidbody>());
			foreach(Material m in s.GetComponent<Renderer>().materials)
				DestroyImmediate(m);
			Destroy(s);
		}

		GameObject[] obstaclesToDelete = GameObject.FindGameObjectsWithTag("Obstacle");
		foreach(GameObject o in obstaclesToDelete){
			obstacles.Remove (o.GetComponent<Rigidbody>());
			DestroyImmediate (o.GetComponent<Renderer>().material);
			Destroy (o);
		}

		score = 0.0F;
		drawnLevelPosition = 0.0F;
		drawnPiecePosition = 80.0F;
		SetColor(Color.white);

		// Change button text if starting over
//		playButton.SetActive(false);

		gameIsRunning = true;
		betweenRoundGUI = false;
		gameStarted = true;

	}

	void Awake() {
		Application.targetFrameRate = 60;
	}
	
	// Use this for initialization
	void Start () {
		eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
		uiCanvas.enabled = true;
		eventSystem.sendNavigationEvents = true;
		UnityEngine.Apple.TV.Remote.touchesEnabled = false; // For menu stuff

		gameIsRunning = false;
		betweenRoundGUI = true;
		controllerConnected = Input.GetJoystickNames().Length > 0;
		headTracking = false;
		// Find the player object
		player = GameObject.FindGameObjectWithTag("Player");
		mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
//		playButton = GameObject.FindGameObjectWithTag("PlayButton");
		target = GameObject.FindGameObjectWithTag ("Player").transform;
//		dicePlusHandler = GameObject.FindGameObjectWithTag("DicePlusHandler");
		tColor = 0.0F;

		segments = new List<Rigidbody>();

		cameraLight = mainCamera.GetComponent<Light>();
		colors = new List<Color>();
//		colors.Add(Color.red);
//		colors.Add(Color.yellow);
//		colors.Add(Color.green);
//		colors.Add(Color.cyan);
//		colors.Add(Color.blue);
//		colors.Add(Color.magenta);

		colors.Add(HexToColor("8a188c"));
		colors.Add(HexToColor("920c82"));
		colors.Add(HexToColor("9b0078"));
		colors.Add(HexToColor("b8075e"));
		colors.Add(HexToColor("d50e44"));
		colors.Add(HexToColor("ea6142"));
		colors.Add(HexToColor("ffb440"));
		colors.Add(HexToColor("ffd932"));
		colors.Add(HexToColor("ffff25"));
		colors.Add(HexToColor("92e62d"));
		colors.Add(HexToColor("26ce35"));
		colors.Add(HexToColor("13bd8f"));
		colors.Add(HexToColor("00adea"));
		colors.Add(HexToColor("2787de"));
		colors.Add(HexToColor("4f61d3"));
		colors.Add(HexToColor("6c3caf"));

		
		colorIndex = 0;
		
		// Style stuff
		labelStyle.font = gameFont;
		timeStyle.font = gameFont;
		buttonStyle.font = gameFont;


		labelStyle.fontSize = Mathf.Min( Screen.height, Screen.width)/15;
		timeStyle.fontSize = Mathf.Min( Screen.height, Screen.width)/15;
		buttonStyle.fontSize = Mathf.Min( Screen.height, Screen.width)/15;
		infoStyle.fontSize = Mathf.Min( Screen.height, Screen.width)/25;

		// Player Prefs
		if(!PlayerPrefs.HasKey("name")){
			PlayerPrefs.SetString("name", "Player");
		}
		if(!PlayerPrefs.HasKey("best")){
			PlayerPrefs.SetFloat("best",0.0F);
		} 
		if(!PlayerPrefs.HasKey ("sound")){
			PlayerPrefs.SetInt ("sound", 1);
		}
		if(!PlayerPrefs.HasKey ("reverse")){
			PlayerPrefs.SetInt ("reverse", 1);
		}
		if(!PlayerPrefs.HasKey("sound")){
			PlayerPrefs.SetInt ("sound", 1);
		}
		
		sound = PlayerPrefs.GetInt("sound") == 1;
		AudioListener.volume = (sound) ? 1.0F : 0.0F;

		playerName = PlayerPrefs.GetString("name");
		playerBest = PlayerPrefs.GetFloat ("best");
		reverseControls = PlayerPrefs.GetInt ("reverse");
		GetLeaderboard();

		soundsImage.sprite = (sound) ? soundOn : soundOff;
		controlsImage.sprite = (reverseControls == 1) ? controlReg : controlInv;
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
		// Back button quits
		if (Input.GetKeyDown(KeyCode.Escape)) 
			Application.Quit(); 

		// Updating the color
		if (tColor < 1){ // if end color not reached yet...
			tColor += Time.deltaTime / colorDuration; // advance timer at the right speed
			Color endColor = (gameIsRunning) ? colors[(colorIndex+1)%colors.Count] : Color.black;
			newColor = Color.Lerp(currentColor, endColor, tColor);
			Color obstacleColor = newColor + obstacleColorMix;
			cameraLight.color = newColor;
			foreach (Rigidbody segment in segments){
				segment.GetComponent<Renderer>().material.SetColor ("_Color", newColor);
			}
			foreach (Rigidbody obstacle in obstacles){

				obstacle.GetComponent<Renderer>().material.SetColor ("_Color", obstacleColor);
			}

			// Particle system color
//			player.GetComponent<ParticleSystem>().startColor = newColor;

		}
		if(gameIsRunning){
			score += Time.deltaTime;
			// Create the new cylinders if necessary
			if(target.transform.position.z > drawnLevelPosition - drawDistance){
				
				// Create the cylinder
				Rigidbody instantiatedCylinder = (Rigidbody) Instantiate(cylinder, Vector3.forward * (drawnLevelPosition + 25.0F), Quaternion.Euler(0.0f, 0.0f, 12.0f) );
				instantiatedCylinder.GetComponent<Renderer>().material.mainTexture = textures[ (int)instantiatedCylinder.transform.position.z / distancePerSection % textures.Length ]; // Random.Range(0,textures.Length)
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
					Rigidbody segmentToDestroy = segments[0];
					
					segments.Remove(segmentToDestroy);
					foreach(Material m in segmentToDestroy.gameObject.GetComponent<Renderer>().materials)
						DestroyImmediate (m);
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
						Destroy (obstacle.gameObject.GetComponent<Renderer>().material);
						Destroy (obstacle.gameObject);
					}

				}
			}

			// Arrow button visibility
			GameObject[] controlButtons = GameObject.FindGameObjectsWithTag("ControlButton");
			foreach(GameObject o in controlButtons){
				float visibility = (5.0F - score)/5.0F;
				if (controllerConnected) visibility = 0.0F;
//				obstacle.renderer.material.SetColor ("_Color", newColor);
				Color c = o.GetComponent<Renderer>().material.color;
				c.a = Mathf.Max(visibility, 0.0F);
				o.GetComponent<Renderer>().material.SetColor("_Color", c);
			}

		}

		if(!gameIsRunning && Input.touchCount == 1){ // Scrolling the leaderboard
			if (Input.touches[0].phase == TouchPhase.Moved)
			{
				// dragging
				scrollPosition.y += Input.touches[0].deltaPosition.y * 16.0F;
			}

//			if(Input.GetTouch(0).phase == TouchPhase.Ended) {
//				ray = Camera.main.ScreenPointToRay(Input.touches[0].position);
//				Debug.DrawLine(ray.origin,ray.direction * 10);
//				if (Input.touches[0].phase == TouchPhase.Ended){
//					if(Physics.Raycast (ray, out hit, 10.0F)){
//						if(hit.transform.tag == "PlayButton") StartGame ();
//					}
//				}
//			}
		}

		if(!gameIsRunning && playButtonTime <= 0.0F){
			foreach (KeyCode k in System.Enum.GetValues(typeof(KeyCode)))
			{
				if (Input.GetKeyDown(k)){
					controllerConnected = Input.GetJoystickNames().Length > 0;
					Debug.Log(k.ToString());
				}

			}

//			// Sound
//			if(Input.GetKeyDown(KeyCode.JoystickButton2) || Input.GetKeyDown(KeyCode.JoystickButton18) || Input.GetKeyDown (KeyCode.LeftArrow)){
//				sound = !sound;
//				PlayerPrefs.SetInt("sound", (sound) ? 1:0);
//				AudioListener.volume = (sound) ? 1.0F:0.0F;
//			}
//
//			// Control
//			if(Input.GetKeyDown(KeyCode.JoystickButton3) || Input.GetKeyDown(KeyCode.JoystickButton19) || Input.GetKeyDown (KeyCode.UpArrow)){
//				reverseControls = (reverseControls == 1) ? -1: 1;
//				PlayerPrefs.SetInt("reverse", reverseControls);
//			}
//
//			// Play
//			if(Input.GetKeyDown(KeyCode.JoystickButton0) || Input.GetKeyDown(KeyCode.JoystickButton16) || Input.GetKeyDown(KeyCode.DownArrow)){
//				StartGame ();
//			}

		}



		if(playButtonTime > 0.0F){
			playButtonTime -= Time.deltaTime;
			if(playButtonTime <= 0.0F){
				betweenRoundGUI = true;
//				playButton.SetActive (true);
			}
		}

	}
	
	void GameOver(){
		UnityEngine.Apple.TV.Remote.touchesEnabled = false;
		uiCanvas.enabled = true;
		eventSystem.sendNavigationEvents = true;

		playButton.Select();

		gameIsRunning = false;
		SetColor(Color.white);
//		player.GetComponent<ParticleSystem>().Stop();
		playButtonTime = playButtonDelay;
//		dicePlusHandler.SetActive(true);

		// Post the score to the database
		SendMessage ("ReportScore",score);
//		ReportScore(score.ToString());
		PlayerPrefs.SetString ("name", playerName);

		// Update the best
		if(score > playerBest){
			PlayerPrefs.SetFloat ("best", score);
			playerBest = score;
		}

		PlayerPrefs.Save ();
	}

	private RaycastHit hit;
	private Ray ray;//ray we create when we touch the screen


	void FixedUpdate(){

	}

	public void ToggleSound(){
		sound = !sound;
		PlayerPrefs.SetInt("sound", (sound) ? 1:0);
		AudioListener.volume = (sound) ? 1.0F:0.0F;
		soundsImage.sprite = (sound) ? soundOn : soundOff;
	}

	public void ToggleControls(){
		reverseControls = (reverseControls == 1) ? -1: 1;
		PlayerPrefs.SetInt("reverse", reverseControls);
		controlsImage.sprite = (reverseControls == 1) ? controlReg : controlInv;
	}

	void OnGUI(){
		if(!gameIsRunning && playButtonTime <= 0.0F){  // Between rounds
////			playerName = GUI.TextField (new Rect(Screen.width/4, 0, Screen.width/2, Screen.height * 0.1F), playerName, 16, buttonStyle);
////			if(GUI.changed) {
////				playerName = playerName.ToLower();
////				PlayerPrefs.SetString("name", playerName);
////				ProcessLeaderboard("");
////			}
//
//			Texture soundTexture = (sound) ? soundOn : soundOff;
//			if(GUI.Button(new Rect(Screen.width * 0.30F, Screen.height * 0.05F, Screen.width * 0.10F, Screen.width* 0.10F), soundTexture, buttonStyle)){
//				sound = !sound;
//				PlayerPrefs.SetInt("sound", (sound) ? 1:0);
//				AudioListener.volume = (sound) ? 1.0F:0.0F;
//			}
//
//			Texture controlsTexture = (reverseControls == 1) ? controlReg : controlInv;
//			if(GUI.Button(new Rect(Screen.width * 0.45F, Screen.height * 0.05F, Screen.width * 0.10F, Screen.width* 0.10F), controlsTexture, buttonStyle)){
//				reverseControls = (reverseControls == 1) ? -1: 1;
//				PlayerPrefs.SetInt("reverse", reverseControls);
//			}
//
//			if(GUI.Button(new Rect(Screen.width * 0.4F, Screen.height * 0.65F, Screen.width * 0.20F, Screen.width* 0.10F), playTexture, buttonStyle)){
//				StartGame();
//			}
//
//			GUI.Label (new Rect(Screen.width * 0.2F, Screen.height * 0.25F, Screen.width * 0.3F, Screen.height * 0.2F), "Best\n" + playerBest.ToString("F3") + " s", buttonStyle);
//
//			// If there is a controller, show the buttons
//			if(controllerConnected){
//				// Sound (x)
//				GUI.Label (new Rect(Screen.width * 0.30F, Screen.height * 0.05F, Screen.width * 0.05F, Screen.width* 0.05F), xTexture);
//				// Controls (y)
//				GUI.Label (new Rect(Screen.width * 0.45F, Screen.height * 0.05F, Screen.width * 0.05F, Screen.width* 0.05F), yTexture);
//				// Play (a)
//				GUI.Label (new Rect(Screen.width * 0.4F, Screen.height * 0.65F, Screen.width * 0.05F, Screen.width* 0.05F), aTexture);
//			} else {
//				if(GUI.Button(new Rect(Screen.width * 0.65F, Screen.height * 0.85F, Screen.width * 0.35F, Screen.width* 0.05F), "More Games", buttonStyle)){
//					Application.OpenURL("http://bold-it.com/games-by-alex-swan/");
//				}
//			}
//
//
//
//			// This stuff is only after the first death
//			if(gameStarted == true){
//				GUI.Label (new Rect(Screen.width * 0.5F, Screen.height * 0.25F, Screen.width * 0.3F, Screen.height * 0.2F), "Last\n" + score.ToString("F3") + " s", buttonStyle);
//			}
		} else { // Gameplay
//			GUI.skin.label.alignment = TextAnchor.LowerCenter;
			GUI.Label(new Rect(0, Screen.height*4/5, Screen.width/2, Screen.height/5), "Time: ", labelStyle);
			GUI.Label(new Rect(Screen.width/2, Screen.height*4/5, Screen.width/2, Screen.height/5), score.ToString("F1") + " s", timeStyle);
		}
	}	

	private void GetLeaderboard(){
		string url = "http://intense-lake-5762.herokuapp.com/leaderboard";
//		string url = "http://localhost:8080/leaderboard";
		WWW www = new WWW(url);
		StartCoroutine(GetLeaderboardResponse(www));
	}

	private void ReportScore(float s){
		string url = "http://intense-lake-5762.herokuapp.com/leaderboard";
//		string url = "http://localhost:8080/leaderboard";
		WWWForm form = new WWWForm();

		System.DateTime now = System.DateTime.Now;

		form.AddField ("score",s.ToString());
		form.AddField ("name",playerName);
		form.AddField ("now", now.ToString());
		form.AddField ("key",Md5Sum (playerName + s + now + "want some salt with that hash"));
		WWW www = new WWW(url, form);
		StartCoroutine(ReportResponse(www));
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

	private IEnumerator GetLeaderboardResponse(WWW www){
		yield return www;
		
		if (www.error == null)
		{
			Debug.Log("GET Ok!: " + www.text);
			// Push the leaderboard to the user prefs
			PlayerPrefs.SetString("leaderboard", www.text);
			ProcessLeaderboard(www.text);
		} else {
			Debug.Log("GET Error: "+ www.error);
			if(PlayerPrefs.HasKey("leaderboard")) ProcessLeaderboard("");
		}    
	}

	private void ProcessLeaderboard(string json){
		if (json == "") json = PlayerPrefs.GetString("leaderboard");
		JSONObject j = new JSONObject(json);
		leaderboardString = "<color=red>World Records</color>\n";
		leaderboardCount = 1;
		foreach(JSONObject position in j.list){
			string name = position.GetField("name").ToString().ToLower().Replace("\"","");
			string score =  float.Parse( position.GetField("score").ToString().Replace("\"","")).ToString ("0.00");
			if(name.ToLower () == playerName.ToLower ()) leaderboardString += "<color=lime>";
			leaderboardString += leaderboardCount + " " + score + "\t" + name;
			if(name.ToLower () == playerName.ToLower ()) leaderboardString += "</color>";
			leaderboardString += "\n";
			leaderboardCount++;
		}
	}

	private IEnumerator ReportResponse(WWW www){
		yield return www;

		if (www.error == null)
		{
			Debug.Log("POST Ok!: " + www.text);
			GetLeaderboard();
		} else {
			Debug.Log("POST Error: "+ www.error);
		}    
	}

	Color HexToColor(string hex)
	{
		byte r = byte.Parse(hex.Substring(0,2), System.Globalization.NumberStyles.HexNumber);
		byte g = byte.Parse(hex.Substring(2,2), System.Globalization.NumberStyles.HexNumber);
		byte b = byte.Parse(hex.Substring(4,2), System.Globalization.NumberStyles.HexNumber);
		return new Color32(r,g,b, 255);
	}
}
