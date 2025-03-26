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
    private List<Transform> _playerAnchor = new List<Transform>();
    /// <summary>エネミーのアンカー</summary>
    [SerializeField]
    private List<Transform> _enemyAnchor = new List<Transform>();
    /// <summary>アイテムのアンカー</summary>
    [SerializeField]
    private List<Transform> _itemAnchor = new List<Transform>();
    /// <summary>ロッカーのアンカー</summary>
    [SerializeField]
    private List<Transform> _lockerAnchor = new List<Transform>();

    /// <summary>
    /// プレイヤーのアンカーの取得
    /// </summary>
    /// <returns></returns>
    public List<Transform> GetPlayerAnchor()
    {
        return _playerAnchor;
    }

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
