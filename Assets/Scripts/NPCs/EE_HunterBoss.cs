using UnityEngine;

public class EE_HunterBoss : EE_NPC 
{
	bool shouldJump = false;

	public Transform phase1PointA;
	public Transform phase1PointB;
	public Transform phase2Point;

	public float teleportCooldown = 3f;

	private float cooldownTimer;

	public bool isInPhase2 = false;
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
			if(isInPhase2) {
				TeleportTo(phase2Point);
			}else {
				TeleportTo(togglePosition ? phase1PointA : phase1PointB);
				togglePosition = !togglePosition;
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
