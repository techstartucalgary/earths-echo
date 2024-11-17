using System;
using Pathfinding.Ionic.Zip;
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
    readonly SeekerAgent agent;
    readonly float wanderRadius;

    public bool CanPerform => !Complete;
    public bool Complete => agent.RemainingDistance() <= 1f && agent.IsDone;

    public WanderStrategy(SeekerAgent agent, float wanderRadius) {
        this.agent = agent;
        this.wanderRadius = wanderRadius;
    }

    public void Start() {

        for (int i = 0; i < 5; i++) {
            Vector3 randomDirection = (UnityEngine.Random.insideUnitCircle * wanderRadius).With(y: 0);
            Vector3 testDestination = agent.transform.position + randomDirection;
            agent.UpdateTestPath(testDestination);

            // TODO change logic to wait for path to finish calculating, then test length

            if (agent.TestPathTotalLength <= wanderRadius) {
                agent.setDestination(testDestination);
                return;
            }
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