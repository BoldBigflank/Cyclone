using UnityEngine;
using System.Collections;

public class RenderQueueSetter : MonoBehaviour {
	
	
	[HideInInspector]
	public int queue = 1000;
	
	void Start () {
		renderer.material.renderQueue = queue;
	}
}
