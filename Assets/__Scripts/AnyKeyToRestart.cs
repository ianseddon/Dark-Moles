using UnityEngine;
using System.Collections;

public class AnyKeyToRestart : MonoBehaviour {

	bool canRestart = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.anyKey && canRestart) {
			Application.LoadLevel(Application.loadedLevel);
		}
	}

	void EnableRestart() {
		canRestart = true;
	}
}
