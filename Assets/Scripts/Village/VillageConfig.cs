using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public struct JobPercentage
{
    public Jobs job;
    [Range(0, 100)] public float percentage;
}

public class VillageConfig : ZoneConfig
{
    [Tooltip("Religion name must be the same as in 'WorldData.json' !!!")]
    public List<string> listReligion = new();

    [Header("Jobs (%)")]
    public List<JobPercentage> jobDistribution = new();

    [Header("Spawn Points (House, Hotel...)")]
    public List<Transform> spawnPoints = new();

    [Tooltip("Village Spawn Zone")]
    private BoxCollider villageBorders;

    protected override void Awake()
    {
        base.Awake();

        GameObject[] prefab = Resources.LoadAll<GameObject>("NPC");
        foreach (GameObject gameObject in prefab)
            populationPrefab.Add(gameObject);
    }

    /// <summary>
    /// Called by WorldManager/InitializeNewGameWorld
    /// </summary>
    public override List<NPCIdentity> InitializeZoneData()
    {
        int totalPopulation = Random.Range(minPopulation, maxPopulation + 1);

        for (int i = 0; i < totalPopulation; i++) {
            NPCIdentity npc = CreateRandomEntity($"{i:D4}");
            population.Add(npc);
        }
        Debug.Log($"{population.Count} villageois ont été crée dans {zoneName}.");

        InstantiateAllEntityInZone();

        return population;
    }

    /// <summary>
    /// Create NPC with : ID, firstName, FamilyName, Race, Gender, Religion and Job
    /// </summary>
    protected override NPCIdentity CreateRandomEntity(string id)
    {
        Race race = listRaceLiving.GetRandomElementFromList();
        Gender gender = (Random.value > 0.5f) ? Gender.Male : Gender.Female;

        var (firstName, familyName) = worldManagerInstance.GetRandomName(race, gender);

        string religion;
        if (listReligion == null)
            listReligion = new(worldManagerInstance.database.Religions.Name);

        religion = listReligion.GetRandomElementFromList();

        Jobs jobs = PickRandomJob();

        return new NPCIdentity {
            npcId = "npc_" + id + "_" + System.Guid.NewGuid().ToString("N")[..8],
            firstName = firstName,
            familyName = familyName,
            race = race,
            faction = zoneFaction,
            gender = gender,
            religion = religion,
            jobs = jobs,
            homeVillage = zoneName,
            currentVillage = zoneName,
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
}
