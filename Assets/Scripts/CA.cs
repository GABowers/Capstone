using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Random = UnityEngine.Random;
using SysRand = System.Random;
//using System.Threading.Tasks;

public class CA
{
    private CellState[] states;
    private Neighborhood neighborhood;
    private GridType gridType;
    private int[,] grid;
    private int[,] backup;
    public int gridWidth;
    public int gridHeight;
    private int numStates;
    SysRand myRand;
    public static List<int> stateCount = new List<int>();
    public List<int> stateCountReplacement = new List<int>();
    public List<int> neighborCount = new List<int>();
    public List<float> neighborAnalysis = new List<float>();

    private StatePageInfo statePageInfo;

    public CA(int width, int height, int numStates, NType type, GridType gType = GridType.Box)
    {
        gridWidth = width;
        gridHeight = height;
        this.numStates = numStates;
        gridType = gType;
        grid = new int[width, height];
        backup = new int[width, height];
        neighborhood = new Neighborhood(type);
        states = new CellState[numStates];
        for (int i = 0; i < numStates; ++i)
        {
            stateCount.Add(0);
            states[i] = new CellState(numStates, numStates, neighborhood.GetNeighborSize());
        }
        myRand = new SysRand();


    }

    public void ChangeCell(int i, int j)
    {
        int currentState = grid[i, j];
        int newState = currentState + 1;
        Debug.Log(newState);
        if (newState >= numStates)
            newState = (newState - numStates);
        Debug.Log(newState);
        grid[i, j] = newState;
    }

    public void ModifiedOneIteration()
    {
        Array.Copy(grid, backup, gridWidth * gridHeight);
        for (int i = 0; i < numStates; ++i)
            stateCount[i] = 0;
    }

    public void InitializeGrid(List<float> ratios)
    {
        // make sure ratios is as big as our numStates
        while (ratios.Count < numStates)
            ratios.Add(0f);
        while (ratios.Count > numStates)
            ratios.Remove(ratios.Last());

        //Convert our list of float ratios to [0,1] percentages
        float total = ratios.Sum();
        for (int i = 0; i < ratios.Count; ++i)
            ratios[i] /= total;

        //Get a list of how many cells of each time we need
        List<int> cellCount = new List<int>();
        int totalCells = gridWidth * gridHeight;
        for (int i = 0; i < numStates; ++i)
            cellCount.Add((int)(ratios[i] * totalCells));

        // Because of rounding we might be a few from our total
        // we'll just add them to state 0. It will not be noticeable
        cellCount[0] += totalCells - cellCount.Sum();

        // this is a list of which states still needed to be added
        List<int> stateIndex = new List<int>();
        for (int i = 0; i < numStates; ++i)
            stateIndex.Add(i);

        float[] pickRange = GetPickRange(ratios);

        for (int k = 0; k < 5; ++k)
        {
            for (int i = 0; i < gridWidth; ++i)
            {
                for (int j = k; j < gridHeight; j += 5)
                {
                    // Get a random number for the states left to do and set it to the grid
                    // we can't just use the number directly becuase stateIndex
                    // could be 0,3,4  if we've finished setting all the 1s and 2s
                    int index = GetIndexFromRange(pickRange);
                    int state = stateIndex[index];
                    grid[i, j] = state;

                    // Subtract that state from its cellcount
                    // And if it goes to zero remove it from both our lists
                    cellCount[index]--;
                    if (cellCount[index] == 0)
                    {
                        cellCount.RemoveAt(index);
                        stateIndex.RemoveAt(index);
                        ratios.RemoveAt(index);
                        pickRange = GetPickRange(ratios);

                    }

                }
            }
        }
    }

    public void SetStateInfo(int startState, int endState, int neighborState, int numNeighbors, float prob)
    {
        states[startState].SetProbability(endState, neighborState, numNeighbors, prob);

    }

    public void SetCellState(int x, int y, int state)
    {
        grid[x, y] = state;
    }

    public int GetCellState(int x, int y)
    {
        return grid[x, y];
    }

    public void OneIteration()
    {
        Array.Copy(grid, backup, gridWidth * gridHeight);
        for (int i = 0; i < numStates; ++i)
            stateCount[i] = 0;
        for (int x = 0; x < gridWidth; ++x)
        {
            for (int y = 0; y < gridHeight; ++y)
            {
                int currentState = grid[x, y];
                List<int> neighborStateCount = GetNeighborCount(x, y);
                float[] probChances = GetProbChances(currentState, neighborStateCount);
                grid[x, y] = GetStateFromProbability(probChances);
                stateCount[currentState] += 1;
            }
        }
    }

