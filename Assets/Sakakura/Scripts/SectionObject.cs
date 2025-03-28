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
    /// <summary>区画のオブジェクト</summary>
    [SerializeField]
    private GameObject _sectionObject = null;
    /// <summary>プレイヤーのアンカー</summary>
    [SerializeField]
    private Transform[] _playerAnchor = null;
    /// <summary>エネミーのアンカー</summary>
    [SerializeField]
    private Transform[] _enemyAnchor = null;
    /// <summary>アイテムのアンカー</summary>
    [SerializeField]
    private Transform[] _itemAnchor = null;
    /// <summary>ロッカーのアンカー</summary>
    [SerializeField]
    private Transform[] _lockerAnchor = null;

    /// <summary>
    /// プレイヤーのアンカーの取得
    /// </summary>
    /// <returns></returns>
    public List<Transform> GetPlayerAnchor()
    {
        List<Transform> list = new List<Transform>(_playerAnchor);
        return list;
    }

    /// <summary>
    /// エネミーのアンカーの取得
    /// </summary>
    /// <returns></returns>
    public List<Transform> GetEnemyAnchor()
    {
        List<Transform> list = new List<Transform>(_enemyAnchor);
        return list;
    }

    /// <summary>
    /// アイテムのアンカーの取得
    /// </summary>
    /// <returns></returns>
    public List<Transform> GetItemAnchor()
    {
        List<Transform> list = new List<Transform>(_itemAnchor);
        return list;
    }

    /// <summary>
    /// ロッカーのアンカーの取得
    /// </summary>
    /// <returns></returns>
    public List<Transform> GetLockerAnchor()
    {
        List<Transform> list = new List<Transform>(_lockerAnchor);
        return list;
    }

    /// <summary>
    /// 区画の表示を切り替える
    /// </summary>
    /// <param name="visible"></param>
    public void SetVisible(bool visible)
    {
        _sectionObject.SetActive(visible);
    }

    /// <summary>
    /// 片付け
    /// </summary>
    public void Teardown()
    {
        Destroy(gameObject);
    }
}
