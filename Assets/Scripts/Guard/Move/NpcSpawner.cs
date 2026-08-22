using System.Collections.Generic;
using UnityEngine;

public class NpcSpawner : MonoBehaviour
{
    public NpcManager npcManager;
    public List<GameObject> npcPrefabs; // kéo 6 prefab NPC khác nhau vào đây
    private Transform player;

    void Awake()
    {
        player = transform;
    }

    void Update()
    {
        // Test nhanh: phím 1-6 để spawn từng loại NPC tương ứng
        for (int i = 0; i < npcPrefabs.Count; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                SpawnNpc(npcPrefabs[i]);
            }
        }
    }

    public void SpawnNpc(GameObject prefab)
    {
        if (!npcManager.CanAddNpc())
        {
            Debug.Log("Không thể gọi thêm NPC");
            return;
        }

        GameObject obj = Instantiate(prefab, player.position, Quaternion.identity);
        NpcFollower npc = obj.GetComponent<NpcFollower>();
        npc.player = player;

        npcManager.AddNpc(npc);
    }
}