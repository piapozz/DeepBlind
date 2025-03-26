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
    private List<Transform> _playerAnchor = new List<Transform>();
    /// <summary>�G�l�~�[�̃A���J�[</summary>
    [SerializeField]
    private List<Transform> _enemyAnchor = new List<Transform>();
    /// <summary>�A�C�e���̃A���J�[</summary>
    [SerializeField]
    private List<Transform> _itemAnchor = new List<Transform>();
    /// <summary>���b�J�[�̃A���J�[</summary>
    [SerializeField]
    private List<Transform> _lockerAnchor = new List<Transform>();

    /// <summary>
    /// �v���C���[�̃A���J�[�̎擾
    /// </summary>
    /// <returns></returns>
    public List<Transform> GetPlayerAnchor()
    {
        return _playerAnchor;
    }

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
