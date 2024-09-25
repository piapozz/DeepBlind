using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientManager : MonoBehaviour
{
    // �󂯎�Ƒ����𕪂��Ĕ����bool�ŕԂ� bool = ���m����
    // Receiver�ɉ��𕷂����̏������� Sender�ɉ���炵�Ă��鑤�̏�������
    public bool CheckHitAmbient(Vector3 receiverPosition, float receiverRange, Vector3 senderPos, float senderRange)
    {
        // �v�Z�p�̕ϐ�
        Vector2 receiverTemp = new Vector2(receiverPosition.x, receiverPosition.z);
        Vector2 senderTemp = new Vector2(senderPos.x, senderPos.z);

        // �O�����̒藝���g���ċ������o��
        float width = Mathf.Pow(receiverTemp.x - senderTemp.x, 2);
        float height = Mathf.Pow(receiverTemp.y - senderTemp.y, 2);

        // �������v�Z
        float radius = Mathf.Sqrt(width) + Mathf.Sqrt(height);

        // �������r����
        if(radius <= receiverRange + senderRange)
        {
            // �͈͂ɓ����Ă�����true
            return true;
        }

        // �͈͊O��������false
        return false;
    }
}
