using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
public class SpriteRenderer : MonoBehaviour {
	
	public static bool isHighRes() {
		return Mathf.Max(Screen.width, Screen.height) > 1024;//1366;
	}


	private static SpriteRenderer instance;
	public static SpriteRenderer Instance {
		get {
			return instance;
		}
	}
	
	MeshFilter meshFilter;
		
	void Awake () {

		if (instance == null) {
			meshFilter = GetComponent(typeof(MeshFilter)) as MeshFilter;
#if UNITY_3_5
			meshA = new Mesh();
			meshB = new Mesh();
#else
			meshFilter.sharedMesh.MarkDynamic();
#endif
			instance = this;
		} else {
			Destroy(gameObject);
		}
	}
	
	void Update() {
		if (dirty) {
			refreshAtlas();
		}
	}
	
	bool dirty = true;
	
	void refreshAtlas() {
		sprites.Sort(
		    delegate(SimpleSprite p1, SimpleSprite p2)
		    {
		        int compare = p1.queue - p2.queue;
		        if (compare == 0)
		        {
					float v = (p1.trans.position.z - p2.trans.position.z);
		            return v>0?1:(v<0?-1:0);
		        }
		        return compare;
		    }
		);
	}
	
	void OnDestroy() {
		if (this == instance) {
			instance = null;
		}
	}

	List<SimpleSprite> sprites = new List<SimpleSprite>();
	
	public void registerSprite(SimpleSprite sprite) {
		sprites.Add(sprite);
		dirty = true;
	}
	public void unregisterSprite(SimpleSprite sprite) {
		sprites.Remove(sprite);
	}

    #region Internal
	private bool ping = false;
    private Mesh meshA;
	private Mesh meshB;
    private Vector3[] vertices;
    private Color[] colors;

    private Vector2[] uvs;
    #endregion
	
    void LateUpdate() {
		
		int vertexCount = 0;
		int triangleCount = 0;

		foreach (SimpleSprite sprite in sprites) {
			if (sprite.isRenderingEnabled()) {
				vertexCount += sprite.getVertexCount();
				triangleCount += sprite.getTriangleCount();
			}
		}
       
        vertices = new Vector3[vertexCount];
        uvs = new Vector2[vertexCount];
		colors = new Color[vertexCount];
		
		// Generate triangles indices
        int[] triangles = new int[triangleCount * 3];
		
		int v = 0;
		int t = 0;
        // Generate vertex, uv and colors
        foreach (SimpleSprite sprite in sprites) {
			if (!sprite.isRenderingEnabled()) {
				continue;
			}
			
			sprite.writeVertices(vertices, uvs, colors, v);
			sprite.writeTriangles(triangles, t, v);
						
			v += sprite.getVertexCount();
			t += sprite.getTriangleCount() * 3;
        }
		
#if UNITY_3_5
		//hack - to address issue described here
		//http://forum.unity3d.com/threads/118723-Huge-performance-loss-in-Mesh-CreateVBO-for-dynamic-meshes-IOS
		meshFilter.sharedMesh = ping?meshA:meshB;
		ping = !ping;
#else
		meshFilter.sharedMesh.MarkDynamic();
#endif
		meshFilter.sharedMesh.Clear();
        // Assign to mesh	
        meshFilter.sharedMesh.vertices = vertices;
		meshFilter.sharedMesh.colors = colors;

        meshFilter.sharedMesh.uv = uvs;
        meshFilter.sharedMesh.triangles = triangles;
    }
}


