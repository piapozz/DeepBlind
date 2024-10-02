using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyAnimationBase : MonoBehaviour
{
    [SerializeField] GameObject animatorObj;

    // �A�j���[�V����
    public enum BoolAnimation
    {
        WALKING,        // ���s
        RUNNING,        // ���s
        SKILL,          // �X�L���g�p��

        MAX
    }

    // �A�j���[�V����
    public enum TriggerAnimation
    {
        SCREAM,         // ����
        LOOKING,        // ���n��

        MAX
    }

    protected string[] boolAnimation;       // bool�p�����[�^�[
    protected string[] triggerAnimation;    // trigger�p�����[�^�[

    Animator animator;                      // �A�j���[�^�[

    // Start is called before the first frame update
    void Start()
    {
        // �A�j���[�^�[�擾
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
