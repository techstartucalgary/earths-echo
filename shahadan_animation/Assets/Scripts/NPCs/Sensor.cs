using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class Sensor : MonoBehaviour
{
    [SerializeField] float detectionRadius = 5f;
    [SerializeField] float timerInterval = 1f;

    CircleCollider2D detectionRange;

    string targetTag = "";

    public event Action OnTargetChanged = delegate { };
    
    public Vector3 TargetPosition => target ? target.transform.position : Vector3.zero;
    public bool IsTargetInRange => TargetPosition != Vector3.zero;

    GameObject target;
    Vector3 lastKnownPosition;
    CountdownTimer timer;

    // Awake is called before Start
    void Awake() {
        detectionRange = GetComponent<CircleCollider2D>();
        detectionRange.isTrigger = true;
        detectionRange.radius = detectionRadius;
    }

    void Start() {
        timer = new CountdownTimer(timerInterval);
        timer.OnTimerStop += () => {
            UpdateTargetPosition(target.OrNull());
            timer.Start();
        };
        timer.Start();
    }

    void Update() {
        timer.Tick(Time.deltaTime);
    }

    void UpdateTargetPosition(GameObject target = null) {
        this.target = target;
        if(IsTargetInRange && (lastKnownPosition != TargetPosition || lastKnownPosition != Vector3.zero)) {
            lastKnownPosition = TargetPosition;
            OnTargetChanged.Invoke();
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
		if(targetTag == "") return;
        if(!other.CompareTag(targetTag)) return;
        UpdateTargetPosition(other.gameObject);
    }

    void OnTriggerExit2D(Collider2D other) {
		if(targetTag == "") return;
        if(!other.CompareTag(targetTag)) return;
        UpdateTargetPosition();
    }

    public void setTargetTag(string tag) {
        targetTag = tag;
    }

    void OnDrawGizmos() {
        Gizmos.color = IsTargetInRange ? Color.red : Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
