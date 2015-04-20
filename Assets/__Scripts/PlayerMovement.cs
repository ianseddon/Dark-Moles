using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

	public float speed = 5.0f;

	public float jumpSpeed = 10.0f;
	public float jumpCooldown = 1.5f;

	public float yVelLimit = 30.0f;

	public float attackDistance1H = 2.0f;
	public float attackDamage1H = 5.0f;

	private float lastAttackTime = 0;
	public float attackCooldown = 0.2f;

	public AudioClip swordHitSound;

	private Animator anim;
	private GameObject boss;

	private float distanceToBoss;
	private float lastJumpTime = 0;
	
	public bool armorless = false;
	private bool knockedDown = false;
	private bool immobilized = false;

	// Use this for initialization
	void Start () {
		
	}

	void Awake () {
		anim = GetComponent<Animator>();
		boss = GameObject.FindGameObjectWithTag ("Boss");
	}

	// Update is called once per frame
	void Update () {
		//boss = GameObject.FindGameObjectWithTag ("Boss");
		distanceToBoss = Mathf.Abs (transform.position.x - boss.transform.position.x);

		AnimatorStateInfo animState = anim.GetCurrentAnimatorStateInfo (0);

		knockedDown = anim.GetBool ("Knocked Down");

		Vector3 newVelocity = rigidbody.velocity;

		bool jumping = Time.time - lastJumpTime < jumpCooldown;

		Vector3 facingVector = transform.right * transform.localScale.x * -1;
		facingVector = facingVector.normalized;

		// only reset x velocity if we're not in the air
		if(!jumping && !knockedDown && !immobilized) {

			newVelocity.x = 0;

			// limit our y velocity
			if(newVelocity.y > yVelLimit) newVelocity.y = yVelLimit;

			if (Mathf.Abs(Input.GetAxis ("Horizontal")) > 0) {

				float m = 1.0f;
				if(armorless) m = 2.0f;

				if(Input.GetAxis("Horizontal") > 0)
					newVelocity.x = speed * m;
				if(Input.GetAxis("Horizontal") < 0)
					newVelocity.x = -speed * m;
			}

			if (Input.GetButton("Jump") && !jumping) {
				lastJumpTime = Time.time;
				newVelocity.y = jumpSpeed;
				if(armorless) newVelocity.y *= 2;
			}

			if (Input.GetButton ("Fire1") && Time.time - lastAttackTime > attackCooldown && !armorless) {

				// play the animation
				//anim.Play("Attack_1h");
				anim.Play("Attack_Slash");

				RaycastHit hit = new RaycastHit();

				if(swordHitSound) {
					audio.Stop();
					audio.PlayOneShot(swordHitSound);
				}
				// hit something so try to do damage to it
				if(Physics.Raycast(transform.position, facingVector, out hit, attackDistance1H)) {
					hit.transform.gameObject.SendMessage("TakeDamage", attackDamage1H, SendMessageOptions.DontRequireReceiver);
				}
				else if(distanceToBoss < 2.0f && boss.transform.position.y < -1.0f) {
					boss.gameObject.SendMessage("TakeDamage", attackDamage1H);
				}

				lastAttackTime = Time.time;

			}
			else if (Input.GetButton ("Fire1") && Time.time - lastAttackTime > attackCooldown && armorless) {
				StartCoroutine(WhiteAttack());
			}

			if(Input.GetButton ("Undress") && !armorless) {
				StartCoroutine(Undress());
			}

			if (newVelocity.x > 0) {
				transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
			} else if (newVelocity.x < 0) {
				transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
			}
		}

		rigidbody.velocity = newVelocity;

		anim.SetFloat("Speed", Mathf.Abs(newVelocity.x));
	}

	IEnumerator WhiteAttack() {
		GameObject.Find ("WhiteAttack").GetComponent<Animator>().Play("WhiteAttack_Attack");
		boss.GetComponent<BossAI> ().enabled = false;
		yield return new WaitForSeconds (2f);
		boss.SendMessage ("TakeDamage", 2000f);
	}

	IEnumerator Undress() {
		anim.SetTrigger("Armorless");
		anim.SetBool("IsArmorless", true);
		armorless = true;
		immobilized = true;
		yield return new WaitForSeconds(1.5f);
		immobilized = false;
	}
}
