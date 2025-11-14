using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BTAction : BTNode
{
    private Func<NodeState> action;
    
    public BTAction(Func<NodeState> action) { this.action = action; }
    
    public override NodeState Evaluate()
    {
        state = action();
        return state;
    }
}
