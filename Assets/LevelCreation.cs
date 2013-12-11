using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelCreation : MonoBehaviour {
	// World creation stats
	public Rigidbody cylinder;
	public Texture[] textures;

	public Rigidbody[] obstacles;

	int maxSegments = 5;
	List<Rigidbody> segments;
	float drawnPosition = 0.0F;
	float drawDistance = 168.0F;
	Transform target;

	// Color stuff
	public float colorDuration = 1.0F;
	public List<Color> colors;
	public int colorIndex;
	float tColor;
	Light mainLight;

	// Use this for initialization
	void Start () {
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

		// Create the new cylinders if necessary
		if(target.transform.position.z > drawnPosition - drawDistance){

			// Create the cylinder
			Rigidbody instantiatedCylinder = (Rigidbody) Instantiate(cylinder, Vector3.forward * (drawnPosition + 25.0F), transform.rotation);
			instantiatedCylinder.GetComponent<MeshRenderer>().material.mainTexture = textures[Random.Range(0,textures.Length) ];
			segments.Add (instantiatedCylinder);
			drawnPosition += 50.0F;

			if((int)drawnPosition > 50){ // The first one triggers the collision at launch
				// Add an obstacle
				int angle = Random.Range (0, 8)  * 45;
				Rigidbody instantiatedObstacle = (Rigidbody) Instantiate(obstacles[0], new Vector3(-5.0F, -5.0F, drawnPosition ), Quaternion.Euler( 0.0F, 0.0F, 0.0F ));
				instantiatedObstacle.transform.RotateAround(Vector3.zero, Vector3.forward, angle);
			}

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

//			foreach(GameObject segment in LevelSegments){
//				Debug.Log ("There's a segment" + segment.transform);
//				if(segment.transform.position.z < target.position.z)
//					Destroy ( segment );
//			}
			

//			for (var segment : GameObject in segments) {
//
//			}

//			Rigidbody segmentToDelete = segments[0];
//			segments.RemoveAt(0);
//			Destroy(segments[0]);


		}

	}
}
