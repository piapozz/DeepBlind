using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compass : ItemBase
{
    [SerializeField]
    private Transform _pin;

    private Transform _player;
    private Vector3 _goalPos;

    public override bool Effect()
    {
        return false;
    }

    public override void Initialize()
    {
        base.Initialize();

        _player = Player.instance.transform;
        // ゴールの座標を取得
        _goalPos = StageManager.instance.GetKeyRoomPosition();
    }

    public override void Proc()
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
