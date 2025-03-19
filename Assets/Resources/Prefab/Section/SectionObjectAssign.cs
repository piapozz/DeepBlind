using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SectionObjectAssign : ScriptableObject
{
    public GameObject[] roomObjectList = null;

    public GameObject startRoom = null;

    public GameObject keyRoom = null;

    public GameObject[] corridorObjectList = null;

    public GameObject wallObject = null;

    public GameObject doorObject = null;

    public GameObject frameObject = null;
}
