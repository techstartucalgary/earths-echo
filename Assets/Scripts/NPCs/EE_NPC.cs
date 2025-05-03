using System;
using Pathfinding;
using UnityEngine;
using System.Collections;

public class EE_NPC : MonoBehaviour
{
    public Transform target;

    [Header("Sensors")]
    [SerializeField]
    protected Sensor targetSensor;

    [Header("Movement")]
    public float speed = 200f;
    public float nextWaypointDistance = 3f;
    public bool useSensorForPath = true;

    [Header("Graphics")]
    public Transform npcGFX;

    [SerializeField]
	public Animator animator;
    protected float animatorXScale;

    protected Path path;
    int currentWaypoint = 0;
    bool reachedEndOfPath = false;

    Seeker seeker;
    protected Rigidbody2D rb;

    protected virtual void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        animatorXScale = npcGFX.localScale[0];

        if (target != null)
        {
            targetSensor.setTargetTag(target.tag);

            if (useSensorForPath)
            {
                targetSensor.OnTargetChanged += CheckTargetSensor;
            }
            else
            {
                InvokeRepeating("UpdatePath", 0f, .5f);
            }
        }
        else
        {
            InvokeRepeating("UpdatePath", 0f, .5f);
        }
    }

    void CheckTargetSensor()
    {
        UpdatePath();
    }

        private IEnumerator LookForTargetRoutine()
    {
        while (target == null)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null)
            {
                target = p.transform;
                InitialiseForTarget(target);
                yield break;
            }
            yield return new WaitForSeconds(1f);
        }
    }

    private void InitialiseForTarget(Transform newTarget)
    {
        if (newTarget == null) return;

        if (targetSensor != null)
        {
            targetSensor.setTargetTag(newTarget.tag);
            if (useSensorForPath) targetSensor.OnTargetChanged += UpdatePath;
        }

        InvokeRepeating(nameof(UpdatePath), 0f, .5f);
    }

    protected void UpdatePath()
    {
        if (target == null) return;          // still no player – do nothing
        if (!seeker.IsDone()) return;        // path request already pending

        seeker.StartPath(rb.position, target.position, OnPathComplete);
    }

    private void OnPathComplete(Path p)
    {
        if (p.error) return;

        path            = p;
        currentWaypoint = 0;
        reachedEndOfPath = false;
    }

    protected virtual void Update()
    {
        if (path == null)
            return;

        if (currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            return;
        }
        else
        {
            reachedEndOfPath = false;
        }

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * speed * Time.deltaTime;

        rb.AddForce(force);

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }

        // Animation
        animator.SetFloat("xVelocity", Math.Abs(rb.velocity.x));
        if (force.x >= 0.01f)
        {
            npcGFX.localScale = new Vector3(
                animatorXScale,
                npcGFX.localScale[1],
                npcGFX.localScale[2]
            );
        }
        else if (force.x <= -0.01f)
        {
            npcGFX.localScale = new Vector3(
                -animatorXScale,
                npcGFX.localScale[1],
                npcGFX.localScale[2]
            );
        }
    }
}
