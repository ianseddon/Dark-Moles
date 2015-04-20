using UnityEngine;
using System.Collections;

public class DeathCounter_Text : MonoBehaviour {

	// Use this for initialization
	void Start () {
		guiText.text = GameObject.Find ("DeathCount").GetComponent<DeathCounter>().deaths.ToString() + " deaths";
	}
	
	// Update is called once per frame
	void Update () {

	}
}
