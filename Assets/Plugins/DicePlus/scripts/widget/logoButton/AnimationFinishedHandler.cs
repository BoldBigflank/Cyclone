using UnityEngine;
using System.Collections;

public class AnimationFinishedHandler : MonoBehaviour {
	
	public void animationFinished(int animationId) {
		DicePlusAnimator.Instance.animationFinished(animationId);
	}

}
