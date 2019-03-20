using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

/// <summary>
/// Class used for storing and serializing saved players from the game.
/// 
/// Decided to save players as a single dictionary instead of in different
/// objects, which could have resulted in several different save files.
/// In the case for a more complex game, with more data regarding each
/// player this would be preferable and safer - for the case of save data corruption, 
/// for instance. In this case, however, it does allow me to ignore things such as 
/// finding specific save files and figuring how many there are in the folder.
/// </summary>
public static class DataManager {


    /// <summary>
    /// Updates a player entry on the dictionary of players, and formats data to binary.
    /// </summary>
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

    /// <summary>
    /// Deserializes player file and returns its data as a GameData object.
    /// </summary>
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
