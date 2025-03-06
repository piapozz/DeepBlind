using Cinemachine;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// エネミーの元となる親クラス
public class EnemyBase : MonoBehaviour
{
    private static System.Func<int, GameObject> _GetObject = null;

    public static void SetGetObjectCallback(Func<int, GameObject> setCallback)
    {
        _GetObject = setCallback;
    }

    public int ID { get; private set; } = -1;
    private int _masterID = -1;
    public Vector3 target { get; private set; }

    public float speed { get; private set; } = -1;       // エネミーの速さ
    public float speedDiameter { get; private set; } = -1;       // 見つけた時の速さの倍率
    public float threatRange { get; private set; } = -1;       // 脅威範囲
    public float viewLength { get; private set; } = -1;       // 視界の長さ
    public float fieldOfView { get; private set; } = -1;       // 視野角

    // それぞれのステートクラス
    private ISeach _seach;
    private IVigilance _vigilance;
    private ITracking _tracking;

    // それぞれのスキルクラス
    private ISkill _skill;
    private List<IEnemyState> _state;
    [SerializeField]
    public State _nowState { get; private set; }

    // ナビメッシュ
    private NavMeshAgent _agent;

    // アニメーター
    private Animator _animator;

    // 3Dオーディオソース
    private AudioSource _audioSource;

    public CinemachineVirtualCamera _camera { get; private set; } = null;

    public virtual void Setup(int setID, Vector3 position, int masterID)
    {
        ID = setID;
        _masterID = masterID;
        GameObject obj = _GetObject(ID);
        if (obj == null) return;

        obj.transform.position = position;
        obj.SetActive(true);
        ResetStatus();

        // ステートの初期化
        int stateMax = (int)State.MAX;
        _state = new List<IEnemyState>(stateMax);

        if (_seach != null) _state.Add(_seach);
        if (_vigilance != null) _state.Add(_vigilance);
        if (_tracking != null) _state.Add(_tracking);

        foreach (var state in _state)
        {
            state.Init(ID);
        }

        _nowState = State.SEARCH;
        _skill?.Init(ID);

        _agent = obj.GetComponent<NavMeshAgent>();
        _animator = obj.GetComponent<Animator>();
        _audioSource = obj.GetComponent<AudioSource>(); 
        _camera = obj.GetComponentInChildren<CinemachineVirtualCamera>();
        _agent.speed = speed;
        target = position;
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
        _GetObject(ID)?.SetActive(false);
        ID = -1;
        _state?.Clear();
    }

    public void SetSpeed(float setSpeed) => speed = setSpeed;
    public void SetSpeedDiameter(float setSpeedDiameter) => speedDiameter = setSpeedDiameter;
    public void SetThreatRange(float setThreatRange) => threatRange = setThreatRange;
    public void SetViewLength(float setViewLength) => viewLength = setViewLength;
    public void SetFieldOfView(float setFieldOfView) => fieldOfView = setFieldOfView;

    public void SetSeach(string setSeach)
    {
        Type type = Type.GetType(setSeach);
        if (type != null)
        {
            _seach = Activator.CreateInstance(type) as ISeach;
        }
    }

    public void SetVigilance(string setVigilance)
    {
        Type type = Type.GetType(setVigilance);
        if (type != null)
        {
            _vigilance = Activator.CreateInstance(type) as IVigilance;
        }
    }

    public void SetTracking(string setTracking)
    {
        Type type = Type.GetType(setTracking);
        if (type != null)
        {
            _tracking = Activator.CreateInstance(type) as ITracking;
        }
    }

    public void SetSkill(string setSkill)
    {
        Type type = Type.GetType(setSkill);
        if (type != null)
        {
            _skill = Activator.CreateInstance(type) as ISkill;
        }
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
        if (_agent != null)
        {
            target = targetPosition;
            _agent.SetDestination(target);
        }
    }

    public void Active()
    {
        _state?[(int)_nowState]?.Activity();
        _skill?.Ability();
    }

    /// <summary>
    /// ステートとスキルの切り替え処理
    /// </summary>
    public void StateChange(State state)
    {
        if (state == State.TRACKING) EnemyUtility.GetPlayer().EnemyFound();
        _nowState = state;
    }

    /// <summary>
    /// アニメーションの速度変更
    /// </summary>
    public void SetAnimationSpeed(float speed)
    {
        if (_animator != null)
        {
            _animator.speed = speed;
        }
    }

    /// <summary>
    /// ナビメッシュの速度変更
    /// </summary>
    public void SetNavSpeed(float speed)
    {
        if (_agent != null)
        {
            _agent.speed = speed;
        }
    }

    // プレイヤーと接触したかどうか
    public bool ChackCaughtPlayer()
    {
        return Vector3.Distance(transform.position, EnemyUtility.GetPlayer().transform.position) < 1.5f;
    }
}
