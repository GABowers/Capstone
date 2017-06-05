using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstController : MonoBehaviour {

    public Canvas mainPageCanvas;
    public Canvas cellPageCanvas;
    public Canvas caPageCanvas;
    public MainPageController mainPageController;
    public StatePageController cellPageController;

    public MainPageInfo mainPageInfo;
    CA myCA;
    public List<StatePageInfo> statePageInfo;

    public FirstOrderControllerScript runCA;

    public MainPageInfo MainPageInfo
    {
        get { return mainPageInfo; }
        set { mainPageInfo = value; }
    }

    private void Awake()
    {
        mainPageInfo = new MainPageInfo();
        statePageInfo = new List<StatePageInfo>();
    }
    
	void Start () {
        CheckSavedState();
        mainPageCanvas.gameObject.SetActive(true);
        mainPageController.SetInfo(mainPageInfo);
	}
	
	void Update () {
	}

    private void CheckSavedState()
    {
        //Check JSON file
    }

    public void MainPageNext()
    {
        if (CheckMainPageInfo() == false)
        {
            //Jump to some error page
            return;
        }
        // if we have the right number of states already
        // because we backed out of them or loaded them
        // skip resetting the statePageInfo
        if (mainPageInfo.numStates != statePageInfo.Count)
            SetupStateInfo();
        mainPageCanvas.gameObject.SetActive(false);
        cellPageCanvas.gameObject.SetActive(true);
        cellPageController.SetInfo(statePageInfo[0]);
    }

    public void CellPageBack(int state)
    {
        // Go back to main page
        if (state == 1)
        {
            cellPageCanvas.gameObject.SetActive(false);
            mainPageCanvas.gameObject.SetActive(true);
            mainPageController.SetInfo(mainPageInfo);
        }

        else
            // state is +1 from index. So -1 is current index, -2 is previous page index
            cellPageController.SetInfo(statePageInfo[state - 2]);
    }

    public void CellPageNext(int state)
    {
        // If we are at the last state start the CA
        if (state == mainPageInfo.numStates)
        {
            if (runCA.alreadyCA == false)
            {
                // All the code to set up the CA fo here
                // and the variables to tell Update to start iterating
                cellPageCanvas.gameObject.SetActive(false);
                caPageCanvas.gameObject.SetActive(true);
                runCA.CreateCA(mainPageInfo);
            }
            if(runCA.alreadyCA == true)
            {
                cellPageCanvas.gameObject.SetActive(false);
                caPageCanvas.gameObject.SetActive(true);
                runCA.ResumeCA();
            }
        }

        else
            cellPageController.SetInfo(statePageInfo[state]); // Our state is +1 from our index.
    }

    public void CAEditSettings()
    {
        caPageCanvas.gameObject.SetActive(false);
        CheckSavedState();
        cellPageCanvas.gameObject.SetActive(true);
        cellPageController.SetInfo(statePageInfo[0]);
    }

    public void CAtoMain()
    {
        caPageCanvas.gameObject.SetActive(false);
        CheckSavedState();
        mainPageCanvas.gameObject.SetActive(true);
        mainPageController.SetInfo(mainPageInfo);
    }

    private bool CheckMainPageInfo()
    {
        bool isOK = true;
        if (mainPageInfo.numStates.HasValue == false)
            isOK = false;
        if (mainPageInfo.gridWidth.HasValue == false)
            isOK = false;
        if (mainPageInfo.gridHeight.HasValue == false)
            isOK = false;
        return isOK;
    }

    private void SetupStateInfo()
    {
        // Clear out old state pages
        statePageInfo.Clear();
        int neighbors;
        switch (mainPageInfo.nType)
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

        for (int i = 0; i < mainPageInfo.numStates; ++i)
        {
            StatePageInfo current = new StatePageInfo(mainPageInfo.numStates.Value, neighbors, i + 1);
            statePageInfo.Add(current);
        }
    }
}
