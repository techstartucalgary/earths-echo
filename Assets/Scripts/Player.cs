using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Controller2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class Player : MonoBehaviour {

	public float maxJumpHeight = 4;
	public float minJumpHeight = 1;
	public float timeToJumpApex = .4f;
	float accelerationTimeAirborne = .2f;
	float accelerationTimeGrounded = .1f;
	float moveSpeed = 6;

    [SerializeField] public float playerSpeed;

    [SerializeField] private float sprintMultiplier = 1.5f;  // Speed multiplier when sprinting
    [SerializeField] private float sprintRampUpTime = 1f;    // Time to reach full sprint speed
    private float currentSprintTime = 0f;                    // Tracks ramp-up progress
    public bool isSprinting = false;

    [SerializeField] private float slideSpeed = 10f;            // Speed boost during slide
    [SerializeField] private float slideJumpBoost = 1.5f;       // Horizontal boost when slide jumping
    [SerializeField] private float crawlSpeed = 2f;             // Slow speed during crawling
    [SerializeField] private float slideCooldown = 0.5f;        // Time between slide jumps
    [SerializeField] private float slideShrinkFactor = 0.5f;    // Factor to shrink player when sliding
    public bool isSliding = false;
    private bool isCrawling = false;
    private Vector3 originalScale;                              // Store the original player scale

    public Vector2 wallJumpClimb;
	public Vector2 wallJumpOff;
	public Vector2 wallLeap;

	public float wallSlideSpeedMax = 3;
	public float wallStickTime = .25f;
	float timeToWallUnstick;

	float gravity;
	float maxJumpVelocity;
	float minJumpVelocity;
	public Vector3 velocity;
	float velocityXSmoothing;

	Controller2D controller;
    BoxCollider2D boxCollider;

	Vector2 directionalInput;
	bool wallSliding;
	int wallDirX;

    // Ladder stuff
    public GameObject touchingLadder = null;
    public bool canClimb = false;
    public bool climbing = false;
    public float climbSpeed = 5f;
    public float dismountTime = 1.5f;
    public float timeToReclimb = 0f;

    void Start() {
		controller = GetComponent<Controller2D> ();
        boxCollider = GetComponent<BoxCollider2D> ();

		gravity = -(2 * maxJumpHeight) / Mathf.Pow (timeToJumpApex, 2);
		maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
		minJumpVelocity = Mathf.Sqrt (2 * Mathf.Abs (gravity) * minJumpHeight);

        originalScale = transform.localScale; // Save original size for scaling during slide
    }

	void Update() {
		
        if (canClimb && directionalInput.y > 0 && timeToReclimb <= 0)
        {
            climbing = true;
            AlignToLadder();
        }
        else if (timeToReclimb > 0)
        {
            timeToReclimb -= Time.deltaTime;
        }

        if (climbing)
        {
            ClimbLadder();
        }
        else
        {
            CalculateVelocity();
        }

        HandleWallSliding();

        controller.Move (velocity * Time.deltaTime, directionalInput);

		if (controller.collisions.above || controller.collisions.below) {
			if (controller.collisions.slidingDownMaxSlope) {
				velocity.y += controller.collisions.slopeNormal.y * -gravity * Time.deltaTime;
			} else {
				velocity.y = 0;
			}
		}

        HandleSprinting();
        HandleSliding();
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            touchingLadder = collision.gameObject; // Save the ladder object
            canClimb = true;                      // Set climbing state to true
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        // Check if the player leaves a ladder
        if (collision.CompareTag("Ladder"))
        {
            touchingLadder = null; // Clear the ladder reference
            canClimb = false;     // Set climbing state to false
        }
    }

    public void AlignToLadder()
    {
        if (touchingLadder != null)
        {
            Vector3 ladderPosition = touchingLadder.transform.position;

            // Align the player's X position to the ladder's X position
            Vector3 newPlayerPosition = transform.position;
            newPlayerPosition.x = ladderPosition.x;
            transform.position = newPlayerPosition;
        }
    }

    public void ClimbLadder()
    {
        BoxCollider2D ladderCollider = touchingLadder.GetComponent<BoxCollider2D>();
        float ladderTop = ladderCollider.bounds.max.y; // Top of the ladder
        float ladderBottom = ladderCollider.bounds.min.y; // Bottom of the ladder

        BoxCollider2D playerCollider = GetComponent<BoxCollider2D>();
        float playerTop = playerCollider.bounds.max.y; // Top of the player
        float playerBottom = playerCollider.bounds.min.y; // Bottom of the player

        velocity.x = 0;
        velocity.y = directionalInput.y * climbSpeed;

        if (directionalInput.y == 0 || 
           (playerBottom >= ladderTop && directionalInput.y > 0) || 
           (playerTop <= ladderBottom && directionalInput.y < 0))
        {
            velocity.y = 0;
        }
    }

    public void SetDirectionalInput (Vector2 input) {
		directionalInput = input;
	}

	public void OnJumpInputDown() {

        bool wasClimbing = false;
        if (climbing)
        {
            climbing = false;
            wasClimbing = true;
            timeToReclimb = dismountTime;
        }

        if (wallSliding)
        {
            if (wallDirX == directionalInput.x)
            {
                velocity.x = -wallDirX * wallJumpClimb.x;
                velocity.y = wallJumpClimb.y;
            }
            else if (directionalInput.x == 0)
            {
                velocity.x = -wallDirX * wallJumpOff.x;
                velocity.y = wallJumpOff.y;
            }
            else
            {
                velocity.x = -wallDirX * wallLeap.x;
                velocity.y = wallLeap.y;
            }
        }
        if (controller.collisions.below || wasClimbing)
        {
            if (controller.collisions.slidingDownMaxSlope)
            {
                if (directionalInput.x != -Mathf.Sign(controller.collisions.slopeNormal.x))
                { // not jumping against max slope
                    velocity.y = maxJumpVelocity * controller.collisions.slopeNormal.y;
                    velocity.x = maxJumpVelocity * controller.collisions.slopeNormal.x;
                }
            }
            else if (isSliding)
            {
                velocity.x = Mathf.Sign(velocity.x) * Mathf.Max(Mathf.Abs(velocity.x) * slideJumpBoost, slideJumpBoost * moveSpeed); // Boost horizontal velocity on jump
                velocity.y = maxJumpVelocity * 0.8f;
                StopSlide();

            }
            else
            {
                velocity.y = maxJumpVelocity;
            }
        }
	}

	public void OnJumpInputUp() {
		if (velocity.y > minJumpVelocity) {
			velocity.y = minJumpVelocity;
		}
	}	

	void HandleWallSliding() {
		wallDirX = (controller.collisions.left) ? -1 : 1;
		wallSliding = false;
		if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0) {
			wallSliding = true;

			if (velocity.y < -wallSlideSpeedMax) {
				velocity.y = -wallSlideSpeedMax;
			}

			if (timeToWallUnstick > 0) {
				velocityXSmoothing = 0;
				velocity.x = 0;

				if (directionalInput.x != wallDirX && directionalInput.x != 0) {
					timeToWallUnstick -= Time.deltaTime;
				}
				else {
					timeToWallUnstick = wallStickTime;
				}
			}
			else {
				timeToWallUnstick = wallStickTime;
			}

		}

	}

    void CalculateVelocity()
    {
        float targetVelocityX;
        if (controller.collisions.below)
        {
            if (isSliding)
            {
                targetVelocityX = directionalInput.x * crawlSpeed;
            }
            else
            {
                targetVelocityX = directionalInput.x * moveSpeed;
            }
        }
        else
        {
            targetVelocityX = directionalInput.x * moveSpeed;
        }
        

        // Apply sprint multiplier if sprinting
        if (isSprinting)
        {
            float sprintFactor = Mathf.Lerp(1, sprintMultiplier, currentSprintTime / sprintRampUpTime);
            targetVelocityX *= sprintFactor;
        }

        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
        velocity.y += gravity * Time.deltaTime;
    }

    void HandleSprinting()
    {
        if (isSprinting)
        {
            if (currentSprintTime < sprintRampUpTime)
            {
                currentSprintTime += Time.deltaTime;
            }
        }
        else
        {
            currentSprintTime = 0f;
        }
    }

    public void SetSprinting(bool sprinting)
    {
        isSprinting = sprinting;
    }

    public void StartSlide()
    {
        if (!controller.collisions.below || !isSprinting || Mathf.Abs(velocity.x) < moveSpeed) return;  // Require sprinting and ground contact

        isSliding = true;
        isCrawling = false;
        transform.localScale = new Vector3(originalScale.x, originalScale.y * slideShrinkFactor, originalScale.z);  // Shrink player vertically
        transform.position = new Vector3(transform.position.x, transform.position.y - (originalScale.y - transform.localScale.y) / 2, transform.position.z);
        boxCollider.transform.position = transform.position;
        boxCollider.bounds.size.Set(transform.localScale.x, transform.localScale.y, transform.localScale.z);
        controller.UpdateRaycastOrigins();
        velocity.x = Mathf.Max(velocity.x, (Mathf.Sign(velocity.x) * slideSpeed)); // Set sliding speed
    }

    public void StopSlide()
    {
        isSliding = false;
        isCrawling = false;
        transform.localScale = originalScale; // Reset to original size
        transform.position = new Vector3(transform.position.x, transform.position.y + (originalScale.y - transform.localScale.y) / 2, transform.position.z);
        boxCollider.transform.position = transform.position;
        boxCollider.bounds.size.Set(transform.localScale.x, transform.localScale.y, transform.localScale.z);
        controller.UpdateRaycastOrigins();
    }

    void HandleSliding()
    {
        if (!isSliding) return;

        if (directionalInput.y < 0)
        {
            isCrawling = true;
            velocity.x = Mathf.SmoothDamp(velocity.x, crawlSpeed, ref velocityXSmoothing, accelerationTimeGrounded);
        }

        if (Mathf.Abs(velocity.x) < crawlSpeed && directionalInput.y >= 0)
        {
            StopSlide();
        }
    }

}
