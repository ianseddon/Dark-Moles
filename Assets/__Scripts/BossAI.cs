using UnityEngine;
using System.Collections;

public class BossAI : MonoBehaviour {

	public Transform player;
	private BossHealth hp;
	private Animator anim;

	private bool awake = false;
	public float awakeDistance = 12.0f;

	// attack vars
	public float attackSwipeRange = 2.0f;
	public float attackSwipeDamage = 15.0f;
	public float maxSwipeCooldown = 1.0f;
	private float swipeCooldown = 0.0f;

	public float attackDashRange = 2.0f;
	public float attackDashTime = 0.75f;
	public float attackDashDamage = 20.0f;
	public float maxDashCooldown = 3.0f;
	private float dashCooldown = 2.0f;

	public float attackJumpRange = 2.0f;
	public float attackJumpMaxDamage = 100.0f;
	public float attackJumpMinDamage = 60.0f;
	public float maxJumpCooldown = 8.0f;
	private float jumpCooldown = 0.0f;
	private float jumpStartTime = 0.0f;
	public float minJumpTime = 3.0f;

	public float walkSpeed = 4.0f;
	public float dashSpeed = 8.0f;

	// state machine vars
	private bool busy = false;		// should we interrupt the update loop?
	private bool sleeping = true;	// are we waiting between actions?
	public float sleepTime = 0.75f;
	private float lastActionTime = 0.0f;

	private bool close = false;
	public float closeRange = 4.0f;
	public float farRange = 8.0f;

	private float distanceToPlayer;

	// sounds
	public AudioClip jumpTakeoffSound;
	public AudioClip jumpLandSound;

	// Use this for initialization
	void Start () {

	}

	void Awake () {
		// Try to find the player if we don't have him assigned
		if (player == null)
			player = GameObject.FindGameObjectWithTag ("Player").transform;

		anim = GetComponent<Animator> ();
		hp = GetComponent<BossHealth> ();
	}

	// Update is called once per frame
	void Update () {

		anim.SetFloat ("VerticalSpeed", rigidbody.velocity.y);
		if(player) distanceToPlayer = Mathf.Abs (player.position.x - transform.position.x);

		// see if we should start attacking player
		if (!awake && distanceToPlayer < awakeDistance)
			awake = true;

		// Should we even bother going through update?
		if (busy || !awake)
			return;

		if (Time.time - lastActionTime > sleepTime)
			sleeping = false;

		// make sure we face the player
		if (transform.position.x >= player.position.x) {
			transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
		} else {
			transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
		}

		// not sleeping and not busy, so take an action
		if (!sleeping) {

			// we're close to the player
			if(distanceToPlayer <= closeRange) {

				if(swipeCooldown <=0) {

					StartCoroutine (SwipeAttack ());
				
				}

				else if(jumpCooldown <=0) {

					StartCoroutine (JumpAttack ());

				}

				else {
					//Debug.Log("idle");
				}

			}
			// we're mid range from the player
			else if(distanceToPlayer <= farRange) {
				
				if(dashCooldown <= 0)
					StartCoroutine (FollowPlayer ());
				//else Debug.Log("Idle");
			}
			// we're far from the player
			else {
				StartCoroutine (DashAttack());
			}
			
			// if we performed an action decrease all cooldowns by 1
			swipeCooldown--;
			dashCooldown--;
			jumpCooldown--;
		}
	}

	IEnumerator Idle() {

		busy = true;

		yield return new WaitForSeconds(0.5f);

		busy = false;
		sleeping = true;

	}

	IEnumerator SwipeAttack() {

		//Debug.Log ("Swiping");

		busy = true;

		// play the animation
		anim.Play("Boss_Swipe");

		yield return new WaitForSeconds (0.75f);
		//yield return WaitForAnimation ("Boss_Swipe");

		// play the animation
		// anim.Play("Attack_1h");
		
		RaycastHit hit = new RaycastHit();
		
		// hit something so try to do damage to it
		if(Physics.Raycast(transform.position, transform.right * transform.localScale.x * -1, out hit, attackSwipeRange) || distanceToPlayer < 1.0f) {
			//Debug.Log("Hit");
			player.transform.gameObject.SendMessage("TakeDamage", attackSwipeDamage);

			player.rigidbody.AddExplosionForce(500.0f, transform.position, attackJumpRange);
		}

		busy = false;
		sleeping = true;
		lastActionTime = Time.time;
		swipeCooldown = maxSwipeCooldown;
	}

