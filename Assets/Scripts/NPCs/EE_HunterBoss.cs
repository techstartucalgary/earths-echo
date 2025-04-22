using UnityEngine;

public class EE_HunterBoss : EE_NPC 
{
	[Header("Boss Logic")]
	public Transform towerPointA;
	public Transform towerPointB;
	public Transform groundPoint;

	[Header("Cooldowns")]
	public float teleportCooldown = 5f;
	public float rangedAttackCooldown = 1.5f;
	public float meleeAttackCooldown = 0.7f;

	private float teleportTimer;
	private float rangedAttackTimer;
	private float meleeAttackTimer;

	[Header("Fight State")]
	public bool shouldFallToGround = false;
	public bool shouldJumpToTower = false;

	private bool onGround = false;
	private bool togglePosition = false;

	[Header("Additional Sensors")]
	[SerializeField]
	Sensor actionSensor;
	float targetSensorRadius;
	float actionSensorRadius;

	[Header("Ranged Attack")]
	public GameObject rangedProjectilePrefab;
	public Transform projectileSpawnPoint;
	public float projectileSpeed = 8f;

    // Start is called before the first frame update
    protected override void Start()
    {
    	base.Start(); 

		// set timers
		teleportTimer = teleportCooldown;
		rangedAttackTimer = rangedAttackCooldown;
		meleeAttackTimer = meleeAttackCooldown;

		// sensor info
		targetSensorRadius = base.targetSensor.detectionRadius;
		actionSensor.setTargetTag(base.target.tag);
		actionSensorRadius = actionSensor.detectionRadius;

    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

		TickTimers();

		HandleTeleporting();
		HandleSensorRadiuses();
		HandleAttacking();
			
    }

	void TickTimers() {
		float tick = Time.deltaTime;

		teleportTimer -= tick;
		rangedAttackTimer -= tick;
		meleeAttackTimer -= tick;
	}

	void TeleportTo(Transform target) {
		if(target != null) {
			transform.position = target.position;
		}else {
			Debug.LogWarning("Teleport target is not assigned!");
		}
	}

	void HandleTeleporting() {
		if (teleportTimer <= 0f) {
			if(shouldFallToGround) {
				shouldFallToGround = false;
				onGround = true;
			}
			else if (shouldJumpToTower){
				shouldJumpToTower = false;
				onGround = false;

				TeleportTo(towerPointB);
			}
			else if (onGround) {

			}
			else if (!onGround) {
				TeleportTo(togglePosition ? towerPointA : towerPointB);
				togglePosition = !togglePosition;

				base.path = null;
			}

			teleportTimer = teleportCooldown;
		}
	}

	void HandleAttacking() {
		if(meleeAttackTimer <= 0f && actionSensor.IsTargetInRange) {
			// Melee attack logic here

			meleeAttackTimer = meleeAttackCooldown;
		}

		if(rangedAttackTimer <= 0f && !actionSensor.IsTargetInRange) {
			Debug.Log("firing");
			// Ranged attack logic here
			if(rangedProjectilePrefab != null && projectileSpawnPoint != null) {
				Vector2 direction = (base.target.position - projectileSpawnPoint.position).normalized;
				float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
				Quaternion rotation = Quaternion.Euler(0, 0, angle);

				GameObject proj = Instantiate(rangedProjectilePrefab, projectileSpawnPoint.position, rotation);
				Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
				if(rb != null) {
					rb.velocity = direction * projectileSpeed;
				}
			}

			rangedAttackTimer = rangedAttackCooldown;
		}
	}

	void HandleSensorRadiuses() {
		if (onGround) {
			base.targetSensor.detectionRadius = targetSensorRadius;
			actionSensor.detectionRadius = actionSensorRadius;
		}
		else {
			base.targetSensor.detectionRadius = 1f;
			actionSensor.detectionRadius = 1f;
		}
	}

}
