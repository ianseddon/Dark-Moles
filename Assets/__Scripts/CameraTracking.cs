using UnityEngine;
using System.Collections;

public class CameraTracking : MonoBehaviour {

	public Transform target;

	public float leftLimit = -3.5f;
	public float rightLimit = 10.0f;

	// Use this for initialization
	void Awake () {
		if (target == null)
			target = GameObject.FindGameObjectWithTag ("Player").transform;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		Vector3 newPosition = transform.position;
		if (target.position.x <= leftLimit)
			newPosition.x = leftLimit;
		else if (target.position.x >= rightLimit)
			newPosition.x = rightLimit;
		else 
			newPosition.x = target.position.x;

		transform.position = newPosition;
	}
}
