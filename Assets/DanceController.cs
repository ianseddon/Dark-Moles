using UnityEngine;
using System.Collections;

public class DanceController : MonoBehaviour {

	public GameObject dancer;
	public GameObject beatIndicator;
	public GameObject upArrow;
	public GameObject leftArrow;
	public GameObject rightArrow;

	public float bpm = 0;
	private float songStartTime = 0;

	// Use this for initialization
	void Start () {
		audio.Play ();
		StartCoroutine (OnBeat ());
		songStartTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {

	}

	IEnumerator OnBeat() {
		do {
			//StartCoroutine(CreateStep());
			yield return new WaitForSeconds (bpm / 60.0f - 0.1f);
		} while(audio.isPlaying);
	}

	public void CreateStep () {

		int arrow = Random.Range (1, 4);

		switch (arrow) {
			case 1:
				Instantiate (upArrow, transform.position, upArrow.transform.rotation);
				break;

			case 2:
				Instantiate (leftArrow, new Vector3(transform.position.x -1.0f, transform.position.y, transform.position.z), leftArrow.transform.rotation);
				break;

			case 3:
				Instantiate (rightArrow, new Vector3(transform.position.x + 1.0f, transform.position.y, transform.position.z), rightArrow.transform.rotation);
				break;

			default:
				break;
		}
	}
}
