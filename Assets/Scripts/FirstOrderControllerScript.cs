using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;

public class FirstOrderControllerScript : MonoBehaviour
{
    JSONStorage jsonStorage;
    SaveLoad saveLoad;
    int amountOfCellTypes;
    public int gridWidth;
    public int gridHeight;
    List<float> ratios = new List<float>();
    List<int> colors = new List<int>();
    List<List<int>> fullCount = new List<List<int>>();
    Dictionary<int, Color> dict;
    Dropdown neighborEffectsDropdown;
    Dropdown gridTypeDropdown;
    List<byte[]> imageList = new List<byte[]>();
    public GameObject saveDialog;

    float minScale;
    public float difHeight;
    public float difWidth;

    public Text pauseButtonText;
    public Text editButtonText;
    public Text editGridText;

    public Toggle storeImages;
    public Toggle backgroundCheckToggle;

    GameObject iterationIFGO;
    Toggle iterationToggle;

    GameObject saveCountIFGO;
    Toggle saveCountToggle;

    GameObject saveImageIFGO;
    Toggle saveImageToggle;

    GameObject runCapIFGO;
    Toggle runCapToggle;

    public int maxIterations;
    public int maxRuns;
    public int saveImageCount;
    public int saveCountCount;

    public int preXValue;
    public int preYValue;
    public int postXValue;
    public int postYValue;

    int colorDropdownValue;
    public List<int> colorDropdownValues;
    int numberCellsOfType;
    public List<int> numberCellsPerType;

    CA myCA;
    List<StatePageInfo> statePageInfoStuff;
    private MainPageInfo mainPageInfo;
    public FirstController otherController;
    
    Texture2D tex;
    Sprite board;
    SpriteRenderer sr;
    //public Canvas gridPanel;
    private int iterations = 0;
    public int runs = 1;
    public Text iterText;
    public Text runText;
    //public List<float> probabilities;

    bool alreadyCA;
    public bool running;
    public bool continueCA = false;
    bool saveOpen = false;
    bool editModeOn = false;

    NType nType;
    GridType gType;
    public string uniqueFileName;

    public Point editPoint;

    private void Awake()
    {
    }

    private void Start()
    {
        iterationIFGO = GameObject.Find("IterationInputField").transform.Find("Text").gameObject;
        saveCountIFGO = GameObject.Find("CountInputField").transform.Find("Text").gameObject;
        saveImageIFGO = GameObject.Find("ImageInputField").transform.Find("Text").gameObject;
        runCapIFGO = GameObject.Find("RunInputField").transform.Find("Text").gameObject;
        runs = 1;
        alreadyCA = false;
        //storeImages = GameObject.Find("ImageStoreToggle").GetComponent<Toggle>();
        iterationToggle = GameObject.Find("IterationToggle").GetComponent<Toggle>();
        saveCountToggle = GameObject.Find("SaveCountToggle").GetComponent<Toggle>();
        saveImageToggle = GameObject.Find("SaveImageToggle").GetComponent<Toggle>();
        runCapToggle = GameObject.Find("RunToggle").GetComponent<Toggle>();
        jsonStorage = GameObject.Find("StorageGO").GetComponent<JSONStorage>();
        saveLoad = GameObject.Find("StorageGO").GetComponent<SaveLoad>();

        int screenWidth = Screen.width;
        int screenHeight = Screen.height;
        minScale = Mathf.Min(((float)screenHeight), ((float)screenWidth));

        //This dictionary only uses the Unity-named colors because it's easier to pick these in a drop-down than to have a color selector.
        //Future updates might adjust the UI page to allow for direct color selection, which could then be passed to this script and avoid the artificial limitations here.
        //After all, this can only show up to 9 cell types. Any number is possible, but with only 9 colors you can only differentiate between nine types.
        dict = new Dictionary<int, Color>();
        dict.Add(0, Color.black);
        dict.Add(1, Color.blue);
        dict.Add(2, Color.cyan);
        dict.Add(3, Color.gray);
        dict.Add(4, Color.green);
        dict.Add(5, Color.magenta);
        dict.Add(6, Color.red);
        dict.Add(7, Color.white);
        dict.Add(8, Color.yellow);
    }

