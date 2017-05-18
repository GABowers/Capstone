using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveLoad : MonoBehaviour
{
    JSONStorage jsonStorage;
    private string filePath;
    private MainPageInfo mainPageInfo;
    public MainPageController mainPageController;

    // Use this for initialization
    void Start ()
    {
        jsonStorage = GameObject.Find("StorageGO").GetComponent<JSONStorage>();
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void Save ()
    {
        string file = Application.dataPath;
        string[] pathArray = file.Split('/');
        file = "";
        for (int i = 0; i < pathArray.Length - 1; i++)
        {
            file += pathArray[i] + "/";
        }
        string newFolder = file + jsonStorage.neighborhoodType.ToString() + "/";
        Directory.CreateDirectory(newFolder);
        string newnewFolder = newFolder + jsonStorage.amountOfCellTypes.ToString() + " Cell Types" + "/";
        Directory.CreateDirectory(newnewFolder);
        filePath = newnewFolder + jsonStorage.uniqueFileName + ".json";
        string jsonString = JsonUtility.ToJson(jsonStorage);
        File.WriteAllText(filePath, jsonString);
    }

    public void Load ()
    {
        string file = Application.dataPath;
        string[] pathArray = file.Split('/');
        file = "";
        for (int i = 0; i < pathArray.Length - 1; i++)
        {
            file += pathArray[i] + "/";
        }
        string oldFolder = file + mainPageController.mainPageInfo.nType.ToString() + "/";
        string oldoldFolder = oldFolder + mainPageController.mainPageInfo.numStates.ToString() + " Cell Types" + "/";
        filePath = oldoldFolder + mainPageController.mainPageInfo.uniqueFileName + ".json";
        string jsonString = File.ReadAllText(filePath);
        JsonUtility.FromJsonOverwrite(jsonString, jsonStorage);
    }
}
