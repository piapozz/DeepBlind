/*
* @file Section.cs
* @brief 区画オブジェクト
* @author sakakura
* @date 2025/3/17
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SectionObject : MonoBehaviour
{
    /// <summary>エネミーのアンカー</summary>
    [SerializeField]
    private List<Transform> _enemyAnchor = null;

    [SerializeField]
    private List<Transform> _itemAnchor = null;

    [SerializeField]
    private List<Transform> _lockerAnchor = null;

    /// <summary>
    /// エネミーのアンカーの取得
    /// </summary>
    /// <returns></returns>
    public List<Transform> GetEnemyAnchor()
    {
        return _enemyAnchor;
    }

    /// <summary>
    /// アイテムのアンカーの取得
    /// </summary>
    /// <returns></returns>
    public List<Transform> GetItemAnchor()
    {
        return _itemAnchor;
    }

    /// <summary>
    /// ロッカーのアンカーの取得
    /// </summary>
    /// <returns></returns>
    public List<Transform> GetLockerAnchor()
    {
        return _lockerAnchor;
    }
}