    private void Update()
    {
        running = false;
        if (Input.GetKeyUp(KeyCode.E) && (saveOpen == false) && (editModeOn == false))
        {
            OneIteration();
            UpdateBoard();
        }
        if (Input.GetKey(KeyCode.C) && (saveOpen == false) && (editModeOn == false))
        {
            OneIteration();
            UpdateBoard();
        }

        if ((continueCA == true) && (saveOpen == false) && (editModeOn == false))
        {
            OneIteration();
            UpdateBoard();
            CheckSettings();
        }

        if (continueCA == false)
            running = false;

        if((Input.GetMouseButtonDown(0)) && (saveOpen == false) && (editModeOn == true))
        {
            MakeEditDown();
        }

        if ((Input.GetMouseButtonUp(0)) && (saveOpen == false) && (editModeOn == true))
        {
            MakeEditUp();
        }

        if (Input.GetKeyUp(KeyCode.Space) && (editModeOn == true))
        {
            EditGrid();
        }

        if ((Input.GetMouseButtonDown(1)) && (saveOpen == false) && (editModeOn == true))
        {
            EditGrid();
        }
    }

    public void SaveFile()
    {
        saveOpen = true;
        saveDialog.GetComponent<CanvasGroup>().alpha = 1;
        saveDialog.GetComponent<CanvasGroup>().interactable = true;
        saveDialog.GetComponent<CanvasGroup>().blocksRaycasts = true;
        sr.GetComponent<SpriteRenderer>().sortingLayerName = "Background";
    }

    public void SaveFileDone()
    {
        SaveSettings(mainPageInfo);

        saveDialog.GetComponent<CanvasGroup>().alpha = 0;
        saveDialog.GetComponent<CanvasGroup>().interactable = false;
        saveDialog.GetComponent<CanvasGroup>().blocksRaycasts = false;
        saveOpen = false;
        sr.GetComponent<SpriteRenderer>().sortingLayerName = "Foreground";
    }

    public void SaveFileCancel()
    {
        saveDialog.GetComponent<CanvasGroup>().alpha = 0;
        saveDialog.GetComponent<CanvasGroup>().interactable = false;
        saveDialog.GetComponent<CanvasGroup>().blocksRaycasts = false;
        saveOpen = false;
        sr.GetComponent<SpriteRenderer>().sortingLayerName = "Foreground";
    }

    public void EditGrid()
    {
            editModeOn = !editModeOn;
            difWidth = ((Screen.width - (sr.GetComponent<SpriteRenderer>().bounds.size.x * 76.8f)) / 2);
            difHeight = ((Screen.height - (sr.GetComponent<SpriteRenderer>().bounds.size.y * 76.8f)) / 2);
            editButtonText.text = editModeOn ? "Editing" : "Edit Grid";
    }

    public void MakeEditDown()
    {
        float tempX = (((Input.mousePosition.x - difWidth) / gridWidth) / (sr.transform.localScale.x)) * gridWidth;
        float tempY = (((Input.mousePosition.y - difHeight) / gridHeight) / (sr.transform.localScale.y)) * gridHeight;

        if (tempX < 0 || tempY < 0)
            return;
        preXValue = (int)((((Input.mousePosition.x - difWidth) / gridWidth) / (sr.transform.localScale.x)) * gridWidth);
        preYValue = (int)((((Input.mousePosition.y - difHeight) / gridHeight) / (sr.transform.localScale.y)) * gridHeight);
    }

    public void MakeEditUp()
    {
        float tempX = (((Input.mousePosition.x - difWidth) / gridWidth) / (sr.transform.localScale.x)) * gridWidth;
        float tempY = (((Input.mousePosition.y - difHeight) / gridHeight) / (sr.transform.localScale.y)) * gridHeight;

        if (tempX < 0 || tempY < 0)
            return;
        postXValue = (int)((((Input.mousePosition.x - difWidth) / gridWidth) / (sr.transform.localScale.x)) * gridWidth);
        postYValue = (int)((((Input.mousePosition.y - difHeight) / gridHeight) / (sr.transform.localScale.y)) * gridHeight);
        if (preXValue > postXValue)
        {
            int preXStorage = preXValue;
            preXValue = postXValue;
            postXValue = preXStorage;
        }
        print("A");
        if (preYValue > postYValue)
        {
            int preYStorage = preYValue;
            preYValue = postYValue;
            postYValue = preYStorage;
        }
        print("B");
        for (int i = preXValue; i <= postXValue; ++i)
        {
            print("Hi!");
            for (int j = preYValue; j <= postYValue; ++j)
            {
                print("Hi again!");
                myCA.ChangeCell(i, j);
                print(i + ", " + j);
            }
        }
        print("C");
        UpdateBoard();
    }

    public void PauseButton()
    {
        continueCA = !continueCA;
        pauseButtonText.text = continueCA ? "UnPaused" : "Paused";
    }

    public void EditProbs()
    {
        if(running == false && editModeOn == false)
        {

        }
    }

