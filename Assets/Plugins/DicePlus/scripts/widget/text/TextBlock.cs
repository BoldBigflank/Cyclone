using UnityEngine;
using System.Collections;

public class TextBlock : MonoBehaviour {
	
	public enum HorizontalAlignment {
		LEFT,
		CENTER,
		RIGHT
	};
	
	[HideInInspector]
	public HorizontalAlignment horizontalAnchorPoint = HorizontalAlignment.CENTER;
	
	[HideInInspector]
	public float lineLength = 20;
	[HideInInspector]
	public Translations.Key key = Translations.Key.BUY; 
	[HideInInspector]
	public Transform background;
	[HideInInspector]
	public bool upperCase = false;
	// Use this for initialization
	void Start () {
		setKey(key);
	}
	
	public void setKey(Translations.Key newKey) {
		key = newKey;
		string txt = Translations.translateKey(key);
		if (upperCase) {
			txt = txt.ToUpper();
		}
		string text = Translations.wrapText(txt, lineLength);
		float bgscalex = (Translations.estimateLength(text) + 3) * (GetComponent<TextMesh>().characterSize + 0.5f);
		GetComponent<TextMesh>().text = text;
		if (background != null) {
			background.localScale = new Vector3(bgscalex, background.localScale.y, background.localScale.z);
		}
		
		Vector3 offset = new Vector3();
		
		if (horizontalAnchorPoint == HorizontalAlignment.LEFT) {
			offset.x = -bgscalex/2;
		} else if (horizontalAnchorPoint == HorizontalAlignment.RIGHT) {
			offset.x = bgscalex/2;
		}
		
		transform.position += offset;
	}
}
