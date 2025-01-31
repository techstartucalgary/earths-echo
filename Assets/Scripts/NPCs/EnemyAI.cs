using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System;

public class EnemyAI : MonoBehaviour
{
    public GameObject target;

    [Header("Sensors")]
    [SerializeField] Sensor chaseSensor;
    [SerializeField] Sensor actionSensor;

    [Header("Movement")]
    public float speed = 200f;
    public float nextWaypointDistance = 3f;
    public bool useSensorForPath = true;

    [Header("Graphics")]
    public Transform enemyGFX;
    [SerializeField] Animator animator;
    float animatorXScale;

    Path path;
    int currentWaypoint = 0;
    bool reachedEndOfPath = false;

    Seeker seeker;
    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        chaseSensor.setTargetTag(target.tag);
        actionSensor.setTargetTag(target.tag);

        animatorXScale = enemyGFX.localScale[0];

        if (useSensorForPath) {
            chaseSensor.OnTargetChanged += CheckChaseSensor;
        }
        else {
            InvokeRepeating("UpdatePath", 0f, .5f);
        }
    }

    void CheckChaseSensor()
    {
        UpdatePath();
    }

    void UpdatePath()
    {
        if (seeker.IsDone()) {
            seeker.StartPath(rb.position, target.transform.position, OnPathComplete);
        }
    }

    void OnPathComplete(Path p)
    {
        if (!p.error) {
            path = p;
            currentWaypoint = 0;
        }
    }

    // FixedUpdate is called fixed number of times per second, ideal for physics stuff
    void Update()
    {
        if (path == null) return;

        if (currentWaypoint >= path.vectorPath.Count) {
            reachedEndOfPath = true;
            return;
        }
        else {
            reachedEndOfPath = false;
        }

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * speed * Time.deltaTime;

        rb.AddForce(force);

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if (distance < nextWaypointDistance) {
            currentWaypoint++;
        }

        // Animation 
        animator.SetFloat("xVelocity", Math.Abs(rb.velocity.x));
        if (force.x >= 0.01f) {
            enemyGFX.localScale = new Vector3(animatorXScale, enemyGFX.localScale[1], enemyGFX.localScale[2]);
        }
        else if (force.x <= -0.01f) {
            enemyGFX.localScale = new Vector3(-animatorXScale, enemyGFX.localScale[1], enemyGFX.localScale[2]);
        }
    }
}