    public void CheckSettings()
    {
        if (iterationToggle.isOn)
        {
            StartCoroutine(IterationAutomate());
        }

        if (saveImageToggle.isOn)
        {
            if (saveImageIFGO.GetComponent<Text>().text != "")
            {
                saveImageCount = int.Parse(saveImageIFGO.GetComponent<Text>().text);
                bool isMultiple = iterations % saveImageCount == 0;
                if (isMultiple == true)
                {
                    SaveImage();
                }
            }
        }

        if (saveCountToggle.isOn)
        {
            if (saveCountIFGO.GetComponent<Text>().text != "")
            {
                saveCountCount = int.Parse(saveCountIFGO.GetComponent<Text>().text);
                bool isMultiple = iterations % saveCountCount == 0;
                if (isMultiple == true)
                {
                    SaveIterationCount();
                }
            }
        }
    }

    public void OneIteration()
    {
        running = true;
        myCA.OneIteration();
        iterations++;
        iterText.text = "Iteration: " + iterations.ToString();
        List<int> currentCellCount = new List<int>();
        currentCellCount.AddRange(CA.stateCount);
        fullCount.Add(currentCellCount);
        //if (storeImages.isOn)
        //{
        //    StoreImage();
        //}
    }

    public void SaveSettings(MainPageInfo info)
    {
        uniqueFileName = saveDialog.transform.Find("SaveFileInputField").Find("Text").GetComponent<Text>().text;
        statePageInfoStuff = otherController.statePageInfo;
        jsonStorage.amountOfCellTypes = mainPageInfo.numStates.Value;
        mainPageInfo = info;
        jsonStorage.neighborhoodType = mainPageInfo.nType;
        gridWidth = mainPageInfo.gridWidth.Value;
        gridHeight = mainPageInfo.gridHeight.Value;
        jsonStorage.gridType = mainPageInfo.gridType;

        for (int h = 0; h < statePageInfoStuff.Count; ++h)
        {
            jsonStorage.numberCellsPerType.Add(statePageInfoStuff[h].startingAmount.Value);
            jsonStorage.colorDropdownValues.Add(statePageInfoStuff[h].color);
            for (int i = 0; i < statePageInfoStuff[h].probs.GetLength(0); ++i)
            {
                for (int j = 0; j < (statePageInfoStuff[h].probs.GetLength(1)); ++j)
                {
                    for (int k = 0; k < statePageInfoStuff[h].probs.GetLength(2); ++k)
                    {
                        jsonStorage.probabilities.Add(statePageInfoStuff[h].probs[i, j, k].Value);
                    }
                }
            }
        }
        
        jsonStorage.uniqueFileName = uniqueFileName;
        jsonStorage.neighborhoodType = nType;
        jsonStorage.gridType = gType;

        saveLoad.Save();
    }

    public void CreateCA(MainPageInfo info)
    {
        editModeOn = false;
        mainPageInfo = info;
        statePageInfoStuff = otherController.statePageInfo;
        amountOfCellTypes = mainPageInfo.numStates.Value;
        nType = mainPageInfo.nType;
        gridWidth = mainPageInfo.gridWidth.Value;
        gridHeight = mainPageInfo.gridHeight.Value;
        gType = mainPageInfo.gridType;
        myCA = new CA(gridWidth, gridHeight, amountOfCellTypes, nType, gType);

        for (int h = 0; h < statePageInfoStuff.Count; ++h)
        {
            ratios.Add(statePageInfoStuff[h].startingAmount.Value);
            colors.Add(statePageInfoStuff[h].color);
            for (int i = 0; i < statePageInfoStuff[h].probs.GetLength(0); ++i)
            {
                for (int j = 0; j < (statePageInfoStuff[h].probs.GetLength(1)); ++j)
                {
                    for (int k = 0; k < statePageInfoStuff[h].probs.GetLength(2); ++k)
                    {
                        myCA.SetStateInfo(h, i, j, k, statePageInfoStuff[h].probs[i, j, k].Value);
                    }
                }
            }
        }

        myCA.InitializeGrid(ratios);
    }

    public void StartCA()
    {
        editModeOn = false;
        if (alreadyCA == false)
        {
            iterations = 0;
            runText.text = "Run: " + runs;
            tex = new Texture2D((gridWidth), (gridHeight));
            tex.filterMode = FilterMode.Point;
            board = Sprite.Create(tex, new Rect(Vector2.zero, new Vector2((gridWidth), (gridHeight))), Vector2.zero, 1f);
            sr = gameObject.AddComponent<SpriteRenderer>();
            sr.transform.localPosition = new Vector2(0f, 0f);
            sr.transform.localScale = new Vector2((minScale / gridWidth), (minScale / gridHeight));
            sr.transform.localPosition = new Vector2((sr.transform.position.x - (((gridWidth) / 2) * sr.transform.localScale.x)), (sr.transform.position.y - (((gridHeight) / 2) * sr.transform.localScale.y)));
            sr.GetComponent<SpriteRenderer>().sortingLayerName = "Foreground";
            sr.sprite = board;
            //gridPanel.sortingLayerName = "Foreforeground";
            UpdateBoard();
            alreadyCA = true;
        }
    }

