/*
* @file Section.cs
* @brief ���I�u�W�F�N�g
* @author sakakura
* @date 2025/3/17
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SectionObject : MonoBehaviour
{
    /// <summary>�G�l�~�[�̃A���J�[</summary>
    [SerializeField]
    private List<Transform> _enemyAnchor = null;

    [SerializeField]
    private List<Transform> _itemAnchor = null;

    [SerializeField]
    private List<Transform> _lockerAnchor = null;

    /// <summary>
    /// �G�l�~�[�̃A���J�[�̎擾
    /// </summary>
    /// <returns></returns>
    public List<Transform> GetEnemyAnchor()
    {
        return _enemyAnchor;
    }

    /// <summary>
    /// �A�C�e���̃A���J�[�̎擾
    /// </summary>
    /// <returns></returns>
    public List<Transform> GetItemAnchor()
    {
        return _itemAnchor;
    }

    /// <summary>
    /// ���b�J�[�̃A���J�[�̎擾
    /// </summary>
    /// <returns></returns>
    public List<Transform> GetLockerAnchor()
    {
        return _lockerAnchor;
    }
}
