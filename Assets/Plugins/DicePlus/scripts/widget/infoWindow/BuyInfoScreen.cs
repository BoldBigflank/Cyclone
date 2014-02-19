using UnityEngine;
using System.Collections;

public class BuyInfoScreen : InfoScreen {
	
	[HideInInspector]
	public TextBlock textBlock;
	
	public override void screenToggled() {
		if (DicePlusConnectionManager.Instance.getConnectedDice().Count > 0) {
			textBlock.setKey(Translations.Key.BUY_1_TEXT);
		} else {
			textBlock.setKey(Translations.Key.BUY_2_TEXT);
		}
	}
}
