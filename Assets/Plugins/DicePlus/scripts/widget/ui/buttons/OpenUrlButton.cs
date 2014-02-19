using UnityEngine;
using System.Collections;

public class OpenUrlButton : SimpleButton {
	
	[HideInInspector]
	public string url = "http://dicepl.us/";
	
	public override void released ()
	{
		base.released ();
	    Application.OpenURL(url);
		
	}

}
