using Cinemachine;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.EventSystems.EventTrigger;

/// <summary>
/// エネミーの基底クラス
/// </summary>
public class EnemyBase : MonoBehaviour
{
    public enum State
    {
        SEARCH,
        VIGILANCE,
        TRACKING,
        MAX
    }

    private static Func<int, GameObject> _GetObject = null;

    public static void SetGetObjectCallback(Func<int, GameObject> setCallback)
    {
        _GetObject = setCallback;
    }

    public int ID { get; private set; } = -1;               // ID
    private int _masterID = -1;                             // マスターID
    public Vector3 target { get; private set; }             // ターゲット座標

    public float speed { get; private set; } = -1;          // 移動速度
    public float speedDiameter { get; private set; } = -1;  // 移動速度の倍率
    public float threatRange { get; private set; } = -1;    // 脅威範囲
    public float viewLength { get; private set; } = -1;     // 視界距離
    public float fieldOfView { get; private set; } = -1;    // 視野角度
    public bool isAbility { get; private set; } = false;    // スキル使用中か
    public List<Transform> searchAnchorList { get; private set; } = null;    // 索敵アンカーリスト

    private ISearch _search;              // 索敵
    private IVigilance _vigilance;      // 警戒
    private ITracking _tracking;        // 追跡
    private ISkill _skill;              // スキル
    private List<IEnemyState> _state;   // ステート

    private bool caught = false;        // プレイヤーを捕まえたか

    public State _nowState { get; private set; }    // 現在のステート

    private NavMeshAgent _agent;        // ナビメッシュ
    private Animator _animator;         // アニメーター
    public CinemachineVirtualCamera _camera { get; private set; } = null;       // 捕獲時のカメラ

    /// <summary>
    /// 初期設定
    /// </summary>
    /// <param name="setID"></param>
    /// <param name="position"></param>
    /// <param name="masterID"></param>
    public virtual void Setup(int setID, Vector3 position, int masterID)
    {
        // 初期設定
        ID = setID;
        _masterID = masterID;
        GameObject obj = _GetObject(ID);
        if (obj == null) return;
        // 位置設定
        // 取得
        _agent = obj.GetComponent<NavMeshAgent>();
        _animator = obj.GetComponent<Animator>();
        _camera = obj.GetComponentInChildren<CinemachineVirtualCamera>();
        _agent.enabled = false;
        obj.SetActive(true);
        obj.transform.position = position;
        ResetStatus();
        _agent.enabled = true;

        // ステート設定
        int stateMax = (int)State.MAX;
        _state = new List<IEnemyState>(stateMax);
        if (_search != null) _state.Add(_search);
        if (_vigilance != null) _state.Add(_vigilance);
        if (_tracking != null) _state.Add(_tracking);
        // 現在のステート
        StateChange(State.SEARCH);
        _skill?.Init(ID);

        if (_agent != null) _agent.speed = speed;
        // ターゲット設定
        searchAnchorList = new List<Transform>();
        EnemyUtility.SetSearchAnchor(ID);
        if(!CommonModule.IsEnableIndex(searchAnchorList, 0)) return;
        target = searchAnchorList[0].position;
    }

    /// <summary>
    /// ステータスリセット
    /// </summary>
    public virtual void ResetStatus()
    {
        // マスターから取得
        var characterMaster = CharacterMasterUtility.GetCharacterMaster(_masterID);
        if (characterMaster == null) return;
        // ステータス設定
        SetSpeed(characterMaster.Speed);
        SetSpeedDiameter(characterMaster.SpeedDiameter);
        SetThreatRange(characterMaster.ThreatRange);
        SetViewLength(characterMaster.ViewLength);
        SetFieldOfView(characterMaster.FieldOfView);
        SetSearch(characterMaster.Search);
        SetVigilance(characterMaster.Vigilance);
        SetTracking(characterMaster.Tracking);
        SetSkill(characterMaster.Skill);
    }

    /// <summary>
    /// 終了処理
    /// </summary>
    public void Teardown()
    {
        _GetObject(ID)?.SetActive(false);
        ID = -1;
        _state?.Clear();
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    public void SetSpeed(float setSpeed) => speed = setSpeed;
    public void SetSpeedDiameter(float setSpeedDiameter) => speedDiameter = setSpeedDiameter;
    public void SetThreatRange(float setThreatRange) => threatRange = setThreatRange;
    public void SetViewLength(float setViewLength) => viewLength = setViewLength;
    public void SetFieldOfView(float setFieldOfView) => fieldOfView = setFieldOfView;

    public void SetSearch(string setSearch)
    {
        Type type = Type.GetType(setSearch);
        if (type != null)
        {
            _search = Activator.CreateInstance(type) as ISearch;
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

    /// <summary>
    /// ターゲット設定
    /// </summary>
    /// <param name="targetPosition"></param>
    public void SetNavTarget(Vector3 targetPosition)
    {
        if (_agent != null)
        {
            target = targetPosition;
            _agent.SetDestination(target);
        }
    }

    /// <summary>
    /// 実行処理
    /// </summary>
    public void Active()
    {
        _state?[(int)_nowState]?.Activity();
        _skill?.Ability();
    }

    /// <summary>
    /// ステート変更
    /// </summary>
    /// <param name="state"></param>
    public void StateChange(State state)
    {
        if (state == State.TRACKING) EnemyUtility.GetPlayer().EnemyFound();
        _nowState = state;
        _state?[(int)_nowState]?.Init(ID);
    }

    /// <summary>
    /// アニメーション速度設定
    /// </summary>
    /// <param name="speed"></param>
    public void SetAnimationSpeed(float speed)
    {
        if (_animator != null)
        {
            _animator.speed = speed;
        }
    }

    /// <summary>
    /// ナビメッシュ速度設定
    /// </summary>
    /// <param name="speed"></param>
    public void SetNavSpeed(float speed)
    {
        if (_agent != null)
        {
            if(speed == 0) _agent.velocity = Vector3.zero;
            _agent.speed = speed;
        }
    }

    /// <summary>
    /// スキル使用中設定
    /// </summary>
    public void SetIsAbility()
    {
        isAbility = !isAbility;
    }

    /// <summary>
    /// プレイヤーを捕まえたか
    /// </summary>
    /// <returns></returns>
    public bool CheckCaughtPlayer()
    {
        // 追跡のときだけ捕まえる
        if(_nowState != State.TRACKING) return false;

        float length = EnemyUtility.EnemyToPlayerLength(ID);

        caught = (length < 0.7 && !isAbility) || caught;

        return caught;
    }

    /// <summary>
    /// プレイヤーを捕まえた
    /// </summary>
    public void CaughtPlayer()
    {
        SetAnimationSpeed(1);
        SetSpeed(0);
        SetScreamTrigger();
    }

    /// <summary>
    /// 捕獲アニメーション再生
    /// </summary>
    public void SetScreamTrigger()
    {
        if (_animator != null)
        {
            _animator.SetTrigger("scream");
        }
    }

    public void SetSearchAnchor(List<Transform> setAnchor)
    {
        searchAnchorList = setAnchor;
    }

    public void ShowSelfData()
    {
        GameObject obj = _GetObject(ID);
        GUILayout.Label("ID: " + ID);
        GUILayout.Label("Name: " + obj.name);
        GUILayout.Label("SelfPosition: " + obj.transform.position);
        GUILayout.Label("State: " + _nowState.ToStateName());
        GUILayout.Label("IsAbilityt: " + isAbility);
    }
}
