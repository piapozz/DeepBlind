using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : SystemObject
{
    [SerializeField]
    private List<GameObject> _itemList = null;

    [SerializeField]
    private GameObject _lockerObject = null;

    [SerializeField]


    public override void Initialize()
    {
        GenerateItem();
        GenerateLocker();
    }

    private void GenerateItem()
    {

    }

    private void GenerateLocker()
    {

    }

    private void GenerateFlavorObject()
    {

    }
}
