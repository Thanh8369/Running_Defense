using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTSelector : BTNode
{
    private List<BTNode> children = new List<BTNode>();
    
    public BTSelector(List<BTNode> nodes) { children = nodes; }
    
    public override NodeState Evaluate()
    {
        foreach (var child in children)
        {
            switch (child.Evaluate())
            {
                case NodeState.Running:
                    state = NodeState.Running;
                    return state;
                case NodeState.Success:
                    state = NodeState.Success;
                    return state;
            }
        }
        state = NodeState.Failure;
        return state;
    }
}
