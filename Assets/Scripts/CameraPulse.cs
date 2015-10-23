using UnityEngine;
using System.Collections;

public class CameraPulse : MonoBehaviour {
	private float[] samples = new float[128];
	private float[] curValues = new float[8];

	public float intensity = 10.0F;
	public float damping = 6.0F;

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
		cameraFOV = baseFOV;
	}
	
	// Update is called once per frame
	void Update () {
		
		int count = 0;
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
			
			curValues[i] = average;
			ss += curValues[i].ToString("0.000")+",";
		}

//		float newFOV = baseFOV - intensity * curValues[0];
		float shine = Mathf.Max(0.0F, curValues[4] - 0.3F);
		Color newColor = new Color(curValues[4], curValues[4], curValues[4]);
		if(newColor.r < GameController.musicColorMix.r)
			GameController.musicColorMix = Color.Lerp(GameController.musicColorMix, newColor, Time.deltaTime* damping);
		else
			GameController.musicColorMix = newColor;
	}

//	// Equalizer visualization
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
