using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshAgentUtil : MonoBehaviour
{
    [HideInInspector] public NavMeshAgent agent;

    private bool isDestination = false;
    
    public Vector3 velocity
    {
        get => agent.velocity;
        set => agent.velocity = value;
    }

    public float radius
    {
        get => agent.radius;
        set => agent.radius = value;
    }
    
    public float stoppingDistance
    {
        get => agent.stoppingDistance;
        set => agent.stoppingDistance = value;
    }

    public int avoidancePriority
    {
        get => agent.avoidancePriority;
        set => agent.avoidancePriority = value;
    }

    public bool isStopped
    {
        get => agent.isStopped;
        set => agent.isStopped = value;
    }

    public bool IsDestination
    {
        get => isDestination;
        private set => isDestination = value;
    }
    public bool pathPending => agent.pathPending;

    public static implicit operator NavMeshAgent(NavMeshAgentUtil value) => value.agent;
    
    public void Awake()
    {
        agent = GetComponentInChildren<NavMeshAgent>();
    }

    public void OnEnable()
    {
        agent.enabled = true;
    }

    public void OnDisable()
    {
        agent.enabled = false;
    }

    public void SetDestination(Vector3 target, Action moveDoneEvent = null) 
    {
        StopAllCoroutines();
        agent.SetDestination(target);
        StartCoroutine(MoveDoneEnumerator(moveDoneEvent));
    }

    public void SetPath(Vector3[] pathPointList, Action moveDoneEvent = null)
    {
        StopAllCoroutines();
        StartCoroutine(MoveEnumerator(pathPointList, moveDoneEvent));
    }

    private IEnumerator MoveEnumerator(Vector3[] pathPointList, Action moveDoneEvent)
    {
        IsDestination = false;
        foreach (var dest in pathPointList)
        {
            agent.SetDestination(dest);
            while (agent.pathPending)
                yield return null;
            while (agent.remainingDistance > agent.stoppingDistance)
                yield return null;
        }
        IsDestination = true;
        moveDoneEvent?.Invoke();
    }

    private IEnumerator MoveDoneEnumerator(Action moveDoneEvent)
    {
        IsDestination = false;
        while (agent.pathPending)
            yield return null;
        while (agent.remainingDistance > agent.stoppingDistance)
            yield return null;
        IsDestination = true;
        moveDoneEvent?.Invoke();
    }
}

