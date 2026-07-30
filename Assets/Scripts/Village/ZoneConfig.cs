using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

[RequireComponent(typeof(BoxCollider))]
public abstract class ZoneConfig : MonoBehaviour
{
    [Header("Zone info")]
    [SerializeField] public string zoneName = "Zone_01";
    [SerializeField] protected FactionSystem.Faction zoneFaction;
    [SerializeField] protected List<Race> listRaceLiving = new();

    [Header("Populations")]
    [SerializeField] protected int minPopulation = 2;
    [SerializeField] protected int maxPopulation = 5;
    [SerializeField] protected List<GameObject> populationPrefab = new();
    protected List<NPCIdentity> population = new();

    [Header("Spawn Settings")]
    [SerializeField] protected List<Transform> explicitSpawnPoints = new();
    [SerializeField] protected int numberOfTry = 10;

    [Tooltip("Spawn Zone")]
    protected BoxCollider zoneBorders;

    // Conteneurs de données universels (réutilisent NPCIdentity)
    protected List<NPCIdentity> populationData = new();
    protected Dictionary<string, GameObject> activeEntities3D = new();

    protected WorldManager worldManagerInstance;
    protected bool isPlayerInside = false;

    protected virtual void Reset()
    {
        zoneBorders = GetComponent<BoxCollider>();
        if (zoneBorders != null) zoneBorders.isTrigger = true;
    }

    protected virtual void Awake()
    {
        zoneBorders = GetComponent<BoxCollider>();
        worldManagerInstance = FindFirstObjectByType<WorldManager>();
    }

    public abstract List<NPCIdentity> InitializeZoneData();

    protected virtual NPCIdentity CreateRandomEntity(string id)
    {
        return null;
    }

    #region Trigger Proximity

    protected virtual void OnTriggerEnter(Collider other) { }
    protected virtual void OnTriggerExit(Collider other) { }
    protected virtual void OnPlayerEnteredZone() { }
    protected virtual void OnPlayerExitedZone() { }

    #endregion

    #region Spawn / Despawn 3D Prefab

    protected virtual void InstantiateAllEntityInZone()
    {
        Transform npcContainer = transform.Find("NPC");

        foreach (NPCIdentity nPC in population) {
            nPC.position = GetRandomPointInBoxCollider(zoneBorders);

            GameObject npcInstance = Instantiate(populationPrefab.GetRandomElementFromList(), nPC.position, Quaternion.identity, npcContainer);

            Vector3 parentScale = npcContainer.lossyScale;
            npcInstance.transform.localScale = new(1f / parentScale.x, 1f / parentScale.y, 1f / parentScale.z);

            npcInstance.GetComponentInChildren<NPCControllers>().Initialize(nPC);
            populationPrefab.Add(npcInstance);
        }
    }

    protected virtual void DestroyAllEntityInZone()
    {
        foreach (GameObject npcInstance in populationPrefab) {
            Destroy(npcInstance);
        }
    }

    private Vector3 GetRandomPointInBoxCollider(BoxCollider box)
    {
        if (box == null)
        {
            return GetValidNavMeshPoint(transform.position);
        }

        Bounds bounds = box.bounds;

        for (int i = 0; i < numberOfTry; i++) {
            float randomX = Random.Range(bounds.min.x, bounds.max.x);
            float randomZ = Random.Range(bounds.min.z, bounds.max.z);

            Vector3 searchOrigin = new(randomX, bounds.center.y, randomZ);

            if (NavMesh.SamplePosition(searchOrigin, out NavMeshHit hit, 20f, NavMesh.AllAreas))
                return hit.position;
        }
        Debug.LogWarning($"[VillageConfig] Position aléatoire introuvable dans la zone de {gameObject.name}. Fallback sur le centre du village.");
        return GetValidNavMeshPoint(transform.position);
    }

    private Vector3 GetValidNavMeshPoint(Vector3 origin)
    {
        if (NavMesh.SamplePosition(origin, out NavMeshHit hit, 50f, NavMesh.AllAreas))
            return hit.position;

        Debug.LogError($"[VillageConfig] CRITIQUE: Aucun NavMesh détecté sous le village {gameObject.name} à la position {origin} ! Check NavMesh Bake.");
        return origin;
    }

    #endregion

    #region Utilitaires NavMesh & Points d'apparition

    protected Vector3 GetValidSpawnPoint()
    {
        return new Vector3(0, 0, 0);
    }

    protected Vector3 GetRandomPointInZoneBounds()
    {
        return new Vector3(0, 0, 0);
    }

    #endregion
}
