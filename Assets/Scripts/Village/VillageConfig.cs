using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public struct JobPercentage
{
    public Jobs job;
    [Range(0, 100)] public float percentage;
}

public class VillageConfig : MonoBehaviour
{
    [Header("Village Info")]
    public string villageName = "VillageName";
    public List<Race> listRaceLiving = new();
    public FactionSystem.Faction factionLiving;

    [Tooltip("Religion name must be the same as in 'WorldData.json' !!!")]
    public List<string> listReligion = new();

    [Header("Populations")]
    public int minPopulation = 30;
    public int maxPopulation = 50;
    List<NPCIdentity> population = new();
    List<GameObject> populationPrefab = new();

    [Header("Jobs (%)")]
    public List<JobPercentage> jobDistribution = new();

    [Header("Spawn Points (House, Hotel...)")]
    public List<Transform> spawnPoints = new();

    [Tooltip("Village Spawn Zone")]
    private BoxCollider villageBorders;

    [Header("Number of try to spawn units if fail")]
    [SerializeField] private int numberOfTry = 10;

    private GameObject npcPrefab;

    private WorldManager worldManagerInstance;

    void Awake()
    {
        villageBorders = GetComponent<BoxCollider>();
        npcPrefab = Resources.Load<GameObject>("NPC");
    }

    void Start()
    {
        worldManagerInstance = WorldManager.instance;
    }

    /// <summary>
    /// Called by WorldManager/InitializeNewGameWorld
    /// </summary>
    public List<NPCIdentity> GenerateVillage()
    {
        int totalPopulation = Random.Range(minPopulation, maxPopulation + 1);

        for (int i = 0; i < totalPopulation; i++) {
            NPCIdentity npc = CreateRandomNPC($"{i:D4}");
            population.Add(npc);
        }
        Debug.Log($"{population.Count} villageois ont été crée dans {villageName}.");

        InstantiateAllNPCinVillage();

        return population;
    }

    /// <summary>
    /// Create NPC with : ID, firstName, FamilyName, Race, Gender, Religion and Job
    /// </summary>
    public NPCIdentity CreateRandomNPC(string id)
    {
        Race race = listRaceLiving.GetRandomElementFromList();
        Gender gender = (Random.value > 0.5f) ? Gender.Male : Gender.Female;

        var (firstName, familyName) = worldManagerInstance.GetRandomName(race, gender);

        string religion;
        if (listReligion == null)
            religion = worldManagerInstance.GetRandomReligion();
        else
            religion = listReligion.GetRandomElementFromList();

        Jobs jobs = PickRandomJob();

        return new NPCIdentity {
            npcId = "npc_" + id + "_" + System.Guid.NewGuid().ToString("N")[..8],
            firstName = firstName,
            familyName = familyName,
            race = race,
            faction = factionLiving,
            gender = gender,
            religion = religion,
            jobs = jobs
        };
    }

    private Jobs PickRandomJob()
    {
        float randomValue = Random.Range(0f, 100f);
        float cumulative = 0f;

        foreach (var jobConfig in jobDistribution) {
            cumulative += jobConfig.percentage;

            if (randomValue <= cumulative)
                return jobConfig.job;
        }

        return Jobs.Soldier;
    }

    private void FormFamiliesAndCouples(List<NPCIdentity> population)
    {
        
    }

    private void AssignJobsAndClasses(List<NPCIdentity> population)
    {
        
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

    public void InstantiateAllNPCinVillage()
    {
        foreach (NPCIdentity nPC in population) {
            nPC.position = GetRandomPointInBoxCollider(villageBorders);

            GameObject npcInstance = Instantiate(npcPrefab, nPC.position, Quaternion.identity);

            npcInstance.GetComponentInChildren<NPCControllers>().Initialize(nPC);
            populationPrefab.Add(npcInstance);
        }
    }

    public void DeleteAllNPCinVillage()
    {
        foreach (GameObject npcInstance in populationPrefab) {
            Destroy(npcInstance);
        }
    }
}
