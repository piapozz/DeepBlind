using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadController : MonoBehaviour
{
    [SerializeField] GameObject loadObject;
    [SerializeField] GameObject completedObject;

    [SerializeField]
    private StageManager _stageManager;

    private UniTask _task;

    private async void Start()
    {
        await Initialize();
    }

    private async UniTask Initialize()
    {
        // �}�X�^�[�f�[�^�̃��[�h
        MasterDataManager.LoadAllData();
        // �X�e�[�W�̐���
        loadObject.SetActive(true);
        StageManager stageManager = Instantiate(_stageManager);
        DontDestroyOnLoad(stageManager);

        await stageManager.Initialize();

        // �I�������R���v���[�g�I�u�W�F�N�g��\��
        loadObject.SetActive(false);
        completedObject.SetActive(true);

        await UniTask.DelayFrame(1);
    }


    public void CompletedEvent()
    {
        if (completedObject.activeSelf)
            FadeSceneChange.instance.ChangeSceneEvent("Main");
    }
}
