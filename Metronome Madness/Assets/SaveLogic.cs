using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public class SaveLogic : MonoBehaviour
{
    List<List<List<string>>> saveThis = new List<List<List<string>>>();
    void Start()
    {

    }

    public void SaveStringList(List<List<List<string>>> listToSave, string filename)
    {
        string filePath = Path.Combine(Application.persistentDataPath, filename);
        string json = JsonConvert.SerializeObject(listToSave, Formatting.Indented);
        File.WriteAllText(filePath, json);
    }
}