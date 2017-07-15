using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Random = UnityEngine.Random;
using SysRand = System.Random;
using System.IO;
//using System.Threading.Tasks;

public class CA
{
    List<AgentController> activeAgents = new List<AgentController>();
    System.Object[,] objects;
    private BlankGrid blankGrid;
    //AgentController agent;
    private CellState[] states;
    private Neighborhood neighborhood;
    private GridType gridType;
    public BlankGrid[,] grid;
    public BlankGrid[,] backup;
    public int gridWidth;
    public int gridHeight;
    private int numStates;
    public int agentLocation;
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
        grid = new BlankGrid[width, height];
        //grid = new int[width, height];
        backup = new BlankGrid[width, height];
        neighborhood = new Neighborhood(type);
        states = new CellState[numStates];
        for (int i = 0; i < numStates; ++i)
        {
            stateCount.Add(0);
            states[i] = new CellState(numStates, numStates, neighborhood.GetNeighborSize());
        }
        myRand = new SysRand();
    }

    //public void ChangeCell(int i, int j)
    //{
    //    //
    //    // THIS NEEDS FIXED. Needs to update agent's state correctly. OR, needs to create agent of the first "state."
    //    //
    //    //khgugyjj;
    //    if (grid[i, j].containsAgent == true)
    //    {
    //        int currentState = grid[i, j].agent.currentState;
    //        int newState = currentState + 1;
    //        Debug.Log(newState);
    //        if (newState >= numStates)
    //            newState = (newState - numStates);
    //        Debug.Log(newState);
    //        grid[i, j].agent.currentState = newState;
    //    }

    //else
    //    {

    //    }
    //}

    /*public void OriginalInitializeGrid(List<int> ratios)
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

        float[] agentAmountPerState = GetAgentAmountPerState(ratios);

        for (int k = 0; k < 5; ++k)
        {
            for (int i = 0; i < gridWidth; ++i)
            {
                for (int j = k; j < gridHeight; j += 5)
                {
                    // Get a random number for the states left to do and set it to the grid
                    // we can't just use the number directly becuase stateIndex
                    // could be 0,3,4  if we've finished setting all the 1s and 2s
                    int index = GetIndexFromRange(agentAmountPerState);
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
                        agentAmountPerState = GetAgentAmountPerState(ratios);

                    }

                }
            }
        }
    }*/

    public void InitializeGrid(List<float> ratios)
    {
        List<int> rand2CurrentMins = new List<int>();
        myRand = new SysRand();
        // This method creates the grid after the user inputs the necessary initial and transition information

        // make sure ratios is as big as our numStates
        while (ratios.Count < numStates)
            ratios.Add(0);
        while (ratios.Count > numStates)
            ratios.Remove(ratios.Last());

        //Get a list of how many cells of each type we need
        List<int> agentCount = new List<int>();
        //int totalCells = gridWidth * gridHeight;
        int totalAgents = 0;
        for (int i = 0; i < numStates; ++i)
        {
            agentCount.Add((int)ratios[i]);
            totalAgents += (int)ratios[i];
        }

        // this is a list of which states still needed to be added
        List<int> stateIndex = new List<int>();
        for (int i = 0; i < numStates; ++i)
            stateIndex.Add(i);

        float[] agentAmountPerState = GetAgentAmountPerState(ratios);

        List<List<int>> gridList = new List<List<int>>();

        for (int i = 0; i < gridWidth; i++)
        {
            gridList.Add(new List<int>());
            rand2CurrentMins.Add(gridWidth);
            for (int j = 0; j < gridHeight; j++)
            {
                gridList[i].Add(j);
            }
        }
        int rand1CurrentMin = gridWidth;
            while (agentCount.Any())
            {
                int rand1 = myRand.Next(0, gridList.Count);
                int rand2 = myRand.Next(0, gridList[rand1].Count);
                int index = GetIndexFromRange(agentAmountPerState);
                int state = stateIndex[index];
                int newRand1 = rand1;

                if (rand1 >= rand1CurrentMin)
                {
                        newRand1 = rand1 + (gridWidth - gridList.Count);
                }
                grid[newRand1, gridList[rand1].ElementAt<int>(rand2)] = new BlankGrid();
                grid[newRand1, gridList[rand1].ElementAt<int>(rand2)].AddAgent();
                grid[newRand1, gridList[rand1].ElementAt<int>(rand2)].containsAgent = true;
                grid[newRand1, gridList[rand1].ElementAt<int>(rand2)].agent.currentState = state;
                grid[newRand1, gridList[rand1].ElementAt<int>(rand2)].agent.xLocation = rand1;
                grid[newRand1, gridList[rand1].ElementAt<int>(rand2)].agent.yLocation = rand2;

                activeAgents.Add(grid[newRand1, gridList[rand1].ElementAt<int>(rand2)].agent);

                // Subtract that state from its agentcount
                // And if it goes to zero remove it from both our lists
                agentCount[index]--;
                if (agentCount[index] == 0)
                {
                    agentCount.RemoveAt(index);
                    stateIndex.RemoveAt(index);
                    ratios.RemoveAt(index);
                    agentAmountPerState = GetAgentAmountPerState(ratios);
                }

                gridList[rand1].RemoveAt(rand2);
                if (rand2 < rand2CurrentMins[rand1])
                {
                    rand2CurrentMins[rand1] = rand2;
                }

                if (gridList[rand1].Any() == false)
                {
                    gridList.RemoveAt(rand1);
                    if (rand1 < rand1CurrentMin)
                    {
                        rand1CurrentMin = rand1;
                    }
                }
            }

            {
                for (int i = 0; i < gridWidth; ++i)
                {
                    for (int j = 0; j < gridHeight; ++j)
                    {
                        if (System.Object.ReferenceEquals(grid[i, j], null))
                        {
                            grid[i, j] = new BlankGrid();
                            grid[i, j].containsAgent = false;
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
        grid[x, y].agent.currentState = state;
    }

    public int GetCellState(int x, int y)
    {
        //This should ONLY fire if the grid point contains an agent.
        return grid[x, y].agent.currentState;
    }

    public void OneIteration()
    {
        //Debug.Log(activeAgents.Count);
        Array.Copy(grid, backup, gridWidth * gridHeight);
        for (int i = 0; i < numStates; ++i)
            stateCount[i] = 0;
        for (int x = 0; x < activeAgents.Count; ++x)
        {
            agentLocation = x;
            AgentMove(activeAgents[x]);
        }
        //HUGE HUGE HUGE PROBLEM. Agents appearing from nowhere. This needs to be fixed!
        RemoveBadAgents();
        //This ^^^ function is ONLY a temp stop-gap measure!
        //Not sure if this should be here
        //GC.Collect();
        //Debug.Log(activeAgents.Count);
    }

    public void ModifiedOneIteration()
    {
        //Use this for adding multi-threading
        Array.Copy(grid, backup, gridWidth * gridHeight);
        for (int i = 0; i < numStates; ++i)
            stateCount[i] = 0;
        for (int x = 0; x < gridWidth; ++x)
        {
            for (int y = 0; y < gridHeight; ++y)
            {
                if (grid[x, y].containsAgent == true)
                {
                    int currentState = grid[x, y].agent.currentState;
                    List<int> neighborStateCount = GetNeighborCount(x, y);
                    float[] probChances = GetProbChances(currentState, neighborStateCount);
                    grid[x, y].agent.currentState = GetStateFromProbability(probChances);
                    stateCount[currentState] += 1;
                }
            }
        }
    }

    public void NeighborAnalysis()
    {
    //
    // This is DAN'S NEIGHBOR ANALYSIS or DNA. Will need to be re-incorporated.
    //
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
                int currentState = grid[x, y].agent.currentState;
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
            neighborCount.Add(new int());
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
            neighborCount[backup[modifiedP.X, modifiedP.Y].agent.currentState]++;
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
        //Should this be float or int?
        float[] agentAmountPerState = GetAgentAmountPerState(probChances.ToList());
        return GetIndexFromRange(agentAmountPerState);
    }

    private int GetIndexFromRange(float[] agentAmountPerState)
    {
        float range = agentAmountPerState[agentAmountPerState.Length - 1];
        float pick = (float)myRand.NextDouble() * range;
        return Array.IndexOf(agentAmountPerState, agentAmountPerState.Where(x => x >= pick).First());
    }

    private float[] GetAgentAmountPerState(List<float> ratios)
    {
        float[] agentAmountPerState = new float[ratios.Count];
        float total = 0;
        for (int i = 0; i < ratios.Count; ++i)
        {
            total += ratios[i];
            //CHANGE HERE to move from an additive count (???) to one that gets the number for each individual state
            agentAmountPerState[i] = ratios[i];
        }
        return agentAmountPerState;
    }

    private void AgentMove(AgentController currentAgent)
    {
        //NEED to check things. Need to make check for probability of Random walk itself (encapsulate in IF). Then, check if the new place is already filled.
        //Debug.Log("Old: " + currentAgent.xLocation + ", " + currentAgent.yLocation);
        int newX = currentAgent.xLocation;
        int newY = currentAgent.yLocation;
        int oldX = currentAgent.xLocation;
        int oldY = currentAgent.yLocation;
        double upProb = 0.2;
        double rightProb = 0.2;
        double downProb = 0.4;
        double leftProb = 0.2;
        double randomWalk = myRand.NextDouble();
        if (randomWalk < upProb)
        {
            newY = currentAgent.yLocation + 1;
        }
        else if (randomWalk < (rightProb + upProb) && randomWalk > upProb)
        {
            newX = currentAgent.xLocation + 1;
        }
        else if (randomWalk < (downProb + rightProb + upProb) && randomWalk > (rightProb + upProb))
        {
            newY = currentAgent.yLocation - 1;
        }
        else
        {
            newX = currentAgent.xLocation - 1;
        }
        if (newX > (gridWidth - 1) || newY > (gridHeight - 1) || newX < 0 || newY < 0)
        {
            switch (gridType)
            {
                case GridType.Box:
                    newX = currentAgent.xLocation;
                    newY = currentAgent.yLocation;
                    break;
                case GridType.CylinderW:
                    if (newX >= gridWidth)
                    {
                        newX -= gridWidth;
                        newY = currentAgent.yLocation;
                    }
                    else
                    {
                        newX += gridWidth;
                        newY = currentAgent.yLocation;
                    }
                    break;
                case GridType.CylinderH:
                    if (newY >= gridHeight)
                    {
                        newX = currentAgent.xLocation;
                        newY -= gridHeight;
                    }
                    else
                    {
                        newX = currentAgent.xLocation;
                        newY += gridHeight;
                    }
                    break;
                case GridType.Torus:
                    {
                        if (newX >= gridWidth)
                        {
                            newX -= gridWidth;
                        }
                        else
                        {
                            newX += gridWidth;
                        }
                    }
                    {
                        if (newY >= gridHeight)
                        {
                            newY -= gridHeight;
                        }
                        else
                        {
                            newY += gridHeight;
                        }
                    }
                    break;
            }
        }
        if (newX != oldX || newY != oldY)
        {
            if (grid[newX, newY].containsAgent == false && System.Object.ReferenceEquals(grid[newX, newY].agent, null) == true)
            {
                grid[newX, newY].AddAgent();
                grid[newX, newY].containsAgent = true;
                grid[newX, newY].agent.currentState = currentAgent.currentState;
                grid[newX, newY].agent.xLocation = newX;
                grid[newX, newY].agent.yLocation = newY;
                activeAgents[agentLocation] = grid[newX, newY].agent;
                //Debug.Log("New: " + grid[newX, newY].agent.xLocation + ", " + grid[newX, newY].agent.yLocation);
                grid[oldX, oldY].containsAgent = false;
                grid[oldX, oldY].agent = null;
            }
        }
    }

    private void RemoveBadAgents()
    {
        int agentCleanup = 0;
        for (int i = 0; i < gridWidth; i++)
        {
            for (int j = 0; j < gridHeight; j++)
            {
                if (activeAgents.Contains(grid[i, j].agent) == false && grid[i, j].containsAgent == true)
                {
                    grid[i, j].agent = null;
                    grid[i, j].containsAgent = false;
                    agentCleanup++;
                }
            }
        }
        Debug.Log("Removed Agents: " + agentCleanup);
    }
}

public enum GridType
{
    Box,
    CylinderW,
    CylinderH,
    Torus
}