using UnityEngine;
using System.Collections;

public class ScaleScript : MonoBehaviour {
	Vector3 startScale;
	float timer;
	// Use this for initialization
	void Start () {
		timer = 0.0F;
		startScale = gameObject.transform.localScale;
//		iTween.ScaleBy(gameObject, iTween.Hash("amount",new Vector3(1.1F, 1.0F, 1.1F),"looptype",iTween.LoopType.pingPong,"time",0.6F,"easetype", iTween.EaseType.easeInOutSine));
	}
	
	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;
		if(GameController.gameIsRunning) gameObject.transform.localScale =  startScale * (1.0F + 0.05F * Mathf.Sin(timer*3.0F));
	}
}
