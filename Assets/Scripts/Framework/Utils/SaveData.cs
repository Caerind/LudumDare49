using UnityEngine;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveData
{
    public const string folder = "/saves/";

    public static void Save<T>(T objectToSave, string key)
    {
        SaveToFile<T>(objectToSave, key);
    }

    public static void Save(Object objectToSave, string key)
    {
        SaveToFile<Object>(objectToSave, key);
    }

    public static T Load<T>(string key)
    {
        string path = Application.persistentDataPath + folder;

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream fileStream = new FileStream(path + key, FileMode.Open);

        T returnValue = default(T);

        try
        {
            returnValue = (T)formatter.Deserialize(fileStream);
        }
        catch (SerializationException exception)
        {
            Debug.Log("Load failed. Error: " + exception.Message);
        }
        finally
        {
            fileStream.Close();
        }

        return returnValue;
    }

    private static void SaveToFile<T>(T objectToSave, string fileName)
    {
        string path = Application.persistentDataPath + folder;

        Directory.CreateDirectory(path);

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream fileStream = new FileStream(path + fileName, FileMode.Create);

        try
        {
            formatter.Serialize(fileStream, objectToSave);
        }
        catch (SerializationException exception)
        {
            Debug.Log("Save failed. Error: " + exception.Message);
        }
        finally
        {
            fileStream.Close();
        }
    }
}
