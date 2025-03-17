using UnityEngine;

[CreateAssetMenu(fileName = "ItemObject", menuName = "ScriptableObjects/ItemData")]

public class ItemData : ScriptableObject
{
    [SerializeField] public string itemName;        // �A�C�e���̖��O
    [SerializeField] public Sprite icon;            // �A�C�R���̃X�v���C�g�f�[�^
    [SerializeField] public bool canStack;          // �X�^�b�N�ł��邩�ǂ���
    [SerializeField] public bool isConsume;         // �����A�C�e�����ǂ���
    [SerializeField] public bool isPassive;         // �������̃A�C�e�����ǂ���
}
