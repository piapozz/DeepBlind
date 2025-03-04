/*
* @file SoundObjectMoveObserver.cs
* @brief �����̃I�u�U�[�o�[
* @author sakakura
* @date 2025/2/8
*/

using UnityEngine;

public interface SoundObjectObserver
{
    // ���W���X�V����
    public void UpdatePosition(int ID, Vector3 position);

    public void SetRing(int ID, bool ring);
}
