using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatePageInfo {
    public int stateNum;
    public int color;
    public int? startingAmount;
    int neighborState;
    public float?[,,] probs; // [x,y] prob to go to state x with y neighbors

    public StatePageInfo(int totalStates, int neighbors, int currentState)
    {
        stateNum = currentState;
        color = 0;
        startingAmount = 0;
        probs = new float?[totalStates, totalStates, neighbors + 1];
    }

    /*public void SetProbability(int state, int numNeighbors, float val)
    {
        probs[state, neighborState, numNeighbors] = val;
    }

    public float GetProbability(int state, int numNeighbors)
    {
        return probs[state, neighborState, numNeighbors];
    }*/
}
