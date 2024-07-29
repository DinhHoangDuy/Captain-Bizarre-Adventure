using UnityEngine;
using System;
using System.IO;
public class FileDataHandler
{
    private string dataDirPath = "";
    private string dataFileName = "";
    private bool useEncryption = false;
    private readonly string encryptionPassword = "TheDreamIsTooGoodToBeReal";
    
    public FileDataHandler(string dataDirPath, string dataFileName, bool useEncryption)
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
        this.useEncryption = useEncryption;
    }

    public GameData Load()
    {
        // Use Path.Combine to combine the dataDirPath and dataFileName
        string fullFilePath = Path.Combine(dataDirPath, dataFileName);
        GameData loadedData = null;
        // Check if the file exists
        if (File.Exists(fullFilePath))
        {
            try
            {
                // Load the Serialized JSON from the file
                string dataToLoad = "";
                using (FileStream stream = new FileStream(fullFilePath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }
                
                // Decrypt the data if encryption is enabled
                if (useEncryption)
                {
                    dataToLoad = EncryptDecrypt(dataToLoad);
                }
                
                // Deserialize the JSON string to a GameData object
                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.LogError("Error loading data from file: " + fullFilePath + "\n" + e);
            }
        }

        return loadedData;
    }

    public void Save(GameData data)
    {
        // Use Path.Combine to combine the dataDirPath and dataFileName
        string fullFilePath = Path.Combine(dataDirPath, dataFileName);
        try
        {
            // Create the directory if it doesn't exist
            Directory.CreateDirectory(Path.GetDirectoryName(fullFilePath));
            
            // Serialize the data to a JSON string
            string jsonData = JsonUtility.ToJson(data, true);
            
            // Encrypt the data if encryption is enabled
            if (useEncryption)
            {
                jsonData = EncryptDecrypt(jsonData);
            }
            
            // Write the JSON string to the file
            using (FileStream stream = new FileStream(fullFilePath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(jsonData);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error saving data to file: " + e.Message);
        }
    }
    
    // Use XOR encryption to encrypt and decrypt the data
    private string EncryptDecrypt(string data)
    {
        string result = string.Empty;
        for (int i = 0; i < data.Length; i++)
        {
            result += (char)(data[i] ^ encryptionPassword[(i % encryptionPassword.Length)]);
        }
        return result;
    }
}
