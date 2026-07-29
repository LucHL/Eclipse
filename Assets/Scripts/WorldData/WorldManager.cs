using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WorldManager : MonoBehaviour, IDataPersistence
{
    public static WorldManager instance;

    [Header("Globale DB")]
    public RootDatabase database;

    [Header("List Village & NPC")]
    private Dictionary<string, List<NPCIdentity>> villagesAndNPCList = new();

    public void LoadData(GameData data)
    {
        
    }

    public void SaveData(GameData data)
    {
        
    }

    private void Awake()
    {
        if (instance != null) {
            Destroy(gameObject);
            return;
        }

        instance = this;
        LoadDatabase();
    }

    void Start()
    {
        InitializeNewGameWorld();
    }

    public void InitializeNewGameWorld()
    {
        GameObject villagesParent = GameObject.Find("VILLAGES");
        
        if (villagesParent != null) {
            VillageConfig[] allVillages = villagesParent.GetComponentsInChildren<VillageConfig>();

            foreach (VillageConfig village in allVillages) {
                villagesAndNPCList.Add(village.villageName, village.GenerateVillage());
            }

            Debug.Log($"[WorldManager] {allVillages.Length} villages ont été initialisés dans le monde !");
        }
    }

    private void LoadDatabase()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("WorldData");

        if (jsonFile != null) {
            database = JsonUtility.FromJson<RootDatabase>(jsonFile.text);
            Debug.Log("[WorldManager] Base de données JSON statique chargée avec succès !");
        } else
            Debug.LogError("[WorldManager] Impossible de trouver 'Resources/WorldData.json'");
    }

    // --- Get random NPC name & family name ---

    public (string firstName, string familyName) GetRandomName(Race race, Gender gender)
    {
        string raceKey = race.ToString();

        NamePoolGroup pool = database.NamePool.GetPoolForRace(race);

        if (pool == null)
            return ("Inconnu", "");

        List<string> firstNamePool = (gender == Gender.Male) ? pool.MaleFirstNames : pool.FemaleFirstNames;
        
        string firstName = firstNamePool.GetRandomElementFromList();
        string familyName = pool.FamilyNames.GetRandomElementFromList();

        return (firstName, familyName);
    }

    public string GetRandomReligion()
    {
        return database.Religions.Name.GetRandomElementFromList();
    }

    public List<NPCIdentity> GetNPCsInVillage(string villageId)
    {
        villagesAndNPCList.TryGetValue(villageId, out List<NPCIdentity> result);
        return result;
    }
}
