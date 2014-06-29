using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LineScript : MonoBehaviour {
	int maxVertices = 100;
	List<Vector3> lineVertexes;
	Vector3 lastVertex;
	LineRenderer lineRenderer;


	// Use this for initialization
	void Start () {
		lineVertexes = new List<Vector3>();
		lineVertexes.Add(transform.position);
		lastVertex = transform.position;
		lineRenderer = gameObject.GetComponent<LineRenderer>();
	}
	
	// Update is called once per frame
	void LateUpdate () {
		// Nothing to update
		if(transform.position.z == lastVertex.z){
			return;
		}

		// New Game
		if(transform.position.z < lastVertex.z ){
			lineVertexes.Clear();
			lineVertexes.Capacity = 0;
		}

		lineVertexes.Add(transform.position);
		lastVertex = transform.position;

		if(lineVertexes.Count > maxVertices){
			lineVertexes.RemoveRange(0,1);
		}

		// Update the lineRenderer
		lineRenderer.SetVertexCount(lineVertexes.Count);
		lineRenderer.SetColors(Color.black, GameController.newColor);
		for(int i = 0; i < lineVertexes.Count; i++){
			lineRenderer.SetPosition(i, lineVertexes[i]);
		}
	}
}
