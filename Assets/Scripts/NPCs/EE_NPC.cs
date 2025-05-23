using System;
using Pathfinding;
using UnityEngine;

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

    void UpdatePath()
    {
        if (seeker.IsDone())
        {
            seeker.StartPath(rb.position, target.transform.position, OnPathComplete);
        }
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
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
