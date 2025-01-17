/*
 * @file Interact.cs
 * @brief �C���^���N�g�������ꂽ�Ƃ��ɓK�؂ȏ��������s����@�\������
 * @author sein
 * @date 2025/1/17
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interact : MonoBehaviour
{
    private IEvent iEvent;

    bool inputStay = false;                 // ���͂��󂯕t���邩���Ǘ�����

    public bool interact = false;

    // Actions��Fire�ɓo�^����Ă���L�[�������ꂽ�Ƃ��ɓ��͒l���擾
    public void OnFire(InputValue inputValue)
    {
        if (inputStay == true) interact = true;
    }

    void OnTriggerStay(Collider other)
    {
        // �A�^�b�`����Ă���C�x���g���擾
        iEvent = other.GetComponent<IEvent>();

        // �C�x���g���������珈�������s
        if (iEvent != null)
        {
            // �C���^���N�g�̓��͂��󂯕t����悤�ɂ���
            inputStay = true;

            // �K����UI��`�悷��
            iEvent.EnableInteractUI();
            
            // �v���C���[���C���^���N�g���N��������
            if (interact == true)
            {
                // �Ή�����C�x���g���s
                iEvent.Event();
            }

            // �C���^���N�g�𖳌��ɂ���
            interact = false;
        }
    }

    void OnTriggerExit(Collider other)
    {
        // �C���^���N�g�̓��͎�t�𖳌�������
        inputStay = false;

        // �C�x���g���擾
        iEvent = other.GetComponent<IEvent>();

        // �C�x���g����������UI�̔�\�������s
        if(iEvent != null) iEvent.DisableInteractUI();
    }
}
