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

    [Header("Populations")]
    public int minPopulation = 30;
    public int maxPopulation = 50;

    [Header("Jobs (%)")]
    public List<JobPercentage> jobDistribution = new();

    [Header("Spawn Points (House, Hotel...)")]
    public List<Transform> spawnPoints = new();

    [Tooltip("Village Spawn Zone")]
    public Collider villageBorders; 


    public NPCIdentity CreateRandomNPC(string id)
    {
        Race race = listRaceLiving.GetRandomElementFromList();
        Gender gender = (Random.value > 0.5f) ? Gender.Male : Gender.Female;

        var (firstName, familyName) = WorldManager.instance.GetRandomName(race, gender);
        string religion = WorldManager.instance.GetRandomReligion();

        return new NPCIdentity {
            npcId = "npc_" + id + "_" + System.Guid.NewGuid().ToString("N").Substring(0, 8),
            firstName = firstName,
            familyName = familyName,
            race = race,
            gender = gender,
            religion = religion,
            jobs = PickRandomJob()
        };
    }

    public List<NPCIdentity> GenerateVillage()
    {
        List<NPCIdentity> population = new();
        int totalPopulation = Random.Range(minPopulation, maxPopulation + 1);

        for (int i = 0; i < totalPopulation; i++) {
            NPCIdentity npc = CreateRandomNPC($"{i:D3}");
            population.Add(npc);
        }
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