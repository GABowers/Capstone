using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class Buttons : MonoBehaviour {

    JSONStorage jsonStorage;
    NType neighborhoodType;
    CanvasGroup[] panels;
    CanvasGroup[] modelScenePanels;
    public GameObject mmp;
    public GameObject nmp;
    public Dropdown neighborEffectsDropdown;
    //public int neighborhoodEffectDropdownValue;
    public Text numbercelltype;
    public GameObject nmpcell;
    public GameObject probinputfield;
    Text probvalue;
    Text probtitle;
    GameObject[] celltitles;
    GameObject[] probtitles;
    public GameObject nmpnow;
    GameObject confirm;
    GameObject designate;
    GameObject saveDialog;
    public int pageCounter = 10;
    Text NMP1pageTitle;
    Text NMP1probTitle;
    public int pageMax;
    public int pageMin;
    bool applyActive = false;
    GameObject mainCanvas;
    public Text pauseButtonText;
    public bool continueCA = false;
    FirstOrderControllerScript FirstOrderControllerScript;

    public float countDiff;
    bool gridActive;

    GameObject gridpanel;
    GameObject[] cellpages;
    GameObject[] inputfields;

    public int surroundingCells;
    public int toProbCell;
    // Use this for initialization
    void Start ()
    {
        gridActive = false;
        gridpanel = GameObject.Find("GridPanel");
        confirm = GameObject.FindGameObjectWithTag("ConfirmBox");
        designate = GameObject.FindGameObjectWithTag("DesignateCellTypes");
        saveDialog = GameObject.FindGameObjectWithTag("SaveFileNamePanel");
        jsonStorage = GameObject.Find("StorageGO").GetComponent<JSONStorage>();
        FirstOrderControllerScript = GameObject.Find("FirstOrderGO").GetComponent<FirstOrderControllerScript>();
        neighborEffectsDropdown = GameObject.FindGameObjectWithTag("NeighborhoodDropdown").GetComponent<Dropdown>();
        probinputfield = GameObject.FindGameObjectWithTag("ProbInputField");
        pageMin = 11;
        panels = GameObject.Find("Canvas").GetComponentsInChildren<CanvasGroup>();
        foreach (CanvasGroup panel in panels)
        {
            panel.alpha = 0;
            panel.interactable = false;
            panel.blocksRaycasts = false;
        }
        gridpanel = GameObject.Find("GridPanel");
        {
            gridpanel.GetComponent<CanvasGroup>().alpha = 0;
            gridpanel.GetComponent<CanvasGroup>().interactable = false;
            gridpanel.GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
        mmp = GameObject.FindGameObjectWithTag("MainMenuPanel");
        mmp.GetComponent<CanvasGroup>().alpha = 1;
        mmp.GetComponent<CanvasGroup>().interactable = true;
        mmp.GetComponent<CanvasGroup>().blocksRaycasts = true;
    }
	
    public void PageCount ()
    {
        if(applyActive == true)
        {
            GameObject[] inputFields = GameObject.FindGameObjectsWithTag("InstantiatedInputFields");
            GameObject[] cellPages = GameObject.FindGameObjectsWithTag("InstantiatedCellPages");
            foreach (GameObject inputField in inputFields)
            {
                GameObject.Destroy(inputField);
            }
            foreach (GameObject cellPage in cellPages)
            {
                GameObject.Destroy(cellPage);
            }
        }
            
            pageCounter = 10;
            pageMin = 11;
    }

	// Update is called once per frame
	void Update ()
    {   
    }

    public void StartNewModel()
    {
        panels = GameObject.Find("Canvas").GetComponentsInChildren<CanvasGroup>();
        foreach (CanvasGroup panel in panels)
        {
            panel.alpha = 0;
            panel.interactable = false;
            panel.blocksRaycasts = false;
        }
        nmp = GameObject.FindGameObjectWithTag("NewModelPanel");
        nmp.GetComponent<CanvasGroup>().alpha = 1;
        nmp.GetComponent<CanvasGroup>().interactable = true;
        nmp.GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    public void ApplyNumber()
    {
        
        probinputfield.SetActive(true);
        mainCanvas = GameObject.Find("Canvas");
        mainCanvas.transform.Find("NMP10").gameObject.SetActive(true);
        int x = 0;
        switch(neighborEffectsDropdown.value)
        {
            case 0:
                x = 1;
                neighborhoodType = NType.None;
                break;
            case 1:
                x = 5;
                neighborhoodType = NType.VonNeumann;
                break;
            case 2:
                x = 9;
                neighborhoodType = NType.Moore;
                break;
            case 3:
                x = 13;
                neighborhoodType = NType.Hybrid;
                break;
        }
        jsonStorage.neighborhoodType = neighborhoodType;
        {
            {
                GameObject[] inputFields = GameObject.FindGameObjectsWithTag("InstantiatedInputFields");
                GameObject[] cellPages = GameObject.FindGameObjectsWithTag("InstantiatedCellPages");
                foreach (GameObject inputField in inputFields)
                {
                    GameObject.Destroy(inputField);
                }
                foreach (GameObject cellPage in cellPages)
                {
                    GameObject.Destroy(cellPage);
                }
            }
            applyActive = true;
            GameObject nmpcellclone;
            GameObject probinputfieldclone;
            nmpcell = GameObject.Find("NMP10");
            numbercelltype = GameObject.FindGameObjectWithTag("numbercelltype").GetComponent<Text>();
            pageMax = ((int.Parse(numbercelltype.text)) + pageCounter);
            for (int pageNumber = 0; pageNumber < (int.Parse(numbercelltype.text) - 1); pageNumber++)
            {
                for (int i = 0; i < x; ++i)
                {
                    probinputfieldclone = Instantiate(probinputfield);
                    probinputfieldclone.transform.SetParent(GameObject.Find("1stProbContent").transform, true);
                    probinputfieldclone.transform.position = new Vector3(probinputfield.transform.position.x, probinputfield.transform.position.y - 20 - ((pageNumber * 200) + (i * 40)), probinputfield.transform.position.z);
                    probinputfieldclone.tag = ("InstantiatedInputFields");
                    probinputfieldclone.name = ("ProbInputField" + "1." + (pageNumber + 1) + "." + i);
                    probinputfieldclone.transform.localScale = new Vector3(1f, 1f, 1f);
                    //probinputfieldclone.transform.Find("ProbTransText").GetComponent<Text>().resizeTextForBestFit = true;
                }
            }
            probinputfield.SetActive(false);
            for (int pageNumber = 1; pageNumber < (int.Parse(numbercelltype.text) + 1); pageNumber++)
            {
                nmpcellclone = Instantiate(nmpcell);
                nmpcellclone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                nmpcellclone.tag = ("InstantiatedCellPages");
                nmpcellclone.name = ("NMP" + "1" + pageNumber);
                probtitles = GameObject.FindGameObjectsWithTag("ProbTitle");
                foreach (GameObject probtitle in probtitles)
                {
                    surroundingCells = int.Parse(probtitle.transform.parent.name.Remove(0, 18));
                    string toProbCellPre = probtitle.transform.parent.name.Remove(0, 16);
                    toProbCell = int.Parse(toProbCellPre.Remove(1, (toProbCellPre.Length - 1)));
                    int fromProbCell = int.Parse(probtitle.transform.parent.parent.parent.parent.name.Remove(0, 4));
                    NMP1probTitle = probtitle.GetComponent<Text>();
                    if (fromProbCell == toProbCell)
                    {
                        NMP1probTitle.text = "Transformation Probability (" + fromProbCell + " to " + (toProbCell + 1) + "), with " + surroundingCells + " surrounding type " + (toProbCell + 1) + " cell(s)";
                    }
                    if (fromProbCell < toProbCell)
                    {
                        NMP1probTitle.text = "Transformation Probability (" + fromProbCell + " to " + (toProbCell + 1) + "), with " + surroundingCells + " surrounding type " + (toProbCell + 1) + " cell(s)";
                    }
                    if (fromProbCell > toProbCell)
                    {
                        NMP1probTitle.text = "Transformation Probability (" + fromProbCell + " to " + toProbCell + "), with " + surroundingCells + " surrounding type " + toProbCell + " cell(s)";
                    }
                }
            }
            celltitles = GameObject.FindGameObjectsWithTag("CellTitle");
            foreach (GameObject celltitle in celltitles)
            {
                NMP1pageTitle = celltitle.GetComponent<Text>();
                NMP1pageTitle.text = "Cell Type " + celltitle.transform.parent.name.Remove(0, 4);
            }
        }
    }

    public void Back1()
    {
        nmpnow = GameObject.Find("NMP" + (pageCounter - 1));
        panels = GameObject.Find("Canvas").GetComponentsInChildren<CanvasGroup>();
        foreach (CanvasGroup panel in panels)
        {
            panel.alpha = 0;
            panel.interactable = false;
            panel.blocksRaycasts = false;
        }
        if (pageCounter > pageMin)
        {
            nmpnow.GetComponent<CanvasGroup>().alpha = 1;
            nmpnow.GetComponent<CanvasGroup>().interactable = true;
            nmpnow.GetComponent<CanvasGroup>().blocksRaycasts = true;
            pageCounter -= 1;
        }
        else if(pageCounter == pageMin)
        {
            probinputfield.SetActive(true);
            nmp = GameObject.FindGameObjectWithTag("NewModelPanel");
            nmp.GetComponent<CanvasGroup>().alpha = 1;
            nmp.GetComponent<CanvasGroup>().interactable = true;
            nmp.GetComponent<CanvasGroup>().blocksRaycasts = true;
            pageCounter -= 1;
        }
        else if(pageCounter < pageMin)
        {
            mmp = GameObject.FindGameObjectWithTag("MainMenuPanel");
            mmp.GetComponent<CanvasGroup>().alpha = 1;
            mmp.GetComponent<CanvasGroup>().interactable = true;
            mmp.GetComponent<CanvasGroup>().blocksRaycasts = true;
        }
    }
    public void Next1()
    {
        countDiff = (pageMax - pageCounter);
        panels = GameObject.Find("Canvas").GetComponentsInChildren<CanvasGroup>();
        foreach (CanvasGroup panel in panels)
        {
            panel.alpha = 0;
            panel.interactable = false;
            panel.blocksRaycasts = false;
        }
        if (pageCounter == pageMax)
        {
            confirm.GetComponent<CanvasGroup>().alpha = 1;
            confirm.GetComponent<CanvasGroup>().interactable = true;
            confirm.GetComponent<CanvasGroup>().blocksRaycasts = true;
        }
        else if(pageMax < pageMin)
        {
            designate.GetComponent<CanvasGroup>().alpha = 1;
            designate.GetComponent<CanvasGroup>().interactable = true;
            designate.GetComponent<CanvasGroup>().blocksRaycasts = true;
        }
        else if (pageCounter < pageMax)
        {
            nmpnow = GameObject.Find("NMP" + (pageCounter + 1));
            nmpnow.GetComponent<CanvasGroup>().alpha = 1;
            nmpnow.GetComponent<CanvasGroup>().interactable = true;
            nmpnow.GetComponent<CanvasGroup>().blocksRaycasts = true;
            pageCounter += 1;
        }
    }

    public void ConfirmYes()
    {
        nmpnow = GameObject.Find("NMP" + pageCounter);
        foreach (CanvasGroup panel in panels)
        {
            panel.alpha = 0;
            panel.interactable = false;
            panel.blocksRaycasts = false;
        }
        {
            gridpanel.GetComponent<CanvasGroup>().alpha = 1;
            gridpanel.GetComponent<CanvasGroup>().interactable = true;
            gridpanel.GetComponent<CanvasGroup>().blocksRaycasts = true;
        }
        //FirstOrderControllerScript.CreateCA(main);
        gridActive = true;
    }

    public void StartCAButton()
    {
        FirstOrderControllerScript.StartCA();
    }
    
    public void PauseButton()
    {
        continueCA = !continueCA;
        pauseButtonText.text = continueCA ? "UnPaused" : "Paused";
    }

    public void GridtoMain()
    {
        if (FirstOrderControllerScript.running == false)
        {
            mainCanvas = GameObject.Find("Canvas");
            cellpages = GameObject.FindGameObjectsWithTag("InstantiatedCellPages");
            inputfields = GameObject.FindGameObjectsWithTag("InstantiatedInputFields");
            probinputfield.SetActive(true);
            pageCounter = 10;
            pageMin = 11;
            pageMax = 0;
            foreach (GameObject cellpage in cellpages)
            {
                Destroy(cellpage);
            }
            foreach (GameObject inputfield in inputfields)
            {
                Destroy(inputfield);
            }
            gridpanel = GameObject.Find("GridPanel");
            {
                gridpanel.GetComponent<CanvasGroup>().alpha = 0;
                gridpanel.GetComponent<CanvasGroup>().interactable = false;
                gridpanel.GetComponent<CanvasGroup>().blocksRaycasts = false;
            }
            {
                mmp = GameObject.FindGameObjectWithTag("MainMenuPanel");
                mmp.GetComponent<CanvasGroup>().alpha = 1;
                mmp.GetComponent<CanvasGroup>().interactable = true;
                mmp.GetComponent<CanvasGroup>().blocksRaycasts = true;
            }
            mainCanvas.transform.Find("NMP10").gameObject.SetActive(true);
            FirstOrderControllerScript.ClearGrid();
            FirstOrderControllerScript.runs = 1;
        }
        gridActive = false;
    }

    public void SaveImage()
    {
        FirstOrderControllerScript.SaveImage();
    }

    public void SaveImages()
    {
        FirstOrderControllerScript.SaveImages();
    }

    public void SaveCellCount()
    {
        FirstOrderControllerScript.SaveIterationCount();
    }

    public void ResetGrid()
    {
        FirstOrderControllerScript.ResetGrid();
    }

    public void ConfirmNo()
    {
        nmpnow = GameObject.Find("NMP" + pageCounter);
        foreach (CanvasGroup panel in panels)
        {
            panel.alpha = 0;
            panel.interactable = false;
            panel.blocksRaycasts = false;
        }
        {
            nmpnow.GetComponent<CanvasGroup>().alpha = 1;
            nmpnow.GetComponent<CanvasGroup>().interactable = true;
            nmpnow.GetComponent<CanvasGroup>().blocksRaycasts = true;
        }
    }
    
    public void Okay()
    {
        foreach (CanvasGroup panel in panels)
        {
            panel.alpha = 0;
            panel.interactable = false;
            panel.blocksRaycasts = false;
        }
        nmp = GameObject.FindGameObjectWithTag("NewModelPanel");
        nmp.GetComponent<CanvasGroup>().alpha = 1;
        nmp.GetComponent<CanvasGroup>().interactable = true;
        nmp.GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void SaveFile()
    {
        if (gridActive == true)
        {
            gridpanel.GetComponent<CanvasGroup>().interactable = false;
            gridpanel.GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
        if (gridActive == false)
        {
            foreach (CanvasGroup panel in panels)
            {
                panel.interactable = false;
                panel.blocksRaycasts = false;
            }
        }

        saveDialog.GetComponent<CanvasGroup>().alpha = 1;
        saveDialog.GetComponent<CanvasGroup>().interactable = true;
        saveDialog.GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    public void SaveFileDone()
    {
        //FirstOrderControllerScript.SaveSettings();
        if (gridActive == true)
        {
            gridpanel.GetComponent<CanvasGroup>().interactable = true;
            gridpanel.GetComponent<CanvasGroup>().blocksRaycasts = true;
        }
        if (gridActive == false)
        {
            foreach (CanvasGroup panel in panels)
            {
                panel.interactable = true;
                panel.blocksRaycasts = true;
            }
        }

        saveDialog.GetComponent<CanvasGroup>().alpha = 0;
        saveDialog.GetComponent<CanvasGroup>().interactable = false;
        saveDialog.GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void SaveFileCancel()
    {
        if (gridActive == true)
        {
            gridpanel.GetComponent<CanvasGroup>().interactable = true;
            gridpanel.GetComponent<CanvasGroup>().blocksRaycasts = true;
        }
        if (gridActive == false)
        {
            foreach (CanvasGroup panel in panels)
            {
                panel.interactable = true;
                panel.blocksRaycasts = true;
            }
        }

        saveDialog.GetComponent<CanvasGroup>().alpha = 0;
        saveDialog.GetComponent<CanvasGroup>().interactable = false;
        saveDialog.GetComponent<CanvasGroup>().blocksRaycasts = false;
    }
}