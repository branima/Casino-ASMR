using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.Text;
using System;

public static class SaveSystem
{
    public static string filePath = Application.persistentDataPath;
    public static string fileName = "CasinoRollSaveData.bin";
    public static string fullSavePath = filePath + "/" + fileName;

    public static void SaveGame(SaveData saveData)
    {

        var aSerializer = new XmlSerializer(typeof(SaveData));
        StringBuilder sb = new StringBuilder();

        FileStream stream = new FileStream(fullSavePath, FileMode.Create);
        aSerializer.Serialize(stream, saveData); // pass an instance of A      
        stream.Close();
    }

    public static SaveData LoadGame()
    {
        if (File.Exists(fullSavePath))
        {

            var aSerializer = new XmlSerializer(typeof(SaveData));
            FileStream stream = new FileStream(fullSavePath, FileMode.Open);

            SaveData saveData = aSerializer.Deserialize(stream) as SaveData;
            stream.Close();

            return saveData;
        }
        else
            return null;
    }
}
