using UnityEngine;
using System.Collections;

public class CameraPulse : MonoBehaviour {
	private float[] samples = new float[128];
	private float[] curValues = new float[8];

	public float intensity = 15.0F;
	public float damping = 2.0F;

	public Texture2D SampleImg;

	float baseFOV;
	float cameraFOV;
	float newFOV;
	string ss = "";

	// Use this for initialization
	void Start () {
//		samples = new float[ audio.clip.samples*audio.clip.channels];
//		audio.clip.GetData(samples, 0);
//		sampleSum = new float[samples.Length/sampleRange];
//		for (int i = 0; i < samples.Length; i++){
//			sampleSum[i/sampleRange] += samples[i];
//		}
//		Debug.Log (sampleSum[0]);
//
		baseFOV = GetComponent<Camera>().fieldOfView;

	}
	
	// Update is called once per frame
	void Update () {
		
		int count = 0;
		float diff = 0;
		AudioListener.GetSpectrumData(samples, 0, FFTWindow.Hamming);

		ss = "";
		for (int i = 0; i < 6; ++i)
		{
			
			float average = 0;
			
			int sampleCount = (int)Mathf.Pow(2, i) * 2;
			
			for (int j = 0; j < sampleCount; ++j)
			{
				
				average += samples[count] * (count + 1);
				++count;
			}
			
			average /= count;
			
			diff = Mathf.Clamp(average * 10 - curValues[i], 0, 4);
			
			curValues[i] = average;
			ss += curValues[i].ToString("0.000")+",";
		}

		float newFOV = baseFOV - intensity * curValues[1];
		if(newFOV > cameraFOV)
			cameraFOV = Mathf.Lerp(cameraFOV, newFOV, Time.deltaTime* damping);
		else
			cameraFOV = newFOV;
		GetComponent<Camera>().fieldOfView = cameraFOV;
	}

	// Equalizer visualization
//	void OnGUI()
//	{
//		
//		GUI.Label(new Rect(150, 600, 600, 500), ss);
//		
//		int ImageHeight = 600;
//		
//		for (int i = 0; i < 8; i++ )
//		{
//			float Height = ImageHeight * (1 - curValues[i]);
//			GUI.DrawTexture(new Rect(i * 50, 100 + Height, 45, ImageHeight - Height), SampleImg); 
//		}
//
//
//	}
}
