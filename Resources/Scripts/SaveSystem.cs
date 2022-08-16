using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

public static class SaveSystem
{
    public static void SavePlayer<T>(ref T fileToSave, string uid)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        bool createNew = false;

        if (!Directory.Exists(Application.persistentDataPath + "/Recordings"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/Recordings");
        }

        if (!Directory.Exists(Application.persistentDataPath + "/Recordings/" + RecordManager.Instance.RecordDateTime))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/Recordings/" + RecordManager.Instance.RecordDateTime);
            createNew = true;
        }

        string path = Application.persistentDataPath + "/Recordings/" + RecordManager.Instance.RecordDateTime + "/" + uid.ToString() + ".lado";

        FileStream stream = new FileStream(path, FileMode.CreateNew);

        formatter.Serialize(stream, fileToSave);
        stream.Close();

        if (!createNew)
        {
            Debug.Log("File saved at: " + path);
        }
    }

    public static string[] GetRecordings()
    {
        List<string> recordings = new List<string>();
        foreach (var item in Directory.GetDirectories(Application.persistentDataPath + "/Recordings/"))
        {
            string currentDirectoryName = "";
            for (int i = (Application.persistentDataPath + "/Recordings/").Length; i < item.Length; i++)
            {
                currentDirectoryName += item[i];
            }
            recordings.Add(currentDirectoryName);
        }
        return recordings.ToArray();
    }

    public static IndexRecordData GetBoardRecordData()
    {
        string path = Application.persistentDataPath + "/Recordings/" + PlayerPrefs.GetString("CurrentRecording") + "/" + UID.BoardSelection.ToString() + ".lado";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            IndexRecordData data = formatter.Deserialize(stream) as IndexRecordData;
            stream.Close();

            return data;
        }

        return null;
    }

    public static TransformRecordData GetTransformRecordData(string uid)
    {
        string path = Application.persistentDataPath + "/Recordings/" + PlayerPrefs.GetString("CurrentRecording") + "/" + uid + ".lado";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            TransformRecordData data = formatter.Deserialize(stream) as TransformRecordData;
            stream.Close();

            return data;
        }

        return null;
    }

    public static WhiteboardRecordData GetWhiteboardData()
    {
        string path = Application.persistentDataPath + "/Recordings/" + PlayerPrefs.GetString("CurrentRecording") + "/" + UID.WhiteboardData + ".lado";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            WhiteboardRecordData data = formatter.Deserialize(stream) as WhiteboardRecordData;
            stream.Close();
            
            return data;
        }

        return null;
    }

    public static WhiteboardRecordData GetWhiteboardInitializationData()
    {
        string path = Application.persistentDataPath + "/Recordings/" + PlayerPrefs.GetString("CurrentRecording") + "/" + UID.WhiteboardInitialization + ".lado";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            WhiteboardRecordData data = formatter.Deserialize(stream) as WhiteboardRecordData;
            stream.Close();

            return data;
        }

        return null;
    }

    public static PdfRecordData GetSlideData()
    {
        string path = Application.persistentDataPath + "/Recordings/" + PlayerPrefs.GetString("CurrentRecording") + "/" + UID.SlidesData + ".lado";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            PdfRecordData data = formatter.Deserialize(stream) as PdfRecordData;
            stream.Close();

            return data;
        }

        return null;
    }
}
