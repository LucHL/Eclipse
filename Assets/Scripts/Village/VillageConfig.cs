using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct JobPercentage
{
    public Jobs job;
    [Range(0, 100)] public float percentage;
}

public class VillageConfig : MonoBehaviour
{
    [Header("Village Info")]
    public string villageName = "villageName";
    public List<Race> listRaceLiving = new();

    [Tooltip("Religion name must be the same as in 'WorldData.json' !!!")]
    public List<string> listReligion = new();

    [Header("Populations")]
    public int minPopulation = 30;
    public int maxPopulation = 50;
    List<NPCIdentity> population = new();

    [Header("Jobs (%)")]
    public List<JobPercentage> jobDistribution = new();

    [Header("Spawn Points (House, Hotel...)")]
    public List<Transform> spawnPoints = new();

    [Tooltip("Village Spawn Zone")]
    public Collider villageBorders;

    private WorldManager worldManagerInstance;

    void Start()
    {
        worldManagerInstance = WorldManager.instance;
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
            gender = gender,
            religion = religion,
            jobs = jobs
        };
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
        return population;
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