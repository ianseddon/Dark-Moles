using UnityEngine;
using System.Collections;

public class Arrow : MonoBehaviour {

	public string key;

	private float distance;

	private Transform line;

	// Use this for initialization
	void Start () {
		line = GameObject.Find ("DanceLine").transform;
	}
	
	// Update is called once per frame
	void Update () {
		distance = Mathf.Abs (line.position.y - transform.position.y);

		if (Input.GetKey (key)) {
			if(distance < 0.5f) Destroy(gameObject);
			else Debug.Log("oops");
		}
	}
}
