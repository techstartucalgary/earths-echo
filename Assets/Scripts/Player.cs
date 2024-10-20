using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Controller2D))]
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

	Vector2 directionalInput;
	bool wallSliding;
	int wallDirX;

	void Start() {
		controller = GetComponent<Controller2D> ();

		gravity = -(2 * maxJumpHeight) / Mathf.Pow (timeToJumpApex, 2);
		maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
		minJumpVelocity = Mathf.Sqrt (2 * Mathf.Abs (gravity) * minJumpHeight);

        originalScale = transform.localScale; // Save original size for scaling during slide
    }

	void Update() {
		CalculateVelocity ();
		HandleWallSliding ();

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

	public void SetDirectionalInput (Vector2 input) {
		directionalInput = input;
	}

	public void OnJumpInputDown() {
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
        if (controller.collisions.below)
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
        velocity.x = Mathf.Max(velocity.x, (Mathf.Sign(velocity.x) * slideSpeed)); // Set sliding speed
    }

    public void StopSlide()
    {
        isSliding = false;
        isCrawling = false;
        transform.localScale = originalScale; // Reset to original size
        transform.position = new Vector3(transform.position.x, transform.position.y + (originalScale.y - transform.localScale.y) / 2, transform.position.z);
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
