using UnityEngine;
using System.Collections;

public class ResolutionTextureSwitcher : MonoBehaviour {
	
	[HideInInspector]
	public Texture2D lowResolution;
	[HideInInspector]
	public Texture2D hiResolution;
	
	void Start () {
		renderer.material.mainTexture = SpriteRenderer.isHighRes()?hiResolution:lowResolution;
	}
	
}
