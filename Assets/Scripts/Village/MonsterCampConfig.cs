using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(BoxCollider))]
public class MonsterCampConfig : MonoBehaviour
{
    [Header("Camp Info")]
    public string campName = "CampName";
    public List<Race> listRaceLiving = new();
    public FactionSystem.Faction factionLiving;

    public int maxPopulation = 50;
    List<NPCIdentity> population = new();
    List<GameObject> population3DModel = new();
    
    [Header("Spawn Settings")]
    [SerializeField] private GameObject monsterPrefab;
    [SerializeField] private int maxMonsters = 5;
    [SerializeField] private int minMonsters = 2;
    [SerializeField] private float respawnCooldown = 60f;
    [SerializeField] private BoxCollider campBounds;

    [Header("Number of try to spawn units if fail")]
    // [SerializeField] private int numberOfTry = 10;

    private List<NPCIdentity> campMonsters = new();
    private Dictionary<string, GameObject> activeMonsterObjects = new();

    // private bool isPlayerInside = false;
    private float lastClearedTime = -1f;

    private WorldManager worldManagerInstance;

    void Reset()
    {
        campBounds = GetComponent<BoxCollider>();
        if (campBounds != null) campBounds.isTrigger = true;
    }

    void Start()
    {
        worldManagerInstance = WorldManager.instance;
        monsterPrefab = Resources.Load<GameObject>("Gobelin");
    }

    public List<NPCIdentity> InitializeCampData()
    {
        int totalMonsters = Random.Range(minMonsters, maxMonsters + 1);

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
                homeVillage = campName,
                currentVillage = campName,
                race = race,
                faction = FactionSystem.Faction.Monsters,
                position = GetRandomPointInCampBounds()
            };
            
            campMonsters.Add(monster);
        }
        return campMonsters;
    }

    #region Trigger Proximity Streaming (Spawn 3D)

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            CheckRespawnTimer();
            SpawnCamp3D();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) {
            DespawnCamp3D();
        }
    }

    #endregion

    #region Instanciation et Despawn

    public void SpawnCamp3D()
    {
        if (monsterPrefab == null)
            return;

        foreach (NPCIdentity monsterData in campMonsters) {
            if (!monsterData.isAlive)
                continue;

            if (activeMonsterObjects.ContainsKey(monsterData.npcId))
                continue;

            GameObject monsterInstance = Instantiate(monsterPrefab, monsterData.position, Quaternion.identity, transform);
            
            NPCControllers controller = monsterInstance.GetComponent<NPCControllers>();
            if (controller != null)
                controller.Initialize(monsterData);

            activeMonsterObjects.Add(monsterData.npcId, monsterInstance);
        }
    }

    public void DespawnCamp3D()
    {
        foreach (var kvp in activeMonsterObjects) {
            if (kvp.Value != null)
                Destroy(kvp.Value);
        }
        activeMonsterObjects.Clear();
    }

    #endregion

    #region Respawn & Vagues

    private void CheckRespawnTimer()
    {
        if (IsCampCleared() && lastClearedTime > 0 && Time.time >= lastClearedTime + respawnCooldown)
            RespawnAllMonsters();
    }

    public void OnMonsterDied(NPCIdentity monsterData)
    {
        monsterData.isAlive = false;

        if (IsCampCleared())
            lastClearedTime = Time.time;
            Debug.Log($"[MonsterCamp] Le camp {campName} a été nettoyé ! Respawnera dans {respawnCooldown}s.");
    }

    private bool IsCampCleared()
    {
        foreach (var monster in campMonsters)
            if (monster.isAlive)
                return false;

        return true;
    }

    private void RespawnAllMonsters()
    {
        foreach (var monster in campMonsters) {
            monster.isAlive = true;
            monster.position = GetRandomPointInCampBounds();
        }
        lastClearedTime = -1f;
        Debug.Log($"[MonsterCamp] Le camp {campName} est de nouveau réapparu !");
    }

    #endregion

    #region Helpers NavMesh

    private Vector3 GetRandomPointInCampBounds()
    {
        if (campBounds == null)
            return transform.position;

        Bounds bounds = campBounds.bounds;

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
