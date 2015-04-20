using UnityEngine;
using System.Collections;

public class ArrowMovement : MonoBehaviour {

	public float speed = 1.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.position += new Vector3 (0, speed * Time.deltaTime, 0);
	}
}
