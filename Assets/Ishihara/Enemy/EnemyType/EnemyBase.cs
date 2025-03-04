using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

// スモーク
// ドア固め
// ロッカーワープ

// エネミーの元となる親クラス

public  class EnemyBase : MonoBehaviour
{
    private static System.Func<int, GameObject> _GetObject = null;

    public static void SetGetObjectCallback(System.Func<int, GameObject> setCallback)
    {
        _GetObject = setCallback;
    }

    public int ID               { get; private set; } = -1;
    private int _masterID = -1;
    public Transform transform  { get; private set; } = null;
    
    public float speed          { get; private set; } = -1;       // エネミーの速さ
    public float speedDiameter  { get; private set; } = -1;       // 見つけた時の速さの倍率
    public float threatRange    { get; private set; } = -1;       // 脅威範囲
    public float viewLength     { get; private set; } = -1;       // 視界の長さ
    public float fieldOfView    { get; private set; } = -1;       // 視野角

    // それぞれのステートクラス
    private ISeach _seach;
    private IVigilance _vigilance;
    private ITracking _tracking;

    // それぞれのスキルクラス
    private ISkill _skill;
    private List<IEnemyState> _state;
    private State _nowState;

    // ナビメッシュ
    private NavMeshAgent _agent;

    private static System.Action<State> _ChangeState = null;

    public virtual void Setup(int setID, Vector3 position, int masterID)
    {
        ID = setID;
        _masterID = masterID;
        _GetObject(ID).transform.position = position;
        _GetObject(ID).SetActive(true);
        ResetStatus();
        _ChangeState = StateChange;

        //ステートの初期化
        int stateMax = (int)State.MAX;
        _state = new List<IEnemyState>(stateMax);
        _state.Add(_seach);
        _state.Add(_vigilance);
        _state.Add(_tracking);
        for (int i = 0, max = stateMax; i < max; i++)
        {
            _state[i].Init(ID);
        }
        _nowState = State.SEARCH;

        _skill.Init();

        _agent = _GetObject(ID).GetComponent<NavMeshAgent>();
    }

    /// <summary>
    /// ステータス初期化
    /// </summary>
    public virtual void ResetStatus()
    {
        var characterMaster = CharacterMasterUtility.GetCharacterMaster(_masterID);
        if (characterMaster == null) return;

        SetSpeed(characterMaster.Speed);
        SetSpeedDiameter(characterMaster.SpeedDiameter);
        SetThreatRange(characterMaster.ThreatRange);
        SetViewLength(characterMaster.ViewLength);
        SetFieldOfView(characterMaster.FieldOfView);
        SetSeach(characterMaster.Seach);
        SetVigilance(characterMaster.Vigilance);
        SetTracking(characterMaster.Tracking);
        SetSkill(characterMaster.Skill);
    }

    public void Teardown()
    {
        _GetObject(ID).SetActive(false);
        ID = -1;
        _state.Clear();
    }

    public void SetSpeed(float setSpeed)
    {
        speed = setSpeed;
    }

    public void SetSpeedDiameter(float setSpeedDiameter)
    {
        speedDiameter = setSpeedDiameter;
    }

    public void SetThreatRange(float setThreatRange)
    {
        threatRange = setThreatRange;
    }

    public void SetViewLength(float setViewLength)
    {
        viewLength = setViewLength;
    }

    public void SetFieldOfView(float setFieldOfView)
    {
        fieldOfView = setFieldOfView;
    }

    public void SetSeach(string setSeach)
    {
        Type type = Type.GetType(setSeach);
        _seach = (ISeach)type;
    }

    public void SetVigilance(string setVigilance)
    {
        Type type = Type.GetType(setVigilance);
        _vigilance = (IVigilance)type;
    }

    public void SetTracking(string setTracking)
    {
        Type type = Type.GetType(setTracking);
        _tracking = (ITracking)type;
    }

    public void SetSkill(string setSkill)
    {
        Type type = Type.GetType(setSkill);
        _skill = (ISkill)type;
    }

    // ステート
    public enum State
    {
        SEARCH,        // 探索
        VIGILANCE,     // 警戒
        TRACKING,      // 追跡

        MAX
    }

    /// <summary>
    /// ナビメッシュで移動する
    /// </summary>
    public void SetNavTarget(Vector3 targetPosition)
    {
        // 目標位置を設定
        _agent.SetDestination(targetPosition);
    }

    public void Active()
    {
        _state[(int)_nowState].Activity();

        _skill.Ability();
    }

    /// <summary>
    /// ステートとスキルの切り替え処理
    /// </summary>
    /// <param name="state"></param>
    public void StateChange(State state)
    {
        _nowState = state;
    }
}