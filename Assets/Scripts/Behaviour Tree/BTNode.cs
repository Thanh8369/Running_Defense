using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BTNode
{
    public enum NodeState { Running, Success, Failure }
    protected NodeState state;
    
    public NodeState State => state;
    public abstract NodeState Evaluate();
}
