using UnityEngine;

[CreateAssetMenu(fileName = "ItemObject", menuName = "ScriptableObjects/ItemData")]

public class ItemData : ScriptableObject
{
    [SerializeField] public string itemName;        // アイテムの名前
    [SerializeField] public Sprite icon;            // アイコンのスプライトデータ
    [SerializeField] public bool canStack;          // スタックできるかどうか
    [SerializeField] public bool isConsume;         // 消費するアイテムかどうか
    [SerializeField] public bool isPassive;         // 持つだけのアイテムかどうか
}