    public void NeighborAnalysis()
    {
        for (int i = 0; i < numStates; ++i)
        {
            stateCountReplacement.Add(0);
            neighborCount.Add(0);
            neighborAnalysis.Add(0);
        }
        for (int x = 0; x < gridWidth; ++x)
        {
            for (int y = 0; y < gridHeight; ++y)
            {
                int currentState = grid[x, y];
                List<int> neighborStateCount = GetNeighborCount(x, y);
                stateCountReplacement[currentState] += 1;
                for (int i = 0; i < numStates; ++i)
                {
                    if (i == currentState)
                    {
                        for (int j = 0; j < neighborStateCount.Count(); ++j)
                        {
                            if (j == currentState)
                                continue;
                            int tempInt = neighborStateCount[j];
                            int tempInt2 = neighborCount[i];
                            neighborCount[i] = (tempInt + tempInt2);
                        }
                    }
                }
            }
        }
        for(int i = 0; i < numStates; ++i)
        {
            neighborAnalysis[i] = ((float)neighborCount[i] / (8 * stateCountReplacement[i]));
        }
    }

    private List<int> GetNeighborCount(int x, int y)
    {
        List<int> neighborCount = new List<int>();
        for (int i = 0; i < numStates; ++i)
            neighborCount.Add(0);
        List<Point> neighbors = neighborhood.GetNeighbors(x, y);

        //Get a count of each state in our neighborhood
        foreach (Point p in neighbors)
        {
            // We can't change the variable in a foreach iteration
            // So we make a copy
            Point modifiedP = p;

            // if modifiedP is not on the grid, adjust grid
            if (!modifiedP.WithinRange(gridWidth, gridHeight))
            {
                switch (gridType)
                {
                    case GridType.Box:
                        modifiedP = null; // make it null to skip it
                        break;
                    case GridType.CylinderW:
                        modifiedP = Point.AdjustCylinderW(gridWidth, modifiedP);
                        break;
                    case GridType.CylinderH:
                        modifiedP = Point.AdjustCylinderH(gridHeight, modifiedP);
                        break;
                    case GridType.Torus:
                        modifiedP = Point.AdjustTorus(gridWidth, gridHeight, modifiedP);
                        break;
                }
            }
            if (modifiedP == null)
                continue;
            neighborCount[backup[modifiedP.X, modifiedP.Y]]++;
        }
        return neighborCount;
    }

    private float[] GetProbChances(int currentState, List<int> neighborStateCount)
    {
        float[] probChances = new float[numStates];
        float totalProb = 0;

        for (int p = 0; p < numStates; ++p)
        {
            float prob = 0;
            //skip if we are on the state of the current cell
            //we'll figure it out after we find out all the other probs
            if (p == currentState)
                continue;

            //the neighbor probabilities of different states are considered ADDITIVE
            //They will combine to form one probability to that other state

            for (int nP = 0; nP < numStates; ++nP)
            {
                float tempProb = 0;
                if (nP == currentState)
                    continue;
                    tempProb = states[currentState].GetProbability((p), nP, neighborStateCount[nP]);
                prob += tempProb;
            }
            probChances[p] = prob;
            totalProb += prob;
        }
        // the chance of us not changing is the product of all the other states not happening
        float noChange = 1 - totalProb;
        probChances[currentState] = noChange > 0 ? noChange : 0;
        return probChances;
    }

    private int GetStateFromProbability(float[] probChances)
    {
        float[] pickRange = GetPickRange(probChances.ToList());
        return GetIndexFromRange(pickRange);
    }

    private int GetIndexFromRange(float[] pickRange)
    {
        float range = pickRange[pickRange.Length - 1];
        float pick = (float)myRand.NextDouble() * range;
        return Array.IndexOf(pickRange, pickRange.Where(x => x >= pick).First());
    }

    private float[] GetPickRange(List<float> prob)
    {
        float[] pickRange = new float[prob.Count];
        float total = 0;
        for (int i = 0; i < prob.Count; ++i)
        {
            total += prob[i];
            pickRange[i] = total;
        }
        return pickRange;
    }
}

public enum GridType
{
    Box,
    CylinderW,
    CylinderH,
    Torus
}