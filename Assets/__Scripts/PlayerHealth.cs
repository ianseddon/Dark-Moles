using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour {

	public float maxHealth = 100.0f;
	public float curHealth;
	public Healthbar healthBar;

	public float knockDownDuration = 1.5f;

	public float deathInputDelay = 1.0f;
	public float deathMessageLength = 4.0f;
	public GameObject deathMessage;

	private PlayerMovement mv;
	private Animator anim;
	private SpriteRenderer sr;
	
	private Color originalColor;
	public Color damageColor;

	private bool dead;
	private float deathTime;

	public AudioSource music;
	private GameObject boss;
	// Use this for initialization
	void Start () {
		curHealth = maxHealth;
	}

	void Awake () {
		mv = GetComponent<PlayerMovement> ();
		anim = GetComponent<Animator> ();
		sr = GetComponent<SpriteRenderer> ();
		boss = GameObject.FindGameObjectWithTag ("Boss");
		originalColor = sr.color;
	}
	
	// Update is called once per frame
	void Update () {

		// Allow the user to restart sooner by pressing a button
		if (dead && Input.anyKey && Time.time - deathTime > deathInputDelay) {
			RestartLevel();
		}

		// update our health bar
		//healthBar.pct = curHealth / maxHealth * 100.0f;

		// kill the player if necessary
		if (curHealth <= 0 && !dead) {
			// handle death
			StartCoroutine(HandleDeath());
		}
	}

	void TakeDamage (float dmg) {
		if (dead)
			return;

		// decrease our health
		curHealth -= dmg;

		// send the message to the health bar to update
		healthBar.SendMessage("UpdateBar", (curHealth / maxHealth * 100.0f));

		// flash our character red
		StartCoroutine (colorDamage ());

		// if we're not knocked down already knock the player down
		if(!anim.GetBool("Knocked Down")) StartCoroutine (knockDown (knockDownDuration));
	}

	IEnumerator colorDamage() {
		sr.color = damageColor;
		yield return new WaitForSeconds(0.1f);
		sr.color = originalColor;
	}

	IEnumerator knockDown(float duration) {

		// make sure we're facing the boss when we get hit so it doesn't look weird
		if (boss) {
			// boss is to our right so face right
			if(boss.transform.position.x > transform.position.x) transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
			else transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
		}

		// we're knocked down
		anim.SetBool ("Knocked Down", true);

		// don't get knocked down for as long if we're naked
		if (anim.GetBool ("IsArmorless"))
			duration /= 2;

		// wait for us to be able to get up
		yield return new WaitForSeconds (duration);

		// get up
		anim.SetBool ("Knocked Down", false);

	}

	IEnumerator HandleDeath() {

		dead = true;
		deathTime = Time.time;

		// Increment death counter
		DeathCounter deathCounter = GameObject.Find ("DeathCount").GetComponent<DeathCounter> ();
		deathCounter.AddDeath ();

		// Disable movement
		mv.enabled = false;

		// Stop us from being locked into the knocked down animation
		anim.SetBool ("Knocked Down", false);

		// Gib player
		anim.SetTrigger("Dead");

		// Show "You died" message
		deathMessage.SetActive (true);

		for (int i = 0; i < 4; i++) {
			yield return new WaitForSeconds (0.5f);
			music.pitch -= 0.2f;
		}
		yield return new WaitForSeconds (0.5f);
		music.Stop ();

		// Wait for a bit (or for button press)
		yield return new WaitForSeconds(deathMessageLength - 0.5f);

		// Reload level
		RestartLevel ();
	}

	void RestartLevel () {
		Application.LoadLevel(Application.loadedLevel);
	}
}
