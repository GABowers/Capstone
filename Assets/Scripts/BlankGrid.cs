using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlankGrid {
    public bool containsAgent = false;
    public AgentController agent;

	// Use this for initialization
	void Start () {
		
	}

    public bool ContainsAgent
    {
        get { return containsAgent; }
        set { containsAgent = value; }
    }

    //public AgentController AgentController
    //{
    //    get { return agent; }
    //    set { agent = value; }
    //}

    public void AddAgent()
    {
        agent = new AgentController();
    }

    //public BlankGrid()
    //{

    //}
}
