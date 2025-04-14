using UnityEngine;
using System.Collections;
using System;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(Controller2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class Player : MonoBehaviour, IDamageable
{
    [Header("Animation")]
    [SerializeField] Animator animator;
    [SerializeField] Transform animatorTransform;
    float animatorXScale;

    [Header("Physics")]
    // Jumping variables
    public float maxJumpHeight = 4;
    public float minJumpHeight = 1;
    public float timeToJumpApex = .4f;
    float accelerationTimeAirborne = .2f;
    float accelerationTimeGrounded = .1f;
    float gravity;
    float maxJumpVelocity;
    float minJumpVelocity;

    // Movement variables
    Vector2 directionalInput;
    float moveSpeed = 6;
    public Vector3 velocity;
    float velocityXSmoothing;
    [SerializeField] public float playerSpeed;
    [SerializeField] private float sprintMultiplier = 1.5f;  // Speed multiplier when sprinting
    [SerializeField] private float sprintRampUpTime = 1f;    // Time to reach full sprint speed
    private float currentSprintTime = 0f;                    // Tracks ramp-up progress
    public bool isSprinting = false;

    // Sliding variables
    [SerializeField] private float slideMultiplier = 0.3f;            // Speed boost during slide
    [SerializeField] private float slideRampDownTime = 2f;    // Time to reach lowest slide speed
    private float currentSlideTime = 0f;
    [SerializeField] private float slideJumpBoost = 1.5f;       // Horizontal boost when slide jumping
    [SerializeField] private float crawlSpeed = 2f;             // Slow speed during crawling
    [SerializeField] private float slideCooldown = 0.5f;        // Time between slide jumps
    [SerializeField] private float slideShrinkFactor = 0.5f;    // Factor to shrink player when sliding
    public bool isSliding = false;
    private bool isCrawling = false;
    public bool isCharging = false; // bow

    // Private invincibility flag for damage handling.
    private bool isInvincible = false;

    private Vector3 originalScale;                              // Store the original player scale

    // Wall interaction variables
    public Vector2 wallJumpClimb;
    public Vector2 wallJumpOff;
    public Vector2 wallLeap;
    public float wallSlideSpeedMax = 3;
    public float wallStickTime = .25f;
    float timeToWallUnstick;
    bool wallSliding;
    int wallDirX;

    // References
    Controller2D controller;
    BoxCollider2D boxCollider;

	[Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth = 100f;
    public HealthBar healthBar;

    // Ladder stuff
    public GameObject touchingLadder = null;
    public bool canClimb = false;
    public bool climbing = false;
    public float climbSpeed = 5f;
    public float dismountTime = 1.5f;
    public float timeToReclimb = 0f;

    // Attack stuff
    [Header("Melee Attack Settings")]
    public float attackDamage = 1f;
    public float attackCooldown = 0f;
    public float attackRange = 0.5f;
    public string attackAnimPrefix = "";
    public AudioClip[] attackSounds;

    public float lastMeleeAttackTime = 0f;
    private Transform UpwardsHitpoint;
    private Transform DownwardHitpoint;
    private Transform SideHitpoint;

    private float SideHitpointLocalX;
    public LayerMask enemyLayer;

    private Camera mainCamera;
    private ScreenShake screenShake;

    [SerializeField] private AudioClip[] damageSoundClips;
    [SerializeField] private AudioClip[] deathSoundClips;

    [Header("Healing Settings")]
    [SerializeField] private float healDelay = 10f;        // Time (in seconds) to wait after taking damage
    [SerializeField] private float healingDuration = 5f;     // Seconds required to heal fully
    private float lastDamageTime = 0f;                       // Time when the player last took damage
    private Coroutine healingCoroutine = null;


    FindGrandchildren finder;

    void Start()
    {
        controller = GetComponent<Controller2D>();
        boxCollider = GetComponent<BoxCollider2D>();

        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);

        originalScale = transform.localScale; // Save original size for scaling during slide

        animatorXScale = animatorTransform.localScale[0];

        finder = new FindGrandchildren();

        UpwardsHitpoint = finder.FindDeepChild(transform, "UpwardHitpoint");
        DownwardHitpoint = finder.FindDeepChild(transform, "DownwardHitpoint");
        SideHitpoint = finder.FindDeepChild(transform, "SideHitpoint");
        SideHitpointLocalX = SideHitpoint.localPosition.x;

        mainCamera = Camera.main;
        screenShake = mainCamera.GetComponent<ScreenShake>();
        if (mainCamera == null)
        {
            Debug.LogError("Main camera not found");
            return;
        }
		currentHealth = maxHealth;
        if (healthBar != null)
        {
            healthBar.Initialize(maxHealth);
        }
    }

    void Update()
    {
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

        controller.Move(velocity * Time.deltaTime, directionalInput);

        if (controller.collisions.above || controller.collisions.below)
        {
            if (controller.collisions.slidingDownMaxSlope)
            {
                velocity.y += controller.collisions.slopeNormal.y * -gravity * Time.deltaTime;
            }
            else
            {
                velocity.y = 0;
            }
        }

        HandleSprinting();
        HandleSliding();

        // y axis flip stuff
        if (animator != null)
        {
            animator.SetBool("isJumping", !controller.collisions.below);
            animator.SetFloat("xVelocity", Math.Abs(velocity.x));
            animator.SetFloat("yVelocity", velocity.y);

            if (velocity.x >= 0.01f)
            {
                animatorTransform.localScale = new Vector3(animatorXScale, animatorTransform.localScale.y, animatorTransform.localScale.z);
                SideHitpoint.localPosition = new Vector3(SideHitpointLocalX, SideHitpoint.localPosition.y, SideHitpoint.localPosition.z);
            }
            else if (velocity.x <= -0.01f)
            {
                animatorTransform.localScale = new Vector3(-animatorXScale, animatorTransform.localScale.y, animatorTransform.localScale.z);
                SideHitpoint.localPosition = new Vector3(-SideHitpointLocalX, SideHitpoint.localPosition.y, SideHitpoint.localPosition.z);
            }
            isChargingHelper();
        }
        if (isSliding && animator != null)
        {
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("sliding"))
            {
                animator.Play("sliding");
            }
        }

        if (currentHealth < maxHealth && Time.time - lastDamageTime >= healDelay && healingCoroutine == null)
        {
            healingCoroutine = StartCoroutine(HealToFull());
        }
        if(healthBar!=null){
            currentHealth = healthBar.GetCurrentHealth();
        }
        
    }

    private void isChargingHelper()
    {
        if (isCharging)
        {
            Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector3 direction = (mouseWorldPosition - transform.position).normalized;
            if (direction.x >= 0f)
            {
                animatorTransform.localScale = new Vector3(animatorXScale, animatorTransform.localScale.y, animatorTransform.localScale.z);
                SideHitpoint.localPosition = new Vector3(SideHitpointLocalX, SideHitpoint.localPosition.y, SideHitpoint.localPosition.z);
            }
            else
            {
                animatorTransform.localScale = new Vector3(-animatorXScale, animatorTransform.localScale.y, animatorTransform.localScale.z);
                SideHitpoint.localPosition = new Vector3(-SideHitpointLocalX, SideHitpoint.localPosition.y, SideHitpoint.localPosition.z);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "FallDetector")
        {
        }
        else if (collision.tag == "Checkpoint")
        {
            GameManager.instance.SetRespawnPoint(collision.transform);
        }
        else if (collision.tag == "NextLevel")
        {
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else if (collision.tag == "PreviousLevel")
        {
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        }
        else if (collision.CompareTag("Ladder"))
        {
            touchingLadder = collision.gameObject; // Save the ladder object
            canClimb = true;                      // Set climbing state to true
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "spike")
        {
            healthBar.Damage(5f);
        }
        if (collision.tag == "water")
        {
            healthBar.Damage(5f);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            touchingLadder = null;
            canClimb = false;
        }
    }

    public void AlignToLadder()
    {
        if (touchingLadder != null)
        {
            Vector3 ladderPosition = touchingLadder.transform.position;
            Vector3 newPlayerPosition = transform.position;
            newPlayerPosition.x = ladderPosition.x;
            transform.position = newPlayerPosition;
        }
    }

    public void ClimbLadder()
    {
        BoxCollider2D ladderCollider = touchingLadder.GetComponent<BoxCollider2D>();
        float ladderTop = ladderCollider.bounds.max.y;
        float ladderBottom = ladderCollider.bounds.min.y;

        BoxCollider2D playerCollider = GetComponent<BoxCollider2D>();
        float playerTop = playerCollider.bounds.max.y;
        float playerBottom = playerCollider.bounds.min.y;

        velocity.x = 0;
        velocity.y = directionalInput.y * climbSpeed;

        if (directionalInput.y == 0 ||
           (playerBottom >= ladderTop && directionalInput.y > 0) ||
           (playerTop <= ladderBottom && directionalInput.y < 0))
        {
            velocity.y = 0;
        }
    }

    public void SetDirectionalInput(Vector2 input)
    {
        directionalInput = input;
    }

    public void OnJumpInputDown()
    {
        bool wasClimbing = false;
        if (climbing)
        {
            climbing = false;
            wasClimbing = true;
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
                {
                    velocity.y = maxJumpVelocity * controller.collisions.slopeNormal.y;
                    velocity.x = maxJumpVelocity * controller.collisions.slopeNormal.x;
                }
            }
            else if (isSliding)
            {
                velocity.x = Mathf.Sign(velocity.x) * Mathf.Max(Mathf.Abs(velocity.x) * slideJumpBoost, slideJumpBoost * moveSpeed);
                velocity.y = maxJumpVelocity * 0.8f;
                StopSlide();
            }
            else
            {
                velocity.y = maxJumpVelocity;
            }
        }
    }

    public void OnJumpInputUp()
    {
        if (velocity.y > minJumpVelocity)
        {
            velocity.y = minJumpVelocity;
        }
    }

    void HandleWallSliding()
    {
        wallDirX = (controller.collisions.left) ? -1 : 1;
        wallSliding = false;
        if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0)
        {
            wallSliding = true;

            if (velocity.y < -wallSlideSpeedMax)
            {
                velocity.y = -wallSlideSpeedMax;
            }

            if (timeToWallUnstick > 0)
            {
                velocityXSmoothing = 0;
                velocity.x = 0;

                if (directionalInput.x != wallDirX && directionalInput.x != 0)
                {
                    timeToWallUnstick -= Time.deltaTime;
                }
                else
                {
                    timeToWallUnstick = wallStickTime;
                }
            }
            else
            {
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

        if (isSprinting && !isSliding)
        {
            float sprintFactor = Mathf.Lerp(1, sprintMultiplier, currentSprintTime / sprintRampUpTime);
            targetVelocityX *= sprintFactor;
        }
        else if (isSliding)
        {
            float slideFactor = Mathf.Lerp(sprintMultiplier, slideMultiplier, currentSlideTime / slideRampDownTime);
            targetVelocityX *= slideFactor;
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
        if (!controller.collisions.below || !isSprinting || Mathf.Abs(velocity.x) < moveSpeed)
            return;

        isSliding = true;
        isCrawling = false;
        transform.localScale = new Vector3(originalScale.x, originalScale.y * slideShrinkFactor, originalScale.z);
        transform.position = new Vector3(transform.position.x, transform.position.y - (originalScale.y - transform.localScale.y) / 2, transform.position.z);
        boxCollider.transform.position = transform.position;
        controller.UpdateRaycastOrigins();
    }

    public void StopSlide()
    {
        isSliding = false;
        isCrawling = false;
        transform.localScale = originalScale;
        transform.position = new Vector3(transform.position.x, transform.position.y + (originalScale.y - transform.localScale.y) / 2, transform.position.z);
        boxCollider.transform.position = transform.position;
        controller.UpdateRaycastOrigins();
    }

    public void PerformSideAttack(float attackDamage, float attackRange)
    {
        if (Time.time < lastMeleeAttackTime + attackCooldown) return;
        lastMeleeAttackTime = Time.time;
        ApplyHitbox(SideHitpoint, attackDamage, attackRange);
        if (!controller.collisions.below)
        {
            animator.Play(attackAnimPrefix + "player_forward_air");
            SoundFXManager.Instance.PlayRandomSoundFXClip(attackSounds, transform, 1f);
        }
        else
        {
            animator.Play(attackAnimPrefix + "player_attack");
            SoundFXManager.Instance.PlayRandomSoundFXClip(attackSounds, transform, 1f);
        }
    }

    public void PerformUpAttack(float attackDamage, float attackRange)
    {
        if (Time.time < lastMeleeAttackTime + attackCooldown) return;
        lastMeleeAttackTime = Time.time;
        ApplyHitbox(UpwardsHitpoint, attackDamage, attackRange);
        if (!controller.collisions.below)
        {
            animator.Play(attackAnimPrefix + "player_up_air");
            SoundFXManager.Instance.PlayRandomSoundFXClip(attackSounds, transform, 1f);
        }
        else
        {
            animator.Play(attackAnimPrefix + "player_up_attack");
            SoundFXManager.Instance.PlayRandomSoundFXClip(attackSounds, transform, 1f);
        }
    }

    public void PerformDownAttack(float attackDamage, float attackRange)
    {
        if (Time.time < lastMeleeAttackTime + attackCooldown) return;
        lastMeleeAttackTime = Time.time;
        ApplyHitbox(DownwardHitpoint, attackDamage, attackRange);
        if (!controller.collisions.below)
        {
            animator.Play(attackAnimPrefix + "player_down_air");
            SoundFXManager.Instance.PlayRandomSoundFXClip(attackSounds, transform, 1f);
        }
        else
        {
            animator.Play(attackAnimPrefix + "player_down_attack");
            SoundFXManager.Instance.PlayRandomSoundFXClip(attackSounds, transform, 1f);
        }
    }

    private void ApplyHitbox(Transform attackDirection, float attackDamage, float attackRange)
    {
        if (attackDirection == null) return;

        Vector3 attackPos = new Vector2(Mathf.Sign(animatorTransform.localScale.x), 0f);
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(new Vector2(attackDirection.position.x, attackDirection.position.y), attackRange, enemyLayer);

        foreach (var enemy in hitEnemies)
        {
            ApplyDamage(enemy, attackDamage, attackPos);
        }
    }

    private void ApplyDamage(Collider2D enemy, float attackDamage, Vector3 impactPos)
    {
        IDamageable iDamageable = enemy.GetComponent<IDamageable>();
        if (iDamageable != null)
        {
            iDamageable.Damage(attackDamage, impactPos);
            Debug.Log($"Dealt {attackDamage} damage to {enemy.name}");
        }
        else
        {
            Debug.Log($"{enemy.name} does not implement IDamageable.");
        }
    }

    /// <summary>
    /// Plays the pullback animation for a projectile weapon.
    /// </summary>
    public void PlayProjectilePullbackAnimation(string animPrefix)
    {
        if (animator != null)
        {
            animator.Play(animPrefix + "pullback");
            Debug.Log("Playing projectile pullback animation with prefix: " + animPrefix);
        }
        else
        {
            Debug.LogWarning("Animator reference is missing in Player script!");
        }
    }

    public void UpdateProjectilePullback(float pullbackCharge)
    {
        if (animator != null)
        {
            animator.SetFloat("PullbackCharge", pullbackCharge);
            Debug.Log("Updating projectile pullback to: " + pullbackCharge);
        }
    }

    public void ResetProjectileAnimation(string animPrefix)
    {
        if (animator != null)
        {
            animator.Play(animPrefix + "idle");
            Debug.Log("Resetting projectile animation to idle with prefix: " + animPrefix);
        }
    }

    public void PlayThrowableAnimation(string animPrefix)
    {
        if (animator != null)
        {
            animator.Play(animPrefix + "throw");
            Debug.Log("Playing throwable animation with prefix: " + animPrefix);
        }
    }

    void HandleSliding()
    {
        if (isSliding)
        {
            if (currentSlideTime < slideRampDownTime)
            {
                currentSlideTime += Time.deltaTime;
            }
        }
        if (!isSliding)
        {
            currentSlideTime = 0f;
            return;
        }

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

#if UNITY_EDITOR
    // Draw Gizmos for attack hitboxes in the Editor
    private void OnDrawGizmosSelected()
    {
        float range = attackRange;

        if (SideHitpoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(SideHitpoint.position, range);
            Collider2D[] sideHits = Physics2D.OverlapCircleAll(SideHitpoint.position, range, enemyLayer);
            foreach (Collider2D hit in sideHits)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawCube(hit.bounds.center, hit.bounds.size);
            }
        }

        if (UpwardsHitpoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(UpwardsHitpoint.position, range);
            Collider2D[] upHits = Physics2D.OverlapCircleAll(UpwardsHitpoint.position, range, enemyLayer);
            foreach (Collider2D hit in upHits)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawCube(hit.bounds.center, hit.bounds.size);
            }
        }

        if (DownwardHitpoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(DownwardHitpoint.position, range);
            Collider2D[] downHits = Physics2D.OverlapCircleAll(DownwardHitpoint.position, range, enemyLayer);
            foreach (Collider2D hit in downHits)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawCube(hit.bounds.center, hit.bounds.size);
            }
        }
    }
#endif

    // IDamageable interface implementations

    public void Damage(float damageAmount, Vector2 impactPos)
    {
        // (Optional) If invincible, you can exit early.
        // if (isInvincible) return;

        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // Update the health bar and play damage sounds/effects.
        if (healthBar != null)
        {
            healthBar.Damage(damageAmount);
            SoundFXManager.Instance.PlayRandomSoundFXClip(damageSoundClips, transform, 0.6f);
            ApplyKnockback(impactPos, damageAmount * 0.5f);
            screenShake.Shake(0.2f, 0.5f);
        }
        
        // Record the time of damage so healing will wait.
        lastDamageTime = Time.time;
        
        // If already healing, stop the healing coroutine.
        if (healingCoroutine != null)
        {
            StopCoroutine(healingCoroutine);
            healingCoroutine = null;
        }

        if (currentHealth <= 0)
        {
            SoundFXManager.Instance.PlayRandomSoundFXClip(deathSoundClips, transform, 0.6f);
            // Handle death (play death animation, remove colliders, etc.)
            screenShake.Shake(0.5f, 0.75f);
        }
        Debug.Log("Player took " + damageAmount + " damage.");
    }


    public void Heal(float healAmount)
    {
        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (healthBar != null)
        {
            healthBar.Heal(healAmount);
        }
        Debug.Log("Player healed " + healAmount);
    }

    public void SetInvincible(bool invincible)
    {
        isInvincible = invincible;
        Debug.Log("Player invincibility set to " + invincible);
    }

    public void ApplyKnockback(Vector3 direction, float force)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.AddForce(direction.normalized * force, ForceMode2D.Impulse);
            StartCoroutine(ResetKnockback(rb, 0.2f));
        }
        Debug.Log("Applied knockback with force " + force);
    }

    private IEnumerator ResetKnockback(Rigidbody2D rb, float delay)
    {
        yield return new WaitForSeconds(delay);
        rb.velocity = Vector2.zero;
    }

        private IEnumerator HealToFull()
    {
        // While the player's health is not full, apply incremental healing.
        while (currentHealth < maxHealth)
        {
            // The healing rate per second (so full recovery happens over healingDuration)
            float healAmount = (maxHealth / healingDuration) * Time.deltaTime;
            Heal(healAmount);  // Call your existing Heal method
            yield return null;
        }
        // Once healing is complete, clear the coroutine reference.
        healingCoroutine = null;
    }


}
