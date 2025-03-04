using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemList", menuName = "ItemList")]
public class ItemList : ScriptableObject
{
    [SerializeField] private List<BaseItem> itemList;
}
