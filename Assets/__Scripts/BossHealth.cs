using UnityEngine;
using System.Collections;

public class BossHealth : MonoBehaviour {

	public float maxHealth = 100.0f;
	public Healthbar healthBar;

	private float curHealth;

	private SpriteRenderer sr;
	private Color originalColor;
	public Color damageColor;

	public GUIText soulsText;
	public GameObject player;
	public GameObject winscreenArmor;
	public GameObject winscreenNoArmor;

	private bool dead = false;

	// Use this for initialization
	void Start () {
		curHealth = maxHealth;
		sr = GetComponent<SpriteRenderer> ();
		originalColor = sr.color;
	}

	void Awake () {
		if(!player)
			player = GameObject.FindGameObjectWithTag ("Player");
	}
	
	// Update is called once per frame
	void Update () {
		if (!player)
			player = GameObject.FindGameObjectWithTag ("Player");

		if (Input.GetButton ("Fire2"))
			TakeDamage(100f);

		// update the health bar
		healthBar.pct = curHealth / maxHealth * 100.0f;

		// check if dead (shouldn't happen)
		if (curHealth <= 0) {
			//Debug.Log("deadzolio");
		}
	}

	void TakeDamage (float dmg) {
		curHealth -= dmg;
		StartCoroutine (colorDamage());

		if (curHealth <= 0f && !dead) {
			StartCoroutine("HandleDeath");
		}
	}

	IEnumerator colorDamage() {
		sr.color = damageColor;
		yield return new WaitForSeconds(0.1f);
		sr.color = originalColor;
	}

	IEnumerator HandleDeath() {

		dead = true;

		// Disable player movement
		player.GetComponent<PlayerMovement> ().enabled = false;

		// disable AI
		GetComponent<BossAI> ().enabled = false;

		// play death animation
		GetComponent<Animator> ().Play ("Boss_Death");

		float souls = 0f;

		soulsText.color = Color.green;

		// increase souls
		while (souls < 9999.0f) {
			souls += Time.deltaTime * 3000;
			if(souls >= 9999.0f) souls = 9999f;
			soulsText.text = ((int) souls).ToString();
			yield return null;
		}

		soulsText.color = Color.white;

		GameObject.FindGameObjectWithTag("BossHealth").SetActive(false);

		//player.GetComponent<PlayerMovement> ().enabled = true;

		if (player.GetComponent<PlayerMovement> ().armorless) {
			Application.LoadLevel ("Bonus");
		}
		else {
			winscreenArmor.SetActive (true);
			yield return new WaitForSeconds(1.5f);
			winscreenArmor.SendMessage("EnableRestart");
		}
		Destroy (gameObject);

	}
}
