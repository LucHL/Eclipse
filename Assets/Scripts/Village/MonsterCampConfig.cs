using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(BoxCollider))]
public class MonsterCampConfig : ZoneConfig
{
    [Header("Respawn Condition")]
    [SerializeField] private float respawnCooldown = 60f;
    private float lastClearedTime = -1f;

    [Header("Respawn Condition")]
    [SerializeField] private string folderPrefab;

    protected override void Awake()
    {
        base.Awake();

        GameObject[] prefab = Resources.LoadAll<GameObject>($"Monster/{folderPrefab}");
        foreach (GameObject gameObject in prefab)
            populationPrefab.Add(gameObject);
    }

    public override List<NPCIdentity> InitializeZoneData()
    {
        int totalMonsters = Random.Range(minPopulation, maxPopulation + 1);

        for (int i = 0; i < totalMonsters; i++) {
            Race race = listRaceLiving.GetRandomElementFromList();
            Gender gender = (Random.value > 0.5f) ? Gender.Male : Gender.Female;

            var (firstName, familyName) = worldManagerInstance.GetRandomName(race, gender);

            NPCIdentity monster = new()
            {
                npcId = $"npc_{i:D4}_{System.Guid.NewGuid().ToString("N")[..8]}",
                firstName = firstName,
                familyName = familyName,
                gender = gender,
                homeVillage = zoneName,
                currentVillage = zoneName,
                race = race,
                faction = FactionSystem.Faction.Monsters,
                position = GetRandomPointInCampBounds()
            };
            
            population.Add(monster);
        }
        return population;
    }

    #region Trigger Proximity Streaming (Spawn 3D)

    protected override void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            CheckRespawnTimer();
            InstantiateAllEntityInZone();
        }
    }

    protected override void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) {
            DestroyAllEntityInZone();
        }
    }

    #endregion

    #region Respawn & Vagues

    private void CheckRespawnTimer()
    {
        if (IsCampCleared() && lastClearedTime > 0 && Time.time >= lastClearedTime + respawnCooldown)
            InstantiateAllEntityInZone();
    }

    public void OnMonsterDied(NPCIdentity monsterData)
    {
        monsterData.isAlive = false;

        if (IsCampCleared())
            lastClearedTime = Time.time;
            Debug.Log($"[MonsterCamp] Le camp {zoneName} a été nettoyé ! Respawnera dans {respawnCooldown}s.");
    }

    private bool IsCampCleared()
    {
        foreach (var monster in population)
            if (monster.isAlive)
                return false;

        return true;
    }

    protected override void InstantiateAllEntityInZone()
    {
        base.InstantiateAllEntityInZone();

        foreach (var monster in population) {
            monster.isAlive = true;
            monster.position = GetRandomPointInCampBounds();
        }
        lastClearedTime = -1f;
        Debug.Log($"[MonsterCamp] Le camp {zoneName} est de nouveau réapparu !");
    }

    #endregion

    #region Helpers NavMesh

    private Vector3 GetRandomPointInCampBounds()
    {
        if (zoneBorders == null)
            return transform.position;

        Bounds bounds = zoneBorders.bounds;

        for (int i = 0; i < 10; i++) {
            float randomX = Random.Range(bounds.min.x, bounds.max.x);
            float randomZ = Random.Range(bounds.min.z, bounds.max.z);
            Vector3 searchOrigin = new Vector3(randomX, bounds.center.y, randomZ);

            if (NavMesh.SamplePosition(searchOrigin, out NavMeshHit hit, 20f, NavMesh.AllAreas))
                return hit.position;
        }

        return transform.position;
    }

    #endregion
}
