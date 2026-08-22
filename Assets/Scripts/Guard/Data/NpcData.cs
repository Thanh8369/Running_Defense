using UnityEngine;

[CreateAssetMenu(fileName = "NewNpcData", menuName = "NPC/Npc Data")]
public class NpcData : ScriptableObject
{
    [Header("Thông tin cơ bản")]
    public string npcName;
    public GameObject prefab;
    public Sprite icon; // dùng cho UI chọn NPC (nếu có màn hình chọn khi level up)

    [Header("Role")]
    public NpcRole role;

    [Header("Chỉ số cơ bản (base stats)")]
    public float baseHp = 50f;
    public float baseDamage = 5f;
    public float attackRange = 3f;
    public float attackSpeed = 1f; // số lần đánh / giây
    public float moveSpeed = 4f;

    [Header("Nâng cấp trong run")]
    public int maxLevel = 5;
    public float hpPerLevel = 10f;
    public float damagePerLevel = 2f;

    [Header("Mô tả (hiện khi player chọn)")]
    [TextArea(2, 4)]
    public string description;
}

public enum NpcRole
{
    Tank,
    MeleeDPS,
    RangedDPS,
    Support
}