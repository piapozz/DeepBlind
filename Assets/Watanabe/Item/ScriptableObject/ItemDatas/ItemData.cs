using UnityEngine;

[CreateAssetMenu(fileName = "ItemObject", menuName = "ScriptableObjects/ItemData")]

public class ItemData : ScriptableObject
{
    /// <summary>
    /// アイテムの基本情報
    /// </summary>
    [SerializeField] public GameObject itemModel;   // アイテムのモデルデータ
    [SerializeField] public Sprite itemIcon;        // アイコンのスプライトデータ
    [SerializeField] public string itemName;        // アイテムの名前

    /// <summary>
    /// アイテムの設定
    /// </summary>
    [SerializeField] public bool canStack;          // スタックできるかどうか
    [SerializeField] public bool isConsume;         // 消費するアイテムかどうか
    [SerializeField] public bool isPassive;         // 持つだけのアイテムかどうか
}
