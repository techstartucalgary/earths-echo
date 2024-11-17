using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System;

[RequireComponent(typeof(Seeker))]
public class SeekerAgent : MonoBehaviour {
    [Header("Movement")]
    public float speed = 200f;
    public float nextWaypointDistance = 3f;

    [Header("Graphics")]
    public Transform agentGFX;

    Vector3 destination;
    Path path;
    Path testPath;
    int currentWaypoint = 0;
    bool reachedEndOfPath = false;

    Seeker seeker;
    Rigidbody2D rb;

    public bool HasPath => path != null && !reachedEndOfPath;

    public bool IsDone => seeker.IsDone();

    public float PathTotalLength => path != null ? path.GetTotalLength() : Single.PositiveInfinity;

    public float TestPathTotalLength => testPath != null ? testPath.GetTotalLength() : Single.PositiveInfinity;

    public float RemainingDistance() {
        if (path == null) {
            return Single.PositiveInfinity;
        }

        float totalRemainingDistance = 0;

        for (int i = currentWaypoint; i < path.vectorPath.Count - 2; i ++) {
            totalRemainingDistance += Vector2.Distance(path.vectorPath[i], path.vectorPath[i + 1]);
        }

        return totalRemainingDistance;
    }

    public void UpdateTestPath(Vector3 testDestination) {
        if (seeker.IsDone()) {
            seeker.StartPath(rb.position, testDestination, OnTestPathComplete);
        }
    }

    public void setDestination(Vector3 dest) {
        destination = dest;
        UpdatePath();
    }

    public void ResetPath() {
        destination = Vector3.zero;
        path = null;
    }

    void OnTestPathComplete(Path p) {
        if (!p.error) {
            testPath = p;
        }
    }

    void UpdatePath()
    {
        if (seeker.IsDone() && destination != Vector3.zero) {
            seeker.StartPath(rb.position, destination, OnPathComplete);
        }
    }
    
    void OnPathComplete(Path p) {
        if (!p.error) {
            path = p;
            currentWaypoint = 0;
        }
    }

    void Start() {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        
        InvokeRepeating("UpdatePath", 0f, .5f);
    }

    void Update() {
        if (path == null) return;

        if (currentWaypoint >= path.vectorPath.Count) {
            reachedEndOfPath = true;
            path = null;
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

        if (force.x >= 0.01f) {
            agentGFX.localScale = new Vector3(-1f, 1f, 1f);
        }
        else if (force.x <= -0.01f) {
            agentGFX.localScale = new Vector3(1f, 1f, 1f);
        }
    }
}