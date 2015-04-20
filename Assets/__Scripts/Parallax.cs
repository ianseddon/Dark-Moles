using UnityEngine;
using System.Collections;

public class Parallax : MonoBehaviour {

	public float smoothing = 1.0f;
	public float parallaxFactor = 10.0f;
	public float xOffset = 0.0f;

	// Use this for initialization
	void Start () {
		xOffset = transform.position.x;
	}
	
	// Update is called once per frame
	void Update () {

		Camera mainCamera = Camera.main;

		Vector3 newPosition = transform.position;
		newPosition.x = Mathf.Lerp (transform.position.x, (mainCamera.transform.position.x / parallaxFactor) + xOffset, smoothing);
		transform.position = newPosition;
	}
}
