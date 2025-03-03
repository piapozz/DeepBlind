using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

// スモーク
// ドア固め
// ロッカーワープ

// エネミーの元となる親クラス

public abstract class EnemyBase : MonoBehaviour
{
    protected static System.Func<int, GameObject> _GetObject = null;

    public static void SetGetObjectCallback(System.Func<int, GameObject> setCallback)
    {
        _GetObject = setCallback;
    }

    public int ID               { get; protected set; } = -1;
    private int _masterID = -1;
    public Vector3 Position     { get; protected set; } = Vector3.zero;

    public float speed          { get; protected set; } = -1;       // エネミーの速さ
    public float speedDiameter  { get; protected set; } = -1;       // 見つけた時の速さの倍率
    public float threatRange    { get; protected set; } = -1;       // 脅威範囲
    public float viewLength     { get; protected set; } = -1;       // 視界の長さ
    public float fieldOfView    { get; protected set; } = -1;       // 視野角

    // それぞれのステートクラス
    protected ISeach seach;
    protected IVigilance vigilance;
    protected ITracking tracking;

    // それぞれのスキルクラス
    protected ISkill skill;

    protected static System.Action<State> _ChangeState = null;

    private IEnemyState _state;

    public virtual void Setup(int setID, Vector3 position, int masterID)
    {
        ID = setID;
        _masterID = masterID;
        _GetObject(ID).transform.position = position;
        _GetObject(ID).SetActive(true);
        ResetStatus();
        _ChangeState = StateChange;
    }

    /// <summary>
    /// ステータス初期化
    /// </summary>
    public virtual void ResetStatus()
    {
        //var characterMaster = CharacterMasterUtility.GetCharacterMaster(_masterID);
        //if (characterMaster == null) return;

        //SetSpeed(characterMaster.Speed);
        //SetSpeedDiameter(characterMaster.SpeedDiameter);
        //SetThreatRange(characterMaster.ThreatRange);
        //SetViewLength(characterMaster.ViewLength);
        //SetFieldOfView(characterMaster.FieldOfView);
        //SetSeach(characterMaster.Seach);
        //SetVigilance(characterMaster.Vigilance);
        //SetTracking(characterMaster.Tracking);
        //SetSkill(characterMaster.Skill);
    }

    public void Teardown()
    {
        _GetObject(ID).SetActive(false);
        ID = -1;
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

    public void SetSeach(ISeach setSeach)
    {
        seach = setSeach;
    }

    public void SetVigilance(IVigilance setVigilance)
    {
        vigilance = setVigilance;
    }

    public void SetTracking(ITracking setTracking)
    {
        tracking = setTracking;
    }

    public void SetSkill(ISkill setSkill)
    {
        skill = setSkill;
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
    public void MoveNavAgent()
    {
        // 目標位置を設定
        //_enemyAgent.SetDestination(myInfo.status.targetPos);
    }

    public void Active()
    {
        _state.Activity();

        skill.Ability();
    }

    /// <summary>
    /// ステートとスキルの切り替え処理
    /// </summary>
    /// <param name="state"></param>
    public void StateChange(State state)
    {
        // ステート、スキルを切り替える
        switch (state)
        {
            case State.SEARCH:

                _state = seach;
                break;

            case State.VIGILANCE:

                _state = vigilance;
                break;

            case State.TRACKING:

                _state = tracking;
                break;
        }

        // ステート、スキルの初期化
        skill.Init();
        _state.Init();
    }
}