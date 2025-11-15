using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTSequence : BTNode
{
    private List<BTNode> children = new List<BTNode>();
    
    public BTSequence(List<BTNode> nodes) { children = nodes; }
    
    public override NodeState Evaluate()
    {
        foreach (var child in children)
        {
            switch (child.Evaluate())
            {
                case NodeState.Running:
                    state = NodeState.Running;
                    return state;
                case NodeState.Failure:
                    state = NodeState.Failure;
                    return state;
            }
        }
        state = NodeState.Success;
        return state;
    }
}
