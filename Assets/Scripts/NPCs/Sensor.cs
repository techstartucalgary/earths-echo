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
        if(!other.CompareTag("Player")) return;
        Debug.Log("Player entered trigger");
        UpdateTargetPosition(other.gameObject);
    }

    void OnTriggerExit2D(Collider2D other) {
        if(!other.CompareTag("Player")) return;
        Debug.Log("Player exited trigger");
        UpdateTargetPosition();
    }

    void OnDrawGizmos() {
        Gizmos.color = IsTargetInRange ? Color.red : Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