	IEnumerator DashAttack() {

		//Debug.Log ("Dashing");

		busy = true;

		//Vector3 dashPosition = new Vector3 (player.position.x, transform.position.y, transform.position.z);

		float elapsedTime = 0.0f;

		Vector2 curPos = new Vector2 (transform.position.x, transform.position.y);
		Vector2 newPos = new Vector2 (player.position.x, transform.position.y);
		Vector2 closer = (newPos - curPos).normalized * 5f;
		newPos = newPos + closer;

		// Make sure our animation is the walk animation
		anim.SetBool ("Rolling", true);

		// travel at 8 units per second
		float travelTime = Mathf.Abs (transform.position.x - player.position.x) / dashSpeed;
		
		while (elapsedTime < travelTime)
		{
			//transform.position = Vector3.Lerp(curPos, newPos, (elapsedTime / travelTime));
			transform.position += new Vector3((newPos - curPos).normalized.x * distanceToPlayer / travelTime * Time.deltaTime, 0,0);
			elapsedTime += Time.deltaTime;
			yield return true;
		}

		anim.SetBool ("Rolling", false);

		// hit something so try to do damage to it
		if(distanceToPlayer < 2.0f) {
			Debug.Log("Hit");
			player.gameObject.SendMessage("TakeDamage", attackDashDamage);
			// knock the player over
		}

		busy = false;
		sleeping = true;
		lastActionTime = Time.time;
		dashCooldown = maxDashCooldown;
	}

	IEnumerator FollowPlayer() {
		
		busy = true;
		
		//Vector3 dashPosition = new Vector3 (player.position.x, transform.position.y, transform.position.z);
		
		float elapsedTime = 0.0f;

		Vector2 curPos = new Vector2 (transform.position.x, transform.position.y);
		Vector2 newPos = new Vector2 (player.position.x, transform.position.y);
		Vector2 closer = (newPos - curPos).normalized * 3;
		newPos = newPos - closer;

		// Make sure our animation is the walk animation
		anim.SetBool ("Walking", true);

		// travel at 4 units per second
		float travelTime = Mathf.Abs (transform.position.x - player.position.x) / walkSpeed;
		
		while (elapsedTime < travelTime)
		{
			//transform.position = Vector3.Lerp(curPos, newPos, (elapsedTime / travelTime));
			transform.position += new Vector3((newPos - curPos).normalized.x * distanceToPlayer / travelTime * Time.deltaTime, 0,0);
			elapsedTime += Time.deltaTime;
			yield return true;
		}
		
		anim.SetBool ("Walking", false);
		
		busy = false;
		//sleeping = true;
		//lastActionTime = Time.time;
	}

	IEnumerator JumpAttack() {

		busy = true;

		// Play the takeoff sound
		audio.PlayOneShot (jumpTakeoffSound);

		anim.Play ("Boss_Jump");

		yield return WaitForAnimation ("Boss_Jump");

		anim.Play ("Boss_Jump_GoingUp");

		jumpStartTime = Time.time;

		// Make it jump
		rigidbody.velocity = new Vector3 (0, 10, 0);

		yield return new WaitForSeconds (2.0f);

		// Shake the camera when we land
		Camera.main.gameObject.GetComponent<CameraShake> ().shake = 0.5f;

		// Play our landing sound
		audio.PlayOneShot (jumpLandSound);

		// Deal damage
		if (distanceToPlayer < attackJumpRange) {
			player.gameObject.SendMessage("TakeDamage", attackJumpMaxDamage - (distanceToPlayer/attackJumpRange*(attackJumpMaxDamage - attackJumpMinDamage)));

			player.rigidbody.AddExplosionForce(1000.0f, transform.position, attackJumpRange);
		}

		busy = false;
		sleeping = true;
		lastActionTime = Time.time;
		jumpCooldown = maxJumpCooldown;
	}

	private IEnumerator WaitForAnimation ( string animName )
	{
		AnimatorStateInfo si = anim.GetCurrentAnimatorStateInfo (0);
		do
		{
			si = anim.GetCurrentAnimatorStateInfo(0);
			yield return null;
		} while ( si.IsName(animName) );
	}

	private IEnumerator WaitForJump ( )
	{
		do
		{
			yield return null;
		} while ( Time.time - jumpStartTime < minJumpTime || Mathf.Abs(rigidbody.velocity.y) > 0.1f);
	}

	/*void OnGUI() {
		GUI.Label ( new Rect(10,20, 100, 20), distanceToPlayer.ToString() + " units away");
	}*/

}
