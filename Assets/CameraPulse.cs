using UnityEngine;
using System.Collections;

public class CameraPulse : MonoBehaviour {
	float[] samples;
	float[] sampleSum;
	int sampleRange = 2940;
	float cameraFOV;
	public float intensity = 15.0F;

	// Use this for initialization
	void Start () {
		samples = new float[ audio.clip.samples*audio.clip.channels];
		audio.clip.GetData(samples, 0);
		sampleSum = new float[samples.Length/sampleRange];
		for (int i = 0; i < samples.Length; i++){
			sampleSum[i/sampleRange] += samples[i];
		}
		Debug.Log (sampleSum[0]);

		cameraFOV = GetComponent<Camera>().fieldOfView;

	}
	
	// Update is called once per frame
	void Update () {
		float currentSample = sampleSum[audio.timeSamples*audio.clip.channels/sampleRange]/sampleRange;
		Camera c = GetComponent<Camera>();
		c.fieldOfView = cameraFOV + intensity * currentSample;
//		Debug.Log();
	}
}