    private void UpdateBoard()
    {
        Color tileColor;
        for (int i = 0; i < gridWidth; ++i)
        {
            for (int j = 0; j < gridHeight; ++j)
            {
                tileColor = dict[colors[myCA.GetCellState(i, j)]];
                tex.SetPixel(i, j, tileColor);
            }
        }
        tex.Apply();
    }

    public void SaveImage()
    {
        string newFolder = Application.persistentDataPath + "/CA Image Captures/" + System.DateTime.Now.ToString("yyyy-MM-dd") + "/";
        byte[] bytes = tex.EncodeToPNG();
        Directory.CreateDirectory(newFolder);
        File.WriteAllBytes(newFolder + System.DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + " Run " + runs + " Iteration " + iterations + ".png", bytes);
    }

    public void StoreImage()
    {
        byte[] bytes = tex.EncodeToPNG();
        var newBytes = bytes;
        imageList.Add(newBytes);
    }

    public void SaveImages()
    {
        string newFolder = Application.persistentDataPath + "/CA Image Captures/" + System.DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + "/";
        Directory.CreateDirectory(newFolder);
        for(int i = 0; i < imageList.Count; ++i)
        {
            File.WriteAllBytes(newFolder + System.DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + " Run " + runs + "Iteration " + i + ".png", imageList[i]);
        }
    }

    public void BackgroundCheck()
    {
        if(backgroundCheckToggle.isOn == true)
        {
            Application.runInBackground = true;
        }

        if (backgroundCheckToggle.isOn == false)
        {
            Application.runInBackground = false;
        }
    }

    public void GridtoMain()
    {
        if (running == false)
        {
            editModeOn = false;
            ClearGrid();
            runs = 1;
        }
    }

    public void SaveIterationCount()
    {
        string newFolder = Application.persistentDataPath + "/CA Image Captures/" + System.DateTime.Now.ToString("yyyy-MM-dd") + "/";
        Directory.CreateDirectory(newFolder);
        using (StreamWriter wt = File.AppendText(newFolder + System.DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + " Run " + runs + ".txt"))
        {

            for (int i = 0; i < fullCount.Count; ++i)
            {
                wt.Write("Iteration: " + i);
                for (int j = 0; j < amountOfCellTypes; ++j)
                {
                    string cellTypeString = (j + 1).ToString();
                    wt.Write(" Cell Type " + cellTypeString + ": " + fullCount[i][j]);
                }
                wt.WriteLine();
            }
            wt.Close();
        }
    }

    public void ClearGrid()
    {
        Destroy(tex);
        Destroy(board);
        Destroy(sr);
        iterations = 0;
        ratios.Clear();
        colors.Clear();
        fullCount.Clear();
        imageList.Clear();
        //probabilities.Clear();
        myCA = null;
        alreadyCA = false;
    }

    public void ResetGrid()
    {
        editModeOn = false;
        if (running == false)
        {
            runs++;
            ClearGrid();
            CreateCA(mainPageInfo);
        }
    }

    public IEnumerator IterationAutomate()
    {
        if (iterationIFGO.GetComponent<Text>().text != "")
        {
            maxIterations = int.Parse(iterationIFGO.GetComponent<Text>().text);

            if (iterations == maxIterations)
            {
                PauseButton();
                running = false;
                if (saveImageToggle.isOn) SaveImage();
                if (saveCountToggle.isOn) SaveIterationCount();

                if (runCapToggle.isOn)
                {
                    maxRuns = int.Parse(runCapIFGO.GetComponent<Text>().text);
                    if (runs < maxRuns)
                    {
                        ResetGrid();
                        yield return new WaitForEndOfFrame();
                        StartCA();
                        PauseButton();
                    }
                    if (runs > maxRuns)
                    {
                        yield return new WaitForEndOfFrame();
                        iterationToggle.isOn = false;
                        yield return new WaitForEndOfFrame();
                        PauseButton();
                    }
                }
                if (runCapToggle.isOn == false)
                {
                    ResetGrid();
                    yield return new WaitForEndOfFrame();
                    StartCA();
                    PauseButton();
                }
            }
        }
    }

}