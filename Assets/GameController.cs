using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {
	// Game Variables
	public Font gameFont;

	bool gameIsRunning;
	string playButtonText = "Play";
	GameObject player;
	GameObject camera;
	float playTime;
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
	Light mainLight;

	void StartGame(){
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
		playTime = 0.0F;
		drawnPosition = 0.0F;

		player.SendMessage("Reset");

		// Change button text if starting over
		playButtonText = "Play Again";

		gameIsRunning = true;
	}

	// Use this for initialization
	void Start () {
		gameIsRunning = false;
		// Find the player object
		player = GameObject.FindGameObjectWithTag("Player");
		camera = GameObject.FindGameObjectWithTag("MainCamera");
		target = GameObject.FindGameObjectWithTag ("Player").transform;
		tColor = 0.0F;

		segments = new List<Rigidbody>();

		GameObject main = GameObject.FindGameObjectWithTag("MainCamera");
		mainLight = main.GetComponent<Light>();
		colors = new List<Color>();
		colors.Add(Color.red);
		colors.Add(Color.yellow);
		colors.Add(Color.green);
		colors.Add(Color.cyan);
		colors.Add(Color.blue);
		colors.Add(Color.magenta);

		colorIndex = 0;

//		segments = new List<Rigidbody>();
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
			mainLight.color = newColor;
			foreach (Rigidbody segment in segments){
				segment.renderer.material.SetColor ("_Color", newColor);
			}
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


	}

	void GameOver(){
		gameIsRunning = false;
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
}
