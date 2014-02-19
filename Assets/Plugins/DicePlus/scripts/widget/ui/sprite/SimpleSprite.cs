using UnityEngine;

public class SimpleSprite : MonoBehaviour
{
	[HideInInspector]
	public string spriteName;
	
	[HideInInspector]
	public Transform trans;
	[HideInInspector]
	public int queue = 1000;
	
	protected int rectIdx = 0;
	[HideInInspector]
	public Rect [] uvArr = new Rect[2];
	[HideInInspector]
	public Rect [] trimRectArr = new Rect[2];
	[HideInInspector]
	public Color color;
	bool render = true;
	
	public bool isRenderingEnabled() {
		return render;
	}
	
	void OnEnable() {
		render = true;
	}
	void OnDisable () {
		render = false;
	}
	
	void Awake() {
		trans = transform;		
	}
	
	void Start() {
		SpriteRenderer.Instance.registerSprite(this);
		rectIdx = SpriteRenderer.isHighRes()?0:1;
	}
	
	void OnDestroy() {
		if (SpriteRenderer.Instance != null) {
			SpriteRenderer.Instance.unregisterSprite(this);
		}
	}
	
	public virtual void writeVertices(Vector3 [] vertices, Vector2 [] uvs, Color [] colors, int i) {
		Vector3 position;
		Quaternion rotation;
		Vector3 scale;
		
		Rect uv = uvArr[rectIdx];
		Rect trimRect = trimRectArr[rectIdx];
		
		position = trans.position;
		rotation = trans.localRotation;

		scale = trans.lossyScale;
		scale = new Vector3(Mathf.Abs(scale.x), Mathf.Abs(scale.y), 0);
		float left = -scale.x/2 + scale.x * trimRect.x;
		float right = -scale.x/2 + scale.x * (trimRect.x + trimRect.width);
		float top = scale.y/2 - scale.y * trimRect.y;
		float down = scale.y/2 - scale.y * (trimRect.y + trimRect.height);
        vertices[i + 0] = position + rotation * new Vector3(left, top, 0);
        vertices[i + 1] = position + rotation * new Vector3(right, top, 0);
		vertices[i + 2] = position + rotation * new Vector3(left, down, 0);
        vertices[i + 3] = position + rotation * new Vector3(right, down, 0);
		
        uvs[i + 0] = new Vector2(uv.x, uv.y + uv.height);
        uvs[i + 1] = new Vector2(uv.x + uv.width, uv.y + uv.height);
		uvs[i + 2] = new Vector2(uv.x, uv.y);
        uvs[i + 3] = new Vector2(uv.x + uv.width, uv.y);
		
        colors[i + 0] = color;
        colors[i + 1] = color;
		colors[i + 2] = color;
        colors[i + 3] = color;
	}
	
	public virtual void writeTriangles(int [] triangles, int t, int v) {
		triangles[t + 0] = v;
        triangles[t + 1] = v + 1;
        triangles[t + 2] = v + 2;

        triangles[t + 3] = v + 2;
        triangles[t + 4] = v + 1;
        triangles[t + 5] = v + 3;
	}
	
	public virtual int getVertexCount() {
		return 4;
	}
	
	public virtual int getTriangleCount() {
		return 2;
	}
}