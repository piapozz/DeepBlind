using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/EnemyPrefabs")]
public class EnemyPrefabs : ScriptableObject
{
    public List<GameObject> enemies = new List<GameObject>();
}
