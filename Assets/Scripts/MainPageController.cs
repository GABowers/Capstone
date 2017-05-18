using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainPageController : MonoBehaviour {

    public FirstController firstController;
    SaveLoad saveLoad;
    JSONStorage jsonStorage;

    public MainPageInfo mainPageInfo;
    Dropdown gridType;
    Dropdown nHood;
    InputField numStates;
    InputField gridWidth;
    InputField gridHeight;
    public GameObject loadDialog;
    public int value;
    public InputField loadFileName;

    private void Awake()
    {
        jsonStorage = GameObject.Find("StorageGO").GetComponent<JSONStorage>();
        gridType = GameObject.FindGameObjectWithTag("GridTypeDropdown").GetComponentInChildren<Dropdown>();
        nHood = GameObject.FindGameObjectWithTag("NeighborhoodDropdown").GetComponentInChildren<Dropdown>();
        InputField[] iFields = GetComponentsInChildren<InputField>();
        numStates = iFields[0];
        gridWidth = iFields[1];
        gridHeight = iFields[2];
        saveLoad = GameObject.Find("StorageGO").GetComponent<SaveLoad>();
    }

    public void SetInfo(MainPageInfo info)
    {
        mainPageInfo = info;

        gridType.value = (int)mainPageInfo.gridType;
        nHood.value = (int)mainPageInfo.nType;
        if (mainPageInfo.numStates.HasValue)
            numStates.text = mainPageInfo.numStates.Value.ToString();
        if (mainPageInfo.gridWidth.HasValue)
            gridWidth.text = mainPageInfo.gridWidth.Value.ToString();
        if (mainPageInfo.gridHeight.HasValue)
            gridHeight.text = mainPageInfo.gridHeight.Value.ToString();
        nHood.Select();
    }

    public void LoadFile()
    {
        UpdateValues();
        loadDialog.GetComponent<CanvasGroup>().alpha = 1;
        loadDialog.GetComponent<CanvasGroup>().interactable = true;
        loadDialog.GetComponent<CanvasGroup>().blocksRaycasts = true;
        loadFileName.Select();
    }

    public void LoadCancel()
    {
        loadDialog.GetComponent<CanvasGroup>().alpha = 0;
        loadDialog.GetComponent<CanvasGroup>().interactable = false;
        loadDialog.GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void LoadDone()
    {
        mainPageInfo.uniqueFileName = loadDialog.transform.Find("LoadFileInputField").Find("Text").GetComponent<Text>().text;
        saveLoad.Load();
        LoadSettings();
        loadDialog.GetComponent<CanvasGroup>().alpha = 0;
        loadDialog.GetComponent<CanvasGroup>().interactable = false;
        loadDialog.GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void LoadSettings()
    {
        firstController.statePageInfo.Clear();
        int neighbors;
        switch (jsonStorage.neighborhoodType)
        {
            case NType.None:
                neighbors = 0;
                break;
            case NType.VonNeumann:
                neighbors = 4;
                break;
            case NType.Moore:
                neighbors = 8;
                break;
            case NType.Hybrid:
                neighbors = 12;
                break;
            default:
                neighbors = 0;
                break;
        }
        value = 0;
        for (int h = 0; h < jsonStorage.amountOfCellTypes; ++h)
        {
            StatePageInfo current = new StatePageInfo(mainPageInfo.numStates.Value, neighbors, h + 1);
            firstController.statePageInfo.Add(current);
            firstController.statePageInfo[h].startingAmount = jsonStorage.numberCellsPerType[h];
            firstController.statePageInfo[h].color = jsonStorage.colorDropdownValues[h];
            //print(firstController.statePageInfo[h].probs.GetLength(2));
            for (int i = 0; i < firstController.statePageInfo[h].probs.GetLength(0); ++i)
            {
                for (int j = 0; j < (firstController.statePageInfo[h].probs.GetLength(1)); ++j)
                {
                    for (int k = 0; k < firstController.statePageInfo[h].probs.GetLength(2); ++k)
                    {
                        firstController.statePageInfo[h].probs[i, j, k] = jsonStorage.probabilities[value];
                        ++value;
                    }
                }
            }
        }
    }

    public void NextButton()
    {
        UpdateValues();
        firstController.MainPageNext();
    }

	public void UpdateValues()
    {
        mainPageInfo.gridType = (GridType)gridType.value;
        mainPageInfo.nType = (NType)nHood.value;
        Tools.AssignIntVal(ref mainPageInfo.numStates, numStates.text);
        Tools.AssignIntVal(ref mainPageInfo.gridWidth, gridWidth.text);
        Tools.AssignIntVal(ref mainPageInfo.gridHeight, gridHeight.text);
    }
}
