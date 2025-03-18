using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.GraphView.GraphView;

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
        // ÉSÅ[ÉãÇÃç¿ïWÇéÊìæ
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
