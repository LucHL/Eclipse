using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("File Storage Config")]
    [SerializeField] private string fileName;
    [SerializeField] private bool useEncryption;

    private GameData gameData; // TODO faire des fichier de sauvegarde selon les besoins ex: village_Thann avec la list des npc
    private List<IDataPersistence> dataPersistenceObjects;
    private FileDataHandler dataHandler;

    public static DataPersistenceManager instance;

    void Awake()
    {
        if (instance != null) {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);
    }

    public void OnSceneLoad(Scene scene, LoadSceneMode loadSceneMode)
    {
        dataPersistenceObjects = FindAllDataPersistenceInScene();
        LoadGame();
    }

    public void NewGame()
    {
        gameData = new();
    }

    public void LoadGame()
    {
        gameData = dataHandler.Load();
    }

    public void SaveGame()
    {
        foreach (IDataPersistence dataPersistence in dataPersistenceObjects)
            dataPersistence.SaveData(gameData);
        
        dataHandler.Save(gameData);
    }

    // void OnApplicationQuit()
    // {
    //     SaveGame();
    // }

    List<IDataPersistence> FindAllDataPersistenceInScene()
    {
        IEnumerable<IDataPersistence> dataPersistences = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistences);
    }

    public bool HasGameData()
    {
        return (gameData != null);
    }
}
