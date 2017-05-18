using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellState
{
    //prob of becoming state X, with Y neighbors of X
    float[,,] prob;

    public CellState( int totalStates, int neighborState, int neighborSize)
    {
        prob = new float[totalStates, neighborState, neighborSize + 1];
    }

    public void SetProbability(int state, int neighborState, int numNeighbors, float val)
    {
        prob[state, neighborState, numNeighbors] = val;
    }

    public float GetProbability(int state, int neighborState, int numNeighbors)
    {
        return prob[state, neighborState, numNeighbors];
    }
}
