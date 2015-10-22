using UnityEngine;
using System.Collections;

public class MemoryScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnGUI() {
        GUILayout.Label("All " + Resources.FindObjectsOfTypeAll<Object>().Length);
        GUILayout.Label("Textures " + Resources.FindObjectsOfTypeAll<Texture>().Length);
        GUILayout.Label("AudioClips " + Resources.FindObjectsOfTypeAll<AudioClip>().Length);
        GUILayout.Label("Meshes " + Resources.FindObjectsOfTypeAll<Mesh>().Length);
        GUILayout.Label("Materials " + Resources.FindObjectsOfTypeAll<Material>().Length);
        GUILayout.Label("GameObjects " + Resources.FindObjectsOfTypeAll<GameObject>().Length);
        GUILayout.Label("Components " + Resources.FindObjectsOfTypeAll<Component>().Length);
    }
}
