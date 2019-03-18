using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

public static class DataManager {
    
    public static void SavePlayerData(PlayerData currentPlayer, Dictionary<string, Dictionary<string, int>> playerBase)
    {
        // Creates path to file.
        string dataPath = Application.persistentDataPath + "/players.dct";

        // Initializes a Formatter, FileStream and GameData object with relevant content.
        BinaryFormatter dataFormatter = new BinaryFormatter();
        FileStream stream = new FileStream(dataPath, FileMode.Create);
        GameData data = new GameData(currentPlayer, playerBase);

        // Serializes data and closes stream.
        dataFormatter.Serialize(stream, data);
        stream.Close();
    }

    public static GameData LoadPlayers()
    {
        // Creates path to file.
        string dataPath = Application.persistentDataPath + "/players.dct";

        // If save file doesn't exist yet, needs creation.
        if (!File.Exists(dataPath)) return null;

        // Then, cast to GameData and return it.
        BinaryFormatter dataFormatter = new BinaryFormatter();
        FileStream stream = new FileStream(dataPath, FileMode.Open);
        GameData data = (GameData) dataFormatter.Deserialize(stream);

        stream.Close();
        return data;
    }

    public static void DeleteSaveFile()
    {
        // Creates path to file.
        string dataPath = Application.persistentDataPath + "/players.dct";

        // If save file exists yet, delete it.
        if (File.Exists(dataPath)) File.Delete(dataPath);
    }
}
