/*
 * @file Interact.cs
 * @brief �C���^���N�g�������ꂽ�Ƃ��ɓK�؂ȏ��������s����@�\������
 * @author sein
 * @date 2025/1/17
 */

using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interact : MonoBehaviour
{
    private IEvent iEvent;

    public void ExecuteEvent(InputAction.CallbackContext context)
    {
        // ��x���ݓ��͂���t
        if (!context.performed) return;
        if (iEvent == null) return;
        iEvent.Event();
    }

    void OnTriggerEnter(Collider other)
    {
        // �A�^�b�`����Ă���C�x���g���擾
        IEvent triggerEvent = other.GetComponent<IEvent>();
        if (triggerEvent == null) return;
        iEvent = triggerEvent;
        iEvent.EnableInteractUI();
    }

    void OnTriggerExit(Collider other)
    {
        IEvent triggerEvent = other.GetComponent<IEvent>();
        if (triggerEvent == null) return;
        // �C�x���g����������UI�̔�\�������s
        iEvent.DisableInteractUI();
        iEvent = null;
    }
}
