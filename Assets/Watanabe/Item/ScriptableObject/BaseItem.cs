using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseItem : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public bool canStack;

    public abstract void ItemEffect(GameObject character);
}