using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using TMPro;

public class FileDataHandler
{
    private string dataDirPath = "";
    private string dataFileName = "";
    public TMP_InputField dataInputField;

    public FileDataHandler(string dataDirPath, string dataFileName)
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
    }

    public GestureList Load()
    {
        // using Path.Combine to account for different OS's having different path separator
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        GestureList loadedData = new GestureList();
        if (File.Exists(fullPath))
        {
            if (dataInputField)
                dataInputField.text = "ya un de fichier";
            try
            {
                // Load the serialized data from the file
                string dataToLoad = "";
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                // deserialize the data from Json back into the c# object
                loadedData = JsonUtility.FromJson<GestureList>(dataToLoad);
            }
            catch (Exception ex)
            {
                Debug.LogError("Error occured when trying to load data to file: " + fullPath + "\n" + ex);
                if (dataInputField)
                    dataInputField.text = "Error occured when trying to load data to file: " + fullPath + "\n" + ex;
            }
        }
        else
        {
            if (dataInputField)
                dataInputField.text = "ya pas de fichier";
        }

        return loadedData;

    }

    public HandsDists HandsDistsLoad()
    {
        // using Path.Combine to account for different OS's having different path separator
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        HandsDists loadedData = new HandsDists();
        if (File.Exists(fullPath))
        {
            if (dataInputField)
                dataInputField.text = "ya un de fichier";
            try
            {
                // Load the serialized data from the file
                string dataToLoad = "";
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                // deserialize the data from Json back into the c# object
                loadedData = JsonUtility.FromJson<HandsDists>(dataToLoad);
            }
            catch (Exception ex)
            {
                Debug.LogError("Error occured when trying to load data to file: " + fullPath + "\n" + ex);
                if (dataInputField)
                    dataInputField.text = "Error occured when trying to load data to file: " + fullPath + "\n" + ex;
            }
        }
        else
        {
            loadedData = null;
            if (dataInputField)
                dataInputField.text = "ya pas de fichier";
        }

        return loadedData;

    }

    public void Save(GestureList data)
    {
        // using Path.Combine to account for different OS's having different path separator
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        try
        {
            // create the directory the file will be written to if it doesn't already exist
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            // serialize the C# game data object into Json
            string dataToStore = JsonUtility.ToJson(data, true);

            // write the serialized data to the file
            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                    if (dataInputField)
                        dataInputField.text = "on a écrit sur le fichier";
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error occured when trying to save data to file: " + fullPath + "\n" + ex);
            if (dataInputField)
                dataInputField.text = "Error occured when trying to save data to file: " + fullPath + "\n" + ex;
        }
    }

    public void Save(HandsDists data)
    {
        // using Path.Combine to account for different OS's having different path separator
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        try
        {
            // create the directory the file will be written to if it doesn't already exist
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            // serialize the C# game data object into Json
            string dataToStore = JsonUtility.ToJson(data, true);

            // write the serialized data to the file
            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                    if (dataInputField)
                        dataInputField.text = "on a écrit sur le fichier";
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error occured when trying to save data to file: " + fullPath + "\n" + ex);
            if (dataInputField)
                dataInputField.text = "Error occured when trying to save data to file: " + fullPath + "\n" + ex;
        }
    }
}
