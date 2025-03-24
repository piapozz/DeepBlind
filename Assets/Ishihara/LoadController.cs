using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadController : MonoBehaviour
{
    [SerializeField] GameObject loadObject;
    [SerializeField] GameObject completedObject;

    [SerializeField] StageManager stageManager;

    // Start is called before the first frame update
    void Start()
    {
        // �X�e�[�W�̐���
        loadObject.SetActive(true);
        
        // �I�������R���v���[�g�I�u�W�F�N�g��\��
        loadObject.SetActive(false);
        completedObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && completedObject.activeSelf)
            FadeSceneChange.instance.ChangeSceneEvent("Main");
    }
}
