using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyAnimationBase : MonoBehaviour
{
    [SerializeField] GameObject animatorObj;

    // アニメーション
    public enum BoolAnimation
    {
        WALKING,        // 歩行
        RUNNING,        // 走行
        SKILL,          // スキル使用時

        MAX
    }

    // アニメーション
    public enum TriggerAnimation
    {
        SCREAM,         // 発見
        LOOKING,        // 見渡し

        MAX
    }

    protected string[] boolAnimation;       // boolパラメーター
    protected string[] triggerAnimation;    // triggerパラメーター

    Animator animator;                      // アニメーター

    // Start is called before the first frame update
    void Start()
    {
        // アニメーター取得
        animator = animatorObj.GetComponent<Animator>();


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Init()
    {

    }


}
