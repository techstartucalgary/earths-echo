using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Analytics;

public interface IActionStrategy {
    bool CanPerform { get; }
    bool Complete { get; }

    void Start() {

    }

    void Update(float deltaTime) {

    }

    void Stop() {
        
    }
}

public class WanderStrategy : IActionStrategy {
    readonly NavMeshAgent agent;
    readonly float wanderRadius;

    public bool CanPerform => !Complete;
    public bool Complete => agent.remainingDistance <= 2f && !agent.pathPending;

    public WanderStrategy(NavMeshAgent agent, float wanderRadius) {
        this.agent = agent;
        this.wanderRadius = wanderRadius;
    }

    public void Start() {
        for (int i = 0; i < 5; i++) {
            Vector2 randomDirection = (UnityEngine.Random.insideUnitCircle * wanderRadius).With(y: 0);

            // TODO: if path can be generated, set destination
            return;
        }
    }
}

public class IdleStrategy : IActionStrategy {
    public bool CanPerform => true; // Agent can always idle
    public bool Complete { get; private set; }

    readonly CountdownTimer timer;

    public IdleStrategy(float duration) {
        timer = new CountdownTimer(duration);
        timer.OnTimerStart += () => Complete = false;
        timer.OnTimerStop += () => Complete = true;
    }

    public void Start() => timer.Start();
    public void Update(float deltaTime) => timer.Tick(deltaTime);
}