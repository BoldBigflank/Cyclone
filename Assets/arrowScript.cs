using UnityEngine;
using System.Collections;

public class arrowScript : MonoBehaviour {
	Vector3 startScale;
	// Use this for initialization
	void Start () {
		startScale = transform.localScale;

		iTween.PunchScale(gameObject, iTween.Hash( "amount", 2.0F * startScale, "looptype",iTween.LoopType.loop));

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
