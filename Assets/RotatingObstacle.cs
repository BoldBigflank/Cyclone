using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RotatingObstacle : MonoBehaviour {
	public float speed = 30.0F;
	public int direction = 1;
	int points = 60;
	List<Vector3> linePoints;
	Vector3 point;
	Vector3 pivot;
	Quaternion angle;
	GameObject lineStart;
	Color startColor;
	Color endColor;
	LineRenderer l;

	// Use this for initialization
	void Start () {
		linePoints = new List<Vector3>();
		l = gameObject.GetComponent<LineRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		transform.RotateAround(Vector3.zero, Vector3.forward, direction * speed * Time.deltaTime);
	}

	void FixedUpdate(){
		point = transform.GetChild (0).transform.position;
		pivot = Vector3.forward * point.z;
		angle = Quaternion.Euler(new Vector3(0.0F, 0.0F, -21.0F * direction));

		linePoints.Add (angle * ( point - pivot) + pivot);
		while(linePoints.Count > points)
			linePoints.RemoveAt(0);
//		l.SetVertexCount(linePoints.Count);
//		for(int i=0; i<linePoints.Count; i++){
//			l.SetPosition(i, linePoints[i]);
//		}
		l.SetPosition (0,linePoints[0]);
		l.SetPosition (1,linePoints[linePoints.Count/2]);
		l.SetPosition(2,linePoints[linePoints.Count-1]);

		startColor = GameController.newColor;
		startColor.a = 0.0F;
		endColor = GameController.newColor;
		endColor.a = 0.9F;

		l.SetColors(startColor, endColor);
		
	}
}
