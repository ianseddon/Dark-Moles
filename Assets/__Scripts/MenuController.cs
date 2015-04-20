using UnityEngine;
using System.Collections;

public class MenuController : MonoBehaviour {

	public string level = "Level1";
	public Animator fade;

	private bool startingGame = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.anyKeyDown && !startingGame) {
			StartCoroutine(StartGame ());
		}
	}

	IEnumerator StartGame() {
		fade.SetTrigger ("Fade");
		yield return new WaitForSeconds(2);
		Application.LoadLevel (level);
	}
}
