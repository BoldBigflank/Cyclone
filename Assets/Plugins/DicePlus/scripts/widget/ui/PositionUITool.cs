using UnityEngine;
using System.Collections;

public class PositionUITool : MonoBehaviour {

	public enum HorizontalAligment {
		LEFT,
		CENTER,
		RIGHT
	}
	
	public enum VerticalAligment {
		TOP,
		CENTER,
		BOTTOM
	}
	
	/// <summary>
	/// Horizontal position of anchor point
	/// </summary>
	public HorizontalAligment hori = HorizontalAligment.RIGHT;
	/// <summary>
	/// Vertical position of anchor point
	/// </summary>
	public VerticalAligment vert = VerticalAligment.BOTTOM;
	[HideInInspector]
	public Camera cam;
	/// <summary>
	/// The distance from anchor point
	/// </summary>
	public Vector2 distance;
	/// <summary>
	/// Is widget dragable
	/// </summary>
	public bool dragable = true;
	
	public void setStartPosition() {
		Vector2 viewport = new Vector2(cam.orthographicSize * cam.aspect , cam.orthographicSize);

		Vector3 newpos = cam.transform.localPosition;
		newpos.z = 0;
		if (hori == HorizontalAligment.LEFT) {
			newpos.x += -1f*viewport.x;
		} else if (hori == HorizontalAligment.RIGHT) {
			newpos.x += 1f*viewport.x;
		}
		
		if (vert == VerticalAligment.TOP) {
			newpos.y += 1f*viewport.y;
		} else if (vert == VerticalAligment.BOTTOM) {
			newpos.y += -1f*viewport.y;
		}

		newpos.x += distance.x;
		newpos.y += distance.y;

		setSnapedPosition(newpos);
	}
	
	[HideInInspector]
	public float radius = 90;
	
	public void setSnapedPosition(Vector2 newPosition) {
		Vector2 viewport = new Vector2(cam.orthographicSize * cam.aspect , cam.orthographicSize);
		newPosition.x = Mathf.Max(Mathf.Min(viewport.x - radius, newPosition.x), -viewport.x + radius);
		newPosition.y = Mathf.Max(Mathf.Min(viewport.y - radius, newPosition.y), -viewport.y + radius);
		newPosition.x = (int) newPosition.x;
		newPosition.y = (int) newPosition.y;
		transform.localPosition = newPosition;
		DicePlusAnimator.Instance.updateLabel();
	}
	
	float prev;

	
	void Start() {
        prev = cam.aspect;
		setStartPosition();

    }
	
    void Update() {
        if (cam.aspect != prev) {
			prev = cam.aspect;
			setStartPosition();
		}
	}
	
}
