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
    /// <summary>���̃I�u�W�F�N�g</summary>
    [SerializeField]
    private GameObject _sectionObject = null;
    /// <summary>�v���C���[�̃A���J�[</summary>
    [SerializeField]
    private Transform[] _playerAnchor = null;
    /// <summary>�G�l�~�[�̃A���J�[</summary>
    [SerializeField]
    private Transform[] _enemyAnchor = null;
    /// <summary>�A�C�e���̃A���J�[</summary>
    [SerializeField]
    private Transform[] _itemAnchor = null;
    /// <summary>���b�J�[�̃A���J�[</summary>
    [SerializeField]
    private Transform[] _lockerAnchor = null;

    /// <summary>
    /// �v���C���[�̃A���J�[�̎擾
    /// </summary>
    /// <returns></returns>
    public List<Transform> GetPlayerAnchor()
    {
        List<Transform> list = new List<Transform>(_playerAnchor);
        return list;
    }

    /// <summary>
    /// �G�l�~�[�̃A���J�[�̎擾
    /// </summary>
    /// <returns></returns>
    public List<Transform> GetEnemyAnchor()
    {
        List<Transform> list = new List<Transform>(_enemyAnchor);
        return list;
    }

    /// <summary>
    /// �A�C�e���̃A���J�[�̎擾
    /// </summary>
    /// <returns></returns>
    public List<Transform> GetItemAnchor()
    {
        List<Transform> list = new List<Transform>(_itemAnchor);
        return list;
    }

    /// <summary>
    /// ���b�J�[�̃A���J�[�̎擾
    /// </summary>
    /// <returns></returns>
    public List<Transform> GetLockerAnchor()
    {
        List<Transform> list = new List<Transform>(_lockerAnchor);
        return list;
    }

    /// <summary>
    /// ���̕\����؂�ւ���
    /// </summary>
    /// <param name="visible"></param>
    public void SetVisible(bool visible)
    {
        _sectionObject.SetActive(visible);
    }

    /// <summary>
    /// �Еt��
    /// </summary>
    public void Teardown()
    {
        Destroy(gameObject);
    }
}
