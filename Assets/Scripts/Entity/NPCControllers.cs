using UnityEngine;
using UnityEngine.AI;

public class NPCControllers : EntityController
{
    [Header("Wander Settings")]
    [SerializeField] private float wanderRadius = 15f;
    [SerializeField] private float minWaitTime = 2f;
    [SerializeField] private float maxWaitTime = 5f;

    private float timer;
    private float currentWaitTime;

    /// <summary>
    /// Called by VillageConfig
    /// </summary>
    public override void Initialize(NPCIdentity npcIdentity)
    {
        base.Initialize(npcIdentity);
        
        SetNewRandomDestination();
    }

    protected override void Update()
    {
        base.Update();

        if (isDead || identity == null)
            return;

        identity.position = transform.position;

        if (agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh) {
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance) {
                timer += Time.deltaTime;
                if (timer >= currentWaitTime) {
                    SetNewRandomDestination();
                    timer = 0f;
                }
            }
        }
    }

    private void SetNewRandomDestination()
    {
        if (agent == null || !agent.isOnNavMesh)
            return;

        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection += transform.position;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, wanderRadius, NavMesh.AllAreas))
            agent.SetDestination(hit.position);

        currentWaitTime = Random.Range(minWaitTime, maxWaitTime);
    }
}
