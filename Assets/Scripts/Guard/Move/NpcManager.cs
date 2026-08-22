using System.Collections.Generic;
using UnityEngine;

public class NpcManager : MonoBehaviour
{
    public int maxSlots = 6;
    private int currentCount = 0;

    public bool CanAddNpc()
    {
        return currentCount < maxSlots;
    }

    public bool AddNpc(NpcFollower npc)
    {
        if (currentCount >= maxSlots)
        {
            Debug.Log("Đội đã đầy 6/6");
            return false;
        }

        npc.slotIndex = currentCount;
        npc.totalSlots = maxSlots; // vẫn giữ = 6 để formation chia đều 6 hướng dù chưa đủ 6 con
        currentCount++;
        return true;
    }
}