using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Compass : ItemBase
{
    [SerializeField]
    private Transform _player;
    [SerializeField]
    private Transform _pin;

    private Vector3 _goalPos;

    protected override void Init()
    {
        // ÉSÅ[ÉãÇÃç¿ïWÇéÊìæ
        _goalPos = StageManager.instance.GetKeyRoomPosition();
    }

    protected override void Proc()
    {
        TurnTarget3D(_goalPos);
    }

    private void TurnTarget3D(Vector3 targetPos)
    {
        Vector3 dir = targetPos - _player.position;
        dir.y = _player.position.y;
        Quaternion dirRot = Quaternion.LookRotation(dir);
        _pin.rotation = dirRot;
    }
}
