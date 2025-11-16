using UnityEngine;

public class StageClearTester : MonoBehaviour
{
    public StageClearRewardUI stageClearUI;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            stageClearUI.ShowReward();
        }
    }
}
