using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BTCondition : BTNode
{
    private Func<bool> condition;
    
    public BTCondition(Func<bool> condition) { this.condition = condition; }
    
    public override NodeState Evaluate()
    {
        state = condition() ? NodeState.Success : NodeState.Failure;
        return state;
    }
}
