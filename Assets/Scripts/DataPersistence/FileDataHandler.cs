using UnityEngine;
using System;
using System.IO;

public class FileDataHandler
{
    private string dataDirPath = "";
    private string dataFileName = "";
    private bool useEncryption = false;
    private readonly string encryptionCodeWord = "vraxx";

    public FileDataHandler(string dataDirPath, string dataFileName, bool useEncryption)
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
        this.useEncryption = useEncryption;
    }

    public GameData Load()
    {
        return null;
    }

    public void Save(GameData data)
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);

        try {
            Directory.CreateDirectory(fullPath);

            string dataToStore = JsonUtility.ToJson(data);

            if (useEncryption) {
                dataToStore = EncryptDecrypt(dataToStore);
            }

            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream)) 
                {
                    writer.Write(dataToStore);
                }
            }

        } catch (Exception e) {
            BugTracker.Error("Failed to save Data: " + e);
        }
    }

    public void Delete(GameData data)
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        
        try {
            if (File.Exists(fullPath))
                Directory.Delete(Path.GetDirectoryName(fullPath));
            else
                BugTracker.Warning("Tried to delete directory but failed to found at path: " + fullPath);

        } catch (Exception e) {
            BugTracker.Error("Failed to save Data: " + e);
        }
    }

    private string EncryptDecrypt(string data) 
    {
        string modifiedData = "";

        for (int i = 0; i < data.Length; i++) 
            modifiedData += (char) (data[i] ^ encryptionCodeWord[i % encryptionCodeWord.Length]);

        return modifiedData;
    }
}
