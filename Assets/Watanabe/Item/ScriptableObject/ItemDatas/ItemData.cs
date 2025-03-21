using UnityEngine;

[CreateAssetMenu(fileName = "ItemObject", menuName = "ScriptableObjects/ItemData")]

public class ItemData : ScriptableObject
{
    /// <summary>
    /// �A�C�e���̊�{���
    /// </summary>
    [SerializeField] public GameObject itemModel;   // �A�C�e���̃��f���f�[�^
    [SerializeField] public Sprite itemIcon;        // �A�C�R���̃X�v���C�g�f�[�^
    [SerializeField] public string itemName;        // �A�C�e���̖��O

    /// <summary>
    /// �A�C�e���̐ݒ�
    /// </summary>
    [SerializeField] public bool canStack;          // �X�^�b�N�ł��邩�ǂ���
    [SerializeField] public bool isConsume;         // �����A�C�e�����ǂ���
    [SerializeField] public bool isPassive;         // �������̃A�C�e�����ǂ���
}
