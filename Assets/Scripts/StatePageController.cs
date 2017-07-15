using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class StatePageController : MonoBehaviour {

    public FirstController firstController;
    public Text prefabText;
    public InputField prefabInputField;

    private StatePageInfo statePageInfo;
    Text mainLabel;
    Dropdown colorDD;
    public InputField startingCellAmount;
    RectTransform labelPanel;
    RectTransform inputPanel;
    List<Text> probLabels;
    List<InputField> probInputs;

    public void Awake()
    {
        mainLabel = GetComponentsInChildren<Text>().Where(x => x.name == "CellStateLabel").First();
        colorDD = GetComponentInChildren<Dropdown>();
        startingCellAmount = GetComponentInChildren<InputField>();
        labelPanel = GetComponentsInChildren<RectTransform>().Where(x => x.name == "LabelPanel").First();
        inputPanel = GetComponentsInChildren<RectTransform>().Where(x => x.name == "InputPanel").First();
        probLabels = new List<Text>();
        probInputs = new List<InputField>();
    }

    public void SetInfo(StatePageInfo info)
    {
        statePageInfo = info;
        mainLabel.text = "Cell State " + info.stateNum.ToString();
        colorDD.value = info.color;

        if (info.startingAmount.HasValue)
            startingCellAmount.text = info.startingAmount.ToString();

        int currentState = 1;
        if (info.probs.GetLength(0) > 1)
        {
            for (int toState = 0; toState < (info.probs.GetLength(0) - 1); ++toState, ++currentState)
            {
                if (currentState == info.stateNum)
                    currentState++;
                for (int neighborState = 0; neighborState < info.probs.GetLength(1); ++neighborState)
                {
                    if ((neighborState + 1) == info.stateNum)
                        continue;
                    for (int neighbors = 0; neighbors < info.probs.GetLength(2); ++neighbors)
                    {
                        Text currentText = Instantiate(prefabText, labelPanel);
                        Vector2 sizeDelta = currentText.rectTransform.sizeDelta;
                        sizeDelta.x = 300;
                        currentText.rectTransform.sizeDelta = sizeDelta;
                        string text = string.Format("From State {0} to State {1} with {2} neighbors of State {3}", info.stateNum, currentState, neighbors, (neighborState + 1));
                        currentText.text = text;
                        probLabels.Add(currentText);
                        InputField currentIF = Instantiate(prefabInputField, inputPanel);

                        if (info.probs[toState, neighborState, neighbors].HasValue)
                            currentIF.text = info.probs[toState, neighborState, neighbors].ToString();
                        probInputs.Add(currentIF);
                    }
                }
            }

            int index = 0;
            for (int i = 0; i < info.probs.GetLength(0); ++i)
            {
                if (i == (info.stateNum - 1))
                    continue;
                for (int j = 0; j < info.probs.GetLength(1); ++j)
                {
                    if (j == (info.stateNum - 1))
                        continue;
                    for (int k = 0; k < info.probs.GetLength(2); ++k)
                    {
                        probInputs[index].text = statePageInfo.probs[i, j, k].ToString();
                        index++;
                    }
                }
            }
            probInputs[0].Select();
        }
    }

    public void BackButton()
    {
        UpdateValues();
        DestroyPrefabs();
        firstController.CellPageBack(statePageInfo.stateNum);
    }

    public void NextButton()
    {
        UpdateValues();
        DestroyPrefabs();
        firstController.CellPageNext(statePageInfo.stateNum);
    }

    private void DestroyPrefabs()
    {
        for (int i = (probLabels.Count - 1); i >= 0; --i)
        {
            Destroy(probLabels[i].gameObject);
            Destroy(probInputs[i].gameObject);
        }
        probLabels.Clear();
        probInputs.Clear();
    }

    private void UpdateValues()
    {
        statePageInfo.color = colorDD.value;
        statePageInfo.startingAmount = int.Parse(startingCellAmount.text);
        int index = 0;
        for (int i = 0; i < statePageInfo.probs.GetLength(0); ++i)
        {
            if (i == (statePageInfo.stateNum - 1))
            {
                for (int j = 0; j < statePageInfo.probs.GetLength(1); ++j)
                {
                    for (int k = 0; k < statePageInfo.probs.GetLength(2); ++k)
                    {
                        Tools.AssignFloatVal(ref statePageInfo.probs[i, j, k], 0.ToString());
                    }
                }
                continue;
            }
            for (int j = 0; j < statePageInfo.probs.GetLength(1); ++j)
            {
                if (j == (statePageInfo.stateNum - 1))
                {
                    for (int k = 0; k < statePageInfo.probs.GetLength(2); ++k)
                    {
                        Tools.AssignFloatVal(ref statePageInfo.probs[i, j, k], 0.ToString());
                    }
                    continue;
                }
                for (int k = 0; k < statePageInfo.probs.GetLength(2); ++k)
                {
                    Tools.AssignFloatVal(ref statePageInfo.probs[i, j, k], probInputs[index].text);
                    index++;
                }
            }
        }
    }
}
