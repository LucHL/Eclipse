using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NPCControllers : MonoBehaviour, IDataPersistence
{
    [Header("NPC Info")]
    [SerializeField] private NPCIdentity identity;

    [Header("Wander Settings")]
    [SerializeField] private float wanderRadius = 15f;
    [SerializeField] private float minWaitTime = 2f;
    [SerializeField] private float maxWaitTime = 5f;

    private NavMeshAgent agent;
    private float timer;
    private float currentWaitTime;
    private bool isDead = false;

    public void SaveData(GameData data)
    {
        
    }

    public void LoadData(GameData data)
    {
        
    }

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        SetNewDestination();
    }

    void Update()
    {
        if (isDead) {
            // gameObject.SetActive(false);
            return;
        }

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance) {
            timer += Time.deltaTime;
            if (timer >= currentWaitTime) {
                SetNewDestination();
                timer = 0f;
            }
        }
    }

    private void SetNewDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection += transform.position;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, wanderRadius, NavMesh.AllAreas))
            agent.SetDestination(hit.position);

        currentWaitTime = Random.Range(minWaitTime, maxWaitTime);
    }

    public void Die()
    {
        if (isDead)
            return;

        isDead = true;

        if (agent != null)
            agent.enabled = false;

        enabled = false;
    }
}
