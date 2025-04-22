using UnityEngine;

public class EE_HunterBoss : EE_NPC 
{
	[Header("Boss Logic")]
	public Transform towerPointA;
	public Transform towerPointB;
	public Transform groundPoint;

	public float teleportCooldown = 3f;

	private float cooldownTimer;

	public bool shouldFallToGround = false;
	public bool shouldJumpToTower = false;

	private bool onGround = false;
	private bool togglePosition = false;

    // Start is called before the first frame update
    protected override void Start()
    {
    	base.Start(); 
		cooldownTimer = teleportCooldown;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

		cooldownTimer -= Time.deltaTime;

		if (cooldownTimer <= 0f) {
			if(shouldFallToGround) {
				TeleportTo(groundPoint);

				shouldFallToGround = false;
				onGround = true;

				base.useSensorForPath = true;
			}
			else if (shouldJumpToTower){
				TeleportTo(towerPointB);

				shouldJumpToTower = false;
				onGround = false;

				base.useSensorForPath = false;
			}
			else if (onGround) {

			}
			else if (!onGround) {
				TeleportTo(togglePosition ? towerPointA : towerPointB);
				togglePosition = !togglePosition;

				base.path = null;
			}

			cooldownTimer = teleportCooldown;
		}

    }

	void TeleportTo(Transform target) {
		if(target != null) {
			transform.position = target.position;
		}else {
			Debug.LogWarning("Teleport target is not assigned!");
		}
	}
}
