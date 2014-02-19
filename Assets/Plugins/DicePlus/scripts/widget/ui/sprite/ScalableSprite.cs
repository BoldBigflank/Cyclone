using UnityEngine;
//sprite which scales in x dimention according to provided information in "uvElement" and "elements" arrays
public class ScalableSprite : SimpleSprite
{
	//texture coordinates for elements
	[HideInInspector]
	public float [] uvElements = new float[4] {
		0f, 0.25f, 0.75f, 1f
	};
	//width of elements, positive values are "exact" pixel values, negative values are ratios at which remaining width is divided
	[HideInInspector]
	public float [] elements = new float[3] {
		10, -1, 10
	};
	
	public enum HorizontalAlignment {
		LEFT,
		CENTER,
		RIGHT
	};
	[HideInInspector]
	public HorizontalAlignment horizontalAnchorPoint = HorizontalAlignment.CENTER;
	
	public enum VerticalAlignment {
		TOP,
		CENTER,
		BOTTOM
	};
	[HideInInspector]
	public VerticalAlignment verticalAnchorPoint = VerticalAlignment.CENTER;
	
	public override int getVertexCount() {
		if (uvElements.Length < 2) {
			return 0;
		}
		return (uvElements.Length) * 2;
	}
	
	public override int getTriangleCount() {
		if (uvElements.Length < 2) {
			return 0;
		}
		return (uvElements.Length - 1) * 2;
	}
	
	public override void writeVertices(Vector3 [] vertices, Vector2 [] uvs, Color [] colors, int v) {
		if (uvElements.Length < 2) {
			return;
		}
		Vector3 position;
		Quaternion rotation;
		Vector3 scale;
		Vector3 parentScale;
		
		Rect uv = uvArr[rectIdx];
		
		position = trans.position;
		rotation = trans.rotation;
		scale = trans.localScale;
		scale = new Vector3(Mathf.Abs(scale.x), Mathf.Abs(scale.y), 0);
		parentScale = trans.parent.lossyScale;
		
		
		float [] offsets = new float[uvElements.Length];
		offsets[0] = 0;
		float sum = 0;
		float ratioSum = 0;
		for (int i = 0; i < uvElements.Length - 1; i++) {
			if (elements[i] > 0) {
				sum += elements[i];
			} else {
				ratioSum += elements[i];
			}
		}
		float remaining = Mathf.Max(0, scale.x - sum);
		
		for (int i = 0; i < uvElements.Length-1; i++) {
			if (elements[i] > 0) {
				offsets[i+1] = offsets[i] + elements[i] * parentScale.x;
			} else {
				offsets[i+1] = offsets[i] + remaining*elements[i] * parentScale.x/ratioSum;
			}
		}
		
		Vector2 alignmentOffset = new Vector2();
		if (horizontalAnchorPoint == HorizontalAlignment.LEFT) {
			alignmentOffset.x += scale.x/2;
		} else if (horizontalAnchorPoint == HorizontalAlignment.RIGHT) {
			alignmentOffset.x -= scale.x/2;
		}
		
		if (verticalAnchorPoint == VerticalAlignment.TOP) {
			alignmentOffset.y += scale.y/2;
		} else if (verticalAnchorPoint == VerticalAlignment.BOTTOM) {
			alignmentOffset.y -= scale.y/2;
		}
		
		for (int i = 0; i < uvElements.Length; i++) {
	        vertices[v + i * 2 + 0] = position + rotation * new Vector3(alignmentOffset.x + -scale.x/2 * parentScale.x + offsets[i], alignmentOffset.y + scale.y/2 * parentScale.y, 0);
			vertices[v + i * 2 + 1] = position + rotation * new Vector3(alignmentOffset.x + -scale.x/2 * parentScale.x + offsets[i], alignmentOffset.y + -scale.y/2 * parentScale.y, 0);

			
			uvs[v + i * 2 + 0] = new Vector2(uv.x + uv.width * uvElements[i], uv.y + uv.height);
			uvs[v + i * 2 + 1] = new Vector2(uv.x + uv.width * uvElements[i], uv.y);
			
			colors[v + i * 2 + 0] = color;
        	colors[v + i * 2 + 1] = color;
		}
	}
	
	public override void writeTriangles(int [] triangles, int t, int v) {
		if (uvElements.Length < 2) {
			return;
		}
		for (int i = 0; i < uvElements.Length - 1; i++) {
			triangles[t + i * 6 + 0] = v + i * 2 + 0;
	        triangles[t + i * 6 + 1] = v + i * 2 + 2;
	        triangles[t + i * 6 + 2] = v + i * 2 + 1;
				
	        triangles[t + i * 6 + 3] = v + i * 2 + 1;
	        triangles[t + i * 6 + 4] = v + i * 2 + 2;
	        triangles[t + i * 6 + 5] = v + i * 2 + 3;
		}
	}
}