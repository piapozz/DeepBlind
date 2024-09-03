using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateItem : MonoBehaviour
{
    [SerializeField] GameObject key;            // ��
    [SerializeField] GameObject map;            // �}�b�v
    [SerializeField] GameObject compass;        // �R���p�X
    [SerializeField] GameObject[] itemObj;      // �����_����������A�C�e��

    GameObject[] itemPlace;
    List<Transform> itemPlaceList = new List<Transform>();

    float itemProbability = 0.5f;       // �A�C�e�������m��

    void Start()
    {
        Init();

        // ���𐶐�
        GenerateKey();
        // �n�}�𐶐�
        GenerateMap();
        // �R���p�X�𐶐�
        GenerateCompass();

        // ���̃A�C�e���𐶐�����
        GenerateRandomItem();
    }

    // ������
    void Init()
    {
        itemPlace = GameObject.FindGameObjectsWithTag("Item");

        // �ϒ��z��ɓ����
        for (int i = 0; i < itemPlace.Length; i++)
        {
            itemPlaceList.Add(itemPlace[i].transform);
        }
    }

    // ���𐶐�����֐�
    void GenerateKey()
    {
        int rand = Random.Range(0, itemPlaceList.Count);

        Instantiate(key, itemPlaceList[rand].position, Quaternion.Euler(itemPlaceList[rand].eulerAngles), itemPlaceList[rand].transform);

        itemPlaceList.RemoveAt(rand);
    }

    // �n�}�𐶐�����֐�
    void GenerateMap()
    {
        int rand = Random.Range(0, itemPlaceList.Count);

        Instantiate(map, itemPlaceList[rand].position, Quaternion.Euler(itemPlaceList[rand].eulerAngles), itemPlaceList[rand].transform);

        itemPlaceList.RemoveAt(rand);
    }

    // �R���p�X�𐶐�����֐�
    void GenerateCompass()
    {
        int rand = Random.Range(0, itemPlaceList.Count);

        Instantiate(compass, itemPlaceList[rand].position, Quaternion.Euler(itemPlaceList[rand].eulerAngles), itemPlaceList[rand].transform);

        itemPlaceList.RemoveAt(rand);
    }

    // �����_���ȃA�C�e���𐶐�����֐�
    void GenerateRandomItem()
    {
        // �����ꏊ�����[��
        for (int i = 0; i < itemPlaceList.Count; i++)
        {
            // �A�C�e���𐶐����邩�������_���Ɍ��߂�
            if (Random.value >= itemProbability) continue;

            // ��������I�u�W�F�N�g�������_���Ɍ��߂�
            int rand = Random.Range(0, itemObj.Length);

            // �A�C�e������
            Instantiate(itemObj[rand], itemPlaceList[i].position, Quaternion.Euler(itemPlaceList[i].eulerAngles), itemPlaceList[i]);
        }
    }
}
