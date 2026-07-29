using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NPCControllers : MonoBehaviour
{
    [Header("NPC Info")]
    [SerializeField] private NPCIdentity identity;

    [Header("Wander Settings")]
    [SerializeField] private float wanderRadius = 15f;
    [SerializeField] private float minWaitTime = 2f;
    [SerializeField] private float maxWaitTime = 5f;

    private NavMeshAgent agent;
    private Animator animator;
    private float timer;
    private float currentWaitTime;
    private bool isDead = false;

    public NPCIdentity Identity => identity;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
    }

    /// <summary>
    /// Called by VillageConfig/WorldManager
    /// </summary>
    public void Initialize(NPCIdentity npcIdentity)
    {
        identity = npcIdentity;
        isDead = !identity.isAlive;

        if (isDead) {
            Die();
            return;
        }

        if (agent != null) {
            agent.Warp(identity.position);
            agent.enabled = true;
        }

        SetNewDestination();
    }

    void Update()
    {
        if (isDead || identity == null)
            return;

        identity.position = transform.position;

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance) {
            timer += Time.deltaTime;
            if (timer >= currentWaitTime) {
                SetNewDestination();
                timer = 0f;
            }
        }

        if (animator != null && animator.enabled)
            animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    private void SetNewDestination()
    {
        if (agent == null || !agent.isOnNavMesh)
            return;

        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection += transform.position;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, wanderRadius, NavMesh.AllAreas))
            agent.SetDestination(hit.position);

        currentWaitTime = Random.Range(minWaitTime, maxWaitTime);
    }

    public void Die()
    {
        isDead = true;

        if (identity != null)
            identity.isAlive = false;

        if (agent != null && agent.enabled)
            agent.enabled = false;

        if (animator != null)
            animator.SetTrigger("Die");

        enabled = false;
    }

    #region Optimisation Camera (Culling)

    private void OnBecameInvisible()
    {
        if (animator != null)
            animator.enabled = false;
    }

    private void OnBecameVisible()
    {
        if (animator != null) animator.enabled = true;
    }

    #endregion
}
