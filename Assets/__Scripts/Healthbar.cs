using UnityEngine;
using System.Collections;

public class Healthbar : MonoBehaviour {

	public float pct = 100.0f;
	public float maxWidth;
	public Healthbar dmgBar;
	public float damageDelay = 1.0f;
	// Use this for initialization
	void Awake () {
		pct = 100.0f;
	}

	// Update is called once per frame
	void Update () {

		// make sure health bar doesnt go below zero
		if (pct < 0)
			pct = 0;

		Rect newInset = guiTexture.pixelInset;
		newInset.width = maxWidth * (pct / 100.0f);
		guiTexture.pixelInset = newInset;
	}

	void UpdateBar(float newPct) {
		pct = newPct;
		if (dmgBar != null)
			StartCoroutine(showDamageDelay (damageDelay));
	}

	IEnumerator showDamageDelay(float s) {
		yield return new WaitForSeconds(s);

		dmgBar.SendMessage("UpdateBar", pct);
	}
}
