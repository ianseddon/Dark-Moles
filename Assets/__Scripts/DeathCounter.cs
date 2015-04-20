using UnityEngine;
using System.Collections;

public class DeathCounter : MonoBehaviour {

	public int deaths = 0;

	// Use this for initialization
	void Start () {
		Object.DontDestroyOnLoad(this);
	}

	public void AddDeath() {
		deaths++;
	}

}
